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

namespace Trivial.Security
{
    /// <summary>
    /// The information about changing password.
    /// </summary>
    public class PasswordChanging
    {
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
        {
            return new ChangeEventArgs<SecureString>(OldPassword, NewPassword, "password", true);
        }
    }
}
