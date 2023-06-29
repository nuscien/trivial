using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Trivial.Collection;
using Trivial.Reflection;
using Trivial.Tasks;
using Trivial.Users;

namespace Trivial.Text;

/// <summary>
/// The chat message thread, may be a user, group, bot or topic.
/// </summary>
public interface IExtendedChatThread
{
    /// <summary>
    /// Gets the identifier.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Gets the nickname.
    /// </summary>
    string Nickname { get; }

    /// <summary>
    /// Gets the URI of the avatar.
    /// </summary>
    Uri AvatarUri { get; }

    /// <summary>
    /// Gets a value indicating whether the thread is read only for current user.
    /// </summary>
    bool IsReadOnly { get; }

    /// <summary>
    /// Gets a value indicating whether the thread can contain multiple participators (the current user and more than 1 other participators).
    /// </summary>
    bool IsMultipleParticipatorsMode { get; }

    /// <summary>
    /// Gets a value indicating whether the thread is round mode to send message.
    /// </summary>
    bool IsRoundMode { get; }
}

/// <summary>
/// The chat message thread, may be a user, group, bot or topic.
/// </summary>
public abstract class BaseExtendedChatThread : BaseObservableProperties, IExtendedChatThread
{
    /// <summary>
    /// Initializes a new instance of the BaseExtendedChatThread class.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="nickname">The nickname.</param>
    /// <param name="avatar">The avatar.</param>
    /// <param name="isReadOnly">true if the thread is read-only for the current user; otherwise, false.</param>
    /// <param name="isMultipleParticipatorsMode">true if the thread can contain multiple participators (the current user and more than 1 other participators); otherwise, false.</param>
    /// <param name="isRoundMode">true if the current user can only send message one by one when the participator is available to receive; otherwise, false.</param>
    public BaseExtendedChatThread(string id, string nickname, Uri avatar, bool isReadOnly = false, bool isMultipleParticipatorsMode = false, bool isRoundMode = false)
    {
        SetProperty(nameof(Id), id);
        SetProperty(nameof(Nickname), nickname);
        SetProperty(nameof(AvatarUri), avatar);
        SetProperty(nameof(IsReadOnly), isReadOnly);
        SetProperty(nameof(IsMultipleParticipatorsMode), isMultipleParticipatorsMode);
        SetProperty(nameof(IsRoundMode), isRoundMode);
    }

    /// <summary>
    /// Gets the identifier.
    /// </summary>
    public virtual string Id => GetCurrentProperty<string>();

    /// <summary>
    /// Gets the nickname.
    /// </summary>
    public virtual string Nickname => GetCurrentProperty<string>();

    /// <summary>
    /// Gets the URI of the avatar.
    /// </summary>
    public virtual Uri AvatarUri => GetCurrentProperty<Uri>();

    /// <summary>
    /// Gets a value indicating whether the thread is read-only for current user.
    /// </summary>
    public virtual bool IsReadOnly => GetCurrentProperty<bool>();

    /// <summary>
    /// Gets a value indicating whether the thread can contain multiple participators (the current user and more than 1 other participators).
    /// </summary>
    public virtual bool IsMultipleParticipatorsMode => GetCurrentProperty<bool>();

    /// <summary>
    /// Gets a value indicating whether the current user can only send message one by one when the participator is available to receive.
    /// </summary>
    public virtual bool IsRoundMode => GetCurrentProperty<bool>();
}

/// <summary>
/// The chat message thread, may be a user, group, bot or topic.
/// </summary>
public class UserExtendedChatThread : IExtendedChatThread, INotifyPropertyChanged
{
    private readonly Dictionary<string, bool> toggles = new();
    private readonly Dictionary<string, string> flags = new();

    /// <summary>
    /// Initializes a new instance of the UserExtendedChatThread class.
    /// </summary>
    /// <param name="user"></param>
    public UserExtendedChatThread(UserItemInfo user)
    {
        User = user ?? new();
        User.PropertyChanged += OnUserPropertyChanged;
    }

