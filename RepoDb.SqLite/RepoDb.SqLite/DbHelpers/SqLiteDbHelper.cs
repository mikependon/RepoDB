using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Resolvers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.DbHelpers
{
    /// <summary>
    /// A helper class for database specially for the direct access. This class is only meant for SqLite.
    /// </summary>
    internal class SqLiteDbHelper : IDbHelper
    {
        private IDbSetting m_dbSetting = DbSettingMapper.Get<SQLiteConnection>();

        /// <summary>
        /// Creates a new instance of <see cref="SqLiteDbHelper"/> class.
        /// </summary>
        public SqLiteDbHelper()
        {
            DbTypeResolver = new SqLiteDbTypeNameToClientTypeResolver();
        }

        #region Properties

        /// <summary>
        /// Gets the type resolver used by this <see cref="SqLiteDbHelper"/> instance.
        /// </summary>
        public IResolver<string, Type> DbTypeResolver { get; }

        #endregion

        #region Helpers

        /// <summary>
        /// Returns the command text that is being used to extract schema definitions.
        /// </summary>
        /// <param name="tableName">The name of the target table.</param>
        /// <returns>The command text.</returns>
        private string GetCommandText(string tableName)
        {
            return $"pragma table_info({tableName});";
        }

        /// <summary>
        /// Converts the <see cref="IDataReader"/> object into <see cref="DbField"/> object.
        /// </summary>
        /// <param name="reader">The instance of <see cref="IDataReader"/> object.</param>
        /// <param name="identityFieldName">The name of the identity column.</param>
        /// <returns>The instance of converted <see cref="DbField"/> object.</returns>
        private DbField ReaderToDbField(IDataReader reader,
            string identityFieldName)
        {
            return new DbField(reader.GetString(1),
                reader.IsDBNull(5) ? false : reader.GetBoolean(5),
                string.Equals(reader.GetString(1), identityFieldName, StringComparison.OrdinalIgnoreCase),
                reader.IsDBNull(3) ? true : reader.GetBoolean(3) == false,
                reader.IsDBNull(2) ? DbTypeResolver.Resolve("text") : DbTypeResolver.Resolve(reader.GetString(2)),
                null,
                null,
                null,
                null);
        }

        /// <summary>
        /// Gets the actual schema of the table from the database.
        /// </summary>
        /// <param name="tableName">The passed table name.</param>
        /// <returns>The actual table schema.</returns>
        private string GetSchema(string tableName)
        {
            // Get the schema and table name
            if (tableName.IndexOf(m_dbSetting.SchemaSeparator) > 0)
            {
                var splitted = tableName.Split(m_dbSetting.SchemaSeparator.ToCharArray());
                return splitted[0].AsUnquoted(true, m_dbSetting);
            }

            // Return the unquoted
            return m_dbSetting.DefaultSchema;
        }

        /// <summary>
        /// Gets the actual name of the table from the database.
        /// </summary>
        /// <param name="tableName">The passed table name.</param>
        /// <returns>The actual table name.</returns>
        private string GetTableName(string tableName)
        {
            // Get the schema and table name
            if (tableName.IndexOf(m_dbSetting.SchemaSeparator) > 0)
            {
                var splitted = tableName.Split(m_dbSetting.SchemaSeparator.ToCharArray());
                return splitted[1].AsUnquoted(true, m_dbSetting);
            }

            // Return the unquoted
            return tableName.AsUnquoted(true, m_dbSetting);
        }

        /// <summary>
        /// Gets the list of <see cref="DbField"/> of the table.
        /// </summary>
        /// <typeparam name="TDbConnection">The type of <see cref="DbConnection"/> object.</typeparam>
        /// <param name="connection">The instance of the connection object.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <returns>A list of <see cref="DbField"/> of the target table.</returns>
        private string GetIdentityFieldName<TDbConnection>(TDbConnection connection,
            string tableName,
            IDbTransaction transaction = null)
            where TDbConnection : IDbConnection
        {
            // Sql text
            var commandText = "SELECT sql FROM [sqlite_master] WHERE name = @TableName AND type = 'table';";
            var sql = connection.ExecuteScalar<string>(commandText, new { TableName = GetTableName(tableName) });
            var fields = ParseTableFieldsFromSql(sql);

            // Iterate the fields
            if (fields != null && fields.Length > 0)
            {
                foreach (var field in fields)
                {
                    if (field.ToUpper().Contains("AUTOINCREMENT"))
                    {
                        return field.Substring(0, field.IndexOf(" "));
                    }
                }
            }

            // Return null
            return null;
        }

        /// <summary>
        /// Parses the table sql and return the list of the fields.
        /// </summary>
        /// <param name="sql">The sql to be parsed.</param>
        /// <returns>The list of the fields.</returns>
        private string[] ParseTableFieldsFromSql(string sql)
        {
            if (string.IsNullOrEmpty(sql))
            {
                return null;
            }

            // Do parse
            var openingTokenIndex = sql.IndexOf("(");
            var closingTokenIndex = sql.IndexOf(")");
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
        /// <typeparam name="TDbConnection">The type of <see cref="DbConnection"/> object.</typeparam>
        /// <param name="connection">The instance of the connection object.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <returns>A list of <see cref="DbField"/> of the target table.</returns>
        public IEnumerable<DbField> GetFields<TDbConnection>(TDbConnection connection,
            string tableName,
            IDbTransaction transaction = null)
            where TDbConnection : IDbConnection
        {
            // Variables
            var commandText = GetCommandText(tableName);

            // Iterate and extract
            using (var reader = connection.ExecuteReader(commandText, transaction: transaction))
            {
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
        }

        /// <summary>
        /// Gets the list of <see cref="DbField"/> of the table in an asychronous way.
        /// </summary>
        /// <typeparam name="TDbConnection">The type of <see cref="DbConnection"/> object.</typeparam>
        /// <param name="connection">The instance of the connection object.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <returns>A list of <see cref="DbField"/> of the target table.</returns>
        public async Task<IEnumerable<DbField>> GetFieldsAsync<TDbConnection>(TDbConnection connection, string tableName, IDbTransaction transaction = null)
            where TDbConnection : IDbConnection
        {
            // Variables
            var commandText = GetCommandText(tableName);
            var param = new
            {
                TableName = GetTableName(tableName)
            };

            // Iterate and extract
            using (var reader = await connection.ExecuteReaderAsync(commandText, param, transaction: transaction))
            {
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
        }

        #endregion

        #region GetScopeIdentity

        /// <summary>
        /// Gets the newly generated identity from the database.
        /// </summary>
        /// <typeparam name="TDbConnection">The type of <see cref="IDbConnection"/> object.</typeparam>
        /// <param name="connection">The instance of the connection object.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <returns>The newly generated identity from the database.</returns>
        public object GetScopeIdentity<TDbConnection>(TDbConnection connection,
            IDbTransaction transaction = null)
            where TDbConnection : IDbConnection
        {
            return connection.ExecuteScalar("SELECT last_insert_rowid();");
        }

        /// <summary>
        /// Gets the newly generated identity from the database in an asychronous way.
        /// </summary>
        /// <typeparam name="TDbConnection">The type of <see cref="IDbConnection"/> object.</typeparam>
        /// <param name="connection">The instance of the connection object.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <returns>The newly generated identity from the database.</returns>
        public async Task<object> GetScopeIdentityAsync<TDbConnection>(TDbConnection connection,
            IDbTransaction transaction = null)
            where TDbConnection : IDbConnection
        {
            return await connection.ExecuteScalarAsync("SELECT last_insert_rowid();");
        }

        #endregion

        #endregion
    }
}
