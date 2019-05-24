using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoDb.Attributes;
using RepoDb.Enumerations;
using RepoDb.Interfaces;
using RepoDb.UnitTests.CustomObjects;
using System.Collections.Generic;

namespace RepoDb.UnitTests.Interfaces
{
    [TestClass]
    public class IStatementBuilderForDbConnectionTest
    {
        #region SubClasses

        private class CustomDbConnectionForDbConnectionIStatementBuilder : CustomDbConnection { }

        private class DataEntityForDbConnectionStatementBuilder
        {
            [Primary, Identity]
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private class DataEntityForDbConnectionStatementBuilderForTableName
        {
            [Primary, Identity]
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private class DataEntityForDbConnectionStatementBuilderForCrossCall
        {
            [Primary, Identity]
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private class DataEntityForDbConnectionStatementBuilderT1
        {
            [Primary, Identity]
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private class DataEntityForDbConnectionStatementBuilderT2
        {
            [Primary, Identity]
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private class DataEntityForDbConnectionStatementBuilderT3
        {
            [Primary, Identity]
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private class DataEntityForDbConnectionStatementBuilderT4
        {
            [Primary, Identity]
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private class DataEntityForDbConnectionStatementBuilderT5
        {
            [Primary, Identity]
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private class DataEntityForDbConnectionStatementBuilderT6
        {
            [Primary, Identity]
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private class DataEntityForDbConnectionStatementBuilderT7
        {
            [Primary, Identity]
            public int Id { get; set; }
            public string Name { get; set; }
        }

        #endregion

        #region CreateBatchQuery

        [TestMethod]
        public void TestDbConnectionStatementBuilderForBatchQuery()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new CustomDbConnectionForDbConnectionIStatementBuilder();

            // Act
            CommandTextCache.Flush();
            connection.BatchQuery<DataEntityForDbConnectionStatementBuilder>(page: 0,
                rowsPerBatch: 10,
                orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                where: (QueryGroup)null,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateBatchQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilder>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.BatchQuery<DataEntityForDbConnectionStatementBuilder>(page: 0,
                rowsPerBatch: 10,
                orderBy: OrderField.Parse(new { Id = Order.Ascending }),
                where: (QueryGroup)null,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateBatchQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilder>()),
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
            var connection = new CustomDbConnectionForDbConnectionIStatementBuilder();

            // Act
            CommandTextCache.Flush();
            connection.Count<DataEntityForDbConnectionStatementBuilder>((object)null,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateCount(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilder>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.Count<DataEntityForDbConnectionStatementBuilder>((object)null,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateCount(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilder>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForCountViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new CustomDbConnectionForDbConnectionIStatementBuilder();

            // Act
            CommandTextCache.Flush();
            connection.Count(ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForTableName>(),
                (object)null,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateCount(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForTableName>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.Count(ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForTableName>(),
                (object)null,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateCount(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForTableName>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForCountViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new CustomDbConnectionForDbConnectionIStatementBuilder();

            // Act
            CommandTextCache.Flush();
            connection.Count<DataEntityForDbConnectionStatementBuilderForCrossCall>((object)null,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateCount(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForCrossCall>()),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.Count(ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForCrossCall>(),
                (object)null,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateCount(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForCrossCall>()),
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
            var connection = new CustomDbConnectionForDbConnectionIStatementBuilder();

            // Act
            CommandTextCache.Flush();
            connection.CountAll<DataEntityForDbConnectionStatementBuilder>(statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateCountAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilder>()),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.CountAll<DataEntityForDbConnectionStatementBuilder>(statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateCountAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilder>()),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForCountAllViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new CustomDbConnectionForDbConnectionIStatementBuilder();

            // Act
            CommandTextCache.Flush();
            connection.CountAll(ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForTableName>(),
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateCountAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForTableName>()),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.CountAll(ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForTableName>(),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateCountAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForTableName>()),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForCountAllViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new CustomDbConnectionForDbConnectionIStatementBuilder();

            // Act
            CommandTextCache.Flush();
            connection.CountAll<DataEntityForDbConnectionStatementBuilderForCrossCall>(statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateCountAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForCrossCall>()),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.CountAll(ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForCrossCall>(),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateCountAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForCrossCall>()),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateDelete

        [TestMethod]
        public void TestDbConnectionStatementBuilderForDelete()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new CustomDbConnectionForDbConnectionIStatementBuilder();

            // Act
            CommandTextCache.Flush();
            connection.Delete<DataEntityForDbConnectionStatementBuilder>(e => e.Id == 1,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDelete(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilder>()),
                    It.IsAny<QueryGroup>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.Delete<DataEntityForDbConnectionStatementBuilder>(e => e.Id == 1,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateDelete(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilder>()),
                    It.IsAny<QueryGroup>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForDeleteViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new CustomDbConnectionForDbConnectionIStatementBuilder();

            // Act
            CommandTextCache.Flush();
            connection.Delete(ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForTableName>(),
                new
                {
                    Id = 1
                },
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDelete(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForTableName>()),
                    It.IsAny<QueryGroup>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.Delete(ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForTableName>(),
                new
                {
                    Id = 1
                },
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateDelete(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForTableName>()),
                    It.IsAny<QueryGroup>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForDeleteViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new CustomDbConnectionForDbConnectionIStatementBuilder();

            // Act
            CommandTextCache.Flush();
            connection.Delete<DataEntityForDbConnectionStatementBuilderForCrossCall>(e => e.Id == 1,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDelete(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForCrossCall>()),
                    It.IsAny<QueryGroup>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.Delete(ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForCrossCall>(),
                new
                {
                    Id = 1
                },
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateDelete(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForCrossCall>()),
                    It.IsAny<QueryGroup>()), Times.Exactly(0));
        }

        #endregion

        #region CreateDeleteAll

        [TestMethod]
        public void TestDbConnectionStatementBuilderForDeleteAll()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new CustomDbConnectionForDbConnectionIStatementBuilder();

            // Act
            CommandTextCache.Flush();
            connection.DeleteAll<DataEntityForDbConnectionStatementBuilder>(statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDeleteAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilder>())), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.DeleteAll<DataEntityForDbConnectionStatementBuilder>(statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateDeleteAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilder>())), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForDeleteAllViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new CustomDbConnectionForDbConnectionIStatementBuilder();

            // Act
            CommandTextCache.Flush();
            connection.DeleteAll(ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForTableName>(),
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDeleteAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForTableName>())), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.DeleteAll(ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForTableName>(),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateDeleteAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForTableName>())), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForDeleteAllViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new CustomDbConnectionForDbConnectionIStatementBuilder();

            // Act
            CommandTextCache.Flush();
            connection.DeleteAll<DataEntityForDbConnectionStatementBuilderForCrossCall>(statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateDeleteAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForCrossCall>())), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.DeleteAll(ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForCrossCall>(),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateDeleteAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForCrossCall>())), Times.Exactly(0));
        }

        #endregion

        #region CreateInsert

        [TestMethod]
        public void TestDbConnectionStatementBuilderForInsert()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new CustomDbConnectionForDbConnectionIStatementBuilder();

            // Act
            CommandTextCache.Flush();
            connection.Insert<DataEntityForDbConnectionStatementBuilder>(
                new DataEntityForDbConnectionStatementBuilder
                {
                    Name = "Name"
                },
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilder>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.Insert<DataEntityForDbConnectionStatementBuilder>(
                new DataEntityForDbConnectionStatementBuilder
                {
                    Name = "Name"
                },
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilder>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForInsertForTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new CustomDbConnectionForDbConnectionIStatementBuilder();

            // Act
            CommandTextCache.Flush();
            connection.Insert(ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForTableName>(),
                new
                {
                    Name = "Name"
                },
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.Insert(ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForTableName>(),
                new
                {
                    Name = "Name"
                },
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForInsertViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new CustomDbConnectionForDbConnectionIStatementBuilder();

            // Act
            CommandTextCache.Flush();
            connection.Insert<DataEntityForDbConnectionStatementBuilderForCrossCall>(
                new DataEntityForDbConnectionStatementBuilderForCrossCall
                {
                    Name = "Name"
                },
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.Insert(ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForCrossCall>(),
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
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        #endregion

        #region CreateInsertAll

        [TestMethod]
        public void TestDbConnectionStatementBuilderForInsertAll()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new CustomDbConnectionForDbConnectionIStatementBuilder();

            // Act
            CommandTextCache.Flush();
            connection.InsertAll<DataEntityForDbConnectionStatementBuilder>(new[]
            {
                new DataEntityForDbConnectionStatementBuilder{ Name = "Name1" },
                new DataEntityForDbConnectionStatementBuilder{ Name = "Name2" },
                new DataEntityForDbConnectionStatementBuilder{ Name = "Name3" }
            },
            statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsertAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilder>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.InsertAll<DataEntityForDbConnectionStatementBuilder>(new[]
            {
                new DataEntityForDbConnectionStatementBuilder{ Name = "Name1" },
                new DataEntityForDbConnectionStatementBuilder{ Name = "Name2" },
                new DataEntityForDbConnectionStatementBuilder{ Name = "Name3" }
            },
            statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsertAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilder>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForInsertAllWithSizePerBatchEqualsToOne()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new CustomDbConnectionForDbConnectionIStatementBuilder();

            // Act
            CommandTextCache.Flush();
            connection.InsertAll<DataEntityForDbConnectionStatementBuilder>(new[]
            {
                new DataEntityForDbConnectionStatementBuilder{ Name = "Name" }
            },
            batchSize: 1,
            statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilder>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.InsertAll<DataEntityForDbConnectionStatementBuilder>(new[]
            {
                new DataEntityForDbConnectionStatementBuilder{ Name = "Name" }
            },
            batchSize: 1,
            statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilder>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForInsertAllForTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new CustomDbConnectionForDbConnectionIStatementBuilder();

            // Act
            CommandTextCache.Flush();
            connection.InsertAll(ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForTableName>(),
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
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.InsertAll(ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForTableName>(),
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
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForInsertAllWithSizePerBatchEqualsToOneForTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new CustomDbConnectionForDbConnectionIStatementBuilder();

            // Act
            CommandTextCache.Flush();
            connection.InsertAll(ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForTableName>(),
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
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.InsertAll(ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForTableName>(),
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
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForInsertAllViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new CustomDbConnectionForDbConnectionIStatementBuilder();

            // Act
            CommandTextCache.Flush();
            connection.InsertAll<DataEntityForDbConnectionStatementBuilderForCrossCall>(
                new[]
                {
                    new DataEntityForDbConnectionStatementBuilderForCrossCall { Name = "Name1" },
                    new DataEntityForDbConnectionStatementBuilderForCrossCall { Name = "Name2" },
                    new DataEntityForDbConnectionStatementBuilderForCrossCall { Name = "Name3" }
                },
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsertAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.InsertAll(ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForCrossCall>(),
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
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForInsertAllWithSizePerBatchEqualsToOneViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new CustomDbConnectionForDbConnectionIStatementBuilder();

            // Act
            CommandTextCache.Flush();
            connection.InsertAll<DataEntityForDbConnectionStatementBuilderForCrossCall>(
                new[]
                {
                    new DataEntityForDbConnectionStatementBuilderForCrossCall { Name = "Name" }
                },
                batchSize: 1,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateInsert(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.InsertAll(ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForCrossCall>(),
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
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        #endregion

        #region CreateMerge

        [TestMethod]
        public void TestDbConnectionStatementBuilderForMerge()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new CustomDbConnectionForDbConnectionIStatementBuilder();

            // Act
            CommandTextCache.Flush();
            connection.Merge<DataEntityForDbConnectionStatementBuilder>(
                new DataEntityForDbConnectionStatementBuilder
                {
                    Name = "Name"
                },
                new Field(nameof(DataEntityForDbConnectionStatementBuilder.Id)),
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilder>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.Merge<DataEntityForDbConnectionStatementBuilder>(
                new DataEntityForDbConnectionStatementBuilder
                {
                    Name = "Name"
                },
                new Field(nameof(DataEntityForDbConnectionStatementBuilder.Id)),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilder>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForMergeViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new CustomDbConnectionForDbConnectionIStatementBuilder();

            // Act
            CommandTextCache.Flush();
            connection.Merge(ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForTableName>(),
                new
                {
                    Name = "Name"
                },
                new Field(nameof(DataEntityForDbConnectionStatementBuilderForTableName.Id)),
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.Merge(ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForTableName>(),
                new
                {
                    Name = "Name"
                },
                new Field(nameof(DataEntityForDbConnectionStatementBuilderForTableName.Id)),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForMergeViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new CustomDbConnectionForDbConnectionIStatementBuilder();

            // Act
            CommandTextCache.Flush();
            connection.Merge<DataEntityForDbConnectionStatementBuilderForCrossCall>(
                new DataEntityForDbConnectionStatementBuilderForCrossCall
                {
                    Name = "Name"
                },
                new Field(nameof(DataEntityForDbConnectionStatementBuilderForCrossCall.Id)),
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.Merge(ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForCrossCall>(),
                new
                {
                    Id = 1,
                    Name = "Name"
                },
                new Field(nameof(DataEntityForDbConnectionStatementBuilderForCrossCall.Id)),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        #endregion

        #region CreateMergeAll

        [TestMethod]
        public void TestDbConnectionStatementBuilderForMergeAll()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new CustomDbConnectionForDbConnectionIStatementBuilder();

            // Act
            CommandTextCache.Flush();
            connection.MergeAll<DataEntityForDbConnectionStatementBuilder>(
                new[]
                {
                    new DataEntityForDbConnectionStatementBuilder { Name = "Name1" },
                    new DataEntityForDbConnectionStatementBuilder { Name = "Name2" },
                    new DataEntityForDbConnectionStatementBuilder { Name = "Name3" }
                },
                new Field(nameof(DataEntityForDbConnectionStatementBuilder.Id)),
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMergeAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilder>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.MergeAll<DataEntityForDbConnectionStatementBuilder>(
                new[]
                {
                    new DataEntityForDbConnectionStatementBuilder { Name = "Name1" },
                    new DataEntityForDbConnectionStatementBuilder { Name = "Name2" },
                    new DataEntityForDbConnectionStatementBuilder { Name = "Name3" }
                },
                new Field(nameof(DataEntityForDbConnectionStatementBuilder.Id)),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMergeAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilder>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForMergeAllWithSizePerBatchEqualsToOne()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new CustomDbConnectionForDbConnectionIStatementBuilder();

            // Act
            CommandTextCache.Flush();
            connection.MergeAll<DataEntityForDbConnectionStatementBuilder>(
                new[]
                {
                    new DataEntityForDbConnectionStatementBuilder { Name = "Name1" }
                },
                new Field(nameof(DataEntityForDbConnectionStatementBuilder.Id)),
                batchSize: 1,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilder>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.MergeAll<DataEntityForDbConnectionStatementBuilder>(
                new[]
                {
                    new DataEntityForDbConnectionStatementBuilder { Name = "Name1" }
                },
                new Field(nameof(DataEntityForDbConnectionStatementBuilder.Id)),
                batchSize: 1,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilder>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForMergeAllViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new CustomDbConnectionForDbConnectionIStatementBuilder();

            // Act
            CommandTextCache.Flush();
            connection.MergeAll(ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForTableName>(),
                new[]
                {
                    new { Name = "Name1" },
                    new { Name = "Name2" },
                    new { Name = "Name3" }
                },
                new Field(nameof(DataEntityForDbConnectionStatementBuilderForTableName.Id)),
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMergeAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.MergeAll(ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForTableName>(),
                new[]
                {
                    new { Name = "Name1" },
                    new { Name = "Name2" },
                    new { Name = "Name3" }
                },
                new Field(nameof(DataEntityForDbConnectionStatementBuilderForTableName.Id)),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMergeAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForMergeAllWithSizePerBatchEqualsToOneViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new CustomDbConnectionForDbConnectionIStatementBuilder();

            // Act
            CommandTextCache.Flush();
            connection.MergeAll(ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForTableName>(),
                new[]
                {
                    new { Name = "Name1" }
                },
                new Field(nameof(DataEntityForDbConnectionStatementBuilderForTableName.Id)),
                batchSize: 1,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.MergeAll(ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForTableName>(),
                new[]
                {
                    new { Name = "Name1" }
                },
                new Field(nameof(DataEntityForDbConnectionStatementBuilderForTableName.Id)),
                batchSize: 1,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForMergeAllViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new CustomDbConnectionForDbConnectionIStatementBuilder();

            // Act
            CommandTextCache.Flush();
            connection.MergeAll<DataEntityForDbConnectionStatementBuilderForCrossCall>(
                new[]
                {
                    new DataEntityForDbConnectionStatementBuilderForCrossCall { Name = "Name1" },
                    new DataEntityForDbConnectionStatementBuilderForCrossCall { Name = "Name2" },
                    new DataEntityForDbConnectionStatementBuilderForCrossCall { Name = "Name3" }
                },
                new Field(nameof(DataEntityForDbConnectionStatementBuilderForCrossCall.Id)),
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMergeAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.MergeAll(ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForCrossCall>(),
                new[]
                {
                    new { Name = "Name1" },
                    new { Name = "Name2" },
                    new { Name = "Name3" }
                },
                new Field(nameof(DataEntityForDbConnectionStatementBuilderForCrossCall.Id)),
                fields: FieldCache.Get<DataEntityForDbConnectionStatementBuilderForCrossCall>(),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMergeAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForMergeAllWithSizePerBatchEqualsToOneViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new CustomDbConnectionForDbConnectionIStatementBuilder();

            // Act
            CommandTextCache.Flush();
            connection.MergeAll<DataEntityForDbConnectionStatementBuilderForCrossCall>(
                new[]
                {
                    new DataEntityForDbConnectionStatementBuilderForCrossCall { Name = "Name1" }
                },
                new Field(nameof(DataEntityForDbConnectionStatementBuilderForCrossCall.Id)),
                batchSize: 1,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.MergeAll(ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForCrossCall>(),
                new[]
                {
                    new { Name = "Name1" }
                },
                new Field(nameof(DataEntityForDbConnectionStatementBuilderForCrossCall.Id)),
                batchSize: 1,
                fields: FieldCache.Get<DataEntityForDbConnectionStatementBuilderForCrossCall>(),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateMerge(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        #endregion

        #region CreateQuery

        [TestMethod]
        public void TestDbConnectionStatementBuilderForQuery()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new CustomDbConnectionForDbConnectionIStatementBuilder();

            // Act
            CommandTextCache.Flush();
            connection.Query<DataEntityForDbConnectionStatementBuilder>(e => e.Id == 1, statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilder>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.Query<DataEntityForDbConnectionStatementBuilder>(e => e.Id == 1, statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilder>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateQuery(Multple)

        [TestMethod]
        public void TestDbConnectionStatementBuilderForQueryMultiple()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new CustomDbConnectionForDbConnectionIStatementBuilder();

            // Act
            CommandTextCache.Flush();
            connection.QueryMultiple<DataEntityForDbConnectionStatementBuilderT1,
                DataEntityForDbConnectionStatementBuilderT2,
                DataEntityForDbConnectionStatementBuilderT3,
                DataEntityForDbConnectionStatementBuilderT4,
                DataEntityForDbConnectionStatementBuilderT5,
                DataEntityForDbConnectionStatementBuilderT6,
                DataEntityForDbConnectionStatementBuilderT7>(e => e.Id == 1,
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
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderT1>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderT2>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderT3>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderT4>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderT5>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderT6>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));
            statementBuilder.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderT7>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.QueryMultiple<DataEntityForDbConnectionStatementBuilderT1,
                DataEntityForDbConnectionStatementBuilderT2,
                DataEntityForDbConnectionStatementBuilderT3,
                DataEntityForDbConnectionStatementBuilderT4,
                DataEntityForDbConnectionStatementBuilderT5,
                DataEntityForDbConnectionStatementBuilderT6,
                DataEntityForDbConnectionStatementBuilderT7>(e => e.Id == 1,
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
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderT1>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderT2>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderT3>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderT4>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderT5>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderT6>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
            statementBuilderNever.Verify(builder =>
                builder.CreateQuery(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderT7>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<IEnumerable<OrderField>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>()), Times.Exactly(0));
        }

        #endregion

        #region CreateTruncate

        [TestMethod]
        public void TestDbConnectionStatementBuilderForTruncate()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new CustomDbConnectionForDbConnectionIStatementBuilder();

            // Act
            CommandTextCache.Flush();
            connection.Truncate<DataEntityForDbConnectionStatementBuilder>(statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateTruncate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilder>())), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.Truncate<DataEntityForDbConnectionStatementBuilder>(statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateTruncate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilder>())), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForTruncateViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new CustomDbConnectionForDbConnectionIStatementBuilder();

            // Act
            CommandTextCache.Flush();
            connection.Truncate(ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForTableName>(),
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateTruncate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForTableName>())), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.Truncate(ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForTableName>(),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateTruncate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForTableName>())), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new CustomDbConnectionForDbConnectionIStatementBuilder();

            // Act
            CommandTextCache.Flush();
            connection.Truncate<DataEntityForDbConnectionStatementBuilderForCrossCall>(statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateTruncate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForCrossCall>())), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.Truncate(ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForCrossCall>(),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateTruncate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForCrossCall>())), Times.Exactly(0));
        }

        #endregion

        #region CreateUpdate

        [TestMethod]
        public void TestDbConnectionStatementBuilderForUpdate()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new CustomDbConnectionForDbConnectionIStatementBuilder();

            // Act
            CommandTextCache.Flush();
            connection.Update<DataEntityForDbConnectionStatementBuilder>(new DataEntityForDbConnectionStatementBuilder { Name = "Update" },
                e => e.Id == 1,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateUpdate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilder>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.Update<DataEntityForDbConnectionStatementBuilder>(new DataEntityForDbConnectionStatementBuilder { Name = "Update" },
                e => e.Id == 1,
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateUpdate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilder>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForUpdateViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new CustomDbConnectionForDbConnectionIStatementBuilder();

            // Act
            CommandTextCache.Flush();
            connection.Update(ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForTableName>(),
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
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.Update(ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForTableName>(),
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
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForUpdateViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new CustomDbConnectionForDbConnectionIStatementBuilder();

            // Act
            CommandTextCache.Flush();
            connection.Update<DataEntityForDbConnectionStatementBuilderForCrossCall>(new DataEntityForDbConnectionStatementBuilderForCrossCall { Name = "Update" },
                e => e.Id == 1,
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateUpdate(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.Update(ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForTableName>(),
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
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<QueryGroup>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        #endregion

        #region CreateUpdateAll

        [TestMethod]
        public void TestDbConnectionStatementBuilderForUpdateAll()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new CustomDbConnectionForDbConnectionIStatementBuilder();

            // Act
            CommandTextCache.Flush();
            connection.UpdateAll<DataEntityForDbConnectionStatementBuilder>(
                new[]
                {
                    new DataEntityForDbConnectionStatementBuilder { Name = "Name1" },
                    new DataEntityForDbConnectionStatementBuilder { Name = "Name2" },
                    new DataEntityForDbConnectionStatementBuilder { Name = "Name3" }
                },
                new Field(nameof(DataEntityForDbConnectionStatementBuilder.Id)),
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateUpdateAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilder>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.UpdateAll<DataEntityForDbConnectionStatementBuilder>(
                new[]
                {
                    new DataEntityForDbConnectionStatementBuilder { Name = "Name1" },
                    new DataEntityForDbConnectionStatementBuilder { Name = "Name2" },
                    new DataEntityForDbConnectionStatementBuilder { Name = "Name3" }
                },
                new Field(nameof(DataEntityForDbConnectionStatementBuilder.Id)),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateUpdateAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilder>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForUpdateAllViaTableName()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new CustomDbConnectionForDbConnectionIStatementBuilder();

            // Act
            CommandTextCache.Flush();
            connection.UpdateAll(ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForTableName>(),
                new[]
                {
                    new { Name = "Name1" },
                    new { Name = "Name2" },
                    new { Name = "Name3" }
                },
                new Field(nameof(DataEntityForDbConnectionStatementBuilderForTableName.Id)),
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateUpdateAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.UpdateAll(ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForTableName>(),
                new[]
                {
                    new { Name = "Name1" },
                    new { Name = "Name2" },
                    new { Name = "Name3" }
                },
                new Field(nameof(DataEntityForDbConnectionStatementBuilderForTableName.Id)),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateUpdateAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForTableName>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        [TestMethod]
        public void TestDbConnectionStatementBuilderForUpdateAllViaCrossCall()
        {
            // Prepare
            var statementBuilder = new Mock<IStatementBuilder>();
            var connection = new CustomDbConnectionForDbConnectionIStatementBuilder();

            // Act
            CommandTextCache.Flush();
            connection.UpdateAll<DataEntityForDbConnectionStatementBuilderForCrossCall>(
                new[]
                {
                    new DataEntityForDbConnectionStatementBuilderForCrossCall { Name = "Name1" },
                    new DataEntityForDbConnectionStatementBuilderForCrossCall { Name = "Name2" },
                    new DataEntityForDbConnectionStatementBuilderForCrossCall { Name = "Name3" }
                },
                new Field(nameof(DataEntityForDbConnectionStatementBuilderForCrossCall.Id)),
                statementBuilder: statementBuilder.Object);

            // Assert
            statementBuilder.Verify(builder =>
                builder.CreateUpdateAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(1));

            // Prepare
            var statementBuilderNever = new Mock<IStatementBuilder>();

            // Act
            connection.UpdateAll(ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForCrossCall>(),
                new[]
                {
                    new { Name = "Name1" },
                    new { Name = "Name2" },
                    new { Name = "Name3" }
                },
                new Field(nameof(DataEntityForDbConnectionStatementBuilderForCrossCall.Id)),
                fields: FieldCache.Get<DataEntityForDbConnectionStatementBuilderForCrossCall>(),
                statementBuilder: statementBuilderNever.Object);

            // Assert
            statementBuilderNever.Verify(builder =>
                builder.CreateUpdateAll(
                    It.IsAny<QueryBuilder>(),
                    It.Is<string>(v => v == ClassMappedNameCache.Get<DataEntityForDbConnectionStatementBuilderForCrossCall>()),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<IEnumerable<Field>>(),
                    It.IsAny<int>(),
                    It.IsAny<DbField>(),
                    It.IsAny<DbField>()), Times.Exactly(0));
        }

        #endregion
    }
}
