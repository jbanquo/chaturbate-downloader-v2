using System.IO;
using System.Reflection;

namespace cb_downloader_v2.Utils
{
    static class EmbeddedResourceHelper
    {
        public static string ReadText(string fileName)
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(fileName);
            
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
