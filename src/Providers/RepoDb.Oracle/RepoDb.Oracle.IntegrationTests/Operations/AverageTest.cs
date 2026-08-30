#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oracle.ManagedDataAccess.Client;
using RepoDb.Enumerations;
using RepoDb.Oracle.IntegrationTests.Models;
using RepoDb.Oracle.IntegrationTests.Setup;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.Oracle.IntegrationTests.Operations
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

        // NOTE: The aggregate target here is "ColumnSmallInt" (not "ColumnInt"). Helper.CreateCompleteTables
        // generates "ColumnInt" across the *full* Int32.MinValue..Int32.MaxValue range, so summing several rows
        // of it - which is exactly what computing an average does under the hood - would very likely overflow
        // the checked 32-bit accumulation that both this test's own LINQ "Average(...)" expectation and Oracle's
        // arithmetic could hit. "ColumnSmallInt" is generated within Int16's much narrower range, so an average
        // over 10 rows can never overflow.

        #region DataEntity

        #region Sync

        [TestMethod]
        public void TestOracleConnectionAverageWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Average<CompleteTable>(e => e.ColumnSmallInt,
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Average(e => e.ColumnSmallInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public void TestOracleConnectionAverageWithoutExpressionWithAutomaticConversion()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
                try
                {
                    // Act
                    var result = connection.Average<CompleteTable>(e => e.ColumnSmallInt,
                        (object)null);

                    // Assert
                    Assert.AreEqual(tables.Average(e => e.ColumnSmallInt), Convert.ToDouble(result));
                }
                finally
                {
                    GlobalConfiguration.Options.ConversionType = ConversionType.Default;
                }
            }
        }

        [TestMethod]
        public void TestOracleConnectionAverageViaExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();
            var ids = new[] { tables.First().Id, tables.Last().Id };

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Average<CompleteTable>(e => e.ColumnSmallInt,
                    e => ids.Contains(e.Id));

                // Assert
                Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Average(e => e.ColumnSmallInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public void TestOracleConnectionAverageViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Average<CompleteTable>(e => e.ColumnSmallInt,
                    new { tables.First().Id });

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Average(e => e.ColumnSmallInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public void TestOracleConnectionAverageViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Average<CompleteTable>(e => e.ColumnSmallInt,
                    new QueryField("Id", tables.First().Id));

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Average(e => e.ColumnSmallInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public void TestOracleConnectionAverageViaQueryFields()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Average<CompleteTable>(e => e.ColumnSmallInt,
                    queryFields);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Average(e => e.ColumnSmallInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public void TestOracleConnectionAverageViaQueryGroup()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Average<CompleteTable>(e => e.ColumnSmallInt,
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Average(e => e.ColumnSmallInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public void TestOracleConnectionAverageWithHintsThrows()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act/Assert: AreTableHintsSupported == false for Oracle - any non-null/non-whitespace
                // "hints" argument must throw, rather than silently being ignored.
                Assert.Throws<NotSupportedException>(() =>
                    connection.Average<CompleteTable>(e => e.ColumnSmallInt, (object)null, hints: "NOLOCK"));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestOracleConnectionAverageAsyncWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.AverageAsync<CompleteTable>(e => e.ColumnSmallInt,
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Average(e => e.ColumnSmallInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public async Task TestOracleConnectionAverageAsyncWithoutExpressionWithAutomaticConversion()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
                try
                {
                    // Act
                    var result = await connection.AverageAsync<CompleteTable>(e => e.ColumnSmallInt,
                        (object)null);

                    // Assert
                    Assert.AreEqual(tables.Average(e => e.ColumnSmallInt), Convert.ToDouble(result));
                }
                finally
                {
                    GlobalConfiguration.Options.ConversionType = ConversionType.Default;
                }
            }
        }

        [TestMethod]
        public async Task TestOracleConnectionAverageAsyncViaExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();
            var ids = new[] { tables.First().Id, tables.Last().Id };

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.AverageAsync<CompleteTable>(e => e.ColumnSmallInt,
                    e => ids.Contains(e.Id));

                // Assert
                Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Average(e => e.ColumnSmallInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public async Task TestOracleConnectionAverageAsyncViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.AverageAsync<CompleteTable>(e => e.ColumnSmallInt,
                    new { tables.First().Id });

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Average(e => e.ColumnSmallInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public async Task TestOracleConnectionAverageAsyncViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.AverageAsync<CompleteTable>(e => e.ColumnSmallInt,
                    new QueryField("Id", tables.First().Id));

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Average(e => e.ColumnSmallInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public async Task TestOracleConnectionAverageAsyncViaQueryFields()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.AverageAsync<CompleteTable>(e => e.ColumnSmallInt,
                    queryFields);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Average(e => e.ColumnSmallInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public async Task TestOracleConnectionAverageAsyncViaQueryGroup()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.AverageAsync<CompleteTable>(e => e.ColumnSmallInt,
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Average(e => e.ColumnSmallInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public async Task TestOracleConnectionAverageAsyncWithHintsThrows()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act/Assert: AreTableHintsSupported == false for Oracle - any non-null/non-whitespace
                // "hints" argument must throw, rather than silently being ignored.
                await Assert.ThrowsAsync<NotSupportedException>(() =>
                    connection.AverageAsync<CompleteTable>(e => e.ColumnSmallInt, (object)null, hints: "NOLOCK"));
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestOracleConnectionAverageViaTableNameWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Average(ClassMappedNameCache.Get<CompleteTable>(),
                    Field.Parse<CompleteTable>(e => e.ColumnSmallInt).First(),
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Average(e => e.ColumnSmallInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public void TestOracleConnectionAverageViaTableNameViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Average(ClassMappedNameCache.Get<CompleteTable>(),
                    Field.Parse<CompleteTable>(e => e.ColumnSmallInt).First(),
                    new { tables.First().Id });

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Average(e => e.ColumnSmallInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public void TestOracleConnectionAverageViaTableNameViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Average(ClassMappedNameCache.Get<CompleteTable>(),
                    Field.Parse<CompleteTable>(e => e.ColumnSmallInt).First(),
                    new QueryField("Id", tables.First().Id));

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Average(e => e.ColumnSmallInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public void TestOracleConnectionAverageViaTableNameViaQueryFields()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Average(ClassMappedNameCache.Get<CompleteTable>(),
                    Field.Parse<CompleteTable>(e => e.ColumnSmallInt).First(),
                    queryFields);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Average(e => e.ColumnSmallInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public void TestOracleConnectionAverageViaTableNameViaQueryGroup()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Average(ClassMappedNameCache.Get<CompleteTable>(),
                    Field.Parse<CompleteTable>(e => e.ColumnSmallInt).First(),
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Average(e => e.ColumnSmallInt), Convert.ToDouble(result));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestOracleConnectionAverageAsyncViaTableNameWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.AverageAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    Field.Parse<CompleteTable>(e => e.ColumnSmallInt).First(),
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Average(e => e.ColumnSmallInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public async Task TestOracleConnectionAverageAsyncViaTableNameViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.AverageAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    Field.Parse<CompleteTable>(e => e.ColumnSmallInt).First(),
                    new { tables.First().Id });

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Average(e => e.ColumnSmallInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public async Task TestOracleConnectionAverageAsyncViaTableNameViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.AverageAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    Field.Parse<CompleteTable>(e => e.ColumnSmallInt).First(),
                    new QueryField("Id", tables.First().Id));

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Average(e => e.ColumnSmallInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public async Task TestOracleConnectionAverageAsyncViaTableNameViaQueryFields()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.AverageAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    Field.Parse<CompleteTable>(e => e.ColumnSmallInt).First(),
                    queryFields);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Average(e => e.ColumnSmallInt), Convert.ToDouble(result));
            }
        }

        [TestMethod]
        public async Task TestOracleConnectionAverageAsyncViaTableNameViaQueryGroup()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.AverageAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    Field.Parse<CompleteTable>(e => e.ColumnSmallInt).First(),
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Average(e => e.ColumnSmallInt), Convert.ToDouble(result));
            }
        }

        #endregion

        #endregion
    }
}
