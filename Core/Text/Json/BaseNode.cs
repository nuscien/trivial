using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.AccessControl;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Trivial.Data;
using Trivial.Reflection;
using Trivial.Security;
using Trivial.Web;

namespace Trivial.Text;

/// <summary>
/// The JSON value node base.
/// </summary>
public abstract class BaseJsonValueNode : IJsonValueNode, IEquatable<IJsonValueNode>, IConvertible
{
    /// <summary>
    /// Initializes a new instance of the JsonNull class.
    /// </summary>
    /// <param name="valueKind">The JSON value kind.</param>
    internal BaseJsonValueNode(JsonValueKind valueKind)
    {
        ValueKind = valueKind;
    }

    /// <summary>
    /// Gets the raw value.
    /// </summary>
    protected abstract object RawValue { get; }

    /// <summary>
    /// Gets the type of the current JSON value.
    /// </summary>
    public JsonValueKind ValueKind { get; }

    /// <summary>
    /// Indicates whether this instance and a specified object are equal.
    /// </summary>
    /// <param name="other">The object to compare with the current instance.</param>
    /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
    public abstract bool Equals(IJsonValueNode other);

    /// <summary>
    /// Indicates whether this instance and a specified object are equal.
    /// </summary>
    /// <param name="other">The object to compare with the current instance.</param>
    /// <returns>true if obj and this instance represent the same value; otherwise, false.</returns>
    public override bool Equals(object other)
    {
        if (other is null) return ValueKind == JsonValueKind.Null || ValueKind == JsonValueKind.Undefined;
        if (other is IJsonValueNode node) return Equals(node);
        try
        {
            var raw = As(other.GetType());
            return other.Equals(raw);
        }
        catch (InvalidOperationException)
        {
        }
        catch (NotSupportedException)
        {
        }
        catch (NotImplementedException)
        {
        }
        catch (OverflowException)
        {
        }
        catch (ArgumentException)
        {
        }
        catch (FormatException)
        {
        }
        catch (ArithmeticException)
        {
        }
        catch (JsonException)
        {
        }
        catch (AggregateException)
        {
        }
        catch (NullReferenceException)
        {
        }
        catch (ExternalException)
        {
        }

        return false;
    }

    /// <inhertidoc />
    public override int GetHashCode()
        => (RawValue ?? DBNull.Value).GetHashCode();

    /// <summary>
    /// Gets the item value count.
    /// It always return 0 because it is not an array or object.
    /// </summary>
    public virtual int Count => 0;

    /// <summary>
    /// Tests if the specific value kind is the one matched.
    /// </summary>
    /// <param name="kind">The value kind to test.</param>
    /// <returns>true if they are the same value kind; otherwise, false.</returns>
    public bool IsValueKind(JsonValueKind kind)
        => ValueKind == kind;

    /// <summary>
    /// Converts to a specific type.
    /// </summary>
    /// <param name="type">The type to convert.</param>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <returns>The value converted.</returns>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    /// <exception cref="OverflowException">The value is outside the range of the underlying type of enum expected.</exception>
    /// <exception cref="NotSupportedException">The type is not supported to convert.</exception>
    public object As(Type type, bool strict = false)
    {
        var obj = TryConvert(type, strict, out var exception, true);
        if (exception != null) throw exception;
        return obj;
    }

    /// <summary>
    /// Converts to a specific type.
    /// </summary>
    /// <typeparam name="T">The type to convert.</typeparam>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <returns>The value converted.</returns>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    /// <exception cref="OverflowException">The value is outside the range of the underlying type of enum expected.</exception>
    /// <exception cref="NotSupportedException">The type is not supported to convert.</exception>
    public T As<T>(bool strict = false)
    {
        var obj = TryConvert(typeof(T), strict, out var exception, true);
        if (exception != null) throw exception;
        try
        {
            return (T)obj;
        }
        catch (InvalidCastException ex)
        {
            throw new NotSupportedException("The type is not supported.", ex);
        }
        catch (ArgumentException ex)
        {
            throw new NotSupportedException("The type is not supported.", ex);
        }
        catch (NotImplementedException ex)
        {
            throw new NotSupportedException("The type is not supported.", ex);
        }
        catch (NullReferenceException ex)
        {
            throw new InvalidOperationException(ex.Message, ex);
        }
    }

    /// <summary>
    /// Converts to a specific type.
    /// </summary>
    /// <typeparam name="T">The type to convert.</typeparam>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <param name="exception">The exception thrown if failed.</param>
    /// <returns>true if converts succeeded; otherwise, false.</returns>
    internal bool TryConvert<T>(bool strict, out T result, out Exception exception)
    {
        try
        {
            result = (T)TryConvert(typeof(T), strict, out exception, false);
            return exception is null;
        }
        catch (InvalidOperationException ex)
        {
            exception = ex;
        }
        catch (InvalidCastException ex)
        {
            exception = ex;
        }
        catch (OverflowException ex)
        {
            exception = ex;
        }
        catch (ArgumentException ex)
        {
            exception = ex;
        }
        catch (FormatException ex)
        {
            exception = ex;
        }
        catch (JsonException ex)
        {
            exception = ex;
        }
        catch (ArithmeticException ex)
        {
            exception = ex;
        }
        catch (NotImplementedException ex)
        {
            exception = ex;
        }
        catch (NullReferenceException ex)
        {
            exception = ex;
        }
        catch (AggregateException ex)
        {
            exception = ex;
        }
        catch (ExternalException ex)
        {
            exception = ex;
        }

        result = default;
        return false;
    }

    /// <summary>
    /// Converts to a specific type.
    /// </summary>
    /// <typeparam name="T">The type to convert.</typeparam>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <returns>The value converted.</returns>
    public T TryConvert<T>(bool strict = false)
        => TryConvert<T>(strict, out var result, out _) ? result : default;

    /// <summary>
    /// Converts to a specific type.
    /// </summary>
    /// <param name="type">The type to convert.</param>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <returns>The value converted.</returns>
    /// <exception cref="NotSupportedException">The type is not supported.</exception>
    protected virtual object FallbackAs(Type type, bool strict)
        => throw new NotSupportedException("The type is not supported.", new InvalidOperationException("The type is not supported."));

