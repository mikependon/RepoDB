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
        /// <param name="name">The reported database type.</param>
        /// <param name="expected">The expected CLR representation.</param>
        [DataRow(" TIMESTAMP(6) ",
            typeof(DateTime))]
        [DataRow("TIMESTAMP(9) WITH TIME ZONE",
            typeof(DateTimeOffset))]
        [DataRow("TIMESTAMP(6) WITH LOCAL TIME ZONE",
            typeof(DateTimeOffset))]
        [DataRow("INTERVAL YEAR(2) TO MONTH",
            typeof(TimeSpan))]
        [DataRow("INTERVAL DAY(2) TO SECOND(6)",
            typeof(TimeSpan))]
        [DataRow("  NUMBER  ",
            typeof(decimal))]
        [TestMethod]
        public void ResolveQualifiedNameReturnsExpectedType(string name,
            Type expected)
        {
            // Setup
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