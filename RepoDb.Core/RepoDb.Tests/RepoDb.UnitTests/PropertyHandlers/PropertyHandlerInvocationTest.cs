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
            StatementBuilderMapper.Add(typeof(PropertyHandlerConnection), new CustomStatementBuilder(), true);
        }

        [TestCleanup]
        public void Cleanup()
        {
            PropertyHandlerMapper.Clear();
        }

        #region CustomObjects

        private class PropertyHandlerConnection : CustomDbConnection { }

        private class PropertyHandlerTestClassRepository : BaseRepository<PropertyHandlerTestClass, PropertyHandlerConnection>
        {
            public PropertyHandlerTestClassRepository()
                : base("ConnectionString")
            { }
        }

        #endregion

        #region SubClasses

        private class PropertyHandlerTestClass
        {
            public int Id { get; set; }
        }

        #endregion

        [TestMethod]
        public void TestPropertyHandlerInvocationOnLinqExpression()
        {
            // Prepare
            var propertyHandler = new Mock<IPropertyHandler<int, int>>();
            var repository = new PropertyHandlerTestClassRepository();
            FluentMapper.Entity<PropertyHandlerTestClass>()
                .PropertyHandler(e => e.Id, propertyHandler.Object);

            // Act
            repository.Query(e => e.Id == 1);

            // Assert
            propertyHandler.Verify(c => c.Set(It.IsAny<int>(), It.IsAny<ClassProperty>()), Times.Once);
        }

        [TestMethod]
        public void TestPropertyHandlerInvocationOnDynamicExpression()
        {
            // Prepare
            var propertyHandler = new Mock<IPropertyHandler<int, int>>();
            var repository = new PropertyHandlerTestClassRepository();
            FluentMapper.Entity<PropertyHandlerTestClass>()
                .PropertyHandler(e => e.Id, propertyHandler.Object);

            // Act
            repository.Query(new { Id = 1 });

            // Assert
            propertyHandler.Verify(c => c.Set(It.IsAny<int>(), It.IsAny<ClassProperty>()), Times.Once);
        }

        [TestMethod]
        public void TestPropertyHandlerInvocationOnQueryFieldExpression()
        {
            // Prepare
            var propertyHandler = new Mock<IPropertyHandler<int, int>>();
            var repository = new PropertyHandlerTestClassRepository();
            FluentMapper.Entity<PropertyHandlerTestClass>()
                .PropertyHandler(e => e.Id, propertyHandler.Object);

            // Act
            repository.Query(new QueryField("Id", 1));

            // Assert
            propertyHandler.Verify(c => c.Set(It.IsAny<int>(), It.IsAny<ClassProperty>()), Times.Once);
        }

        [TestMethod]
        public void TestPropertyHandlerInvocationOnQueryFieldsExpression()
        {
            // Prepare
            var propertyHandler = new Mock<IPropertyHandler<int, int>>();
            var repository = new PropertyHandlerTestClassRepository();
            FluentMapper.Entity<PropertyHandlerTestClass>()
                .PropertyHandler(e => e.Id, propertyHandler.Object);

            // Act
            repository.Query((new QueryField("Id", 1)).AsEnumerable());

            // Assert
            propertyHandler.Verify(c => c.Set(It.IsAny<int>(), It.IsAny<ClassProperty>()), Times.Once);
        }

        [TestMethod]
        public void TestPropertyHandlerInvocationOnQueryGroupExpression()
        {
            // Prepare
            var propertyHandler = new Mock<IPropertyHandler<int, int>>();
            var repository = new PropertyHandlerTestClassRepository();
            FluentMapper.Entity<PropertyHandlerTestClass>()
                .PropertyHandler(e => e.Id, propertyHandler.Object);

            // Act
            repository.Query(new QueryGroup(new QueryField("Id", 1)));

            // Assert
            propertyHandler.Verify(c => c.Set(It.IsAny<int>(), It.IsAny<ClassProperty>()), Times.Once);
        }
    }
}
