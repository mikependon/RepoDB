#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClickHouse.Driver;
using ClickHouse.Driver.ADO;
using RepoDb.Enumerations;
using RepoDb.Exceptions;
using RepoDb.ClickHouse.IntegrationTests.Models;
using RepoDb.ClickHouse.IntegrationTests.Setup;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.ClickHouse.IntegrationTests.Operations
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
        public void TestClickHouseConnectionQuerySingleViaPrimaryKey()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QuerySingle<CompleteTable>(table.Id);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionQuerySingleViaExpression()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QuerySingle<CompleteTable>(e => e.Id == table.Id);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionQuerySingleViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QuerySingle<CompleteTable>(new { table.Id });

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionQuerySingleViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QuerySingle<CompleteTable>(new QueryField("Id", table.Id));

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionQuerySingleViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QuerySingle<CompleteTable>(queryFields);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionQuerySingleViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QuerySingle<CompleteTable>(new QueryGroup(queryFields));

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionQuerySingleWithTop()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).First();

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QuerySingle<CompleteTable>((object)null, orderBy: new[] { new OrderField("Id", Order.Ascending) }, top: 1);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestClickHouseConnectionQuerySingleIfNoRowsFound()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.QuerySingle<CompleteTable>((object)null));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestClickHouseConnectionQuerySingleIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.QuerySingle<CompleteTable>(new QueryField("Id", -1L)));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestClickHouseConnectionQuerySingleIfMultipleRowsFound()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<MultipleRowsFoundException>(() => connection.QuerySingle<CompleteTable>((object)null));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestClickHouseConnectionQuerySingleAsyncViaPrimaryKey()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QuerySingleAsync<CompleteTable>(table.Id).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestClickHouseConnectionQuerySingleAsyncViaExpression()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QuerySingleAsync<CompleteTable>(e => e.Id == table.Id).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestClickHouseConnectionQuerySingleAsyncViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QuerySingleAsync<CompleteTable>(new { table.Id }).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestClickHouseConnectionQuerySingleAsyncViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QuerySingleAsync<CompleteTable>(new QueryField("Id", table.Id)).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestClickHouseConnectionQuerySingleAsyncViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QuerySingleAsync<CompleteTable>(queryFields).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestClickHouseConnectionQuerySingleAsyncViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QuerySingleAsync<CompleteTable>(new QueryGroup(queryFields)).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestClickHouseConnectionQuerySingleAsyncWithTop()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).First();

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QuerySingleAsync<CompleteTable>((object)null, orderBy: new[] { new OrderField("Id", Order.Ascending) }, top: 1).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestClickHouseConnectionQuerySingleAsyncIfNoRowsFound()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.QuerySingleAsync<CompleteTable>((object)null).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestClickHouseConnectionQuerySingleAsyncIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.QuerySingleAsync<CompleteTable>(new QueryField("Id", -1L)).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestClickHouseConnectionQuerySingleAsyncIfMultipleRowsFound()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
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
        public void TestClickHouseConnectionQuerySingleViaTableNameViaPrimaryKey()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
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
        public void TestClickHouseConnectionQuerySingleViaTableNameViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
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
        public void TestClickHouseConnectionQuerySingleViaTableNameViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
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
        public void TestClickHouseConnectionQuerySingleViaTableNameViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
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
        public void TestClickHouseConnectionQuerySingleViaTableNameViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
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
        public void TestClickHouseConnectionQuerySingleViaTableNameWithTop()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).First();

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
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
        public void ThrowExceptionOnTestClickHouseConnectionQuerySingleViaTableNameIfNoRowsFound()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.QuerySingle(ClassMappedNameCache.Get<CompleteTable>(), (object)null));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestClickHouseConnectionQuerySingleViaTableNameIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.QuerySingle(ClassMappedNameCache.Get<CompleteTable>(), new QueryField("Id", -1L)));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestClickHouseConnectionQuerySingleViaTableNameIfMultipleRowsFound()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<MultipleRowsFoundException>(() => connection.QuerySingle(ClassMappedNameCache.Get<CompleteTable>(), (object)null));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestClickHouseConnectionQuerySingleAsyncViaTableNameViaPrimaryKey()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
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
        public async Task TestClickHouseConnectionQuerySingleAsyncViaTableNameViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
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
        public async Task TestClickHouseConnectionQuerySingleAsyncViaTableNameViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
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
        public async Task TestClickHouseConnectionQuerySingleAsyncViaTableNameViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
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
        public async Task TestClickHouseConnectionQuerySingleAsyncViaTableNameViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
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
        public async Task TestClickHouseConnectionQuerySingleAsyncViaTableNameWithTop()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).First();

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
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
        public async Task ThrowExceptionOnTestClickHouseConnectionQuerySingleAsyncViaTableNameIfNoRowsFound()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.QuerySingleAsync(ClassMappedNameCache.Get<CompleteTable>(), (object)null).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestClickHouseConnectionQuerySingleAsyncViaTableNameIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.QuerySingleAsync(ClassMappedNameCache.Get<CompleteTable>(), new QueryField("Id", -1L)).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestClickHouseConnectionQuerySingleAsyncViaTableNameIfMultipleRowsFound()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<MultipleRowsFoundException>(async () => await connection.QuerySingleAsync(ClassMappedNameCache.Get<CompleteTable>(), (object)null).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        #endregion

        #endregion
    }
}
