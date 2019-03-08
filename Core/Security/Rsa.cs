using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;

using Trivial.Collection;
using Trivial.Text;

namespace Trivial.Security
{
    /// <summary>
    /// RSA utility.
    /// </summary>
    public static class RSAUtility
    {
        /// <summary>
        /// Encoded OID sequence for PKCS #1 rsaEncryption szOID_RSA_RSA = "1.2.840.113549.1.1.1".
        /// </summary>
        private static readonly byte[] seqOID = new byte[] { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };

        /// <summary>
        /// PEM versions.
        /// </summary>
        private static readonly byte[] verPem = new byte[] { 0x02, 0x01, 0x00 };

        /// <summary>
        /// Parses the parameters from OpenSSL RSA key (PEM Base64) or the RSA parameters XML string.
        /// </summary>
        /// <param name="key">The OpenSSL RSA key string (PEM Base64) or the RSA parameters XML string.</param>
        /// <returns>The RSA parameters; or null, if parse failed.</returns>
        public static RSAParameters? ParseParameters(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return null;
            key = key.Trim();
            if (key.IndexOf("<") == 0 && key.LastIndexOf(">") == key.Length - 1)
            {
                var xml = XElement.Parse(key);
                if (xml?.Name?.LocalName != "RSAKeyValue") return null;
                var p = new RSAParameters();
                foreach (var ele in xml.Elements())
                {
                    var chars = Convert.FromBase64String(ele.Value);
                    switch (ele.Name?.LocalName)
                    {
                        case "Modulus":
                            p.Modulus = chars;
                            break;
                        case "Exponent":
                            p.Exponent = chars;
                            break;
                        case "P":
                            p.P = chars;
                            break;
                        case "Q":
                            p.Q = chars;
                            break;
                        case "DP":
                            p.DP = chars;
                            break;
                        case "DQ":
                            p.DQ = chars;
                            break;
                        case "InverseQ":
                            p.InverseQ = chars;
                            break;
                        case "D":
                            p.D = chars;
                            break;
                    }
                }

                if (p.Modulus == null || p.Modulus.Length == 0 || p.Exponent == null || p.Exponent.Length == 0) return null;
                return p;
            }

            var lines = key.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length == 0) return null;
            var isPrivate = true;
            if (lines.Length == 1)
            {
                key = lines[0];
            }
            else if (lines.Length == 2)
            {
                isPrivate = lines[0].IndexOf("PRIVATE KEY") > 0;
                key = lines[1];
            }
            else
            {
                isPrivate = lines[0].IndexOf(" PRIVATE KEY-") > 0;
                key = string.Join(string.Empty, lines.Skip(1).Take(lines.Length - 2));
            }

