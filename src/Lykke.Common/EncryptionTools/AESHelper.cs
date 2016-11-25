using System.Security.Cryptography;
using System.Text;

namespace Common.EncryptionTools
{
    public static class AESHelper
    {
        public static string Encrypt128ECB(string data, string key)
        {
            var keyBytes = Encoding.ASCII.GetBytes(key);
            var aes = Aes.Create();
            aes.BlockSize = 128;
            aes.KeySize = 128;
            aes.Key = keyBytes;
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.Zeros;
            aes.IV = keyBytes;
            
            var encryptor = aes.CreateEncryptor(keyBytes, keyBytes);
            var res = encryptor.TransformFinalBlock(Encoding.ASCII.GetBytes(data), 0, data.Length);

            return StringUtils.GetBytesToHexString(res).ToLower();
        }
    }
}
