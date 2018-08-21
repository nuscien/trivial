using System;
using System.Collections.Generic;
using System.Text;

namespace Trivial.Console
{
    /// <summary>
    /// The parameter resolving modes.
    /// </summary>
    public enum ParameterModes
    {
        /// <summary>
        /// The first parameter.
        /// </summary>
        First = 0,

        /// <summary>
        /// The last parameter.
        /// </summary>
        Last = 1,

        /// <summary>
        /// Parameters merged.
        /// </summary>
        All = 2
    }

    /// <summary>
    /// The argument mapping attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ArgumentAttribute: Attribute
    {
        /// <summary>
        /// Initializes a new instance of the ArgumentAttribute class.
        /// </summary>
        public ArgumentAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ArgumentAttribute class.
        /// </summary>
        /// <param name="name">The attribute name.</param>
        /// <param name="canBeShort">true if it can be short in the first charactor; otherwise, false.</param>
        /// <param name="secondaryName">The optional secondary attribute name.</param>
        public ArgumentAttribute(string name, bool canBeShort = false, string secondaryName= null)
        {
            Name = name;
            Short = canBeShort;
            SecondaryName = secondaryName;
        }

        /// <summary>
        /// Gets or sets the attribute name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the attribute name can be short as its first charactor.
        /// </summary>
        public bool Short { get; set; }

        /// <summary>
        /// Gets or sets the optional secondary attribute name.
        /// </summary>
        public string SecondaryName { get; set; }

        /// <summary>
        /// Gets or sets the parameter resolving mode.
        /// </summary>
        public ParameterModes Mode { get; set; }
    }
}
