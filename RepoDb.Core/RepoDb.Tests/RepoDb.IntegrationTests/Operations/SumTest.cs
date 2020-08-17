using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System;
using System.Linq;

namespace RepoDb.IntegrationTests.Operations
{
    [TestClass]
    public class SumTest
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

        #region Sum<TEntity>

        [TestMethod]
        public void TestSqlConnectionSumWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Sum<IdentityTable>(e => e.ColumnInt,
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Sum<IdentityTable>(e => e.ColumnInt,
                    item => item.ColumnInt > 5 && item.ColumnInt <= 8);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Sum<IdentityTable>(e => e.ColumnInt,
                    new { ColumnInt = 1 });

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt == 1).Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Sum<IdentityTable>(e => e.ColumnInt,
                    field);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5).Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Sum<IdentityTable>(e => e.ColumnInt,
                    fields);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Sum<IdentityTable>(e => e.ColumnInt,
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumTypedResultWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Sum<IdentityTable, int?>(e => e.ColumnInt,
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumTypedResultViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Sum<IdentityTable, int?>(e => e.ColumnInt,
                    item => item.ColumnInt > 5 && item.ColumnInt <= 8);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Sum(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumTypedResultViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Sum<IdentityTable, int?>(e => e.ColumnInt,
                    new { ColumnInt = 1 });

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt == 1).Sum(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumTypedResultViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Sum<IdentityTable, int?>(e => e.ColumnInt,
                    field);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5).Sum(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumTypedResultViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Sum<IdentityTable, int?>(e => e.ColumnInt,
                    fields);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Sum(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumTypedResultViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Sum<IdentityTable, int?>(e => e.ColumnInt,
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Sum(t => t.ColumnInt), result);
            }
        }

        #endregion

        #region SumAsync<TEntity>

        [TestMethod]
        public void TestSqlConnectionSumAsyncWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.SumAsync<IdentityTable>(e => e.ColumnInt,
                    (object)null).Result;

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumAsyncViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.SumAsync<IdentityTable>(e => e.ColumnInt,
                    item => item.ColumnInt > 5 && item.ColumnInt <= 8).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumAsyncViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.SumAsync<IdentityTable>(e => e.ColumnInt,
                    new { ColumnInt = 1 }).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt == 1).Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumAsyncViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.SumAsync<IdentityTable>(e => e.ColumnInt,
                    field).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5).Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumAsyncViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.SumAsync<IdentityTable>(e => e.ColumnInt,
                    fields).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumAsyncViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.SumAsync<IdentityTable>(e => e.ColumnInt,
                    queryGroup).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumAsyncTypedResultWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.SumAsync<IdentityTable, int?>(e => e.ColumnInt,
                    (object)null).Result;

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumAsyncTypedResultViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.SumAsync<IdentityTable, int?>(e => e.ColumnInt,
                    item => item.ColumnInt > 5 && item.ColumnInt <= 8).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Sum(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumAsyncTypedResultViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.SumAsync<IdentityTable, int?>(e => e.ColumnInt,
                    new { ColumnInt = 1 }).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt == 1).Sum(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumAsyncTypedResultViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.SumAsync<IdentityTable, int?>(e => e.ColumnInt,
                    field).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5).Sum(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumAsyncTypedResultViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.SumAsync<IdentityTable, int?>(e => e.ColumnInt,
                    fields).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Sum(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumAsyncTypedResultViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.SumAsync<IdentityTable, int?>(e => e.ColumnInt,
                    queryGroup).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Sum(t => t.ColumnInt), result);
            }
        }

        #endregion

        #region Sum(TableName)

        [TestMethod]
        public void TestSqlConnectionSumViaTableNameWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Sum(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumViaTableNameViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Sum(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    new { ColumnInt = 1 });

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt == 1).Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumViaTableNameViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Sum(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    field);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5).Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumViaTableNameViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Sum(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    fields);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumViaTableNameViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Sum(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumViaTableNameTypedResultWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Sum<int?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumViaTableNameTypedResultViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Sum<int?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    new { ColumnInt = 1 });

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt == 1).Sum(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumViaTableNameTypedResultViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Sum<int?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    field);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5).Sum(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumViaTableNameTypedResultViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Sum<int?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    fields);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Sum(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumViaTableNameTypedResultViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Sum<int?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Sum(t => t.ColumnInt), result);
            }
        }

        #endregion

        #region SumAsync(TableName)

        [TestMethod]
        public void TestSqlConnectionSumAsyncViaTableNameWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.SumAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    (object)null).Result;

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumAsyncViaTableNameViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.SumAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    new { ColumnInt = 1 }).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt == 1).Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumAsyncViaTableNameViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.SumAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    field).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5).Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumAsyncViaTableNameViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.SumAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    fields).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumAsyncViaTableNameViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.SumAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    queryGroup).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumAsyncTypedResultViaTableNameWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.SumAsync<int?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    (object)null).Result;

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumAsyncTypedResultViaTableNameViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.SumAsync<int?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    new { ColumnInt = 1 }).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt == 1).Sum(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumAsyncTypedResultViaTableNameViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.SumAsync<int?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    field).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5).Sum(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumAsyncTypedResultViaTableNameViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.SumAsync<int?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    fields).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Sum(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumAsyncTypedResultViaTableNameViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.SumAsync<int?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    queryGroup).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Sum(t => t.ColumnInt), result);
            }
        }

        #endregion
    }
}
