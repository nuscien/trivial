using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
public class UserItemInfo : BaseSecurityEntityInfo
{
    /// <summary>
    /// Initializes a new instance of the UserItemInfo class.
    /// </summary>
    public UserItemInfo()
        : base(SecurityEntityTypes.User)
    {
    }

    /// <summary>
    /// Initializes a new instance of the UserItemInfo class.
    /// </summary>
    public UserItemInfo(string id, string nickname, Genders gender = Genders.Unknown, Uri avatar = null)
        : base(gender == Genders.Machine ? SecurityEntityTypes.Bot : SecurityEntityTypes.User, id, nickname, avatar)
    {
        Gender = gender;
    }

    /// <summary>
    /// Initializes a new instance of the UserItemInfo class.
    /// </summary>
    /// <param name="json">The JSON object to parse.</param>
    public UserItemInfo(JsonObjectNode json)
        : base(json, GetSecurityEntityType, SecurityEntityTypes.User)
    {
        if (json == null) return;
        Gender = json.TryGetEnumValue<Genders>("gender") ?? Genders.Unknown;
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
    /// Converts to JSON object.
    /// </summary>
    /// <returns>A JSON object.</returns>
    public override JsonObjectNode ToJson()
    {
        var json = base.ToJson();
        json.SetValue("gender", Gender.ToString());
        return json;
    }

    /// <summary>
    /// Converts the JSON raw back.
    /// </summary>
    /// <param name="value">The source value.</param>
    /// <returns>The request instance.</returns>
    public static implicit operator UserItemInfo(JsonObjectNode value)
        => value is null ? null : new(value);

    private static SecurityEntityTypes GetSecurityEntityType(JsonObjectNode json)
    {
        var type = json?.TryGetStringTrimmedValue("gender", true)?.ToLowerInvariant();
        if (type == null) return SecurityEntityTypes.User;
        return type switch
        {
            "bot" or "robot" or "machine" or "6" => SecurityEntityTypes.Bot,
            "agent" => SecurityEntityTypes.Agent,
            _ => SecurityEntityTypes.User
        };
    }
}
