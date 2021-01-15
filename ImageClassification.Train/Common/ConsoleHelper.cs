using System;

namespace ImageClassification.Train.Common
{
    public static class ConsoleHelper
    {
        private static object _lock = new object();

        public static void ColorWriteLine(ConsoleColor color, object obj)
        {
            lock (_lock)
            {
                var temp = Console.ForegroundColor;
                Console.ForegroundColor = color;
                Console.WriteLine(obj);
                Console.ForegroundColor = temp;
            }
        }
        public static void ColorWriteLine(ConsoleColor color, string format, params object[] args)
        {
            lock (_lock)
            {
                var temp = Console.ForegroundColor;
                Console.ForegroundColor = color;
                Console.WriteLine(format, args);
                Console.ForegroundColor = temp;
            }
        }
    }
}
