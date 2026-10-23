#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ahtola.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Data.Enumerations;
using RepoDb.Data.Models;
using RepoDb.Data.SqliteAhtola.IntegrationTests.Setup;
using RepoDb.Schema;

namespace RepoDb.Data.SqliteAhtola.IntegrationTests
{
    /// <summary>
    /// Loads the rows of a JSON file into a table of a database.
    /// </summary>
    [TestClass]
    public class LoadDataFromJsonTest
    {
        #region Private Variables

        /// <summary>
        /// The JSON files that the test has written (they are deleted when the test ends).
        /// </summary>
        private readonly List<string> _files = new List<string>();

        #endregion

        #region Initialize

        [TestInitialize]
        public void Initialize()
        {
            Database.Initialize();
            Cleanup();
            Helper.ExecuteOnTarget(@"CREATE TABLE ""LoadItem"" (""Id"" INTEGER NOT NULL PRIMARY KEY, ""Name"" VARCHAR(100) NULL, ""Amount"" DECIMAL(10,2) NULL, ""IsActive"" BOOLEAN NULL, ""Born"" DATETIME NULL, ""Ref"" UNIQUEIDENTIFIER NULL)");
        }

        [TestCleanup]
        public void Cleanup()
        {
            SchemaReaderMapper.Clear();
            Database.Cleanup();
            foreach (var file in _files.Where(File.Exists))
            {
                File.Delete(file);
            }
            _files.Clear();
        }

        #endregion

        #region Helpers

        private string Write(string text)
        {
            var path = Path.Combine(Path.GetTempPath(), "repodb_load_" + Guid.NewGuid().ToString("N") + ".json");
            File.WriteAllText(path, text, new UTF8Encoding(false));
            _files.Add(path);
            return path;
        }

        private static string Row(int id) =>
            "{\"Id\":" + id + ",\"Name\":\"Name " + id + "\",\"Amount\":" + id + ".25,\"IsActive\":" + (id % 2 == 0 ? "true" : "false") + ",\"Born\":\"2026-01-02T03:04:05Z\",\"Ref\":\"" + new Guid(id, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10) + "\"}";

        private string Array(int rows) =>
            Write("[" + string.Join(",", Enumerable.Range(1, rows).Select(Row)) + "]");

        private static List<Dictionary<string, object>> Rows() =>
            Helper.TargetRows("\"LoadItem\"", "\"Id\"");

        #endregion

        #region LoadDataFromJson

        [TestMethod]
        public void TestLoadDataFromJson()
        {
            using (var target = new SqliteConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                var path = Array(5);

                // Act
                var actual = target.LoadDataFromJson("LoadItem", path);

                // Assert
                Assert.AreEqual(5L, actual.TotalRowCount);
                Assert.AreEqual(5L, actual.LoadedRowCount);
                Assert.IsTrue(actual.BatchNumber >= 1);
                Assert.AreEqual(DateTimeKind.Utc, actual.Start.Kind);
                var rows = Rows();
                CollectionAssert.AreEqual(new long[] { 1, 2, 3, 4, 5 }, rows.Select(r => Convert.ToInt64(r["Id"])).ToArray());
                CollectionAssert.AreEqual(new[] { "Name 1", "Name 2", "Name 3", "Name 4", "Name 5" }, rows.Select(r => (string)r["Name"]).ToArray());
            }
        }

        [TestMethod]
        public void TestLoadDataFromJsonLoadsTheValuesOfTheColumnsWithTheirTypes()
        {
            using (var target = new SqliteConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                var path = Array(2);

                // Act
                target.LoadDataFromJson("LoadItem", path);

                // Assert
                var rows = Rows();
                Assert.AreEqual(1.25m, Convert.ToDecimal(rows[0]["Amount"]));
                Assert.AreEqual(2.25m, Convert.ToDecimal(rows[1]["Amount"]));
                Assert.IsFalse(Convert.ToBoolean(rows[0]["IsActive"]));
                Assert.IsTrue(Convert.ToBoolean(rows[1]["IsActive"]));
                var born = Convert.ToDateTime(rows[0]["Born"]);
                Assert.AreEqual(new DateTime(2026, 1, 2), born.Date);
                Assert.AreEqual(new Guid(1, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10), Guid.Parse(Convert.ToString(rows[0]["Ref"])));
            }
        }