            if (isPrivate)
            {
                var privateKeyBits = Convert.FromBase64String(key);
                using (var reader = new BinaryReader(new MemoryStream(privateKeyBits)))
                {
                    var twoBytes = reader.ReadUInt16();
                    if (twoBytes == 0x8130) reader.ReadByte();
                    else if (twoBytes == 0x8230) reader.ReadInt16();
                    else return null;
                    twoBytes = reader.ReadUInt16();
                    if (twoBytes != 0x0102) return null;
                    var testByte = reader.ReadByte();
                    if (testByte != 0x00) return null;
                    return new RSAParameters
                    {
                        Modulus = reader.ReadBytes(GetIntegerSize(reader)),
                        Exponent = reader.ReadBytes(GetIntegerSize(reader)),
                        D = reader.ReadBytes(GetIntegerSize(reader)),
                        P = reader.ReadBytes(GetIntegerSize(reader)),
                        Q = reader.ReadBytes(GetIntegerSize(reader)),
                        DP = reader.ReadBytes(GetIntegerSize(reader)),
                        DQ = reader.ReadBytes(GetIntegerSize(reader)),
                        InverseQ = reader.ReadBytes(GetIntegerSize(reader))
                    };
                }
            }
            else
            {
                var x509key = Convert.FromBase64String(key);
                var x509size = x509key.Length;

                // Set up stream to read the asn.1 encoded SubjectPublicKeyInfo blob.
                using (var stream = new MemoryStream(x509key))
                {
                    using (var reader = new BinaryReader(stream))  // Wrap in-memory stream with BinaryReader for easy reading.
                    {
                        var twoBytes = reader.ReadUInt16();
                        if (twoBytes == 0x8130) // Data read as little endian order (actual data order for Sequence is 30 81).
                            reader.ReadByte();  // Advance 1 byte.
                        else if (twoBytes == 0x8230)
                            reader.ReadInt16(); // Advance 2 bytes.
                        else
                            return null;

                        var seq = reader.ReadBytes(15); // Read the Sequence OID.
                        if (!ListUtility.Equals(seq, seqOID))   // Make sure Sequence for OID is correct.
                            return null;

                        twoBytes = reader.ReadUInt16();
                        if (twoBytes == 0x8103) // Data read as little endian order (actual data order for Bit String is 03 81).
                            reader.ReadByte();  // Advance 1 byte.
                        else if (twoBytes == 0x8203)
                            reader.ReadInt16(); // Advance 2 bytes.
                        else
                            return null;

                        var testByte = reader.ReadByte();
                        if (testByte != 0x00) // Expect null byte next.
                            return null;

                        twoBytes = reader.ReadUInt16();
                        if (twoBytes == 0x8130) // Data read as little endian order (actual data order for Sequence is 30 81).
                            reader.ReadByte();  // Advance 1 byte.
                        else if (twoBytes == 0x8230)
                            reader.ReadInt16(); // Advance 2 bytes.
                        else
                            return null;

                        twoBytes = reader.ReadUInt16();
                        byte lowByte = 0x00;
                        byte highByte = 0x00;

                        if (twoBytes == 0x8102) // Data read as little endian order (actual data order for Integer is 02 81).
                        {
                            lowByte = reader.ReadByte();    // Read next bytes which is bytes in modulus.
                        }
                        else if (twoBytes == 0x8202)
                        {
                            highByte = reader.ReadByte();   // Advance 2 bytes.
                            lowByte = reader.ReadByte();
                        }
                        else
                        {
                            return null;
                        }

                        byte[] modInt = { lowByte, highByte, 0x00, 0x00 };   // Reverse byte order since asn.1 key uses big endian order.
                        var modsize = BitConverter.ToInt32(modInt, 0);
                        var firstByte = reader.PeekChar();
                        if (firstByte == 0x00)
                        {   // Don't include it if the first byte (highest order) of modulus is zero.
                            reader.ReadByte();  // Skip this null byte.
                            modsize -= 1;   // Reduce modulus buffer size by 1.
                        }

                        var modulus = reader.ReadBytes(modsize);    // Read the modulus bytes.
                        if (reader.ReadByte() != 0x02)  // Expect an Integer for the exponent data.
                            return null;
                        var expBytes = (int)reader.ReadByte();  // Should only need one byte for actual exponent data (for all useful values).
                        var exponent = reader.ReadBytes(expBytes);

                        // Create RSACryptoServiceProvider instance and initialize with public key.
                        return new RSAParameters
                        {
                            Modulus = modulus,
                            Exponent = exponent
                        };
                    }
                }
            }
        }

        /// <summary>
        /// Converts the RSA parameters to an XElement object.
        /// </summary>
        /// <param name="parameters">The RSA parameters.</param>
        /// <param name="includePrivateKey">true if includes the private key; otherwise, false.</param>
        /// <returns>An XElement object.</returns>
        public static XElement ToXElement(this RSAParameters parameters, bool includePrivateKey)
        {
            var xml = new XElement("RSAKeyValue",
                new XElement("Modulus") { Value = Convert.ToBase64String(parameters.Modulus) },
                new XElement("Exponent") { Value = Convert.ToBase64String(parameters.Exponent) });
            if (includePrivateKey) xml.Add(
                new XElement("P") { Value = Convert.ToBase64String(parameters.P) },
                new XElement("Q") { Value = Convert.ToBase64String(parameters.Q) },
                new XElement("DP") { Value = Convert.ToBase64String(parameters.DP) },
                new XElement("DQ") { Value = Convert.ToBase64String(parameters.DQ) },
                new XElement("P") { Value = Convert.ToBase64String(parameters.P) },
                new XElement("InverseQ") { Value = Convert.ToBase64String(parameters.InverseQ) },
                new XElement("D") { Value = Convert.ToBase64String(parameters.D) });
            return xml;
        }

