using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Data.SqlClient;
using RepoDb.Enumerations;
using RepoDb.SqlServer.IntegrationTests.Models;
using RepoDb.SqlServer.IntegrationTests.Setup;
using System;
using System.Linq;

namespace RepoDb.SqlServer.IntegrationTests.Operations
{
    [TestClass]
    public class ExistsTest
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
        public void TestSqlServerConnectionExistsWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Exists<CompleteTable>((object)null);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionExistsViaExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var ids = new[] { tables.First().Id, tables.Last().Id };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Exists<CompleteTable>(e => ids.Contains(e.Id));

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionExistsViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Exists<CompleteTable>(new { tables.First().Id });

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionExistsViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Exists<CompleteTable>(new QueryField("Id", tables.First().Id));

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionExistsViaQueryFields()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Exists<CompleteTable>(queryFields);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionExistsViaQueryGroup()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Exists<CompleteTable>(queryGroup);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionExistsWithoutExpressionWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Exists<CompleteTable>((object)null,
                    SqlServerTableHints.NoLock);

                // Assert
                Assert.IsTrue(result);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestSqlServerConnectionExistsAsyncWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExistsAsync<CompleteTable>((object)null).Result;

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionExistsAsyncViaExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var ids = new[] { tables.First().Id, tables.Last().Id };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExistsAsync<CompleteTable>(e => ids.Contains(e.Id)).Result;

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionExistsAsyncViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExistsAsync<CompleteTable>(new { tables.First().Id }).Result;

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionExistsAsyncViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExistsAsync<CompleteTable>(new QueryField("Id", tables.First().Id)).Result;

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionExistsAsyncViaQueryFields()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExistsAsync<CompleteTable>(queryFields).Result;

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionExistsAsyncViaQueryGroup()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExistsAsync<CompleteTable>(queryGroup).Result;

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionExistsAsyncWithoutExpressionWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExistsAsync<CompleteTable>((object)null,
                    SqlServerTableHints.NoLock).Result;

                // Assert
                Assert.IsTrue(result);
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestSqlServerConnectionExistsViaTableNameWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Exists(ClassMappedNameCache.Get<CompleteTable>(),
                    (object)null);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionExistsViaTableNameViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Exists(ClassMappedNameCache.Get<CompleteTable>(),
                    new { tables.First().Id });

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionExistsViaTableNameViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Exists(ClassMappedNameCache.Get<CompleteTable>(),
                    new QueryField("Id", tables.First().Id));

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionExistsViaTableNameViaQueryFields()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Exists(ClassMappedNameCache.Get<CompleteTable>(),
                    queryFields);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionExistsViaTableNameViaQueryGroup()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Exists(ClassMappedNameCache.Get<CompleteTable>(),
                    queryGroup);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionExistsViaTableNameWithoutExpressionWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Exists(ClassMappedNameCache.Get<CompleteTable>(),
                    (object)null,
                    SqlServerTableHints.NoLock);

                // Assert
                Assert.IsTrue(result);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestSqlServerConnectionExistsAsyncViaTableNameWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExistsAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    (object)null).Result;

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionExistsAsyncViaTableNameViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExistsAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    new { tables.First().Id }).Result;

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionExistsAsyncViaTableNameViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExistsAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    new QueryField("Id", tables.First().Id)).Result;

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionExistsAsyncViaTableNameViaQueryFields()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExistsAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    queryFields).Result;

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionExistsAsyncViaTableNameViaQueryGroup()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExistsAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    queryGroup).Result;

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionExistsAsyncViaTableNameWithoutExpressionWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExistsAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    (object)null,
                    SqlServerTableHints.NoLock).Result;

                // Assert
                Assert.IsTrue(result);
            }
        }

        #endregion

        #endregion
    }
}
