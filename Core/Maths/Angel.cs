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
    public class Angle : IComparable, IComparable<Angle>, IEquatable<Angle>, IComparable<double>, IEquatable<double>, IComparable<int>, IEquatable<int>, IAdvancedAdditionCapable<Angle>
    {
        /// <summary>
        /// The error string during value is out of argument.
        /// </summary>
        private const string ErrStr = "{0} should be less than 60, and greater than or equals 0.";

        /// <summary>
        /// Angle symbols.
        /// </summary>
        public static class Symbols
        {
            /// <summary>
            /// The unit of degree.
            /// </summary>
            public const string DegreeUnit = "°";

            /// <summary>
            /// The unit of arcminute.
            /// </summary>
            public const string ArcminuteUnit = @"'";

            /// <summary>
            /// The unit of arcsecond.
            /// </summary>
            public const string ArcsecondUnit = @"""";

            /// <summary>
            /// The sign of angle.
            /// </summary>
            public const string Sign = "∠";

            /// <summary>
            /// The sign of right angle.
            /// </summary>
            public const string RightAngleSign = "∟";

            /// <summary>
            /// The sign of radian.
            /// </summary>
            public const string RadianSign = "⌒";

            /// <summary>
            /// The sign of circle center.
            /// </summary>
            public const string CircleCenterSign = "⊙";

            /// <summary>
            /// The sign of triangle.
            /// </summary>
            public const string TriangleSign = "∆";

            /// <summary>
            /// The sign of right-angled triangle.
            /// </summary>
            public const string RightAngledTriangleSign = "⊿";
        }

        /// <summary>
        /// The degree value.
        /// </summary>
        private int _degree;

        /// <summary>
        /// The arcminute value.
        /// </summary>
        private int _arcmin;

        /// <summary>
        /// The arcsecond value.
        /// </summary>
        private float _arcsec;

        /// <summary>
        /// A value indicating whether it is negative.
        /// </summary>
        private bool _negative;

        /// <summary>
        /// Gets an angle with 0 degree.
        /// </summary>
        public static Angle ZeroDegree { get { return new Angle { Degree = 0 }; } }

        /// <summary>
        /// Gets an angle with 1 degree.
        /// </summary>
        public static Angle OneDegree { get { return new Angle { Degree = 1 }; } }

        /// <summary>
        /// Gets the right angle.
        /// </summary>
        public static Angle Right { get { return new Angle { Degree = 90 }; } }

        /// <summary>
        /// Gets the straight angle.
        /// </summary>
        public static Angle Straight { get { return new Angle { Degree = 180 }; } }

        /// <summary>
        /// Gets the full angle.
        /// </summary>
        public static Angle Full { get { return new Angle { Degree = 380 }; } }

        /// <summary>
        /// Initializes a new instance of the Angle class.
        /// </summary>
        public Angle()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Angle class.
        /// </summary>
        public Angle(double degrees)
        {
            Degrees = degrees;
        }

        /// <summary>
        /// Initializes a new instance of the Angle class.
        /// </summary>
        public Angle(int degree, int minute, float second)
        {
            Degree = degree;
            Arcminute = minute;
            Arcsecond = second;
        }

        /// <summary>
        /// Gets or sets a value indicating whether it is positive.
        /// </summary>
        public bool Positive
        {
            get
            {
                return !_negative;
            }

            set
            {
                _negative = !value;
            }
        }

        /// <summary>
        /// Gets or sets the degree of the angle.
        /// </summary>
        public int Degree
        {
            get
            {
                return _degree * (Positive ? 1 : -1);
            }

            set
            {
                _degree = Math.Abs(value);
                Positive = (value >= 0);
            }
        }

        /// <summary>
        /// Gets or sets the arcminute of the angle.
        /// </summary>
        public int Arcminute
        {
            get
            {
                return _arcmin;
            }
            
            set
            {
                if (value >= 60 || value < 0)
                {
                    var remainder = value % 60;
                    var d = (value - remainder) / 60;
                    if (remainder < 0)
                    {
                        d--;
                        value += 60;
                    }

                    value = remainder;
                    Degree += d;
                }

                _arcmin = value;
            }
        }

        /// <summary>
        /// Gets or sets the arcsecond of the angle.
        /// </summary>
        public float Arcsecond
        {
            get
            {
                return _arcsec;
            }

            set
            {
                if (value >= 60 || value < 0) throw new ArgumentOutOfRangeException("value", string.Format(ErrStr, "value"));
                _arcsec = value;
            }
        }

        /// <summary>
        /// Gets or sets to degrees with float format.
        /// </summary>
        /// <returns>A single float for the angle.</returns>
        public double Degrees
        {
            get
            {
                return (_degree + Arcminute / 60.0 + Arcsecond / 3600.0) * (Positive ? 1 : -1);
            }

            set
            {
                Degree = Math.Abs((int)value);
                Positive = value >= 0;
                var restValue = (float)Math.Abs(value - Degree) * 60;
                Arcminute = (int)restValue;
                Arcsecond = (restValue - Arcminute) * 60;
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
            get { return Degrees/180; }
            set { Degrees = value*180; }
        }

        /// <summary>
        /// Converts to turns value.
        /// </summary>
        /// <returns>A single float about turns.</returns>
        public double Turns
        {
            get { return Degrees/360; }
            set { Degrees = value*360; }
        }

        /// <summary>
        /// Converts to grads value.
        /// </summary>
        /// <returns>A single float about grads.</returns>
        public double Grads
        {
            get { return Degrees/0.9; }
            set { Degrees = (float)(value * 0.9); }
        }

        /// <summary>
        /// Gets a value indicating whether the angle is an acute angle.
        /// </summary>
        public bool IsAcute
        {
            get { return Degree < 90 && Degree > 0; }
        }

        /// <summary>
        /// Gets a value indicating whether the angle is a right angle.
        /// </summary>
        public bool IsRight
        {
            get { return Arcsecond.Equals(0) && Arcminute == 0 && Degree == 90; }
        }

        /// <summary>
        /// Gets a value indicating whether the angle is an oblique angle.
        /// </summary>
        public bool IsOblique
        {
            get { return !Arcsecond.Equals(0) || Arcminute != 0 || Degree%90 != 0; }
        }

        /// <summary>
        /// Gets a value indicating whether the angle is an obtuse angle.
        /// </summary>
        public bool IsObtuse
        {
            get { return Degree < 180 && Degree > 90; }
        }

        /// <summary>
        /// Gets a value indicating whether the angle is a reflex angle.
        /// </summary>
        public bool IsReflex
        {
            get { return Degree < 360 && Degree > 180; }
        }

        /// <summary>
        /// Gets a value indicating whether the angle is a negative angle.
        /// </summary>
        public bool IsNegative
        {
            get { return Degree < 0; }
        }

        /// <summary>
        /// Gets a value indicating whether the angle is a zero degree angle.
        /// </summary>
        public bool IsZero
        {
            get { return Arcsecond.Equals(0) && Arcminute == 0 && Degree == 0; }
        }

        /// <summary>
        /// Gets a value indicating whether the angle is less than 360 degrees and not less than 0 degree.
        /// </summary>
        public bool LessThanRound
        {
            get { return Degree < 360 && Degree > 0; }
        }

        /// <summary>
        /// Converts a number to latitude.
        /// </summary>
        /// <param name="value">The raw value.</param>
        public static implicit operator Angle(double value)
        {
            return new Angle(value);
        }

        /// <summary>
        /// Converts a number to latitude.
        /// </summary>
        /// <param name="value">The raw value.</param>
        public static implicit operator Angle(int value)
        {
            return new Angle(value);
        }

        /// <summary>
        /// Negates a specific angle.
        /// </summary>
        /// <param name="value">A value to create mirror.</param>
        /// <returns>A result mirrored with the specific angle.</returns>
        public static Angle operator -(Angle value)
        {
            return new Angle
            {
                Degrees = -value.Degrees
            };
        }

        /// <summary>
        /// Pluses two angles.
        /// leftValue + rightValue
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after addition.</returns>
        public static Angle operator +(Angle leftValue, Angle rightValue)
        {
            return new Angle
            {
                Degrees = leftValue.Degrees + rightValue.Degrees,
            };
        }

        /// <summary>
        /// Minuses two angles.
        /// leftValue - rightValue
        /// </summary>
        /// <param name="leftValue">The left value for subtration operator.</param>
        /// <param name="rightValue">The right value for subtration operator.</param>
        /// <returns>A result after subtration.</returns>
        public static Angle operator -(Angle leftValue, Angle rightValue)
        {
            return new Angle
            {
                Degrees = leftValue.Degrees - rightValue.Degrees,
            };
        }

        /// <summary>
        /// Compares two angles to indicate if they are same.
        /// leftValue == rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>A result after subtration.</returns>
        public static bool operator ==(Angle leftValue, Angle rightValue)
        {
            if (leftValue == null && rightValue == null) return true;
            if (leftValue == null || rightValue == null) return false;
            return leftValue.Degrees == rightValue.Degrees;
        }

        /// <summary>
        /// Compares two angles to indicate if they are different.
        /// leftValue != rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>A result after subtration.</returns>
        public static bool operator !=(Angle leftValue, Angle rightValue)
        {
            if (leftValue == null && rightValue == null) return false;
            if (leftValue == null || rightValue == null) return true;
            return leftValue.Degrees != rightValue.Degrees;
        }

        /// <summary>
        /// Compares if left is smaller than right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>A result after subtration.</returns>
        public static bool operator <(Angle leftValue, Angle rightValue)
        {
            if (leftValue == null && rightValue == null) return false;
            if (leftValue == null) return false;
            if (rightValue == null) return true;
            return leftValue.Degrees < rightValue.Degrees;
        }

        /// <summary>
        /// Compares if left is greater than right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>A result after subtration.</returns>
        public static bool operator >(Angle leftValue, Angle rightValue)
        {
            if (leftValue == null && rightValue == null) return false;
            if (leftValue == null) return true;
            if (rightValue == null) return false;
            return leftValue.Degrees < rightValue.Degrees;
        }

        /// <summary>
        /// Compares if left is smaller than or equals to right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>A result after subtration.</returns>
        public static bool operator <=(Angle leftValue, Angle rightValue)
        {
            if (leftValue == null && rightValue == null) return true;
            if (leftValue == null) return false;
            if (rightValue == null) return true;
            return leftValue.Degrees <= rightValue.Degrees;
        }

        /// <summary>
        /// Compares if left is greater than or equals to right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>A result after subtration.</returns>
        public static bool operator >=(Angle leftValue, Angle rightValue)
        {
            if (leftValue == null && rightValue == null) return true;
            if (leftValue == null) return true;
            if (rightValue == null) return false;
            return leftValue.Degrees <= rightValue.Degrees;
        }

        /// <summary>
        /// Creates a new instance of Angle with the same value as the value of this instance.
        /// </summary>
        /// <param name="obj">The Angle to copy.</param>
        /// <returns>A new Angle object with the same value as this instance.</returns>
        public static Angle Copy(Angle obj)
        {
            return new Angle { Degree = obj.Degree, Arcminute = obj.Arcminute, Arcsecond = obj.Arcsecond };
        }

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
                return new Angle { Degrees = degrees };
            }

            var positive = true;
            if (split[0].IndexOf("-") >= 0)
            {
                positive = false;
                split[0] = split[0].Replace("-", string.Empty);
            }

            var resultObj = new Angle { Degree = int.Parse(split[0]), Arcminute = int.Parse(split[1]), Positive = positive };
            if (split.Length > 2) resultObj.Arcsecond = float.Parse(split[2]);
            return resultObj;
        }

        /// <summary>
        /// Returns a new angle that adds the specified number of degrees to the value of this instance.
        /// </summary>
        /// <param name="value">A number of whole and fractional degrees. The value parameter can be negative or positive.</param>
        public Angle AddDegrees(int value)
        {
            var nuInst = new Angle { Degrees = value };
            return this + nuInst;
        }

        /// <summary>
        /// Returns a new angle that adds the specified number of arcminutes to the value of this instance.
        /// </summary>
        /// <param name="value">A number of whole and fractional arcminutes. The value parameter can be negative or positive.</param>
        public Angle AddArcminutes(int value)
        {
            var nuInst = new Angle { Degrees = value/60.0 };
            return this + nuInst;
        }

        /// <summary>
        /// Returns a new angle that adds the specified number of arcseconds to the value of this instance.
        /// </summary>
        /// <param name="value">A number of whole and fractional arcseconds. The value parameter can be negative or positive.</param>
        public Angle AddArcseconds(float value)
        {
            var nuInst = new Angle { Degrees = value / 3600.0 };
            return this + nuInst;
        }

        /// <summary>
        /// Returns the sine of this angle.
        /// </summary>
        /// <returns>The sine of this angle. If this angle is equal to System.Double.NaN, System.Double.NegativeInfinity, or System.Double.PositiveInfinity, this method returns System.Double.NaN.</returns>
        public double Sin()
        {
            return Math.Sin(Degrees);
        }

        /// <summary>
        /// Returns the cosine of this angle.
        /// </summary>
        /// <returns>The cosine of this angle. If this angle is equal to System.Double.NaN, System.Double.NegativeInfinity, or System.Double.PositiveInfinity, this method returns System.Double.NaN.</returns>
        public double Cos()
        {
            return Math.Cos(Degrees);
        }

        /// <summary>
        /// Returns the tangent of this angle.
        /// </summary>
        /// <returns>The tangent of this angle. If this angle is equal to System.Double.NaN, System.Double.NegativeInfinity, or System.Double.PositiveInfinity, this method returns System.Double.NaN.</returns>
        public double Tan()
        {
            return Math.Tan(Degrees);
        }

        /// <summary>
        /// Returns the cotangent of this angle.
        /// </summary>
        /// <returns>The cotangent of this angle. If this angle is equal to System.Double.NaN, System.Double.NegativeInfinity, or System.Double.PositiveInfinity, this method returns System.Double.NaN.</returns>
        public double Cot()
        {
            return 1 / Math.Tan(Degrees);
        }

        /// <summary>
        /// Returns the secant of this angle.
        /// </summary>
        /// <returns>The secant of this angle. If this angle is equal to System.Double.NaN, System.Double.NegativeInfinity, or System.Double.PositiveInfinity, this method returns System.Double.NaN.</returns>
        public double Sec()
        {
            return 1/ Math.Cos(Degrees);
        }

        /// <summary>
        /// Returns the cosecant of this angle.
        /// </summary>
        /// <returns>The cosecant of this angle. If this angle is equal to System.Double.NaN, System.Double.NegativeInfinity, or System.Double.PositiveInfinity, this method returns System.Double.NaN.</returns>
        public double Csc()
        {
            return 1 / Math.Sin(Degrees);
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
        public int CompareTo(Angle other)
        {
            if (other == null) return Degrees.CompareTo(null);
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
            if (other == null) return false;
            if (other is Angle) return Degrees.Equals((Angle) other);
            return Degrees.Equals(other);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(Angle other)
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
            return new Angle
            {
                Degrees = Degrees + value.Degrees,
            };
        }

        /// <summary>
        /// Minuses another value.
        /// this - value
        /// </summary>
        /// <param name="value">A given value to be added.</param>
        /// <returns>A result after subtraction.</returns>
        public Angle Minus(Angle value)
        {
            return new Angle
            {
                Degrees = Degrees - value.Degrees,
            };
        }

        /// <summary>
        /// Negates the current value.
        /// -this
        /// </summary>
        /// <returns>A result after negation.</returns>
        public Angle Negate()
        {
            return new Angle
            {
                Degrees = -Degrees
            };
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
            if (Arcsecond == 0)
            {
                if (Arcminute == 0)
                {
                    return string.Format("{0}{1}{2}", Positive ? string.Empty : "-", _degree, Symbols.DegreeUnit);
                }

                return string.Format("{0}{1}{2}{3}{4}", Positive ? string.Empty : "-", _degree, Symbols.DegreeUnit, Arcminute, Symbols.ArcminuteUnit);
            }

            return string.Format("{0}{1}{2}{3}{4}{5}{6}", Positive ? string.Empty : "-", _degree, Symbols.DegreeUnit, Arcminute, Symbols.ArcminuteUnit, Arcsecond, Symbols.ArcsecondUnit);
        }
    }
}
