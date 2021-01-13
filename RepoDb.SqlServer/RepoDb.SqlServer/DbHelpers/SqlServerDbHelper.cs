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
    public sealed class SqlServerDbHelper : IDbHelper
    {
        /// <summary>
        /// Creates a new instance of <see cref="SqlServerDbHelper"/> class.
        /// </summary>
        public SqlServerDbHelper()
            : this(new SqlServerDbTypeNameToClientTypeResolver())
        { }

        /// <summary>
        /// Creates a new instance of <see cref="SqlServerDbHelper"/> class.
        /// </summary>
        /// <param name="dbTypeResolver">The type resolver to be used.</param>
        public SqlServerDbHelper(IResolver<string, Type> dbTypeResolver)
        {
            DbTypeResolver = dbTypeResolver;
        }

        #region Properties

        /// <summary>
        /// Gets the type resolver used by this <see cref="SqlServerDbHelper"/> instance.
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
                reader.IsDBNull(6) ? 0 : reader.GetByte(6),
                reader.IsDBNull(7) ? 0 : reader.GetByte(7),
                reader.IsDBNull(7) ? "text" : reader.GetString(4));
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
                await reader.IsDBNullAsync(5, cancellationToken) ? 0 : await reader.GetFieldValueAsync<int>(5, cancellationToken),
                await reader.IsDBNullAsync(6, cancellationToken) ? 0 : await reader.GetFieldValueAsync<byte>(6, cancellationToken),
                await reader.IsDBNullAsync(7, cancellationToken) ? 0 : await reader.GetFieldValueAsync<byte>(7, cancellationToken),
                await reader.IsDBNullAsync(7, cancellationToken) ? "text" : await reader.GetFieldValueAsync<string>(4, cancellationToken));
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
                Schema = DataEntityExtension.GetSchema(tableName, setting),
                TableName = DataEntityExtension.GetTableName(tableName, setting)
            };

            // Iterate and extract
            using (var reader = (DbDataReader)connection.ExecuteReader(commandText, param, transaction: transaction))
            {
                var dbFields = new List<DbField>();

                // Iterate the list of the fields
                while (reader.Read())
                {
                    dbFields.Add(ReaderToDbField(reader));
                }

                // Return the list of fields
                return dbFields;
            }
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
                Schema = DataEntityExtension.GetSchema(tableName, setting),
                TableName = DataEntityExtension.GetTableName(tableName, setting)
            };

            // Iterate and extract
            using (var reader = (DbDataReader)await connection.ExecuteReaderAsync(commandText, param,
                transaction: transaction, cancellationToken: cancellationToken))
            {
                var dbFields = new List<DbField>();

                // Iterate the list of the fields
                while (await reader.ReadAsync(cancellationToken))
                {
                    dbFields.Add(await ReaderToDbFieldAsync(reader, cancellationToken));
                }

                // Return the list of fields
                return dbFields;
            }
        }

        #endregion

        #region GetScopeIdentity

        /// <summary>
        /// Gets the newly generated identity from the database.
        /// </summary>
        /// <param name="connection">The instance of the connection object.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <returns>The newly generated identity from the database.</returns>
        public object GetScopeIdentity(IDbConnection connection,
            IDbTransaction transaction = null)
        {
            return connection.ExecuteScalar("SELECT COALESCE(SCOPE_IDENTITY(), @@IDENTITY);",
                transaction: transaction);
        }

        /// <summary>
        /// Gets the newly generated identity from the database in an asynchronous way.
        /// </summary>
        /// <param name="connection">The instance of the connection object.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> object to be used during the asynchronous operation.</param>
        /// <returns>The newly generated identity from the database.</returns>
        public async Task<object> GetScopeIdentityAsync(IDbConnection connection,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return await connection.ExecuteScalarAsync("SELECT COALESCE(SCOPE_IDENTITY(), @@IDENTITY);",
                transaction: transaction,
                cancellationToken: cancellationToken);
        }

        #endregion

        #endregion
    }
}
