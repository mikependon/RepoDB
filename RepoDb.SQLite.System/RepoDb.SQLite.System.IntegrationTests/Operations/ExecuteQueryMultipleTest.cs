using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;
using RepoDb.SQLite.System.IntegrationTests.Models;
using RepoDb.SQLite.System.IntegrationTests.Setup;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.SQLite.System.IntegrationTests.Operations.SDS
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
        public void TestSqLiteConnectionExecuteQueryMultiple()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection);

                // Act
                using (var extractor = connection.ExecuteQueryMultiple(@"SELECT * FROM [SdsCompleteTable];
                    SELECT * FROM [SdsCompleteTable];"))
                {
                    var list = new List<IEnumerable<SdsCompleteTable>>();

                    // Act
                    list.Add(extractor.Extract<SdsCompleteTable>());
                    list.Add(extractor.Extract<SdsCompleteTable>());

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
        public void TestSqLiteConnectionExecuteQueryMultipleWithParameters()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection);

                // Act
                using (var extractor = connection.ExecuteQueryMultiple(@"SELECT * FROM [SdsCompleteTable] WHERE Id = @Id1;
                    SELECT * FROM [SdsCompleteTable] WHERE Id = @Id2;",
                    new
                    {
                        Id1 = tables.First().Id,
                        Id2 = tables.Last().Id
                    }))
                {
                    var list = new List<IEnumerable<SdsCompleteTable>>();

                    // Act
                    list.Add(extractor.Extract<SdsCompleteTable>());
                    list.Add(extractor.Extract<SdsCompleteTable>());

                    // Assert
                    list.ForEach(item =>
                    {
                        item.AsList().ForEach(current => Helper.AssertPropertiesEquality(current, tables.First(e => e.Id == current.Id)));
                    });
                }
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionExecuteQueryMultipleWithSharedParameters()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection);

                // Act
                using (var extractor = connection.ExecuteQueryMultiple(@"SELECT * FROM [SdsCompleteTable] WHERE Id = @Id;
                    SELECT * FROM [SdsCompleteTable] WHERE Id = @Id;",
                    new { Id = tables.Last().Id }))
                {
                    var list = new List<IEnumerable<SdsCompleteTable>>();

                    // Act
                    list.Add(extractor.Extract<SdsCompleteTable>());
                    list.Add(extractor.Extract<SdsCompleteTable>());

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
        public async Task TestSqLiteConnectionExecuteQueryMultipleAsync()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection);

                // Act
                using (var extractor = await connection.ExecuteQueryMultipleAsync(@"SELECT * FROM [SdsCompleteTable];
                    SELECT * FROM [SdsCompleteTable];"))
                {
                    var list = new List<IEnumerable<SdsCompleteTable>>();

                    // Act
                    list.Add(extractor.Extract<SdsCompleteTable>());
                    list.Add(extractor.Extract<SdsCompleteTable>());

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
        public async Task TestSqLiteConnectionExecuteQueryMultipleAsyncWithParameters()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection);

                // Act
                using (var extractor = await connection.ExecuteQueryMultipleAsync(@"SELECT * FROM [SdsCompleteTable] WHERE Id = @Id1;
                    SELECT * FROM [SdsCompleteTable] WHERE Id = @Id2;",
                    new
                    {
                        Id1 = tables.First().Id,
                        Id2 = tables.Last().Id
                    }))
                {
                    var list = new List<IEnumerable<SdsCompleteTable>>();

                    // Act
                    list.Add(extractor.Extract<SdsCompleteTable>());
                    list.Add(extractor.Extract<SdsCompleteTable>());

                    // Assert
                    list.ForEach(item =>
                    {
                        item.AsList().ForEach(current => Helper.AssertPropertiesEquality(current, tables.First(e => e.Id == current.Id)));
                    });
                }
            }
        }

        [TestMethod]
        public async Task TestSqLiteConnectionExecuteQueryMultipleAsyncWithSharedParameters()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection);

                // Act
                using (var extractor = await connection.ExecuteQueryMultipleAsync(@"SELECT * FROM [SdsCompleteTable] WHERE Id = @Id;
                    SELECT * FROM [SdsCompleteTable] WHERE Id = @Id;",
                    new { Id = tables.Last().Id }))
                {
                    var list = new List<IEnumerable<SdsCompleteTable>>();

                    // Act
                    list.Add(extractor.Extract<SdsCompleteTable>());
                    list.Add(extractor.Extract<SdsCompleteTable>());

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
