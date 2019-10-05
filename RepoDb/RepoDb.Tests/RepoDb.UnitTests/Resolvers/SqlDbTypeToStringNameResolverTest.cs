using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using System.Data;

namespace RepoDb.UnitTests.Resolvers
{
    [TestClass]
    public class SqlDbTypeToStringNameResolverTest
    {
        /*
         * Taken:
         * https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/sql-server-data-type-mappings
         */

        private readonly SqlDbTypeToStringNameResolver m_resolver = new SqlDbTypeToStringNameResolver();

        [TestMethod]
        public void TestSqlDbTypeToStringNameResolverForBigInt()
        {
            // Setup
            var dbType = DbType.Int64;

            // Act
            var name = m_resolver.Resolve(dbType).ToLower();

            // Assert
            Assert.AreEqual("bigint", name);
        }

        [TestMethod]
        public void TestSqlDbTypeToStringNameResolverForBinary()
        {
            // Setup
            var dbType = DbType.Binary;

            // Act
            var name = m_resolver.Resolve(dbType).ToLower();

            // Assert
            Assert.AreEqual("binary", name);
        }

        [TestMethod]
        public void TestSqlDbTypeToStringNameResolverForBoolean()
        {
            // Setup
            var dbType = DbType.Boolean;

            // Act
            var name = m_resolver.Resolve(dbType).ToLower();

            // Assert
            Assert.AreEqual("bit", name);
        }

        [TestMethod]
        public void TestSqlDbTypeToStringNameResolverForAnsiStringFixedLength()
        {
            // Setup
            var dbType = DbType.AnsiStringFixedLength;

            // Act
            var name = m_resolver.Resolve(dbType).ToLower();

            // Assert
            Assert.AreEqual("char", name);
        }

        [TestMethod]
        public void TestSqlDbTypeToStringNameResolverForDate()
        {
            // Setup
            var dbType = DbType.Date;

            // Act
            var name = m_resolver.Resolve(dbType).ToLower();

            // Assert
            Assert.AreEqual("date", name);
        }

        [TestMethod]
        public void TestSqlDbTypeToStringNameResolverForDateTime()
        {
            // Setup
            var dbType = DbType.DateTime;

            // Act
            var name = m_resolver.Resolve(dbType).ToLower();

            // Assert
            Assert.AreEqual("datetime", name);
        }

        [TestMethod]
        public void TestSqlDbTypeToStringNameResolverForDateTime2()
        {
            // Setup
            var dbType = DbType.DateTime2;

            // Act
            var name = m_resolver.Resolve(dbType).ToLower();

            // Assert
            Assert.AreEqual("datetime2", name);
        }

        [TestMethod]
        public void TestSqlDbTypeToStringNameResolverForDateTimeOffset()
        {
            // Setup
            var dbType = DbType.DateTimeOffset;

            // Act
            var name = m_resolver.Resolve(dbType).ToLower();

            // Assert
            Assert.AreEqual("datetimeoffset", name);
        }

        [TestMethod]
        public void TestSqlDbTypeToStringNameResolverForDouble()
        {
            // Setup
            var dbType = DbType.Double;

            // Act
            var name = m_resolver.Resolve(dbType).ToLower();

            // Assert
            Assert.AreEqual("float", name);
        }

        [TestMethod]
        public void TestSqlDbTypeToStringNameResolverForInt()
        {
            // Setup
            var dbType = DbType.Int32;

            // Act
            var name = m_resolver.Resolve(dbType).ToLower();

            // Assert
            Assert.AreEqual("int", name);
        }

        [TestMethod]
        public void TestSqlDbTypeToStringNameResolverForStringFixedLength()
        {
            // Setup
            var dbType = DbType.StringFixedLength;

            // Act
            var name = m_resolver.Resolve(dbType).ToLower();

            // Assert
            Assert.AreEqual("nchar", name);
        }

        [TestMethod]
        public void TestSqlDbTypeToStringNameResolverForSingle()
        {
            // Setup
            var dbType = DbType.Single;

            // Act
            var name = m_resolver.Resolve(dbType).ToLower();

            // Assert
            Assert.AreEqual("real", name);
        }

        [TestMethod]
        public void TestSqlDbTypeToStringNameResolverForString()
        {
            // Setup
            var dbType = DbType.String;

            // Act
            var name = m_resolver.Resolve(dbType).ToLower();

            // Assert
            Assert.AreEqual("nvarchar", name);
        }

        [TestMethod]
        public void TestSqlDbTypeToStringNameResolverForObject()
        {
            // Setup
            var dbType = DbType.Object;

            // Act
            var name = m_resolver.Resolve(dbType).ToLower();

            // Assert
            Assert.AreEqual("object", name);
        }

        [TestMethod]
        public void TestSqlDbTypeToStringNameResolverForTime()
        {
            // Setup
            var dbType = DbType.Time;

            // Act
            var name = m_resolver.Resolve(dbType).ToLower();

            // Assert
            Assert.AreEqual("time", name);
        }

        [TestMethod]
        public void TestSqlDbTypeToStringNameResolverForByte()
        {
            // Setup
            var dbType = DbType.Byte;

            // Act
            var name = m_resolver.Resolve(dbType).ToLower();

            // Assert
            Assert.AreEqual("tinyint", name);
        }

        [TestMethod]
        public void TestSqlDbTypeToStringNameResolverForGuid()
        {
            // Setup
            var dbType = DbType.Guid;

            // Act
            var name = m_resolver.Resolve(dbType).ToLower();

            // Assert
            Assert.AreEqual("uniqueidentifier", name);
        }

        [TestMethod]
        public void TestSqlDbTypeToStringNameResolverForAnsiString()
        {
            // Setup
            var dbType = DbType.AnsiString;

            // Act
            var name = m_resolver.Resolve(dbType).ToLower();

            // Assert
            Assert.AreEqual("varchar", name);
        }

        [TestMethod]
        public void TestSqlDbTypeToStringNameResolverForXml()
        {
            // Setup
            var dbType = DbType.Xml;

            // Act
            var name = m_resolver.Resolve(dbType).ToLower();

            // Assert
            Assert.AreEqual("xml", name);
        }
    }
}