    private object FallbackAs(Type type, bool strict, out Exception exception, bool throwException)
    {
        try
        {
            exception = null;
            return FallbackAs(type, strict);
        }
        catch (InvalidCastException ex)
        {
            exception = new NotSupportedException("The type is not supported.", ex);
        }
        catch (ArgumentException ex)
        {
            exception = new NotSupportedException("The type is not supported.", ex);
        }
        catch (ArithmeticException ex)
        {
            exception = new InvalidOperationException(ex.Message, ex);
        }
        catch (JsonException ex)
        {
            exception = new NotSupportedException("The type is not supported.", ex);
        }
        catch (NotImplementedException ex)
        {
            exception = new NotSupportedException("The type is not supported.", ex);
        }
        catch (FormatException ex)
        {
            exception = new InvalidOperationException(ex.Message, ex);
        }
        catch (AggregateException ex)
        {
            exception = new InvalidOperationException(ex.Message, ex);
        }
        catch (ApplicationException ex)
        {
            exception = new InvalidOperationException(ex.Message, ex);
        }
        catch (ExternalException ex)
        {
            exception = new NotSupportedException("The type is not supported.", ex);
        }
        catch (OutOfMemoryException)
        {
            throw;
        }
        catch (Exception ex)
        {
            exception = ex;
            if (throwException) throw;
        }

        throw exception ?? new InvalidOperationException("Unknown error.");
    }

    private T TryConvert<T>(JsonNodeTryConvertHandler<T> handler, bool strict, out Exception ex, bool throwEx, string expect, bool overflow = false)
    {
        if (handler(strict, out var result))
        {
            ex = null;
            return result;
        }

        ex = CreateInvalidOperationException(expect, overflow);
        if (throwEx) throw ex;
        return default;
    }

    private T TryConvert<T>(JsonNodeTryConvertHandler<T> handler, bool strict, out Exception ex, bool throwEx, JsonValueKind expect)
    {
        if (handler(strict, out var result))
        {
            ex = null;
            return result;
        }

        ex = CreateInvalidOperationException(expect);
        if (throwEx) throw ex;
        return default;
    }

    /// <summary>
    /// Converts to a specific type.
    /// </summary>
    /// <param name="type">The type to convert.</param>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="exception">The exception thrown if failed.</param>
    /// <param name="throwException">true if throw the exception directly if failed; otherwise, false.</param>
    /// <returns>The value converted.</returns>
    private object TryConvert(Type type, bool strict, out Exception exception, bool throwException)
    {
        if (type == typeof(IJsonValueNode) || type == typeof(BaseJsonValueNode) || type == GetType())
        {
            exception = null;
            return this;
        }

        var isNull = ValueKind == JsonValueKind.Null || ValueKind == JsonValueKind.Undefined;
        if (type.IsValueType)
        {
            if (isNull)
            {
                exception = null;
                if (ObjectConvert.IsNullableValueType(type)) return null;
                if (type == typeof(Guid))
                {
                    exception = null;
                    return Guid.Empty;
                }

                if (!strict)
                {
                    if (type == typeof(double)) return double.NaN;
                    if (type == typeof(float)) return float.NaN;
                }

                exception = new InvalidOperationException("Expect a value not-null but it is null or undefined.");
                if (throwException) throw exception;
                return default;
            }

            if (type == typeof(int)) return TryConvert<int>(TryConvert, strict, out exception, throwException, "an integer", true);
            if (type == typeof(long)) return TryConvert<long>(TryConvert, strict, out exception, throwException, "an integer", true);
            if (type == typeof(short)) return TryConvert<short>(TryConvert, strict, out exception, throwException, "an integer", true);
            if (type == typeof(uint)) return TryConvert<uint>(TryConvert, strict, out exception, throwException, "an integer", true);
            if (type == typeof(ulong)) return TryConvert<ulong>(TryConvert, strict, out exception, throwException, "an integer", true);
            if (type == typeof(bool)) return TryConvert<bool>(TryConvert, strict, out exception, throwException, "a boolean");
            if (type == typeof(float)) return TryConvert<float>(TryConvert, strict, out exception, throwException, JsonValueKind.Number);
            if (type == typeof(double)) return TryConvert<double>(TryConvert, strict, out exception, throwException, JsonValueKind.Number);
            if (type == typeof(decimal)) return TryConvert<decimal>(TryConvert, strict, out exception, throwException, JsonValueKind.Number);
            if (type == typeof(byte)) return TryConvert<short>(TryConvert, strict, out exception, throwException, "an integer", true);
            if (type == typeof(ushort))
            {
                var i = TryConvert<int>(TryConvert, strict, out exception, throwException, "an integer", true);
                if (i >= 0 && i <= ushort.MaxValue) return (ushort)i;
                exception = CreateInvalidOperationException("an interger", true);
                if (throwException) throw exception;
                return default;
            }

            if (type == typeof(sbyte))
            {
                var i = TryConvert<short>(TryConvert, strict, out exception, throwException, "an integer", true);
                if (i >= sbyte.MinValue && i <= sbyte.MaxValue) return (ushort)i;
                exception = CreateInvalidOperationException("an interger", true);
                if (throwException) throw exception;
                return default;
            }

            if (type == typeof(DateTime))
            {
                if (TryConvert(out DateTime dt))
                {
                    exception = null;
                    return dt;
                }

                exception = CreateInvalidOperationException("a date time format string or a JavaScript tick");
                if (throwException) throw exception;
                return WebFormat.ZeroTick;
            }

            if (type == typeof(Guid))
            {
                if (TryConvert(true, out string guidStr) && Guid.TryParse(guidStr, out var guid))
                {
                    exception = null;
                    return guid;
                }

                exception = CreateInvalidOperationException("a GUID format string");
                if (throwException) throw exception;
                return Guid.Empty;
            }

            if (type == typeof(byte[]))
            {
                if (ValueKind != JsonValueKind.String)
                {
                    exception = CreateInvalidOperationException("a base64 encoded string");
                    if (throwException) throw exception;
                    return Array.Empty<byte>();
                }

                try
                {
                    exception = null;
                    return ((IJsonValueNode)this).GetBytesFromBase64();
                }
                catch (Exception ex)
                {
                    exception = ex;
                    if (throwException) throw;
                    return Array.Empty<byte>();
                }
            }

            if (type == typeof(char))
            {
                if (this is IJsonValueNode<int> i1)
                {
                    exception = null;
                    try
                    {
                        return (char)i1.Value;
                    }
                    catch (Exception ex)
                    {
                        exception = ex;
                        if (throwException) throw exception;
                        return default;
                    }
                }

                if (this is IJsonValueNode<long> i2)
                {
                    exception = null;
                    try
                    {
                        return (char)i2.Value;
                    }
                    catch (Exception ex)
                    {
                        exception = ex;
                        if (throwException) throw exception;
                        return default;
                    }
                }

                if (this is IJsonValueNode<string> i3)
                {
                    var i4 = i3.Value;
                    if (i4 == null || i4.Length != 1)
                    {
                        exception = CreateInvalidOperationException("a string with only one char");
                        if (throwException) throw exception;
                        return default;
                    }

                    exception = null;
                    return i4[0];
                }

                if (this is IJsonValueNode<char> i5)
                {
                    exception = null;
                    return i5.Value;
                }
            }

            if (type == typeof(JsonValueKind))
            {
                exception = null;
                return ValueKind;
            }

            if (type.IsEnum)
            {
                try
                {
                    if (!TryConvert(true, out string enumStr))
                    {
                        if (!TryConvert(true, out int enumInt))
                        {
                            exception = CreateInvalidOperationException("a string or an integer");
                            if (throwException) throw exception;
                            return default;
                        }

                        exception = null;
                        return Enum.ToObject(type, enumInt);
                    }

#if NET6_0_OR_GREATER
                    if (Enum.TryParse(type, enumStr, out object enumObj))
                    {
                        exception = null;
                        return enumObj;
                    }
                    else
                    {
                        exception = CreateInvalidOperationException("a string or an integer");
                        if (throwException) throw exception;
                        return default;
                    }
#else
                    exception = null;
                    return Enum.Parse(type, enumStr);

#endif
                }
                catch (ArgumentException ex)
                {
                    exception = new InvalidOperationException("The input value is not a valid enum expected.", ex);
                    if (throwException) throw exception;
                    return default;
                }
                catch (Exception ex)
                {
                    exception = ex;
                    if (throwException) throw;
                    return default;
                }
            }

            return FallbackAs(type, strict, out exception, throwException);
        }

