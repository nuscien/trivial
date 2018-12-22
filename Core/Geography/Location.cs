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
    public class Latitude : IEquatable<Latitude>, IComparable<Latitude>
    {
        private double globalValue = 0;

        /// <summary>
        /// Initializes a new instance of the Latitude class.
        /// </summary>
        public Latitude()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Latitude class.
        /// </summary>
        /// <param name="raw">The raw value.</param>
        public Latitude(double raw) => globalValue = AdaptValue(raw);

        /// <summary>
        /// Initializes a new instance of the Latitude class.
        /// </summary>
        /// <param name="zone">The zone type.</param>
        /// <param name="value">The absolute value.</param>
        public Latitude(Latitudes zone, double value)
        {
            value = AdaptValue(value);
            if (value < 0)
            {
                if (zone == Latitudes.North) zone = Latitudes.South;
                else if (zone == Latitudes.South) zone = Latitudes.North;
                value = -value;
            }

            if (zone == Latitudes.North) globalValue = value;
            else if (zone == Latitudes.Equator) globalValue = 0;
            else globalValue = -value;
        }

        /// <summary>
        /// Gets or sets the raw value.
        /// </summary>
        public double GlobalValue
        {
            get
            {
                return globalValue;
            }

            set
            {
                if (value > 90 || value < -90)
                    throw new ArgumentOutOfRangeException(nameof(GlobalValue), "The value should be in 90°S and 90°N.");
                globalValue = value;
            }
        }

        /// <summary>
        /// Gets or sets the zone type.
        /// </summary>
        public Latitudes Zone
        {
            get
            {
                if (GlobalValue > 0) return Latitudes.North;
                else if (GlobalValue == 0) return Latitudes.Equator;
                else return Latitudes.South;
            }

            set
            {
                if (value == Latitudes.North) GlobalValue = Math.Abs(GlobalValue);
                else if (value == Latitudes.Equator) globalValue = 0;
                else GlobalValue = -Math.Abs(GlobalValue);
            }
        }

        /// <summary>
        /// Gets or sets the absolute value in the zone.
        /// </summary>
        public double Value
        {
            get
            {
                return Math.Abs(GlobalValue);
            }

            set
            {
                if (GlobalValue < 0) GlobalValue = -value;
                else GlobalValue = value;
            }
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="zone">The zone type.</param>
        /// <param name="value">The absolute value.</param>
        public void Set(Latitudes zone, double value)
        {
            value = AdaptValue(value);
            if (value < 0)
            {
                if (zone == Latitudes.North) zone = Latitudes.South;
                else if (zone == Latitudes.South) zone = Latitudes.North;
                value = -value;
            }

            if (zone == Latitudes.North) globalValue = value;
            else if (zone == Latitudes.Equator) globalValue = 0;
            else globalValue = -value;
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has the following meanings:
        /// Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.
        /// Zero This object is equal to <paramref name="other"/>.
        /// Greater than zero This object is greater than <paramref name="other"/>. 
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public int CompareTo(Latitude other)
        {
            if (other == null) return globalValue.CompareTo(null);
            return globalValue.CompareTo(other.GlobalValue);
        }

        /// <summary>
        /// Returns the angle string value of this instance.
        /// </summary>
        /// <returns>A System.String containing this angle.</returns>
        public override string ToString()
        {
            if (globalValue == 0) return globalValue + Maths.Angle.Symbols.DegreeUnit;
            return string.Format("{0}°{1}", Value, globalValue > 0 ? "N" : "S");
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return globalValue.GetHashCode();
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(Latitude other)
        {
            if (other == null) return false;
            return globalValue == other.GlobalValue;
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
            if (other is Latitude l) return globalValue == l.globalValue;
            if (other is double n1) return globalValue == n1;
            if (other is float n2) return globalValue == n2;
            if (other is int n3) return globalValue == n3;
            return false;
        }

        /// <summary>
        /// Converts a number to latitude.
        /// </summary>
        /// <param name="value">The raw value.</param>
        public static implicit operator Latitude(double value)
        {
            return new Latitude(value);
        }

        /// <summary>
        /// Converts a number to latitude.
        /// </summary>
        /// <param name="value">The raw value.</param>
        public static implicit operator Latitude(int value)
        {
            return new Latitude(value);
        }

        /// <summary>
        /// Compares two latitude to indicate if they are same.
        /// leftValue == rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>A result after subtration.</returns>
        public static bool operator ==(Latitude leftValue, Latitude rightValue)
        {
            if (leftValue == null && rightValue == null) return true;
            if (leftValue == null || rightValue == null) return false;
            return leftValue.globalValue == rightValue.globalValue;
        }

        /// <summary>
        /// Compares two latitude to indicate if they are different.
        /// leftValue != rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>A result after subtration.</returns>
        public static bool operator !=(Latitude leftValue, Latitude rightValue)
        {
            if (leftValue == null && rightValue == null) return false;
            if (leftValue == null || rightValue == null) return true;
            return leftValue.globalValue != rightValue.globalValue;
        }

        /// <summary>
        /// Compares if left is smaller than right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>A result after subtration.</returns>
        public static bool operator <(Latitude leftValue, Latitude rightValue)
        {
            if (leftValue == null && rightValue == null) return false;
            if (leftValue == null) return false;
            if (rightValue == null) return true;
            return leftValue.globalValue < rightValue.globalValue;
        }

        /// <summary>
        /// Compares if left is greater than right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>A result after subtration.</returns>
        public static bool operator >(Latitude leftValue, Latitude rightValue)
        {
            if (leftValue == null && rightValue == null) return false;
            if (leftValue == null) return true;
            if (rightValue == null) return false;
            return leftValue.globalValue < rightValue.globalValue;
        }

        /// <summary>
        /// Compares if left is smaller than or equals to right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>A result after subtration.</returns>
        public static bool operator <=(Latitude leftValue, Latitude rightValue)
        {
            if (leftValue == null && rightValue == null) return true;
            if (leftValue == null) return false;
            if (rightValue == null) return true;
            return leftValue.globalValue <= rightValue.globalValue;
        }

        /// <summary>
        /// Compares if left is greater than or equals to right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>A result after subtration.</returns>
        public static bool operator >=(Latitude leftValue, Latitude rightValue)
        {
            if (leftValue == null && rightValue == null) return true;
            if (leftValue == null) return true;
            if (rightValue == null) return false;
            return leftValue.globalValue <= rightValue.globalValue;
        }

        private double AdaptValue(double value)
        {
            if (value > 90)
            {
                if (value >= 360) value = value % 360;
                if (value > 90) value = 180 - value;
            }
            else if (value < -90)
            {
                if (value <= -360) value = -(-value % 360);
                if (value < 90) value = 180 + value;
            }

            return value;
        }
    }

    /// <summary>
    /// Longitude.
    /// </summary>
    public class Longitude : IEquatable<Longitude>, IComparable<Longitude>
    {
        private double globalValue = 0;

        /// <summary>
        /// Initializes a new instance of the Longitude class.
        /// </summary>
        public Longitude()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Longitude class.
        /// </summary>
        /// <param name="raw">The raw value.</param>
        public Longitude(double raw) => globalValue = AdaptValue(raw);

        /// <summary>
        /// Initializes a new instance of the Longitude class.
        /// </summary>
        /// <param name="zone">The zone type.</param>
        /// <param name="value">The absolute value.</param>
        public Longitude(Longitudes zone, double value)
        {
            value = AdaptValue(value);
            if (value < 0)
            {
                if (zone == Longitudes.East) zone = Longitudes.West;
                else if (zone == Longitudes.West) zone = Longitudes.East;
                value = -value;
            }

            if (zone == Longitudes.East) globalValue = value;
            else if (zone == Longitudes.PrimeMeridian) globalValue = 0;
            else if (zone == Longitudes.CalendarLine) globalValue = 180;
            else globalValue = -value;
        }

        /// <summary>
        /// Gets or sets the raw value.
        /// </summary>
        public double GlobalValue
        {
            get
            {
                return globalValue;
            }

            set
            {
                globalValue = AdaptValue(value);
            }
        }

        /// <summary>
        /// Gets or sets the zone type.
        /// </summary>
        public Longitudes Zone
        {
            get
            {
                if (GlobalValue > 0) return Longitudes.East;
                else if (GlobalValue == 0) return Longitudes.PrimeMeridian;
                else if (GlobalValue == 180 || GlobalValue == -180) return Longitudes.CalendarLine;
                else return Longitudes.West;
            }

            set
            {
                if (value == Longitudes.East) GlobalValue = Math.Abs(GlobalValue);
                else if (value == Longitudes.PrimeMeridian) GlobalValue = 0;
                else if (value == Longitudes.CalendarLine) GlobalValue = 180;
                else GlobalValue = -Math.Abs(GlobalValue);
            }
        }

        /// <summary>
        /// Gets or sets the absolute value in the zone.
        /// </summary>
        public double Value
        {
            get
            {
                return Math.Abs(GlobalValue);
            }

            set
            {
                if (GlobalValue < 0) GlobalValue = -value;
                else GlobalValue = value;
            }
        }
        /// <summary>
        /// Sets the longitude value.
        /// </summary>
        /// <param name="zone">The zone type.</param>
        /// <param name="value">The absolute value.</param>
        public void Set(Longitudes zone, double value)
        {
            value = AdaptValue(value);
            if (value < 0)
            {
                if (zone == Longitudes.East) zone = Longitudes.West;
                else if (zone == Longitudes.West) zone = Longitudes.East;
                value = -value;
            }

            if (zone == Longitudes.East) globalValue = value;
            else if (zone == Longitudes.PrimeMeridian) globalValue = 0;
            else if (zone == Longitudes.CalendarLine) globalValue = 180;
            else globalValue = -value;
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has the following meanings:
        /// Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.
        /// Zero This object is equal to <paramref name="other"/>.
        /// Greater than zero This object is greater than <paramref name="other"/>. 
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public int CompareTo(Longitude other)
        {
            if (other == null) return globalValue.CompareTo(null);
            return globalValue.CompareTo(other.GlobalValue);
        }

        /// <summary>
        /// Returns the angle string value of this instance.
        /// </summary>
        /// <returns>A System.String containing this angle.</returns>
        public override string ToString()
        {
            if (globalValue == 0 || globalValue == 180 || globalValue == -180) return globalValue + Maths.Angle.Symbols.DegreeUnit;
            return string.Format("{0}°{1}", Value, globalValue > 0 ? "E" : "W");
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return globalValue.GetHashCode();
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(Longitude other)
        {
            if (other == null) return false;
            return globalValue == other.GlobalValue;
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
            if (other is Longitude l) return globalValue == l.globalValue;
            if (other is double n1) return globalValue == n1;
            if (other is float n2) return globalValue == n2;
            if (other is int n3) return globalValue == n3;
            return false;
        }

        /// <summary>
        /// Converts a number to latitude.
        /// </summary>
        /// <param name="value">The raw value.</param>
        public static implicit operator Longitude(double value)
        {
            return new Longitude(value);
        }

        /// <summary>
        /// Converts a number to latitude.
        /// </summary>
        /// <param name="value">The raw value.</param>
        public static implicit operator Longitude(int value)
        {
            return new Longitude(value);
        }

        /// <summary>
        /// Compares two longitude to indicate if they are same.
        /// leftValue == rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>A result after subtration.</returns>
        public static bool operator ==(Longitude leftValue, Longitude rightValue)
        {
            if (leftValue == null && rightValue == null) return true;
            if (leftValue == null || rightValue == null) return false;
            return leftValue.globalValue == rightValue.globalValue;
        }

        /// <summary>
        /// Compares two longitude to indicate if they are different.
        /// leftValue != rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>A result after subtration.</returns>
        public static bool operator !=(Longitude leftValue, Longitude rightValue)
        {
            if (leftValue == null && rightValue == null) return false;
            if (leftValue == null || rightValue == null) return true;
            return leftValue.globalValue != rightValue.globalValue;
        }

        /// <summary>
        /// Compares if left is smaller than right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>A result after subtration.</returns>
        public static bool operator <(Longitude leftValue, Longitude rightValue)
        {
            if (leftValue == null && rightValue == null) return false;
            if (leftValue == null) return false;
            if (rightValue == null) return true;
            return leftValue.globalValue < rightValue.globalValue;
        }

        /// <summary>
        /// Compares if left is greater than right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>A result after subtration.</returns>
        public static bool operator >(Longitude leftValue, Longitude rightValue)
        {
            if (leftValue == null && rightValue == null) return false;
            if (leftValue == null) return true;
            if (rightValue == null) return false;
            return leftValue.globalValue < rightValue.globalValue;
        }

        /// <summary>
        /// Compares if left is smaller than or equals to right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>A result after subtration.</returns>
        public static bool operator <=(Longitude leftValue, Longitude rightValue)
        {
            if (leftValue == null && rightValue == null) return true;
            if (leftValue == null) return false;
            if (rightValue == null) return true;
            return leftValue.globalValue <= rightValue.globalValue;
        }

        /// <summary>
        /// Compares if left is greater than or equals to right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>A result after subtration.</returns>
        public static bool operator >=(Longitude leftValue, Longitude rightValue)
        {
            if (leftValue == null && rightValue == null) return true;
            if (leftValue == null) return true;
            if (rightValue == null) return false;
            return leftValue.globalValue <= rightValue.globalValue;
        }

        private double AdaptValue(double value)
        {
            if (value > 180)
            {
                if (value >= 360) value = value % 360;
                value = value - 360;
            }
            else if (value < -180)
            {
                if (value <= -360) value = -(-value % 360);
                value = value + 360;
            }

            return value;
        }
    }

    /// <summary>
    /// Geo location.
    /// </summary>
    public class Location
    {
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
            return string.Format("{0} {1}", Latitude, Longitude).GetHashCode();
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(Location other)
        {
            if (other == null) return false;
            return Latitude == other.Latitude && Longitude == other.Longitude;
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
            if (other is Location l) return Latitude == l.Latitude && Longitude == l.Longitude;
            return false;
        }
    }
}
