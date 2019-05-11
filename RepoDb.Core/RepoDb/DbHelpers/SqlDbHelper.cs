using RepoDb.Extensions;
using RepoDb.Interfaces;
using System;
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
            var result = new List<DbField>();

            // Open a connection
            using (var connection = new SqlConnection(connectionString).EnsureOpen())
            {
                // Check for the command type
                var commandText = @"
                    SELECT C.COLUMN_NAME AS ColumnName
	                    , CASE WHEN TC.CONSTRAINT_TYPE = 'PRIMARY KEY' THEN
		                    CONVERT(BIT, 1)
	                      ELSE
		                    CONVERT(BIT, 0)
	                      END AS IsPrimary
	                    , TMP.is_identity AS IsIdentity
	                    , TMP.is_nullable AS IsNullable
	                    , C.DATA_TYPE AS DataType
						, TMP.max_length AS Size
						, TMP.precision AS Precision
						, TMP.scale AS Scale
                    FROM INFORMATION_SCHEMA.COLUMNS C
                    LEFT JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE KCU
	                    ON KCU.TABLE_SCHEMA = C.TABLE_SCHEMA
	                    AND KCU.TABLE_NAME = C.TABLE_NAME
	                    AND KCU.COLUMN_NAME = C.COLUMN_NAME
                    LEFT JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS TC
	                    ON TC.TABLE_SCHEMA = C.TABLE_SCHEMA
	                    AND TC.TABLE_NAME = C.TABLE_NAME
	                    AND TC.CONSTRAINT_NAME = KCU.CONSTRAINT_NAME
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
                        while (reader.Read())
                        {
                            yield return new DbField(reader.GetString(0),
                                reader.GetBoolean(1),
                                reader.GetBoolean(2),
                                reader.GetBoolean(3),
                                DbTypeResolver.Resolve(reader.GetString(4)),
                                reader.GetInt16(5),
                                reader.GetByte(6),
                                reader.GetByte(7),
                                reader.GetString(4));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the type resolver used by this <see cref="SqlDbHelper"/> instance.
        /// </summary>
        public IResolver<string, Type> DbTypeResolver { get; }
    }
}
