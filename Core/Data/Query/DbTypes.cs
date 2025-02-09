// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbTypes.cs" company="Nanchang Jinchen Software Co., Ltd.">
//   Copyright (c) 2010 Nanchang Jinchen Software Co., Ltd. All rights reserved.
// </copyright>
// <summary>
//   The types of each operations for database.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Trivial.Data;

/// <summary>
/// Specifies database value types.
/// </summary>
public enum DbValueType : byte
{
    /// <summary>
    /// String.
    /// </summary>
    LiteralString = 0,

    /// <summary>
    /// Integer with 32 bits length.
    /// </summary>
    Int32 = 1,

    /// <summary>
    /// Single floating number.
    /// </summary>
    SingleFloating = 2,

    /// <summary>
    /// Date time UTC.
    /// </summary>
    DateTimeUtc = 3,
    
    /// <summary>
    /// Boolean.
    /// </summary>
    Boolean = 4,

    /// <summary>
    /// Integer with 64 bits length.
    /// </summary>
    Int64 = 5,

    /// <summary>
    /// Double floating number.
    /// </summary>
    DoubleFloating = 6,

    /// <summary>
    /// Date time with offset.
    /// </summary>
    DateTimeOffset = 7
}
