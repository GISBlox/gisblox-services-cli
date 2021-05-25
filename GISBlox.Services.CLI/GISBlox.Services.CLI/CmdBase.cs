﻿using GISBlox.Services.SDK;
using GISBlox.Services.SDK.Models;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GISBlox.Services.CLI
{
   [HelpOption("--help")]
   abstract class CmdBase
   {
      private UserProfile _userProfile;
      private GISBloxClient _gisbloxClient;
      private const string BASE_URL = "https://services.gisblox.com";

      protected IConsole _console;    

      protected virtual Task<int> OnExecute(CommandLineApplication app)
      {
         return Task.FromResult(0);
      }

      protected GISBloxClient GISBloxClient
      {
         get
         {
            if (_gisbloxClient == null)
            {
               _gisbloxClient = GISBloxClient.CreateClient(BASE_URL, UserProfile.ServiceKey);
            }
            return _gisbloxClient;
         }
      }

      #region User profile

      protected string ProfileFolder
      {
         get
         {
            return $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile, Environment.SpecialFolderOption.Create)}\\.gbs\\";
         }
      }

      protected string ProfileFullName
      { 
         get
         {
            return $"{ProfileFolder}default";
         }
      }

      protected UserProfile UserProfile
      {
         get
         {
            if (_userProfile == null)
            {
               var text = File.ReadAllText(ProfileFullName);
               if (!string.IsNullOrEmpty(text))
               {
                  _userProfile = JsonSerializer.Deserialize<UserProfile>(text);
                  if (_userProfile != null)
                  {
                     _userProfile.ServiceKey = Decrypt(_userProfile.ServiceKey);
                  }
               }
            }
            return _userProfile;
         }
      }

      protected bool UserProfileExists()
      {         
         return File.Exists(ProfileFullName);
      }

      protected void CreateUserProfileFolder()
      {
         if (!Directory.Exists(ProfileFolder))
         {
            Directory.CreateDirectory(ProfileFolder);
         }
      }

      protected void DeleteUserProfile()
      {         
         if (File.Exists(ProfileFullName))
         {
            File.Delete(ProfileFullName);            
         }
      }
     
      protected async Task SaveUserProfile(UserProfile profile)
      {
         CreateUserProfileFolder();
         await File.WriteAllTextAsync(ProfileFullName, JsonSerializer.Serialize(profile, typeof(UserProfile)), UTF8Encoding.UTF8);         
      }

      #endregion

      #region Security utils

      protected static string SecureStringToString(SecureString value)
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

      protected static string Encrypt(string text)
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

      protected static string Decrypt(string cipherText)
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

      #endregion

      #region Parsers

      /// <summary>
      /// Converts a string into a Coordinate type.
      /// </summary>
      /// <param name="coordinate">A string containing a coordinate pair in 'x,y' format.</param>
      /// <param name="latLon">Set to True if the coordinate pair is passed in lat/lon order, False if in lon/lat order.</param>
      /// <returns>A Coordinate type with the converted coordinate string.</returns>
      protected static Coordinate CoordinateFromString(string coordinate, bool latLon = true)
      {
         Coordinate c = new();
         if (coordinate == null || coordinate.Length == 0)
            throw new ArgumentNullException(nameof(coordinate));
         string[] splitCoordinate = coordinate.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
         if (splitCoordinate.Length != 2)
            throw new ArgumentException("Invalid coordinate string.");
         if (!double.TryParse(splitCoordinate[0].Replace(".", ","), out double x))
            throw new ArgumentException("Invalid coordinate string: the X component (" + splitCoordinate[0].Replace(".", ",") + ") is not of type 'double'.");
         if (!double.TryParse(splitCoordinate[1].Replace(".", ","), out double y))
            throw new ArgumentException("Invalid coordinate string: the Y component (" + splitCoordinate[1].Replace(".", ",") + ") is not of type 'double'.");
         if (latLon)
         {
            c.Lat = x;
            c.Lon = y;
         }
         else
         {
            c.Lat = y;
            c.Lon = x;
         }
         return c;
      }

      #endregion

      protected void OnException(Exception ex)
      {
         if (ex is ClientApiException)
         {
            ClientApiException apiException = (ClientApiException)ex;
            if (apiException.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
               // Remove stale information
               DeleteUserProfile();
               OutputToConsole("Unauthorized - check your subscription details at https://account.gisblox.com", ConsoleColor.Red);
            }
            else
            {
               OutputError(apiException.StatusCode.ToString());
            }
         }
         else
         {
            OutputError(ex.Message);
         }
      }

      protected void OutputJson(string data)
      {         
          Output(data);  
      }

      protected void Output(string data)
      {         
         OutputToConsole(data);         
      }      

      protected void OutputToConsole(string data, ConsoleColor foregroundColor = ConsoleColor.White, ConsoleColor backgroundColor = ConsoleColor.Black)
      {
         _console.BackgroundColor = backgroundColor;
         _console.ForegroundColor = foregroundColor;
         _console.Out.WriteLine(data);         
         _console.ResetColor();
      }

      protected void OutputError(string message)
      {
         _console.BackgroundColor = ConsoleColor.Red;
         _console.ForegroundColor = ConsoleColor.White;
         _console.Error.WriteLine(message);
         _console.ResetColor();
      }    
   }
}