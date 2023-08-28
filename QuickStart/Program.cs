using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Remoting.Proxies;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace QuickStart
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var rootCommand = new RootCommand("Quickstart / qs, a simple CLI program quick launcher");


            var runViaPath = new Command("+p", "add a path to the aliases");

            var runViaAlias = new Command("a", "r");


            rootCommand.Add(runViaAlias);
            rootCommand.Add(runViaPath);


            rootCommand.Add(new Command("VV", "BBBBBBBBBBBBB"));

            //Get the config path
            string configPath =
                $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\quickstart.json";

            if (!File.Exists(configPath))
            {
                //Create a file there if needed
                File.CreateText(configPath).Close();
            }

            //Read the qss file
            var qss = JsonConvert.DeserializeObject<QuickstartSave>(File.ReadAllText(configPath));

            if (qss == null)
            {
                qss = new QuickstartSave();
            }


            var addCom = new Command("+",
                "Add a path, alias, keyword, etc. Choose +a for alias, +k for keyword, +p for path");

            new AliasAddCommands(rootCommand, qss);
            rootCommand.Add(addCom);


            //List path area *******************************************************
            var listPath = new Command("pl", "List all paths");
            rootCommand.Add(listPath);
            listPath.SetHandler((h) =>
            {
                foreach (var qprog in qss.programs)
                {
                    Console.WriteLine(qprog.path);
                }
            });
            //************************************************************************************
            var listAliases = new Command("al", "List all paths and aliases");
            rootCommand.Add(listAliases);
            listAliases.SetHandler((h) =>
            {
                TablePrinter tablePrinter = new TablePrinter();

                tablePrinter.header = new List<string>() { "Path", "Aliases", "Keywords" };
                int yOffset = 0;

                foreach (var program in qss.programs)
                {
                    tablePrinter.SetCell(yOffset, 0, program.path);
                    tablePrinter.SetCell(yOffset, 1, "\"" + string.Join("\",\"", program.aliases.ToArray()) + "\"");
                    tablePrinter.SetCell(yOffset, 2, string.Join(",", program.keywords.ToArray()));
                    yOffset++;
                }

                Console.WriteLine(tablePrinter.ToString());
            });


            await rootCommand.InvokeAsync(args);

            if (qss.invalidated)
            {
                File.WriteAllText(configPath, JsonConvert.SerializeObject(qss));
            }
        }
    }
}