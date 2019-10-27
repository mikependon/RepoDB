using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;

namespace RepoDb.UnitTests.Others
{
    [TestClass]
    public class IdentityCacheTest
    {
        #region SubClasses

        public class BaseClass
        {
            public int PrimaryId { get; set; }
            public string Property1 { get; set; }
        }

        public class BaseClassWithIdentity
        {
            [Identity]
            public int IdentityId { get; set; }
            public string Property1 { get; set; }
        }

        public class DerivedClass : BaseClass
        {
            public string Property2 { get; set; }
            public string Property3 { get; set; }
        }

        public class DerivedClassWithIdentityAtBase : BaseClassWithIdentity
        {
            public string Property2 { get; set; }
            public string Property3 { get; set; }
        }

        #endregion

        #region BaseClass

        [TestMethod]
        public void TestIdentityCacheForBaseClass()
        {
            // Act
            var identity = IdentityCache.Get<BaseClassWithIdentity>();

            // Assert
            Assert.IsNotNull(identity);
        }

        [TestMethod]
        public void TestIdentityCacheForTypeOfBaseClass()
        {
            // Act
            var identity = IdentityCache.Get(typeof(BaseClassWithIdentity));

            // Assert
            Assert.IsNotNull(identity);
        }

        [TestMethod]
        public void TestIdentityCacheForBaseClassWithoutIdentity()
        {
            // Act
            var identity = IdentityCache.Get<BaseClass>();

            // Assert
            Assert.IsNull(identity);
        }

        [TestMethod]
        public void TestIdentityCacheForTypeOfBaseClassWithoutIdentity()
        {
            // Act
            var identity = IdentityCache.Get(typeof(BaseClass));

            // Assert
            Assert.IsNull(identity);
        }

        #endregion

        #region DerivedClass

        [TestMethod]
        public void TestIdentityCacheForDerivedClass()
        {
            // Act
            var identity = IdentityCache.Get<DerivedClassWithIdentityAtBase>();

            // Assert
            Assert.IsNotNull(identity);
        }

        [TestMethod]
        public void TestIdentityCacheForTypeOfDerivedClass()
        {
            // Act
            var identity = IdentityCache.Get(typeof(DerivedClassWithIdentityAtBase));

            // Assert
            Assert.IsNotNull(identity);
        }

        [TestMethod]
        public void TestIdentityCacheForDerivedClassWithoutIdentity()
        {
            // Act
            var identity = IdentityCache.Get<DerivedClass>();

            // Assert
            Assert.IsNull(identity);
        }

        [TestMethod]
        public void TestIdentityCacheForTypeOfDerivedClassWithoutIdentity()
        {
            // Act
            var identity = IdentityCache.Get(typeof(DerivedClass));

            // Assert
            Assert.IsNull(identity);
        }

        #endregion
    }
}
