using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoDb.Contexts.Cachers;
using RepoDb.Enumerations;
using RepoDb.Interfaces;
using RepoDb.UnitTests.CustomObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RepoDb.UnitTests.Interfaces
{
    [TestClass]
    public class IStatementBuilderForDbConnectionTest
    {
        [TestInitialize]
        public void Initialize()
        {
            DbSettingMapper.Add<StatementBuilderDbConnection>(new CustomDbSetting(), true);
            DbHelperMapper.Add<StatementBuilderDbConnection>(new CustomDbHelper(), true);
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
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.CountAll<StatementBuilderEntity>(statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateCountAll(
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
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.DeleteAll<StatementBuilderEntity>(statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateDeleteAll(
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
                    new { Name = "Name1" },
                    new { Name = "Name2" },
                    new { Name = "Name3" }
                },
                fields: new[] { new Field("Id", typeof(int)), new Field("Name", typeof(string)) },
                batchSize: 1,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsert(
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
                    new { Name = "Name1" },
                    new { Name = "Name2" },
                    new { Name = "Name3" }
                },
                fields: new[] { new Field("Id", typeof(int)), new Field("Name", typeof(string)) },
                batchSize: 1,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsert(
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
                fields: FieldCache.Get<StatementBuilderEntityForCrossCall>(),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsertAll(
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
                    new { Id = 0, Name = "Name1" },
                    new { Id = 0, Name = "Name2" },
                    new { Id = 0, Name = "Name3" }
                },
                new Field(nameof(StatementBuilderEntityForCrossCall.Id)),
                fields: FieldCache.Get<StatementBuilderEntityForCrossCall>(),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMergeAll(
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
                    new StatementBuilderEntityForCrossCall { Name = "Name1" },
                    new StatementBuilderEntityForCrossCall { Name = "Name2" },
                    new StatementBuilderEntityForCrossCall { Name = "Name3" }
                },
                new Field(nameof(StatementBuilderEntityForCrossCall.Id)),
                batchSize: 1,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMerge(
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
                    new { Id = 0, Name = "Name1" },
                    new { Id = 0, Name = "Name2" },
                    new { Id = 0, Name = "Name3" }
                },
                new Field(nameof(StatementBuilderEntityForCrossCall.Id)),
                batchSize: 1,
                fields: FieldCache.Get<StatementBuilderEntityForCrossCall>(),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMerge(
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
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT1>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT2>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT3>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT4>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT5>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT6>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
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
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT1>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT2>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT3>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT4>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT5>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT6>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
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
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>())), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.Truncate<StatementBuilderEntity>(statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateTruncate(
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
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>())), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.Truncate(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateTruncate(
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
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>())), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.Truncate(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateTruncate(
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
                    new { Id = 0, Name = "Name1" },
                    new { Id = 0, Name = "Name2" },
                    new { Id = 0, Name = "Name3" }
                },
                new Field(nameof(StatementBuilderEntityForCrossCall.Id)),
                fields: FieldCache.Get<StatementBuilderEntityForCrossCall>(),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateUpdateAll(
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
        public async Task TestDbConnectionStatementBuilderForAverageAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            await connection.AverageAsync<StatementBuilderEntity>(e => e.Id,
                (object)null,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateAverage(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            await connection.AverageAsync<StatementBuilderEntity>(e => e.Id,
                (object)null,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateAverage(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public async Task TestDbConnectionStatementBuilderForAverageAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            await connection.AverageAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"),
                (object)null,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateAverage(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            await connection.AverageAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"),
                (object)null,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateAverage(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public async Task TestDbConnectionStatementBuilderForAverageAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            await connection.AverageAsync<StatementBuilderEntityForCrossCall>(e => e.Id,
                (object)null,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateAverage(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            await connection.AverageAsync(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new Field("Id", typeof(int)),
                (object)null,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateAverage(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateAverageAll

        [TestMethod]
        public async Task TestDbConnectionStatementBuilderForAverageAllAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            await connection.AverageAllAsync<StatementBuilderEntity>(e => e.Id,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateAverageAll(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            await connection.AverageAllAsync<StatementBuilderEntity>(e => e.Id,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateAverageAll(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public async Task TestDbConnectionStatementBuilderForAverageAllAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            await connection.AverageAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"),
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateAverageAll(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            await connection.AverageAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateAverageAll(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public async Task TestDbConnectionStatementBuilderForAverageAllAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            await connection.AverageAllAsync<StatementBuilderEntityForCrossCall>(e => e.Id,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateAverageAll(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            await connection.AverageAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new Field("Id", typeof(int)),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateAverageAll(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateBatchQueryAsync

        [TestMethod]
        public async Task TestDbConnectionStatementBuilderForBatchQueryAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            await connection.BatchQueryAsync<StatementBuilderEntity>(page: 0,
                rowsPerBatch: 10,
                orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                where: (QueryGroup)null,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateBatchQuery(
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
            await connection.BatchQueryAsync<StatementBuilderEntity>(page: 0,
                rowsPerBatch: 10,
                orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                where: (QueryGroup)null,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateBatchQuery(
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
        public async Task TestDbConnectionStatementBuilderForCountAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            await connection.CountAsync<StatementBuilderEntity>((object)null,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateCount(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            await connection.CountAsync<StatementBuilderEntity>((object)null,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateCount(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public async Task TestDbConnectionStatementBuilderForCountAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            await connection.CountAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                (object)null,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateCount(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            await connection.CountAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                (object)null,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateCount(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public async Task TestDbConnectionStatementBuilderForCountAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            await connection.CountAsync<StatementBuilderEntityForCrossCall>((object)null,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateCount(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            await connection.CountAsync(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                (object)null,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateCount(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateCountAllAsync

        [TestMethod]
        public async Task TestDbConnectionStatementBuilderForCountAllAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            await connection.CountAllAsync<StatementBuilderEntity>(statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateCountAll(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            await connection.CountAllAsync<StatementBuilderEntity>(statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateCountAll(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public async Task TestDbConnectionStatementBuilderForCountAllAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            await connection.CountAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateCountAll(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            await connection.CountAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateCountAll(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public async Task TestDbConnectionStatementBuilderForCountAllAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            await connection.CountAllAsync<StatementBuilderEntityForCrossCall>(statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateCountAll(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            await connection.CountAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateCountAll(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateDeleteAsync

        [TestMethod]
        public async Task TestDbConnectionStatementBuilderForDeleteAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            await connection.DeleteAsync<StatementBuilderEntity>(e => e.Id == 1,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDelete(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            await connection.DeleteAsync<StatementBuilderEntity>(e => e.Id == 1,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateDelete(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public async Task TestDbConnectionStatementBuilderForDeleteAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            await connection.DeleteAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new
                {
                    Id = 1
                },
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDelete(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            await connection.DeleteAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new
                {
                    Id = 1
                },
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateDelete(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public async Task TestDbConnectionStatementBuilderForDeleteAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            await connection.DeleteAsync<StatementBuilderEntityForCrossCall>(e => e.Id == 1,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDelete(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            await connection.DeleteAsync(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new
                {
                    Id = 1
                },
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateDelete(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateDeleteAllAsync

        [TestMethod]
        public async Task TestDbConnectionStatementBuilderForDeleteAllAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            await connection.DeleteAllAsync<StatementBuilderEntity>(statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDeleteAll(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            await connection.DeleteAllAsync<StatementBuilderEntity>(statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateDeleteAll(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public async Task TestDbConnectionStatementBuilderForDeleteAllAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            await connection.DeleteAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDeleteAll(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            await connection.DeleteAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateDeleteAll(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public async Task TestDbConnectionStatementBuilderForDeleteAllAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            await connection.DeleteAllAsync<StatementBuilderEntityForCrossCall>(statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDeleteAll(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
           await connection.DeleteAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateDeleteAll(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateExistsAsync

        [TestMethod]
        public async Task TestDbConnectionStatementBuilderForExistsAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            await connection.ExistsAsync<StatementBuilderEntity>((object)null,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateExists(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            await connection.ExistsAsync<StatementBuilderEntity>((object)null,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateExists(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public async Task TestDbConnectionStatementBuilderForExistsAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            await connection.ExistsAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                (object)null,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateExists(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            await connection.ExistsAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                (object)null,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateExists(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public async Task TestDbConnectionStatementBuilderForExistsAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            await connection.ExistsAsync<StatementBuilderEntityForCrossCall>((object)null,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateExists(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            await connection.ExistsAsync(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                (object)null,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateExists(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateInsertAsync

        [TestMethod]
        public async Task TestDbConnectionStatementBuilderForInsertAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            InsertExecutionContextCache.Flush();
            CommandTextCache.Flush();
            await connection.InsertAsync<StatementBuilderEntity>(
                new StatementBuilderEntity
                {
                    Name = "Name"
                },
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsert(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            await connection.InsertAsync<StatementBuilderEntity>(
                new StatementBuilderEntity
                {
                    Name = "Name"
                },
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsert(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public async Task TestDbConnectionStatementBuilderForInsertAsyncForTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            InsertExecutionContextCache.Flush();
            CommandTextCache.Flush();
            await connection.InsertAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new
                {
                    Name = "Name"
                },
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsert(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            await connection.InsertAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new
                {
                    Name = "Name"
                },
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsert(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public async Task TestDbConnectionStatementBuilderForInsertAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            InsertExecutionContextCache.Flush();
            CommandTextCache.Flush();
            await connection.InsertAsync<StatementBuilderEntityForCrossCall>(
                new StatementBuilderEntityForCrossCall
                {
                    Name = "Name"
                },
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsert(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            await connection.InsertAsync(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new
                {
                    Id = 1,
                    Name = "Name"
                },
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsert(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateInsertAllAsync

        [TestMethod]
        public async Task TestDbConnectionStatementBuilderForInsertAllAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            InsertAllExecutionContextCache.Flush();
            CommandTextCache.Flush();
            await connection.InsertAllAsync<StatementBuilderEntity>(new[]
            {
                new StatementBuilderEntity{ Name = "Name1" },
                new StatementBuilderEntity{ Name = "Name2" },
                new StatementBuilderEntity{ Name = "Name3" }
            },
            statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsertAll(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            await connection.InsertAllAsync<StatementBuilderEntity>(new[]
            {
                new StatementBuilderEntity{ Name = "Name1" },
                new StatementBuilderEntity{ Name = "Name2" },
                new StatementBuilderEntity{ Name = "Name3" }
            },
            statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsertAll(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public async Task TestDbConnectionStatementBuilderForInsertAllAsyncWithSizePerBatchEqualsToOne()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            InsertAllExecutionContextCache.Flush();
            CommandTextCache.Flush();
            await connection.InsertAllAsync<StatementBuilderEntity>(new[]
            {
                new StatementBuilderEntity{ Name = "Name" }
            },
            batchSize: 1,
            statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsert(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            await connection.InsertAllAsync<StatementBuilderEntity>(new[]
            {
                new StatementBuilderEntity{ Name = "Name" }
            },
            batchSize: 1,
            statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsert(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public async Task TestDbConnectionStatementBuilderForInsertAllAsyncForTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            InsertAllExecutionContextCache.Flush();
            CommandTextCache.Flush();
            await connection.InsertAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
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
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            await connection.InsertAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
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
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public async Task TestDbConnectionStatementBuilderForInsertAllAsyncWithSizePerBatchEqualsToOneForTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            InsertAllExecutionContextCache.Flush();
            CommandTextCache.Flush();
            await connection.InsertAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
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
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            await connection.InsertAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
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
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public async Task TestDbConnectionStatementBuilderForInsertAllAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            InsertAllExecutionContextCache.Flush();
            CommandTextCache.Flush();
            await connection.InsertAllAsync<StatementBuilderEntityForCrossCall>(
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
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            await connection.InsertAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
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
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public async Task TestDbConnectionStatementBuilderForInsertAllAsyncWithSizePerBatchEqualsToOneViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            InsertAllExecutionContextCache.Flush();
            CommandTextCache.Flush();
            await connection.InsertAllAsync<StatementBuilderEntityForCrossCall>(
                new[]
                {
                    new StatementBuilderEntityForCrossCall { Name = "Name" }
                },
                batchSize: 1,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsert(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            await connection.InsertAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
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
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateMaxAsync

        [TestMethod]
        public async Task TestDbConnectionStatementBuilderForMaxAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            await connection.MaxAsync<StatementBuilderEntity>(e => e.Id,
                (object)null,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMax(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            await connection.MaxAsync<StatementBuilderEntity>(e => e.Id,
                (object)null,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMax(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public async Task TestDbConnectionStatementBuilderForMaxAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            await connection.MaxAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"),
                (object)null,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMax(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            await connection.MaxAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"),
                (object)null,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMax(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public async Task TestDbConnectionStatementBuilderForMaxAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            await connection.MaxAsync<StatementBuilderEntityForCrossCall>(e => e.Id,
                (object)null,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMax(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            await connection.MaxAsync(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new Field("Id", typeof(int)),
                (object)null,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMax(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateMaxAllAsync

        [TestMethod]
        public async Task TestDbConnectionStatementBuilderForMaxAllAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            await connection.MaxAllAsync<StatementBuilderEntity>(e => e.Id,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMaxAll(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            await connection.MaxAllAsync<StatementBuilderEntity>(e => e.Id,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMaxAll(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public async Task TestDbConnectionStatementBuilderForMaxAllAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            await connection.MaxAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"),
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMaxAll(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            await connection.MaxAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMaxAll(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public async Task TestDbConnectionStatementBuilderForMaxAllAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            await connection.MaxAllAsync<StatementBuilderEntityForCrossCall>(e => e.Id,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMaxAll(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            await connection.MaxAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new Field("Id", typeof(int)),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMaxAll(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateMergeAsync

        [TestMethod]
        public async Task TestDbConnectionStatementBuilderForMergeAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            MergeExecutionContextCache.Flush();
            CommandTextCache.Flush();
            await connection.MergeAsync<StatementBuilderEntity>(
                new StatementBuilderEntity
                {
                    Name = "Name"
                },
                new Field(nameof(StatementBuilderEntity.Id)),
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMerge(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            await connection.MergeAsync<StatementBuilderEntity>(
                new StatementBuilderEntity
                {
                    Name = "Name"
                },
                new Field(nameof(StatementBuilderEntity.Id)),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMerge(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public async Task TestDbConnectionStatementBuilderForMergeAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            MergeExecutionContextCache.Flush();
            CommandTextCache.Flush();
            await connection.MergeAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new
                {
                    Name = "Name"
                },
                new Field(nameof(StatementBuilderEntityForTableName.Id)),
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMerge(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            await connection.MergeAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new
                {
                    Name = "Name"
                },
                new Field(nameof(StatementBuilderEntityForTableName.Id)),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMerge(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public async Task TestDbConnectionStatementBuilderForMergeAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            MergeExecutionContextCache.Flush();
            CommandTextCache.Flush();
            await connection.MergeAsync<StatementBuilderEntityForCrossCall>(
                new StatementBuilderEntityForCrossCall
                {
                    Name = "Name"
                },
                new Field(nameof(StatementBuilderEntityForCrossCall.Id)),
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMerge(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            await connection.MergeAsync(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
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
        public async Task TestDbConnectionStatementBuilderForMergeAllAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            MergeAllExecutionContextCache.Flush();
            CommandTextCache.Flush();
            await connection.MergeAllAsync<StatementBuilderEntity>(
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
            await connection.MergeAllAsync<StatementBuilderEntity>(
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
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public async Task TestDbConnectionStatementBuilderForMergeAllAsyncWithSizePerBatchEqualsToOne()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            MergeAllExecutionContextCache.Flush();
            CommandTextCache.Flush();
            await connection.MergeAllAsync<StatementBuilderEntity>(
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
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            await connection.MergeAllAsync<StatementBuilderEntity>(
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
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public async Task TestDbConnectionStatementBuilderForMergeAllAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            MergeAllExecutionContextCache.Flush();
            CommandTextCache.Flush();
            await connection.MergeAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
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
            await connection.MergeAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
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
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public async Task TestDbConnectionStatementBuilderForMergeAllAsyncWithSizePerBatchEqualsToOneViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            MergeAllExecutionContextCache.Flush();
            CommandTextCache.Flush();
            await connection.MergeAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
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
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            await connection.MergeAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
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
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public async Task TestDbConnectionStatementBuilderForMergeAllAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            MergeAllExecutionContextCache.Flush();
            CommandTextCache.Flush();
            await connection.MergeAllAsync<StatementBuilderEntityForCrossCall>(
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
            await connection.MergeAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new[]
                {
                    new { Id = 0, Name = "Name1" },
                    new { Id = 0, Name = "Name2" },
                    new { Id = 0, Name = "Name3" }
                },
                new Field(nameof(StatementBuilderEntityForCrossCall.Id)),
                fields: FieldCache.Get<StatementBuilderEntityForCrossCall>(),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMergeAll(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public async Task TestDbConnectionStatementBuilderForMergeAllAsyncWithSizePerBatchEqualsToOneViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            MergeAllExecutionContextCache.Flush();
            CommandTextCache.Flush();
            await connection.MergeAllAsync<StatementBuilderEntityForCrossCall>(
                new[]
                {
                    new StatementBuilderEntityForCrossCall { Name = "Name1" },
                    new StatementBuilderEntityForCrossCall { Name = "Name2" },
                    new StatementBuilderEntityForCrossCall { Name = "Name3" }
                },
                new Field(nameof(StatementBuilderEntityForCrossCall.Id)),
                batchSize: 1,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMerge(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            await connection.MergeAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new[]
                {
                    new { Id = 0, Name = "Name1" },
                    new { Id = 0, Name = "Name2" },
                    new { Id = 0, Name = "Name3" }
                },
                new Field(nameof(StatementBuilderEntityForCrossCall.Id)),
                batchSize: 1,
                fields: FieldCache.Get<StatementBuilderEntityForCrossCall>(),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMerge(
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
        public async Task TestDbConnectionStatementBuilderForMinAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            await connection.MinAsync<StatementBuilderEntity>(e => e.Id,
                (object)null,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMin(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            await connection.MinAsync<StatementBuilderEntity>(e => e.Id,
                (object)null,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMin(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public async Task TestDbConnectionStatementBuilderForMinAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            await connection.MinAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"),
                (object)null,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMin(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            await connection.MinAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"),
                (object)null,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMin(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public async Task TestDbConnectionStatementBuilderForMinAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            await connection.MinAsync<StatementBuilderEntityForCrossCall>(e => e.Id,
                (object)null,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMin(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            await connection.MinAsync(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new Field("Id", typeof(int)),
                (object)null,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMin(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateMinAllAsync

        [TestMethod]
        public async Task TestDbConnectionStatementBuilderForMinAllAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            await connection.MinAllAsync<StatementBuilderEntity>(e => e.Id,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMinAll(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            await connection.MinAllAsync<StatementBuilderEntity>(e => e.Id,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMinAll(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public async Task TestDbConnectionStatementBuilderForMinAllAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            await connection.MinAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"),
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMinAll(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            await connection.MinAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMinAll(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public async Task TestDbConnectionStatementBuilderForMinAllAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            await connection.MinAllAsync<StatementBuilderEntityForCrossCall>(e => e.Id,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMinAll(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            await connection.MinAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new Field("Id", typeof(int)),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMinAll(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateQueryAsync

        [TestMethod]
        public async Task TestDbConnectionStatementBuilderForQueryAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            await connection.QueryAsync<StatementBuilderEntity>(e => e.Id == 1, statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            await connection.QueryAsync<StatementBuilderEntity>(e => e.Id == 1, statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public async Task TestDbConnectionStatementBuilderForQueryAsyncForTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            await connection.QueryAsync(ClassMappedNameCache.Get<StatementBuilderEntity>(),
                new { Id = 1 },
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            await connection.QueryAsync(ClassMappedNameCache.Get<StatementBuilderEntity>(),
                new { Id = 1 },
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public async Task TestDbConnectionStatementBuilderForQueryAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            await connection.QueryAsync<StatementBuilderEntity>(e => e.Id == 1, statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            await connection.QueryAsync(ClassMappedNameCache.Get<StatementBuilderEntity>(),
                new { Id = 1 },
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
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
        public async Task TestDbConnectionStatementBuilderForQueryAllAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            await connection.QueryAllAsync<StatementBuilderEntity>(statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateQueryAll(
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            await connection.QueryAllAsync<StatementBuilderEntity>(statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateQueryAll(
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public async Task TestDbConnectionStatementBuilderForQueryAllAsyncForTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            await connection.QueryAllAsync(ClassMappedNameCache.Get<StatementBuilderEntity>(), statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateQueryAll(
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            await connection.QueryAllAsync(ClassMappedNameCache.Get<StatementBuilderEntity>(), statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateQueryAll(
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public async Task TestDbConnectionStatementBuilderForQueryAllAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            await connection.QueryAllAsync<StatementBuilderEntity>(statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateQueryAll(
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            await connection.QueryAllAsync(ClassMappedNameCache.Get<StatementBuilderEntity>(), statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateQueryAll(
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateQueryAsync(Multple)

        [TestMethod]
        public async Task TestDbConnectionStatementBuilderForQueryMultipleAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            await connection.QueryMultipleAsync<StatementBuilderEntityT1,
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
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT1>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT2>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT3>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT4>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT5>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT6>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT7>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            await connection.QueryMultipleAsync<StatementBuilderEntityT1,
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
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT1>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT2>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT3>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT4>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT5>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityT6>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
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
        public async Task TestDbConnectionStatementBuilderForSumAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            await connection.SumAsync<StatementBuilderEntity>(e => e.Id,
                (object)null,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateSum(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            await connection.SumAsync<StatementBuilderEntity>(e => e.Id,
                (object)null,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateSum(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public async Task TestDbConnectionStatementBuilderForSumAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            await connection.SumAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"),
                (object)null,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateSum(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            await connection.SumAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"),
                (object)null,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateSum(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public async Task TestDbConnectionStatementBuilderForSumAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            await connection.SumAsync<StatementBuilderEntityForCrossCall>(e => e.Id,
                (object)null,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateSum(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            await connection.SumAsync(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new Field("Id", typeof(int)),
                (object)null,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateSum(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateSumAllAsync

        [TestMethod]
        public async Task TestDbConnectionStatementBuilderForSumAllAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            await connection.SumAllAsync<StatementBuilderEntity>(e => e.Id,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateSumAll(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            await connection.SumAllAsync<StatementBuilderEntity>(e => e.Id,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateSumAll(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public async Task TestDbConnectionStatementBuilderForSumAllAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            await connection.SumAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"),
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateSumAll(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            await connection.SumAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                new Field("Id"),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateSumAll(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public async Task TestDbConnectionStatementBuilderForSumAllAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            await connection.SumAllAsync<StatementBuilderEntityForCrossCall>(e => e.Id,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateSumAll(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            await connection.SumAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new Field("Id", typeof(int)),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateSumAll(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<Field>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateTruncateAsync

        [TestMethod]
        public async Task TestDbConnectionStatementBuilderForTruncateAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            await connection.TruncateAsync<StatementBuilderEntity>(statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateTruncate(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>())), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            await connection.TruncateAsync<StatementBuilderEntity>(statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateTruncate(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>())), Times.Exactly(0));
        }

        [TestMethod]
        public async Task TestDbConnectionStatementBuilderForTruncateAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            await connection.TruncateAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateTruncate(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>())), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            await connection.TruncateAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateTruncate(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>())), Times.Exactly(0));
        }

        [TestMethod]
        public async Task TestDbConnectionStatementBuilderForTruncateAsyncCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            CommandTextCache.Flush();
            await connection.TruncateAsync<StatementBuilderEntityForCrossCall>(statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateTruncate(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>())), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            await connection.TruncateAsync(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateTruncate(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>())), Times.Exactly(0));
        }

        #endregion

        #region CreateUpdateAsync

        [TestMethod]
        public async Task TestDbConnectionStatementBuilderForUpdateAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            UpdateExecutionContextCache.Flush();
            CommandTextCache.Flush();
            await connection.UpdateAsync<StatementBuilderEntity>(new StatementBuilderEntity { Name = "Update" },
                e => e.Id == 1,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateUpdate(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            await connection.UpdateAsync<StatementBuilderEntity>(new StatementBuilderEntity { Name = "Update" },
                e => e.Id == 1,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateUpdate(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public async Task TestDbConnectionStatementBuilderForUpdateAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            UpdateExecutionContextCache.Flush();
            CommandTextCache.Flush();
            await connection.UpdateAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
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
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            await connection.UpdateAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
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
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public async Task TestDbConnectionStatementBuilderForUpdateAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            UpdateExecutionContextCache.Flush();
            CommandTextCache.Flush();
            await connection.UpdateAsync<StatementBuilderEntityForCrossCall>(new StatementBuilderEntityForCrossCall { Name = "Update" },
                e => e.Id == 1,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateUpdate(
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            await connection.UpdateAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
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
        public async Task TestDbConnectionStatementBuilderForUpdateAllAsync()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            UpdateAllExecutionContextCache.Flush();
            CommandTextCache.Flush();
            await connection.UpdateAllAsync<StatementBuilderEntity>(
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
            await connection.UpdateAllAsync<StatementBuilderEntity>(
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
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntity>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public async Task TestDbConnectionStatementBuilderForUpdateAllAsyncViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            UpdateAllExecutionContextCache.Flush();
            CommandTextCache.Flush();
            await connection.UpdateAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
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
            await connection.UpdateAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForTableName>(),
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
                    It.Is<string>(v => v == ClassMappedNameCache.Get<StatementBuilderEntityForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public async Task TestDbConnectionStatementBuilderForUpdateAllAsyncViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new StatementBuilderDbConnection();

            // Act
            UpdateAllExecutionContextCache.Flush();
            CommandTextCache.Flush();
            await  connection.UpdateAllAsync<StatementBuilderEntityForCrossCall>(
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
            await connection.UpdateAllAsync(ClassMappedNameCache.Get<StatementBuilderEntityForCrossCall>(),
                new[]
                {
                    new { Id = 0, Name = "Name1" },
                    new { Id = 0, Name = "Name2" },
                    new { Id = 0, Name = "Name3" }
                },
                new Field(nameof(StatementBuilderEntityForCrossCall.Id)),
                fields: FieldCache.Get<StatementBuilderEntityForCrossCall>(),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateUpdateAll(
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
