using RepoDb.Interfaces;
using System;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class used to resolve the Db2 Database Types into its equivalent .NET CLR Types.
    /// </summary>
    public class Db2DbTypeNameToClientTypeResolver : IResolver<string, Type>
    {
        /*
         * Taken:
         * https://www.ibm.com/docs/en/db2/11.5?topic=catalog-syscatcolumns-view (TYPENAME domain)
         * https://www.ibm.com/docs/en/db2/11.5?topic=elements-data-types (SQL data types)
         */

        /// <summary>
        /// Returns the equivalent .NET CLR Types of the Database Type.
        /// </summary>
        /// <param name="dbTypeName">The name of the database type (e.g. as returned by SYSCAT.COLUMNS.TYPENAME).</param>
        /// <returns>The equivalent .NET CLR type.</returns>
        public virtual Type Resolve(string dbTypeName)
        {
            if (dbTypeName == null)
            {
                throw new NullReferenceException("The DB Type name must not be null.");
            }

            var name = dbTypeName.ToLowerInvariant().Trim();

            // TIMESTAMP columns are reported with an inline scale/qualifier, e.g. "TIMESTAMP(6)"
            // or "TIMESTAMP(6) WITH TIME ZONE". Unlike Oracle, Db2 has no "WITH LOCAL TIME ZONE"
            // variant, and the IBM.Data.Db2 DB2Type enumeration has no timezone-aware member at
            // all (every Date/Time/Timestamp member maps only to a plain DateTime/TimeSpan) - so
            // "WITH TIME ZONE" is mapped here for the SQL-level type only, on a best-effort basis.
            if (name.StartsWith("timestamp"))
            {
                return name.Contains("with time zone") ? typeof(DateTimeOffset) : typeof(DateTime);
            }

            return name switch
            {
                "smallint" => typeof(short),
                "integer" or "int" => typeof(int),
                "bigint" => typeof(long),
                "decimal" or "numeric" or "dec" or "decfloat" => typeof(decimal),
                "real" => typeof(float),
                "double" or "double precision" or "float" => typeof(double),
                "char" or "character" or "varchar" or "long varchar" or
                    "graphic" or "vargraphic" or "long vargraphic" or
                    "clob" or "dbclob" or "xml" or "rowid" => typeof(string),
                "date" => typeof(DateTime),
                "time" => typeof(TimeSpan),
                "blob" or "binary" or "varbinary" => typeof(byte[]),
                "boolean" => typeof(bool),
                _ => typeof(object),
            };
        }
    }
}
