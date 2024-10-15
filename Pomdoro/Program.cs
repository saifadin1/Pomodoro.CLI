using Microsoft.Extensions.Configuration;
using Spectre.Console.Cli;
using static Pomodoro.Commands.StartCommand;
using Newtonsoft.Json;

namespace Pomodoro
{
    internal class Program
    {
        public static string json = File.ReadAllText("settings.json");
        public static dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
        static void Main(string[] args)
        {
            var app = new CommandApp();

            app.Configure(config =>
            {
                config.AddCommand<Commands.StartCommand>("start");
            });

            app.Run(args);
        }
        
    }
}
