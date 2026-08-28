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
        public void TestVerticaDbTypeNameToClientTypeResolverForSmallInt()
        {
            // Setup
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("smallint");

            // Assert
            Assert.AreEqual(typeof(short), result);
        }

        [TestMethod]
        public void TestVerticaDbTypeNameToClientTypeResolverForInteger()
        {
            // Setup
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("integer");

            // Assert
            Assert.AreEqual(typeof(int), result);
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
        public void TestVerticaDbTypeNameToClientTypeResolverForInt128()
        {
            // Setup
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("int128");

            // Assert
            Assert.AreEqual(typeof(System.Numerics.BigInteger), result);
        }

        [TestMethod]
        public void TestVerticaDbTypeNameToClientTypeResolverForFloat()
        {
            // Setup
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("float");

            // Assert
            Assert.AreEqual(typeof(float), result);
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
        public void TestVerticaDbTypeNameToClientTypeResolverForDec16()
        {
            // Setup
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("dec16");

            // Assert
            Assert.AreEqual(typeof(decimal), result);
        }

        [TestMethod]
        public void TestVerticaDbTypeNameToClientTypeResolverForDec34()
        {
            // Setup
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("dec34");

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
        public void TestVerticaDbTypeNameToClientTypeResolverForBlobText()
        {
            // Setup
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("blob_text");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestVerticaDbTypeNameToClientTypeResolverForBlobBinary()
        {
            // Setup
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("blob_binary");

            // Assert
            Assert.AreEqual(typeof(byte[]), result);
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
            Assert.AreEqual(typeof(TimeSpan), result);
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
        public void TestVerticaDbTypeNameToClientTypeResolverForTimeTz()
        {
            // Setup
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("time_tz");

            // Assert
            Assert.AreEqual(typeof(DateTimeOffset), result);
        }

        [TestMethod]
        public void TestVerticaDbTypeNameToClientTypeResolverForTimestampTz()
        {
            // Setup
            var resolver = new VerticaDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("timestamp_tz");

            // Assert
            Assert.AreEqual(typeof(DateTimeOffset), result);
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
