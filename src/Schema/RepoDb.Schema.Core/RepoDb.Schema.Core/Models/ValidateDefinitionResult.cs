#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Collections.Generic;

namespace RepoDb.Schema.Models
{
    /// <summary>
    /// Contains the outcome of validating a saved schema definition file.
    /// </summary>
    public sealed class ValidateDefinitionResult
    {
        #region Properties

        /// <summary>
        /// Gets the definition that was read from the file, or <c>null</c> if the file is not valid.
        /// </summary>
        public SchemaDefinitionInfo Definition { get; internal set; }

        /// <summary>
        /// Gets the problems that make the file invalid. It is empty if the file is valid.
        /// A file that cannot be read reports its first problem, starting with the most general message and followed by the messages of its underlying causes.
        /// A file that can be read reports all the problems that the strict checks find: names that are used twice, tables without columns,
        /// identity columns that are not numeric, empty check constraints, and foreign keys that do not reference a key of the referenced table.
        /// </summary>
        public IList<string> Errors { get; internal set; } = new List<string>();

        /// <summary>
        /// Gets a value indicating whether the file is a valid schema definition.
        /// </summary>
        public bool IsValid => Errors.Count == 0;

        /// <summary>
        /// Gets the concerns that were found in a file that can be read. They do not make the file invalid, as they are allowed by some databases or cannot be proven:
        /// nullable primary key columns, more than one identity column, foreign key columns whose type differs from the referenced column, foreign keys to a table
        /// that is not part of the file, quoted names in SQL expressions (check constraints, computed columns and index filters) that are not columns of the table,
        /// names that differ only by case (columns, constraints, indexes and tables), and constraint names that more than one table of the same schema uses.
        /// </summary>
        public IList<string> Warnings { get; internal set; } = new List<string>();

        #endregion
    }
}
