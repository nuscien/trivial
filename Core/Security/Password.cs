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
using System.Text;

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
        public string NewPassword { get; set; }

        /// <summary>
        /// Gets or sets the old password.
        /// </summary>
        public string OldPassword { get; set; }

        /// <summary>
        /// Gets or sets the owner identifier. Null for current user or default instance.
        /// </summary>
        public string Id { get; set; }
    }
}
