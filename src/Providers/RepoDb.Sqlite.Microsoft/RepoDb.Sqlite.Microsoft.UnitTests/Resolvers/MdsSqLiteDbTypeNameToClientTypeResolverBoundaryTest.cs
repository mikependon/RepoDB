using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using System;

namespace RepoDb.Sqlite.Microsoft.UnitTests.Resolvers
{
    /// <summary>
    /// Covers supported mappings and resolver boundaries.
    /// </summary>
    [TestClass]
    public class MdsSqLiteDbTypeNameToClientTypeResolverBoundaryTest
    {
        #region Public Methods

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        /// <param name="name">The database type name.</param>
        /// <param name="expected">The expected CLR type.</param>
        [DataRow("bigint",
            typeof(long))]
        [DataRow("BIGINT",
            typeof(long))]
        [DataRow("decimal",
            typeof(long))]
        [DataRow("DECIMAL",
            typeof(long))]
        [DataRow("int",
            typeof(long))]
        [DataRow("INT",
            typeof(long))]
        [DataRow("integer",
            typeof(long))]
        [DataRow("INTEGER",
            typeof(long))]
        [DataRow("numeric",
            typeof(long))]
        [DataRow("NUMERIC",
            typeof(long))]
        [DataRow("blob",
            typeof(byte[]))]
        [DataRow("BLOB",
            typeof(byte[]))]
        [DataRow("double",
            typeof(double))]
        [DataRow("DOUBLE",
            typeof(double))]
        [DataRow("real",
            typeof(double))]
        [DataRow("REAL",
            typeof(double))]
        [DataRow("boolean",
            typeof(string))]
        [DataRow("BOOLEAN",
            typeof(string))]
        [DataRow("char",
            typeof(string))]
        [DataRow("CHAR",
            typeof(string))]
        [DataRow("date",
            typeof(string))]
        [DataRow("DATE",
            typeof(string))]
        [DataRow("datetime",
            typeof(string))]
        [DataRow("DATETIME",
            typeof(string))]
        [DataRow("none",
            typeof(string))]
        [DataRow("NONE",
            typeof(string))]
        [DataRow("string",
            typeof(string))]
        [DataRow("STRING",
            typeof(string))]
        [DataRow("text",
            typeof(string))]
        [DataRow("TEXT",
            typeof(string))]
        [DataRow("time",
            typeof(string))]
        [DataRow("TIME",
            typeof(string))]
        [DataRow("varchar",
            typeof(string))]
        [DataRow("VARCHAR",
            typeof(string))]
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedType(string name,
            Type expected)
        {
            // Setup
            var resolver = new MdsSqLiteDbTypeNameToClientTypeResolver();


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
            var resolver = new MdsSqLiteDbTypeNameToClientTypeResolver();


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
            var resolver = new MdsSqLiteDbTypeNameToClientTypeResolver();


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