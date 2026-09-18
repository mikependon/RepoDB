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

        #region DataEntity

        #region Sync

        [TestMethod]
        public void TestSqLiteConnectionQuerySingleViaPrimaryKey()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateMdsCompleteTables(10, connection).Last();

                // Act
                var result = connection.QuerySingle<MdsCompleteTable>(table.Id);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionQuerySingleViaExpression()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateMdsCompleteTables(10, connection).Last();

                // Act
                var result = connection.QuerySingle<MdsCompleteTable>(e => e.Id == table.Id);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionQuerySingleViaDynamic()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateMdsCompleteTables(10, connection).Last();

                // Act
                var result = connection.QuerySingle<MdsCompleteTable>(new { table.Id });

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionQuerySingleViaQueryField()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateMdsCompleteTables(10, connection).Last();

                // Act
                var result = connection.QuerySingle<MdsCompleteTable>(new QueryField("Id", table.Id));

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionQuerySingleViaQueryFields()
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
                var result = connection.QuerySingle<MdsCompleteTable>(queryFields);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionQuerySingleViaQueryGroup()
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
                var result = connection.QuerySingle<MdsCompleteTable>(new QueryGroup(queryFields));

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionQuerySingleWithTop()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateMdsCompleteTables(10, connection).First();

                // Act
                var result = connection.QuerySingle<MdsCompleteTable>((object)null, orderBy: new[] { new OrderField("Id", Order.Ascending) }, top: 1);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqLiteConnectionQuerySingleIfNoRowsFound()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                Database.CreateMdsCompleteTable(connection);

                // Act
                Assert.Throws<EmptyException>(() => connection.QuerySingle<MdsCompleteTable>((object)null));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqLiteConnectionQuerySingleIfNoRowsMatchTheFilter()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                Database.CreateMdsCompleteTables(10, connection);

                // Act
                Assert.Throws<EmptyException>(() => connection.QuerySingle<MdsCompleteTable>(new QueryField("Id", -1L)));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqLiteConnectionQuerySingleIfMultipleRowsFound()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                Database.CreateMdsCompleteTables(10, connection);

                // Act
                Assert.Throws<MultipleRowsFoundException>(() => connection.QuerySingle<MdsCompleteTable>((object)null));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestSqLiteConnectionQuerySingleAsyncViaPrimaryKey()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateMdsCompleteTables(10, connection).Last();

                // Act
                var result = await connection.QuerySingleAsync<MdsCompleteTable>(table.Id).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestSqLiteConnectionQuerySingleAsyncViaExpression()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateMdsCompleteTables(10, connection).Last();

                // Act
                var result = await connection.QuerySingleAsync<MdsCompleteTable>(e => e.Id == table.Id).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestSqLiteConnectionQuerySingleAsyncViaDynamic()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateMdsCompleteTables(10, connection).Last();

                // Act
                var result = await connection.QuerySingleAsync<MdsCompleteTable>(new { table.Id }).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestSqLiteConnectionQuerySingleAsyncViaQueryField()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateMdsCompleteTables(10, connection).Last();

                // Act
                var result = await connection.QuerySingleAsync<MdsCompleteTable>(new QueryField("Id", table.Id)).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestSqLiteConnectionQuerySingleAsyncViaQueryFields()
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
                var result = await connection.QuerySingleAsync<MdsCompleteTable>(queryFields).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestSqLiteConnectionQuerySingleAsyncViaQueryGroup()
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
                var result = await connection.QuerySingleAsync<MdsCompleteTable>(new QueryGroup(queryFields)).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestSqLiteConnectionQuerySingleAsyncWithTop()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateMdsCompleteTables(10, connection).First();

                // Act
                var result = await connection.QuerySingleAsync<MdsCompleteTable>((object)null, orderBy: new[] { new OrderField("Id", Order.Ascending) }, top: 1).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqLiteConnectionQuerySingleAsyncIfNoRowsFound()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                Database.CreateMdsCompleteTable(connection);

                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.QuerySingleAsync<MdsCompleteTable>((object)null).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqLiteConnectionQuerySingleAsyncIfNoRowsMatchTheFilter()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                Database.CreateMdsCompleteTables(10, connection);

                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.QuerySingleAsync<MdsCompleteTable>(new QueryField("Id", -1L)).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqLiteConnectionQuerySingleAsyncIfMultipleRowsFound()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                Database.CreateMdsCompleteTables(10, connection);

                // Act
                await Assert.ThrowsAsync<MultipleRowsFoundException>(async () => await connection.QuerySingleAsync<MdsCompleteTable>((object)null).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestSqLiteConnectionQuerySingleViaTableNameViaPrimaryKey()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateMdsCompleteTables(10, connection).Last();

                // Act
                var result = connection.QuerySingle(ClassMappedNameCache.Get<MdsCompleteTable>(), table.Id);

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(table.Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(table, kvp);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionQuerySingleViaTableNameViaDynamic()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateMdsCompleteTables(10, connection).Last();

                // Act
                var result = connection.QuerySingle(ClassMappedNameCache.Get<MdsCompleteTable>(), new { table.Id });

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(table.Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(table, kvp);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionQuerySingleViaTableNameViaQueryField()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateMdsCompleteTables(10, connection).Last();

                // Act
                var result = connection.QuerySingle(ClassMappedNameCache.Get<MdsCompleteTable>(), new QueryField("Id", table.Id));

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(table.Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(table, kvp);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionQuerySingleViaTableNameViaQueryFields()
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
                var result = connection.QuerySingle(ClassMappedNameCache.Get<MdsCompleteTable>(), queryFields);

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(table.Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(table, kvp);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionQuerySingleViaTableNameViaQueryGroup()
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
                var result = connection.QuerySingle(ClassMappedNameCache.Get<MdsCompleteTable>(), new QueryGroup(queryFields));

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(table.Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(table, kvp);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionQuerySingleViaTableNameWithTop()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateMdsCompleteTables(10, connection).First();

                // Act
                var result = connection.QuerySingle(ClassMappedNameCache.Get<MdsCompleteTable>(), (object)null, orderBy: new[] { new OrderField("Id", Order.Ascending) }, top: 1);

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(table.Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(table, kvp);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqLiteConnectionQuerySingleViaTableNameIfNoRowsFound()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                Database.CreateMdsCompleteTable(connection);

                // Act
                Assert.Throws<EmptyException>(() => connection.QuerySingle(ClassMappedNameCache.Get<MdsCompleteTable>(), (object)null));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqLiteConnectionQuerySingleViaTableNameIfNoRowsMatchTheFilter()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                Database.CreateMdsCompleteTables(10, connection);

                // Act
                Assert.Throws<EmptyException>(() => connection.QuerySingle(ClassMappedNameCache.Get<MdsCompleteTable>(), new QueryField("Id", -1L)));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestSqLiteConnectionQuerySingleViaTableNameIfMultipleRowsFound()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                Database.CreateMdsCompleteTables(10, connection);

                // Act
                Assert.Throws<MultipleRowsFoundException>(() => connection.QuerySingle(ClassMappedNameCache.Get<MdsCompleteTable>(), (object)null));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestSqLiteConnectionQuerySingleAsyncViaTableNameViaPrimaryKey()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateMdsCompleteTables(10, connection).Last();

                // Act
                var result = await connection.QuerySingleAsync(ClassMappedNameCache.Get<MdsCompleteTable>(), table.Id).ConfigureAwait(false);

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(table.Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(table, kvp);
            }
        }

        [TestMethod]
        public async Task TestSqLiteConnectionQuerySingleAsyncViaTableNameViaDynamic()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateMdsCompleteTables(10, connection).Last();

                // Act
                var result = await connection.QuerySingleAsync(ClassMappedNameCache.Get<MdsCompleteTable>(), new { table.Id }).ConfigureAwait(false);

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(table.Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(table, kvp);
            }
        }

        [TestMethod]
        public async Task TestSqLiteConnectionQuerySingleAsyncViaTableNameViaQueryField()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateMdsCompleteTables(10, connection).Last();

                // Act
                var result = await connection.QuerySingleAsync(ClassMappedNameCache.Get<MdsCompleteTable>(), new QueryField("Id", table.Id)).ConfigureAwait(false);

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(table.Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(table, kvp);
            }
        }

        [TestMethod]
        public async Task TestSqLiteConnectionQuerySingleAsyncViaTableNameViaQueryFields()
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
                var result = await connection.QuerySingleAsync(ClassMappedNameCache.Get<MdsCompleteTable>(), queryFields).ConfigureAwait(false);

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(table.Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(table, kvp);
            }
        }

        [TestMethod]
        public async Task TestSqLiteConnectionQuerySingleAsyncViaTableNameViaQueryGroup()
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
                var result = await connection.QuerySingleAsync(ClassMappedNameCache.Get<MdsCompleteTable>(), new QueryGroup(queryFields)).ConfigureAwait(false);

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(table.Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(table, kvp);
            }
        }

        [TestMethod]
        public async Task TestSqLiteConnectionQuerySingleAsyncViaTableNameWithTop()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateMdsCompleteTables(10, connection).First();

                // Act
                var result = await connection.QuerySingleAsync(ClassMappedNameCache.Get<MdsCompleteTable>(), (object)null, orderBy: new[] { new OrderField("Id", Order.Ascending) }, top: 1).ConfigureAwait(false);

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(table.Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(table, kvp);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqLiteConnectionQuerySingleAsyncViaTableNameIfNoRowsFound()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                Database.CreateMdsCompleteTable(connection);

                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.QuerySingleAsync(ClassMappedNameCache.Get<MdsCompleteTable>(), (object)null).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqLiteConnectionQuerySingleAsyncViaTableNameIfNoRowsMatchTheFilter()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                Database.CreateMdsCompleteTables(10, connection);

                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.QuerySingleAsync(ClassMappedNameCache.Get<MdsCompleteTable>(), new QueryField("Id", -1L)).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestSqLiteConnectionQuerySingleAsyncViaTableNameIfMultipleRowsFound()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                Database.CreateMdsCompleteTables(10, connection);

                // Act
                await Assert.ThrowsAsync<MultipleRowsFoundException>(async () => await connection.QuerySingleAsync(ClassMappedNameCache.Get<MdsCompleteTable>(), (object)null).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        #endregion

        #endregion
    }
}
