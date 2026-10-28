#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Collections.Generic;
using System.Text.Json.Nodes;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema.Models
{
    /// <summary>
    /// Contains a saved definition, including all selected tables and its extraction metadata.
    /// </summary>
    public sealed class SchemaDefinitionResult
    {
        #region Properties

        /// <summary>
        /// Gets the definition format version.
        /// </summary>
        public int FormatVersion { get; internal set; }

        /// <summary>
        /// Gets the source provider name, independent of the connection used to read the file.
        /// </summary>
        public string Provider { get; internal set; }

        /// <summary>
        /// Gets the source database name.
        /// </summary>
        public string Database { get; internal set; }

        /// <summary>
        /// Gets the identity of the table requested when the definition was saved.
        /// </summary>
        public TableInfo RootTable { get; internal set; }

        /// <summary>
        /// Gets the relationship behavior used when the definition was saved.
        /// </summary>
        public RelatedTableBehavior RelationshipBehavior { get; internal set; }

        /// <summary>
        /// Gets every table definition included in the file, in file order.
        /// </summary>
        public IList<TableSchema> Tables { get; internal set; }

        /// <summary>
        /// Gets the identities of referenced tables whose definitions are outside the file.
        /// </summary>
        public IList<TableInfo> ExternalDependencies { get; internal set; }

        /// <summary>
        /// Gets the complete parsed document, including portable metadata without an equivalent schema model property.
        /// CLR types are reconstructed from portable logical types; unknown types have a null CLR type.
        /// </summary>
        public JsonObject Definition { get; internal set; }

        #endregion
    }
}
