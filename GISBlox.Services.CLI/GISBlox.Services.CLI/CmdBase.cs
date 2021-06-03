// ------------------------------------------------------------
// Copyright (c) Bartels Online.  All rights reserved.
// ------------------------------------------------------------

using GISBlox.Services.CLI.Utils;
using GISBlox.Services.SDK;
using McMaster.Extensions.CommandLineUtils;
using System;
using System.IO;
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
                     _userProfile.ServiceKey = Security.Decrypt(_userProfile.ServiceKey);
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
