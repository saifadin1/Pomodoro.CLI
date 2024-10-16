using Newtonsoft.Json.Linq;
using Pomodoro.Helpers;
using Spectre.Console;
using Spectre.Console.Cli;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pomodoro.Commands
{
    internal class StateCommand : AsyncCommand<StateCommand.Settings>
    {
        public class Settings : CommandSettings
        {
            
        }

        public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
        {
            Program.SaveConfiguration();

            int CompletedSessions = Int32.Parse(Program.jsonObj["completedSessions"].ToString());
            double TotalWorkTime = Double.Parse(Program.jsonObj["time"]["work"].ToString()) * CompletedSessions / 60;
            var completedTasksList = Program.jsonObj["completed tasks"] as JArray;
            
            BarChart barChart = new BarChart();
            foreach(var task in completedTasksList)
            {
                string name = task["name"].ToString();
                double timeSpent = 0;
                if(Double.TryParse((task["timeSpent"].ToString()), out double time))
                {
                    timeSpent = time;
                }

                Random rnd = new Random();
                timeSpent /= 60;
                barChart.AddItem(name, Math.Round(timeSpent , 2), Color.FromInt32(rnd.Next(255)));
            }

            AnsiConsole.Write(barChart
                .Width(60)
                .Label($"[green bold underline]Total time: {Math.Round(TotalWorkTime , 2)}[/]")
                .CenterLabel());

            return 0;
        }
    }
}
