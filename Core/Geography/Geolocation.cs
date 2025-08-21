using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Trivial.Maths;
using Trivial.Text;

namespace Trivial.Geography;

/// <summary>
/// Latitudes.
/// </summary>
public enum Latitudes : byte
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
public enum Longitudes : byte
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
[Guid("D0B365CC-B266-4A1C-8249-B2FABBBA73D3")]
public struct Latitude : IEquatable<Latitude>
{
    /// <summary>
    /// Latitude model.
    /// </summary>
    [Guid("2FCE08C5-797A-4E0D-8BB2-3D96897F2AA7")]
    public class Model : Angle.Model, ICloneable
    {
        /// <summary>
        /// Initializes a new instance of the Latitude.Model class.
        /// </summary>
        public Model() : base(BoundaryOptions)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Latitude.Model class.
        /// </summary>
        /// <param name="degrees">The total degrees.</param>
        public Model(double degrees) : base(degrees, BoundaryOptions)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Latitude.Model class.
        /// </summary>
        /// <param name="degree">The degree part.</param>
        /// <param name="minute">The minute part.</param>
        /// <param name="second">The second part.</param>
        public Model(int degree, int minute, float second = 0) : base(degree, minute, second, BoundaryOptions)
        {
        }

        /// <summary>
        /// Gets or sets the latitue zone.
        /// </summary>
        public Latitudes Type
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
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public override bool Equals(object other)
        {
            if (other is null) return false;
            if (other is IAngle a) return Degrees.Equals(a.Degrees);
            if (other is Latitude l) return Degrees.Equals(l.Value.Degrees);
            if (other is double d) return Degrees.Equals(d);
            return Degrees.Equals(other);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
            => base.GetHashCode();

        /// <summary>
        /// Returns the latitude string value of this instance.
        /// </summary>
        /// <returns>A System.String containing this latitude.</returns>
        public override string ToString()
        {
            if (IsZero) return Numbers.NumberZero + Angle.Symbols.DegreeUnit;
            return this.ToAbsAngleString() + (Positive ? "N" : "S");
        }

        /// <summary>
        /// Clones an object.
        /// </summary>
        /// <returns>The object copied from this instance.</returns>
        public new Model Clone()
            => new(Degree, Arcminute, Arcsecond);

        /// <summary>
        /// Clones an object.
        /// </summary>
        /// <returns>The object copied from this instance.</returns>
        object ICloneable.Clone()
            => Clone();

        /// <summary>
        /// Compares two latitudes to indicate if they are same.
        /// leftValue == rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if they are same; otherwise, false.</returns>
        public static bool operator ==(Model leftValue, Latitude rightValue)
            => leftValue is not null && leftValue.Equals(rightValue);

        /// <summary>
        /// Compares two latitudes to indicate if they are same.
        /// leftValue == rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if they are same; otherwise, false.</returns>
        public static bool operator ==(Model leftValue, Model rightValue)
        {
            if (ReferenceEquals(leftValue, rightValue)) return true;
            return leftValue is not null && leftValue.Equals(rightValue);
        }

        /// <summary>
        /// Compares two latitudes to indicate if they are different.
        /// leftValue != rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if they are different; otherwise, false.</returns>
        public static bool operator !=(Model leftValue, Latitude rightValue)
            => leftValue is null || !leftValue.Equals(rightValue);

        /// <summary>
        /// Compares two latitudes to indicate if they are different.
        /// leftValue != rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if they are different; otherwise, false.</returns>
        public static bool operator !=(Model leftValue, Model rightValue)
        {
            if (ReferenceEquals(leftValue, rightValue)) return false;
            return leftValue is null || !leftValue.Equals(rightValue);
        }

        /// <summary>
        /// Gets the geolocation.
        /// </summary>
        /// <param name="latitude">The latitude.</param>
        /// <param name="longitude">The longitude.</param>
        /// <returns>The geolocation.</returns>
        public static Geolocation.Model operator &(Model latitude, Longitude.Model longitude)
            => new(latitude, longitude);

        /// <summary>
        /// Converts a latitude to its model.
        /// </summary>
        /// <param name="value">The instance.</param>
        public static implicit operator Model(Latitude value)
            => new() { Degrees = value.Value.Degrees };
    }

    /// <summary>
    /// The angle boundary of the latitude.
    /// </summary>
    public static readonly Angle.BoundaryOptions BoundaryOptions = new(90, true, Angle.RectifyModes.Bounce);

    /// <summary>
    /// Initializes a new instance of the Latitude struct.
    /// </summary>
    /// <param name="type">The latitude type.</param>
    /// <param name="degree">The degree part.</param>
    /// <param name="minute">The minute part.</param>
    /// <param name="second">The second part.</param>
    public Latitude(Latitudes type, int degree, int minute, float second)
    {
        Type = type;
        switch (type)
        {
            case Latitudes.South:
                degree = -degree;
                break;
            case Latitudes.Equator:
                Value = new Angle(0, 0, 0);
                return;
            default:
                break;
        }

        Value = new Angle(degree, minute, second, BoundaryOptions);
    }

