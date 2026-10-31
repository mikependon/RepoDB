#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using RepoDb.Schema.Models;

namespace RepoDb.Schema
{
    /// <summary>
    /// Checks that the names of the constraints and of the indexes of a table do not collide.
    /// </summary>
    internal static class SchemaDefinitionNameRules
    {
        #region Internal Methods

        /// <summary>
        /// Checks the names of the constraints and of the indexes of the table.
        /// </summary>
        /// <param name="table">The table to be checked.</param>
        /// <param name="findings">The findings that receive the problems.</param>
        internal static void Check(TableSchema table,
            DefinitionFindings findings)
        {
            ReportDuplicates(table,
                SchemaDefinitionNames.GetConstraintNames(table),
                "constraint",
                findings);
            ReportDuplicates(table,
                GetIndexNames(table),
                "index",
                findings);
            ReportIndexesNamedAsKeys(table,
                findings);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the names of the indexes of the table.
        /// </summary>
        /// <param name="table">The table that has the indexes.</param>
        /// <returns>The names of the indexes.</returns>
        private static IEnumerable<string> GetIndexNames(TableSchema table) =>
            table.Indexes.Select(index => index.Name);

        /// <summary>
        /// Reports every name that is used more than once.
        /// </summary>
        /// <param name="table">The table that has the names.</param>
        /// <param name="names">The names to be checked.</param>
        /// <param name="kind">The kind of the objects that have the names (i.e.: constraint or index).</param>
        /// <param name="findings">The findings that receive the problems.</param>
        private static void ReportDuplicates(TableSchema table,
            IEnumerable<string> names,
            string kind,
            DefinitionFindings findings)
        {
            var duplicates = names
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .GroupBy(name => name, StringComparer.Ordinal)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key);
            foreach (var name in duplicates)
            {
                findings.AddError(table, $"The {kind} name '{name}' is used more than once. Give each {kind} a distinct name. Location: {DefinitionFindings.Location(table, kind == "index" ? "Indexes" : "Primary Key / Unique Constraints / Foreign Keys / Check Constraints", name, "Name")}.");
            }
        }

        /// <summary>
        /// Reports the indexes that are named like the primary key or like a unique constraint, as these own an index with their name.
        /// </summary>
        /// <param name="table">The table that has the indexes.</param>
        /// <param name="findings">The findings that receive the problems.</param>
        private static void ReportIndexesNamedAsKeys(TableSchema table,
            DefinitionFindings findings)
        {
            var keyNames = new HashSet<string>(StringComparer.Ordinal);
            if (table.PrimaryKey != null)
            {
                keyNames.Add(table.PrimaryKey.Name);
            }
            foreach (var constraint in table.UniqueConstraints)
            {
                keyNames.Add(constraint.Name);
            }
            foreach (var index in table.Indexes.Where(index => index.Name != null && keyNames.Contains(index.Name)))
            {
                findings.AddError(table, $"The index '{index.Name}' has the name of the primary key or of a unique constraint. Rename the index so it differs from the key names. Location: {DefinitionFindings.Location(table, "Indexes", index.Name, "Name")}.");
            }
        }

        #endregion
    }
}
