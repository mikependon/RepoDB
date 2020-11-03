using RepoDb.PostgreSql.IntegrationTests.Models;
using Npgsql;
using System;
using System.Collections.Generic;

namespace RepoDb.PostgreSql.IntegrationTests.Setup
{
    public static class Database
    {
        #region Properties

        /// <summary>
        /// Gets or sets the connection string to be used for Postgres database.
        /// </summary>
        public static string ConnectionStringForPosgres { get; private set; }

        /// <summary>
        /// Gets or sets the connection string to be used.
        /// </summary>
        public static string ConnectionString { get; private set; }

        #endregion

        #region Methods

        public static void Initialize()
        {
            // Check the connection string
            var connectionStringForPosgres = Environment.GetEnvironmentVariable("REPODB_CONSTR_POSTGRESDB", EnvironmentVariableTarget.Process);
            var connectionString = Environment.GetEnvironmentVariable("REPODB_CONSTR", EnvironmentVariableTarget.Process);

            // Master connection
            ConnectionStringForPosgres = (connectionStringForPosgres ?? "Server=127.0.0.1;Port=5432;Database=postgres;User Id=postgres;Password=Password123;");

            // RepoDb connection
            ConnectionString = (connectionString ?? "Server=127.0.0.1;Port=5432;Database=RepoDb;User Id=postgres;Password=Password123;");

            // Initialize PostgreSql
            PostgreSqlBootstrap.Initialize();

            // Create databases
            CreateDatabase();

            // Create tables
            CreateTables();
        }

        public static void Cleanup()
        {
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Truncate<CompleteTable>();
                connection.Truncate<NonIdentityCompleteTable>();
            }
        }

        #endregion

        #region CreateDatabases

