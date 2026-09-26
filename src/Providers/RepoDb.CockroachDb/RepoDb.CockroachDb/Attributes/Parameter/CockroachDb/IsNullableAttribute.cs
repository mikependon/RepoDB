#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Connector.CockroachDb;

namespace RepoDb.Attributes.Parameter.CockroachDb
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="CockroachDbParameter.IsNullable"/>
    /// property via an entity property before the actual execution.
    /// </summary>
    public class IsNullableAttribute : PropertyValueAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="IsNullableAttribute"/> class.
        /// </summary>
        /// <param name="isNullable">The value that defines whether the parameter accepts a null value.</param>
        public IsNullableAttribute(bool isNullable)
            : base(typeof(CockroachDbParameter), nameof(CockroachDbParameter.IsNullable), isNullable)
        { }

        /// <summary>
        /// Gets the mapped value that defines whether the parameter accepts a null value.
        /// </summary>
        public bool IsNullable => (bool)Value;
    }
}
