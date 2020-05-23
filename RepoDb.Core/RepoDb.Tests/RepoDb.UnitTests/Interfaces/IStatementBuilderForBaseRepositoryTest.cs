using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoDb.Contexts.Execution;
using RepoDb.Interfaces;
using RepoDb.UnitTests.CustomObjects;
using System.Collections.Generic;

namespace RepoDb.UnitTests.Interfaces
{
    [TestClass]
    public class IStatementBuilderForBaseRepositoryTest
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

        private class StatementBuilderEntityRepository : BaseRepository<StatementBuilderEntity, StatementBuilderDbConnection>
        {
            public StatementBuilderEntityRepository(IStatementBuilder statementBuilder)
                : base("Connection", statementBuilder) { }
        }

        #endregion

        #region Sync

        #region CreateAverage

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForAverage()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new StatementBuilderEntityRepository(statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.Average(e => e.Id,
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
            var repositoryNever = new StatementBuilderEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.Average(e => e.Id,
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

        #endregion

        #region CreateAverageAll

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForAverageAll()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new StatementBuilderEntityRepository(statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.AverageAll(e => e.Id);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateAverageAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new StatementBuilderEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.AverageAll(e => e.Id);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateAverageAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateBatchQuery

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForBatchQuery()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new StatementBuilderEntityRepository(statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.BatchQuery(0,
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
            var repositoryNever = new StatementBuilderEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.BatchQuery(0,
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
        public void TestBaseRepositoryStatementBuilderForCount()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new StatementBuilderEntityRepository(statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.Count((object)null);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateCount(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new StatementBuilderEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.Count((object)null);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateCount(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateCountAll

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForCountAll()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new StatementBuilderEntityRepository(statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.CountAll();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateCountAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new StatementBuilderEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.CountAll();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateCountAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateDelete

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForDelete()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new StatementBuilderEntityRepository(statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.Delete(e => e.Id == 1);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDelete(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new StatementBuilderEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.Delete(e => e.Id == 1);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateDelete(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateDeleteAll

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForDeleteAll()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new StatementBuilderEntityRepository(statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.DeleteAll();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDeleteAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new StatementBuilderEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.DeleteAll();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateDeleteAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateExists

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForExists()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new StatementBuilderEntityRepository(statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.Exists((object)null);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateExists(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new StatementBuilderEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.Exists((object)null);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateExists(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateInsert

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForInsert()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new StatementBuilderEntityRepository(statementBuilder.Object);

            // Act
            InsertExecutionContextCache<StatementBuilderEntity>.Flush();
            CommandTextCache.Flush();
            repository.Insert(
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
            var repositoryNever = new StatementBuilderEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.Insert(new StatementBuilderEntity
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

        #endregion

        #region CreateInsertAll

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForInsertAll()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new StatementBuilderEntityRepository(statementBuilder.Object);

            // Act
            InsertAllExecutionContextCache<StatementBuilderEntity>.Flush();
            CommandTextCache.Flush();
            repository.InsertAll(
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
            var repositoryNever = new StatementBuilderEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.InsertAll(
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
        public void TestBaseRepositoryStatementBuilderForInsertAllWithSizePerBatchEqualsToOne()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new StatementBuilderEntityRepository(statementBuilder.Object);

            // Act
            InsertAllExecutionContextCache<StatementBuilderEntity>.Flush();
            CommandTextCache.Flush();
            repository.InsertAll(
                new[]
                {
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
            var repositoryNever = new StatementBuilderEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.InsertAll(
                new[]
                {
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

        #endregion

        #region CreateMax

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForMax()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new StatementBuilderEntityRepository(statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.Max(e => e.Id,
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
            var repositoryNever = new StatementBuilderEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.Max(e => e.Id,
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

        #endregion

        #region CreateMaxAll

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForMaxAll()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new StatementBuilderEntityRepository(statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.MaxAll(e => e.Id);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMaxAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new StatementBuilderEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.MaxAll(e => e.Id);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMaxAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateMerge

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForMerge()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new StatementBuilderEntityRepository(statementBuilder.Object);

            // Act
            MergeExecutionContextCache<StatementBuilderEntity>.Flush();
            CommandTextCache.Flush();
            repository.Merge(
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
            var repositoryNever = new StatementBuilderEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.Merge(
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

        #endregion

        #region CreateMergeAll

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForMergeAll()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new StatementBuilderEntityRepository(statementBuilder.Object);

            // Act
            MergeAllExecutionContextCache<StatementBuilderEntity>.Flush();
            CommandTextCache.Flush();
            repository.MergeAll(
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
            repository.MergeAll(
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
        public void TestBaseRepositoryStatementBuilderForMergeAllWithSizePerBatchEqualsToOne()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new StatementBuilderEntityRepository(statementBuilder.Object);

            // Act
            MergeAllExecutionContextCache<StatementBuilderEntity>.Flush();
            CommandTextCache.Flush();
            repository.MergeAll(
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
            repository.MergeAll(
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

        #endregion

        #region CreateMin

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForMin()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new StatementBuilderEntityRepository(statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.Min(e => e.Id,
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
            var repositoryNever = new StatementBuilderEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.Min(e => e.Id,
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

        #endregion

        #region CreateMinAll

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForMinAll()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new StatementBuilderEntityRepository(statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.MinAll(e => e.Id);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMinAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new StatementBuilderEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.MinAll(e => e.Id);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMinAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateQuery

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForQuery()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new StatementBuilderEntityRepository(statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.Query(e => e.Id == 1);

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
            var repositoryNever = new StatementBuilderEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.Query(e => e.Id == 1);

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
        public void TestBaseRepositoryStatementBuilderForQueryAll()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new StatementBuilderEntityRepository(statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.QueryAll();

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
            repository.QueryAll();

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

        #region CreateSum

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForSum()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new StatementBuilderEntityRepository(statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.Sum(e => e.Id,
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
            var repositoryNever = new StatementBuilderEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.Sum(e => e.Id,
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

        #endregion

        #region CreateSumAll

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForSumAll()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new StatementBuilderEntityRepository(statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.SumAll(e => e.Id);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateSumAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new StatementBuilderEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.SumAll(e => e.Id);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateSumAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateTruncate

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForTruncate()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new StatementBuilderEntityRepository(statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.Truncate();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateTruncate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>())), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new StatementBuilderEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.Truncate();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateTruncate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>())), Times.Exactly(0));
        }

        #endregion

        #region CreateUpdate

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForUpdate()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new StatementBuilderEntityRepository(statementBuilder.Object);

            // Act
            UpdateExecutionContextCache<StatementBuilderEntity>.Flush();
            CommandTextCache.Flush();
            repository.Update(
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
            var repositoryNever = new StatementBuilderEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.Update(
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

        #endregion

        #region CreateUpdateAll

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForUpdateAll()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new StatementBuilderEntityRepository(statementBuilder.Object);

            // Act
            UpdateAllExecutionContextCache<StatementBuilderEntity>.Flush();
            CommandTextCache.Flush();
            repository.UpdateAll(
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
            repository.UpdateAll(
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

        #endregion

        #endregion

        #region Async

        #region CreateAverageAsync

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForAverageAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new StatementBuilderEntityRepository(statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.AverageAsync(e => e.Id,
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
            var repositoryNever = new StatementBuilderEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.AverageAsync(e => e.Id,
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

        #endregion

        #region CreateAverageAsyncAll

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForAverageAsyncAll()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new StatementBuilderEntityRepository(statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.AverageAllAsync(e => e.Id);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateAverageAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new StatementBuilderEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.AverageAllAsync(e => e.Id);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateAverageAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateBatchQueryAsync

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForBatchQueryAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new StatementBuilderEntityRepository(statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.BatchQueryAsync(0,
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
            var repositoryNever = new StatementBuilderEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.BatchQueryAsync(0,
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
        public void TestBaseRepositoryStatementBuilderForCountAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new StatementBuilderEntityRepository(statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.CountAsync((object)null).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateCount(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new StatementBuilderEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.CountAsync((object)null).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateCount(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateCountAllAsync

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForCountAllAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new StatementBuilderEntityRepository(statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.CountAllAsync().Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateCountAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new StatementBuilderEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.CountAllAsync().Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateCountAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateDeleteAsync

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForDeleteAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new StatementBuilderEntityRepository(statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.DeleteAsync(e => e.Id == 1).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDelete(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new StatementBuilderEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.DeleteAsync(e => e.Id == 1).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateDelete(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateDeleteAllAsync

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForDeleteAllAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new StatementBuilderEntityRepository(statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.DeleteAllAsync().Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDeleteAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new StatementBuilderEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.DeleteAllAsync().Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateDeleteAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateExistsAsync

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForExistsAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new StatementBuilderEntityRepository(statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.ExistsAsync((object)null).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateExists(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new StatementBuilderEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.ExistsAsync((object)null).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateExists(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateInsertAsync

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForInsertAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new StatementBuilderEntityRepository(statementBuilder.Object);

            // Act
            InsertExecutionContextCache<StatementBuilderEntity>.Flush();
            CommandTextCache.Flush();
            repository.InsertAsync(
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
            var repositoryNever = new StatementBuilderEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.InsertAsync(new StatementBuilderEntity
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

        #endregion

        #region CreateInsertAllAsync

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForInsertAllAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new StatementBuilderEntityRepository(statementBuilder.Object);

            // Act
            InsertAllExecutionContextCache<StatementBuilderEntity>.Flush();
            CommandTextCache.Flush();
            repository.InsertAllAsync(
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
            var repositoryNever = new StatementBuilderEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.InsertAllAsync(
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
        public void TestBaseRepositoryStatementBuilderForInsertAllAsyncWithSizePerBatchEqualsToOne()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new StatementBuilderEntityRepository(statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.InsertAllAsync(
                new[]
                {
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
            var repositoryNever = new StatementBuilderEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.InsertAllAsync(
                new[]
                {
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

        #endregion

        #region CreateMaxAsync

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForMaxAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new StatementBuilderEntityRepository(statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.MaxAsync(e => e.Id,
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
            var repositoryNever = new StatementBuilderEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.MaxAsync(e => e.Id,
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

        #endregion

        #region CreateMaxAsyncAll

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForMaxAsyncAll()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new StatementBuilderEntityRepository(statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.MaxAllAsync(e => e.Id);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMaxAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new StatementBuilderEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.MaxAllAsync(e => e.Id);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMaxAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateMergeAsync

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForMergeAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new StatementBuilderEntityRepository(statementBuilder.Object);

            // Act
            MergeExecutionContextCache<StatementBuilderEntity>.Flush();
            CommandTextCache.Flush();
            repository.MergeAsync(
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
            var repositoryNever = new StatementBuilderEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.MergeAsync(
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

        #endregion

        #region CreateMergeAllAsync

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForMergeAllAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new StatementBuilderEntityRepository(statementBuilder.Object);

            // Act
            MergeAllExecutionContextCache<StatementBuilderEntity>.Flush();
            CommandTextCache.Flush();
            repository.MergeAllAsync(
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
            repository.MergeAllAsync(
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
        public void TestBaseRepositoryStatementBuilderForMergeAllAsyncWithSizePerBatchEqualsToOne()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new StatementBuilderEntityRepository(statementBuilder.Object);

            // Act
            MergeAllExecutionContextCache<StatementBuilderEntity>.Flush();
            CommandTextCache.Flush();
            repository.MergeAllAsync(
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
            repository.MergeAllAsync(
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

        #endregion

        #region CreateMinAsync

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForMinAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new StatementBuilderEntityRepository(statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.MinAsync(e => e.Id,
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
            var repositoryNever = new StatementBuilderEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.MinAsync(e => e.Id,
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

        #endregion

        #region CreateMinAsyncAll

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForMinAsyncAll()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new StatementBuilderEntityRepository(statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.MinAllAsync(e => e.Id);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMinAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new StatementBuilderEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.MinAllAsync(e => e.Id);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMinAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateQueryAsync

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForQueryAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new StatementBuilderEntityRepository(statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.QueryAsync(e => e.Id == 1).Wait();

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
            var repositoryNever = new StatementBuilderEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.QueryAsync(e => e.Id == 1).Wait();

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
        public void TestBaseRepositoryStatementBuilderForQueryAllAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new StatementBuilderEntityRepository(statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.QueryAllAsync().Wait();

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
            repository.QueryAllAsync().Wait();

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

        #region CreateSumAsync

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForSumAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new StatementBuilderEntityRepository(statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.SumAsync(e => e.Id,
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
            var repositoryNever = new StatementBuilderEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.SumAsync(e => e.Id,
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

        #endregion

        #region CreateSumAsyncAll

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForSumAsyncAll()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new StatementBuilderEntityRepository(statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.SumAllAsync(e => e.Id);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateSumAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new StatementBuilderEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.SumAllAsync(e => e.Id);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateSumAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateTruncateAsync

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForTruncateAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new StatementBuilderEntityRepository(statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.TruncateAsync().Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateTruncate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>())), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new StatementBuilderEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.TruncateAsync().Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateTruncate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>())), Times.Exactly(0));
        }

        #endregion

        #region CreateUpdateAsync

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForUpdateAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new StatementBuilderEntityRepository(statementBuilder.Object);

            // Act
            UpdateExecutionContextCache<StatementBuilderEntity>.Flush();
            CommandTextCache.Flush();
            repository.UpdateAsync(
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
            var repositoryNever = new StatementBuilderEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.UpdateAsync(
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

        #endregion

        #region CreateUpdateAllAsync

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForUpdateAllAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new StatementBuilderEntityRepository(statementBuilder.Object);

            // Act
            UpdateAllExecutionContextCache<StatementBuilderEntity>.Flush();
            CommandTextCache.Flush();
            repository.UpdateAllAsync(
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
            repository.UpdateAllAsync(
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

        #endregion

        #endregion
    }
}
