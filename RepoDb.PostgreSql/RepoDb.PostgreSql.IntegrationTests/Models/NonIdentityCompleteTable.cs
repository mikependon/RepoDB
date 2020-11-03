using System;

namespace RepoDb.PostgreSql.IntegrationTests.Models
{
    public class NonIdentityCompleteTable
    {
        public System.Int64 Id { get; set; }
        public System.Nullable<System.Char> ColumnChar { get; set; }
        //public System.Array ColumnCharAsArray { get; set; }
        //public System.String ColumnAclItem { get; set; }
        //public System.String ColumnAclItemAsArray { get; set; }
        public System.Nullable<System.Int64> ColumnBigInt { get; set; }
        public System.Array ColumnBigIntAsArray { get; set; }
        public System.Nullable<System.Int64> ColumnBigSerial { get; set; }
        //public System.Nullable<System.Boolean> ColumnBit { get; set; }
        //public System.Collections.BitArray ColumnBitVarying { get; set; }
        //public System.Array ColumnBitVaryingAsArray { get; set; }
        //public System.Array ColumnBitAsArray { get; set; }
        public System.Nullable<System.Boolean> ColumnBoolean { get; set; }
        //public System.Array ColumnBooleanAsArray { get; set; }
        //public System.Nullable<NpgsqlTypes.NpgsqlBox> ColumnBox { get; set; }
        //public System.Array ColumnBoxAsArray { get; set; }
        //public System.Byte[] ColumnByteA { get; set; }
        //public System.Array ColumnByteAAsArray { get; set; }
        public System.String ColumnCharacter { get; set; }
        public System.String ColumnCharacterVarying { get; set; }
        //public System.Array ColumnCharacterVaryingAsArray { get; set; }
        //public System.Nullable<System.UInt32> ColumnCid { get; set; }
        //public System.Array ColumnCidAsArray { get; set; }
        ////public System.Nullable<System.ValueTuple`2[[System.Net.IPAddress, System.Net.Primitives, Version = 4.1.1.0, Culture = neutral, PublicKeyToken = b03f5f7f11d50a3a],[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e]]> ColumnCidr { get; set; }
        //public System.Nullable<NpgsqlTypes.NpgsqlCircle> ColumnCircle { get; set; }
        //public System.Array ColumnCircleAsArray { get; set; }
        public System.Nullable<System.DateTime> ColumnDate { get; set; }
        public System.Array ColumnDateAsArray { get; set; }
        ////public System.Nullable<NpgsqlTypes.NpgsqlRange`1[[System.DateTime, System.Private.CoreLib, Version = 4.0.0.0, Culture = neutral, PublicKeyToken = 7cec85d7bea7798e]]> ColumnDateRange { get; set; }
        //public System.Array ColumnDateRangeAsArray { get; set; }
        //public System.Nullable<System.Double> ColumnDoublePrecision { get; set; }
        //public System.Array ColumnDoublePrecisionAsArray { get; set; }
        //public System.String ColumnGtsVector { get; set; }
        //public System.Net.IPAddress ColumnInet { get; set; }
        //public System.Array ColumnInetAsArray { get; set; }
        //public System.Array ColumnInt2Vector { get; set; }
        //public System.Array ColumnInt2VectorAsArray { get; set; }
        ////public System.Nullable<NpgsqlTypes.NpgsqlRange`1[[System.Int32, System.Private.CoreLib, Version = 4.0.0.0, Culture = neutral, PublicKeyToken = 7cec85d7bea7798e]]> ColumnInt4Range { get; set; }
        //public System.Array ColumnInt4RangeAsArray { get; set; }
        ////public System.Nullable<NpgsqlTypes.NpgsqlRange`1[[System.Int64, System.Private.CoreLib, Version = 4.0.0.0, Culture = neutral, PublicKeyToken = 7cec85d7bea7798e]]> ColumnInt8Range { get; set; }
        //public System.Array ColumnInt8RangeAsArray { get; set; }
        public System.Nullable<System.Int32> ColumnInteger { get; set; }
        public System.Array ColumnIntegerAsArray { get; set; }
        public System.Nullable<System.TimeSpan> ColumnInterval { get; set; }
        public System.Array ColumnIntervalAsArray { get; set; }
        //public System.String ColumnJson { get; set; }
        //public System.Array ColumnJsonAsArray { get; set; }
        //public System.String ColumnJsonB { get; set; }
        //public System.Array ColumnJsonBAsArray { get; set; }
        //public System.String ColumnJsonPath { get; set; }
        //public System.String ColumnJsonPathAsArray { get; set; }
        //public System.Nullable<NpgsqlTypes.NpgsqlLine> ColumnLine { get; set; }
        //public System.Array ColumnLineAsArray { get; set; }
        //public System.Nullable<NpgsqlTypes.NpgsqlLSeg> ColumnLSeg { get; set; }
        //public System.Array ColumnLSegAsArray { get; set; }
        //public System.Net.NetworkInformation.PhysicalAddress ColumnMacAddr { get; set; }
        //public System.Array ColumnMacAddrAsArray { get; set; }
        //public System.Net.NetworkInformation.PhysicalAddress ColumnMacAddr8 { get; set; }
        //public System.Array ColumnMacAddr8AsArray { get; set; }
        public System.Nullable<System.Decimal> ColumnMoney { get; set; }
        //public System.Array ColumnMoneyAsArray { get; set; }
        public System.String ColumnName { get; set; }
        //public System.Array ColumnNameAsArray { get; set; }
        //public System.Nullable<System.Decimal> ColumnNumeric { get; set; }
        //public System.Array ColumnNumericAsArray { get; set; }
        ////public System.Nullable<NpgsqlTypes.NpgsqlRange`1[[System.Decimal, System.Private.CoreLib, Version = 4.0.0.0, Culture = neutral, PublicKeyToken = 7cec85d7bea7798e]]> ColumnNumRange { get; set; }
        //public System.Array ColumnNumRangeAsArray { get; set; }
        //public System.Nullable<System.UInt32> ColumnOId { get; set; }
        //public System.Array ColumnOIdAsArray { get; set; }
        //public System.Array ColumnOIdVector { get; set; }
        //public System.Array ColumnOIdVectorAsArray { get; set; }
        //public System.Nullable<NpgsqlTypes.NpgsqlPath> ColumnPath { get; set; }
        //public System.Array ColumnPathAsArray { get; set; }
        //public System.String ColumnPgDependencies { get; set; }
        //public System.String ColumnPgLsn { get; set; }
        //public System.String ColumnPgLsnAsArray { get; set; }
        //public System.String ColumnPgMcvList { get; set; }
        //public System.String ColumnPgNDistinct { get; set; }
        //public System.String ColumnPgNodeTree { get; set; }
        //public System.Nullable<NpgsqlTypes.NpgsqlPoint> ColumnPoint { get; set; }
        //public System.Array ColumnPointAsArray { get; set; }
        //public System.Nullable<NpgsqlTypes.NpgsqlPolygon> ColumnPolygon { get; set; }
        //public System.Array ColumnPolygonAsArray { get; set; }
        public System.Nullable<System.Single> ColumnReal { get; set; }
        //public System.Array ColumnRealAsArray { get; set; }
        //public System.String ColumnRefCursor { get; set; }
        //public System.Array ColumnRefCursorAsArray { get; set; }
        //public System.String ColumnRegClass { get; set; }
        //public System.String ColumnRegClassAsArray { get; set; }
        //public System.Nullable<System.UInt32> ColumnRegConfig { get; set; }
        //public System.Array ColumnRegConfigAsArray { get; set; }
        //public System.String ColumnRegDictionary { get; set; }
        //public System.String ColumnRegDictionaryAsArray { get; set; }
        //public System.String ColumnRegNamespace { get; set; }
        //public System.String ColumnRegNamespaceAsArray { get; set; }
        //public System.String ColumnRegOper { get; set; }
        //public System.String ColumnRegOperAsArray { get; set; }
        //public System.String ColumnRegOperator { get; set; }
        //public System.String ColumnRegOperationAsArray { get; set; }
        //public System.String ColumnRegProc { get; set; }
        //public System.String ColumnRegProcAsArray { get; set; }
        //public System.String ColumnRegProcedure { get; set; }
        //public System.String ColumnRegProcedureAsArray { get; set; }
        //public System.String ColumnRegRole { get; set; }
        //public System.String ColumnRegRoleAsArray { get; set; }
        //public System.Nullable<System.UInt32> ColumnRegType { get; set; }
        //public System.Array ColumnRegTypeAsArray { get; set; }
        //public System.Nullable<System.Int32> ColumnSerial { get; set; }
        public System.Nullable<System.Int16> ColumnSmallInt { get; set; }
        //public System.Array ColumnSmallIntAsArray { get; set; }
        //public System.Nullable<System.Int16> ColumnSmallSerial { get; set; }
        public System.String ColumnText { get; set; }
        //public System.Array ColumnTextAsArray { get; set; }
        //public System.Nullable<NpgsqlTypes.NpgsqlTid> ColumnTId { get; set; }
        //public System.Array ColumnTidAsArray { get; set; }
        //public System.Array ColumnTimeWithTimeZoneAsArray { get; set; }
        //public System.Nullable<System.DateTimeOffset> ColumnTimeWithTimeZone { get; set; }
        //public System.Nullable<System.TimeSpan> ColumnTimeWithoutTimeZone { get; set; }
        //public System.Array ColumnTimeWithoutTimeZoneAsArray { get; set; }
        public System.Nullable<System.DateTime> ColumnTimestampWithTimeZone { get; set; }
        //public System.Array ColumnTimestampWithTimeZoneAsArray { get; set; }
        public System.Nullable<System.DateTime> ColumnTimestampWithoutTimeZone { get; set; }
        //public System.Array ColumnTimestampWithoutTimeZoneAsArray { get; set; }
        //public NpgsqlTypes.NpgsqlTsQuery ColumnTSQuery { get; set; }
        //public System.Array ColumnTSQueryAsArray { get; set; }
        ////public System.Nullable<NpgsqlTypes.NpgsqlRange`1[[System.DateTime, System.Private.CoreLib, Version = 4.0.0.0, Culture = neutral, PublicKeyToken = 7cec85d7bea7798e]]> ColumnTSRange { get; set; }
        //public System.Array ColumnTSRangeAsArray { get; set; }
        ////public System.Nullable<NpgsqlTypes.NpgsqlRange`1[[System.DateTime, System.Private.CoreLib, Version = 4.0.0.0, Culture = neutral, PublicKeyToken = 7cec85d7bea7798e]]> ColumnTSTZRange { get; set; }
        //public System.Array ColumnTSTZRangeAsArray { get; set; }
        //public NpgsqlTypes.NpgsqlTsVector ColumnTSVector { get; set; }
        //public System.Array ColumnTSVectorAsArray { get; set; }
        //public System.String ColumnTXIDSnapshot { get; set; }
        //public System.String ColumnTXIDSnapshotAsArray { get; set; }
        //public System.Nullable<System.Guid> ColumnUUID { get; set; }
        //public System.Array ColumnUUIDAsArray { get; set; }
        //public System.Nullable<System.UInt32> ColumnXID { get; set; }
        //public System.Array ColumnXIDAsArray { get; set; }
        //public System.String ColumnXML { get; set; }
        //public System.Array ColumnXMLAsArray { get; set; }
    }
}
