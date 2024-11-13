﻿using MySqlConnector;
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
    /// A helper class for database specially for the direct access. This class is only meant for MySql.
    /// </summary>
    public sealed class MySqlConnectorDbHelper : IDbHelper
    {
        private IDbSetting m_dbSetting = DbSettingMapper.Get<MySqlConnection>();

        /// <summary>
        /// Creates a new instance of <see cref="MySqlConnectorDbHelper"/> class.
        /// </summary>
        public MySqlConnectorDbHelper()
            : this(new MySqlConnectorDbTypeNameToClientTypeResolver())
        { }

        /// <summary>
        /// Creates a new instance of <see cref="MySqlConnectorDbHelper"/> class.
        /// </summary>
        /// <param name="dbTypeResolver">The type resolver to be used.</param>
        public MySqlConnectorDbHelper(IResolver<string, Type> dbTypeResolver)
        {
            DbTypeResolver = dbTypeResolver;
        }

        #region Properties

        /// <summary>
        /// Gets the type resolver used by this <see cref="MySqlConnectorDbHelper"/> instance.
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
            return $@"SELECT COLUMN_NAME AS ColumnName
                , CASE WHEN COLUMN_KEY = 'PRI' THEN 1 ELSE 0 END AS IsPrimary
                , CASE WHEN EXTRA LIKE '%auto_increment%' THEN 1 ELSE 0 END AS IsIdentity
                , CASE WHEN IS_NULLABLE = 'YES' THEN 1 ELSE 0 END AS IsNullable
                , DATA_TYPE AS ColumnType /*COLUMN_TYPE AS ColumnType*/
                , CHARACTER_MAXIMUM_LENGTH AS Size
                , COALESCE(NUMERIC_PRECISION, DATETIME_PRECISION) AS `Precision`
                , NUMERIC_SCALE AS Scale
                , DATA_TYPE AS DatabaseType
                , CASE WHEN COLUMN_DEFAULT IS NOT NULL THEN 1 ELSE 0 END AS HasDefaultValue
            FROM INFORMATION_SCHEMA.COLUMNS
            WHERE TABLE_SCHEMA = @TableSchema
                AND TABLE_NAME = @TableName
            ORDER BY ORDINAL_POSITION;";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private HashSet<string> GetBlobTypes()
        {
            return new()
            {
                "blob",
                "blobasarray",
                "binary",
                "longtext",
                "mediumtext",
                "longblob",
                "mediumblob",
                "tinyblob",
                "varbinary"
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
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
                "MYSQLC");
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
                "MYSQLC");
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
            IDbTransaction? transaction = null)
        {
            // Variables
            var commandText = GetCommandText();
            var param = new
            {
                TableSchema = connection.Database,
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
            IDbTransaction? transaction = null,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Variables
            var commandText = GetCommandText();
            var param = new
            {
                TableSchema = connection.Database,
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
            IDbTransaction? transaction = null)
        {
            return connection.ExecuteScalar<T>("SELECT LAST_INSERT_ID();", transaction: transaction);
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
            IDbTransaction? transaction = null,
            CancellationToken cancellationToken = default)
        {
            return connection.ExecuteScalarAsync<T>("SELECT LAST_INSERT_ID();", transaction: transaction,
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
                HandleDbParameterPostCreation((MySqlParameter)(object)instance);
            }
        }

        #region Handlers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        private void HandleDbParameterPostCreation(MySqlParameter parameter)
        {
            // Do nothing for now
        }

        #endregion

        #endregion

        #endregion
    }
}
