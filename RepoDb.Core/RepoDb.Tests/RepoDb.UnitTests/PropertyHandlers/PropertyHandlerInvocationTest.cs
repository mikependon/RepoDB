using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.UnitTests.CustomObjects;

namespace RepoDb.UnitTests.PropertyHandlers
{
    [TestClass]
    public class PropertyHandlerInvocationTest
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

        #region SubClasses

        private class PropertyHandlerTestClass
        {
            public int Id { get; set; }
        }

        private class PropertyHandlerTestClassForPrimaryKey : PropertyHandlerTestClass { }

        private class PropertyHandlerTestClassForDynamic : PropertyHandlerTestClass { }

        private class PropertyHandlerTestClassForLinq : PropertyHandlerTestClass { }

        private class PropertyHandlerTestClassForQueryField : PropertyHandlerTestClass { }

        private class PropertyHandlerTestClassForQueryFields : PropertyHandlerTestClass { }

        private class PropertyHandlerTestClassForQueryGroup : PropertyHandlerTestClass { }

        #endregion

        [TestMethod]
        public void TestPropertyHandlerInvocationOnPrimaryKey()
        {
            // Prepare
            var propertyHandler = new Mock<IPropertyHandler<int, int>>();
            FluentMapper
                .Entity<PropertyHandlerTestClassForPrimaryKey>()
                .PropertyHandler(e => e.Id, propertyHandler.Object);

            // Act
            using (var connection = new PropertyHandlerConnection())
            {
                connection.Query<PropertyHandlerTestClassForPrimaryKey>(1);
            }

            // Assert
            propertyHandler.Verify(c => c.Set(It.IsAny<int>(), It.IsAny<ClassProperty>()), Times.Once);
        }

        [TestMethod]
        public void TestPropertyHandlerInvocationOnLinqExpression()
        {
            // Prepare
            var propertyHandler = new Mock<IPropertyHandler<int, int>>();
            FluentMapper
                .Entity<PropertyHandlerTestClassForLinq>()
                .PropertyHandler(e => e.Id, propertyHandler.Object);

            // Act
            using (var connection = new PropertyHandlerConnection())
            {
                connection.Query<PropertyHandlerTestClassForLinq>(e => e.Id == 1);
            }

            // Assert
            propertyHandler.Verify(c => c.Set(It.IsAny<int>(), It.IsAny<ClassProperty>()), Times.Once);
        }

        [TestMethod]
        public void TestPropertyHandlerInvocationOnDynamicExpression()
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
        public void TestPropertyHandlerInvocationOnQueryFieldExpression()
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
        public void TestPropertyHandlerInvocationOnQueryFieldsExpression()
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
        public void TestPropertyHandlerInvocationOnQueryGroupExpression()
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
    }
}
