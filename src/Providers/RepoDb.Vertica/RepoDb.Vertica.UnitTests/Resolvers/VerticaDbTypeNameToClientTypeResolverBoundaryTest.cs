using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using System;

namespace RepoDb.Vertica.UnitTests.Resolvers
{
    /// <summary>
    /// Covers supported mappings and resolver boundaries.
    /// </summary>
    [TestClass]
    public class VerticaDbTypeNameToClientTypeResolverBoundaryTest
    {
        #region Public Methods

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        /// <param name="name">The database type name.</param>
        /// <param name="expected">The expected CLR type.</param>
        [DataRow("int",
            typeof(long))]
        [DataRow("INT",
            typeof(long))]
        [DataRow("integer",
            typeof(long))]
        [DataRow("INTEGER",
            typeof(long))]
        [DataRow("smallint",
            typeof(long))]
        [DataRow("SMALLINT",
            typeof(long))]
        [DataRow("bigint",
            typeof(long))]
        [DataRow("BIGINT",
            typeof(long))]
        [DataRow("int8",
            typeof(long))]
        [DataRow("INT8",
            typeof(long))]
        [DataRow("tinyint",
            typeof(long))]
        [DataRow("TINYINT",
            typeof(long))]
        [DataRow("float",
            typeof(double))]
        [DataRow("FLOAT",
            typeof(double))]
        [DataRow("float8",
            typeof(double))]
        [DataRow("FLOAT8",
            typeof(double))]
        [DataRow("double precision",
            typeof(double))]
        [DataRow("DOUBLE PRECISION",
            typeof(double))]
        [DataRow("real",
            typeof(double))]
        [DataRow("REAL",
            typeof(double))]
        [DataRow("numeric",
            typeof(decimal))]
        [DataRow("NUMERIC",
            typeof(decimal))]
        [DataRow("decimal",
            typeof(decimal))]
        [DataRow("DECIMAL",
            typeof(decimal))]
        [DataRow("number",
            typeof(decimal))]
        [DataRow("NUMBER",
            typeof(decimal))]
        [DataRow("money",
            typeof(decimal))]
        [DataRow("MONEY",
            typeof(decimal))]
        [DataRow("char",
            typeof(string))]
        [DataRow("CHAR",
            typeof(string))]
        [DataRow("varchar",
            typeof(string))]
        [DataRow("VARCHAR",
            typeof(string))]
        [DataRow("long varchar",
            typeof(string))]
        [DataRow("LONG VARCHAR",
            typeof(string))]
        [DataRow("binary",
            typeof(byte[]))]
        [DataRow("BINARY",
            typeof(byte[]))]
        [DataRow("varbinary",
            typeof(byte[]))]
        [DataRow("VARBINARY",
            typeof(byte[]))]
        [DataRow("long varbinary",
            typeof(byte[]))]
        [DataRow("LONG VARBINARY",
            typeof(byte[]))]
        [DataRow("bytea",
            typeof(byte[]))]
        [DataRow("BYTEA",
            typeof(byte[]))]
        [DataRow("raw",
            typeof(byte[]))]
        [DataRow("RAW",
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
            typeof(DateTime))]
        [DataRow("TIME",
            typeof(DateTime))]
        [DataRow("time with timezone",
            typeof(DateTimeOffset))]
        [DataRow("TIME WITH TIMEZONE",
            typeof(DateTimeOffset))]
        [DataRow("timestamp",
            typeof(DateTime))]
        [DataRow("TIMESTAMP",
            typeof(DateTime))]
        [DataRow("datetime",
            typeof(DateTime))]
        [DataRow("DATETIME",
            typeof(DateTime))]
        [DataRow("smalldatetime",
            typeof(DateTime))]
        [DataRow("SMALLDATETIME",
            typeof(DateTime))]
        [DataRow("timestamp with timezone",
            typeof(DateTimeOffset))]
        [DataRow("TIMESTAMP WITH TIMEZONE",
            typeof(DateTimeOffset))]
        [DataRow("uuid",
            typeof(Guid))]
        [DataRow("UUID",
            typeof(Guid))]
        [DataRow("none",
            typeof(object))]
        [DataRow("NONE",
            typeof(object))]
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedType(string name,
            Type expected)
        {
            // Setup
            var resolver = new VerticaDbTypeNameToClientTypeResolver();


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
            var resolver = new VerticaDbTypeNameToClientTypeResolver();


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
            var resolver = new VerticaDbTypeNameToClientTypeResolver();


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