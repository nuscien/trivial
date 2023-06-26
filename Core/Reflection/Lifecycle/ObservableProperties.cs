using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Trivial.Text;

namespace Trivial.Reflection;

/// <summary>
/// The policies used to set property.
/// </summary>
public enum PropertySettingPolicies
{
    /// <summary>
    /// Writable property.
    /// </summary>
    Allow = 0,

    /// <summary>
    /// Read-only property but skip error when set value.
    /// </summary>
    Skip = 1,

    /// <summary>
    /// Read-only property and require to throw an exception when set value.
    /// </summary>
    Forbidden = 2
}

/// <summary>
/// Base model with observable properties.
/// </summary>
public abstract class BaseObservableProperties : INotifyPropertyChanged
{
    /// <summary>
    /// Data cache.
    /// </summary>
    private readonly Dictionary<string, object> cache = new();

    /// <summary>
    /// Gets an enumerable collection that contains the keys in this instance.
    /// </summary>
    protected IEnumerable<string> Keys => cache.Keys;

    /// <summary>
    /// Gets or sets the policy used to set property value.
    /// </summary>
    protected PropertySettingPolicies PropertiesSettingPolicy { get; set; } = PropertySettingPolicies.Allow;

    /// <summary>
    /// Adds or removes the event handler raised on property changed.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

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
        cache[key] = initializer();
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
            return cache.TryGetValue(key, out var v) ? (T)v : defaultValue;
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
    protected bool SetCurrentProperty(object value, [CallerMemberName] string key = null)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            if (PropertiesSettingPolicy == PropertySettingPolicies.Forbidden)
                throw new ArgumentNullException(nameof(key), "The property key should not be null, empty, or consists only of white-space characters.");
            return false;
        }

        if (cache.TryGetValue(key, out var v) && v == value) return false;
        if (PropertiesSettingPolicy == PropertySettingPolicies.Allow)
        {
            cache[key] = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(key));
            return true;
        }

        if (PropertiesSettingPolicy == PropertySettingPolicies.Skip) return false;
        throw new InvalidOperationException("Forbid to set property.");
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
            return cache.TryGetValue(key, out var v) ? (T)v : defaultValue;
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
    /// <param name="key">The key.</param>
    /// <param name="result">The property value.</param>
    /// <returns>true if contains; otherwise, false.</returns>
    protected bool GetProperty<T>(string key, out T result)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(key) && cache.TryGetValue(key, out var v))
            {
                result = (T)v;
                return true;
            }
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
    protected bool SetProperty(string key, object value)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            if (PropertiesSettingPolicy == PropertySettingPolicies.Forbidden)
                throw new ArgumentNullException(nameof(key), "The property key should not be null, empty, or consists only of white-space characters.");
            return false;
        }

        if (cache.TryGetValue(key, out var v) && v == value) return false;
        if (PropertiesSettingPolicy == PropertySettingPolicies.Allow)
        {
            cache[key] = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(key));
            return true;
        }

        if (PropertiesSettingPolicy == PropertySettingPolicies.Skip) return false;
        throw new InvalidOperationException("Forbid to set property.");
    }

    /// <summary>
    /// Removes a property.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>true if the element is successfully found and removed; otherwise, false.</returns>
    protected bool RemoveProperty(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            if (PropertiesSettingPolicy == PropertySettingPolicies.Forbidden)
                throw new ArgumentNullException(nameof(key), "The property key should not be null, empty, or consists only of white-space characters.");
            return false;
        }

        if (PropertiesSettingPolicy == PropertySettingPolicies.Allow)
        {
            var result = cache.Remove(key);
            if (result) PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(key));
            return result;
        }

        if (PropertiesSettingPolicy == PropertySettingPolicies.Skip) return false;
        throw new InvalidOperationException("Forbid to set property.");
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
            return cache.TryGetValue(key, out var v) ? v?.GetType() : null;
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
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(key));

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
        if (v is System.Text.Json.Nodes.JsonNode node) return node.ToJsonString(options);
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
            if (v is Guid g) return g.ToString();
            if (v is DateTime dt) return JsonStringNode.ToJson(dt);
            if (v is DateTimeOffset dto) return JsonStringNode.ToJson(dto.UtcDateTime);
            if (v is JsonElement jEle) return jEle.ToString();
            if (v is uint ui) return ui.ToString("g");
            if (v is ulong ul) return ul.ToString("g");
        }

        if (v is StringBuilder sb) return sb.ToString();
        return JsonSerializer.Serialize(v, options);
    }

    /// <summary>
    /// Writes this instance to the specified writer as a JSON value.
    /// </summary>
    /// <param name="writer">The writer to which to write this instance.</param>
    protected void WriteTo(Utf8JsonWriter writer) => JsonObjectNode.ConvertFrom(this).WriteTo(writer);

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
}

/// <summary>
/// The model with observable properties.
/// </summary>
public class ObservableProperties : BaseObservableProperties
{
    /// <summary>
    /// Gets an enumerable collection that contains the keys in this instance.
    /// </summary>
    public new IEnumerable<string> Keys => base.Keys;

    /// <summary>
    /// Gets or sets the policy used to set property value.
    /// </summary>
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
    public new void WriteTo(Utf8JsonWriter writer) => JsonObjectNode.ConvertFrom(this).WriteTo(writer);
}

/// <summary>
/// The model with observable name and value.
/// </summary>
[DataContract]
public class NameValueObservableProperties<T> : BaseObservableProperties
{
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    [DataMember(Name = "name")]
    [JsonPropertyName("name")]
    public string Name
    {
        get => GetCurrentProperty<string>();
        set => SetCurrentProperty(value);
    }

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    [DataMember(Name = "value")]
    [JsonPropertyName("value")]
    public T Value
    {
        get => GetCurrentProperty<T>();
        set => SetCurrentProperty(value);
    }
}
