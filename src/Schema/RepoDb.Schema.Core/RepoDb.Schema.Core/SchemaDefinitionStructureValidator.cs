#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Collections.Generic;
using RepoDb.Schema.Models;

namespace RepoDb.Schema
{
    /// <summary>
    /// Applies the structural checks to a definition, and collects all the problems that they find instead of stopping at the first one.
    /// A definition that fails them cannot be trusted by the strict checks, which only run on a definition with a sound structure.
    /// </summary>
    internal static class SchemaDefinitionStructureValidator
    {
        #region Internal Methods

        /// <summary>
        /// Checks the format of the definition, the identities of the tables, the structure of every table, and the external flags of the foreign keys.
        /// </summary>
        /// <param name="definition">The definition to be checked.</param>
        /// <returns>The errors that were found.</returns>
        internal static DefinitionFindings Validate(SchemaDefinitionInfo definition)
        {
            var findings = new DefinitionFindings();
            var tables = IndexTables(definition);
            var external = new HashSet<TableInfo>(definition.ExternalDependencies ?? new List<TableInfo>());
            SchemaDefinitionFormatRules.Check(definition,
                findings);
            SchemaDefinitionIdentityRules.Check(definition,
                tables,
                findings);
            foreach (var table in definition.Tables)
            {
                SchemaDefinitionTableRules.Check(table,
                    tables,
                    external,
                    findings);
            }
            SchemaDefinitionExternalFlagRules.Check(definition,
                findings);
            return findings;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Indexes the tables of the definition by their identity. If an identity is repeated, the first table that has it is kept.
        /// </summary>
        /// <param name="definition">The definition that has the tables.</param>
        /// <returns>The tables, by their identity.</returns>
        private static IDictionary<TableInfo, TableSchema> IndexTables(SchemaDefinitionInfo definition)
        {
            var tables = new Dictionary<TableInfo, TableSchema>();
            foreach (var table in definition.Tables)
            {
                if (!tables.ContainsKey(table.Table))
                {
                    tables.Add(table.Table,
                        table);
                }
            }
            return tables;
        }

        #endregion
    }
}
