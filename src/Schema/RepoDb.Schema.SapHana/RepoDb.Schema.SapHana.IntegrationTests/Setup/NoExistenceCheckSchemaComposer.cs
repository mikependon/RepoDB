#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICESapHanaE file in the project root for full license information.

#endregion

using System.Collections.Generic;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.SapHana.IntegrationTests.Setup
{
    /// <summary>
    /// A composer that does the same as the composer that it wraps, but does not compose the statements that check whether a table, a column or an index exists.
    /// A copy that uses it does not know what exists in the destination database, so it always tries to create the tables (it is used to make a statement fail).
    /// </summary>
    public sealed class NoExistenceCheckSchemaComposer : ISchemaComposer
    {
        private readonly ISchemaComposer _composer;

        public NoExistenceCheckSchemaComposer(ISchemaComposer composer)
        {
            _composer = composer;
        }

        public IEnumerable<string> ComposeSchema(TableSchema schema) => _composer.ComposeSchema(schema);

        public IEnumerable<string> ComposeSchemas(IEnumerable<TableSchema> schemas) => _composer.ComposeSchemas(schemas);

        public string ComposeCreateTable(TableSchema schema) => _composer.ComposeCreateTable(schema);

        public string ComposeCreateIndex(string tableName, IndexInfo index) => _composer.ComposeCreateIndex(tableName, index);

        public string ComposeAddForeignKey(string tableName, ForeignKeyInfo foreignKey) => _composer.ComposeAddForeignKey(tableName, foreignKey);

        public string ComposeAddColumn(string tableName, ColumnInfo column) => _composer.ComposeAddColumn(tableName, column);

        public string ComposeDropTable(string tableName) => _composer.ComposeDropTable(tableName);

        public string ComposeName(TableInfo table) => _composer.ComposeName(table);

        public string ComposeTableExists(string tableName) => null;

        public string ComposeColumnExists(string tableName, string columnName) => null;

        public string ComposeIndexExists(string tableName, string indexName) => null;

        public string ComposeTypeName(ColumnInfo column) => _composer.ComposeTypeName(column);
    }
}
