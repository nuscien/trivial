// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Interval.cs" company="Nanchang Jinchen Software Co., Ltd.">
//   Copyright (c) 2010 Nanchang Jinchen Software Co., Ltd. All rights reserved.
// </copyright>
// <summary>
//   The interval object.
// </summary>
// <author>Kingcean Tuan</author>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Trivial.Maths;

/// <summary>
/// The interface for simple closed interval.
/// </summary>
/// <typeparam name="T">The type of the object in interval.</typeparam>
public interface ISimpleInterval<T>
{
    /// <summary>
    /// Gets a value indicating whether it is left bounded.
    /// </summary>
    bool LeftBounded { get; }

    /// <summary>
    /// Gets a value indicating whether it is right bounded.
    /// </summary>
    bool RightBounded { get; }

    /// <summary>
    /// Gets or sets a value indicating whether it is left open.
    /// </summary>
    bool LeftOpen { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether it is right open.
    /// </summary>
    bool RightOpen { get; set; }

    /// <summary>
    /// Gets or sets the minimum value of the interval.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">The value should be less than MaxValue.</exception>
    T MinValue { get; set; }

    /// <summary>
    /// Gets or sets the maximum value of the interval.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">The value should be greater than MinValue.</exception>
    T MaxValue { get; set; }

    /// <summary>
    /// Checks if the value is less than MinValue.
    /// </summary>
    /// <param name="value">A value to compare with MinValue.</param>
    /// <returns>true if the specific value is less than or equal MinValue; otherwise, false.</returns>
    bool IsLessThanMinValue(T value);

    /// <summary>
    /// Checks if the value is greater than MinValue.
    /// </summary>
    /// <param name="value">A value to compare with MinValue.</param>
    /// <returns>true if the specific value is greater than or equal MinValue; otherwise, false.</returns>
    bool IsGreaterThanMinValue(T value);

    /// <summary>
    /// Checks if the value equals MinValue.
    /// </summary>
    /// <param name="value">A value to compare with MinValue.</param>
    /// <returns>true if the specific value is greater than or equal MinValue; otherwise, false.</returns>
    bool EqualsMinValue(T value);

    /// <summary>
    /// Checks if the value is less than MaxValue.
    /// </summary>
    /// <param name="value">A value to compare with MaxValue.</param>
    /// <returns>true if the specific value is less than or equal MaxValue; otherwise, false.</returns>
    bool IsLessThanMaxValue(T value);

    /// <summary>
    /// Checks if the value is greater than MaxValue.
    /// </summary>
    /// <param name="value">A value to compare with MaxValue.</param>
    /// <returns>true if the specific value is greater than or equal MaxValue; otherwise, false.</returns>
    bool IsGreaterThanMaxValue(T value);

    /// <summary>
    /// Checks if the value equals MaxValue.
    /// </summary>
    /// <param name="value">A value to compare with MaxValue.</param>
    /// <returns>true if the specific value is greater than or equal MaxValue; otherwise, false.</returns>
    bool EqualsMaxValue(T value);

    /// <summary>
    /// Converts the interval to a tuple, which includes MinValue and MaxValue.
    /// </summary>
    /// <returns>A tuple including MinValue and MaxValue.</returns>
    Tuple<T, T> ToTuple();

    /// <summary>
    /// Converts the interval to a list, which includes MinValue and MaxValue.
    /// </summary>
    /// <returns>A list including MinValue and MaxValue.</returns>
    IList<T> ToList();
}

/// <summary>
/// The simple closed interval base class.
/// </summary>
[DataContract]
public abstract class SimpleInterval<T> : ISimpleInterval<T>
{
    /// <summary>
    /// The minimum value of the interval in memory.
    /// </summary>
    private T _minValue;

    /// <summary>
    /// The maximum value of the interval in memory.
    /// </summary>
    private T _maxValue;

    /// <summary>
    /// The value indicating whether it is left open.
    /// </summary>
    private bool _leftOpen;

    /// <summary>
    /// The value indicating whether it is right open.
    /// </summary>
    private bool _rightOpen;

    /// <summary>
    /// Initializes a new instance of the SimpleInterval class.
    /// </summary>
    protected SimpleInterval()
    {
        LeftOpen = true;
        RightOpen = true;
    }

