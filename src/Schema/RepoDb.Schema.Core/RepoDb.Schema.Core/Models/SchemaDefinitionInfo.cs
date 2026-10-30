#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Collections.Generic;
using System.Text.Json.Nodes;
using RepoDb.Schema.Attributes;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema.Models
{
    /// <summary>
    /// Contains a saved definition, including all selected tables and its extraction metadata.
    /// </summary>
    public sealed class SchemaDefinitionInfo
    {
        #region Properties

        /// <summary>
        /// Gets or sets the source database name.
        /// </summary>
        public string Database { get; set; }

        /// <summary>
        /// Gets or sets the complete parsed document, including portable metadata without an equivalent schema model property.
        /// CLR types are reconstructed from portable logical types; unknown types have a null CLR type.
        /// </summary>
        [DefinitionIgnore]
        public JsonObject Definition { get; set; }

        /// <summary>
        /// Gets or sets the identities of referenced tables whose definitions are outside the file.
        /// </summary>
        [DefinitionIgnore]
        public IList<TableInfo> ExternalDependencies { get; set; }

        /// <summary>
        /// Gets or sets the format of the definition: the value of the <c>format</c> property of the file, which identifies it as a schema definition.
        /// A valid definition has the format <c>RepoDb.Schema.Definition</c>. It is checked by the validation of the definition, not by the read.
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// Gets or sets the definition format version.
        /// </summary>
        public int FormatVersion { get; set; }

        /// <summary>
        /// Gets or sets the source provider name, independent of the connection used to read the file.
        /// </summary>
        public string Provider { get; set; }

        /// <summary>
        /// Gets or sets the relationship behavior used when the definition was saved.
        /// </summary>
        public RelatedTableBehavior RelationshipBehavior { get; set; }

        /// <summary>
        /// Gets or sets the identity of the table requested when the definition was saved.
        /// </summary>
        public TableInfo RootTable { get; set; }

        /// <summary>
        /// Gets or sets every table definition included in the file, in file order.
        /// </summary>
        public IList<TableSchema> Tables { get; set; }

        #endregion
    }
}
