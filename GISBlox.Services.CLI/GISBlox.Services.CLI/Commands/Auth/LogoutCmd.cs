using McMaster.Extensions.CommandLineUtils;
using System;
using System.Threading.Tasks;

namespace GISBlox.Services.CLI.Commands.Auth
{
   [Command(Name = "logout", Description = "Log out of a GISBlox Services host.")]
   class LogoutCmd : CmdBase
   {
      public LogoutCmd(IConsole console)
      {
         _console = console;
      }

      protected override async Task<int> OnExecute(CommandLineApplication app)
      {
         try
         {
            OutputToConsole("Attempting to log out...");

            if (UserProfileExists())
            {
               DeleteUserProfile();
               OutputToConsole("Successfully logged out.", ConsoleColor.Green);
            }
            else
            {
               OutputToConsole("Not logged in!", ConsoleColor.Yellow);
            }            
            return await Task.FromResult(0);
         }
         catch (Exception ex)
         {
            OnException(ex);
            return 1;
         }
      }
   }      
}
