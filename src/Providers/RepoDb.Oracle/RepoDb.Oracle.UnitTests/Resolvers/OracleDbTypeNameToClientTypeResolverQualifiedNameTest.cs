using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using System;

namespace RepoDb.Oracle.UnitTests.Resolvers
{
    /// <summary>
    /// Covers supported mappings and resolver boundaries.
    /// </summary>
    [TestClass]
    public class OracleDbTypeNameToClientTypeResolverQualifiedNameTest
    {
        #region Public Methods

        /// <summary>
        /// Handles qualified names and malformed wrappers.
        /// </summary>
        [TestMethod]
        public void ResolveQualifiedNameReturnsExpectedTypeForTimestamp6InUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = " TIMESTAMP(6) ";
            Type expected = typeof(DateTime);
            var resolver = new OracleDbTypeNameToClientTypeResolver();

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
        public void ResolveQualifiedNameReturnsExpectedTypeForTimestamp9WithTimeZoneInUppercase()
        {
            // Setup
            string name = "TIMESTAMP(9) WITH TIME ZONE";
            Type expected = typeof(DateTimeOffset);
            var resolver = new OracleDbTypeNameToClientTypeResolver();

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
        public void ResolveQualifiedNameReturnsExpectedTypeForTimestamp6WithLocalTimeZoneInUppercase()
        {
            // Setup
            string name = "TIMESTAMP(6) WITH LOCAL TIME ZONE";
            Type expected = typeof(DateTimeOffset);
            var resolver = new OracleDbTypeNameToClientTypeResolver();

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
        public void ResolveQualifiedNameReturnsExpectedTypeForIntervalYear2ToMonthInUppercase()
        {
            // Setup
            string name = "INTERVAL YEAR(2) TO MONTH";
            Type expected = typeof(TimeSpan);
            var resolver = new OracleDbTypeNameToClientTypeResolver();

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
        public void ResolveQualifiedNameReturnsExpectedTypeForIntervalDay2ToSecond6InUppercase()
        {
            // Setup
            string name = "INTERVAL DAY(2) TO SECOND(6)";
            Type expected = typeof(TimeSpan);
            var resolver = new OracleDbTypeNameToClientTypeResolver();

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
        public void ResolveQualifiedNameReturnsExpectedTypeForNumberInUppercaseWithSurroundingWhitespace()
        {
            // Setup
            string name = "  NUMBER  ";
            Type expected = typeof(decimal);
            var resolver = new OracleDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        #endregion
    }
}