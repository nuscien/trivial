using System;
using System.Collections.Generic;
using System.Text;

namespace Trivial.Data;

/// <summary>
/// The model with GUID property Id.
/// </summary>
public interface IGuidPropertyModel
{
    /// <summary>
    /// Gets the identifier of the model.
    /// </summary>
    public Guid Id { get; }
}

/// <summary>
/// The model with string property Id.
/// </summary>
public interface IIdPropertyModel
{
    /// <summary>
    /// Gets the identifier.
    /// </summary>
    public string Id { get; }
}

/// <summary>
/// The model with property name.
/// </summary>
public interface INamePropertyModel
{
    /// <summary>
    /// Gets the name of the model.
    /// </summary>
    public string Name { get; }
}

/// <summary>
/// The model with property title.
/// </summary>
public interface ITitlePropertyModel
{
    /// <summary>
    /// Gets the title of the model.
    /// </summary>
    public string Title { get; }
}

/// <summary>
/// The model with property description.
/// </summary>
public interface IDescriptionPropertyModel
{
    /// <summary>
    /// Gets the description.
    /// </summary>
    public string Description { get; }
}
