using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pomodoro.Helpers
{
    internal class Colors 
    {
        public static Color GetRandomColor()
        {
            Random rand = new Random();
            int red = rand.Next(256);
            int green = rand.Next(256);  
            int blue = rand.Next(256); 
            return Color.FromArgb(red, green, blue);
        }
    }
}
