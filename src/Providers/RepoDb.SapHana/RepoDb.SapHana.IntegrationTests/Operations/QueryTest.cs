#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sap.Data.Hana;
using RepoDb.Extensions;
using RepoDb.SapHana.IntegrationTests.Models;
using RepoDb.SapHana.IntegrationTests.Setup;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.SapHana.IntegrationTests.Operations
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
        public void TestHanaConnectionQueryViaPrimaryKey()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Query<CompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestHanaConnectionQueryViaExpression()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Query<CompleteTable>(e => e.Id == table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestHanaConnectionQueryViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Query<CompleteTable>(new { table.Id }).First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestHanaConnectionQueryViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Query<CompleteTable>(new QueryField("Id", table.Id)).First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestHanaConnectionQueryViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Query<CompleteTable>(queryFields).First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestHanaConnectionQueryViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Query<CompleteTable>(queryGroup).First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestHanaConnectionQueryWithTop()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
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

            using (var connection = new HanaConnection(Database.ConnectionString))
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
        public async Task TestHanaConnectionQueryAsyncViaPrimaryKey()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = (await connection.QueryAsync<CompleteTable>(table.Id)).First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestHanaConnectionQueryAsyncViaExpression()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = (await connection.QueryAsync<CompleteTable>(e => e.Id == table.Id)).First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestHanaConnectionQueryAsyncViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = (await connection.QueryAsync<CompleteTable>(new { table.Id })).First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestHanaConnectionQueryAsyncViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = (await connection.QueryAsync<CompleteTable>(new QueryField("Id", table.Id))).First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestHanaConnectionQueryAsyncViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = (await connection.QueryAsync<CompleteTable>(queryFields)).First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestHanaConnectionQueryAsyncViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = (await connection.QueryAsync<CompleteTable>(queryGroup)).First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestHanaConnectionQueryAsyncWithTop()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QueryAsync<CompleteTable>((object)null,
                    top: 2);

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

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<NotSupportedException>(async () =>
                    await connection.QueryAsync<CompleteTable>((object)null,
                        hints: "WhatEver"));
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestHanaConnectionQueryViaTableNameViaPrimaryKey()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Query(ClassMappedNameCache.Get<CompleteTable>(), table.Id).First();

                // Assert
                Helper.AssertMembersEquality(table, result);
            }
        }

        [TestMethod]
        public void TestHanaConnectionQueryViaTableNameViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Query(ClassMappedNameCache.Get<CompleteTable>(), new { table.Id }).First();

                // Assert
                Helper.AssertMembersEquality(table, result);
            }
        }

        [TestMethod]
        public void TestHanaConnectionQueryViaTableNameViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Query(ClassMappedNameCache.Get<CompleteTable>(), new QueryField("Id", table.Id)).First();

                // Assert
                Helper.AssertMembersEquality(table, result);
            }
        }

        [TestMethod]
        public void TestHanaConnectionQueryViaTableNameViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Query(ClassMappedNameCache.Get<CompleteTable>(), queryFields).First();

                // Assert
                Helper.AssertMembersEquality(table, result);
            }
        }

        [TestMethod]
        public void TestHanaConnectionQueryViaTableNameViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Query(ClassMappedNameCache.Get<CompleteTable>(), queryGroup).First();

                // Assert
                Helper.AssertMembersEquality(table, result);
            }
        }

        [TestMethod]
        public void TestHanaConnectionQueryViaTableNameWithTop()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
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

            using (var connection = new HanaConnection(Database.ConnectionString))
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
        public async Task TestHanaConnectionQueryAsyncViaTableNameViaPrimaryKey()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = (await connection.QueryAsync(ClassMappedNameCache.Get<CompleteTable>(), table.Id)).First();

                // Assert
                Helper.AssertMembersEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestHanaConnectionQueryAsyncViaTableNameViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = (await connection.QueryAsync(ClassMappedNameCache.Get<CompleteTable>(), new { table.Id })).First();

                // Assert
                Helper.AssertMembersEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestHanaConnectionQueryAsyncViaTableNameViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = (await connection.QueryAsync(ClassMappedNameCache.Get<CompleteTable>(), new QueryField("Id", table.Id))).First();

                // Assert
                Helper.AssertMembersEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestHanaConnectionQueryAsyncViaTableNameViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = (await connection.QueryAsync(ClassMappedNameCache.Get<CompleteTable>(), queryFields)).First();

                // Assert
                Helper.AssertMembersEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestHanaConnectionQueryAsyncViaTableNameViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = (await connection.QueryAsync(ClassMappedNameCache.Get<CompleteTable>(), queryGroup)).First();

                // Assert
                Helper.AssertMembersEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestHanaConnectionQueryAsyncViaTableNameWithTop()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QueryAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    (object)null,
                    top: 2);

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

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<NotSupportedException>(async () =>
                    await connection.QueryAsync(ClassMappedNameCache.Get<CompleteTable>(),
                        (object)null,
                        hints: "WhatEver"));
            }
        }

        #endregion

        #endregion
    }
}
