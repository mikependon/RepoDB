#region Copyright Attributions

// Copyright (c) 2019 Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.Data.SqlClient;
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
    /// A helper class for database specially for the direct access. This class is only meant for SQL Server.
    /// </summary>
    /// <remarks>
    /// Creates a new instance of <see cref="SqlServerDbHelper"/> class.
    /// </remarks>
    /// <param name="dbTypeResolver">The type resolver to be used.</param>
    public sealed class SqlServerDbHelper(IResolver<string, Type> dbTypeResolver) : IDbHelper
    {
        /// <summary>
        /// Creates a new instance of <see cref="SqlServerDbHelper"/> class.
        /// </summary>
        public SqlServerDbHelper()
            : this(new SqlServerDbTypeNameToClientTypeResolver())
        { }

        #region Properties

        /// <summary>
        /// Gets the type resolver used by this <see cref="SqlServerDbHelper"/> instance.
        /// </summary>
        public IResolver<string, Type> DbTypeResolver { get; } = dbTypeResolver;

        #endregion

        #region Helpers

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        private string GetCommandText()
        {
            return @"
                SELECT C.COLUMN_NAME AS ColumnName
                    , CONVERT(BIT, COALESCE(TC.is_primary, 0)) AS IsPrimary
                    , CONVERT(BIT, COALESCE(TMP.is_identity, 1)) AS IsIdentity
                    , CONVERT(BIT, COALESCE(TMP.is_nullable, 1)) AS IsNullable
                    , C.DATA_TYPE AS DataType
                    , CASE WHEN TMP.max_length > COALESCE(C.CHARACTER_MAXIMUM_LENGTH, TMP.max_length) THEN
                        TMP.max_length
                      ELSE
                        COALESCE(C.CHARACTER_MAXIMUM_LENGTH, TMP.max_length)
                      END AS Size
                    , CONVERT(TINYINT, COALESCE(TMP.precision, 1)) AS Precision
                    , CONVERT(TINYINT, COALESCE(TMP.scale, 1)) AS Scale
                    , CONVERT(BIT, IIF(C.COLUMN_DEFAULT IS NOT NULL, 1, 0)) AS DefaultValue
                FROM INFORMATION_SCHEMA.COLUMNS C
                OUTER APPLY
                (
                    SELECT 1 AS is_primary
                    FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE KCU
                    LEFT JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS TC
                        ON TC.TABLE_SCHEMA = C.TABLE_SCHEMA
                        AND TC.TABLE_NAME = C.TABLE_NAME
                        AND TC.CONSTRAINT_NAME = KCU.CONSTRAINT_NAME
                    WHERE KCU.TABLE_SCHEMA = C.TABLE_SCHEMA
                        AND KCU.TABLE_NAME = C.TABLE_NAME
                        AND KCU.COLUMN_NAME = C.COLUMN_NAME
                        AND TC.CONSTRAINT_TYPE = 'PRIMARY KEY'
                ) TC 
                OUTER APPLY
                (
                    SELECT SC.name
                        , SC.is_identity
                        , SC.is_nullable
                        , SC.max_length
                        , SC.scale
                        , SC.precision
                    FROM [sys].[columns] SC
                    INNER JOIN [sys].[tables] ST ON ST.object_id = SC.object_id
                    INNER JOIN [sys].[schemas] S ON S.schema_id = ST.schema_id
                    WHERE SC.name = C.COLUMN_NAME
                        AND ST.name = C.TABLE_NAME
                        AND S.name = C.TABLE_SCHEMA
                ) TMP
                WHERE
                    C.TABLE_SCHEMA = @Schema
                    AND C.TABLE_NAME = @TableName;";
        }

        /// <summary>
        /// Gets the command text used to extract the fields of a local (<c>#</c>) or global (<c>##</c>) temporary table.
        /// </summary>
        /// <returns></returns>
        private string GetTempTableCommandText()
        {
            return @"
                SELECT C.name AS ColumnName
                    , CONVERT(BIT, COALESCE(TC.is_primary, 0)) AS IsPrimary
                    , CONVERT(BIT, C.is_identity) AS IsIdentity
                    , CONVERT(BIT, C.is_nullable) AS IsNullable
                    , TP.name AS DataType
                    , CONVERT(INT, C.max_length) AS Size
                    , CONVERT(TINYINT, C.precision) AS Precision
                    , CONVERT(TINYINT, C.scale) AS Scale
                    , CONVERT(BIT, IIF(C.default_object_id <> 0, 1, 0)) AS DefaultValue
                FROM tempdb.sys.columns C
                INNER JOIN tempdb.sys.types TP
                    ON TP.user_type_id = C.user_type_id
                OUTER APPLY
                (
                    SELECT 1 AS is_primary
                    FROM tempdb.sys.index_columns IC
                    INNER JOIN tempdb.sys.indexes I
                        ON I.object_id = IC.object_id
                        AND I.index_id = IC.index_id
                        AND I.is_primary_key = 1
                    WHERE IC.object_id = C.object_id
                        AND IC.column_id = C.column_id
                ) TC
                WHERE C.object_id = OBJECT_ID(N'tempdb..' + @TableName)
                ORDER BY C.column_id;";
        }

        /// <summary>
        /// Checks whether the given (already unquoted) table name refers to a local (<c>#</c>) or global (<c>##</c>)
        /// temporary table.
        /// </summary>
        /// <param name="unquotedTableName">The unquoted table name.</param>
        /// <returns><c>true</c> if the table name refers to a temporary table.</returns>
        private static bool IsTempTable(string unquotedTableName) =>
            !string.IsNullOrEmpty(unquotedTableName) && unquotedTableName[0] == '#';

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
                reader.IsDBNull(5) ? 0 : reader.GetInt32(5),
                reader.IsDBNull(6) ? (byte?)0 : reader.GetByte(6),
                reader.IsDBNull(7) ? (byte?)0 : reader.GetByte(7),
                reader.IsDBNull(7) ? "text" : reader.GetString(4),
                !reader.IsDBNull(8) && reader.GetBoolean(8),
                "MSSQL");
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
                await reader.IsDBNullAsync(5, cancellationToken).ConfigureAwait(false) ? 0 : await reader.GetFieldValueAsync<int>(5, cancellationToken).ConfigureAwait(false),
                await reader.IsDBNullAsync(6, cancellationToken).ConfigureAwait(false) ? (byte?)0 : await reader.GetFieldValueAsync<byte>(6, cancellationToken).ConfigureAwait(false),
                await reader.IsDBNullAsync(7, cancellationToken).ConfigureAwait(false) ? (byte?)0 : await reader.GetFieldValueAsync<byte>(7, cancellationToken).ConfigureAwait(false),
                await reader.IsDBNullAsync(7, cancellationToken).ConfigureAwait(false) ? "text" : await reader.GetFieldValueAsync<string>(4, cancellationToken).ConfigureAwait(false),
                !await reader.IsDBNullAsync(8, cancellationToken).ConfigureAwait(false) && await reader.GetFieldValueAsync<bool>(8, cancellationToken).ConfigureAwait(false),
                "MSSQL");
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
            var setting = connection.GetDbSetting();
            var unquotedTableName = DataEntityExtension.GetTableName(tableName, setting).AsUnquoted(setting);
            var isTempTable = IsTempTable(unquotedTableName);
            var commandText = isTempTable ? GetTempTableCommandText() : GetCommandText();
            object param = isTempTable
                ? new { TableName = unquotedTableName }
                : new
                {
                    Schema = DataEntityExtension.GetSchema(tableName, setting).AsUnquoted(setting),
                    TableName = unquotedTableName
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
            var setting = connection.GetDbSetting();
            var unquotedTableName = DataEntityExtension.GetTableName(tableName, setting).AsUnquoted(setting);
            var isTempTable = IsTempTable(unquotedTableName);
            var commandText = isTempTable ? GetTempTableCommandText() : GetCommandText();
            object param = isTempTable
                ? new { TableName = unquotedTableName }
                : new
                {
                    Schema = DataEntityExtension.GetSchema(tableName, setting).AsUnquoted(setting),
                    TableName = unquotedTableName
                };

            // Iterate and extract
            using var reader = (DbDataReader)await connection.ExecuteReaderAsync(commandText, param,
                transaction: transaction, cancellationToken: cancellationToken).ConfigureAwait(false);

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
        {
            return connection.ExecuteScalar<T>("SELECT COALESCE(SCOPE_IDENTITY(), @@IDENTITY);",
                transaction: transaction);
        }

        /// <summary>
        /// Gets the newly generated identity from the database in an asynchronous way.
        /// </summary>
        /// <typeparam name="T">The type of newly generated identity.</typeparam>
        /// <param name="connection">The instance of the connection object.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The newly generated identity from the database.</returns>
        public async Task<T> GetScopeIdentityAsync<T>(IDbConnection connection,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return await connection.ExecuteScalarAsync<T>("SELECT COALESCE(SCOPE_IDENTITY(), @@IDENTITY);",
                transaction: transaction,
                cancellationToken: cancellationToken).ConfigureAwait(false);
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
                HandleDbParameterPostCreation((SqlParameter)(object)instance);
            }
        }

        #region Handlers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        private void HandleDbParameterPostCreation(SqlParameter parameter)
        {
            // Do nothing for now
        }

        #endregion

        #endregion

        #endregion
    }
}
