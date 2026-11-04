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
        /// <param name="name">The reported database type.</param>
        /// <param name="expected">The expected CLR representation.</param>
        [DataRow(" Nullable( LowCardinality( String ) ) ",
            typeof(string))]
        [DataRow("lowcardinality(nullable(UInt64))",
            typeof(ulong))]
        [DataRow("Nullable(Nullable(Int32))",
            typeof(int))]
        [DataRow("FixedString(16)",
            typeof(string))]
        [DataRow("DateTime64(3, 'UTC')",
            typeof(DateTime))]
        [DataRow("Decimal(18, 4)",
            typeof(decimal))]
        [DataRow("Nullable(Decimal128(8))",
            typeof(decimal))]
        [DataRow("Enum8('first' = 1)",
            typeof(string))]
        [DataRow("Array(Int32)",
            typeof(object))]
        [DataRow("Tuple(String, Int32)",
            typeof(object))]
        [DataRow("Map(String, UInt64)",
            typeof(object))]
        [DataRow("Nullable(Nothing)",
            typeof(object))]
        [DataRow("Nullable(Int32",
            typeof(object))]
        [DataRow("LowCardinality(String",
            typeof(object))]
        [TestMethod]
        public void ResolveQualifiedNameReturnsExpectedType(string name,
            Type expected)
        {
            // Setup
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