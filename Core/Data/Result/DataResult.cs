// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataValue.cs" company="Nanchang Jinchen Software Co., Ltd.">
//   Copyright (c) 2010 Nanchang Jinchen Software Co., Ltd. All rights reserved.
// </copyright>
// <summary>
//   The data value for definition for a list row or a table column.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

using Trivial.Data;
using Trivial.Reflection;
using Trivial.Security;
using Trivial.Text;

namespace Trivial.Data;

/// <summary>
/// The collection result.
/// </summary>
[DataContract]
[Guid("FFF9B1F6-12E5-44B1-BAC4-3CB9C78F02BF")]
public class MessageResult : BaseObservableProperties
{
    /// <summary>
    /// Initializes a new instance of the MessageResult class.
    /// </summary>
    public MessageResult()
    {
    }

    /// <summary>
    /// Initializes a new instance of the MessageResult class.
    /// </summary>
    /// <param name="message">The message.</param>
    public MessageResult(string message)
    {
        Message = message;
    }

    /// <summary>
    /// Initializes a new instance of the MessageResult class.
    /// </summary>
    /// <param name="copy">The instance to copy its properites.</param>
    protected MessageResult(MessageResult copy)
    {
        if (copy == null) return;
        Message = copy.Message;
        TrackingId = copy.TrackingId;
        Tag = copy.Tag;
    }

