using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Principal;
using System.Text;
using System.Text.Json.Serialization;

using Trivial.Security;
using Trivial.Text;

namespace Trivial.Users
{
    /// <summary>
    /// Security entity types.
    /// </summary>
    public enum SecurityEntityTypes : byte
    {
        /// <summary>
        /// Unknown.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// User.
        /// </summary>
        User = 1,

        /// <summary>
        /// User group.
        /// </summary>
        Group = 2,

        /// <summary>
        /// Service.
        /// </summary>
        Service = 3,

        /// <summary>
        /// Bot.
        /// </summary>
        Bot = 4,

        /// <summary>
        /// The special agent.
        /// </summary>
        Agent = 6,
    }
}
