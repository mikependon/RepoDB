using Moq;
using NUnit.Framework;
using RepoDb.Attributes;
using RepoDb.Interfaces;
using RepoDb.UnitTests.CustomObjects;

namespace RepoDb.UnitTests.Interfaces
{
    [TestFixture]
    public class ITraceTest
    {
        private class TraceEntity : DataEntity
        {
            [Primary, Identity]
            public int Id { get; set; }
            public string Name { get; set; }
        }

        // BatchQuery

        [Test]
        public void TestBeforeBatchQuery()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new Mock<DbRepository<CustomDbConnection>>("ConnectionString", trace.Object);

            // Setup
            trace.Setup(t => t.BeforeBatchQuery(It.IsAny<CancellableTraceLog>()));

            // Act
            repository.Object.BatchQuery<TraceEntity>(0, 10, null, null);

            // Assert
            trace.Verify(t => t.BeforeBatchQuery(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [Test]
        public void TestAfterBatchQuery()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new Mock<DbRepository<CustomDbConnection>>("ConnectionString", trace.Object);

            // Setup
            trace.Setup(t => t.AfterBatchQuery(It.IsAny<TraceLog>()));

            // Act
            repository.Object.BatchQuery<TraceEntity>(0, 10, null, null);

            // Assert
            trace.Verify(t => t.AfterBatchQuery(It.IsAny<TraceLog>()), Times.Once);
        }

        // Count

        [Test]
        public void TestBeforeBulkInsert()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new Mock<DbRepository<CustomDbConnection>>("ConnectionString", trace.Object);

            // Setup
            trace.Setup(t => t.BeforeCount(It.IsAny<CancellableTraceLog>()));

            // Act
            repository.Object.Count<TraceEntity>();

            // Assert
            trace.Verify(t => t.BeforeCount(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [Test]
        public void TestAfterCount()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new Mock<DbRepository<CustomDbConnection>>("ConnectionString", trace.Object);

            // Setup
            trace.Setup(t => t.AfterCount(It.IsAny<TraceLog>()));

            // Act
            repository.Object.Count<TraceEntity>();

            // Assert
            trace.Verify(t => t.AfterCount(It.IsAny<TraceLog>()), Times.Once);
        }

        // Delete

        [Test]
        public void TestBeforeDelete()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new Mock<DbRepository<CustomDbConnection>>("ConnectionString", trace.Object);

            // Setup
            trace.Setup(t => t.BeforeDelete(It.IsAny<CancellableTraceLog>()));

            // Act
            repository.Object.Delete<TraceEntity>();

            // Assert
            trace.Verify(t => t.BeforeDelete(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [Test]
        public void TestAfterDelete()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new Mock<DbRepository<CustomDbConnection>>("ConnectionString", trace.Object);

            // Setup
            trace.Setup(t => t.AfterDelete(It.IsAny<TraceLog>()));

            // Act
            repository.Object.Delete<TraceEntity>();

            // Assert
            trace.Verify(t => t.AfterDelete(It.IsAny<TraceLog>()), Times.Once);
        }

        // InlineInsert

        [Test]
        public void TestBeforeInlineInsert()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new Mock<DbRepository<CustomDbConnection>>("ConnectionString", trace.Object);

            // Setup
            trace.Setup(t => t.BeforeInlineInsert(It.IsAny<CancellableTraceLog>()));

            // Act
            repository.Object.InlineInsert<TraceEntity>(new { Id = 1, Name = "Name" });

            // Assert
            trace.Verify(t => t.BeforeInlineInsert(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [Test]
        public void TestAfterInlineInsert()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new Mock<DbRepository<CustomDbConnection>>("ConnectionString", trace.Object);

            // Setup
            trace.Setup(t => t.AfterInlineInsert(It.IsAny<CancellableTraceLog>()));

            // Act
            repository.Object.InlineInsert<TraceEntity>(new { Id = 1, Name = "Name" });

            // Assert
            trace.Verify(t => t.AfterInlineInsert(It.IsAny<TraceLog>()), Times.Once);
        }

        // InlineMerge

        [Test]
        public void TestBeforeInlineMerge()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new Mock<DbRepository<CustomDbConnection>>("ConnectionString", trace.Object);

            // Setup
            trace.Setup(t => t.BeforeInlineMerge(It.IsAny<CancellableTraceLog>()));

            // Act
            repository.Object.InlineMerge<TraceEntity>(new { Id = 1, Name = "Name" });

            // Assert
            trace.Verify(t => t.BeforeInlineMerge(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [Test]
        public void TestAfterInlineMerge()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new Mock<DbRepository<CustomDbConnection>>("ConnectionString", trace.Object);

            // Setup
            trace.Setup(t => t.AfterInlineMerge(It.IsAny<CancellableTraceLog>()));

            // Act
            repository.Object.InlineMerge<TraceEntity>(new { Id = 1, Name = "Name" });

            // Assert
            trace.Verify(t => t.AfterInlineMerge(It.IsAny<TraceLog>()), Times.Once);
        }

        // InlineUpdate

        [Test]
        public void TestBeforeInlineUpdate()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new Mock<DbRepository<CustomDbConnection>>("ConnectionString", trace.Object);

            // Setup
            trace.Setup(t => t.BeforeInlineUpdate(It.IsAny<CancellableTraceLog>()));

            // Act
            repository.Object.InlineUpdate<TraceEntity>(new { Name = "Name" }, new { Id = 1 });

            // Assert
            trace.Verify(t => t.BeforeInlineUpdate(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [Test]
        public void TestAfterInlineUpdate()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new Mock<DbRepository<CustomDbConnection>>("ConnectionString", trace.Object);

            // Setup
            trace.Setup(t => t.AfterInlineUpdate(It.IsAny<CancellableTraceLog>()));

            // Act
            repository.Object.InlineUpdate<TraceEntity>(new { Name = "Name" }, new { Id = 1 });

            // Assert
            trace.Verify(t => t.AfterInlineUpdate(It.IsAny<TraceLog>()), Times.Once);
        }

        // Insert

        [Test]
        public void TestBeforeInsert()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new Mock<DbRepository<CustomDbConnection>>("ConnectionString", trace.Object);

            // Setup
            trace.Setup(t => t.BeforeInsert(It.IsAny<CancellableTraceLog>()));

            // Act
            repository.Object.Insert<TraceEntity>(new TraceEntity { Name = "Name" });

            // Assert
            trace.Verify(t => t.BeforeInsert(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [Test]
        public void TestAfterInsert()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new Mock<DbRepository<CustomDbConnection>>("ConnectionString", trace.Object);

            // Setup
            trace.Setup(t => t.AfterInsert(It.IsAny<CancellableTraceLog>()));

            // Act
            repository.Object.Insert<TraceEntity>(new TraceEntity { Name = "Name" });

            // Assert
            trace.Verify(t => t.AfterInsert(It.IsAny<TraceLog>()), Times.Once);
        }

        // Merge

        [Test]
        public void TestBeforeMerge()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new Mock<DbRepository<CustomDbConnection>>("ConnectionString", trace.Object);

            // Setup
            trace.Setup(t => t.BeforeMerge(It.IsAny<CancellableTraceLog>()));

            // Act
            repository.Object.Merge<TraceEntity>(new TraceEntity { Id = 1, Name = "Name" });

            // Assert
            trace.Verify(t => t.BeforeMerge(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [Test]
        public void TestAfterMerge()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new Mock<DbRepository<CustomDbConnection>>("ConnectionString", trace.Object);

            // Setup
            trace.Setup(t => t.AfterMerge(It.IsAny<CancellableTraceLog>()));

            // Act
            repository.Object.Merge<TraceEntity>(new TraceEntity { Id = 1, Name = "Name" });

            // Assert
            trace.Verify(t => t.AfterMerge(It.IsAny<TraceLog>()), Times.Once);
        }

        // Query

        [Test]
        public void TestBeforeQuery()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new Mock<DbRepository<CustomDbConnection>>("ConnectionString", trace.Object);

            // Setup
            trace.Setup(t => t.BeforeQuery(It.IsAny<CancellableTraceLog>()));

            // Act
            repository.Object.Query<TraceEntity>(new { Id = 1 });

            // Assert
            trace.Verify(t => t.BeforeQuery(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [Test]
        public void TestAfterQuery()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new Mock<DbRepository<CustomDbConnection>>("ConnectionString", trace.Object);

            // Setup
            trace.Setup(t => t.AfterQuery(It.IsAny<CancellableTraceLog>()));

            // Act
            repository.Object.Query<TraceEntity>(new TraceEntity { Id = 1 });

            // Assert
            trace.Verify(t => t.AfterQuery(It.IsAny<TraceLog>()), Times.Once);
        }

        // Truncate

        [Test]
        public void TestBeforeTruncate()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new Mock<DbRepository<CustomDbConnection>>("ConnectionString", trace.Object);

            // Setup
            trace.Setup(t => t.BeforeTruncate(It.IsAny<CancellableTraceLog>()));

            // Act
            repository.Object.Truncate<TraceEntity>();

            // Assert
            trace.Verify(t => t.BeforeTruncate(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [Test]
        public void TestAfterTruncate()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new Mock<DbRepository<CustomDbConnection>>("ConnectionString", trace.Object);

            // Setup
            trace.Setup(t => t.AfterTruncate(It.IsAny<CancellableTraceLog>()));

            // Act
            repository.Object.Truncate<TraceEntity>();

            // Assert
            trace.Verify(t => t.AfterTruncate(It.IsAny<TraceLog>()), Times.Once);
        }

        // Update

        [Test]
        public void TestBeforeUpdate()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new Mock<DbRepository<CustomDbConnection>>("ConnectionString", trace.Object);

            // Setup
            trace.Setup(t => t.BeforeUpdate(It.IsAny<CancellableTraceLog>()));

            // Act
            repository.Object.Update<TraceEntity>(new TraceEntity { Id = 1, Name = "Name" });

            // Assert
            trace.Verify(t => t.BeforeUpdate(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [Test]
        public void TestAfterUpdate()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new Mock<DbRepository<CustomDbConnection>>("ConnectionString", trace.Object);

            // Setup
            trace.Setup(t => t.AfterUpdate(It.IsAny<CancellableTraceLog>()));

            // Act
            repository.Object.Update<TraceEntity>(new TraceEntity { Id = 1, Name = "Name" });

            // Assert
            trace.Verify(t => t.AfterUpdate(It.IsAny<TraceLog>()), Times.Once);
        }
    }
}
