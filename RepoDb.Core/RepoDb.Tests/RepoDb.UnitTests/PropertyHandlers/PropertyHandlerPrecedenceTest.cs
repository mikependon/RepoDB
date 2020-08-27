using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoDb.Attributes;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.UnitTests.CustomObjects;
using System;
using System.Linq.Expressions;

namespace RepoDb.UnitTests.PropertyHandlers
{
    [TestClass]
    public class PropertyHandlerPrecedenceTest
    {
        [TestInitialize]
        public void Initialize()
        {
            DbSettingMapper.Add(typeof(PropertyHandlerConnection), new CustomDbSetting(), true);
            DbHelperMapper.Add(typeof(PropertyHandlerConnection), new CustomDbHelper(), true);
            StatementBuilderMapper.Add(typeof(PropertyHandlerConnection), new CustomStatementBuilder(), true);
        }

        [TestCleanup]
        public void Cleanup()
        {
            PropertyHandlerMapper.Clear();
        }

        #region CustomObjects

        private class PropertyHandlerConnection : CustomDbConnection { }

        #endregion

        #region PropertyHandlers

        private class IdentityPropertyHandler : IPropertyHandler<int, int>
        {
            public int Get(int input, ClassProperty property)
            {
                return input;
            }

            public int Set(int input, ClassProperty property)
            {
                return input;
            }
        }

        #endregion

        #region SubClasses

        private class PropertyHandlerTestClass
        {
            public int Id { get; set; }
        }

        private class PropertyHandlerTestClassWithAttribute
        {
            [PropertyHandler(typeof(IdentityPropertyHandler))]
            public int Id { get; set; }
        }

        /*
         * Dynamic
         */
        private class PropertyHandlerTestClassForDynamic : PropertyHandlerTestClass { }

        private class PropertyHandlerTestClassWithAttributeForDynamic : PropertyHandlerTestClassWithAttribute { }

        /*
         * Linq
         */
        private class PropertyHandlerTestClassForLinq : PropertyHandlerTestClass { }

        private class PropertyHandlerTestClassWithAttributeForLinq : PropertyHandlerTestClassWithAttribute { }

        /*
         * QueryField
         */
        private class PropertyHandlerTestClassForQueryField : PropertyHandlerTestClass { }

        private class PropertyHandlerTestClassWithAttributeForQueryField : PropertyHandlerTestClassWithAttribute { }

        /*
         * QueryFields
         */
        private class PropertyHandlerTestClassForQueryFields : PropertyHandlerTestClass { }

        private class PropertyHandlerTestClassWithAttributeForQueryFields : PropertyHandlerTestClassWithAttribute { }

        /*
         * QueryGroup
         */
        private class PropertyHandlerTestClassForQueryGroup : PropertyHandlerTestClass { }

        private class PropertyHandlerTestClassWithAttributeForQueryGroup : PropertyHandlerTestClassWithAttribute { }

        #endregion

        #region Linq Expression

        [TestMethod]
        public void TestPropertyHandlerPrecedenceOnLinqExpression()
        {
            // Prepare
            var propertyHandler = new Mock<IPropertyHandler<int, int>>();
            FluentMapper
                .Entity<PropertyHandlerTestClassForLinq>()
                .PropertyHandler(e => e.Id, propertyHandler.Object);

            // Act
            using (var connection = new PropertyHandlerConnection())
            {
                connection.Query((Expression<Func<PropertyHandlerTestClassForLinq, bool>>)(e => e.Id == 1));
            }

            // Assert
            propertyHandler.Verify(c => c.Set(It.IsAny<int>(), It.IsAny<ClassProperty>()), Times.Once);
        }

        [TestMethod]
        public void TestPropertyHandlerWithAttributePrecedenceOnLinqExpression()
        {
            // Prepare
            var propertyHandler = new Mock<IPropertyHandler<int, int>>();
            FluentMapper
                .Entity<PropertyHandlerTestClassWithAttributeForLinq>()
                .PropertyHandler(e => e.Id, propertyHandler.Object);

            // Act
            using (var connection = new PropertyHandlerConnection())
            {
                connection.Query((Expression<Func<PropertyHandlerTestClassWithAttributeForLinq, bool>>)(e => e.Id == 1));
            }

            // Assert
            propertyHandler.Verify(c => c.Set(It.IsAny<int>(), It.IsAny<ClassProperty>()), Times.Never);
        }

        #endregion

        #region Dynamic

        [TestMethod]
        public void TestPropertyHandlerPrecedenceOnDynamicExpression()
        {
            // Prepare
            var propertyHandler = new Mock<IPropertyHandler<int, int>>();
            FluentMapper
                .Entity<PropertyHandlerTestClassForDynamic>()
                .PropertyHandler(e => e.Id, propertyHandler.Object);

            // Act
            using (var connection = new PropertyHandlerConnection())
            {
                connection.Query<PropertyHandlerTestClassForDynamic>(new { Id = 1 });
            }

            // Assert
            propertyHandler.Verify(c => c.Set(It.IsAny<int>(), It.IsAny<ClassProperty>()), Times.Once);
        }

