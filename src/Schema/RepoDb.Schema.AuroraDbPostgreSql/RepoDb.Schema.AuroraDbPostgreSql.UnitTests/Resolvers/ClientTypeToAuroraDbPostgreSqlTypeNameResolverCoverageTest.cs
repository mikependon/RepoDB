using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using System;

namespace RepoDb.Schema.AuroraDbPostgreSql.UnitTests.Resolvers
{
    /// <summary>
    /// Covers supported values and boundary cases.
    /// </summary>
    [TestClass]
    public class ClientTypeToAuroraDbPostgreSqlTypeNameResolverCoverageTest
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
            string expected = "varchar";
            var resolver = new ClientTypeToAuroraDbPostgreSqlTypeNameResolver();

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
            string expected = "integer";
            var resolver = new ClientTypeToAuroraDbPostgreSqlTypeNameResolver();

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
            string expected = "integer";
            var resolver = new ClientTypeToAuroraDbPostgreSqlTypeNameResolver();

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
            string expected = "bigint";
            var resolver = new ClientTypeToAuroraDbPostgreSqlTypeNameResolver();

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
            string expected = "bigint";
            var resolver = new ClientTypeToAuroraDbPostgreSqlTypeNameResolver();

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
            string expected = "smallint";
            var resolver = new ClientTypeToAuroraDbPostgreSqlTypeNameResolver();

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
            string expected = "smallint";
            var resolver = new ClientTypeToAuroraDbPostgreSqlTypeNameResolver();

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
            string expected = "smallint";
            var resolver = new ClientTypeToAuroraDbPostgreSqlTypeNameResolver();

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
            string expected = "smallint";
            var resolver = new ClientTypeToAuroraDbPostgreSqlTypeNameResolver();

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
            string expected = "boolean";
            var resolver = new ClientTypeToAuroraDbPostgreSqlTypeNameResolver();

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
            string expected = "boolean";
            var resolver = new ClientTypeToAuroraDbPostgreSqlTypeNameResolver();

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
            string expected = "numeric";
            var resolver = new ClientTypeToAuroraDbPostgreSqlTypeNameResolver();

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
            string expected = "numeric";
            var resolver = new ClientTypeToAuroraDbPostgreSqlTypeNameResolver();

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
            string expected = "double precision";
            var resolver = new ClientTypeToAuroraDbPostgreSqlTypeNameResolver();

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
            string expected = "double precision";
            var resolver = new ClientTypeToAuroraDbPostgreSqlTypeNameResolver();

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
            string expected = "real";
            var resolver = new ClientTypeToAuroraDbPostgreSqlTypeNameResolver();

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
            string expected = "real";
            var resolver = new ClientTypeToAuroraDbPostgreSqlTypeNameResolver();

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
            string expected = "timestamp";
            var resolver = new ClientTypeToAuroraDbPostgreSqlTypeNameResolver();

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
            string expected = "timestamp";
            var resolver = new ClientTypeToAuroraDbPostgreSqlTypeNameResolver();

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
            string expected = "timestamptz";
            var resolver = new ClientTypeToAuroraDbPostgreSqlTypeNameResolver();

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
            string expected = "timestamptz";
            var resolver = new ClientTypeToAuroraDbPostgreSqlTypeNameResolver();

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
            string expected = "time";
            var resolver = new ClientTypeToAuroraDbPostgreSqlTypeNameResolver();

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
            string expected = "time";
            var resolver = new ClientTypeToAuroraDbPostgreSqlTypeNameResolver();

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
            string expected = "uuid";
            var resolver = new ClientTypeToAuroraDbPostgreSqlTypeNameResolver();

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
            string expected = "uuid";
            var resolver = new ClientTypeToAuroraDbPostgreSqlTypeNameResolver();

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
            string expected = "bytea";
            var resolver = new ClientTypeToAuroraDbPostgreSqlTypeNameResolver();

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
            string expected = "text";
            var resolver = new ClientTypeToAuroraDbPostgreSqlTypeNameResolver();

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
            var resolver = new ClientTypeToAuroraDbPostgreSqlTypeNameResolver();

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
