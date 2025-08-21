using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Trivial.Text;

namespace Trivial.Tasks;

/// <summary>
/// A request object for JSON based stateless light-weight remote procedure call protocol.
/// </summary>
[JsonConverter(typeof(JsonRpcRequestJsonConverter))]
public class JsonRpcRequestObject : IJsonObjectHost
{
    /// <summary>
    /// Initializes a new instance of the <see cref="JsonRpcRequestObject{T}"/> class.
    /// </summary>
    /// <param name="method">The method name.</param>
    public JsonRpcRequestObject(string method)
        : this(Guid.NewGuid(), method)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonRpcRequestObject{T}"/> class.
    /// </summary>
    /// <param name="id">An identifier established by the client; or null, if this request object is a notification.</param>
    /// <param name="method">The name of the method to be invoked.</param>
    public JsonRpcRequestObject(string id, string method)
        : this(null, id, method)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonRpcRequestObject{T}"/> class.
    /// </summary>
    /// <param name="id">An identifier established by the client; or null, if this request object is a notification.</param>
    /// <param name="method">The name of the method to be invoked.</param>
    public JsonRpcRequestObject(Guid id, string method)
        : this(null, id, method)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonRpcRequestObject{T}"/> class.
    /// </summary>
    /// <param name="version">The version of the JSON-RPC protocol. The default is 2.0.</param>
    /// <param name="id">An identifier established by the client; or null, if this request object is a notification.</param>
    /// <param name="method">The name of the method to be invoked.</param>
    public JsonRpcRequestObject(string version, string id, string method)
    {
        Version = version ?? JsonRpcConstants.Version;
        Id = id;
        Method = method;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonRpcRequestObject{T}"/> class.
    /// </summary>
    /// <param name="version">The version of the JSON-RPC protocol. The default is 2.0.</param>
    /// <param name="id">An identifier established by the client; or null, if this request object is a notification.</param>
    /// <param name="method">The name of the method to be invoked.</param>
    public JsonRpcRequestObject(string version, Guid id, string method)
    {
        Version = version ?? JsonRpcConstants.Version;
        Id = id.ToString();
        Method = method;
    }

    /// <summary>
    /// Gets the JSON-RPC protocol version.
    /// </summary>
    [JsonPropertyName("jsonrpc")]
    [Description("The JSON-PRC protocol version.")]
    public string Version { get; }

    /// <summary>
    /// Gets the name of the method to be invoked.
    /// </summary>
    [JsonPropertyName("method")]
    [Description("The name of the method to ve invoked.")]
    public string Method { get; }

    /// <summary>
    /// Gets the identifier established by the client; or null, if this request object is a notification.
    /// </summary>
    [JsonPropertyName("id")]
    [Description("The identifier established by the client; or null, if this request object is a notification.")]
    public string Id { get; }

    /// <summary>
    /// Creates a success response object with the specified result.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="result">The value of this member is determined by the method invoked on the server.</param>
    /// <returns>The JSON-PRC response object in success case.</returns>
    public SuccessJsonRpcResponseObject<TResult> ToSuccessResponse<TResult>(TResult result)
        => new(Version, Id, result);

    /// <summary>
    /// Creates an error response object with the specified result.
    /// </summary>
    /// <param name="code">A code that indicates error type that occurred.</param>
    /// <param name="message">A short description of the error.</param>
    /// <returns>The JSON-PRC response object in error case.</returns>
    public ErrorJsonRpcResponseObject ToErrorResponse(int code, string message = null)
        => new(Version, Id, code, message);

    /// <summary>
    /// Creates an error response object with the specified result.
    /// </summary>
    /// <param name="data">The additional information about the error.</param>
    /// <param name="code">A code that indicates error type that occurred.</param>
    /// <param name="message">A short description of the error.</param>
    /// <returns>The JSON-PRC response object in error case.</returns>
    public ErrorJsonRpcResponseObject<TData> ToErrorResponse<TData>(TData data, int code, string message = null)
        => new(Version, Id, data, code, message);

    /// <summary>
    /// Writes current object to JSON.
    /// </summary>
    /// <param name="writer">The UTF-8 JSON writer to write to.</param>
    /// <param name="options">The JSON serializer options being used.</param>
    public void Write(Utf8JsonWriter writer, JsonSerializerOptions options = default)
        => ToJson().WriteTo(writer);

    /// <summary>
    /// Fills the properties of JSON object to this entity.
    /// </summary>
    /// <param name="json">The JSON object to read.</param>
    /// <exception cref="InvalidOperationException">The identifier is not matched.</exception>
    protected virtual void Fill(JsonObjectNode json)
    {
    }

    /// <summary>
    /// Converts to JSON object node.
    /// </summary>
    /// <returns>The JSON object node about current instance.</returns>
    public JsonObjectNode ToJson()
    {
        var json = new JsonObjectNode()
        {
            { "jsonrpc", Version },
            { "id", Id },
            { "method", Method }
        };
        Fill(json);
        return json;
    }

    /// <summary>
    /// Converts a JSON object to JSON-RPC request object.
    /// </summary>
    /// <param name="json">The JSON object to convert.</param>
    public static implicit operator JsonRpcRequestObject(JsonObjectNode json)
    {
        if (json == null) return null;
        var id = json.TryGetStringValue("id");
        var v = json.TryGetStringTrimmedValue("jsonrpc", true);
        var method = json.TryGetStringValue("method");
        const string parameter = "params";
        var kind = json.GetValueKind(parameter);
        switch (kind)
        {
            case JsonValueKind.Object:
                return new JsonRpcRequestObject<JsonObjectNode>(v, id, method, json.TryGetObjectValue(parameter));
            case JsonValueKind.Array:
                return new JsonRpcRequestObject<JsonArrayNode>(v, id, method, json.TryGetArrayValue(parameter));
            case JsonValueKind.String:
                return new JsonRpcRequestObject<string>(v, id, method, json.TryGetStringValue(parameter));
            case JsonValueKind.True:
                return new JsonRpcRequestObject<bool>(v, id, method, true);
            case JsonValueKind.False:
                return new JsonRpcRequestObject<bool>(v, id, method, false);
            case JsonValueKind.Number:
                {
                    if (json.GetValue(parameter) is not IJsonNumberNode num) break;
                    if (!num.IsInteger)
                        return new JsonRpcRequestObject<double>(v, id, method, num.GetDouble());
                    var i = num.GetInt64();
                    return i > int.MaxValue || i < int.MinValue
                        ? new JsonRpcRequestObject<long>(v, id, method, i)
                        : new JsonRpcRequestObject<int>(v, id, method, (int)i);
                }
        }

        return new JsonRpcRequestObject(v, id, method);
    }
}

/// <summary>
/// A request object for JSON based stateless light-weight remote procedure call protocol.
/// </summary>
/// <typeparam name="T">The type of the parameter.</typeparam>
public class JsonRpcRequestObject<T> : JsonRpcRequestObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="JsonRpcRequestObject{T}"/> class.
    /// </summary>
    /// <param name="method">The method name.</param>
    /// <param name="parameter">The parameters.</param>
    public JsonRpcRequestObject(string method, T parameter)
        : this(Guid.NewGuid(), method, parameter)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonRpcRequestObject{T}"/> class.
    /// </summary>
    /// <param name="id">An identifier established by the client; or null, if this request object is a notification.</param>
    /// <param name="method">The name of the method to be invoked.</param>
    /// <param name="parameter">A structured value that holds the parameter values to be used during the invocation of the method.</param>
    public JsonRpcRequestObject(string id, string method, T parameter)
        : this(JsonRpcConstants.Version, id, method, parameter)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonRpcRequestObject{T}"/> class.
    /// </summary>
    /// <param name="id">An identifier established by the client; or null, if this request object is a notification.</param>
    /// <param name="method">The name of the method to be invoked.</param>
    /// <param name="parameter">A structured value that holds the parameter values to be used during the invocation of the method.</param>
    public JsonRpcRequestObject(Guid id, string method, T parameter)
        : this(JsonRpcConstants.Version, id, method, parameter)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonRpcRequestObject{T}"/> class.
    /// </summary>
    /// <param name="version">The version of the JSON-RPC protocol. The default is 2.0.</param>
    /// <param name="id">An identifier established by the client; or null, if this request object is a notification.</param>
    /// <param name="method">The name of the method to be invoked.</param>
    /// <param name="parameter">The parameter values to be used during the invocation of the method.</param>
    public JsonRpcRequestObject(string version, string id, string method, T parameter)
        : base(version, id, method)
    {
        Parameter = parameter;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonRpcRequestObject{T}"/> class.
    /// </summary>
    /// <param name="version">The version of the JSON-RPC protocol. The default is 2.0.</param>
    /// <param name="id">An identifier established by the client; or null, if this request object is a notification.</param>
    /// <param name="method">The name of the method to be invoked.</param>
    /// <param name="parameter">The parameter values to be used during the invocation of the method.</param>
    public JsonRpcRequestObject(string version, Guid id, string method, T parameter)
        : base(version, id, method)
    {
        Parameter = parameter;
    }

    /// <summary>
    /// Gets the parameter value to be used during the invocation of the method.
    /// </summary>
    [JsonPropertyName("params")]
    [Description("The parameter value to be used during the invocation of the method.")]
    public T Parameter { get; }

    /// <inheritdoc />
    protected override void Fill(JsonObjectNode json)
    {
        base.Fill(json);
        json.SetValueInternal("params", Parameter);
    }
}

/// <summary>
/// The JSON converter of JSON-RPC request object.
/// </summary>
internal sealed class JsonRpcRequestJsonConverter : JsonConverter<JsonRpcRequestObject>
{
    /// <inheritdoc />
    public override JsonRpcRequestObject Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        JsonValues.SkipComments(ref reader);
        if (reader.TokenType == JsonTokenType.Null) return null;
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException($"The token type is {reader.TokenType} but expect a JSON object.");
        var json = JsonObjectNode.ParseValue(ref reader);
        return json;
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, JsonRpcRequestObject value, JsonSerializerOptions options)
    {
        if (value == null)
            writer.WriteNullValue();
        else
            value.Write(writer, options);
    }
}
