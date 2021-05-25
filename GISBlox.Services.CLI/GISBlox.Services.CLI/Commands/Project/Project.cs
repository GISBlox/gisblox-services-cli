using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GISBlox.Services.CLI.Commands.Project
{
   [Command(Name = "project", OptionsComparison = StringComparison.InvariantCultureIgnoreCase)]   
   [Subcommand(
         typeof(ToRDNewCmd)         
      )]
   class Project : CmdBase
   {
      public Project(IConsole console)
      {
         _console = console;
      }
   }
}
