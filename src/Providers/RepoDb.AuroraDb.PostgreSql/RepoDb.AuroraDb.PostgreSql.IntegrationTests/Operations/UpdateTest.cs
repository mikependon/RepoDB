#region Copyright Attributions

// Copyright (c) 2019 Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Connector.AuroraDb.Npgsql;
using RepoDb.AuroraDb.PostgreSql.IntegrationTests.Models;
using RepoDb.AuroraDb.PostgreSql.IntegrationTests.Setup;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.AuroraDb.PostgreSql.IntegrationTests.Operations
{
    [TestClass]
    public class UpdateTest
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
        public void TestAuroraDbConnectionUpdateViaDataEntity()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.Update<CompleteTable>(table);

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestAuroraDbConnectionUpdateViaExpression()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.Update<CompleteTable>(table, e => e.Id == table.Id);

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestAuroraDbConnectionUpdateViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.Update<CompleteTable>(table, new { table.Id });

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestAuroraDbConnectionUpdateViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.Update<CompleteTable>(table, new QueryField("Id", table.Id));

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestAuroraDbConnectionUpdateViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInteger", table.ColumnInteger)
            };

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.Update<CompleteTable>(table, queryFields);

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestAuroraDbConnectionUpdateViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInteger", table.ColumnInteger)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.Update<CompleteTable>(table, queryGroup);

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestAuroraDbConnectionUpdateAsyncViaDataEntity()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = await connection.UpdateAsync<CompleteTable>(table).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public async Task TestAuroraDbConnectionUpdateAsyncViaExpression()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = await connection.UpdateAsync<CompleteTable>(table, e => e.Id == table.Id).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public async Task TestAuroraDbConnectionUpdateAsyncViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = await connection.UpdateAsync<CompleteTable>(table, new { table.Id }).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public async Task TestAuroraDbConnectionUpdateAsyncViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = await connection.UpdateAsync<CompleteTable>(table, new QueryField("Id", table.Id)).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public async Task TestAuroraDbConnectionUpdateAsyncViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInteger", table.ColumnInteger)
            };

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = await connection.UpdateAsync<CompleteTable>(table, queryFields).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public async Task TestAuroraDbConnectionUpdateAsyncViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInteger", table.ColumnInteger)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = await connection.UpdateAsync<CompleteTable>(table, queryGroup).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestAuroraDbConnectionUpdateViaTableNameViaExpandoObject()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Setup
                var entity = Helper.CreateCompleteTablesAsExpandoObjects(1).First();
                ((IDictionary<string, object>)entity)["Id"] = table.Id;

                // Act
                var result = connection.Update(ClassMappedNameCache.Get<CompleteTable>(),
                    entity);

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(queryResult, entity);
            }
        }

        [TestMethod]
        public void TestAuroraDbConnectionUpdateViaTableNameViaDataEntity()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.Update(ClassMappedNameCache.Get<CompleteTable>(),
                    table);

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestAuroraDbConnectionUpdateViaTableNameViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.Update(ClassMappedNameCache.Get<CompleteTable>(),
                    table,
                    new { table.Id });

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestAuroraDbConnectionUpdateViaTableNameViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.Update(ClassMappedNameCache.Get<CompleteTable>(),
                    table,
                    new QueryField("Id", table.Id));

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestAuroraDbConnectionUpdateViaTableNameViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInteger", table.ColumnInteger)
            };

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.Update(ClassMappedNameCache.Get<CompleteTable>(),
                    table,
                    queryFields);

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestAuroraDbConnectionUpdateViaTableNameViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInteger", table.ColumnInteger)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.Update(ClassMappedNameCache.Get<CompleteTable>(),
                    table,
                    queryGroup);

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestAuroraDbConnectionUpdateAsyncViaTableNameViaExpandoObject()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Setup
                var entity = Helper.CreateCompleteTablesAsExpandoObjects(1).First();
                ((IDictionary<string, object>)entity)["Id"] = table.Id;

                // Act
                var result = await connection.UpdateAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    entity).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(queryResult, entity);
            }
        }

        [TestMethod]
        public async Task TestAuroraDbConnectionUpdateAsyncViaTableNameViaDataEntity()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = await connection.UpdateAsync(ClassMappedNameCache.Get<CompleteTable>(), table).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public async Task TestAuroraDbConnectionUpdateAsyncViaTableNameViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = await connection.UpdateAsync(ClassMappedNameCache.Get<CompleteTable>(), table, new { table.Id }).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public async Task TestAuroraDbConnectionUpdateAsyncViaTableNameViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = await connection.UpdateAsync(ClassMappedNameCache.Get<CompleteTable>(), table, new QueryField("Id", table.Id)).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public async Task TestAuroraDbConnectionUpdateAsyncViaTableNameViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInteger", table.ColumnInteger)
            };

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = await connection.UpdateAsync(ClassMappedNameCache.Get<CompleteTable>(), table, queryFields).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public async Task TestAuroraDbConnectionUpdateAsyncViaTableNameViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInteger", table.ColumnInteger)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = await connection.UpdateAsync(ClassMappedNameCache.Get<CompleteTable>(), table, queryGroup).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        #endregion

        #endregion
    }
}
