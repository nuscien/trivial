using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
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
    public char? Hotkey { get; }

    /// <summary>
    /// Gets or sets the title to display and for user typing.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Gets or sets the data.
    /// </summary>
    public T Data { get; }
}

/// <summary>
/// The collection selection input information.
/// </summary>
/// <typeparam name="T">The type of data.</typeparam>
public class SelectionData<T>
{
    /// <summary>
    /// The list.
    /// </summary>
    private readonly List<SelectionItem<T>> list = new();

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
    /// Copies a list.
    /// </summary>
    /// <returns>A list copied.</returns>
    public List<SelectionItem<T>> ToList()
        => new(list);
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
    public bool IsCanceled => InputType == SelectionResultTypes.Canceled;

    /// <summary>
    /// Gets a value indicating whether it is not supported.
    /// </summary>
    public bool IsNotSupported { get; internal set; }

    /// <summary>
    /// Gets the index of item displayed.
    /// </summary>
    public int Index { get; }

    /// <summary>
    /// Gets the value input.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Gets the item data.
    /// </summary>
    public T Data { get; }

    /// <summary>
    /// Gets the title displayed in item.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Gets input type.
    /// </summary>
    public SelectionResultTypes InputType { get; }
}
