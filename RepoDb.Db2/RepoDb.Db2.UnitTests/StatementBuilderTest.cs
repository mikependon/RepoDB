using Microsoft.VisualStudio.TestTools.UnitTesting;
using IBM.Data.Db2;
using RepoDb.Enumerations;
using RepoDb.Exceptions;
using System;

namespace RepoDb.Db2.UnitTests
{
    [TestClass]
    public class StatementBuilderTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseDb2();
        }

        #region CreateAverage

        // NOTE: BaseStatementBuilder.CreateAverage/CreateAverageAll always assign
        // field.Type = AverageableClientTypeResolver.Resolve(field.Type ?? DbSetting.AverageableType)
        // before building the SQL - even when the caller never set a Field.Type. Since
        // Db2DbSetting.AverageableType is typeof(double), an untyped field ends up with
        // Type = typeof(double), which is non-null, so Db2ConvertFieldResolver then wraps it in a
        // CAST(... AS DOUBLE) - Db2's real double-precision floating-point type (there is no
        // "BINARY_DOUBLE" in Db2; that's Oracle's name for the same concept). This CAST only happens
        // for Average/AverageAll - Count/Max/Min/Sum never touch field.Type, so they never get cast
        // (see the other regions in this file).

        [TestMethod]
        public void TestDb2StatementBuilderCreateAverage()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var tableName = "Table";
            var field = new Field("Field1");

            // Act
            var actual = statementBuilder.CreateAverage(tableName: tableName, field: field, where: null);
            var expected = "SELECT AVG (CAST(\"Field1\" AS DOUBLE)) AS \"AverageValue\" FROM \"Table\"";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestDb2StatementBuilderCreateAverageWithWhereExpression()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var tableName = "Table";
            var field = new Field("Field1");
            var where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = statementBuilder.CreateAverage(tableName: tableName, field: field, where: where);
            var expected = $"" +
                $"SELECT AVG (CAST(\"Field1\" AS DOUBLE)) AS \"AverageValue\" " +
                $"FROM \"Table\" " +
                $"WHERE (\"Id\" = :Id)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestDb2StatementBuilderCreateAverageWithQuotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var tableName = "\"SCHEMA\".\"Table\"";
            var field = new Field("Field1");

            // Act
            var actual = statementBuilder.CreateAverage(tableName: tableName, field: field, where: null);
            var expected = "SELECT AVG (CAST(\"Field1\" AS DOUBLE)) AS \"AverageValue\" FROM \"SCHEMA\".\"Table\"";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestDb2StatementBuilderCreateAverageWithHintsThrowsSinceDb2DoesNotSupportTableHints()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var tableName = "Table";
            var field = new Field("Field1");

            // Act
            Assert.Throws<NotSupportedException>(() =>
                statementBuilder.CreateAverage(tableName: tableName, field: field, hints: "NOLOCK"));
        }

        [TestMethod]
        public void ThrowExceptionOnDb2StatementBuilderCreateAverageIfTheFieldIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var tableName = "Table";

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateAverage(tableName: tableName, field: null));
        }

        [TestMethod]
        public void ThrowExceptionOnDb2StatementBuilderCreateAverageIfTheTableIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var field = new Field("Field1");

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateAverage(tableName: null, field: field));
        }

        #endregion

        #region CreateAverageAll

        [TestMethod]
        public void TestDb2StatementBuilderCreateAverageAll()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var tableName = "Table";
            var field = new Field("Field1");

            // Act
            var actual = statementBuilder.CreateAverageAll(tableName: tableName, field: field);
            var expected = "SELECT AVG (CAST(\"Field1\" AS DOUBLE)) AS \"AverageValue\" FROM \"Table\"";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestDb2StatementBuilderCreateAverageAllWithHintsThrowsSinceDb2DoesNotSupportTableHints()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var tableName = "Table";
            var field = new Field("Field1");

            // Act
            Assert.Throws<NotSupportedException>(() =>
                statementBuilder.CreateAverageAll(tableName: tableName, field: field, hints: "NOLOCK"));
        }

        [TestMethod]
        public void ThrowExceptionOnDb2StatementBuilderCreateAverageAllIfTheFieldIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var tableName = "Table";

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateAverageAll(tableName: tableName, field: null));
        }

        #endregion

        #region CreateBatchQuery

        [TestMethod]
        public void TestDb2StatementBuilderCreateBatchQueryFirstBatch()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void TestDb2StatementBuilderCreateBatchQuerySecondBatch()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void ThrowExceptionOnDb2StatementBuilderCreateBatchQueryIfThereAreNoOrderFields()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void TestDb2StatementBuilderCreateBatchQueryWithWhereExpression()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void TestDb2StatementBuilderCreateBatchQueryWithQuotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void TestDb2StatementBuilderCreateBatchQueryWithUnquotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void TestDb2StatementBuilderCreateBatchQueryWithHintsThrowsSinceDb2DoesNotSupportTableHints()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void ThrowExceptionOnDb2StatementBuilderCreateBatchQueryIfTheTableIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void ThrowExceptionOnDb2StatementBuilderCreateBatchQueryIfTheTableIsEmpty()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void ThrowExceptionOnDb2StatementBuilderCreateBatchQueryIfTheTableIsWhitespace()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void ThrowExceptionOnDb2StatementBuilderCreateBatchQueryIfTheFieldsAreNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void ThrowExceptionOnDb2StatementBuilderCreateBatchQueryIfThePageIsLessThanZero()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void ThrowExceptionOnDb2StatementBuilderCreateBatchQueryIfTheRowsPerBatchIsLessThanOne()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void TestDb2StatementBuilderCreateCount()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var tableName = "Table";

            // Act - Db2 has no BIGINT-forcing "COUNT_BIG" equivalent need (NUMBER already handles
            // large counts natively), so this uses the generic ANSI "COUNT (*)" from BaseStatementBuilder
            // unlike SqlServerStatementBuilder, which overrides this to use COUNT_BIG.
            var actual = statementBuilder.CreateCount(tableName: tableName, where: null);
            var expected = "SELECT COUNT (*) AS \"CountValue\" FROM \"Table\"";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestDb2StatementBuilderCreateCountWithWhereExpression()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void TestDb2StatementBuilderCreateCountWithQuotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var tableName = "\"SCHEMA\".\"Table\"";

            // Act
            var actual = statementBuilder.CreateCount(tableName: tableName, where: null);
            var expected = "SELECT COUNT (*) AS \"CountValue\" FROM \"SCHEMA\".\"Table\"";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestDb2StatementBuilderCreateCountWithUnquotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var tableName = "SCHEMA.Table";

            // Act
            var actual = statementBuilder.CreateCount(tableName: tableName, where: null);
            var expected = "SELECT COUNT (*) AS \"CountValue\" FROM \"SCHEMA\".\"Table\"";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestDb2StatementBuilderCreateCountWithHintsThrowsSinceDb2DoesNotSupportTableHints()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var tableName = "Table";

            // Act
            Assert.Throws<NotSupportedException>(() =>
                statementBuilder.CreateCount(tableName: tableName, hints: "NOLOCK"));
        }

        [TestMethod]
        public void ThrowExceptionOnDb2StatementBuilderCreateCountIfTheTableIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateCount(tableName: null));
        }

        #endregion

        #region CreateCountAll

        [TestMethod]
        public void TestDb2StatementBuilderCreateCountAll()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var tableName = "Table";

            // Act
            var actual = statementBuilder.CreateCountAll(tableName: tableName, hints: null);
            var expected = "SELECT COUNT (*) AS \"CountValue\" FROM \"Table\"";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestDb2StatementBuilderCreateCountAllWithQuotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var tableName = "\"SCHEMA\".\"Table\"";

            // Act
            var actual = statementBuilder.CreateCountAll(tableName: tableName, hints: null);
            var expected = "SELECT COUNT (*) AS \"CountValue\" FROM \"SCHEMA\".\"Table\"";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestDb2StatementBuilderCreateCountAllWithUnquotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var tableName = "SCHEMA.Table";

            // Act
            var actual = statementBuilder.CreateCountAll(tableName: tableName, hints: null);
            var expected = "SELECT COUNT (*) AS \"CountValue\" FROM \"SCHEMA\".\"Table\"";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestDb2StatementBuilderCreateCountAllWithHintsThrowsSinceDb2DoesNotSupportTableHints()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var tableName = "Table";

            // Act
            Assert.Throws<NotSupportedException>(() =>
                statementBuilder.CreateCountAll(tableName: tableName, hints: "NOLOCK"));
        }

        [TestMethod]
        public void ThrowExceptionOnDb2StatementBuilderCreateCountAllIfTheTableIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateCountAll(tableName: null));
        }

        #endregion

        #region CreateDelete

        [TestMethod]
        public void TestDb2StatementBuilderCreateDelete()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var tableName = "Table";

            // Act
            var actual = statementBuilder.CreateDelete(tableName: tableName, where: null);
            var expected = "DELETE FROM \"Table\"";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestDb2StatementBuilderCreateDeleteWithWhereExpression()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void TestDb2StatementBuilderCreateDeleteWithQuotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var tableName = "\"SCHEMA\".\"Table\"";

            // Act
            var actual = statementBuilder.CreateDelete(tableName: tableName, where: null);
            var expected = "DELETE FROM \"SCHEMA\".\"Table\"";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestDb2StatementBuilderCreateDeleteWithUnquotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var tableName = "SCHEMA.Table";

            // Act
            var actual = statementBuilder.CreateDelete(tableName: tableName, where: null);
            var expected = "DELETE FROM \"SCHEMA\".\"Table\"";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestDb2StatementBuilderCreateDeleteWithHintsThrowsSinceDb2DoesNotSupportTableHints()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var tableName = "Table";

            // Act
            Assert.Throws<NotSupportedException>(() =>
                statementBuilder.CreateDelete(tableName: tableName, hints: "NOLOCK"));
        }

        [TestMethod]
        public void ThrowExceptionOnDb2StatementBuilderCreateDeleteIfTheTableIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateDelete(tableName: null));
        }

        [TestMethod]
        public void ThrowExceptionOnDb2StatementBuilderCreateDeleteIfTheTableIsEmpty()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateDelete(tableName: ""));
        }

        [TestMethod]
        public void ThrowExceptionOnDb2StatementBuilderCreateDeleteIfTheTableIsWhitespace()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateDelete(tableName: " "));
        }

        #endregion

        #region CreateDeleteAll

        [TestMethod]
        public void TestDb2StatementBuilderCreateDeleteAll()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var tableName = "Table";

            // Act
            var actual = statementBuilder.CreateDeleteAll(tableName: tableName);
            var expected = "DELETE FROM \"Table\"";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestDb2StatementBuilderCreateDeleteAllWithQuotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var tableName = "\"SCHEMA\".\"Table\"";

            // Act
            var actual = statementBuilder.CreateDeleteAll(tableName: tableName);
            var expected = "DELETE FROM \"SCHEMA\".\"Table\"";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestDb2StatementBuilderCreateDeleteAllWithUnquotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var tableName = "SCHEMA.Table";

            // Act
            var actual = statementBuilder.CreateDeleteAll(tableName: tableName);
            var expected = "DELETE FROM \"SCHEMA\".\"Table\"";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestDb2StatementBuilderCreateDeleteAllWithHintsThrowsSinceDb2DoesNotSupportTableHints()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var tableName = "Table";

            // Act
            Assert.Throws<NotSupportedException>(() =>
                statementBuilder.CreateDeleteAll(tableName: tableName, hints: "NOLOCK"));
        }

        [TestMethod]
        public void ThrowExceptionOnDb2StatementBuilderCreateDeleteAllIfTheTableIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateDeleteAll(tableName: null));
        }

        [TestMethod]
        public void ThrowExceptionOnDb2StatementBuilderCreateDeleteAllIfTheTableIsEmpty()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateDeleteAll(tableName: ""));
        }

        [TestMethod]
        public void ThrowExceptionOnDb2StatementBuilderCreateDeleteAllIfTheTableIsWhitespace()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateDeleteAll(tableName: " "));
        }

        #endregion

        #region CreateExists

        [TestMethod]
        public void TestDb2StatementBuilderCreateExists()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void TestDb2StatementBuilderCreateExistsWithoutWhere()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void TestDb2StatementBuilderCreateExistsWithQuotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void TestDb2StatementBuilderCreateExistsWithUnquotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void TestDb2StatementBuilderCreateExistsWithHintsThrowsSinceDb2DoesNotSupportTableHints()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var tableName = "Table";

            // Act
            Assert.Throws<NotSupportedException>(() =>
                statementBuilder.CreateExists(tableName: tableName, hints: "NOLOCK"));
        }

        [TestMethod]
        public void ThrowExceptionOnDb2StatementBuilderCreateExistsIfTheTableIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateExists(tableName: null));
        }

        [TestMethod]
        public void ThrowExceptionOnDb2StatementBuilderCreateExistsIfTheTableIsEmpty()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateExists(tableName: ""));
        }

        [TestMethod]
        public void ThrowExceptionOnDb2StatementBuilderCreateExistsIfTheTableIsWhitespace()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateExists(tableName: " "));
        }

        #endregion

        #region CreateInsert

        [TestMethod]
        public void TestDb2StatementBuilderCreateInsertWithNoKeyColumn()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void TestDb2StatementBuilderCreateInsertWithIdentity()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var identityField = new DbField("Field1", false, true, false, typeof(int), null, null, null, null);

            // Act
            var actual = statementBuilder.CreateInsert(tableName: tableName,
                fields: fields,
                primaryField: null,
                identityField: identityField);
            var expected = $"" +
                $"SELECT \"Field1\" FROM FINAL TABLE (" +
                $"INSERT INTO \"Table\" ( \"Field2\", \"Field3\" ) VALUES ( :Field2, :Field3 )" +
                $")";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestDb2StatementBuilderCreateInsertWithPrimary()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
                $"SELECT \"Field1\" FROM FINAL TABLE (" +
                $"INSERT INTO \"Table\" ( \"Field1\", \"Field2\", \"Field3\" ) VALUES ( :Field1, :Field2, :Field3 )" +
                $")";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestDb2StatementBuilderCreateInsertWithQuotedTableSchemaAndNoKeyColumn()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void TestDb2StatementBuilderCreateInsertWithUnquotedTableSchemaAndNoKeyColumn()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void TestDb2StatementBuilderCreateInsertWithQuotedTableSchemaAndIdentity()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var tableName = "\"SCHEMA\".\"Table\"";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var identityField = new DbField("Field1", false, true, false, typeof(int), null, null, null, null);

            // Act
            var actual = statementBuilder.CreateInsert(tableName: tableName,
                fields: fields,
                primaryField: null,
                identityField: identityField);
            var expected = $"" +
                $"SELECT \"Field1\" FROM FINAL TABLE (" +
                $"INSERT INTO \"SCHEMA\".\"Table\" ( \"Field2\", \"Field3\" ) VALUES ( :Field2, :Field3 )" +
                $")";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestDb2StatementBuilderCreateInsertWithHintsThrowsSinceDb2DoesNotSupportTableHints()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2" });

            // Act
            Assert.Throws<NotSupportedException>(() =>
                statementBuilder.CreateInsert(tableName: tableName, fields: fields, hints: "NOLOCK"));
        }

        [TestMethod]
        public void ThrowExceptionOnDb2StatementBuilderCreateInsertIfTheTableIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var fields = Field.From(new[] { "Field1", "Field2" });

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateInsert(tableName: null, fields: fields));
        }

        [TestMethod]
        public void ThrowExceptionOnDb2StatementBuilderCreateInsertIfTheTableIsEmpty()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var fields = Field.From(new[] { "Field1", "Field2" });

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateInsert(tableName: "", fields: fields));
        }

        [TestMethod]
        public void ThrowExceptionOnDb2StatementBuilderCreateInsertIfTheTableIsWhitespace()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var fields = Field.From(new[] { "Field1", "Field2" });

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateInsert(tableName: " ", fields: fields));
        }

        [TestMethod]
        public void ThrowExceptionOnDb2StatementBuilderCreateInsertIfTheFieldsAreNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var tableName = "Table";

            // Act
            Assert.Throws<EmptyException>(() =>
                statementBuilder.CreateInsert(tableName: tableName, fields: null));
        }

        [TestMethod]
        public void ThrowExceptionOnDb2StatementBuilderCreateInsertIfThePrimaryIsNotReallyAPrimary()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void ThrowExceptionOnDb2StatementBuilderCreateInsertIfTheIdentityIsNotReallyAnIdentity()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void TestDb2StatementBuilderCreateInsertAllWithIdentity()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
                $"SELECT \"Field1\" FROM FINAL TABLE (" +
                $"INSERT INTO \"Table\" ( \"Field2\", \"Field3\" ) VALUES ( :Field2, :Field3 )" +
                $")";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ThrowExceptionOnDb2StatementBuilderCreateInsertAllIfBatchSizeIsGreaterThanOne()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });

            // Act
            Assert.Throws<NotSupportedException>(() =>
                statementBuilder.CreateInsertAll(tableName: tableName,
                    fields: fields,
                    batchSize: 2));
        }

        [TestMethod]
        public void TestDb2StatementBuilderCreateInsertAllWithNoKeyColumn()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void TestDb2StatementBuilderCreateInsertAllWithQuotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
                $"SELECT \"Field1\" FROM FINAL TABLE (" +
                $"INSERT INTO \"SCHEMA\".\"Table\" ( \"Field2\", \"Field3\" ) VALUES ( :Field2, :Field3 )" +
                $")";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestDb2StatementBuilderCreateInsertAllWithHintsThrowsSinceDb2DoesNotSupportTableHints()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void ThrowExceptionOnDb2StatementBuilderCreateInsertAllIfTheTableIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var fields = Field.From(new[] { "Field1", "Field2" });

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateInsertAll(tableName: null, fields: fields, batchSize: 1));
        }

        [TestMethod]
        public void ThrowExceptionOnDb2StatementBuilderCreateInsertAllIfTheFieldsAreNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var tableName = "Table";

            // Act
            Assert.Throws<EmptyException>(() =>
                statementBuilder.CreateInsertAll(tableName: tableName, fields: null, batchSize: 1));
        }

        #endregion

        #region CreateMax

        [TestMethod]
        public void TestDb2StatementBuilderCreateMax()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var tableName = "Table";
            var field = new Field("Field1");

            // Act
            var actual = statementBuilder.CreateMax(tableName: tableName, field: field, where: null);
            var expected = "SELECT MAX (\"Field1\") AS \"MaxValue\" FROM \"Table\"";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestDb2StatementBuilderCreateMaxWithWhereExpression()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void TestDb2StatementBuilderCreateMaxWithHintsThrowsSinceDb2DoesNotSupportTableHints()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var tableName = "Table";
            var field = new Field("Field1");

            // Act
            Assert.Throws<NotSupportedException>(() =>
                statementBuilder.CreateMax(tableName: tableName, field: field, hints: "NOLOCK"));
        }

        [TestMethod]
        public void ThrowExceptionOnDb2StatementBuilderCreateMaxIfTheFieldIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var tableName = "Table";

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateMax(tableName: tableName, field: null));
        }

        [TestMethod]
        public void ThrowExceptionOnDb2StatementBuilderCreateMaxIfTheTableIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var field = new Field("Field1");

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateMax(tableName: null, field: field));
        }

        #endregion

        #region CreateMaxAll

        [TestMethod]
        public void TestDb2StatementBuilderCreateMaxAll()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var tableName = "Table";
            var field = new Field("Field1");

            // Act
            var actual = statementBuilder.CreateMaxAll(tableName: tableName, field: field);
            var expected = "SELECT MAX (\"Field1\") AS \"MaxValue\" FROM \"Table\"";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ThrowExceptionOnDb2StatementBuilderCreateMaxAllIfTheFieldIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var tableName = "Table";

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateMaxAll(tableName: tableName, field: null));
        }

        #endregion

        #region CreateMerge

        [TestMethod]
        public void TestDb2StatementBuilderCreateMergeWithNoKeyColumn()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void ThrowExceptionOnDb2StatementBuilderCreateMergeIfThereAreNoQualifiersAndNoPrimary()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void TestDb2StatementBuilderCreateMergeWithQuotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void TestDb2StatementBuilderCreateMergeWithUnquotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void TestDb2StatementBuilderCreateMergeWithCoveredPrimary()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
                $"SELECT \"Field1\" FROM FINAL TABLE (" +
                $"MERGE INTO \"Table\" T " +
                $"USING ( SELECT :Field1 AS \"Field1\", :Field2 AS \"Field2\", :Field3 AS \"Field3\" FROM DUAL ) " +
                $"S ON ( (S.\"Field1\" = T.\"Field1\" OR (S.\"Field1\" IS NULL AND T.\"Field1\" IS NULL)) ) " +
                $"WHEN MATCHED THEN " +
                $"UPDATE SET T.\"Field2\" = S.\"Field2\", T.\"Field3\" = S.\"Field3\" " +
                $"WHEN NOT MATCHED THEN " +
                $"INSERT ( \"Field1\", \"Field2\", \"Field3\" ) " +
                $"VALUES ( S.\"Field1\", S.\"Field2\", S.\"Field3\" )" +
                $")";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestDb2StatementBuilderCreateMergeWithUncoveredPrimary()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From("Field1");
            var primaryField = new DbField("Id", true, false, false, typeof(int), null, null, null, null);

            // Act - Db2's CreateMerge (unlike CreateInsert) never checks that the primary field
            // is actually present among the given "fields"; it's only used to resolve the RETURNING
            // key column.
            var actual = statementBuilder.CreateMerge(tableName: tableName,
                fields: fields,
                qualifiers: qualifiers,
                primaryField: primaryField,
                identityField: null);
            var expected = $"" +
                $"SELECT \"Id\" FROM FINAL TABLE (" +
                $"MERGE INTO \"Table\" T " +
                $"USING ( SELECT :Field1 AS \"Field1\", :Field2 AS \"Field2\", :Field3 AS \"Field3\" FROM DUAL ) " +
                $"S ON ( (S.\"Field1\" = T.\"Field1\" OR (S.\"Field1\" IS NULL AND T.\"Field1\" IS NULL)) ) " +
                $"WHEN MATCHED THEN " +
                $"UPDATE SET T.\"Field2\" = S.\"Field2\", T.\"Field3\" = S.\"Field3\" " +
                $"WHEN NOT MATCHED THEN " +
                $"INSERT ( \"Field1\", \"Field2\", \"Field3\" ) " +
                $"VALUES ( S.\"Field1\", S.\"Field2\", S.\"Field3\" )" +
                $")";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestDb2StatementBuilderCreateMergeWithCoveredIdentity()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
                $"SELECT \"Field1\" FROM FINAL TABLE (" +
                $"MERGE INTO \"Table\" T " +
                $"USING ( SELECT :Field1 AS \"Field1\", :Field2 AS \"Field2\", :Field3 AS \"Field3\" FROM DUAL ) " +
                $"S ON ( (S.\"Field1\" = T.\"Field1\" OR (S.\"Field1\" IS NULL AND T.\"Field1\" IS NULL)) ) " +
                $"WHEN MATCHED THEN " +
                $"UPDATE SET T.\"Field2\" = S.\"Field2\", T.\"Field3\" = S.\"Field3\" " +
                $"WHEN NOT MATCHED THEN " +
                $"INSERT ( \"Field2\", \"Field3\" ) " +
                $"VALUES ( S.\"Field2\", S.\"Field3\" )" +
                $")";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestDb2StatementBuilderCreateMergeWithUncoveredIdentity()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
                $"SELECT \"Id\" FROM FINAL TABLE (" +
                $"MERGE INTO \"Table\" T " +
                $"USING ( SELECT :Field1 AS \"Field1\", :Field2 AS \"Field2\", :Field3 AS \"Field3\" FROM DUAL ) " +
                $"S ON ( (S.\"Field1\" = T.\"Field1\" OR (S.\"Field1\" IS NULL AND T.\"Field1\" IS NULL)) ) " +
                $"WHEN MATCHED THEN " +
                $"UPDATE SET T.\"Field2\" = S.\"Field2\", T.\"Field3\" = S.\"Field3\" " +
                $"WHEN NOT MATCHED THEN " +
                $"INSERT ( \"Field1\", \"Field2\", \"Field3\" ) " +
                $"VALUES ( S.\"Field1\", S.\"Field2\", S.\"Field3\" )" +
                $")";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestDb2StatementBuilderCreateMergeWithHintsThrowsSinceDb2DoesNotSupportTableHints()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void ThrowExceptionOnDb2StatementBuilderCreateMergeIfTheTableIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From("Field1");

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateMerge(tableName: null, fields: fields, qualifiers: qualifiers));
        }

        [TestMethod]
        public void ThrowExceptionOnDb2StatementBuilderCreateMergeIfTheTableIsEmpty()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From("Field1");

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateMerge(tableName: "", fields: fields, qualifiers: qualifiers));
        }

        [TestMethod]
        public void ThrowExceptionOnDb2StatementBuilderCreateMergeIfTheTableIsWhitespace()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From("Field1");

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateMerge(tableName: " ", fields: fields, qualifiers: qualifiers));
        }

        [TestMethod]
        public void ThrowExceptionOnDb2StatementBuilderCreateMergeIfThereAreNoFields()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var tableName = "Table";
            var qualifiers = Field.From("Field1");

            // Act
            Assert.Throws<EmptyException>(() =>
                statementBuilder.CreateMerge(tableName: tableName, fields: null, qualifiers: qualifiers));
        }

        [TestMethod]
        public void ThrowExceptionOnDb2StatementBuilderCreateMergeIfThePrimaryIsNotReallyAPrimary()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void ThrowExceptionOnDb2StatementBuilderCreateMergeIfTheIdentityIsNotReallyAnIdentity()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void ThrowExceptionOnDb2StatementBuilderCreateMergeAllIfBatchSizeIsGreaterThanOne()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void TestDb2StatementBuilderCreateMergeAllWithNoKeyColumn()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void TestDb2StatementBuilderCreateMergeAllWithIdentity()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
                $"SELECT \"Field1\" FROM FINAL TABLE (" +
                $"MERGE INTO \"Table\" T " +
                $"USING ( SELECT :Field1 AS \"Field1\", :Field2 AS \"Field2\", :Field3 AS \"Field3\" FROM DUAL ) " +
                $"S ON ( (S.\"Field1\" = T.\"Field1\" OR (S.\"Field1\" IS NULL AND T.\"Field1\" IS NULL)) ) " +
                $"WHEN MATCHED THEN " +
                $"UPDATE SET T.\"Field2\" = S.\"Field2\", T.\"Field3\" = S.\"Field3\" " +
                $"WHEN NOT MATCHED THEN " +
                $"INSERT ( \"Field2\", \"Field3\" ) " +
                $"VALUES ( S.\"Field2\", S.\"Field3\" )" +
                $")";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestDb2StatementBuilderCreateMergeAllWithHintsThrowsSinceDb2DoesNotSupportTableHints()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void ThrowExceptionOnDb2StatementBuilderCreateMergeAllIfTheTableIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void ThrowExceptionOnDb2StatementBuilderCreateMergeAllIfThereAreNoQualifiersAndNoPrimary()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void TestDb2StatementBuilderCreateMin()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var tableName = "Table";
            var field = new Field("Field1");

            // Act
            var actual = statementBuilder.CreateMin(tableName: tableName, field: field, where: null);
            var expected = "SELECT MIN (\"Field1\") AS \"MinValue\" FROM \"Table\"";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestDb2StatementBuilderCreateMinWithWhereExpression()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void TestDb2StatementBuilderCreateMinWithHintsThrowsSinceDb2DoesNotSupportTableHints()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var tableName = "Table";
            var field = new Field("Field1");

            // Act
            Assert.Throws<NotSupportedException>(() =>
                statementBuilder.CreateMin(tableName: tableName, field: field, hints: "NOLOCK"));
        }

        [TestMethod]
        public void ThrowExceptionOnDb2StatementBuilderCreateMinIfTheFieldIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var tableName = "Table";

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateMin(tableName: tableName, field: null));
        }

        [TestMethod]
        public void ThrowExceptionOnDb2StatementBuilderCreateMinIfTheTableIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var field = new Field("Field1");

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateMin(tableName: null, field: field));
        }

        #endregion

        #region CreateMinAll

        [TestMethod]
        public void TestDb2StatementBuilderCreateMinAll()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var tableName = "Table";
            var field = new Field("Field1");

            // Act
            var actual = statementBuilder.CreateMinAll(tableName: tableName, field: field);
            var expected = "SELECT MIN (\"Field1\") AS \"MinValue\" FROM \"Table\"";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ThrowExceptionOnDb2StatementBuilderCreateMinAllIfTheFieldIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var tableName = "Table";

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateMinAll(tableName: tableName, field: null));
        }

        #endregion

        #region CreateQuery

        [TestMethod]
        public void TestDb2StatementBuilderCreateQuery()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2" });

            // Act
            var actual = statementBuilder.CreateQuery(tableName: tableName, fields: fields);
            var expected = "SELECT \"Field1\", \"Field2\" FROM \"Table\"";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestDb2StatementBuilderCreateQueryWithTop()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2" });

            // Act
            var actual = statementBuilder.CreateQuery(tableName: tableName, fields: fields, top: 10);
            var expected = "SELECT \"Field1\", \"Field2\" FROM \"Table\" FETCH FIRST 10 ROWS ONLY";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestDb2StatementBuilderCreateQueryWithWhereExpression()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void TestDb2StatementBuilderCreateQueryWithOrderBy()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void TestDb2StatementBuilderCreateQueryWithQuotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var tableName = "\"SCHEMA\".\"Table\"";
            var fields = Field.From(new[] { "Field1", "Field2" });

            // Act
            var actual = statementBuilder.CreateQuery(tableName: tableName, fields: fields);
            var expected = "SELECT \"Field1\", \"Field2\" FROM \"SCHEMA\".\"Table\"";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestDb2StatementBuilderCreateQueryWithUnquotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var tableName = "SCHEMA.Table";
            var fields = Field.From(new[] { "Field1", "Field2" });

            // Act
            var actual = statementBuilder.CreateQuery(tableName: tableName, fields: fields);
            var expected = "SELECT \"Field1\", \"Field2\" FROM \"SCHEMA\".\"Table\"";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestDb2StatementBuilderCreateQueryWithHintsThrowsSinceDb2DoesNotSupportTableHints()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2" });

            // Act
            Assert.Throws<NotSupportedException>(() =>
                statementBuilder.CreateQuery(tableName: tableName, fields: fields, hints: "NOLOCK"));
        }

        [TestMethod]
        public void ThrowExceptionOnDb2StatementBuilderCreateQueryIfTheFieldsAreNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var tableName = "Table";

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateQuery(tableName: tableName, fields: null));
        }

        [TestMethod]
        public void ThrowExceptionOnDb2StatementBuilderCreateQueryIfTheTableIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var fields = Field.From(new[] { "Field1", "Field2" });

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateQuery(tableName: null, fields: fields));
        }

        [TestMethod]
        public void ThrowExceptionOnDb2StatementBuilderCreateQueryIfTheTableIsEmpty()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var fields = Field.From(new[] { "Field1", "Field2" });

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateQuery(tableName: "", fields: fields));
        }

        [TestMethod]
        public void ThrowExceptionOnDb2StatementBuilderCreateQueryIfTheTableIsWhitespace()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var fields = Field.From(new[] { "Field1", "Field2" });

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateQuery(tableName: " ", fields: fields));
        }

        #endregion

        #region CreateSkipQuery

        [TestMethod]
        public void TestDb2StatementBuilderCreateSkipQuery()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void TestDb2StatementBuilderCreateSkipQueryWithWhereExpression()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void TestDb2StatementBuilderCreateSkipQueryWithQuotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void TestDb2StatementBuilderCreateSkipQueryWithUnquotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void TestDb2StatementBuilderCreateSkipQueryWithHintsThrowsSinceDb2DoesNotSupportTableHints()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void ThrowExceptionOnDb2StatementBuilderCreateSkipQueryIfTheTableIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void ThrowExceptionOnDb2StatementBuilderCreateSkipQueryIfTheTableIsEmpty()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void ThrowExceptionOnDb2StatementBuilderCreateSkipQueryIfTheTableIsWhitespace()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void ThrowExceptionOnDb2StatementBuilderCreateSkipQueryIfTheFieldsAreNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void ThrowExceptionOnDb2StatementBuilderCreateSkipQueryIfThereAreNoOrderFields()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void ThrowExceptionOnDb2StatementBuilderCreateSkipQueryIfTheSkipIsLessThanZero()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void ThrowExceptionOnDb2StatementBuilderCreateSkipQueryIfTheTakeIsLessThanOne()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void TestDb2StatementBuilderCreateSum()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var tableName = "Table";
            var field = new Field("Field1");

            // Act
            var actual = statementBuilder.CreateSum(tableName: tableName, field: field, where: null);
            var expected = "SELECT SUM (\"Field1\") AS \"SumValue\" FROM \"Table\"";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestDb2StatementBuilderCreateSumWithWhereExpression()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void TestDb2StatementBuilderCreateSumWithHintsThrowsSinceDb2DoesNotSupportTableHints()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var tableName = "Table";
            var field = new Field("Field1");

            // Act
            Assert.Throws<NotSupportedException>(() =>
                statementBuilder.CreateSum(tableName: tableName, field: field, hints: "NOLOCK"));
        }

        [TestMethod]
        public void ThrowExceptionOnDb2StatementBuilderCreateSumIfTheFieldIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var tableName = "Table";

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateSum(tableName: tableName, field: null));
        }

        [TestMethod]
        public void ThrowExceptionOnDb2StatementBuilderCreateSumIfTheTableIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var field = new Field("Field1");

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateSum(tableName: null, field: field));
        }

        #endregion

        #region CreateSumAll

        [TestMethod]
        public void TestDb2StatementBuilderCreateSumAll()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var tableName = "Table";
            var field = new Field("Field1");

            // Act
            var actual = statementBuilder.CreateSumAll(tableName: tableName, field: field);
            var expected = "SELECT SUM (\"Field1\") AS \"SumValue\" FROM \"Table\"";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ThrowExceptionOnDb2StatementBuilderCreateSumAllIfTheFieldIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var tableName = "Table";

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateSumAll(tableName: tableName, field: null));
        }

        #endregion

        #region CreateTruncate

        [TestMethod]
        public void TestDb2StatementBuilderCreateTruncate()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var tableName = "Table";

            // Act
            var actual = statementBuilder.CreateTruncate(tableName: tableName);
            var expected = "TRUNCATE TABLE \"Table\" IMMEDIATE";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestDb2StatementBuilderCreateTruncateWithQuotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var tableName = "\"SCHEMA\".\"Table\"";

            // Act
            var actual = statementBuilder.CreateTruncate(tableName: tableName);
            var expected = "TRUNCATE TABLE \"SCHEMA\".\"Table\" IMMEDIATE";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestDb2StatementBuilderCreateTruncateWithUnquotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var tableName = "SCHEMA.Table";

            // Act
            var actual = statementBuilder.CreateTruncate(tableName: tableName);
            var expected = "TRUNCATE TABLE \"SCHEMA\".\"Table\" IMMEDIATE";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ThrowExceptionOnDb2StatementBuilderCreateTruncateIfTheTableIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateTruncate(tableName: null));
        }

        [TestMethod]
        public void ThrowExceptionOnDb2StatementBuilderCreateTruncateIfTheTableIsEmpty()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateTruncate(tableName: ""));
        }

        [TestMethod]
        public void ThrowExceptionOnDb2StatementBuilderCreateTruncateIfTheTableIsWhitespace()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateTruncate(tableName: " "));
        }

        #endregion

        #region CreateUpdate

        [TestMethod]
        public void TestDb2StatementBuilderCreateUpdateAppliesUpdateParameterPrefixToWhereClause()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2" });

            // In the real end-to-end pipeline, RepoDb.Core's Update operation calls
            // QueryGroup.IsForUpdate() -> QueryField.IsForUpdate() -> Parameter.PrependText(
            // StringConstant.UpdateParameterPrefix) on every WHERE-clause field BEFORE the statement
            // builder ever runs, prefixing the bind variable (but not the column name) with "m_" so it
            // can never collide with a same-named SET-clause parameter (e.g. "Id" -> bind variable
            // "m_Id", column reference stays "Id"). This unit test calls CreateUpdate directly against
            // a plain, not-yet-prefixed QueryField, so Db2StatementBuilder's own defensive
            // EnsureParameters(where) call is what applies the "m_" prefix here.
            var where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = statementBuilder.CreateUpdate(tableName: tableName, fields: fields, where: where);
            var expected = $"" +
                $"UPDATE \"Table\" " +
                $"SET \"Field1\" = :Field1, \"Field2\" = :Field2 " +
                $"WHERE (\"Id\" = :m_Id)";

            // Assert - the bind variable is ":m_Id" (a letter-first, Db2-legal identifier).
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestDb2StatementBuilderCreateUpdateDoesNotDoublePrefixAnAlreadyPrefixedWhereParameter()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void TestDb2StatementBuilderCreateUpdate()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void TestDb2StatementBuilderCreateUpdateWithQuotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void TestDb2StatementBuilderCreateUpdateWithUnquotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void TestDb2StatementBuilderCreateUpdateWithPrimaryExcludedFromSetClause()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void TestDb2StatementBuilderCreateUpdateWithIdentityExcludedFromSetClause()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void TestDb2StatementBuilderCreateUpdateWithHintsThrowsSinceDb2DoesNotSupportTableHints()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2" });

            // Act - Db2DbSetting.AreTableHintsSupported == false, so GuardHints() throws for any
            // non-null/non-whitespace hints, regardless of what the hints text actually says.
            Assert.Throws<NotSupportedException>(() =>
                statementBuilder.CreateUpdate(tableName: tableName,
                    fields: fields,
                    where: null,
                    hints: "NOLOCK"));
        }

        [TestMethod]
        public void ThrowExceptionOnDb2StatementBuilderCreateUpdateIfTheTableIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var fields = Field.From(new[] { "Field1", "Field2" });

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateUpdate(tableName: null, fields: fields, where: null));
        }

        [TestMethod]
        public void ThrowExceptionOnDb2StatementBuilderCreateUpdateIfTheTableIsEmpty()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
            var fields = Field.From(new[] { "Field1", "Field2" });

            // Act
            Assert.Throws<NullReferenceException>(() =>
                statementBuilder.CreateUpdate(tableName: "", fields: fields, where: null));
        }

        [TestMethod]
        public void ThrowExceptionOnDb2StatementBuilderCreateUpdateIfThereAreNoUpdatableFields()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void TestDb2StatementBuilderCreateUpdateAll()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void TestDb2StatementBuilderCreateUpdateAllWithPrimaryAsQualifierFallback()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void TestDb2StatementBuilderCreateUpdateAllWithQuotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void TestDb2StatementBuilderCreateUpdateAllWithUnquotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void TestDb2StatementBuilderCreateUpdateAllWithHintsThrowsSinceDb2DoesNotSupportTableHints()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void ThrowExceptionOnDb2StatementBuilderCreateUpdateAllIfBatchSizeIsGreaterThanOne()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void ThrowExceptionOnDb2StatementBuilderCreateUpdateAllIfTheTableIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void ThrowExceptionOnDb2StatementBuilderCreateUpdateAllIfThereAreNoFields()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void ThrowExceptionOnDb2StatementBuilderCreateUpdateAllIfTheQualifiersAreNotPresentAtTheGivenFields()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void ThrowExceptionOnDb2StatementBuilderCreateUpdateAllIfThePrimaryAsQualifierIsNotPresentAtTheGivenFields()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void ThrowExceptionOnDb2StatementBuilderCreateUpdateAllIfThereAreNoQualifiersAndNoPrimary()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void ThrowExceptionOnDb2StatementBuilderCreateUpdateAllIfThePrimaryIsNotReallyAPrimary()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
        public void ThrowExceptionOnDb2StatementBuilderCreateUpdateAllIfTheIdentityIsNotReallyAnIdentity()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DB2Connection>();
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
