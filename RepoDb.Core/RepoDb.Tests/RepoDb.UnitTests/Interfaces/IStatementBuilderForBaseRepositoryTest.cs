using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoDb.Attributes;
using RepoDb.Interfaces;
using RepoDb.UnitTests.CustomObjects;
using System.Collections.Generic;

namespace RepoDb.UnitTests.Interfaces
{
    [TestClass]
    public class IStatementBuilderForBaseRepositoryTest
    {
        public class DataEntity
        {
            [Primary, Identity]
            public int Id { get; set; }
            public string Name { get; set; }
        }

        // CreateBatchQuery

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForBatchQuery()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new Mock<BaseRepository<DataEntity, CustomDbConnection>>("ConnectionString", statementBuilder.Object);

            // Setup
            statementBuilder.Setup(builder =>
                builder.CreateBatchQuery<DataEntity>(
                    It.IsAny<QueryBuilder<DataEntity>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<IEnumerable<OrderField>>()));

            // Act
            repository.Object.BatchQuery(0, 10, null, null);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateBatchQuery<DataEntity>(
                    It.IsAny<QueryBuilder<DataEntity>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<IEnumerable<OrderField>>()), Times.Once);
        }

        // CreateCount

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForCount()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new Mock<BaseRepository<DataEntity, CustomDbConnection>>("ConnectionString", statementBuilder.Object);

            // Setup
            statementBuilder.Setup(builder =>
                builder.CreateCount<DataEntity>(
                    It.IsAny<QueryBuilder<DataEntity>>(),
                    It.IsAny<QueryGroup>()));

            // Act
            repository.Object.Count();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateCount<DataEntity>(
                    It.IsAny<QueryBuilder<DataEntity>>(),
                    It.IsAny<QueryGroup>()), Times.Once);
        }

        // CreateDelete

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForDelete()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new Mock<BaseRepository<DataEntity, CustomDbConnection>>("ConnectionString", statementBuilder.Object);

            // Setup
            statementBuilder.Setup(builder =>
                builder.CreateDelete<DataEntity>(
                    It.IsAny<QueryBuilder<DataEntity>>(),
                    It.IsAny<QueryGroup>()));

            // Act
            repository.Object.Delete(e => e.Id == 1);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDelete<DataEntity>(
                    It.IsAny<QueryBuilder<DataEntity>>(),
                    It.IsAny<QueryGroup>()), Times.Once);
        }

        // CreateDeleteAll

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForDeleteAll()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new Mock<BaseRepository<DataEntity, CustomDbConnection>>("ConnectionString", statementBuilder.Object);

            // Setup
            statementBuilder.Setup(builder =>
                builder.CreateDeleteAll<DataEntity>(
                    It.IsAny<QueryBuilder<DataEntity>>()));

            // Act
            repository.Object.DeleteAll();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDeleteAll<DataEntity>(
                    It.IsAny<QueryBuilder<DataEntity>>()), Times.Once);
        }

        // CreateInlineInsert

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForInlineInsert()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new Mock<BaseRepository<DataEntity, CustomDbConnection>>("ConnectionString", statementBuilder.Object);

            // Setup
            statementBuilder.Setup(builder =>
                builder.CreateInlineInsert<DataEntity>(
                    It.IsAny<QueryBuilder<DataEntity>>(),
                    It.IsAny<IEnumerable<Field>>()));

            // Act
            repository.Object.InlineInsert(new { Id = 1 });

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInlineInsert<DataEntity>(
                    It.IsAny<QueryBuilder<DataEntity>>(),
                    It.IsAny<IEnumerable<Field>>()), Times.Once);
        }

        // CreateInlineMerge

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForInlineMerge()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new Mock<BaseRepository<DataEntity, CustomDbConnection>>("ConnectionString", statementBuilder.Object);

            // Setup
            statementBuilder.Setup(builder =>
                builder.CreateInlineMerge<DataEntity>(
                    It.IsAny<QueryBuilder<DataEntity>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>()));

            // Act
            repository.Object.InlineMerge(new { Id = 1, Name = "Name" });

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInlineMerge<DataEntity>(
                    It.IsAny<QueryBuilder<DataEntity>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>()), Times.Once);
        }

        // CreateInlineUpdate

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForInlineUpdate()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new Mock<BaseRepository<DataEntity, CustomDbConnection>>("ConnectionString", statementBuilder.Object);

            // Setup
            statementBuilder.Setup(builder =>
                builder.CreateInlineUpdate<DataEntity>(
                    It.IsAny<QueryBuilder<DataEntity>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>()));

            // Act
            repository.Object.InlineUpdate(new { Name = "Name" }, e => e.Id == 1);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInlineUpdate<DataEntity>(
                    It.IsAny<QueryBuilder<DataEntity>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>()), Times.Once);
        }

        // CreateInsert

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForInsert()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new Mock<BaseRepository<DataEntity, CustomDbConnection>>("ConnectionString", statementBuilder.Object);

            // Setup
            statementBuilder.Setup(builder =>
                builder.CreateInsert<DataEntity>(
                    It.IsAny<QueryBuilder<DataEntity>>()));

            // Act
            repository.Object.Insert(new DataEntity { Name = "Name" });

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsert<DataEntity>(
                    It.IsAny<QueryBuilder<DataEntity>>()), Times.Once);
        }

        // CreateMerge

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForMerge()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new Mock<BaseRepository<DataEntity, CustomDbConnection>>("ConnectionString", statementBuilder.Object);

            // Setup
            statementBuilder.Setup(builder =>
                builder.CreateMerge<DataEntity>(
                    It.IsAny<QueryBuilder<DataEntity>>(),
                    It.IsAny<IEnumerable<Field>>()));

            // Act
            repository.Object.Merge(new DataEntity { Name = "Name" }, new Field(nameof(DataEntity.Id)));

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMerge<DataEntity>(
                    It.IsAny<QueryBuilder<DataEntity>>(),
                    It.IsAny<IEnumerable<Field>>()), Times.Once);
        }

        // CreateQuery

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForQuery()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new Mock<BaseRepository<DataEntity, CustomDbConnection>>("ConnectionString", statementBuilder.Object);

            // Setup
            statementBuilder.Setup(builder =>
                builder.CreateQuery<DataEntity>(
                    It.IsAny<QueryBuilder<DataEntity>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()));

            // Act
            repository.Object.Query(e => e.Id == 1);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateQuery<DataEntity>(
                    It.IsAny<QueryBuilder<DataEntity>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Once);
        }

        // CreateTruncate

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForTruncate()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new Mock<BaseRepository<DataEntity, CustomDbConnection>>("ConnectionString", statementBuilder.Object);

            // Setup
            statementBuilder.Setup(builder =>
                builder.CreateTruncate<DataEntity>(
                    It.IsAny<QueryBuilder<DataEntity>>()));

            // Act
            repository.Object.Truncate();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateTruncate<DataEntity>(
                    It.IsAny<QueryBuilder<DataEntity>>()), Times.Once);
        }

        // CreateUpdate

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForUpdate()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new Mock<BaseRepository<DataEntity, CustomDbConnection>>("ConnectionString", statementBuilder.Object);

            // Setup
            statementBuilder.Setup(builder =>
                builder.CreateUpdate<DataEntity>(
                    It.IsAny<QueryBuilder<DataEntity>>(),
                    It.IsAny<QueryGroup>()));

            // Act
            repository.Object.Update(new DataEntity { Name = "Update" }, e => e.Id == 1);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateUpdate<DataEntity>(
                    It.IsAny<QueryBuilder<DataEntity>>(),
                    It.IsAny<QueryGroup>()), Times.Once);
        }
    }
}
