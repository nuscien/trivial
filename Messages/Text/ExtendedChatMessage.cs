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
        : this(id.ToString("N"), sender, message, creation, kind, info)
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
        => value is null ? null : new(value);

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
public abstract class ExtendedChatMessage<T> : ExtendedChatMessage where T : class
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
        : this(kind, id.ToString("N"), sender, data, message, creation, info)
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
    protected ExtendedChatMessage(string kind, string id, UserItemInfo sender, T data, string message, DateTime? creation = null, JsonObjectNode info = null)
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
    /// Gets the data of customized message details.
    /// </summary>
    public T Data { get; protected set; }

    /// <summary>
    /// Converts to JSON object.
    /// </summary>
    /// <param name="value">The JSON value.</param>
    /// <returns>A JSON object.</returns>
    public static explicit operator JsonObjectNode(ExtendedChatMessage<T> value)
        => value?.ToJson();
}

/// <summary>
/// The chat message record with attachment.
/// </summary>
public class AttachmentExtendedChatMessage : ExtendedChatMessage<AttachmentLinkItem>
{
    private const string DefaultKind = "attachment\\item";

    /// <summary>
    /// Initializes a new instance of the AttachmentExtendedChatMessage class.
    /// </summary>
    /// <param name="sender">The nickname of the sender.</param>
    /// <param name="data">The message data.</param>
    /// <param name="message">The message text.</param>
    /// <param name="creation">The creation date time; or null if use now.</param>
    /// <param name="info">The additional information; or null if create a new one.</param>
    public AttachmentExtendedChatMessage(UserItemInfo sender, AttachmentLinkItem data, string message, DateTime? creation = null, JsonObjectNode info = null)
        : base(DefaultKind, Guid.NewGuid(), sender, data, message, creation, info)
    {
    }

    /// <summary>
    /// Initializes a new instance of the AttachmentExtendedChatMessage class.
    /// </summary>
    /// <param name="id">The message identifier.</param>
    /// <param name="sender">The nickname of the sender.</param>
    /// <param name="data">The message data.</param>
    /// <param name="message">The message text.</param>
    /// <param name="creation">The creation date time; or null if use now.</param>
    /// <param name="info">The additional information; or null if create a new one.</param>
    public AttachmentExtendedChatMessage(Guid id, UserItemInfo sender, AttachmentLinkItem data, string message, DateTime? creation = null, JsonObjectNode info = null)
        : base(DefaultKind, id, sender, data, message, creation, info)
    {
    }

    /// <summary>
    /// Initializes a new instance of the AttachmentExtendedChatMessage class.
    /// </summary>
    /// <param name="id">The message identifier.</param>
    /// <param name="sender">The nickname of the sender.</param>
    /// <param name="data">The message data.</param>
    /// <param name="message">The message text.</param>
    /// <param name="creation">The creation date time; or null if use now.</param>
    /// <param name="info">The additional information; or null if create a new one.</param>
    public AttachmentExtendedChatMessage(string id, UserItemInfo sender, AttachmentLinkItem data, string message, DateTime? creation = null, JsonObjectNode info = null)
        : base(DefaultKind, id, sender, data, message, creation, info)
    {
    }

    /// <summary>
    /// Initializes a new instance of the AttachmentExtendedChatMessage class.
    /// </summary>
    /// <param name="json">The JSON object to parse.</param>
    public AttachmentExtendedChatMessage(JsonObjectNode json)
        : this(json, false)
    {
    }

    /// <summary>
    /// Initializes a new instance of the AttachmentExtendedChatMessage class.
    /// </summary>
    /// <param name="kind">The message kind.</param>
    /// <param name="id">The message identifier.</param>
    /// <param name="sender">The nickname of the sender.</param>
    /// <param name="data">The message data.</param>
    /// <param name="message">The message text.</param>
    /// <param name="creation">The creation date time; or null if use now.</param>
    /// <param name="info">The additional information; or null if create a new one.</param>
    protected AttachmentExtendedChatMessage(string kind, string id, UserItemInfo sender, AttachmentLinkItem data, string message, DateTime? creation = null, JsonObjectNode info = null)
        : base(kind, id, sender, data, message, creation, info)
    {
    }

