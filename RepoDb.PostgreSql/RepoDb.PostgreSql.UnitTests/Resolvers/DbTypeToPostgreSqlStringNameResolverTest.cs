using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using System.Data;

namespace RepoDb.PostgreSql.UnitTests.Resolvers
{
    [TestClass]
    public class DbTypeToPostgreSqlStringNameResolverTest
    {
        [TestMethod]
        public void TestDbTypeToSqLiteStringNameResolverInt64()
        {
            // Setup
            var resolver = new DbTypeToPostgreSqlStringNameResolver();

            // Assert
            Assert.AreEqual("BIGINT", resolver.Resolve(DbType.Int64));
        }

        [TestMethod]
        public void TestDbTypeToSqLiteStringNameResolverByte()
        {
            // Setup
            var resolver = new DbTypeToPostgreSqlStringNameResolver();

            // Assert
            Assert.AreEqual("BYTEA", resolver.Resolve(DbType.Byte));
        }

        [TestMethod]
        public void TestDbTypeToSqLiteStringNameResolverBinary()
        {
            // Setup
            var resolver = new DbTypeToPostgreSqlStringNameResolver();

            // Assert
            Assert.AreEqual("BYTEA", resolver.Resolve(DbType.Binary));
        }

        [TestMethod]
        public void TestDbTypeToSqLiteStringNameResolverBoolean()
        {
            // Setup
            var resolver = new DbTypeToPostgreSqlStringNameResolver();

            // Assert
            Assert.AreEqual("BOOLEAN", resolver.Resolve(DbType.Boolean));
        }

        [TestMethod]
        public void TestDbTypeToSqLiteStringNameResolverString()
        {
            // Setup
            var resolver = new DbTypeToPostgreSqlStringNameResolver();

            // Assert
            Assert.AreEqual("TEXT", resolver.Resolve(DbType.String));
        }

        [TestMethod]
        public void TestDbTypeToSqLiteStringNameResolverAnsiString()
        {
            // Setup
            var resolver = new DbTypeToPostgreSqlStringNameResolver();

            // Assert
            Assert.AreEqual("TEXT", resolver.Resolve(DbType.AnsiString));
        }

        [TestMethod]
        public void TestDbTypeToSqLiteStringNameResolverAnsiStringFixedLength()
        {
            // Setup
            var resolver = new DbTypeToPostgreSqlStringNameResolver();

            // Assert
            Assert.AreEqual("TEXT", resolver.Resolve(DbType.AnsiStringFixedLength));
        }

        [TestMethod]
        public void TestDbTypeToSqLiteStringNameResolverStringFixedLength()
        {
            // Setup
            var resolver = new DbTypeToPostgreSqlStringNameResolver();

            // Assert
            Assert.AreEqual("TEXT", resolver.Resolve(DbType.StringFixedLength));
        }

        [TestMethod]
        public void TestDbTypeToSqLiteStringNameResolverDate()
        {
            // Setup
            var resolver = new DbTypeToPostgreSqlStringNameResolver();

            // Assert
            Assert.AreEqual("DATE", resolver.Resolve(DbType.Date));
        }

        [TestMethod]
        public void TestDbTypeToSqLiteStringNameResolverDateTime()
        {
            // Setup
            var resolver = new DbTypeToPostgreSqlStringNameResolver();

            // Assert
            Assert.AreEqual("TIMESTAMP", resolver.Resolve(DbType.DateTime));
        }

        [TestMethod]
        public void TestDbTypeToSqLiteStringNameResolverDateTime2()
        {
            // Setup
            var resolver = new DbTypeToPostgreSqlStringNameResolver();

            // Assert
            Assert.AreEqual("TIMESTAMP", resolver.Resolve(DbType.DateTime2));
        }

        [TestMethod]
        public void TestDbTypeToSqLiteStringNameResolverDateTimeOffset()
        {
            // Setup
            var resolver = new DbTypeToPostgreSqlStringNameResolver();

            // Assert
            Assert.AreEqual("TIMESTAMPTZ", resolver.Resolve(DbType.DateTimeOffset));
        }

        [TestMethod]
        public void TestDbTypeToSqLiteStringNameResolverDecimal()
        {
            // Setup
            var resolver = new DbTypeToPostgreSqlStringNameResolver();

            // Assert
            Assert.AreEqual("NUMERIC", resolver.Resolve(DbType.Decimal));
        }

        [TestMethod]
        public void TestDbTypeToSqLiteStringNameResolverSingle()
        {
            // Setup
            var resolver = new DbTypeToPostgreSqlStringNameResolver();

            // Assert
            Assert.AreEqual("REAL", resolver.Resolve(DbType.Single));
        }

        [TestMethod]
        public void TestDbTypeToSqLiteStringNameResolverDouble()
        {
            // Setup
            var resolver = new DbTypeToPostgreSqlStringNameResolver();

            // Assert
            Assert.AreEqual("DOUBLE PRECISION", resolver.Resolve(DbType.Double));
        }

        [TestMethod]
        public void TestDbTypeToSqLiteStringNameResolverInt32()
        {
            // Setup
            var resolver = new DbTypeToPostgreSqlStringNameResolver();

            // Assert
            Assert.AreEqual("INTEGER", resolver.Resolve(DbType.Int32));
        }

        [TestMethod]
        public void TestDbTypeToSqLiteStringNameResolverInt16()
        {
            // Setup
            var resolver = new DbTypeToPostgreSqlStringNameResolver();

            // Assert
            Assert.AreEqual("SMALLINT", resolver.Resolve(DbType.Int16));
        }

        [TestMethod]
        public void TestDbTypeToSqLiteStringNameResolverTime()
        {
            // Setup
            var resolver = new DbTypeToPostgreSqlStringNameResolver();

            // Assert
            Assert.AreEqual("INTERVAL", resolver.Resolve(DbType.Time));
        }
    }
}