    /// <summary>
    /// Gets or sets the message.
    /// It could be an error message, a status description or a notice text. This can be null if no such information.
    /// </summary>
    [DataMember(Name = "message", EmitDefaultValue = false)]
    [JsonPropertyName("message")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [Description("The additional message for this operation. It could be an error message, a status description or a notice text. This can be null if no such information.")]
    public string Message 
    {
        get => GetCurrentProperty<string>();
        set => SetCurrentProperty(value);
    }

    /// <summary>
    /// Gets or sets the tracking identifier which is used to mark a transaction of operation process..
    /// </summary>
    [DataMember(Name = "track", EmitDefaultValue = false)]
    [JsonPropertyName("track")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [Description("A tracking ID to mark the transaction of operation process.")]
    public string TrackingId
    {
        get => GetCurrentProperty<string>();
        set => SetCurrentProperty(value);
    }

    /// <summary>
    /// Gets or sets the additional optional tag.
    /// </summary>
    /// <remarks>The property is used for caller information and is not under observable.</remarks>
    [JsonIgnore]
    public object Tag { get; set; }

    /// <summary>
    /// Creates a new tracking ID to use.
    /// </summary>
    public void CreateNewTrackingId()
        => TrackingId = Guid.NewGuid().ToString("N");

    /// <summary>
    /// Creates a new tracking ID to use.
    /// </summary>
    public void CreateNewTrackingId(Guid id)
        => TrackingId = id.ToString("N");
}

/// <summary>
/// The data result.
/// </summary>
[DataContract]
[Guid("151653E9-0220-447D-919C-4F5A7732CAE4")]
public class DataResult<T> : MessageResult
{
    /// <summary>
    /// Initializes a new instance of the DataResult class.
    /// </summary>
    public DataResult()
    {
    }

    /// <summary>
    /// Initializes a new instance of the DataResult class.
    /// </summary>
    /// <param name="data">The data.</param>
    public DataResult(T data)
    {
        Data = data;
    }

    /// <summary>
    /// Initializes a new instance of the DataResult class.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="message">The message.</param>
    public DataResult(T data, string message)
        : base(message)
    {
        Data = data;
    }

    /// <summary>
    /// Initializes a new instance of the DataResult class.
    /// </summary>
    /// <param name="copy">The instance to copy its properties.</param>
    protected DataResult(DataResult<T> copy)
        : base(copy)
    {
        if (copy == null) return;
        Data = copy.Data;
    }

    /// <summary>
    /// Gets or sets the data content.
    /// </summary>
    [DataMember(Name = "data")]
    [JsonPropertyName("data")]
    [Description("The data content of the result.")]
    public T Data
    {
        get => GetCurrentProperty<T>();
        set => SetCurrentProperty(value);
    }

    /// <summary>
    /// Gets a value indicating whether the data is null.
    /// </summary>
    [JsonIgnore]
    public bool IsNull => Data is null;

    /// <summary>
    /// Gets the type of the data.
    /// </summary>
    /// <returns>The type of the data.</returns>
    public Type GetDataType()
        => Data?.GetType() ?? typeof(T);

    /// <summary>
    /// Serializes the data to JSON format string.
    /// </summary>
    /// <param name="options">Options to control the conversion behavior.</param>
    /// <returns>A string representation of the value.</returns>
    /// <exception cref="NotSupportedException">There is no compatible JSON converter for the data or its serializable members.</exception>
    public virtual string SerializeData(JsonSerializerOptions options = null)
        => JsonSerializer.Serialize(Data, options);
}

/// <summary>
/// The data result.
/// </summary>
[DataContract]
[Guid("151653E9-0220-447D-919C-4F5A7732CAE4")]
public class DataResult<TData, TInfo> : DataResult<TData>
{
    /// <summary>
    /// Initializes a new instance of the DataResult class.
    /// </summary>
    public DataResult()
    {
    }

    /// <summary>
    /// Initializes a new instance of the DataResult class.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="additional">The object of additional information.</param>
    public DataResult(TData data, TInfo additional)
        : base(data)
    {
        AdditionalInfo = additional;
    }

    /// <summary>
    /// Initializes a new instance of the DataResult class.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="additional">The object of additional information.</param>
    /// <param name="message">The message.</param>
    public DataResult(TData data, TInfo additional, string message)
        : base(data, message)
    {
        AdditionalInfo = additional;
    }

    /// <summary>
    /// Initializes a new instance of the DataResult class.
    /// </summary>
    /// <param name="copy">The instance to copy its properties.</param>
    protected DataResult(DataResult<TData, TInfo> copy)
        : base(copy)
    {
        if (copy == null) return;
        AdditionalInfo = copy.AdditionalInfo;
    }

    /// <summary>
    /// Gets or sets the additional info.
    /// </summary>
    [DataMember(Name = "info")]
    [JsonPropertyName("info")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [Description("The additional information of the result.")]
    public TInfo AdditionalInfo
    {
        get => GetCurrentProperty<TInfo>();
        set => SetCurrentProperty(value);
    }
}

/// <summary>
/// The data result.
/// </summary>
[DataContract]
[Guid("53843D30-66F1-4AC8-A7DD-509ED7E50FD8")]
public class JsonDataResult : DataResult<JsonObjectNode, JsonObjectNode>
{
    /// <summary>
    /// Initializes a new instance of the JsonDataResult class.
    /// </summary>
    public JsonDataResult()
    {
    }

    /// <summary>
    /// Initializes a new instance of the JsonDataResult class.
    /// </summary>
    /// <param name="data">The data.</param>
    public JsonDataResult(JsonObjectNode data)
    {
        Data = data;
    }

    /// <summary>
    /// Initializes a new instance of the JsonDataResult class.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="message">The message.</param>
    public JsonDataResult(JsonObjectNode data, string message)
        : base(data, null, message)
    {
    }

    /// <summary>
    /// Gets or sets the components.
    /// </summary>
    [DataMember(Name = "components")]
    [JsonPropertyName("components")]
    [Description("The components for reference.")]
    public JsonObjectNode Components
    {
        get => GetCurrentProperty<JsonObjectNode>();
        set => SetCurrentProperty(value);
    }

    /// <summary>
    /// Gets the value of the specific property.
    /// </summary>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <param name="key">The property key.</param>
    /// <returns>The value.</returns>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
    /// <exception cref="InvalidOperationException">The type is not supported to convert.</exception>
    public T GetValue<T>(string key)
    {
        if (Data is null) throw new InvalidOperationException("Data is null.");
        return Data.GetValue<T>(key);
    }

    /// <summary>
    /// Gets the value of the specific property.
    /// </summary>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <param name="key">The property key.</param>
    /// <returns>The value.</returns>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
    /// <exception cref="InvalidOperationException">The type is not supported to convert.</exception>
    public T GetValue<T>(ReadOnlySpan<char> key)
    {
        if (Data is null) throw new InvalidOperationException("Data is null.");
        return Data.GetValue<T>(key);
    }

    /// <summary>
    /// Gets the value of the specific property.
    /// </summary>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <param name="key">The property key.</param>
    /// <param name="subKey">The sub-key of the previous property.</param>
    /// <param name="keyPath">The additional property key path.</param>
    /// <returns>The value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
    /// <exception cref="InvalidOperationException">Cannot get the property value or the type is not supported to convert.</exception>
    public T GetValue<T>(string key, string subKey, params string[] keyPath)
    {
        if (Data is null) throw new InvalidOperationException("Data is null.");
        return Data.GetValue<T>(key, subKey, keyPath);
    }

    /// <summary>
    /// Gets the value of the specific property.
    /// </summary>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <param name="keyPath">The property key path.</param>
    /// <returns>The value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
    /// <exception cref="InvalidOperationException">Cannot get the property value or the type is not supported to convert.</exception>
    public T GetValue<T>(IEnumerable<string> keyPath)
    {
        if (Data is null) throw new InvalidOperationException("Data is null.");
        return Data.GetValue<T>(keyPath);
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <returns>The value.</returns>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
    public BaseJsonValueNode TryGetValue(string key)
        => Data?.TryGetValue(key);

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="keyPath">The property key path.</param>
    /// <returns>The value.</returns>
    /// <exception cref="InvalidOperationException">Cannot get the property value.</exception>
    public BaseJsonValueNode TryGetValue(IEnumerable<string> keyPath)
        => Data?.TryGetValue(keyPath);

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="subKey">The sub-key of the previous property.</param>
    /// <param name="keyPath">The additional property key path.</param>
    /// <returns>The value.</returns>
    /// <exception cref="InvalidOperationException">Cannot get the property value.</exception>
    public BaseJsonValueNode TryGetValue(string key, string subKey, params string[] keyPath)
        => Data?.TryGetValue(key, subKey, keyPath);

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <returns>The value.</returns>
    /// <exception cref="ArgumentNullException">The property key should not be null, empty, or consists only of white-space characters.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The property does not exist.</exception>
    public BaseJsonValueNode TryGetValue(ReadOnlySpan<char> key)
        => Data?.TryGetValue(key);

    /// <summary>
    /// Gets schema.
    /// </summary>
    /// <param name="key">The schema key.</param>
    /// <returns>The schema information.</returns>
    public JsonObjectNode GetSchema(string key)
        => string.IsNullOrWhiteSpace(key) ? null : GetComponent("schemas")?.TryGetObjectValue(key);

    /// <summary>
    /// Gets reference object.
    /// </summary>
    /// <param name="type">The type or group key of the resource.</param>
    /// <param name="id">The identifier of the object.</param>
    /// <param name="httpClientResolver">The optional HTTP client resolver.</param>
    /// <returns>The reference object.</returns>
    public JsonObjectNode GetReferenceObject(string type, string id, IObjectResolver<System.Net.Http.HttpClient> httpClientResolver = null)
    {
        if (string.IsNullOrWhiteSpace(id)) return null;
        var components = GetComponents();
        if (components == null) return null;
        var valueKind = components.GetValueKind(type);
        var json = valueKind switch
        {
            JsonValueKind.Object => components.TryGetObjectValue(type)?.TryGetObjectValue(id),
            JsonValueKind.Array => components.TryGetArrayValue(type)?.TryGetObjectValueById(id),
            _ => null,
        };
        if (json == null) return null;
        var refPath = json.TryGetStringValue("$ref");
        if (refPath == null) return json;
        return JsonObjectNode.TryGetRefObjectValue(Data, json, ToJson(), httpClientResolver);
    }

    internal JsonObjectNode GetComponents()
        => Components ?? Data?.TryGetObjectValue("components");

    internal JsonObjectNode GetComponent(string key)
    {
        var dict = GetComponents();
        return string.IsNullOrWhiteSpace(key) ? dict : dict?.TryGetObjectValue(key);
    }

    internal JsonObjectNode ToJson()
        => new()
        {
            { "track", TrackingId },
            { "message", Message },
            { "data", Data },
            { "info", AdditionalInfo },
            { "components", Components }
        };
}

/// <summary>
/// The collection result.
/// </summary>
[DataContract]
[Guid("ED51EAB1-B281-46EF-9AFD-7B4E75EF9F4D")]
public class ErrorMessageResult : MessageResult
{
    /// <summary>
    /// Initializes a new instance of the ErrorMessageResult class.
    /// </summary>
    public ErrorMessageResult()
    {
    }

    /// <summary>
    /// Initializes a new instance of the ErrorMessageResult class.
    /// </summary>
    /// <param name="ex">The exception.</param>
    public ErrorMessageResult(Exception ex) : this(ex, ex?.GetType()?.Name)
    {
    }

    /// <summary>
    /// Initializes a new instance of the ErrorMessageResult class.
    /// </summary>
    /// <param name="ex">The exception.</param>
    /// <param name="errorCode">The error code.</param>
    public ErrorMessageResult(Exception ex, string errorCode) : base(ex?.Message)
    {
        ErrorCode = errorCode;
        if (ex == null) return;
        var innerEx = ex?.InnerException;
        if (ex is AggregateException aggEx && aggEx.InnerExceptions != null)
        {
            if (aggEx.InnerExceptions.Count == 1)
            {
                innerEx = aggEx.InnerExceptions[0];
            }
            else
            {
                Details = aggEx.InnerExceptions.Select(ele => ele?.Message).Where(ele => ele != null).ToList();
                return;
            }
        }

        if (innerEx == null) return;
        Details = new List<string>
        {
            innerEx.Message
        };
        var msg = innerEx.InnerException?.Message;
        if (string.IsNullOrWhiteSpace(msg)) return;
        Details.Add(msg);
        msg = innerEx.InnerException.InnerException?.Message;
        if (string.IsNullOrWhiteSpace(msg)) return;
        Details.Add(msg);
    }

    /// <summary>
    /// Initializes a new instance of the ErrorMessageResult class.
    /// </summary>
    /// <param name="message">The message.</param>
    public ErrorMessageResult(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the ErrorMessageResult class.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="errorCode">The error code.</param>
    public ErrorMessageResult(string message, string errorCode) : base(message)
    {
        ErrorCode = errorCode;
    }

    /// <summary>
    /// Gets or sets the offset of the result.
    /// </summary>
    [DataMember(Name = TokenInfo.ErrorCodeProperty, EmitDefaultValue = false)]
    [JsonPropertyName(TokenInfo.ErrorCodeProperty)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [Description("The error code.")]
    public string ErrorCode
    {
        get => GetCurrentProperty<string>();
        set => SetCurrentProperty(value);
    }

    /// <summary>
    /// Gets or sets the URL of help link.
    /// </summary>
    [DataMember(Name = "link", EmitDefaultValue = false)]
    [JsonPropertyName("link")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [Description("The help link URL for the error details.")]
    public string LinkUrl
    {
        get => GetCurrentProperty<string>();
        set => SetCurrentProperty(value);
    }

    /// <summary>
    /// Gets or sets the offset of the result.
    /// </summary>
    [DataMember(Name = "details", EmitDefaultValue = false)]
    [JsonPropertyName("details")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [Description("The error details.")]
    public List<string> Details
    {
        get => GetCurrentProperty<List<string>>();
        set => SetCurrentProperty(value);
    }

    /// <summary>
    /// Gets or sets the additional info.
    /// </summary>
    [DataMember(Name = "info")]
    [JsonPropertyName("info")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [Description("The additional information of the result.")]
    public JsonObjectNode AdditionalInfo
    {
        get => GetCurrentProperty<JsonObjectNode>();
        set => SetCurrentProperty(value);
    }
}
