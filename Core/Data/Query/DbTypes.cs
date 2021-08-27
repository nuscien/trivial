// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbTypes.cs" company="Nanchang Jinchen Software Co., Ltd.">
//   Copyright (c) 2010 Nanchang Jinchen Software Co., Ltd. All rights reserved.
// </copyright>
// <summary>
//   The types of each operations for database.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Trivial.Data
{
    /// <summary>
    /// Specifies database value types.
    /// </summary>
    public enum DbValueType
    {
        /// <summary>
        /// String.
        /// </summary>
        LiteralString = 0,

        /// <summary>
        /// Integer with 32 bit length.
        /// </summary>
        Int32 = 1,

        /// <summary>
        /// Single decimal number.
        /// </summary>
        SingleDecimal = 2,

        /// <summary>
        /// Date time UTC.
        /// </summary>
        DateTimeUtc = 3,
        
        /// <summary>
        /// Boolean.
        /// </summary>
        Boolean = 4,

        /// <summary>
        /// Integer with 64 bit length.
        /// </summary>
        Int64 = 5,

        /// <summary>
        /// Double decimal number.
        /// </summary>
        DoubleDecimal = 6,

        /// <summary>
        /// Date time with offset.
        /// </summary>
        DateTimeOffset = 7
    }
}
