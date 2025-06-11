﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trivial.Reflection;
using Trivial.Security;

namespace Trivial.Text;

/// <summary>
/// The unit test for string.
/// </summary>
[TestClass]
public class StringUnitTest
{
    /// <summary>
    /// Tests text finder.
    /// </summary>
    [TestMethod]
    public void TestStringFinder()
    {
        const string TestString = "There was once a queen who had no children, and it grieved her sorely. One winter's afternoon she was sitting by the window sewing when she pricked her finger, and three drops of blood fell on the snow. Then she thought to herself:\r\n\"Ah, what would I give to have a daughter with skin as white as snow and cheeks as red as blood.\"";
        var finder = new StringFinder(TestString);
        Assert.AreEqual("There was once a queen who had no children", finder.Before(',', true));
        finder.Before(1);
        Assert.AreEqual("and it grieved her sorely.", finder.Until("."));
        Assert.AreEqual(' ', finder.Read());
        finder.Until("\r\n");
        Assert.IsTrue(finder.Contains("\""));
        Assert.AreEqual(3, finder.IndexOf(','));
        Assert.AreEqual(1, finder.IndexOf("Ah"));
        Assert.IsFalse(finder.BeforeIfContains(new List<string>
        {
            "After a while a little daughter came to her with skin as white as snow and cheeks as red as blood. So they called her Snow White.",
            "But before Snow White had grown up, her mother, the Queen, died and her father married again, a most beautiful princess who was very vain of her beauty and jealous of all women who might be thought as beautiful as she was. But before Snow White had grown up, her mother, the Queen, died and her father married again, a most beautiful princess who was very vain of her beauty and jealous of all women who might be thought as beautiful as she was.",
            "\"Mirror, mirror, on the wall,\r\nWho is the fairest of us all?\""
        }, StringsMatchingRules.Rear));
        Assert.IsTrue(finder.BeforeIfContains(new[]
        {
            "\"",
            "\'",
            "."
        }, StringsMatchingRules.Front, true));
        finder.BeforeIfContains(new List<string>
        {
            "\"",
            "\'",
            "."
        });
        Assert.IsFalse(finder.IsEnd);
        Assert.AreEqual(TestString.Length - 1, finder.Offset);
        Assert.AreEqual("\"", finder.Before(1));
        Assert.AreEqual("\"", finder.Value);
        Assert.IsTrue(finder.IsEnd);

        Assert.AreEqual("good morning", StringExtensions.Between("let's say good morning to you", " ", " to", false, 6));
        Assert.AreEqual("good morning", StringExtensions.Between("let's say good morning", "good", "x", true));
        Assert.AreEqual("good morning", StringExtensions.Between("let's say good morning to you", " ", " to", StringComparison.OrdinalIgnoreCase, false, 6));
        Assert.AreEqual("good morning", StringExtensions.Between("let's say good morning", "good", "x", StringComparison.OrdinalIgnoreCase, true));
    }

    /// <summary>
    /// Tests the description.
    /// </summary>
    [TestMethod]
    public void TestDescription()
    {
        var s = StringExtensions.GetDescription(PropertySettingPolicies.Allow);
        Assert.IsTrue(s.Length > 6);
        var d1 = StringExtensions.GetEnumDescriptionMapping<PropertySettingPolicies>();
        Assert.AreEqual(s, d1[PropertySettingPolicies.Allow]);
        var d2 = StringExtensions.GetEnumDescriptionMapping(typeof(PropertySettingPolicies));
        Assert.AreEqual(s, d2[nameof(PropertySettingPolicies.Allow)]);
        s = StringExtensions.GetDescription((PropertySettingPolicies)19);
        Assert.AreEqual("19", s);
        s = StringExtensions.GetDescription(typeof(JsonWebTokenPayload).GetProperty(nameof(JsonWebTokenPayload.Id)));
        Assert.IsTrue(s.Length > 6);
        var uri = StringExtensions.TryCreateUri("https://www.kingcean.net");
        Assert.IsNotNull(uri);
        uri = StringExtensions.TryCreateUri("https://www.kingcean.net", UriKind.RelativeOrAbsolute);
        Assert.IsNotNull(uri);
        uri = StringExtensions.TryCreateUri("hello", UriKind.Absolute);
        Assert.IsNull(uri);
    }
}
