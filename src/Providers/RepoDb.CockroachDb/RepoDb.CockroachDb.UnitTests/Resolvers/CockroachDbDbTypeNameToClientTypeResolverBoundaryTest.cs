using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using System;

namespace RepoDb.CockroachDb.UnitTests.Resolvers
{
    /// <summary>
    /// Covers supported mappings and resolver boundaries.
    /// </summary>
    [TestClass]
    public class CockroachDbDbTypeNameToClientTypeResolverBoundaryTest
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
        [DataRow("int8",
            typeof(Int64))]
        [DataRow("INT8",
            typeof(Int64))]
        [DataRow("array",
            typeof(Array))]
        [DataRow("ARRAY",
            typeof(Array))]
        [DataRow("character",
            typeof(String))]
        [DataRow("CHARACTER",
            typeof(String))]
        [DataRow("char",
            typeof(String))]
        [DataRow("CHAR",
            typeof(String))]
        [DataRow("character varying",
            typeof(String))]
        [DataRow("CHARACTER VARYING",
            typeof(String))]
        [DataRow("varchar",
            typeof(String))]
        [DataRow("VARCHAR",
            typeof(String))]
        [DataRow("citext",
            typeof(String))]
        [DataRow("CITEXT",
            typeof(String))]
        [DataRow("json",
            typeof(String))]
        [DataRow("JSON",
            typeof(String))]
        [DataRow("jsonb",
            typeof(String))]
        [DataRow("JSONB",
            typeof(String))]
        [DataRow("ltree",
            typeof(String))]
        [DataRow("LTREE",
            typeof(String))]
        [DataRow("name",
            typeof(String))]
        [DataRow("NAME",
            typeof(String))]
        [DataRow("regclass",
            typeof(String))]
        [DataRow("REGCLASS",
            typeof(String))]
        [DataRow("regnamespace",
            typeof(String))]
        [DataRow("REGNAMESPACE",
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
        [DataRow("string",
            typeof(String))]
        [DataRow("STRING",
            typeof(String))]
        [DataRow("text",
            typeof(String))]
        [DataRow("TEXT",
            typeof(String))]
        [DataRow("boolean",
            typeof(Boolean))]
        [DataRow("BOOLEAN",
            typeof(Boolean))]
        [DataRow("bool",
            typeof(Boolean))]
        [DataRow("BOOL",
            typeof(Boolean))]
        [DataRow("bit",
            typeof(System.Collections.BitArray))]
        [DataRow("BIT",
            typeof(System.Collections.BitArray))]
        [DataRow("bit varying",
            typeof(System.Collections.BitArray))]
        [DataRow("BIT VARYING",
            typeof(System.Collections.BitArray))]
        [DataRow("varbit",
            typeof(System.Collections.BitArray))]
        [DataRow("VARBIT",
            typeof(System.Collections.BitArray))]
        [DataRow("bytea",
            typeof(Byte[]))]
        [DataRow("BYTEA",
            typeof(Byte[]))]
        [DataRow("bytes",
            typeof(Byte[]))]
        [DataRow("BYTES",
            typeof(Byte[]))]
        [DataRow("oid",
            typeof(UInt32))]
        [DataRow("OID",
            typeof(UInt32))]
        [DataRow("regtype",
            typeof(UInt32))]
        [DataRow("REGTYPE",
            typeof(UInt32))]
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
        [DataRow("float8",
            typeof(Double))]
        [DataRow("FLOAT8",
            typeof(Double))]
        [DataRow("float",
            typeof(Double))]
        [DataRow("FLOAT",
            typeof(Double))]
        [DataRow("inet",
            typeof(System.Net.IPAddress))]
        [DataRow("INET",
            typeof(System.Net.IPAddress))]
        [DataRow("integer",
            typeof(Int32))]
        [DataRow("INTEGER",
            typeof(Int32))]
        [DataRow("int4",
            typeof(Int32))]
        [DataRow("INT4",
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
        [DataRow("numeric",
            typeof(Decimal))]
        [DataRow("NUMERIC",
            typeof(Decimal))]
        [DataRow("decimal",
            typeof(Decimal))]
        [DataRow("DECIMAL",
            typeof(Decimal))]
        [DataRow("real",
            typeof(Single))]
        [DataRow("REAL",
            typeof(Single))]
        [DataRow("float4",
            typeof(Single))]
        [DataRow("FLOAT4",
            typeof(Single))]
        [DataRow("smallint",
            typeof(Int16))]
        [DataRow("SMALLINT",
            typeof(Int16))]
        [DataRow("int2",
            typeof(Int16))]
        [DataRow("INT2",
            typeof(Int16))]
        [DataRow("timetz",
            typeof(DateTimeOffset))]
        [DataRow("TIMETZ",
            typeof(DateTimeOffset))]
        [DataRow("time with time zone",
            typeof(DateTimeOffset))]
        [DataRow("TIME WITH TIME ZONE",
            typeof(DateTimeOffset))]
        [DataRow("uuid",
            typeof(Guid))]
        [DataRow("UUID",
            typeof(Guid))]
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedType(string name,
            Type expected)
        {
            // Setup
            var resolver = new CockroachDbDbTypeNameToClientTypeResolver();


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
            var resolver = new CockroachDbDbTypeNameToClientTypeResolver();


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
            var resolver = new CockroachDbDbTypeNameToClientTypeResolver();


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