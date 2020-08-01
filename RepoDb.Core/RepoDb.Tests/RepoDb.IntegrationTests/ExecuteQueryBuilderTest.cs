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
    }
}
