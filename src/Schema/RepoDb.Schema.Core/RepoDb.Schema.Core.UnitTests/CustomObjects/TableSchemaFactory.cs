#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.Core.UnitTests.CustomObjects
{
    /// <summary>
    /// Creates the tables and the keys that the tests of the definition rules work with.
    /// </summary>
    internal static class TableSchemaFactory
    {
        #region Internal Methods

        /// <summary>
        /// Adds a foreign key to the table.
        /// </summary>
        /// <param name="table">The table that gets the foreign key.</param>
        /// <param name="name">The name of the foreign key.</param>
        /// <param name="parent">The referenced table.</param>
        /// <param name="columns">The columns of the table that make the foreign key.</param>
        /// <param name="referencedColumns">The columns of the referenced table.</param>
        /// <returns>The foreign key that was added.</returns>
        internal static ForeignKeyInfo AddForeignKey(TableSchema table,
            string name,
            TableInfo parent,
            string[] columns,
            string[] referencedColumns)
        {
            var foreignKey = new ForeignKeyInfo(name)
            {
                Columns = columns,
                ReferencedTable = parent,
                ReferencedColumns = referencedColumns
            };
            table.ForeignKeys.Add(foreignKey);
            return foreignKey;
        }

        /// <summary>
        /// Creates a column, whose type is an integer by default.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <param name="type">The type of the column, or <c>null</c> if it is not known.</param>
        /// <param name="ordinal">The position of the column in the table.</param>
        /// <param name="isNullable">Whether the column is nullable.</param>
        /// <param name="isIdentity">Whether the column is an identity column.</param>
        /// <returns>The column.</returns>
        internal static ColumnInfo CreateColumn(string name,
            Type type,
            int ordinal,
            bool isNullable = false,
            bool isIdentity = false) =>
            new ColumnInfo
            {
                Ordinal = ordinal,
                Field = new DbField(name, false, isIdentity, isNullable, type, null, null, null, null)
            };

        /// <summary>
        /// Creates a table that has the given columns, all of them integers that are not nullable.
        /// </summary>
        /// <param name="name">The name of the table.</param>
        /// <param name="columnNames">The names of the columns.</param>
        /// <returns>The table.</returns>
        internal static TableSchema CreateTable(string name,
            params string[] columnNames)
        {
            var table = new TableSchema(new TableInfo(name, "dbo", "TestDatabase", "TestProvider"));
            for (var index = 0; index < columnNames.Length; index++)
            {
                table.Columns.Add(CreateColumn(columnNames[index], typeof(int), index + 1));
            }
            return table;
        }

        /// <summary>
        /// Sets the primary key of the table.
        /// </summary>
        /// <param name="table">The table that gets the primary key.</param>
        /// <param name="name">The name of the primary key.</param>
        /// <param name="columns">The columns of the primary key.</param>
        internal static void SetPrimaryKey(TableSchema table,
            string name,
            params string[] columns)
        {
            table.PrimaryKey = new PrimaryKeyInfo(name) { Columns = columns };
        }

        #endregion
    }
}
