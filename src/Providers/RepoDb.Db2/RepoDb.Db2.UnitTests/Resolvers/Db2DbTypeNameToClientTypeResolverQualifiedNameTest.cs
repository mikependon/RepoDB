using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using System;

namespace RepoDb.Db2.UnitTests.Resolvers
{
    /// <summary>
    /// Covers supported mappings and resolver boundaries.
    /// </summary>
    [TestClass]
    public class Db2DbTypeNameToClientTypeResolverQualifiedNameTest
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
            var resolver = new Db2DbTypeNameToClientTypeResolver();

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
        public void ResolveQualifiedNameReturnsExpectedTypeForTimestamp12WithTimeZoneInUppercase()
        {
            // Setup
            string name = "TIMESTAMP(12) WITH TIME ZONE";
            Type expected = typeof(DateTimeOffset);
            var resolver = new Db2DbTypeNameToClientTypeResolver();

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
        public void ResolveQualifiedNameReturnsExpectedTypeForTimestampWithTimeZone()
        {
            // Setup
            string name = "timestamp with time zone";
            Type expected = typeof(DateTimeOffset);
            var resolver = new Db2DbTypeNameToClientTypeResolver();

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
        public void ResolveQualifiedNameReturnsExpectedTypeForIntegerWithSurroundingWhitespace()
        {
            // Setup
            string name = "  integer  ";
            Type expected = typeof(int);
            var resolver = new Db2DbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        #endregion
    }
}