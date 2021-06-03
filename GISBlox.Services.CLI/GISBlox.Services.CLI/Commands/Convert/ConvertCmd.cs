// ------------------------------------------------------------
// Copyright (c) Bartels Online.  All rights reserved.
// ------------------------------------------------------------

using GISBlox.Services.SDK.Models;
using McMaster.Extensions.CommandLineUtils;
using System;
using System.Threading.Tasks;

namespace GISBlox.Services.CLI.Commands.Convert
{
   [Command(Name = "convert", Description = "Converts WKT geometries into GeoJson.", OptionsComparison = StringComparison.InvariantCultureIgnoreCase)]   
   class ConvertCmd : CmdBase
   {
      [Option(CommandOptionType.SingleValue, ShortName = "w", LongName = "wkt", Description = "The WKT geometry to convert.", ValueName = "wkt", ShowInHelpText = true)]      
      public string Wkt { get; set; }

      [Option(CommandOptionType.SingleValue, ShortName = "g", LongName = "geojson", Description = "The GeoJson to convert.", ValueName = "geojson", ShowInHelpText = false)]
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
