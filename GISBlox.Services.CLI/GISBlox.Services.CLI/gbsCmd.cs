using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GISBlox.Services.CLI
{
   [Command(Name = "gbs", OptionsComparison = StringComparison.InvariantCultureIgnoreCase)]
   [VersionOptionFromMember("--version", MemberName = nameof(GetVersion))]
   [Subcommand(
        typeof(LoginCmd),
        typeof(ListTicketCmd))]
   class gbsCmd : gbsCmdBase
   {
      public gbsCmd(IConsole console)
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
          => typeof(gbsCmd).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
   }
}
