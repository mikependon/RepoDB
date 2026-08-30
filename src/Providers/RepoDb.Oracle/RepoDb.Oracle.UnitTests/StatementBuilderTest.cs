#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oracle.ManagedDataAccess.Client;
using RepoDb.Enumerations;
using RepoDb.Exceptions;
using System;

namespace RepoDb.Oracle.UnitTests
{
    [TestClass]
    public class StatementBuilderTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseOracle();
        }

        #region CreateAverage

        [TestMethod]
        public void TestOracleStatementBuilderCreateAverage()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var field = new Field("Field1");

            // Act
            var actual = statementBuilder.CreateAverage(tableName: tableName, field: field, where: null);
            var expected = "SELECT AVG (CAST(\"Field1\" AS BINARY_DOUBLE)) AS \"AverageValue\" FROM \"Table\"";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateAverageWithWhereExpression()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var field = new Field("Field1");
            var where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = statementBuilder.CreateAverage(tableName: tableName, field: field, where: where);
            var expected = $"" +
                $"SELECT AVG (CAST(\"Field1\" AS BINARY_DOUBLE)) AS \"AverageValue\" " +
                $"FROM \"Table\" " +
                $"WHERE (\"Id\" = :Id)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateAverageWithQuotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "\"SCHEMA\".\"Table\"";
            var field = new Field("Field1");

            // Act
            var actual = statementBuilder.CreateAverage(tableName: tableName, field: field, where: null);
            var expected = "SELECT AVG (CAST(\"Field1\" AS BINARY_DOUBLE)) AS \"AverageValue\" FROM \"SCHEMA\".\"Table\"";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateAverageWithHintsThrowsSinceOracleDoesNotSupportTableHints()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var field = new Field("Field1");

            // Act
            Assert.Throws<NotSupportedException>(() =>
                statementBuilder.CreateAverage(tableName: tableName, field: field, hints: "NOLOCK"));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateAverageIfTheFieldIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateAverage(tableName: tableName, field: null));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateAverageIfTheTableIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var field = new Field("Field1");

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateAverage(tableName: null, field: field));
        }

        #endregion

        #region CreateAverageAll

        [TestMethod]
        public void TestOracleStatementBuilderCreateAverageAll()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var field = new Field("Field1");

            // Act
            var actual = statementBuilder.CreateAverageAll(tableName: tableName, field: field);
            var expected = "SELECT AVG (CAST(\"Field1\" AS BINARY_DOUBLE)) AS \"AverageValue\" FROM \"Table\"";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateAverageAllWithHintsThrowsSinceOracleDoesNotSupportTableHints()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var field = new Field("Field1");

            // Act
            Assert.Throws<NotSupportedException>(() =>
                statementBuilder.CreateAverageAll(tableName: tableName, field: field, hints: "NOLOCK"));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateAverageAllIfTheFieldIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateAverageAll(tableName: tableName, field: null));
        }

        #endregion

        #region CreateBatchQuery

        [TestMethod]
        public void TestOracleStatementBuilderCreateBatchQueryFirstBatch()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2" });
            var orderBy = OrderField.Parse(new { Field1 = Order.Ascending });

            // Act
            var actual = statementBuilder.CreateBatchQuery(tableName: tableName,
                fields: fields,
                page: 0,
                rowsPerBatch: 10,
                orderBy: orderBy,
                where: null);
            var expected = $"" +
                $"SELECT \"Field1\", \"Field2\" " +
                $"FROM \"Table\" " +
                $"ORDER BY \"Field1\" ASC " +
                $"OFFSET 0 " +
                $"ROWS FETCH NEXT 10 ROWS ONLY";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateBatchQuerySecondBatch()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2" });
            var orderBy = OrderField.Parse(new { Field1 = Order.Ascending });

            // Act
            var actual = statementBuilder.CreateBatchQuery(tableName: tableName,
                fields: fields,
                page: 1,
                rowsPerBatch: 10,
                orderBy: orderBy,
                where: null);
            var expected = $"" +
                $"SELECT \"Field1\", \"Field2\" " +
                $"FROM \"Table\" " +
                $"ORDER BY \"Field1\" ASC " +
                $"OFFSET 10 " +
                $"ROWS FETCH NEXT 10 ROWS ONLY";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateBatchQueryIfThereAreNoOrderFields()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2" });

            // Act
            Assert.Throws<EmptyException>(() =>
                statementBuilder.CreateBatchQuery(tableName: tableName,
                    fields: fields,
                    page: 0,
                    rowsPerBatch: 10,
                    orderBy: null,
                    where: null));
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateBatchQueryWithWhereExpression()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2" });
            var where = new QueryGroup(new QueryField("Field1", Operation.NotEqual, 1));
            var orderBy = OrderField.Parse(new { Field1 = Order.Ascending });

            // Act
            var actual = statementBuilder.CreateBatchQuery(tableName: tableName,
                fields: fields,
                page: 1,
                rowsPerBatch: 10,
                orderBy: orderBy,
                where: where);
            var expected = $"" +
                $"SELECT \"Field1\", \"Field2\" " +
                $"FROM \"Table\" " +
                $"WHERE (\"Field1\" <> :Field1) " +
                $"ORDER BY \"Field1\" ASC " +
                $"OFFSET 10 " +
                $"ROWS FETCH NEXT 10 ROWS ONLY";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateBatchQueryWithQuotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "\"SCHEMA\".\"Table\"";
            var fields = Field.From(new[] { "Field1", "Field2" });
            var orderBy = OrderField.Parse(new { Field1 = Order.Ascending });

            // Act
            var actual = statementBuilder.CreateBatchQuery(tableName: tableName,
                fields: fields,
                page: 0,
                rowsPerBatch: 10,
                orderBy: orderBy,
                where: null);
            var expected = $"" +
                $"SELECT \"Field1\", \"Field2\" " +
                $"FROM \"SCHEMA\".\"Table\" " +
                $"ORDER BY \"Field1\" ASC " +
                $"OFFSET 0 " +
                $"ROWS FETCH NEXT 10 ROWS ONLY";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateBatchQueryWithUnquotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "SCHEMA.Table";
            var fields = Field.From(new[] { "Field1", "Field2" });
            var orderBy = OrderField.Parse(new { Field1 = Order.Ascending });

            // Act
            var actual = statementBuilder.CreateBatchQuery(tableName: tableName,
                fields: fields,
                page: 0,
                rowsPerBatch: 10,
                orderBy: orderBy,
                where: null);
            var expected = $"" +
                $"SELECT \"Field1\", \"Field2\" " +
                $"FROM \"SCHEMA\".\"Table\" " +
                $"ORDER BY \"Field1\" ASC " +
                $"OFFSET 0 " +
                $"ROWS FETCH NEXT 10 ROWS ONLY";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateBatchQueryWithHintsThrowsSinceOracleDoesNotSupportTableHints()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2" });
            var orderBy = OrderField.Parse(new { Field1 = Order.Ascending });

            // Act
            Assert.Throws<NotSupportedException>(() =>
                statementBuilder.CreateBatchQuery(tableName: tableName,
                    fields: fields,
                    page: 0,
                    rowsPerBatch: 10,
                    orderBy: orderBy,
                    where: null,
                    hints: "NOLOCK"));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateBatchQueryIfTheTableIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var fields = Field.From(new[] { "Field1", "Field2" });
            var orderBy = OrderField.Parse(new { Field1 = Order.Ascending });

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateBatchQuery(tableName: null,
                    fields: fields,
                    page: 0,
                    rowsPerBatch: 10,
                    orderBy: orderBy,
                    where: null));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateBatchQueryIfTheTableIsEmpty()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var fields = Field.From(new[] { "Field1", "Field2" });
            var orderBy = OrderField.Parse(new { Field1 = Order.Ascending });

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateBatchQuery(tableName: "",
                    fields: fields,
                    page: 0,
                    rowsPerBatch: 10,
                    orderBy: orderBy,
                    where: null));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateBatchQueryIfTheTableIsWhitespace()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var fields = Field.From(new[] { "Field1", "Field2" });
            var orderBy = OrderField.Parse(new { Field1 = Order.Ascending });

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateBatchQuery(tableName: " ",
                    fields: fields,
                    page: 0,
                    rowsPerBatch: 10,
                    orderBy: orderBy,
                    where: null));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateBatchQueryIfTheFieldsAreNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var orderBy = OrderField.Parse(new { Field1 = Order.Ascending });

            // Act
            Assert.Throws<MissingFieldsException>(() =>
                statementBuilder.CreateBatchQuery(tableName: tableName,
                    fields: null,
                    page: 0,
                    rowsPerBatch: 10,
                    orderBy: orderBy,
                    where: null));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateBatchQueryIfThePageIsLessThanZero()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2" });
            var orderBy = OrderField.Parse(new { Field1 = Order.Ascending });

            // Act
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                statementBuilder.CreateBatchQuery(tableName: tableName,
                    fields: fields,
                    page: -1,
                    rowsPerBatch: 10,
                    orderBy: orderBy,
                    where: null));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateBatchQueryIfTheRowsPerBatchIsLessThanOne()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2" });
            var orderBy = OrderField.Parse(new { Field1 = Order.Ascending });

            // Act
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                statementBuilder.CreateBatchQuery(tableName: tableName,
                    fields: fields,
                    page: 0,
                    rowsPerBatch: 0,
                    orderBy: orderBy,
                    where: null));
        }

        #endregion

        #region CreateCount

        [TestMethod]
        public void TestOracleStatementBuilderCreateCount()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";

            // Act - Oracle has no BIGINT-forcing "COUNT_BIG" equivalent need (NUMBER already handles
            // large counts natively), so this uses the generic ANSI "COUNT (*)" from BaseStatementBuilder
            // unlike SqlServerStatementBuilder, which overrides this to use COUNT_BIG.
            var actual = statementBuilder.CreateCount(tableName: tableName, where: null);
            var expected = "SELECT COUNT (*) AS \"CountValue\" FROM \"Table\"";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateCountWithWhereExpression()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = statementBuilder.CreateCount(tableName: tableName, where: where);
            var expected = $"" +
                $"SELECT COUNT (*) AS \"CountValue\" " +
                $"FROM \"Table\" " +
                $"WHERE (\"Id\" = :Id)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateCountWithQuotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "\"SCHEMA\".\"Table\"";

            // Act
            var actual = statementBuilder.CreateCount(tableName: tableName, where: null);
            var expected = "SELECT COUNT (*) AS \"CountValue\" FROM \"SCHEMA\".\"Table\"";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateCountWithUnquotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "SCHEMA.Table";

            // Act
            var actual = statementBuilder.CreateCount(tableName: tableName, where: null);
            var expected = "SELECT COUNT (*) AS \"CountValue\" FROM \"SCHEMA\".\"Table\"";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateCountWithHintsThrowsSinceOracleDoesNotSupportTableHints()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";

            // Act
            Assert.Throws<NotSupportedException>(() =>
                statementBuilder.CreateCount(tableName: tableName, hints: "NOLOCK"));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateCountIfTheTableIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateCount(tableName: null));
        }

        #endregion

        #region CreateCountAll

        [TestMethod]
        public void TestOracleStatementBuilderCreateCountAll()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";

            // Act
            var actual = statementBuilder.CreateCountAll(tableName: tableName, hints: null);
            var expected = "SELECT COUNT (*) AS \"CountValue\" FROM \"Table\"";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateCountAllWithQuotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "\"SCHEMA\".\"Table\"";

            // Act
            var actual = statementBuilder.CreateCountAll(tableName: tableName, hints: null);
            var expected = "SELECT COUNT (*) AS \"CountValue\" FROM \"SCHEMA\".\"Table\"";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateCountAllWithUnquotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "SCHEMA.Table";

            // Act
            var actual = statementBuilder.CreateCountAll(tableName: tableName, hints: null);
            var expected = "SELECT COUNT (*) AS \"CountValue\" FROM \"SCHEMA\".\"Table\"";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateCountAllWithHintsThrowsSinceOracleDoesNotSupportTableHints()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";

            // Act
            Assert.Throws<NotSupportedException>(() =>
                statementBuilder.CreateCountAll(tableName: tableName, hints: "NOLOCK"));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateCountAllIfTheTableIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateCountAll(tableName: null));
        }

        #endregion

        #region CreateDelete

        [TestMethod]
        public void TestOracleStatementBuilderCreateDelete()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";

            // Act
            var actual = statementBuilder.CreateDelete(tableName: tableName, where: null);
            var expected = "DELETE FROM \"Table\"";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateDeleteWithWhereExpression()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = statementBuilder.CreateDelete(tableName: tableName, where: where);
            var expected = $"" +
                $"DELETE FROM \"Table\" " +
                $"WHERE (\"Id\" = :Id)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateDeleteWithQuotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "\"SCHEMA\".\"Table\"";

            // Act
            var actual = statementBuilder.CreateDelete(tableName: tableName, where: null);
            var expected = "DELETE FROM \"SCHEMA\".\"Table\"";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateDeleteWithUnquotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "SCHEMA.Table";

            // Act
            var actual = statementBuilder.CreateDelete(tableName: tableName, where: null);
            var expected = "DELETE FROM \"SCHEMA\".\"Table\"";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateDeleteWithHintsThrowsSinceOracleDoesNotSupportTableHints()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";

            // Act
            Assert.Throws<NotSupportedException>(() =>
                statementBuilder.CreateDelete(tableName: tableName, hints: "NOLOCK"));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateDeleteIfTheTableIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateDelete(tableName: null));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateDeleteIfTheTableIsEmpty()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateDelete(tableName: ""));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateDeleteIfTheTableIsWhitespace()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateDelete(tableName: " "));
        }

        #endregion

        #region CreateDeleteAll

        [TestMethod]
        public void TestOracleStatementBuilderCreateDeleteAll()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";

            // Act
            var actual = statementBuilder.CreateDeleteAll(tableName: tableName);
            var expected = "DELETE FROM \"Table\"";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateDeleteAllWithQuotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "\"SCHEMA\".\"Table\"";

            // Act
            var actual = statementBuilder.CreateDeleteAll(tableName: tableName);
            var expected = "DELETE FROM \"SCHEMA\".\"Table\"";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateDeleteAllWithUnquotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "SCHEMA.Table";

            // Act
            var actual = statementBuilder.CreateDeleteAll(tableName: tableName);
            var expected = "DELETE FROM \"SCHEMA\".\"Table\"";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateDeleteAllWithHintsThrowsSinceOracleDoesNotSupportTableHints()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";

            // Act
            Assert.Throws<NotSupportedException>(() =>
                statementBuilder.CreateDeleteAll(tableName: tableName, hints: "NOLOCK"));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateDeleteAllIfTheTableIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateDeleteAll(tableName: null));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateDeleteAllIfTheTableIsEmpty()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateDeleteAll(tableName: ""));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateDeleteAllIfTheTableIsWhitespace()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateDeleteAll(tableName: " "));
        }

        #endregion

        #region CreateExists

        [TestMethod]
        public void TestOracleStatementBuilderCreateExists()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = statementBuilder.CreateExists(tableName: tableName, where: where);
            var expected = $"" +
                $"SELECT 1 AS \"ExistsValue\" " +
                $"FROM \"Table\" " +
                $"WHERE (\"Id\" = :Id) " +
                $"FETCH FIRST 1 ROWS ONLY";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateExistsWithoutWhere()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";

            // Act
            var actual = statementBuilder.CreateExists(tableName: tableName, where: null);
            var expected = $"" +
                $"SELECT 1 AS \"ExistsValue\" " +
                $"FROM \"Table\" " +
                $"FETCH FIRST 1 ROWS ONLY";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateExistsWithQuotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "\"SCHEMA\".\"Table\"";

            // Act
            var actual = statementBuilder.CreateExists(tableName: tableName, where: null);
            var expected = $"" +
                $"SELECT 1 AS \"ExistsValue\" " +
                $"FROM \"SCHEMA\".\"Table\" " +
                $"FETCH FIRST 1 ROWS ONLY";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateExistsWithUnquotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "SCHEMA.Table";

            // Act
            var actual = statementBuilder.CreateExists(tableName: tableName, where: null);
            var expected = $"" +
                $"SELECT 1 AS \"ExistsValue\" " +
                $"FROM \"SCHEMA\".\"Table\" " +
                $"FETCH FIRST 1 ROWS ONLY";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateExistsWithHintsThrowsSinceOracleDoesNotSupportTableHints()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";

            // Act
            Assert.Throws<NotSupportedException>(() =>
                statementBuilder.CreateExists(tableName: tableName, hints: "NOLOCK"));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateExistsIfTheTableIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateExists(tableName: null));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateExistsIfTheTableIsEmpty()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateExists(tableName: ""));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateExistsIfTheTableIsWhitespace()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateExists(tableName: " "));
        }

        #endregion

        #region CreateInsert

        [TestMethod]
        public void TestOracleStatementBuilderCreateInsertWithNoKeyColumn()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });

            // Act
            var actual = statementBuilder.CreateInsert(tableName: tableName,
                fields: fields,
                primaryField: null,
                identityField: null);
            var expected = $"" +
                $"INSERT INTO \"Table\" " +
                $"( \"Field1\", \"Field2\", \"Field3\" ) " +
                $"VALUES " +
                $"( :Field1, :Field2, :Field3 )";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateInsertWithIdentity()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var identityField = new DbField("Field1", false, true, false, typeof(int), null, null, null, null);

            // Act
            var actual = statementBuilder.CreateInsert(tableName: tableName,
                fields: fields,
                primaryField: null,
                identityField: identityField);
            var expected = $"" +
                $"DECLARE l_repodb_result \"Table\".\"Field1\"%TYPE; " +
                $"l_repodb_cursor SYS_REFCURSOR; " +
                $"BEGIN " +
                $"INSERT INTO \"Table\" ( \"Field2\", \"Field3\" ) VALUES ( :Field2, :Field3 ) " +
                $"RETURNING \"Field1\" INTO l_repodb_result; " +
                $"OPEN l_repodb_cursor FOR SELECT l_repodb_result AS \"Result\" FROM DUAL; " +
                $"DBMS_SQL.RETURN_RESULT(l_repodb_cursor); " +
                $"END;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateInsertWithPrimary()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var primaryField = new DbField("Field1", true, false, false, typeof(int), null, null, null, null);

            // Act - a plain (non-identity) primary is included in the INSERT column list (only an
            // identity column is excluded), and since there's no identityField, GetReturnKeyColumnAsDbField
            // (default KeyColumnReturnBehavior.IdentityOrElsePrimary) falls back to the primary as the
            // RETURNING key column.
            var actual = statementBuilder.CreateInsert(tableName: tableName,
                fields: fields,
                primaryField: primaryField,
                identityField: null);
            var expected = $"" +
                $"DECLARE l_repodb_result \"Table\".\"Field1\"%TYPE; " +
                $"l_repodb_cursor SYS_REFCURSOR; " +
                $"BEGIN " +
                $"INSERT INTO \"Table\" ( \"Field1\", \"Field2\", \"Field3\" ) VALUES ( :Field1, :Field2, :Field3 ) " +
                $"RETURNING \"Field1\" INTO l_repodb_result; " +
                $"OPEN l_repodb_cursor FOR SELECT l_repodb_result AS \"Result\" FROM DUAL; " +
                $"DBMS_SQL.RETURN_RESULT(l_repodb_cursor); " +
                $"END;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateInsertWithQuotedTableSchemaAndNoKeyColumn()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "\"SCHEMA\".\"Table\"";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });

            // Act
            var actual = statementBuilder.CreateInsert(tableName: tableName,
                fields: fields,
                primaryField: null,
                identityField: null);
            var expected = $"" +
                $"INSERT INTO \"SCHEMA\".\"Table\" " +
                $"( \"Field1\", \"Field2\", \"Field3\" ) " +
                $"VALUES " +
                $"( :Field1, :Field2, :Field3 )";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateInsertWithUnquotedTableSchemaAndNoKeyColumn()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "SCHEMA.Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });

            // Act
            var actual = statementBuilder.CreateInsert(tableName: tableName,
                fields: fields,
                primaryField: null,
                identityField: null);
            var expected = $"" +
                $"INSERT INTO \"SCHEMA\".\"Table\" " +
                $"( \"Field1\", \"Field2\", \"Field3\" ) " +
                $"VALUES " +
                $"( :Field1, :Field2, :Field3 )";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateInsertWithQuotedTableSchemaAndIdentity()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "\"SCHEMA\".\"Table\"";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var identityField = new DbField("Field1", false, true, false, typeof(int), null, null, null, null);

            // Act
            var actual = statementBuilder.CreateInsert(tableName: tableName,
                fields: fields,
                primaryField: null,
                identityField: identityField);
            var expected = $"" +
                $"DECLARE l_repodb_result \"SCHEMA\".\"Table\".\"Field1\"%TYPE; " +
                $"l_repodb_cursor SYS_REFCURSOR; " +
                $"BEGIN " +
                $"INSERT INTO \"SCHEMA\".\"Table\" ( \"Field2\", \"Field3\" ) VALUES ( :Field2, :Field3 ) " +
                $"RETURNING \"Field1\" INTO l_repodb_result; " +
                $"OPEN l_repodb_cursor FOR SELECT l_repodb_result AS \"Result\" FROM DUAL; " +
                $"DBMS_SQL.RETURN_RESULT(l_repodb_cursor); " +
                $"END;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateInsertWithHintsThrowsSinceOracleDoesNotSupportTableHints()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2" });

            // Act
            Assert.Throws<NotSupportedException>(() =>
                statementBuilder.CreateInsert(tableName: tableName, fields: fields, hints: "NOLOCK"));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateInsertIfTheTableIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var fields = Field.From(new[] { "Field1", "Field2" });

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateInsert(tableName: null, fields: fields));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateInsertIfTheTableIsEmpty()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var fields = Field.From(new[] { "Field1", "Field2" });

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateInsert(tableName: "", fields: fields));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateInsertIfTheTableIsWhitespace()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var fields = Field.From(new[] { "Field1", "Field2" });

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateInsert(tableName: " ", fields: fields));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateInsertIfTheFieldsAreNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";

            // Act
            Assert.Throws<EmptyException>(() =>
                statementBuilder.CreateInsert(tableName: tableName, fields: null));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateInsertIfThePrimaryIsNotReallyAPrimary()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2" });
            var primaryField = new DbField("Field1", false, false, false, typeof(int), null, null, null, null);

            // Act
            Assert.Throws<InvalidOperationException>(() =>
                statementBuilder.CreateInsert(tableName: tableName,
                    fields: fields,
                    primaryField: primaryField,
                    identityField: null));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateInsertIfTheIdentityIsNotReallyAnIdentity()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2" });
            var identityField = new DbField("Field2", false, false, false, typeof(int), null, null, null, null);

            // Act
            Assert.Throws<InvalidOperationException>(() =>
                statementBuilder.CreateInsert(tableName: tableName,
                    fields: fields,
                    primaryField: null,
                    identityField: identityField));
        }

        #endregion

        #region CreateInsertAll

        [TestMethod]
        public void TestOracleStatementBuilderCreateInsertAllWithIdentity()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var identityField = new DbField("Field1", false, true, false, typeof(int), null, null, null, null);

            // Act
            var actual = statementBuilder.CreateInsertAll(tableName: tableName,
                fields: fields,
                batchSize: 1,
                primaryField: null,
                identityField: identityField);
            var expected = $"" +
                $"DECLARE l_repodb_result \"Table\".\"Field1\"%TYPE; " +
                $"l_repodb_cursor SYS_REFCURSOR; " +
                $"BEGIN " +
                $"INSERT INTO \"Table\" ( \"Field2\", \"Field3\" ) VALUES ( :Field2, :Field3 ) " +
                $"RETURNING \"Field1\" INTO l_repodb_result; " +
                $"OPEN l_repodb_cursor FOR SELECT l_repodb_result AS \"Result\" FROM DUAL; " +
                $"DBMS_SQL.RETURN_RESULT(l_repodb_cursor); " +
                $"END;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateInsertAllIfBatchSizeIsGreaterThanOne()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });

            // Act
            Assert.Throws<NotSupportedException>(() =>
                statementBuilder.CreateInsertAll(tableName: tableName,
                    fields: fields,
                    batchSize: 2));
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateInsertAllWithNoKeyColumn()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });

            // Act
            var actual = statementBuilder.CreateInsertAll(tableName: tableName,
                fields: fields,
                batchSize: 1,
                primaryField: null,
                identityField: null);
            var expected = $"" +
                $"INSERT INTO \"Table\" " +
                $"( \"Field1\", \"Field2\", \"Field3\" ) " +
                $"VALUES " +
                $"( :Field1, :Field2, :Field3 )";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateInsertAllWithQuotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "\"SCHEMA\".\"Table\"";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var identityField = new DbField("Field1", false, true, false, typeof(int), null, null, null, null);

            // Act
            var actual = statementBuilder.CreateInsertAll(tableName: tableName,
                fields: fields,
                batchSize: 1,
                primaryField: null,
                identityField: identityField);
            var expected = $"" +
                $"DECLARE l_repodb_result \"SCHEMA\".\"Table\".\"Field1\"%TYPE; " +
                $"l_repodb_cursor SYS_REFCURSOR; " +
                $"BEGIN " +
                $"INSERT INTO \"SCHEMA\".\"Table\" ( \"Field2\", \"Field3\" ) VALUES ( :Field2, :Field3 ) " +
                $"RETURNING \"Field1\" INTO l_repodb_result; " +
                $"OPEN l_repodb_cursor FOR SELECT l_repodb_result AS \"Result\" FROM DUAL; " +
                $"DBMS_SQL.RETURN_RESULT(l_repodb_cursor); " +
                $"END;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateInsertAllWithHintsThrowsSinceOracleDoesNotSupportTableHints()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2" });

            // Act
            Assert.Throws<NotSupportedException>(() =>
                statementBuilder.CreateInsertAll(tableName: tableName,
                    fields: fields,
                    batchSize: 1,
                    hints: "NOLOCK"));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateInsertAllIfTheTableIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var fields = Field.From(new[] { "Field1", "Field2" });

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateInsertAll(tableName: null, fields: fields, batchSize: 1));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateInsertAllIfTheFieldsAreNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";

            // Act
            Assert.Throws<EmptyException>(() =>
                statementBuilder.CreateInsertAll(tableName: tableName, fields: null, batchSize: 1));
        }

        #endregion

        #region CreateMax

        [TestMethod]
        public void TestOracleStatementBuilderCreateMax()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var field = new Field("Field1");

            // Act
            var actual = statementBuilder.CreateMax(tableName: tableName, field: field, where: null);
            var expected = "SELECT MAX (\"Field1\") AS \"MaxValue\" FROM \"Table\"";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateMaxWithWhereExpression()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var field = new Field("Field1");
            var where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = statementBuilder.CreateMax(tableName: tableName, field: field, where: where);
            var expected = $"" +
                $"SELECT MAX (\"Field1\") AS \"MaxValue\" " +
                $"FROM \"Table\" " +
                $"WHERE (\"Id\" = :Id)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateMaxWithHintsThrowsSinceOracleDoesNotSupportTableHints()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var field = new Field("Field1");

            // Act
            Assert.Throws<NotSupportedException>(() =>
                statementBuilder.CreateMax(tableName: tableName, field: field, hints: "NOLOCK"));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateMaxIfTheFieldIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateMax(tableName: tableName, field: null));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateMaxIfTheTableIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var field = new Field("Field1");

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateMax(tableName: null, field: field));
        }

        #endregion

        #region CreateMaxAll

        [TestMethod]
        public void TestOracleStatementBuilderCreateMaxAll()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var field = new Field("Field1");

            // Act
            var actual = statementBuilder.CreateMaxAll(tableName: tableName, field: field);
            var expected = "SELECT MAX (\"Field1\") AS \"MaxValue\" FROM \"Table\"";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateMaxAllIfTheFieldIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateMaxAll(tableName: tableName, field: null));
        }

        #endregion

        #region CreateMerge

        [TestMethod]
        public void TestOracleStatementBuilderCreateMergeWithNoKeyColumn()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From("Field1");

            // Act
            var actual = statementBuilder.CreateMerge(tableName: tableName,
                fields: fields,
                qualifiers: qualifiers,
                primaryField: null,
                identityField: null);
            var expected = $"" +
                $"MERGE INTO \"Table\" T " +
                $"USING ( SELECT :Field1 AS \"Field1\", :Field2 AS \"Field2\", :Field3 AS \"Field3\" FROM DUAL ) " +
                $"S ON ( (S.\"Field1\" = T.\"Field1\" OR (S.\"Field1\" IS NULL AND T.\"Field1\" IS NULL)) ) " +
                $"WHEN MATCHED THEN " +
                $"UPDATE SET T.\"Field2\" = S.\"Field2\", T.\"Field3\" = S.\"Field3\" " +
                $"WHEN NOT MATCHED THEN " +
                $"INSERT ( \"Field1\", \"Field2\", \"Field3\" ) " +
                $"VALUES ( S.\"Field1\", S.\"Field2\", S.\"Field3\" )";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateMergeIfThereAreNoQualifiersAndNoPrimary()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });

            // Act
            Assert.Throws<PrimaryFieldNotFoundException>(() =>
                statementBuilder.CreateMerge(tableName: tableName,
                    fields: fields,
                    qualifiers: null,
                    primaryField: null,
                    identityField: null));
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateMergeWithQuotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "\"SCHEMA\".\"Table\"";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From("Field1");

            // Act
            var actual = statementBuilder.CreateMerge(tableName: tableName,
                fields: fields,
                qualifiers: qualifiers,
                primaryField: null,
                identityField: null);
            var expected = $"" +
                $"MERGE INTO \"SCHEMA\".\"Table\" T " +
                $"USING ( SELECT :Field1 AS \"Field1\", :Field2 AS \"Field2\", :Field3 AS \"Field3\" FROM DUAL ) " +
                $"S ON ( (S.\"Field1\" = T.\"Field1\" OR (S.\"Field1\" IS NULL AND T.\"Field1\" IS NULL)) ) " +
                $"WHEN MATCHED THEN " +
                $"UPDATE SET T.\"Field2\" = S.\"Field2\", T.\"Field3\" = S.\"Field3\" " +
                $"WHEN NOT MATCHED THEN " +
                $"INSERT ( \"Field1\", \"Field2\", \"Field3\" ) " +
                $"VALUES ( S.\"Field1\", S.\"Field2\", S.\"Field3\" )";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateMergeWithUnquotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "SCHEMA.Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From("Field1");

            // Act
            var actual = statementBuilder.CreateMerge(tableName: tableName,
                fields: fields,
                qualifiers: qualifiers,
                primaryField: null,
                identityField: null);
            var expected = $"" +
                $"MERGE INTO \"SCHEMA\".\"Table\" T " +
                $"USING ( SELECT :Field1 AS \"Field1\", :Field2 AS \"Field2\", :Field3 AS \"Field3\" FROM DUAL ) " +
                $"S ON ( (S.\"Field1\" = T.\"Field1\" OR (S.\"Field1\" IS NULL AND T.\"Field1\" IS NULL)) ) " +
                $"WHEN MATCHED THEN " +
                $"UPDATE SET T.\"Field2\" = S.\"Field2\", T.\"Field3\" = S.\"Field3\" " +
                $"WHEN NOT MATCHED THEN " +
                $"INSERT ( \"Field1\", \"Field2\", \"Field3\" ) " +
                $"VALUES ( S.\"Field1\", S.\"Field2\", S.\"Field3\" )";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateMergeWithCoveredPrimary()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From("Field1");
            var primaryField = new DbField("Field1", true, false, false, typeof(int), null, null, null, null);

            // Act
            var actual = statementBuilder.CreateMerge(tableName: tableName,
                fields: fields,
                qualifiers: qualifiers,
                primaryField: primaryField,
                identityField: null);
            var expected = $"" +
                $"DECLARE l_repodb_result \"Table\".\"Field1\"%TYPE; " +
                $"l_repodb_cursor SYS_REFCURSOR; " +
                $"BEGIN " +
                $"MERGE INTO \"Table\" T " +
                $"USING ( SELECT :Field1 AS \"Field1\", :Field2 AS \"Field2\", :Field3 AS \"Field3\" FROM DUAL ) " +
                $"S ON ( (S.\"Field1\" = T.\"Field1\" OR (S.\"Field1\" IS NULL AND T.\"Field1\" IS NULL)) ) " +
                $"WHEN MATCHED THEN " +
                $"UPDATE SET T.\"Field2\" = S.\"Field2\", T.\"Field3\" = S.\"Field3\" " +
                $"WHEN NOT MATCHED THEN " +
                $"INSERT ( \"Field1\", \"Field2\", \"Field3\" ) " +
                $"VALUES ( S.\"Field1\", S.\"Field2\", S.\"Field3\" ) " +
                $"RETURNING \"Field1\" INTO l_repodb_result; " +
                $"OPEN l_repodb_cursor FOR SELECT l_repodb_result AS \"Result\" FROM DUAL; " +
                $"DBMS_SQL.RETURN_RESULT(l_repodb_cursor); " +
                $"END;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateMergeWithUncoveredPrimary()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From("Field1");
            var primaryField = new DbField("Id", true, false, false, typeof(int), null, null, null, null);

            // Act - Oracle's CreateMerge (unlike CreateInsert) never checks that the primary field
            // is actually present among the given "fields"; it's only used to resolve the RETURNING
            // key column.
            var actual = statementBuilder.CreateMerge(tableName: tableName,
                fields: fields,
                qualifiers: qualifiers,
                primaryField: primaryField,
                identityField: null);
            var expected = $"" +
                $"DECLARE l_repodb_result \"Table\".\"Id\"%TYPE; " +
                $"l_repodb_cursor SYS_REFCURSOR; " +
                $"BEGIN " +
                $"MERGE INTO \"Table\" T " +
                $"USING ( SELECT :Field1 AS \"Field1\", :Field2 AS \"Field2\", :Field3 AS \"Field3\" FROM DUAL ) " +
                $"S ON ( (S.\"Field1\" = T.\"Field1\" OR (S.\"Field1\" IS NULL AND T.\"Field1\" IS NULL)) ) " +
                $"WHEN MATCHED THEN " +
                $"UPDATE SET T.\"Field2\" = S.\"Field2\", T.\"Field3\" = S.\"Field3\" " +
                $"WHEN NOT MATCHED THEN " +
                $"INSERT ( \"Field1\", \"Field2\", \"Field3\" ) " +
                $"VALUES ( S.\"Field1\", S.\"Field2\", S.\"Field3\" ) " +
                $"RETURNING \"Id\" INTO l_repodb_result; " +
                $"OPEN l_repodb_cursor FOR SELECT l_repodb_result AS \"Result\" FROM DUAL; " +
                $"DBMS_SQL.RETURN_RESULT(l_repodb_cursor); " +
                $"END;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateMergeWithCoveredIdentity()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From("Field1");
            var identityField = new DbField("Field1", false, true, false, typeof(int), null, null, null, null);

            // Act
            var actual = statementBuilder.CreateMerge(tableName: tableName,
                fields: fields,
                qualifiers: qualifiers,
                primaryField: null,
                identityField: identityField);
            var expected = $"" +
                $"DECLARE l_repodb_result \"Table\".\"Field1\"%TYPE; " +
                $"l_repodb_cursor SYS_REFCURSOR; " +
                $"BEGIN " +
                $"MERGE INTO \"Table\" T " +
                $"USING ( SELECT :Field1 AS \"Field1\", :Field2 AS \"Field2\", :Field3 AS \"Field3\" FROM DUAL ) " +
                $"S ON ( (S.\"Field1\" = T.\"Field1\" OR (S.\"Field1\" IS NULL AND T.\"Field1\" IS NULL)) ) " +
                $"WHEN MATCHED THEN " +
                $"UPDATE SET T.\"Field2\" = S.\"Field2\", T.\"Field3\" = S.\"Field3\" " +
                $"WHEN NOT MATCHED THEN " +
                $"INSERT ( \"Field2\", \"Field3\" ) " +
                $"VALUES ( S.\"Field2\", S.\"Field3\" ) " +
                $"RETURNING \"Field1\" INTO l_repodb_result; " +
                $"OPEN l_repodb_cursor FOR SELECT l_repodb_result AS \"Result\" FROM DUAL; " +
                $"DBMS_SQL.RETURN_RESULT(l_repodb_cursor); " +
                $"END;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateMergeWithUncoveredIdentity()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From("Field1");
            var identityField = new DbField("Id", false, true, false, typeof(int), null, null, null, null);

            // Act
            var actual = statementBuilder.CreateMerge(tableName: tableName,
                fields: fields,
                qualifiers: qualifiers,
                primaryField: null,
                identityField: identityField);
            var expected = $"" +
                $"DECLARE l_repodb_result \"Table\".\"Id\"%TYPE; " +
                $"l_repodb_cursor SYS_REFCURSOR; " +
                $"BEGIN " +
                $"MERGE INTO \"Table\" T " +
                $"USING ( SELECT :Field1 AS \"Field1\", :Field2 AS \"Field2\", :Field3 AS \"Field3\" FROM DUAL ) " +
                $"S ON ( (S.\"Field1\" = T.\"Field1\" OR (S.\"Field1\" IS NULL AND T.\"Field1\" IS NULL)) ) " +
                $"WHEN MATCHED THEN " +
                $"UPDATE SET T.\"Field2\" = S.\"Field2\", T.\"Field3\" = S.\"Field3\" " +
                $"WHEN NOT MATCHED THEN " +
                $"INSERT ( \"Field1\", \"Field2\", \"Field3\" ) " +
                $"VALUES ( S.\"Field1\", S.\"Field2\", S.\"Field3\" ) " +
                $"RETURNING \"Id\" INTO l_repodb_result; " +
                $"OPEN l_repodb_cursor FOR SELECT l_repodb_result AS \"Result\" FROM DUAL; " +
                $"DBMS_SQL.RETURN_RESULT(l_repodb_cursor); " +
                $"END;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateMergeWithHintsThrowsSinceOracleDoesNotSupportTableHints()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From("Field1");

            // Act
            Assert.Throws<NotSupportedException>(() =>
                statementBuilder.CreateMerge(tableName: tableName,
                    fields: fields,
                    qualifiers: qualifiers,
                    hints: "NOLOCK"));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateMergeIfTheTableIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From("Field1");

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateMerge(tableName: null, fields: fields, qualifiers: qualifiers));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateMergeIfTheTableIsEmpty()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From("Field1");

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateMerge(tableName: "", fields: fields, qualifiers: qualifiers));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateMergeIfTheTableIsWhitespace()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From("Field1");

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateMerge(tableName: " ", fields: fields, qualifiers: qualifiers));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateMergeIfThereAreNoFields()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var qualifiers = Field.From("Field1");

            // Act
            Assert.Throws<EmptyException>(() =>
                statementBuilder.CreateMerge(tableName: tableName, fields: null, qualifiers: qualifiers));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateMergeIfThePrimaryIsNotReallyAPrimary()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var primaryField = new DbField("Field1", false, false, false, typeof(int), null, null, null, null);

            // Act
            Assert.Throws<InvalidOperationException>(() =>
                statementBuilder.CreateMerge(tableName: tableName,
                    fields: fields,
                    qualifiers: null,
                    primaryField: primaryField,
                    identityField: null));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateMergeIfTheIdentityIsNotReallyAnIdentity()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From("Field1");
            var identityField = new DbField("Field2", false, false, false, typeof(int), null, null, null, null);

            // Act
            Assert.Throws<InvalidOperationException>(() =>
                statementBuilder.CreateMerge(tableName: tableName,
                    fields: fields,
                    qualifiers: qualifiers,
                    primaryField: null,
                    identityField: identityField));
        }

        #endregion

        #region CreateMergeAll

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateMergeAllIfBatchSizeIsGreaterThanOne()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From("Field1");

            // Act
            Assert.Throws<NotSupportedException>(() =>
                statementBuilder.CreateMergeAll(tableName: tableName,
                    fields: fields,
                    qualifiers: qualifiers,
                    batchSize: 2));
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateMergeAllWithNoKeyColumn()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From("Field1");

            // Act
            var actual = statementBuilder.CreateMergeAll(tableName: tableName,
                fields: fields,
                qualifiers: qualifiers,
                batchSize: 1,
                primaryField: null,
                identityField: null);
            var expected = $"" +
                $"MERGE INTO \"Table\" T " +
                $"USING ( SELECT :Field1 AS \"Field1\", :Field2 AS \"Field2\", :Field3 AS \"Field3\" FROM DUAL ) " +
                $"S ON ( (S.\"Field1\" = T.\"Field1\" OR (S.\"Field1\" IS NULL AND T.\"Field1\" IS NULL)) ) " +
                $"WHEN MATCHED THEN " +
                $"UPDATE SET T.\"Field2\" = S.\"Field2\", T.\"Field3\" = S.\"Field3\" " +
                $"WHEN NOT MATCHED THEN " +
                $"INSERT ( \"Field1\", \"Field2\", \"Field3\" ) " +
                $"VALUES ( S.\"Field1\", S.\"Field2\", S.\"Field3\" )";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateMergeAllWithIdentity()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From("Field1");
            var identityField = new DbField("Field1", false, true, false, typeof(int), null, null, null, null);

            // Act
            var actual = statementBuilder.CreateMergeAll(tableName: tableName,
                fields: fields,
                qualifiers: qualifiers,
                batchSize: 1,
                primaryField: null,
                identityField: identityField);
            var expected = $"" +
                $"DECLARE l_repodb_result \"Table\".\"Field1\"%TYPE; " +
                $"l_repodb_cursor SYS_REFCURSOR; " +
                $"BEGIN " +
                $"MERGE INTO \"Table\" T " +
                $"USING ( SELECT :Field1 AS \"Field1\", :Field2 AS \"Field2\", :Field3 AS \"Field3\" FROM DUAL ) " +
                $"S ON ( (S.\"Field1\" = T.\"Field1\" OR (S.\"Field1\" IS NULL AND T.\"Field1\" IS NULL)) ) " +
                $"WHEN MATCHED THEN " +
                $"UPDATE SET T.\"Field2\" = S.\"Field2\", T.\"Field3\" = S.\"Field3\" " +
                $"WHEN NOT MATCHED THEN " +
                $"INSERT ( \"Field2\", \"Field3\" ) " +
                $"VALUES ( S.\"Field2\", S.\"Field3\" ) " +
                $"RETURNING \"Field1\" INTO l_repodb_result; " +
                $"OPEN l_repodb_cursor FOR SELECT l_repodb_result AS \"Result\" FROM DUAL; " +
                $"DBMS_SQL.RETURN_RESULT(l_repodb_cursor); " +
                $"END;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateMergeAllWithHintsThrowsSinceOracleDoesNotSupportTableHints()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From("Field1");

            // Act
            Assert.Throws<NotSupportedException>(() =>
                statementBuilder.CreateMergeAll(tableName: tableName,
                    fields: fields,
                    qualifiers: qualifiers,
                    batchSize: 1,
                    hints: "NOLOCK"));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateMergeAllIfTheTableIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From("Field1");

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateMergeAll(tableName: null,
                    fields: fields,
                    qualifiers: qualifiers,
                    batchSize: 1));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateMergeAllIfThereAreNoQualifiersAndNoPrimary()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });

            // Act
            Assert.Throws<PrimaryFieldNotFoundException>(() =>
                statementBuilder.CreateMergeAll(tableName: tableName,
                    fields: fields,
                    qualifiers: null,
                    batchSize: 1,
                    primaryField: null,
                    identityField: null));
        }

        #endregion

        #region CreateMin

        [TestMethod]
        public void TestOracleStatementBuilderCreateMin()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var field = new Field("Field1");

            // Act
            var actual = statementBuilder.CreateMin(tableName: tableName, field: field, where: null);
            var expected = "SELECT MIN (\"Field1\") AS \"MinValue\" FROM \"Table\"";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateMinWithWhereExpression()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var field = new Field("Field1");
            var where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = statementBuilder.CreateMin(tableName: tableName, field: field, where: where);
            var expected = $"" +
                $"SELECT MIN (\"Field1\") AS \"MinValue\" " +
                $"FROM \"Table\" " +
                $"WHERE (\"Id\" = :Id)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateMinWithHintsThrowsSinceOracleDoesNotSupportTableHints()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var field = new Field("Field1");

            // Act
            Assert.Throws<NotSupportedException>(() =>
                statementBuilder.CreateMin(tableName: tableName, field: field, hints: "NOLOCK"));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateMinIfTheFieldIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateMin(tableName: tableName, field: null));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateMinIfTheTableIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var field = new Field("Field1");

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateMin(tableName: null, field: field));
        }

        #endregion

        #region CreateMinAll

        [TestMethod]
        public void TestOracleStatementBuilderCreateMinAll()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var field = new Field("Field1");

            // Act
            var actual = statementBuilder.CreateMinAll(tableName: tableName, field: field);
            var expected = "SELECT MIN (\"Field1\") AS \"MinValue\" FROM \"Table\"";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateMinAllIfTheFieldIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateMinAll(tableName: tableName, field: null));
        }

        #endregion

        #region CreateQuery

        [TestMethod]
        public void TestOracleStatementBuilderCreateQuery()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2" });

            // Act
            var actual = statementBuilder.CreateQuery(tableName: tableName, fields: fields);
            var expected = "SELECT \"Field1\", \"Field2\" FROM \"Table\"";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateQueryWithTop()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2" });

            // Act
            var actual = statementBuilder.CreateQuery(tableName: tableName, fields: fields, top: 10);
            var expected = "SELECT \"Field1\", \"Field2\" FROM \"Table\" FETCH FIRST 10 ROWS ONLY";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateQueryWithWhereExpression()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2" });
            var where = new QueryGroup(new QueryField("Field1", Operation.NotEqual, 1));

            // Act
            var actual = statementBuilder.CreateQuery(tableName: tableName, fields: fields, where: where);
            var expected = $"" +
                $"SELECT \"Field1\", \"Field2\" " +
                $"FROM \"Table\" " +
                $"WHERE (\"Field1\" <> :Field1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateQueryWithOrderBy()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2" });
            var orderBy = OrderField.Parse(new { Field1 = Order.Ascending });

            // Act
            var actual = statementBuilder.CreateQuery(tableName: tableName, fields: fields, orderBy: orderBy);
            var expected = $"" +
                $"SELECT \"Field1\", \"Field2\" " +
                $"FROM \"Table\" " +
                $"ORDER BY \"Field1\" ASC";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateQueryWithQuotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "\"SCHEMA\".\"Table\"";
            var fields = Field.From(new[] { "Field1", "Field2" });

            // Act
            var actual = statementBuilder.CreateQuery(tableName: tableName, fields: fields);
            var expected = "SELECT \"Field1\", \"Field2\" FROM \"SCHEMA\".\"Table\"";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateQueryWithUnquotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "SCHEMA.Table";
            var fields = Field.From(new[] { "Field1", "Field2" });

            // Act
            var actual = statementBuilder.CreateQuery(tableName: tableName, fields: fields);
            var expected = "SELECT \"Field1\", \"Field2\" FROM \"SCHEMA\".\"Table\"";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateQueryWithHintsThrowsSinceOracleDoesNotSupportTableHints()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2" });

            // Act
            Assert.Throws<NotSupportedException>(() =>
                statementBuilder.CreateQuery(tableName: tableName, fields: fields, hints: "NOLOCK"));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateQueryIfTheFieldsAreNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateQuery(tableName: tableName, fields: null));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateQueryIfTheTableIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var fields = Field.From(new[] { "Field1", "Field2" });

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateQuery(tableName: null, fields: fields));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateQueryIfTheTableIsEmpty()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var fields = Field.From(new[] { "Field1", "Field2" });

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateQuery(tableName: "", fields: fields));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateQueryIfTheTableIsWhitespace()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var fields = Field.From(new[] { "Field1", "Field2" });

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateQuery(tableName: " ", fields: fields));
        }

        #endregion

        #region CreateSkipQuery

        [TestMethod]
        public void TestOracleStatementBuilderCreateSkipQuery()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2" });
            var orderBy = OrderField.Parse(new { Field1 = Order.Ascending });

            // Act
            var actual = statementBuilder.CreateSkipQuery(tableName: tableName,
                fields: fields,
                skip: 20,
                take: 10,
                orderBy: orderBy,
                where: null);
            var expected = $"" +
                $"SELECT \"Field1\", \"Field2\" " +
                $"FROM \"Table\" " +
                $"ORDER BY \"Field1\" ASC " +
                $"OFFSET 20 " +
                $"ROWS FETCH NEXT 10 ROWS ONLY";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateSkipQueryWithWhereExpression()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2" });
            var where = new QueryGroup(new QueryField("Field1", Operation.NotEqual, 1));
            var orderBy = OrderField.Parse(new { Field1 = Order.Ascending });

            // Act
            var actual = statementBuilder.CreateSkipQuery(tableName: tableName,
                fields: fields,
                skip: 10,
                take: 10,
                orderBy: orderBy,
                where: where);
            var expected = $"" +
                $"SELECT \"Field1\", \"Field2\" " +
                $"FROM \"Table\" " +
                $"WHERE (\"Field1\" <> :Field1) " +
                $"ORDER BY \"Field1\" ASC " +
                $"OFFSET 10 " +
                $"ROWS FETCH NEXT 10 ROWS ONLY";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateSkipQueryWithQuotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "\"SCHEMA\".\"Table\"";
            var fields = Field.From(new[] { "Field1", "Field2" });
            var orderBy = OrderField.Parse(new { Field1 = Order.Ascending });

            // Act
            var actual = statementBuilder.CreateSkipQuery(tableName: tableName,
                fields: fields,
                skip: 0,
                take: 10,
                orderBy: orderBy,
                where: null);
            var expected = $"" +
                $"SELECT \"Field1\", \"Field2\" " +
                $"FROM \"SCHEMA\".\"Table\" " +
                $"ORDER BY \"Field1\" ASC " +
                $"OFFSET 0 " +
                $"ROWS FETCH NEXT 10 ROWS ONLY";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateSkipQueryWithUnquotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "SCHEMA.Table";
            var fields = Field.From(new[] { "Field1", "Field2" });
            var orderBy = OrderField.Parse(new { Field1 = Order.Ascending });

            // Act
            var actual = statementBuilder.CreateSkipQuery(tableName: tableName,
                fields: fields,
                skip: 0,
                take: 10,
                orderBy: orderBy,
                where: null);
            var expected = $"" +
                $"SELECT \"Field1\", \"Field2\" " +
                $"FROM \"SCHEMA\".\"Table\" " +
                $"ORDER BY \"Field1\" ASC " +
                $"OFFSET 0 " +
                $"ROWS FETCH NEXT 10 ROWS ONLY";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateSkipQueryWithHintsThrowsSinceOracleDoesNotSupportTableHints()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2" });
            var orderBy = OrderField.Parse(new { Field1 = Order.Ascending });

            // Act
            Assert.Throws<NotSupportedException>(() =>
                statementBuilder.CreateSkipQuery(tableName: tableName,
                    fields: fields,
                    skip: 0,
                    take: 10,
                    orderBy: orderBy,
                    where: null,
                    hints: "NOLOCK"));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateSkipQueryIfTheTableIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var fields = Field.From(new[] { "Field1", "Field2" });

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateSkipQuery(tableName: null,
                    fields: fields,
                    skip: 0,
                    take: 10,
                    orderBy: null,
                    where: null));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateSkipQueryIfTheTableIsEmpty()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var fields = Field.From(new[] { "Field1", "Field2" });

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateSkipQuery(tableName: "",
                    fields: fields,
                    skip: 0,
                    take: 10,
                    orderBy: null,
                    where: null));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateSkipQueryIfTheTableIsWhitespace()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var fields = Field.From(new[] { "Field1", "Field2" });

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateSkipQuery(tableName: " ",
                    fields: fields,
                    skip: 0,
                    take: 10,
                    orderBy: null,
                    where: null));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateSkipQueryIfTheFieldsAreNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var orderBy = OrderField.Parse(new { Field1 = Order.Ascending });

            // Act
            Assert.Throws<MissingFieldsException>(() =>
                statementBuilder.CreateSkipQuery(tableName: tableName,
                    fields: null,
                    skip: 0,
                    take: 10,
                    orderBy: orderBy,
                    where: null));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateSkipQueryIfThereAreNoOrderFields()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2" });

            // Act
            Assert.Throws<EmptyException>(() =>
                statementBuilder.CreateSkipQuery(tableName: tableName,
                    fields: fields,
                    skip: 0,
                    take: 10,
                    orderBy: null,
                    where: null));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateSkipQueryIfTheSkipIsLessThanZero()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2" });
            var orderBy = OrderField.Parse(new { Field1 = Order.Ascending });

            // Act
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                statementBuilder.CreateSkipQuery(tableName: tableName,
                    fields: fields,
                    skip: -1,
                    take: 10,
                    orderBy: orderBy,
                    where: null));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateSkipQueryIfTheTakeIsLessThanOne()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2" });
            var orderBy = OrderField.Parse(new { Field1 = Order.Ascending });

            // Act
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                statementBuilder.CreateSkipQuery(tableName: tableName,
                    fields: fields,
                    skip: 0,
                    take: 0,
                    orderBy: orderBy,
                    where: null));
        }

        #endregion

        #region CreateSum

        [TestMethod]
        public void TestOracleStatementBuilderCreateSum()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var field = new Field("Field1");

            // Act
            var actual = statementBuilder.CreateSum(tableName: tableName, field: field, where: null);
            var expected = "SELECT SUM (\"Field1\") AS \"SumValue\" FROM \"Table\"";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateSumWithWhereExpression()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var field = new Field("Field1");
            var where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = statementBuilder.CreateSum(tableName: tableName, field: field, where: where);
            var expected = $"" +
                $"SELECT SUM (\"Field1\") AS \"SumValue\" " +
                $"FROM \"Table\" " +
                $"WHERE (\"Id\" = :Id)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateSumWithHintsThrowsSinceOracleDoesNotSupportTableHints()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var field = new Field("Field1");

            // Act
            Assert.Throws<NotSupportedException>(() =>
                statementBuilder.CreateSum(tableName: tableName, field: field, hints: "NOLOCK"));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateSumIfTheFieldIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateSum(tableName: tableName, field: null));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateSumIfTheTableIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var field = new Field("Field1");

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateSum(tableName: null, field: field));
        }

        #endregion

        #region CreateSumAll

        [TestMethod]
        public void TestOracleStatementBuilderCreateSumAll()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var field = new Field("Field1");

            // Act
            var actual = statementBuilder.CreateSumAll(tableName: tableName, field: field);
            var expected = "SELECT SUM (\"Field1\") AS \"SumValue\" FROM \"Table\"";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateSumAllIfTheFieldIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateSumAll(tableName: tableName, field: null));
        }

        #endregion

        #region CreateTruncate

        [TestMethod]
        public void TestOracleStatementBuilderCreateTruncate()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";

            // Act
            var actual = statementBuilder.CreateTruncate(tableName: tableName);
            var expected = "TRUNCATE TABLE \"Table\"";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateTruncateWithQuotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "\"SCHEMA\".\"Table\"";

            // Act
            var actual = statementBuilder.CreateTruncate(tableName: tableName);
            var expected = "TRUNCATE TABLE \"SCHEMA\".\"Table\"";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateTruncateWithUnquotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "SCHEMA.Table";

            // Act
            var actual = statementBuilder.CreateTruncate(tableName: tableName);
            var expected = "TRUNCATE TABLE \"SCHEMA\".\"Table\"";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateTruncateIfTheTableIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateTruncate(tableName: null));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateTruncateIfTheTableIsEmpty()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateTruncate(tableName: ""));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateTruncateIfTheTableIsWhitespace()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateTruncate(tableName: " "));
        }

        #endregion

        #region CreateUpdate

        [TestMethod]
        public void TestOracleStatementBuilderCreateUpdateAppliesUpdateParameterPrefixToWhereClause()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2" });

            // In the real end-to-end pipeline, RepoDb.Core's Update operation calls
            // QueryGroup.IsForUpdate() -> QueryField.IsForUpdate() -> Parameter.PrependText(
            // StringConstant.UpdateParameterPrefix) on every WHERE-clause field BEFORE the statement
            // builder ever runs, prefixing the bind variable (but not the column name) with "m_" so it
            // can never collide with a same-named SET-clause parameter (e.g. "Id" -> bind variable
            // "m_Id", column reference stays "Id"). This unit test calls CreateUpdate directly against
            // a plain, not-yet-prefixed QueryField, so OracleStatementBuilder's own defensive
            // EnsureParameters(where) call is what applies the "m_" prefix here.
            var where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = statementBuilder.CreateUpdate(tableName: tableName, fields: fields, where: where);
            var expected = $"" +
                $"UPDATE \"Table\" " +
                $"SET \"Field1\" = :Field1, \"Field2\" = :Field2 " +
                $"WHERE (\"Id\" = :m_Id)";

            // Assert - the bind variable is ":m_Id" (a letter-first, Oracle-legal identifier).
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateUpdateDoesNotDoublePrefixAnAlreadyPrefixedWhereParameter()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2" });

            // Simulates a QueryField whose Parameter.Name already starts with the "m_" update prefix
            // (the normal end-to-end state, once Core's IsForUpdate() has already run). Parameter.
            // PrependText is internal to RepoDb.Core (not visible to this test project), so this
            // constructs the already-prefixed state directly via the public QueryField(name, value)
            // constructor instead of calling PrependText a second time - EnsureParameters must leave
            // an already-"m_"-prefixed name alone rather than doubling it into "m_m_Id".
            var where = new QueryGroup(new QueryField("m_Id", 1));

            // Act
            var actual = statementBuilder.CreateUpdate(tableName: tableName, fields: fields, where: where);
            var expected = $"" +
                $"UPDATE \"Table\" " +
                $"SET \"Field1\" = :Field1, \"Field2\" = :Field2 " +
                $"WHERE (\"m_Id\" = :m_Id)";

            // Assert - not ":m_m_Id".
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateUpdate()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });

            // Act
            var actual = statementBuilder.CreateUpdate(tableName: tableName, fields: fields, where: null);
            var expected = $"" +
                $"UPDATE \"Table\" " +
                $"SET \"Field1\" = :Field1, \"Field2\" = :Field2, \"Field3\" = :Field3";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateUpdateWithQuotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "\"SCHEMA\".\"Table\"";
            var fields = Field.From(new[] { "Field1", "Field2" });

            // Act
            var actual = statementBuilder.CreateUpdate(tableName: tableName, fields: fields, where: null);
            var expected = $"" +
                $"UPDATE \"SCHEMA\".\"Table\" " +
                $"SET \"Field1\" = :Field1, \"Field2\" = :Field2";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateUpdateWithUnquotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "SCHEMA.Table";
            var fields = Field.From(new[] { "Field1", "Field2" });

            // Act
            var actual = statementBuilder.CreateUpdate(tableName: tableName, fields: fields, where: null);
            var expected = $"" +
                $"UPDATE \"SCHEMA\".\"Table\" " +
                $"SET \"Field1\" = :Field1, \"Field2\" = :Field2";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateUpdateWithPrimaryExcludedFromSetClause()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var primaryField = new DbField("Field1", true, false, false, typeof(int), null, null, null, null);

            // Act
            var actual = statementBuilder.CreateUpdate(tableName: tableName,
                fields: fields,
                where: null,
                primaryField: primaryField,
                identityField: null);
            var expected = $"" +
                $"UPDATE \"Table\" " +
                $"SET \"Field2\" = :Field2, \"Field3\" = :Field3";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateUpdateWithIdentityExcludedFromSetClause()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var identityField = new DbField("Field1", false, true, false, typeof(int), null, null, null, null);

            // Act
            var actual = statementBuilder.CreateUpdate(tableName: tableName,
                fields: fields,
                where: null,
                primaryField: null,
                identityField: identityField);
            var expected = $"" +
                $"UPDATE \"Table\" " +
                $"SET \"Field2\" = :Field2, \"Field3\" = :Field3";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateUpdateWithHintsThrowsSinceOracleDoesNotSupportTableHints()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2" });

            // Act - OracleDbSetting.AreTableHintsSupported == false, so GuardHints() throws for any
            // non-null/non-whitespace hints, regardless of what the hints text actually says.
            Assert.Throws<NotSupportedException>(() =>
                statementBuilder.CreateUpdate(tableName: tableName,
                    fields: fields,
                    where: null,
                    hints: "NOLOCK"));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateUpdateIfTheTableIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var fields = Field.From(new[] { "Field1", "Field2" });

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateUpdate(tableName: null, fields: fields, where: null));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateUpdateIfTheTableIsEmpty()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var fields = Field.From(new[] { "Field1", "Field2" });

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateUpdate(tableName: "", fields: fields, where: null));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateUpdateIfThereAreNoUpdatableFields()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1" });
            var primaryField = new DbField("Field1", true, false, false, typeof(int), null, null, null, null);

            // Act - "Field1" is the only field, and it's excluded from the SET clause as the primary,
            // leaving no updatable fields at all.
            Assert.Throws<EmptyException>(() =>
                statementBuilder.CreateUpdate(tableName: tableName,
                    fields: fields,
                    where: null,
                    primaryField: primaryField,
                    identityField: null));
        }

        #endregion

        #region CreateUpdateAll

        [TestMethod]
        public void TestOracleStatementBuilderCreateUpdateAll()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From("Field1");

            // Act
            var actual = statementBuilder.CreateUpdateAll(tableName: tableName,
                fields: fields,
                qualifiers: qualifiers,
                batchSize: 1,
                primaryField: null,
                identityField: null);
            var expected = $"" +
                $"UPDATE \"Table\" " +
                $"SET \"Field2\" = :Field2, \"Field3\" = :Field3 " +
                $"WHERE (\"Field1\" = :Field1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateUpdateAllWithPrimaryAsQualifierFallback()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var primaryField = new DbField("Field1", true, false, false, typeof(int), null, null, null, null);

            // Act - no explicit qualifiers, so the primary field (once confirmed present in "fields")
            // is used as the default qualifier.
            var actual = statementBuilder.CreateUpdateAll(tableName: tableName,
                fields: fields,
                qualifiers: null,
                batchSize: 1,
                primaryField: primaryField,
                identityField: null);
            var expected = $"" +
                $"UPDATE \"Table\" " +
                $"SET \"Field2\" = :Field2, \"Field3\" = :Field3 " +
                $"WHERE (\"Field1\" = :Field1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateUpdateAllWithQuotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "\"SCHEMA\".\"Table\"";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From("Field1");

            // Act
            var actual = statementBuilder.CreateUpdateAll(tableName: tableName,
                fields: fields,
                qualifiers: qualifiers,
                batchSize: 1);
            var expected = $"" +
                $"UPDATE \"SCHEMA\".\"Table\" " +
                $"SET \"Field2\" = :Field2, \"Field3\" = :Field3 " +
                $"WHERE (\"Field1\" = :Field1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateUpdateAllWithUnquotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "SCHEMA.Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From("Field1");

            // Act
            var actual = statementBuilder.CreateUpdateAll(tableName: tableName,
                fields: fields,
                qualifiers: qualifiers,
                batchSize: 1);
            var expected = $"" +
                $"UPDATE \"SCHEMA\".\"Table\" " +
                $"SET \"Field2\" = :Field2, \"Field3\" = :Field3 " +
                $"WHERE (\"Field1\" = :Field1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestOracleStatementBuilderCreateUpdateAllWithHintsThrowsSinceOracleDoesNotSupportTableHints()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From("Field1");

            // Act
            Assert.Throws<NotSupportedException>(() =>
                statementBuilder.CreateUpdateAll(tableName: tableName,
                    fields: fields,
                    qualifiers: qualifiers,
                    batchSize: 1,
                    hints: "NOLOCK"));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateUpdateAllIfBatchSizeIsGreaterThanOne()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From("Field1");

            // Act
            Assert.Throws<NotSupportedException>(() =>
                statementBuilder.CreateUpdateAll(tableName: tableName,
                    fields: fields,
                    qualifiers: qualifiers,
                    batchSize: 2));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateUpdateAllIfTheTableIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From("Field1");

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateUpdateAll(tableName: null,
                    fields: fields,
                    qualifiers: qualifiers,
                    batchSize: 1));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateUpdateAllIfThereAreNoFields()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var qualifiers = Field.From("Field1");

            // Act
            Assert.Throws<EmptyException>(() =>
                statementBuilder.CreateUpdateAll(tableName: tableName,
                    fields: null,
                    qualifiers: qualifiers,
                    batchSize: 1));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateUpdateAllIfTheQualifiersAreNotPresentAtTheGivenFields()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From("Id");

            // Act
            Assert.Throws<InvalidQualifiersException>(() =>
                statementBuilder.CreateUpdateAll(tableName: tableName,
                    fields: fields,
                    qualifiers: qualifiers,
                    batchSize: 1));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateUpdateAllIfThePrimaryAsQualifierIsNotPresentAtTheGivenFields()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var primaryField = new DbField("Id", true, false, false, typeof(int), null, null, null, null);

            // Act
            Assert.Throws<InvalidQualifiersException>(() =>
                statementBuilder.CreateUpdateAll(tableName: tableName,
                    fields: fields,
                    qualifiers: null,
                    batchSize: 1,
                    primaryField: primaryField,
                    identityField: null));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateUpdateAllIfThereAreNoQualifiersAndNoPrimary()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateUpdateAll(tableName: tableName,
                    fields: fields,
                    qualifiers: null,
                    batchSize: 1,
                    primaryField: null,
                    identityField: null));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateUpdateAllIfThePrimaryIsNotReallyAPrimary()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var primaryField = new DbField("Field1", false, false, false, typeof(int), null, null, null, null);

            // Act
            Assert.Throws<InvalidOperationException>(() =>
                statementBuilder.CreateUpdateAll(tableName: tableName,
                    fields: fields,
                    qualifiers: null,
                    batchSize: 1,
                    primaryField: primaryField,
                    identityField: null));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleStatementBuilderCreateUpdateAllIfTheIdentityIsNotReallyAnIdentity()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<OracleConnection>();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From("Field1");
            var identityField = new DbField("Field2", false, false, false, typeof(int), null, null, null, null);

            // Act
            Assert.Throws<InvalidOperationException>(() =>
                statementBuilder.CreateUpdateAll(tableName: tableName,
                    fields: fields,
                    qualifiers: qualifiers,
                    batchSize: 1,
                    primaryField: null,
                    identityField: identityField));
        }

        #endregion
    }
}
