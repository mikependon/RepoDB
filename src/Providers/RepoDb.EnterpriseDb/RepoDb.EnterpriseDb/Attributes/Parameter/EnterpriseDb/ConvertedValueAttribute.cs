#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using RepoDb.Connector.EnterpriseDb;

namespace RepoDb.Attributes.Parameter.EnterpriseDb
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="EDBParameter"/> for ConvertedValue
    /// property via an entity property before the actual execution.
    /// </summary>
    [Obsolete("No longer supported by RepoDb.Connector.EnterpriseDb.")]
    public class ConvertedValueAttribute : PropertyValueAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="ConvertedValueAttribute"/> class.
        /// </summary>
        /// <param name="convertedValue">The converted value.</param>
        public ConvertedValueAttribute(object convertedValue)
            : base(typeof(EDBParameter),  "ConvertedValue", convertedValue)
        { }

        /// <summary>
        /// Gets the mapped converted value of the parameter.
        /// </summary>
        public object ConvertedValue => Value;
    }
}
