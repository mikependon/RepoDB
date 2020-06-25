using Npgsql;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Resolvers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace RepoDb.DbHelpers
{
    /// <summary>
    /// A helper class for database specially for the direct access. This class is only meant for PostgreSql.
    /// </summary>
    public sealed class PostgreSqlDbHelper : IDbHelper
    {
        private IDbSetting m_dbSetting = DbSettingMapper.Get<NpgsqlConnection>();

        /// <summary>
        /// Creates a new instance of <see cref="PostgreSqlDbHelper"/> class.
        /// </summary>
        public PostgreSqlDbHelper()
            : this(new PostgreSqlDbTypeNameToClientTypeResolver())
        { }

        /// <summary>
        /// Creates a new instance of <see cref="PostgreSqlDbHelper"/> class.
        /// </summary>
        /// <param name="dbTypeResolver">The type resolver to be used.</param>
        public PostgreSqlDbHelper(IResolver<string, Type> dbTypeResolver)
        {
            DbTypeResolver = dbTypeResolver;
        }

        #region Properties

        /// <summary>
        /// Gets the type resolver used by this <see cref="PostgreSqlDbHelper"/> instance.
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
            return @"
                SELECT C.column_name
	                , CAST((CASE WHEN C.column_name = TMP.column_name THEN 1 ELSE 0 END) AS BOOLEAN) AS IsPrimary
	                , CAST(C.is_identity AS BOOLEAN) AS IsIdentity
	                , CAST(C.is_nullable AS BOOLEAN) AS IsNullable
	                , C.data_type AS DataType
                FROM information_schema.columns C
                LEFT JOIN
                (
	                SELECT C.table_schema
		                , C.table_name
		                , C.column_name
	                FROM information_schema.table_constraints TC 
	                JOIN information_schema.constraint_column_usage AS CCU USING (constraint_schema, constraint_name) 
	                JOIN information_schema.columns AS C ON C.table_schema = TC.constraint_schema
	  	                AND TC.table_name = C.table_name
		                AND CCU.column_name = C.column_name
	                WHERE TC.constraint_type = 'PRIMARY KEY'
                ) TMP ON TMP.table_schema = C.table_schema
	                AND TMP.table_name = C.table_name
	                AND TMP.column_name = C.column_name
                WHERE C.table_name = @TableName
	                AND C.table_schema = @Schema;";
        }

        /// <summary>
        /// Converts the <see cref="IDataReader"/> object into <see cref="DbField"/> object.
        /// </summary>
        /// <param name="reader">The instance of <see cref="IDataReader"/> object.</param>
        /// <returns>The instance of converted <see cref="DbField"/> object.</returns>
        private DbField ReaderToDbField(IDataReader reader)
        {
            return new DbField(reader.GetString(0),
                reader.IsDBNull(1) ? false : reader.GetBoolean(1),
                reader.IsDBNull(2) ? false : reader.GetBoolean(2),
                reader.IsDBNull(3) ? false : reader.GetBoolean(3),
                reader.IsDBNull(4) ? DbTypeResolver.Resolve("text") : DbTypeResolver.Resolve(reader.GetString(4)),
                null,
                null,
                null,
                reader.IsDBNull(4) ? "text" : reader.GetString(4));
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
                Schema = GetSchema(tableName),
                TableName = GetTableName(tableName)
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
        /// Gets the list of <see cref="DbField"/> of the table in an asynchronous way.
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
                Schema = GetSchema(tableName),
                TableName = GetTableName(tableName)
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
            // TODO: May fail with trigger?
            return connection.ExecuteScalar("SELECT lastval();");
        }

        /// <summary>
        /// Gets the newly generated identity from the database in an asynchronous way.
        /// </summary>
        /// <param name="connection">The instance of the connection object.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <returns>The newly generated identity from the database.</returns>
        public async Task<object> GetScopeIdentityAsync(IDbConnection connection,
            IDbTransaction transaction = null)
        {
            // TODO: May fail with trigger?
            return await connection.ExecuteScalarAsync("SELECT lastval();");
        }

        #endregion

        #endregion
    }
}
