using GISBlox.Services.SDK;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GISBlox.Services.CLI
{
   [HelpOption("--help")]
   abstract class gbsCmdBase
   {
      private GISBloxClient _gisbloxClient;

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
               _gisbloxClient = GISBloxClient.CreateClient("https://services.gisblox.com", "key");
            }
            return _gisbloxClient;
         }
      }

      protected void OnException(Exception ex)
      {
         OutputError(ex.Message);        
      }

      protected void OutputJson(string data)
      {         
          Output(data);  
      }

      protected void Output(string data)
      {         
         OutputToConsole(data);         
      }      

      protected void OutputToConsole(string data)
      {
         _console.BackgroundColor = ConsoleColor.Black;
         _console.ForegroundColor = ConsoleColor.White;
         _console.Out.Write(data);
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
