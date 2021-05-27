using McMaster.Extensions.CommandLineUtils;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GISBlox.Services.CLI.Commands.Auth
{
   [Command(Name = "status", Description = "Verifies and displays information about your authentication state.", OptionsComparison = StringComparison.InvariantCultureIgnoreCase)]
   class StatusCmd : CmdBase
   {
      public StatusCmd(IConsole console)
      {
         _console = console;
      }

      protected override async Task<int> OnExecute(CommandLineApplication app)
      {
         try
         {
            if (!UserProfileExists())
            {
               OutputToConsole("Not logged in!", ConsoleColor.Yellow);
               return 1;
            }
            else
            {
               OutputToConsole("Checking status...");

               var result = await GISBloxClient.Info.GetSubscriptions();
               var locationServicesSub = result.Where(s => s.Code.StartsWith("GBLS-")).SingleOrDefault();
               if (locationServicesSub != null)
               {
                  OutputToConsole("Successfully authenticated.", ConsoleColor.Green);

                  Output("Subscription details:");
                  Output($" - Name: { locationServicesSub.Name }");
                  Output($" - Code: { locationServicesSub.Code }");
                  Output($" - Description: { locationServicesSub.Description }");
                  Output($" - Registration date: { locationServicesSub.RegisterDate }");
                  Output($" - Expiration date: { locationServicesSub.ExpirationDate }");
                  Output($" - Expired: { locationServicesSub.Expired }");                  
                  return 0;
               }
               else
               {
                  return 1;
               }
            }
         }
         catch (Exception ex)
         {
            OnException(ex);
            return 1;
         }
      }
   }
}
