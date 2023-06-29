using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Trivial.Collection;
using Trivial.Data;
using Trivial.Reflection;
using Trivial.Tasks;
using Trivial.Users;

namespace Trivial.Text;

/// <summary>
/// The modification kinds of the message.
/// </summary>
public enum ChatMessageModificationKinds : byte
{
    /// <summary>
    /// The message is neve modified.
    /// </summary>
    Original = 0,

    /// <summary>
    /// The streaming message which means the message is transferring by continious updating.
    /// </summary>
    Streaming = 1,

    /// <summary>
    /// The message has been modified by sender.
    /// </summary>
    Modified = 2,

    /// <summary>
    /// The message has been modified and is open to update by others.
    /// </summary>
    Collaborative = 3,

    /// <summary>
    /// The message has been removed by sender.
    /// </summary>
    Removed = 5,

    /// <summary>
    /// The message is banned by system.
    /// </summary>
    Ban = 6,

    /// <summary>
    /// Others.
    /// </summary>
    Others = 15
}

/// <summary>
/// The chat message record.
/// </summary>
public class ExtendedChatMessage : BaseResourceEntityInfo
{
    /// <summary>
    /// Initializes a new instance of the ExtendedChatMessage class.
    /// </summary>
    /// <param name="sender">The nickname of the sender.</param>
    /// <param name="message">The message text.</param>
    /// <param name="creation">The creation date time; or null if use now.</param>
    /// <param name="info">The additional information; or null if create a new one.</param>
    public ExtendedChatMessage(UserItemInfo sender, string message, DateTime? creation = null, JsonObjectNode info = null)
        : this(Guid.NewGuid(), sender, message, creation, info, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the ExtendedChatMessage class.
    /// </summary>
    /// <param name="id">The message identifier.</param>
    /// <param name="sender">The nickname of the sender.</param>
    /// <param name="message">The message text.</param>
    /// <param name="creation">The creation date time; or null if use now.</param>
    /// <param name="info">The additional information; or null if create a new one.</param>
    /// <param name="type">The message type.</param>
    public ExtendedChatMessage(Guid id, UserItemInfo sender, string message, DateTime? creation = null, JsonObjectNode info = null, string type = null)
        : this(ExtendedChatMessages.ToIdString(id), sender, message, creation, info, type)
    {
    }

    /// <summary>
    /// Initializes a new instance of the ExtendedChatMessage class.
    /// </summary>
    /// <param name="id">The message identifier.</param>
    /// <param name="sender">The nickname of the sender.</param>
    /// <param name="message">The message text.</param>
    /// <param name="creation">The creation date time; or null if use now.</param>
    /// <param name="info">The additional information; or null if create a new one.</param>
    /// <param name="type">The message type.</param>
    public ExtendedChatMessage(string id, UserItemInfo sender, string message, DateTime? creation = null, JsonObjectNode info = null, string type = null)
        : base(id)
    {
        var time = creation ?? DateTime.Now;
        SetProperty(nameof(Sender), sender);
        SetProperty(nameof(Message), message);
        if (!string.IsNullOrEmpty(type)) SetProperty(nameof(MessageType), type);
        SetProperty(nameof(CreationTime), time);
        SetProperty(nameof(LastModificationTime), time);
        Info = info ?? new();
    }

    /// <summary>
    /// Initializes a new instance of the ExtendedChatMessage class.
    /// </summary>
    /// <param name="json">The JSON object to parse.</param>
    public ExtendedChatMessage(JsonObjectNode json)
    {
        if (json is null) return;
        Id = json.TryGetStringTrimmedValue("id", true) ?? json.Id;
        SetProperty(nameof(Sender), (UserItemInfo)json.TryGetObjectValue("sender"));
        SetProperty(nameof(Message), json.TryGetStringValue("text") ?? json.TryGetStringValue("message"));
        SetProperty(nameof(MessageType), json.TryGetStringTrimmedValue("type", true));
        SetProperty(nameof(CreationTime), json.TryGetDateTimeValue("created") ?? DateTime.Now);
        SetProperty(nameof(LastModificationTime), json.TryGetDateTimeValue("modified") ?? DateTime.Now);
        Info = json.TryGetObjectValue("info") ?? new();
        Category = json.TryGetStringTrimmedValue("category", true);
        SetProperty("Data", json.TryGetObjectValue("data"));
    }

    /// <summary>
    /// Gets the sender.
    /// </summary>
    public UserItemInfo Sender => GetCurrentProperty<UserItemInfo>();

    /// <summary>
    /// Gets the plain text of the message.
    /// </summary>
    public string Message
    {
        get
        {
            return GetCurrentProperty<string>();
        }

        set
        {
            if (!SetCurrentProperty(value)) return;
            if (GetProperty<ChatMessageModificationKinds>(nameof(ModificationKind)) != ChatMessageModificationKinds.Original)
                SetProperty(nameof(ModificationKind), ChatMessageModificationKinds.Modified);
            SetProperty(nameof(LastModificationTime), DateTime.Now);
        }
    }

    /// <summary>
    /// Gets or sets the modification kind.
    /// </summary>
    public ChatMessageModificationKinds ModificationKind
    {
        get => GetCurrentProperty<ChatMessageModificationKinds>();
        set => SetCurrentProperty(value);
    }

    /// <summary>
    /// Gets the message type.
    /// </summary>
    public string MessageType => GetCurrentProperty<string>();

    /// <summary>
    /// Gets the creation date time.
    /// </summary>
    public DateTime CreationTime => GetCurrentProperty<DateTime>();

    /// <summary>
    /// Gets the creation date time.
    /// </summary>
    public DateTime LastModificationTime
    {
        get => GetCurrentProperty<DateTime>();
        set => SetCurrentProperty(value);
    }

    /// <summary>
    /// Gets the category.
    /// </summary>
    public string Category
    {
        get => GetCurrentProperty<string>();
        set => SetCurrentProperty(value);
    }

    /// <summary>
    /// Gets the additional data.
    /// </summary>
    public JsonObjectNode Info { get; }

    /// <summary>
    /// Converts to JSON object.
    /// </summary>
    /// <returns>A JSON object.</returns>
    public override JsonObjectNode ToJson()
    {
        var json = base.ToJson();
        json.SetValue("sender", Sender);
        json.SetValue("text", Message);
        json.SetValue("created", CreationTime);
        json.SetValue("modified", LastModificationTime);
        json.SetValue("type", MessageType);
        if (Info.Count > 0) json.SetValue("info", Info);
        if (!string.IsNullOrWhiteSpace(Category)) json.SetValue("category", Category);
        var data = GetProperty<object>("Data");
        if (data == null) return json;
        if (data is JsonObjectNode j)
        {
            json.SetValue("data", j);
            return json;
        }

        if (data is IJsonObjectHost joh)
        {
            json.SetValue("data", joh);
            return json;
        }

        try
        {
            var d = JsonSerializer.Serialize(data);
            if (string.IsNullOrWhiteSpace(d)) return json;
            json.SetValue("data", JsonObjectNode.ConvertFrom(d));
            return json;
        }
        catch (ArgumentException)
        {
        }
        catch (JsonException)
        {
        }
        catch (NotSupportedException)
        {
        }
        catch (InvalidOperationException)
        {
        }

        return json;
    }

    /// <summary>
    /// Returns a string which represents the object instance.
    /// </summary>
    /// <returns>A string which represents the object instance.</returns>
    public override string ToString()
    {
        var nickname = Sender?.Nickname?.Trim();
        if (string.IsNullOrEmpty(nickname)) nickname = "?";
        return $"{nickname} ({LastModificationTime}){Environment.NewLine}{Message}";
    }

    /// <summary>
    /// Converts the JSON raw back.
    /// </summary>
    /// <param name="value">The source value.</param>
    /// <returns>A model of the message.</returns>
    public static implicit operator ExtendedChatMessage(JsonObjectNode value)
    {
        if (value is null) return null;
        var type = value.TryGetStringTrimmedValue("type", true)?.ToLowerInvariant();
        if (type == null) return new(value);
        return type switch
        {
            ExtendedChatMessages.AttachmentLinkItemKey => new ExtendedChatMessage<AttachmentLinkItem>(value, json => new AttachmentLinkItem(json), type),
            ExtendedChatMessages.AttachmentLinkSetKey => new ExtendedChatMessage<AttachmentLinkSet>(value, json => new AttachmentLinkSet(json), type),
            ExtendedChatMessages.MarkdownKey or "text\\md" or "text/markdown" or "markdown" => new ExtendedChatMessage<ExtendedChatMessageTextData>(value, json => new ExtendedChatMessageTextData(json), ExtendedChatMessages.MarkdownKey),
            _ => new(value)
        };
    }

    /// <summary>
    /// Converts to JSON object.
    /// </summary>
    /// <param name="value">The JSON value.</param>
    /// <returns>A JSON object.</returns>
    public static explicit operator JsonObjectNode(ExtendedChatMessage value)
        => value?.ToJson();
}

/// <summary>
/// The chat message record.
/// </summary>
public class ExtendedChatMessage<T> : ExtendedChatMessage where T : class
{
    /// <summary>
    /// Initializes a new instance of the ExtendedChatMessage class.
    /// </summary>
    /// <param name="type">The message type.</param>
    /// <param name="sender">The nickname of the sender.</param>
    /// <param name="data">The message data.</param>
    /// <param name="message">The message text.</param>
    /// <param name="creation">The creation date time; or null if use now.</param>
    /// <param name="info">The additional information; or null if create a new one.</param>
    protected ExtendedChatMessage(string type, UserItemInfo sender, T data, string message, DateTime? creation = null, JsonObjectNode info = null)
        : this(type, Guid.NewGuid(), sender, data, message, creation, info)
    {
    }

