using System.Threading.Tasks;

namespace ImageClassification.Preparation_Train_Scenario
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await Preparation.Program.Main(args);
            await Train.Program.Main(args);
        }
    }
}
