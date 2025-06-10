using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Trivial.Data;
using Trivial.IO;

namespace Trivial.Text;

/// <summary>
/// The host service for JSON object node to provide a way to access its JSON properties.
/// </summary>
public class BaseJsonObjectHostService : IJsonObjectHost
{
    private readonly Dictionary<string, object> cache = new();

    /// <summary>
    /// Initializes a new instance of the BaseJsonObjectHostService class.
    /// </summary>
    /// <param name="source">The JSON object node source.</param>
    public BaseJsonObjectHostService(JsonObjectNode source)
    {
        Source = source ?? new();
        source.PropertyChanged += OnPropertyChanged;
    }

    /// <summary>
    /// Gets the JSON object source.
    /// </summary>
    protected JsonObjectNode Source { get; private set; }

    /// <summary>
    /// Tries to get the specific value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="key">The property key.</param>
    /// <returns>The result.</returns>
    protected T TryGetValue<T>(string key) where T : class
        => TryGetValue(key, false, out T value) ? value : default;

    /// <summary>
    /// Tries to get the specific value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="key">The property key.</param>
    /// <param name="reload">true if reload the value of the property; otherwise, false.</param>
    /// <returns>The result.</returns>
    protected T TryGetValue<T>(string key, bool reload) where T : class
        => TryGetValue(key, reload, out T value) ? value : default;

    /// <summary>
    /// Tries to get the specific value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="key">The property key.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if get succeeded; otherwise, false.</returns>
    protected bool TryGetValue<T>(string key, out T result) where T : class
        => TryGetValue(key, false, default, out result);

    /// <summary>
    /// Tries to get the specific value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="key">The property key.</param>
    /// <param name="reload">true if reload the value of the property; otherwise, false.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if get succeeded; otherwise, false.</returns>
    protected bool TryGetValue<T>(string key, bool reload, out T result) where T : class
        => TryGetValue(key, reload, default, out result);

    /// <summary>
    /// Tries to get the specific value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="key">The property key.</param>
    /// <param name="reload">true if reload the value of the property; otherwise, false.</param>
    /// <param name="defaultValue">The default value for null.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if get succeeded; otherwise, false.</returns>
    protected bool TryGetValue<T>(string key, bool reload, T defaultValue, out T result) where T : class
    {
        try
        {
            if (!reload && cache.TryGetValue(key, out var value))
            {
                if (value is T v)
                {
                    result = v;
                    return true;
                }

                if (value is null)
                {
                    result = defaultValue;
                    return true;
                }
            }
        }
        catch (InvalidOperationException)
        {
        }
        catch (ExternalException)
        {
        }

        var json = Source.TryGetObjectValue(key);
        if (json is null)
        {
            if (reload) cache[key] = defaultValue;
            result = defaultValue;
            return true;
        }

        try
        {
            var v = CreateObject<T>(json);
            cache[key] = v;
            result = v;
            return true;
        }
        catch (ArgumentException)
        {
        }
        catch (InvalidOperationException)
        {
        }
        catch (InvalidCastException)
        {
        }
        catch (NullReferenceException)
        {
        }
        catch (NotSupportedException)
        {
        }
        catch (JsonException)
        {
        }
        catch (ApplicationException)
        {
        }
        catch (ExternalException)
        {
        }

        result = defaultValue;
        return false;
    }

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <returns>A string.</returns>
    protected string TryGetStringValue(string key)
        => Source.TryGetStringValue(key);

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="strictMode">true if enable strict mode; otherwise, false, to return undefined for non-existing.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
    protected bool TryGetStringValue(string key, bool strictMode, out string result)
        => Source.TryGetStringValue(key, strictMode, out result);

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="result">The result.</param>
    /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
    protected bool TryGetStringValue(string key, out string result)
        => Source.TryGetStringValue(key, false, out result);

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="returnNullIfEmpty">true if returns null when the value is empty or white space; otherwise, false.</param>
    /// <returns>A string trimmed.</returns>
    protected string TryGetStringTrimmedValue(string key, bool returnNullIfEmpty = false)
        => Source.TryGetStringTrimmedValue(key, returnNullIfEmpty);

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="valueCase">The case.</param>
    /// <param name="invariant">true if uses the casing rules of invariant culture; otherwise, false.</param>
    /// <param name="returnNullIfEmpty">true if returns null when the value is empty or white space; otherwise, false.</param>
    /// <returns>A string trimmed.</returns>
    protected string TryGetStringTrimmedValue(string key, Cases valueCase, bool invariant = false, bool returnNullIfEmpty = false)
        => Source.TryGetStringTrimmedValue(key, valueCase, invariant, returnNullIfEmpty);

