#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using RepoDb.Interfaces;

namespace RepoDb.Schema
{
    /// <summary>
    /// Creates a schema in a database for the <c>CreateSchema</c> extension methods of the providers that support the schemas: the schema is created only if it does not exist yet, so
    /// calling it again is not an error. The provider gives its own statements (a schema cannot be a parameter of a statement, so its name is quoted by the provider into the statement).
    /// </summary>
    internal static class CreateSchemaExtension
    {
        #region Constants

        /// <summary>
        /// The name of the parameter of the statement that checks whether the schema exists.
        /// </summary>
        public const string ParameterName = "SchemaName";

        #endregion

        #region Methods

        /// <summary>
        /// Creates the schema if it does not exist.
        /// </summary>
        /// <param name="connection">The connection of the database.</param>
        /// <param name="schemaName">The name of the schema (it is not quoted, and it is matched as it is).</param>
        /// <param name="parameterPrefix">The prefix of the parameters of the provider (i.e.: <c>@</c>).</param>
        /// <param name="existsSql">The statement that counts the schemas that have the name, with <c>{0}</c> in the place of the parameter.</param>
        /// <param name="createSql">The statement that creates the schema, with <c>{0}</c> in the place of the quoted name.</param>
        /// <param name="quote">Quotes the name of the schema for the provider (a quote inside the name is escaped).</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="traceKey">The tracking key to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <returns><c>true</c> if the schema was created; <c>false</c> if it already existed.</returns>
        /// <exception cref="ArgumentNullException">The connection or the name of the schema is <c>null</c> (or white space).</exception>
        [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", Justification = "Parameters are a Dictionary<string, object>, and the non-query has no parameters; neither path reflects over an object.")]
        public static bool CreateSchema(IDbConnection connection,
            string schemaName,
            string parameterPrefix,
            string existsSql,
            string createSql,
            Func<string, string> quote,
            int? commandTimeout,
            string traceKey,
            ITrace trace,
            IDbTransaction transaction)
        {
            Validate(connection, schemaName);
            var exists = connection.ExecuteScalar(string.Format(CultureInfo.InvariantCulture, existsSql, parameterPrefix + ParameterName),
                new Dictionary<string, object> { [ParameterName] = schemaName },
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace);
            if (Exists(exists))
            {
                return false;
            }
            connection.ExecuteNonQuery(string.Format(CultureInfo.InvariantCulture, createSql, quote(schemaName)),
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace);
            return true;
        }

        /// <summary>
        /// Creates the schema if it does not exist in an asynchronous way.
        /// </summary>
        /// <param name="connection">The connection of the database.</param>
        /// <param name="schemaName">The name of the schema (it is not quoted, and it is matched as it is).</param>
        /// <param name="parameterPrefix">The prefix of the parameters of the provider (i.e.: <c>@</c>).</param>
        /// <param name="existsSql">The statement that counts the schemas that have the name, with <c>{0}</c> in the place of the parameter.</param>
        /// <param name="createSql">The statement that creates the schema, with <c>{0}</c> in the place of the quoted name.</param>
        /// <param name="quote">Quotes the name of the schema for the provider (a quote inside the name is escaped).</param>
        /// <param name="commandTimeout">The command timeout in seconds to be used.</param>
        /// <param name="traceKey">The tracking key to be used.</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is <c>true</c> if the schema was created; <c>false</c> if it already existed.</returns>
        /// <exception cref="ArgumentNullException">The connection or the name of the schema is <c>null</c> (or white space).</exception>
        [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", Justification = "Parameters are a Dictionary<string, object>, and the non-query has no parameters; neither path reflects over an object.")]
        public static async Task<bool> CreateSchemaAsync(IDbConnection connection,
            string schemaName,
            string parameterPrefix,
            string existsSql,
            string createSql,
            Func<string, string> quote,
            int? commandTimeout,
            string traceKey,
            ITrace trace,
            IDbTransaction transaction,
            CancellationToken cancellationToken)
        {
            Validate(connection, schemaName);
            var exists = await connection.ExecuteScalarAsync(string.Format(CultureInfo.InvariantCulture, existsSql, parameterPrefix + ParameterName),
                new Dictionary<string, object> { [ParameterName] = schemaName },
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                cancellationToken: cancellationToken).ConfigureAwait(false);
            if (Exists(exists))
            {
                return false;
            }
            await connection.ExecuteNonQueryAsync(string.Format(CultureInfo.InvariantCulture, createSql, quote(schemaName)),
                commandTimeout: commandTimeout,
                traceKey: traceKey,
                transaction: transaction,
                trace: trace,
                cancellationToken: cancellationToken).ConfigureAwait(false);
            return true;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Validates the arguments.
        /// </summary>
        /// <param name="connection">The connection of the database.</param>
        /// <param name="schemaName">The name of the schema.</param>
        /// <exception cref="ArgumentNullException">The connection or the name of the schema is <c>null</c> (or white space).</exception>
        private static void Validate(IDbConnection connection,
            string schemaName)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }
            if (string.IsNullOrWhiteSpace(schemaName))
            {
                throw new ArgumentNullException(nameof(schemaName));
            }
        }

        /// <summary>
        /// Checks whether the result of the statement that counts the schemas says that the schema exists.
        /// </summary>
        /// <param name="count">The result (the number of schemas), or <c>null</c>.</param>
        /// <returns><c>true</c> if at least one schema was found.</returns>
        private static bool Exists(object count) =>
            count != null && !(count is DBNull) && Convert.ToInt64(count, CultureInfo.InvariantCulture) > 0;

        #endregion
    }
}
