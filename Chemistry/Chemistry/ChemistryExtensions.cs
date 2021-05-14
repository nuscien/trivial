using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.Chemistry
{
    /// <summary>
    /// The helpers and extensions of chemistry.
    /// </summary>
    public static class ChemistryExtensions
    {
        /// <summary>
        /// Adds molecular formula into a list.
        /// </summary>
        /// <param name="formulas">The molecular fomula collection.</param>
        /// <param name="value">A molecular formula instance to add.</param>
        /// <param name="count">The count of the value.</param>
        /// <param name="another">Another optional molecular formula instance to add.</param>
        /// <param name="countForAnother">The count of another.</param>
        /// <param name="other">The optional third molecular formula instance to add</param>
        /// <param name="countForLast">The count of the third molecular formula instance to add.</param>
        public static void Add(this IList<MolecularFormula> formulas, MolecularFormula value, int count = 1, MolecularFormula another = null, int countForAnother = 1, MolecularFormula other = null, int countForLast = 1)
        {
            if (formulas is null) return;
            if (value != null)
            {
                for (var i = 0; i < count; i++)
                {
                    formulas.Add(value);
                }
            }

            if (another != null)
            {
                for (var i = 0; i < countForAnother; i++)
                {
                    formulas.Add(another);
                }
            }

            if (other != null)
            {
                for (var i = 0; i < countForLast; i++)
                {
                    formulas.Add(other);
                }
            }
        }
    }
}
