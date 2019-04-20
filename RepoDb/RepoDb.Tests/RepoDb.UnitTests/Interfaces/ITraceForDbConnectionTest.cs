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

        // Count

        [TestMethod]
        public void TestDbConnectionTraceForBeforeCount()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.Count<TraceEntity>(trace: trace.Object,
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
                trace: trace.Object,
                statementBuilder: m_statementBuilder);

            // Assert
            trace.Verify(t => t.AfterCount(It.IsAny<TraceLog>()), Times.Exactly(1));
        }

        // Delete

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

        // Insert

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

        // Merge

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

        // Query

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

        // QueryMultiple

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

        // Truncate

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

        // Update

        [TestMethod]
        public void TestDbConnectionTraceForBeforeUpdate()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var connection = new CustomDbConnection();

            // Act
            connection.Update<TraceEntity>(new TraceEntity
            {
                Id = 1,
                Name = "Name"
            },
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
            connection.Update<TraceEntity>(new TraceEntity
            {
                Id = 1,
                Name = "Name"
            },
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
    }
}
