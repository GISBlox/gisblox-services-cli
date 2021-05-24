using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GISBlox.Services.CLI.Commands;
using GISBlox.Services.CLI.Commands.Auth;
using GISBlox.Services.CLI.Commands.Convert;
//using GISBlox.Services.CLI.Commands.Project;

namespace GISBlox.Services.CLI
{
   [Command(Name = "gbs", OptionsComparison = StringComparison.InvariantCultureIgnoreCase)]
   [VersionOptionFromMember("--version", MemberName = nameof(GetVersion))]
   [Subcommand(
         typeof(LoginCmd),
         typeof(LogoutCmd),
         typeof(StatusCmd),
         typeof(ConvertCmd)
      )]
   class Cmd : CmdBase
   {
      public Cmd(IConsole console)
      {
         _console = console;
      }

      protected override Task<int> OnExecute(CommandLineApplication app)
      {
         // this shows help even if the --help option isn't specified         
         app.ShowHelp();
         return Task.FromResult(0);
      }

      private static string GetVersion()
          => typeof(Cmd).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
   }
}
