using RepoDb.Interfaces;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class that is being used to resolve a <see cref="DbField"/> into its equivalent Firebird
    /// column type declaration (e.g. <c>NUMERIC(18,2)</c>, <c>VARCHAR(255)</c>).
    /// </summary>
    public class DbTypeNameToColumnNameResolver : IResolver<DbField, string>
    {
        /// <summary>
        /// Returns the equivalent Firebird column type declaration of the <see cref="DbField"/>.
        /// </summary>
        /// <param name="field">The field whose column type declaration is being resolved.</param>
        /// <returns>The equivalent column type declaration.</returns>
        public virtual string Resolve(DbField field)
        {
            var precision = field.Precision ?? 18;
            var scale = field.Scale ?? 0;
            var size = field.Size ?? 1;

            return field.DatabaseType?.ToLowerInvariant() switch
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
                "numeric" => $"NUMERIC({precision},{scale})",
                "decimal" => $"DECIMAL({precision},{scale})",
                "dec16" => "DECFLOAT(16)",
                "dec34" => "DECFLOAT(34)",
                "int128" => "INT128",
                "char" => $"CHAR({size})",
                "varchar" => $"VARCHAR({size})",
                "binary" => $"CHAR({size}) CHARACTER SET OCTETS",
                "varbinary" => $"VARCHAR({size}) CHARACTER SET OCTETS",
                "blob_binary" => "BLOB SUB_TYPE 0",
                _ => "BLOB SUB_TYPE TEXT", // blob_text, and a safe catch-all for anything unrecognized.
            };
        }
    }
}
