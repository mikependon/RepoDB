using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using RepoDb.Enumerations;
using RepoDb.Exceptions;

namespace RepoDb.MariaDb.UnitTests
{
    [TestClass]
    public class StatementBuilderTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseMariaDb();
        }

        #region CreateBatchQuery

        [TestMethod]
        public void TestMariaDbStatementBuilderCreateBatchQuery()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            var query = builder.CreateBatchQuery("Table",
                Field.From("Id", "Name"),
                0,
                10,
                OrderField.Parse(new { Id = Order.Ascending }));
            var expected = "SELECT `Id`, `Name` FROM `Table` ORDER BY `Id` ASC LIMIT 10 ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestMariaDbStatementBuilderCreateBatchQueryWithPage()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            var query = builder.CreateBatchQuery("Table",
                Field.From("Id", "Name"),
                3,
                10,
                OrderField.Parse(new { Id = Order.Ascending }));
            var expected = "SELECT `Id`, `Name` FROM `Table` ORDER BY `Id` ASC LIMIT 30, 10 ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbStatementBuilderCreateBatchQueryIfThereAreNoFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            Assert.Throws<NullReferenceException>(() =>
                builder.CreateBatchQuery("Table",
                    null,
                    0,
                    10,
                    OrderField.Parse(new { Id = Order.Ascending })));
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbStatementBuilderCreateBatchQueryIfThereAreNoOrderFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            Assert.Throws<EmptyException>(() =>
                builder.CreateBatchQuery("Table",
                    Field.From("Id", "Name"),
                    0,
                    10,
                    null));
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbStatementBuilderCreateBatchQueryIfThePageValueIsNullOrOutOfRange()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                builder.CreateBatchQuery("Table",
                    Field.From("Id", "Name"),
                    -1,
                    10,
                    OrderField.Parse(new { Id = Order.Ascending })));
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbStatementBuilderCreateBatchQueryIfTheRowsPerBatchValueIsNullOrOutOfRange()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                builder.CreateBatchQuery("Table",
                    Field.From("Id", "Name"),
                    0,
                    -1,
                    OrderField.Parse(new { Id = Order.Ascending })));
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbStatementBuilderCreateBatchQueryIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateBatchQuery("Table",
                    Field.From("Id", "Name"),
                    0,
                    -1,
                    OrderField.Parse(new { Id = Order.Ascending }),
                    null,
                    "WhatEver"));
        }

        #endregion

        #region CreateCount

        [TestMethod]
        public void TestMariaDbStatementBuilderCreateCount()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            var query = builder.CreateCount("Table",
                null,
                null);
            var expected = "SELECT COUNT(*) AS `CountValue` FROM `Table` ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestMariaDbStatementBuilderCreateCountWithExpression()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            var query = builder.CreateCount("Table",
                QueryGroup.Parse(new { Id = 1 }),
                null);
            var expected = "SELECT COUNT(*) AS `CountValue` FROM `Table` WHERE (`Id` = @Id) ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbStatementBuilderCreateCountIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateCount("Table",
                    QueryGroup.Parse(new { Id = 1 }),
                    "WhatEver"));
        }

        #endregion

        #region CreateCountAll

        [TestMethod]
        public void TestMariaDbStatementBuilderCreateCountAll()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            var query = builder.CreateCountAll("Table",
                null);
            var expected = "SELECT COUNT(*) AS `CountValue` FROM `Table` ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbStatementBuilderCreateCountAllIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateCountAll("Table",
                    "WhatEver"));
        }

        #endregion

        #region CreateExists

        [TestMethod]
        public void TestMariaDbStatementBuilderCreateExists()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            var query = builder.CreateExists("Table",
                QueryGroup.Parse(new { Id = 1 }));
            var expected = "SELECT 1 AS `ExistsValue` FROM `Table` WHERE (`Id` = @Id) LIMIT 1 ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        #endregion

        #region CreateInsert

        [TestMethod]
        public void TestMariaDbStatementBuilderCreateInsert()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            var query = builder.CreateInsert("Table",
                Field.From("Id", "Name", "Address"),
                null,
                null);
            var expected = "INSERT INTO `Table` ( `Id`, `Name`, `Address` ) VALUES ( @Id, @Name, @Address ) ; SELECT NULL AS `Result` ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestMariaDbStatementBuilderCreateInsertWithPrimary()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            var query = builder.CreateInsert("Table",
                Field.From("Id", "Name", "Address"),
                new DbField("Id", true, false, false, typeof(int), null, null, null, null, false),
                null);
            var expected = "INSERT INTO `Table` ( `Id`, `Name`, `Address` ) VALUES ( @Id, @Name, @Address ) ; SELECT `Id` AS `Result` FROM `Table` WHERE (`Id` = @Id) ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestMariaDbStatementBuilderCreateInsertWithIdentity()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            var query = builder.CreateInsert("Table",
                Field.From("Id", "Name", "Address"),
                null,
                new DbField("Id", false, true, false, typeof(int), null, null, null, null, false));
            var expected = "INSERT INTO `Table` ( `Name`, `Address` ) VALUES ( @Name, @Address ) ; SELECT LAST_INSERT_ID() AS `Result` ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbStatementBuilderCreateInsertIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateInsert("Table",
                    Field.From("Id", "Name", "Address"),
                    null,
                    new DbField("Id", false, true, false, typeof(int), null, null, null, null, false),
                    "WhatEver"));
        }

        #endregion

        #region CreateInsertAll

        [TestMethod]
        public void TestMariaDbStatementBuilderCreateInsertAll()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            var query = builder.CreateInsertAll("Table",
                Field.From("Id", "Name", "Address"),
                3,
                null,
                null);
            var expected = "INSERT INTO `Table` ( `Id`, `Name`, `Address` ) VALUES ( @Id, @Name, @Address ) ," +
                " ( @Id_1, @Name_1, @Address_1 ) , ( @Id_2, @Name_2, @Address_2 ) ; SELECT NULL AS `Result`;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestMariaDbStatementBuilderCreateInsertAllWithPrimary()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            var query = builder.CreateInsertAll("Table",
                Field.From("Id", "Name", "Address"),
                3,
                new DbField("Id", true, false, false, typeof(int), null, null, null, null, false),
                null);
            var expected = "INSERT INTO `Table` ( `Id`, `Name`, `Address` ) VALUES ( @Id, @Name, @Address ) , ( @Id_1, @Name_1, @Address_1 ) ," +
                " ( @Id_2, @Name_2, @Address_2 ) ; SELECT `Id` AS `Result` FROM `Table` WHERE (`Id` = @Id) ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestMariaDbStatementBuilderCreateInsertAllWithIdentity()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            var query = builder.CreateInsertAll("Table",
                Field.From("Id", "Name", "Address"),
                3,
                null,
                new DbField("Id", false, true, false, typeof(int), null, null, null, null, false));
            var expected = "INSERT INTO `Table` ( `Name`, `Address` ) VALUES ( @Name, @Address ) , " +
                "( @Name_1, @Address_1 ) , ( @Name_2, @Address_2 ) ; VALUES ( LAST_INSERT_ID() + 0 ) " +
                ", ( LAST_INSERT_ID() + 1 ) , ( LAST_INSERT_ID() + 2 ) ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbStatementBuilderCreateInsertAllIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateInsertAll("Table",
                    Field.From("Id", "Name", "Address"),
                    3,
                    null,
                    new DbField("Id", false, true, false, typeof(int), null, null, null, null, false),
                    "WhatEver"));
        }

        #endregion

        #region CreateMax

        [TestMethod]
        public void TestMariaDbStatementBuilderCreateMax()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            var query = builder.CreateMax("Table",
                new Field("Field", typeof(int)),
                null,
                null);
            var expected = "SELECT MAX(`Field`) AS `MaxValue` FROM `Table` ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestMariaDbStatementBuilderCreateMaxWithExpression()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            var query = builder.CreateMax("Table",
                new Field("Field", typeof(int)),
                QueryGroup.Parse(new { Id = 1 }),
                null);
            var expected = "SELECT MAX(`Field`) AS `MaxValue` FROM `Table` WHERE (`Id` = @Id) ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbStatementBuilderCreateMaxIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateMax("Table",
                    new Field("Field", typeof(int)),
                    QueryGroup.Parse(new { Id = 1 }),
                    "WhatEver"));
        }

        #endregion

        #region CreateMaxAll

        [TestMethod]
        public void TestMariaDbStatementBuilderCreateMaxAll()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            var query = builder.CreateMaxAll("Table",
                new Field("Field", typeof(int)),
                null);
            var expected = "SELECT MAX(`Field`) AS `MaxValue` FROM `Table` ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbStatementBuilderCreateMaxAllIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateMaxAll("Table",
                    new Field("Field", typeof(int)),
                    "WhatEver"));
        }

        #endregion

        #region CreateMin

        [TestMethod]
        public void TestMariaDbStatementBuilderCreateMin()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            var query = builder.CreateMin("Table",
                new Field("Field", typeof(int)),
                null,
                null);
            var expected = "SELECT MIN(`Field`) AS `MinValue` FROM `Table` ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestMariaDbStatementBuilderCreateMinWithExpression()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            var query = builder.CreateMin("Table",
                new Field("Field", typeof(int)),
                QueryGroup.Parse(new { Id = 1 }),
                null);
            var expected = "SELECT MIN(`Field`) AS `MinValue` FROM `Table` WHERE (`Id` = @Id) ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbStatementBuilderCreateMinIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateMin("Table",
                    new Field("Field", typeof(int)),
                    QueryGroup.Parse(new { Id = 1 }),
                    "WhatEver"));
        }

        #endregion

        #region CreateMinAll

        [TestMethod]
        public void TestMariaDbStatementBuilderCreateMinAll()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            var query = builder.CreateMinAll("Table",
                new Field("Field", typeof(int)),
                null);
            var expected = "SELECT MIN(`Field`) AS `MinValue` FROM `Table` ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbStatementBuilderCreateMinAllIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateMinAll("Table",
                    new Field("Field", typeof(int)),
                    "WhatEver"));
        }

        #endregion

        #region CreateMerge

        [TestMethod]
        public void TestMariaDbStatementBuilderCreateMerge()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            var query = builder.CreateMerge("Table",
                Field.From("Id", "Name", "Address"),
                null,
                new DbField("Id", true, false, false, typeof(int), null, null, null, null, false),
                null);
            var expected = "INSERT INTO `Table` ( `Id`, `Name`, `Address` ) VALUES ( @Id, @Name, @Address ) ON DUPLICATE KEY " +
                "UPDATE `Id` = @Id, `Name` = @Name, `Address` = @Address ; SELECT CAST(COALESCE(@Id, LAST_INSERT_ID()) AS SIGNED) AS `Result` ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestMariaDbStatementBuilderCreateMergeWithPrimaryAsQualifier()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            var query = builder.CreateMerge("Table",
                Field.From("Id", "Name", "Address"),
                Field.From("Id"),
                new DbField("Id", true, false, false, typeof(int), null, null, null, null, false),
                null);
            var expected = "INSERT INTO `Table` ( `Id`, `Name`, `Address` ) VALUES ( @Id, @Name, @Address ) ON DUPLICATE KEY " +
                "UPDATE `Id` = @Id, `Name` = @Name, `Address` = @Address ; SELECT CAST(COALESCE(@Id, LAST_INSERT_ID()) AS SIGNED) AS `Result` ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestMariaDbStatementBuilderCreateMergeWithIdentity()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            var query = builder.CreateMerge("Table",
                Field.From("Id", "Name", "Address"),
                null,
                new DbField("Id", true, false, false, typeof(int), null, null, null, null, false),
                new DbField("Id", false, true, false, typeof(int), null, null, null, null, false));
            var expected = "INSERT INTO `Table` ( `Id`, `Name`, `Address` ) VALUES ( @Id, @Name, @Address ) ON DUPLICATE KEY " +
                "UPDATE `Id` = @Id, `Name` = @Name, `Address` = @Address ; SELECT CAST(COALESCE(@Id, LAST_INSERT_ID()) AS SIGNED) AS `Result` ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbStatementBuilderCreateMergeIfThereIsNoPrimary()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            Assert.Throws<PrimaryFieldNotFoundException>(() =>
                builder.CreateMerge("Table",
                    Field.From("Id", "Name", "Address"),
                    null,
                    null,
                    null));
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbStatementBuilderCreateMergeIfThereAreNoFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            Assert.Throws<PrimaryFieldNotFoundException>(() =>
                builder.CreateMerge("Table",
                    Field.From("Id", "Name", "Address"),
                    null,
                    null,
                    null));
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbStatementBuilderCreateMergeIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateMerge("Table",
                    Field.From("Id", "Name", "Address"),
                    Field.From("Id", "Name"),
                    new DbField("Id", true, false, false, typeof(int), null, null, null, null, false),
                    null,
                    "WhatEver"));
        }

        #endregion

        #region CreateMergeAll

        [TestMethod]
        public void TestMariaDbStatementBuilderCreateMergeAll()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            var query = builder.CreateMergeAll("Table",
                Field.From("Id", "Name", "Address"),
                null,
                3,
                new DbField("Id", true, false, false, typeof(int), null, null, null, null, false),
                null);
            var expected = "INSERT INTO `Table` ( `Id`, `Name`, `Address` ) VALUES ( @Id, @Name, @Address ) ON DUPLICATE KEY UPDATE `Id` = @Id, `Name` = @Name, `Address` = @Address ; SELECT CAST(COALESCE(@Id, LAST_INSERT_ID()) AS SIGNED) AS `Result`, @__RepoDb_OrderColumn_0 AS `OrderColumn` ; " +
                "INSERT INTO `Table` ( `Id`, `Name`, `Address` ) VALUES ( @Id_1, @Name_1, @Address_1 ) ON DUPLICATE KEY UPDATE `Id` = @Id_1, `Name` = @Name_1, `Address` = @Address_1 ; SELECT CAST(COALESCE(@Id_1, LAST_INSERT_ID()) AS SIGNED) AS `Result`, @__RepoDb_OrderColumn_1 AS `OrderColumn` ; " +
                "INSERT INTO `Table` ( `Id`, `Name`, `Address` ) VALUES ( @Id_2, @Name_2, @Address_2 ) ON DUPLICATE KEY UPDATE `Id` = @Id_2, `Name` = @Name_2, `Address` = @Address_2 ; SELECT CAST(COALESCE(@Id_2, LAST_INSERT_ID()) AS SIGNED) AS `Result`, @__RepoDb_OrderColumn_2 AS `OrderColumn` ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestMariaDbStatementBuilderCreateMergeAllWithPrimaryAsQualifier()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            var query = builder.CreateMergeAll("Table",
                Field.From("Id", "Name", "Address"),
                Field.From("Id"),
                3,
                new DbField("Id", true, false, false, typeof(int), null, null, null, null, false),
                null);
            var expected = "INSERT INTO `Table` ( `Id`, `Name`, `Address` ) VALUES ( @Id, @Name, @Address ) ON DUPLICATE KEY UPDATE `Id` = @Id, `Name` = @Name, `Address` = @Address ; SELECT CAST(COALESCE(@Id, LAST_INSERT_ID()) AS SIGNED) AS `Result`, @__RepoDb_OrderColumn_0 AS `OrderColumn` ; " +
                "INSERT INTO `Table` ( `Id`, `Name`, `Address` ) VALUES ( @Id_1, @Name_1, @Address_1 ) ON DUPLICATE KEY UPDATE `Id` = @Id_1, `Name` = @Name_1, `Address` = @Address_1 ; SELECT CAST(COALESCE(@Id_1, LAST_INSERT_ID()) AS SIGNED) AS `Result`, @__RepoDb_OrderColumn_1 AS `OrderColumn` ; " +
                "INSERT INTO `Table` ( `Id`, `Name`, `Address` ) VALUES ( @Id_2, @Name_2, @Address_2 ) ON DUPLICATE KEY UPDATE `Id` = @Id_2, `Name` = @Name_2, `Address` = @Address_2 ; SELECT CAST(COALESCE(@Id_2, LAST_INSERT_ID()) AS SIGNED) AS `Result`, @__RepoDb_OrderColumn_2 AS `OrderColumn` ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestMariaDbStatementBuilderCreateMergeAllWithIdentity()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            var query = builder.CreateMergeAll("Table",
                Field.From("Id", "Name", "Address"),
                null,
                3,
                new DbField("Id", true, false, false, typeof(int), null, null, null, null, false),
                new DbField("Id", false, true, false, typeof(int), null, null, null, null, false));
            var expected = "INSERT INTO `Table` ( `Id`, `Name`, `Address` ) VALUES ( @Id, @Name, @Address ) ON DUPLICATE KEY UPDATE `Id` = @Id, `Name` = @Name, `Address` = @Address ; SELECT CAST(COALESCE(@Id, LAST_INSERT_ID()) AS SIGNED) AS `Result`, @__RepoDb_OrderColumn_0 AS `OrderColumn` ; " +
                "INSERT INTO `Table` ( `Id`, `Name`, `Address` ) VALUES ( @Id_1, @Name_1, @Address_1 ) ON DUPLICATE KEY UPDATE `Id` = @Id_1, `Name` = @Name_1, `Address` = @Address_1 ; SELECT CAST(COALESCE(@Id_1, LAST_INSERT_ID()) AS SIGNED) AS `Result`, @__RepoDb_OrderColumn_1 AS `OrderColumn` ; " +
                "INSERT INTO `Table` ( `Id`, `Name`, `Address` ) VALUES ( @Id_2, @Name_2, @Address_2 ) ON DUPLICATE KEY UPDATE `Id` = @Id_2, `Name` = @Name_2, `Address` = @Address_2 ; SELECT CAST(COALESCE(@Id_2, LAST_INSERT_ID()) AS SIGNED) AS `Result`, @__RepoDb_OrderColumn_2 AS `OrderColumn` ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbStatementBuilderCreateMergeAllIfThereIsNoPrimary()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            Assert.Throws<PrimaryFieldNotFoundException>(() =>
                builder.CreateMergeAll("Table",
                    Field.From("Id", "Name", "Address"),
                    null,
                    3,
                    null,
                    null));
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbStatementBuilderCreateMergeAllIfThereAreNoFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            Assert.Throws<PrimaryFieldNotFoundException>(() =>
                builder.CreateMergeAll("Table",
                    Field.From("Id", "Name", "Address"),
                    null,
                    3,
                    null,
                    null));
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbStatementBuilderCreateMergeAllIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateMergeAll("Table",
                    Field.From("Id", "Name", "Address"),
                    Field.From("Id", "Name"),
                    3,
                    new DbField("Id", true, false, false, typeof(int), null, null, null, null, false),
                    null,
                    "WhatEver"));
        }

        #endregion

        #region CreateQuery

        [TestMethod]
        public void TestMariaDbStatementBuilderCreateQuery()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            var query = builder.CreateQuery("Table",
                Field.From("Id", "Name", "Address"),
                null,
                null,
                null,
                null);
            var expected = "SELECT `Id`, `Name`, `Address` FROM `Table` ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestMariaDbStatementBuilderCreateQueryWithExpression()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            var query = builder.CreateQuery("Table",
                Field.From("Id", "Name", "Address"),
                QueryGroup.Parse(new { Id = 1, Name = "Michael" }),
                null,
                null,
                null);
            var expected = "SELECT `Id`, `Name`, `Address` FROM `Table` WHERE (`Id` = @Id AND `Name` = @Name) ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestMariaDbStatementBuilderCreateQueryWithTop()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            var query = builder.CreateQuery("Table",
                Field.From("Id", "Name", "Address"),
                null,
                null,
                10,
                null);
            var expected = "SELECT `Id`, `Name`, `Address` FROM `Table` LIMIT 10 ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestMariaDbStatementBuilderCreateQueryOrderBy()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            var query = builder.CreateQuery("Table",
                Field.From("Id", "Name", "Address"),
                null,
                OrderField.Parse(new { Id = Order.Ascending }),
                null,
                null);
            var expected = "SELECT `Id`, `Name`, `Address` FROM `Table` ORDER BY `Id` ASC ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestMariaDbStatementBuilderCreateQueryOrderByFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            var query = builder.CreateQuery("Table",
                Field.From("Id", "Name", "Address"),
                null,
                OrderField.Parse(new { Id = Order.Ascending, Name = Order.Ascending }),
                null,
                null);
            var expected = "SELECT `Id`, `Name`, `Address` FROM `Table` ORDER BY `Id` ASC, `Name` ASC ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestMariaDbStatementBuilderCreateQueryOrderByDescending()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            var query = builder.CreateQuery("Table",
                Field.From("Id", "Name", "Address"),
                null,
                OrderField.Parse(new { Id = Order.Descending }),
                null,
                null);
            var expected = "SELECT `Id`, `Name`, `Address` FROM `Table` ORDER BY `Id` DESC ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestMariaDbStatementBuilderCreateQueryOrderByFieldsDescending()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            var query = builder.CreateQuery("Table",
                Field.From("Id", "Name", "Address"),
                null,
                OrderField.Parse(new { Id = Order.Descending, Name = Order.Descending }),
                null,
                null);
            var expected = "SELECT `Id`, `Name`, `Address` FROM `Table` ORDER BY `Id` DESC, `Name` DESC ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestMariaDbStatementBuilderCreateQueryOrderByFieldsMultiDirection()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            var query = builder.CreateQuery("Table",
                Field.From("Id", "Name", "Address"),
                null,
                OrderField.Parse(new { Id = Order.Ascending, Name = Order.Descending }),
                null,
                null);
            var expected = "SELECT `Id`, `Name`, `Address` FROM `Table` ORDER BY `Id` ASC, `Name` DESC ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbStatementBuilderCreateQueryIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateQuery("Table",
                    Field.From("Id", "Name", "Address"),
                    null,
                    null,
                    null,
                    "WhatEver"));
        }

        #endregion

        #region CreateSkipQuery

        [TestMethod]
        public void TestMariaDbStatementBuilderCreateSkipQuery()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            var query = builder.CreateSkipQuery("Table",
                Field.From("Id", "Name"),
                0,
                10,
                OrderField.Parse(new { Id = Order.Ascending }));
            var expected = "SELECT `Id`, `Name` FROM `Table` ORDER BY `Id` ASC LIMIT 10 ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestMariaDbStatementBuilderCreateSkipQueryWithSkip()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            var query = builder.CreateSkipQuery("Table",
                Field.From("Id", "Name"),
                30,
                10,
                OrderField.Parse(new { Id = Order.Ascending }));
            var expected = "SELECT `Id`, `Name` FROM `Table` ORDER BY `Id` ASC LIMIT 30, 10 ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbStatementBuilderCreateSkipQueryIfThereAreNoFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            Assert.Throws<NullReferenceException>(() =>
                builder.CreateSkipQuery("Table",
                    null,
                    0,
                    10,
                    OrderField.Parse(new { Id = Order.Ascending })));
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbStatementBuilderCreateSkipQueryIfThereAreNoOrderFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            Assert.Throws<EmptyException>(() =>
                builder.CreateSkipQuery("Table",
                    Field.From("Id", "Name"),
                    0,
                    10,
                    null));
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbStatementBuilderCreateSkipQueryIfTheSkipValueIsNullOrOutOfRange()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                builder.CreateSkipQuery("Table",
                    Field.From("Id", "Name"),
                    -1,
                    10,
                    OrderField.Parse(new { Id = Order.Ascending })));
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbStatementBuilderCreateSkipQueryIfTheTakeValueIsNullOrOutOfRange()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                builder.CreateSkipQuery("Table",
                    Field.From("Id", "Name"),
                    0,
                    -1,
                    OrderField.Parse(new { Id = Order.Ascending })));
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbStatementBuilderCreateSkipQueryIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateSkipQuery("Table",
                    Field.From("Id", "Name"),
                    0,
                    -1,
                    OrderField.Parse(new { Id = Order.Ascending }),
                    null,
                    "WhatEver"));
        }

        #endregion

        #region CreateSum

        [TestMethod]
        public void TestMariaDbStatementBuilderCreateSum()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            var query = builder.CreateSum("Table",
                new Field("Field", typeof(int)),
                null,
                null);
            var expected = "SELECT SUM(`Field`) AS `SumValue` FROM `Table` ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestMariaDbStatementBuilderCreateSumWithExpression()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            var query = builder.CreateSum("Table",
                new Field("Field", typeof(int)),
                QueryGroup.Parse(new { Id = 1 }),
                null);
            var expected = "SELECT SUM(`Field`) AS `SumValue` FROM `Table` WHERE (`Id` = @Id) ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbStatementBuilderCreateSumIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateSum("Table",
                    new Field("Field", typeof(int)),
                    QueryGroup.Parse(new { Id = 1 }),
                    "WhatEver"));
        }

        #endregion

        #region CreateSumAll

        [TestMethod]
        public void TestMariaDbStatementBuilderCreateSumAll()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            var query = builder.CreateSumAll("Table",
                new Field("Field", typeof(int)),
                null);
            var expected = "SELECT SUM(`Field`) AS `SumValue` FROM `Table` ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbStatementBuilderCreateSumAllIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateSumAll("Table",
                    new Field("Field", typeof(int)),
                    "WhatEver"));
        }

        #endregion
    }
}
