using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using System;

namespace RepoDb.ClickHouse.UnitTests.Resolvers
{
    [TestClass]
    public class ClickHouseDbTypeNameToClientTypeResolverTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseClickHouse();
        }

        [TestMethod]
        public void TestClickHouseDbTypeNameToClientTypeResolverForInt8()
        {
            // Setup
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("Int8");

            // Assert
            Assert.AreEqual(typeof(sbyte), result);
        }

        [TestMethod]
        public void TestClickHouseDbTypeNameToClientTypeResolverForInt16()
        {
            // Setup
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("Int16");

            // Assert
            Assert.AreEqual(typeof(short), result);
        }

        [TestMethod]
        public void TestClickHouseDbTypeNameToClientTypeResolverForInt32()
        {
            // Setup
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("Int32");

            // Assert
            Assert.AreEqual(typeof(int), result);
        }

        [TestMethod]
        public void TestClickHouseDbTypeNameToClientTypeResolverForInt64()
        {
            // Setup
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("Int64");

            // Assert
            Assert.AreEqual(typeof(long), result);
        }

        [TestMethod]
        public void TestClickHouseDbTypeNameToClientTypeResolverForInt128()
        {
            // Setup
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("Int128");

            // Assert
            Assert.AreEqual(typeof(long), result);
        }

        [TestMethod]
        public void TestClickHouseDbTypeNameToClientTypeResolverForUInt8()
        {
            // Setup
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("UInt8");

            // Assert
            Assert.AreEqual(typeof(byte), result);
        }

        [TestMethod]
        public void TestClickHouseDbTypeNameToClientTypeResolverForUInt16()
        {
            // Setup
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("UInt16");

            // Assert
            Assert.AreEqual(typeof(ushort), result);
        }

        [TestMethod]
        public void TestClickHouseDbTypeNameToClientTypeResolverForUInt32()
        {
            // Setup
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("UInt32");

            // Assert
            Assert.AreEqual(typeof(uint), result);
        }

        [TestMethod]
        public void TestClickHouseDbTypeNameToClientTypeResolverForUInt64()
        {
            // Setup
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("UInt64");

            // Assert
            Assert.AreEqual(typeof(ulong), result);
        }

        [TestMethod]
        public void TestClickHouseDbTypeNameToClientTypeResolverForFloat32()
        {
            // Setup
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("Float32");

            // Assert
            Assert.AreEqual(typeof(float), result);
        }

        [TestMethod]
        public void TestClickHouseDbTypeNameToClientTypeResolverForFloat64()
        {
            // Setup
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("Float64");

            // Assert
            Assert.AreEqual(typeof(double), result);
        }

        [TestMethod]
        public void TestClickHouseDbTypeNameToClientTypeResolverForBool()
        {
            // Setup
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("Bool");

            // Assert
            Assert.AreEqual(typeof(bool), result);
        }

        [TestMethod]
        public void TestClickHouseDbTypeNameToClientTypeResolverForString()
        {
            // Setup
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("String");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestClickHouseDbTypeNameToClientTypeResolverForFixedString()
        {
            // Setup
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("FixedString(16)");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestClickHouseDbTypeNameToClientTypeResolverForDate()
        {
            // Setup
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("Date");

            // Assert
            Assert.AreEqual(typeof(DateTime), result);
        }

        [TestMethod]
        public void TestClickHouseDbTypeNameToClientTypeResolverForDateTime()
        {
            // Setup
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("DateTime");

            // Assert
            Assert.AreEqual(typeof(DateTime), result);
        }

        [TestMethod]
        public void TestClickHouseDbTypeNameToClientTypeResolverForDateTime64()
        {
            // Setup
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("DateTime64(3)");

            // Assert
            Assert.AreEqual(typeof(DateTime), result);
        }

        [TestMethod]
        public void TestClickHouseDbTypeNameToClientTypeResolverForDecimal()
        {
            // Setup
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("Decimal(18,2)");

            // Assert
            Assert.AreEqual(typeof(decimal), result);
        }

        [TestMethod]
        public void TestClickHouseDbTypeNameToClientTypeResolverForUuid()
        {
            // Setup
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("UUID");

            // Assert
            Assert.AreEqual(typeof(Guid), result);
        }

        [TestMethod]
        public void TestClickHouseDbTypeNameToClientTypeResolverForNullableWrapper()
        {
            // Setup
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("Nullable(Int32)");

            // Assert
            Assert.AreEqual(typeof(int), result);
        }

        [TestMethod]
        public void TestClickHouseDbTypeNameToClientTypeResolverForLowCardinalityWrapper()
        {
            // Setup
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("LowCardinality(String)");

            // Assert
            Assert.AreEqual(typeof(string), result);
        }

        [TestMethod]
        public void TestClickHouseDbTypeNameToClientTypeResolverForArrayFallsBackToObject()
        {
            // Setup
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("Array(String)");

            // Assert
            Assert.AreEqual(typeof(object), result);
        }

        [TestMethod]
        public void TestClickHouseDbTypeNameToClientTypeResolverForNone()
        {
            // Setup
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var result = resolver.Resolve("None");

            // Assert
            Assert.AreEqual(typeof(object), result);
        }
    }
}
