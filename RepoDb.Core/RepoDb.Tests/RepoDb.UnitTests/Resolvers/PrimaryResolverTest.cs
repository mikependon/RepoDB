using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using RepoDb.Resolvers;
using System.ComponentModel.DataAnnotations;

namespace RepoDb.UnitTests.Resolvers
{
    [TestClass]
    public class PrimaryResolverTest
    {
        #region SubClasses

        private class EntityModelWithPrimaryAttribute
        {
            [Primary]
            public int PrimaryId { get; set; }
            [Key]
            public int SecondaryId { get; set; }
        }

        private class EntityModelWithKeyAttribute
        {
            [Primary]
            public int PrimaryId { get; set; }
            [Key]
            public int SecondaryId { get; set; }
        }

        private class EntityModelWithPrimaryAndKeyAttribute
        {
            [Primary]
            public int PrimaryId { get; set; }
            [Key]
            public int SecondaryId { get; set; }
        }

        private class EntityModelWithPrimaryProperty
        {
            public int Id { get; set; }
        }

        private class EntityModelWithClassAndPrimaryProperty
        {
            public int EntityModelWithClassAndPrimaryPropertyId { get; set; }
        }

        [Map("Map")]
        private class EntityModelWithMapAttributeAndPrimaryProperty
        {
            public int MapId { get; set; }
        }

        [Map("Table")]
        private class EntityModelWithTableAttributeAndPrimaryProperty
        {
            public int TableId { get; set; }
        }

        #endregion

        /*
         * With Attributes
         */

        [TestMethod]
        public void TestPrimaryResolverWithPrimaryAttribute()
        {
            // Setup
            var resolver = new PrimaryResolver();

            // Act
            var result = resolver.Resolve(typeof(EntityModelWithPrimaryAttribute))?.GetMappedName();
            var expected = "PrimaryId";

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestPrimaryResolverWithKeyAttribute()
        {
            // Setup
            var resolver = new PrimaryResolver();

            // Act
            var result = resolver.Resolve(typeof(EntityModelWithKeyAttribute))?.GetMappedName();
            var expected = "PrimaryId";

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestPrimaryResolverWithMapAndKeyAttribute()
        {
            // Setup
            var resolver = new PrimaryResolver();

            // Act
            var result = resolver.Resolve(typeof(EntityModelWithPrimaryAndKeyAttribute))?.GetMappedName();
            var expected = "PrimaryId";

            // Assert
            Assert.AreEqual(expected, result);
        }

        /*
         * With Mappings
         */

        [TestMethod]
        public void TestPrimaryResolverWithPrimaryAttributeAndMappings()
        {
            // Setup
            var resolver = new PrimaryResolver();
            FluentMapper
                .Entity<EntityModelWithPrimaryAttribute>()
                .Primary(e => e.SecondaryId);

            // Act
            var result = resolver.Resolve(typeof(EntityModelWithPrimaryAttribute))?.GetMappedName();
            var expected = "PrimaryId";

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestPrimaryResolverWithKeyAttributeAndMappings()
        {
            // Setup
            var resolver = new PrimaryResolver();
            FluentMapper
                .Entity<EntityModelWithKeyAttribute>()
                .Primary(e => e.SecondaryId);

            // Act
            var result = resolver.Resolve(typeof(EntityModelWithKeyAttribute))?.GetMappedName();
            var expected = "PrimaryId";

            // Assert
            Assert.AreEqual(expected, result);
        }

        /*
         * With Primary Property
         */

        [TestMethod]
        public void TestPrimaryResolverWithPrimaryProperty()
        {
            // Setup
            var resolver = new PrimaryResolver();

            // Act
            var result = resolver.Resolve(typeof(EntityModelWithPrimaryProperty))?.GetMappedName();
            var expected = "Id";

            // Assert
            Assert.AreEqual(expected, result);
        }

        /*
         * With Class + Primary Property
         */

        [TestMethod]
        public void TestPrimaryResolverWithClassAndPrimaryProperty()
        {
            // Setup
            var resolver = new PrimaryResolver();

            // Act
            var result = resolver.Resolve(typeof(EntityModelWithClassAndPrimaryProperty))?.GetMappedName();
            var expected = "EntityModelWithClassAndPrimaryPropertyId";

            // Assert
            Assert.AreEqual(expected, result);
        }

        /*
         * With MapAttribute + Primary Property
         */

        [TestMethod]
        public void TestPrimaryResolverWithMapAttributeAndWithPrimaryProperty()
        {
            // Setup
            var resolver = new PrimaryResolver();

            // Act
            var result = resolver.Resolve(typeof(EntityModelWithMapAttributeAndPrimaryProperty))?.GetMappedName();
            var expected = "MapId";

            // Assert
            Assert.AreEqual(expected, result);
        }

        /*
         * With TableAttribute + Primary Property
         */

        [TestMethod]
        public void TestPrimaryResolverWithTableAttributeAndWithPrimaryProperty()
        {
            // Setup
            var resolver = new PrimaryResolver();

            // Act
            var result = resolver.Resolve(typeof(EntityModelWithTableAttributeAndPrimaryProperty))?.GetMappedName();
            var expected = "TableId";

            // Assert
            Assert.AreEqual(expected, result);
        }
    }
}
