using RepoDb;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Resolvers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Threading.Tasks;

namespace RepoDb.DbHelpers
{
    public class SqLiteDbHelper : IDbHelper
    {
        /// <summary>
        /// Creates a new instance of <see cref="SqlDbHelper"/> class.
        /// </summary>
        public SqLiteDbHelper()
        {
            DbTypeResolver = new SqLiteDbTypeNameToClientTypeResolver();
        }

        /// <summary>
        /// Gets the type resolver used by this <see cref="SqlDbHelper"/> instance.
        /// </summary>
        public IResolver<string, Type> DbTypeResolver { get; }

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
        /// <returns>The instance of converted <see cref="DbField"/> object.</returns>
        private DbField ReaderToDbField(IDataReader reader)
        {
            return new DbField(reader.GetString(1),
                reader.IsDBNull(5) ? false : reader.GetBoolean(5),
                false,
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
            if (tableName.IndexOf(".") > 0)
            {
                var splitted = tableName.Split(".".ToCharArray());
                return splitted[0].AsUnquoted(true);
            }
            return "dbo"; // TODO: Research the default schema of SqLite
        }

        /// <summary>
        /// Gets the actual name of the table from the database.
        /// </summary>
        /// <param name="tableName">The passed table name.</param>
        /// <returns>The actual table name.</returns>
        private string GetTableName(string tableName)
        {
            if (tableName.IndexOf(".") > 0)
            {
                var splitted = tableName.Split(".".ToCharArray());
                return splitted[1].AsUnquoted(true);
            }
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
            using (var connection = new SQLiteConnection(connectionString))
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
            using (var connection = new SQLiteConnection(connectionString))
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
            tableName = GetTableName(tableName);

            // Open a command
            using (var dbCommand = connection.EnsureOpen().CreateCommand(GetCommandText(tableName), transaction: transaction))
            {
                // Create parameters
                dbCommand.CreateParameters(new { TableName = tableName });

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
            tableName = GetTableName(tableName);

            // Open a command
            using (var dbCommand = ((DbConnection)await connection.EnsureOpenAsync()).CreateCommand(GetCommandText(tableName), transaction: transaction))
            {
                // Create parameters
                dbCommand.CreateParameters(new { Schema = GetSchema(tableName), TableName = tableName });

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
