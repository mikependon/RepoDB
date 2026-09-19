#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using FirebirdSql.Data.FirebirdClient;
using RepoDb.Enumerations;
using RepoDb.Exceptions;
using RepoDb.Firebird.IntegrationTests.Models;
using RepoDb.Firebird.IntegrationTests.Setup;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.Firebird.IntegrationTests.Operations
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
        public void TestFirebirdConnectionQueryFirstViaPrimaryKey()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            var result = connection.QueryFirst<CompleteTable>(table.Id);

            // Assert
            Assert.AreEqual(table.Id, result.Id);
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public void TestFirebirdConnectionQueryFirstViaExpression()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            var result = connection.QueryFirst<CompleteTable>(e => e.Id == table.Id);

            // Assert
            Assert.AreEqual(table.Id, result.Id);
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public void TestFirebirdConnectionQueryFirstViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            var result = connection.QueryFirst<CompleteTable>(new { table.Id });

            // Assert
            Assert.AreEqual(table.Id, result.Id);
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public void TestFirebirdConnectionQueryFirstViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            var result = connection.QueryFirst<CompleteTable>(new QueryField("Id", table.Id));

            // Assert
            Assert.AreEqual(table.Id, result.Id);
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public void TestFirebirdConnectionQueryFirstViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            var result = connection.QueryFirst<CompleteTable>(queryFields);

            // Assert
            Assert.AreEqual(table.Id, result.Id);
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public void TestFirebirdConnectionQueryFirstViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            var result = connection.QueryFirst<CompleteTable>(new QueryGroup(queryFields));

            // Assert
            Assert.AreEqual(table.Id, result.Id);
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public void TestFirebirdConnectionQueryFirstWithOrderBy()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            var ascending = connection.QueryFirst<CompleteTable>((object)null, orderBy: new[] { new OrderField("Id", Order.Ascending) });
            var descending = connection.QueryFirst<CompleteTable>((object)null, orderBy: new[] { new OrderField("Id", Order.Descending) });

            // Assert
            Assert.AreEqual(tables.First().Id, ascending.Id);
            Helper.AssertPropertiesEquality(tables.First(), ascending);
            Assert.AreEqual(tables.Last().Id, descending.Id);
            Helper.AssertPropertiesEquality(tables.Last(), descending);
        }

        [TestMethod]
        public void ThrowExceptionOnTestFirebirdConnectionQueryFirstIfNoRowsFound()
        {
            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            Assert.Throws<EmptyException>(() => connection.QueryFirst<CompleteTable>((object)null));
        }

        [TestMethod]
        public void ThrowExceptionOnTestFirebirdConnectionQueryFirstIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            Assert.Throws<EmptyException>(() => connection.QueryFirst<CompleteTable>(new QueryField("Id", -1)));
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestFirebirdConnectionQueryFirstAsyncViaPrimaryKey()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            var result = await connection.QueryFirstAsync<CompleteTable>(table.Id).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(table.Id, result.Id);
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public async Task TestFirebirdConnectionQueryFirstAsyncViaExpression()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            var result = await connection.QueryFirstAsync<CompleteTable>(e => e.Id == table.Id).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(table.Id, result.Id);
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public async Task TestFirebirdConnectionQueryFirstAsyncViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            var result = await connection.QueryFirstAsync<CompleteTable>(new { table.Id }).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(table.Id, result.Id);
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public async Task TestFirebirdConnectionQueryFirstAsyncViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            var result = await connection.QueryFirstAsync<CompleteTable>(new QueryField("Id", table.Id)).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(table.Id, result.Id);
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public async Task TestFirebirdConnectionQueryFirstAsyncViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            var result = await connection.QueryFirstAsync<CompleteTable>(queryFields).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(table.Id, result.Id);
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public async Task TestFirebirdConnectionQueryFirstAsyncViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            var result = await connection.QueryFirstAsync<CompleteTable>(new QueryGroup(queryFields)).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(table.Id, result.Id);
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public async Task TestFirebirdConnectionQueryFirstAsyncWithOrderBy()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            var ascending = await connection.QueryFirstAsync<CompleteTable>((object)null, orderBy: new[] { new OrderField("Id", Order.Ascending) }).ConfigureAwait(false);
            var descending = await connection.QueryFirstAsync<CompleteTable>((object)null, orderBy: new[] { new OrderField("Id", Order.Descending) }).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(tables.First().Id, ascending.Id);
            Helper.AssertPropertiesEquality(tables.First(), ascending);
            Assert.AreEqual(tables.Last().Id, descending.Id);
            Helper.AssertPropertiesEquality(tables.Last(), descending);
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestFirebirdConnectionQueryFirstAsyncIfNoRowsFound()
        {
            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            await Assert.ThrowsAsync<EmptyException>(async () => await connection.QueryFirstAsync<CompleteTable>((object)null).ConfigureAwait(false)).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestFirebirdConnectionQueryFirstAsyncIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            await Assert.ThrowsAsync<EmptyException>(async () => await connection.QueryFirstAsync<CompleteTable>(new QueryField("Id", -1)).ConfigureAwait(false)).ConfigureAwait(false);
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestFirebirdConnectionQueryFirstViaTableNameViaPrimaryKey()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            var result = connection.QueryFirst(ClassMappedNameCache.Get<CompleteTable>(), table.Id);

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(table.Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(table, kvp);
        }

        [TestMethod]
        public void TestFirebirdConnectionQueryFirstViaTableNameViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            var result = connection.QueryFirst(ClassMappedNameCache.Get<CompleteTable>(), new { table.Id });

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(table.Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(table, kvp);
        }

        [TestMethod]
        public void TestFirebirdConnectionQueryFirstViaTableNameViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            var result = connection.QueryFirst(ClassMappedNameCache.Get<CompleteTable>(), new QueryField("Id", table.Id));

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(table.Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(table, kvp);
        }

        [TestMethod]
        public void TestFirebirdConnectionQueryFirstViaTableNameViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            var result = connection.QueryFirst(ClassMappedNameCache.Get<CompleteTable>(), queryFields);

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(table.Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(table, kvp);
        }

        [TestMethod]
        public void TestFirebirdConnectionQueryFirstViaTableNameViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            var result = connection.QueryFirst(ClassMappedNameCache.Get<CompleteTable>(), new QueryGroup(queryFields));

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(table.Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(table, kvp);
        }

        [TestMethod]
        public void TestFirebirdConnectionQueryFirstViaTableNameWithOrderBy()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            var ascending = connection.QueryFirst(ClassMappedNameCache.Get<CompleteTable>(), (object)null, orderBy: new[] { new OrderField("Id", Order.Ascending) });
            var descending = connection.QueryFirst(ClassMappedNameCache.Get<CompleteTable>(), (object)null, orderBy: new[] { new OrderField("Id", Order.Descending) });

            // Assert
            var ascendingKvp = (IDictionary<string, object>)ascending;
            var descendingKvp = (IDictionary<string, object>)descending;
            Assert.AreEqual(tables.First().Id, System.Convert.ToInt64(ascendingKvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(tables.First(), ascendingKvp);
            Assert.AreEqual(tables.Last().Id, System.Convert.ToInt64(descendingKvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(tables.Last(), descendingKvp);
        }

        [TestMethod]
        public void ThrowExceptionOnTestFirebirdConnectionQueryFirstViaTableNameIfNoRowsFound()
        {
            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            Assert.Throws<EmptyException>(() => connection.QueryFirst(ClassMappedNameCache.Get<CompleteTable>(), (object)null));
        }

        [TestMethod]
        public void ThrowExceptionOnTestFirebirdConnectionQueryFirstViaTableNameIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            Assert.Throws<EmptyException>(() => connection.QueryFirst(ClassMappedNameCache.Get<CompleteTable>(), new QueryField("Id", -1)));
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestFirebirdConnectionQueryFirstAsyncViaTableNameViaPrimaryKey()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            var result = await connection.QueryFirstAsync(ClassMappedNameCache.Get<CompleteTable>(), table.Id).ConfigureAwait(false);

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(table.Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(table, kvp);
        }

        [TestMethod]
        public async Task TestFirebirdConnectionQueryFirstAsyncViaTableNameViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            var result = await connection.QueryFirstAsync(ClassMappedNameCache.Get<CompleteTable>(), new { table.Id }).ConfigureAwait(false);

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(table.Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(table, kvp);
        }

        [TestMethod]
        public async Task TestFirebirdConnectionQueryFirstAsyncViaTableNameViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            var result = await connection.QueryFirstAsync(ClassMappedNameCache.Get<CompleteTable>(), new QueryField("Id", table.Id)).ConfigureAwait(false);

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(table.Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(table, kvp);
        }

        [TestMethod]
        public async Task TestFirebirdConnectionQueryFirstAsyncViaTableNameViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            var result = await connection.QueryFirstAsync(ClassMappedNameCache.Get<CompleteTable>(), queryFields).ConfigureAwait(false);

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(table.Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(table, kvp);
        }

        [TestMethod]
        public async Task TestFirebirdConnectionQueryFirstAsyncViaTableNameViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            var result = await connection.QueryFirstAsync(ClassMappedNameCache.Get<CompleteTable>(), new QueryGroup(queryFields)).ConfigureAwait(false);

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(table.Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(table, kvp);
        }

        [TestMethod]
        public async Task TestFirebirdConnectionQueryFirstAsyncViaTableNameWithOrderBy()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            var ascending = await connection.QueryFirstAsync(ClassMappedNameCache.Get<CompleteTable>(), (object)null, orderBy: new[] { new OrderField("Id", Order.Ascending) }).ConfigureAwait(false);
            var descending = await connection.QueryFirstAsync(ClassMappedNameCache.Get<CompleteTable>(), (object)null, orderBy: new[] { new OrderField("Id", Order.Descending) }).ConfigureAwait(false);

            // Assert
            var ascendingKvp = (IDictionary<string, object>)ascending;
            var descendingKvp = (IDictionary<string, object>)descending;
            Assert.AreEqual(tables.First().Id, System.Convert.ToInt64(ascendingKvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(tables.First(), ascendingKvp);
            Assert.AreEqual(tables.Last().Id, System.Convert.ToInt64(descendingKvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(tables.Last(), descendingKvp);
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestFirebirdConnectionQueryFirstAsyncViaTableNameIfNoRowsFound()
        {
            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            await Assert.ThrowsAsync<EmptyException>(async () => await connection.QueryFirstAsync(ClassMappedNameCache.Get<CompleteTable>(), (object)null).ConfigureAwait(false)).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestFirebirdConnectionQueryFirstAsyncViaTableNameIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new FbConnection(Database.ConnectionString);

            // Act
            await Assert.ThrowsAsync<EmptyException>(async () => await connection.QueryFirstAsync(ClassMappedNameCache.Get<CompleteTable>(), new QueryField("Id", -1)).ConfigureAwait(false)).ConfigureAwait(false);
        }

        #endregion

        #endregion
    }
}
