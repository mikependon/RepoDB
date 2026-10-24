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
using System.Xml.Linq;
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
    /// Exports the rows of a table of a database into a XML file.
    /// </summary>
    [TestClass]
    public class ExportDataAsXmlTest
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
            Path.Combine(_directory, Guid.NewGuid().ToString("N") + ".xml");

        private static XElement[] Rows(string path) =>
            XDocument.Load(path).Root.Elements("row").ToArray();

        private static string[] Names(string path,
            string column) =>
            Rows(path).Select(row => row.Element(column).Value).ToArray();

        private static string[] Columns(string path) =>
            Rows(path)[0].Elements().Select(element => element.Name.LocalName).ToArray();

        private sealed class UpperCaseNameInterceptor : IDataInterceptor<CopyRow>
        {
            public CopyRow Transform(CopyRow data)
            {
                data["Name"] = ((string)data["Name"]).ToUpperInvariant();
                return data;
            }
        }

        #endregion

        #region ExportDataAsXml

        [TestMethod]
        public void TestExportDataAsXml()
        {
            using (var source = new SqliteConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var path = NewPath();

                // Act
                var actual = source.ExportDataAsXml("Country", path);

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
        public void TestExportDataAsXmlCallsTheCallbackAfterTheRowsAreCountedAfterEveryRowAndWhenItEnds()
        {
            using (var source = new SqliteConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var seen = new List<(long Position, long Rows, long Total, bool Ended)>();
                var total = Helper.SourceCount("\"Country\"");

                // Act
                source.ExportDataAsXml("Country", NewPath(), progressCallback: p => seen.Add((p.Position, p.CurrentRowCount, p.TotalRowCount, p.End.HasValue)));

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
        public void TestExportDataAsXmlCountsAndExportsOnlyTheRowsThatTheFilterSelects()
        {
            using (var source = new SqliteConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var path = NewPath();
                var expected = Helper.SourceRows("\"Country\"", "\"Id\"", "\"Id\" > 1");

                // Act
                var actual = source.ExportDataAsXml("Country", path, where: new QueryGroup(new QueryField("Id", Operation.GreaterThan, 1)));

                // Assert
                Assert.AreEqual((long)expected.Count, actual.TotalRowCount);
                CollectionAssert.AreEqual(expected.Select(r => (string)r["Name"]).ToArray(), Names(path, "Name"));
            }
        }

        [TestMethod]
        public void TestExportDataAsXmlWritesOnlyTheMappedColumnsByTheirDestinations()
        {
            using (var source = new SqliteConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var path = NewPath();
                var mappings = new[] { new CopyColumnMapping("Name", "CountryName"), new CopyColumnMapping("Id", "Key") };

                // Act
                source.ExportDataAsXml("Country", path, mappings);

                // Assert
                CollectionAssert.AreEqual(new[] { "CountryName", "Key" }, Columns(path));
                var expected = Helper.SourceRows("\"Country\"", "\"Id\"");
                CollectionAssert.AreEqual(expected.Select(r => (string)r["Name"]).ToArray(), Names(path, "CountryName"));
            }
        }

        [TestMethod]
        public void TestExportDataAsXmlWritesTheRowsThatTheInterceptorsGive()
        {
            using (var source = new SqliteConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var path = NewPath();

                // Act
                source.ExportDataAsXml("Country", path, dataInterceptors: new List<IDataInterceptor<CopyRow>> { new UpperCaseNameInterceptor() });

                // Assert
                var expected = Helper.SourceRows("\"Country\"", "\"Id\"");
                CollectionAssert.AreEqual(expected.Select(r => ((string)r["Name"]).ToUpperInvariant()).ToArray(), Names(path, "Name"));
            }
        }

        [TestMethod]
        public void TestExportDataAsXmlCanBeLoadedBackByImportDataFromXml()
        {
            using (var source = new SqliteConnection(Database.ConnectionStringForSource))
            using (var target = new SqliteConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                var path = NewPath();
                Helper.CreateTargetTables("Country");

                // Act
                source.ExportDataAsXml("Country", path);
                var actual = target.ImportDataFromXml("Country", path);

                // Assert
                Assert.AreEqual((long)Helper.SourceCount("\"Country\""), actual.LoadedRowCount);
                Helper.AssertRowsEqual(Helper.SourceRows("\"Country\"", "\"Id\""), Helper.TargetRows("\"Country\"", "\"Id\""));
            }
        }

        [TestMethod]
        public void TestExportDataAsXmlCanBeLoadedBackByImportDataFromXmlForEveryTypeOfTheSample()
        {
            using (var source = new SqliteConnection(Database.ConnectionStringForSource))
            using (var target = new SqliteConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                var path = NewPath();
                Helper.CreateTargetTables("Sample");

                // Act
                source.ExportDataAsXml("Sample", path);
                target.ImportDataFromXml("Sample", path);

                // Assert
                Helper.AssertRowsEqual(Helper.SourceRows("\"Sample\"", "\"Id\""), Helper.TargetRows("\"Sample\"", "\"Id\""));
            }
        }

        [TestMethod]
        public void TestExportDataAsXmlCanBeLoadedBackByImportDataFromXmlForATableWithNamesThatNeedEncoding()
        {
            using (var source = new SqliteConnection(Database.ConnectionStringForSource))
            using (var target = new SqliteConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                var path = NewPath();
                Helper.CreateTargetTables("Order Details");

                // Act
                source.ExportDataAsXml("Order Details", path);
                target.ImportDataFromXml("Order Details", path);

                // Assert
                Helper.AssertRowsEqual(Helper.SourceRows("\"Order Details\"", "\"Id\""), Helper.TargetRows("\"Order Details\"", "\"Id\""));
            }
        }

        [TestMethod]
        public async Task TestExportDataAsXmlAsync()
        {
            using (var source = new SqliteConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var path = NewPath();
                var seen = new List<long>();

                // Act
                var actual = await source.ExportDataAsXmlAsync("Country", path, where: new QueryGroup(new QueryField("Id", Operation.LessThan, 3)), progressCallback: p => seen.Add(p.Position));

                // Assert
                var expected = Helper.SourceRows("\"Country\"", "\"Id\"", "\"Id\" < 3");
                Assert.AreEqual((long)expected.Count, actual.TotalRowCount);
                Assert.AreEqual(expected.Count, Names(path, "Name").Length);
                Assert.AreEqual(expected.Count + 2, seen.Count);
            }
        }

        [TestMethod]
        public void TestExportDataAsXmlOfAnEntityExportsTheRowsThatTheExpressionSelects()
        {
            using (var source = new SqliteConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var path = NewPath();
                var expected = Helper.SourceRows("\"Person\"", "\"Id\"", "\"Id\" > 1");

                // Act
                var actual = source.ExportDataAsXml<Person>(path, where: p => p.Id > 1);

                // Assert
                Assert.AreEqual((long)expected.Count, actual.TotalRowCount);
                CollectionAssert.AreEqual(expected.Select(r => (string)r["Name"]).ToArray(), Names(path, "Name"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnExportDataAsXmlIfTheTableDoesNotExistAndLeaveNoFile()
        {
            using (var source = new SqliteConnection(Database.ConnectionStringForSource))
            {
                // Setup
                var path = NewPath();

                // Act
                var failed = false;
                try
                {
                    source.ExportDataAsXml("NotATable", path);
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
