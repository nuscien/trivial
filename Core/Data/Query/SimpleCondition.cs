// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SimpleCondition.cs" company="Nanchang Jinchen Software Co., Ltd.">
//   Copyright (c) 2010 Nanchang Jinchen Software Co., Ltd. All rights reserved.
// </copyright>
// <summary>
//   The simple condition.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Trivial.Maths;
using Trivial.Reflection;

namespace Trivial.Data;

/// <summary>
/// The interface for simple condition.
/// </summary>
public interface ISimpleCondition
{
    /// <summary>
    /// Gets or sets he value for the comparing in the condition object.
    /// </summary>
    object Value { get; set; }

    /// <summary>
    /// Gets or sets he comparing operator.
    /// </summary>
    DbCompareOperator Operator { get; set; }
    
    /// <summary>
    /// Gets the type of the value.
    /// </summary>
    DbValueType ValueType { get; }

    /// <summary>
    /// Gets a value indicating whether the value of the condition is null.
    /// </summary>
    bool ValueIsNull { get; }
}

/// <summary>
/// The base class for simple condition.
/// </summary>
public interface IClassSimpleCondition<T> where T : class
{
    /// <summary>
    /// Gets or sets the value for the comparing in the condition object.
    /// </summary>
    T Value { get; set; }

    /// <summary>
    /// Gets or sets he comparing operator.
    /// </summary>
    DbCompareOperator Operator { get; set; }

    /// <summary>
    /// Gets the type of the value.
    /// </summary>
    DbValueType ValueType { get; }

    /// <summary>
    /// Gets a value indicating whether the value of the condition is null.
    /// </summary>
    bool ValueIsNull { get; }
}

/// <summary>
/// The base class for simple condition.
/// </summary>
public interface IStructSimpleCondition<T> where T : struct
{
    /// <summary>
    /// Gets or sets the value for the comparing in the condition object.
    /// </summary>
    T? Value { get; set; }

    /// <summary>
    /// Gets or sets he comparing operator.
    /// </summary>
    DbCompareOperator Operator { get; set; }

    /// <summary>
    /// Gets the type of the value.
    /// </summary>
    DbValueType ValueType { get; }

    /// <summary>
    /// Gets a value indicating whether the value of the condition is null.
    /// </summary>
    bool ValueIsNull { get; }
}

/// <summary>
/// The base class for simple condition.
/// </summary>
public abstract class SimpleCondition : ISimpleCondition
{
    /// <summary>
    /// A list about valid comparing operator for null or boolean value.
    /// </summary>
    private static List<DbCompareOperator> _validNullValueOp;

    /// <summary>
    /// A list about valid comparing operator for literal value.
    /// </summary>
    private static List<DbCompareOperator> _validLiteralOp;

    /// <summary>
    /// A list about valid comparing operator for comparable value.
    /// </summary>
    private static List<DbCompareOperator> _validComparableOp;

    /// <summary>
    /// Gets or sets the value for the comparing in the condition object.
    /// </summary>
    public virtual object Value { get; set; }

    /// <summary>
    /// Gets or sets the comparing operator.
    /// </summary>
    public DbCompareOperator Operator { get; set; }

    /// <summary>
    /// Gets the type of the value.
    /// </summary>
    public abstract DbValueType ValueType { get; }

    /// <summary>
    /// Gets a value indicating whether the value of the condition is null.
    /// </summary>
    public abstract bool ValueIsNull { get; }

    /// <summary>
    /// Gets a left comparing operator from a simple interval.
    /// </summary>
    /// <typeparam name="T">The type of interval value.</typeparam>
    /// <param name="value">A simple interval instance.</param>
    /// <returns>A comparing operator.</returns>
    public static DbCompareOperator GetLeftOperator<T>(ISimpleInterval<T> value)
        => value == null
            ? throw ObjectConvert.ArgumentNull(nameof(value))
            : (value.LeftOpen ? DbCompareOperator.Greater : DbCompareOperator.GreaterOrEqual);

    /// <summary>
    /// Gets a right comparing operator from a simple interval.
    /// </summary>
    /// <typeparam name="T">The type of interval value.</typeparam>
    /// <param name="value">A simple interval instance.</param>
    /// <returns>A comparing operator.</returns>
    public static DbCompareOperator GetRightOperator<T>(ISimpleInterval<T> value)
        => value == null
            ? throw ObjectConvert.ArgumentNull(nameof(value))
            : (value.RightOpen ? DbCompareOperator.Less : DbCompareOperator.LessOrEqual);

    /// <summary>
    /// Gets the list about valid comparing operator for null or boolean value.
    /// </summary>
    public static ICollection<DbCompareOperator> GetBasicValidOperators()
        => _validNullValueOp ??= new List<DbCompareOperator>
                                           {
                                               DbCompareOperator.Equal,
                                               DbCompareOperator.NotEqual
                                           };

    /// <summary>
    /// Gets the list about valid comparing operator for literal value.
    /// </summary>
    public static ICollection<DbCompareOperator> GetLiteralValidOperators()
        => _validLiteralOp ??= new List<DbCompareOperator>
                                           {
                                               DbCompareOperator.Equal,
                                               DbCompareOperator.NotEqual,
                                               DbCompareOperator.Contains,
                                               DbCompareOperator.EndsWith,
                                               DbCompareOperator.StartsWith
                                           };

    /// <summary>
    /// Gets the list about valid comparing operator for comparable value.
    /// </summary>
    public static ICollection<DbCompareOperator> GetComparableValidOperators()
        => _validComparableOp ??= new List<DbCompareOperator>
                                           {
                                               DbCompareOperator.Equal,
                                               DbCompareOperator.NotEqual,
                                               DbCompareOperator.Greater,
                                               DbCompareOperator.Less,
                                               DbCompareOperator.GreaterOrEqual,
                                               DbCompareOperator.LessOrEqual
                                           };

    /// <summary>
    /// Converts operation string.
    /// </summary>
    /// <param name="op"></param>
    /// <returns>A string that represents the operation.</returns>
    public static string ToString(DbCompareOperator op)
        => op switch
        {
            DbCompareOperator.Equal => BooleanOperations.EqualSign,
            DbCompareOperator.Greater => BooleanOperations.GreaterSign,
            DbCompareOperator.GreaterOrEqual => BooleanOperations.GreaterOrEqualSign,
            DbCompareOperator.Less => BooleanOperations.LessSign,
            DbCompareOperator.LessOrEqual => BooleanOperations.LessOrEqualSign,
            DbCompareOperator.NotEqual => BooleanOperations.NotEqualSign,
            _ => op.ToString(),
        };

