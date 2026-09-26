#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Connector.CockroachDb;
using RepoDb.Extensions;
using RepoDb.CockroachDb.IntegrationTests.Models;
using RepoDb.CockroachDb.IntegrationTests.Setup;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.CockroachDb.IntegrationTests.Operations
{
    [TestClass]
    public class QueryTest
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
        public void TestCockroachDbConnectionQueryViaPrimaryKey()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Query<CompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestCockroachDbConnectionQueryViaExpression()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Query<CompleteTable>(e => e.Id == table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestCockroachDbConnectionQueryViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Query<CompleteTable>(new { table.Id }).First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestCockroachDbConnectionQueryViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Query<CompleteTable>(new QueryField("Id", table.Id)).First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestCockroachDbConnectionQueryViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInteger", table.ColumnInteger)
            };

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Query<CompleteTable>(queryFields).First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestCockroachDbConnectionQueryViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInteger", table.ColumnInteger)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Query<CompleteTable>(queryGroup).First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestCockroachDbConnectionQueryWithTop()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Query<CompleteTable>((object)null,
                    top: 2);

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
            }
        }

        [TestMethod]
        public void ThrowExceptionQueryWithHints()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<NotSupportedException>(() =>
                    connection.Query<CompleteTable>((object)null,
                        hints: "WhatEver"));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestCockroachDbConnectionQueryAsyncViaPrimaryKey()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = (await connection.QueryAsync<CompleteTable>(table.Id).ConfigureAwait(false)).First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestCockroachDbConnectionQueryAsyncViaExpression()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = (await connection.QueryAsync<CompleteTable>(e => e.Id == table.Id).ConfigureAwait(false)).First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestCockroachDbConnectionQueryAsyncViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = (await connection.QueryAsync<CompleteTable>(new { table.Id }).ConfigureAwait(false)).First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestCockroachDbConnectionQueryAsyncViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = (await connection.QueryAsync<CompleteTable>(new QueryField("Id", table.Id)).ConfigureAwait(false)).First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestCockroachDbConnectionQueryAsyncViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInteger", table.ColumnInteger)
            };

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = (await connection.QueryAsync<CompleteTable>(queryFields).ConfigureAwait(false)).First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestCockroachDbConnectionQueryAsyncViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInteger", table.ColumnInteger)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = (await connection.QueryAsync<CompleteTable>(queryGroup).ConfigureAwait(false)).First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestCockroachDbConnectionQueryAsyncWithTop()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QueryAsync<CompleteTable>((object)null,
                    top: 2).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionQueryAsyncWithHints()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<NotSupportedException>(async () =>
                    await connection.QueryAsync<CompleteTable>((object)null,
                        hints: "WhatEver").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestCockroachDbConnectionQueryViaTableNameViaPrimaryKey()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Query(ClassMappedNameCache.Get<CompleteTable>(), table.Id).First();

                // Assert
                Helper.AssertMembersEquality(table, result);
            }
        }

        [TestMethod]
        public void TestCockroachDbConnectionQueryViaTableNameViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Query(ClassMappedNameCache.Get<CompleteTable>(), new { table.Id }).First();

                // Assert
                Helper.AssertMembersEquality(table, result);
            }
        }

        [TestMethod]
        public void TestCockroachDbConnectionQueryViaTableNameViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Query(ClassMappedNameCache.Get<CompleteTable>(), new QueryField("Id", table.Id)).First();

                // Assert
                Helper.AssertMembersEquality(table, result);
            }
        }

        [TestMethod]
        public void TestCockroachDbConnectionQueryViaTableNameViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInteger", table.ColumnInteger)
            };

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Query(ClassMappedNameCache.Get<CompleteTable>(), queryFields).First();

                // Assert
                Helper.AssertMembersEquality(table, result);
            }
        }

        [TestMethod]
        public void TestCockroachDbConnectionQueryViaTableNameViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInteger", table.ColumnInteger)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Query(ClassMappedNameCache.Get<CompleteTable>(), queryGroup).First();

                // Assert
                Helper.AssertMembersEquality(table, result);
            }
        }

        [TestMethod]
        public void TestCockroachDbConnectionQueryViaTableNameWithTop()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Query(ClassMappedNameCache.Get<CompleteTable>(),
                    (object)null,
                    top: 2);

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
            }
        }

        [TestMethod]
        public void ThrowExceptionQueryViaTableNameWithHints()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<NotSupportedException>(() =>
                    connection.Query(ClassMappedNameCache.Get<CompleteTable>(),
                        (object)null,
                        hints: "WhatEver"));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestCockroachDbConnectionQueryAsyncViaTableNameViaPrimaryKey()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = (await connection.QueryAsync(ClassMappedNameCache.Get<CompleteTable>(), table.Id).ConfigureAwait(false)).First();

                // Assert
                Helper.AssertMembersEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestCockroachDbConnectionQueryAsyncViaTableNameViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = (await connection.QueryAsync(ClassMappedNameCache.Get<CompleteTable>(), new { table.Id }).ConfigureAwait(false)).First();

                // Assert
                Helper.AssertMembersEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestCockroachDbConnectionQueryAsyncViaTableNameViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = (await connection.QueryAsync(ClassMappedNameCache.Get<CompleteTable>(), new QueryField("Id", table.Id)).ConfigureAwait(false)).First();

                // Assert
                Helper.AssertMembersEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestCockroachDbConnectionQueryAsyncViaTableNameViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInteger", table.ColumnInteger)
            };

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = (await connection.QueryAsync(ClassMappedNameCache.Get<CompleteTable>(), queryFields).ConfigureAwait(false)).First();

                // Assert
                Helper.AssertMembersEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestCockroachDbConnectionQueryAsyncViaTableNameViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInteger", table.ColumnInteger)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = (await connection.QueryAsync(ClassMappedNameCache.Get<CompleteTable>(), queryGroup).ConfigureAwait(false)).First();

                // Assert
                Helper.AssertMembersEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestCockroachDbConnectionQueryAsyncViaTableNameWithTop()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QueryAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    (object)null,
                    top: 2).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionQueryAsyncViaTableNameWithHints()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<NotSupportedException>(async () =>
                    await connection.QueryAsync(ClassMappedNameCache.Get<CompleteTable>(),
                        (object)null,
                        hints: "WhatEver").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        #endregion

        #endregion
    }
}
