using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
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
        private static readonly Regex codePEM = new Regex(@"--+.+?--+|\s+");
        private static readonly byte[] seqOID = new byte[] { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
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
                    byte bt = 0;
                    ushort twobytes = 0;
                    twobytes = reader.ReadUInt16();
                    if (twobytes == 0x8130) reader.ReadByte();
                    else if (twobytes == 0x8230) reader.ReadInt16();
                    else return null;
                    twobytes = reader.ReadUInt16();
                    if (twobytes != 0x0102) return null;
                    bt = reader.ReadByte();
                    if (bt != 0x00) return null;
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
                // Encoded OID sequence for  PKCS #1 rsaEncryption szOID_RSA_RSA = "1.2.840.113549.1.1.1".
                byte[] SeqOID = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
                byte[] x509key;
                byte[] seq = new byte[15];
                int x509size;

                x509key = Convert.FromBase64String(key);
                x509size = x509key.Length;

                // Set up stream to read the asn.1 encoded SubjectPublicKeyInfo blob.
                using (var stream = new MemoryStream(x509key))
                {
                    using (var reader = new BinaryReader(stream))  // Wrap in-memory stream with BinaryReader for easy reading.
                    {
                        byte bt = 0;
                        ushort twobytes = 0;
                        twobytes = reader.ReadUInt16();
                        if (twobytes == 0x8130) // Data read as little endian order (actual data order for Sequence is 30 81).
                            reader.ReadByte();  // Advance 1 byte.
                        else if (twobytes == 0x8230)
                            reader.ReadInt16(); // Advance 2 bytes.
                        else
                            return null;

                        seq = reader.ReadBytes(15); // Read the Sequence OID.
                        if (!ListUtility.Equals(seq, SeqOID))   // Make sure Sequence for OID is correct.
                            return null;

                        twobytes = reader.ReadUInt16();
                        if (twobytes == 0x8103) // Data read as little endian order (actual data order for Bit String is 03 81).
                            reader.ReadByte();  // Advance 1 byte.
                        else if (twobytes == 0x8203)
                            reader.ReadInt16(); // Advance 2 bytes.
                        else
                            return null;

                        bt = reader.ReadByte();
                        if (bt != 0x00) // Expect null byte next.
                            return null;

                        twobytes = reader.ReadUInt16();
                        if (twobytes == 0x8130) // Data read as little endian order (actual data order for Sequence is 30 81).
                            reader.ReadByte();  // Advance 1 byte.
                        else if (twobytes == 0x8230)
                            reader.ReadInt16(); // Advance 2 bytes.
                        else
                            return null;

                        twobytes = reader.ReadUInt16();
                        byte lowbyte = 0x00;
                        byte highbyte = 0x00;

                        if (twobytes == 0x8102) // Data read as little endian order (actual data order for Integer is 02 81).
                        {
                            lowbyte = reader.ReadByte();    // Read next bytes which is bytes in modulus.
                        }
                        else if (twobytes == 0x8202)
                        {
                            highbyte = reader.ReadByte();   // Advance 2 bytes.
                            lowbyte = reader.ReadByte();
                        }
                        else
                        {
                            return null;
                        }

                        byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };   // Reverse byte order since asn.1 key uses big endian order.
                        var modsize = BitConverter.ToInt32(modint, 0);
                        var firstbyte = reader.PeekChar();
                        if (firstbyte == 0x00)
                        {   // Don't include it if the first byte (highest order) of modulus is zero.
                            reader.ReadByte();  // Skip this null byte.
                            modsize -= 1;   // Reduce modulus buffer size by 1.
                        }

                        var modulus = reader.ReadBytes(modsize);    // Read the modulus bytes.
                        if (reader.ReadByte() != 0x02)  // Expect an Integer for the exponent data.
                            return null;
                        var expbytes = (int)reader.ReadByte();  // Should only need one byte for actual exponent data (for all useful values).
                        var exponent = reader.ReadBytes(expbytes);

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
            if (!includePrivateKey) return xml;
            xml.Add(new XElement("P") { Value = Convert.ToBase64String(parameters.P) });
            xml.Add(new XElement("Q") { Value = Convert.ToBase64String(parameters.Q) });
            xml.Add(new XElement("DP") { Value = Convert.ToBase64String(parameters.DP) });
            xml.Add(new XElement("DQ") { Value = Convert.ToBase64String(parameters.DQ) });
            xml.Add(new XElement("P") { Value = Convert.ToBase64String(parameters.P) });
            xml.Add(new XElement("InverseQ") { Value = Convert.ToBase64String(parameters.InverseQ) });
            xml.Add(new XElement("D") { Value = Convert.ToBase64String(parameters.D) });
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
                var byts = stream.ToArray();
                if (index2 != -1)
                {
                    byts = WriteLen(stream, index3, byts);
                    byts = WriteLen(stream, index2, byts);
                }

                // Flag.
                byts = WriteLen(stream, index1, byts);
                var flag = " PRIVATE KEY";
                if (!usePKCS8)
                {
                    flag = " RSA" + flag;
                }

                // Return Pem.
                return "-----BEGIN" + flag + "-----\n" + StringUtility.BreakLines(Convert.ToBase64String(byts), 64) + "\n-----END" + flag + "-----";
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

                // Encoded OID sequence for PKCS #1 rsaEncryption szOID_RSA_RSA = "1.2.840.113549.1.1.1".
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
                var byts = stream.ToArray();
                byts = WriteLen(stream, index3, byts);
                byts = WriteLen(stream, index2, byts);
                byts = WriteLen(stream, index1, byts);

                // Return PEM.
                return "-----BEGIN PUBLIC KEY-----\n" + StringUtility.BreakLines(Convert.ToBase64String(byts), 64) + "\n-----END PUBLIC KEY-----";
            }
        }

        private static int GetIntegerSize(BinaryReader binr)
        {
            byte bt = 0;
            byte lowbyte = 0x00;
            byte highbyte = 0x00;
            int count = 0;
            bt = binr.ReadByte();
            if (bt != 0x02)
                return 0;
            bt = binr.ReadByte();

            if (bt == 0x81)
                count = binr.ReadByte();
            else
                if (bt == 0x82)
            {
                highbyte = binr.ReadByte();
                lowbyte = binr.ReadByte();
                byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                count = BitConverter.ToInt32(modint, 0);
            }
            else
            {
                count = bt;
            }

            while (binr.ReadByte() == 0x00)
            {
                count -= 1;
            }

            binr.BaseStream.Seek(-1, SeekOrigin.Current);
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
