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
    public partial struct Angle
    {
        /// <summary>
        /// The angle.
        /// </summary>
        public class Model : IAngle, IComparable, IComparable<IAngle>, IEquatable<IAngle>, IComparable<double>, IEquatable<double>, IComparable<int>, IEquatable<int>, IAdvancedAdditionCapable<Model>
        {
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
            /// Initializes a new instance of the Angle.Model class.
            /// </summary>
            public Model()
            {
            }

            /// <summary>
            /// Initializes a new instance of the Angle.Model class.
            /// </summary>
            /// <param name="boundary">The boundary options.</param>
            public Model(BoundaryOptions boundary)
            {
                Boundary = boundary;
            }

            /// <summary>
            /// Gets the boundary options.
            /// </summary>
            public virtual BoundaryOptions Boundary { get; }

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
            /// Gets or sets the absolute degree of the angle.
            /// </summary>
            public int AbsDegree => _degree;

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
                    if (Boundary != null && Boundary.MaxDegree > 0)
                    {
                        switch (Boundary.RectifyMode)
                        {
                            case RectifyModes.Bounce:
                                {
                                    if (!Boundary.Negative)
                                    {
                                        value = value % (Boundary.MaxDegree * 2);
                                        value = -value;
                                    }
                                    else
                                    {
                                        value = (value + Boundary.MaxDegree) % (Boundary.MaxDegree * 4) - Boundary.MaxDegree;
                                        if (value > Boundary.MaxDegree)
                                        {
                                            value = Boundary.MaxDegree * 2 - value;
                                        }
                                        else if (value == Boundary.MaxDegree && _arcmin > 0 && _arcsec > 0)
                                        {
                                            value = Boundary.MaxDegree - 1;
                                            _arcmin = 60 - _arcmin - (_arcsec > 0 ? 1 : 0);
                                            _arcsec = 60 - _arcsec;
                                        }
                                        else if (value < -Boundary.MaxDegree)
                                        {
                                            value = Boundary.MaxDegree * 2 + value;
                                        }
                                        else if (value == -Boundary.MaxDegree && _arcmin > 0 && _arcsec > 0)
                                        {
                                            value = Boundary.MaxDegree + 1;
                                            _arcmin = 60 - _arcmin - (_arcsec > 0 ? 1 : 0);
                                            _arcsec = 60 - _arcsec;
                                        }
                                    }

                                    break;
                                }
                            case RectifyModes.Cycle:
                                {
                                    if (!Boundary.Negative)
                                    {
                                        value = value % Boundary.MaxDegree;
                                        if (value < 0) value += Boundary.MaxDegree;
                                    }
                                    else
                                    {
                                        value = (value + Boundary.MaxDegree) % (Boundary.MaxDegree * 2) - Boundary.MaxDegree;
                                    }

                                    break;
                                }
                            case RectifyModes.None:
                                {
                                    if (value > Boundary.MaxDegree)
                                        throw new ArgumentOutOfRangeException(nameof(Degree), string.Format("Cannot be greater than {0} degrees.", Boundary.MaxDegree));
                                    if (Boundary.Negative && value < -Boundary.MaxDegree)
                                        throw new ArgumentOutOfRangeException(nameof(Degree), string.Format("Cannot be less than -{0} degrees.", Boundary.MaxDegree));
                                    if (!Boundary.Negative && value < 0)
                                        throw new ArgumentOutOfRangeException(nameof(Degree), "Cannot be less than 0 degree.");
                                    break;
                                }
                        }
                    }

                    _degree = Math.Abs(value);
                    Positive = value >= 0;
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
                    var remainder = value % 60;
                    var d = (value - remainder) / 60;
                    if (remainder < 0)
                    {
                        d--;
                        remainder += 60;
                    }

                    _arcmin = remainder;
                    if (d != 0) Degree += d;
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
            /// Gets the absolute total degrees with float format.
            /// </summary>
            public double AbsDegrees => _degree + Arcminute / 60.0 + Arcsecond / 3600.0;

            /// <summary>
            /// Gets or sets the total degrees.
            /// </summary>
            public double Degrees
            {
                get
                {
                    return (_degree + Arcminute / 60.0 + Arcsecond / 3600.0) * (Positive ? 1 : -1);
                }

                set
                {
                    var degree = Math.Abs((int)value);
                    var restValue = (float)Math.Abs(Math.Abs(value) - degree) * 60;
                    Arcminute = (int)restValue;
                    Arcsecond = (restValue - Arcminute) * 60;
                    Degree = degree * (value >= 0 ? 1 : -1);
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
            public bool IsZero => Arcsecond.Equals(0) && Arcminute == 0 && Degree == 0;

            /// <summary>
            /// Gets a value indicating whether the angle is a negative angle.
            /// </summary>
            public bool IsNegative => _negative;

            /// <summary>
            /// Converts a number to latitude.
            /// </summary>
            /// <param name="value">The raw value.</param>
            public static implicit operator Model(double value)
            {
                return new Model { Degrees = value };
            }

            /// <summary>
            /// Converts a number to latitude.
            /// </summary>
            /// <param name="value">The raw value.</param>
            public static implicit operator Model(int value)
            {
                return new Model { Degree = value };
            }

            /// <summary>
            /// Negates a specific angle.
            /// </summary>
            /// <param name="value">A value to create mirror.</param>
            /// <returns>A result mirrored with the specific angle.</returns>
            public static Model operator -(Model value)
            {
                return new Model
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
            public static Model operator +(Model leftValue, IAngle rightValue)
            {
                return new Model
                {
                    Degrees = leftValue.Degrees + rightValue.Degrees
                };
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
                return new Model
                {
                    Degrees = leftValue.Degrees - rightValue.Degrees
                };
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
            public static bool operator !=(Model leftValue, IAngle rightValue)
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
            public static bool operator <(Model leftValue, IAngle rightValue)
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
            public static bool operator >(Model leftValue, IAngle rightValue)
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
            public static bool operator <=(Model leftValue, IAngle rightValue)
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
            public static bool operator >=(Model leftValue, IAngle rightValue)
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
            public static Model Copy(Model obj)
            {
                return new Model { Degree = obj.Degree, Arcminute = obj.Arcminute, Arcsecond = obj.Arcsecond };
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
                    return new Model { Degrees = degrees };
                }

                var positive = true;
                if (split[0].IndexOf("-") >= 0)
                {
                    positive = false;
                    split[0] = split[0].Replace("-", string.Empty);
                }

                var resultObj = new Model { Degree = int.Parse(split[0]), Arcminute = int.Parse(split[1]), Positive = positive };
                if (split.Length > 2) resultObj.Arcsecond = float.Parse(split[2]);
                return resultObj;
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
                if (other is IAngle a) return Equals(a);
                if (other is double d) return Degrees.Equals(d);
                if (other is int i) return Degrees.Equals(i);
                if (other is long l) return Degrees.Equals(l);
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
                return (Degree == other.Degree) && (Arcminute == other.Arcminute) && Arcsecond.Equals(other.Arcsecond);
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
            /// Pluses another value.
            /// this + value
            /// </summary>
            /// <param name="value">A given value to be added.</param>
            /// <returns>A result after addition.</returns>
            public Model Plus(Model value)
            {
                return new Model
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
            public Model Minus(Model value)
            {
                return new Model
                {
                    Degrees = Degrees - value.Degrees,
                };
            }

            /// <summary>
            /// Negates the current value.
            /// -this
            /// </summary>
            /// <returns>A result after negation.</returns>
            public Model Negate()
            {
                return new Model
                {
                    Degrees = -Degrees
                };
            }

            /// <summary>
            /// Gets the absolute value.
            /// Math.Abs(this)
            /// </summary>
            /// <returns>A absolute result.</returns>
            public Model Abs()
            {
                return new Model
                {
                    Degrees = Math.Abs(Degrees)
                };
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
        }
    }
}
