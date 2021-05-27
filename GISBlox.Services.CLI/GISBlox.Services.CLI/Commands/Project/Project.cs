using McMaster.Extensions.CommandLineUtils;
using System;

namespace GISBlox.Services.CLI.Commands.Project
{
   [Command(Name = "project", Description = "Contains commands to re-project coordinates.", OptionsComparison = StringComparison.InvariantCultureIgnoreCase)]   
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