    /// <summary>
    /// Initializes a new instance of the Latitude struct.
    /// </summary>
    /// <param name="degree">The degree part.</param>
    /// <param name="minute">The minute part.</param>
    /// <param name="second">The second part.</param>
    public Latitude(int degree, int minute, float second)
    {
        Value = new Angle(degree, minute, second, BoundaryOptions);
        var degrees = Value.Degrees;
        if (degrees > 0) Type = Latitudes.North;
        else if (degrees < 0) Type = Latitudes.South;
        else Type = Latitudes.Equator;
    }

    /// <summary>
    /// Initializes a new instance of the Latitude struct.
    /// </summary>
    /// <param name="degrees">The total degrees.</param>
    public Latitude(double degrees) : this(new Angle(degrees, BoundaryOptions))
    {
    }

    /// <summary>
    /// Initializes a new instance of the Latitude struct.
    /// </summary>
    /// <param name="angle">The angle.</param>
    public Latitude(Angle angle) : this(angle.Degree, angle.Arcminute, angle.Arcsecond)
    {
    }

    /// <summary>
    /// Initializes a new instance of the Latitude struct.
    /// </summary>
    /// <param name="type">The latitude type.</param>
    /// <param name="degrees">The total degrees.</param>
    public Latitude(Latitudes type, double degrees) : this(GetDegrees(type, degrees))
    {
    }

    /// <summary>
    /// Gets the latitue zone.
    /// </summary>
    public Latitudes Type { get; }

    /// <summary>
    /// The angle value in the latitude type.
    /// </summary>
    public Angle Value { get; }

    /// <summary>
    /// The angle degrees of the latitude.
    /// </summary>
    public double Degrees => Value.Degrees;

    /// <summary>
    /// The absolute angle degrees in the latitude type.
    /// </summary>
    public double AbsDegrees => Value.AbsDegrees;

    /// <summary>
    /// The angle degrees of the latitude.
    /// </summary>
    public int Degree => Value.Degree;

    /// <summary>
    /// The absolute angle degrees in the latitude type.
    /// </summary>
    public int AbsDegree => Value.Degree;

    /// <summary>
    /// Gets the arcminute of the latitude.
    /// </summary>
    public int Arcminute => Value.Arcminute;

    /// <summary>
    /// Gets the arcsecond of the latitude.
    /// </summary>
    public float Arcsecond => Value.Arcsecond;

    /// <summary>
    /// Returns the hash code for this instance.
    /// </summary>
    /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
    public override int GetHashCode()
        => Value.Degrees.GetHashCode();

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <returns>
    /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
    /// </returns>
    /// <param name="other">An object to compare with this object.</param>
    public override bool Equals(object other)
    {
        if (other is null) return false;
        if (other is Latitude l) return Value.Degrees.Equals(l.Value.Degrees);
        if (other is Model m) return Value.Degrees.Equals(m.Degrees);
        return Value.Equals(other);
    }

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <returns>
    /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
    /// </returns>
    /// <param name="other">An object to compare with this object.</param>
    public bool Equals(Latitude other)
        => Value == other.Value;

    /// <summary>
    /// Returns the latitude string value of this instance.
    /// </summary>
    /// <returns>A System.String containing this latitude.</returns>
    public override string ToString()
    {
        if (Value.IsZero) return Numbers.NumberZero + Angle.Symbols.DegreeUnit;
        return Value.ToAbsAngleString() + (Value.Positive ? "N" : "S");
    }

    /// <summary>
    /// Compares two latitudes to indicate if they are same.
    /// leftValue == rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are same; otherwise, false.</returns>
    public static bool operator ==(Latitude leftValue, Latitude rightValue)
        => leftValue.Value.Degrees == rightValue.Value.Degrees;

    /// <summary>
    /// Compares two latitudes to indicate if they are same.
    /// leftValue == rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are same; otherwise, false.</returns>
    public static bool operator ==(Latitude leftValue, Model rightValue)
        => rightValue is not null && leftValue.Value.Degrees == rightValue.Degrees;

    /// <summary>
    /// Compares two latitudes to indicate if they are different.
    /// leftValue != rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are different; otherwise, false.</returns>
    public static bool operator !=(Latitude leftValue, Latitude rightValue)
        => leftValue.Value.Degrees != rightValue.Value.Degrees;

    /// <summary>
    /// Compares two latitudes to indicate if they are different.
    /// leftValue != rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are different; otherwise, false.</returns>
    public static bool operator !=(Latitude leftValue, Model rightValue)
        => rightValue is null || leftValue.Value.Degrees != rightValue.Degrees;

    /// <summary>
    /// Converts a number to latitude.
    /// </summary>
    /// <param name="value">The instance.</param>
    public static implicit operator Latitude(Model value)
    {
        if (value is null) return new Latitude();
        return new Latitude(value.Degrees);
    }

    /// <summary>
    /// Gets the geolocation.
    /// </summary>
    /// <param name="latitude">The latitude.</param>
    /// <param name="longitude">The longitude.</param>
    /// <returns>The geolocation.</returns>
    public static Geolocation operator &(Latitude latitude, Longitude longitude)
        => new(latitude, longitude);

