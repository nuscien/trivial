using System;
using System.IO;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Trivial.Data;

/// <summary>
/// Tests CBOR wire encodings and streaming behavior.
/// </summary>
[TestClass]
public class CborWriterUnitTest
{
    [TestMethod]
    public void TestNestedContainers()
    {
        using var stream = new MemoryStream();
        var writer = new SimpleCborStreamWriter(stream);
        writer.StartObject();
        writer.AppendProperty("a", 1);
        writer.StartArray("b");
        writer.AppendItem("x");
        writer.StartObject();
        writer.AppendNullProperty("n");
        writer.StartObject("o");
        writer.EndObject();
        writer.EndObject();
        writer.StartArray();
        writer.AppendNullItem();
        writer.EndArray();
        writer.EndArray();
        writer.EndObject();
        Assert.AreEqual("BF-61-61-01-61-62-9F-61-78-BF-61-6E-F6-61-6F-BF-FF-FF-9F-F6-FF-FF-FF", BitConverter.ToString(stream.ToArray()));
    }

    [TestMethod]
    public void TestIntegerBoundaries()
    {
        AssertEncoding("00", w => w.AppendItem(0));
        AssertEncoding("17", w => w.AppendItem(23L));
        AssertEncoding("18-18", w => w.AppendItem(24L));
        AssertEncoding("18-FF", w => w.AppendItem(255L));
        AssertEncoding("19-01-00", w => w.AppendItem(256L));
        AssertEncoding("19-FF-FF", w => w.AppendItem(65535L));
        AssertEncoding("1A-00-01-00-00", w => w.AppendItem(65536L));
        AssertEncoding("1A-FF-FF-FF-FF", w => w.AppendItem(uint.MaxValue));
        AssertEncoding("1B-00-00-00-01-00-00-00-00", w => w.AppendItem(4294967296UL));
        AssertEncoding("1B-FF-FF-FF-FF-FF-FF-FF-FF", w => w.AppendItem(ulong.MaxValue));
        AssertEncoding("1B-7F-FF-FF-FF-FF-FF-FF-FF", w => w.AppendItem(long.MaxValue));
        AssertEncoding("20", w => w.AppendItem(-1));
        AssertEncoding("37", w => w.AppendItem(-24L));
        AssertEncoding("38-18", w => w.AppendItem(-25L));
        AssertEncoding("3B-7F-FF-FF-FF-FF-FF-FF-FF", w => w.AppendItem(long.MinValue));
        AssertEncoding("01", w => w.AppendItem((byte)1));
        AssertEncoding("20", w => w.AppendItem((sbyte)-1));
        AssertEncoding("20", w => w.AppendItem((short)-1));
        AssertEncoding("01", w => w.AppendItem((ushort)1));
    }

    [TestMethod]
    public void TestStringsAndSimpleValues()
    {
        AssertEncoding("60", w => w.AppendItem(string.Empty));
        AssertEncoding("67-E4-B8-AD-F0-9F-98-80", w => w.AppendItem("\u4e2d\ud83d\ude00"));
        AssertEncoding("78-18-" + BitConverter.ToString(Encoding.UTF8.GetBytes(new string('x', 24))), w => w.AppendItem(new string('x', 24)));
        AssertEncoding("40", w => w.AppendItem(new byte[0]));
        AssertEncoding("43-00-80-FF", w => w.AppendItem(new byte[] { 0, 128, 255 }));
        AssertEncoding("F4", w => w.AppendItem(false));
        AssertEncoding("F5", w => w.AppendItem(true));
        AssertEncoding("F6", w => w.AppendNullItem());
        AssertEncoding("F6", w => w.AppendItem((string)null));
        AssertEncoding("F6", w => w.AppendItem((byte[])null));
    }

