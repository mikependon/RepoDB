#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Connector.AuroraDb.MySqlConnector;
using RepoDb.Enumerations;
using RepoDb.Exceptions;

namespace RepoDb.AuroraDb.MySqlConnector.UnitTests
{
    [TestClass]
    public class StatementBuilderTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseAuroraDb();
        }

        #region CreateBatchQuery

        [TestMethod]
        public void TestAuroraDbStatementBuilderCreateBatchQuery()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<AuroraDbConnection>();

            // Act
            var query = builder.CreateBatchQuery("Table",
                Field.From("Id", "Name"),
                0,
                10,
                OrderField.Parse(new { Id = Order.Ascending }));
            var expected = "SELECT `Id`, `Name` FROM `Table` ORDER BY `Id` ASC LIMIT 10 ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbStatementBuilderCreateBatchQueryWithPage()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<AuroraDbConnection>();

            // Act
            var query = builder.CreateBatchQuery("Table",
                Field.From("Id", "Name"),
                3,
                10,
                OrderField.Parse(new { Id = Order.Ascending }));
            var expected = "SELECT `Id`, `Name` FROM `Table` ORDER BY `Id` ASC LIMIT 30, 10 ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnAuroraDbStatementBuilderCreateBatchQueryIfThereAreNoFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<AuroraDbConnection>();

            // Act
            Assert.Throws<ArgumentNullException>(() =>
                builder.CreateBatchQuery("Table",
                    null,
                    0,
                    10,
                    OrderField.Parse(new { Id = Order.Ascending })));
        }

        [TestMethod]
        public void ThrowExceptionOnAuroraDbStatementBuilderCreateBatchQueryIfThereAreNoOrderFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<AuroraDbConnection>();

            // Act
            Assert.Throws<EmptyException>(() =>
                builder.CreateBatchQuery("Table",
                    Field.From("Id", "Name"),
                    0,
                    10,
                    null));
        }

        [TestMethod]
        public void ThrowExceptionOnAuroraDbStatementBuilderCreateBatchQueryIfThePageValueIsNullOrOutOfRange()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<AuroraDbConnection>();

            // Act
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                builder.CreateBatchQuery("Table",
                    Field.From("Id", "Name"),
                    -1,
                    10,
                    OrderField.Parse(new { Id = Order.Ascending })));
        }

        [TestMethod]
        public void ThrowExceptionOnAuroraDbStatementBuilderCreateBatchQueryIfTheRowsPerBatchValueIsNullOrOutOfRange()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<AuroraDbConnection>();

            // Act
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                builder.CreateBatchQuery("Table",
                    Field.From("Id", "Name"),
                    0,
                    -1,
                    OrderField.Parse(new { Id = Order.Ascending })));
        }

        [TestMethod]
        public void ThrowExceptionOnAuroraDbStatementBuilderCreateBatchQueryIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<AuroraDbConnection>();

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
        public void TestAuroraDbStatementBuilderCreateCount()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<AuroraDbConnection>();

            // Act
            var query = builder.CreateCount("Table",
                null,
                null);
            var expected = "SELECT COUNT(*) AS `CountValue` FROM `Table` ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbStatementBuilderCreateCountWithExpression()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<AuroraDbConnection>();

            // Act
            var query = builder.CreateCount("Table",
                QueryGroup.Parse(new { Id = 1 }),
                null);
            var expected = "SELECT COUNT(*) AS `CountValue` FROM `Table` WHERE (`Id` = @Id) ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnAuroraDbStatementBuilderCreateCountIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<AuroraDbConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateCount("Table",
                    QueryGroup.Parse(new { Id = 1 }),
                    "WhatEver"));
        }

        #endregion

        #region CreateCountAll

        [TestMethod]
        public void TestAuroraDbStatementBuilderCreateCountAll()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<AuroraDbConnection>();

            // Act
            var query = builder.CreateCountAll("Table",
                null);
            var expected = "SELECT COUNT(*) AS `CountValue` FROM `Table` ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnAuroraDbStatementBuilderCreateCountAllIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<AuroraDbConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateCountAll("Table",
                    "WhatEver"));
        }

        #endregion

        #region CreateExists

        [TestMethod]
        public void TestAuroraDbStatementBuilderCreateExists()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<AuroraDbConnection>();

            // Act
            var query = builder.CreateExists("Table",
                QueryGroup.Parse(new { Id = 1 }));
            var expected = "SELECT 1 AS `ExistsValue` FROM `Table` WHERE (`Id` = @Id) LIMIT 1 ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        #endregion

        #region CreateInsert

        [TestMethod]
        public void TestAuroraDbStatementBuilderCreateInsert()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<AuroraDbConnection>();

            // Act
            var query = builder.CreateInsert("Table",
                Field.From("Id", "Name", "Address"),
                null,
                null);
            var expected = "INSERT INTO `Table` ( `Id`, `Name`, `Address` ) VALUES ( @Id, @Name, @Address ) ; SELECT NULL AS `Result` ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbStatementBuilderCreateInsertWithPrimary()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<AuroraDbConnection>();

            // Act
            var query = builder.CreateInsert("Table",
                Field.From("Id", "Name", "Address"),
                new DbField("Id", true, false, false, typeof(int), null, null, null, null, false),
                null);
            var expected = "INSERT INTO `Table` ( `Id`, `Name`, `Address` ) VALUES ( @Id, @Name, @Address ) ; SELECT `Id` AS `Result` FROM `Table` WHERE (`Id` = @Id) ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbStatementBuilderCreateInsertWithIdentity()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<AuroraDbConnection>();

            // Act
            var query = builder.CreateInsert("Table",
                Field.From("Id", "Name", "Address"),
                null,
                new DbField("Id", false, true, false, typeof(int), null, null, null, null, false));
            var expected = "INSERT INTO `Table` ( `Name`, `Address` ) VALUES ( @Name, @Address ) ; SELECT LAST_INSERT_ID() AS `Result` ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnAuroraDbStatementBuilderCreateInsertIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<AuroraDbConnection>();

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
        public void TestAuroraDbStatementBuilderCreateInsertAll()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<AuroraDbConnection>();

            // Act
            var query = builder.CreateInsertAll("Table",
                Field.From("Id", "Name", "Address"),
                3,
                null,
                null);
            var expected = "INSERT INTO `Table` ( `Id`, `Name`, `Address` ) VALUES ( @Id, @Name, @Address ) ," +
                " ( @Id_1, @Name_1, @Address_1 ) , ( @Id_2, @Name_2, @Address_2 ) ; SELECT NULL AS `Result`;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbStatementBuilderCreateInsertAllWithPrimary()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<AuroraDbConnection>();

            // Act
            var query = builder.CreateInsertAll("Table",
                Field.From("Id", "Name", "Address"),
                3,
                new DbField("Id", true, false, false, typeof(int), null, null, null, null, false),
                null);
            var expected = "INSERT INTO `Table` ( `Id`, `Name`, `Address` ) VALUES ( @Id, @Name, @Address ) , ( @Id_1, @Name_1, @Address_1 ) ," +
                " ( @Id_2, @Name_2, @Address_2 ) ; SELECT `Id` AS `Result` FROM `Table` WHERE (`Id` = @Id) ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbStatementBuilderCreateInsertAllWithIdentity()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<AuroraDbConnection>();

            // Act
            var query = builder.CreateInsertAll("Table",
                Field.From("Id", "Name", "Address"),
                3,
                null,
                new DbField("Id", false, true, false, typeof(int), null, null, null, null, false));
            var expected = "INSERT INTO `Table` ( `Name`, `Address` ) VALUES ( @Name, @Address ) , " +
                "( @Name_1, @Address_1 ) , ( @Name_2, @Address_2 ) ; VALUES ROW ( LAST_INSERT_ID() + 0 ) " +
                ", ROW ( LAST_INSERT_ID() + 1 ) , ROW ( LAST_INSERT_ID() + 2 ) ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnAuroraDbStatementBuilderCreateInsertAllIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<AuroraDbConnection>();

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
        public void TestAuroraDbStatementBuilderCreateMax()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<AuroraDbConnection>();

            // Act
            var query = builder.CreateMax("Table",
                new Field("Field", typeof(int)),
                null,
                null);
            var expected = "SELECT MAX(`Field`) AS `MaxValue` FROM `Table` ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbStatementBuilderCreateMaxWithExpression()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<AuroraDbConnection>();

            // Act
            var query = builder.CreateMax("Table",
                new Field("Field", typeof(int)),
                QueryGroup.Parse(new { Id = 1 }),
                null);
            var expected = "SELECT MAX(`Field`) AS `MaxValue` FROM `Table` WHERE (`Id` = @Id) ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnAuroraDbStatementBuilderCreateMaxIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<AuroraDbConnection>();

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
        public void TestAuroraDbStatementBuilderCreateMaxAll()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<AuroraDbConnection>();

            // Act
            var query = builder.CreateMaxAll("Table",
                new Field("Field", typeof(int)),
                null);
            var expected = "SELECT MAX(`Field`) AS `MaxValue` FROM `Table` ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnAuroraDbStatementBuilderCreateMaxAllIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<AuroraDbConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateMaxAll("Table",
                    new Field("Field", typeof(int)),
                    "WhatEver"));
        }

        #endregion

        #region CreateMin

        [TestMethod]
        public void TestAuroraDbStatementBuilderCreateMin()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<AuroraDbConnection>();

            // Act
            var query = builder.CreateMin("Table",
                new Field("Field", typeof(int)),
                null,
                null);
            var expected = "SELECT MIN(`Field`) AS `MinValue` FROM `Table` ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbStatementBuilderCreateMinWithExpression()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<AuroraDbConnection>();

            // Act
            var query = builder.CreateMin("Table",
                new Field("Field", typeof(int)),
                QueryGroup.Parse(new { Id = 1 }),
                null);
            var expected = "SELECT MIN(`Field`) AS `MinValue` FROM `Table` WHERE (`Id` = @Id) ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnAuroraDbStatementBuilderCreateMinIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<AuroraDbConnection>();

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
        public void TestAuroraDbStatementBuilderCreateMinAll()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<AuroraDbConnection>();

            // Act
            var query = builder.CreateMinAll("Table",
                new Field("Field", typeof(int)),
                null);
            var expected = "SELECT MIN(`Field`) AS `MinValue` FROM `Table` ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnAuroraDbStatementBuilderCreateMinAllIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<AuroraDbConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateMinAll("Table",
                    new Field("Field", typeof(int)),
                    "WhatEver"));
        }

        #endregion

        #region CreateMerge

        [TestMethod]
        public void TestAuroraDbStatementBuilderCreateMerge()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<AuroraDbConnection>();

            // Act
            var query = builder.CreateMerge("Table",
                Field.From("Id", "Name", "Address"),
                null,
                new DbField("Id", true, false, false, typeof(int), null, null, null, null, false),
                null);
            var expected = "INSERT INTO `Table` ( `Id`, `Name`, `Address` ) VALUES ( @Id, @Name, @Address ) ON DUPLICATE KEY " +
                "UPDATE `Id` = @Id, `Name` = @Name, `Address` = @Address ; SELECT CAST(@Id AS SIGNED) AS `Result` ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbStatementBuilderCreateMergeWithPrimaryAsQualifier()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<AuroraDbConnection>();

            // Act
            var query = builder.CreateMerge("Table",
                Field.From("Id", "Name", "Address"),
                Field.From("Id"),
                new DbField("Id", true, false, false, typeof(int), null, null, null, null, false),
                null);
            var expected = "INSERT INTO `Table` ( `Id`, `Name`, `Address` ) VALUES ( @Id, @Name, @Address ) ON DUPLICATE KEY " +
                "UPDATE `Id` = @Id, `Name` = @Name, `Address` = @Address ; SELECT CAST(@Id AS SIGNED) AS `Result` ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbStatementBuilderCreateMergeWithIdentity()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<AuroraDbConnection>();

            // Act
            var query = builder.CreateMerge("Table",
                Field.From("Id", "Name", "Address"),
                null,
                new DbField("Id", true, false, false, typeof(int), null, null, null, null, false),
                new DbField("Id", false, true, false, typeof(int), null, null, null, null, false));
            var expected = "INSERT INTO `Table` ( `Id`, `Name`, `Address` ) VALUES ( @Id, @Name, @Address ) ON DUPLICATE KEY " +
                "UPDATE `Id` = COALESCE(NULLIF(@Id, 0), LAST_INSERT_ID(`Id`)), `Name` = @Name, `Address` = @Address ; SELECT CAST(COALESCE(NULLIF(@Id, 0), LAST_INSERT_ID()) AS SIGNED) AS `Result` ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnAuroraDbStatementBuilderCreateMergeIfThereIsNoPrimary()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<AuroraDbConnection>();

            // Act
            Assert.Throws<PrimaryFieldNotFoundException>(() =>
                builder.CreateMerge("Table",
                    Field.From("Id", "Name", "Address"),
                    null,
                    null,
                    null));
        }

        [TestMethod]
        public void ThrowExceptionOnAuroraDbStatementBuilderCreateMergeIfThereAreNoFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<AuroraDbConnection>();

            // Act
            Assert.Throws<PrimaryFieldNotFoundException>(() =>
                builder.CreateMerge("Table",
                    Field.From("Id", "Name", "Address"),
                    null,
                    null,
                    null));
        }

        [TestMethod]
        public void ThrowExceptionOnAuroraDbStatementBuilderCreateMergeIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<AuroraDbConnection>();

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
        public void TestAuroraDbStatementBuilderCreateMergeAll()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<AuroraDbConnection>();

            // Act
            var query = builder.CreateMergeAll("Table",
                Field.From("Id", "Name", "Address"),
                null,
                3,
                new DbField("Id", true, false, false, typeof(int), null, null, null, null, false),
                null);
            var expected = "INSERT INTO `Table` ( `Id`, `Name`, `Address` ) VALUES ( @Id, @Name, @Address ) ON DUPLICATE KEY UPDATE `Id` = @Id, `Name` = @Name, `Address` = @Address ; SELECT CAST(@Id AS SIGNED) AS `Result`, @__RepoDb_OrderColumn_0 AS `OrderColumn` ; " +
                "INSERT INTO `Table` ( `Id`, `Name`, `Address` ) VALUES ( @Id_1, @Name_1, @Address_1 ) ON DUPLICATE KEY UPDATE `Id` = @Id_1, `Name` = @Name_1, `Address` = @Address_1 ; SELECT CAST(@Id_1 AS SIGNED) AS `Result`, @__RepoDb_OrderColumn_1 AS `OrderColumn` ; " +
                "INSERT INTO `Table` ( `Id`, `Name`, `Address` ) VALUES ( @Id_2, @Name_2, @Address_2 ) ON DUPLICATE KEY UPDATE `Id` = @Id_2, `Name` = @Name_2, `Address` = @Address_2 ; SELECT CAST(@Id_2 AS SIGNED) AS `Result`, @__RepoDb_OrderColumn_2 AS `OrderColumn` ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbStatementBuilderCreateMergeAllWithPrimaryAsQualifier()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<AuroraDbConnection>();

            // Act
            var query = builder.CreateMergeAll("Table",
                Field.From("Id", "Name", "Address"),
                Field.From("Id"),
                3,
                new DbField("Id", true, false, false, typeof(int), null, null, null, null, false),
                null);
            var expected = "INSERT INTO `Table` ( `Id`, `Name`, `Address` ) VALUES ( @Id, @Name, @Address ) ON DUPLICATE KEY UPDATE `Id` = @Id, `Name` = @Name, `Address` = @Address ; SELECT CAST(@Id AS SIGNED) AS `Result`, @__RepoDb_OrderColumn_0 AS `OrderColumn` ; " +
                "INSERT INTO `Table` ( `Id`, `Name`, `Address` ) VALUES ( @Id_1, @Name_1, @Address_1 ) ON DUPLICATE KEY UPDATE `Id` = @Id_1, `Name` = @Name_1, `Address` = @Address_1 ; SELECT CAST(@Id_1 AS SIGNED) AS `Result`, @__RepoDb_OrderColumn_1 AS `OrderColumn` ; " +
                "INSERT INTO `Table` ( `Id`, `Name`, `Address` ) VALUES ( @Id_2, @Name_2, @Address_2 ) ON DUPLICATE KEY UPDATE `Id` = @Id_2, `Name` = @Name_2, `Address` = @Address_2 ; SELECT CAST(@Id_2 AS SIGNED) AS `Result`, @__RepoDb_OrderColumn_2 AS `OrderColumn` ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbStatementBuilderCreateMergeAllWithIdentity()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<AuroraDbConnection>();

            // Act
            var query = builder.CreateMergeAll("Table",
                Field.From("Id", "Name", "Address"),
                null,
                3,
                new DbField("Id", true, false, false, typeof(int), null, null, null, null, false),
                new DbField("Id", false, true, false, typeof(int), null, null, null, null, false));
            var expected = "INSERT INTO `Table` ( `Id`, `Name`, `Address` ) VALUES ( @Id, @Name, @Address ) ON DUPLICATE KEY UPDATE `Id` = COALESCE(NULLIF(@Id, 0), LAST_INSERT_ID(`Id`)), `Name` = @Name, `Address` = @Address ; SELECT CAST(COALESCE(NULLIF(@Id, 0), LAST_INSERT_ID()) AS SIGNED) AS `Result`, @__RepoDb_OrderColumn_0 AS `OrderColumn` ; " +
                "INSERT INTO `Table` ( `Id`, `Name`, `Address` ) VALUES ( @Id_1, @Name_1, @Address_1 ) ON DUPLICATE KEY UPDATE `Id` = COALESCE(NULLIF(@Id_1, 0), LAST_INSERT_ID(`Id`)), `Name` = @Name_1, `Address` = @Address_1 ; SELECT CAST(COALESCE(NULLIF(@Id_1, 0), LAST_INSERT_ID()) AS SIGNED) AS `Result`, @__RepoDb_OrderColumn_1 AS `OrderColumn` ; " +
                "INSERT INTO `Table` ( `Id`, `Name`, `Address` ) VALUES ( @Id_2, @Name_2, @Address_2 ) ON DUPLICATE KEY UPDATE `Id` = COALESCE(NULLIF(@Id_2, 0), LAST_INSERT_ID(`Id`)), `Name` = @Name_2, `Address` = @Address_2 ; SELECT CAST(COALESCE(NULLIF(@Id_2, 0), LAST_INSERT_ID()) AS SIGNED) AS `Result`, @__RepoDb_OrderColumn_2 AS `OrderColumn` ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnAuroraDbStatementBuilderCreateMergeAllIfThereIsNoPrimary()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<AuroraDbConnection>();

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
        public void ThrowExceptionOnAuroraDbStatementBuilderCreateMergeAllIfThereAreNoFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<AuroraDbConnection>();

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
        public void ThrowExceptionOnAuroraDbStatementBuilderCreateMergeAllIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<AuroraDbConnection>();

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
        public void TestAuroraDbStatementBuilderCreateQuery()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<AuroraDbConnection>();

            // Act
            var query = builder.CreateQuery("Table",
                Field.From("Id", "Name", "Address"),
                null,
                null,
                null,
                null);
            var expected = "SELECT `Id`, `Name`, `Address` FROM `Table` ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbStatementBuilderCreateQueryWithExpression()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<AuroraDbConnection>();

            // Act
            var query = builder.CreateQuery("Table",
                Field.From("Id", "Name", "Address"),
                QueryGroup.Parse(new { Id = 1, Name = "Michael" }),
                null,
                null,
                null);
            var expected = "SELECT `Id`, `Name`, `Address` FROM `Table` WHERE (`Id` = @Id AND `Name` = @Name) ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbStatementBuilderCreateQueryWithTop()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<AuroraDbConnection>();

            // Act
            var query = builder.CreateQuery("Table",
                Field.From("Id", "Name", "Address"),
                null,
                null,
                10,
                null);
            var expected = "SELECT `Id`, `Name`, `Address` FROM `Table` LIMIT 10 ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbStatementBuilderCreateQueryOrderBy()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<AuroraDbConnection>();

            // Act
            var query = builder.CreateQuery("Table",
                Field.From("Id", "Name", "Address"),
                null,
                OrderField.Parse(new { Id = Order.Ascending }),
                null,
                null);
            var expected = "SELECT `Id`, `Name`, `Address` FROM `Table` ORDER BY `Id` ASC ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbStatementBuilderCreateQueryOrderByFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<AuroraDbConnection>();

            // Act
            var query = builder.CreateQuery("Table",
                Field.From("Id", "Name", "Address"),
                null,
                OrderField.Parse(new { Id = Order.Ascending, Name = Order.Ascending }),
                null,
                null);
            var expected = "SELECT `Id`, `Name`, `Address` FROM `Table` ORDER BY `Id` ASC, `Name` ASC ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbStatementBuilderCreateQueryOrderByDescending()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<AuroraDbConnection>();

            // Act
            var query = builder.CreateQuery("Table",
                Field.From("Id", "Name", "Address"),
                null,
                OrderField.Parse(new { Id = Order.Descending }),
                null,
                null);
            var expected = "SELECT `Id`, `Name`, `Address` FROM `Table` ORDER BY `Id` DESC ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbStatementBuilderCreateQueryOrderByFieldsDescending()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<AuroraDbConnection>();

            // Act
            var query = builder.CreateQuery("Table",
                Field.From("Id", "Name", "Address"),
                null,
                OrderField.Parse(new { Id = Order.Descending, Name = Order.Descending }),
                null,
                null);
            var expected = "SELECT `Id`, `Name`, `Address` FROM `Table` ORDER BY `Id` DESC, `Name` DESC ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbStatementBuilderCreateQueryOrderByFieldsMultiDirection()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<AuroraDbConnection>();

            // Act
            var query = builder.CreateQuery("Table",
                Field.From("Id", "Name", "Address"),
                null,
                OrderField.Parse(new { Id = Order.Ascending, Name = Order.Descending }),
                null,
                null);
            var expected = "SELECT `Id`, `Name`, `Address` FROM `Table` ORDER BY `Id` ASC, `Name` DESC ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnAuroraDbStatementBuilderCreateQueryIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<AuroraDbConnection>();

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
        public void TestAuroraDbStatementBuilderCreateSkipQuery()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<AuroraDbConnection>();

            // Act
            var query = builder.CreateSkipQuery("Table",
                Field.From("Id", "Name"),
                0,
                10,
                OrderField.Parse(new { Id = Order.Ascending }));
            var expected = "SELECT `Id`, `Name` FROM `Table` ORDER BY `Id` ASC LIMIT 10 ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbStatementBuilderCreateSkipQueryWithSkip()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<AuroraDbConnection>();

            // Act
            var query = builder.CreateSkipQuery("Table",
                Field.From("Id", "Name"),
                30,
                10,
                OrderField.Parse(new { Id = Order.Ascending }));
            var expected = "SELECT `Id`, `Name` FROM `Table` ORDER BY `Id` ASC LIMIT 30, 10 ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnAuroraDbStatementBuilderCreateSkipQueryIfThereAreNoFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<AuroraDbConnection>();

            // Act
            Assert.Throws<ArgumentNullException>(() =>
                builder.CreateSkipQuery("Table",
                    null,
                    0,
                    10,
                    OrderField.Parse(new { Id = Order.Ascending })));
        }

        [TestMethod]
        public void ThrowExceptionOnAuroraDbStatementBuilderCreateSkipQueryIfThereAreNoOrderFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<AuroraDbConnection>();

            // Act
            Assert.Throws<EmptyException>(() =>
                builder.CreateSkipQuery("Table",
                    Field.From("Id", "Name"),
                    0,
                    10,
                    null));
        }

        [TestMethod]
        public void ThrowExceptionOnAuroraDbStatementBuilderCreateSkipQueryIfTheSkipValueIsNullOrOutOfRange()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<AuroraDbConnection>();

            // Act
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                builder.CreateSkipQuery("Table",
                    Field.From("Id", "Name"),
                    -1,
                    10,
                    OrderField.Parse(new { Id = Order.Ascending })));
        }

        [TestMethod]
        public void ThrowExceptionOnAuroraDbStatementBuilderCreateSkipQueryIfTheTakeValueIsNullOrOutOfRange()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<AuroraDbConnection>();

            // Act
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                builder.CreateSkipQuery("Table",
                    Field.From("Id", "Name"),
                    0,
                    -1,
                    OrderField.Parse(new { Id = Order.Ascending })));
        }

        [TestMethod]
        public void ThrowExceptionOnAuroraDbStatementBuilderCreateSkipQueryIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<AuroraDbConnection>();

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
        public void TestAuroraDbStatementBuilderCreateSum()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<AuroraDbConnection>();

            // Act
            var query = builder.CreateSum("Table",
                new Field("Field", typeof(int)),
                null,
                null);
            var expected = "SELECT SUM(`Field`) AS `SumValue` FROM `Table` ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbStatementBuilderCreateSumWithExpression()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<AuroraDbConnection>();

            // Act
            var query = builder.CreateSum("Table",
                new Field("Field", typeof(int)),
                QueryGroup.Parse(new { Id = 1 }),
                null);
            var expected = "SELECT SUM(`Field`) AS `SumValue` FROM `Table` WHERE (`Id` = @Id) ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnAuroraDbStatementBuilderCreateSumIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<AuroraDbConnection>();

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
        public void TestAuroraDbStatementBuilderCreateSumAll()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<AuroraDbConnection>();

            // Act
            var query = builder.CreateSumAll("Table",
                new Field("Field", typeof(int)),
                null);
            var expected = "SELECT SUM(`Field`) AS `SumValue` FROM `Table` ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnAuroraDbStatementBuilderCreateSumAllIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<AuroraDbConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateSumAll("Table",
                    new Field("Field", typeof(int)),
                    "WhatEver"));
        }

        #endregion
    }
}
