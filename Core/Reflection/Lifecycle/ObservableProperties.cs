using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Security;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Trivial.Data;
using Trivial.Text;
using Trivial.Web;

namespace Trivial.Reflection;

/// <summary>
/// The policies used to set property.
/// </summary>
[Description("The policies about if the properties are allowed to set values.")]
public enum PropertySettingPolicies
{
    /// <summary>
    /// Writable property.
    /// </summary>
    [Description("The properties are writable.")]
    Allow = 0,

    /// <summary>
    /// Read-only property but skip error when set value.
    /// </summary>
    [Description("The properties are read-only. Any update will be ignored.")]
    Skip = 1,

    /// <summary>
    /// Read-only property and require to throw an exception when set value.
    /// </summary>
    [Description("The properties are read-only. Any update will occur an exception thrown.")]
    Forbidden = 2
}

/// <summary>
/// Base model with observable properties.
/// </summary>
/// <example>
/// <code>
/// public class TestModel : BaseObservableProperties
/// {
///     public string Name
///     {
///         get => GetCurrentProperty&lt;string&gt;();
///         set => SetCurrentProperty(value);
///     }
/// }
/// </code>
/// <code>
/// var m = new TestModel();
/// m.PropertyChanged += (sender, e) => Console.WriteLine($"Property {e.PropertyName} changed.");
/// m.Name = "Programming";
/// </code>
/// </example>
public abstract class BaseObservableProperties : INotifyPropertyChanged
{
    /// <summary>
    /// Data cache.
    /// </summary>
    private readonly Dictionary<string, object> cache = new();

    /// <summary>
    /// The property changed event handler field..
    /// </summary>
    private event PropertyChangedEventHandler propertyChanged;

    /// <summary>
    /// Adds or removes the event handler raised on property changed.
    /// </summary>
    event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
    {
        add => propertyChanged += value;
        remove => propertyChanged -= value;
    }

    /// <summary>
    /// Adds or removes the event handler raised on property changed.
    /// </summary>
    public event ChangeEventHandler<object> PropertyChanged;

    /// <summary>
    /// Gets the revision token of member-wised property updated.
    /// </summary>
    [JsonIgnore]
    protected object RevisionToken { get; private set; } = new();

    /// <summary>
    /// Gets an enumerable collection that contains the keys in this instance.
    /// </summary>
    [JsonIgnore]
    protected IEnumerable<string> Keys => cache.Keys;

    /// <summary>
    /// Gets or sets the policy used to set property value.
    /// </summary>
    [JsonIgnore]
    protected PropertySettingPolicies PropertiesSettingPolicy { get; set; } = PropertySettingPolicies.Allow;

    /// <summary>
    /// Determines whether this instance contains an element that has the specified key.
    /// </summary>
    /// <param name="key">The key to locate.</param>
    /// <returns>true if this instance contains an element that has the specified key; otherwise, false.</returns>
    protected bool ContainsKey(string key)
        => cache.ContainsKey(key);

    /// <summary>
    /// Sets and property initialize. This change will not occur the event property changed,
    /// </summary>
    /// <typeparam name="T">The type of the property value.</typeparam>
    /// <param name="key">The property key.</param>
    /// <param name="initializer">A handler to resolve value of the specific property.</param>
    protected void InitializeProperty<T>(string key, Func<T> initializer)
    {
        if (initializer is null || cache.ContainsKey(key)) return;
        SetPropertyInternal(key, initializer());
    }

    /// <summary>
    /// Gets a property value.
    /// </summary>
    /// <typeparam name="T">The type of the property value.</typeparam>
    /// <param name="defaultValue">The default value.</param>
    /// <param name="key">The additional key.</param>
    /// <returns>A property value.</returns>
    protected T GetCurrentProperty<T>(T defaultValue = default, [CallerMemberName] string key = null)
    {
        if (string.IsNullOrWhiteSpace(key)) return defaultValue;
        try
        {
            return TryGetPropertyInternal(key, out var v) ? (T)v : defaultValue;
        }
        catch (InvalidCastException)
        {
        }
        catch (NullReferenceException)
        {
        }
        catch (ArgumentException)
        {
        }

        return defaultValue;
    }

