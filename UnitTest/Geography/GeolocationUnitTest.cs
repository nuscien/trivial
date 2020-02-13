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
        private readonly double delta = 0.0001;

        /// <summary>
        /// Tests longitude model.
        /// </summary>
        [TestMethod]
        public void TestLongitude()
        {
            var longitude = new Longitude.Model(56.722 + 360);
            Assert.AreEqual(56.722, longitude.Degrees, delta);
            Assert.AreEqual(Longitudes.East, longitude.Type);
            longitude.Arcsecond = 0;
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

            longitude = new Longitude.Model(56.722);
            Assert.AreEqual(56.722, longitude.Degrees, delta);
            Assert.AreEqual(Longitudes.East, longitude.Type);

            longitude = new Longitude.Model(56.7 + 180);
            Assert.AreEqual(180 - 56.722, longitude.Degrees, delta);
            Assert.AreEqual(Longitudes.West, longitude.Type);
        }

        /// <summary>
        /// Tests longitude struct.
        /// </summary>
        [TestMethod]
        public void TestLongitudeStruct()
        {
            var longitudeStruct = new Longitude(56.7 + 360);
            Assert.AreEqual(56.7, longitudeStruct.Degrees, delta);
            Assert.AreEqual(Longitudes.East, longitudeStruct.Type);
            longitudeStruct = new Longitude(-56.7 - 360);
            Assert.AreEqual(-56.7, longitudeStruct.Degrees, delta);
            Assert.AreEqual(Longitudes.West, longitudeStruct.Type);
            longitudeStruct = new Longitude(0);
            Assert.AreEqual(0, longitudeStruct.Degrees, delta);
            Assert.AreEqual(Longitudes.PrimeMeridian, longitudeStruct.Type);
            longitudeStruct = new Longitude(-180);
            Assert.AreEqual(180, longitudeStruct.Degrees, delta);
            Assert.AreEqual(Longitudes.CalendarLine, longitudeStruct.Type);
        }

        /// <summary>
        /// Tests longitude model.
        /// </summary>
        [TestMethod]
        public void TestLatitude()
        {
            var latitude = new Latitude.Model(56.722 + 90);
            Assert.AreEqual(90 - 56.722, latitude.Degrees, delta);
            Assert.AreEqual(Latitudes.North, latitude.Type);
            latitude.Arcsecond = 0;
            latitude.Arcminute = 18;
            latitude.Degree = -100;
            latitude.Arcminute = 30;
            Assert.AreEqual(-80.5, latitude.Degrees, delta);
            Assert.AreEqual(Latitudes.South, latitude.Type);
            latitude.Arcsecond = 6.98f;
            Assert.AreEqual(6.98, latitude.Arcsecond, delta);
            Assert.AreEqual(30, latitude.Arcminute);
            latitude.Arcminute = 48;
            Assert.AreEqual(6.98, latitude.Arcsecond, delta);
            Assert.AreEqual(48, latitude.Arcminute);
            latitude.Arcsecond = 100;
            Assert.AreEqual(40, latitude.Arcsecond, delta);
            Assert.AreEqual(49, latitude.Arcminute);
            latitude.Arcsecond = -100;
            Assert.AreEqual(20, latitude.Arcsecond, delta);
            Assert.AreEqual(47, latitude.Arcminute);

            latitude = new Latitude.Model(-56.722 - 90);
            Assert.AreEqual(56.722 - 90, latitude.Degrees, delta);
            Assert.AreEqual(Latitudes.South, latitude.Type);
            latitude.Arcsecond = 0;
            latitude.Arcminute = 18;
            latitude.Degree = 180;
            Assert.AreEqual(0.3, latitude.Degrees, delta);
            Assert.AreEqual(Latitudes.South, latitude.Type);
            latitude.Arcminute = 0;
            Assert.AreEqual(0, latitude.Degrees, delta);
            Assert.AreEqual(Latitudes.Equator, latitude.Type);

            latitude = new Latitude.Model(32.1235);
            Assert.AreEqual(32.1235, latitude.Degrees, delta);
            Assert.AreEqual(Latitudes.North, latitude.Type);

            latitude = new Latitude.Model(32.1235 + 180);
            Assert.AreEqual(-32.1235, latitude.Degrees, delta);
            Assert.AreEqual(Latitudes.South, latitude.Type);
        }

        /// <summary>
        /// Tests longitude struct.
        /// </summary>
        [TestMethod]
        public void TestLatitudeStruct()
        {
            var latitudeStruct = new Latitude(56.7 + 90);
            Assert.AreEqual(90 - 56.7, latitudeStruct.Degrees, delta);
            Assert.AreEqual(Latitudes.North, latitudeStruct.Type);
            latitudeStruct = new Latitude(-56.7 - 90);
            Assert.AreEqual(56.7 - 90, latitudeStruct.Degrees, delta);
            Assert.AreEqual(Latitudes.South, latitudeStruct.Type);
        }
    }
}
