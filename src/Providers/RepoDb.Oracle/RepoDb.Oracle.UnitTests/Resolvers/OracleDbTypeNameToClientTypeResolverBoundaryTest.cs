using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using System;

namespace RepoDb.Oracle.UnitTests.Resolvers
{
    /// <summary>
    /// Covers supported mappings and resolver boundaries.
    /// </summary>
    [TestClass]
    public class OracleDbTypeNameToClientTypeResolverBoundaryTest
    {
        #region Public Methods

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        /// <param name="name">The database type name.</param>
        /// <param name="expected">The expected CLR type.</param>
        [DataRow("number",
            typeof(decimal))]
        [DataRow("NUMBER",
            typeof(decimal))]
        [DataRow("float",
            typeof(decimal))]
        [DataRow("FLOAT",
            typeof(decimal))]
        [DataRow("dec",
            typeof(decimal))]
        [DataRow("DEC",
            typeof(decimal))]
        [DataRow("decimal",
            typeof(decimal))]
        [DataRow("DECIMAL",
            typeof(decimal))]
        [DataRow("numeric",
            typeof(decimal))]
        [DataRow("NUMERIC",
            typeof(decimal))]
        [DataRow("binary_float",
            typeof(float))]
        [DataRow("BINARY_FLOAT",
            typeof(float))]
        [DataRow("binary_double",
            typeof(double))]
        [DataRow("BINARY_DOUBLE",
            typeof(double))]
        [DataRow("char",
            typeof(string))]
        [DataRow("CHAR",
            typeof(string))]
        [DataRow("nchar",
            typeof(string))]
        [DataRow("NCHAR",
            typeof(string))]
        [DataRow("varchar",
            typeof(string))]
        [DataRow("VARCHAR",
            typeof(string))]
        [DataRow("varchar2",
            typeof(string))]
        [DataRow("VARCHAR2",
            typeof(string))]
        [DataRow("nvarchar2",
            typeof(string))]
        [DataRow("NVARCHAR2",
            typeof(string))]
        [DataRow("long",
            typeof(string))]
        [DataRow("LONG",
            typeof(string))]
        [DataRow("clob",
            typeof(string))]
        [DataRow("CLOB",
            typeof(string))]
        [DataRow("nclob",
            typeof(string))]
        [DataRow("NCLOB",
            typeof(string))]
        [DataRow("rowid",
            typeof(string))]
        [DataRow("ROWID",
            typeof(string))]
        [DataRow("urowid",
            typeof(string))]
        [DataRow("UROWID",
            typeof(string))]
        [DataRow("xmltype",
            typeof(string))]
        [DataRow("XMLTYPE",
            typeof(string))]
        [DataRow("date",
            typeof(DateTime))]
        [DataRow("DATE",
            typeof(DateTime))]
        [DataRow("blob",
            typeof(byte[]))]
        [DataRow("BLOB",
            typeof(byte[]))]
        [DataRow("raw",
            typeof(byte[]))]
        [DataRow("RAW",
            typeof(byte[]))]
        [DataRow("long raw",
            typeof(byte[]))]
        [DataRow("LONG RAW",
            typeof(byte[]))]
        [DataRow("bfile",
            typeof(byte[]))]
        [DataRow("BFILE",
            typeof(byte[]))]
        [DataRow("boolean",
            typeof(bool))]
        [DataRow("BOOLEAN",
            typeof(bool))]
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedType(string name,
            Type expected)
        {
            // Setup
            var resolver = new OracleDbTypeNameToClientTypeResolver();


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
            var resolver = new OracleDbTypeNameToClientTypeResolver();


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
            var resolver = new OracleDbTypeNameToClientTypeResolver();


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