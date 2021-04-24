using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using System.Data;

namespace RepoDb.SqlServer.UnitTests.Resolvers
{
    [TestClass]
    public class DbTypeToSqlServerStringNameResolverTest
    {
        /*
         * Taken:
         * https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/sql-server-data-type-mappings
         */

        private readonly DbTypeToSqlServerStringNameResolver m_resolver = new();

        [TestMethod]
        public void TestDbTypeToSqlServerStringNameResolverForBigInt()
        {
            // Setup
            var dbType = DbType.Int64;

            // Act
            var name = m_resolver.Resolve(dbType).ToLowerInvariant();

            // Assert
            Assert.AreEqual("bigint", name);
        }

        [TestMethod]
        public void TestDbTypeToSqlServerStringNameResolverForBinary()
        {
            // Setup
            var dbType = DbType.Binary;

            // Act
            var name = m_resolver.Resolve(dbType).ToLowerInvariant();

            // Assert
            Assert.AreEqual("binary", name);
        }

        [TestMethod]
        public void TestDbTypeToSqlServerStringNameResolverForBoolean()
        {
            // Setup
            var dbType = DbType.Boolean;

            // Act
            var name = m_resolver.Resolve(dbType).ToLowerInvariant();

            // Assert
            Assert.AreEqual("bit", name);
        }

        [TestMethod]
        public void TestDbTypeToSqlServerStringNameResolverForAnsiStringFixedLength()
        {
            // Setup
            var dbType = DbType.AnsiStringFixedLength;

            // Act
            var name = m_resolver.Resolve(dbType).ToLowerInvariant();

            // Assert
            Assert.AreEqual("char", name);
        }

        [TestMethod]
        public void TestDbTypeToSqlServerStringNameResolverForDate()
        {
            // Setup
            var dbType = DbType.Date;

            // Act
            var name = m_resolver.Resolve(dbType).ToLowerInvariant();

            // Assert
            Assert.AreEqual("date", name);
        }

        [TestMethod]
        public void TestDbTypeToSqlServerStringNameResolverForDateTime()
        {
            // Setup
            var dbType = DbType.DateTime;

            // Act
            var name = m_resolver.Resolve(dbType).ToLowerInvariant();

            // Assert
            Assert.AreEqual("datetime", name);
        }

        [TestMethod]
        public void TestDbTypeToSqlServerStringNameResolverForDateTime2()
        {
            // Setup
            var dbType = DbType.DateTime2;

            // Act
            var name = m_resolver.Resolve(dbType).ToLowerInvariant();

            // Assert
            Assert.AreEqual("datetime2", name);
        }

        [TestMethod]
        public void TestDbTypeToSqlServerStringNameResolverForDateTimeOffset()
        {
            // Setup
            var dbType = DbType.DateTimeOffset;

            // Act
            var name = m_resolver.Resolve(dbType).ToLowerInvariant();

            // Assert
            Assert.AreEqual("datetimeoffset", name);
        }

        [TestMethod]
        public void TestDbTypeToSqlServerStringNameResolverForDouble()
        {
            // Setup
            var dbType = DbType.Double;

            // Act
            var name = m_resolver.Resolve(dbType).ToLowerInvariant();

            // Assert
            Assert.AreEqual("float", name);
        }

        [TestMethod]
        public void TestDbTypeToSqlServerStringNameResolverForInt()
        {
            // Setup
            var dbType = DbType.Int32;

            // Act
            var name = m_resolver.Resolve(dbType).ToLowerInvariant();

            // Assert
            Assert.AreEqual("int", name);
        }

        [TestMethod]
        public void TestDbTypeToSqlServerStringNameResolverForStringFixedLength()
        {
            // Setup
            var dbType = DbType.StringFixedLength;

            // Act
            var name = m_resolver.Resolve(dbType).ToLowerInvariant();

            // Assert
            Assert.AreEqual("nchar", name);
        }

        [TestMethod]
        public void TestDbTypeToSqlServerStringNameResolverForSingle()
        {
            // Setup
            var dbType = DbType.Single;

            // Act
            var name = m_resolver.Resolve(dbType).ToLowerInvariant();

            // Assert
            Assert.AreEqual("real", name);
        }

        [TestMethod]
        public void TestDbTypeToSqlServerStringNameResolverForString()
        {
            // Setup
            var dbType = DbType.String;

            // Act
            var name = m_resolver.Resolve(dbType).ToLowerInvariant();

            // Assert
            Assert.AreEqual("nvarchar", name);
        }

        [TestMethod]
        public void TestDbTypeToSqlServerStringNameResolverForObject()
        {
            // Setup
            var dbType = DbType.Object;

            // Act
            var name = m_resolver.Resolve(dbType).ToLowerInvariant();

            // Assert
            Assert.AreEqual("object", name);
        }

        [TestMethod]
        public void TestDbTypeToSqlServerStringNameResolverForTime()
        {
            // Setup
            var dbType = DbType.Time;

            // Act
            var name = m_resolver.Resolve(dbType).ToLowerInvariant();

            // Assert
            Assert.AreEqual("time", name);
        }

        [TestMethod]
        public void TestDbTypeToSqlServerStringNameResolverForByte()
        {
            // Setup
            var dbType = DbType.Byte;

            // Act
            var name = m_resolver.Resolve(dbType).ToLowerInvariant();

            // Assert
            Assert.AreEqual("tinyint", name);
        }

        [TestMethod]
        public void TestDbTypeToSqlServerStringNameResolverForGuid()
        {
            // Setup
            var dbType = DbType.Guid;

            // Act
            var name = m_resolver.Resolve(dbType).ToLowerInvariant();

            // Assert
            Assert.AreEqual("uniqueidentifier", name);
        }

        [TestMethod]
        public void TestDbTypeToSqlServerStringNameResolverForAnsiString()
        {
            // Setup
            var dbType = DbType.AnsiString;

            // Act
            var name = m_resolver.Resolve(dbType).ToLowerInvariant();

            // Assert
            Assert.AreEqual("varchar", name);
        }

        [TestMethod]
        public void TestDbTypeToSqlServerStringNameResolverForXml()
        {
            // Setup
            var dbType = DbType.Xml;

            // Act
            var name = m_resolver.Resolve(dbType).ToLowerInvariant();

            // Assert
            Assert.AreEqual("xml", name);
        }
    }
}
