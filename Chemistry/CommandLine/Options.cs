using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Trivial.Collection;
using Trivial.Chemistry;

namespace Trivial.CommandLine
{
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
    public class ChemicalElementConsoleStyle
    {
        /// <summary>
        /// Gets or sets the fallback console color of element name.
        /// </summary>
        public ConsoleColor? NameConsoleColor { get; set; }

        /// <summary>
        /// Gets or sets the RGB color of element name.
        /// </summary>
        public Color? NameRgbColor { get; set; }

        /// <summary>
        /// Gets or sets the fallback console color of element symbol.
        /// </summary>
        public ConsoleColor? SymbolConsoleColor { get; set; } = ConsoleColor.Yellow;

        /// <summary>
        /// Gets or sets the RGB color of element symbol.
        /// </summary>
        public Color? SymbolRgbColor { get; set; } = Color.FromArgb(0xF9, 0xEE, 0x88);

        /// <summary>
        /// Gets or sets the fallback console color of atomic number.
        /// </summary>
        public ConsoleColor? AtomicNumberConsoleColor { get; set; }

        /// <summary>
        /// Gets or sets the RGB color of atomic number.
        /// </summary>
        public Color? AtomicNumberRgbColor { get; set; } = Color.FromArgb(160, 160, 240);

        /// <summary>
        /// Gets or sets the fallback console color of element additional property key.
        /// </summary>
        public ConsoleColor? PropertyKeyConsoleColor { get; set; }

        /// <summary>
        /// Gets or sets the RGB color of additional property key.
        /// </summary>
        public Color? PropertyKeyRgbColor { get; set; } = Color.FromArgb(0xCE, 0x91, 0x78);

        /// <summary>
        /// Gets or sets the fallback console color of additional property value.
        /// </summary>
        public ConsoleColor? PropertyValueConsoleColor { get; set; }

        /// <summary>
        /// Gets or sets the RGB color of additional property value.
        /// </summary>
        public Color? PropertyValueRgbColor { get; set; }

        /// <summary>
        /// Gets or sets the fallback console color of element isotope information.
        /// </summary>
        public ConsoleColor? IsotopeConsoleColor { get; set; }

        /// <summary>
        /// Gets or sets the RGB color of element isotope information.
        /// </summary>
        public Color? IsotopeRgbColor { get; set; } = Color.FromArgb(160, 160, 160);

        /// <summary>
        /// Gets or sets the fallback console color of punctuation marks.
        /// </summary>
        public ConsoleColor? PunctuationConsoleColor { get; set; }

        /// <summary>
        /// Gets or sets the RGB color of element punctuation marks.
        /// </summary>
        public Color? PunctuationRgbColor { get; set; } = Color.FromArgb(128, 128, 128);

        /// <summary>
        /// Gets or sets the fallback console color of background.
        /// </summary>
        public ConsoleColor? BackgroundConsoleColor { get; set; }

        /// <summary>
        /// Gets or sets the RGB color of background.
        /// </summary>
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
    }
}
