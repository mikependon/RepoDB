#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Data.SqlClient;
using RepoDb.Enumerations;
using RepoDb.Exceptions;
using RepoDb.SqlServer.IntegrationTests.Models;
using RepoDb.SqlServer.IntegrationTests.Setup;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.SqlServer.IntegrationTests.Operations
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

        #region DataEntity

        #region Sync

        [TestMethod]
        public void TestSqlServerConnectionQueryFirstViaPrimaryKey()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(10).Last();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QueryFirst<IdentityCompleteTable>(table.Id);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionQueryFirstViaExpression()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(10).Last();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QueryFirst<IdentityCompleteTable>(e => e.Id == table.Id);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionQueryFirstViaDynamic()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(10).Last();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QueryFirst<IdentityCompleteTable>(new { table.Id });

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionQueryFirstViaQueryField()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(10).Last();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QueryFirst<IdentityCompleteTable>(new QueryField("Id", table.Id));

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionQueryFirstViaQueryFields()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(10).Last();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QueryFirst<IdentityCompleteTable>(queryFields);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionQueryFirstViaQueryGroup()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(10).Last();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QueryFirst<IdentityCompleteTable>(new QueryGroup(queryFields));

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionQueryFirstWithOrderBy()
        {
            // Setup
            var tables = Database.CreateIdentityCompleteTables(10).ToList();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var ascending = connection.QueryFirst<IdentityCompleteTable>((object)null, orderBy: new[] { new OrderField("Id", Order.Ascending) });
                var descending = connection.QueryFirst<IdentityCompleteTable>((object)null, orderBy: new[] { new OrderField("Id", Order.Descending) });

                // Assert
                Assert.AreEqual(tables.First().Id, ascending.Id);
                Helper.AssertPropertiesEquality(tables.First(), ascending);
                Assert.AreEqual(tables.Last().Id, descending.Id);
                Helper.AssertPropertiesEquality(tables.Last(), descending);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqlServerConnectionQueryFirstIfNoRowsFound()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.QueryFirst<IdentityCompleteTable>((object)null));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqlServerConnectionQueryFirstIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateIdentityCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.QueryFirst<IdentityCompleteTable>(new QueryField("Id", -1)));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestSqlServerConnectionQueryFirstAsyncViaPrimaryKey()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(10).Last();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QueryFirstAsync<IdentityCompleteTable>(table.Id).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestSqlServerConnectionQueryFirstAsyncViaExpression()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(10).Last();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QueryFirstAsync<IdentityCompleteTable>(e => e.Id == table.Id).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestSqlServerConnectionQueryFirstAsyncViaDynamic()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(10).Last();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QueryFirstAsync<IdentityCompleteTable>(new { table.Id }).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestSqlServerConnectionQueryFirstAsyncViaQueryField()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(10).Last();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QueryFirstAsync<IdentityCompleteTable>(new QueryField("Id", table.Id)).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestSqlServerConnectionQueryFirstAsyncViaQueryFields()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(10).Last();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QueryFirstAsync<IdentityCompleteTable>(queryFields).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestSqlServerConnectionQueryFirstAsyncViaQueryGroup()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(10).Last();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QueryFirstAsync<IdentityCompleteTable>(new QueryGroup(queryFields)).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestSqlServerConnectionQueryFirstAsyncWithOrderBy()
        {
            // Setup
            var tables = Database.CreateIdentityCompleteTables(10).ToList();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var ascending = await connection.QueryFirstAsync<IdentityCompleteTable>((object)null, orderBy: new[] { new OrderField("Id", Order.Ascending) }).ConfigureAwait(false);
                var descending = await connection.QueryFirstAsync<IdentityCompleteTable>((object)null, orderBy: new[] { new OrderField("Id", Order.Descending) }).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.First().Id, ascending.Id);
                Helper.AssertPropertiesEquality(tables.First(), ascending);
                Assert.AreEqual(tables.Last().Id, descending.Id);
                Helper.AssertPropertiesEquality(tables.Last(), descending);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqlServerConnectionQueryFirstAsyncIfNoRowsFound()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.QueryFirstAsync<IdentityCompleteTable>((object)null).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqlServerConnectionQueryFirstAsyncIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateIdentityCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.QueryFirstAsync<IdentityCompleteTable>(new QueryField("Id", -1)).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestSqlServerConnectionQueryFirstViaTableNameViaPrimaryKey()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(10).Last();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QueryFirst(ClassMappedNameCache.Get<IdentityCompleteTable>(), table.Id);

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(table.Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(table, kvp);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionQueryFirstViaTableNameViaDynamic()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(10).Last();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QueryFirst(ClassMappedNameCache.Get<IdentityCompleteTable>(), new { table.Id });

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(table.Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(table, kvp);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionQueryFirstViaTableNameViaQueryField()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(10).Last();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QueryFirst(ClassMappedNameCache.Get<IdentityCompleteTable>(), new QueryField("Id", table.Id));

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(table.Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(table, kvp);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionQueryFirstViaTableNameViaQueryFields()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(10).Last();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QueryFirst(ClassMappedNameCache.Get<IdentityCompleteTable>(), queryFields);

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(table.Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(table, kvp);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionQueryFirstViaTableNameViaQueryGroup()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(10).Last();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QueryFirst(ClassMappedNameCache.Get<IdentityCompleteTable>(), new QueryGroup(queryFields));

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(table.Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(table, kvp);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionQueryFirstViaTableNameWithOrderBy()
        {
            // Setup
            var tables = Database.CreateIdentityCompleteTables(10).ToList();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var ascending = connection.QueryFirst(ClassMappedNameCache.Get<IdentityCompleteTable>(), (object)null, orderBy: new[] { new OrderField("Id", Order.Ascending) });
                var descending = connection.QueryFirst(ClassMappedNameCache.Get<IdentityCompleteTable>(), (object)null, orderBy: new[] { new OrderField("Id", Order.Descending) });

                // Assert
                var ascendingKvp = (IDictionary<string, object>)ascending;
                var descendingKvp = (IDictionary<string, object>)descending;
                Assert.AreEqual(tables.First().Id, System.Convert.ToInt32(ascendingKvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(tables.First(), ascendingKvp);
                Assert.AreEqual(tables.Last().Id, System.Convert.ToInt32(descendingKvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(tables.Last(), descendingKvp);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqlServerConnectionQueryFirstViaTableNameIfNoRowsFound()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.QueryFirst(ClassMappedNameCache.Get<IdentityCompleteTable>(), (object)null));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqlServerConnectionQueryFirstViaTableNameIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateIdentityCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.QueryFirst(ClassMappedNameCache.Get<IdentityCompleteTable>(), new QueryField("Id", -1)));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestSqlServerConnectionQueryFirstAsyncViaTableNameViaPrimaryKey()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(10).Last();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QueryFirstAsync(ClassMappedNameCache.Get<IdentityCompleteTable>(), table.Id).ConfigureAwait(false);

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(table.Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(table, kvp);
            }
        }

        [TestMethod]
        public async Task TestSqlServerConnectionQueryFirstAsyncViaTableNameViaDynamic()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(10).Last();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QueryFirstAsync(ClassMappedNameCache.Get<IdentityCompleteTable>(), new { table.Id }).ConfigureAwait(false);

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(table.Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(table, kvp);
            }
        }

        [TestMethod]
        public async Task TestSqlServerConnectionQueryFirstAsyncViaTableNameViaQueryField()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(10).Last();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QueryFirstAsync(ClassMappedNameCache.Get<IdentityCompleteTable>(), new QueryField("Id", table.Id)).ConfigureAwait(false);

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(table.Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(table, kvp);
            }
        }

        [TestMethod]
        public async Task TestSqlServerConnectionQueryFirstAsyncViaTableNameViaQueryFields()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(10).Last();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QueryFirstAsync(ClassMappedNameCache.Get<IdentityCompleteTable>(), queryFields).ConfigureAwait(false);

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(table.Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(table, kvp);
            }
        }

        [TestMethod]
        public async Task TestSqlServerConnectionQueryFirstAsyncViaTableNameViaQueryGroup()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(10).Last();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QueryFirstAsync(ClassMappedNameCache.Get<IdentityCompleteTable>(), new QueryGroup(queryFields)).ConfigureAwait(false);

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(table.Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(table, kvp);
            }
        }

        [TestMethod]
        public async Task TestSqlServerConnectionQueryFirstAsyncViaTableNameWithOrderBy()
        {
            // Setup
            var tables = Database.CreateIdentityCompleteTables(10).ToList();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var ascending = await connection.QueryFirstAsync(ClassMappedNameCache.Get<IdentityCompleteTable>(), (object)null, orderBy: new[] { new OrderField("Id", Order.Ascending) }).ConfigureAwait(false);
                var descending = await connection.QueryFirstAsync(ClassMappedNameCache.Get<IdentityCompleteTable>(), (object)null, orderBy: new[] { new OrderField("Id", Order.Descending) }).ConfigureAwait(false);

                // Assert
                var ascendingKvp = (IDictionary<string, object>)ascending;
                var descendingKvp = (IDictionary<string, object>)descending;
                Assert.AreEqual(tables.First().Id, System.Convert.ToInt32(ascendingKvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(tables.First(), ascendingKvp);
                Assert.AreEqual(tables.Last().Id, System.Convert.ToInt32(descendingKvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(tables.Last(), descendingKvp);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqlServerConnectionQueryFirstAsyncViaTableNameIfNoRowsFound()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.QueryFirstAsync(ClassMappedNameCache.Get<IdentityCompleteTable>(), (object)null).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqlServerConnectionQueryFirstAsyncViaTableNameIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateIdentityCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.QueryFirstAsync(ClassMappedNameCache.Get<IdentityCompleteTable>(), new QueryField("Id", -1)).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        #endregion

        #endregion
    }
}