    /// <summary>
    /// Tries to get the value of the specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="result">The result trimmed.</param>
    /// <returns>true if has the property and the type is the one expected; otherwise, false.</returns>
    protected bool TryGetStringTrimmedValue(string key, out string result)
        => Source.TryGetStringTrimmedValue(key, out result);

    /// <summary>
    /// Gets the value of the specific key of the property.
    /// </summary>
    /// <typeparam name="T">The type of value to convert.</typeparam>
    /// <param name="defaultValue">The default value if get failed.</param>
    /// <param name="key">The property key. The default key is the property name uncapitalized.</param>
    /// <returns>The value of the property.</returns>
    protected T GetCurrentValue<T>(T defaultValue = default, [CallerMemberName] string key = null)
    {
        var path = new[] { key?.ToSpecificCase(Cases.Uncapitalize) };
        if (Source.TryGetValue<T>(path, out var r)) return r;
        return defaultValue;
    }

    /// <summary>
    /// Gets the value of the specific key of the property.
    /// </summary>
    /// <param name="key">The property key. The default key is the property name uncapitalized.</param>
    /// <returns>The value of the property.</returns>
    protected string GetCurrentStringValue([CallerMemberName] string key = null)
        => Source.TryGetStringValue(key?.ToSpecificCase(Cases.Uncapitalize));

    /// <summary>
    /// Gets the value of the specific key of the property.
    /// </summary>
    /// <param name="key">The property key. The default key is the property name uncapitalized.</param>
    /// <param name="returnNullIfEmpty">true if returns null when the value is empty or white space; otherwise, false.</param>
    /// <returns>The value of the property.</returns>
    protected string GetCurrentStringTrimmedValue(bool returnNullIfEmpty = false, [CallerMemberName] string key = null)
        => Source.TryGetStringTrimmedValue(key?.ToSpecificCase(Cases.Uncapitalize), returnNullIfEmpty);

    /// <summary>
    /// Gets the value of the specific key of the property.
    /// </summary>
    /// <param name="key">The property key. The default key is the property name uncapitalized.</param>
    /// <returns>The value of the property.</returns>
    protected List<string> GetCurrentStringListValue([CallerMemberName] string key = null)
        => Source.TryGetStringListValue(key?.ToSpecificCase(Cases.Uncapitalize));

    /// <summary>
    /// Gets the value of the specific key of the property.
    /// </summary>
    /// <param name="key">The property key. The default key is the property name uncapitalized.</param>
    /// <returns>The value of the property.</returns>
    protected bool? GetCurrentBooleanValue([CallerMemberName] string key = null)
        => Source.TryGetBooleanValue(key?.ToSpecificCase(Cases.Uncapitalize));

    /// <summary>
    /// Gets the value of the specific key of the property.
    /// </summary>
    /// <param name="key">The property key. The default key is the property name uncapitalized.</param>
    /// <returns>The value of the property.</returns>
    protected int? GetCurrentInt32Value([CallerMemberName] string key = null)
        => Source.TryGetInt32Value(key?.ToSpecificCase(Cases.Uncapitalize));

    /// <summary>
    /// Gets the value of the specific key of the property.
    /// </summary>
    /// <param name="key">The property key. The default key is the property name uncapitalized.</param>
    /// <returns>The value of the property.</returns>
    protected long? GetCurrentInt64Value([CallerMemberName] string key = null)
        => Source.TryGetInt64Value(key?.ToSpecificCase(Cases.Uncapitalize));

    /// <summary>
    /// Gets the value of the specific key of the property.
    /// </summary>
    /// <param name="key">The property key. The default key is the property name uncapitalized.</param>
    /// <returns>The value of the property.</returns>
    protected uint? GetCurrentUInt32Value([CallerMemberName] string key = null)
        => Source.TryGetUInt32Value(key?.ToSpecificCase(Cases.Uncapitalize));

