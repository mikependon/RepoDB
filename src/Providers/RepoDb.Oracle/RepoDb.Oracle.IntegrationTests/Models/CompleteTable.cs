#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Oracle.ManagedDataAccess.Client;
using RepoDb.Attributes.Parameter.Oracle;

namespace RepoDb.Oracle.IntegrationTests.Models
{
    public class CompleteTable
    {
        public System.Int32 Id { get; set; }
        public System.Guid SessionId { get; set; }
        public System.String ColumnVarchar { get; set; }
        public System.Decimal ColumnNumber { get; set; }
        public System.DateTime ColumnDate { get; set; }
        public System.DateTime ColumnTimestamp { get; set; }

        // Additional character types (VARCHAR2 already covered above via "ColumnVarchar").
        public System.String ColumnVarchar2 { get; set; }
        public System.String ColumnChar { get; set; }
        public System.String ColumnNChar { get; set; }

        // Additional NUMBER precisions, mapped to narrower CLR integral types.
        public System.Int32 ColumnInt { get; set; }
        public System.Int64 ColumnBigInt { get; set; }
        public System.Int16 ColumnSmallInt { get; set; }
        public System.Byte ColumnTinyInt { get; set; }

        // Native Oracle floating-point types (distinct from NUMBER-backed "ColumnNumber").
        public System.Single ColumnBinaryFloat { get; set; }
        public System.Double ColumnBinaryDouble { get; set; }

        // Time-zone-aware timestamps (DATE/TIMESTAMP without a zone are already covered above).
        // "ColumnTimestampTz" needs no attribute - RepoDb.Core's ClientTypeToDbTypeResolver maps
        // DateTimeOffset -> DbType.DateTimeOffset, and ODP.NET's own DbType->OracleDbType table maps
        // that to the correct OracleDbType.TimeStampTZ. "ColumnTimestampLtz" DOES need one, though:
        // RepoDb.Core has no separate DbType for "with local time zone" - it's still DateTimeOffset
        // at the CLR level - so without an explicit override it would bind as TimeStampTZ too,
        // silently mismatching the actual WITH LOCAL TIME ZONE column type.
        public System.DateTimeOffset ColumnTimestampTz { get; set; }

        [OracleDbType(OracleDbType.TimeStampLTZ)]
        public System.DateTimeOffset ColumnTimestampLtz { get; set; }

        // A second RAW column (SessionId already exercises RAW(16) via the Guid property handler).
        public System.Byte[] ColumnRaw { get; set; }

        // Large object types. ODP.NET's default OracleDbType inference for a plain string/byte[]
        // parameter is Varchar2/Raw, which is bind-size-limited - explicitly typing these as
        // Clob/NClob/Blob is what actually exercises LOB binding rather than silently falling back
        // to an in-line VARCHAR2/RAW bind for short values.
        [OracleDbType(OracleDbType.Clob)]
        public System.String ColumnClob { get; set; }

        [OracleDbType(OracleDbType.NClob)]
        public System.String ColumnNClob { get; set; }

        [OracleDbType(OracleDbType.Blob)]
        public System.Byte[] ColumnBlob { get; set; }

        // INTERVAL DAY TO SECOND maps naturally to TimeSpan (unlike INTERVAL YEAR TO MONTH, whose
        // variable month-length semantics don't have a faithful fixed-duration CLR equivalent -
        // deliberately not included here). REQUIRES the explicit attribute below: RepoDb.Core's
        // ClientTypeToDbTypeResolver maps TimeSpan -> DbType.Time, and ODP.NET's own DbType ->
        // OracleDbType inference table maps DbType.Time -> OracleDbType.TimeStamp (not IntervalDS).
        // Since RepoDb.Core sets Value first and then DbType (the last setting wins per ODP.NET's
        // own rules), that silently clobbers ODP.NET's correct auto-inferred IntervalDS with
        // TimeStamp - and because TimeSpan doesn't implement IConvertible, ODP.NET can't coerce it
        // to the (now wrong) TimeStamp bind type either, so it fails validation before the command
        // ever reaches the server (System.ArgumentException: "ORA-50028: Invalid parameter binding").
        // The explicit attribute is applied after DbType in RepoDb.Core's parameter-assignment
        // order, so it correctly overrides the bad inference back to IntervalDS.
        [OracleDbType(OracleDbType.IntervalDS)]
        public System.TimeSpan ColumnIntervalDs { get; set; }

        // XMLType round-trips as a plain string through ODP.NET. Deliberately NOT attributed with
        // [OracleDbType(OracleDbType.XmlType)] - that enum value is documented as "Not Available in
        // ODP.NET, Managed Driver" (Oracle.ManagedDataAccess, which is what this provider uses) on
        // the driver version this project is pinned to, and using it threw a client-side
        // ORA-50028 "Invalid parameter binding" ArgumentException before the command ever reached
        // the server. Binding as a plain, untyped string and letting Oracle implicitly convert
        // VARCHAR2 -> XMLTYPE on INSERT is the standard workaround and needs no attribute at all.
        public System.String ColumnXml { get; set; }

        // NOTE: a native Oracle 23c+ BOOLEAN column was attempted here (bound via
        // [OracleDbType(OracleDbType.Boolean)]) but removed - that enum value is likewise documented
        // as "Not Available in ODP.NET, Managed Driver" and, unlike XMLTYPE, there's no implicit
        // string/numeric-to-BOOLEAN conversion in Oracle DML to fall back on, so it was dropped
        // entirely rather than worked around. See project notes for details.
    }
}
