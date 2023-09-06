using System;
using System.CommandLine;
using System.Linq;

namespace QSn
{
    public class PathCommands
    {
        public PathCommands(RootCommand rootCommand, QuickstartSave qss)
        {
            //Run via path ***************************************************************
            {
                var runPath = new Command("p", "Run via a path in the program list.");
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


            //Add Path ************************************************************************
            {
                var addCommand = new Command("+p",
                    "Add a path to the program list. Can include aliases. Put path in double quotes \"\"");
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


            //Rename Path *****************************************************************
            {
                var pathCommand = new Command("pr",
                    "Path Rename. Renames the Old Path to the New Path, maintains aliases, args, etc.");
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


            //Remove Path **********************************************
            {
                var removeCommand = new Command("-p",
                    "Remove program by it's path. This is permanent and removes aliases, arguments, and everything associated with this path.");
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

            //List path area *******************************************************
            {
                var listCommand = new Command("pl", "List all paths");

                rootCommand.Add(listCommand);
                listCommand.SetHandler((h) =>
                {
                    var sorted = qss.programs.OrderBy(x => x.Path);
                    foreach (var qprog in sorted)
                    {
                        Console.WriteLine(qprog.Path);
                    }
                });
            }

            //Remove path via alias
            {
                var removeCommand = new Command("-pa", "Remove path via one of it's aliases");
                rootCommand.Add(removeCommand);

                var aliasArg = new Argument<string>("Alias", "The alias to remove");
                removeCommand.AddArgument(aliasArg);

                removeCommand.SetHandler(h =>
                {
                    var alias = h.ParseResult.GetValueForArgument(aliasArg);

                    if (qss.GetByAlias(alias, out var program))
                    {
                        qss.RemoveProgramViaPath(program.Path);
                    }
                    else
                    {
                        Console.Error.WriteLine($"Failure to find program with alias \"{alias}\"");
                    }
                });
            }
        }
    }
}