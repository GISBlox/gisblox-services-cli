using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GISBlox.Services.CLI.Commands.Auth
{
   [Command(Name = "status", Description = "Verifies and displays information about your authentication state.")]
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
            OutputToConsole("Checking status...");

            
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