    [TestMethod]
    public void TestFloatingPointAndDecimal()
    {
        AssertEncoding("FA-3F-C0-00-00", w => w.AppendItem(1.5f));
        AssertEncoding("FA-80-00-00-00", w => w.AppendItem(-0.0f));
        AssertEncoding("FA-7F-80-00-00", w => w.AppendItem(float.PositiveInfinity));
        AssertEncoding("FB-3F-F8-00-00-00-00-00-00", w => w.AppendItem(1.5d));
        AssertEncoding("FB-FF-F0-00-00-00-00-00-00", w => w.AppendItem(double.NegativeInfinity));
        AssertEncoding("C4-82-00-00", w => w.AppendItem(0m));
        AssertEncoding("C4-82-21-19-04-D2", w => w.AppendItem(12.34m));
        AssertEncoding("C4-82-21-39-04-D1", w => w.AppendItem(-12.34m));
        AssertEncoding("C4-82-38-1B-01", w => w.AppendItem(0.0000000000000000000000000001m));
        AssertEncoding("C4-82-00-1B-FF-FF-FF-FF-FF-FF-FF-FF", w => w.AppendItem(18446744073709551615m));
        AssertEncoding("C4-82-00-C2-49-01-00-00-00-00-00-00-00-00", w => w.AppendItem(18446744073709551616m));
        AssertEncoding("C4-82-00-3B-FF-FF-FF-FF-FF-FF-FF-FF", w => w.AppendItem(-18446744073709551616m));
        AssertEncoding("C4-82-00-C2-4C-FF-FF-FF-FF-FF-FF-FF-FF-FF-FF-FF-FF", w => w.AppendItem(decimal.MaxValue));
        AssertEncoding("C4-82-00-C3-4C-FF-FF-FF-FF-FF-FF-FF-FF-FF-FF-FF-FE", w => w.AppendItem(decimal.MinValue));
    }

    [TestMethod]
    public void TestDates()
    {
        var utc = new DateTime(2026, 11, 13, 5, 24, 2, DateTimeKind.Utc).AddTicks(1234567);
        var utcBytes = "C0-78-1C-" + BitConverter.ToString(Encoding.UTF8.GetBytes("2026-11-13T05:24:02.1234567Z"));
        AssertEncoding(utcBytes, w => w.AppendItem(utc));
        AssertEncoding(utcBytes, w => w.AppendItem(DateTime.SpecifyKind(utc, DateTimeKind.Unspecified)));
        AssertEncoding(utcBytes, w => w.AppendItem(utc.ToLocalTime()));
        var offset = new DateTimeOffset(2026, 11, 13, 13, 24, 2, TimeSpan.FromHours(8));
        AssertEncoding("C0-78-21-" + BitConverter.ToString(Encoding.UTF8.GetBytes("2026-11-13T13:24:02.0000000+08:00")), w => w.AppendItem(offset));
    }

    [TestMethod]
    public void TestPropertyOverloads()
    {
        AssertPropertyEncoding("61-78", w => w.AppendProperty("k", "x"));
        AssertPropertyEncoding("41-01", w => w.AppendProperty("k", new byte[] { 1 }));
        AssertPropertyEncoding("F6", w => w.AppendProperty("k", (string)null));
        AssertPropertyEncoding("F6", w => w.AppendProperty("k", (byte[])null));
        AssertPropertyEncoding("F6", w => w.AppendNullProperty("k"));
        AssertPropertyEncoding("F5", w => w.AppendProperty("k", true));
        AssertPropertyEncoding("F4", w => w.AppendProperty("k", false));
        AssertPropertyEncoding("20", w => w.AppendProperty("k", -1L));
        AssertPropertyEncoding("1B-FF-FF-FF-FF-FF-FF-FF-FF", w => w.AppendProperty("k", ulong.MaxValue));
        AssertPropertyEncoding("FA-3F-C0-00-00", w => w.AppendProperty("k", 1.5f));
        AssertPropertyEncoding("FB-3F-F8-00-00-00-00-00-00", w => w.AppendProperty("k", 1.5d));
        AssertPropertyEncoding("C4-82-21-19-04-D2", w => w.AppendProperty("k", 12.34m));
        AssertPropertyEncoding("C0-78-1C-" + BitConverter.ToString(Encoding.UTF8.GetBytes("2026-11-13T00:00:00.0000000Z")),
            w => w.AppendProperty("k", new DateTime(2026, 11, 13, 0, 0, 0, DateTimeKind.Utc)));
        AssertPropertyEncoding("C0-78-21-" + BitConverter.ToString(Encoding.UTF8.GetBytes("2026-11-13T00:00:00.0000000+08:00")),
            w => w.AppendProperty("k", new DateTimeOffset(2026, 11, 13, 0, 0, 0, TimeSpan.FromHours(8))));
    }

