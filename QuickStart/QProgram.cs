using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace QuickStart
{
    public class QProgram
    {
        private string path = "";

        public string Path
        {
            get { return path; }
            set { path = value.Trim(); }
        }

        public string additionalArgs = "";
        public List<string> aliases = new List<string>();
        public List<string> keywords = new List<string>();
        public bool useAdmin = false;


        /// <summary>
        /// Thanks to: https://stackoverflow.com/a/37519195/21769995 
        /// </summary>
        /// <param name="fileName"></param>
        public void ExecuteAsAdmin(string fileName, string args)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = fileName;
            proc.StartInfo.UseShellExecute = true;
            proc.StartInfo.Arguments = args;
            proc.StartInfo.Verb = "runas";
            proc.Start();
        }

        public bool Launch()
        {
            try
            {
                if (useAdmin)
                {
                    ExecuteAsAdmin(Path, additionalArgs);
                }
                else
                {
                    Process.Start(Path, additionalArgs);
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}