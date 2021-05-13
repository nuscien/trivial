﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Trivial.Text;

namespace Trivial.Chemistry
{
    /// <summary>
    /// The model of chemical element.
    /// </summary>
    public partial class ChemicalElement : IEquatable<ChemicalElement>
    {
        /// <summary>
        /// The maximum atomic number in each period.
        /// </summary>
        private readonly static int[] periodNumbers = new []{ 2, 10, 18, 36, 54, 86, 118, 168, 218, 290, 362, 460, 558, 686, 814, 976, 1138, 1438, 1738 };

        /// <summary>
        /// The element name.
        /// </summary>
        private readonly Func<string> name;

        /// <summary>
        /// Initializes a new instance of the ChemicalElement class.
        /// </summary>
        /// <param name="number">The atomic number (or proton number, symbol Z) of the chemical element. The number is one-based.</param>
        /// <param name="symbol">The element identifier.</param>
        /// <param name="englishName">The English name.</param>
        /// <param name="atomicWeight">The atomic weight in dalton (unified atomic mass unit).</param>
        internal ChemicalElement(int number, string symbol, string englishName, double atomicWeight)
        {
            symbol = symbol?.Trim();
            Symbol = string.IsNullOrEmpty(symbol) ? GetSymbol(number) : symbol;
            AtomicNumber = number;
            (Period, IndexInPeriod) = GetPeriod(number);
            Group = GetGroup(Period, IndexInPeriod);
            englishName = englishName?.Trim();
            EnglishName = string.IsNullOrEmpty(englishName) ? GetName(number) : englishName;
            AtomicWeight = atomicWeight;
            HasAtomicWeight = !double.IsNaN(atomicWeight);
        }

        /// <summary>
        /// Initializes a new instance of the ChemicalElement class.
        /// </summary>
        /// <param name="number">The atomic number (or proton number, symbol Z) of the chemical element. The number is one-based.</param>
        /// <param name="symbol">The element symbol.</param>
        /// <param name="englishName">The English name.</param>
        /// <param name="atomicWeight">The atomic weight in dalton (unified atomic mass unit).</param>
        internal ChemicalElement(int number, string symbol, string englishName, int atomicWeight)
            : this(number, symbol, englishName, double.NaN)
        {
            AtomicWeight = atomicWeight;
        }

        /// <summary>
        /// Initializes a new instance of the ChemicalElement class.
        /// </summary>
        /// <param name="number">The atomic number (or proton number, symbol Z) of the chemical element. The number is one-based.</param>
        /// <param name="symbol">The element symbol.</param>
        /// <param name="englishName">The English name.</param>
        /// <param name="name">The name resolver.</param>
        /// <param name="atomicWeight">The atomic weight in dalton (unified atomic mass unit).</param>
        public ChemicalElement(int number, string symbol, string englishName, Func<string> name, double atomicWeight)
            : this(number, symbol, englishName, atomicWeight)
        {
            if (name is null) return;
            this.name = name;
        }

        /// <summary>
        /// Initializes a new instance of the ChemicalElement class.
        /// </summary>
        /// <param name="number">The atomic number (or proton number, symbol Z) of the chemical element. The number is one-based.</param>
        /// <param name="symbol">The element symbol.</param>
        /// <param name="englishName">The English name.</param>
        /// <param name="name">The name.</param>
        /// <param name="atomicWeight">The atomic weight in dalton (unified atomic mass unit).</param>
        public ChemicalElement(int number, string symbol, string englishName, string name, double atomicWeight)
            : this(number, symbol, englishName, atomicWeight)
        {
            name = name?.Trim();
            if (!string.IsNullOrEmpty(name)) this.name = () => name;
        }

        /// <summary>
        /// Initializes a new instance of the ChemicalElement class.
        /// </summary>
        /// <param name="number">The atomic number (or proton number, symbol Z) of the chemical element. The number is one-based.</param>
        /// <param name="symbol">The element symbol.</param>
        /// <param name="name">The name.</param>
        /// <param name="isAlsoEnglishName">true if the parameter name is also an English name.</param>
        /// <param name="atomicWeight">The atomic weight in dalton (unified atomic mass unit).</param>
        public ChemicalElement(int number, string symbol, string name, bool isAlsoEnglishName, double atomicWeight)
            : this(number, symbol, isAlsoEnglishName ? name : null, name, atomicWeight)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ChemicalElement class.
        /// </summary>
        /// <param name="number">The atomic number (or proton number, symbol Z) of the chemical element. The number is one-based.</param>
        /// <param name="symbol">The element identifier.</param>
        /// <param name="englishName">The English name.</param>
        /// <param name="name">The name.</param>
        public ChemicalElement(int number, string symbol, string englishName, string name)
            : this(number, symbol, englishName, name, double.NaN)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ChemicalElement class.
        /// </summary>
        /// <param name="number">The atomic number (or proton number, symbol Z) of the chemical element. The number is one-based.</param>
        /// <param name="symbol">The element symbol.</param>
        /// <param name="name">The name.</param>
        /// <param name="isAlsoEnglishName">true if the parameter name is also an English name.</param>
        public ChemicalElement(int number, string symbol, string name, bool isAlsoEnglishName)
            : this(number, symbol, isAlsoEnglishName ? name : null, name, double.NaN)
        {
        }