        private static void CreateDatabase()
        {
            using (var connection = new NpgsqlConnection(ConnectionStringForPosgres))
            {
                var recordCount = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM pg_database WHERE datname = 'RepoDb';");
                if (recordCount <= 0)
                {
                    connection.ExecuteNonQuery(@"CREATE DATABASE ""RepoDb""
                        WITH OWNER = ""postgres""
                        ENCODING = ""UTF8""
                        CONNECTION LIMIT = -1;");
                }
            }
        }

        #endregion

        #region CreateTables

        private static void CreateTables()
        {
            CreateCompleteTable();
            CreateNonIdentityCompleteTable();
        }

        private static void CreateCompleteTable()
        {
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS public.""CompleteTable""
                    (
                        ""Id"" bigint GENERATED ALWAYS AS IDENTITY,
                        ""ColumnChar"" ""char"",
                        ""ColumnCharAsArray"" ""char""[],
                        --""ColumnAclItem"" aclitem,
                        --""ColumnAclItemAsArray"" aclitem[],
                        ""ColumnBigInt"" bigint,
                        ""ColumnBigIntAsArray"" bigint[],
                        ""ColumnBigSerial"" bigint,
                        ""ColumnBit"" bit(1),
                        ""ColumnBitVarying"" bit varying,
                        ""ColumnBitVaryingAsArray"" bit varying[],
                        ""ColumnBitAsArray"" bit(1)[],
                        ""ColumnBoolean"" boolean,
                        ""ColumnBooleanAsArray"" boolean[],
                        ""ColumnBox"" box,
                        ""ColumnBoxAsArray"" box[],
                        ""ColumnByteA"" bytea,
                        ""ColumnByteAAsArray"" bytea[],
                        ""ColumnCharacter"" character(1) COLLATE pg_catalog.""default"",
                        ""ColumnCharacterVarying"" character varying COLLATE pg_catalog.""default"",
                        ""ColumnCharacterVaryingAsArray"" character varying[] COLLATE pg_catalog.""default"",
                        ""ColumnCid"" cid,
                        ""ColumnCidAsArray"" cid[],
                        ""ColumnCidr"" cidr,
                        ""ColumnCircle"" circle,
                        ""ColumnCircleAsArray"" circle[],
                        ""ColumnDate"" date,
                        ""ColumnDateAsArray"" date[],
                        ""ColumnDateRange"" daterange,
                        ""ColumnDateRangeAsArray"" daterange[],
                        ""ColumnDoublePrecision"" double precision,
                        ""ColumnDoublePrecisionAsArray"" double precision[],
                        --""ColumnGtsVector"" gtsvector,
                        ""ColumnInet"" inet,
                        ""ColumnInetAsArray"" inet[],
                        ""ColumnInt2Vector"" int2vector,
                        ""ColumnInt2VectorAsArray"" int2vector[],
                        ""ColumnInt4Range"" int4range,
                        ""ColumnInt4RangeAsArray"" int4range[],
                        ""ColumnInt8Range"" int8range,
                        ""ColumnInt8RangeAsArray"" int8range[],
                        ""ColumnInteger"" integer,
                        ""ColumnIntegerAsArray"" integer[],
                        ""ColumnInterval"" interval,
                        ""ColumnIntervalAsArray"" interval[],
                        ""ColumnJson"" json,
                        ""ColumnJsonAsArray"" json[],
                        ""ColumnJsonB"" jsonb,
                        ""ColumnJsonBAsArray"" jsonb[],
                        ""ColumnJsonPath"" jsonpath,
                        ""ColumnJsonPathAsArray"" jsonpath[],
                        ""ColumnLine"" line,
                        ""ColumnLineAsArray"" line[],
                        ""ColumnLSeg"" lseg,
                        ""ColumnLSegAsArray"" lseg[],
                        ""ColumnMacAddr"" macaddr,
                        ""ColumnMacAddrAsArray"" macaddr[],
                        ""ColumnMacAddr8"" macaddr8,
                        ""ColumnMacAddr8AsArray"" macaddr8[],
                        ""ColumnMoney"" money,
                        ""ColumnMoneyAsArray"" money[],
                        ""ColumnName"" name COLLATE pg_catalog.""C"",
                        ""ColumnNameAsArray"" name[] COLLATE pg_catalog.""C"",
                        ""ColumnNumeric"" numeric,
                        ""ColumnNumericAsArray"" numeric[],
                        ""ColumnNumRange"" numrange,
                        ""ColumnNumRangeAsArray"" numrange[],
                        ""ColumnOId"" oid,
                        ""ColumnOIdAsArray"" oid[],
                        ""ColumnOIdVector"" oidvector,
                        ""ColumnOIdVectorAsArray"" oidvector[],
                        ""ColumnPath"" path,
                        ""ColumnPathAsArray"" path[],
                        ""ColumnPgDependencies"" pg_dependencies COLLATE pg_catalog.""default"",
                        ""ColumnPgLsn"" pg_lsn,
                        ""ColumnPgLsnAsArray"" pg_lsn[],
                        ""ColumnPgMcvList"" pg_mcv_list COLLATE pg_catalog.""default"",
                        ""ColumnPgNDistinct"" pg_ndistinct COLLATE pg_catalog.""default"",
                        ""ColumnPgNodeTree"" pg_node_tree COLLATE pg_catalog.""default"",
                        ""ColumnPoint"" point,
                        ""ColumnPointAsArray"" point[],
                        ""ColumnPolygon"" polygon,
                        ""ColumnPolygonAsArray"" polygon[],
                        ""ColumnReal"" real,
                        ""ColumnRealAsArray"" real[],
                        ""ColumnRefCursor"" refcursor,
                        ""ColumnRefCursorAsArray"" refcursor[],
                        ""ColumnRegClass"" regclass,
                        ""ColumnRegClassAsArray"" regclass[],
                        ""ColumnRegConfig"" regconfig,
                        ""ColumnRegConfigAsArray"" regconfig[],
                        ""ColumnRegDictionary"" regdictionary,
                        ""ColumnRegDictionaryAsArray"" regdictionary[],
                        ""ColumnRegNamespace"" regnamespace,
                        ""ColumnRegNamespaceAsArray"" regnamespace[],
                        ""ColumnRegOper"" regoper,
                        ""ColumnRegOperAsArray"" regoper[],
                        ""ColumnRegOperator"" regoperator,
                        ""ColumnRegOperationAsArray"" regoperator[],
                        ""ColumnRegProc"" regproc,
                        ""ColumnRegProcAsArray"" regproc[],
                        ""ColumnRegProcedure"" regprocedure,
                        ""ColumnRegProcedureAsArray"" regprocedure[],
                        ""ColumnRegRole"" regrole,
                        ""ColumnRegRoleAsArray"" regrole[],
                        ""ColumnRegType"" regtype,
                        ""ColumnRegTypeAsArray"" regtype[],
                        ""ColumnSerial"" integer,
                        ""ColumnSmallInt"" smallint,
                        ""ColumnSmallIntAsArray"" smallint[],
                        ""ColumnSmallSerial"" smallint,
                        ""ColumnText"" text COLLATE pg_catalog.""default"",
                        ""ColumnTextAsArray"" text[] COLLATE pg_catalog.""default"",
                        ""ColumnTId"" tid,
                        ""ColumnTidAsArray"" tid[],
                        ""ColumnTimeWithTimeZoneAsArray"" time with time zone[],
                        ""ColumnTimeWithTimeZone"" time with time zone,
                        ""ColumnTimeWithoutTimeZone"" time without time zone,
                        ""ColumnTimeWithoutTimeZoneAsArray"" time without time zone[],
                        ""ColumnTimestampWithTimeZone"" timestamp with time zone,
                        ""ColumnTimestampWithTimeZoneAsArray"" timestamp with time zone[],
                        ""ColumnTimestampWithoutTimeZone"" timestamp without time zone,
                        ""ColumnTimestampWithoutTimeZoneAsArray"" timestamp without time zone[],
                        ""ColumnTSQuery"" tsquery,
                        ""ColumnTSQueryAsArray"" tsquery[],
                        ""ColumnTSRange"" tsrange,
                        ""ColumnTSRangeAsArray"" tsrange[],
                        ""ColumnTSTZRange"" tstzrange,
                        ""ColumnTSTZRangeAsArray"" tstzrange[],
                        ""ColumnTSVector"" tsvector,
                        ""ColumnTSVectorAsArray"" tsvector[],
                        ""ColumnTXIDSnapshot"" txid_snapshot,
                        ""ColumnTXIDSnapshotAsArray"" txid_snapshot[],
                        ""ColumnUUID"" uuid,
                        ""ColumnUUIDAsArray"" uuid[],
                        ""ColumnXID"" xid,
                        ""ColumnXIDAsArray"" xid[],
                        ""ColumnXML"" xml,
                        ""ColumnXMLAsArray"" xml[],
                        CONSTRAINT ""CompleteTable_pkey"" PRIMARY KEY (""Id"")
                    )

                    TABLESPACE pg_default;

                    ALTER TABLE public.""CompleteTable""
                        OWNER to postgres;");
            }
        }

