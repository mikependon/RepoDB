#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Connector.CockroachDb;
using System.Data;

namespace RepoDb.Attributes.Parameter.CockroachDb
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="CockroachDbParameter.Direction"/>
    /// property via an entity property before the actual execution.
    /// </summary>
    public class DirectionAttribute : PropertyValueAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="DirectionAttribute"/> class.
        /// </summary>
        /// <param name="direction">The value that indicates the direction of the parameter.</param>
        public DirectionAttribute(ParameterDirection direction)
            : base(typeof(CockroachDbParameter), nameof(CockroachDbParameter.Direction), direction)
        { }

        /// <summary>
        /// Gets the mapped value that indicates whether the parameter is input, output, bidirectional
        /// or a return value from the stored procedure.
        /// </summary>
        public ParameterDirection Direction => (ParameterDirection)Value;
    }
}
