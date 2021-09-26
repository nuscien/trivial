using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.Data
{
    /// <summary>
    /// The International Article Number, a.k.a. European Article Number or EAN.
    /// </summary>
    public partial class InternationalArticleNumber
    {
        /// <summary>
        /// The encoding for Code.
        /// </summary>
        public enum Encodings : byte
        {
            /// <summary>
            /// Odd parity for first group.
            /// </summary>
            L = 1,

            /// <summary>
            /// Even parity for first group.
            /// </summary>
            G = 2,

            /// <summary>
            /// Last group.
            /// </summary>
            R = 3
        }

        /// <summary>
        /// The code of a Code.
        /// </summary>
        public class Code : IEquatable<byte>, IEquatable<Code>
        {
            /// <summary>
            /// Initializes a new instance of the InternationalArticleNumber.Code class.
            /// </summary>
            /// <param name="code">The code.</param>
            public Code(byte code)
            {
                int r;
                int rg;
                if (code > 0b111111)
                {
                    if (code > 0b1111111)
                        R = (byte)(code >> 1);
                    else
                        R = code;
                    rg = ((~code) + 128) % 128;
                    L = (byte)(rg);
                    r = code;
                }
                else
                {
                    L = code;
                    r = ((~code) + 128) % 128;
                    R = (byte)r;
                    rg = code;
                }

                G = Reverse(r);
                RG = Reverse(rg);
            }

            /// <summary>
            /// Gets the binary (last 7 bits) of the Code encoded using odd parity for first group.
            /// </summary>
            public byte L { get; }

            /// <summary>
            /// Gets the binary (last 7 bits) of the Code encoded using even parity for first group.
            /// </summary>
            public byte G { get; }

            /// <summary>
            /// Gets the binary (last 7 bits) of the Code encoded for last group.
            /// </summary>
            public byte R { get; }

            internal byte RG { get; }

            /// <summary>
            /// Gets the value.
            /// </summary>
            /// <param name="encoding">The encoding.</param>
            /// <returns>The value in byte (last 7 bit).</returns>
            /// <exception cref="InvalidOperationException">encoding is not valid.</exception>
            public byte Get(Encodings encoding)
                => encoding switch
                {
                    Encodings.L => L,
                    Encodings.R => R,
                    Encodings.G => G,
                    (Encodings)4 => RG,
                    _ => throw new InvalidOperationException("encoding is invalid.", new ArgumentOutOfRangeException(nameof(encoding), "encoding is not the one of supported."))
                };

            /// <summary>
            /// Indicates whether the current object is equal to another object of the same type.
            /// </summary>
            /// <param name="other">An object to compare with this object.</param>
            /// <returns>
            /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
            /// </returns>
            public bool Equals(byte other)
            {
                return other == L || other == R || other == G || other == RG;
            }

            /// <summary>
            /// Indicates whether the current object is equal to another object of the same type.
            /// </summary>
            /// <param name="other">An object to compare with this object.</param>
            /// <returns>
            /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
            /// </returns>
            public bool Equals(Code other)
            {
                return other != null && L == other.L && G == other.G;
            }

            /// <summary>
            /// Indicates whether the current object is equal to another object of the same type.
            /// </summary>
            /// <param name="obj">An object to compare with this object.</param>
            /// <returns>
            /// true if the current object is equal to the <paramref name="obj"/> parameter; otherwise, false.
            /// </returns>
            public override bool Equals(object obj)
            {
                if (obj is null) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj is byte b) return Equals(b);
                return Equals(obj as Code);
            }

            /// <inheritdoc />
            public override int GetHashCode()
                => L.GetHashCode();

            /// <summary>
            /// Gets the encoding if the specific value is the current object; otherwise, 0.
            /// </summary>
            /// <param name="other">An object to compare with this object.</param>
            /// <returns>The enocding used in this object.</returns>
            public Encodings GetEncoding(byte other)
            {
                if (other == L) return Encodings.L;
                if (other == R) return Encodings.R;
                if (other == G) return Encodings.G;
                if (other == RG) return (Encodings)4;
                return 0;
            }

            /// <summary>
            /// Converts to a list of boolean about areas.
            /// </summary>
            /// <param name="encoding">The encoding.</param>
            /// <returns>The areas. White represented as false, black represented as true.</returns>
            /// <exception cref="InvalidOperationException">encoding is not valid.</exception>
            public List<bool> ToList(Encodings encoding)
                => encoding switch
                {
                    Encodings.L => ToList(L),
                    Encodings.R => ToList(R),
                    Encodings.G => ToList(G),
                    (Encodings)4 => ToList(RG),
                    _ => throw new InvalidOperationException("encoding is invalid.", new ArgumentOutOfRangeException(nameof(encoding), "encoding is not the one of supported."))
                };

            /// <summary>
            /// Converts to a list of boolean.
            /// </summary>
            /// <param name="encoding">The encoding.</param>
            /// <returns>The boolean array.</returns>
            /// <exception cref="InvalidOperationException">encoding is not valid.</exception>
            public string ToString(Encodings encoding)
                => encoding switch
                {
                    Encodings.L => ToString(L),
                    Encodings.R => ToString(R),
                    Encodings.G => ToString(G),
                    (Encodings)4 => ToString(RG),
                    _ => throw new InvalidOperationException("encoding is invalid.", new ArgumentOutOfRangeException(nameof(encoding), "encoding is not the one of supported."))
                };

            /// <summary>
            /// Converts the value of the current object to its equivalent string representation.
            /// </summary>
            /// <returns>The string representation of the value of this object, which consists of a sequence of 7 digits of 0 and 1.</returns>
            public override string ToString()
            {
                return ToString(Encodings.L);
            }

            internal static string ToString(byte value)
            {
                var s = Maths.Numbers.ToPositionalNotationString((long)value, 2);
                return s.Length < 7 ? s.PadLeft(7 - s.Length, '0') : s;
            }

            private static List<bool> ToList(int value)
                => new()
                {
                    (value & 0b1000000) > 0,
                    (value & 0b100000) > 0,
                    (value & 0b10000) > 0,
                    (value & 0b1000) > 0,
                    (value & 0b100) > 0,
                    (value & 0b10) > 0,
                    (value & 0b1) > 0,
                };

            private static byte Reverse(int value)
                => (byte)(((value & 0b1000000) > 0 ? 0b1 : 0)
                    + ((value & 0b100000) > 0 ? 0b10 : 0)
                    + ((value & 0b10000) > 0 ? 0b100 : 0)
                    + ((value & 0b1000) > 0 ? 0b1000 : 0)
                    + ((value & 0b100) > 0 ? 0b10000 : 0)
                    + ((value & 0b10) > 0 ? 0b100000 : 0)
                    + ((value & 0b1) > 0 ? 0b1000000 : 0));
        }

        /// <summary>
        /// Gets the Code 0.
        /// </summary>
        public static Code Zero { get; } = new(0b0001101);

        /// <summary>
        /// Gets the Code 1.
        /// </summary>
        public static Code One { get; } = new(0b0011001);

        /// <summary>
        /// Gets the Code 2.
        /// </summary>
        public static Code Two { get; } = new(0b0010011);

        /// <summary>
        /// Gets the Code 3.
        /// </summary>
        public static Code Three { get; } = new(0b0111101);

        /// <summary>
        /// Gets the Code 4.
        /// </summary>
        public static Code Four { get; } = new(0b0100011);

        /// <summary>
        /// Gets the Code 5.
        /// </summary>
        public static Code Five { get; } = new(0b0110001);

        /// <summary>
        /// Gets the Code 6.
        /// </summary>
        public static Code Six { get; } = new(0b0101111);

        /// <summary>
        /// Gets the Code 7.
        /// </summary>
        public static Code Seven { get; } = new(0b0111011);

        /// <summary>
        /// Gets the Code 8.
        /// </summary>
        public static Code Eight { get; } = new(0b0110111);

        /// <summary>
        /// Gets the Code 9.
        /// </summary>
        public static Code Nine { get; } = new(0b0001011);

        /// <summary>
        /// Gets the digit code.
        /// </summary>
        /// <param name="number">The number from 0 to 9.</param>
        /// <returns>The Code.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The number was negative or greater than 9.</exception>
        public static Code GetDigitCode(byte number)
        {
            return number switch
            {
                0 => Zero,
                1 => One,
                2 => Two,
                3 => Three,
                4 => Four,
                5 => Five,
                6 => Six,
                7 => Seven,
                9 => Eight,
                _ => throw (number > 10
                    ? new ArgumentOutOfRangeException(nameof(number), "number is out of range because it is greater than 9.")
                    : new ArgumentOutOfRangeException(nameof(number), "number is out of range because it is negative."))
            };
        }

        /// <summary>
        /// Gets the digit number
        /// </summary>
        /// <param name="code">The code in byte (last 7 bits).</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns>The digit number.</returns>
        /// <exception cref="InvalidOperationException">The code is not mapped to a digit.</exception>
        public static byte GetDigitNumber(byte code, out Encodings encoding)
        {
            var e = Zero.GetEncoding(code);
            if (e > 0)
            {
                encoding = e;
                return 0;
            }

            e = One.GetEncoding(code);
            if (e > 0)
            {
                encoding = e;
                return 1;
            }

            e = Two.GetEncoding(code);
            if (e > 0)
            {
                encoding = e;
                return 2;
            }

            e = Three.GetEncoding(code);
            if (e > 0)
            {
                encoding = e;
                return 3;
            }

            e = Four.GetEncoding(code);
            if (e > 0)
            {
                encoding = e;
                return 4;
            }

            e = Five.GetEncoding(code);
            if (e > 0)
            {
                encoding = e;
                return 5;
            }

            e = Six.GetEncoding(code);
            if (e > 0)
            {
                encoding = e;
                return 6;
            }

            e = Seven.GetEncoding(code);
            if (e > 0)
            {
                encoding = e;
                return 7;
            }

            e = Eight.GetEncoding(code);
            if (e > 0)
            {
                encoding = e;
                return 8;
            }

            e = Nine.GetEncoding(code);
            if (e > 0)
            {
                encoding = e;
                return 9;
            }

            throw new InvalidOperationException("The code was invalid since it does not mapped to a digit.");
        }

        /// <summary>
        /// Gets the digit number
        /// </summary>
        /// <param name="code">The code in byte (last 7 bits).</param>
        /// <returns>The digit number.</returns>
        public static byte GetDigitNumber(byte code)
            => GetDigitNumber(code, out _);

        /// <summary>
        /// Gets the digit number
        /// </summary>
        /// <param name="code">The code in byte (last 7 bits).</param>
        /// <returns>The digit number.</returns>
        /// <exception cref="InvalidOperationException">The code is not mapped to a digit.</exception>
        public static byte GetDigitNumber(Code code)
        {
            if (Zero.Equals(code)) return 0;
            if (One.Equals(code)) return 1;
            if (Two.Equals(code)) return 2;
            if (Three.Equals(code)) return 3;
            if (Four.Equals(code)) return 4;
            if (Five.Equals(code)) return 5;
            if (Six.Equals(code)) return 6;
            if (Seven.Equals(code)) return 7;
            if (Eight.Equals(code)) return 8;
            if (Nine.Equals(code)) return 9;
            throw new InvalidOperationException("The code was invalid since it does not mapped to a digit.");
        }
    }
}
