#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using MySqlConnector;

namespace RepoDb.Attributes.Parameter.MySqlConnector
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="MySqlParameter.SourceColumnNullMapping"/>
    /// property via an entity property before the actual execution.
    /// </summary>
    public class SourceColumnNullMappingAttribute : PropertyValueAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="SourceColumnNullMappingAttribute"/> class.
        /// </summary>
        /// <param name="sourceColumnNullMapping">The value that indicates whether the source column is nullable.</param>
        public SourceColumnNullMappingAttribute(bool sourceColumnNullMapping)
            : base(typeof(MySqlParameter), nameof(MySqlParameter.SourceColumnNullMapping), sourceColumnNullMapping)
        { }

        /// <summary>
        /// Gets the mapped value that indicates whether the source column is nullable.
        /// </summary>
        public bool SourceColumnNullMapping => (bool)Value;
    }
}
