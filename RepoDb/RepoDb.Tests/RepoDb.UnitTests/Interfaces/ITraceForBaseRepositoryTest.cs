using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoDb.Attributes;
using RepoDb.Enumerations;
using RepoDb.Interfaces;
using RepoDb.UnitTests.CustomObjects;
using System;
using System.Collections.Generic;

namespace RepoDb.UnitTests.Interfaces
{
    [TestClass]
    public class ITraceForBaseRepositoryTest
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            DbHelperMapper.Add(typeof(CustomDbConnection), new BaseRepositoryCustomDbHelper(), true);
        }

        #region SubClasses

        private class TraceEntity
        {
            [Primary, Identity]
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private class TraceEntityRepository : BaseRepository<TraceEntity, CustomDbConnection>
        {
            public TraceEntityRepository(ITrace trace) :
                base("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace,
                new SqlStatementBuilder())
            { }
        }

        private class BaseRepositoryCustomDbHelper : IDbHelper
        {
            public IResolver<string, Type> DbTypeResolver => throw new NotImplementedException();

            public IEnumerable<DbField> GetFields(string connectionString, string tableName)
            {
                if (tableName == ClassMappedNameCache.Get<TraceEntity>())
                {
                    return new[]
                    {
                        new DbField("Id", true, true, false),
                        new DbField("Name", false, false, true)
                    };
                }
                return null;
            }
        }

        #endregion

        #region BatchQuery

        #region BatchQuery

        [TestMethod]
        public void TestBaseRepositoryTraceForBeforeBatchQuery()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.BatchQuery(0,
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
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.BatchQuery(0,
                10,
                OrderField.Parse(new { Id = Order.Ascending }),
                (object)null);

            // Assert
            trace.Verify(t => t.AfterBatchQuery(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #region BatchQueryAsnc

        [TestMethod]
        public void TestBaseRepositoryTraceForBeforeBatchQueryAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.BatchQueryAsync(0,
                10,
                OrderField.Parse(new { Id = Order.Ascending }),
                (object)null);

            // Assert
            trace.Verify(t => t.BeforeBatchQuery(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestBaseRepositoryTraceForAfterBatchQueryAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.BatchQueryAsync(0,
                10,
                OrderField.Parse(new { Id = Order.Ascending }),
                (object)null);

            // Assert
            trace.Verify(t => t.AfterBatchQuery(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #endregion

        #region Count

        #region Count

        [TestMethod]
        public void TestBaseRepositoryTraceForBeforeCount()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.Count((object)null);

            // Assert
            trace.Verify(t => t.BeforeCount(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestBaseRepositoryTraceForAfterCount()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.Count((object)null);

            // Assert
            trace.Verify(t => t.AfterCount(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #region CountAsync

        [TestMethod]
        public void TestBaseRepositoryTraceForBeforeCountAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.CountAsync((object)null);

            // Assert
            trace.Verify(t => t.BeforeCount(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestBaseRepositoryTraceForAfterCountAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.CountAsync((object)null);

            // Assert
            trace.Verify(t => t.AfterCount(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #endregion

        #region CountAll

        #region CountAll

        [TestMethod]
        public void TestBaseRepositoryTraceForBeforeCountAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.CountAll();

            // Assert
            trace.Verify(t => t.BeforeCountAll(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestBaseRepositoryTraceForAfterCountAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.CountAll();

            // Assert
            trace.Verify(t => t.AfterCountAll(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #region CountAllAsync

        [TestMethod]
        public void TestBaseRepositoryTraceForBeforeCountAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.CountAllAsync();

            // Assert
            trace.Verify(t => t.BeforeCountAll(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestBaseRepositoryTraceForAfterCountAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.CountAllAsync();

            // Assert
            trace.Verify(t => t.AfterCountAll(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #endregion

        #region Delete

        #region Delete

        [TestMethod]
        public void TestBaseRepositoryTraceForBeforeDelete()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.Delete(0);

            // Assert
            trace.Verify(t => t.BeforeDelete(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestBaseRepositoryTraceForAfterDelete()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.Delete(0);

            // Assert
            trace.Verify(t => t.AfterDelete(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #region DeleteAsync

        [TestMethod]
        public void TestBaseRepositoryTraceForBeforeDeleteAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.DeleteAsync(0);

            // Assert
            trace.Verify(t => t.BeforeDelete(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestBaseRepositoryTraceForAfterDeleteAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.DeleteAsync(0);

            // Assert
            trace.Verify(t => t.AfterDelete(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #endregion

        #region Insert

        #region Insert

        [TestMethod]
        public void TestBaseRepositoryTraceForBeforeInsert()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.Insert(new TraceEntity
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
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.Insert(new TraceEntity
            {
                Name = "Name"
            });

            // Assert
            trace.Verify(t => t.AfterInsert(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #region InsertAsync

        [TestMethod]
        public void TestBaseRepositoryTraceForBeforeInsertAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.InsertAsync(new TraceEntity
            {
                Name = "Name"
            });

            // Assert
            trace.Verify(t => t.BeforeInsert(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestBaseRepositoryTraceForAfterInsertAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.InsertAsync(new TraceEntity
            {
                Name = "Name"
            });

            // Assert
            trace.Verify(t => t.AfterInsert(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #endregion

        #region InsertAll

        #region InsertAll

        [TestMethod]
        public void TestBaseRepositoryTraceForBeforeInsertAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.InsertAll(new[] { new TraceEntity { Name = "Name" } });

            // Assert
            trace.Verify(t => t.BeforeInsertAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestBaseRepositoryTraceForAfterInsertAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.InsertAll(new[] { new TraceEntity { Name = "Name" } });

            // Assert
            trace.Verify(t => t.AfterInsertAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #region InsertAllAsync

        [TestMethod]
        public void TestBaseRepositoryTraceForBeforeInsertAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.InsertAllAsync(new[] { new TraceEntity { Name = "Name" } });

            // Assert
            trace.Verify(t => t.BeforeInsertAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestBaseRepositoryTraceForAfterInsertAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.InsertAll(new[] { new TraceEntity { Name = "Name" } });

            // Assert
            trace.Verify(t => t.AfterInsertAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #endregion

        #region Merge

        #region Merge

        [TestMethod]
        public void TestBaseRepositoryTraceForBeforeMerge()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.Merge(new TraceEntity
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
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.Merge(new TraceEntity
            {
                Id = 1,
                Name = "Name"
            });

            // Assert
            trace.Verify(t => t.AfterMerge(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #region MergeAsync

        [TestMethod]
        public void TestBaseRepositoryTraceForBeforeMergeAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.MergeAsync(new TraceEntity
            {
                Id = 1,
                Name = "Name"
            });

            // Assert
            trace.Verify(t => t.BeforeMerge(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestBaseRepositoryTraceForAfterMergeAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.MergeAsync(new TraceEntity
            {
                Id = 1,
                Name = "Name"
            });

            // Assert
            trace.Verify(t => t.AfterMerge(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #endregion

        #region Query

        #region Query

        [TestMethod]
        public void TestBaseRepositoryTraceForBeforeQuery()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.Query(te => te.Id == 1);

            // Assert
            trace.Verify(t => t.BeforeQuery(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestBaseRepositoryTraceForAfterQuery()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.Query(te => te.Id == 1);

            // Assert
            trace.Verify(t => t.AfterQuery(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #region QueryAsync

        [TestMethod]
        public void TestBaseRepositoryTraceForBeforeQueryAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.QueryAsync(te => te.Id == 1);

            // Assert
            trace.Verify(t => t.BeforeQuery(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestBaseRepositoryTraceForAfterQueryAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.QueryAsync(te => te.Id == 1);

            // Assert
            trace.Verify(t => t.AfterQuery(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #endregion

        #region QueryAll

        #region QueryAll

        [TestMethod]
        public void TestBaseRepositoryTraceForBeforeQueryAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.QueryAll();

            // Assert
            trace.Verify(t => t.BeforeQueryAll(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestBaseRepositoryTraceForAfterQueryAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.QueryAll();

            // Assert
            trace.Verify(t => t.AfterQueryAll(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #region QueryAllAsync

        [TestMethod]
        public void TestBaseRepositoryTraceForBeforeQueryAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.QueryAllAsync();

            // Assert
            trace.Verify(t => t.BeforeQueryAll(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestBaseRepositoryTraceForAfterQueryAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.QueryAllAsync();

            // Assert
            trace.Verify(t => t.AfterQueryAll(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #endregion

        #region Truncate

        #region Truncate

        [TestMethod]
        public void TestBaseRepositoryTraceForBeforeTruncate()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.Truncate();

            // Assert
            trace.Verify(t => t.BeforeTruncate(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestBaseRepositoryTraceForAfterTruncate()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.Truncate();

            // Assert
            trace.Verify(t => t.AfterTruncate(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #region TruncateAsync

        [TestMethod]
        public void TestBaseRepositoryTraceForBeforeTruncateAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.TruncateAsync();

            // Assert
            trace.Verify(t => t.BeforeTruncate(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestBaseRepositoryTraceForAfterTruncateAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.TruncateAsync();

            // Assert
            trace.Verify(t => t.AfterTruncate(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #endregion

        #region Update

        #region Update

        [TestMethod]
        public void TestBaseRepositoryTraceForBeforeUpdate()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.Update(
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
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.Update(
                new TraceEntity
                {
                    Id = 1,
                    Name = "Name"
                },
                whereOrPrimaryKey: 1);

            // Assert
            trace.Verify(t => t.AfterUpdate(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #region UpdateAsync

        [TestMethod]
        public void TestBaseRepositoryTraceForBeforeUpdateAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.UpdateAsync(
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
        public void TestBaseRepositoryTraceForAfterUpdateAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new TraceEntityRepository(trace.Object);

            // Act
            repository.UpdateAsync(
                new TraceEntity
                {
                    Id = 1,
                    Name = "Name"
                },
                whereOrPrimaryKey: 1);

            // Assert
            trace.Verify(t => t.AfterUpdate(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #endregion
    }
}
