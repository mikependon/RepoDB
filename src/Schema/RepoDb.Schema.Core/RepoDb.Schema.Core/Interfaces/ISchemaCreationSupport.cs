#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.Schema
{
    /// <summary>
    /// An interface that a schema composer implements when the destination database has schemas that can be created. The loaders of the definition files
    /// create the schemas of the tables that do not exist yet in the destination database before the tables are created.
    /// </summary>
    public interface ISchemaCreationSupport
    {
        #region Properties

        /// <summary>
        /// Gets the statement that creates a schema (the placeholder <c>{0}</c> takes the quoted name of the schema).
        /// </summary>
        string CreateSchemaSql { get; }

        /// <summary>
        /// Gets the prefix of the parameters of the destination database (i.e.: <c>@</c>).
        /// </summary>
        string SchemaParameterPrefix { get; }

        /// <summary>
        /// Gets the statement that counts the schemas that have a name (the placeholder <c>{0}</c> takes the name of the parameter with its prefix).
        /// </summary>
        string SchemaExistsSql { get; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Quotes the name of the schema in the dialect of the destination database.
        /// </summary>
        /// <param name="schemaName">The name of the schema.</param>
        /// <returns>The quoted name of the schema.</returns>
        string QuoteSchemaName(string schemaName);

        #endregion
    }
}
