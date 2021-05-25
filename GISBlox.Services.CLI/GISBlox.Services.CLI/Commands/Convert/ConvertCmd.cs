using GISBlox.Services.SDK.Models;
using McMaster.Extensions.CommandLineUtils;
using System;
using System.Threading.Tasks;

namespace GISBlox.Services.CLI.Commands.Convert
{
   [Command(Name = "convert", Description = "Convert WKT geometries into GeoJson")]   
   class ConvertCmd : CmdBase
   {
      [Option(CommandOptionType.SingleValue, ShortName = "w", LongName = "wkt", Description = "WKT geometry", ValueName = "wkt", ShowInHelpText = true)]      
      public string Wkt { get; set; }

      [Option(CommandOptionType.SingleValue, ShortName = "g", LongName = "geojson", Description = "GeoJson", ValueName = "geojson", ShowInHelpText = true)]
      public string GeoJson { get; set; }

      public ConvertCmd(IConsole console)
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
               if (!string.IsNullOrEmpty(Wkt) && string.IsNullOrEmpty(GeoJson))
               {
                  OutputToConsole($"Converting WKT '{ Wkt }' to GeoJson...");

                  var geoJson = await GISBloxClient.Conversion.ToGeoJson(new WKT(Wkt), false);
                  OutputJson(geoJson);
                  return 0;
               }
               else if (string.IsNullOrEmpty(Wkt) && !string.IsNullOrEmpty(GeoJson))
               {
                  OutputToConsole($"Converting GeoJson to WKT...");

                  Output("Not implemented.");
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
