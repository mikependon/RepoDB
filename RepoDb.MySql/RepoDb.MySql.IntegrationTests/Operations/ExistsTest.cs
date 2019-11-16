using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using RepoDb.Enumerations;
using RepoDb.MySql.IntegrationTests.Models;
using RepoDb.MySql.IntegrationTests.Setup;
using System;
using System.Linq;

namespace RepoDb.MySql.IntegrationTests.Operations
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
        public void TestExistsWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Exists<CompleteTable>((object)null);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestExistsViaExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var ids = new[] { tables.First().Id, tables.Last().Id };

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Exists<CompleteTable>(e => ids.Contains(e.Id));

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestExistsViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Exists<CompleteTable>(new { tables.First().Id });

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestExistsViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Exists<CompleteTable>(new QueryField("Id", tables.First().Id));

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestExistsViaQueryFields()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Exists<CompleteTable>(queryFields);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestExistsViaQueryGroup()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Exists<CompleteTable>(queryGroup);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnExistsWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Exists<CompleteTable>((object)null,
                    hints: "WhatEver");
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestExistsAsyncWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExistsAsync<CompleteTable>((object)null).Result;

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestExistsAsyncViaExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var ids = new[] { tables.First().Id, tables.Last().Id };

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExistsAsync<CompleteTable>(e => ids.Contains(e.Id)).Result;

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestExistsAsyncViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExistsAsync<CompleteTable>(new { tables.First().Id }).Result;

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestExistsAsyncViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExistsAsync<CompleteTable>(new QueryField("Id", tables.First().Id)).Result;

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestExistsAsyncViaQueryFields()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExistsAsync<CompleteTable>(queryFields).Result;

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestExistsAsyncViaQueryGroup()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExistsAsync<CompleteTable>(queryGroup).Result;

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnExistsAsyncWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                connection.ExistsAsync<CompleteTable>((object)null,
                    hints: "WhatEver").Wait();
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestExistsViaTableNameWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Exists(ClassMappedNameCache.Get<CompleteTable>(),
                    (object)null);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestExistsViaTableNameViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Exists(ClassMappedNameCache.Get<CompleteTable>(),
                    new { tables.First().Id });

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestExistsViaTableNameViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Exists(ClassMappedNameCache.Get<CompleteTable>(),
                    new QueryField("Id", tables.First().Id));

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestExistsViaTableNameViaQueryFields()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Exists(ClassMappedNameCache.Get<CompleteTable>(),
                    queryFields);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestExistsViaTableNameViaQueryGroup()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Exists(ClassMappedNameCache.Get<CompleteTable>(),
                    queryGroup);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnExistsViaTableNameWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Exists(ClassMappedNameCache.Get<CompleteTable>(),
                    (object)null,
                    hints: "WhatEver");
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestExistsAsyncViaTableNameWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExistsAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    (object)null).Result;

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestExistsAsyncViaTableNameViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExistsAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    new { tables.First().Id }).Result;

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestExistsAsyncViaTableNameViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExistsAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    new QueryField("Id", tables.First().Id)).Result;

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestExistsAsyncViaTableNameViaQueryFields()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExistsAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    queryFields).Result;

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestExistsAsyncViaTableNameViaQueryGroup()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExistsAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    queryGroup).Result;

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnExistsAsyncViaTableNameWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                connection.ExistsAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    (object)null,
                    hints: "WhatEver").Wait();
            }
        }

        #endregion

        #endregion
    }
}
