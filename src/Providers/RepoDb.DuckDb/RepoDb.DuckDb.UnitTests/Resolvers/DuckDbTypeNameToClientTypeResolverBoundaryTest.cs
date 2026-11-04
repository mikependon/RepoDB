using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using System.Numerics;
using System;

namespace RepoDb.DuckDb.UnitTests.Resolvers
{
    /// <summary>
    /// Covers supported mappings and resolver boundaries.
    /// </summary>
    [TestClass]
    public class DuckDbTypeNameToClientTypeResolverBoundaryTest
    {
        #region Public Methods

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        /// <param name="name">The database type name.</param>
        /// <param name="expected">The expected CLR type.</param>
        [DataRow("boolean",
            typeof(bool))]
        [DataRow("BOOLEAN",
            typeof(bool))]
        [DataRow("bool",
            typeof(bool))]
        [DataRow("BOOL",
            typeof(bool))]
        [DataRow("logical",
            typeof(bool))]
        [DataRow("LOGICAL",
            typeof(bool))]
        [DataRow("tinyint",
            typeof(sbyte))]
        [DataRow("TINYINT",
            typeof(sbyte))]
        [DataRow("int1",
            typeof(sbyte))]
        [DataRow("INT1",
            typeof(sbyte))]
        [DataRow("smallint",
            typeof(short))]
        [DataRow("SMALLINT",
            typeof(short))]
        [DataRow("int2",
            typeof(short))]
        [DataRow("INT2",
            typeof(short))]
        [DataRow("short",
            typeof(short))]
        [DataRow("SHORT",
            typeof(short))]
        [DataRow("integer",
            typeof(int))]
        [DataRow("INTEGER",
            typeof(int))]
        [DataRow("int4",
            typeof(int))]
        [DataRow("INT4",
            typeof(int))]
        [DataRow("int",
            typeof(int))]
        [DataRow("INT",
            typeof(int))]
        [DataRow("signed",
            typeof(int))]
        [DataRow("SIGNED",
            typeof(int))]
        [DataRow("bigint",
            typeof(long))]
        [DataRow("BIGINT",
            typeof(long))]
        [DataRow("int8",
            typeof(long))]
        [DataRow("INT8",
            typeof(long))]
        [DataRow("long",
            typeof(long))]
        [DataRow("LONG",
            typeof(long))]
        [DataRow("hugeint",
            typeof(BigInteger))]
        [DataRow("HUGEINT",
            typeof(BigInteger))]
        [DataRow("utinyint",
            typeof(byte))]
        [DataRow("UTINYINT",
            typeof(byte))]
        [DataRow("usmallint",
            typeof(ushort))]
        [DataRow("USMALLINT",
            typeof(ushort))]
        [DataRow("uinteger",
            typeof(uint))]
        [DataRow("UINTEGER",
            typeof(uint))]
        [DataRow("ubigint",
            typeof(ulong))]
        [DataRow("UBIGINT",
            typeof(ulong))]
        [DataRow("uhugeint",
            typeof(BigInteger))]
        [DataRow("UHUGEINT",
            typeof(BigInteger))]
        [DataRow("float",
            typeof(float))]
        [DataRow("FLOAT",
            typeof(float))]
        [DataRow("float4",
            typeof(float))]
        [DataRow("FLOAT4",
            typeof(float))]
        [DataRow("real",
            typeof(float))]
        [DataRow("REAL",
            typeof(float))]
        [DataRow("double",
            typeof(double))]
        [DataRow("DOUBLE",
            typeof(double))]
        [DataRow("float8",
            typeof(double))]
        [DataRow("FLOAT8",
            typeof(double))]
        [DataRow("decimal",
            typeof(decimal))]
        [DataRow("DECIMAL",
            typeof(decimal))]
        [DataRow("numeric",
            typeof(decimal))]
        [DataRow("NUMERIC",
            typeof(decimal))]
        [DataRow("varchar",
            typeof(string))]
        [DataRow("VARCHAR",
            typeof(string))]
        [DataRow("char",
            typeof(string))]
        [DataRow("CHAR",
            typeof(string))]
        [DataRow("bpchar",
            typeof(string))]
        [DataRow("BPCHAR",
            typeof(string))]
        [DataRow("text",
            typeof(string))]
        [DataRow("TEXT",
            typeof(string))]
        [DataRow("string",
            typeof(string))]
        [DataRow("STRING",
            typeof(string))]
        [DataRow("json",
            typeof(string))]
        [DataRow("JSON",
            typeof(string))]
        [DataRow("blob",
            typeof(byte[]))]
        [DataRow("BLOB",
            typeof(byte[]))]
        [DataRow("bytea",
            typeof(byte[]))]
        [DataRow("BYTEA",
            typeof(byte[]))]
        [DataRow("binary",
            typeof(byte[]))]
        [DataRow("BINARY",
            typeof(byte[]))]
        [DataRow("varbinary",
            typeof(byte[]))]
        [DataRow("VARBINARY",
            typeof(byte[]))]
        [DataRow("date",
            typeof(DateTime))]
        [DataRow("DATE",
            typeof(DateTime))]
        [DataRow("time",
            typeof(TimeSpan))]
        [DataRow("TIME",
            typeof(TimeSpan))]
        [DataRow("timestamp",
            typeof(DateTime))]
        [DataRow("TIMESTAMP",
            typeof(DateTime))]
        [DataRow("datetime",
            typeof(DateTime))]
        [DataRow("DATETIME",
            typeof(DateTime))]
        [DataRow("timestamp_s",
            typeof(DateTime))]
        [DataRow("TIMESTAMP_S",
            typeof(DateTime))]
        [DataRow("timestamp_ms",
            typeof(DateTime))]
        [DataRow("TIMESTAMP_MS",
            typeof(DateTime))]
        [DataRow("timestamp_ns",
            typeof(DateTime))]
        [DataRow("TIMESTAMP_NS",
            typeof(DateTime))]
        [DataRow("timestamp with time zone",
            typeof(DateTime))]
        [DataRow("TIMESTAMP WITH TIME ZONE",
            typeof(DateTime))]
        [DataRow("timestamptz",
            typeof(DateTime))]
        [DataRow("TIMESTAMPTZ",
            typeof(DateTime))]
        [DataRow("uuid",
            typeof(Guid))]
        [DataRow("UUID",
            typeof(Guid))]
        [DataRow("bit",
            typeof(string))]
        [DataRow("BIT",
            typeof(string))]
        [DataRow("bitstring",
            typeof(string))]
        [DataRow("BITSTRING",
            typeof(string))]
        [DataRow("interval",
            typeof(TimeSpan))]
        [DataRow("INTERVAL",
            typeof(TimeSpan))]
        [DataRow("none",
            typeof(object))]
        [DataRow("NONE",
            typeof(object))]
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedType(string name,
            Type expected)
        {
            // Setup
            var resolver = new DuckDbTypeNameToClientTypeResolver();


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
            var resolver = new DuckDbTypeNameToClientTypeResolver();


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
            var resolver = new DuckDbTypeNameToClientTypeResolver();


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