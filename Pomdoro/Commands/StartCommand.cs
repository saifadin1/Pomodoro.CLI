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
            var SoundManager = new SoundManager();



            var choiceOfWorkSession = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Do you want to start a work session rigt now?")
                .PageSize(10)
                .AddChoices(new[] {
                    "Yes" , "No"
                }));

            await SoundManager.PlayStartSound();


            if (choiceOfWorkSession == "No")
            {
                return 0;
            }

            var totalWorkMinutes = Double.Parse(Program.Configuration["time:work"]);
            var updateInterval = 1;
            var totalWorkSeconds = totalWorkMinutes * 60;
            var totalSteps = totalWorkSeconds / updateInterval;


            Program.Configuration["current task name"] = settings.Name;
            Program.Configuration["current task type"] = "work";
            Program.Configuration["currunt task time left"] = Program.Configuration["time:work"];


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

            await SoundManager.PlayEndSound();

            var choiceOfBreakSession = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Do you want to start a break?")
                .PageSize(10)
                .AddChoices(new[] {
                    "Yes" , "No"
                }));

            await SoundManager.PlayStartSound();


            if (choiceOfBreakSession == "No")
            {
                Program.Configuration["current task type"] = null;
                Program.Configuration["currunt task time left"] = null;
                Program.Configuration["current task name"] = null;
                Program.Configuration["current task type"] = null;
                Program.Configuration["completed sessions"] = "0";
                return 0;
            }
            else
            { 
                string completedSessions = Program.Configuration["completed sessions"];
                int completedSessionsInt = Int32.Parse(completedSessions);
                completedSessionsInt++;
                Program.Configuration["completed sessions"] = completedSessionsInt.ToString();

                bool isLongBreak = (completedSessionsInt % 4 == 0);

                var totalBreakMinutes = Double.Parse(isLongBreak ? Program.Configuration["time:LongBreak"] 
                                                                 : Program.Configuration["time:shortBreak"]);
                var totalBreakSeconds = totalBreakMinutes * 60;
                totalSteps = totalBreakSeconds / updateInterval;


                Program.Configuration["current task type"] = isLongBreak ? "LongBreak":"shortBreak";
                Program.Configuration["currunt task time left"] = isLongBreak ? Program.Configuration["time:LongBreak"]
                                                                               :Program.Configuration["time:shortBreak"];

                await AnsiConsole.Progress()
                .StartAsync(async ctx =>
                {

                    var task1 = ctx.AddTask($"[green]Break: {settings.Name}[/]");
                    task1.MaxValue(totalSteps);

                    for (int i = 0; i < totalSteps; i++)
                    {
                        await Task.Delay(updateInterval * 1000);
                        task1.Increment(1);
                    }

                    task1.StopTask();

                });

                Program.Configuration["current task type"] = null;
                Program.Configuration["currunt task time left"] = null;

                await SoundManager.PlayEndSound();

                await ExecuteAsync(context, settings);
            }

            return 0;
        }
    }
}
