using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Trivial.Text;

namespace Trivial.CommandLine
{
    /// <summary>
    /// The conversation modes for command processing.
    /// </summary>
    public enum CommandConversationModes
    {
        /// <summary>
        /// Turns on when no arguments input; otherwise, turns off.
        /// </summary>
        Auto = 0,

        /// <summary>
        /// Turns off the conversation mode.
        /// </summary>
        Off = 1,

        /// <summary>
        /// Keeps to turns on.
        /// </summary>
        On = 2
    }

    /// <summary>
    /// The context of command conversation.
    /// </summary>
    public class CommandConversationContext
    {
        /// <summary>
        /// Gets the tracking identifier.
        /// </summary>
        public Guid Id { get; } = Guid.NewGuid();

        /// <summary>
        /// Gets the current conversation modes for command processing.
        /// </summary>
        public CommandConversationModes Mode { get; internal set; }

        /// <summary>
        /// Gets the shared data.
        /// </summary>
        public JsonObjectNode Data { get; } = new JsonObjectNode();

        /// <summary>
        /// Gets some of verb description registered.
        /// </summary>
        public IDictionary<string, string> Description { get; internal set; }

        /// <summary>
        /// Gets some of verb key registered.
        /// </summary>
        public IEnumerable<string> StaticKeys => Description.Keys;

        /// <summary>
        /// Gets the keys for exit.
        /// </summary>
        public IReadOnlyList<string> ExitKeys { get; internal set; }

        /// <summary>
        /// Gets the current command handler.
        /// </summary>
        public ICommandHandler Handler { get; internal set; }

        /// <summary>
        /// Gets the current processing date time.
        /// </summary>
        public DateTime ProcessingTime { get; internal set; } = DateTime.Now;
    }
}
