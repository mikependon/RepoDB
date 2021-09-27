using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;

namespace RepoDb.SqLite.UnitTests.Resolvers
{
    [TestClass]
    public class MdsSqLiteDbTypeNameToClientTypeResolverTest
    {
        [TestInitialize]
        public void Initialize()
        {
            SqLiteBootstrap.Initialize();
        }

        [TestMethod]
        public void TestMdsSqLiteDbTypeNameToClientTypeResolverForBigInt()
        {
            // Setup
            var resolver = new MdsSqLiteDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("BIGINT");

            // Assert
            Assert.AreEqual(typeof(long), result);
        }

        [TestMethod]
        public void TestMdsSqLiteDbTypeNameToClientTypeResolverForInteger()
        {
            // Setup
            var resolver = new MdsSqLiteDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("INTEGER");

            // Assert
            Assert.AreEqual(typeof(long), result);
        }

        [TestMethod]
        public void TestMdsSqLiteDbTypeNameToClientTypeResolverForBlob()
        {
            // Setup
            var resolver = new MdsSqLiteDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("BLOB");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestMdsSqLiteDbTypeNameToClientTypeResolverForBoolean()
        {
            // Setup
            var resolver = new MdsSqLiteDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("BOOLEAN");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestMdsSqLiteDbTypeNameToClientTypeResolverForChar()
        {
            // Setup
            var resolver = new MdsSqLiteDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("CHAR");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestMdsSqLiteDbTypeNameToClientTypeResolverForString()
        {
            // Setup
            var resolver = new MdsSqLiteDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("STRING");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestMdsSqLiteDbTypeNameToClientTypeResolverForText()
        {
            // Setup
            var resolver = new MdsSqLiteDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TEXT");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestMdsSqLiteDbTypeNameToClientTypeResolverForVarChar()
        {
            // Setup
            var resolver = new MdsSqLiteDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("VARCHAR");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestMdsSqLiteDbTypeNameToClientTypeResolverForDate()
        {
            // Setup
            var resolver = new MdsSqLiteDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("DATE");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestMdsSqLiteDbTypeNameToClientTypeResolverForDateTime()
        {
            // Setup
            var resolver = new MdsSqLiteDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("DATETIME");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestMdsSqLiteDbTypeNameToClientTypeResolverForTime()
        {
            // Setup
            var resolver = new MdsSqLiteDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TIME");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestMdsSqLiteDbTypeNameToClientTypeResolverForDecimal()
        {
            // Setup
            var resolver = new MdsSqLiteDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("DECIMAL");

            // Assert
            Assert.AreEqual(typeof(long), result);
        }

        [TestMethod]
        public void TestMdsSqLiteDbTypeNameToClientTypeResolverForNumeric()
        {
            // Setup
            var resolver = new MdsSqLiteDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("NUMERIC");

            // Assert
            Assert.AreEqual(typeof(long), result);
        }

        [TestMethod]
        public void TestMdsSqLiteDbTypeNameToClientTypeResolverForDouble()
        {
            // Setup
            var resolver = new MdsSqLiteDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("DOUBLE");

            // Assert
            Assert.AreEqual(typeof(double), result);
        }

        [TestMethod]
        public void TestMdsSqLiteDbTypeNameToClientTypeResolverForReal()
        {
            // Setup
            var resolver = new MdsSqLiteDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("REAL");

            // Assert
            Assert.AreEqual(typeof(double), result);
        }

        [TestMethod]
        public void TestMdsSqLiteDbTypeNameToClientTypeResolverForInt()
        {
            // Setup
            var resolver = new MdsSqLiteDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("INT");

            // Assert
            Assert.AreEqual(typeof(long), result);
        }

        [TestMethod]
        public void TestMdsSqLiteDbTypeNameToClientTypeResolverForNone()
        {
            // Setup
            var resolver = new MdsSqLiteDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("NONE");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestMdsSqLiteDbTypeNameToClientTypeResolverForOther()
        {
            // Setup
            var resolver = new MdsSqLiteDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("WHATEVER");

            // Assert
            Assert.AreEqual(typeof(object), result);
        }
    }
}
