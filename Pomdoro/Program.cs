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
                config.AddCommand<Commands.StateCommand>("state");
            });

            app.Run(args);
        }

        public static void ClearCurrentTask()
        {
            Program.jsonObj["current task"]["name"] = "";
            Program.jsonObj["current task"]["type"] = "";
            Program.jsonObj["current task"]["timeLeft"] = "";
        }

        public static void SaveConfiguration()
        {
            string output = Newtonsoft.Json.JsonConvert.SerializeObject(Program.jsonObj, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText("settings.json", output);
        }

    }
}
