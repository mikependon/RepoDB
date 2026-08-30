#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Vertica.Data.VerticaClient;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Resolvers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb.DbHelpers
{
    /// <summary>
    /// A helper class for database specially for the direct access. This class is only meant for Vertica.
    /// </summary>
    /// <remarks>
    /// Column metadata is read from Vertica's own <c>v_catalog.columns</c>/<c>v_catalog.primary_keys</c>
    /// system tables - Vertica has no relation to Firebird's <c>RDB$</c> catalog.
    /// </remarks>
    public sealed class VerticaDbHelper : IDbHelper
    {
        /// <summary>
        /// Creates a new instance of <see cref="VerticaDbHelper"/> class.
        /// </summary>
        public VerticaDbHelper()
            : this(new VerticaDbTypeNameToClientTypeResolver())
        { }

        /// <summary>
        /// Creates a new instance of <see cref="VerticaDbHelper"/> class.
        /// </summary>
        /// <param name="dbTypeResolver">The type resolver to be used.</param>
        public VerticaDbHelper(IResolver<string, Type> dbTypeResolver)
        {
            DbTypeResolver = dbTypeResolver;
        }

        #region Properties

        /// <summary>
        /// Gets the type resolver used by this <see cref="VerticaDbHelper"/> instance.
        /// </summary>
        public IResolver<string, Type> DbTypeResolver { get; }

        #endregion

        #region Helpers

        /// <summary>
        /// Builds the query against Vertica's <c>v_catalog.columns</c>/<c>v_catalog.primary_keys</c> system
        /// tables - the real Vertica equivalent of Firebird's <c>RDB$RELATION_FIELDS</c>/<c>RDB$FIELDS</c>.
        /// </summary>
        /// <returns></returns>
        private string GetCommandText()
        {
            // IsIdentity/IsNullable are selected straight from v_catalog.columns' own genuinely-boolean
            // columns; IsPrimary/HasDefaultValue are derived, so they use TRUE/FALSE (not 1/0) to keep
            // the result column's server-side type a real BOOLEAN rather than INTEGER - GetFieldValueAsync<bool>
            // does a strict type check (unlike the sync GetBoolean(), which coerces), so an INTEGER
            // result here throws InvalidCastException on the async path only.
            return @"SELECT c.column_name AS ColumnName
                , CASE WHEN pk.column_name IS NOT NULL THEN TRUE ELSE FALSE END AS IsPrimary
                , c.is_identity AS IsIdentity
                , c.is_nullable AS IsNullable
                , c.data_type AS DataType
                , c.character_maximum_length AS ColumnSize
                , c.numeric_precision AS NumericPrecision
                , c.numeric_scale AS NumericScale
                , CASE WHEN (c.column_default IS NOT NULL AND c.column_default != '') OR c.is_identity
                    THEN TRUE ELSE FALSE END AS HasDefaultValue
            FROM v_catalog.columns c
            LEFT JOIN v_catalog.primary_keys pk
                ON pk.table_schema = c.table_schema
                AND pk.table_name = c.table_name
                AND pk.column_name = c.column_name
                AND pk.constraint_type = 'p'
            WHERE c.table_name = @TableName
            ORDER BY c.ordinal_position;";
        }

        /// <summary>
        /// Strips the <c>(size)</c>/<c>(precision,scale)</c> suffix off a raw <c>v_catalog.columns.data_type</c>
        /// value (e.g. <c>"varchar(256)"</c>, <c>"long varbinary(1000000)"</c>, <c>"numeric(18,2)"</c>) down to
        /// its base type-name keyword (e.g. <c>"varchar"</c>, <c>"long varbinary"</c>, <c>"numeric"</c>), lower-cased.
        /// </summary>
        /// <param name="rawDataType"></param>
        /// <returns></returns>
        private static string ResolveColumnTypeName(string rawDataType)
        {
            if (string.IsNullOrEmpty(rawDataType))
            {
                return "none";
            }

            var parenIndex = rawDataType.IndexOf('(');
            var baseType = (parenIndex >= 0 ? rawDataType[..parenIndex] : rawDataType).Trim();

            return baseType.ToLowerInvariant();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private DbField ReaderToDbField(DbDataReader reader)
        {
            var columnType = ResolveColumnTypeName(reader.GetString(4));

            return new DbField(reader.GetString(0),
                reader.GetBoolean(1),
                reader.GetBoolean(2),
                reader.GetBoolean(3),
                DbTypeResolver.Resolve(columnType),
                reader.IsDBNull(5) ? (int?)null : reader.GetInt32(5),
                reader.IsDBNull(6) ? (byte?)null : (byte)reader.GetInt32(6),
                reader.IsDBNull(7) ? (byte?)null : (byte)reader.GetInt32(7),
                columnType,
                reader.GetBoolean(8),
                "VERTICA");
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
            var columnType = ResolveColumnTypeName(await reader.GetFieldValueAsync<string>(4, cancellationToken));

            return new DbField(await reader.GetFieldValueAsync<string>(0, cancellationToken),
                await reader.GetFieldValueAsync<bool>(1, cancellationToken),
                await reader.GetFieldValueAsync<bool>(2, cancellationToken),
                await reader.GetFieldValueAsync<bool>(3, cancellationToken),
                DbTypeResolver.Resolve(columnType),
                await reader.IsDBNullAsync(5, cancellationToken) ? (int?)null : (int)(await reader.GetFieldValueAsync<long>(5, cancellationToken)),
                await reader.IsDBNullAsync(6, cancellationToken) ? null : (byte)(await reader.GetFieldValueAsync<long>(6, cancellationToken)),
                await reader.IsDBNullAsync(7, cancellationToken) ? null : (byte)(await reader.GetFieldValueAsync<long>(7, cancellationToken)),
                columnType,
                await reader.GetFieldValueAsync<bool>(8, cancellationToken),
                "VERTICA");
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
            var setting = connection.GetDbSetting();
            var param = new
            {
                TableName = DataEntityExtension.GetTableName(tableName, setting).AsUnquoted(setting)
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
            var setting = connection.GetDbSetting();
            var param = new
            {
                TableName = DataEntityExtension.GetTableName(tableName, setting).AsUnquoted(setting)
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
        /// Returns the newly generated identity from the database.
        /// </summary>
        /// <typeparam name="T">The type of newly generated identity.</typeparam>
        /// <param name="connection">The instance of the connection object.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <returns>The newly generated identity from the database.</returns>
        public T GetScopeIdentity<T>(IDbConnection connection,
            IDbTransaction transaction = null) =>
            connection.ExecuteScalar<T>("SELECT LAST_INSERT_ID()", transaction: transaction);

        /// <summary>
        /// Returns the newly generated identity from the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="T">The type of newly generated identity.</typeparam>
        /// <param name="connection">The instance of the connection object.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The newly generated identity from the database.</returns>
        public Task<T> GetScopeIdentityAsync<T>(IDbConnection connection,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default) =>
            connection.ExecuteScalarAsync<T>("SELECT LAST_INSERT_ID()", transaction: transaction, cancellationToken: cancellationToken);

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
                HandleDbParameterPostCreation((VerticaParameter)(object)instance);
            }
        }

        #region Handlers

        /// <summary>
        ///
        /// </summary>
        /// <param name="parameter"></param>
        private void HandleDbParameterPostCreation(VerticaParameter parameter)
        {
            // Do nothing for now
        }

        #endregion

        #endregion

        #endregion
    }
}
