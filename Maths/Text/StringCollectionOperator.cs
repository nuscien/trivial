using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.Text;

/// <summary>
/// The operators for boolean collection.
/// </summary>
public enum StringCollectionOperators : byte
{
    /// <summary>
    /// Returns an empty string.
    /// </summary>
    Empty = 0,

    /// <summary>
    /// Join each strings.
    /// </summary>
    Join = 1,

    /// <summary>
    /// Join strings in each line.
    /// </summary>
    Lines = 2,

    /// <summary>
    /// Join strings seperated by tab.
    /// </summary>
    Tabs = 3,

    /// <summary>
    /// Join strings seperated by semicolon.
    /// </summary>
    Tags = 4,

    /// <summary>
    /// Join strings seperated by comma.
    /// </summary>
    Commas = 5,

    /// <summary>
    /// Join strings seperated by dot.
    /// </summary>
    Dots = 6,

    /// <summary>
    /// Join strings seperated by slash.
    /// </summary>
    Slashes = 7,

    /// <summary>
    /// Join strings seperated by vertical line.
    /// </summary>
    VerticalLines = 8,

    /// <summary>
    /// Join strings seperated by white space.
    /// </summary>
    WhiteSpaces = 10,

    /// <summary>
    /// Join strings seperated by white space.
    /// </summary>
    DoubleWhiteSpaces = 11,

    /// <summary>
    /// Join strings seperated by white space.
    /// </summary>
    TripleWhiteSpaces = 12,

    /// <summary>
    /// Join strings seperated by ampersand with white space around.
    /// </summary>
    And = 13,

    /// <summary>
    /// Join strings seperated by spit poin with white space around.
    /// </summary>
    SplitPoints = 14,

    /// <summary>
    /// JSON array string format.
    /// </summary>
    JsonArray = 15,

    /// <summary>
    /// In bullet list. Each item is in a line with prefix of a split dot and a white space.
    /// </summary>
    Bullet = 16,

    /// <summary>
    /// In numbering list. Each item is in a line with prefix of one-based index and a tab.
    /// </summary>
    Numbering = 17,

    /// <summary>
    /// Returns the first one.
    /// </summary>
    First = 18,

    /// <summary>
    /// Returns the last one.
    /// </summary>
    Last = 19,

    /// <summary>
    /// Returns the first longest one.
    /// </summary>
    Longest = 20,

    /// <summary>
    /// Returns the last longest one.
    /// </summary>
    LastLongest = 21,

    /// <summary>
    /// Returns the first shortest one.
    /// </summary>
    Shortest = 22,

    /// <summary>
    /// Returns the last shortest one.
    /// </summary>
    LastShortest = 23,
}