    /// <summary>
    /// Initializes a new instance of the ExtendedChatMessage class.
    /// </summary>
    /// <param name="type">The message type.</param>
    /// <param name="id">The message identifier.</param>
    /// <param name="sender">The nickname of the sender.</param>
    /// <param name="data">The message data.</param>
    /// <param name="message">The message text.</param>
    /// <param name="creation">The creation date time; or null if use now.</param>
    /// <param name="info">The additional information; or null if create a new one.</param>
    protected ExtendedChatMessage(string type, Guid id, UserItemInfo sender, T data, string message, DateTime? creation = null, JsonObjectNode info = null)
        : this(type, ExtendedChatMessages.ToIdString(id), sender, data, message, creation, info)
    {
    }

    /// <summary>
    /// Initializes a new instance of the ExtendedChatMessage class.
    /// </summary>
    /// <param name="type">The message type.</param>
    /// <param name="id">The message identifier.</param>
    /// <param name="sender">The nickname of the sender.</param>
    /// <param name="data">The message data.</param>
    /// <param name="message">The message text.</param>
    /// <param name="creation">The creation date time; or null if use now.</param>
    /// <param name="info">The additional information; or null if create a new one.</param>
    protected internal ExtendedChatMessage(string type, string id, UserItemInfo sender, T data, string message, DateTime? creation = null, JsonObjectNode info = null)
        : base(id, sender, message, creation, info, type)
    {
        Data = data;
    }

