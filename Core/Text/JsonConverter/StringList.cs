using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Trivial.Maths;
using Trivial.Reflection;

namespace Trivial.Text;

/// <summary>
/// JSON string collection and json array converter.
/// </summary>
public class JsonStringListConverter : JsonConverter<IEnumerable<string>>, IJsonNodeSchemaCreationHandler<Type>
{
    private readonly char[] chars;
    private readonly bool trim;

    /// <summary>
    /// JSON string collection with white space and new line separated.
    /// </summary>
    /// <example>
    /// <code>
    /// { "tags": "tag1 tag2 tag3" }
    /// </code>
    /// <code>
    /// public class MyClass
    /// {
    ///   [JsonPropertyName("tags")]
    ///   [JsonConverter(typeof(WhiteSpaceSeparatedConverter))]
    ///   public IEnumerable&lt;string&gt; Tags { get; set; } 
    /// }
    /// </code>
    /// </example>
    public sealed class WhiteSpaceSeparatedConverter : JsonStringListConverter
    {
        private static readonly char[] splitChars = new[] { ' ', '　', '\r', '\n', '\t' };

        /// <summary>
        /// Initializes a new instance of the WhiteSpaceSeparatedConverter class.
        /// </summary>
        public WhiteSpaceSeparatedConverter() : base(splitChars, false)
        {
        }
    }

    /// <summary>
    /// JSON string collection with comma separated.
    /// </summary>
    /// <example>
    /// <code>
    /// { "tags": "tag1,tag2,tag3" }
    /// </code>
    /// <code>
    /// public class MyClass
    /// {
    ///   [JsonPropertyName("tags")]
    ///   [JsonConverter(typeof(CommaSeparatedConverter))]
    ///   public IEnumerable&lt;string&gt; Tags { get; set; }
    /// }
    /// </code>
    /// </example>
    public sealed class CommaSeparatedConverter : JsonStringListConverter
    {
        /// <summary>
        /// Initializes a new instance of the CommaSeparatedConverter class.
        /// </summary>
        public CommaSeparatedConverter() : base(',')
        {
        }
    }

    /// <summary>
    /// JSON string collection with semicolon separated.
    /// </summary>
    /// <example>
    /// <code>
    /// { "tags": "tag1;tag2;tag3" }
    /// </code>
    /// <code>
    /// public class MyClass
    /// {
    ///   [JsonPropertyName("tags")]
    ///   [JsonConverter(typeof(SemicolonSeparatedConverter))]
    ///   public IEnumerable&lt;string&gt; Tags { get; set; }
    /// }
    /// </code>
    /// </example>
    public sealed class SemicolonSeparatedConverter : JsonStringListConverter
    {
        /// <summary>
        /// Initializes a new instance of the SemicolonSeparatedConverter class.
        /// </summary>
        public SemicolonSeparatedConverter() : base(';')
        {
        }
    }

    /// <summary>
    /// JSON string collection with enumeration comma separated.
    /// </summary>
    /// <example>
    /// <code>
    /// { "tags": "tag1、tag2、tag3" }
    /// </code>
    /// <code>
    /// public class MyClass
    /// {
    ///   [JsonPropertyName("tags")]
    ///   [JsonConverter(typeof(EnumerationCommaSeparatedConverter))]
    ///   public IEnumerable&lt;string&gt; Tags { get; set; }
    /// }
    /// </code>
    /// </example>
    public sealed class EnumerationCommaSeparatedConverter : JsonStringListConverter
    {
        /// <summary>
        /// Initializes a new instance of the EnumerationCommaSeparatedConverter class.
        /// </summary>
        public EnumerationCommaSeparatedConverter() : base('、')
        {
        }
    }

    /// <summary>
    /// JSON string collection with semicolon separated.
    /// </summary>
    /// <example>
    /// <code>
    /// { "tags": "tag1；tag2；tag3" }
    /// </code>
    /// <code>
    /// public class MyClass
    /// {
    ///   [JsonPropertyName("tags")]
    ///   [JsonConverter(typeof(ChineseSemicolonSeparatedConverter))]
    ///   public IEnumerable&lt;string&gt; Tags { get; set; }
    /// }
    /// </code>
    /// </example>
    public sealed class ChineseSemicolonSeparatedConverter : JsonStringListConverter
    {
        /// <summary>
        /// Initializes a new instance of the SemicolonSeparatedConverter class.
        /// </summary>
        public ChineseSemicolonSeparatedConverter() : base('；')
        {
        }
    }

    /// <summary>
    /// JSON string collection with vertical bar separated.
    /// </summary>
    /// <example>
    /// <code>
    /// { "tags": "tag1|tag2|tag3" }
    /// </code>
    /// <code>
    /// public class MyClass
    /// {
    ///   [JsonPropertyName("tags")]
    ///   [JsonConverter(typeof(VerticalBarSeparatedConverter))]
    ///   public IEnumerable&lt;string&gt; Tags { get; set; }
    /// }
    /// </code>
    /// </example>
    public sealed class VerticalBarSeparatedConverter : JsonStringListConverter
    {
        /// <summary>
        /// Initializes a new instance of the VerticalBarSeparatedConverter class.
        /// </summary>
        public VerticalBarSeparatedConverter() : base('|')
        {
        }
    }

