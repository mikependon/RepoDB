#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ahtola.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Data.Interfaces;
using RepoDb.Data.Models;
using RepoDb.Data.SqliteAhtola.IntegrationTests.Entities;
using RepoDb.Data.SqliteAhtola.IntegrationTests.Setup;
using RepoDb.Enumerations;
using RepoDb.Schema;

namespace RepoDb.Data.SqliteAhtola.IntegrationTests
{
    /// <summary>
    /// Exports the rows of a table of a database into a CSV file.
    /// </summary>
    [TestClass]
    public class ExportDataAsCsvTest
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
            Path.Combine(_directory, Guid.NewGuid().ToString("N") + ".csv");

        private static string[] Lines(string path) =>
            File.ReadAllText(path).Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

        private static string[] Columns(string path) =>
            Lines(path)[0].Split(',');

        private static string[] Names(string path,
            string column)
        {
            var position = Array.IndexOf(Columns(path), column);
            return Lines(path).Skip(1).Select(line => line.Split(',')[position]).ToArray();
        }

        private sealed class UpperCaseNameInterceptor : IDataInterceptor<CopyRow>
        {
            public CopyRow Transform(CopyRow data)
            {
                data["Name"] = ((string)data["Name"]).ToUpperInvariant();
                return data;
            }
        }

        #endregion

        #region ExportDataAsCsv

        [TestMethod]
        public void TestExportDataAsCsv()
        {
            using (var source = new SqliteConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var path = NewPath();

                // Act
                var actual = source.ExportDataAsCsv("Country", path);

                // Assert
                var expected = Helper.SourceRows("\"Country\"", "\"Id\"");
                Assert.AreEqual((long)expected.Count, actual.TotalRowCount);
                Assert.AreEqual((long)expected.Count, actual.CurrentRowCount);
                Assert.AreEqual((long)expected.Count, actual.Position);
                Assert.IsNotNull(actual.End);
                CollectionAssert.AreEqual(expected.Select(r => (string)r["Name"]).ToArray(), Names(path, "Name"));
                CollectionAssert.AreEqual(expected.Select(r => Convert.ToInt64(r["Id"]).ToString()).ToArray(), Names(path, "Id"));
            }
        }

