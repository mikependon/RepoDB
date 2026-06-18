using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySqlConnector;
using RepoDb.Enumerations;
using RepoDb.Exceptions;

namespace RepoDb.MySql.UnitTests
{
    [TestClass]
    public class StatementBuilderTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseMySqlConnector();
        }

        #region CreateBatchQuery

        [TestMethod]
        public void TestMySqlStatementBuilderCreateBatchQuery()
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
        public void TestMySqlStatementBuilderCreateBatchQueryWithPage()
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

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnMySqlStatementBuilderCreateBatchQueryIfThereAreNoFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            builder.CreateBatchQuery("Table",
                null,
                0,
                10,
                OrderField.Parse(new { Id = Order.Ascending }));
        }

        [TestMethod, ExpectedException(typeof(EmptyException))]
        public void ThrowExceptionOnMySqlStatementBuilderCreateBatchQueryIfThereAreNoOrderFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            builder.CreateBatchQuery("Table",
                Field.From("Id", "Name"),
                0,
                10,
                null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ThrowExceptionOnMySqlStatementBuilderCreateBatchQueryIfThePageValueIsNullOrOutOfRange()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            builder.CreateBatchQuery("Table",
                Field.From("Id", "Name"),
                -1,
                10,
                OrderField.Parse(new { Id = Order.Ascending }));
        }

        [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ThrowExceptionOnMySqlStatementBuilderCreateBatchQueryIfTheRowsPerBatchValueIsNullOrOutOfRange()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            builder.CreateBatchQuery("Table",
                Field.From("Id", "Name"),
                0,
                -1,
                OrderField.Parse(new { Id = Order.Ascending }));
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnMySqlStatementBuilderCreateBatchQueryIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            builder.CreateBatchQuery("Table",
                Field.From("Id", "Name"),
                0,
                -1,
                OrderField.Parse(new { Id = Order.Ascending }),
                null,
                "WhatEver");
        }

        #endregion

        #region CreateCount

        [TestMethod]
        public void TestMySqlStatementBuilderCreateCount()
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
        public void TestMySqlStatementBuilderCreateCountWithExpression()
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

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnMySqlStatementBuilderCreateCountIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            builder.CreateCount("Table",
                QueryGroup.Parse(new { Id = 1 }),
                "WhatEver");
        }

        #endregion

        #region CreateCountAll

        [TestMethod]
        public void TestMySqlStatementBuilderCreateCountAll()
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

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnMySqlStatementBuilderCreateCountAllIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            builder.CreateCountAll("Table",
                "WhatEver");
        }

        #endregion

        #region CreateExists

        [TestMethod]
        public void TestMySqlStatementBuilderCreateExists()
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
        public void TestMySqlStatementBuilderCreateInsert()
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
        public void TestMySqlStatementBuilderCreateInsertWithPrimary()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            var query = builder.CreateInsert("Table",
                Field.From("Id", "Name", "Address"),
                new DbField("Id", true, false, false, typeof(int), null, null, null, null, false),
                null);
            var expected = "INSERT INTO `Table` ( `Id`, `Name`, `Address` ) VALUES ( @Id, @Name, @Address ) ; SELECT @Id AS `Result` ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestMySqlStatementBuilderCreateInsertWithIdentity()
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

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnMySqlStatementBuilderCreateInsertIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            builder.CreateInsert("Table",
                Field.From("Id", "Name", "Address"),
                null,
                new DbField("Id", false, true, false, typeof(int), null, null, null, null, false),
                "WhatEver");
        }

        #endregion

        #region CreateInsertAll

        [TestMethod]
        public void TestMySqlStatementBuilderCreateInsertAll()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            var query = builder.CreateInsertAll("Table",
                Field.From("Id", "Name", "Address"),
                3,
                null,
                null);
            var expected = "INSERT INTO `Table` ( `Id`, `Name`, `Address` ) VALUES ROW ( @Id, @Name, @Address ) , " +
                "ROW ( @Id_1, @Name_1, @Address_1 ) , ROW ( @Id_2, @Name_2, @Address_2 ) ; SELECT NULL AS `Result`;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestMySqlStatementBuilderCreateInsertAllWithPrimary()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            var query = builder.CreateInsertAll("Table",
                Field.From("Id", "Name", "Address"),
                3,
                new DbField("Id", true, false, false, typeof(int), null, null, null, null, false),
                null);
            var expected = "INSERT INTO `Table` ( `Id`, `Name`, `Address` ) VALUES ROW ( @Id, @Name, @Address ) , ROW ( @Id_1, @Name_1, @Address_1 ) , " +
                "ROW ( @Id_2, @Name_2, @Address_2 ) ; SELECT @Id AS `Result`;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestMySqlStatementBuilderCreateInsertAllWithIdentity()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            var query = builder.CreateInsertAll("Table",
                Field.From("Id", "Name", "Address"),
                3,
                null,
                new DbField("Id", false, true, false, typeof(int), null, null, null, null, false));
            var expected = "INSERT INTO `Table` ( `Name`, `Address` ) VALUES ROW ( @Name, @Address ) , ROW ( @Name_1, @Address_1 ) , " +
                "ROW ( @Name_2, @Address_2 ) ; VALUES ROW ( LAST_INSERT_ID() + 0 ) , ROW ( LAST_INSERT_ID() + 1 ) , ROW ( LAST_INSERT_ID() + 2 ) ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnMySqlStatementBuilderCreateInsertAllIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            builder.CreateInsertAll("Table",
                Field.From("Id", "Name", "Address"),
                3,
                null,
                new DbField("Id", false, true, false, typeof(int), null, null, null, null, false),
                "WhatEver");
        }

        #endregion

        #region CreateMax

        [TestMethod]
        public void TestMySqlStatementBuilderCreateMax()
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
        public void TestMySqlStatementBuilderCreateMaxWithExpression()
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

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnMySqlStatementBuilderCreateMaxIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            builder.CreateMax("Table",
                new Field("Field", typeof(int)),
                QueryGroup.Parse(new { Id = 1 }),
                "WhatEver");
        }

        #endregion

        #region CreateMaxAll

        [TestMethod]
        public void TestMySqlStatementBuilderCreateMaxAll()
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

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnMySqlStatementBuilderCreateMaxAllIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            builder.CreateMaxAll("Table",
                new Field("Field", typeof(int)),
                "WhatEver");
        }

        #endregion

        #region CreateMin

        [TestMethod]
        public void TestMySqlStatementBuilderCreateMin()
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
        public void TestMySqlStatementBuilderCreateMinWithExpression()
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

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnMySqlStatementBuilderCreateMinIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            builder.CreateMin("Table",
                new Field("Field", typeof(int)),
                QueryGroup.Parse(new { Id = 1 }),
                "WhatEver");
        }

        #endregion

        #region CreateMinAll

        [TestMethod]
        public void TestMySqlStatementBuilderCreateMinAll()
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

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnMySqlStatementBuilderCreateMinAllIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            builder.CreateMinAll("Table",
                new Field("Field", typeof(int)),
                "WhatEver");
        }

        #endregion

        #region CreateMerge

        [TestMethod]
        public void TestMySqlStatementBuilderCreateMerge()
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
                "UPDATE `Id` = @Id, `Name` = @Name, `Address` = @Address ; SELECT COALESCE(@Id, LAST_INSERT_ID()) AS `Result` ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestMySqlStatementBuilderCreateMergeWithPrimaryAsQualifier()
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
                "UPDATE `Id` = @Id, `Name` = @Name, `Address` = @Address ; SELECT COALESCE(@Id, LAST_INSERT_ID()) AS `Result` ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestMySqlStatementBuilderCreateMergeWithIdentity()
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
                "UPDATE `Id` = @Id, `Name` = @Name, `Address` = @Address ; SELECT COALESCE(@Id, LAST_INSERT_ID()) AS `Result` ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod, ExpectedException(typeof(PrimaryFieldNotFoundException))]
        public void ThrowExceptionOnMySqlStatementBuilderCreateMergeIfThereIsNoPrimary()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            builder.CreateMerge("Table",
                Field.From("Id", "Name", "Address"),
                null,
                null,
                null);
        }

        [TestMethod, ExpectedException(typeof(PrimaryFieldNotFoundException))]
        public void ThrowExceptionOnMySqlStatementBuilderCreateMergeIfThereAreNoFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            builder.CreateMerge("Table",
                Field.From("Id", "Name", "Address"),
                null,
                null,
                null);
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnMySqlStatementBuilderCreateMergeIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            builder.CreateMerge("Table",
                Field.From("Id", "Name", "Address"),
                Field.From("Id", "Name"),
                new DbField("Id", true, false, false, typeof(int), null, null, null, null, false),
                null,
                "WhatEver");
        }

        #endregion

        #region CreateMergeAll

        [TestMethod]
        public void TestMySqlStatementBuilderCreateMergeAll()
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
            var expected = "INSERT INTO `Table` ( `Id`, `Name`, `Address` ) VALUES ( @Id, @Name, @Address ) ON DUPLICATE KEY UPDATE `Id` = @Id, `Name` = @Name, `Address` = @Address ; SELECT COALESCE(@Id, LAST_INSERT_ID()) AS `Result`, @__RepoDb_OrderColumn_0 AS `OrderColumn` ; " +
                "INSERT INTO `Table` ( `Id`, `Name`, `Address` ) VALUES ( @Id_1, @Name_1, @Address_1 ) ON DUPLICATE KEY UPDATE `Id` = @Id_1, `Name` = @Name_1, `Address` = @Address_1 ; SELECT COALESCE(@Id_1, LAST_INSERT_ID()) AS `Result`, @__RepoDb_OrderColumn_1 AS `OrderColumn` ; " +
                "INSERT INTO `Table` ( `Id`, `Name`, `Address` ) VALUES ( @Id_2, @Name_2, @Address_2 ) ON DUPLICATE KEY UPDATE `Id` = @Id_2, `Name` = @Name_2, `Address` = @Address_2 ; SELECT COALESCE(@Id_2, LAST_INSERT_ID()) AS `Result`, @__RepoDb_OrderColumn_2 AS `OrderColumn` ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestMySqlStatementBuilderCreateMergeAllWithPrimaryAsQualifier()
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
            var expected = "INSERT INTO `Table` ( `Id`, `Name`, `Address` ) VALUES ( @Id, @Name, @Address ) ON DUPLICATE KEY UPDATE `Id` = @Id, `Name` = @Name, `Address` = @Address ; SELECT COALESCE(@Id, LAST_INSERT_ID()) AS `Result`, @__RepoDb_OrderColumn_0 AS `OrderColumn` ; " +
                "INSERT INTO `Table` ( `Id`, `Name`, `Address` ) VALUES ( @Id_1, @Name_1, @Address_1 ) ON DUPLICATE KEY UPDATE `Id` = @Id_1, `Name` = @Name_1, `Address` = @Address_1 ; SELECT COALESCE(@Id_1, LAST_INSERT_ID()) AS `Result`, @__RepoDb_OrderColumn_1 AS `OrderColumn` ; " +
                "INSERT INTO `Table` ( `Id`, `Name`, `Address` ) VALUES ( @Id_2, @Name_2, @Address_2 ) ON DUPLICATE KEY UPDATE `Id` = @Id_2, `Name` = @Name_2, `Address` = @Address_2 ; SELECT COALESCE(@Id_2, LAST_INSERT_ID()) AS `Result`, @__RepoDb_OrderColumn_2 AS `OrderColumn` ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestMySqlStatementBuilderCreateMergeAllWithIdentity()
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
            var expected = "INSERT INTO `Table` ( `Id`, `Name`, `Address` ) VALUES ( @Id, @Name, @Address ) ON DUPLICATE KEY UPDATE `Id` = @Id, `Name` = @Name, `Address` = @Address ; SELECT COALESCE(@Id, LAST_INSERT_ID()) AS `Result`, @__RepoDb_OrderColumn_0 AS `OrderColumn` ; " +
                "INSERT INTO `Table` ( `Id`, `Name`, `Address` ) VALUES ( @Id_1, @Name_1, @Address_1 ) ON DUPLICATE KEY UPDATE `Id` = @Id_1, `Name` = @Name_1, `Address` = @Address_1 ; SELECT COALESCE(@Id_1, LAST_INSERT_ID()) AS `Result`, @__RepoDb_OrderColumn_1 AS `OrderColumn` ; " +
                "INSERT INTO `Table` ( `Id`, `Name`, `Address` ) VALUES ( @Id_2, @Name_2, @Address_2 ) ON DUPLICATE KEY UPDATE `Id` = @Id_2, `Name` = @Name_2, `Address` = @Address_2 ; SELECT COALESCE(@Id_2, LAST_INSERT_ID()) AS `Result`, @__RepoDb_OrderColumn_2 AS `OrderColumn` ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod, ExpectedException(typeof(PrimaryFieldNotFoundException))]
        public void ThrowExceptionOnMySqlStatementBuilderCreateMergeAllIfThereIsNoPrimary()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            builder.CreateMergeAll("Table",
                Field.From("Id", "Name", "Address"),
                null,
                3,
                null,
                null);
        }

        [TestMethod, ExpectedException(typeof(PrimaryFieldNotFoundException))]
        public void ThrowExceptionOnMySqlStatementBuilderCreateMergeAllIfThereAreNoFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            builder.CreateMergeAll("Table",
                Field.From("Id", "Name", "Address"),
                null,
                3,
                null,
                null);
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnMySqlStatementBuilderCreateMergeAllIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            builder.CreateMergeAll("Table",
                Field.From("Id", "Name", "Address"),
                Field.From("Id", "Name"),
                3,
                new DbField("Id", true, false, false, typeof(int), null, null, null, null, false),
                null,
                "WhatEver");
        }

        #endregion

        #region CreateQuery

        [TestMethod]
        public void TestMySqlStatementBuilderCreateQuery()
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
        public void TestMySqlStatementBuilderCreateQueryWithExpression()
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
        public void TestMySqlStatementBuilderCreateQueryWithTop()
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
        public void TestMySqlStatementBuilderCreateQueryOrderBy()
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
        public void TestMySqlStatementBuilderCreateQueryOrderByFields()
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
        public void TestMySqlStatementBuilderCreateQueryOrderByDescending()
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
        public void TestMySqlStatementBuilderCreateQueryOrderByFieldsDescending()
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
        public void TestMySqlStatementBuilderCreateQueryOrderByFieldsMultiDirection()
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

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnMySqlStatementBuilderCreateQueryIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            builder.CreateQuery("Table",
                Field.From("Id", "Name", "Address"),
                null,
                null,
                null,
                "WhatEver");
        }

        #endregion

        #region CreateSkipQuery

        [TestMethod]
        public void TestMySqlStatementBuilderCreateSkipQuery()
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
        public void TestMySqlStatementBuilderCreateSkipQueryWithSkip()
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

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnMySqlStatementBuilderCreateSkipQueryIfThereAreNoFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            builder.CreateSkipQuery("Table",
                null,
                0,
                10,
                OrderField.Parse(new { Id = Order.Ascending }));
        }

        [TestMethod, ExpectedException(typeof(EmptyException))]
        public void ThrowExceptionOnMySqlStatementBuilderCreateSkipQueryIfThereAreNoOrderFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            builder.CreateSkipQuery("Table",
                Field.From("Id", "Name"),
                0,
                10,
                null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ThrowExceptionOnMySqlStatementBuilderCreateSkipQueryIfTheSkipValueIsNullOrOutOfRange()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            builder.CreateSkipQuery("Table",
                Field.From("Id", "Name"),
                -1,
                10,
                OrderField.Parse(new { Id = Order.Ascending }));
        }

        [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ThrowExceptionOnMySqlStatementBuilderCreateSkipQueryIfTheTakeValueIsNullOrOutOfRange()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            builder.CreateSkipQuery("Table",
                Field.From("Id", "Name"),
                0,
                -1,
                OrderField.Parse(new { Id = Order.Ascending }));
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnMySqlStatementBuilderCreateSkipQueryIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            builder.CreateSkipQuery("Table",
                Field.From("Id", "Name"),
                0,
                -1,
                OrderField.Parse(new { Id = Order.Ascending }),
                null,
                "WhatEver");
        }

        #endregion

        #region CreateSum

        [TestMethod]
        public void TestMySqlStatementBuilderCreateSum()
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
        public void TestMySqlStatementBuilderCreateSumWithExpression()
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

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnMySqlStatementBuilderCreateSumIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            builder.CreateSum("Table",
                new Field("Field", typeof(int)),
                QueryGroup.Parse(new { Id = 1 }),
                "WhatEver");
        }

        #endregion

        #region CreateSumAll

        [TestMethod]
        public void TestMySqlStatementBuilderCreateSumAll()
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

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnMySqlStatementBuilderCreateSumAllIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MySqlConnection>();

            // Act
            builder.CreateSumAll("Table",
                new Field("Field", typeof(int)),
                "WhatEver");
        }

        #endregion
    }
}
