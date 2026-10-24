#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using DuckDB.NET.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Data.Interfaces;
using RepoDb.Data.Models;
using RepoDb.Data.DuckDb.IntegrationTests.Setup;
using RepoDb.Enumerations;
using RepoDb.Data.DuckDb.IntegrationTests.Entities;
using RepoDb.Schema;

namespace RepoDb.Data.DuckDb.IntegrationTests
{
    /// <summary>
    /// Exports the rows of a table of a database into a JSON file.
    /// </summary>
    [TestClass]
    public class ExportDataAsJsonTest
    {
        #region Private Variables

        private string _directory;

        #endregion

        #region Initialize

        [TestInitialize]
        public void Initialize()
        {
            Database.Initialize();
            Cleanup();
            _directory = Path.Combine(Path.GetTempPath(), "repodb_export_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_directory);
        }

        [TestCleanup]
        public void Cleanup()
        {
            SchemaReaderMapper.Clear();
            Database.Cleanup();
            if (_directory != null && Directory.Exists(_directory))
            {
                Directory.Delete(_directory, true);
            }
        }

        #endregion

        #region Helpers

        private string NewPath() =>
            Path.Combine(_directory, Guid.NewGuid().ToString("N") + ".json");

        private static JsonElement[] Read(string path)
        {
            using (var document = JsonDocument.Parse(File.ReadAllText(path)))
            {
                return document.RootElement.EnumerateArray().Select(row => row.Clone()).ToArray();
            }
        }

        private sealed class UpperCaseNameInterceptor : IDataInterceptor<Row>
        {
            public Row Transform(Row data)
            {
                data["Name"] = ((string)data["Name"]).ToUpperInvariant();
                return data;
            }
        }

        private sealed class AddColumnInterceptor : IDataInterceptor<Row>
        {
            public Row Transform(Row data) =>
                data.Add("Label", typeof(string), "#" + data["Id"]);
        }

        #endregion

        #region ExportDataAsJson

        [TestMethod]
        public void TestExportDataAsJson()
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var path = NewPath();

                // Act
                var actual = source.ExportDataAsJson("Country", path);

                // Assert
                var expected = Helper.SourceRows("\"Country\"", "\"Id\"");
                var rows = Read(path);
                Assert.AreEqual(expected.Count, rows.Length);
                Assert.AreEqual((long)expected.Count, actual.TotalRowCount);
                Assert.AreEqual((long)expected.Count, actual.CurrentRowCount);
                Assert.AreEqual((long)expected.Count, actual.Position);
                Assert.IsNotNull(actual.End);
                CollectionAssert.AreEqual(expected.Select(r => Convert.ToInt64(r["Id"])).ToArray(), rows.Select(r => r.GetProperty("Id").GetInt64()).ToArray());
                CollectionAssert.AreEqual(expected.Select(r => (string)r["Name"]).ToArray(), rows.Select(r => r.GetProperty("Name").GetString()).ToArray());
            }
        }

        [TestMethod]
        public void TestExportDataAsJsonCallsTheCallbackAfterTheRowsAreCountedAfterEveryRowAndWhenItEnds()
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var seen = new List<(long Position, long Rows, long Total, bool Ended)>();
                var total = Helper.SourceCount("\"Country\"");

                // Act
                source.ExportDataAsJson("Country", NewPath(), progressCallback: p => seen.Add((p.Position, p.CurrentRowCount, p.TotalRowCount, p.End.HasValue)));

