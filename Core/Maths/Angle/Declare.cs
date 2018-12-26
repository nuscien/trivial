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
    /// Angle.
    /// </summary>
    public interface IAngle
    {
        /// <summary>
        /// Gets the degree part of the angle.
        /// </summary>
        int Degree { get; }

        /// <summary>
        /// Gets the arcminute of the angle.
        /// </summary>
        int Arcminute { get; }

        /// <summary>
        /// Gets the arcsecond of the angle.
        /// </summary>
        float Arcsecond { get; }

        /// <summary>
        /// Gets the total degrees.
        /// </summary>
        double Degrees { get; }
    }

    /// <summary>
    /// The struct of degree (angle).
    /// </summary>
    public partial struct Angle
    {
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
        /// Gets an angle with 0 degree.
        /// </summary>
        public static Angle ZeroDegree { get; } = new Angle(0);

        /// <summary>
        /// Gets an angle with 1 degree.
        /// </summary>
        public static Angle OneDegree { get; } = new Angle(1);

        /// <summary>
        /// Gets the right angle.
        /// </summary>
        public static Angle Right { get; } = new Angle(90);

        /// <summary>
        /// Gets the straight angle.
        /// </summary>
        public static Angle Straight { get; } = new Angle(180);

        /// <summary>
        /// Gets the full angle.
        /// </summary>
        public static Angle Full { get; } = new Angle(360);
        /// <summary>
        /// Gets an angle with 0 degree.
        /// </summary>
        public static Model ZeroDegreeModel { get { return new Model { Degree = 0 }; } }

        /// <summary>
        /// Gets an angle with 1 degree.
        /// </summary>
        public static Model OneDegreeModel { get { return new Model { Degree = 1 }; } }

        /// <summary>
        /// Gets the right angle.
        /// </summary>
        public static Model RightModel { get { return new Model { Degree = 90 }; } }

        /// <summary>
        /// Gets the straight angle.
        /// </summary>
        public static Model StraightModel { get { return new Model { Degree = 180 }; } }

        /// <summary>
        /// Gets the full angle.
        /// </summary>
        public static Model FullModel { get { return new Model { Degree = 360 }; } }
    }
}
