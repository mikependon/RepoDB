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
    public class MinTest
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

        #region Min<TEntity>

        [TestMethod]
        public void TestSqlConnectionMinWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Min<IdentityTable>(e => e.ColumnInt,
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMinViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Min<IdentityTable>(e => e.ColumnInt,
                    item => item.ColumnInt > 5 && item.ColumnInt <= 8);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Min(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMinViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Min<IdentityTable>(e => e.ColumnInt,
                    new { ColumnInt = 1 });

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt == 1).Min(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMinViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Min<IdentityTable>(e => e.ColumnInt,
                    field);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5).Min(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMinViaQueryFields()
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
                var result = connection.Min<IdentityTable>(e => e.ColumnInt,
                    fields);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Min(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMinViaQueryGroup()
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
                var result = connection.Min<IdentityTable>(e => e.ColumnInt,
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Min(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMinTypedResultWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Min<IdentityTable, int?>(e => e.ColumnInt,
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMinTypedResultViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Min<IdentityTable, int?>(e => e.ColumnInt,
                    item => item.ColumnInt > 5 && item.ColumnInt <= 8);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Min(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMinTypedResultViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Min<IdentityTable, int?>(e => e.ColumnInt,
                    new { ColumnInt = 1 });

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt == 1).Min(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMinTypedResultViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Min<IdentityTable, int?>(e => e.ColumnInt,
                    field);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5).Min(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMinTypedResultViaQueryFields()
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
                var result = connection.Min<IdentityTable, int?>(e => e.ColumnInt,
                    fields);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Min(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMinTypedResultViaQueryGroup()
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
                var result = connection.Min<IdentityTable, int?>(e => e.ColumnInt,
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Min(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMinTypedResultDateTimeWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Min<IdentityTable, DateTime?>(e => e.ColumnDateTime,
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnDateTime), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMinTypedResultDateTimeViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Min<IdentityTable, DateTime?>(e => e.ColumnDateTime,
                    item => item.ColumnDateTime > Helper.EpocDate.AddDays(5) && item.ColumnDateTime <= Helper.EpocDate.AddDays(8));

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnDateTime > Helper.EpocDate.AddDays(5) && t.ColumnDateTime <= Helper.EpocDate.AddDays(8)).Min(t => t.ColumnDateTime), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMinTypedResultDateTimeViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Min<IdentityTable, DateTime?>(e => e.ColumnDateTime,
                    new { ColumnDateTime = Helper.EpocDate.AddDays(1) });

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnDateTime == Helper.EpocDate.AddDays(1)).Min(t => t.ColumnDateTime), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMinTypedResultDateTimeViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnDateTime), Operation.GreaterThan, Helper.EpocDate.AddDays(5));

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Min<IdentityTable, DateTime?>(e => e.ColumnDateTime,
                    field);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnDateTime > Helper.EpocDate.AddDays(5)).Min(t => t.ColumnDateTime), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMinTypedResultDateTimeViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDateTime), Operation.GreaterThan, Helper.EpocDate.AddDays(5)),
                new QueryField(nameof(IdentityTable.ColumnDateTime), Operation.LessThanOrEqual, Helper.EpocDate.AddDays(8))
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Min<IdentityTable, DateTime?>(e => e.ColumnDateTime,
                    fields);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnDateTime > Helper.EpocDate.AddDays(5) && t.ColumnDateTime <= Helper.EpocDate.AddDays(8)).Min(t => t.ColumnDateTime), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMinTypedResultDateTimeViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDateTime), Operation.GreaterThan, Helper.EpocDate.AddDays(5)),
                new QueryField(nameof(IdentityTable.ColumnDateTime), Operation.LessThanOrEqual, Helper.EpocDate.AddDays(8))
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Min<IdentityTable, DateTime?>(e => e.ColumnDateTime,
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnDateTime > Helper.EpocDate.AddDays(5) && t.ColumnDateTime <= Helper.EpocDate.AddDays(8)).Min(t => t.ColumnDateTime), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMinTypedResultDoubleWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Min<IdentityTable, double?>(e => e.ColumnFloat,
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMinTypedResultDoubleViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Min<IdentityTable, double?>(e => e.ColumnFloat,
                    item => item.ColumnFloat > 5d && item.ColumnFloat <= 8d);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnFloat > 5d && t.ColumnFloat <= 8d).Min(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMinTypedResultDoubleViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Min<IdentityTable, double?>(e => e.ColumnFloat,
                    new { ColumnFloat = 1d });

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnFloat == 1d).Min(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMinTypedResultDoubleViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnFloat), Operation.GreaterThan, 5d);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Min<IdentityTable, double?>(e => e.ColumnFloat,
                    field);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnFloat > 5d).Min(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMinTypedResultDoubleViaQueryFields()
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
                var result = connection.Min<IdentityTable, double?>(e => e.ColumnFloat,
                    fields);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnFloat > 5d && t.ColumnFloat <= 8d).Min(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMinTypedResultDoubleViaQueryGroup()
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
                var result = connection.Min<IdentityTable, double?>(e => e.ColumnFloat,
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnFloat > 5d && t.ColumnFloat <= 8d).Min(t => t.ColumnFloat), result);
            }
        }

        #endregion

        #region MinAsync<TEntity>

        [TestMethod]
        public async Task TestSqlConnectionMinAsyncWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.MinAsync<IdentityTable>(e => e.ColumnInt,
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMinAsyncViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.MinAsync<IdentityTable>(e => e.ColumnInt,
                    item => item.ColumnInt > 5 && item.ColumnInt <= 8);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Min(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMinAsyncViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.MinAsync<IdentityTable>(e => e.ColumnInt,
                    new { ColumnInt = 1 });

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt == 1).Min(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMinAsyncViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.MinAsync<IdentityTable>(e => e.ColumnInt,
                    field);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5).Min(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMinAsyncViaQueryFields()
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
                var result = await connection.MinAsync<IdentityTable>(e => e.ColumnInt,
                    fields);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Min(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMinAsyncViaQueryGroup()
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
                var result = await connection.MinAsync<IdentityTable>(e => e.ColumnInt,
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Min(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMinAsyncTypedResultWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.MinAsync<IdentityTable, int?>(e => e.ColumnInt,
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMinAsyncTypedResultViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.MinAsync<IdentityTable, int?>(e => e.ColumnInt,
                    item => item.ColumnInt > 5 && item.ColumnInt <= 8);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Min(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMinAsyncTypedResultViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.MinAsync<IdentityTable, int?>(e => e.ColumnInt,
                    new { ColumnInt = 1 });

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt == 1).Min(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMinAsyncTypedResultViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.MinAsync<IdentityTable, int?>(e => e.ColumnInt,
                    field);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5).Min(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMinAsyncTypedResultViaQueryFields()
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
                var result = await connection.MinAsync<IdentityTable, int?>(e => e.ColumnInt,
                    fields);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Min(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMinAsyncTypedResultViaQueryGroup()
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
                var result = await connection.MinAsync<IdentityTable, int?>(e => e.ColumnInt,
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Min(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMinAsyncTypedResultDateTimeWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.MinAsync<IdentityTable, DateTime?>(e => e.ColumnDateTime,
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnDateTime), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMinAsyncTypedResultDateTimeViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.MinAsync<IdentityTable, DateTime?>(e => e.ColumnDateTime,
                    item => item.ColumnDateTime > Helper.EpocDate.AddDays(5) && item.ColumnDateTime <= Helper.EpocDate.AddDays(8));

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnDateTime > Helper.EpocDate.AddDays(5) && t.ColumnDateTime <= Helper.EpocDate.AddDays(8)).Min(t => t.ColumnDateTime), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMinAsyncTypedResultDateTimeViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.MinAsync<IdentityTable, DateTime?>(e => e.ColumnDateTime,
                    new { ColumnDateTime = Helper.EpocDate.AddDays(1) });

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnDateTime == Helper.EpocDate.AddDays(1)).Min(t => t.ColumnDateTime), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMinAsyncTypedResultDateTimeViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnDateTime), Operation.GreaterThan, Helper.EpocDate.AddDays(5));

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.MinAsync<IdentityTable, DateTime?>(e => e.ColumnDateTime,
                    field);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnDateTime > Helper.EpocDate.AddDays(5)).Min(t => t.ColumnDateTime), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMinAsyncTypedResultDateTimeViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDateTime), Operation.GreaterThan, Helper.EpocDate.AddDays(5)),
                new QueryField(nameof(IdentityTable.ColumnDateTime), Operation.LessThanOrEqual, Helper.EpocDate.AddDays(8))
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.MinAsync<IdentityTable, DateTime?>(e => e.ColumnDateTime,
                    fields);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnDateTime > Helper.EpocDate.AddDays(5) && t.ColumnDateTime <= Helper.EpocDate.AddDays(8)).Min(t => t.ColumnDateTime), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMinAsyncTypedResultDateTimeViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDateTime), Operation.GreaterThan, Helper.EpocDate.AddDays(5)),
                new QueryField(nameof(IdentityTable.ColumnDateTime), Operation.LessThanOrEqual, Helper.EpocDate.AddDays(8))
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.MinAsync<IdentityTable, DateTime?>(e => e.ColumnDateTime,
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnDateTime > Helper.EpocDate.AddDays(5) && t.ColumnDateTime <= Helper.EpocDate.AddDays(8)).Min(t => t.ColumnDateTime), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMinAsyncTypedResultDoubleWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.MinAsync<IdentityTable, double?>(e => e.ColumnFloat,
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMinAsyncTypedResultDoubleViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.MinAsync<IdentityTable, double?>(e => e.ColumnFloat,
                    item => item.ColumnFloat > 5d && item.ColumnFloat <= 8d);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnFloat > 5d && t.ColumnFloat <= 8d).Min(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMinAsyncTypedResultDoubleViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.MinAsync<IdentityTable, double?>(e => e.ColumnFloat,
                    new { ColumnFloat = 1d });

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnFloat == 1d).Min(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMinAsyncTypedResultDoubleViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnFloat), Operation.GreaterThan, 5d);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.MinAsync<IdentityTable, double?>(e => e.ColumnFloat,
                    field);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnFloat > 5d).Min(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMinAsyncTypedResultDoubleViaQueryFields()
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
                var result = await connection.MinAsync<IdentityTable, double?>(e => e.ColumnFloat,
                    fields);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnFloat > 5d && t.ColumnFloat <= 8d).Min(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMinAsyncTypedResultDoubleViaQueryGroup()
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
                var result = await connection.MinAsync<IdentityTable, double?>(e => e.ColumnFloat,
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnFloat > 5d && t.ColumnFloat <= 8d).Min(t => t.ColumnFloat), result);
            }
        }

        #endregion

        #region Min(TableName)

        [TestMethod]
        public void TestSqlConnectionMinViaTableNameWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Min(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMinViaTableNameViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Min(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    new { ColumnInt = 1 });

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt == 1).Min(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMinViaTableNameViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Min(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    field);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5).Min(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMinViaTableNameViaQueryFields()
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
                var result = connection.Min(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    fields);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Min(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMinViaTableNameViaQueryGroup()
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
                var result = connection.Min(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Min(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMinViaTableNameTypedResultWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Min<int?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMinViaTableNameTypedResultViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Min<int?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    new { ColumnInt = 1 });

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt == 1).Min(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMinViaTableNameTypedResultViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Min<int?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    field);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5).Min(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMinViaTableNameTypedResultViaQueryFields()
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
                var result = connection.Min<int?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    fields);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Min(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMinViaTableNameTypedResultViaQueryGroup()
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
                var result = connection.Min<int?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Min(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMinViaTableNameTypedResultDateTimeWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Min<DateTime?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnDateTime"),
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnDateTime), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMinViaTableNameTypedResultDateTimeViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Min<DateTime?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnDateTime"),
                    new { ColumnDateTime = Helper.EpocDate.AddDays(1) });

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnDateTime == Helper.EpocDate.AddDays(1)).Min(t => t.ColumnDateTime), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMinViaTableNameTypedResultDateTimeViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnDateTime), Operation.GreaterThan, Helper.EpocDate.AddDays(5));

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Min<DateTime?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnDateTime"),
                    field);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnDateTime > Helper.EpocDate.AddDays(5)).Min(t => t.ColumnDateTime), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMinViaTableNameTypedResultDateTimeViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDateTime), Operation.GreaterThan, Helper.EpocDate.AddDays(5)),
                new QueryField(nameof(IdentityTable.ColumnDateTime), Operation.LessThanOrEqual, Helper.EpocDate.AddDays(8))
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Min<DateTime?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnDateTime"),
                    fields);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnDateTime > Helper.EpocDate.AddDays(5) && t.ColumnDateTime <= Helper.EpocDate.AddDays(8)).Min(t => t.ColumnDateTime), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMinViaTableNameTypedResultDateTimeViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDateTime), Operation.GreaterThan, Helper.EpocDate.AddDays(5)),
                new QueryField(nameof(IdentityTable.ColumnDateTime), Operation.LessThanOrEqual, Helper.EpocDate.AddDays(8))
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Min<DateTime?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnDateTime"),
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnDateTime > Helper.EpocDate.AddDays(5) && t.ColumnDateTime <= Helper.EpocDate.AddDays(8)).Min(t => t.ColumnDateTime), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMinViaTableNameTypedResultDoubleWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Min<double?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnFloat"),
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMinViaTableNameTypedResultDoubleViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Min<double?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnFloat"),
                    new { ColumnFloat = 1d });

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnFloat == 1d).Min(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMinViaTableNameTypedResultDoubleViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnFloat), Operation.GreaterThan, 5d);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Min<double?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnFloat"),
                    field);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnFloat > 5d).Min(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMinViaTableNameTypedResultDoubleViaQueryFields()
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
                var result = connection.Min<double?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnFloat"),
                    fields);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnFloat > 5d && t.ColumnFloat <= 8d).Min(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMinViaTableNameTypedResultDoubleViaQueryGroup()
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
                var result = connection.Min<double?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnFloat"),
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnFloat > 5d && t.ColumnFloat <= 8d).Min(t => t.ColumnFloat), result);
            }
        }

        #endregion

        #region MinAsync(TableName)

        [TestMethod]
        public async Task TestSqlConnectionMinAsyncViaTableNameWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.MinAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMinAsyncViaTableNameViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.MinAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    new { ColumnInt = 1 });

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt == 1).Min(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMinAsyncViaTableNameViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.MinAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    field);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5).Min(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMinAsyncViaTableNameViaQueryFields()
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
                var result = await connection.MinAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    fields);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Min(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMinAsyncViaTableNameViaQueryGroup()
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
                var result = await connection.MinAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Min(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMinAsyncTypedResultViaTableNameWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.MinAsync<int?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMinAsyncTypedResultViaTableNameViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.MinAsync<int?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    new { ColumnInt = 1 });

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt == 1).Min(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMinAsyncTypedResultViaTableNameViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.MinAsync<int?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    field);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5).Min(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMinAsyncTypedResultViaTableNameViaQueryFields()
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
                var result = await connection.MinAsync<int?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    fields);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Min(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMinAsyncTypedResultViaTableNameViaQueryGroup()
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
                var result = await connection.MinAsync<int?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Min(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMinAsyncTypedResultDateTimeViaTableNameWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.MinAsync<DateTime?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnDateTime"),
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnDateTime), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMinAsyncTypedResultDateTimeViaTableNameViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.MinAsync<DateTime?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnDateTime"),
                    new { ColumnDateTime = Helper.EpocDate.AddDays(1) });

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnDateTime == Helper.EpocDate.AddDays(1)).Min(t => t.ColumnDateTime), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMinAsyncTypedResultDateTimeViaTableNameViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnDateTime), Operation.GreaterThan, Helper.EpocDate.AddDays(5));

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.MinAsync<DateTime?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnDateTime"),
                    field);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnDateTime > Helper.EpocDate.AddDays(5)).Min(t => t.ColumnDateTime), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMinAsyncTypedResultDateTimeViaTableNameViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDateTime), Operation.GreaterThan, Helper.EpocDate.AddDays(5)),
                new QueryField(nameof(IdentityTable.ColumnDateTime), Operation.LessThanOrEqual, Helper.EpocDate.AddDays(8))
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.MinAsync<DateTime?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnDateTime"),
                    fields);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnDateTime > Helper.EpocDate.AddDays(5) && t.ColumnDateTime <= Helper.EpocDate.AddDays(8)).Min(t => t.ColumnDateTime), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMinAsyncTypedResultDateTimeViaTableNameViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnDateTime), Operation.GreaterThan, Helper.EpocDate.AddDays(5)),
                new QueryField(nameof(IdentityTable.ColumnDateTime), Operation.LessThanOrEqual, Helper.EpocDate.AddDays(8))
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.MinAsync<DateTime?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnDateTime"),
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnDateTime > Helper.EpocDate.AddDays(5) && t.ColumnDateTime <= Helper.EpocDate.AddDays(8)).Min(t => t.ColumnDateTime), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMinAsyncTypedResultDoubleViaTableNameWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.MinAsync<double?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnFloat"),
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Min(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMinAsyncTypedResultDoubleViaTableNameViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.MinAsync<double?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnFloat"),
                    new { ColumnFloat = 1d });

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnFloat == 1d).Min(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMinAsyncTypedResultDoubleViaTableNameViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnFloat), Operation.GreaterThan, 5d);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.MinAsync<double?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnFloat"),
                    field);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnFloat > 5d).Min(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMinAsyncTypedResultDoubleViaTableNameViaQueryFields()
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
                var result = await connection.MinAsync<double?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnFloat"),
                    fields);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnFloat > 5d && t.ColumnFloat <= 8d).Min(t => t.ColumnFloat), result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMinAsyncTypedResultDoubleViaTableNameViaQueryGroup()
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
                var result = await connection.MinAsync<double?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnFloat"),
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnFloat > 5d && t.ColumnFloat <= 8d).Min(t => t.ColumnFloat), result);
            }
        }

        #endregion
    }
}
