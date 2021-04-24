using RepoDb.Interfaces;
using System;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class used to resolve the PostgreSql Database Types into its equivalent .NET CLR Types.
    /// </summary>
    public class PostgreSqlDbTypeNameToClientTypeResolver : IResolver<string, Type>
    {
        /// <summary>
        /// Returns the equivalent .NET CLR Types of the Database Type.
        /// </summary>
        /// <param name="dbTypeName">The name of the database type.</param>
        /// <returns>The equivalent .NET CLR type.</returns>
        public virtual Type Resolve(string dbTypeName)
        {
            if (dbTypeName == null)
            {
                throw new NullReferenceException("The DB Type name must not be null.");
            }
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
            return dbTypeName.ToLowerInvariant() switch
            {
                "bigint" => typeof(System.Int64),
                "char" or "\"char\"" => typeof(System.Char),
                "array" => typeof(System.Array),
                "character" or "character varying" or "json" or "jsonb" or "jsonpath" or "name" or "pg_dependencies" or "pg_lsn" or "pg_mcv_list" or "pg_ndistinct" or "pg_node_tree" or "refcursor" or "regclass" or "regdictionary" or "regnamespace" or "regoper" or "regoperator" or "regproc" or "regprocedure" or "regrole" or "text" or "txid_snapshot" or "xml" => typeof(System.String),
                "bit" or "boolean" => typeof(System.Boolean),
                "bit varying" => typeof(System.Collections.BitArray),
                "box" => typeof(NpgsqlTypes.NpgsqlBox),
                "bytea" => typeof(System.Byte[]),
                "cid" or "oid" or "regconfig" or "regtype" or "xid" => typeof(System.UInt32),
                "circle" => typeof(NpgsqlTypes.NpgsqlCircle),
                "date" or "timestamp without time zone" or "timestamp" or "timestamp with time zone" or "timestamptz" => typeof(System.DateTime),
                "double precision" => typeof(System.Double),
                "inet" => typeof(System.Net.IPAddress),
                "integer" => typeof(System.Int32),
                "interval" or "time without time zone" or "time" => typeof(System.TimeSpan),
                "line" => typeof(NpgsqlTypes.NpgsqlLine),
                "lseg" => typeof(NpgsqlTypes.NpgsqlLSeg),
                "macaddr" or "macaddr8" => typeof(System.Net.NetworkInformation.PhysicalAddress),
                "money" or "numeric" => typeof(System.Decimal),
                "path" => typeof(NpgsqlTypes.NpgsqlPath),
                "point" => typeof(NpgsqlTypes.NpgsqlPoint),
                "polygon" => typeof(NpgsqlTypes.NpgsqlPolygon),
                "real" => typeof(System.Single),
                "smallint" => typeof(System.Int16),
                "tid" => typeof(NpgsqlTypes.NpgsqlTid),
                "timetz" or "time with time zone" => typeof(System.DateTimeOffset),
                "tsquery" => typeof(NpgsqlTypes.NpgsqlTsQuery),
                "tsvector" => typeof(NpgsqlTypes.NpgsqlTsVector),
                "uuid" => typeof(System.Guid),
                _ => typeof(object),
            };
        }
    }
}