    internal static DbCompareOperator Parse(string s)
        => (s?.Trim()?.ToLowerInvariant() ?? string.Empty) switch
        {
            "equal" or "eq" or "==" => DbCompareOperator.Equal,
            "not equal" or "neq" or "!=" or "<>" => DbCompareOperator.NotEqual,
            "greater" or "gt" or ">" => DbCompareOperator.Greater,
            "less" or "lt" or "<" => DbCompareOperator.Greater,
            "greater than or equal to" or "greater or equal" or "ge" or ">=" or "≥" => DbCompareOperator.GreaterOrEqual,
            "less than or equal to" or "less or equal" or "le" or "<=" or "≤" => DbCompareOperator.LessOrEqual,
            "start" or "start with" => DbCompareOperator.StartsWith,
            "end" or "end with" => DbCompareOperator.EndsWith,
            "contain" or "like" => DbCompareOperator.Contains,
            _ => throw new NotSupportedException("The operator does not supported.")
        };

    internal static BasicCompareOperator ParseBasic(string s)
        => (s?.Trim()?.ToLowerInvariant() ?? string.Empty) switch
        {
            "equal" or "eq" or "==" => BasicCompareOperator.Equal,
            "not equal" or "neq" or "!=" or "<>" => BasicCompareOperator.NotEqual,
            "greater" or "gt" or ">" => BasicCompareOperator.Greater,
            "less" or "lt" or "<" => BasicCompareOperator.Greater,
            "greater than or equal to" or "greater or equal" or "ge" or ">=" or "≥" => BasicCompareOperator.GreaterOrEqual,
            "less than or equal to" or "less or equal" or "le" or "<=" or "≤" => BasicCompareOperator.LessOrEqual,
            _ => throw new NotSupportedException("The operator does not supported.")
        };

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
        => ToString(Operator) + " " + (Value != null ? Value.ToString() : "null");
}

/// <summary>
/// The base generic class for simple condition with a reference value.
/// </summary>
/// <typeparam name="T">The type of the value. Should be a reference type.</typeparam>
public abstract class ClassSimpleCondition<T> : ISimpleCondition, IClassSimpleCondition<T> where T : class
{
    /// <summary>
    /// Initializes a new instance of the ClassSimpleCondition class.
    /// </summary>
    /// <remarks>You can use this to initialize an instance for the class.</remarks>
    protected ClassSimpleCondition()
    {
    }

    /// <summary>
    /// Initializes a new instance of the ClassSimpleCondition class.
    /// </summary>
    /// <param name="copier">An instance to copy.</param>
    /// <remarks>You can use this to initialize an instance for the class.</remarks>
    protected ClassSimpleCondition(IClassSimpleCondition<T> copier)
    {
        if (copier == null) return;
        Value = copier.Value;
        Operator = copier.Operator;
    }

    /// <summary>
    /// Initializes a new instance of the ClassSimpleCondition class.
    /// </summary>
    /// <param name="op">The operator in the condition.</param>
    /// <param name="value">The value for comparing in the condition.</param>
    /// <remarks>You can use this to initialize an instance for the class.</remarks>
    protected ClassSimpleCondition(DbCompareOperator op, T value)
    {
        Value = value;
        Operator = op;
    }

    /// <summary>
    /// Gets or sets the value for the comparing in the condition object.
    /// </summary>
    /// <exception cref="InvalidCastException">The value to set is not the specific type.</exception>
    object ISimpleCondition.Value
    {
        get => Value;
        set => Value = (T)value;
    }

    /// <summary>
    /// Gets or sets the value for the comparing in the condition object.
    /// </summary>
    public T Value { get; set; }

    /// <summary>
    /// Gets or sets the comparing operator.
    /// </summary>
    public DbCompareOperator Operator { get; set; }

    /// <summary>
    /// Gets a value indicating whether the value of the condition is null.
    /// </summary>
    public bool ValueIsNull => Value == null;

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
        => SimpleCondition.ToString(Operator) + " " + (Value != null ? Value.ToString() : "null");

    /// <summary>
    /// Checks if the simple condition with reference value is valid.
    /// </summary>
    /// <param name="value">The condition object.</param>
    /// <param name="handler">A checker handler.</param>
    /// <returns>true if it is valid; otherwise, false.</returns>
    internal static bool IsValid(ClassSimpleCondition<T> value, Func<DbCompareOperator, bool> handler)
    {
        if (value == null) return false;
        return value.Value == null
                   ? SimpleCondition.GetBasicValidOperators().Contains(value.Operator)
                   : handler(value.Operator);
    }

    /// <summary>
    /// Gets the type of the value.
    /// </summary>
    public abstract DbValueType ValueType { get; }
}

/// <summary>
/// The base generic class for simple condition with a reference value.
/// </summary>
/// <typeparam name="T">The type of the value. Should be a struct type.</typeparam>
public abstract class StructSimpleCondition<T> : ISimpleCondition, IStructSimpleCondition<T> where T : struct
{
    /// <summary>
    /// Initializes a new instance of the StructSimpleCondition class.
    /// </summary>
    /// <remarks>You can use this to initialize an instance for the class.</remarks>
    protected StructSimpleCondition()
    {
    }

    /// <summary>
    /// Initializes a new instance of the StructSimpleCondition class.
    /// </summary>
    /// <param name="copier">An instance to copy.</param>
    /// <remarks>You can use this to initialize an instance for the class.</remarks>
    protected StructSimpleCondition(IStructSimpleCondition<T> copier)
    {
        if (copier == null) return;
        Value = copier.Value;
        Operator = copier.Operator;
    }

    /// <summary>
    /// Initializes a new instance of the StructSimpleCondition class.
    /// </summary>
    /// <param name="op">The operator in the condition.</param>
    /// <param name="value">The value for comparing in the condition.</param>
    /// <remarks>You can use this to initialize an instance for the class.</remarks>
    protected StructSimpleCondition(DbCompareOperator op, T value)
    {
        Value = value;
        Operator = op;
    }

    /// <summary>
    /// Gets or sets the value for the comparing in the condition object.
    /// </summary>
    object ISimpleCondition.Value
    {
        get { return Value; }
        set { Value = (T) value; }
    }

    /// <summary>
    /// Gets or sets the value for the comparing in the condition object.
    /// </summary>
    public T? Value { get; set; }

    /// <summary>
    /// Gets or sets the comparing operator.
    /// </summary>
    public DbCompareOperator Operator { get; set; }

