using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trivial.Collection;

/// <summary>
/// Represents a lookup that can be edited by adding values to key-based groups.
/// </summary>
/// <typeparam name="TKey">The type of keys in the lookup.</typeparam>
/// <typeparam name="TItem">The type of values in each key-based group.</typeparam>
public class EditableLookup<TKey, TItem> : Dictionary<TKey, List<TItem>>, ILookup<TKey, TItem>
{
    internal class GroupingItem : List<TItem>, IGrouping<TKey, TItem>
    {
        public GroupingItem(TKey key, IEnumerable<TItem> items)
        {
            Key = key;
            if (items is not null) AddRange(items);
        }

        public TKey Key { get; }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            => base.GetEnumerator();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EditableLookup{TKey, TItem}"/> class.
    /// </summary>
    public EditableLookup()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EditableLookup{TKey, TItem}"/> class that uses the specified equality comparer for the keys.
    /// </summary>
    /// <param name="comparer">The equality comparer to use for the keys.</param>
    public EditableLookup(IEqualityComparer<TKey> comparer)
        : base(comparer)
    {
    }

    IEnumerable<TItem> ILookup<TKey, TItem>.this[TKey key] => base[key];

    bool ILookup<TKey, TItem>.Contains(TKey key)
        => ContainsKey(key);

    IEnumerator<IGrouping<TKey, TItem>> IEnumerable<IGrouping<TKey, TItem>>.GetEnumerator()
    {
        var list = new List<IGrouping<TKey, TItem>>();
        foreach (var item in this)
        {
            list.Add(new GroupingItem(item.Key, item.Value));
        }

        return list.GetEnumerator();
    }

    /// <summary>
    /// Adds a value to the group associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the group to add the value to.</param>
    /// <param name="value">The value to add.</param>
    public void AddItem(TKey key, TItem value)
    {
        if (!TryGetValue(key, out var list) || list is null)
        {
            list = [];
            this[key] = list;
        }

        list.Add(value);
    }

    /// <summary>
    /// Adds values to the group associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the group to add values to.</param>
    /// <param name="values">The values to add.</param>
    public void AddItem(TKey key, IEnumerable<TItem> values)
    {
        if (!TryGetValue(key, out var list) || list is null)
        {
            list = [];
            this[key] = list;
        }
        
        list.AddRange(values);
    }

    /// <summary>
    /// Adds values to the group associated with the specified key.
    /// </summary>
    /// <param name="values">The values to add.</param>
    public void AddItem(IGrouping<TKey, TItem> values)
    {
        if (!TryGetValue(values.Key, out var list) || list is null)
        {
            list = [];
            this[values.Key] = list;
        }

        list.AddRange(values);
    }

    /// <summary>
    /// Adds values to the group associated with the specified key.
    /// </summary>
    /// <param name="values">The values to add.</param>
    public void AddItem(KeyValuePair<TKey, IEnumerable<TItem>> values)
    {
        if (!TryGetValue(values.Key, out var list) || list is null)
        {
            list = [];
            this[values.Key] = list;
        }

        list.AddRange(values.Value);
    }

    /// <summary>
    /// Adds value to the group associated with the specified key.
    /// </summary>
    /// <param name="value">The value to add.</param>
    public void AddItem(KeyValuePair<TKey, TItem> value)
    {
        if (!TryGetValue(value.Key, out var list) || list is null)
        {
            list = [];
            this[value.Key] = list;
        }

        list.Add(value.Value);
    }

    /// <summary>
    /// Tries to get the list of values associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the group to get the values for.</param>
    /// <returns>The list of values associated with the specified key, or null if the key does not exist.</returns>
    public List<TItem> TryGetValue(TKey key)
        => TryGetValue(key, out var list) ? list : null;
}
