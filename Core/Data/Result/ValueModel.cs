using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Trivial.Collection;
using Trivial.Reflection;
using Trivial.Text;

namespace Trivial.Data;

/// <summary>
/// The model with observable key and value.
/// </summary>
/// <typeparam name="T">The type of the value.</typeparam>
[DataContract]
public class KeyValueObservableModel<T> : BaseObservableProperties
{
    /// <summary>
    /// Initializes a new instance of the KeyValueObservableModel class.
    /// </summary>
    public KeyValueObservableModel()
    {
    }

    /// <summary>
    /// Initializes a new instance of the KeyValueObservableModel class.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    public KeyValueObservableModel(string key, T value)
    {
        Key = key;
        Value = value;
    }

    /// <summary>
    /// Gets or sets the key.
    /// </summary>
    [DataMember(Name = "key")]
    [JsonPropertyName("key")]
    [Description("The property key.")]
    public string Key

    {
        get => GetCurrentProperty<string>();
        set => SetCurrentProperty(value, OnChange);
    }

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    [DataMember(Name = "value")]
    [JsonPropertyName("value")]
    [Description("The value.")]
    public T Value
    {
        get => GetCurrentProperty<T>();
        set => SetCurrentProperty(value, OnChange);
    }

    /// <summary>
    /// Gets or sets the optional additional object.
    /// </summary>
    [JsonIgnore]
    public object Tag
    {
        get => GetCurrentProperty<object>();
        set => SetCurrentProperty(value);
    }

    /// <summary>
    /// Occurs on the property key is changed.
    /// </summary>
    /// <param name="newValue">The new value of the property.</param>
    /// <param name="exist">true if the old value of the property exists; otherwise, false.</param>
    /// <param name="oldValue">The old value of the property.</param>
    protected virtual void OnKeyChange(object newValue, bool exist, object oldValue)
    {
    }

    /// <summary>
    /// Occurs on the property value is changed.
    /// </summary>
    /// <param name="newValue">The new value of the property.</param>
    /// <param name="exist">true if the old value of the property exists; otherwise, false.</param>
    /// <param name="oldValue">The old value of the property.</param>
    protected virtual void OnValueChange(object newValue, bool exist, object oldValue)
    {
    }

    private void OnChange(string key, object newValue, bool exist, object oldValue)
    {
        if (string.IsNullOrEmpty(key)) return;
        switch (key)
        {
            case nameof(Key):
                OnKeyChange(newValue, exist, oldValue);
                break;
            case nameof(Value):
                OnValueChange(newValue, exist, oldValue);
                break;
        }
    }
}

/// <summary>
/// The model with observable name and value.
/// </summary>
/// <typeparam name="T">The type of the value.</typeparam>
[DataContract]
public class NameValueObservableModel<T> : BaseObservableProperties
{
    /// <summary>
    /// Initializes a new instance of the NameValueObservableModel class.
    /// </summary>
    public NameValueObservableModel()
    {
    }

    /// <summary>
    /// Initializes a new instance of the NameValueObservableModel class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="value">The value.</param>
    public NameValueObservableModel(string name, T value)
    {
        Name = name;
        Value = value;
    }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    [DataMember(Name = "name")]
    [JsonPropertyName("name")]
    [Description("The item name.")]
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
    [Description("The value.")]
    public T Value
    {
        get => GetCurrentProperty<T>();
        set => SetCurrentProperty(value);
    }

    /// <summary>
    /// Gets or sets the optional additional object.
    /// </summary>
    [JsonIgnore]
    public object Tag
    {
        get => GetCurrentProperty<object>();
        set => SetCurrentProperty(value);
    }

    /// <summary>
    /// Occurs on the property name is changed.
    /// </summary>
    /// <param name="newValue">The new value of the property.</param>
    /// <param name="exist">true if the old value of the property exists; otherwise, false.</param>
    /// <param name="oldValue">The old value of the property.</param>
    protected virtual void OnNameChange(object newValue, bool exist, object oldValue)
    {
    }

    /// <summary>
    /// Occurs on the property value is changed.
    /// </summary>
    /// <param name="newValue">The new value of the property.</param>
    /// <param name="exist">true if the old value of the property exists; otherwise, false.</param>
    /// <param name="oldValue">The old value of the property.</param>
    protected virtual void OnValueChange(object newValue, bool exist, object oldValue)
    {
    }

    private void OnChange(string key, object newValue, bool exist, object oldValue)
    {
        if (string.IsNullOrEmpty(key)) return;
        switch (key)
        {
            case nameof(Name):
                OnNameChange(newValue, exist, oldValue);
                break;
            case nameof(Value):
                OnValueChange(newValue, exist, oldValue);
                break;
        }
    }
}
