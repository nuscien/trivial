// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Criteria.cs" company="Nanchang Jinchen Software Co., Ltd.">
//   Copyright (c) 2010 Nanchang Jinchen Software Co., Ltd. All rights reserved.
// </copyright>
// <summary>
//   Query criteria.
// </summary>
// <author>Kingcean Tuan</author>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Trivial.Maths;

namespace Trivial.Data;

/// <summary>
/// Criteria types.
/// </summary>
public enum CriteriaType : byte
{
    /// <summary>
    /// For all resources without query criteria.
    /// </summary>
    All = 0,

    /// <summary>
    /// The collection query criteria.
    /// </summary>
    Collection = 1,

    /// <summary>
    /// The property query criteria.
    /// </summary>
    Property = 2
}

/// <summary>
/// The query criteria interface.
/// </summary>
public interface ICriteria
{
    /// <summary>
    /// Gets the query criteria type.
    /// </summary>
    CriteriaType CriteriaType { get; }
}

/// <summary>
/// Collection query criteria.
/// </summary>
public sealed class CollectionCriteria : List<ICriteria>, ICriteria
{
    /// <summary>
    /// Gets the query criteria type.
    /// </summary>
    public CriteriaType CriteriaType => CriteriaType.Collection;

    /// <summary>
    /// Gets or sets the value of operator.
    /// </summary>
    public CriteriaBooleanOperator Operator { get; set; }

    /// <summary>
    /// Initializes a new instance of the CollectionCriteria class.
    /// </summary>
    /// <remarks>You can use this to initialize an instance for the class.</remarks>
    public CollectionCriteria()
    {
        Operator = CriteriaBooleanOperator.And;
    }

    /// <summary>
    /// Initializes a new instance of the CollectionCriteria class.
    /// </summary>
    /// <param name="op">The collection operator for each item.</param>
    /// <remarks>You can use this to initialize an instance for the class.</remarks>
    public CollectionCriteria(CriteriaBooleanOperator op)
    {
        Operator = op;
    }

    /// <summary>
    /// AND operator for criteria.
    /// </summary>
    /// <param name="x">Criteria x.</param>
    /// <param name="y">Criteria y.</param>
    /// <returns>A collection of criteria.</returns>
    public static CollectionCriteria operator &(CollectionCriteria x, ICriteria y)
        => And(x, y);

    /// <summary>
    /// AND operator for criteria.
    /// </summary>
    /// <param name="x">Criteria x.</param>
    /// <param name="y">Criteria y.</param>
    /// <returns>A collection of criteria.</returns>
    public static CollectionCriteria operator &(ICriteria x, CollectionCriteria y)
        => And(x, y);

    /// <summary>
    /// AND operator for criteria.
    /// </summary>
    /// <param name="x">Criteria x.</param>
    /// <param name="y">Criteria y.</param>
    /// <returns>A collection of criteria.</returns>
    public static CollectionCriteria operator &(CollectionCriteria x, CollectionCriteria y)
        => And(x, y);

    /// <summary>
    /// OR operator for criteria.
    /// </summary>
    /// <param name="x">Criteria x.</param>
    /// <param name="y">Criteria y.</param>
    /// <returns>A collection of criteria.</returns>
    public static CollectionCriteria operator |(CollectionCriteria x, ICriteria y)
        => Or(x, y);

    /// <summary>
    /// OR operator for criteria.
    /// </summary>
    /// <param name="x">Criteria x.</param>
    /// <param name="y">Criteria y.</param>
    /// <returns>A collection of criteria.</returns>
    public static CollectionCriteria operator |(ICriteria x, CollectionCriteria y)
        => Or(x, y);

    /// <summary>
    /// OR operator for criteria.
    /// </summary>
    /// <param name="x">Criteria x.</param>
    /// <param name="y">Criteria y.</param>
    /// <returns>A collection of criteria.</returns>
    public static CollectionCriteria operator |(CollectionCriteria x, CollectionCriteria y)
        => Or(x, y);