    /// <summary>
    /// Initializes a new instance of the ExtendedChatMessage class.
    /// </summary>
    /// <param name="json">The JSON object to parse.</param>
    /// <param name="ignoreDataDeserialize">true if skip to deserialize the data; otherwise, false.</param>
    /// <param name="type">The message type to override; or null, if use the one in JSON input.</param>
    protected ExtendedChatMessage(JsonObjectNode json, bool ignoreDataDeserialize = false, string type = null)
        : base(json)
    {
        if (!string.IsNullOrWhiteSpace(type)) SetProperty(nameof(MessageType), type);
        if (ignoreDataDeserialize || json == null) return;
        var data = json.TryGetObjectValue("data");
        if (data == null) return;
        Data = data.Deserialize<T>();
    }

    /// <summary>
    /// Initializes a new instance of the ExtendedChatMessage class.
    /// </summary>
    /// <param name="json">The JSON object to parse.</param>
    /// <param name="dataConverter">The data converter.</param>
    /// <param name="type">The message type to override; or null, if use the one in JSON input.</param>
    protected internal ExtendedChatMessage(JsonObjectNode json, Func<JsonObjectNode, T> dataConverter, string type = null)
        : base(json)
    {
        if (!string.IsNullOrWhiteSpace(type)) SetProperty(nameof(MessageType), type);
        var data = json?.TryGetObjectValue("data");
        if (data == null)
        {
            var s = json.TryGetStringTrimmedValue("data", true);
            if (s == null || dataConverter == null) return;
            if (s.StartsWith("{") && s.StartsWith("}")) data = JsonObjectNode.TryParse(s);
            else if (s.StartsWith("[") || s.StartsWith("<")) return;
            else data = new()
            {
                { "value", s }
            };
        }

        if (data == null) return;
        Data = dataConverter == null ? data.Deserialize<T>() : dataConverter(data);
    }

    /// <summary>
    /// Gets the data of customized message details.
    /// </summary>
    public T Data
    {
        get => GetCurrentProperty<T>();
        protected set => SetCurrentProperty(value);
    }

    /// <summary>
    /// Converts to JSON object.
    /// </summary>
    /// <param name="value">The JSON value.</param>
    /// <returns>A JSON object.</returns>
    public static explicit operator JsonObjectNode(ExtendedChatMessage<T> value)
        => value?.ToJson();
}

/// <summary>
/// The rich text data for chat message.
/// </summary>
public class ExtendedChatMessageTextData : BaseObservableProperties, IJsonObjectHost
{
    /// <summary>
    /// Initializes a new instance of the ExtendedChatMessageTextData class.
    /// </summary>
    public ExtendedChatMessageTextData()
    {
        Info = new();
    }

    /// <summary>
    /// Initializes a new instance of the ExtendedChatMessageTextData class.
    /// </summary>
    /// <param name="value">The rich text.</param>
    public ExtendedChatMessageTextData(string value)
    {
        Value = value;
        Info = new();
    }

    /// <summary>
    /// Initializes a new instance of the ExtendedChatMessageTextData class.
    /// </summary>
    /// <param name="json">The JSON object to parse.</param>
    public ExtendedChatMessageTextData(JsonObjectNode json)
    {
        if (json == null) return;
        Value = json.TryGetStringValue("value");
        Info = json.TryGetObjectValue("info") ?? new();
    }

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    public string Value
    {
        get => GetCurrentProperty<string>();
        set => SetCurrentProperty(value);
    }

