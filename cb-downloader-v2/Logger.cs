using System;
using System.Diagnostics;
using System.IO;

namespace cb_downloader_v2
{
    internal class Logger
    {

        public static string LogFileName { get; set; } = "logs.txt";
        public static bool LogToFile { get; set; } = false;

        public static void Log(string evt, string line)
        {
            string log = DateTime.Now.ToString("s") + " [" + evt + "] " + line;
            
            // Print line if in debug mode
#if DEBUG
            Debug.WriteLine(log);
#endif

            // Write to log file if required
            if (LogToFile)
            {
                try
                {
                    File.AppendAllText(LogFileName, log + Environment.NewLine);
                }
                catch
                {
                    // ignored
                }
            }
        }
    }
}
