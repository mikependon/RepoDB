#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Connector.CockroachDb;

namespace RepoDb.Attributes.Parameter.CockroachDb
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="CockroachDbParameter.Scale"/>
    /// property via an entity property before the actual execution.
    /// </summary>
    public class ScaleAttribute : PropertyValueAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="ScaleAttribute"/> class.
        /// </summary>
        /// <param name="scale">The scale of the parameter.</param>
        public ScaleAttribute(byte scale)
            : base(typeof(CockroachDbParameter), nameof(CockroachDbParameter.Scale), scale)
        { }

        /// <summary>
        /// Gets the mapped scale value of the parameter.
        /// </summary>
        public byte Scale => (byte)Value;
    }
}
