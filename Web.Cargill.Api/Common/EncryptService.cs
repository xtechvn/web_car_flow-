﻿using B2B.Utilities.Common;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;


namespace LIB.Utilities.Common
{
    public static class EncryptService
    {
        public static byte[] Get_AESKey(byte[] data)
        {

            if (data.Length >= 16 && data.Length < 24)
            {
                return data.Take(16).ToArray();
            }
            if (data.Length >= 24 && data.Length < 32)
            {
                return data.Take(24).ToArray();
            }
            if (data.Length >= 32)
            {
                return data.Take(32).ToArray();
            }

            return data;

        }
        public static byte[] Get_AESIV(byte[] data)
        {

            if (data.Length >= 16)
            {
                return data.Take(16).ToArray();
            }

            return data;

        }
        public static byte[] ConvertBase64StringToByte(string text)
        {
            try
            {
                return Convert.FromBase64String(text);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ConvertBase64StringToByte - MFAService" + ex);
            }
            return new byte[0];
        }
        public static string ConvertByteToBase64String(byte[] data)
        {
            try
            {
                return Convert.ToBase64String(data);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ConvertBase64StringToByte - MFAService" + ex);
            }
            return null;
        }
        public static byte[] AES_EncryptToByte(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        public static string AES_DecryptToString(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }


        public static string FormatKey(string unformattedKey)
        {
            try
            {
                return Regex.Replace(unformattedKey.Trim(), ".{4}", "$0 ");
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("FormatKey - AccountController" + ex);
                return null;
            }
        }

        public static string Base64StringToURLParam(string text)
        {
            var p = text.Replace(@"/", "-").Replace("+", "_");
            return p;
        }
        public static string URLParamToBase64String(string text)
        {
            var p = text.Replace("-", @"/").Replace("_", "+");
            return p;
        }

        /// <summary>
        /// Mã hóa chuỗi bằng AES và trả về kết quả dưới dạng Base64.
        /// </summary>
        /// <param name="plainText">Chuỗi gốc cần mã hóa.</param>
        /// <param name="key">Khóa AES (32 byte cho AES-256).</param>
        /// <param name="iv">Vector khởi tạo (IV) (16 byte).</param>
        /// <returns>Chuỗi đã mã hóa dưới dạng Base64.</returns>
        public static string AES_EncryptToBase64(string plainText, string key, string iv)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentNullException(nameof(plainText));
            if (string.IsNullOrEmpty(key) || key.Length != 32)
                throw new ArgumentException("Khóa phải có độ dài 32 byte cho AES-256.", nameof(key));
            if (string.IsNullOrEmpty(iv) || iv.Length != 16)
                throw new ArgumentException("IV phải có độ dài 16 byte.", nameof(iv));

            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = Encoding.UTF8.GetBytes(iv);
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }

                    byte[] encrypted = msEncrypt.ToArray();
                    return Convert.ToBase64String(encrypted); // Trả về chuỗi mã hóa dưới dạng Base64
                }
            }
        }
    }

}