        if (type == typeof(DBNull))
        {
            if (isNull)
            {
                exception = null;
            }
            else
            {
                exception = CreateInvalidOperationException(JsonValueKind.Null);
                if (throwException) throw exception;
            }

            return DBNull.Value;
        }

        if (isNull)
        {
            exception = null;
            return null;
        }

        if (type == typeof(string)) return TryConvert<string>(TryConvert, strict, out exception, throwException, JsonValueKind.String);
        if (type == typeof(StringBuilder))
        {
            var str = TryConvert<string>(TryConvert, strict, out exception, throwException, JsonValueKind.String);
            return str != null ? new StringBuilder(str) : null;
        }

        if (type == typeof(SecureString))
        {
            var str = TryConvert<string>(TryConvert, strict, out exception, throwException, JsonValueKind.String);
            return str != null ? SecureStringExtensions.ToSecure(str) : null;
        }

        if (type == typeof(Uri))
        {
            var str = TryConvert<string>(TryConvert, strict, out exception, throwException, JsonValueKind.String);
            if (str != null && Uri.TryCreate(str, UriKind.RelativeOrAbsolute, out var uri)) return uri;
            exception = CreateInvalidOperationException("a URL string");
            if (throwException) throw exception;
            return null;
        }

        if (this is JsonObjectNode json && !type.IsInterface)
        {
            try
            {
                exception = null;
                return json.Deserialize(type);
            }
            catch (JsonException ex)
            {
                exception = new InvalidOperationException("Deserialize failed.", ex);
                if (throwException) throw exception;
                return default;
            }
            catch (ArgumentException ex)
            {
                exception = new InvalidOperationException("Deserialize failed.", ex);
                if (throwException) throw exception;
                return default;
            }
        }
        else if (this is JsonArrayNode arr)
        {
            try
            {
                exception = null;
                return arr.Deserialize(type);
            }
            catch (JsonException ex)
            {
                exception = new InvalidOperationException("Deserialize failed.", ex);
                if (throwException) throw exception;
                return default;
            }
            catch (ArgumentException ex)
            {
                exception = new InvalidOperationException("Deserialize failed.", ex);
                if (throwException) throw exception;
                return default;
            }
        }

        return FallbackAs(type, strict, out exception, throwException);
    }

    private T Cast<T>()
    {
        try
        {
            var obj = TryConvert(typeof(T), false, out var exception, true);
            if (exception != null) throw exception;
            try
            {
                return (T)obj;
            }
            catch (NullReferenceException ex)
            {
                throw new InvalidCastException(ex.Message, ex);
            }
        }
        catch (InvalidOperationException ex)
        {
            throw new InvalidCastException(ex.Message, ex);
        }
        catch (NotSupportedException ex)
        {
            var innerEx = ex.InnerException ?? throw new InvalidCastException(ex.Message, ex);
            throw innerEx is InvalidCastException ? innerEx : new InvalidCastException(ex.Message, innerEx);
        }
        catch (NotImplementedException ex)
        {
            throw new InvalidCastException(ex.Message, ex);
        }
        catch (ArgumentException ex)
        {
            throw new InvalidCastException(ex.Message, ex);
        }
        catch (ArithmeticException ex)
        {
            throw new InvalidCastException(ex.Message, ex);
        }
        catch (JsonException ex)
        {
            throw new InvalidCastException(ex.Message, ex);
        }
        catch (AggregateException ex)
        {
            throw new InvalidCastException(ex.Message, ex);
        }
        catch (ExternalException ex)
        {
            throw new InvalidCastException(ex.Message, ex);
        }
    }

