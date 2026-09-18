#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.IntegrationTests.Operations
{
    [TestClass]
    public class QuerySingleTest
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

        #region QuerySingle<TEntity>

        [TestMethod]
        public void TestSqlConnectionQuerySingleWithDynamicWhat()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QuerySingle<IdentityTable>(new { tables.Last().Id });

                // Assert
                Helper.AssertPropertiesEquality(tables.Last(), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionQuerySingleWithExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QuerySingle<IdentityTable>(e => e.Id == tables.Last().Id);

                // Assert
                Helper.AssertPropertiesEquality(tables.Last(), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionQuerySingleWithQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QuerySingle<IdentityTable>(new QueryField("Id", tables.Last().Id));

                // Assert
                Helper.AssertPropertiesEquality(tables.Last(), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionQuerySingleWithQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();
            var where = new QueryGroup(new[]
            {
                new QueryField("ColumnInt", last.ColumnInt),
                new QueryField("ColumnBit", last.ColumnBit)
            });

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QuerySingle<IdentityTable>(where);

                // Assert
                Helper.AssertPropertiesEquality(last, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionQuerySingleWithEntityTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QuerySingle<IdentityTable>(ClassMappedNameCache.Get<IdentityTable>(),
                    new QueryField("Id", tables.Last().Id));

                // Assert
                Helper.AssertPropertiesEquality(tables.Last(), result);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqlConnectionQuerySingleIfNoRowsFound()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.QuerySingle<IdentityTable>((object)null));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqlConnectionQuerySingleIfMultipleRowsFound()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                Assert.Throws<MultipleRowsFoundException>(() => connection.QuerySingle<IdentityTable>((object)null));
            }
        }

        #endregion

        #region QuerySingleAsync<TEntity>

        [TestMethod]
        public async Task TestSqlConnectionQuerySingleAsyncWithDynamicWhat()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.QuerySingleAsync<IdentityTable>(new { tables.Last().Id }).ConfigureAwait(false);

                // Assert
                Helper.AssertPropertiesEquality(tables.Last(), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionQuerySingleAsyncWithExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.QuerySingleAsync<IdentityTable>(e => e.Id == tables.Last().Id).ConfigureAwait(false);

                // Assert
                Helper.AssertPropertiesEquality(tables.Last(), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionQuerySingleAsyncWithQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();
            var where = new QueryGroup(new[]
            {
                new QueryField("ColumnInt", last.ColumnInt),
                new QueryField("ColumnBit", last.ColumnBit)
            });

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.QuerySingleAsync<IdentityTable>(where).ConfigureAwait(false);

                // Assert
                Helper.AssertPropertiesEquality(last, result);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqlConnectionQuerySingleAsyncIfNoRowsFound()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.QuerySingleAsync<IdentityTable>((object)null).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqlConnectionQuerySingleAsyncIfMultipleRowsFound()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                await Assert.ThrowsAsync<MultipleRowsFoundException>(async () => await connection.QuerySingleAsync<IdentityTable>((object)null).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        #endregion

        #region QuerySingle(TableName)

        [TestMethod]
        public void TestSqlConnectionQuerySingleViaDynamicsWithDynamicWhat()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QuerySingle(ClassMappedNameCache.Get<IdentityTable>(),
                    new { tables.Last().Id });

                // Assert
                var kvp = (System.Collections.Generic.IDictionary<string, object>)result;
                Assert.AreEqual(tables.Last().Id, kvp[nameof(IdentityTable.Id)]);
            }
        }

        [TestMethod]
        public void TestSqlConnectionQuerySingleViaDynamicsWithQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();
            var where = new QueryGroup(new[]
            {
                new QueryField("ColumnInt", last.ColumnInt),
                new QueryField("ColumnBit", last.ColumnBit)
            });

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QuerySingle(ClassMappedNameCache.Get<IdentityTable>(), where);

                // Assert
                var kvp = (System.Collections.Generic.IDictionary<string, object>)result;
                Assert.AreEqual(last.Id, kvp[nameof(IdentityTable.Id)]);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqlConnectionQuerySingleViaDynamicsIfNoRowsFound()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.QuerySingle(ClassMappedNameCache.Get<IdentityTable>(), (object)null));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqlConnectionQuerySingleViaDynamicsIfMultipleRowsFound()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                Assert.Throws<MultipleRowsFoundException>(() => connection.QuerySingle(ClassMappedNameCache.Get<IdentityTable>(), (object)null));
            }
        }

        #endregion

        #region QuerySingleAsync(TableName)

        [TestMethod]
        public async Task TestSqlConnectionQuerySingleAsyncViaDynamicsWithDynamicWhat()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.QuerySingleAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new { tables.Last().Id }).ConfigureAwait(false);

                // Assert
                var kvp = (System.Collections.Generic.IDictionary<string, object>)result;
                Assert.AreEqual(tables.Last().Id, kvp[nameof(IdentityTable.Id)]);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionQuerySingleAsyncViaDynamicsWithQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();
            var where = new QueryGroup(new[]
            {
                new QueryField("ColumnInt", last.ColumnInt),
                new QueryField("ColumnBit", last.ColumnBit)
            });

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.QuerySingleAsync(ClassMappedNameCache.Get<IdentityTable>(), where).ConfigureAwait(false);

                // Assert
                var kvp = (System.Collections.Generic.IDictionary<string, object>)result;
                Assert.AreEqual(last.Id, kvp[nameof(IdentityTable.Id)]);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqlConnectionQuerySingleAsyncViaDynamicsIfNoRowsFound()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.QuerySingleAsync(ClassMappedNameCache.Get<IdentityTable>(), (object)null).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqlConnectionQuerySingleAsyncViaDynamicsIfMultipleRowsFound()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                await Assert.ThrowsAsync<MultipleRowsFoundException>(async () => await connection.QuerySingleAsync(ClassMappedNameCache.Get<IdentityTable>(), (object)null).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        #endregion
    }
}
