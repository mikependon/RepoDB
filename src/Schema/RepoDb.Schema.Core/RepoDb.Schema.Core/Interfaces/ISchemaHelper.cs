#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;

namespace RepoDb.Schema
{
    /// <summary>
    /// An interface that is used to define the schema helper of the library.
    /// </summary>
    public interface ISchemaHelper
    {
        /// <summary>
        /// Extracts the schema of the table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>The extracted schema of the table.</returns>
        string Extract(string tableName);
    }
}
