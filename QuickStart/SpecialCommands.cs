using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Diagnostics;
using System.Linq;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;

namespace QuickStart
{
    public class SpecialCommands
    {
        public SpecialCommands(Command rootCommand, QuickstartSave qss, string version)
        {
            //Install quickstart to Program Files
            {
                Process.Start("cmd",
                    @"mkdir C:\Program Files\QuickStart\" + "\n" +
                    @"cd C:\Program Files\QuickStart" + "\n" +
                    @"git clone "
                );
            }


            //Splash Screen version ******************************************************************
            {
                var splashCommand = new Command("version", "Version splashscreen");
                rootCommand.Add(splashCommand);

                var splashCommandAlt = new Command("v", "Version splashscreen");
                rootCommand.Add(splashCommandAlt);

                var handleAction = new Action<InvocationContext>(h =>
                {
                    SplashScreen.PrintSplashScreen(SplashScreen.GetSplashScreen());
                    SplashScreen.CenterPrint("-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-");
                    SplashScreen.CenterPrint("[Quickstart]");
                    SplashScreen.CenterPrint($"[Version {version}]");
                    SplashScreen.CenterPrint("[Tobin Cavanaugh]");
                    SplashScreen.CenterPrint(@"[https://github.com/TobinCavanaugh/QuickStart]");
                });

                splashCommand.SetHandler(h => handleAction.Invoke(h));
                splashCommandAlt.SetHandler(h => handleAction.Invoke(h));
            }

            //List windows apps  *****************************************************************
            {
                var browseCommand = new Command("wappl",
                    "(Windows Application List) List installed windows apps in the AppsFolder");
                rootCommand.Add(browseCommand);

                //Maybe check registry later?
                //HKEY_CLASSES_ROOT\Extensions\ContractId\Windows.Protocol\PackageId

                browseCommand.SetHandler(h =>
                {
                    //Thanks to https://stackoverflow.com/a/62935931/21769995 
                    //GUID taken from https://learn.microsoft.com/en-us/windows/win32/shell/knownfolderid
                    var FOLDERID_AppsFolder = new Guid("{1e87508d-89c2-42f0-8a7e-645a0f50ca58}");
                    ShellObject appsFolder =
                        (ShellObject)KnownFolderHelper.FromKnownFolderId(FOLDERID_AppsFolder);
                    List<KeyValuePair<string, string>> appNamePathDictionary =
                        new List<KeyValuePair<string, string>>();

                    foreach (var app in (IKnownFolder)appsFolder)
                    {
                        string name = app.Name;
                        // Accessing the properties of the app
                        var linkPathProperty =
                            app.Properties.GetProperty(SystemProperties.System.Link.TargetParsingPath);

                        string path = linkPathProperty?.ValueAsObject as string;


                        if (path != null && name != null)
                        {
                            appNamePathDictionary.Add(new KeyValuePair<string, string>(path, name));
                        }
                    }

                    appNamePathDictionary = appNamePathDictionary.OrderBy(x => x.Value).ToList();

                    TablePrinter printer = new TablePrinter();
                    int iterator = 0;
                    foreach (var keyValuePair in appNamePathDictionary)
                    {
                        printer.SetCell(iterator, 0, keyValuePair.Value);
                        printer.SetCell(iterator, 1, keyValuePair.Key);
                        iterator++;
//                        Console.WriteLine(keyValuePair.Value + " | " + keyValuePair.Key);
                    }

                    Console.WriteLine(printer.ToString());
                });
            }
        }
    }
}