    /// <summary>
    /// Gets a property value.
    /// </summary>
    /// <typeparam name="T">The type of the property value.</typeparam>
    /// <param name="keyCase">A casing rules of the specified culture.</param>
    /// <param name="culture">An object that supplies culture-specific casing rules.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <param name="key">The additional key.</param>
    /// <returns>A property value.</returns>
    protected T GetCurrentProperty<T>(Cases keyCase, CultureInfo culture = null, T defaultValue = default, [CallerMemberName] string key = null)
    {
        key = StringExtensions.ToSpecificCase(key, keyCase, culture);
        if (string.IsNullOrWhiteSpace(key)) return defaultValue;
        try
        {
            return TryGetPropertyInternal(key, out var v) ? (T)v : defaultValue;
        }
        catch (InvalidCastException)
        {
        }
        catch (NullReferenceException)
        {
        }
        catch (ArgumentException)
        {
        }

        return defaultValue;
    }

    /// <summary>
    /// Sets a property.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="key">The additional key.</param>
    /// <returns>true if set succeeded; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">key was null.</exception>
    /// <exception cref="ArgumentException">key was empty or consists only of white-space characters; or s was not in correct format to parse.</exception>
    protected bool SetCurrentProperty(object value, [CallerMemberName] string key = null)
    {
        if (!AssertPropertyKey(key)) return false;
        var exist = cache.TryGetValue(key, out var v);
        if (exist && v == value) return false;
        if (PropertiesSettingPolicy == PropertySettingPolicies.Allow)
        {
            if (!SetPropertyInternal(key, value)) return false;
            RaisePropertyChange(v, value, exist ? ChangeMethods.Update : ChangeMethods.Add, key);
            return true;
        }

        return RejectSet();
    }

    /// <summary>
    /// Sets a property.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="keyCase">A casing rules of the specified culture.</param>
    /// <param name="culture">An object that supplies culture-specific casing rules.</param>
    /// <param name="key">The additional key.</param>
    /// <returns>true if set succeeded; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">key was null.</exception>
    /// <exception cref="ArgumentException">key was empty or consists only of white-space characters; or s was not in correct format to parse.</exception>
    protected bool SetCurrentProperty(object value, Cases keyCase, CultureInfo culture = null, [CallerMemberName] string key = null)
    {
        key = StringExtensions.ToSpecificCase(key, keyCase, culture);
        if (!AssertPropertyKey(key)) return false;
        var exist = cache.TryGetValue(key, out var v);
        if (exist && v == value) return false;
        if (PropertiesSettingPolicy == PropertySettingPolicies.Allow)
        {
            if (!SetPropertyInternal(key, value)) return false;
            RaisePropertyChange(v, value, exist ? ChangeMethods.Update : ChangeMethods.Add, key);
            return true;
        }

        return RejectSet();
    }

    /// <summary>
    /// Sets a property.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="notify">The notification function.</param>
    /// <param name="key">The additional key.</param>
    /// <returns>true if set succeeded; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">key was null.</exception>
    /// <exception cref="ArgumentException">key was empty or consists only of white-space characters; or s was not in correct format to parse.</exception>
    protected bool SetCurrentProperty(object value, Action<string, object, bool, object> notify, [CallerMemberName] string key = null)
    {
        if (!AssertPropertyKey(key)) return false;
        var exist = cache.TryGetValue(key, out var v);
        if (exist && v == value) return false;
        if (PropertiesSettingPolicy == PropertySettingPolicies.Allow)
        {
            if (!SetPropertyInternal(key, value)) return false;
            notify?.Invoke(key, value, exist, v);
            RaisePropertyChange(v, value, exist ? ChangeMethods.Update : ChangeMethods.Add, key);
            return true;
        }

        return RejectSet();
    }

