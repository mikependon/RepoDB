#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sap.Data.Hana;
using RepoDb.Extensions;
using RepoDb.SapHana.IntegrationTests.Models;
using RepoDb.SapHana.IntegrationTests.Setup;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.SapHana.IntegrationTests.Operations
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
        [Ignore("ExecuteQueryMultiple executes caller-supplied raw SQL text containing multiple " +
            "statements as a single command, reading each result set via NextResult() - HANA's ADO.NET " +
            "client rejects any command text with more than one statement outright, and there's no way " +
            "to split it into separate round-trips since the combined text is opaque to RepoDb.Core.")]
        public void TestHanaConnectionExecuteQueryMultiple()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
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
        [Ignore("See the remark on TestHanaConnectionExecuteQueryMultiple.")]
        public void TestHanaConnectionExecuteQueryMultipleWithParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                using (var extractor = connection.ExecuteQueryMultiple(@"SELECT * FROM ""CompleteTable"" WHERE Id = :Id1;
                    SELECT * FROM ""CompleteTable"" WHERE Id = :Id2;",
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
        [Ignore("See the remark on TestHanaConnectionExecuteQueryMultiple.")]
        public void TestHanaConnectionExecuteQueryMultipleWithSharedParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                using (var extractor = connection.ExecuteQueryMultiple(@"SELECT * FROM ""CompleteTable"" WHERE Id = :Id;
                    SELECT * FROM ""CompleteTable"" WHERE Id = :Id;",
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
        [Ignore("See the remark on TestHanaConnectionExecuteQueryMultiple.")]
        public async Task TestHanaConnectionExecuteQueryMultipleAsync()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                using (var extractor = await connection.ExecuteQueryMultipleAsync(@"SELECT * FROM ""CompleteTable"";
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
        [Ignore("See the remark on TestHanaConnectionExecuteQueryMultiple.")]
        public async Task TestHanaConnectionExecuteQueryMultipleAsyncWithParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                using (var extractor = await connection.ExecuteQueryMultipleAsync(@"SELECT * FROM ""CompleteTable"" WHERE Id = :Id1;
                    SELECT * FROM ""CompleteTable"" WHERE Id = :Id2;",
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
        [Ignore("See the remark on TestHanaConnectionExecuteQueryMultiple.")]
        public async Task TestHanaConnectionExecuteQueryMultipleAsyncWithSharedParameters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Act
                using (var extractor = await connection.ExecuteQueryMultipleAsync(@"SELECT * FROM ""CompleteTable"" WHERE Id = :Id;
                    SELECT * FROM ""CompleteTable"" WHERE Id = :Id;",
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
    }
}