    /// <summary>
    /// Gets the type of the value.
    /// </summary>
    public abstract DbValueType ValueType { get; }

    /// <summary>
    /// Gets a value indicating whether the value of the condition is null.
    /// </summary>
    public bool ValueIsNull { get { return Value == null; } }

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
    {
        return SimpleCondition.ToString(Operator) + " " + (Value.HasValue ? Value.ToString() : "null");
    }

    /// <summary>
    /// Checks if the simple condition with struct value is valid.
    /// </summary>
    /// <param name="value">The condition object.</param>
    /// <param name="handler">A checker handler.</param>
    /// <returns>true if it is valid; otherwise, false.</returns>
    internal static bool IsValid(StructSimpleCondition<T> value, Func<DbCompareOperator, bool> handler)
    {
        if (value == null) return false;
        return value.Value == null
                   ? SimpleCondition.GetBasicValidOperators().Contains(value.Operator)
                   : handler(value.Operator);
    }
}

/// <summary>
/// The simple condition class for string.
/// </summary>
public class StringCondition : ClassSimpleCondition<string>
{
    /// <summary>
    /// Initializes a new instance of the StringCondition class.
    /// </summary>
    /// <remarks>You can use this to initialize an instance for the class.</remarks>
    public StringCondition()
    {
    }

    /// <summary>
    /// Initializes a new instance of the StringCondition class.
    /// </summary>
    /// <param name="copier">An instance to copy.</param>
    /// <remarks>You can use this to initialize an instance for the class.</remarks>
    public StringCondition(IClassSimpleCondition<string> copier) : base(copier)
    {
    }

    /// <summary>
    /// Initializes a new instance of the StringCondition class.
    /// </summary>
    /// <param name="op">The operator in the condition.</param>
    /// <param name="value">The value for comparing in the condition.</param>
    /// <remarks>You can use this to initialize an instance for the class.</remarks>
    public StringCondition(DbCompareOperator op, string value) : base(op, value)
    {
    }

    /// <summary>
    /// Creates a string condition as starting with a value.
    /// </summary>
    /// <param name="value">The value for comparing in the condition.</param>
    /// <returns>A StringCondition value.</returns>
    public static StringCondition CreateForStartingWith(string value)
        => new(DbCompareOperator.StartsWith, value);

    /// <summary>
    /// Creates a string condition as ending with a value.
    /// </summary>
    /// <param name="value">The value for comparing in the condition.</param>
    /// <returns>A StringCondition value.</returns>
    public static StringCondition CreateForEndingWith(string value)
        => new(DbCompareOperator.EndsWith, value);

    /// <summary>
    /// Creates a string condition as liking with a value.
    /// </summary>
    /// <param name="value">The value for comparing in the condition.</param>
    /// <returns>A StringCondition value.</returns>
    public static StringCondition CreateForLiking(string value)
        => new(DbCompareOperator.Contains, value);

    /// <summary>
    /// Creates a string condition as equaling with a value.
    /// </summary>
    /// <param name="value">The value for comparing in the condition.</param>
    /// <returns>A StringCondition value.</returns>
    public static StringCondition CreateForEqualing(string value)
        => new(DbCompareOperator.Equal, value);

    /// <summary>
    /// Creates a string condition as not equaling with a value.
    /// </summary>
    /// <param name="value">The value for comparing in the condition.</param>
    /// <returns>A StringCondition value.</returns>
    public static StringCondition CreateForNotEqualing(string value)
        => new(DbCompareOperator.NotEqual, value);

    /// <summary>
    /// Checks if a string condition is valid.
    /// </summary>
    /// <param name="value">The condition object.</param>
    /// <returns>true if it is valid; otherwise, false.</returns>
    public static bool IsValid(StringCondition value)
        => IsValid(value, SimpleCondition.GetLiteralValidOperators().Contains);

    /// <summary>
    /// Gets the type of the value.
    /// </summary>
    public override DbValueType ValueType => DbValueType.LiteralString;

    /// <summary>
    /// Tests if the given string is macthed.
    /// </summary>
    /// <param name="source">The source to test the condition.</param>
    /// <returns>true if it is matched; otherwise, false.</returns>
    public bool IsMatched(string source)
    {
        switch (Operator)
        {
            case DbCompareOperator.Contains:
                if (string.IsNullOrEmpty(Value)) return true;
                if (string.IsNullOrEmpty(source)) return false;
                return source.IndexOf(Value) >= 0;
            case DbCompareOperator.StartsWith:
                if (string.IsNullOrEmpty(Value)) return true;
                if (string.IsNullOrEmpty(source)) return false;
                return source.IndexOf(Value) == 0;
            case DbCompareOperator.EndsWith:
                if (string.IsNullOrEmpty(Value)) return true;
                if (string.IsNullOrEmpty(source)) return false;
                return source.LastIndexOf(Value) == source.Length - Value.Length;
            case DbCompareOperator.Equal:
                return source == Value;
            case DbCompareOperator.NotEqual:
                return source != Value;
        }

        return false;
    }
}

/// <summary>
/// The simple condition class for Int32.
/// </summary>
public class Int32Condition : StructSimpleCondition<int>
{
    /// <summary>
    /// Initializes a new instance of the Int32Condition class.
    /// </summary>
    /// <remarks>You can use this to initialize an instance for the class.</remarks>
    public Int32Condition()
    {
    }

    /// <summary>
    /// Initializes a new instance of the Int32Condition class.
    /// </summary>
    /// <param name="copier">An instance to copy.</param>
    /// <remarks>You can use this to initialize an instance for the class.</remarks>
    public Int32Condition(IStructSimpleCondition<int> copier)
        : base(copier)
    {
    }

    /// <summary>
    /// Initializes a new instance of the Int32Condition class.
    /// </summary>
    /// <param name="op">The operator in the condition.</param>
    /// <param name="value">The value for comparing in the condition.</param>
    /// <remarks>You can use this to initialize an instance for the class.</remarks>
    public Int32Condition(DbCompareOperator op, int value) : base(op, value)
    {
    }

    /// <summary>
    /// Get a condition from the given struct value simple interval left bound.
    /// </summary>
    /// <param name="value">A simple interval.</param>
    /// <returns>A condition from left bound of a specific simple interval.</returns>
    public static Int32Condition CreateFromLeft(StructValueSimpleInterval<int> value)
    {
        if (value == null) throw ObjectConvert.ArgumentNull(nameof(value));
        return new()
        {
            Value = value.MinValue,
            Operator = SimpleCondition.GetLeftOperator(value)
        };
    }

