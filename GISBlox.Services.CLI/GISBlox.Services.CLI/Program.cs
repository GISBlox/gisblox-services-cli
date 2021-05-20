using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using GISBlox.Services.SDK;

namespace GISBlox.Services.CLI
{
   class Program
   {
      private static async Task<int> Main(string[] args)
      {
         var builder = new HostBuilder()
             .ConfigureServices((hostContext, services) =>
             {
               
             });

         try
         {
            return await builder.RunCommandLineApplicationAsync<gbsCmd>(args);
         }
         catch (Exception ex)
         {
            Console.WriteLine(ex.Message);
            return 1;
         }
      }
   }
}
