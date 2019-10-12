using System.Text.RegularExpressions;

namespace cb_downloader_v2.Utils
{
    public static class UrlHelper
    {
        private static readonly Regex ChaturbateLinkRegex = new Regex(@"^(https?:\/\/)?chaturbate\.com\/[\da-zA-Z_]+\/?$");
        private static readonly Regex UniversalChaturbateLinkRegex = new Regex(@"^(https?:\/\/)?(ar|de|en|el|es|fr|hi|it|ja|ko|nl|pt|ru|tr|zh)\.chaturbate\.com\/[\da-zA-Z_]+\/?$");

        public static bool IsChaturbateUrl(string url)
        {
            return ChaturbateLinkRegex.Match(url).Success || UniversalChaturbateLinkRegex.Match(url).Success;
        }

        public static string GetModelName(string chaturbateUrl)
        {
            // find last slash after removing the terminal one, if present
            string modelName = chaturbateUrl.TrimEnd('/');
            int lastSlashIdx = modelName.LastIndexOf('/');

            if (lastSlashIdx == -1)
            {
                // this should NEVER occur due the applied regex
                return null;
            }

            // Return the model name without the slash
            modelName = modelName.Substring(lastSlashIdx + 1);
            return modelName;
        }
    }
}
