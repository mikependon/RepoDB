using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoDb.Attributes;
using RepoDb.Interfaces;
using RepoDb.UnitTests.CustomObjects;
using System.Collections.Generic;

namespace RepoDb.UnitTests.Interfaces
{
    [TestClass]
    public class IStatementBuilderForDbRepositoryTest
    {
        public class DataEntity
        {
            [Primary, Identity]
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class DataEntityT1
        {
            [Primary, Identity]
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class DataEntityT2
        {
            [Primary, Identity]
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class DataEntityT3
        {
            [Primary, Identity]
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class DataEntityT4
        {
            [Primary, Identity]
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class DataEntityT5
        {
            [Primary, Identity]
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class DataEntityT6
        {
            [Primary, Identity]
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class DataEntityT7
        {
            [Primary, Identity]
            public int Id { get; set; }
            public string Name { get; set; }
        }

        // CreateBatchQuery

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForBatchQuery()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new Mock<DbRepository<CustomDbConnection>>("ConnectionString", statementBuilder.Object);

            // Setup
            statementBuilder.Setup(builder =>
                builder.CreateBatchQuery<DataEntity>(
                    It.IsAny<QueryBuilder<DataEntity>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<string>()));

            // Act
            repository.Object.BatchQuery<DataEntity>(0, 10, null, null);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateBatchQuery<DataEntity>(
                    It.IsAny<QueryBuilder<DataEntity>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<string>()), Times.Once);
        }

        // CreateCount

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForCount()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new Mock<DbRepository<CustomDbConnection>>("ConnectionString", statementBuilder.Object);

            // Setup
            statementBuilder.Setup(builder =>
                builder.CreateCount<DataEntity>(
                    It.IsAny<QueryBuilder<DataEntity>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()));

            // Act
            repository.Object.Count<DataEntity>();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateCount<DataEntity>(
                    It.IsAny<QueryBuilder<DataEntity>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Once);
        }

        // CreateDelete

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForDelete()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new Mock<DbRepository<CustomDbConnection>>("ConnectionString", statementBuilder.Object);

            // Setup
            statementBuilder.Setup(builder =>
                builder.CreateDelete<DataEntity>(
                    It.IsAny<QueryBuilder<DataEntity>>(),
                    It.IsAny<QueryGroup>()));

            // Act
            repository.Object.Delete<DataEntity>(e => e.Id == 1);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDelete<DataEntity>(
                    It.IsAny<QueryBuilder<DataEntity>>(),
                    It.IsAny<QueryGroup>()), Times.Once);
        }

        // CreateDeleteAll

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForDeleteAll()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new Mock<DbRepository<CustomDbConnection>>("ConnectionString", statementBuilder.Object);

            // Setup
            statementBuilder.Setup(builder =>
                builder.CreateDeleteAll<DataEntity>(
                    It.IsAny<QueryBuilder<DataEntity>>()));

            // Act
            repository.Object.DeleteAll<DataEntity>();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDeleteAll<DataEntity>(
                    It.IsAny<QueryBuilder<DataEntity>>()), Times.Once);
        }

        // CreateInlineInsert

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForInlineInsert()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new Mock<DbRepository<CustomDbConnection>>("ConnectionString", statementBuilder.Object);

            // Setup
            statementBuilder.Setup(builder =>
                builder.CreateInlineInsert<DataEntity>(
                    It.IsAny<QueryBuilder<DataEntity>>(),
                    It.IsAny<IEnumerable<Field>>()));

