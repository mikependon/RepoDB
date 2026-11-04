using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using System;

namespace RepoDb.ClickHouse.UnitTests.Resolvers
{
    /// <summary>
    /// Covers supported mappings and resolver boundaries.
    /// </summary>
    [TestClass]
    public class ClickHouseDbTypeNameToClientTypeResolverBoundaryTest
    {
        #region Public Methods

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        /// <param name="name">The database type name.</param>
        /// <param name="expected">The expected CLR type.</param>
        [DataRow("int8",
            typeof(sbyte))]
        [DataRow("INT8",
            typeof(sbyte))]
        [DataRow("int16",
            typeof(short))]
        [DataRow("INT16",
            typeof(short))]
        [DataRow("int32",
            typeof(int))]
        [DataRow("INT32",
            typeof(int))]
        [DataRow("int64",
            typeof(long))]
        [DataRow("INT64",
            typeof(long))]
        [DataRow("int128",
            typeof(long))]
        [DataRow("INT128",
            typeof(long))]
        [DataRow("int256",
            typeof(long))]
        [DataRow("INT256",
            typeof(long))]
        [DataRow("uint8",
            typeof(byte))]
        [DataRow("UINT8",
            typeof(byte))]
        [DataRow("uint16",
            typeof(ushort))]
        [DataRow("UINT16",
            typeof(ushort))]
        [DataRow("uint32",
            typeof(uint))]
        [DataRow("UINT32",
            typeof(uint))]
        [DataRow("uint64",
            typeof(ulong))]
        [DataRow("UINT64",
            typeof(ulong))]
        [DataRow("uint128",
            typeof(ulong))]
        [DataRow("UINT128",
            typeof(ulong))]
        [DataRow("uint256",
            typeof(ulong))]
        [DataRow("UINT256",
            typeof(ulong))]
        [DataRow("float32",
            typeof(float))]
        [DataRow("FLOAT32",
            typeof(float))]
        [DataRow("float64",
            typeof(double))]
        [DataRow("FLOAT64",
            typeof(double))]
        [DataRow("bfloat16",
            typeof(float))]
        [DataRow("BFLOAT16",
            typeof(float))]
        [DataRow("bool",
            typeof(bool))]
        [DataRow("BOOL",
            typeof(bool))]
        [DataRow("boolean",
            typeof(bool))]
        [DataRow("BOOLEAN",
            typeof(bool))]
        [DataRow("string",
            typeof(string))]
        [DataRow("STRING",
            typeof(string))]
        [DataRow("fixedstring",
            typeof(string))]
        [DataRow("FIXEDSTRING",
            typeof(string))]
        [DataRow("json",
            typeof(string))]
        [DataRow("JSON",
            typeof(string))]
        [DataRow("object",
            typeof(string))]
        [DataRow("OBJECT",
            typeof(string))]
        [DataRow("enum8",
            typeof(string))]
        [DataRow("ENUM8",
            typeof(string))]
        [DataRow("enum16",
            typeof(string))]
        [DataRow("ENUM16",
            typeof(string))]
        [DataRow("date",
            typeof(DateTime))]
        [DataRow("DATE",
            typeof(DateTime))]
        [DataRow("date32",
            typeof(DateTime))]
        [DataRow("DATE32",
            typeof(DateTime))]
        [DataRow("datetime",
            typeof(DateTime))]
        [DataRow("DATETIME",
            typeof(DateTime))]
        [DataRow("datetime32",
            typeof(DateTime))]
        [DataRow("DATETIME32",
            typeof(DateTime))]
        [DataRow("datetime64",
            typeof(DateTime))]
        [DataRow("DATETIME64",
            typeof(DateTime))]
        [DataRow("time",
            typeof(TimeSpan))]
        [DataRow("TIME",
            typeof(TimeSpan))]
        [DataRow("time64",
            typeof(TimeSpan))]
        [DataRow("TIME64",
            typeof(TimeSpan))]
        [DataRow("decimal",
            typeof(decimal))]
        [DataRow("DECIMAL",
            typeof(decimal))]
        [DataRow("decimal32",
            typeof(decimal))]
        [DataRow("DECIMAL32",
            typeof(decimal))]
        [DataRow("decimal64",
            typeof(decimal))]
        [DataRow("DECIMAL64",
            typeof(decimal))]
        [DataRow("decimal128",
            typeof(decimal))]
        [DataRow("DECIMAL128",
            typeof(decimal))]
        [DataRow("decimal256",
            typeof(decimal))]
        [DataRow("DECIMAL256",
            typeof(decimal))]
        [DataRow("uuid",
            typeof(Guid))]
        [DataRow("UUID",
            typeof(Guid))]
        [DataRow("ipv4",
            typeof(string))]
        [DataRow("IPV4",
            typeof(string))]
        [DataRow("ipv6",
            typeof(string))]
        [DataRow("IPV6",
            typeof(string))]
        [DataRow("none",
            typeof(object))]
        [DataRow("NONE",
            typeof(object))]
        [DataRow("nothing",
            typeof(object))]
        [DataRow("NOTHING",
            typeof(object))]
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedType(string name,
            Type expected)
        {
            // Setup
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();


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
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();


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
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();


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