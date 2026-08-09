using IBM.Data.Db2;
using RepoDb.Attributes.Parameter.Db2;

namespace RepoDb.Db2.IntegrationTests.Models
{
    public class CompleteTable
    {
        public System.Int32 Id { get; set; }

        // Db2 has no native GUID/UNIQUEIDENTIFIER type; "SessionId" is stored as a 16-byte
        // "CHAR(16) FOR BIT DATA" column and converted via the Guid<->byte[] PropertyHandler
        // registered in Setup/Database.cs.
        public System.Guid SessionId { get; set; }

        public System.String ColumnVarchar { get; set; }
        public System.Decimal ColumnNumber { get; set; }
        public System.DateTime ColumnDate { get; set; }
        public System.DateTime ColumnTimestamp { get; set; }

        // A second, differently-sized VARCHAR column. Unlike Oracle, Db2 has no distinct "VARCHAR2"
        // type - a plain VARCHAR already stores whatever the database's configured code page/encoding
        // is (typically UTF-8), so there's nothing separate to exercise here besides another size.
        public System.String ColumnVarchar2 { get; set; }

        public System.String ColumnChar { get; set; }

        // Db2's GRAPHIC type is its fixed-length double-byte/graphic string type - there is no type
        // literally named "NCHAR" the way Oracle has one. Needs an explicit attribute: a plain string
        // property would otherwise default to VarChar/Char binding, not Graphic.
        [Db2Type(DB2Type.Graphic)]
        public System.String ColumnNChar { get; set; }

        // Additional integer precisions.
        public System.Int32 ColumnInt { get; set; }
        public System.Int64 ColumnBigInt { get; set; }
        public System.Int16 ColumnSmallInt { get; set; }

        // Db2 for Linux/UNIX/Windows has no native 8-bit TINYINT data type - SMALLINT (16-bit) is the
        // smallest built-in integer type, so a CLR byte is bound explicitly as SmallInt here.
        [Db2Type(DB2Type.SmallInt)]
        public System.Byte ColumnTinyInt { get; set; }

        // Native Db2 floating-point types (distinct from the DECIMAL-backed "ColumnNumber").
        [Db2Type(DB2Type.Real)]
        public System.Single ColumnBinaryFloat { get; set; }

        [Db2Type(DB2Type.Double)]
        public System.Double ColumnBinaryDouble { get; set; }

        // A second binary column (SessionId already exercises the binary/Guid path via the property
        // handler, above). Stored as "VARCHAR(500) FOR BIT DATA" - the traditional, version-portable
        // way to declare a variable-length binary column in Db2 (as opposed to the newer, dedicated
        // VARBINARY type introduced in Db2 11.1). DB2Type.Binary is the generic byte[] binding that
        // covers the whole CHAR/VARCHAR-FOR-BIT-DATA family; a plain byte[] property would otherwise
        // default to Blob binding instead.
        [Db2Type(DB2Type.Binary)]
        public System.Byte[] ColumnRaw { get; set; }

        // NOTE: "ColumnClob"/"ColumnNClob"/"ColumnBlob" (CLOB/DBCLOB/BLOB) and "ColumnXml" (Db2's
        // native XML column type) were removed from this test fixture - see the "Known limitations"
        // section of RepoDb.Db2/README.md for why. Insert/Update/Query against columns of these
        // types are unaffected by this and still work the same way they always did for any
        // RepoDb.Db2 consumer with columns of their own.

        // NOTE: unlike Oracle, Db2 has no "TIMESTAMP ... WITH LOCAL TIME ZONE" variant and no ANSI
        // INTERVAL data type at all (Db2 supports labeled-duration arithmetic, e.g. "date + 1 DAY",
        // but there is no storable INTERVAL column type). The DB2Type enumeration likewise has no
        // timezone-aware or interval member - every Date/Time/Timestamp member maps only to a plain
        // DateTime/TimeSpan (see IBM's DB2Type enumeration reference). Columns that mirrored Oracle's
        // "ColumnTimestampTz"/"ColumnTimestampLtz"/"ColumnIntervalDs" were deliberately removed here
        // rather than faked with a type that doesn't actually exist for this provider/driver.
    }
}
