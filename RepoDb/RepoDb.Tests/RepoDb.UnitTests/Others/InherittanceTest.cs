using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;
using System.Linq;

namespace RepoDb.UnitTests.Others
{
    [TestClass]
    public class InherittanceTest
    {
        #region SubClasses

        public class BaseClass
        {
            public int PrimaryId { get; set; }
            public string Property1 { get; set; }
        }

        public class DerivedClass : BaseClass
        {
            public string Property2 { get; set; }
            public string Property3 { get; set; }
        }

        #endregion

        #region QueryGroup

        [TestMethod]
        public void TestQueryGroupParseForDerivedClass()
        {
            // Act
            var queryGroup = QueryGroup.Parse(new DerivedClass());

            // Assert
            Assert.AreEqual(4, queryGroup.GetFields(true).Count());
        }

        #endregion

        #region PropertyCache

        [TestMethod]
        public void TestPropertyCacheGetForDerivedClass()
        {
            // Act
            var properties = PropertyCache.Get<DerivedClass>().AsList();

            // Assert
            Assert.AreEqual<long>(4, properties.Count());
        }

        #endregion

        #region FieldCache

        [TestMethod]
        public void TestFieldCacheGetForDerivedClass()
        {
            // Act
            var fields = FieldCache.Get<DerivedClass>().AsList();

            // Assert
            Assert.AreEqual(4, fields.Count());
        }

        [TestMethod]
        public void TestFieldParseForDerivedClass()
        {
            // Act
            var fields = Field.Parse(new DerivedClass()).AsList();

            // Assert
            Assert.AreEqual(4, fields.Count());
        }

        [TestMethod]
        public void TestFieldParseForDerivedClassAsExpression()
        {
            // Act
            var field = Field.Parse<DerivedClass>(e => e.PrimaryId);

            // Assert
            Assert.AreEqual("PrimaryId", field.Name);
        }

        #endregion
    }
}
