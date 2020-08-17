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

        #region Max<TEntity>

        [TestMethod]
        public void TestSqlConnectionMaxWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Max<IdentityTable>(e => e.ColumnInt,
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Max(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMaxViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Max<IdentityTable>(e => e.ColumnInt,
                    item => item.ColumnInt > 5 && item.ColumnInt <= 8);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Max(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMaxViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Max<IdentityTable>(e => e.ColumnInt,
                    new { ColumnInt = 1 });

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt == 1).Max(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMaxViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Max<IdentityTable>(e => e.ColumnInt,
                    field);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5).Max(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMaxViaQueryFields()
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
                var result = connection.Max<IdentityTable>(e => e.ColumnInt,
                    fields);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Max(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMaxViaQueryGroup()
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
                var result = connection.Max<IdentityTable>(e => e.ColumnInt,
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Max(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMaxTypedResultWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Max<IdentityTable, int?>(e => e.ColumnInt,
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Max(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMaxTypedResultViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Max<IdentityTable, int?>(e => e.ColumnInt,
                    item => item.ColumnInt > 5 && item.ColumnInt <= 8);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Max(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMaxTypedResultViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Max<IdentityTable, int?>(e => e.ColumnInt,
                    new { ColumnInt = 1 });

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt == 1).Max(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMaxTypedResultViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Max<IdentityTable, int?>(e => e.ColumnInt,
                    field);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5).Max(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMaxTypedResultViaQueryFields()
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
                var result = connection.Max<IdentityTable, int?>(e => e.ColumnInt,
                    fields);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Max(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMaxTypedResultViaQueryGroup()
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
                var result = connection.Max<IdentityTable, int?>(e => e.ColumnInt,
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Max(t => t.ColumnInt), result);
            }
        }

        #endregion

        #region MaxAsync<TEntity>

        [TestMethod]
        public void TestSqlConnectionMaxAsyncWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MaxAsync<IdentityTable>(e => e.ColumnInt,
                    (object)null).Result;

                // Assert
                Assert.AreEqual(tables.Max(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMaxAsyncViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MaxAsync<IdentityTable>(e => e.ColumnInt,
                    item => item.ColumnInt > 5 && item.ColumnInt <= 8).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Max(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMaxAsyncViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MaxAsync<IdentityTable>(e => e.ColumnInt,
                    new { ColumnInt = 1 }).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt == 1).Max(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMaxAsyncViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MaxAsync<IdentityTable>(e => e.ColumnInt,
                    field).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5).Max(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMaxAsyncViaQueryFields()
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
                var result = connection.MaxAsync<IdentityTable>(e => e.ColumnInt,
                    fields).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Max(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMaxAsyncViaQueryGroup()
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
                var result = connection.MaxAsync<IdentityTable>(e => e.ColumnInt,
                    queryGroup).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Max(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMaxAsyncTypedResultWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MaxAsync<IdentityTable, int?>(e => e.ColumnInt,
                    (object)null).Result;

                // Assert
                Assert.AreEqual(tables.Max(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMaxAsyncTypedResultViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MaxAsync<IdentityTable, int?>(e => e.ColumnInt,
                    item => item.ColumnInt > 5 && item.ColumnInt <= 8).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Max(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMaxAsyncTypedResultViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MaxAsync<IdentityTable, int?>(e => e.ColumnInt,
                    new { ColumnInt = 1 }).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt == 1).Max(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMaxAsyncTypedResultViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MaxAsync<IdentityTable, int?>(e => e.ColumnInt,
                    field).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5).Max(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMaxAsyncTypedResultViaQueryFields()
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
                var result = connection.MaxAsync<IdentityTable, int?>(e => e.ColumnInt,
                    fields).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Max(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMaxAsyncTypedResultViaQueryGroup()
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
                var result = connection.MaxAsync<IdentityTable, int?>(e => e.ColumnInt,
                    queryGroup).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Max(t => t.ColumnInt), result);
            }
        }

        #endregion

        #region Max(TableName)

        [TestMethod]
        public void TestSqlConnectionMaxViaTableNameWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Max(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Max(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMaxViaTableNameViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Max(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    new { ColumnInt = 1 });

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt == 1).Max(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMaxViaTableNameViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Max(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    field);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5).Max(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMaxViaTableNameViaQueryFields()
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
                var result = connection.Max(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    fields);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Max(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMaxViaTableNameViaQueryGroup()
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
                var result = connection.Max(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Max(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMaxViaTableNameTypedResultWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Max<int?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Max(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMaxViaTableNameTypedResultViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Max<int?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    new { ColumnInt = 1 });

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt == 1).Max(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMaxViaTableNameTypedResultViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Max<int?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    field);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5).Max(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMaxViaTableNameTypedResultViaQueryFields()
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
                var result = connection.Max<int?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    fields);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Max(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMaxViaTableNameTypedResultViaQueryGroup()
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
                var result = connection.Max<int?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Max(t => t.ColumnInt), result);
            }
        }

        #endregion

        #region MaxAsync(TableName)

        [TestMethod]
        public void TestSqlConnectionMaxAsyncViaTableNameWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MaxAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    (object)null).Result;

                // Assert
                Assert.AreEqual(tables.Max(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMaxAsyncViaTableNameViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MaxAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    new { ColumnInt = 1 }).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt == 1).Max(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMaxAsyncViaTableNameViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MaxAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    field).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5).Max(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMaxAsyncViaTableNameViaQueryFields()
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
                var result = connection.MaxAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    fields).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Max(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMaxAsyncViaTableNameViaQueryGroup()
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
                var result = connection.MaxAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    queryGroup).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Max(t => t.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMaxAsyncTypedResultViaTableNameWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MaxAsync<int?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    (object)null).Result;

                // Assert
                Assert.AreEqual(tables.Max(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMaxAsyncTypedResultViaTableNameViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MaxAsync<int?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    new { ColumnInt = 1 }).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt == 1).Max(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMaxAsyncTypedResultViaTableNameViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.MaxAsync<int?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    field).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5).Max(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMaxAsyncTypedResultViaTableNameViaQueryFields()
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
                var result = connection.MaxAsync<int?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    fields).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Max(t => t.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMaxAsyncTypedResultViaTableNameViaQueryGroup()
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
                var result = connection.MaxAsync<int?>(ClassMappedNameCache.Get<IdentityTable>(),
                    new Field("ColumnInt"),
                    queryGroup).Result;

                // Assert
                Assert.AreEqual(tables.Where(t => t.ColumnInt > 5 && t.ColumnInt <= 8).Max(t => t.ColumnInt), result);
            }
        }

        #endregion
    }
}
