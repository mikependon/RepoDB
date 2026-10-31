#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using RepoDb.Schema.Models;

namespace RepoDb.Schema
{
    /// <summary>
    /// Checks that the external flag that a version 1 definition file saves for each foreign key agrees with the tables that the definition includes.
    /// The check needs the document that the definition was read from, so it is skipped for a definition that has none, and for the later versions that do not save the flag.
    /// </summary>
    internal static class SchemaDefinitionExternalFlagRules
    {
        #region Internal Methods

        /// <summary>
        /// Checks the external flags of the foreign keys of the definition.
        /// </summary>
        /// <param name="definition">The definition to be checked.</param>
        /// <param name="findings">The findings that receive the problems.</param>
        internal static void Check(SchemaDefinitionInfo definition,
            DefinitionFindings findings)
        {
            if (definition.FormatVersion != 1 || !(definition.Definition?["tables"] is JsonArray tableNodes) || tableNodes.Count != definition.Tables.Count)
            {
                return;
            }
            var included = new HashSet<TableInfo>(definition.Tables.Select(table => table.Table));
            for (var index = 0; index < definition.Tables.Count; index++)
            {
                CheckTable(definition.Tables[index],
                    tableNodes[index],
                    included,
                    findings);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Checks the external flags of the foreign keys of a table. The foreign keys of the table are in the same order as the ones of its node.
        /// </summary>
        /// <param name="table">The table that has the foreign keys.</param>
        /// <param name="tableNode">The node of the table in the document.</param>
        /// <param name="included">The identities of the tables of the definition.</param>
        /// <param name="findings">The findings that receive the problems.</param>
        private static void CheckTable(TableSchema table,
            JsonNode tableNode,
            ISet<TableInfo> included,
            DefinitionFindings findings)
        {
            if (!(tableNode?["foreignKeys"] is JsonArray keyNodes) || keyNodes.Count != table.ForeignKeys.Count)
            {
                return;
            }
            for (var index = 0; index < table.ForeignKeys.Count; index++)
            {
                var foreignKey = table.ForeignKeys[index];
                var shouldBeExternal = foreignKey.ReferencedTable == null || !included.Contains(foreignKey.ReferencedTable);
                if (!TryGetFlag(keyNodes[index], out var flag) || flag != shouldBeExternal)
                {
                    findings.AddError(table, $"A foreign-key external flag disagrees with the included tables: the foreign key {DefinitionFindings.ObjectName(foreignKey.Name, index)} should be {(shouldBeExternal ? "external" : "internal")}. Set the reference external flag to {(shouldBeExternal ? "true" : "false")} to match whether the referenced table is included in Tables. Location: {DefinitionFindings.Location(table, "Foreign Keys", DefinitionFindings.ObjectName(foreignKey.Name, index), "Reference > External")}.");
                }
            }
        }

        /// <summary>
        /// Gets the external flag of the reference of a foreign key node.
        /// </summary>
        /// <param name="keyNode">The node of the foreign key in the document.</param>
        /// <param name="flag">The flag, if the node has one.</param>
        /// <returns><c>true</c> if the node has a flag that is a boolean; otherwise, <c>false</c>.</returns>
        private static bool TryGetFlag(JsonNode keyNode,
            out bool flag)
        {
            flag = false;
            return keyNode?["references"]?["external"] is JsonValue value && value.TryGetValue(out flag);
        }

        #endregion
    }
}
