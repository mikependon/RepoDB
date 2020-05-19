using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using RepoDb.Resolvers;
using System.ComponentModel.DataAnnotations;

namespace RepoDb.UnitTests.Resolvers
{
    [TestClass]
    public class IdentityResolverTest
    {
        #region SubClasses

        private class EntityModelWithIdentityAttribute
        {
            [Identity]
            public int PrimaryId { get; set; }
            [Key]
            public int SecondaryId { get; set; }
        }

        private class EntityModelWithKeyAttribute
        {
            [Identity]
            public int PrimaryId { get; set; }
            [Key]
            public int SecondaryId { get; set; }
        }

        private class EntityModelWithIdentityAndKeyAttribute
        {
            [Identity]
            public int PrimaryId { get; set; }
            [Key]
            public int SecondaryId { get; set; }
        }

        #endregion

        /*
         * With Attributes
         */

        [TestMethod]
        public void TestIdentityResolverWithIdentityAttribute()
        {
            // Setup
            var resolver = new IdentityResolver();

            // Act
            var result = resolver.Resolve(typeof(EntityModelWithIdentityAttribute)).GetMappedName();
            var expected = "PrimaryId";

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestIdentityResolverWithKeyAttribute()
        {
            // Setup
            var resolver = new IdentityResolver();

            // Act
            var result = resolver.Resolve(typeof(EntityModelWithKeyAttribute)).GetMappedName();
            var expected = "PrimaryId";

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestIdentityResolverWithMapAndKeyAttribute()
        {
            // Setup
            var resolver = new IdentityResolver();

            // Act
            var result = resolver.Resolve(typeof(EntityModelWithIdentityAndKeyAttribute)).GetMappedName();
            var expected = "PrimaryId";

            // Assert
            Assert.AreEqual(expected, result);
        }

        /*
         * With Mappings
         */

        [TestMethod]
        public void TestIdentityResolverWithIdentityAttributeAndMappings()
        {
            // Setup
            var resolver = new IdentityResolver();
            FluentMapper
                .Entity<EntityModelWithIdentityAttribute>()
                .Primary(e => e.SecondaryId);

            // Act
            var result = resolver.Resolve(typeof(EntityModelWithIdentityAttribute)).GetMappedName();
            var expected = "PrimaryId";

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestIdentityResolverWithKeyAttributeAndMappings()
        {
            // Setup
            var resolver = new IdentityResolver();
            FluentMapper
                .Entity<EntityModelWithKeyAttribute>()
                .Primary(e => e.SecondaryId);

            // Act
            var result = resolver.Resolve(typeof(EntityModelWithKeyAttribute)).GetMappedName();
            var expected = "PrimaryId";

            // Assert
            Assert.AreEqual(expected, result);
        }
    }
}
