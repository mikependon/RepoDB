using RepoDb.Enumerations;
using RepoDb.Extensions;
using System;
using System.Data;
using System.Data.SqlClient;

namespace RepoDb
{
    /// <summary>
    /// A helper class for database specially for the direct access. This class is only meant for SQL Server.
    /// </summary>
    internal static class SqlDbHelper
    {
        /// <summary>
        /// Checks whether the target column is an identity field from the database.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="connectionString">The connection string object to be used.</param>
        /// <param name="command">The target command.</param>
        /// <param name="columnName">The name of the column.</param>
        /// <returns>A boolean value indicating the identification of the column.</returns>
        public static bool IsIdentity<TEntity>(string connectionString, Command command, string columnName)
            where TEntity : class
        {
            var isIdentity = false;

            // Open a connection
            using (var connection = new SqlConnection(connectionString).EnsureOpen())
            {
                var commandType = CommandTypeCache.Get<TEntity>(command);

                // Check for the command type
                if (commandType != CommandType.StoredProcedure)
                {
                    var mappedName = ClassMappedNameCache.Get<TEntity>(command);
                    var commandText = @"
                        SELECT CONVERT(INT, c.is_identity) AS IsIdentity
                        FROM [sys].[columns] c
                        INNER JOIN [sys].[objects] o ON o.object_id = c.object_id
                        WHERE (c.[name] = @ColumnName)
	                        AND (o.[name] = @TableName)
	                        AND (o.[type] = 'U');";

                    // Open a command
                    using (var dbCommand = connection.EnsureOpen().CreateCommand(commandText))
                    {
                        // Create parameters
                        dbCommand.CreateParameters(new
                        {
                            ColumnName = columnName.AsUnquoted(),
                            TableName = mappedName.AsUnquoted()
                        });

                        // Execute and set the result
                        var result = dbCommand.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            isIdentity = Convert.ToBoolean(result);
                        }
                    }
                }
            }

            // Return the value
            return isIdentity;
        }
    }
}
