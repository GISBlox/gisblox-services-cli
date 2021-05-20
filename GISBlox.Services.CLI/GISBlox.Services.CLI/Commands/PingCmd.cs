using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace GISBlox.Services.CLI.Commands
{
   [Command(Name = "ping", Description = "Ping the GISBlox Location Services.")]
   class PingCmd : CmdBase
   {
      public PingCmd(IConsole console)
      {
         _console = console;
      }
      
      protected override async Task<int> OnExecute(CommandLineApplication app)
      {         
         try
         {

            Output("Hello from GISBlox!");            
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