    /// <summary>
    /// Get a condition from the given struct value simple interval left bound.
    /// </summary>
    /// <param name="value">A simple interval.</param>
    /// <returns>A condition from right bound of a specific simple interval.</returns>
    public static Int32Condition CreateFromRight(StructValueSimpleInterval<int> value)
    {
        if (value == null) throw ObjectConvert.ArgumentNull(nameof(value));
        return new()
        {
            Value = value.MaxValue,
            Operator = SimpleCondition.GetRightOperator(value)
        };
    }

    /// <summary>
    /// Get a condition from the given nullable value simple interval left bound.
    /// </summary>
    /// <param name="value">A simple interval.</param>
    /// <returns>A condition from left bound of a specific simple interval.</returns>
    public static Int32Condition CreateFromLeft(NullableValueSimpleInterval<int> value)
    {
        if (value == null) throw ObjectConvert.ArgumentNull(nameof(value));
        if (!value.LeftBounded) return null;
        return new()
        {
            Value = value.MinValue,
            Operator = SimpleCondition.GetLeftOperator(value)
        };
    }

    /// <summary>
    /// Get a condition from the given nullable value simple interval left bound.
    /// </summary>
    /// <param name="value">A simple interval.</param>
    /// <returns>A condition from right bound of a specific simple interval.</returns>
    public static Int32Condition CreateFromRight(NullableValueSimpleInterval<int> value)
    {
        if (value == null) throw ObjectConvert.ArgumentNull(nameof(value));
        if (!value.RightBounded) return null;
        return new()
        {
            Value = value.MaxValue,
            Operator = SimpleCondition.GetRightOperator(value)
        };
    }

    /// <summary>
    /// Checks if a integer condition is valid.
    /// </summary>
    /// <param name="value">The condition object.</param>
    /// <returns>true if it is valid; otherwise, false.</returns>
    public static bool IsValid(Int32Condition value)
        => IsValid(value, SimpleCondition.GetComparableValidOperators().Contains);

    /// <summary>
    /// Gets the type of the value.
    /// </summary>
    public override DbValueType ValueType => DbValueType.Int32;

    /// <summary>
    /// Tests if the given number is macthed.
    /// </summary>
    /// <param name="source">The source to test the condition.</param>
    /// <returns>true if it is matched; otherwise, false.</returns>
    public bool IsMatched(int source)
        => Operator switch
        {
            DbCompareOperator.Equal => source == Value,
            DbCompareOperator.NotEqual => source != Value,
            DbCompareOperator.Greater => source > Value,
            DbCompareOperator.GreaterOrEqual => source >= Value,
            DbCompareOperator.Less => source < Value,
            DbCompareOperator.LessOrEqual => source <= Value,
            _ => false,
        };

    /// <summary>
    /// Tests if the given number is macthed.
    /// </summary>
    /// <param name="source">The source to test the condition.</param>
    /// <returns>true if it is matched; otherwise, false.</returns>
    public bool IsMatched(long source)
        => Operator switch
        {
            DbCompareOperator.Equal => source == Value,
            DbCompareOperator.NotEqual => source != Value,
            DbCompareOperator.Greater => source > Value,
            DbCompareOperator.GreaterOrEqual => source >= Value,
            DbCompareOperator.Less => source < Value,
            DbCompareOperator.LessOrEqual => source <= Value,
            _ => false,
        };

    /// <summary>
    /// Converts to simple interval.
    /// </summary>
    /// <param name="value">The original value.</param>
    public static explicit operator NullableValueSimpleInterval<int>(Int32Condition value)
    {
        if (value is null) return null;
        if (value.ValueIsNull) return new NullableValueSimpleInterval<int>();
        return new NullableValueSimpleInterval<int>(value.Value.Value, value.Operator switch
        {
            DbCompareOperator.Greater => BasicCompareOperator.Greater,
            DbCompareOperator.GreaterOrEqual => BasicCompareOperator.GreaterOrEqual,
            DbCompareOperator.StartsWith => BasicCompareOperator.GreaterOrEqual,
            DbCompareOperator.Less => BasicCompareOperator.Less,
            DbCompareOperator.LessOrEqual => BasicCompareOperator.LessOrEqual,
            DbCompareOperator.EndsWith => BasicCompareOperator.LessOrEqual,
            DbCompareOperator.NotEqual => BasicCompareOperator.NotEqual,
            _ => BasicCompareOperator.Equal
        });
    }
}

/// <summary>
/// The simple condition class for Int64.
/// </summary>
public class Int64Condition : StructSimpleCondition<long>
{
    /// <summary>
    /// Initializes a new instance of the Int64Condition class.
    /// </summary>
    /// <remarks>You can use this to initialize an instance for the class.</remarks>
    public Int64Condition()
    {
    }

    /// <summary>
    /// Initializes a new instance of the Int64Condition class.
    /// </summary>
    /// <param name="copier">An instance to copy.</param>
    /// <remarks>You can use this to initialize an instance for the class.</remarks>
    public Int64Condition(IStructSimpleCondition<long> copier)
        : base(copier)
    {
    }

    /// <summary>
    /// Initializes a new instance of the Int64Condition class.
    /// </summary>
    /// <param name="op">The operator in the condition.</param>
    /// <param name="value">The value for comparing in the condition.</param>
    /// <remarks>You can use this to initialize an instance for the class.</remarks>
    public Int64Condition(DbCompareOperator op, long value) : base(op, value)
    {
    }

    /// <summary>
    /// Get a condition from the given struct value simple interval left bound.
    /// </summary>
    /// <param name="value">A simple interval.</param>
    /// <returns>A condition from left bound of a specific simple interval.</returns>
    public static Int64Condition CreateFromLeft(StructValueSimpleInterval<long> value)
    {
        if (value == null) throw ObjectConvert.ArgumentNull(nameof(value));
        return new()
        {
            Value = value.MinValue,
            Operator = SimpleCondition.GetLeftOperator(value)
        };
    }

    /// <summary>
    /// Get a condition from the given struct value simple interval left bound.
    /// </summary>
    /// <param name="value">A simple interval.</param>
    /// <returns>A condition from right bound of a specific simple interval.</returns>
    public static Int64Condition CreateFromRight(StructValueSimpleInterval<long> value)
    {
        if (value == null) throw ObjectConvert.ArgumentNull(nameof(value));
        return new()
        {
            Value = value.MaxValue,
            Operator = SimpleCondition.GetRightOperator(value)
        };
    }