        /// <summary>
        /// Converts the RSA parameter to OpenSSL RSA private key format (PEM Base64).
        /// </summary>
        /// <param name="parameters">The RSA parameters.</param>
        /// <param name="usePKCS8">true if use PKCS8; otherwise, false.</param>
        /// <returns>The PEM.</returns>
        public static string ToPrivatePEM(this RSAParameters parameters, bool usePKCS8)
        {
            using (var stream = new MemoryStream())
            {
                // Wrtie total length and version.
                stream.WriteByte(0x30);
                var index1 = (int)stream.Length;
                stream.Write(verPem, 0, verPem.Length);
                int index2 = -1, index3 = -1;

                // PKCS8 only.
                if (usePKCS8)
                {
                    stream.Write(seqOID, 0, seqOID.Length);
                    stream.WriteByte(0x04);
                    index2 = (int)stream.Length;
                    stream.WriteByte(0x30);
                    index3 = (int)stream.Length;
                    stream.Write(verPem, 0, verPem.Length);
                }

                // Write data.
                WriteBlock(stream, parameters.Modulus);
                WriteBlock(stream, parameters.Exponent);
                WriteBlock(stream, parameters.D);
                WriteBlock(stream, parameters.P);
                WriteBlock(stream, parameters.Q);
                WriteBlock(stream, parameters.DP);
                WriteBlock(stream, parameters.DQ);
                WriteBlock(stream, parameters.InverseQ);

                // Calculate blanks.
                var bytes = stream.ToArray();
                if (index2 != -1)
                {
                    bytes = WriteLen(stream, index3, bytes);
                    bytes = WriteLen(stream, index2, bytes);
                }

                // Flag.
                bytes = WriteLen(stream, index1, bytes);
                var flag = " PRIVATE KEY";
                if (!usePKCS8) flag = " RSA" + flag;

                // Return Pem.
                return "-----BEGIN" + flag + "-----\n" + StringUtility.BreakLines(Convert.ToBase64String(bytes), 64) + "\n-----END" + flag + "-----";
            }
        }

        /// <summary>
        /// Converts the RSA parameter to OpenSSL RSA public key format (PEM Base64).
        /// </summary>
        /// <param name="parameters">The RSA parameters.</param>
        /// <returns>The PEM.</returns>
        public static string ToPublicPEM(this RSAParameters parameters)
        {
            using (var stream = new MemoryStream())
            {
                stream.WriteByte(0x30);
                var index1 = (int)stream.Length;
                stream.Write(seqOID, 0, seqOID.Length);

                // Read length after 0x00.
                stream.WriteByte(0x03);
                var index2 = (int)stream.Length;
                stream.WriteByte(0x00);

                // Rest content length.
                stream.WriteByte(0x30);

                // Write modulus and exponent.
                var index3 = (int)stream.Length;
                WriteBlock(stream, parameters.Modulus);
                WriteBlock(stream, parameters.Exponent);
                var bytes = stream.ToArray();
                bytes = WriteLen(stream, index3, bytes);
                bytes = WriteLen(stream, index2, bytes);
                bytes = WriteLen(stream, index1, bytes);

                // Return PEM.
                return "-----BEGIN PUBLIC KEY-----\n" + StringUtility.BreakLines(Convert.ToBase64String(bytes), 64) + "\n-----END PUBLIC KEY-----";
            }
        }