        [TestMethod]
        public void TestLoadDataFromJsonLoadsNullValuesAndTheValuesThatAreMissing()
        {
            using (var target = new SqliteConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                var path = Write("[{\"Id\":1,\"Name\":null,\"Amount\":null},{\"Id\":2,\"Name\":\"Two\"}]");

                // Act
                var actual = target.LoadDataFromJson("LoadItem", path);

                // Assert
                Assert.AreEqual(2L, actual.LoadedRowCount);
                var rows = Rows();
                Assert.IsTrue(rows[0]["Name"] == null || rows[0]["Name"] is DBNull);
                Assert.IsTrue(rows[0]["Amount"] == null || rows[0]["Amount"] is DBNull);
                Assert.AreEqual("Two", rows[1]["Name"]);
                Assert.IsTrue(rows[1]["Amount"] == null || rows[1]["Amount"] is DBNull);
            }
        }

        [TestMethod]
        public void TestLoadDataFromJsonLoadsAnObjectThatHoldsTheRows()
        {
            using (var target = new SqliteConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                var path = Write("{\"source\":\"export\",\"count\":3,\"rows\":[" + string.Join(",", Enumerable.Range(1, 3).Select(Row)) + "]}");

                // Act
                var actual = target.LoadDataFromJson("LoadItem", path);

                // Assert
                Assert.AreEqual(3L, actual.TotalRowCount);
                Assert.AreEqual(3, Helper.TargetCount("\"LoadItem\""));
            }
        }

        [TestMethod]
        public void TestLoadDataFromJsonLoadsASequenceOfObjects()
        {
            using (var target = new SqliteConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                var path = Write(string.Join("\n", Enumerable.Range(1, 4).Select(Row)));

                // Act
                var actual = target.LoadDataFromJson("LoadItem", path);

                // Assert
                Assert.AreEqual(4L, actual.TotalRowCount);
                Assert.AreEqual(4, Helper.TargetCount("\"LoadItem\""));
            }
        }

        [TestMethod]
        public void TestLoadDataFromJsonLoadsAValueWithItsType()
        {
            using (var target = new SqliteConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                var path = Write("[{\"Id\":{\"type\":\"Int32\",\"value\":8},\"Name\":{\"type\":\"String\",\"value\":\"Eight\"}}]");

                // Act
                target.LoadDataFromJson("LoadItem", path);

                // Assert
                var rows = Rows();
                Assert.AreEqual(8L, Convert.ToInt64(rows[0]["Id"]));
                Assert.AreEqual("Eight", rows[0]["Name"]);
            }
        }

        [TestMethod]
        public void TestLoadDataFromJsonIgnoresTheColumnsThatTheTableDoesNotHave()
        {
            using (var target = new SqliteConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                var path = Write("[{\"Id\":1,\"Name\":\"One\",\"Unknown\":\"x\"},{\"Id\":2,\"Name\":\"Two\",\"Unknown\":\"y\"}]");

                // Act
                var actual = target.LoadDataFromJson("LoadItem", path);

                // Assert
                Assert.AreEqual(2L, actual.LoadedRowCount);
                CollectionAssert.AreEqual(new[] { "One", "Two" }, Rows().Select(r => (string)r["Name"]).ToArray());
            }
        }

        [TestMethod]
        public void TestLoadDataFromJsonAddsTheRowsToTheRowsThatTheTableHas()
        {
            using (var target = new SqliteConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget("INSERT INTO \"LoadItem\" (\"Id\", \"Name\") VALUES (100, 'Existing')");
                var path = Array(3);

                // Act
                var actual = target.LoadDataFromJson("LoadItem", path);

                // Assert
                Assert.AreEqual(3L, actual.LoadedRowCount);
                Assert.AreEqual(4, Helper.TargetCount("\"LoadItem\""));
            }
        }

        [TestMethod]
        public void TestLoadDataFromJsonOfAnEmptyArrayLoadsNothing()
        {
            using (var target = new SqliteConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                var path = Write("[]");
                var calls = 0;

                // Act
                var actual = target.LoadDataFromJson("LoadItem", path, progressCallback: _ => calls++);

                // Assert
                Assert.AreEqual(0L, actual.TotalRowCount);
                Assert.AreEqual(0L, actual.LoadedRowCount);
                Assert.AreEqual(0, actual.BatchNumber);
                Assert.AreEqual(2, calls);
                Assert.AreEqual(0, Helper.TargetCount("\"LoadItem\""));
            }
        }

        #endregion

        #region Batch And Bulk Operations

        [TestMethod]
        public void TestLoadDataFromJsonBelowTheCutoffSizeInsertsTheRowsInBatches()
        {
            using (var target = new SqliteConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                var path = Array(3000);

                // Act
                var actual = target.LoadDataFromJson("LoadItem", path, cutoffSize: 100000, batchBufferSize: 500);

                // Assert
                Assert.AreEqual(3000L, actual.TotalRowCount);
                Assert.AreEqual(3000L, actual.LoadedRowCount);
                Assert.AreEqual(6, actual.BatchNumber);
                Assert.AreEqual(3000, Helper.TargetCount("\"LoadItem\""));
            }
        }

