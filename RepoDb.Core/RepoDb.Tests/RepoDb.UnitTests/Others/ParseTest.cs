using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;
using System.Linq;

namespace RepoDb.UnitTests.Others
{
    [TestClass]
    public class ParseTest
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
        public void TestQueryGroupParse()
        {
            // Act
            var properties = typeof(DerivedClass).GetProperties().AsList();
            var queryGroup = QueryGroup.Parse(new DerivedClass());
            var queryFields = queryGroup.GetFields(true).AsList();

            // Assert
            Assert.AreEqual(4, queryFields.Count());
            properties.ForEach(p =>
            {
                var queryField = queryFields.FirstOrDefault(qf => qf.Field.Name == p.Name);
                Assert.IsNotNull(queryField);
                Assert.AreEqual(p.PropertyType, queryField.Field.Type);
            });
        }

        [TestMethod]
        public void TestQueryGroupParseAsExpression()
        {
            // Act
            var queryGroup = QueryGroup.Parse<DerivedClass>(e => e.PrimaryId == 1);
            var queryFields = queryGroup.GetFields(true).AsList();

            // Assert
            Assert.AreEqual(1, queryFields.Count());

            // Prepare
            var queryField = queryFields.First();

            // Assert
            Assert.AreEqual("PrimaryId", queryField.Field.Name);
            Assert.AreEqual(typeof(int), queryField.Field.Type);
        }

        #endregion

        #region PropertyCache

        [TestMethod]
        public void TestPropertyCacheGet()
        {
            // Act
            var properties = typeof(DerivedClass).GetProperties().AsList();
            var classProperties = PropertyCache.Get<DerivedClass>().AsList();

            // Assert
            Assert.AreEqual(4, properties.Count());
            Assert.AreEqual(4, classProperties.Count());
            properties.ForEach(p =>
            {
                var property = classProperties.FirstOrDefault(cp => cp.PropertyInfo == p);
                Assert.IsNotNull(property);
            });
        }

        #endregion

        #region FieldCache

        [TestMethod]
        public void TestFieldCacheGet()
        {
            // Act
            var properties = typeof(DerivedClass).GetProperties().AsList();
            var fields = FieldCache.Get<DerivedClass>().AsList();

            // Assert
            Assert.AreEqual(4, properties.Count());
            Assert.AreEqual(4, fields.Count());
            properties.ForEach(p =>
            {
                var field = fields.FirstOrDefault(f => f.Name == p.Name);
                Assert.IsNotNull(field);
                Assert.AreEqual(p.PropertyType, field.Type);
            });
        }

        [TestMethod]
        public void TestFieldParse()
        {
            // Act
            var properties = typeof(DerivedClass).GetProperties().AsList();
            var fields = Field.Parse(new DerivedClass()).AsList();

            // Assert
            Assert.AreEqual(4, properties.Count());
            Assert.AreEqual(4, fields.Count());
            properties.ForEach(p =>
            {
                var field = fields.FirstOrDefault(f => f.Name == p.Name);
                Assert.IsNotNull(field);
                Assert.AreEqual(p.PropertyType, field.Type);
            });
        }

        [TestMethod]
        public void TestFieldParseAsExpression()
        {
            // Act
            var field = Field.Parse<DerivedClass>(e => e.PrimaryId)?.FirstOrDefault();

            // Assert
            Assert.AreEqual("PrimaryId", field?.Name);
            Assert.AreEqual(typeof(int), field?.Type);
        }

        #endregion
    }
}
