using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trivial.IO;
using Trivial.Maths;

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
            Assert.AreEqual(56.7 - 180, longitude.Degrees, delta);
            Assert.AreEqual(Longitudes.West, longitude.Type);

            longitude = new Longitude.Model(-56.7 - 180);
            Assert.AreEqual(180 - 56.7, longitude.Degrees, delta);
            Assert.AreEqual(Longitudes.East, longitude.Type);
        }

        /// <summary>
        /// Tests longitude model.
        /// </summary>
        [TestMethod]
        public void TestCelestialLongitude()
        {
            var longitude = new Longitude.Model(true, 56.722 + 360);
            Assert.AreEqual(56.722, longitude.Degrees, delta);
            Assert.AreEqual(Longitudes.East, longitude.Type);
            longitude.Arcsecond = 0;
            longitude.Degree = -100;
            longitude.Arcminute = 30;
            Assert.AreEqual(260.5, longitude.Degrees, delta);
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

            longitude = new Longitude.Model(true, -56.7 - 360);
            Assert.AreEqual(360 - 56.7, longitude.Degrees, delta);
            Assert.AreEqual(Longitudes.West, longitude.Type);
            longitude.Degree = 720;
            Assert.AreEqual(0.3, longitude.Degrees, delta);
            Assert.AreEqual(Longitudes.East, longitude.Type);
            longitude.Arcminute = 0;
            Assert.AreEqual(0, longitude.Degrees, delta);
            Assert.AreEqual(Longitudes.PrimeMeridian, longitude.Type);
            longitude.Type = Longitudes.CalendarLine;
            Assert.AreEqual(180, longitude.Degrees, delta);
            longitude.Arcminute = 30;
            Assert.AreEqual(360 - 179.5, longitude.Degrees, delta);
            Assert.AreEqual(Longitudes.West, longitude.Type);

            longitude = new Longitude.Model(true, 56.722);
            Assert.AreEqual(56.722, longitude.Degrees, delta);
            Assert.AreEqual(Longitudes.East, longitude.Type);

            longitude = new Longitude.Model(true, 56.7 + 180);
            Assert.AreEqual(56.7 + 180, longitude.Degrees, delta);
            Assert.AreEqual(Longitudes.West, longitude.Type);

            longitude = new Longitude.Model(true, -56.7 - 180);
            Assert.AreEqual(180 - 56.7, longitude.Degrees, delta);
            Assert.AreEqual(Longitudes.East, longitude.Type);
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

            longitudeStruct = new Longitude(true, 56.7 + 360);
            Assert.AreEqual(56.7, longitudeStruct.Degrees, delta);
            Assert.AreEqual(Longitudes.East, longitudeStruct.Type);
            longitudeStruct = new Longitude(true, -56.7 - 360);
            Assert.AreEqual(360 - 56.7, longitudeStruct.Degrees, delta);
            Assert.AreEqual(Longitudes.West, longitudeStruct.Type);

            longitudeStruct = new Longitude(true, 0);
            Assert.AreEqual(0, longitudeStruct.Degrees, delta);
            Assert.AreEqual(Longitudes.PrimeMeridian, longitudeStruct.Type);
            longitudeStruct = new Longitude(true, -180);
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

        [TestMethod]
        public void TestLatitudeConstructorsConversionsAndComparison()
        {
            var north = new Latitude(Latitudes.North, 12, 30, 15);
            var south = new Latitude(Latitudes.South, 12, 30, 15);
            var equator = new Latitude(Latitudes.Equator, 12, 30, 15);
            Assert.AreEqual(12.504166, north.Degrees, delta);
            Assert.AreEqual(-12.504166, south.Degrees, delta);
            Assert.AreEqual(0, equator.Degrees, delta);
            Assert.AreEqual(Latitudes.Equator, equator.Type);
            Assert.AreEqual("0°", equator.ToString());
            Assert.IsTrue(north.ToString().EndsWith("N"));
            Assert.IsTrue(south.ToString().EndsWith("S"));

            var fromAngle = new Latitude(new Angle(-12, 30, 15));
            Assert.AreEqual(south, fromAngle);
            Assert.AreEqual(-12, south.AbsDegree);
            Assert.AreEqual(30, south.Arcminute);
            Assert.AreEqual(15, south.Arcsecond, delta);
            Assert.IsTrue(south.Equals(new Latitude.Model(-12.5041666667)));
            Assert.IsFalse(south.Equals(null));
            Assert.IsFalse(south.Equals("latitude"));
            Assert.AreEqual(south.GetHashCode(), new Latitude(-12.5041666667).GetHashCode());

            Latitude.Model model = south;
            Assert.AreEqual(south.Degrees, model.Degrees, delta);
            Assert.IsTrue(model == south);
            Assert.IsFalse(model != south);
            var clone = model.Clone();
            Assert.IsNotNull(clone);
            Assert.AreNotSame(model, clone);
            model.Type = Latitudes.North;
            Assert.AreEqual(Latitudes.North, model.Type);
            model.Type = Latitudes.South;
            Assert.AreEqual(Latitudes.South, model.Type);
            model.Type = Latitudes.Equator;
            Assert.AreEqual(0, model.Degrees, delta);

            Latitude fromNullModel = (Latitude)(Latitude.Model)null;
            Assert.AreEqual(0, fromNullModel.Degrees, delta);
            Assert.IsTrue(north != south);
        }

        [TestMethod]
        public void TestLongitudeConstructorsConversionsAndComparison()
        {
            var east = new Longitude(Longitudes.East, 12.5);
            var west = new Longitude(Longitudes.West, 12.5);
            var prime = new Longitude(Longitudes.PrimeMeridian, 12.5);
            var calendar = new Longitude(Longitudes.CalendarLine, 12.5);
            Assert.AreEqual(12.5, east.Degrees, delta);
            Assert.AreEqual(-12.5, west.Degrees, delta);
            Assert.AreEqual(0, prime.Degrees, delta);
            Assert.AreEqual(180, calendar.Degrees, delta);
            Assert.AreEqual("0°", prime.ToString());
            Assert.AreEqual("180°", calendar.ToString());
            Assert.IsTrue(east.ToString().EndsWith("E"));
            Assert.IsTrue(west.ToString().EndsWith("W"));

            var celestial = new Longitude(true, new Angle(270));
            Assert.IsTrue(celestial.IsCelestial);
            Assert.AreEqual(Longitudes.West, celestial.Type);
            var celestialModel = new Longitude.Model(true, 270);
            celestialModel.IsCelestial = false;
            Assert.AreEqual(270, celestialModel.Degrees, delta);
            Assert.AreEqual(Longitudes.East, celestialModel.Type);
            celestialModel.IsCelestial = true;
            Assert.AreEqual(270, celestialModel.Degrees, delta);
            celestialModel.Type = Longitudes.East;
            Assert.AreEqual(90, celestialModel.Degrees, delta);
            celestialModel.Type = Longitudes.West;
            Assert.AreEqual(270, celestialModel.Degrees, delta);

            var model = new Longitude.Model(-12.5);
            Longitude converted = model;
            Assert.AreEqual(west, converted);
            Assert.IsTrue(model == west);
            Assert.IsFalse(model != west);
            var clone = model.Clone();
            Assert.IsNotNull(clone);
            Assert.AreNotSame(model, clone);
            model.IsCelestial = true;
            Assert.AreEqual(-12.5, model.Degrees, delta);
            Assert.AreEqual(Longitudes.East, model.Type);
            model.Type = Longitudes.PrimeMeridian;
            Assert.AreEqual(Longitudes.PrimeMeridian, model.Type);

            Longitude fromNullModel = (Longitude)(Longitude.Model)null;
            Assert.AreEqual(0, fromNullModel.Degrees, delta);
            Assert.IsFalse(west.Equals(null));
            Assert.IsFalse(west.Equals("longitude"));
            Assert.AreEqual(west.GetHashCode(), new Longitude(-12.5).GetHashCode());
        }

        [TestMethod]
        public void TestGeolocationConstructorsAltitudeAndComparison()
        {
            var latitude = new Latitude(10.5);
            var longitude = new Longitude(-20.5);
            var location = new Geolocation(latitude, longitude, 100, "summit", 5, 10);
            Assert.AreEqual(latitude, location.Latitude);
            Assert.AreEqual(longitude, location.Longitude);
            Assert.AreEqual(100, location.Altitude);
            Assert.AreEqual(5, location.RadiusDeviation);
            Assert.AreEqual(10, location.AltitudeDeviation);
            Assert.AreEqual("summit", location.Description);
            Assert.IsTrue(location.IsAltitude(90));
            Assert.IsTrue(location.IsAltitude(110));
            Assert.IsFalse(location.IsAltitude(89.9));
            Assert.IsTrue(location.ToString().Contains("Altitude = 100m"));

            var noAltitude = new Geolocation(latitude, longitude, "sea", 2);
            Assert.IsTrue(noAltitude.IsAltitude(double.MinValue));
            Assert.IsTrue(noAltitude.ToString().Contains("Longtitude"));
            var exactAltitude = new Geolocation(latitude, longitude, 10, "exact", 1, 0);
            Assert.IsTrue(exactAltitude.IsAltitude(10));
            Assert.IsFalse(exactAltitude.IsAltitude(10.1));
            var negativeDeviation = new Geolocation(latitude, longitude, 100, null, null, -5);
            Assert.IsTrue(negativeDeviation.IsAltitude(95));
            Assert.IsTrue(negativeDeviation.IsAltitude(105));
            Assert.IsFalse(negativeDeviation.IsAltitude(106));
            Assert.IsFalse(new Geolocation(latitude, longitude, double.NaN).Altitude.HasValue);

            var sameLocation = new Geolocation(latitude, longitude, 100, "other", 5, 10);
            Assert.AreEqual(location, sameLocation);
            Assert.AreEqual(location.GetHashCode(), sameLocation.GetHashCode());
            Assert.IsFalse(location.Equals((object)null));
            Assert.IsFalse(location.Equals("location"));
            Assert.IsTrue((latitude & longitude).Equals(new Geolocation(latitude, longitude)));
            Assert.IsTrue((longitude & latitude).Equals(new Geolocation(latitude, longitude)));
        }

        [TestMethod]
        public void TestGeolocationModelAndJsonSerialization()
        {
            var model = new Geolocation.Model(new Latitude.Model(10.5), new Longitude.Model(-20.5), 100, "summit")
            {
                RadiusDeviation = 5,
                AltitudeDeviation = 10
            };
            Assert.IsTrue(model.IsAltitude(90));
            Assert.IsFalse(model.IsAltitude(89));
            Assert.IsTrue(model.Equals(new Geolocation(new Latitude(10.5), new Longitude(-20.5), 100, "other", 5, 10)));
            var coordinateOnlyModel = model.Latitude & model.Longitude;
            Assert.AreEqual(model.Latitude.Degrees, coordinateOnlyModel.Latitude.Degrees, delta);
            Assert.AreEqual(model.Longitude.Degrees, coordinateOnlyModel.Longitude.Degrees, delta);
            Assert.IsFalse(coordinateOnlyModel.Altitude.HasValue);

            var value = new Geolocation(new Latitude(10.5), new Longitude(-20.5), 100, "summit", 5, 10);
            var json = JsonSerializer.Serialize(value);
            StringAssert.Contains(json, "\"latitude\":10.5");
            StringAssert.Contains(json, "\"longitude\":-20.5");
            Assert.AreEqual(value, JsonSerializer.Deserialize<Geolocation>(json));
            Assert.AreEqual(model, JsonSerializer.Deserialize<Geolocation.Model>(JsonSerializer.Serialize(model)));
            Assert.ThrowsExactly<JsonException>(() => JsonSerializer.Deserialize<Geolocation>("[]"));
            Assert.ThrowsExactly<JsonException>(() => JsonSerializer.Deserialize<Geolocation>("{\"latitude\":1}"));
        }
    }
}
