using System;
using System.Collections.Generic;
using System.Text;

namespace Trivial.Geography
{
    /// <summary>
    /// Latitudes.
    /// </summary>
    public enum Latitudes
    {
        /// <summary>
        /// The equator.
        /// </summary>
        Equator = 0,

        /// <summary>
        /// North latitudes.
        /// </summary>
        North = 1,

        /// <summary>
        /// South latitudes.
        /// </summary>
        South = 2
    }

    /// <summary>
    /// Longitudes.
    /// </summary>
    public enum Longitudes
    {
        /// <summary>
        /// Prime meridian.
        /// </summary>
        PrimeMeridian = 0,

        /// <summary>
        /// East longitude.
        /// </summary>
        East = 1,

        /// <summary>
        /// West longitude.
        /// </summary>
        West = 2,

        /// <summary>
        /// Calendar line.
        /// </summary>
        CalendarLine = 3
    }

    /// <summary>
    /// Latitude.
    /// </summary>
    public class Latitude : Maths.Angle
    {
        /// <summary>
        /// Initializes a new instance of the Latitude class.
        /// </summary>
        public Latitude() : base(new BoundaryOptions(90, true, RectifyModes.Bounce))
        {
        }

        /// <summary>
        /// Gets or sets the latitue zone.
        /// </summary>
        public Latitudes Zone
        {
            get
            {
                if (IsZero) return Latitudes.Equator;
                return Positive ? Latitudes.North : Latitudes.South;
            }

            set
            {
                switch (value)
                {
                    case Latitudes.Equator:
                        Degrees = 0;
                        break;
                    case Latitudes.North:
                        if (IsNegative) Degree = -Degree;
                        break;
                    case Latitudes.South:
                        if (Positive) Degree = -Degree;
                        break;
                }
            }
        }

        /// <summary>
        /// Returns the latitude string value of this instance.
        /// </summary>
        /// <returns>A System.String containing this latitude.</returns>
        public override string ToString()
        {
            if (IsZero) return Maths.NumberSymbols.NumberZero + Symbols.DegreeUnit;
            return string.Format("{0}°{1}", Degrees, Positive ? "N" : "S");
        }
    }

    /// <summary>
    /// Longitude.
    /// </summary>
    public class Longitude : Maths.Angle
    {
        /// <summary>
        /// Initializes a new instance of the Longitude class.
        /// </summary>
        public Longitude() : base(new BoundaryOptions(180, true, RectifyModes.Cycle))
        {
        }

        /// <summary>
        /// Gets or sets the longitude zone.
        /// </summary>
        public Longitudes Zone
        {
            get
            {
                var degrees = Degrees;
                if (degrees == 0) return Longitudes.PrimeMeridian;
                if (degrees == 180 || degrees == -180) return Longitudes.CalendarLine;
                return degrees > 0 ? Longitudes.East : Longitudes.West;
            }

            set
            {
                switch (value)
                {
                    case Longitudes.PrimeMeridian:
                        Degrees = 0;
                        break;
                    case Longitudes.CalendarLine:
                        Degrees = 180;
                        break;
                    case Longitudes.East:
                        if (IsNegative) Degree = -Degree;
                        break;
                    case Longitudes.West:
                        if (Positive) Degree = -Degree;
                        break;
                }
            }
        }

        /// <summary>
        /// Returns the longitude string value of this instance.
        /// </summary>
        /// <returns>A System.String containing this longitude.</returns>
        public override string ToString()
        {
            var degrees = Degrees;
            if (IsZero) return Maths.NumberSymbols.NumberZero + Symbols.DegreeUnit;
            if (degrees == 180 || degrees == -180) return "180" + Symbols.DegreeUnit;
            return string.Format("{0}°{1}", degrees, Positive ? "E" : "W");
        }
    }

    /// <summary>
    /// Geolocation with latitude and longitude.
    /// </summary>
    public class PlaneGeolocation : IEquatable<PlaneGeolocation>
    {
        /// <summary>
        /// Initializes a new instance of the PlaneGeolocation class.
        /// </summary>
        public PlaneGeolocation()
        {
        }

