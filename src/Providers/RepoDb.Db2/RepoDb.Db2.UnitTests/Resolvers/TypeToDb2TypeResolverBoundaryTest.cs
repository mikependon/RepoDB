using Microsoft.VisualStudio.TestTools.UnitTesting;
using IBM.Data.Db2;
using RepoDb.Resolvers;
using System.Collections.Generic;
using System;

namespace RepoDb.Db2.UnitTests.Resolvers
{
    /// <summary>
    /// Covers supported mappings and resolver boundaries.
    /// </summary>
    [TestClass]
    public class TypeToDb2TypeResolverBoundaryTest
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
            DB2Type expected = DB2Type.VarChar;
            var resolver = new TypeToDb2TypeResolver();

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
            DB2Type expected = DB2Type.Char;
            var resolver = new TypeToDb2TypeResolver();

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
            DB2Type expected = DB2Type.Char;
            var resolver = new TypeToDb2TypeResolver();

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
            DB2Type expected = DB2Type.SmallInt;
            var resolver = new TypeToDb2TypeResolver();

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
            DB2Type expected = DB2Type.SmallInt;
            var resolver = new TypeToDb2TypeResolver();

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
            DB2Type expected = DB2Type.Byte;
            var resolver = new TypeToDb2TypeResolver();

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
            DB2Type expected = DB2Type.Byte;
            var resolver = new TypeToDb2TypeResolver();

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
            DB2Type expected = DB2Type.SmallInt;
            var resolver = new TypeToDb2TypeResolver();

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
            DB2Type expected = DB2Type.SmallInt;
            var resolver = new TypeToDb2TypeResolver();

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
            DB2Type expected = DB2Type.SmallInt;
            var resolver = new TypeToDb2TypeResolver();

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
            DB2Type expected = DB2Type.SmallInt;
            var resolver = new TypeToDb2TypeResolver();

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
            DB2Type expected = DB2Type.Integer;
            var resolver = new TypeToDb2TypeResolver();

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
            DB2Type expected = DB2Type.Integer;
            var resolver = new TypeToDb2TypeResolver();

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
            DB2Type expected = DB2Type.Integer;
            var resolver = new TypeToDb2TypeResolver();

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
            DB2Type expected = DB2Type.Integer;
            var resolver = new TypeToDb2TypeResolver();

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
            DB2Type expected = DB2Type.BigInt;
            var resolver = new TypeToDb2TypeResolver();

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
            DB2Type expected = DB2Type.BigInt;
            var resolver = new TypeToDb2TypeResolver();

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
            DB2Type expected = DB2Type.BigInt;
            var resolver = new TypeToDb2TypeResolver();

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
            DB2Type expected = DB2Type.BigInt;
            var resolver = new TypeToDb2TypeResolver();

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
            DB2Type expected = DB2Type.Decimal;
            var resolver = new TypeToDb2TypeResolver();

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
            DB2Type expected = DB2Type.Decimal;
            var resolver = new TypeToDb2TypeResolver();

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
            DB2Type expected = DB2Type.Real;
            var resolver = new TypeToDb2TypeResolver();

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
            DB2Type expected = DB2Type.Real;
            var resolver = new TypeToDb2TypeResolver();

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
            DB2Type expected = DB2Type.Double;
            var resolver = new TypeToDb2TypeResolver();

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
            DB2Type expected = DB2Type.Double;
            var resolver = new TypeToDb2TypeResolver();

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
            DB2Type expected = DB2Type.Decimal;
            var resolver = new TypeToDb2TypeResolver();

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
            DB2Type expected = DB2Type.Decimal;
            var resolver = new TypeToDb2TypeResolver();

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
            DB2Type expected = DB2Type.Timestamp;
            var resolver = new TypeToDb2TypeResolver();

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
            DB2Type expected = DB2Type.Timestamp;
            var resolver = new TypeToDb2TypeResolver();

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
            DB2Type expected = DB2Type.TimeStampWithTimeZone;
            var resolver = new TypeToDb2TypeResolver();

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
            DB2Type expected = DB2Type.TimeStampWithTimeZone;
            var resolver = new TypeToDb2TypeResolver();

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
            DB2Type expected = DB2Type.Time;
            var resolver = new TypeToDb2TypeResolver();

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
            DB2Type expected = DB2Type.Time;
            var resolver = new TypeToDb2TypeResolver();

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
            DB2Type expected = DB2Type.Binary;
            var resolver = new TypeToDb2TypeResolver();

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
            DB2Type expected = DB2Type.Binary;
            var resolver = new TypeToDb2TypeResolver();

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
            DB2Type expected = DB2Type.Blob;
            var resolver = new TypeToDb2TypeResolver();

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
            var resolver = new TypeToDb2TypeResolver();

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
            var resolver = new TypeToDb2TypeResolver();

            // Act
            var actual = resolver.Resolve(typeof(Uri));

            // Assert
            Assert.AreEqual(DB2Type.VarChar,
                actual);
        }

        #endregion
    }
}