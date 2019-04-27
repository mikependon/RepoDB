using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoDb.Attributes;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.UnitTests.CustomObjects;

namespace RepoDb.UnitTests.Interfaces
{
    [TestClass]
    public class ITraceForDbConnectionTest
    {
        private readonly IStatementBuilder m_statementBuilder = new SqlStatementBuilder();

        #region SubClasses

        public class TraceEntity
        {
            [Primary, Identity]
            public int Id { get; set; }
            public string Name { get; set; }
        }

        #endregion

        #region BatchQuery

        #region BatchQuery

        [TestMethod]
        public void TestDbConnectionTraceForBeforeBatchQuery()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.BatchQuery<TraceEntity>(0,
                10,
                OrderField.Ascending<TraceEntity>(t => t.Id).AsEnumerable(),
                where: (QueryGroup)null,
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeBatchQuery(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterBatchQuery()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.BatchQuery<TraceEntity>(0,
                10,
                OrderField.Ascending<TraceEntity>(t => t.Id).AsEnumerable(),
                where: (QueryGroup)null,
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterBatchQuery(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #region BatchQueryAsync

        [TestMethod]
        public void TestDbConnectionTraceForBeforeBatchQueryAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.BatchQueryAsync<TraceEntity>(0,
                10,
                OrderField.Ascending<TraceEntity>(t => t.Id).AsEnumerable(),
                where: (QueryGroup)null,
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeBatchQuery(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterBatchQueryAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.BatchQueryAsync<TraceEntity>(0,
                10,
                OrderField.Ascending<TraceEntity>(t => t.Id).AsEnumerable(),
                where: (QueryGroup)null,
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterBatchQuery(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #endregion

        #region Count

        #region Count

        [TestMethod]
        public void TestDbConnectionTraceForBeforeCount()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.Count<TraceEntity>(trace: trace.Object,
                where: (object)null,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeCount(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterCount()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.Count<TraceEntity>(trace: trace.Object,
                where: (object)null,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterCount(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeCountViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.Count(ClassMappedNameCache.Get<TraceEntity>(),
                where: (object)null,
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeCount(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterCountViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.Count(ClassMappedNameCache.Get<TraceEntity>(),
                where: (object)null,
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterCount(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #region CountAsync

        [TestMethod]
        public void TestDbConnectionTraceForBeforeCountAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.CountAsync<TraceEntity>(trace: trace.Object,
                where: (object)null,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeCount(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterCountAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.Count<TraceEntity>(trace: trace.Object,
                where: (object)null,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterCount(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeCountAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.CountAsync(ClassMappedNameCache.Get<TraceEntity>(),
                where: (object)null,
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeCount(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterCountAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.CountAsync(ClassMappedNameCache.Get<TraceEntity>(),
                where: (object)null,
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterCount(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #endregion

        #region CountAll

        [TestMethod]
        public void TestDbConnectionTraceForBeforeCountAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.CountAll<TraceEntity>(trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeCountAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterCountAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.CountAll<TraceEntity>(trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterCountAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeCountAllViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.CountAll(ClassMappedNameCache.Get<TraceEntity>(),
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeCountAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterCountAllViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.CountAll(ClassMappedNameCache.Get<TraceEntity>(),
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterCountAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #region Delete

        #region Delete

        [TestMethod]
        public void TestDbConnectionTraceForBeforeDelete()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.Delete<TraceEntity>(0,
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeDelete(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterDelete()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.Delete<TraceEntity>(0,
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterDelete(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeDeleteViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.Delete(ClassMappedNameCache.Get<TraceEntity>(),
                new
                {
                    Id = 1
                },
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeDelete(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterDeleteViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.Delete(ClassMappedNameCache.Get<TraceEntity>(),
                new
                {
                    Id = 1
                },
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterDelete(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #region DeleteAsync

        [TestMethod]
        public void TestDbConnectionTraceForBeforeDeleteAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.DeleteAsync<TraceEntity>(0,
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeDelete(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterDeleteAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.DeleteAsync<TraceEntity>(0,
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterDelete(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeDeleteAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.DeleteAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new
                {
                    Id = 1
                },
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeDelete(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterDeleteAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.DeleteAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new
                {
                    Id = 1
                },
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterDelete(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #endregion

        #region Insert

        #region Insert

        [TestMethod]
        public void TestDbConnectionTraceForBeforeInsert()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.Insert<TraceEntity>(new TraceEntity
            {
                Name = "Name"
            },
            trace: trace.Object,
            statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeInsert(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterInsert()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.Insert<TraceEntity>(new TraceEntity
            {
                Name = "Name"
            },
            trace: trace.Object,
            statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterInsert(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeInsertViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.Insert(ClassMappedNameCache.Get<TraceEntity>(),
                new
                {
                    Name = "Name"
                },
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeInsert(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterInsertViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.Insert(ClassMappedNameCache.Get<TraceEntity>(),
                new
                {
                    Name = "Name"
                },
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterInsert(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #region InsertAsync

        [TestMethod]
        public void TestDbConnectionTraceForBeforeInsertAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.InsertAsync<TraceEntity>(new TraceEntity
            {
                Name = "Name"
            },
            trace: trace.Object,
            statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeInsert(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterInsertAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.InsertAsync<TraceEntity>(new TraceEntity
            {
                Name = "Name"
            },
            trace: trace.Object,
            statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterInsert(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeInsertAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.InsertAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new
                {
                    Name = "Name"
                },
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeInsert(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterInsertAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.InsertAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new
                {
                    Name = "Name"
                },
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterInsert(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #endregion

        #region InsertAll

        #region InsertAll

        [TestMethod]
        public void TestDbConnectionTraceForBeforeInsertAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.InsertAll<TraceEntity>(new[] { new TraceEntity { Name = "Name" } },
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeInsertAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterInsertAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.InsertAll<TraceEntity>(new[] { new TraceEntity { Name = "Name" } },
                trace: trace.Object,
            statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterInsertAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeInsertAllViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.InsertAll(ClassMappedNameCache.Get<TraceEntity>(),
                new[] { new { Name = "Name" } },
                fields: Field.From("Name"),
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeInsertAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterInsertAllViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.InsertAll(ClassMappedNameCache.Get<TraceEntity>(),
                new[] { new { Name = "Name" } },
                fields: Field.From("Name"),
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterInsertAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #region InsertAllAsync

        [TestMethod]
        public void TestDbConnectionTraceForBeforeInsertAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.InsertAllAsync<TraceEntity>(new[] { new TraceEntity { Name = "Name" } },
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeInsertAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterInsertAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.InsertAll<TraceEntity>(new[] { new TraceEntity { Name = "Name" } },
                trace: trace.Object,
            statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterInsertAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeInsertAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.InsertAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new[] { new { Name = "Name" } },
                fields: Field.From("Name"),
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeInsertAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterInsertAllAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.InsertAllAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new[] { new { Name = "Name" } },
                fields: Field.From("Name"),
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterInsertAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #endregion

        #region Merge

        #region Merge

        [TestMethod]
        public void TestDbConnectionTraceForBeforeMerge()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.Merge<TraceEntity>(new TraceEntity
            {
                Id = 1,
                Name = "Name"
            },
            trace: trace.Object,
            statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeMerge(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterMerge()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.Merge<TraceEntity>(new TraceEntity
            {
                Id = 1,
                Name = "Name"
            },
            trace: trace.Object,
            statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterMerge(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeMergeViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.Merge(ClassMappedNameCache.Get<TraceEntity>(),
                new
                {
                    Id = 1,
                    Name = "Name"
                },
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeMerge(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterMergeViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.Merge(ClassMappedNameCache.Get<TraceEntity>(),
                new
                {
                    Id = 1,
                    Name = "Name"
                }, trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterMerge(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #region MergeAsync

        [TestMethod]
        public void TestDbConnectionTraceForBeforeMergeAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.MergeAsync<TraceEntity>(new TraceEntity
            {
                Id = 1,
                Name = "Name"
            },
            trace: trace.Object,
            statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeMerge(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterMergeAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.MergeAsync<TraceEntity>(new TraceEntity
            {
                Id = 1,
                Name = "Name"
            },
            trace: trace.Object,
            statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterMerge(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeMergeAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.MergeAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new
                {
                    Id = 1,
                    Name = "Name"
                },
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeMerge(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterMergeAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.MergeAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new
                {
                    Id = 1,
                    Name = "Name"
                }, trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterMerge(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #endregion

        #region Query

        #region Query

        [TestMethod]
        public void TestDbConnectionTraceForBeforeQuery()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.Query<TraceEntity>(te => te.Id == 1,
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeQuery(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterQuery()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.Query<TraceEntity>(te => te.Id == 1,
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterQuery(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #region QueryAsync

        [TestMethod]
        public void TestDbConnectionTraceForBeforeQueryAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.QueryAsync<TraceEntity>(te => te.Id == 1,
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeQuery(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterQueryAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.QueryAsync<TraceEntity>(te => te.Id == 1,
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterQuery(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #endregion

        #region QueryAll

        #region QueryAll

        [TestMethod]
        public void TestDbConnectionTraceForBeforeQueryAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.QueryAll<TraceEntity>(trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeQueryAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterQueryAll()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.QueryAll<TraceEntity>(trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterQueryAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #region QueryAllAsync

        [TestMethod]
        public void TestDbConnectionTraceForBeforeQueryAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.QueryAllAsync<TraceEntity>(trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeQueryAll(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterQueryAllAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.QueryAllAsync<TraceEntity>(trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterQueryAll(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #endregion

        #region QueryMultiple

        #region QueryMultiple

        [TestMethod]
        public void TestDbConnectionTraceForBeforeQueryMultiple()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.QueryMultiple<TraceEntity, TraceEntity>(te => te.Id == 1,
                te => te.Id == 1,
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeQueryMultiple(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterQueryMultiple()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.QueryMultiple<TraceEntity, TraceEntity>(te => te.Id == 1,
                te => te.Id == 1,
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterQueryMultiple(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #region QueryMultipleAsync

        [TestMethod]
        public void TestDbConnectionTraceForBeforeQueryMultipleAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.QueryMultipleAsync<TraceEntity, TraceEntity>(te => te.Id == 1,
                te => te.Id == 1,
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeQueryMultiple(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterQueryMultipleAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.QueryMultipleAsync<TraceEntity, TraceEntity>(te => te.Id == 1,
                te => te.Id == 1,
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterQueryMultiple(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #endregion

        #region Truncate

        #region Truncate

        [TestMethod]
        public void TestDbConnectionTraceForBeforeTruncate()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.Truncate<TraceEntity>(trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeTruncate(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterTruncate()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.Truncate<TraceEntity>(trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterTruncate(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeTruncateViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.Truncate(ClassMappedNameCache.Get<TraceEntity>(),
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeTruncate(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterTruncateViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.Truncate(ClassMappedNameCache.Get<TraceEntity>(),
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterTruncate(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #region TruncateAsync

        [TestMethod]
        public void TestDbConnectionTraceForBeforeTruncateAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.TruncateAsync<TraceEntity>(trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeTruncate(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterTruncateAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.TruncateAsync<TraceEntity>(trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterTruncate(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeTruncateAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.TruncateAsync(ClassMappedNameCache.Get<TraceEntity>(),
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeTruncate(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterTruncateAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.TruncateAsync(ClassMappedNameCache.Get<TraceEntity>(),
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterTruncate(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #endregion

        #region Update

        #region Update

        [TestMethod]
        public void TestDbConnectionTraceForBeforeUpdate()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.Update<TraceEntity>(
                new TraceEntity
                {
                    Id = 1,
                    Name = "Name"
                },
                whereOrPrimaryKey: 1,
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeUpdate(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterUpdate()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.Update<TraceEntity>(
                new TraceEntity
                {
                    Id = 1,
                    Name = "Name"
                },
                whereOrPrimaryKey: 1,
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterUpdate(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeUpdateViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.Update(ClassMappedNameCache.Get<TraceEntity>(),
                new
                {
                    Name = "Name"
                },
                new
                {
                    Id = 1
                },
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeUpdate(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterUpdateViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.Update(ClassMappedNameCache.Get<TraceEntity>(),
                new
                {
                    Name = "Name"
                },
                new
                {
                    Id = 1
                },
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterUpdate(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #region UpdateAsync

        [TestMethod]
        public void TestDbConnectionTraceForBeforeUpdateAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.UpdateAsync<TraceEntity>(
                new TraceEntity
                {
                    Id = 1,
                    Name = "Name"
                },
                whereOrPrimaryKey: 1,
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeUpdate(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterUpdateAsync()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.UpdateAsync<TraceEntity>(
                new TraceEntity
                {
                    Id = 1,
                    Name = "Name"
                },
                whereOrPrimaryKey: 1,
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterUpdate(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForBeforeUpdateAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.UpdateAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new
                {
                    Name = "Name"
                },
                new
                {
                    Id = 1
                },
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.BeforeUpdate(It.IsAny<CancellableTraceLog>()), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDbConnectionTraceForAfterUpdateAsyncViaTableName()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.UpdateAsync(ClassMappedNameCache.Get<TraceEntity>(),
                new
                {
                    Name = "Name"
                },
                new
                {
                    Id = 1
                },
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterUpdate(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        #endregion

        #endregion
    }
}