        /// <summary>
        /// The element symbol.
        /// </summary>
        public string Symbol { get; }

        /// <summary>
        /// The name.
        /// </summary>
        public string Name
        {
            get
            {
                var s = name?.Invoke();
                if (!string.IsNullOrEmpty(s)) return s;
                var table = periodicTable;
                if (name is null && table != null && AtomicNumber < table.Length)
                {
                    try
                    {
                        s = ChemistryResource.ResourceManager.GetString("Element" + AtomicNumber.ToString("000"))?.Trim();
                        if (!string.IsNullOrEmpty(s)) return s;
                    }
                    catch (InvalidOperationException)
                    {
                    }
                    catch (System.Resources.MissingManifestResourceException)
                    {
                    }
                    catch (System.Resources.MissingSatelliteAssemblyException)
                    {
                    }
                }

                s = ChemistryResource.ElementX?.Trim();
                if (!string.IsNullOrEmpty(s) && s.Contains('{'))
                    return s
                        .Replace("{0}", AtomicNumber.ToString("g"))
                        .Replace("{number}", AtomicNumber.ToString("g"))
                        .Replace("{symbol}", Symbol);
                return EnglishName ?? Symbol ?? ChemistryResource.Element;
            }
        }

        /// <summary>
        /// The element name in English accepted by IUPAC.
        /// </summary>
        public string EnglishName { get; }

        /// <summary>
        /// Gets the atomic number (or proton number, symbol Z) of the chemical element.
        /// It is the one-based number of protons found in the nucleus of every atom of that element.
        /// </summary>
        public int AtomicNumber { get; }

        /// <summary>
        /// Gets the period which is a one-based index of horizontal row in the periodic table.
        /// </summary>
        public int Period { get; }

        /// <summary>
        /// Gets a zero-based index of the element in the period.
        /// </summary>
        public int IndexInPeriod { get; }

        /// <summary>
        /// Gets the IUPAC group (or family) which is a one-based index of vertical column in the periodic table.
        /// -1 for others (N/A) which only appears in Period 6 and later.
        /// </summary>
        public int Group { get; }

        /// <summary>
        /// Gets the atomic weight in dalton (unified atomic mass unit).
        /// </summary>
        public double AtomicWeight { get; }

        /// <summary>
        /// Gets a value indicating whether has atomic weight information.
        /// </summary>
        public bool HasAtomicWeight { get; }

        /// <summary>
        /// Gets a value indicating whether this element is valid.
        /// </summary>
        public bool IsValid()
            => Period > 0 || !string.IsNullOrEmpty(Symbol);

        /// <summary>
        /// Creates an isotope.
        /// </summary>
        /// <param name="atomicMassNumber">The atomic mass number (total protons and neutrons).</param>
        /// <returns>An isotope of this element with the specific numbers of neutrons.</returns>
        public Isotope Isotope(int atomicMassNumber)
            => new(this, atomicMassNumber, Math.Abs(atomicMassNumber - AtomicWeight) < 0.5 ? AtomicWeight : double.NaN);

        /// <summary>
        /// Creates an isotope.
        /// </summary>
        /// <param name="atomicMassNumber">The atomic mass number (total protons and neutrons).</param>
        /// <param name="atomicWeight">The atomic weight in dalton (unified atomic mass unit).</param>
        /// <returns>An isotope of this element with the specific numbers of neutrons.</returns>
        public Isotope Isotope(int atomicMassNumber, double atomicWeight)
            => new(this, atomicMassNumber, atomicWeight);

