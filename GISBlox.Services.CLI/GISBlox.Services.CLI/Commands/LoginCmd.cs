using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace GISBlox.Services.CLI.Commands
{
   [Command(Name = "login", Description = "Authenticate with a GISBlox Services host.")]
   class LoginCmd : CmdBase
   {
      [Option(CommandOptionType.SingleValue, ShortName = "t", LongName = "with-token", Description = "GISBlox Services service key", ValueName = "service key", ShowInHelpText = true)]
      public string Token { get; set; }

      public LoginCmd(IConsole console)
      {
         _console = console;
      }

      protected override async Task<int> OnExecute(CommandLineApplication app)
      {  
         try
         {
            OutputToConsole("Attempting to log in...");

            if (UserProfileExists())
            {
               OutputToConsole("Already logged in!", ConsoleColor.Green);
            }
            else
            {
               // Get the service key from the user if not provided on the command line
               if (string.IsNullOrEmpty(Token))
               {
                  Token = SecureStringToString(Prompt.GetPasswordAsSecureString("Service key:"));
               }

               // Create a user profile with the encrypted service key
               var userProfile = new UserProfile()
               {
                  ServiceKey = Encrypt(Token)
               };

               // Save to disc
               await SaveUserProfile(userProfile);

               // Add subscription info to user profile
               var result = await GISBloxClient.Info.GetSubscriptions();
               var locationServicesSub = result.Where(s => s.Code.StartsWith("GBLS-")).SingleOrDefault();
               if (locationServicesSub != null)
               {
                  userProfile.SubscriptionExpirationDate = locationServicesSub.ExpirationDate;
                  await SaveUserProfile(userProfile);

                  OutputToConsole("Successfully logged in.", ConsoleColor.Green);                  
               }
            }
            return 0;
         }
         catch (Exception ex)
         {            
            OnException(ex);
            return 1;
         }
      }
      
      protected async Task SaveUserProfile(UserProfile profile)
      {
         // Create the user profile folder (if not exists)
         if (!Directory.Exists(ProfileFolder))
         {
            Directory.CreateDirectory(ProfileFolder);
         }

         // Serialize the user profile to disc
         await File.WriteAllTextAsync($"{ProfileFolder}default", JsonSerializer.Serialize(profile, typeof(UserProfile)), UTF8Encoding.UTF8);
      }      
   }
}
