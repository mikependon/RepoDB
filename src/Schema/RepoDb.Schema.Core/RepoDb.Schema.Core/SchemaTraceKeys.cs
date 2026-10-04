#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.Schema
{
    /// <summary>
    /// A class that holds the constant values of the schema operation tracking keys.
    /// </summary>
    public static class SchemaTraceKeys
    {
        /// <summary>
        /// The trace key for the 'CopySchemaTo' operation.
        /// </summary>
        public const string CopySchemaTo = "CopySchemaTo";

        /// <summary>
        /// The trace key for the 'CopySchemasTo' operation.
        /// </summary>
        public const string CopySchemasTo = "CopySchemasTo";

        /// <summary>
        /// The trace key used when the check constraints of a table are being read.
        /// </summary>
        public const string GetCheckConstraints = "GetCheckConstraints";

        /// <summary>
        /// The trace key used when the columns of a table are being read.
        /// </summary>
        public const string GetColumns = "GetColumns";

        /// <summary>
        /// The trace key used when the foreign keys of a table are being read.
        /// </summary>
        public const string GetForeignKeys = "GetForeignKeys";

        /// <summary>
        /// The trace key used when the indexes of a table are being read.
        /// </summary>
        public const string GetIndexes = "GetIndexes";

        /// <summary>
        /// The trace key used when the primary key of a table is being read.
        /// </summary>
        public const string GetPrimaryKey = "GetPrimaryKey";

        /// <summary>
        /// The trace key used when the tables of a database are being read.
        /// </summary>
        public const string GetTables = "GetTables";

        /// <summary>
        /// The trace key used when the unique constraints of a table are being read.
        /// </summary>
        public const string GetUniqueConstraints = "GetUniqueConstraints";

        /// <summary>
        /// The trace key used when the schema name of a table is being resolved.
        /// </summary>
        public const string ResolveSchemaName = "ResolveSchemaName";

        /// <summary>
        /// The trace key used when the existence of a table is being checked.
        /// </summary>
        public const string TableExists = "TableExists";
    }
}
