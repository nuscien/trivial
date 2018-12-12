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
    /// The polar coordinate point.
    /// </summary>
    public class PolarPoint
    {
        /// <summary>
        /// Polar coordinate point symbols.
        /// </summary>
        public static class Symbols
        {
            /// <summary>
            /// The parameter name of theta.
            /// </summary>
            public const string ThetaSymbol = "θ";

            /// <summary>
            /// The parameter name of radius.
            /// </summary>
            public const string RadiusSymbol = "r";
        }

        /// <summary>
        /// Initializes a new instance of the PolarPoint class.
        /// </summary>
        /// <remarks>You can use this to initialize an instance for the class.</remarks>
        public PolarPoint()
        {
        }

        /// <summary>
        /// Initializes a new instance of the PolarPoint class.
        /// </summary>
        /// <param name="r">The length between center point and the specific point.</param>
        /// <param name="theta">The value of angel.</param>
        /// <remarks>You can use this to initialize an instance for the class.</remarks>
        public PolarPoint(float r, Angle theta)
        {
            Radius = r;
            Theta = theta;
        }

        /// <summary>
        /// Gets or sets the length between center point and the specific point (r).
        /// </summary>
        public float Radius
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the angel (θ).
        /// </summary>
        public Angle Theta
        {
            get;
            set;
        }

        /// <summary>
        /// Returns a tuple that represents the values of current coordinate point object.
        /// </summary>
        /// <returns>The tuple representation of this coordinate point object.</returns>
        public Tuple<float, Angle> ToTuple()
        {
            return new Tuple<float, Angle>(Radius, Theta);
        }

        /// <summary>
        /// Returns the point string value of this instance.
        /// </summary>
        /// <returns>A System.String containing this point.</returns>
        public override string ToString()
        {
            var radius = Radius.ToString();
            var theta = Theta.ToString();
            var longStr = string.Format("{0} - {1}", radius, theta);
            var sep = false;
            if (longStr.IndexOfAny(new[] { ',', ';' }) > -1) sep = true;
            if (!sep && longStr.IndexOf(';') > -1)
            {
                const string quoteStr = "\"{0}\"";
                radius = string.Format(quoteStr, radius.Replace("\"", "\\\""));
                theta = string.Format(quoteStr, theta.Replace("\"", "\\\""));
            }

            return string.Format("{0} = {1}{2} {3} = {4}", Symbols.RadiusSymbol, radius, sep ? ";" : ",", Symbols.ThetaSymbol, theta);
        }
    }
}
