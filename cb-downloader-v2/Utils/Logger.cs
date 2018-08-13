using System;
using System.Diagnostics;
using System.IO;

namespace cb_downloader_v2.Utils
{
    /// <summary>
    ///     A simple text file logger.
    /// </summary>
    internal class Logger
    {
        /// <summary>
        ///     The name of the log file.
        /// </summary>
        public static string LogFileName { get; set; } = "logs.txt";
        /// <summary>
        ///     Whether to log to file or not.
        /// </summary>
        public static bool LogToFile { get; set; } = false;

        /// <summary>
        ///     General logging of a line alongside a time stamp.
        /// </summary>
        /// <param name="line"></param>
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
                catch (IOException ex)
                {
#if DEBUG
                    Debug.WriteLine("Error writing to log file: " + line + ", Exception: " + ex.Message + ", Trace: " + ex.StackTrace);
#endif
                }
            }
        }

        /// <summary>
        ///     Formatted logging associated with a specific model name.
        /// </summary>
        /// <param name="modelName"></param>
        /// <param name="line"></param>
        public static void Log(string modelName, string line)
        {
            Log(modelName + " " + line);
        }
    }
}
