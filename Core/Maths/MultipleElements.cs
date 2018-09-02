// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultipleElements.cs" company="Nanchang Jinchen Software Co., Ltd.">
//   Copyright (c) 2010 Nanchang Jinchen Software Co., Ltd. All rights reserved.
// </copyright>
// <summary>
//   The mulitple elements object.
// </summary>
// <author>Kingcean Tuan</author>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Trivial.Maths
{
    /// <summary>
    /// A collection with two elements.
    /// </summary>
    /// <typeparam name="T">The type of elements.</typeparam>
    public class TwoElements<T>
    {
        /// <summary>
        /// Gets or sets the first value of the current multiple elements object.
        /// </summary>
        public T ItemA
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the second value of the current multiple elements object.
        /// </summary>
        public T ItemB
        {
            get;
            set;
        }

        /// <summary>
        /// Loads the multiple value from the specific source object.
        /// </summary>
        /// <param name="value">Another multiple value to copy into the current one.</param>
        public void Load(TwoElements<T> value)
        {
            if (value == null) return;
            ItemA = value.ItemA;
            ItemB = value.ItemB;
        }

        /// <summary>
        /// Returns a tuple that represents the values of current multiple elements object.
        /// </summary>
        /// <returns>The tuple representation of this multiple elements object.</returns>
        public Tuple<T, T> ToTuple()
        {
            return new Tuple<T, T>(ItemA, ItemB);
        }

        /// <summary>
        /// Returns a list that represents the values of current multiple elements object.
        /// </summary>
        /// <returns>The list representation of this multiple elements object.</returns>
        public IList<T> ToList()
        {
            return new List<T> { ItemA, ItemB };
        }
    }

    /// <summary>
    /// A collection with three elements.
    /// </summary>
    /// <typeparam name="T">The type of elements.</typeparam>
    public class ThreeElements<T>
    {
        /// <summary>
        /// Gets or sets the first value of the current multiple elements object.
        /// </summary>
        public T ItemA
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the second value of the current multiple elements object.
        /// </summary>
        public T ItemB
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the third value of the current multiple elements object.
        /// </summary>
        public T ItemC
        {
            get;
            set;
        }

        /// <summary>
        /// Loads the multiple value from the specific source object.
        /// </summary>
        /// <param name="value">Another multiple value to copy into the current one.</param>
        public void Load(TwoElements<T> value)
        {
            if (value == null) return;
            ItemA = value.ItemA;
            ItemB = value.ItemB;
        }

        /// <summary>
        /// Loads the multiple value from the specific source object.
        /// </summary>
        /// <param name="value">Another multiple value to copy into the current one.</param>
        public void Load(ThreeElements<T> value)
        {
            if (value == null) return;
            ItemA = value.ItemA;
            ItemB = value.ItemB;
            ItemC = value.ItemC;
        }

        /// <summary>
        /// Returns a tuple that represents the values of current multiple elements object.
        /// </summary>
        /// <returns>The tuple representation of this multiple elements object.</returns>
        public Tuple<T, T, T> ToTuple()
        {
            return new Tuple<T, T, T>(ItemA, ItemB, ItemC);
        }

        /// <summary>
        /// Returns a list that represents the values of current multiple elements object.
        /// </summary>
        /// <returns>The list representation of this multiple elements object.</returns>
        public IList<T> ToList()
        {
            return new List<T> { ItemA, ItemB, ItemC };
        }
    }

    /// <summary>
    /// A collection with four elements.
    /// </summary>
    /// <typeparam name="T">The type of elements.</typeparam>
    public class FourElements<T>
    {
        /// <summary>
        /// Gets or sets the first value of the current multiple elements object.
        /// </summary>
        public T ItemA
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the second value of the current multiple elements object.
        /// </summary>
        public T ItemB
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the third value of the current multiple elements object.
        /// </summary>
        public T ItemC
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the forth value of the current multiple elements object.
        /// </summary>
        public T ItemD
        {
            get;
            set;
        }

        /// <summary>
        /// Loads the multiple value from the specific source object.
        /// </summary>
        /// <param name="value">Another multiple value to copy into the current one.</param>
        public void Load(TwoElements<T> value)
        {
            if (value == null) return;
            ItemA = value.ItemA;
            ItemB = value.ItemB;
        }

        /// <summary>
        /// Loads the multiple value from the specific source object.
        /// </summary>
        /// <param name="value">Another multiple value to copy into the current one.</param>
        public void Load(ThreeElements<T> value)
        {
            if (value == null) return;
            ItemA = value.ItemA;
            ItemB = value.ItemB;
            ItemC = value.ItemC;
        }

        /// <summary>
        /// Loads the multiple value from the specific source object.
        /// </summary>
        /// <param name="value">Another multiple value to copy into the current one.</param>
        public void Load(FourElements<T> value)
        {
            if (value == null) return;
            ItemA = value.ItemA;
            ItemB = value.ItemB;
            ItemC = value.ItemC;
            ItemD = value.ItemD;
        }

        /// <summary>
        /// Returns a tuple that represents the values of current multiple elements object.
        /// </summary>
        /// <returns>The tuple representation of this multiple elements object.</returns>
        public Tuple<T, T, T, T> ToTuple()
        {
            return new Tuple<T, T, T, T>(ItemA, ItemB, ItemC, ItemD);
        }

        /// <summary>
        /// Returns a list that represents the values of current multiple elements object.
        /// </summary>
        /// <returns>The list representation of this multiple elements object.</returns>
        public IList<T> ToList()
        {
            return new List<T> { ItemA, ItemB, ItemC, ItemD };
        }
    }

    /// <summary>
    /// A collection with five elements.
    /// </summary>
    /// <typeparam name="T">The type of elements.</typeparam>
    public class FiveElements<T>
    {
        /// <summary>
        /// Gets or sets the first value of the current multiple elements object.
        /// </summary>
        public T ItemA
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the second value of the current multiple elements object.
        /// </summary>
        public T ItemB
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the third value of the current multiple elements object.
        /// </summary>
        public T ItemC
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the forth value of the current multiple elements object.
        /// </summary>
        public T ItemD
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the fifth value of the current multiple elements object.
        /// </summary>
        public T ItemE
        {
            get;
            set;
        }

        /// <summary>
        /// Loads the multiple value from the specific source object.
        /// </summary>
        /// <param name="value">Another multiple value to copy into the current one.</param>
        public void Load(TwoElements<T> value)
        {
            if (value == null) return;
            ItemA = value.ItemA;
            ItemB = value.ItemB;
        }

        /// <summary>
        /// Loads the multiple value from the specific source object.
        /// </summary>
        /// <param name="value">Another multiple value to copy into the current one.</param>
        public void Load(ThreeElements<T> value)
        {
            if (value == null) return;
            ItemA = value.ItemA;
            ItemB = value.ItemB;
            ItemC = value.ItemC;
        }

        /// <summary>
        /// Loads the multiple value from the specific source object.
        /// </summary>
        /// <param name="value">Another multiple value to copy into the current one.</param>
        public void Load(FourElements<T> value)
        {
            if (value == null) return;
            ItemA = value.ItemA;
            ItemB = value.ItemB;
            ItemC = value.ItemC;
            ItemD = value.ItemD;
        }

        /// <summary>
        /// Loads the multiple value from the specific source object.
        /// </summary>
        /// <param name="value">Another multiple value to copy into the current one.</param>
        public void Load(FiveElements<T> value)
        {
            if (value == null) return;
            ItemA = value.ItemA;
            ItemB = value.ItemB;
            ItemC = value.ItemC;
            ItemD = value.ItemD;
            ItemE = value.ItemE;
        }

        /// <summary>
        /// Returns a tuple that represents the values of current multiple elements object.
        /// </summary>
        /// <returns>The tuple representation of this multiple elements object.</returns>
        public Tuple<T, T, T, T, T> ToTuple()
        {
            return new Tuple<T, T, T, T, T>(ItemA, ItemB, ItemC, ItemD, ItemE);
        }

        /// <summary>
        /// Returns a list that represents the values of current multiple elements object.
        /// </summary>
        /// <returns>The list representation of this multiple elements object.</returns>
        public IList<T> ToList()
        {
            return new List<T> { ItemA, ItemB, ItemC, ItemD, ItemE };
        }
    }

    /// <summary>
    /// A collection with six elements.
    /// </summary>
    /// <typeparam name="T">The type of elements.</typeparam>
    public class SixElements<T>
    {
        /// <summary>
        /// Gets or sets the first value of the current multiple elements object.
        /// </summary>
        public T ItemA
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the second value of the current multiple elements object.
        /// </summary>
        public T ItemB
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the third value of the current multiple elements object.
        /// </summary>
        public T ItemC
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the forth value of the current multiple elements object.
        /// </summary>
        public T ItemD
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the fifth value of the current multiple elements object.
        /// </summary>
        public T ItemE
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the sixth value of the current multiple elements object.
        /// </summary>
        public T ItemF
        {
            get;
            set;
        }

        /// <summary>
        /// Loads the multiple value from the specific source object.
        /// </summary>
        /// <param name="value">Another multiple value to copy into the current one.</param>
        public void Load(TwoElements<T> value)
        {
            if (value == null) return;
            ItemA = value.ItemA;
            ItemB = value.ItemB;
        }

        /// <summary>
        /// Loads the multiple value from the specific source object.
        /// </summary>
        /// <param name="value">Another multiple value to copy into the current one.</param>
        public void Load(ThreeElements<T> value)
        {
            if (value == null) return;
            ItemA = value.ItemA;
            ItemB = value.ItemB;
            ItemC = value.ItemC;
        }

        /// <summary>
        /// Loads the multiple value from the specific source object.
        /// </summary>
        /// <param name="value">Another multiple value to copy into the current one.</param>
        public void Load(FourElements<T> value)
        {
            if (value == null) return;
            ItemA = value.ItemA;
            ItemB = value.ItemB;
            ItemC = value.ItemC;
            ItemD = value.ItemD;
        }

        /// <summary>
        /// Loads the multiple value from the specific source object.
        /// </summary>
        /// <param name="value">Another multiple value to copy into the current one.</param>
        public void Load(FiveElements<T> value)
        {
            if (value == null) return;
            ItemA = value.ItemA;
            ItemB = value.ItemB;
            ItemC = value.ItemC;
            ItemD = value.ItemD;
            ItemE = value.ItemE;
        }

        /// <summary>
        /// Loads the multiple value from the specific source object.
        /// </summary>
        /// <param name="value">Another multiple value to copy into the current one.</param>
        public void Load(SixElements<T> value)
        {
            if (value == null) return;
            ItemA = value.ItemA;
            ItemB = value.ItemB;
            ItemC = value.ItemC;
            ItemD = value.ItemD;
            ItemE = value.ItemE;
            ItemF = value.ItemF;
        }

        /// <summary>
        /// Returns a tuple that represents the values of current multiple elements object.
        /// </summary>
        /// <returns>The tuple representation of this multiple elements object.</returns>
        public Tuple<T, T, T, T, T, T> ToTuple()
        {
            return new Tuple<T, T, T, T, T, T>(ItemA, ItemB, ItemC, ItemD, ItemE, ItemF);
        }

        /// <summary>
        /// Returns a list that represents the values of current multiple elements object.
        /// </summary>
        /// <returns>The list representation of this multiple elements object.</returns>
        public IList<T> ToList()
        {
            return new List<T> { ItemA, ItemB, ItemC, ItemD, ItemE, ItemF };
        }
    }

    /// <summary>
    /// A collection with seven elements.
    /// </summary>
    /// <typeparam name="T">The type of elements.</typeparam>
    public class SevenElements<T>
    {
        /// <summary>
        /// Gets or sets the first value of the current multiple elements object.
        /// </summary>
        public T ItemA
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the second value of the current multiple elements object.
        /// </summary>
        public T ItemB
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the third value of the current multiple elements object.
        /// </summary>
        public T ItemC
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the forth value of the current multiple elements object.
        /// </summary>
        public T ItemD
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the fifth value of the current multiple elements object.
        /// </summary>
        public T ItemE
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the sixth value of the current multiple elements object.
        /// </summary>
        public T ItemF
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the seventh value of the current multiple elements object.
        /// </summary>
        public T ItemG
        {
            get;
            set;
        }

        /// <summary>
        /// Loads the multiple value from the specific source object.
        /// </summary>
        /// <param name="value">Another multiple value to copy into the current one.</param>
        public void Load(TwoElements<T> value)
        {
            if (value == null) return;
            ItemA = value.ItemA;
            ItemB = value.ItemB;
        }

        /// <summary>
        /// Loads the multiple value from the specific source object.
        /// </summary>
        /// <param name="value">Another multiple value to copy into the current one.</param>
        public void Load(ThreeElements<T> value)
        {
            if (value == null) return;
            ItemA = value.ItemA;
            ItemB = value.ItemB;
            ItemC = value.ItemC;
        }

        /// <summary>
        /// Loads the multiple value from the specific source object.
        /// </summary>
        /// <param name="value">Another multiple value to copy into the current one.</param>
        public void Load(FourElements<T> value)
        {
            if (value == null) return;
            ItemA = value.ItemA;
            ItemB = value.ItemB;
            ItemC = value.ItemC;
            ItemD = value.ItemD;
        }

        /// <summary>
        /// Loads the multiple value from the specific source object.
        /// </summary>
        /// <param name="value">Another multiple value to copy into the current one.</param>
        public void Load(FiveElements<T> value)
        {
            if (value == null) return;
            ItemA = value.ItemA;
            ItemB = value.ItemB;
            ItemC = value.ItemC;
            ItemD = value.ItemD;
            ItemE = value.ItemE;
        }

        /// <summary>
        /// Loads the multiple value from the specific source object.
        /// </summary>
        /// <param name="value">Another multiple value to copy into the current one.</param>
        public void Load(SixElements<T> value)
        {
            if (value == null) return;
            ItemA = value.ItemA;
            ItemB = value.ItemB;
            ItemC = value.ItemC;
            ItemD = value.ItemD;
            ItemE = value.ItemE;
            ItemF = value.ItemF;
        }

        /// <summary>
        /// Loads the multiple value from the specific source object.
        /// </summary>
        /// <param name="value">Another multiple value to copy into the current one.</param>
        public void Load(SevenElements<T> value)
        {
            if (value == null) return;
            ItemA = value.ItemA;
            ItemB = value.ItemB;
            ItemC = value.ItemC;
            ItemD = value.ItemD;
            ItemE = value.ItemE;
            ItemF = value.ItemF;
            ItemG = value.ItemG;
        }

        /// <summary>
        /// Returns a tuple that represents the values of current multiple elements object.
        /// </summary>
        /// <returns>The tuple representation of this multiple elements object.</returns>
        public Tuple<T, T, T, T, T, T, T> ToTuple()
        {
            return new Tuple<T, T, T, T, T, T, T>(ItemA, ItemB, ItemC, ItemD, ItemE, ItemF, ItemG);
        }

        /// <summary>
        /// Returns a list that represents the values of current multiple elements object.
        /// </summary>
        /// <returns>The list representation of this multiple elements object.</returns>
        public IList<T> ToList()
        {
            return new List<T> { ItemA, ItemB, ItemC, ItemD, ItemE, ItemF, ItemG };
        }
    }

    /// <summary>
    /// A collection with eight elements.
    /// </summary>
    /// <typeparam name="T">The type of elements.</typeparam>
    public class EightElements<T>
    {
        /// <summary>
        /// Gets or sets the first value of the current multiple elements object.
        /// </summary>
        public T ItemA
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the second value of the current multiple elements object.
        /// </summary>
        public T ItemB
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the third value of the current multiple elements object.
        /// </summary>
        public T ItemC
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the forth value of the current multiple elements object.
        /// </summary>
        public T ItemD
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the fifth value of the current multiple elements object.
        /// </summary>
        public T ItemE
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the sixth value of the current multiple elements object.
        /// </summary>
        public T ItemF
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the seventh value of the current multiple elements object.
        /// </summary>
        public T ItemG
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the eighth value of the current multiple elements object.
        /// </summary>
        public T ItemH
        {
            get;
            set;
        }

        /// <summary>
        /// Loads the multiple value from the specific source object.
        /// </summary>
        /// <param name="value">Another multiple value to copy into the current one.</param>
        public void Load(TwoElements<T> value)
        {
            if (value == null) return;
            ItemA = value.ItemA;
            ItemB = value.ItemB;
        }

        /// <summary>
        /// Loads the multiple value from the specific source object.
        /// </summary>
        /// <param name="value">Another multiple value to copy into the current one.</param>
        public void Load(ThreeElements<T> value)
        {
            if (value == null) return;
            ItemA = value.ItemA;
            ItemB = value.ItemB;
            ItemC = value.ItemC;
        }

        /// <summary>
        /// Loads the multiple value from the specific source object.
        /// </summary>
        /// <param name="value">Another multiple value to copy into the current one.</param>
        public void Load(FourElements<T> value)
        {
            if (value == null) return;
            ItemA = value.ItemA;
            ItemB = value.ItemB;
            ItemC = value.ItemC;
            ItemD = value.ItemD;
        }

        /// <summary>
        /// Loads the multiple value from the specific source object.
        /// </summary>
        /// <param name="value">Another multiple value to copy into the current one.</param>
        public void Load(FiveElements<T> value)
        {
            if (value == null) return;
            ItemA = value.ItemA;
            ItemB = value.ItemB;
            ItemC = value.ItemC;
            ItemD = value.ItemD;
            ItemE = value.ItemE;
        }

        /// <summary>
        /// Loads the multiple value from the specific source object.
        /// </summary>
        /// <param name="value">Another multiple value to copy into the current one.</param>
        public void Load(SixElements<T> value)
        {
            if (value == null) return;
            ItemA = value.ItemA;
            ItemB = value.ItemB;
            ItemC = value.ItemC;
            ItemD = value.ItemD;
            ItemE = value.ItemE;
            ItemF = value.ItemF;
        }

        /// <summary>
        /// Loads the multiple value from the specific source object.
        /// </summary>
        /// <param name="value">Another multiple value to copy into the current one.</param>
        public void Load(SevenElements<T> value)
        {
            if (value == null) return;
            ItemA = value.ItemA;
            ItemB = value.ItemB;
            ItemC = value.ItemC;
            ItemD = value.ItemD;
            ItemE = value.ItemE;
            ItemF = value.ItemF;
            ItemG = value.ItemG;
        }

        /// <summary>
        /// Loads the multiple value from the specific source object.
        /// </summary>
        /// <param name="value">Another multiple value to copy into the current one.</param>
        public void Load(EightElements<T> value)
        {
            if (value == null) return;
            ItemA = value.ItemA;
            ItemB = value.ItemB;
            ItemC = value.ItemC;
            ItemD = value.ItemD;
            ItemE = value.ItemE;
            ItemF = value.ItemF;
            ItemG = value.ItemG;
            ItemH = value.ItemH;
        }

        /// <summary>
        /// Returns a tuple that represents the values of current multiple elements object.
        /// </summary>
        /// <returns>The tuple representation of this multiple elements object.</returns>
        public Tuple<T, T, T, T, T, T, T, Tuple<T>> ToTuple()
        {
            return new Tuple<T, T, T, T, T, T, T, Tuple<T>>(ItemA, ItemB, ItemC, ItemD, ItemE, ItemF, ItemG, new Tuple<T>(ItemH));
        }

        /// <summary>
        /// Returns a list that represents the values of current multiple elements object.
        /// </summary>
        /// <returns>The list representation of this multiple elements object.</returns>
        public IList<T> ToList()
        {
            return new List<T> { ItemA, ItemB, ItemC, ItemD, ItemE, ItemF, ItemG, ItemH };
        }
    }
}
