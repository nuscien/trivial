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
    /// The generic 2D (plane) coordinate point.
    /// </summary>
    /// <typeparam name="TUnit">The type of unit.</typeparam>
    public class TwoDimensionalPoint<TUnit> : TwoElements<TUnit> where TUnit : struct, IComparable<TUnit>, IEquatable<TUnit>
    {
        /// <summary>
        /// Initializes a new instance of the TwoDimensionalPoint class.
        /// </summary>
        /// <remarks>You can use this to initialize an instance for the class.</remarks>
        public TwoDimensionalPoint()
        {
        }

        /// <summary>
        /// Initializes a new instance of the TwoDimensionalPoint class.
        /// </summary>
        /// <param name="x">The value of X.</param>
        /// <param name="y">The value of Y.</param>
        /// <remarks>You can use this to initialize an instance for the class.</remarks>
        public TwoDimensionalPoint(TUnit x, TUnit y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Gets or sets the value of X (horizontal position). The value is same as ItemA.
        /// </summary>
        public TUnit X
        {
            get => base.ItemA;
            set => base.ItemA = value;
        }

        /// <summary>
        /// Gets or sets the value of Y (vertical position). The value is same as ItemB.
        /// </summary>
        public TUnit Y
        {
            get => base.ItemB;
            set => base.ItemB = value;
        }

        private new TUnit ItemA
        {
            get => base.ItemA;
            set => base.ItemA = value;
        }

        private new TUnit ItemB
        {
            get => base.ItemB;
            set => base.ItemB = value;
        }

        /// <summary>
        /// Returns the point string value of this instance.
        /// </summary>
        /// <returns>A System.String containing this point.</returns>
        public override string ToString()
        {
            var x = X.ToString();
            var y = Y.ToString();
            var longStr = string.Format("{0} - {1}", x, y);
            var sep = false;
            if (longStr.IndexOfAny(new[] { ',', ';' }) > -1) sep = true;
            if (!sep && longStr.IndexOf(';') > -1)
            {
                const string quoteStr = "\"{0}\"";
                x = string.Format(quoteStr, x.Replace("\"", "\\\""));
                y = string.Format(quoteStr, y.Replace("\"", "\\\""));
            }

            return string.Format("X = {0}{1} Y = {2}", x, sep ? ";" : ",", y);
        }
    }
}
