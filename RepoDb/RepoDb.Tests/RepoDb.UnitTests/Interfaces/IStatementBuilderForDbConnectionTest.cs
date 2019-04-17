using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoDb.Attributes;
using RepoDb.Interfaces;
using RepoDb.UnitTests.CustomObjects;
using System.Collections.Generic;

namespace RepoDb.UnitTests.Interfaces
{
    [TestClass]
    public class IStatementBuilderForDbConnectionTest
    {
        public class StatementBuilderForDbConnectionDataEntity
        {
            [Primary, Identity]
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class StatementBuilderForDbConnectionDataEntityT1
        {
            [Primary, Identity]
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class StatementBuilderForDbConnectionDataEntityT2
        {
            [Primary, Identity]
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class StatementBuilderForDbConnectionDataEntityT3
        {
            [Primary, Identity]
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class StatementBuilderForDbConnectionDataEntityT4
        {
            [Primary, Identity]
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class StatementBuilderForDbConnectionDataEntityT5
        {
            [Primary, Identity]
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class StatementBuilderForDbConnectionDataEntityT6
        {
            [Primary, Identity]
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class StatementBuilderForDbConnectionDataEntityT7
        {
            [Primary, Identity]
            public int Id { get; set; }
            public string Name { get; set; }
        }

        // CreateBatchQuery

        [TestMethod]
        public void TestDbConnectionStatementBuilderForBatchQuery()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new CustomDbConnection();

            // Act
            connection.BatchQuery<StatementBuilderForDbConnectionDataEntity>(0, 10, null, null, statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateBatchQuery<StatementBuilderForDbConnectionDataEntity>(
                    It.IsAny<QueryBuilder>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<string>()), Times.Once);
        }

        // CreateCount

        [TestMethod]
        public void TestDbConnectionStatementBuilderForCount()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new CustomDbConnection();

            // Act
            connection.Count<StatementBuilderForDbConnectionDataEntity>(statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateCount<StatementBuilderForDbConnectionDataEntity>(
                    It.IsAny<QueryBuilder>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Once);
        }

        // CreateDelete

        [TestMethod]
        public void TestDbConnectionStatementBuilderForDelete()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new CustomDbConnection();

            // Act
            connection.Delete<StatementBuilderForDbConnectionDataEntity>(e => e.Id == 1, statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDelete<StatementBuilderForDbConnectionDataEntity>(
                    It.IsAny<QueryBuilder>(),
                    It.IsAny<QueryGroup>()), Times.Once);
        }

        // CreateDeleteAll

        [TestMethod]
        public void TestDbConnectionStatementBuilderForDeleteAll()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new CustomDbConnection();

            // Act
            connection.DeleteAll<StatementBuilderForDbConnectionDataEntity>(statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDeleteAll<StatementBuilderForDbConnectionDataEntity>(
                    It.IsAny<QueryBuilder>()), Times.Once);
        }

        // CreateInlineInsert

        [TestMethod]
        public void TestDbConnectionStatementBuilderForInlineInsert()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new CustomDbConnection();

            // Act
            connection.InlineInsert<StatementBuilderForDbConnectionDataEntity>(new { Id = 1, Name = "Name" }, statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInlineInsert<StatementBuilderForDbConnectionDataEntity>(
                    It.IsAny<QueryBuilder>(),
                    It.IsAny<DbField>(),
                    It.IsAny<IEnumerable<Field>>()), Times.Once);
        }

        // CreateInlineMerge

        [TestMethod]
        public void TestDbConnectionStatementBuilderForInlineMerge()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new CustomDbConnection();

            // Act
            connection.InlineMerge<StatementBuilderForDbConnectionDataEntity>(new { Name = "Name" }, new Field(nameof(StatementBuilderForDbConnectionDataEntity.Id)), statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInlineMerge<StatementBuilderForDbConnectionDataEntity>(
                    It.IsAny<QueryBuilder>(),
                    It.IsAny<DbField>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>()), Times.Once);
        }

        // CreateInlineUpdate

        [TestMethod]
        public void TestDbConnectionStatementBuilderForInlineUpdate()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new CustomDbConnection();

            // Act
            connection.InlineUpdate<StatementBuilderForDbConnectionDataEntity>(new { Name = "Name" }, e => e.Id == 1, statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInlineUpdate<StatementBuilderForDbConnectionDataEntity>(
                    It.IsAny<QueryBuilder>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>()), Times.Once);
        }

