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
    public partial struct Angle : IComparable, IComparable<IAngle>, IEquatable<IAngle>, IComparable<double>, IEquatable<double>, IComparable<int>, IEquatable<int>, IAdvancedAdditionCapable<Angle>
    {
        /// <summary>
        /// The modes to rectify.
        /// </summary>
        public enum RectifyModes
        {
            /// <summary>
            /// Forbidden.
            /// It will throw an ArgumentOutOfRangeException if the value is out of range.
            /// </summary>
            None = 0,

            /// <summary>
            /// Circulation.
            /// It will turn to the other side of the range for the value out of range.
            /// </summary>
            Cycle = 1,

            /// <summary>
            /// Turn back like a pendulum.
            /// It will retrace when touch the boundary.
            /// </summary>
            Bounce = 2
        }

        /// <summary>
        /// The boundary options.
        /// </summary>
        public class BoundaryOptions
        {
            /// <summary>
            /// Initializes a new instance of the BoundaryOptions class.
            /// </summary>
            /// <param name="max">The maximum degree.</param>
            /// <param name="negative">true if the minimum degree is the negatived of the maximum one; otherwise, false, the minimum is 0.</param>
            /// <param name="rectify">The mode to rectify.</param>
            public BoundaryOptions(int max, bool negative, RectifyModes rectify)
            {
                MaxDegree = max;
                Negative = negative;
                RectifyMode = rectify;
            }

            /// <summary>
            /// The maximum degree supported. Should be greater than 0.
            /// </summary>
            public int MaxDegree { get; }

            /// <summary>
            /// Gets a value indicating whether set the mirror mode.
            /// </summary>
            public bool Negative { get; }

            /// <summary>
            /// The mode to rectify if the value is out of the scope.
            /// </summary>
            public RectifyModes RectifyMode { get; }
        }
    }
}
