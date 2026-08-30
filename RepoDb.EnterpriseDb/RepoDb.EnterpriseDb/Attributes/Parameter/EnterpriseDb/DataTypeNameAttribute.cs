#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using EnterpriseDB.EDBClient;

namespace RepoDb.Attributes.Parameter.EnterpriseDb
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="EDBParameter.DataTypeName"/>
    /// property via an entity property before the actual execution.
    /// </summary>
    public class DataTypeNameAttribute : PropertyValueAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="DataTypeNameAttribute"/> class.
        /// </summary>
        /// <param name="dataTypeName">The name of the PostgreSQL type.</param>
        public DataTypeNameAttribute(string dataTypeName)
            : base(typeof(EDBParameter), nameof(EDBParameter.DataTypeName), dataTypeName)
        { }

        /// <summary>
        /// Gets the mapped name of the PostgreSQL type of the parameter.
        /// </summary>
        public string DataTypeName => (string)Value;
    }
}