    /// <summary>
    /// Gets the additional information.
    /// </summary>
    public JsonObjectNode Info { get; }

    /// <summary>
    /// Converts to JSON object.
    /// </summary>
    /// <returns>A JSON object.</returns>
    public virtual JsonObjectNode ToJson()
    {
        var json = new JsonObjectNode
        {
            { "value", Value }
        };
        if (Info.Count > 0) json.SetValue("info", Info);
        return json;
    }

    /// <summary>
    /// Returns a string which represents the object instance.
    /// </summary>
    /// <returns>A string which represents the object instance.</returns>
    public override string ToString()
        => Value;

    /// <summary>
    /// Converts the JSON raw back.
    /// </summary>
    /// <param name="value">The source value.</param>
    /// <returns>A model of the message.</returns>
    public static implicit operator ExtendedChatMessageTextData(JsonObjectNode value)
        => new(value);

    /// <summary>
    /// Converts to JSON object.
    /// </summary>
    /// <param name="value">The JSON value.</param>
    /// <returns>A JSON object.</returns>
    public static explicit operator JsonObjectNode(ExtendedChatMessageTextData value)
        => value?.ToJson();
}

/// <summary>
/// The factory of the extended chat message data.
/// </summary>
/// <typeparam name="T">The type of data.</typeparam>
public abstract class BaseExtendedChatMessageDataFactory<T> where T : class
{
    /// <summary>
    /// Gets the message type.
    /// </summary>
    public string MessageType { get; }

    /// <summary>
    /// Generates the data instance from a JSON object.
    /// </summary>
    /// <param name="json">The JSON object node input.</param>
    /// <returns>The instance of the data.</returns>
    public virtual T Create(JsonObjectNode json)
        => json == null ? default : json.Deserialize<T>();

    /// <summary>
    /// Occurs on the message is created.
    /// </summary>
    /// <param name="message">The chat message created.</param>
    protected virtual void OnCreateMessage(ExtendedChatMessage<T> message)
    {
    }

    /// <summary>
    /// Creates a chat message record.
    /// </summary>
    /// <param name="sender">The nickname of the sender.</param>
    /// <param name="data">The message data.</param>
    /// <param name="message">The message text.</param>
    /// <param name="creation">The creation date time; or null if use now.</param>
    /// <param name="info">The additional information; or null if create a new one.</param>
    /// <returns>The chat message.</returns>
    public ExtendedChatMessage<T> CreateMessage(UserItemInfo sender, T data, string message, DateTime? creation = null, JsonObjectNode info = null)
        => CreateMessage(Guid.NewGuid(), sender, data, message, creation, info);

    /// <summary>
    /// Creates a chat message record.
    /// </summary>
    /// <param name="id">The message identifier.</param>
    /// <param name="sender">The nickname of the sender.</param>
    /// <param name="data">The message data.</param>
    /// <param name="message">The message text.</param>
    /// <param name="creation">The creation date time; or null if use now.</param>
    /// <param name="info">The additional information; or null if create a new one.</param>
    /// <returns>The chat message.</returns>
    public ExtendedChatMessage<T> CreateMessage(Guid id, UserItemInfo sender, T data, string message, DateTime? creation = null, JsonObjectNode info = null)
        => CreateMessage(ExtendedChatMessages.ToIdString(id), sender, data, message, creation, info);

    /// <summary>
    /// Creates a chat message record.
    /// </summary>
    /// <param name="id">The message identifier.</param>
    /// <param name="sender">The nickname of the sender.</param>
    /// <param name="data">The message data.</param>
    /// <param name="message">The message text.</param>
    /// <param name="creation">The creation date time; or null if use now.</param>
    /// <param name="info">The additional information; or null if create a new one.</param>
    /// <returns>The chat message.</returns>
    public ExtendedChatMessage<T> CreateMessage(string id, UserItemInfo sender, T data, string message, DateTime? creation = null, JsonObjectNode info = null)
    {
        if (string.IsNullOrWhiteSpace(MessageType)) return null;
        var obj = new ExtendedChatMessage<T>(MessageType, id, sender, data, message, creation, info);
        OnCreateMessage(obj);
        return obj;
    }

    /// <summary>
    /// Creates a chat message record.
    /// </summary>
    /// <param name="json">The JSON object to parse.</param>
    /// <returns>The chat message.</returns>
    public ExtendedChatMessage<T> CreateMessage(JsonObjectNode json)
    {
        var obj = new ExtendedChatMessage<T>(json, Create, MessageType);
        if (string.IsNullOrWhiteSpace(obj.MessageType)) return null;
        OnCreateMessage(obj);
        return obj;
    }
}

/// <summary>
/// The helper of extended chat message.
/// </summary>
public static class ExtendedChatMessages
{
    internal const string AttachmentLinkItemKey = "attachment\\item";
    internal const string AttachmentLinkSetKey = "attachment\\list";
    internal const string MarkdownKey = "text\\markdown";

