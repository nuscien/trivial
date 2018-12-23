// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Coordinate.cs" company="Nanchang Jinchen Software Co., Ltd.">
//   Copyright (c) 2010 Nanchang Jinchen Software Co., Ltd. All rights reserved.
// </copyright>
// <summary>
//   The classes of coordinates and points.
// </summary>
// <author>Kingcean Tuan</author>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Trivial.Maths
{
    /// <summary>
    /// The generic 1D (line) coordinate point.
    /// </summary>
    /// <typeparam name="TUnit">The type of unit.</typeparam>
    public class OneDimensionalPoint<TUnit> : SingleElement<TUnit>, IEquatable<SingleElement<TUnit>> where TUnit : struct, IComparable<TUnit>, IEquatable<TUnit>
    {
        /// <summary>
        /// Initializes a new instance of the OneDimensionalPoint class.
        /// </summary>
        /// <remarks>You can use this to initialize an instance for the class.</remarks>
        public OneDimensionalPoint()
        {
        }

        /// <summary>
        /// Initializes a new instance of the OneDimensionalPoint class.
        /// </summary>
        /// <param name="x">The value of X.</param>
        /// <remarks>You can use this to initialize an instance for the class.</remarks>
        public OneDimensionalPoint(TUnit x)
        {
            X = x;
        }

        /// <summary>
        /// Gets or sets the value of X (horizontal position). The value is same as ItemA.
        /// </summary>
        public TUnit X
        {
            get => base.ItemA;
            set => base.ItemA = value;
        }

        private new TUnit ItemA
        {
            get => base.ItemA;
            set => base.ItemA = value;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public override object Clone()
        {
            return new OneDimensionalPoint<TUnit>(X);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns> true if the current object is equal to the other parameter; otherwise, false.</returns>
        public bool Equals(SingleElement<TUnit> other)
        {
            return other != null && X.Equals(other.ItemA);
        }

        /// <summary>
        /// Returns the point string value of this instance.
        /// </summary>
        /// <returns>A System.String containing this point.</returns>
        public override string ToString()
        {
            return X.ToString();
        }
    }

    /// <summary>
    /// The point of 1D (line) mathematics coordinate.
    /// </summary>
    public class DoubleOneDimensionalPoint
        : OneDimensionalPoint<double>,
        IAdvancedAdditionCapable<DoubleOneDimensionalPoint>,
        IComparable<OneDimensionalPoint<double>>,
        IComparable<OneDimensionalPoint<int>>,
        IComparable<double>,
        IComparable<int>
    {
        /// <summary>
        /// Initializes a new instance of the DoubleOneDimensionalPoint class.
        /// </summary>
        /// <remarks>You can use this to initialize an instance for the class.</remarks>
        public DoubleOneDimensionalPoint()
        {
        }

        /// <summary>
        /// Initializes a new instance of the DoubleOneDimensionalPoint class.
        /// </summary>
        /// <param name="x">The value of X.</param>
        /// <remarks>You can use this to initialize an instance for the class.</remarks>
        public DoubleOneDimensionalPoint(double x) : base(x)
        {
        }

        /// <summary>
        /// Gets a value indicating whether the current element is negative.
        /// true if it is positve or zero; otherwise, false.
        /// </summary>
        public bool IsNegative => X < 0;

        /// <summary>
        /// Gets a value indicating whether the current element is a unit element of zero.
        /// </summary>
        public bool IsZero => X == 0;

        /// <summary>
        /// Pluses another value to return. Current value will not be changed.
        /// this + value
        /// </summary>
        /// <param name="value">The value to be plused.</param>
        /// <returns>A result after leftValue plus rightValue.</returns>
        public DoubleOneDimensionalPoint Plus(OneDimensionalPoint<double> value)
        {
            return value != null
                ? new DoubleOneDimensionalPoint(X + value.X)
                : new DoubleOneDimensionalPoint(X);
        }

        /// <summary>
        /// Pluses another value to return. Current value will not be changed.
        /// this + value
        /// </summary>
        /// <param name="value">The value to be plused.</param>
        /// <returns>A result after leftValue plus rightValue.</returns>
        public DoubleOneDimensionalPoint Plus(OneDimensionalPoint<int> value)
        {
            return value != null
                ? new DoubleOneDimensionalPoint(X + value.X)
                : new DoubleOneDimensionalPoint(X);
        }

        /// <summary>
        /// Pluses another value to return. Current value will not be changed.
        /// this + value
        /// </summary>
        /// <param name="value">The value to be plused.</param>
        /// <returns>A result after leftValue plus rightValue.</returns>
        public DoubleOneDimensionalPoint Plus(DoubleOneDimensionalPoint value)
        {
            return value != null
                ? new DoubleOneDimensionalPoint(X + value.X)
                : new DoubleOneDimensionalPoint(X);
        }

        /// <summary>
        /// Minuses another value to return. Current value will not be changed.
        /// this - value
        /// </summary>
        /// <param name="value">The value to be minuses.</param>
        /// <returns>A result after leftValue minus rightValue.</returns>
        public DoubleOneDimensionalPoint Minus(OneDimensionalPoint<double> value)
        {
            return value != null
                ? new DoubleOneDimensionalPoint(X - value.X)
                : new DoubleOneDimensionalPoint(X);
        }

        /// <summary>
        /// Minuses another value to return. Current value will not be changed.
        /// this - value
        /// </summary>
        /// <param name="value">The value to be minuses.</param>
        /// <returns>A result after leftValue minus rightValue.</returns>
        public DoubleOneDimensionalPoint Minus(OneDimensionalPoint<int> value)
        {
            return value != null
                ? new DoubleOneDimensionalPoint(X - value.X)
                : new DoubleOneDimensionalPoint(X);
        }

        /// <summary>
        /// Minuses another value to return. Current value will not be changed.
        /// this - value
        /// </summary>
        /// <param name="value">The value to be minuses.</param>
        /// <returns>A result after leftValue minus rightValue.</returns>
        public DoubleOneDimensionalPoint Minus(DoubleOneDimensionalPoint value)
        {
            return value != null
                ? new DoubleOneDimensionalPoint(X - value.X)
                : new DoubleOneDimensionalPoint(X);
        }

        /// <summary>
        /// Negates the current value to return. Current value will not be changed.
        /// -this
        /// </summary>
        /// <returns>A result after negation.</returns>
        public DoubleOneDimensionalPoint Negate()
        {
            return new DoubleOneDimensionalPoint(-X);
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public override object Clone()
        {
            return new DoubleOneDimensionalPoint(X);
        }

        /// <summary>
        /// Gets a unit element for addition and subtraction.
        /// 0
        /// </summary>
        /// <returns>An element zero for the value.</returns>
        public DoubleOneDimensionalPoint GetElementZero()
        {
            return new DoubleOneDimensionalPoint(0);
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
        public int CompareTo(OneDimensionalPoint<double> other)
        {
            if (other is null) return X.CompareTo(null);
            return X.CompareTo(other.X);
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
        public int CompareTo(OneDimensionalPoint<int> other)
        {
            if (other is null) return X.CompareTo(null);
            return X.CompareTo(other.X);
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
        public int CompareTo(double other)
        {
            return X.CompareTo(other);
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
        public int CompareTo(int other)
        {
            return X.CompareTo(other);
        }

        /// <summary>
        /// Converts a number to one dimensional point.
        /// </summary>
        /// <param name="value">The point.</param>
        public static implicit operator DoubleOneDimensionalPoint(double value)
        {
            return new DoubleOneDimensionalPoint(value);
        }

        /// <summary>
        /// Converts a number to one dimensional point.
        /// </summary>
        /// <param name="value">The point.</param>
        public static implicit operator DoubleOneDimensionalPoint(long value)
        {
            return new DoubleOneDimensionalPoint(value);
        }

        /// <summary>
        /// Converts a number to one dimensional point.
        /// </summary>
        /// <param name="value">The point.</param>
        public static implicit operator DoubleOneDimensionalPoint(int value)
        {
            return new DoubleOneDimensionalPoint(value);
        }

        /// <summary>
        /// Pluses two points in coordinate.
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after addition.</returns>
        public static DoubleOneDimensionalPoint operator +(DoubleOneDimensionalPoint leftValue, DoubleOneDimensionalPoint rightValue)
        {
            return (leftValue ?? new DoubleOneDimensionalPoint()).Plus(rightValue);
        }

        /// <summary>
        /// Pluses two points in coordinate.
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after addition.</returns>
        public static DoubleOneDimensionalPoint operator -(DoubleOneDimensionalPoint leftValue, DoubleOneDimensionalPoint rightValue)
        {
            return (leftValue ?? new DoubleOneDimensionalPoint()).Minus(rightValue);
        }
    }

    /// <summary>
    /// The point of 1D (line) integer coordinate.
    /// </summary>
    public class Int32OneDimensionalPoint
        : OneDimensionalPoint<int>,
        IAdvancedAdditionCapable<Int32OneDimensionalPoint>,
        IComparable<OneDimensionalPoint<double>>,
        IComparable<OneDimensionalPoint<int>>,
        IComparable<double>,
        IComparable<int>
    {
        /// <summary>
        /// Initializes a new instance of the Int32OneDimensionalPoint class.
        /// </summary>
        /// <remarks>You can use this to initialize an instance for the class.</remarks>
        public Int32OneDimensionalPoint()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Int32OneDimensionalPoint class.
        /// </summary>
        /// <param name="x">The value of X.</param>
        /// <remarks>You can use this to initialize an instance for the class.</remarks>
        public Int32OneDimensionalPoint(int x) : base(x)
        {
        }

        /// <summary>
        /// Gets a value indicating whether the current element is negative.
        /// true if it is positve or zero; otherwise, false.
        /// </summary>
        public bool IsNegative => X < 0;

        /// <summary>
        /// Gets a value indicating whether the current element is a unit element of zero.
        /// </summary>
        public bool IsZero => X == 0;

        /// <summary>
        /// Pluses another value to return. Current value will not be changed.
        /// this + value
        /// </summary>
        /// <param name="value">The value to be plused.</param>
        /// <returns>A result after leftValue plus rightValue.</returns>
        public Int32OneDimensionalPoint Plus(OneDimensionalPoint<int> value)
        {
            return value != null
                ? new Int32OneDimensionalPoint(X + value.X)
                : new Int32OneDimensionalPoint(X);
        }

        /// <summary>
        /// Pluses another value to return. Current value will not be changed.
        /// this + value
        /// </summary>
        /// <param name="value">The value to be plused.</param>
        /// <returns>A result after leftValue plus rightValue.</returns>
        public Int32OneDimensionalPoint Plus(Int32OneDimensionalPoint value)
        {
            return value != null
                ? new Int32OneDimensionalPoint(X + value.X)
                : new Int32OneDimensionalPoint(X);
        }

        /// <summary>
        /// Minuses another value to return. Current value will not be changed.
        /// this - value
        /// </summary>
        /// <param name="value">The value to be minuses.</param>
        /// <returns>A result after leftValue minus rightValue.</returns>
        public Int32OneDimensionalPoint Minus(OneDimensionalPoint<int> value)
        {
            return value != null
                ? new Int32OneDimensionalPoint(X - value.X)
                : new Int32OneDimensionalPoint(X);
        }

        /// <summary>
        /// Minuses another value to return. Current value will not be changed.
        /// this - value
        /// </summary>
        /// <param name="value">The value to be minuses.</param>
        /// <returns>A result after leftValue minus rightValue.</returns>
        public Int32OneDimensionalPoint Minus(Int32OneDimensionalPoint value)
        {
            return value != null
                ? new Int32OneDimensionalPoint(X - value.X)
                : new Int32OneDimensionalPoint(X);
        }

        /// <summary>
        /// Negates the current value to return. Current value will not be changed.
        /// -this
        /// </summary>
        /// <returns>A result after negation.</returns>
        public Int32OneDimensionalPoint Negate()
        {
            return new Int32OneDimensionalPoint(-X);
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public override object Clone()
        {
            return new Int32OneDimensionalPoint(X);
        }

        /// <summary>
        /// Gets a unit element for addition and subtraction.
        /// 0
        /// </summary>
        /// <returns>An element zero for the value.</returns>
        public Int32OneDimensionalPoint GetElementZero()
        {
            return new Int32OneDimensionalPoint(0);
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
        public int CompareTo(OneDimensionalPoint<double> other)
        {
            if (other is null) return X.CompareTo(null);
            return X.CompareTo(other.X);
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
        public int CompareTo(OneDimensionalPoint<int> other)
        {
            if (other is null) return X.CompareTo(null);
            return X.CompareTo(other.X);
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
        public int CompareTo(double other)
        {
            return ((double)X).CompareTo(other);
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
        public int CompareTo(int other)
        {
            return X.CompareTo(other);
        }

        /// <summary>
        /// Converts a number to one dimensional point.
        /// </summary>
        /// <param name="value">The point.</param>
        public static implicit operator Int32OneDimensionalPoint(int value)
        {
            return new Int32OneDimensionalPoint(value);
        }

        /// <summary>
        /// Pluses two points in coordinate.
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after addition.</returns>
        public static Int32OneDimensionalPoint operator +(Int32OneDimensionalPoint leftValue, Int32OneDimensionalPoint rightValue)
        {
            return (leftValue ?? new Int32OneDimensionalPoint()).Plus(rightValue);
        }

        /// <summary>
        /// Pluses two points in coordinate.
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after addition.</returns>
        public static Int32OneDimensionalPoint operator -(Int32OneDimensionalPoint leftValue, Int32OneDimensionalPoint rightValue)
        {
            return (leftValue ?? new Int32OneDimensionalPoint()).Minus(rightValue);
        }
    }
}
