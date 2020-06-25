using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Authentication.Common
{
    public class ColourUtil
    {
        public static string[] colours = new string[] { "#c00", "#FF02FF", "FF0", "#ccc", "#0cc", "#0c0", "#00c", "#FF8F00", "#055F0C", "#5F3B0D", "#8F00FF", "#B6FFCE", "#AF8A13" };

        public static string GetRandomColour()
        {
            Random rnd = new Random();
            var i =  rnd.Next(0, 12);
            return colours[i];
        }
    }
}