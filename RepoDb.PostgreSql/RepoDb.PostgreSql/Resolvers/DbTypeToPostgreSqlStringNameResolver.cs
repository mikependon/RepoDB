using RepoDb.Interfaces;
using System.Data;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class used to resolve the <see cref="DbType"/> into its equivalent database string name.
    /// </summary>
    public class DbTypeToPostgreSqlStringNameResolver : IResolver<DbType, string>
    {
        /// <summary>
        /// Returns the equivalent <see cref="DbType"/> of the .NET CLR Types.
        /// </summary>
        /// <param name="dbType">The type of the database.</param>
        /// <returns>The equivalent string name.</returns>
        public virtual string Resolve(DbType dbType)
        {
            /*
            Id = System.Int64
            ColumnChar = System.Char
            ColumnCharAsArray = System.Array
            ColumnAclItem = System.String
            ColumnAclItemAsArray = System.String
            ColumnBigInt = System.Int64
            ColumnBigIntAsArray = System.Array
            ColumnBigSerial = System.Int64
            ColumnBit = System.Boolean
            ColumnBitVarying = System.Collections.BitArray
            ColumnBitVaryingAsArray = System.Array
            ColumnBitAsArray = System.Array
            ColumnBoolean = System.Boolean
            ColumnBooleanAsArray = System.Array
            ColumnBox = NpgsqlTypes.NpgsqlBox
            ColumnBoxAsArray = System.Array
            ColumnByteA = System.Byte[]
            ColumnByteAAsArray = System.Array
            ColumnCharacter = System.String
            ColumnCharacterVarying = System.String
            ColumnCharacterVaryingAsArray = System.Array
            ColumnCid = System.UInt32
            ColumnCidAsArray = System.Array
            ColumnCidr = System.ValueTuple`2[[System.Net.IPAddress, System.Net.Primitives, Version=4.1.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e]]
            ColumnCircle = NpgsqlTypes.NpgsqlCircle
            ColumnCircleAsArray = System.Array
            ColumnDate = System.DateTime
            ColumnDateAsArray = System.Array
            ColumnDateRange = NpgsqlTypes.NpgsqlRange`1[[System.DateTime, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e]]
            ColumnDateRangeAsArray = System.Array
            ColumnDoublePrecision = System.Double
            ColumnDoublePrecisionAsArray = System.Array
            ColumnGtsVector = System.String
            ColumnInet = System.Net.IPAddress
            ColumnInetAsArray = System.Array
            ColumnInt2Vector = System.Array
            ColumnInt2VectorAsArray = System.Array
            ColumnInt4Range = NpgsqlTypes.NpgsqlRange`1[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e]]
            ColumnInt4RangeAsArray = System.Array
            ColumnInt8Range = NpgsqlTypes.NpgsqlRange`1[[System.Int64, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e]]
            ColumnInt8RangeAsArray = System.Array
            ColumnInteger = System.Int32
            ColumnIntegerAsArray = System.Array
            ColumnInterval = System.TimeSpan
            ColumnIntervalAsArray = System.Array
            ColumnJson = System.String
            ColumnJsonAsArray = System.Array
            ColumnJsonB = System.String
            ColumnJsonBAsArray = System.Array
            ColumnJsonPath = System.String
            ColumnJsonPathAsArray = System.String
            ColumnLine = NpgsqlTypes.NpgsqlLine
            ColumnLineAsArray = System.Array
            ColumnLSeg = NpgsqlTypes.NpgsqlLSeg
            ColumnLSegAsArray = System.Array
            ColumnMacAddr = System.Net.NetworkInformation.PhysicalAddress
            ColumnMacAddrAsArray = System.Array
            ColumnMacAddr8 = System.Net.NetworkInformation.PhysicalAddress
            ColumnMacAddr8AsArray = System.Array
            ColumnMoney = System.Decimal
            ColumnMoneyAsArray = System.Array
            ColumnName = System.String
            ColumnNameAsArray = System.Array
            ColumnNumeric = System.Decimal
            ColumnNumericAsArray = System.Array
            ColumnNumRange = NpgsqlTypes.NpgsqlRange`1[[System.Decimal, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e]]
            ColumnNumRangeAsArray = System.Array
            ColumnOId = System.UInt32
            ColumnOIdAsArray = System.Array
            ColumnOIdVector = System.Array
            ColumnOIdVectorAsArray = System.Array
            ColumnPath = NpgsqlTypes.NpgsqlPath
            ColumnPathAsArray = System.Array
            ColumnPgDependencies = System.String
            ColumnPgLsn = System.String
            ColumnPgLsnAsArray = System.String
            ColumnPgMcvList = System.String
            ColumnPgNDistinct = System.String
            ColumnPgNodeTree = System.String
            ColumnPoint = NpgsqlTypes.NpgsqlPoint
            ColumnPointAsArray = System.Array
            ColumnPolygon = NpgsqlTypes.NpgsqlPolygon
            ColumnPolygonAsArray = System.Array
            ColumnReal = System.Single
            ColumnRealAsArray = System.Array
            ColumnRefCursor = System.String
            ColumnRefCursorAsArray = System.Array
            ColumnRegClass = System.String
            ColumnRegClassAsArray = System.String
            ColumnRegConfig = System.UInt32
            ColumnRegConfigAsArray = System.Array
            ColumnRegDictionary = System.String
            ColumnRegDictionaryAsArray = System.String
            ColumnRegNamespace = System.String
            ColumnRegNamespaceAsArray = System.String
            ColumnRegOper = System.String
            ColumnRegOperAsArray = System.String
            ColumnRegOperator = System.String
            ColumnRegOperationAsArray = System.String
            ColumnRegProc = System.String
            ColumnRegProcAsArray = System.String
            ColumnRegProcedure = System.String
            ColumnRegProcedureAsArray = System.String
            ColumnRegRole = System.String
            ColumnRegRoleAsArray = System.String
            ColumnRegType = System.UInt32
            ColumnRegTypeAsArray = System.Array
            ColumnSerial = System.Int32
            ColumnSmallInt = System.Int16
            ColumnSmallIntAsArray = System.Array
            ColumnSmallSerial = System.Int16
            ColumnText = System.String
            ColumnTextAsArray = System.Array
            ColumnTId = NpgsqlTypes.NpgsqlTid
            ColumnTidAsArray = System.Array
            ColumnTimeWithTimeZoneAsArray = System.Array
            ColumnTimeWithTimeZone = System.DateTimeOffset
            ColumnTimeWithoutTimeZone = System.TimeSpan
            ColumnTimeWithoutTimeZoneAsArray = System.Array
            ColumnTimestampWithTimeZone = System.Array
            ColumnTimestampWithTimeZoneAsArray = System.Array
            ColumnTimestampWithoutTimeZone = System.DateTime
            ColumnTimestampWithoutTimeZoneAsArray = System.Array
            ColumnTSQuery = NpgsqlTypes.NpgsqlTsQuery
            ColumnTSQueryAsArray = System.Array
            ColumnTSRange = NpgsqlTypes.NpgsqlRange`1[[System.DateTime, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e]]
            ColumnTSRangeAsArray = System.Array
            ColumnTSTZRange = NpgsqlTypes.NpgsqlRange`1[[System.DateTime, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e]]
            ColumnTSTZRangeAsArray = System.Array
            ColumnTSVector = NpgsqlTypes.NpgsqlTsVector
            ColumnTSVectorAsArray = System.Array
            ColumnTXIDSnapshot = System.String
            ColumnTXIDSnapshotAsArray = System.String
            ColumnUUID = System.Guid
            ColumnUUIDAsArray = System.Array
            ColumnXID = System.UInt32
            ColumnXIDAsArray = System.Array
            ColumnXML = System.String
            ColumnXMLAsArray = System.Array
            */
            switch (dbType)
            {
                case DbType.Int64:
                    return "BIGINT";
                case DbType.Binary:
                case DbType.Byte:
                    return "BYTEA";
                case DbType.Boolean:
                    return "BOOLEAN";
                case DbType.AnsiString:
                case DbType.AnsiStringFixedLength:
                case DbType.String:
                case DbType.StringFixedLength:
                    return "TEXT";
                case DbType.Date:
                case DbType.DateTime:
                case DbType.DateTime2:
                case DbType.DateTimeOffset:
                    return "DATE";
                case DbType.Decimal:
                    return "NUMERIC";
                case DbType.Single:
                    return "REAL";
                case DbType.Double:
                    return "DOUBLE PRECISION";
                case DbType.Int32:
                    return "INTEGER";
                case DbType.Int16:
                    return "SMALLINT";
                case DbType.Time:
                    return "INTERVAL";
                default:
                    /* DbType.Guid
                     * DbType.Xml
                     * DbType.Object
                     */
                    return "TEXT";
            }
        }
    }
}
