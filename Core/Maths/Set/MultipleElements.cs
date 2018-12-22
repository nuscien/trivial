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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Trivial.Maths
{
    /// <summary>
    /// A collection with multiple elements.
    /// </summary>
    /// <typeparam name="T">The type of elements.</typeparam>
    public abstract class BaseMultipleElements<T> : IReadOnlyList<T>, IEquatable<BaseMultipleElements<T>>, IEquatable<IEnumerable<T>>, ICloneable
    {
        /// <summary>
        /// Gets the number of elements in the collection.
        /// </summary>
        public virtual int Count => ToList().Count;

        /// <summary>
        /// Gets the element at the specified index in the instance.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The element at the specified index in the instance.</returns>
        public T this[int index] => ToList()[index];

        /// <summary>
        /// Resets all the items.
        /// </summary>
        /// <param name="value">The value to set for all the items.</param>
        public abstract void Reset(T value = default);

        /// <summary>
        /// Returns a list that represents the values of current multiple elements object.
        /// </summary>
        /// <returns>The list representation of this multiple elements object.</returns>
        public abstract IList<T> ToList();

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public bool Equals(BaseMultipleElements<T> other)
        {
            if (other == null) return false;
            var a = ToList();
            var b = other.ToList();
            if (a.Count != b.Count) return false;
            for (var i = 0; i < a.Count; i++)
            {
                if (a[i] == null && b[i] != null) return false;
                if (!a[i].Equals(b)) return false;
            }

            return true;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public bool Equals(IEnumerable<T> other)
        {
            if (other == null) return false;
            var a = ToList();
            var b = other.ToList();
            if (a.Count != b.Count) return false;
            for (var i = 0; i < a.Count; i++)
            {
                if (a[i] == null && b[i] != null) return false;
                if (!a[i].Equals(b)) return false;
            }

            return true;
        }

        /// <summary>
        /// Compares two multiple elements to indicate if they are same.
        /// leftValue == rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>A result after subtration.</returns>
        public static bool operator ==(BaseMultipleElements<T> leftValue, BaseMultipleElements<T> rightValue)
        {
            if (leftValue == null && rightValue == null) return true;
            if (leftValue == null || rightValue == null) return false;
            return leftValue.Equals(rightValue);
        }

        /// <summary>
        /// Compares two multiple elements to indicate if they are different.
        /// leftValue != rightValue
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>A result after subtration.</returns>
        public static bool operator !=(BaseMultipleElements<T> leftValue, BaseMultipleElements<T> rightValue)
        {
            if (leftValue == null && rightValue == null) return false;
            if (leftValue == null || rightValue == null) return true;
            return !leftValue.Equals(rightValue);
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public abstract object Clone();

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return ToList().GetHashCode();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return ToList().GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return (ToList() as IEnumerable).GetEnumerator();
        }
    }

    /// <summary>
    /// A collection with single element.
    /// </summary>
    /// <typeparam name="T">The type of elements.</typeparam>
    public class SingleElement<T> : BaseMultipleElements<T>
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
        /// Gets the number of elements in the collection.
        /// </summary>
        public override int Count => 1;

        /// <summary>
        /// Loads the multiple value from the specific source object.
        /// </summary>
        /// <param name="value">Another multiple value to copy into the current one.</param>
        public void Load(SingleElement<T> value)
        {
            if (value == null) return;
            ItemA = value.ItemA;
        }

        /// <summary>
        /// Resets all the items.
        /// </summary>
        /// <param name="value">The value to set for all the items.</param>
        public override void Reset(T value = default)
        {
            ItemA = value;
        }

        /// <summary>
        /// Returns a tuple that represents the values of current multiple elements object.
        /// </summary>
        /// <returns>The tuple representation of this multiple elements object.</returns>
        public Tuple<T> ToTuple()
        {
            return new Tuple<T>(ItemA);
        }

        /// <summary>
        /// Returns a list that represents the values of current multiple elements object.
        /// </summary>
        /// <returns>The list representation of this multiple elements object.</returns>
        public override IList<T> ToList()
        {
            return new List<T> { ItemA };
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public override object Clone()
        {
            return new SingleElement<T>
            {
                ItemA = ItemA
            };
        }
    }

    /// <summary>
    /// A collection with two elements.
    /// </summary>
    /// <typeparam name="T">The type of elements.</typeparam>
    public class TwoElements<T> : SingleElement<T>
    {
        /// <summary>
        /// Gets or sets the second value of the current multiple elements object.
        /// </summary>
        public T ItemB
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the number of elements in the collection.
        /// </summary>
        public override int Count => 2;

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
        /// Resets all the items.
        /// </summary>
        /// <param name="value">The value to set for all the items.</param>
        public override void Reset(T value = default)
        {
            base.Reset(value);
            ItemB = value;
        }

        /// <summary>
        /// Returns a tuple that represents the values of current multiple elements object.
        /// </summary>
        /// <returns>The tuple representation of this multiple elements object.</returns>
        public new Tuple<T, T> ToTuple()
        {
            return new Tuple<T, T>(ItemA, ItemB);
        }

        /// <summary>
        /// Returns a list that represents the values of current multiple elements object.
        /// </summary>
        /// <returns>The list representation of this multiple elements object.</returns>
        public override IList<T> ToList()
        {
            return new List<T> { ItemA, ItemB };
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public override object Clone()
        {
            return new TwoElements<T>
            {
                ItemA = ItemA,
                ItemB = ItemB
            };
        }
    }

    /// <summary>
    /// A collection with three elements.
    /// </summary>
    /// <typeparam name="T">The type of elements.</typeparam>
    public class ThreeElements<T> : TwoElements<T>
    {
        /// <summary>
        /// Gets or sets the third value of the current multiple elements object.
        /// </summary>
        public T ItemC
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the number of elements in the collection.
        /// </summary>
        public override int Count => 3;

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
        /// Resets all the items.
        /// </summary>
        /// <param name="value">The value to set for all the items.</param>
        public override void Reset(T value = default)
        {
            base.Reset(value);
            ItemC = value;
        }

        /// <summary>
        /// Returns a tuple that represents the values of current multiple elements object.
        /// </summary>
        /// <returns>The tuple representation of this multiple elements object.</returns>
        public new Tuple<T, T, T> ToTuple()
        {
            return new Tuple<T, T, T>(ItemA, ItemB, ItemC);
        }

        /// <summary>
        /// Returns a list that represents the values of current multiple elements object.
        /// </summary>
        /// <returns>The list representation of this multiple elements object.</returns>
        public override IList<T> ToList()
        {
            return new List<T> { ItemA, ItemB, ItemC };
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public override object Clone()
        {
            return new ThreeElements<T>
            {
                ItemA = ItemA,
                ItemB = ItemB,
                ItemC = ItemC
            };
        }
    }

    /// <summary>
    /// A collection with four elements.
    /// </summary>
    /// <typeparam name="T">The type of elements.</typeparam>
    public class FourElements<T> : ThreeElements<T>
    {
        /// <summary>
        /// Gets or sets the forth value of the current multiple elements object.
        /// </summary>
        public T ItemD
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the number of elements in the collection.
        /// </summary>
        public override int Count => 4;

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
        /// Resets all the items.
        /// </summary>
        /// <param name="value">The value to set for all the items.</param>
        public override void Reset(T value = default)
        {
            base.Reset(value);
            ItemD = value;
        }

        /// <summary>
        /// Returns a tuple that represents the values of current multiple elements object.
        /// </summary>
        /// <returns>The tuple representation of this multiple elements object.</returns>
        public new Tuple<T, T, T, T> ToTuple()
        {
            return new Tuple<T, T, T, T>(ItemA, ItemB, ItemC, ItemD);
        }

        /// <summary>
        /// Returns a list that represents the values of current multiple elements object.
        /// </summary>
        /// <returns>The list representation of this multiple elements object.</returns>
        public override IList<T> ToList()
        {
            return new List<T> { ItemA, ItemB, ItemC, ItemD };
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public override object Clone()
        {
            return new FourElements<T>
            {
                ItemA = ItemA,
                ItemB = ItemB,
                ItemC = ItemC,
                ItemD = ItemD
            };
        }
    }

    /// <summary>
    /// A collection with five elements.
    /// </summary>
    /// <typeparam name="T">The type of elements.</typeparam>
    public class FiveElements<T> : FourElements<T>
    {

        /// <summary>
        /// Gets or sets the fifth value of the current multiple elements object.
        /// </summary>
        public T ItemE
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the number of elements in the collection.
        /// </summary>
        public override int Count => 5;

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
        /// Resets all the items.
        /// </summary>
        /// <param name="value">The value to set for all the items.</param>
        public override void Reset(T value = default)
        {
            base.Reset(value);
            ItemE = value;
        }

        /// <summary>
        /// Returns a tuple that represents the values of current multiple elements object.
        /// </summary>
        /// <returns>The tuple representation of this multiple elements object.</returns>
        public new Tuple<T, T, T, T, T> ToTuple()
        {
            return new Tuple<T, T, T, T, T>(ItemA, ItemB, ItemC, ItemD, ItemE);
        }

        /// <summary>
        /// Returns a list that represents the values of current multiple elements object.
        /// </summary>
        /// <returns>The list representation of this multiple elements object.</returns>
        public override IList<T> ToList()
        {
            return new List<T> { ItemA, ItemB, ItemC, ItemD, ItemE };
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public override object Clone()
        {
            return new FiveElements<T>
            {
                ItemA = ItemA,
                ItemB = ItemB,
                ItemC = ItemC,
                ItemD = ItemD,
                ItemE = ItemE
            };
        }
    }

    /// <summary>
    /// A collection with six elements.
    /// </summary>
    /// <typeparam name="T">The type of elements.</typeparam>
    public class SixElements<T> : FiveElements<T>
    {
        /// <summary>
        /// Gets or sets the sixth value of the current multiple elements object.
        /// </summary>
        public T ItemF
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the number of elements in the collection.
        /// </summary>
        public override int Count => 6;

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
        /// Resets all the items.
        /// </summary>
        /// <param name="value">The value to set for all the items.</param>
        public override void Reset(T value = default)
        {
            base.Reset(value);
            ItemF = value;
        }

        /// <summary>
        /// Returns a tuple that represents the values of current multiple elements object.
        /// </summary>
        /// <returns>The tuple representation of this multiple elements object.</returns>
        public new Tuple<T, T, T, T, T, T> ToTuple()
        {
            return new Tuple<T, T, T, T, T, T>(ItemA, ItemB, ItemC, ItemD, ItemE, ItemF);
        }

        /// <summary>
        /// Returns a list that represents the values of current multiple elements object.
        /// </summary>
        /// <returns>The list representation of this multiple elements object.</returns>
        public override IList<T> ToList()
        {
            return new List<T> { ItemA, ItemB, ItemC, ItemD, ItemE, ItemF };
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public override object Clone()
        {
            return new SixElements<T>
            {
                ItemA = ItemA,
                ItemB = ItemB,
                ItemC = ItemC,
                ItemD = ItemD,
                ItemE = ItemE,
                ItemF = ItemF
            };
        }
    }

    /// <summary>
    /// A collection with seven elements.
    /// </summary>
    /// <typeparam name="T">The type of elements.</typeparam>
    public class SevenElements<T> : SixElements<T>
    {
        /// <summary>
        /// Gets or sets the seventh value of the current multiple elements object.
        /// </summary>
        public T ItemG
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the number of elements in the collection.
        /// </summary>
        public override int Count => 7;

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
        /// Resets all the items.
        /// </summary>
        /// <param name="value">The value to set for all the items.</param>
        public override void Reset(T value = default)
        {
            base.Reset(value);
            ItemG = value;
        }

        /// <summary>
        /// Returns a tuple that represents the values of current multiple elements object.
        /// </summary>
        /// <returns>The tuple representation of this multiple elements object.</returns>
        public new Tuple<T, T, T, T, T, T, T> ToTuple()
        {
            return new Tuple<T, T, T, T, T, T, T>(ItemA, ItemB, ItemC, ItemD, ItemE, ItemF, ItemG);
        }

        /// <summary>
        /// Returns a list that represents the values of current multiple elements object.
        /// </summary>
        /// <returns>The list representation of this multiple elements object.</returns>
        public override IList<T> ToList()
        {
            return new List<T> { ItemA, ItemB, ItemC, ItemD, ItemE, ItemF, ItemG };
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public override object Clone()
        {
            return new SevenElements<T>
            {
                ItemA = ItemA,
                ItemB = ItemB,
                ItemC = ItemC,
                ItemD = ItemD,
                ItemE = ItemE,
                ItemF = ItemF,
                ItemG = ItemG
            };
        }
    }

    /// <summary>
    /// A collection with eight elements.
    /// </summary>
    /// <typeparam name="T">The type of elements.</typeparam>
    public class EightElements<T> : SevenElements<T>
    {
        /// <summary>
        /// Gets or sets the eighth value of the current multiple elements object.
        /// </summary>
        public T ItemH
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the number of elements in the collection.
        /// </summary>
        public override int Count => 8;

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
        /// Resets all the items.
        /// </summary>
        /// <param name="value">The value to set for all the items.</param>
        public override void Reset(T value = default)
        {
            base.Reset(value);
            ItemH = value;
        }

        /// <summary>
        /// Returns a tuple that represents the values of current multiple elements object.
        /// </summary>
        /// <returns>The tuple representation of this multiple elements object.</returns>
        public new Tuple<T, T, T, T, T, T, T, T> ToTuple()
        {
            return new Tuple<T, T, T, T, T, T, T, T>(ItemA, ItemB, ItemC, ItemD, ItemE, ItemF, ItemG, ItemH);
        }

        /// <summary>
        /// Returns a list that represents the values of current multiple elements object.
        /// </summary>
        /// <returns>The list representation of this multiple elements object.</returns>
        public override IList<T> ToList()
        {
            return new List<T> { ItemA, ItemB, ItemC, ItemD, ItemE, ItemF, ItemG, ItemH };
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public override object Clone()
        {
            return new EightElements<T>
            {
                ItemA = ItemA,
                ItemB = ItemB,
                ItemC = ItemC,
                ItemD = ItemD,
                ItemE = ItemE,
                ItemF = ItemF,
                ItemG = ItemG,
                ItemH = ItemH
            };
        }
    }

    /// <summary>
    /// A collection with eight elements.
    /// </summary>
    /// <typeparam name="T">The type of elements.</typeparam>
    public class NineElements<T> : EightElements<T>
    {
        /// <summary>
        /// Gets or sets the eighth value of the current multiple elements object.
        /// </summary>
        public T ItemI
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the number of elements in the collection.
        /// </summary>
        public override int Count => 9;

        /// <summary>
        /// Loads the multiple value from the specific source object.
        /// </summary>
        /// <param name="value">Another multiple value to copy into the current one.</param>
        public void Load(NineElements<T> value)
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
            ItemI = value.ItemI;
        }

        /// <summary>
        /// Resets all the items.
        /// </summary>
        /// <param name="value">The value to set for all the items.</param>
        public override void Reset(T value = default)
        {
            base.Reset(value);
            ItemI = value;
        }

        /// <summary>
        /// Returns a tuple that represents the values of current multiple elements object.
        /// </summary>
        /// <returns>The tuple representation of this multiple elements object.</returns>
        public new Tuple<T, T, T, T, T, T, T, Tuple<T, T>> ToTuple()
        {
            return new Tuple<T, T, T, T, T, T, T, Tuple<T, T>>(ItemA, ItemB, ItemC, ItemD, ItemE, ItemF, ItemG, new Tuple<T, T>(ItemH, ItemI));
        }

        /// <summary>
        /// Returns a list that represents the values of current multiple elements object.
        /// </summary>
        /// <returns>The list representation of this multiple elements object.</returns>
        public override IList<T> ToList()
        {
            return new List<T> { ItemA, ItemB, ItemC, ItemD, ItemE, ItemF, ItemG, ItemH, ItemI };
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public override object Clone()
        {
            return new NineElements<T>
            {
                ItemA = ItemA,
                ItemB = ItemB,
                ItemC = ItemC,
                ItemD = ItemD,
                ItemE = ItemE,
                ItemF = ItemF,
                ItemG = ItemG,
                ItemH = ItemH,
                ItemI = ItemI
            };
        }
    }

    /// <summary>
    /// A collection with eight elements.
    /// </summary>
    /// <typeparam name="T">The type of elements.</typeparam>
    public class TenElements<T> : NineElements<T>
    {
        /// <summary>
        /// Gets or sets the eighth value of the current multiple elements object.
        /// </summary>
        public T ItemJ
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the number of elements in the collection.
        /// </summary>
        public override int Count => 10;

        /// <summary>
        /// Loads the multiple value from the specific source object.
        /// </summary>
        /// <param name="value">Another multiple value to copy into the current one.</param>
        public void Load(TenElements<T> value)
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
            ItemI = value.ItemI;
            ItemJ = value.ItemJ;
        }

        /// <summary>
        /// Resets all the items.
        /// </summary>
        /// <param name="value">The value to set for all the items.</param>
        public override void Reset(T value = default)
        {
            base.Reset(value);
            ItemJ = value;
        }

        /// <summary>
        /// Returns a tuple that represents the values of current multiple elements object.
        /// </summary>
        /// <returns>The tuple representation of this multiple elements object.</returns>
        public new Tuple<T, T, T, T, T, T, T, Tuple<T, T, T>> ToTuple()
        {
            return new Tuple<T, T, T, T, T, T, T, Tuple<T, T, T>>(ItemA, ItemB, ItemC, ItemD, ItemE, ItemF, ItemG, new Tuple<T, T, T>(ItemH, ItemI, ItemJ));
        }

        /// <summary>
        /// Returns a list that represents the values of current multiple elements object.
        /// </summary>
        /// <returns>The list representation of this multiple elements object.</returns>
        public override IList<T> ToList()
        {
            return new List<T> { ItemA, ItemB, ItemC, ItemD, ItemE, ItemF, ItemG, ItemH, ItemI, ItemJ };
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public override object Clone()
        {
            return new TenElements<T>
            {
                ItemA = ItemA,
                ItemB = ItemB,
                ItemC = ItemC,
                ItemD = ItemD,
                ItemE = ItemE,
                ItemF = ItemF,
                ItemG = ItemG,
                ItemH = ItemH,
                ItemI = ItemI,
                ItemJ = ItemJ
            };
        }
    }

    /// <summary>
    /// A collection with eight elements.
    /// </summary>
    /// <typeparam name="T">The type of elements.</typeparam>
    public class ElevenElements<T> : TenElements<T>
    {
        /// <summary>
        /// Gets or sets the eighth value of the current multiple elements object.
        /// </summary>
        public T ItemK
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the number of elements in the collection.
        /// </summary>
        public override int Count => 11;

        /// <summary>
        /// Loads the multiple value from the specific source object.
        /// </summary>
        /// <param name="value">Another multiple value to copy into the current one.</param>
        public void Load(ElevenElements<T> value)
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
            ItemI = value.ItemI;
            ItemJ = value.ItemJ;
            ItemK = value.ItemK;
        }

        /// <summary>
        /// Resets all the items.
        /// </summary>
        /// <param name="value">The value to set for all the items.</param>
        public override void Reset(T value = default)
        {
            base.Reset(value);
            ItemK = value;
        }

        /// <summary>
        /// Returns a tuple that represents the values of current multiple elements object.
        /// </summary>
        /// <returns>The tuple representation of this multiple elements object.</returns>
        public new Tuple<T, T, T, T, T, T, T, Tuple<T, T, T, T>> ToTuple()
        {
            return new Tuple<T, T, T, T, T, T, T, Tuple<T, T, T, T>>(ItemA, ItemB, ItemC, ItemD, ItemE, ItemF, ItemG, new Tuple<T, T, T, T>(ItemH, ItemI, ItemJ, ItemK));
        }

        /// <summary>
        /// Returns a list that represents the values of current multiple elements object.
        /// </summary>
        /// <returns>The list representation of this multiple elements object.</returns>
        public override IList<T> ToList()
        {
            return new List<T> { ItemA, ItemB, ItemC, ItemD, ItemE, ItemF, ItemG, ItemH, ItemI, ItemJ, ItemK };
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public override object Clone()
        {
            return new ElevenElements<T>
            {
                ItemA = ItemA,
                ItemB = ItemB,
                ItemC = ItemC,
                ItemD = ItemD,
                ItemE = ItemE,
                ItemF = ItemF,
                ItemG = ItemG,
                ItemH = ItemH,
                ItemI = ItemI,
                ItemJ = ItemJ,
                ItemK = ItemK
            };
        }
    }

    /// <summary>
    /// A collection with eight elements.
    /// </summary>
    /// <typeparam name="T">The type of elements.</typeparam>
    public class TwelveElements<T> : ElevenElements<T>
    {
        /// <summary>
        /// Gets or sets the eighth value of the current multiple elements object.
        /// </summary>
        public T ItemL
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the number of elements in the collection.
        /// </summary>
        public override int Count => 12;

        /// <summary>
        /// Loads the multiple value from the specific source object.
        /// </summary>
        /// <param name="value">Another multiple value to copy into the current one.</param>
        public void Load(TwelveElements<T> value)
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
            ItemI = value.ItemI;
            ItemJ = value.ItemJ;
            ItemK = value.ItemK;
            ItemL = value.ItemL;
        }

        /// <summary>
        /// Resets all the items.
        /// </summary>
        /// <param name="value">The value to set for all the items.</param>
        public override void Reset(T value = default)
        {
            base.Reset(value);
            ItemL = value;
        }

        /// <summary>
        /// Returns a tuple that represents the values of current multiple elements object.
        /// </summary>
        /// <returns>The tuple representation of this multiple elements object.</returns>
        public new Tuple<T, T, T, T, T, T, T, Tuple<T, T, T, T, T>> ToTuple()
        {
            return new Tuple<T, T, T, T, T, T, T, Tuple<T, T, T, T, T>>(ItemA, ItemB, ItemC, ItemD, ItemE, ItemF, ItemG, new Tuple<T, T, T, T, T>(ItemH, ItemI, ItemJ, ItemK, ItemL));
        }

        /// <summary>
        /// Returns a list that represents the values of current multiple elements object.
        /// </summary>
        /// <returns>The list representation of this multiple elements object.</returns>
        public override IList<T> ToList()
        {
            return new List<T> { ItemA, ItemB, ItemC, ItemD, ItemE, ItemF, ItemG, ItemH, ItemI, ItemJ, ItemK, ItemL };
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public override object Clone()
        {
            return new TwelveElements<T>
            {
                ItemA = ItemA,
                ItemB = ItemB,
                ItemC = ItemC,
                ItemD = ItemD,
                ItemE = ItemE,
                ItemF = ItemF,
                ItemG = ItemG,
                ItemH = ItemH,
                ItemI = ItemI,
                ItemJ = ItemJ,
                ItemK = ItemK,
                ItemL = ItemL
            };
        }
    }
}
