using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using System;

namespace RepoDb.ClickHouse.UnitTests.Resolvers
{
    /// <summary>
    /// Covers supported mappings and resolver boundaries.
    /// </summary>
    [TestClass]
    public class ClickHouseDbTypeNameToClientTypeResolverQualifiedNameTest
    {
        #region Public Methods

        /// <summary>
        /// Handles qualified names and malformed wrappers.
        /// </summary>
        [TestMethod]
        public void ResolveQualifiedNameReturnsExpectedTypeForNullableLowcardinalityStringInMixedCaseWithSurroundingWhitespace()
        {
            // Setup
            string name = " Nullable( LowCardinality( String ) ) ";
            Type expected = typeof(string);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Handles qualified names and malformed wrappers.
        /// </summary>
        [TestMethod]
        public void ResolveQualifiedNameReturnsExpectedTypeForLowcardinalityNullableUint64InMixedCase()
        {
            // Setup
            string name = "lowcardinality(nullable(UInt64))";
            Type expected = typeof(ulong);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Handles qualified names and malformed wrappers.
        /// </summary>
        [TestMethod]
        public void ResolveQualifiedNameReturnsExpectedTypeForNullableNullableInt32InMixedCase()
        {
            // Setup
            string name = "Nullable(Nullable(Int32))";
            Type expected = typeof(int);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Handles qualified names and malformed wrappers.
        /// </summary>
        [TestMethod]
        public void ResolveQualifiedNameReturnsExpectedTypeForFixedstring16InMixedCase()
        {
            // Setup
            string name = "FixedString(16)";
            Type expected = typeof(string);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Handles qualified names and malformed wrappers.
        /// </summary>
        [TestMethod]
        public void ResolveQualifiedNameReturnsExpectedTypeForDatetime643UtcInMixedCase()
        {
            // Setup
            string name = "DateTime64(3, 'UTC')";
            Type expected = typeof(DateTime);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Handles qualified names and malformed wrappers.
        /// </summary>
        [TestMethod]
        public void ResolveQualifiedNameReturnsExpectedTypeForDecimal184InMixedCase()
        {
            // Setup
            string name = "Decimal(18, 4)";
            Type expected = typeof(decimal);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Handles qualified names and malformed wrappers.
        /// </summary>
        [TestMethod]
        public void ResolveQualifiedNameReturnsExpectedTypeForNullableDecimal1288InMixedCase()
        {
            // Setup
            string name = "Nullable(Decimal128(8))";
            Type expected = typeof(decimal);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Handles qualified names and malformed wrappers.
        /// </summary>
        [TestMethod]
        public void ResolveQualifiedNameReturnsExpectedTypeForEnum8First1InMixedCase()
        {
            // Setup
            string name = "Enum8('first' = 1)";
            Type expected = typeof(string);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Handles qualified names and malformed wrappers.
        /// </summary>
        [TestMethod]
        public void ResolveQualifiedNameReturnsExpectedTypeForArrayInt32InMixedCase()
        {
            // Setup
            string name = "Array(Int32)";
            Type expected = typeof(object);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Handles qualified names and malformed wrappers.
        /// </summary>
        [TestMethod]
        public void ResolveQualifiedNameReturnsExpectedTypeForTupleStringInt32InMixedCase()
        {
            // Setup
            string name = "Tuple(String, Int32)";
            Type expected = typeof(object);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Handles qualified names and malformed wrappers.
        /// </summary>
        [TestMethod]
        public void ResolveQualifiedNameReturnsExpectedTypeForMapStringUint64InMixedCase()
        {
            // Setup
            string name = "Map(String, UInt64)";
            Type expected = typeof(object);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Handles qualified names and malformed wrappers.
        /// </summary>
        [TestMethod]
        public void ResolveQualifiedNameReturnsExpectedTypeForNullableNothingInMixedCase()
        {
            // Setup
            string name = "Nullable(Nothing)";
            Type expected = typeof(object);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Handles qualified names and malformed wrappers.
        /// </summary>
        [TestMethod]
        public void ResolveQualifiedNameReturnsExpectedTypeForNullableInt32InMixedCaseWithMissingClosingParenthesis()
        {
            // Setup
            string name = "Nullable(Int32";
            Type expected = typeof(object);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Handles qualified names and malformed wrappers.
        /// </summary>
        [TestMethod]
        public void ResolveQualifiedNameReturnsExpectedTypeForLowcardinalityStringInMixedCaseWithMissingClosingParenthesis()
        {
            // Setup
            string name = "LowCardinality(String";
            Type expected = typeof(object);
            var resolver = new ClickHouseDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        #endregion
    }
}