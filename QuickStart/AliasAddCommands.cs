using System;
using System.CommandLine;
using System.Linq;
using System.Runtime.CompilerServices;

namespace QuickStart
{
    public class AliasAddCommands
    {
        public Command rootCommand;

        public AliasAddCommands(Command _rootCommand, QuickstartSave qss)
        {
            rootCommand = _rootCommand;

            var addAliasViaPath = new Command("+a", "Add an alias");
            var aliasPathArg = new Argument<string>();
            var aliasesArg = new Argument<string[]>();
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


            var addAliasViaAlias = new Command("+aa", "Add an alias via a path");
            var initialAliasName = new Argument<string>();
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
    }
}