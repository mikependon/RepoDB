#region Copyright Attributions

// Copyright (c) 2021 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.Data.SqlClient;

namespace RepoDb.Attributes.Parameter.SqlServer
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="SqlParameter.XmlSchemaCollectionDatabase"/>
    /// property via an entity property before the actual execution.
    /// </summary>
    /// <remarks>
    /// Creates a new instance of <see cref="XmlSchemaCollectionDatabaseAttribute"/> class.
    /// </remarks>
    /// <param name="databaseName">The name of the database where the schema collection is located.</param>
    [System.AttributeUsage(System.AttributeTargets.All)]
    public class XmlSchemaCollectionDatabaseAttribute(string databaseName) : PropertyValueAttribute(typeof(SqlParameter), nameof(SqlParameter.XmlSchemaCollectionDatabase), databaseName)
    {

        /// <summary>
        /// Gets the mapped name of the database where the schema collection is located for the parameter.
        /// </summary>
        public string XmlSchemaCollectionDatabase => (string)Value;
    }
}