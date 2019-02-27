﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Trivial.Text
{
    /// <summary>
    /// Letter cases.
    /// </summary>
    public enum Cases
    {
        /// <summary>
        /// Keep original.
        /// </summary>
        Original = 0,

        /// <summary>
        /// Uppercase.
        /// </summary>
        Upper = 1,

        /// <summary>
        /// Lowercase.
        /// </summary>
        Lower = 2,

        /// <summary>
        /// First letter uppercase and rest keeping original.
        /// </summary>
        FirstLetterUpper = 3,

        /// <summary>
        /// First letter lowercase and rest keeping original.
        /// </summary>
        FirstLetterLower = 4
    }

    /// <summary>
    /// The string extension and helper.
    /// </summary>
    public static class StringUtility
    {
        /// <summary>
        /// Returns a copy of this string converted to specific case, using the casing rules of the specified culture.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="options">The specific case.</param>
        /// <param name="culture">An object that supplies culture-specific casing rules.</param>
        /// <returns>The specific case equivalent of the current string.</returns>
        /// <exception cref="ArgumentNullException">culture is null.</exception>
        public static string ToSpecificCase(this string source, Cases options, CultureInfo culture)
        {
            if (string.IsNullOrWhiteSpace(source)) return source;
            switch (options)
            {
                case Cases.Original:
                    return source;
                case Cases.Upper:
                    return source.ToUpper(culture);
                case Cases.Lower:
                    return source.ToLower(culture);
                case Cases.FirstLetterUpper:
                    return $"{source.Substring(0, 1).ToUpper(culture)}{source.Substring(1)}";
                case Cases.FirstLetterLower:
                    return $"{source.Substring(0, 1).ToLower(culture)}{source.Substring(1)}";
                default:
                    return source;
            }
        }

        /// <summary>
        /// Returns a copy of this string converted to specific case.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="options">The specific case.</param>
        /// <returns>The specific case equivalent of the current string.</returns>
        public static string ToSpecificCase(this string source, Cases options)
        {
            if (string.IsNullOrWhiteSpace(source)) return source;
            switch (options)
            {
                case Cases.Original:
                    return source;
                case Cases.Upper:
                    return source.ToUpper();
                case Cases.Lower:
                    return source.ToLower();
                case Cases.FirstLetterUpper:
                    return $"{source.Substring(0, 1).ToUpper()}{source.Substring(1)}";
                case Cases.FirstLetterLower:
                    return $"{source.Substring(0, 1).ToLower()}{source.Substring(1)}";
                default:
                    return source;
            }
        }

        /// <summary>
        /// Returns a copy of this string converted to specific case, using the casing rules of the invariant culture.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="options">The specific case.</param>
        /// <returns>The specific case equivalent of the current string.</returns>
        public static string ToSpecificCaseInvariant(this string source, Cases options)
        {
            return ToSpecificCase(source, options, CultureInfo.InvariantCulture);
        }
    }
}
