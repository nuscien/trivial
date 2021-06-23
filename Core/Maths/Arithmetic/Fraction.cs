using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.Maths
{
    /// <summary>
    /// Fraction.
    /// </summary>
    public struct Fraction : IEquatable<Fraction>, IEquatable<double>, IComparable<Fraction>, IComparable<double>
    {
        /// <summary>
        /// Initializes a new instance of the Fraction class.
        /// </summary>
        /// <param name="integer">The integer.</param>
        public Fraction(int integer)
        {
            IsNegative = integer < 0;
            IsPositive = integer > 0;
            LongNumerator = integer;
            LongDenominator = 1L;
            LongIntegerPart = integer;
            LongNumeratorOfDecimalPart = 0L;
        }

        /// <summary>
        /// Initializes a new instance of the Fraction class.
        /// </summary>
        /// <param name="integer">The integer.</param>
        public Fraction(long integer)
        {
            IsNegative = integer < 0;
            IsPositive = integer > 0;
            LongNumerator = integer;
            LongDenominator = 1L;
            LongIntegerPart = integer;
            LongNumeratorOfDecimalPart = 0L;
        }

        /// <summary>
        /// Initializes a new instance of the Fraction class.
        /// </summary>
        /// <param name="numerator">The numerator.</param>
        /// <param name="denominator">The denominator.</param>
        public Fraction(int numerator, int denominator)
        {
            IsNegative = (numerator < 0 && denominator >= 0) || (numerator > 0 && denominator < 0);
            IsPositive = (numerator > 0 && denominator >= 0) || (numerator < 0 && denominator < 0);
            var a = Math.Abs(numerator);
            var b = Math.Abs(denominator);
            var gcd = Arithmetic.Gcd(a, b);
            if (gcd > 1)
            {
                LongNumerator = a / gcd;
                LongDenominator = b / gcd;
            }
            else
            {
                LongNumerator = a;
                LongDenominator = b;
            }

            if (LongDenominator == 0)
            {
                LongIntegerPart = 0;
                LongNumeratorOfDecimalPart = LongNumerator;
            }
            else
            {
                var i = Math.DivRem(LongNumerator, LongDenominator, out var rem);
                LongIntegerPart = IsNegative ? -i : i;
                LongNumeratorOfDecimalPart = rem;
            }
        }

        /// <summary>
        /// Initializes a new instance of the Fraction class.
        /// </summary>
        /// <param name="numerator">The numerator.</param>
        /// <param name="denominator">The denominator.</param>
        public Fraction(long numerator, long denominator)
        {
            IsNegative = (numerator < 0 && denominator >= 0) || (numerator > 0 && denominator < 0);
            IsPositive = (numerator > 0 && denominator >= 0) || (numerator < 0 && denominator < 0);
            var a = Math.Abs(numerator);
            var b = Math.Abs(denominator);
            var gcd = Arithmetic.Gcd(a, b);
            if (gcd > 1)
            {
                LongNumerator = a / gcd;
                LongDenominator = b / gcd;
            }
            else
            {
                LongNumerator = a;
                LongDenominator = b;
            }

            if (LongDenominator == 0)
            {
                LongIntegerPart = 0;
                LongNumeratorOfDecimalPart = LongNumerator;
            }
            else
            {
                var i = Math.DivRem(LongNumerator, LongDenominator, out var rem);
                LongIntegerPart = IsNegative ? -i : i;
                LongNumeratorOfDecimalPart = rem;
            }
        }

        /// <summary>
        /// Gets the numerator.
        /// </summary>
        public long LongNumerator { get; }

        /// <summary>
        /// Gets the denominator.
        /// </summary>
        public long LongDenominator { get; }

        /// <summary>
        /// Gets the integer part.
        /// </summary>
        public long LongIntegerPart { get; }

        /// <summary>
        /// Gets the numerator in decimal part.
        /// </summary>
        public long LongNumeratorOfDecimalPart { get; }

        /// <summary>
        /// Gets the numerator.
        /// </summary>
        public int Numerator => (int)LongNumerator;

        /// <summary>
        /// Gets the denominator.
        /// </summary>
        public int Denominator => (int)LongDenominator;

        /// <summary>
        /// Gets the integer part.
        /// </summary>
        public int IntegerPart => (int)LongIntegerPart;

        /// <summary>
        /// Gets the numerator in decimal part.
        /// </summary>
        public int NumeratorOfDecimalPart => (int)LongNumeratorOfDecimalPart;

        /// <summary>
        /// Gets a value indicating whether it is a negative number.
        /// </summary>
        public bool IsNegative { get; }

        /// <summary>
        /// Gets a value indicating whether it is a positive number.
        /// </summary>
        public bool IsPositive { get; }

        /// <summary>
        /// Gets a value indicating whether the specified number is finite.
        /// </summary>
        public bool IsFinite => LongDenominator != 0;

        /// <summary>
        /// Gets a value indicating it is not a number (both numerator and denominator are zero).
        /// </summary>
        public bool IsNaN => LongDenominator == 0 && LongNumerator == 0;

        /// <summary>
        /// Gets a value indicating whether the specified number evaluates to negative or positive infinity.
        /// </summary>
        public bool IsInfinity => LongDenominator == 0 && LongNumerator != 0;

        /// <summary>
        /// Gets a value indicating whether the specified number evaluates to negative infinity.
        /// </summary>
        public bool IsNegativeInfinity => LongDenominator == 0 && LongNumerator < 0;

        /// <summary>
        /// Gets a value indicating whether the specified number evaluates to positive infinity.
        /// </summary>
        public bool IsPositiveInfinity => LongDenominator == 0 && LongNumerator > 0;

        /// <summary>
        /// Gets a value indicating whether it is a positive number.
        /// </summary>
        public bool IsZero => Numerator == 0;

        /// <summary>
        /// Gets a value indicating whether it is an integer.
        /// </summary>
        public bool IsInteger => LongDenominator == 1L;

        /// <summary>
        /// Returns the fraction string value of this instance.
        /// </summary>
        /// <returns>A System.String containing this fraction.</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            if (IsNegative) sb.Append('-');
            if (IsInfinity)
            {
                sb.Append(Numbers.InfiniteSymbol);  // Infinity
            }
            else
            {
                sb.Append(LongNumerator);
                sb.Append('/');
                sb.Append(LongDenominator);
            }

            return sb.ToString();
        }

        /// <inhericdoc />
        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (obj is Fraction f) return Equals(f);
            if (obj is double d) return (double)this == d;
            try
            {
                if (obj is decimal m) return (decimal)this == m;
                if (obj is float s) return (float)this == s;
                if (obj is int i) return LongDenominator == 1L && LongNumerator == (long)Math.Abs(i) && IsNegative == i < 0;
                if (obj is long l) return LongDenominator == 1L && LongNumerator == Math.Abs(l) && IsNegative == l < 0;
            }
            catch (InvalidCastException)
            {
            }
            catch (OverflowException)
            {
            }
            catch (InvalidOperationException)
            {
            }

            return false;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(Fraction other)
        {
            return LongNumerator == other.LongNumerator
                && LongDenominator == other.LongDenominator
                && IsPositive == other.IsPositive
                && IsNegative == other.IsNegative;
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
            return (double)this == other;
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This object is equal to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>. 
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public int CompareTo(Fraction other)
        {
            return ((double)this).CompareTo((double)other);
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
            return ((double)this).CompareTo(other);
        }

        /// <inhericdoc />
        public override int GetHashCode()
        {
            return ((double)this).GetHashCode();
        }

        /// <summary>
        /// Compares two angles to indicate if they are same.
        /// leftValue == rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if they are same; otherwise, false.</returns>
        public static bool operator ==(Fraction leftValue, Fraction rightValue)
        {
            return leftValue.LongNumerator == rightValue.LongNumerator
                && leftValue.LongDenominator == rightValue.LongDenominator
                && leftValue.IsPositive == rightValue.IsPositive
                && leftValue.IsNegative == rightValue.IsNegative;
        }

        /// <summary>
        /// Compares two angles to indicate if they are different.
        /// leftValue != rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if they are different; otherwise, false.</returns>
        public static bool operator !=(Fraction leftValue, Fraction rightValue)
        {
            return leftValue.LongNumerator != rightValue.LongNumerator
                || leftValue.LongDenominator != rightValue.LongDenominator
                || leftValue.IsPositive != rightValue.IsPositive
                || leftValue.IsNegative != rightValue.IsNegative;
        }

        /// <summary>
        /// Compares two angles to indicate if they are same.
        /// leftValue == rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if they are same; otherwise, false.</returns>
        public static bool operator ==(Fraction leftValue, double rightValue)
        {
            return (double)leftValue == rightValue;
        }

        /// <summary>
        /// Compares two angles to indicate if they are different.
        /// leftValue != rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if they are different; otherwise, false.</returns>
        public static bool operator !=(Fraction leftValue, double rightValue)
        {
            return (double)leftValue != rightValue;
        }

        /// <summary>
        /// Compares if left is smaller than right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is smaller than the right one; otherwise, false.</returns>
        public static bool operator <(Fraction leftValue, Fraction rightValue)
        {
            return (double)leftValue < (double)rightValue;
        }

        /// <summary>
        /// Compares if left is greater than right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is greater than the right one; otherwise, false.</returns>
        public static bool operator >(Fraction leftValue, Fraction rightValue)
        {
            return (double)leftValue > (double)rightValue;
        }

        /// <summary>
        /// Compares if left is smaller than right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is smaller than the right one; otherwise, false.</returns>
        public static bool operator <(Fraction leftValue, double rightValue)
        {
            return (double)leftValue < rightValue;
        }

        /// <summary>
        /// Compares if left is greater than right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is greater than the right one; otherwise, false.</returns>
        public static bool operator >(Fraction leftValue, double rightValue)
        {
            return (double)leftValue > rightValue;
        }

        /// <summary>
        /// Compares if left is smaller than or equals to right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is smaller than or equals to the right one; otherwise, false.</returns>
        public static bool operator <=(Fraction leftValue, Fraction rightValue)
        {
            if (leftValue.LongNumerator == rightValue.LongNumerator
                && leftValue.LongDenominator == rightValue.LongDenominator
                && leftValue.IsPositive == rightValue.IsPositive
                && leftValue.IsNegative == rightValue.IsNegative)
                return true;
            return (double)leftValue <= (double)rightValue;
        }

        /// <summary>
        /// Compares if left is greater than or equals to right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is greater than or equals to the right one; otherwise, false.</returns>
        public static bool operator >=(Fraction leftValue, Fraction rightValue)
        {
            if (leftValue.LongNumerator == rightValue.LongNumerator
                && leftValue.LongDenominator == rightValue.LongDenominator
                && leftValue.IsPositive == rightValue.IsPositive
                && leftValue.IsNegative == rightValue.IsNegative)
                return true;
            return (double)leftValue >= (double)rightValue;
        }

        /// <summary>
        /// Compares if left is smaller than or equals to right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is smaller than or equals to the right one; otherwise, false.</returns>
        public static bool operator <=(Fraction leftValue, double rightValue)
        {
            return (double)leftValue <= rightValue;
        }

        /// <summary>
        /// Compares if left is greater than or equals to right.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if the left one is greater than or equals to the right one; otherwise, false.</returns>
        public static bool operator >=(Fraction leftValue, double rightValue)
        {
            return (double)leftValue >= rightValue;
        }

        /// <summary>
        /// Converts to a floating number.
        /// </summary>
        /// <param name="value">The fraction value.</param>
        /// <returns>A floating number.</returns>
        public static explicit operator double(Fraction value)
        {
            return value.LongNumerator * (value.IsNegative ? - 1.0 : 1.0) / value.LongDenominator;
        }

        /// <summary>
        /// Converts to a floating number.
        /// </summary>
        /// <param name="value">The fraction value.</param>
        /// <returns>A floating number.</returns>
        public static explicit operator float(Fraction value)
        {
            return value.LongNumerator * (value.IsNegative ? -1f : 1f) / value.LongDenominator;
        }

        /// <summary>
        /// Converts to a floating number.
        /// </summary>
        /// <param name="value">The fraction value.</param>
        /// <returns>A floating number.</returns>
        public static explicit operator decimal(Fraction value)
        {
            return value.LongNumerator * (value.IsNegative ? -1m : 1m) / value.LongDenominator;
        }

        /// <summary>
        /// Converts to an integer.
        /// </summary>
        /// <param name="value">The fraction value.</param>
        /// <returns>A floating number.</returns>
        public static explicit operator long(Fraction value)
        {
            return (long)(value.LongNumerator * (value.IsNegative ? -1.0 : 1.0) / value.LongDenominator);
        }

        /// <summary>
        /// Converts to an integer.
        /// </summary>
        /// <param name="value">The fraction value.</param>
        /// <returns>A floating number.</returns>
        public static explicit operator int(Fraction value)
        {
            return (int)(value.LongNumerator * (value.IsNegative ? -1.0 : 1.0) / value.LongDenominator);
        }

        /// <summary>
        /// Converts to an integer.
        /// </summary>
        /// <param name="value">The fraction value.</param>
        /// <returns>A floating number.</returns>
        public static explicit operator short(Fraction value)
        {
            return (short)(value.LongNumerator * (value.IsNegative ? -1.0 : 1.0) / value.LongDenominator);
        }

        /// <summary>
        /// Converts to a floating number.
        /// </summary>
        /// <param name="value">The fraction value.</param>
        /// <returns>A floating number.</returns>
        public static explicit operator Text.JsonDouble(Fraction value)
        {
            return new(value.LongNumerator * (value.IsNegative ? -1.0 : 1.0) / value.LongDenominator);
        }

        /// <summary>
        /// Converts to a floating number.
        /// </summary>
        /// <param name="value">The fraction value.</param>
        /// <returns>A floating number.</returns>
        public static explicit operator Text.JsonString(Fraction value)
        {
            return new(value.ToString());
        }

        /// <summary>
        /// Converts to a floating number.
        /// </summary>
        /// <param name="value">The fraction value.</param>
        /// <returns>A floating number.</returns>
        public static explicit operator System.Text.Json.Nodes.JsonValue(Fraction value)
        {
            return System.Text.Json.Nodes.JsonValue.Create(value.LongNumerator * (value.IsNegative ? -1.0 : 1.0) / value.LongDenominator);
        }

        /// <summary>
        /// Converts to a floating number.
        /// </summary>
        /// <param name="value">The fraction value.</param>
        /// <returns>A floating number.</returns>
        public static explicit operator System.Text.Json.Nodes.JsonNode(Fraction value)
        {
            return System.Text.Json.Nodes.JsonValue.Create(value.LongNumerator * (value.IsNegative ? -1.0 : 1.0) / value.LongDenominator);
        }

        /// <summary>
        /// Pluses fractions.
        /// leftValue + rightValue
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after addition.</returns>
        public static Fraction operator +(Fraction leftValue, Fraction rightValue)
        {
            var lcm = Arithmetic.Lcm(rightValue.LongDenominator, leftValue.LongDenominator);
            var left = lcm / leftValue.LongDenominator * leftValue.LongNumerator;
            var right = lcm / rightValue.LongDenominator * rightValue.LongNumerator;
            return new Fraction(left + right, lcm);
        }

        /// <summary>
        /// Pluses fractions.
        /// leftValue + rightValue
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after addition.</returns>
        public static Fraction operator +(Fraction leftValue, int rightValue)
        {
            return new Fraction(rightValue * leftValue.LongDenominator + leftValue.LongNumerator, leftValue.LongDenominator);
        }

        /// <summary>
        /// Pluses fractions.
        /// leftValue + rightValue
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after addition.</returns>
        public static Fraction operator +(int leftValue, Fraction rightValue)
        {
            return new Fraction(leftValue * rightValue.LongDenominator + rightValue.LongNumerator, rightValue.LongDenominator);
        }

        /// <summary>
        /// Pluses fractions.
        /// leftValue + rightValue
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after addition.</returns>
        public static Fraction operator +(Fraction leftValue, long rightValue)
        {
            return new Fraction(rightValue * leftValue.LongDenominator + leftValue.LongNumerator, leftValue.LongDenominator);
        }

        /// <summary>
        /// Pluses fractions.
        /// leftValue + rightValue
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after addition.</returns>
        public static Fraction operator +(long leftValue, Fraction rightValue)
        {
            return new Fraction(leftValue * rightValue.LongDenominator + rightValue.LongNumerator, rightValue.LongDenominator);
        }

        /// <summary>
        /// Subtracts fractions.
        /// leftValue - rightValue
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after Subtraction.</returns>
        public static Fraction operator -(Fraction leftValue, Fraction rightValue)
        {
            var lcm = Arithmetic.Lcm(rightValue.LongDenominator, leftValue.LongDenominator);
            var left = lcm / leftValue.LongDenominator * leftValue.LongNumerator;
            var right = lcm / rightValue.LongDenominator * rightValue.LongNumerator;
            return new Fraction(left - right, lcm);
        }

        /// <summary>
        /// Subtracts fractions.
        /// leftValue - rightValue
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after Subtraction.</returns>
        public static Fraction operator -(Fraction leftValue, int rightValue)
        {
            return new Fraction(-rightValue * leftValue.LongDenominator + leftValue.LongNumerator, leftValue.LongDenominator);
        }

        /// <summary>
        /// Subtracts fractions.
        /// leftValue - rightValue
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after Subtraction.</returns>
        public static Fraction operator -(int leftValue, Fraction rightValue)
        {
            return new Fraction(leftValue * rightValue.LongDenominator - rightValue.LongNumerator, rightValue.LongDenominator);
        }

        /// <summary>
        /// Subtracts fractions.
        /// leftValue - rightValue
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after Subtraction.</returns>
        public static Fraction operator -(Fraction leftValue, long rightValue)
        {
            return new Fraction(-rightValue * leftValue.LongDenominator + leftValue.LongNumerator, leftValue.LongDenominator);
        }

        /// <summary>
        /// Subtracts fractions.
        /// leftValue - rightValue
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after Subtraction.</returns>
        public static Fraction operator -(long leftValue, Fraction rightValue)
        {
            return new Fraction(leftValue * rightValue.LongDenominator + rightValue.LongNumerator, rightValue.LongDenominator);
        }

        /// <summary>
        /// Negates the current value.
        /// -this
        /// </summary>
        /// <param name="value">The value to negate.</param>
        /// <returns>A result after negation.</returns>
        public static Fraction operator -(Fraction value)
        {
            return new Fraction(-value.LongNumerator, value.LongDenominator);
        }

        /// <summary>
        /// Multiplies fractions.
        /// leftValue * rightValue
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after multiply.</returns>
        public static Fraction operator *(Fraction leftValue, Fraction rightValue)
        {
            return new Fraction(leftValue.LongNumerator * rightValue.LongNumerator, leftValue.LongDenominator * rightValue.LongDenominator);
        }

        /// <summary>
        /// Multiplies fractions.
        /// leftValue * rightValue
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after multiply.</returns>
        public static Fraction operator *(Fraction leftValue, int rightValue)
        {
            return new Fraction(leftValue.LongNumerator * rightValue, leftValue.LongDenominator);
        }

        /// <summary>
        /// Multiplies fractions.
        /// leftValue * rightValue
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after multiply.</returns>
        public static Fraction operator *(int leftValue, Fraction rightValue)
        {
            return new Fraction(leftValue * rightValue.LongNumerator, rightValue.LongDenominator);
        }

        /// <summary>
        /// Multiplies fractions.
        /// leftValue * rightValue
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after multiply.</returns>
        public static Fraction operator *(Fraction leftValue, long rightValue)
        {
            return new Fraction(leftValue.LongNumerator * rightValue, leftValue.LongDenominator);
        }

        /// <summary>
        /// Multiplies fractions.
        /// leftValue * rightValue
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after multiply.</returns>
        public static Fraction operator *(long leftValue, Fraction rightValue)
        {
            return new Fraction(leftValue * rightValue.LongNumerator, rightValue.LongDenominator);
        }

        /// <summary>
        /// Multiplies fractions.
        /// leftValue * rightValue
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after multiply.</returns>
        public static Fraction operator /(Fraction leftValue, Fraction rightValue)
        {
            return new Fraction(leftValue.LongNumerator * rightValue.LongDenominator, leftValue.LongNumerator * rightValue.LongDenominator);
        }

        /// <summary>
        /// Multiplies fractions.
        /// leftValue * rightValue
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after multiply.</returns>
        public static Fraction operator /(Fraction leftValue, int rightValue)
        {
            return new Fraction(leftValue.LongNumerator, leftValue.LongDenominator * rightValue);
        }

        /// <summary>
        /// Multiplies fractions.
        /// leftValue * rightValue
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after multiply.</returns>
        public static Fraction operator /(int leftValue, Fraction rightValue)
        {
            return new Fraction(leftValue * rightValue.LongDenominator, rightValue.LongNumerator);
        }

        /// <summary>
        /// Multiplies fractions.
        /// leftValue * rightValue
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after multiply.</returns>
        public static Fraction operator /(Fraction leftValue, long rightValue)
        {
            return new Fraction(leftValue.LongNumerator, leftValue.LongDenominator * rightValue);
        }

        /// <summary>
        /// Multiplies fractions.
        /// leftValue * rightValue
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after multiply.</returns>
        public static Fraction operator /(long leftValue, Fraction rightValue)
        {
            return new Fraction(leftValue * rightValue.LongDenominator, rightValue.LongNumerator);
        }
    }
}
