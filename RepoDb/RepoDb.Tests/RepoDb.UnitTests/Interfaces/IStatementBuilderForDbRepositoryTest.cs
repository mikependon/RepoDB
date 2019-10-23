using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoDb.Attributes;
using RepoDb.Contexts.Execution;
using RepoDb.Interfaces;
using RepoDb.UnitTests.CustomObjects;
using RepoDb.UnitTests.Setup;
using System.Collections.Generic;

namespace RepoDb.UnitTests.Interfaces
{
    [TestClass]
    public class IStatementBuilderForDbRepositoryTest
    {
        [TestInitialize]
        public void Initialize()
        {
            DbSettingMapper.Add(typeof(CustomDbConnectionForDbRepositoryIStatementBuilder), Helper.DbSetting, true);
            DbValidatorMapper.Add(typeof(CustomDbConnectionForDbRepositoryIStatementBuilder), Helper.DbValidator, true);
        }

        #region SubClasses

        private class CustomDbConnectionForDbRepositoryIStatementBuilder : CustomDbConnection { }

        private class DataEntityForDbRepositoryStatementBuilder
        {
            [Primary, Identity]
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private class DataEntityForDbRepositoryStatementBuilderForTableName
        {
            [Primary, Identity]
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private class DataEntityForDbRepositoryStatementBuilderForCrossCall
        {
            [Primary, Identity]
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private class DataEntityForDbRepositoryStatementBuilderT1
        {
            [Primary, Identity]
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private class DataEntityForDbRepositoryStatementBuilderT2
        {
            [Primary, Identity]
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private class DataEntityForDbRepositoryStatementBuilderT3
        {
            [Primary, Identity]
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private class DataEntityForDbRepositoryStatementBuilderT4
        {
            [Primary, Identity]
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private class DataEntityForDbRepositoryStatementBuilderT5
        {
            [Primary, Identity]
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private class DataEntityForDbRepositoryStatementBuilderT6
        {
            [Primary, Identity]
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private class DataEntityForDbRepositoryStatementBuilderT7
        {
            [Primary, Identity]
            public int Id { get; set; }
            public string Name { get; set; }
        }

        #endregion

        #region Sync

        #region CreateAverage

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForAverage()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.Average<DataEntityForDbRepositoryStatementBuilder>(e => e.Id,
                (object)null);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateAverage(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Average<DataEntityForDbRepositoryStatementBuilder>(e => e.Id,
                (object)null);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateAverage(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForAverageViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.Average(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                new Field("Id"),
                (object)null);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateAverage(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Average(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                new Field("Id"),
                (object)null);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateAverage(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForAverageViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.Average<DataEntityForDbRepositoryStatementBuilderForCrossCall>(e => e.Id,
                (object)null);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateAverage(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Average(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>(),
                new Field("Id", typeof(int)),
                (object)null);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateAverage(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateAverageAll

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForAverageAll()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.AverageAll<DataEntityForDbRepositoryStatementBuilder>(e => e.Id);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateAverageAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.AverageAll<DataEntityForDbRepositoryStatementBuilder>(e => e.Id);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateAverageAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForAverageAllViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.AverageAll(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                new Field("Id"));

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateAverageAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.AverageAll(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                new Field("Id"));

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateAverageAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForAverageAllViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.AverageAll<DataEntityForDbRepositoryStatementBuilderForCrossCall>(e => e.Id);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateAverageAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.AverageAll(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>(),
                new Field("Id", typeof(int)));

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateAverageAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateBatchQuery

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForBatchQuery()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.BatchQuery<DataEntityForDbRepositoryStatementBuilder>(0,
                10,
                null,
                (object)null);

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
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.BatchQuery<DataEntityForDbRepositoryStatementBuilder>(0,
                10,
                null,
                (object)null);

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

        #endregion

        #region CreateCount

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForCount()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.Count<DataEntityForDbRepositoryStatementBuilder>((object)null);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateCount(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Count<DataEntityForDbRepositoryStatementBuilder>((object)null);

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
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.Count(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                (object)null);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateCount(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Count(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                (object)null);

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
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.Count<DataEntityForDbRepositoryStatementBuilderForCrossCall>((object)null);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateCount(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Count(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>(),
                (object)null);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateCount(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateCountAll

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForCountAll()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.CountAll<DataEntityForDbRepositoryStatementBuilder>();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateCountAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.CountAll<DataEntityForDbRepositoryStatementBuilder>();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateCountAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForCountAllViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.CountAll(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>());

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateCountAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.CountAll(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>());

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateCountAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForCountAllViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.CountAll<DataEntityForDbRepositoryStatementBuilderForCrossCall>();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateCountAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.CountAll(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>());

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateCountAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateDelete

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForDelete()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.Delete<DataEntityForDbRepositoryStatementBuilder>(e => e.Id == 1);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDelete(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<QueryGroup>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

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
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
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
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

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
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.Delete<DataEntityForDbRepositoryStatementBuilderForCrossCall>(e => e.Id == 1);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDelete(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<QueryGroup>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

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

        #endregion

        #region CreateDeleteAll

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForDeleteAll()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.DeleteAll<DataEntityForDbRepositoryStatementBuilder>();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDeleteAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>())), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

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
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.DeleteAll(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>());

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDeleteAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>())), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

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
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.DeleteAll<DataEntityForDbRepositoryStatementBuilderForCrossCall>();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDeleteAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>())), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.DeleteAll(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>());

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateDeleteAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>())), Times.Exactly(0));
        }

        #endregion

        #region CreateInsert

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForInsert()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            InsertExecutionContextCache<DataEntityForDbRepositoryStatementBuilder>.Flush();
            CommandTextCache.Flush();
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
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

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
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForInsertViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            InsertExecutionContextCache<object>.Flush();
            CommandTextCache.Flush();
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
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

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
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForInsertViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            InsertExecutionContextCache<DataEntityForDbRepositoryStatementBuilderForCrossCall>.Flush();
            InsertExecutionContextCache<object>.Flush();
            CommandTextCache.Flush();
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
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

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
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        #endregion

        #region CreateInsertAll

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForInsertAll()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            InsertAllExecutionContextCache<DataEntityForDbRepositoryStatementBuilder>.Flush();
            CommandTextCache.Flush();
            repository.InsertAll<DataEntityForDbRepositoryStatementBuilder>(
                new[]
                {
                    new DataEntityForDbRepositoryStatementBuilder{ Name = "Name" },
                    new DataEntityForDbRepositoryStatementBuilder{ Name = "Name" }
                });

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsertAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.Is<int>(v => v > 1),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.InsertAll<DataEntityForDbRepositoryStatementBuilder>(
                new[]
                {
                    new DataEntityForDbRepositoryStatementBuilder{ Name = "Name" },
                    new DataEntityForDbRepositoryStatementBuilder{ Name = "Name" }
                });

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsertAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.Is<int>(v => v > 1),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForInsertAllWithSizePerBatchEqualsToOne()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            InsertAllExecutionContextCache<DataEntityForDbRepositoryStatementBuilder>.Flush();
            CommandTextCache.Flush();
            repository.InsertAll<DataEntityForDbRepositoryStatementBuilder>(
                new[]
                {
                    new DataEntityForDbRepositoryStatementBuilder{ Name = "Name" },
                    new DataEntityForDbRepositoryStatementBuilder{ Name = "Name" }
                },
                batchSize: 1);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.InsertAll<DataEntityForDbRepositoryStatementBuilder>(
                new[]
                {
                    new DataEntityForDbRepositoryStatementBuilder{ Name = "Name" },
                    new DataEntityForDbRepositoryStatementBuilder{ Name = "Name" }
                },
                batchSize: 1);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForInsertAllForTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            InsertAllExecutionContextCache<object>.Flush();
            CommandTextCache.Flush();
            repository.InsertAll(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                new[]
                {
                    new DataEntityForDbRepositoryStatementBuilderForTableName{ Name = "Name" },
                    new DataEntityForDbRepositoryStatementBuilderForTableName{ Name = "Name" }
                },
                fields: new[] { new Field("Id", typeof(int)), new Field("Name", typeof(string)) });

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsertAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.Is<int>(v => v > 1),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.InsertAll(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                new[]
                {
                    new DataEntityForDbRepositoryStatementBuilderForTableName{ Name = "Name" },
                    new DataEntityForDbRepositoryStatementBuilderForTableName{ Name = "Name" }
                },
                fields: new[] { new Field("Id", typeof(int)), new Field("Name", typeof(string)) });

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsertAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.Is<int>(v => v > 1),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForInsertAllWithSizePerBatchEqualsToOneForTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            InsertAllExecutionContextCache<object>.Flush();
            CommandTextCache.Flush();
            repository.InsertAll(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                new[]
                {
                    new DataEntityForDbRepositoryStatementBuilderForTableName{ Name = "Name" },
                    new DataEntityForDbRepositoryStatementBuilderForTableName{ Name = "Name" }
                },
                fields: new[] { new Field("Id", typeof(int)), new Field("Name", typeof(string)) },
                batchSize: 1);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.InsertAll(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                new[]
                {
                    new DataEntityForDbRepositoryStatementBuilderForTableName{ Name = "Name" },
                    new DataEntityForDbRepositoryStatementBuilderForTableName{ Name = "Name" }
                },
                fields: new[] { new Field("Id", typeof(int)), new Field("Name", typeof(string)) },
                batchSize: 1);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForInsertAllViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            InsertAllExecutionContextCache<DataEntityForDbRepositoryStatementBuilderForCrossCall>.Flush();
            InsertAllExecutionContextCache<object>.Flush();
            CommandTextCache.Flush();
            repository.InsertAll<DataEntityForDbRepositoryStatementBuilderForCrossCall>(
                new[]
                {
                    new DataEntityForDbRepositoryStatementBuilderForCrossCall { Name = "Name" },
                    new DataEntityForDbRepositoryStatementBuilderForCrossCall { Name = "Name" }
                });

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsertAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.Is<int>(v => v > 1),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.InsertAll(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>(),
                new[]
                {
                    new DataEntityForDbRepositoryStatementBuilderForCrossCall { Name = "Name" },
                    new DataEntityForDbRepositoryStatementBuilderForCrossCall { Name = "Name" }
                },
                fields: new[] { new Field("Id", typeof(int)), new Field("Name", typeof(string)) });

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsertAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.Is<int>(v => v > 1),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForInsertAllWithSizePerBatchEqualsToOneViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            InsertAllExecutionContextCache<DataEntityForDbRepositoryStatementBuilderForCrossCall>.Flush();
            InsertAllExecutionContextCache<object>.Flush();
            CommandTextCache.Flush();
            repository.InsertAll<DataEntityForDbRepositoryStatementBuilderForCrossCall>(
                new[]
                {
                    new DataEntityForDbRepositoryStatementBuilderForCrossCall { Name = "Name" },
                    new DataEntityForDbRepositoryStatementBuilderForCrossCall { Name = "Name" }
                },
                batchSize: 1);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.InsertAll(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>(),
                new[]
                {
                    new DataEntityForDbRepositoryStatementBuilderForCrossCall { Name = "Name" },
                    new DataEntityForDbRepositoryStatementBuilderForCrossCall { Name = "Name" }
                },
                fields: new[] { new Field("Id", typeof(int)), new Field("Name", typeof(string)) },
                batchSize: 1);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        #endregion

        #region CreateMaximum

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMaximum()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.Maximum<DataEntityForDbRepositoryStatementBuilder>(e => e.Id,
                (object)null);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMaximum(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Maximum<DataEntityForDbRepositoryStatementBuilder>(e => e.Id,
                (object)null);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMaximum(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMaximumViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.Maximum(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                new Field("Id"),
                (object)null);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMaximum(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Maximum(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                new Field("Id"),
                (object)null);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMaximum(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMaximumViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.Maximum<DataEntityForDbRepositoryStatementBuilderForCrossCall>(e => e.Id,
                (object)null);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMaximum(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Maximum(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>(),
                new Field("Id", typeof(int)),
                (object)null);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMaximum(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateMaximumAll

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMaximumAll()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.MaximumAll<DataEntityForDbRepositoryStatementBuilder>(e => e.Id);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMaximumAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.MaximumAll<DataEntityForDbRepositoryStatementBuilder>(e => e.Id);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMaximumAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMaximumAllViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.MaximumAll(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                new Field("Id"));

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMaximumAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.MaximumAll(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                new Field("Id"));

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMaximumAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMaximumAllViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.MaximumAll<DataEntityForDbRepositoryStatementBuilderForCrossCall>(e => e.Id);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMaximumAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.MaximumAll(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>(),
                new Field("Id", typeof(int)));

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMaximumAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateMerge

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMerge()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            MergeExecutionContextCache<DataEntityForDbRepositoryStatementBuilder>.Flush();
            CommandTextCache.Flush();
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
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

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
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMergeViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            MergeExecutionContextCache<object>.Flush();
            CommandTextCache.Flush();
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
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

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
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMergeViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            MergeExecutionContextCache<DataEntityForDbRepositoryStatementBuilderForCrossCall>.Flush();
            MergeExecutionContextCache<object>.Flush();
            CommandTextCache.Flush();
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
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

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
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        #endregion

        #region CreateMergeAll

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMergeAll()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            MergeAllExecutionContextCache<DataEntityForDbRepositoryStatementBuilder>.Flush();
            CommandTextCache.Flush();
            repository.MergeAll<DataEntityForDbRepositoryStatementBuilder>(
                new[]
                {
                    new DataEntityForDbRepositoryStatementBuilder { Name = "Name1" },
                    new DataEntityForDbRepositoryStatementBuilder { Name = "Name2" },
                    new DataEntityForDbRepositoryStatementBuilder { Name = "Name3" }
                },
                Field.From(nameof(DataEntityForDbRepositoryStatementBuilder.Id)));

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMergeAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            repository.MergeAll<DataEntityForDbRepositoryStatementBuilder>(
                new[]
                {
                    new DataEntityForDbRepositoryStatementBuilder { Name = "Name1" },
                    new DataEntityForDbRepositoryStatementBuilder { Name = "Name2" },
                    new DataEntityForDbRepositoryStatementBuilder { Name = "Name3" }
                },
                Field.From(nameof(DataEntityForDbRepositoryStatementBuilder.Id)));

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMergeAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMergeAllWithSizePerBatchEqualsToOne()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            MergeAllExecutionContextCache<DataEntityForDbRepositoryStatementBuilder>.Flush();
            CommandTextCache.Flush();
            repository.MergeAll<DataEntityForDbRepositoryStatementBuilder>(
                new[]
                {
                    new DataEntityForDbRepositoryStatementBuilder { Name = "Name1" }
                },
                Field.From(nameof(DataEntityForDbRepositoryStatementBuilder.Id)),
                batchSize: 1);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            repository.MergeAll<DataEntityForDbRepositoryStatementBuilder>(
                new[]
                {
                    new DataEntityForDbRepositoryStatementBuilder { Name = "Name1" }
                },
                Field.From(nameof(DataEntityForDbRepositoryStatementBuilder.Id)),
                batchSize: 1);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMergeAllViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            MergeAllExecutionContextCache<object>.Flush();
            CommandTextCache.Flush();
            repository.MergeAll(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                new[]
                {
                    new { Name = "Name1" },
                    new { Name = "Name2" },
                    new { Name = "Name3" }
                },
                Field.From(nameof(DataEntityForDbRepositoryStatementBuilderForTableName.Id)));

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMergeAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            repository.MergeAll(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                new[]
                {
                    new { Name = "Name1" },
                    new { Name = "Name2" },
                    new { Name = "Name3" }
                },
                Field.From(nameof(DataEntityForDbRepositoryStatementBuilderForTableName.Id)));

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMergeAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMergeAllWithSizePerBatchEqualsToOneViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            MergeAllExecutionContextCache<object>.Flush();
            CommandTextCache.Flush();
            repository.MergeAll(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                new[]
                {
                    new { Name = "Name1" }
                },
                Field.From(nameof(DataEntityForDbRepositoryStatementBuilderForTableName.Id)),
                batchSize: 1);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            repository.MergeAll(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                new[]
                {
                    new { Name = "Name1" }
                },
                Field.From(nameof(DataEntityForDbRepositoryStatementBuilderForTableName.Id)),
                batchSize: 1);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMergeAllViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            MergeAllExecutionContextCache<DataEntityForDbRepositoryStatementBuilderForCrossCall>.Flush();
            MergeAllExecutionContextCache<object>.Flush();
            CommandTextCache.Flush();
            repository.MergeAll<DataEntityForDbRepositoryStatementBuilderForCrossCall>(
                new[]
                {
                    new DataEntityForDbRepositoryStatementBuilderForCrossCall { Name = "Name1" },
                    new DataEntityForDbRepositoryStatementBuilderForCrossCall { Name = "Name2" },
                    new DataEntityForDbRepositoryStatementBuilderForCrossCall { Name = "Name3" }
                },
                Field.From(nameof(DataEntityForDbRepositoryStatementBuilderForCrossCall.Id)));

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMergeAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            repository.MergeAll(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>(),
                new[]
                {
                    new { Name = "Name1" },
                    new { Name = "Name2" },
                    new { Name = "Name3" }
                },
                Field.From(nameof(DataEntityForDbRepositoryStatementBuilderForCrossCall.Id)),
                fields: FieldCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>());

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMergeAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMergeAllWithSizePerBatchEqualsToOneViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            MergeAllExecutionContextCache<DataEntityForDbRepositoryStatementBuilderForCrossCall>.Flush();
            MergeAllExecutionContextCache<object>.Flush();
            CommandTextCache.Flush();
            repository.MergeAll<DataEntityForDbRepositoryStatementBuilderForCrossCall>(
                new[]
                {
                    new DataEntityForDbRepositoryStatementBuilderForCrossCall { Name = "Name1" }
                },
                Field.From(nameof(DataEntityForDbRepositoryStatementBuilderForCrossCall.Id)),
                batchSize: 1);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            repository.MergeAll(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>(),
                new[]
                {
                    new { Name = "Name1" }
                },
                Field.From(nameof(DataEntityForDbRepositoryStatementBuilderForCrossCall.Id)),
                batchSize: 1,
                fields: FieldCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>());

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        #endregion

        #region CreateMinimum

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMinimum()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.Minimum<DataEntityForDbRepositoryStatementBuilder>(e => e.Id,
                (object)null);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMinimum(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Minimum<DataEntityForDbRepositoryStatementBuilder>(e => e.Id,
                (object)null);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMinimum(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMinimumViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.Minimum(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                new Field("Id"),
                (object)null);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMinimum(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Minimum(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                new Field("Id"),
                (object)null);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMinimum(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMinimumViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.Minimum<DataEntityForDbRepositoryStatementBuilderForCrossCall>(e => e.Id,
                (object)null);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMinimum(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Minimum(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>(),
                new Field("Id", typeof(int)),
                (object)null);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMinimum(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateMinimumAll

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMinimumAll()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.MinimumAll<DataEntityForDbRepositoryStatementBuilder>(e => e.Id);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMinimumAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.MinimumAll<DataEntityForDbRepositoryStatementBuilder>(e => e.Id);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMinimumAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMinimumAllViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.MinimumAll(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                new Field("Id"));

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMinimumAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.MinimumAll(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                new Field("Id"));

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMinimumAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMinimumAllViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.MinimumAll<DataEntityForDbRepositoryStatementBuilderForCrossCall>(e => e.Id);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMinimumAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.MinimumAll(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>(),
                new Field("Id", typeof(int)));

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMinimumAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateQuery

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForQuery()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
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

            // Act
            repository.Query<DataEntityForDbRepositoryStatementBuilder>(e => e.Id == 1);

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

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForQueryForTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.Query(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>(),
                new { Id = 1 });

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

            // Act
            repository.Query(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>(),
                new { Id = 1 });

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

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForQueryViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
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

            // Act
            repository.Query(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>(),
                new { Id = 1 });

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

        #endregion

        #region CreateQueryAll

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForQueryAll()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.QueryAll<DataEntityForDbRepositoryStatementBuilder>();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateQueryAll(
                    It.IsAny<QueryBuilder>(),
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            repository.QueryAll<DataEntityForDbRepositoryStatementBuilder>();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateQueryAll(
                    It.IsAny<QueryBuilder>(),
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForQueryAllForTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.QueryAll(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>());

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateQueryAll(
                    It.IsAny<QueryBuilder>(),
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            repository.QueryAll(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>());

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateQueryAll(
                    It.IsAny<QueryBuilder>(),
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForQueryAllViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.QueryAll<DataEntityForDbRepositoryStatementBuilder>();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateQueryAll(
                    It.IsAny<QueryBuilder>(),
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            repository.QueryAll(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>());

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateQueryAll(
                    It.IsAny<QueryBuilder>(),
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateQuery(Multple)

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForQueryMultiple()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
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
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

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

        #endregion

        #region CreateSum

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForSum()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.Sum<DataEntityForDbRepositoryStatementBuilder>(e => e.Id,
                (object)null);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateSum(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Sum<DataEntityForDbRepositoryStatementBuilder>(e => e.Id,
                (object)null);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateSum(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForSumViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.Sum(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                new Field("Id"),
                (object)null);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateSum(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Sum(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                new Field("Id"),
                (object)null);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateSum(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForSumViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.Sum<DataEntityForDbRepositoryStatementBuilderForCrossCall>(e => e.Id,
                (object)null);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateSum(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Sum(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>(),
                new Field("Id", typeof(int)),
                (object)null);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateSum(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateSumAll

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForSumAll()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.SumAll<DataEntityForDbRepositoryStatementBuilder>(e => e.Id);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateSumAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.SumAll<DataEntityForDbRepositoryStatementBuilder>(e => e.Id);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateSumAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForSumAllViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.SumAll(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                new Field("Id"));

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateSumAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.SumAll(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                new Field("Id"));

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateSumAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForSumAllViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.SumAll<DataEntityForDbRepositoryStatementBuilderForCrossCall>(e => e.Id);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateSumAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.SumAll(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>(),
                new Field("Id", typeof(int)));

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateSumAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateTruncate

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForTruncate()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.Truncate<DataEntityForDbRepositoryStatementBuilder>();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateTruncate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>())), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

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
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.Truncate(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>());

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateTruncate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>())), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

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
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.Truncate<DataEntityForDbRepositoryStatementBuilderForCrossCall>();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateTruncate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>())), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.Truncate(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>());

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateTruncate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>())), Times.Exactly(0));
        }

        #endregion

        #region CreateUpdate

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForUpdate()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            UpdateExecutionContextCache<DataEntityForDbRepositoryStatementBuilder>.Flush();
            CommandTextCache.Flush();
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
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

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
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForUpdateTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            UpdateExecutionContextCache<object>.Flush();
            CommandTextCache.Flush();
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
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

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
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForUpdateViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            UpdateExecutionContextCache<DataEntityForDbRepositoryStatementBuilderForCrossCall>.Flush();
            UpdateExecutionContextCache<object>.Flush();
            CommandTextCache.Flush();
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
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

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
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        #endregion

        #region CreateUpdateAll

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForUpdateAll()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            UpdateAllExecutionContextCache<DataEntityForDbRepositoryStatementBuilder>.Flush();
            CommandTextCache.Flush();
            repository.UpdateAll<DataEntityForDbRepositoryStatementBuilder>(
                new[]
                {
                    new DataEntityForDbRepositoryStatementBuilder { Name = "Name1" },
                    new DataEntityForDbRepositoryStatementBuilder { Name = "Name2" },
                    new DataEntityForDbRepositoryStatementBuilder { Name = "Name3" }
                },
                Field.From(nameof(DataEntityForDbRepositoryStatementBuilder.Id)));

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateUpdateAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            repository.UpdateAll<DataEntityForDbRepositoryStatementBuilder>(
                new[]
                {
                    new DataEntityForDbRepositoryStatementBuilder { Name = "Name1" },
                    new DataEntityForDbRepositoryStatementBuilder { Name = "Name2" },
                    new DataEntityForDbRepositoryStatementBuilder { Name = "Name3" }
                },
                Field.From(nameof(DataEntityForDbRepositoryStatementBuilder.Id)));

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateUpdateAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForUpdateAllViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            UpdateAllExecutionContextCache<object>.Flush();
            CommandTextCache.Flush();
            repository.UpdateAll(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                new[]
                {
                    new { Name = "Name1" },
                    new { Name = "Name2" },
                    new { Name = "Name3" }
                },
                Field.From(nameof(DataEntityForDbRepositoryStatementBuilderForTableName.Id)));

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateUpdateAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            repository.UpdateAll(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                new[]
                {
                    new { Name = "Name1" },
                    new { Name = "Name2" },
                    new { Name = "Name3" }
                },
                Field.From(nameof(DataEntityForDbRepositoryStatementBuilderForTableName.Id)));

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateUpdateAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForUpdateAllViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            UpdateAllExecutionContextCache<DataEntityForDbRepositoryStatementBuilderForCrossCall>.Flush();
            UpdateAllExecutionContextCache<object>.Flush();
            CommandTextCache.Flush();
            repository.UpdateAll<DataEntityForDbRepositoryStatementBuilderForCrossCall>(
                new[]
                {
                    new DataEntityForDbRepositoryStatementBuilderForCrossCall { Name = "Name1" },
                    new DataEntityForDbRepositoryStatementBuilderForCrossCall { Name = "Name2" },
                    new DataEntityForDbRepositoryStatementBuilderForCrossCall { Name = "Name3" }
                },
                Field.From(nameof(DataEntityForDbRepositoryStatementBuilderForCrossCall.Id)));

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateUpdateAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            repository.UpdateAll(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>(),
                new[]
                {
                    new { Name = "Name1" },
                    new { Name = "Name2" },
                    new { Name = "Name3" }
                },
                Field.From(nameof(DataEntityForDbRepositoryStatementBuilderForCrossCall.Id)),
                fields: FieldCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>());

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateUpdateAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        #endregion

        #endregion

        #region Async

        #region CreateAverage

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForAverageAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.AverageAsync<DataEntityForDbRepositoryStatementBuilder>(e => e.Id,
                (object)null).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateAverage(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.AverageAsync<DataEntityForDbRepositoryStatementBuilder>(e => e.Id,
                (object)null).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateAverage(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForAverageAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.AverageAsync(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                new Field("Id"),
                (object)null).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateAverage(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.AverageAsync(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                new Field("Id"),
                (object)null).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateAverage(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForAverageAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.AverageAsync<DataEntityForDbRepositoryStatementBuilderForCrossCall>(e => e.Id,
                (object)null).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateAverage(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.AverageAsync(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>(),
                new Field("Id", typeof(int)),
                (object)null).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateAverage(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateAverageAll

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForAverageAllAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.AverageAllAsync<DataEntityForDbRepositoryStatementBuilder>(e => e.Id).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateAverageAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.AverageAllAsync<DataEntityForDbRepositoryStatementBuilder>(e => e.Id).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateAverageAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForAverageAllAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.AverageAllAsync(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                new Field("Id")).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateAverageAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.AverageAllAsync(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                new Field("Id")).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateAverageAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForAverageAllAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.AverageAllAsync<DataEntityForDbRepositoryStatementBuilderForCrossCall>(e => e.Id).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateAverageAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.AverageAllAsync(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>(),
                new Field("Id", typeof(int))).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateAverageAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateBatchQueryAsync

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForBatchQueryAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.BatchQueryAsync<DataEntityForDbRepositoryStatementBuilder>(0,
                10,
                null,
                (object)null).Wait();

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
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.BatchQueryAsync<DataEntityForDbRepositoryStatementBuilder>(0,
                10,
                null,
                (object)null).Wait();

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

        #endregion

        #region CreateCountAsync

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForCountAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.CountAsync<DataEntityForDbRepositoryStatementBuilder>((object)null).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateCount(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.CountAsync<DataEntityForDbRepositoryStatementBuilder>((object)null).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateCount(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForCountAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.CountAsync(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                (object)null).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateCount(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.CountAsync(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                (object)null).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateCount(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForCountAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.CountAsync<DataEntityForDbRepositoryStatementBuilderForCrossCall>((object)null).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateCount(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.CountAsync(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>(),
                (object)null).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateCount(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateCountAllAsync

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForCountAllAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.CountAllAsync<DataEntityForDbRepositoryStatementBuilder>().Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateCountAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.CountAllAsync<DataEntityForDbRepositoryStatementBuilder>().Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateCountAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForCountAllAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.CountAllAsync(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateCountAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.CountAllAsync(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateCountAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForCountAllAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.CountAllAsync<DataEntityForDbRepositoryStatementBuilderForCrossCall>().Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateCountAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.CountAllAsync(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateCountAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateDeleteAsync

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForDeleteAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.DeleteAsync<DataEntityForDbRepositoryStatementBuilder>(e => e.Id == 1).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDelete(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<QueryGroup>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.DeleteAsync<DataEntityForDbRepositoryStatementBuilder>(e => e.Id == 1).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateDelete(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<QueryGroup>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForDeleteAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.DeleteAsync(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                new
                {
                    Id = 1
                }).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDelete(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<QueryGroup>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.DeleteAsync(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                new
                {
                    Id = 1
                }).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateDelete(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<QueryGroup>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForDeleteAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.DeleteAsync<DataEntityForDbRepositoryStatementBuilderForCrossCall>(e => e.Id == 1).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDelete(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<QueryGroup>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.DeleteAsync(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>(),
                new
                {
                    Id = 1
                }).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateDelete(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<QueryGroup>()), Times.Exactly(0));
        }

        #endregion

        #region CreateDeleteAllAsync

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForDeleteAllAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.DeleteAllAsync<DataEntityForDbRepositoryStatementBuilder>().Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDeleteAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>())), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.DeleteAllAsync<DataEntityForDbRepositoryStatementBuilder>().Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateDeleteAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>())), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForDeleteAllAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.DeleteAllAsync(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDeleteAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>())), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.DeleteAllAsync(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateDeleteAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>())), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForDeleteAllAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.DeleteAllAsync<DataEntityForDbRepositoryStatementBuilderForCrossCall>().Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDeleteAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>())), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.DeleteAllAsync(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateDeleteAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>())), Times.Exactly(0));
        }

        #endregion

        #region CreateInsertAsync

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForInsertAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            InsertExecutionContextCache<DataEntityForDbRepositoryStatementBuilder>.Flush();
            CommandTextCache.Flush();
            repository.InsertAsync<DataEntityForDbRepositoryStatementBuilder>(
                new DataEntityForDbRepositoryStatementBuilder
                {
                    Name = "Name"
                }).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.InsertAsync<DataEntityForDbRepositoryStatementBuilder>(
                new DataEntityForDbRepositoryStatementBuilder
                {
                    Name = "Name"
                }).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForInsertAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            InsertExecutionContextCache<object>.Flush();
            CommandTextCache.Flush();
            repository.InsertAsync(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                new
                {
                    Name = "Name"
                }).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.InsertAsync(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                new
                {
                    Name = "Name"
                }).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForInsertAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            InsertExecutionContextCache<DataEntityForDbRepositoryStatementBuilderForCrossCall>.Flush();
            InsertExecutionContextCache<object>.Flush();
            CommandTextCache.Flush();
            repository.InsertAsync<DataEntityForDbRepositoryStatementBuilderForCrossCall>(
                new DataEntityForDbRepositoryStatementBuilderForCrossCall
                {
                    Name = "Name"
                }).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.InsertAsync(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>(),
                new
                {
                    Id = 1,
                    Name = "Name"
                }).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        #endregion

        #region CreateInsertAllAsync

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForInsertAllAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            InsertAllExecutionContextCache<DataEntityForDbRepositoryStatementBuilder>.Flush();
            CommandTextCache.Flush();
            repository.InsertAllAsync<DataEntityForDbRepositoryStatementBuilder>(
                new[]
                {
                    new DataEntityForDbRepositoryStatementBuilder{ Name = "Name" },
                    new DataEntityForDbRepositoryStatementBuilder{ Name = "Name" }
                }).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsertAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.Is<int>(v => v > 1),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.InsertAllAsync<DataEntityForDbRepositoryStatementBuilder>(
                new[]
                {
                    new DataEntityForDbRepositoryStatementBuilder{ Name = "Name" },
                    new DataEntityForDbRepositoryStatementBuilder{ Name = "Name" }
                }).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsertAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.Is<int>(v => v > 1),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForInsertAllAsyncWithSizePerBatchEqualsToOne()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            InsertAllExecutionContextCache<DataEntityForDbRepositoryStatementBuilder>.Flush();
            CommandTextCache.Flush();
            repository.InsertAllAsync<DataEntityForDbRepositoryStatementBuilder>(
                new[]
                {
                    new DataEntityForDbRepositoryStatementBuilder{ Name = "Name" },
                    new DataEntityForDbRepositoryStatementBuilder{ Name = "Name" }
                },
                batchSize: 1).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.InsertAllAsync<DataEntityForDbRepositoryStatementBuilder>(
                new[]
                {
                    new DataEntityForDbRepositoryStatementBuilder{ Name = "Name" },
                    new DataEntityForDbRepositoryStatementBuilder{ Name = "Name" }
                },
                batchSize: 1).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForInsertAllAsyncForTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            InsertAllExecutionContextCache<object>.Flush();
            CommandTextCache.Flush();
            repository.InsertAllAsync(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                new[]
                {
                    new DataEntityForDbRepositoryStatementBuilderForTableName{ Name = "Name" },
                    new DataEntityForDbRepositoryStatementBuilderForTableName{ Name = "Name" }
                },
                fields: new[] { new Field("Id", typeof(int)), new Field("Name", typeof(string)) }).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsertAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.Is<int>(v => v > 1),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.InsertAllAsync(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                new[]
                {
                    new DataEntityForDbRepositoryStatementBuilderForTableName{ Name = "Name" },
                    new DataEntityForDbRepositoryStatementBuilderForTableName{ Name = "Name" }
                },
                fields: new[] { new Field("Id", typeof(int)), new Field("Name", typeof(string)) }).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsertAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.Is<int>(v => v > 1),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForInsertAllAsyncWithSizePerBatchEqualsToOneForTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            InsertAllExecutionContextCache<object>.Flush();
            CommandTextCache.Flush();
            repository.InsertAllAsync(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                new[]
                {
                    new DataEntityForDbRepositoryStatementBuilderForTableName{ Name = "Name" },
                    new DataEntityForDbRepositoryStatementBuilderForTableName{ Name = "Name" }
                },
                fields: new[] { new Field("Id", typeof(int)), new Field("Name", typeof(string)) },
                batchSize: 1).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.InsertAllAsync(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                new[]
                {
                    new DataEntityForDbRepositoryStatementBuilderForTableName{ Name = "Name" },
                    new DataEntityForDbRepositoryStatementBuilderForTableName{ Name = "Name" }
                },
                fields: new[] { new Field("Id", typeof(int)), new Field("Name", typeof(string)) },
                batchSize: 1).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForInsertAllAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            InsertAllExecutionContextCache<DataEntityForDbRepositoryStatementBuilderForCrossCall>.Flush();
            InsertAllExecutionContextCache<object>.Flush();
            CommandTextCache.Flush();
            repository.InsertAllAsync<DataEntityForDbRepositoryStatementBuilderForCrossCall>(
                new[]
                {
                    new DataEntityForDbRepositoryStatementBuilderForCrossCall { Name = "Name" },
                    new DataEntityForDbRepositoryStatementBuilderForCrossCall { Name = "Name" }
                }).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsertAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.Is<int>(v => v > 1),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.InsertAllAsync(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>(),
                new[]
                {
                    new DataEntityForDbRepositoryStatementBuilderForCrossCall { Name = "Name" },
                    new DataEntityForDbRepositoryStatementBuilderForCrossCall { Name = "Name" }
                },
                fields: new[] { new Field("Id", typeof(int)), new Field("Name", typeof(string)) }).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsertAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.Is<int>(v => v > 1),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForInsertAllAsyncWithSizePerBatchEqualsToOneViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            InsertAllExecutionContextCache<DataEntityForDbRepositoryStatementBuilderForCrossCall>.Flush();
            InsertAllExecutionContextCache<object>.Flush();
            CommandTextCache.Flush();
            repository.InsertAllAsync<DataEntityForDbRepositoryStatementBuilderForCrossCall>(
                new[]
                {
                    new DataEntityForDbRepositoryStatementBuilderForCrossCall { Name = "Name" },
                    new DataEntityForDbRepositoryStatementBuilderForCrossCall { Name = "Name" }
                },
                batchSize: 1).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.InsertAllAsync(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>(),
                new[]
                {
                    new DataEntityForDbRepositoryStatementBuilderForCrossCall { Name = "Name" },
                    new DataEntityForDbRepositoryStatementBuilderForCrossCall { Name = "Name" }
                },
                fields: new[] { new Field("Id", typeof(int)), new Field("Name", typeof(string)) },
                batchSize: 1).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        #endregion

        #region CreateMaximum

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMaximumAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.MaximumAsync<DataEntityForDbRepositoryStatementBuilder>(e => e.Id,
                (object)null).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMaximum(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.MaximumAsync<DataEntityForDbRepositoryStatementBuilder>(e => e.Id,
                (object)null).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMaximum(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMaximumAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.MaximumAsync(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                new Field("Id"),
                (object)null).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMaximum(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.MaximumAsync(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                new Field("Id"),
                (object)null).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMaximum(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMaximumAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.MaximumAsync<DataEntityForDbRepositoryStatementBuilderForCrossCall>(e => e.Id,
                (object)null).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMaximum(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.MaximumAsync(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>(),
                new Field("Id", typeof(int)),
                (object)null).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMaximum(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateMaximumAll

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMaximumAllAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.MaximumAllAsync<DataEntityForDbRepositoryStatementBuilder>(e => e.Id).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMaximumAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.MaximumAllAsync<DataEntityForDbRepositoryStatementBuilder>(e => e.Id).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMaximumAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMaximumAllAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.MaximumAllAsync(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                new Field("Id")).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMaximumAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.MaximumAllAsync(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                new Field("Id")).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMaximumAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMaximumAllAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.MaximumAllAsync<DataEntityForDbRepositoryStatementBuilderForCrossCall>(e => e.Id).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMaximumAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.MaximumAllAsync(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>(),
                new Field("Id", typeof(int))).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMaximumAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateMergeAsync

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMergeAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            MergeExecutionContextCache<DataEntityForDbRepositoryStatementBuilder>.Flush();
            CommandTextCache.Flush();
            repository.MergeAsync<DataEntityForDbRepositoryStatementBuilder>(
                new DataEntityForDbRepositoryStatementBuilder
                {
                    Name = "Name"
                },
                new Field(nameof(DataEntityForDbRepositoryStatementBuilder.Id))).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.MergeAsync<DataEntityForDbRepositoryStatementBuilder>(
                new DataEntityForDbRepositoryStatementBuilder
                {
                    Name = "Name"
                },
                new Field(nameof(DataEntityForDbRepositoryStatementBuilder.Id))).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMergeAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            MergeExecutionContextCache<object>.Flush();
            CommandTextCache.Flush();
            repository.MergeAsync(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                new
                {
                    Name = "Name"
                },
                new Field(nameof(DataEntityForDbRepositoryStatementBuilderForTableName.Id))).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.MergeAsync(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                new
                {
                    Name = "Name"
                },
                new Field(nameof(DataEntityForDbRepositoryStatementBuilderForTableName.Id))).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMergeAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            MergeExecutionContextCache<DataEntityForDbRepositoryStatementBuilderForCrossCall>.Flush();
            MergeExecutionContextCache<object>.Flush();
            CommandTextCache.Flush();
            repository.MergeAsync<DataEntityForDbRepositoryStatementBuilderForCrossCall>(
                new DataEntityForDbRepositoryStatementBuilderForCrossCall
                {
                    Name = "Name"
                },
                new Field(nameof(DataEntityForDbRepositoryStatementBuilderForCrossCall.Id))).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.MergeAsync(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>(),
                new
                {
                    Id = 1,
                    Name = "Name"
                },
                new Field(nameof(DataEntityForDbRepositoryStatementBuilderForCrossCall.Id))).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        #endregion

        #region CreateMergeAllAsync

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMergeAllAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            MergeAllExecutionContextCache<DataEntityForDbRepositoryStatementBuilder>.Flush();
            CommandTextCache.Flush();
            repository.MergeAllAsync<DataEntityForDbRepositoryStatementBuilder>(
                new[]
                {
                    new DataEntityForDbRepositoryStatementBuilder { Name = "Name1" },
                    new DataEntityForDbRepositoryStatementBuilder { Name = "Name2" },
                    new DataEntityForDbRepositoryStatementBuilder { Name = "Name3" }
                },
                Field.From(nameof(DataEntityForDbRepositoryStatementBuilder.Id))).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMergeAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            repository.MergeAllAsync<DataEntityForDbRepositoryStatementBuilder>(
                new[]
                {
                    new DataEntityForDbRepositoryStatementBuilder { Name = "Name1" },
                    new DataEntityForDbRepositoryStatementBuilder { Name = "Name2" },
                    new DataEntityForDbRepositoryStatementBuilder { Name = "Name3" }
                },
                Field.From(nameof(DataEntityForDbRepositoryStatementBuilder.Id))).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMergeAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMergeAllAsyncWithSizePerBatchEqualsToOne()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            MergeAllExecutionContextCache<DataEntityForDbRepositoryStatementBuilder>.Flush();
            CommandTextCache.Flush();
            repository.MergeAllAsync<DataEntityForDbRepositoryStatementBuilder>(
                new[]
                {
                    new DataEntityForDbRepositoryStatementBuilder { Name = "Name1" }
                },
                Field.From(nameof(DataEntityForDbRepositoryStatementBuilder.Id)),
                batchSize: 1).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            repository.MergeAllAsync<DataEntityForDbRepositoryStatementBuilder>(
                new[]
                {
                    new DataEntityForDbRepositoryStatementBuilder { Name = "Name1" }
                },
                Field.From(nameof(DataEntityForDbRepositoryStatementBuilder.Id)),
                batchSize: 1).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMergeAllAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            MergeAllExecutionContextCache<object>.Flush();
            CommandTextCache.Flush();
            repository.MergeAllAsync(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                new[]
                {
                    new { Name = "Name1" },
                    new { Name = "Name2" },
                    new { Name = "Name3" }
                },
                Field.From(nameof(DataEntityForDbRepositoryStatementBuilderForTableName.Id))).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMergeAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            repository.MergeAllAsync(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                new[]
                {
                    new { Name = "Name1" },
                    new { Name = "Name2" },
                    new { Name = "Name3" }
                },
                Field.From(nameof(DataEntityForDbRepositoryStatementBuilderForTableName.Id))).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMergeAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMergeAllAsyncWithSizePerBatchEqualsToOneViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            MergeAllExecutionContextCache<object>.Flush();
            CommandTextCache.Flush();
            repository.MergeAllAsync(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                new[]
                {
                    new { Name = "Name1" }
                },
                Field.From(nameof(DataEntityForDbRepositoryStatementBuilderForTableName.Id)),
                batchSize: 1).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            repository.MergeAllAsync(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                new[]
                {
                    new { Name = "Name1" }
                },
                Field.From(nameof(DataEntityForDbRepositoryStatementBuilderForTableName.Id)),
                batchSize: 1).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMergeAllAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            MergeAllExecutionContextCache<DataEntityForDbRepositoryStatementBuilderForCrossCall>.Flush();
            MergeAllExecutionContextCache<object>.Flush();
            CommandTextCache.Flush();
            repository.MergeAllAsync<DataEntityForDbRepositoryStatementBuilderForCrossCall>(
                new[]
                {
                    new DataEntityForDbRepositoryStatementBuilderForCrossCall { Name = "Name1" },
                    new DataEntityForDbRepositoryStatementBuilderForCrossCall { Name = "Name2" },
                    new DataEntityForDbRepositoryStatementBuilderForCrossCall { Name = "Name3" }
                },
                Field.From(nameof(DataEntityForDbRepositoryStatementBuilderForCrossCall.Id))).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMergeAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            repository.MergeAllAsync(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>(),
                new[]
                {
                    new { Name = "Name1" },
                    new { Name = "Name2" },
                    new { Name = "Name3" }
                },
                Field.From(nameof(DataEntityForDbRepositoryStatementBuilderForCrossCall.Id)),
                fields: FieldCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMergeAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMergeAllAsyncWithSizePerBatchEqualsToOneViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            MergeAllExecutionContextCache<DataEntityForDbRepositoryStatementBuilderForCrossCall>.Flush();
            MergeAllExecutionContextCache<object>.Flush();
            CommandTextCache.Flush();
            repository.MergeAllAsync<DataEntityForDbRepositoryStatementBuilderForCrossCall>(
                new[]
                {
                    new DataEntityForDbRepositoryStatementBuilderForCrossCall { Name = "Name1" }
                },
                Field.From(nameof(DataEntityForDbRepositoryStatementBuilderForCrossCall.Id)),
                batchSize: 1).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            repository.MergeAllAsync(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>(),
                new[]
                {
                    new { Name = "Name1" }
                },
                Field.From(nameof(DataEntityForDbRepositoryStatementBuilderForCrossCall.Id)),
                batchSize: 1,
                fields: FieldCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        #endregion

        #region CreateMinimum

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMinimumAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.MinimumAsync<DataEntityForDbRepositoryStatementBuilder>(e => e.Id,
                (object)null).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMinimum(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.MinimumAsync<DataEntityForDbRepositoryStatementBuilder>(e => e.Id,
                (object)null).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMinimum(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMinimumAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.MinimumAsync(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                new Field("Id"),
                (object)null).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMinimum(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.MinimumAsync(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                new Field("Id"),
                (object)null).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMinimum(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMinimumAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.MinimumAsync<DataEntityForDbRepositoryStatementBuilderForCrossCall>(e => e.Id,
                (object)null).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMinimum(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.MinimumAsync(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>(),
                new Field("Id", typeof(int)),
                (object)null).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMinimum(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateMinimumAll

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMinimumAllAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.MinimumAllAsync<DataEntityForDbRepositoryStatementBuilder>(e => e.Id).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMinimumAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.MinimumAllAsync<DataEntityForDbRepositoryStatementBuilder>(e => e.Id).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMinimumAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMinimumAllAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.MinimumAllAsync(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                new Field("Id")).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMinimumAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.MinimumAllAsync(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                new Field("Id")).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMinimumAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForMinimumAllAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.MinimumAllAsync<DataEntityForDbRepositoryStatementBuilderForCrossCall>(e => e.Id).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMinimumAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.MinimumAllAsync(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>(),
                new Field("Id", typeof(int))).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMinimumAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateQueryAsync

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForQueryAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.QueryAsync<DataEntityForDbRepositoryStatementBuilder>(e => e.Id == 1).Wait();

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

            // Act
            repository.QueryAsync<DataEntityForDbRepositoryStatementBuilder>(e => e.Id == 1).Wait();

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

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForQueryAsyncForTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.QueryAsync(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>(),
                new { Id = 1 }).Wait();

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

            // Act
            repository.QueryAsync(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>(),
                new { Id = 1 }).Wait();

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

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForQueryAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.QueryAsync<DataEntityForDbRepositoryStatementBuilder>(e => e.Id == 1).Wait();

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

            // Act
            repository.QueryAsync(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>(),
                new { Id = 1 }).Wait();

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

        #endregion

        #region CreateQueryAllAsync

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForQueryAllAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.QueryAllAsync<DataEntityForDbRepositoryStatementBuilder>().Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateQueryAll(
                    It.IsAny<QueryBuilder>(),
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            repository.QueryAllAsync<DataEntityForDbRepositoryStatementBuilder>().Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateQueryAll(
                    It.IsAny<QueryBuilder>(),
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForQueryAllAsyncForTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.QueryAllAsync(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateQueryAll(
                    It.IsAny<QueryBuilder>(),
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            repository.QueryAllAsync(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateQueryAll(
                    It.IsAny<QueryBuilder>(),
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForQueryAllAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.QueryAllAsync<DataEntityForDbRepositoryStatementBuilder>().Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateQueryAll(
                    It.IsAny<QueryBuilder>(),
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            repository.QueryAllAsync(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateQueryAll(
                    It.IsAny<QueryBuilder>(),
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateQueryAsync(Multple)

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForQueryMultipleAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.QueryMultipleAsync<DataEntityForDbRepositoryStatementBuilderT1,
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
                e => e.Id == 1).Wait();

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
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.QueryMultipleAsync<DataEntityForDbRepositoryStatementBuilderT1,
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
                e => e.Id == 1).Wait();

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

        #endregion

        #region CreateSum

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForSumAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.SumAsync<DataEntityForDbRepositoryStatementBuilder>(e => e.Id,
                (object)null).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateSum(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.SumAsync<DataEntityForDbRepositoryStatementBuilder>(e => e.Id,
                (object)null).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateSum(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForSumAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.SumAsync(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                new Field("Id"),
                (object)null).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateSum(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.SumAsync(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                new Field("Id"),
                (object)null).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateSum(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForSumAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.SumAsync<DataEntityForDbRepositoryStatementBuilderForCrossCall>(e => e.Id,
                (object)null).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateSum(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.SumAsync(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>(),
                new Field("Id", typeof(int)),
                (object)null).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateSum(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateSumAll

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForSumAllAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.SumAllAsync<DataEntityForDbRepositoryStatementBuilder>(e => e.Id).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateSumAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.SumAllAsync<DataEntityForDbRepositoryStatementBuilder>(e => e.Id).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateSumAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForSumAllAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.SumAllAsync(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                new Field("Id")).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateSumAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.SumAllAsync(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                new Field("Id")).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateSumAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForSumAllAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.SumAllAsync<DataEntityForDbRepositoryStatementBuilderForCrossCall>(e => e.Id).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateSumAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.SumAllAsync(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>(),
                new Field("Id", typeof(int))).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateSumAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateTruncate

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForTruncateAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.TruncateAsync<DataEntityForDbRepositoryStatementBuilder>().Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateTruncate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>())), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.TruncateAsync<DataEntityForDbRepositoryStatementBuilder>().Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateTruncate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>())), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForTruncateAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.TruncateAsync(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateTruncate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>())), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.TruncateAsync(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateTruncate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>())), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForTruncateAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            CommandTextCache.Flush();
            repository.TruncateAsync<DataEntityForDbRepositoryStatementBuilderForCrossCall>().Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateTruncate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>())), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.TruncateAsync(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateTruncate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>())), Times.Exactly(0));
        }

        #endregion

        #region CreateUpdateAsync

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForUpdateAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            UpdateExecutionContextCache<DataEntityForDbRepositoryStatementBuilder>.Flush();
            CommandTextCache.Flush();
            repository.UpdateAsync<DataEntityForDbRepositoryStatementBuilder>(
                new DataEntityForDbRepositoryStatementBuilder
                {
                    Name = "Update"
                },
                e => e.Id == 1).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateUpdate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.UpdateAsync<DataEntityForDbRepositoryStatementBuilder>(
                new DataEntityForDbRepositoryStatementBuilder
                {
                    Name = "Update"
                },
                e => e.Id == 1).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateUpdate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForUpdateAsyncTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            UpdateExecutionContextCache<object>.Flush();
            CommandTextCache.Flush();
            repository.UpdateAsync(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                new DataEntityForDbRepositoryStatementBuilderForTableName
                {
                    Name = "Update"
                },
                new { Id = 1 }).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateUpdate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.UpdateAsync(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                new DataEntityForDbRepositoryStatementBuilderForTableName
                {
                    Name = "Update"
                },
                new { Id = 1 }).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateUpdate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForUpdateAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            UpdateExecutionContextCache<DataEntityForDbRepositoryStatementBuilderForCrossCall>.Flush();
            UpdateExecutionContextCache<object>.Flush();
            CommandTextCache.Flush();
            repository.UpdateAsync<DataEntityForDbRepositoryStatementBuilderForCrossCall>(
                new DataEntityForDbRepositoryStatementBuilderForCrossCall
                {
                    Name = "Update"
                },
                e => e.Id == 1).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateUpdate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();
            var repositoryNever = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilderNever.Object);

            // Act
            repositoryNever.UpdateAsync(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>(),
                new DataEntityForDbRepositoryStatementBuilderForCrossCall
                {
                    Id = 1,
                    Name = "Update"
                },
                new { Id = 1 }).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateUpdate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        #endregion

        #region CreateUpdateAllAsync

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForUpdateAllAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            UpdateAllExecutionContextCache<DataEntityForDbRepositoryStatementBuilder>.Flush();
            CommandTextCache.Flush();
            repository.UpdateAllAsync<DataEntityForDbRepositoryStatementBuilder>(
                new[]
                {
                    new DataEntityForDbRepositoryStatementBuilder { Name = "Name1" },
                    new DataEntityForDbRepositoryStatementBuilder { Name = "Name2" },
                    new DataEntityForDbRepositoryStatementBuilder { Name = "Name3" }
                },
                Field.From(nameof(DataEntityForDbRepositoryStatementBuilder.Id))).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateUpdateAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            repository.UpdateAllAsync<DataEntityForDbRepositoryStatementBuilder>(
                new[]
                {
                    new DataEntityForDbRepositoryStatementBuilder { Name = "Name1" },
                    new DataEntityForDbRepositoryStatementBuilder { Name = "Name2" },
                    new DataEntityForDbRepositoryStatementBuilder { Name = "Name3" }
                },
                Field.From(nameof(DataEntityForDbRepositoryStatementBuilder.Id))).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateUpdateAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilder>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForUpdateAllAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            UpdateAllExecutionContextCache<object>.Flush();
            CommandTextCache.Flush();
            repository.UpdateAllAsync(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                new[]
                {
                    new { Name = "Name1" },
                    new { Name = "Name2" },
                    new { Name = "Name3" }
                },
                Field.From(nameof(DataEntityForDbRepositoryStatementBuilderForTableName.Id))).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateUpdateAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            repository.UpdateAllAsync(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>(),
                new[]
                {
                    new { Name = "Name1" },
                    new { Name = "Name2" },
                    new { Name = "Name3" }
                },
                Field.From(nameof(DataEntityForDbRepositoryStatementBuilderForTableName.Id))).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateUpdateAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbRepositoryStatementBuilderForUpdateAllAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var repository = new DbRepository<CustomDbConnectionForDbRepositoryIStatementBuilder>("ConnectionString", statementBuilder.Object);

            // Act
            UpdateAllExecutionContextCache<DataEntityForDbRepositoryStatementBuilderForCrossCall>.Flush();
            UpdateAllExecutionContextCache<object>.Flush();
            CommandTextCache.Flush();
            repository.UpdateAllAsync<DataEntityForDbRepositoryStatementBuilderForCrossCall>(
                new[]
                {
                    new DataEntityForDbRepositoryStatementBuilderForCrossCall { Name = "Name1" },
                    new DataEntityForDbRepositoryStatementBuilderForCrossCall { Name = "Name2" },
                    new DataEntityForDbRepositoryStatementBuilderForCrossCall { Name = "Name3" }
                },
                Field.From(nameof(DataEntityForDbRepositoryStatementBuilderForCrossCall.Id))).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateUpdateAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            repository.UpdateAllAsync(ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>(),
                new[]
                {
                    new { Name = "Name1" },
                    new { Name = "Name2" },
                    new { Name = "Name3" }
                },
                Field.From(nameof(DataEntityForDbRepositoryStatementBuilderForCrossCall.Id)),
                fields: FieldCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateUpdateAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbRepositoryStatementBuilderForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        #endregion

        #endregion
    }
}
