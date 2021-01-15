using System;

namespace ImageClassification.Train.Common
{
    public static class ConsoleHelper
    {
        public static void ColorWriteLine(ConsoleColor color, object obj)
        {
            var temp = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(obj);
            Console.ForegroundColor = temp;
        }
        public static void ColorWriteLine(ConsoleColor color, string format, params object[] args)
        {
            var temp = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(format, args);
            Console.ForegroundColor = temp;
        }
    }
}
