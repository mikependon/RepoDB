using RepoDb.Interfaces;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class that is being used to resolve a Firebird database type name (e.g. <see cref="DbField.DatabaseType"/>)
    /// into its equivalent base Firebird column type keyword (e.g. <c>NUMERIC</c>, <c>VARCHAR</c>). Sized types
    /// (numeric, decimal, char, varchar, binary, varbinary) are returned without their <c>(precision,scale)</c>/
    /// <c>(size)</c> portion - the caller is expected to append that using the field's own precision/scale/size.
    /// </summary>
    public class DbTypeNameToColumnNameResolver : IResolver<string, string>
    {
        /// <summary>
        /// Returns the equivalent base Firebird column type keyword of the database type name.
        /// </summary>
        /// <param name="dbTypeName">The name of the database type (i.e. <see cref="DbField.DatabaseType"/>).</param>
        /// <returns>The equivalent base column type keyword.</returns>
        public virtual string Resolve(string dbTypeName)
        {
            return dbTypeName?.ToLowerInvariant() switch
            {
                "smallint" => "SMALLINT",
                "integer" => "INTEGER",
                "bigint" => "BIGINT",
                "boolean" => "BOOLEAN",
                "float" => "FLOAT",
                "double precision" => "DOUBLE PRECISION",
                "date" => "DATE",
                "time" => "TIME",
                "time_tz" => "TIME WITH TIME ZONE",
                "timestamp" => "TIMESTAMP",
                "timestamp_tz" => "TIMESTAMP WITH TIME ZONE",
                "numeric" => "NUMERIC",
                "decimal" => "DECIMAL",
                "dec16" => "DECFLOAT(16)",
                "dec34" => "DECFLOAT(34)",
                "int128" => "INT128",
                "char" => "CHAR",
                "varchar" => "VARCHAR",
                "binary" => "CHAR",
                "varbinary" => "VARCHAR",
                "blob_binary" => "BLOB SUB_TYPE 0",
                _ => "BLOB SUB_TYPE TEXT", // blob_text, and a safe catch-all for anything unrecognized.
            };
        }
    }
}
