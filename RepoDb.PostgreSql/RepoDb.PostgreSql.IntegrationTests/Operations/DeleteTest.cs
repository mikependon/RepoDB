using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.Enumerations;
using RepoDb.PostgreSql.IntegrationTests.Models;
using RepoDb.PostgreSql.IntegrationTests.Setup;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.PostgreSql.IntegrationTests.Operations
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
        public void TestPostgreSqlConnectionDeleteWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Delete<CompleteTable>((object)null);

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod]
        public void TestPostgreSqlConnectionDeleteViaPrimaryKey()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Delete<CompleteTable>(tables.First().Id);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestPostgreSqlConnectionDeleteViaDataEntity()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Delete<CompleteTable>(tables.First());

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestPostgreSqlConnectionDeleteViaExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Delete<CompleteTable>(e => e.Id == tables.First().Id);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestPostgreSqlConnectionDeleteViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Delete<CompleteTable>(new { Id = tables.First().Id });

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestPostgreSqlConnectionDeleteViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Delete<CompleteTable>(new QueryField("Id", tables.First().Id));

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestPostgreSqlConnectionDeleteViaQueryFields()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Delete<CompleteTable>(queryFields);

                // Assert
                Assert.AreEqual(8, result);
            }
        }

        [TestMethod]
        public void TestPostgreSqlConnectionDeleteViaQueryGroup()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public async Task TestPostgreSqlConnectionDeleteAsyncWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.DeleteAsync<CompleteTable>((object)null);

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod]
        public async Task TestPostgreSqlConnectionDeleteAsyncViaPrimaryKey()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.DeleteAsync<CompleteTable>(tables.First().Id);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public async Task TestPostgreSqlConnectionDeleteAsyncViaDataEntity()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.DeleteAsync<CompleteTable>(tables.First());

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public async Task TestPostgreSqlConnectionDeleteAsyncViaExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.DeleteAsync<CompleteTable>(e => e.Id == tables.First().Id);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public async Task TestPostgreSqlConnectionDeleteAsyncViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.DeleteAsync<CompleteTable>(new { Id = tables.First().Id });

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public async Task TestPostgreSqlConnectionDeleteAsyncViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.DeleteAsync<CompleteTable>(new QueryField("Id", tables.First().Id));

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public async Task TestPostgreSqlConnectionDeleteAsyncViaQueryFields()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.DeleteAsync<CompleteTable>(queryFields);

                // Assert
                Assert.AreEqual(8, result);
            }
        }

        [TestMethod]
        public async Task TestPostgreSqlConnectionDeleteAsyncViaQueryGroup()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public void TestPostgreSqlConnectionDeleteViaTableNameWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Delete(ClassMappedNameCache.Get<CompleteTable>(), (object)null);

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod]
        public void TestPostgreSqlConnectionDeleteViaTableNameViaPrimaryKey()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Delete<CompleteTable>(tables.First().Id);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestPostgreSqlConnectionDeleteViaTableNameViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Delete(ClassMappedNameCache.Get<CompleteTable>(), new { Id = tables.First().Id });

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestPostgreSqlConnectionDeleteViaTableNameViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Delete(ClassMappedNameCache.Get<CompleteTable>(), new QueryField("Id", tables.First().Id));

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestPostgreSqlConnectionDeleteViaTableNameViaQueryFields()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Delete(ClassMappedNameCache.Get<CompleteTable>(), queryFields);

                // Assert
                Assert.AreEqual(8, result);
            }
        }

        [TestMethod]
        public void TestPostgreSqlConnectionDeleteViaTableNameViaQueryGroup()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public async Task TestPostgreSqlConnectionDeleteAsyncViaTableNameWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.DeleteAsync(ClassMappedNameCache.Get<CompleteTable>(), (object)null);

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod]
        public async Task TestPostgreSqlConnectionDeleteAsyncViaTableNameViaPrimaryKey()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.DeleteAsync<CompleteTable>(tables.First().Id);

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public async Task TestPostgreSqlConnectionDeleteAsyncViaTableNameViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.DeleteAsync(ClassMappedNameCache.Get<CompleteTable>(), new { Id = tables.First().Id });

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public async Task TestPostgreSqlConnectionDeleteAsyncViaTableNameViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.DeleteAsync(ClassMappedNameCache.Get<CompleteTable>(), new QueryField("Id", tables.First().Id));

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public async Task TestPostgreSqlConnectionDeleteAsyncViaTableNameViaQueryFields()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.DeleteAsync(ClassMappedNameCache.Get<CompleteTable>(), queryFields);

                // Assert
                Assert.AreEqual(8, result);
            }
        }

        [TestMethod]
        public async Task TestPostgreSqlConnectionDeleteAsyncViaTableNameViaQueryGroup()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
