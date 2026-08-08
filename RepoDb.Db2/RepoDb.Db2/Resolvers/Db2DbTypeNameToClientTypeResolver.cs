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
                // DECIMAL/NUMERIC carry no fixed CLR equivalent (they can represent both integers
                // and fractional values depending on precision/scale); decimal is the safest
                // lossless default. Callers with access to the catalog's precision/scale columns
                // (see Db2DbHelper) can refine this further.
                "decimal" or "numeric" or "dec" or "decfloat" => typeof(decimal),
                "real" => typeof(float),
                "double" or "double precision" or "float" => typeof(double),
                // Db2 has no distinct "NVARCHAR2"/"VARCHAR2" the way Oracle does - VARCHAR/CHAR
                // already store whatever the database's configured code page/encoding is. GRAPHIC/
                // VARGRAPHIC are Db2's fixed/variable-length double-byte/graphic string types (the
                // closest equivalent to Oracle's NCHAR/NVARCHAR2), and CLOB/DBCLOB are their large-
                // object counterparts (DBCLOB being the closest equivalent to Oracle's NCLOB).
                "char" or "character" or "varchar" or "long varchar" or
                    "graphic" or "vargraphic" or "long vargraphic" or
                    "clob" or "dbclob" or "xml" or "rowid" => typeof(string),
                "date" => typeof(DateTime),
                // Db2's TIME type has no sub-second precision at all (unlike Oracle's INTERVAL DAY
                // TO SECOND workaround for a fractional TimeSpan) - it maps to the whole-second
                // resolution of TimeSpan.
                "time" => typeof(TimeSpan),
                // BINARY/VARBINARY are the newer (Db2 11.1+) dedicated binary types; CHAR/VARCHAR/
                // LONG VARCHAR "FOR BIT DATA" columns are also reported as BLOB-compatible byte[]
                // here since the catalog's TYPENAME for those doesn't distinguish "FOR BIT DATA"
                // from the plain character type by name alone.
                "blob" or "binary" or "varbinary" => typeof(byte[]),
                "boolean" => typeof(bool),
                _ => typeof(object),
            };
        }
    }
}
