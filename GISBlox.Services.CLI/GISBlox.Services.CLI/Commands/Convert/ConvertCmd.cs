using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GISBlox.Services.SDK.Models;

namespace GISBlox.Services.CLI.Commands.Convert
{
   [Command(Name = "convert", Description = "Convert WKT geometries into GeoJson")]
   class ConvertCmd : CmdBase
   {
      [Option(CommandOptionType.SingleValue, ShortName = "w", LongName = "wkt", Description = "WKT geometry", ValueName = "wkt", ShowInHelpText = true)]
      public string Wkt { get; set; }

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
               if (string.IsNullOrEmpty(Wkt))
               {
                  Wkt = Prompt.GetString("WKT:", Wkt);
               }
               OutputToConsole($"Converting WKT '{ Wkt }' to GeoJson...");

               var geoJson = await GISBloxClient.Conversion.ToGeoJson(new WKT(Wkt), false);

               OutputJson(geoJson);
               return 0;
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
