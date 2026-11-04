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
        /// <param name="name">The reported database type.</param>
        /// <param name="expected">The expected CLR representation.</param>
        [DataRow(" TIMESTAMP(6) ",
            typeof(DateTime))]
        [DataRow("TIMESTAMP(12) WITH TIME ZONE",
            typeof(DateTimeOffset))]
        [DataRow("timestamp with time zone",
            typeof(DateTimeOffset))]
        [DataRow("  integer  ",
            typeof(int))]
        [TestMethod]
        public void ResolveQualifiedNameReturnsExpectedType(string name,
            Type expected)
        {
            // Setup
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