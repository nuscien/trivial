using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.Json.Serialization;

namespace Trivial.Reflection;

/// <summary>
/// The breadcrumb for node.
/// </summary>
[DebuggerDisplay("{Current}")]
public class NodePathBreadcrumb
{
    /// <summary>
    /// Initializes a new instance of the NodePathBreadcrumb class.
    /// </summary>
    /// <param name="parent">The parent breadcrumb.</param>
    public NodePathBreadcrumb(NodePathBreadcrumb parent)
    {
        ParentBreadcrumb = parent;
    }

    /// <summary>
    /// Initializes a new instance of the NodePathBreadcrumb class.
    /// </summary>
    /// <param name="obj">The current object.</param>
    /// <param name="parent">The parent breadcrumb.</param>
    /// <param name="property">The property name in parent.</param>
    public NodePathBreadcrumb(object obj, NodePathBreadcrumb parent, string property = null)
        : this(parent)
    {
        Current = obj;
        PropertyName = property;
    }

    /// <summary>
    /// Gets the current object.
    /// </summary>
    [JsonPropertyName("value")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object Current { get; }

    /// <summary>
    /// Gets the parent breadcrumb.
    /// </summary>
    [JsonPropertyName("parent")]
    public NodePathBreadcrumb ParentBreadcrumb { get; }

    /// <summary>
    /// Gets a value indicating whether has parent.
    /// </summary>
    [JsonIgnore]
    public bool HasParent => ParentBreadcrumb != null;

    /// <summary>
    /// Gets the parent object.
    /// </summary>
    [JsonIgnore]
    public object Parent => ParentBreadcrumb?.Current;

    /// <summary>
    /// Gets the property name in parent.
    /// </summary>
    [JsonPropertyName("property")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string PropertyName { get; }

    /// <summary>
    /// Gets or sets the additional tag.
    /// </summary>
    [JsonPropertyName("tag")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object Tag { get; set; }

    /// <summary>
    /// Gets the type of the current object.
    /// </summary>
    /// <returns>The type of the current object.</returns>
    public Type GetCurrentType() => Current?.GetType();

    /// <summary>
    /// Gets the root breadcrumb;
    /// </summary>
    /// <returns>The root breadcrumb.</returns>
    public NodePathBreadcrumb GetRootBreadcrumb()
    {
        var root = this;
        while (true)
        {
            var parent = root.ParentBreadcrumb;
            if (parent is null) break;
            if (ReferenceEquals(parent, this)) return null;
            root = parent;
        }

        return root;
    }

    /// <summary>
    /// Gets the root object;
    /// </summary>
    /// <returns>The root object.</returns>
    public object GetRoot()
        => GetRootBreadcrumb()?.Current;
}

/// <summary>
/// The breadcrumb for node.
/// </summary>
/// <typeparam name="T">The type of the object.</typeparam>
public class NodePathBreadcrumb<T> : NodePathBreadcrumb
{
    /// <summary>
    /// Initializes a new instance of the NodePathBreadcrumb class.
    /// </summary>
    /// <param name="current">The current object.</param>
    /// <param name="parent">The parent breadcrumb.</param>
    /// <param name="property">The property name in parent.</param>
    public NodePathBreadcrumb(T current, NodePathBreadcrumb parent, string property = null) : base(current, parent, property)
    {
        Current = current;
    }

    /// <summary>
    /// Gets the current object.
    /// </summary>
    public new T Current { get; }
}

/// <summary>
/// A model with the relationship between the item selected and its parent.
/// </summary>
public class SelectionRelationship<TParent, TItem>
{
    /// <summary>
    /// Initializes a new instance of the SelectionRelationship class.
    /// </summary>
    public SelectionRelationship()
    {
    }

    /// <summary>
    /// Initializes a new instance of the SelectionRelationship class.
    /// </summary>
    /// <param name="tuple">The parent and item selected.</param>
    public SelectionRelationship((TParent, TItem) tuple)
    {
        IsSelected = true;
        Parent = tuple.Item1;
        ItemSelected = tuple.Item2;
    }

    /// <summary>
    /// Initializes a new instance of the SelectionRelationship class.
    /// </summary>
    /// <param name="parent">The parent.</param>
    public SelectionRelationship(TParent parent)
    {
        Parent = parent;
    }

    /// <summary>
    /// Initializes a new instance of the SelectionRelationship class.
    /// </summary>
    /// <param name="parent">The parent.</param>
    /// <param name="itemSelected">The item selected.</param>
    public SelectionRelationship(TParent parent, TItem itemSelected)
    {
        IsSelected = true;
        Parent = parent;
        ItemSelected = itemSelected;
    }

    /// <summary>
    /// Gets a value indicating whether there is an item selected.
    /// </summary>
    public bool IsSelected { get; }

    /// <summary>
    /// Gets the parent which contains the item.
    /// </summary>
    public TParent Parent { get; }

    /// <summary>
    /// Gets the item selected.
    /// </summary>
    public TItem ItemSelected { get; }

    /// <summary>
    /// Converts to tuple.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <return>The tuple.</return>
    public static explicit operator (TParent, TItem, bool)(SelectionRelationship<TParent, TItem> value)
    {
        return value != null ? (value.Parent, value.ItemSelected, value.IsSelected) : (default(TParent), default(TItem), false);
    }
}
