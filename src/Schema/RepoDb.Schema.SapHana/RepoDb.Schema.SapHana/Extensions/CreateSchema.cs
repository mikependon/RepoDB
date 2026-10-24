#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Sap.Data.Hana;
using RepoDb.Interfaces;

namespace RepoDb.Schema.SapHana
{
    /// <summary>
    /// Contains the extension methods of the <see cref="HanaConnection"/> object for creating a schema in a SAP HANA database.
    /// </summary>
    public static class CreateSchemaExtension
    {
        #region Methods

        /// <summary>
        /// Creates a schema in the database if it does not exist yet, so calling it again for the same schema is not an error.
        /// </summary>
        /// <param name="connection">The connection of the database.</param>
        /// <param name="schemaName">The name of the schema to be created. It is quoted by the method, so it can have spaces or quotes, and it is matched as it is.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="SchemaTraceKeys.CreateSchema"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used. The default is <c>null</c>.</param>
        /// <returns><c>true</c> if the schema was created; <c>false</c> if it already existed.</returns>
        /// <exception cref="ArgumentNullException">The connection or the name of the schema is <c>null</c> (or white space).</exception>
        public static bool CreateSchema(this HanaConnection connection,
            string schemaName,
            int? commandTimeout = null,
            string traceKey = SchemaTraceKeys.CreateSchema,
            ITrace trace = null,
            IDbTransaction transaction = null) =>
            RepoDb.Schema.CreateSchemaExtension.CreateSchema(connection, schemaName, ":", SapHanaSchemaText.SchemaExistsSql, SapHanaSchemaText.CreateSchemaSql, SapHanaSchemaHelper.Quote, commandTimeout, traceKey, trace, transaction);

        /// <summary>
        /// Creates a schema in the database in an asynchronous way if it does not exist yet, so calling it again for the same schema is not an error.
        /// </summary>
        /// <param name="connection">The connection of the database.</param>
        /// <param name="schemaName">The name of the schema to be created. It is quoted by the method, so it can have spaces or quotes, and it is matched as it is.</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used. The default is <c>null</c>.</param>
        /// <param name="traceKey">The tracking key to be used. The default is <see cref="SchemaTraceKeys.CreateSchema"/>.</param>
        /// <param name="trace">The trace object to be used. The default is <c>null</c>.</param>
        /// <param name="transaction">The transaction to be used. The default is <c>null</c>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is <c>true</c> if the schema was created; <c>false</c> if it already existed.</returns>
        /// <exception cref="ArgumentNullException">The connection or the name of the schema is <c>null</c> (or white space).</exception>
        public static Task<bool> CreateSchemaAsync(this HanaConnection connection,
            string schemaName,
            int? commandTimeout = null,
            string traceKey = SchemaTraceKeys.CreateSchema,
            ITrace trace = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default) =>
            RepoDb.Schema.CreateSchemaExtension.CreateSchemaAsync(connection, schemaName, ":", SapHanaSchemaText.SchemaExistsSql, SapHanaSchemaText.CreateSchemaSql, SapHanaSchemaHelper.Quote, commandTimeout, traceKey, trace, transaction, cancellationToken);

        #endregion
    }
}
