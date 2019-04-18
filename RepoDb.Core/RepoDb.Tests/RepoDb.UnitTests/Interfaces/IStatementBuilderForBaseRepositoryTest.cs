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
        public class StatementBuilderForBaseRepositoryDataEntity
        {
            [Primary, Identity]
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class DataEntityRepository : BaseRepository<StatementBuilderForBaseRepositoryDataEntity, CustomDbConnection>
        {
            public DataEntityRepository(IStatementBuilder statementBuilder)
                : base("Connection", statementBuilder) { }
        }

        // CreateBatchQuery

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForBatchQuery()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DataEntityRepository(statementBuilder.Object);

            // Act
            repository.BatchQuery(0, 10, null, null);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateBatchQuery<StatementBuilderForBaseRepositoryDataEntity>(
                    It.IsAny<QueryBuilder>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<string>()), Times.Once);

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DataEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.BatchQuery(0, 10, null, null);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateBatchQuery<StatementBuilderForBaseRepositoryDataEntity>(
                    It.IsAny<QueryBuilder>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<string>()), Times.Never);
        }

        // CreateCount

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForCount()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DataEntityRepository(statementBuilder.Object);

            // Act
            repository.Count();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateCount<StatementBuilderForBaseRepositoryDataEntity>(
                    It.IsAny<QueryBuilder>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Once);

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DataEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.Count();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateCount<StatementBuilderForBaseRepositoryDataEntity>(
                    It.IsAny<QueryBuilder>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Never);
        }

        // CreateDelete

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForDelete()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DataEntityRepository(statementBuilder.Object);

            // Act
            repository.Delete(e => e.Id == 1);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDelete<StatementBuilderForBaseRepositoryDataEntity>(
                    It.IsAny<QueryBuilder>(),
                    It.IsAny<QueryGroup>()), Times.Once);

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DataEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.Delete(e => e.Id == 1);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateDelete<StatementBuilderForBaseRepositoryDataEntity>(
                    It.IsAny<QueryBuilder>(),
                    It.IsAny<QueryGroup>()), Times.Never);
        }

        // CreateDeleteAll

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForDeleteAll()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DataEntityRepository(statementBuilder.Object);

            // Act
            repository.DeleteAll();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDeleteAll<StatementBuilderForBaseRepositoryDataEntity>(
                    It.IsAny<QueryBuilder>()), Times.Once);

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DataEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.DeleteAll();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateDeleteAll<StatementBuilderForBaseRepositoryDataEntity>(
                    It.IsAny<QueryBuilder>()), Times.Never);
        }

        // CreateInlineInsert

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForInlineInsert()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DataEntityRepository(statementBuilder.Object);

            // Act
            repository.InlineInsert(new { Id = 1 });

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInlineInsert<StatementBuilderForBaseRepositoryDataEntity>(
                    It.IsAny<QueryBuilder>(),
                    It.IsAny<DbField>(),
                    It.IsAny<IEnumerable<Field>>()), Times.Once);

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DataEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.InlineInsert(new { Id = 1 });

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInlineInsert<StatementBuilderForBaseRepositoryDataEntity>(
                    It.IsAny<QueryBuilder>(),
                    It.IsAny<DbField>(),
                    It.IsAny<IEnumerable<Field>>()), Times.Never);
        }

        // CreateInlineMerge

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForInlineMerge()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DataEntityRepository(statementBuilder.Object);

            // Act
            repository.InlineMerge(new { Id = 1, Name = "Name" });

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInlineMerge<StatementBuilderForBaseRepositoryDataEntity>(
                    It.IsAny<QueryBuilder>(),
                    It.IsAny<DbField>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>()), Times.Once);

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DataEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.InlineMerge(new { Id = 1, Name = "Name" });

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInlineMerge<StatementBuilderForBaseRepositoryDataEntity>(
                    It.IsAny<QueryBuilder>(),
                    It.IsAny<DbField>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>()), Times.Never);
        }

        // CreateInlineUpdate

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForInlineUpdate()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DataEntityRepository(statementBuilder.Object);

            // Act
            repository.InlineUpdate(new { Name = "Name" }, e => e.Id == 1);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInlineUpdate<StatementBuilderForBaseRepositoryDataEntity>(
                    It.IsAny<QueryBuilder>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>()), Times.Once);

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DataEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.InlineUpdate(new { Name = "Name" }, e => e.Id == 1);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInlineUpdate<StatementBuilderForBaseRepositoryDataEntity>(
                    It.IsAny<QueryBuilder>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>()), Times.Never);
        }

        // CreateInsert

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForInsert()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DataEntityRepository(statementBuilder.Object);

            // Act
            repository.Insert(new StatementBuilderForBaseRepositoryDataEntity { Name = "Name" });

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsert<StatementBuilderForBaseRepositoryDataEntity>(
                    It.IsAny<QueryBuilder>(),
                    It.IsAny<DbField>()), Times.Once);

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DataEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.Insert(new StatementBuilderForBaseRepositoryDataEntity { Name = "Name" });

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsert<StatementBuilderForBaseRepositoryDataEntity>(
                    It.IsAny<QueryBuilder>(),
                    It.IsAny<DbField>()), Times.Never);
        }

        // CreateMerge

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForMerge()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DataEntityRepository(statementBuilder.Object);

            // Act
            repository.Merge(new StatementBuilderForBaseRepositoryDataEntity { Name = "Name" }, new Field(nameof(StatementBuilderForBaseRepositoryDataEntity.Id)));

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMerge<StatementBuilderForBaseRepositoryDataEntity>(
                    It.IsAny<QueryBuilder>(),
                    It.IsAny<DbField>(),
                    It.IsAny<IEnumerable<Field>>()), Times.Once);

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DataEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.Merge(new StatementBuilderForBaseRepositoryDataEntity { Name = "Name" }, new Field(nameof(StatementBuilderForBaseRepositoryDataEntity.Id)));

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMerge<StatementBuilderForBaseRepositoryDataEntity>(
                    It.IsAny<QueryBuilder>(),
                    It.IsAny<DbField>(),
                    It.IsAny<IEnumerable<Field>>()), Times.Never);
        }

        // CreateQuery

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForQuery()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DataEntityRepository(statementBuilder.Object);

            // Act
            repository.Query(e => e.Id == 1);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateQuery<StatementBuilderForBaseRepositoryDataEntity>(
                    It.IsAny<QueryBuilder>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Once);

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DataEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.Query(e => e.Id == 1);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery<StatementBuilderForBaseRepositoryDataEntity>(
                    It.IsAny<QueryBuilder>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Never);
        }

        // CreateTruncate

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForTruncate()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DataEntityRepository(statementBuilder.Object);

            // Act
            repository.Truncate();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateTruncate<StatementBuilderForBaseRepositoryDataEntity>(
                    It.IsAny<QueryBuilder>()), Times.Once);

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DataEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.Truncate();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateTruncate<StatementBuilderForBaseRepositoryDataEntity>(
                    It.IsAny<QueryBuilder>()), Times.Never);
        }

        // CreateUpdate

        [TestMethod]
        public void TestBaseRepositoryStatementBuilderForUpdate()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DataEntityRepository(statementBuilder.Object);

            // Act
            repository.Update(new StatementBuilderForBaseRepositoryDataEntity { Name = "Update" }, e => e.Id == 1);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateUpdate<StatementBuilderForBaseRepositoryDataEntity>(
                    It.IsAny<QueryBuilder>(),
                    It.IsAny<QueryGroup>()), Times.Once);

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DataEntityRepository(statementBuilderNever.Object);

            // Act
            repositoryNever.Update(new StatementBuilderForBaseRepositoryDataEntity { Name = "Update" }, e => e.Id == 1);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateUpdate<StatementBuilderForBaseRepositoryDataEntity>(
                    It.IsAny<QueryBuilder>(),
                    It.IsAny<QueryGroup>()), Times.Never);
        }
    }
}
