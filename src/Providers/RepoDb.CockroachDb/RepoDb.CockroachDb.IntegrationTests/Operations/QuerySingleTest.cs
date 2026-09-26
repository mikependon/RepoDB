#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Connector.CockroachDb;
using RepoDb.Enumerations;
using RepoDb.Exceptions;
using RepoDb.CockroachDb.IntegrationTests.Models;
using RepoDb.CockroachDb.IntegrationTests.Setup;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.CockroachDb.IntegrationTests.Operations
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
        public void TestCockroachDbConnectionQuerySingleViaPrimaryKey()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QuerySingle<CompleteTable>(table.Id);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestCockroachDbConnectionQuerySingleViaExpression()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QuerySingle<CompleteTable>(e => e.Id == table.Id);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestCockroachDbConnectionQuerySingleViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QuerySingle<CompleteTable>(new { table.Id });

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestCockroachDbConnectionQuerySingleViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QuerySingle<CompleteTable>(new QueryField("Id", table.Id));

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestCockroachDbConnectionQuerySingleViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInteger", table.ColumnInteger)
            };

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QuerySingle<CompleteTable>(queryFields);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestCockroachDbConnectionQuerySingleViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInteger", table.ColumnInteger)
            };

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QuerySingle<CompleteTable>(new QueryGroup(queryFields));

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestCockroachDbConnectionQuerySingleWithTop()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).First();

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QuerySingle<CompleteTable>((object)null, orderBy: new[] { new OrderField("Id", Order.Ascending) }, top: 1);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestCockroachDbConnectionQuerySingleIfNoRowsFound()
        {
            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.QuerySingle<CompleteTable>((object)null));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestCockroachDbConnectionQuerySingleIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.QuerySingle<CompleteTable>(new QueryField("Id", -1L)));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestCockroachDbConnectionQuerySingleIfMultipleRowsFound()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<MultipleRowsFoundException>(() => connection.QuerySingle<CompleteTable>((object)null));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestCockroachDbConnectionQuerySingleAsyncViaPrimaryKey()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QuerySingleAsync<CompleteTable>(table.Id).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestCockroachDbConnectionQuerySingleAsyncViaExpression()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QuerySingleAsync<CompleteTable>(e => e.Id == table.Id).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestCockroachDbConnectionQuerySingleAsyncViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QuerySingleAsync<CompleteTable>(new { table.Id }).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestCockroachDbConnectionQuerySingleAsyncViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QuerySingleAsync<CompleteTable>(new QueryField("Id", table.Id)).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestCockroachDbConnectionQuerySingleAsyncViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInteger", table.ColumnInteger)
            };

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QuerySingleAsync<CompleteTable>(queryFields).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestCockroachDbConnectionQuerySingleAsyncViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInteger", table.ColumnInteger)
            };

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QuerySingleAsync<CompleteTable>(new QueryGroup(queryFields)).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestCockroachDbConnectionQuerySingleAsyncWithTop()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).First();

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QuerySingleAsync<CompleteTable>((object)null, orderBy: new[] { new OrderField("Id", Order.Ascending) }, top: 1).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestCockroachDbConnectionQuerySingleAsyncIfNoRowsFound()
        {
            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.QuerySingleAsync<CompleteTable>((object)null).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestCockroachDbConnectionQuerySingleAsyncIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.QuerySingleAsync<CompleteTable>(new QueryField("Id", -1L)).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestCockroachDbConnectionQuerySingleAsyncIfMultipleRowsFound()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<MultipleRowsFoundException>(async () => await connection.QuerySingleAsync<CompleteTable>((object)null).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestCockroachDbConnectionQuerySingleViaTableNameViaPrimaryKey()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QuerySingle(ClassMappedNameCache.Get<CompleteTable>(), table.Id);

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(table.Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(table, kvp);
            }
        }

        [TestMethod]
        public void TestCockroachDbConnectionQuerySingleViaTableNameViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QuerySingle(ClassMappedNameCache.Get<CompleteTable>(), new { table.Id });

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(table.Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(table, kvp);
            }
        }

        [TestMethod]
        public void TestCockroachDbConnectionQuerySingleViaTableNameViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QuerySingle(ClassMappedNameCache.Get<CompleteTable>(), new QueryField("Id", table.Id));

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(table.Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(table, kvp);
            }
        }

        [TestMethod]
        public void TestCockroachDbConnectionQuerySingleViaTableNameViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInteger", table.ColumnInteger)
            };

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QuerySingle(ClassMappedNameCache.Get<CompleteTable>(), queryFields);

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(table.Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(table, kvp);
            }
        }

        [TestMethod]
        public void TestCockroachDbConnectionQuerySingleViaTableNameViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInteger", table.ColumnInteger)
            };

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QuerySingle(ClassMappedNameCache.Get<CompleteTable>(), new QueryGroup(queryFields));

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(table.Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(table, kvp);
            }
        }

        [TestMethod]
        public void TestCockroachDbConnectionQuerySingleViaTableNameWithTop()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).First();

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QuerySingle(ClassMappedNameCache.Get<CompleteTable>(), (object)null, orderBy: new[] { new OrderField("Id", Order.Ascending) }, top: 1);

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(table.Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(table, kvp);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestCockroachDbConnectionQuerySingleViaTableNameIfNoRowsFound()
        {
            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.QuerySingle(ClassMappedNameCache.Get<CompleteTable>(), (object)null));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestCockroachDbConnectionQuerySingleViaTableNameIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.QuerySingle(ClassMappedNameCache.Get<CompleteTable>(), new QueryField("Id", -1L)));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestCockroachDbConnectionQuerySingleViaTableNameIfMultipleRowsFound()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<MultipleRowsFoundException>(() => connection.QuerySingle(ClassMappedNameCache.Get<CompleteTable>(), (object)null));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestCockroachDbConnectionQuerySingleAsyncViaTableNameViaPrimaryKey()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QuerySingleAsync(ClassMappedNameCache.Get<CompleteTable>(), table.Id).ConfigureAwait(false);

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(table.Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(table, kvp);
            }
        }

        [TestMethod]
        public async Task TestCockroachDbConnectionQuerySingleAsyncViaTableNameViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QuerySingleAsync(ClassMappedNameCache.Get<CompleteTable>(), new { table.Id }).ConfigureAwait(false);

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(table.Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(table, kvp);
            }
        }

        [TestMethod]
        public async Task TestCockroachDbConnectionQuerySingleAsyncViaTableNameViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QuerySingleAsync(ClassMappedNameCache.Get<CompleteTable>(), new QueryField("Id", table.Id)).ConfigureAwait(false);

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(table.Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(table, kvp);
            }
        }

        [TestMethod]
        public async Task TestCockroachDbConnectionQuerySingleAsyncViaTableNameViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInteger", table.ColumnInteger)
            };

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QuerySingleAsync(ClassMappedNameCache.Get<CompleteTable>(), queryFields).ConfigureAwait(false);

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(table.Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(table, kvp);
            }
        }

        [TestMethod]
        public async Task TestCockroachDbConnectionQuerySingleAsyncViaTableNameViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInteger", table.ColumnInteger)
            };

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QuerySingleAsync(ClassMappedNameCache.Get<CompleteTable>(), new QueryGroup(queryFields)).ConfigureAwait(false);

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(table.Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(table, kvp);
            }
        }

        [TestMethod]
        public async Task TestCockroachDbConnectionQuerySingleAsyncViaTableNameWithTop()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).First();

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QuerySingleAsync(ClassMappedNameCache.Get<CompleteTable>(), (object)null, orderBy: new[] { new OrderField("Id", Order.Ascending) }, top: 1).ConfigureAwait(false);

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(table.Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(table, kvp);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestCockroachDbConnectionQuerySingleAsyncViaTableNameIfNoRowsFound()
        {
            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.QuerySingleAsync(ClassMappedNameCache.Get<CompleteTable>(), (object)null).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestCockroachDbConnectionQuerySingleAsyncViaTableNameIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.QuerySingleAsync(ClassMappedNameCache.Get<CompleteTable>(), new QueryField("Id", -1L)).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestCockroachDbConnectionQuerySingleAsyncViaTableNameIfMultipleRowsFound()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<MultipleRowsFoundException>(async () => await connection.QuerySingleAsync(ClassMappedNameCache.Get<CompleteTable>(), (object)null).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        #endregion

        #endregion
    }
}
