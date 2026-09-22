#region Copyright Attributions

// Copyright (c) 2019 Bradley Graigner and Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using DuckDB.NET.Data;
using RepoDb.Extensions;
using RepoDb.DuckDb.IntegrationTests.Models;
using RepoDb.DuckDb.IntegrationTests.Setup;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.DuckDb.IntegrationTests.Operations
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
        public void TestDuckDBConnectionExecuteQueryMultiple()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Act
                using (var extractor = connection.ExecuteQueryMultiple(@"SELECT * FROM ""CompleteTable"";
                    SELECT * FROM ""CompleteTable"";"))
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
        public void TestDuckDBConnectionExecuteQueryMultipleWithParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Act
                using (var extractor = connection.ExecuteQueryMultiple(@"SELECT * FROM ""CompleteTable"" WHERE Id = $Id1;
                    SELECT * FROM ""CompleteTable"" WHERE Id = $Id2;",
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
        public void TestDuckDBConnectionExecuteQueryMultipleWithSharedParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Act
                using (var extractor = connection.ExecuteQueryMultiple(@"SELECT * FROM ""CompleteTable"" WHERE Id = $Id;
                    SELECT * FROM ""CompleteTable"" WHERE Id = $Id;",
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
        public async Task TestDuckDBConnectionExecuteQueryMultipleAsync()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Act
                using (var extractor = await connection.ExecuteQueryMultipleAsync(@"SELECT * FROM ""CompleteTable"";
                    SELECT * FROM ""CompleteTable"";").ConfigureAwait(false))
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
        public async Task TestDuckDBConnectionExecuteQueryMultipleAsyncWithParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Act
                using (var extractor = await connection.ExecuteQueryMultipleAsync(@"SELECT * FROM ""CompleteTable"" WHERE Id = $Id1;
                    SELECT * FROM ""CompleteTable"" WHERE Id = $Id2;",
                    new
                    {
                        Id1 = tables.First().Id,
                        Id2 = tables.Last().Id
                    }).ConfigureAwait(false))
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
        public async Task TestDuckDBConnectionExecuteQueryMultipleAsyncWithSharedParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new DuckDBConnection(Database.ConnectionString))
            {
                // Act
                using (var extractor = await connection.ExecuteQueryMultipleAsync(@"SELECT * FROM ""CompleteTable"" WHERE Id = $Id;
                    SELECT * FROM ""CompleteTable"" WHERE Id = $Id;",
                    new { Id = tables.Last().Id }).ConfigureAwait(false))
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
