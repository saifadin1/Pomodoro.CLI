using Spectre.Console;
using Spectre.Console.Cli;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pomodoro.Commands
{
    internal class StartCommand : AsyncCommand<StartCommand.Settings>
    {
        public class Settings : CommandSettings
        {
            [CommandArgument(0, "<Name>")]
            public string Name { get; set; }
        }

        public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
        {
            const int totalMinutes = 2; 
            const int updateInterval = 1;
            var totalSeconds = totalMinutes * 60;
            var totalSteps = totalSeconds / updateInterval;

            var SoundManager = new SoundManager();
            await SoundManager.PlayStartSound();

            await AnsiConsole.Progress()
            .StartAsync(async ctx =>
            {
                
                var task1 = ctx.AddTask($"[green]Task: {settings.Name}[/]");
                task1.MaxValue(totalSteps);

                for (int i = 0; i < totalSteps; i++)
                {
                    await Task.Delay(updateInterval * 1000);
                    task1.Increment(1);
                }

                task1.StopTask();

            });
            return 0;
        }
    }
}
