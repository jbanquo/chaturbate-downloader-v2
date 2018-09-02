using System;
using System.IO;

namespace cb_downloader_v2.Utils
{
    class FileHelper
    {
        public static bool IsFileAccessible(string fileName)
        {
            return ExistsOnPath(fileName) || File.Exists(fileName);
        }

        public static bool ExistsOnPath(string fileName)
        {
            // Source: https://stackoverflow.com/a/3856090
            return GetFullPath(fileName) != null;
        }

        public static string GetFullPath(string fileName)
        {
            // Source: https://stackoverflow.com/a/3856090
            if (File.Exists(fileName))
                return Path.GetFullPath(fileName);

            var values = Environment.GetEnvironmentVariable("PATH");

            if (values == null)
                return null;

            foreach (var path in values.Split(';'))
            {
                var fullPath = Path.Combine(path, fileName);
                if (File.Exists(fullPath))
                    return fullPath;
            }
            return null;
        }
    }
}
