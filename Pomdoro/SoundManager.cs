using Microsoft.Extensions.Configuration;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pomodoro
{
    internal class SoundManager
    {   
        public async Task PlayStartSound()
        {
            try
            {
                string relativePath = Program.Configuration["sounds:start"];

                string filePath = Path.Combine(AppContext.BaseDirectory, relativePath.TrimStart('~', '/'));

                if (filePath == null)
                {
                    Console.WriteLine("No sound file path found in settings.json");
                    return;
                }
                using (var audioFile = new AudioFileReader(filePath))
                using (var outputDevice = new WaveOutEvent())
                {
                    outputDevice.Init(audioFile);
                    outputDevice.Play();

                    while (outputDevice.PlaybackState == PlaybackState.Playing)
                    {
                        await Task.Delay(500);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public async Task PlayEndSound()
        {
            try
            {
                string relativePath = Program.Configuration["sounds:end"];

                string filePath = Path.Combine(AppContext.BaseDirectory, relativePath.TrimStart('~', '/'));

                if (filePath == null)
                {
                    Console.WriteLine("No sound file path found in settings.json");
                    return;
                }
                using (var audioFile = new AudioFileReader(filePath))
                using (var outputDevice = new WaveOutEvent())
                {
                    outputDevice.Init(audioFile);
                    outputDevice.Play();

                    while (outputDevice.PlaybackState == PlaybackState.Playing)
                    {
                        await Task.Delay(500);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
