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

        private class EntityModelWithIdentityProperty
        {
            public int Id { get; set; }
        }

        private class EntityModelWithClassIdentityProperty
        {
            public int EntityModelWithClassIdentityPropertyId { get; set; }
        }

        [Map("[dbo].[Map]")]
        private class EntityModelWithMapAttributeAndIdentityProperty
        {
            public int MapId { get; set; }
        }

        [Map("[dbo].[Table]")]
        private class EntityModelWithTableAttributeAndIdentityProperty
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
         * With Identity Property
         */

        [TestMethod]
        public void TestPrimaryResolverWithIdentityProperty()
        {
            // Setup
            var resolver = new PrimaryResolver();

            // Act
            var result = resolver.Resolve(typeof(EntityModelWithIdentityProperty))?.GetMappedName();
            var expected = "Id";

            // Assert
            Assert.AreEqual(expected, result);
        }

        /*
         * With Class + Identity Property
         */

        [TestMethod]
        public void TestPrimaryResolverWithClassIdentityProperty()
        {
            // Setup
            var resolver = new PrimaryResolver();

            // Act
            var result = resolver.Resolve(typeof(EntityModelWithClassIdentityProperty))?.GetMappedName();
            var expected = "EntityModelWithClassIdentityPropertyId";

            // Assert
            Assert.AreEqual(expected, result);
        }

        /*
         * With MapAttribute + Identity Property
         */

        [TestMethod]
        public void TestPrimaryResolverWithMapAttributeAndWithIdentityProperty()
        {
            // Setup
            var resolver = new PrimaryResolver();

            // Act
            var result = resolver.Resolve(typeof(EntityModelWithMapAttributeAndIdentityProperty))?.GetMappedName();
            var expected = "MapId";

            // Assert
            Assert.AreEqual(expected, result);
        }

        /*
         * With TableAttribute + Identity Property
         */

        [TestMethod]
        public void TestPrimaryResolverWithTableAttributeAndWithIdentityProperty()
        {
            // Setup
            var resolver = new PrimaryResolver();

            // Act
            var result = resolver.Resolve(typeof(EntityModelWithTableAttributeAndIdentityProperty))?.GetMappedName();
            var expected = "TableId";

            // Assert
            Assert.AreEqual(expected, result);
        }
    }
}
