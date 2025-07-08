using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Trivial.Text;

namespace Trivial.Tasks;

/// <summary>
/// A base response object for JSON based stateless light-weight remote procedure call protocol.
/// </summary>
[JsonConverter(typeof(JsonRpcResponseJsonConverter))]
public abstract class BaseJsonRpcResponseObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseJsonRpcResponseObject"/> class.
    /// </summary>
    /// <param name="request">The request object.</param>
    protected BaseJsonRpcResponseObject(JsonRpcRequestObject request)
    {
        if (request == null)
        {
            Version = JsonRpcConstants.Version;
            return;
        }

        Version = request.Version ?? JsonRpcConstants.Version;
        Id = request.Id;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseJsonRpcResponseObject"/> class.
    /// </summary>
    /// <param name="id">An identifier established by the client (from the request object).</param>
    protected BaseJsonRpcResponseObject(string id)
        : this(null, id)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseJsonRpcResponseObject"/> class.
    /// </summary>
    /// <param name="id">An identifier established by the client (from the request object).</param>
    protected BaseJsonRpcResponseObject(Guid id)
        : this(null, id)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseJsonRpcResponseObject"/> class.
    /// </summary>
    /// <param name="version">The version of the JSON-RPC protocol. The default is 2.0.</param>
    /// <param name="id">An identifier established by the client (from the request object).</param>
    protected BaseJsonRpcResponseObject(string version, string id)
    {
        Version = version ?? JsonRpcConstants.Version;
        Id = id;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseJsonRpcResponseObject"/> class.
    /// </summary>
    /// <param name="version">The version of the JSON-RPC protocol. The default is 2.0.</param>
    /// <param name="id">An identifier established by the client (from the request object).</param>
    protected BaseJsonRpcResponseObject(string version, Guid id)
    {
        Version = version ?? JsonRpcConstants.Version;
        Id = id.ToString();
    }

    /// <summary>
    /// Gets the JSON-RPC protocol version.
    /// </summary>
    public string Version { get; }

    /// <summary>
    /// Gets the identifier established by the client (from the request object).
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Writes current object to JSON.
    /// </summary>
    /// <param name="writer">The UTF-8 JSON writer to write to.</param>
    /// <param name="options">The JSON serializer options being used.</param>
    public void Write(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString("jsonrpc", Version);
        if (Id != null) writer.WriteString("id", Id);
        WriteProperties(writer, options);
        writer.WriteEndObject();
    }

    /// <summary>
    /// Writes further properties to JSON.
    /// </summary>
    /// <param name="writer">The UTF-8 JSON writer to write to.</param>
    /// <param name="options">The JSON serializer options being used.</param>
    protected virtual void WriteProperties(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
    }
}

/// <summary>
/// A success response object for JSON based stateless light-weight remote procedure call protocol.
/// </summary>
public abstract class SuccessJsonRpcResponseObject : BaseJsonRpcResponseObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SuccessJsonRpcResponseObject"/> class.
    /// </summary>
    /// <param name="request">The request object.</param>
    /// <param name="result">The value of this member is determined by the method invoked on the server.</param>
    protected SuccessJsonRpcResponseObject(JsonRpcRequestObject request, object result)
        : base(request)
    {
        Result = result;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SuccessJsonRpcResponseObject"/> class.
    /// </summary>
    /// <param name="id">An identifier established by the client (from the request object).</param>
    /// <param name="result">The value of this member is determined by the method invoked on the server.</param>
    protected SuccessJsonRpcResponseObject(string id, object result)
        : this(null, id, result)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SuccessJsonRpcResponseObject"/> class.
    /// </summary>
    /// <param name="id">An identifier established by the client (from the request object).</param>
    /// <param name="result">The value of this member is determined by the method invoked on the server.</param>
    protected SuccessJsonRpcResponseObject(Guid id, object result)
        : this(null, id, result)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SuccessJsonRpcResponseObject"/> class.
    /// </summary>
    /// <param name="version">The version of the JSON-RPC protocol. The default is 2.0.</param>
    /// <param name="id">An identifier established by the client (from the request object).</param>
    /// <param name="result">The value of this member is determined by the method invoked on the server.</param>
    protected SuccessJsonRpcResponseObject(string version, string id, object result)
        : base(version, id)
    {
        Result = result;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SuccessJsonRpcResponseObject"/> class.
    /// </summary>
    /// <param name="version">The version of the JSON-RPC protocol. The default is 2.0.</param>
    /// <param name="id">An identifier established by the client (from the request object).</param>
    /// <param name="result">The value of this member is determined by the method invoked on the server.</param>
    protected SuccessJsonRpcResponseObject(string version, Guid id, object result)
         : base(version, id)
    {
        Result = result;
    }

    /// <summary>
    /// Gets the value of this member is determined by the method invoked on the server.
    /// </summary>
    public object Result { get; }
}

/// <summary>
/// A success response object for JSON based stateless light-weight remote procedure call protocol.
/// </summary>
/// <typeparam name="T">The type of the result.</typeparam>
public class SuccessJsonRpcResponseObject<T> : SuccessJsonRpcResponseObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SuccessJsonRpcResponseObject{T}"/> class.
    /// </summary>
    /// <param name="request">The request object.</param>
    /// <param name="result">The value of this member is determined by the method invoked on the server.</param>
    public SuccessJsonRpcResponseObject(JsonRpcRequestObject request, T result)
        : base(request, result)
    {
        Result = result;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SuccessJsonRpcResponseObject{T}"/> class.
    /// </summary>
    /// <param name="id">An identifier established by the client (from the request object).</param>
    /// <param name="result">The value of this member is determined by the method invoked on the server.</param>
    public SuccessJsonRpcResponseObject(string id, T result)
        : this(null, id, result)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SuccessJsonRpcResponseObject{T}"/> class.
    /// </summary>
    /// <param name="id">An identifier established by the client (from the request object).</param>
    /// <param name="result">The value of this member is determined by the method invoked on the server.</param>
    public SuccessJsonRpcResponseObject(Guid id, T result)
        : this(null, id, result)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SuccessJsonRpcResponseObject{T}"/> class.
    /// </summary>
    /// <param name="version">The version of the JSON-RPC protocol. The default is 2.0.</param>
    /// <param name="id">An identifier established by the client (from the request object).</param>
    /// <param name="result">The value of this member is determined by the method invoked on the server.</param>
    public SuccessJsonRpcResponseObject(string version, string id, T result)
        : base(version, id, result)
    {
        Result = result;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SuccessJsonRpcResponseObject{T}"/> class.
    /// </summary>
    /// <param name="version">The version of the JSON-RPC protocol. The default is 2.0.</param>
    /// <param name="id">An identifier established by the client (from the request object).</param>
    /// <param name="result">The value of this member is determined by the method invoked on the server.</param>
    public SuccessJsonRpcResponseObject(string version, Guid id, T result)
         : base(version, id, result)
    {
        Result = result;
    }

    /// <summary>
    /// Gets the value of this member is determined by the method invoked on the server.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public new T Result { get; }

    /// <summary>
    /// Writes parameter to JSON.
    /// </summary>
    /// <param name="writer">The UTF-8 JSON writer to write to.</param>
    /// <param name="options">The JSON serializer options being used.</param>
    protected override void WriteProperties(Utf8JsonWriter writer, JsonSerializerOptions options)
        => JsonValues.Write(writer, "result", Result, options);
}

/// <summary>
/// An error response object for JSON based stateless light-weight remote procedure call protocol.
/// </summary>
public class ErrorJsonRpcResponseObject : BaseJsonRpcResponseObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorJsonRpcResponseObject"/> class.
    /// </summary>
    /// <param name="request">The request object.</param>
    /// <param name="code">A code that indicates error type that occurred.</param>
    /// <param name="message">A short description of the error.</param>
    public ErrorJsonRpcResponseObject(JsonRpcRequestObject request, int code, string message = null)
        : base(request)
    {
        Code = code;
        Message = message;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorJsonRpcResponseObject"/> class.
    /// </summary>
    /// <param name="id">An identifier established by the client (from the request object).</param>
    /// <param name="code">A code that indicates error type that occurred.</param>
    /// <param name="message">A short description of the error.</param>
    public ErrorJsonRpcResponseObject(string id, int code, string message = null)
        : this(null, id, code, message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorJsonRpcResponseObject"/> class.
    /// </summary>
    /// <param name="id">An identifier established by the client (from the request object).</param>
    /// <param name="code">A code that indicates error type that occurred.</param>
    /// <param name="message">A short description of the error.</param>
    public ErrorJsonRpcResponseObject(Guid id, int code, string message = null)
        : this(null, id, code, message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorJsonRpcResponseObject"/> class.
    /// </summary>
    /// <param name="version">The version of the JSON-RPC protocol. The default is 2.0.</param>
    /// <param name="id">An identifier established by the client (from the request object).</param>
    /// <param name="code">A code that indicates error type that occurred.</param>
    /// <param name="message">A short description of the error.</param>
    public ErrorJsonRpcResponseObject(string version, string id, int code, string message = null)
        : base(version, id)
    {
        Code = code;
        Message = message;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorJsonRpcResponseObject"/> class.
    /// </summary>
    /// <param name="version">The version of the JSON-RPC protocol. The default is 2.0.</param>
    /// <param name="id">An identifier established by the client (from the request object).</param>
    /// <param name="code">A code that indicates error type that occurred.</param>
    /// <param name="message">A short description of the error.</param>
    public ErrorJsonRpcResponseObject(string version, Guid id, int code, string message = null)
         : base(version, id)
    {
        Code = code;
        Message = message;
    }

    /// <summary>
    /// Gets the code that indicates error type that occurred.
    /// </summary>
    public int Code { get; }

    /// <summary>
    /// Gets the short description of the error.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Writes current object to JSON.
    /// </summary>
    /// <param name="writer">The UTF-8 JSON writer to write to.</param>
    /// <param name="options">The JSON serializer options being used.</param>
    protected override void WriteProperties(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        writer.WriteNumber("code", Code);
        if (Message != null) writer.WriteString("message", Message);
    }
}

/// <summary>
/// Am error response object for JSON based stateless light-weight remote procedure call protocol.
/// </summary>
/// <typeparam name="T">The type of error data.</typeparam>
public class ErrorJsonRpcResponseObject<T> : ErrorJsonRpcResponseObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorJsonRpcResponseObject{T}"/> class.
    /// </summary>
    /// <param name="request">The request object.</param>
    /// <param name="data">The additional information about the error.</param>
    /// <param name="code">A code that indicates error type that occurred.</param>
    /// <param name="message">A short description of the error.</param>
    public ErrorJsonRpcResponseObject(JsonRpcRequestObject request, T data, int code, string message = null)
        : base(request, code, message)
    {
        Data = data;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorJsonRpcResponseObject{T}"/> class.
    /// </summary>
    /// <param name="id">An identifier established by the client (from the request object).</param>
    /// <param name="data">The additional information about the error.</param>
    /// <param name="code">A code that indicates error type that occurred.</param>
    /// <param name="message">A short description of the error.</param>
    public ErrorJsonRpcResponseObject(string id, T data, int code, string message = null)
        : this(null, id, data, code, message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorJsonRpcResponseObject{T}"/> class.
    /// </summary>
    /// <param name="id">An identifier established by the client (from the request object).</param>
    /// <param name="data">The additional information about the error.</param>
    /// <param name="code">A code that indicates error type that occurred.</param>
    /// <param name="message">A short description of the error.</param>
    public ErrorJsonRpcResponseObject(Guid id, T data, int code, string message = null)
        : this(null, id, data, code, message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorJsonRpcResponseObject{T}"/> class.
    /// </summary>
    /// <param name="version">The version of the JSON-RPC protocol. The default is 2.0.</param>
    /// <param name="id">An identifier established by the client (from the request object).</param>
    /// <param name="data">The additional information about the error.</param>
    /// <param name="code">A code that indicates error type that occurred.</param>
    /// <param name="message">A short description of the error.</param>
    public ErrorJsonRpcResponseObject(string version, string id, T data, int code, string message = null)
        : base(version, id, code, message)
    {
        Data = data;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorJsonRpcResponseObject{T}"/> class.
    /// </summary>
    /// <param name="version">The version of the JSON-RPC protocol. The default is 2.0.</param>
    /// <param name="id">An identifier established by the client (from the request object).</param>
    /// <param name="data">The additional information about the error.</param>
    /// <param name="code">A code that indicates error type that occurred.</param>
    /// <param name="message">A short description of the error.</param>
    public ErrorJsonRpcResponseObject(string version, Guid id, T data, int code, string message = null)
         : base(version, id, code, message)
    {
        Data = data;
    }

    /// <summary>
    /// Gets the additional information about the error.
    /// </summary>
    public T Data { get; }

    /// <summary>
    /// Writes error data to JSON.
    /// </summary>
    /// <param name="writer">The UTF-8 JSON writer to write to.</param>
    /// <param name="options">The JSON serializer options being used.</param>
    protected override void WriteProperties(Utf8JsonWriter writer, JsonSerializerOptions options)
        => JsonValues.Write(writer, "error", Data, options);
}

/// <summary>
/// An error response object for JSON based stateless light-weight remote procedure call protocol.
/// </summary>
public class StreamStateJsonRpcResponseObject : BaseJsonRpcResponseObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StreamStateJsonRpcResponseObject"/> class.
    /// </summary>
    /// <param name="request">The request object.</param>
    /// <param name="message">A short description of the streaming state.</param>
    public StreamStateJsonRpcResponseObject(JsonRpcRequestObject request, string message = null)
        : base(request)
    {
        Message = message;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamStateJsonRpcResponseObject"/> class.
    /// </summary>
    /// <param name="id">An identifier established by the client (from the request object).</param>
    /// <param name="message">A short description of the streaming state.</param>
    public StreamStateJsonRpcResponseObject(string id, string message = null)
        : this(null, id, message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamStateJsonRpcResponseObject"/> class.
    /// </summary>
    /// <param name="id">An identifier established by the client (from the request object).</param>
    /// <param name="message">A short description of the streaming state.</param>
    public StreamStateJsonRpcResponseObject(Guid id, string message = null)
        : this(null, id, message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamStateJsonRpcResponseObject"/> class.
    /// </summary>
    /// <param name="version">The version of the JSON-RPC protocol. The default is 2.0.</param>
    /// <param name="id">An identifier established by the client (from the request object).</param>
    /// <param name="message">A short description of the streaming state.</param>
    public StreamStateJsonRpcResponseObject(string version, string id, string message = null)
        : base(version, id)
    {
        Message = message;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamStateJsonRpcResponseObject"/> class.
    /// </summary>
    /// <param name="version">The version of the JSON-RPC protocol. The default is 2.0.</param>
    /// <param name="id">An identifier established by the client (from the request object).</param>
    /// <param name="message">A short description of the streaming state.</param>
    public StreamStateJsonRpcResponseObject(string version, Guid id, string message = null)
         : base(version, id)
    {
        Message = message;
    }

    /// <summary>
    /// Gets the short description of the streaming state.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Writes current object to JSON.
    /// </summary>
    /// <param name="writer">The UTF-8 JSON writer to write to.</param>
    /// <param name="options">The JSON serializer options being used.</param>
    protected override void WriteProperties(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        writer.WriteString("stream", Message);
    }
}

/// <summary>
/// The JSON converter of JSON-RPC request object.
/// </summary>
internal sealed class JsonRpcResponseJsonConverter : JsonConverter<BaseJsonRpcResponseObject>
{
    /// <inheritdoc />
    public override BaseJsonRpcResponseObject Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        JsonValues.SkipComments(ref reader);
        if (reader.TokenType == JsonTokenType.Null) return null;
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException($"The token type is {reader.TokenType} but expect a JSON object.");
        var json = JsonObjectNode.ParseValue(ref reader);
        if (json == null) return null;
        var id = json.TryGetStringValue("id");
        var v = json.TryGetStringTrimmedValue("jsonrpc", true);
        var error = json.TryGetObjectValue("error");
        if (error != null)
        {
            const string dataKey = "data";
            var dataKind = error.GetValueKind(dataKey);
            var code = error.TryGetInt32Value("code") ?? 0;
            var message = error.TryGetStringValue("message");
            switch (dataKind)
            {
                case JsonValueKind.Object:
                    return new ErrorJsonRpcResponseObject<JsonObjectNode>(v, id, json.TryGetObjectValue(dataKey), code, message);
                case JsonValueKind.Array:
                    return new ErrorJsonRpcResponseObject<JsonArrayNode>(v, id, json.TryGetArrayValue(dataKey), code, message);
                case JsonValueKind.String:
                    return new ErrorJsonRpcResponseObject<string>(v, id, json.TryGetStringValue(dataKey), code, message);
                case JsonValueKind.True:
                    return new ErrorJsonRpcResponseObject<bool>(v, id, true, code, message);
                case JsonValueKind.False:
                    return new ErrorJsonRpcResponseObject<bool>(v, id, false, code, message);
                case JsonValueKind.Number:
                    {
                        if (json.GetValue(dataKey) is not IJsonNumberNode num) break;
                        if (!num.IsInteger)
                            return new ErrorJsonRpcResponseObject<double>(v, id, num.GetDouble(), code, message);
                        var i = num.GetInt64();
                        return i > int.MaxValue || i < int.MinValue
                            ? new ErrorJsonRpcResponseObject<long>(v, id, i, code, message)
                            : new ErrorJsonRpcResponseObject<int>(v, id, (int)i, code, message);
                    }
            }

            return new ErrorJsonRpcResponseObject(v, id, code, message);
        }

        const string result = "result";
        var kind = json.GetValueKind(result);
        switch (kind)
        {
            case JsonValueKind.Object:
                return new SuccessJsonRpcResponseObject<JsonObjectNode>(v, id, json.TryGetObjectValue(result));
            case JsonValueKind.Array:
                return new SuccessJsonRpcResponseObject<JsonArrayNode>(v, id, json.TryGetArrayValue(result));
            case JsonValueKind.String:
                return new SuccessJsonRpcResponseObject<string>(v, id, json.TryGetStringValue(result));
            case JsonValueKind.True:
                return new SuccessJsonRpcResponseObject<bool>(v, id, true);
            case JsonValueKind.False:
                return new SuccessJsonRpcResponseObject<bool>(v, id, false);
            case JsonValueKind.Number:
                {
                    if (json.GetValue(result) is not IJsonNumberNode num) break;
                    if (!num.IsInteger)
                        return new SuccessJsonRpcResponseObject<double>(v, id, num.GetDouble());
                    var i = num.GetInt64();
                    return i > int.MaxValue || i < int.MinValue
                        ? new SuccessJsonRpcResponseObject<long>(v, id, i)
                        : new SuccessJsonRpcResponseObject<int>(v, id, (int)i);
                }
        }

        var streamMessage = json.TryGetStringValue("stream");
        if (streamMessage != null) return new StreamStateJsonRpcResponseObject(v, id, streamMessage);
        return new SuccessJsonRpcResponseObject<JsonObjectNode>(v, id, null);
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, BaseJsonRpcResponseObject value, JsonSerializerOptions options)
    {
        if (value == null)
            writer.WriteNullValue();
        else
            value.Write(writer, options);
    }
}
