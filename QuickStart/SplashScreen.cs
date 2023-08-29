using System;

namespace QuickStart
{
    public class SplashScreen
    {
        public static string GetSplashScreen()
        {
            string returnVal = @"
     QQQQQQQQQ        SSSSSSSSSSSSSSS 
   QQ:::::::::QQ    SS:::::::::::::::S
 QQ:::::::::::::QQ S:::::SSSSSS::::::S
Q:::::::QQQ:::::::QS:::::S     SSSSSSS
Q::::::O   Q::::::QS:::::S            
Q:::::O     Q:::::QS:::::S            
Q:::::O     Q:::::Q S::::SSSS         
Q:::::O     Q:::::Q  SS::::::SSSSS    
Q:::::O     Q:::::Q    SSS::::::::SS  
Q:::::O     Q:::::Q       SSSSSS::::S 
Q:::::O  QQQQ:::::Q            S:::::S
Q::::::O Q::::::::Q            S:::::S
Q:::::::QQ::::::::QSSSSSSS     S:::::S
 QQ::::::::::::::Q S::::::SSSSSS:::::S
   QQ:::::::::::Q  S:::::::::::::::SS 
     QQQQQQQQ::::QQ SSSSSSSSSSSSSSS   
             Q:::::Q
              QQQQQQ";

            return returnVal;
        }

        public static void PrintSplashScreen(string splashScreen)
        {
            foreach (var line in splashScreen.Split('\n'))
            {
                string l = line.Replace("\n", "").Replace("\t", "");
                CenterPrint(l);
            }
        }
        
        public static void CenterPrint(string inString)
        {
            var result = String.Format("{0," + ((Console.WindowWidth / 2) + (inString.Length / 2)) + "}",
                inString);
            Console.WriteLine(result);
        }
    }
}