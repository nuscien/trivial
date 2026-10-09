using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
    [Description("Properties are writable.")]
    Allow = 0,

    /// <summary>
    /// Read-only property but skip error when set value.
    /// </summary>
    [Description("Properties are read-only. Any update will be ignored.")]
    Skip = 1,

    /// <summary>
    /// Read-only property and require to throw an exception when set value.
    /// </summary>
    [Description("Properties are read-only. Any update will occur an exception thrown.")]
    Forbidden = 2
}

/// <summary>
/// The model with observable properties.
/// </summary>
public class ObservableProperties : BaseObservableProperties
{
    /// <summary>
    /// Initializes a new instance of the ObservableProperties class.
    /// </summary>
    public ObservableProperties()
        : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the ObservableProperties class.
    /// </summary>
    /// <param name="copy">The properties copied to the new container.</param>
    public ObservableProperties(IDictionary<string, object> copy)
        : base(copy)
    {
    }

    /// <summary>
    /// Initializes a new instance of the ObservableProperties class.
    /// </summary>
    /// <param name="copy">The properties copied to the new container.</param>
    public ObservableProperties(BaseObservableProperties copy)
        : base(copy)
    {
    }

    /// <summary>
    /// Gets the revision token of member-wised property updated.
    /// </summary>
    [JsonIgnore]
#if NET10_0_OR_GREATER
    [NotMapped]
#endif
    public new object RevisionToken => base.RevisionToken;

    /// <summary>
    /// Gets an enumerable collection that contains the keys in this instance.
    /// </summary>
    [JsonIgnore]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
#if NET10_0_OR_GREATER
    [NotMapped]
#endif
    public new IEnumerable<string> Keys => base.Keys;

    /// <summary>
    /// Gets or sets the policy used to set property value.
    /// </summary>
    [JsonIgnore]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
#if NET10_0_OR_GREATER
    [NotMapped]
#endif
    public new PropertySettingPolicies PropertiesSettingPolicy
    {
        get => base.PropertiesSettingPolicy;
        set => base.PropertiesSettingPolicy = value;
    }

    /// <summary>
    /// Determines whether this instance contains an element that has the specified key.
    /// </summary>
    /// <param name="key">The key to locate.</param>
    /// <returns><c>true</c> if this instance contains an element that has the specified key; otherwise, <c>false</c>.</returns>
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
    /// <returns><c>true</c> if contains; otherwise, <c>false</c>.</returns>
    public new bool GetProperty<T>(string key, out T result) => base.GetProperty(key, out result);

    /// <summary>
    /// Sets a property.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    /// <returns><c>true</c> if set succeeded; otherwise, <c>false</c>.</returns>
    public new bool SetProperty(string key, object value) => base.SetProperty(key, value);

    /// <summary>
    /// Removes a property.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns><c>true</c> if the element is successfully found and removed; otherwise, <c>false</c>.</returns>
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

    /// <summary>
    /// Enumerates all properties.
    /// </summary>
    /// <param name="callback">The callback handler in for-each loop.</param>
    public new void ForEach(Action<string, object> callback) => base.ForEach(callback);

    /// <summary>
    /// Projects each element of a sequence into a new form by incorporating the property key.
    /// </summary>
    /// <param name="selector">A transform function to apply to each property.</param>
    public new IEnumerable<T> Select<T>(Func<string, object, T> selector) => base.Select(selector);
}
