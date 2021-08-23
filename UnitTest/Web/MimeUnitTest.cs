using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Trivial.Web
{
    /// <summary>
    /// MIME unit test.
    /// </summary>
    [TestClass]
    public class MimeUnitTest
    {
        private const string TestMimeValue = "application/x-test";

        /// <summary>
        /// Tests Pinyin.
        /// </summary>
        [TestMethod]
        public void TestMime()
        {
            MimeConstants.FileExtensionMapping[".pdb"] = "application/vnd.palm";
            MimeConstants.FileExtensionMapping[".ts"] = "application/x-typescript";
            MimeConstants.FileExtensionMapping.BackupGetter = GetMime;
            Assert.AreEqual(TestMimeValue, MimeConstants.FromFileExtension(".test"));
            Assert.AreEqual(3, MimeConstants.FileExtensionMapping.Count);
            Assert.AreEqual("application/vnd.palm", MimeConstants.FromFileExtension(".pdb"));
            Assert.AreEqual("application/x-typescript", MimeConstants.FromFileExtension(".ts"));
            Assert.AreEqual(MimeConstants.Videos.Mp4, MimeConstants.FromFileExtension(".mp4"));
            Assert.AreEqual(MimeConstants.Documents.Docx, MimeConstants.FromFileExtension(".docx"));
            Assert.AreEqual(MimeConstants.StreamMIME, MimeConstants.FromFileExtension(".abcdefg"));
            Assert.IsNull(MimeConstants.FromFileExtension(".abcdefg", true));
        }

        private static bool GetMime(string key, out string value)
        {
            if (key == ".test")
            {
                value = TestMimeValue;
                return true;
            }

            value = default;
            return false;
        }
    }
}
