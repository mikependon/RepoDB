using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoDb.Attributes;
using RepoDb.Contexts.Execution;
using RepoDb.Interfaces;
using RepoDb.UnitTests.CustomObjects;
using RepoDb.UnitTests.Setup;
using System.Collections.Generic;

namespace RepoDb.UnitTests.Interfaces
{
    [TestClass]
    public class IStatementBuilderForBaseRepositoryTest
    {
        [TestInitialize]
        public void Initialize()
        {
            DbSettingMapper.Add(typeof(CustomDbConnectionForBaseRepositoryIStatementBuilder), Helper.DbSetting, true);
            DbValidatorMapper.Add(typeof(CustomDbConnectionForBaseRepositoryIStatementBuilder), Helper.DbValidator, true);
        }

        #region SubClasses

        private class CustomDbConnectionForBaseRepositoryIStatementBuilder : CustomDbConnection { }

        private class DataEntityForBaseRepositoryStatementBuilder
        {
            [Primary, Identity]
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private class DataEntityRepository : BaseRepository<DataEntityForBaseRepositoryStatementBuilder, CustomDbConnectionForBaseRepositoryIStatementBuilder>
        {
            public DataEntityRepository(IStatementBuilder statementBuilder)
                : base("Connection", statementBuilder) { }
        }

        #endregion

        #region Sync

