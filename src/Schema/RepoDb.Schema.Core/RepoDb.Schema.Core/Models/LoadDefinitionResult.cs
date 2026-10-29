#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.Schema.Models
{
    /// <summary>
    /// Contains the complete file definition and the outcomes of applying it to the destination database.
    /// </summary>
    public sealed class LoadDefinitionResult
    {
        #region Properties

        /// <summary>
        /// Gets the creation, skip, or alignment outcomes, executed scripts, and warnings for the destination tables.
        /// </summary>
        public CopyTableSchemaResult CopyResult { get; internal set; }

        /// <summary>
        /// Gets the definition read from the file, including tables and extraction metadata.
        /// This describes the file; destination adjustments and unsupported features are reported in the copy result.
        /// </summary>
        public SchemaDefinitionResult Definition { get; internal set; }

        #endregion
    }
}
