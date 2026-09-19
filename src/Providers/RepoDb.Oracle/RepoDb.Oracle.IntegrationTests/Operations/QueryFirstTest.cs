#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oracle.ManagedDataAccess.Client;
using RepoDb.Enumerations;
using RepoDb.Exceptions;
using RepoDb.Oracle.IntegrationTests.Models;
using RepoDb.Oracle.IntegrationTests.Setup;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.Oracle.IntegrationTests.Operations
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
        public void TestOracleConnectionQueryFirstViaPrimaryKey()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = connection.QueryFirst<CompleteTable>(table.Id);

            // Assert
            Assert.AreEqual(table.Id, result.Id);
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public void TestOracleConnectionQueryFirstViaExpression()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = connection.QueryFirst<CompleteTable>(e => e.Id == table.Id);

            // Assert
            Assert.AreEqual(table.Id, result.Id);
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public void TestOracleConnectionQueryFirstViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = connection.QueryFirst<CompleteTable>(new { table.Id });

            // Assert
            Assert.AreEqual(table.Id, result.Id);
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public void TestOracleConnectionQueryFirstViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = connection.QueryFirst<CompleteTable>(new QueryField("Id", table.Id));

            // Assert
            Assert.AreEqual(table.Id, result.Id);
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public void TestOracleConnectionQueryFirstViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = connection.QueryFirst<CompleteTable>(queryFields);

            // Assert
            Assert.AreEqual(table.Id, result.Id);
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public void TestOracleConnectionQueryFirstViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = connection.QueryFirst<CompleteTable>(new QueryGroup(queryFields));

            // Assert
            Assert.AreEqual(table.Id, result.Id);
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public void TestOracleConnectionQueryFirstWithOrderBy()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new OracleConnection(Database.ConnectionString);

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
        public void ThrowExceptionOnTestOracleConnectionQueryFirstIfNoRowsFound()
        {
            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            Assert.Throws<EmptyException>(() => connection.QueryFirst<CompleteTable>((object)null));
        }

        [TestMethod]
        public void ThrowExceptionOnTestOracleConnectionQueryFirstIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            Assert.Throws<EmptyException>(() => connection.QueryFirst<CompleteTable>(new QueryField("Id", -1)));
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestOracleConnectionQueryFirstAsyncViaPrimaryKey()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = await connection.QueryFirstAsync<CompleteTable>(table.Id).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(table.Id, result.Id);
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public async Task TestOracleConnectionQueryFirstAsyncViaExpression()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = await connection.QueryFirstAsync<CompleteTable>(e => e.Id == table.Id).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(table.Id, result.Id);
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public async Task TestOracleConnectionQueryFirstAsyncViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = await connection.QueryFirstAsync<CompleteTable>(new { table.Id }).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(table.Id, result.Id);
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public async Task TestOracleConnectionQueryFirstAsyncViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = await connection.QueryFirstAsync<CompleteTable>(new QueryField("Id", table.Id)).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(table.Id, result.Id);
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public async Task TestOracleConnectionQueryFirstAsyncViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = await connection.QueryFirstAsync<CompleteTable>(queryFields).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(table.Id, result.Id);
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public async Task TestOracleConnectionQueryFirstAsyncViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = await connection.QueryFirstAsync<CompleteTable>(new QueryGroup(queryFields)).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(table.Id, result.Id);
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public async Task TestOracleConnectionQueryFirstAsyncWithOrderBy()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new OracleConnection(Database.ConnectionString);

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
        public async Task ThrowExceptionOnTestOracleConnectionQueryFirstAsyncIfNoRowsFound()
        {
            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            await Assert.ThrowsAsync<EmptyException>(async () => await connection.QueryFirstAsync<CompleteTable>((object)null).ConfigureAwait(false)).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestOracleConnectionQueryFirstAsyncIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            await Assert.ThrowsAsync<EmptyException>(async () => await connection.QueryFirstAsync<CompleteTable>(new QueryField("Id", -1)).ConfigureAwait(false)).ConfigureAwait(false);
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestOracleConnectionQueryFirstViaTableNameViaPrimaryKey()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = connection.QueryFirst(ClassMappedNameCache.Get<CompleteTable>(), table.Id);

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(table.Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(table, kvp);
        }

        [TestMethod]
        public void TestOracleConnectionQueryFirstViaTableNameViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = connection.QueryFirst(ClassMappedNameCache.Get<CompleteTable>(), new { table.Id });

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(table.Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(table, kvp);
        }

        [TestMethod]
        public void TestOracleConnectionQueryFirstViaTableNameViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = connection.QueryFirst(ClassMappedNameCache.Get<CompleteTable>(), new QueryField("Id", table.Id));

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(table.Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(table, kvp);
        }

        [TestMethod]
        public void TestOracleConnectionQueryFirstViaTableNameViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = connection.QueryFirst(ClassMappedNameCache.Get<CompleteTable>(), queryFields);

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(table.Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(table, kvp);
        }

        [TestMethod]
        public void TestOracleConnectionQueryFirstViaTableNameViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = connection.QueryFirst(ClassMappedNameCache.Get<CompleteTable>(), new QueryGroup(queryFields));

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(table.Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(table, kvp);
        }

        [TestMethod]
        public void TestOracleConnectionQueryFirstViaTableNameWithOrderBy()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new OracleConnection(Database.ConnectionString);

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
        public void ThrowExceptionOnTestOracleConnectionQueryFirstViaTableNameIfNoRowsFound()
        {
            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            Assert.Throws<EmptyException>(() => connection.QueryFirst(ClassMappedNameCache.Get<CompleteTable>(), (object)null));
        }

        [TestMethod]
        public void ThrowExceptionOnTestOracleConnectionQueryFirstViaTableNameIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            Assert.Throws<EmptyException>(() => connection.QueryFirst(ClassMappedNameCache.Get<CompleteTable>(), new QueryField("Id", -1)));
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestOracleConnectionQueryFirstAsyncViaTableNameViaPrimaryKey()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = await connection.QueryFirstAsync(ClassMappedNameCache.Get<CompleteTable>(), table.Id).ConfigureAwait(false);

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(table.Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(table, kvp);
        }

        [TestMethod]
        public async Task TestOracleConnectionQueryFirstAsyncViaTableNameViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = await connection.QueryFirstAsync(ClassMappedNameCache.Get<CompleteTable>(), new { table.Id }).ConfigureAwait(false);

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(table.Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(table, kvp);
        }

        [TestMethod]
        public async Task TestOracleConnectionQueryFirstAsyncViaTableNameViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = await connection.QueryFirstAsync(ClassMappedNameCache.Get<CompleteTable>(), new QueryField("Id", table.Id)).ConfigureAwait(false);

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(table.Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(table, kvp);
        }

        [TestMethod]
        public async Task TestOracleConnectionQueryFirstAsyncViaTableNameViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = await connection.QueryFirstAsync(ClassMappedNameCache.Get<CompleteTable>(), queryFields).ConfigureAwait(false);

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(table.Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(table, kvp);
        }

        [TestMethod]
        public async Task TestOracleConnectionQueryFirstAsyncViaTableNameViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = await connection.QueryFirstAsync(ClassMappedNameCache.Get<CompleteTable>(), new QueryGroup(queryFields)).ConfigureAwait(false);

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(table.Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(table, kvp);
        }

        [TestMethod]
        public async Task TestOracleConnectionQueryFirstAsyncViaTableNameWithOrderBy()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using var connection = new OracleConnection(Database.ConnectionString);

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
        public async Task ThrowExceptionOnTestOracleConnectionQueryFirstAsyncViaTableNameIfNoRowsFound()
        {
            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            await Assert.ThrowsAsync<EmptyException>(async () => await connection.QueryFirstAsync(ClassMappedNameCache.Get<CompleteTable>(), (object)null).ConfigureAwait(false)).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestOracleConnectionQueryFirstAsyncViaTableNameIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            await Assert.ThrowsAsync<EmptyException>(async () => await connection.QueryFirstAsync(ClassMappedNameCache.Get<CompleteTable>(), new QueryField("Id", -1)).ConfigureAwait(false)).ConfigureAwait(false);
        }

        #endregion

        #endregion
    }
}
