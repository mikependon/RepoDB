#region Copyright Attributions

// Copyright (c) 2021 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.Data.SqlClient;

namespace RepoDb.Attributes.Parameter.SqlServer
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="SqlParameter.XmlSchemaCollectionName"/>
    /// property via an entity property before the actual execution.
    /// </summary>
    /// <remarks>
    /// Creates a new instance of <see cref="XmlSchemaCollectionNameAttribute"/> class.
    /// </remarks>
    /// <param name="collectionName">The value of the schema collection.</param>
    [System.AttributeUsage(System.AttributeTargets.All)]
    public class XmlSchemaCollectionNameAttribute(string collectionName) : PropertyValueAttribute(typeof(SqlParameter), nameof(SqlParameter.XmlSchemaCollectionName), collectionName)
    {

        /// <summary>
        /// Gets the mapped value of the schema collection of the parameter.
        /// </summary>
        public string XmlSchemaCollectionName => (string)Value;
    }
}