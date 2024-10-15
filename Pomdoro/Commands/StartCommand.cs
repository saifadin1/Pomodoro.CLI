using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
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

            await SoundManager.PlaySoundAsync(Program.jsonObj["sounds"]["start"]);


            if (choiceOfWorkSession == "No")
            {
                Program.ClearCurrentTask();
                Program.SaveConfiguration();
                return 0;
            }


            double totalWorkMinutes = Double.Parse(Program.jsonObj["time"]["work"].ToString());
            var updateInterval = 1;
            var totalWorkSeconds = totalWorkMinutes * 60;
            var totalSteps = totalWorkSeconds / updateInterval;

            Program.jsonObj["current task"]["name"] = settings.Name;
            Program.jsonObj["current task"]["type"] = "work";
            Program.jsonObj["current task"]["timeLeft"] = Program.jsonObj["time"]["work"];


            //SoundManager.Play(Program.jsonObj["sounds"]["background"]);

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

            await SoundManager.PlaySoundAsync(Program.jsonObj["sounds"]["end"]);

            var choiceOfTaskEnded = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
            .Title($"Did you finish the task: {Program.jsonObj["current task"]["name"]}?")
            .PageSize(10)
            .AddChoices(new[] {
                "Yes finally!" + Emoji.Known.GrinningFaceWithSmilingEyes,
                "Not yet" + Emoji.Known.SadButRelievedFace
            }));

            if(choiceOfTaskEnded == "Yes finally!" + Emoji.Known.GrinningFaceWithSmilingEyes)
            {
                JArray completedTasks = Program.jsonObj["completed tasks"] as JArray;

                if (completedTasks == null)
                {
                    completedTasks = new JArray();
                    Program.jsonObj["completed tasks"] = completedTasks;
                }


                JObject completedTask = new JObject
                {
                    { "name", Program.jsonObj["current task"]["name"].ToString() },
                    { "timeSpent", Program.jsonObj["current task"]["time spent"].ToString() }  
                };

                completedTasks.Add(completedTask);

                var newTask = AnsiConsole.Prompt(new TextPrompt<string>("Well done!\n so, What's the new task?" + Emoji.Known.CowboyHatFace));
                Program.jsonObj["current task"]["name"] = newTask;
            }

            double timeSpent = totalWorkMinutes;
            if(Double.TryParse(Program.jsonObj["current task"]["time spent"].ToString(), out double time)){
                timeSpent += time;
            }
            Program.jsonObj["current task"]["time spent"] = timeSpent.ToString();
            var choiceOfBreakSession = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Do you want to start a break?")
                .PageSize(10)
                .AddChoices(new[] {
                    "Yes", "No" , "Skip"
                }));

            await SoundManager.PlaySoundAsync(Program.jsonObj["sounds"]["start"]);


            if (choiceOfBreakSession == "No")
            {
                Program.ClearCurrentTask();
                Program.SaveConfiguration();
                return 0;
            } 
            else if(choiceOfBreakSession == "Skip")
            {
                await ExecuteAsync(context, settings);
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

            Program.ClearCurrentTask();
            Program.SaveConfiguration();

            await SoundManager.PlaySoundAsync(Program.jsonObj["sounds"]["end"]);

            await ExecuteAsync(context, settings);
            return 0;
        }

        
    }
}