    /// <summary>
    /// Creates a collection criteria with AND operation.
    /// </summary>
    /// <param name="criteriaA">Criteria A.</param>
    /// <param name="criteriaB">Criteria B.</param>
    /// <param name="criterias">Other criterias.</param>
    /// <returns>A collection criteria</returns>
    public static CollectionCriteria And(ICriteria criteriaA, ICriteria criteriaB, params ICriteria[] criterias)
    {
        var cri = new CollectionCriteria { Operator = CriteriaBooleanOperator.And };
        if (criteriaA != null) cri.Add(criteriaA);
        if (criteriaB != null) cri.Add(criteriaB);
        if (criterias != null && criterias.Length > 0) cri.AddRange(criterias);
        return cri;
    }

    /// <summary>
    /// Creates a collection criteria with Or operation.
    /// </summary>
    /// <param name="criteriaA">Criteria A.</param>
    /// <param name="criteriaB">Criteria B.</param>
    /// <param name="criterias">Other criterias.</param>
    /// <returns>A collection criteria</returns>
    public static CollectionCriteria Or(ICriteria criteriaA, ICriteria criteriaB, params ICriteria[] criterias)
    {
        var cri = new CollectionCriteria { Operator = CriteriaBooleanOperator.Or };
        if (criteriaA != null) cri.Add(criteriaA);
        if (criteriaB != null) cri.Add(criteriaB);
        if (criterias != null && criterias.Length > 0) cri.AddRange(criterias);
        return cri;
    }

    /// <summary>
    /// Gets if given criteria is for all.
    /// </summary>
    /// <param name="criteria">The criteria for testing.</param>
    /// <returns>true if for getting all; otherwise, false.</returns>
    public static bool IsForAll(ICriteria criteria)
    {
        if (criteria == null || criteria.CriteriaType == CriteriaType.All) return true;
        if (criteria.CriteriaType == CriteriaType.Collection)
        {
            if (criteria is IEnumerable<ICriteria> col && !col.Any()) return true;
        }
        else if (criteria.CriteriaType == CriteriaType.Property)
        {
            if (criteria is PropertyCriteria prop
                && prop.Condition.ValueIsNull
                && (prop.Condition.Operator == DbCompareOperator.Contains || prop.Condition.Operator == DbCompareOperator.StartsWith || prop.Condition.Operator == DbCompareOperator.EndsWith))
                return true;
        }

        return false;
    }

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
    {
        var str = new StringBuilder();
        var step = 0;
        foreach (var item in this)
        {
            if (item == null) continue;
            if (step > 0) str.AppendFormat(CultureInfo.InvariantCulture, " {0} ", Operator.ToString());
            str.AppendFormat(CultureInfo.InvariantCulture, "({0})", item.ToString());
            step++;
        }

        return str.ToString();
    }
}

/// <summary>
/// Property query criteria.
/// </summary>
public sealed class PropertyCriteria : ICriteria
{
    /// <summary>
    /// Initializes a new instance of the PropertyCriteria class.
    /// </summary>
    /// <remarks>You can use this to initialize an instance for the class.</remarks>
    public PropertyCriteria()
    {
    }

    /// <summary>
    /// Initializes a new instance of the PropertyCriteria class.
    /// </summary>
    /// <param name="name">The property name.</param>
    /// <param name="condition">The condition.</param>
    /// <remarks>You can use this to initialize an instance for the class.</remarks>
    public PropertyCriteria(string name, ISimpleCondition condition)
    {
        Name = name;
        Condition = condition;
    }

    /// <summary>
    /// Initializes a new instance of the PropertyCriteria class.
    /// </summary>
    /// <param name="name">The property name.</param>
    /// <param name="op">The operator.</param>
    /// <param name="value">The value of property.</param>
    /// <remarks>You can use this to initialize an instance for the class.</remarks>
    public PropertyCriteria(string name, DbCompareOperator op, string value)
    {
        Name = name;
        Condition = new StringCondition { Operator = op, Value = value };
    }

    /// <summary>
    /// Initializes a new instance of the PropertyCriteria class.
    /// </summary>
    /// <param name="name">The property name.</param>
    /// <param name="op">The operator.</param>
    /// <param name="value">The value of property.</param>
    /// <remarks>You can use this to initialize an instance for the class.</remarks>
    public PropertyCriteria(string name, BasicCompareOperator op, int value)
    {
        Name = name;
        var ope = ToOperator(op);
        Condition = new Int32Condition { Operator = ope, Value = value };
    }

