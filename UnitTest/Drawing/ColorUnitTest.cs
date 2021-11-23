using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trivial.Collection;
using Trivial.Text;

namespace Trivial.Drawing
{
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
            Assert.AreEqual(168, color.B);

            Assert.AreEqual(102, ColorCalculator.Alpha(color, 0.5).A);
            Assert.AreEqual(128, ColorCalculator.Alpha(color, 0.5, true).A);
            Assert.IsTrue(ColorCalculator.Darken(color, 0.1).R < 226);

            color = ColorCalculator.Parse("red");
            Assert.AreEqual(255, color.R);
            Assert.AreEqual(0, color.G);
            Assert.AreEqual(0, color.B);
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
            Assert.AreEqual(Color.FromArgb(0, 255, 255), ColorCalculator.Reverse(color));
            Assert.AreEqual(color, ColorCalculator.ToggleBrightness(color));
            color = Color.FromArgb(240, 10, 10);
            Assert.AreEqual(Color.FromArgb(245, 15, 15), ColorCalculator.ToggleBrightness(color));
        }
    }
}
