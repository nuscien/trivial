using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.Text
{
    /// <summary>
    /// The Latin alphabet (a.k.a Roman alphabet) which is used in English, French, Italian, etc.
    /// </summary>
    public static partial class LatinAlphabet
    {
        /// <summary>
        /// Frensh (Français) alphabet.
        /// </summary>
        public static class French
        {
            /// <summary>
            /// The name of French in French.
            /// </summary>
            public const string LanguageName = "Français";

            /// <summary>
            /// The language name in English.
            /// </summary>
            public const string EnglishLanguageName = "French";

            /// <summary>
            /// The language code.
            /// </summary>
            public const string LanguageCode = "fr";

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
}
