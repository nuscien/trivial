using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using Trivial.Collection;
using Trivial.Text;

namespace Trivial.CommandLine;

/// <summary>
/// The highlight console text style.
/// </summary>
public class HighlightConsoleStyle : IConsoleTextPrettier
{
    /// <summary>
    /// Initialzies a new instance of the HighlightConsoleStyle class.
    /// </summary>
    /// <param name="normal">The normal style.</param>
    /// <param name="highlight">The highlight style.</param>
    /// <param name="q">The query string.</param>
    public HighlightConsoleStyle(ConsoleTextStyle normal, ConsoleTextStyle highlight, IEnumerable<string> q)
    {
        Normal = normal;
        Highlight = highlight;
        if (q != null) Query.AddRange(q.Where(ele => !string.IsNullOrEmpty(ele)));
    }

    /// <summary>
    /// Initialzies a new instance of the HighlightConsoleStyle class.
    /// </summary>
    /// <param name="normal">The normal style.</param>
    /// <param name="highlight">The highlight style.</param>
    /// <param name="q">The query string.</param>
    /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
    public HighlightConsoleStyle(ConsoleTextStyle normal, ConsoleTextStyle highlight, string q, StringComparison? comparisonType = null)
    {
        Normal = normal;
        Highlight = highlight;
        if (!string.IsNullOrEmpty(q)) Query.Add(q);
        if (comparisonType.HasValue) ComparisonType = comparisonType.Value;
    }

    /// <summary>
    /// Gets or sets the fallback foreground color.
    /// </summary>
    [JsonPropertyName("normal")]
    public ConsoleTextStyle Normal { get; } = new();

    /// <summary>
    /// Gets or sets the fallback background color.
    /// </summary>
    [JsonPropertyName("highlight")]
    public ConsoleTextStyle Highlight { get; } = new();

    /// <summary>
    /// Gets or sets a value indicating whether the text is strikeout.
    /// </summary>
    [JsonPropertyName("q")]
    public List<string> Query { get; } = new();

    /// <summary>
    /// Gets or sets one of the enumeration values that specifies the rules for the search.
    /// </summary>
    [JsonPropertyName("compare")]
    [JsonConverter(typeof(JsonIntegerEnumCompatibleConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public StringComparison ComparisonType { get; set; }

    /// <summary>
    /// The search starting position to search.
    /// </summary>
    [JsonPropertyName("start")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int StartPosition { get; set; }

#pragma warning disable IDE0057
    /// <summary>
    /// Creates the console text collection based on this style.
    /// </summary>
    /// <param name="s">The text.</param>
    /// <returns>A collection of console text.</returns>
    IEnumerable<ConsoleText> IConsoleTextPrettier.CreateTextCollection(string s)
    {
        var col = new List<ConsoleText>();
        var q = Query.Where(ele => !string.IsNullOrEmpty(ele)).ToList();
        var pos = StartPosition > 0 ? StartPosition : 0;
        while (true)
        {
            if (pos >= s.Length) break;
            var i = -1;
            var hl = string.Empty;
            foreach (var item in q)
            {
                var j = s.IndexOf(item, pos, ComparisonType);
                if (j < 0 || (i >= 0 && j > i) || (i == j && item.Length < hl.Length)) continue;
                i = j;
                hl = item;
            }

            if (i < 0) break;
            col.Add(s.Substring(pos, i - pos), Normal);
            col.Add(hl, Highlight);
            pos += hl.Length;
        }

        if (pos < s.Length) col.Add(s.Substring(pos), Normal);
        return col;
    }
#pragma warning restore IDE0057
}
