using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System;
using System.Linq;
using System.Threading.Tasks;

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

            using (var connection = new SqlConnection(Database.ConnectionString))
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

            using (var connection = new SqlConnection(Database.ConnectionString))
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

            using (var connection = new SqlConnection(Database.ConnectionString))
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

            using (var connection = new SqlConnection(Database.ConnectionString))
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

            using (var connection = new SqlConnection(Database.ConnectionString))
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

            using (var connection = new SqlConnection(Database.ConnectionString))
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

            using (var connection = new SqlConnection(Database.ConnectionString))
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

            using (var connection = new SqlConnection(Database.ConnectionString))
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

            using (var connection = new SqlConnection(Database.ConnectionString))
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

            using (var connection = new SqlConnection(Database.ConnectionString))
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

            using (var connection = new SqlConnection(Database.ConnectionString))
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

            using (var connection = new SqlConnection(Database.ConnectionString))
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

        [TestMethod]
        public void TestSqlConnectionSumTypedResultDecimalWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Sum<IdentityTable, decimal?>(e => e.ColumnDecimal,
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnDecimal), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumTypedResultDecimalViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Sum<IdentityTable, decimal?>(e => e.ColumnDecimal,
                    item => item.ColumnDecimal > 5m && item.ColumnDecimal <= 8m);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnDecimal > 5m && t.ColumnDecimal <= 8m).Sum(t => t.ColumnDecimal), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumTypedResultDecimalViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Sum<IdentityTable, decimal?>(e => e.ColumnDecimal,
                    new { ColumnDecimal = 1m });

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnDecimal == 1m).Sum(t => t.ColumnDecimal), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumTypedResultDecimalViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.GreaterThan, 5m);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Sum<IdentityTable, decimal?>(e => e.ColumnDecimal,
                    field);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnDecimal > 5m).Sum(t => t.ColumnDecimal), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumTypedResultDecimalViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.GreaterThan, 5m),
                new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.LessThanOrEqual, 8m)
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Sum<IdentityTable, decimal?>(e => e.ColumnDecimal,
                    fields);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnDecimal > 5m && t.ColumnDecimal <= 8m).Sum(t => t.ColumnDecimal), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumTypedResultDecimalViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.GreaterThan, 5m),
                new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.LessThanOrEqual, 8m)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Sum<IdentityTable, decimal?>(e => e.ColumnDecimal,
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnDecimal > 5m && t.ColumnDecimal <= 8m).Sum(t => t.ColumnDecimal), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumTypedResultDoubleWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Sum<IdentityTable, double?>(e => e.ColumnFloat,
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumTypedResultDoubleViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Sum<IdentityTable, double?>(e => e.ColumnFloat,
                    item => item.ColumnFloat > 5d && item.ColumnFloat <= 8d);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnFloat > 5d && t.ColumnFloat <= 8d).Sum(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumTypedResultDoubleViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Sum<IdentityTable, double?>(e => e.ColumnFloat,
                    new { ColumnFloat = 1d });

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnFloat == 1d).Sum(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumTypedResultDoubleViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnFloat), Operation.GreaterThan, 5d);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Sum<IdentityTable, double?>(e => e.ColumnFloat,
                    field);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnFloat > 5d).Sum(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumTypedResultDoubleViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnFloat), Operation.GreaterThan, 5d),
                new QueryField(nameof(IdentityTable.ColumnFloat), Operation.LessThanOrEqual, 8d)
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Sum<IdentityTable, double?>(e => e.ColumnFloat,
                    fields);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnFloat > 5d && t.ColumnFloat <= 8d).Sum(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumTypedResultDoubleViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnFloat), Operation.GreaterThan, 5d),
                new QueryField(nameof(IdentityTable.ColumnFloat), Operation.LessThanOrEqual, 8d)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Sum<IdentityTable, double?>(e => e.ColumnFloat,
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnFloat > 5d && t.ColumnFloat <= 8d).Sum(t => t.ColumnFloat), result);
            }
        }

        #endregion

        #region SumAsync<TEntity>

        [TestMethod]
        public async Task TestSqlConnectionSumAsyncWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.SumAsync<IdentityTable>(e => e.ColumnInt,
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionSumAsyncViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.SumAsync<IdentityTable>(e => e.ColumnInt,
                    item => item.ColumnInt > 5 && item.ColumnInt <= 8);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionSumAsyncViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.SumAsync<IdentityTable>(e => e.ColumnInt,
                    new { ColumnInt = 1 });

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt == 1).Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionSumAsyncViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.SumAsync<IdentityTable>(e => e.ColumnInt,
                    field);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5).Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionSumAsyncViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.SumAsync<IdentityTable>(e => e.ColumnInt,
                    fields);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionSumAsyncViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.SumAsync<IdentityTable>(e => e.ColumnInt,
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionSumAsyncTypedResultWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.SumAsync<IdentityTable, int?>(e => e.ColumnInt,
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionSumAsyncTypedResultViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.SumAsync<IdentityTable, int?>(e => e.ColumnInt,
                    item => item.ColumnInt > 5 && item.ColumnInt <= 8);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Sum(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionSumAsyncTypedResultViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.SumAsync<IdentityTable, int?>(e => e.ColumnInt,
                    new { ColumnInt = 1 });

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt == 1).Sum(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionSumAsyncTypedResultViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.SumAsync<IdentityTable, int?>(e => e.ColumnInt,
                    field);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5).Sum(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionSumAsyncTypedResultViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.SumAsync<IdentityTable, int?>(e => e.ColumnInt,
                    fields);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Sum(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionSumAsyncTypedResultViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.SumAsync<IdentityTable, int?>(e => e.ColumnInt,
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Sum(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionSumAsyncTypedResultDecimalWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.SumAsync<IdentityTable, decimal?>(e => e.ColumnDecimal,
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnDecimal), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionSumAsyncTypedResultDecimalViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.SumAsync<IdentityTable, decimal?>(e => e.ColumnDecimal,
                    item => item.ColumnDecimal > 5m && item.ColumnDecimal <= 8m);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnDecimal > 5m && t.ColumnDecimal <= 8m).Sum(t => t.ColumnDecimal), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionSumAsyncTypedResultDecimalViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.SumAsync<IdentityTable, decimal?>(e => e.ColumnDecimal,
                    new { ColumnDecimal = 1m });

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnDecimal == 1m).Sum(t => t.ColumnDecimal), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionSumAsyncTypedResultDecimalViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.GreaterThan, 5m);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.SumAsync<IdentityTable, decimal?>(e => e.ColumnDecimal,
                    field);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnDecimal > 5m).Sum(t => t.ColumnDecimal), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionSumAsyncTypedResultDecimalViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.GreaterThan, 5m),
                new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.LessThanOrEqual, 8m)
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.SumAsync<IdentityTable, decimal?>(e => e.ColumnDecimal,
                    fields);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnDecimal > 5m && t.ColumnDecimal <= 8m).Sum(t => t.ColumnDecimal), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionSumAsyncTypedResultDecimalViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.GreaterThan, 5m),
                new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.LessThanOrEqual, 8m)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.SumAsync<IdentityTable, decimal?>(e => e.ColumnDecimal,
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnDecimal > 5m && t.ColumnDecimal <= 8m).Sum(t => t.ColumnDecimal), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionSumAsyncTypedResultDoubleWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.SumAsync<IdentityTable, double?>(e => e.ColumnFloat,
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionSumAsyncTypedResultDoubleViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.SumAsync<IdentityTable, double?>(e => e.ColumnFloat,
                    item => item.ColumnFloat > 5d && item.ColumnFloat <= 8d);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnFloat > 5d && t.ColumnFloat <= 8d).Sum(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionSumAsyncTypedResultDoubleViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.SumAsync<IdentityTable, double?>(e => e.ColumnFloat,
                    new { ColumnFloat = 1d });

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnFloat == 1d).Sum(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionSumAsyncTypedResultDoubleViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnFloat), Operation.GreaterThan, 5d);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.SumAsync<IdentityTable, double?>(e => e.ColumnFloat,
                    field);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnFloat > 5d).Sum(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionSumAsyncTypedResultDoubleViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnFloat), Operation.GreaterThan, 5d),
                new QueryField(nameof(IdentityTable.ColumnFloat), Operation.LessThanOrEqual, 8d)
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.SumAsync<IdentityTable, double?>(e => e.ColumnFloat,
                    fields);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnFloat > 5d && t.ColumnFloat <= 8d).Sum(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionSumAsyncTypedResultDoubleViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnFloat), Operation.GreaterThan, 5d),
                new QueryField(nameof(IdentityTable.ColumnFloat), Operation.LessThanOrEqual, 8d)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.SumAsync<IdentityTable, double?>(e => e.ColumnFloat,
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnFloat > 5d && t.ColumnFloat <= 8d).Sum(t => t.ColumnFloat), result);
            }
        }

        #endregion

        #region Sum(TableName)

        [TestMethod]
        public void TestSqlConnectionSumViaTableNameWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
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

            using (var connection = new SqlConnection(Database.ConnectionString))
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

            using (var connection = new SqlConnection(Database.ConnectionString))
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

            using (var connection = new SqlConnection(Database.ConnectionString))
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

            using (var connection = new SqlConnection(Database.ConnectionString))
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

            using (var connection = new SqlConnection(Database.ConnectionString))
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

            using (var connection = new SqlConnection(Database.ConnectionString))
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

            using (var connection = new SqlConnection(Database.ConnectionString))
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

            using (var connection = new SqlConnection(Database.ConnectionString))
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

            using (var connection = new SqlConnection(Database.ConnectionString))
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

        [TestMethod]
        public void TestSqlConnectionSumViaTableNameTypedResultDecimalWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Sum<decimal?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnDecimal"),
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnDecimal), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumViaTableNameTypedResultDecimalViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Sum<decimal?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnDecimal"),
                    new { ColumnDecimal = 1m });

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnDecimal == 1m).Sum(t => t.ColumnDecimal), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumViaTableNameTypedResultDecimalViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.GreaterThan, 5m);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Sum<decimal?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnDecimal"),
                    field);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnDecimal > 5m).Sum(t => t.ColumnDecimal), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumViaTableNameTypedResultDecimalViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.GreaterThan, 5m),
                new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.LessThanOrEqual, 8m)
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Sum<decimal?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnDecimal"),
                    fields);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnDecimal > 5m && t.ColumnDecimal <= 8m).Sum(t => t.ColumnDecimal), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumViaTableNameTypedResultDecimalViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.GreaterThan, 5m),
                new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.LessThanOrEqual, 8m)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Sum<decimal?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnDecimal"),
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnDecimal > 5m && t.ColumnDecimal <= 8m).Sum(t => t.ColumnDecimal), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumViaTableNameTypedResultDoubleWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Sum<double?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnFloat"),
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumViaTableNameTypedResultDoubleViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Sum<double?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnFloat"),
                    new { ColumnFloat = 1d });

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnFloat == 1d).Sum(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumViaTableNameTypedResultDoubleViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnFloat), Operation.GreaterThan, 5d);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Sum<double?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnFloat"),
                    field);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnFloat > 5d).Sum(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumViaTableNameTypedResultDoubleViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnFloat), Operation.GreaterThan, 5d),
                new QueryField(nameof(IdentityTable.ColumnFloat), Operation.LessThanOrEqual, 8d)
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Sum<double?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnFloat"),
                    fields);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnFloat > 5d && t.ColumnFloat <= 8d).Sum(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionSumViaTableNameTypedResultDoubleViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnFloat), Operation.GreaterThan, 5d),
                new QueryField(nameof(IdentityTable.ColumnFloat), Operation.LessThanOrEqual, 8d)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Sum<double?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnFloat"),
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnFloat > 5d && t.ColumnFloat <= 8d).Sum(t => t.ColumnFloat), result);
            }
        }

        #endregion

        #region SumAsync(TableName)

        [TestMethod]
        public async Task TestSqlConnectionSumAsyncViaTableNameWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.SumAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionSumAsyncViaTableNameViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.SumAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    new { ColumnInt = 1 });

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt == 1).Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionSumAsyncViaTableNameViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.SumAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    field);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5).Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionSumAsyncViaTableNameViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.SumAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    fields);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionSumAsyncViaTableNameViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.SumAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Sum(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionSumAsyncTypedResultViaTableNameWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.SumAsync<int?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionSumAsyncTypedResultViaTableNameViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.SumAsync<int?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    new { ColumnInt = 1 });

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt == 1).Sum(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionSumAsyncTypedResultViaTableNameViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.SumAsync<int?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    field);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5).Sum(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionSumAsyncTypedResultViaTableNameViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.SumAsync<int?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    fields);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Sum(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionSumAsyncTypedResultViaTableNameViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.SumAsync<int?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Sum(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionSumAsyncTypedResultDecimalViaTableNameWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.SumAsync<decimal?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnDecimal"),
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnDecimal), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionSumAsyncTypedResultDecimalViaTableNameViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.SumAsync<decimal?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnDecimal"),
                    new { ColumnDecimal = 1m });

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnDecimal == 1m).Sum(t => t.ColumnDecimal), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionSumAsyncTypedResultDecimalViaTableNameViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.GreaterThan, 5m);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.SumAsync<decimal?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnDecimal"),
                    field);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnDecimal > 5m).Sum(t => t.ColumnDecimal), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionSumAsyncTypedResultDecimalViaTableNameViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.GreaterThan, 5m),
                new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.LessThanOrEqual, 8m)
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.SumAsync<decimal?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnDecimal"),
                    fields);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnDecimal > 5m && t.ColumnDecimal <= 8m).Sum(t => t.ColumnDecimal), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionSumAsyncTypedResultDecimalViaTableNameViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.GreaterThan, 5m),
                new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.LessThanOrEqual, 8m)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.SumAsync<decimal?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnDecimal"),
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnDecimal > 5m && t.ColumnDecimal <= 8m).Sum(t => t.ColumnDecimal), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionSumAsyncTypedResultDoubleViaTableNameWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.SumAsync<double?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnFloat"),
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Sum(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionSumAsyncTypedResultDoubleViaTableNameViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.SumAsync<double?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnFloat"),
                    new { ColumnFloat = 1d });

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnFloat == 1d).Sum(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionSumAsyncTypedResultDoubleViaTableNameViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnFloat), Operation.GreaterThan, 5d);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.SumAsync<double?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnFloat"),
                    field);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnFloat > 5d).Sum(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionSumAsyncTypedResultDoubleViaTableNameViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnFloat), Operation.GreaterThan, 5d),
                new QueryField(nameof(IdentityTable.ColumnFloat), Operation.LessThanOrEqual, 8d)
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.SumAsync<double?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnFloat"),
                    fields);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnFloat > 5d && t.ColumnFloat <= 8d).Sum(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionSumAsyncTypedResultDoubleViaTableNameViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnFloat), Operation.GreaterThan, 5d),
                new QueryField(nameof(IdentityTable.ColumnFloat), Operation.LessThanOrEqual, 8d)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.SumAsync<double?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnFloat"),
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnFloat > 5d && t.ColumnFloat <= 8d).Sum(t => t.ColumnFloat), result);
            }
        }

        #endregion
    }
}