        [TestMethod]
        public void TestExportDataAsCsvCallsTheCallbackAfterTheRowsAreCountedAfterEveryRowAndWhenItEnds()
        {
            using (var source = new SqliteConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var seen = new List<(long Position, long Rows, long Total, bool Ended)>();
                var total = Helper.SourceCount("\"Country\"");

                // Act
                source.ExportDataAsCsv("Country", NewPath(), progressCallback: p => seen.Add((p.Position, p.CurrentRowCount, p.TotalRowCount, p.End.HasValue)));

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
        public void TestExportDataAsCsvCountsAndExportsOnlyTheRowsThatTheFilterSelects()
        {
            using (var source = new SqliteConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var path = NewPath();
                var expected = Helper.SourceRows("\"Country\"", "\"Id\"", "\"Id\" > 1");

                // Act
                var actual = source.ExportDataAsCsv("Country", path, where: new QueryGroup(new QueryField("Id", Operation.GreaterThan, 1)));

                // Assert
                Assert.AreEqual((long)expected.Count, actual.TotalRowCount);
                CollectionAssert.AreEqual(expected.Select(r => (string)r["Name"]).ToArray(), Names(path, "Name"));
            }
        }

        [TestMethod]
        public void TestExportDataAsCsvWritesOnlyTheMappedColumnsByTheirDestinations()
        {
            using (var source = new SqliteConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var path = NewPath();
                var mappings = new[] { new CopyColumnMapping("Name", "CountryName"), new CopyColumnMapping("Id", "Key") };

                // Act
                source.ExportDataAsCsv("Country", path, mappings);

                // Assert
                CollectionAssert.AreEqual(new[] { "CountryName", "Key" }, Columns(path));
                var expected = Helper.SourceRows("\"Country\"", "\"Id\"");
                CollectionAssert.AreEqual(expected.Select(r => (string)r["Name"]).ToArray(), Names(path, "CountryName"));
            }
        }

        [TestMethod]
        public void TestExportDataAsCsvWritesTheRowsThatTheInterceptorsGive()
        {
            using (var source = new SqliteConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var path = NewPath();

                // Act
                source.ExportDataAsCsv("Country", path, dataInterceptors: new List<IDataInterceptor<CopyRow>> { new UpperCaseNameInterceptor() });

                // Assert
                var expected = Helper.SourceRows("\"Country\"", "\"Id\"");
                CollectionAssert.AreEqual(expected.Select(r => ((string)r["Name"]).ToUpperInvariant()).ToArray(), Names(path, "Name"));
            }
        }

        [TestMethod]
        public void TestExportDataAsCsvCanBeLoadedBackByImportDataFromCsv()
        {
            using (var source = new SqliteConnection(Database.ConnectionStringForSource))
            using (var target = new SqliteConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                var path = NewPath();
                Helper.CreateTargetTables("Country");

                // Act
                source.ExportDataAsCsv("Country", path);
                var actual = target.ImportDataFromCsv("Country", path);

                // Assert
                Assert.AreEqual((long)Helper.SourceCount("\"Country\""), actual.LoadedRowCount);
                Helper.AssertRowsEqual(Helper.SourceRows("\"Country\"", "\"Id\""), Helper.TargetRows("\"Country\"", "\"Id\""));
            }
        }

        [TestMethod]
        public void TestExportDataAsCsvCanBeLoadedBackByImportDataFromCsvForEveryTypeOfTheSample()
        {
            using (var source = new SqliteConnection(Database.ConnectionStringForSource))
            using (var target = new SqliteConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                var path = NewPath();
                Helper.CreateTargetTables("Sample");

                // Act
                source.ExportDataAsCsv("Sample", path);
                target.ImportDataFromCsv("Sample", path);

                // Assert
                Helper.AssertRowsEqual(Helper.SourceRows("\"Sample\"", "\"Id\""), Helper.TargetRows("\"Sample\"", "\"Id\""));
            }
        }

        [TestMethod]
        public void TestExportDataAsCsvCanBeLoadedBackByImportDataFromCsvForATableWithNamesThatNeedEncoding()
        {
            using (var source = new SqliteConnection(Database.ConnectionStringForSource))
            using (var target = new SqliteConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                var path = NewPath();
                Helper.CreateTargetTables("Order Details");

                // Act
                source.ExportDataAsCsv("Order Details", path);
                target.ImportDataFromCsv("Order Details", path);

                // Assert
                Helper.AssertRowsEqual(Helper.SourceRows("\"Order Details\"", "\"Id\""), Helper.TargetRows("\"Order Details\"", "\"Id\""));
            }
        }

        [TestMethod]
        public async Task TestExportDataAsCsvAsync()
        {
            using (var source = new SqliteConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var path = NewPath();
                var seen = new List<long>();

                // Act
                var actual = await source.ExportDataAsCsvAsync("Country", path, where: new QueryGroup(new QueryField("Id", Operation.LessThan, 3)), progressCallback: p => seen.Add(p.Position));

                // Assert
                var expected = Helper.SourceRows("\"Country\"", "\"Id\"", "\"Id\" < 3");
                Assert.AreEqual((long)expected.Count, actual.TotalRowCount);
                Assert.AreEqual(expected.Count, Names(path, "Name").Length);
                Assert.AreEqual(expected.Count + 2, seen.Count);
            }
        }

        [TestMethod]
        public void TestExportDataAsCsvOfAnEntityExportsTheRowsThatTheExpressionSelects()
        {
            using (var source = new SqliteConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var path = NewPath();
                var expected = Helper.SourceRows("\"Person\"", "\"Id\"", "\"Id\" > 1");

                // Act
                var actual = source.ExportDataAsCsv<Person>(path, where: p => p.Id > 1);

                // Assert
                Assert.AreEqual((long)expected.Count, actual.TotalRowCount);
                CollectionAssert.AreEqual(expected.Select(r => (string)r["Name"]).ToArray(), Names(path, "Name"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnExportDataAsCsvIfTheTableDoesNotExistAndLeaveNoFile()
        {
            using (var source = new SqliteConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var path = NewPath();

                // Act
                var failed = false;
                try
                {
                    source.ExportDataAsCsv("NotATable", path);
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

        [TestMethod]
        public void TestExportDataAsCsvSeparatesTheFieldsBySeparatorAndCanBeLoadedBack()
        {
            using (var source = new SqliteConnection(Database.ConnectionStringForSource))
            using (var target = new SqliteConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                var path = NewPath();
                Helper.CreateTargetTables("Country");

                // Act
                source.ExportDataAsCsv("Country", path, separator: ';');
                target.ImportDataFromCsv("Country", path, separator: ';');

                // Assert
                Assert.AreEqual("Id;Name", File.ReadAllText(path).Split(new[] { "\r\n" }, StringSplitOptions.None)[0]);
                Helper.AssertRowsEqual(Helper.SourceRows("\"Country\"", "\"Id\""), Helper.TargetRows("\"Country\"", "\"Id\""));
            }
        }

        #endregion
    }
}
