using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Text;

namespace Trivial.Data;

/// <summary>
/// Writes one CBOR data item to a writable stream without buffering containers.
/// <para>
/// This class is obsolete and will be removed in a future version. Use the CborWriter class in .NET instead.
/// </para>
/// </summary>
/// <remarks>
/// Arrays and objects use indefinite lengths. The caller owns the stream and is responsible
/// for flushing and disposing it. This writer is not thread-safe and must not be reused after
/// a stream write fails. Unspecified DateTime values are interpreted as UTC.
/// </remarks>
public sealed class SimpleCborStreamWriter
{
    public const string Mime = "application/cbor";

    private static readonly Encoding Utf8 = new UTF8Encoding(false, true);
    private readonly Stream stream;
    private readonly Stack<bool> containers = new();
    private bool hasRoot;

    /// <summary>
    /// Initializes a new instance of the SimpleCborStreamWriter class.
    /// </summary>
    /// <param name="stream">The output stream. Its current position is preserved until writing begins.</param>
    /// <exception cref="ArgumentNullException">The stream is null.</exception>
    /// <exception cref="ArgumentException">The stream is not writable.</exception>
    public SimpleCborStreamWriter(Stream stream)
    {
        if (stream == null) throw new ArgumentNullException(nameof(stream));
        if (!stream.CanWrite) throw new ArgumentException("The stream must be writable.", nameof(stream));
        this.stream = stream;
    }

    /// <summary>
    /// Starts an array at the root or as an array item.
    /// </summary>
    public void StartArray()
    {
        BeginItem();
        StartContainer(false);
    }

    /// <summary>
    /// Starts an array property in the current object.
    /// </summary>
    /// <param name="key">The property name.</param>
    public void StartArray(string key)
    {
        BeginProperty(key);
        StartContainer(false);
    }

    /// <summary>
    /// Ends the current array.
    /// </summary>
    public void EndArray() => EndContainer(false);

    /// <summary>
    /// Starts an object (CBOR map) at the root or as an array item.
    /// </summary>
    public void StartObject()
    {
        BeginItem();
        StartContainer(true);
    }

    /// <summary>
    /// Starts an object property in the current object.
    /// </summary>
    /// <param name="key">The property name.</param>
    public void StartObject(string key)
    {
        BeginProperty(key);
        StartContainer(true);
    }

    /// <summary>
    /// Ends the current object.
    /// </summary>
    public void EndObject() => EndContainer(true);

    /// <summary>
    /// Appends a text property, or null when the value is null.
    /// </summary>
    /// <param name="key">The property name.</param>
    /// <param name="value">The value.</param>
    public void AppendProperty(string key, string value)
    {
        var bytes = value == null ? null : Utf8.GetBytes(value);
        BeginProperty(key);
        WriteString(bytes, 3);
    }

    /// <summary>
    /// Appends a byte string property, or null when the value is null.
    /// </summary>
    /// <param name="key">The property name.</param>
    /// <param name="value">The value.</param>
    public void AppendProperty(string key, byte[] value)
    {
        BeginProperty(key);
        WriteString(value, 2);
    }

    /// <summary>
    /// Appends a Boolean property.
    /// </summary>
    /// <param name="key">The property name.</param>
    /// <param name="value">The value.</param>
    public void AppendProperty(string key, bool value)
    {
        BeginProperty(key);
        stream.WriteByte(value ? (byte)0xf5 : (byte)0xf4);
    }

    /// <summary>
    /// Appends a signed integer property. Smaller signed integer types convert implicitly.
    /// </summary>
    /// <param name="key">The property name.</param>
    /// <param name="value">The value.</param>
    public void AppendProperty(string key, long value)
    {
        BeginProperty(key);
        WriteInteger(value);
    }

    /// <summary>
    /// Appends an unsigned integer property.
    /// </summary>
    /// <param name="key">The property name.</param>
    /// <param name="value">The value.</param>
    public void AppendProperty(string key, ulong value)
    {
        BeginProperty(key);
        WriteHead(0, value);
    }

    /// <summary>
    /// Appends a single-precision floating-point property, including infinity and NaN.
    /// </summary>
    /// <param name="key">The property name.</param>
    /// <param name="value">The value.</param>
    public void AppendProperty(string key, float value)
    {
        BeginProperty(key);
        WriteFloatingPoint(0xfa, BitConverter.GetBytes(value));
    }