    /// <summary>
    /// Creates a chat message record.
    /// </summary>
    /// <param name="sender">The nickname of the sender.</param>
    /// <param name="data">The message data.</param>
    /// <param name="message">The message text.</param>
    /// <param name="creation">The creation date time; or null if use now.</param>
    /// <param name="info">The additional information; or null if create a new one.</param>
    /// <returns>The chat message.</returns>
    public static ExtendedChatMessage<AttachmentLinkItem> Create(UserItemInfo sender, AttachmentLinkItem data, string message, DateTime? creation = null, JsonObjectNode info = null)
        => Create(Guid.NewGuid(), sender, data, message, creation, info);

    /// <summary>
    /// Creates a chat message record.
    /// </summary>
    /// <param name="id">The message identifier.</param>
    /// <param name="sender">The nickname of the sender.</param>
    /// <param name="data">The message data.</param>
    /// <param name="message">The message text.</param>
    /// <param name="creation">The creation date time; or null if use now.</param>
    /// <param name="info">The additional information; or null if create a new one.</param>
    /// <returns>The chat message.</returns>
    public static ExtendedChatMessage<AttachmentLinkItem> Create(Guid id, UserItemInfo sender, AttachmentLinkItem data, string message, DateTime? creation = null, JsonObjectNode info = null)
        => Create(ToIdString(id), sender, data, message, creation, info);

    /// <summary>
    /// Creates a chat message record.
    /// </summary>
    /// <param name="id">The message identifier.</param>
    /// <param name="sender">The nickname of the sender.</param>
    /// <param name="data">The message data.</param>
    /// <param name="message">The message text.</param>
    /// <param name="creation">The creation date time; or null if use now.</param>
    /// <param name="info">The additional information; or null if create a new one.</param>
    /// <returns>The chat message.</returns>
    public static ExtendedChatMessage<AttachmentLinkItem> Create(string id, UserItemInfo sender, AttachmentLinkItem data, string message, DateTime? creation = null, JsonObjectNode info = null)
        => new(AttachmentLinkItemKey, id, sender, data, message, creation, info);

    /// <summary>
    /// Creates a chat message record.
    /// </summary>
    /// <param name="json">The JSON object to parse.</param>
    /// <returns>The chat message.</returns>
    public static ExtendedChatMessage<AttachmentLinkItem> CreateAttachmentLinkItem(JsonObjectNode json)
        => new(json, data => new AttachmentLinkItem(data), AttachmentLinkItemKey);

    /// <summary>
    /// Creates a chat message record.
    /// </summary>
    /// <param name="sender">The nickname of the sender.</param>
    /// <param name="data">The message data.</param>
    /// <param name="message">The message text.</param>
    /// <param name="creation">The creation date time; or null if use now.</param>
    /// <param name="info">The additional information; or null if create a new one.</param>
    /// <returns>The chat message.</returns>
    public static ExtendedChatMessage<AttachmentLinkSet> Create(UserItemInfo sender, AttachmentLinkSet data, string message, DateTime? creation = null, JsonObjectNode info = null)
        => Create(Guid.NewGuid(), sender, data ?? new(), message, creation, info);

    /// <summary>
    /// Creates a chat message record.
    /// </summary>
    /// <param name="id">The message identifier.</param>
    /// <param name="sender">The nickname of the sender.</param>
    /// <param name="data">The message data.</param>
    /// <param name="message">The message text.</param>
    /// <param name="creation">The creation date time; or null if use now.</param>
    /// <param name="info">The additional information; or null if create a new one.</param>
    /// <returns>The chat message.</returns>
    public static ExtendedChatMessage<AttachmentLinkSet> Create(Guid id, UserItemInfo sender, AttachmentLinkSet data, string message, DateTime? creation = null, JsonObjectNode info = null)
        => Create(ToIdString(id), sender, data ?? new(), message, creation, info);

    /// <summary>
    /// Creates a chat message record.
    /// </summary>
    /// <param name="id">The message identifier.</param>
    /// <param name="sender">The nickname of the sender.</param>
    /// <param name="data">The message data.</param>
    /// <param name="message">The message text.</param>
    /// <param name="creation">The creation date time; or null if use now.</param>
    /// <param name="info">The additional information; or null if create a new one.</param>
    /// <returns>The chat message.</returns>
    public static ExtendedChatMessage<AttachmentLinkSet> Create(string id, UserItemInfo sender, AttachmentLinkSet data, string message, DateTime? creation = null, JsonObjectNode info = null)
        => new(AttachmentLinkSetKey, id, sender, data ?? new(), message, creation, info);

    /// <summary>
    /// Creates a chat message record.
    /// </summary>
    /// <param name="json">The JSON object to parse.</param>
    /// <returns>The chat message.</returns>
    public static ExtendedChatMessage<AttachmentLinkSet> CreateAttachmentLinkSet(JsonObjectNode json)
        => new(json, data => new AttachmentLinkSet(data), AttachmentLinkSetKey);