    /// <summary>
    /// Initializes a new instance of the SimpleInterval class.
    /// </summary>
    /// <param name="value">The value to set.</param>
    /// <param name="op">The operation.</param>
    /// <param name="minValue">The minimum value.</param>
    /// <param name="maxValue">The maximum value.</param>
    protected SimpleInterval(T value, BasicCompareOperator op, T minValue, T maxValue)
    {
        switch (op)
        {
            case BasicCompareOperator.Greater:
                MinValue = value;
                MaxValue = maxValue;
                LeftOpen = true;
                RightOpen = true;
                break;
            case BasicCompareOperator.GreaterOrEqual:
                MinValue = value;
                MaxValue = maxValue;
                LeftOpen = false;
                RightOpen = true;
                break;
            case BasicCompareOperator.Less:
                MinValue = minValue;
                MaxValue = value;
                LeftOpen = true;
                RightOpen = false;
                break;
            case BasicCompareOperator.LessOrEqual:
                MinValue = minValue;
                MaxValue = value;
                LeftOpen = true;
                RightOpen = false;
                break;
            case BasicCompareOperator.NotEqual:
                MinValue = maxValue;
                MaxValue = minValue;
                LeftOpen = false;
                RightOpen = false;
                break;
            default:
                MinValue = value;
                MaxValue = value;
                LeftOpen = false;
                RightOpen = false;
                break;
        }
    }

    /// <summary>
    /// Initializes a new instance of the SimpleInterval class.
    /// </summary>
    /// <param name="value">The value to set.</param>
    /// <param name="op">The operation.</param>
    protected SimpleInterval(T value, BasicCompareOperator op) : this(value, op, default, default)
    {
    }

    /// <summary>
    /// Initializes a new instance of the SimpleInterval class.
    /// </summary>
    /// <param name="valueA">The first value.</param>
    /// <param name="valueB">The seconde value.</param>
    protected SimpleInterval(T valueA, T valueB) : this(valueA, valueB, false)
    {
    }

    /// <summary>
    /// Initializes a new instance of the SimpleInterval class.
    /// </summary>
    /// <param name="valueA">The first value.</param>
    /// <param name="valueB">The seconde value.</param>
    /// <param name="autoFix">true if fix the order automatically; otherwise, false.</param>
    protected SimpleInterval(T valueA, T valueB, bool autoFix)
    {
        if (autoFix)
        {
            this.SetValues(valueA, valueB);
        }
        else
        {
            MinValue = valueA;
            MaxValue = valueB;
        }

        LeftOpen = valueA is null;
        RightOpen = valueB is null;
    }

    /// <summary>
    /// Initializes a new instance of the SimpleInterval class.
    /// </summary>
    /// <param name="valueA">The first value.</param>
    /// <param name="valueB">The seconde value.</param>
    /// <param name="leftOpen">true if it is left open; otherwise, left closed.</param>
    /// <param name="rightOpen">true if it is right open; otherwise, right closed.</param>
    protected SimpleInterval(T valueA, T valueB, bool leftOpen, bool rightOpen)
    {
        MinValue = valueA;
        MaxValue = valueB;
        LeftOpen = leftOpen;
        RightOpen = rightOpen;
    }

    /// <summary>
    /// Occurs on minimum (left) value is changed.
    /// </summary>
    public event Data.ChangeEventHandler<T> MinValueChanged;

    /// <summary>
    /// Occurs on maximum (right) value is changed.
    /// </summary>
    public event Data.ChangeEventHandler<T> MaxValueChanged;

    /// <summary>
    /// Gets a value indicating whether it is left bounded.
    /// </summary>
    [JsonIgnore]
    public abstract bool LeftBounded { get; }

    /// <summary>
    /// Gets a value indicating whether it is right bounded.
    /// </summary>
    [JsonIgnore]
    public abstract bool RightBounded { get; }

