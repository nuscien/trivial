using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
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
[Guid("05E04036-CE59-443E-AA10-604D8D52CCC1")]
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

    private void OnChange(ChangeEventArgs<object> ev)
    {
        if (string.IsNullOrEmpty(ev.Key)) return;
        switch (ev.Key)
        {
            case nameof(Key):
                OnKeyChange(ev.NewValue, ev.Method != ChangeMethods.Add, ev.OldValue);
                break;
            case nameof(Value):
                OnValueChange(ev.NewValue, ev.Method != ChangeMethods.Add, ev.OldValue);
                break;
        }
    }
}

/// <summary>
/// The model with observable name and value.
/// </summary>
/// <typeparam name="T">The type of the value.</typeparam>
[DataContract]
[Guid("CB88EE38-D835-4838-8FB4-EB0088B44F48")]
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

    private void OnChange(ChangeEventArgs<object> ev)
    {
        if (string.IsNullOrEmpty(ev.Key)) return;
        switch (ev.Key)
        {
            case nameof(Name):
                OnNameChange(ev.NewValue, ev.Method != ChangeMethods.Add, ev.OldValue);
                break;
            case nameof(Value):
                OnValueChange(ev.NewValue, ev.Method != ChangeMethods.Add, ev.OldValue);
                break;
        }
    }
}

/// <summary>
/// The observable model of the name and arguments.
/// </summary>
[DataContract]
[Guid("BFCF2C61-D656-45EE-8B1C-61E1B124ECBF")]
public class NameArgsObservableModel : ObservableProperties
{
    /// <summary>
    /// Initializes a new instance of the NameArgsObservableModel class.
    /// </summary>
    public NameArgsObservableModel()
    {
    }

    /// <summary>
    /// Initializes a new instance of the NameArgsObservableModel class.
    /// </summary>
    /// <param name="json">The intialized properties to copy and fill into this instance.</param>
    public NameArgsObservableModel(JsonObjectNode json)
    {
        Id = json.TryGetStringTrimmedValue("id", true);
        Name = json.TryGetStringTrimmedValue("name");
        Arguments = json.TryGetObjectValue("arguments") ?? json.TryGetObjectValue("args") ?? json.TryGetObjectValue("parameters");
    }

    /// <summary>
    /// Initializes a new instance of the NameArgsObservableModel class.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="name">The name.</param>
    /// <param name="value">The value.</param>
    public NameArgsObservableModel(string id, string name, JsonObjectNode value = null)
        : this(name, value)
    {
        Id = id;
    }

    /// <summary>
    /// Initializes a new instance of the NameArgsObservableModel class.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="name">The name.</param>
    /// <param name="value">The value.</param>
    public NameArgsObservableModel(Guid id, string name, JsonObjectNode value = null)
        : this(id.ToString(), name, value)
    {
    }

    /// <summary>
    /// Initializes a new instance of the NameArgsObservableModel class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="value">The value.</param>
    public NameArgsObservableModel(string name, JsonObjectNode value = null)
    {
        Name = name;
        Arguments = value;
    }

    /// <summary>
    /// Initializes a new instance of the NameArgsObservableModel class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="value">The value.</param>
    public NameArgsObservableModel(string name, Dictionary<string, string> value = null)
    {
        Name = name;
        if (value == null) return;
        Arguments = new();
        Arguments.SetRange(value);
    }