    /// <summary>
    /// Deconstructor.
    /// </summary>
    ~UserExtendedChatThread()
    {
        var user = User;
        if (user == null) return;
        user.PropertyChanged -= OnUserPropertyChanged;
    }

    /// <summary>
    /// Adds or removes the event handler raised on property changed.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Gets the user.
    /// </summary>
    public UserItemInfo User { get; }

    /// <summary>
    /// Gets the identifier.
    /// </summary>
    public virtual string Id => User.Id;

    /// <summary>
    /// Gets the nickname.
    /// </summary>
    public virtual string Nickname => User.Nickname;

    /// <summary>
    /// Gets the URI of the avatar.
    /// </summary>
    public virtual Uri AvatarUri => User.AvatarUri;

    /// <summary>
    /// Gets the gender.
    /// </summary>
    public virtual Genders Gender => User.Gender;

    /// <summary>
    /// Gets the security entity type.
    /// </summary>
    public virtual SecurityEntityTypes SecurityEntityType => User.SecurityEntityType;

    /// <summary>
    /// Gets a value indicating whether the thread is read-only for current user.
    /// </summary>
    public virtual bool IsReadOnly => GetBooleanFlag(nameof(IsReadOnly)) ?? false;

    /// <summary>
    /// Gets a value indicating whether the thread can contain multiple participators (the current user and more than 1 other participators).
    /// </summary>
    public virtual bool IsMultipleParticipatorsMode => GetBooleanFlag(nameof(IsMultipleParticipatorsMode)) ?? false;

    /// <summary>
    /// Gets a value indicating whether the current user can only send message one by one when the participator is available to receive.
    /// </summary>
    public virtual bool IsRoundMode => GetBooleanFlag(nameof(IsRoundMode)) ?? false;

    /// <summary>
    /// Sets a flag.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value of the property.</param>
    protected void SetFlag(string key, string value)
    {
        flags[key] = value;
        PropertyChanged?.Invoke(this, new(key));
    }

    /// <summary>
    /// Sets a flag.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value of the property.</param>
    protected void SetFlag(string key, bool value)
    {
        toggles[key] = value;
        flags[key] = value ? JsonBooleanNode.TrueString : JsonBooleanNode.FalseString;
        PropertyChanged?.Invoke(this, new(key));
    }

    /// <summary>
    /// Sets a flag to false.
    /// </summary>
    /// <param name="key">The property key.</param>
    protected void SetFalse(string key)
        => SetFlag(key, false);

    /// <summary>
    /// Sets a flag to true.
    /// </summary>
    /// <param name="key">The property key.</param>
    protected void SetTrue(string key)
        => SetFlag(key, true);

    /// <summary>
    /// Gets the specific flag.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <returns>The value of the property.</returns>
    protected string GetFlag(string key)
        => flags.TryGetValue(key, out var s) ? s : null;

    /// <summary>
    /// Gets the specific flag.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <returns>true if the flag value is true; false if the flag value is false; null if does not exists or is not a boolean flag.</returns>
    protected bool? GetBooleanFlag(string key)
        => toggles.TryGetValue(key, out var b) ? b : null;

    private void OnUserPropertyChanged(object sender, PropertyChangedEventArgs e)
        => PropertyChanged?.Invoke(this, e);
}

/// <summary>
/// The chat message thread, may be a user, group, bot or topic.
/// </summary>
public class CommandGuidanceExtendedChatThread : UserExtendedChatThread
{
    /// <summary>
    /// Initializes a new instance of the CommandGuidanceExtendedChatThread class.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="client">The chat command guidance client.</param>
    /// <param name="nickname">The display name.</param>
    /// <param name="avatar">The avatar URI.</param>
    public CommandGuidanceExtendedChatThread(string id, BaseChatCommandGuidanceClient client, string nickname, Uri avatar)
        : base(new(id, nickname, Genders.Machine, avatar))
    {
        Client = client;
        SetTrue(nameof(IsRoundMode));
    }

    /// <summary>
    /// Gets the chat command guidance client.
    /// </summary>
    public BaseChatCommandGuidanceClient Client { get; }
}