using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using RepoDb.Interfaces;
using RepoDb.Resolvers;

namespace RepoDb.UnitTests.Resolvers
{
    [TestClass]
    public class PropertyHandlerPropertyLevelResolverTest
    {
        #region PropertyHandlers

        private class IdentityPropertyHandler : IPropertyHandler<int, int>
        {
            public int Get(int input, ClassProperty property)
            {
                return input;
            }

            public int Set(int input, ClassProperty property)
            {
                return input;
            }
        }

        private class IntPropertyHandler : IPropertyHandler<int, int>
        {
            public int Get(int input, ClassProperty property)
            {
                return input;
            }

            public int Set(int input, ClassProperty property)
            {
                return input;
            }
        }

        #endregion

        #region SubClasses

        private class EntityModelWithPropertyHandlerAttribute
        {
            [PropertyHandler(typeof(IdentityPropertyHandler))]
            public int Id { get; set; }
        }

        private class EntityModelWithoutPropertyHandlerAttribute
        {
            public int Id { get; set; }
        }

        #endregion

        [TestMethod]
        public void TestPropertyHandlerPropertyLevelResolverWithAttributes()
        {
            // Setup
            var resolver = new PropertyHandlerPropertyLevelResolver();
            var entityType = typeof(EntityModelWithPropertyHandlerAttribute);
            var property = entityType.GetProperty("Id");
            FluentMapper
                .Entity<EntityModelWithPropertyHandlerAttribute>()
                .PropertyHandler<IntPropertyHandler>(e => e.Id);

            // Act
            var result = resolver.Resolve(entityType, property)?.GetType();
            var expected = typeof(IdentityPropertyHandler);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestPropertyHandlerPropertyLevelResolverWithoutAttributes()
        {
            // Setup
            var resolver = new PropertyHandlerPropertyLevelResolver();
            var entityType = typeof(EntityModelWithoutPropertyHandlerAttribute);
            var property = entityType.GetProperty("Id");
            FluentMapper
                .Entity<EntityModelWithoutPropertyHandlerAttribute>()
                .PropertyHandler<IdentityPropertyHandler>(e => e.Id);

            // Act
            var result = resolver.Resolve(entityType, property)?.GetType();
            var expected = typeof(IdentityPropertyHandler);

            // Assert
            Assert.AreEqual(expected, result);
        }
    }
}