    /// <summary>
    /// Appends a double-precision floating-point property, including infinity and NaN.
    /// </summary>
    /// <param name="key">The property name.</param>
    /// <param name="value">The value.</param>
    public void AppendProperty(string key, double value)
    {
        BeginProperty(key);
        WriteFloatingPoint(0xfb, BitConverter.GetBytes(value));
    }

    /// <summary>
    /// Appends an exact decimal fraction property (CBOR tag 4).
    /// </summary>
    /// <param name="key">The property name.</param>
    /// <param name="value">The value.</param>
    public void AppendProperty(string key, decimal value)
    {
        BeginProperty(key);
        WriteDecimal(value);
    }

    /// <summary>
    /// Appends a UTC date/time property (CBOR tag 0). Unspecified values are treated as UTC.
    /// </summary>
    /// <param name="key">The property name.</param>
    /// <param name="value">The value.</param>
    public void AppendProperty(string key, DateTime value)
    {
        var bytes = EncodeDateTime(value);
        BeginProperty(key);
        WriteDateTime(bytes);
    }

    /// <summary>
    /// Appends a date/time property (CBOR tag 0), preserving its offset.
    /// </summary>
    /// <param name="key">The property name.</param>
    /// <param name="value">The value.</param>
    public void AppendProperty(string key, DateTimeOffset value)
    {
        var bytes = Utf8.GetBytes(value.ToString("O", CultureInfo.InvariantCulture));
        BeginProperty(key);
        WriteDateTime(bytes);
    }

    /// <summary>
    /// Appends a null property.
    /// </summary>
    /// <param name="key">The property name.</param>
    public void AppendNullProperty(string key)
    {
        BeginProperty(key);
        stream.WriteByte(0xf6);
    }

    /// <summary>
    /// Appends a text array item or root value, or null when the value is null.
    /// </summary>
    /// <param name="value">The value.</param>
    public void AppendItem(string value)
    {
        var bytes = value == null ? null : Utf8.GetBytes(value);
        BeginItem();
        WriteString(bytes, 3);
    }

    /// <summary>
    /// Appends a byte string array item or root value, or null when the value is null.
    /// </summary>
    /// <param name="value">The value.</param>
    public void AppendItem(byte[] value)
    {
        BeginItem();
        WriteString(value, 2);
    }

    /// <summary>
    /// Appends a Boolean array item or root value.
    /// </summary>
    /// <param name="value">The value.</param>
    public void AppendItem(bool value)
    {
        BeginItem();
        stream.WriteByte(value ? (byte)0xf5 : (byte)0xf4);
    }

    /// <summary>
    /// Appends a signed integer array item or root value. Smaller signed types convert implicitly.
    /// </summary>
    /// <param name="value">The value.</param>
    public void AppendItem(long value)
    {
        BeginItem();
        WriteInteger(value);
    }

    /// <summary>
    /// Appends an unsigned integer array item or root value.
    /// </summary>
    /// <param name="value">The value.</param>
    public void AppendItem(ulong value)
    {
        BeginItem();
        WriteHead(0, value);
    }

    /// <summary>
    /// Appends a single-precision array item or root value, including infinity and NaN.
    /// </summary>
    /// <param name="value">The value.</param>
    public void AppendItem(float value)
    {
        BeginItem();
        WriteFloatingPoint(0xfa, BitConverter.GetBytes(value));
    }

    /// <summary>
    /// Appends a double-precision array item or root value, including infinity and NaN.
    /// </summary>
    /// <param name="value">The value.</param>
    public void AppendItem(double value)
    {
        BeginItem();
        WriteFloatingPoint(0xfb, BitConverter.GetBytes(value));
    }

    /// <summary>
    /// Appends an exact decimal fraction array item or root value (CBOR tag 4).
    /// </summary>
    /// <param name="value">The value.</param>
    public void AppendItem(decimal value)
    {
        BeginItem();
        WriteDecimal(value);
    }

    /// <summary>
    /// Appends a UTC date/time array item or root value (tag 0). Unspecified values are treated as UTC.
    /// </summary>
    /// <param name="value">The value.</param>
    public void AppendItem(DateTime value)
    {
        var bytes = EncodeDateTime(value);
        BeginItem();
        WriteDateTime(bytes);
    }

