﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

using Trivial.Collection;
using Trivial.Text;

namespace Trivial.Security;

/// <summary>
/// RSA parameters convert.
/// </summary>
public static class RSAParametersConvert
{
    /// <summary>
    /// Encoded OID sequence for PKCS #1 RSA encryption szOID_RSA_RSA = "1.2.840.113549.1.1.1".
    /// </summary>
    private static readonly byte[] seqOID = [0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00];

    /// <summary>
    /// PEM versions.
    /// </summary>
    private static readonly byte[] verPem = [0x02, 0x01, 0x00];

    /// <summary>
    /// Parses the parameters from OpenSSL RSA key (PEM Base64) or the RSA parameters XML string.
    /// </summary>
    /// <param name="key">The OpenSSL RSA key string (PEM Base64) or the RSA parameters XML string.</param>
    /// <returns>The RSA parameters; or null, if parse failed.</returns>
    public static RSAParameters? Parse(string key)
    {
        if (string.IsNullOrWhiteSpace(key)) return null;
        key = key.Trim();
        if (key.IndexOf("<") == 0 && key.LastIndexOf(">") == key.Length - 1)
        {
            var xml = XElement.Parse(key);
            if (xml == null) return null;
            var p = new RSAParameters();
            foreach (var ele in xml.Elements())
            {
                if (string.IsNullOrWhiteSpace(ele?.Value)) continue;
                var chars = Convert.FromBase64String(ele.Value);
                switch (ele.Name?.LocalName?.ToLowerInvariant())
                {
                    case "modulus":
                        p.Modulus = chars;
                        break;
                    case "exponent":
                        p.Exponent = chars;
                        break;
                    case "p":
                        p.P = chars;
                        break;
                    case "q":
                        p.Q = chars;
                        break;
                    case "dp":
                        p.DP = chars;
                        break;
                    case "dq":
                        p.DQ = chars;
                        break;
                    case "inverseq":
                        p.InverseQ = chars;
                        break;
                    case "d":
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
        var firstLine = lines[0].Trim();
        if (lines.Length == 1)
        {
            key = firstLine;
        }
        else if (firstLine.IndexOf("ssh-rsa ") == 0)
        {
            isPrivate = false;
            key = string.Join(string.Empty, lines).Trim();
            if (key.Length < 16) return null;
            key = key.Substring(8);
            var blankPos = key.IndexOf(' ');
            if (blankPos > 0) key = key.Substring(0, blankPos);
        }
        else if (lines.Length == 2)
        {
            isPrivate = firstLine.IndexOf("PRIVATE KEY") >= 0;
            key = lines[1];
        }
        else
        {
            isPrivate = firstLine.IndexOf(" PRIVATE KEY") > 0 || firstLine.IndexOf("PRIVATE KEY") == 0;
            key = string.Join(string.Empty, lines.Skip(1).Take(lines.Length - 2));
        }

        using var stream = new MemoryStream(Convert.FromBase64String(key));
        using var reader = new BinaryReader(stream);
        if (isPrivate)
        {
            var twoBytes = reader.ReadUInt16();
            if (twoBytes == 0x8130) reader.ReadByte();
            else if (twoBytes == 0x8230) reader.ReadInt16();
            else return null;
            if (!AreSame(reader, verPem)) return null;

            // For PKCS8.
            if (reader.BaseStream.Position + seqOID.Length < reader.BaseStream.Length)
            {
                var isPkcs8 = true;
                var i = 0;
                for (; i < seqOID.Length; i++)
                {
                    if (seqOID[i] == reader.ReadByte()) continue;
                    isPkcs8 = false;
                    break;
                }

                if (isPkcs8)
                {
                    GetIntegerSize(reader, 0x04);
                    GetIntegerSize(reader, 0x30);
                    if (!AreSame(reader, verPem)) return null;
                }
                else
                {
                    i++;
                    reader.BaseStream.Seek(-i, SeekOrigin.Current);
                }
            }

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
        else
        {
            var twoBytes = reader.ReadUInt16();
            if (twoBytes == 0x8130) // Data read as little endian order (actual data order for Sequence is 30 81).
                reader.ReadByte();  // Advance 1 byte.
            else if (twoBytes == 0x8230)
                reader.ReadInt16(); // Advance 2 bytes.
            else
                return null;

            var seq = reader.ReadBytes(15); // Read the Sequence OID.
            if (!ListExtensions.Equals(seq, seqOID))   // Make sure Sequence for OID is correct.
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
            new XElement("InverseQ") { Value = Convert.ToBase64String(parameters.InverseQ) },
            new XElement("D") { Value = Convert.ToBase64String(parameters.D) });
        return xml;
    }

    /// <summary>
    /// Converts the RSA parameters to an XmlDocument object.
    /// </summary>
    /// <param name="parameters">The RSA parameters.</param>
    /// <param name="includePrivateKey">true if includes the private key; otherwise, false.</param>
    /// <returns>An XmlDocument object.</returns>
    public static XmlDocument ToXmlDocument(this RSAParameters parameters, bool includePrivateKey)
    {
        var doc = new XmlDocument();
        doc.LoadXml("<RSAKeyValue></RSAKeyValue>");
        AppendChildWithBase64(doc, "Modulus", parameters.Modulus);
        AppendChildWithBase64(doc, "Exponent", parameters.Exponent);
        if (includePrivateKey) {
            AppendChildWithBase64(doc, "P", parameters.P);
            AppendChildWithBase64(doc, "Q", parameters.Q);
            AppendChildWithBase64(doc, "DP", parameters.DP);
            AppendChildWithBase64(doc, "DQ", parameters.DQ);
            AppendChildWithBase64(doc, "InverseQ", parameters.InverseQ);
            AppendChildWithBase64(doc, "D", parameters.D);
        }

        return doc;
    }

    /// <summary>
    /// Converts the RSA parameter to OpenSSL RSA private key format string (PEM Base64).
    /// </summary>
    /// <param name="parameters">The RSA parameters.</param>
    /// <param name="usePKCS8">true if use Private-Key Information Syntax Standard #8; otherwise, false.</param>
    /// <param name="onlyBase64Value">true if only contains the Base64 value but without the first and last type description lines; otherwise, false.</param>
    /// <returns>The PEM string of private key.</returns>
    public static string ToPrivatePEMString(this RSAParameters parameters, bool usePKCS8, bool onlyBase64Value = false)
    {
        if (parameters.D == null || parameters.D.Length == 0) return ToPublicPEMString(parameters);
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
            var flag = "PRIVATE KEY";
            if (!usePKCS8) flag = "RSA " + flag;

            // Return Pem.
            var result = Convert.ToBase64String(bytes);
            if (onlyBase64Value) return result;
            return $"-----BEGIN {flag}-----\n{StringExtensions.BreakLines(result, 64, '\n')}\n-----END {flag}-----";
        }
    }

    /// <summary>
    /// Converts the RSA parameter to OpenSSL RSA public key format string (PEM Base64).
    /// </summary>
    /// <param name="parameters">The RSA parameters.</param>
    /// <param name="onlyBase64Value">true if only contains the Base64 value but without the first and last type description lines; otherwise, false.</param>
    /// <returns>The PEM string of public key.</returns>
    public static string ToPublicPEMString(this RSAParameters parameters, bool onlyBase64Value = false)
    {
        if (parameters.Modulus == null || parameters.Modulus.Length < 1 || parameters.Exponent == null || parameters.Exponent.Length < 1) return null;
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
            var result = Convert.ToBase64String(bytes);
            if (onlyBase64Value) return result;
            return $"-----BEGIN PUBLIC KEY-----\n{StringExtensions.BreakLines(result, 64, '\n')}\n-----END PUBLIC KEY-----";
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
    public static byte[] Encrypt(this RSA rsa, string data, RSAEncryptionPadding padding, Encoding encoding = null)
    {
        if (data == null) return null;
        if (rsa == null) throw new ArgumentNullException(nameof(rsa), "rsa should not be null.");
        return rsa.Encrypt((encoding ?? Encoding.UTF8).GetBytes(data), padding);
    }

    /// <summary>
    /// Encrypts the input string using the specified encoding and padding mode.
    /// </summary>
    /// <param name="rsa">The RSA algorithm instance.</param>
    /// <param name="data">The string data to encrypt.</param>
    /// <param name="encoding">The encoding.</param>
    /// <param name="padding">The padding mode.</param>
    /// <returns></returns>
    public static byte[] Encrypt(this RSA rsa, SecureString data, RSAEncryptionPadding padding, Encoding encoding = null)
    {
        if (data == null) return null;
        if (rsa == null) throw new ArgumentNullException(nameof(rsa), "rsa should not be null.");
        return rsa.Encrypt((encoding ?? Encoding.UTF8).GetBytes(SecureStringExtensions.ToUnsecureString(data)), padding);
    }

    /// <summary>
    /// Decrypts the input string using the specified encoding and padding mode.
    /// </summary>
    /// <param name="rsa">The RSA algorithm instance.</param>
    /// <param name="data">The data to decrypt.</param>
    /// <param name="padding">The padding mode.</param>
    /// <param name="encoding">The encoding.</param>
    /// <returns></returns>
    public static string DecryptText(this RSA rsa, byte[] data, RSAEncryptionPadding padding, Encoding encoding = null)
    {
        if (data == null) return null;
        if (rsa == null) throw new ArgumentNullException(nameof(rsa), "rsa should not be null.");
        return (encoding ?? Encoding.UTF8).GetString(rsa.Decrypt(data, padding));
    }

    /// <summary>
    /// Decrypts the input string using the specified encoding and padding mode.
    /// </summary>
    /// <param name="rsa">The RSA algorithm instance.</param>
    /// <param name="base64">The Base64 string of the value encrypted.</param>
    /// <param name="padding">The padding mode.</param>
    /// <param name="encoding">The encoding.</param>
    /// <returns></returns>
    public static string DecryptText(this RSA rsa, string base64, RSAEncryptionPadding padding, Encoding encoding = null)
    {
        if (base64 == null) return null;
        if (rsa == null) throw new ArgumentNullException(nameof(rsa), "rsa should not be null.");
        return (encoding ?? Encoding.UTF8).GetString(rsa.Decrypt(Convert.FromBase64String(base64), padding));
    }

    private static bool AreSame(BinaryReader reader, byte[] bytes)
    {
        if (bytes == null) return true;
        if (reader == null) return false;
        foreach (var b in bytes)
        {
            if (reader.Read() != b) return false;
        }

        return true;
    }

    private static int GetIntegerSize(BinaryReader reader, byte firstByte = 0x02)
    {
        int count;
        var testByte = reader.ReadByte();
        if (testByte != firstByte) return 0;
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

    private static XmlElement AppendChildWithText(XmlDocument doc, string name, string value)
    {
        var ele = doc.CreateElement(name);
        doc.AppendChild(ele);
        if (!string.IsNullOrEmpty(value)) ele.AppendChild(doc.CreateTextNode(value));
        return ele;
    }

    private static XmlElement AppendChildWithBase64(XmlDocument doc, string name, byte[] value)
        => AppendChildWithText(doc, name, value != null ? Convert.ToBase64String(value) : null);
}
