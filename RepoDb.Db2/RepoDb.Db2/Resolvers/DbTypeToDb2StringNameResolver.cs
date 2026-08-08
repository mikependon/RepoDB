using RepoDb.Interfaces;
using System.Data;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class used to resolve the <see cref="DbType"/> into its equivalent Db2 database string name.
    /// </summary>
    public class DbTypeToDb2StringNameResolver : IResolver<DbType, string>
    {
        /*
         * Taken:
         * https://www.ibm.com/docs/en/db2/11.5?topic=elements-data-types
         */

        /// <summary>
        /// Returns the equivalent <see cref="DbType"/> of the .NET CLR Types.
        /// </summary>
        /// <param name="dbType">The type of the database.</param>
        /// <returns>The equivalent string name.</returns>
        public virtual string Resolve(DbType dbType)
        {
            return dbType switch
            {
                DbType.Int64 => "BIGINT",
                DbType.Binary => "BLOB(1M)",
                // Db2 has no native BOOLEAN-in-CAST-expression guarantee across all versions (it
                // was only added in 11.1), so a portable 0/1 SMALLINT is used instead - same choice
                // made for "ColumnTinyInt"-style columns elsewhere in this provider.
                DbType.Boolean => "SMALLINT",
                DbType.String => "VARCHAR(2000)",
                // Db2's DATE type holds no time-of-day component (unlike Oracle's DATE, which
                // always carries one) - a plain .NET DateTime is therefore cast to TIMESTAMP, not
                // DATE, to avoid silently truncating the time portion. DbType.Date (which
                // represents a date-only value) still maps to DATE.
                DbType.Date => "DATE",
                DbType.DateTime => "TIMESTAMP",
                DbType.DateTime2 => "TIMESTAMP",
                // The IBM.Data.Db2 DB2Type enumeration has no timezone-aware member at all: every
                // Date/Time/Timestamp member maps only to a plain DateTime/TimeSpan. "TIMESTAMP" is
                // used here on a best-effort basis; the offset itself is not preserved.
                DbType.DateTimeOffset => "TIMESTAMP",
                DbType.Decimal => "DECIMAL(18,2)",
                DbType.Single => "REAL",
                DbType.Double => "DOUBLE",
                DbType.Int32 => "INTEGER",
                DbType.Int16 => "SMALLINT",
                // Db2's TIME type has no sub-second precision at all, unlike Oracle's INTERVAL DAY
                // TO SECOND workaround for a fractional TimeSpan - there is no lossless Db2
                // equivalent for a fractional-second duration, so this deliberately accepts the
                // whole-second-only limitation rather than casting to a type that doesn't exist.
                DbType.Time => "TIME",
                // Db2 for Linux/UNIX/Windows has no native 8-bit TINYINT type; SMALLINT is the
                // smallest built-in integer type.
                DbType.Byte => "SMALLINT",
                // Db2 has no native GUID/UNIQUEIDENTIFIER type; the idiomatic storage for one is a
                // fixed-length 16-byte "CHAR(16) FOR BIT DATA" column.
                DbType.Guid => "CHAR(16) FOR BIT DATA",
                DbType.AnsiString => "VARCHAR(2000)",
                // Db2 CHAR's maximum length is 254 bytes (unlike Oracle's CHAR, whose maximum is
                // 2000) - capped at that documented maximum rather than emitting an invalid DDL/CAST
                // target.
                DbType.AnsiStringFixedLength => "CHAR(254)",
                // Db2 has no "NCHAR" type; GRAPHIC is its fixed-length double-byte/graphic string
                // type. GRAPHIC's maximum length is 127 (double-byte) characters.
                DbType.StringFixedLength => "GRAPHIC(127)",
                DbType.Object => "BLOB(1M)",
                // Db2's native XML type (unlike Oracle's XMLTYPE, which is a distinct object type).
                DbType.Xml => "XML",
                _ => "VARCHAR(2000)",
            };
        }
    }
}
