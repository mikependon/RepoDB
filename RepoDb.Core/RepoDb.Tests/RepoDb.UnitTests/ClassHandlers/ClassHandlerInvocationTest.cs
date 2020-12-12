using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoDb.Interfaces;
using RepoDb.UnitTests.CustomObjects;
using System.Data;
using System.Data.Common;

namespace RepoDb.UnitTests.ClassHandlers
{
    [TestClass]
    public class ClassHandlerInvocationTest
    {
        [TestInitialize]
        public void Initialize()
        {
            DbSettingMapper.Add(typeof(ClassHandlerConnection), new CustomDbSetting(), true);
            DbHelperMapper.Add(typeof(ClassHandlerConnection), new CustomDbHelper(), true);
            StatementBuilderMapper.Add(typeof(ClassHandlerConnection), new CustomStatementBuilder(), true);
        }

        [TestCleanup]
        public void Cleanup()
        {
            ClassHandlerMapper.Clear();
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

        #endregion

        #region SubClasses

        public class ClassHandlerTestClass
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        #endregion

        #region Methods

        [TestMethod]
        public void TestClassHandlerInvocationOnGet()
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

        //[TestMethod]
        //public void TestClassHandlerInvocationOnSet()
        //{
        //    // Prepare
        //    var classHandler = new Mock<IClassHandler<ClassHandlerTestClass>>();
        //    FluentMapper
        //        .Entity<ClassHandlerTestClass>()
        //        .ClassHandler(classHandler.Object);

        //    classHandler
        //        .Setup(e => e.Set(It.IsAny<ClassHandlerTestClass>()))
        //        .Returns(new ClassHandlerTestClass { Id = 1, Name = "James Smith" });

        //    // Act
        //    using (var connection = new ClassHandlerConnection())
        //    {
        //        connection.Insert<ClassHandlerTestClass>(new ClassHandlerTestClass { Id = 1, Name = "James Smith" });
        //    }

        //    // Assert
        //    classHandler.Verify(c => c.Set(It.IsAny<ClassHandlerTestClass>()), Times.Once);
        //}

        #endregion
    }
}