        /// <summary>
        /// Encrypts the input string using the specified encoding and padding mode.
        /// </summary>
        /// <param name="rsa">The RSA algorithm instance.</param>
        /// <param name="data">The string data to encrypt.</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="padding">The padding mode.</param>
        /// <returns></returns>
        public static byte[] Encrypt(this RSA rsa, string data, Encoding encoding, RSAEncryptionPadding padding)
        {
            if (data == null) return null;
            if (rsa == null) throw new ArgumentNullException(nameof(rsa), "rsa should not be null.");
            if (encoding == null) throw new ArgumentNullException(nameof(encoding), "encoding should not be null.");
            return rsa.Encrypt(encoding.GetBytes(data), padding);
        }

        /// <summary>
        /// Encrypts the input string using the specified encoding and padding mode.
        /// </summary>
        /// <param name="rsa">The RSA algorithm instance.</param>
        /// <param name="data">The string data to encrypt.</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="padding">The padding mode.</param>
        /// <returns></returns>
        public static byte[] Encrypt(this RSA rsa, SecureString data, Encoding encoding, RSAEncryptionPadding padding)
        {
            if (data == null || data.Length == 0) return null;
            if (rsa == null) throw new ArgumentNullException(nameof(rsa), "rsa should not be null.");
            if (encoding == null) throw new ArgumentNullException(nameof(encoding), "encoding should not be null.");
            return rsa.Encrypt(encoding.GetBytes(SecureStringUtility.ToUnsecureString(data)), padding);
        }

        /// <summary>
        /// Decrypts the input string using the specified encoding and padding mode.
        /// </summary>
        /// <param name="rsa">The RSA algorithm instance.</param>
        /// <param name="data">The data to decrypt.</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="padding">The padding mode.</param>
        /// <returns></returns>
        public static string Decrypt(this RSA rsa, byte[] data, Encoding encoding, RSAEncryptionPadding padding)
        {
            if (data == null) return null;
            if (rsa == null) throw new ArgumentNullException(nameof(rsa), "rsa should not be null.");
            if (encoding == null) throw new ArgumentNullException(nameof(encoding), "encoding should not be null.");
            return encoding.GetString(rsa.Decrypt(data, padding));
        }

        private static int GetIntegerSize(BinaryReader reader)
        {
            var count = 0;
            var testByte = reader.ReadByte();
            if (testByte != 0x02) return 0;
            testByte = reader.ReadByte();
            if (testByte == 0x81)
            {
                count = reader.ReadByte();
            }
            else if (testByte == 0x82)
            {
                var highByte = reader.ReadByte();
                var lowByte = reader.ReadByte();
                byte[] modInt = { lowByte, highByte, 0x00, 0x00 };
                count = BitConverter.ToInt32(modInt, 0);
            }
            else
            {
                count = testByte;
            }

            while (reader.ReadByte() == 0x00)
            {
                count -= 1;
            }

            reader.BaseStream.Seek(-1, SeekOrigin.Current);
            return count;
        }

        private static void WriteLenByte(MemoryStream stream, int len)
        {
            if (len < 0x80)
            {
                stream.WriteByte((byte)len);
            }
            else if (len <= 0xff)
            {
                stream.WriteByte(0x81);
                stream.WriteByte((byte)len);
            }
            else
            {
                stream.WriteByte(0x82);
                stream.WriteByte((byte)(len >> 8 & 0xff));
                stream.WriteByte((byte)(len & 0xff));
            }
        }

        private static void WriteBlock(MemoryStream stream, byte[] bytes)
        {
            var addZero = (bytes[0] >> 4) >= 0x8;
            stream.WriteByte(0x02);
            var len = bytes.Length + (addZero ? 1 : 0);
            WriteLenByte(stream, len);
            if (addZero)
            {
                stream.WriteByte(0x00);
            }

            stream.Write(bytes, 0, bytes.Length);
        }

        private static byte[] WriteLen(MemoryStream stream, int index, byte[] bytes) {
            var len = bytes.Length - index;
            stream.SetLength(0);
            stream.Write(bytes, 0, index);
            WriteLenByte(stream, len);
            stream.Write(bytes, index, len);
            return stream.ToArray();
        }
    }
}
