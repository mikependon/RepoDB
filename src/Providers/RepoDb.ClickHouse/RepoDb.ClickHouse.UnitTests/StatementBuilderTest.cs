#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClickHouse.Driver.ADO;
using RepoDb.Enumerations;
using RepoDb.Exceptions;

namespace RepoDb.ClickHouse.UnitTests
{
    [TestClass]
    public class StatementBuilderTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseClickHouse();
        }

        #region CreateBatchQuery

        [TestMethod]
        public void TestClickHouseStatementBuilderCreateBatchQuery()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<ClickHouseConnection>();

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
        public void TestClickHouseStatementBuilderCreateBatchQueryWithPage()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<ClickHouseConnection>();

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
        public void ThrowExceptionOnClickHouseStatementBuilderCreateBatchQueryIfThereAreNoFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<ClickHouseConnection>();

            // Act
            Assert.Throws<NullReferenceException>(() =>
                builder.CreateBatchQuery("Table",
                    null,
                    0,
                    10,
                    OrderField.Parse(new { Id = Order.Ascending })));
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseStatementBuilderCreateBatchQueryIfThereAreNoOrderFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<ClickHouseConnection>();

            // Act
            Assert.Throws<EmptyException>(() =>
                builder.CreateBatchQuery("Table",
                    Field.From("Id", "Name"),
                    0,
                    10,
                    null));
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseStatementBuilderCreateBatchQueryIfThePageValueIsNullOrOutOfRange()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<ClickHouseConnection>();

            // Act
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                builder.CreateBatchQuery("Table",
                    Field.From("Id", "Name"),
                    -1,
                    10,
                    OrderField.Parse(new { Id = Order.Ascending })));
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseStatementBuilderCreateBatchQueryIfTheRowsPerBatchValueIsNullOrOutOfRange()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<ClickHouseConnection>();

            // Act
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                builder.CreateBatchQuery("Table",
                    Field.From("Id", "Name"),
                    0,
                    -1,
                    OrderField.Parse(new { Id = Order.Ascending })));
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseStatementBuilderCreateBatchQueryIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<ClickHouseConnection>();

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
        public void TestClickHouseStatementBuilderCreateCount()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<ClickHouseConnection>();

            // Act
            var query = builder.CreateCount("Table",
                null,
                null);
            var expected = "SELECT COUNT (*) AS `CountValue` FROM `Table` ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestClickHouseStatementBuilderCreateCountWithExpression()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<ClickHouseConnection>();

            // Act
            var query = builder.CreateCount("Table",
                QueryGroup.Parse(new { Id = 1 }),
                null);
            var expected = "SELECT COUNT (*) AS `CountValue` FROM `Table` WHERE (`Id` = @Id) ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseStatementBuilderCreateCountIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<ClickHouseConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateCount("Table",
                    QueryGroup.Parse(new { Id = 1 }),
                    "WhatEver"));
        }

        #endregion

        #region CreateExists

        [TestMethod]
        public void TestClickHouseStatementBuilderCreateExists()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<ClickHouseConnection>();

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
        public void TestClickHouseStatementBuilderCreateInsert()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<ClickHouseConnection>();

            // Act
            var query = builder.CreateInsert("Table",
                Field.From("Id", "Name", "Address"),
                null,
                null);
            var expected = "INSERT INTO `Table` ( `Id`, `Name`, `Address` ) VALUES ( @Id, @Name, @Address ) ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestClickHouseStatementBuilderCreateInsertWithPrimary()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<ClickHouseConnection>();

            // Act
            var query = builder.CreateInsert("Table",
                Field.From("Id", "Name", "Address"),
                new DbField("Id", true, false, false, typeof(int), null, null, null, null, false),
                null);
            var expected = "INSERT INTO `Table` ( `Id`, `Name`, `Address` ) VALUES ( @Id, @Name, @Address ) ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseStatementBuilderCreateInsertIfThereIsAnIdentity()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<ClickHouseConnection>();

            // Act - ClickHouse has no identity/auto-increment mechanism
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateInsert("Table",
                    Field.From("Id", "Name", "Address"),
                    null,
                    new DbField("Id", false, true, false, typeof(int), null, null, null, null, false)));
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseStatementBuilderCreateInsertIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<ClickHouseConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateInsert("Table",
                    Field.From("Id", "Name", "Address"),
                    null,
                    null,
                    "WhatEver"));
        }

        #endregion

        #region CreateInsertAll

        [TestMethod]
        public void TestClickHouseStatementBuilderCreateInsertAll()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<ClickHouseConnection>();

            // Act
            var query = builder.CreateInsertAll("Table",
                Field.From("Id", "Name", "Address"),
                1,
                null,
                null);
            var expected = "INSERT INTO `Table` ( `Id`, `Name`, `Address` ) VALUES ( @Id, @Name, @Address ) ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseStatementBuilderCreateInsertAllIfBatchSizeIsGreaterThanOne()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<ClickHouseConnection>();

            // Act - ClickHouse.Driver executes a single statement per command/request
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateInsertAll("Table",
                    Field.From("Id", "Name", "Address"),
                    3,
                    null,
                    null));
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseStatementBuilderCreateInsertAllIfThereIsAnIdentity()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<ClickHouseConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateInsertAll("Table",
                    Field.From("Id", "Name", "Address"),
                    1,
                    null,
                    new DbField("Id", false, true, false, typeof(int), null, null, null, null, false)));
        }

        #endregion

        #region CreateMax

        [TestMethod]
        public void TestClickHouseStatementBuilderCreateMax()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<ClickHouseConnection>();

            // Act
            var query = builder.CreateMax("Table",
                new Field("Field", typeof(int)),
                null,
                null);
            var expected = "SELECT MAX (`Field`) AS `MaxValue` FROM `Table` ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        #endregion

        #region CreateMin

        [TestMethod]
        public void TestClickHouseStatementBuilderCreateMin()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<ClickHouseConnection>();

            // Act
            var query = builder.CreateMin("Table",
                new Field("Field", typeof(int)),
                null,
                null);
            var expected = "SELECT MIN (`Field`) AS `MinValue` FROM `Table` ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        #endregion

        #region CreateMerge

        [TestMethod]
        public void TestClickHouseStatementBuilderCreateMerge()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<ClickHouseConnection>();

            // Act
            var query = builder.CreateMerge("Table",
                Field.From("Id", "Name", "Address"),
                null,
                new DbField("Id", true, false, false, typeof(int), null, null, null, null, false),
                null);
            var expected = "INSERT INTO `Table` ( `Id`, `Name`, `Address` ) VALUES ( @Id, @Name, @Address ) ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseStatementBuilderCreateMergeIfThereIsNoPrimary()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<ClickHouseConnection>();

            // Act
            Assert.Throws<PrimaryFieldNotFoundException>(() =>
                builder.CreateMerge("Table",
                    Field.From("Id", "Name", "Address"),
                    null,
                    null,
                    null));
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseStatementBuilderCreateMergeIfThereIsAnIdentity()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<ClickHouseConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateMerge("Table",
                    Field.From("Id", "Name", "Address"),
                    null,
                    new DbField("Id", true, false, false, typeof(int), null, null, null, null, false),
                    new DbField("Id", false, true, false, typeof(int), null, null, null, null, false)));
        }

        #endregion

        #region CreateMergeAll

        [TestMethod]
        public void TestClickHouseStatementBuilderCreateMergeAll()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<ClickHouseConnection>();

            // Act
            var query = builder.CreateMergeAll("Table",
                Field.From("Id", "Name", "Address"),
                null,
                1,
                new DbField("Id", true, false, false, typeof(int), null, null, null, null, false),
                null);
            var expected = "INSERT INTO `Table` ( `Id`, `Name`, `Address` ) VALUES ( @Id, @Name, @Address ) ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseStatementBuilderCreateMergeAllIfThereIsNoPrimary()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<ClickHouseConnection>();

            // Act
            Assert.Throws<PrimaryFieldNotFoundException>(() =>
                builder.CreateMergeAll("Table",
                    Field.From("Id", "Name", "Address"),
                    null,
                    1,
                    null,
                    null));
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseStatementBuilderCreateMergeAllIfBatchSizeIsGreaterThanOne()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<ClickHouseConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateMergeAll("Table",
                    Field.From("Id", "Name", "Address"),
                    null,
                    3,
                    new DbField("Id", true, false, false, typeof(int), null, null, null, null, false),
                    null));
        }

        #endregion

        #region CreateQuery

        [TestMethod]
        public void TestClickHouseStatementBuilderCreateQuery()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<ClickHouseConnection>();

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
        public void TestClickHouseStatementBuilderCreateQueryWithExpression()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<ClickHouseConnection>();

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
        public void TestClickHouseStatementBuilderCreateQueryWithTop()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<ClickHouseConnection>();

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
        public void TestClickHouseStatementBuilderCreateQueryOrderBy()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<ClickHouseConnection>();

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
        public void ThrowExceptionOnClickHouseStatementBuilderCreateQueryIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<ClickHouseConnection>();

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
        public void TestClickHouseStatementBuilderCreateSkipQuery()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<ClickHouseConnection>();

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
        public void TestClickHouseStatementBuilderCreateSkipQueryWithSkip()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<ClickHouseConnection>();

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

        #endregion

        #region CreateSum

        [TestMethod]
        public void TestClickHouseStatementBuilderCreateSum()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<ClickHouseConnection>();

            // Act
            var query = builder.CreateSum("Table",
                new Field("Field", typeof(int)),
                null,
                null);
            var expected = "SELECT SUM (`Field`) AS `SumValue` FROM `Table` ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        #endregion

        #region CreateUpdate

        [TestMethod]
        public void TestClickHouseStatementBuilderCreateUpdate()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<ClickHouseConnection>();

            // Act
            var query = builder.CreateUpdate("Table",
                Field.From("Id", "Name", "Address"),
                QueryGroup.Parse(new { Id = 1 }),
                new DbField("Id", true, false, false, typeof(int), null, null, null, null, false),
                null);
            var expected = "ALTER TABLE `Table` UPDATE `Name` = @Name, `Address` = @Address WHERE (`Id` = @Id) ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        #endregion

        #region CreateUpdateAll

        [TestMethod]
        public void TestClickHouseStatementBuilderCreateUpdateAll()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<ClickHouseConnection>();

            // Act
            var query = builder.CreateUpdateAll("Table",
                Field.From("Id", "Name", "Address"),
                Field.From("Id"),
                1,
                new DbField("Id", true, false, false, typeof(int), null, null, null, null, false),
                null);
            var expected = "ALTER TABLE `Table` UPDATE `Name` = @Name, `Address` = @Address WHERE (`Id` = @Id) ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseStatementBuilderCreateUpdateAllIfBatchSizeIsGreaterThanOne()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<ClickHouseConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateUpdateAll("Table",
                    Field.From("Id", "Name", "Address"),
                    Field.From("Id"),
                    3,
                    new DbField("Id", true, false, false, typeof(int), null, null, null, null, false),
                    null));
        }

        #endregion
    }
}