    private static double GetDegrees(Latitudes type, double degrees)
    {
        return type switch
        {
            Latitudes.North => degrees,
            Latitudes.South => -degrees,
            _ => 0,
        };
    }
}

/// <summary>
/// Longitude.
/// </summary>
[Guid("459EE246-77FF-4BE8-8F73-31C28A0F8B44")]
public struct Longitude : IEquatable<Longitude>
{
    /// <summary>
    /// Longitude model.
    /// </summary>
    [Guid("AC1C2B92-D66E-410D-AEBC-B6BDD00E1EB0")]
    public class Model : Angle.Model, ICloneable
    {
        /// <summary>
        /// The flag indicating whether it is celestial longitude.
        /// </summary>
        private bool celestial;

        /// <summary>
        /// Initializes a new instance of the Longitude.Model class.
        /// </summary>
        public Model() : base(BoundaryOptions)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Longitude.Model class.
        /// </summary>
        /// <param name="isCelestial">true if it is celestial longitude; otherwise, false.</param>
        public Model(bool isCelestial) : base(isCelestial ? CelestialBoundaryOptions : BoundaryOptions)
        {
            IsCelestial = isCelestial;
        }

        /// <summary>
        /// Initializes a new instance of the Longitude.Model class.
        /// </summary>
        /// <param name="degrees">The total degrees.</param>
        public Model(double degrees) : base(degrees, BoundaryOptions)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Longitude.Model class.
        /// </summary>
        /// <param name="degree">The degree part.</param>
        /// <param name="minute">The minute part.</param>
        /// <param name="second">The second part.</param>
        public Model(int degree, int minute, float second = 0) : base(degree, minute, second, BoundaryOptions)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Longitude.Model class.
        /// </summary>
        /// <param name="isCelestial">true if it is celestial longitude; otherwise, false.</param>
        /// <param name="degrees">The total degrees.</param>
        public Model(bool isCelestial, double degrees) : base(degrees, isCelestial ? CelestialBoundaryOptions : BoundaryOptions)
        {
            IsCelestial = isCelestial;
        }

        /// <summary>
        /// Initializes a new instance of the Longitude.Model class.
        /// </summary>
        /// <param name="isCelestial">true if it is celestial longitude; otherwise, false.</param>
        /// <param name="degree">The degree part.</param>
        /// <param name="minute">The minute part.</param>
        /// <param name="second">The second part.</param>
        public Model(bool isCelestial, int degree, int minute, float second = 0) : base(degree, minute, second, isCelestial ? CelestialBoundaryOptions : BoundaryOptions)
        {
            IsCelestial = isCelestial;
        }

        /// <summary>
        /// Gets a value indicating wether it is celestial longitude.
        /// </summary>
        public bool IsCelestial
        {
            get
            {
                return celestial;
            }

            set
            {
                if (value == celestial) return;
                var degrees = Degrees;
                celestial = value;
                if (value)
                {
                    if (degrees < 0) Degrees = degrees + 360;
                    return;
                }

                if (degrees > 180) Degrees = degrees - 360;
            }
        }

        /// <summary>
        /// Gets or sets the longitude zone.
        /// </summary>
        public Longitudes Type
        {
            get
            {
                var degrees = Degrees;
                if (degrees == 0 || degrees == 360) return Longitudes.PrimeMeridian;
                if (degrees == 180 || degrees == -180) return Longitudes.CalendarLine;
                if (IsCelestial) return degrees > 180 ? Longitudes.West : Longitudes.East;
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
                        if (IsCelestial)
                        {
                            var degrees = Degrees;
                            if (degrees > 180) Degrees = degrees - 180;
                            break;
                        }
                        
                        if (IsNegative) Degree = -Degree;
                        break;
                    case Longitudes.West:
                        if (IsCelestial)
                        {
                            var degrees = Degrees;
                            if (degrees < 180 && degrees > 0) Degrees = degrees + 180;
                            break;
                        }
                        
                        if (Positive) Degree = -Degree;
                        break;
                }
            }
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
            if (other is null) return false;
            if (other is IAngle a) return Degrees.Equals(a.Degrees);
            if (other is Longitude l) return Degrees.Equals(l.Value.Degrees);
            if (other is double d) return Degrees.Equals(d);
            return Degrees.Equals(other);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
            => base.GetHashCode();

        /// <summary>
        /// Returns the longitude string value of this instance.
        /// </summary>
        /// <returns>A System.String containing this longitude.</returns>
        public override string ToString()
        {
            if (IsCelestial) return base.ToString();
            var degrees = Degrees;
            if (degrees == 0) return Numbers.NumberZero + Angle.Symbols.DegreeUnit;
            if (degrees == 180 || degrees == -180) return "180" + Angle.Symbols.DegreeUnit;
            return this.ToAbsAngleString() + (Positive ? "E" : "W");
        }

