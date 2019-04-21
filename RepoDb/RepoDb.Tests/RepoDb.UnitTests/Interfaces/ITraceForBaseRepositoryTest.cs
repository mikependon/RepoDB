using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoDb.Attributes;
using RepoDb.Enumerations;
using RepoDb.Interfaces;
using RepoDb.UnitTests.CustomObjects;

namespace RepoDb.UnitTests.Interfaces
{
    [TestClass]
    public class ITraceForBaseRepositoryTest
    {
        public class TraceEntity
        {
            [Primary, Identity]
            public int Id { get; set; }
            public string Name { get; set; }
        }

        // BatchQuery

        [TestMethod]
        public void TestBaseRepositoryTraceForBeforeBatchQuery()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new Mock<BaseRepository<TraceEntity, CustomDbConnection>>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                new SqlStatementBuilder());

            // Act
            repository.Object.BatchQuery(0,
                10,
                OrderField.Parse(new { Id = Order.Ascending }),
                (object)null);

            // Assert
            trace.Verify(t => t.BeforeBatchQuery(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestBaseRepositoryTraceForAfterBatchQuery()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new Mock<BaseRepository<TraceEntity, CustomDbConnection>>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                new SqlStatementBuilder());

            // Act
            repository.Object.BatchQuery(0,
                10,
                OrderField.Parse(new { Id = Order.Ascending }),
                (object)null);

            // Assert
            trace.Verify(t => t.AfterBatchQuery(It.IsAny<TraceLog>()), Times.Once);
        }

        // Count

        [TestMethod]
        public void TestBaseRepositoryTraceForBeforeCount()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new Mock<BaseRepository<TraceEntity, CustomDbConnection>>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                new SqlStatementBuilder());

            // Act
            repository.Object.Count((object)null);

            // Assert
            trace.Verify(t => t.BeforeCount(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestBaseRepositoryTraceForAfterCount()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new Mock<BaseRepository<TraceEntity, CustomDbConnection>>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                new SqlStatementBuilder());

            // Act
            repository.Object.Count((object)null);

            // Assert
            trace.Verify(t => t.AfterCount(It.IsAny<TraceLog>()), Times.Once);
        }

        // CountAll

        [TestMethod]
        public void TestBaseRepositoryTraceForBeforeCountAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new Mock<BaseRepository<TraceEntity, CustomDbConnection>>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                new SqlStatementBuilder());

            // Act
            repository.Object.CountAll();

            // Assert
            trace.Verify(t => t.BeforeCountAll(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestBaseRepositoryTraceForAfterCountAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new Mock<BaseRepository<TraceEntity, CustomDbConnection>>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                new SqlStatementBuilder());

            // Act
            repository.Object.CountAll();

            // Assert
            trace.Verify(t => t.AfterCountAll(It.IsAny<TraceLog>()), Times.Once);
        }

        // Delete

        [TestMethod]
        public void TestBaseRepositoryTraceForBeforeDelete()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new Mock<BaseRepository<TraceEntity, CustomDbConnection>>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                new SqlStatementBuilder());

            // Act
            repository.Object.Delete(0);

            // Assert
            trace.Verify(t => t.BeforeDelete(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestBaseRepositoryTraceForAfterDelete()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new Mock<BaseRepository<TraceEntity, CustomDbConnection>>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                new SqlStatementBuilder());

            // Act
            repository.Object.Delete(0);

            // Assert
            trace.Verify(t => t.AfterDelete(It.IsAny<TraceLog>()), Times.Once);
        }

        // Insert

        [TestMethod]
        public void TestBaseRepositoryTraceForBeforeInsert()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new Mock<BaseRepository<TraceEntity, CustomDbConnection>>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                new SqlStatementBuilder());

            // Act
            repository.Object.Insert(new TraceEntity
            {
                Name = "Name"
            });

            // Assert
            trace.Verify(t => t.BeforeInsert(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestBaseRepositoryTraceForAfterInsert()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new Mock<BaseRepository<TraceEntity, CustomDbConnection>>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                new SqlStatementBuilder());

            // Act
            repository.Object.Insert(new TraceEntity
            {
                Name = "Name"
            });

            // Assert
            trace.Verify(t => t.AfterInsert(It.IsAny<TraceLog>()), Times.Once);
        }

        // Merge

        [TestMethod]
        public void TestBaseRepositoryTraceForBeforeMerge()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new Mock<BaseRepository<TraceEntity, CustomDbConnection>>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                new SqlStatementBuilder());

            // Act
            repository.Object.Merge(new TraceEntity
            {
                Id = 1,
                Name = "Name"
            });

            // Assert
            trace.Verify(t => t.BeforeMerge(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestBaseRepositoryTraceForAfterMerge()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new Mock<BaseRepository<TraceEntity, CustomDbConnection>>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                new SqlStatementBuilder());

            // Act
            repository.Object.Merge(new TraceEntity
            {
                Id = 1,
                Name = "Name"
            });

            // Assert
            trace.Verify(t => t.AfterMerge(It.IsAny<TraceLog>()), Times.Once);
        }

        // Query

        [TestMethod]
        public void TestBaseRepositoryTraceForBeforeQuery()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new Mock<BaseRepository<TraceEntity, CustomDbConnection>>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                new SqlStatementBuilder());

            // Act
            repository.Object.Query(te => te.Id == 1);

            // Assert
            trace.Verify(t => t.BeforeQuery(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestBaseRepositoryTraceForAfterQuery()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new Mock<BaseRepository<TraceEntity, CustomDbConnection>>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                new SqlStatementBuilder());

            // Act
            repository.Object.Query(te => te.Id == 1);

            // Assert
            trace.Verify(t => t.AfterQuery(It.IsAny<TraceLog>()), Times.Once);
        }

        // Truncate

        [TestMethod]
        public void TestBaseRepositoryTraceForBeforeTruncate()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new Mock<BaseRepository<TraceEntity, CustomDbConnection>>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                new SqlStatementBuilder());

            // Act
            repository.Object.Truncate();

            // Assert
            trace.Verify(t => t.BeforeTruncate(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestBaseRepositoryTraceForAfterTruncate()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new Mock<BaseRepository<TraceEntity, CustomDbConnection>>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                new SqlStatementBuilder());

            // Act
            repository.Object.Truncate();

            // Assert
            trace.Verify(t => t.AfterTruncate(It.IsAny<TraceLog>()), Times.Once);
        }

        // Update

        [TestMethod]
        public void TestBaseRepositoryTraceForBeforeUpdate()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new Mock<BaseRepository<TraceEntity, CustomDbConnection>>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                new SqlStatementBuilder());

            // Act
            repository.Object.Update(
                new TraceEntity
                {
                    Id = 1,
                    Name = "Name"
                },
                whereOrPrimaryKey: 1);

            // Assert
            trace.Verify(t => t.BeforeUpdate(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestBaseRepositoryTraceForAfterUpdate()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new Mock<BaseRepository<TraceEntity, CustomDbConnection>>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                new SqlStatementBuilder());

            // Act
            repository.Object.Update(
                new TraceEntity
                {
                    Id = 1,
                    Name = "Name"
                },
                whereOrPrimaryKey: 1);

            // Assert
            trace.Verify(t => t.AfterUpdate(It.IsAny<TraceLog>()), Times.Once);
        }
    }
}
