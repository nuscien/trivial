using System;
using System.Collections.Generic;
using System.Text;

namespace Trivial.Console
{
    public enum ParameterMode
    {
        First = 0,
        Last = 1,
        All = 2
    }

    public class ArgumentAttribute: Attribute
    {
        public string Name { get; set; }

        public bool Short { get; set; }

        public string SecondaryName { get; set; }

        public ParameterMode Mode { get; set; }
    }
}
