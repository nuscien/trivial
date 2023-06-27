using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trivial.Collection;
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
public class SimpleChatMessage : BaseResourceObservableProperties
{
    /// <summary>
    /// Initializes a new instance of the SimpleChatMessage class.
    /// </summary>
    /// <param name="sender">The nickname of the sender.</param>
    /// <param name="message">The message text.</param>
    /// <param name="creation">The creation date time; or null if use now.</param>
    /// <param name="kind">The message kind.</param>
    /// <param name="data">The additional data; or null if create a new one.</param>
    public SimpleChatMessage(UserItemInfo sender, string message, DateTime? creation = null, string kind = null, JsonObjectNode data = null)
        : this(Guid.NewGuid(), sender, message, creation, kind, data)
    {
    }

    /// <summary>
    /// Initializes a new instance of the SimpleChatMessage class.
    /// </summary>
    /// <param name="id">The message identifier.</param>
    /// <param name="sender">The nickname of the sender.</param>
    /// <param name="message">The message text.</param>
    /// <param name="creation">The creation date time; or null if use now.</param>
    /// <param name="kind">The message kind.</param>
    /// <param name="data">The additional data; or null if create a new one.</param>
    public SimpleChatMessage(Guid id, UserItemInfo sender, string message, DateTime? creation = null, string kind = null, JsonObjectNode data = null)
        : this(id.ToString("N"), sender, message, creation, kind, data)
    {
    }

    /// <summary>
    /// Initializes a new instance of the SimpleChatMessage class.
    /// </summary>
    /// <param name="id">The message identifier.</param>
    /// <param name="sender">The nickname of the sender.</param>
    /// <param name="message">The message text.</param>
    /// <param name="creation">The creation date time; or null if use now.</param>
    /// <param name="kind">The message kind.</param>
    /// <param name="data">The additional data; or null if create a new one.</param>
    public SimpleChatMessage(string id, UserItemInfo sender, string message, DateTime? creation = null, string kind = null, JsonObjectNode data = null)
        : base(id)
    {
        var time = creation ?? DateTime.Now;
        SetProperty(nameof(Sender), sender);
        SetProperty(nameof(Message), message);
        if (kind != null) SetProperty(nameof(Kind), kind);
        SetProperty(nameof(CreationTime), time);
        SetProperty(nameof(LastModificationTime), time);
        if (data != null) Data = data;
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
    public JsonObjectNode Data { get; }

    /// <summary>
    /// Converts the JSON raw back.
    /// </summary>
    /// <param name="value">The source value.</param>
    /// <returns>A model of the message.</returns>
    public static implicit operator SimpleChatMessage(JsonObjectNode value)
    {
        if (value is null) return null;
        return new(
            value.Id,
            (UserItemInfo)value.TryGetObjectValue("sender"),
            value.TryGetStringValue("text") ?? value.TryGetStringValue("message"),
            value.TryGetDateTimeValue("created"),
            value.TryGetStringTrimmedValue("type", true),
            value.TryGetObjectValue("data"));
    }

    /// <summary>
    /// Converts to JSON object.
    /// </summary>
    /// <param name="value">The JSON value.</param>
    /// <returns>A JSON object.</returns>
    public static explicit operator JsonObjectNode(SimpleChatMessage value)
    {
        if (value is null) return null;
        return new JsonObjectNode
        {
            { "sender", (JsonObjectNode)value.Sender },
            { "text", value.Message },
            { "created", value.CreationTime },
            { "type", value.Kind },
            { "data", value.Data },
        };
    }
}
