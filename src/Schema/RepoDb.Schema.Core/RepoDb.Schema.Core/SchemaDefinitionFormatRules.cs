#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using RepoDb.Schema.Models;

namespace RepoDb.Schema
{
    /// <summary>
    /// Checks that a definition has the format of a schema definition. The comparison is exact: the case is significant.
    /// </summary>
    internal static class SchemaDefinitionFormatRules
    {
        #region Internal Methods

        /// <summary>
        /// Checks the format of the definition.
        /// </summary>
        /// <param name="definition">The definition to be checked.</param>
        /// <param name="findings">The findings that receive the problems.</param>
        internal static void Check(SchemaDefinitionInfo definition,
            DefinitionFindings findings)
        {
            if (!string.Equals(definition.Format, SchemaDefinitionFormat.Name, StringComparison.Ordinal))
            {
                findings.AddError($"The file is not a RepoDB schema definition: its format must be '{SchemaDefinitionFormat.Name}', but it is {Describe(definition.Format)}. Use a RepoDB schema definition or correct its format marker. Location: Format.");
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Describes a format for a message.
        /// </summary>
        /// <param name="format">The format of the definition, or <c>null</c> if it has none.</param>
        /// <returns>The format between quotes, or a description of its absence.</returns>
        private static string Describe(string format) =>
            format == null ? "missing" : $"'{format}'";

        #endregion
    }
}
