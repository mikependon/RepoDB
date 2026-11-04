using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.Resolvers;
using System;

namespace RepoDb.PostgreSql.UnitTests.Resolvers
{
    /// <summary>
    /// Covers supported mappings and resolver boundaries.
    /// </summary>
    [TestClass]
    public class PostgreSqlDbTypeNameToClientTypeResolverBoundaryTest
    {
        #region Public Methods

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        /// <param name="name">The database type name.</param>
        /// <param name="expected">The expected CLR type.</param>
        [DataRow("bigint",
            typeof(Int64))]
        [DataRow("BIGINT",
            typeof(Int64))]
        [DataRow("array",
            typeof(Array))]
        [DataRow("ARRAY",
            typeof(Array))]
        [DataRow("character",
            typeof(String))]
        [DataRow("CHARACTER",
            typeof(String))]
        [DataRow("character varying",
            typeof(String))]
        [DataRow("CHARACTER VARYING",
            typeof(String))]
        [DataRow("json",
            typeof(String))]
        [DataRow("JSON",
            typeof(String))]
        [DataRow("jsonb",
            typeof(String))]
        [DataRow("JSONB",
            typeof(String))]
        [DataRow("jsonpath",
            typeof(String))]
        [DataRow("JSONPATH",
            typeof(String))]
        [DataRow("name",
            typeof(String))]
        [DataRow("NAME",
            typeof(String))]
        [DataRow("pg_dependencies",
            typeof(String))]
        [DataRow("PG_DEPENDENCIES",
            typeof(String))]
        [DataRow("pg_lsn",
            typeof(String))]
        [DataRow("PG_LSN",
            typeof(String))]
        [DataRow("pg_mcv_list",
            typeof(String))]
        [DataRow("PG_MCV_LIST",
            typeof(String))]
        [DataRow("pg_ndistinct",
            typeof(String))]
        [DataRow("PG_NDISTINCT",
            typeof(String))]
        [DataRow("pg_node_tree",
            typeof(String))]
        [DataRow("PG_NODE_TREE",
            typeof(String))]
        [DataRow("refcursor",
            typeof(String))]
        [DataRow("REFCURSOR",
            typeof(String))]
        [DataRow("regclass",
            typeof(String))]
        [DataRow("REGCLASS",
            typeof(String))]
        [DataRow("regdictionary",
            typeof(String))]
        [DataRow("REGDICTIONARY",
            typeof(String))]
        [DataRow("regnamespace",
            typeof(String))]
        [DataRow("REGNAMESPACE",
            typeof(String))]
        [DataRow("regoper",
            typeof(String))]
        [DataRow("REGOPER",
            typeof(String))]
        [DataRow("regoperator",
            typeof(String))]
        [DataRow("REGOPERATOR",
            typeof(String))]
        [DataRow("regproc",
            typeof(String))]
        [DataRow("REGPROC",
            typeof(String))]
        [DataRow("regprocedure",
            typeof(String))]
        [DataRow("REGPROCEDURE",
            typeof(String))]
        [DataRow("regrole",
            typeof(String))]
        [DataRow("REGROLE",
            typeof(String))]
        [DataRow("text",
            typeof(String))]
        [DataRow("TEXT",
            typeof(String))]
        [DataRow("txid_snapshot",
            typeof(String))]
        [DataRow("TXID_SNAPSHOT",
            typeof(String))]
        [DataRow("xml",
            typeof(String))]
        [DataRow("XML",
            typeof(String))]
        [DataRow("bit",
            typeof(Boolean))]
        [DataRow("BIT",
            typeof(Boolean))]
        [DataRow("boolean",
            typeof(Boolean))]
        [DataRow("BOOLEAN",
            typeof(Boolean))]
        [DataRow("bit varying",
            typeof(System.Collections.BitArray))]
        [DataRow("BIT VARYING",
            typeof(System.Collections.BitArray))]
        [DataRow("box",
            typeof(NpgsqlTypes.NpgsqlBox))]
        [DataRow("BOX",
            typeof(NpgsqlTypes.NpgsqlBox))]
        [DataRow("bytea",
            typeof(Byte[]))]
        [DataRow("BYTEA",
            typeof(Byte[]))]
        [DataRow("cid",
            typeof(UInt32))]
        [DataRow("CID",
            typeof(UInt32))]
        [DataRow("oid",
            typeof(UInt32))]
        [DataRow("OID",
            typeof(UInt32))]
        [DataRow("regconfig",
            typeof(UInt32))]
        [DataRow("REGCONFIG",
            typeof(UInt32))]
        [DataRow("regtype",
            typeof(UInt32))]
        [DataRow("REGTYPE",
            typeof(UInt32))]
        [DataRow("xid",
            typeof(UInt32))]
        [DataRow("XID",
            typeof(UInt32))]
        [DataRow("circle",
            typeof(NpgsqlTypes.NpgsqlCircle))]
        [DataRow("CIRCLE",
            typeof(NpgsqlTypes.NpgsqlCircle))]
        [DataRow("date",
            typeof(DateOnly))]
        [DataRow("DATE",
            typeof(DateOnly))]
        [DataRow("timestamp without time zone",
            typeof(DateTime))]
        [DataRow("TIMESTAMP WITHOUT TIME ZONE",
            typeof(DateTime))]
        [DataRow("timestamp",
            typeof(DateTime))]
        [DataRow("TIMESTAMP",
            typeof(DateTime))]
        [DataRow("timestamp with time zone",
            typeof(DateTimeOffset))]
        [DataRow("TIMESTAMP WITH TIME ZONE",
            typeof(DateTimeOffset))]
        [DataRow("timestamptz",
            typeof(DateTimeOffset))]
        [DataRow("TIMESTAMPTZ",
            typeof(DateTimeOffset))]
        [DataRow("double precision",
            typeof(Double))]
        [DataRow("DOUBLE PRECISION",
            typeof(Double))]
        [DataRow("inet",
            typeof(System.Net.IPAddress))]
        [DataRow("INET",
            typeof(System.Net.IPAddress))]
        [DataRow("integer",
            typeof(Int32))]
        [DataRow("INTEGER",
            typeof(Int32))]
        [DataRow("time without time zone",
            typeof(TimeOnly))]
        [DataRow("TIME WITHOUT TIME ZONE",
            typeof(TimeOnly))]
        [DataRow("time",
            typeof(TimeOnly))]
        [DataRow("TIME",
            typeof(TimeOnly))]
        [DataRow("interval",
            typeof(TimeSpan))]
        [DataRow("INTERVAL",
            typeof(TimeSpan))]
        [DataRow("line",
            typeof(NpgsqlTypes.NpgsqlLine))]
        [DataRow("LINE",
            typeof(NpgsqlTypes.NpgsqlLine))]
        [DataRow("lseg",
            typeof(NpgsqlTypes.NpgsqlLSeg))]
        [DataRow("LSEG",
            typeof(NpgsqlTypes.NpgsqlLSeg))]
        [DataRow("macaddr",
            typeof(System.Net.NetworkInformation.PhysicalAddress))]
        [DataRow("MACADDR",
            typeof(System.Net.NetworkInformation.PhysicalAddress))]
        [DataRow("macaddr8",
            typeof(System.Net.NetworkInformation.PhysicalAddress))]
        [DataRow("MACADDR8",
            typeof(System.Net.NetworkInformation.PhysicalAddress))]
        [DataRow("money",
            typeof(Decimal))]
        [DataRow("MONEY",
            typeof(Decimal))]
        [DataRow("numeric",
            typeof(Decimal))]
        [DataRow("NUMERIC",
            typeof(Decimal))]
        [DataRow("path",
            typeof(NpgsqlTypes.NpgsqlPath))]
        [DataRow("PATH",
            typeof(NpgsqlTypes.NpgsqlPath))]
        [DataRow("point",
            typeof(NpgsqlTypes.NpgsqlPoint))]
        [DataRow("POINT",
            typeof(NpgsqlTypes.NpgsqlPoint))]
        [DataRow("polygon",
            typeof(NpgsqlTypes.NpgsqlPolygon))]
        [DataRow("POLYGON",
            typeof(NpgsqlTypes.NpgsqlPolygon))]
        [DataRow("real",
            typeof(Single))]
        [DataRow("REAL",
            typeof(Single))]
        [DataRow("smallint",
            typeof(Int16))]
        [DataRow("SMALLINT",
            typeof(Int16))]
        [DataRow("tid",
            typeof(NpgsqlTypes.NpgsqlTid))]
        [DataRow("TID",
            typeof(NpgsqlTypes.NpgsqlTid))]
        [DataRow("timetz",
            typeof(DateTimeOffset))]
        [DataRow("TIMETZ",
            typeof(DateTimeOffset))]
        [DataRow("time with time zone",
            typeof(DateTimeOffset))]
        [DataRow("TIME WITH TIME ZONE",
            typeof(DateTimeOffset))]
        [DataRow("tsquery",
            typeof(NpgsqlTypes.NpgsqlTsQuery))]
        [DataRow("TSQUERY",
            typeof(NpgsqlTypes.NpgsqlTsQuery))]
        [DataRow("tsvector",
            typeof(NpgsqlTypes.NpgsqlTsVector))]
        [DataRow("TSVECTOR",
            typeof(NpgsqlTypes.NpgsqlTsVector))]
        [DataRow("uuid",
            typeof(Guid))]
        [DataRow("UUID",
            typeof(Guid))]
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedType(string name,
            Type expected)
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();


            // Act
            var actual = resolver.Resolve(name);


            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Falls back for empty and unknown names.
        /// </summary>
        /// <param name="name">The unsupported name.</param>
        [DataRow("")]
        [DataRow("__unsupported_type__")]
        [DataRow("   ")]
        [TestMethod]
        public void ResolveUnknownNameReturnsObject(string name)
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();


            // Act
            var actual = resolver.Resolve(name);


            // Assert
            Assert.AreEqual(typeof(object),
                actual);
        }

        /// <summary>
        /// Identifies the missing database type name.
        /// </summary>
        [TestMethod]
        public void ResolveNullReportsArgumentName()
        {
            // Setup
            var resolver = new PostgreSqlDbTypeNameToClientTypeResolver();


            // Act
            var exception = Assert.ThrowsExactly<ArgumentNullException>(() => resolver.Resolve(null));


            // Assert
            Assert.AreEqual("dbTypeName",
                exception.ParamName,
                StringComparer.Ordinal);
        }

        #endregion
    }
}