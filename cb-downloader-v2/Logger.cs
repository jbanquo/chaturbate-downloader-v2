using System;
using System.Diagnostics;
using System.IO;

namespace cb_downloader_v2
{
    internal class Logger
    {

        public static string LogFileName { get; set; } = "logs.txt";
        public static bool LogToFile { get; set; } = false;

        public static void Log(string line)
        {
            string log = DateTime.Now.ToString("s") + " " + line;

            // Print in output if in debug mode
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

        public static void Log(string modelName, string line)
        {
            Log(modelName + " " + line);
        }
    }
}
