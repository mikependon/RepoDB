using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using System;

namespace RepoDb.AuroraDb.MySqlConnector.UnitTests.Resolvers
{
    /// <summary>
    /// Covers supported mappings and resolver boundaries.
    /// </summary>
    [TestClass]
    public class AuroraDbDbTypeNameToClientTypeResolverBoundaryTest
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
        [DataRow("integer",
            typeof(long))]
        [DataRow("INTEGER",
            typeof(long))]
        [DataRow("blob",
            typeof(byte[]))]
        [DataRow("BLOB",
            typeof(byte[]))]
        [DataRow("blobasarray",
            typeof(byte[]))]
        [DataRow("BLOBASARRAY",
            typeof(byte[]))]
        [DataRow("binary",
            typeof(byte[]))]
        [DataRow("BINARY",
            typeof(byte[]))]
        [DataRow("longblob",
            typeof(byte[]))]
        [DataRow("LONGBLOB",
            typeof(byte[]))]
        [DataRow("mediumblob",
            typeof(byte[]))]
        [DataRow("MEDIUMBLOB",
            typeof(byte[]))]
        [DataRow("tinyblob",
            typeof(byte[]))]
        [DataRow("TINYBLOB",
            typeof(byte[]))]
        [DataRow("varbinary",
            typeof(byte[]))]
        [DataRow("VARBINARY",
            typeof(byte[]))]
        [DataRow("geometry",
            typeof(byte[]))]
        [DataRow("GEOMETRY",
            typeof(byte[]))]
        [DataRow("linestring",
            typeof(byte[]))]
        [DataRow("LINESTRING",
            typeof(byte[]))]
        [DataRow("multilinestring",
            typeof(byte[]))]
        [DataRow("MULTILINESTRING",
            typeof(byte[]))]
        [DataRow("multipoint",
            typeof(byte[]))]
        [DataRow("MULTIPOINT",
            typeof(byte[]))]
        [DataRow("multipolygon",
            typeof(byte[]))]
        [DataRow("MULTIPOLYGON",
            typeof(byte[]))]
        [DataRow("point",
            typeof(byte[]))]
        [DataRow("POINT",
            typeof(byte[]))]
        [DataRow("polygon",
            typeof(byte[]))]
        [DataRow("POLYGON",
            typeof(byte[]))]
        [DataRow("boolean",
            typeof(bool))]
        [DataRow("BOOLEAN",
            typeof(bool))]
        [DataRow("char",
            typeof(string))]
        [DataRow("CHAR",
            typeof(string))]
        [DataRow("json",
            typeof(string))]
        [DataRow("JSON",
            typeof(string))]
        [DataRow("longtext",
            typeof(string))]
        [DataRow("LONGTEXT",
            typeof(string))]
        [DataRow("mediumtext",
            typeof(string))]
        [DataRow("MEDIUMTEXT",
            typeof(string))]
        [DataRow("nchar",
            typeof(string))]
        [DataRow("NCHAR",
            typeof(string))]
        [DataRow("nvarchar",
            typeof(string))]
        [DataRow("NVARCHAR",
            typeof(string))]
        [DataRow("string",
            typeof(string))]
        [DataRow("STRING",
            typeof(string))]
        [DataRow("text",
            typeof(string))]
        [DataRow("TEXT",
            typeof(string))]
        [DataRow("tinytext",
            typeof(string))]
        [DataRow("TINYTEXT",
            typeof(string))]
        [DataRow("varchar",
            typeof(string))]
        [DataRow("VARCHAR",
            typeof(string))]
        [DataRow("date",
            typeof(DateTime))]
        [DataRow("DATE",
            typeof(DateTime))]
        [DataRow("datetime",
            typeof(DateTime))]
        [DataRow("DATETIME",
            typeof(DateTime))]
        [DataRow("datetime2",
            typeof(DateTime))]
        [DataRow("DATETIME2",
            typeof(DateTime))]
        [DataRow("timestamp",
            typeof(DateTime))]
        [DataRow("TIMESTAMP",
            typeof(DateTime))]
        [DataRow("time",
            typeof(TimeSpan))]
        [DataRow("TIME",
            typeof(TimeSpan))]
        [DataRow("decimal",
            typeof(decimal))]
        [DataRow("DECIMAL",
            typeof(decimal))]
        [DataRow("decimal2",
            typeof(decimal))]
        [DataRow("DECIMAL2",
            typeof(decimal))]
        [DataRow("numeric",
            typeof(decimal))]
        [DataRow("NUMERIC",
            typeof(decimal))]
        [DataRow("double",
            typeof(double))]
        [DataRow("DOUBLE",
            typeof(double))]
        [DataRow("real",
            typeof(double))]
        [DataRow("REAL",
            typeof(double))]
        [DataRow("float",
            typeof(float))]
        [DataRow("FLOAT",
            typeof(float))]
        [DataRow("int",
            typeof(int))]
        [DataRow("INT",
            typeof(int))]
        [DataRow("int2",
            typeof(int))]
        [DataRow("INT2",
            typeof(int))]
        [DataRow("mediumint",
            typeof(int))]
        [DataRow("MEDIUMINT",
            typeof(int))]
        [DataRow("year",
            typeof(int))]
        [DataRow("YEAR",
            typeof(int))]
        [DataRow("smallint",
            typeof(short))]
        [DataRow("SMALLINT",
            typeof(short))]
        [DataRow("tinyint",
            typeof(sbyte))]
        [DataRow("TINYINT",
            typeof(sbyte))]
        [DataRow("bit",
            typeof(ulong))]
        [DataRow("BIT",
            typeof(ulong))]
        [DataRow("none",
            typeof(object))]
        [DataRow("NONE",
            typeof(object))]
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedType(string name,
            Type expected)
        {
            // Setup
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();


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
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();


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
            var resolver = new AuroraDbDbTypeNameToClientTypeResolver();


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