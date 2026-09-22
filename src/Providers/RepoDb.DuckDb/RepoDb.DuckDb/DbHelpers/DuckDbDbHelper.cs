#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using DuckDB.NET.Data;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Resolvers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb.DbHelpers
{
    /// <summary>
    /// A helper class for database specially for the direct access. This class is only meant for DuckDB.
    /// </summary>
    public sealed class DuckDbDbHelper : IDbHelper
    {
        private IDbSetting m_dbSetting = DbSettingMapper.Get<DuckDBConnection>();

        /// <summary>
        /// Creates a new instance of <see cref="DuckDbDbHelper"/> class.
        /// </summary>
        public DuckDbDbHelper()
            : this(new DuckDbTypeNameToClientTypeResolver())
        { }

        /// <summary>
        /// Creates a new instance of <see cref="DuckDbDbHelper"/> class.
        /// </summary>
        /// <param name="dbTypeResolver">The type resolver to be used.</param>
        public DuckDbDbHelper(IResolver<string, Type> dbTypeResolver)
        {
            DbTypeResolver = dbTypeResolver;
        }

        #region Properties

        /// <summary>
        /// Gets the type resolver used by this <see cref="DuckDbDbHelper"/> instance.
        /// </summary>
        public IResolver<string, Type> DbTypeResolver { get; }

        #endregion

        #region Helpers

        /// <summary>
        /// Builds the introspection query. Primary-key detection joins against <c>duckdb_constraints()</c>
        /// (DuckDB's <c>information_schema.columns</c> has no MySQL-style COLUMN_KEY/EXTRA columns), and
        /// identity/auto-generated columns are detected by their default expression referencing a sequence
        /// via 'nextval(...)' — DuckDB has no native AUTO_INCREMENT, so a sequence-backed DEFAULT is the
        /// closest analog (see DuckDbStatementBuilder / the integration tests' DDL for how the sequence is set up).
        /// </summary>
        /// <returns></returns>
        private string GetCommandText()
        {
            return @"SELECT
                    c.column_name AS ColumnName
                    , COALESCE(pk.is_primary, FALSE) AS IsPrimary
                    , (c.column_default IS NOT NULL AND CONTAINS(UPPER(c.column_default), 'NEXTVAL')) AS IsIdentity
                    , (c.is_nullable = 'YES') AS IsNullable
                    , c.data_type AS ColumnType
                    , c.character_maximum_length AS Size
                    , c.numeric_precision AS ColumnPrecision
                    , c.numeric_scale AS ColumnScale
                    , c.data_type AS DatabaseType
                    , (c.column_default IS NOT NULL) AS HasDefaultValue
                FROM information_schema.columns c
                LEFT JOIN (
                    SELECT table_name, UNNEST(constraint_column_names) AS column_name, TRUE AS is_primary
                    FROM duckdb_constraints()
                    WHERE constraint_type = 'PRIMARY KEY'
                ) pk ON pk.column_name = c.column_name AND pk.table_name = c.table_name
                WHERE c.table_schema = $TableSchema
                    AND c.table_name = $TableName
                ORDER BY c.ordinal_position;";
        }

        /// <summary>
        /// DuckDB reports parameterized types (e.g. 'DECIMAL(18,4)') as part of the type name itself;
        /// strip the parenthesized part before resolving it to a CLR type.
        /// </summary>
        /// <param name="columnType"></param>
        /// <returns></returns>
        private static string GetBaseTypeName(string columnType)
        {
            var index = columnType.IndexOf('(');
            return index >= 0 ? columnType.Substring(0, index).Trim() : columnType;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private DbField ReaderToDbField(DbDataReader reader)
        {
            var columnType = reader.GetString(4);
            return new DbField(reader.GetString(0),
                reader.GetBoolean(1),
                reader.GetBoolean(2),
                reader.GetBoolean(3),
                DbTypeResolver.Resolve(GetBaseTypeName(columnType)),
                reader.IsDBNull(5) ? (int?)null : Convert.ToInt32(reader.GetValue(5), CultureInfo.InvariantCulture),
                reader.IsDBNull(6) ? (byte?)null : Convert.ToByte(reader.GetValue(6), CultureInfo.InvariantCulture),
                reader.IsDBNull(7) ? (byte?)null : Convert.ToByte(reader.GetValue(7), CultureInfo.InvariantCulture),
                reader.GetString(8),
                reader.GetBoolean(9),
                "DUCKDB");
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
            var columnType = await reader.GetFieldValueAsync<string>(4, cancellationToken).ConfigureAwait(false);
            return new DbField(await reader.GetFieldValueAsync<string>(0, cancellationToken).ConfigureAwait(false),
                await reader.GetFieldValueAsync<bool>(1, cancellationToken).ConfigureAwait(false),
                await reader.GetFieldValueAsync<bool>(2, cancellationToken).ConfigureAwait(false),
                await reader.GetFieldValueAsync<bool>(3, cancellationToken).ConfigureAwait(false),
                DbTypeResolver.Resolve(GetBaseTypeName(columnType)),
                await reader.IsDBNullAsync(5, cancellationToken).ConfigureAwait(false) ? (int?)null :
                    Convert.ToInt32(await reader.GetFieldValueAsync<object>(5, cancellationToken).ConfigureAwait(false), CultureInfo.InvariantCulture),
                await reader.IsDBNullAsync(6, cancellationToken).ConfigureAwait(false) ? (byte?)null :
                    Convert.ToByte(await reader.GetFieldValueAsync<object>(6, cancellationToken).ConfigureAwait(false), CultureInfo.InvariantCulture),
                await reader.IsDBNullAsync(7, cancellationToken).ConfigureAwait(false) ? (byte?)null :
                    Convert.ToByte(await reader.GetFieldValueAsync<object>(7, cancellationToken).ConfigureAwait(false), CultureInfo.InvariantCulture),
                await reader.GetFieldValueAsync<string>(8, cancellationToken).ConfigureAwait(false),
                await reader.GetFieldValueAsync<bool>(9, cancellationToken).ConfigureAwait(false),
                "DUCKDB");
        }

        #endregion

        #region Methods

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
        {
            // Variables
            var commandText = GetCommandText();
            var param = new
            {
                TableSchema = DataEntityExtension.GetSchema(tableName, m_dbSetting).AsUnquoted(m_dbSetting),
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
        public async Task<IEnumerable<DbField>> GetFieldsAsync(IDbConnection connection,
            string tableName,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Variables
            var commandText = GetCommandText();
            var param = new
            {
                TableSchema = DataEntityExtension.GetSchema(tableName, m_dbSetting).AsUnquoted(m_dbSetting),
                TableName = DataEntityExtension.GetTableName(tableName, m_dbSetting).AsUnquoted(m_dbSetting)
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
        /// DuckDB has no session-scoped "last identity" function (unlike MySQL's LAST_INSERT_ID() or
        /// PostgreSQL's lastval()); retrieving a sequence's current value requires knowing the sequence's
        /// name via 'currval(seq)', which this method's signature does not provide. This path is only ever
        /// reached by <c>RepoDb.Core</c> when <see cref="IDbSetting.IsMultiStatementExecutable"/> is
        /// <c>false</c> (see Operations/DbConnection/Insert.cs); since <see cref="DuckDbDbSetting"/> always
        /// sets it to <c>true</c>, identity values are returned via a 'RETURNING' clause on the
        /// insert/merge statement itself (see <see cref="StatementBuilders.DuckDbStatementBuilder"/>) and
        /// this method is effectively unreachable in normal use.
        /// </summary>
        /// <typeparam name="T">The type of newly generated identity.</typeparam>
        /// <param name="connection">The instance of the connection object.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <returns>The newly generated identity from the database.</returns>
        public T GetScopeIdentity<T>(IDbConnection connection,
            IDbTransaction transaction = null)
        {
            throw new NotSupportedException("DuckDB has no session-scoped 'last identity' function. " +
                "Identity values are returned via the 'RETURNING' clause on Insert/InsertAll/Merge/MergeAll instead.");
        }

        /// <summary>
        /// Gets the newly generated identity from the database in an asynchronous way. See the remarks on
        /// <see cref="GetScopeIdentity{T}(IDbConnection, IDbTransaction)"/>.
        /// </summary>
        /// <typeparam name="T">The type of newly generated identity.</typeparam>
        /// <param name="connection">The instance of the connection object.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The newly generated identity from the database.</returns>
        public Task<T> GetScopeIdentityAsync<T>(IDbConnection connection,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException("DuckDB has no session-scoped 'last identity' function. " +
                "Identity values are returned via the 'RETURNING' clause on Insert/InsertAll/Merge/MergeAll instead.");
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
                HandleDbParameterPostCreation((DuckDBParameter)(object)instance);
            }
        }

        #region Handlers

        /// <summary>
        /// RepoDb.Core creates every ad-hoc <see cref="DuckDBParameter"/> with its <c>ParameterName</c>
        /// already carrying the provider's placeholder sigil (e.g. "$TableName"), matching the ADO.NET
        /// convention used by SqlClient/Npgsql/MySqlConnector where the sigil is literally part of
        /// <c>ParameterName</c>. DuckDB.NET does not follow that convention: it requires the bare name
        /// (matching what follows '$' in the command text) and fails to bind (or silently mismatches) a
        /// parameter whose <c>ParameterName</c> still has the sigil attached. Strip it here so every
        /// parameter created through the core library binds correctly, regardless of call path.
        /// </summary>
        /// <param name="parameter"></param>
        private void HandleDbParameterPostCreation(DuckDBParameter parameter)
        {
            var prefix = m_dbSetting.ParameterPrefix;
            if (!string.IsNullOrEmpty(prefix) &&
                !string.IsNullOrEmpty(parameter?.ParameterName) &&
                parameter.ParameterName.StartsWith(prefix, StringComparison.Ordinal))
            {
                parameter.ParameterName = parameter.ParameterName.Substring(prefix.Length);
            }
        }

        #endregion

        #endregion

        #endregion
    }
}
