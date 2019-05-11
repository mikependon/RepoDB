using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Types;
using System;
using System.Data;

namespace RepoDb.UnitTests.Resolvers
{
    [TestClass]
    public class ClientTypeToSqlDbTypeResolverTest
    {
        /*
         * Taken:
         * https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/sql-server-data-type-mappings
         */

        private readonly ClientTypeToSqlDbTypeResolver m_resolver = new ClientTypeToSqlDbTypeResolver();

        [TestMethod]
        public void TestClientTypeToSqlDbTypeResolverForBigInt()
        {
            // Setup
            var clientType = typeof(long);

            // Act
            var dbType = m_resolver.Resolve(clientType);

            // Assert
            Assert.AreEqual(DbType.Int64, dbType);
        }

        [TestMethod]
        public void TestClientTypeToSqlDbTypeResolverForByte()
        {
            // Setup
            var clientType = typeof(byte);

            // Act
            var dbType = m_resolver.Resolve(clientType);

            // Assert
            Assert.AreEqual(DbType.Byte, dbType);
        }

        [TestMethod]
        public void TestClientTypeToSqlDbTypeResolverForBytes()
        {
            // Setup
            var clientType = typeof(byte[]);

            // Act
            var dbType = m_resolver.Resolve(clientType);

            // Assert
            Assert.AreEqual(DbType.Binary, dbType);
        }

        [TestMethod]
        public void TestClientTypeToSqlDbTypeResolverForBoolean()
        {
            // Setup
            var clientType = typeof(bool);

            // Act
            var dbType = m_resolver.Resolve(clientType);

            // Assert
            Assert.AreEqual(DbType.Boolean, dbType);
        }

        [TestMethod]
        public void TestClientTypeToSqlDbTypeResolverForString()
        {
            // Setup
            var clientType = typeof(string);

            // Act
            var dbType = m_resolver.Resolve(clientType);

            // Assert
            Assert.AreEqual(DbType.String, dbType);
        }

        [TestMethod]
        public void TestClientTypeToSqlDbTypeResolverForChars()
        {
            // Setup
            var clientType = typeof(char[]);

            // Act
            var dbType = m_resolver.Resolve(clientType);

            // Assert
            Assert.AreEqual(DbType.String, dbType);
        }

        [TestMethod]
        public void TestClientTypeToSqlDbTypeResolverForDateTime()
        {
            // Setup
            var clientType = typeof(DateTime);

            // Act
            var dbType = m_resolver.Resolve(clientType);

            // Assert
            Assert.AreEqual(DbType.DateTime, dbType);
        }

        [TestMethod]
        public void TestClientTypeToSqlDbTypeResolverForDateTimeOffset()
        {
            // Setup
            var clientType = typeof(DateTimeOffset);

            // Act
            var dbType = m_resolver.Resolve(clientType);

            // Assert
            Assert.AreEqual(DbType.DateTimeOffset, dbType);
        }

        [TestMethod]
        public void TestClientTypeToSqlDbTypeResolverForDecimal()
        {
            // Setup
            var clientType = typeof(decimal);

            // Act
            var dbType = m_resolver.Resolve(clientType);

            // Assert
            Assert.AreEqual(DbType.Decimal, dbType);
        }

        [TestMethod]
        public void TestClientTypeToSqlDbTypeResolverForDouble()
        {
            // Setup
            var clientType = typeof(double);

            // Act
            var dbType = m_resolver.Resolve(clientType);

            // Assert
            Assert.AreEqual(DbType.Double, dbType);
        }

        [TestMethod]
        public void TestClientTypeToSqlDbTypeResolverForInt()
        {
            // Setup
            var clientType = typeof(int);

            // Act
            var dbType = m_resolver.Resolve(clientType);

            // Assert
            Assert.AreEqual(DbType.Int32, dbType);
        }

        [TestMethod]
        public void TestClientTypeToSqlDbTypeResolverForSingle()
        {
            // Setup
            var clientType = typeof(float);

            // Act
            var dbType = m_resolver.Resolve(clientType);

            // Assert
            Assert.AreEqual(DbType.Single, dbType);
        }

        [TestMethod]
        public void TestClientTypeToSqlDbTypeResolverForShort()
        {
            // Setup
            var clientType = typeof(short);

            // Act
            var dbType = m_resolver.Resolve(clientType);

            // Assert
            Assert.AreEqual(DbType.Int16, dbType);
        }

        [TestMethod]
        public void TestClientTypeToSqlDbTypeResolverForObject()
        {
            // Setup
            var clientType = typeof(object);

            // Act
            var dbType = m_resolver.Resolve(clientType);

            // Assert
            Assert.AreEqual(DbType.String, dbType);
        }

        [TestMethod]
        public void TestClientTypeToSqlDbTypeResolverForTimeSpan()
        {
            // Setup
            var clientType = typeof(TimeSpan);

            // Act
            var dbType = m_resolver.Resolve(clientType);

            // Assert
            Assert.AreEqual(DbType.Time, dbType);
        }

        [TestMethod]
        public void TestClientTypeToSqlDbTypeResolverForGuid()
        {
            // Setup
            var clientType = typeof(Guid);

            // Act
            var dbType = m_resolver.Resolve(clientType);

            // Assert
            Assert.AreEqual(DbType.Guid, dbType);
        }

        [TestMethod]
        public void TestClientTypeToSqlDbTypeResolverForSqlVariant()
        {
            // Setup
            var clientType = typeof(SqlVariant);

            // Act
            var dbType = m_resolver.Resolve(clientType);

            // Assert
            Assert.AreEqual(DbType.Object, dbType);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowOnExceptionTestClientTypeToSqlDbTypeResolverIfTypeIsNull()
        {
            // Act
            m_resolver.Resolve(null);
        }
    }
}
