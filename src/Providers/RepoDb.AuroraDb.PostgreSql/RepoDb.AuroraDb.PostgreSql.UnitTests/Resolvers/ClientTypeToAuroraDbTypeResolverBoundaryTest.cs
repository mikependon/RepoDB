using Microsoft.VisualStudio.TestTools.UnitTesting;
using NpgsqlTypes;
using RepoDb.Connector.AuroraDb.Npgsql;
using RepoDb.Resolvers;
using System.Collections.Generic;
using System;

namespace RepoDb.AuroraDb.PostgreSql.UnitTests.Resolvers
{
    /// <summary>
    /// Covers supported mappings and resolver boundaries.
    /// </summary>
    [TestClass]
    public class ClientTypeToAuroraDbTypeResolverBoundaryTest
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
            AuroraDbType expected = AuroraDbType.Box;
            var resolver = new ClientTypeToAuroraDbTypeResolver();

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
            AuroraDbType expected = AuroraDbType.Circle;
            var resolver = new ClientTypeToAuroraDbTypeResolver();

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
            AuroraDbType expected = AuroraDbType.Line;
            var resolver = new ClientTypeToAuroraDbTypeResolver();

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
            AuroraDbType expected = AuroraDbType.LSeg;
            var resolver = new ClientTypeToAuroraDbTypeResolver();

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
            AuroraDbType expected = AuroraDbType.Path;
            var resolver = new ClientTypeToAuroraDbTypeResolver();

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
            AuroraDbType expected = AuroraDbType.Point;
            var resolver = new ClientTypeToAuroraDbTypeResolver();

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
            AuroraDbType expected = AuroraDbType.Polygon;
            var resolver = new ClientTypeToAuroraDbTypeResolver();

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
        public void ResolveSupportedTypeReturnsProviderTypeForNpgsqlCidr()
        {
            // Setup
            Type type = typeof(NpgsqlCidr);
            AuroraDbType expected = AuroraDbType.Cidr;
            var resolver = new ClientTypeToAuroraDbTypeResolver();

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
            AuroraDbType expected = AuroraDbType.IntegerRange;
            var resolver = new ClientTypeToAuroraDbTypeResolver();

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
            AuroraDbType expected = AuroraDbType.BigIntRange;
            var resolver = new ClientTypeToAuroraDbTypeResolver();

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
            AuroraDbType expected = AuroraDbType.NumericRange;
            var resolver = new ClientTypeToAuroraDbTypeResolver();

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
        public void ResolveSupportedTypeReturnsProviderTypeForNpgsqlRangeOfDateOnly()
        {
            // Setup
            Type type = typeof(NpgsqlRange<DateOnly>);
            AuroraDbType expected = AuroraDbType.DateRange;
            var resolver = new ClientTypeToAuroraDbTypeResolver();

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
            AuroraDbType expected = AuroraDbType.TsQuery;
            var resolver = new ClientTypeToAuroraDbTypeResolver();

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
            AuroraDbType expected = AuroraDbType.TsVector;
            var resolver = new ClientTypeToAuroraDbTypeResolver();

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
            AuroraDbType expected = AuroraDbType.MacAddr;
            var resolver = new ClientTypeToAuroraDbTypeResolver();

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
        public void ResolveSupportedTypeReturnsProviderTypeForDictionaryOfStringAndString()
        {
            // Setup
            Type type = typeof(Dictionary<String, String>);
            AuroraDbType expected = AuroraDbType.Hstore;
            var resolver = new ClientTypeToAuroraDbTypeResolver();

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
            AuroraDbType expected = AuroraDbType.Boolean;
            var resolver = new ClientTypeToAuroraDbTypeResolver();

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
            AuroraDbType expected = AuroraDbType.Bytea;
            var resolver = new ClientTypeToAuroraDbTypeResolver();

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
            AuroraDbType expected = AuroraDbType.Char;
            var resolver = new ClientTypeToAuroraDbTypeResolver();

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
            AuroraDbType expected = AuroraDbType.Bit;
            var resolver = new ClientTypeToAuroraDbTypeResolver();

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
            AuroraDbType expected = AuroraDbType.Timestamp;
            var resolver = new ClientTypeToAuroraDbTypeResolver();

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
            AuroraDbType expected = AuroraDbType.TimestampTz;
            var resolver = new ClientTypeToAuroraDbTypeResolver();

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
            AuroraDbType expected = AuroraDbType.Date;
            var resolver = new ClientTypeToAuroraDbTypeResolver();

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
            AuroraDbType expected = AuroraDbType.Time;
            var resolver = new ClientTypeToAuroraDbTypeResolver();

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
            AuroraDbType expected = AuroraDbType.Decimal;
            var resolver = new ClientTypeToAuroraDbTypeResolver();

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
            AuroraDbType expected = AuroraDbType.Double;
            var resolver = new ClientTypeToAuroraDbTypeResolver();

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
            AuroraDbType expected = AuroraDbType.Uuid;
            var resolver = new ClientTypeToAuroraDbTypeResolver();

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
            AuroraDbType expected = AuroraDbType.SmallInt;
            var resolver = new ClientTypeToAuroraDbTypeResolver();

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
            AuroraDbType expected = AuroraDbType.Integer;
            var resolver = new ClientTypeToAuroraDbTypeResolver();

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
            AuroraDbType expected = AuroraDbType.BigInt;
            var resolver = new ClientTypeToAuroraDbTypeResolver();

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
            AuroraDbType expected = AuroraDbType.Inet;
            var resolver = new ClientTypeToAuroraDbTypeResolver();

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
            AuroraDbType expected = AuroraDbType.Real;
            var resolver = new ClientTypeToAuroraDbTypeResolver();

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
            AuroraDbType expected = AuroraDbType.Text;
            var resolver = new ClientTypeToAuroraDbTypeResolver();

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
            AuroraDbType expected = AuroraDbType.Interval;
            var resolver = new ClientTypeToAuroraDbTypeResolver();

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
            var resolver = new ClientTypeToAuroraDbTypeResolver();

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
            var resolver = new ClientTypeToAuroraDbTypeResolver();

            // Act
            Action action = () => resolver.Resolve(typeof(Uri));

            // Assert
            Assert.ThrowsExactly<InvalidOperationException>(action);
        }

        #endregion
    }
}