    /// <summary>
    /// Gets the value of the element as a byte array.
    /// </summary>
    /// <returns>The value decoded as a byte array.</returns>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    byte[] IJsonValueNode.GetBytesFromBase64()
    {
        if (!TryConvert(true, out string s)) throw CreateInvalidOperationException(JsonValueKind.String);
        if (string.IsNullOrEmpty(s)) return Array.Empty<byte>();
        return Convert.FromBase64String(s);
    }

    /// <summary>
    /// Gets the value of the element as a boolean.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <returns>The value of the element as a boolean.</returns>
    bool IJsonValueNode.GetBoolean(bool strict)
        => TryConvert(strict, out bool result) ? result : throw CreateInvalidOperationException("a boolean");

    /// <summary>
    /// Tries to get the value of the element as a boolean.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool IJsonValueNode.TryConvert(bool strict, out bool result)
        => TryConvert(strict, out result);

    /// <summary>
    /// Tries to get the value of the element as a boolean.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected virtual bool TryConvert(bool strict, out bool result)
    {
        result = default;
        return false;
    }

    /// <summary>
    /// Gets the value of the element as a date time.
    /// </summary>
    /// <returns>The value of the element as a date time.</returns>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    DateTime IJsonValueNode.GetDateTime()
        => TryConvert(out DateTime result) ? result : throw CreateInvalidOperationException("a date time string or a tick number");

    /// <summary>
    /// Tries to get the value of the element as a date time.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool IJsonValueNode.TryConvert(out DateTime result)
        => TryConvert(out result);

    /// <summary>
    /// Tries to get the value of the element as a date time.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected virtual bool TryConvert(out DateTime result)
    {
        result = WebFormat.ZeroTick;
        return false;
    }

    /// <summary>
    /// Gets the value of the element as a number.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <returns>The value of the element as a number.</returns>
    decimal IJsonValueNode.GetDecimal(bool strict)
        => TryConvert(strict, out decimal result) ? result : throw CreateInvalidOperationException(JsonValueKind.Number);

    /// <summary>
    /// Tries to get the value of the element as a number.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool IJsonValueNode.TryConvert(bool strict, out decimal result)
        => TryConvert(strict, out result);

    /// <summary>
    /// Tries to get the value of the element as a number in strict mode.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected virtual bool TryConvert(bool strict, out decimal result)
    {
        result = default;
        return false;
    }

    /// <summary>
    /// Gets the value of the element as a number.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <returns>The value of the element as a number.</returns>
    float IJsonValueNode.GetSingle(bool strict)
        => TryConvert(strict, out float result) ? result : throw CreateInvalidOperationException(JsonValueKind.Number);

    /// <summary>
    /// Tries to get the value of the element as a number.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool IJsonValueNode.TryConvert(bool strict, out float result)
        => TryConvert(strict, out result);

    /// <summary>
    /// Tries to get the value of the element as a number.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected virtual bool TryConvert(bool strict, out float result)
    {
        result = default;
        return false;
    }

    /// <summary>
    /// Gets the value of the element as a number.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <returns>The value of the element as a number.</returns>
    double IJsonValueNode.GetDouble(bool strict)
        => TryConvert(strict, out double result) ? result : throw CreateInvalidOperationException(JsonValueKind.Number);

    /// <summary>
    /// Tries to get the value of the element as a number.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool IJsonValueNode.TryConvert(bool strict, out double result)
        => TryConvert(strict, out result);

    /// <summary>
    /// Tries to get the value of the element as a number.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected virtual bool TryConvert(bool strict, out double result)
    {
        result = default;
        return false;
    }

    /// <summary>
    /// Gets the value of the element as a number.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <returns>The value of the element as a number.</returns>
    short IJsonValueNode.GetInt16(bool strict)
        => TryConvert(strict, out short result) ? result : throw CreateInvalidOperationException(JsonValueKind.Number);

    /// <summary>
    /// Tries to get the value of the element as a number.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool IJsonValueNode.TryConvert(bool strict, out short result)
        => TryConvert(strict, out result);

    /// <summary>
    /// Tries to get the value of the element as a number.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected virtual bool TryConvert(bool strict, out short result)
    {
        result = default;
        return false;
    }

    /// <summary>
    /// Gets the value of the element as a number.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <returns>The value of the element as a number.</returns>
    uint IJsonValueNode.GetUInt32(bool strict)
        => TryConvert(strict, out uint result) ? result : throw CreateInvalidOperationException("an integer", true);

    /// <summary>
    /// Tries to get the value of the element as a number.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool IJsonValueNode.TryConvert(bool strict, out uint result)
        => TryConvert(strict, out result);

    /// <summary>
    /// Tries to get the value of the element as a number.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected virtual bool TryConvert(bool strict, out uint result)
    {
        result = default;
        return false;
    }

    /// <summary>
    /// Gets the value of the element as a number.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <returns>The value of the element as a number.</returns>
    int IJsonValueNode.GetInt32(bool strict)
        => TryConvert(strict, out int result) ? result : throw CreateInvalidOperationException("an integer", true);

    /// <summary>
    /// Tries to get the value of the element as a number.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool IJsonValueNode.TryConvert(bool strict, out int result)
        => TryConvert(strict, out result);

    /// <summary>
    /// Tries to get the value of the element as a number.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected virtual bool TryConvert(bool strict, out int result)
    {
        result = default;
        return false;
    }

    /// <summary>
    /// Gets the value of the element as a number.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <returns>The value of the element as a number.</returns>
    long IJsonValueNode.GetInt64(bool strict)
        => TryConvert(strict, out long result) ? result : throw CreateInvalidOperationException("an integer", true);

