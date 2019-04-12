using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoDb.Attributes;
using RepoDb.Interfaces;
using RepoDb.UnitTests.CustomObjects;

namespace RepoDb.UnitTests.Interfaces
{
    [TestClass]
    public class ITraceForDbRepositoryTest
    {
        public class TraceEntity
        {
            [Primary, Identity]
            public int Id { get; set; }
            public string Name { get; set; }
        }

        // BatchQuery

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeBatchQuery()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new Mock<DbRepository<CustomDbConnection>>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                new SqlStatementBuilder());

            // Setup
            trace.Setup(t => t.BeforeBatchQuery(It.IsAny<CancellableTraceLog>()));

            // Act
            repository.Object.BatchQuery<TraceEntity>(0, 10, null, null);

            // Assert
            trace.Verify(t => t.BeforeBatchQuery(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterBatchQuery()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new Mock<DbRepository<CustomDbConnection>>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                new SqlStatementBuilder());

            // Setup
            trace.Setup(t => t.AfterBatchQuery(It.IsAny<TraceLog>()));

            // Act
            repository.Object.BatchQuery<TraceEntity>(0, 10, null, null);

            // Assert
            trace.Verify(t => t.AfterBatchQuery(It.IsAny<TraceLog>()), Times.Once);
        }

        // Count

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeBulkInsert()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new Mock<DbRepository<CustomDbConnection>>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                new SqlStatementBuilder());

            // Setup
            trace.Setup(t => t.BeforeCount(It.IsAny<CancellableTraceLog>()));

            // Act
            repository.Object.Count<TraceEntity>();

            // Assert
            trace.Verify(t => t.BeforeCount(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterCount()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new Mock<DbRepository<CustomDbConnection>>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                new SqlStatementBuilder());

            // Setup
            trace.Setup(t => t.AfterCount(It.IsAny<TraceLog>()));

            // Act
            repository.Object.Count<TraceEntity>();

            // Assert
            trace.Verify(t => t.AfterCount(It.IsAny<TraceLog>()), Times.Once);
        }

        // Delete

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeDelete()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new Mock<DbRepository<CustomDbConnection>>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                new SqlStatementBuilder());

            // Setup
            trace.Setup(t => t.BeforeDelete(It.IsAny<CancellableTraceLog>()));

            // Act
            repository.Object.Delete<TraceEntity>(0);

            // Assert
            trace.Verify(t => t.BeforeDelete(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterDelete()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new Mock<DbRepository<CustomDbConnection>>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                new SqlStatementBuilder());

            // Setup
            trace.Setup(t => t.AfterDelete(It.IsAny<TraceLog>()));

            // Act
            repository.Object.Delete<TraceEntity>(0);

            // Assert
            trace.Verify(t => t.AfterDelete(It.IsAny<TraceLog>()), Times.Once);
        }

        // InlineInsert

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeInlineInsert()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new Mock<DbRepository<CustomDbConnection>>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                new SqlStatementBuilder());

            // Setup
            trace.Setup(t => t.BeforeInlineInsert(It.IsAny<CancellableTraceLog>()));

            // Act
            repository.Object.InlineInsert<TraceEntity>(new { Id = 1, Name = "Name" });

            // Assert
            trace.Verify(t => t.BeforeInlineInsert(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterInlineInsert()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new Mock<DbRepository<CustomDbConnection>>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                new SqlStatementBuilder());

            // Setup
            trace.Setup(t => t.AfterInlineInsert(It.IsAny<CancellableTraceLog>()));

            // Act
            repository.Object.InlineInsert<TraceEntity>(new { Id = 1, Name = "Name" });

            // Assert
            trace.Verify(t => t.AfterInlineInsert(It.IsAny<TraceLog>()), Times.Once);
        }

        // InlineMerge

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeInlineMerge()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new Mock<DbRepository<CustomDbConnection>>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                new SqlStatementBuilder());

            // Setup
            trace.Setup(t => t.BeforeInlineMerge(It.IsAny<CancellableTraceLog>()));

            // Act
            repository.Object.InlineMerge<TraceEntity>(new { Id = 1, Name = "Name" });

            // Assert
            trace.Verify(t => t.BeforeInlineMerge(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterInlineMerge()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new Mock<DbRepository<CustomDbConnection>>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                new SqlStatementBuilder());

            // Setup
            trace.Setup(t => t.AfterInlineMerge(It.IsAny<CancellableTraceLog>()));

            // Act
            repository.Object.InlineMerge<TraceEntity>(new { Id = 1, Name = "Name" });

            // Assert
            trace.Verify(t => t.AfterInlineMerge(It.IsAny<TraceLog>()), Times.Once);
        }

        // InlineUpdate

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeInlineUpdate()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new Mock<DbRepository<CustomDbConnection>>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                new SqlStatementBuilder());

            // Setup
            trace.Setup(t => t.BeforeInlineUpdate(It.IsAny<CancellableTraceLog>()));

            // Act
            repository.Object.InlineUpdate<TraceEntity>(new { Name = "Name" }, te => te.Id == 1);

            // Assert
            trace.Verify(t => t.BeforeInlineUpdate(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterInlineUpdate()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new Mock<DbRepository<CustomDbConnection>>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                new SqlStatementBuilder());

            // Setup
            trace.Setup(t => t.AfterInlineUpdate(It.IsAny<CancellableTraceLog>()));

            // Act
            repository.Object.InlineUpdate<TraceEntity>(new { Name = "Name" }, te => te.Id == 1);

            // Assert
            trace.Verify(t => t.AfterInlineUpdate(It.IsAny<TraceLog>()), Times.Once);
        }

        // Insert

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeInsert()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new Mock<DbRepository<CustomDbConnection>>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                new SqlStatementBuilder());

            // Setup
            trace.Setup(t => t.BeforeInsert(It.IsAny<CancellableTraceLog>()));

            // Act
            repository.Object.Insert<TraceEntity>(new TraceEntity { Name = "Name" });

            // Assert
            trace.Verify(t => t.BeforeInsert(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterInsert()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new Mock<DbRepository<CustomDbConnection>>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                new SqlStatementBuilder());

            // Setup
            trace.Setup(t => t.AfterInsert(It.IsAny<CancellableTraceLog>()));

            // Act
            repository.Object.Insert<TraceEntity>(new TraceEntity { Name = "Name" });

            // Assert
            trace.Verify(t => t.AfterInsert(It.IsAny<TraceLog>()), Times.Once);
        }

        // Merge

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeMerge()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new Mock<DbRepository<CustomDbConnection>>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                new SqlStatementBuilder());

            // Setup
            trace.Setup(t => t.BeforeMerge(It.IsAny<CancellableTraceLog>()));

            // Act
            repository.Object.Merge<TraceEntity>(new TraceEntity { Id = 1, Name = "Name" });

            // Assert
            trace.Verify(t => t.BeforeMerge(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterMerge()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new Mock<DbRepository<CustomDbConnection>>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                new SqlStatementBuilder());

            // Setup
            trace.Setup(t => t.AfterMerge(It.IsAny<CancellableTraceLog>()));

            // Act
            repository.Object.Merge<TraceEntity>(new TraceEntity { Id = 1, Name = "Name" });

            // Assert
            trace.Verify(t => t.AfterMerge(It.IsAny<TraceLog>()), Times.Once);
        }

        // Query

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeQuery()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new Mock<DbRepository<CustomDbConnection>>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                new SqlStatementBuilder());

            // Setup
            trace.Setup(t => t.BeforeQuery(It.IsAny<CancellableTraceLog>()));

            // Act
            repository.Object.Query<TraceEntity>(te => te.Id == 1);

            // Assert
            trace.Verify(t => t.BeforeQuery(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterQuery()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new Mock<DbRepository<CustomDbConnection>>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                new SqlStatementBuilder());

            // Setup
            trace.Setup(t => t.AfterQuery(It.IsAny<CancellableTraceLog>()));

            // Act
            repository.Object.Query<TraceEntity>(te => te.Id == 1);

            // Assert
            trace.Verify(t => t.AfterQuery(It.IsAny<TraceLog>()), Times.Once);
        }

        // QueryMultiple

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeQueryMultiple()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new Mock<DbRepository<CustomDbConnection>>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                new SqlStatementBuilder());

            // Setup
            trace.Setup(t => t.BeforeQueryMultiple(It.IsAny<CancellableTraceLog>()));

            // Act
            repository.Object.QueryMultiple<TraceEntity, TraceEntity>(te => te.Id == 1, te => te.Id == 1);

            // Assert
            trace.Verify(t => t.BeforeQueryMultiple(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterQueryMultiple()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new Mock<DbRepository<CustomDbConnection>>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                new SqlStatementBuilder());

            // Setup
            trace.Setup(t => t.AfterQueryMultiple(It.IsAny<CancellableTraceLog>()));

            // Act
            repository.Object.QueryMultiple<TraceEntity, TraceEntity>(te => te.Id == 1, te => te.Id == 1);

            // Assert
            trace.Verify(t => t.AfterQueryMultiple(It.IsAny<TraceLog>()), Times.Once);
        }

        // Truncate

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeTruncate()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new Mock<DbRepository<CustomDbConnection>>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                new SqlStatementBuilder());

            // Setup
            trace.Setup(t => t.BeforeTruncate(It.IsAny<CancellableTraceLog>()));

            // Act
            repository.Object.Truncate<TraceEntity>();

            // Assert
            trace.Verify(t => t.BeforeTruncate(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterTruncate()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new Mock<DbRepository<CustomDbConnection>>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                new SqlStatementBuilder());

            // Setup
            trace.Setup(t => t.AfterTruncate(It.IsAny<CancellableTraceLog>()));

            // Act
            repository.Object.Truncate<TraceEntity>();

            // Assert
            trace.Verify(t => t.AfterTruncate(It.IsAny<TraceLog>()), Times.Once);
        }

        // Update

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeUpdate()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new Mock<DbRepository<CustomDbConnection>>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                new SqlStatementBuilder());

            // Setup
            trace.Setup(t => t.BeforeUpdate(It.IsAny<CancellableTraceLog>()));

            // Act
            repository.Object.Update<TraceEntity>(new TraceEntity { Id = 1, Name = "Name" });

            // Assert
            trace.Verify(t => t.BeforeUpdate(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterUpdate()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new Mock<DbRepository<CustomDbConnection>>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                new SqlStatementBuilder());

            // Setup
            trace.Setup(t => t.AfterUpdate(It.IsAny<CancellableTraceLog>()));

            // Act
            repository.Object.Update<TraceEntity>(new TraceEntity { Id = 1, Name = "Name" });

            // Assert
            trace.Verify(t => t.AfterUpdate(It.IsAny<TraceLog>()), Times.Once);
        }
    }
}
