using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trivial.IO;

namespace Trivial.Geography
{
    /// <summary>
    /// Stream unit test.
    /// </summary>
    [TestClass]
    public class GeolocationUnitTest
    {
        /// <summary>
        /// Test stream utilities.
        /// </summary>
        [TestMethod]
        public void TestLongitude()
        {
            System.Console.WriteLine(57.4 - 57);
            System.Console.WriteLine(0.4 - (57.4 - 57));
            Assert.IsTrue(Math.Abs(0.4 - (57.4 - 57)) <= double.Epsilon);
            var longitude = new Longitude.Model(56.7 + 360);
            Assert.AreEqual(56.7, longitude.Degrees);
            Assert.AreEqual(Longitudes.East, longitude.Type);
            longitude.Degree = -100;
            longitude.Arcminute = 30;
            Assert.AreEqual(-100.5, longitude.Degrees);
            Assert.AreEqual(Longitudes.West, longitude.Type);

            longitude = new Longitude.Model(-56.7 - 360);
            Assert.AreEqual(-56.7, longitude.Degrees);
            Assert.AreEqual(Longitudes.West, longitude.Type);
            longitude.Degree = 720;
            Assert.AreEqual(0.7, longitude.Degrees);
            Assert.AreEqual(Longitudes.East, longitude.Type);
            longitude.Arcminute = 0;
            Assert.AreEqual(0, longitude.Degrees);
            Assert.AreEqual(Longitudes.PrimeMeridian, longitude.Type);
            longitude.Type = Longitudes.CalendarLine;
            Assert.AreEqual(180, longitude.Degrees);
            longitude.Arcminute = 30;
            Assert.AreEqual(179.5, longitude.Degrees);
            Assert.AreEqual(Longitudes.West, longitude.Type);

            var longitudeStruct = new Longitude(-56.7 - 360);
            Assert.AreEqual(-56.7, longitudeStruct.Degrees);
            longitudeStruct = new Longitude(-56.7 - 360);
            Assert.AreEqual(-56.7, longitudeStruct.Degrees);
        }
    }
}
