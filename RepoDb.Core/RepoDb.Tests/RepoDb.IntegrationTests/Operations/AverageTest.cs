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
    public class AverageTest
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

        #region Average<TEntity>

        [TestMethod]
        public void TestSqlConnectionAverageWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Average<IdentityTable>(e => e.ColumnInt,
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Average(t => t.ColumnInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionAverageViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Average<IdentityTable>(e => e.ColumnInt,
                    item => item.ColumnInt > 5 && item.ColumnInt <= 8);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Average(t => t.ColumnInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionAverageViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Average<IdentityTable>(e => e.ColumnInt,
                    new { ColumnInt = 1 });

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt == 1).Average(t => t.ColumnInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionAverageViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Average<IdentityTable>(e => e.ColumnInt,
                    field);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5).Average(t => t.ColumnInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionAverageViaQueryFields()
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
                var result = connection.Average<IdentityTable>(e => e.ColumnInt,
                    fields);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Average(t => t.ColumnInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionAverageViaQueryGroup()
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
                var result = connection.Average<IdentityTable>(e => e.ColumnInt,
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Average(t => t.ColumnInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionAverageTypedResultWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Average<IdentityTable, double?>(e => e.ColumnInt,
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Average(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionAverageTypedResultViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Average<IdentityTable, double?>(e => e.ColumnInt,
                    item => item.ColumnInt > 5 && item.ColumnInt <= 8);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Average(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionAverageTypedResultViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Average<IdentityTable, double?>(e => e.ColumnInt,
                    new { ColumnInt = 1 });

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt == 1).Average(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionAverageTypedResultViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Average<IdentityTable, double?>(e => e.ColumnInt,
                    field);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5).Average(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionAverageTypedResultViaQueryFields()
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
                var result = connection.Average<IdentityTable, double?>(e => e.ColumnInt,
                    fields);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Average(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionAverageTypedResultViaQueryGroup()
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
                var result = connection.Average<IdentityTable, double?>(e => e.ColumnInt,
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Average(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionAverageTypedResultDecimalWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Average<IdentityTable, decimal?>(e => e.ColumnDecimal,
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Average(t => t.ColumnDecimal), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionAverageTypedResultDecimalViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Average<IdentityTable, decimal?>(e => e.ColumnDecimal,
                    item => item.ColumnDecimal > 5m && item.ColumnDecimal <= 8m);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnDecimal > 5m && t.ColumnDecimal <= 8m).Average(t => t.ColumnDecimal), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionAverageTypedResultDecimalViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Average<IdentityTable, decimal?>(e => e.ColumnDecimal,
                    new { ColumnDecimal = 1m });

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnDecimal == 1m).Average(t => t.ColumnDecimal), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionAverageTypedResultDecimalViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.GreaterThan, 5m);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Average<IdentityTable, decimal?>(e => e.ColumnDecimal,
                    field);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnDecimal > 5m).Average(t => t.ColumnDecimal), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionAverageTypedResultDecimalViaQueryFields()
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
                var result = connection.Average<IdentityTable, decimal?>(e => e.ColumnDecimal,
                    fields);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnDecimal > 5m && t.ColumnDecimal <= 8m).Average(t => t.ColumnDecimal), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionAverageTypedResultDecimalViaQueryGroup()
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
                var result = connection.Average<IdentityTable, decimal?>(e => e.ColumnDecimal,
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnDecimal > 5m && t.ColumnDecimal <= 8m).Average(t => t.ColumnDecimal), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionAverageTypedResultDoubleWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Average<IdentityTable, double?>(e => e.ColumnFloat,
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Average(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionAverageTypedResultDoubleViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Average<IdentityTable, double?>(e => e.ColumnFloat,
                    item => item.ColumnFloat > 5d && item.ColumnFloat <= 8d);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnFloat > 5d && t.ColumnFloat <= 8d).Average(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionAverageTypedResultDoubleViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Average<IdentityTable, double?>(e => e.ColumnFloat,
                    new { ColumnFloat = 1d });

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnFloat == 1d).Average(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionAverageTypedResultDoubleViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnFloat), Operation.GreaterThan, 5d);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Average<IdentityTable, double?>(e => e.ColumnFloat,
                    field);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnFloat > 5d).Average(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionAverageTypedResultDoubleViaQueryFields()
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
                var result = connection.Average<IdentityTable, double?>(e => e.ColumnFloat,
                    fields);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnFloat > 5d && t.ColumnFloat <= 8d).Average(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionAverageTypedResultDoubleViaQueryGroup()
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
                var result = connection.Average<IdentityTable, double?>(e => e.ColumnFloat,
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnFloat > 5d && t.ColumnFloat <= 8d).Average(t => t.ColumnFloat), result);
            }
        }

        #endregion

        #region AverageAsync<TEntity>

        [TestMethod]
        public async Task TestSqlConnectionAverageAsyncWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.AverageAsync<IdentityTable>(e => e.ColumnInt,
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Average(t => t.ColumnInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionAverageAsyncViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.AverageAsync<IdentityTable>(e => e.ColumnInt,
                    item => item.ColumnInt > 5 && item.ColumnInt <= 8);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Average(t => t.ColumnInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionAverageAsyncViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.AverageAsync<IdentityTable>(e => e.ColumnInt,
                    new { ColumnInt = 1 });

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt == 1).Average(t => t.ColumnInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionAverageAsyncViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.AverageAsync<IdentityTable>(e => e.ColumnInt,
                    field);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5).Average(t => t.ColumnInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionAverageAsyncViaQueryFields()
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
                var result = await connection.AverageAsync<IdentityTable>(e => e.ColumnInt,
                    fields);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Average(t => t.ColumnInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionAverageAsyncViaQueryGroup()
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
                var result = await connection.AverageAsync<IdentityTable>(e => e.ColumnInt,
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Average(t => t.ColumnInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionAverageAsyncTypedResultWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.AverageAsync<IdentityTable, double?>(e => e.ColumnInt,
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Average(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionAverageAsyncTypedResultViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.AverageAsync<IdentityTable, double?>(e => e.ColumnInt,
                    item => item.ColumnInt > 5 && item.ColumnInt <= 8);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Average(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionAverageAsyncTypedResultViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.AverageAsync<IdentityTable, double?>(e => e.ColumnInt,
                    new { ColumnInt = 1 });

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt == 1).Average(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionAverageAsyncTypedResultViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.AverageAsync<IdentityTable, double?>(e => e.ColumnInt,
                    field);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5).Average(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionAverageAsyncTypedResultViaQueryFields()
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
                var result = await connection.AverageAsync<IdentityTable, double?>(e => e.ColumnInt,
                    fields);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Average(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionAverageAsyncTypedResultViaQueryGroup()
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
                var result = await connection.AverageAsync<IdentityTable, double?>(e => e.ColumnInt,
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Average(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionAverageAsyncTypedResultDecimalWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.AverageAsync<IdentityTable, decimal?>(e => e.ColumnDecimal,
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Average(t => t.ColumnDecimal), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionAverageAsyncTypedResultDecimalViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.AverageAsync<IdentityTable, decimal?>(e => e.ColumnDecimal,
                    item => item.ColumnDecimal > 5m && item.ColumnDecimal <= 8m);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnDecimal > 5m && t.ColumnDecimal <= 8m).Average(t => t.ColumnDecimal), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionAverageAsyncTypedResultDecimalViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.AverageAsync<IdentityTable, decimal?>(e => e.ColumnDecimal,
                    new { ColumnDecimal = 1m });

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnDecimal == 1m).Average(t => t.ColumnDecimal), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionAverageAsyncTypedResultDecimalViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.GreaterThan, 5m);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.AverageAsync<IdentityTable, decimal?>(e => e.ColumnDecimal,
                    field);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnDecimal > 5m).Average(t => t.ColumnDecimal), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionAverageAsyncTypedResultDecimalViaQueryFields()
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
                var result = await connection.AverageAsync<IdentityTable, decimal?>(e => e.ColumnDecimal,
                    fields);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnDecimal > 5m && t.ColumnDecimal <= 8m).Average(t => t.ColumnDecimal), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionAverageAsyncTypedResultDecimalViaQueryGroup()
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
                var result = await connection.AverageAsync<IdentityTable, decimal?>(e => e.ColumnDecimal,
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnDecimal > 5m && t.ColumnDecimal <= 8m).Average(t => t.ColumnDecimal), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionAverageAsyncTypedResultDoubleWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.AverageAsync<IdentityTable, double?>(e => e.ColumnFloat,
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Average(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionAverageAsyncTypedResultDoubleViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.AverageAsync<IdentityTable, double?>(e => e.ColumnFloat,
                    item => item.ColumnFloat > 5d && item.ColumnFloat <= 8d);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnFloat > 5d && t.ColumnFloat <= 8d).Average(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionAverageAsyncTypedResultDoubleViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.AverageAsync<IdentityTable, double?>(e => e.ColumnFloat,
                    new { ColumnFloat = 1d });

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnFloat == 1d).Average(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionAverageAsyncTypedResultDoubleViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnFloat), Operation.GreaterThan, 5d);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.AverageAsync<IdentityTable, double?>(e => e.ColumnFloat,
                    field);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnFloat > 5d).Average(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionAverageAsyncTypedResultDoubleViaQueryFields()
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
                var result = await connection.AverageAsync<IdentityTable, double?>(e => e.ColumnFloat,
                    fields);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnFloat > 5d && t.ColumnFloat <= 8d).Average(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionAverageAsyncTypedResultDoubleViaQueryGroup()
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
                var result = await connection.AverageAsync<IdentityTable, double?>(e => e.ColumnFloat,
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnFloat > 5d && t.ColumnFloat <= 8d).Average(t => t.ColumnFloat), result);
            }
        }

        #endregion

        #region Average(TableName)

        [TestMethod]
        public void TestSqlConnectionAverageViaTableNameWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Average(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Average(t => t.ColumnInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionAverageViaTableNameViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Average(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    new { ColumnInt = 1 });

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt == 1).Average(t => t.ColumnInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionAverageViaTableNameViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Average(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    field);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5).Average(t => t.ColumnInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionAverageViaTableNameViaQueryFields()
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
                var result = connection.Average(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    fields);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Average(t => t.ColumnInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionAverageViaTableNameViaQueryGroup()
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
                var result = connection.Average(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Average(t => t.ColumnInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionAverageViaTableNameTypedResultWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Average<double?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Average(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionAverageViaTableNameTypedResultViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Average<double?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    new { ColumnInt = 1 });

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt == 1).Average(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionAverageViaTableNameTypedResultViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Average<double?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    field);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5).Average(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionAverageViaTableNameTypedResultViaQueryFields()
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
                var result = connection.Average<double?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    fields);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Average(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionAverageViaTableNameTypedResultViaQueryGroup()
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
                var result = connection.Average<double?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Average(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionAverageViaTableNameTypedResultDecimalWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Average<decimal?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnDecimal", typeof(decimal)),
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Average(t => t.ColumnDecimal), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionAverageViaTableNameTypedResultDecimalViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Average<decimal?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnDecimal", typeof(decimal)),
                    new { ColumnDecimal = 1m });

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnDecimal == 1m).Average(t => t.ColumnDecimal), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionAverageViaTableNameTypedResultDecimalViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.GreaterThan, 5m);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Average<decimal?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnDecimal", typeof(decimal)),
                    field);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnDecimal > 5m).Average(t => t.ColumnDecimal), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionAverageViaTableNameTypedResultDecimalViaQueryFields()
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
                var result = connection.Average<decimal?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnDecimal", typeof(decimal)),
                    fields);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnDecimal > 5m && t.ColumnDecimal <= 8m).Average(t => t.ColumnDecimal), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionAverageViaTableNameTypedResultDecimalViaQueryGroup()
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
                var result = connection.Average<decimal?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnDecimal", typeof(decimal)),
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnDecimal > 5m && t.ColumnDecimal <= 8m).Average(t => t.ColumnDecimal), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionAverageViaTableNameTypedResultDoubleWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Average<double?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnFloat"),
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Average(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionAverageViaTableNameTypedResultDoubleViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Average<double?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnFloat"),
                    new { ColumnFloat = 1d });

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnFloat == 1d).Average(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionAverageViaTableNameTypedResultDoubleViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnFloat), Operation.GreaterThan, 5d);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Average<double?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnFloat"),
                    field);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnFloat > 5d).Average(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionAverageViaTableNameTypedResultDoubleViaQueryFields()
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
                var result = connection.Average<double?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnFloat"),
                    fields);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnFloat > 5d && t.ColumnFloat <= 8d).Average(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionAverageViaTableNameTypedResultDoubleViaQueryGroup()
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
                var result = connection.Average<double?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnFloat"),
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnFloat > 5d && t.ColumnFloat <= 8d).Average(t => t.ColumnFloat), result);
            }
        }

        #endregion

        #region AverageAsync(TableName)

        [TestMethod]
        public async Task TestSqlConnectionAverageAsyncViaTableNameWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.AverageAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Average(t => t.ColumnInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionAverageAsyncViaTableNameViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.AverageAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    new { ColumnInt = 1 });

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt == 1).Average(t => t.ColumnInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionAverageAsyncViaTableNameViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.AverageAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    field);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5).Average(t => t.ColumnInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionAverageAsyncViaTableNameViaQueryFields()
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
                var result = await connection.AverageAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    fields);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Average(t => t.ColumnInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionAverageAsyncViaTableNameViaQueryGroup()
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
                var result = await connection.AverageAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Average(t => t.ColumnInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionAverageAsyncTypedResultViaTableNameWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.AverageAsync<double?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Average(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionAverageAsyncTypedResultViaTableNameViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.AverageAsync<double?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    new { ColumnInt = 1 });

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt == 1).Average(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionAverageAsyncTypedResultViaTableNameViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.AverageAsync<double?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    field);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5).Average(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionAverageAsyncTypedResultViaTableNameViaQueryFields()
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
                var result = await connection.AverageAsync<double?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    fields);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Average(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionAverageAsyncTypedResultViaTableNameViaQueryGroup()
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
                var result = await connection.AverageAsync<double?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Average(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionAverageAsyncTypedResultDecimalViaTableNameWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.AverageAsync<decimal?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnDecimal", typeof(decimal)),
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Average(t => t.ColumnDecimal), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionAverageAsyncTypedResultDecimalViaTableNameViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.AverageAsync<decimal?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnDecimal", typeof(decimal)),
                    new { ColumnDecimal = 1m });

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnDecimal == 1m).Average(t => t.ColumnDecimal), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionAverageAsyncTypedResultDecimalViaTableNameViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnDecimal), Operation.GreaterThan, 5m);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.AverageAsync<decimal?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnDecimal", typeof(decimal)),
                    field);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnDecimal > 5m).Average(t => t.ColumnDecimal), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionAverageAsyncTypedResultDecimalViaTableNameViaQueryFields()
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
                var result = await connection.AverageAsync<decimal?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnDecimal", typeof(decimal)),
                    fields);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnDecimal > 5m && t.ColumnDecimal <= 8m).Average(t => t.ColumnDecimal), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionAverageAsyncTypedResultDecimalViaTableNameViaQueryGroup()
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
                var result = await connection.AverageAsync<decimal?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnDecimal", typeof(decimal)),
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnDecimal > 5m && t.ColumnDecimal <= 8m).Average(t => t.ColumnDecimal), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionAverageAsyncTypedResultDoubleViaTableNameWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.AverageAsync<double?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnFloat"),
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Average(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionAverageAsyncTypedResultDoubleViaTableNameViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.AverageAsync<double?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnFloat"),
                    new { ColumnFloat = 1d });

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnFloat == 1d).Average(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionAverageAsyncTypedResultDoubleViaTableNameViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnFloat), Operation.GreaterThan, 5d);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.AverageAsync<double?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnFloat"),
                    field);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnFloat > 5d).Average(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionAverageAsyncTypedResultDoubleViaTableNameViaQueryFields()
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
                var result = await connection.AverageAsync<double?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnFloat"),
                    fields);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnFloat > 5d && t.ColumnFloat <= 8d).Average(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionAverageAsyncTypedResultDoubleViaTableNameViaQueryGroup()
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
                var result = await connection.AverageAsync<double?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnFloat"),
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnFloat > 5d && t.ColumnFloat <= 8d).Average(t => t.ColumnFloat), result);
            }
        }

        #endregion
    }
}
