using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.Chemistry
{
    /// <summary>
    /// The model of isotope.
    /// </summary>
    public class Isotope : IEquatable<Isotope>
    {
        /// <summary>
        /// Initializes a new instance of the Isotope class.
        /// </summary>
        /// <param name="element">The chemical element.</param>
        /// <param name="atomicMassNumber">The atomic mass number (total protons and neutrons).</param>
        public Isotope(ChemicalElement element, int atomicMassNumber)
        {
            Element = element;
            if (element is null || element.AtomicNumber < 1)
            {
                AtomicMassNumber = atomicMassNumber < 0 ? -1 : atomicMassNumber;
                return;
            }

            AtomicMassNumber = atomicMassNumber < element.AtomicNumber
                ? element.AtomicNumber
                : atomicMassNumber;
        }

        /// <summary>
        /// Initializes a new instance of the Isotope class.
        /// </summary>
        /// <param name="atomicNumber">The atomic number (or proton number, symbol Z) of the chemical element. The number is one-based.</param>
        /// <param name="atomicMassNumber">The atomic mass number (total protons and neutrons).</param>
        public Isotope(int atomicNumber, int atomicMassNumber)
            : this(ChemicalElement.Get(atomicNumber), atomicMassNumber)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Isotope class.
        /// </summary>
        /// <param name="element">The chemical element.</param>
        /// <param name="atomicMassNumber">The atomic mass number (total protons and neutrons).</param>
        /// <param name="atomicWeight">The atomic weight in dalton (unified atomic mass unit).</param>
        public Isotope(ChemicalElement element, int atomicMassNumber, double atomicWeight)
            : this(element, atomicMassNumber)
        {
            AtomicWeight = atomicWeight;
        }

        /// <summary>
        /// Initializes a new instance of the Isotope class.
        /// </summary>
        /// <param name="atomicNumber">The atomic number (or proton number, symbol Z) of the chemical element. The number is one-based.</param>
        /// <param name="atomicMassNumber">The atomic mass number (total protons and neutrons).</param>
        /// <param name="atomicWeight">The atomic weight in dalton (unified atomic mass unit).</param>
        public Isotope(int atomicNumber, int atomicMassNumber, double atomicWeight)
            : this(ChemicalElement.Get(atomicNumber), atomicMassNumber)
        {
            AtomicWeight = atomicWeight;
        }

        /// <summary>
        /// Gets the chemical element.
        /// </summary>
        public ChemicalElement Element { get; }

        /// <summary>
        /// Gets the element symbol.
        /// </summary>
        public string ElementSymbol => Element?.Symbol;

        /// <summary>
        /// Gets the atomic number (or proton number, symbol Z) of the chemical element.
        /// It is the one-based number of protons found in the nucleus of every atom of that element.
        /// </summary>
        public int AtomicNumber => Element?.AtomicNumber ?? -1;

        /// <summary>
        /// Gets the atomic mass number (total protons and neutrons).
        /// </summary>
        public int AtomicMassNumber { get; }

        /// <summary>
        /// Gets the atomic weight in dalton (unified atomic mass unit).
        /// </summary>
        public double AtomicWeight { get; }

        /// <summary>
        /// Gets a value indicating whether has atomic weight information.
        /// </summary>
        public bool HasAtomicWeight => !double.IsNaN(AtomicWeight);

        /// <summary>
        /// Gets the numbers of neutron.
        /// </summary>
        public int Neutrons
        {
            get
            {
                if (AtomicMassNumber < 1)
                    return AtomicMassNumber == 0 ? 0 : -1;
                if (AtomicNumber > 0)
                    return AtomicMassNumber - AtomicNumber;
                return AtomicMassNumber;
            }
        }

        /// <summary>
        /// Peturns a string that represents the current chemical element information.
        /// </summary>
        /// <returns>A string that represents the current chemical element information.</returns>
        public override string ToString()
        {
            if (Element is null) return "?";
            if (AtomicMassNumber < 1) return Element.Symbol;
            if (Element.AtomicNumber == 1 && Element.Symbol == "H")
            {
                switch (AtomicMassNumber)
                {
                    case 1:
                        return "H";
                    case 2:
                        return "D";
                    case 3:
                        return "T";
                }
            }

            return AtomicMassNumber
                .ToString("g")
                .Replace('0', '⁰')
                .Replace('1', '¹')
                .Replace('2', '²')
                .Replace('3', '³')
                .Replace('4', '⁴')
                .Replace('5', '⁵')
                .Replace('6', '⁶')
                .Replace('7', '⁷')
                .Replace('8', '⁸')
                .Replace('9', '⁹')
                + Element.Symbol;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="other">The object to compare with the current instance.</param>
        /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
        public bool Equals(Isotope other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Element == other.Element
                && AtomicMassNumber == other.AtomicMassNumber;
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="other">The object to compare with the current instance.</param>
        /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
        public override bool Equals(object other)
        {
            if (other is null) return false;
            if (other is Isotope isotope) return Equals(isotope);
            if (other is ChemicalElement element) return element.Equals(Element);
            return false;
        }

        /// <summary>
        /// Compares two isotopes to indicate if they are same.
        /// leftValue == rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if they are same; otherwise, false.</returns>
        public static bool operator ==(Isotope leftValue, Isotope rightValue)
        {
            if (ReferenceEquals(leftValue, rightValue)) return true;
            if (leftValue is null || rightValue is null) return false;
            return leftValue.Equals(rightValue);
        }

        /// <summary>
        /// Compares two isotopes to indicate if they are different.
        /// leftValue != rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>true if they are different; otherwise, false.</returns>
        public static bool operator !=(Isotope leftValue, Isotope rightValue)
        {
            if (ReferenceEquals(leftValue, rightValue)) return false;
            if (leftValue is null || rightValue is null) return true;
            return !leftValue.Equals(rightValue);
        }
    }
}
