using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Trivial.IO;

/// <summary>
/// The file association information.
/// </summary>
[DataContract]
public class FileAssociationInfo
{
    /// <summary>
    /// Gets or sets the file extension part.
    /// </summary>
    [JsonPropertyName("ext")]
    [DataMember(Name = "ext")]
    public string FileExtension { get; set; }

    /// <summary>
    /// Gets or sets the MIME content type.
    /// </summary>
    [JsonPropertyName("mime")]
    [DataMember(Name = "mime")]
    public string ContentType { get; set; }

    /// <summary>
    /// Gets or sets the default icon path.
    /// </summary>
    [JsonPropertyName("icon")]
    [DataMember(Name = "icon")]
    public string Icon { get; set; }

    /// <summary>
    /// Ges or sets the friendly name of the content type.
    /// </summary>
    [JsonPropertyName("name")]
    [DataMember(Name = "name")]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the defaul command.
    /// </summary>
    [JsonPropertyName("defaultcmd")]
    [DataMember(Name = "defaultcmd")]
    public FileOpenCommandInfo DefaultCommand { get; set; }

    /// <summary>
    /// Gets or sets the commands.
    /// </summary>
    [JsonPropertyName("cmds")]
    [DataMember(Name = "cmds")]
    public List<FileOpenCommandInfo> Commands { get; set; } = new();
}

/// <summary>
/// The file open command information.
/// </summary>
[DataContract]
public class FileOpenCommandInfo
{
    /// <summary>
    /// Gets or sets the command key.
    /// </summary>
    [JsonPropertyName("key")]
    [DataMember(Name = "key")]
    public string Key { get; set; } = "Open";

    /// <summary>
    /// Gets or sets the command.
    /// </summary>
    [JsonPropertyName("cmd")]
    [DataMember(Name = "cmd")]
    public string Command { get; set; }

    /// <summary>
    /// Ges or sets the friendly name of the command.
    /// </summary>
    [JsonPropertyName("name")]
    [DataMember(Name = "name")]
    public string Name { get; set; }
}