    /// <summary>
    /// Gets the value of the specific key of the property.
    /// </summary>
    /// <param name="key">The property key. The default key is the property name uncapitalized.</param>
    /// <returns>The value of the property.</returns>
    protected short? GetCurrentInt16Value([CallerMemberName] string key = null)
        => Source.TryGetInt16Value(key?.ToSpecificCase(Cases.Uncapitalize));

    /// <summary>
    /// Gets the value of the specific key of the property.
    /// </summary>
    /// <param name="key">The property key. The default key is the property name uncapitalized.</param>
    /// <returns>The value of the property.</returns>
    protected ushort? GetCurrentUInt16Value([CallerMemberName] string key = null)
        => Source.TryGetUInt16Value(key?.ToSpecificCase(Cases.Uncapitalize));

    /// <summary>
    /// Gets the value of the specific key of the property.
    /// </summary>
    /// <param name="key">The property key. The default key is the property name uncapitalized.</param>
    /// <returns>The value of the property.</returns>
    protected float? GetCurrentSingleValue([CallerMemberName] string key = null)
        => Source.TryGetSingleValue(key?.ToSpecificCase(Cases.Uncapitalize));

    /// <summary>
    /// Gets the value of the specific key of the property.
    /// </summary>
    /// <param name="defaultValue">The default value.</param>
    /// <param name="key">The property key. The default key is the property name uncapitalized.</param>
    /// <returns>The value of the property.</returns>
    protected float GetCurrentSingleValue(float defaultValue, [CallerMemberName] string key = null)
        => Source.TryGetSingleValue(key?.ToSpecificCase(Cases.Uncapitalize), defaultValue);

    /// <summary>
    /// Gets the value of the specific key of the property.
    /// </summary>
    /// <param name="defaultIsZero">true if returns zero for default or getting failure; otherwise, false, to return NaN.</param>
    /// <param name="key">The property key. The default key is the property name uncapitalized.</param>
    /// <returns>The value of the property.</returns>
    protected float GetCurrentSingleValue(bool defaultIsZero, [CallerMemberName] string key = null)
        => Source.TryGetSingleValue(key?.ToSpecificCase(Cases.Uncapitalize), defaultIsZero);

    /// <summary>
    /// Gets the value of the specific key of the property.
    /// </summary>
    /// <param name="key">The property key. The default key is the property name uncapitalized.</param>
    /// <returns>The value of the property.</returns>
    protected double? GetCurrentDoubleValue([CallerMemberName] string key = null)
        => Source.TryGetDoubleValue(key?.ToSpecificCase(Cases.Uncapitalize));

    /// <summary>
    /// Gets the value of the specific key of the property.
    /// </summary>
    /// <param name="defaultValue">The default value.</param>
    /// <param name="key">The property key. The default key is the property name uncapitalized.</param>
    /// <returns>The value of the property.</returns>
    protected double GetCurrentDoubleValue(double defaultValue, [CallerMemberName] string key = null)
        => Source.TryGetDoubleValue(key?.ToSpecificCase(Cases.Uncapitalize), defaultValue);

    /// <summary>
    /// Gets the value of the specific key of the property.
    /// </summary>
    /// <param name="defaultIsZero">true if returns zero for default or getting failure; otherwise, false, to return NaN.</param>
    /// <param name="key">The property key. The default key is the property name uncapitalized.</param>
    /// <returns>The value of the property.</returns>
    protected double GetCurrentDoubleValue(bool defaultIsZero, [CallerMemberName] string key = null)
        => Source.TryGetDoubleValue(key?.ToSpecificCase(Cases.Uncapitalize), defaultIsZero);

    /// <summary>
    /// Gets the value of the specific key of the property.
    /// </summary>
    /// <param name="key">The property key. The default key is the property name uncapitalized.</param>
    /// <returns>The value of the property.</returns>
    protected decimal? GetCurrentDecimalValue([CallerMemberName] string key = null)
        => Source.TryGetDecimalValue(key?.ToSpecificCase(Cases.Uncapitalize));

