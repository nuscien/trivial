// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Arithmetic\Operation.cs" company="Nanchang Jinchen Software Co., Ltd.">
//   Copyright (c) 2010 Nanchang Jinchen Software Co., Ltd. All rights reserved.
// </copyright>
// <summary>
//   The basic arithmetic operations.
// </summary>
// <author>Kingcean Tuan</author>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Trivial.Text;

namespace Trivial.Maths;

/// <summary>
/// The addition capable interface.
/// </summary>
/// <typeparam name="T">The type of value for addition.</typeparam>
public interface IAdditionCapable<T>
{
    /// <summary>
    /// Pluses another value to return. Current value will not be changed.
    /// this + value
    /// </summary>
    /// <param name="value">A given value to be added.</param>
    /// <returns>A result after addition.</returns>
    T Plus(T value);
}

/// <summary>
/// The subtraction capable interface.
/// </summary>
/// <typeparam name="T">The type of value for subtraction.</typeparam>
public interface ISubtractionCapable<T>
{
    /// <summary>
    /// Minuses another value to return. Current value will not be changed.
    /// this - value
    /// </summary>
    /// <param name="value">A given value to be added.</param>
    /// <returns>A result after subtraction.</returns>
    T Minus(T value);
}

/// <summary>
/// The subtraction capable interface.
/// </summary>
/// <typeparam name="T">The type of value for subtraction.</typeparam>
public interface INegationCapable<out T>
{
    /// <summary>
    /// Negates the current value to return. Current value will not be changed.
    /// -this
    /// </summary>
    /// <returns>A result after negation.</returns>
    T Negate();
}

/// <summary>
/// The addition and subtraction capable interface.
/// </summary>
/// <typeparam name="T">The type of value for addition and subtraction.</typeparam>
public interface IAdvancedAdditionCapable<T> : IAdditionCapable<T>, ISubtractionCapable<T>, INegationCapable<T>
{
    /// <summary>
    /// Gets a unit element for addition and subtraction.
    /// 0
    /// </summary>
    /// <returns>An element zero for the value.</returns>
    T GetElementZero();

    /// <summary>
    /// Gets a value indicating whether the current element is negative.
    /// true if it is positve or zero; otherwise, false.
    /// </summary>
    bool IsNegative { get; }

    /// <summary>
    /// Gets a value indicating whether the current element is a unit element of zero.
    /// </summary>
    bool IsZero { get; }
}

/// <summary>
/// The binary operation formula.
/// </summary>
/// <typeparam name="TValue">The type of value.</typeparam>
/// <typeparam name="TOperator">The type of operation.</typeparam>
[JsonConverter(typeof(JsonBinaryOperationFormulaConverter))]
public class BinaryOperationFormula<TValue, TOperator>
    where TValue : IEquatable<TValue>
    where TOperator : struct, Enum
{
    /// <summary>
    /// Initializes a new instance of the BinaryOperationFormula class.
    /// </summary>
    /// <param name="leftValue">The left value to calculate.</param>
    /// <param name="op">The operator.</param>
    /// <param name="rightValue">The right value to calculate.</param>
    public BinaryOperationFormula(TValue leftValue, TOperator op, TValue rightValue)
    {
        LeftValue = leftValue;
        Operator = op;
        RightValue = rightValue;
    }

    /// <summary>
    /// Gets or sets the left value to calculate.
    /// </summary>
    [JsonPropertyName("a")]
    public TValue LeftValue { get; set; }

    /// <summary>
    /// Gets or sets the operator.
    /// </summary>
    [JsonPropertyName("op")]
    public TOperator Operator { get; set; }

    /// <summary>
    /// Gets or sets the right value to calculate.
    /// </summary>
    [JsonPropertyName("b")]
    public TValue RightValue { get; set; }
}

