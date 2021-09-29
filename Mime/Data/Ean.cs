using System;
using System.Collections.Generic;
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
        /// All digits.
        /// </summary>
        private string value;

        /// <summary>
        /// Converts the value of the current object to its equivalent string representation.
        /// </summary>
        /// <returns>The string representation of the value of this object, which consists of a sequence of 7 digits of 0 and 1.</returns>
        public override string ToString()
            => value ?? string.Empty;

        /// <summary>
        /// Gets the checksum of EAN13.
        /// </summary>
        /// <returns>The checksum.</returns>
        /// <exception cref="InvalidOperationException">The digits was not valid.</exception>
        public byte Checksum()
        {
            if (string.IsNullOrEmpty(value))
                throw new InvalidOperationException("The value should not be null.");
            return Checksum(value);
        }

        /// <summary>
        /// Converts to code list.
        /// </summary>
        /// <returns>The code list.</returns>
        public List<Code> ToList()
        {
            if (value == null) return new();
            return value.Select(ele => ele switch
            {
                '0' => Zero,
                '1' => One,
                '2' => Two,
                '3' => Three,
                '4' => Four,
                '5' => Five,
                '6' => Six,
                '7' => Seven,
                '8' => Eight,
                '9' => Nine,
                _ => null
            }).Where(ele => ele != null).ToList();
        }

        /// <summary>
        /// Returns a string that represents the barcode in stroke path for SVG or XAML.
        /// </summary>
        /// <param name="height">The bar height.</param>
        /// <returns>A stroke path string that represents the barcode.</returns>
        public string ToPathString(int height = 40)
        {
            var sb = new StringBuilder();
            var i = 6;
            sb.Append("M0,0 ");
            foreach (var b in ToBarcode())
            {
                i++;
                if (b) sb.Append($"M{i},0 L{i},{height} ");
            }

            sb.Append($"M{i + 6},0");
            return sb.ToString();
        }

        /// <summary>
        /// Converts to boolean list.
        /// White represented as false, black represented as true.
        /// </summary>
        /// <returns>The boolean list of barcode.</returns>
        /// <exception cref="InvalidOperationException">It was not an EAN-13 ro EAN-8 code.</exception>
        public List<bool> ToBarcode()
        {
            var codes = ToList();
            var col = new List<bool>();
            if (codes.Count == 13)
            {
                col.Add(true);
                col.Add(false);
                col.Add(true);
                switch (value.FirstOrDefault())
                {
                    case '0':
                        col.AddRange(codes[1].ToList(Encodings.L));
                        col.AddRange(codes[2].ToList(Encodings.L));
                        col.AddRange(codes[3].ToList(Encodings.L));
                        col.AddRange(codes[4].ToList(Encodings.L));
                        col.AddRange(codes[5].ToList(Encodings.L));
                        col.AddRange(codes[6].ToList(Encodings.L));
                        break;
                    case '1':
                        col.AddRange(codes[1].ToList(Encodings.L));
                        col.AddRange(codes[2].ToList(Encodings.L));
                        col.AddRange(codes[3].ToList(Encodings.G));
                        col.AddRange(codes[4].ToList(Encodings.L));
                        col.AddRange(codes[5].ToList(Encodings.G));
                        col.AddRange(codes[6].ToList(Encodings.G));
                        break;
                    case '2':
                        col.AddRange(codes[1].ToList(Encodings.L));
                        col.AddRange(codes[2].ToList(Encodings.L));
                        col.AddRange(codes[3].ToList(Encodings.G));
                        col.AddRange(codes[4].ToList(Encodings.G));
                        col.AddRange(codes[5].ToList(Encodings.L));
                        col.AddRange(codes[6].ToList(Encodings.G));
                        break;
                    case '3':
                        col.AddRange(codes[1].ToList(Encodings.L));
                        col.AddRange(codes[2].ToList(Encodings.L));
                        col.AddRange(codes[3].ToList(Encodings.G));
                        col.AddRange(codes[4].ToList(Encodings.G));
                        col.AddRange(codes[5].ToList(Encodings.G));
                        col.AddRange(codes[6].ToList(Encodings.L));
                        break;
                    case '4':
                        col.AddRange(codes[1].ToList(Encodings.L));
                        col.AddRange(codes[2].ToList(Encodings.G));
                        col.AddRange(codes[3].ToList(Encodings.L));
                        col.AddRange(codes[4].ToList(Encodings.L));
                        col.AddRange(codes[5].ToList(Encodings.G));
                        col.AddRange(codes[6].ToList(Encodings.G));
                        break;
                    case '5':
                        col.AddRange(codes[1].ToList(Encodings.L));
                        col.AddRange(codes[2].ToList(Encodings.G));
                        col.AddRange(codes[3].ToList(Encodings.G));
                        col.AddRange(codes[4].ToList(Encodings.L));
                        col.AddRange(codes[5].ToList(Encodings.L));
                        col.AddRange(codes[6].ToList(Encodings.G));
                        break;
                    case '6':
                        col.AddRange(codes[1].ToList(Encodings.L));
                        col.AddRange(codes[2].ToList(Encodings.G));
                        col.AddRange(codes[3].ToList(Encodings.G));
                        col.AddRange(codes[4].ToList(Encodings.G));
                        col.AddRange(codes[5].ToList(Encodings.L));
                        col.AddRange(codes[6].ToList(Encodings.L));
                        break;
                    case '7':
                        col.AddRange(codes[1].ToList(Encodings.L));
                        col.AddRange(codes[2].ToList(Encodings.G));
                        col.AddRange(codes[3].ToList(Encodings.L));
                        col.AddRange(codes[4].ToList(Encodings.G));
                        col.AddRange(codes[5].ToList(Encodings.L));
                        col.AddRange(codes[6].ToList(Encodings.G));
                        break;
                    case '8':
                        col.AddRange(codes[1].ToList(Encodings.L));
                        col.AddRange(codes[2].ToList(Encodings.G));
                        col.AddRange(codes[3].ToList(Encodings.L));
                        col.AddRange(codes[4].ToList(Encodings.G));
                        col.AddRange(codes[5].ToList(Encodings.G));
                        col.AddRange(codes[6].ToList(Encodings.L));
                        break;
                    case '9':
                        col.AddRange(codes[1].ToList(Encodings.L));
                        col.AddRange(codes[2].ToList(Encodings.G));
                        col.AddRange(codes[3].ToList(Encodings.G));
                        col.AddRange(codes[4].ToList(Encodings.L));
                        col.AddRange(codes[5].ToList(Encodings.G));
                        col.AddRange(codes[6].ToList(Encodings.L));
                        break;
                }

                col.Add(false);
                col.Add(true);
                col.Add(false);
                col.Add(true);
                col.Add(false);
                col.AddRange(codes[7].ToList(Encodings.R));
                col.AddRange(codes[8].ToList(Encodings.R));
                col.AddRange(codes[9].ToList(Encodings.R));
                col.AddRange(codes[10].ToList(Encodings.R));
                col.AddRange(codes[11].ToList(Encodings.R));
                col.AddRange(codes[12].ToList(Encodings.R));
                col.Add(true);
                col.Add(false);
                col.Add(true);
                return col;
            }
            else if (codes.Count == 8)
            {
                col.Add(true);
                col.Add(false);
                col.Add(true);
                col.AddRange(codes[0].ToList(Encodings.L));
                col.AddRange(codes[1].ToList(Encodings.L));
                col.AddRange(codes[2].ToList(Encodings.L));
                col.AddRange(codes[3].ToList(Encodings.L));
                col.Add(false);
                col.Add(true);
                col.Add(false);
                col.Add(true);
                col.Add(false);
                col.AddRange(codes[4].ToList(Encodings.R));
                col.AddRange(codes[5].ToList(Encodings.R));
                col.AddRange(codes[6].ToList(Encodings.R));
                col.AddRange(codes[7].ToList(Encodings.R));
                col.Add(true);
                col.Add(false);
                col.Add(true);
                return col;
            }
            else if (codes.Count == 5)
            {
                var checksum = Checksum();
                col.Add(false);
                col.Add(true);
                col.Add(false);
                col.Add(true);
                col.Add(true);
                col.AddRange(codes[0].ToList(checksum < 4 ? Encodings.G : Encodings.L));
                col.Add(false);
                col.Add(true);
                col.AddRange(codes[1].ToList(checksum == 0 || checksum == 4 || checksum == 7 || checksum == 8 ? Encodings.G : Encodings.L));
                col.Add(false);
                col.Add(true);
                col.AddRange(codes[2].ToList(checksum == 1 || checksum == 4 || checksum == 5 || checksum == 9 ? Encodings.G : Encodings.L));
                col.Add(false);
                col.Add(true);
                col.AddRange(codes[3].ToList(checksum == 2 || checksum == 5 || checksum == 6 || checksum == 7 ? Encodings.G : Encodings.L));
                col.Add(false);
                col.Add(true);
                col.AddRange(codes[4].ToList(checksum == 3 || checksum == 6 || checksum == 8 || checksum == 9 ? Encodings.G : Encodings.L));
                return col;
            }
            else if (codes.Count == 2)
            {
                var checksum = Checksum();
                col.Add(false);
                col.Add(true);
                col.Add(false);
                col.Add(true);
                col.Add(true);
                col.AddRange(codes[0].ToList(checksum % 4 < 2 ? Encodings.L : Encodings.G));
                col.Add(false);
                col.Add(true);
                col.AddRange(codes[1].ToList(checksum % 2 == 0 ? Encodings.L : Encodings.G));
                return col;
            }

            throw new InvalidOperationException("The count of digit is not any of 7, 8, 12 or 13.");
        }

        /// <summary>
        /// Converts to a string.
        /// </summary>
        /// <returns>The barcode string.</returns>
        /// <exception cref="InvalidOperationException">It was not an EAN-13 ro EAN-8 code.</exception>
        public string ToBarcodeString()
            => string.Join(string.Empty, ToBarcode().Select(ele => ele ? '1' : '0'));

        /// <summary>
        /// Converts to a string.
        /// </summary>
        /// <param name="black">The value of black represented.</param>
        /// <param name="white">The value of white represented.</param>
        /// <returns>The barcode string.</returns>
        /// <exception cref="InvalidOperationException">It was not an EAN-13 ro EAN-8 code.</exception>
        public string ToBarcodeString(char black, char white)
            => string.Join(string.Empty, ToBarcode().Select(ele => ele ? black : white));

        /// <summary>
        /// Converts to a string.
        /// </summary>
        /// <param name="selector">The selector to convert boolean array to a string. Per boolean value, white represented as false, black represented as true.</param>
        /// <returns>The barcode string.</returns>
        /// <exception cref="InvalidOperationException">It was not an EAN-13 ro EAN-8 code.</exception>
        public string ToBarcodeString(Func<bool, char> selector)
            => selector != null ? string.Join(string.Empty, ToBarcode().Select(selector)) : ToBarcodeString();

        /// <summary>
        /// Converts to a string.
        /// </summary>
        /// <param name="selector">The selector to convert boolean array to a string. Per boolean value, white represented as false, black represented as true.</param>
        /// <returns>The barcode string.</returns>
        /// <exception cref="InvalidOperationException">It was not an EAN-13 ro EAN-8 code.</exception>
        public string ToBarcodeString(Func<bool, int, char> selector)
            => selector != null ? string.Join(string.Empty, ToBarcode().Select(selector)) : ToBarcodeString();

        /// <summary>
        /// Creates an EAN.
        /// </summary>
        /// <param name="digits">The digits.</param>
        /// <returns>An instance of the EAN.</returns>
        /// <exception cref="ArgumentNullException">The input sequence was null.</exception>
        /// <exception cref="InvalidOperationException">The digits was not valid.</exception>
        public static InternationalArticleNumber Create(params byte[] digits)
        {
            if (digits.Length < 6)
            {
                foreach (var d in digits)
                {
                    if (d > 9) throw new InvalidOperationException("The digit should be less than 10.");
                }

                if (digits.Length == 5 || digits.Length == 2)
                    return new InternationalArticleNumber
                    {
                        value = string.Join(string.Empty, digits)
                    };

                throw new InvalidOperationException("The count of digit is too less.");
            }

            var check = Checksum(digits);
            var col = digits.Length switch
            {
                13 => digits[12] == check ? digits.Take(12) : null,
                8 => digits[7] == check ? digits.Take(7) : null,
                18 => digits[17] == check ? digits.Take(17) : null,
                _ => digits
            };
            if (col == null)
                throw new InvalidOperationException($"Check failed. Expects {check} but {digits.LastOrDefault()}.");
            return new InternationalArticleNumber
            {
                value = string.Join(string.Empty, col) + check
            };
        }

        /// <summary>
        /// Creates an EAN.
        /// </summary>
        /// <param name="digits">The digits.</param>
        /// <returns>An instance of the EAN.</returns>
        /// <exception cref="ArgumentNullException">The input sequence was null.</exception>
        /// <exception cref="InvalidOperationException">The digits was not valid.</exception>
        public static InternationalArticleNumber Create(IEnumerable<byte> digits)
            => Create(digits?.ToArray());

        /// <summary>
        /// Creates an EAN.
        /// </summary>
        /// <param name="sequence">The EAN digits; or barcode areas that white represented as 0 and black represented as 1.</param>
        /// <returns>An instance of the EAN.</returns>
        /// <exception cref="ArgumentNullException">The input sequence was null.</exception>
        /// <exception cref="InvalidOperationException">The digits was not valid.</exception>
        public static InternationalArticleNumber Create(string sequence)
        {
            if (sequence == null) throw new ArgumentNullException(nameof(sequence), "sequence should not be null.");
            sequence = sequence.Trim();
            if (sequence.Length < 21 || sequence.Contains('9') || sequence.Replace("0", string.Empty).Replace("1", string.Empty).Replace("-", string.Empty).Trim().Length > 0)
                return Create(ToList(sequence));
            var col = new List<bool>();
            foreach (var c in sequence)
            {
                if (c == '0') col.Add(false);
                else if (c == '1') col.Add(true);
            }

            return Create(col);
        }

        /// <summary>
        /// Creates an EAN.
        /// </summary>
        /// <param name="barcode">The barcode. White represented as false, black represented as true.</param>
        /// <returns>An instance of the EAN.</returns>
        /// <exception cref="ArgumentNullException">The barcode was null.</exception>
        /// <exception cref="InvalidOperationException">The barcode was not valid.</exception>
        public static InternationalArticleNumber Create(IEnumerable<bool> barcode)
        {
#pragma warning disable IDE0056
            if (barcode == null) throw new ArgumentNullException(nameof(barcode), "barcode should not be null.");
            var col = barcode?.ToList();
            var sb = new StringBuilder();
            if (col.Count < 49)
            {
                if (col.Count != 21 && col.Count != 48)
                    throw new InvalidOperationException("The count of barcode area is too less.");
                if (col[0] || !col[1] || col[2] || !col[3] || !col[4])
                    throw new InvalidOperationException("The start marker is not invalid.");
                for (var i = 5; i < col.Count; i += 9)
                {
                    var n = GetLeftDigitNumber(col, i, out _);
                    sb.Append(n);
                }

                return Create(sb.ToString());
            }

            if (!col[0] || col[1] || !col[2] || col[3])
                throw new InvalidOperationException("The start marker is not invalid.");
            if (!col[col.Count - 1] || col[col.Count - 2] || !col[col.Count - 3] || col[col.Count - 4])
                throw new InvalidOperationException("The end marker is not invalid.");
            if (col.Count == 95)
            {
                {
                    var n = GetLeftDigitNumber(col, 3, out var e);
                    if (e == Encodings.G)
                    {
                        col.Reverse();
                        n = GetLeftDigitNumber(col, 3, out e);
                    }

                    if (e != Encodings.L) throw new InvalidOperationException("The format of the first digit is invalid.");
                    sb.Append(n);
                }

                var left = new List<bool> { false };
                for (var i = 10; i < 45; i += 7)
                {
                    var n = GetLeftDigitNumber(col, i, out var e);
                    left.Add(e == Encodings.G);
                    sb.Append(n);
                }

                sb.Insert(0, GetFirstDigitNumber(left));
                for (var i = 50; i < 92; i += 7)
                {
                    var n = GetRightDigitNumber(col, i);
                    sb.Append(n);
                }
            }
            else if (col.Count == 67)
            {
                {
                    var n = GetLeftDigitNumber(col, 3, out var e);
                    if (e == Encodings.G)
                    {
                        col.Reverse();
                        n = GetLeftDigitNumber(col, 3, out e);
                    }

                    if (e != Encodings.L) throw new InvalidOperationException("The format of the first digit is invalid.");
                    sb.Append(n);
                }

                for (var i = 10; i < 31; i += 7)
                {
                    var n = GetLeftDigitNumber(col, i, out _);
                    sb.Append(n);
                }

                for (var i = 36; i < 64; i += 7)
                {
                    var n = GetRightDigitNumber(col, i);
                    sb.Append(n);
                }
            }
            else if (col.Count == 84)
            {
                {
                    var n = GetLeftDigitNumber(col, 0, out var e);
                    if (e == Encodings.G)
                    {
                        col.Reverse();
                        n = GetLeftDigitNumber(col, 0, out e);
                    }

                    if (e != Encodings.L) throw new InvalidOperationException("The format of the first digit is invalid.");
                    sb.Append(n);
                }

                var left = new List<bool> { false };
                for (var i = 7; i < 42; i += 7)
                {
                    var n = GetLeftDigitNumber(col, i, out var e);
                    left.Add(e == Encodings.G);
                    sb.Append(n);
                }

                sb.Insert(0, GetFirstDigitNumber(left));
                for (var i = 42; i < 84; i += 7)
                {
                    var n = GetRightDigitNumber(col, i);
                    sb.Append(n);
                }
            }
            else
            {
                throw new InvalidOperationException("The count of barcode is invalid. Should be 95 or 67.");
            }

            return Create(sb.ToString());
#pragma warning restore IDE0056
        }

        /// <summary>
        /// Gets the checksum.
        /// </summary>
        /// <param name="digits">The digits.</param>
        /// <returns>The checksum.</returns>
        /// <exception cref="ArgumentNullException">The input sequence was null.</exception>
        /// <exception cref="InvalidOperationException">The digits was not valid.</exception>
        public static byte Checksum(params byte[] digits)
        {
            if (digits == null) throw new ArgumentNullException(nameof(digits), "digits should not be null.");
            foreach (var d in digits)
            {
                if (d > 9) throw new InvalidOperationException("The digit should be less than 10.");
            }

            return (byte)(digits.Length switch
            {
                12 or 13 => (10 - ((digits[0] + digits[2] + digits[4] + digits[6] + digits[8] + digits[10] + (digits[1] + digits[3] + digits[5] + digits[7] + digits[9] + digits[11]) * 3) % 10)) % 10,
                7 or 8 => (10 - ((digits[1] + digits[3] + digits[5] + (digits[0] + digits[2] + digits[4] + digits[6]) * 3) % 10)) % 10,
                17 or 18 => (10 - ((digits[1] + digits[3] + digits[5] + digits[7] + digits[9] + digits[11] + digits[13] + digits[15] + (digits[0] + digits[2] + digits[4] + digits[6] + digits[8] + digits[10] + digits[12] + digits[14] + digits[16]) * 3) % 10)) % 10,
                5 => ((digits[0] + digits[2] + digits[4]) * 3 + (digits[1] + digits[3]) * 9) % 10,
                2 => (digits[0] * 2 + digits[1]) % 4,
                _ => throw new InvalidOperationException("The count of digit is not any of 7, 8, 12 or 13.")
            });
        }

        /// <summary>
        /// Gets the checksum.
        /// </summary>
        /// <param name="digits">The digits.</param>
        /// <returns>The checksum.</returns>
        /// <exception cref="ArgumentNullException">The input sequence was null.</exception>
        /// <exception cref="InvalidOperationException">The digits was not valid.</exception>
        public static byte Checksum(IEnumerable<byte> digits)
            => Checksum(digits?.ToArray());

        /// <summary>
        /// Gets the checksum.
        /// </summary>
        /// <param name="sequence">The EAN digits.</param>
        /// <returns>The checksum.</returns>
        /// <exception cref="ArgumentNullException">The input sequence was null.</exception>
        /// <exception cref="InvalidOperationException">The input sequence was not valid.</exception>
        public static byte Checksum(string sequence)
            => Checksum(ToList(sequence));

        /// <summary>
        /// Validates the checksum.
        /// </summary>
        /// <param name="digits">The digits.</param>
        /// <returns>true if checksum is correct; otherwise, false.</returns>
        public static bool Validate(params byte[] digits)
            => digits != null && digits.Length switch
            {
                13 => (10 - ((digits[0] + digits[2] + digits[4] + digits[6] + digits[8] + digits[10] + (digits[1] + digits[3] + digits[5] + digits[7] + digits[9] + digits[11]) * 3) % 10)) % 10 == digits[12],
                8 => (10 - ((digits[1] + digits[3] + digits[5] + (digits[0] + digits[2] + digits[4] + digits[6]) * 3) % 10)) % 10 == digits[7],
                18 => (10 - ((digits[1] + digits[3] + digits[5] + digits[7] + digits[9] + digits[11] + digits[13] + digits[15] + (digits[0] + digits[2] + digits[4] + digits[6] + digits[8] + digits[10] + digits[12] + digits[14] + digits[16]) * 3) % 10)) % 10 == digits[17],
                5 or 2 => true,
                _ => false
            };

        /// <summary>
        /// Validates the checksum.
        /// </summary>
        /// <param name="digits">The digits.</param>
        /// <returns>true if checksum is correct; otherwise, false.</returns>
        public static bool Validate(IEnumerable<byte> digits)
            => Validate(digits?.ToArray());

        /// <summary>
        /// Validates the checksum.
        /// </summary>
        /// <param name="sequence">The EAN digits.</param>
        /// <returns>true if checksum is correct; otherwise, false.</returns>
        public static bool Validate(string sequence)
        {
            if (string.IsNullOrEmpty(sequence)) return false;
            try
            {
                return Validate(ToList(sequence));
            }
            catch (InvalidOperationException)
            {
            }

            return false;
        }

        private static byte GetFirstDigitNumber(List<bool> col)
            => string.Join(string.Empty, col.Select(ele => ele ? 'G' : 'L')) switch
            {
                "LLLLLL" => 0,
                "LLGLGG" => 1,
                "LLGGLG" => 2,
                "LLGGGL" => 3,
                "LGLLGG" => 4,
                "LGGLLG" => 5,
                "LGGGLL" => 6,
                "LGLGLG" => 7,
                "LGLGGL" => 8,
                "LGGLGL" => 9,
                _ => throw new InvalidOperationException("The encoding of first group is invalid.")
            };

        private static byte GetLeftDigitNumber(List<bool> col, int i, out Encodings encoding)
        {
            if (col[i]) throw new InvalidOperationException($"Position {i} should be white (false or 0) but is black in fact.");
            if (!col[i + 6]) throw new InvalidOperationException($"Position {i + 6} should be black (true or 1) but is white in fact.");
            var b = (byte)(0b1
                + (col[i + 1] ? 0b100000 : 0)
                + (col[i + 2] ? 0b10000 : 0)
                + (col[i + 3] ? 0b1000 : 0)
                + (col[i + 4] ? 0b100 : 0)
                + (col[i + 5] ? 0b10 : 0));
            try
            {
                return GetDigitNumber(b, out encoding);
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException($"Code {Code.ToString(b)} at position {i} is not a valid digit.");
            }
        }

        private static byte GetRightDigitNumber(List<bool> col, int i)
        {
            if (!col[i]) throw new InvalidOperationException($"Position {i} should be black (true or 1) but is white in fact.");
            if (col[i + 6]) throw new InvalidOperationException($"Position {i + 6} should be white (false or 0) but is black in fact.");
            var b = (byte)(0b1000000
                + (col[i + 1] ? 0b100000 : 0)
                + (col[i + 2] ? 0b10000 : 0)
                + (col[i + 3] ? 0b1000 : 0)
                + (col[i + 4] ? 0b100 : 0)
                + (col[i + 5] ? 0b10 : 0));
            byte r;
            Encodings encoding;
            try
            {
                r = GetDigitNumber(b, out encoding);
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException($"Code {Code.ToString(b)} at position {i} is not a valid digit.");
            }

            if (encoding != Encodings.R)
                throw new InvalidOperationException($"Code {Code.ToString(b)} ({r}) at position {i} is not R-column.");
            return r;
        }

        /// <summary>
        /// Creates a byte list.
        /// </summary>
        /// <param name="sequence">The EAN digits.</param>
        /// <returns>The byte list.</returns>
        /// <exception cref="ArgumentNullException">The input sequence was null.</exception>
        /// <exception cref="InvalidOperationException">The input sequence was not valid.</exception>
        private static List<byte> ToList(string sequence)
        {
            if (sequence == null) throw new ArgumentNullException(nameof(sequence), "sequence should not be null.");
            var arr = new List<byte>();
            var i = -1;
            foreach (var c in sequence)
            {
                i++;
                switch (c)
                {
                    case '-':
                    case ' ':
                    case '(':
                    case ')':
                        continue;
                    case '0':
                        arr.Add(0);
                        break;
                    case '1':
                        arr.Add(1);
                        break;
                    case '2':
                        arr.Add(2);
                        break;
                    case '3':
                        arr.Add(3);
                        break;
                    case '4':
                        arr.Add(4);
                        break;
                    case '5':
                        arr.Add(5);
                        break;
                    case '6':
                        arr.Add(6);
                        break;
                    case '7':
                        arr.Add(7);
                        break;
                    case '8':
                        arr.Add(8);
                        break;
                    case '9':
                        arr.Add(9);
                        break;
                    default:
                        throw new InvalidOperationException(
                            $"The input sequence is not valid. Character at index {i} is {c} but expects a digit.",
                            new ArgumentException("sequence format is not valid.", nameof(sequence)));
                }
            }

            return arr;
        }
    }
}
