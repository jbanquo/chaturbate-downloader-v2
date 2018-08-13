using System.Text.RegularExpressions;

namespace cb_downloader_v2
{
    public class UrlHelper
    {
        private static readonly Regex ChaturbateLinkRegex = new Regex(@"^(https?:\/\/)?chaturbate\.com\/[\da-zA-Z_]+\/?$");
        private static readonly Regex UniversalChaturbateLinkRegex = new Regex(@"^(https?:\/\/)?(ar|de|en|el|es|fr|hi|it|ja|ko|nl|pt|ru|tr|zh)\.chaturbate\.com\/[\da-zA-Z_]+\/?$");

        public static bool IsChaturbateUrl(string url)
        {
            return ChaturbateLinkRegex.Match(url).Success || UniversalChaturbateLinkRegex.Match(url).Success;
        }
    }
}
