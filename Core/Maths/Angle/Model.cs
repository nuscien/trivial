// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Angle\Model.cs" company="Nanchang Jinchen Software Co., Ltd.">
//   Copyright (c) 2010 Nanchang Jinchen Software Co., Ltd. All rights reserved.
// </copyright>
// <summary>
//   The model of angle.
// </summary>
// <author>Kingcean Tuan</author>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Trivial.Maths
{
    /// <summary>
    /// The struct of degree (angle).
    /// </summary>
    public partial struct Angle
    {
        /// <summary>
        /// The angle model.
        /// </summary>
        public class Model : IAngle, ICloneable, IComparable, IComparable<IAngle>, IEquatable<IAngle>, IComparable<double>, IEquatable<double>, IComparable<int>, IEquatable<int>, IAdvancedAdditionCapable<Model>
        {
            /// <summary>
            /// The total degrees value.
            /// </summary>
            private double? raw;

            /// <summary>
            /// The degree value.
            /// </summary>
            private int degree;

            /// <summary>
            /// The minute value.
            /// </summary>
            private int minute;

            /// <summary>
            /// The second value.
            /// </summary>
            private float second;

            /// <summary>
            /// Initializes a new instance of the Angle.Model class.
            /// </summary>
            public Model()
            {
            }

            /// <summary>
            /// Initializes a new instance of the Angle.Model class.
            /// </summary>
            /// <param name="boundary">The boundary options.</param>
            public Model(BoundaryOptions boundary) : this(0, boundary)
            {
            }

            /// <summary>
            /// Initializes a new instance of the Angle.Model class.
            /// </summary>
            /// <param name="degrees">The total degrees.</param>
            /// <param name="boundary">The boundary options.</param>
            public Model(double degrees, BoundaryOptions boundary = null)
            {
                Boundary = boundary;
                SetDegrees(degrees);
            }

            /// <summary>
            /// Initializes a new instance of the Angle.Model class.
            /// </summary>
            /// <param name="degree">The degree part.</param>
            /// <param name="minute">The minute part.</param>
            /// <param name="second">The second part.</param>
            public Model(int degree, int minute, float second = 0)
            {
                AdaptValue(null, ref degree, ref minute, ref second);
            }

            /// <summary>
            /// Initializes a new instance of the Angle.Model class.
            /// </summary>
            /// <param name="degree">The degree part.</param>
            /// <param name="minute">The minute part.</param>
            /// <param name="second">The second part.</param>
            /// <param name="boundary">The boundary options.</param>
            public Model(int degree, int minute, float second, BoundaryOptions boundary)
            {
                Boundary = boundary;
                AdaptValue(boundary, ref degree, ref minute, ref second);
            }

            /// <summary>
            /// Gets the boundary options.
            /// </summary>
            public BoundaryOptions Boundary { get; protected set; }

            /// <summary>
            /// Gets or sets a value indicating whether it is positive.
            /// </summary>
            public bool Positive
            {
                get
                {
                    return degree > 0;
                }

                set
                {
                    if (degree > 0 && value) return;
                    if (degree < 0 && !value) return;
                    degree = -degree;
                    var r = raw;
                    if (r.HasValue) raw = -r.Value;
                }
            }

            /// <summary>
            /// Gets or sets the absolute degree of the angle.
            /// </summary>
            public int AbsDegree => Math.Abs((int)raw);

            /// <summary>
            /// Gets or sets the degree of the angle.
            /// </summary>
            public int Degree
            {
                get
                {
                    return degree;
                }

                set
                {
                    if (degree == value) return;
                    degree = value;
                    var r = raw;
                    var m = 0;
                    var s = 0f;
                    AdaptValue(Boundary, ref degree, ref m, ref s);
                    AdaptValue(Boundary, ref degree, ref minute, ref second);
                    if (!r.HasValue) return;
                    var abs = Math.Abs(r.Value);
                    raw = abs - (int)abs + degree;
                }
            }

            /// <summary>
            /// Gets or sets the arcminute of the angle.
            /// </summary>
            public int Arcminute
            {
                get
                {
                    return minute;
                }

                set
                {
                    if (minute == value) return;
                    minute = value;
                    raw = null;
                    AdaptValue(Boundary, ref degree, ref minute, ref second);
                }
            }

            /// <summary>
            /// Gets or sets the arcsecond of the angle.
            /// </summary>
            public float Arcsecond
            {
                get
                {
                    return second;
                }

                set
                {
                    if (second == value) return;
                    second = value;
                    raw = null;
                    AdaptValue(Boundary, ref degree, ref minute, ref second);
                }
            }

            /// <summary>
            /// Gets the absolute total degrees with float format.
            /// </summary>
            public double AbsDegrees => Math.Abs(Degrees);

            /// <summary>
            /// Gets or sets the total degrees.
            /// </summary>
            public double Degrees
            {
                get
                {
                    var r = raw;
                    if (r.HasValue) return r.Value;
                    var pos = degree >= 0;
                    var rest = Math.Abs(degree) + minute / 60d + second / 3600d;
                    rest = pos ? rest : -rest;
                    raw = rest;
                    return rest;
                }

                set
                {
                    SetDegrees(value);
                }
            }

            /// <summary>
            /// Converts to radians value.
            /// </summary>
            /// <returns>A double number for the angle.</returns>
            public double Radians
            {
                get { return Degrees / 180 * Math.PI; }
                set { Degrees = (float)(value / Math.PI * 180); }
            }

            /// <summary>
            /// Converts to radians value which has not muliplied PI.
            /// </summary>
            /// <returns>A double number for the angle before mulipling PI.</returns>
            public double RadiansWithoutPi
            {
                get { return Degrees / 180; }
                set { Degrees = value * 180; }
            }

            /// <summary>
            /// Converts to turns value.
            /// </summary>
            /// <returns>A single float about turns.</returns>
            public double Turns
            {
                get { return Degrees / 360; }
                set { Degrees = value * 360; }
            }

            /// <summary>
            /// Converts to grads value.
            /// </summary>
            /// <returns>A single float about grads.</returns>
            public double Grads
            {
                get { return Degrees / 0.9; }
                set { Degrees = (float)(value * 0.9); }
            }

            /// <summary>
            /// Gets a value indicating whether the angle is a zero degree angle.
            /// </summary>
            public bool IsZero => raw == 0;

            /// <summary>
            /// Gets a value indicating whether the angle is a negative angle.
            /// </summary>
            public bool IsNegative => raw < 0;

            /// <summary>
            /// Converts a number to angle model.
            /// </summary>
            /// <param name="value">The raw value.</param>
            public static implicit operator Model(double value)
            {
                return new Model(value);
            }

            /// <summary>
            /// Converts a number to angle model.
            /// </summary>
            /// <param name="value">The raw value.</param>
            public static implicit operator Model(int value)
            {
                return new Model(value);
            }

            /// <summary>
            /// Converts an angle to angle model.
            /// </summary>
            /// <param name="value">The raw value.</param>
            public static implicit operator Model(Angle value)
            {
                return new Model(value.Degrees);
            }

            /// <summary>
            /// Negates a specific angle.
            /// </summary>
            /// <param name="value">A value to create mirror.</param>
            /// <returns>A result mirrored with the specific angle.</returns>
            public static Model operator -(Model value)
            {
                if (value is null) return null;
                return new Model(-value.Degrees);
            }

            /// <summary>
            /// Pluses two angles.
            /// leftValue + rightValue
            /// </summary>
            /// <param name="leftValue">The left value for addition operator.</param>
            /// <param name="rightValue">The right value for addition operator.</param>
            /// <returns>A result after addition.</returns>
            public static Model operator +(Model leftValue, IAngle rightValue)
            {
                if (leftValue is null) leftValue = new Model();
                if (rightValue is null) rightValue = new Model();
                return new Model(leftValue.Degrees + rightValue.Degrees);
            }

            /// <summary>
            /// Minuses two angles.
            /// leftValue - rightValue
            /// </summary>
            /// <param name="leftValue">The left value for subtration operator.</param>
            /// <param name="rightValue">The right value for subtration operator.</param>
            /// <returns>A result after subtration.</returns>
            public static Model operator -(Model leftValue, IAngle rightValue)
            {
                if (leftValue is null) leftValue = new Model();
                if (rightValue is null) rightValue = new Model();
                return new Model(leftValue.Degrees - rightValue.Degrees);
            }

            /// <summary>
            /// Multiplies.
            /// leftValue * rightValue
            /// </summary>
            /// <param name="leftValue">The left value.</param>
            /// <param name="rightValue">The right value.</param>
            /// <returns>A result after addition.</returns>
            public static Model operator *(Model leftValue, int rightValue)
            {
                if (leftValue is null) return null;
                return new Model(leftValue.Degrees * rightValue);
            }

            /// <summary>
            /// Multiplies.
            /// leftValue * rightValue
            /// </summary>
            /// <param name="leftValue">The left value.</param>
            /// <param name="rightValue">The right value.</param>
            /// <returns>A result after addition.</returns>
            public static Model operator *(Model leftValue, long rightValue)
            {
                if (leftValue is null) return null;
                return new Model(leftValue.Degrees * rightValue);
            }

            /// <summary>
            /// Multiplies.
            /// leftValue * rightValue
            /// </summary>
            /// <param name="leftValue">The left value.</param>
            /// <param name="rightValue">The right value.</param>
            /// <returns>A result after addition.</returns>
            public static Model operator *(Model leftValue, double rightValue)
            {
                if (leftValue is null) return null;
                return new Model(leftValue.Degrees * rightValue);
            }

            /// <summary>
            /// Divides.
            /// leftValue / rightValue
            /// </summary>
            /// <param name="leftValue">The left value.</param>
            /// <param name="rightValue">The right value.</param>
            /// <returns>A result after addition.</returns>
            public static Model operator /(Model leftValue, int rightValue)
            {
                if (leftValue is null) return null;
                return new Model(leftValue.Degrees / rightValue);
            }

            /// <summary>
            /// Divides.
            /// leftValue / rightValue
            /// </summary>
            /// <param name="leftValue">The left value.</param>
            /// <param name="rightValue">The right value.</param>
            /// <returns>A result after addition.</returns>
            public static Model operator /(Model leftValue, long rightValue)
            {
                if (leftValue is null) return null;
                return new Model(leftValue.Degrees / rightValue);
            }

            /// <summary>
            /// Divides.
            /// leftValue / rightValue
            /// </summary>
            /// <param name="leftValue">The left value.</param>
            /// <param name="rightValue">The right value.</param>
            /// <returns>A result after addition.</returns>
            public static Model operator /(Model leftValue, double rightValue)
            {
                if (leftValue is null) return null;
                return new Model(leftValue.Degrees / rightValue);
            }

            /// <summary>
            /// Compares two angles to indicate if they are same.
            /// leftValue == rightValue
            /// </summary>
            /// <param name="leftValue">The left value to compare.</param>
            /// <param name="rightValue">The right value to compare.</param>
            /// <returns>A result after subtration.</returns>
            public static bool operator ==(Model leftValue, IAngle rightValue)
            {
                if (ReferenceEquals(leftValue, rightValue)) return true;
                if (leftValue is null || rightValue is null) return false;
                return leftValue.Degrees == rightValue.Degrees;
            }

            /// <summary>
            /// Compares two angles to indicate if they are different.
            /// leftValue != rightValue
            /// </summary>
            /// <param name="leftValue">The left value to compare.</param>
            /// <param name="rightValue">The right value to compare.</param>
            /// <returns>A result after subtration.</returns>
            public static bool operator !=(Model leftValue, IAngle rightValue)
            {
                if (ReferenceEquals(leftValue, rightValue)) return false;
                if (leftValue is null || rightValue is null) return true;
                return leftValue.Degrees != rightValue.Degrees;
            }

            /// <summary>
            /// Compares if left is smaller than right.
            /// </summary>
            /// <param name="leftValue">The left value to compare.</param>
            /// <param name="rightValue">The right value to compare.</param>
            /// <returns>A result after subtration.</returns>
            public static bool operator <(Model leftValue, IAngle rightValue)
            {
                if (ReferenceEquals(leftValue, rightValue)) return false;
                if (leftValue is null) return false;
                if (rightValue is null) return true;
                return leftValue.Degrees < rightValue.Degrees;
            }

            /// <summary>
            /// Compares if left is greater than right.
            /// </summary>
            /// <param name="leftValue">The left value to compare.</param>
            /// <param name="rightValue">The right value to compare.</param>
            /// <returns>A result after subtration.</returns>
            public static bool operator >(Model leftValue, IAngle rightValue)
            {
                if (ReferenceEquals(leftValue, rightValue)) return false;
                if (leftValue is null) return true;
                if (rightValue is null) return false;
                return leftValue.Degrees < rightValue.Degrees;
            }

            /// <summary>
            /// Compares if left is smaller than or equals to right.
            /// </summary>
            /// <param name="leftValue">The left value to compare.</param>
            /// <param name="rightValue">The right value to compare.</param>
            /// <returns>A result after subtration.</returns>
            public static bool operator <=(Model leftValue, IAngle rightValue)
            {
                if (ReferenceEquals(leftValue, rightValue)) return true;
                if (leftValue is null) return false;
                if (rightValue is null) return true;
                return leftValue.Degrees <= rightValue.Degrees;
            }

            /// <summary>
            /// Compares if left is greater than or equals to right.
            /// </summary>
            /// <param name="leftValue">The left value to compare.</param>
            /// <param name="rightValue">The right value to compare.</param>
            /// <returns>A result after subtration.</returns>
            public static bool operator >=(Model leftValue, IAngle rightValue)
            {
                if (ReferenceEquals(leftValue, rightValue)) return true;
                if (leftValue is null) return true;
                if (rightValue is null) return false;
                return leftValue.Degrees <= rightValue.Degrees;
            }

            /// <summary>
            /// Creates a new instance of Angle with the same value as the value of this instance.
            /// </summary>
            /// <param name="obj">The Angle to copy.</param>
            /// <returns>A new Angle object with the same value as this instance.</returns>
            public static Model Copy(Model obj)
            {
                return obj?.Clone();
            }

            /// <summary>
            /// Converts the string representation of a angle in a specified style and culture-specific format to its angle equivalent.
            /// </summary>
            /// <param name="s">A string containing a angle to convert.</param>
            /// <returns>A angle equivalent to the angle specified in s.</returns>
            public static Model Parse(string s)
            {
                var split = s.Split(';');
                if (split.Length < 2)
                {
                    split = s.Split(',');
                }

                if (split.Length < 2)
                {
                    split = s.Split(new[] { Symbols.DegreeUnit, Symbols.ArcminuteUnit, Symbols.ArcsecondUnit }, StringSplitOptions.RemoveEmptyEntries);
                }

                if (split.Length < 2)
                {
                    var degrees = float.Parse(s.Replace(";", string.Empty).Replace(",", string.Empty).Replace(Symbols.DegreeUnit, string.Empty).Replace(Symbols.ArcminuteUnit, string.Empty).Replace(Symbols.ArcsecondUnit, string.Empty));
                    return new Model(degrees);
                }

                var positive = true;
                if (split[0].IndexOf("-") >= 0)
                {
                    positive = false;
                    split[0] = split[0].Replace("-", string.Empty);
                }

                return new Model((positive ? 1 : -1) * int.Parse(split[0]), int.Parse(split[1]), split.Length > 2 ? float.Parse(split[2]) : 0);
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
                if (other is IAngle a) return Equals(a);
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
            public virtual bool Equals(IAngle other)
            {
                return Degrees == other.Degrees;
            }

            /// <summary>
            /// Indicates whether the current object is equal to another object of the same type.
            /// </summary>
            /// <returns>
            /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
            /// </returns>
            /// <param name="other">An object to compare with this object.</param>
            public virtual bool Equals(double other)
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
            public virtual bool Equals(int other)
            {
                return Degrees.Equals(other);
            }

            /// <summary>
            /// Clones an object.
            /// </summary>
            /// <returns>The object copied from this instance.</returns>
            public virtual Model Clone()
            {
                return new Model(Degree, Arcminute, Arcsecond, Boundary)
                {
                    raw = raw
                };
            }

            /// <summary>
            /// Clones an object.
            /// </summary>
            /// <returns>The object copied from this instance.</returns>
            object ICloneable.Clone()
            {
                return Clone();
            }

            /// <summary>
            /// Pluses another value.
            /// this + value
            /// </summary>
            /// <param name="value">A given value to be added.</param>
            /// <returns>A result after addition.</returns>
            public Model Plus(Model value)
            {
                return new Model(Degrees + value.Degrees);
            }

            /// <summary>
            /// Minuses another value.
            /// this - value
            /// </summary>
            /// <param name="value">A given value to be added.</param>
            /// <returns>A result after subtraction.</returns>
            public Model Minus(Model value)
            {
                return new Model(Degrees - value.Degrees);
            }

            /// <summary>
            /// Negates the current value.
            /// -this
            /// </summary>
            /// <returns>A result after negation.</returns>
            public Model Negate()
            {
                return new Model(-Degrees);
            }

            /// <summary>
            /// Gets the absolute value.
            /// Math.Abs(this)
            /// </summary>
            /// <returns>A absolute result.</returns>
            public Model Abs()
            {
                return new Model(Math.Abs(Degrees));
            }

            /// <summary>
            /// Gets a unit element for addition and subtraction.
            /// 0
            /// </summary>
            /// <returns>An element zero for the value.</returns>
            public Model GetElementZero()
            {
                return new Model();
            }

            /// <summary>
            /// Returns the angle string value of this instance.
            /// </summary>
            /// <returns>A System.String containing this angle.</returns>
            public override string ToString()
            {
                return (Positive ? string.Empty : "-") + this.ToAbsAngleString();
            }

            /// <summary>
            /// Sets a total degrees.
            /// </summary>
            /// <param name="degrees">The total degrees.</param>
            private void SetDegrees(double degrees)
            {
                degree = (int)degrees;
                var d = degree;
                var boundary = Boundary;
                var rest = (Math.Abs(degrees) - Math.Abs(degree)) * 60;
                var m = minute = (int)rest;
                var s = second = (float)((rest - minute) * 60);
                AdaptValue(boundary, ref degree, ref minute, ref second);
                if (minute != m || second != s) return;
                if (degree == d)
                {
                    raw = degrees;
                    return;
                }

                var abs = Math.Abs(degrees);
                rest = abs - (int)abs;
                raw = degree + (degree >= 0 ? rest : -rest);
            }
        }
    }
}
