using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.UnitTests.CustomObjects;
using System.Data;
using System.Data.Common;

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

        private class PropertyHandlerConnection : CustomDbConnection
        {
            protected override DbCommand CreateDbCommand()
            {
                return new PropertyHandlerDbCommand();
            }
        }

        private class PropertyHandlerDbCommand : CustomDbCommand
        {
            protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
            {
                var reader = new DataEntityDataReader<PropertyHandlerQueryTestClass>(new[]
                {
                    new PropertyHandlerQueryTestClass { Id = 1, Name = "John Doe" }
                });
                return reader;
            }
        }

        #endregion

        #region SubClasses

        private class PropertyHandlerTestClass
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private class PropertyHandlerQueryTestClass : PropertyHandlerTestClass { }

        private class PropertyHandlerInsertTestClass : PropertyHandlerTestClass { }

        private class PropertyHandlerTestClassForPrimaryKey : PropertyHandlerTestClass { }

        private class PropertyHandlerTestClassForDynamic : PropertyHandlerTestClass { }

        private class PropertyHandlerTestClassForLinq : PropertyHandlerTestClass { }

        private class PropertyHandlerTestClassForQueryField : PropertyHandlerTestClass { }

        private class PropertyHandlerTestClassForQueryFields : PropertyHandlerTestClass { }

        private class PropertyHandlerTestClassForQueryGroup : PropertyHandlerTestClass { }

        #endregion

        #region Method

        #region Property Expression

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

        #endregion

        #region Get

        [TestMethod]
        public void TestPropertyHandlerGetInvocation()
        {
            // Prepare
            var propertyHandler = new Mock<IPropertyHandler<int, int>>();
            FluentMapper
                .Entity<PropertyHandlerQueryTestClass>()
                .PropertyHandler(e => e.Id, propertyHandler.Object);

            // Act
            using (var connection = new PropertyHandlerConnection())
            {
                var result = connection.QueryAll<PropertyHandlerQueryTestClass>();
            }

            // Assert
            propertyHandler.Verify(c => c.Get(It.IsAny<int>(), It.IsAny<ClassProperty>()), Times.Once);
        }

        #endregion

        #region Set

        [TestMethod]
        public void TestPropertyHandlerSetInvocation()
        {
            // Prepare
            var propertyHandler = new Mock<IPropertyHandler<string, string>>();
            FluentMapper
                .Entity<PropertyHandlerInsertTestClass>()
                .PropertyHandler(e => e.Name, propertyHandler.Object);

            // Act
            using (var connection = new PropertyHandlerConnection())
            {
                connection.Insert<PropertyHandlerInsertTestClass>(new PropertyHandlerInsertTestClass
                {
                    Id = 1,
                    Name = "James Smith"
                });
            }

            // Assert
            propertyHandler.Verify(c => c.Set(It.IsAny<string>(), It.IsAny<ClassProperty>()), Times.Once);
        }

        #endregion

        #endregion
    }
}