    /// <summary>
    /// Gets the value of the specific key of the property.
    /// </summary>
    /// <param name="key">The property key. The default key is the property name uncapitalized.</param>
    /// <returns>The value of the property.</returns>
    protected DateTime? GetCurrentDateTimeValue([CallerMemberName] string key = null)
        => Source.TryGetDateTimeValue(key?.ToSpecificCase(Cases.Uncapitalize));

    /// <summary>
    /// Gets the value of the specific key of the property.
    /// </summary>
    /// <param name="useUnixTimestampsFallback">true if use Unix timestamp to convert if the value is a number; otherwise, false, to use JavaScript date ticks fallback.</param>
    /// <param name="key">The property key. The default key is the property name uncapitalized.</param>
    /// <returns>The value of the property.</returns>
    protected DateTime? GetCurrentDateTimeValue(bool useUnixTimestampsFallback, [CallerMemberName] string key = null)
        => Source.TryGetDateTimeValue(key?.ToSpecificCase(Cases.Uncapitalize), useUnixTimestampsFallback);

    /// <summary>
    /// Gets the value of the specific key of the property.
    /// </summary>
    /// <param name="key">The property key. The default key is the property name uncapitalized.</param>
    /// <returns>The value of the property.</returns>
    protected Guid? GetCurrentGuidValue([CallerMemberName] string key = null)
        => Source.TryGetGuidValue(key?.ToSpecificCase(Cases.Uncapitalize));

    /// <summary>
    /// Gets the value of the specific key of the property.
    /// </summary>
    /// <param name="key">The property key. The default key is the property name uncapitalized.</param>
    /// <returns>The value of the property.</returns>
    protected Uri GetCurrentUriValue([CallerMemberName] string key = null)
        => Source.TryGetUriValue(key?.ToSpecificCase(Cases.Uncapitalize));

    /// <summary>
    /// Gets the value of the specific key of the property.
    /// </summary>
    /// <param name="key">The property key. The default key is the property name uncapitalized.</param>
    /// <returns>The value of the property.</returns>
    protected T? GetCurrentEnumValue<T>([CallerMemberName] string key = null) where T : struct, Enum
        => Source.TryGetEnumValue<T>(key?.ToSpecificCase(Cases.Uncapitalize));

    /// <summary>
    /// Gets the value of the specific key of the property.
    /// </summary>
    /// <param name="key">The property key. The default key is the property name uncapitalized.</param>
    /// <param name="ignoreCase">true if ignore case; otherwise, false.</param>
    /// <returns>The value of the property.</returns>
    protected T? GetCurrentEnumValue<T>(bool ignoreCase, [CallerMemberName] string key = null) where T : struct, Enum
        => Source.TryGetEnumValue<T>(key?.ToSpecificCase(Cases.Uncapitalize), ignoreCase);

    /// <summary>
    /// Gets the value of the specific key of the property.
    /// </summary>
    /// <param name="key">The property key. The default key is the property name uncapitalized.</param>
    /// <returns>The value of the property.</returns>
    protected JsonObjectNode GetCurrentObjectValue([CallerMemberName] string key = null)
        => Source.TryGetObjectValue(key?.ToSpecificCase(Cases.Uncapitalize));

    /// <summary>
    /// Gets the value of the specific key of the property.
    /// </summary>
    /// <param name="key">The property key. The default key is the property name uncapitalized.</param>
    /// <returns>The value of the property.</returns>
    protected JsonArrayNode GetCurrentArrayValue([CallerMemberName] string key = null)
        => Source.TryGetArrayValue(key?.ToSpecificCase(Cases.Uncapitalize));

    /// <summary>
    /// Gets the value of the specific key of the property.
    /// </summary>
    /// <param name="key">The property key. The default key is the property name uncapitalized.</param>
    /// <returns>The value of the property.</returns>
    protected List<JsonObjectNode> GetCurrentObjectListValue([CallerMemberName] string key = null)
        => Source.TryGetObjectListValue(key?.ToSpecificCase(Cases.Uncapitalize));

    /// <summary>
    /// Sets the current value to source node.
    /// </summary>
    /// <remarks>This method should be called directly in the setter of a property of the class.</remarks>
    /// <param name="value">The value to set.</param>
    /// <param name="key">The property key. The default key is the property name uncapitalized.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null.</exception>
    /// <exception cref="ArgumentException">The property key should not be empty or consists only of white-space characters.</exception>
    protected void SetCurrentValue(string value, [CallerMemberName] string key = null)
        => Source.SetValue(key?.ToSpecificCase(Cases.Uncapitalize), value);

