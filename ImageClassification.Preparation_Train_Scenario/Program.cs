using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ImageClassification.Preparation_Train_Scenario
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Scenario `{0}` has been started", typeof(Program).Assembly.GetName().Name);

            var stopwatch = Stopwatch.StartNew();
            await Preparation.Program.Main(args);
            await Train.Program.Main(args);
            stopwatch.Stop();

            Console.WriteLine();

            Console.WriteLine("Scenario `{0}` has been finished", typeof(Program).Assembly.GetName().Name);
            Console.WriteLine("Scenario took: {0}", stopwatch.Elapsed);
        }
    }
}
