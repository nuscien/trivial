using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Trivial.Data;
using Trivial.Reflection;
using Trivial.Tasks;
using Trivial.Text;

namespace Trivial.Users;

/// <summary>
/// The user group item information.
/// </summary>
public class BaseUserGroupItemInfo : BaseSecurityEntityInfo
{
    /// <summary>
    /// Initializes a new instance of the BaseUserGroupItemInfo class.
    /// </summary>
    public BaseUserGroupItemInfo()
        : base(SecurityEntityTypes.Group)
    {
    }

    /// <summary>
    /// Initializes a new instance of the BaseUserGroupItemInfo class.
    /// </summary>
    public BaseUserGroupItemInfo(string id, string nickname, Uri avatar = null)
        : base(SecurityEntityTypes.Group, id, nickname, avatar)
    {
    }

    /// <summary>
    /// Initializes a new instance of the BaseUserGroupItemInfo class.
    /// </summary>
    /// <param name="json">The JSON object to parse.</param>
    public BaseUserGroupItemInfo(JsonObjectNode json)
        : base(SecurityEntityTypes.Group, json)
    {
        if (json == null) return;
        DefaultMembershipPolicy = json.TryGetEnumValue<UserGroupMembershipPolicies>("memberPolicy") ?? UserGroupMembershipPolicies.Forbidden;
    }

    /// <summary>
    /// Gets or sets the membership policy.
    /// </summary>
    public UserGroupMembershipPolicies DefaultMembershipPolicy
    {
        get => GetCurrentProperty<UserGroupMembershipPolicies>();
        set => SetCurrentProperty(value);
    }

    /// <summary>
    /// Converts to JSON object.
    /// </summary>
    /// <returns>A JSON object.</returns>
    public override JsonObjectNode ToJson()
    {
        var json = base.ToJson();
        if (DefaultMembershipPolicy != 0) json.SetValue("memberPolicy", DefaultMembershipPolicy.ToString());
        return json;
    }

    /// <summary>
    /// Converts the JSON raw back.
    /// </summary>
    /// <param name="value">The source value.</param>
    /// <returns>The request instance.</returns>
    public static implicit operator BaseUserGroupItemInfo(JsonObjectNode value)
        => value is null ? null : new(value);
}
