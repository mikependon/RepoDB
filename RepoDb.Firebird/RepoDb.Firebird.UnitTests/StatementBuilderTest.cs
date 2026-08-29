#region Copyright Attributions

// Copyright (c) 2019 Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FirebirdSql.Data.FirebirdClient;
using RepoDb.Enumerations;
using RepoDb.Exceptions;

namespace RepoDb.Firebird.UnitTests
{
    [TestClass]
    public class StatementBuilderTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseFirebird();
        }

        #region CreateBatchQuery

        [TestMethod]
        public void TestFirebirdStatementBuilderCreateBatchQuery()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

            // Act
            var query = builder.CreateBatchQuery("Table",
                Field.From("Id", "Name"),
                0,
                10,
                OrderField.Parse(new { Id = Order.Ascending }));
            var expected = "SELECT FIRST 10 SKIP 0 \"Id\", \"Name\" FROM \"Table\" ORDER BY \"Id\" ASC";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestFirebirdStatementBuilderCreateBatchQueryWithPage()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

            // Act
            var query = builder.CreateBatchQuery("Table",
                Field.From("Id", "Name"),
                3,
                10,
                OrderField.Parse(new { Id = Order.Ascending }));
            var expected = "SELECT FIRST 10 SKIP 30 \"Id\", \"Name\" FROM \"Table\" ORDER BY \"Id\" ASC";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnFirebirdStatementBuilderCreateBatchQueryIfThereAreNoFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

            // Act
            Assert.Throws<NullReferenceException>(() =>
                builder.CreateBatchQuery("Table",
                    null,
                    0,
                    10,
                    OrderField.Parse(new { Id = Order.Ascending })));
        }

        [TestMethod]
        public void ThrowExceptionOnFirebirdStatementBuilderCreateBatchQueryIfThereAreNoOrderFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

            // Act
            Assert.Throws<EmptyException>(() =>
                builder.CreateBatchQuery("Table",
                    Field.From("Id", "Name"),
                    0,
                    10,
                    null));
        }

        [TestMethod]
        public void ThrowExceptionOnFirebirdStatementBuilderCreateBatchQueryIfThePageValueIsNullOrOutOfRange()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

            // Act
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                builder.CreateBatchQuery("Table",
                    Field.From("Id", "Name"),
                    -1,
                    10,
                    OrderField.Parse(new { Id = Order.Ascending })));
        }

        [TestMethod]
        public void ThrowExceptionOnFirebirdStatementBuilderCreateBatchQueryIfTheRowsPerBatchValueIsNullOrOutOfRange()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

            // Act
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                builder.CreateBatchQuery("Table",
                    Field.From("Id", "Name"),
                    0,
                    -1,
                    OrderField.Parse(new { Id = Order.Ascending })));
        }

        [TestMethod]
        public void ThrowExceptionOnFirebirdStatementBuilderCreateBatchQueryIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateBatchQuery("Table",
                    Field.From("Id", "Name"),
                    0,
                    10,
                    OrderField.Parse(new { Id = Order.Ascending }),
                    null,
                    "WhatEver"));
        }

        #endregion

        #region CreateCount

        [TestMethod]
        public void TestFirebirdStatementBuilderCreateCount()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

            // Act
            var query = builder.CreateCount("Table",
                null,
                null);
            var expected = "SELECT COUNT (*) AS \"CountValue\" FROM \"Table\"";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestFirebirdStatementBuilderCreateCountWithExpression()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

            // Act
            var query = builder.CreateCount("Table",
                QueryGroup.Parse(new { Id = 1 }),
                null);
            var expected = "SELECT COUNT (*) AS \"CountValue\" FROM \"Table\" WHERE (\"Id\" = @Id)";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnFirebirdStatementBuilderCreateCountIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateCount("Table",
                    QueryGroup.Parse(new { Id = 1 }),
                    "WhatEver"));
        }

        #endregion

        #region CreateCountAll

        [TestMethod]
        public void TestFirebirdStatementBuilderCreateCountAll()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

            // Act
            var query = builder.CreateCountAll("Table",
                null);
            var expected = "SELECT COUNT (*) AS \"CountValue\" FROM \"Table\"";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnFirebirdStatementBuilderCreateCountAllIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateCountAll("Table",
                    "WhatEver"));
        }

        #endregion

        #region CreateExists

        [TestMethod]
        public void TestFirebirdStatementBuilderCreateExists()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

            // Act
            var query = builder.CreateExists("Table",
                QueryGroup.Parse(new { Id = 1 }));
            var expected = "SELECT FIRST 1 1 AS \"ExistsValue\" FROM \"Table\" WHERE (\"Id\" = @Id)";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnFirebirdStatementBuilderCreateExistsIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateExists("Table",
                    QueryGroup.Parse(new { Id = 1 }),
                    "WhatEver"));
        }

        #endregion

        #region CreateInsert

        [TestMethod]
        public void TestFirebirdStatementBuilderCreateInsertWithNoKeyColumn()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

            // Act
            var query = builder.CreateInsert("Table",
                Field.From("Id", "Name", "Address"),
                null,
                null);
            var expected = "INSERT INTO \"Table\" ( \"Id\", \"Name\", \"Address\" ) VALUES ( @Id, @Name, @Address )";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestFirebirdStatementBuilderCreateInsertWithPrimary()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

            // Act - a plain (non-identity) primary is included in the column list (only an identity
            // column is excluded); GetReturnKeyColumnAsDbField's default (IdentityOrElsePrimary) falls
            // back to the primary as the RETURNING key column since there is no identity here.
            var query = builder.CreateInsert("Table",
                Field.From("Id", "Name", "Address"),
                new DbField("Id", true, false, false, typeof(int), null, null, null, null),
                null);
            var expected = "INSERT INTO \"Table\" ( \"Id\", \"Name\", \"Address\" ) VALUES ( @Id, @Name, @Address ) RETURNING \"Id\" AS \"Result\"";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestFirebirdStatementBuilderCreateInsertWithIdentity()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

            // Act
            var query = builder.CreateInsert("Table",
                Field.From("Id", "Name", "Address"),
                null,
                new DbField("Id", false, true, false, typeof(int), null, null, null, null));
            var expected = "INSERT INTO \"Table\" ( \"Name\", \"Address\" ) VALUES ( @Name, @Address ) RETURNING \"Id\" AS \"Result\"";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnFirebirdStatementBuilderCreateInsertIfThereAreNoFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

            // Act
            Assert.Throws<EmptyException>(() =>
                builder.CreateInsert("Table",
                    null,
                    null,
                    null));
        }

        [TestMethod]
        public void ThrowExceptionOnFirebirdStatementBuilderCreateInsertIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateInsert("Table",
                    Field.From("Id", "Name", "Address"),
                    null,
                    new DbField("Id", false, true, false, typeof(int), null, null, null, null),
                    "WhatEver"));
        }

        #endregion

        #region CreateInsertAll

        [TestMethod]
        public void TestFirebirdStatementBuilderCreateInsertAllWithIdentity()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

            // Act - batchSize of 1 reuses the single-row Insert statement.
            var query = builder.CreateInsertAll("Table",
                Field.From("Id", "Name", "Address"),
                1,
                null,
                new DbField("Id", false, true, false, typeof(int), null, null, null, null));
            var expected = "INSERT INTO \"Table\" ( \"Name\", \"Address\" ) VALUES ( @Name, @Address ) RETURNING \"Id\" AS \"Result\"";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnFirebirdStatementBuilderCreateInsertAllIfBatchSizeIsGreaterThanOne()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

            // Act - FirebirdDbSetting.IsMultiStatementExecutable is false, so a batchSize greater than
            // 1 is rejected rather than silently producing multiple statements the ADO.NET provider
            // cannot execute in one round-trip.
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateInsertAll("Table",
                    Field.From("Id", "Name", "Address"),
                    3,
                    null,
                    null));
        }

        [TestMethod]
        public void ThrowExceptionOnFirebirdStatementBuilderCreateInsertAllIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateInsertAll("Table",
                    Field.From("Id", "Name", "Address"),
                    1,
                    null,
                    new DbField("Id", false, true, false, typeof(int), null, null, null, null),
                    "WhatEver"));
        }

        #endregion

        #region CreateMax

        [TestMethod]
        public void TestFirebirdStatementBuilderCreateMax()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

            // Act
            var query = builder.CreateMax("Table",
                new Field("Field"),
                null,
                null);
            var expected = "SELECT MAX (\"Field\") AS \"MaxValue\" FROM \"Table\"";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestFirebirdStatementBuilderCreateMaxWithExpression()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

            // Act
            var query = builder.CreateMax("Table",
                new Field("Field"),
                QueryGroup.Parse(new { Id = 1 }),
                null);
            var expected = "SELECT MAX (\"Field\") AS \"MaxValue\" FROM \"Table\" WHERE (\"Id\" = @Id)";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnFirebirdStatementBuilderCreateMaxIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateMax("Table",
                    new Field("Field"),
                    QueryGroup.Parse(new { Id = 1 }),
                    "WhatEver"));
        }

        #endregion

        #region CreateMaxAll

        [TestMethod]
        public void TestFirebirdStatementBuilderCreateMaxAll()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

            // Act
            var query = builder.CreateMaxAll("Table",
                new Field("Field"),
                null);
            var expected = "SELECT MAX (\"Field\") AS \"MaxValue\" FROM \"Table\"";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnFirebirdStatementBuilderCreateMaxAllIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateMaxAll("Table",
                    new Field("Field"),
                    "WhatEver"));
        }

        #endregion

        #region CreateMin

        [TestMethod]
        public void TestFirebirdStatementBuilderCreateMin()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

            // Act
            var query = builder.CreateMin("Table",
                new Field("Field"),
                null,
                null);
            var expected = "SELECT MIN (\"Field\") AS \"MinValue\" FROM \"Table\"";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestFirebirdStatementBuilderCreateMinWithExpression()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

            // Act
            var query = builder.CreateMin("Table",
                new Field("Field"),
                QueryGroup.Parse(new { Id = 1 }),
                null);
            var expected = "SELECT MIN (\"Field\") AS \"MinValue\" FROM \"Table\" WHERE (\"Id\" = @Id)";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnFirebirdStatementBuilderCreateMinIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateMin("Table",
                    new Field("Field"),
                    QueryGroup.Parse(new { Id = 1 }),
                    "WhatEver"));
        }

        #endregion

        #region CreateMinAll

        [TestMethod]
        public void TestFirebirdStatementBuilderCreateMinAll()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

            // Act
            var query = builder.CreateMinAll("Table",
                new Field("Field"),
                null);
            var expected = "SELECT MIN (\"Field\") AS \"MinValue\" FROM \"Table\"";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnFirebirdStatementBuilderCreateMinAllIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateMinAll("Table",
                    new Field("Field"),
                    "WhatEver"));
        }

        #endregion

        #region CreateSum

        [TestMethod]
        public void TestFirebirdStatementBuilderCreateSum()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

            // Act
            var query = builder.CreateSum("Table",
                new Field("Field"),
                null,
                null);
            var expected = "SELECT SUM (\"Field\") AS \"SumValue\" FROM \"Table\"";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestFirebirdStatementBuilderCreateSumWithExpression()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

            // Act
            var query = builder.CreateSum("Table",
                new Field("Field"),
                QueryGroup.Parse(new { Id = 1 }),
                null);
            var expected = "SELECT SUM (\"Field\") AS \"SumValue\" FROM \"Table\" WHERE (\"Id\" = @Id)";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnFirebirdStatementBuilderCreateSumIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateSum("Table",
                    new Field("Field"),
                    QueryGroup.Parse(new { Id = 1 }),
                    "WhatEver"));
        }

        #endregion

        #region CreateSumAll

        [TestMethod]
        public void TestFirebirdStatementBuilderCreateSumAll()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

            // Act
            var query = builder.CreateSumAll("Table",
                new Field("Field"),
                null);
            var expected = "SELECT SUM (\"Field\") AS \"SumValue\" FROM \"Table\"";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnFirebirdStatementBuilderCreateSumAllIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateSumAll("Table",
                    new Field("Field"),
                    "WhatEver"));
        }

        #endregion

        #region CreateAverage

        [TestMethod]
        public void TestFirebirdStatementBuilderCreateAverage()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

            // Act - the averaged field is CAST to DOUBLE PRECISION because Firebird's AVG() otherwise
            // returns a value of the same exact-numeric type as its argument (e.g. AVG(INTEGER) is
            // itself INTEGER), truncating the fractional part instead of widening like MySQL/SQL Server.
            var query = builder.CreateAverage("Table",
                new Field("Field", typeof(int)),
                null,
                null);
            var expected = "SELECT AVG (CAST(\"Field\" AS DOUBLE PRECISION)) AS \"AverageValue\" FROM \"Table\"";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestFirebirdStatementBuilderCreateAverageWithoutFieldTypeDefaultsToDouble()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

            // Act - BaseStatementBuilder.CreateAverage defaults a null field.Type to typeof(double)
            // before this provider's ConvertFieldResolver ever runs, so even a type-less field still
            // gets CAST to DOUBLE PRECISION, not left bare.
            var query = builder.CreateAverage("Table",
                new Field("Field"),
                null,
                null);
            var expected = "SELECT AVG (CAST(\"Field\" AS DOUBLE PRECISION)) AS \"AverageValue\" FROM \"Table\"";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestFirebirdStatementBuilderCreateAverageWithDecimalFieldTypeIsNotWidened()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

            // Act - only exact-numeric integer types (short/int/long and their unsigned counterparts)
            // are widened to double by ClientTypeToAverageableClientTypeResolver; a decimal field is
            // passed through as-is and CAST to Firebird's DECIMAL(18,2).
            var query = builder.CreateAverage("Table",
                new Field("Field", typeof(decimal)),
                null,
                null);
            var expected = "SELECT AVG (CAST(\"Field\" AS DECIMAL(18,2))) AS \"AverageValue\" FROM \"Table\"";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnFirebirdStatementBuilderCreateAverageIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateAverage("Table",
                    new Field("Field", typeof(int)),
                    null,
                    "WhatEver"));
        }

        #endregion

        #region CreateMerge

        [TestMethod]
        public void TestFirebirdStatementBuilderCreateMerge()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

            // Act
            var query = builder.CreateMerge("Table",
                Field.From("Id", "Name", "Address"),
                null,
                new DbField("Id", true, false, false, typeof(int), null, null, null, null),
                null);
            var expected = "UPDATE OR INSERT INTO \"Table\" ( \"Id\", \"Name\", \"Address\" ) VALUES ( @Id, @Name, @Address ) " +
                "MATCHING ( \"Id\" ) RETURNING \"Id\" AS \"Result\"";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestFirebirdStatementBuilderCreateMergeWithPrimaryAsQualifier()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

            // Act
            var query = builder.CreateMerge("Table",
                Field.From("Id", "Name", "Address"),
                Field.From("Id"),
                new DbField("Id", true, false, false, typeof(int), null, null, null, null),
                null);
            var expected = "UPDATE OR INSERT INTO \"Table\" ( \"Id\", \"Name\", \"Address\" ) VALUES ( @Id, @Name, @Address ) " +
                "MATCHING ( \"Id\" ) RETURNING \"Id\" AS \"Result\"";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestFirebirdStatementBuilderCreateMergeWithIdentity()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

            // Act
            var query = builder.CreateMerge("Table",
                Field.From("Id", "Name", "Address"),
                null,
                new DbField("Id", true, false, false, typeof(int), null, null, null, null),
                new DbField("Id", false, true, false, typeof(int), null, null, null, null));
            var expected = "EXECUTE BLOCK (" +
                "P0 TYPE OF COLUMN \"Table\".\"Id\" = @Id, " +
                "P1 TYPE OF COLUMN \"Table\".\"Name\" = @Name, " +
                "P2 TYPE OF COLUMN \"Table\".\"Address\" = @Address" +
                ") RETURNS (R0 TYPE OF COLUMN \"Table\".\"Id\") AS BEGIN " +
                "IF (:P0 IS NULL OR :P0 = 0) THEN BEGIN " +
                "INSERT INTO \"Table\" (\"Name\", \"Address\") VALUES (:P1, :P2) RETURNING \"Id\" INTO :R0; END " +
                "ELSE BEGIN " +
                "UPDATE OR INSERT INTO \"Table\" (\"Id\", \"Name\", \"Address\") VALUES (:P0, :P1, :P2) MATCHING (\"Id\") RETURNING \"Id\" INTO :R0; END " +
                "SUSPEND; END";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestFirebirdStatementBuilderCreateMergeWithCustomQualifiers()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

            // Act
            var query = builder.CreateMerge("Table",
                Field.From("Id", "Name", "Address"),
                Field.From("Name"),
                new DbField("Id", true, false, false, typeof(int), null, null, null, null),
                null);
            var expected = "UPDATE OR INSERT INTO \"Table\" ( \"Id\", \"Name\", \"Address\" ) VALUES ( @Id, @Name, @Address ) " +
                "MATCHING ( \"Name\" ) RETURNING \"Id\" AS \"Result\"";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnFirebirdStatementBuilderCreateMergeIfThereIsNoPrimary()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

            // Act
            Assert.Throws<PrimaryFieldNotFoundException>(() =>
                builder.CreateMerge("Table",
                    Field.From("Id", "Name", "Address"),
                    null,
                    null,
                    null));
        }

        [TestMethod]
        public void ThrowExceptionOnFirebirdStatementBuilderCreateMergeIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateMerge("Table",
                    Field.From("Id", "Name", "Address"),
                    Field.From("Id"),
                    new DbField("Id", true, false, false, typeof(int), null, null, null, null),
                    null,
                    "WhatEver"));
        }

        #endregion

        #region CreateMergeAll

        [TestMethod]
        public void TestFirebirdStatementBuilderCreateMergeAll()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

            // Act - batchSize of 1 reuses the single-row Merge statement.
            var query = builder.CreateMergeAll("Table",
                Field.From("Id", "Name", "Address"),
                null,
                1,
                new DbField("Id", true, false, false, typeof(int), null, null, null, null),
                null);
            var expected = "UPDATE OR INSERT INTO \"Table\" ( \"Id\", \"Name\", \"Address\" ) VALUES ( @Id, @Name, @Address ) " +
                "MATCHING ( \"Id\" ) RETURNING \"Id\" AS \"Result\"";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnFirebirdStatementBuilderCreateMergeAllIfBatchSizeIsGreaterThanOne()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateMergeAll("Table",
                    Field.From("Id", "Name", "Address"),
                    null,
                    3,
                    new DbField("Id", true, false, false, typeof(int), null, null, null, null),
                    null));
        }

        [TestMethod]
        public void ThrowExceptionOnFirebirdStatementBuilderCreateMergeAllIfThereIsNoPrimary()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

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
        public void ThrowExceptionOnFirebirdStatementBuilderCreateMergeAllIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateMergeAll("Table",
                    Field.From("Id", "Name", "Address"),
                    Field.From("Id"),
                    1,
                    new DbField("Id", true, false, false, typeof(int), null, null, null, null),
                    null,
                    "WhatEver"));
        }

        #endregion

        #region CreateQuery

        [TestMethod]
        public void TestFirebirdStatementBuilderCreateQuery()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

            // Act
            var query = builder.CreateQuery("Table",
                Field.From("Id", "Name", "Address"),
                null,
                null,
                null,
                null);
            var expected = "SELECT \"Id\", \"Name\", \"Address\" FROM \"Table\"";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestFirebirdStatementBuilderCreateQueryWithExpression()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

            // Act
            var query = builder.CreateQuery("Table",
                Field.From("Id", "Name", "Address"),
                QueryGroup.Parse(new { Id = 1, Name = "Michael" }),
                null,
                null,
                null);
            var expected = "SELECT \"Id\", \"Name\", \"Address\" FROM \"Table\" WHERE (\"Id\" = @Id AND \"Name\" = @Name)";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestFirebirdStatementBuilderCreateQueryWithTop()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

            // Act
            var query = builder.CreateQuery("Table",
                Field.From("Id", "Name", "Address"),
                null,
                null,
                10,
                null);
            var expected = "SELECT FIRST 10 \"Id\", \"Name\", \"Address\" FROM \"Table\"";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestFirebirdStatementBuilderCreateQueryOrderBy()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

            // Act
            var query = builder.CreateQuery("Table",
                Field.From("Id", "Name", "Address"),
                null,
                OrderField.Parse(new { Id = Order.Ascending }),
                null,
                null);
            var expected = "SELECT \"Id\", \"Name\", \"Address\" FROM \"Table\" ORDER BY \"Id\" ASC";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestFirebirdStatementBuilderCreateQueryOrderByFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

            // Act
            var query = builder.CreateQuery("Table",
                Field.From("Id", "Name", "Address"),
                null,
                OrderField.Parse(new { Id = Order.Ascending, Name = Order.Ascending }),
                null,
                null);
            var expected = "SELECT \"Id\", \"Name\", \"Address\" FROM \"Table\" ORDER BY \"Id\" ASC, \"Name\" ASC";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestFirebirdStatementBuilderCreateQueryOrderByDescending()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

            // Act
            var query = builder.CreateQuery("Table",
                Field.From("Id", "Name", "Address"),
                null,
                OrderField.Parse(new { Id = Order.Descending }),
                null,
                null);
            var expected = "SELECT \"Id\", \"Name\", \"Address\" FROM \"Table\" ORDER BY \"Id\" DESC";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnFirebirdStatementBuilderCreateQueryIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

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
        public void TestFirebirdStatementBuilderCreateSkipQuery()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

            // Act
            var query = builder.CreateSkipQuery("Table",
                Field.From("Id", "Name"),
                0,
                10,
                OrderField.Parse(new { Id = Order.Ascending }));
            var expected = "SELECT FIRST 10 SKIP 0 \"Id\", \"Name\" FROM \"Table\" ORDER BY \"Id\" ASC";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestFirebirdStatementBuilderCreateSkipQueryWithSkip()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

            // Act
            var query = builder.CreateSkipQuery("Table",
                Field.From("Id", "Name"),
                30,
                10,
                OrderField.Parse(new { Id = Order.Ascending }));
            var expected = "SELECT FIRST 10 SKIP 30 \"Id\", \"Name\" FROM \"Table\" ORDER BY \"Id\" ASC";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnFirebirdStatementBuilderCreateSkipQueryIfThereAreNoFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

            // Act
            Assert.Throws<NullReferenceException>(() =>
                builder.CreateSkipQuery("Table",
                    null,
                    0,
                    10,
                    OrderField.Parse(new { Id = Order.Ascending })));
        }

        [TestMethod]
        public void ThrowExceptionOnFirebirdStatementBuilderCreateSkipQueryIfThereAreNoOrderFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

            // Act
            Assert.Throws<EmptyException>(() =>
                builder.CreateSkipQuery("Table",
                    Field.From("Id", "Name"),
                    0,
                    10,
                    null));
        }

        [TestMethod]
        public void ThrowExceptionOnFirebirdStatementBuilderCreateSkipQueryIfTheSkipValueIsNullOrOutOfRange()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

            // Act
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                builder.CreateSkipQuery("Table",
                    Field.From("Id", "Name"),
                    -1,
                    10,
                    OrderField.Parse(new { Id = Order.Ascending })));
        }

        [TestMethod]
        public void ThrowExceptionOnFirebirdStatementBuilderCreateSkipQueryIfTheTakeValueIsNullOrOutOfRange()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

            // Act
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                builder.CreateSkipQuery("Table",
                    Field.From("Id", "Name"),
                    0,
                    -1,
                    OrderField.Parse(new { Id = Order.Ascending })));
        }

        [TestMethod]
        public void ThrowExceptionOnFirebirdStatementBuilderCreateSkipQueryIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateSkipQuery("Table",
                    Field.From("Id", "Name"),
                    0,
                    10,
                    OrderField.Parse(new { Id = Order.Ascending }),
                    null,
                    "WhatEver"));
        }

        #endregion

        #region CreateUpdate

        [TestMethod]
        public void TestFirebirdStatementBuilderCreateUpdate()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

            // Act
            var query = builder.CreateUpdate("Table",
                Field.From("Name", "Address"),
                QueryGroup.Parse(new { Id = 1 }),
                new DbField("Id", true, false, false, typeof(int), null, null, null, null),
                null);
            var expected = "UPDATE \"Table\" SET \"Name\" = @Name, \"Address\" = @Address WHERE (\"Id\" = @Id)";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnFirebirdStatementBuilderCreateUpdateAllIfBatchSizeIsGreaterThanOne()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateUpdateAll("Table",
                    Field.From("Name", "Address"),
                    Field.From("Id"),
                    3,
                    new DbField("Id", true, false, false, typeof(int), null, null, null, null),
                    null));
        }

        #endregion

        #region CreateDelete

        [TestMethod]
        public void TestFirebirdStatementBuilderCreateDelete()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

            // Act
            var query = builder.CreateDelete("Table",
                QueryGroup.Parse(new { Id = 1 }));
            var expected = "DELETE FROM \"Table\" WHERE (\"Id\" = @Id)";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestFirebirdStatementBuilderCreateDeleteAll()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

            // Act
            var query = builder.CreateDeleteAll("Table");
            var expected = "DELETE FROM \"Table\"";

            // Assert
            Assert.AreEqual(expected, query);
        }

        #endregion

        #region CreateTruncate

        [TestMethod]
        public void TestFirebirdStatementBuilderCreateTruncate()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

            // Act - Firebird has no TRUNCATE TABLE statement (as of 5.0); DELETE FROM without a WHERE
            // clause is the closest equivalent.
            var query = builder.CreateTruncate("Table");
            var expected = "DELETE FROM \"Table\"";

            // Assert
            Assert.AreEqual(expected, query);
        }

        #endregion
    }
}
