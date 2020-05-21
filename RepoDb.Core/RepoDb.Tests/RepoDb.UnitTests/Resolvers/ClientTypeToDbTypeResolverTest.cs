using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using RepoDb.Types;
using System;
using System.Data;

namespace RepoDb.UnitTests.Resolvers
{
    [TestClass]
    public class ClientTypeToDbTypeResolverTest
    {
        /*
         * Taken:
         * https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/sql-server-data-type-mappings
         */

        private readonly ClientTypeToDbTypeResolver m_resolver = new ClientTypeToDbTypeResolver();

        [TestMethod]
        public void TestClientTypeToDbTypeResolverForBigInt()
        {
            // Setup
            var clientType = typeof(long);

            // Act
            var dbType = m_resolver.Resolve(clientType);

            // Assert
            Assert.AreEqual(DbType.Int64, dbType);
        }

        [TestMethod]
        public void TestClientTypeToDbTypeResolverForByte()
        {
            // Setup
            var clientType = typeof(byte);

            // Act
            var dbType = m_resolver.Resolve(clientType);

            // Assert
            Assert.AreEqual(DbType.Byte, dbType);
        }

        [TestMethod]
        public void TestClientTypeToDbTypeResolverForBytes()
        {
            // Setup
            var clientType = typeof(byte[]);

            // Act
            var dbType = m_resolver.Resolve(clientType);

            // Assert
            Assert.AreEqual(DbType.Binary, dbType);
        }

        [TestMethod]
        public void TestClientTypeToDbTypeResolverForBoolean()
        {
            // Setup
            var clientType = typeof(bool);

            // Act
            var dbType = m_resolver.Resolve(clientType);

            // Assert
            Assert.AreEqual(DbType.Boolean, dbType);
        }

        [TestMethod]
        public void TestClientTypeToDbTypeResolverForString()
        {
            // Setup
            var clientType = typeof(string);

            // Act
            var dbType = m_resolver.Resolve(clientType);

            // Assert
            Assert.AreEqual(DbType.String, dbType);
        }

        [TestMethod]
        public void TestClientTypeToDbTypeResolverForChars()
        {
            // Setup
            var clientType = typeof(char[]);

            // Act
            var dbType = m_resolver.Resolve(clientType);

            // Assert
            Assert.AreEqual(DbType.String, dbType);
        }

        [TestMethod]
        public void TestClientTypeToDbTypeResolverForDateTime()
        {
            // Setup
            var clientType = typeof(DateTime);

            // Act
            var dbType = m_resolver.Resolve(clientType);

            // Assert
            Assert.AreEqual(DbType.DateTime, dbType);
        }

        [TestMethod]
        public void TestClientTypeToDbTypeResolverForDateTimeOffset()
        {
            // Setup
            var clientType = typeof(DateTimeOffset);

            // Act
            var dbType = m_resolver.Resolve(clientType);

            // Assert
            Assert.AreEqual(DbType.DateTimeOffset, dbType);
        }

        [TestMethod]
        public void TestClientTypeToDbTypeResolverForDecimal()
        {
            // Setup
            var clientType = typeof(decimal);

            // Act
            var dbType = m_resolver.Resolve(clientType);

            // Assert
            Assert.AreEqual(DbType.Decimal, dbType);
        }

        [TestMethod]
        public void TestClientTypeToDbTypeResolverForDouble()
        {
            // Setup
            var clientType = typeof(double);

            // Act
            var dbType = m_resolver.Resolve(clientType);

            // Assert
            Assert.AreEqual(DbType.Double, dbType);
        }

        [TestMethod]
        public void TestClientTypeToDbTypeResolverForInt()
        {
            // Setup
            var clientType = typeof(int);

            // Act
            var dbType = m_resolver.Resolve(clientType);

            // Assert
            Assert.AreEqual(DbType.Int32, dbType);
        }

        [TestMethod]
        public void TestClientTypeToDbTypeResolverForSingle()
        {
            // Setup
            var clientType = typeof(float);

            // Act
            var dbType = m_resolver.Resolve(clientType);

            // Assert
            Assert.AreEqual(DbType.Single, dbType);
        }

        [TestMethod]
        public void TestClientTypeToDbTypeResolverForShort()
        {
            // Setup
            var clientType = typeof(short);

            // Act
            var dbType = m_resolver.Resolve(clientType);

            // Assert
            Assert.AreEqual(DbType.Int16, dbType);
        }

        [TestMethod]
        public void TestClientTypeToDbTypeResolverForObject()
        {
            // Setup
            var clientType = typeof(object);

            // Act
            var dbType = m_resolver.Resolve(clientType);

            // Assert
            Assert.AreEqual(DbType.String, dbType);
        }

        [TestMethod]
        public void TestClientTypeToDbTypeResolverForTimeSpan()
        {
            // Setup
            var clientType = typeof(TimeSpan);

            // Act
            var dbType = m_resolver.Resolve(clientType);

            // Assert
            Assert.AreEqual(DbType.Time, dbType);
        }

        [TestMethod]
        public void TestClientTypeToDbTypeResolverForGuid()
        {
            // Setup
            var clientType = typeof(Guid);

            // Act
            var dbType = m_resolver.Resolve(clientType);

            // Assert
            Assert.AreEqual(DbType.Guid, dbType);
        }

        [TestMethod]
        public void TestClientTypeToDbTypeResolverForSqlVariant()
        {
            // Setup
            var clientType = typeof(SqlVariant);

            // Act
            var dbType = m_resolver.Resolve(clientType);

            // Assert
            Assert.AreEqual(DbType.Object, dbType);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowOnExceptionClientTypeToDbTypeResolverIfTypeIsNull()
        {
            // Act
            m_resolver.Resolve(null);
        }
    }
}