        [TestMethod]
        public void TestPropertyHandlerWithAttributePrecedenceOnDynamicExpression()
        {
            // Prepare
            var propertyHandler = new Mock<IPropertyHandler<int, int>>();
            FluentMapper
                .Entity<PropertyHandlerTestClassWithAttributeForDynamic>()
                .PropertyHandler(e => e.Id, propertyHandler.Object);

            // Act
            using (var connection = new PropertyHandlerConnection())
            {
                connection.Query<PropertyHandlerTestClassWithAttributeForDynamic>(new { Id = 1 });
            }

            // Assert
            propertyHandler.Verify(c => c.Set(It.IsAny<int>(), It.IsAny<ClassProperty>()), Times.Never);
        }

        #endregion

        #region QueryField

        [TestMethod]
        public void TestPropertyHandlerPrecedenceOnQueryFieldExpression()
        {
            // Prepare
            var propertyHandler = new Mock<IPropertyHandler<int, int>>();
            FluentMapper
                .Entity<PropertyHandlerTestClassForQueryField>()
                .PropertyHandler(e => e.Id, propertyHandler.Object);

            // Act
            using (var connection = new PropertyHandlerConnection())
            {
                connection.Query<PropertyHandlerTestClassForQueryField>(new QueryField("Id", 1));
            }

            // Assert
            propertyHandler.Verify(c => c.Set(It.IsAny<int>(), It.IsAny<ClassProperty>()), Times.Once);
        }

        [TestMethod]
        public void TestPropertyHandlerWithAttributePrecedenceOnQueryFieldExpression()
        {
            // Prepare
            var propertyHandler = new Mock<IPropertyHandler<int, int>>();
            FluentMapper
                .Entity<PropertyHandlerTestClassWithAttributeForQueryField>()
                .PropertyHandler(e => e.Id, propertyHandler.Object);

            // Act
            using (var connection = new PropertyHandlerConnection())
            {
                connection.Query<PropertyHandlerTestClassWithAttributeForQueryField>(new QueryField("Id", 1));
            }

            // Assert
            propertyHandler.Verify(c => c.Set(It.IsAny<int>(), It.IsAny<ClassProperty>()), Times.Never);
        }

        #endregion

        #region QueryFields

        [TestMethod]
        public void TestPropertyHandlerPrecedenceOnQueryFieldsExpression()
        {
            // Prepare
            var propertyHandler = new Mock<IPropertyHandler<int, int>>();
            FluentMapper
                .Entity<PropertyHandlerTestClassForQueryFields>()
                .PropertyHandler(e => e.Id, propertyHandler.Object);

            // Act
            using (var connection = new PropertyHandlerConnection())
            {
                connection.Query<PropertyHandlerTestClassForQueryFields>((new QueryField("Id", 1)).AsEnumerable());
            }

            // Assert
            propertyHandler.Verify(c => c.Set(It.IsAny<int>(), It.IsAny<ClassProperty>()), Times.Once);
        }

        [TestMethod]
        public void TestPropertyHandlerWithAttributePrecedenceOnQueryFieldsExpression()
        {
            // Prepare
            var propertyHandler = new Mock<IPropertyHandler<int, int>>();
            FluentMapper
                .Entity<PropertyHandlerTestClassWithAttributeForQueryFields>()
                .PropertyHandler(e => e.Id, propertyHandler.Object);

            // Act
            using (var connection = new PropertyHandlerConnection())
            {
                connection.Query<PropertyHandlerTestClassWithAttributeForQueryFields>((new QueryField("Id", 1)).AsEnumerable());
            }

            // Assert
            propertyHandler.Verify(c => c.Set(It.IsAny<int>(), It.IsAny<ClassProperty>()), Times.Never);
        }

        #endregion

        #region GroupGroup

        [TestMethod]
        public void TestPropertyHandlerPrecedenceOnQueryGroupExpression()
        {
            // Prepare
            var propertyHandler = new Mock<IPropertyHandler<int, int>>();
            FluentMapper
                .Entity<PropertyHandlerTestClassForQueryGroup>()
                .PropertyHandler(e => e.Id, propertyHandler.Object);

            // Act
            using (var connection = new PropertyHandlerConnection())
            {
                connection.Query<PropertyHandlerTestClassForQueryGroup>(new QueryGroup(new QueryField("Id", 1)));
            }

            // Assert
            propertyHandler.Verify(c => c.Set(It.IsAny<int>(), It.IsAny<ClassProperty>()), Times.Once);
        }

        [TestMethod]
        public void TestPropertyHandlerWithAttributePrecedenceOnQueryGroupExpression()
        {
            // Prepare
            var propertyHandler = new Mock<IPropertyHandler<int, int>>();
            FluentMapper
                .Entity<PropertyHandlerTestClassWithAttributeForQueryGroup>()
                .PropertyHandler(e => e.Id, propertyHandler.Object);

            // Act
            using (var connection = new PropertyHandlerConnection())
            {
                connection.Query<PropertyHandlerTestClassWithAttributeForQueryGroup>(new QueryGroup(new QueryField("Id", 1)));
            }

            // Assert
            propertyHandler.Verify(c => c.Set(It.IsAny<int>(), It.IsAny<ClassProperty>()), Times.Never);
        }

        #endregion
    }
}