    /// <summary>
    /// Sets the current value to source node.
    /// </summary>
    /// <remarks>This method should be called directly in the setter of a property of the class.</remarks>
    /// <param name="value">The value to set.</param>
    /// <param name="key">The property key. The default key is the property name uncapitalized.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null.</exception>
    /// <exception cref="ArgumentException">The property key should not be empty or consists only of white-space characters.</exception>
    protected void SetCurrentValue(StringBuilder value, [CallerMemberName] string key = null)
        => Source.SetValue(key?.ToSpecificCase(Cases.Uncapitalize), value);

    /// <summary>
    /// Sets the current value to source node.
    /// </summary>
    /// <remarks>This method should be called directly in the setter of a property of the class.</remarks>
    /// <param name="value">The value to set.</param>
    /// <param name="key">The property key. The default key is the property name uncapitalized.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null.</exception>
    /// <exception cref="ArgumentException">The property key should not be empty or consists only of white-space characters.</exception>
    protected void SetCurrentValue(IEnumerable<string> value, [CallerMemberName] string key = null)
        => Source.SetValue(key?.ToSpecificCase(Cases.Uncapitalize), value);

    /// <summary>
    /// Sets the current value to source node.
    /// </summary>
    /// <remarks>This method should be called directly in the setter of a property of the class.</remarks>
    /// <param name="value">The value to set.</param>
    /// <param name="key">The property key. The default key is the property name uncapitalized.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null.</exception>
    /// <exception cref="ArgumentException">The property key should not be empty or consists only of white-space characters.</exception>
    protected void SetCurrentValue(bool value, [CallerMemberName] string key = null)
        => Source.SetValue(key?.ToSpecificCase(Cases.Uncapitalize), value);

    /// <summary>
    /// Sets the current value to source node.
    /// </summary>
    /// <remarks>This method should be called directly in the setter of a property of the class.</remarks>
    /// <param name="value">The value to set.</param>
    /// <param name="key">The property key. The default key is the property name uncapitalized.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null.</exception>
    /// <exception cref="ArgumentException">The property key should not be empty or consists only of white-space characters.</exception>
    protected void SetCurrentValue(int value, [CallerMemberName] string key = null)
        => Source.SetValue(key?.ToSpecificCase(Cases.Uncapitalize), value);

    /// <summary>
    /// Sets the current value to source node.
    /// </summary>
    /// <remarks>This method should be called directly in the setter of a property of the class.</remarks>
    /// <param name="value">The value to set.</param>
    /// <param name="key">The property key. The default key is the property name uncapitalized.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null.</exception>
    /// <exception cref="ArgumentException">The property key should not be empty or consists only of white-space characters.</exception>
    protected void SetCurrentValue(long value, [CallerMemberName] string key = null)
        => Source.SetValue(key?.ToSpecificCase(Cases.Uncapitalize), value);

    /// <summary>
    /// Sets the current value to source node.
    /// </summary>
    /// <remarks>This method should be called directly in the setter of a property of the class.</remarks>
    /// <param name="value">The value to set.</param>
    /// <param name="key">The property key. The default key is the property name uncapitalized.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null.</exception>
    /// <exception cref="ArgumentException">The property key should not be empty or consists only of white-space characters.</exception>
    protected void SetCurrentValue(float value, [CallerMemberName] string key = null)
        => Source.SetValue(key?.ToSpecificCase(Cases.Uncapitalize), value);

    /// <summary>
    /// Sets the current value to source node.
    /// </summary>
    /// <remarks>This method should be called directly in the setter of a property of the class.</remarks>
    /// <param name="value">The value to set.</param>
    /// <param name="key">The property key. The default key is the property name uncapitalized.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null.</exception>
    /// <exception cref="ArgumentException">The property key should not be empty or consists only of white-space characters.</exception>
    protected void SetCurrentValue(double value, [CallerMemberName] string key = null)
        => Source.SetValue(key?.ToSpecificCase(Cases.Uncapitalize), value);

