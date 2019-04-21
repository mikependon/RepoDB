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
        public class DataEntityForDbRepositoryStatementBuilder
        {
            [Primary, Identity]
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class DataEntityForDbRepositoryStatementBuilderForTableName
        {
            [Primary, Identity]
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class DataEntityForDbRepositoryStatementBuilderForCrossCall
        {
            [Primary, Identity]
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class DataEntityForDbRepositoryStatementBuilderT1
        {
            [Primary, Identity]
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class DataEntityForDbRepositoryStatementBuilderT2
        {
            [Primary, Identity]
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class DataEntityForDbRepositoryStatementBuilderT3
        {
            [Primary, Identity]
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class DataEntityForDbRepositoryStatementBuilderT4
        {
            [Primary, Identity]
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class DataEntityForDbRepositoryStatementBuilderT5
        {
            [Primary, Identity]
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class DataEntityForDbRepositoryStatementBuilderT6
        {
            [Primary, Identity]
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class DataEntityForDbRepositoryStatementBuilderT7
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
            var repository = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            repository.BatchQuery<DataEntityForDbRepositoryStatementBuilder>(0,
                10,
                null,
                null);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateBatchQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.BatchQuery<DataEntityForDbRepositoryStatementBuilder>(0,
                10,
                null,
                null);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateBatchQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        // CreateCount

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForCount()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            repository.Count<DataEntityForDbRepositoryStatementBuilder>();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateCount(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Count<DataEntityForDbRepositoryStatementBuilder>();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateCount(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForCountViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            repository.Count(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>());

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateCount(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Count(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>());

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateCount(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForCountViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            repository.Count<DataEntityForDbRepositoryStatementBuilderForCrossCall>();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateCount(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Count(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>());

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateCount(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        // CreateDelete

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForDelete()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            repository.Delete<DataEntityForDbRepositoryStatementBuilder>(e => e.Id == 1);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDelete(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<QueryGroup>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Delete<DataEntityForDbRepositoryStatementBuilder>(e => e.Id == 1);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateDelete(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<QueryGroup>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForDeleteViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            repository.Delete(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                new
                {
                    Id = 1
                });

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDelete(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<QueryGroup>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Delete(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                new
                {
                    Id = 1
                });

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateDelete(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<QueryGroup>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForDeleteViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            repository.Delete<DataEntityForDbRepositoryStatementBuilderForCrossCall>(e => e.Id == 1);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDelete(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<QueryGroup>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Delete(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>(),
                new
                {
                    Id = 1
                });

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateDelete(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<QueryGroup>()), Times.Exactly(0));
        }

        // CreateDeleteAll

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForDeleteAll()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            repository.DeleteAll<DataEntityForDbRepositoryStatementBuilder>();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDeleteAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>())), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.DeleteAll<DataEntityForDbRepositoryStatementBuilder>();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateDeleteAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>())), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForDeleteAllViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            repository.DeleteAll(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>());

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDeleteAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>())), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.DeleteAll(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>());

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateDeleteAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>())), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForDeleteAllViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            repository.DeleteAll<DataEntityForDbRepositoryStatementBuilderForCrossCall>();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDeleteAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>())), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.DeleteAll(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>());

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateDeleteAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>())), Times.Exactly(0));
        }

        // CreateInsert

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForInsert()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            repository.Insert<DataEntityForDbRepositoryStatementBuilder>(
                new DataEntityForDbRepositoryStatementBuilder
                {
                    Name = "Name"
                });

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Insert<DataEntityForDbRepositoryStatementBuilder>(
                new DataEntityForDbRepositoryStatementBuilder
                {
                    Name = "Name"
                });

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForInsertViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            repository.Insert(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                new
                {
                    Name = "Name"
                });

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Insert(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                new
                {
                    Name = "Name"
                });

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForInsertViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            repository.Insert<DataEntityForDbRepositoryStatementBuilderForCrossCall>(
                new DataEntityForDbRepositoryStatementBuilderForCrossCall
                {
                    Name = "Name"
                });

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Insert(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>(),
                new
                {
                    Id = 1,
                    Name = "Name"
                });

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        // CreateMerge

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMerge()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            repository.Merge<DataEntityForDbRepositoryStatementBuilder>(
                new DataEntityForDbRepositoryStatementBuilder
                {
                    Name = "Name"
                },
                new Field(nameof(DataEntityForDbRepositoryStatementBuilder.Id)));

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Merge<DataEntityForDbRepositoryStatementBuilder>(
                new DataEntityForDbRepositoryStatementBuilder
                {
                    Name = "Name"
                },
                new Field(nameof(DataEntityForDbRepositoryStatementBuilder.Id)));

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMergeViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            repository.Merge(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                new
                {
                    Name = "Name"
                },
                new Field(nameof(DataEntityForDbRepositoryStatementBuilderForTableName.Id)));

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Merge(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                new
                {
                    Name = "Name"
                },
                new Field(nameof(DataEntityForDbRepositoryStatementBuilderForTableName.Id)));

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMergeViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            repository.Merge<DataEntityForDbRepositoryStatementBuilderForCrossCall>(
                new DataEntityForDbRepositoryStatementBuilderForCrossCall
                {
                    Name = "Name"
                },
                new Field(nameof(DataEntityForDbRepositoryStatementBuilderForCrossCall.Id)));

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Merge(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>(),
                new
                {
                    Id = 1,
                    Name = "Name"
                },
                new Field(nameof(DataEntityForDbRepositoryStatementBuilderForCrossCall.Id)));

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        // CreateQuery

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForQuery()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            repository.Query<DataEntityForDbRepositoryStatementBuilder>(e => e.Id == 1);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Query<DataEntityForDbRepositoryStatementBuilder>(e => e.Id == 1);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        // QueryMultple

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForQueryMultiple()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            repository.QueryMultiple<DataEntityForDbRepositoryStatementBuilderT1,
                DataEntityForDbRepositoryStatementBuilderT2,
                DataEntityForDbRepositoryStatementBuilderT3,
                DataEntityForDbRepositoryStatementBuilderT4,
                DataEntityForDbRepositoryStatementBuilderT5,
                DataEntityForDbRepositoryStatementBuilderT6,
                DataEntityForDbRepositoryStatementBuilderT7>(e => e.Id == 1,
                e => e.Id == 1,
                e => e.Id == 1,
                e => e.Id == 1,
                e => e.Id == 1,
                e => e.Id == 1,
                e => e.Id == 1);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderT1>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderT2>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderT3>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderT4>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderT5>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderT6>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderT7>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.QueryMultiple<DataEntityForDbRepositoryStatementBuilderT1,
                DataEntityForDbRepositoryStatementBuilderT2,
                DataEntityForDbRepositoryStatementBuilderT3,
                DataEntityForDbRepositoryStatementBuilderT4,
                DataEntityForDbRepositoryStatementBuilderT5,
                DataEntityForDbRepositoryStatementBuilderT6,
                DataEntityForDbRepositoryStatementBuilderT7>(e => e.Id == 1,
                e => e.Id == 1,
                e => e.Id == 1,
                e => e.Id == 1,
                e => e.Id == 1,
                e => e.Id == 1,
                e => e.Id == 1);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderT1>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderT2>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderT3>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderT4>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderT5>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderT6>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderT7>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        // CreateTruncate

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForTruncate()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            repository.Truncate<DataEntityForDbRepositoryStatementBuilder>();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateTruncate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>())), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Truncate<DataEntityForDbRepositoryStatementBuilder>();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateTruncate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>())), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForTruncateViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            repository.Truncate(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>());

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateTruncate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>())), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Truncate(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>());

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateTruncate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>())), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForTruncateViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            repository.Truncate<DataEntityForDbRepositoryStatementBuilderForCrossCall>();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateTruncate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>())), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Truncate(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>());

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateTruncate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>())), Times.Exactly(0));
        }

        // CreateUpdate

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForUpdate()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            repository.Update<DataEntityForDbRepositoryStatementBuilder>(
                new DataEntityForDbRepositoryStatementBuilder
                {
                    Name = "Update"
                },
                e => e.Id == 1);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateUpdate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Update<DataEntityForDbRepositoryStatementBuilder>(
                new DataEntityForDbRepositoryStatementBuilder
                {
                    Name = "Update"
                },
                e => e.Id == 1);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateUpdate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForUpdateTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            repository.Update(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                new DataEntityForDbRepositoryStatementBuilderForTableName
                {
                    Name = "Update"
                },
                new { Id = 1 });

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateUpdate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Update(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                new DataEntityForDbRepositoryStatementBuilderForTableName
                {
                    Name = "Update"
                },
                new { Id = 1 });

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateUpdate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForUpdateViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            repository.Update<DataEntityForDbRepositoryStatementBuilderForCrossCall>(
                new DataEntityForDbRepositoryStatementBuilderForCrossCall
                {
                    Name = "Update"
                },
                e => e.Id == 1);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateUpdate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Update(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>(),
                new DataEntityForDbRepositoryStatementBuilderForCrossCall
                {
                    Id = 1,
                    Name = "Update"
                },
                new { Id = 1 });

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateUpdate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }
    }
}
