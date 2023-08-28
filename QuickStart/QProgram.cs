using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace QuickStart
{
    public class QProgram
    {
        public string path = "";
        public string additionalArgs = "";
        public List<string> aliases = new List<string>();
        public List<string> keywords = new List<string>();

        public bool Launch()
        {
            try
            {
                Process.Start(path, additionalArgs);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}