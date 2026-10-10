using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
/// The error result with message.
/// </summary>
[DataContract]
[Guid("ED51EAB1-B281-46EF-9AFD-7B4E75EF9F4D")]
public class ErrorMessageResult : MessageResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorMessageResult"/> class.
    /// </summary>
    public ErrorMessageResult()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorMessageResult"/> class.
    /// </summary>
    /// <param name="ex">The exception.</param>
    public ErrorMessageResult(Exception ex) : this(ex, ex?.GetType()?.Name)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorMessageResult"/> class.
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
    /// Initializes a new instance of the <see cref="ErrorMessageResult"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    public ErrorMessageResult(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorMessageResult"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="errorCode">The error code.</param>
    public ErrorMessageResult(string message, string errorCode) : base(message)
    {
        ErrorCode = errorCode;
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

    /// <summary>
    /// Converts to the data result.
    /// </summary>
    /// <typeparam name="T">The type of data.</typeparam>
    /// <returns>The data result converted.</returns>
    public DataResult<T> ToData<T>()
        => new ErrorDataResult<T>(ErrorCode, Message)
        {
            Details = Details,
            LinkUrl = LinkUrl,
        };

    /// <summary>
    /// Converts to the data result.
    /// </summary>
    /// <typeparam name="T">The type of data.</typeparam>
    /// <returns>The data result converted.</returns>
    public DataResult<T> ToData<T>(T data)
        => new ErrorDataResult<T>(ErrorCode, data, Message)
        {
            Details = Details,
            LinkUrl = LinkUrl,
        };

    /// <summary>
    /// Converts to the data result.
    /// </summary>
    /// <typeparam name="TData">The type of data.</typeparam>
    /// <typeparam name="TInfo">The type of additional information.</typeparam>
    /// <returns>The data result converted.</returns>
    public DataResult<TData, TInfo> ToData<TData, TInfo>()
        => new ErrorDataResult<TData, TInfo>(ErrorCode, Message)
        {
            Details = Details,
            LinkUrl = LinkUrl,
        };

    /// <summary>
    /// Converts to the data result.
    /// </summary>
    /// <typeparam name="TData">The type of data.</typeparam>
    /// <typeparam name="TInfo">The type of additional information.</typeparam>
    /// <returns>The data result converted.</returns>
    public DataResult<TData, TInfo> ToData<TData, TInfo>(TData data, TInfo additional)
        => new ErrorDataResult<TData, TInfo>(ErrorCode, data, additional, Message)
        {
            Details = Details,
            LinkUrl = LinkUrl,
        };
}

/// <summary>
/// The error data result.
/// </summary>
/// <typeparam name="T">The type of data.</typeparam>
[DataContract]
internal class ErrorDataResult<T> : DataResult<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorDataResult{T}"/> class.
    /// </summary>
    public ErrorDataResult()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorDataResult{T}"/> class.
    /// </summary>
    /// <param name="errorCode">The error code.</param>
    /// <param name="message">The message.</param>
    public ErrorDataResult(string errorCode, string message)
        : base(message)
    {
        ErrorCode = errorCode;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorDataResult{T}"/> class.
    /// </summary>
    /// <param name="errorCode">The error code.</param>
    /// <param name="data">The data.</param>
    /// <param name="message">The message.</param>
    public ErrorDataResult(string errorCode, T data, string message)
        : base(data, message)
    {
        ErrorCode = errorCode;
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
}

/// <summary>
/// The error data result.
/// </summary>
/// <typeparam name="TData">The type of data.</typeparam>
/// <typeparam name="TInfo">The type of additional information.</typeparam>
[DataContract]
internal class ErrorDataResult<TData, TInfo> : DataResult<TData, TInfo>
{
    /// <summary>
    /// Initializes a new instance of the ErrorDataResult class.
    /// </summary>
    public ErrorDataResult()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorDataResult{TData, TInfo}"/> class.
    /// </summary>
    /// <param name="errorCode">The error code.</param>
    /// <param name="message">The message.</param>
    public ErrorDataResult(string errorCode, string message)
        : base(message)
    {
        ErrorCode = errorCode;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorDataResult{TData, TInfo}"/> class.
    /// </summary>
    /// <param name="errorCode">The error code.</param>
    /// <param name="data">The data.</param>
    /// <param name="additional">The object of additional information.</param>
    /// <param name="message">The message.</param>
    public ErrorDataResult(string errorCode, TData data, TInfo additional, string message)
        : base(data, additional, message)
    {
        ErrorCode = errorCode;
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
}
