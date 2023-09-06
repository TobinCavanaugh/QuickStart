using System;
using System.IO;
using Newtonsoft.Json;

namespace QSn
{
    public class JsonHandler
    {
        public static string configPath = "";

        static void InitConfigPath()
        {
            if (configPath == "")
            {
                //Get the config path
                configPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\quickstart.json";
            }
        } 

        public static QuickstartSave GetQuickstartSave()
        {
            InitConfigPath();

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

            return qss;
        }

        public static void UpdateQuickstartSave(QuickstartSave qss)
        {
            InitConfigPath();
            //if (qss.invalidated)
            //{
                File.WriteAllText(configPath, JsonConvert.SerializeObject(qss));
            //}
        }
    }
}