    /// <summary>
    /// Sets the current value to source node.
    /// </summary>
    /// <remarks>This method should be called directly in the setter of a property of the class.</remarks>
    /// <param name="value">The value to set.</param>
    /// <param name="key">The property key. The default key is the property name uncapitalized.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null.</exception>
    /// <exception cref="ArgumentException">The property key should not be empty or consists only of white-space characters.</exception>
    protected void SetCurrentValue(decimal value, [CallerMemberName] string key = null)
        => Source.SetValue(key?.ToSpecificCase(Cases.Uncapitalize), value);

    /// <summary>
    /// Sets the current value to source node.
    /// </summary>
    /// <remarks>This method should be called directly in the setter of a property of the class.</remarks>
    /// <param name="value">The value to set.</param>
    /// <param name="key">The property key. The default key is the property name uncapitalized.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null.</exception>
    /// <exception cref="ArgumentException">The property key should not be empty or consists only of white-space characters.</exception>
    protected void SetCurrentValue(uint value, [CallerMemberName] string key = null)
        => Source.SetValue(key?.ToSpecificCase(Cases.Uncapitalize), value);

    /// <summary>
    /// Sets the current value to source node.
    /// </summary>
    /// <remarks>This method should be called directly in the setter of a property of the class.</remarks>
    /// <param name="value">The value to set.</param>
    /// <param name="key">The property key. The default key is the property name uncapitalized.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null.</exception>
    /// <exception cref="ArgumentException">The property key should not be empty or consists only of white-space characters.</exception>
    protected void SetCurrentValue(short value, [CallerMemberName] string key = null)
        => Source.SetValue(key?.ToSpecificCase(Cases.Uncapitalize), value);

    /// <summary>
    /// Sets the current value to source node.
    /// </summary>
    /// <remarks>This method should be called directly in the setter of a property of the class.</remarks>
    /// <param name="value">The value to set.</param>
    /// <param name="key">The property key. The default key is the property name uncapitalized.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null.</exception>
    /// <exception cref="ArgumentException">The property key should not be empty or consists only of white-space characters.</exception>
    protected void SetCurrentValue(ushort value, [CallerMemberName] string key = null)
        => Source.SetValue(key?.ToSpecificCase(Cases.Uncapitalize), value);

    /// <summary>
    /// Sets the current value to source node.
    /// </summary>
    /// <remarks>This method should be called directly in the setter of a property of the class.</remarks>
    /// <param name="value">The value to set.</param>
    /// <param name="key">The property key. The default key is the property name uncapitalized.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null.</exception>
    /// <exception cref="ArgumentException">The property key should not be empty or consists only of white-space characters.</exception>
    protected void SetCurrentValue(DateTime value, [CallerMemberName] string key = null)
        => Source.SetValue(key?.ToSpecificCase(Cases.Uncapitalize), value);

    /// <summary>
    /// Sets the current value to source node.
    /// </summary>
    /// <remarks>This method should be called directly in the setter of a property of the class.</remarks>
    /// <param name="value">The value to set.</param>
    /// <param name="key">The property key. The default key is the property name uncapitalized.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null.</exception>
    /// <exception cref="ArgumentException">The property key should not be empty or consists only of white-space characters.</exception>
    protected void SetCurrentValue(Guid value, [CallerMemberName] string key = null)
        => Source.SetValue(key?.ToSpecificCase(Cases.Uncapitalize), value);

    /// <summary>
    /// Sets the current value to source node.
    /// </summary>
    /// <remarks>This method should be called directly in the setter of a property of the class.</remarks>
    /// <param name="value">The value to set.</param>
    /// <param name="key">The property key. The default key is the property name uncapitalized.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null.</exception>
    /// <exception cref="ArgumentException">The property key should not be empty or consists only of white-space characters.</exception>
    protected void SetCurrentValue(Uri value, [CallerMemberName] string key = null)
        => Source.SetValue(key?.ToSpecificCase(Cases.Uncapitalize), value);