        #region CreateBatchQuery

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForBatchQuery()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DataEntityRepository(statementBuilder.Object);

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
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForBaseRepositoryStatementBuilder>(Helper.DbSetting)),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DataEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.BatchQuery(0,
                10,
                null,
                (object)null);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateBatchQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForBaseRepositoryStatementBuilder>(Helper.DbSetting)),
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
            var repository = new DataEntityRepository(statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.Count((object)null);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateCount(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForBaseRepositoryStatementBuilder>(Helper.DbSetting)),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DataEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.Count((object)null);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateCount(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForBaseRepositoryStatementBuilder>(Helper.DbSetting)),
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
            var repository = new DataEntityRepository(statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.CountAll();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateCountAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForBaseRepositoryStatementBuilder>(Helper.DbSetting)),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DataEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.CountAll();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateCountAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForBaseRepositoryStatementBuilder>(Helper.DbSetting)),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateDelete

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForDelete()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DataEntityRepository(statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.Delete(e => e.Id == 1);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDelete(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForBaseRepositoryStatementBuilder>(Helper.DbSetting)),
                    It.IsAny<QueryGroup>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DataEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.Delete(e => e.Id == 1);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateDelete(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForBaseRepositoryStatementBuilder>(Helper.DbSetting)),
                    It.IsAny<QueryGroup>()), Times.Exactly(0));
        }

        #endregion

        #region CreateDeleteAll

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForDeleteAll()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DataEntityRepository(statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.DeleteAll();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDeleteAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForBaseRepositoryStatementBuilder>(Helper.DbSetting))), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DataEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.DeleteAll();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateDeleteAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForBaseRepositoryStatementBuilder>(Helper.DbSetting))), Times.Exactly(0));
        }

        #endregion

        #region CreateInsert

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForInsert()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DataEntityRepository(statementBuilder.Object);

            // Act
            InsertExecutionContextCache<DataEntityForBaseRepositoryStatementBuilder>.Flush();
            CommandTextCache.Flush();
            repository.Insert(
                new DataEntityForBaseRepositoryStatementBuilder
                {
                    Name = "Name"
                });

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForBaseRepositoryStatementBuilder>(Helper.DbSetting)),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DataEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.Insert(new DataEntityForBaseRepositoryStatementBuilder
            {
                Name = "Name"
            });

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForBaseRepositoryStatementBuilder>(Helper.DbSetting)),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        #endregion

        #region CreateInsertAll

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForInsertAll()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DataEntityRepository(statementBuilder.Object);

            // Act
            InsertAllExecutionContextCache<DataEntityForBaseRepositoryStatementBuilder>.Flush();
            CommandTextCache.Flush();
            repository.InsertAll(
                new[]
                {
                    new DataEntityForBaseRepositoryStatementBuilder{ Name = "Name" },
                    new DataEntityForBaseRepositoryStatementBuilder{ Name = "Name" }
                });

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsertAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForBaseRepositoryStatementBuilder>(Helper.DbSetting)),
                    It.IsAny<IEnumerable<Field>>(),
                    It.Is<int>(v => v > 1),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DataEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.InsertAll(
                new[]
                {
                    new DataEntityForBaseRepositoryStatementBuilder{ Name = "Name" },
                    new DataEntityForBaseRepositoryStatementBuilder{ Name = "Name" }
                });

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsertAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForBaseRepositoryStatementBuilder>(Helper.DbSetting)),
                    It.IsAny<IEnumerable<Field>>(),
                    It.Is<int>(v => v > 1),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForInsertAllWithSizePerBatchEqualsToOne()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DataEntityRepository(statementBuilder.Object);

            // Act
            InsertAllExecutionContextCache<DataEntityForBaseRepositoryStatementBuilder>.Flush();
            CommandTextCache.Flush();
            repository.InsertAll(
                new[]
                {
                    new DataEntityForBaseRepositoryStatementBuilder{ Name = "Name" }
                },
                batchSize: 1);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForBaseRepositoryStatementBuilder>(Helper.DbSetting)),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DataEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.InsertAll(
                new[]
                {
                    new DataEntityForBaseRepositoryStatementBuilder{ Name = "Name" }
                },
                batchSize: 1);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForBaseRepositoryStatementBuilder>(Helper.DbSetting)),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        #endregion

        #region CreateMerge

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForMerge()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DataEntityRepository(statementBuilder.Object);

            // Act
            MergeExecutionContextCache<DataEntityForBaseRepositoryStatementBuilder>.Flush();
            CommandTextCache.Flush();
            repository.Merge(
                new DataEntityForBaseRepositoryStatementBuilder
                {
                    Name = "Name"
                },
                new Field(nameof(DataEntityForBaseRepositoryStatementBuilder.Id), Helper.DbSetting));

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForBaseRepositoryStatementBuilder>(Helper.DbSetting)),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DataEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.Merge(
                new DataEntityForBaseRepositoryStatementBuilder
                {
                    Name = "Name"
                },
                new Field(nameof(DataEntityForBaseRepositoryStatementBuilder.Id), Helper.DbSetting));

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForBaseRepositoryStatementBuilder>(Helper.DbSetting)),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        #endregion

        #region CreateMergeAll

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForMergeAll()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DataEntityRepository(statementBuilder.Object);

            // Act
            MergeAllExecutionContextCache<DataEntityForBaseRepositoryStatementBuilder>.Flush();
            CommandTextCache.Flush();
            repository.MergeAll(
                new[]
                {
                    new DataEntityForBaseRepositoryStatementBuilder { Name = "Name1" },
                    new DataEntityForBaseRepositoryStatementBuilder { Name = "Name2" },
                    new DataEntityForBaseRepositoryStatementBuilder { Name = "Name3" }
                },
                Field.From(nameof(DataEntityForBaseRepositoryStatementBuilder.Id), Helper.DbSetting));

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMergeAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForBaseRepositoryStatementBuilder>(Helper.DbSetting)),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            repository.MergeAll(
                new[]
                {
                    new DataEntityForBaseRepositoryStatementBuilder { Name = "Name1" },
                    new DataEntityForBaseRepositoryStatementBuilder { Name = "Name2" },
                    new DataEntityForBaseRepositoryStatementBuilder { Name = "Name3" }
                },
                Field.From(nameof(DataEntityForBaseRepositoryStatementBuilder.Id), Helper.DbSetting));

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMergeAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForBaseRepositoryStatementBuilder>(Helper.DbSetting)),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForMergeAllWithSizePerBatchEqualsToOne()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DataEntityRepository(statementBuilder.Object);

            // Act
            MergeAllExecutionContextCache<DataEntityForBaseRepositoryStatementBuilder>.Flush();
            CommandTextCache.Flush();
            repository.MergeAll(
                new[]
                {
                    new DataEntityForBaseRepositoryStatementBuilder { Name = "Name1" }
                },
                Field.From(nameof(DataEntityForBaseRepositoryStatementBuilder.Id), Helper.DbSetting),
                batchSize: 1);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForBaseRepositoryStatementBuilder>(Helper.DbSetting)),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            repository.MergeAll(
                new[]
                {
                    new DataEntityForBaseRepositoryStatementBuilder { Name = "Name1" }
                },
                Field.From(nameof(DataEntityForBaseRepositoryStatementBuilder.Id), Helper.DbSetting),
                batchSize: 1);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForBaseRepositoryStatementBuilder>(Helper.DbSetting)),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        #endregion

        #region CreateQuery

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForQuery()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DataEntityRepository(statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.Query(e => e.Id == 1);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForBaseRepositoryStatementBuilder>(Helper.DbSetting)),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DataEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.Query(e => e.Id == 1);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForBaseRepositoryStatementBuilder>(Helper.DbSetting)),
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
            var repository = new DataEntityRepository(statementBuilder.Object);

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

        #region CreateTruncate

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForTruncate()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DataEntityRepository(statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.Truncate();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateTruncate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForBaseRepositoryStatementBuilder>(Helper.DbSetting))), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DataEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.Truncate();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateTruncate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForBaseRepositoryStatementBuilder>(Helper.DbSetting))), Times.Exactly(0));
        }

        #endregion

        #region CreateUpdate

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForUpdate()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DataEntityRepository(statementBuilder.Object);

            // Act
            UpdateExecutionContextCache<DataEntityForBaseRepositoryStatementBuilder>.Flush();
            CommandTextCache.Flush();
            repository.Update(
                new DataEntityForBaseRepositoryStatementBuilder
                {
                    Name = "Update"
                },
                e => e.Id == 1);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateUpdate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForBaseRepositoryStatementBuilder>(Helper.DbSetting)),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DataEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.Update(
                new DataEntityForBaseRepositoryStatementBuilder
                {
                    Name = "Update"
                },
                e => e.Id == 1);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateUpdate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForBaseRepositoryStatementBuilder>(Helper.DbSetting)),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        #endregion

        #region CreateUpdateAll

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForUpdateAll()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DataEntityRepository(statementBuilder.Object);

            // Act
            UpdateAllExecutionContextCache<DataEntityForBaseRepositoryStatementBuilder>.Flush();
            CommandTextCache.Flush();
            repository.UpdateAll(
                new[]
                {
                    new DataEntityForBaseRepositoryStatementBuilder { Name = "Name1" },
                    new DataEntityForBaseRepositoryStatementBuilder { Name = "Name2" },
                    new DataEntityForBaseRepositoryStatementBuilder { Name = "Name3" }
                },
                Field.From(nameof(DataEntityForBaseRepositoryStatementBuilder.Id), Helper.DbSetting));

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateUpdateAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForBaseRepositoryStatementBuilder>(Helper.DbSetting)),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            repository.UpdateAll(
                new[]
                {
                    new DataEntityForBaseRepositoryStatementBuilder { Name = "Name1" },
                    new DataEntityForBaseRepositoryStatementBuilder { Name = "Name2" },
                    new DataEntityForBaseRepositoryStatementBuilder { Name = "Name3" }
                },
                Field.From(nameof(DataEntityForBaseRepositoryStatementBuilder.Id), Helper.DbSetting));

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateUpdateAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForBaseRepositoryStatementBuilder>(Helper.DbSetting)),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        #endregion

        #endregion

        #region Async

        #region CreateBatchQueryAsync

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForBatchQueryAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DataEntityRepository(statementBuilder.Object);

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
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForBaseRepositoryStatementBuilder>(Helper.DbSetting)),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DataEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.BatchQueryAsync(0,
                10,
                null,
                (object)null).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateBatchQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForBaseRepositoryStatementBuilder>(Helper.DbSetting)),
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
            var repository = new DataEntityRepository(statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.CountAsync((object)null).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateCount(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForBaseRepositoryStatementBuilder>(Helper.DbSetting)),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DataEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.CountAsync((object)null).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateCount(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForBaseRepositoryStatementBuilder>(Helper.DbSetting)),
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
            var repository = new DataEntityRepository(statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.CountAllAsync().Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateCountAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForBaseRepositoryStatementBuilder>(Helper.DbSetting)),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DataEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.CountAllAsync().Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateCountAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForBaseRepositoryStatementBuilder>(Helper.DbSetting)),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateDeleteAsync

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForDeleteAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DataEntityRepository(statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.DeleteAsync(e => e.Id == 1).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDelete(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForBaseRepositoryStatementBuilder>(Helper.DbSetting)),
                    It.IsAny<QueryGroup>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DataEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.DeleteAsync(e => e.Id == 1).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateDelete(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForBaseRepositoryStatementBuilder>(Helper.DbSetting)),
                    It.IsAny<QueryGroup>()), Times.Exactly(0));
        }

        #endregion

        #region CreateDeleteAllAsync

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForDeleteAllAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DataEntityRepository(statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.DeleteAllAsync().Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDeleteAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForBaseRepositoryStatementBuilder>(Helper.DbSetting))), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DataEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.DeleteAllAsync().Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateDeleteAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForBaseRepositoryStatementBuilder>(Helper.DbSetting))), Times.Exactly(0));
        }

        #endregion

        #region CreateInsertAsync

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForInsertAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DataEntityRepository(statementBuilder.Object);

            // Act
            InsertExecutionContextCache<DataEntityForBaseRepositoryStatementBuilder>.Flush();
            CommandTextCache.Flush();
            repository.InsertAsync(
                new DataEntityForBaseRepositoryStatementBuilder
                {
                    Name = "Name"
                }).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForBaseRepositoryStatementBuilder>(Helper.DbSetting)),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DataEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.InsertAsync(new DataEntityForBaseRepositoryStatementBuilder
            {
                Name = "Name"
            }).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForBaseRepositoryStatementBuilder>(Helper.DbSetting)),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        #endregion

        #region CreateInsertAllAsync

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForInsertAllAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DataEntityRepository(statementBuilder.Object);

            // Act
            InsertAllExecutionContextCache<DataEntityForBaseRepositoryStatementBuilder>.Flush();
            CommandTextCache.Flush();
            repository.InsertAllAsync(
                new[]
                {
                    new DataEntityForBaseRepositoryStatementBuilder{ Name = "Name" },
                    new DataEntityForBaseRepositoryStatementBuilder{ Name = "Name" }
                }).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsertAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForBaseRepositoryStatementBuilder>(Helper.DbSetting)),
                    It.IsAny<IEnumerable<Field>>(),
                    It.Is<int>(v => v > 1),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DataEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.InsertAllAsync(
                new[]
                {
                    new DataEntityForBaseRepositoryStatementBuilder{ Name = "Name" },
                    new DataEntityForBaseRepositoryStatementBuilder{ Name = "Name" }
                }).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsertAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForBaseRepositoryStatementBuilder>(Helper.DbSetting)),
                    It.IsAny<IEnumerable<Field>>(),
                    It.Is<int>(v => v > 1),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForInsertAllAsyncWithSizePerBatchEqualsToOne()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DataEntityRepository(statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.InsertAllAsync(
                new[]
                {
                    new DataEntityForBaseRepositoryStatementBuilder{ Name = "Name" }
                },
                batchSize: 1).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForBaseRepositoryStatementBuilder>(Helper.DbSetting)),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DataEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.InsertAllAsync(
                new[]
                {
                    new DataEntityForBaseRepositoryStatementBuilder{ Name = "Name" }
                },
                batchSize: 1).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForBaseRepositoryStatementBuilder>(Helper.DbSetting)),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        #endregion

        #region CreateMergeAsync

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForMergeAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DataEntityRepository(statementBuilder.Object);

            // Act
            MergeExecutionContextCache<DataEntityForBaseRepositoryStatementBuilder>.Flush();
            CommandTextCache.Flush();
            repository.MergeAsync(
                new DataEntityForBaseRepositoryStatementBuilder
                {
                    Name = "Name"
                },
                new Field(nameof(DataEntityForBaseRepositoryStatementBuilder.Id), Helper.DbSetting)).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForBaseRepositoryStatementBuilder>(Helper.DbSetting)),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DataEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.MergeAsync(
                new DataEntityForBaseRepositoryStatementBuilder
                {
                    Name = "Name"
                },
                new Field(nameof(DataEntityForBaseRepositoryStatementBuilder.Id), Helper.DbSetting)).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForBaseRepositoryStatementBuilder>(Helper.DbSetting)),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        #endregion

        #region CreateMergeAllAsync

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForMergeAllAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DataEntityRepository(statementBuilder.Object);

            // Act
            MergeAllExecutionContextCache<DataEntityForBaseRepositoryStatementBuilder>.Flush();
            CommandTextCache.Flush();
            repository.MergeAllAsync(
                new[]
                {
                    new DataEntityForBaseRepositoryStatementBuilder { Name = "Name1" },
                    new DataEntityForBaseRepositoryStatementBuilder { Name = "Name2" },
                    new DataEntityForBaseRepositoryStatementBuilder { Name = "Name3" }
                },
                Field.From(nameof(DataEntityForBaseRepositoryStatementBuilder.Id), Helper.DbSetting)).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMergeAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForBaseRepositoryStatementBuilder>(Helper.DbSetting)),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            repository.MergeAllAsync(
                new[]
                {
                    new DataEntityForBaseRepositoryStatementBuilder { Name = "Name1" },
                    new DataEntityForBaseRepositoryStatementBuilder { Name = "Name2" },
                    new DataEntityForBaseRepositoryStatementBuilder { Name = "Name3" }
                },
                Field.From(nameof(DataEntityForBaseRepositoryStatementBuilder.Id), Helper.DbSetting)).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMergeAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForBaseRepositoryStatementBuilder>(Helper.DbSetting)),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForMergeAllAsyncWithSizePerBatchEqualsToOne()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DataEntityRepository(statementBuilder.Object);

            // Act
            MergeAllExecutionContextCache<DataEntityForBaseRepositoryStatementBuilder>.Flush();
            CommandTextCache.Flush();
            repository.MergeAllAsync(
                new[]
                {
                    new DataEntityForBaseRepositoryStatementBuilder { Name = "Name1" }
                },
                Field.From(nameof(DataEntityForBaseRepositoryStatementBuilder.Id), Helper.DbSetting),
                batchSize: 1).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForBaseRepositoryStatementBuilder>(Helper.DbSetting)),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            repository.MergeAllAsync(
                new[]
                {
                    new DataEntityForBaseRepositoryStatementBuilder { Name = "Name1" }
                },
                Field.From(nameof(DataEntityForBaseRepositoryStatementBuilder.Id), Helper.DbSetting),
                batchSize: 1).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForBaseRepositoryStatementBuilder>(Helper.DbSetting)),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        #endregion

        #region CreateQueryAsync

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForQueryAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DataEntityRepository(statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.QueryAsync(e => e.Id == 1).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForBaseRepositoryStatementBuilder>(Helper.DbSetting)),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DataEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.QueryAsync(e => e.Id == 1).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForBaseRepositoryStatementBuilder>(Helper.DbSetting)),
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
            var repository = new DataEntityRepository(statementBuilder.Object);

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

        #region CreateTruncateAsync

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForTruncateAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DataEntityRepository(statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.TruncateAsync().Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateTruncate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForBaseRepositoryStatementBuilder>(Helper.DbSetting))), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DataEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.TruncateAsync().Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateTruncate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForBaseRepositoryStatementBuilder>(Helper.DbSetting))), Times.Exactly(0));
        }

        #endregion

        #region CreateUpdateAsync

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForUpdateAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DataEntityRepository(statementBuilder.Object);

            // Act
            UpdateExecutionContextCache<DataEntityForBaseRepositoryStatementBuilder>.Flush();
            CommandTextCache.Flush();
            repository.UpdateAsync(
                new DataEntityForBaseRepositoryStatementBuilder
                {
                    Name = "Update"
                },
                e => e.Id == 1).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateUpdate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForBaseRepositoryStatementBuilder>(Helper.DbSetting)),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DataEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.UpdateAsync(
                new DataEntityForBaseRepositoryStatementBuilder
                {
                    Name = "Update"
                },
                e => e.Id == 1).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateUpdate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForBaseRepositoryStatementBuilder>(Helper.DbSetting)),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        #endregion

        #region CreateUpdateAllAsync

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForUpdateAllAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DataEntityRepository(statementBuilder.Object);

            // Act
            UpdateAllExecutionContextCache<DataEntityForBaseRepositoryStatementBuilder>.Flush();
            CommandTextCache.Flush();
            repository.UpdateAllAsync(
                new[]
                {
                    new DataEntityForBaseRepositoryStatementBuilder { Name = "Name1" },
                    new DataEntityForBaseRepositoryStatementBuilder { Name = "Name2" },
                    new DataEntityForBaseRepositoryStatementBuilder { Name = "Name3" }
                },
                Field.From(nameof(DataEntityForBaseRepositoryStatementBuilder.Id), Helper.DbSetting)).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateUpdateAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForBaseRepositoryStatementBuilder>(Helper.DbSetting)),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            repository.UpdateAllAsync(
                new[]
                {
                    new DataEntityForBaseRepositoryStatementBuilder { Name = "Name1" },
                    new DataEntityForBaseRepositoryStatementBuilder { Name = "Name2" },
                    new DataEntityForBaseRepositoryStatementBuilder { Name = "Name3" }
                },
                Field.From(nameof(DataEntityForBaseRepositoryStatementBuilder.Id), Helper.DbSetting)).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateUpdateAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForBaseRepositoryStatementBuilder>(Helper.DbSetting)),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        #endregion

        #endregion
    }
}
