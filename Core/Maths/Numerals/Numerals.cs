// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Numerals.cs" company="Nanchang Jinchen Software Co., Ltd.">
//   Copyright (c) 2010 Nanchang Jinchen Software Co., Ltd. All rights reserved.
// </copyright>
// <summary>
//   The base models and interfaces of numeral.
// </summary>
// <author>Kingcean Tuan</author>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Trivial.Maths
{
    /// <summary>
    /// The integer sample.
    /// </summary>
    public interface IIntegerSample
    {
        /// <summary>
        /// Gets a value indicating whether it supports number 0.
        /// </summary>
        bool IsZeroSupported { get; }

        /// <summary>
        /// Number 0.
        /// </summary>
        string Zero { get; }

        /// <summary>
        /// Number 1.
        /// </summary>
        string One { get; }

        /// <summary>
        /// Number 2.
        /// </summary>
        string Two { get; }

        /// <summary>
        /// Number 3.
        /// </summary>
        string Three { get; }

        /// <summary>
        /// Number 4.
        /// </summary>
        string Four { get; }

        /// <summary>
        /// Number 5.
        /// </summary>
        string Five { get; }

        /// <summary>
        /// Number 6.
        /// </summary>
        string Six { get; }

        /// <summary>
        /// Number 7.
        /// </summary>
        string Seven { get; }

        /// <summary>
        /// Number 8.
        /// </summary>
        string Eight { get; }

        /// <summary>
        /// Number 9.
        /// </summary>
        string Nine { get; }

        /// <summary>
        /// Number 10.
        /// </summary>
        string Ten { get; }

        /// <summary>
        /// Number 11.
        /// </summary>
        string Eleven { get; }

        /// <summary>
        /// Number 12.
        /// </summary>
        string Twelve { get; }

        /// <summary>
        /// Number 13.
        /// </summary>
        string Thirteen { get; }

        /// <summary>
        /// Number 14.
        /// </summary>
        string Fourteen { get; }

        /// <summary>
        /// Number 15.
        /// </summary>
        string Fifteen { get; }

        /// <summary>
        /// Number 16.
        /// </summary>
        string Sixteen { get; }

        /// <summary>
        /// Number 17.
        /// </summary>
        string Seventeen { get; }

        /// <summary>
        /// Number 18.
        /// </summary>
        string Eighteen { get; }

        /// <summary>
        /// Number 19.
        /// </summary>
        string Nineteen { get; }

        /// <summary>
        /// Number 20.
        /// </summary>
        string Twenty { get; }

        /// <summary>
        /// Number 30.
        /// </summary>
        string Thirty { get; }

        /// <summary>
        /// Number 40.
        /// </summary>
        string Forty { get; }

        /// <summary>
        /// Number 50.
        /// </summary>
        string Fifty { get; }

        /// <summary>
        /// Number 60.
        /// </summary>
        string Sixty { get; }

        /// <summary>
        /// Number 70.
        /// </summary>
        string Seventy { get; }

        /// <summary>
        /// Number 80.
        /// </summary>
        string Eighty { get; }

        /// <summary>
        /// Number 90.
        /// </summary>
        string Ninety { get; }

        /// <summary>
        /// Number 100.
        /// </summary>
        string OneHundred { get; }

        /// <summary>
        /// Number 500.
        /// </summary>
        string FiveHundred { get; }

        /// <summary>
        /// Number 1000.
        /// </summary>
        string OneThousand { get; }
    }

    /// <summary>
    /// The local string resolver for number.
    /// </summary>
    public interface IIntegerLocalization
    {
        /// <summary>
        /// Gets the string of a specific number.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <param name="digitOnly">true if return the digit one by one directly; otherwise, false.</param>
        /// <returns>A string for the number.</returns>
        string ToString(long number, bool digitOnly = false);

        /// <summary>
        /// Gets the string of a specific number.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <param name="digitOnly">true if return the digit one by one directly; otherwise, false.</param>
        /// <returns>A string for the number.</returns>
        string ToString(ulong number, bool digitOnly = false);
    }

    /// <summary>
    /// The local string resolver for number.
    /// </summary>
    public interface INumberLocalization : IIntegerLocalization
    {
        /// <summary>
        /// Gets the string of a specific number.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns>A string for the number.</returns>
        string ToString(double number);
    }

    /// <summary>
    /// The base number digits.
    /// </summary>
    public abstract class LocalNumerals : IIntegerSample, IIntegerLocalization
    {
        /// <summary>
        /// Gets a value indicating whether it supports number 0.
        /// </summary>
        public bool IsZeroSupported => true;

        /// <summary>
        /// Sign of negative.
        /// </summary>
        public abstract string PositiveSign { get; }

        /// <summary>
        /// Sign of negative.
        /// </summary>
        public abstract string NegativeSign { get; }

        /// <summary>
        /// Number 0.
        /// </summary>
        public virtual string Zero => ToString(0);

        /// <summary>
        /// Number 1.
        /// </summary>
        public virtual string One => ToString(1);

        /// <summary>
        /// Number 2.
        /// </summary>
        public virtual string Two => ToString(2);

        /// <summary>
        /// Number 3.
        /// </summary>
        public virtual string Three => ToString(3);

        /// <summary>
        /// Number 4.
        /// </summary>
        public virtual string Four => ToString(4);

        /// <summary>
        /// Number 5.
        /// </summary>
        public virtual string Five => ToString(5);

        /// <summary>
        /// Number 6.
        /// </summary>
        public virtual string Six => ToString(6);

        /// <summary>
        /// Number 7.
        /// </summary>
        public virtual string Seven => ToString(7);

        /// <summary>
        /// Number 8.
        /// </summary>
        public virtual string Eight => ToString(8);

        /// <summary>
        /// Number 9.
        /// </summary>
        public virtual string Nine => ToString(9);

        /// <summary>
        /// Number 10.
        /// </summary>
        public virtual string Ten => ToString(10);

        /// <summary>
        /// Number 11.
        /// </summary>
        public virtual string Eleven => ToString(11);

        /// <summary>
        /// Number 12.
        /// </summary>
        public virtual string Twelve => ToString(12);

        /// <summary>
        /// Number 13.
        /// </summary>
        public virtual string Thirteen => ToString(13);

        /// <summary>
        /// Number 14.
        /// </summary>
        public virtual string Fourteen => ToString(14);

        /// <summary>
        /// Number 15.
        /// </summary>
        public virtual string Fifteen => ToString(15);

        /// <summary>
        /// Number 16.
        /// </summary>
        public virtual string Sixteen => ToString(16);

        /// <summary>
        /// Number 17.
        /// </summary>
        public virtual string Seventeen => ToString(17);

        /// <summary>
        /// Number 18.
        /// </summary>
        public virtual string Eighteen => ToString(18);

        /// <summary>
        /// Number 19.
        /// </summary>
        public virtual string Nineteen => ToString(19);

        /// <summary>
        /// Number 20.
        /// </summary>
        public virtual string Twenty => ToString(20);

        /// <summary>
        /// Number 30.
        /// </summary>
        public virtual string Thirty => ToString(30);

        /// <summary>
        /// Number 40.
        /// </summary>
        public virtual string Forty => ToString(40);

        /// <summary>
        /// Number 50.
        /// </summary>
        public virtual string Fifty => ToString(50);

        /// <summary>
        /// Number 60.
        /// </summary>
        public virtual string Sixty => ToString(60);

        /// <summary>
        /// Number 70.
        /// </summary>
        public virtual string Seventy => ToString(70);

        /// <summary>
        /// Number 80.
        /// </summary>
        public virtual string Eighty => ToString(80);

        /// <summary>
        /// Number 90.
        /// </summary>
        public virtual string Ninety => ToString(90);

        /// <summary>
        /// Number 100.
        /// </summary>
        public virtual string OneHundred => ToString(100);

        /// <summary>
        /// Number 500.
        /// </summary>
        public virtual string FiveHundred => ToString(500);

        /// <summary>
        /// Number 1,000.
        /// </summary>
        public virtual string OneThousand => ToString(1000);

        /// <summary>
        /// Number 5,000.
        /// </summary>
        public virtual string FiveThousand => ToString(5000);

        /// <summary>
        /// Number 10,000.
        /// </summary>
        public virtual string TenThousand => ToString(10000);

        /// <summary>
        /// Number 100,000.
        /// </summary>
        public virtual string OneHundredThousand => ToString(100000);

        /// <summary>
        /// Number 1,000,000.
        /// </summary>
        public virtual string OneMillion => ToString(1000000);

        /// <summary>
        /// Gets the string of a specific number.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <param name="digitOnly">true if return the digit one by one directly; otherwise, false.</param>
        /// <returns>A string for the number.</returns>
        public abstract string ToString(long number, bool digitOnly = false);

        /// <summary>
        /// Gets the string of a specific number.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <param name="digitOnly">true if return the digit one by one directly; otherwise, false.</param>
        /// <returns>A string for the number.</returns>
        public abstract string ToString(ulong number, bool digitOnly = false);
    }
}
