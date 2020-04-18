using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RepoDb.UnitTests
{
    [TestClass]
    public class ClassExpressionTest
    {
        #region SubClasses

        private class ClassExpressionTestClass
        {
            public int Id { get; set; }
            public string Property1 { get; set; }
        }

        #endregion

        #region Helpers

        private IEnumerable<ClassExpressionTestClass> GetEntities(int count = 10)
        {
            for (var i = 0; i < count; i++)
            {
                yield return new ClassExpressionTestClass
                {
                    Id = i,
                    Property1 = Guid.NewGuid().ToString()
                };
            }
        }

        #endregion

        #region GetEntitiesPropertyValues

        [TestMethod]
        public void TestClassExpressionGetEntitiesPropertyValuesViaExpression()
        {
            // Setup
            var entities = GetEntities(10).AsList();

            // Act
            var propertyValues = ClassExpression.GetEntitiesPropertyValues<ClassExpressionTestClass, string>(entities, e => e.Property1);

            // Assert
            Assert.AreEqual(entities.Count(), propertyValues.Count());
            for (var i = 0; i < entities.Count(); i++)
            {
                var entity = entities.ElementAt(i);
                Assert.AreEqual(entity.Property1, propertyValues.ElementAt(i));
            }
        }

        [TestMethod]
        public void TestClassExpressionGetEntitiesPropertyValuesViaPropertyName()
        {
            // Setup
            var entities = GetEntities(10).AsList();

            // Act
            var propertyValues = ClassExpression.GetEntitiesPropertyValues<ClassExpressionTestClass, string>(entities, "Property1");

            // Assert
            Assert.AreEqual(entities.Count(), propertyValues.Count());
            for (var i = 0; i < entities.Count(); i++)
            {
                var entity = entities.ElementAt(i);
                Assert.AreEqual(entity.Property1, propertyValues.ElementAt(i));
            }
        }

        [TestMethod]
        public void TestClassExpressionGetEntitiesPropertyValuesViaField()
        {
            // Setup
            var entities = GetEntities(10).AsList();

            // Act
            var propertyValues = ClassExpression.GetEntitiesPropertyValues<ClassExpressionTestClass, string>(entities, new Field("Property1"));

            // Assert
            Assert.AreEqual(entities.Count(), propertyValues.Count());
            for (var i = 0; i < entities.Count(); i++)
            {
                var entity = entities.ElementAt(i);
                Assert.AreEqual(entity.Property1, propertyValues.ElementAt(i));
            }
        }

        #endregion

        #region GetProperties

        [TestMethod]
        public void TestClassExpressionGetProperties()
        {
            // Act
            var properties = ClassExpression.GetProperties<ClassExpressionTestClass>();

            // Assert
            Assert.AreEqual(2, properties.Count());
        }

        #endregion

        #region GetPropertiesAndValues

        [TestMethod]
        public void TestClassExpressionGetPropertiesAndValues()
        {
            // Setup
            var entity = GetEntities(1).First();

            // Act
            var propertyValues = ClassExpression.GetPropertiesAndValues<ClassExpressionTestClass>(entity);

            // Assert
            Assert.AreEqual(2, propertyValues.Count());
            Assert.AreEqual("Id", propertyValues.ElementAt(0).Name);
            Assert.AreEqual(entity.Id, propertyValues.ElementAt(0).Value);
            Assert.AreEqual("Property1", propertyValues.ElementAt(1).Name);
            Assert.AreEqual(entity.Property1, propertyValues.ElementAt(1).Value);
        }

        #endregion
    }
}
