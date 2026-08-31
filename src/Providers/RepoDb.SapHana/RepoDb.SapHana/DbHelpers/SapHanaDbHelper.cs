#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Sap.Data.Hana;
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
    /// A helper class for database specially for the direct access. This class is only meant for SAP HANA.
    /// </summary>
    public sealed class SapHanaDbHelper : IDbHelper
    {
        private IDbSetting m_dbSetting = DbSettingMapper.Get<HanaConnection>();

        /// <summary>
        /// Creates a new instance of <see cref="SapHanaDbHelper"/> class.
        /// </summary>
        public SapHanaDbHelper()
            : this(new SapHanaDbTypeNameToClientTypeResolver())
        { }

        /// <summary>
        /// Creates a new instance of <see cref="SapHanaDbHelper"/> class.
        /// </summary>
        /// <param name="dbTypeResolver">The type resolver to be used.</param>
        public SapHanaDbHelper(IResolver<string, Type> dbTypeResolver)
        {
            DbTypeResolver = dbTypeResolver;
        }

        #region Properties

        /// <summary>
        /// Gets the type resolver used by this <see cref="SapHanaDbHelper"/> instance.
        /// </summary>
        public IResolver<string, Type> DbTypeResolver { get; }

        #endregion

        #region Helpers

        /// <summary>
        ///
        /// </summary>
        private string GetCommandText()
        {
            return @"SELECT C.COLUMN_NAME AS ColumnName
                , CAST(CASE WHEN PK.IS_PRIMARY_KEY = 'TRUE' THEN 1 ELSE 0 END AS BOOLEAN) AS IsPrimary
                , CAST(CASE WHEN C.GENERATED_ALWAYS_AS IS NOT NULL THEN 1 ELSE 0 END AS BOOLEAN) AS IsIdentity
                , CAST(CASE WHEN C.IS_NULLABLE = 'TRUE' THEN 1 ELSE 0 END AS BOOLEAN) AS IsNullable
                , C.DATA_TYPE_NAME AS ColumnType
                , C.LENGTH AS Size
                , CASE WHEN C.DATA_TYPE_NAME IN ('DECIMAL', 'SMALLDECIMAL') THEN C.LENGTH ELSE NULL END AS ColumnPrecision
                , CASE WHEN C.DATA_TYPE_NAME IN ('DECIMAL', 'SMALLDECIMAL') THEN C.SCALE ELSE NULL END AS Scale
                , C.DATA_TYPE_NAME AS DatabaseType
                , CAST(CASE WHEN C.DEFAULT_VALUE IS NOT NULL THEN 1 ELSE 0 END AS BOOLEAN) AS HasDefaultValue
            FROM SYS.TABLE_COLUMNS C
            LEFT JOIN SYS.CONSTRAINTS PK
                ON PK.SCHEMA_NAME = C.SCHEMA_NAME
                AND PK.TABLE_NAME = C.TABLE_NAME
                AND PK.COLUMN_NAME = C.COLUMN_NAME
                AND PK.IS_PRIMARY_KEY = 'TRUE'
            WHERE C.SCHEMA_NAME = :TableSchema
                AND C.TABLE_NAME = :TableName
            ORDER BY C.POSITION";
        }

        /// <summary>
        ///
        /// </summary>
        private HashSet<string> GetBlobTypes()
        {
            return new()
            {
                "blob",
                "clob",
                "nclob",
                "text",
                "bintext",
                "varbinary"
            };
        }

        /// <summary>
        ///
        /// </summary>
        private DbField ReaderToDbField(DbDataReader reader)
        {
            var columnType = reader.GetString(4);
            var excluded = GetBlobTypes();
            var size = (int?)null;
            if (excluded.Contains(columnType.ToLowerInvariant()))
            {
                size = null;
            }
            else
            {
                size = reader.IsDBNull(5) ? (int?)null : reader.GetInt32(5);
            }
            return new DbField(reader.GetString(0),
                reader.GetBoolean(1),
                reader.GetBoolean(2),
                reader.GetBoolean(3),
                DbTypeResolver.Resolve(columnType),
                size,
                reader.IsDBNull(6) ? (byte?)null : byte.Parse(reader.GetInt32(6).ToString()),
                reader.IsDBNull(7) ? (byte?)null : byte.Parse(reader.GetInt32(7).ToString()),
                reader.GetString(8),
                reader.GetBoolean(9),
                "HANA");
        }

        /// <summary>
        ///
        /// </summary>
        private async Task<DbField> ReaderToDbFieldAsync(DbDataReader reader,
            CancellationToken cancellationToken = default)
        {
            var columnType = await reader.GetFieldValueAsync<string>(4, cancellationToken);
            var excluded = GetBlobTypes();
            int? size;
            if (excluded.Contains(columnType.ToLowerInvariant()))
            {
                size = null;
            }
            else
            {
                size = await reader.IsDBNullAsync(5, cancellationToken) ? (int?)null :
                    await reader.GetFieldValueAsync<int>(5, cancellationToken);
            }
            return new DbField(await reader.GetFieldValueAsync<string>(0, cancellationToken),
                await reader.GetFieldValueAsync<bool>(1, cancellationToken),
                await reader.GetFieldValueAsync<bool>(2, cancellationToken),
                await reader.GetFieldValueAsync<bool>(3, cancellationToken),
                DbTypeResolver.Resolve(columnType),
                size,
                await reader.IsDBNullAsync(6, cancellationToken) ? null : byte.Parse((await reader.GetFieldValueAsync<int>(6, cancellationToken)).ToString()),
                await reader.IsDBNullAsync(7, cancellationToken) ? null : byte.Parse((await reader.GetFieldValueAsync<int>(7, cancellationToken)).ToString()),
                await reader.GetFieldValueAsync<string>(8, cancellationToken),
                await reader.GetFieldValueAsync<bool>(9, cancellationToken),
                "HANA");
        }

        #endregion

        #region Methods

        #region GetFields

        /// <summary>
        /// Gets the list of <see cref="DbField"/> of the table.
        /// </summary>
        public IEnumerable<DbField> GetFields(IDbConnection connection,
            string tableName,
            IDbTransaction transaction = null)
        {
            var commandText = GetCommandText();
            var param = new
            {
                TableSchema = connection.ExecuteScalar<string>("SELECT CURRENT_SCHEMA FROM DUMMY;", transaction: transaction),
                TableName = DataEntityExtension.GetTableName(tableName, m_dbSetting).AsUnquoted(m_dbSetting)
            };

            using var reader = (DbDataReader)connection.ExecuteReader(commandText, param, transaction: transaction);

            var dbFields = new List<DbField>();

            while (reader.Read())
            {
                dbFields.Add(ReaderToDbField(reader));
            }

            return dbFields;
        }

        /// <summary>
        /// Gets the list of <see cref="DbField"/> of the table in an asynchronous way.
        /// </summary>
        public async Task<IEnumerable<DbField>> GetFieldsAsync(IDbConnection connection,
            string tableName,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var commandText = GetCommandText();
            var param = new
            {
                TableSchema = await connection.ExecuteScalarAsync<string>("SELECT CURRENT_SCHEMA FROM DUMMY;", transaction: transaction,
                    cancellationToken: cancellationToken),
                TableName = DataEntityExtension.GetTableName(tableName, m_dbSetting).AsUnquoted(m_dbSetting)
            };

            using var reader = (DbDataReader)await connection.ExecuteReaderAsync(commandText, param, transaction: transaction,
                cancellationToken: cancellationToken);

            var dbFields = new List<DbField>();

            while (await reader.ReadAsync(cancellationToken))
            {
                dbFields.Add(await ReaderToDbFieldAsync(reader, cancellationToken));
            }

            return dbFields;
        }

        #endregion

        #region GetScopeIdentity

        /// <summary>
        /// Gets the newly generated identity from the database.
        /// </summary>
        public T GetScopeIdentity<T>(IDbConnection connection,
            IDbTransaction transaction = null)
        {
            return connection.ExecuteScalar<T>("SELECT CURRENT_IDENTITY_VALUE() FROM DUMMY;", transaction: transaction);
        }

        /// <summary>
        /// Gets the newly generated identity from the database in an asynchronous way.
        /// </summary>
        public Task<T> GetScopeIdentityAsync<T>(IDbConnection connection,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return connection.ExecuteScalarAsync<T>("SELECT CURRENT_IDENTITY_VALUE() FROM DUMMY;", transaction: transaction,
                cancellationToken: cancellationToken);
        }

        #endregion

        #region DynamicHandler

        /// <summary>
        /// A backdoor access from the core library used to handle an instance of an object to whatever purpose within the extended library.
        /// </summary>
        public void DynamicHandler<TEventInstance>(TEventInstance instance,
            string key)
        {
            if (key == "RepoDb.Internal.Compiler.Events[AfterCreateDbParameter]")
            {
                HandleDbParameterPostCreation((HanaParameter)(object)instance);
            }
        }

        #region Handlers

        /// <summary>
        ///
        /// </summary>
        private void HandleDbParameterPostCreation(HanaParameter parameter)
        {
            // Do nothing for now
        }

        #endregion

        #endregion

        #endregion
    }
}