            // Act
            repository.Object.InlineInsert<DataEntity>(new { Id = 1, Name = "Name" });

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInlineInsert<DataEntity>(
                    It.IsAny<QueryBuilder<DataEntity>>(),
                    It.IsAny<IEnumerable<Field>>()), Times.Once);
        }

        // CreateInlineMerge

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForInlineMerge()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new Mock<DbRepository<CustomDbConnection>>("ConnectionString", statementBuilder.Object);

            // Setup
            statementBuilder.Setup(builder =>
                builder.CreateInlineMerge<DataEntity>(
                    It.IsAny<QueryBuilder<DataEntity>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>()));

            // Act
            repository.Object.InlineMerge<DataEntity>(new { Name = "Name" }, new Field(nameof(DataEntity.Id)));

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInlineMerge<DataEntity>(
                    It.IsAny<QueryBuilder<DataEntity>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>()), Times.Once);
        }

        // CreateInlineUpdate

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForInlineUpdate()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new Mock<DbRepository<CustomDbConnection>>("ConnectionString", statementBuilder.Object);

            // Setup
            statementBuilder.Setup(builder =>
                builder.CreateInlineUpdate<DataEntity>(
                    It.IsAny<QueryBuilder<DataEntity>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>()));

            // Act
            repository.Object.InlineUpdate<DataEntity>(new { Name = "Name" }, e => e.Id == 1);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInlineUpdate<DataEntity>(
                    It.IsAny<QueryBuilder<DataEntity>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>()), Times.Once);
        }

        // CreateInsert

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForInsert()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new Mock<DbRepository<CustomDbConnection>>("ConnectionString", statementBuilder.Object);

            // Setup
            statementBuilder.Setup(builder =>
                builder.CreateInsert<DataEntity>(
                    It.IsAny<QueryBuilder<DataEntity>>()));

            // Act
            repository.Object.Insert<DataEntity>(new DataEntity { Name = "Name" });

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsert<DataEntity>(
                    It.IsAny<QueryBuilder<DataEntity>>()), Times.Once);
        }

        // CreateMerge

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMerge()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new Mock<DbRepository<CustomDbConnection>>("ConnectionString", statementBuilder.Object);

            // Setup
            statementBuilder.Setup(builder =>
                builder.CreateMerge<DataEntity>(
                    It.IsAny<QueryBuilder<DataEntity>>(),
                    It.IsAny<IEnumerable<Field>>()));

            // Act
            repository.Object.Merge<DataEntity>(new DataEntity { Name = "Name" }, new Field(nameof(DataEntity.Id)));

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMerge<DataEntity>(
                    It.IsAny<QueryBuilder<DataEntity>>(),
                    It.IsAny<IEnumerable<Field>>()), Times.Once);
        }

        // CreateQuery

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForQuery()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new Mock<DbRepository<CustomDbConnection>>("ConnectionString", statementBuilder.Object);

            // Setup
            statementBuilder.Setup(builder =>
                builder.CreateQuery<DataEntity>(
                    It.IsAny<QueryBuilder<DataEntity>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()));

            // Act
            repository.Object.Query<DataEntity>(e => e.Id == 1);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateQuery<DataEntity>(
                    It.IsAny<QueryBuilder<DataEntity>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Once);
        }

        // QueryMultple

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForQueryMultiple()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new Mock<DbRepository<CustomDbConnection>>("ConnectionString", statementBuilder.Object);

            // Setup
            statementBuilder.Setup(builder =>
                builder.CreateQuery<DataEntityT1>(
                    It.IsAny<QueryBuilder<DataEntityT1>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()));
            statementBuilder.Setup(builder =>
                builder.CreateQuery<DataEntityT2>(
                    It.IsAny<QueryBuilder<DataEntityT2>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()));
            statementBuilder.Setup(builder =>
                builder.CreateQuery<DataEntityT3>(
                    It.IsAny<QueryBuilder<DataEntityT3>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()));
            statementBuilder.Setup(builder =>
                builder.CreateQuery<DataEntityT4>(
                    It.IsAny<QueryBuilder<DataEntityT4>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()));
            statementBuilder.Setup(builder =>
                builder.CreateQuery<DataEntityT5>(
                    It.IsAny<QueryBuilder<DataEntityT5>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()));
            statementBuilder.Setup(builder =>
                builder.CreateQuery<DataEntityT6>(
                    It.IsAny<QueryBuilder<DataEntityT6>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()));
            statementBuilder.Setup(builder =>
                builder.CreateQuery<DataEntityT7>(
                    It.IsAny<QueryBuilder<DataEntityT7>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()));

            // Act
            repository.Object.QueryMultiple<DataEntityT1,
                DataEntityT2,
                DataEntityT3,
                DataEntityT4,
                DataEntityT5,
                DataEntityT6,
                DataEntityT7>(e => e.Id == 1,
                e => e.Id == 1,
                e => e.Id == 1,
                e => e.Id == 1,
                e => e.Id == 1,
                e => e.Id == 1,
                e => e.Id == 1);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateQuery<DataEntityT1>(
                    It.IsAny<QueryBuilder<DataEntityT1>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));
            statementBuilder.Verify(builder =>
                builder.CreateQuery<DataEntityT2>(
                    It.IsAny<QueryBuilder<DataEntityT2>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));
            statementBuilder.Verify(builder =>
                builder.CreateQuery<DataEntityT3>(
                    It.IsAny<QueryBuilder<DataEntityT3>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));
            statementBuilder.Verify(builder =>
                builder.CreateQuery<DataEntityT4>(
                    It.IsAny<QueryBuilder<DataEntityT4>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));
            statementBuilder.Verify(builder =>
                builder.CreateQuery<DataEntityT5>(
                    It.IsAny<QueryBuilder<DataEntityT5>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));
            statementBuilder.Verify(builder =>
                builder.CreateQuery<DataEntityT6>(
                    It.IsAny<QueryBuilder<DataEntityT6>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));
            statementBuilder.Verify(builder =>
                builder.CreateQuery<DataEntityT7>(
                    It.IsAny<QueryBuilder<DataEntityT7>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));
        }

        // CreateTruncate

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForTruncate()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new Mock<DbRepository<CustomDbConnection>>("ConnectionString", statementBuilder.Object);

            // Setup
            statementBuilder.Setup(builder =>
                builder.CreateTruncate<DataEntity>(
                    It.IsAny<QueryBuilder<DataEntity>>()));

            // Act
            repository.Object.Truncate<DataEntity>();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateTruncate<DataEntity>(
                    It.IsAny<QueryBuilder<DataEntity>>()), Times.Once);
        }

        // CreateUpdate

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForUpdate()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new Mock<DbRepository<CustomDbConnection>>("ConnectionString", statementBuilder.Object);

            // Setup
            statementBuilder.Setup(builder =>
                builder.CreateUpdate<DataEntity>(
                    It.IsAny<QueryBuilder<DataEntity>>(),
                    It.IsAny<QueryGroup>()));

            // Act
            repository.Object.Update<DataEntity>(new DataEntity { Name = "Update" }, e => e.Id == 1);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateUpdate<DataEntity>(
                    It.IsAny<QueryBuilder<DataEntity>>(),
                    It.IsAny<QueryGroup>()), Times.Once);
        }
    }
}
