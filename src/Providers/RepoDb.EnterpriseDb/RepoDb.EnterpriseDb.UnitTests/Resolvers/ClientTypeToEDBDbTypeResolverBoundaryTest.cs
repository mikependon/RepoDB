using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Connector.EnterpriseDb;
using RepoDb.Resolvers;
using System;

namespace RepoDb.EnterpriseDb.UnitTests.Resolvers
{
    /// <summary>
    /// Covers supported mappings and resolver boundaries.
    /// </summary>
    [TestClass]
    public class ClientTypeToEDBDbTypeResolverBoundaryTest
    {
        #region Public Methods

        /// <summary>
        /// Resolves CLR types to their provider representation.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedTypeReturnsProviderTypeForBoolean()
        {
            // Setup
            Type type = typeof(Boolean);
            EDBType expected = EDBType.Boolean;
            var resolver = new ClientTypeToEDBDbTypeResolver();

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
            EDBType expected = EDBType.Bytea;
            var resolver = new ClientTypeToEDBDbTypeResolver();

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
            EDBType expected = EDBType.Char;
            var resolver = new ClientTypeToEDBDbTypeResolver();

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
            EDBType expected = EDBType.Bit;
            var resolver = new ClientTypeToEDBDbTypeResolver();

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
            EDBType expected = EDBType.Timestamp;
            var resolver = new ClientTypeToEDBDbTypeResolver();

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
            EDBType expected = EDBType.TimestampTz;
            var resolver = new ClientTypeToEDBDbTypeResolver();

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
            EDBType expected = EDBType.Date;
            var resolver = new ClientTypeToEDBDbTypeResolver();

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
            EDBType expected = EDBType.Time;
            var resolver = new ClientTypeToEDBDbTypeResolver();

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
            EDBType expected = EDBType.Money;
            var resolver = new ClientTypeToEDBDbTypeResolver();

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
            EDBType expected = EDBType.Double;
            var resolver = new ClientTypeToEDBDbTypeResolver();

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
            EDBType expected = EDBType.Uuid;
            var resolver = new ClientTypeToEDBDbTypeResolver();

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
            EDBType expected = EDBType.SmallInt;
            var resolver = new ClientTypeToEDBDbTypeResolver();

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
            EDBType expected = EDBType.Integer;
            var resolver = new ClientTypeToEDBDbTypeResolver();

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
            EDBType expected = EDBType.BigInt;
            var resolver = new ClientTypeToEDBDbTypeResolver();

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
            EDBType expected = EDBType.Inet;
            var resolver = new ClientTypeToEDBDbTypeResolver();

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
            EDBType expected = EDBType.MacAddr;
            var resolver = new ClientTypeToEDBDbTypeResolver();

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
            EDBType expected = EDBType.Real;
            var resolver = new ClientTypeToEDBDbTypeResolver();

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
            EDBType expected = EDBType.Char;
            var resolver = new ClientTypeToEDBDbTypeResolver();

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
            EDBType expected = EDBType.Interval;
            var resolver = new ClientTypeToEDBDbTypeResolver();

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
            EDBType expected = EDBType.Cidr;
            var resolver = new ClientTypeToEDBDbTypeResolver();

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
            var resolver = new ClientTypeToEDBDbTypeResolver();

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
            var resolver = new ClientTypeToEDBDbTypeResolver();

            // Act
            Action action = () => resolver.Resolve(typeof(Uri));

            // Assert
            Assert.ThrowsExactly<InvalidOperationException>(action);
        }

        #endregion
    }
}