    [TestMethod]
    public void TestInvalidCallsDoNotWrite()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => new SimpleCborStreamWriter(null));
        using var readOnly = new MemoryStream(new byte[0], false);
        Assert.ThrowsExactly<ArgumentException>(() => new SimpleCborStreamWriter(readOnly));
        using var stream = new MemoryStream();
        var writer = new SimpleCborStreamWriter(stream);
        Assert.ThrowsExactly<InvalidOperationException>(() => writer.EndArray());
        Assert.ThrowsExactly<InvalidOperationException>(() => writer.EndObject());
        Assert.ThrowsExactly<InvalidOperationException>(() => writer.AppendNullProperty("k"));
        Assert.ThrowsExactly<InvalidOperationException>(() => writer.StartArray("k"));
        Assert.ThrowsExactly<EncoderFallbackException>(() => writer.AppendItem("\ud800"));
        Assert.AreEqual(0L, stream.Length);
        writer.StartObject();
        Assert.ThrowsExactly<InvalidOperationException>(() => writer.AppendItem(1));
        Assert.ThrowsExactly<InvalidOperationException>(() => writer.StartArray());
        Assert.ThrowsExactly<InvalidOperationException>(() => writer.StartObject());
        Assert.ThrowsExactly<InvalidOperationException>(() => writer.EndArray());
        Assert.ThrowsExactly<ArgumentNullException>(() => writer.AppendNullProperty(null));
        Assert.ThrowsExactly<EncoderFallbackException>(() => writer.AppendProperty("k", "\ud800"));
        Assert.ThrowsExactly<EncoderFallbackException>(() => writer.AppendProperty("\ud800", 1));
        Assert.AreEqual("BF", BitConverter.ToString(stream.ToArray()));
        writer.StartArray("k");
        Assert.ThrowsExactly<InvalidOperationException>(() => writer.AppendNullProperty("k"));
        Assert.ThrowsExactly<InvalidOperationException>(() => writer.StartObject("k"));
        Assert.ThrowsExactly<InvalidOperationException>(() => writer.EndObject());
        writer.EndArray();
        writer.EndObject();
        Assert.ThrowsExactly<InvalidOperationException>(() => writer.AppendNullItem());
        Assert.ThrowsExactly<InvalidOperationException>(() => writer.StartObject());
        Assert.AreEqual("BF-61-6B-9F-FF-FF", BitConverter.ToString(stream.ToArray()));
    }

    [TestMethod]
    public void TestNonSeekableStreamAndWriteFailure()
    {
        using var output = new MemoryStream();
        output.WriteByte(0x01);
        using var stream = new WriteOnlyStream(output);
        var writer = new SimpleCborStreamWriter(stream);
        Assert.AreEqual("01", BitConverter.ToString(output.ToArray()));
        writer.StartArray();
        Assert.AreEqual("01-9F", BitConverter.ToString(output.ToArray()));
        writer.AppendItem(1);
        writer.EndArray();
        Assert.AreEqual("01-9F-01-FF", BitConverter.ToString(output.ToArray()));
        Assert.IsTrue(output.CanWrite);
        stream.FailWrites = true;
        var failingWriter = new SimpleCborStreamWriter(stream);
        Assert.ThrowsExactly<IOException>(() => failingWriter.StartArray());
        Assert.AreEqual("01-9F-01-FF", BitConverter.ToString(output.ToArray()));
    }

    private static void AssertEncoding(string expected, Action<SimpleCborStreamWriter> write)
    {
        using var stream = new MemoryStream();
        write(new SimpleCborStreamWriter(stream));
        Assert.AreEqual(expected, BitConverter.ToString(stream.ToArray()));
    }

    private static void AssertPropertyEncoding(string expectedValue, Action<SimpleCborStreamWriter> write)
    {
        AssertEncoding("BF-61-6B-" + expectedValue + "-FF", writer =>
        {
            writer.StartObject();
            write(writer);
            writer.EndObject();
        });
    }

    private sealed class WriteOnlyStream(Stream output) : Stream
    {
        public bool FailWrites { get; set; }

        public override bool CanRead => false;

        public override bool CanSeek => false;

        public override bool CanWrite => true;

        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override void Flush() => output.Flush();

        public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();

        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

        public override void SetLength(long value) => throw new NotSupportedException();

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (FailWrites) throw new IOException("Test write failure.");
            output.Write(buffer, offset, count);
        }
    }
}