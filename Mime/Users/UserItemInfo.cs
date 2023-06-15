using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trivial.Data;
using Trivial.Reflection;
using Trivial.Tasks;
using Trivial.Text;

namespace Trivial.Users;

/// <summary>
/// The user item information.
/// </summary>
public class UserItemInfo : ObservableProperties
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    public string Id
    {
        get => GetCurrentProperty<string>();
        set => SetCurrentProperty(value);
    }

    /// <summary>
    /// Gets or sets the nickname.
    /// </summary>
    public string Nickname
    {
        get => GetCurrentProperty<string>();
        set => SetCurrentProperty(value);
    }

    /// <summary>
    /// Gets or sets the gender.
    /// </summary>
    public Genders Gender
    {
        get => GetCurrentProperty<Genders>();
        set => SetCurrentProperty(value);
    }

    /// <summary>
    /// Gets or sets the URI of avatar.
    /// </summary>
    public Uri AvatarUri
    {
        get => GetCurrentProperty<Uri>();
        set => SetCurrentProperty(value);
    }

    /// <summary>
    /// Converts to JSON object.
    /// </summary>
    /// <returns>A JSON object.</returns>
    public virtual JsonObjectNode ToJson()
        => new()
        {
            { "id", Id },
            { "nickname", Nickname },
            { "gender", Gender.ToString() },
            { "avatar", AvatarUri?.OriginalString },
        };

    /// <summary>
    /// Converts the JSON raw back.
    /// </summary>
    /// <param name="value">The source value.</param>
    /// <returns>The request instance.</returns>
    public static implicit operator UserItemInfo(JsonObjectNode value)
    {
        if (value is null) return null;
        var item = new UserItemInfo()
        {
            Id = value.TryGetStringTrimmedValue("id", true) ?? value.Id,
            Nickname = value.TryGetStringTrimmedValue("nickname", true),
            Gender = value.TryGetEnumValue<Genders>("gender") ?? Genders.Unknown,
            AvatarUri = value.TryGetUriValue("avatar")
        };
        if (value.TryGetBooleanValue("_raw") != false) item.SetProperty("_raw", value);
        return item;
    }

    /// <summary>
    /// Converts to JSON object.
    /// </summary>
    /// <param name="value">The JSON value.</param>
    /// <returns>A JSON object.</returns>
    public static explicit operator JsonObjectNode(UserItemInfo value)
        => value?.ToJson();
}
