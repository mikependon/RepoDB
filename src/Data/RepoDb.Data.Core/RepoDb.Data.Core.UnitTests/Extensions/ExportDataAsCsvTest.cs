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
using System.Text;
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
    public class ExportDataAsCsvTest
    {
        #region Helpers

        private string _directory;

        [TestInitialize]
        public void Initialize()
        {
            _directory = Path.Combine(Path.GetTempPath(), "repodb_export_csv_" + Guid.NewGuid().ToString("N"));
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
            Path.Combine(_directory, Guid.NewGuid().ToString("N") + ".csv");

        private static IList<IDataInterceptor<CopyRow>> Interceptors(params Func<CopyRow, CopyRow>[] transforms) =>
            transforms.Select(transform => (IDataInterceptor<CopyRow>)new FakeDataInterceptor(transform)).ToList();

        private static FakeDbConnection Source(int rows = 3) =>
            new FakeDbConnection { Data = FakeDbConnection.CreatePersons(rows) };

        private static FakeDbConnection SourceOf(params object[][] rows)
        {
            var table = new DataTable("Person");
            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Birthday", typeof(DateTime));
            foreach (var row in rows)
            {
                table.Rows.Add(row);
            }
            return new FakeDbConnection { Data = table };
        }

        private static string[] Lines(string path) =>
            File.ReadAllText(path).Split(new[] { "\r\n" }, StringSplitOptions.None);

        public class Person
        {
            public int Id { get; set; }

            public string Name { get; set; }
        }

        #endregion

        #region Arguments

        [TestMethod]
        public void ThrowExceptionOnExportDataAsCsvIfTheConnectionIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => ((IDbConnection)null).ExportDataAsCsv("Person", NewPath()));
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("  ")]
        public void ThrowExceptionOnExportDataAsCsvIfTheTableNameOrTheFileNameIsNullOrWhiteSpace(string value)
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => Source().ExportDataAsCsv(value, NewPath()));
            Assert.Throws<ArgumentNullException>(() => Source().ExportDataAsCsv("Person", value));
        }

        [DataTestMethod]
        [DataRow('"')]
        [DataRow('\r')]
        [DataRow('\n')]
        public void ThrowExceptionOnExportDataAsCsvIfTheSeparatorIsAQuoteOrALineBreak(char separator)
        {
            // Setup
            var path = NewPath();

            // Act/Assert
            Assert.Throws<ArgumentException>(() => Source().ExportDataAsCsv("Person", path, separator: separator));
            Assert.IsFalse(File.Exists(path));
        }

        [TestMethod]
        public async Task ThrowExceptionOnExportDataAsCsvAsyncIfTheSeparatorIsAQuote()
        {
            // Act/Assert
            await Assert.ThrowsAsync<ArgumentException>(() => Source().ExportDataAsCsvAsync("Person", NewPath(), separator: '"'));
        }

        [TestMethod]
        public void ThrowExceptionOnExportDataAsCsvIfAnInterceptorIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentException>(() => Source().ExportDataAsCsv("Person", NewPath(), dataInterceptors: new List<IDataInterceptor<CopyRow>> { null }));
        }

        [TestMethod]
        public void ThrowExceptionOnExportDataAsCsvIfAMappingIsNotValid()
        {
            // Act/Assert
            Assert.Throws<ArgumentException>(() => Source().ExportDataAsCsv("Person", NewPath(), new CopyColumnMapping[] { null }));
            Assert.Throws<ArgumentException>(() => Source().ExportDataAsCsv("Person", NewPath(), new[] { new CopyColumnMapping("Missing", "Id") }));
            Assert.Throws<ArgumentException>(() => Source().ExportDataAsCsv("Person", NewPath(), new[] { new CopyColumnMapping("Id", "Key"), new CopyColumnMapping("Name", "key") }));
        }

        #endregion

        #region Rows

        [TestMethod]
        public void TestExportDataAsCsvWritesTheHeaderThenALineForEachRow()
        {
            // Setup
            var path = NewPath();

            // Act
            Source().ExportDataAsCsv("Person", path);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                "Id,Name,Birthday",
                "1,Name 1," + new DateTime(2000, 1, 1).ToString("o"),
                "2,Name 2,",
                "3,Name 3," + new DateTime(2000, 1, 3).ToString("o"),
                string.Empty
            }, Lines(path));
        }

        [TestMethod]
        public void TestExportDataAsCsvWritesUtf8WithoutAByteOrderMark()
        {
            // Setup
            var path = NewPath();

            // Act
            SourceOf(new object[] { 1, "漢字 é", DBNull.Value }).ExportDataAsCsv("Person", path);

            // Assert
            var bytes = File.ReadAllBytes(path);
            Assert.IsFalse(bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF);
            StringAssert.Contains(File.ReadAllText(path, Encoding.UTF8), "漢字 é");
        }

        [TestMethod]
        public async Task TestExportDataAsCsvAsyncWritesTheRows()
        {
            // Setup
            var path = NewPath();

            // Act
            await Source(3).ExportDataAsCsvAsync("Person", path);

            // Assert
            Assert.AreEqual(5, Lines(path).Length);
            Assert.AreEqual("Id,Name,Birthday", Lines(path)[0]);
        }

        [TestMethod]
        public void TestExportDataAsCsvOfATableWithoutRowsWritesTheHeaderOnly()
        {
            // Setup
            var path = NewPath();
            var seen = new List<(long Position, long Rows, long Total, bool Ended)>();

            // Act
            Source(0).ExportDataAsCsv("Person", path, progressCallback: p => seen.Add((p.Position, p.CurrentRowCount, p.TotalRowCount, p.End.HasValue)));

            // Assert
            CollectionAssert.AreEqual(new[] { "Id,Name,Birthday", string.Empty }, Lines(path));
            CollectionAssert.AreEqual(new[] { (0L, 0L, 0L, false), (0L, 0L, 0L, true) }, seen);
        }

        [TestMethod]
        public void TestExportDataAsCsvQuotesTheFieldsThatNeedIt()
        {
            // Setup
            var path = NewPath();
            var source = SourceOf(
                new object[] { 1, "a,b", DBNull.Value },
                new object[] { 2, "say \"hi\"", DBNull.Value },
                new object[] { 3, "two\r\nlines", DBNull.Value },
                new object[] { 4, " padded ", DBNull.Value });

            // Act
            source.ExportDataAsCsv("Person", path);

            // Assert
            var text = File.ReadAllText(path);
            StringAssert.Contains(text, "1,\"a,b\",\r\n");
            StringAssert.Contains(text, "2,\"say \"\"hi\"\"\",\r\n");
            StringAssert.Contains(text, "3,\"two\r\nlines\",\r\n");
            StringAssert.Contains(text, "4, padded ,\r\n");
        }

        [TestMethod]
        public void TestExportDataAsCsvWritesANullAsAnUnquotedEmptyFieldAndAnEmptyTextAsAQuotedEmptyField()
        {
            // Setup
            var path = NewPath();

            // Act
            SourceOf(new object[] { 1, DBNull.Value, DBNull.Value }, new object[] { 2, string.Empty, DBNull.Value }).ExportDataAsCsv("Person", path);

            // Assert
            CollectionAssert.AreEqual(new[] { "Id,Name,Birthday", "1,,", "2,\"\",", string.Empty }, Lines(path));
        }

        [DataTestMethod]
        [DataRow(';')]
        [DataRow('\t')]
        [DataRow('|')]
        public void TestExportDataAsCsvSeparatesTheFieldsBySeparator(char separator)
        {
            // Setup
            var path = NewPath();

            // Act
            SourceOf(new object[] { 1, "a" + separator + "b", DBNull.Value }).ExportDataAsCsv("Person", path, separator: separator);

            // Assert
            var lines = Lines(path);
            Assert.AreEqual("Id" + separator + "Name" + separator + "Birthday", lines[0]);
            Assert.AreEqual("1" + separator + "\"a" + separator + "b\"" + separator, lines[1]);
        }

        [TestMethod]
        public void TestExportDataAsCsvQuotesTheNamesOfTheHeaderThatNeedIt()
        {
            // Setup
            var path = NewPath();
            var table = new DataTable("Person");
            table.Columns.Add("Unit,Price", typeof(decimal));
            table.Rows.Add(1.5m);
            var source = new FakeDbConnection { Data = table };

            // Act
            source.ExportDataAsCsv("Person", path);

            // Assert
            CollectionAssert.AreEqual(new[] { "\"Unit,Price\"", "1.5", string.Empty }, Lines(path));
        }

        [TestMethod]
        public void TestExportDataAsCsvCanBeLoadedBackByImportDataFromCsv()
        {
            // Setup
            var path = NewPath();
            var destination = new FakeDbConnection();
            var source = SourceOf(
                new object[] { 1, "a,b \"q\"\nline", new DateTime(2026, 1, 2, 3, 4, 5, DateTimeKind.Utc) },
                new object[] { 2, string.Empty, DBNull.Value },
                new object[] { 3, DBNull.Value, DBNull.Value });
            source.ExportDataAsCsv("Person", path);

            // Act
            var actual = destination.ImportDataFromCsv("Person", path);

            // Assert
            var inserts = destination.Executions.Where(e => e.CommandText.StartsWith("INSERT INTO", StringComparison.Ordinal)).ToList();
            Assert.AreEqual(3L, actual.LoadedRowCount);
            CollectionAssert.AreEquivalent(new object[] { 1, "a,b \"q\"\nline", new DateTime(2026, 1, 2, 3, 4, 5, DateTimeKind.Utc) }, inserts[0].Parameters.Values.ToArray());
            Assert.IsTrue(inserts[1].Parameters.Values.Contains(string.Empty));
            Assert.IsFalse(inserts[2].Parameters.Values.Any(v => v is string));
        }

        [TestMethod]
        public void TestExportDataAsCsvReplacesAnExistingFile()
        {
            // Setup
            var path = NewPath();
            File.WriteAllText(path, "old content");

            // Act
            Source(2).ExportDataAsCsv("Person", path);

            // Assert
            Assert.AreEqual(4, Lines(path).Length);
            Assert.AreEqual(1, Directory.GetFiles(_directory).Length);
        }

        #endregion

        #region Progress

        [TestMethod]
        public void TestExportDataAsCsvCallsTheCallbackAfterTheRowsAreCountedAfterEveryRowAndWhenItEnds()
        {
            // Setup
            var seen = new List<(long Position, long Rows, long Total, bool Ended)>();
            var instances = new List<ExportProgress>();

            // Act
            var actual = Source(3).ExportDataAsCsv("Person", NewPath(), progressCallback: p =>
            {
                seen.Add((p.Position, p.CurrentRowCount, p.TotalRowCount, p.End.HasValue));
                instances.Add(p);
            });

            // Assert
            CollectionAssert.AreEqual(new[] { (0L, 0L, 3L, false), (1L, 1L, 3L, false), (2L, 2L, 3L, false), (3L, 3L, 3L, false), (3L, 3L, 3L, true) }, seen);
            Assert.IsTrue(instances.All(instance => ReferenceEquals(instance, actual)));
            Assert.AreEqual(DateTimeKind.Utc, actual.Start.Kind);
            Assert.IsTrue(actual.End >= actual.Start);
        }

        [TestMethod]
        public async Task TestExportDataAsCsvAsyncCallsTheCallback()
        {
            // Setup
            var seen = new List<(long Position, long Rows, long Total, bool Ended)>();

            // Act
            await Source(2).ExportDataAsCsvAsync("Person", NewPath(), progressCallback: p => seen.Add((p.Position, p.CurrentRowCount, p.TotalRowCount, p.End.HasValue)));

            // Assert
            CollectionAssert.AreEqual(new[] { (0L, 0L, 2L, false), (1L, 1L, 2L, false), (2L, 2L, 2L, false), (2L, 2L, 2L, true) }, seen);
        }

        #endregion

        #region Select

        [TestMethod]
        public void TestExportDataAsCsvSelectsOnlyTheMappedColumnsAndWritesThemByTheirDestinationsInTheHeader()
        {
            // Setup
            var path = NewPath();
            var source = Source();
            source.Data.Columns["Id"].ColumnName = "Key";
            source.Data.Columns["Name"].ColumnName = "Title";

            // Act
            source.ExportDataAsCsv("Person", path, new[] { new CopyColumnMapping("Id", "Key"), new CopyColumnMapping("Name", "Title") });

            // Assert
            StringAssert.Contains(source.Executions.Single(e => e.Kind == "Reader").CommandText, "[Id] AS [Key], [Name] AS [Title]");
            Assert.AreEqual("Key,Title,Birthday", Lines(path)[0]);
        }

        [TestMethod]
        public void TestExportDataAsCsvCountsAndSelectsTheRowsThatTheFilterSelects()
        {
            // Setup
            var source = Source();

            // Act
            source.ExportDataAsCsv("Person", NewPath(), where: new QueryGroup(new QueryField("Id", Operation.GreaterThan, 1)), commandTimeout: 66);

            // Assert
            var kinds = source.Executions.Select(e => e.Kind).ToList();
            var select = source.Executions.Single(e => e.Kind == "Reader");
            StringAssert.Contains(source.Executions.First(e => e.Kind == "Scalar").CommandText, "WHERE");
            StringAssert.Contains(select.CommandText, "WHERE");
            Assert.AreEqual(1, select.Parameters.Values.Single());
            Assert.AreEqual(66, select.CommandTimeout);
            Assert.IsTrue(kinds.IndexOf("Scalar") < kinds.IndexOf("Reader"));
        }

        #endregion

        #region Interceptors

        [TestMethod]
        public void TestExportDataAsCsvWritesTheRowsThatTheInterceptorsGiveInOrder()
        {
            // Setup
            var path = NewPath();
            var interceptors = Interceptors(
                row => { row["Name"] = ((string)row["Name"]).ToUpperInvariant(); return row; },
                row => { row["Name"] = row["Name"] + "!"; return row; });

            // Act
            Source(2).ExportDataAsCsv("Person", path, dataInterceptors: interceptors);

            // Assert
            Assert.IsTrue(Lines(path)[1].Contains("NAME 1!"));
            Assert.IsTrue(Lines(path)[2].Contains("NAME 2!"));
        }

        [TestMethod]
        public void TestExportDataAsCsvWritesTheColumnsThatTheInterceptorsAddOrRemoveWithTheHeaderOfTheFirstRow()
        {
            // Setup
            var path = NewPath();
            var interceptors = Interceptors(row => row.Add("Extra", typeof(string), "e" + row["Id"]), row => { row.Remove("Birthday"); return row; });

            // Act
            Source(2).ExportDataAsCsv("Person", path, dataInterceptors: interceptors);

            // Assert
            CollectionAssert.AreEqual(new[] { "Id,Name,Extra", "1,Name 1,e1", "2,Name 2,e2", string.Empty }, Lines(path));
        }

        [TestMethod]
        public void ThrowExceptionOnExportDataAsCsvIfTheInterceptorsGiveAnotherShapeToAnotherRowAndLeaveNoFile()
        {
            // Setup
            var path = NewPath();
            var interceptors = Interceptors(row => (int)row["Id"] == 2 ? row.Add("Extra", typeof(int), 1) : row);

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => Source(3).ExportDataAsCsv("Person", path, dataInterceptors: interceptors));
            Assert.IsFalse(File.Exists(path));
            Assert.AreEqual(0, Directory.GetFiles(_directory).Length);
        }

        [TestMethod]
        public async Task TestExportDataAsCsvAsyncWritesTheRowsThatTheInterceptorsGive()
        {
            // Setup
            var path = NewPath();

            // Act
            await Source(2).ExportDataAsCsvAsync("Person", path, dataInterceptors: Interceptors(row => { row["Name"] = "Changed"; return row; }));

            // Assert
            Assert.IsTrue(Lines(path)[1].Contains("Changed"));
        }

        [TestMethod]
        public void ThrowExceptionOnExportDataAsCsvIfAnInterceptorReturnsNullAndLeaveNoFile()
        {
            // Setup
            var path = NewPath();
            File.WriteAllText(path, "old content");

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => Source().ExportDataAsCsv("Person", path, dataInterceptors: Interceptors(row => null)));
            Assert.AreEqual("old content", File.ReadAllText(path));
            Assert.AreEqual(1, Directory.GetFiles(_directory).Length);
        }

        #endregion

        #region Failures

        [TestMethod]
        public void ThrowExceptionOnExportDataAsCsvIfTheCallbackFailsAndLeaveNoPartialFile()
        {
            // Setup
            var path = NewPath();

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => Source().ExportDataAsCsv("Person", path, progressCallback: p =>
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
        public async Task ThrowExceptionOnExportDataAsCsvAsyncIfTheTokenIsCancelledAndLeaveNoFile()
        {
            // Setup
            var path = NewPath();
            using (var source = new CancellationTokenSource())
            {
                source.Cancel();

                // Act/Assert
                await Assert.ThrowsAsync<OperationCanceledException>(() => Source().ExportDataAsCsvAsync("Person", path, cancellationToken: source.Token));
                Assert.IsFalse(File.Exists(path));
                Assert.AreEqual(0, Directory.GetFiles(_directory).Length);
            }
        }

        #endregion

        #region Entity

        [TestMethod]
        public void TestExportDataAsCsvOfAnEntityExportsTheTableOfTheEntityFilteredByTheExpression()
        {
            // Setup
            var source = Source();

            // Act
            var actual = source.ExportDataAsCsv<Person>(NewPath(), where: p => p.Id > 1);

            // Assert
            var select = source.Executions.Single(e => e.Kind == "Reader");
            Assert.AreEqual(3L, actual.TotalRowCount);
            StringAssert.Contains(select.CommandText, "[Person]");
            StringAssert.Contains(select.CommandText, "WHERE");
            Assert.AreEqual(1, select.Parameters.Values.Single());
        }

        [TestMethod]
        public void TestExportDataAsCsvOfAnEntityPassesTheSeparator()
        {
            // Setup
            var path = NewPath();

            // Act
            Source(1).ExportDataAsCsv<Person>(path, separator: ';');

            // Assert
            Assert.AreEqual("Id;Name;Birthday", Lines(path)[0]);
        }

        [TestMethod]
        public void TestExportDataAsCsvOfAnEntityWithATableNameExportsThatTable()
        {
            // Setup
            var source = Source();

            // Act
            source.ExportDataAsCsv<Person>("Customer", NewPath(), separator: '|', where: p => p.Id == 2);

            // Assert
            var select = source.Executions.Single(e => e.Kind == "Reader");
            StringAssert.Contains(select.CommandText, "[Customer]");
            StringAssert.Contains(select.CommandText, "WHERE");
        }

        [TestMethod]
        public async Task TestExportDataAsCsvAsyncOfAnEntityExportsTheTableOfTheEntity()
        {
            // Setup
            var source = Source();

            // Act
            await source.ExportDataAsCsvAsync<Person>(NewPath(), where: p => p.Id >= 2);

            // Assert
            StringAssert.Contains(source.Executions.Single(e => e.Kind == "Reader").CommandText, "[Person]");
        }

        [TestMethod]
        public async Task TestExportDataAsCsvAsyncOfAnEntityWithATableNameExportsThatTable()
        {
            // Setup
            var source = Source();

            // Act
            await source.ExportDataAsCsvAsync<Person>("Customer", NewPath(), separator: ';');

            // Assert
            StringAssert.Contains(source.Executions.Single(e => e.Kind == "Reader").CommandText, "[Customer]");
        }

        #endregion
    }
}
