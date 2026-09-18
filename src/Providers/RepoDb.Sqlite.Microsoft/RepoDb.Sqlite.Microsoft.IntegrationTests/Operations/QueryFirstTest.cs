#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Data.Sqlite;
using RepoDb.Enumerations;
using RepoDb.Exceptions;
using RepoDb.Sqlite.Microsoft.IntegrationTests.Models;
using RepoDb.Sqlite.Microsoft.IntegrationTests.Setup;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.Sqlite.Microsoft.IntegrationTests.Operations.MDS
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
        public void TestSqLiteConnectionQueryFirstViaPrimaryKey()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateMdsCompleteTables(10, connection).Last();

                // Act
                var result = connection.QueryFirst<MdsCompleteTable>(table.Id);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionQueryFirstViaExpression()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateMdsCompleteTables(10, connection).Last();

                // Act
                var result = connection.QueryFirst<MdsCompleteTable>(e => e.Id == table.Id);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionQueryFirstViaDynamic()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateMdsCompleteTables(10, connection).Last();

                // Act
                var result = connection.QueryFirst<MdsCompleteTable>(new { table.Id });

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionQueryFirstViaQueryField()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateMdsCompleteTables(10, connection).Last();

                // Act
                var result = connection.QueryFirst<MdsCompleteTable>(new QueryField("Id", table.Id));

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionQueryFirstViaQueryFields()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateMdsCompleteTables(10, connection).Last();
                var queryFields = new[]
                {
                    new QueryField("Id", table.Id),
                    new QueryField("ColumnInt", table.ColumnInt)
                };

                // Act
                var result = connection.QueryFirst<MdsCompleteTable>(queryFields);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionQueryFirstViaQueryGroup()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateMdsCompleteTables(10, connection).Last();
                var queryFields = new[]
                {
                    new QueryField("Id", table.Id),
                    new QueryField("ColumnInt", table.ColumnInt)
                };

                // Act
                var result = connection.QueryFirst<MdsCompleteTable>(new QueryGroup(queryFields));

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionQueryFirstWithOrderBy()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection).ToList();

                // Act
                var ascending = connection.QueryFirst<MdsCompleteTable>((object)null, orderBy: new[] { new OrderField("Id", Order.Ascending) });
                var descending = connection.QueryFirst<MdsCompleteTable>((object)null, orderBy: new[] { new OrderField("Id", Order.Descending) });

                // Assert
                Assert.AreEqual(tables.First().Id, ascending.Id);
                Helper.AssertPropertiesEquality(tables.First(), ascending);
                Assert.AreEqual(tables.Last().Id, descending.Id);
                Helper.AssertPropertiesEquality(tables.Last(), descending);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqLiteConnectionQueryFirstIfNoRowsFound()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                Database.CreateMdsCompleteTable(connection);

                // Act
                Assert.Throws<EmptyException>(() => connection.QueryFirst<MdsCompleteTable>((object)null));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqLiteConnectionQueryFirstIfNoRowsMatchTheFilter()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                Database.CreateMdsCompleteTables(10, connection);

                // Act
                Assert.Throws<EmptyException>(() => connection.QueryFirst<MdsCompleteTable>(new QueryField("Id", -1L)));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestSqLiteConnectionQueryFirstAsyncViaPrimaryKey()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateMdsCompleteTables(10, connection).Last();

                // Act
                var result = await connection.QueryFirstAsync<MdsCompleteTable>(table.Id).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestSqLiteConnectionQueryFirstAsyncViaExpression()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateMdsCompleteTables(10, connection).Last();

                // Act
                var result = await connection.QueryFirstAsync<MdsCompleteTable>(e => e.Id == table.Id).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestSqLiteConnectionQueryFirstAsyncViaDynamic()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateMdsCompleteTables(10, connection).Last();

                // Act
                var result = await connection.QueryFirstAsync<MdsCompleteTable>(new { table.Id }).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestSqLiteConnectionQueryFirstAsyncViaQueryField()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateMdsCompleteTables(10, connection).Last();

                // Act
                var result = await connection.QueryFirstAsync<MdsCompleteTable>(new QueryField("Id", table.Id)).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestSqLiteConnectionQueryFirstAsyncViaQueryFields()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateMdsCompleteTables(10, connection).Last();
                var queryFields = new[]
                {
                    new QueryField("Id", table.Id),
                    new QueryField("ColumnInt", table.ColumnInt)
                };

                // Act
                var result = await connection.QueryFirstAsync<MdsCompleteTable>(queryFields).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestSqLiteConnectionQueryFirstAsyncViaQueryGroup()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateMdsCompleteTables(10, connection).Last();
                var queryFields = new[]
                {
                    new QueryField("Id", table.Id),
                    new QueryField("ColumnInt", table.ColumnInt)
                };

                // Act
                var result = await connection.QueryFirstAsync<MdsCompleteTable>(new QueryGroup(queryFields)).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestSqLiteConnectionQueryFirstAsyncWithOrderBy()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection).ToList();

                // Act
                var ascending = await connection.QueryFirstAsync<MdsCompleteTable>((object)null, orderBy: new[] { new OrderField("Id", Order.Ascending) }).ConfigureAwait(false);
                var descending = await connection.QueryFirstAsync<MdsCompleteTable>((object)null, orderBy: new[] { new OrderField("Id", Order.Descending) }).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.First().Id, ascending.Id);
                Helper.AssertPropertiesEquality(tables.First(), ascending);
                Assert.AreEqual(tables.Last().Id, descending.Id);
                Helper.AssertPropertiesEquality(tables.Last(), descending);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqLiteConnectionQueryFirstAsyncIfNoRowsFound()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                Database.CreateMdsCompleteTable(connection);

                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.QueryFirstAsync<MdsCompleteTable>((object)null).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqLiteConnectionQueryFirstAsyncIfNoRowsMatchTheFilter()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                Database.CreateMdsCompleteTables(10, connection);

                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.QueryFirstAsync<MdsCompleteTable>(new QueryField("Id", -1L)).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestSqLiteConnectionQueryFirstViaTableNameViaPrimaryKey()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateMdsCompleteTables(10, connection).Last();

                // Act
                var result = connection.QueryFirst(ClassMappedNameCache.Get<MdsCompleteTable>(), table.Id);

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(table.Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(table, kvp);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionQueryFirstViaTableNameViaDynamic()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateMdsCompleteTables(10, connection).Last();

                // Act
                var result = connection.QueryFirst(ClassMappedNameCache.Get<MdsCompleteTable>(), new { table.Id });

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(table.Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(table, kvp);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionQueryFirstViaTableNameViaQueryField()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateMdsCompleteTables(10, connection).Last();

                // Act
                var result = connection.QueryFirst(ClassMappedNameCache.Get<MdsCompleteTable>(), new QueryField("Id", table.Id));

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(table.Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(table, kvp);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionQueryFirstViaTableNameViaQueryFields()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateMdsCompleteTables(10, connection).Last();
                var queryFields = new[]
                {
                    new QueryField("Id", table.Id),
                    new QueryField("ColumnInt", table.ColumnInt)
                };

                // Act
                var result = connection.QueryFirst(ClassMappedNameCache.Get<MdsCompleteTable>(), queryFields);

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(table.Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(table, kvp);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionQueryFirstViaTableNameViaQueryGroup()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateMdsCompleteTables(10, connection).Last();
                var queryFields = new[]
                {
                    new QueryField("Id", table.Id),
                    new QueryField("ColumnInt", table.ColumnInt)
                };

                // Act
                var result = connection.QueryFirst(ClassMappedNameCache.Get<MdsCompleteTable>(), new QueryGroup(queryFields));

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(table.Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(table, kvp);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionQueryFirstViaTableNameWithOrderBy()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection).ToList();

                // Act
                var ascending = connection.QueryFirst(ClassMappedNameCache.Get<MdsCompleteTable>(), (object)null, orderBy: new[] { new OrderField("Id", Order.Ascending) });
                var descending = connection.QueryFirst(ClassMappedNameCache.Get<MdsCompleteTable>(), (object)null, orderBy: new[] { new OrderField("Id", Order.Descending) });

                // Assert
                var ascendingKvp = (IDictionary<string, object>)ascending;
                var descendingKvp = (IDictionary<string, object>)descending;
                Assert.AreEqual(tables.First().Id, System.Convert.ToInt64(ascendingKvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(tables.First(), ascendingKvp);
                Assert.AreEqual(tables.Last().Id, System.Convert.ToInt64(descendingKvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(tables.Last(), descendingKvp);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqLiteConnectionQueryFirstViaTableNameIfNoRowsFound()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                Database.CreateMdsCompleteTable(connection);

                // Act
                Assert.Throws<EmptyException>(() => connection.QueryFirst(ClassMappedNameCache.Get<MdsCompleteTable>(), (object)null));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqLiteConnectionQueryFirstViaTableNameIfNoRowsMatchTheFilter()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                Database.CreateMdsCompleteTables(10, connection);

                // Act
                Assert.Throws<EmptyException>(() => connection.QueryFirst(ClassMappedNameCache.Get<MdsCompleteTable>(), new QueryField("Id", -1L)));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestSqLiteConnectionQueryFirstAsyncViaTableNameViaPrimaryKey()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateMdsCompleteTables(10, connection).Last();

                // Act
                var result = await connection.QueryFirstAsync(ClassMappedNameCache.Get<MdsCompleteTable>(), table.Id).ConfigureAwait(false);

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(table.Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(table, kvp);
            }
        }

        [TestMethod]
        public async Task TestSqLiteConnectionQueryFirstAsyncViaTableNameViaDynamic()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateMdsCompleteTables(10, connection).Last();

                // Act
                var result = await connection.QueryFirstAsync(ClassMappedNameCache.Get<MdsCompleteTable>(), new { table.Id }).ConfigureAwait(false);

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(table.Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(table, kvp);
            }
        }

        [TestMethod]
        public async Task TestSqLiteConnectionQueryFirstAsyncViaTableNameViaQueryField()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateMdsCompleteTables(10, connection).Last();

                // Act
                var result = await connection.QueryFirstAsync(ClassMappedNameCache.Get<MdsCompleteTable>(), new QueryField("Id", table.Id)).ConfigureAwait(false);

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(table.Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(table, kvp);
            }
        }

        [TestMethod]
        public async Task TestSqLiteConnectionQueryFirstAsyncViaTableNameViaQueryFields()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateMdsCompleteTables(10, connection).Last();
                var queryFields = new[]
                {
                    new QueryField("Id", table.Id),
                    new QueryField("ColumnInt", table.ColumnInt)
                };

                // Act
                var result = await connection.QueryFirstAsync(ClassMappedNameCache.Get<MdsCompleteTable>(), queryFields).ConfigureAwait(false);

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(table.Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(table, kvp);
            }
        }

        [TestMethod]
        public async Task TestSqLiteConnectionQueryFirstAsyncViaTableNameViaQueryGroup()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateMdsCompleteTables(10, connection).Last();
                var queryFields = new[]
                {
                    new QueryField("Id", table.Id),
                    new QueryField("ColumnInt", table.ColumnInt)
                };

                // Act
                var result = await connection.QueryFirstAsync(ClassMappedNameCache.Get<MdsCompleteTable>(), new QueryGroup(queryFields)).ConfigureAwait(false);

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(table.Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(table, kvp);
            }
        }

        [TestMethod]
        public async Task TestSqLiteConnectionQueryFirstAsyncViaTableNameWithOrderBy()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection).ToList();

                // Act
                var ascending = await connection.QueryFirstAsync(ClassMappedNameCache.Get<MdsCompleteTable>(), (object)null, orderBy: new[] { new OrderField("Id", Order.Ascending) }).ConfigureAwait(false);
                var descending = await connection.QueryFirstAsync(ClassMappedNameCache.Get<MdsCompleteTable>(), (object)null, orderBy: new[] { new OrderField("Id", Order.Descending) }).ConfigureAwait(false);

                // Assert
                var ascendingKvp = (IDictionary<string, object>)ascending;
                var descendingKvp = (IDictionary<string, object>)descending;
                Assert.AreEqual(tables.First().Id, System.Convert.ToInt64(ascendingKvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(tables.First(), ascendingKvp);
                Assert.AreEqual(tables.Last().Id, System.Convert.ToInt64(descendingKvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(tables.Last(), descendingKvp);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqLiteConnectionQueryFirstAsyncViaTableNameIfNoRowsFound()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                Database.CreateMdsCompleteTable(connection);

                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.QueryFirstAsync(ClassMappedNameCache.Get<MdsCompleteTable>(), (object)null).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqLiteConnectionQueryFirstAsyncViaTableNameIfNoRowsMatchTheFilter()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                Database.CreateMdsCompleteTables(10, connection);

                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.QueryFirstAsync(ClassMappedNameCache.Get<MdsCompleteTable>(), new QueryField("Id", -1L)).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        #endregion

        #endregion
    }
}
