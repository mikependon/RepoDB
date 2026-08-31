#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

// NOTE: The expected SQL string literals below were translated by hand from the original
// SapHana test suite (backtick -> double-quote identifiers, "@" -> ":" parameter prefix,
// LAST_INSERT_ID() -> CURRENT_IDENTITY_VALUE(), MySQL's "LIMIT skip, take" -> HANA's ANSI
// "LIMIT take OFFSET skip", and MySQL's "INSERT ... ON DUPLICATE KEY UPDATE" merge -> HANA's native
// "UPSERT ... WITH PRIMARY KEY"), tracing through the shared RepoDb.Core QueryBuilder/DbSetting
// formatting rather than by running the code - this project could not be compiled or executed in the
// environment these tests were written in. Verify against a real build before relying on them.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sap.Data.Hana;
using RepoDb.Enumerations;
using RepoDb.Exceptions;

namespace RepoDb.SapHana.UnitTests
{
    [TestClass]
    public class StatementBuilderTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseSapHana();
        }

        #region CreateBatchQuery

        [TestMethod]
        public void TestSapHanaStatementBuilderCreateBatchQuery()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<HanaConnection>();

            // Act
            var query = builder.CreateBatchQuery("Table",
                Field.From("Id", "Name"),
                0,
                10,
                OrderField.Parse(new { Id = Order.Ascending }));
            var expected = "SELECT \"Id\", \"Name\" FROM \"Table\" ORDER BY \"Id\" ASC LIMIT 10 ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestSapHanaStatementBuilderCreateBatchQueryWithPage()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<HanaConnection>();

            // Act
            var query = builder.CreateBatchQuery("Table",
                Field.From("Id", "Name"),
                3,
                10,
                OrderField.Parse(new { Id = Order.Ascending }));
            var expected = "SELECT \"Id\", \"Name\" FROM \"Table\" ORDER BY \"Id\" ASC LIMIT 10 OFFSET 30 ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnSapHanaStatementBuilderCreateBatchQueryIfThereAreNoFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<HanaConnection>();

            // Act
            Assert.Throws<NullReferenceException>(() =>
                builder.CreateBatchQuery("Table",
                    null,
                    0,
                    10,
                    OrderField.Parse(new { Id = Order.Ascending })));
        }

        [TestMethod]
        public void ThrowExceptionOnSapHanaStatementBuilderCreateBatchQueryIfThereAreNoOrderFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<HanaConnection>();

            // Act
            Assert.Throws<EmptyException>(() =>
                builder.CreateBatchQuery("Table",
                    Field.From("Id", "Name"),
                    0,
                    10,
                    null));
        }

        [TestMethod]
        public void ThrowExceptionOnSapHanaStatementBuilderCreateBatchQueryIfThePageValueIsNullOrOutOfRange()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<HanaConnection>();

            // Act
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                builder.CreateBatchQuery("Table",
                    Field.From("Id", "Name"),
                    -1,
                    10,
                    OrderField.Parse(new { Id = Order.Ascending })));
        }

        [TestMethod]
        public void ThrowExceptionOnSapHanaStatementBuilderCreateBatchQueryIfTheRowsPerBatchValueIsNullOrOutOfRange()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<HanaConnection>();

            // Act
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                builder.CreateBatchQuery("Table",
                    Field.From("Id", "Name"),
                    0,
                    -1,
                    OrderField.Parse(new { Id = Order.Ascending })));
        }

        [TestMethod]
        public void ThrowExceptionOnSapHanaStatementBuilderCreateBatchQueryIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<HanaConnection>();

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
        public void TestSapHanaStatementBuilderCreateCount()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<HanaConnection>();

            // Act
            var query = builder.CreateCount("Table",
                null,
                null);
            var expected = "SELECT COUNT(*) AS \"CountValue\" FROM \"Table\" ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestSapHanaStatementBuilderCreateCountWithExpression()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<HanaConnection>();

            // Act
            var query = builder.CreateCount("Table",
                QueryGroup.Parse(new { Id = 1 }),
                null);
            var expected = "SELECT COUNT(*) AS \"CountValue\" FROM \"Table\" WHERE (\"Id\" = :Id) ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnSapHanaStatementBuilderCreateCountIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<HanaConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateCount("Table",
                    QueryGroup.Parse(new { Id = 1 }),
                    "WhatEver"));
        }

        #endregion

        #region CreateCountAll

        [TestMethod]
        public void TestSapHanaStatementBuilderCreateCountAll()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<HanaConnection>();

            // Act
            var query = builder.CreateCountAll("Table",
                null);
            var expected = "SELECT COUNT(*) AS \"CountValue\" FROM \"Table\" ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnSapHanaStatementBuilderCreateCountAllIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<HanaConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateCountAll("Table",
                    "WhatEver"));
        }

        #endregion

        #region CreateExists

        [TestMethod]
        public void TestSapHanaStatementBuilderCreateExists()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<HanaConnection>();

            // Act
            var query = builder.CreateExists("Table",
                QueryGroup.Parse(new { Id = 1 }));
            var expected = "SELECT 1 AS \"ExistsValue\" FROM \"Table\" WHERE (\"Id\" = :Id) LIMIT 1 ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        #endregion

        #region CreateInsert

        [TestMethod]
        public void TestSapHanaStatementBuilderCreateInsert()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<HanaConnection>();

            // Act
            var query = builder.CreateInsert("Table",
                Field.From("Id", "Name", "Address"),
                null,
                null);
            var expected = "INSERT INTO \"Table\" ( \"Id\", \"Name\", \"Address\" ) VALUES ( :Id, :Name, :Address ) ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestSapHanaStatementBuilderCreateInsertWithPrimary()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<HanaConnection>();

            // Act
            var query = builder.CreateInsert("Table",
                Field.From("Id", "Name", "Address"),
                new DbField("Id", true, false, false, typeof(int), null, null, null, null, false),
                null);
            var expected = "INSERT INTO \"Table\" ( \"Id\", \"Name\", \"Address\" ) VALUES ( :Id, :Name, :Address ) ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestSapHanaStatementBuilderCreateInsertWithIdentity()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<HanaConnection>();

            // Act
            var query = builder.CreateInsert("Table",
                Field.From("Id", "Name", "Address"),
                null,
                new DbField("Id", false, true, false, typeof(int), null, null, null, null, false));
            var expected = "INSERT INTO \"Table\" ( \"Name\", \"Address\" ) VALUES ( :Name, :Address ) ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnSapHanaStatementBuilderCreateInsertIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<HanaConnection>();

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
        public void TestSapHanaStatementBuilderCreateInsertAll()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<HanaConnection>();

            // Act
            var query = builder.CreateInsertAll("Table",
                Field.From("Id", "Name", "Address"),
                1,
                null,
                null);
            var expected = "INSERT INTO \"Table\" ( \"Id\", \"Name\", \"Address\" ) VALUES ( :Id, :Name, :Address ) ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestSapHanaStatementBuilderCreateInsertAllWithPrimary()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<HanaConnection>();

            // Act
            var query = builder.CreateInsertAll("Table",
                Field.From("Id", "Name", "Address"),
                1,
                new DbField("Id", true, false, false, typeof(int), null, null, null, null, false),
                null);
            var expected = "INSERT INTO \"Table\" ( \"Id\", \"Name\", \"Address\" ) VALUES ( :Id, :Name, :Address ) ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestSapHanaStatementBuilderCreateInsertAllWithIdentity()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<HanaConnection>();

            // Act
            var query = builder.CreateInsertAll("Table",
                Field.From("Id", "Name", "Address"),
                1,
                null,
                new DbField("Id", false, true, false, typeof(int), null, null, null, null, false));
            var expected = "INSERT INTO \"Table\" ( \"Name\", \"Address\" ) VALUES ( :Name, :Address ) ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnSapHanaStatementBuilderCreateInsertAllIfBatchSizeIsGreaterThanOne()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<HanaConnection>();

            // Act - HANA's ADO.NET client rejects multi-statement command text, so SapHanaDbSetting sets
            // IsMultiStatementExecutable = false and this must reject any batchSize greater than 1.
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateInsertAll("Table",
                    Field.From("Id", "Name", "Address"),
                    3,
                    null,
                    null));
        }

        [TestMethod]
        public void ThrowExceptionOnSapHanaStatementBuilderCreateInsertAllIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<HanaConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateInsertAll("Table",
                    Field.From("Id", "Name", "Address"),
                    1,
                    null,
                    new DbField("Id", false, true, false, typeof(int), null, null, null, null, false),
                    "WhatEver"));
        }

        #endregion

        #region CreateMax

        [TestMethod]
        public void TestSapHanaStatementBuilderCreateMax()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<HanaConnection>();

            // Act
            var query = builder.CreateMax("Table",
                new Field("Field", typeof(int)),
                null,
                null);
            var expected = "SELECT MAX(\"Field\") AS \"MaxValue\" FROM \"Table\" ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestSapHanaStatementBuilderCreateMaxWithExpression()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<HanaConnection>();

            // Act
            var query = builder.CreateMax("Table",
                new Field("Field", typeof(int)),
                QueryGroup.Parse(new { Id = 1 }),
                null);
            var expected = "SELECT MAX(\"Field\") AS \"MaxValue\" FROM \"Table\" WHERE (\"Id\" = :Id) ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnSapHanaStatementBuilderCreateMaxIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<HanaConnection>();

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
        public void TestSapHanaStatementBuilderCreateMaxAll()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<HanaConnection>();

            // Act
            var query = builder.CreateMaxAll("Table",
                new Field("Field", typeof(int)),
                null);
            var expected = "SELECT MAX(\"Field\") AS \"MaxValue\" FROM \"Table\" ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnSapHanaStatementBuilderCreateMaxAllIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<HanaConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateMaxAll("Table",
                    new Field("Field", typeof(int)),
                    "WhatEver"));
        }

        #endregion

        #region CreateMin

        [TestMethod]
        public void TestSapHanaStatementBuilderCreateMin()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<HanaConnection>();

            // Act
            var query = builder.CreateMin("Table",
                new Field("Field", typeof(int)),
                null,
                null);
            var expected = "SELECT MIN(\"Field\") AS \"MinValue\" FROM \"Table\" ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestSapHanaStatementBuilderCreateMinWithExpression()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<HanaConnection>();

            // Act
            var query = builder.CreateMin("Table",
                new Field("Field", typeof(int)),
                QueryGroup.Parse(new { Id = 1 }),
                null);
            var expected = "SELECT MIN(\"Field\") AS \"MinValue\" FROM \"Table\" WHERE (\"Id\" = :Id) ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnSapHanaStatementBuilderCreateMinIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<HanaConnection>();

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
        public void TestSapHanaStatementBuilderCreateMinAll()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<HanaConnection>();

            // Act
            var query = builder.CreateMinAll("Table",
                new Field("Field", typeof(int)),
                null);
            var expected = "SELECT MIN(\"Field\") AS \"MinValue\" FROM \"Table\" ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnSapHanaStatementBuilderCreateMinAllIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<HanaConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateMinAll("Table",
                    new Field("Field", typeof(int)),
                    "WhatEver"));
        }

        #endregion

        #region CreateMerge

        [TestMethod]
        public void TestSapHanaStatementBuilderCreateMerge()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<HanaConnection>();

            // Act
            var query = builder.CreateMerge("Table",
                Field.From("Id", "Name", "Address"),
                null,
                new DbField("Id", true, false, false, typeof(int), null, null, null, null, false),
                null);
            var expected = "UPSERT \"Table\" ( \"Id\", \"Name\", \"Address\" ) VALUES ( :Id, :Name, :Address ) WITH PRIMARY KEY ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestSapHanaStatementBuilderCreateMergeWithPrimaryAsQualifier()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<HanaConnection>();

            // Act
            var query = builder.CreateMerge("Table",
                Field.From("Id", "Name", "Address"),
                Field.From("Id"),
                new DbField("Id", true, false, false, typeof(int), null, null, null, null, false),
                null);
            var expected = "UPSERT \"Table\" ( \"Id\", \"Name\", \"Address\" ) VALUES ( :Id, :Name, :Address ) WITH PRIMARY KEY ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestSapHanaStatementBuilderCreateMergeWithIdentity()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<HanaConnection>();

            // Act
            var query = builder.CreateMerge("Table",
                Field.From("Id", "Name", "Address"),
                null,
                new DbField("Id", true, false, false, typeof(int), null, null, null, null, false),
                new DbField("Id", false, true, false, typeof(int), null, null, null, null, false));
            var expected = "UPSERT \"Table\" ( \"Id\", \"Name\", \"Address\" ) VALUES ( :Id, :Name, :Address ) WITH PRIMARY KEY ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnSapHanaStatementBuilderCreateMergeIfThereIsNoPrimary()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<HanaConnection>();

            // Act
            Assert.Throws<PrimaryFieldNotFoundException>(() =>
                builder.CreateMerge("Table",
                    Field.From("Id", "Name", "Address"),
                    null,
                    null,
                    null));
        }

        [TestMethod]
        public void ThrowExceptionOnSapHanaStatementBuilderCreateMergeIfThereAreNoFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<HanaConnection>();

            // Act
            Assert.Throws<PrimaryFieldNotFoundException>(() =>
                builder.CreateMerge("Table",
                    Field.From("Id", "Name", "Address"),
                    null,
                    null,
                    null));
        }

        [TestMethod]
        public void ThrowExceptionOnSapHanaStatementBuilderCreateMergeIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<HanaConnection>();

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
        public void TestSapHanaStatementBuilderCreateMergeAll()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<HanaConnection>();

            // Act
            var query = builder.CreateMergeAll("Table",
                Field.From("Id", "Name", "Address"),
                null,
                1,
                new DbField("Id", true, false, false, typeof(int), null, null, null, null, false),
                null);
            var expected = "UPSERT \"Table\" ( \"Id\", \"Name\", \"Address\" ) VALUES ( :Id, :Name, :Address ) WITH PRIMARY KEY ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestSapHanaStatementBuilderCreateMergeAllWithPrimaryAsQualifier()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<HanaConnection>();

            // Act
            var query = builder.CreateMergeAll("Table",
                Field.From("Id", "Name", "Address"),
                Field.From("Id"),
                1,
                new DbField("Id", true, false, false, typeof(int), null, null, null, null, false),
                null);
            var expected = "UPSERT \"Table\" ( \"Id\", \"Name\", \"Address\" ) VALUES ( :Id, :Name, :Address ) WITH PRIMARY KEY ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestSapHanaStatementBuilderCreateMergeAllWithIdentity()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<HanaConnection>();

            // Act
            var query = builder.CreateMergeAll("Table",
                Field.From("Id", "Name", "Address"),
                null,
                1,
                new DbField("Id", true, false, false, typeof(int), null, null, null, null, false),
                new DbField("Id", false, true, false, typeof(int), null, null, null, null, false));
            var expected = "UPSERT \"Table\" ( \"Id\", \"Name\", \"Address\" ) VALUES ( :Id, :Name, :Address ) WITH PRIMARY KEY ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnSapHanaStatementBuilderCreateMergeAllIfBatchSizeIsGreaterThanOne()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<HanaConnection>();

            // Act - HANA's ADO.NET client rejects multi-statement command text, so SapHanaDbSetting sets
            // IsMultiStatementExecutable = false and this must reject any batchSize greater than 1.
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateMergeAll("Table",
                    Field.From("Id", "Name", "Address"),
                    null,
                    3,
                    new DbField("Id", true, false, false, typeof(int), null, null, null, null, false),
                    null));
        }

        [TestMethod]
        public void ThrowExceptionOnSapHanaStatementBuilderCreateMergeAllIfThereIsNoPrimary()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<HanaConnection>();

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
        public void ThrowExceptionOnSapHanaStatementBuilderCreateMergeAllIfThereAreNoFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<HanaConnection>();

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
        public void ThrowExceptionOnSapHanaStatementBuilderCreateMergeAllIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<HanaConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateMergeAll("Table",
                    Field.From("Id", "Name", "Address"),
                    Field.From("Id", "Name"),
                    1,
                    new DbField("Id", true, false, false, typeof(int), null, null, null, null, false),
                    null,
                    "WhatEver"));
        }

        #endregion

        #region CreateQuery

        [TestMethod]
        public void TestSapHanaStatementBuilderCreateQuery()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<HanaConnection>();

            // Act
            var query = builder.CreateQuery("Table",
                Field.From("Id", "Name", "Address"),
                null,
                null,
                null,
                null);
            var expected = "SELECT \"Id\", \"Name\", \"Address\" FROM \"Table\" ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestSapHanaStatementBuilderCreateQueryWithExpression()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<HanaConnection>();

            // Act
            var query = builder.CreateQuery("Table",
                Field.From("Id", "Name", "Address"),
                QueryGroup.Parse(new { Id = 1, Name = "Michael" }),
                null,
                null,
                null);
            var expected = "SELECT \"Id\", \"Name\", \"Address\" FROM \"Table\" WHERE (\"Id\" = :Id AND \"Name\" = :Name) ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestSapHanaStatementBuilderCreateQueryWithTop()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<HanaConnection>();

            // Act
            var query = builder.CreateQuery("Table",
                Field.From("Id", "Name", "Address"),
                null,
                null,
                10,
                null);
            var expected = "SELECT \"Id\", \"Name\", \"Address\" FROM \"Table\" LIMIT 10 ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestSapHanaStatementBuilderCreateQueryOrderBy()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<HanaConnection>();

            // Act
            var query = builder.CreateQuery("Table",
                Field.From("Id", "Name", "Address"),
                null,
                OrderField.Parse(new { Id = Order.Ascending }),
                null,
                null);
            var expected = "SELECT \"Id\", \"Name\", \"Address\" FROM \"Table\" ORDER BY \"Id\" ASC ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestSapHanaStatementBuilderCreateQueryOrderByFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<HanaConnection>();

            // Act
            var query = builder.CreateQuery("Table",
                Field.From("Id", "Name", "Address"),
                null,
                OrderField.Parse(new { Id = Order.Ascending, Name = Order.Ascending }),
                null,
                null);
            var expected = "SELECT \"Id\", \"Name\", \"Address\" FROM \"Table\" ORDER BY \"Id\" ASC, \"Name\" ASC ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestSapHanaStatementBuilderCreateQueryOrderByDescending()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<HanaConnection>();

            // Act
            var query = builder.CreateQuery("Table",
                Field.From("Id", "Name", "Address"),
                null,
                OrderField.Parse(new { Id = Order.Descending }),
                null,
                null);
            var expected = "SELECT \"Id\", \"Name\", \"Address\" FROM \"Table\" ORDER BY \"Id\" DESC ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestSapHanaStatementBuilderCreateQueryOrderByFieldsDescending()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<HanaConnection>();

            // Act
            var query = builder.CreateQuery("Table",
                Field.From("Id", "Name", "Address"),
                null,
                OrderField.Parse(new { Id = Order.Descending, Name = Order.Descending }),
                null,
                null);
            var expected = "SELECT \"Id\", \"Name\", \"Address\" FROM \"Table\" ORDER BY \"Id\" DESC, \"Name\" DESC ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestSapHanaStatementBuilderCreateQueryOrderByFieldsMultiDirection()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<HanaConnection>();

            // Act
            var query = builder.CreateQuery("Table",
                Field.From("Id", "Name", "Address"),
                null,
                OrderField.Parse(new { Id = Order.Ascending, Name = Order.Descending }),
                null,
                null);
            var expected = "SELECT \"Id\", \"Name\", \"Address\" FROM \"Table\" ORDER BY \"Id\" ASC, \"Name\" DESC ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnSapHanaStatementBuilderCreateQueryIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<HanaConnection>();

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
        public void TestSapHanaStatementBuilderCreateSkipQuery()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<HanaConnection>();

            // Act
            var query = builder.CreateSkipQuery("Table",
                Field.From("Id", "Name"),
                0,
                10,
                OrderField.Parse(new { Id = Order.Ascending }));
            var expected = "SELECT \"Id\", \"Name\" FROM \"Table\" ORDER BY \"Id\" ASC LIMIT 10 ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestSapHanaStatementBuilderCreateSkipQueryWithSkip()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<HanaConnection>();

            // Act
            var query = builder.CreateSkipQuery("Table",
                Field.From("Id", "Name"),
                30,
                10,
                OrderField.Parse(new { Id = Order.Ascending }));
            var expected = "SELECT \"Id\", \"Name\" FROM \"Table\" ORDER BY \"Id\" ASC LIMIT 10 OFFSET 30 ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnSapHanaStatementBuilderCreateSkipQueryIfThereAreNoFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<HanaConnection>();

            // Act
            Assert.Throws<NullReferenceException>(() =>
                builder.CreateSkipQuery("Table",
                    null,
                    0,
                    10,
                    OrderField.Parse(new { Id = Order.Ascending })));
        }

        [TestMethod]
        public void ThrowExceptionOnSapHanaStatementBuilderCreateSkipQueryIfThereAreNoOrderFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<HanaConnection>();

            // Act
            Assert.Throws<EmptyException>(() =>
                builder.CreateSkipQuery("Table",
                    Field.From("Id", "Name"),
                    0,
                    10,
                    null));
        }

        [TestMethod]
        public void ThrowExceptionOnSapHanaStatementBuilderCreateSkipQueryIfTheSkipValueIsNullOrOutOfRange()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<HanaConnection>();

            // Act
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                builder.CreateSkipQuery("Table",
                    Field.From("Id", "Name"),
                    -1,
                    10,
                    OrderField.Parse(new { Id = Order.Ascending })));
        }

        [TestMethod]
        public void ThrowExceptionOnSapHanaStatementBuilderCreateSkipQueryIfTheTakeValueIsNullOrOutOfRange()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<HanaConnection>();

            // Act
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                builder.CreateSkipQuery("Table",
                    Field.From("Id", "Name"),
                    0,
                    -1,
                    OrderField.Parse(new { Id = Order.Ascending })));
        }

        [TestMethod]
        public void ThrowExceptionOnSapHanaStatementBuilderCreateSkipQueryIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<HanaConnection>();

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
        public void TestSapHanaStatementBuilderCreateSum()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<HanaConnection>();

            // Act
            var query = builder.CreateSum("Table",
                new Field("Field", typeof(int)),
                null,
                null);
            var expected = "SELECT SUM(\"Field\") AS \"SumValue\" FROM \"Table\" ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestSapHanaStatementBuilderCreateSumWithExpression()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<HanaConnection>();

            // Act
            var query = builder.CreateSum("Table",
                new Field("Field", typeof(int)),
                QueryGroup.Parse(new { Id = 1 }),
                null);
            var expected = "SELECT SUM(\"Field\") AS \"SumValue\" FROM \"Table\" WHERE (\"Id\" = :Id) ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnSapHanaStatementBuilderCreateSumIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<HanaConnection>();

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
        public void TestSapHanaStatementBuilderCreateSumAll()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<HanaConnection>();

            // Act
            var query = builder.CreateSumAll("Table",
                new Field("Field", typeof(int)),
                null);
            var expected = "SELECT SUM(\"Field\") AS \"SumValue\" FROM \"Table\" ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnSapHanaStatementBuilderCreateSumAllIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<HanaConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateSumAll("Table",
                    new Field("Field", typeof(int)),
                    "WhatEver"));
        }

        #endregion
    }
}
