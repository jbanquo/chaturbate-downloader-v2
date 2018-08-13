using cb_downloader_v2;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace unit_tests
{
    [TestClass]
    public class TestUrlHelper
    {
        [TestMethod]
        public void TestIsChaturbateUrl()
        {
            Assert.IsTrue(UrlHelper.IsChaturbateUrl("chaturbate.com/model"));
            Assert.IsTrue(UrlHelper.IsChaturbateUrl("chaturbate.com/model"));

            Assert.IsTrue(UrlHelper.IsChaturbateUrl("chaturbate.com/model/"));
            Assert.IsTrue(UrlHelper.IsChaturbateUrl("chaturbate.com/model/"));

            Assert.IsTrue(UrlHelper.IsChaturbateUrl("http://chaturbate.com/model"));
            Assert.IsTrue(UrlHelper.IsChaturbateUrl("https://chaturbate.com/model"));

            Assert.IsTrue(UrlHelper.IsChaturbateUrl("http://chaturbate.com/model/"));
            Assert.IsTrue(UrlHelper.IsChaturbateUrl("https://chaturbate.com/model/"));
        }

        [TestMethod]
        public void TestIsChaturbateUrl_UniversalURLs()
        {
            Assert.IsTrue(UrlHelper.IsChaturbateUrl("de.chaturbate.com/model"));
            Assert.IsTrue(UrlHelper.IsChaturbateUrl("de.chaturbate.com/model"));
            Assert.IsFalse(UrlHelper.IsChaturbateUrl("chaturbate.de/model"));

            Assert.IsTrue(UrlHelper.IsChaturbateUrl("de.chaturbate.com/model/"));
            Assert.IsTrue(UrlHelper.IsChaturbateUrl("de.chaturbate.com/model/"));
            Assert.IsFalse(UrlHelper.IsChaturbateUrl("chaturbate.de/model/"));

            Assert.IsTrue(UrlHelper.IsChaturbateUrl("http://de.chaturbate.com/model"));
            Assert.IsTrue(UrlHelper.IsChaturbateUrl("https://de.chaturbate.com/model"));
            Assert.IsFalse(UrlHelper.IsChaturbateUrl("https://chaturbate.de/model"));

            Assert.IsTrue(UrlHelper.IsChaturbateUrl("http://de.chaturbate.com/model/"));
            Assert.IsTrue(UrlHelper.IsChaturbateUrl("https://de.chaturbate.com/model/"));
            Assert.IsFalse(UrlHelper.IsChaturbateUrl("https://chaturbate.de/model/"));
        }
    }
}