    /// <summary>
    /// Creates a chat message record.
    /// </summary>
    /// <param name="factory">The data factory.</param>
    /// <param name="sender">The nickname of the sender.</param>
    /// <param name="data">The message data.</param>
    /// <param name="message">The message text.</param>
    /// <param name="creation">The creation date time; or null if use now.</param>
    /// <param name="info">The additional information; or null if create a new one.</param>
    /// <returns>The chat message.</returns>
    public static ExtendedChatMessage<T> Create<T>(BaseExtendedChatMessageDataFactory<T> factory, UserItemInfo sender, T data, string message, DateTime? creation = null, JsonObjectNode info = null) where T : class
        => factory?.CreateMessage(Guid.NewGuid(), sender, data, message, creation, info);

    /// <summary>
    /// Creates a chat message record.
    /// </summary>
    /// <param name="factory">The data factory.</param>
    /// <param name="id">The message identifier.</param>
    /// <param name="sender">The nickname of the sender.</param>
    /// <param name="data">The message data.</param>
    /// <param name="message">The message text.</param>
    /// <param name="creation">The creation date time; or null if use now.</param>
    /// <param name="info">The additional information; or null if create a new one.</param>
    /// <returns>The chat message.</returns>
    public static ExtendedChatMessage<T> Create<T>(BaseExtendedChatMessageDataFactory<T> factory, Guid id, UserItemInfo sender, T data, string message, DateTime? creation = null, JsonObjectNode info = null) where T : class
        => factory?.CreateMessage(ToIdString(id), sender, data, message, creation, info);

    /// <summary>
    /// Creates a chat message record.
    /// </summary>
    /// <param name="factory">The data factory.</param>
    /// <param name="id">The message identifier.</param>
    /// <param name="sender">The nickname of the sender.</param>
    /// <param name="data">The message data.</param>
    /// <param name="message">The message text.</param>
    /// <param name="creation">The creation date time; or null if use now.</param>
    /// <param name="info">The additional information; or null if create a new one.</param>
    /// <returns>The chat message.</returns>
    public static ExtendedChatMessage<T> Create<T>(BaseExtendedChatMessageDataFactory<T> factory, string id, UserItemInfo sender, T data, string message, DateTime? creation = null, JsonObjectNode info = null) where T : class
        => factory?.CreateMessage(id, sender, data, message, creation, info);

    /// <summary>
    /// Creates a chat message record.
    /// </summary>
    /// <param name="factory">The data factory.</param>
    /// <param name="json">The JSON object to parse.</param>
    /// <returns>The chat message.</returns>
    public static ExtendedChatMessage<T> Create<T>(BaseExtendedChatMessageDataFactory<T> factory, JsonObjectNode json) where T : class
        => factory?.CreateMessage(json);

    /// <summary>
    /// Creates a chat message record.
    /// </summary>
    /// <param name="languageName">The lowercase name of the programming language without whitespace.</param>
    /// <param name="sender">The nickname of the sender.</param>
    /// <param name="codeSnippet">The code snippet.</param>
    /// <param name="creation">The creation date time; or null if use now.</param>
    /// <param name="info">The additional information; or null if create a new one.</param>
    /// <returns>The chat message.</returns>
    public static ExtendedChatMessage CreateCodeSnippet(string languageName, UserItemInfo sender, string codeSnippet, DateTime? creation = null, JsonObjectNode info = null)
        => new(Guid.NewGuid(), sender, codeSnippet, creation, info, string.Concat("code\\", languageName));

    /// <summary>
    /// Creates a chat message record.
    /// </summary>
    /// <param name="languageName">The lowercase name of the programming language without whitespace.</param>
    /// <param name="id">The message identifier.</param>
    /// <param name="sender">The nickname of the sender.</param>
    /// <param name="codeSnippet">The code snippet.</param>
    /// <param name="creation">The creation date time; or null if use now.</param>
    /// <param name="info">The additional information; or null if create a new one.</param>
    /// <returns>The chat message.</returns>
    public static ExtendedChatMessage CreateCodeSnippet(string languageName, Guid id, UserItemInfo sender, string codeSnippet, DateTime? creation = null, JsonObjectNode info = null)
        => new(id, sender, codeSnippet, creation, info, string.Concat("code\\", languageName));

    /// <summary>
    /// Creates a chat message record.
    /// </summary>
    /// <param name="languageName">The lowercase name of the programming language without whitespace.</param>
    /// <param name="id">The message identifier.</param>
    /// <param name="sender">The nickname of the sender.</param>
    /// <param name="codeSnippet">The code snippet.</param>
    /// <param name="creation">The creation date time; or null if use now.</param>
    /// <param name="info">The additional information; or null if create a new one.</param>
    /// <returns>The chat message.</returns>
    public static ExtendedChatMessage CreateCodeSnippet(string languageName, string id, UserItemInfo sender, string codeSnippet, DateTime? creation = null, JsonObjectNode info = null)
        => new(id, sender, codeSnippet, creation, info, string.Concat("code\\", languageName));

