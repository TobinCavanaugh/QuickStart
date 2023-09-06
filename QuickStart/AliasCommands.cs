using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;

namespace QSn
{
    public class AliasCommands
    {
        public AliasCommands(RootCommand rootCommand, QuickstartSave qss)
        {
            //Adding an alias via a path ******************************************************************************
            {
                var addAliasViaPath = new Command("+a",
                    "Creates/Adds an alias based on the Path argument. Can take any amount of aliases.");
                var aliasPathArg =
                    new Argument<string>("Path", "The path of the application, put in double quotes! \"C:\\\"");
                var aliasesArg = new Argument<string[]>("Aliases", "All the aliases you want to add.");
                addAliasViaPath.AddArgument(aliasPathArg);
                addAliasViaPath.AddArgument(aliasesArg);

                addAliasViaPath.SetHandler((h) =>
                {
                    var path = h.ParseResult.GetValueForArgument(aliasPathArg);
                    var aliases = h.ParseResult.GetValueForArgument(aliasesArg);

                    //If its not a pre-existing program
                    if (!qss.AddAliasesPath(path, aliases, out _))
                    {
                        Console.WriteLine($"{path} is not added... Adding it with aliases");
                    }
                });

                rootCommand.Add(addAliasViaPath);
            }

            //Adding an alias via an alias ***********************************************************
            {
                var addAliasViaAlias = new Command("+aa",
                    "Adds an alias to a pre-existing program using one of it's aliases.");
                var initialAliasName =
                    new Argument<string>("Initial Alias", "This is the alias of the program being added to.");
                var aliasesArg = new Argument<string[]>("Aliases", "The aliases to be added.");
                addAliasViaAlias.AddArgument(initialAliasName);
                addAliasViaAlias.AddArgument(aliasesArg);

                addAliasViaAlias.SetHandler((h) =>
                {
                    if (qss.GetByAlias(h.ParseResult.GetValueForArgument(initialAliasName), out var program))
                    {
                        var naliases = h.ParseResult.GetValueForArgument(aliasesArg);
                        if (naliases != null && naliases.Length > 0)
                        {
                            program.aliases.AddUniqueRange(naliases);
                        }
                    }
                });

                rootCommand.Add(addAliasViaAlias);
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
                    new Argument<string>("Alias",
                        "The alias to remove alias from. This will be removed if you include no other aliases.");
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

                        if (aliases.Length > 0)
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


            //List Aliases ************************************************************************************
            {
                var listCommand = new Command("al", "List all paths and aliases");
                rootCommand.Add(listCommand);
                listCommand.SetHandler((h) =>
                {
                    TablePrinter tablePrinter = new TablePrinter();

                    tablePrinter.header = new List<string>() { "Path", "Aliases", "Keywords", "Admin"};
                    int yOffset = 0;

                    var sorted = qss.programs.OrderBy(x =>
                    {
                        if (x.aliases.Count > 0)
                            return x.aliases[0];
                        else return "";
                    });

                    foreach (var program in sorted)
                    {
                        tablePrinter.SetCell(yOffset, 0, program.Path);
                        tablePrinter.SetCell(yOffset, 1,
                            "\"" + string.Join("\",\"", program.aliases.ToArray()) + "\"");
                        tablePrinter.SetCell(yOffset, 2, string.Join(",", program.keywords.ToArray()));
                        yOffset++;
                        tablePrinter.SetCell(yOffset, 3, program.useAdmin.ToString());
                    }

                    Console.WriteLine(tablePrinter.ToString());
                });
            }

            {
                var adminCommand = new Command("ar", "Admin Run, runs the program as admin");
                rootCommand.Add(adminCommand);

                var aliasArg = new Argument<string>("Alias", "The name of the alias to change");
                adminCommand.AddArgument(aliasArg);
                
                var stateArg = new Argument<bool>("State", "True runs as admin, False runs as regular");
                adminCommand.AddArgument(stateArg);
                
                adminCommand.SetHandler(h =>
                {

                    bool state = h.ParseResult.GetValueForArgument(stateArg);
                    string alias = h.ParseResult.GetValueForArgument(aliasArg);

                    if (qss.GetByAlias(alias, out QProgram program))
                    {
                        program.useAdmin = state;
                    }
                });
            }
        }
    }
}