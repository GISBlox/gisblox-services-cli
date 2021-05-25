using GISBlox.Services.SDK.Models;
using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GISBlox.Services.CLI.Commands.Project
{
   [Command(Name = "to-rds", Description = "Converts coordinates to Rijksdriehoekstelsel (RDNew) locations.")]

   class ToRDNewCmd : CmdBase
   {
      [Option(CommandOptionType.SingleValue, ShortName = "c", LongName = "coord", Description = "coordinate", ValueName = "coordinate", ShowInHelpText = true)]
      public string Coordinate { get; set; }

      [Option(CommandOptionType.NoValue, ShortName = "s", LongName = "source", Description = "include source", ValueName = "source", ShowInHelpText = true)]
      public bool IncludeSource { get; set; }

      [Option(CommandOptionType.SingleValue, ShortName = "l", LongName = "lat-lon", Description = "lat-lon format", ValueName = "lat-lon", ShowInHelpText = true)]
      public bool LatLonFormat { get; set; }

      [Option(CommandOptionType.SingleValue, ShortName = "o", LongName = "output", Description = "output file", ValueName = "output file", ShowInHelpText = true)]
      public string OutputFile { get; set; }

      [Option(CommandOptionType.SingleValue, ShortName = "i", LongName = "input", Description = "input file", ValueName = "input file", ShowInHelpText = true)]
      public string InputFile { get; set; }

      public ToRDNewCmd(IConsole console)
      {
         _console = console;
      }
      protected override async Task<int> OnExecute(CommandLineApplication app)
      {
         try
         {
            if (!UserProfileExists())
            {
               OutputToConsole("Not logged in!", ConsoleColor.Yellow);
               return 1;
            }
            else
            {
               if (!string.IsNullOrEmpty(Coordinate) && string.IsNullOrEmpty(InputFile) && string.IsNullOrEmpty(OutputFile))
               {
                  // Single coordinate                  
                  Coordinate c = CoordinateFromString(Coordinate, LatLonFormat);
                  if (IncludeSource)
                  {
                     Location location = await GISBloxClient.Projection.ToRDSComplete(c);
                     OutputToConsole($"Lat: { location.Lat } Lon: { location.Lon } X:{ location.X } Y:{ location.Y }", ConsoleColor.Green);
                  }
                  else
                  {
                     RDPoint rdPoint = await GISBloxClient.Projection.ToRDS(c);
                     OutputToConsole($"X:{ rdPoint.X } Y:{ rdPoint.Y }", ConsoleColor.Green);
                  }
                  return 0;
               }
               else if (!string.IsNullOrEmpty(InputFile) && !string.IsNullOrEmpty(OutputFile))
               {
                  // Process file
                  return 0;
               }
               else
               {
                  OutputToConsole("Invalid combination of command options!", ConsoleColor.Red);
                  return 1;
               }
            }
         }
         catch (Exception ex)
         {
            OnException(ex);
            return 1;
         }
      }
   }
}