    /// <summary>
    /// Gets or sets a value indicating whether it is left open.
    /// </summary>
    [JsonPropertyName("leftopen")]
    [DataMember(Name = "leftopen")]
    public bool LeftOpen
    { 
        get
        {
            return _leftOpen || !LeftBounded;
        }

        set
        {
            _leftOpen = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether it is right open.
    /// </summary>
    [JsonPropertyName("rightopen")]
    [DataMember(Name = "rightopen")]
    public bool RightOpen {
        get
        {
            return _rightOpen || !RightBounded;
        }
        
        set
        {
            _rightOpen = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether throw an exception if the value is not valid.
    /// </summary>
    [JsonIgnore]
    public bool ValidateValueSetting { get; set; }

    /// <summary>
    /// Gets or sets the minimum (left) value of the interval.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">The value should be less than MaxValue.</exception>
    [JsonPropertyName("left")]
    [DataMember(Name = "left")]
    public T MinValue
    {
        get
        {
            return _minValue;
        }

        set
        {
            if (value is null && _minValue is null) return;
            if (ValidateValueSetting && IsGreaterThanMinValue(value) && IsGreaterThanMaxValue(value) && RightBounded)
                throw new ArgumentOutOfRangeException("value", "The value should be less than MaxValue.");
            var oldValue = _minValue;
            _minValue = value;
            MinValueChanged?.Invoke(this, new Data.ChangeEventArgs<T>(oldValue, value, Data.ChangeMethods.Update, nameof(MinValue)));
        }
    }

    /// <summary>
    /// Gets or sets the maximum (right) value of the interval.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">The value should be greater than MinValue.</exception>
    [JsonPropertyName("right")]
    [DataMember(Name = "right")]
    public T MaxValue
    {
        get
        {
            return _maxValue;
        }

        set
        {
            if (value is null && _maxValue is null) return;
            if (ValidateValueSetting && IsLessThanMaxValue(value) && IsLessThanMinValue(value) && LeftBounded)
                throw new ArgumentOutOfRangeException("value", "The value should be greater than MinValue.");
            var oldValue = _minValue;
            _maxValue = value;
            MaxValueChanged?.Invoke(this, new Data.ChangeEventArgs<T>(oldValue, value, Data.ChangeMethods.Update, nameof(MaxValue)));
        }
    }

    /// <summary>
    /// Checks if the value is less than MinValue.
    /// </summary>
    /// <param name="value">A value to compare with MinValue.</param>
    /// <returns>true if the specific value is less than or equal MinValue; otherwise, false.</returns>
    public abstract bool IsLessThanMinValue(T value);

    /// <summary>
    /// Checks if the value is greater than MinValue.
    /// </summary>
    /// <param name="value">A value to compare with MinValue.</param>
    /// <returns>true if the specific value is greater than or equal MinValue; otherwise, false.</returns>
    public abstract bool IsGreaterThanMinValue(T value);

    /// <summary>
    /// Checks if the value equals MinValue.
    /// </summary>
    /// <param name="value">A value to compare with MinValue.</param>
    /// <returns>true if the specific value is greater than or equal MinValue; otherwise, false.</returns>
    public abstract bool EqualsMinValue(T value);

    /// <summary>
    /// Checks if the value is less than MaxValue.
    /// </summary>
    /// <param name="value">A value to compare with MaxValue.</param>
    /// <returns>true if the specific value is less than or equal MaxValue; otherwise, false.</returns>
    public abstract bool IsLessThanMaxValue(T value);

    /// <summary>
    /// Checks if the value is greater than MaxValue.
    /// </summary>
    /// <param name="value">A value to compare with MaxValue.</param>
    /// <returns>true if the specific value is greater than or equal MaxValue; otherwise, false.</returns>
    public abstract bool IsGreaterThanMaxValue(T value);

    /// <summary>
    /// Checks if the value equals MaxValue.
    /// </summary>
    /// <param name="value">A value to compare with MaxValue.</param>
    /// <returns>true if the specific value is greater than or equal MaxValue; otherwise, false.</returns>
    public abstract bool EqualsMaxValue(T value);

    /// <summary>
    /// Tests a value if is in the interval.
    /// </summary>
    /// <param name="value">The value to test.</param>
    /// <returns>true if the test value is in the interval; otherwise, false.</returns>
    public bool Contains(T value)
    {
        var compareLeft = LeftOpen ? IsGreaterThanMinValue(value) : IntervalUtility.IsGreaterThanOrEqualMinValue(this, value);
        if (!compareLeft) return false;
        return RightOpen ? IsLessThanMaxValue(value) : IntervalUtility.IsLessThanOrEqualMaxValue(this, value);
    }

    /// <summary>
    /// Converts the interval to a tuple, which includes MinValue and MaxValue.
    /// </summary>
    /// <returns>A tuple including MinValue and MaxValue.</returns>
    public Tuple<T, T> ToTuple()
        => new(MinValue, MaxValue);

    /// <summary>
    /// Converts the interval to a tuple, which includes MinValue and MaxValue.
    /// </summary>
    /// <returns>A tuple including MinValue and MaxValue.</returns>
    public ValueTuple<T, T> ToValueTuple()
        => new(MinValue, MaxValue);

    /// <summary>
    /// Converts the interval to a two elements object, which includes MinValue and MaxValue.
    /// </summary>
    /// <returns>A multiple elements object including MinValue and MaxValue.</returns>
    public TwoElements<T> ToTwoElements()
        => new() { ItemA = MinValue, ItemB = MaxValue };

    /// <summary>
    /// Converts the interval to a list, which includes MinValue and MaxValue.
    /// </summary>
    /// <returns>A list including MinValue and MaxValue.</returns>
    public IList<T> ToList()
        => new List<T> { MinValue, MaxValue };

    /// <summary>
    /// Returns the interval string value of this instance.
    /// </summary>
    /// <returns>A System.String containing this interval.</returns>
    public override string ToString()
    {
        var leftStr = LeftBounded ? ToValueString(MinValue) : Numbers.NegativeInfiniteSymbol;
        var rightStr = RightBounded ? ToValueString(MaxValue) : Numbers.PositiveInfiniteSymbol;
        var longStr = string.Format("{0} - {1}", leftStr, rightStr);
        var sep = false;
        if (longStr.IndexOfAny(new[] { ',', ';' }) > -1) sep = true;
        if (!sep && longStr.IndexOf(';') > -1)
        {
            const string quoteStr = "\"{0}\"";
            leftStr = string.Format(quoteStr, leftStr.Replace("\"", "\\\""));
            rightStr = string.Format(quoteStr, rightStr.Replace("\"", "\\\""));
        }

        return string.Format("{0}{1}{2} {3}{4}", LeftOpen ? "(" : "[", leftStr, sep ? ";" : ",", rightStr, RightOpen ? ")" : "]");
    }

    /// <summary>
    /// Gets the string format of the value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>A string.</returns>
    protected virtual string ToValueString(T value)
    {
        if (value is null) return string.Empty;
        return value.ToString();
    }
}

/// <summary>
/// The simple interval with class value.
/// </summary>
[DataContract]
public class RefValueSimpleInterval<T> : SimpleInterval<T> where T : class, IComparable<T>
{
    /// <summary>
    /// Initializes a new instance of the RefValueSimpleInterval class.
    /// </summary>
    public RefValueSimpleInterval()
    {
    }

    /// <summary>
    /// Initializes a new instance of the RefValueSimpleInterval class.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="op">The operation.</param>
    public RefValueSimpleInterval(T value, BasicCompareOperator op)
        : base(value, op, null, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the RefValueSimpleInterval class.
    /// </summary>
    /// <param name="valueA">The first value.</param>
    /// <param name="valueB">The seconde value.</param>
    public RefValueSimpleInterval(T valueA, T valueB)
        : base(valueA, valueB)
    {
    }

    /// <summary>
    /// Initializes a new instance of the RefValueSimpleInterval class.
    /// </summary>
    /// <param name="valueA">The first value.</param>
    /// <param name="valueB">The seconde value.</param>
    /// <param name="autoFix">true if fix the order automatically; otherwise, false.</param>
    public RefValueSimpleInterval(T valueA, T valueB, bool autoFix)
        : base(valueA, valueB, autoFix)
    {
    }

    /// <summary>
    /// Initializes a new instance of the RefValueSimpleInterval class.
    /// </summary>
    /// <param name="valueA">The first value.</param>
    /// <param name="valueB">The seconde value.</param>
    /// <param name="leftOpen">true if it is left open; otherwise, left closed.</param>
    /// <param name="rightOpen">true if it is right open; otherwise, right closed.</param>
    public RefValueSimpleInterval(T valueA, T valueB, bool leftOpen, bool rightOpen)
        : base(valueA, valueB, leftOpen, rightOpen)
    {
    }

    /// <summary>
    /// Gets a value indicating whether it is left bounded.
    /// </summary>
    [JsonIgnore]
    public override bool LeftBounded { get { return MinValue != null; } }

    /// <summary>
    /// Gets a value indicating whether it is right bounded.
    /// </summary>
    [JsonIgnore]
    public override bool RightBounded { get { return MaxValue != null; } }

    /// <summary>
    /// Checks if the value is less than MinValue.
    /// </summary>
    /// <param name="value">A value to compare with MinValue.</param>
    /// <returns>true if the specific value is less than or equal MinValue; otherwise, false.</returns>
    public override bool IsLessThanMinValue(T value)
    {
        if (MinValue == null) return false;
        if (value == null) return true;
        return 0 < (ValueComparer?.Compare(MinValue, value) ?? MinValue.CompareTo(value));
    }

    /// <summary>
    /// Checks if the value is greater than MinValue.
    /// </summary>
    /// <param name="value">A value to compare with MinValue.</param>
    /// <returns>true if the specific value is greater than or equal MinValue; otherwise, false.</returns>
    public override bool IsGreaterThanMinValue(T value)
    {
        if (value == null) return false;
        if (MinValue == null) return true;
        return 0 > (ValueComparer?.Compare(MinValue, value) ?? MinValue.CompareTo(value));
    }

    /// <summary>
    /// Checks if the value equals MinValue.
    /// </summary>
    /// <param name="value">A value to compare with MinValue.</param>
    /// <returns>true if the specific value is greater than or equal MinValue; otherwise, false.</returns>
    public override bool EqualsMinValue(T value)
    {
        if (MinValue == null && value == null) return true;
        if (MinValue == null || value == null) return false;
        return 0 == (ValueComparer?.Compare(MinValue, value) ?? MinValue.CompareTo(value));
    }

    /// <summary>
    /// Checks if the value is less than MaxValue.
    /// </summary>
    /// <param name="value">A value to compare with MaxValue.</param>
    /// <returns>true if the specific value is less than or equal MaxValue; otherwise, false.</returns>
    public override bool IsLessThanMaxValue(T value)
    {
        if (value == null) return false;
        if (MaxValue == null) return true;
        return 0 < (ValueComparer?.Compare(MaxValue, value) ?? MaxValue.CompareTo(value));
    }

    /// <summary>
    /// Checks if the value is greater than MaxValue.
    /// </summary>
    /// <param name="value">A value to compare with MaxValue.</param>
    /// <returns>true if the specific value is greater than or equal MaxValue; otherwise, false.</returns>
    public override bool IsGreaterThanMaxValue(T value)
    {
        if (MaxValue == null) return false;
        if (value == null) return true;
        return 0 > (ValueComparer?.Compare(MaxValue, value) ?? MaxValue.CompareTo(value));
    }

    /// <summary>
    /// Checks if the value equals MaxValue.
    /// </summary>
    /// <param name="value">A value to compare with MaxValue.</param>
    /// <returns>true if the specific value equals MaxValue; otherwise, false.</returns>
    public override bool EqualsMaxValue(T value)
    {
        if (MaxValue == null && value == null) return true;
        if (MaxValue == null || value == null) return false;
        return 0 == (ValueComparer?.Compare(MaxValue, value) ?? MaxValue.CompareTo(value));
    }

    /// <summary>
    /// Gets the value comparer.
    /// </summary>
    [JsonIgnore]
    protected virtual IComparer<T> ValueComparer { get; }
}

/// <summary>
/// The simple interval with struct value.
/// </summary>
[DataContract]
public class StructValueSimpleInterval<T> : SimpleInterval<T> where T : struct, IComparable<T>
{
    private readonly T? positiveInfiniteValue;
    private readonly T? negativeInfiniteValue;

    /// <summary>
    /// Initializes a new instance of the StructValueSimpleInterval class.
    /// </summary>
    public StructValueSimpleInterval()
    {
    }

    /// <summary>
    /// Initializes a new instance of the StructValueSimpleInterval class.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="op">The operation.</param>
    /// <param name="minValue">The minimum value.</param>
    /// <param name="maxValue">The maximum value.</param>
    public StructValueSimpleInterval(T value, BasicCompareOperator op, T minValue, T maxValue)
        : base(value, op, minValue, maxValue)
    {
    }

    /// <summary>
    /// Initializes a new instance of the StructValueSimpleInterval class.
    /// </summary>
    /// <param name="valueA">The first value.</param>
    /// <param name="valueB">The seconde value.</param>
    public StructValueSimpleInterval(T valueA, T valueB)
        : base(valueA, valueB)
    {
    }

    /// <summary>
    /// Initializes a new instance of the StructValueSimpleInterval class.
    /// </summary>
    /// <param name="valueA">The first value.</param>
    /// <param name="valueB">The seconde value.</param>
    /// <param name="autoFix">true if fix the order automatically; otherwise, false.</param>
    public StructValueSimpleInterval(T valueA, T valueB, bool autoFix)
        : base(valueA, valueB, autoFix)
    {
    }

    /// <summary>
    /// Initializes a new instance of the StructValueSimpleInterval class.
    /// </summary>
    /// <param name="valueA">The first value.</param>
    /// <param name="valueB">The seconde value.</param>
    /// <param name="leftOpen">true if it is left open; otherwise, left closed.</param>
    /// <param name="rightOpen">true if it is right open; otherwise, right closed.</param>
    public StructValueSimpleInterval(T valueA, T valueB, bool leftOpen, bool rightOpen)
        : base(valueA, valueB, leftOpen, rightOpen)
    {
    }

    /// <summary>
    /// Initializes a new instance of the StructValueSimpleInterval class.
    /// </summary>
    /// <param name="valueA">The first value.</param>
    /// <param name="valueB">The seconde value.</param>
    /// <param name="leftOpen">true if it is left open; otherwise, left closed.</param>
    /// <param name="rightOpen">true if it is right open; otherwise, right closed.</param>
    /// <param name="negativeInfinite">The negative infinite value.</param>
    /// <param name="positiveInfinite">The positive infinite value.</param>
    public StructValueSimpleInterval(T valueA, T valueB, bool leftOpen, bool rightOpen, T? negativeInfinite, T? positiveInfinite)
        : base(valueA, valueB, leftOpen, rightOpen)
    {
        positiveInfiniteValue = positiveInfinite;
        negativeInfiniteValue = negativeInfinite;
    }

    /// <summary>
    /// Gets a value indicating whether it is left bounded.
    /// </summary>
    [JsonIgnore]
    public override bool LeftBounded => !negativeInfiniteValue.HasValue || !negativeInfiniteValue.Value.Equals(MinValue);

    /// <summary>
    /// Gets a value indicating whether it is right bounded.
    /// </summary>
    [JsonIgnore]
    public override bool RightBounded => !positiveInfiniteValue.HasValue || !positiveInfiniteValue.Value.Equals(MaxValue);

    /// <summary>
    /// Checks if the value is less than MinValue.
    /// </summary>
    /// <param name="value">A value to compare with MinValue.</param>
    /// <returns>true if the specific value is less than or equal MinValue; otherwise, false.</returns>
    public override bool IsLessThanMinValue(T value)
    {
        return 0 < (ValueComparer?.Compare(MinValue, value) ?? MinValue.CompareTo(value));
    }

    /// <summary>
    /// Checks if the value is greater than MinValue.
    /// </summary>
    /// <param name="value">A value to compare with MinValue.</param>
    /// <returns>true if the specific value is greater than or equal MinValue; otherwise, false.</returns>
    public override bool IsGreaterThanMinValue(T value)
    {
        return 0 > (ValueComparer?.Compare(MinValue, value) ?? MinValue.CompareTo(value));
    }

    /// <summary>
    /// Checks if the value equals MinValue.
    /// </summary>
    /// <param name="value">A value to compare with MinValue.</param>
    /// <returns>true if the specific value equals MinValue; otherwise, false.</returns>
    public override bool EqualsMinValue(T value)
    {
        return 0 == (ValueComparer?.Compare(MinValue, value) ?? MinValue.CompareTo(value));
    }

    /// <summary>
    /// Checks if the value is less than MaxValue.
    /// </summary>
    /// <param name="value">A value to compare with MaxValue.</param>
    /// <returns>true if the specific value is less than or equal MaxValue; otherwise, false.</returns>
    public override bool IsLessThanMaxValue(T value)
    {
        return 0 < (ValueComparer?.Compare(MaxValue, value) ?? MaxValue.CompareTo(value));
    }

    /// <summary>
    /// Checks if the value is greater than MaxValue.
    /// </summary>
    /// <param name="value">A value to compare with MaxValue.</param>
    /// <returns>true if the specific value is greater than or equal MaxValue; otherwise, false.</returns>
    public override bool IsGreaterThanMaxValue(T value)
    {
        return 0 > (ValueComparer?.Compare(MaxValue, value) ?? MaxValue.CompareTo(value));
    }

    /// <summary>
    /// Checks if the value equals MaxValue.
    /// </summary>
    /// <param name="value">A value to compare with MaxValue.</param>
    /// <returns>true if the specific value equals MaxValue; otherwise, false.</returns>
    public override bool EqualsMaxValue(T value)
    {
        return 0 == (ValueComparer?.Compare(MaxValue, value) ?? MaxValue.CompareTo(value));
    }

    /// <summary>
    /// Gets the value comparer.
    /// </summary>
    [JsonIgnore]
    protected virtual IComparer<T> ValueComparer { get; }

    /// <summary>
    /// Converts to nullable interval.
    /// </summary>
    /// <param name="value">The original value.</param>
    public static explicit operator NullableValueSimpleInterval<T>(StructValueSimpleInterval<T> value)
    {
        if (value is null) return null;
        return new NullableValueSimpleInterval<T>(value.MinValue, value.MaxValue, value.LeftOpen, value.RightOpen);
    }
}

/// <summary>
/// The simple interval with nullable value.
/// </summary>
[DataContract]
public class NullableValueSimpleInterval<T> : SimpleInterval<T?> where T : struct, IComparable<T>
{
    /// <summary>
    /// Initializes a new instance of the NullableValueSimpleInterval class.
    /// </summary>
    public NullableValueSimpleInterval()
    {
    }

    /// <summary>
    /// Initializes a new instance of the NullableValuedSimpleInterval class.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="op">The operation.</param>
    public NullableValueSimpleInterval(T value, BasicCompareOperator op)
        : base(value, op, null, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the NullableValueSimpleInterval class.
    /// </summary>
    /// <param name="valueA">The first value.</param>
    /// <param name="valueB">The seconde value.</param>
    public NullableValueSimpleInterval(T? valueA, T? valueB)
        : base(valueA, valueB)
    {
    }

    /// <summary>
    /// Initializes a new instance of the NullableValueSimpleInterval class.
    /// </summary>
    /// <param name="valueA">The first value.</param>
    /// <param name="valueB">The seconde value.</param>
    /// <param name="autoFix">true if fix the order automatically; otherwise, false.</param>
    public NullableValueSimpleInterval(T? valueA, T? valueB, bool autoFix)
        : base(valueA, valueB, autoFix)
    {
    }

    /// <summary>
    /// Initializes a new instance of the NullableValueSimpleInterval class.
    /// </summary>
    /// <param name="valueA">The first value.</param>
    /// <param name="valueB">The seconde value.</param>
    /// <param name="leftOpen">true if it is left open; otherwise, left closed.</param>
    /// <param name="rightOpen">true if it is right open; otherwise, right closed.</param>
    public NullableValueSimpleInterval(T? valueA, T? valueB, bool leftOpen, bool rightOpen)
        : base(valueA, valueB, leftOpen || !valueA.HasValue, rightOpen || !valueB.HasValue)
    {
    }

    /// <summary>
    /// Gets a value indicating whether it is left bounded.
    /// </summary>
    [JsonIgnore]
    public override bool LeftBounded => MinValue != null;

    /// <summary>
    /// Gets a value indicating whether it is right bounded.
    /// </summary>
    [JsonIgnore]
    public override bool RightBounded => MaxValue != null;

    /// <summary>
    /// Gets the left value.
    /// </summary>
    /// <exception cref="InvalidOperationException">The left value is null.</exception>
    [JsonIgnore]
    public T LeftValue => MinValue.Value;

    /// <summary>
    /// Gets the right value.
    /// </summary>
    /// <exception cref="InvalidOperationException">The right value is null.</exception>
    [JsonIgnore]
    public T RightValue => MaxValue.Value;

    /// <summary>
    /// Checks if the value is less than MinValue.
    /// </summary>
    /// <param name="value">A value to compare with MinValue.</param>
    /// <returns>true if the specific value is less than or equal MinValue; otherwise, false.</returns>
    public override bool IsLessThanMinValue(T? value)
    {
        if (MinValue == null) return false;
        if (value == null) return true;
        return 0 < (ValueComparer?.Compare(MinValue.Value, value.Value) ?? MinValue.Value.CompareTo(value.Value));
    }

    /// <summary>
    /// Checks if the value is greater than MinValue.
    /// </summary>
    /// <param name="value">A value to compare with MinValue.</param>
    /// <returns>true if the specific value is greater than or equal MinValue; otherwise, false.</returns>
    public override bool IsGreaterThanMinValue(T? value)
    {
        if (value == null) return false;
        if (MinValue == null) return true;
        return 0 > (ValueComparer?.Compare(MinValue.Value, value.Value) ?? MinValue.Value.CompareTo(value.Value));
    }

    /// <summary>
    /// Checks if the value equals MinValue.
    /// </summary>
    /// <param name="value">A value to compare with MinValue.</param>
    /// <returns>true if the specific value is greater than or equal MinValue; otherwise, false.</returns>
    public override bool EqualsMinValue(T? value)
    {
        if (MinValue == null && value == null) return true;
        if (MinValue == null || value == null) return false;
        return 0 == (ValueComparer?.Compare(MinValue.Value, value.Value) ?? MinValue.Value.CompareTo(value.Value));
    }

    /// <summary>
    /// Checks if the value is less than MaxValue.
    /// </summary>
    /// <param name="value">A value to compare with MaxValue.</param>
    /// <returns>true if the specific value is less than or equal MaxValue; otherwise, false.</returns>
    public override bool IsLessThanMaxValue(T? value)
    {
        if (value == null) return false;
        if (MaxValue == null) return true;
        return 0 < (ValueComparer?.Compare(MaxValue.Value, value.Value) ?? MaxValue.Value.CompareTo(value.Value));
    }

    /// <summary>
    /// Checks if the value is greater than MaxValue.
    /// </summary>
    /// <param name="value">A value to compare with MaxValue.</param>
    /// <returns>true if the specific value is greater than or equal MaxValue; otherwise, false.</returns>
    public override bool IsGreaterThanMaxValue(T? value)
    {
        if (MaxValue == null) return false;
        if (value == null) return true;
        return 0 > (ValueComparer?.Compare(MaxValue.Value, value.Value) ?? MaxValue.Value.CompareTo(value.Value));
    }

    /// <summary>
    /// Checks if the value equals MaxValue.
    /// </summary>
    /// <param name="value">A value to compare with MaxValue.</param>
    /// <returns>true if the specific value equals MinValue; otherwise, false.</returns>
    public override bool EqualsMaxValue(T? value)
    {
        if (MaxValue == null && value == null) return true;
        if (MaxValue == null || value == null) return false;
        return 0 == (ValueComparer?.Compare(MaxValue.Value, value.Value) ?? MaxValue.Value.CompareTo(value.Value));
    }

    /// <summary>
    /// Gets the value comparer.
    /// </summary>
    [JsonIgnore]
    protected virtual IComparer<T> ValueComparer { get; }
}

/// <summary>
/// The semantic version interval with class value.
/// </summary>
[DataContract]
[JsonConverter(typeof(Text.JsonNumberConverter))]
public class VersionSimpleInterval : RefValueSimpleInterval<string>
{
    private Version minVer;
    private Version maxVer;

    /// <summary>
    /// Initializes a new instance of the VersionSimpleInterval class.
    /// </summary>
    public VersionSimpleInterval()
    {
        OnInit();
    }

    /// <summary>
    /// Initializes a new instance of the VersionSimpleInterval class.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="op">The operation.</param>
    public VersionSimpleInterval(string value, BasicCompareOperator op)
        : base(value, op)
    {
        OnInit();
    }

    /// <summary>
    /// Initializes a new instance of the VersionSimpleInterval class.
    /// </summary>
    /// <param name="valueA">The first value.</param>
    /// <param name="valueB">The seconde value.</param>
    public VersionSimpleInterval(string valueA, string valueB)
        : base(valueA, valueB)
    {
        OnInit();
    }

    /// <summary>
    /// Initializes a new instance of the VersionSimpleInterval class.
    /// </summary>
    /// <param name="valueA">The first value.</param>
    /// <param name="valueB">The seconde value.</param>
    /// <param name="autoFix">true if fix the order automatically; otherwise, false.</param>
    public VersionSimpleInterval(string valueA, string valueB, bool autoFix)
        : base(valueA, valueB, autoFix)
    {
        OnInit();
    }

    /// <summary>
    /// Initializes a new instance of the VersionSimpleInterval class.
    /// </summary>
    /// <param name="valueA">The first value.</param>
    /// <param name="valueB">The seconde value.</param>
    /// <param name="leftOpen">true if it is left open; otherwise, left closed.</param>
    /// <param name="rightOpen">true if it is right open; otherwise, right closed.</param>
    public VersionSimpleInterval(string valueA, string valueB, bool leftOpen, bool rightOpen)
        : base(valueA, valueB, leftOpen, rightOpen)
    {
        OnInit();
    }

    /// <summary>
    /// Gets the minimum version.
    /// </summary>
    [JsonIgnore]
    public Version MinVersion
    {
        get
        {
            var v = minVer ?? Reflection.VersionComparer.ToVersion(MinValue);
            return minVer = v;
        }
    }

    /// <summary>
    /// Gets the maximum version.
    /// </summary>
    [JsonIgnore]
    public Version MaxVersion
    {
        get
        {
            var v = maxVer ?? Reflection.VersionComparer.ToVersion(MaxValue);
            return maxVer = v;
        }
    }

    /// <summary>
    /// Checks if a value is in the interval.
    /// </summary>
    /// <param name="value">A value to check.</param>
    /// <returns>true if the value is in the interval; otherwise, false.</returns>
    public bool IsInInterval(Version value) => value != null && Contains(value.ToString());

    /// <summary>
    /// Parses a version interval.
    /// </summary>
    /// <param name="s">The version interval string.</param>
    /// <returns>A version interval.</returns>
    public static VersionSimpleInterval Parse(string s)
    {
        return IntervalUtility.Parse(s, (left, leftOpen, right, rightOpen) => new VersionSimpleInterval(left, right, leftOpen, rightOpen), ele => ele, null);
    }

    /// <summary>
    /// Gets the value comparer.
    /// </summary>
    [JsonIgnore]
    protected override IComparer<string> ValueComparer { get; } = new Reflection.VersionComparer { IsWideX = true };

    private void OnInit()
    {
        MinValueChanged += (sender, ev) =>
        {
            minVer = null;
        };
        MaxValueChanged += (sender, ev) =>
        {
            maxVer = null;
        };
    }
}
