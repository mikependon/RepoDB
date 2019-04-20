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

        public class DataEntityForDbRepositoryStatementBuilderViaTableName
        {
            [Primary, Identity]
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class DataEntityForDbRepositoryStatementBuilderAcrossCalls
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
                    It.IsAny<QueryGroup>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<IEnumerable<OrderField>>(),
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
                    It.IsAny<QueryGroup>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<IEnumerable<OrderField>>(),
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
            repository.Count(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderViaTableName>());

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateCount(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderViaTableName>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Count(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderViaTableName>());

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateCount(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderViaTableName>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForCountAcrossCalls()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            repository.Count<DataEntityForDbRepositoryStatementBuilderAcrossCalls>();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateCount(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderAcrossCalls>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Count(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderAcrossCalls>());

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateCount(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderAcrossCalls>()),
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
            repository.Delete(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderViaTableName>(),
                new
                {
                    Id = 1
                });

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDelete(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderViaTableName>()),
                    It.IsAny<QueryGroup>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Delete(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderViaTableName>(),
                new
                {
                    Id = 1
                });

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateDelete(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderViaTableName>()),
                    It.IsAny<QueryGroup>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForDeleteAcrossCalls()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            repository.Delete<DataEntityForDbRepositoryStatementBuilderAcrossCalls>(e => e.Id == 1);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDelete(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderAcrossCalls>()),
                    It.IsAny<QueryGroup>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Delete(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderAcrossCalls>(),
                new
                {
                    Id = 1
                });

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateDelete(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderAcrossCalls>()),
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
            repository.DeleteAll(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderViaTableName>());

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDeleteAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderViaTableName>())), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.DeleteAll(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderViaTableName>());

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateDeleteAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderViaTableName>())), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForDeleteAllAcrossCalls()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            repository.DeleteAll<DataEntityForDbRepositoryStatementBuilderAcrossCalls>();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDeleteAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderAcrossCalls>())), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.DeleteAll(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderAcrossCalls>());

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateDeleteAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderAcrossCalls>())), Times.Exactly(0));
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
                    It.IsAny<DbField>(),
                    It.IsAny<IEnumerable<Field>>()), Times.Exactly(1));

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
                    It.IsAny<DbField>(),
                    It.IsAny<IEnumerable<Field>>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForInsertForTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            repository.Insert(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderViaTableName>(),
                new
                {
                    Name = "Name"
                });

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderViaTableName>()),
                    It.IsAny<DbField>(),
                    It.IsAny<IEnumerable<Field>>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Insert(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderViaTableName>(),
                new
                {
                    Name = "Name"
                });

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderViaTableName>()),
                    It.IsAny<DbField>(),
                    It.IsAny<IEnumerable<Field>>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForInsertAcrossCalls()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            repository.Insert<DataEntityForDbRepositoryStatementBuilderAcrossCalls>(
                new DataEntityForDbRepositoryStatementBuilderAcrossCalls
                {
                    Name = "Name"
                });

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderAcrossCalls>()),
                    It.IsAny<DbField>(),
                    It.IsAny<IEnumerable<Field>>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Insert(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderAcrossCalls>(),
                new
                {
                    Id = 1,
                    Name = "Name"
                });

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderAcrossCalls>()),
                    It.IsAny<DbField>(),
                    It.IsAny<IEnumerable<Field>>()), Times.Exactly(0));
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
                    It.IsAny<DbField>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>()), Times.Exactly(1));

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
                    It.IsAny<DbField>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMergeForTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            repository.Merge(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderViaTableName>(),
                new
                {
                    Name = "Name"
                },
                new Field(nameof(DataEntityForDbRepositoryStatementBuilderViaTableName.Id)));

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderViaTableName>()),
                    It.IsAny<DbField>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Merge(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderViaTableName>(),
                new
                {
                    Name = "Name"
                },
                new Field(nameof(DataEntityForDbRepositoryStatementBuilderViaTableName.Id)));

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderViaTableName>()),
                    It.IsAny<DbField>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMergeAcrossCalls()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            repository.Merge<DataEntityForDbRepositoryStatementBuilderAcrossCalls>(
                new DataEntityForDbRepositoryStatementBuilderAcrossCalls
                {
                    Name = "Name"
                },
                new Field(nameof(DataEntityForDbRepositoryStatementBuilderAcrossCalls.Id)));

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderAcrossCalls>()),
                    It.IsAny<DbField>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Merge(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderAcrossCalls>(),
                new
                {
                    Id = 1,
                    Name = "Name"
                },
                new Field(nameof(DataEntityForDbRepositoryStatementBuilderAcrossCalls.Id)));

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderAcrossCalls>()),
                    It.IsAny<DbField>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>()), Times.Exactly(0));
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
        public void TestDbRepositoryStatementBuilderForTruncateForTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            repository.Truncate(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderViaTableName>());

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateTruncate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderViaTableName>())), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Truncate(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderViaTableName>());

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateTruncate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderViaTableName>())), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForTruncateAcrossCalls()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            repository.Truncate<DataEntityForDbRepositoryStatementBuilderAcrossCalls>();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateTruncate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderAcrossCalls>())), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Truncate(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderAcrossCalls>());

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateTruncate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderAcrossCalls>())), Times.Exactly(0));
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
                    It.IsAny<DbField>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>()), Times.Exactly(1));

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
                    It.IsAny<DbField>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForUpdateTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            repository.Update(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderViaTableName>(),
                new DataEntityForDbRepositoryStatementBuilderViaTableName
                {
                    Name = "Update"
                },
                new { Id = 1 });

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateUpdate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderViaTableName>()),
                    It.IsAny<DbField>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Update(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderViaTableName>(),
                new DataEntityForDbRepositoryStatementBuilderViaTableName
                {
                    Name = "Update"
                },
                new { Id = 1 });

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateUpdate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderViaTableName>()),
                    It.IsAny<DbField>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForUpdateAcrossCalls()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilder.Object);

            // Act
            repository.Update<DataEntityForDbRepositoryStatementBuilderAcrossCalls>(
                new DataEntityForDbRepositoryStatementBuilderAcrossCalls
                {
                    Name = "Update"
                },
                e => e.Id == 1);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateUpdate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderAcrossCalls>()),
                    It.IsAny<DbField>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnection>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Update(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderAcrossCalls>(),
                new DataEntityForDbRepositoryStatementBuilderAcrossCalls
                {
                    Id = 1,
                    Name = "Update"
                },
                new { Id = 1 });

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateUpdate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderAcrossCalls>()),
                    It.IsAny<DbField>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>()), Times.Exactly(0));
        }
    }
}
