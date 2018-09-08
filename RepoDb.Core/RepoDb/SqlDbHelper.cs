using RepoDb.Enumerations;
using RepoDb.Extensions;
using System;
using System.Collections.Generic;
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
        /// Gets the field definitions of the table.
        /// </summary>
        /// <param name="connectionString">The connection string to be used.</param>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>An enumerable list of field definitions.</returns>
        public static IEnumerable<FieldDefinition> GetFieldDefinitions(string connectionString, string tableName)
        {
            var result = new List<FieldDefinition>();

            // Open a connection
            using (var connection = new SqlConnection(connectionString).EnsureOpen())
            {
                // Check for the command type
                var commandText = @"
                    SELECT C.name
	                    , C.is_identity
	                    , C.is_nullable
                    FROM [sys].[columns] C
                    WHERE (C.object_id = OBJECT_ID(@TableName));";

                // Open a command
                using (var dbCommand = connection.EnsureOpen().CreateCommand(commandText))
                {
                    // Create parameters
                    dbCommand.CreateParameters(new
                    {
                        TableName = tableName.AsUnquoted()
                    });

                    // Execute and set the result
                    using (var reader = dbCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new FieldDefinition
                            {
                                Name = reader.GetString(0),
                                IsIdentity = reader.GetBoolean(1),
                                IsNullable = reader.GetBoolean(2)
                            });
                        }
                    }
                }
            }

            return result;
        }
    }
}
