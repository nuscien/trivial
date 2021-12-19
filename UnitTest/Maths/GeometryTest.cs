using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Trivial.Maths
{
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
        }
    }
}