    /// <summary>
    /// Initializes a new instance of the JsonStringListConverter class.
    /// </summary>
    public JsonStringListConverter()
    {
    }

    /// <summary>
    /// Initializes a new instance of the JsonStringListConverter class.
    /// </summary>
    /// <param name="split">The split characters.</param>
    /// <param name="needTrim">true if need trim each string item and ignore empty; otherwise, false. Default value is true.</param>
    public JsonStringListConverter(char split, bool needTrim = true)
    {
        chars = new[] { split };
        trim = needTrim;
    }

    /// <summary>
    /// Initializes a new instance of the JsonStringListConverter class.
    /// </summary>
    /// <param name="split">The split characters.</param>
    /// <param name="needTrim">true if need trim each string item and ignore empty; otherwise, false. Default value is true.</param>
    public JsonStringListConverter(char[] split, bool needTrim = true)
    {
        chars = split;
        trim = needTrim;
    }

    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert)
        => typeof(IEnumerable<string>).IsAssignableFrom(typeToConvert);

    /// <inheritdoc />
    public override IEnumerable<string> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        JsonValues.SkipComments(ref reader);
        var col = new List<string>();
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (TryGetString(ref reader, out var str))
        {
            if (str != null)
            {
                if (chars != null && chars.Length > 0)
                {
                    IEnumerable<string> arr = str.Split(chars, StringSplitOptions.RemoveEmptyEntries);
                    if (trim) arr = arr.Select(ele => ele.Trim()).Where(ele => ele.Length > 0);
                    col.AddRange(arr);
                }
                else
                {
                    col.Add(str);
                }
            }
        }
        else if (reader.TokenType == JsonTokenType.StartArray)
        {
            while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
            {
                while (reader.TokenType == JsonTokenType.Comment || reader.TokenType == JsonTokenType.None)
                {
                    reader.Read();
                }

                if (!TryGetString(ref reader, out var value))
                {
                    throw new JsonException($"The token type is {reader.TokenType} but expect string or null.");
                }

                if (trim)
                {
                    if (value == null) continue;
                    value = value.Trim();
                    if (value.Length == 0) continue;
                }

                col.Add(value);
            }
        }

        return ToList(col, typeToConvert);
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, IEnumerable<string> value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        if (chars != null && chars.Length > 0)
        {
            var arr = value.Where(ele => !string.IsNullOrWhiteSpace(ele));
            if (trim) arr = arr.Select(ele => ele.Trim()).Where(ele => ele.Length > 0);
#if NETFRAMEWORK
            var str = string.Join(new string(chars[0], 1), arr);
#else
            var str = string.Join(chars[0], arr);
#endif
            writer.WriteStringValue(str);
            return;
        }

        writer.WriteStartArray();
        foreach (var item in value)
        {
            if (item == null) writer.WriteNullValue();
            else if (trim) writer.WriteStringValue(item.Trim());
            else writer.WriteStringValue(item);
        }

        writer.WriteEndArray();
    }

    JsonNodeSchemaDescription IJsonNodeSchemaCreationHandler<Type>.Convert(Type type, JsonNodeSchemaDescription result, NodePathBreadcrumb<Type> breadcrumb)
        => result is JsonArraySchemaDescription desc ? desc : new JsonArraySchemaDescription
        {
            DefaultItems = new JsonStringSchemaDescription()
        };

    private static IEnumerable<string> ToList(List<string> col, Type typeToConvert)
    {
        if (typeToConvert == typeof(List<string>) || typeToConvert.IsInterface)
        {
            return col;
        }

        if (typeToConvert == typeof(string[]))
        {
            return col.ToArray();
        }

        if (col.Count == 0)
        {
            try
            {
                return (IEnumerable<string>)Activator.CreateInstance(typeToConvert);
            }
            catch (MemberAccessException)
            {
            }
        }

        try
        {
            return (IEnumerable<string>)Activator.CreateInstance(typeToConvert, new[] { col });
        }
        catch (MemberAccessException ex)
        {
            if (!typeof(ICollection<string>).IsAssignableFrom(typeToConvert))
                throw new JsonException("The enumerable type is not supported.", ex);

            try
            {
                var c = (ICollection<string>)Activator.CreateInstance(typeToConvert);
                if (c.IsReadOnly) throw new JsonException("Cannot add items because the collection is read-only.", new NotSupportedException("The collection is read-only."));
                foreach (var item in col)
                {
                    c.Add(item);
                }

                return c;
            }
            catch (MemberAccessException ex2)
            {
                throw new JsonException("Cannot create the enumerable instance by no argument.", ex2);
            }
        }
    }

    private static bool TryGetString(ref Utf8JsonReader reader, out string result)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.Null:
            case JsonTokenType.False:
                result = null;
                return true;
            case JsonTokenType.String:
                result = reader.GetString();
                return true;
            case JsonTokenType.Number:
                result = reader.TryGetInt64(out var int64v)
                    ? int64v.ToString("g", CultureInfo.InvariantCulture)
                    : reader.GetDouble().ToString("g", CultureInfo.InvariantCulture);
                return true;
            default:
                result = null;
                return false;
        };
    }
}
