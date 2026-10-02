#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.Schema
{
    /// <summary>
    /// An interface that is used to define the extractor of the schema of a table.
    /// </summary>
    public interface ISchemaExtractor
    {
        /// <summary>
        /// Gets the fields of the table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>An array of <see cref="DbField"/> objects that represents the fields of the table.</returns>
        DbField[] GetFields(string tableName);

        /// <summary>
        /// Extracts the schema of the table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>The extracted schema of the table.</returns>
        string Extract(string tableName);
    }
}
