using RepoDb.Extensions;
using RepoDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace RepoDb.DbHelpers
{
    /// <summary>
    /// A helper class for database specially for the direct access. This class is only meant for SQL Server.
    /// </summary>
    public class SqlDbHelper : IDbHelper
    {
        /// <summary>
        /// Creates a new instance of <see cref="SqlDbHelper"/> class.
        /// </summary>
        public SqlDbHelper()
        {
            DbTypeResolver = new SqlDbTypeNameToClientTypeResolver();
        }

        /// <summary>
        /// Gets the type resolver used by this <see cref="SqlDbHelper"/> instance.
        /// </summary>
        public IResolver<string, Type> DbTypeResolver { get; }

        /// <summary>
        /// Returns the command text that is being used to extract schema definitions.
        /// </summary>
        /// <returns>The command text.</returns>
        private string GetCommandText()
        {
            return @"
                SELECT C.COLUMN_NAME AS ColumnName
	                , CONVERT(BIT, COALESCE(TC.is_primary, 0)) AS IsPrimary
	                , CONVERT(BIT, COALESCE(TMP.is_identity, 1)) AS IsIdentity
	                , CONVERT(BIT, COALESCE(TMP.is_nullable, 1)) AS IsNullable
	                , C.DATA_TYPE AS DataType
	                , CONVERT(INT, COALESCE(TMP.max_length, 1)) AS Size
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
	                WHERE SC.name = C.COLUMN_NAME
		                AND ST.name = C.TABLE_NAME
                ) TMP
                WHERE
	                C.TABLE_SCHEMA = @Schema
	                AND C.TABLE_NAME = @TableName;";
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
                reader.IsDBNull(5) ? 0 : reader.GetInt32(5),
                reader.IsDBNull(6) ? (byte?)0 : reader.GetByte(6),
                reader.IsDBNull(7) ? (byte?)0 : reader.GetByte(7),
                reader.IsDBNull(7) ? "text" : reader.GetString(4));
        }

        /// <summary>
        /// Gets the actual schema of the table from the database.
        /// </summary>
        /// <param name="tableName">The passed table name.</param>
        /// <returns>The actual table schema.</returns>
        private string GetSchema(string tableName)
        {
            // Get the schema and table name
            if (tableName.IndexOf(".") > 0)
            {
                var splitted = tableName.Split(".".ToCharArray());
                return splitted[0].AsUnquoted(true);
            }

            // Return the unquoted
            return "dbo";
        }

        /// <summary>
        /// Gets the actual name of the table from the database.
        /// </summary>
        /// <param name="tableName">The passed table name.</param>
        /// <returns>The actual table name.</returns>
        private string GetTableName(string tableName)
        {
            // Check for the command type
            var schema = "dbo";

            // Get the schema and table name
            if (tableName.IndexOf(".") > 0)
            {
                var splitted = tableName.Split(".".ToCharArray());
                schema = splitted[0].AsUnquoted(true);

                // Return the splitted one
                return splitted[1].AsUnquoted(true);
            }

            // Return the unquoted
            return tableName.AsUnquoted();
        }

        /// <summary>
        /// Gets the list of <see cref="DbField"/> of the table.
        /// </summary>
        /// <param name="connectionString">The connection string to connect to.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <returns>A list of <see cref="DbField"/> of the target table.</returns>
        public IEnumerable<DbField> GetFields(string connectionString, string tableName, IDbTransaction transaction = null)
        {
            // Open a connection
            using (var connection = new SqlConnection(connectionString))
            {
                return GetFieldsInternal(connection, tableName, transaction);
            }
        }

        /// <summary>
        /// Gets the list of <see cref="DbField"/> of the table in an asychronous way.
        /// </summary>
        /// <param name="connectionString">The connection string to connect to.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <returns>A list of <see cref="DbField"/> of the target table.</returns>
        public Task<IEnumerable<DbField>> GetFieldsAsync(string connectionString, string tableName, IDbTransaction transaction = null)
        {
            // Open a connection
            using (var connection = new SqlConnection(connectionString))
            {
                return GetFieldsAsyncInternal(connection, tableName, transaction);
            }
        }

        /// <summary>
        /// Gets the list of <see cref="DbField"/> of the table.
        /// </summary>
        /// <typeparam name="TDbConnection">The type of <see cref="DbConnection"/> object.</typeparam>
        /// <param name="connection">The instance of the connection object.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <returns>A list of <see cref="DbField"/> of the target table.</returns>
        public IEnumerable<DbField> GetFields<TDbConnection>(TDbConnection connection, string tableName, IDbTransaction transaction = null)
            where TDbConnection : IDbConnection
        {
            return GetFieldsInternal(connection, tableName, transaction);
        }

        /// <summary>
        /// Gets the list of <see cref="DbField"/> of the table in an asychronous way.
        /// </summary>
        /// <typeparam name="TDbConnection">The type of <see cref="DbConnection"/> object.</typeparam>
        /// <param name="connection">The instance of the connection object.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <returns>A list of <see cref="DbField"/> of the target table.</returns>
        public Task<IEnumerable<DbField>> GetFieldsAsync<TDbConnection>(TDbConnection connection, string tableName, IDbTransaction transaction = null)
            where TDbConnection : IDbConnection
        {
            return GetFieldsAsyncInternal(connection, tableName, transaction);
        }

        /// <summary>
        /// Gets the list of <see cref="DbField"/> of the table.
        /// </summary>
        /// <typeparam name="TDbConnection">The type of <see cref="DbConnection"/> object.</typeparam>
        /// <param name="connection">The instance of the connection object.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <param name="transaction">The transaction object that is currently in used.</param>
        /// <returns>A list of <see cref="DbField"/> of the target table.</returns>
        internal IEnumerable<DbField> GetFieldsInternal<TDbConnection>(TDbConnection connection, string tableName, IDbTransaction transaction = null)
            where TDbConnection : IDbConnection
        {
            // Open a command
            using (var dbCommand = connection.EnsureOpen().CreateCommand(GetCommandText(), transaction: transaction))
            {
                // Create parameters
                dbCommand.CreateParameters(new { Schema = GetSchema(tableName), TableName = GetTableName(tableName) });

                // Execute and set the result
                using (var reader = dbCommand.ExecuteReader())
                {
                    var dbFields = new List<DbField>();

                    // Iterate the list of the fields
                    while (reader.Read())
                    {
                        dbFields.Add(ReaderToDbField(reader));
                    }

                    // return the list of fields
                    return dbFields;
                }
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
        internal async Task<IEnumerable<DbField>> GetFieldsAsyncInternal<TDbConnection>(TDbConnection connection, string tableName, IDbTransaction transaction = null)
            where TDbConnection : IDbConnection
        {
            // Open a command
            using (var dbCommand = ((DbConnection)await connection.EnsureOpenAsync()).CreateCommand(GetCommandText(), transaction: transaction))
            {
                // Create parameters
                dbCommand.CreateParameters(new { Schema = GetSchema(tableName), TableName = GetTableName(tableName) });

                // Execute and set the result
                using (var reader = await ((DbCommand)dbCommand).ExecuteReaderAsync())
                {
                    var dbFields = new List<DbField>();

                    // Iterate the list of the fields
                    while (await reader.ReadAsync())
                    {
                        dbFields.Add(ReaderToDbField(reader));
                    }

                    // return the list of fields
                    return dbFields;
                }
            }
        }
    }
}