        [TestMethod]
        public void TestLoadDataFromJsonFromTheCutoffSizeLoadsAllTheRows()
        {
            using (var target = new SqliteConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                var path = Array(3000);

                // Act
                var actual = target.LoadDataFromJson("LoadItem", path, cutoffSize: 1000, batchBufferSize: 50, bulkBufferSize: 1000);

                // Assert
                Assert.AreEqual(3000L, actual.LoadedRowCount);
                Assert.AreEqual(60, actual.BatchNumber);
                var rows = Rows();
                Assert.AreEqual(3000, rows.Count);
                Assert.AreEqual("Name 1", rows[0]["Name"]);
                Assert.AreEqual("Name 3000", rows[2999]["Name"]);
                Assert.AreEqual(3000L, Convert.ToInt64(rows[2999]["Id"]));
            }
        }

        [TestMethod]
        public void TestLoadDataFromJsonCallsTheProgressCallbackAfterTheCountAfterEveryOperationAndAtTheEnd()
        {
            using (var target = new SqliteConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                var path = Array(1200);
                var seen = new List<(int Batch, long Loaded, long Total)>();

                // Act
                var actual = target.LoadDataFromJson("LoadItem", path, cutoffSize: 100000, batchBufferSize: 500, progressCallback: p => seen.Add((p.BatchNumber, p.LoadedRowCount, p.TotalRowCount)));

                // Assert
                CollectionAssert.AreEqual(new[] { (0, 0L, 1200L), (1, 500L, 1200L), (2, 1000L, 1200L), (3, 1200L, 1200L), (3, 1200L, 1200L) }, seen);
                Assert.AreEqual(3, actual.BatchNumber);
            }
        }

        [TestMethod]
        public void TestLoadDataFromJsonKeepsTheBatchesThatSucceededIfALaterBatchFails()
        {
            using (var target = new SqliteConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                var path = Write("[" + string.Join(",", new[] { 1, 2, 3, 4, 1, 6 }.Select(Row)) + "]");
                LoadProgress progress = null;

                // Act/Assert
                Assert.Throws<SqliteException>(() => target.LoadDataFromJson("LoadItem", path, cutoffSize: 100000, batchBufferSize: 2, progressCallback: p => progress = p));
                Assert.AreEqual(4, Helper.TargetCount("\"LoadItem\""));
                Assert.AreEqual(2, progress.BatchNumber);
                Assert.AreEqual(4L, progress.LoadedRowCount);
            }
        }

        [TestMethod]
        public void TestLoadDataFromJsonWithATransactionThatIsRolledBackLoadsNothing()
        {
            using (var target = new SqliteConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                var path = Array(10);
                target.Open();

                // Act
                using (var transaction = target.BeginTransaction())
                {
                    var actual = target.LoadDataFromJson("LoadItem", path, cutoffSize: 100000, batchBufferSize: 4, transaction: transaction);
                    transaction.Rollback();

                    // Assert
                    Assert.AreEqual(10L, actual.LoadedRowCount);
                }
                Assert.AreEqual(0, Helper.TargetCount("\"LoadItem\""));
            }
        }

        #endregion

        #region Errors

