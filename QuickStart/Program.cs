using System.CommandLine;
using System.Threading.Tasks;

namespace QuickStart
{
    public class Program
    {
        public static string version = "1.0";

        public static async Task Main(string[] args)
        {
            var rootCommand = new RootCommand("Quickstart / qs, a simple CLI program quick launcher");

            var qss = JsonHandler.GetQuickstartSave();

            var ac = new AliasCommands(rootCommand, qss);
            var pc = new PathCommands(rootCommand, qss);
            var sc = new SpecialCommands(rootCommand, qss, version);

            await rootCommand.InvokeAsync(args);

            JsonHandler.UpdateQuickstartSave(qss);
        }
    }
}