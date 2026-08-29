using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;

namespace RepoDb.Vertica.UnitTests.Resolvers
{
    [TestClass]
    public class DbTypeNameToColumnNameResolverTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseVertica();
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForSmallInt()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act
            var result = resolver.Resolve("smallint");

            // Assert
            Assert.AreEqual("SMALLINT", result);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForInteger()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act
            var result = resolver.Resolve("integer");

            // Assert
            Assert.AreEqual("INTEGER", result);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForBigInt()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act
            var result = resolver.Resolve("bigint");

            // Assert
            Assert.AreEqual("BIGINT", result);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForBoolean()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act
            var result = resolver.Resolve("boolean");

            // Assert
            Assert.AreEqual("BOOLEAN", result);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForFloat()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act
            var result = resolver.Resolve("float");

            // Assert
            Assert.AreEqual("FLOAT", result);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForDoublePrecision()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act
            var result = resolver.Resolve("double precision");

            // Assert
            Assert.AreEqual("DOUBLE PRECISION", result);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForDate()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act
            var result = resolver.Resolve("date");

            // Assert
            Assert.AreEqual("DATE", result);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForTime()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act
            var result = resolver.Resolve("time");

            // Assert
            Assert.AreEqual("TIME", result);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForTimeWithTimeZone()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act
            var result = resolver.Resolve("time_tz");

            // Assert
            Assert.AreEqual("TIME WITH TIME ZONE", result);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForTimestamp()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act
            var result = resolver.Resolve("timestamp");

            // Assert
            Assert.AreEqual("TIMESTAMP", result);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForTimestampWithTimeZone()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act
            var result = resolver.Resolve("timestamp_tz");

            // Assert
            Assert.AreEqual("TIMESTAMP WITH TIME ZONE", result);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForNumeric()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act - the resolver returns the base keyword only; callers append (precision,scale) themselves.
            var result = resolver.Resolve("numeric");

            // Assert
            Assert.AreEqual("NUMERIC", result);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForDecimal()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act - the resolver returns the base keyword only; callers append (precision,scale) themselves.
            var result = resolver.Resolve("decimal");

            // Assert
            Assert.AreEqual("DECIMAL", result);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForDec16()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act
            var result = resolver.Resolve("dec16");

            // Assert
            Assert.AreEqual("DECFLOAT(16)", result);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForDec34()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act
            var result = resolver.Resolve("dec34");

            // Assert
            Assert.AreEqual("DECFLOAT(34)", result);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForInt128()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act
            var result = resolver.Resolve("int128");

            // Assert
            Assert.AreEqual("INT128", result);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForChar()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act - the resolver returns the base keyword only; callers append (size) themselves.
            var result = resolver.Resolve("char");

            // Assert
            Assert.AreEqual("CHAR", result);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForVarchar()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act - the resolver returns the base keyword only; callers append (size) themselves.
            var result = resolver.Resolve("varchar");

            // Assert
            Assert.AreEqual("VARCHAR", result);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForBinary()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act - binary maps onto the same base keyword as char; callers append the size and
            // "CHARACTER SET OCTETS" themselves.
            var result = resolver.Resolve("binary");

            // Assert
            Assert.AreEqual("CHAR", result);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForVarbinary()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act - varbinary maps onto the same base keyword as varchar; callers append the size and
            // "CHARACTER SET OCTETS" themselves.
            var result = resolver.Resolve("varbinary");

            // Assert
            Assert.AreEqual("VARCHAR", result);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForBlobBinary()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act
            var result = resolver.Resolve("blob_binary");

            // Assert
            Assert.AreEqual("BLOB SUB_TYPE 0", result);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForBlobText()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act
            var result = resolver.Resolve("blob_text");

            // Assert
            Assert.AreEqual("BLOB SUB_TYPE TEXT", result);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForUnrecognizedTypeName()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act - unrecognized type names fall back to the same safe catch-all as "blob_text".
            var result = resolver.Resolve("some_unknown_type");

            // Assert
            Assert.AreEqual("BLOB SUB_TYPE TEXT", result);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverForNull()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act
            var result = resolver.Resolve(null);

            // Assert
            Assert.AreEqual("BLOB SUB_TYPE TEXT", result);
        }

        [TestMethod]
        public void TestDbTypeNameToColumnNameResolverIsCaseInsensitive()
        {
            // Setup
            var resolver = new DbTypeNameToColumnNameResolver();

            // Act
            var result = resolver.Resolve("INTEGER");

            // Assert
            Assert.AreEqual("INTEGER", result);
        }
    }
}