        /// <summary>
        /// Clones an object.
        /// </summary>
        /// <returns>The object copied from this instance.</returns>
        public new Model Clone()
            => new(IsCelestial, Degree, Arcminute, Arcsecond);

        /// <summary>
        /// Clones an object.
        /// </summary>
        /// <returns>The object copied from this instance.</returns>
        object ICloneable.Clone()
            => Clone();

        /// <summary>
        /// Compares two longitudes to indicate if they are same.
        /// leftValue == rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if they are same; otherwise, false.</returns>
        public static bool operator ==(Model leftValue, Longitude rightValue)
            => leftValue is not null && leftValue.Equals(rightValue);

        /// <summary>
        /// Compares two longitudes to indicate if they are same.
        /// leftValue == rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if they are same; otherwise, false.</returns>
        public static bool operator ==(Model leftValue, Model rightValue)
        {
            if (ReferenceEquals(leftValue, rightValue)) return true;
            return leftValue is not null && leftValue.Equals(rightValue);
        }

        /// <summary>
        /// Compares two longitudes to indicate if they are different.
        /// leftValue != rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if they are different; otherwise, false.</returns>
        public static bool operator !=(Model leftValue, Longitude rightValue)
            => leftValue is null || !leftValue.Equals(rightValue);

        /// <summary>
        /// Compares two longitudes to indicate if they are different.
        /// leftValue != rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if they are different; otherwise, false.</returns>
        public static bool operator !=(Model leftValue, Model rightValue)
        {
            if (ReferenceEquals(leftValue, rightValue)) return false;
            return leftValue is null || !leftValue.Equals(rightValue);
        }

        /// <summary>
        /// Gets the geolocation.
        /// </summary>
        /// <param name="longitude">The longitude.</param>
        /// <param name="latitude">The latitude.</param>
        /// <returns>The geolocation.</returns>
        public static Geolocation.Model operator &(Model longitude, Latitude.Model latitude)
            => new(latitude, longitude);

        /// <summary>
        /// Converts a longitude to its model.
        /// </summary>
        /// <param name="value">The instance.</param>
        public static implicit operator Model(Longitude value)
            => new() { Degrees = value.Value.Degrees };
    }

    /// <summary>
    /// The angle boundary of the longitude.
    /// </summary>
    public static readonly Angle.BoundaryOptions BoundaryOptions = new(180, true, Angle.RectifyModes.Cycle);

    /// <summary>
    /// The angle boundary of the celestial longitude.
    /// </summary>
    public static readonly Angle.BoundaryOptions CelestialBoundaryOptions = new(360, false, Angle.RectifyModes.Cycle);

    /// <summary>
    /// Initializes a new instance of the Longitude struct.
    /// </summary>
    /// <param name="type">The latitude type.</param>
    /// <param name="degree">The degree part.</param>
    /// <param name="minute">The minute part.</param>
    /// <param name="second">The second part.</param>
    public Longitude(Longitudes type, int degree, int minute, float second = 0)
    {
        IsCelestial = false;
        Type = type;
        switch (type)
        {
            case Longitudes.PrimeMeridian:
                Value = new Angle(0);
                return;
            case Longitudes.CalendarLine:
                Value = new Angle(degree == -180 ? -180 : 180);
                Type = Longitudes.CalendarLine;
                return;
            case Longitudes.West:
                Value = new Angle(-Math.Abs(degree % 360));
                return;
            case Longitudes.East:
                Value = new Angle(Math.Abs(degree % 360));
                return;
        }

        Value = new Angle(degree, minute, second, BoundaryOptions);
        var degrees = Value.Degrees;
        if (degrees == 180 || degrees == -180) Type = Longitudes.CalendarLine;
        else if (degrees > 0) Type = Longitudes.East;
        else if (degrees < 0) Type = Longitudes.West;
        else Type = Longitudes.PrimeMeridian;
    }

    /// <summary>
    /// Initializes a new instance of the Longitude struct.
    /// </summary>
    /// <param name="degree">The degree part.</param>
    /// <param name="minute">The minute part.</param>
    /// <param name="second">The second part.</param>
    public Longitude(int degree, int minute, float second = 0) : this(false, degree, minute, second)
    {
    }

    /// <summary>
    /// Initializes a new instance of the Longitude struct.
    /// </summary>
    /// <param name="isCelestial">true if it is celestial longitude; otherwise, false.</param>
    /// <param name="degree">The degree part.</param>
    /// <param name="minute">The minute part.</param>
    /// <param name="second">The second part.</param>
    public Longitude(bool isCelestial, int degree, int minute, float second = 0)
    {
        IsCelestial = isCelestial;
        Value = new Angle(degree, minute, second, isCelestial ? CelestialBoundaryOptions : BoundaryOptions);
        var degrees = Value.Degrees;
        if (degrees == 180 || degrees == -180) Type = Longitudes.CalendarLine;
        else if (isCelestial && degrees > 180) Type = Longitudes.West;
        else if (degrees > 0) Type = Longitudes.East;
        else if (degrees < 0) Type = Longitudes.West;
        else Type = Longitudes.PrimeMeridian;
    }

