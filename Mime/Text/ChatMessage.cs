using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trivial.Reflection;

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
    /// The message has been modified.
    /// </summary>
    Modified = 1,

    /// <summary>
    /// The streaming message which means the message is transferring by continious updating.
    /// </summary>
    Streaming = 2,

    /// <summary>
    /// The message has been removed.
    /// </summary>
    Removed = 3
}

/// <summary>
/// The chat message record.
/// </summary>
public class SimpleChatMessage : ObservableProperties
{
    /// <summary>
    /// Initializes a new instance of the SimpleChatMessage class.
    /// </summary>
    /// <param name="sender">The nickname of the sender.</param>
    /// <param name="message">The message text.</param>
    /// <param name="creation">The creation date time; or null if use now.</param>
    /// <param name="type">The message type.</param>
    /// <param name="data">The additional data; or null if create a new one.</param>
    public SimpleChatMessage(string sender, string message, DateTime? creation = null, string type = null, JsonObjectNode data = null)
    {
        var time = creation ?? DateTime.Now;
        SetProperty(nameof(SenderName), sender);
        SetProperty(nameof(Message), message);
        if (type != null) SetProperty(nameof(Type), type);
        SetProperty(nameof(CreationTime), time);
        SetProperty(nameof(LastModificationTime), time);
        if (data != null) Data = data;
    }

    /// <summary>
    /// Gets the sender.
    /// </summary>
    public string SenderName => GetCurrentProperty<string>();

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
    /// Gets the type.
    /// </summary>
    public string Type => GetCurrentProperty<string>();

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
        var model = new SimpleChatMessage(
            value.TryGetStringTrimmedValue("sender", true),
            value.TryGetStringValue("text") ?? value.TryGetStringValue("message"),
            value.TryGetDateTimeValue("created"),
            value.TryGetStringTrimmedValue("type", true),
            value.TryGetObjectValue("data"));
        return model.SenderName == null ? null : model;
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
            { "sender", value.SenderName },
            { "text", value.Message },
            { "created", value.CreationTime },
            { "type", value.Type },
            { "data", value.Data },
        };
    }
}
