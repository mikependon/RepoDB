using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.Extensions;
using RepoDb.PostgreSql.IntegrationTests.Models;
using RepoDb.PostgreSql.IntegrationTests.Setup;
using System.Collections.Generic;
using System.Linq;

namespace RepoDb.PostgreSql.IntegrationTests.Operations
{
    [TestClass]
    public class ExecuteQueryMultipleTest
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

        #region Sync

        [TestMethod]
        public void TestPostgreSqlConnectionExecuteQueryMultiple()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                using (var extractor = connection.ExecuteQueryMultiple(@"SELECT * FROM `CompleteTable`;
                    SELECT * FROM `CompleteTable`;"))
                {
                    var list = new List<IEnumerable<CompleteTable>>();

                    // Act
                    list.Add(extractor.Extract<CompleteTable>());
                    list.Add(extractor.Extract<CompleteTable>());

                    // Assert
                    list.ForEach(item =>
                    {
                        Assert.AreEqual(tables.Count(), item.Count());
                        tables.AsList().ForEach(table => Helper.AssertPropertiesEquality(table, item.First(e => e.Id == table.Id)));
                    });
                }
            }
        }

        [TestMethod]
        public void TestPostgreSqlConnectionExecuteQueryMultipleWithParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                using (var extractor = connection.ExecuteQueryMultiple(@"SELECT * FROM `CompleteTable` WHERE Id = @Id1;
                    SELECT * FROM `CompleteTable` WHERE Id = @Id2;",
                    new
                    {
                        Id1 = tables.First().Id,
                        Id2 = tables.Last().Id
                    }))
                {
                    var list = new List<IEnumerable<CompleteTable>>();

                    // Act
                    list.Add(extractor.Extract<CompleteTable>());
                    list.Add(extractor.Extract<CompleteTable>());

                    // Assert
                    list.ForEach(item =>
                    {
                        item.AsList().ForEach(current => Helper.AssertPropertiesEquality(current, tables.First(e => e.Id == current.Id)));
                    });
                }
            }
        }

        [TestMethod]
        public void TestPostgreSqlConnectionExecuteQueryMultipleWithSharedParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                using (var extractor = connection.ExecuteQueryMultiple(@"SELECT * FROM `CompleteTable` WHERE Id = @Id;
                    SELECT * FROM `CompleteTable` WHERE Id = @Id;",
                    new { Id = tables.Last().Id }))
                {
                    var list = new List<IEnumerable<CompleteTable>>();

                    // Act
                    list.Add(extractor.Extract<CompleteTable>());
                    list.Add(extractor.Extract<CompleteTable>());

                    // Assert
                    list.ForEach(item =>
                    {
                        item.AsList().ForEach(current => Helper.AssertPropertiesEquality(current, tables.First(e => e.Id == current.Id)));
                    });
                }
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestPostgreSqlConnectionExecuteQueryMultipleAsync()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                using (var extractor = connection.ExecuteQueryMultipleAsync(@"SELECT * FROM `CompleteTable`;
                    SELECT * FROM `CompleteTable`;").Result)
                {
                    var list = new List<IEnumerable<CompleteTable>>();

                    // Act
                    list.Add(extractor.Extract<CompleteTable>());
                    list.Add(extractor.Extract<CompleteTable>());

                    // Assert
                    list.ForEach(item =>
                    {
                        Assert.AreEqual(tables.Count(), item.Count());
                        tables.AsList().ForEach(table => Helper.AssertPropertiesEquality(table, item.First(e => e.Id == table.Id)));
                    });
                }
            }
        }

        [TestMethod]
        public void TestPostgreSqlConnectionExecuteQueryMultipleAsyncWithParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                using (var extractor = connection.ExecuteQueryMultipleAsync(@"SELECT * FROM `CompleteTable` WHERE Id = @Id1;
                    SELECT * FROM `CompleteTable` WHERE Id = @Id2;",
                    new
                    {
                        Id1 = tables.First().Id,
                        Id2 = tables.Last().Id
                    }).Result)
                {
                    var list = new List<IEnumerable<CompleteTable>>();

                    // Act
                    list.Add(extractor.Extract<CompleteTable>());
                    list.Add(extractor.Extract<CompleteTable>());

                    // Assert
                    list.ForEach(item =>
                    {
                        item.AsList().ForEach(current => Helper.AssertPropertiesEquality(current, tables.First(e => e.Id == current.Id)));
                    });
                }
            }
        }

        [TestMethod]
        public void TestPostgreSqlConnectionExecuteQueryMultipleAsyncWithSharedParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                using (var extractor = connection.ExecuteQueryMultipleAsync(@"SELECT * FROM `CompleteTable` WHERE Id = @Id;
                    SELECT * FROM `CompleteTable` WHERE Id = @Id;",
                    new { Id = tables.Last().Id }).Result)
                {
                    var list = new List<IEnumerable<CompleteTable>>();

                    // Act
                    list.Add(extractor.Extract<CompleteTable>());
                    list.Add(extractor.Extract<CompleteTable>());

                    // Assert
                    list.ForEach(item =>
                    {
                        item.AsList().ForEach(current => Helper.AssertPropertiesEquality(current, tables.First(e => e.Id == current.Id)));
                    });
                }
            }
        }

        #endregion
    }
}
