using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using System;

namespace RepoDb.Firebird.UnitTests.Resolvers
{
    /// <summary>
    /// Covers supported mappings and resolver boundaries.
    /// </summary>
    [TestClass]
    public class FirebirdDbTypeNameToClientTypeResolverBoundaryTest
    {
        #region Public Methods

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        /// <param name="name">The database type name.</param>
        /// <param name="expected">The expected CLR type.</param>
        [DataRow("smallint",
            typeof(short))]
        [DataRow("SMALLINT",
            typeof(short))]
        [DataRow("integer",
            typeof(int))]
        [DataRow("INTEGER",
            typeof(int))]
        [DataRow("bigint",
            typeof(long))]
        [DataRow("BIGINT",
            typeof(long))]
        [DataRow("int128",
            typeof(System.Numerics.BigInteger))]
        [DataRow("INT128",
            typeof(System.Numerics.BigInteger))]
        [DataRow("float",
            typeof(float))]
        [DataRow("FLOAT",
            typeof(float))]
        [DataRow("double precision",
            typeof(double))]
        [DataRow("DOUBLE PRECISION",
            typeof(double))]
        [DataRow("numeric",
            typeof(decimal))]
        [DataRow("NUMERIC",
            typeof(decimal))]
        [DataRow("decimal",
            typeof(decimal))]
        [DataRow("DECIMAL",
            typeof(decimal))]
        [DataRow("dec16",
            typeof(decimal))]
        [DataRow("DEC16",
            typeof(decimal))]
        [DataRow("dec34",
            typeof(decimal))]
        [DataRow("DEC34",
            typeof(decimal))]
        [DataRow("char",
            typeof(string))]
        [DataRow("CHAR",
            typeof(string))]
        [DataRow("varchar",
            typeof(string))]
        [DataRow("VARCHAR",
            typeof(string))]
        [DataRow("blob_text",
            typeof(string))]
        [DataRow("BLOB_TEXT",
            typeof(string))]
        [DataRow("blob_binary",
            typeof(byte[]))]
        [DataRow("BLOB_BINARY",
            typeof(byte[]))]
        [DataRow("binary",
            typeof(byte[]))]
        [DataRow("BINARY",
            typeof(byte[]))]
        [DataRow("varbinary",
            typeof(byte[]))]
        [DataRow("VARBINARY",
            typeof(byte[]))]
        [DataRow("boolean",
            typeof(bool))]
        [DataRow("BOOLEAN",
            typeof(bool))]
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
        [DataRow("time_tz",
            typeof(DateTimeOffset))]
        [DataRow("TIME_TZ",
            typeof(DateTimeOffset))]
        [DataRow("timestamp_tz",
            typeof(DateTimeOffset))]
        [DataRow("TIMESTAMP_TZ",
            typeof(DateTimeOffset))]
        [DataRow("none",
            typeof(object))]
        [DataRow("NONE",
            typeof(object))]
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedType(string name,
            Type expected)
        {
            // Setup
            var resolver = new FirebirdDbTypeNameToClientTypeResolver();


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
            var resolver = new FirebirdDbTypeNameToClientTypeResolver();


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
            var resolver = new FirebirdDbTypeNameToClientTypeResolver();


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