    /// <summary>
    /// Initializes a new instance of the AttachmentExtendedChatMessage class.
    /// </summary>
    /// <param name="kind">The message kind.</param>
    /// <param name="id">The message identifier.</param>
    /// <param name="sender">The nickname of the sender.</param>
    /// <param name="data">The message data.</param>
    /// <param name="message">The message text.</param>
    /// <param name="creation">The creation date time; or null if use now.</param>
    /// <param name="info">The additional information; or null if create a new one.</param>
    protected AttachmentExtendedChatMessage(string kind, Guid id, UserItemInfo sender, AttachmentLinkItem data, string message, DateTime? creation = null, JsonObjectNode info = null)
        : base(kind, id, sender, data, message, creation, info)
    {
    }

    /// <summary>
    /// Initializes a new instance of the AttachmentExtendedChatMessage class.
    /// </summary>
    /// <param name="json">The JSON object to parse.</param>
    /// <param name="ignoreDataDeserialize">true if skip to deserialize the data; otherwise, false.</param>
    /// <param name="kind">The message kind to override; or null, if use the one in JSON input.</param>
    protected AttachmentExtendedChatMessage(JsonObjectNode json, bool ignoreDataDeserialize = false, string kind = null)
        : base(json, true, kind ?? DefaultKind)
    {
        if (ignoreDataDeserialize) return;
        Data = new(json?.TryGetObjectValue("data"));
    }

    /// <summary>
    /// Gets a value indicating whether contains an attachment item.
    /// </summary>
    public bool HasAttachment => Data?.Link != null;

    /// <summary>
    /// Gets the URI of the attachment.
    /// </summary>
    public Uri AttachmentLink => Data?.Link;

    /// <summary>
    /// Gets the MIME value of the attachment.
    /// </summary>
    public string AttachmentContentType => Data?.ContentType;

    /// <summary>
    /// Converts the JSON raw back.
    /// </summary>
    /// <param name="value">The source value.</param>
    /// <returns>A model of the message.</returns>
    public static implicit operator AttachmentExtendedChatMessage(JsonObjectNode value)
        => value is null ? null : new(value);

    /// <summary>
    /// Converts to JSON object.
    /// </summary>
    /// <param name="value">The JSON value.</param>
    /// <returns>A JSON object.</returns>
    public static explicit operator JsonObjectNode(AttachmentExtendedChatMessage value)
        => value?.ToJson();

    /// <summary>
    /// Converts to JSON object.
    /// </summary>
    /// <param name="value">The JSON value.</param>
    /// <returns>A JSON object.</returns>
    public static explicit operator AttachmentListExtendedChatMessage(AttachmentExtendedChatMessage value)
        => value == null ? null : new(value.Id, value.Sender, new(value.Data), value.Message, value.CreationTime, value.Info)
        {
            LastModificationTime = value.LastModificationTime,
            ModificationKind = value.ModificationKind
        };
}

/// <summary>
/// The chat message record with attachment list.
/// </summary>
public class AttachmentListExtendedChatMessage : ExtendedChatMessage<AttachmentLinkSet>
{
    private const string DefaultKind = "attachment\\list";

    /// <summary>
    /// Initializes a new instance of the AttachmentListExtendedChatMessage class.
    /// </summary>
    /// <param name="sender">The nickname of the sender.</param>
    /// <param name="data">The message data.</param>
    /// <param name="message">The message text.</param>
    /// <param name="creation">The creation date time; or null if use now.</param>
    /// <param name="info">The additional information; or null if create a new one.</param>
    public AttachmentListExtendedChatMessage(UserItemInfo sender, AttachmentLinkSet data, string message, DateTime? creation = null, JsonObjectNode info = null)
        : base(DefaultKind, Guid.NewGuid(), sender, data ?? new(), message, creation, info)
    {
    }

