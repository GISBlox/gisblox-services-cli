using McMaster.Extensions.CommandLineUtils;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GISBlox.Services.CLI.Commands.Auth
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
               var newProfile = new UserProfile() { ServiceKey = Encrypt(Token) };               
               await SaveUserProfile(newProfile);

               // Get subscription info and add it to the user profile
               var result = await GISBloxClient.Info.GetSubscriptions();
               var locationServicesSub = result.Where(s => s.Code.StartsWith("GBLS-")).SingleOrDefault();
               if (locationServicesSub != null)
               {
                  UserProfile.SubscriptionExpirationDate = locationServicesSub.ExpirationDate;
                  await SaveUserProfile();

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
