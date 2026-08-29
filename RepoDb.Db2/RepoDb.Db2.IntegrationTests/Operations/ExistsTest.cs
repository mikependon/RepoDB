#region Copyright Attributions

// Copyright (c) 2019 Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using IBM.Data.Db2;
using RepoDb.Enumerations;
using RepoDb.Db2.IntegrationTests.Models;
using RepoDb.Db2.IntegrationTests.Setup;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.Db2.IntegrationTests.Operations
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
        public void TestDb2ConnectionExistsWithoutExpression()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act
                var result = connection.Exists<CompleteTable>((object)null);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestDb2ConnectionExistsWithoutExpressionWithAutomaticConversion()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
                try
                {
                    // Act
                    var result = connection.Exists<CompleteTable>((object)null);

                    // Assert
                    Assert.IsTrue(result);
                }
                finally
                {
                    GlobalConfiguration.Options.ConversionType = ConversionType.Default;
                }
            }
        }

        [TestMethod]
        public void TestDb2ConnectionExistsViaExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();
            var ids = new[] { tables.First().Id, tables.Last().Id };

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act
                var result = connection.Exists<CompleteTable>(e => ids.Contains(e.Id));

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestDb2ConnectionExistsViaExpressionNoMatch()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();
            var missingId = tables.Max(t => t.Id) + 1000;

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act
                var result = connection.Exists<CompleteTable>(e => e.Id == missingId);

                // Assert
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public void TestDb2ConnectionExistsViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act
                var result = connection.Exists<CompleteTable>(new { tables.First().Id });

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestDb2ConnectionExistsViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act
                var result = connection.Exists<CompleteTable>(new QueryField("Id", tables.First().Id));

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestDb2ConnectionExistsViaQueryFields()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act
                var result = connection.Exists<CompleteTable>(queryFields);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestDb2ConnectionExistsViaQueryGroup()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act
                var result = connection.Exists<CompleteTable>(queryGroup);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestDb2ConnectionExistsWithHintsThrows()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act/Assert: AreTableHintsSupported == false for Db2 - any non-null/non-whitespace
                // "hints" argument must throw, rather than silently being ignored.
                Assert.Throws<NotSupportedException>(() =>
                    connection.Exists<CompleteTable>((object)null, hints: "NOLOCK"));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestDb2ConnectionExistsAsyncWithoutExpression()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExistsAsync<CompleteTable>((object)null);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task TestDb2ConnectionExistsAsyncWithoutExpressionWithAutomaticConversion()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
                try
                {
                    // Act
                    var result = await connection.ExistsAsync<CompleteTable>((object)null);

                    // Assert
                    Assert.IsTrue(result);
                }
                finally
                {
                    GlobalConfiguration.Options.ConversionType = ConversionType.Default;
                }
            }
        }

        [TestMethod]
        public async Task TestDb2ConnectionExistsAsyncViaExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();
            var ids = new[] { tables.First().Id, tables.Last().Id };

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExistsAsync<CompleteTable>(e => ids.Contains(e.Id));

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task TestDb2ConnectionExistsAsyncViaExpressionNoMatch()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();
            var missingId = tables.Max(t => t.Id) + 1000;

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExistsAsync<CompleteTable>(e => e.Id == missingId);

                // Assert
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task TestDb2ConnectionExistsAsyncViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExistsAsync<CompleteTable>(new { tables.First().Id });

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task TestDb2ConnectionExistsAsyncViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExistsAsync<CompleteTable>(new QueryField("Id", tables.First().Id));

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task TestDb2ConnectionExistsAsyncViaQueryFields()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExistsAsync<CompleteTable>(queryFields);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task TestDb2ConnectionExistsAsyncViaQueryGroup()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExistsAsync<CompleteTable>(queryGroup);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task TestDb2ConnectionExistsAsyncWithHintsThrows()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act/Assert: AreTableHintsSupported == false for Db2 - any non-null/non-whitespace
                // "hints" argument must throw, rather than silently being ignored.
                await Assert.ThrowsAsync<NotSupportedException>(() =>
                    connection.ExistsAsync<CompleteTable>((object)null, hints: "NOLOCK"));
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestDb2ConnectionExistsViaTableNameWithoutExpression()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act
                var result = connection.Exists(ClassMappedNameCache.Get<CompleteTable>(),
                    (object)null);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestDb2ConnectionExistsViaTableNameViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act
                var result = connection.Exists(ClassMappedNameCache.Get<CompleteTable>(),
                    new { tables.First().Id });

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestDb2ConnectionExistsViaTableNameViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act
                var result = connection.Exists(ClassMappedNameCache.Get<CompleteTable>(),
                    new QueryField("Id", tables.First().Id));

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestDb2ConnectionExistsViaTableNameViaQueryFields()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act
                var result = connection.Exists(ClassMappedNameCache.Get<CompleteTable>(),
                    queryFields);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestDb2ConnectionExistsViaTableNameViaQueryGroup()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act
                var result = connection.Exists(ClassMappedNameCache.Get<CompleteTable>(),
                    queryGroup);

                // Assert
                Assert.IsTrue(result);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestDb2ConnectionExistsAsyncViaTableNameWithoutExpression()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExistsAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    (object)null);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task TestDb2ConnectionExistsAsyncViaTableNameViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExistsAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    new { tables.First().Id });

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task TestDb2ConnectionExistsAsyncViaTableNameViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExistsAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    new QueryField("Id", tables.First().Id));

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task TestDb2ConnectionExistsAsyncViaTableNameViaQueryFields()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExistsAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    queryFields);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task TestDb2ConnectionExistsAsyncViaTableNameViaQueryGroup()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExistsAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    queryGroup);

                // Assert
                Assert.IsTrue(result);
            }
        }

        #endregion

        #endregion
    }
}
