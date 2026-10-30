#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.Schema
{
    /// <summary>
    /// Holds the format that identifies a saved schema definition. The writer saves it, and the validation of a definition expects it.
    /// </summary>
    internal static class SchemaDefinitionFormat
    {
        #region Constants

        /// <summary>
        /// The format of a schema definition: the value of the <c>format</c> property of a definition file, and of <c>SchemaDefinitionInfo.Format</c>.
        /// </summary>
        public const string Name = "RepoDb.Schema.Definition";

        #endregion
    }
}
