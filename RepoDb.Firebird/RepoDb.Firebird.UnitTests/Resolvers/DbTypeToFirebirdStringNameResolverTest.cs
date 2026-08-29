using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using System.Data;

namespace RepoDb.Firebird.UnitTests.Resolvers
{
    [TestClass]
    public class DbTypeToFirebirdStringNameResolverTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseFirebird();
        }

        [TestMethod]
        public void TestDbTypeToFirebirdStringNameResolverForInt64()
        {
            // Setup
            var resolver = new DbTypeToFirebirdStringNameResolver();

            // Act
            var result = resolver.Resolve(DbType.Int64);

            // Assert
            Assert.AreEqual("BIGINT", result);
        }

        [TestMethod]
        public void TestDbTypeToFirebirdStringNameResolverForInt32()
        {
            // Setup
            var resolver = new DbTypeToFirebirdStringNameResolver();

            // Act
            var result = resolver.Resolve(DbType.Int32);

            // Assert
            Assert.AreEqual("INTEGER", result);
        }

        [TestMethod]
        public void TestDbTypeToFirebirdStringNameResolverForInt16()
        {
            // Setup
            var resolver = new DbTypeToFirebirdStringNameResolver();

            // Act
            var result = resolver.Resolve(DbType.Int16);

            // Assert
            Assert.AreEqual("SMALLINT", result);
        }

        [TestMethod]
        public void TestDbTypeToFirebirdStringNameResolverForByte()
        {
            // Setup
            var resolver = new DbTypeToFirebirdStringNameResolver();

            // Act
            var result = resolver.Resolve(DbType.Byte);

            // Assert - Firebird has no TINYINT; the next-widest exact type is SMALLINT.
            Assert.AreEqual("SMALLINT", result);
        }

        [TestMethod]
        public void TestDbTypeToFirebirdStringNameResolverForDouble()
        {
            // Setup
            var resolver = new DbTypeToFirebirdStringNameResolver();

            // Act
            var result = resolver.Resolve(DbType.Double);

            // Assert
            Assert.AreEqual("DOUBLE PRECISION", result);
        }

        [TestMethod]
        public void TestDbTypeToFirebirdStringNameResolverForSingle()
        {
            // Setup
            var resolver = new DbTypeToFirebirdStringNameResolver();

            // Act
            var result = resolver.Resolve(DbType.Single);

            // Assert
            Assert.AreEqual("FLOAT", result);
        }

        [TestMethod]
        public void TestDbTypeToFirebirdStringNameResolverForDecimal()
        {
            // Setup
            var resolver = new DbTypeToFirebirdStringNameResolver();

            // Act
            var result = resolver.Resolve(DbType.Decimal);

            // Assert
            Assert.AreEqual("DECIMAL(18,2)", result);
        }

        [TestMethod]
        public void TestDbTypeToFirebirdStringNameResolverForBoolean()
        {
            // Setup
            var resolver = new DbTypeToFirebirdStringNameResolver();

            // Act
            var result = resolver.Resolve(DbType.Boolean);

            // Assert
            Assert.AreEqual("BOOLEAN", result);
        }

        [TestMethod]
        public void TestDbTypeToFirebirdStringNameResolverForDate()
        {
            // Setup
            var resolver = new DbTypeToFirebirdStringNameResolver();

            // Act
            var result = resolver.Resolve(DbType.Date);

            // Assert
            Assert.AreEqual("DATE", result);
        }

        [TestMethod]
        public void TestDbTypeToFirebirdStringNameResolverForDateTime()
        {
            // Setup
            var resolver = new DbTypeToFirebirdStringNameResolver();

            // Act
            var result = resolver.Resolve(DbType.DateTime);

            // Assert
            Assert.AreEqual("TIMESTAMP", result);
        }

        [TestMethod]
        public void TestDbTypeToFirebirdStringNameResolverForDateTimeOffset()
        {
            // Setup
            var resolver = new DbTypeToFirebirdStringNameResolver();

            // Act
            var result = resolver.Resolve(DbType.DateTimeOffset);

            // Assert
            Assert.AreEqual("TIMESTAMP WITH TIME ZONE", result);
        }

        [TestMethod]
        public void TestDbTypeToFirebirdStringNameResolverForTime()
        {
            // Setup
            var resolver = new DbTypeToFirebirdStringNameResolver();

            // Act
            var result = resolver.Resolve(DbType.Time);

            // Assert
            Assert.AreEqual("TIME", result);
        }

        [TestMethod]
        public void TestDbTypeToFirebirdStringNameResolverForGuid()
        {
            // Setup
            var resolver = new DbTypeToFirebirdStringNameResolver();

            // Act
            var result = resolver.Resolve(DbType.Guid);

            // Assert
            Assert.AreEqual("CHAR(16) CHARACTER SET OCTETS", result);
        }

        [TestMethod]
        public void TestDbTypeToFirebirdStringNameResolverForBinary()
        {
            // Setup
            var resolver = new DbTypeToFirebirdStringNameResolver();

            // Act
            var result = resolver.Resolve(DbType.Binary);

            // Assert
            Assert.AreEqual("BLOB SUB_TYPE 0", result);
        }

        [TestMethod]
        public void TestDbTypeToFirebirdStringNameResolverForString()
        {
            // Setup
            var resolver = new DbTypeToFirebirdStringNameResolver();

            // Act
            var result = resolver.Resolve(DbType.String);

            // Assert
            Assert.AreEqual("VARCHAR(8191)", result);
        }

        [TestMethod]
        public void TestDbTypeToFirebirdStringNameResolverForAnsiStringFixedLength()
        {
            // Setup
            var resolver = new DbTypeToFirebirdStringNameResolver();

            // Act
            var result = resolver.Resolve(DbType.AnsiStringFixedLength);

            // Assert
            Assert.AreEqual("CHAR(8191)", result);
        }

        [TestMethod]
        public void TestDbTypeToFirebirdStringNameResolverForXml()
        {
            // Setup
            var resolver = new DbTypeToFirebirdStringNameResolver();

            // Act
            var result = resolver.Resolve(DbType.Xml);

            // Assert
            Assert.AreEqual("BLOB SUB_TYPE TEXT", result);
        }
    }
}
