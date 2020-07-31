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
                var sql = builder.CreateQuery(null, ClassMappedNameCache.Get<IdentityTable>(), fields: fields, where: where);

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
                var sql = builder.CreateQuery(null, ClassMappedNameCache.Get<IdentityTable>(), fields: fields, where: where);

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
                var sql = builder.CreateQuery(null, ClassMappedNameCache.Get<IdentityTable>(), fields: fields, where: where);

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
                var sql = builder.CreateQuery(null, ClassMappedNameCache.Get<IdentityTable>(), fields: fields, where: where);

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
                var sql = builder.CreateQuery(null, ClassMappedNameCache.Get<IdentityTable>(), fields: fields, where: where);

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
                var sql = builder.CreateQuery(null, ClassMappedNameCache.Get<IdentityTable>(), fields: fields, where: where);

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
                var sql = builder.CreateQuery(null, ClassMappedNameCache.Get<IdentityTable>(), fields: fields, where: where);

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
                var sql = builder.CreateQuery(null, ClassMappedNameCache.Get<IdentityTable>(), fields: fields, where: where);

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
                var sql = builder.CreateQuery(null, ClassMappedNameCache.Get<IdentityTable>(), fields: fields, where: where);

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
                var sql = builder.CreateQuery(null, ClassMappedNameCache.Get<IdentityTable>(), fields: fields, where: where);

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
                var sql = builder.CreateQuery(null, ClassMappedNameCache.Get<IdentityTable>(), fields: fields, where: where);

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
                var sql = builder.CreateQuery(null, ClassMappedNameCache.Get<IdentityTable>(), fields: fields, where: where);

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
                var sql = builder.CreateQuery(null, ClassMappedNameCache.Get<IdentityTable>(), fields: fields, where: where);

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
                var sql = builder.CreateQuery(null, ClassMappedNameCache.Get<IdentityTable>(), fields: fields, where: where);

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
                var sql = builder.CreateQuery(null, ClassMappedNameCache.Get<IdentityTable>(), fields: fields, where: where);

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
                var sql = builder.CreateQuery(null, ClassMappedNameCache.Get<IdentityTable>(), fields: fields, where: where);

                // Act
                connection.ExecuteQuery<IdentityTable>(sql, where);
            }
        }

        #endregion
    }
}
