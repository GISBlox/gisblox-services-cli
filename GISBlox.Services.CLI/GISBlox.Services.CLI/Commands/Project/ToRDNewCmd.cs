using GISBlox.Services.CLI.Utils;
using GISBlox.Services.SDK.Models;
using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GISBlox.Services.CLI.Commands.Project
{
   [Command(Name = "to-rds", Description = "Converts coordinates to Rijksdriehoekstelsel (RDNew) locations. Projects a single coordinate, or batch-processes a file with coordinates.", 
            OptionsComparison = StringComparison.InvariantCultureIgnoreCase)]
   class ToRDNewCmd : CmdBase
   {
      [Option(CommandOptionType.SingleValue, ShortName = "c", LongName = "coord", Description = "The coordinate to project to RDNew.", ValueName = "coordinate", ShowInHelpText = true)]
      public string Coordinate { get; set; }
      
      [Option(CommandOptionType.SingleValue, ShortName = "s", LongName = "sep", Description = "Specifies the coordinate separator, e.g. \";\".", ValueName = "coordinate separator", ShowInHelpText = true)]
      public string Separator { get; set; }

      [Option(CommandOptionType.NoValue, ShortName = "is", LongName = "include-source", Description = "Includes the source coordinate in the result if specified.", ValueName = "source", ShowInHelpText = true)]
      public bool IncludeSource { get; set; }

      [Option(CommandOptionType.SingleValue, ShortName = "i", LongName = "input", Description = "The input file that contains the coordinates to project.", ValueName = "input file", ShowInHelpText = true)]
      public string InputFile { get; set; }

      [Option(CommandOptionType.SingleValue, ShortName = "o", LongName = "output", Description = "The output file to which to write the result.", ValueName = "output file", ShowInHelpText = true)]
      public string OutputFile { get; set; }

      [Option(CommandOptionType.SingleValue, ShortName = "f", LongName = "format", Description = "The file format to use (CSV or XLS).", ValueName = "file format", ShowInHelpText = true)]
      public string FileFormat { get; set; }

      [Option(CommandOptionType.SingleValue, ShortName = "h", LongName = "headers", Description = "True to specify the input file contains headers.", ValueName = "headers", ShowInHelpText = true)]
      public bool HasHeaders { get; set; }

      [Option(CommandOptionType.SingleValue,  ShortName = "l", LongName = "lat-lon", Description = "True to specify the coordinates are in lat-lon format, False if they are in lon-lat format (file only).", ValueName = "lat-lon", ShowInHelpText = true)]
      public bool LatLonFormat { get; set; }

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
                  Coordinate c = PositionParser.CoordinateFromString(Coordinate, Separator, LatLonFormat ? CoordinateOrderEnum.LatLon : CoordinateOrderEnum.LonLat);
                  if (IncludeSource)
                  {
                     Location location = await GISBloxClient.Projection.ToRDSComplete(c);
                     OutputToConsole($"Lat: { location.Lat.ToString(System.Globalization.CultureInfo.InvariantCulture) } Lon: { location.Lon.ToString(System.Globalization.CultureInfo.InvariantCulture) } X:{ location.X } Y:{ location.Y }", ConsoleColor.Green);
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
                  // Validate parameter(s)
                  if (string.IsNullOrEmpty(Separator))
                  {
                     throw new ArgumentNullException(nameof(Separator));
                  }
                  
                  // Process file
                  OutputToConsole($"Processing file '{ InputFile }'...");                  
                  List<Coordinate> coordinates = await IO.LoadCoordinatesFromFile(InputFile, Separator, HasHeaders, LatLonFormat ? CoordinateOrderEnum.LatLon : CoordinateOrderEnum.LonLat, FileFormat);
                  
                  OutputToConsole("Successfully processed file", ConsoleColor.Green);
                  OutputToConsole("Writing output...");
                  if (IncludeSource)
                  {
                     List<Location> locations = await GISBloxClient.Projection.ToRDSComplete(coordinates);
                     await IO.SaveToCSVFile(OutputFile, Separator, locations);                     
                  }
                  else
                  {
                     List<RDPoint> rdPoints = await GISBloxClient.Projection.ToRDS(coordinates);
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
