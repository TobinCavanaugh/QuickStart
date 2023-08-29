using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using Newtonsoft.Json;

namespace QuickStart
{
    public class Program
    {
        public static string version = "1.0";

        public static async Task Main(string[] args)
        {
            var rootCommand = new RootCommand("Quickstart / qs, a simple CLI program quick launcher");


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

            new AliasAddCommands(rootCommand, qss);


            //Splash Screen version ******************************************************************
            {
                var splashCommand = new Command("version", "Version splashscreen");
                rootCommand.Add(splashCommand);

                var splashCommandAlt = new Command("v", "Version splashscreen");
                rootCommand.Add(splashCommandAlt);

                var handleAction = new Action<InvocationContext>(h =>
                {
                    SplashScreen.PrintSplashScreen(SplashScreen.GetSplashScreen());
                    //Console.WriteLine("\n");
                    //                    Console.WriteLine("----------------------------------------");
                    SplashScreen.CenterPrint("-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-");
                    SplashScreen.CenterPrint("[Quickstart]");
                    SplashScreen.CenterPrint($"[Version {version}]");
                    SplashScreen.CenterPrint("[Tobin Cavanaugh]");
                    SplashScreen.CenterPrint(@"[https://github.com/TobinCavanaugh/QuickStart]");
                });

                splashCommand.SetHandler(h => handleAction.Invoke(h));
                splashCommandAlt.SetHandler(h => handleAction.Invoke(h));
            }
            //Run via path ***************************************************************
            {
                var runPath = new Command("p", "Run via path");
                rootCommand.Add(runPath);

                Argument<string> pathArg =
                    new Argument<string>("Path", "The path of the program to run will run with args etc.");
                runPath.AddArgument(pathArg);

                runPath.SetHandler(h =>
                {
                    var path = h.ParseResult.GetValueForArgument(pathArg);

                    if (qss.GetByPath(path, true, out var program))
                    {
                        if (!program.Launch())
                        {
                            Console.Error.WriteLine($"Failed to launch program at path \"{path}\"");
                        }
                    }
                    else
                    {
                        Console.Error.WriteLine($"Failed to find saved program with path \"{path}\"");
                    }
                });
            }

            //Run via alias *****************************************************************
            {
                var runCommand = new Command("a", "Run via alias name");
                rootCommand.Add(runCommand);

                Argument<string> aliasName = new Argument<string>("Alias", "The alias of the program to run");
                runCommand.AddArgument(aliasName);

                runCommand.SetHandler(h =>
                {
                    var alias = h.ParseResult.GetValueForArgument(aliasName);
                    if (qss.GetByAlias(alias, out var program))
                    {
                        if (!program.Launch())
                        {
                            Console.Error.WriteLine(
                                $"Failed to launch program at path \"{program.Path}\". Check there is a program there, if not use `qs pr` to rename it");
                        }
                    }
                    else
                    {
                        Console.Error.WriteLine(
                            $"Failed to run program with alias \"{alias}\". Run `qs al` to see a list of your aliases");
                    }
                });
            }

            //Rename Path *****************************************************************
            {
                var pathCommand = new Command("pr", "Path Replace, replace a path with another");
                rootCommand.Add(pathCommand);
                Argument<string> fromName = new Argument<string>("Old Path", "Original path that will be replaced");
                Argument<string> toName = new Argument<string>("New Path", "New path to replace the old one");
                pathCommand.AddArgument(fromName);
                pathCommand.AddArgument(toName);

                pathCommand.SetHandler(h =>
                {
                    string from = h.ParseResult.GetValueForArgument(fromName);
                    string to = h.ParseResult.GetValueForArgument(toName);

                    if (qss.GetByPath(from, false, out var qProgram))
                    {
                        qProgram.Path = to;
                    }
                    else
                    {
                        Console.Error.WriteLine($"Couldnt find saved path \"{from}\"");
                    }
                });
            }

            //Add Path ************************************************************************
            {
                var addCommand = new Command("+p", "add a path to the aliases");
                rootCommand.Add(addCommand);

                Argument<string> path = new Argument<string>("Path", "The path of the new program");
                addCommand.AddArgument(path);

                Argument<string[]> aliases = new Argument<string[]>("Aliases", "Aliases for the program");
                addCommand.AddArgument(aliases);

                addCommand.SetHandler(h =>
                {
                    qss.AddAliasesPath(h.ParseResult.GetValueForArgument(path),
                        h.ParseResult.GetValueForArgument(aliases), out _);
                });
            }

            //Add Path Browse *****************************************************************
            {
                var browseCommand = new Command("+papp", "Add an existing windows application (Untested)");
                rootCommand.Add(browseCommand);

                //HKEY_CLASSES_ROOT\Extensions\ContractId\Windows.Protocol\PackageId

                browseCommand.SetHandler(h =>
                {
                    // GUID taken from https://learn.microsoft.com/en-us/windows/win32/shell/knownfolderid
                    var FOLDERID_AppsFolder = new Guid("{1e87508d-89c2-42f0-8a7e-645a0f50ca58}");
                    ShellObject appsFolder = (ShellObject)KnownFolderHelper.FromKnownFolderId(FOLDERID_AppsFolder);
                    List<KeyValuePair<string, string>> appNamePathDictionary =
                        new List<KeyValuePair<string, string>>();

                    foreach (var app in (IKnownFolder)appsFolder)
                    {
                        string name = app.Name;
                        string appUserModelID = app.ParsingName;

                        // Accessing the properties of the app
                        var linkPathProperty =
                            app.Properties.GetProperty(SystemProperties.System.Link.TargetParsingPath);

                        string path = linkPathProperty?.ValueAsObject as string;


                        if (path != null && name != null)
                        {
                            appNamePathDictionary.Add(new KeyValuePair<string, string>(path, name));

                            Console.WriteLine(name + " | " + path);
                        }
                    }

                    Console.WriteLine("Add via name (Type `N` to quit):");

                    string userResult = Console.ReadLine()?.Trim().ToLower();

                    if (userResult == "n" || userResult == null || userResult == "")
                    {
                        return;
                    }

                    string resultPath = "";
                    string resultAlias = "";
                    foreach (var app in appNamePathDictionary)
                    {
                        if (app.Value.ToLower().Trim() == userResult)
                        {
                            resultPath = app.Key;
                            resultAlias = app.Value;
                        }
                    }

                    resultPath = resultPath.Trim();

                    if (resultPath == "")
                    {
                        Console.WriteLine($"Couldn't find application with name \"{userResult}\"");
                        return;
                    }

                    qss.AddPath(resultPath, out var prog);
                    prog.aliases.AddUnique(resultAlias);
                });
            }

            //Remove Path **********************************************
            {
                var removeCommand = new Command("-p", "Remove path");
                rootCommand.Add(removeCommand);

                var pathArg = new Argument<string>("Path", "The path to remove");
                removeCommand.AddArgument(pathArg);

                removeCommand.SetHandler(h =>
                {
                    var path = h.ParseResult.GetValueForArgument(pathArg);

                    if (qss.GetByPath(path, false, out var qProgram))
                    {
                        qss.programs.Remove(qProgram);
                    }
                    else
                    {
                        Console.Error.WriteLine($"Failed to find program with path \"{path}\"");
                    }
                });
            }

            //Clear aliases **********************************************
            {
                var clearCommand = new Command("ac", "Clear all aliases from a path");
                rootCommand.Add(clearCommand);

                var pathArg = new Argument<string>("Path", "The path to have the aliases removed from");
                clearCommand.AddArgument(pathArg);

                clearCommand.SetHandler(h =>
                {
                    string path = h.ParseResult.GetValueForArgument(pathArg);
                    if (qss.GetByPath(path, true, out var qProgram))
                    {
                        qProgram.aliases.Clear();
                    }
                    else
                    {
                        Console.Error.WriteLine($"Failed to find program with path \"{path}\"");
                    }
                });
            }

            //Remove Alias ****************************************************
            {
                var removeCommand = new Command("-a", "Remove alias via path");
                rootCommand.Add(removeCommand);

                var targetAliasArg =
                    new Argument<string>("Alias", "The alias to remove alias from. This will also be removed.");
                removeCommand.AddArgument(targetAliasArg);

                var aliasesArg = new Option<string[]>("Aliases", "Aliases to remove from the path");
                removeCommand.Add(aliasesArg);

                removeCommand.SetHandler(h =>
                {
                    string targetAlias = h.ParseResult.GetValueForArgument(targetAliasArg);
                    string[] aliases = h.ParseResult.GetValueForOption(aliasesArg);

                    if (qss.GetByAlias(targetAlias, out var program))
                    {
                        for (int i = 0; i < aliases.Length; i++)
                        {
                            program.aliases.Remove(aliases[i]);
                        }

                        program.aliases.Remove(targetAlias);
                    }
                    else
                    {
                        string aliasCombined = string.Join("\"", aliases);
                        Console.Error.WriteLine($"Failed to find application with aliases \"{aliasCombined}\"");
                    }

                    qss.invalidated = true;
                });
            }

            //List path area *******************************************************
            {
                var listCommand = new Command("pl", "List all paths");

                rootCommand.Add(listCommand);
                listCommand.SetHandler((h) =>
                {
                    foreach (var qprog in qss.programs)
                    {
                        Console.WriteLine(qprog.Path);
                    }
                });
            }

            //List Aliases ************************************************************************************
            {
                var listCommand = new Command("al", "List all paths and aliases");
                rootCommand.Add(listCommand);
                listCommand.SetHandler((h) =>
                {
                    TablePrinter tablePrinter = new TablePrinter();

                    tablePrinter.header = new List<string>() { "Path", "Aliases", "Keywords" };
                    int yOffset = 0;

                    foreach (var program in qss.programs)
                    {
                        tablePrinter.SetCell(yOffset, 0, program.Path);
                        tablePrinter.SetCell(yOffset, 1,
                            "\"" + string.Join("\",\"", program.aliases.ToArray()) + "\"");
                        tablePrinter.SetCell(yOffset, 2, string.Join(",", program.keywords.ToArray()));
                        yOffset++;
                    }

                    Console.WriteLine(tablePrinter.ToString());
                });
            }


            await rootCommand.InvokeAsync(args);
            if (qss.invalidated)
            {
                File.WriteAllText(configPath, JsonConvert.SerializeObject(qss));
            }
        }
    }
}