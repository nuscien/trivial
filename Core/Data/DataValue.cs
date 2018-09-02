// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataValue.cs" company="Nanchang Jinchen Software Co., Ltd.">
//   Copyright (c) 2010 Nanchang Jinchen Software Co., Ltd. All rights reserved.
// </copyright>
// <summary>
//   The data value for definition for a list row or a table column.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trivial.Data
{
    /// <summary>
    /// The sources of data form value.
    /// </summary>
    public enum DataValueSource
    {
        /// <summary>
        /// The static source.
        /// </summary>
        Static = 0,

        /// <summary>
        /// The text source.
        /// </summary>
        Text = 1,

        /// <summary>
        /// The source of data linking with property path.
        /// </summary>
        Data = 2,

        /// <summary>
        /// The source linked by url.
        /// </summary>
        Url = 3,

        /// <summary>
        /// The source provided by request job.
        /// </summary>
        Job = 4,

        /// <summary>
        /// The function name.
        /// </summary>
        Function = 5
    }

    /// <summary>
    /// The types of data form field.
    /// </summary>
    public enum DataFieldType
    {
        /// <summary>
        /// The hidden field.
        /// </summary>
        Hidden = 0,

        /// <summary>
        /// The simple content field.
        /// </summary>
        SimpleContent = 1,

        /// <summary>
        /// The specific function provider field.
        /// </summary>
        Function = 2,

        /// <summary>
        /// The normal text field.
        /// </summary>
        Text = 3,

        /// <summary>
        /// The email field.
        /// </summary>
        Email = 4,

        /// <summary>
        /// The phone field.
        /// </summary>
        Phone = 5,

        /// <summary>
        /// The password field.
        /// </summary>
        Password = 6,

        /// <summary>
        /// The date field.
        /// </summary>
        Date = 7,

        /// <summary>
        /// The time field.
        /// </summary>
        Time = 8,

        /// <summary>
        /// The date and time field.
        /// </summary>
        DateTime = 9,

        /// <summary>
        /// The integer number field.
        /// </summary>
        Integer = 10,

        /// <summary>
        /// The decimal number field.
        /// </summary>
        Decimal = 11,

        /// <summary>
        /// The person name field.
        /// </summary>
        Name = 12,

        /// <summary>
        /// The single selection field.
        /// </summary>
        Single = 13,

        /// <summary>
        /// The tags field.
        /// </summary>
        Tags = 14,

        /// <summary>
        /// The checkbox field.
        /// </summary>
        Checkbox = 15,

        /// <summary>
        /// The richtext field.
        /// </summary>
        Richtext = 16,

        /// <summary>
        /// The image file field.
        /// </summary>
        Image = 17,

        /// <summary>
        /// The music file field.
        /// </summary>
        Music = 18,

        /// <summary>
        /// The movie file field.
        /// </summary>
        Movie = 19,

        /// <summary>
        /// The documentation file field.
        /// </summary>
        Doc = 20,

        /// <summary>
        /// The zip file field.
        /// </summary>
        Zip = 21,

        /// <summary>
        /// The other type of field.
        /// </summary>
        Other = 63
    }

    /// <summary>
    /// Customized value reference.
    /// </summary>
    public class CustomizedValueReference
    {
        /// <summary>
        /// Gets or sets the value of the field.
        /// </summary>
        public virtual object Value { get; set; }

        /// <summary>
        /// Gets or sets the data source of the field.
        /// </summary>
        public virtual DataValueSource Source { get; set; }

        /// <summary>
        /// Gets or sets the data source of the field.
        /// </summary>
        public string SourceString { get { return Source.ToString().ToLower(); } }
    }

    /// <summary>
    /// Customized value reference.
    /// </summary>
    public class DataValueReference : CustomizedValueReference
    {
        /// <summary>
        /// Gets or sets the data source of the field.
        /// </summary>
        public override DataValueSource Source { get { return DataValueSource.Data; } }

        /// <summary>
        /// Gets or sets a value indicating whether the source is from original.
        /// </summary>
        public bool IsFromOriginal { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the source is only an appending parameter.
        /// </summary>
        public bool IsAppendParameter { get; set; }
    }
}
