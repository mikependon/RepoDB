using RepoDb.Interfaces;
using System;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class used to resolve the Oracle Database Types into its equivalent .NET CLR Types.
    /// </summary>
    public class OracleDbTypeNameToClientTypeResolver : IResolver<string, Type>
    {
        /*
         * Taken:
         * https://docs.oracle.com/en/database/oracle/oracle-database/23/sqlrf/Data-Types.html
         */

        /// <summary>
        /// Returns the equivalent .NET CLR Types of the Database Type.
        /// </summary>
        /// <param name="dbTypeName">The name of the database type (as returned by ALL_TAB_COLUMNS.DATA_TYPE).</param>
        /// <returns>The equivalent .NET CLR type.</returns>
        public virtual Type Resolve(string dbTypeName)
        {
            if (dbTypeName == null)
            {
                throw new NullReferenceException("The DB Type name must not be null.");
            }

            var name = dbTypeName.ToLowerInvariant().Trim();

            // TIMESTAMP columns are reported with an inline scale/qualifier, e.g. "TIMESTAMP(6)",
            // "TIMESTAMP(6) WITH TIME ZONE" or "TIMESTAMP(6) WITH LOCAL TIME ZONE".
            if (name.StartsWith("timestamp"))
            {
                return name.Contains("with time zone") || name.Contains("with local time zone") ?
                    typeof(DateTimeOffset) : typeof(DateTime);
            }

            // INTERVAL columns are reported as "INTERVAL YEAR(2) TO MONTH" or
            // "INTERVAL DAY(2) TO SECOND(6)".
            if (name.StartsWith("interval"))
            {
                return typeof(TimeSpan);
            }

            return name switch
            {
                // NUMBER carries no fixed CLR equivalent (it can represent both integers and
                // decimals depending on its precision/scale); decimal is the safest lossless
                // default. Callers with access to DATA_PRECISION/DATA_SCALE (see OracleDbHelper)
                // can refine this further.
                "number" or "float" or "dec" or "decimal" or "numeric" => typeof(decimal),
                "binary_float" => typeof(float),
                "binary_double" => typeof(double),
                "char" or "nchar" or "varchar" or "varchar2" or "nvarchar2" or "long" or
                    "clob" or "nclob" or "rowid" or "urowid" or "xmltype" => typeof(string),
                "date" => typeof(DateTime),
                "blob" or "raw" or "long raw" or "bfile" => typeof(byte[]),
                "boolean" => typeof(bool),
                _ => typeof(object),
            };
        }
    }
}
