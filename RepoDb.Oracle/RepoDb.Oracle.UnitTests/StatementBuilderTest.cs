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
                $"FETCH FIRST 1 ROWS ONLY ;";

            // Assert
            Assert.AreEqual(expected, actual);
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
            var expected = "SELECT \"Field1\", \"Field2\" FROM \"Table\" ;";

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
            var expected = "SELECT \"Field1\", \"Field2\" FROM \"Table\" FETCH FIRST 10 ROWS ONLY ;";

            // Assert
            Assert.AreEqual(expected, actual);
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
                $"ROWS FETCH NEXT 10 ROWS ONLY ;";

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
                $"ROWS FETCH NEXT 10 ROWS ONLY ;";

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
                $"ROWS FETCH NEXT 10 ROWS ONLY ;";

            // Assert
            Assert.AreEqual(expected, actual);
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
                $"( :Field1, :Field2, :Field3 ) ;";

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
                $"BEGIN " +
                $"INSERT INTO \"Table\" ( \"Field2\", \"Field3\" ) VALUES ( :Field2, :Field3 ) " +
                $"RETURNING \"Field1\" INTO l_repodb_result; " +
                $"DBMS_SQL.RETURN_RESULT(CURSOR(SELECT l_repodb_result AS \"Result\" FROM DUAL)); " +
                $"END;";

            // Assert
            Assert.AreEqual(expected, actual);
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
                $"BEGIN " +
                $"INSERT INTO \"Table\" ( \"Field2\", \"Field3\" ) VALUES ( :Field2, :Field3 ) " +
                $"RETURNING \"Field1\" INTO l_repodb_result; " +
                $"DBMS_SQL.RETURN_RESULT(CURSOR(SELECT l_repodb_result AS \"Result\" FROM DUAL)); " +
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
                $"MERGE INTO \"Table\" AS T " +
                $"USING ( SELECT :Field1 AS \"Field1\", :Field2 AS \"Field2\", :Field3 AS \"Field3\" FROM DUAL ) " +
                $"AS S ON ( (S.\"Field1\" = T.\"Field1\" OR (S.\"Field1\" IS NULL AND T.\"Field1\" IS NULL)) ) " +
                $"WHEN MATCHED THEN " +
                $"UPDATE SET T.\"Field2\" = S.\"Field2\", T.\"Field3\" = S.\"Field3\" " +
                $"WHEN NOT MATCHED THEN " +
                $"INSERT ( \"Field1\", \"Field2\", \"Field3\" ) " +
                $"VALUES ( S.\"Field1\", S.\"Field2\", S.\"Field3\" ) ;";

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

        #endregion
    }
}
