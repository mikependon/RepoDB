using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using System;

namespace RepoDb.Schema.ClickHouse.UnitTests.Resolvers
{
    /// <summary>
    /// Covers supported values and boundary cases.
    /// </summary>
    [TestClass]
    public class ClientTypeToClickHouseTypeNameResolverCoverageTest
    {
        #region Public Methods

        /// <summary>
        /// Resolves CLR types and nullable counterparts.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedAndNullableTypesForString()
        {
            // Setup
            Type type = typeof(string);
            string expected = "String";
            var resolver = new ClientTypeToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves CLR types and nullable counterparts.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedAndNullableTypesForInt32()
        {
            // Setup
            Type type = typeof(int);
            string expected = "Int32";
            var resolver = new ClientTypeToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves CLR types and nullable counterparts.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedAndNullableTypesForNullableInt32()
        {
            // Setup
            Type type = typeof(int?);
            string expected = "Int32";
            var resolver = new ClientTypeToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves CLR types and nullable counterparts.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedAndNullableTypesForInt64()
        {
            // Setup
            Type type = typeof(long);
            string expected = "Int64";
            var resolver = new ClientTypeToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves CLR types and nullable counterparts.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedAndNullableTypesForNullableInt64()
        {
            // Setup
            Type type = typeof(long?);
            string expected = "Int64";
            var resolver = new ClientTypeToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves CLR types and nullable counterparts.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedAndNullableTypesForInt16()
        {
            // Setup
            Type type = typeof(short);
            string expected = "Int16";
            var resolver = new ClientTypeToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves CLR types and nullable counterparts.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedAndNullableTypesForNullableInt16()
        {
            // Setup
            Type type = typeof(short?);
            string expected = "Int16";
            var resolver = new ClientTypeToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves CLR types and nullable counterparts.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedAndNullableTypesForSByte()
        {
            // Setup
            Type type = typeof(sbyte);
            string expected = "Int8";
            var resolver = new ClientTypeToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves CLR types and nullable counterparts.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedAndNullableTypesForNullableSByte()
        {
            // Setup
            Type type = typeof(sbyte?);
            string expected = "Int8";
            var resolver = new ClientTypeToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves CLR types and nullable counterparts.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedAndNullableTypesForByte()
        {
            // Setup
            Type type = typeof(byte);
            string expected = "UInt8";
            var resolver = new ClientTypeToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves CLR types and nullable counterparts.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedAndNullableTypesForNullableByte()
        {
            // Setup
            Type type = typeof(byte?);
            string expected = "UInt8";
            var resolver = new ClientTypeToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves CLR types and nullable counterparts.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedAndNullableTypesForUInt32()
        {
            // Setup
            Type type = typeof(uint);
            string expected = "UInt32";
            var resolver = new ClientTypeToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves CLR types and nullable counterparts.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedAndNullableTypesForNullableUInt32()
        {
            // Setup
            Type type = typeof(uint?);
            string expected = "UInt32";
            var resolver = new ClientTypeToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves CLR types and nullable counterparts.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedAndNullableTypesForUInt64()
        {
            // Setup
            Type type = typeof(ulong);
            string expected = "UInt64";
            var resolver = new ClientTypeToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves CLR types and nullable counterparts.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedAndNullableTypesForNullableUInt64()
        {
            // Setup
            Type type = typeof(ulong?);
            string expected = "UInt64";
            var resolver = new ClientTypeToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves CLR types and nullable counterparts.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedAndNullableTypesForUInt16()
        {
            // Setup
            Type type = typeof(ushort);
            string expected = "UInt16";
            var resolver = new ClientTypeToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves CLR types and nullable counterparts.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedAndNullableTypesForNullableUInt16()
        {
            // Setup
            Type type = typeof(ushort?);
            string expected = "UInt16";
            var resolver = new ClientTypeToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves CLR types and nullable counterparts.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedAndNullableTypesForBoolean()
        {
            // Setup
            Type type = typeof(bool);
            string expected = "Bool";
            var resolver = new ClientTypeToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves CLR types and nullable counterparts.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedAndNullableTypesForNullableBoolean()
        {
            // Setup
            Type type = typeof(bool?);
            string expected = "Bool";
            var resolver = new ClientTypeToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves CLR types and nullable counterparts.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedAndNullableTypesForDecimal()
        {
            // Setup
            Type type = typeof(decimal);
            string expected = "Decimal";
            var resolver = new ClientTypeToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves CLR types and nullable counterparts.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedAndNullableTypesForNullableDecimal()
        {
            // Setup
            Type type = typeof(decimal?);
            string expected = "Decimal";
            var resolver = new ClientTypeToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves CLR types and nullable counterparts.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedAndNullableTypesForDouble()
        {
            // Setup
            Type type = typeof(double);
            string expected = "Float64";
            var resolver = new ClientTypeToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves CLR types and nullable counterparts.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedAndNullableTypesForNullableDouble()
        {
            // Setup
            Type type = typeof(double?);
            string expected = "Float64";
            var resolver = new ClientTypeToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves CLR types and nullable counterparts.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedAndNullableTypesForSingle()
        {
            // Setup
            Type type = typeof(float);
            string expected = "Float32";
            var resolver = new ClientTypeToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves CLR types and nullable counterparts.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedAndNullableTypesForNullableSingle()
        {
            // Setup
            Type type = typeof(float?);
            string expected = "Float32";
            var resolver = new ClientTypeToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves CLR types and nullable counterparts.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedAndNullableTypesForDateTime()
        {
            // Setup
            Type type = typeof(DateTime);
            string expected = "DateTime64";
            var resolver = new ClientTypeToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves CLR types and nullable counterparts.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedAndNullableTypesForNullableDateTime()
        {
            // Setup
            Type type = typeof(DateTime?);
            string expected = "DateTime64";
            var resolver = new ClientTypeToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves CLR types and nullable counterparts.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedAndNullableTypesForDateTimeOffset()
        {
            // Setup
            Type type = typeof(DateTimeOffset);
            string expected = "DateTime64";
            var resolver = new ClientTypeToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves CLR types and nullable counterparts.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedAndNullableTypesForNullableDateTimeOffset()
        {
            // Setup
            Type type = typeof(DateTimeOffset?);
            string expected = "DateTime64";
            var resolver = new ClientTypeToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves CLR types and nullable counterparts.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedAndNullableTypesForTimeSpan()
        {
            // Setup
            Type type = typeof(TimeSpan);
            string expected = "String";
            var resolver = new ClientTypeToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves CLR types and nullable counterparts.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedAndNullableTypesForNullableTimeSpan()
        {
            // Setup
            Type type = typeof(TimeSpan?);
            string expected = "String";
            var resolver = new ClientTypeToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves CLR types and nullable counterparts.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedAndNullableTypesForGuid()
        {
            // Setup
            Type type = typeof(Guid);
            string expected = "UUID";
            var resolver = new ClientTypeToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves CLR types and nullable counterparts.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedAndNullableTypesForNullableGuid()
        {
            // Setup
            Type type = typeof(Guid?);
            string expected = "UUID";
            var resolver = new ClientTypeToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves CLR types and nullable counterparts.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedAndNullableTypesForByteArray()
        {
            // Setup
            Type type = typeof(byte[]);
            string expected = "String";
            var resolver = new ClientTypeToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves CLR types and nullable counterparts.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedAndNullableTypesForUri()
        {
            // Setup
            Type type = typeof(Uri);
            string expected = "String";
            var resolver = new ClientTypeToClickHouseTypeNameResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Rejects a missing CLR type.
        /// </summary>
        [TestMethod]
        public void ResolveNullIdentifiesArgument()
        {
            // Setup
            var resolver = new ClientTypeToClickHouseTypeNameResolver();

            // Act
            var exception = Assert.ThrowsExactly<ArgumentNullException>(() => resolver.Resolve(null));

            // Assert
            Assert.AreEqual("type",
                exception.ParamName,
                StringComparer.Ordinal);
        }

        #endregion
    }
}
