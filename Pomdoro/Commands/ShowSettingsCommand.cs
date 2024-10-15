using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pomodoro.Commands
{
    internal class ShowSettingsCommand : AsyncCommand<ShowSettingsCommand.Settings>
    {
        public class Settings : CommandSettings
        {
        }

        public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
        {
            var json = new JsonText(File.ReadAllText("settings.json"));
            AnsiConsole.Write(
            new Panel(json)
                .Header("Settings")
                .Collapse()
                .RoundedBorder()
                .BorderColor(Color.Yellow));

            return 0;
        }
    }
   
}
