using ASTDiffTool.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.Tests
{
    public class EncryptionHelperTests
    {
        private readonly string _validKey;
        private readonly string _validIV;

        public EncryptionHelperTests()
        {
            // generate a valid 256-bit key and 128-bit IV
            using var aes = Aes.Create();
            aes.GenerateKey();
            aes.GenerateIV();

            _validKey = Convert.ToBase64String(aes.Key);
            _validIV = Convert.ToBase64String(aes.IV);
        }

        [Fact]
        public void EncryptAndDecrypt_ShouldReturnOriginalText()
        {
            string plainText = "This is a test message";

            string encryptedText = EncryptionHelper.Encrypt(plainText, _validKey, _validIV);
            string decryptedText = EncryptionHelper.Decrypt(encryptedText, _validKey, _validIV);

            Assert.Equal(plainText, decryptedText);
        }

        [Fact]
        public void Encrypt_ShouldProduceDifferentOutputs_WithDifferentIVs()
        {
            string plainText = "This is a test message";
            using var aes = Aes.Create();
            aes.GenerateIV();
            string differentIV = Convert.ToBase64String(aes.IV);

            string encryptedText1 = EncryptionHelper.Encrypt(plainText, _validKey, _validIV);
            string encryptedText2 = EncryptionHelper.Encrypt(plainText, _validKey, differentIV);

            Assert.NotEqual(encryptedText1, encryptedText2);
        }

        [Fact]
        public void Decrypt_ShouldFail_WithWrongKey()
        {
            string plainText = "This is a test message";
            string encryptedText = EncryptionHelper.Encrypt(plainText, _validKey, _validIV);

            using var aes = Aes.Create();
            aes.GenerateKey();
            string wrongKey = Convert.ToBase64String(aes.Key);

            Assert.Throws<CryptographicException>(() => EncryptionHelper.Decrypt(encryptedText, wrongKey, _validIV));
        }

        [Fact]
        public void Decrypt_ShouldFail_WithWrongIV()
        {
            string plainText = "This is a test message";
            string encryptedText = EncryptionHelper.Encrypt(plainText, _validKey, _validIV);

            using var aes = Aes.Create();
            aes.GenerateIV();
            string wrongIV = Convert.ToBase64String(aes.IV);

            string decryptedText;
            try
            {
                decryptedText = EncryptionHelper.Decrypt(encryptedText, _validKey, wrongIV);
            }
            catch (CryptographicException)
            {
                return;
            }


            Assert.NotEqual(plainText, decryptedText);
        }

        [Fact]
        public void Encrypt_ShouldThrowException_WhenKeyIsInvalid()
        {
            string plainText = "This is a test message";
            string invalidKey = "InvalidKey";

            Assert.Throws<FormatException>(() => EncryptionHelper.Encrypt(plainText, invalidKey, _validIV));
        }

        [Fact]
        public void Decrypt_ShouldThrowException_WhenCipherTextIsInvalid()
        {
            string invalidCipherText = "InvalidCipherText";

            Assert.Throws<FormatException>(() => EncryptionHelper.Decrypt(invalidCipherText, _validKey, _validIV));
        }

        [Fact]
        public void Encrypt_ShouldThrowException_WhenIVIsInvalid()
        {
            string plainText = "This is a test message";
            string invalidIV = "InvalidIV";

            Assert.Throws<FormatException>(() => EncryptionHelper.Encrypt(plainText, _validKey, invalidIV));
        }

        [Fact]
        public void Encrypt_ShouldThrowException_WhenPlainTextIsNull()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => EncryptionHelper.Encrypt(null, _validKey, _validIV));
            Assert.Equal("plainText", exception.ParamName);
        }


        [Fact]
        public void Decrypt_ShouldThrowException_WhenCipherTextIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => EncryptionHelper.Decrypt(null, _validKey, _validIV));
        }
    }
}
