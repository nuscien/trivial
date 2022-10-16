using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Trivial.CommandLine;

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
        var str = "do something --path /usr/local -path2 \"/usr\" -d /usr -f C:\\Program Files\\Windows NT -url https://dot.net -a Abc 000 -a Xyz";
        var args = new CommandArguments(str);
        Assert.IsFalse(args.IsUrlLike);
        Assert.AreEqual("do something", args.Verb.ToString());
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
        Assert.AreEqual("Abc 000", param.FirstValue);
        Assert.AreEqual("Abc 000 Xyz", param.MergedValue);
        Assert.AreEqual("-a Abc 000 -a Xyz", param.ToString());
        Assert.AreEqual("Abc 000", args.GetFirstOrNext("a").Value);
        Assert.AreEqual("Abc", args.GetFirst("a").Values[0]);

        str = "do/something?path=%2Fusr%2Flocal&path2=%2Fusr&d=&usr=&f=C%3A%5CProgram%20Files%5CWindows%20NT&url=https%3A%2F%2Fdot.net&a=Abc,000&a=Xyz";
        args = new CommandArguments(str);
        Assert.IsTrue(args.IsUrlLike);
        Assert.AreEqual("do/something", args.Verb.ToString());
        param = args.Get("path");
        Assert.AreEqual("/usr/local", param.FirstValue);
        Assert.AreEqual("/usr/local", args.GetFirst("path", "path2").Value);
        Assert.AreEqual("/usr", args.GetFirst("path2").Value);
        Assert.IsTrue(args.GetFirst("d").IsEmpty);
        Assert.AreEqual("&usr=", args.GetFirstOrNext("d", true).Value);
        param = args.Get("f");
        Assert.AreEqual("C:\\Program Files\\Windows NT", param.MergedValue);
        Assert.AreEqual("https://dot.net", args.GetFirst("url").Value);
        param = args.Get("a");
        Assert.AreEqual("Abc,000", param.FirstValue);
        Assert.AreEqual("Abc,000,Xyz", param.MergedValue);
        Assert.AreEqual("a=Abc,000&a=Xyz", param.ToString());
        Assert.AreEqual("Abc,000", args.GetFirstOrNext("a").Value);
        Assert.AreEqual("Abc", args.GetFirst("a").Values[0]);

        args = new CommandArguments(string.Empty);
        Assert.AreEqual(0, args.Count);
        args = new CommandArguments(null as string);
        Assert.AreEqual(0, args.Count);
        args = new CommandArguments(new List<string>());
        Assert.AreEqual(0, args.Count);
    }
}
