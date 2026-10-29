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
    /// Applies the strict checks to a definition that was read successfully, and collects all the problems that they find instead of stopping at the first one.
    /// </summary>
    internal static class SchemaDefinitionStrictValidator
    {
        #region Internal Methods

        /// <summary>
        /// Checks every table of the definition.
        /// </summary>
        /// <param name="definition">The definition that was read from the file.</param>
        /// <returns>The errors and the warnings that were found.</returns>
        internal static DefinitionFindings Validate(SchemaDefinitionResult definition)
        {
            var findings = new DefinitionFindings();
            var tables = definition.Tables.ToDictionary(table => table.Table);
            foreach (var table in definition.Tables)
            {
                CheckTable(table,
                    tables,
                    findings);
            }
            SchemaDefinitionCrossTableRules.Check(definition.Tables,
                findings);
            return findings;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Applies all the rules to a table.
        /// </summary>
        /// <param name="table">The table to be checked.</param>
        /// <param name="tables">The tables of the definition, by their identity.</param>
        /// <param name="findings">The findings that receive the problems.</param>
        private static void CheckTable(TableSchema table,
            IDictionary<TableInfo, TableSchema> tables,
            DefinitionFindings findings)
        {
            SchemaDefinitionNameRules.Check(table,
                findings);
            SchemaDefinitionCaseRules.Check(table,
                findings);
            SchemaDefinitionColumnRules.Check(table,
                findings);
            SchemaDefinitionExpressionRules.Check(table,
                findings);
            SchemaDefinitionForeignKeyRules.Check(table,
                tables,
                findings);
        }

        #endregion
    }
}
