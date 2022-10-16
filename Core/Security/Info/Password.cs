// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Password.cs" company="Nanchang Jinchen Software Co., Ltd.">
//   Copyright (c) 2010 Nanchang Jinchen Software Co., Ltd. All rights reserved.
// </copyright>
// <summary>
//   The helper and extension for password.
// </summary>
// <author>Kingcean Tuan</author>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;

using Trivial.Data;

namespace Trivial.Security;

/// <summary>
/// The information about changing password.
/// </summary>
public class PasswordChanging
{
    /// <summary>
    /// Initializes a new instance of the PasswordChaning class.
    /// </summary>
    public PasswordChanging()
    {
    }

    /// <summary>
    /// Initializes a new instance of the PasswordChaning class.
    /// </summary>
    /// <param name="oldPassword">The old password.</param>
    /// <param name="newPassword">The new password.</param>
    public PasswordChanging(string oldPassword, string newPassword)
    {
        OldPassword = oldPassword?.ToSecure();
        NewPassword = newPassword?.ToSecure();
    }

    /// <summary>
    /// Initializes a new instance of the PasswordChaning class.
    /// </summary>
    /// <param name="oldPassword">The old password.</param>
    /// <param name="newPassword">The new password.</param>
    public PasswordChanging(SecureString oldPassword, SecureString newPassword)
    {
        OldPassword = oldPassword;
        NewPassword = newPassword;
    }

    /// <summary>
    /// Initializes a new instance of the PasswordChaning class.
    /// </summary>
    /// <param name="oldPassword">The old password.</param>
    /// <param name="newPassword">The new password.</param>
    public PasswordChanging(string oldPassword, SecureString newPassword)
    {
        OldPassword = oldPassword?.ToSecure();
        NewPassword = newPassword;
    }

    /// <summary>
    /// Deconstructor.
    /// </summary>
    ~PasswordChanging()
    {
        NewPassword = OldPassword = null;
    }

    /// <summary>
    /// Gets or sets the new password.
    /// </summary>
    public SecureString NewPassword { get; set; }

    /// <summary>
    /// Gets or sets the old password.
    /// </summary>
    public SecureString OldPassword { get; set; }

    /// <summary>
    /// Gets or sets the owner identifier. Null for current user or default instance.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Converts to the chagne event argument object.
    /// </summary>
    /// <returns>The chagne event argument object.</returns>
    public ChangeEventArgs<SecureString> ToChangeEventArgs()
        => new ChangeEventArgs<SecureString>(OldPassword, NewPassword, "password", true);
}
