using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Trivial.Collection;
using Trivial.Reflection;
using Trivial.Text;

namespace Trivial.Data;

/// <summary>
/// The concise model interface.
/// </summary>
public interface IConciseModel
{
    /// <summary>
    /// Gets the identifier.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Gets the title.
    /// </summary>
    string Title { get; }

    /// <summary>
    /// Gets the description.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Gets the image URI (thumbnail or avatar).
    /// </summary>
    Uri ImageUri { get; }

    /// <summary>
    /// Gets the link.
    /// </summary>
    string Link { get; }

    /// <summary>
    /// Gets the keywords.
    /// </summary>
    public IReadOnlyList<string> Keywords { get; }
}

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
public class ConciseModel : BaseObservableProperties, IConciseModel
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
        if (keywords != null) Keywords = keywords as ObservableCollection<string> ?? new(keywords);
    }

    /// <summary>
    /// Initializes a new instance of the ConciseModel class.
    /// </summary>
    /// <param name="id">The identifier of the video.</param>
    /// <param name="title">The title of the video.</param>
    /// <param name="link">The URL of the video to play.</param>
    /// <param name="description">The video description.</param>
    /// <param name="keywords">The keywords of the video.</param>
    public ConciseModel(Guid id, string title, string link = null, string description = null, IEnumerable<string> keywords = null)
        : this(id.ToString("N"), title, link, description, keywords)
    {
    }

    /// <summary>
    /// Initializes a new instance of the ConciseModel class.
    /// </summary>
    /// <param name="id">The identifier of the video.</param>
    /// <param name="title">The title of the video.</param>
    /// <param name="keywords">The keywords of the video.</param>
    /// <param name="link">The URL of the video to play.</param>
    public ConciseModel(string id, string title, IEnumerable<string> keywords, string link = null)
        : this(id, title, link, null, keywords)
    {
    }

    /// <summary>
    /// Initializes a new instance of the ConciseModel class.
    /// </summary>
    /// <param name="id">The identifier of the video.</param>
    /// <param name="title">The title of the video.</param>
    /// <param name="keywords">The keywords of the video.</param>
    /// <param name="link">The URL of the video to play.</param>
    public ConciseModel(Guid id, string title, IEnumerable<string> keywords, string link = null)
        : this(id.ToString("N"), title, link, null, keywords)
    {
    }

    /// <summary>
    /// Initializes a new instance of the ConciseModel class.
    /// </summary>
    /// <param name="copy">The model to copy.</param>
    public ConciseModel(IConciseModel copy)
    {
        if (copy == null) return;
        Id = copy.Id;
        Title = copy.Title;
        Link = copy.Link;
        Description = copy.Description;
        ImageUri = copy.ImageUri;
        var keywords = copy.Keywords;
        if (keywords != null) Keywords = [.. keywords];
        if (copy is not ConciseModel model) return;
        Raw = model.Raw;
        Tag = model.Tag;
    }

    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    [JsonPropertyName("id")]
    [Description("The identifier of the item.")]
    public string Id
    {
        get => GetCurrentProperty<string>();
        set => SetCurrentProperty(value);
    }

    /// <summary>
    /// Gets or sets the title.
    /// </summary>
    [JsonPropertyName("title")]
    [Description("The title of the item.")]
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
    [Description("The short description of the item.")]
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
    [JsonPropertyName("image")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [Description("The thumbnail or avatar URL of the item.")]
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
    [Description("The URL to details page of the item.")]
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
    [Description("The keywords of the item.")]
    [JsonConverter(typeof(JsonStringListConverter.SemicolonSeparatedConverter))]
    public ObservableCollection<string> Keywords
    {
        get => GetCurrentProperty<ObservableCollection<string>>();
        set => SetCurrentProperty(value);
    }

    /// <summary>
    /// Gets or sets the keywords.
    /// </summary>
    [JsonIgnore]
    IReadOnlyList<string> IConciseModel.Keywords => Keywords;

    /// <summary>
    /// Gets the count of keywords.
    /// </summary>
    [JsonIgnore]
    public int KeywordCount
    {
        get
        {
            var keywords = Keywords;
            return keywords == null ? 0 : keywords.Count;
        }
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
    /// Tests if contains the specific keyword.
    /// </summary>
    /// <param name="value">The keyword to test.</param>
    /// <param name="comparison">One of the enumeration values that specifies how the strings will be compared.</param>
    /// <returns>true if contains; otherwise, false.</returns>
    public bool ContainKeyword(string value, StringComparison comparison)
    {
        var keywords = Keywords;
        if (keywords == null || string.IsNullOrEmpty(value)) return false;
        value = value.Trim();
        foreach (var keyword in keywords)
        {
            if (string.IsNullOrEmpty(keyword)) continue;
            if (keyword.Trim().Equals(value, comparison)) return true;
        }

        return false;
    }

    /// <summary>
    /// Tests if contains the specific keyword.
    /// </summary>
    /// <param name="value">The keyword to test.</param>
    /// <param name="ignoreCase">true if case insensitve; otherwise, false.</param>
    /// <returns>true if contains; otherwise, false.</returns>
    public bool ContainKeyword(string value, bool ignoreCase = false)
    {
        var keywords = Keywords;
        if (keywords == null || string.IsNullOrEmpty(value)) return false;
        if (!ignoreCase) return keywords.Contains(value);
        value = value.Trim();
        foreach (var keyword in keywords)
        {
            if (string.IsNullOrEmpty(keyword)) continue;
            if (keyword.Trim().Equals(value, StringComparison.OrdinalIgnoreCase)) return true;
        }

        return false;
    }

    /// <summary>
    /// Converts to JSON object.
    /// </summary>
    /// <param name="options">Options to control the reader behavior during parsing.</param>
    /// <returns>A JSON object instance.</returns>
    /// <exception cref="JsonException">Its property does not represent a valid single JSON object.</exception>
    public virtual JsonObjectNode ToJson(JsonSerializerOptions options = default)
        => JsonObjectNode.ConvertFrom(this, options);

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
