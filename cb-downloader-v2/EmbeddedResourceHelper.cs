using System.IO;
using System.Reflection;

namespace cb_downloader_v2
{
    class EmbeddedResourceHelper
    {
        public static string ReadText(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            string result;

            using (Stream stream = assembly.GetManifestResourceStream(fileName))
            using (StreamReader reader = new StreamReader(stream))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }
    }
}
