using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Common;
using Lykke.Common.EncryptionTools;
using Lykke.Common.MasterPassword.Exceptions;
using Microsoft.AspNetCore.DataProtection;

namespace Lykke.Common.MasterPassword
{    
    /// <summary>
    /// Protects password with 3 keys encryption algrorithm
    /// </summary>
    public  class CompositePasswordProtectorService : ICompositePasswordProtectorService, IDisposable
    {
        private readonly RNGCryptoServiceProvider _cryptoProvider = new RNGCryptoServiceProvider();
        private readonly SHA256 _sha256Instance = SHA256.Create();

        private readonly IDataProtector _dataProtector;

        private byte[] _fullPasswordProtected;

        private readonly IDictionary<int, byte[]> _partStorageProtected;

        private readonly IPasswordValidationHashRepository _passwordValidationHashRepository;

        private readonly Random _random;
        
        private bool _passwordValidated;
        private int ValidationPartNumber =>_partStorageProtected.Keys.Max();

        public CompositePasswordProtectorService(IPasswordValidationHashRepository passwordValidationHashRepository,
            IDataProtector dataProtector, uint partsCount)
        {
            if (partsCount < 1)
            {
                throw new ArgumentException("Parts count should be greater than 1", nameof(partsCount));
            } 

            _passwordValidationHashRepository = passwordValidationHashRepository ?? throw new ArgumentNullException(nameof(dataProtector));
            _dataProtector = dataProtector ?? throw new ArgumentNullException(nameof(dataProtector));
            _random = new Random();

            _partStorageProtected = Enumerable.Range(1, (int) partsCount).ToDictionary(p => p, p => (byte[]) null);
        }
        
        public bool IsPasswordValid { get; protected set; }


        private byte[] GetFullPasswordBytesDecrypted()
        {
            if (!IsPasswordValid)
            {
                throw new MasterPasswordNotInitiatedException("Password is invalid");
            }

            return _dataProtector.Unprotect(_fullPasswordProtected);
        }

        public string GetFullPassword()
        {
            return GetFullPasswordBytesDecrypted().ToBase64();
        }

        public string Ecrypt(string input)
        {
           return Aes256Helper.Encrypt(input, GetFullPasswordBytesDecrypted());
        }

        public string Decrypt(string input)
        {
            try
            {
                return Aes256Helper.Decrypt(input, GetFullPasswordBytesDecrypted());
            }
            catch (FormatException e)
            {
                throw new MasterPasswordDecryptionException("Decryption failed", e);
            }
        }

        public async Task SetPasswordPartAsync(int partNumber, string partValue)
        {
            if (string.IsNullOrEmpty(partValue))
            {
                throw new ArgumentNullException(nameof(partValue));
            }

            if (!_partStorageProtected.ContainsKey(partNumber))
            {
                throw new ArgumentException(nameof(partNumber));
            }

            var passwordPartHash = partNumber != ValidationPartNumber ? ComputeHash(partValue) : Encoding.UTF8.GetBytes(partValue);

            partValue.OvewriteInMemory('*');

            // This delay is used to hide information about actual number of required password parts.
            await Task.Delay(_random.Next(100, 500));

            if (_partStorageProtected[partNumber] != null)
            {
                return;
            }

            _partStorageProtected[partNumber] = _dataProtector.Protect
            (
                plaintext: passwordPartHash
            );

            if (_partStorageProtected.All(p => p.Value != null))
            {
                var lastPartNumberBytes = _dataProtector.Unprotect(_partStorageProtected[ValidationPartNumber]);

                // When all password parts are set in any ored, we calculate master password
                _fullPasswordProtected = _dataProtector.Protect
                (
                    plaintext: ComputeHashUsingSha256
                    (
                        _partStorageProtected
                            .Where(p => p.Key != ValidationPartNumber)
                            .Select(p => _dataProtector.Unprotect(p.Value)).ToArray()
                    )
                );
                
                await ValidatePasswordAsync(lastPartNumberBytes);
            }
        }

        private async Task ValidatePasswordAsync(byte[] validationBytes)
        {
            if (_passwordValidated)
            {
                Array.Clear(validationBytes, 0, validationBytes.Length);
                return;
            }

            _passwordValidated = true;

            var actualValidationHash = ComputeHashUsingSha256
            (
                new [] { validationBytes }
                    .Concat(_partStorageProtected.Select(p => _dataProtector.Unprotect(p.Value)))
                    .ToArray()
            ).ToHexString();

            await ValidatePasswordHashes(actualValidationHash);
        }

        private async Task ValidatePasswordHashes(string actualValidationHash)
        {
            var exptectedValidationHashes = (await _passwordValidationHashRepository.GetAsync()).ToList();

            if (exptectedValidationHashes.Any())
            {
                IsPasswordValid = exptectedValidationHashes.Contains(actualValidationHash);
            }
            else
            {
                // Alongside with real validation phrase hash, we add 999 fake hashes, that a randomly sorted.
                // So, if inruder gets access to the database he will not know, which validation hash is real

                var validationHashes = new List<string>(1000)
                {
                    actualValidationHash
                };

                var randomBytes = new byte[32];

                while (validationHashes.Count < 1000)
                {
                    _cryptoProvider.GetBytes(randomBytes);

                    validationHashes.Add
                    (
                        ComputeHashUsingSha256(randomBytes).ToHexString()
                    );
                }

                validationHashes = validationHashes
                    .OrderBy(x => _random.Next())
                    .ToList();

                await _passwordValidationHashRepository.InsertAsync(validationHashes);

                IsPasswordValid = true;
            }
        }

        private byte[] ComputeHashUsingSha256(params byte[][] data)
        {
            return _sha256Instance.ComputeHash
            (
                data.SelectMany(x => x).ToArray()
            );
        }

        private byte[] ComputeHash(string data)
        {
            return ComputeHashUsingSha256
            (
                Encoding.UTF8.GetBytes(data)
            );
        }

        public void Dispose()
        {
            _sha256Instance.Dispose();
            _cryptoProvider.Dispose();
        }
    }
}
