// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Angle.cs" company="Nanchang Jinchen Software Co., Ltd.">
//   Copyright (c) 2010 Nanchang Jinchen Software Co., Ltd. All rights reserved.
// </copyright>
// <summary>
//   The struct of angle.
// </summary>
// <author>Kingcean Tuan</author>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Trivial.Maths
{
    /// <summary>
    /// The struct of degree (angle).
    /// </summary>
    public partial struct Angle : IAngle, IComparable, IComparable<IAngle>, IEquatable<IAngle>, IComparable<double>, IEquatable<double>, IComparable<int>, IEquatable<int>, IAdvancedAdditionCapable<Angle>
    {
        /// <summary>
        /// The error string during value is out of argument.
        /// </summary>
        private const string ErrStr = "{0} should be less than 60, and greater than or equals 0.";

        /// <summary>
        /// Initializes a new instance of the Angle struct.
        /// </summary>
        public Angle(double degrees)
        {
            Degrees = degrees;
            AbsDegrees = Math.Abs(Degrees);
            Positive = Degrees > 0;
            IsNegative = Degrees < 0;
            Degree = (int)Degrees;
            AbsDegree = Math.Abs(Degree);
            var restValue = Math.Abs(AbsDegrees - AbsDegree) * 60;
            Arcminute = (int)restValue;
            Arcsecond = (float)(restValue - Arcminute) * 60;
        }

        /// <summary>
        /// Initializes a new instance of the Angle struct.
        /// </summary>
        public Angle(int degree, int minute, float second) : this(GetDegrees(degree, minute, second))
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether it is positive.
        /// </summary>
        public bool Positive { get; }

        /// <summary>
        /// Gets or sets the absolute degree of the angle.
        /// </summary>
        public int AbsDegree { get; }

        /// <summary>
        /// Gets or sets the degree of the angle.
        /// </summary>
        public int Degree { get; }

        /// <summary>
        /// Gets or sets the arcminute of the angle.
        /// </summary>
        public int Arcminute { get; }

        /// <summary>
        /// Gets or sets the arcsecond of the angle.
        /// </summary>
        public float Arcsecond { get; }

        /// <summary>
        /// Gets the absolute total degrees with float format.
        /// </summary>
        public double AbsDegrees { get; }

        /// <summary>
        /// Gets or sets the total degrees.
        /// </summary>
        public double Degrees { get; }

        /// <summary>
        /// Converts to radians value.
        /// </summary>
        /// <returns>A double number for the angle.</returns>
        public double Radians => Degrees / 180 * Math.PI;

        /// <summary>
        /// Converts to radians value which has not muliplied PI.
        /// </summary>
        /// <returns>A double number for the angle before mulipling PI.</returns>
        public double RadiansWithoutPi => Degrees / 180;

        /// <summary>
        /// Converts to turns value.
        /// </summary>
        /// <returns>A single float about turns.</returns>
        public double Turns => Degrees / 360;

        /// <summary>
        /// Converts to grads value.
        /// </summary>
        /// <returns>A single float about grads.</returns>
        public double Grads => Degrees / 0.9;

        /// <summary>
        /// Gets a value indicating whether the angle is a zero degree angle.
        /// </summary>
        public bool IsZero => Arcsecond.Equals(0) && Arcminute == 0 && Degree == 0;

        /// <summary>
        /// Gets a value indicating whether the angle is a negative angle.
        /// </summary>
        public bool IsNegative { get; }

        /// <summary>
        /// Converts a number to angle.
        /// </summary>
        /// <param name="value">The raw value.</param>
        public static implicit operator Angle(double value)
        {
            return new Angle(value);
        }

        /// <summary>
        /// Converts a number to angle.
        /// </summary>
        /// <param name="value">The raw value.</param>
        public static implicit operator Angle(int value)
        {
            return new Angle(value);
        }

        /// <summary>
        /// Converts an angel model to angle.
        /// </summary>
        /// <param name="value">The raw value.</param>
        public static implicit operator Angle(Model value)
        {
            if (value == null) return ZeroDegree;
            return new Angle(value.Degrees);
        }

        /// <summary>
        /// Negates a specific angle.
        /// </summary>
        /// <param name="value">A value to create mirror.</param>
        /// <returns>A result mirrored with the specific angle.</returns>
        public static Angle operator -(Angle value)
        {
            return new Angle(-value.Degrees);
        }

        /// <summary>
        /// Pluses two angles.
        /// leftValue + rightValue
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after addition.</returns>
        public static Angle operator +(Angle leftValue, IAngle rightValue)
        {
            return new Angle(leftValue.Degrees + rightValue.Degrees);
        }

        /// <summary>
        /// Minuses two angles.
        /// leftValue - rightValue
        /// </summary>
        /// <param name="leftValue">The left value for subtration operator.</param>
        /// <param name="rightValue">The right value for subtration operator.</param>
        /// <returns>A result after subtration.</returns>
        public static Angle operator -(Angle leftValue, IAngle rightValue)
        {
            return new Angle(leftValue.Degrees - rightValue.Degrees);
        }

        /// <summary>
        /// Compares two angles to indicate if they are same.
        /// leftValue == rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>A result after subtration.</returns>
        public static bool operator ==(Angle leftValue, IAngle rightValue)
        {
            if (ReferenceEquals(leftValue, rightValue)) return true;
            if (rightValue is null) return false;
            return leftValue.Degrees == rightValue.Degrees;
        }

        /// <summary>
        /// Compares two angles to indicate if they are different.
        /// leftValue != rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>A result after subtration.</returns>
        public static bool operator !=(Angle leftValue, IAngle rightValue)
        {
            if (ReferenceEquals(leftValue, rightValue)) return false;
            if (rightValue is null) return true;
            return leftValue.Degrees != rightValue.Degrees;
        }

        /// <summary>
        /// Compares if left is smaller than right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>A result after subtration.</returns>
        public static bool operator <(Angle leftValue, IAngle rightValue)
        {
            if (ReferenceEquals(leftValue, rightValue)) return false;
            if (rightValue is null) return true;
            return leftValue.Degrees < rightValue.Degrees;
        }

        /// <summary>
        /// Compares if left is greater than right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>A result after subtration.</returns>
        public static bool operator >(Angle leftValue, IAngle rightValue)
        {
            if (ReferenceEquals(leftValue, rightValue)) return false;
            if (rightValue is null) return false;
            return leftValue.Degrees < rightValue.Degrees;
        }

        /// <summary>
        /// Compares if left is smaller than or equals to right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>A result after subtration.</returns>
        public static bool operator <=(Angle leftValue, IAngle rightValue)
        {
            if (ReferenceEquals(leftValue, rightValue)) return true;
            if (rightValue is null) return true;
            return leftValue.Degrees <= rightValue.Degrees;
        }

        /// <summary>
        /// Compares if left is greater than or equals to right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>A result after subtration.</returns>
        public static bool operator >=(Angle leftValue, IAngle rightValue)
        {
            if (ReferenceEquals(leftValue, rightValue)) return true;
            if (rightValue is null) return false;
            return leftValue.Degrees <= rightValue.Degrees;
        }

        /// <summary>
        /// Creates a new instance of Angle with the same value as the value of this instance.
        /// </summary>
        /// <param name="obj">The Angle to copy.</param>
        /// <returns>A new Angle object with the same value as this instance.</returns>
        public static Angle Copy(Angle obj)
        {
            return new Angle(obj.Degree, obj.Arcminute, obj.Arcsecond);
        }

        /// <summary>
        /// Computes the total degrees.
        /// </summary>
        /// <param name="degree">The degree part.</param>
        /// <param name="minute">The arcminute part.</param>
        /// <param name="second">The arcsecond part.</param>
        /// <returns>The total degrees of the angle.</returns>
        public static double GetDegrees(int degree, int minute, float second) => (degree > 0 ? 1 : -1) * (Math.Abs(degree) + minute / 60.0 + second / 3600.0);

        /// <summary>
        /// Converts the string representation of a angle in a specified style and culture-specific format to its angle equivalent.
        /// </summary>
        /// <param name="s">A string containing a angle to convert.</param>
        /// <returns>A angle equivalent to the angle specified in s.</returns>
        public static Angle Parse(string s)
        {
            var split = s.Split(';');
            if (split.Length < 2)
            {
                split = s.Split(',');
            }

            if (split.Length < 2)
            {
                split = s.Split(new[] {Symbols.DegreeUnit, Symbols.ArcminuteUnit, Symbols.ArcsecondUnit }, StringSplitOptions.RemoveEmptyEntries);
            }

            if (split.Length < 2)
            {
                var degrees = float.Parse(s.Replace(";", string.Empty).Replace(",", string.Empty).Replace(Symbols.DegreeUnit, string.Empty).Replace(Symbols.ArcminuteUnit, string.Empty).Replace(Symbols.ArcsecondUnit, string.Empty));
                return new Angle(degrees);
            }

            var positive = true;
            if (split[0].IndexOf("-") >= 0)
            {
                positive = false;
                split[0] = split[0].Replace("-", string.Empty);
            }

            return new Angle(int.Parse(split[0]) * (positive ? 1 : -1), int.Parse(split[1]), split.Length > 2 ? float.Parse(split[2]) : 0);
        }

        /// <summary>
        /// Returns a new angle that adds the specified number of degrees to the value of this instance.
        /// </summary>
        /// <param name="value">A number of whole and fractional degrees. The value parameter can be negative or positive.</param>
        public Angle AddDegrees(int value)
        {
            return new Angle(Degree + value, Arcminute, Arcsecond);
        }

        /// <summary>
        /// Returns a new angle that adds the specified number of arcminutes to the value of this instance.
        /// </summary>
        /// <param name="value">A number of whole and fractional arcminutes. The value parameter can be negative or positive.</param>
        public Angle AddArcminutes(int value)
        {
            return new Angle(Degree, Arcminute + value, Arcsecond);
        }

        /// <summary>
        /// Returns a new angle that adds the specified number of arcseconds to the value of this instance.
        /// </summary>
        /// <param name="value">A number of whole and fractional arcseconds. The value parameter can be negative or positive.</param>
        public Angle AddArcseconds(float value)
        {
            return new Angle(Degree, Arcminute, Arcsecond + value);
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This object is equal to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>. 
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public int CompareTo(object other)
        {
            return Degrees.CompareTo(other);
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
        public int CompareTo(IAngle other)
        {
            if (other is null) return Degrees.CompareTo(null);
            return Degrees.CompareTo(other.Degrees);
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This object is equal to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>. 
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public int CompareTo(double other)
        {
            return Degrees.CompareTo(other);
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This object is equal to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>. 
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public int CompareTo(int other)
        {
            return Degrees.CompareTo(other);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return Degrees.GetHashCode();
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
            if (other is double d) return Degrees.Equals(d);
            return Degrees.Equals(other);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(IAngle other)
        {
            return (Degree == other.Degree) && (Arcminute == other.Arcminute) && Arcsecond.Equals(other.Arcsecond);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(double other)
        {
            return Degrees.Equals(other);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(int other)
        {
            return Degrees.Equals(other);
        }

        /// <summary>
        /// Pluses another value.
        /// this + value
        /// </summary>
        /// <param name="value">A given value to be added.</param>
        /// <returns>A result after addition.</returns>
        public Angle Plus(Angle value)
        {
            return new Angle(Degrees + value.Degrees);
        }

        /// <summary>
        /// Minuses another value.
        /// this - value
        /// </summary>
        /// <param name="value">A given value to be added.</param>
        /// <returns>A result after subtraction.</returns>
        public Angle Minus(Angle value)
        {
            return new Angle(Degrees - value.Degrees);
        }

        /// <summary>
        /// Negates the current value.
        /// -this
        /// </summary>
        /// <returns>A result after negation.</returns>
        public Angle Negate()
        {
            return new Angle(-Degrees);
        }

        /// <summary>
        /// Gets the absolute value.
        /// Math.Abs(this)
        /// </summary>
        /// <returns>A absolute result.</returns>
        public Angle Abs()
        {
            return new Angle(AbsDegrees);
        }

        /// <summary>
        /// Gets a unit element for addition and subtraction.
        /// 0
        /// </summary>
        /// <returns>An element zero for the value.</returns>
        public Angle GetElementZero()
        {
            return ZeroDegree;
        }

        /// <summary>
        /// Returns the angle string value of this instance.
        /// </summary>
        /// <returns>A System.String containing this angle.</returns>
        public override string ToString()
        {
            return (Positive ? string.Empty : "-") + this.ToAbsAngleString();
        }
    }
}
