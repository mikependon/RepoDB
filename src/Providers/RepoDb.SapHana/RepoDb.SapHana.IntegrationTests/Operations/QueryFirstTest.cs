#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sap.Data.Hana;
using RepoDb.Enumerations;
using RepoDb.Exceptions;
using RepoDb.SapHana.IntegrationTests.Models;
using RepoDb.SapHana.IntegrationTests.Setup;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.SapHana.IntegrationTests.Operations
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
        public void TestHanaConnectionQueryFirstViaPrimaryKey()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QueryFirst<CompleteTable>(table.Id);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestHanaConnectionQueryFirstViaExpression()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QueryFirst<CompleteTable>(e => e.Id == table.Id);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestHanaConnectionQueryFirstViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QueryFirst<CompleteTable>(new { table.Id });

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestHanaConnectionQueryFirstViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QueryFirst<CompleteTable>(new QueryField("Id", table.Id));

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestHanaConnectionQueryFirstViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QueryFirst<CompleteTable>(queryFields);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestHanaConnectionQueryFirstViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QueryFirst<CompleteTable>(new QueryGroup(queryFields));

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestHanaConnectionQueryFirstWithOrderBy()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var ascending = connection.QueryFirst<CompleteTable>((object)null, orderBy: new[] { new OrderField("Id", Order.Ascending) });
                var descending = connection.QueryFirst<CompleteTable>((object)null, orderBy: new[] { new OrderField("Id", Order.Descending) });

                // Assert
                Assert.AreEqual(tables.First().Id, ascending.Id);
                Helper.AssertPropertiesEquality(tables.First(), ascending);
                Assert.AreEqual(tables.Last().Id, descending.Id);
                Helper.AssertPropertiesEquality(tables.Last(), descending);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestHanaConnectionQueryFirstIfNoRowsFound()
        {
            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.QueryFirst<CompleteTable>((object)null));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestHanaConnectionQueryFirstIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.QueryFirst<CompleteTable>(new QueryField("Id", -1L)));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestHanaConnectionQueryFirstAsyncViaPrimaryKey()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QueryFirstAsync<CompleteTable>(table.Id).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestHanaConnectionQueryFirstAsyncViaExpression()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QueryFirstAsync<CompleteTable>(e => e.Id == table.Id).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestHanaConnectionQueryFirstAsyncViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QueryFirstAsync<CompleteTable>(new { table.Id }).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestHanaConnectionQueryFirstAsyncViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QueryFirstAsync<CompleteTable>(new QueryField("Id", table.Id)).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestHanaConnectionQueryFirstAsyncViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QueryFirstAsync<CompleteTable>(queryFields).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestHanaConnectionQueryFirstAsyncViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QueryFirstAsync<CompleteTable>(new QueryGroup(queryFields)).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(table.Id, result.Id);
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestHanaConnectionQueryFirstAsyncWithOrderBy()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var ascending = await connection.QueryFirstAsync<CompleteTable>((object)null, orderBy: new[] { new OrderField("Id", Order.Ascending) }).ConfigureAwait(false);
                var descending = await connection.QueryFirstAsync<CompleteTable>((object)null, orderBy: new[] { new OrderField("Id", Order.Descending) }).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.First().Id, ascending.Id);
                Helper.AssertPropertiesEquality(tables.First(), ascending);
                Assert.AreEqual(tables.Last().Id, descending.Id);
                Helper.AssertPropertiesEquality(tables.Last(), descending);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestHanaConnectionQueryFirstAsyncIfNoRowsFound()
        {
            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.QueryFirstAsync<CompleteTable>((object)null).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestHanaConnectionQueryFirstAsyncIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.QueryFirstAsync<CompleteTable>(new QueryField("Id", -1L)).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestHanaConnectionQueryFirstViaTableNameViaPrimaryKey()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QueryFirst(ClassMappedNameCache.Get<CompleteTable>(), table.Id);

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(table.Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(table, kvp);
            }
        }

        [TestMethod]
        public void TestHanaConnectionQueryFirstViaTableNameViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QueryFirst(ClassMappedNameCache.Get<CompleteTable>(), new { table.Id });

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(table.Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(table, kvp);
            }
        }

        [TestMethod]
        public void TestHanaConnectionQueryFirstViaTableNameViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QueryFirst(ClassMappedNameCache.Get<CompleteTable>(), new QueryField("Id", table.Id));

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(table.Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(table, kvp);
            }
        }

        [TestMethod]
        public void TestHanaConnectionQueryFirstViaTableNameViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QueryFirst(ClassMappedNameCache.Get<CompleteTable>(), queryFields);

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(table.Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(table, kvp);
            }
        }

        [TestMethod]
        public void TestHanaConnectionQueryFirstViaTableNameViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QueryFirst(ClassMappedNameCache.Get<CompleteTable>(), new QueryGroup(queryFields));

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(table.Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(table, kvp);
            }
        }

        [TestMethod]
        public void TestHanaConnectionQueryFirstViaTableNameWithOrderBy()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
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
        }

        [TestMethod]
        public void ThrowExceptionOnTestHanaConnectionQueryFirstViaTableNameIfNoRowsFound()
        {
            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.QueryFirst(ClassMappedNameCache.Get<CompleteTable>(), (object)null));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnTestHanaConnectionQueryFirstViaTableNameIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<EmptyException>(() => connection.QueryFirst(ClassMappedNameCache.Get<CompleteTable>(), new QueryField("Id", -1L)));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestHanaConnectionQueryFirstAsyncViaTableNameViaPrimaryKey()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QueryFirstAsync(ClassMappedNameCache.Get<CompleteTable>(), table.Id).ConfigureAwait(false);

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(table.Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(table, kvp);
            }
        }

        [TestMethod]
        public async Task TestHanaConnectionQueryFirstAsyncViaTableNameViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QueryFirstAsync(ClassMappedNameCache.Get<CompleteTable>(), new { table.Id }).ConfigureAwait(false);

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(table.Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(table, kvp);
            }
        }

        [TestMethod]
        public async Task TestHanaConnectionQueryFirstAsyncViaTableNameViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QueryFirstAsync(ClassMappedNameCache.Get<CompleteTable>(), new QueryField("Id", table.Id)).ConfigureAwait(false);

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(table.Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(table, kvp);
            }
        }

        [TestMethod]
        public async Task TestHanaConnectionQueryFirstAsyncViaTableNameViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QueryFirstAsync(ClassMappedNameCache.Get<CompleteTable>(), queryFields).ConfigureAwait(false);

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(table.Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(table, kvp);
            }
        }

        [TestMethod]
        public async Task TestHanaConnectionQueryFirstAsyncViaTableNameViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(10).Last();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QueryFirstAsync(ClassMappedNameCache.Get<CompleteTable>(), new QueryGroup(queryFields)).ConfigureAwait(false);

                // Assert
                var kvp = (IDictionary<string, object>)result;
                Assert.AreEqual(table.Id, System.Convert.ToInt64(kvp["Id"], CultureInfo.InvariantCulture));
                Helper.AssertMembersEquality(table, kvp);
            }
        }

        [TestMethod]
        public async Task TestHanaConnectionQueryFirstAsyncViaTableNameWithOrderBy()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
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
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestHanaConnectionQueryFirstAsyncViaTableNameIfNoRowsFound()
        {
            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.QueryFirstAsync(ClassMappedNameCache.Get<CompleteTable>(), (object)null).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnTestHanaConnectionQueryFirstAsyncViaTableNameIfNoRowsMatchTheFilter()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<EmptyException>(async () => await connection.QueryFirstAsync(ClassMappedNameCache.Get<CompleteTable>(), new QueryField("Id", -1L)).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        #endregion

        #endregion
    }
}
