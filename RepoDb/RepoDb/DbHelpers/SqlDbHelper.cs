using RepoDb.Extensions;
using RepoDb.Interfaces;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace RepoDb.DbHelpers
{
    /// <summary>
    /// A helper class for database specially for the direct access. This class is only meant for SQL Server.
    /// </summary>
    public class SqlDbHelper : IDbHelper
    {
        /// <summary>
        /// Gets the list of <see cref="DbField"/> of the table.
        /// </summary>
        /// <param name="connectionString">The connection string to connect to.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <returns>A list of <see cref="DbField"/> of the target table.</returns>
        public IEnumerable<DbField> GetFields(string connectionString, string tableName)
        {
            var result = new List<DbField>();

            // Open a connection
            using (var connection = new SqlConnection(connectionString).EnsureOpen())
            {
                // Check for the command type
                var commandText = @"
                    WITH Primary_CTE AS
                    (
	                    SELECT KCU.COLUMN_NAME AS [ColumnName]
	                    FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE KCU
	                    INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS TC ON TC.CONSTRAINT_NAME = KCU.CONSTRAINT_NAME
	                    WHERE KCU.TABLE_NAME = @TableName
		                    AND TC.CONSTRAINT_TYPE = 'PRIMARY KEY'
                    )
                    SELECT C.[name]
	                    , COALESCE(P.is_primary, 0) AS is_primary
	                    , C.is_identity
	                    , C.is_nullable
                    FROM [sys].[columns] C
                    OUTER APPLY
                    (
	                    SELECT 1 AS is_primary
	                    FROM Primary_CTE
	                    WHERE [ColumnName] = C.[name]
                    ) P
                    WHERE (C.object_id = OBJECT_ID(@TableName));";

                // Open a command
                using (var dbCommand = connection.EnsureOpen().CreateCommand(commandText))
                {
                    // Create parameters
                    dbCommand.CreateParameters(new { TableName = tableName.AsUnquoted() });

                    // Execute and set the result
                    using (var reader = dbCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            yield return new DbField(reader.GetString(0),
                                reader.GetInt32(1) > 0,
                                reader.GetBoolean(2),
                                reader.GetBoolean(3));
                        }
                    }
                }
            }
        }
    }
}
