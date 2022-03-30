using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using Trivial.Collection;
using Trivial.Chemistry;
using Trivial.Text;

namespace Trivial.CommandLine;

/// <summary>
/// The kinds to represent the information of chemical element.
/// </summary>
public enum ChemicalElementRepresentationKinds
{
    /// <summary>
    /// Summary.
    /// </summary>
    Summary = 0,

    /// <summary>
    /// Detail information.
    /// </summary>
    Details = 1,

    /// <summary>
    /// Detail information and isotopes.
    /// </summary>
    DetailsAndIsotopes = 2
}

/// <summary>
/// The console style for chemical element.
/// </summary>
public class ChemicalElementConsoleStyle : ICloneable, IConsoleTextCreator<ChemicalElement, ChemicalElementRepresentationKinds>
{
    /// <summary>
    /// Gets or sets the fallback console color of element name.
    /// </summary>
    [JsonPropertyName("name2")]
    [JsonConverter(typeof(JsonIntegerEnumCompatibleConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ConsoleColor? NameConsoleColor { get; set; }

    /// <summary>
    /// Gets or sets the RGB color of element name.
    /// </summary>
    [JsonPropertyName("name")]
    [JsonConverter(typeof(JsonNumberConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Color? NameRgbColor { get; set; }

    /// <summary>
    /// Gets or sets the fallback console color of element symbol.
    /// </summary>
    [JsonPropertyName("symbol2")]
    [JsonConverter(typeof(JsonIntegerEnumCompatibleConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ConsoleColor? SymbolConsoleColor { get; set; } = ConsoleColor.Yellow;

    /// <summary>
    /// Gets or sets the RGB color of element symbol.
    /// </summary>
    [JsonPropertyName("symbol")]
    [JsonConverter(typeof(JsonNumberConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Color? SymbolRgbColor { get; set; } = Color.FromArgb(0xF9, 0xEE, 0x88);

    /// <summary>
    /// Gets or sets the fallback console color of atomic number.
    /// </summary>
    [JsonPropertyName("z2")]
    [JsonConverter(typeof(JsonIntegerEnumCompatibleConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ConsoleColor? AtomicNumberConsoleColor { get; set; }

    /// <summary>
    /// Gets or sets the RGB color of atomic number.
    /// </summary>
    [JsonPropertyName("z")]
    [JsonConverter(typeof(JsonNumberConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Color? AtomicNumberRgbColor { get; set; } = Color.FromArgb(160, 160, 240);

    /// <summary>
    /// Gets or sets the fallback console color of element additional property key.
    /// </summary>
    [JsonPropertyName("key2")]
    [JsonConverter(typeof(JsonIntegerEnumCompatibleConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ConsoleColor? PropertyKeyConsoleColor { get; set; }

    /// <summary>
    /// Gets or sets the RGB color of additional property key.
    /// </summary>
    [JsonPropertyName("key")]
    [JsonConverter(typeof(JsonNumberConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Color? PropertyKeyRgbColor { get; set; } = Color.FromArgb(0xCE, 0x91, 0x78);

    /// <summary>
    /// Gets or sets the fallback console color of additional property value.
    /// </summary>
    [JsonPropertyName("value2")]
    [JsonConverter(typeof(JsonIntegerEnumCompatibleConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ConsoleColor? PropertyValueConsoleColor { get; set; }

    /// <summary>
    /// Gets or sets the RGB color of additional property value.
    /// </summary>
    [JsonPropertyName("value")]
    [JsonConverter(typeof(JsonNumberConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Color? PropertyValueRgbColor { get; set; }

    /// <summary>
    /// Gets or sets the fallback console color of element isotope information.
    /// </summary>
    [JsonPropertyName("isotope2")]
    [JsonConverter(typeof(JsonIntegerEnumCompatibleConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ConsoleColor? IsotopeConsoleColor { get; set; }

    /// <summary>
    /// Gets or sets the RGB color of element isotope information.
    /// </summary>
    [JsonPropertyName("isotope")]
    [JsonConverter(typeof(JsonNumberConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Color? IsotopeRgbColor { get; set; } = Color.FromArgb(160, 160, 160);

    /// <summary>
    /// Gets or sets the fallback console color of punctuation marks.
    /// </summary>
    [JsonPropertyName("punctuation2")]
    [JsonConverter(typeof(JsonIntegerEnumCompatibleConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ConsoleColor? PunctuationConsoleColor { get; set; }

    /// <summary>
    /// Gets or sets the RGB color of element punctuation marks.
    /// </summary>
    [JsonPropertyName("punctuation")]
    [JsonConverter(typeof(JsonNumberConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Color? PunctuationRgbColor { get; set; } = Color.FromArgb(128, 128, 128);

    /// <summary>
    /// Gets or sets the fallback console color of background.
    /// </summary>
    [JsonPropertyName("back2")]
    [JsonConverter(typeof(JsonIntegerEnumCompatibleConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ConsoleColor? BackgroundConsoleColor { get; set; }

    /// <summary>
    /// Gets or sets the RGB color of background.
    /// </summary>
    [JsonPropertyName("back")]
    [JsonConverter(typeof(JsonNumberConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Color? BackgroundRgbColor { get; set; }

    /// <summary>
    /// Clears all colors.
    /// </summary>
    public void Clear()
    {
        NameConsoleColor = null;
        NameRgbColor = null;
        SymbolConsoleColor = null;
        SymbolRgbColor = null;
        AtomicNumberConsoleColor = null;
        AtomicNumberRgbColor = null;
        PropertyKeyConsoleColor = null;
        PropertyKeyRgbColor = null;
        PropertyValueConsoleColor = null;
        PropertyValueRgbColor = null;
        IsotopeConsoleColor = null;
        IsotopeRgbColor = null;
        PunctuationConsoleColor = null;
        PunctuationRgbColor = null;
        BackgroundConsoleColor = null;
        BackgroundRgbColor = null;
    }

    /// <summary>
    /// Clones an object.
    /// </summary>
    /// <returns>The object copied from this instance.</returns>
    public virtual ChemicalElementConsoleStyle Clone()
        => MemberwiseClone() as ChemicalElementConsoleStyle;

    /// <summary>
    /// Clones an object.
    /// </summary>
    /// <returns>The object copied from this instance.</returns>
    object ICloneable.Clone()
        => MemberwiseClone();

    /// <summary>
    /// Gets a value indicating whether it has already contained a line terminator.
    /// </summary>
    bool IConsoleTextCreator<ChemicalElement, ChemicalElementRepresentationKinds>.ContainsTerminator => true;

    /// <summary>
    /// Creates the console text collection based on this style.
    /// </summary>
    /// <param name="element">The chemical element.</param>
    /// <param name="kind">The presentation kind.</param>
    /// <returns>A collection of console text.</returns>
    IEnumerable<ConsoleText> IConsoleTextCreator<ChemicalElement, ChemicalElementRepresentationKinds>.CreateTextCollection(ChemicalElement element, ChemicalElementRepresentationKinds kind)
        => kind switch
        {
            ChemicalElementRepresentationKinds.Details => ChemistryCommandLine.ToConsoleText(this, element, false),
            ChemicalElementRepresentationKinds.DetailsAndIsotopes => ChemistryCommandLine.ToConsoleText(this, element, true),
            _ => ChemistryCommandLine.ToSimpleConsoleText(this, element, false),
        };
}
