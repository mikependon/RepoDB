#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using EnterpriseDB.EDBClient;
using EDBTypes;
using RepoDb.DbSettings;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Resolvers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb.DbHelpers
{
    /// <summary>
    /// A helper class for database specially for the direct access. This class is only meant for EnterpriseDB Postgres Advanced Server.
    /// </summary>
    public sealed class EnterpriseDbDbHelper : IDbHelper
    {
        private readonly IDbSetting m_dbSetting = new EnterpriseDbDbSetting();

        /// <summary>
        /// Creates a new instance of <see cref="EnterpriseDbDbHelper"/> class.
        /// </summary>
        public EnterpriseDbDbHelper()
            : this(new EnterpriseDbDbTypeNameToClientTypeResolver())
        { }

        /// <summary>
        /// Creates a new instance of <see cref="EnterpriseDbDbHelper"/> class.
        /// </summary>
        /// <param name="dbTypeResolver">The type resolver to be used.</param>
        public EnterpriseDbDbHelper(IResolver<string, Type> dbTypeResolver)
        {
            DbTypeResolver = dbTypeResolver;
        }

        #region Properties

        /// <summary>
        /// Gets the type resolver used by this <see cref="EnterpriseDbDbHelper"/> instance.
        /// </summary>
        public IResolver<string, Type> DbTypeResolver { get; }

        #endregion

        #region Helpers

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        private string GetCommandText()
        {
            return """
                   SELECT DISTINCT C.column_name,
                       COALESCE(I.indisprimary, FALSE) AS IsPrimary,
                       CASE
                           WHEN C.is_identity = 'YES'
                                OR POSITION('NEXTVAL' IN UPPER(C.column_default)) >= 1 THEN TRUE
                           ELSE FALSE
                       END AS IsIdentity,
                       CAST(C.is_nullable AS BOOLEAN) AS IsNullable,
                       C.data_type AS DataType,
                       CASE
                           WHEN C.column_default IS NOT NULL THEN TRUE
                           ELSE FALSE
                       END AS HasDefaultValue
                   FROM information_schema.columns C
                   LEFT JOIN pg_index I ON I.indrelid = (quote_ident(C.table_schema) || '.' || quote_ident(C.table_name))::regclass
                   AND C.ordinal_position = ANY (I.indkey)
                   WHERE C.table_name = @TableName
                     AND (
                         C.table_schema = @Schema
                         OR C.table_schema = (SELECT nspname FROM pg_namespace WHERE oid = pg_my_temp_schema())
                     );
                   """;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private DbField ReaderToDbField(DbDataReader reader)
        {
            return new DbField(reader.GetString(0),
                !reader.IsDBNull(1) && reader.GetBoolean(1),
                !reader.IsDBNull(2) && reader.GetBoolean(2),
                !reader.IsDBNull(3) && reader.GetBoolean(3),
                reader.IsDBNull(4) ? DbTypeResolver.Resolve("text") : DbTypeResolver.Resolve(reader.GetString(4)),
                null,
                null,
                null,
                reader.IsDBNull(4) ? "text" : reader.GetString(4),
                !reader.IsDBNull(5) && reader.GetBoolean(5),
                "PGSQL");
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DbField> ReaderToDbFieldAsync(DbDataReader reader,
            CancellationToken cancellationToken = default)
        {
            return new DbField(await reader.GetFieldValueAsync<string>(0, cancellationToken),
                !await reader.IsDBNullAsync(1, cancellationToken) && await reader.GetFieldValueAsync<bool>(1, cancellationToken),
                !await reader.IsDBNullAsync(2, cancellationToken) && await reader.GetFieldValueAsync<bool>(2, cancellationToken),
                !await reader.IsDBNullAsync(3, cancellationToken) && await reader.GetFieldValueAsync<bool>(3, cancellationToken),
                await reader.IsDBNullAsync(4, cancellationToken) ? DbTypeResolver.Resolve("text") : DbTypeResolver.Resolve(await reader.GetFieldValueAsync<string>(4, cancellationToken)),
                null,
                null,
                null,
                await reader.IsDBNullAsync(4, cancellationToken) ? "text" : reader.GetString(4),
                !await reader.IsDBNullAsync(5, cancellationToken) && await reader.GetFieldValueAsync<bool>(5, cancellationToken),
                "PGSQL");
        }

        #endregion

        #region Methods

        /// <summary>
        /// Matches an "operation already in progress on this connection" exception from either the official
        /// <c>EnterpriseDB.EDBClient</c> driver (<c>EDBOperationInProgressException</c>) or the Npgsql-backed
        /// <c>RepoDb.Connector.EnterpriseDb</c> driver (Npgsql's own <c>NpgsqlOperationInProgressException</c>,
        /// surfaced as-is since that connector wraps <c>NpgsqlConnection</c> directly rather than translating
        /// its exceptions) - matched by type name so this helper needs no compile-time reference to either
        /// driver's exception type beyond the one (<c>EDBOperationInProgressException</c>) it already
        /// references for other purposes.
        /// </summary>
        private static bool IsOperationInProgressException(Exception ex) =>
            ex.GetType().Name is nameof(EDBOperationInProgressException) or "NpgsqlOperationInProgressException";

        private TResult TryExecuteOnExistingConnection<TResult>(IDbConnection connection, Func<IDbConnection, TResult> func)
        {
            try
            {
                return func(connection);
            }
            catch (Exception ex) when (IsOperationInProgressException(ex))
            {
                Debug.WriteLine($"{ex.GetType().Name} occurred. Retrying the operation on a new connection.");
                using var newConnection = (IDbConnection)Activator.CreateInstance(connection.GetType(), connection.ConnectionString);
                newConnection.Open();
                return func(newConnection);
            }
        }

        private async Task<TResult> TryExecuteOnExistingConnectionAsync<TResult>(IDbConnection connection, Func<IDbConnection, Task<TResult>> func)
        {
            try
            {
                return await func(connection);
            }
            catch (Exception ex) when (IsOperationInProgressException(ex))
            {
                Debug.WriteLine($"{ex.GetType().Name} occurred. Retrying the operation on a new connection.");
                await using var newConnection = (DbConnection)Activator.CreateInstance(connection.GetType(), connection.ConnectionString);
                await newConnection.OpenAsync();
                return await func(newConnection);
            }
        }

        #region GetFields

        /// <summary>
        /// Gets the list of <see cref="DbField"/> of the table.
        /// </summary>
        /// <param name="connection">The instance of the connection object.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <returns>A list of <see cref="DbField"/> of the target table.</returns>
        public IEnumerable<DbField> GetFields(IDbConnection connection,
            string tableName,
            IDbTransaction transaction = null)

         => TryExecuteOnExistingConnection(connection, c => GetFieldsInternal(c, tableName, transaction));

        private IEnumerable<DbField> GetFieldsInternal(IDbConnection connection,
            string tableName,
            IDbTransaction transaction = null)
        {
            // Variables
            var commandText = GetCommandText();
            var param = new
            {
                Schema = DataEntityExtension.GetSchema(tableName, m_dbSetting).AsUnquoted(m_dbSetting),
                TableName = DataEntityExtension.GetTableName(tableName, m_dbSetting).AsUnquoted(m_dbSetting)
            };

            // Iterate and extract
            using var reader = (DbDataReader)connection.ExecuteReader(commandText, param, transaction: transaction);

            var dbFields = new List<DbField>();

            // Iterate the list of the fields
            while (reader.Read())
            {
                dbFields.Add(ReaderToDbField(reader));
            }

            // Return the list of fields
            return dbFields;
        }

        /// <summary>
        /// Gets the list of <see cref="DbField"/> of the table in an asynchronous way.
        /// </summary>
        /// <param name="connection">The instance of the connection object.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>A list of <see cref="DbField"/> of the target table.</returns>
        public Task<IEnumerable<DbField>> GetFieldsAsync(IDbConnection connection,
            string tableName,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)

         => TryExecuteOnExistingConnectionAsync(connection, c => GetFieldsAsyncInternal(c, tableName, transaction, cancellationToken));

        private async Task<IEnumerable<DbField>> GetFieldsAsyncInternal(IDbConnection connection,
            string tableName,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            // Variables
            var commandText = GetCommandText();
            var param = new
            {
                Schema = DataEntityExtension.GetSchema(tableName, m_dbSetting).AsUnquoted(m_dbSetting),
                TableName = DataEntityExtension.GetTableName(tableName, m_dbSetting).AsUnquoted(m_dbSetting)
            };

            // Iterate and extract
            using var reader = (DbDataReader)await connection.ExecuteReaderAsync(commandText, param, transaction: transaction,
                cancellationToken: cancellationToken);

            var dbFields = new List<DbField>();

            // Iterate the list of the fields
            while (await reader.ReadAsync(cancellationToken))
            {
                dbFields.Add(await ReaderToDbFieldAsync(reader, cancellationToken));
            }

            // Return the list of fields
            return dbFields;
        }

        #endregion

        #region GetScopeIdentity

        /// <summary>
        /// Gets the newly generated identity from the database.
        /// </summary>
        /// <typeparam name="T">The type of newly generated identity.</typeparam>
        /// <param name="connection">The instance of the connection object.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <returns>The newly generated identity from the database.</returns>
        public T GetScopeIdentity<T>(IDbConnection connection,
            IDbTransaction transaction = null)

         => TryExecuteOnExistingConnection(connection, c => GetScopeIdentityInternal<T>(c, transaction));

        private T GetScopeIdentityInternal<T>(IDbConnection connection,
            IDbTransaction transaction = null)
        {
            // TODO: May fail with trigger?
            return connection.ExecuteScalar<T>("SELECT lastval();", transaction: transaction);
        }

        /// <summary>
        /// Gets the newly generated identity from the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="T">The type of newly generated identity.</typeparam>
        /// <param name="connection">The instance of the connection object.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The newly generated identity from the database.</returns>
        public Task<T> GetScopeIdentityAsync<T>(IDbConnection connection,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)

         => TryExecuteOnExistingConnectionAsync(connection, c => GetScopeIdentityAsyncInternal<T>(c, transaction, cancellationToken));

        private Task<T> GetScopeIdentityAsyncInternal<T>(IDbConnection connection,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            // TODO: May fail with trigger?
            return connection.ExecuteScalarAsync<T>("SELECT lastval();", transaction: transaction,
                cancellationToken: cancellationToken);
        }

        #endregion

        #region DynamicHandler

        /// <summary>
        /// A backdoor access from the core library used to handle an instance of an object to whatever purpose within the extended library.
        /// </summary>
        /// <typeparam name="TEventInstance">The type of the event instance to handle.</typeparam>
        /// <param name="instance">The instance of the event object to handle.</param>
        /// <param name="key">The key of the event to handle.</param>
        public void DynamicHandler<TEventInstance>(TEventInstance instance,
            string key)
        {
            if (key == "RepoDb.Internal.Compiler.Events[AfterCreateDbParameter]")
            {
                HandleDbParameterPostCreation(instance as IDbDataParameter);
            }
        }

        #region Handlers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        private void HandleDbParameterPostCreation(IDbDataParameter parameter)
        {
            if (parameter?.Value is Array sourceArray)
            {
                HandleArrayDbParameterPostCreation(parameter as EDBParameter, sourceArray);
                return;
            }

            if (parameter?.Value is DateTime dateTime && dateTime.Kind != DateTimeKind.Utc)
            {
                parameter.DbType = DbType.DateTime2;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="sourceArray"></param>
        private void HandleArrayDbParameterPostCreation(EDBParameter parameter, Array sourceArray)
        {
            if (parameter == null)
            {
                return;
            }
            var resolvedDbType = new ClientTypeToEDBDbTypeResolver().Resolve(sourceArray.GetType());
            if (resolvedDbType == null)
            {
                return;
            }
            parameter.EDBDbType = resolvedDbType.Value;
            var elementType = sourceArray.GetType().GetElementType();
            if (elementType == typeof(DateOnly))
            {
                parameter.Value = Array.ConvertAll((DateOnly[])sourceArray, d => d.ToDateTime(TimeOnly.MinValue));
            }
            else if (elementType == typeof(TimeOnly))
            {
                parameter.Value = Array.ConvertAll((TimeOnly[])sourceArray, t => t.ToTimeSpan());
            }
        }

#endregion

#endregion

#endregion
    }
}
