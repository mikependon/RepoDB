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
        public void TestOracleConnectionQuerySingleViaPrimaryKey()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = connection.QuerySingle<CompleteTable>(table.Id);

            // Assert
            Assert.AreEqual(table.Id, result.Id);
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public void TestOracleConnectionQuerySingleViaExpression()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = connection.QuerySingle<CompleteTable>(e => e.Id == table.Id);

            // Assert
            Assert.AreEqual(table.Id, result.Id);
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public void TestOracleConnectionQuerySingleViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = connection.QuerySingle<CompleteTable>(new { table.Id });

            // Assert
            Assert.AreEqual(table.Id, result.Id);
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public void TestOracleConnectionQuerySingleViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = connection.QuerySingle<CompleteTable>(new QueryField("Id", table.Id));

            // Assert
            Assert.AreEqual(table.Id, result.Id);
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public void TestOracleConnectionQuerySingleViaQueryFields()
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
            var result = connection.QuerySingle<CompleteTable>(queryFields);

            // Assert
            Assert.AreEqual(table.Id, result.Id);
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public void TestOracleConnectionQuerySingleViaQueryGroup()
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
            var result = connection.QuerySingle<CompleteTable>(new QueryGroup(queryFields));

            // Assert
            Assert.AreEqual(table.Id, result.Id);
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public void TestOracleConnectionQuerySingleWithTop()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).First();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = connection.QuerySingle<CompleteTable>((object)null, orderBy: new[] { new OrderField("Id", Order.Ascending) }, top: 1);

            // Assert
            Assert.AreEqual(table.Id, result.Id);
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public void ThrowExceptionOnTestOracleConnectionQuerySingleIfNoRowsFound()
        {
            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            Assert.Throws<EmptyException>(() => connection.QuerySingle<CompleteTable>((object)null));
        }

        [TestMethod]
        public void ThrowExceptionOnTestOracleConnectionQuerySingleIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            Assert.Throws<EmptyException>(() => connection.QuerySingle<CompleteTable>(new QueryField("Id", -1)));
        }

        [TestMethod]
        public void ThrowExceptionOnTestOracleConnectionQuerySingleIfMultipleRowsFound()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            Assert.Throws<MultipleRowsFoundException>(() => connection.QuerySingle<CompleteTable>((object)null));
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestOracleConnectionQuerySingleAsyncViaPrimaryKey()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = await connection.QuerySingleAsync<CompleteTable>(table.Id).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(table.Id, result.Id);
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public async Task TestOracleConnectionQuerySingleAsyncViaExpression()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = await connection.QuerySingleAsync<CompleteTable>(e => e.Id == table.Id).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(table.Id, result.Id);
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public async Task TestOracleConnectionQuerySingleAsyncViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = await connection.QuerySingleAsync<CompleteTable>(new { table.Id }).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(table.Id, result.Id);
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public async Task TestOracleConnectionQuerySingleAsyncViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = await connection.QuerySingleAsync<CompleteTable>(new QueryField("Id", table.Id)).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(table.Id, result.Id);
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public async Task TestOracleConnectionQuerySingleAsyncViaQueryFields()
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
            var result = await connection.QuerySingleAsync<CompleteTable>(queryFields).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(table.Id, result.Id);
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public async Task TestOracleConnectionQuerySingleAsyncViaQueryGroup()
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
            var result = await connection.QuerySingleAsync<CompleteTable>(new QueryGroup(queryFields)).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(table.Id, result.Id);
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public async Task TestOracleConnectionQuerySingleAsyncWithTop()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).First();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = await connection.QuerySingleAsync<CompleteTable>((object)null, orderBy: new[] { new OrderField("Id", Order.Ascending) }, top: 1).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(table.Id, result.Id);
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestOracleConnectionQuerySingleAsyncIfNoRowsFound()
        {
            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            await Assert.ThrowsAsync<EmptyException>(async () => await connection.QuerySingleAsync<CompleteTable>((object)null).ConfigureAwait(false)).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestOracleConnectionQuerySingleAsyncIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            await Assert.ThrowsAsync<EmptyException>(async () => await connection.QuerySingleAsync<CompleteTable>(new QueryField("Id", -1)).ConfigureAwait(false)).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestOracleConnectionQuerySingleAsyncIfMultipleRowsFound()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            await Assert.ThrowsAsync<MultipleRowsFoundException>(async () => await connection.QuerySingleAsync<CompleteTable>((object)null).ConfigureAwait(false)).ConfigureAwait(false);
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestOracleConnectionQuerySingleViaTableNameViaPrimaryKey()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = connection.QuerySingle(ClassMappedNameCache.Get<CompleteTable>(), table.Id);

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(table.Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(table, kvp);
        }

        [TestMethod]
        public void TestOracleConnectionQuerySingleViaTableNameViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = connection.QuerySingle(ClassMappedNameCache.Get<CompleteTable>(), new { table.Id });

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(table.Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(table, kvp);
        }

        [TestMethod]
        public void TestOracleConnectionQuerySingleViaTableNameViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = connection.QuerySingle(ClassMappedNameCache.Get<CompleteTable>(), new QueryField("Id", table.Id));

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(table.Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(table, kvp);
        }

        [TestMethod]
        public void TestOracleConnectionQuerySingleViaTableNameViaQueryFields()
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
            var result = connection.QuerySingle(ClassMappedNameCache.Get<CompleteTable>(), queryFields);

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(table.Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(table, kvp);
        }

        [TestMethod]
        public void TestOracleConnectionQuerySingleViaTableNameViaQueryGroup()
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
            var result = connection.QuerySingle(ClassMappedNameCache.Get<CompleteTable>(), new QueryGroup(queryFields));

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(table.Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(table, kvp);
        }

        [TestMethod]
        public void TestOracleConnectionQuerySingleViaTableNameWithTop()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).First();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = connection.QuerySingle(ClassMappedNameCache.Get<CompleteTable>(), (object)null, orderBy: new[] { new OrderField("Id", Order.Ascending) }, top: 1);

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(table.Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(table, kvp);
        }

        [TestMethod]
        public void ThrowExceptionOnTestOracleConnectionQuerySingleViaTableNameIfNoRowsFound()
        {
            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            Assert.Throws<EmptyException>(() => connection.QuerySingle(ClassMappedNameCache.Get<CompleteTable>(), (object)null));
        }

        [TestMethod]
        public void ThrowExceptionOnTestOracleConnectionQuerySingleViaTableNameIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            Assert.Throws<EmptyException>(() => connection.QuerySingle(ClassMappedNameCache.Get<CompleteTable>(), new QueryField("Id", -1)));
        }

        [TestMethod]
        public void ThrowExceptionOnTestOracleConnectionQuerySingleViaTableNameIfMultipleRowsFound()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            Assert.Throws<MultipleRowsFoundException>(() => connection.QuerySingle(ClassMappedNameCache.Get<CompleteTable>(), (object)null));
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestOracleConnectionQuerySingleAsyncViaTableNameViaPrimaryKey()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = await connection.QuerySingleAsync(ClassMappedNameCache.Get<CompleteTable>(), table.Id).ConfigureAwait(false);

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(table.Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(table, kvp);
        }

        [TestMethod]
        public async Task TestOracleConnectionQuerySingleAsyncViaTableNameViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = await connection.QuerySingleAsync(ClassMappedNameCache.Get<CompleteTable>(), new { table.Id }).ConfigureAwait(false);

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(table.Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(table, kvp);
        }

        [TestMethod]
        public async Task TestOracleConnectionQuerySingleAsyncViaTableNameViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = await connection.QuerySingleAsync(ClassMappedNameCache.Get<CompleteTable>(), new QueryField("Id", table.Id)).ConfigureAwait(false);

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(table.Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(table, kvp);
        }

        [TestMethod]
        public async Task TestOracleConnectionQuerySingleAsyncViaTableNameViaQueryFields()
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
            var result = await connection.QuerySingleAsync(ClassMappedNameCache.Get<CompleteTable>(), queryFields).ConfigureAwait(false);

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(table.Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(table, kvp);
        }

        [TestMethod]
        public async Task TestOracleConnectionQuerySingleAsyncViaTableNameViaQueryGroup()
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
            var result = await connection.QuerySingleAsync(ClassMappedNameCache.Get<CompleteTable>(), new QueryGroup(queryFields)).ConfigureAwait(false);

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(table.Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(table, kvp);
        }

        [TestMethod]
        public async Task TestOracleConnectionQuerySingleAsyncViaTableNameWithTop()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).First();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = await connection.QuerySingleAsync(ClassMappedNameCache.Get<CompleteTable>(), (object)null, orderBy: new[] { new OrderField("Id", Order.Ascending) }, top: 1).ConfigureAwait(false);

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(table.Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(table, kvp);
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestOracleConnectionQuerySingleAsyncViaTableNameIfNoRowsFound()
        {
            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            await Assert.ThrowsAsync<EmptyException>(async () => await connection.QuerySingleAsync(ClassMappedNameCache.Get<CompleteTable>(), (object)null).ConfigureAwait(false)).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestOracleConnectionQuerySingleAsyncViaTableNameIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            await Assert.ThrowsAsync<EmptyException>(async () => await connection.QuerySingleAsync(ClassMappedNameCache.Get<CompleteTable>(), new QueryField("Id", -1)).ConfigureAwait(false)).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestOracleConnectionQuerySingleAsyncViaTableNameIfMultipleRowsFound()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            await Assert.ThrowsAsync<MultipleRowsFoundException>(async () => await connection.QuerySingleAsync(ClassMappedNameCache.Get<CompleteTable>(), (object)null).ConfigureAwait(false)).ConfigureAwait(false);
        }

        #endregion

        #endregion
    }
}
