using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;

namespace RepoDb.UnitTests.Caches
{
    [TestClass]
    public class PrimaryCacheTest
    {
        #region SubClasses

        public class BaseClass
        {
            public int PrimaryId { get; set; }
            public string Property1 { get; set; }
        }

        public class BaseClassWithPrimary
        {
            [Primary]
            public int PrimaryId { get; set; }
            public string Property1 { get; set; }
        }

        public class DerivedClass : BaseClass
        {
            public string Property2 { get; set; }
            public string Property3 { get; set; }
        }

        public class DerivedClassWithPrimaryAtBase : BaseClassWithPrimary
        {
            public string Property2 { get; set; }
            public string Property3 { get; set; }
        }

        private class PrimaryClass
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private class PrimaryClassWithPrimary
        {
            [Primary]
            public int PrimaryId { get; set; }
            public string Name { get; set; }
        }

        private class PrimaryClassWithUniformPrimary
        {
            public int PrimaryClassWithUniformPrimaryId { get; set; }
            public string Name { get; set; }
        }

        [Map("Primary")]
        private class PrimaryClassWithUniformPrimaryFromTheMapping
        {
            public int PrimaryId { get; set; }
            public string Name { get; set; }
        }

        #endregion

        #region BaseClass

        [TestMethod]
        public void TestPrimaryCacheForBaseClass()
        {
            // Act
            var primary = PrimaryCache.Get<BaseClassWithPrimary>();

            // Assert
            Assert.IsNotNull(primary);
        }

        [TestMethod]
        public void TestPrimaryCacheForTypeOfBaseClass()
        {
            // Act
            var primary = PrimaryCache.Get(typeof(BaseClassWithPrimary));

            // Assert
            Assert.IsNotNull(primary);
        }

        [TestMethod]
        public void TestPrimaryCacheForBaseClassWithoutPrimary()
        {
            // Act
            var primary = PrimaryCache.Get<BaseClass>();

            // Assert
            Assert.IsNull(primary);
        }

        [TestMethod]
        public void TestPrimaryCacheForTypeOfBaseClassWithoutPrimary()
        {
            // Act
            var primary = PrimaryCache.Get(typeof(BaseClass));

            // Assert
            Assert.IsNull(primary);
        }

        #endregion

        #region DerivedClass

        [TestMethod]
        public void TestPrimaryCacheForDerivedClass()
        {
            // Act
            var primary = PrimaryCache.Get<DerivedClassWithPrimaryAtBase>();

            // Assert
            Assert.IsNotNull(primary);
        }

        [TestMethod]
        public void TestPrimaryCacheForTypeOfDerivedClass()
        {
            // Act
            var primary = PrimaryCache.Get(typeof(DerivedClassWithPrimaryAtBase));

            // Assert
            Assert.IsNotNull(primary);
        }

        [TestMethod]
        public void TestPrimaryCacheForDerivedClassWithoutPrimary()
        {
            // Act
            var primary = PrimaryCache.Get<DerivedClass>();

            // Assert
            Assert.IsNull(primary);
        }

        [TestMethod]
        public void TestPrimaryCacheForTypeOfDerivedClassWithoutPrimary()
        {
            // Act
            var primary = PrimaryCache.Get(typeof(DerivedClass));

            // Assert
            Assert.IsNull(primary);
        }

        #endregion

        #region Names

        [TestMethod]
        public void TestPrimaryClass()
        {
            // Act
            var primary = PrimaryCache.Get<PrimaryClass>();

            // Assert
            Assert.IsNotNull(primary);
        }

        [TestMethod]
        public void TestPrimaryClassWithPrimary()
        {
            // Act
            var primary = PrimaryCache.Get<PrimaryClassWithPrimary>();

            // Assert
            Assert.IsNotNull(primary);
        }

        [TestMethod]
        public void TestPrimaryClassWithUniformPrimary()
        {
            // Act
            var primary = PrimaryCache.Get<PrimaryClassWithUniformPrimary>();

            // Assert
            Assert.IsNotNull(primary);
        }

        [TestMethod]
        public void TestPrimaryClassWithUniformPrimaryFromTheMapping()
        {
            // Act
            var primary = PrimaryCache.Get<PrimaryClassWithUniformPrimaryFromTheMapping>();

            // Assert
            Assert.IsNotNull(primary);
        }

        #endregion
    }
}
