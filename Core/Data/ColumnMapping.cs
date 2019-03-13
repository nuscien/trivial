using System;
using System.Collections.Generic;
using System.Text;

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
            Add(new ColumnMappingItem(columnName, propertyName));
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
                foreach (var attr in prop.GetCustomAttributes(typeof(ColumnAttribute), true))
                {
                    var info = attr as ColumnAttribute;
                    if (info == null) continue;
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

    public class ColumnAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the ColumnAttribute class.
        /// </summary>
        /// <param name="name">The column name.</param>
        public ColumnAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Gets or sets the column name of the table.
        /// </summary>
        public string Name { get; set; }
    }
}
