using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Threading.Tasks;

namespace GISBlox.Services.CLI
{
   class Program
   {
      private static async Task<int> Main(string[] args)
      {
         var Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                //.AddJsonFile(AppDomain.CurrentDomain.BaseDirectory + "\\appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

         var builder = new HostBuilder()
             .ConfigureServices((hostContext, services) =>
             {
               
             });

         try
         {
            return await builder.RunCommandLineApplicationAsync<Cmd>(args);
         }
         catch (Exception ex)
         {
            Console.WriteLine(ex.Message);
            return 1;
         }
      }
   }
}
