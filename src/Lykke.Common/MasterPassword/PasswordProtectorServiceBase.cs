using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Common;
using Microsoft.AspNetCore.DataProtection;

namespace Lykke.Common.MasterPassword
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public abstract class PasswordProtectorServiceBase : IPasswordProtectorService
    {
        private static readonly RNGCryptoServiceProvider CryptoProvider = new RNGCryptoServiceProvider();
        private static readonly SHA256 SHA256Instance = SHA256.Create();
        protected readonly IDataProtector _dataProtector;
        protected readonly byte[][] _password;
        protected readonly IPasswordValidationHashRepository _passwordValidationHashRepository;
        protected readonly Random _random;
        protected readonly HashSet<int> _setPartNumbers;
        protected bool _passwordValidated;
        public bool IsPasswordValid { get; protected set; }


        public PasswordProtectorServiceBase(IPasswordValidationHashRepository passwordValidationHashRepository)
        {
            _dataProtector = CreateDataProtector();
            _passwordValidationHashRepository = passwordValidationHashRepository;
            _password = new byte[3][];
            _random = new Random();
            _setPartNumbers = new HashSet<int>();
        }

        public virtual async Task SetPasswordKeyAsync(int partNumber, string partValue)
        {
            var passwordPartHash = partNumber != 3 ? ComputeHash(partValue) : Encoding.UTF8.GetBytes(partValue);

            partValue.OvewriteInMemory('*');

            // This delay is used to hide information about actual number of required password parts.
            await Task.Delay(_random.Next(1000, 2000));

            if (_setPartNumbers.Contains(partNumber))
            {
                return;
            }

            _setPartNumbers.Add(partNumber);

            if (partNumber >= 1 && partNumber <= 2)
            {
                _password[partNumber] = _dataProtector.Protect
                (
                    plaintext: passwordPartHash
                );
            }
            else if (partNumber == 3)
            {
                _password[0] = _dataProtector.Protect
                (
                    plaintext: passwordPartHash
                );
            }

            if (_password[0] != null && _password[1] != null && _password[2] != null)
            {
                var partTheeBytes = _dataProtector.Unprotect(_password[0]);
                Array.Clear(_password, 0, 1);

                // When both password parts are set in any ored, we calculate master password
                _password[0] = _dataProtector.Protect
                (
                    plaintext: ComputeHash
                    (
                        _dataProtector.Unprotect(_password[1]),
                        _dataProtector.Unprotect(_password[2])
                    )
                );


                await ValidatePasswordAsync(partTheeBytes);
            }
        }

        public virtual async Task ValidatePasswordAsync(byte[] validationBytes)
        {
            if (_passwordValidated)
            {
                Array.Clear(validationBytes, 0, validationBytes.Length);

                return;
            }

            _passwordValidated = true;

            if (_password[1] == null || _password[2] == null)
            {
                Array.Clear(validationBytes, 0, validationBytes.Length);

                return;
            }

            var actualValidationHash = Utils.ToHexString((ICollection<byte>)ComputeHash
            (
                validationBytes,
                _dataProtector.Unprotect(_password[0]),
                _dataProtector.Unprotect(_password[1]),
                _dataProtector.Unprotect(_password[2])

            ));

            Array.Clear(_password[1], 0, _password[1].Length);
            Array.Clear(_password[2], 0, _password[2].Length);
            Array.Clear(validationBytes, 0, validationBytes.Length);

            await ValidatePasswordHashes(actualValidationHash);
        }

        protected virtual async Task ValidatePasswordHashes(string actualValidationHash)
        {
            var exptectedValidationHashes = (await _passwordValidationHashRepository.GetAsync()).ToList();

            if (!exptectedValidationHashes.Any())
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
                    CryptoProvider.GetBytes(randomBytes);

                    validationHashes.Add
                    (
                        Utils.ToHexString((ICollection<byte>)ComputeHash(randomBytes))
                    );
                }

                validationHashes = validationHashes
                    .OrderBy(x => _random.Next())
                    .ToList();

                await _passwordValidationHashRepository.InsertAsync(validationHashes);

                IsPasswordValid = true;
            }
            else
            {
                IsPasswordValid = exptectedValidationHashes.Contains(actualValidationHash);
            }
        }

        protected static IDataProtector CreateDataProtector()
        {
            // DataProtector helps us not to store sensitive information as plain-text in memory

            var provider = new EphemeralDataProtectionProvider();

            return provider.CreateProtector
            (
                purpose: "MasterPasswordService"
            );
        }

        protected static byte[] ComputeHash(params byte[][] data)
        {
            return SHA256Instance.ComputeHash
            (
                data.SelectMany(x => x).ToArray()
            );
        }

        protected static byte[] ComputeHash(string data)
        {
            return ComputeHash
            (
                Encoding.UTF8.GetBytes(data)
            );
        }
    }
}
