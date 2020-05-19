using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using RepoDb.Resolvers;
using System.ComponentModel.DataAnnotations.Schema;

namespace RepoDb.UnitTests.Resolvers
{
    [TestClass]
    public class ClassMappedNameResolverTest
    {
        #region SubClasses

        private class EntityModel { }

        [Map("[dbo].[Map]")]
        private class EntityModelWithMapAttribute { }

        [Table("[dbo].[Table]")]
        private class EntityModelWithTableAttribute { }

        [Map("[dbo].[Map]"), Table("[dbo].[Table]")]
        private class EntityModelWithMapAndTableAttribute { }

        #endregion

        /*
         * No Attributes
         */

        [TestMethod]
        public void TestClassMappedNameResolverWithoutAttribute()
        {
            // Setup
            var resolver = new ClassMappedNameResolver();

            // Act
            var result = resolver.Resolve(typeof(EntityModel));
            var expected = "EntityModel";

            // Assert
            Assert.AreEqual(expected, result);
        }

        /*
         * With Attributes
         */

        [TestMethod]
        public void TestClassMappedNameResolverWithMapAttribute()
        {
            // Setup
            var resolver = new ClassMappedNameResolver();

            // Act
            var result = resolver.Resolve(typeof(EntityModelWithMapAttribute));
            var expected = "[dbo].[Map]";

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestClassMappedNameResolverWithTableAttribute()
        {
            // Setup
            var resolver = new ClassMappedNameResolver();

            // Act
            var result = resolver.Resolve(typeof(EntityModelWithTableAttribute));
            var expected = "[dbo].[Table]";

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestClassMappedNameResolverWithMapAndTableAttribute()
        {
            // Setup
            var resolver = new ClassMappedNameResolver();

            // Act
            var result = resolver.Resolve(typeof(EntityModelWithMapAndTableAttribute));
            var expected = "[dbo].[Map]";

            // Assert
            Assert.AreEqual(expected, result);
        }

        /*
         * With Mappings
         */

        [TestMethod]
        public void TestClassMappedNameResolverWithMapAttributeAndMappings()
        {
            // Setup
            var resolver = new ClassMappedNameResolver();
            FluentMapper
                .Entity<EntityModelWithMapAttribute>()
                .Table("[dbo].[Mapping]");

            // Act
            var result = resolver.Resolve(typeof(EntityModelWithMapAttribute));
            var expected = "[dbo].[Map]";

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestClassMappedNameResolverWithTableAttributeAndMappings()
        {
            // Setup
            var resolver = new ClassMappedNameResolver();
            FluentMapper
                .Entity<EntityModelWithTableAttribute>()
                .Table("[dbo].[Mapping]");

            // Act
            var result = resolver.Resolve(typeof(EntityModelWithTableAttribute));
            var expected = "[dbo].[Table]";

            // Assert
            Assert.AreEqual(expected, result);
        }
    }
}