    /// <summary>
    /// Initializes a new instance of the Longitude struct.
    /// </summary>
    /// <param name="degrees">The total degrees.</param>
    public Longitude(double degrees) : this(false, new Angle(degrees, BoundaryOptions))
    {
    }

    /// <summary>
    /// Initializes a new instance of the Longitude struct.
    /// </summary>
    /// <param name="isCelestial">true if it is celestial longitude; otherwise, false.</param>
    /// <param name="degrees">The total degrees.</param>
    public Longitude(bool isCelestial, double degrees) : this(isCelestial, new Angle(degrees, isCelestial ? CelestialBoundaryOptions : BoundaryOptions))
    {
    }

    /// <summary>
    /// Initializes a new instance of the Longitude struct.
    /// </summary>
    /// <param name="angle">The angle.</param>
    public Longitude(Angle angle) : this(false, angle.Degree, angle.Arcminute, angle.Arcsecond)
    {
    }

    /// <summary>
    /// Initializes a new instance of the Longitude struct.
    /// </summary>
    /// <param name="isCelestial">true if it is celestial longitude; otherwise, false.</param>
    /// <param name="angle">The angle.</param>
    public Longitude(bool isCelestial, Angle angle) : this(isCelestial, angle.Degree, angle.Arcminute, angle.Arcsecond)
    {
    }

    /// <summary>
    /// Initializes a new instance of the Longitude struct.
    /// </summary>
    /// <param name="type">The longitude type.</param>
    /// <param name="degrees">The total degrees.</param>
    public Longitude(Longitudes type, double degrees) : this(GetDegrees(type, degrees))
    {
    }

    /// <summary>
    /// Gets a value indicating wether it is celestial longitude.
    /// </summary>
    public bool IsCelestial { get; }

    /// <summary>
    /// Gets the longitude zone.
    /// </summary>
    public Longitudes Type { get; }

    /// <summary>
    /// The angle value in the longitude type.
    /// </summary>
    public Angle Value { get; }

    /// <summary>
    /// The angle degrees of the longitude.
    /// </summary>
    public double Degrees => Value.Degrees;

    /// <summary>
    /// The absolute angle degrees in the longitude type.
    /// </summary>
    public double AbsDegrees => Value.AbsDegrees;

    /// <summary>
    /// The angle degrees of the longitude.
    /// </summary>
    public int Degree => Value.Degree;

    /// <summary>
    /// The absolute angle degrees in the longitude type.
    /// </summary>
    public int AbsDegree => Value.Degree;

    /// <summary>
    /// Gets the arcminute of the longitude.
    /// </summary>
    public int Arcminute => Value.Arcminute;

    /// <summary>
    /// Gets the arcsecond of the longitude.
    /// </summary>
    public float Arcsecond => Value.Arcsecond;

    /// <summary>
    /// Returns the hash code for this instance.
    /// </summary>
    /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
    public override int GetHashCode()
        => Value.Degrees.GetHashCode();

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <returns>
    /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
    /// </returns>
    /// <param name="other">An object to compare with this object.</param>
    public override bool Equals(object other)
    {
        if (other is null) return false;
        if (other is Longitude l) return Value.Degrees.Equals(l.Value.Degrees);
        if (other is Model m) return Value.Degrees.Equals(m.Degrees);
        return Value.Equals(other);
    }

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <returns>
    /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
    /// </returns>
    /// <param name="other">An object to compare with this object.</param>
    public bool Equals(Longitude other)
        => Value == other.Value;

    /// <summary>
    /// Returns the latitude string value of this instance.
    /// </summary>
    /// <returns>A System.String containing this latitude.</returns>
    public override string ToString()
    {
        if (IsCelestial) return base.ToString();
        var degrees = Value.Degrees;
        if (degrees == 0) return Numbers.NumberZero + Angle.Symbols.DegreeUnit;
        if (degrees == 180 || degrees == -180) return "180" + Angle.Symbols.DegreeUnit;
        return Value.ToAbsAngleString() + (Value.Positive ? "E" : "W");
    }

    /// <summary>
    /// Compares two longitudes to indicate if they are same.
    /// leftValue == rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are same; otherwise, false.</returns>
    public static bool operator ==(Longitude leftValue, Longitude rightValue)
        => leftValue.Value.Degrees == rightValue.Value.Degrees;

    /// <summary>
    /// Compares two longitudes to indicate if they are same.
    /// leftValue == rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are same; otherwise, false.</returns>
    public static bool operator ==(Longitude leftValue, Model rightValue)
        => rightValue is not null && leftValue.Value.Degrees == rightValue.Degrees;

    /// <summary>
    /// Compares two longitudes to indicate if they are different.
    /// leftValue != rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are different; otherwise, false.</returns>
    public static bool operator !=(Longitude leftValue, Longitude rightValue)
        => leftValue.Value.Degrees != rightValue.Value.Degrees;

    /// <summary>
    /// Compares two longitudes to indicate if they are different.
    /// leftValue != rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are different; otherwise, false.</returns>
    public static bool operator !=(Longitude leftValue, Model rightValue)
        => rightValue is null || leftValue.Value.Degrees != rightValue.Degrees;