    /// <summary>
    /// Sets a property.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="notify">The notification function.</param>
    /// <param name="key">The additional key.</param>
    /// <returns>true if set succeeded; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">key was null.</exception>
    /// <exception cref="ArgumentException">key was empty or consists only of white-space characters; or s was not in correct format to parse.</exception>
    protected bool SetCurrentProperty(object value, Action<object> notify, [CallerMemberName] string key = null)
    {
        if (!AssertPropertyKey(key)) return false;
        var exist = cache.TryGetValue(key, out var v);
        if (exist && v == value) return false;
        if (PropertiesSettingPolicy == PropertySettingPolicies.Allow)
        {
            if (!SetPropertyInternal(key, value)) return false;
            notify?.Invoke(value);
            RaisePropertyChange(v, value, exist ? ChangeMethods.Update : ChangeMethods.Add, key);
            return true;
        }

        return RejectSet();
    }

    /// <summary>
    /// Gets a property value.
    /// </summary>
    /// <typeparam name="T">The type of the property value.</typeparam>
    /// <param name="key">The key.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>A property value.</returns>
    protected T GetProperty<T>(string key, T defaultValue = default)
    {
        if (string.IsNullOrWhiteSpace(key)) return defaultValue;
        try
        {
            if (!TryGetPropertyInternal(key, out var v)) return defaultValue;
            if (v is T result) return result;
            var typeE = typeof(T);
            if (typeE.IsInterface || typeE.IsAbstract) return defaultValue;
            if (v is null) return default;
            var typeO = v.GetType();
            if (v is string s)
            {
                if (typeE.IsEnum)
                {
#if NETCOREAPP
                    return Enum.TryParse(typeE, s, out var en) ? (T)en : defaultValue;
#else
                    return (T)Enum.Parse(typeE, s);
#endif
                }

                if (typeE.IsValueType)
                {
                    if (typeE == typeof(bool))
                    {
                        var b = JsonBooleanNode.TryParse(s);
                        return b == null ? defaultValue : (T)(object)b.Value;
                    }

                    if (string.IsNullOrEmpty(s)) return defaultValue;
                    if (typeE == typeof(int))
                        return Maths.Numbers.TryParseToInt32(s, 10, out var i) ? (T)(object)i : defaultValue;
                    if (typeE == typeof(long))
                        return Maths.Numbers.TryParseToInt64(s, 10, out var i) ? (T)(object)i : defaultValue;
                    if (typeE == typeof(short))
                        return Maths.Numbers.TryParseToInt16(s, 10, out var i) ? (T)(object)i : defaultValue;
                    if (typeE == typeof(double))
                        return double.TryParse(s, out var i) ? (T)(object)i : defaultValue;
                    if (typeE == typeof(float))
                        return float.TryParse(s, out var i) ? (T)(object)i : defaultValue;
                    if (typeE == typeof(decimal))
                        return decimal.TryParse(s, out var i) ? (T)(object)i : defaultValue;
                    if (typeE == typeof(uint))
                        return Maths.Numbers.TryParseToUInt32(s, 10, out var i) ? (T)(object)i : defaultValue;
                    if (typeE == typeof(ushort))
                        return Maths.Numbers.TryParseToUInt16(s, 10, out var i) ? (T)(object)i : defaultValue;
                    if (typeE == typeof(Guid))
                        return Guid.TryParse(s, out var i) ? (T)(object)i : defaultValue;
#if NET8_0_OR_GREATER
                    if (typeE == typeof(Int128))
                        return Maths.Numbers.TryParseToInt128(s, 10, out var i) ? (T)(object)i : defaultValue;
#endif
                }
                else if (typeE == typeof(StringBuilder))
                {
                    return (T)(object)new StringBuilder(s);
                }
                else if (typeE == typeof(Uri))
                {
                    var uri = StringExtensions.TryCreateUri(s);
                    return uri == null ? defaultValue : (T)(object)uri;
                }
                else if (typeE == typeof(JsonEncodedText))
                {
                    return (T)(object)JsonEncodedText.Encode(s);
                }
                else if (typeE == typeof(JsonStringNode) || typeE == typeof(IJsonValueNode<string>))
                {
                    return (T)(object)new JsonStringNode(s);
                }
            }
            else if (typeO.IsEnum)
            {
                if (typeE == typeof(string))
                {
                    return (T)(object)v.ToString();
                }
                if (typeE.IsValueType)
                {
                    if (typeE == typeof(int))
                        return (T)(object)(int)v;
                    if (typeE == typeof(long))
                        return (T)(object)(long)v;
                    if (typeE == typeof(short))
                        return (T)(object)(short)v;
                    if (typeE == typeof(double))
                        return (T)(object)(double)v;
                    if (typeE == typeof(float))
                        return (T)(object)(float)v;
                    if (typeE == typeof(decimal))
                        return (T)(object)(decimal)v;
                    if (typeE == typeof(uint))
                        return (T)(object)(uint)v;
                    if (typeE == typeof(ushort))
                        return (T)(object)(ushort)v;
                }
            }
            else if (typeO.IsValueType)
            {
                if (typeE == typeof(string)
                    && (typeO == typeof(char) || typeO == typeof(float) || typeO == typeof(double) || typeO == typeof(decimal)
                    || typeO == typeof(int) || typeO == typeof(long) || typeO == typeof(short) || typeO == typeof(byte)
                    || typeO == typeof(uint) || typeO == typeof(ulong)) || typeO == typeof(ushort) || typeO == typeof(sbyte)
#if NET8_0_OR_GREATER
                    || typeO == typeof(Int128) || typeO == typeof(UInt128) || typeO == typeof(Half) || typeO == typeof(DateOnly) || typeO == typeof(TimeOnly)
#endif
                    || typeO == typeof(bool) || typeO == typeof(Guid) || typeO == typeof(BigInteger) || typeO == typeof(DateTime) || typeO == typeof(DateTimeOffset))
                {
                    return (T)(object)v.ToString();
                }
                else if (typeE == typeof(int))
                {
                    if (v is long i2) return (T)(object)(int)i2;
                    if (v is short i3) return (T)(object)(int)i3;
                    if (v is double i6) return (T)(object)(int)i6;
                    if (v is float i7) return (T)(object)(int)i7;
                    if (v is decimal i8) return (T)(object)(int)i8;
                    if (v is bool b) return (T)(object)(b ? 1 : 0);
                    if (v is string s2) return Maths.Numbers.TryParseToInt32(s2, 10, out var i0) ? (T)(object)i0 : defaultValue;
                }
                else if (typeE == typeof(long))
                {
                    if (v is int i1) return (T)(object)(long)i1;
                    if (v is short i3) return (T)(object)(long)i3;
                    if (v is double i6) return (T)(object)(long)i6;
                    if (v is float i7) return (T)(object)(long)i7;
                    if (v is decimal i8) return (T)(object)(long)i8;
                    if (v is string s2) return Maths.Numbers.TryParseToInt64(s2, 10, out var i0) ? (T)(object)i0 : defaultValue;
                }
                else if (typeE == typeof(double))
                {
                    if (v is float i7) return (T)(object)(double)i7;
                    if (v is decimal i8) return (T)(object)(double)i8;
                    if (v is int i1) return (T)(object)(double)i1;
                    if (v is long i2) return (T)(object)(double)i2;
                }
                else if (typeE == typeof(float))
                {
                    if (v is double i6) return (T)(object)(float)i6;
                    if (v is decimal i8) return (T)(object)(float)i8;
                    if (v is int i1) return (T)(object)(float)i1;
                    if (v is long i2) return (T)(object)(float)i2;
                }
                else if (typeE == typeof(decimal))
                {
                    if (v is float i7) return (T)(object)(decimal)i7;
                    if (v is double i6) return (T)(object)(decimal)i6;
                    if (v is int i1) return (T)(object)(decimal)i1;
                    if (v is long i2) return (T)(object)(decimal)i2;
                }
                else if (typeE == typeof(bool))
                {
                    if (v is int i1)
                    {
                        if (i1 == 1 || i1 == 200) return (T)(object)true;
                        if (i1 == 0 || i1 == -1) return (T)(object)false;
                        return defaultValue;
                    }

                    if (v is string s2) return JsonBooleanNode.TryParse(s2, out var b) ? (T)(object)b.Value : defaultValue;
                }
                else if (typeE == typeof(DateTime))
                {
                    if (v is string s2)
                    {
                        var dt = WebFormat.ParseDate(s2);
                        if (dt.HasValue) return (T)(object)dt.Value;
                        return defaultValue;
                    }

                    if (v is DateTimeOffset dto) return (T)(object)dto.UtcDateTime;
                }
            }
            else if (typeO == typeof(Uri))
            {
                if (typeE == typeof(string))
                    return (T)(object)((Uri)v).OriginalString;
            }
            else if (v is JsonObjectNode json)
            {
                if (typeE == typeof(string))
                    return (T)(object)json.ToString();
                else if (typeE == typeof(JsonObject))
                    return (T)(object)(JsonObject)json;
                else if (typeE == typeof(JsonNode))
                    return (T)(object)(JsonNode)json;
            }
            else if (v is IJsonObjectHost joh)
            {
                if (typeE == typeof(JsonObjectNode))
                    return (T)(object)joh.ToJson();
                else if (typeE == typeof(string))
                    return (T)(object)joh.ToJson().ToString();
            }
            else if (v is SecureString secure)
            {
                if (typeE == typeof(string))
                    return (T)(object)Security.SecureStringExtensions.ToUnsecureString(secure);
                else if (typeE == typeof(StringBuilder))
                    return (T)(object)new StringBuilder(Security.SecureStringExtensions.ToUnsecureString(secure));
            }
            else if (v is BaseNestedParameter n)
            {
                if (n.ParameterIs(out T nr)) return nr;
            }
            else if (v is TypedNestedParameter tn)
            {
                if (tn.TryGet(out T nr)) return nr;
            }

            return defaultValue;
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
        catch (ArgumentException)
        {
        }
        catch (OverflowException)
        {
        }
        catch (ArithmeticException)
        {
        }

        return defaultValue;
    }

    /// <summary>
    /// Gets a property value.
    /// </summary>
    /// <typeparam name="T">The type of the property value.</typeparam>
    /// <param name="key">The key.</param>
    /// <param name="result">The property value.</param>
    /// <returns>true if contains; otherwise, false.</returns>
    protected bool GetProperty<T>(string key, out T result)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(key) && TryGetPropertyInternal(key, out var v) && v is T t)
            {
                result = t;
                return true;
            }
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
        catch (ArgumentException)
        {
        }