    /// <summary>
    /// Get a condition from the given nullable value simple interval left bound.
    /// </summary>
    /// <param name="value">A simple interval.</param>
    /// <returns>A condition from left bound of a specific simple interval.</returns>
    public static Int64Condition CreateFromLeft(NullableValueSimpleInterval<long> value)
    {
        if (value == null) throw ObjectConvert.ArgumentNull(nameof(value));
        if (!value.LeftBounded) return null;
        return new()
        {
            Value = value.MinValue,
            Operator = SimpleCondition.GetLeftOperator(value)
        };
    }

    /// <summary>
    /// Get a condition from the given nullable value simple interval left bound.
    /// </summary>
    /// <param name="value">A simple interval.</param>
    /// <returns>A condition from right bound of a specific simple interval.</returns>
    public static Int64Condition CreateFromRight(NullableValueSimpleInterval<long> value)
    {
        if (value == null) throw ObjectConvert.ArgumentNull(nameof(value));
        if (!value.RightBounded) return null;
        return new()
        {
            Value = value.MaxValue,
            Operator = SimpleCondition.GetRightOperator(value)
        };
    }

    /// <summary>
    /// Checks if a integer condition is valid.
    /// </summary>
    /// <param name="value">The condition object.</param>
    /// <returns>true if it is valid; otherwise, false.</returns>
    public static bool IsValid(Int64Condition value)
        => IsValid(value, SimpleCondition.GetComparableValidOperators().Contains);

    /// <summary>
    /// Gets the type of the value.
    /// </summary>
    public override DbValueType ValueType => DbValueType.Int64;

    /// <summary>
    /// Tests if the given number is macthed.
    /// </summary>
    /// <param name="source">The source to test the condition.</param>
    /// <returns>true if it is matched; otherwise, false.</returns>
    public bool IsMatched(long source)
        => Operator switch
        {
            DbCompareOperator.Equal => source == Value,
            DbCompareOperator.NotEqual => source != Value,
            DbCompareOperator.Greater => source > Value,
            DbCompareOperator.GreaterOrEqual => source >= Value,
            DbCompareOperator.Less => source < Value,
            DbCompareOperator.LessOrEqual => source <= Value,
            _ => false,
        };

    /// <summary>
    /// Tests if the given number is macthed.
    /// </summary>
    /// <param name="source">The source to test the condition.</param>
    /// <returns>true if it is matched; otherwise, false.</returns>
    public bool IsMatched(int source)
        => Operator switch
        {
            DbCompareOperator.Equal => source == Value,
            DbCompareOperator.NotEqual => source != Value,
            DbCompareOperator.Greater => source > Value,
            DbCompareOperator.GreaterOrEqual => source >= Value,
            DbCompareOperator.Less => source < Value,
            DbCompareOperator.LessOrEqual => source <= Value,
            _ => false,
        };

    /// <summary>
    /// Converts to simple interval.
    /// </summary>
    /// <param name="value">The original value.</param>
    public static explicit operator NullableValueSimpleInterval<long>(Int64Condition value)
    {
        if (value is null) return null;
        if (value.ValueIsNull) return new NullableValueSimpleInterval<long>();
        return new NullableValueSimpleInterval<long>(value.Value.Value, value.Operator switch
        {
            DbCompareOperator.Greater => BasicCompareOperator.Greater,
            DbCompareOperator.GreaterOrEqual => BasicCompareOperator.GreaterOrEqual,
            DbCompareOperator.StartsWith => BasicCompareOperator.GreaterOrEqual,
            DbCompareOperator.Less => BasicCompareOperator.Less,
            DbCompareOperator.LessOrEqual => BasicCompareOperator.LessOrEqual,
            DbCompareOperator.EndsWith => BasicCompareOperator.LessOrEqual,
            DbCompareOperator.NotEqual => BasicCompareOperator.NotEqual,
            _ => BasicCompareOperator.Equal
        });
    }
}

/// <summary>
/// The simple condition class for single float.
/// </summary>
public class SingleCondition : StructSimpleCondition<float>
{
    /// <summary>
    /// Initializes a new instance of the SingleCondition class.
    /// </summary>
    /// <remarks>You can use this to initialize an instance for the class.</remarks>
    public SingleCondition()
    {
    }

    /// <summary>
    /// Initializes a new instance of the SingleCondition class.
    /// </summary>
    /// <param name="copier">An instance to copy.</param>
    /// <remarks>You can use this to initialize an instance for the class.</remarks>
    public SingleCondition(IStructSimpleCondition<float> copier)
        : base(copier)
    {
    }

    /// <summary>
    /// Initializes a new instance of the SingleCondition class.
    /// </summary>
    /// <param name="op">The operator in the condition.</param>
    /// <param name="value">The value for comparing in the condition.</param>
    /// <remarks>You can use this to initialize an instance for the class.</remarks>
    public SingleCondition(DbCompareOperator op, float value) : base(op, value)
    {
    }

    /// <summary>
    /// Get a condition from the given struct value simple interval left bound.
    /// </summary>
    /// <param name="value">A simple interval.</param>
    /// <returns>A condition from left bound of a specific simple interval.</returns>
    public static SingleCondition CreateFromLeft(StructValueSimpleInterval<float> value)
    {
        if (value == null) throw ObjectConvert.ArgumentNull(nameof(value));
        return new()
        {
            Value = value.MinValue,
            Operator = SimpleCondition.GetLeftOperator(value)
        };
    }

    /// <summary>
    /// Get a condition from the given struct value simple interval left bound.
    /// </summary>
    /// <param name="value">A simple interval.</param>
    /// <returns>A condition from right bound of a specific simple interval.</returns>
    public static SingleCondition CreateFromRight(StructValueSimpleInterval<float> value)
    {
        if (value == null) throw ObjectConvert.ArgumentNull(nameof(value));
        return new()
        {
            Value = value.MaxValue,
            Operator = SimpleCondition.GetRightOperator(value)
        };
    }

    /// <summary>
    /// Get a condition from the given nullable value simple interval left bound.
    /// </summary>
    /// <param name="value">A simple interval.</param>
    /// <returns>A condition from left bound of a specific simple interval.</returns>
    public static SingleCondition CreateFromLeft(NullableValueSimpleInterval<float> value)
    {
        if (value == null) throw ObjectConvert.ArgumentNull(nameof(value));
        if (!value.LeftBounded) return null;
        return new()
        {
            Value = value.MinValue,
            Operator = SimpleCondition.GetLeftOperator(value)
        };
    }