    /// <summary>
    /// Tries to get the value of the element as a number.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool IJsonValueNode.TryConvert(bool strict, out long result)
        => TryConvert(strict, out result);

    /// <summary>
    /// Tries to get the value of the element as a number.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected virtual bool TryConvert(bool strict, out long result)
    {
        result = default;
        return false;
    }

    /// <summary>
    /// Gets the value of the element as a number.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <returns>The value of the element as a number.</returns>
    ulong IJsonValueNode.GetUInt64(bool strict)
        => TryConvert(strict, out ulong result) ? result : throw CreateInvalidOperationException("an integer", true);

    /// <summary>
    /// Tries to get the value of the element as a number.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool IJsonValueNode.TryConvert(bool strict, out ulong result)
        => TryConvert(strict, out result);

    /// <summary>
    /// Tries to get the value of the element as a number.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected virtual bool TryConvert(bool strict, out ulong result)
    {
        result = default;
        return false;
    }

    /// <summary>
    /// Gets the value of the element as a number.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <returns>The value of the element as a number.</returns>
    string IJsonValueNode.GetString(bool strict)
        => TryConvert(strict, out string result) ? result : throw CreateInvalidOperationException(JsonValueKind.String);

    /// <summary>
    /// Tries to get the value of the element as a number.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool IJsonValueNode.TryConvert(bool strict, out string result)
        => TryConvert(strict, out result);

    /// <summary>
    /// Tries to get the value of the element as a number.
    /// </summary>
    /// <param name="strict">true if enable strict mode that compare the value kind firstly; otherwise, false, to convert in compatible mode.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    protected abstract bool TryConvert(bool strict, out string result);

    /// <summary>
    /// Gets the value of the element as a GUID.
    /// </summary>
    /// <returns>The value of the element as a GUID.</returns>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    Guid IJsonValueNode.GetGuid()
    {
        if (TryConvert(true, out string s)) return Guid.Parse(s);
        throw CreateInvalidOperationException("a GUID format string");
    }

    /// <summary>
    /// Tries to get the value of the element as a GUID.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool IJsonValueNode.TryConvert(out Guid result)
    {
        if (TryConvert(true, out string s)) return Guid.TryParse(s, out result);
        result = Guid.Empty;
        return false;
    }

    /// <summary>
    /// Gets all property keys.
    /// </summary>
    /// <returns>The property keys.</returns>
    /// <exception cref="InvalidOperationException">The value kind is not an object.</exception>
    IEnumerable<string> IJsonValueNode.Keys()
        => GetPropertyKeys();

    /// <summary>
    /// Gets all property keys.
    /// </summary>
    /// <returns>The property keys.</returns>
    /// <exception cref="InvalidOperationException">The value kind is not an object.</exception>
    protected virtual IEnumerable<string> GetPropertyKeys()
        => throw CreateInvalidOperationException(JsonValueKind.Object);

    /// <summary>
    /// Gets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <returns>The value.</returns>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    IJsonValueNode IJsonValueNode.GetValue(string key)
    {
        var v = TryGetValueOrNull(key);
        if (v is not null && v.ValueKind != JsonValueKind.Undefined) return v;
        if (ValueKind != JsonValueKind.Object) throw CreateInvalidOperationException(JsonValueKind.Object);
        throw new ArgumentOutOfRangeException(nameof(key), "key was outside the range of the JSON properties.");
    }

