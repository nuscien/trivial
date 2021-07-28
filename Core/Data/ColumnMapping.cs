using System;
using System.Collections.Generic;
using System.Text;

using Trivial.Text;

namespace Trivial.Data
{
    /// <summary>
    /// Column mapping.
    /// </summary>
    public class ColumnMapping : List<ColumnMappingItem>
    {
        /// <summary>
        /// Adds a column-property-mapping item.
        /// </summary>
        /// <param name="columnName">The column name.</param>
        /// <param name="propertyName">The property name.</param>
        public void Add(string columnName, string propertyName)
        {
            if (string.IsNullOrWhiteSpace(columnName)) return;
            if (string.IsNullOrWhiteSpace(propertyName)) propertyName = columnName;
            Add(new ColumnMappingItem(columnName, propertyName));
        }

        /// <summary>
        /// Adds a column-property-mapping item.
        /// </summary>
        /// <param name="columnName">The column name.</param>
        /// <param name="propertyNameCase">The case for column name convert to property name.</param>
        public void AddColumn(string columnName, Cases propertyNameCase = Cases.Original)
        {
            Add(columnName, StringExtensions.ToSpecificCaseInvariant(columnName, propertyNameCase));
        }

        /// <summary>
        /// Adds a column-property-mapping item.
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        /// <param name="columnNameCase">The case for property name convert to column name.</param>
        public void AddProperty(string propertyName, Cases columnNameCase = Cases.Original)
        {
            Add(StringExtensions.ToSpecificCaseInvariant(propertyName, columnNameCase), propertyName);
        }

        /// <summary>
        /// Gets column mapping for a specific class.
        /// </summary>
        /// <param name="type">The type to get column mapping.</param>
        /// <returns>A collection of column mapping between database and the class.</returns>
        public static ColumnMapping Load(Type type)
        {
            var mapping = new ColumnMapping();
            var customized = type.GetMethod("TableMapping", Type.EmptyTypes);
            if (customized != null && customized.IsStatic)
            {
                return customized.Invoke(null, null) as ColumnMapping;
            }

            foreach (var prop in type.GetProperties())
            {
                foreach (var attr in prop.GetCustomAttributes(typeof(ColumnMappingAttribute), true))
                {
                    if (attr is not ColumnMappingAttribute info) continue;
                    mapping.Add(info.Name, prop.Name);
                }
            }

            return mapping;
        }
    }

    /// <summary>
    /// Column mapping item.
    /// </summary>
    public class ColumnMappingItem
    {
        /// <summary>
        /// Initializes a new instance of the ColumnMappingItem class.
        /// </summary>
        /// <param name="columnName">The column name.</param>
        /// <param name="propertyName">The property name.</param>
        public ColumnMappingItem(string columnName, string propertyName)
        {
            ColumnName = columnName;
            PropertyName = propertyName;
        }

        /// <summary>
        /// Gets or sets the column name of the table.
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// Gets or sets the propery name of the class.
        /// </summary>
        public string PropertyName { get; set; }
    }

    /// <summary>
    /// The column mapping attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class ColumnMappingAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the ColumnMappingAttribute class.
        /// </summary>
        /// <param name="name">The column name.</param>
        public ColumnMappingAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Gets or sets the column name of the table.
        /// </summary>
        public string Name { get; set; }
    }
}