    /// <summary>
    /// Get a condition from the given nullable value simple interval left bound.
    /// </summary>
    /// <param name="value">A simple interval.</param>
    /// <returns>A condition from right bound of a specific simple interval.</returns>
    public static SingleCondition CreateFromRight(NullableValueSimpleInterval<float> value)
    {
        if (value == null) throw ObjectConvert.ArgumentNull(nameof(value));
        if (!value.RightBounded) return null;
        return new()
        {
            Value = value.MaxValue,
            Operator = SimpleCondition.GetRightOperator(value)
        };
    }

    /// <summary>
    /// Checks if a single float value condition is valid.
    /// </summary>
    /// <param name="value">The condition object.</param>
    /// <returns>true if it is valid; otherwise, false.</returns>
    public static bool IsValid(SingleCondition value)
        => IsValid(value, SimpleCondition.GetComparableValidOperators().Contains);

    /// <summary>
    /// Gets the type of the value.
    /// </summary>
    public override DbValueType ValueType => DbValueType.SingleFloating;

    /// <summary>
    /// Tests if the given number is macthed.
    /// </summary>
    /// <param name="source">The source to test the condition.</param>
    /// <returns>true if it is matched; otherwise, false.</returns>
    public bool IsMatched(float source)
        => Operator switch
        {
            DbCompareOperator.Equal => source == Value,
            DbCompareOperator.NotEqual => source != Value,
            DbCompareOperator.Greater => source > Value,
            DbCompareOperator.GreaterOrEqual => source >= Value,
            DbCompareOperator.Less => source < Value,
            DbCompareOperator.LessOrEqual => source <= Value,
            _ => false,
        };

    /// <summary>
    /// Converts to simple interval.
    /// </summary>
    /// <param name="value">The original value.</param>
    public static explicit operator StructValueSimpleInterval<float>(SingleCondition value)
    {
        if (value is null) return null;
        if (value.ValueIsNull) return new StructValueSimpleInterval<float>(float.NegativeInfinity, float.PositiveInfinity, true, true, float.NegativeInfinity, float.PositiveInfinity);
        return new StructValueSimpleInterval<float>(value.Value.Value, value.Operator switch
        {
            DbCompareOperator.Greater => BasicCompareOperator.Greater,
            DbCompareOperator.GreaterOrEqual => BasicCompareOperator.GreaterOrEqual,
            DbCompareOperator.StartsWith => BasicCompareOperator.GreaterOrEqual,
            DbCompareOperator.Less => BasicCompareOperator.Less,
            DbCompareOperator.LessOrEqual => BasicCompareOperator.LessOrEqual,
            DbCompareOperator.EndsWith => BasicCompareOperator.LessOrEqual,
            DbCompareOperator.NotEqual => BasicCompareOperator.NotEqual,
            _ => BasicCompareOperator.Equal
        }, float.NegativeInfinity, float.PositiveInfinity);
    }
}

/// <summary>
/// The simple condition class for double float.
/// </summary>
public class DoubleCondition : StructSimpleCondition<double>
{
    /// <summary>
    /// Initializes a new instance of the DoubleCondition class.
    /// </summary>
    /// <remarks>You can use this to initialize an instance for the class.</remarks>
    public DoubleCondition()
    {
    }

    /// <summary>
    /// Initializes a new instance of the DoubleCondition class.
    /// </summary>
    /// <param name="copier">An instance to copy.</param>
    /// <remarks>You can use this to initialize an instance for the class.</remarks>
    public DoubleCondition(IStructSimpleCondition<double> copier)
        : base(copier)
    {
    }

    /// <summary>
    /// Initializes a new instance of the DoubleCondition class.
    /// </summary>
    /// <param name="op">The operator in the condition.</param>
    /// <param name="value">The value for comparing in the condition.</param>
    /// <remarks>You can use this to initialize an instance for the class.</remarks>
    public DoubleCondition(DbCompareOperator op, double value) : base(op, value)
    {
    }

    /// <summary>
    /// Get a condition from the given struct value simple interval left bound.
    /// </summary>
    /// <param name="value">A simple interval.</param>
    /// <returns>A condition from left bound of a specific simple interval.</returns>
    public static DoubleCondition CreateFromLeft(StructValueSimpleInterval<double> value)
    {
        if (value == null) throw ObjectConvert.ArgumentNull(nameof(value));
        return new()
        {
            Value = value.MinValue,
            Operator = SimpleCondition.GetLeftOperator(value)
        };
    }

    /// <summary>
    /// Get a condition from the given struct value simple interval left bound.
    /// </summary>
    /// <param name="value">A simple interval.</param>
    /// <returns>A condition from right bound of a specific simple interval.</returns>
    public static DoubleCondition CreateFromRight(StructValueSimpleInterval<double> value)
    {
        if (value == null) throw ObjectConvert.ArgumentNull(nameof(value));
        return new()
        {
            Value = value.MaxValue,
            Operator = SimpleCondition.GetRightOperator(value)
        };
    }

    /// <summary>
    /// Get a condition from the given nullable value simple interval left bound.
    /// </summary>
    /// <param name="value">A simple interval.</param>
    /// <returns>A condition from left bound of a specific simple interval.</returns>
    public static DoubleCondition CreateFromLeft(NullableValueSimpleInterval<double> value)
    {
        if (value == null) throw ObjectConvert.ArgumentNull(nameof(value));
        if (!value.LeftBounded) return null;
        return new()
        {
            Value = value.MinValue,
            Operator = SimpleCondition.GetLeftOperator(value)
        };
    }

    /// <summary>
    /// Get a condition from the given nullable value simple interval left bound.
    /// </summary>
    /// <param name="value">A simple interval.</param>
    /// <returns>A condition from right bound of a specific simple interval.</returns>
    public static DoubleCondition CreateFromRight(NullableValueSimpleInterval<double> value)
    {
        if (value == null) throw ObjectConvert.ArgumentNull(nameof(value));
        if (!value.RightBounded) return null;
        return new()
        {
            Value = value.MaxValue,
            Operator = SimpleCondition.GetRightOperator(value)
        };
    }

    /// <summary>
    /// Checks if a double float value condition is valid.
    /// </summary>
    /// <param name="value">The condition object.</param>
    /// <returns>true if it is valid; otherwise, false.</returns>
    public static bool IsValid(DoubleCondition value)
        => IsValid(value, SimpleCondition.GetComparableValidOperators().Contains);

    /// <summary>
    /// Gets the type of the value.
    /// </summary>
    public override DbValueType ValueType => DbValueType.DoubleFloating;

