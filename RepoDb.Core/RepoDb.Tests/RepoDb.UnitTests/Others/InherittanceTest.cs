using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using RepoDb.Extensions;
using System.Linq;

namespace RepoDb.UnitTests.Others
{
    [TestClass]
    public class InherittanceTest
    {
        #region SubClasses

        public interface IBaseModel
        {
            int Id { get; set; }
        }

        public class BaseModel : IBaseModel
        {
            [Map("PrimaryId")]
            public int Id { get; set; }
            public string Property1 { get; set; }
        }

        public class DerivedModel : BaseModel
        {
            public string Property2 { get; set; }
            public string Property3 { get; set; }
        }

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

        #region Repositories

        public interface IRepository<TModel> where TModel : class
        {
            QueryGroup Parse(int id);
        }

        public class GenericRepository<TModel> : IRepository<TModel>
            where TModel : class, IBaseModel, new()
        {
            public QueryGroup Parse(int id)
            {
                return QueryGroup.Parse<TModel>(e => e.Id == id);
            }
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

        [TestMethod]
        public void TestQueryGroupParseForInterfaceProperty()
        {
            // Act
            var queryGroup = QueryGroup.Parse(new DerivedModel());

            // Assert
            Assert.AreEqual(4, queryGroup.GetFields(true).Count());
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionForInterfacePropertyMapping()
        {
            // Act
            var queryGroup = QueryGroup.Parse<DerivedModel>(e => e.Id == 1);

            // Assert
            Assert.AreEqual(1, queryGroup.GetFields(true).Count());

            // Setup
            var queryField = queryGroup.GetFields(true).First();

            // Assert
            Assert.AreEqual("PrimaryId", queryField.Field.Name);
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
            var field = Field.Parse<DerivedClass>(e => e.PrimaryId).FirstOrDefault();

            // Assert
            Assert.AreEqual("PrimaryId", field?.Name);
        }

        #endregion

        #region Community Reported

        /// <summary>
        /// Issue: https://github.com/mikependon/RepoDb/issues/364
        /// Description: Error: Invalid expression. The property <Id> is not defined on a target type #364
        /// </summary>
        [TestMethod]
        public void TestQueryGroupParseExpressionForRepositoryInterfacePropertyMapping()
        {
            // Act
            var queryGroup = new GenericRepository<DerivedModel>().Parse(1);

            // Assert
            Assert.AreEqual(1, queryGroup.GetFields(true).Count());

            // Setup
            var queryField = queryGroup.GetFields(true).First();

            // Assert
            Assert.AreEqual("PrimaryId", queryField.Field.Name);
        }

        #endregion
    }
}