        // CreateInsert

        [TestMethod]
        public void TestDbConnectionStatementBuilderForInsert()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new CustomDbConnection();

            // Act
            connection.Insert<StatementBuilderForDbConnectionDataEntity>(new StatementBuilderForDbConnectionDataEntity { Name = "Name" }, statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsert<StatementBuilderForDbConnectionDataEntity>(
                    It.IsAny<QueryBuilder>(),
                    It.IsAny<DbField>()), Times.Once);
        }

        // CreateMerge

        [TestMethod]
        public void TestDbConnectionStatementBuilderForMerge()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new CustomDbConnection();

            // Act
            connection.Merge<StatementBuilderForDbConnectionDataEntity>(new StatementBuilderForDbConnectionDataEntity { Name = "Name" }, new Field(nameof(StatementBuilderForDbConnectionDataEntity.Id)), statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMerge<StatementBuilderForDbConnectionDataEntity>(
                    It.IsAny<QueryBuilder>(),
                    It.IsAny<DbField>(),
                    It.IsAny<IEnumerable<Field>>()), Times.Once);
        }

        // CreateQuery

        [TestMethod]
        public void TestDbConnectionStatementBuilderForQuery()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new CustomDbConnection();

            // Act
            connection.Query<StatementBuilderForDbConnectionDataEntity>(e => e.Id == 1, statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateQuery<StatementBuilderForDbConnectionDataEntity>(
                    It.IsAny<QueryBuilder>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Once);
        }

        // QueryMultple

        [TestMethod]
        public void TestDbConnectionStatementBuilderForQueryMultiple()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new CustomDbConnection();

            // Act
            connection.QueryMultiple<StatementBuilderForDbConnectionDataEntityT1,
                StatementBuilderForDbConnectionDataEntityT2,
                StatementBuilderForDbConnectionDataEntityT3,
                StatementBuilderForDbConnectionDataEntityT4,
                StatementBuilderForDbConnectionDataEntityT5,
                StatementBuilderForDbConnectionDataEntityT6,
                StatementBuilderForDbConnectionDataEntityT7>(e => e.Id == 1,
                e => e.Id == 1,
                e => e.Id == 1,
                e => e.Id == 1,
                e => e.Id == 1,
                e => e.Id == 1,
                e => e.Id == 1, statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateQuery<StatementBuilderForDbConnectionDataEntityT1>(
                    It.IsAny<QueryBuilder>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));
            statementBuilder.Verify(builder =>
                builder.CreateQuery<StatementBuilderForDbConnectionDataEntityT2>(
                    It.IsAny<QueryBuilder>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));
            statementBuilder.Verify(builder =>
                builder.CreateQuery<StatementBuilderForDbConnectionDataEntityT3>(
                    It.IsAny<QueryBuilder>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));
            statementBuilder.Verify(builder =>
                builder.CreateQuery<StatementBuilderForDbConnectionDataEntityT4>(
                    It.IsAny<QueryBuilder>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));
            statementBuilder.Verify(builder =>
                builder.CreateQuery<StatementBuilderForDbConnectionDataEntityT5>(
                    It.IsAny<QueryBuilder>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));
            statementBuilder.Verify(builder =>
                builder.CreateQuery<StatementBuilderForDbConnectionDataEntityT6>(
                    It.IsAny<QueryBuilder>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));
            statementBuilder.Verify(builder =>
                builder.CreateQuery<StatementBuilderForDbConnectionDataEntityT7>(
                    It.IsAny<QueryBuilder>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));
        }

        // CreateTruncate

        [TestMethod]
        public void TestDbConnectionStatementBuilderForTruncate()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new CustomDbConnection();

            // Act
            connection.Truncate<StatementBuilderForDbConnectionDataEntity>(statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateTruncate<StatementBuilderForDbConnectionDataEntity>(
                    It.IsAny<QueryBuilder>()), Times.Once);
        }

        // CreateUpdate

        [TestMethod]
        public void TestDbConnectionStatementBuilderForUpdate()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new CustomDbConnection();

            // Act
            connection.Update<StatementBuilderForDbConnectionDataEntity>(new StatementBuilderForDbConnectionDataEntity { Name = "Update" }, e => e.Id == 1, statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateUpdate<StatementBuilderForDbConnectionDataEntity>(
                    It.IsAny<QueryBuilder>(),
                    It.IsAny<QueryGroup>()), Times.Once);
        }
    }
}
