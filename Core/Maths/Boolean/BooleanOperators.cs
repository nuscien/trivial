// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BooleanOperators.cs" company="Nanchang Jinchen Software Co., Ltd.">
//   Copyright (c) 2010 Nanchang Jinchen Software Co., Ltd. All rights reserved.
// </copyright>
// <summary>
//   About value operators.
// </summary>
// <author>Kingcean Tuan</author>
// --------------------------------------------------------------------------------------------------------------------

namespace Trivial.Maths
{
    /// <summary>
    /// The symbols for boolean operators.
    /// </summary>
    public static class BooleanSymbols
    {
        /// <summary>
        /// The operator sign of not.
        /// </summary>
        public const string NotSign = "¬";

        /// <summary>
        /// The operator sign of equal.
        /// </summary>
        public const string EqualSign = "=";

        /// <summary>
        /// The operator sign of not equal.
        /// </summary>
        public const string NotEqualSign = "≠";

        /// <summary>
        /// The operator sign of similar.
        /// </summary>
        public const string SimilarSign = "≈";

        /// <summary>
        /// The operator sign of greater.
        /// </summary>
        public const string GreaterSign = ">";

        /// <summary>
        /// The operator sign of less.
        /// </summary>
        public const string LessSign = "<";

        /// <summary>
        /// The operator sign of greater or equal.
        /// </summary>
        public const string GreaterOrEqualSign = "≥";

        /// <summary>
        /// The operator sign of less or equal.
        /// </summary>
        public const string LessOrEqualSign = "≤";

        /// <summary>
        /// The operator sign of not greater.
        /// </summary>
        public const string NotGreaterSign = "≮";

        /// <summary>
        /// The operator sign of not less.
        /// </summary>
        public const string NotLessSign = "≯";

        /// <summary>
        /// The operator sign of and.
        /// </summary>
        public const string AndSign = "∧";

        /// <summary>
        /// The operator sign of or.
        /// </summary>
        public const string OrSign = "∨";

        /// <summary>
        /// The operator sign of eor.
        /// </summary>
        public const string XorSign = "⊕";

        /// <summary>
        /// The operator sign of union.
        /// </summary>
        public const string UnionSign = "∪";

        /// <summary>
        /// The operator sign of intersection.
        /// </summary>
        public const string IntersectionSign = "∩";

        /// <summary>
        /// The operator sign of include.
        /// </summary>
        public const string IncludeSign = "∈";
    }

    /// <summary>
    /// Specifies basic boolean comparing operators.
    /// </summary>
    public enum BasicCompareOperator
    {
        /// <summary>
        /// Equaling.
        /// </summary>
        Equal = 0,

        /// <summary>
        /// Not equaling.
        /// </summary>
        NotEqual = 1,

        /// <summary>
        /// Greater than.
        /// </summary>
        Greater = 2,

        /// <summary>
        /// Less than.
        /// </summary>
        Less = 3,

        /// <summary>
        /// Greater than or equaling.
        /// </summary>
        GreaterOrEqual = 4,

        /// <summary>
        /// Less than or equaling.
        /// </summary>
        LessOrEqual = 5,
    }

    /// <summary>
    /// Specifies unary boolean computing operators.
    /// </summary>
    public enum UnaryBooleanOperator
    {
        /// <summary>
        /// No action.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Not the one. Negative.
        /// </summary>
        Not = 1
    }

    /// <summary>
    /// Specifies binary boolean computing operators.
    /// </summary>
    public enum BinaryBooleanOperator
    {
        /// <summary>
        /// AND Boolean operator.
        /// </summary>
        And = 0,

        /// <summary>
        /// OR Boolean operator.
        /// </summary>
        Or = 1,

        /// <summary>
        /// XOR Boolean operator.
        /// </summary>
        Xor = 2
    }

    /// <summary>
    /// Specifies 3D boolean computing operators.
    /// </summary>
    public enum CollectionBooleanOperator
    {
        /// <summary>
        /// Union two collections.
        /// </summary>
        Union = 0,

        /// <summary>
        /// Intersection two collectons.
        /// </summary>
        Intersection = 1,

        /// <summary>
        /// Subtranction a collection by another collection.
        /// </summary>
        Subtranction = 2
    }

    /// <summary>
    /// Specifies criteria boolean computing operators.
    /// </summary>
    public enum CriteriaBooleanOperator
    {
        /// <summary>
        /// AND Boolean operator.
        /// </summary>
        And = 0,

        /// <summary>
        /// OR Boolean operator.
        /// </summary>
        Or = 1
    }
}
