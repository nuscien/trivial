using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trivial.Collection;
using Trivial.Text;

namespace Trivial.Drawing;

/// <summary>
/// The unit test of color calculator.
/// </summary>
[TestClass]
public class ColorUnitTest
{
    /// <summary>
    /// Tests color calculator, color parser and color systems converters.
    /// </summary>
    [TestMethod]
    public void TestCalculator()
    {
        var color = ColorCalculator.Parse("hsl(318.413, 76.518%, 0.51568)");
        var hsl = ColorCalculator.ToHSL(color);
        Assert.IsTrue(Math.Abs(hsl.Item1 - 318.412) < 0.1);
        Assert.IsTrue(Math.Abs(hsl.Item2 - 0.77) < 0.1);
        Assert.IsTrue(Math.Abs(hsl.Item3 - 0.52) < 0.1);
        Assert.AreEqual(226, color.R);
        Assert.AreEqual(37, color.G);
        Assert.AreEqual(168, color.B);
        color = ColorCalculator.Parse("hsl(318.413, 76.518%, 0.51568, 0.8)");
        Assert.AreEqual(226, color.R);

        color = ColorCalculator.Parse("cmyk(0, 0.83628, 0.25664, 0.11373)");
        var cmyk = ColorCalculator.ToCMYK(color);
        Assert.AreEqual(0, cmyk.Item1);
        Assert.IsTrue(Math.Abs(cmyk.Item2 - 0.83628) < 0.01);
        Assert.IsTrue(Math.Abs(cmyk.Item3 - 0.25664) < 0.01);
        Assert.IsTrue(Math.Abs(cmyk.Item4 - 0.11373) < 0.01);
        Assert.AreEqual(226, color.R);
        Assert.AreEqual(37, color.G);
        Assert.AreEqual(168, color.B);
        color = ColorCalculator.Parse("cmyk(0, 0.83628, 0.25664, 0.11373, 0.8)");
        Assert.AreEqual(37, color.G);

        color = ColorCalculator.Parse("rgb(226, 37, 0xA8)");
        Assert.AreEqual(226, color.R);
        Assert.AreEqual(37, color.G);
        Assert.AreEqual(168, color.B);
        Assert.IsTrue(Math.Abs(color.GetHue() - 318.412) < 0.1);
        color = ColorCalculator.Parse("rgb(226, 37, 168, 0.8)");
        Assert.AreEqual("rgba(226, 37, 168, 0.8)", ColorCalculator.ToRgbaString(color));
        Assert.AreEqual(168, color.B);
        Assert.AreEqual(102, ColorCalculator.Opacity(color, 0.5).A);
        Assert.AreEqual(128, ColorCalculator.Opacity(color, 0.5, true).A);
        Assert.IsTrue(ColorCalculator.Darken(color, 0.1).R < 226);

        color = ColorCalculator.Parse("red");
        Assert.AreEqual(255, color.R);
        Assert.AreEqual(0, color.G);
        Assert.AreEqual(0, color.B);
        Assert.AreEqual("rgba(255, 0, 0, 1)", ColorCalculator.ToRgbaString(color));
        color = ColorCalculator.Parse("#F00");
        Assert.AreEqual(255, color.R);
        Assert.AreEqual(0, color.G);
        Assert.AreEqual(0, color.B);
        color = ColorCalculator.Parse("#FF0000");
        Assert.AreEqual(255, color.R);
        Assert.AreEqual(0, color.G);
        Assert.AreEqual(0, color.B);
        color = ColorCalculator.Parse("#FFFF0000");
        Assert.AreEqual(255, color.R);
        Assert.AreEqual(0, color.G);
        Assert.AreEqual(0, color.B);

        color = Color.FromArgb(255, 0, 0);
        Assert.AreEqual(Color.FromArgb(0, 255, 255), ColorCalculator.Invert(color));
        Assert.AreEqual(color, ColorCalculator.ToggleBrightness(color));
        color = Color.FromArgb(240, 10, 10);
        Assert.AreEqual(Color.FromArgb(245, 15, 15), ColorCalculator.ToggleBrightness(color));

        color = ColorCalculator.Overlay(
            Color.FromArgb(128, 255, 0, 0),
            Color.FromArgb(255, 0, 0, 255));
        Assert.AreEqual(128, color.R);
        Assert.AreEqual(255, color.A);
        Assert.AreEqual(127, color.B);

        color = ColorCalculator.Mix(
            ColorMixTypes.Normal,
            Color.FromArgb(240, 0, 0),
            Color.FromArgb(0, 240, 0));
        Assert.AreEqual(120, color.R);
        Assert.AreEqual(120, color.G);
        Assert.AreEqual(0, color.B);
        color = ColorCalculator.Mix(
            ColorMixTypes.Lighten,
            Color.FromArgb(255, 192, 0),
            Color.FromArgb(0, 240, 64));
        Assert.AreEqual(255, color.R);
        Assert.AreEqual(240, color.G);
        Assert.AreEqual(64, color.B);
        color = ColorCalculator.Mix(
            ColorMixTypes.Darken,
            Color.FromArgb(255, 192, 0),
            Color.FromArgb(0, 240, 64));
        Assert.AreEqual(0, color.R);
        Assert.AreEqual(192, color.G);
        Assert.AreEqual(0, color.B);
        color = ColorCalculator.Mix(
            ColorMixTypes.Accent,
            Color.FromArgb(255, 192, 0),
            Color.FromArgb(0, 240, 64));
        Assert.AreEqual(255, color.R);
        Assert.AreEqual(255, color.G);
        Assert.AreEqual(64, color.B);

        color = ColorCalculator.LinearGradient(Color.FromArgb(255, 0, 0, 0), Color.FromArgb(255, 100, 100, 100), 1).First();
        Assert.AreEqual(255, color.A);
        Assert.AreEqual(50, color.R);
        Assert.AreEqual(50, color.G);
        Assert.AreEqual(50, color.B);
        color = ColorCalculator.LinearGradient(Color.FromArgb(255, 0, 0, 0), Color.FromArgb(255, 100, 100, 100), 3).Skip(1).First();
        Assert.AreEqual(255, color.A);
        Assert.AreEqual(50, color.R);
        Assert.AreEqual(50, color.G);
        Assert.AreEqual(50, color.B);

        var colors = CreateArray(10);
        colors = ColorCalculator.Opacity(colors, 12).ToArray();
        for (var i = 0; i < colors.Length; i++)
        {
            Assert.IsTrue(colors[i].A <= 12);
        }
    }

    private static Color[] CreateArray(int count)
    {
        var arr = new Color[count];
        var random = new Random();
        for (var i = 0; i < count; i++)
        {
            arr[i] = Color.FromArgb(random.Next(255), random.Next(255), random.Next(255), random.Next(255));
        }

        return arr;
    }
}
