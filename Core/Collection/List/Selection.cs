using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Trivial.Collection;

/// <summary>
/// The input types for selection result.
/// </summary>
public enum SelectionResultTypes : byte
{
    /// <summary>
    /// The operation has been cancelled.
    /// </summary>
    Canceled = 0,

    /// <summary>
    /// The result is by selecting.
    /// </summary>
    Selected = 1,

    /// <summary>
    /// The result is by manual input.
    /// </summary>
    Typed = 2,

    /// <summary>
    /// The selection is not supported.
    /// </summary>
    NotSupported = 3
}

/// <summary>
/// The model of the selection item.
/// </summary>
/// <typeparam name="T">The type of data.</typeparam>
public class SelectionItem<T>
{
    /// <summary>
    /// Initialzies a new instance of the SelectionItem class.
    /// </summary>
    /// <param name="title">The description displayed in item.</param>
    /// <param name="data">The optional data.</param>
    public SelectionItem(string title, T data = default)
    {
        Hotkey = null;
        Data = data;
        Title = title;
    }

    /// <summary>
    /// Initialzies a new instance of the SelectionItem class.
    /// </summary>
    /// <param name="key">The hotkey mapped.</param>
    /// <param name="title">The description displayed in item.</param>
    /// <param name="data">The optional data.</param>
    public SelectionItem(char key, string title, T data = default)
    {
        Hotkey = key;
        Data = data;
        Title = title;
    }

    /// <summary>
    /// Gets or sets the hotkey.
    /// </summary>
    [JsonPropertyName("hotkey")]
    public char? Hotkey { get; }

    /// <summary>
    /// Gets or sets the title to display and for user typing.
    /// </summary>
    [JsonPropertyName("name")]
    public string Title { get; }

    /// <summary>
    /// Gets or sets the data.
    /// </summary>
    [JsonPropertyName("data")]
    public T Data { get; }

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
        => Title ?? string.Empty;
}

/// <summary>
/// The collection selection input information.
/// </summary>
/// <typeparam name="T">The type of data.</typeparam>
public class SelectionData<T> : IList<SelectionItem<T>>
{
    /// <summary>
    /// The list.
    /// </summary>
    private readonly List<SelectionItem<T>> list = new();

    /// <summary>
    /// Gets the specific item by index.
    /// </summary>
    /// <param name="index">The index of the item to get.</param>
    /// <returns>The item at the specified index.</returns>
    public SelectionItem<T> this[int index]
    {
        get => list[index];
        set => list[index] = value;
    }

    /// <summary>
    /// Gets the count of items in the selection data.
    /// </summary>
    public int Count => list.Count;

    bool ICollection<SelectionItem<T>>.IsReadOnly => ((IList<SelectionItem<T>>)list).IsReadOnly;

    /// <summary>
    /// Adds an item.
    /// </summary>
    /// <param name="value">The value to output.</param>
    public void Add(SelectionItem<T> value)
    {
        if (value == null) return;
        list.Add(value);
    }

    /// <summary>
    /// Adds an item.
    /// </summary>
    /// <param name="title">The description displayed in item.</param>
    /// <param name="data">The optional data.</param>
    public void Add(string title, T data = default)
        => list.Add(new SelectionItem<T>(title, data));

    /// <summary>
    /// Adds an item.
    /// </summary>
    /// <param name="key">The hotkey mapped.</param>
    /// <param name="title">The description displayed in item.</param>
    /// <param name="data">The optional data.</param>
    public void Add(char key, string title, T data = default)
        => list.Add(new SelectionItem<T>(key, title, data));

    /// <summary>
    /// Adds items to the end.
    /// </summary>
    /// <param name="values">The values to output.</param>
    public void AddRange(IEnumerable<string> values)
    {
        if (values != null) list.AddRange(values.Select(item => new SelectionItem<T>(item, default)));
    }

    /// <summary>
    /// Adds items to the end.
    /// </summary>
    /// <param name="values">The values to output.</param>
    public void AddRange(IEnumerable<SelectionItem<T>> values)
    {
        if (values != null) list.AddRange(values.Where(ele => ele != null));
    }

    /// <summary>
    /// Clears the contents of selection data.
    /// </summary>
    public void Clear()
        => list.Clear();

    /// <summary>
    /// Tests if the specified element is contained in the selection data.
    /// </summary>
    /// <param name="item">The item to test if contains.</param>
    /// <returns>true if the specified element is in the selection data; otherwise, false.</returns>
    public bool Contains(SelectionItem<T> item)
        => list.Contains(item);

    /// <summary>
    /// Copies the elements of the selection data to an array, starting at a particular array index.
    /// </summary>
    /// <param name="array">The array to copy the elements to.</param>
    /// <param name="arrayIndex">The index in the array at which to start copying.</param>
    public void CopyTo(SelectionItem<T>[] array, int arrayIndex)
        => list.CopyTo(array, arrayIndex);