    /// <summary>
    /// Sets the current value to source node.
    /// </summary>
    /// <remarks>This method should be called directly in the setter of a property of the class.</remarks>
    /// <param name="value">The value to set.</param>
    /// <param name="key">The property key. The default key is the property name uncapitalized.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null.</exception>
    /// <exception cref="ArgumentException">The property key should not be empty or consists only of white-space characters.</exception>
    protected void SetCurrentValue(JsonObjectNode value, [CallerMemberName] string key = null)
        => Source.SetValue(key?.ToSpecificCase(Cases.Uncapitalize), value);

    /// <summary>
    /// Sets the current value to source node.
    /// </summary>
    /// <remarks>This method should be called directly in the setter of a property of the class.</remarks>
    /// <param name="value">The value to set.</param>
    /// <param name="key">The property key. The default key is the property name uncapitalized.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null.</exception>
    /// <exception cref="ArgumentException">The property key should not be empty or consists only of white-space characters.</exception>
    protected void SetCurrentValue(JsonArrayNode value, [CallerMemberName] string key = null)
        => Source.SetValue(key?.ToSpecificCase(Cases.Uncapitalize), value);

    /// <summary>
    /// Sets the current value to source node.
    /// </summary>
    /// <remarks>This method should be called directly in the setter of a property of the class.</remarks>
    /// <param name="value">The value to set.</param>
    /// <param name="key">The property key. The default key is the property name uncapitalized.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null.</exception>
    /// <exception cref="ArgumentException">The property key should not be empty or consists only of white-space characters.</exception>
    protected void SetCurrentValue(IEnumerable<JsonObjectNode> value, [CallerMemberName] string key = null)
        => Source.SetValue(key?.ToSpecificCase(Cases.Uncapitalize), value);

    /// <summary>
    /// Removes the current value to source node.
    /// </summary>
    /// <remarks>This method should be called directly in the setter of a property of the class.</remarks>
    /// <param name="key">The property key. The default key is the property name uncapitalized.</param>
    /// <exception cref="ArgumentNullException">The property key should not be null.</exception>
    /// <exception cref="ArgumentException">The property key should not be empty or consists only of white-space characters.</exception>
    protected void SetCurrentNullValue([CallerMemberName] string key = null)
        => Source.SetNullValue(key?.ToSpecificCase(Cases.Uncapitalize));

    /// <summary>
    /// Synchronizes back to source node.
    /// </summary>
    /// <returns>The properties synchronized.</returns>
    protected int SyncToParent()
    {
        var col = new Dictionary<string, object>(cache);
        foreach (var item in col)
        {
            var json = JsonObjectNode.ConvertFrom(item.Value);
            Source.SetValue(item.Key, json);
            cache[item.Key] = item.Value;
        }

        return col.Count;
    }

    /// <summary>
    /// Clears cache.
    /// </summary>
    protected void ClearCache()
        => cache.Clear();

    /// <summary>
    /// Resets the source JSON object node.
    /// </summary>
    /// <param name="parent">The source JSON object node.</param>
    /// <param name="keepCache">true if need keeping the cache; otherwise, false.</param>
    protected void ResetParent(JsonObjectNode parent, bool keepCache = false)
    {
        var oldParent = Source;
        if (ReferenceEquals(parent, oldParent))
        {
            if (keepCache) SyncToParent();
            return;
        }

        if (oldParent != null) oldParent.PropertyChanged -= OnPropertyChanged;
        parent ??= new();
        Source = parent;
        if (keepCache) SyncToParent();
        else cache.Clear();
        parent.PropertyChanged += OnPropertyChanged;
    }

    /// <summary>
    /// Tries to load from a file.
    /// </summary>
    /// <param name="file">A file with JSON object string content to parse.</param>
    /// <param name="options">Options to control the reader behavior during parsing.</param>
    /// <returns>true if load succeded; otherwise, false.</returns>
    protected bool TryLoadFile(FileInfo file, JsonDocumentOptions options = default)
    {
        var json = JsonObjectNode.TryParse(file, options);
        if (json == null) return false;
        ResetParent(json);
        return true;
    }

    /// <summary>
    /// Tries to load from a file.
    /// </summary>
    /// <param name="path">The file path with JSON object string content to parse.</param>
    /// <param name="options">Options to control the reader behavior during parsing.</param>
    /// <returns>true if load succeded; otherwise, false.</returns>
    protected bool TryLoadFile(string path, JsonDocumentOptions options = default)
    {
        var file = FileSystemInfoUtility.TryGetFileInfo(path);
        return TryLoadFile(file, options);
    }