    /// <summary>
    /// Initializes a new instance of the AttachmentListExtendedChatMessage class.
    /// </summary>
    /// <param name="id">The message identifier.</param>
    /// <param name="sender">The nickname of the sender.</param>
    /// <param name="data">The message data.</param>
    /// <param name="message">The message text.</param>
    /// <param name="creation">The creation date time; or null if use now.</param>
    /// <param name="info">The additional information; or null if create a new one.</param>
    public AttachmentListExtendedChatMessage(Guid id, UserItemInfo sender, AttachmentLinkSet data, string message, DateTime? creation = null, JsonObjectNode info = null)
        : base(DefaultKind, id, sender, data ?? new(), message, creation, info)
    {
    }

    /// <summary>
    /// Initializes a new instance of the AttachmentListExtendedChatMessage class.
    /// </summary>
    /// <param name="id">The message identifier.</param>
    /// <param name="sender">The nickname of the sender.</param>
    /// <param name="data">The message data.</param>
    /// <param name="message">The message text.</param>
    /// <param name="creation">The creation date time; or null if use now.</param>
    /// <param name="info">The additional information; or null if create a new one.</param>
    public AttachmentListExtendedChatMessage(string id, UserItemInfo sender, AttachmentLinkSet data, string message, DateTime? creation = null, JsonObjectNode info = null)
        : base(DefaultKind, id, sender, data ?? new(), message, creation, info)
    {
    }

    /// <summary>
    /// Initializes a new instance of the AttachmentListExtendedChatMessage class.
    /// </summary>
    /// <param name="json">The JSON object to parse.</param>
    public AttachmentListExtendedChatMessage(JsonObjectNode json)
        : this(json, false)
    {
    }

    /// <summary>
    /// Initializes a new instance of the AttachmentListExtendedChatMessage class.
    /// </summary>
    /// <param name="kind">The message kind.</param>
    /// <param name="id">The message identifier.</param>
    /// <param name="sender">The nickname of the sender.</param>
    /// <param name="data">The message data.</param>
    /// <param name="message">The message text.</param>
    /// <param name="creation">The creation date time; or null if use now.</param>
    /// <param name="info">The additional information; or null if create a new one.</param>
    protected AttachmentListExtendedChatMessage(string kind, string id, UserItemInfo sender, AttachmentLinkSet data, string message, DateTime? creation = null, JsonObjectNode info = null)
        : base(kind, id, sender, data ?? new(), message, creation, info)
    {
    }

    /// <summary>
    /// Initializes a new instance of the AttachmentListExtendedChatMessage class.
    /// </summary>
    /// <param name="json">The JSON object to parse.</param>
    /// <param name="ignoreDataDeserialize">true if skip to deserialize the data; otherwise, false.</param>
    /// <param name="kind">The message kind to override; or null, if use the one in JSON input.</param>
    protected AttachmentListExtendedChatMessage(JsonObjectNode json, bool ignoreDataDeserialize = false, string kind = null)
        : base(json, true, kind ?? DefaultKind)
    {
        if (ignoreDataDeserialize) return;
        Data = new(json?.TryGetObjectValue("data"));
    }

    /// <summary>
    /// Gets all attachment items.
    /// </summary>
    public ObservableCollection<AttachmentLinkItem> Items => Data.Items;

    /// <summary>
    /// Tests if contains the specific item.
    /// </summary>
    /// <param name="item">The attachment item.</param>
    /// <returns>true if contains; otherwise, false.</returns>
    public bool ContainsItem(AttachmentLinkItem item)
        => Data.Contains(item);

    /// <summary>
    /// Tests if contains the specific item.
    /// </summary>
    /// <param name="link">The attachment link.</param>
    /// <returns>true if contains; otherwise, false.</returns>
    public bool ContainsItem(Uri link)
        => Data.Contains(link);

