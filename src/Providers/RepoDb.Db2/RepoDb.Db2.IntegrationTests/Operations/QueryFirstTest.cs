#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using IBM.Data.Db2;
using RepoDb.Enumerations;
using RepoDb.Exceptions;
using RepoDb.Db2.IntegrationTests.Models;
using RepoDb.Db2.IntegrationTests.Setup;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.Db2.IntegrationTests.Operations
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
        public void TestDb2ConnectionQueryFirstViaPrimaryKey()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = connection.QueryFirst<CompleteTable>(table.Id);

            // Assert
            Assert.AreEqual(table.Id, result.Id);
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public void TestDb2ConnectionQueryFirstViaExpression()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = connection.QueryFirst<CompleteTable>(e => e.Id == table.Id);

            // Assert
            Assert.AreEqual(table.Id, result.Id);
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public void TestDb2ConnectionQueryFirstViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = connection.QueryFirst<CompleteTable>(new { table.Id });

            // Assert
            Assert.AreEqual(table.Id, result.Id);
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public void TestDb2ConnectionQueryFirstViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = connection.QueryFirst<CompleteTable>(new QueryField("Id", table.Id));

            // Assert
            Assert.AreEqual(table.Id, result.Id);
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public void TestDb2ConnectionQueryFirstViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = connection.QueryFirst<CompleteTable>(queryFields);

            // Assert
            Assert.AreEqual(table.Id, result.Id);
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public void TestDb2ConnectionQueryFirstViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = connection.QueryFirst<CompleteTable>(new QueryGroup(queryFields));

            // Assert
            Assert.AreEqual(table.Id, result.Id);
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public void TestDb2ConnectionQueryFirstWithOrderBy()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new DB2Connection(Database.ConnectionString);

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
        public void ThrowExceptionOnTestDb2ConnectionQueryFirstIfNoRowsFound()
        {
            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            Assert.Throws<EmptyException>(() => connection.QueryFirst<CompleteTable>((object)null));
        }

        [TestMethod]
        public void ThrowExceptionOnTestDb2ConnectionQueryFirstIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            Assert.Throws<EmptyException>(() => connection.QueryFirst<CompleteTable>(new QueryField("Id", -1)));
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestDb2ConnectionQueryFirstAsyncViaPrimaryKey()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = await connection.QueryFirstAsync<CompleteTable>(table.Id).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(table.Id, result.Id);
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public async Task TestDb2ConnectionQueryFirstAsyncViaExpression()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = await connection.QueryFirstAsync<CompleteTable>(e => e.Id == table.Id).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(table.Id, result.Id);
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public async Task TestDb2ConnectionQueryFirstAsyncViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = await connection.QueryFirstAsync<CompleteTable>(new { table.Id }).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(table.Id, result.Id);
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public async Task TestDb2ConnectionQueryFirstAsyncViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = await connection.QueryFirstAsync<CompleteTable>(new QueryField("Id", table.Id)).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(table.Id, result.Id);
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public async Task TestDb2ConnectionQueryFirstAsyncViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = await connection.QueryFirstAsync<CompleteTable>(queryFields).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(table.Id, result.Id);
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public async Task TestDb2ConnectionQueryFirstAsyncViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = await connection.QueryFirstAsync<CompleteTable>(new QueryGroup(queryFields)).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(table.Id, result.Id);
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public async Task TestDb2ConnectionQueryFirstAsyncWithOrderBy()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new DB2Connection(Database.ConnectionString);

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
        public async Task ThrowExceptionOnTestDb2ConnectionQueryFirstAsyncIfNoRowsFound()
        {
            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            await Assert.ThrowsAsync<EmptyException>(async () => await connection.QueryFirstAsync<CompleteTable>((object)null).ConfigureAwait(false)).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestDb2ConnectionQueryFirstAsyncIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            await Assert.ThrowsAsync<EmptyException>(async () => await connection.QueryFirstAsync<CompleteTable>(new QueryField("Id", -1)).ConfigureAwait(false)).ConfigureAwait(false);
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestDb2ConnectionQueryFirstViaTableNameViaPrimaryKey()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = connection.QueryFirst(ClassMappedNameCache.Get<CompleteTable>(), table.Id);

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(table.Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(table, kvp);
        }

        [TestMethod]
        public void TestDb2ConnectionQueryFirstViaTableNameViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = connection.QueryFirst(ClassMappedNameCache.Get<CompleteTable>(), new { table.Id });

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(table.Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(table, kvp);
        }

        [TestMethod]
        public void TestDb2ConnectionQueryFirstViaTableNameViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = connection.QueryFirst(ClassMappedNameCache.Get<CompleteTable>(), new QueryField("Id", table.Id));

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(table.Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(table, kvp);
        }

        [TestMethod]
        public void TestDb2ConnectionQueryFirstViaTableNameViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = connection.QueryFirst(ClassMappedNameCache.Get<CompleteTable>(), queryFields);

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(table.Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(table, kvp);
        }

        [TestMethod]
        public void TestDb2ConnectionQueryFirstViaTableNameViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = connection.QueryFirst(ClassMappedNameCache.Get<CompleteTable>(), new QueryGroup(queryFields));

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(table.Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(table, kvp);
        }

        [TestMethod]
        public void TestDb2ConnectionQueryFirstViaTableNameWithOrderBy()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var ascending = connection.QueryFirst(ClassMappedNameCache.Get<CompleteTable>(), (object)null, orderBy: new[] { new OrderField("Id", Order.Ascending) });
            var descending = connection.QueryFirst(ClassMappedNameCache.Get<CompleteTable>(), (object)null, orderBy: new[] { new OrderField("Id", Order.Descending) });

            // Assert
            var ascendingKvp = (IDictionary<string, object>)ascending;
            var descendingKvp = (IDictionary<string, object>)descending;
            Assert.AreEqual(tables.First().Id, System.Convert.ToInt32(ascendingKvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(tables.First(), ascendingKvp);
            Assert.AreEqual(tables.Last().Id, System.Convert.ToInt32(descendingKvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(tables.Last(), descendingKvp);
        }

        [TestMethod]
        public void ThrowExceptionOnTestDb2ConnectionQueryFirstViaTableNameIfNoRowsFound()
        {
            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            Assert.Throws<EmptyException>(() => connection.QueryFirst(ClassMappedNameCache.Get<CompleteTable>(), (object)null));
        }

        [TestMethod]
        public void ThrowExceptionOnTestDb2ConnectionQueryFirstViaTableNameIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            Assert.Throws<EmptyException>(() => connection.QueryFirst(ClassMappedNameCache.Get<CompleteTable>(), new QueryField("Id", -1)));
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestDb2ConnectionQueryFirstAsyncViaTableNameViaPrimaryKey()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = await connection.QueryFirstAsync(ClassMappedNameCache.Get<CompleteTable>(), table.Id).ConfigureAwait(false);

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(table.Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(table, kvp);
        }

        [TestMethod]
        public async Task TestDb2ConnectionQueryFirstAsyncViaTableNameViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = await connection.QueryFirstAsync(ClassMappedNameCache.Get<CompleteTable>(), new { table.Id }).ConfigureAwait(false);

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(table.Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(table, kvp);
        }

        [TestMethod]
        public async Task TestDb2ConnectionQueryFirstAsyncViaTableNameViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = await connection.QueryFirstAsync(ClassMappedNameCache.Get<CompleteTable>(), new QueryField("Id", table.Id)).ConfigureAwait(false);

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(table.Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(table, kvp);
        }

        [TestMethod]
        public async Task TestDb2ConnectionQueryFirstAsyncViaTableNameViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = await connection.QueryFirstAsync(ClassMappedNameCache.Get<CompleteTable>(), queryFields).ConfigureAwait(false);

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(table.Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(table, kvp);
        }

        [TestMethod]
        public async Task TestDb2ConnectionQueryFirstAsyncViaTableNameViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = await connection.QueryFirstAsync(ClassMappedNameCache.Get<CompleteTable>(), new QueryGroup(queryFields)).ConfigureAwait(false);

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(table.Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(table, kvp);
        }

        [TestMethod]
        public async Task TestDb2ConnectionQueryFirstAsyncViaTableNameWithOrderBy()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var ascending = await connection.QueryFirstAsync(ClassMappedNameCache.Get<CompleteTable>(), (object)null, orderBy: new[] { new OrderField("Id", Order.Ascending) }).ConfigureAwait(false);
            var descending = await connection.QueryFirstAsync(ClassMappedNameCache.Get<CompleteTable>(), (object)null, orderBy: new[] { new OrderField("Id", Order.Descending) }).ConfigureAwait(false);

            // Assert
            var ascendingKvp = (IDictionary<string, object>)ascending;
            var descendingKvp = (IDictionary<string, object>)descending;
            Assert.AreEqual(tables.First().Id, System.Convert.ToInt32(ascendingKvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(tables.First(), ascendingKvp);
            Assert.AreEqual(tables.Last().Id, System.Convert.ToInt32(descendingKvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(tables.Last(), descendingKvp);
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestDb2ConnectionQueryFirstAsyncViaTableNameIfNoRowsFound()
        {
            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            await Assert.ThrowsAsync<EmptyException>(async () => await connection.QueryFirstAsync(ClassMappedNameCache.Get<CompleteTable>(), (object)null).ConfigureAwait(false)).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestDb2ConnectionQueryFirstAsyncViaTableNameIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            await Assert.ThrowsAsync<EmptyException>(async () => await connection.QueryFirstAsync(ClassMappedNameCache.Get<CompleteTable>(), new QueryField("Id", -1)).ConfigureAwait(false)).ConfigureAwait(false);
        }

        #endregion

        #endregion
    }
}
