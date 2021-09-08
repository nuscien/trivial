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
        /// Tests MIME mapping.
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
            Assert.AreEqual(TestMimeValue, MimeConstants.GetByFileExtension(".test"));
            Assert.AreEqual(3, MimeConstants.FileExtensionMapping.Count);
            Assert.AreEqual("application/vnd.palm", MimeConstants.GetByFileExtension(".pdb"));
            Assert.AreEqual("application/x-typescript", MimeConstants.GetByFileExtension(".ts"));
            Assert.AreEqual(MimeConstants.Videos.Mp4, MimeConstants.GetByFileExtension(".mp4"));
            Assert.AreEqual(MimeConstants.Documents.Docx, MimeConstants.GetByFileExtension(".docx"));
            Assert.AreEqual(MimeConstants.StreamMIME, MimeConstants.GetByFileExtension(".abcdefg"));
            Assert.IsNull(MimeConstants.GetByFileExtension(".abcdefg", true));
            MimeConstants.FileExtensionMapping.Clear();
            Assert.AreEqual(0, MimeConstants.FileExtensionMapping.Values.Count);
            Assert.AreEqual(MimeConstants.Packages.DownloadToRun, MimeConstants.GetByFileExtension(".pdb"));
            Assert.AreEqual(MimeConstants.StreamMIME, MimeConstants.GetByFileExtension(".inf"));

#if NETFRAMEWORK
            if (!MimeConstants.Registry.RegisterFileExtensionMapping(".wma")) return;
            MimeConstants.Registry.RegisterFileExtensionMapping(MimeConstants.Images.Jpeg);
            Assert.AreEqual(MimeConstants.Audio.Wma, MimeConstants.FileExtensionMapping[".wma"]);
            Assert.IsFalse(MimeConstants.Registry.RegisterFileExtensionMapping(".abcdefg"));
            Assert.IsFalse(MimeConstants.Registry.RegisterFileExtensionMapping("application/x-test-c"));
            var association = MimeConstants.Registry.GetFileAssociationInfo(".cs");
            Assert.AreEqual(".cs", association.FileExtension);
            association = MimeConstants.Registry.GetFileAssociationInfo(".wma");
            Assert.AreEqual(MimeConstants.Audio.Wma, association.ContentType);
            association = MimeConstants.Registry.GetFileAssociationInfo(MimeConstants.Audio.Wma);
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
