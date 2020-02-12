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
            var delta = 0.0001;
            var longitude = new Longitude.Model(56.7 + 360);
            Assert.AreEqual(56.7, longitude.Degrees, delta);
            Assert.AreEqual(Longitudes.East, longitude.Type);
            longitude.Degree = -100;
            longitude.Arcminute = 30;
            Assert.AreEqual(-100.5, longitude.Degrees, delta);
            Assert.AreEqual(Longitudes.West, longitude.Type);
            longitude.Arcsecond = 6.98f;
            Assert.AreEqual(6.98, longitude.Arcsecond, delta);
            Assert.AreEqual(30, longitude.Arcminute);
            longitude.Arcminute = 48;
            Assert.AreEqual(6.98, longitude.Arcsecond, delta);
            Assert.AreEqual(48, longitude.Arcminute);
            longitude.Arcsecond = 100;
            Assert.AreEqual(40, longitude.Arcsecond, delta);
            Assert.AreEqual(49, longitude.Arcminute);
            longitude.Arcsecond = -100;
            Assert.AreEqual(20, longitude.Arcsecond, delta);
            Assert.AreEqual(47, longitude.Arcminute);

            longitude = new Longitude.Model(-56.7 - 360);
            Assert.AreEqual(-56.7, longitude.Degrees, delta);
            Assert.AreEqual(Longitudes.West, longitude.Type);
            longitude.Degree = 720;
            Assert.AreEqual(0.7, longitude.Degrees, delta);
            Assert.AreEqual(Longitudes.East, longitude.Type);
            longitude.Arcminute = 0;
            Assert.AreEqual(0, longitude.Degrees, delta);
            Assert.AreEqual(Longitudes.PrimeMeridian, longitude.Type);
            longitude.Type = Longitudes.CalendarLine;
            Assert.AreEqual(180, longitude.Degrees, delta);
            longitude.Arcminute = 30;
            Assert.AreEqual(-179.5, longitude.Degrees, delta);
            Assert.AreEqual(Longitudes.West, longitude.Type);

            var longitudeStruct = new Longitude(-56.7 - 360);
            Assert.AreEqual(-56.7, longitudeStruct.Degrees, delta);
            longitudeStruct = new Longitude(-56.7 - 360);
            Assert.AreEqual(-56.7, longitudeStruct.Degrees, delta);
        }
    }
}