/// <summary>
/// The binary operation formula.
/// </summary>
/// <typeparam name="TValue">The type of value.</typeparam>
/// <typeparam name="TOperator">The type of operation.</typeparam>
[JsonConverter(typeof(JsonBinaryOperationFormulaConverter))]
public abstract class BaseBinaryOperationFormula<TValue, TOperator> : BinaryOperationFormula<TValue, TOperator>
    where TValue : IEquatable<TValue>
    where TOperator : struct, Enum
{
    private TValue result;

    /// <summary>
    /// Initializes a new instance of the BaseBinaryOperationFormula class.
    /// </summary>
    /// <param name="leftValue">The left value to calculate.</param>
    /// <param name="op">The operator.</param>
    /// <param name="rightValue">The right value to calculate.</param>
    protected BaseBinaryOperationFormula(TValue leftValue, TOperator op, TValue rightValue)
        : base(leftValue, op, rightValue)
    {
        IsValid = TryGetResult(out var result);
        this.result = result;
    }

    /// <summary>
    /// Gets the result.
    /// </summary>
    /// <exception cref="NotSupportedException">The operator is not supported; or the values can not computed.</exception>
    [JsonIgnore]
    public virtual TValue Result => result ?? throw new($"The operation {Operator} is not supported; or the values can not computed.");

    /// <summary>
    /// Gets a value indicating whether the operator is valid.
    /// </summary>
    [JsonIgnore]
    public virtual bool IsValid { get; }

    /// <summary>
    /// Tries to get result.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>true if valid; otherwise, false.</returns>
    protected abstract bool TryGetResult(out TValue result);
}

internal class UnknownBinaryOperationFormula<TValue, TOperator> : BaseBinaryOperationFormula<TValue, TOperator>
    where TValue : IEquatable<TValue>
    where TOperator : struct, Enum
{
    /// <summary>
    /// Initializes a new instance of the UnknownBinaryOperationFormula class.
    /// </summary>
    /// <param name="leftValue">The left value to calculate.</param>
    /// <param name="op">The operator.</param>
    /// <param name="rightValue">The right value to calculate.</param>
    public UnknownBinaryOperationFormula(TValue leftValue, TOperator op, TValue rightValue)
        : base(leftValue, op, rightValue)
    {
    }

    /// <inheritdoc />
    [JsonIgnore]
    public override TValue Result => throw new NotSupportedException("Unknown operation.", new NotImplementedException("No implementation for the formula."));

    /// <inheritdoc />
    protected override bool TryGetResult(out TValue result)
    {
        result = default;
        return false;
    }
}

/// <summary>
/// The JSON converter of JSON binary operation formula.
/// </summary>
public class JsonBinaryOperationFormulaConverter : JsonConverterFactory
{
    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert)
        => GetBaseType(typeToConvert) is not null;

    /// <inheritdoc />
    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var baseType = GetBaseType(typeToConvert);
        var types = baseType.GenericTypeArguments;
        if (types == null || types.Length != 2) throw Exception(typeToConvert);
        var t = typeof(JsonBinaryOperationFormulaConverter<,>).MakeGenericType(types);
        var c = Activator.CreateInstance(t, new object[] { options }) as JsonConverter;
        return c ?? throw Exception(typeToConvert);
    }

    private static JsonException Exception(Type typeToConvert)
        => throw new JsonException(string.Concat("The type ", typeToConvert.Name, " is not supported to serialize or deserialize."), new NotSupportedException("The type is not supported to serialize or deserialize."));

    private static Type GetBaseType(Type typeToConvert)
    {
        var baseType = typeToConvert;
        while (!baseType.IsGenericType || baseType.GetGenericTypeDefinition() != typeof(BinaryOperationFormula<,>))
        {
            if (baseType is null) break;
            baseType = baseType.BaseType;
        }

        return baseType;
    }
}

