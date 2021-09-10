// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Arithmetic\Basic.cs" company="Nanchang Jinchen Software Co., Ltd.">
//   Copyright (c) 2010 Nanchang Jinchen Software Co., Ltd. All rights reserved.
// </copyright>
// <summary>
//   The basic arithmetic functions.
// </summary>
// <author>Kingcean Tuan</author>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trivial.Maths
{
    /// <summary>
    /// The result for rank of 3 items.
    /// </summary>
    /// <typeparam name="T">The type of item.</typeparam>
    public class RankResult3<T> where T : IComparable
    {
        /// <summary>
        /// Initializes a new instance of the RankResult3 class.
        /// </summary>
        /// <param name="item1">Item 1.</param>
        /// <param name="item2">Item 2.</param>
        /// <param name="item3">Item 3.</param>
        public RankResult3(T item1, T item2, T item3)
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;

            if (item1.CompareTo(item2) > 0)
            {
                if (item2.CompareTo(item3) > 0)
                    SetRank(1, 2, 3, 1, 2, 3);
                else if (item2.CompareTo(item3) == 0)
                    SetRank(1, 2, 2, 1, 2, 3);
                else if (item1.CompareTo(item3) > 0)
                    SetRank(1, 3, 2, 1, 3, 2);
                else if (item1.CompareTo(item3) < 0)
                    SetRank(2, 3, 1, 3, 1, 2);
                else
                    SetRank(1, 2, 1, 1, 3, 2);
            }
            else if (item1.CompareTo(item2) < 0)
            {
                if (item1.CompareTo(item3) > 0)
                    SetRank(2, 1, 3, 2, 1, 3);
                else if (item1.CompareTo(item3) == 0)
                    SetRank(2, 1, 2, 2, 1, 3);
                else if (item2.CompareTo(item3) > 0)
                    SetRank(3, 1, 2, 2, 3, 1);
                else if (item2.CompareTo(item3) < 0)
                    SetRank(3, 2, 1, 3, 2, 1);
                else
                    SetRank(2, 1, 1, 2, 3, 1);
            }
            else
            {
                if (item1.CompareTo(item3) > 0)
                    SetRank(1, 1, 2, 1, 2, 3);
                else if (item1.CompareTo(item3) < 0)
                    SetRank(2, 2, 1, 3, 1, 2);
                else
                    SetRank(1, 1, 1, 1, 2, 3);
            }
        }

        /// <summary>
        /// Gets the item 1.
        /// </summary>
        public T Item1 { get; }

        /// <summary>
        /// Gets the item 2.
        /// </summary>
        public T Item2 { get; }

        /// <summary>
        /// Gets the item 3.
        /// </summary>
        public T Item3 { get; }

        /// <summary>
        /// Gets the rank number for item 1.
        /// </summary>
        public int RankFor1 { get; private set; }

        /// <summary>
        /// Gets the rank number for item 2.
        /// </summary>
        public int RankFor2 { get; private set; }

        /// <summary>
        /// Gets the rank number for item 3.
        /// </summary>
        public int RankFor3 { get; private set; }

        /// <summary>
        /// Gets the number 1 of item.
        /// </summary>
        public T Number1 { get; private set; }

        /// <summary>
        /// Gets the number 2 of item.
        /// </summary>
        public T Number2 { get; private set; }

        /// <summary>
        /// Gets the number 3 of item.
        /// </summary>
        public T Number3 { get; private set; }

        /// <summary>
        /// Gets the index of number 1.
        /// </summary>
        public int IndexOfNumber1 { get; private set; }

        /// <summary>
        /// Gets the index of number 2.
        /// </summary>
        public int IndexOfNumber2 { get; private set; }

        /// <summary>
        /// Gets the index of number 3.
        /// </summary>
        public int IndexOfNumber3 { get; private set; }

        /// <summary>
        /// Gets the item by index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The value of the specific item.</returns>
        /// <exception cref="ArgumentOutOfRangeException">index was less than 1 or greater than 3.</exception>
        public T this[int index]
        {
            get => index switch
            {
                1 => Item1,
                2 => Item2,
                3 => Item3,
                _ => throw new ArgumentOutOfRangeException(nameof(index), "index is less than 1 or greater than 3.")
            };
        }

        private void SetRank(int a, int b, int c, int no1, int no2, int no3)
        {
            RankFor1 = a;
            RankFor2 = b;
            RankFor3 = c;
            IndexOfNumber1 = no1;
            IndexOfNumber2 = no2;
            IndexOfNumber3 = no3;
            Number1 = this[1];
            Number2 = this[2];
            Number3 = this[3];
        }
    }
}
