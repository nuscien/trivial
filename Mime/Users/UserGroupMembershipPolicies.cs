using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.Users;

/// <summary>
/// User group membership policies.
/// </summary>
public enum UserGroupMembershipPolicies
{
    /// <summary>
    /// Disallow to join in.
    /// </summary>
    Forbidden = 0,

    /// <summary>
    /// Need apply for membership with approval.
    /// </summary>
    Application = 1,

    /// <summary>
    /// Allow to join in directly.
    /// </summary>
    Allow = 2,
}
