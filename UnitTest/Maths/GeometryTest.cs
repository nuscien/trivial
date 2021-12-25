using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Trivial.Maths;

/// <summary>
/// Arithmetic unit test.
/// </summary>
[TestClass]
public class GeometryTest
{
    /// <summary>
    /// Tests arithmetic.
    /// </summary>
    [TestMethod]
    public void TestGeometry()
    {
        // Points
        var angle = Geometry.Angle(new DoublePoint2D(0, 0), new DoublePoint2D(1, 0), new DoublePoint2D(0, 1));
        Assert.AreEqual(90, angle.AbsDegree);

        var point = JsonSerializer.Deserialize<IntPoint2D>("{ \"x\": 12.8, \"y\": \"62\" }");
        Assert.AreEqual(13, point.X);
        Assert.AreEqual(62, point.Y);

        var point2 = JsonSerializer.Deserialize<DoublePoint3D>("{ \"x\": 12.8, \"y\": \"62\" }");
        Assert.AreEqual(12.8, point2.X);
        Assert.AreEqual(62, point2.Y);
        Assert.AreEqual(0, point2.Z);
    }
}
