using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace Lykke.Common.EncryptionTools
{
    public static class Aes256Helper
    {
        /// <summary>
        ///    Decrypts base64 string with AES256 (key 32 bytes)
        /// </summary>
        public static string Decrypt(string encryptedData, byte[] key)
        {
            using (var aesAlg = Aes.Create())
            {
                // ReSharper disable once PossibleNullReferenceException
                aesAlg.KeySize = 256;
                aesAlg.Key = key;
                aesAlg.IV = key.Take(16).ToArray();


                var encryptedBytes = Convert.FromBase64String(encryptedData);
                var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (var memoryStream = new MemoryStream(encryptedBytes))
                using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                using (var reader = new StreamReader(cryptoStream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        /// <summary>
        ///    Encrypts base64 data with AES256 (key 32 bytes)
        /// </summary>
        public static string Encrypt(string data, byte[] key)
        {
            using (var aesAlg = Aes.Create())
            {
                // ReSharper disable once PossibleNullReferenceException
                aesAlg.KeySize = 256;
                aesAlg.Key = key;
                aesAlg.IV = key.Take(16).ToArray();

                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (var memoryStream = new MemoryStream())
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                using (var writer = new StreamWriter(cryptoStream))
                {
                    writer.Write(data);
                    writer.Close();

                    return Convert.ToBase64String(memoryStream.ToArray());
                }
            }
        }
    }
}
