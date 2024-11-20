using System;
using System.Security.Cryptography;
using System.Text;
namespace ASTDiffTool.Shared
{
    public class EncryptionHelper
    {
        public static string Encrypt(string plainText, string key, string iv)
        {
            using var aes = Aes.Create();

            aes.Key = Convert.FromBase64String(key); // Decoding Base64-encoded key
            aes.IV = Convert.FromBase64String(iv);   // Decoding Base64-encoded IV

            using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
            using (var sw = new StreamWriter(cs))
            {
                sw.Write(plainText);
            }

            return Convert.ToBase64String(ms.ToArray());
        }

        public static string Decrypt(string cipherText, string key, string iv)
        {
            using var aes = Aes.Create();

            aes.Key = Convert.FromBase64String(key);
            aes.IV = Convert.FromBase64String(iv);

            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream(Convert.FromBase64String(cipherText));
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);

            return sr.ReadToEnd();
        }
    }
}
