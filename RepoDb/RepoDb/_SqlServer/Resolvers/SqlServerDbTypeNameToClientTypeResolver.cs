using RepoDb.Interfaces;
using RepoDb.Types;
using System;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class used to resolve the SQL Database Types into its equivalent .NET CLR Types.
    /// </summary>
    public class SqlServerDbTypeNameToClientTypeResolver : IResolver<string, Type>
    {
        /*
         * Taken:
         * https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/sql-server-data-type-mappings
         */

        /// <summary>
        /// Returns the equivalent .NET CLR Types of the Database Type.
        /// </summary>
        /// <param name="dbTypeName">The name of the database type.</param>
        /// <returns>The equivalent .NET CLR type.</returns>
        public Type Resolve(string dbTypeName)
        {
            if (dbTypeName == null)
            {
                throw new NullReferenceException("The DB Type name must not be null.");
            }
            switch (dbTypeName.ToLowerInvariant())
            {
                case "bigint":
                    return typeof(long);
                case "attribute":
                case "binary":
                case "filestream":
                case "image":
                case "rowversion":
                case "timestamp":
                case "varbinary":
                case "varbinary(max)":
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
