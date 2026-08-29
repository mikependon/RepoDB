#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Connector.MariaDbConnector;

namespace RepoDb.Attributes.Parameter.MariaDb
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="MariaDbParameter.SourceColumn"/>
    /// property via an entity property before the actual execution.
    /// </summary>
    public class SourceColumnAttribute : PropertyValueAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="SourceColumnAttribute"/> class.
        /// </summary>
        /// <param name="sourceColumn">The name of the source column mapped to the <see cref="System.Data.DataSet"/>.</param>
        public SourceColumnAttribute(string sourceColumn)
            : base(typeof(MariaDbParameter), nameof(MariaDbParameter.SourceColumn), sourceColumn)
        { }

        /// <summary>
        /// Gets the mapped source column name of the parameter.
        /// </summary>
        public string SourceColumn => (string)Value;
    }
}
