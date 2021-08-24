using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public string FileExtension { get; set; }

        /// <summary>
        /// Gets or sets the MIME content type.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the default icon path.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Ges or sets the friendly name of the content type.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the defaul command.
        /// </summary>
        public FileOpenCommandInfo DefaultCommand { get; set; }

        /// <summary>
        /// Gets or sets the commands.
        /// </summary>
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
        public string Key { get; set; } = "Open";

        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        public string Command { get; set; }

        /// <summary>
        /// Ges or sets the friendly name of the command.
        /// </summary>
        public string Name { get; set; }
    }
}