    /// <summary>
    /// Writes to file.
    /// </summary>
    /// <param name="path">The path of the file. If the target file already exists, it is overwritten.</param>
    /// <param name="style">The indent style.</param>
    /// <exception cref="IOException">IO exception.</exception>
    /// <exception cref="SecurityException">Write failed because of security exception.</exception>
    /// <exception cref="ArgumentException">path was invalid..</exception>
    /// <exception cref="ArgumentNullException">path was null.</exception>
    /// <exception cref="NotSupportedException">path was not supported.</exception>
    /// <exception cref="UnauthorizedAccessException">Write failed because of unauthorized access exception.</exception>
    protected void WriteTo(string path, IndentStyles style = IndentStyles.Minified)
    {
        SyncToParent();
        Source.WriteTo(path, style);
    }

    /// <summary>
    /// Writes to file.
    /// </summary>
    /// <param name="file">The file to write.</param>
    /// <param name="style">The indent style.</param>
    /// <exception cref="IOException">IO exception.</exception>
    /// <exception cref="SecurityException">Write failed because of security exception.</exception>
    /// <exception cref="ArgumentNullException">The file path was null.</exception>
    /// <exception cref="NotSupportedException">The file path was not supported.</exception>
    /// <exception cref="UnauthorizedAccessException">Write failed because of unauthorized access exception.</exception>
    protected void WriteTo(FileInfo file, IndentStyles style = IndentStyles.Minified)
    {
        SyncToParent();
        Source.WriteTo(file, style);
    }

    /// <summary>
    /// Tries to write to a file.
    /// </summary>
    /// <param name="path">The path of the file. If the target file already exists, it is overwritten.</param>
    /// <param name="style">The indent style.</param>
    /// <returns>true if write succeeded; otherwise, false.</returns>
    protected bool TryWriteTo(string path, IndentStyles style = IndentStyles.Minified)
    {
        try
        {
            SyncToParent();
            Source.WriteTo(path, style);
            return true;
        }
        catch (ArgumentException)
        {
        }
        catch (InvalidOperationException)
        {
        }
        catch (IOException)
        {
        }
        catch (JsonException)
        {
        }
        catch (NotSupportedException)
        {
        }
        catch (NotImplementedException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
        catch (SecurityException)
        {
        }
        catch (ApplicationException)
        {
        }
        catch (ExternalException)
        {
        }

        return false;
    }

    /// <summary>
    /// Tries to write to a file.
    /// </summary>
    /// <param name="file">The file to save.</param>
    /// <param name="style">The indent style.</param>
    /// <returns>true if write succeeded; otherwise, false.</returns>
    protected bool TryWriteTo(FileInfo file, IndentStyles style = IndentStyles.Minified)
    {
        try
        {
            SyncToParent();
            Source.WriteTo(file, style);
            return true;
        }
        catch (ArgumentException)
        {
        }
        catch (InvalidOperationException)
        {
        }
        catch (IOException)
        {
        }
        catch (JsonException)
        {
        }
        catch (NotSupportedException)
        {
        }
        catch (NotImplementedException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
        catch (SecurityException)
        {
        }
        catch (ApplicationException)
        {
        }
        catch (ExternalException)
        {
        }

        return false;
    }

    /// <summary>
    /// Creates the object from a JSON object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="json"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    protected virtual T CreateObject<T>(JsonObjectNode json)
    {
        if (json == null) return default;
        var type = typeof(T);
        if (type.IsInterface) throw new NotSupportedException();
        var constructor = type.GetConstructor(new[] { typeof(JsonObjectNode) });
        if (constructor != null) return (T)constructor.Invoke(new[] { json });
        return json.Deserialize<T>();
    }

    /// <summary>
    /// The host for JSON object node.
    /// </summary>
    JsonObjectNode IJsonObjectHost.ToJson()
        => Source;

    private void OnPropertyChanged(object sender, KeyValueEventArgs<string, BaseJsonValueNode> e)
    {
        var key = e.Key;
        if (string.IsNullOrEmpty(key)) return;
        cache.Remove(key);
    }
}
