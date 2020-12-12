using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoDb.Attributes;
using RepoDb.Interfaces;
using RepoDb.UnitTests.CustomObjects;
using System.Data;
using System.Data.Common;

namespace RepoDb.UnitTests.ClassHandlers
{
    [TestClass]
    public class ClassHandlerPrecedenceTest
    {
        [TestInitialize]
        public void Initialize()
        {
            // For Non-Attributed Entity
            DbSettingMapper.Add(typeof(ClassHandlerConnection), new CustomDbSetting(), true);
            DbHelperMapper.Add(typeof(ClassHandlerConnection), new CustomDbHelper(), true);
            StatementBuilderMapper.Add(typeof(ClassHandlerConnection), new CustomStatementBuilder(), true);

            // For Attributed Entity
            DbSettingMapper.Add(typeof(ClassHandlerForEntityWithAttributeConnection), new CustomDbSetting(), true);
            DbHelperMapper.Add(typeof(ClassHandlerForEntityWithAttributeConnection), new CustomDbHelper(), true);
            StatementBuilderMapper.Add(typeof(ClassHandlerForEntityWithAttributeConnection), new CustomStatementBuilder(), true);
        }

        [TestCleanup]
        public void Cleanup()
        {
            PropertyHandlerMapper.Clear();
        }

        #region CustomObjects

        private class ClassHandlerConnection : CustomDbConnection
        {
            protected override DbCommand CreateDbCommand()
            {
                return new ClassHandlerDbCommand();
            }
        }

        private class ClassHandlerDbCommand : CustomDbCommand
        {
            protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
            {
                var reader = new DataEntityDataReader<ClassHandlerTestClass>(new[]
                {
                    new ClassHandlerTestClass { Id = 1, Name = "James Doe" }
                });
                return reader;
            }
        }

        private class ClassHandlerForEntityWithAttributeConnection : CustomDbConnection
        {
            protected override DbCommand CreateDbCommand()
            {
                return new ClassHandlerForEntityWithAttributeDbCommand();
            }
        }

        private class ClassHandlerForEntityWithAttributeDbCommand : CustomDbCommand
        {
            protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
            {
                var reader = new DataEntityDataReader<ClassHandlerTestClassWithAttribute>(new[]
                {
                    new ClassHandlerTestClassWithAttribute { Id = 1, Name = "James Doe" }
                });
                return reader;
            }
        }

        #endregion

        #region ClassHandlers

        public class TestClassHandler : IClassHandler<ClassHandlerTestClass>
        {
            public ClassHandlerTestClass Get(ClassHandlerTestClass entity, DbDataReader reader)
            {
                return entity;
            }

            public ClassHandlerTestClass Set(ClassHandlerTestClass entity)
            {
                return entity;
            }
        }

        public class TestClassHandlerForEntityWithAttribute : IClassHandler<ClassHandlerTestClassWithAttribute>
        {
            public ClassHandlerTestClassWithAttribute Get(ClassHandlerTestClassWithAttribute entity, DbDataReader reader)
            {
                return entity;
            }

            public ClassHandlerTestClassWithAttribute Set(ClassHandlerTestClassWithAttribute entity)
            {
                return entity;
            }
        }

        #endregion

        #region SubClasses

        public class ClassHandlerTestClass
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        [ClassHandler(typeof(TestClassHandlerForEntityWithAttribute))]
        public class ClassHandlerTestClassWithAttribute
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        #endregion

        #region Methods

        [TestMethod]
        public void TestClassHandlerPrecedence()
        {
            // Prepare
            var classHandler = new Mock<IClassHandler<ClassHandlerTestClass>>();
            FluentMapper
                .Entity<ClassHandlerTestClass>()
                .ClassHandler(classHandler.Object);

            // Act
            using (var connection = new ClassHandlerConnection())
            {
                var result = connection.QueryAll<ClassHandlerTestClass>();
            }

            // Assert
            classHandler.Verify(c => c.Get(It.IsAny<ClassHandlerTestClass>(), It.IsAny<DbDataReader>()), Times.Once);
        }

        [TestMethod]
        public void TestClassHandlerPrecedenceWithAttribute()
        {
            // Prepare
            var classHandler = new Mock<IClassHandler<ClassHandlerTestClassWithAttribute>>();
            FluentMapper
                .Entity<ClassHandlerTestClassWithAttribute>()
                .ClassHandler(classHandler.Object);

            // Act
            using (var connection = new ClassHandlerForEntityWithAttributeConnection())
            {
                var result = connection.QueryAll<ClassHandlerTestClassWithAttribute>();
            }

            // Assert
            classHandler.Verify(c => c.Get(It.IsAny<ClassHandlerTestClassWithAttribute>(), It.IsAny<DbDataReader>()), Times.Never);
        }

        #endregion
    }
}
