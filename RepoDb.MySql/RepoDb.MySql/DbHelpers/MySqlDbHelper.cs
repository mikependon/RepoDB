using MySql.Data.MySqlClient;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Resolvers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.DbHelpers
{
    /// <summary>
    /// A helper class for database specially for the direct access. This class is only meant for MySql.
    /// </summary>
    public sealed class MySqlDbHelper : IDbHelper
    {
        private IDbSetting m_dbSetting = DbSettingMapper.Get<MySqlConnection>();

        /// <summary>
        /// Creates a new instance of <see cref="MySqlDbHelper"/> class.
        /// </summary>
        public MySqlDbHelper()
            : this(new MySqlDbTypeNameToClientTypeResolver())
        { }

        /// <summary>
        /// Creates a new instance of <see cref="MySqlDbHelper"/> class.
        /// </summary>
        /// <param name="dbTypeResolver">The type resolver to be used.</param>
        public MySqlDbHelper(IResolver<string, Type> dbTypeResolver)
        {
            DbTypeResolver = dbTypeResolver;
        }

        #region Properties

        /// <summary>
        /// Gets the type resolver used by this <see cref="MySqlDbHelper"/> instance.
        /// </summary>
        public IResolver<string, Type> DbTypeResolver { get; }

        #endregion

        #region Helpers

        /// <summary>
        /// Returns the command text that is being used to extract schema definitions.
        /// </summary>
        /// <returns>The command text.</returns>
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
            FROM INFORMATION_SCHEMA.COLUMNS
            WHERE TABLE_SCHEMA = @TableSchema
                AND TABLE_NAME = @TableName
            ORDER BY ORDINAL_POSITION;";
        }

        /// <summary>
        /// Get the list of type names for all blob-types for MySql.
        /// </summary>
        /// <returns>The list of column type names.</returns>
        private IEnumerable<string> GetBlobTypes()
        {
            return new[]
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
        /// Converts the <see cref="IDataReader"/> object into <see cref="DbField"/> object.
        /// </summary>
        /// <param name="reader">The instance of <see cref="IDataReader"/> object.</param>
        /// <returns>The instance of converted <see cref="DbField"/> object.</returns>
        private DbField ReaderToDbField(IDataReader reader)
        {
            var columnType = reader.GetString(4);
            var excluded = GetBlobTypes();
            var size = (int?)null;
            if (excluded.Contains(columnType.ToLower()))
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
                reader.GetString(8));
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
                TableSchema = connection.Database,
                TableName = DataEntityExtension.GetTableName(tableName, m_dbSetting)
            };

            // Iterate and extract
            using (var reader = connection.ExecuteReader(commandText, param, transaction: transaction))
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
        /// Gets the list of <see cref="DbField"/> of the table in an asychronous way.
        /// </summary>
        /// <param name="connection">The instance of the connection object.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <returns>A list of <see cref="DbField"/> of the target table.</returns>
        public async Task<IEnumerable<DbField>> GetFieldsAsync(IDbConnection connection,
            string tableName,
            IDbTransaction transaction = null)
        {
            // Variables
            var commandText = GetCommandText();
            var param = new
            {
                TableSchema = connection.Database,
                TableName = DataEntityExtension.GetTableName(tableName, m_dbSetting)
            };

            // Iterate and extract
            using (var reader = await connection.ExecuteReaderAsync(commandText, param, transaction: transaction))
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
            return connection.ExecuteScalar("SELECT LAST_INSERT_ID();");
        }

        /// <summary>
        /// Gets the newly generated identity from the database in an asychronous way.
        /// </summary>
        /// <param name="connection">The instance of the connection object.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <returns>The newly generated identity from the database.</returns>
        public async Task<object> GetScopeIdentityAsync(IDbConnection connection,
            IDbTransaction transaction = null)
        {
            return await connection.ExecuteScalarAsync("SELECT LAST_INSERT_ID();");
        }

        #endregion

        #endregion
    }
}