    /// <summary>
    /// Tries to get the specific item.
    /// </summary>
    /// <param name="i">The zero-base index.</param>
    /// <returns>The attachment item; or null, if the index is not valid.</returns>
    public AttachmentLinkItem TryGetItem(int i)
        => Data.TryGet(i);

    /// <summary>
    /// Adds an attachment.
    /// </summary>
    /// <param name="item">The attachment to add.</param>
    public void AddItem(AttachmentLinkItem item)
        => Data.Add(item);

    /// <summary>
    /// Adds an attachment.
    /// </summary>
    /// <param name="link">The URI of the attachment.</param>
    /// <param name="mime">The MIME value of the attachment.</param>
    public AttachmentLinkItem AddItem(Uri link, string mime)
        => Data.Add(link, mime);

    /// <summary>
    /// Adds an attachment.
    /// </summary>
    /// <param name="link">The URI of the attachment.</param>
    /// <param name="mime">The MIME value of the attachment.</param>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="thumbnail">The thumbnail URI of the attachment.</param>
    public AttachmentLinkItem AddItem(Uri link, string mime, string name, Uri thumbnail)
        => Data.Add(link, mime, name, thumbnail);

    /// <summary>
    /// Adds an attachment.
    /// </summary>
    /// <param name="items">The attachment items to add.</param>
    /// <returns>The count of the item added.</returns>
    public int AddItems(IEnumerable<AttachmentLinkItem> items)
        => Data.AddRange(items);

    /// <summary>
    /// Adds an attachment.
    /// </summary>
    /// <param name="index">The zero-based index to insert the specific item.</param>
    /// <param name="item">The attachment to add.</param>
    /// <exception cref="ArgumentOutOfRangeException">The index is not valid.</exception>
    public void InsertItem(int index, AttachmentLinkItem item)
        => Data.Insert(index, item);

    /// <summary>
    /// Adds an attachment.
    /// </summary>
    /// <param name="index">The zero-based index to insert the specific item.</param>
    /// <param name="link">The URI of the attachment.</param>
    /// <param name="mime">The MIME value of the attachment.</param>
    /// <exception cref="ArgumentOutOfRangeException">The index is not valid.</exception>
    public AttachmentLinkItem InsertItem(int index, Uri link, string mime)
        => Data.Insert(index, link, mime);

    /// <summary>
    /// Adds an attachment.
    /// </summary>
    /// <param name="index">The zero-based index to insert the specific item.</param>
    /// <param name="link">The URI of the attachment.</param>
    /// <param name="mime">The MIME value of the attachment.</param>
    /// <param name="name">The name of the attachment.</param>
    /// <param name="thumbnail">The thumbnail URI of the attachment.</param>
    /// <exception cref="ArgumentOutOfRangeException">The index is not valid.</exception>
    public AttachmentLinkItem InsertItem(int index, Uri link, string mime, string name, Uri thumbnail)
        => Data.Insert(index, link, mime, name, thumbnail);

    /// <summary>
    /// Removes a specific attachment.
    /// </summary>
    /// <param name="item">The attachment to remove.</param>
    /// <returns>true if item is found and successfully removed; otherwise, false.</returns>
    public bool RemoveItem(AttachmentLinkItem item)
        => Data.Remove(item);

    /// <summary>
    /// Converts the JSON raw back.
    /// </summary>
    /// <param name="value">The source value.</param>
    /// <returns>A model of the message.</returns>
    public static implicit operator AttachmentListExtendedChatMessage(JsonObjectNode value)
        => value is null ? null : new(value);

    /// <summary>
    /// Converts to JSON object.
    /// </summary>
    /// <param name="value">The JSON value.</param>
    /// <returns>A JSON object.</returns>
    public static explicit operator JsonObjectNode(AttachmentListExtendedChatMessage value)
        => value?.ToJson();
}
