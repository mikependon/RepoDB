using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vertica.Data.VerticaClient;
using RepoDb.Enumerations;
using RepoDb.Exceptions;

namespace RepoDb.Vertica.UnitTests
{
    [TestClass]
    public class StatementBuilderTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseVertica();
        }

        #region CreateBatchQuery

        [TestMethod]
        public void TestVerticaStatementBuilderCreateBatchQuery()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

            // Act
            var query = builder.CreateBatchQuery("Table",
                Field.From("Id", "Name"),
                0,
                10,
                OrderField.Parse(new { Id = Order.Ascending }));
            var expected = "SELECT \"Id\", \"Name\" FROM \"Table\" ORDER BY \"Id\" ASC LIMIT 10";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestVerticaStatementBuilderCreateBatchQueryWithPage()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

            // Act
            var query = builder.CreateBatchQuery("Table",
                Field.From("Id", "Name"),
                3,
                10,
                OrderField.Parse(new { Id = Order.Ascending }));
            var expected = "SELECT \"Id\", \"Name\" FROM \"Table\" ORDER BY \"Id\" ASC LIMIT 10 OFFSET 30";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnVerticaStatementBuilderCreateBatchQueryIfThereAreNoFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

            // Act
            Assert.Throws<NullReferenceException>(() =>
                builder.CreateBatchQuery("Table",
                    null,
                    0,
                    10,
                    OrderField.Parse(new { Id = Order.Ascending })));
        }

        [TestMethod]
        public void ThrowExceptionOnVerticaStatementBuilderCreateBatchQueryIfThereAreNoOrderFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

            // Act
            Assert.Throws<EmptyException>(() =>
                builder.CreateBatchQuery("Table",
                    Field.From("Id", "Name"),
                    0,
                    10,
                    null));
        }

        [TestMethod]
        public void ThrowExceptionOnVerticaStatementBuilderCreateBatchQueryIfThePageValueIsNullOrOutOfRange()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

            // Act
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                builder.CreateBatchQuery("Table",
                    Field.From("Id", "Name"),
                    -1,
                    10,
                    OrderField.Parse(new { Id = Order.Ascending })));
        }

        [TestMethod]
        public void ThrowExceptionOnVerticaStatementBuilderCreateBatchQueryIfTheRowsPerBatchValueIsNullOrOutOfRange()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

            // Act
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                builder.CreateBatchQuery("Table",
                    Field.From("Id", "Name"),
                    0,
                    -1,
                    OrderField.Parse(new { Id = Order.Ascending })));
        }

        [TestMethod]
        public void ThrowExceptionOnVerticaStatementBuilderCreateBatchQueryIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

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
        public void TestVerticaStatementBuilderCreateCount()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

            // Act
            var query = builder.CreateCount("Table",
                null,
                null);
            var expected = "SELECT COUNT (*) AS \"CountValue\" FROM \"Table\"";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestVerticaStatementBuilderCreateCountWithExpression()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

            // Act
            var query = builder.CreateCount("Table",
                QueryGroup.Parse(new { Id = 1 }),
                null);
            var expected = "SELECT COUNT (*) AS \"CountValue\" FROM \"Table\" WHERE (\"Id\" = @Id)";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnVerticaStatementBuilderCreateCountIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateCount("Table",
                    QueryGroup.Parse(new { Id = 1 }),
                    "WhatEver"));
        }

        #endregion

        #region CreateCountAll

        [TestMethod]
        public void TestVerticaStatementBuilderCreateCountAll()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

            // Act
            var query = builder.CreateCountAll("Table",
                null);
            var expected = "SELECT COUNT (*) AS \"CountValue\" FROM \"Table\"";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnVerticaStatementBuilderCreateCountAllIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateCountAll("Table",
                    "WhatEver"));
        }

        #endregion

        #region CreateExists

        [TestMethod]
        public void TestVerticaStatementBuilderCreateExists()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

            // Act
            var query = builder.CreateExists("Table",
                QueryGroup.Parse(new { Id = 1 }));
            var expected = "SELECT 1 AS \"ExistsValue\" FROM \"Table\" WHERE (\"Id\" = @Id) LIMIT 1";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnVerticaStatementBuilderCreateExistsIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateExists("Table",
                    QueryGroup.Parse(new { Id = 1 }),
                    "WhatEver"));
        }

        #endregion

        #region CreateInsert

        [TestMethod]
        public void TestVerticaStatementBuilderCreateInsertWithNoKeyColumn()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

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
        public void TestVerticaStatementBuilderCreateInsertWithPrimary()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

            // Act
            var query = builder.CreateInsert("Table",
                Field.From("Id", "Name", "Address"),
                new DbField("Id", true, false, false, typeof(int), null, null, null, null),
                null);
            var expected = "INSERT INTO \"Table\" ( \"Id\", \"Name\", \"Address\" ) VALUES ( @Id, @Name, @Address )";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestVerticaStatementBuilderCreateInsertWithIdentity()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

            // Act
            var query = builder.CreateInsert("Table",
                Field.From("Id", "Name", "Address"),
                null,
                new DbField("Id", false, true, false, typeof(int), null, null, null, null));
            var expected = "INSERT INTO \"Table\" ( \"Name\", \"Address\" ) VALUES ( @Name, @Address )";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnVerticaStatementBuilderCreateInsertIfThereAreNoFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

            // Act
            Assert.Throws<EmptyException>(() =>
                builder.CreateInsert("Table",
                    null,
                    null,
                    null));
        }

        [TestMethod]
        public void ThrowExceptionOnVerticaStatementBuilderCreateInsertIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

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
        public void TestVerticaStatementBuilderCreateInsertAllWithIdentity()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

            // Act
            var query = builder.CreateInsertAll("Table",
                Field.From("Id", "Name", "Address"),
                1,
                null,
                new DbField("Id", false, true, false, typeof(int), null, null, null, null));
            var expected = "INSERT INTO \"Table\" ( \"Name\", \"Address\" ) VALUES ( @Name, @Address )";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnVerticaStatementBuilderCreateInsertAllIfBatchSizeIsGreaterThanOne()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

            // Act - VerticaDbSetting.IsMultiStatementExecutable is false, so a batchSize greater than
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
        public void ThrowExceptionOnVerticaStatementBuilderCreateInsertAllIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

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
        public void TestVerticaStatementBuilderCreateMax()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

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
        public void TestVerticaStatementBuilderCreateMaxWithExpression()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

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
        public void ThrowExceptionOnVerticaStatementBuilderCreateMaxIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

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
        public void TestVerticaStatementBuilderCreateMaxAll()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

            // Act
            var query = builder.CreateMaxAll("Table",
                new Field("Field"),
                null);
            var expected = "SELECT MAX (\"Field\") AS \"MaxValue\" FROM \"Table\"";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnVerticaStatementBuilderCreateMaxAllIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateMaxAll("Table",
                    new Field("Field"),
                    "WhatEver"));
        }

        #endregion

        #region CreateMin

        [TestMethod]
        public void TestVerticaStatementBuilderCreateMin()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

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
        public void TestVerticaStatementBuilderCreateMinWithExpression()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

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
        public void ThrowExceptionOnVerticaStatementBuilderCreateMinIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

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
        public void TestVerticaStatementBuilderCreateMinAll()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

            // Act
            var query = builder.CreateMinAll("Table",
                new Field("Field"),
                null);
            var expected = "SELECT MIN (\"Field\") AS \"MinValue\" FROM \"Table\"";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnVerticaStatementBuilderCreateMinAllIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateMinAll("Table",
                    new Field("Field"),
                    "WhatEver"));
        }

        #endregion

        #region CreateSum

        [TestMethod]
        public void TestVerticaStatementBuilderCreateSum()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

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
        public void TestVerticaStatementBuilderCreateSumWithExpression()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

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
        public void ThrowExceptionOnVerticaStatementBuilderCreateSumIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

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
        public void TestVerticaStatementBuilderCreateSumAll()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

            // Act
            var query = builder.CreateSumAll("Table",
                new Field("Field"),
                null);
            var expected = "SELECT SUM (\"Field\") AS \"SumValue\" FROM \"Table\"";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnVerticaStatementBuilderCreateSumAllIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateSumAll("Table",
                    new Field("Field"),
                    "WhatEver"));
        }

        #endregion

        #region CreateAverage

        [TestMethod]
        public void TestVerticaStatementBuilderCreateAverage()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

            // Act - the averaged field is CAST to DOUBLE PRECISION because Vertica's AVG() otherwise
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
        public void TestVerticaStatementBuilderCreateAverageWithoutFieldTypeDefaultsToDouble()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

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
        public void TestVerticaStatementBuilderCreateAverageWithDecimalFieldTypeIsNotWidened()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

            // Act - only exact-numeric integer types (short/int/long and their unsigned counterparts)
            // are widened to double by ClientTypeToAverageableClientTypeResolver; a decimal field is
            // passed through as-is and CAST to Vertica's DECIMAL(18,2).
            var query = builder.CreateAverage("Table",
                new Field("Field", typeof(decimal)),
                null,
                null);
            var expected = "SELECT AVG (CAST(\"Field\" AS DECIMAL(18,2))) AS \"AverageValue\" FROM \"Table\"";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnVerticaStatementBuilderCreateAverageIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

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
        public void TestVerticaStatementBuilderCreateMerge()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

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
        public void TestVerticaStatementBuilderCreateMergeWithPrimaryAsQualifier()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

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
        public void TestVerticaStatementBuilderCreateMergeWithIdentity()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

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
        public void TestVerticaStatementBuilderCreateMergeWithCustomQualifiers()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

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
        public void ThrowExceptionOnVerticaStatementBuilderCreateMergeIfThereIsNoPrimary()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

            // Act
            Assert.Throws<PrimaryFieldNotFoundException>(() =>
                builder.CreateMerge("Table",
                    Field.From("Id", "Name", "Address"),
                    null,
                    null,
                    null));
        }

        [TestMethod]
        public void ThrowExceptionOnVerticaStatementBuilderCreateMergeIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

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
        public void TestVerticaStatementBuilderCreateMergeAll()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

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
        public void ThrowExceptionOnVerticaStatementBuilderCreateMergeAllIfBatchSizeIsGreaterThanOne()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

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
        public void ThrowExceptionOnVerticaStatementBuilderCreateMergeAllIfThereIsNoPrimary()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

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
        public void ThrowExceptionOnVerticaStatementBuilderCreateMergeAllIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

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
        public void TestVerticaStatementBuilderCreateQuery()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

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
        public void TestVerticaStatementBuilderCreateQueryWithExpression()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

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
        public void TestVerticaStatementBuilderCreateQueryWithTop()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

            // Act
            var query = builder.CreateQuery("Table",
                Field.From("Id", "Name", "Address"),
                null,
                null,
                10,
                null);
            var expected = "SELECT \"Id\", \"Name\", \"Address\" FROM \"Table\" LIMIT 10";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestVerticaStatementBuilderCreateQueryOrderBy()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

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
        public void TestVerticaStatementBuilderCreateQueryOrderByFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

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
        public void TestVerticaStatementBuilderCreateQueryOrderByDescending()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

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
        public void ThrowExceptionOnVerticaStatementBuilderCreateQueryIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

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
        public void TestVerticaStatementBuilderCreateSkipQuery()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

            // Act
            var query = builder.CreateSkipQuery("Table",
                Field.From("Id", "Name"),
                0,
                10,
                OrderField.Parse(new { Id = Order.Ascending }));
            var expected = "SELECT \"Id\", \"Name\" FROM \"Table\" ORDER BY \"Id\" ASC LIMIT 10";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestVerticaStatementBuilderCreateSkipQueryWithSkip()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

            // Act
            var query = builder.CreateSkipQuery("Table",
                Field.From("Id", "Name"),
                30,
                10,
                OrderField.Parse(new { Id = Order.Ascending }));
            var expected = "SELECT \"Id\", \"Name\" FROM \"Table\" ORDER BY \"Id\" ASC LIMIT 10 OFFSET 30";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnVerticaStatementBuilderCreateSkipQueryIfThereAreNoFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

            // Act
            Assert.Throws<NullReferenceException>(() =>
                builder.CreateSkipQuery("Table",
                    null,
                    0,
                    10,
                    OrderField.Parse(new { Id = Order.Ascending })));
        }

        [TestMethod]
        public void ThrowExceptionOnVerticaStatementBuilderCreateSkipQueryIfThereAreNoOrderFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

            // Act
            Assert.Throws<EmptyException>(() =>
                builder.CreateSkipQuery("Table",
                    Field.From("Id", "Name"),
                    0,
                    10,
                    null));
        }

        [TestMethod]
        public void ThrowExceptionOnVerticaStatementBuilderCreateSkipQueryIfTheSkipValueIsNullOrOutOfRange()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

            // Act
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                builder.CreateSkipQuery("Table",
                    Field.From("Id", "Name"),
                    -1,
                    10,
                    OrderField.Parse(new { Id = Order.Ascending })));
        }

        [TestMethod]
        public void ThrowExceptionOnVerticaStatementBuilderCreateSkipQueryIfTheTakeValueIsNullOrOutOfRange()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

            // Act
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                builder.CreateSkipQuery("Table",
                    Field.From("Id", "Name"),
                    0,
                    -1,
                    OrderField.Parse(new { Id = Order.Ascending })));
        }

        [TestMethod]
        public void ThrowExceptionOnVerticaStatementBuilderCreateSkipQueryIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

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
        public void TestVerticaStatementBuilderCreateUpdate()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

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
        public void ThrowExceptionOnVerticaStatementBuilderCreateUpdateAllIfBatchSizeIsGreaterThanOne()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

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
        public void TestVerticaStatementBuilderCreateDelete()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

            // Act
            var query = builder.CreateDelete("Table",
                QueryGroup.Parse(new { Id = 1 }));
            var expected = "DELETE FROM \"Table\" WHERE (\"Id\" = @Id)";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestVerticaStatementBuilderCreateDeleteAll()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

            // Act
            var query = builder.CreateDeleteAll("Table");
            var expected = "DELETE FROM \"Table\"";

            // Assert
            Assert.AreEqual(expected, query);
        }

        #endregion

        #region CreateTruncate

        [TestMethod]
        public void TestVerticaStatementBuilderCreateTruncate()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

            // Act - Vertica has no TRUNCATE TABLE statement (as of 5.0); DELETE FROM without a WHERE
            // clause is the closest equivalent.
            var query = builder.CreateTruncate("Table");
            var expected = "DELETE FROM \"Table\"";

            // Assert
            Assert.AreEqual(expected, query);
        }

        #endregion
    }
}
