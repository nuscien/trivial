using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Trivial.Collection;
using Trivial.Reflection;
using Trivial.Text;

namespace Trivial.Data;

/// <summary>
/// The interface to convert the current object to consise model.
/// </summary>
public interface IConciseModelDescriptive
{
    /// <summary>
    /// Converts to the concise model content.
    /// </summary>
    /// <returns>The concise model instance.</returns>
    ConciseModel ToConciseModel();
}

/// <summary>
/// The concise model content.
/// </summary>
public class ConciseModel : BaseObservableProperties
{
    /// <summary>
    /// Initializes a new instance of the ConciseModel class.
    /// </summary>
    public ConciseModel()
    {
    }

    /// <summary>
    /// Initializes a new instance of the ConciseModel class.
    /// </summary>
    /// <param name="id">The identifier of the video.</param>
    /// <param name="title">The title of the video.</param>
    /// <param name="link">The URL of the video to play.</param>
    /// <param name="description">The video description.</param>
    /// <param name="keywords">The keywords of the video.</param>
    public ConciseModel(string id, string title, string link = null, string description = null, IEnumerable<string> keywords = null)
    {
        Id = id;
        Title = title;
        Link = link;
        Description = description;
        Keywords = keywords as ObservableCollection<string> ?? new(keywords);
    }

    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    [JsonPropertyName("id")]
    [Description("The identifier of the model.")]
    public string Id
    {
        get => GetCurrentProperty<string>();
        set => SetCurrentProperty(value);
    }

    /// <summary>
    /// Gets or sets the title.
    /// </summary>
    [JsonPropertyName("title")]
    [Description("The title of the model.")]
    public string Title
    {
        get => GetCurrentProperty<string>();
        set => SetCurrentProperty(value);
    }

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    [JsonPropertyName("desc")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [Description("The short description of the model.")]
    public string Description
    {
        get => GetCurrentProperty<string>();
        set => SetCurrentProperty(value);
    }

    /// <summary>
    /// Gets or sets the image URI (thumbnail or avatar).
    /// </summary>
    [JsonIgnore]
    public Uri ImageUri
    {
        get => GetCurrentProperty<Uri>();
        set => SetCurrentProperty(value);
    }

    /// <summary>
    /// Gets or sets the image URI (thumbnail or avatar).
    /// </summary>
    [JsonPropertyName("id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [Description("The thumbnail or avatar URL of the model.")]
    public string ImageUrl
    {
        get => ImageUri?.OriginalString;
        set => ImageUri = new(value, UriKind.RelativeOrAbsolute);
    }

    /// <summary>
    /// Gets or sets the link.
    /// </summary>
    [JsonPropertyName("url")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [Description("The URL to details page of the model.")]
    public string Link
    {
        get => GetCurrentProperty<string>();
        set => SetCurrentProperty(value);
    }

    /// <summary>
    /// Gets or sets the keywords.
    /// </summary>
    [JsonPropertyName("keywords")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [Description("The keywords of the model.")]
    public ObservableCollection<string> Keywords
    {
        get => GetCurrentProperty<ObservableCollection<string>>();
        set => SetCurrentProperty(value);
    }

    /// <summary>
    /// Gets or sets the raw data.
    /// </summary>
    [JsonPropertyName("raw")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [Description("The source object.")]
    public object Raw
    {
        get => GetCurrentProperty<object>();
        set => SetCurrentProperty(value);
    }

    /// <summary>
    /// Gets or sets the optional additional object.
    /// </summary>
    [JsonIgnore]
    public object Tag
    {
        get => GetCurrentProperty<object>();
        set => SetCurrentProperty(value);
    }

    /// <summary>
    /// Gets the string format of keywords.
    /// </summary>
    /// <param name="seperator">The seperator.</param>
    /// <param name="nullIfEmpty">true if return null for empty; otherwise, false.</param>
    /// <param name="suffix">The suffix.</param>
    /// <returns>The keywords string.</returns>
    public string GetKeywordString(string seperator, bool nullIfEmpty = false, string suffix = null)
    {
        var keywords = Keywords?.Select(ele => ele?.Trim())?.Where(ele => !string.IsNullOrEmpty(ele))?.ToList();
        if (keywords == null || keywords.Count < 1) return nullIfEmpty ? null : string.Empty;
        var s = string.Join(seperator, keywords);
        return string.IsNullOrWhiteSpace(suffix) ? s : string.Join(s, suffix);
    }

    /// <summary>
    /// Adds a keyword.
    /// </summary>
    /// <param name="value">The keyword to add.</param>
    public void AddKeyword(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return;
        var keywords = Keywords;
        if (keywords == null)
        {
            keywords = [];
            Keywords = keywords;
        }

        if (!keywords.Contains(value)) keywords.Add(value);
    }

    /// <summary>
    /// Removes a keyword.
    /// </summary>
    /// <param name="value">The keyword to remove.</param>
    /// <returns>true if item is successfully removed; otherwise, false. This method also returns false if item was not found in the original keyword collection.</returns>
    public bool RemoveKeyword(string value)
    {
        var keywords = Keywords;
        if (keywords == null) return false;
        return keywords.Remove(value);
    }

    /// <summary>
    /// Returns a string that represents the current model.
    /// </summary>
    /// <returns>A string that represents the current model.</returns>
    public override string ToString()
    {
        var col = new List<string>();
        if (!string.IsNullOrWhiteSpace(Title)) col.Add(Title);
        if (!string.IsNullOrWhiteSpace(Description)) col.Add(Description);
        if (!string.IsNullOrWhiteSpace(Link)) col.Add(Description);
        return col.Count < 1 ? (Id ?? string.Concat('(', GetType().Name, ')')) : string.Join(Environment.NewLine, col);
    }
}

/// <summary>
/// The model with observable name and value.
/// </summary>
/// <typeparam name="T">The type of the value.</typeparam>
[DataContract]
public class KeyValueObservableModel<T> : BaseObservableProperties
{
    /// <summary>
    /// Initializes a new instance of the KeyValueObservableModel class.
    /// </summary>
    public KeyValueObservableModel()
    {
    }

    /// <summary>
    /// Initializes a new instance of the KeyValueObservableModel class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="value">The value.</param>
    public KeyValueObservableModel(string name, T value)
    {
        Key = name;
        Value = value;
    }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    [DataMember(Name = "key")]
    [JsonPropertyName("key")]
    [Description("The property key.")]
    public string Key

    {
        get => GetCurrentProperty<string>();
        set => SetCurrentProperty(value);
    }

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    [DataMember(Name = "value")]
    [JsonPropertyName("value")]
    [Description("The value.")]
    public T Value
    {
        get => GetCurrentProperty<T>();
        set => SetCurrentProperty(value);
    }

    /// <summary>
    /// Gets or sets the optional additional object.
    /// </summary>
    [JsonIgnore]
    public object Tag
    {
        get => GetCurrentProperty<object>();
        set => SetCurrentProperty(value);
    }
}
