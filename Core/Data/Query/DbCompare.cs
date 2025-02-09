﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbTypes.cs" company="Nanchang Jinchen Software Co., Ltd.">
//   Copyright (c) 2010 Nanchang Jinchen Software Co., Ltd. All rights reserved.
// </copyright>
// <summary>
//   The types of each operations for database.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Trivial.Data;

/// <summary>
/// Specifies comparing operators for database.
/// </summary>
public enum DbCompareOperator : byte
{
    /// <summary>
    /// Equaling.
    /// </summary>
    Equal = 0,

    /// <summary>
    /// Not equaling.
    /// </summary>
    NotEqual = 1,

    /// <summary>
    /// Greater than.
    /// </summary>
    Greater = 2,

    /// <summary>
    /// Less than.
    /// </summary>
    Less = 3,

    /// <summary>
    /// Greater than or equaling.
    /// </summary>
    GreaterOrEqual = 4,

    /// <summary>
    /// Less than or equaling.
    /// </summary>
    LessOrEqual = 5,

    /// <summary>
    /// The literal is about containing the specific one.
    /// </summary>
    Contains = 6,

    /// <summary>
    /// The literal is about starting with specific one.
    /// </summary>
    StartsWith = 7,

    /// <summary>
    /// The literal is about ending with specific one.
    /// </summary>
    EndsWith = 8
}

/// <summary>
/// Specifies the sequence way.
/// </summary>
public enum SortingOrder : byte
{
    /// <summary>
    /// Default sequence.
    /// </summary>
    None = 0,

    /// <summary>
    /// Ascending
    /// </summary>
    Ascending = 1,

    /// <summary>
    /// Descending
    /// </summary>
    Descending = 2
}
