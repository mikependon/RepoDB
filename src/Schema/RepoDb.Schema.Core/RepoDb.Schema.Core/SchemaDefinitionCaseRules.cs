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
    /// Warns about the names of a table that differ only by case (i.e.: <c>Name</c> and <c>name</c>). Some databases (i.e.: SQL Server and MySQL)
    /// treat them as the same name, so the table could not be created there, while others (i.e.: PostgreSQL) keep them apart. It is only a warning for that reason.
    /// </summary>
    internal static class SchemaDefinitionCaseRules
    {
        #region Internal Methods

        /// <summary>
        /// Checks the names of the columns, the constraints and the indexes of the table.
        /// </summary>
        /// <param name="table">The table to be checked.</param>
        /// <param name="findings">The findings that receive the concerns.</param>
        internal static void Check(TableSchema table,
            DefinitionFindings findings)
        {
            ReportNamesThatDifferOnlyByCase(table,
                table.Columns.Select(column => column.Field.Name),
                "column",
                findings);
            ReportNamesThatDifferOnlyByCase(table,
                SchemaDefinitionNames.GetConstraintNames(table),
                "constraint",
                findings);
            ReportNamesThatDifferOnlyByCase(table,
                table.Indexes.Select(index => index.Name),
                "index",
                findings);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Reports the groups of names that are the same when the case is ignored, but are not the same when it is not. Names that are exactly the same are errors that are reported elsewhere.
        /// </summary>
        /// <param name="table">The table that has the names.</param>
        /// <param name="names">The names to be checked.</param>
        /// <param name="kind">The kind of the objects that have the names (i.e.: column, constraint or index).</param>
        /// <param name="findings">The findings that receive the concerns.</param>
        private static void ReportNamesThatDifferOnlyByCase(TableSchema table,
            IEnumerable<string> names,
            string kind,
            DefinitionFindings findings)
        {
            var groups = names
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .GroupBy(name => name, StringComparer.OrdinalIgnoreCase)
                .Select(group => group.Distinct(StringComparer.Ordinal).ToList())
                .Where(variants => variants.Count > 1);
            foreach (var variants in groups)
            {
                findings.AddWarning(table, $"The {kind} names ({string.Join(", ", variants)}) differ only by case, which some databases treat as the same name.");
            }
        }

        #endregion
    }
}