    /// <summary>
    /// Converts a number to longitude.
    /// </summary>
    /// <param name="value">The instance.</param>
    public static implicit operator Longitude(Model value)
    {
        if (value is null) return new Longitude();
        return new Longitude(value.Degrees);
    }

    /// <summary>
    /// Gets the geolocation.
    /// </summary>
    /// <param name="longitude">The longitude.</param>
    /// <param name="latitude">The latitude.</param>
    /// <returns>The geolocation.</returns>
    public static Geolocation operator &(Longitude longitude, Latitude latitude)
        => new(latitude, longitude);

    private static double GetDegrees(Longitudes type, double degrees)
    {
        return type switch
        {
            Longitudes.East => degrees,
            Longitudes.West => -degrees,
            Longitudes.CalendarLine => 180,
            _ => 0,
        };
    }
}

/// <summary>
/// Geolocation with latitude, longitude and optional altitude.
/// </summary>
[JsonConverter(typeof(ValueConverter))]
[Guid("A40A530E-0D87-4CF7-BF00-184BB18F614E")]
public struct Geolocation : IEquatable<Geolocation>
{
    /// <summary>
    /// The JSON converter for enum. The output will be an integer value.
    /// </summary>
    internal class ValueConverter : JsonConverter<Geolocation>
    {
        /// <inheritdoc />
        public override Geolocation Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException($"The token type is {reader.TokenType} but expect an object.");
            var json = JsonObjectNode.ParseValue(ref reader) ?? throw new JsonException("Parse JSON failed.");
            if (!json.TryGetDoubleValue("longitude", out var longitude) || !json.TryGetDoubleValue("latitude", out var latitude))
                throw new JsonException("longitude and latitude are required.");
            return new(new Latitude(latitude), new Longitude(longitude), json.TryGetDoubleValue("altitude", false), json.TryGetStringTrimmedValue("desc"), json.TryGetDoubleValue("deviation"), json.TryGetDoubleValue("altitudeDeviation"));
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, Geolocation value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteNumber("latitude", value.Latitude.Degrees);
            writer.WriteNumber("longitude", value.Longitude.Degrees);
            if (value.Altitude.HasValue) writer.WriteNumber("altitude", value.Altitude.Value);
            if (!string.IsNullOrWhiteSpace(value.Description)) writer.WriteString("desc", value.Description);
            if (value.RadiusDeviation.HasValue) writer.WriteNumber("deviation", value.RadiusDeviation.Value);
            if (value.AltitudeDeviation.HasValue) writer.WriteNumber("altitudeDeviation", value.AltitudeDeviation.Value);
            writer.WriteEndObject();
        }
    }

