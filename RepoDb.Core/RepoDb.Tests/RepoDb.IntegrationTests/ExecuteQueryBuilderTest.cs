using System;
using Microsoft.Data.SqlClient;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using RepoDb.Enumerations;
using System.Collections.Generic;
using RepoDb.Exceptions;

namespace RepoDb.IntegrationTests
{
    [TestClass]
    public class ExecuteQueryBuilderTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Database.Initialize();
            Cleanup();
        }

        [TestCleanup]
        public void Cleanup()
        {
            Database.Cleanup();
        }

        #region Specialized

        #region In

        /*
         * Array
         */

        [TestMethod]
        public void TestSqlConnectionExecuteQueryFromQueryBuilderCreateQueryForInOperationViaArray()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new long[] { 1, 3, 4, 8 };
            var where = new QueryGroup(new QueryField("Id", Operation.In, values));
            var fields = FieldCache.Get<IdentityTable>();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var builder = connection.GetStatementBuilder();
                var sql = builder.CreateQuery(null,
                    ClassMappedNameCache.Get<IdentityTable>(),
                    fields: fields,
                    where: where);

                // Act
                var result = connection.ExecuteQuery<IdentityTable>(sql, where);

                // Assert
                Assert.AreEqual(4, result.Count());
                result.AsList().ForEach(item =>
                {
                    Assert.IsTrue(values.Contains(item.Id));
                    Helper.AssertPropertiesEquality(tables.First(v => v.Id == item.Id), item);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryFromQueryBuilderCreateQueryForNotInOperationViaArray()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new long[] { 1, 3, 4, 8 };
            var where = new QueryGroup(new QueryField("Id", Operation.NotIn, values));
            var fields = FieldCache.Get<IdentityTable>();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var builder = connection.GetStatementBuilder();
                var sql = builder.CreateQuery(null,
                    ClassMappedNameCache.Get<IdentityTable>(),
                    fields: fields,
                    where: where);

                // Act
                var result = connection.ExecuteQuery<IdentityTable>(sql, where);

                // Assert
                Assert.AreEqual(6, result.Count());
                result.AsList().ForEach(item =>
                {
                    Assert.IsFalse(values.Contains(item.Id));
                    Helper.AssertPropertiesEquality(tables.First(v => v.Id == item.Id), item);
                });
            }
        }

        /*
         * List
         */

        [TestMethod]
        public void TestSqlConnectionExecuteQueryFromQueryBuilderCreateQueryForInOperationViaList()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new List<long> { 1, 3, 4, 8 };
            var where = new QueryGroup(new QueryField("Id", Operation.In, values));
            var fields = FieldCache.Get<IdentityTable>();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var builder = connection.GetStatementBuilder();
                var sql = builder.CreateQuery(null,
                    ClassMappedNameCache.Get<IdentityTable>(),
                    fields: fields,
                    where: where);

                // Act
                var result = connection.ExecuteQuery<IdentityTable>(sql, where);

                // Assert
                Assert.AreEqual(4, result.Count());
                result.AsList().ForEach(item =>
                {
                    Assert.IsTrue(values.Contains(item.Id));
                    Helper.AssertPropertiesEquality(tables.First(v => v.Id == item.Id), item);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryFromQueryBuilderCreateQueryForNotInOperationViaList()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new List<long> { 1, 3, 4, 8 };
            var where = new QueryGroup(new QueryField("Id", Operation.NotIn, values));
            var fields = FieldCache.Get<IdentityTable>();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var builder = connection.GetStatementBuilder();
                var sql = builder.CreateQuery(null,
                    ClassMappedNameCache.Get<IdentityTable>(),
                    fields: fields,
                    where: where);

                // Act
                var result = connection.ExecuteQuery<IdentityTable>(sql, where);

                // Assert
                Assert.AreEqual(6, result.Count());
                result.AsList().ForEach(item =>
                {
                    Assert.IsFalse(values.Contains(item.Id));
                    Helper.AssertPropertiesEquality(tables.First(v => v.Id == item.Id), item);
                });
            }
        }

        #endregion

        #region Between

        /*
         * Array
         */

        [TestMethod]
        public void TestSqlConnectionExecuteQueryFromQueryBuilderCreateQueryForBetweenOperationViaArray()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new long[] { 3, 7 };
            var where = new QueryGroup(new QueryField("Id", Operation.Between, values));
            var fields = FieldCache.Get<IdentityTable>();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var builder = connection.GetStatementBuilder();
                var sql = builder.CreateQuery(null,
                    ClassMappedNameCache.Get<IdentityTable>(),
                    fields: fields,
                    where: where);

                // Act
                var result = connection.ExecuteQuery<IdentityTable>(sql, where);

                // Assert
                Assert.AreEqual(5, result.Count());
                result.AsList().ForEach(item =>
                {
                    Helper.AssertPropertiesEquality(tables.First(v => v.Id == item.Id), item);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryFromQueryBuilderCreateQueryForNotBetweenOperationViaArray()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new long[] { 3, 7 };
            var where = new QueryGroup(new QueryField("Id", Operation.NotBetween, values));
            var fields = FieldCache.Get<IdentityTable>();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var builder = connection.GetStatementBuilder();
                var sql = builder.CreateQuery(null,
                    ClassMappedNameCache.Get<IdentityTable>(),
                    fields: fields,
                    where: where);

                // Act
                var result = connection.ExecuteQuery<IdentityTable>(sql, where);

                // Assert
                Assert.AreEqual(5, result.Count());
                result.AsList().ForEach(item =>
                {
                    Helper.AssertPropertiesEquality(tables.First(v => v.Id == item.Id), item);
                });
            }
        }

        [TestMethod, ExpectedException(typeof(InvalidParameterException))]
        public void ThrowExceptionOnSqlConnectionExecuteQueryFromQueryBuilderCreateQueryForNotBetweenOperationViaList()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new List<long> { 1, 3, 7 };
            var where = new QueryGroup(new QueryField("Id", Operation.NotBetween, values));
            var fields = FieldCache.Get<IdentityTable>();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var builder = connection.GetStatementBuilder();
                var sql = builder.CreateQuery(null,
                    ClassMappedNameCache.Get<IdentityTable>(),
                    fields: fields,
                    where: where);

                // Act
                connection.ExecuteQuery<IdentityTable>(sql, where);
            }
        }

        [TestMethod, ExpectedException(typeof(InvalidParameterException))]
        public void ThrowExceptionOnSqlConnectionExecuteQueryFromQueryBuilderCreateQueryForNotBetweenOperationViaArrayWithEmptyValues()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new long[0];
            var where = new QueryGroup(new QueryField("Id", Operation.NotBetween, values));
            var fields = FieldCache.Get<IdentityTable>();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var builder = connection.GetStatementBuilder();
                var sql = builder.CreateQuery(null,
                    ClassMappedNameCache.Get<IdentityTable>(),
                    fields: fields,
                    where: where);

                // Act
                connection.ExecuteQuery<IdentityTable>(sql, where);
            }
        }

        [TestMethod, ExpectedException(typeof(InvalidParameterException))]
        public void ThrowExceptionOnSqlConnectionExecuteQueryFromQueryBuilderCreateQueryForNotBetweenOperationViaArrayWithLessValues()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new long[] { 1 };
            var where = new QueryGroup(new QueryField("Id", Operation.NotBetween, values));
            var fields = FieldCache.Get<IdentityTable>();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var builder = connection.GetStatementBuilder();
                var sql = builder.CreateQuery(null,
                    ClassMappedNameCache.Get<IdentityTable>(),
                    fields: fields,
                    where: where);

                // Act
                connection.ExecuteQuery<IdentityTable>(sql, where);
            }
        }

        [TestMethod, ExpectedException(typeof(InvalidParameterException))]
        public void ThrowExceptionOnSqlConnectionExecuteQueryFromQueryBuilderCreateQueryForNotBetweenOperationViaArrayWithMoreVaues()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new long[] { 1, 3, 7 };
            var where = new QueryGroup(new QueryField("Id", Operation.NotBetween, values));
            var fields = FieldCache.Get<IdentityTable>();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var builder = connection.GetStatementBuilder();
                var sql = builder.CreateQuery(null,
                    ClassMappedNameCache.Get<IdentityTable>(),
                    fields: fields,
                    where: where);

                // Act
                connection.ExecuteQuery<IdentityTable>(sql, where);
            }
        }

        /*
         * List
         */

        [TestMethod]
        public void TestSqlConnectionExecuteQueryFromQueryBuilderCreateQueryForBetweenOperationViaList()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new List<long> { 3, 7 };
            var where = new QueryGroup(new QueryField("Id", Operation.Between, values));
            var fields = FieldCache.Get<IdentityTable>();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var builder = connection.GetStatementBuilder();
                var sql = builder.CreateQuery(null,
                    ClassMappedNameCache.Get<IdentityTable>(),
                    fields: fields,
                    where: where);

                // Act
                var result = connection.ExecuteQuery<IdentityTable>(sql, where);

                // Assert
                Assert.AreEqual(5, result.Count());
                result.AsList().ForEach(item =>
                {
                    Helper.AssertPropertiesEquality(tables.First(v => v.Id == item.Id), item);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryFromQueryBuilderCreateQueryForNotBetweenOperationViaList()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new List<long> { 3, 7 };
            var where = new QueryGroup(new QueryField("Id", Operation.NotBetween, values));
            var fields = FieldCache.Get<IdentityTable>();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var builder = connection.GetStatementBuilder();
                var sql = builder.CreateQuery(null,
                    ClassMappedNameCache.Get<IdentityTable>(),
                    fields: fields,
                    where: where);

                // Act
                var result = connection.ExecuteQuery<IdentityTable>(sql, where);

                // Assert
                Assert.AreEqual(5, result.Count());
                result.AsList().ForEach(item =>
                {
                    Helper.AssertPropertiesEquality(tables.First(v => v.Id == item.Id), item);
                });
            }
        }

        [TestMethod, ExpectedException(typeof(InvalidParameterException))]
        public void ThrowExceptionOnSqlConnectionExecuteQueryFromQueryBuilderCreateQueryForNotBetweenOperationViaListWithEmptyValues()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new List<long>();
            var where = new QueryGroup(new QueryField("Id", Operation.NotBetween, values));
            var fields = FieldCache.Get<IdentityTable>();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var builder = connection.GetStatementBuilder();
                var sql = builder.CreateQuery(null,
                    ClassMappedNameCache.Get<IdentityTable>(),
                    fields: fields,
                    where: where);

                // Act
                connection.ExecuteQuery<IdentityTable>(sql, where);
            }
        }

        [TestMethod, ExpectedException(typeof(InvalidParameterException))]
        public void ThrowExceptionOnSqlConnectionExecuteQueryFromQueryBuilderCreateQueryForNotBetweenOperationViaListWithLessValues()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new List<long> { 1 };
            var where = new QueryGroup(new QueryField("Id", Operation.NotBetween, values));
            var fields = FieldCache.Get<IdentityTable>();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var builder = connection.GetStatementBuilder();
                var sql = builder.CreateQuery(null,
                    ClassMappedNameCache.Get<IdentityTable>(),
                    fields: fields,
                    where: where);

                // Act
                connection.ExecuteQuery<IdentityTable>(sql, where);
            }
        }

        [TestMethod, ExpectedException(typeof(InvalidParameterException))]
        public void ThrowExceptionOnSqlConnectionExecuteQueryFromQueryBuilderCreateQueryForNotBetweenOperationViaListWithMoreVaues()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var values = new List<long> { 1, 3, 7 };
            var where = new QueryGroup(new QueryField("Id", Operation.NotBetween, values));
            var fields = FieldCache.Get<IdentityTable>();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var builder = connection.GetStatementBuilder();
                var sql = builder.CreateQuery(null,
                    ClassMappedNameCache.Get<IdentityTable>(),
                    fields: fields,
                    where: where);

                // Act
                connection.ExecuteQuery<IdentityTable>(sql, where);
            }
        }

        /*
         * Shared
         */

        [TestMethod, ExpectedException(typeof(InvalidParameterException))]
        public void ThrowExceptionOnSqlConnectionExecuteQueryFromQueryBuilderCreateQueryForNotBetweenOperationWithNullValues()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var where = new QueryGroup(new QueryField("Id", Operation.NotBetween, null));
            var fields = FieldCache.Get<IdentityTable>();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var builder = connection.GetStatementBuilder();
                var sql = builder.CreateQuery(null,
                    ClassMappedNameCache.Get<IdentityTable>(),
                    fields: fields,
                    where: where);

                // Act
                connection.ExecuteQuery<IdentityTable>(sql, where);
            }
        }

        #endregion

        #endregion

        #region Operations

        #region Average

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryFromQueryBuilderCreateAverage()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var where = new QueryGroup(new QueryField("Id", Operation.GreaterThanOrEqual, 0));

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var builder = connection.GetStatementBuilder();
                var sql = builder.CreateAverage(null,
                    ClassMappedNameCache.Get<IdentityTable>(),
                    field: Field.Parse<IdentityTable>(e => e.ColumnInt).First(),
                    where: where);

                // Act
                var result = connection.ExecuteScalar<double>(sql, where);

                // Assert
                Assert.AreEqual(tables.Average(e => e.ColumnInt), result);
            }
        }

        #endregion

        #region AverageAll

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryFromQueryBuilderCreateAverageAll()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var builder = connection.GetStatementBuilder();
                var sql = builder.CreateAverageAll(null,
                    ClassMappedNameCache.Get<IdentityTable>(),
                    field: Field.Parse<IdentityTable>(e => e.ColumnInt).First());

                // Act
                var result = connection.ExecuteScalar<double>(sql);

                // Assert
                Assert.AreEqual(tables.Average(e => e.ColumnInt), result);
            }
        }

        #endregion

        #region BatchQuery

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryFromQueryBuilderCreateBatchQuery()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var where = new QueryGroup(new QueryField("Id", Operation.GreaterThanOrEqual, 0));
            var fields = FieldCache.Get<IdentityTable>();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var builder = connection.GetStatementBuilder();
                var sql = builder.CreateBatchQuery(null,
                    ClassMappedNameCache.Get<IdentityTable>(),
                    fields: fields,
                    page: 2,
                    rowsPerBatch: 2,
                    orderBy: OrderField.Ascending<IdentityTable>(e => e.Id).AsEnumerable(),
                    where: where);

                // Act
                var result = connection.ExecuteQuery<IdentityTable>(sql, where);

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(item =>
                {
                    Helper.AssertPropertiesEquality(tables.First(v => v.Id == item.Id), item);
                });
            }
        }

        #endregion

        #region Count

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryFromQueryBuilderCreateCount()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var where = new QueryGroup(new QueryField("Id", Operation.GreaterThanOrEqual, 4));

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var builder = connection.GetStatementBuilder();
                var sql = builder.CreateCount(null,
                    ClassMappedNameCache.Get<IdentityTable>(),
                    where: where);

                // Act
                var result = connection.ExecuteScalar<int>(sql, where);

                // Assert
                Assert.AreEqual(tables.Count(e => e.Id >= 4), result);
            }
        }

        #endregion

        #region CountAll

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryFromQueryBuilderCreateCountAll()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var builder = connection.GetStatementBuilder();
                var sql = builder.CreateCountAll(null,
                    ClassMappedNameCache.Get<IdentityTable>());

                // Act
                var result = connection.ExecuteScalar<int>(sql);

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        #endregion

        #region Delete

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryFromQueryBuilderCreateDelete()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var where = new QueryGroup(new QueryField("Id", Operation.GreaterThanOrEqual, 4));

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Assert
                Assert.AreEqual(tables.Count(), connection.CountAll<IdentityTable>());

                // Setup
                var builder = connection.GetStatementBuilder();
                var sql = builder.CreateDelete(null,
                    ClassMappedNameCache.Get<IdentityTable>(),
                    where: where);

                // Act
                var result = connection.ExecuteNonQuery(sql, where);

                // Assert
                Assert.AreEqual(7, result);
                Assert.AreEqual(3, connection.CountAll<IdentityTable>());
            }
        }

        #endregion

        #region CountAll

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryFromQueryBuilderCreateDeleteAll()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Assert
                Assert.AreEqual(tables.Count(), connection.CountAll<IdentityTable>());

                // Setup
                var builder = connection.GetStatementBuilder();
                var sql = builder.CreateDeleteAll(null,
                    ClassMappedNameCache.Get<IdentityTable>());

                // Act
                var result = connection.ExecuteNonQuery(sql);

                // Assert
                Assert.AreEqual(tables.Count(), result);
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
            }
        }

        #endregion

        #region Exists

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryFromQueryBuilderCreateExists()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var where = new QueryGroup(new QueryField("Id", tables.Last().Id));
                var builder = connection.GetStatementBuilder();
                var sql = builder.CreateExists(null,
                    ClassMappedNameCache.Get<IdentityTable>(),
                    where: where);

                // Act
                var result = connection.ExecuteScalar<bool>(sql, where);

                // Assert
                Assert.IsTrue(result);
            }
        }

        #endregion

        #region Insert

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryFromQueryBuilderCreateInsert()
        {
            // Setup
            var table = Helper.CreateIdentityTables(1).First();
            var fields = FieldCache.Get<IdentityTable>();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var dbFields = DbFieldCache.Get(connection, ClassMappedNameCache.Get<IdentityTable>(), null);
                var builder = connection.GetStatementBuilder();
                var sql = builder.CreateInsert(null,
                    ClassMappedNameCache.Get<IdentityTable>(),
                    fields: fields,
                    primaryField: dbFields.FirstOrDefault(e => e.IsPrimary),
                    identityField: dbFields.FirstOrDefault(e => e.IsIdentity));

                // Act
                var id = connection.ExecuteScalar(sql, table);

                // Assert
                Assert.IsNotNull(id);

                // Setup
                var result = connection.QueryAll<IdentityTable>().First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        #endregion

        #region Max

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryFromQueryBuilderCreateMax()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var where = new QueryGroup(new QueryField("Id", Operation.GreaterThanOrEqual, 0));

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var builder = connection.GetStatementBuilder();
                var sql = builder.CreateMax(null,
                    ClassMappedNameCache.Get<IdentityTable>(),
                    field: Field.Parse<IdentityTable>(e => e.ColumnInt).First(),
                    where: where);

                // Act
                var result = connection.ExecuteScalar<int>(sql, where);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id >= 0).Max(e => e.ColumnInt), result);
            }
        }

        #endregion

        #region MaxAll

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryFromQueryBuilderCreateMaxAll()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var builder = connection.GetStatementBuilder();
                var sql = builder.CreateMaxAll(null,
                    ClassMappedNameCache.Get<IdentityTable>(),
                    field: Field.Parse<IdentityTable>(e => e.ColumnInt).First());

                // Act
                var result = connection.ExecuteScalar<int>(sql);

                // Assert
                Assert.AreEqual(tables.Max(e => e.ColumnInt), result);
            }
        }

        #endregion

        #region Merge

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryFromQueryBuilderCreateMerge()
        {
            // Setup
            var table = Helper.CreateIdentityTables(1).First();
            var fields = FieldCache.Get<IdentityTable>();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.Insert(table);

                // Set the properties
                table.ColumnNVarChar = $"{table.ColumnNVarChar}-Merged";

                // Setup
                var dbFields = DbFieldCache.Get(connection, ClassMappedNameCache.Get<IdentityTable>(), null);
                var builder = connection.GetStatementBuilder();
                var sql = builder.CreateMerge(null,
                    ClassMappedNameCache.Get<IdentityTable>(),
                    fields: fields,
                    qualifiers: fields.Where(f => dbFields.FirstOrDefault(df => (df.IsPrimary || df.IsIdentity) && df.Name == f.Name) != null),
                    primaryField: dbFields.FirstOrDefault(e => e.IsPrimary),
                    identityField: dbFields.FirstOrDefault(e => e.IsIdentity));

                // Act
                var affectedRow = connection.ExecuteNonQuery(sql, table);

                // Assert
                Assert.AreEqual(1, affectedRow);

                // Setup
                var result = connection.QueryAll<IdentityTable>().First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        #endregion

        #region Min

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryFromQueryBuilderCreateMin()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var where = new QueryGroup(new QueryField("Id", Operation.GreaterThanOrEqual, 6));

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var builder = connection.GetStatementBuilder();
                var sql = builder.CreateMin(null,
                    ClassMappedNameCache.Get<IdentityTable>(),
                    field: Field.Parse<IdentityTable>(e => e.ColumnInt).First(),
                    where: where);

                // Act
                var result = connection.ExecuteScalar<int>(sql, where);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id >= 6).Min(e => e.ColumnInt), result);
            }
        }

        #endregion

        #region MinAll

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryFromQueryBuilderCreateMinAll()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var builder = connection.GetStatementBuilder();
                var sql = builder.CreateMinAll(null,
                    ClassMappedNameCache.Get<IdentityTable>(),
                    field: Field.Parse<IdentityTable>(e => e.ColumnInt).First());

                // Act
                var result = connection.ExecuteScalar<int>(sql);

                // Assert
                Assert.AreEqual(tables.Min(e => e.ColumnInt), result);
            }
        }

        #endregion

        #region Query

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryFromQueryBuilderCreateQueryForIn()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var where = new QueryGroup(new QueryField("Id", Operation.In, new[] { 4, 6 }));
            var fields = FieldCache.Get<IdentityTable>();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var builder = connection.GetStatementBuilder();
                var sql = builder.CreateQuery(null,
                    ClassMappedNameCache.Get<IdentityTable>(),
                    fields: fields,
                    where: where);

                // Act
                var result = connection.ExecuteQuery<IdentityTable>(sql, where);

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(item =>
                {
                    Helper.AssertPropertiesEquality(tables.First(v => v.Id == item.Id), item);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryFromQueryBuilderCreateQueryForOr()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var where = new QueryGroup(new[]
                {
                    new QueryField("Id", 1),
                    new QueryField("Id", 9)
                },
                Conjunction.Or);
            var fields = FieldCache.Get<IdentityTable>();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var builder = connection.GetStatementBuilder();
                var sql = builder.CreateQuery(null,
                    ClassMappedNameCache.Get<IdentityTable>(),
                    fields: fields,
                    where: where);

                // Act
                var result = connection.ExecuteQuery<IdentityTable>(sql, where);

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(item =>
                {
                    Helper.AssertPropertiesEquality(tables.First(v => v.Id == item.Id), item);
                });
            }
        }

        #endregion

        #region QueryAll

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryFromQueryBuilderCreateQueryAll()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = FieldCache.Get<IdentityTable>();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var builder = connection.GetStatementBuilder();
                var sql = builder.CreateQueryAll(null,
                    ClassMappedNameCache.Get<IdentityTable>(),
                    fields: fields);

                // Act
                var result = connection.ExecuteQuery<IdentityTable>(sql);

                // Assert
                Assert.AreEqual(tables.Count(), result.Count());
                result.AsList().ForEach(item =>
                {
                    Helper.AssertPropertiesEquality(tables.First(v => v.Id == item.Id), item);
                });
            }
        }

        #endregion

        #region Sum

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryFromQueryBuilderCreateSum()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var where = new QueryGroup(new QueryField("Id", Operation.GreaterThanOrEqual, 6));

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var builder = connection.GetStatementBuilder();
                var sql = builder.CreateSum(null,
                    ClassMappedNameCache.Get<IdentityTable>(),
                    field: Field.Parse<IdentityTable>(e => e.ColumnInt).First(),
                    where: where);

                // Act
                var result = connection.ExecuteScalar<int>(sql, where);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id >= 6).Sum(e => e.ColumnInt), result);
            }
        }

        #endregion

        #region SumAll

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryFromQueryBuilderCreateSumAll()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var builder = connection.GetStatementBuilder();
                var sql = builder.CreateSumAll(null,
                    ClassMappedNameCache.Get<IdentityTable>(),
                    field: Field.Parse<IdentityTable>(e => e.ColumnInt).First());

                // Act
                var result = connection.ExecuteScalar<int>(sql);

                // Assert
                Assert.AreEqual(tables.Sum(e => e.ColumnInt), result);
            }
        }

        #endregion

        #region Truncate

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryFromQueryBuilderCreateTruncate()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Assert
                Assert.AreEqual(tables.Count(), connection.CountAll<IdentityTable>());

                // Setup
                var builder = connection.GetStatementBuilder();
                var sql = builder.CreateTruncate(null,
                    ClassMappedNameCache.Get<IdentityTable>());

                // Act
                connection.ExecuteNonQuery(sql);

                // Assert
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
            }
        }

        #endregion

        #region Update

        [TestMethod]
        public void TestSqlConnectionExecuteNonQueryFromQueryBuilderCreateUpdate()
        {
            // Setup
            var table = Helper.CreateIdentityTables(1).First();
            var fields = FieldCache.Get<IdentityTable>();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.Insert(table);

                // Set the properties
                table.ColumnNVarChar = $"{table.ColumnNVarChar}-Updated";

                // Setup
                var where = new QueryGroup(new QueryField("Id", id));

                // Setup
                var dbFields = DbFieldCache.Get(connection, ClassMappedNameCache.Get<IdentityTable>(), null);
                var builder = connection.GetStatementBuilder();
                var sql = builder.CreateUpdate(null,
                    ClassMappedNameCache.Get<IdentityTable>(),
                    fields: fields,
                    where: where,
                    primaryField: dbFields.FirstOrDefault(e => e.IsPrimary),
                    identityField: dbFields.FirstOrDefault(e => e.IsIdentity));

                // Act
                var affectedRow = connection.ExecuteNonQuery(sql, table);

                // Assert
                Assert.AreEqual(1, affectedRow);

                // Setup
                var result = connection.QueryAll<IdentityTable>().First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        #endregion

        #endregion
    }
}
