using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace GISBlox.Services.CLI.Commands
{
   [Command(Name = "login", Description = "Login to GISBlox Location Services.")]
   class LoginCmd : CmdBase
   {
      [Option(CommandOptionType.SingleValue, ShortName = "", LongName = "with-token", Description = "GISBlox Services service key", ValueName = "service key", ShowInHelpText = true)]
      public string Token { get; set; }

      public LoginCmd(IConsole console)
      {
         _console = console;
      }

      protected override async Task<int> OnExecute(CommandLineApplication app)
      {
         if (string.IsNullOrEmpty(Token))
         {  
            Token = Prompt.GetString("Service token:", Token);
         }

         try
         {
            var userProfile = new UserProfile()
            {
               ServiceKey = Encrypt(Token)               
            };

            if (!Directory.Exists(ProfileFolder))
            {
               Directory.CreateDirectory(ProfileFolder);
            }

            await File.WriteAllTextAsync($"{ProfileFolder}default", JsonSerializer.Serialize(userProfile, typeof(UserProfile)), UTF8Encoding.UTF8);
            return 0;
         }
         catch (Exception ex)
         {
            OnException(ex);
            return 1;
         }
      }     
   }
}