    /// <summary>
    /// Gets the item.
    /// </summary>
    /// <param name="c">The hotkey.</param>
    /// <param name="index">The index output.</param>
    /// <returns>The item.</returns>
    public SelectionItem<T> Get(char c, out int index)
    {
        var i = -1;
        foreach (var item in list)
        {
            i++;
            if (!item.Hotkey.HasValue || item.Hotkey != c) continue;
            index = i;
            return item;
        }

        index = -1;
        return null;
    }

    /// <summary>
    /// Gets the item.
    /// </summary>
    /// <param name="c">The hotkey.</param>
    /// <returns>The item.</returns>
    public SelectionItem<T> Get(char c)
        => Get(c, out _);

    /// <summary>
    /// Gets an enumerator for this selection data with the given permission for removal of elements.
    /// </summary>
    /// <returns>An enumerator instance used by foreach.</returns>
    public IEnumerator<SelectionItem<T>> GetEnumerator()
        => list.GetEnumerator();

    /// <summary>
    /// Returns the index of the first occurrence of a given value in a range of this list. The list is searched forwards from beginning to end.
    /// The items of the selection data are compared to the given value using the Object.Equals method.
    /// </summary>
    /// <param name="item">The object to locate in the list.</param>
    /// <returns>The index of item if found in the list; otherwise, -1.</returns>
    public int IndexOf(SelectionItem<T> item)
        => list.IndexOf(item);
    
    /// <summary>
    /// Inserts an item at the specified index.
    /// </summary>
    /// <param name="index">The index at which to insert the item.</param>
    /// <param name="item">The item to insert.</param>
    public void Insert(int index, SelectionItem<T> item)
        => list.Insert(index, item);

    /// <summary>
    /// Removes the first occurrence of a specific object from the list.
    /// </summary>
    /// <param name="item">The item to remove.</param>
    /// <returns>true if the item was removed; otherwise, false.</returns>
    public bool Remove(SelectionItem<T> item)
        => list.Remove(item);

    /// <summary>
    /// Removes the item at the specified index.
    /// </summary>
    /// <param name="index">The index of the item to remove.</param>
    public void RemoveAt(int index)
        => list.RemoveAt(index);

    /// <summary>
    /// Copies a list.
    /// </summary>
    /// <returns>A list copied.</returns>
    public List<SelectionItem<T>> ToList()
        => new(list);

    IEnumerator IEnumerable.GetEnumerator()
        => ((IEnumerable)list).GetEnumerator();
}

/// <summary>
/// The result of the collection selection.
/// </summary>
/// <typeparam name="T">The type of data.</typeparam>
public class SelectionResult<T>
{
    /// <summary>
    /// Initializes a new instance of the SelectionResult class.
    /// </summary>
    /// <param name="value">The value input.</param>
    /// <param name="type">The input type.</param>
    public SelectionResult(string value, SelectionResultTypes type)
    {
        Value = value;
        Index = -1;
        Data = default;
        InputType = type;
        if (type == SelectionResultTypes.NotSupported) IsNotSupported = true;
    }

    /// <summary>
    /// Initializes a new instance of the SelectionResult class.
    /// </summary>
    /// <param name="value">The value input.</param>
    /// <param name="index">The index of item displayed.</param>
    /// <param name="data">The item data.</param>
    /// <param name="title">The title displayed in item.</param>
    /// <param name="type">The input type.</param>
    public SelectionResult(string value, int index, T data, string title, SelectionResultTypes type = SelectionResultTypes.Selected)
    {
        Value = value;
        Index = index;
        Data = data;
        Title = title;
        InputType = type;
    }

    /// <summary>
    /// Gets a value indicating whether it is cancelled.
    /// </summary>
    [JsonIgnore]
    public bool IsCanceled => InputType == SelectionResultTypes.Canceled;

    /// <summary>
    /// Gets a value indicating whether it is not supported.
    /// </summary>
    [JsonIgnore]
    public bool IsNotSupported { get; internal set; }

    /// <summary>
    /// Gets a value indicating whether the title is null and value is null or empty.
    /// </summary>
    [JsonIgnore]
    public bool IsEmpty => string.IsNullOrEmpty(Title ?? Value);

    /// <summary>
    /// Gets the index of item displayed.
    /// </summary>
    [JsonPropertyName("index")]
    public int Index { get; }

    /// <summary>
    /// Gets the value input.
    /// </summary>
    [JsonPropertyName("value")]
    public string Value { get; }

    /// <summary>
    /// Gets the item data.
    /// </summary>
    [JsonPropertyName("data")]
    public T Data { get; }

    /// <summary>
    /// Gets the title displayed in item.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; }

    /// <summary>
    /// Gets input type.
    /// </summary>
    [JsonPropertyName("method")]
    public SelectionResultTypes InputType { get; }

    /// <summary>
    /// Gets the type of data.
    /// </summary>
    /// <returns>The data type.</returns>
    public Type GetDataType()
        => Data?.GetType() ?? typeof(T);

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
    {
        var text = Title ?? Value;
        if (string.IsNullOrWhiteSpace(text)) text = InputType.ToString();
        return text;
    }
}
