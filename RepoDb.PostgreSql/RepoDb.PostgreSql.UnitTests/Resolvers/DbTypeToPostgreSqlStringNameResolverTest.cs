using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using System.Data;

namespace RepoDb.PostgreSql.UnitTests.Resolvers
{
    [TestClass]
    public class DbTypeToPostgreSqlStringNameResolverTest
    {
        [TestMethod]
        public void TestDbTypeToPostgreSqlStringNameResolverInt64()
        {
            // Setup
            var resolver = new DbTypeToPostgreSqlStringNameResolver();

            // Assert
            Assert.AreEqual("BIGINT", resolver.Resolve(DbType.Int64));
        }

        [TestMethod]
        public void TestDbTypeToPostgreSqlStringNameResolverByte()
        {
            // Setup
            var resolver = new DbTypeToPostgreSqlStringNameResolver();

            // Assert
            Assert.AreEqual("BYTEA", resolver.Resolve(DbType.Byte));
        }

        [TestMethod]
        public void TestDbTypeToPostgreSqlStringNameResolverBinary()
        {
            // Setup
            var resolver = new DbTypeToPostgreSqlStringNameResolver();

            // Assert
            Assert.AreEqual("BYTEA", resolver.Resolve(DbType.Binary));
        }

        [TestMethod]
        public void TestDbTypeToPostgreSqlStringNameResolverBoolean()
        {
            // Setup
            var resolver = new DbTypeToPostgreSqlStringNameResolver();

            // Assert
            Assert.AreEqual("BOOLEAN", resolver.Resolve(DbType.Boolean));
        }

        [TestMethod]
        public void TestDbTypeToPostgreSqlStringNameResolverString()
        {
            // Setup
            var resolver = new DbTypeToPostgreSqlStringNameResolver();

            // Assert
            Assert.AreEqual("TEXT", resolver.Resolve(DbType.String));
        }

        [TestMethod]
        public void TestDbTypeToPostgreSqlStringNameResolverAnsiString()
        {
            // Setup
            var resolver = new DbTypeToPostgreSqlStringNameResolver();

            // Assert
            Assert.AreEqual("TEXT", resolver.Resolve(DbType.AnsiString));
        }

        [TestMethod]
        public void TestDbTypeToPostgreSqlStringNameResolverAnsiStringFixedLength()
        {
            // Setup
            var resolver = new DbTypeToPostgreSqlStringNameResolver();

            // Assert
            Assert.AreEqual("TEXT", resolver.Resolve(DbType.AnsiStringFixedLength));
        }

        [TestMethod]
        public void TestDbTypeToPostgreSqlStringNameResolverStringFixedLength()
        {
            // Setup
            var resolver = new DbTypeToPostgreSqlStringNameResolver();

            // Assert
            Assert.AreEqual("TEXT", resolver.Resolve(DbType.StringFixedLength));
        }

        [TestMethod]
        public void TestDbTypeToPostgreSqlStringNameResolverDate()
        {
            // Setup
            var resolver = new DbTypeToPostgreSqlStringNameResolver();

            // Assert
            Assert.AreEqual("DATE", resolver.Resolve(DbType.Date));
        }

        [TestMethod]
        public void TestDbTypeToPostgreSqlStringNameResolverDateTime()
        {
            // Setup
            var resolver = new DbTypeToPostgreSqlStringNameResolver();

            // Assert
            Assert.AreEqual("TIMESTAMP", resolver.Resolve(DbType.DateTime));
        }

        [TestMethod]
        public void TestDbTypeToPostgreSqlStringNameResolverDateTime2()
        {
            // Setup
            var resolver = new DbTypeToPostgreSqlStringNameResolver();

            // Assert
            Assert.AreEqual("TIMESTAMP", resolver.Resolve(DbType.DateTime2));
        }

        [TestMethod]
        public void TestDbTypeToPostgreSqlStringNameResolverDateTimeOffset()
        {
            // Setup
            var resolver = new DbTypeToPostgreSqlStringNameResolver();

            // Assert
            Assert.AreEqual("TIMESTAMPTZ", resolver.Resolve(DbType.DateTimeOffset));
        }

        [TestMethod]
        public void TestDbTypeToPostgreSqlStringNameResolverDecimal()
        {
            // Setup
            var resolver = new DbTypeToPostgreSqlStringNameResolver();

            // Assert
            Assert.AreEqual("NUMERIC", resolver.Resolve(DbType.Decimal));
        }

        [TestMethod]
        public void TestDbTypeToPostgreSqlStringNameResolverSingle()
        {
            // Setup
            var resolver = new DbTypeToPostgreSqlStringNameResolver();

            // Assert
            Assert.AreEqual("REAL", resolver.Resolve(DbType.Single));
        }

        [TestMethod]
        public void TestDbTypeToPostgreSqlStringNameResolverDouble()
        {
            // Setup
            var resolver = new DbTypeToPostgreSqlStringNameResolver();

            // Assert
            Assert.AreEqual("DOUBLE PRECISION", resolver.Resolve(DbType.Double));
        }

        [TestMethod]
        public void TestDbTypeToPostgreSqlStringNameResolverInt32()
        {
            // Setup
            var resolver = new DbTypeToPostgreSqlStringNameResolver();

            // Assert
            Assert.AreEqual("INTEGER", resolver.Resolve(DbType.Int32));
        }

        [TestMethod]
        public void TestDbTypeToPostgreSqlStringNameResolverInt16()
        {
            // Setup
            var resolver = new DbTypeToPostgreSqlStringNameResolver();

            // Assert
            Assert.AreEqual("SMALLINT", resolver.Resolve(DbType.Int16));
        }

        [TestMethod]
        public void TestDbTypeToPostgreSqlStringNameResolverTime()
        {
            // Setup
            var resolver = new DbTypeToPostgreSqlStringNameResolver();

            // Assert
            Assert.AreEqual("INTERVAL", resolver.Resolve(DbType.Time));
        }
    }
}
