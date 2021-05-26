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
   [Command(Name = "to-rds", Description = "Converts coordinates to Rijksdriehoekstelsel (RDNew) locations.")]

   class ToRDNewCmd : CmdBase
   {
      [Option(CommandOptionType.SingleValue, ShortName = "c", LongName = "coord", Description = "coordinate", ValueName = "coordinate", ShowInHelpText = true)]
      public string Coordinate { get; set; }
      
      [Option(CommandOptionType.SingleValue, ShortName = "s", LongName = "sep", Description = "coordinate separator", ValueName = "coordinate separator", ShowInHelpText = true)]
      public string Separator { get; set; }

      [Option(CommandOptionType.NoValue, ShortName = "is", LongName = "source", Description = "include source", ValueName = "source", ShowInHelpText = true)]
      public bool IncludeSource { get; set; }

      [Option(CommandOptionType.SingleValue,  ShortName = "l", LongName = "lat-lon", Description = "lat-lon format", ValueName = "lat-lon", ShowInHelpText = true)]
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
                  Coordinate c = PositionParser.CoordinateFromString(Coordinate, Separator, LatLonFormat ? CoordinateOrder.LatLon : CoordinateOrder.LonLat);
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
                  OutputToConsole("Processing file...");
                  List<Coordinate> coordinates = await IO.LoadCoordinatesFromCSVFile(InputFile, Separator, true, LatLonFormat ? CoordinateOrder.LatLon : CoordinateOrder.LonLat);
                  
                  OutputToConsole("Writing output...");
                  if (IncludeSource)
                  {
                     List<Location> locations = await GISBloxClient.Projection.ToRDSComplete(coordinates);
                     
                     //locations.ForEach(l => OutputToConsole($"Lat: { l.Lat } Lon: { l.Lon } X:{ l.X } Y:{ l.Y }", ConsoleColor.Green));
                  }
                  else
                  {
                     List<RDPoint> rdPoints = await GISBloxClient.Projection.ToRDS(coordinates);
                     //rdPoints.ForEach(p => OutputToConsole($"X:{ p.X } Y:{ p.Y }", ConsoleColor.Green));
                  }
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
