// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Angle\Options.cs" company="Nanchang Jinchen Software Co., Ltd.">
//   Copyright (c) 2010 Nanchang Jinchen Software Co., Ltd. All rights reserved.
// </copyright>
// <summary>
//   The options of angle.
// </summary>
// <author>Kingcean Tuan</author>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Text.Json.Serialization;

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
            /// <param name="canBeNegative">true if the minimum degree is the negatived of the maximum one; otherwise, false, the minimum is 0.</param>
            /// <param name="rectify">The mode to rectify.</param>
            public BoundaryOptions(int max, bool canBeNegative, RectifyModes rectify)
            {
                MaxDegree = max;
                CanBeNegative = canBeNegative;
                RectifyMode = rectify;
            }

            /// <summary>
            /// Gets the maximum degree supported. Should be greater than 0.
            /// </summary>
            [JsonPropertyName("max")]
            public int MaxDegree { get; }

            /// <summary>
            /// Gets a value indicating whether use the mirror mode.
            /// </summary>
            [JsonPropertyName("negative")]
            public bool CanBeNegative { get; }

            /// <summary>
            /// Gets the mode to rectify if the value is out of the scope.
            /// </summary>
            [JsonPropertyName("rectify")]
            [JsonConverter(typeof(JsonStringEnumConverter))]
            public RectifyModes RectifyMode { get; }
        }
    }
}
