#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Collections.Generic;
using System.Linq;
using RepoDb.Schema.Models;

namespace RepoDb.Schema
{
    /// <summary>
    /// Gets the names of the objects of a table, for the rules that check them.
    /// </summary>
    internal static class SchemaDefinitionNames
    {
        #region Internal Methods

        /// <summary>
        /// Gets the names of the constraints of the table, which share the same namespace: the primary key, the unique, check and foreign key constraints.
        /// </summary>
        /// <param name="table">The table that has the constraints.</param>
        /// <returns>The names of the constraints, in that order. A constraint without a name has a <c>null</c> or empty name.</returns>
        internal static IList<string> GetConstraintNames(TableSchema table)
        {
            var names = new List<string>();
            if (table.PrimaryKey != null)
            {
                names.Add(table.PrimaryKey.Name);
            }
            names.AddRange(table.UniqueConstraints.Select(constraint => constraint.Name));
            names.AddRange(table.CheckConstraints.Select(constraint => constraint.Name));
            names.AddRange(table.ForeignKeys.Select(constraint => constraint.Name));
            return names;
        }

        #endregion
    }
}
