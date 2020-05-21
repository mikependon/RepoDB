using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using RepoDb.Resolvers;
using System;
using System.Data;

namespace RepoDb.UnitTests.Resolvers
{
    [TestClass]
    public class TypeMapPropertyLevelResolverTest
    {
        #region SubClasses

        private class EntityModelWithPropertyHandlerAttribute
        {
            [TypeMap(DbType.Guid)]
            public string Id { get; set; }
        }

        private class EntityModelWithoutPropertyHandlerAttribute
        {
            public string Id { get; set; }
        }

        #endregion

        [TestMethod]
        public void TestTypeMapPropertyLevelResolverWithAttributes()
        {
            // Setup
            var resolver = new TypeMapPropertyLevelResolver();
            var entityType = typeof(EntityModelWithPropertyHandlerAttribute);
            var property = entityType.GetProperty("Id");
            FluentMapper
                .Entity<EntityModelWithPropertyHandlerAttribute>()
                .DbType(e => e.Id, DbType.AnsiStringFixedLength);

            // Act
            var result = resolver.Resolve(property);
            var expected = DbType.Guid;

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestTypeMapPropertyLevelResolverWithoutAttributes()
        {
            // Setup
            var resolver = new TypeMapPropertyLevelResolver();
            var entityType = typeof(EntityModelWithoutPropertyHandlerAttribute);
            var property = entityType.GetProperty("Id");
            FluentMapper
                .Entity<EntityModelWithoutPropertyHandlerAttribute>()
                .DbType(e => e.Id, DbType.AnsiStringFixedLength);

            // Act
            var result = resolver.Resolve(property);
            var expected = DbType.AnsiStringFixedLength;

            // Assert
            Assert.AreEqual(expected, result);
        }
    }
}