    /// <summary>
    /// Tests if the given number is macthed.
    /// </summary>
    /// <param name="source">The source to test the condition.</param>
    /// <returns>true if it is matched; otherwise, false.</returns>
    public bool IsMatched(double source)
        => Operator switch
        {
            DbCompareOperator.Equal => source == Value,
            DbCompareOperator.NotEqual => source != Value,
            DbCompareOperator.Greater => source > Value,
            DbCompareOperator.GreaterOrEqual => source >= Value,
            DbCompareOperator.Less => source < Value,
            DbCompareOperator.LessOrEqual => source <= Value,
            _ => false,
        };

    /// <summary>
    /// Converts to simple interval.
    /// </summary>
    /// <param name="value">The original value.</param>
    public static explicit operator StructValueSimpleInterval<double>(DoubleCondition value)
    {
        if (value is null) return null;
        if (value.ValueIsNull) return new StructValueSimpleInterval<double>(double.NegativeInfinity, double.PositiveInfinity, true, true, double.NegativeInfinity, double.PositiveInfinity);
        return new StructValueSimpleInterval<double>(value.Value.Value, value.Operator switch
        {
            DbCompareOperator.Greater => BasicCompareOperator.Greater,
            DbCompareOperator.GreaterOrEqual => BasicCompareOperator.GreaterOrEqual,
            DbCompareOperator.StartsWith => BasicCompareOperator.GreaterOrEqual,
            DbCompareOperator.Less => BasicCompareOperator.Less,
            DbCompareOperator.LessOrEqual => BasicCompareOperator.LessOrEqual,
            DbCompareOperator.EndsWith => BasicCompareOperator.LessOrEqual,
            DbCompareOperator.NotEqual => BasicCompareOperator.NotEqual,
            _ => BasicCompareOperator.Equal
        }, double.NegativeInfinity, double.PositiveInfinity);
    }
}

/// <summary>
/// The simple condition class for decimal float.
/// </summary>
public class DecimalCondition : StructSimpleCondition<decimal>
{
    /// <summary>
    /// Initializes a new instance of the DecimalCondition class.
    /// </summary>
    /// <remarks>You can use this to initialize an instance for the class.</remarks>
    public DecimalCondition()
    {
    }

    /// <summary>
    /// Initializes a new instance of the DecimalCondition class.
    /// </summary>
    /// <param name="copier">An instance to copy.</param>
    /// <remarks>You can use this to initialize an instance for the class.</remarks>
    public DecimalCondition(IStructSimpleCondition<decimal> copier)
        : base(copier)
    {
    }

    /// <summary>
    /// Initializes a new instance of the DecimalCondition class.
    /// </summary>
    /// <param name="op">The operator in the condition.</param>
    /// <param name="value">The value for comparing in the condition.</param>
    /// <remarks>You can use this to initialize an instance for the class.</remarks>
    public DecimalCondition(DbCompareOperator op, decimal value) : base(op, value)
    {
    }

    /// <summary>
    /// Get a condition from the given struct value simple interval left bound.
    /// </summary>
    /// <param name="value">A simple interval.</param>
    /// <returns>A condition from left bound of a specific simple interval.</returns>
    public static DecimalCondition CreateFromLeft(StructValueSimpleInterval<decimal> value)
    {
        if (value == null) throw ObjectConvert.ArgumentNull(nameof(value));
        return new()
        {
            Value = value.MinValue,
            Operator = SimpleCondition.GetLeftOperator(value)
        };
    }

    /// <summary>
    /// Get a condition from the given struct value simple interval left bound.
    /// </summary>
    /// <param name="value">A simple interval.</param>
    /// <returns>A condition from right bound of a specific simple interval.</returns>
    public static DecimalCondition CreateFromRight(StructValueSimpleInterval<decimal> value)
    {
        if (value == null) throw ObjectConvert.ArgumentNull(nameof(value));
        return new()
        {
            Value = value.MaxValue,
            Operator = SimpleCondition.GetRightOperator(value)
        };
    }

    /// <summary>
    /// Get a condition from the given nullable value simple interval left bound.
    /// </summary>
    /// <param name="value">A simple interval.</param>
    /// <returns>A condition from left bound of a specific simple interval.</returns>
    public static DecimalCondition CreateFromLeft(NullableValueSimpleInterval<decimal> value)
    {
        if (value == null) throw ObjectConvert.ArgumentNull(nameof(value));
        if (!value.LeftBounded) return null;
        return new()
        {
            Value = value.MinValue,
            Operator = SimpleCondition.GetLeftOperator(value)
        };
    }

    /// <summary>
    /// Get a condition from the given nullable value simple interval left bound.
    /// </summary>
    /// <param name="value">A simple interval.</param>
    /// <returns>A condition from right bound of a specific simple interval.</returns>
    public static DecimalCondition CreateFromRight(NullableValueSimpleInterval<decimal> value)
    {
        if (value == null) throw ObjectConvert.ArgumentNull(nameof(value));
        if (!value.RightBounded) return null;
        return new()
        {
            Value = value.MaxValue,
            Operator = SimpleCondition.GetRightOperator(value)
        };
    }

    /// <summary>
    /// Checks if a decimal float value condition is valid.
    /// </summary>
    /// <param name="value">The condition object.</param>
    /// <returns>true if it is valid; otherwise, false.</returns>
    public static bool IsValid(DecimalCondition value)
        => IsValid(value, SimpleCondition.GetComparableValidOperators().Contains);

    /// <summary>
    /// Gets the type of the value.
    /// </summary>
    public override DbValueType ValueType => DbValueType.DoubleFloating;

    /// <summary>
    /// Tests if the given number is macthed.
    /// </summary>
    /// <param name="source">The source to test the condition.</param>
    /// <returns>true if it is matched; otherwise, false.</returns>
    public bool IsMatched(decimal source)
        => Operator switch
        {
            DbCompareOperator.Equal => source == Value,
            DbCompareOperator.NotEqual => source != Value,
            DbCompareOperator.Greater => source > Value,
            DbCompareOperator.GreaterOrEqual => source >= Value,
            DbCompareOperator.Less => source < Value,
            DbCompareOperator.LessOrEqual => source <= Value,
            _ => false,
        };

