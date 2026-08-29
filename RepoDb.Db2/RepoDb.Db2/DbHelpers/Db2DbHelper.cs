#region Copyright Attributions

// Copyright (c) 2019 Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using IBM.Data.Db2;
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
    /// A helper class for database specially for the direct access. This class is only meant for Db2.
    /// </summary>
    public sealed class Db2DbHelper : IDbHelper
    {
        /// <summary>
        /// Creates a new instance of <see cref="Db2DbHelper"/> class.
        /// </summary>
        public Db2DbHelper()
            : this(new Db2DbTypeNameToClientTypeResolver())
        { }

        /// <summary>
        /// Creates a new instance of <see cref="Db2DbHelper"/> class.
        /// </summary>
        /// <param name="dbTypeResolver">The type resolver to be used.</param>
        public Db2DbHelper(IResolver<string, Type> dbTypeResolver)
        {
            DbTypeResolver = dbTypeResolver;
        }

        #region Properties

        /// <summary>
        /// Gets the type resolver used by this <see cref="Db2DbHelper"/> instance.
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
            return @"
                SELECT COLNAME AS ColumnName
                    , CASE WHEN KEYSEQ > 0 THEN 1 ELSE 0 END AS IsPrimary
                    , CASE WHEN IDENTITY = 'Y' THEN 1 ELSE 0 END AS IsIdentity
                    , CASE WHEN NULLS = 'Y' THEN 1 ELSE 0 END AS IsNullable
                    , TYPENAME AS DataType
                    , LENGTH AS ColumnSize
                    , CASE WHEN TYPENAME IN ('DECIMAL', 'NUMERIC') THEN LENGTH ELSE NULL END AS Precision
                    , SCALE AS Scale
                    , CASE WHEN ""DEFAULT"" IS NOT NULL THEN 1 ELSE 0 END AS HasDefaultValue
                FROM SYSCAT.COLUMNS
                WHERE TABSCHEMA = COALESCE(:Schema, CURRENT SCHEMA)
                    AND TABNAME = :TableName
                ORDER BY COLNO";
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private DbField ReaderToDbField(DbDataReader reader)
        {
            return new DbField(reader.GetString(0),
                !reader.IsDBNull(1) && Convert.ToInt32(reader.GetValue(1)) == 1,
                !reader.IsDBNull(2) && Convert.ToInt32(reader.GetValue(2)) == 1,
                !reader.IsDBNull(3) && Convert.ToInt32(reader.GetValue(3)) == 1,
                reader.IsDBNull(4) ? DbTypeResolver.Resolve("varchar") : DbTypeResolver.Resolve(reader.GetString(4)),
                reader.IsDBNull(5) ? 0 : Convert.ToInt32(reader.GetValue(5)),
                reader.IsDBNull(6) ? (byte?)0 : Convert.ToByte(reader.GetValue(6)),
                reader.IsDBNull(7) ? (byte?)0 : Convert.ToByte(reader.GetValue(7)),
                reader.IsDBNull(4) ? "varchar" : reader.GetString(4),
                !reader.IsDBNull(8) && Convert.ToInt32(reader.GetValue(8)) == 1,
                "DB2");
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
                !await reader.IsDBNullAsync(1, cancellationToken) && Convert.ToInt32(await reader.GetFieldValueAsync<object>(1, cancellationToken)) == 1,
                !await reader.IsDBNullAsync(2, cancellationToken) && Convert.ToInt32(await reader.GetFieldValueAsync<object>(2, cancellationToken)) == 1,
                !await reader.IsDBNullAsync(3, cancellationToken) && Convert.ToInt32(await reader.GetFieldValueAsync<object>(3, cancellationToken)) == 1,
                await reader.IsDBNullAsync(4, cancellationToken) ? DbTypeResolver.Resolve("varchar") : DbTypeResolver.Resolve(await reader.GetFieldValueAsync<string>(4, cancellationToken)),
                await reader.IsDBNullAsync(5, cancellationToken) ? 0 : Convert.ToInt32(await reader.GetFieldValueAsync<object>(5, cancellationToken)),
                await reader.IsDBNullAsync(6, cancellationToken) ? (byte?)0 : Convert.ToByte(await reader.GetFieldValueAsync<object>(6, cancellationToken)),
                await reader.IsDBNullAsync(7, cancellationToken) ? (byte?)0 : Convert.ToByte(await reader.GetFieldValueAsync<object>(7, cancellationToken)),
                await reader.IsDBNullAsync(4, cancellationToken) ? "varchar" : await reader.GetFieldValueAsync<string>(4, cancellationToken),
                !await reader.IsDBNullAsync(8, cancellationToken) && Convert.ToInt32(await reader.GetFieldValueAsync<object>(8, cancellationToken)) == 1,
                "DB2");
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
                Schema = DataEntityExtension.GetSchema(tableName, setting)?.AsUnquoted(setting),
                TableName = DataEntityExtension.GetTableName(tableName, setting)?.AsUnquoted(setting)
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
            // Variables
            var commandText = GetCommandText();
            var setting = connection.GetDbSetting();
            var param = new
            {
                Schema = DataEntityExtension.GetSchema(tableName, setting)?.AsUnquoted(setting),
                TableName = DataEntityExtension.GetTableName(tableName, setting)?.AsUnquoted(setting)
            };

            // Iterate and extract
            using var reader = (DbDataReader)await connection.ExecuteReaderAsync(commandText, param,
                transaction: transaction, cancellationToken: cancellationToken);

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
        /// Gets the newly generated identity from the database. Db2's session-wide "last identity"
        /// construct - the equivalent of SQL Server's SCOPE_IDENTITY() - is the built-in
        /// <c>IDENTITY_VAL_LOCAL()</c> function, which returns the most recently generated identity
        /// value for the current connection, regardless of table. <c>SYSIBM.SYSDUMMY1</c> is Db2's
        /// single-row dummy table, needed here because Db2 has no scalar function call without FROM
        /// (the equivalent of Oracle's DUAL).
        /// </summary>
        /// <typeparam name="T">The type of newly generated identity.</typeparam>
        /// <param name="connection">The instance of the connection object.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <returns>The newly generated identity from the database.</returns>
        public T GetScopeIdentity<T>(IDbConnection connection,
            IDbTransaction transaction = null) =>
            connection.ExecuteScalar<T>("SELECT IDENTITY_VAL_LOCAL() FROM SYSIBM.SYSDUMMY1;",
                transaction: transaction);

        /// <summary>
        /// Gets the newly generated identity from the database in an asynchronous way. See
        /// <see cref="GetScopeIdentity{T}(IDbConnection, IDbTransaction)"/> for why
        /// <c>IDENTITY_VAL_LOCAL()</c>/<c>SYSIBM.SYSDUMMY1</c> is used.
        /// </summary>
        /// <typeparam name="T">The type of newly generated identity.</typeparam>
        /// <param name="connection">The instance of the connection object.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The newly generated identity from the database.</returns>
        public Task<T> GetScopeIdentityAsync<T>(IDbConnection connection,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default) =>
            connection.ExecuteScalarAsync<T>("SELECT IDENTITY_VAL_LOCAL() FROM SYSIBM.SYSDUMMY1;",
                transaction: transaction,
                cancellationToken: cancellationToken);

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
                HandleDbParameterPostCreation((DB2Parameter)(object)instance);
            }
        }

        #region Handlers

        /// <summary>
        ///
        /// </summary>
        /// <param name="parameter"></param>
        private void HandleDbParameterPostCreation(DB2Parameter parameter)
        {
            // Do nothing for now
        }

        #endregion

        #endregion

        #endregion
    }
}
