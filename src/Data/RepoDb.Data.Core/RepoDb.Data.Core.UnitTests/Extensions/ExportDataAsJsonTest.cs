#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Data.Core.UnitTests.CustomObjects;
using RepoDb.Data.Interfaces;
using RepoDb.Data.Models;
using RepoDb.Enumerations;

namespace RepoDb.Data.Core.UnitTests.Extensions
{
    [TestClass]
    public class ExportDataAsJsonTest
    {
        #region Helpers

        private string _directory;

        [TestInitialize]
        public void Initialize()
        {
            _directory = Path.Combine(Path.GetTempPath(), "repodb_export_json_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_directory);
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists(_directory))
            {
                Directory.Delete(_directory, true);
            }
        }

        private string NewPath() =>
            Path.Combine(_directory, Guid.NewGuid().ToString("N") + ".json");

        private static JsonElement[] Read(string path)
        {
            using (var document = JsonDocument.Parse(File.ReadAllText(path)))
            {
                return document.RootElement.EnumerateArray().Select(row => row.Clone()).ToArray();
            }
        }

        private static IList<IDataInterceptor<Row>> Interceptors(params Func<Row, Row>[] transforms) =>
            transforms.Select(transform => (IDataInterceptor<Row>)new FakeDataInterceptor(transform)).ToList();

        private static FakeDbConnection Source(int rows = 3) =>
            new FakeDbConnection { Data = FakeDbConnection.CreatePersons(rows) };

        #endregion

        #region Arguments

