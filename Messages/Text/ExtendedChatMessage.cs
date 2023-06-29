using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
    /// <param name="kind">The message kind.</param>
    /// <param name="info">The additional information; or null if create a new one.</param>
    public ExtendedChatMessage(UserItemInfo sender, string message, DateTime? creation = null, string kind = null, JsonObjectNode info = null)
        : this(Guid.NewGuid(), sender, message, creation, kind, info)
    {
    }

    /// <summary>
    /// Initializes a new instance of the ExtendedChatMessage class.
    /// </summary>
    /// <param name="id">The message identifier.</param>
    /// <param name="sender">The nickname of the sender.</param>
    /// <param name="message">The message text.</param>
    /// <param name="creation">The creation date time; or null if use now.</param>
    /// <param name="kind">The message kind.</param>
    /// <param name="info">The additional information; or null if create a new one.</param>
    public ExtendedChatMessage(Guid id, UserItemInfo sender, string message, DateTime? creation = null, string kind = null, JsonObjectNode info = null)
        : this(ExtendedChatMessages.ToIdString(id), sender, message, creation, kind, info)
    {
    }

    /// <summary>
    /// Initializes a new instance of the ExtendedChatMessage class.
    /// </summary>
    /// <param name="id">The message identifier.</param>
    /// <param name="sender">The nickname of the sender.</param>
    /// <param name="message">The message text.</param>
    /// <param name="creation">The creation date time; or null if use now.</param>
    /// <param name="kind">The message kind.</param>
    /// <param name="info">The additional information; or null if create a new one.</param>
    public ExtendedChatMessage(string id, UserItemInfo sender, string message, DateTime? creation = null, string kind = null, JsonObjectNode info = null)
        : base(id)
    {
        var time = creation ?? DateTime.Now;
        SetProperty(nameof(Sender), sender);
        SetProperty(nameof(Message), message);
        if (kind != null) SetProperty(nameof(Kind), kind);
        SetProperty(nameof(CreationTime), time);
        SetProperty(nameof(LastModificationTime), time);
        if (info != null) Info = info;
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
        SetProperty(nameof(Kind), json.TryGetStringTrimmedValue("type", true));
        SetProperty(nameof(CreationTime), json.TryGetDateTimeValue("created") ?? DateTime.Now);
        SetProperty(nameof(LastModificationTime), json.TryGetDateTimeValue("modified") ?? DateTime.Now);
        Info = json.TryGetObjectValue("info");
        SetProperty("Data", json.TryGetObjectValue("data"));
    }

    /// <summary>
    /// Gets the sender.
    /// </summary>
    public UserItemInfo Sender => GetCurrentProperty<UserItemInfo>();

    /// <summary>
    /// Gets the message.
    /// </summary>
    public string Message
    {
        get
        {
            return GetCurrentProperty<string>();
        }

        set
        {
            if (SetCurrentProperty(value) && GetProperty<ChatMessageModificationKinds>(nameof(ModificationKind)) == ChatMessageModificationKinds.Original)
                SetProperty(nameof(ModificationKind), ChatMessageModificationKinds.Modified);
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
    /// Gets the kind.
    /// </summary>
    public string Kind => GetCurrentProperty<string>();

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
        json.SetValue("type", Kind);
        json.SetValue("info", Info);
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
        var kind = value.TryGetStringTrimmedValue("kind", true)?.Trim();
        if (kind == null) return new(value);
        return kind switch
        {
            "attachment\\item" => new ExtendedChatMessage<AttachmentLinkItem>(value, json => new AttachmentLinkItem(json), kind),
            "attachment\\list" => new ExtendedChatMessage<AttachmentLinkSet>(value, json => new AttachmentLinkSet(json), kind),
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
    /// <param name="kind">The message kind.</param>
    /// <param name="sender">The nickname of the sender.</param>
    /// <param name="data">The message data.</param>
    /// <param name="message">The message text.</param>
    /// <param name="creation">The creation date time; or null if use now.</param>
    /// <param name="info">The additional information; or null if create a new one.</param>
    protected ExtendedChatMessage(string kind, UserItemInfo sender, T data, string message, DateTime? creation = null, JsonObjectNode info = null)
        : this(kind, Guid.NewGuid(), sender, data, message, creation, info)
    {
    }

    /// <summary>
    /// Initializes a new instance of the ExtendedChatMessage class.
    /// </summary>
    /// <param name="kind">The message kind.</param>
    /// <param name="id">The message identifier.</param>
    /// <param name="sender">The nickname of the sender.</param>
    /// <param name="data">The message data.</param>
    /// <param name="message">The message text.</param>
    /// <param name="creation">The creation date time; or null if use now.</param>
    /// <param name="info">The additional information; or null if create a new one.</param>
    protected ExtendedChatMessage(string kind, Guid id, UserItemInfo sender, T data, string message, DateTime? creation = null, JsonObjectNode info = null)
        : this(kind, ExtendedChatMessages.ToIdString(id), sender, data, message, creation, info)
    {
    }

    /// <summary>
    /// Initializes a new instance of the ExtendedChatMessage class.
    /// </summary>
    /// <param name="kind">The message kind.</param>
    /// <param name="id">The message identifier.</param>
    /// <param name="sender">The nickname of the sender.</param>
    /// <param name="data">The message data.</param>
    /// <param name="message">The message text.</param>
    /// <param name="creation">The creation date time; or null if use now.</param>
    /// <param name="info">The additional information; or null if create a new one.</param>
    protected internal ExtendedChatMessage(string kind, string id, UserItemInfo sender, T data, string message, DateTime? creation = null, JsonObjectNode info = null)
        : base(id, sender, message, creation, kind, info)
    {
        Data = data;
    }

    /// <summary>
    /// Initializes a new instance of the ExtendedChatMessage class.
    /// </summary>
    /// <param name="json">The JSON object to parse.</param>
    /// <param name="ignoreDataDeserialize">true if skip to deserialize the data; otherwise, false.</param>
    /// <param name="kind">The message kind to override; or null, if use the one in JSON input.</param>
    protected ExtendedChatMessage(JsonObjectNode json, bool ignoreDataDeserialize = false, string kind = null)
        : base(json)
    {
        if (!string.IsNullOrWhiteSpace(kind)) SetProperty(nameof(Kind), kind);
        if (ignoreDataDeserialize) return;
        var data = json?.TryGetObjectValue("data");
        if (data == null) return;
        Data = data.Deserialize<T>();
    }

    /// <summary>
    /// Initializes a new instance of the ExtendedChatMessage class.
    /// </summary>
    /// <param name="json">The JSON object to parse.</param>
    /// <param name="dataConverter">The data converter.</param>
    /// <param name="kind">The message kind to override; or null, if use the one in JSON input.</param>
    protected internal ExtendedChatMessage(JsonObjectNode json, Func<JsonObjectNode, T> dataConverter, string kind = null)
        : base(json)
    {
        if (!string.IsNullOrWhiteSpace(kind)) SetProperty(nameof(Kind), kind);
        var data = json?.TryGetObjectValue("data");
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
/// The factory of the extended chat message data.
/// </summary>
/// <typeparam name="T">The type of data.</typeparam>
public abstract class BaseExtendedChatMessageDataFactory<T> where T : class
{
    /// <summary>
    /// Gets the message kind.
    /// </summary>
    public string Kind { get; }

    /// <summary>
    /// Generates the data instance from a JSON object.
    /// </summary>
    /// <param name="json">The JSON object node input.</param>
    /// <returns>The instance of the data.</returns>
    public virtual T Create(JsonObjectNode json)
        => json == null ? default : json.Deserialize<T>();
}

/// <summary>
/// The helper of extended chat message.
/// </summary>
public static class ExtendedChatMessages
{
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
        => new("attachment\\item", id, sender, data, message, creation, info);

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
        => new("attachment\\list", id, sender, data ?? new(), message, creation, info);

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
        => Create(factory, Guid.NewGuid(), sender, data, message, creation, info);

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
        => Create(factory, ToIdString(id), sender, data, message, creation, info);

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
        => string.IsNullOrWhiteSpace(factory?.Kind) ? null : new(factory.Kind, id, sender, data, message, creation, info);

    /// <summary>
    /// Creates a chat message record.
    /// </summary>
    /// <param name="factory">The data factory.</param>
    /// <param name="json">The JSON object to parse.</param>
    /// <returns>The chat message.</returns>
    public static ExtendedChatMessage<T> Create<T>(BaseExtendedChatMessageDataFactory<T> factory, JsonObjectNode json) where T : class
        => string.IsNullOrWhiteSpace(factory?.Kind) ? null : new(json, factory.Create, factory.Kind);

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
        => new(sender, codeSnippet, creation, string.Concat("code\\", languageName), info);

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
        => new(id, sender, codeSnippet, creation, string.Concat("code\\", languageName), info);

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
        => new(id, sender, codeSnippet, creation, string.Concat("code\\", languageName), info);

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