    /// <summary>
    /// Gets the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <returns>The value.</returns>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    IJsonValueNode IJsonValueNode.GetValue(ReadOnlySpan<char> key)
    {
        var v = TryGetValueOrNull(key);
        if (v is not null && v.ValueKind != JsonValueKind.Undefined) return v;
        if (ValueKind != JsonValueKind.Object) throw CreateInvalidOperationException(JsonValueKind.Object);
        throw new ArgumentOutOfRangeException(nameof(key), "key was outside the range of the JSON properties.");
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool IJsonValueNode.TryGetValue(string key, out IJsonValueNode result)
    {
        var v = TryGetValueOrNull(key);
        result = v ?? JsonValues.Undefined;
        return result.ValueKind != JsonValueKind.Undefined;
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool IJsonValueNode.TryGetValue(ReadOnlySpan<char> key, out IJsonValueNode result)
    {
        var v = TryGetValueOrNull(key);
        result = v ?? JsonValues.Undefined;
        return result.ValueKind != JsonValueKind.Undefined;
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <returns>The property value; or null, if non-exist.</returns>
    protected virtual BaseJsonValueNode TryGetValueOrNull(string key) => null;

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <returns>The property value; or null, if non-exist.</returns>
    protected virtual BaseJsonValueNode TryGetValueOrNull(ReadOnlySpan<char> key)
        => TryGetValueOrNull(key.ToString());

    /// <summary>
    /// Gets the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <returns>The value.</returns>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    IJsonValueNode IJsonValueNode.GetValue(int index)
    {
        if (index < 0) throw new ArgumentOutOfRangeException(nameof(index), "index should be equals to or greater than 0.");
        var v = TryGetValueOrNull(index);
        if (v is not null && v.ValueKind != JsonValueKind.Undefined) return v;
        if (ValueKind != JsonValueKind.Array) throw CreateInvalidOperationException(JsonValueKind.Array);
        throw new ArgumentOutOfRangeException(nameof(index), "index should be less than the item count of the array.");
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool IJsonValueNode.TryGetValue(int index, out IJsonValueNode result)
    {
        var v = TryGetValueOrNull(index);
        result = v ?? JsonValues.Undefined;
        return v.ValueKind != JsonValueKind.Undefined;
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <returns>The item; or null, if non-exist.</returns>
    protected virtual BaseJsonValueNode TryGetValueOrNull(int index) => null;

#if !NETFRAMEWORK
    /// <summary>
    /// Gets the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <returns>The value.</returns>
    /// <exception cref="InvalidOperationException">The value kind is not expected.</exception>
    IJsonValueNode IJsonValueNode.GetValue(Index index)
    {
        var v = TryGetValueOrNull(index);
        if (v is not null && v.ValueKind != JsonValueKind.Undefined) return v;
        if (ValueKind != JsonValueKind.Array) throw CreateInvalidOperationException(JsonValueKind.Array);
        throw new ArgumentOutOfRangeException(nameof(index), "index should be less than the item count of the array.");
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <returns>The item; or null, if non-exist.</returns>
    protected virtual BaseJsonValueNode TryGetValueOrNull(Index index)
        => TryGetValueOrNull(index.IsFromEnd ? (Count - index.Value) : index.Value);

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if the kind is the one expected; otherwise, false.</returns>
    bool IJsonValueNode.TryGetValue(Index index, out IJsonValueNode result)
    {
        try
        {
            result = TryGetValueOrNull(index.IsFromEnd ? (Count - index.Value) : index.Value);
            return result is not null && result.ValueKind != JsonValueKind.Undefined;
        }
        catch (ArgumentException)
        {
        }
        catch (InvalidOperationException)
        {
        }

        result = null;
        return false;
    }
#endif

    /// <summary>
    /// Converts to JSON node.
    /// </summary>
    /// <returns>An instance of the JSON node.</returns>
    public abstract JsonNode ToJsonNode();

    private string GetValueKindExceptionMessage(JsonValueKind expect)
        => string.Concat("Expect ", ConvertToWord(expect), " but it is ", ConvertToWord(ValueKind), ".");

    private InvalidOperationException CreateInvalidOperationException(JsonValueKind expect)
    {
        if (expect == ValueKind && ValueKind == JsonValueKind.Number)
        {
            var s = string.Concat("The number is overflow.");
            return new InvalidOperationException(s, new OverflowException(s));
        }
        else
        {
            var s = GetValueKindExceptionMessage(expect);
            return new InvalidOperationException(s, new InvalidCastException(s));
        }
    }

    private string GetValueKindExceptionMessage(string expect)
        => string.Concat("Expect ", expect, " but it is ", ConvertToWord(ValueKind), ".");

    private InvalidOperationException CreateInvalidOperationException(string expect, bool overflow = false)
    {
        if (overflow && ValueKind == JsonValueKind.Number)
        {
            var s = string.Concat("The number is overflow.");
            return new InvalidOperationException(s, new OverflowException(s));
        }
        else
        {
            var s = GetValueKindExceptionMessage(expect);
            return new InvalidOperationException(s, new InvalidCastException(s));
        }
    }

    private string ConvertToWord(JsonValueKind expect)
        => expect switch
        {
            JsonValueKind.String => "a string",
            JsonValueKind.Array => "an array",
            JsonValueKind.Object => "an object",
            JsonValueKind.True => "a boolean (true)",
            JsonValueKind.False => "a boolean (false)",
            JsonValueKind.Number => "a number",
            JsonValueKind.Null => "null",
            JsonValueKind.Undefined => "undefined",
            _ => "an unknown value"
        };

    TypeCode IConvertible.GetTypeCode()
        => ValueKind switch
        {
            JsonValueKind.Null or JsonValueKind.Undefined => TypeCode.Empty,
            JsonValueKind.Number => this is IJsonNumberNode i && i.IsInteger ? TypeCode.Int64 : TypeCode.Double,
            JsonValueKind.String => TypeCode.String,
            JsonValueKind.True or JsonValueKind.False => TypeCode.Boolean,
            _ => TypeCode.Object
        };

    bool IConvertible.ToBoolean(IFormatProvider provider)
        => As<bool>();

    byte IConvertible.ToByte(IFormatProvider provider)
        => As<byte>();

    char IConvertible.ToChar(IFormatProvider provider)
        => As<char>();

    DateTime IConvertible.ToDateTime(IFormatProvider provider)
        => As<DateTime>();

    decimal IConvertible.ToDecimal(IFormatProvider provider)
        => As<decimal>();

    double IConvertible.ToDouble(IFormatProvider provider)
        => As<double>();

    short IConvertible.ToInt16(IFormatProvider provider)
        => As<short>();

    int IConvertible.ToInt32(IFormatProvider provider)
        => As<int>();

    long IConvertible.ToInt64(IFormatProvider provider)
        => As<long>();

    sbyte IConvertible.ToSByte(IFormatProvider provider)
        => As<sbyte>();

    float IConvertible.ToSingle(IFormatProvider provider)
        => As<float>();

    string IConvertible.ToString(IFormatProvider provider)
        => As<string>();

    object IConvertible.ToType(Type conversionType, IFormatProvider provider)
        => As(conversionType);

    ushort IConvertible.ToUInt16(IFormatProvider provider)
        => As<ushort>();

    uint IConvertible.ToUInt32(IFormatProvider provider)
        => As<uint>();

    ulong IConvertible.ToUInt64(IFormatProvider provider)
        => As<ulong>();

    /// <summary>
    /// Converts to JSON value.
    /// </summary>
    /// <param name="value">The source value.</param>
    /// <returns>A JSON value.</returns>
    public static implicit operator BaseJsonValueNode(string value)
        => new JsonStringNode(value);

    /// <summary>
    /// Converts to JSON value.
    /// </summary>
    /// <param name="value">The source value.</param>
    /// <returns>A JSON value.</returns>
    public static implicit operator BaseJsonValueNode(ReadOnlySpan<char> value)
        => new JsonStringNode(value);

    /// <summary>
    /// Converts to JSON value.
    /// </summary>
    /// <param name="value">The source value.</param>
    /// <returns>A JSON value.</returns>
    public static implicit operator BaseJsonValueNode(char[] value)
        => new JsonStringNode(value);

    /// <summary>
    /// Converts to JSON value.
    /// </summary>
    /// <param name="value">The source value.</param>
    /// <returns>A JSON value.</returns>
    public static implicit operator BaseJsonValueNode(StringBuilder value)
        => new JsonStringNode(value);

    /// <summary>
    /// Converts to JSON value.
    /// </summary>
    /// <param name="value">The source value.</param>
    /// <returns>A JSON value.</returns>
    public static implicit operator BaseJsonValueNode(Guid value)
        => new JsonStringNode(value);

    /// <summary>
    /// Converts to JSON value.
    /// </summary>
    /// <param name="value">The source value.</param>
    /// <returns>A JSON value.</returns>
    public static implicit operator BaseJsonValueNode(ushort value)
        => new JsonIntegerNode(value);

    /// <summary>
    /// Converts to JSON value.
    /// </summary>
    /// <param name="value">The source value.</param>
    /// <returns>A JSON value.</returns>
    public static implicit operator BaseJsonValueNode(short value)
        => new JsonIntegerNode(value);

    /// <summary>
    /// Converts to JSON value.
    /// </summary>
    /// <param name="value">The source value.</param>
    /// <returns>A JSON value.</returns>
    public static implicit operator BaseJsonValueNode(uint value)
        => new JsonIntegerNode(value);

    /// <summary>
    /// Converts to JSON value.
    /// </summary>
    /// <param name="value">The source value.</param>
    /// <returns>A JSON value.</returns>
    public static implicit operator BaseJsonValueNode(int value)
        => new JsonIntegerNode(value);

    /// <summary>
    /// Converts to JSON value.
    /// </summary>
    /// <param name="value">The source value.</param>
    /// <returns>A JSON value.</returns>
    public static implicit operator BaseJsonValueNode(ulong value)
        => new JsonIntegerNode(value);

    /// <summary>
    /// Converts to JSON value.
    /// </summary>
    /// <param name="value">The source value.</param>
    /// <returns>A JSON value.</returns>
    public static implicit operator BaseJsonValueNode(long value)
        => new JsonIntegerNode(value);

    /// <summary>
    /// Converts to JSON value.
    /// </summary>
    /// <param name="value">The source value.</param>
    /// <returns>A JSON value.</returns>
    public static implicit operator BaseJsonValueNode(float value)
        => new JsonDoubleNode(value);

    /// <summary>
    /// Converts to JSON value.
    /// </summary>
    /// <param name="value">The source value.</param>
    /// <returns>A JSON value.</returns>
    public static implicit operator BaseJsonValueNode(double value)
        => new JsonDoubleNode(value);

    /// <summary>
    /// Converts to JSON value.
    /// </summary>
    /// <param name="value">The source value.</param>
    /// <returns>A JSON value.</returns>
    public static implicit operator BaseJsonValueNode(decimal value)
        => new JsonDecimalNode(value);

    /// <summary>
    /// Converts to JSON value.
    /// </summary>
    /// <param name="value">The JSON node.</param>
    /// <returns>A JSON value node.</returns>
    /// <exception cref="InvalidCastException">Not supported.</exception>
    public static implicit operator BaseJsonValueNode(JsonNode value)
    {
        if (value is null) return JsonValues.Null;
        if (value is JsonObject o) return (JsonObjectNode)o;
        if (value is JsonArray a) return (JsonArrayNode)a;
        if (value is not JsonValue token)
            throw new InvalidCastException($"Only supports JsonValue, JsonObject and JsonArray but its type is {value.GetType().Name}.");
#if !NET461 && !NET6_0
        switch (token.GetValueKind())
        {
            case JsonValueKind.Null:
                return JsonValues.Null;
            case JsonValueKind.Undefined:
                return JsonValues.Undefined;
            case JsonValueKind.String:
                if (token.TryGetValue(out string s)) return new JsonStringNode(s);
                break;
            case JsonValueKind.True:
                return JsonBooleanNode.True;
            case JsonValueKind.False:
                return JsonBooleanNode.False;
            case JsonValueKind.Number:
                if (token.TryGetValue(out int i)) return new JsonIntegerNode(i);
                if (token.TryGetValue(out uint ui)) return new JsonIntegerNode(ui);
                if (token.TryGetValue(out long l)) return new JsonIntegerNode(l);
                if (token.TryGetValue(out ulong ul)) return new JsonIntegerNode(ul);
                if (token.TryGetValue(out double d)) return new JsonDoubleNode(d);
                if (token.TryGetValue(out float f)) return new JsonDoubleNode(f);
                if (token.TryGetValue(out decimal de)) return new JsonDecimalNode(de);
                break;
            default:
                throw new InvalidCastException($"Expect a JSON value but it is {value.GetType().Name}.");
        }
#endif

        {
            if (token.TryGetValue(out string s))
                return new JsonStringNode(s);
            if (token.TryGetValue(out bool b))
                return b ? JsonBooleanNode.True : JsonBooleanNode.False;
            if (token.TryGetValue(out long l))
                return new JsonIntegerNode(l);
            if (token.TryGetValue(out int i))
                return new JsonIntegerNode(i);
            if (token.TryGetValue(out uint ui))
                return new JsonIntegerNode(ui);
            if (token.TryGetValue(out short sh))
                return new JsonIntegerNode(sh);
            if (token.TryGetValue(out ushort ush))
                return new JsonIntegerNode(ush);
            if (token.TryGetValue(out sbyte sb))
                return new JsonIntegerNode(sb);
            if (token.TryGetValue(out byte by))
                return new JsonIntegerNode(by);
            if (token.TryGetValue(out double d))
                return new JsonDoubleNode(d);
            if (token.TryGetValue(out float f))
                return new JsonDoubleNode(f);
            if (token.TryGetValue(out decimal de))
                return new JsonDecimalNode(de);
            if (token.TryGetValue(out Guid g))
                return new JsonStringNode(g);
            if (token.TryGetValue(out DateTime dt))
                return new JsonStringNode(dt);
            if (token.TryGetValue(out DateTimeOffset dto))
                return new JsonStringNode(dto);
            if (token.TryGetValue(out char c))
                return new JsonStringNode(c);
        }

        if (token.TryGetValue(out JsonElement e))
            return e;
        throw new InvalidCastException($"Only supports JsonValue, JsonObject and JsonArray but its type is {value.GetType().Name}.");
    }

    /// <summary>
    /// Converts from JSON element.
    /// </summary>
    /// <param name="json">The JSON element.</param>
    /// <returns>The JSON value node.</returns>
    /// <exception cref="InvalidCastException">Not supported.</exception>
    public static implicit operator BaseJsonValueNode(JsonElement json)
        => JsonValues.ToJsonValue(json) ?? throw new InvalidCastException($"The value kind {json.ValueKind} is not supported.");

    /// <summary>
    /// Converts from JSON document.
    /// </summary>
    /// <param name="json">The JSON document.</param>
    /// <returns>The JSON value node.</returns>
    /// <exception cref="InvalidCastException">Not supported.</exception>
    public static implicit operator BaseJsonValueNode(JsonDocument json)
        => json?.RootElement;

    /// <summary>
    /// Converts the JSON raw back.
    /// </summary>
    /// <param name="json">The JSON value.</param>
    /// <returns>A string.</returns>
    public static explicit operator string(BaseJsonValueNode json)
        => json?.Cast<string>();

    /// <summary>
    /// Converts the JSON raw back.
    /// </summary>
    /// <param name="json">The JSON value.</param>
    /// <returns>A string.</returns>
    public static explicit operator StringBuilder(BaseJsonValueNode json)
        => json?.Cast<StringBuilder>();

    /// <summary>
    /// Converts the JSON raw back.
    /// </summary>
    /// <param name="json">The JSON value.</param>
    /// <returns>A string.</returns>
    public static explicit operator byte(BaseJsonValueNode json)
        => json?.Cast<byte>() ?? 0;

    /// <summary>
    /// Converts the JSON raw back.
    /// </summary>
    /// <param name="json">The JSON value.</param>
    /// <returns>A string.</returns>
    public static explicit operator ushort(BaseJsonValueNode json)
        => json?.Cast<ushort>() ?? 0;

    /// <summary>
    /// Converts the JSON raw back.
    /// </summary>
    /// <param name="json">The JSON value.</param>
    /// <returns>A string.</returns>
    public static explicit operator short(BaseJsonValueNode json)
        => json?.Cast<short>() ?? 0;

    /// <summary>
    /// Converts the JSON raw back.
    /// </summary>
    /// <param name="json">The JSON value.</param>
    /// <returns>A string.</returns>
    public static explicit operator uint(BaseJsonValueNode json)
        => json?.Cast<uint>() ?? 0;

    /// <summary>
    /// Converts the JSON raw back.
    /// </summary>
    /// <param name="json">The JSON value.</param>
    /// <returns>A string.</returns>
    public static explicit operator int(BaseJsonValueNode json)
        => json?.Cast<int>() ?? 0;

    /// <summary>
    /// Converts the JSON raw back.
    /// </summary>
    /// <param name="json">The JSON value.</param>
    /// <returns>A string.</returns>
    public static explicit operator ulong(BaseJsonValueNode json)
        => json?.Cast<ulong>() ?? 0;

    /// <summary>
    /// Converts the JSON raw back.
    /// </summary>
    /// <param name="json">The JSON value.</param>
    /// <returns>A string.</returns>
    public static explicit operator long(BaseJsonValueNode json)
        => json?.Cast<long>() ?? 0L;

    /// <summary>
    /// Converts the JSON raw back.
    /// </summary>
    /// <param name="json">The JSON value.</param>
    /// <returns>A string.</returns>
    public static explicit operator decimal(BaseJsonValueNode json)
        => json?.Cast<decimal>() ?? decimal.Zero;

    /// <summary>
    /// Converts the JSON raw back.
    /// </summary>
    /// <param name="json">The JSON value.</param>
    /// <returns>A string.</returns>
    public static explicit operator float(BaseJsonValueNode json)
        => json?.Cast<float>() ?? float.NaN;

    /// <summary>
    /// Converts the JSON raw back.
    /// </summary>
    /// <param name="json">The JSON value.</param>
    /// <returns>A string.</returns>
    public static explicit operator double(BaseJsonValueNode json)
        => json?.Cast<double>() ?? double.NaN;

    /// <summary>
    /// Converts the JSON raw back.
    /// </summary>
    /// <param name="json">The JSON value.</param>
    /// <returns>A string.</returns>
    public static explicit operator DateTime(BaseJsonValueNode json)
        => json?.Cast<DateTime>() ?? throw new InvalidCastException();

    /// <summary>
    /// Converts the JSON raw back.
    /// </summary>
    /// <param name="json">The JSON value.</param>
    /// <returns>A string.</returns>
    public static explicit operator Guid(BaseJsonValueNode json)
        => json?.Cast<Guid>() ?? Guid.Empty;

    /// <summary>
    /// Converts the JSON raw back.
    /// </summary>
    /// <param name="json">The JSON value.</param>
    /// <returns>A string.</returns>
    public static explicit operator Uri(BaseJsonValueNode json)
        => json?.Cast<Uri>() ?? null;

    /// <summary>
    /// Converts to JSON node.
    /// </summary>
    /// <param name="json">The JSON value.</param>
    /// <returns>An instance of the JsonNode class.</returns>
    public static explicit operator System.Text.Json.Nodes.JsonNode(BaseJsonValueNode json)
        => json?.ToJsonNode();

    /// <summary>
    /// Compares two instances to indicate if they are same.
    /// leftValue == rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are same; otherwise, false.</returns>
    public static bool operator ==(BaseJsonValueNode leftValue, IJsonValueNode rightValue)
    {
        if (ReferenceEquals(leftValue, rightValue)) return true;
        return leftValue.Equals(rightValue);
    }

    /// <summary>
    /// Compares two instances to indicate if they are different.
    /// leftValue != rightValue
    /// </summary>
    /// <param name="leftValue">The left value to compare.</param>
    /// <param name="rightValue">The right value to compare.</param>
    /// <returns>true if they are different; otherwise, false.</returns>
    public static bool operator !=(BaseJsonValueNode leftValue, IJsonValueNode rightValue)
    {
        if (ReferenceEquals(leftValue, rightValue)) return false;
        return !leftValue.Equals(rightValue);
    }
}