    /// <summary>
    /// Appends a date/time array item or root value (tag 0), preserving its offset.
    /// </summary>
    /// <param name="value">The value.</param>
    public void AppendItem(DateTimeOffset value)
    {
        var bytes = Utf8.GetBytes(value.ToString("O", CultureInfo.InvariantCulture));
        BeginItem();
        WriteDateTime(bytes);
    }

    /// <summary>
    /// Appends a null array item or root value.
    /// </summary>
    public void AppendNullItem()
    {
        BeginItem();
        stream.WriteByte(0xf6);
    }

    private void BeginItem()
    {
        if (containers.Count > 0)
        {
            if (containers.Peek()) throw new InvalidOperationException("An object value requires a property name.");
        }
        else
        {
            if (hasRoot) throw new InvalidOperationException("Only one root data item may be written.");
            hasRoot = true;
        }
    }

    private void BeginProperty(string key)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));
        if (containers.Count == 0 || !containers.Peek())
            throw new InvalidOperationException("A property requires an open object.");
        WriteString(Utf8.GetBytes(key), 3);
    }

    private void StartContainer(bool isObject)
    {
        stream.WriteByte(isObject ? (byte)0xbf : (byte)0x9f);
        containers.Push(isObject);
    }

    private void EndContainer(bool isObject)
    {
        if (containers.Count == 0 || containers.Peek() != isObject)
            throw new InvalidOperationException("The end method must match the current open container.");
        stream.WriteByte(0xff);
        containers.Pop();
    }

    private void WriteInteger(long value)
    {
        if (value >= 0) WriteHead(0, (ulong)value);
        else WriteHead(1, (ulong)(-(value + 1)));
    }

    private void WriteHead(int majorType, ulong value)
    {
        var prefix = majorType << 5;
        if (value < 24)
        {
            stream.WriteByte((byte)(prefix | (int)value));
            return;
        }

        var size = value <= byte.MaxValue ? 1 : value <= ushort.MaxValue ? 2 : value <= uint.MaxValue ? 4 : 8;
        stream.WriteByte((byte)(prefix | (size == 1 ? 24 : size == 2 ? 25 : size == 4 ? 26 : 27)));
        for (var shift = (size - 1) * 8; shift >= 0; shift -= 8)
            stream.WriteByte(unchecked((byte)(value >> shift)));
    }

    private void WriteString(byte[] bytes, int majorType)
    {
        if (bytes == null)
        {
            stream.WriteByte(0xf6);
            return;
        }

        WriteHead(majorType, (ulong)bytes.Length);
        stream.Write(bytes, 0, bytes.Length);
    }

    private void WriteFloatingPoint(byte prefix, byte[] bytes)
    {
        if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
        stream.WriteByte(prefix);
        stream.Write(bytes, 0, bytes.Length);
    }

    private static byte[] EncodeDateTime(DateTime value)
    {
        value = value.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(value, DateTimeKind.Utc)
            : value.ToUniversalTime();
        return Utf8.GetBytes(value.ToString("O", CultureInfo.InvariantCulture));
    }

    private void WriteDateTime(byte[] bytes)
    {
        WriteHead(6, 0);
        WriteString(bytes, 3);
    }

    private void WriteDecimal(decimal value)
    {
        var bits = decimal.GetBits(value);
        var mantissa = (new BigInteger(unchecked((uint)bits[2])) << 64)
            | (new BigInteger(unchecked((uint)bits[1])) << 32)
            | new BigInteger(unchecked((uint)bits[0]));
        var negative = bits[3] < 0 && !mantissa.IsZero;
        if (negative) mantissa -= BigInteger.One;
        WriteHead(6, 4);
        stream.WriteByte(0x82);
        WriteInteger(-((bits[3] >> 16) & 0xff));
        if (mantissa <= ulong.MaxValue)
        {
            WriteHead(negative ? 1 : 0, (ulong)mantissa);
            return;
        }

        // Tag 3 stores -1-n rather than the absolute value of a negative bignum.
        var bytes = mantissa.ToByteArray();
        var length = bytes.Length;
        while (length > 1 && bytes[length - 1] == 0) length--;
        Array.Reverse(bytes, 0, length);
        WriteHead(6, negative ? 3UL : 2UL);
        WriteHead(2, (ulong)length);
        stream.Write(bytes, 0, length);
    }
}
