using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoDb.Attributes;
using RepoDb.Interfaces;
using RepoDb.UnitTests.CustomObjects;

namespace RepoDb.UnitTests.Interfaces
{
    [TestClass]
    public class ITraceForDbConnectionTest
    {
        public class TraceEntity
        {
            [Primary, Identity]
            public int Id { get; set; }
            public string Name { get; set; }
        }

        // BatchQuery

        [TestMethod]
        public void TestDbConnectionTraceForBeforeBatchQuery()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.BatchQuery<TraceEntity>(0, 10, null, null, trace: trace.Object, statementBuilder: new SqlStatementBuilder());

            // Assert
            trace.Verify(t => t.BeforeBatchQuery(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterBatchQuery()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.BatchQuery<TraceEntity>(0, 10, null, null, trace: trace.Object, statementBuilder: new SqlStatementBuilder());

            // Assert
            trace.Verify(t => t.AfterBatchQuery(It.IsAny<TraceLog>()), Times.Once);
        }

        // Count

        [TestMethod]
        public void TestDbConnectionTraceForBeforeBulkInsert()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.Count<TraceEntity>(trace: trace.Object, statementBuilder: new SqlStatementBuilder());

            // Assert
            trace.Verify(t => t.BeforeCount(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterCount()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.Count<TraceEntity>(trace: trace.Object, statementBuilder: new SqlStatementBuilder());

            // Assert
            trace.Verify(t => t.AfterCount(It.IsAny<TraceLog>()), Times.Once);
        }

        // Delete

        [TestMethod]
        public void TestDbConnectionTraceForBeforeDelete()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.Delete<TraceEntity>(0, trace: trace.Object, statementBuilder: new SqlStatementBuilder());

            // Assert
            trace.Verify(t => t.BeforeDelete(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterDelete()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.Delete<TraceEntity>(0, trace: trace.Object, statementBuilder: new SqlStatementBuilder());

            // Assert
            trace.Verify(t => t.AfterDelete(It.IsAny<TraceLog>()), Times.Once);
        }

        // InlineInsert

        [TestMethod]
        public void TestDbConnectionTraceForBeforeInlineInsert()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.InlineInsert<TraceEntity>(new { Id = 1, Name = "Name" }, trace: trace.Object, statementBuilder: new SqlStatementBuilder());

            // Assert
            trace.Verify(t => t.BeforeInlineInsert(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterInlineInsert()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.InlineInsert<TraceEntity>(new { Id = 1, Name = "Name" }, trace: trace.Object, statementBuilder: new SqlStatementBuilder());

            // Assert
            trace.Verify(t => t.AfterInlineInsert(It.IsAny<TraceLog>()), Times.Once);
        }

        // InlineMerge

        [TestMethod]
        public void TestDbConnectionTraceForBeforeInlineMerge()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.InlineMerge<TraceEntity>(new { Id = 1, Name = "Name" }, trace: trace.Object, statementBuilder: new SqlStatementBuilder());

            // Assert
            trace.Verify(t => t.BeforeInlineMerge(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterInlineMerge()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.InlineMerge<TraceEntity>(new { Id = 1, Name = "Name" }, trace: trace.Object, statementBuilder: new SqlStatementBuilder());

            // Assert
            trace.Verify(t => t.AfterInlineMerge(It.IsAny<TraceLog>()), Times.Once);
        }

        // InlineUpdate

        [TestMethod]
        public void TestDbConnectionTraceForBeforeInlineUpdate()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.InlineUpdate<TraceEntity>(new { Name = "Name" }, te => te.Id == 1, trace: trace.Object, statementBuilder: new SqlStatementBuilder());

            // Assert
            trace.Verify(t => t.BeforeInlineUpdate(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterInlineUpdate()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.InlineUpdate<TraceEntity>(new { Name = "Name" }, te => te.Id == 1, trace: trace.Object, statementBuilder: new SqlStatementBuilder());

            // Assert
            trace.Verify(t => t.AfterInlineUpdate(It.IsAny<TraceLog>()), Times.Once);
        }

        // Insert

        [TestMethod]
        public void TestDbConnectionTraceForBeforeInsert()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.Insert<TraceEntity>(new TraceEntity { Name = "Name" }, trace: trace.Object, statementBuilder: new SqlStatementBuilder());

            // Assert
            trace.Verify(t => t.BeforeInsert(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterInsert()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.Insert<TraceEntity>(new TraceEntity { Name = "Name" }, trace: trace.Object, statementBuilder: new SqlStatementBuilder());

            // Assert
            trace.Verify(t => t.AfterInsert(It.IsAny<TraceLog>()), Times.Once);
        }

        // Merge

        [TestMethod]
        public void TestDbConnectionTraceForBeforeMerge()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.Merge<TraceEntity>(new TraceEntity { Id = 1, Name = "Name" }, trace: trace.Object, statementBuilder: new SqlStatementBuilder());

            // Assert
            trace.Verify(t => t.BeforeMerge(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterMerge()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.Merge<TraceEntity>(new TraceEntity { Id = 1, Name = "Name" }, trace: trace.Object, statementBuilder: new SqlStatementBuilder());

            // Assert
            trace.Verify(t => t.AfterMerge(It.IsAny<TraceLog>()), Times.Once);
        }

        // Query

        [TestMethod]
        public void TestDbConnectionTraceForBeforeQuery()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.Query<TraceEntity>(te => te.Id == 1, trace: trace.Object, statementBuilder: new SqlStatementBuilder());

            // Assert
            trace.Verify(t => t.BeforeQuery(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterQuery()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.Query<TraceEntity>(te => te.Id == 1, trace: trace.Object, statementBuilder: new SqlStatementBuilder());

            // Assert
            trace.Verify(t => t.AfterQuery(It.IsAny<TraceLog>()), Times.Once);
        }

        // QueryMultiple

        [TestMethod]
        public void TestDbConnectionTraceForBeforeQueryMultiple()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.QueryMultiple<TraceEntity, TraceEntity>(te => te.Id == 1, te => te.Id == 1, trace: trace.Object, statementBuilder: new SqlStatementBuilder());

            // Assert
            trace.Verify(t => t.BeforeQueryMultiple(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterQueryMultiple()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.QueryMultiple<TraceEntity, TraceEntity>(te => te.Id == 1, te => te.Id == 1, trace: trace.Object, statementBuilder: new SqlStatementBuilder());

            // Assert
            trace.Verify(t => t.AfterQueryMultiple(It.IsAny<TraceLog>()), Times.Once);
        }

        // Truncate

        [TestMethod]
        public void TestDbConnectionTraceForBeforeTruncate()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.Truncate<TraceEntity>(trace: trace.Object, statementBuilder: new SqlStatementBuilder());

            // Assert
            trace.Verify(t => t.BeforeTruncate(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterTruncate()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.Truncate<TraceEntity>(trace: trace.Object, statementBuilder: new SqlStatementBuilder());

            // Assert
            trace.Verify(t => t.AfterTruncate(It.IsAny<TraceLog>()), Times.Once);
        }

        // Update

        [TestMethod]
        public void TestDbConnectionTraceForBeforeUpdate()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.Update<TraceEntity>(new TraceEntity { Id = 1, Name = "Name" }, trace: trace.Object, statementBuilder: new SqlStatementBuilder());

            // Assert
            trace.Verify(t => t.BeforeUpdate(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterUpdate()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.Update<TraceEntity>(new TraceEntity { Id = 1, Name = "Name" }, trace: trace.Object, statementBuilder: new SqlStatementBuilder());

            // Assert
            trace.Verify(t => t.AfterUpdate(It.IsAny<TraceLog>()), Times.Once);
        }
    }
}
