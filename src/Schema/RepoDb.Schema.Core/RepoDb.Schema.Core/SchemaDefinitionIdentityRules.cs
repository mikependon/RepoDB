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
    /// Checks the identities of the tables of a definition: a table is defined once, the root table is defined, and the external dependencies are tables that the definition does not define.
    /// </summary>
    internal static class SchemaDefinitionIdentityRules
    {
        #region Internal Methods

        /// <summary>
        /// Checks the identities of the tables of the definition.
        /// </summary>
        /// <param name="definition">The definition to be checked.</param>
        /// <param name="tables">The tables of the definition, by their identity.</param>
        /// <param name="findings">The findings that receive the problems.</param>
        internal static void Check(SchemaDefinitionInfo definition,
            IDictionary<TableInfo, TableSchema> tables,
            DefinitionFindings findings)
        {
            ReportRepeatedTables(definition,
                findings);
            ReportMissingRootTable(definition,
                tables,
                findings);
            ReportInvalidExternalDependencies(definition,
                tables,
                findings);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Reports the tables that are defined more than once.
        /// </summary>
        /// <param name="definition">The definition to be checked.</param>
        /// <param name="findings">The findings that receive the problems.</param>
        private static void ReportRepeatedTables(SchemaDefinitionInfo definition,
            DefinitionFindings findings)
        {
            var repeated = definition.Tables
                .GroupBy(table => table.Table)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key);
            foreach (var identity in repeated)
            {
                findings.AddError($"Duplicate table identity: the table '{DefinitionFindings.Describe(identity)}' is defined more than once.");
            }
        }

        /// <summary>
        /// Reports a definition whose root table is not one of its tables.
        /// </summary>
        /// <param name="definition">The definition to be checked.</param>
        /// <param name="tables">The tables of the definition, by their identity.</param>
        /// <param name="findings">The findings that receive the problems.</param>
        private static void ReportMissingRootTable(SchemaDefinitionInfo definition,
            IDictionary<TableInfo, TableSchema> tables,
            DefinitionFindings findings)
        {
            if (definition.RootTable == null || !tables.ContainsKey(definition.RootTable))
            {
                findings.AddError("The root table definition is missing.");
            }
        }

        /// <summary>
        /// Reports external dependencies that are repeated, or that are also defined as tables of the definition.
        /// </summary>
        /// <param name="definition">The definition to be checked.</param>
        /// <param name="tables">The tables of the definition, by their identity.</param>
        /// <param name="findings">The findings that receive the problems.</param>
        private static void ReportInvalidExternalDependencies(SchemaDefinitionInfo definition,
            IDictionary<TableInfo, TableSchema> tables,
            DefinitionFindings findings)
        {
            var dependencies = definition.ExternalDependencies ?? new List<TableInfo>();
            var distinct = new HashSet<TableInfo>(dependencies);
            if (distinct.Count != dependencies.Count || distinct.Any(tables.ContainsKey))
            {
                findings.AddError("Invalid external dependency identities: they must be distinct, and must not be tables of the definition.");
            }
        }

        #endregion
    }
}