        /// <summary>
        /// Initializes a new instance of the PlaneGeolocation class.
        /// </summary>
        /// <param name="latitude">The latitude.</param>
        /// <param name="longitude">The longitude.</param>
        public PlaneGeolocation(Latitude latitude, Longitude longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        /// <summary>
        /// Initializes a new instance of the PlaneGeolocation class.
        /// </summary>
        /// <param name="latitude">The latitude.</param>
        /// <param name="longitude">The longitude.</param>
        /// <param name="radiusDeviation">The radius deviation in meters.</param>
        public PlaneGeolocation(Latitude latitude, Longitude longitude, double radiusDeviation) : this(latitude, longitude)
        {
            RadiusDeviation = radiusDeviation;
        }

        /// <summary>
        /// Gets or sets the radius deviation in meters.
        /// </summary>
        public double RadiusDeviation { get; set; }

        /// <summary>
        /// Gets or sets the latitude.
        /// </summary>
        public Latitude Latitude { get; } = new Latitude();

        /// <summary>
        /// Gets or sets the longitude.
        /// </summary>
        public Longitude Longitude { get; } = new Longitude();

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return string.Format("{0}; {1}. (r {2})", Latitude, Longitude, RadiusDeviation).GetHashCode();
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public virtual bool Equals(PlaneGeolocation other)
        {
            if (other == null) return false;
            return Latitude == other.Latitude && Longitude == other.Longitude && RadiusDeviation == other.RadiusDeviation;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public override bool Equals(object other)
        {
            if (other == null) return false;
            if (other is PlaneGeolocation l) return Latitude == l.Latitude && Longitude == l.Longitude && RadiusDeviation == l.RadiusDeviation;
            return false;
        }

        /// <summary>
        /// Compares two locations to indicate if they are same.
        /// leftValue == rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>A result after subtration.</returns>
        public static bool operator ==(PlaneGeolocation leftValue, PlaneGeolocation rightValue)
        {
            if (leftValue == null && rightValue == null) return true;
            if (leftValue == null) return false;
            return leftValue.Equals(rightValue);
        }

        /// <summary>
        /// Compares two locations to indicate if they are different.
        /// leftValue != rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>A result after subtration.</returns>
        public static bool operator !=(PlaneGeolocation leftValue, PlaneGeolocation rightValue)
        {
            if (leftValue == null && rightValue == null) return false;
            if (leftValue == null) return true;
            return !leftValue.Equals(rightValue);
        }

        /// <summary>
        /// Returns the geolocation string value of this instance.
        /// </summary>
        /// <returns>A System.String containing this geolocation instance.</returns>
        public override string ToString()
        {
            return string.Format("Latitude = {0}; Longtitude = {1}", Latitude, Longitude);
        }
    }

    /// <summary>
    /// Geolocation with latitude, longitude and altitude.
    /// </summary>
    public class Geolocation : PlaneGeolocation, IEquatable<Geolocation>
    {
        /// <summary>
        /// Initializes a new instance of the Geolocation class.
        /// </summary>
        public Geolocation()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Geolocation class.
        /// </summary>
        /// <param name="latitude">The latitude.</param>
        /// <param name="longitude">The longitude.</param>
        public Geolocation(Latitude latitude, Longitude longitude) : base(latitude, longitude)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Geolocation class.
        /// </summary>
        /// <param name="latitude">The latitude.</param>
        /// <param name="longitude">The longitude.</param>
        /// <param name="altitude">The altitude.</param>
        public Geolocation(Latitude latitude, Longitude longitude, double altitude) : base(latitude, longitude)
        {
            Altitude = altitude;
        }

        /// <summary>
        /// Initializes a new instance of the Geolocation class.
        /// </summary>
        /// <param name="latitude">The latitude.</param>
        /// <param name="longitude">The longitude.</param>
        /// <param name="altitude">The altitude.</param>
        /// <param name="desc">The place description.</param>
        public Geolocation(Latitude latitude, Longitude longitude, double altitude, string desc) : base(latitude, longitude)
        {
            Altitude = altitude;
            Description = desc;
        }

        /// <summary>
        /// Initializes a new instance of the Geolocation class.
        /// </summary>
        /// <param name="latitude">The latitude.</param>
        /// <param name="longitude">The longitude.</param>
        /// <param name="altitude">The altitude.</param>
        /// <param name="radiusDeviation">The radius deviation in meters.</param>
        /// <param name="altitudeDeviation">The altitude deviation in meters.</param>
        public Geolocation(Latitude latitude, Longitude longitude, double altitude, double radiusDeviation, double altitudeDeviation) : base(latitude, longitude, radiusDeviation)
        {
            Altitude = altitude;
            AltitudeDeviation = altitudeDeviation;
        }

        /// <summary>
        /// Initializes a new instance of the Geolocation class.
        /// </summary>
        /// <param name="latitude">The latitude.</param>
        /// <param name="longitude">The longitude.</param>
        /// <param name="altitude">The altitude.</param>
        /// <param name="desc">The place description.</param>
        /// <param name="radiusDeviation">The radius deviation in meters.</param>
        /// <param name="altitudeDeviation">The altitude deviation in meters.</param>
        public Geolocation(Latitude latitude, Longitude longitude, double altitude, string desc, double radiusDeviation, double altitudeDeviation) : base(latitude, longitude, radiusDeviation)
        {
            Altitude = altitude;
            AltitudeDeviation = altitudeDeviation;
            Description = desc;
        }

        /// <summary>
        /// Gets or sets the place description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the altitude.
        /// </summary>
        public double Altitude { get; set; }

        /// <summary>
        /// Gets or sets the altitude deviation in meters.
        /// </summary>
        public double AltitudeDeviation { get; set; }

        /// <summary>
        /// Tests if the given altitude is in the deviation of this instance.
        /// </summary>
        /// <param name="value">The altitude to test.</param>
        /// <returns>true if they are in the same altitude; otherwise, false.</returns>
        public bool IsAltitude(double value)
        {
            var min = Altitude - AltitudeDeviation;
            var max = Altitude + AltitudeDeviation;
            return Math.Min(min, max) <= value && value <= Math.Max(min, max);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return string.Format("{0}; {1}; {2}. (r {3} & a {4})", Latitude, Longitude, Altitude, RadiusDeviation, AltitudeDeviation).GetHashCode();
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public virtual bool Equals(Geolocation other)
        {
            if (other == null) return false;
            return Latitude == other.Latitude && Longitude == other.Longitude && Altitude == other.Altitude && RadiusDeviation == other.RadiusDeviation && AltitudeDeviation == other.AltitudeDeviation;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public override bool Equals(PlaneGeolocation other)
        {
            if (other == null || !IsAltitude(0)) return false;
            return Latitude == other.Latitude && Longitude == other.Longitude && RadiusDeviation == other.RadiusDeviation;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public override bool Equals(object other)
        {
            if (other == null) return false;
            if (other is Geolocation l) return Latitude == l.Latitude && Longitude == l.Longitude && Altitude == l.Altitude && RadiusDeviation == l.RadiusDeviation && AltitudeDeviation == l.AltitudeDeviation;
            if (other is PlaneGeolocation p) return IsAltitude(0) && Latitude == p.Latitude && Longitude == p.Longitude && RadiusDeviation == p.RadiusDeviation;
            return false;
        }

        /// <summary>
        /// Returns the geolocation string value of this instance.
        /// </summary>
        /// <returns>A System.String containing this geolocation instance.</returns>
        public override string ToString()
        {
            return string.Format("Latitude = {0}; Longtitude = {1}; Altitude = {2}", Latitude, Longitude, Altitude);
        }
    }
}