        result = default;
        return false;
    }

    /// <summary>
    /// Sets a property.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    /// <returns>true if set succeeded; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">key was null.</exception>
    /// <exception cref="ArgumentException">key was empty or consists only of white-space characters; or s was not in correct format to parse.</exception>
    protected bool SetProperty(string key, object value)
    {
        if (!AssertPropertyKey(key)) return false;
        var exist = cache.TryGetValue(key, out var v);
        if (exist && v == value) return false;
        if (PropertiesSettingPolicy == PropertySettingPolicies.Allow)
        {
            if (!SetPropertyInternal(key, value)) return false;
            RaisePropertyChange(v, value, exist ? ChangeMethods.Update : ChangeMethods.Add, key);
            return true;
        }

        return RejectSet();
    }

    /// <summary>
    /// Tests whether the new property to set is valid.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value of the property to set.</param>
    /// <returns>true if the property is valid; otherwise, false.</returns>
    protected virtual bool IsPropertyValid(string key, object value)
        => true;

    /// <summary>
    /// Occurs on request to get a specific value. This can be used to fill a default value.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">When this method returns, contains the value associated with the specified key, if the key is found; otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.</param>
    /// <returns>true if need to set a default value; otherwise, false.</returns>
    protected virtual bool FillNonExistProperty(string key, out object value)
    {
        value = default;
        return false;
    }

    /// <summary>
    /// Removes a property.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>true if the element is successfully found and removed; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">key was null.</exception>
    /// <exception cref="ArgumentException">key was empty or consists only of white-space characters; or s was not in correct format to parse.</exception>
    protected bool RemoveProperty(string key)
    {
        if (!AssertPropertyKey(key)) return false;
        if (PropertiesSettingPolicy == PropertySettingPolicies.Allow)
        {
            if (PropertyChanged is null)
            {
                var result = cache.Remove(key);
                if (result)
                {
                    RevisionToken = new();
                    propertyChanged?.Invoke(this, new(key));
                }

                return result;
            }
            else
            {
                var exist = cache.TryGetValue(key, out var v);
                if (!exist) return false;
                var result = cache.Remove(key);
                if (result)
                {
                    RevisionToken = new();
                    RaisePropertyChange(v, null, ChangeMethods.Remove, key);
                }

                return result;
            }
        }

        return RejectSet();
    }

    /// <summary>
    /// Removes a set of properties.
    /// </summary>
    /// <param name="keys">The keys of property to remove.</param>
    /// <returns>The count of property removed.</returns>
    protected int RemoveProperties(IEnumerable<string> keys)
    {
        var i = 0;
        if (keys == null) return i;
        foreach (var key in keys)
        {
            if (RemoveProperty(key)) i++;
        }

        return i;
    }

    /// <summary>
    /// Removes a set of properties.
    /// </summary>
    /// <param name="keys">The keys of property to remove.</param>
    /// <returns>The count of property removed.</returns>
    protected int RemoveProperties(params string[] keys)
    {
        var i = 0;
        foreach (var key in keys)
        {
            if (RemoveProperty(key)) i++;
        }

        return i;
    }

    /// <summary>
    /// Removes a set of properties.
    /// </summary>
    /// <param name="keys">The keys of property to remove.</param>
    /// <returns>The count of property removed.</returns>
    protected int RemoveProperties(params ReadOnlySpan<string> keys)
    {
        var i = 0;
        foreach (var key in keys)
        {
            if (RemoveProperty(key)) i++;
        }

        return i;
    }

    /// <summary>
    /// Gets the type of a specific property.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>The type of the property value; or null, if no such property.</returns>
    protected Type GetPropertyType(string key)
    {
        if (string.IsNullOrWhiteSpace(key)) return null;
        try
        {
            return TryGetPropertyInternal(key, out var v) ? v?.GetType() : null;
        }
        catch (ArgumentException)
        {
        }

        return null;
    }

    /// <summary>
    /// Forces rasing the property changed notification.
    /// </summary>
    /// <param name="key">The property key.</param>
    protected void ForceNotify(string key)
    {
        propertyChanged?.Invoke(this, new(key));
        if (PropertyChanged is null) return;
        var exist = cache.TryGetValue(key, out var v);
        if (!exist) return;
        PropertyChanged?.Invoke(this, new(v, v, ChangeMethods.Same, key));
    }

    /// <summary>
    /// Gets the property in JSON format string.
    /// </summary>
    /// <typeparam name="T">The type of the propery value.</typeparam>
    /// <param name="key">The property key.</param>
    /// <param name="options">The optional JSON serializer options.</param>
    /// <returns>The propery value in JSON format string.</returns>
    protected string GetPropertyJson<T>(string key, JsonSerializerOptions options = null)
    {
        var v = GetProperty<T>(key);
        if (v is null || v is DBNull) return null;
        if (v is string s) return JsonStringNode.ToJson(s);
        if (v is JsonObjectNode json) return options?.WriteIndented == true ? json.ToString(IndentStyles.Compact) : json.ToString();
        if (v is JsonArrayNode arr) return options?.WriteIndented == true ? arr.ToString(IndentStyles.Compact) : arr.ToString();
        if (v is JsonNode node) return node.ToJsonString(options);
        if (v is JsonDocument jDoc) return jDoc.RootElement.ToString();
        if (v is IJsonObjectHost joh) return joh.ToJson()?.ToString();
        if (v is Net.QueryData q) return q.ToString();
        if (v is Uri u) return u.OriginalString;
        if (v.GetType().IsValueType)
        {
            if (v is bool b) return b ? JsonBooleanNode.TrueString : JsonBooleanNode.FalseString;
            if (v is int i) return i.ToString("g");
            if (v is long l) return l.ToString("g");
            if (v is float f) return float.IsNaN(f) ? null : f.ToString("g");
            if (v is double d) return double.IsNaN(d) ? null : d.ToString("g");
            if (v is decimal d2) return d2.ToString("g");
            if (v is short i2) return i2.ToString("g");
            if (v is Guid g) return g.ToString();
            if (v is DateTime dt) return JsonStringNode.ToJson(dt);
            if (v is DateTimeOffset dto) return JsonStringNode.ToJson(dto.UtcDateTime);
            if (v is JsonElement jEle) return jEle.ToString();
            if (v is uint ui) return ui.ToString("g");
            if (v is ulong ul) return ul.ToString("g");
#if NET8_0_OR_GREATER
            if (v is Int128 i3) return i3.ToString("g");
            if (v is Half d3) return d3.ToString("g");
            if (v is DateOnly dt2) return dt2.ToString("g");
            if (v is TimeOnly dt3) return dt3.ToString("g");
#endif
        }

        if (v is StringBuilder sb) return sb.ToString();
        return JsonSerializer.Serialize(v, options);
    }

    /// <summary>
    /// Writes this instance to the specified writer as a JSON value.
    /// </summary>
    /// <param name="writer">The writer to which to write this instance.</param>
    protected virtual void WriteTo(Utf8JsonWriter writer)
    {
        var json = this is IJsonObjectHost obj ? obj.ToJson() : JsonObjectNode.ConvertFrom(this);
        if (json is null) writer.WriteNullValue();
        else json.WriteTo(writer);
    }

    /// <summary>
    /// Copies data from another instance.
    /// </summary>
    /// <param name="props">The properties to copy.</param>
    /// <param name="skipIfExist">true if skip when the property has already existed; otherwise, false.</param>
    protected void CopyFrom(BaseObservableProperties props, bool skipIfExist = false)
    {
        var copy = new Dictionary<string, object>(props.cache);
        foreach (var prop in copy)
        {
            if (skipIfExist && cache.ContainsKey(prop.Key)) continue;
            SetProperty(prop.Key, prop.Value);
        }
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the collection.</returns>
    protected IEnumerator<KeyValuePair<string, object>> EnumerateObject()
        => cache.GetEnumerator();

    /// <summary>
    /// Tries to get property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">When this method returns, contains the value associated with the specified key, if the key is found; otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.</param>
    /// <returns>true if the cache contains an element with the specified key; otherwise, false.</returns>
    private bool TryGetPropertyInternal(string key, out object value)
    {
        if (cache.TryGetValue(key, out value)) return true;
        if (!FillNonExistProperty(key, out value)) return false;
        cache[key] = value;
        RevisionToken = new();
        RaisePropertyChange(null, value, ChangeMethods.Add, key);
        return true;
    }

    /// <summary>
    /// Sets the property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <param name="value">The value of the property to set.</param>
    /// <returns>true if set succeeded; otherwise, false.</returns>
    private bool SetPropertyInternal(string key, object value)
    {
        try
        {
            if (!IsPropertyValid(key, value)) return false;
            cache[key] = value;
            RevisionToken = new();
            return true;
        }
        catch (ArgumentException)
        {
        }
        catch (InvalidOperationException)
        {
        }
        catch (NotSupportedException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
        catch (NullReferenceException)
        {
        }
        catch (ApplicationException)
        {
        }

        return false;
    }

    private void RaisePropertyChange(object oldValue, object newValue, ChangeMethods method, string key)
    {
        propertyChanged?.Invoke(this, new(key));
        PropertyChanged?.Invoke(this, new(oldValue, newValue, method, key));
    }

    private bool AssertPropertyKey(string key)
    {
        if (!string.IsNullOrWhiteSpace(key)) return true;
        if (PropertiesSettingPolicy == PropertySettingPolicies.Forbidden)
            throw new ArgumentNullException(nameof(key), "The property key should not be null, empty, or consists only of white-space characters.");
        return false;
    }

    private bool RejectSet()
    {
        if (PropertiesSettingPolicy == PropertySettingPolicies.Skip) return false;
        throw new InvalidOperationException("Forbid to set property.", new UnauthorizedAccessException("No permission to set property."));
    }
}

/// <summary>
/// The model with observable properties.
/// </summary>
public class ObservableProperties : BaseObservableProperties
{
    /// <summary>
    /// Gets the revision token of member-wised property updated.
    /// </summary>
    [JsonIgnore]
    public new object RevisionToken => base.RevisionToken;

    /// <summary>
    /// Gets an enumerable collection that contains the keys in this instance.
    /// </summary>
    [JsonIgnore]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public new IEnumerable<string> Keys => base.Keys;

    /// <summary>
    /// Gets or sets the policy used to set property value.
    /// </summary>
    [JsonIgnore]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public new PropertySettingPolicies PropertiesSettingPolicy
    {
        get => base.PropertiesSettingPolicy;
        set => base.PropertiesSettingPolicy = value;
    }

    /// <summary>
    /// Determines whether this instance contains an element that has the specified key.
    /// </summary>
    /// <param name="key">The key to locate.</param>
    /// <returns>true if this instance contains an element that has the specified key; otherwise, false.</returns>
    public new bool ContainsKey(string key) => base.ContainsKey(key);

    /// <summary>
    /// Sets and property initialize. This change will not occur the event property changed,
    /// </summary>
    /// <typeparam name="T">The type of the property value.</typeparam>
    /// <param name="key">The property key.</param>
    /// <param name="initializer">A handler to resolve value of the specific property.</param>
    public new void InitializeProperty<T>(string key, Func<T> initializer) => base.InitializeProperty(key, initializer);

    /// <summary>
    /// Gets a property value.
    /// </summary>
    /// <typeparam name="T">The type of the property value.</typeparam>
    /// <param name="key">The key.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>A property value.</returns>
    public new T GetProperty<T>(string key, T defaultValue = default) => base.GetProperty(key, defaultValue);

    /// <summary>
    /// Gets a property value.
    /// </summary>
    /// <typeparam name="T">The type of the property value.</typeparam>
    /// <param name="key">The key.</param>
    /// <param name="result">The property value.</param>
    /// <returns>true if contains; otherwise, false.</returns>
    public new bool GetProperty<T>(string key, out T result) => base.GetProperty(key, out result);

    /// <summary>
    /// Sets a property.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    /// <returns>true if set succeeded; otherwise, false.</returns>
    public new bool SetProperty(string key, object value) => base.SetProperty(key, value);

    /// <summary>
    /// Removes a property.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>true if the element is successfully found and removed; otherwise, false.</returns>
    public new bool RemoveProperty(string key) => base.RemoveProperty(key);

    /// <summary>
    /// Gets the type of a specific property.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>The type of the property value; or null, if no such property.</returns>
    public new Type GetPropertyType(string key) => base.GetPropertyType(key);

    /// <summary>
    /// Forces rasing the property changed notification.
    /// </summary>
    /// <param name="key">The property key.</param>
    public new void ForceNotify(string key) => base.ForceNotify(key);

    /// <summary>
    /// Writes this instance to the specified writer as a JSON value.
    /// </summary>
    /// <param name="writer">The writer to which to write this instance.</param>
    public new void WriteTo(Utf8JsonWriter writer) => base.WriteTo(writer);
}
