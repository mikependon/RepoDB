﻿using RepoDb.Extensions;
using RepoDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb.DbHelpers
{
    /// <summary>
    /// A helper class for database specially for the direct access. This class is only meant for SqLite.
    /// </summary>
    public sealed class SqLiteDbHelper : IDbHelper
    {
        private const string doubleQuote = "\"";

        /// <summary>
        /// Creates a new instance of <see cref="SqLiteDbHelper"/> class.
        /// </summary>
        /// <param name="dbTypeResolver">The type resolver to be used.</param>
        /// <param name="dbSetting">The instance of the <see cref="IDbSetting"/> object to be used.</param>
        public SqLiteDbHelper(IDbSetting dbSetting,
            IResolver<string, Type> dbTypeResolver)
        {
            DbSetting = dbSetting;
            DbTypeResolver = dbTypeResolver;
        }

        #region Properties

        /// <summary>
        /// Gets the database setting used by this <see cref="SqLiteDbHelper"/> instance.
        /// </summary>
        public IDbSetting DbSetting { get; }

        /// <summary>
        /// Gets the type resolver used by this <see cref="SqLiteDbHelper"/> instance.
        /// </summary>
        public IResolver<string, Type> DbTypeResolver { get; }

        #endregion

        #region Helpers

        /// <summary>
        ///
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private string GetCommandText(string tableName) =>
            $"pragma table_info({DataEntityExtension.GetTableName(tableName, DbSetting).AsUnquoted(DbSetting)});";

        /// <summary>
        ///
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="identityFieldName"></param>
        /// <returns></returns>
        private DbField ReaderToDbField(DbDataReader reader,
            string identityFieldName)
        {
            return new DbField(reader.GetString(1),
                !reader.IsDBNull(5) && reader.GetBoolean(5),
                string.Equals(reader.GetString(1), identityFieldName, StringComparison.OrdinalIgnoreCase),
                reader.IsDBNull(3) || reader.GetBoolean(3) == false,
                reader.IsDBNull(2) ? DbTypeResolver.Resolve("text") : DbTypeResolver.Resolve(reader.GetString(2)),
                null,
                null,
                null,
                null,
                !reader.IsDBNull(4),
                "SYSSQLITE");
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="identityFieldName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DbField> ReaderToDbFieldAsync(DbDataReader reader,
            string identityFieldName,
            CancellationToken cancellationToken = default)
        {
            return new DbField(await reader.GetFieldValueAsync<string>(1, cancellationToken),
                !await reader.IsDBNullAsync(5, cancellationToken) && Convert.ToBoolean(await reader.GetFieldValueAsync<long>(5, cancellationToken)),
                string.Equals(await reader.GetFieldValueAsync<string>(1, cancellationToken), identityFieldName, StringComparison.OrdinalIgnoreCase),
                await reader.IsDBNullAsync(3, cancellationToken) || Convert.ToBoolean(await reader.GetFieldValueAsync<long>(3, cancellationToken)) == false,
                await reader.IsDBNullAsync(2, cancellationToken) ? DbTypeResolver.Resolve("text") : DbTypeResolver.Resolve(await reader.GetFieldValueAsync<string>(2, cancellationToken)),
                null,
                null,
                null,
                null,
                !await reader.IsDBNullAsync(4, cancellationToken),
                "SYSSQLITE");
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TDbConnection"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private string GetIdentityFieldName<TDbConnection>(TDbConnection connection,
            string tableName,
            IDbTransaction? transaction = null)
            where TDbConnection : IDbConnection
        {
            // Sql text
            var commandText = "SELECT sql FROM [sqlite_master] WHERE name = @TableName AND type = 'table';";
            var sql = connection.ExecuteScalar<string>(commandText: commandText,
                param: new { TableName = DataEntityExtension.GetTableName(tableName, DbSetting).AsUnquoted(DbSetting) },
                transaction: transaction);

            // Return
            return GetIdentityFieldNameInternal(sql)?
                .AsUnquoted(connection.GetDbSetting())?
                .Replace(doubleQuote, string.Empty);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TDbConnection"></typeparam>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<string> GetIdentityFieldNameAsync<TDbConnection>(TDbConnection connection,
            string tableName,
            IDbTransaction? transaction = null,
            CancellationToken cancellationToken = default)
            where TDbConnection : IDbConnection
        {
            // Sql text
            var commandText = "SELECT sql FROM [sqlite_master] WHERE name = @TableName AND type = 'table';";
            var sql = await connection.ExecuteScalarAsync<string>(commandText: commandText,
                param: new { TableName = DataEntityExtension.GetTableName(tableName, DbSetting).AsUnquoted(DbSetting) },
                transaction: transaction,
                cancellationToken: cancellationToken);

            // Return
            return GetIdentityFieldNameInternal(sql)?
                .AsUnquoted(connection.GetDbSetting())?
                .Replace(doubleQuote, string.Empty);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        private string GetIdentityFieldNameInternal(string sql)
        {
            // Sql text
            var fields = ParseTableFieldsFromSql(sql);

            // Iterate the fields
            if (fields?.Length > 0)
            {
                foreach (var field in fields)
                {
                    if (IsIdentity(field))
                    {
                        var fieldName = field;

                        // This happens if the table has been created with the PRIMARY KEY keyword
                        // defined at the end of schema. See issue #802
                        if (field.StartsWith("PRIMARY KEY", StringComparison.OrdinalIgnoreCase))
                        {
                            fieldName = fieldName
                                .Replace("PRIMARY KEY", string.Empty)
                                .Trim()
                                .Replace("(", string.Empty);
                        }

                        // Return
                        return fieldName.Substring(0, fieldName.IndexOf(' '));
                    }
                }
            }

            // Return null
            return null;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        private bool IsIdentity(string field)
        {
            return field.Contains("AUTOINCREMENT", StringComparison.OrdinalIgnoreCase) ||
                   (field.Contains("INTEGER", StringComparison.OrdinalIgnoreCase)
                    && field.Contains("PRIMARY KEY", StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        private string[] ParseTableFieldsFromSql(string sql)
        {
            if (string.IsNullOrEmpty(sql))
            {
                return null;
            }

            // Do parse
            var openingTokenIndex = sql.IndexOf('(');
            var closingTokenIndex = sql.IndexOf(')');
            var parsed = sql.Substring((openingTokenIndex + 1), (closingTokenIndex - (openingTokenIndex + 1)));

            // Simply split by comma
            return parsed
                .Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .AsArray();
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
            var commandText = GetCommandText(tableName);

            // Iterate and extract
            using var reader = (DbDataReader)connection.ExecuteReader(commandText, transaction: transaction);

            var dbFields = new List<DbField>();
            var identity = GetIdentityFieldName(connection, tableName, transaction);

            // Iterate the list of the fields
            while (reader.Read())
            {
                dbFields.Add(ReaderToDbField(reader, identity));
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
            // Variables
            var commandText = GetCommandText(tableName);

            // Iterate and extract
            using var reader = (DbDataReader)await connection.ExecuteReaderAsync(commandText, transaction: transaction, cancellationToken: cancellationToken);

            var dbFields = new List<DbField>();
            var identity = await GetIdentityFieldNameAsync(connection, tableName, transaction, cancellationToken);

            // Iterate the list of the fields
            while (await reader.ReadAsync(cancellationToken))
            {
                dbFields.Add(await ReaderToDbFieldAsync(reader, identity, cancellationToken));
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
            return connection.ExecuteScalar<T>("SELECT last_insert_rowid();", transaction: transaction);
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
            return connection.ExecuteScalarAsync<T>("SELECT last_insert_rowid();", transaction: transaction,
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
                HandleDbParameterPostCreation((SQLiteParameter)(object)instance);
            }
        }

        #region Handlers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        private void HandleDbParameterPostCreation(SQLiteParameter parameter)
        {
            // Do nothing for now
        }

        #endregion

        #endregion

        #endregion
    }
}
