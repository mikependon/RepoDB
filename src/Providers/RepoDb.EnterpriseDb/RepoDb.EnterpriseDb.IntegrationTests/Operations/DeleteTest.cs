#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Connector.EnterpriseDb;
using RepoDb.Enumerations;
using RepoDb.EnterpriseDb.IntegrationTests.Models;
using RepoDb.EnterpriseDb.IntegrationTests.Setup;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.EnterpriseDb.IntegrationTests.Operations
{
    [TestClass]
    public class DeleteTest
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
        public void TestEnterpriseDbConnectionDeleteWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Delete<CompleteTable>((object)null);

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod]
        public void TestEnterpriseDbConnectionDeleteViaPrimaryKey()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Delete<CompleteTable>(tables.First().Id);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestEnterpriseDbConnectionDeleteViaDataEntity()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Delete<CompleteTable>(tables.First());

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestEnterpriseDbConnectionDeleteViaExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Delete<CompleteTable>(e => e.Id == tables.First().Id);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestEnterpriseDbConnectionDeleteViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Delete<CompleteTable>(new { Id = tables.First().Id });

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestEnterpriseDbConnectionDeleteViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Delete<CompleteTable>(new QueryField("Id", tables.First().Id));

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestEnterpriseDbConnectionDeleteViaQueryFields()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Delete<CompleteTable>(queryFields);

                // Assert
                Assert.AreEqual(8, result);
            }
        }

        [TestMethod]
        public void TestEnterpriseDbConnectionDeleteViaQueryGroup()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Delete<CompleteTable>(queryGroup);

                // Assert
                Assert.AreEqual(8, result);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestEnterpriseDbConnectionDeleteAsyncWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.DeleteAsync<CompleteTable>((object)null);

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod]
        public async Task TestEnterpriseDbConnectionDeleteAsyncViaPrimaryKey()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.DeleteAsync<CompleteTable>(tables.First().Id);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public async Task TestEnterpriseDbConnectionDeleteAsyncViaDataEntity()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.DeleteAsync<CompleteTable>(tables.First());

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public async Task TestEnterpriseDbConnectionDeleteAsyncViaExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.DeleteAsync<CompleteTable>(e => e.Id == tables.First().Id);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public async Task TestEnterpriseDbConnectionDeleteAsyncViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.DeleteAsync<CompleteTable>(new { Id = tables.First().Id });

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public async Task TestEnterpriseDbConnectionDeleteAsyncViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.DeleteAsync<CompleteTable>(new QueryField("Id", tables.First().Id));

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public async Task TestEnterpriseDbConnectionDeleteAsyncViaQueryFields()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.DeleteAsync<CompleteTable>(queryFields);

                // Assert
                Assert.AreEqual(8, result);
            }
        }

        [TestMethod]
        public async Task TestEnterpriseDbConnectionDeleteAsyncViaQueryGroup()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.DeleteAsync<CompleteTable>(queryGroup);

                // Assert
                Assert.AreEqual(8, result);
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestEnterpriseDbConnectionDeleteViaTableNameWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Delete(ClassMappedNameCache.Get<CompleteTable>(), (object)null);

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod]
        public void TestEnterpriseDbConnectionDeleteViaTableNameViaPrimaryKey()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Delete<CompleteTable>(tables.First().Id);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestEnterpriseDbConnectionDeleteViaTableNameViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Delete(ClassMappedNameCache.Get<CompleteTable>(), new { Id = tables.First().Id });

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestEnterpriseDbConnectionDeleteViaTableNameViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Delete(ClassMappedNameCache.Get<CompleteTable>(), new QueryField("Id", tables.First().Id));

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestEnterpriseDbConnectionDeleteViaTableNameViaQueryFields()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Delete(ClassMappedNameCache.Get<CompleteTable>(), queryFields);

                // Assert
                Assert.AreEqual(8, result);
            }
        }

        [TestMethod]
        public void TestEnterpriseDbConnectionDeleteViaTableNameViaQueryGroup()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Delete(ClassMappedNameCache.Get<CompleteTable>(), queryGroup);

                // Assert
                Assert.AreEqual(8, result);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestEnterpriseDbConnectionDeleteAsyncViaTableNameWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.DeleteAsync(ClassMappedNameCache.Get<CompleteTable>(), (object)null);

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod]
        public async Task TestEnterpriseDbConnectionDeleteAsyncViaTableNameViaPrimaryKey()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.DeleteAsync<CompleteTable>(tables.First().Id);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public async Task TestEnterpriseDbConnectionDeleteAsyncViaTableNameViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.DeleteAsync(ClassMappedNameCache.Get<CompleteTable>(), new { Id = tables.First().Id });

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public async Task TestEnterpriseDbConnectionDeleteAsyncViaTableNameViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.DeleteAsync(ClassMappedNameCache.Get<CompleteTable>(), new QueryField("Id", tables.First().Id));

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public async Task TestEnterpriseDbConnectionDeleteAsyncViaTableNameViaQueryFields()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.DeleteAsync(ClassMappedNameCache.Get<CompleteTable>(), queryFields);

                // Assert
                Assert.AreEqual(8, result);
            }
        }

        [TestMethod]
        public async Task TestEnterpriseDbConnectionDeleteAsyncViaTableNameViaQueryGroup()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.DeleteAsync(ClassMappedNameCache.Get<CompleteTable>(), queryGroup);

                // Assert
                Assert.AreEqual(8, result);
            }
        }

        #endregion

        #endregion
    }
}
