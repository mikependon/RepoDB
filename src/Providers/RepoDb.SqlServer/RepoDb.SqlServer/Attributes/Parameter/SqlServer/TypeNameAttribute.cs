#region Copyright Attributions

// Copyright (c) 2021 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.Data.SqlClient;

namespace RepoDb.Attributes.Parameter.SqlServer
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="SqlParameter.TypeName"/> property via an entity property
    /// before the actual execution.
    /// </summary>
    /// <remarks>
    /// Creates a new instance of <see cref="TypeNameAttribute"/> class.
    /// </remarks>
    /// <param name="typeName">The type name of the table-valued parameter (TVP) object.</param>
    [System.AttributeUsage(System.AttributeTargets.All)]
    public class TypeNameAttribute(string typeName) : PropertyValueAttribute(typeof(SqlParameter), nameof(SqlParameter.TypeName), typeName)
    {

        /// <summary>
        /// Gets the name of the mapped table-valued parameter object (TVP) of the parameter.
        /// </summary>
        public string TypeName => (string)Value;
    }
}