        /// <summary>
        /// Writes this instance to the specified writer as a JSON value.
        /// </summary>
        /// <param name="writer">The writer to which to write this instance.</param>
        public void WriteTo(Utf8JsonWriter writer)
        {
            var json = (JsonObject)this;
            json.WriteTo(writer);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="other">The object to compare with the current instance.</param>
        /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
        public bool Equals(ChemicalElement other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return AtomicNumber == other.AtomicNumber
                && Symbol == other.Symbol;
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="other">The object to compare with the current instance.</param>
        /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
        public bool Equals(JsonObject other)
        {
            if (other is null) return false;
            return AtomicNumber == other.TryGetInt32Value("number")
                && Symbol == other.TryGetStringValue("symbol");
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="other">The object to compare with the current instance.</param>
        /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
        public override bool Equals(object other)
        {
            if (other is null) return false;
            if (other is ChemicalElement element) return Equals(element);
            if (other is JsonObject json) return Equals(json);
            return false;
        }

        /// <summary>
        /// Peturns a string that represents the current chemical element information.
        /// </summary>
        /// <returns>A string that represents the current chemical element information.</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(Name);
            sb.Append(" (");
            if (AtomicNumber < 1)
            {
                sb.Append('?');
            }
            else
            {
                sb.Append('#');
                sb.Append(AtomicNumber);
                sb.Append(' ');
            }

            if (!Name.Equals(Symbol, StringComparison.OrdinalIgnoreCase))
            {
                sb.Append(Symbol);
            }

            sb.Append(')');
            return sb.ToString();
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Converts to a JSON object.
        /// </summary>
        /// <param name="element">The chemical element to convert.</param>
        public static explicit operator JsonObject(ChemicalElement element)
        {
            if (element is null || element.AtomicNumber < 1) return null;
            var json = new JsonObject
            {
                { "number", element.AtomicNumber },
                { "symbol", element.Symbol },
                { "period", element.Period },
                { "name", element.Name }
            };
            if (element.Group > 0)
                json.SetValue("group", element.Group);
            if (!double.IsNaN(element.AtomicWeight))
                json["weight"] = element.HasAtomicWeight ? new JsonDouble(element.AtomicWeight) : new JsonInteger(element.AtomicWeight);
            if (!element.EnglishName.Equals(element.Name))
                json.SetValue("name_en", element.EnglishName);
            return json;
        }

        /// <summary>
        /// Pluses two chemical elements.
        /// </summary>
        /// <param name="leftValue">The left value for addition operator.</param>
        /// <param name="rightValue">The right value for addition operator.</param>
        /// <returns>A result after addition.</returns>
        public static MolecularFormula operator +(ChemicalElement leftValue, ChemicalElement rightValue)
        {
            var isLeftEmpty = string.IsNullOrEmpty(leftValue?.Symbol);
            var isRightEmpty = string.IsNullOrEmpty(rightValue?.Symbol);
            if (isLeftEmpty && isRightEmpty) return null;
            if (isLeftEmpty) return new MolecularFormula(rightValue);
            if (isRightEmpty) return new MolecularFormula(leftValue);
            return new MolecularFormula(leftValue, 1, rightValue, 1);
        }

        /// <summary>
        /// Multiplies a chemical element and an integer number.
        /// </summary>
        /// <param name="leftValue">The left value for multiple operator.</param>
        /// <param name="rightValue">The right value for multiple operator.</param>
        /// <returns>A result after multiple.</returns>
        public static MolecularFormula operator *(ChemicalElement leftValue, int rightValue)
        {
            if (string.IsNullOrEmpty(leftValue?.Symbol) || rightValue < 1) return null;
            return new MolecularFormula(leftValue, rightValue);
        }

        /// <summary>
        /// Multiplies a chemical element and an integer number.
        /// </summary>
        /// <param name="leftValue">The left value for multiple operator.</param>
        /// <param name="rightValue">The right value for multiple operator.</param>
        /// <returns>A result after multiple.</returns>
        public static MolecularFormula operator *(int leftValue, ChemicalElement rightValue)
        {
            if (string.IsNullOrEmpty(rightValue?.Symbol) || leftValue < 1) return null;
            return new MolecularFormula(rightValue, leftValue);
        }

        /// <summary>
        /// Gets the element count in a specific period.
        /// </summary>
        /// <param name="period">The period which is a one-based index of horizontal row in the periodic table.</param>
        /// <returns>The count in this period; or -1, if the period is out of range.</returns>
        public static int CountInPeriod(int period)
        {
            if (period < 1) return -1;
            var count = 2;
            var diff = 2;
            for (var i = 1; i < period; i++)
            {
                diff += 4;
                count += diff;
                i++;
                if (i < period) break;
                count += diff;
            }

            return count > 0 ? count : -1;
        }

        /// <summary>
        /// Gets the group.
        /// </summary>
        /// <param name="period">The period which is a one-based index of horizontal row in the periodic table.</param>
        /// <param name="index">Gets a zero-based index of the element in the period.</param>
        /// <returns>The count in this period; or -1, if not available.</returns>
        private static int GetGroup(int period, int index)
        {
            if (period < 1 || index < 0) return -1;
            if (index == 0) return 1;
            if (period == 1) return 18;
            if (index == 1) return 2;
            if (period < 4) return index + 11;
            if (period < 6) return index + 1;
            var i = 18 - CountInPeriod(period) + index + 1;
            return i < 3 ? 3 : i;
        }

        /// <summary>
        /// Gets the period which is a one-based index of horizontal row in the periodic table.
        /// </summary>
        /// <param name="number">The atomic number (or proton number, symbol Z) of the chemical element.</param>
        /// <returns>The period, and the index in this period.</returns>
        private static (int, int) GetPeriod(int number)
        {
            if (number < 1) return (-1, -1);
            if (number <= 2) return (1, number - 1);
            for (var i = 1; i < periodNumbers.Length; i++)
            {
                if (number > periodNumbers[i]) continue;
                var j = number - periodNumbers[i - 1] - 1;
                return (i + 1, j);
            }

            var diff = 42;
            var count = 342;
            var max = 2080;
            for (var i = 20; max > 0; i++)
            {
                if (number <= max) return (i, max - count + number);
                max += count;
                if (number <= max) return (i, max - count + number);
                count += diff;
                max += count;
            }

            return (-1, -1);
        }
    }
}