    /// <summary>
    /// Creates a chat message record.
    /// </summary>
    /// <param name="sender">The nickname of the sender.</param>
    /// <param name="markdown">The markdown text.</param>
    /// <param name="creation">The creation date time; or null if use now.</param>
    /// <param name="info">The additional information; or null if create a new one.</param>
    /// <returns>The chat message.</returns>
    public static ExtendedChatMessage<ExtendedChatMessageTextData> CreateMarkdown(UserItemInfo sender, string markdown, DateTime? creation = null, JsonObjectNode info = null)
        => CreateMarkdown(Guid.NewGuid(), sender, markdown, creation, info);

    /// <summary>
    /// Creates a chat message record.
    /// </summary>
    /// <param name="id">The message identifier.</param>
    /// <param name="sender">The nickname of the sender.</param>
    /// <param name="markdown">The markdown text.</param>
    /// <param name="creation">The creation date time; or null if use now.</param>
    /// <param name="info">The additional information; or null if create a new one.</param>
    /// <returns>The chat message.</returns>
    public static ExtendedChatMessage<ExtendedChatMessageTextData> CreateMarkdown(Guid id, UserItemInfo sender, string markdown, DateTime? creation = null, JsonObjectNode info = null)
        => CreateMarkdown(id, sender, new ExtendedChatMessageTextData(markdown), creation, info);

    /// <summary>
    /// Creates a chat message record.
    /// </summary>
    /// <param name="id">The message identifier.</param>
    /// <param name="sender">The nickname of the sender.</param>
    /// <param name="markdown">The markdown text.</param>
    /// <param name="creation">The creation date time; or null if use now.</param>
    /// <param name="info">The additional information; or null if create a new one.</param>
    /// <returns>The chat message.</returns>
    public static ExtendedChatMessage<ExtendedChatMessageTextData> CreateMarkdown(string id, UserItemInfo sender, string markdown, DateTime? creation = null, JsonObjectNode info = null)
        => CreateMarkdown(id, sender, new ExtendedChatMessageTextData(markdown), creation, info);

    /// <summary>
    /// Creates a chat message record.
    /// </summary>
    /// <param name="id">The message identifier.</param>
    /// <param name="sender">The nickname of the sender.</param>
    /// <param name="markdown">The markdown text.</param>
    /// <param name="creation">The creation date time; or null if use now.</param>
    /// <param name="info">The additional information; or null if create a new one.</param>
    /// <returns>The chat message.</returns>
    public static ExtendedChatMessage<ExtendedChatMessageTextData> CreateMarkdown(Guid id, UserItemInfo sender, ExtendedChatMessageTextData markdown, DateTime? creation = null, JsonObjectNode info = null)
        => CreateMarkdown(ToIdString(id), sender, markdown, creation, info);

    /// <summary>
    /// Creates a chat message record.
    /// </summary>
    /// <param name="id">The message identifier.</param>
    /// <param name="sender">The nickname of the sender.</param>
    /// <param name="markdown">The markdown text.</param>
    /// <param name="creation">The creation date time; or null if use now.</param>
    /// <param name="info">The additional information; or null if create a new one.</param>
    /// <returns>The chat message.</returns>
    public static ExtendedChatMessage<ExtendedChatMessageTextData> CreateMarkdown(string id, UserItemInfo sender, ExtendedChatMessageTextData markdown, DateTime? creation = null, JsonObjectNode info = null)
        => new(MarkdownKey, id, sender, markdown, null, creation, info);

    /// <summary>
    /// Creates a chat message record.
    /// </summary>
    /// <param name="json">The JSON object to parse.</param>
    /// <returns>The chat message.</returns>
    public static ExtendedChatMessage<ExtendedChatMessageTextData> CreateMarkdown(JsonObjectNode json)
        => new(json, data => new ExtendedChatMessageTextData(data), MarkdownKey);

    /// <summary>
    /// Tests if the message contains an attachment item.
    /// </summary>
    /// <param name="message">The chat message.</param>
    /// <returns>true if contains; otherwise, false.</returns>
    public static bool HasAttachment(this ExtendedChatMessage<AttachmentLinkItem> message)
        => message?.Data?.Link != null;

    /// <summary>
    /// Tests if contains the specific item.
    /// </summary>
    /// <param name="message">The chat message.</param>
    /// <param name="item">The attachment item.</param>
    /// <returns>true if contains; otherwise, false.</returns>
    public static bool ContainsItem(this ExtendedChatMessage<AttachmentLinkSet> message, AttachmentLinkItem item)
        => message != null && message.Data.Contains(item);

    /// <summary>
    /// Tests if contains the specific item.
    /// </summary>
    /// <param name="message">The chat message.</param>
    /// <param name="link">The attachment link.</param>
    /// <returns>true if contains; otherwise, false.</returns>
    public static bool ContainsItem(this ExtendedChatMessage<AttachmentLinkSet> message, Uri link)
        => message != null && message.Data.Contains(link);

