using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using RepoDb.Resolvers;
using System.ComponentModel.DataAnnotations.Schema;

namespace RepoDb.UnitTests.Resolvers
{
    [TestClass]
    public class PropertyMappedNameResolverTest
    {
        #region SubClasses

        private class EntityModel
        {
            public int Id { get; set; }
        }

        private class EntityModelWithMapAttribute
        {
            [Map("[PrimaryId]")]
            public int Id { get; set; }
        }

        private class EntityModelWithColumnAttribute
        {
            [Column("[PrimaryId]")]
            public int Id { get; set; }
        }

        private class EntityModelWithMapAndColumnAttribute
        {
            [Map("[MapId]"), Column("[ColumnId]")]
            public int Id { get; set; }
        }

        #endregion

        /*
         * No Attributes
         */

        [TestMethod]
        public void TestPropertyMappedNameResolverWithoutAttribute()
        {
            // Setup
            var resolver = new PropertyMappedNameResolver();

            // Act
            var result = resolver.Resolve(typeof(EntityModel).GetProperty("Id"), null);
            var expected = "Id";

            // Assert
            Assert.AreEqual(expected, result);
        }

        /*
         * With Attributes
         */

        [TestMethod]
        public void TestPropertyMappedNameResolverWithMapAttribute()
        {
            // Setup
            var resolver = new PropertyMappedNameResolver();

            // Act
            var result = resolver.Resolve(typeof(EntityModelWithMapAttribute).GetProperty("Id"), null);
            var expected = "[PrimaryId]";

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestPropertyMappedNameResolverWithColumnAttribute()
        {
            // Setup
            var resolver = new PropertyMappedNameResolver();

            // Act
            var result = resolver.Resolve(typeof(EntityModelWithColumnAttribute).GetProperty("Id"), null);
            var expected = "[PrimaryId]";

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestPropertyMappedNameResolverWithMapAndColumnAttribute()
        {
            // Setup
            var resolver = new PropertyMappedNameResolver();

            // Act
            var result = resolver.Resolve(typeof(EntityModelWithMapAndColumnAttribute).GetProperty("Id"), null);
            var expected = "[MapId]";

            // Assert
            Assert.AreEqual(expected, result);
        }

        /*
         * With Mappings
         */

        [TestMethod]
        public void TestPropertyMappedNameResolverWithMapAttributeAndMappings()
        {
            // Setup
            var resolver = new PropertyMappedNameResolver();
            FluentMapper
                .Entity<EntityModelWithMapAttribute>()
                .Column(e => e.Id, "[MapId]");

            // Act
            var result = resolver.Resolve(typeof(EntityModelWithMapAttribute).GetProperty("Id"), null);
            var expected = "[PrimaryId]";

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestPropertyMappedNameResolverWithColumnAttributeAndMappings()
        {
            // Setup
            var resolver = new PropertyMappedNameResolver();
            FluentMapper
                .Entity<EntityModelWithColumnAttribute>()
                .Column(e => e.Id, "[ColumnId]");

            // Act
            var result = resolver.Resolve(typeof(EntityModelWithColumnAttribute).GetProperty("Id"), null);
            var expected = "[PrimaryId]";

            // Assert
            Assert.AreEqual(expected, result);
        }
    }
}
