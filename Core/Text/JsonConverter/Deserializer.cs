using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Trivial.Text;

/// <summary>
/// The base deserializer for JSON.
/// </summary>
/// <typeparam name="T">The type of the target.</typeparam>
public class JsonTypedDeserializer<T>
{
    private readonly BaseJsonTypedDeserializer<T> impl;

    /// <summary>
    /// Initializes a new instance of the JsonTypedDeserializer class.
    /// </summary>
    public JsonTypedDeserializer()
    {
    }

    /// <summary>
    /// Initializes a new instance of the JsonTypedDeserializer class.
    /// </summary>
    public JsonTypedDeserializer(Func<Stream, T> deserializer)
    {
        impl = new StreamJsonTypedDeserializer<T>(deserializer);
        IsStreamPriority = true;
    }

    /// <summary>
    /// Initializes a new instance of the JsonTypedDeserializer class.
    /// </summary>
    public JsonTypedDeserializer(Func<string, T> deserializer)
    {
        impl = new StringJsonTypedDeserializer<T>(deserializer);
    }

    /// <summary>
    /// Gets the type of the target to deserialize.
    /// </summary>
    public Type TargetType => typeof(T);

    /// <summary>
    /// Gets a value indicating whether use stream as high priority instead of string to deserialize; otherwise, false.
    /// </summary>
    public virtual bool IsStreamPriority { get; }

    /// <summary>
    /// Deserializes from a stream.
    /// </summary>
    /// <param name="stream">The stream to deserialize.</param>
    /// <returns>The object deserialized.</returns>
    public virtual T Deserialize(Stream stream)
    {
        if (impl == null) return JsonSerializer.Deserialize<T>(stream);
        return impl.Deserialize(stream);
    }

    /// <summary>
    /// Deserializes from a string.
    /// </summary>
    /// <param name="s">The string to deserialize.</param>
    /// <returns>The object deserialized.</returns>
    public virtual T Deserialize(string s)
    {
        if (impl == null) return JsonSerializer.Deserialize<T>(s);
        return impl.Deserialize(s);
    }
}

/// <summary>
/// The base deserializer for JSON.
/// </summary>
/// <typeparam name="T">The type of the target.</typeparam>
internal abstract class BaseJsonTypedDeserializer<T>
{
    /// <summary>
    /// Initializes a new instance of the BaseJsonTypedDeserializer class.
    /// </summary>
    internal BaseJsonTypedDeserializer()
    {
    }

    /// <summary>
    /// Deserializes from a stream.
    /// </summary>
    /// <param name="stream">The stream to deserialize.</param>
    /// <returns>The object deserialized.</returns>
    public abstract T Deserialize(Stream stream);

    /// <summary>
    /// Deserializes from a string.
    /// </summary>
    /// <param name="s">The string to deserialize.</param>
    /// <returns>The object deserialized.</returns>
    public abstract T Deserialize(string s);
}

/// <summary>
/// The deserializer for JSON from a stream.
/// </summary>
/// <typeparam name="T">The type of the target.</typeparam>
/// <remarks>
/// Initializes a new instance of the StreamJsonTypedDeserializer class.
/// </remarks>
internal class StreamJsonTypedDeserializer<T>(Func<Stream, T> deserializer) : BaseJsonTypedDeserializer<T>
{
    /// <summary>
    /// Deserializes from a stream.
    /// </summary>
    /// <param name="stream">The stream to deserialize.</param>
    /// <returns>The object deserialized.</returns>
    public override T Deserialize(Stream stream)
    {
        if (deserializer == null) return JsonSerializer.Deserialize<T>(stream);
        return deserializer(stream);
    }

    /// <summary>
    /// Deserializes from a string.
    /// </summary>
    /// <param name="s">The string to deserialize.</param>
    /// <returns>The object deserialized.</returns>
    public override T Deserialize(string s)
    {
        if (deserializer == null) return JsonSerializer.Deserialize<T>(s);
        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream, Encoding.UTF8);
        writer.Write(s);
        writer.Flush();
        stream.Position = 0;
        return deserializer(stream);
    }
}

/// <summary>
/// The deserializer for JSON from a string.
/// </summary>
/// <typeparam name="T">The type of the target.</typeparam>
/// <remarks>
/// Initializes a new instance of the StreamJsonTypedDeserializer class.
/// </remarks>
internal class StringJsonTypedDeserializer<T>(Func<string, T> deserializer) : BaseJsonTypedDeserializer<T>
{
    /// <summary>
    /// Deserializes from a stream.
    /// </summary>
    /// <param name="stream">The stream to deserialize.</param>
    /// <returns>The object deserialized.</returns>
    public override T Deserialize(Stream stream)
    {
        if (deserializer == null) return JsonSerializer.Deserialize<T>(stream);
        var s = StringExtensions.ToString(stream);
        return deserializer(s);
    }

    /// <summary>
    /// Deserializes from a string.
    /// </summary>
    /// <param name="s">The string to deserialize.</param>
    /// <returns>The object deserialized.</returns>
    public override T Deserialize(string s)
    {
        if (deserializer == null) return JsonSerializer.Deserialize<T>(s);
        return deserializer(s);
    }
}
