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
            MimeConstants.FileExtensionMapping.Add(".ts", "application/typescript");
            MimeConstants.FileExtensionMapping.Set(".ts", "application/x-typescript", true);
            MimeConstants.RegisterFileExtensionMapping(new Text.JsonObjectNode
            {
                { "mime", new Text.JsonObjectNode
                {
                    { ".pdb", "application/x-program-database" },
                    { ".a", "application/x-test-a" }
                } }
            });
            MimeConstants.RegisterFileExtensionMapping(new Text.JsonArrayNode());
            MimeConstants.FileExtensionMapping.Remove(new string[] { null, ".a", ".b" });
            MimeConstants.FileExtensionMapping.Remove(".mp4");
            MimeConstants.FileExtensionMapping.BackupGetter = GetMime;
            Assert.AreEqual(TestMimeValue, MimeConstants.ByFileExtension(".test"));
            Assert.AreEqual(3, MimeConstants.FileExtensionMapping.Count);
            Assert.AreEqual("application/vnd.palm", MimeConstants.ByFileExtension(".pdb"));
            Assert.AreEqual("application/x-typescript", MimeConstants.ByFileExtension(".ts"));
            Assert.AreEqual(MimeConstants.Videos.Mp4, MimeConstants.ByFileExtension(".mp4"));
            Assert.AreEqual(MimeConstants.Documents.Docx, MimeConstants.ByFileExtension(".docx"));
            Assert.AreEqual(MimeConstants.StreamMIME, MimeConstants.ByFileExtension(".abcdefg"));
            Assert.IsNull(MimeConstants.ByFileExtension(".abcdefg", true));
            MimeConstants.FileExtensionMapping.Clear();
            Assert.AreEqual(0, MimeConstants.FileExtensionMapping.Values.Count);
            Assert.AreEqual(MimeConstants.Packages.DownloadToRun, MimeConstants.ByFileExtension(".pdb"));

#if NETFRAMEWORK
            if (!MimeConstants.Win32Registry.RegisterFileExtensionMapping(".wma")) return;
            MimeConstants.Win32Registry.RegisterFileExtensionMapping(MimeConstants.Images.Jpeg);
            Assert.AreEqual(MimeConstants.Audio.Wma, MimeConstants.FileExtensionMapping[".wma"]);
            Assert.IsFalse(MimeConstants.Win32Registry.RegisterFileExtensionMapping(".abcdefg"));
            Assert.IsFalse(MimeConstants.Win32Registry.RegisterFileExtensionMapping("application/x-test-c"));
            var association = MimeConstants.Win32Registry.GetFileAssociationInfo(".cs");
            Assert.AreEqual(".cs", association.FileExtension);
            association = MimeConstants.Win32Registry.GetFileAssociationInfo(".wma");
            Assert.AreEqual(MimeConstants.Audio.Wma, association.ContentType);
            association = MimeConstants.Win32Registry.GetFileAssociationInfo(MimeConstants.Audio.Wma);
            Assert.AreEqual(MimeConstants.Audio.Wma, association.ContentType);
#endif
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
