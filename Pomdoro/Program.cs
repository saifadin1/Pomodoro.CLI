using Microsoft.Extensions.Configuration;
using Spectre.Console.Cli;

namespace Pomodoro
{
    internal class Program
    {
        public static IConfiguration Configuration { get; private set; }
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("settings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();


            var app = new CommandApp();

            app.Configure(config =>
            {
                config.AddCommand<Commands.StartCommand>("start");
            });

            app.Run(args);
        }
    }
}
