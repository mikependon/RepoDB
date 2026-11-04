using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Connector.CockroachDb;
using RepoDb.Resolvers;
using System;

namespace RepoDb.CockroachDb.UnitTests.Resolvers
{
    /// <summary>
    /// Covers supported mappings and resolver boundaries.
    /// </summary>
    [TestClass]
    public class ClientTypeToCockroachDbTypeResolverBoundaryTest
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
            CockroachDbType expected = CockroachDbType.Boolean;
            var resolver = new ClientTypeToCockroachDbTypeResolver();

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
            CockroachDbType expected = CockroachDbType.Bytea;
            var resolver = new ClientTypeToCockroachDbTypeResolver();

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
            CockroachDbType expected = CockroachDbType.Char;
            var resolver = new ClientTypeToCockroachDbTypeResolver();

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
            CockroachDbType expected = CockroachDbType.Bit;
            var resolver = new ClientTypeToCockroachDbTypeResolver();

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
            CockroachDbType expected = CockroachDbType.Timestamp;
            var resolver = new ClientTypeToCockroachDbTypeResolver();

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
            CockroachDbType expected = CockroachDbType.TimestampTz;
            var resolver = new ClientTypeToCockroachDbTypeResolver();

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
            CockroachDbType expected = CockroachDbType.Date;
            var resolver = new ClientTypeToCockroachDbTypeResolver();

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
            CockroachDbType expected = CockroachDbType.Time;
            var resolver = new ClientTypeToCockroachDbTypeResolver();

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
            CockroachDbType expected = CockroachDbType.Decimal;
            var resolver = new ClientTypeToCockroachDbTypeResolver();

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
            CockroachDbType expected = CockroachDbType.Double;
            var resolver = new ClientTypeToCockroachDbTypeResolver();

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
            CockroachDbType expected = CockroachDbType.Uuid;
            var resolver = new ClientTypeToCockroachDbTypeResolver();

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
            CockroachDbType expected = CockroachDbType.SmallInt;
            var resolver = new ClientTypeToCockroachDbTypeResolver();

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
            CockroachDbType expected = CockroachDbType.Integer;
            var resolver = new ClientTypeToCockroachDbTypeResolver();

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
            CockroachDbType expected = CockroachDbType.BigInt;
            var resolver = new ClientTypeToCockroachDbTypeResolver();

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
            CockroachDbType expected = CockroachDbType.Inet;
            var resolver = new ClientTypeToCockroachDbTypeResolver();

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
            CockroachDbType expected = CockroachDbType.Real;
            var resolver = new ClientTypeToCockroachDbTypeResolver();

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
            CockroachDbType expected = CockroachDbType.Text;
            var resolver = new ClientTypeToCockroachDbTypeResolver();

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
            CockroachDbType expected = CockroachDbType.Interval;
            var resolver = new ClientTypeToCockroachDbTypeResolver();

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
            var resolver = new ClientTypeToCockroachDbTypeResolver();

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
            var resolver = new ClientTypeToCockroachDbTypeResolver();

            // Act
            Action action = () => resolver.Resolve(typeof(Uri));

            // Assert
            Assert.ThrowsExactly<InvalidOperationException>(action);
        }

        #endregion
    }
}