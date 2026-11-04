using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using RepoDb.Types;
using System;

namespace RepoDb.SqlServer.UnitTests.Resolvers
{
    /// <summary>
    /// Covers supported mappings and resolver boundaries.
    /// </summary>
    [TestClass]
    public class SqlServerDbTypeNameToClientTypeResolverBoundaryTest
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
        [DataRow("attribute",
            typeof(byte[]))]
        [DataRow("ATTRIBUTE",
            typeof(byte[]))]
        [DataRow("binary",
            typeof(byte[]))]
        [DataRow("BINARY",
            typeof(byte[]))]
        [DataRow("filestream",
            typeof(byte[]))]
        [DataRow("FILESTREAM",
            typeof(byte[]))]
        [DataRow("image",
            typeof(byte[]))]
        [DataRow("IMAGE",
            typeof(byte[]))]
        [DataRow("rowversion",
            typeof(byte[]))]
        [DataRow("ROWVERSION",
            typeof(byte[]))]
        [DataRow("timestamp",
            typeof(byte[]))]
        [DataRow("TIMESTAMP",
            typeof(byte[]))]
        [DataRow("varbinary",
            typeof(byte[]))]
        [DataRow("VARBINARY",
            typeof(byte[]))]
        [DataRow("varbinary(max)",
            typeof(byte[]))]
        [DataRow("VARBINARY(MAX)",
            typeof(byte[]))]
        [DataRow("bit",
            typeof(bool))]
        [DataRow("BIT",
            typeof(bool))]
        [DataRow("char",
            typeof(string))]
        [DataRow("CHAR",
            typeof(string))]
        [DataRow("nchar",
            typeof(string))]
        [DataRow("NCHAR",
            typeof(string))]
        [DataRow("ntext",
            typeof(string))]
        [DataRow("NTEXT",
            typeof(string))]
        [DataRow("nvarchar",
            typeof(string))]
        [DataRow("NVARCHAR",
            typeof(string))]
        [DataRow("text",
            typeof(string))]
        [DataRow("TEXT",
            typeof(string))]
        [DataRow("varchar",
            typeof(string))]
        [DataRow("VARCHAR",
            typeof(string))]
        [DataRow("xml",
            typeof(string))]
        [DataRow("XML",
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
        [DataRow("smalldatetime",
            typeof(DateTime))]
        [DataRow("SMALLDATETIME",
            typeof(DateTime))]
        [DataRow("datetimeoffset",
            typeof(DateTimeOffset))]
        [DataRow("DATETIMEOFFSET",
            typeof(DateTimeOffset))]
        [DataRow("decimal",
            typeof(decimal))]
        [DataRow("DECIMAL",
            typeof(decimal))]
        [DataRow("money",
            typeof(decimal))]
        [DataRow("MONEY",
            typeof(decimal))]
        [DataRow("numeric",
            typeof(decimal))]
        [DataRow("NUMERIC",
            typeof(decimal))]
        [DataRow("smallmoney",
            typeof(decimal))]
        [DataRow("SMALLMONEY",
            typeof(decimal))]
        [DataRow("float",
            typeof(double))]
        [DataRow("FLOAT",
            typeof(double))]
        [DataRow("int",
            typeof(int))]
        [DataRow("INT",
            typeof(int))]
        [DataRow("real",
            typeof(float))]
        [DataRow("REAL",
            typeof(float))]
        [DataRow("smallint",
            typeof(short))]
        [DataRow("SMALLINT",
            typeof(short))]
        [DataRow("sql_variant",
            typeof(SqlVariant))]
        [DataRow("SQL_VARIANT",
            typeof(SqlVariant))]
        [DataRow("time",
            typeof(TimeSpan))]
        [DataRow("TIME",
            typeof(TimeSpan))]
        [DataRow("tinyint",
            typeof(byte))]
        [DataRow("TINYINT",
            typeof(byte))]
        [DataRow("uniqueidentifier",
            typeof(Guid))]
        [DataRow("UNIQUEIDENTIFIER",
            typeof(Guid))]
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedType(string name,
            Type expected)
        {
            // Setup
            var resolver = new SqlServerDbTypeNameToClientTypeResolver();


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
            var resolver = new SqlServerDbTypeNameToClientTypeResolver();


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
            var resolver = new SqlServerDbTypeNameToClientTypeResolver();


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