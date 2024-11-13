using System;

namespace Trivial.Maths;

/// <summary>
/// Specifies basic boolean comparing operators.
/// </summary>
public enum BasicCompareOperator : byte
{
    /// <summary>
    /// Equaling.
    /// Symbol =.
    /// </summary>
    Equal = 0,

    /// <summary>
    /// Not equaling.
    /// Symbol ≠.
    /// </summary>
    NotEqual = 1,

    /// <summary>
    /// Greater than.
    /// Symbol &gt;.
    /// </summary>
    Greater = 2,

    /// <summary>
    /// Less than.
    /// Symbol &lt;.
    /// </summary>
    Less = 3,

    /// <summary>
    /// Greater than or equaling.
    /// Symbol ≥.
    /// </summary>
    GreaterOrEqual = 4,

    /// <summary>
    /// Less than or equaling.
    /// Symbol ≤.
    /// </summary>
    LessOrEqual = 5,
}

/// <summary>
/// Specifies unary boolean computing operators.
/// </summary>
public enum UnaryBooleanOperator : byte
{
    /// <summary>
    /// No action.
    /// </summary>
    Default = 0,

    /// <summary>
    /// Not the one. Negative.
    /// Symbol ¬.
    /// </summary>
    Not = 1
}

/// <summary>
/// Specifies binary boolean computing operators.
/// </summary>
public enum BinaryBooleanOperator : byte
{
    /// <summary>
    /// AND boolean operator.
    /// Symbol ∧.
    /// </summary>
    And = 0,

    /// <summary>
    /// OR boolean operator.
    /// Symobl ∨.
    /// </summary>
    Or = 1,

    /// <summary>
    /// Exclusive OR boolean operator.
    /// Symbol ⊕.
    /// </summary>
    Xor = 2,

    /// <summary>
    /// Negated AND boolean operator.
    /// </summary>
    Nand = 3,

    /// <summary>
    /// Negated OR boolean operator.
    /// </summary>
    Nor = 4,

    /// <summary>
    /// Exclusive Negated OR boolean operator.
    /// Symbol ⊙.
    /// </summary>
    Xnor = 5
}

/// <summary>
/// Specifies 3D boolean computing operators.
/// </summary>
public enum CollectionBooleanOperator : byte
{
    /// <summary>
    /// Union two collections.
    /// Symbol ∪.
    /// </summary>
    Union = 0,

    /// <summary>
    /// Intersection two collectons.
    /// Symbol ∩.
    /// </summary>
    Intersection = 1,

    /// <summary>
    /// Subtranction a collection by another collection.
    /// </summary>
    Subtranction = 2
}

/// <summary>
/// Specifies sequence boolean computing operators.
/// </summary>
public enum SequenceBooleanOperator : byte
{
    /// <summary>
    /// AND boolean operator.
    /// That means to return true if all are true; otherwise, false.
    /// </summary>
    And = 0,

    /// <summary>
    /// OR boolean operator.
    /// That means to return true if one or more are true; otherwise, false. 
    /// </summary>
    Or = 1,

    /// <summary>
    /// Negated AND boolean operator.
    /// That means: AND operator first, not operator then.
    /// </summary>
    Nand = 2,

    /// <summary>
    /// Negated OR boolean operator.
    /// That means: OR operator first, not operator then.
    /// </summary>
    Nor = 3,

    /// <summary>
    /// All the same.
    /// </summary>
    Same = 4,

    /// <summary>
    /// Various values.
    /// </summary>
    Various = 5,

    /// <summary>
    /// The first value.
    /// </summary>
    First = 6,

    /// <summary>
    /// The last value.
    /// </summary>
    Last = 7,

    /// <summary>
    /// The most value.
    /// </summary>
    Most = 8,

    /// <summary>
    /// The most value.
    /// </summary>
    MostOrFirst = 9,

    /// <summary>
    /// The most value.
    /// </summary>
    MostOrLast = 10,

    /// <summary>
    /// The least value.
    /// </summary>
    Least = 11,

    /// <summary>
    /// The least value.
    /// </summary>
    LeastOrFirst = 12,

    /// <summary>
    /// The least value.
    /// </summary>
    LeastOrLast = 13,

    /// <summary>
    /// Each values are a half.
    /// </summary>
    Half = 14,

    /// <summary>
    /// Each values are not a half.
    /// </summary>
    Nhalf = 15,

    /// <summary>
    /// The count of true is greater than or equals to the one of false.
    /// </summary>
    Positive = 16,

    /// <summary>
    /// The count of false is greater than or equals to the one of true.
    /// </summary>
    Negative = 17,

    /// <summary>
    /// Always true.
    /// </summary>
    True = 30,

    /// <summary>
    /// Always false.
    /// </summary>
    False = 31,
}

/// <summary>
/// Specifies criteria boolean computing operators.
/// </summary>
public enum CriteriaBooleanOperator : byte
{
    /// <summary>
    /// AND Boolean operator.
    /// Symbol &amp;
    /// </summary>
    And = 0,

    /// <summary>
    /// OR Boolean operator.
    /// Symbol |.
    /// </summary>
    Or = 1
}