    /// <summary>
    /// Converts to simple interval.
    /// </summary>
    /// <param name="value">The original value.</param>
    public static explicit operator StructValueSimpleInterval<decimal>(DecimalCondition value)
    {
        if (value is null) return null;
        if (value.ValueIsNull) return new StructValueSimpleInterval<decimal>(decimal.MinValue, decimal.MaxValue, false, false, null, null);
        return new(value.Value.Value, value.Operator switch
        {
            DbCompareOperator.Greater => BasicCompareOperator.Greater,
            DbCompareOperator.GreaterOrEqual => BasicCompareOperator.GreaterOrEqual,
            DbCompareOperator.StartsWith => BasicCompareOperator.GreaterOrEqual,
            DbCompareOperator.Less => BasicCompareOperator.Less,
            DbCompareOperator.LessOrEqual => BasicCompareOperator.LessOrEqual,
            DbCompareOperator.EndsWith => BasicCompareOperator.LessOrEqual,
            DbCompareOperator.NotEqual => BasicCompareOperator.NotEqual,
            _ => BasicCompareOperator.Equal
        }, decimal.MinValue, decimal.MaxValue);
    }
}

/// <summary>
/// The simple condition class for date time.
/// </summary>
public class DateTimeCondition : StructSimpleCondition<DateTime>
{
    /// <summary>
    /// Initializes a new instance of the DateTimeCondition class.
    /// </summary>
    /// <remarks>You can use this to initialize an instance for the class.</remarks>
    public DateTimeCondition()
    {
    }

    /// <summary>
    /// Initializes a new instance of the DateTimeCondition class.
    /// </summary>
    /// <param name="copier">An instance to copy.</param>
    /// <remarks>You can use this to initialize an instance for the class.</remarks>
    public DateTimeCondition(IStructSimpleCondition<DateTime> copier)
        : base(copier)
    {
    }

    /// <summary>
    /// Get a condition from the given struct value simple interval left bound.
    /// </summary>
    /// <param name="value">A simple interval.</param>
    /// <returns>A condition from left bound of a specific simple interval.</returns>
    public static DateTimeCondition CreateFromLeft(StructValueSimpleInterval<DateTime> value)
    {
        if (value == null) throw ObjectConvert.ArgumentNull(nameof(value));
        return new DateTimeCondition
        {
            Value = value.MinValue,
            Operator = SimpleCondition.GetLeftOperator(value)
        };
    }

    /// <summary>
    /// Get a condition from the given struct value simple interval left bound.
    /// </summary>
    /// <param name="value">A simple interval.</param>
    /// <returns>A condition from right bound of a specific simple interval.</returns>
    public static DateTimeCondition CreateFromRight(StructValueSimpleInterval<DateTime> value)
    {
        if (value == null) throw ObjectConvert.ArgumentNull(nameof(value));
        return new DateTimeCondition
        {
            Value = value.MaxValue,
            Operator = SimpleCondition.GetRightOperator(value)
        };
    }

    /// <summary>
    /// Get a condition from the given nullable value simple interval left bound.
    /// </summary>
    /// <param name="value">A simple interval.</param>
    /// <returns>A condition from left bound of a specific simple interval.</returns>
    public static DateTimeCondition CreateFromLeft(NullableValueSimpleInterval<DateTime> value)
    {
        if (value == null) throw ObjectConvert.ArgumentNull(nameof(value));
        if (!value.LeftBounded) return null;
        return new DateTimeCondition
        {
            Value = value.MinValue,
            Operator = SimpleCondition.GetLeftOperator(value)
        };
    }

    /// <summary>
    /// Get a condition from the given nullable value simple interval left bound.
    /// </summary>
    /// <param name="value">A simple interval.</param>
    /// <returns>A condition from right bound of a specific simple interval.</returns>
    public static DateTimeCondition CreateFromRight(NullableValueSimpleInterval<DateTime> value)
    {
        if (value == null) throw ObjectConvert.ArgumentNull(nameof(value));
        if (!value.RightBounded) return null;
        return new DateTimeCondition
        {
            Value = value.MaxValue,
            Operator = SimpleCondition.GetRightOperator(value)
        };
    }

    /// <summary>
    /// Checks if a date time condition is valid.
    /// </summary>
    /// <param name="value">The condition object.</param>
    /// <returns>true if it is valid; otherwise, false.</returns>
    public static bool IsValid(DateTimeCondition value)
        => IsValid(value, SimpleCondition.GetComparableValidOperators().Contains);

    /// <summary>
    /// Gets the type of the value.
    /// </summary>
    public override DbValueType ValueType => DbValueType.DateTimeUtc;

    /// <summary>
    /// Tests if the given date time is macthed.
    /// </summary>
    /// <param name="source">The source to test the condition.</param>
    /// <returns>true if it is matched; otherwise, false.</returns>
    public bool IsMatched(DateTime source)
        => Operator switch
        {
            DbCompareOperator.Equal => source == Value,
            DbCompareOperator.NotEqual => source != Value,
            DbCompareOperator.Greater => source > Value,
            DbCompareOperator.GreaterOrEqual => source >= Value,
            DbCompareOperator.Less => source < Value,
            DbCompareOperator.LessOrEqual => source <= Value,
            _ => false,
        };
}

/// <summary>
/// The simple condition class for boolean.
/// </summary>
public class BooleanCondition : StructSimpleCondition<bool>
{
    /// <summary>
    /// Initializes a new instance of the BooleanCondition class.
    /// </summary>
    /// <remarks>You can use this to initialize an instance for the class.</remarks>
    public BooleanCondition()
    {
    }

    /// <summary>
    /// Initializes a new instance of the BooleanCondition class.
    /// </summary>
    /// <param name="copier">An instance to copy.</param>
    /// <remarks>You can use this to initialize an instance for the class.</remarks>
    public BooleanCondition(IStructSimpleCondition<bool> copier)
        : base(copier)
    {
    }

    /// <summary>
    /// Checks if a boolean value condition is valid.
    /// </summary>
    /// <param name="value">The condition object.</param>
    /// <returns>true if it is valid; otherwise, false.</returns>
    public static bool IsValid(BooleanCondition value)
        => IsValid(value, SimpleCondition.GetBasicValidOperators().Contains);

    /// <summary>
    /// Gets the type of the value.
    /// </summary>
    public override DbValueType ValueType => DbValueType.Boolean;

    /// <summary>
    /// Tests if the given boolean is macthed.
    /// </summary>
    /// <param name="source">The source to test the condition.</param>
    /// <returns>true if it is matched; otherwise, false.</returns>
    public bool IsMatched(bool source)
        => Operator switch
        {
            DbCompareOperator.Equal => source == Value,
            DbCompareOperator.NotEqual => source != Value,
            _ => false,
        };
}
