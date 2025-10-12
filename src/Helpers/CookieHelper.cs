using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;

namespace PortoWeb.Helpers
{
    public static class CookieHelper
    {
        private static readonly string EncryptionKey = "3dcefdbabd687352";
        public static string Encrypt(string plainText)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(plainText);
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(EncryptionKey);
                aes.IV = new byte[16]; // Set a default IV
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                byte[] encrypted = encryptor.TransformFinalBlock(bytes, 0, bytes.Length);
                return Convert.ToBase64String(encrypted);
            }
        }
        public static string Decrypt(string encryptedText)
        {
            byte[] bytes = Convert.FromBase64String(encryptedText);
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(EncryptionKey);
                aes.IV = new byte[16];
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                byte[] decrypted = decryptor.TransformFinalBlock(bytes, 0, bytes.Length);
                return Encoding.UTF8.GetString(decrypted);
            }
        }
        public static void SetEncryptedCookie(string key, string value, HttpResponseBase response)
        {
            var encryptedValue = Encrypt(value);
            HttpCookie cookie = new HttpCookie(key, encryptedValue)
            {
                HttpOnly = true
            };
            response.Cookies.Add(cookie);
        }

        public static string GetDecryptedCookie(string key, HttpRequestBase request)
        {
            var cookie = request.Cookies[key];
            return cookie != null ? Decrypt(cookie.Value) : null;
        }
    }
}