using System.Data;
using System.Data.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoDb.Attributes;
using RepoDb.Interfaces;
using RepoDb.Options;
using RepoDb.UnitTests.CustomObjects;

namespace RepoDb.UnitTests.ClassHandlers
{
    [TestClass]
    public class ClassHandlerPrecedenceTest
    {
        [TestInitialize]
        public void Initialize()
        {
            // For Non-Attributed Entity
            DbSettingMapper.Add<ClassHandlerConnection>(new CustomDbSetting(), true);
            DbHelperMapper.Add<ClassHandlerConnection>(new CustomDbHelper(), true);
            StatementBuilderMapper.Add<ClassHandlerConnection>(new CustomStatementBuilder(), true);

            // For Attributed Entity
            DbSettingMapper.Add<ClassHandlerForEntityWithAttributeConnection>(new CustomDbSetting(), true);
            DbHelperMapper.Add<ClassHandlerForEntityWithAttributeConnection>(new CustomDbHelper(), true);
            StatementBuilderMapper.Add<ClassHandlerForEntityWithAttributeConnection>(new CustomStatementBuilder(), true);

#if NET
            // For Generic Attributed Entity
            DbSettingMapper.Add<ClassHandlerForEntityWithGenericAttributeConnection>(new CustomDbSetting(), true);
            DbHelperMapper.Add<ClassHandlerForEntityWithGenericAttributeConnection>(new CustomDbHelper(), true);
            StatementBuilderMapper.Add<ClassHandlerForEntityWithGenericAttributeConnection>(new CustomStatementBuilder(), true);
#endif
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

#if NET
        private class ClassHandlerForEntityWithGenericAttributeConnection : CustomDbConnection
        {
            protected override DbCommand CreateDbCommand()
            {
                return new ClassHandlerForEntityWithGenericAttributeDbCommand();
            }
        }

        private class ClassHandlerForEntityWithGenericAttributeDbCommand : CustomDbCommand
        {
            protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
            {
                var reader = new DataEntityDataReader<ClassHandlerTestClassWithGenericAttribute>(new[]
                {
                    new ClassHandlerTestClassWithGenericAttribute { Id = 1, Name = "James Doe" }
                });
                return reader;
            }
        }
#endif
        #endregion

        #region ClassHandlers

        public class TestClassHandler : IClassHandler<ClassHandlerTestClass>
        {
            public ClassHandlerTestClass Get(ClassHandlerTestClass entity, ClassHandlerGetOptions options)
            {
                return entity;
            }

            public ClassHandlerTestClass Set(ClassHandlerTestClass entity, ClassHandlerSetOptions options)
            {
                return entity;
            }
        }

        public class TestClassHandlerForEntityWithAttribute : IClassHandler<ClassHandlerTestClassWithAttribute>
        {
            public ClassHandlerTestClassWithAttribute Get(ClassHandlerTestClassWithAttribute entity, ClassHandlerGetOptions options)
            {
                return entity;
            }

            public ClassHandlerTestClassWithAttribute Set(ClassHandlerTestClassWithAttribute entity, ClassHandlerSetOptions options)
            {
                return entity;
            }
        }

#if NET
        public class TestClassHandlerForEntityWithGenericAttribute : IClassHandler<ClassHandlerTestClassWithGenericAttribute>
        {
            public ClassHandlerTestClassWithGenericAttribute Get(ClassHandlerTestClassWithGenericAttribute entity, ClassHandlerGetOptions options)
            {
                return entity;
            }

            public ClassHandlerTestClassWithGenericAttribute Set(ClassHandlerTestClassWithGenericAttribute entity, ClassHandlerSetOptions options)
            {
                return entity;
            }
        }
#endif

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

#if NET
        [ClassHandler<TestClassHandlerForEntityWithGenericAttribute>]
        public class ClassHandlerTestClassWithGenericAttribute
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
#endif
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
            classHandler.Verify(c => c.Get(It.IsAny<ClassHandlerTestClass>(), It.IsAny<ClassHandlerGetOptions>()), Times.Once);
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
            classHandler.Verify(c => c.Get(It.IsAny<ClassHandlerTestClassWithAttribute>(), It.IsAny<ClassHandlerGetOptions>()), Times.Never);
        }

#if NET
        [TestMethod]
        public void TestClassHandlerPrecedenceWithGenericAttribute()
        {
            // Prepare
            var classHandler = new Mock<IClassHandler<ClassHandlerTestClassWithGenericAttribute>>();
            FluentMapper
                .Entity<ClassHandlerTestClassWithGenericAttribute>()
                .ClassHandler(classHandler.Object);

            // Act
            using (var connection = new ClassHandlerForEntityWithGenericAttributeConnection())
            {
                var result = connection.QueryAll<ClassHandlerTestClassWithGenericAttribute>();
            }

            // Assert
            classHandler.Verify(c => c.Get(It.IsAny<ClassHandlerTestClassWithGenericAttribute>(), It.IsAny<ClassHandlerGetOptions>()), Times.Never);
        }
#endif
        #endregion
    }
}
