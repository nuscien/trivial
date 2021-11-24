using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Trivial.IO
{
    /// <summary>
    /// The file association information.
    /// </summary>
    public class FileAssociationInfo
    {
        /// <summary>
        /// Gets or sets the file extension part.
        /// </summary>
        [JsonPropertyName("ext")]
        public string FileExtension { get; set; }

        /// <summary>
        /// Gets or sets the MIME content type.
        /// </summary>
        [JsonPropertyName("mime")]
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the default icon path.
        /// </summary>
        [JsonPropertyName("icon")]
        public string Icon { get; set; }

        /// <summary>
        /// Ges or sets the friendly name of the content type.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the defaul command.
        /// </summary>
        [JsonPropertyName("defaultcmd")]
        public FileOpenCommandInfo DefaultCommand { get; set; }

        /// <summary>
        /// Gets or sets the commands.
        /// </summary>
        [JsonPropertyName("cmds")]
        public List<FileOpenCommandInfo> Commands { get; set; } = new();
    }

    /// <summary>
    /// The file open command information.
    /// </summary>
    public class FileOpenCommandInfo
    {
        /// <summary>
        /// Gets or sets the command key.
        /// </summary>
        [JsonPropertyName("key")]
        public string Key { get; set; } = "Open";

        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        [JsonPropertyName("cmd")]
        public string Command { get; set; }

        /// <summary>
        /// Ges or sets the friendly name of the command.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
