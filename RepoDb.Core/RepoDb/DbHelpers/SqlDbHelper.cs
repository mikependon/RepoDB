using RepoDb.Extensions;
using RepoDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

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
        /// Gets the list of <see cref="DbField"/> of the table.
        /// </summary>
        /// <param name="connectionString">The connection string to connect to.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <returns>A list of <see cref="DbField"/> of the target table.</returns>
        public IEnumerable<DbField> GetFields(string connectionString, string tableName)
        {
            // Open a connection
            using (var connection = new SqlConnection(connectionString).EnsureOpen())
            {
                return GetFields(connection, tableName);
            }
        }

        /// <summary>
        /// Gets the list of <see cref="DbField"/> of the table.
        /// </summary>
        /// <typeparam name="TDbConnection">The type of <see cref="DbConnection"/> object.</typeparam>
        /// <param name="connection">The instance of the connection object.</param>
        /// <param name="tableName">The name of the target table.</param>
        /// <returns>A list of <see cref="DbField"/> of the target table.</returns>
        public IEnumerable<DbField> GetFields<TDbConnection>(TDbConnection connection, string tableName)
            where TDbConnection : IDbConnection
        {
            // Check for the command type
            var commandText = @"
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
            var schema = "dbo";

            // Get the schema and table name
            if (tableName.IndexOf(".") > 0)
            {
                var splitted = tableName.Split(".".ToCharArray());
                schema = splitted[0].AsUnquoted(true);
                tableName = splitted[1].AsUnquoted(true);
            }
            else
            {
                tableName = tableName.AsUnquoted();
            }

            // Open a command
            using (var dbCommand = connection.EnsureOpen().CreateCommand(commandText))
            {
                // Create parameters
                dbCommand.CreateParameters(new { Schema = schema, TableName = tableName });

                // Execute and set the result
                using (var reader = dbCommand.ExecuteReader())
                {
                    var dbFields = new List<DbField>();

                    // Iterate the list of the fields
                    while (reader.Read())
                    {
                        var dbField = new DbField(reader.GetString(0),
                           reader.IsDBNull(1) ? false : reader.GetBoolean(1),
                           reader.IsDBNull(2) ? false : reader.GetBoolean(2),
                           reader.IsDBNull(3) ? false : reader.GetBoolean(3),
                           reader.IsDBNull(4) ? DbTypeResolver.Resolve("text") : DbTypeResolver.Resolve(reader.GetString(4)),
                           reader.IsDBNull(5) ? 0 : reader.GetInt32(5),
                           reader.IsDBNull(6) ? (byte?)0 : reader.GetByte(6),
                           reader.IsDBNull(7) ? (byte?)0 : reader.GetByte(7),
                           reader.IsDBNull(7) ? "text" : reader.GetString(4));

                        // Add the field to the list
                        dbFields.Add(dbField);
                    }

                    // return the list of fields
                    return dbFields;
                }
            }
        }

        /// <summary>
        /// Gets the type resolver used by this <see cref="SqlDbHelper"/> instance.
        /// </summary>
        public IResolver<string, Type> DbTypeResolver { get; }
    }
}
