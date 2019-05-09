using RepoDb.Interfaces;
using RepoDb.Types;
using System;

namespace RepoDb
{
    /// <summary>
    /// A class used to resolve the SQL Database Types into .NET CLR Types.
    /// </summary>
    public class SqlDbTypeToClientTypeResolver : IResolver<string, Type>
    {
        /*
         * Taken:
         * https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/sql-server-data-type-mappings
         */

        /// <summary>
        /// Returns the equivalent .NET CLR Types of the Database Type.
        /// </summary>
        /// <param name="dbType">The name of the database type.</param>
        /// <returns>The equivalent .NET CLR type.</returns>
        public Type Resolve(string dbType)
        {
            switch (dbType.ToLower())
            {
                case "bigint":
                    return typeof(long);
                case "binary":
                case "image":
                case "rowversion":
                case "timestamp":
                case "varbinary":
                    return typeof(byte[]);
                case "bit":
                    return typeof(bool);
                case "char":
                case "nchar":
                case "ntext":
                case "nvarchar":
                case "text":
                case "varchar":
                case "xml": // Xml?
                    return typeof(string);
                case "date":
                case "datetime":
                case "datetime2":
                case "smalldatetime":
                    return typeof(DateTime);
                case "datetimeoffset":
                    return typeof(DateTimeOffset);
                case "decimal":
                case "money":
                case "numeric":
                case "smallmoney":
                    return typeof(decimal);
                case "FILESTREAM":
                case "attribute":
                case "varbinary(max)":
                    return typeof(byte[]);
                case "float":
                    return typeof(double);
                case "int":
                    return typeof(int);
                case "real":
                    return typeof(float);
                case "smallint":
                    return typeof(short);
                case "sql_variant":
                    return typeof(SqlVariant);
                case "time":
                    return typeof(TimeSpan);
                case "tinyint":
                    return typeof(byte);
                case "uniqueidentifier":
                    return typeof(Guid);
                default:
                    return typeof(object);
            }
        }
    }
}
