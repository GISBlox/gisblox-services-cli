using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace GISBlox.Services.CLI.Utils
{
   internal class Security
   {
      public static string SecureStringToString(SecureString value)
      {
         IntPtr valuePtr = IntPtr.Zero;
         try
         {
            valuePtr = Marshal.SecureStringToGlobalAllocUnicode(value);
            return Marshal.PtrToStringUni(valuePtr);
         }
         finally
         {
            Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
         }
      }

      private static string EncryptKey
      {
         get
         {
            var keyLen = 32;
            var key = Environment.UserName;
            if (key.Length > keyLen)
            {
               key = key.Substring(0, keyLen);
            }
            else if (key.Length < keyLen)
            {
               var len = key.Length;
               for (var i = 0; i < keyLen - len; i++)
               {
                  key += ((char)(65 + i)).ToString();
               }
            }
            return key;
         }
      }

      public static string Encrypt(string text)
      {
         var keyString = EncryptKey;
         var key = Encoding.UTF8.GetBytes(keyString);
         using (var aesAlg = Aes.Create())
         {
            using (var encryptor = aesAlg.CreateEncryptor(key, aesAlg.IV))
            {
               using (var msEncrypt = new MemoryStream())
               {
                  using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                  using (var swEncrypt = new StreamWriter(csEncrypt))
                  {
                     swEncrypt.Write(text);
                  }
                  var iv = aesAlg.IV;
                  var decryptedContent = msEncrypt.ToArray();
                  var result = new byte[iv.Length + decryptedContent.Length];
                  Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
                  Buffer.BlockCopy(decryptedContent, 0, result, iv.Length, decryptedContent.Length);
                  return Convert.ToBase64String(result);
               }
            }
         }
      }

      public static string Decrypt(string cipherText)
      {
         var keyString = EncryptKey;
         var fullCipher = Convert.FromBase64String(cipherText);

         var iv = new byte[16];
         var cipher = new byte[fullCipher.Length - 16];
         Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
         Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, cipher.Length);

         var key = Encoding.UTF8.GetBytes(keyString);
         using (var aesAlg = Aes.Create())
         {
            using (var decryptor = aesAlg.CreateDecryptor(key, iv))
            {
               string result;
               using (var msDecrypt = new MemoryStream(cipher))
               {
                  using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                  {
                     using (var srDecrypt = new StreamReader(csDecrypt))
                     {
                        result = srDecrypt.ReadToEnd();
                     }
                  }
               }
               return result;
            }
         }
      }
   }
}
