using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using System;
using System.Data;

namespace RepoDb.UnitTests.Resolvers
{
    [TestClass]
    public class TypeMapTypeLevelResolverTest
    {
        [TestMethod]
        public void TestTypeMapTypeLevelResolverWithAttributes()
        {
            // Setup
            var resolver = new TypeMapTypeLevelResolver();
            FluentMapper
                .Type<Guid>()
                .DbType(DbType.AnsiStringFixedLength);

            // Act
            var result = resolver.Resolve(typeof(Guid));
            var expected = DbType.AnsiStringFixedLength;

            // Assert
            Assert.AreEqual(expected, result);
        }
    }
}
