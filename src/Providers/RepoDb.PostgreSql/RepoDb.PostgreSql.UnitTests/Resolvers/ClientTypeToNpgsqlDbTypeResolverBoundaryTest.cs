using Microsoft.VisualStudio.TestTools.UnitTesting;
using NpgsqlTypes;
using RepoDb.Resolvers;
using System;

namespace RepoDb.PostgreSql.UnitTests.Resolvers
{
    /// <summary>
    /// Covers supported mappings and resolver boundaries.
    /// </summary>
    [TestClass]
    public class ClientTypeToNpgsqlDbTypeResolverBoundaryTest
    {
        #region Public Methods

        /// <summary>
        /// Resolves CLR types to their provider representation.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedTypeReturnsProviderTypeForNpgsqlBox()
        {
            // Setup
            Type type = typeof(NpgsqlBox);
            NpgsqlDbType expected = NpgsqlDbType.Box;
            var resolver = new ClientTypeToNpgsqlDbTypeResolver();

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
        public void ResolveSupportedTypeReturnsProviderTypeForNpgsqlCircle()
        {
            // Setup
            Type type = typeof(NpgsqlCircle);
            NpgsqlDbType expected = NpgsqlDbType.Circle;
            var resolver = new ClientTypeToNpgsqlDbTypeResolver();

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
        public void ResolveSupportedTypeReturnsProviderTypeForNpgsqlLine()
        {
            // Setup
            Type type = typeof(NpgsqlLine);
            NpgsqlDbType expected = NpgsqlDbType.Line;
            var resolver = new ClientTypeToNpgsqlDbTypeResolver();

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
        public void ResolveSupportedTypeReturnsProviderTypeForNpgsqlLogSequenceNumber()
        {
            // Setup
            Type type = typeof(NpgsqlLogSequenceNumber);
            NpgsqlDbType expected = NpgsqlDbType.PgLsn;
            var resolver = new ClientTypeToNpgsqlDbTypeResolver();

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
        public void ResolveSupportedTypeReturnsProviderTypeForNpgsqlLSeg()
        {
            // Setup
            Type type = typeof(NpgsqlLSeg);
            NpgsqlDbType expected = NpgsqlDbType.LSeg;
            var resolver = new ClientTypeToNpgsqlDbTypeResolver();

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
        public void ResolveSupportedTypeReturnsProviderTypeForNpgsqlPath()
        {
            // Setup
            Type type = typeof(NpgsqlPath);
            NpgsqlDbType expected = NpgsqlDbType.Path;
            var resolver = new ClientTypeToNpgsqlDbTypeResolver();

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
        public void ResolveSupportedTypeReturnsProviderTypeForNpgsqlPoint()
        {
            // Setup
            Type type = typeof(NpgsqlPoint);
            NpgsqlDbType expected = NpgsqlDbType.Point;
            var resolver = new ClientTypeToNpgsqlDbTypeResolver();

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
        public void ResolveSupportedTypeReturnsProviderTypeForNpgsqlPolygon()
        {
            // Setup
            Type type = typeof(NpgsqlPolygon);
            NpgsqlDbType expected = NpgsqlDbType.Polygon;
            var resolver = new ClientTypeToNpgsqlDbTypeResolver();

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
        public void ResolveSupportedTypeReturnsProviderTypeForNpgsqlRangeOfDateTime()
        {
            // Setup
            Type type = typeof(NpgsqlRange<DateTime>);
            NpgsqlDbType expected = NpgsqlDbType.Unknown;
            var resolver = new ClientTypeToNpgsqlDbTypeResolver();

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
        public void ResolveSupportedTypeReturnsProviderTypeForNpgsqlRangeOfDecimal()
        {
            // Setup
            Type type = typeof(NpgsqlRange<Decimal>);
            NpgsqlDbType expected = NpgsqlDbType.Unknown;
            var resolver = new ClientTypeToNpgsqlDbTypeResolver();

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
        public void ResolveSupportedTypeReturnsProviderTypeForNpgsqlRangeOfInt32()
        {
            // Setup
            Type type = typeof(NpgsqlRange<Int32>);
            NpgsqlDbType expected = NpgsqlDbType.Unknown;
            var resolver = new ClientTypeToNpgsqlDbTypeResolver();

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
        public void ResolveSupportedTypeReturnsProviderTypeForNpgsqlRangeOfInt64()
        {
            // Setup
            Type type = typeof(NpgsqlRange<Int64>);
            NpgsqlDbType expected = NpgsqlDbType.Unknown;
            var resolver = new ClientTypeToNpgsqlDbTypeResolver();

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
        public void ResolveSupportedTypeReturnsProviderTypeForNpgsqlTid()
        {
            // Setup
            Type type = typeof(NpgsqlTid);
            NpgsqlDbType expected = NpgsqlDbType.Tid;
            var resolver = new ClientTypeToNpgsqlDbTypeResolver();

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
        public void ResolveSupportedTypeReturnsProviderTypeForNpgsqlTsQuery()
        {
            // Setup
            Type type = typeof(NpgsqlTsQuery);
            NpgsqlDbType expected = NpgsqlDbType.TsQuery;
            var resolver = new ClientTypeToNpgsqlDbTypeResolver();

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
        public void ResolveSupportedTypeReturnsProviderTypeForNpgsqlTsVector()
        {
            // Setup
            Type type = typeof(NpgsqlTsVector);
            NpgsqlDbType expected = NpgsqlDbType.TsVector;
            var resolver = new ClientTypeToNpgsqlDbTypeResolver();

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
        public void ResolveSupportedTypeReturnsProviderTypeForArray()
        {
            // Setup
            Type type = typeof(Array);
            NpgsqlDbType expected = NpgsqlDbType.Unknown;
            var resolver = new ClientTypeToNpgsqlDbTypeResolver();

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
            Type type = typeof(Boolean);
            NpgsqlDbType expected = NpgsqlDbType.Boolean;
            var resolver = new ClientTypeToNpgsqlDbTypeResolver();

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
            Type type = typeof(Byte[]);
            NpgsqlDbType expected = NpgsqlDbType.Bytea;
            var resolver = new ClientTypeToNpgsqlDbTypeResolver();

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
            Type type = typeof(Char);
            NpgsqlDbType expected = NpgsqlDbType.InternalChar;
            var resolver = new ClientTypeToNpgsqlDbTypeResolver();

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
        public void ResolveSupportedTypeReturnsProviderTypeForBitArray()
        {
            // Setup
            Type type = typeof(System.Collections.BitArray);
            NpgsqlDbType expected = NpgsqlDbType.Bit;
            var resolver = new ClientTypeToNpgsqlDbTypeResolver();

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
            NpgsqlDbType expected = NpgsqlDbType.Timestamp;
            var resolver = new ClientTypeToNpgsqlDbTypeResolver();

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
            NpgsqlDbType expected = NpgsqlDbType.TimestampTz;
            var resolver = new ClientTypeToNpgsqlDbTypeResolver();

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
        public void ResolveSupportedTypeReturnsProviderTypeForDateOnly()
        {
            // Setup
            Type type = typeof(DateOnly);
            NpgsqlDbType expected = NpgsqlDbType.Date;
            var resolver = new ClientTypeToNpgsqlDbTypeResolver();

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
        public void ResolveSupportedTypeReturnsProviderTypeForTimeOnly()
        {
            // Setup
            Type type = typeof(TimeOnly);
            NpgsqlDbType expected = NpgsqlDbType.Time;
            var resolver = new ClientTypeToNpgsqlDbTypeResolver();

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
            Type type = typeof(Decimal);
            NpgsqlDbType expected = NpgsqlDbType.Money;
            var resolver = new ClientTypeToNpgsqlDbTypeResolver();

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
            Type type = typeof(Double);
            NpgsqlDbType expected = NpgsqlDbType.Double;
            var resolver = new ClientTypeToNpgsqlDbTypeResolver();

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
            NpgsqlDbType expected = NpgsqlDbType.Uuid;
            var resolver = new ClientTypeToNpgsqlDbTypeResolver();

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
            Type type = typeof(Int16);
            NpgsqlDbType expected = NpgsqlDbType.Smallint;
            var resolver = new ClientTypeToNpgsqlDbTypeResolver();

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
            Type type = typeof(Int32);
            NpgsqlDbType expected = NpgsqlDbType.Integer;
            var resolver = new ClientTypeToNpgsqlDbTypeResolver();

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
            Type type = typeof(Int64);
            NpgsqlDbType expected = NpgsqlDbType.Bigint;
            var resolver = new ClientTypeToNpgsqlDbTypeResolver();

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
        public void ResolveSupportedTypeReturnsProviderTypeForIPAddress()
        {
            // Setup
            Type type = typeof(System.Net.IPAddress);
            NpgsqlDbType expected = NpgsqlDbType.Inet;
            var resolver = new ClientTypeToNpgsqlDbTypeResolver();

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
        public void ResolveSupportedTypeReturnsProviderTypeForPhysicalAddress()
        {
            // Setup
            Type type = typeof(System.Net.NetworkInformation.PhysicalAddress);
            NpgsqlDbType expected = NpgsqlDbType.MacAddr;
            var resolver = new ClientTypeToNpgsqlDbTypeResolver();

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
            Type type = typeof(Single);
            NpgsqlDbType expected = NpgsqlDbType.Real;
            var resolver = new ClientTypeToNpgsqlDbTypeResolver();

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
        public void ResolveSupportedTypeReturnsProviderTypeForString()
        {
            // Setup
            Type type = typeof(String);
            NpgsqlDbType expected = NpgsqlDbType.Char;
            var resolver = new ClientTypeToNpgsqlDbTypeResolver();

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
            NpgsqlDbType expected = NpgsqlDbType.Interval;
            var resolver = new ClientTypeToNpgsqlDbTypeResolver();

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
            Type type = typeof(UInt32);
            NpgsqlDbType expected = NpgsqlDbType.Cid;
            var resolver = new ClientTypeToNpgsqlDbTypeResolver();

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
        public void ResolveSupportedTypeReturnsProviderTypeForValueTupleOfIPAddressAndInt32()
        {
            // Setup
            Type type = typeof(ValueTuple<System.Net.IPAddress, Int32>);
            NpgsqlDbType expected = NpgsqlDbType.Cidr;
            var resolver = new ClientTypeToNpgsqlDbTypeResolver();

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
            var resolver = new ClientTypeToNpgsqlDbTypeResolver();

            // Act
            var exception = Assert.ThrowsExactly<ArgumentNullException>(() => resolver.Resolve(null));

            // Assert
            Assert.AreEqual("type",
                exception.ParamName,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Rejects types without a provider representation.
        /// </summary>
        [TestMethod]
        public void ResolveUnsupportedTypeThrows()
        {
            // Setup
            var resolver = new ClientTypeToNpgsqlDbTypeResolver();

            // Act
            Action action = () => resolver.Resolve(typeof(Uri));

            // Assert
            Assert.ThrowsExactly<InvalidOperationException>(action);
        }

        #endregion
    }
}