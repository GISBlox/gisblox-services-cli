using GISBlox.Services.CLI.Utils;
using GISBlox.Services.SDK.Models;
using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GISBlox.Services.CLI.Commands.Project
{
   [Command(Name = "to-wgs84", Description = "Converts Rijksdriehoekstelsel (RDNew) locations to WGS84 coordinates. Projects a single location, or batch-processes a file with locations.",
            OptionsComparison = StringComparison.InvariantCultureIgnoreCase)]
   class ToCoordinateCmd : CmdBase
   {
      [Option(CommandOptionType.SingleValue, ShortName = "l", LongName = "location", Description = "The RDNew location to project to a WGS84 coordinate (ignored when an input file is used).", ValueName = "location", ShowInHelpText = true)]
      public string Location { get; set; }

      [Option(CommandOptionType.SingleValue, ShortName = "s", LongName = "separator", Description = "Specifies the location separator, e.g. \";\" (ignored when the input file is an Excel file).", ValueName = "separator", ShowInHelpText = true)]
      public string Separator { get; set; }
      
      [Option(CommandOptionType.SingleValue, ShortName = "d", LongName = "decimals", Description = "Rounds the coordinate(s) to the specified amount of fractional digits.", ValueName = "decimals", ShowInHelpText = true)]
      public int Decimals { get; set; }

      [Option(CommandOptionType.NoValue, ShortName = "is", LongName = "include-source", Description = "Includes the source location in the result if specified.", ValueName = "source", ShowInHelpText = true)]
      public bool IncludeSource { get; set; }

      [Option(CommandOptionType.SingleValue, ShortName = "i", LongName = "input", Description = "The input file that contains the RDNew locations to project.", ValueName = "input file", ShowInHelpText = true)]
      public string InputFile { get; set; }

      [Option(CommandOptionType.SingleValue, ShortName = "r", LongName = "range", Description = "The Excel range that contains the coordinates to project (only if the input file is an Excel file).", ValueName = "Excel range", ShowInHelpText = true)]
      public string InputRange { get; set; }

      [Option(CommandOptionType.SingleValue, ShortName = "o", LongName = "output", Description = "The output file to which to write the result.", ValueName = "output file", ShowInHelpText = true)]
      public string OutputFile { get; set; }

      [Option(CommandOptionType.SingleValue, ShortName = "h", LongName = "headers", Description = "True to specify the input file contains headers (ignored when the input file is an Excel file).", ValueName = "true/false", ShowInHelpText = true)]
      public bool HasHeaders { get; set; }

      [Option(CommandOptionType.SingleValue, ShortName = "x", LongName = "XY-format", Description = "True to indicate the coordinates are specified in X-Y format, False if they are specified in Y-X format.", ValueName = "true/false", ShowInHelpText = true)]
      public bool XYFormat { get; set; }

      public ToCoordinateCmd(IConsole console)
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
               if (!string.IsNullOrEmpty(Location) && string.IsNullOrEmpty(InputFile) && string.IsNullOrEmpty(OutputFile))
               {
                  // Single location                                                      
                  RDPoint p = PositionParser.RDPointFromString(Location, Separator, XYFormat ? RDPointOrderEnum.XY : RDPointOrderEnum.YX);
                  if (IncludeSource)
                  {
                     Location location = await GISBloxClient.Projection.ToWGS84Complete(p, Decimals > 0 ? Decimals : -1); 
                     OutputToConsole($"X: { location.X } Y: { location.Y } Lat: { location.Lat.ToString(System.Globalization.CultureInfo.InvariantCulture) } Lon: { location.Lon.ToString(System.Globalization.CultureInfo.InvariantCulture) }", ConsoleColor.Green);
                  }
                  else
                  {
                     Coordinate coordinate = await GISBloxClient.Projection.ToWGS84(p, Decimals > 0 ? Decimals : -1);                     
                     OutputToConsole($"Lat: { coordinate.Lat.ToString(System.Globalization.CultureInfo.InvariantCulture) } Lon: { coordinate.Lon.ToString(System.Globalization.CultureInfo.InvariantCulture) }", ConsoleColor.Green);
                  }
                  return 0;
               }
               else if (!string.IsNullOrEmpty(InputFile) && !string.IsNullOrEmpty(OutputFile))
               {
                  // File with locations
                  OutputToConsole($"Processing file '{ InputFile }'...");
                  List<RDPoint> rdPoints = await IO.LoadRDPointsFromFile(InputFile, Separator, HasHeaders, XYFormat ? RDPointOrderEnum.XY : RDPointOrderEnum.YX, InputRange);

                  OutputToConsole("Successfully processed file", ConsoleColor.Green);
                  OutputToConsole("Writing output...");
                  if (IncludeSource)
                  {
                     List<Location> locations = await GISBloxClient.Projection.ToWGS84Complete(rdPoints, Decimals > 0 ? Decimals : -1);
                     await IO.SaveToCSVFile(OutputFile, Separator, locations);
                  }
                  else
                  {
                     List<Coordinate> coordinates = await GISBloxClient.Projection.ToWGS84(rdPoints, Decimals > 0 ? Decimals : -1);
                     await IO.SaveToCSVFile(OutputFile, Separator, rdPoints);
                  }
                  OutputToConsole($"Output saved to file '{ OutputFile }'", ConsoleColor.Green);
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
