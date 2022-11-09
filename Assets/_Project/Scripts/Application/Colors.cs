using UnityEngine;

namespace Project.Application
{
    public class Colors
    {
        public const string Positive = "#5EBC01";
        public const string Negative = "#F30035";
        public const string White = "#FAF8E0";

        public static Color GetColor(string hex)
        {
             if (ColorUtility.TryParseHtmlString(hex, out var color))
             {
                 return color;
             }
             
             return Color.magenta;
        }
        
    }
}