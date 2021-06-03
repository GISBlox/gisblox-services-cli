// ------------------------------------------------------------
// Copyright (c) Bartels Online.  All rights reserved.
// ------------------------------------------------------------

using GISBlox.Services.CLI.Utils;
using McMaster.Extensions.CommandLineUtils;
using System;
using System.Threading.Tasks;

namespace GISBlox.Services.CLI.Commands.Auth
{
   [Command(Name = "login", Description = "Authenticate with a GISBlox Services host.", OptionsComparison = StringComparison.InvariantCultureIgnoreCase)]
   class LoginCmd : CmdBase
   {
      [Option(CommandOptionType.SingleValue, ShortName = "t", LongName = "with-token", Description = "Authenticate with a GISBlox Services service key.", ValueName = "service key", ShowInHelpText = true)]
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
                  Token = Security.SecureStringToString(Prompt.GetPasswordAsSecureString("Service key:"));
               }

               // Create a user profile with the encrypted service key
               var newProfile = new UserProfile() { ServiceKey = Security.Encrypt(Token) };               
               await SaveUserProfile(newProfile);

               // Make an API call to authorize user
               var result = await GISBloxClient.Info.GetSubscriptions();               
               if (result != null)
               {                
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
   }
}
