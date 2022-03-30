using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Trivial.Reflection;
using Trivial.Text;

namespace Trivial.Data;

/// <summary>
/// String table data reader.
/// </summary>
public class StringTableDataReader : BaseListDataReader<IReadOnlyList<string>>
{
    private readonly string defaultColumnTypeName;
    private readonly Func<DbColumnInfo, DbColumnInfo> columnResolver;
    private IReadOnlyList<DbColumnInfo> columnsInfo;
    private bool isInit = false;

    /// <summary>
    /// Initializes a new instance of the ReadOnlyStringTableDataReader class.
    /// </summary>
    /// <param name="lines">Lines to read.</param>
    /// <param name="columnNames">The names of column.</param>
    /// <param name="hasHeader">true if the first is the table header, and the column names will be replaced by the header; otherwise, false.</param>
    public StringTableDataReader(IEnumerable<IReadOnlyList<string>> lines, IEnumerable<string> columnNames, bool hasHeader = false) : this(lines, columnNames, null, hasHeader)
    {
        if (columnNames == null) return;
        columnsInfo = columnNames.Select(ele => new DbColumnInfo(ele, typeof(string))).ToList().AsReadOnly();
    }

    /// <summary>
    /// Initializes a new instance of the ReadOnlyStringTableDataReader class.
    /// </summary>
    /// <param name="lines">Lines to read.</param>
    /// <param name="columnNames">The names of column.</param>
    /// <param name="columnTypeName">The type name of each column.</param>
    /// <param name="hasHeader">true if the first is the table header, and the column names will be replaced by the header; otherwise, false.</param>
    public StringTableDataReader(IEnumerable<IReadOnlyList<string>> lines, IEnumerable<string> columnNames, string columnTypeName, bool hasHeader = false) : base(lines, hasHeader ? 1 : 0)
    {
        defaultColumnTypeName = columnTypeName;
        if (columnNames == null) return;
        columnsInfo = columnNames.Select(ele => new DbColumnInfo(ele, columnTypeName)).ToList().AsReadOnly();
    }

    /// <summary>
    /// Initializes a new instance of the ReadOnlyStringTableDataReader class.
    /// </summary>
    /// <param name="lines">Lines to read.</param>
    /// <param name="columnInfoResolver">The column info resolver from the first line.</param>
    /// <param name="columnNames">The names of column.</param>
    /// <param name="columnTypeName">The type name of each column.</param>
    public StringTableDataReader(IEnumerable<IReadOnlyList<string>> lines, Func<DbColumnInfo, DbColumnInfo> columnInfoResolver, IEnumerable<string> columnNames = null, string columnTypeName = null) : this(lines, columnNames, columnTypeName, true)
    {
        columnResolver = columnInfoResolver;
        defaultColumnTypeName = columnTypeName;
        if (columnNames == null) return;
        columnsInfo = columnNames.Select(ele => new DbColumnInfo(ele, typeof(string))).ToList().AsReadOnly();
    }

    /// <summary>
    /// Initializes a new instance of the ReadOnlyStringTableDataReader class.
    /// </summary>
    /// <param name="lines">Lines to read.</param>
    /// <param name="columns">The columns information</param>
    /// <param name="hasHeader">true if the first is the table header, and the column names will be replaced by the header; otherwise, false.</param>
    public StringTableDataReader(IEnumerable<IReadOnlyList<string>> lines, IEnumerable<DbColumnInfo> columns, bool hasHeader = false) : base(lines, hasHeader ? 1 : 0)
    {
        if (columns == null) return;
        columnsInfo = columns.ToList().AsReadOnly();
    }

    /// <summary>
    /// Initializes a new instance of the ReadOnlyStringTableDataReader class.
    /// </summary>
    /// <param name="lines">Lines to read.</param>
    /// <param name="columnInfoResolver">The column info resolver from the first line.</param>
    /// <param name="columns">The columns information</param>
    /// <param name="columnTypeName">The type name of each column.</param>
    public StringTableDataReader(IEnumerable<IReadOnlyList<string>> lines, Func<DbColumnInfo, DbColumnInfo> columnInfoResolver, IEnumerable<DbColumnInfo> columns = null, string columnTypeName = null) : this(lines, columns, true)
    {
        columnResolver = columnInfoResolver;
        defaultColumnTypeName = columnTypeName;
        if (columns == null) return;
        columnsInfo = columns.ToList().AsReadOnly();
    }

    /// <summary>
    /// Gets the current record instance.
    /// </summary>
    /// <returns>The current record instance.</returns>
    public new IReadOnlyList<string> CurrentRecord => base.CurrentRecord;

    /// <summary>
    /// Gets the information of columns.
    /// </summary>
    public override IReadOnlyList<DbColumnInfo> ColumnsInfo => columnsInfo;

    /// <summary>
    /// Gets the value of the specified column as an instance of System.Object.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>The value of the specified column.</returns>
    public override object this[int ordinal] => CurrentRecord[ordinal];

    /// <summary>
    /// Gets the value of the specified column as a string.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>The value of the specified column.</returns>
    public override string GetString(int ordinal)
    {
        return CurrentRecord[ordinal];
    }

    /// <summary>
    /// Returns the typed collection.
    /// </summary>
    /// <typeparam name="T">The type of each instance in the collection.</typeparam>
    /// <param name="creator">The instance factory.</param>
    /// <param name="propertyNames">The property names to map.</param>
    /// <returns>A typed collection based on the data parsed.</returns>
    public T ConvertTo<T>(Func<IReadOnlyList<string>, T> creator, IEnumerable<string> propertyNames)
    {
        return ObjectConvert.Invoke<T>(CurrentRecord, creator, propertyNames);
    }

    /// <summary>
    /// Returns the typed collection.
    /// </summary>
    /// <typeparam name="T">The type of each instance in the collection.</typeparam>
    /// <param name="propertyNames">The property names to map.</param>
    /// <returns>A typed collection based on the data parsed.</returns>
    public T ConvertTo<T>(IEnumerable<string> propertyNames)
    {
        return ConvertTo<T>(null, propertyNames);
    }

    /// <summary>
    /// Processes on reading record.
    /// </summary>
    /// <param name="record">The record to read.</param>
    protected override void OnReadRecord(IReadOnlyList<string> record)
    {
        base.OnReadRecord(record);
        if (isInit) return;
        isInit = true;
        if (Skip < 1) return;
        var columns = ColumnsInfo != null ? new List<DbColumnInfo>() : new List<DbColumnInfo>(ColumnsInfo);
        for (var i = 0; i < record.Count; i++)
        {
            if (i <= columns.Count) columns.Add(new DbColumnInfo(record[i], defaultColumnTypeName ?? typeof(string).Name));
            else if (columns[i] == null) columns[i] = new DbColumnInfo(record[i], defaultColumnTypeName ?? typeof(string).Name);
            else columns[i] = new DbColumnInfo(record[i], columns[i].TypeName ?? defaultColumnTypeName ?? typeof(string).Name);
            if (columnResolver == null) continue;
            var col = columnResolver(columns[i]);
            if (col != null) columns[i] = col;
        }

        columnsInfo = columns.AsReadOnly();
    }
}
