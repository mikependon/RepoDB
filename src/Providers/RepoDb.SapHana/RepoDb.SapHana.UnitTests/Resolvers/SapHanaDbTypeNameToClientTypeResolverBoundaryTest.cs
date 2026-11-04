using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using System;

namespace RepoDb.SapHana.UnitTests.Resolvers
{
    /// <summary>
    /// Covers supported mappings and resolver boundaries.
    /// </summary>
    [TestClass]
    public class SapHanaDbTypeNameToClientTypeResolverBoundaryTest
    {
        #region Public Methods

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        /// <param name="name">The database type name.</param>
        /// <param name="expected">The expected CLR type.</param>
        [DataRow("tinyint",
            typeof(byte))]
        [DataRow("TINYINT",
            typeof(byte))]
        [DataRow("smallint",
            typeof(short))]
        [DataRow("SMALLINT",
            typeof(short))]
        [DataRow("integer",
            typeof(int))]
        [DataRow("INTEGER",
            typeof(int))]
        [DataRow("int",
            typeof(int))]
        [DataRow("INT",
            typeof(int))]
        [DataRow("bigint",
            typeof(long))]
        [DataRow("BIGINT",
            typeof(long))]
        [DataRow("decimal",
            typeof(decimal))]
        [DataRow("DECIMAL",
            typeof(decimal))]
        [DataRow("smalldecimal",
            typeof(decimal))]
        [DataRow("SMALLDECIMAL",
            typeof(decimal))]
        [DataRow("numeric",
            typeof(decimal))]
        [DataRow("NUMERIC",
            typeof(decimal))]
        [DataRow("real",
            typeof(float))]
        [DataRow("REAL",
            typeof(float))]
        [DataRow("float",
            typeof(float))]
        [DataRow("FLOAT",
            typeof(float))]
        [DataRow("double",
            typeof(double))]
        [DataRow("DOUBLE",
            typeof(double))]
        [DataRow("boolean",
            typeof(bool))]
        [DataRow("BOOLEAN",
            typeof(bool))]
        [DataRow("varchar",
            typeof(string))]
        [DataRow("VARCHAR",
            typeof(string))]
        [DataRow("nvarchar",
            typeof(string))]
        [DataRow("NVARCHAR",
            typeof(string))]
        [DataRow("alphanum",
            typeof(string))]
        [DataRow("ALPHANUM",
            typeof(string))]
        [DataRow("shorttext",
            typeof(string))]
        [DataRow("SHORTTEXT",
            typeof(string))]
        [DataRow("char",
            typeof(string))]
        [DataRow("CHAR",
            typeof(string))]
        [DataRow("nchar",
            typeof(string))]
        [DataRow("NCHAR",
            typeof(string))]
        [DataRow("text",
            typeof(string))]
        [DataRow("TEXT",
            typeof(string))]
        [DataRow("clob",
            typeof(string))]
        [DataRow("CLOB",
            typeof(string))]
        [DataRow("nclob",
            typeof(string))]
        [DataRow("NCLOB",
            typeof(string))]
        [DataRow("string",
            typeof(string))]
        [DataRow("STRING",
            typeof(string))]
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
        [DataRow("seconddate",
            typeof(DateTime))]
        [DataRow("SECONDDATE",
            typeof(DateTime))]
        [DataRow("blob",
            typeof(byte[]))]
        [DataRow("BLOB",
            typeof(byte[]))]
        [DataRow("varbinary",
            typeof(byte[]))]
        [DataRow("VARBINARY",
            typeof(byte[]))]
        [DataRow("binary",
            typeof(byte[]))]
        [DataRow("BINARY",
            typeof(byte[]))]
        [DataRow("bintext",
            typeof(byte[]))]
        [DataRow("BINTEXT",
            typeof(byte[]))]
        [DataRow("none",
            typeof(object))]
        [DataRow("NONE",
            typeof(object))]
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedType(string name,
            Type expected)
        {
            // Setup
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();


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
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();


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
            var resolver = new SapHanaDbTypeNameToClientTypeResolver();


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