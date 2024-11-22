using System;
using System.Security.Cryptography;
using System.Text;
namespace ASTDiffTool.Shared
{
    /// <summary>
    /// Class responsible for implementing basic encryption and decryption methos
    /// </summary>
    public class EncryptionHelper
    {
        /// <summary>
        /// Method the encrypt a given text.
        /// </summary>
        /// <param name="plainText">The text to encrypt</param>
        /// <param name="key">The key</param>
        /// <param name="iv">The iv</param>
        /// <returns>The encrypted value</returns>
        /// <exception cref="ArgumentNullException">In case of empty text</exception>
        public static string Encrypt(string plainText, string key, string iv)
        {
            if (plainText == null)
            {
                throw new ArgumentNullException(nameof(plainText), "Plain text cannot be null");
            }

            using var aes = Aes.Create();

            aes.Key = Convert.FromBase64String(key);
            aes.IV = Convert.FromBase64String(iv);

            using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
            using (var sw = new StreamWriter(cs))
            {
                sw.Write(plainText);
            }

            return Convert.ToBase64String(ms.ToArray());
        }

        /// <summary>
        /// Decrypts the provided cipher text using AES decryption with the specified key and IV.
        /// </summary>
        /// <param name="cipherText">The encrypted text that needs to be decrypted, represented as a Base64 encoded string.</param>
        /// <param name="key">The key used for decryption, represented as a Base64 encoded string. Must match the key used during encryption.</param>
        /// <param name="iv">The initialization vector (IV) used for decryption, represented as a Base64 encoded string. Must match the IV used during encryption.</param>
        /// <returns>The decrypted plain text as a string.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="cipherText"/> is null.</exception>
        /// <exception cref="FormatException">Thrown if <paramref name="cipherText"/>, <paramref name="key"/>, or <paramref name="iv"/> are not valid Base64 strings.</exception>
        /// <exception cref="CryptographicException">Thrown when decryption fails, such as when the key or IV is incorrect.</exception>
        public static string Decrypt(string cipherText, string key, string iv)
        {
            if (cipherText == null)
            {
                throw new ArgumentNullException(nameof(cipherText), "Cipher text cannot be null");
            }

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
