using Microsoft.Extensions.Configuration;
using Spectre.Console;
using Spectre.Console.Cli;
using System;
using System.IO;
using System.Text.Json.Nodes;
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
                .Title("Do you want to start a work session right now?")
                .PageSize(10)
                .AddChoices(new[] {
                    "Yes", "No"
                }));

            await SoundManager.PlayStartSound();

            if (choiceOfWorkSession == "No")
            {
                ClearCurrentTask();
                SaveConfiguration();
                return 0;
            }


            double totalWorkMinutes = Double.Parse(Program.jsonObj["time"]["work"].ToString());
            var updateInterval = 1;
            var totalWorkSeconds = totalWorkMinutes * 60;
            var totalSteps = totalWorkSeconds / updateInterval;

            Program.jsonObj["current task"]["name"] = settings.Name;
            Program.jsonObj["current task"]["type"] = "work";
            Program.jsonObj["current task"]["timeLeft"] = Program.jsonObj["time"]["work"];

            await AnsiConsole.Progress()
            .StartAsync(async ctx =>
            {
                var task1 = ctx.AddTask($"[green]Task: {settings.Name}[/]");
                task1.MaxValue((int)totalSteps);

                for (int i = 0; i < totalSteps; i++)
                {
                    await Task.Delay(updateInterval * 1000);
                    task1.Increment(1);
                }

                task1.StopTask();
            });

            

            int completedSessionsInt = Int32.Parse(Program.jsonObj["completedSessions"].ToString());
            completedSessionsInt++;
            Program.jsonObj["completedSessions"] = completedSessionsInt.ToString();

            await SoundManager.PlayEndSound();

            var choiceOfBreakSession = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Do you want to start a break?")
                .PageSize(10)
                .AddChoices(new[] {
                    "Yes", "No"
                }));

            await SoundManager.PlayStartSound();

            if (choiceOfBreakSession == "No")
            {
                ClearCurrentTask();
                SaveConfiguration();
                return 0;
            }
            bool isLongBreak = (completedSessionsInt % 4 == 0);
            var totalBreakMinutes = Double.Parse(isLongBreak ? Program.jsonObj["time"]["longBreak"].ToString()
                                                                : Program.jsonObj["time"]["shortBreak"].ToString());
            var totalBreakSeconds = totalBreakMinutes * 60;
            totalSteps = totalBreakSeconds / updateInterval;

            Program.jsonObj["current task"]["type"] = isLongBreak ? "longBreak" : "shortBreak";
            Program.jsonObj["current task"]["timeleft"] = isLongBreak ? Program.jsonObj["time"]["longBreak"].ToString()
                                                                            : Program.jsonObj["time"]["shortBreak"].ToString();


            Console.WriteLine("Break started");
            await AnsiConsole.Progress()
            .StartAsync(async ctx =>
            {
                var task1 = ctx.AddTask($"[green]Break: {settings.Name}[/]");
                task1.MaxValue((int)totalSteps);

                for (int i = 0; i < totalSteps; i++)
                {
                    await Task.Delay(updateInterval * 1000);
                    task1.Increment(1);
                }

                task1.StopTask();
            });

            ClearCurrentTask();
            SaveConfiguration();

            await SoundManager.PlayEndSound();

            await ExecuteAsync(context, settings);
            return 0;
        }

        private void ClearCurrentTask()
        {
            Program.jsonObj["current task"]["name"] = "";
            Program.jsonObj["current task"]["type"] = "";
            Program.jsonObj["current task"]["timeLeft"] = "";
        }

        private void SaveConfiguration()
        {
            string output = Newtonsoft.Json.JsonConvert.SerializeObject(Program.jsonObj, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText("settings.json", output);
        }
    }
}
