using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoDb.Contexts.Execution;
using RepoDb.Interfaces;
using RepoDb.UnitTests.CustomObjects;
using System.Collections.Generic;

namespace RepoDb.UnitTests.Interfaces
{
    [TestClass]
    public class IStatementBuilderForDbRepositoryTest
    {
        [TestInitialize]
        public void Initialize()
        {
            DbSettingMapper.Add(typeof(StatementBuilderDbConnection), new CustomDbSetting(), true);
            DbHelperMapper.Add(typeof(StatementBuilderDbConnection), new CustomDbHelper(), true);
        }

        #region SubClasses

        private class StatementBuilderDbConnection : CustomDbConnection { }

        private class StatementBuilderEntity
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private class StatementBuilderEntityForTableName
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private class StatementBuilderEntityForCrossCall
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private class StatementBuilderEntityT1
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private class StatementBuilderEntityT2
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private class StatementBuilderEntityT3
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private class StatementBuilderEntityT4
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private class StatementBuilderEntityT5
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private class StatementBuilderEntityT6
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private class StatementBuilderEntityT7
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        #endregion

        #region Sync

        #region CreateAverage

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForAverage()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.Average<StatementBuilderEntity>(e => e.Id,
                (object)null);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateAverage(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Average<StatementBuilderEntity>(e => e.Id,
                (object)null);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateAverage(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForAverageViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.Average(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"),
                (object)null);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateAverage(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Average(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"),
                (object)null);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateAverage(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForAverageViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.Average<StatementBuilderEntityForCrossCall>(e => e.Id,
                (object)null);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateAverage(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Average(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new Field("Id", typeof(int)),
                (object)null);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateAverage(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateAverageAll

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForAverageAll()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.AverageAll<StatementBuilderEntity>(e => e.Id);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateAverageAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.AverageAll<StatementBuilderEntity>(e => e.Id);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateAverageAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForAverageAllViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.AverageAll(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"));

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateAverageAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.AverageAll(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"));

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateAverageAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForAverageAllViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.AverageAll<StatementBuilderEntityForCrossCall>(e => e.Id);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateAverageAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.AverageAll(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new Field("Id", typeof(int)));

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateAverageAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateBatchQuery

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForBatchQuery()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.BatchQuery<StatementBuilderEntity>(0,
                10,
                null,
                (object)null);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateBatchQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.BatchQuery<StatementBuilderEntity>(0,
                10,
                null,
                (object)null);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateBatchQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateCount

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForCount()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.Count<StatementBuilderEntity>((object)null);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateCount(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Count<StatementBuilderEntity>((object)null);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateCount(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForCountViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.Count(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                (object)null);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateCount(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Count(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                (object)null);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateCount(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForCountViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.Count<StatementBuilderEntityForCrossCall>((object)null);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateCount(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Count(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                (object)null);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateCount(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateCountAll

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForCountAll()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.CountAll<StatementBuilderEntity>();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateCountAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.CountAll<StatementBuilderEntity>();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateCountAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForCountAllViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.CountAll(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>());

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateCountAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.CountAll(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>());

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateCountAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForCountAllViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.CountAll<StatementBuilderEntityForCrossCall>();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateCountAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.CountAll(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>());

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateCountAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateDelete

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForDelete()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.Delete<StatementBuilderEntity>(e => e.Id == 1);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDelete(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Delete<StatementBuilderEntity>(e => e.Id == 1);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateDelete(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForDeleteViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.Delete(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new
                {
                    Id = 1
                });

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDelete(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Delete(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new
                {
                    Id = 1
                });

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateDelete(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForDeleteViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.Delete<StatementBuilderEntityForCrossCall>(e => e.Id == 1);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDelete(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Delete(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new
                {
                    Id = 1
                });

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateDelete(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateDeleteAll

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForDeleteAll()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.DeleteAll<StatementBuilderEntity>();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDeleteAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.DeleteAll<StatementBuilderEntity>();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateDeleteAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForDeleteAllViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.DeleteAll(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>());

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDeleteAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.DeleteAll(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>());

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateDeleteAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForDeleteAllViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.DeleteAll<StatementBuilderEntityForCrossCall>();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDeleteAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.DeleteAll(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>());

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateDeleteAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateExists

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForExists()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.Exists<StatementBuilderEntity>((object)null);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateExists(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Exists<StatementBuilderEntity>((object)null);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateExists(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForExistsViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.Exists(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                (object)null);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateExists(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Exists(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                (object)null);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateExists(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForExistsViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.Exists<StatementBuilderEntityForCrossCall>((object)null);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateExists(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Exists(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                (object)null);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateExists(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateInsert

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForInsert()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            InsertExecutionContextCache<StatementBuilderEntity>.Flush();
            CommandTextCache.Flush();
            repository.Insert<StatementBuilderEntity>(
                new StatementBuilderEntity
                {
                    Name = "Name"
                });

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Insert<StatementBuilderEntity>(
                new StatementBuilderEntity
                {
                    Name = "Name"
                });

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForInsertViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            InsertExecutionContextCache<object>.Flush();
            CommandTextCache.Flush();
            repository.Insert(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new
                {
                    Name = "Name"
                });

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Insert(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new
                {
                    Name = "Name"
                });

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForInsertViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            InsertExecutionContextCache<StatementBuilderEntityForCrossCall>.Flush();
            InsertExecutionContextCache<object>.Flush();
            CommandTextCache.Flush();
            repository.Insert<StatementBuilderEntityForCrossCall>(
                new StatementBuilderEntityForCrossCall
                {
                    Name = "Name"
                });

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Insert(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new
                {
                    Id = 1,
                    Name = "Name"
                });

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateInsertAll

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForInsertAll()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            InsertAllExecutionContextCache<StatementBuilderEntity>.Flush();
            CommandTextCache.Flush();
            repository.InsertAll<StatementBuilderEntity>(
                new[]
                {
                    new StatementBuilderEntity{ Name = "Name" },
                    new StatementBuilderEntity{ Name = "Name" }
                });

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsertAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.Is<int>(v => v > 1),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.InsertAll<StatementBuilderEntity>(
                new[]
                {
                    new StatementBuilderEntity{ Name = "Name" },
                    new StatementBuilderEntity{ Name = "Name" }
                });

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsertAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.Is<int>(v => v > 1),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForInsertAllWithSizePerBatchEqualsToOne()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            InsertAllExecutionContextCache<StatementBuilderEntity>.Flush();
            CommandTextCache.Flush();
            repository.InsertAll<StatementBuilderEntity>(
                new[]
                {
                    new StatementBuilderEntity{ Name = "Name" },
                    new StatementBuilderEntity{ Name = "Name" }
                },
                batchSize: 1);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.InsertAll<StatementBuilderEntity>(
                new[]
                {
                    new StatementBuilderEntity{ Name = "Name" },
                    new StatementBuilderEntity{ Name = "Name" }
                },
                batchSize: 1);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForInsertAllForTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            InsertAllExecutionContextCache<object>.Flush();
            CommandTextCache.Flush();
            repository.InsertAll(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new[]
                {
                    new StatementBuilderEntityForTableName{ Name = "Name" },
                    new StatementBuilderEntityForTableName{ Name = "Name" }
                },
                fields: new[] { new Field("Id", typeof(int)), new Field("Name", typeof(string)) });

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsertAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.Is<int>(v => v > 1),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.InsertAll(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new[]
                {
                    new StatementBuilderEntityForTableName{ Name = "Name" },
                    new StatementBuilderEntityForTableName{ Name = "Name" }
                },
                fields: new[] { new Field("Id", typeof(int)), new Field("Name", typeof(string)) });

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsertAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.Is<int>(v => v > 1),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForInsertAllWithSizePerBatchEqualsToOneForTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            InsertAllExecutionContextCache<object>.Flush();
            CommandTextCache.Flush();
            repository.InsertAll(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new[]
                {
                    new StatementBuilderEntityForTableName{ Name = "Name" },
                    new StatementBuilderEntityForTableName{ Name = "Name" }
                },
                fields: new[] { new Field("Id", typeof(int)), new Field("Name", typeof(string)) },
                batchSize: 1);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.InsertAll(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new[]
                {
                    new StatementBuilderEntityForTableName{ Name = "Name" },
                    new StatementBuilderEntityForTableName{ Name = "Name" }
                },
                fields: new[] { new Field("Id", typeof(int)), new Field("Name", typeof(string)) },
                batchSize: 1);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForInsertAllViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            InsertAllExecutionContextCache<StatementBuilderEntityForCrossCall>.Flush();
            InsertAllExecutionContextCache<object>.Flush();
            CommandTextCache.Flush();
            repository.InsertAll<StatementBuilderEntityForCrossCall>(
                new[]
                {
                    new StatementBuilderEntityForCrossCall { Name = "Name" },
                    new StatementBuilderEntityForCrossCall { Name = "Name" }
                });

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsertAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.Is<int>(v => v > 1),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.InsertAll(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new[]
                {
                    new StatementBuilderEntityForCrossCall { Name = "Name" },
                    new StatementBuilderEntityForCrossCall { Name = "Name" }
                },
                fields: new[] { new Field("Id", typeof(int)), new Field("Name", typeof(string)) });

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsertAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.Is<int>(v => v > 1),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForInsertAllWithSizePerBatchEqualsToOneViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            InsertAllExecutionContextCache<StatementBuilderEntityForCrossCall>.Flush();
            InsertAllExecutionContextCache<object>.Flush();
            CommandTextCache.Flush();
            repository.InsertAll<StatementBuilderEntityForCrossCall>(
                new[]
                {
                    new StatementBuilderEntityForCrossCall { Name = "Name" },
                    new StatementBuilderEntityForCrossCall { Name = "Name" }
                },
                batchSize: 1);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.InsertAll(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new[]
                {
                    new StatementBuilderEntityForCrossCall { Name = "Name" },
                    new StatementBuilderEntityForCrossCall { Name = "Name" }
                },
                fields: new[] { new Field("Id", typeof(int)), new Field("Name", typeof(string)) },
                batchSize: 1);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateMax

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMax()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.Max<StatementBuilderEntity>(e => e.Id,
                (object)null);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMax(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Max<StatementBuilderEntity>(e => e.Id,
                (object)null);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMax(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMaxViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.Max(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"),
                (object)null);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMax(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Max(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"),
                (object)null);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMax(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMaxViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.Max<StatementBuilderEntityForCrossCall>(e => e.Id,
                (object)null);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMax(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Max(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new Field("Id", typeof(int)),
                (object)null);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMax(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateMaxAll

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMaxAll()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.MaxAll<StatementBuilderEntity>(e => e.Id);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMaxAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.MaxAll<StatementBuilderEntity>(e => e.Id);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMaxAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMaxAllViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.MaxAll(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"));

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMaxAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.MaxAll(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"));

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMaxAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMaxAllViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.MaxAll<StatementBuilderEntityForCrossCall>(e => e.Id);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMaxAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.MaxAll(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new Field("Id", typeof(int)));

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMaxAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateMerge

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMerge()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            MergeExecutionContextCache<StatementBuilderEntity>.Flush();
            CommandTextCache.Flush();
            repository.Merge<StatementBuilderEntity>(
                new StatementBuilderEntity
                {
                    Name = "Name"
                },
                new Field(nameof(StatementBuilderEntity.Id)));

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Merge<StatementBuilderEntity>(
                new StatementBuilderEntity
                {
                    Name = "Name"
                },
                new Field(nameof(StatementBuilderEntity.Id)));

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMergeViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            MergeExecutionContextCache<object>.Flush();
            CommandTextCache.Flush();
            repository.Merge(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new
                {
                    Name = "Name"
                },
                new Field(nameof(StatementBuilderEntityForTableName.Id)));

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Merge(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new
                {
                    Name = "Name"
                },
                new Field(nameof(StatementBuilderEntityForTableName.Id)));

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMergeViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            MergeExecutionContextCache<StatementBuilderEntityForCrossCall>.Flush();
            MergeExecutionContextCache<object>.Flush();
            CommandTextCache.Flush();
            repository.Merge<StatementBuilderEntityForCrossCall>(
                new StatementBuilderEntityForCrossCall
                {
                    Name = "Name"
                },
                new Field(nameof(StatementBuilderEntityForCrossCall.Id)));

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Merge(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new
                {
                    Id = 1,
                    Name = "Name"
                },
                new Field(nameof(StatementBuilderEntityForCrossCall.Id)));

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateMergeAll

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMergeAll()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            MergeAllExecutionContextCache<StatementBuilderEntity>.Flush();
            CommandTextCache.Flush();
            repository.MergeAll<StatementBuilderEntity>(
                new[]
                {
                    new StatementBuilderEntity { Name = "Name1" },
                    new StatementBuilderEntity { Name = "Name2" },
                    new StatementBuilderEntity { Name = "Name3" }
                },
                Field.From(nameof(StatementBuilderEntity.Id)));

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMergeAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            repository.MergeAll<StatementBuilderEntity>(
                new[]
                {
                    new StatementBuilderEntity { Name = "Name1" },
                    new StatementBuilderEntity { Name = "Name2" },
                    new StatementBuilderEntity { Name = "Name3" }
                },
                Field.From(nameof(StatementBuilderEntity.Id)));

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMergeAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMergeAllWithSizePerBatchEqualsToOne()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            MergeAllExecutionContextCache<StatementBuilderEntity>.Flush();
            CommandTextCache.Flush();
            repository.MergeAll<StatementBuilderEntity>(
                new[]
                {
                    new StatementBuilderEntity { Name = "Name1" }
                },
                Field.From(nameof(StatementBuilderEntity.Id)),
                batchSize: 1);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            repository.MergeAll<StatementBuilderEntity>(
                new[]
                {
                    new StatementBuilderEntity { Name = "Name1" }
                },
                Field.From(nameof(StatementBuilderEntity.Id)),
                batchSize: 1);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMergeAllViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            MergeAllExecutionContextCache<object>.Flush();
            CommandTextCache.Flush();
            repository.MergeAll(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new[]
                {
                    new { Name = "Name1" },
                    new { Name = "Name2" },
                    new { Name = "Name3" }
                },
                Field.From(nameof(StatementBuilderEntityForTableName.Id)));

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMergeAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            repository.MergeAll(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new[]
                {
                    new { Name = "Name1" },
                    new { Name = "Name2" },
                    new { Name = "Name3" }
                },
                Field.From(nameof(StatementBuilderEntityForTableName.Id)));

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMergeAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMergeAllWithSizePerBatchEqualsToOneViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            MergeAllExecutionContextCache<object>.Flush();
            CommandTextCache.Flush();
            repository.MergeAll(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new[]
                {
                    new { Name = "Name1" }
                },
                Field.From(nameof(StatementBuilderEntityForTableName.Id)),
                batchSize: 1);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            repository.MergeAll(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new[]
                {
                    new { Name = "Name1" }
                },
                Field.From(nameof(StatementBuilderEntityForTableName.Id)),
                batchSize: 1);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMergeAllViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            MergeAllExecutionContextCache<StatementBuilderEntityForCrossCall>.Flush();
            MergeAllExecutionContextCache<object>.Flush();
            CommandTextCache.Flush();
            repository.MergeAll<StatementBuilderEntityForCrossCall>(
                new[]
                {
                    new StatementBuilderEntityForCrossCall { Name = "Name1" },
                    new StatementBuilderEntityForCrossCall { Name = "Name2" },
                    new StatementBuilderEntityForCrossCall { Name = "Name3" }
                },
                Field.From(nameof(StatementBuilderEntityForCrossCall.Id)));

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMergeAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            repository.MergeAll(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new[]
                {
                    new { Name = "Name1" },
                    new { Name = "Name2" },
                    new { Name = "Name3" }
                },
                Field.From(nameof(StatementBuilderEntityForCrossCall.Id)),
                fields: FieldCache.Get<StatementBuilderEntityForCrossCall>());

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMergeAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMergeAllWithSizePerBatchEqualsToOneViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            MergeAllExecutionContextCache<StatementBuilderEntityForCrossCall>.Flush();
            MergeAllExecutionContextCache<object>.Flush();
            CommandTextCache.Flush();
            repository.MergeAll<StatementBuilderEntityForCrossCall>(
                new[]
                {
                    new StatementBuilderEntityForCrossCall { Name = "Name1" }
                },
                Field.From(nameof(StatementBuilderEntityForCrossCall.Id)),
                batchSize: 1);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            repository.MergeAll(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new[]
                {
                    new { Name = "Name1" }
                },
                Field.From(nameof(StatementBuilderEntityForCrossCall.Id)),
                batchSize: 1,
                fields: FieldCache.Get<StatementBuilderEntityForCrossCall>());

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateMin

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMin()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.Min<StatementBuilderEntity>(e => e.Id,
                (object)null);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMin(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Min<StatementBuilderEntity>(e => e.Id,
                (object)null);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMin(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMinViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.Min(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"),
                (object)null);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMin(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Min(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"),
                (object)null);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMin(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMinViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.Min<StatementBuilderEntityForCrossCall>(e => e.Id,
                (object)null);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMin(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Min(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new Field("Id", typeof(int)),
                (object)null);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMin(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateMinAll

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMinAll()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.MinAll<StatementBuilderEntity>(e => e.Id);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMinAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.MinAll<StatementBuilderEntity>(e => e.Id);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMinAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMinAllViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.MinAll(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"));

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMinAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.MinAll(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"));

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMinAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMinAllViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.MinAll<StatementBuilderEntityForCrossCall>(e => e.Id);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMinAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.MinAll(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new Field("Id", typeof(int)));

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMinAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateQuery

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForQuery()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.Query<StatementBuilderEntity>(e => e.Id == 1);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            repository.Query<StatementBuilderEntity>(e => e.Id == 1);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForQueryForTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.Query(ClassMappedNameCache.Get<StatementBuilderEntity>(),
                new { Id = 1 });

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            repository.Query(ClassMappedNameCache.Get<StatementBuilderEntity>(),
                new { Id = 1 });

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForQueryViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.Query<StatementBuilderEntity>(e => e.Id == 1);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            repository.Query(ClassMappedNameCache.Get<StatementBuilderEntity>(),
                new { Id = 1 });

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateQueryAll

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForQueryAll()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.QueryAll<StatementBuilderEntity>();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateQueryAll(
                    It.IsAny<QueryBuilder>(),
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            repository.QueryAll<StatementBuilderEntity>();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateQueryAll(
                    It.IsAny<QueryBuilder>(),
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForQueryAllForTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.QueryAll(ClassMappedNameCache.Get<StatementBuilderEntity>());

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateQueryAll(
                    It.IsAny<QueryBuilder>(),
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            repository.QueryAll(ClassMappedNameCache.Get<StatementBuilderEntity>());

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateQueryAll(
                    It.IsAny<QueryBuilder>(),
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForQueryAllViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.QueryAll<StatementBuilderEntity>();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateQueryAll(
                    It.IsAny<QueryBuilder>(),
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            repository.QueryAll(ClassMappedNameCache.Get<StatementBuilderEntity>());

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateQueryAll(
                    It.IsAny<QueryBuilder>(),
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateQuery(Multple)

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForQueryMultiple()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.QueryMultiple<StatementBuilderEntityT1,
                StatementBuilderEntityT2,
                StatementBuilderEntityT3,
                StatementBuilderEntityT4,
                StatementBuilderEntityT5,
                StatementBuilderEntityT6,
                StatementBuilderEntityT7>(e => e.Id == 1,
                e => e.Id == 1,
                e => e.Id == 1,
                e => e.Id == 1,
                e => e.Id == 1,
                e => e.Id == 1,
                e => e.Id == 1);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT1>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT2>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT3>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT4>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT5>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT6>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT7>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.QueryMultiple<StatementBuilderEntityT1,
                StatementBuilderEntityT2,
                StatementBuilderEntityT3,
                StatementBuilderEntityT4,
                StatementBuilderEntityT5,
                StatementBuilderEntityT6,
                StatementBuilderEntityT7>(e => e.Id == 1,
                e => e.Id == 1,
                e => e.Id == 1,
                e => e.Id == 1,
                e => e.Id == 1,
                e => e.Id == 1,
                e => e.Id == 1);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT1>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT2>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT3>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT4>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT5>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT6>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT7>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateSum

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForSum()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.Sum<StatementBuilderEntity>(e => e.Id,
                (object)null);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateSum(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Sum<StatementBuilderEntity>(e => e.Id,
                (object)null);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateSum(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForSumViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.Sum(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"),
                (object)null);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateSum(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Sum(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"),
                (object)null);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateSum(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForSumViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.Sum<StatementBuilderEntityForCrossCall>(e => e.Id,
                (object)null);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateSum(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Sum(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new Field("Id", typeof(int)),
                (object)null);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateSum(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateSumAll

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForSumAll()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.SumAll<StatementBuilderEntity>(e => e.Id);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateSumAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.SumAll<StatementBuilderEntity>(e => e.Id);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateSumAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForSumAllViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.SumAll(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"));

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateSumAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.SumAll(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"));

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateSumAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForSumAllViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.SumAll<StatementBuilderEntityForCrossCall>(e => e.Id);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateSumAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.SumAll(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new Field("Id", typeof(int)));

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateSumAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateTruncate

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForTruncate()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.Truncate<StatementBuilderEntity>();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateTruncate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>())), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Truncate<StatementBuilderEntity>();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateTruncate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>())), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForTruncateViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.Truncate(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>());

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateTruncate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>())), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Truncate(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>());

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateTruncate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>())), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForTruncateViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.Truncate<StatementBuilderEntityForCrossCall>();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateTruncate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>())), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Truncate(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>());

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateTruncate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>())), Times.Exactly(0));
        }

        #endregion

        #region CreateUpdate

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForUpdate()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            UpdateExecutionContextCache<StatementBuilderEntity>.Flush();
            CommandTextCache.Flush();
            repository.Update<StatementBuilderEntity>(
                new StatementBuilderEntity
                {
                    Name = "Update"
                },
                e => e.Id == 1);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateUpdate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Update<StatementBuilderEntity>(
                new StatementBuilderEntity
                {
                    Name = "Update"
                },
                e => e.Id == 1);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateUpdate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForUpdateTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            UpdateExecutionContextCache<object>.Flush();
            CommandTextCache.Flush();
            repository.Update(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new StatementBuilderEntityForTableName
                {
                    Name = "Update"
                },
                new { Id = 1 });

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateUpdate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Update(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new StatementBuilderEntityForTableName
                {
                    Name = "Update"
                },
                new { Id = 1 });

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateUpdate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForUpdateViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            UpdateExecutionContextCache<StatementBuilderEntityForCrossCall>.Flush();
            UpdateExecutionContextCache<object>.Flush();
            CommandTextCache.Flush();
            repository.Update<StatementBuilderEntityForCrossCall>(
                new StatementBuilderEntityForCrossCall
                {
                    Name = "Update"
                },
                e => e.Id == 1);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateUpdate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Update(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new StatementBuilderEntityForCrossCall
                {
                    Id = 1,
                    Name = "Update"
                },
                new { Id = 1 });

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateUpdate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateUpdateAll

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForUpdateAll()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            UpdateAllExecutionContextCache<StatementBuilderEntity>.Flush();
            CommandTextCache.Flush();
            repository.UpdateAll<StatementBuilderEntity>(
                new[]
                {
                    new StatementBuilderEntity { Name = "Name1" },
                    new StatementBuilderEntity { Name = "Name2" },
                    new StatementBuilderEntity { Name = "Name3" }
                },
                Field.From(nameof(StatementBuilderEntity.Id)));

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateUpdateAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            repository.UpdateAll<StatementBuilderEntity>(
                new[]
                {
                    new StatementBuilderEntity { Name = "Name1" },
                    new StatementBuilderEntity { Name = "Name2" },
                    new StatementBuilderEntity { Name = "Name3" }
                },
                Field.From(nameof(StatementBuilderEntity.Id)));

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateUpdateAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForUpdateAllViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            UpdateAllExecutionContextCache<object>.Flush();
            CommandTextCache.Flush();
            repository.UpdateAll(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new[]
                {
                    new { Name = "Name1" },
                    new { Name = "Name2" },
                    new { Name = "Name3" }
                },
                Field.From(nameof(StatementBuilderEntityForTableName.Id)));

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateUpdateAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            repository.UpdateAll(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new[]
                {
                    new { Name = "Name1" },
                    new { Name = "Name2" },
                    new { Name = "Name3" }
                },
                Field.From(nameof(StatementBuilderEntityForTableName.Id)));

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateUpdateAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForUpdateAllViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            UpdateAllExecutionContextCache<StatementBuilderEntityForCrossCall>.Flush();
            UpdateAllExecutionContextCache<object>.Flush();
            CommandTextCache.Flush();
            repository.UpdateAll<StatementBuilderEntityForCrossCall>(
                new[]
                {
                    new StatementBuilderEntityForCrossCall { Name = "Name1" },
                    new StatementBuilderEntityForCrossCall { Name = "Name2" },
                    new StatementBuilderEntityForCrossCall { Name = "Name3" }
                },
                Field.From(nameof(StatementBuilderEntityForCrossCall.Id)));

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateUpdateAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            repository.UpdateAll(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new[]
                {
                    new { Name = "Name1" },
                    new { Name = "Name2" },
                    new { Name = "Name3" }
                },
                Field.From(nameof(StatementBuilderEntityForCrossCall.Id)),
                fields: FieldCache.Get<StatementBuilderEntityForCrossCall>());

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateUpdateAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #endregion

        #region Async

        #region CreateAverage

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForAverageAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.AverageAsync<StatementBuilderEntity>(e => e.Id,
                (object)null).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateAverage(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.AverageAsync<StatementBuilderEntity>(e => e.Id,
                (object)null).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateAverage(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForAverageAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.AverageAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"),
                (object)null).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateAverage(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.AverageAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"),
                (object)null).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateAverage(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForAverageAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.AverageAsync<StatementBuilderEntityForCrossCall>(e => e.Id,
                (object)null).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateAverage(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.AverageAsync(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new Field("Id", typeof(int)),
                (object)null).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateAverage(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateAverageAll

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForAverageAllAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.AverageAllAsync<StatementBuilderEntity>(e => e.Id).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateAverageAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.AverageAllAsync<StatementBuilderEntity>(e => e.Id).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateAverageAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForAverageAllAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.AverageAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id")).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateAverageAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.AverageAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id")).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateAverageAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForAverageAllAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.AverageAllAsync<StatementBuilderEntityForCrossCall>(e => e.Id).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateAverageAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.AverageAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new Field("Id", typeof(int))).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateAverageAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateBatchQueryAsync

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForBatchQueryAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.BatchQueryAsync<StatementBuilderEntity>(0,
                10,
                null,
                (object)null).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateBatchQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.BatchQueryAsync<StatementBuilderEntity>(0,
                10,
                null,
                (object)null).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateBatchQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateCountAsync

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForCountAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.CountAsync<StatementBuilderEntity>((object)null).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateCount(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.CountAsync<StatementBuilderEntity>((object)null).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateCount(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForCountAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.CountAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                (object)null).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateCount(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.CountAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                (object)null).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateCount(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForCountAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.CountAsync<StatementBuilderEntityForCrossCall>((object)null).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateCount(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.CountAsync(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                (object)null).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateCount(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateCountAllAsync

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForCountAllAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.CountAllAsync<StatementBuilderEntity>().Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateCountAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.CountAllAsync<StatementBuilderEntity>().Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateCountAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForCountAllAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.CountAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateCountAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.CountAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateCountAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForCountAllAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.CountAllAsync<StatementBuilderEntityForCrossCall>().Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateCountAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.CountAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateCountAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateDeleteAsync

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForDeleteAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.DeleteAsync<StatementBuilderEntity>(e => e.Id == 1).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDelete(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.DeleteAsync<StatementBuilderEntity>(e => e.Id == 1).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateDelete(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForDeleteAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.DeleteAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new
                {
                    Id = 1
                }).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDelete(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.DeleteAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new
                {
                    Id = 1
                }).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateDelete(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForDeleteAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.DeleteAsync<StatementBuilderEntityForCrossCall>(e => e.Id == 1).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDelete(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.DeleteAsync(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new
                {
                    Id = 1
                }).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateDelete(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateDeleteAllAsync

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForDeleteAllAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.DeleteAllAsync<StatementBuilderEntity>().Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDeleteAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.DeleteAllAsync<StatementBuilderEntity>().Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateDeleteAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForDeleteAllAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.DeleteAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDeleteAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.DeleteAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateDeleteAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForDeleteAllAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.DeleteAllAsync<StatementBuilderEntityForCrossCall>().Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDeleteAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.DeleteAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateDeleteAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateExistsAsync

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForExistsAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.ExistsAsync<StatementBuilderEntity>((object)null).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateExists(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.ExistsAsync<StatementBuilderEntity>((object)null).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateExists(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForExistsAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.ExistsAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                (object)null).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateExists(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.ExistsAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                (object)null).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateExists(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForExistsAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.ExistsAsync<StatementBuilderEntityForCrossCall>((object)null).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateExists(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.ExistsAsync(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                (object)null).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateExists(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateInsertAsync

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForInsertAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            InsertExecutionContextCache<StatementBuilderEntity>.Flush();
            CommandTextCache.Flush();
            repository.InsertAsync<StatementBuilderEntity>(
                new StatementBuilderEntity
                {
                    Name = "Name"
                }).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.InsertAsync<StatementBuilderEntity>(
                new StatementBuilderEntity
                {
                    Name = "Name"
                }).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForInsertAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            InsertExecutionContextCache<object>.Flush();
            CommandTextCache.Flush();
            repository.InsertAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new
                {
                    Name = "Name"
                }).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.InsertAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new
                {
                    Name = "Name"
                }).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForInsertAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            InsertExecutionContextCache<StatementBuilderEntityForCrossCall>.Flush();
            InsertExecutionContextCache<object>.Flush();
            CommandTextCache.Flush();
            repository.InsertAsync<StatementBuilderEntityForCrossCall>(
                new StatementBuilderEntityForCrossCall
                {
                    Name = "Name"
                }).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.InsertAsync(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new
                {
                    Id = 1,
                    Name = "Name"
                }).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateInsertAllAsync

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForInsertAllAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            InsertAllExecutionContextCache<StatementBuilderEntity>.Flush();
            CommandTextCache.Flush();
            repository.InsertAllAsync<StatementBuilderEntity>(
                new[]
                {
                    new StatementBuilderEntity{ Name = "Name" },
                    new StatementBuilderEntity{ Name = "Name" }
                }).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsertAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.Is<int>(v => v > 1),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.InsertAllAsync<StatementBuilderEntity>(
                new[]
                {
                    new StatementBuilderEntity{ Name = "Name" },
                    new StatementBuilderEntity{ Name = "Name" }
                }).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsertAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.Is<int>(v => v > 1),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForInsertAllAsyncWithSizePerBatchEqualsToOne()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            InsertAllExecutionContextCache<StatementBuilderEntity>.Flush();
            CommandTextCache.Flush();
            repository.InsertAllAsync<StatementBuilderEntity>(
                new[]
                {
                    new StatementBuilderEntity{ Name = "Name" },
                    new StatementBuilderEntity{ Name = "Name" }
                },
                batchSize: 1).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.InsertAllAsync<StatementBuilderEntity>(
                new[]
                {
                    new StatementBuilderEntity{ Name = "Name" },
                    new StatementBuilderEntity{ Name = "Name" }
                },
                batchSize: 1).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForInsertAllAsyncForTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            InsertAllExecutionContextCache<object>.Flush();
            CommandTextCache.Flush();
            repository.InsertAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new[]
                {
                    new StatementBuilderEntityForTableName{ Name = "Name" },
                    new StatementBuilderEntityForTableName{ Name = "Name" }
                },
                fields: new[] { new Field("Id", typeof(int)), new Field("Name", typeof(string)) }).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsertAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.Is<int>(v => v > 1),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.InsertAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new[]
                {
                    new StatementBuilderEntityForTableName{ Name = "Name" },
                    new StatementBuilderEntityForTableName{ Name = "Name" }
                },
                fields: new[] { new Field("Id", typeof(int)), new Field("Name", typeof(string)) }).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsertAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.Is<int>(v => v > 1),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForInsertAllAsyncWithSizePerBatchEqualsToOneForTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            InsertAllExecutionContextCache<object>.Flush();
            CommandTextCache.Flush();
            repository.InsertAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new[]
                {
                    new StatementBuilderEntityForTableName{ Name = "Name" },
                    new StatementBuilderEntityForTableName{ Name = "Name" }
                },
                fields: new[] { new Field("Id", typeof(int)), new Field("Name", typeof(string)) },
                batchSize: 1).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.InsertAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new[]
                {
                    new StatementBuilderEntityForTableName{ Name = "Name" },
                    new StatementBuilderEntityForTableName{ Name = "Name" }
                },
                fields: new[] { new Field("Id", typeof(int)), new Field("Name", typeof(string)) },
                batchSize: 1).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForInsertAllAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            InsertAllExecutionContextCache<StatementBuilderEntityForCrossCall>.Flush();
            InsertAllExecutionContextCache<object>.Flush();
            CommandTextCache.Flush();
            repository.InsertAllAsync<StatementBuilderEntityForCrossCall>(
                new[]
                {
                    new StatementBuilderEntityForCrossCall { Name = "Name" },
                    new StatementBuilderEntityForCrossCall { Name = "Name" }
                }).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsertAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.Is<int>(v => v > 1),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.InsertAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new[]
                {
                    new StatementBuilderEntityForCrossCall { Name = "Name" },
                    new StatementBuilderEntityForCrossCall { Name = "Name" }
                },
                fields: new[] { new Field("Id", typeof(int)), new Field("Name", typeof(string)) }).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsertAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.Is<int>(v => v > 1),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForInsertAllAsyncWithSizePerBatchEqualsToOneViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            InsertAllExecutionContextCache<StatementBuilderEntityForCrossCall>.Flush();
            InsertAllExecutionContextCache<object>.Flush();
            CommandTextCache.Flush();
            repository.InsertAllAsync<StatementBuilderEntityForCrossCall>(
                new[]
                {
                    new StatementBuilderEntityForCrossCall { Name = "Name" },
                    new StatementBuilderEntityForCrossCall { Name = "Name" }
                },
                batchSize: 1).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.InsertAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new[]
                {
                    new StatementBuilderEntityForCrossCall { Name = "Name" },
                    new StatementBuilderEntityForCrossCall { Name = "Name" }
                },
                fields: new[] { new Field("Id", typeof(int)), new Field("Name", typeof(string)) },
                batchSize: 1).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateMax

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMaxAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.MaxAsync<StatementBuilderEntity>(e => e.Id,
                (object)null).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMax(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.MaxAsync<StatementBuilderEntity>(e => e.Id,
                (object)null).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMax(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMaxAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.MaxAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"),
                (object)null).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMax(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.MaxAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"),
                (object)null).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMax(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMaxAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.MaxAsync<StatementBuilderEntityForCrossCall>(e => e.Id,
                (object)null).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMax(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.MaxAsync(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new Field("Id", typeof(int)),
                (object)null).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMax(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateMaxAll

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMaxAllAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.MaxAllAsync<StatementBuilderEntity>(e => e.Id).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMaxAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.MaxAllAsync<StatementBuilderEntity>(e => e.Id).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMaxAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMaxAllAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.MaxAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id")).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMaxAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.MaxAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id")).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMaxAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMaxAllAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.MaxAllAsync<StatementBuilderEntityForCrossCall>(e => e.Id).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMaxAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.MaxAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new Field("Id", typeof(int))).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMaxAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateMergeAsync

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMergeAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            MergeExecutionContextCache<StatementBuilderEntity>.Flush();
            CommandTextCache.Flush();
            repository.MergeAsync<StatementBuilderEntity>(
                new StatementBuilderEntity
                {
                    Name = "Name"
                },
                new Field(nameof(StatementBuilderEntity.Id))).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.MergeAsync<StatementBuilderEntity>(
                new StatementBuilderEntity
                {
                    Name = "Name"
                },
                new Field(nameof(StatementBuilderEntity.Id))).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMergeAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            MergeExecutionContextCache<object>.Flush();
            CommandTextCache.Flush();
            repository.MergeAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new
                {
                    Name = "Name"
                },
                new Field(nameof(StatementBuilderEntityForTableName.Id))).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.MergeAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new
                {
                    Name = "Name"
                },
                new Field(nameof(StatementBuilderEntityForTableName.Id))).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMergeAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            MergeExecutionContextCache<StatementBuilderEntityForCrossCall>.Flush();
            MergeExecutionContextCache<object>.Flush();
            CommandTextCache.Flush();
            repository.MergeAsync<StatementBuilderEntityForCrossCall>(
                new StatementBuilderEntityForCrossCall
                {
                    Name = "Name"
                },
                new Field(nameof(StatementBuilderEntityForCrossCall.Id))).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.MergeAsync(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new
                {
                    Id = 1,
                    Name = "Name"
                },
                new Field(nameof(StatementBuilderEntityForCrossCall.Id))).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateMergeAllAsync

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMergeAllAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            MergeAllExecutionContextCache<StatementBuilderEntity>.Flush();
            CommandTextCache.Flush();
            repository.MergeAllAsync<StatementBuilderEntity>(
                new[]
                {
                    new StatementBuilderEntity { Name = "Name1" },
                    new StatementBuilderEntity { Name = "Name2" },
                    new StatementBuilderEntity { Name = "Name3" }
                },
                Field.From(nameof(StatementBuilderEntity.Id))).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMergeAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            repository.MergeAllAsync<StatementBuilderEntity>(
                new[]
                {
                    new StatementBuilderEntity { Name = "Name1" },
                    new StatementBuilderEntity { Name = "Name2" },
                    new StatementBuilderEntity { Name = "Name3" }
                },
                Field.From(nameof(StatementBuilderEntity.Id))).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMergeAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMergeAllAsyncWithSizePerBatchEqualsToOne()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            MergeAllExecutionContextCache<StatementBuilderEntity>.Flush();
            CommandTextCache.Flush();
            repository.MergeAllAsync<StatementBuilderEntity>(
                new[]
                {
                    new StatementBuilderEntity { Name = "Name1" }
                },
                Field.From(nameof(StatementBuilderEntity.Id)),
                batchSize: 1).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            repository.MergeAllAsync<StatementBuilderEntity>(
                new[]
                {
                    new StatementBuilderEntity { Name = "Name1" }
                },
                Field.From(nameof(StatementBuilderEntity.Id)),
                batchSize: 1).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMergeAllAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            MergeAllExecutionContextCache<object>.Flush();
            CommandTextCache.Flush();
            repository.MergeAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new[]
                {
                    new { Name = "Name1" },
                    new { Name = "Name2" },
                    new { Name = "Name3" }
                },
                Field.From(nameof(StatementBuilderEntityForTableName.Id))).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMergeAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            repository.MergeAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new[]
                {
                    new { Name = "Name1" },
                    new { Name = "Name2" },
                    new { Name = "Name3" }
                },
                Field.From(nameof(StatementBuilderEntityForTableName.Id))).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMergeAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMergeAllAsyncWithSizePerBatchEqualsToOneViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            MergeAllExecutionContextCache<object>.Flush();
            CommandTextCache.Flush();
            repository.MergeAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new[]
                {
                    new { Name = "Name1" }
                },
                Field.From(nameof(StatementBuilderEntityForTableName.Id)),
                batchSize: 1).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            repository.MergeAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new[]
                {
                    new { Name = "Name1" }
                },
                Field.From(nameof(StatementBuilderEntityForTableName.Id)),
                batchSize: 1).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMergeAllAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            MergeAllExecutionContextCache<StatementBuilderEntityForCrossCall>.Flush();
            MergeAllExecutionContextCache<object>.Flush();
            CommandTextCache.Flush();
            repository.MergeAllAsync<StatementBuilderEntityForCrossCall>(
                new[]
                {
                    new StatementBuilderEntityForCrossCall { Name = "Name1" },
                    new StatementBuilderEntityForCrossCall { Name = "Name2" },
                    new StatementBuilderEntityForCrossCall { Name = "Name3" }
                },
                Field.From(nameof(StatementBuilderEntityForCrossCall.Id))).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMergeAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            repository.MergeAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new[]
                {
                    new { Name = "Name1" },
                    new { Name = "Name2" },
                    new { Name = "Name3" }
                },
                Field.From(nameof(StatementBuilderEntityForCrossCall.Id)),
                fields: FieldCache.Get<StatementBuilderEntityForCrossCall>()).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMergeAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMergeAllAsyncWithSizePerBatchEqualsToOneViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            MergeAllExecutionContextCache<StatementBuilderEntityForCrossCall>.Flush();
            MergeAllExecutionContextCache<object>.Flush();
            CommandTextCache.Flush();
            repository.MergeAllAsync<StatementBuilderEntityForCrossCall>(
                new[]
                {
                    new StatementBuilderEntityForCrossCall { Name = "Name1" }
                },
                Field.From(nameof(StatementBuilderEntityForCrossCall.Id)),
                batchSize: 1).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            repository.MergeAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new[]
                {
                    new { Name = "Name1" }
                },
                Field.From(nameof(StatementBuilderEntityForCrossCall.Id)),
                batchSize: 1,
                fields: FieldCache.Get<StatementBuilderEntityForCrossCall>()).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateMin

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMinAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.MinAsync<StatementBuilderEntity>(e => e.Id,
                (object)null).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMin(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.MinAsync<StatementBuilderEntity>(e => e.Id,
                (object)null).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMin(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMinAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.MinAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"),
                (object)null).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMin(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.MinAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"),
                (object)null).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMin(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMinAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.MinAsync<StatementBuilderEntityForCrossCall>(e => e.Id,
                (object)null).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMin(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.MinAsync(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new Field("Id", typeof(int)),
                (object)null).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMin(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateMinAll

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMinAllAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.MinAllAsync<StatementBuilderEntity>(e => e.Id).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMinAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.MinAllAsync<StatementBuilderEntity>(e => e.Id).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMinAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMinAllAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.MinAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id")).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMinAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.MinAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id")).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMinAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMinAllAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.MinAllAsync<StatementBuilderEntityForCrossCall>(e => e.Id).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMinAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.MinAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new Field("Id", typeof(int))).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMinAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateQueryAsync

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForQueryAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.QueryAsync<StatementBuilderEntity>(e => e.Id == 1).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            repository.QueryAsync<StatementBuilderEntity>(e => e.Id == 1).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForQueryAsyncForTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.QueryAsync(ClassMappedNameCache.Get<StatementBuilderEntity>(),
                new { Id = 1 }).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            repository.QueryAsync(ClassMappedNameCache.Get<StatementBuilderEntity>(),
                new { Id = 1 }).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForQueryAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.QueryAsync<StatementBuilderEntity>(e => e.Id == 1).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            repository.QueryAsync(ClassMappedNameCache.Get<StatementBuilderEntity>(),
                new { Id = 1 }).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateQueryAllAsync

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForQueryAllAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.QueryAllAsync<StatementBuilderEntity>().Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateQueryAll(
                    It.IsAny<QueryBuilder>(),
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            repository.QueryAllAsync<StatementBuilderEntity>().Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateQueryAll(
                    It.IsAny<QueryBuilder>(),
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForQueryAllAsyncForTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.QueryAllAsync(ClassMappedNameCache.Get<StatementBuilderEntity>()).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateQueryAll(
                    It.IsAny<QueryBuilder>(),
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            repository.QueryAllAsync(ClassMappedNameCache.Get<StatementBuilderEntity>()).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateQueryAll(
                    It.IsAny<QueryBuilder>(),
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForQueryAllAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.QueryAllAsync<StatementBuilderEntity>().Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateQueryAll(
                    It.IsAny<QueryBuilder>(),
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            repository.QueryAllAsync(ClassMappedNameCache.Get<StatementBuilderEntity>()).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateQueryAll(
                    It.IsAny<QueryBuilder>(),
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateQueryAsync(Multple)

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForQueryMultipleAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.QueryMultipleAsync<StatementBuilderEntityT1,
                StatementBuilderEntityT2,
                StatementBuilderEntityT3,
                StatementBuilderEntityT4,
                StatementBuilderEntityT5,
                StatementBuilderEntityT6,
                StatementBuilderEntityT7>(e => e.Id == 1,
                e => e.Id == 1,
                e => e.Id == 1,
                e => e.Id == 1,
                e => e.Id == 1,
                e => e.Id == 1,
                e => e.Id == 1).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT1>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT2>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT3>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT4>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT5>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT6>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT7>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.QueryMultipleAsync<StatementBuilderEntityT1,
                StatementBuilderEntityT2,
                StatementBuilderEntityT3,
                StatementBuilderEntityT4,
                StatementBuilderEntityT5,
                StatementBuilderEntityT6,
                StatementBuilderEntityT7>(e => e.Id == 1,
                e => e.Id == 1,
                e => e.Id == 1,
                e => e.Id == 1,
                e => e.Id == 1,
                e => e.Id == 1,
                e => e.Id == 1).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT1>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT2>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT3>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT4>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT5>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT6>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT7>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateSum

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForSumAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.SumAsync<StatementBuilderEntity>(e => e.Id,
                (object)null).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateSum(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.SumAsync<StatementBuilderEntity>(e => e.Id,
                (object)null).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateSum(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForSumAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.SumAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"),
                (object)null).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateSum(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.SumAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"),
                (object)null).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateSum(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForSumAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.SumAsync<StatementBuilderEntityForCrossCall>(e => e.Id,
                (object)null).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateSum(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.SumAsync(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new Field("Id", typeof(int)),
                (object)null).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateSum(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateSumAll

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForSumAllAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.SumAllAsync<StatementBuilderEntity>(e => e.Id).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateSumAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.SumAllAsync<StatementBuilderEntity>(e => e.Id).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateSumAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForSumAllAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.SumAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id")).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateSumAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.SumAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id")).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateSumAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForSumAllAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.SumAllAsync<StatementBuilderEntityForCrossCall>(e => e.Id).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateSumAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.SumAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new Field("Id", typeof(int))).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateSumAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateTruncate

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForTruncateAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.TruncateAsync<StatementBuilderEntity>().Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateTruncate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>())), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.TruncateAsync<StatementBuilderEntity>().Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateTruncate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>())), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForTruncateAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.TruncateAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateTruncate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>())), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.TruncateAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateTruncate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>())), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForTruncateAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.TruncateAsync<StatementBuilderEntityForCrossCall>().Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateTruncate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>())), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.TruncateAsync(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateTruncate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>())), Times.Exactly(0));
        }

        #endregion

        #region CreateUpdateAsync

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForUpdateAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            UpdateExecutionContextCache<StatementBuilderEntity>.Flush();
            CommandTextCache.Flush();
            repository.UpdateAsync<StatementBuilderEntity>(
                new StatementBuilderEntity
                {
                    Name = "Update"
                },
                e => e.Id == 1).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateUpdate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.UpdateAsync<StatementBuilderEntity>(
                new StatementBuilderEntity
                {
                    Name = "Update"
                },
                e => e.Id == 1).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateUpdate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForUpdateAsyncTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            UpdateExecutionContextCache<object>.Flush();
            CommandTextCache.Flush();
            repository.UpdateAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new StatementBuilderEntityForTableName
                {
                    Name = "Update"
                },
                new { Id = 1 }).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateUpdate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.UpdateAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new StatementBuilderEntityForTableName
                {
                    Name = "Update"
                },
                new { Id = 1 }).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateUpdate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForUpdateAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            UpdateExecutionContextCache<StatementBuilderEntityForCrossCall>.Flush();
            UpdateExecutionContextCache<object>.Flush();
            CommandTextCache.Flush();
            repository.UpdateAsync<StatementBuilderEntityForCrossCall>(
                new StatementBuilderEntityForCrossCall
                {
                    Name = "Update"
                },
                e => e.Id == 1).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateUpdate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.UpdateAsync(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new StatementBuilderEntityForCrossCall
                {
                    Id = 1,
                    Name = "Update"
                },
                new { Id = 1 }).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateUpdate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateUpdateAllAsync

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForUpdateAllAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            UpdateAllExecutionContextCache<StatementBuilderEntity>.Flush();
            CommandTextCache.Flush();
            repository.UpdateAllAsync<StatementBuilderEntity>(
                new[]
                {
                    new StatementBuilderEntity { Name = "Name1" },
                    new StatementBuilderEntity { Name = "Name2" },
                    new StatementBuilderEntity { Name = "Name3" }
                },
                Field.From(nameof(StatementBuilderEntity.Id))).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateUpdateAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            repository.UpdateAllAsync<StatementBuilderEntity>(
                new[]
                {
                    new StatementBuilderEntity { Name = "Name1" },
                    new StatementBuilderEntity { Name = "Name2" },
                    new StatementBuilderEntity { Name = "Name3" }
                },
                Field.From(nameof(StatementBuilderEntity.Id))).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateUpdateAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForUpdateAllAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            UpdateAllExecutionContextCache<object>.Flush();
            CommandTextCache.Flush();
            repository.UpdateAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new[]
                {
                    new { Name = "Name1" },
                    new { Name = "Name2" },
                    new { Name = "Name3" }
                },
                Field.From(nameof(StatementBuilderEntityForTableName.Id))).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateUpdateAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            repository.UpdateAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new[]
                {
                    new { Name = "Name1" },
                    new { Name = "Name2" },
                    new { Name = "Name3" }
                },
                Field.From(nameof(StatementBuilderEntityForTableName.Id))).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateUpdateAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForUpdateAllAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<StatementBuilderDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            UpdateAllExecutionContextCache<StatementBuilderEntityForCrossCall>.Flush();
            UpdateAllExecutionContextCache<object>.Flush();
            CommandTextCache.Flush();
            repository.UpdateAllAsync<StatementBuilderEntityForCrossCall>(
                new[]
                {
                    new StatementBuilderEntityForCrossCall { Name = "Name1" },
                    new StatementBuilderEntityForCrossCall { Name = "Name2" },
                    new StatementBuilderEntityForCrossCall { Name = "Name3" }
                },
                Field.From(nameof(StatementBuilderEntityForCrossCall.Id))).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateUpdateAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            repository.UpdateAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new[]
                {
                    new { Name = "Name1" },
                    new { Name = "Name2" },
                    new { Name = "Name3" }
                },
                Field.From(nameof(StatementBuilderEntityForCrossCall.Id)),
                fields: FieldCache.Get<StatementBuilderEntityForCrossCall>()).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateUpdateAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #endregion
    }
}
