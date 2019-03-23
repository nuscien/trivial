using System;
using System.Collections.Generic;
using System.Text;

namespace Trivial.Security
{
    public abstract class AccessTokeRequest : AppAccessingKey
    {
        public abstract string GrantType { get; }
    }
}
