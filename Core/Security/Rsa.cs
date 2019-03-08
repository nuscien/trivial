using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Trivial.Collection;

namespace Trivial.Security
{
    /// <summary>
    /// RSA utility.
    /// </summary>
    public static class RSAUtility
    {
        /// <summary>
        /// Parses the OpenSSL RSA key.
        /// </summary>
        /// <param name="key">The OpenSSL RSA key.</param>
        /// <returns>The RSA parameters; or null, if parse failed.</returns>
        public static RSAParameters? ParseOpenSSLKey(string key)
        {
            var lines = key.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length == 0) return null;
            var isPrivate = true;
            if (lines.Length == 1)
            {
                key = lines[0];
            }
            else
            {
                isPrivate = lines[0].IndexOf(" PRIVATE KEY-") > 0;
                key = lines.Length == 2 ? lines[1] : string.Join(string.Empty, lines.Skip(1).Take(lines.Length - 2));
            }

            if (isPrivate)
            {
                var privateKeyBits = Convert.FromBase64String(key);
                var parameters = new RSAParameters();
                using (var reader = new BinaryReader(new MemoryStream(privateKeyBits)))
                {
                    byte bt = 0;
                    ushort twobytes = 0;
                    twobytes = reader.ReadUInt16();
                    if (twobytes == 0x8130)
                        reader.ReadByte();
                    else if (twobytes == 0x8230)
                        reader.ReadInt16();
                    else
                        return null;

                    twobytes = reader.ReadUInt16();
                    if (twobytes != 0x0102)
                        return null;

                    bt = reader.ReadByte();
                    if (bt != 0x00)
                        return null;

                    parameters.Modulus = reader.ReadBytes(GetIntegerSize(reader));
                    parameters.Exponent = reader.ReadBytes(GetIntegerSize(reader));
                    parameters.D = reader.ReadBytes(GetIntegerSize(reader));
                    parameters.P = reader.ReadBytes(GetIntegerSize(reader));
                    parameters.Q = reader.ReadBytes(GetIntegerSize(reader));
                    parameters.DP = reader.ReadBytes(GetIntegerSize(reader));
                    parameters.DQ = reader.ReadBytes(GetIntegerSize(reader));
                    parameters.InverseQ = reader.ReadBytes(GetIntegerSize(reader));
                }

                return parameters;
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
                    using (var reader = new BinaryReader(stream))  // Wrap memory stream with BinaryReader for easy reading.
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
    }
}