    /// <summary>
    /// Initializes a new instance of the NameArgsObservableModel class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="value">The value.</param>
    public NameArgsObservableModel(string name, Dictionary<string, JsonObjectNode> value = null)
    {
        Name = name;
        if (value == null) return;
        Arguments = new();
        Arguments.SetRange(value);
    }

    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    [DataMember(Name = "id")]
    [JsonPropertyName("id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Description("The identifier.")]
    public string Id
    {
        get => GetCurrentProperty<string>();
        set => SetCurrentProperty(value, OnChange);
    }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    [DataMember(Name = "name")]
    [JsonPropertyName("name")]
    [Description("The name.")]
    public string Name
    {
        get => GetCurrentProperty<string>();
        set => SetCurrentProperty(value, OnChange);
    }

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    [DataMember(Name = "arguments")]
    [JsonPropertyName("arguments")]
    [Description("The arguments.")]
    public JsonObjectNode Arguments
    {
        get => GetCurrentProperty<JsonObjectNode>();
        set => SetCurrentProperty(value, OnChange);
    }

    /// <summary>
    /// Occurs on the property identifier is changed.
    /// </summary>
    /// <param name="newValue">The new value of the property.</param>
    /// <param name="exist">true if the old value of the property exists; otherwise, false.</param>
    /// <param name="oldValue">The old value of the property.</param>
    protected virtual void OnIdChange(object newValue, bool exist, object oldValue)
    {
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
    /// Occurs on the property arguments is changed.
    /// </summary>
    /// <param name="newValue">The new value of the property.</param>
    /// <param name="exist">true if the old value of the property exists; otherwise, false.</param>
    /// <param name="oldValue">The old value of the property.</param>
    protected virtual void OnArgumentsChange(object newValue, bool exist, object oldValue)
    {
    }

    private void OnChange(ChangeEventArgs<object> ev)
    {
        if (string.IsNullOrEmpty(ev.Key)) return;
        switch (ev.Key)
        {
            case nameof(Id):
                OnIdChange(ev.NewValue, ev.Method != ChangeMethods.Add, ev.OldValue);
                break;
            case nameof(Name):
                OnNameChange(ev.NewValue, ev.Method != ChangeMethods.Add, ev.OldValue);
                break;
            case nameof(Arguments):
                OnArgumentsChange(ev.NewValue, ev.Method != ChangeMethods.Add, ev.OldValue);
                break;
        }
    }

    /// <summary>
    /// Parses.
    /// </summary>
    /// <param name="s">The string to parse.</param>
    /// <returns>The instance parsed; or null, if failed.</returns>
    public static NameArgsObservableModel Parse(string s)
    {
        if (string.IsNullOrWhiteSpace(s)) return null;
        if (s.StartsWith('`'))
        {
            s = s.Trim('`');
            if (s.StartsWith("json", StringComparison.OrdinalIgnoreCase)) s = s.Substring(4).Trim();
        }

        if (s.StartsWith('{'))
        {
            var json = JsonObjectNode.TryParse(s) ?? JsonObjectNode.TryParse(string.Concat(s, '}'));
            if (json == null) return null;
            return new(json);
        }

        var lines = StringExtensions.ReadLines(s).ToList();
        if (lines.Count == 4 && lines[3] == "```")
        {
            if (lines[1] == "```python")
            {
                var key = lines[0]?.Trim();
                if (string.IsNullOrEmpty(key)) return null;
                var cmd = lines[2]?.Trim();
                if (string.IsNullOrEmpty(cmd) || !cmd.StartsWith("tool_call(")) return null;
                cmd = cmd.Substring(10).TrimEnd(')');
                var args = cmd.Split(',');
                var json = new JsonObjectNode();
                foreach (var kvp in args)
                {
                    try
                    {
                        var i = kvp.IndexOf('=');
                        if (i < 1) continue;
                        var item = kvp.Substring(i + 1).Trim();
                        if (item.StartsWith('"')) json.SetValue(kvp.Substring(0, i), item.Trim('"'));
                        else if (item.StartsWith('\'')) json.SetValue(kvp.Substring(0, i), item.Trim('\''));
                        else if (item.IndexOf('.') > 0 && double.TryParse(item, out var d)) json.SetValue(kvp.Substring(0, i), d);
                        else if (int.TryParse(item, out var j)) json.SetValue(kvp.Substring(0, i), j);
                        else json.SetValue(kvp.Substring(0, i), item);
                    }
                    catch (ArgumentException)
                    {
                    }
                }

                return new(json);
            }
            else if (lines[1] == "```json")
            {
                var key = lines[0]?.Trim();
                var cmd = JsonObjectNode.TryParse(lines[2]?.Trim());
                if (cmd == null) return new();
                var json = cmd.TryGetObjectValue("arguments");
                if (json != null)
                {
                    cmd = json;
                    key = cmd.TryGetStringTrimmedValue("name", true) ?? key;
                }

                if (string.IsNullOrEmpty(key)) return null;
                return new(key, cmd);
            }
        }
        else if (lines.Count == 2 && !string.IsNullOrEmpty(lines[1]))
        {
            if (lines[1].Trim().Trim('`').StartsWith('{'))
            {
                var cmd = JsonObjectNode.TryParse(lines[1]?.Trim());
                if (cmd != null)
                {
                    var id = cmd.TryGetStringTrimmedValue("id", true);
                    var key = cmd.TryGetStringTrimmedValue("name", true);
                    var args = cmd.TryGetObjectValue("arguments");
                    var firstLine = lines[0].Length > 1 && !lines[0].Contains(' ') ? lines[0].Trim() : null;
                    if (key != null)
                    {
                        if (args != null || cmd.Count < 2) return new(id, key, args);
                    }
                    else if (args == null && firstLine != null)
                    {
                        return new(id, firstLine, cmd);
                    }
                }
            }
        }

        return null;
    }
}

/// <summary>
/// The model with observable name, verison and description.
/// </summary>
[DataContract]
[Guid("63CE7D37-4662-45DF-8455-613FEC982031")]
public class PackageInfoObservableModel : BaseObservableProperties
{
    /// <summary>
    /// Initializes a new instance of the PackageInfoObservableModel class.
    /// </summary>
    public PackageInfoObservableModel()
    {
    }

    /// <summary>
    /// Initializes a new instance of the PackageInfoObservableModel class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="version">The version.</param>
    /// <param name="description">The description.</param>
    public PackageInfoObservableModel(string name, string version, string description = null)
    {
        Name = name;
        Version = version;
        Description = description;
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
        set => SetCurrentProperty(value, OnChange);
    }

    /// <summary>
    /// Gets or sets the version.
    /// </summary>
    [DataMember(Name = "version")]
    [JsonPropertyName("version")]
    [Description("The version.")]
    public string Version
    {
        get => GetCurrentProperty<string>();
        set => SetCurrentProperty(value, OnChange);
    }

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    [DataMember(Name = "description")]
    [JsonPropertyName("description")]
    [Description("The description.")]
    public string Description
    {
        get => GetCurrentProperty<string>();
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
    /// Occurs on the property name is changed.
    /// </summary>
    /// <param name="newValue">The new value of the property.</param>
    /// <param name="exist">true if the old value of the property exists; otherwise, false.</param>
    /// <param name="oldValue">The old value of the property.</param>
    protected virtual void OnNameChange(object newValue, bool exist, object oldValue)
    {
    }

    /// <summary>
    /// Occurs on the property version is changed.
    /// </summary>
    /// <param name="newValue">The new value of the property.</param>
    /// <param name="exist">true if the old value of the property exists; otherwise, false.</param>
    /// <param name="oldValue">The old value of the property.</param>
    protected virtual void OnVersionChange(object newValue, bool exist, object oldValue)
    {
    }

    /// <summary>
    /// Occurs on the property description is changed.
    /// </summary>
    /// <param name="newValue">The new value of the property.</param>
    /// <param name="exist">true if the old value of the property exists; otherwise, false.</param>
    /// <param name="oldValue">The old value of the property.</param>
    protected virtual void OnDescriptionChange(object newValue, bool exist, object oldValue)
    {
    }

    private void OnChange(ChangeEventArgs<object> ev)
    {
        if (string.IsNullOrEmpty(ev.Key)) return;
        switch (ev.Key)
        {
            case nameof(Name):
                OnNameChange(ev.NewValue, ev.Method != ChangeMethods.Add, ev.OldValue);
                break;
            case nameof(Version):
                OnVersionChange(ev.NewValue, ev.Method != ChangeMethods.Add, ev.OldValue);
                break;
            case nameof(Description):
                OnDescriptionChange(ev.NewValue, ev.Method != ChangeMethods.Add, ev.OldValue);
                break;
        }
    }
}
