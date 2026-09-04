#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Connector.EnterpriseDb;

namespace RepoDb.Attributes.Parameter.EnterpriseDb
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="EDBParameter.SourceColumn"/>
    /// property via an entity property before the actual execution.
    /// </summary>
    public class SourceColumnAttribute : PropertyValueAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="SourceColumnAttribute"/> class.
        /// </summary>
        /// <param name="sourceColumn">The name of the source column mapped to the <see cref="System.Data.DataSet"/>.</param>
        public SourceColumnAttribute(string sourceColumn)
            : base(typeof(EDBParameter), nameof(EDBParameter.SourceColumn), sourceColumn)
        { }

        /// <summary>
        /// Gets the mapped name of the source column mapped to the <see cref="System.Data.DataSet"/>.
        /// </summary>
        public string SourceColumn => (string)Value;
    }
}