        [TestMethod]
        public void ThrowExceptionOnExportDataAsJsonIfTheConnectionIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => ((IDbConnection)null).ExportDataAsJson("Person", NewPath()));
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("  ")]
        public void ThrowExceptionOnExportDataAsJsonIfTheTableNameIsNullOrWhiteSpace(string tableName)
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => Source().ExportDataAsJson(tableName, NewPath()));
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("  ")]
        public void ThrowExceptionOnExportDataAsJsonIfTheFileNameIsNullOrWhiteSpace(string filename)
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => Source().ExportDataAsJson("Person", filename));
        }

        [TestMethod]
        public async Task ThrowExceptionOnExportDataAsJsonAsyncIfTheConnectionIsNull()
        {
            // Act/Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => ((IDbConnection)null).ExportDataAsJsonAsync("Person", NewPath()));
        }

        [TestMethod]
        public void ThrowExceptionOnExportDataAsJsonIfAnInterceptorIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentException>(() => Source().ExportDataAsJson("Person", NewPath(), dataInterceptors: new List<IDataInterceptor<Row>> { null }));
        }

        [TestMethod]
        public void ThrowExceptionOnExportDataAsJsonIfAMappingIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentException>(() => Source().ExportDataAsJson("Person", NewPath(), new ColumnMapping[] { null }));
        }

        [TestMethod]
        public void ThrowExceptionOnExportDataAsJsonIfAMappingHasAnEmptyName()
        {
            // Act/Assert
            Assert.Throws<ArgumentException>(() => Source().ExportDataAsJson("Person", NewPath(), new[] { new ColumnMapping("Id", " ") }));
        }

        [TestMethod]
        public void ThrowExceptionOnExportDataAsJsonIfASourceColumnOfAMappingIsNotAColumnOfTheTable()
        {
            // Setup
            var path = NewPath();

            // Act/Assert
            Assert.Throws<ArgumentException>(() => Source().ExportDataAsJson("Person", path, new[] { new ColumnMapping("Missing", "Id") }));
            Assert.IsFalse(File.Exists(path));
        }

        [TestMethod]
        public void ThrowExceptionOnExportDataAsJsonIfADestinationIsMappedMoreThanOnce()
        {
            // Act/Assert
            Assert.Throws<ArgumentException>(() => Source().ExportDataAsJson("Person", NewPath(), new[] { new ColumnMapping("Id", "Key"), new ColumnMapping("Name", "key") }));
        }

        #endregion

        #region Rows

        [TestMethod]
        public void TestExportDataAsJsonWritesTheRowsAsAnArrayOfObjects()
        {
            // Setup
            var path = NewPath();

            // Act
            Source().ExportDataAsJson("Person", path);

            // Assert
            var rows = Read(path);
            Assert.AreEqual(3, rows.Length);
            Assert.AreEqual(1, rows[0].GetProperty("Id").GetInt32());
            Assert.AreEqual("Name 1", rows[0].GetProperty("Name").GetString());
            Assert.AreEqual(new DateTime(2000, 1, 1), rows[0].GetProperty("Birthday").GetDateTime());
            Assert.AreEqual(JsonValueKind.Null, rows[1].GetProperty("Birthday").ValueKind);
            CollectionAssert.AreEqual(new[] { "Id", "Name", "Birthday" }, rows[2].EnumerateObject().Select(p => p.Name).ToArray());
        }

        [TestMethod]
        public async Task TestExportDataAsJsonAsyncWritesTheRowsAsAnArrayOfObjects()
        {
            // Setup
            var path = NewPath();

            // Act
            await Source(5).ExportDataAsJsonAsync("Person", path);

            // Assert
            var rows = Read(path);
            CollectionAssert.AreEqual(new[] { 1, 2, 3, 4, 5 }, rows.Select(row => row.GetProperty("Id").GetInt32()).ToArray());
        }

        [TestMethod]
        public void TestExportDataAsJsonOfATableWithoutRowsWritesAnEmptyArray()
        {
            // Setup
            var path = NewPath();
            var seen = new List<(long Position, long Rows, long Total, bool Ended)>();

            // Act
            var actual = Source(0).ExportDataAsJson("Person", path, progressCallback: p => seen.Add((p.Position, p.CurrentRowCount, p.TotalRowCount, p.End.HasValue)));

            // Assert
            Assert.AreEqual(0, Read(path).Length);
            CollectionAssert.AreEqual(new[] { (0L, 0L, 0L, false), (0L, 0L, 0L, true) }, seen);
            Assert.AreEqual(0L, actual.TotalRowCount);
        }

        [TestMethod]
        public void TestExportDataAsJsonReplacesAnExistingFile()
        {
            // Setup
            var path = NewPath();
            File.WriteAllText(path, "old content");

            // Act
            Source(2).ExportDataAsJson("Person", path);

            // Assert
            Assert.AreEqual(2, Read(path).Length);
            Assert.AreEqual(1, Directory.GetFiles(_directory).Length);
        }

        [TestMethod]
        public void TestExportDataAsJsonWritesTheValuesOfEveryTypeThatImportDataFromJsonReads()
        {
            // Setup
            var path = NewPath();
            var id = Guid.NewGuid();
            var table = new DataTable("Person");
            table.Columns.Add("Text", typeof(string));
            table.Columns.Add("Flag", typeof(bool));
            table.Columns.Add("Long", typeof(long));
            table.Columns.Add("Amount", typeof(decimal));
            table.Columns.Add("Real", typeof(double));
            table.Columns.Add("Unique", typeof(Guid));
            table.Columns.Add("Time", typeof(TimeSpan));
            table.Columns.Add("Moment", typeof(DateTimeOffset));
            table.Columns.Add("Bytes", typeof(byte[]));
            table.Columns.Add("Nan", typeof(double));
            table.Rows.Add("quote \\\" é 漢字 <tag>", true, long.MaxValue, 12.345m, 1.5d, id, new TimeSpan(1, 2, 3), new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero), new byte[] { 1, 2, 3 }, double.NaN);
            var source = new FakeDbConnection { Data = table };

            // Act
            source.ExportDataAsJson("Person", path);

            // Assert
            var row = Read(path).Single();
            Assert.AreEqual("quote \\\" é 漢字 <tag>", row.GetProperty("Text").GetString());
            Assert.IsTrue(row.GetProperty("Flag").GetBoolean());
            Assert.AreEqual(long.MaxValue, row.GetProperty("Long").GetInt64());
            Assert.AreEqual(12.345m, row.GetProperty("Amount").GetDecimal());
            Assert.AreEqual(1.5d, row.GetProperty("Real").GetDouble());
            Assert.AreEqual(id, row.GetProperty("Unique").GetGuid());
            Assert.AreEqual("01:02:03", row.GetProperty("Time").GetString());
            Assert.AreEqual(new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero), row.GetProperty("Moment").GetDateTimeOffset());
            CollectionAssert.AreEqual(new byte[] { 1, 2, 3 }, row.GetProperty("Bytes").GetBytesFromBase64());
            Assert.AreEqual("NaN", row.GetProperty("Nan").GetString());
            StringAssert.Contains(File.ReadAllText(path), "漢字");
        }

        [TestMethod]
        public void TestExportDataAsJsonWritesAStreamAsBase64()
        {
            // Setup
            var path = NewPath();
            var table = new DataTable("Person");
            table.Columns.Add("Content", typeof(object));
            table.Rows.Add(new MemoryStream(new byte[] { 4, 5, 6 }));
            var source = new FakeDbConnection { Data = table };

            // Act
            source.ExportDataAsJson("Person", path);

            // Assert
            CollectionAssert.AreEqual(new byte[] { 4, 5, 6 }, Read(path).Single().GetProperty("Content").GetBytesFromBase64());
        }

        [TestMethod]
        public void TestExportDataAsJsonCanBeLoadedBackByImportDataFromJson()
        {
            // Setup
            var path = NewPath();
            var destination = new FakeDbConnection();
            Source(3).ExportDataAsJson("Person", path);

            // Act
            var actual = destination.ImportDataFromJson("Person", path);

            // Assert
            var values = destination.Executions.Where(e => e.CommandText.StartsWith("INSERT INTO", StringComparison.Ordinal)).SelectMany(e => e.Parameters.Values).ToList();
            Assert.AreEqual(3L, actual.LoadedRowCount);
            CollectionAssert.IsSubsetOf(new object[] { 1, 2, 3, "Name 1", "Name 2", "Name 3", new DateTime(2000, 1, 1) }, values);
        }

        #endregion

        #region Progress

        [TestMethod]
        public void TestExportDataAsJsonCallsTheCallbackAfterTheRowsAreCountedAfterEveryRowAndWhenItEnds()
        {
            // Setup
            var path = NewPath();
            var seen = new List<(long Position, long Rows, long Total, bool Ended)>();
            var elapsed = new List<TimeSpan>();
            var instances = new List<ExportProgress>();

            // Act
            var actual = Source(3).ExportDataAsJson("Person", path, progressCallback: p =>
            {
                seen.Add((p.Position, p.CurrentRowCount, p.TotalRowCount, p.End.HasValue));
                elapsed.Add(p.Elapsed);
                instances.Add(p);
            });

            // Assert
            CollectionAssert.AreEqual(new[] { (0L, 0L, 3L, false), (1L, 1L, 3L, false), (2L, 2L, 3L, false), (3L, 3L, 3L, false), (3L, 3L, 3L, true) }, seen);
            Assert.IsTrue(instances.All(instance => ReferenceEquals(instance, actual)));
            for (var i = 1; i < elapsed.Count; i++)
            {
                Assert.IsTrue(elapsed[i] >= elapsed[i - 1]);
            }
        }

        [TestMethod]
        public void TestExportDataAsJsonSetsTheStartInUtcAndTheEndWhenItEnds()
        {
            // Setup
            var before = DateTime.UtcNow;

            // Act
            var actual = Source().ExportDataAsJson("Person", NewPath());

            // Assert
            Assert.AreEqual(DateTimeKind.Utc, actual.Start.Kind);
            Assert.IsTrue(actual.Start >= before);
            Assert.IsTrue(actual.End >= actual.Start);
            Assert.IsTrue(actual.Elapsed >= TimeSpan.Zero);
        }

        [TestMethod]
        public void TestExportDataAsJsonLeavesTheEndEmptyUntilTheLastCall()
        {
            // Setup
            var ends = new List<DateTime?>();

            // Act
            Source(2).ExportDataAsJson("Person", NewPath(), progressCallback: p => ends.Add(p.End));

            // Assert
            Assert.IsTrue(ends.Take(ends.Count - 1).All(end => end == null));
            Assert.IsNotNull(ends.Last());
        }

        [TestMethod]
        public async Task TestExportDataAsJsonAsyncCallsTheCallbackAfterTheRowsAreCountedAfterEveryRowAndWhenItEnds()
        {
            // Setup
            var seen = new List<(long Position, long Rows, long Total, bool Ended)>();

            // Act
            await Source(2).ExportDataAsJsonAsync("Person", NewPath(), progressCallback: p => seen.Add((p.Position, p.CurrentRowCount, p.TotalRowCount, p.End.HasValue)));

            // Assert
            CollectionAssert.AreEqual(new[] { (0L, 0L, 2L, false), (1L, 1L, 2L, false), (2L, 2L, 2L, false), (2L, 2L, 2L, true) }, seen);
        }

        [TestMethod]
        public void TestExportDataAsJsonWritesIntoATemporaryFileUntilTheExportHasSucceeded()
        {
            // Setup
            var path = NewPath();
            var duringTheExport = new List<(bool TargetExists, int Files)>();

            // Act
            Source(3).ExportDataAsJson("Person", path, progressCallback: p => duringTheExport.Add((File.Exists(path), Directory.GetFiles(_directory).Length)));

            // Assert
            CollectionAssert.AreEqual(new[] { (false, 0), (false, 1), (false, 1), (false, 1), (true, 1) }, duringTheExport);
            Assert.IsTrue(File.Exists(path));
            Assert.AreEqual(1, Directory.GetFiles(_directory).Length);
        }

        #endregion

        #region Select

        [TestMethod]
        public void TestExportDataAsJsonSelectsAllTheColumnsIfThereAreNoMappings()
        {
            // Setup
            var source = Source();

            // Act
            source.ExportDataAsJson("Person", NewPath());

            // Assert
            var select = source.Executions.Single(e => e.Kind == "Reader");
            StringAssert.Contains(select.CommandText, "[Id]");
            StringAssert.Contains(select.CommandText, "[Name]");
            StringAssert.Contains(select.CommandText, "[Birthday]");
            Assert.IsFalse(select.CommandText.Contains(" AS "));
        }

        [TestMethod]
        public void TestExportDataAsJsonSelectsOnlyTheMappedColumnsAliasedWithTheirDestinations()
        {
            // Setup
            var source = Source();
            var mappings = new[] { new ColumnMapping("Id", "Key"), new ColumnMapping("Name", "Title") };

            // Act
            source.ExportDataAsJson("Person", NewPath(), mappings);

            // Assert
            var select = source.Executions.Single(e => e.Kind == "Reader");
            StringAssert.Contains(select.CommandText, "[Id] AS [Key], [Name] AS [Title]");
            Assert.IsFalse(select.CommandText.Contains("[Birthday]"));
        }

        [TestMethod]
        public void TestExportDataAsJsonCountsAndSelectsTheRowsThatTheFilterSelects()
        {
            // Setup
            var source = Source();
            var where = new QueryGroup(new QueryField("Id", Operation.GreaterThan, 1));

            // Act
            source.ExportDataAsJson("Person", NewPath(), where: where);

            // Assert
            var count = source.Executions.First(e => e.Kind == "Scalar");
            var select = source.Executions.Single(e => e.Kind == "Reader");
            StringAssert.Contains(count.CommandText, "WHERE");
            StringAssert.Contains(select.CommandText, "WHERE");
            Assert.AreEqual(1, select.Parameters.Values.Single());
            Assert.IsTrue(count.Parameters.Values.Contains(1));
        }

        [TestMethod]
        public void TestExportDataAsJsonCountsTheRowsBeforeItSelectsThem()
        {
            // Setup
            var source = Source();

            // Act
            source.ExportDataAsJson("Person", NewPath());

            // Assert
            var kinds = source.Executions.Select(e => e.Kind).ToList();
            Assert.IsTrue(kinds.IndexOf("Scalar") < kinds.IndexOf("Reader"));
        }

        [TestMethod]
        public void TestExportDataAsJsonPassesTheCommandTimeoutAndTheTraceKeyToTheSelect()
        {
            // Setup
            var source = Source();

            // Act
            source.ExportDataAsJson("Person", NewPath(), commandTimeout: 77, traceKey: "MyExport");

            // Assert
            Assert.AreEqual(77, source.Executions.Single(e => e.Kind == "Reader").CommandTimeout);
        }

        [TestMethod]
        public void TestExportDataAsJsonWritesThePropertiesByTheDestinationsOfTheMappings()
        {
            // Setup
            var path = NewPath();
            var mappings = new[] { new ColumnMapping("Id", "Key"), new ColumnMapping("Name", "Title") };
            var source = Source();
            source.Data.Columns["Id"].ColumnName = "Key";
            source.Data.Columns["Name"].ColumnName = "Title";

            // Act
            source.ExportDataAsJson("Person", path, mappings);

            // Assert
            CollectionAssert.AreEqual(new[] { "Key", "Title", "Birthday" }, Read(path)[0].EnumerateObject().Select(p => p.Name).ToArray());
        }

        #endregion

        #region Interceptors

        [TestMethod]
        public void TestExportDataAsJsonWritesTheRowsThatTheInterceptorsGiveInOrder()
        {
            // Setup
            var path = NewPath();
            var order = new List<string>();
            var interceptors = Interceptors(
                row => { order.Add("first:" + row["Name"]); row["Name"] = ((string)row["Name"]).ToUpperInvariant(); return row; },
                row => { order.Add("second:" + row["Name"]); row["Name"] = row["Name"] + "!"; return row; });

            // Act
            Source(2).ExportDataAsJson("Person", path, dataInterceptors: interceptors);

            // Assert
            CollectionAssert.AreEqual(new[] { "first:Name 1", "second:NAME 1", "first:Name 2", "second:NAME 2" }, order);
            CollectionAssert.AreEqual(new[] { "NAME 1!", "NAME 2!" }, Read(path).Select(row => row.GetProperty("Name").GetString()).ToArray());
        }

        [TestMethod]
        public async Task TestExportDataAsJsonAsyncWritesTheRowsThatTheInterceptorsGive()
        {
            // Setup
            var path = NewPath();

            // Act
            await Source(2).ExportDataAsJsonAsync("Person", path, dataInterceptors: Interceptors(row => { row["Name"] = "Changed"; return row; }));

            // Assert
            CollectionAssert.AreEqual(new[] { "Changed", "Changed" }, Read(path).Select(row => row.GetProperty("Name").GetString()).ToArray());
        }

        [TestMethod]
        public void TestExportDataAsJsonWritesTheColumnsThatTheInterceptorsAddOrRemove()
        {
            // Setup
            var path = NewPath();
            var interceptors = Interceptors(row => row.Add("Extra", typeof(string), "e" + row["Id"]), row => { row.Remove("Birthday"); return row; });

            // Act
            Source(2).ExportDataAsJson("Person", path, dataInterceptors: interceptors);

            // Assert
            var rows = Read(path);
            CollectionAssert.AreEqual(new[] { "Id", "Name", "Extra" }, rows[0].EnumerateObject().Select(p => p.Name).ToArray());
            Assert.AreEqual("e2", rows[1].GetProperty("Extra").GetString());
        }

        [TestMethod]
        public void TestExportDataAsJsonWritesTheNullValuesThatTheInterceptorsGiveAsNull()
        {
            // Setup
            var path = NewPath();

            // Act
            Source(1).ExportDataAsJson("Person", path, dataInterceptors: Interceptors(row => { row["Name"] = null; return row; }));

            // Assert
            Assert.AreEqual(JsonValueKind.Null, Read(path)[0].GetProperty("Name").ValueKind);
        }

        [TestMethod]
        public void TestExportDataAsJsonGivesTheInterceptorsTheColumnsThatTheMappingsSelect()
        {
            // Setup
            var names = new List<string>();
            var source = Source(1);
            source.Data.Columns["Id"].ColumnName = "Key";

            // Act
            source.ExportDataAsJson("Person", NewPath(), new[] { new ColumnMapping("Id", "Key") }, dataInterceptors: Interceptors(row => { names.AddRange(row.Columns.Select(c => c.Name)); return row; }));

            // Assert
            CollectionAssert.Contains(names, "Key");
        }

        [TestMethod]
        public void ThrowExceptionOnExportDataAsJsonIfAnInterceptorReturnsNullAndLeaveNoFile()
        {
            // Setup
            var path = NewPath();
            File.WriteAllText(path, "old content");

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => Source().ExportDataAsJson("Person", path, dataInterceptors: Interceptors(row => null)));
            Assert.AreEqual("old content", File.ReadAllText(path));
            Assert.AreEqual(1, Directory.GetFiles(_directory).Length);
        }

        #endregion

        #region Failures

        [TestMethod]
        public void ThrowExceptionOnExportDataAsJsonIfTheCallbackFailsAndLeaveNoPartialFile()
        {
            // Setup
            var path = NewPath();

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => Source().ExportDataAsJson("Person", path, progressCallback: p =>
            {
                if (p.Position == 2)
                {
                    throw new InvalidOperationException("stop");
                }
            }));
            Assert.IsFalse(File.Exists(path));
            Assert.AreEqual(0, Directory.GetFiles(_directory).Length);
        }

        [TestMethod]
        public void ThrowExceptionOnExportDataAsJsonIfTheDirectoryDoesNotExist()
        {
            // Act/Assert
            Assert.Throws<DirectoryNotFoundException>(() => Source().ExportDataAsJson("Person", Path.Combine(_directory, "missing", "file.json")));
        }

        [TestMethod]
        public async Task ThrowExceptionOnExportDataAsJsonAsyncIfTheTokenIsCancelledAndLeaveNoFile()
        {
            // Setup
            var path = NewPath();
            using (var source = new CancellationTokenSource())
            {
                source.Cancel();

                // Act/Assert
                await Assert.ThrowsAsync<OperationCanceledException>(() => Source().ExportDataAsJsonAsync("Person", path, cancellationToken: source.Token));
                Assert.IsFalse(File.Exists(path));
                Assert.AreEqual(0, Directory.GetFiles(_directory).Length);
            }
        }

        #endregion

        #region Entity

        public class Person
        {
            public int Id { get; set; }

            public string Name { get; set; }
        }

        [TestMethod]
        public void TestExportDataAsJsonOfAnEntityExportsTheTableOfTheEntity()
        {
            // Setup
            var path = NewPath();
            var source = Source();

            // Act
            var actual = source.ExportDataAsJson<Person>(path);

            // Assert
            Assert.AreEqual(3, Read(path).Length);
            Assert.AreEqual(3L, actual.TotalRowCount);
            StringAssert.Contains(source.Executions.Single(e => e.Kind == "Reader").CommandText, "[Person]");
        }

        [TestMethod]
        public void TestExportDataAsJsonOfAnEntityCountsAndExportsTheRowsThatTheExpressionSelects()
        {
            // Setup
            var source = Source();

            // Act
            source.ExportDataAsJson<Person>(NewPath(), where: p => p.Id > 1);

            // Assert
            var count = source.Executions.First(e => e.Kind == "Scalar");
            var select = source.Executions.Single(e => e.Kind == "Reader");
            StringAssert.Contains(count.CommandText, "WHERE");
            StringAssert.Contains(select.CommandText, "WHERE");
            Assert.AreEqual(1, select.Parameters.Values.Single());
            Assert.IsTrue(count.Parameters.Values.Contains(1));
        }

        [TestMethod]
        public void TestExportDataAsJsonOfAnEntityWithoutAnExpressionExportsAllTheRows()
        {
            // Setup
            var source = Source();

            // Act
            source.ExportDataAsJson<Person>(NewPath(), where: null);

            // Assert
            Assert.IsFalse(source.Executions.Single(e => e.Kind == "Reader").CommandText.Contains("WHERE"));
        }

        [TestMethod]
        public void TestExportDataAsJsonOfAnEntityWithATableNameExportsThatTable()
        {
            // Setup
            var path = NewPath();
            var source = Source();

            // Act
            source.ExportDataAsJson<Person>("Customer", path, where: p => p.Id == 2);

            // Assert
            var select = source.Executions.Single(e => e.Kind == "Reader");
            StringAssert.Contains(select.CommandText, "[Customer]");
            StringAssert.Contains(select.CommandText, "WHERE");
            Assert.AreEqual(2, select.Parameters.Values.Single());
        }

        [TestMethod]
        public void TestExportDataAsJsonOfAnEntityPassesTheMappingsTheInterceptorsAndTheCallback()
        {
            // Setup
            var path = NewPath();
            var source = Source();
            var seen = new List<long>();
            var mappings = new[] { new ColumnMapping("Name", "Title") };
            source.Data.Columns["Name"].ColumnName = "Title";

            // Act
            source.ExportDataAsJson<Person>(path, mappings, dataInterceptors: Interceptors(row => { row["Title"] = "T"; return row; }), progressCallback: p => seen.Add(p.Position), commandTimeout: 55);

            // Assert
            StringAssert.Contains(source.Executions.Single(e => e.Kind == "Reader").CommandText, "[Name] AS [Title]");
            Assert.AreEqual(55, source.Executions.Single(e => e.Kind == "Reader").CommandTimeout);
            CollectionAssert.AreEqual(new[] { "T", "T", "T" }, Read(path).Select(row => row.GetProperty("Title").GetString()).ToArray());
            CollectionAssert.AreEqual(new long[] { 0, 1, 2, 3, 3 }, seen);
        }

        [TestMethod]
        public async Task TestExportDataAsJsonAsyncOfAnEntityExportsTheRowsThatTheExpressionSelects()
        {
            // Setup
            var path = NewPath();
            var source = Source();

            // Act
            var actual = await source.ExportDataAsJsonAsync<Person>(path, where: p => p.Id >= 2);

            // Assert
            Assert.AreEqual(3, Read(path).Length);
            Assert.AreEqual(3L, actual.TotalRowCount);
            StringAssert.Contains(source.Executions.Single(e => e.Kind == "Reader").CommandText, "WHERE");
        }

        [TestMethod]
        public async Task TestExportDataAsJsonAsyncOfAnEntityWithATableNameExportsThatTable()
        {
            // Setup
            var source = Source();

            // Act
            await source.ExportDataAsJsonAsync<Person>("Customer", NewPath(), where: p => p.Name == "Name 1");

            // Assert
            StringAssert.Contains(source.Executions.Single(e => e.Kind == "Reader").CommandText, "[Customer]");
        }

        [TestMethod]
        public void ThrowExceptionOnExportDataAsJsonOfAnEntityIfTheConnectionIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => ((IDbConnection)null).ExportDataAsJson<Person>(NewPath()));
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow(" ")]
        public void ThrowExceptionOnExportDataAsJsonOfAnEntityIfTheTableNameOrTheFileNameIsNullOrWhiteSpace(string value)
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => Source().ExportDataAsJson<Person>(value, NewPath()));
            Assert.Throws<ArgumentNullException>(() => Source().ExportDataAsJson<Person>("Customer", value));
            Assert.Throws<ArgumentNullException>(() => Source().ExportDataAsJson<Person>(value));
        }

        [TestMethod]
        public async Task ThrowExceptionOnExportDataAsJsonAsyncOfAnEntityIfTheFileNameIsNull()
        {
            // Act/Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => Source().ExportDataAsJsonAsync<Person>(null));
        }

        #endregion
    }
}
