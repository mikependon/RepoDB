using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oracle.ManagedDataAccess.Client;
using RepoDb.Resolvers;
using System.Collections.Generic;
using System;

namespace RepoDb.Oracle.UnitTests.Resolvers
{
    /// <summary>
    /// Covers supported mappings and resolver boundaries.
    /// </summary>
    [TestClass]
    public class TypeToOracleDbTypeResolverBoundaryTest
    {
        #region Public Methods

        /// <summary>
        /// Resolves CLR types to their provider representation.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedTypeReturnsProviderTypeForString()
        {
            // Setup
            Type type = typeof(string);
            OracleDbType expected = OracleDbType.NVarchar2;
            var resolver = new TypeToOracleDbTypeResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves CLR types to their provider representation.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedTypeReturnsProviderTypeForChar()
        {
            // Setup
            Type type = typeof(char);
            OracleDbType expected = OracleDbType.NChar;
            var resolver = new TypeToOracleDbTypeResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves CLR types to their provider representation.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedTypeReturnsProviderTypeForNullableChar()
        {
            // Setup
            Type type = typeof(char?);
            OracleDbType expected = OracleDbType.NChar;
            var resolver = new TypeToOracleDbTypeResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves CLR types to their provider representation.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedTypeReturnsProviderTypeForBoolean()
        {
            // Setup
            Type type = typeof(bool);
            OracleDbType expected = OracleDbType.Int32;
            var resolver = new TypeToOracleDbTypeResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves CLR types to their provider representation.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedTypeReturnsProviderTypeForNullableBoolean()
        {
            // Setup
            Type type = typeof(bool?);
            OracleDbType expected = OracleDbType.Int32;
            var resolver = new TypeToOracleDbTypeResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves CLR types to their provider representation.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedTypeReturnsProviderTypeForByte()
        {
            // Setup
            Type type = typeof(byte);
            OracleDbType expected = OracleDbType.Byte;
            var resolver = new TypeToOracleDbTypeResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves CLR types to their provider representation.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedTypeReturnsProviderTypeForNullableByte()
        {
            // Setup
            Type type = typeof(byte?);
            OracleDbType expected = OracleDbType.Byte;
            var resolver = new TypeToOracleDbTypeResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves CLR types to their provider representation.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedTypeReturnsProviderTypeForSByte()
        {
            // Setup
            Type type = typeof(sbyte);
            OracleDbType expected = OracleDbType.Int16;
            var resolver = new TypeToOracleDbTypeResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves CLR types to their provider representation.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedTypeReturnsProviderTypeForNullableSByte()
        {
            // Setup
            Type type = typeof(sbyte?);
            OracleDbType expected = OracleDbType.Int16;
            var resolver = new TypeToOracleDbTypeResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves CLR types to their provider representation.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedTypeReturnsProviderTypeForInt16()
        {
            // Setup
            Type type = typeof(short);
            OracleDbType expected = OracleDbType.Int16;
            var resolver = new TypeToOracleDbTypeResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves CLR types to their provider representation.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedTypeReturnsProviderTypeForNullableInt16()
        {
            // Setup
            Type type = typeof(short?);
            OracleDbType expected = OracleDbType.Int16;
            var resolver = new TypeToOracleDbTypeResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves CLR types to their provider representation.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedTypeReturnsProviderTypeForUInt16()
        {
            // Setup
            Type type = typeof(ushort);
            OracleDbType expected = OracleDbType.Int32;
            var resolver = new TypeToOracleDbTypeResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves CLR types to their provider representation.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedTypeReturnsProviderTypeForNullableUInt16()
        {
            // Setup
            Type type = typeof(ushort?);
            OracleDbType expected = OracleDbType.Int32;
            var resolver = new TypeToOracleDbTypeResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves CLR types to their provider representation.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedTypeReturnsProviderTypeForInt32()
        {
            // Setup
            Type type = typeof(int);
            OracleDbType expected = OracleDbType.Int32;
            var resolver = new TypeToOracleDbTypeResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves CLR types to their provider representation.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedTypeReturnsProviderTypeForNullableInt32()
        {
            // Setup
            Type type = typeof(int?);
            OracleDbType expected = OracleDbType.Int32;
            var resolver = new TypeToOracleDbTypeResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves CLR types to their provider representation.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedTypeReturnsProviderTypeForUInt32()
        {
            // Setup
            Type type = typeof(uint);
            OracleDbType expected = OracleDbType.Int64;
            var resolver = new TypeToOracleDbTypeResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves CLR types to their provider representation.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedTypeReturnsProviderTypeForNullableUInt32()
        {
            // Setup
            Type type = typeof(uint?);
            OracleDbType expected = OracleDbType.Int64;
            var resolver = new TypeToOracleDbTypeResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves CLR types to their provider representation.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedTypeReturnsProviderTypeForInt64()
        {
            // Setup
            Type type = typeof(long);
            OracleDbType expected = OracleDbType.Int64;
            var resolver = new TypeToOracleDbTypeResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves CLR types to their provider representation.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedTypeReturnsProviderTypeForNullableInt64()
        {
            // Setup
            Type type = typeof(long?);
            OracleDbType expected = OracleDbType.Int64;
            var resolver = new TypeToOracleDbTypeResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves CLR types to their provider representation.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedTypeReturnsProviderTypeForUInt64()
        {
            // Setup
            Type type = typeof(ulong);
            OracleDbType expected = OracleDbType.Decimal;
            var resolver = new TypeToOracleDbTypeResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves CLR types to their provider representation.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedTypeReturnsProviderTypeForNullableUInt64()
        {
            // Setup
            Type type = typeof(ulong?);
            OracleDbType expected = OracleDbType.Decimal;
            var resolver = new TypeToOracleDbTypeResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves CLR types to their provider representation.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedTypeReturnsProviderTypeForSingle()
        {
            // Setup
            Type type = typeof(float);
            OracleDbType expected = OracleDbType.BinaryFloat;
            var resolver = new TypeToOracleDbTypeResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves CLR types to their provider representation.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedTypeReturnsProviderTypeForNullableSingle()
        {
            // Setup
            Type type = typeof(float?);
            OracleDbType expected = OracleDbType.BinaryFloat;
            var resolver = new TypeToOracleDbTypeResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves CLR types to their provider representation.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedTypeReturnsProviderTypeForDouble()
        {
            // Setup
            Type type = typeof(double);
            OracleDbType expected = OracleDbType.BinaryDouble;
            var resolver = new TypeToOracleDbTypeResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves CLR types to their provider representation.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedTypeReturnsProviderTypeForNullableDouble()
        {
            // Setup
            Type type = typeof(double?);
            OracleDbType expected = OracleDbType.BinaryDouble;
            var resolver = new TypeToOracleDbTypeResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves CLR types to their provider representation.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedTypeReturnsProviderTypeForDecimal()
        {
            // Setup
            Type type = typeof(decimal);
            OracleDbType expected = OracleDbType.Decimal;
            var resolver = new TypeToOracleDbTypeResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves CLR types to their provider representation.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedTypeReturnsProviderTypeForNullableDecimal()
        {
            // Setup
            Type type = typeof(decimal?);
            OracleDbType expected = OracleDbType.Decimal;
            var resolver = new TypeToOracleDbTypeResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves CLR types to their provider representation.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedTypeReturnsProviderTypeForDateTime()
        {
            // Setup
            Type type = typeof(DateTime);
            OracleDbType expected = OracleDbType.TimeStamp;
            var resolver = new TypeToOracleDbTypeResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves CLR types to their provider representation.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedTypeReturnsProviderTypeForNullableDateTime()
        {
            // Setup
            Type type = typeof(DateTime?);
            OracleDbType expected = OracleDbType.TimeStamp;
            var resolver = new TypeToOracleDbTypeResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves CLR types to their provider representation.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedTypeReturnsProviderTypeForDateTimeOffset()
        {
            // Setup
            Type type = typeof(DateTimeOffset);
            OracleDbType expected = OracleDbType.TimeStampTZ;
            var resolver = new TypeToOracleDbTypeResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves CLR types to their provider representation.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedTypeReturnsProviderTypeForNullableDateTimeOffset()
        {
            // Setup
            Type type = typeof(DateTimeOffset?);
            OracleDbType expected = OracleDbType.TimeStampTZ;
            var resolver = new TypeToOracleDbTypeResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves CLR types to their provider representation.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedTypeReturnsProviderTypeForTimeSpan()
        {
            // Setup
            Type type = typeof(TimeSpan);
            OracleDbType expected = OracleDbType.IntervalDS;
            var resolver = new TypeToOracleDbTypeResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves CLR types to their provider representation.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedTypeReturnsProviderTypeForNullableTimeSpan()
        {
            // Setup
            Type type = typeof(TimeSpan?);
            OracleDbType expected = OracleDbType.IntervalDS;
            var resolver = new TypeToOracleDbTypeResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves CLR types to their provider representation.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedTypeReturnsProviderTypeForGuid()
        {
            // Setup
            Type type = typeof(Guid);
            OracleDbType expected = OracleDbType.Raw;
            var resolver = new TypeToOracleDbTypeResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves CLR types to their provider representation.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedTypeReturnsProviderTypeForNullableGuid()
        {
            // Setup
            Type type = typeof(Guid?);
            OracleDbType expected = OracleDbType.Raw;
            var resolver = new TypeToOracleDbTypeResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves CLR types to their provider representation.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedTypeReturnsProviderTypeForByteArray()
        {
            // Setup
            Type type = typeof(byte[]);
            OracleDbType expected = OracleDbType.Blob;
            var resolver = new TypeToOracleDbTypeResolver();

            // Act
            var actual = resolver.Resolve(type);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Identifies the missing CLR type.
        /// </summary>
        [TestMethod]
        public void ResolveNullReportsArgumentName()
        {
            // Setup
            var resolver = new TypeToOracleDbTypeResolver();

            // Act
            var exception = Assert.ThrowsExactly<ArgumentNullException>(() => resolver.Resolve(null));

            // Assert
            Assert.AreEqual("nullableType",
                exception.ParamName,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Uses the default representation for unknown types.
        /// </summary>
        [TestMethod]
        public void ResolveUnsupportedTypeUsesFallback()
        {
            // Setup
            var resolver = new TypeToOracleDbTypeResolver();

            // Act
            var actual = resolver.Resolve(typeof(Uri));

            // Assert
            Assert.AreEqual(OracleDbType.NVarchar2,
                actual);
        }

        #endregion
    }
}