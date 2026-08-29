using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using System;

namespace RepoDb.Vertica.UnitTests.Resolvers
{
    [TestClass]
    public class VerticaDbTypeNameToClientTypeResolverTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseVertica();
        }

        [TestMethod]
        public void TestVerticaDbTypeNameToClientTypeResolverForInt()
        {
            // Setup
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("int");

            // Assert
            Assert.AreEqual(typeof(long), result);
        }

        [TestMethod]
        public void TestVerticaDbTypeNameToClientTypeResolverForBigInt()
        {
            // Setup
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("bigint");

            // Assert
            Assert.AreEqual(typeof(long), result);
        }

        [TestMethod]
        public void TestVerticaDbTypeNameToClientTypeResolverForFloat()
        {
            // Setup
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("float");

            // Assert
            Assert.AreEqual(typeof(double), result);
        }

        [TestMethod]
        public void TestVerticaDbTypeNameToClientTypeResolverForDoublePrecision()
        {
            // Setup
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("double precision");

            // Assert
            Assert.AreEqual(typeof(double), result);
        }

        [TestMethod]
        public void TestVerticaDbTypeNameToClientTypeResolverForReal()
        {
            // Setup
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("real");

            // Assert
            Assert.AreEqual(typeof(double), result);
        }

        [TestMethod]
        public void TestVerticaDbTypeNameToClientTypeResolverForNumeric()
        {
            // Setup
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("numeric");

            // Assert
            Assert.AreEqual(typeof(decimal), result);
        }

        [TestMethod]
        public void TestVerticaDbTypeNameToClientTypeResolverForDecimal()
        {
            // Setup
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("decimal");

            // Assert
            Assert.AreEqual(typeof(decimal), result);
        }

        [TestMethod]
        public void TestVerticaDbTypeNameToClientTypeResolverForMoney()
        {
            // Setup
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("money");

            // Assert
            Assert.AreEqual(typeof(decimal), result);
        }

        [TestMethod]
        public void TestVerticaDbTypeNameToClientTypeResolverForChar()
        {
            // Setup
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("char");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestVerticaDbTypeNameToClientTypeResolverForVarChar()
        {
            // Setup
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("varchar");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestVerticaDbTypeNameToClientTypeResolverForLongVarChar()
        {
            // Setup
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("long varchar");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestVerticaDbTypeNameToClientTypeResolverForBinary()
        {
            // Setup
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("binary");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestVerticaDbTypeNameToClientTypeResolverForVarBinary()
        {
            // Setup
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("varbinary");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestVerticaDbTypeNameToClientTypeResolverForLongVarBinary()
        {
            // Setup
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("long varbinary");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
        }

        [TestMethod]
        public void TestVerticaDbTypeNameToClientTypeResolverForBoolean()
        {
            // Setup
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("boolean");

            // Assert
            Assert.AreEqual(typeof(bool), result);
        }

        [TestMethod]
        public void TestVerticaDbTypeNameToClientTypeResolverForDate()
        {
            // Setup
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("date");

            // Assert
            Assert.AreEqual(typeof(DateTime), result);
        }

        [TestMethod]
        public void TestVerticaDbTypeNameToClientTypeResolverForTime()
        {
            // Setup
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("time");

            // Assert
            Assert.AreEqual(typeof(DateTime), result);
        }

        [TestMethod]
        public void TestVerticaDbTypeNameToClientTypeResolverForTimeWithTimezone()
        {
            // Setup
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("time with timezone");

            // Assert
            Assert.AreEqual(typeof(DateTimeOffset), result);
        }

        [TestMethod]
        public void TestVerticaDbTypeNameToClientTypeResolverForTimestamp()
        {
            // Setup
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("timestamp");

            // Assert
            Assert.AreEqual(typeof(DateTime), result);
        }

        [TestMethod]
        public void TestVerticaDbTypeNameToClientTypeResolverForTimestampWithTimezone()
        {
            // Setup
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("timestamp with timezone");

            // Assert
            Assert.AreEqual(typeof(DateTimeOffset), result);
        }

        [TestMethod]
        public void TestVerticaDbTypeNameToClientTypeResolverForUuid()
        {
            // Setup
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("uuid");

            // Assert
            Assert.AreEqual(typeof(Guid), result);
        }

        [TestMethod]
        public void TestVerticaDbTypeNameToClientTypeResolverForNone()
        {
            // Setup
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("none");

            // Assert
            Assert.AreEqual(typeof(object), result);
        }

        [TestMethod]
        public void ThrowExceptionOnVerticaDbTypeNameToClientTypeResolverIfTheDbTypeNameIsNull()
        {
            // Setup
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            Assert.Throws<NullReferenceException>(() => resolver.Resolve(null));
        }
    }
}
