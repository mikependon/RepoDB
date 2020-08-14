using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoDb.Contexts.Cachers;
using RepoDb.Enumerations;
using RepoDb.Interfaces;
using RepoDb.UnitTests.CustomObjects;
using System.Collections.Generic;

namespace RepoDb.UnitTests.Interfaces
{
    [TestClass]
    public class IStatementBuilderForDbConnectionTest
    {
        [TestInitialize]
        public void Initialize()
        {
            DbSettingMapper.Add(typeof(StatementBuilderDbConnection), new CustomDbSetting(), true);
            DbHelperMapper.Add(typeof(StatementBuilderDbConnection), new CustomDbHelper(), true);
        }

        #region SubClasses

        private class StatementBuilderDbConnection : CustomDbConnection { }

        private class StatementBuilderEntity
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private class StatementBuilderEntityForTableName
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private class StatementBuilderEntityForCrossCall
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private class StatementBuilderEntityT1
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private class StatementBuilderEntityT2
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private class StatementBuilderEntityT3
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private class StatementBuilderEntityT4
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private class StatementBuilderEntityT5
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private class StatementBuilderEntityT6
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private class StatementBuilderEntityT7
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        #endregion

        #region Sync

        #region CreateAverage

        [TestMethod]
        public void TestDbConnectionStatementBuilderForAverage()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.Average<StatementBuilderEntity>(e => e.Id,
                (object)null,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateAverage(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.Average<StatementBuilderEntity>(e => e.Id,
                (object)null,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateAverage(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForAverageViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.Average(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"),
                (object)null,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateAverage(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.Average(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"),
                (object)null,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateAverage(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForAverageViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.Average<StatementBuilderEntityForCrossCall>(e => e.Id,
                (object)null,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateAverage(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.Average(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new Field("Id", typeof(int)),
                (object)null,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateAverage(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateAverageAll

        [TestMethod]
        public void TestDbConnectionStatementBuilderForAverageAll()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.AverageAll<StatementBuilderEntity>(e => e.Id,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateAverageAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.AverageAll<StatementBuilderEntity>(e => e.Id,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateAverageAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForAverageAllViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.AverageAll(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"),
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateAverageAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.AverageAll(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateAverageAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForAverageAllViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.AverageAll<StatementBuilderEntityForCrossCall>(e => e.Id,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateAverageAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.AverageAll(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new Field("Id", typeof(int)),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateAverageAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateBatchQuery

        [TestMethod]
        public void TestDbConnectionStatementBuilderForBatchQuery()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.BatchQuery<StatementBuilderEntity>(page: 0,
                rowsPerBatch: 10,
                orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                where: (QueryGroup)null,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateBatchQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.BatchQuery<StatementBuilderEntity>(page: 0,
                rowsPerBatch: 10,
                orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                where: (QueryGroup)null,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateBatchQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
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
        public void TestDbConnectionStatementBuilderForCount()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.Count<StatementBuilderEntity>((object)null,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateCount(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.Count<StatementBuilderEntity>((object)null,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateCount(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForCountViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.Count(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                (object)null,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateCount(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.Count(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                (object)null,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateCount(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForCountViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.Count<StatementBuilderEntityForCrossCall>((object)null,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateCount(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.Count(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                (object)null,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateCount(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateCountAll

        [TestMethod]
        public void TestDbConnectionStatementBuilderForCountAll()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.CountAll<StatementBuilderEntity>(statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateCountAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.CountAll<StatementBuilderEntity>(statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateCountAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForCountAllViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.CountAll(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateCountAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.CountAll(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateCountAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForCountAllViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.CountAll<StatementBuilderEntityForCrossCall>(statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateCountAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.CountAll(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateCountAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateDelete

        [TestMethod]
        public void TestDbConnectionStatementBuilderForDelete()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.Delete<StatementBuilderEntity>(e => e.Id == 1,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDelete(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.Delete<StatementBuilderEntity>(e => e.Id == 1,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateDelete(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForDeleteViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.Delete(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new
                {
                    Id = 1
                },
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDelete(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.Delete(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new
                {
                    Id = 1
                },
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateDelete(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForDeleteViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.Delete<StatementBuilderEntityForCrossCall>(e => e.Id == 1,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDelete(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.Delete(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new
                {
                    Id = 1
                },
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateDelete(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateDeleteAll

        [TestMethod]
        public void TestDbConnectionStatementBuilderForDeleteAll()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.DeleteAll<StatementBuilderEntity>(statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDeleteAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.DeleteAll<StatementBuilderEntity>(statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateDeleteAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForDeleteAllViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.DeleteAll(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDeleteAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.DeleteAll(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateDeleteAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForDeleteAllViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.DeleteAll<StatementBuilderEntityForCrossCall>(statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDeleteAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.DeleteAll(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateDeleteAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateExists

        [TestMethod]
        public void TestDbConnectionStatementBuilderForExists()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.Exists<StatementBuilderEntity>((object)null,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateExists(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.Exists<StatementBuilderEntity>((object)null,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateExists(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForExistsViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.Exists(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                (object)null,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateExists(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.Exists(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                (object)null,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateExists(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForExistsViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.Exists<StatementBuilderEntityForCrossCall>((object)null,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateExists(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.Exists(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                (object)null,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateExists(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateInsert

        [TestMethod]
        public void TestDbConnectionStatementBuilderForInsert()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            InsertExecutionContextCache.Flush();
            CommandTextCache.Flush();
            connection.Insert<StatementBuilderEntity>(
                new StatementBuilderEntity
                {
                    Name = "Name"
                },
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.Insert<StatementBuilderEntity>(
                new StatementBuilderEntity
                {
                    Name = "Name"
                },
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForInsertForTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            InsertExecutionContextCache.Flush();
            CommandTextCache.Flush();
            connection.Insert(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new
                {
                    Name = "Name"
                },
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.Insert(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new
                {
                    Name = "Name"
                },
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForInsertViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            InsertExecutionContextCache.Flush();
            CommandTextCache.Flush();
            connection.Insert<StatementBuilderEntityForCrossCall>(
                new StatementBuilderEntityForCrossCall
                {
                    Name = "Name"
                },
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.Insert(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new
                {
                    Id = 1,
                    Name = "Name"
                },
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateInsertAll

        [TestMethod]
        public void TestDbConnectionStatementBuilderForInsertAll()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            InsertAllExecutionContextCache.Flush();
            CommandTextCache.Flush();
            connection.InsertAll<StatementBuilderEntity>(new[]
            {
                new StatementBuilderEntity{ Name = "Name1" },
                new StatementBuilderEntity{ Name = "Name2" },
                new StatementBuilderEntity{ Name = "Name3" }
            },
            statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsertAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.InsertAll<StatementBuilderEntity>(new[]
            {
                new StatementBuilderEntity{ Name = "Name1" },
                new StatementBuilderEntity{ Name = "Name2" },
                new StatementBuilderEntity{ Name = "Name3" }
            },
            statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsertAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForInsertAllWithSizePerBatchEqualsToOne()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            InsertAllExecutionContextCache.Flush();
            CommandTextCache.Flush();
            connection.InsertAll<StatementBuilderEntity>(new[]
            {
                new StatementBuilderEntity{ Name = "Name" }
            },
            batchSize: 1,
            statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.InsertAll<StatementBuilderEntity>(new[]
            {
                new StatementBuilderEntity{ Name = "Name" }
            },
            batchSize: 1,
            statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForInsertAllForTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            InsertAllExecutionContextCache.Flush();
            CommandTextCache.Flush();
            connection.InsertAll(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new[]
                {
                    new { Name = "Name1" },
                    new { Name = "Name2" },
                    new { Name = "Name3" }
                },
                fields: new[] { new Field("Id", typeof(int)), new Field("Name", typeof(string)) },
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsertAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.InsertAll(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new[]
                {
                    new { Name = "Name1" },
                    new { Name = "Name2" },
                    new { Name = "Name3" }
                },
                fields: new[] { new Field("Id", typeof(int)), new Field("Name", typeof(string)) },
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsertAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForInsertAllWithSizePerBatchEqualsToOneForTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            InsertAllExecutionContextCache.Flush();
            CommandTextCache.Flush();
            connection.InsertAll(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new[]
                {
                    new { Name = "Name" }
                },
                fields: new[] { new Field("Id", typeof(int)), new Field("Name", typeof(string)) },
                batchSize: 1,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.InsertAll(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new[]
                {
                    new { Name = "Name" }
                },
                fields: new[] { new Field("Id", typeof(int)), new Field("Name", typeof(string)) },
                batchSize: 1,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForInsertAllViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            InsertAllExecutionContextCache.Flush();
            CommandTextCache.Flush();
            connection.InsertAll<StatementBuilderEntityForCrossCall>(
                new[]
                {
                    new StatementBuilderEntityForCrossCall { Name = "Name1" },
                    new StatementBuilderEntityForCrossCall { Name = "Name2" },
                    new StatementBuilderEntityForCrossCall { Name = "Name3" }
                },
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsertAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.InsertAll(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new[]
                {
                    new { Name = "Name1" },
                    new { Name = "Name2" },
                    new { Name = "Name3" }
                },
                fields: new[] { new Field("Id", typeof(int)), new Field("Name", typeof(string)) },
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsertAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForInsertAllWithSizePerBatchEqualsToOneViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            InsertAllExecutionContextCache.Flush();
            CommandTextCache.Flush();
            connection.InsertAll<StatementBuilderEntityForCrossCall>(
                new[]
                {
                    new StatementBuilderEntityForCrossCall { Name = "Name" }
                },
                batchSize: 1,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.InsertAll(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new[]
                {
                    new { Name = "Name" }
                },
                fields: new[] { new Field("Id", typeof(int)), new Field("Name", typeof(string)) },
                batchSize: 1,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateMax

        [TestMethod]
        public void TestDbConnectionStatementBuilderForMax()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.Max<StatementBuilderEntity>(e => e.Id,
                (object)null,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMax(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.Max<StatementBuilderEntity>(e => e.Id,
                (object)null,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMax(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForMaxViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.Max(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"),
                (object)null,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMax(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.Max(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"),
                (object)null,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMax(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForMaxViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.Max<StatementBuilderEntityForCrossCall>(e => e.Id,
                (object)null,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMax(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.Max(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new Field("Id", typeof(int)),
                (object)null,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMax(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateMaxAll

        [TestMethod]
        public void TestDbConnectionStatementBuilderForMaxAll()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.MaxAll<StatementBuilderEntity>(e => e.Id,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMaxAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.MaxAll<StatementBuilderEntity>(e => e.Id,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMaxAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForMaxAllViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.MaxAll(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"),
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMaxAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.MaxAll(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMaxAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForMaxAllViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.MaxAll<StatementBuilderEntityForCrossCall>(e => e.Id,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMaxAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.MaxAll(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new Field("Id", typeof(int)),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMaxAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateMerge

        [TestMethod]
        public void TestDbConnectionStatementBuilderForMerge()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            MergeExecutionContextCache.Flush();
            CommandTextCache.Flush();
            connection.Merge<StatementBuilderEntity>(
                new StatementBuilderEntity
                {
                    Name = "Name"
                },
                new Field(nameof(StatementBuilderEntity.Id)),
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.Merge<StatementBuilderEntity>(
                new StatementBuilderEntity
                {
                    Name = "Name"
                },
                new Field(nameof(StatementBuilderEntity.Id)),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForMergeViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            MergeExecutionContextCache.Flush();
            CommandTextCache.Flush();
            connection.Merge(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new
                {
                    Name = "Name"
                },
                new Field(nameof(StatementBuilderEntityForTableName.Id)),
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.Merge(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new
                {
                    Name = "Name"
                },
                new Field(nameof(StatementBuilderEntityForTableName.Id)),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForMergeViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            MergeExecutionContextCache.Flush();
            CommandTextCache.Flush();
            connection.Merge<StatementBuilderEntityForCrossCall>(
                new StatementBuilderEntityForCrossCall
                {
                    Name = "Name"
                },
                new Field(nameof(StatementBuilderEntityForCrossCall.Id)),
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.Merge(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new
                {
                    Id = 1,
                    Name = "Name"
                },
                new Field(nameof(StatementBuilderEntityForCrossCall.Id)),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateMergeAll

        [TestMethod]
        public void TestDbConnectionStatementBuilderForMergeAll()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            MergeAllExecutionContextCache.Flush();
            CommandTextCache.Flush();
            connection.MergeAll<StatementBuilderEntity>(
                new[]
                {
                    new StatementBuilderEntity { Name = "Name1" },
                    new StatementBuilderEntity { Name = "Name2" },
                    new StatementBuilderEntity { Name = "Name3" }
                },
                new Field(nameof(StatementBuilderEntity.Id)),
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMergeAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.MergeAll<StatementBuilderEntity>(
                new[]
                {
                    new StatementBuilderEntity { Name = "Name1" },
                    new StatementBuilderEntity { Name = "Name2" },
                    new StatementBuilderEntity { Name = "Name3" }
                },
                new Field(nameof(StatementBuilderEntity.Id)),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMergeAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForMergeAllWithSizePerBatchEqualsToOne()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            MergeAllExecutionContextCache.Flush();
            CommandTextCache.Flush();
            connection.MergeAll<StatementBuilderEntity>(
                new[]
                {
                    new StatementBuilderEntity { Name = "Name1" }
                },
                new Field(nameof(StatementBuilderEntity.Id)),
                batchSize: 1,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.MergeAll<StatementBuilderEntity>(
                new[]
                {
                    new StatementBuilderEntity { Name = "Name1" }
                },
                new Field(nameof(StatementBuilderEntity.Id)),
                batchSize: 1,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForMergeAllViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            MergeAllExecutionContextCache.Flush();
            CommandTextCache.Flush();
            connection.MergeAll(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new[]
                {
                    new { Name = "Name1" },
                    new { Name = "Name2" },
                    new { Name = "Name3" }
                },
                new Field(nameof(StatementBuilderEntityForTableName.Id)),
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMergeAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.MergeAll(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new[]
                {
                    new { Name = "Name1" },
                    new { Name = "Name2" },
                    new { Name = "Name3" }
                },
                new Field(nameof(StatementBuilderEntityForTableName.Id)),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMergeAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForMergeAllWithSizePerBatchEqualsToOneViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            MergeAllExecutionContextCache.Flush();
            CommandTextCache.Flush();
            connection.MergeAll(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new[]
                {
                    new { Name = "Name1" }
                },
                new Field(nameof(StatementBuilderEntityForTableName.Id)),
                batchSize: 1,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.MergeAll(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new[]
                {
                    new { Name = "Name1" }
                },
                new Field(nameof(StatementBuilderEntityForTableName.Id)),
                batchSize: 1,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForMergeAllViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            MergeAllExecutionContextCache.Flush();
            CommandTextCache.Flush();
            connection.MergeAll<StatementBuilderEntityForCrossCall>(
                new[]
                {
                    new StatementBuilderEntityForCrossCall { Name = "Name1" },
                    new StatementBuilderEntityForCrossCall { Name = "Name2" },
                    new StatementBuilderEntityForCrossCall { Name = "Name3" }
                },
                new Field(nameof(StatementBuilderEntityForCrossCall.Id)),
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMergeAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.MergeAll(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new[]
                {
                    new { Name = "Name1" },
                    new { Name = "Name2" },
                    new { Name = "Name3" }
                },
                new Field(nameof(StatementBuilderEntityForCrossCall.Id)),
                fields: FieldCache.Get<StatementBuilderEntityForCrossCall>(),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMergeAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForMergeAllWithSizePerBatchEqualsToOneViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            MergeAllExecutionContextCache.Flush();
            CommandTextCache.Flush();
            connection.MergeAll<StatementBuilderEntityForCrossCall>(
                new[]
                {
                    new StatementBuilderEntityForCrossCall { Name = "Name1" }
                },
                new Field(nameof(StatementBuilderEntityForCrossCall.Id)),
                batchSize: 1,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.MergeAll(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new[]
                {
                    new { Name = "Name1" }
                },
                new Field(nameof(StatementBuilderEntityForCrossCall.Id)),
                batchSize: 1,
                fields: FieldCache.Get<StatementBuilderEntityForCrossCall>(),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateMin

        [TestMethod]
        public void TestDbConnectionStatementBuilderForMin()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.Min<StatementBuilderEntity>(e => e.Id,
                (object)null,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMin(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.Min<StatementBuilderEntity>(e => e.Id,
                (object)null,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMin(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForMinViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.Min(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"),
                (object)null,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMin(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.Min(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"),
                (object)null,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMin(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForMinViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.Min<StatementBuilderEntityForCrossCall>(e => e.Id,
                (object)null,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMin(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.Min(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new Field("Id", typeof(int)),
                (object)null,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMin(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateMinAll

        [TestMethod]
        public void TestDbConnectionStatementBuilderForMinAll()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.MinAll<StatementBuilderEntity>(e => e.Id,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMinAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.MinAll<StatementBuilderEntity>(e => e.Id,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMinAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForMinAllViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.MinAll(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"),
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMinAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.MinAll(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMinAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForMinAllViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.MinAll<StatementBuilderEntityForCrossCall>(e => e.Id,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMinAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.MinAll(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new Field("Id", typeof(int)),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMinAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateQuery

        [TestMethod]
        public void TestDbConnectionStatementBuilderForQuery()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.Query<StatementBuilderEntity>(e => e.Id == 1, statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.Query<StatementBuilderEntity>(e => e.Id == 1, statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForQueryForTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.Query(ClassMappedNameCache.Get<StatementBuilderEntity>(),
                new { Id = 1 },
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.Query(ClassMappedNameCache.Get<StatementBuilderEntity>(),
                new { Id = 1 },
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForQueryViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.Query<StatementBuilderEntity>(e => e.Id == 1, statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.Query(ClassMappedNameCache.Get<StatementBuilderEntity>(),
                new { Id = 1 },
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateQueryAll

        [TestMethod]
        public void TestDbConnectionStatementBuilderForQueryAll()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.QueryAll<StatementBuilderEntity>(statementBuilder: statementBuilder.Object);

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
            connection.QueryAll<StatementBuilderEntity>(statementBuilder: statementBuilderNever.Object);

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
        public void TestDbConnectionStatementBuilderForQueryAllForTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.QueryAll(ClassMappedNameCache.Get<StatementBuilderEntity>(), statementBuilder: statementBuilder.Object);

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
            connection.QueryAll(ClassMappedNameCache.Get<StatementBuilderEntity>(), statementBuilder: statementBuilder.Object);

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
        public void TestDbConnectionStatementBuilderForQueryAllViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.QueryAll<StatementBuilderEntity>(statementBuilder: statementBuilder.Object);

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
            connection.QueryAll(ClassMappedNameCache.Get<StatementBuilderEntity>(), statementBuilder: statementBuilder.Object);

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
        public void TestDbConnectionStatementBuilderForQueryMultiple()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.QueryMultiple<StatementBuilderEntityT1,
                StatementBuilderEntityT2,
                StatementBuilderEntityT3,
                StatementBuilderEntityT4,
                StatementBuilderEntityT5,
                StatementBuilderEntityT6,
                StatementBuilderEntityT7>(e => e.Id == 1,
                e => e.Id == 1,
                e => e.Id == 1,
                e => e.Id == 1,
                e => e.Id == 1,
                e => e.Id == 1,
                e => e.Id == 1, statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT1>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT2>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT3>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT4>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT5>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT6>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT7>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.QueryMultiple<StatementBuilderEntityT1,
                StatementBuilderEntityT2,
                StatementBuilderEntityT3,
                StatementBuilderEntityT4,
                StatementBuilderEntityT5,
                StatementBuilderEntityT6,
                StatementBuilderEntityT7>(e => e.Id == 1,
                e => e.Id == 1,
                e => e.Id == 1,
                e => e.Id == 1,
                e => e.Id == 1,
                e => e.Id == 1,
                e => e.Id == 1, statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT1>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT2>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT3>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT4>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT5>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT6>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT7>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateSum

        [TestMethod]
        public void TestDbConnectionStatementBuilderForSum()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.Sum<StatementBuilderEntity>(e => e.Id,
                (object)null,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateSum(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.Sum<StatementBuilderEntity>(e => e.Id,
                (object)null,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateSum(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForSumViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.Sum(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"),
                (object)null,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateSum(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.Sum(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"),
                (object)null,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateSum(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForSumViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.Sum<StatementBuilderEntityForCrossCall>(e => e.Id,
                (object)null,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateSum(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.Sum(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new Field("Id", typeof(int)),
                (object)null,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateSum(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateSumAll

        [TestMethod]
        public void TestDbConnectionStatementBuilderForSumAll()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.SumAll<StatementBuilderEntity>(e => e.Id,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateSumAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.SumAll<StatementBuilderEntity>(e => e.Id,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateSumAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForSumAllViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.SumAll(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"),
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateSumAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.SumAll(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateSumAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForSumAllViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.SumAll<StatementBuilderEntityForCrossCall>(e => e.Id,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateSumAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.SumAll(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new Field("Id", typeof(int)),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateSumAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateTruncate

        [TestMethod]
        public void TestDbConnectionStatementBuilderForTruncate()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.Truncate<StatementBuilderEntity>(statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateTruncate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>())), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.Truncate<StatementBuilderEntity>(statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateTruncate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>())), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForTruncateViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.Truncate(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateTruncate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>())), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.Truncate(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateTruncate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>())), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForTruncateCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.Truncate<StatementBuilderEntityForCrossCall>(statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateTruncate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>())), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.Truncate(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateTruncate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>())), Times.Exactly(0));
        }

        #endregion

        #region CreateUpdate

        [TestMethod]
        public void TestDbConnectionStatementBuilderForUpdate()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            UpdateExecutionContextCache.Flush();
            CommandTextCache.Flush();
            connection.Update<StatementBuilderEntity>(new StatementBuilderEntity { Name = "Update" },
                e => e.Id == 1,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateUpdate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.Update<StatementBuilderEntity>(new StatementBuilderEntity { Name = "Update" },
                e => e.Id == 1,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateUpdate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForUpdateViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            UpdateExecutionContextCache.Flush();
            CommandTextCache.Flush();
            connection.Update(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new
                {
                    Name = "Update"
                },
                new
                {
                    Id = 1
                },
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateUpdate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.Update(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new
                {
                    Name = "Update"
                },
                new
                {
                    Id = 1
                },
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateUpdate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForUpdateViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            UpdateExecutionContextCache.Flush();
            CommandTextCache.Flush();
            connection.Update<StatementBuilderEntityForCrossCall>(new StatementBuilderEntityForCrossCall { Name = "Update" },
                e => e.Id == 1,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateUpdate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.Update(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new
                {
                    Id = 1,
                    Name = "Update"
                },
                new
                {
                    Id = 1
                },
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateUpdate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateUpdateAll

        [TestMethod]
        public void TestDbConnectionStatementBuilderForUpdateAll()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            UpdateAllExecutionContextCache.Flush();
            CommandTextCache.Flush();
            connection.UpdateAll<StatementBuilderEntity>(
                new[]
                {
                    new StatementBuilderEntity { Name = "Name1" },
                    new StatementBuilderEntity { Name = "Name2" },
                    new StatementBuilderEntity { Name = "Name3" }
                },
                new Field(nameof(StatementBuilderEntity.Id)),
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateUpdateAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.UpdateAll<StatementBuilderEntity>(
                new[]
                {
                    new StatementBuilderEntity { Name = "Name1" },
                    new StatementBuilderEntity { Name = "Name2" },
                    new StatementBuilderEntity { Name = "Name3" }
                },
                new Field(nameof(StatementBuilderEntity.Id)),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateUpdateAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForUpdateAllViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            UpdateAllExecutionContextCache.Flush();
            CommandTextCache.Flush();
            connection.UpdateAll(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new[]
                {
                    new { Name = "Name1" },
                    new { Name = "Name2" },
                    new { Name = "Name3" }
                },
                new Field(nameof(StatementBuilderEntityForTableName.Id)),
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateUpdateAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.UpdateAll(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new[]
                {
                    new { Name = "Name1" },
                    new { Name = "Name2" },
                    new { Name = "Name3" }
                },
                new Field(nameof(StatementBuilderEntityForTableName.Id)),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateUpdateAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForUpdateAllViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            UpdateAllExecutionContextCache.Flush();
            CommandTextCache.Flush();
            connection.UpdateAll<StatementBuilderEntityForCrossCall>(
                new[]
                {
                    new StatementBuilderEntityForCrossCall { Name = "Name1" },
                    new StatementBuilderEntityForCrossCall { Name = "Name2" },
                    new StatementBuilderEntityForCrossCall { Name = "Name3" }
                },
                new Field(nameof(StatementBuilderEntityForCrossCall.Id)),
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateUpdateAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.UpdateAll(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new[]
                {
                    new { Name = "Name1" },
                    new { Name = "Name2" },
                    new { Name = "Name3" }
                },
                new Field(nameof(StatementBuilderEntityForCrossCall.Id)),
                fields: FieldCache.Get<StatementBuilderEntityForCrossCall>(),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateUpdateAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #endregion

        #region Async

        #region CreateAverage

        [TestMethod]
        public void TestDbConnectionStatementBuilderForAverageAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.AverageAsync<StatementBuilderEntity>(e => e.Id,
                (object)null,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateAverage(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.AverageAsync<StatementBuilderEntity>(e => e.Id,
                (object)null,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateAverage(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForAverageAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.AverageAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"),
                (object)null,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateAverage(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.AverageAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"),
                (object)null,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateAverage(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForAverageAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.AverageAsync<StatementBuilderEntityForCrossCall>(e => e.Id,
                (object)null,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateAverage(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.AverageAsync(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new Field("Id", typeof(int)),
                (object)null,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateAverage(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateAverageAll

        [TestMethod]
        public void TestDbConnectionStatementBuilderForAverageAllAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.AverageAllAsync<StatementBuilderEntity>(e => e.Id,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateAverageAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.AverageAllAsync<StatementBuilderEntity>(e => e.Id,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateAverageAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForAverageAllAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.AverageAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"),
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateAverageAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.AverageAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateAverageAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForAverageAllAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.AverageAllAsync<StatementBuilderEntityForCrossCall>(e => e.Id,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateAverageAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.AverageAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new Field("Id", typeof(int)),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateAverageAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateBatchQueryAsync

        [TestMethod]
        public void TestDbConnectionStatementBuilderForBatchQueryAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.BatchQueryAsync<StatementBuilderEntity>(page: 0,
                rowsPerBatch: 10,
                orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                where: (QueryGroup)null,
                statementBuilder: statementBuilder.Object).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateBatchQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.BatchQueryAsync<StatementBuilderEntity>(page: 0,
                rowsPerBatch: 10,
                orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                where: (QueryGroup)null,
                statementBuilder: statementBuilderNever.Object).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateBatchQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
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
        public void TestDbConnectionStatementBuilderForCountAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.CountAsync<StatementBuilderEntity>((object)null,
                statementBuilder: statementBuilder.Object).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateCount(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.CountAsync<StatementBuilderEntity>((object)null,
                statementBuilder: statementBuilderNever.Object).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateCount(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForCountAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.CountAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                (object)null,
                statementBuilder: statementBuilder.Object).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateCount(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.CountAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                (object)null,
                statementBuilder: statementBuilderNever.Object).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateCount(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForCountAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.CountAsync<StatementBuilderEntityForCrossCall>((object)null,
                statementBuilder: statementBuilder.Object).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateCount(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.CountAsync(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                (object)null,
                statementBuilder: statementBuilderNever.Object).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateCount(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateCountAllAsync

        [TestMethod]
        public void TestDbConnectionStatementBuilderForCountAllAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.CountAllAsync<StatementBuilderEntity>(statementBuilder: statementBuilder.Object).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateCountAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.CountAllAsync<StatementBuilderEntity>(statementBuilder: statementBuilderNever.Object).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateCountAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForCountAllAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.CountAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                statementBuilder: statementBuilder.Object).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateCountAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.CountAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                statementBuilder: statementBuilderNever.Object).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateCountAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForCountAllAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.CountAllAsync<StatementBuilderEntityForCrossCall>(statementBuilder: statementBuilder.Object).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateCountAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.CountAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                statementBuilder: statementBuilderNever.Object).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateCountAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateDeleteAsync

        [TestMethod]
        public void TestDbConnectionStatementBuilderForDeleteAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.DeleteAsync<StatementBuilderEntity>(e => e.Id == 1,
                statementBuilder: statementBuilder.Object).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDelete(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.DeleteAsync<StatementBuilderEntity>(e => e.Id == 1,
                statementBuilder: statementBuilderNever.Object).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateDelete(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForDeleteAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.DeleteAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new
                {
                    Id = 1
                },
                statementBuilder: statementBuilder.Object).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDelete(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.DeleteAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new
                {
                    Id = 1
                },
                statementBuilder: statementBuilderNever.Object).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateDelete(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForDeleteAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.DeleteAsync<StatementBuilderEntityForCrossCall>(e => e.Id == 1,
                statementBuilder: statementBuilder.Object).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDelete(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.DeleteAsync(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new
                {
                    Id = 1
                },
                statementBuilder: statementBuilderNever.Object).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateDelete(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateDeleteAllAsync

        [TestMethod]
        public void TestDbConnectionStatementBuilderForDeleteAllAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.DeleteAllAsync<StatementBuilderEntity>(statementBuilder: statementBuilder.Object).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDeleteAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.DeleteAllAsync<StatementBuilderEntity>(statementBuilder: statementBuilderNever.Object).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateDeleteAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForDeleteAllAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.DeleteAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                statementBuilder: statementBuilder.Object).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDeleteAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.DeleteAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                statementBuilder: statementBuilderNever.Object).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateDeleteAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForDeleteAllAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.DeleteAllAsync<StatementBuilderEntityForCrossCall>(statementBuilder: statementBuilder.Object).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDeleteAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.DeleteAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                statementBuilder: statementBuilderNever.Object).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateDeleteAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateExistsAsync

        [TestMethod]
        public void TestDbConnectionStatementBuilderForExistsAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.ExistsAsync<StatementBuilderEntity>((object)null,
                statementBuilder: statementBuilder.Object).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateExists(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.ExistsAsync<StatementBuilderEntity>((object)null,
                statementBuilder: statementBuilderNever.Object).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateExists(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForExistsAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.ExistsAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                (object)null,
                statementBuilder: statementBuilder.Object).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateExists(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.ExistsAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                (object)null,
                statementBuilder: statementBuilderNever.Object).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateExists(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForExistsAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.ExistsAsync<StatementBuilderEntityForCrossCall>((object)null,
                statementBuilder: statementBuilder.Object).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateExists(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.ExistsAsync(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                (object)null,
                statementBuilder: statementBuilderNever.Object).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateExists(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateInsertAsync

        [TestMethod]
        public void TestDbConnectionStatementBuilderForInsertAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            InsertExecutionContextCache.Flush();
            CommandTextCache.Flush();
            connection.InsertAsync<StatementBuilderEntity>(
                new StatementBuilderEntity
                {
                    Name = "Name"
                },
                statementBuilder: statementBuilder.Object).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.InsertAsync<StatementBuilderEntity>(
                new StatementBuilderEntity
                {
                    Name = "Name"
                },
                statementBuilder: statementBuilderNever.Object).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForInsertAsyncForTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            InsertExecutionContextCache.Flush();
            CommandTextCache.Flush();
            connection.InsertAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new
                {
                    Name = "Name"
                },
                statementBuilder: statementBuilder.Object).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.InsertAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new
                {
                    Name = "Name"
                },
                statementBuilder: statementBuilderNever.Object).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForInsertAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            InsertExecutionContextCache.Flush();
            CommandTextCache.Flush();
            connection.InsertAsync<StatementBuilderEntityForCrossCall>(
                new StatementBuilderEntityForCrossCall
                {
                    Name = "Name"
                },
                statementBuilder: statementBuilder.Object).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.InsertAsync(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new
                {
                    Id = 1,
                    Name = "Name"
                },
                statementBuilder: statementBuilderNever.Object).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateInsertAllAsync

        [TestMethod]
        public void TestDbConnectionStatementBuilderForInsertAllAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            InsertAllExecutionContextCache.Flush();
            CommandTextCache.Flush();
            connection.InsertAllAsync<StatementBuilderEntity>(new[]
            {
                new StatementBuilderEntity{ Name = "Name1" },
                new StatementBuilderEntity{ Name = "Name2" },
                new StatementBuilderEntity{ Name = "Name3" }
            },
            statementBuilder: statementBuilder.Object).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsertAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.InsertAllAsync<StatementBuilderEntity>(new[]
            {
                new StatementBuilderEntity{ Name = "Name1" },
                new StatementBuilderEntity{ Name = "Name2" },
                new StatementBuilderEntity{ Name = "Name3" }
            },
            statementBuilder: statementBuilderNever.Object).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsertAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForInsertAllAsyncWithSizePerBatchEqualsToOne()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            InsertAllExecutionContextCache.Flush();
            CommandTextCache.Flush();
            connection.InsertAllAsync<StatementBuilderEntity>(new[]
            {
                new StatementBuilderEntity{ Name = "Name" }
            },
            batchSize: 1,
            statementBuilder: statementBuilder.Object).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.InsertAllAsync<StatementBuilderEntity>(new[]
            {
                new StatementBuilderEntity{ Name = "Name" }
            },
            batchSize: 1,
            statementBuilder: statementBuilderNever.Object).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForInsertAllAsyncForTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            InsertAllExecutionContextCache.Flush();
            CommandTextCache.Flush();
            connection.InsertAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new[]
                {
                    new { Name = "Name1" },
                    new { Name = "Name2" },
                    new { Name = "Name3" }
                },
                fields: new[] { new Field("Id", typeof(int)), new Field("Name", typeof(string)) },
                statementBuilder: statementBuilder.Object).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsertAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.InsertAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new[]
                {
                    new { Name = "Name1" },
                    new { Name = "Name2" },
                    new { Name = "Name3" }
                },
                fields: new[] { new Field("Id", typeof(int)), new Field("Name", typeof(string)) },
                statementBuilder: statementBuilderNever.Object).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsertAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForInsertAllAsyncWithSizePerBatchEqualsToOneForTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            InsertAllExecutionContextCache.Flush();
            CommandTextCache.Flush();
            connection.InsertAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new[]
                {
                    new { Name = "Name" }
                },
                fields: new[] { new Field("Id", typeof(int)), new Field("Name", typeof(string)) },
                batchSize: 1,
                statementBuilder: statementBuilder.Object).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.InsertAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new[]
                {
                    new { Name = "Name" }
                },
                fields: new[] { new Field("Id", typeof(int)), new Field("Name", typeof(string)) },
                batchSize: 1,
                statementBuilder: statementBuilderNever.Object).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForInsertAllAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            InsertAllExecutionContextCache.Flush();
            CommandTextCache.Flush();
            connection.InsertAllAsync<StatementBuilderEntityForCrossCall>(
                new[]
                {
                    new StatementBuilderEntityForCrossCall { Name = "Name1" },
                    new StatementBuilderEntityForCrossCall { Name = "Name2" },
                    new StatementBuilderEntityForCrossCall { Name = "Name3" }
                },
                statementBuilder: statementBuilder.Object).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsertAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.InsertAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new[]
                {
                    new { Name = "Name1" },
                    new { Name = "Name2" },
                    new { Name = "Name3" }
                },
                fields: new[] { new Field("Id", typeof(int)), new Field("Name", typeof(string)) },
                statementBuilder: statementBuilderNever.Object).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsertAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForInsertAllAsyncWithSizePerBatchEqualsToOneViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            InsertAllExecutionContextCache.Flush();
            CommandTextCache.Flush();
            connection.InsertAllAsync<StatementBuilderEntityForCrossCall>(
                new[]
                {
                    new StatementBuilderEntityForCrossCall { Name = "Name" }
                },
                batchSize: 1,
                statementBuilder: statementBuilder.Object).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.InsertAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new[]
                {
                    new { Name = "Name" }
                },
                fields: new[] { new Field("Id", typeof(int)), new Field("Name", typeof(string)) },
                batchSize: 1,
                statementBuilder: statementBuilderNever.Object).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateMaxAsync

        [TestMethod]
        public void TestDbConnectionStatementBuilderForMaxAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.MaxAsync<StatementBuilderEntity>(e => e.Id,
                (object)null,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMax(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.MaxAsync<StatementBuilderEntity>(e => e.Id,
                (object)null,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMax(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForMaxAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.MaxAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"),
                (object)null,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMax(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.MaxAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"),
                (object)null,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMax(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForMaxAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.MaxAsync<StatementBuilderEntityForCrossCall>(e => e.Id,
                (object)null,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMax(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.MaxAsync(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new Field("Id", typeof(int)),
                (object)null,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMax(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateMaxAllAsync

        [TestMethod]
        public void TestDbConnectionStatementBuilderForMaxAllAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.MaxAllAsync<StatementBuilderEntity>(e => e.Id,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMaxAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.MaxAllAsync<StatementBuilderEntity>(e => e.Id,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMaxAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForMaxAllAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.MaxAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"),
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMaxAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.MaxAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMaxAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForMaxAllAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.MaxAllAsync<StatementBuilderEntityForCrossCall>(e => e.Id,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMaxAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.MaxAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new Field("Id", typeof(int)),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMaxAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateMergeAsync

        [TestMethod]
        public void TestDbConnectionStatementBuilderForMergeAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            MergeExecutionContextCache.Flush();
            CommandTextCache.Flush();
            connection.MergeAsync<StatementBuilderEntity>(
                new StatementBuilderEntity
                {
                    Name = "Name"
                },
                new Field(nameof(StatementBuilderEntity.Id)),
                statementBuilder: statementBuilder.Object).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.MergeAsync<StatementBuilderEntity>(
                new StatementBuilderEntity
                {
                    Name = "Name"
                },
                new Field(nameof(StatementBuilderEntity.Id)),
                statementBuilder: statementBuilderNever.Object).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForMergeAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            MergeExecutionContextCache.Flush();
            CommandTextCache.Flush();
            connection.MergeAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new
                {
                    Name = "Name"
                },
                new Field(nameof(StatementBuilderEntityForTableName.Id)),
                statementBuilder: statementBuilder.Object).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.MergeAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new
                {
                    Name = "Name"
                },
                new Field(nameof(StatementBuilderEntityForTableName.Id)),
                statementBuilder: statementBuilderNever.Object).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForMergeAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            MergeExecutionContextCache.Flush();
            CommandTextCache.Flush();
            connection.MergeAsync<StatementBuilderEntityForCrossCall>(
                new StatementBuilderEntityForCrossCall
                {
                    Name = "Name"
                },
                new Field(nameof(StatementBuilderEntityForCrossCall.Id)),
                statementBuilder: statementBuilder.Object).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.MergeAsync(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new
                {
                    Id = 1,
                    Name = "Name"
                },
                new Field(nameof(StatementBuilderEntityForCrossCall.Id)),
                statementBuilder: statementBuilderNever.Object).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateMergeAllAsync

        [TestMethod]
        public void TestDbConnectionStatementBuilderForMergeAllAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            MergeAllExecutionContextCache.Flush();
            CommandTextCache.Flush();
            connection.MergeAllAsync<StatementBuilderEntity>(
                new[]
                {
                    new StatementBuilderEntity { Name = "Name1" },
                    new StatementBuilderEntity { Name = "Name2" },
                    new StatementBuilderEntity { Name = "Name3" }
                },
                new Field(nameof(StatementBuilderEntity.Id)),
                statementBuilder: statementBuilder.Object).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMergeAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            MergeAllExecutionContextCache.Flush();
            connection.MergeAllAsync<StatementBuilderEntity>(
                new[]
                {
                    new StatementBuilderEntity { Name = "Name1" },
                    new StatementBuilderEntity { Name = "Name2" },
                    new StatementBuilderEntity { Name = "Name3" }
                },
                new Field(nameof(StatementBuilderEntity.Id)),
                statementBuilder: statementBuilderNever.Object).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMergeAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForMergeAllAsyncWithSizePerBatchEqualsToOne()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            MergeAllExecutionContextCache.Flush();
            CommandTextCache.Flush();
            connection.MergeAllAsync<StatementBuilderEntity>(
                new[]
                {
                    new StatementBuilderEntity { Name = "Name1" }
                },
                new Field(nameof(StatementBuilderEntity.Id)),
                batchSize: 1,
                statementBuilder: statementBuilder.Object).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.MergeAllAsync<StatementBuilderEntity>(
                new[]
                {
                    new StatementBuilderEntity { Name = "Name1" }
                },
                new Field(nameof(StatementBuilderEntity.Id)),
                batchSize: 1,
                statementBuilder: statementBuilderNever.Object).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForMergeAllAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            MergeAllExecutionContextCache.Flush();
            CommandTextCache.Flush();
            connection.MergeAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new[]
                {
                    new { Name = "Name1" },
                    new { Name = "Name2" },
                    new { Name = "Name3" }
                },
                new Field(nameof(StatementBuilderEntityForTableName.Id)),
                statementBuilder: statementBuilder.Object).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMergeAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.MergeAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new[]
                {
                    new { Name = "Name1" },
                    new { Name = "Name2" },
                    new { Name = "Name3" }
                },
                new Field(nameof(StatementBuilderEntityForTableName.Id)),
                statementBuilder: statementBuilderNever.Object).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMergeAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForMergeAllAsyncWithSizePerBatchEqualsToOneViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            MergeAllExecutionContextCache.Flush();
            CommandTextCache.Flush();
            connection.MergeAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new[]
                {
                    new { Name = "Name1" }
                },
                new Field(nameof(StatementBuilderEntityForTableName.Id)),
                batchSize: 1,
                statementBuilder: statementBuilder.Object).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.MergeAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new[]
                {
                    new { Name = "Name1" }
                },
                new Field(nameof(StatementBuilderEntityForTableName.Id)),
                batchSize: 1,
                statementBuilder: statementBuilderNever.Object).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForMergeAllAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            MergeAllExecutionContextCache.Flush();
            CommandTextCache.Flush();
            connection.MergeAllAsync<StatementBuilderEntityForCrossCall>(
                new[]
                {
                    new StatementBuilderEntityForCrossCall { Name = "Name1" },
                    new StatementBuilderEntityForCrossCall { Name = "Name2" },
                    new StatementBuilderEntityForCrossCall { Name = "Name3" }
                },
                new Field(nameof(StatementBuilderEntityForCrossCall.Id)),
                statementBuilder: statementBuilder.Object).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMergeAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.MergeAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new[]
                {
                    new { Name = "Name1" },
                    new { Name = "Name2" },
                    new { Name = "Name3" }
                },
                new Field(nameof(StatementBuilderEntityForCrossCall.Id)),
                fields: FieldCache.Get<StatementBuilderEntityForCrossCall>(),
                statementBuilder: statementBuilderNever.Object).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMergeAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForMergeAllAsyncWithSizePerBatchEqualsToOneViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            MergeAllExecutionContextCache.Flush();
            CommandTextCache.Flush();
            connection.MergeAllAsync<StatementBuilderEntityForCrossCall>(
                new[]
                {
                    new StatementBuilderEntityForCrossCall { Name = "Name1" }
                },
                new Field(nameof(StatementBuilderEntityForCrossCall.Id)),
                batchSize: 1,
                statementBuilder: statementBuilder.Object).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.MergeAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new[]
                {
                    new { Name = "Name1" }
                },
                new Field(nameof(StatementBuilderEntityForCrossCall.Id)),
                batchSize: 1,
                fields: FieldCache.Get<StatementBuilderEntityForCrossCall>(),
                statementBuilder: statementBuilderNever.Object).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateMinAsync

        [TestMethod]
        public void TestDbConnectionStatementBuilderForMinAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.MinAsync<StatementBuilderEntity>(e => e.Id,
                (object)null,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMin(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.MinAsync<StatementBuilderEntity>(e => e.Id,
                (object)null,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMin(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForMinAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.MinAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"),
                (object)null,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMin(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.MinAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"),
                (object)null,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMin(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForMinAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.MinAsync<StatementBuilderEntityForCrossCall>(e => e.Id,
                (object)null,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMin(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.MinAsync(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new Field("Id", typeof(int)),
                (object)null,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMin(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateMinAllAsync

        [TestMethod]
        public void TestDbConnectionStatementBuilderForMinAllAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.MinAllAsync<StatementBuilderEntity>(e => e.Id,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMinAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.MinAllAsync<StatementBuilderEntity>(e => e.Id,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMinAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForMinAllAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.MinAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"),
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMinAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.MinAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMinAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForMinAllAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.MinAllAsync<StatementBuilderEntityForCrossCall>(e => e.Id,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMinAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.MinAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new Field("Id", typeof(int)),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMinAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateQueryAsync

        [TestMethod]
        public void TestDbConnectionStatementBuilderForQueryAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.QueryAsync<StatementBuilderEntity>(e => e.Id == 1, statementBuilder: statementBuilder.Object).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.QueryAsync<StatementBuilderEntity>(e => e.Id == 1, statementBuilder: statementBuilderNever.Object).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForQueryAsyncForTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.QueryAsync(ClassMappedNameCache.Get<StatementBuilderEntity>(),
                new { Id = 1 },
                statementBuilder: statementBuilder.Object).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.QueryAsync(ClassMappedNameCache.Get<StatementBuilderEntity>(),
                new { Id = 1 },
                statementBuilder: statementBuilder.Object).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForQueryAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.QueryAsync<StatementBuilderEntity>(e => e.Id == 1, statementBuilder: statementBuilder.Object).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.QueryAsync(ClassMappedNameCache.Get<StatementBuilderEntity>(),
                new { Id = 1 },
                statementBuilder: statementBuilder.Object).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateQueryAll

        [TestMethod]
        public void TestDbConnectionStatementBuilderForQueryAllAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.QueryAllAsync<StatementBuilderEntity>(statementBuilder: statementBuilder.Object).Wait();

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
            connection.QueryAllAsync<StatementBuilderEntity>(statementBuilder: statementBuilderNever.Object).Wait();

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
        public void TestDbConnectionStatementBuilderForQueryAllAsyncForTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.QueryAllAsync(ClassMappedNameCache.Get<StatementBuilderEntity>(), statementBuilder: statementBuilder.Object).Wait();

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
            connection.QueryAllAsync(ClassMappedNameCache.Get<StatementBuilderEntity>(), statementBuilder: statementBuilder.Object).Wait();

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
        public void TestDbConnectionStatementBuilderForQueryAllAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.QueryAllAsync<StatementBuilderEntity>(statementBuilder: statementBuilder.Object).Wait();

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
            connection.QueryAllAsync(ClassMappedNameCache.Get<StatementBuilderEntity>(), statementBuilder: statementBuilder.Object).Wait();

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
        public void TestDbConnectionStatementBuilderForQueryMultipleAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.QueryMultipleAsync<StatementBuilderEntityT1,
                StatementBuilderEntityT2,
                StatementBuilderEntityT3,
                StatementBuilderEntityT4,
                StatementBuilderEntityT5,
                StatementBuilderEntityT6,
                StatementBuilderEntityT7>(e => e.Id == 1,
                e => e.Id == 1,
                e => e.Id == 1,
                e => e.Id == 1,
                e => e.Id == 1,
                e => e.Id == 1,
                e => e.Id == 1, statementBuilder: statementBuilder.Object).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT1>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT2>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT3>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT4>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT5>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT6>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT7>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.QueryMultipleAsync<StatementBuilderEntityT1,
                StatementBuilderEntityT2,
                StatementBuilderEntityT3,
                StatementBuilderEntityT4,
                StatementBuilderEntityT5,
                StatementBuilderEntityT6,
                StatementBuilderEntityT7>(e => e.Id == 1,
                e => e.Id == 1,
                e => e.Id == 1,
                e => e.Id == 1,
                e => e.Id == 1,
                e => e.Id == 1,
                e => e.Id == 1, statementBuilder: statementBuilderNever.Object).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT1>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT2>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT3>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT4>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT5>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT6>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT7>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateSumAsync

        [TestMethod]
        public void TestDbConnectionStatementBuilderForSumAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.SumAsync<StatementBuilderEntity>(e => e.Id,
                (object)null,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateSum(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.SumAsync<StatementBuilderEntity>(e => e.Id,
                (object)null,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateSum(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForSumAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.SumAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"),
                (object)null,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateSum(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.SumAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"),
                (object)null,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateSum(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForSumAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.SumAsync<StatementBuilderEntityForCrossCall>(e => e.Id,
                (object)null,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateSum(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.SumAsync(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new Field("Id", typeof(int)),
                (object)null,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateSum(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateSumAllAsync

        [TestMethod]
        public void TestDbConnectionStatementBuilderForSumAllAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.SumAllAsync<StatementBuilderEntity>(e => e.Id,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateSumAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.SumAllAsync<StatementBuilderEntity>(e => e.Id,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateSumAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForSumAllAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.SumAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"),
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateSumAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.SumAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateSumAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForSumAllAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.SumAllAsync<StatementBuilderEntityForCrossCall>(e => e.Id,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateSumAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.SumAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new Field("Id", typeof(int)),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateSumAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateTruncateAsync

        [TestMethod]
        public void TestDbConnectionStatementBuilderForTruncateAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.TruncateAsync<StatementBuilderEntity>(statementBuilder: statementBuilder.Object).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateTruncate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>())), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.TruncateAsync<StatementBuilderEntity>(statementBuilder: statementBuilderNever.Object).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateTruncate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>())), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForTruncateAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.TruncateAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                statementBuilder: statementBuilder.Object).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateTruncate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>())), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.TruncateAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                statementBuilder: statementBuilderNever.Object).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateTruncate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>())), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForTruncateAsyncCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            connection.TruncateAsync<StatementBuilderEntityForCrossCall>(statementBuilder: statementBuilder.Object).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateTruncate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>())), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.TruncateAsync(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                statementBuilder: statementBuilderNever.Object).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateTruncate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>())), Times.Exactly(0));
        }

        #endregion

        #region CreateUpdateAsync

        [TestMethod]
        public void TestDbConnectionStatementBuilderForUpdateAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            UpdateExecutionContextCache.Flush();
            CommandTextCache.Flush();
            connection.UpdateAsync<StatementBuilderEntity>(new StatementBuilderEntity { Name = "Update" },
                e => e.Id == 1,
                statementBuilder: statementBuilder.Object).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateUpdate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.UpdateAsync<StatementBuilderEntity>(new StatementBuilderEntity { Name = "Update" },
                e => e.Id == 1,
                statementBuilder: statementBuilderNever.Object).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateUpdate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForUpdateAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            UpdateExecutionContextCache.Flush();
            CommandTextCache.Flush();
            connection.UpdateAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new
                {
                    Name = "Update"
                },
                new
                {
                    Id = 1
                },
                statementBuilder: statementBuilder.Object).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateUpdate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.UpdateAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new
                {
                    Name = "Update"
                },
                new
                {
                    Id = 1
                },
                statementBuilder: statementBuilderNever.Object).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateUpdate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForUpdateAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            UpdateExecutionContextCache.Flush();
            CommandTextCache.Flush();
            connection.UpdateAsync<StatementBuilderEntityForCrossCall>(new StatementBuilderEntityForCrossCall { Name = "Update" },
                e => e.Id == 1,
                statementBuilder: statementBuilder.Object).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateUpdate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.UpdateAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new
                {
                    Id = 1,
                    Name = "Update"
                },
                new
                {
                    Id = 1
                },
                statementBuilder: statementBuilderNever.Object).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateUpdate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateUpdateAllAsync

        [TestMethod]
        public void TestDbConnectionStatementBuilderForUpdateAllAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            UpdateAllExecutionContextCache.Flush();
            CommandTextCache.Flush();
            connection.UpdateAllAsync<StatementBuilderEntity>(
                new[]
                {
                    new StatementBuilderEntity { Name = "Name1" },
                    new StatementBuilderEntity { Name = "Name2" },
                    new StatementBuilderEntity { Name = "Name3" }
                },
                new Field(nameof(StatementBuilderEntity.Id)),
                statementBuilder: statementBuilder.Object).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateUpdateAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.UpdateAllAsync<StatementBuilderEntity>(
                new[]
                {
                    new StatementBuilderEntity { Name = "Name1" },
                    new StatementBuilderEntity { Name = "Name2" },
                    new StatementBuilderEntity { Name = "Name3" }
                },
                new Field(nameof(StatementBuilderEntity.Id)),
                statementBuilder: statementBuilderNever.Object).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateUpdateAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForUpdateAllAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            UpdateAllExecutionContextCache.Flush();
            CommandTextCache.Flush();
            connection.UpdateAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new[]
                {
                    new { Name = "Name1" },
                    new { Name = "Name2" },
                    new { Name = "Name3" }
                },
                new Field(nameof(StatementBuilderEntityForTableName.Id)),
                statementBuilder: statementBuilder.Object).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateUpdateAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.UpdateAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new[]
                {
                    new { Name = "Name1" },
                    new { Name = "Name2" },
                    new { Name = "Name3" }
                },
                new Field(nameof(StatementBuilderEntityForTableName.Id)),
                statementBuilder: statementBuilderNever.Object).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateUpdateAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForUpdateAllAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            UpdateAllExecutionContextCache.Flush();
            CommandTextCache.Flush();
            connection.UpdateAllAsync<StatementBuilderEntityForCrossCall>(
                new[]
                {
                    new StatementBuilderEntityForCrossCall { Name = "Name1" },
                    new StatementBuilderEntityForCrossCall { Name = "Name2" },
                    new StatementBuilderEntityForCrossCall { Name = "Name3" }
                },
                new Field(nameof(StatementBuilderEntityForCrossCall.Id)),
                statementBuilder: statementBuilder.Object).Wait();

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateUpdateAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.UpdateAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new[]
                {
                    new { Name = "Name1" },
                    new { Name = "Name2" },
                    new { Name = "Name3" }
                },
                new Field(nameof(StatementBuilderEntityForCrossCall.Id)),
                fields: FieldCache.Get<StatementBuilderEntityForCrossCall>(),
                statementBuilder: statementBuilderNever.Object).Wait();

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateUpdateAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #endregion
    }
}
