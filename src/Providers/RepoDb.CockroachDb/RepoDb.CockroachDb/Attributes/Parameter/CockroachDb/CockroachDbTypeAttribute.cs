#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Connector.CockroachDb;

namespace RepoDb.Attributes.Parameter.CockroachDb
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="CockroachDbParameter.CockroachDbType"/>
    /// property via an entity property before the actual execution.
    /// </summary>
    public class CockroachDbTypeAttribute : PropertyValueAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="CockroachDbTypeAttribute"/> class.
        /// </summary>
        /// <param name="cockroachDbType">The target <see cref="CockroachDbType"/> value.</param>
        public CockroachDbTypeAttribute(CockroachDbType cockroachDbType)
            : base(typeof(CockroachDbParameter), nameof(CockroachDbParameter.CockroachDbType), cockroachDbType)
        { }

        /// <summary>
        /// Gets the mapped <see cref="CockroachDbType"/> value of the parameter.
        /// </summary>
        public CockroachDbType CockroachDbType => (CockroachDbType)Value;
    }
}
