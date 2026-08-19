using ClickHouse.Driver.ADO;
using ClickHouse.Driver.ADO.Parameters;
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
    /// A helper class for database specially for the direct access. This class is only meant for ClickHouse.
    /// </summary>
    public sealed class ClickHouseDbHelper : IDbHelper
    {
        private IDbSetting m_dbSetting = DbSettingMapper.Get<RepoDbClickHouseConnection>();

        /// <summary>
        /// Creates a new instance of <see cref="ClickHouseDbHelper"/> class.
        /// </summary>
        public ClickHouseDbHelper()
            : this(new ClickHouseDbTypeNameToClientTypeResolver())
        { }

        /// <summary>
        /// Creates a new instance of <see cref="ClickHouseDbHelper"/> class.
        /// </summary>
        /// <param name="dbTypeResolver">The type resolver to be used.</param>
        public ClickHouseDbHelper(IResolver<string, Type> dbTypeResolver)
        {
            DbTypeResolver = dbTypeResolver;
        }

        #region Properties

        /// <summary>
        /// Gets the type resolver used by this <see cref="ClickHouseDbHelper"/> instance.
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
            return @"SELECT name AS ColumnName
                , is_in_primary_key AS IsPrimary
                , 0 AS IsIdentity
                , type AS ColumnType
                , character_octet_length AS Size
                , numeric_precision AS `Precision`
                , numeric_scale AS Scale
                , type AS DatabaseType
                , if(default_expression = '', 0, 1) AS HasDefaultValue
            FROM system.columns
            WHERE database = {TableSchema:String}
                AND table = {TableName:String}
            ORDER BY position;";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private IDbCommand CreateGetFieldsCommand(IDbConnection connection,
            string tableName,
            IDbTransaction transaction)
        {
            var command = connection.EnsureOpen().CreateCommand();
            command.CommandText = GetCommandText();
            command.Transaction = transaction;

            var tableSchemaParameter = command.CreateParameter();
            tableSchemaParameter.ParameterName = "TableSchema";
            tableSchemaParameter.Value = connection.Database;
            command.Parameters.Add(tableSchemaParameter);

            var tableNameParameter = command.CreateParameter();
            tableNameParameter.ParameterName = "TableName";
            tableNameParameter.Value = DataEntityExtension.GetTableName(tableName, m_dbSetting).AsUnquoted(m_dbSetting);
            command.Parameters.Add(tableNameParameter);

            return command;
        }

        /// <summary>
        /// Determines whether the given ClickHouse type name is wrapped in <c>Nullable(...)</c>.
        /// </summary>
        /// <param name="columnType"></param>
        /// <returns></returns>
        private static bool IsNullableType(string columnType) =>
            columnType?.StartsWith("Nullable(", StringComparison.OrdinalIgnoreCase) == true;

        /// <summary>
        ///
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private DbField ReaderToDbField(DbDataReader reader)
        {
            var columnType = reader.GetString(3);
            return new DbField(reader.GetString(0),
                reader.GetBoolean(1),
                reader.GetBoolean(2),
                IsNullableType(columnType),
                DbTypeResolver.Resolve(columnType),
                reader.IsDBNull(4) ? (int?)null : Convert.ToInt32(reader.GetValue(4)),
                reader.IsDBNull(5) ? null : byte.Parse(reader.GetValue(5).ToString()),
                reader.IsDBNull(6) ? null : byte.Parse(reader.GetValue(6).ToString()),
                reader.GetString(7),
                reader.GetBoolean(8),
                "CLICKHOUSE");
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
            var columnType = await reader.GetFieldValueAsync<string>(3, cancellationToken);
            return new DbField(await reader.GetFieldValueAsync<string>(0, cancellationToken),
                Convert.ToBoolean(await reader.GetFieldValueAsync<object>(1, cancellationToken)),
                Convert.ToBoolean(await reader.GetFieldValueAsync<object>(2, cancellationToken)),
                IsNullableType(columnType),
                DbTypeResolver.Resolve(columnType),
                await reader.IsDBNullAsync(4, cancellationToken) ? (int?)null : Convert.ToInt32(await reader.GetFieldValueAsync<object>(4, cancellationToken)),
                await reader.IsDBNullAsync(5, cancellationToken) ? null : byte.Parse((await reader.GetFieldValueAsync<object>(5, cancellationToken)).ToString()),
                await reader.IsDBNullAsync(6, cancellationToken) ? null : byte.Parse((await reader.GetFieldValueAsync<object>(6, cancellationToken)).ToString()),
                await reader.GetFieldValueAsync<string>(7, cancellationToken),
                Convert.ToBoolean(await reader.GetFieldValueAsync<object>(8, cancellationToken)),
                "CLICKHOUSE");
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
            // Iterate and extract
            using var command = CreateGetFieldsCommand(connection, tableName, transaction);
            using var reader = (DbDataReader)command.ExecuteReader();

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

            // Iterate and extract
            using var command = CreateGetFieldsCommand(connection, tableName, transaction);
            using var reader = (DbDataReader)await ((DbCommand)command).ExecuteReaderAsync(cancellationToken);

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
        /// Gets the newly generated identity from the database. Not supported: ClickHouse has no
        /// session-wide scope identity, sequence, or auto-increment mechanism of any kind.
        /// </summary>
        /// <typeparam name="T">The type of newly generated identity.</typeparam>
        /// <param name="connection">The instance of the connection object.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <returns>Never returns; always throws.</returns>
        public T GetScopeIdentity<T>(IDbConnection connection,
            IDbTransaction transaction = null) =>
            throw new NotSupportedException("ClickHouse has no session-wide scope identity, sequence, or auto-increment mechanism.");

        /// <summary>
        /// Gets the newly generated identity from the database in an asynchronous way. Not supported:
        /// ClickHouse has no session-wide scope identity, sequence, or auto-increment mechanism of any kind.
        /// </summary>
        /// <typeparam name="T">The type of newly generated identity.</typeparam>
        /// <param name="connection">The instance of the connection object.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>Never returns; always throws.</returns>
        public Task<T> GetScopeIdentityAsync<T>(IDbConnection connection,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException("ClickHouse has no session-wide scope identity, sequence, or auto-increment mechanism.");

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
                HandleDbParameterPostCreation((ClickHouseDbParameter)(object)instance);
            }
        }

        #region Handlers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        private void HandleDbParameterPostCreation(ClickHouseDbParameter parameter)
        {
            if (parameter?.ParameterName?.StartsWith("@") == true)
            {
                parameter.ParameterName = parameter.ParameterName.Substring(1);
            }
        }

        #endregion

        #endregion

        #endregion
    }
}
