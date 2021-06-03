// ------------------------------------------------------------
// Copyright (c) Bartels Online.  All rights reserved.
// ------------------------------------------------------------

using GISBlox.Services.CLI.Commands.Auth;
using GISBlox.Services.CLI.Commands.Convert;
using GISBlox.Services.CLI.Commands.Project;
using McMaster.Extensions.CommandLineUtils;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace GISBlox.Services.CLI
{
   [Command(Name = "gbs", Description = "GISBlox Services CLI", OptionsComparison = StringComparison.InvariantCultureIgnoreCase)]
   [VersionOptionFromMember("--version", ShortName = "v", LongName = "Version", MemberName = nameof(GetVersion))]
   [Subcommand(
         typeof(LoginCmd),
         typeof(LogoutCmd),
         typeof(StatusCmd),
         typeof(ConvertCmd),
         typeof(Project)
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