    /// <summary>
    /// Initializes a new instance of the PropertyCriteria class.
    /// </summary>
    /// <param name="name">The property name.</param>
    /// <param name="op">The operator.</param>
    /// <param name="value">The value of property.</param>
    /// <remarks>You can use this to initialize an instance for the class.</remarks>
    public PropertyCriteria(string name, BasicCompareOperator op, float value)
    {
        Name = name;
        var ope = ToOperator(op);
        Condition = new SingleCondition { Operator = ope, Value = value };
    }

    /// <summary>
    /// Initializes a new instance of the PropertyCriteria class.
    /// </summary>
    /// <param name="name">The property name.</param>
    /// <param name="op">The operator.</param>
    /// <param name="value">The value of property.</param>
    /// <remarks>You can use this to initialize an instance for the class.</remarks>
    public PropertyCriteria(string name, BasicCompareOperator op, DateTime value)
    {
        Name = name;
        var ope = ToOperator(op);
        Condition = new DateTimeCondition { Operator = ope, Value = value };
    }

    /// <summary>
    /// Gets the query criteria type.
    /// </summary>
    public CriteriaType CriteriaType => CriteriaType.Property;

    /// <summary>
    /// Gets or sets property name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets condition.
    /// </summary>
    public ISimpleCondition Condition { get; set; }

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
        => string.Format(
            CultureInfo.InvariantCulture,
            "[{0}] {1}",
            Name ?? "[empty]",
            Condition != null ? Condition.ToString() : "null");

    /// <summary>
    /// Converts given basic compare operator to database compare operator.
    /// </summary>
    /// <param name="op">A basic compare operator to convert.</param>
    /// <returns>A database compare operator converted.</returns>
    public static DbCompareOperator ToOperator(BasicCompareOperator op)
        => op switch
        {
            BasicCompareOperator.Greater => DbCompareOperator.Greater,
            BasicCompareOperator.GreaterOrEqual => DbCompareOperator.GreaterOrEqual,
            BasicCompareOperator.Less => DbCompareOperator.Less,
            BasicCompareOperator.LessOrEqual => DbCompareOperator.LessOrEqual,
            BasicCompareOperator.NotEqual => DbCompareOperator.NotEqual,
            _ => DbCompareOperator.Equal,
        };

    /// <summary>
    /// AND operator for criteria.
    /// </summary>
    /// <param name="x">Criteria x.</param>
    /// <param name="y">Criteria y.</param>
    /// <returns>A collection of criteria.</returns>
    public static CollectionCriteria operator &(PropertyCriteria x, ICriteria y)
        => CollectionCriteria.And(x, y);

    /// <summary>
    /// AND operator for criteria.
    /// </summary>
    /// <param name="x">Criteria x.</param>
    /// <param name="y">Criteria y.</param>
    /// <returns>A collection of criteria.</returns>
    public static CollectionCriteria operator &(ICriteria x, PropertyCriteria y)
        => CollectionCriteria.And(x, y);

    /// <summary>
    /// AND operator for criteria.
    /// </summary>
    /// <param name="x">Criteria x.</param>
    /// <param name="y">Criteria y.</param>
    /// <returns>A collection of criteria.</returns>
    public static CollectionCriteria operator &(PropertyCriteria x, PropertyCriteria y)
        => CollectionCriteria.And(x, y);

    /// <summary>
    /// OR operator for criteria.
    /// </summary>
    /// <param name="x">Criteria x.</param>
    /// <param name="y">Criteria y.</param>
    /// <returns>A collection of criteria.</returns>
    public static CollectionCriteria operator |(PropertyCriteria x, ICriteria y)
        => CollectionCriteria.Or(x, y);

    /// <summary>
    /// OR operator for criteria.
    /// </summary>
    /// <param name="x">Criteria x.</param>
    /// <param name="y">Criteria y.</param>
    /// <returns>A collection of criteria.</returns>
    public static CollectionCriteria operator |(ICriteria x, PropertyCriteria y)
        => CollectionCriteria.Or(x, y);

    /// <summary>
    /// OR operator for criteria.
    /// </summary>
    /// <param name="x">Criteria x.</param>
    /// <param name="y">Criteria y.</param>
    /// <returns>A collection of criteria.</returns>
    public static CollectionCriteria operator |(PropertyCriteria x, PropertyCriteria y)
        => CollectionCriteria.Or(x, y);
}
