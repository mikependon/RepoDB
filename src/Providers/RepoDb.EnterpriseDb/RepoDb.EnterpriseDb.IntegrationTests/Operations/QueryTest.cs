#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Connector.EnterpriseDb;
using RepoDb.Extensions;
using RepoDb.EnterpriseDb.IntegrationTests.Models;
using RepoDb.EnterpriseDb.IntegrationTests.Setup;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.EnterpriseDb.IntegrationTests.Operations
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
        public void TestEnterpriseDbConnectionQueryViaPrimaryKey()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Query<CompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestEnterpriseDbConnectionQueryViaExpression()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Query<CompleteTable>(e => e.Id == table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestEnterpriseDbConnectionQueryViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Query<CompleteTable>(new { table.Id }).First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestEnterpriseDbConnectionQueryViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Query<CompleteTable>(new QueryField("Id", table.Id)).First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestEnterpriseDbConnectionQueryViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInteger", table.ColumnInteger)
            };

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Query<CompleteTable>(queryFields).First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestEnterpriseDbConnectionQueryViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInteger", table.ColumnInteger)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Query<CompleteTable>(queryGroup).First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestEnterpriseDbConnectionQueryWithTop()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
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

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public async Task TestEnterpriseDbConnectionQueryAsyncViaPrimaryKey()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = (await connection.QueryAsync<CompleteTable>(table.Id)).First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestEnterpriseDbConnectionQueryAsyncViaExpression()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = (await connection.QueryAsync<CompleteTable>(e => e.Id == table.Id)).First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestEnterpriseDbConnectionQueryAsyncViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = (await connection.QueryAsync<CompleteTable>(new { table.Id })).First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestEnterpriseDbConnectionQueryAsyncViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = (await connection.QueryAsync<CompleteTable>(new QueryField("Id", table.Id))).First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestEnterpriseDbConnectionQueryAsyncViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInteger", table.ColumnInteger)
            };

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = (await connection.QueryAsync<CompleteTable>(queryFields)).First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestEnterpriseDbConnectionQueryAsyncViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInteger", table.ColumnInteger)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = (await connection.QueryAsync<CompleteTable>(queryGroup)).First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestEnterpriseDbConnectionQueryAsyncWithTop()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
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

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public void TestEnterpriseDbConnectionQueryViaTableNameViaPrimaryKey()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Query(ClassMappedNameCache.Get<CompleteTable>(), table.Id).First();

                // Assert
                Helper.AssertMembersEquality(table, result);
            }
        }

        [TestMethod]
        public void TestEnterpriseDbConnectionQueryViaTableNameViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Query(ClassMappedNameCache.Get<CompleteTable>(), new { table.Id }).First();

                // Assert
                Helper.AssertMembersEquality(table, result);
            }
        }

        [TestMethod]
        public void TestEnterpriseDbConnectionQueryViaTableNameViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Query(ClassMappedNameCache.Get<CompleteTable>(), new QueryField("Id", table.Id)).First();

                // Assert
                Helper.AssertMembersEquality(table, result);
            }
        }

        [TestMethod]
        public void TestEnterpriseDbConnectionQueryViaTableNameViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInteger", table.ColumnInteger)
            };

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Query(ClassMappedNameCache.Get<CompleteTable>(), queryFields).First();

                // Assert
                Helper.AssertMembersEquality(table, result);
            }
        }

        [TestMethod]
        public void TestEnterpriseDbConnectionQueryViaTableNameViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInteger", table.ColumnInteger)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Query(ClassMappedNameCache.Get<CompleteTable>(), queryGroup).First();

                // Assert
                Helper.AssertMembersEquality(table, result);
            }
        }

        [TestMethod]
        public void TestEnterpriseDbConnectionQueryViaTableNameWithTop()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
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

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public async Task TestEnterpriseDbConnectionQueryAsyncViaTableNameViaPrimaryKey()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = (await connection.QueryAsync(ClassMappedNameCache.Get<CompleteTable>(), table.Id)).First();

                // Assert
                Helper.AssertMembersEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestEnterpriseDbConnectionQueryAsyncViaTableNameViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = (await connection.QueryAsync(ClassMappedNameCache.Get<CompleteTable>(), new { table.Id })).First();

                // Assert
                Helper.AssertMembersEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestEnterpriseDbConnectionQueryAsyncViaTableNameViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = (await connection.QueryAsync(ClassMappedNameCache.Get<CompleteTable>(), new QueryField("Id", table.Id))).First();

                // Assert
                Helper.AssertMembersEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestEnterpriseDbConnectionQueryAsyncViaTableNameViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInteger", table.ColumnInteger)
            };

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = (await connection.QueryAsync(ClassMappedNameCache.Get<CompleteTable>(), queryFields)).First();

                // Assert
                Helper.AssertMembersEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestEnterpriseDbConnectionQueryAsyncViaTableNameViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInteger", table.ColumnInteger)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = (await connection.QueryAsync(ClassMappedNameCache.Get<CompleteTable>(), queryGroup)).First();

                // Assert
                Helper.AssertMembersEquality(table, result);
            }
        }

        [TestMethod]
        public async Task TestEnterpriseDbConnectionQueryAsyncViaTableNameWithTop()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
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

            using (var connection = new EDBConnection(Database.ConnectionString))
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
