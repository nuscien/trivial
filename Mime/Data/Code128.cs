using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.Data
{
    /// <summary>
    /// The code-128, which is a high-density linear barcode symbology defined in ISO/IEC 15417:2007.
    /// It is used for alphanumeric or numeric-only barcodes.
    /// </summary>
    public partial class Code128
    {
        /// <summary>
        /// The sub-types.
        /// </summary>
        public enum Subtypes : byte
        {
            /// <summary>
            /// Code A.
            /// </summary>
            A = 1,

            /// <summary>
            /// Code B.
            /// </summary>
            B = 2,

            /// <summary>
            /// Code C.
            /// </summary>
            C = 3
        }

        /// <summary>
        /// The output string format.
        /// </summary>
        public enum Formats : byte
        {
            /// <summary>
            /// Regular format.
            /// </summary>
            Regular = 0,

            /// <summary>
            /// Text only.
            /// </summary>
            Text = 1,

            /// <summary>
            /// Symbol values.
            /// </summary>
            Values = 2,
            
            /// <summary>
            /// Hex values.
            /// </summary>
            Hex = 3,

            /// <summary>
            /// The barcode areas that white represented as 0 and black represented as 1.
            /// </summary>
            Barcode = 4,

            /// <summary>
            /// The stroke path data used in SVG and XAML.
            /// </summary>
            Path = 5
        }

        /// <summary>
        /// All patterns.
        /// </summary>
        private readonly static List<string> patterns = new() { "11011001100", "11001101100", "11001100110", "10010011000", "10010001100", "10001001100", "10011001000", "10011000100", "10001100100", "11001001000", "11001000100", "11000100100", "10110011100", "10011011100", "10011001110", "10111001100", "10011101100", "10011100110", "11001110010", "11001011100", "11001001110", "11011100100", "11001110100", "11101101110", "11101001100", "11100101100", "11100100110", "11101100100", "11100110100", "11100110010", "11011011000", "11011000110", "11000110110", "10100011000", "10001011000", "10001000110", "10110001000", "10001101000", "10001100010", "11010001000", "11000101000", "11000100010", "10110111000", "10110001110", "10001101110", "10111011000", "10111000110", "10001110110", "11101110110", "11010001110", "11000101110", "11011101000", "11011100010", "11011101110", "11101011000", "11101000110", "11100010110", "11101101000", "11101100010", "11100011010", "11101111010", "11001000010", "11110001010", "10100110000", "10100001100", "10010110000", "10010000110", "10000101100", "10000100110", "10110010000", "10110000100", "10011010000", "10011000010", "10000110100", "10000110010", "11000010010", "11001010000", "11110111010", "11000010100", "10001111010", "10100111100", "10010111100", "10010011110", "10111100100", "10011110100", "10011110010", "11110100100", "11110010100", "11110010010", "11011011110", "11011110110", "11110110110", "10101111000", "10100011110", "10001011110", "10111101000", "10111100010", "11110101000", "11110100010", "10111011110", "10111101110", "11101011110", "11110101110", "11010000100", "11010010000", "11010011100", "11000111010" };

        /// <summary>
        /// Gets the pattern of the specific symbol value.
        /// White represented as false, black represented as true.
        /// </summary>
        /// <param name="value">The symbol value.</param>
        /// <returns>The areas in boolean collection.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The value was greater than 106.</exception>
        public static List<bool> GetPattern(byte value)
            => GetPatternInternal(value).ToList();

        /// <summary>
        /// Gets the pattern of the specific symbol value.
        /// </summary>
        /// <param name="value">The symbol value.</param>
        /// <returns>The barcode string.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The value was greater than 106.</exception>
        public static string GetPatternString(byte value)
            => value < 107 ? patterns[value] : throw new ArgumentOutOfRangeException(nameof(value), "value should be less than 107.");

        /// <summary>
        /// Gets the pattern of the specific symbol value.
        /// </summary>
        /// <param name="value">The symbol value.</param>
        /// <param name="black">The value of black represented.</param>
        /// <param name="white">The value of white represented.</param>
        /// <returns>The barcode string.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The value was greater than 106.</exception>
        public static string ToBarcodeString(byte value, char black, char white)
            => ToBarcodeString(value, ele => ele ? black : white);

        /// <summary>
        /// Gets the pattern of the specific symbol value.
        /// </summary>
        /// <param name="value">The symbol value.</param>
        /// <param name="selector">The selector to convert boolean array to a string. Per boolean value, white represented as false, black represented as true.</param>
        /// <returns>The barcode string.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The value was greater than 106.</exception>
        public static string ToBarcodeString(byte value, Func<bool, char> selector)
            => string.Join(string.Empty, GetPatternInternal(value).Select(selector));

        /// <summary>
        /// Gets the pattern of the specific symbol value.
        /// </summary>
        /// <param name="value">The symbol value.</param>
        /// <param name="selector">The selector to convert boolean array to a string. Per boolean value, white represented as false, black represented as true.</param>
        /// <returns>The barcode string.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The value was greater than 106.</exception>
        public static string ToBarcodeString(byte value, Func<bool, int, char> selector)
            => string.Join(string.Empty, GetPatternInternal(value).Select(selector));

        /// <summary>
        /// Converts a symbol value to string.
        /// </summary>
        /// <param name="subtype">The sub-type.</param>
        /// <param name="value">The symbol value.</param>
        /// <returns>A string represented.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static string ToString(Subtypes subtype, byte value)
        {
            if (value > 101) return value switch
            {
                102 => "[FNC1]",
                103 => "[Start A]",
                104 => "[Start B]",
                105 => "[Start C]",
                106 => "[Stop]",
                _ => string.Empty
            };
            if (subtype == Subtypes.C)
                return value < 100 ? value.ToString("g") : value switch
                {
                    100 => "[Code B]",
                    101 => "[Code A]",
                    _ => string.Empty
                };

            var isA = subtype == Subtypes.A;
            if (!isA && subtype != Subtypes.B)
                throw new InvalidOperationException("subtype is not valid.");
            if (value < 64 || (!isA && value < 96))
                return new string((char)(value + 32), 1);
            if (value < 96)
                return new string((char)(value - 64), 1);
            return value switch
            {
                96 => "[FNC3]",
                97 => "[FNC2]",
                98 => isA ? "[Shift B]" : "[Shift A]",
                99 => "[Code C]",
                100 => isA ? "[Code B]" : "[FNC4]",
                101 => isA ? "[FNC4]" : "[Code A]",
                _ => string.Empty
            };
        }

        /// <summary>
        /// Gets the pattern of the specific symbol value.
        /// </summary>
        /// <param name="value">The symbol value.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">The value was greater than 106.</exception>
        private static IEnumerable<bool> GetPatternInternal(byte value)
        {
            if (value > 106) throw new ArgumentOutOfRangeException(nameof(value), "value should be less than 107.");
            var item = patterns[value];
            return item.Select(c => c switch
            {
                '0' => false,
                _ => true
            });
        }
    }
}
