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
    public class OneDimensionalPoint<TUnit> : SingleElement<TUnit> where TUnit : struct, IComparable<TUnit>, IEquatable<TUnit>
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
        /// Pluses two points in coordinate.
        /// leftValue + rightValue
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after addition.</returns>
        public static OneDimensionalPoint<int> Add(OneDimensionalPoint<int> leftValue, OneDimensionalPoint<int> rightValue)
        {
            if (leftValue == null) leftValue = new OneDimensionalPoint<int>();
            if (rightValue == null) rightValue = new OneDimensionalPoint<int>();
            return new OneDimensionalPoint<int>
            {
                X = leftValue.X + rightValue.X
            };
        }

        /// <summary>
        /// Pluses two points in coordinate.
        /// leftValue + rightValue
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after addition.</returns>
        public static OneDimensionalPoint<long> Add(OneDimensionalPoint<long> leftValue, OneDimensionalPoint<long> rightValue)
        {
            if (leftValue == null) leftValue = new OneDimensionalPoint<long>();
            if (rightValue == null) rightValue = new OneDimensionalPoint<long>();
            return new OneDimensionalPoint<long>
            {
                X = leftValue.X + rightValue.X
            };
        }

        /// <summary>
        /// Pluses two points in coordinate.
        /// leftValue + rightValue
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after addition.</returns>
        public static OneDimensionalPoint<float> Add(OneDimensionalPoint<float> leftValue, OneDimensionalPoint<float> rightValue)
        {
            if (leftValue == null) leftValue = new OneDimensionalPoint<float>();
            if (rightValue == null) rightValue = new OneDimensionalPoint<float>();
            return new OneDimensionalPoint<float>
            {
                X = leftValue.X + rightValue.X
            };
        }

        /// <summary>
        /// Pluses two points in coordinate.
        /// leftValue + rightValue
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after addition.</returns>
        public static OneDimensionalPoint<double> Add(OneDimensionalPoint<double> leftValue, OneDimensionalPoint<double> rightValue)
        {
            if (leftValue == null) leftValue = new OneDimensionalPoint<double>();
            if (rightValue == null) rightValue = new OneDimensionalPoint<double>();
            return new OneDimensionalPoint<double>
            {
                X = leftValue.X + rightValue.X
            };
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
}
