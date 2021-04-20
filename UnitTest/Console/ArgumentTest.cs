using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Trivial.Console
{
    /// <summary>
    /// Unit test for argument.
    /// </summary>
    [TestClass]
    public class ArgumentTest
    {
        /// <summary>
        /// Tests argument parsing.
        /// </summary>
        [TestMethod]
        public void TestArguments()
        {
            var str = "something --path /usr/local -path2 \"/usr\" -d /usr -f C:\\Program Files\\Windows NT -url https://dot.net -a Abc -a Xyz";
            var args = new Arguments(str);
            Assert.AreEqual("something", args.Verb.Key);
            var param = args.Get("path");
            Assert.AreEqual("/usr/local", param.FirstValue);
            Assert.AreEqual("/usr/local", args.GetFirst("path", "path2").Value);
            Assert.AreEqual("/usr", args.GetFirst("path2").Value);
            Assert.IsTrue(args.GetFirst("d").IsEmpty);
            Assert.AreEqual("/usr", args.GetFirstOrNext("d", true).Value);
            param = args.Get("f");
            Assert.AreEqual("C:\\Program Files\\Windows NT", param.MergedValue);
            Assert.AreEqual("https://dot.net", args.GetFirst("url").Value);
            param = args.Get("a");
            Assert.AreEqual("Abc Xyz", param.MergedValue);
            Assert.AreEqual("Abc", args.GetFirstOrNext("a").Value);

            args = new Arguments(string.Empty);
            Assert.AreEqual(0, args.Count);
            args = new Arguments(null as string);
            Assert.AreEqual(0, args.Count);
            args = new Arguments(new List<string>());
            Assert.AreEqual(0, args.Count);
        }
    }
}