/// <summary>
/// The JSON converter of JSON binary operation formula.
/// </summary>
/// <typeparam name="TValue">The type of value.</typeparam>
/// <typeparam name="TOperator">The type of operator.</typeparam>
internal class JsonBinaryOperationFormulaConverter<TValue, TOperator> : JsonConverter<BinaryOperationFormula<TValue, TOperator>>
    where TValue : IEquatable<TValue>
    where TOperator : struct, Enum
{
    private readonly JsonConverter<TOperator> opConverter;
    private readonly JsonConverter<TValue> vConverter;

    /// <summary>
    /// Initializes a new instance of the JsonBinaryOperationFormulaConverter class.
    /// </summary>
    public JsonBinaryOperationFormulaConverter(JsonSerializerOptions options)
    {
        opConverter = (JsonConverter<TOperator>)options.GetConverter(typeof(TOperator));
        vConverter = (JsonConverter<TValue>)options.GetConverter(typeof(TValue));
    }

    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert)
        => typeof(BinaryOperationFormula<TValue, TOperator>).IsAssignableFrom(typeToConvert);

    /// <inheritdoc />
    public override BinaryOperationFormula<TValue, TOperator> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (typeToConvert == typeof(BaseBinaryOperationFormula<TValue, TOperator>)) typeToConvert = typeof(UnknownBinaryOperationFormula<TValue, TOperator>);
        JsonValues.SkipComments(ref reader);
        switch (reader.TokenType)
        {
            case JsonTokenType.Null:
            case JsonTokenType.False:
                return null;
            case JsonTokenType.String:
                var parser = typeToConvert.GetMethod("Parse", new[] { typeof(string) });
                if (parser is null || !parser.IsStatic) throw new JsonException("Expect a JSON but a string.");
                try
                {
                    var obj = parser.Invoke(null, new object[] { reader.GetString() });
                    if (obj is BinaryOperationFormula<TValue, TOperator> f) return f;
                }
                catch (InvalidOperationException)
                {
                }
                catch (TargetException)
                {
                }
                catch (MemberAccessException)
                {
                }
                catch (NotSupportedException)
                {
                }

                throw new JsonException("Expect a JSON but a string.");
            case JsonTokenType.StartObject:
                var json = new JsonObjectNode(ref reader);
                var a = json.DeserializeValue<TValue>("a");
                var op = json.DeserializeValue<TOperator>("op");
                var b = json.DeserializeValue<TValue>("b");
                var constructor = typeToConvert.GetConstructor(new[] { typeof(TValue), typeof(TOperator), typeof(TValue) });
                if (constructor is not null) return Create(constructor, new object[] { a, op, b });
                constructor = typeToConvert.GetConstructor(new[] { typeof(TOperator), typeof(TValue), typeof(TValue) });
                if (constructor is not null) return Create(constructor, new object[] { op, a, b });
                var deserializer = typeToConvert.GetMethod("Parse", new Type[] { typeof(JsonObjectNode) });
                if (deserializer is not null && deserializer.IsStatic)
                {
                    try
                    {
                        var obj = deserializer.Invoke(null, new object[] { json });
                        if (obj is BinaryOperationFormula<TValue, TOperator> f) return f;
                    }
                    catch (InvalidOperationException)
                    {
                    }
                    catch (TargetException)
                    {
                    }
                    catch (MemberAccessException)
                    {
                    }
                    catch (NotSupportedException)
                    {
                    }
                }

                throw new JsonException("The type does not support to deserialze.");
        }

        throw new JsonException($"Expect a JSON but a {reader.TokenType}.");
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, BinaryOperationFormula<TValue, TOperator> value, JsonSerializerOptions options)
    {
        if (value is IJsonObjectHost host)
        {
            var json = host.ToJson();
            if (json is null) writer.WriteNullValue();
            else json.WriteTo(writer);
            return;
        }

        writer.WriteStartObject();
        writer.WritePropertyName("op");
        opConverter.Write(writer, value.Operator, options);
        writer.WritePropertyName("a");
        vConverter.Write(writer, value.LeftValue, options);
        writer.WritePropertyName("b");
        vConverter.Write(writer, value.RightValue, options);
        writer.WriteEndObject();
    }

    private static BinaryOperationFormula<TValue, TOperator> Create(ConstructorInfo constructor, object[] parameters)
    {
        try
        {
            var obj = constructor.Invoke(parameters);
            if (obj is BinaryOperationFormula<TValue, TOperator> f) return f;
        }
        catch (InvalidOperationException)
        {
        }
        catch (TargetException)
        {
        }
        catch (MemberAccessException)
        {
        }
        catch (NotSupportedException)
        {
        }

        throw new JsonException("Cannot deserialize the formula.");
    }
}
