﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.Text;

/// <summary>
/// The Latin alphabet (a.k.a Roman alphabet) which is used in English, French, Italian, etc.
/// </summary>
public static partial class LatinAlphabet
{
    /// <summary>
    /// English alphabet.
    /// </summary>
    public static class English
    {
        /// <summary>
        /// The name of English in English.
        /// </summary>
        public const string LanguageName = "English";

        /// <summary>
        /// The name of English in English.
        /// </summary>
        public const string EnglishLanguageName = "English";

        /// <summary>
        /// The language code.
        /// </summary>
        public const string LanguageCode = "en";

        /// <summary>
        /// The count of the letter.
        /// </summary>
        public const int Count = 26;

        /// <summary>
        /// Lists the capital letters.
        /// </summary>
        /// <returns>All captial letters.</returns>
        public static IEnumerable<string> CapitalLetters()
            => LatinAlphabet.CapitalLetters();

        /// <summary>
        /// Lists the small letters.
        /// </summary>
        /// <returns>All small letters.</returns>
        public static IEnumerable<string> SmallLetters()
            => LatinAlphabet.SmallLetters();
    }
}