    /// <summary>
    /// Tries to get the specific item.
    /// </summary>
    /// <param name="message">The chat message.</param>
    /// <param name="index">The zero-base index.</param>
    /// <returns>The attachment item; or null, if the index is not valid.</returns>
    public static AttachmentLinkItem TryGetItem(this ExtendedChatMessage<AttachmentLinkSet> message, int index)
        => message?.Data.TryGet(index);

    /// <summary>
    /// Adds an attachment.
    /// </summary>
    /// <param name="message">The chat message.</param>
    /// <param name="item">The attachment to add.</param>
    public static void AddItem(this ExtendedChatMessage<AttachmentLinkSet> message, AttachmentLinkItem item)
        => message?.Data.Add(item);

    /// <summary>
    /// Adds an attachment.
    /// </summary>
    /// <param name="message">The chat message.</param>
    /// <param name="link">The URI of the attachment.</param>
    /// <param name="mime">The MIME value of the attachment.</param>
    public static AttachmentLinkItem AddItem(this ExtendedChatMessage<AttachmentLinkSet> message, Uri link, string mime)
        => message?.Data.Add(link, mime);

    /// <summary>
    /// Adds an attachment.
    /// </summary>
    /// <param name="message">The chat message.</param>
    /// <param name="link">The URI of the attachment.</param>
    /// <param name="mime">The MIME value of the attachment.</param>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="thumbnail">The thumbnail URI of the attachment.</param>
    public static AttachmentLinkItem AddItem(this ExtendedChatMessage<AttachmentLinkSet> message, Uri link, string mime, string name, Uri thumbnail)
        => message?.Data.Add(link, mime, name, thumbnail);

    /// <summary>
    /// Adds an attachment.
    /// </summary>
    /// <param name="message">The chat message.</param>
    /// <param name="items">The attachment items to add.</param>
    /// <returns>The count of the item added.</returns>
    public static int AddItems(this ExtendedChatMessage<AttachmentLinkSet> message, IEnumerable<AttachmentLinkItem> items)
        => message?.Data.AddRange(items) ?? 0;

    /// <summary>
    /// Adds an attachment.
    /// </summary>
    /// <param name="message">The chat message.</param>
    /// <param name="index">The zero-based index to insert the specific item.</param>
    /// <param name="item">The attachment to add.</param>
    /// <exception cref="ArgumentOutOfRangeException">The index is not valid.</exception>
    public static void InsertItem(this ExtendedChatMessage<AttachmentLinkSet> message, int index, AttachmentLinkItem item)
        => message?.Data.Insert(index, item);

    /// <summary>
    /// Adds an attachment.
    /// </summary>
    /// <param name="message">The chat message.</param>
    /// <param name="index">The zero-based index to insert the specific item.</param>
    /// <param name="link">The URI of the attachment.</param>
    /// <param name="mime">The MIME value of the attachment.</param>
    /// <exception cref="ArgumentOutOfRangeException">The index is not valid.</exception>
    public static AttachmentLinkItem InsertItem(this ExtendedChatMessage<AttachmentLinkSet> message, int index, Uri link, string mime)
        => message?.Data.Insert(index, link, mime);

    /// <summary>
    /// Adds an attachment.
    /// </summary>
    /// <param name="message">The chat message.</param>
    /// <param name="index">The zero-based index to insert the specific item.</param>
    /// <param name="link">The URI of the attachment.</param>
    /// <param name="mime">The MIME value of the attachment.</param>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="thumbnail">The thumbnail URI of the attachment.</param>
    /// <exception cref="ArgumentOutOfRangeException">The index is not valid.</exception>
    public static AttachmentLinkItem InsertItem(this ExtendedChatMessage<AttachmentLinkSet> message, int index, Uri link, string mime, string name, Uri thumbnail)
        => message?.Data.Insert(index, link, mime, name, thumbnail);

    /// <summary>
    /// Tests if the message is in plain text.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <returns>Try if the message body is plain text; otherwise, false.</returns>
    public static bool IsPlainTextMessage(ExtendedChatMessage message)
    {
        if (message == null) return false;
        var type = message.MessageType?.ToLowerInvariant();
        if (string.IsNullOrEmpty(type)) return true;
        return type == "text" || type == "txt" || type == "text\\plain" || type == "text/plain";
    }

    /// <summary>
    /// Removes a specific attachment.
    /// </summary>
    /// <param name="message">The chat message.</param>
    /// <param name="item">The attachment to remove.</param>
    /// <returns>true if item is found and successfully removed; otherwise, false.</returns>
    public static bool RemoveItem(this ExtendedChatMessage<AttachmentLinkSet> message, AttachmentLinkItem item)
        => message != null && message.Data.Remove(item);

    internal static string ToIdString(Guid id)
        => id.ToString("N");
}
