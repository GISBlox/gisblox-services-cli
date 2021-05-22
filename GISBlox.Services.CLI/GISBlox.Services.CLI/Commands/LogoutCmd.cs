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
            if (Directory.Exists(ProfileFolder))
            {
               string userProfile = $"{ProfileFolder}default";
               if (File.Exists(userProfile))
               {
                  File.Delete(userProfile);
                  OutputToConsole("Successfully logged out.", ConsoleColor.Green);
               }
               else
               {
                  OutputToConsole("Not logged in!", ConsoleColor.Yellow);
               }
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
