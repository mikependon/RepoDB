using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.Enumerations;
using RepoDb.PostgreSql.IntegrationTests.Models;
using RepoDb.PostgreSql.IntegrationTests.Setup;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.PostgreSql.IntegrationTests.Operations
{
    [TestClass]
    public class MaxTest
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
        public void TestPostgreSqlConnectionMaxWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Max<CompleteTable>(e => e.ColumnInteger,
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Max(e => e.ColumnInteger), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestPostgreSqlConnectionMaxViaExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var ids = new[] { tables.First().Id, tables.Last().Id };

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Max<CompleteTable>(e => e.ColumnInteger,
                    e => ids.Contains(e.Id));

                // Assert
                Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Max(e => e.ColumnInteger), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestPostgreSqlConnectionMaxViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Max<CompleteTable>(e => e.ColumnInteger,
                    new { tables.First().Id });

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Max(e => e.ColumnInteger), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestPostgreSqlConnectionMaxViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Max<CompleteTable>(e => e.ColumnInteger,
                    new QueryField("Id", tables.First().Id));

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Max(e => e.ColumnInteger), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestPostgreSqlConnectionMaxViaQueryFields()
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
                var result = connection.Max<CompleteTable>(e => e.ColumnInteger,
                    queryFields);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Max(e => e.ColumnInteger), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestPostgreSqlConnectionMaxViaQueryGroup()
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
                var result = connection.Max<CompleteTable>(e => e.ColumnInteger,
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Max(e => e.ColumnInteger), Convert.ToInt32(result));
            }
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnPostgreSqlConnectionMaxWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Max<CompleteTable>(e => e.ColumnInteger,
                    (object)null,
                    hints: "WhatEver");
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestPostgreSqlConnectionMaxAsyncWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.MaxAsync<CompleteTable>(e => e.ColumnInteger,
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Max(e => e.ColumnInteger), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestPostgreSqlConnectionMaxAsyncViaExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var ids = new[] { tables.First().Id, tables.Last().Id };

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.MaxAsync<CompleteTable>(e => e.ColumnInteger,
                    e => ids.Contains(e.Id));

                // Assert
                Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Max(e => e.ColumnInteger), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestPostgreSqlConnectionMaxAsyncViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.MaxAsync<CompleteTable>(e => e.ColumnInteger,
                    new { tables.First().Id });

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Max(e => e.ColumnInteger), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestPostgreSqlConnectionMaxAsyncViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.MaxAsync<CompleteTable>(e => e.ColumnInteger,
                    new QueryField("Id", tables.First().Id));

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Max(e => e.ColumnInteger), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestPostgreSqlConnectionMaxAsyncViaQueryFields()
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
                var result = await connection.MaxAsync<CompleteTable>(e => e.ColumnInteger,
                    queryFields);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Max(e => e.ColumnInteger), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestPostgreSqlConnectionMaxAsyncViaQueryGroup()
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
                var result = await connection.MaxAsync<CompleteTable>(e => e.ColumnInteger,
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Max(e => e.ColumnInteger), Convert.ToInt32(result));
            }
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public async Task ThrowExceptionOnPostgreSqlConnectionMaxAsyncWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                await connection.MaxAsync<CompleteTable>(e => e.ColumnInteger,
                    (object)null,
                    hints: "WhatEver");
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestPostgreSqlConnectionMaxViaTableNameWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Max(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInteger", typeof(int)),
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Max(e => e.ColumnInteger), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestPostgreSqlConnectionMaxViaTableNameViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Max(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInteger", typeof(int)),
                    new { tables.First().Id });

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Max(e => e.ColumnInteger), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestPostgreSqlConnectionMaxViaTableNameViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Max(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInteger", typeof(int)),
                    new QueryField("Id", tables.First().Id));

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Max(e => e.ColumnInteger), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestPostgreSqlConnectionMaxViaTableNameViaQueryFields()
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
                var result = connection.Max(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInteger", typeof(int)),
                    queryFields);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Max(e => e.ColumnInteger), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestPostgreSqlConnectionMaxViaTableNameViaQueryGroup()
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
                var result = connection.Max(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInteger", typeof(int)),
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Max(e => e.ColumnInteger), Convert.ToInt32(result));
            }
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnPostgreSqlConnectionMaxViaTableNameWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Max(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInteger", typeof(int)),
                    (object)null,
                    hints: "WhatEver");
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestPostgreSqlConnectionMaxAsyncViaTableNameWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.MaxAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInteger", typeof(int)),
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Max(e => e.ColumnInteger), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestPostgreSqlConnectionMaxAsyncViaTableNameViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.MaxAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInteger", typeof(int)),
                    new { tables.First().Id });

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Max(e => e.ColumnInteger), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestPostgreSqlConnectionMaxAsyncViaTableNameViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.MaxAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInteger", typeof(int)),
                    new QueryField("Id", tables.First().Id));

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Max(e => e.ColumnInteger), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestPostgreSqlConnectionMaxAsyncViaTableNameViaQueryFields()
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
                var result = await connection.MaxAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInteger", typeof(int)),
                    queryFields);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Max(e => e.ColumnInteger), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestPostgreSqlConnectionMaxAsyncViaTableNameViaQueryGroup()
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
                var result = await connection.MaxAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInteger", typeof(int)),
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Max(e => e.ColumnInteger), Convert.ToInt32(result));
            }
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public async Task ThrowExceptionOnPostgreSqlConnectionMaxAsyncViaTableNameWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                await connection.MaxAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInteger", typeof(int)),
                    (object)null,
                    hints: "WhatEver");
            }
        }

        #endregion

        #endregion
    }
}