                // Assert
                Assert.AreEqual(total + 2, seen.Count);
                Assert.AreEqual((0L, 0L, total, false), seen[0]);
                for (var i = 1; i <= total; i++)
                {
                    Assert.AreEqual((i, i, total, false), seen[i]);
                }
                Assert.AreEqual((total, total, total, true), seen[seen.Count - 1]);
            }
        }

        [TestMethod]
        public void TestExportDataAsJsonCountsAndExportsOnlyTheRowsThatTheFilterSelects()
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var path = NewPath();
                var where = new QueryGroup(new QueryField("Id", Operation.GreaterThan, 1));
                var expected = Helper.SourceRows("\"Country\"", "\"Id\"", "\"Id\" > 1");
                var totals = new List<long>();

                // Act
                var actual = source.ExportDataAsJson("Country", path, where: where, progressCallback: p => totals.Add(p.TotalRowCount));

                // Assert
                Assert.AreEqual(expected.Count, Read(path).Length);
                Assert.AreEqual((long)expected.Count, actual.TotalRowCount);
                Assert.IsTrue(totals.All(total => total == expected.Count));
                CollectionAssert.AreEqual(expected.Select(r => Convert.ToInt64(r["Id"])).ToArray(), Read(path).Select(r => r.GetProperty("Id").GetInt64()).ToArray());
            }
        }

        [TestMethod]
        public void TestExportDataAsJsonWritesOnlyTheMappedColumnsByTheirDestinations()
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var path = NewPath();
                var mappings = new[] { new ColumnMapping("Name", "CountryName"), new ColumnMapping("Id", "Key") };

                // Act
                source.ExportDataAsJson("Country", path, mappings);

                // Assert
                var rows = Read(path);
                CollectionAssert.AreEqual(new[] { "CountryName", "Key" }, rows[0].EnumerateObject().Select(p => p.Name).ToArray());
                var expected = Helper.SourceRows("\"Country\"", "\"Id\"");
                CollectionAssert.AreEquivalent(expected.Select(r => (string)r["Name"]).ToArray(), rows.Select(r => r.GetProperty("CountryName").GetString()).ToArray());
            }
        }

        [TestMethod]
        public void TestExportDataAsJsonWritesTheRowsThatTheInterceptorsGive()
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var path = NewPath();
                var interceptors = new List<IDataInterceptor<Row>> { new UpperCaseNameInterceptor(), new AddColumnInterceptor() };

                // Act
                source.ExportDataAsJson("Country", path, dataInterceptors: interceptors);

                // Assert
                var expected = Helper.SourceRows("\"Country\"", "\"Id\"");
                var rows = Read(path);
                CollectionAssert.AreEqual(expected.Select(r => ((string)r["Name"]).ToUpperInvariant()).ToArray(), rows.Select(r => r.GetProperty("Name").GetString()).ToArray());
                CollectionAssert.AreEqual(expected.Select(r => "#" + Convert.ToInt64(r["Id"])).ToArray(), rows.Select(r => r.GetProperty("Label").GetString()).ToArray());
            }
        }

        [TestMethod]
        public void TestExportDataAsJsonCanBeLoadedBackByImportDataFromJson()
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                var path = NewPath();
                Helper.CreateTargetTables("Country");

                // Act
                source.ExportDataAsJson("Country", path);
                var actual = target.ImportDataFromJson("Country", path);

                // Assert
                Assert.AreEqual((long)Helper.SourceCount("\"Country\""), actual.LoadedRowCount);
                Helper.AssertRowsEqual(Helper.SourceRows("\"Country\"", "\"Id\""), Helper.TargetRows("\"Country\"", "\"Id\""));
            }
        }

        [TestMethod]
        public void TestExportDataAsJsonCanBeLoadedBackByImportDataFromJsonForEveryTypeOfTheSample()
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                var path = NewPath();
                Helper.CreateTargetTables("Sample");

                // Act
                source.ExportDataAsJson("Sample", path);
                target.ImportDataFromJson("Sample", path);

                // Assert
                Helper.AssertRowsEqual(Helper.SourceRows("\"Sample\"", "\"Id\""), Helper.TargetRows("\"Sample\"", "\"Id\""));
            }
        }

        [TestMethod]
        public async Task TestExportDataAsJsonAsync()
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var path = NewPath();
                var seen = new List<long>();

                // Act
                var actual = await source.ExportDataAsJsonAsync("Country", path, where: new QueryGroup(new QueryField("Id", Operation.LessThan, 3)), progressCallback: p => seen.Add(p.Position));

                // Assert
                var expected = Helper.SourceRows("\"Country\"", "\"Id\"", "\"Id\" < 3");
                Assert.AreEqual(expected.Count, Read(path).Length);
                Assert.AreEqual((long)expected.Count, actual.TotalRowCount);
                Assert.AreEqual(expected.Count + 2, seen.Count);
            }
        }

        [TestMethod]
        public void TestExportDataAsJsonOfAnEntityExportsTheRowsThatTheExpressionSelects()
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var path = NewPath();
                var expected = Helper.SourceRows("\"Person\"", "\"Id\"", "\"Id\" > 1");

                // Act
                var actual = source.ExportDataAsJson<Person>(path, where: p => p.Id > 1);

                // Assert
                var rows = Read(path);
                Assert.AreEqual((long)expected.Count, actual.TotalRowCount);
                CollectionAssert.AreEqual(expected.Select(r => Convert.ToInt64(r["Id"])).ToArray(), rows.Select(r => r.GetProperty("Id").GetInt64()).ToArray());
                CollectionAssert.AreEqual(expected.Select(r => (string)r["Name"]).ToArray(), rows.Select(r => r.GetProperty("Name").GetString()).ToArray());
            }
        }

        [TestMethod]
        public async Task TestExportDataAsJsonAsyncOfAnEntityWithATableNameExportsTheRowsThatTheExpressionSelects()
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var path = NewPath();
                var expected = Helper.SourceRows("\"Person\"", "\"Id\"", "\"Id\" = 2");

                // Act
                var actual = await source.ExportDataAsJsonAsync<Person>("Person", path, where: p => p.Id == 2);

                // Assert
                Assert.AreEqual((long)expected.Count, actual.TotalRowCount);
                Assert.AreEqual(expected.Count, Read(path).Length);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnExportDataAsJsonIfTheTableDoesNotExistAndLeaveNoFile()
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var path = NewPath();

                // Act
                var failed = false;
                try
                {
                    source.ExportDataAsJson("NotATable", path);
                }
                catch (Exception)
                {
                    failed = true;
                }

                // Assert
                Assert.IsTrue(failed);
                Assert.IsFalse(File.Exists(path));
                Assert.AreEqual(0, Directory.GetFiles(_directory).Length);
            }
        }

        #endregion
    }
}