    /// <summary>
    /// The JSON converter for enum. The output will be an integer value.
    /// </summary>
    internal class ModelConverter : JsonConverter<Model>
    {
        /// <inheritdoc />
        public override Model Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException($"The token type is {reader.TokenType} but expect an object.");
            var json = JsonObjectNode.ParseValue(ref reader) ?? throw new JsonException("Parse JSON failed.");
            if (!json.TryGetDoubleValue("longitude", out var longitude) || !json.TryGetDoubleValue("latitude", out var latitude))
                throw new JsonException("longitude and latitude are required.");
            var model = new Model(new Latitude(latitude), new Longitude(longitude), json.TryGetStringTrimmedValue("desc"));
            if (json.TryGetDoubleValue("altitude", out longitude))
                model.Altitude = longitude;
            if (json.TryGetDoubleValue("deviation", out longitude))
                model.RadiusDeviation = longitude;
            if (json.TryGetDoubleValue("altitudeDeviation", out longitude))
                model.AltitudeDeviation = longitude;
            return model;
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, Model value, JsonSerializerOptions options)
        {
            if (value is null)
            {
                writer.WriteNullValue();
                return;
            }

            writer.WriteStartObject();
            writer.WriteNumber("latitude", value.Latitude.Degrees);
            writer.WriteNumber("longitude", value.Longitude.Degrees);
            if (value.Altitude.HasValue) writer.WriteNumber("altitude", value.Altitude.Value);
            if (!string.IsNullOrWhiteSpace(value.Description)) writer.WriteString("desc", value.Description);
            if (value.RadiusDeviation.HasValue) writer.WriteNumber("deviation", value.RadiusDeviation.Value);
            if (value.AltitudeDeviation.HasValue) writer.WriteNumber("altitudeDeviation", value.AltitudeDeviation.Value);
            writer.WriteEndObject();
        }
    }

    /// <summary>
    /// The geolocation model.
    /// </summary>
    [JsonConverter(typeof(ModelConverter))]
    [Guid("818D8EC5-8E16-4210-AB69-AAF8AD70FCAF")]
    public class Model
    {
        /// <summary>
        /// Initializes a new instance of the Model class.
        /// </summary>
        public Model()
        {
            Latitude = new Latitude.Model();
            Longitude = new Longitude.Model();
            Altitude = null;
        }

        /// <summary>
        /// Initializes a new instance of the Model class.
        /// </summary>
        /// <param name="latitude">The latitude.</param>
        /// <param name="longitude">The longitude.</param>
        /// <param name="desc">The optional description.</param>
        public Model(Latitude.Model latitude, Longitude.Model longitude, string desc = null)
        {
            Latitude = latitude;
            Longitude = longitude;
            Altitude = null;
            Description = desc;
        }

        /// <summary>
        /// Initializes a new instance of the Model class.
        /// </summary>
        /// <param name="latitude">The latitude.</param>
        /// <param name="longitude">The longitude.</param>
        /// <param name="altitude">The altitude.</param>
        /// <param name="desc">The optional description.</param>
        public Model(Latitude.Model latitude, Longitude.Model longitude, double altitude, string desc = null)
        {
            Latitude = latitude;
            Longitude = longitude;
            Altitude = altitude;
            Description = desc;
        }

        /// <summary>
        /// Gets the latitude.
        /// </summary>
        public Latitude.Model Latitude { get; }

        /// <summary>
        /// Gets the longitude.
        /// </summary>
        public Longitude.Model Longitude { get; }

        /// <summary>
        /// Gets or sets the radius deviation in meters.
        /// </summary>
        public double? RadiusDeviation { get; set; }

        /// <summary>
        /// Gets or sets the altitude.
        /// </summary>
        public double? Altitude { get; set; }

        /// <summary>
        /// Gets or sets the altitude deviation in meters.
        /// </summary>
        public double? AltitudeDeviation { get; set; }

        /// <summary>
        /// Gets or sets the place description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Tests if the given altitude is in the deviation of this instance.
        /// </summary>
        /// <param name="value">The altitude to test.</param>
        /// <returns>true if they are in the same altitude; otherwise, false.</returns>
        public bool IsAltitude(double value)
        {
            if (!Altitude.HasValue) return true;
            if (!AltitudeDeviation.HasValue || AltitudeDeviation == 0) return Altitude == value;
            var min = Altitude.Value - AltitudeDeviation.Value;
            var max = Altitude.Value + AltitudeDeviation.Value;
            return Math.Min(min, max) <= value && value <= Math.Max(min, max);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
            => string.Format(CultureInfo.InvariantCulture, "{0}; {1}; {2}. (r {3} & a {4})", Latitude, Longitude, Altitude, RadiusDeviation, AltitudeDeviation).GetHashCode();

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(Geolocation other)
            => Latitude == other.Latitude && Longitude == other.Longitude && Altitude == other.Altitude && RadiusDeviation == other.RadiusDeviation && AltitudeDeviation == other.AltitudeDeviation;

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(Model other)
            => Latitude == other.Latitude && Longitude == other.Longitude && Altitude == other.Altitude && RadiusDeviation == other.RadiusDeviation && AltitudeDeviation == other.AltitudeDeviation;

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
            if (other is Model m) return Latitude == m.Latitude && Longitude == m.Longitude && Altitude == m.Altitude && RadiusDeviation == m.RadiusDeviation && AltitudeDeviation == m.AltitudeDeviation;
            return false;
        }

        /// <summary>
        /// Returns the geolocation string value of this instance.
        /// </summary>
        /// <returns>A System.String containing this geolocation instance.</returns>
        public override string ToString()
        {
            if (!Altitude.HasValue) return string.Format("Latitude = {0}; Longtitude = {1}", Latitude, Longitude);
            return string.Format("Latitude = {0}; Longtitude = {1}; Altitude = {2}m", Latitude, Longitude, Altitude.Value);
        }

        /// <summary>
        /// Compares two longitudes to indicate if they are same.
        /// leftValue == rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if they are same; otherwise, false.</returns>
        public static bool operator ==(Model leftValue, Geolocation rightValue)
            => leftValue is not null && leftValue.Equals(rightValue);

        /// <summary>
        /// Compares two longitudes to indicate if they are same.
        /// leftValue == rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if they are same; otherwise, false.</returns>
        public static bool operator ==(Model leftValue, Model rightValue)
        {
            if (ReferenceEquals(leftValue, rightValue)) return true;
            return leftValue is not null && leftValue.Equals(rightValue);
        }

        /// <summary>
        /// Compares two longitudes to indicate if they are different.
        /// leftValue != rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if they are different; otherwise, false.</returns>
        public static bool operator !=(Model leftValue, Geolocation rightValue)
            => leftValue is null || !leftValue.Equals(rightValue);

        /// <summary>
        /// Compares two longitudes to indicate if they are different.
        /// leftValue != rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if they are different; otherwise, false.</returns>
        public static bool operator !=(Model leftValue, Model rightValue)
        {
            if (ReferenceEquals(leftValue, rightValue)) return false;
            return leftValue is null || !leftValue.Equals(rightValue);
        }
    }

    /// <summary>
    /// Initializes a new instance of the Geolocation class.
    /// </summary>
    /// <param name="latitude">The latitude.</param>
    /// <param name="longitude">The longitude.</param>
    /// <param name="desc">The place description.</param>
    /// <param name="radiusDeviation">The radius deviation in meters.</param>
    public Geolocation(Latitude latitude, Longitude longitude, string desc = null, double? radiusDeviation = null)
    {
        Latitude = latitude;
        Longitude = longitude;
        Altitude = null;
        RadiusDeviation = radiusDeviation;
        AltitudeDeviation = null;
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
    public Geolocation(Latitude latitude, Longitude longitude, double altitude, double radiusDeviation, double? altitudeDeviation = null) : this(latitude, longitude, altitude, null, altitudeDeviation, radiusDeviation)
    {
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
    public Geolocation(Latitude latitude, Longitude longitude, double altitude, string desc = null, double? radiusDeviation = null, double? altitudeDeviation = null)
    {
        Latitude = latitude;
        Longitude = longitude;
        Altitude = double.IsNaN(altitude) ? null : altitude;
        RadiusDeviation = radiusDeviation;
        AltitudeDeviation = altitudeDeviation;
        Description = desc;
    }

    /// <summary>
    /// Gets the latitude.
    /// </summary>
    public Latitude Latitude { get; }

    /// <summary>
    /// Gets the longitude.
    /// </summary>
    public Longitude Longitude { get; }

    /// <summary>
    /// Gets the radius deviation in meters.
    /// </summary>
    public double? RadiusDeviation { get; }

    /// <summary>
    /// Gets the altitude.
    /// </summary>
    public double? Altitude { get; }

    /// <summary>
    /// Gets the altitude deviation in meters.
    /// </summary>
    public double? AltitudeDeviation { get; }

    /// <summary>
    /// Gets the place description.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Tests if the given altitude is in the deviation of this instance.
    /// </summary>
    /// <param name="value">The altitude to test.</param>
    /// <returns>true if they are in the same altitude; otherwise, false.</returns>
    public bool IsAltitude(double value)
    {
        if (!Altitude.HasValue) return true;
        if (!AltitudeDeviation.HasValue || AltitudeDeviation == 0) return Altitude == value;
        var min = Altitude.Value - AltitudeDeviation.Value;
        var max = Altitude.Value + AltitudeDeviation.Value;
        return Math.Min(min, max) <= value && value <= Math.Max(min, max);
    }

    /// <summary>
    /// Returns the hash code for this instance.
    /// </summary>
    /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
    public override int GetHashCode()
        => string.Format(CultureInfo.InvariantCulture, "{0}; {1}; {2}. (r {3} & a {4})", Latitude, Longitude, Altitude, RadiusDeviation, AltitudeDeviation).GetHashCode();

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <returns>
    /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
    /// </returns>
    /// <param name="other">An object to compare with this object.</param>
    public bool Equals(Geolocation other)
        => Latitude == other.Latitude && Longitude == other.Longitude && Altitude == other.Altitude && RadiusDeviation == other.RadiusDeviation && AltitudeDeviation == other.AltitudeDeviation;

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <returns>
    /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
    /// </returns>
    /// <param name="other">An object to compare with this object.</param>
    public bool Equals(Model other)
        => Latitude == other.Latitude && Longitude == other.Longitude && Altitude == other.Altitude && RadiusDeviation == other.RadiusDeviation && AltitudeDeviation == other.AltitudeDeviation;

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
        if (other is Model m) return Latitude == m.Latitude && Longitude == m.Longitude && Altitude == m.Altitude && RadiusDeviation == m.RadiusDeviation && AltitudeDeviation == m.AltitudeDeviation;
        return false;
    }

    /// <summary>
    /// Returns the geolocation string value of this instance.
    /// </summary>
    /// <returns>A System.String containing this geolocation instance.</returns>
    public override string ToString()
    {
        if (!Altitude.HasValue) return string.Format("Latitude = {0}; Longtitude = {1}", Latitude, Longitude);
        return string.Format("Latitude = {0}; Longtitude = {1}; Altitude = {2}m", Latitude, Longitude, Altitude.Value);
    }

    /// <summary>
    /// Compares two longitudes to indicate if they are same.
    /// leftValue == rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are same; otherwise, false.</returns>
    public static bool operator ==(Geolocation leftValue, Geolocation rightValue)
        => leftValue.Equals(rightValue);

    /// <summary>
    /// Compares two longitudes to indicate if they are same.
    /// leftValue == rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are same; otherwise, false.</returns>
    public static bool operator ==(Geolocation leftValue, Model rightValue)
        => leftValue.Equals(rightValue);

    /// <summary>
    /// Compares two longitudes to indicate if they are different.
    /// leftValue != rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are different; otherwise, false.</returns>
    public static bool operator !=(Geolocation leftValue, Geolocation rightValue)
        => !leftValue.Equals(rightValue);

    /// <summary>
    /// Compares two longitudes to indicate if they are different.
    /// leftValue != rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are different; otherwise, false.</returns>
    public static bool operator !=(Geolocation leftValue, Model rightValue)
        => !leftValue.Equals(rightValue);
}
