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
        public void TestDb2ConnectionQuerySingleViaPrimaryKey()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = connection.QuerySingle<CompleteTable>(table.Id);

            // Assert
            Assert.AreEqual(table.Id, result.Id);
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public void TestDb2ConnectionQuerySingleViaExpression()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = connection.QuerySingle<CompleteTable>(e => e.Id == table.Id);

            // Assert
            Assert.AreEqual(table.Id, result.Id);
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public void TestDb2ConnectionQuerySingleViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = connection.QuerySingle<CompleteTable>(new { table.Id });

            // Assert
            Assert.AreEqual(table.Id, result.Id);
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public void TestDb2ConnectionQuerySingleViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = connection.QuerySingle<CompleteTable>(new QueryField("Id", table.Id));

            // Assert
            Assert.AreEqual(table.Id, result.Id);
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public void TestDb2ConnectionQuerySingleViaQueryFields()
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
            var result = connection.QuerySingle<CompleteTable>(queryFields);

            // Assert
            Assert.AreEqual(table.Id, result.Id);
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public void TestDb2ConnectionQuerySingleViaQueryGroup()
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
            var result = connection.QuerySingle<CompleteTable>(new QueryGroup(queryFields));

            // Assert
            Assert.AreEqual(table.Id, result.Id);
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public void TestDb2ConnectionQuerySingleWithTop()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).First();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = connection.QuerySingle<CompleteTable>((object)null, orderBy: new[] { new OrderField("Id", Order.Ascending) }, top: 1);

            // Assert
            Assert.AreEqual(table.Id, result.Id);
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public void ThrowExceptionOnTestDb2ConnectionQuerySingleIfNoRowsFound()
        {
            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            Assert.Throws<EmptyException>(() => connection.QuerySingle<CompleteTable>((object)null));
        }

        [TestMethod]
        public void ThrowExceptionOnTestDb2ConnectionQuerySingleIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            Assert.Throws<EmptyException>(() => connection.QuerySingle<CompleteTable>(new QueryField("Id", -1)));
        }

        [TestMethod]
        public void ThrowExceptionOnTestDb2ConnectionQuerySingleIfMultipleRowsFound()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            Assert.Throws<MultipleRowsFoundException>(() => connection.QuerySingle<CompleteTable>((object)null));
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestDb2ConnectionQuerySingleAsyncViaPrimaryKey()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = await connection.QuerySingleAsync<CompleteTable>(table.Id).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(table.Id, result.Id);
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public async Task TestDb2ConnectionQuerySingleAsyncViaExpression()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = await connection.QuerySingleAsync<CompleteTable>(e => e.Id == table.Id).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(table.Id, result.Id);
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public async Task TestDb2ConnectionQuerySingleAsyncViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = await connection.QuerySingleAsync<CompleteTable>(new { table.Id }).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(table.Id, result.Id);
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public async Task TestDb2ConnectionQuerySingleAsyncViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = await connection.QuerySingleAsync<CompleteTable>(new QueryField("Id", table.Id)).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(table.Id, result.Id);
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public async Task TestDb2ConnectionQuerySingleAsyncViaQueryFields()
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
            var result = await connection.QuerySingleAsync<CompleteTable>(queryFields).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(table.Id, result.Id);
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public async Task TestDb2ConnectionQuerySingleAsyncViaQueryGroup()
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
            var result = await connection.QuerySingleAsync<CompleteTable>(new QueryGroup(queryFields)).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(table.Id, result.Id);
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public async Task TestDb2ConnectionQuerySingleAsyncWithTop()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).First();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = await connection.QuerySingleAsync<CompleteTable>((object)null, orderBy: new[] { new OrderField("Id", Order.Ascending) }, top: 1).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(table.Id, result.Id);
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestDb2ConnectionQuerySingleAsyncIfNoRowsFound()
        {
            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            await Assert.ThrowsAsync<EmptyException>(async () => await connection.QuerySingleAsync<CompleteTable>((object)null).ConfigureAwait(false)).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestDb2ConnectionQuerySingleAsyncIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            await Assert.ThrowsAsync<EmptyException>(async () => await connection.QuerySingleAsync<CompleteTable>(new QueryField("Id", -1)).ConfigureAwait(false)).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestDb2ConnectionQuerySingleAsyncIfMultipleRowsFound()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            await Assert.ThrowsAsync<MultipleRowsFoundException>(async () => await connection.QuerySingleAsync<CompleteTable>((object)null).ConfigureAwait(false)).ConfigureAwait(false);
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestDb2ConnectionQuerySingleViaTableNameViaPrimaryKey()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = connection.QuerySingle(ClassMappedNameCache.Get<CompleteTable>(), table.Id);

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(table.Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(table, kvp);
        }

        [TestMethod]
        public void TestDb2ConnectionQuerySingleViaTableNameViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = connection.QuerySingle(ClassMappedNameCache.Get<CompleteTable>(), new { table.Id });

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(table.Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(table, kvp);
        }

        [TestMethod]
        public void TestDb2ConnectionQuerySingleViaTableNameViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = connection.QuerySingle(ClassMappedNameCache.Get<CompleteTable>(), new QueryField("Id", table.Id));

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(table.Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(table, kvp);
        }

        [TestMethod]
        public void TestDb2ConnectionQuerySingleViaTableNameViaQueryFields()
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
            var result = connection.QuerySingle(ClassMappedNameCache.Get<CompleteTable>(), queryFields);

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(table.Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(table, kvp);
        }

        [TestMethod]
        public void TestDb2ConnectionQuerySingleViaTableNameViaQueryGroup()
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
            var result = connection.QuerySingle(ClassMappedNameCache.Get<CompleteTable>(), new QueryGroup(queryFields));

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(table.Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(table, kvp);
        }

        [TestMethod]
        public void TestDb2ConnectionQuerySingleViaTableNameWithTop()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).First();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = connection.QuerySingle(ClassMappedNameCache.Get<CompleteTable>(), (object)null, orderBy: new[] { new OrderField("Id", Order.Ascending) }, top: 1);

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(table.Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(table, kvp);
        }

        [TestMethod]
        public void ThrowExceptionOnTestDb2ConnectionQuerySingleViaTableNameIfNoRowsFound()
        {
            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            Assert.Throws<EmptyException>(() => connection.QuerySingle(ClassMappedNameCache.Get<CompleteTable>(), (object)null));
        }

        [TestMethod]
        public void ThrowExceptionOnTestDb2ConnectionQuerySingleViaTableNameIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            Assert.Throws<EmptyException>(() => connection.QuerySingle(ClassMappedNameCache.Get<CompleteTable>(), new QueryField("Id", -1)));
        }

        [TestMethod]
        public void ThrowExceptionOnTestDb2ConnectionQuerySingleViaTableNameIfMultipleRowsFound()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            Assert.Throws<MultipleRowsFoundException>(() => connection.QuerySingle(ClassMappedNameCache.Get<CompleteTable>(), (object)null));
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestDb2ConnectionQuerySingleAsyncViaTableNameViaPrimaryKey()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = await connection.QuerySingleAsync(ClassMappedNameCache.Get<CompleteTable>(), table.Id).ConfigureAwait(false);

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(table.Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(table, kvp);
        }

        [TestMethod]
        public async Task TestDb2ConnectionQuerySingleAsyncViaTableNameViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = await connection.QuerySingleAsync(ClassMappedNameCache.Get<CompleteTable>(), new { table.Id }).ConfigureAwait(false);

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(table.Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(table, kvp);
        }

        [TestMethod]
        public async Task TestDb2ConnectionQuerySingleAsyncViaTableNameViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = await connection.QuerySingleAsync(ClassMappedNameCache.Get<CompleteTable>(), new QueryField("Id", table.Id)).ConfigureAwait(false);

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(table.Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(table, kvp);
        }

        [TestMethod]
        public async Task TestDb2ConnectionQuerySingleAsyncViaTableNameViaQueryFields()
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
            var result = await connection.QuerySingleAsync(ClassMappedNameCache.Get<CompleteTable>(), queryFields).ConfigureAwait(false);

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(table.Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(table, kvp);
        }

        [TestMethod]
        public async Task TestDb2ConnectionQuerySingleAsyncViaTableNameViaQueryGroup()
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
            var result = await connection.QuerySingleAsync(ClassMappedNameCache.Get<CompleteTable>(), new QueryGroup(queryFields)).ConfigureAwait(false);

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(table.Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(table, kvp);
        }

        [TestMethod]
        public async Task TestDb2ConnectionQuerySingleAsyncViaTableNameWithTop()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).First();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = await connection.QuerySingleAsync(ClassMappedNameCache.Get<CompleteTable>(), (object)null, orderBy: new[] { new OrderField("Id", Order.Ascending) }, top: 1).ConfigureAwait(false);

            // Assert
            var kvp = (IDictionary<string, object>)result;
            Assert.AreEqual(table.Id, System.Convert.ToInt32(kvp["Id"], CultureInfo.InvariantCulture));
            Helper.AssertMembersEquality(table, kvp);
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestDb2ConnectionQuerySingleAsyncViaTableNameIfNoRowsFound()
        {
            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            await Assert.ThrowsAsync<EmptyException>(async () => await connection.QuerySingleAsync(ClassMappedNameCache.Get<CompleteTable>(), (object)null).ConfigureAwait(false)).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestDb2ConnectionQuerySingleAsyncViaTableNameIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            await Assert.ThrowsAsync<EmptyException>(async () => await connection.QuerySingleAsync(ClassMappedNameCache.Get<CompleteTable>(), new QueryField("Id", -1)).ConfigureAwait(false)).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestDb2ConnectionQuerySingleAsyncViaTableNameIfMultipleRowsFound()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            await Assert.ThrowsAsync<MultipleRowsFoundException>(async () => await connection.QuerySingleAsync(ClassMappedNameCache.Get<CompleteTable>(), (object)null).ConfigureAwait(false)).ConfigureAwait(false);
        }

        #endregion

        #endregion
    }
}
