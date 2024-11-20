using System;
using System.Security.Cryptography;
using System.Text;
namespace ASTDiffTool.Shared
{
    public class EncryptionHelper
    {
        private static readonly byte[] Key = Convert.FromBase64String(Environment.GetEnvironmentVariable("ENCRYPTION_KEY") ?? throw new InvalidOperationException("ENCRYPTION_KEY not set."));
        private static readonly byte[] IV = Convert.FromBase64String(Environment.GetEnvironmentVariable("ENCRYPTION_IV") ?? throw new InvalidOperationException("ENCRYPTION_IV not set."));

        public static string Encrypt(string plainText)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = IV;

                var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                var plainBytes = Encoding.UTF8.GetBytes(plainText);

                var encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                return Convert.ToBase64String(encryptedBytes);
            }
        }

        public static string Decrypt(string encryptedText)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = IV;

                var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                var encryptedBytes = Convert.FromBase64String(encryptedText);

                var decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                return Encoding.UTF8.GetString(decryptedBytes);
            }
        }
    }
}
