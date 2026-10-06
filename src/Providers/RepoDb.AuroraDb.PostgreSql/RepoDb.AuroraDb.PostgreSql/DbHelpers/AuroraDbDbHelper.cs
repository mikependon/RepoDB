#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Diagnostics.CodeAnalysis;
using RepoDb.Connector.AuroraDb.Npgsql;
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
    /// A helper class for database specially for the direct access. This class is only meant for Aurora PostgreSQL.
    /// </summary>
    public sealed class AuroraDbDbHelper : IDbHelper
    {
        private readonly IDbSetting m_dbSetting = new AuroraDbDbSetting();

        /// <summary>
        /// Creates a new instance of <see cref="AuroraDbDbHelper"/> class.
        /// </summary>
        public AuroraDbDbHelper()
            : this(new AuroraDbDbTypeNameToClientTypeResolver())
        { }

        /// <summary>
        /// Creates a new instance of <see cref="AuroraDbDbHelper"/> class.
        /// </summary>
        /// <param name="dbTypeResolver">The type resolver to be used.</param>
        public AuroraDbDbHelper(IResolver<string, Type> dbTypeResolver)
        {
            DbTypeResolver = dbTypeResolver;
        }

        #region Properties

        /// <summary>
        /// Gets the type resolver used by this <see cref="AuroraDbDbHelper"/> instance.
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
                       END AS HasDefaultValue,
                       C.ordinal_position AS OrdinalPosition
                   FROM information_schema.columns C
                   LEFT JOIN pg_index I ON I.indrelid = (quote_ident(C.table_schema) || '.' || quote_ident(C.table_name))::regclass
                   AND C.ordinal_position = ANY (I.indkey)
                   WHERE C.table_name = @TableName
                     AND (
                         C.table_schema = @Schema
                         OR C.table_schema = (SELECT nspname FROM pg_namespace WHERE oid = pg_my_temp_schema())
                     )
                   ORDER BY C.ordinal_position;
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
            return new DbField(await reader.GetFieldValueAsync<string>(0, cancellationToken).ConfigureAwait(false),
                !await reader.IsDBNullAsync(1, cancellationToken).ConfigureAwait(false) && await reader.GetFieldValueAsync<bool>(1, cancellationToken).ConfigureAwait(false),
                !await reader.IsDBNullAsync(2, cancellationToken).ConfigureAwait(false) && await reader.GetFieldValueAsync<bool>(2, cancellationToken).ConfigureAwait(false),
                !await reader.IsDBNullAsync(3, cancellationToken).ConfigureAwait(false) && await reader.GetFieldValueAsync<bool>(3, cancellationToken).ConfigureAwait(false),
                await reader.IsDBNullAsync(4, cancellationToken).ConfigureAwait(false) ? DbTypeResolver.Resolve("text") : DbTypeResolver.Resolve(await reader.GetFieldValueAsync<string>(4, cancellationToken).ConfigureAwait(false)),
                null,
                null,
                null,
                await reader.IsDBNullAsync(4, cancellationToken).ConfigureAwait(false) ? "text" : reader.GetString(4),
                !await reader.IsDBNullAsync(5, cancellationToken).ConfigureAwait(false) && await reader.GetFieldValueAsync<bool>(5, cancellationToken).ConfigureAwait(false),
                "PGSQL");
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        private static bool IsOperationInProgressException(Exception ex) => string.Equals(ex.GetType().Name, "NpgsqlOperationInProgressException", StringComparison.Ordinal);

        /// <summary>

        /// Creates a new (closed) connection with the same connection string as the given connection.

        /// </summary>

        /// <param name="connection">The existing connection.</param>

        /// <returns>The new connection.</returns>

        private static DbConnection CreateNewConnection(IDbConnection connection) =>

            connection.GetType() == typeof(AuroraDbConnection) ? new AuroraDbConnection(connection.ConnectionString) : CreateNewConnectionViaReflection(connection);


        /// <summary>

        /// Creates a new (closed) connection of the runtime type of the given connection (i.e.: a type derived from <see cref="AuroraDbConnection"/>).

        /// </summary>

        /// <param name="connection">The existing connection.</param>

        /// <returns>The new connection.</returns>

        [UnconditionalSuppressMessage("Trimming", "IL2072:Target parameter argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method.",

            Justification = "Only used for the types derived from AuroraDbConnection (which is constructed directly), whose constructors are expected to be preserved by the application.")]

        private static DbConnection CreateNewConnectionViaReflection(IDbConnection connection) =>

            (DbConnection)Activator.CreateInstance(connection.GetType(), connection.ConnectionString);


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="connection"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        private TResult TryExecuteOnExistingConnection<TResult>(IDbConnection connection,
            Func<IDbConnection, TResult> func)
        {
            try
            {
                return func(connection);
            }
            catch (Exception ex) when (IsOperationInProgressException(ex))
            {
                Debug.WriteLine($"{ex.GetType().Name} occurred. Retrying the operation on a new connection.");
                using var newConnection = CreateNewConnection(connection);
                newConnection.Open();
                return func(newConnection);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="connection"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        private async Task<TResult> TryExecuteOnExistingConnectionAsync<TResult>(IDbConnection connection,
            Func<IDbConnection,
            Task<TResult>> func)
        {
            try
            {
                return await func(connection).ConfigureAwait(false);
            }
            catch (Exception ex) when (IsOperationInProgressException(ex))
            {
                Debug.WriteLine($"{ex.GetType().Name} occurred. Retrying the operation on a new connection.");
                var newConnection = CreateNewConnection(connection);
                await using (newConnection.ConfigureAwait(true))
                {
                    await newConnection.OpenAsync().ConfigureAwait(false);
                    return await func(newConnection).ConfigureAwait(false);
                }
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

        [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", Justification = "The parameter object passed to the RepoDB execute methods is either null or a Dictionary<string, object>, which is not reflected.")]
        private IEnumerable<DbField> GetFieldsInternal(IDbConnection connection,
            string tableName,
            IDbTransaction transaction = null)
        {
            // Variables
            var commandText = GetCommandText();
            var param = new Dictionary<string, object>
            {
                ["Schema"] = DataEntityExtension.GetSchema(tableName, m_dbSetting).AsUnquoted(m_dbSetting),
                ["TableName"] = DataEntityExtension.GetTableName(tableName, m_dbSetting).AsUnquoted(m_dbSetting)
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

        [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", Justification = "The parameter object passed to the RepoDB execute methods is either null or a Dictionary<string, object>, which is not reflected.")]
        private async Task<IEnumerable<DbField>> GetFieldsAsyncInternal(IDbConnection connection,
            string tableName,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            // Variables
            var commandText = GetCommandText();
            var param = new Dictionary<string, object>
            {
                ["Schema"] = DataEntityExtension.GetSchema(tableName, m_dbSetting).AsUnquoted(m_dbSetting),
                ["TableName"] = DataEntityExtension.GetTableName(tableName, m_dbSetting).AsUnquoted(m_dbSetting)
            };

            // Iterate and extract
            using var reader = (DbDataReader)await connection.ExecuteReaderAsync(commandText, param, transaction: transaction,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            var dbFields = new List<DbField>();

            // Iterate the list of the fields
            while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            {
                dbFields.Add(await ReaderToDbFieldAsync(reader, cancellationToken).ConfigureAwait(false));
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

        [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", Justification = "No parameter object is passed to the RepoDB execute methods.")]
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

        [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", Justification = "No parameter object is passed to the RepoDB execute methods.")]
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
            if (string.Equals(key, "RepoDb.Internal.Compiler.Events[AfterCreateDbParameter]", StringComparison.Ordinal))
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
                HandleArrayDbParameterPostCreation(parameter as AuroraDbParameter, sourceArray);
                return;
            }

            if (parameter?.Value is DateOnly dateOnly)
            {
                parameter.Value = dateOnly.ToDateTime(TimeOnly.MinValue);
                parameter.DbType = DbType.Date;
                return;
            }

            if (parameter?.Value is TimeOnly timeOnly)
            {
                parameter.Value = timeOnly.ToTimeSpan();
                parameter.DbType = DbType.Time;
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
        private void HandleArrayDbParameterPostCreation(AuroraDbParameter parameter, Array sourceArray)
        {
            if (parameter == null)
            {
                return;
            }
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
