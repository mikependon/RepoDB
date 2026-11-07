#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema.Models
{
    /// <summary>
    /// A class that tells a kind of default value (see <see cref="ColumnDefaultValue"/>) by the pattern of its expression in a dialect.
    /// </summary>
    public sealed class SchemaDefaultPattern
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="SchemaDefaultPattern"/> class.
        /// </summary>
        /// <param name="kind">The kind of default value that the pattern tells.</param>
        /// <param name="pattern">The regular expression that matches the whole expression, without case.</param>
        public SchemaDefaultPattern(ColumnDefaultValue kind,
            string pattern)
        {
            Kind = kind;
            Pattern = pattern;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the kind of default value that the pattern tells.
        /// </summary>
        public ColumnDefaultValue Kind { get; }

        /// <summary>
        /// Gets the regular expression that matches the whole expression, without case.
        /// </summary>
        public string Pattern { get; }

        #endregion
    }
}
