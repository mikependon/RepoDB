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
    /// Warns about the names that are used by more than one table of the same schema: tables whose names differ only by case, and constraint names that more than one table uses.
    /// Some databases (i.e.: SQL Server and PostgreSQL) require the name of a constraint to be unique in the whole schema, while others do not, so these are warnings.
    /// The names of the indexes are not checked, as the load of a definition renames the indexes whose name is already used when the destination requires it.
    /// </summary>
    internal static class SchemaDefinitionCrossTableRules
    {
        #region Internal Methods

        /// <summary>
        /// Checks the tables of the definition against each other.
        /// </summary>
        /// <param name="tables">The tables of the definition.</param>
        /// <param name="findings">The findings that receive the concerns.</param>
        internal static void Check(IEnumerable<TableSchema> tables,
            DefinitionFindings findings)
        {
            foreach (var schema in tables.GroupBy(table => table.Table.Schema ?? string.Empty, StringComparer.OrdinalIgnoreCase))
            {
                ReportTableNamesThatDifferOnlyByCase(schema.ToList(),
                    findings);
                ReportConstraintNamesUsedByMoreThanOneTable(schema.ToList(),
                    findings);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the distinct names of the constraints of the table that have a name, ignoring the case.
        /// </summary>
        /// <param name="table">The table that has the constraints.</param>
        /// <returns>The distinct names of the constraints that have a name.</returns>
        private static IEnumerable<string> GetNamedConstraints(TableSchema table) =>
            SchemaDefinitionNames.GetConstraintNames(table)
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

        /// <summary>
        /// Reports the tables of a schema whose names are the same when the case is ignored, but are not the same when it is not.
        /// </summary>
        /// <param name="tables">The tables of one schema.</param>
        /// <param name="findings">The findings that receive the concerns.</param>
        private static void ReportTableNamesThatDifferOnlyByCase(IList<TableSchema> tables,
            DefinitionFindings findings)
        {
            var groups = tables
                .GroupBy(table => table.Table.Name, StringComparer.OrdinalIgnoreCase)
                .Select(group => group.Select(table => DefinitionFindings.Describe(table.Table)).Distinct(StringComparer.Ordinal).ToList())
                .Where(variants => variants.Count > 1);
            foreach (var variants in groups)
            {
                findings.AddWarning($"The tables ({string.Join(", ", variants)}) differ only by case, which some databases treat as the same name.");
            }
        }

        /// <summary>
        /// Reports the constraint names that are used by more than one table of a schema.
        /// </summary>
        /// <param name="tables">The tables of one schema.</param>
        /// <param name="findings">The findings that receive the concerns.</param>
        private static void ReportConstraintNamesUsedByMoreThanOneTable(IList<TableSchema> tables,
            DefinitionFindings findings)
        {
            var owners = tables
                .SelectMany(table => GetNamedConstraints(table).Select(name => new { Name = name, Table = table }))
                .GroupBy(item => item.Name, StringComparer.OrdinalIgnoreCase)
                .Where(group => group.Select(item => item.Table).Distinct().Count() > 1);
            foreach (var group in owners)
            {
                var names = group.Select(item => DefinitionFindings.Describe(item.Table.Table));
                findings.AddWarning($"The constraint name '{group.Key}' is used by more than one table ({string.Join(", ", names)}), but some databases require it to be unique in the whole schema.");
            }
        }

        #endregion
    }
}