        private static void CreateNonIdentityCompleteTable()
        {
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS public.""NonIdentityCompleteTable""
                    (
                        ""Id"" bigint NOT NULL,
                        ""ColumnChar"" ""char"",
                        ""ColumnCharAsArray"" ""char""[],
                        --""ColumnAclItem"" aclitem,
                        --""ColumnAclItemAsArray"" aclitem[],
                        ""ColumnBigInt"" bigint,
                        ""ColumnBigIntAsArray"" bigint[],
                        ""ColumnBigSerial"" bigint,
                        ""ColumnBit"" bit(1),
                        ""ColumnBitVarying"" bit varying,
                        ""ColumnBitVaryingAsArray"" bit varying[],
                        ""ColumnBitAsArray"" bit(1)[],
                        ""ColumnBoolean"" boolean,
                        ""ColumnBooleanAsArray"" boolean[],
                        ""ColumnBox"" box,
                        ""ColumnBoxAsArray"" box[],
                        ""ColumnByteA"" bytea,
                        ""ColumnByteAAsArray"" bytea[],
                        ""ColumnCharacter"" character(1) COLLATE pg_catalog.""default"",
                        ""ColumnCharacterVarying"" character varying COLLATE pg_catalog.""default"",
                        ""ColumnCharacterVaryingAsArray"" character varying[] COLLATE pg_catalog.""default"",
                        ""ColumnCid"" cid,
                        ""ColumnCidAsArray"" cid[],
                        ""ColumnCidr"" cidr,
                        ""ColumnCircle"" circle,
                        ""ColumnCircleAsArray"" circle[],
                        ""ColumnDate"" date,
                        ""ColumnDateAsArray"" date[],
                        ""ColumnDateRange"" daterange,
                        ""ColumnDateRangeAsArray"" daterange[],
                        ""ColumnDoublePrecision"" double precision,
                        ""ColumnDoublePrecisionAsArray"" double precision[],
                        --""ColumnGtsVector"" gtsvector,
                        ""ColumnInet"" inet,
                        ""ColumnInetAsArray"" inet[],
                        ""ColumnInt2Vector"" int2vector,
                        ""ColumnInt2VectorAsArray"" int2vector[],
                        ""ColumnInt4Range"" int4range,
                        ""ColumnInt4RangeAsArray"" int4range[],
                        ""ColumnInt8Range"" int8range,
                        ""ColumnInt8RangeAsArray"" int8range[],
                        ""ColumnInteger"" integer,
                        ""ColumnIntegerAsArray"" integer[],
                        ""ColumnInterval"" interval,
                        ""ColumnIntervalAsArray"" interval[],
                        ""ColumnJson"" json,
                        ""ColumnJsonAsArray"" json[],
                        ""ColumnJsonB"" jsonb,
                        ""ColumnJsonBAsArray"" jsonb[],
                        ""ColumnJsonPath"" jsonpath,
                        ""ColumnJsonPathAsArray"" jsonpath[],
                        ""ColumnLine"" line,
                        ""ColumnLineAsArray"" line[],
                        ""ColumnLSeg"" lseg,
                        ""ColumnLSegAsArray"" lseg[],
                        ""ColumnMacAddr"" macaddr,
                        ""ColumnMacAddrAsArray"" macaddr[],
                        ""ColumnMacAddr8"" macaddr8,
                        ""ColumnMacAddr8AsArray"" macaddr8[],
                        ""ColumnMoney"" money,
                        ""ColumnMoneyAsArray"" money[],
                        ""ColumnName"" name COLLATE pg_catalog.""C"",
                        ""ColumnNameAsArray"" name[] COLLATE pg_catalog.""C"",
                        ""ColumnNumeric"" numeric,
                        ""ColumnNumericAsArray"" numeric[],
                        ""ColumnNumRange"" numrange,
                        ""ColumnNumRangeAsArray"" numrange[],
                        ""ColumnOId"" oid,
                        ""ColumnOIdAsArray"" oid[],
                        ""ColumnOIdVector"" oidvector,
                        ""ColumnOIdVectorAsArray"" oidvector[],
                        ""ColumnPath"" path,
                        ""ColumnPathAsArray"" path[],
                        ""ColumnPgDependencies"" pg_dependencies COLLATE pg_catalog.""default"",
                        ""ColumnPgLsn"" pg_lsn,
                        ""ColumnPgLsnAsArray"" pg_lsn[],
                        ""ColumnPgMcvList"" pg_mcv_list COLLATE pg_catalog.""default"",
                        ""ColumnPgNDistinct"" pg_ndistinct COLLATE pg_catalog.""default"",
                        ""ColumnPgNodeTree"" pg_node_tree COLLATE pg_catalog.""default"",
                        ""ColumnPoint"" point,
                        ""ColumnPointAsArray"" point[],
                        ""ColumnPolygon"" polygon,
                        ""ColumnPolygonAsArray"" polygon[],
                        ""ColumnReal"" real,
                        ""ColumnRealAsArray"" real[],
                        ""ColumnRefCursor"" refcursor,
                        ""ColumnRefCursorAsArray"" refcursor[],
                        ""ColumnRegClass"" regclass,
                        ""ColumnRegClassAsArray"" regclass[],
                        ""ColumnRegConfig"" regconfig,
                        ""ColumnRegConfigAsArray"" regconfig[],
                        ""ColumnRegDictionary"" regdictionary,
                        ""ColumnRegDictionaryAsArray"" regdictionary[],
                        ""ColumnRegNamespace"" regnamespace,
                        ""ColumnRegNamespaceAsArray"" regnamespace[],
                        ""ColumnRegOper"" regoper,
                        ""ColumnRegOperAsArray"" regoper[],
                        ""ColumnRegOperator"" regoperator,
                        ""ColumnRegOperationAsArray"" regoperator[],
                        ""ColumnRegProc"" regproc,
                        ""ColumnRegProcAsArray"" regproc[],
                        ""ColumnRegProcedure"" regprocedure,
                        ""ColumnRegProcedureAsArray"" regprocedure[],
                        ""ColumnRegRole"" regrole,
                        ""ColumnRegRoleAsArray"" regrole[],
                        ""ColumnRegType"" regtype,
                        ""ColumnRegTypeAsArray"" regtype[],
                        ""ColumnSerial"" integer,
                        ""ColumnSmallInt"" smallint,
                        ""ColumnSmallIntAsArray"" smallint[],
                        ""ColumnSmallSerial"" smallint,
                        ""ColumnText"" text COLLATE pg_catalog.""default"",
                        ""ColumnTextAsArray"" text[] COLLATE pg_catalog.""default"",
                        ""ColumnTId"" tid,
                        ""ColumnTidAsArray"" tid[],
                        ""ColumnTimeWithTimeZoneAsArray"" time with time zone[],
                        ""ColumnTimeWithTimeZone"" time with time zone,
                        ""ColumnTimeWithoutTimeZone"" time without time zone,
                        ""ColumnTimeWithoutTimeZoneAsArray"" time without time zone[],
                        ""ColumnTimestampWithTimeZone"" timestamp with time zone,
                        ""ColumnTimestampWithTimeZoneAsArray"" timestamp with time zone[],
                        ""ColumnTimestampWithoutTimeZone"" timestamp without time zone,
                        ""ColumnTimestampWithoutTimeZoneAsArray"" timestamp without time zone[],
                        ""ColumnTSQuery"" tsquery,
                        ""ColumnTSQueryAsArray"" tsquery[],
                        ""ColumnTSRange"" tsrange,
                        ""ColumnTSRangeAsArray"" tsrange[],
                        ""ColumnTSTZRange"" tstzrange,
                        ""ColumnTSTZRangeAsArray"" tstzrange[],
                        ""ColumnTSVector"" tsvector,
                        ""ColumnTSVectorAsArray"" tsvector[],
                        ""ColumnTXIDSnapshot"" txid_snapshot,
                        ""ColumnTXIDSnapshotAsArray"" txid_snapshot[],
                        ""ColumnUUID"" uuid,
                        ""ColumnUUIDAsArray"" uuid[],
                        ""ColumnXID"" xid,
                        ""ColumnXIDAsArray"" xid[],
                        ""ColumnXML"" xml,
                        ""ColumnXMLAsArray"" xml[],
                        CONSTRAINT ""NonIdentityCompleteTable_pkey"" PRIMARY KEY (""Id"")
                    )

                    TABLESPACE pg_default;

                    ALTER TABLE public.""NonIdentityCompleteTable""
                        OWNER to postgres;");
            }
        }

        #endregion

        #region CompleteTable

        public static IEnumerable<CompleteTable> CreateCompleteTables(int count)
        {
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                var tables = Helper.CreateCompleteTables(count);
                connection.InsertAll(tables);
                return tables;
            }
        }

        #endregion

        #region NonIdentityCompleteTable

        public static IEnumerable<NonIdentityCompleteTable> CreateNonIdentityCompleteTables(int count)
        {
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                var tables = Helper.CreateNonIdentityCompleteTables(count);
                connection.InsertAll(tables);
                return tables;
            }
        }

        #endregion
    }
}
