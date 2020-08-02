using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using System;

namespace RepoDb.SqLite.UnitTests.Resolvers
{
    [TestClass]
    public class SdsSqLiteDbTypeNameToClientTypeResolverTest
    {
        [TestInitialize]
        public void Initialize()
        {
            SqLiteBootstrap.Initialize();
        }

        [TestMethod]
        public void TestSqLiteDbTypeNameToClientTypeResolverForBigInt()
        {
            // Setup
            var resolver = new SdsSqLiteDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("BIGINT");

            // Assert
            Assert.AreEqual(typeof(long), result);
        }

        [TestMethod]
        public void TestSqLiteDbTypeNameToClientTypeResolverForInteger()
        {
            // Setup
            var resolver = new SdsSqLiteDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("INTEGER");

            // Assert
            Assert.AreEqual(typeof(long), result);
        }

        [TestMethod]
        public void TestSqLiteDbTypeNameToClientTypeResolverForBlob()
        {
            // Setup
            var resolver = new SdsSqLiteDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("BLOB");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestSqLiteDbTypeNameToClientTypeResolverForBoolean()
        {
            // Setup
            var resolver = new SdsSqLiteDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("BOOLEAN");

            // Assert
            Assert.AreEqual(typeof(long), result);
        }

        [TestMethod]
        public void TestSqLiteDbTypeNameToClientTypeResolverForChar()
        {
            // Setup
            var resolver = new SdsSqLiteDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("CHAR");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestSqLiteDbTypeNameToClientTypeResolverForString()
        {
            // Setup
            var resolver = new SdsSqLiteDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("STRING");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestSqLiteDbTypeNameToClientTypeResolverForText()
        {
            // Setup
            var resolver = new SdsSqLiteDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TEXT");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestSqLiteDbTypeNameToClientTypeResolverForVarChar()
        {
            // Setup
            var resolver = new SdsSqLiteDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("VARCHAR");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestSqLiteDbTypeNameToClientTypeResolverForDate()
        {
            // Setup
            var resolver = new SdsSqLiteDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("DATE");

            // Assert
            Assert.AreEqual(typeof(DateTime), result);
        }

        [TestMethod]
        public void TestSqLiteDbTypeNameToClientTypeResolverForDateTime()
        {
            // Setup
            var resolver = new SdsSqLiteDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("DATETIME");

            // Assert
            Assert.AreEqual(typeof(DateTime), result);
        }

        [TestMethod]
        public void TestSqLiteDbTypeNameToClientTypeResolverForTime()
        {
            // Setup
            var resolver = new SdsSqLiteDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("TIME");

            // Assert
            Assert.AreEqual(typeof(DateTime), result);
        }

        [TestMethod]
        public void TestSqLiteDbTypeNameToClientTypeResolverForDecimal()
        {
            // Setup
            var resolver = new SdsSqLiteDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("DECIMAL");

            // Assert
            Assert.AreEqual(typeof(decimal), result);
        }

        [TestMethod]
        public void TestSqLiteDbTypeNameToClientTypeResolverForNumeric()
        {
            // Setup
            var resolver = new SdsSqLiteDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("NUMERIC");

            // Assert
            Assert.AreEqual(typeof(decimal), result);
        }

        [TestMethod]
        public void TestSqLiteDbTypeNameToClientTypeResolverForDouble()
        {
            // Setup
            var resolver = new SdsSqLiteDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("DOUBLE");

            // Assert
            Assert.AreEqual(typeof(double), result);
        }

        [TestMethod]
        public void TestSqLiteDbTypeNameToClientTypeResolverForReal()
        {
            // Setup
            var resolver = new SdsSqLiteDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("REAL");

            // Assert
            Assert.AreEqual(typeof(double), result);
        }

        [TestMethod]
        public void TestSqLiteDbTypeNameToClientTypeResolverForInt()
        {
            // Setup
            var resolver = new SdsSqLiteDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("INT");

            // Assert
            Assert.AreEqual(typeof(int), result);
        }

        [TestMethod]
        public void TestSqLiteDbTypeNameToClientTypeResolverForNone()
        {
            // Setup
            var resolver = new SdsSqLiteDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("NONE");

            // Assert
            Assert.AreEqual(typeof(object), result);
        }

        [TestMethod]
        public void TestSqLiteDbTypeNameToClientTypeResolverForOther()
        {
            // Setup
            var resolver = new SdsSqLiteDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("WHATEVER");

            // Assert
            Assert.AreEqual(typeof(object), result);
        }
    }
}