        [TestMethod]
        public void ThrowExceptionOnLoadDataFromJsonIfAValueCannotBeConvertedIntoTheTypeOfItsColumn()
        {
            using (var target = new SqliteConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                var path = Write("[{\"Id\":1,\"Name\":\"One\"},{\"Id\":\"abc\",\"Name\":\"Two\"}]");

                // Act/Assert
                var exception = Assert.Throws<FormatException>(() => target.LoadDataFromJson("LoadItem", path));
                StringAssert.Contains(exception.Message, "Row 2", StringComparison.Ordinal);
                StringAssert.Contains(exception.Message, "Id", StringComparison.Ordinal);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnLoadDataFromJsonIfTheFileIsNotValidJson()
        {
            using (var target = new SqliteConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                var path = Write("[{\"Id\":1,\"Name\":\"One\"},{\"Id\":2,");

                // Act/Assert
                Assert.Throws<FormatException>(() => target.LoadDataFromJson("LoadItem", path));
                Assert.AreEqual(0, Helper.TargetCount("\"LoadItem\""));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnLoadDataFromJsonIfNoColumnOfTheFileMatchesAColumnOfTheTable()
        {
            using (var target = new SqliteConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                var path = Write("[{\"A\":1,\"B\":2}]");

                // Act/Assert
                Assert.Throws<InvalidOperationException>(() => target.LoadDataFromJson("LoadItem", path));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestLoadDataFromJsonAsync()
        {
            using (var target = new SqliteConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                var path = Array(5);

                // Act
                var actual = await target.LoadDataFromJsonAsync("LoadItem", path);

                // Assert
                Assert.AreEqual(5L, actual.TotalRowCount);
                Assert.AreEqual(5L, actual.LoadedRowCount);
                CollectionAssert.AreEqual(new long[] { 1, 2, 3, 4, 5 }, Rows().Select(r => Convert.ToInt64(r["Id"])).ToArray());
            }
        }

        [TestMethod]
        public async Task TestLoadDataFromJsonAsyncFromTheCutoffSizeLoadsAllTheRows()
        {
            using (var target = new SqliteConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                var path = Array(3000);
                var last = 0L;

                // Act
                var actual = await target.LoadDataFromJsonAsync("LoadItem", path, cutoffSize: 1000, batchBufferSize: 50, bulkBufferSize: 1000, progressCallback: p => last = p.LoadedRowCount);

                // Assert
                Assert.AreEqual(3000L, actual.LoadedRowCount);
                Assert.AreEqual(3000L, last);
                Assert.AreEqual(60, actual.BatchNumber);
                Assert.AreEqual(3000, Helper.TargetCount("\"LoadItem\""));
            }
        }

        [TestMethod]
        public async Task TestLoadDataFromJsonAsyncLoadsASequenceOfObjects()
        {
            using (var target = new SqliteConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                var path = Write(string.Join("\n", Enumerable.Range(1, 6).Select(Row)));

                // Act
                var actual = await target.LoadDataFromJsonAsync("LoadItem", path, batchBufferSize: 4);

                // Assert
                Assert.AreEqual(6L, actual.LoadedRowCount);
                Assert.AreEqual(6, Helper.TargetCount("\"LoadItem\""));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnLoadDataFromJsonAsyncIfAValueCannotBeConvertedIntoTheTypeOfItsColumn()
        {
            using (var target = new SqliteConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                var path = Write("[{\"Id\":\"abc\"}]");

                // Act/Assert
                await Assert.ThrowsAsync<FormatException>(() => target.LoadDataFromJsonAsync("LoadItem", path));
            }
        }

        #endregion

        #region Identity And Column Mapping Behaviors

        [TestMethod]
        public void TestLoadDataFromJsonKeepsTheIdentityValuesOfTheFileByDefault()
        {
            using (var target = new SqliteConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                var path = Write("[" + string.Join(",", new[] { 10, 20, 30 }.Select(Row)) + "]");

                // Act
                target.LoadDataFromJson("LoadItem", path);

                // Assert
                CollectionAssert.AreEqual(new long[] { 10, 20, 30 }, Rows().Select(r => Convert.ToInt64(r["Id"])).ToArray());
            }
        }

        [TestMethod]
        public void TestLoadDataFromJsonKeepsTheIdentityValuesOfTheFileWithTheBulkInsert()
        {
            using (var target = new SqliteConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                var path = Write("[" + string.Join(",", new[] { 10, 20, 30 }.Select(Row)) + "]");

                // Act
                target.LoadDataFromJson("LoadItem", path, cutoffSize: 1, identityBehavior: LoadIdentityBehavior.KeepIdentity);

                // Assert
                CollectionAssert.AreEqual(new long[] { 10, 20, 30 }, Rows().Select(r => Convert.ToInt64(r["Id"])).ToArray());
            }
        }

        [TestMethod]
        public void TestLoadDataFromJsonWithTheStrictBypassColumnMappingBehavior()
        {
            using (var target = new SqliteConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                var path = Array(3);

                // Act
                var actual = target.LoadDataFromJson("LoadItem", path, cutoffSize: 1, columnMappingBehavior: LoadColumnMappingBehavior.StrictBypass);

                // Assert
                Assert.AreEqual(3L, actual.LoadedRowCount);
                Assert.AreEqual(3, Helper.TargetCount("\"LoadItem\""));
            }
        }

        [TestMethod]
        public void TestLoadDataFromJsonLeavesTheIdentityValuesToTheDatabaseIfItIsNotToKeepThem()
        {
            using (var target = new SqliteConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                var path = Write("[" + string.Join(",", new[] { 10, 20, 30 }.Select(Row)) + "]");

                // Act
                var actual = target.LoadDataFromJson("LoadItem", path, identityBehavior: LoadIdentityBehavior.ReturnIdentity);

                // Assert
                Assert.AreEqual(3L, actual.LoadedRowCount);
                CollectionAssert.AreEqual(new long[] { 1, 2, 3 }, Rows().Select(r => Convert.ToInt64(r["Id"])).ToArray());
                CollectionAssert.AreEqual(new[] { "Name 10", "Name 20", "Name 30" }, Rows().Select(r => (string)r["Name"]).ToArray());
            }
        }

        #endregion
    }
}
