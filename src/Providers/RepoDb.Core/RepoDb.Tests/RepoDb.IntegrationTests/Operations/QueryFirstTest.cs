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
    public class QueryFirstTest
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

        #region QueryFirst<TEntity>

        [TestMethod]
        public void TestSqlConnectionQueryFirstWithDynamicWhat()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryFirst<IdentityTable>(new { tables.Last().Id });

                // Assert
                Helper.AssertPropertiesEquality(tables.Last(), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryFirstWithExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryFirst<IdentityTable>(e => e.Id == tables.Last().Id);

                // Assert
                Helper.AssertPropertiesEquality(tables.Last(), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryFirstWithQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryFirst<IdentityTable>(new QueryField("Id", tables.Last().Id));

                // Assert
                Helper.AssertPropertiesEquality(tables.Last(), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryFirstWithQueryGroup()
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
                var result = connection.QueryFirst<IdentityTable>(where);

                // Assert
                Helper.AssertPropertiesEquality(last, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryFirstWithEntityTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryFirst<IdentityTable>(ClassMappedNameCache.Get<IdentityTable>(),
                    new QueryField("Id", tables.Last().Id));

                // Assert
                Helper.AssertPropertiesEquality(tables.Last(), result);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqlConnectionQueryFirstIfNoRowsFound()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.QueryFirst<IdentityTable>((object)null));
            }
        }

        #endregion

        #region QueryFirstAsync<TEntity>

        [TestMethod]
        public async Task TestSqlConnectionQueryFirstAsyncWithDynamicWhat()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.QueryFirstAsync<IdentityTable>(new { tables.Last().Id }).ConfigureAwait(false);

                // Assert
                Helper.AssertPropertiesEquality(tables.Last(), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionQueryFirstAsyncWithExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.QueryFirstAsync<IdentityTable>(e => e.Id == tables.Last().Id).ConfigureAwait(false);

                // Assert
                Helper.AssertPropertiesEquality(tables.Last(), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionQueryFirstAsyncWithQueryGroup()
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
                var result = await connection.QueryFirstAsync<IdentityTable>(where).ConfigureAwait(false);

                // Assert
                Helper.AssertPropertiesEquality(last, result);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqlConnectionQueryFirstAsyncIfNoRowsFound()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.QueryFirstAsync<IdentityTable>((object)null).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        #endregion

        #region QueryFirst(TableName)

        [TestMethod]
        public void TestSqlConnectionQueryFirstViaDynamicsWithDynamicWhat()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryFirst(ClassMappedNameCache.Get<IdentityTable>(),
                    new { tables.Last().Id });

                // Assert
                var kvp = (System.Collections.Generic.IDictionary<string, object>)result;
                Assert.AreEqual(tables.Last().Id, kvp[nameof(IdentityTable.Id)]);
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryFirstViaDynamicsWithQueryGroup()
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
                var result = connection.QueryFirst(ClassMappedNameCache.Get<IdentityTable>(), where);

                // Assert
                var kvp = (System.Collections.Generic.IDictionary<string, object>)result;
                Assert.AreEqual(last.Id, kvp[nameof(IdentityTable.Id)]);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqlConnectionQueryFirstViaDynamicsIfNoRowsFound()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.QueryFirst(ClassMappedNameCache.Get<IdentityTable>(), (object)null));
            }
        }

        #endregion

        #region QueryFirstAsync(TableName)

        [TestMethod]
        public async Task TestSqlConnectionQueryFirstAsyncViaDynamicsWithDynamicWhat()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.QueryFirstAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new { tables.Last().Id }).ConfigureAwait(false);

                // Assert
                var kvp = (System.Collections.Generic.IDictionary<string, object>)result;
                Assert.AreEqual(tables.Last().Id, kvp[nameof(IdentityTable.Id)]);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionQueryFirstAsyncViaDynamicsWithQueryGroup()
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
                var result = await connection.QueryFirstAsync(ClassMappedNameCache.Get<IdentityTable>(), where).ConfigureAwait(false);

                // Assert
                var kvp = (System.Collections.Generic.IDictionary<string, object>)result;
                Assert.AreEqual(last.Id, kvp[nameof(IdentityTable.Id)]);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqlConnectionQueryFirstAsyncViaDynamicsIfNoRowsFound()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.QueryFirstAsync(ClassMappedNameCache.Get<IdentityTable>(), (object)null).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        #endregion
    }
}
