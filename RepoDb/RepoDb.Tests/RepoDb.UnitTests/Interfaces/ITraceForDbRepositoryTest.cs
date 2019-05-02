using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoDb.Attributes;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.UnitTests.CustomObjects;
using System;
using System.Collections.Generic;

namespace RepoDb.UnitTests.Interfaces
{
    [TestClass]
    public class ITraceForDbRepositoryTest
    {
        private readonly IStatementBuilder m_statementBuilder = new SqlStatementBuilder();

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            DbHelperMapper.Add(typeof(CustomDbConnection), new DbRepositoryCustomDbHelper(), true);
        }

        #region SubClasses

        private class TraceEntity
        {
            [Primary, Identity]
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private class DbRepositoryCustomDbHelper : IDbHelper
        {
            public IResolver<string, Type> DbTypeResolver { get; set; }

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
        public void TestDbRepositoryTraceForBeforeBatchQuery()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.BatchQuery<TraceEntity>(0,
                10,
                OrderField.Ascending<TraceEntity>(t => t.Id).AsEnumerable(),
                (object)null);

            // Assert
            trace.Verify(t => t.BeforeBatchQuery(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterBatchQuery()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.BatchQuery<TraceEntity>(0,
                10,
                OrderField.Ascending<TraceEntity>(t => t.Id).AsEnumerable(),
                (object)null);

            // Assert
            trace.Verify(t => t.AfterBatchQuery(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #region BatchQueryAsync

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeBatchQueryAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.BatchQueryAsync<TraceEntity>(0,
                10,
                OrderField.Ascending<TraceEntity>(t => t.Id).AsEnumerable(),
                (object)null);

            // Assert
            trace.Verify(t => t.BeforeBatchQuery(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterBatchQueryAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.BatchQueryAsync<TraceEntity>(0,
                10,
                OrderField.Ascending<TraceEntity>(t => t.Id).AsEnumerable(),
                (object)null);

            // Assert
            trace.Verify(t => t.AfterBatchQuery(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #endregion

        #region Count

        #region Count

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeCount()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.Count<TraceEntity>((object)null);

            // Assert
            trace.Verify(t => t.BeforeCount(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterCount()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.Count<TraceEntity>((object)null);

            // Assert
            trace.Verify(t => t.AfterCount(It.IsAny<TraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeCountViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.Count(ClassMappedNameCache.Get<TraceEntity>(),
                (object)null);

            // Assert
            trace.Verify(t => t.BeforeCount(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterCountViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.Count(ClassMappedNameCache.Get<TraceEntity>(),
                (object)null);

            // Assert
            trace.Verify(t => t.AfterCount(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #region CountAsync

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeCountAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.CountAsync<TraceEntity>((object)null);

            // Assert
            trace.Verify(t => t.BeforeCount(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterCountAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.CountAsync<TraceEntity>((object)null);

            // Assert
            trace.Verify(t => t.AfterCount(It.IsAny<TraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeCountAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.CountAsync(ClassMappedNameCache.Get<TraceEntity>(),
                (object)null);

            // Assert
            trace.Verify(t => t.BeforeCount(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterCountAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.CountAsync(ClassMappedNameCache.Get<TraceEntity>(),
                (object)null);

            // Assert
            trace.Verify(t => t.AfterCount(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #endregion

        #region CountAll

        #region CountAll

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeCountAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.CountAll<TraceEntity>();

            // Assert
            trace.Verify(t => t.BeforeCountAll(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterCountAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.CountAll<TraceEntity>();

            // Assert
            trace.Verify(t => t.AfterCountAll(It.IsAny<TraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeCountAllViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.CountAll(ClassMappedNameCache.Get<TraceEntity>());

            // Assert
            trace.Verify(t => t.BeforeCountAll(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterCountAllViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.CountAll(ClassMappedNameCache.Get<TraceEntity>());

            // Assert
            trace.Verify(t => t.AfterCountAll(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #region CountAll

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeCountAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.CountAllAsync<TraceEntity>();

            // Assert
            trace.Verify(t => t.BeforeCountAll(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterCountAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.CountAllAsync<TraceEntity>();

            // Assert
            trace.Verify(t => t.AfterCountAll(It.IsAny<TraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeCountAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.CountAllAsync(ClassMappedNameCache.Get<TraceEntity>());

            // Assert
            trace.Verify(t => t.BeforeCountAll(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterCountAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.CountAllAsync(ClassMappedNameCache.Get<TraceEntity>());

            // Assert
            trace.Verify(t => t.AfterCountAll(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #endregion

        #region Delete

        #region Delete

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeDelete()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.Delete<TraceEntity>(0);

            // Assert
            trace.Verify(t => t.BeforeDelete(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterDelete()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.Delete<TraceEntity>(0);

            // Assert
            trace.Verify(t => t.AfterDelete(It.IsAny<TraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeDeleteViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.Delete(ClassMappedNameCache.Get<TraceEntity>(), new { Id = 0 });

            // Assert
            trace.Verify(t => t.BeforeDelete(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterDeleteViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.Delete(ClassMappedNameCache.Get<TraceEntity>(), new { Id = 0 });

            // Assert
            trace.Verify(t => t.AfterDelete(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #region DeleteAsync

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeDeleteAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.DeleteAsync<TraceEntity>(0);

            // Assert
            trace.Verify(t => t.BeforeDelete(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterDeleteAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.DeleteAsync<TraceEntity>(0);

            // Assert
            trace.Verify(t => t.AfterDelete(It.IsAny<TraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeDeleteAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.DeleteAsync(ClassMappedNameCache.Get<TraceEntity>(), new { Id = 0 });

            // Assert
            trace.Verify(t => t.BeforeDelete(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterDeleteAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.DeleteAsync(ClassMappedNameCache.Get<TraceEntity>(), new { Id = 0 });

            // Assert
            trace.Verify(t => t.AfterDelete(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #endregion

        #region Insert

        #region Insert

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeInsert()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.Insert<TraceEntity>(new TraceEntity
            {
                Name = "Name"
            });

            // Assert
            trace.Verify(t => t.BeforeInsert(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterInsert()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.Insert<TraceEntity>(new TraceEntity
            {
                Name = "Name"
            });

            // Assert
            trace.Verify(t => t.AfterInsert(It.IsAny<TraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeInsertViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.Insert(ClassMappedNameCache.Get<TraceEntity>(), new
            {
                Name = "Name"
            });

            // Assert
            trace.Verify(t => t.BeforeInsert(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterInsertViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.Insert(ClassMappedNameCache.Get<TraceEntity>(), new
            {
                Name = "Name"
            });

            // Assert
            trace.Verify(t => t.AfterInsert(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #region InsertAsync

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeInsertAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.InsertAsync<TraceEntity>(new TraceEntity
            {
                Name = "Name"
            });

            // Assert
            trace.Verify(t => t.BeforeInsert(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterInsertAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.InsertAsync<TraceEntity>(new TraceEntity
            {
                Name = "Name"
            });

            // Assert
            trace.Verify(t => t.AfterInsert(It.IsAny<TraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeInsertAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.InsertAsync(ClassMappedNameCache.Get<TraceEntity>(), new
            {
                Name = "Name"
            });

            // Assert
            trace.Verify(t => t.BeforeInsert(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterInsertAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.InsertAsync(ClassMappedNameCache.Get<TraceEntity>(), new
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
        public void TestDbRepositoryTraceForBeforeInsertAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.InsertAll<TraceEntity>(new[] { new TraceEntity { Name = "Name" } });

            // Assert
            trace.Verify(t => t.BeforeInsertAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterInsertAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.InsertAll<TraceEntity>(new[] { new TraceEntity { Name = "Name" } });

            // Assert
            trace.Verify(t => t.AfterInsertAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeInsertAllViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.InsertAll(ClassMappedNameCache.Get<TraceEntity>(),
                new[] { new { Name = "Name" } },
                fields: Field.From("Name"));

            // Assert
            trace.Verify(t => t.BeforeInsertAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterInsertAllViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.InsertAll(ClassMappedNameCache.Get<TraceEntity>(),
                new[] { new { Name = "Name" } },
                fields: Field.From("Name"));

            // Assert
            trace.Verify(t => t.AfterInsertAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #region InsertAllAsync

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeInsertAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.InsertAllAsync<TraceEntity>(new[] { new TraceEntity { Name = "Name" } });

            // Assert
            trace.Verify(t => t.BeforeInsertAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterInsertAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.InsertAll<TraceEntity>(new[] { new TraceEntity { Name = "Name" } });

            // Assert
            trace.Verify(t => t.AfterInsertAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeInsertAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.InsertAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new[] { new { Name = "Name" } },
                fields: Field.From("Name"));

            // Assert
            trace.Verify(t => t.BeforeInsertAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterInsertAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.InsertAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new[] { new { Name = "Name" } },
                fields: Field.From("Name"));

            // Assert
            trace.Verify(t => t.AfterInsertAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #endregion

        #region Merge

        #region Merge

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeMerge()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.Merge<TraceEntity>(new TraceEntity
            {
                Id = 1,
                Name = "Name"
            });

            // Assert
            trace.Verify(t => t.BeforeMerge(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterMerge()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.Merge<TraceEntity>(new TraceEntity
            {
                Id = 1,
                Name = "Name"
            });

            // Assert
            trace.Verify(t => t.AfterMerge(It.IsAny<TraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeMergeViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.Merge(ClassMappedNameCache.Get<TraceEntity>(), new
            {
                Id = 1,
                Name = "Name"
            });

            // Assert
            trace.Verify(t => t.BeforeMerge(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterMergeViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.Merge(ClassMappedNameCache.Get<TraceEntity>(), new
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
        public void TestDbRepositoryTraceForBeforeMergeAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.MergeAsync<TraceEntity>(new TraceEntity
            {
                Id = 1,
                Name = "Name"
            });

            // Assert
            trace.Verify(t => t.BeforeMerge(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterMergeAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.MergeAsync<TraceEntity>(new TraceEntity
            {
                Id = 1,
                Name = "Name"
            });

            // Assert
            trace.Verify(t => t.AfterMerge(It.IsAny<TraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeMergeAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.MergeAsync(ClassMappedNameCache.Get<TraceEntity>(), new
            {
                Id = 1,
                Name = "Name"
            });

            // Assert
            trace.Verify(t => t.BeforeMerge(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterMergeAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.MergeAsync(ClassMappedNameCache.Get<TraceEntity>(), new
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
        public void TestDbRepositoryTraceForBeforeQuery()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.Query<TraceEntity>(te => te.Id == 1);

            // Assert
            trace.Verify(t => t.BeforeQuery(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterQuery()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.Query<TraceEntity>(te => te.Id == 1);

            // Assert
            trace.Verify(t => t.AfterQuery(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #region QueryAsync

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeQueryAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.QueryAsync<TraceEntity>(te => te.Id == 1);

            // Assert
            trace.Verify(t => t.BeforeQuery(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterQueryAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.QueryAsync<TraceEntity>(te => te.Id == 1);

            // Assert
            trace.Verify(t => t.AfterQuery(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #endregion

        #region

        #region QueryAll

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeQueryAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.QueryAll<TraceEntity>();

            // Assert
            trace.Verify(t => t.BeforeQueryAll(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterQueryAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.QueryAll<TraceEntity>();

            // Assert
            trace.Verify(t => t.AfterQueryAll(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #region QueryAllAsync

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeQueryAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.QueryAllAsync<TraceEntity>();

            // Assert
            trace.Verify(t => t.BeforeQueryAll(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterQueryAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.QueryAllAsync<TraceEntity>();

            // Assert
            trace.Verify(t => t.AfterQueryAll(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #endregion

        #region QueryMultiple

        #region QueryMultiple

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeQueryMultiple()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.QueryMultiple<TraceEntity, TraceEntity>(te => te.Id == 1,
                te => te.Id == 1);

            // Assert
            trace.Verify(t => t.BeforeQueryMultiple(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterQueryMultiple()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.QueryMultiple<TraceEntity, TraceEntity>(te => te.Id == 1,
                te => te.Id == 1);

            // Assert
            trace.Verify(t => t.AfterQueryMultiple(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #region QueryMultipleAsync

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeQueryMultipleAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.QueryMultipleAsync<TraceEntity, TraceEntity>(te => te.Id == 1,
                te => te.Id == 1);

            // Assert
            trace.Verify(t => t.BeforeQueryMultiple(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterQueryMultipleAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.QueryMultipleAsync<TraceEntity, TraceEntity>(te => te.Id == 1,
                te => te.Id == 1);

            // Assert
            trace.Verify(t => t.AfterQueryMultiple(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #endregion

        #region Truncate

        #region Truncate

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeTruncate()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.Truncate<TraceEntity>();

            // Assert
            trace.Verify(t => t.BeforeTruncate(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterTruncate()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.Truncate<TraceEntity>();

            // Assert
            trace.Verify(t => t.AfterTruncate(It.IsAny<TraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeTruncateViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.Truncate(ClassMappedNameCache.Get<TraceEntity>());

            // Assert
            trace.Verify(t => t.BeforeTruncate(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterTruncateViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.Truncate(ClassMappedNameCache.Get<TraceEntity>());

            // Assert
            trace.Verify(t => t.AfterTruncate(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #region TruncateAsync

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeTruncateAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.TruncateAsync<TraceEntity>();

            // Assert
            trace.Verify(t => t.BeforeTruncate(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterTruncateAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.TruncateAsync<TraceEntity>();

            // Assert
            trace.Verify(t => t.AfterTruncate(It.IsAny<TraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeTruncateAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.TruncateAsync(ClassMappedNameCache.Get<TraceEntity>());

            // Assert
            trace.Verify(t => t.BeforeTruncate(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterTruncateAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.TruncateAsync(ClassMappedNameCache.Get<TraceEntity>());

            // Assert
            trace.Verify(t => t.AfterTruncate(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #endregion

        #region Update

        #region Update

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeUpdate()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.Update<TraceEntity>(
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
        public void TestDbRepositoryTraceForAfterUpdate()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.Update<TraceEntity>(
                new TraceEntity
                {
                    Id = 1,
                    Name = "Name"
                },
                whereOrPrimaryKey: 1);

            // Assert
            trace.Verify(t => t.AfterUpdate(It.IsAny<TraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeUpdateViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.Update(ClassMappedNameCache.Get<TraceEntity>(),
                new
                {
                    Name = "Name"
                },
                new
                {
                    Id = 1
                });

            // Assert
            trace.Verify(t => t.BeforeUpdate(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterUpdateViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.Update(ClassMappedNameCache.Get<TraceEntity>(),
                new
                {
                    Name = "Name"
                },
                new
                {
                    Id = 1
                });

            // Assert
            trace.Verify(t => t.AfterUpdate(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion

        #region Update

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeUpdateAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.UpdateAsync<TraceEntity>(
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
        public void TestDbRepositoryTraceForAfterUpdateAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.UpdateAsync<TraceEntity>(
                new TraceEntity
                {
                    Id = 1,
                    Name = "Name"
                },
                whereOrPrimaryKey: 1);

            // Assert
            trace.Verify(t => t.AfterUpdate(It.IsAny<TraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForBeforeUpdateAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.UpdateAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new
                {
                    Name = "Name"
                },
                new
                {
                    Id = 1
                });

            // Assert
            trace.Verify(t => t.BeforeUpdate(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryTraceForAfterUpdateAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                null,
                Constant.DefaultCacheItemExpirationInMinutes,
                trace.Object,
                m_statementBuilder);

            // Act
            repository.UpdateAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new
                {
                    Name = "Name"
                },
                new
                {
                    Id = 1
                });

            // Assert
            trace.Verify(t => t.AfterUpdate(It.IsAny<TraceLog>()), Times.Once);
        }

        #endregion
        #endregion
    }
}
