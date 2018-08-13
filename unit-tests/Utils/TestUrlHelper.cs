using cb_downloader_v2.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace unit_tests.Utils
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

        [TestMethod]
        public void TestGetModelName()
        {
            Assert.AreEqual("model", UrlHelper.GetModelName("chaturbate.com/model"));
            Assert.AreEqual("model", UrlHelper.GetModelName("chaturbate.com/model/"));

            Assert.AreEqual("model", UrlHelper.GetModelName("https://chaturbate.com/model/"));
            Assert.AreEqual("model", UrlHelper.GetModelName("https://chaturbate.com/model/"));
        }

        [TestMethod]
        public void TestGetModelName_UniversalURLs()
        {
            Assert.AreEqual("model", UrlHelper.GetModelName("de.chaturbate.com/model"));
            Assert.AreEqual("model", UrlHelper.GetModelName("de.chaturbate.com/model/"));

            Assert.AreEqual("model", UrlHelper.GetModelName("https://de.chaturbate.com/model/"));
            Assert.AreEqual("model", UrlHelper.GetModelName("https://de.chaturbate.com/model/"));
        }
    }
}
