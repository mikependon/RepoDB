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
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Data.Core.UnitTests.CustomObjects;
using RepoDb.Data.Interfaces;
using RepoDb.Data.Models;
using RepoDb.Enumerations;

namespace RepoDb.Data.Core.UnitTests.Extensions
{
    [TestClass]
    public class ExportDataAsXmlTest
    {
        #region Helpers

        private string _directory;

        [TestInitialize]
        public void Initialize()
        {
            _directory = Path.Combine(Path.GetTempPath(), "repodb_export_xml_" + Guid.NewGuid().ToString("N"));
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
            Path.Combine(_directory, Guid.NewGuid().ToString("N") + ".xml");

        private static List<XElement> Read(string path) =>
            XDocument.Load(path).Root.Elements("row").ToList();

        private static IList<IDataInterceptor<Row>> Interceptors(params Func<Row, Row>[] transforms) =>
            transforms.Select(transform => (IDataInterceptor<Row>)new FakeDataInterceptor(transform)).ToList();

        private static FakeDbConnection Source(int rows = 3) =>
            new FakeDbConnection { Data = FakeDbConnection.CreatePersons(rows) };

        public class Person
        {
            public int Id { get; set; }

            public string Name { get; set; }
        }

        #endregion

        #region Arguments

        [TestMethod]
        public void ThrowExceptionOnExportDataAsXmlIfTheConnectionIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => ((IDbConnection)null).ExportDataAsXml("Person", NewPath()));
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("  ")]
        public void ThrowExceptionOnExportDataAsXmlIfTheTableNameOrTheFileNameIsNullOrWhiteSpace(string value)
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => Source().ExportDataAsXml(value, NewPath()));
            Assert.Throws<ArgumentNullException>(() => Source().ExportDataAsXml("Person", value));
        }

        [TestMethod]
        public void ThrowExceptionOnExportDataAsXmlIfAnInterceptorIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentException>(() => Source().ExportDataAsXml("Person", NewPath(), dataInterceptors: new List<IDataInterceptor<Row>> { null }));
        }

        [TestMethod]
        public void ThrowExceptionOnExportDataAsXmlIfAMappingIsNotValid()
        {
            // Act/Assert
            Assert.Throws<ArgumentException>(() => Source().ExportDataAsXml("Person", NewPath(), new ColumnMapping[] { null }));
            Assert.Throws<ArgumentException>(() => Source().ExportDataAsXml("Person", NewPath(), new[] { new ColumnMapping("Id", " ") }));
            Assert.Throws<ArgumentException>(() => Source().ExportDataAsXml("Person", NewPath(), new[] { new ColumnMapping("Missing", "Id") }));
            Assert.Throws<ArgumentException>(() => Source().ExportDataAsXml("Person", NewPath(), new[] { new ColumnMapping("Id", "Key"), new ColumnMapping("Name", "KEY") }));
        }

        #endregion

        #region Rows

        [TestMethod]
        public void TestExportDataAsXmlWritesARootWithARowElementForEachRowAndAnElementForEachColumn()
        {
            // Setup
            var path = NewPath();

            // Act
            Source().ExportDataAsXml("Person", path);

            // Assert
            var document = XDocument.Load(path);
            var rows = Read(path);
            Assert.AreEqual("rows", document.Root.Name.LocalName);
            Assert.AreEqual(3, rows.Count);
            CollectionAssert.AreEqual(new[] { "Id", "Name", "Birthday" }, rows[0].Elements().Select(e => e.Name.LocalName).ToArray());
            Assert.AreEqual("1", rows[0].Element("Id").Value);
            Assert.AreEqual("Name 1", rows[0].Element("Name").Value);
            Assert.AreEqual(new DateTime(2000, 1, 1).ToString("o"), rows[0].Element("Birthday").Value);
            Assert.AreEqual(string.Empty, rows[1].Element("Birthday").Value);
            Assert.IsTrue(rows[1].Element("Birthday").IsEmpty);
        }

        [TestMethod]
        public void TestExportDataAsXmlWritesTheDeclarationInUtf8WithoutAByteOrderMark()
        {
            // Setup
            var path = NewPath();

            // Act
            Source(1).ExportDataAsXml("Person", path);

            // Assert
            var bytes = File.ReadAllBytes(path);
            Assert.IsFalse(bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF);
            StringAssert.StartsWith(File.ReadAllText(path), "<?xml version=\"1.0\" encoding=\"utf-8\"?>");
        }

        [TestMethod]
        public async Task TestExportDataAsXmlAsyncWritesTheRows()
        {
            // Setup
            var path = NewPath();

            // Act
            await Source(5).ExportDataAsXmlAsync("Person", path);

            // Assert
            CollectionAssert.AreEqual(new[] { "1", "2", "3", "4", "5" }, Read(path).Select(row => row.Element("Id").Value).ToArray());
        }

        [TestMethod]
        public void TestExportDataAsXmlOfATableWithoutRowsWritesAnEmptyRoot()
        {
            // Setup
            var path = NewPath();
            var seen = new List<(long Position, long Rows, long Total, bool Ended)>();

            // Act
            Source(0).ExportDataAsXml("Person", path, progressCallback: p => seen.Add((p.Position, p.CurrentRowCount, p.TotalRowCount, p.End.HasValue)));

            // Assert
            Assert.AreEqual(0, Read(path).Count);
            Assert.AreEqual("rows", XDocument.Load(path).Root.Name.LocalName);
            CollectionAssert.AreEqual(new[] { (0L, 0L, 0L, false), (0L, 0L, 0L, true) }, seen);
        }

        [TestMethod]
        public void TestExportDataAsXmlWritesTheSpecialCharactersAndTheNamesThatAreNotXmlNames()
        {
            // Setup
            var path = NewPath();
            var table = new DataTable("Person");
            table.Columns.Add("Unit Price", typeof(decimal));
            table.Columns.Add("1st", typeof(string));
            table.Columns.Add("Text", typeof(string));
            table.Rows.Add(12.5m, "x", "a & b <c> \"q\" 'x' é 漢字");
            var source = new FakeDbConnection { Data = table };

            // Act
            source.ExportDataAsXml("Person", path);

            // Assert
            var row = Read(path).Single();
            Assert.AreEqual("Unit Price", XmlConvert.DecodeName(row.Elements().ElementAt(0).Name.LocalName));
            Assert.AreEqual("1st", XmlConvert.DecodeName(row.Elements().ElementAt(1).Name.LocalName));
            Assert.AreEqual("12.5", row.Elements().ElementAt(0).Value);
            Assert.AreEqual("a & b <c> \"q\" 'x' é 漢字", row.Element("Text").Value);
        }

        [TestMethod]
        public void TestExportDataAsXmlCanBeLoadedBackByImportDataFromXml()
        {
            // Setup
            var path = NewPath();
            var destination = new FakeDbConnection();
            var table = new DataTable("Person");
            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Birthday", typeof(DateTime));
            table.Rows.Add(1, "a & b <c>", new DateTime(2026, 1, 2, 3, 4, 5, DateTimeKind.Utc));
            table.Rows.Add(2, DBNull.Value, DBNull.Value);
            new FakeDbConnection { Data = table }.ExportDataAsXml("Person", path);

            // Act
            var actual = destination.ImportDataFromXml("Person", path);

            // Assert
            var inserts = destination.Executions.Where(e => e.CommandText.StartsWith("INSERT INTO", StringComparison.Ordinal)).ToList();
            Assert.AreEqual(2L, actual.LoadedRowCount);
            CollectionAssert.AreEquivalent(new object[] { 1, "a & b <c>", new DateTime(2026, 1, 2, 3, 4, 5, DateTimeKind.Utc) }, inserts[0].Parameters.Values.ToArray());
            Assert.IsTrue(inserts[1].Parameters.Values.Contains(2));
            Assert.IsFalse(inserts[1].Parameters.Values.Any(v => v is string));
        }

        [TestMethod]
        public void TestExportDataAsXmlWritesANullAsASelfClosingElementAndAnEmptyTextWithItsStartAndEndTags()
        {
            // Setup
            var path = NewPath();
            var table = new DataTable("Person");
            table.Columns.Add("Name", typeof(string));
            table.Rows.Add(DBNull.Value);
            table.Rows.Add(string.Empty);
            var source = new FakeDbConnection { Data = table };

            // Act
            source.ExportDataAsXml("Person", path);

            // Assert
            var text = File.ReadAllText(path);
            StringAssert.Contains(text, "<Name />");
            StringAssert.Contains(text, "<Name></Name>");
        }

        [TestMethod]
        public void TestExportDataAsXmlKeepsAnEmptyTextWhenItIsLoadedBackAndAnEmptyTextOfAColumnThatIsNotATextIsNull()
        {
            // Setup
            var path = NewPath();
            var destination = new FakeDbConnection();
            var table = new DataTable("Person");
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Id", typeof(string));
            table.Rows.Add(string.Empty, string.Empty);
            table.Rows.Add(DBNull.Value, DBNull.Value);
            new FakeDbConnection { Data = table }.ExportDataAsXml("Person", path);

            // Act
            destination.ImportDataFromXml("Person", path);

            // Assert
            var inserts = destination.Executions.Where(e => e.CommandText.StartsWith("INSERT INTO", StringComparison.Ordinal)).ToList();
            Assert.IsTrue(inserts[0].Parameters.Values.Contains(string.Empty));
            Assert.IsFalse(inserts[0].Parameters.Values.Any(v => v is int));
            Assert.IsFalse(inserts[1].Parameters.Values.Any(v => v is string));
        }

        [TestMethod]
        public void TestExportDataAsXmlReplacesAnExistingFile()
        {
            // Setup
            var path = NewPath();
            File.WriteAllText(path, "old content");

            // Act
            Source(2).ExportDataAsXml("Person", path);

            // Assert
            Assert.AreEqual(2, Read(path).Count);
            Assert.AreEqual(1, Directory.GetFiles(_directory).Length);
        }

        #endregion

        #region Progress

        [TestMethod]
        public void TestExportDataAsXmlCallsTheCallbackAfterTheRowsAreCountedAfterEveryRowAndWhenItEnds()
        {
            // Setup
            var seen = new List<(long Position, long Rows, long Total, bool Ended)>();
            var instances = new List<ExportProgress>();

            // Act
            var actual = Source(3).ExportDataAsXml("Person", NewPath(), progressCallback: p =>
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
        public async Task TestExportDataAsXmlAsyncCallsTheCallback()
        {
            // Setup
            var seen = new List<(long Position, long Rows, long Total, bool Ended)>();

            // Act
            await Source(2).ExportDataAsXmlAsync("Person", NewPath(), progressCallback: p => seen.Add((p.Position, p.CurrentRowCount, p.TotalRowCount, p.End.HasValue)));

            // Assert
            CollectionAssert.AreEqual(new[] { (0L, 0L, 2L, false), (1L, 1L, 2L, false), (2L, 2L, 2L, false), (2L, 2L, 2L, true) }, seen);
        }

        [TestMethod]
        public void TestExportDataAsXmlWritesIntoATemporaryFileUntilTheExportHasSucceeded()
        {
            // Setup
            var path = NewPath();
            var duringTheExport = new List<(bool TargetExists, int Files)>();

            // Act
            Source(2).ExportDataAsXml("Person", path, progressCallback: p => duringTheExport.Add((File.Exists(path), Directory.GetFiles(_directory).Length)));

            // Assert
            CollectionAssert.AreEqual(new[] { (false, 0), (false, 1), (false, 1), (true, 1) }, duringTheExport);
        }

        #endregion

        #region Select

        [TestMethod]
        public void TestExportDataAsXmlSelectsOnlyTheMappedColumnsAndWritesThemByTheirDestinations()
        {
            // Setup
            var path = NewPath();
            var source = Source();
            source.Data.Columns["Id"].ColumnName = "Key";
            source.Data.Columns["Name"].ColumnName = "Title";

            // Act
            source.ExportDataAsXml("Person", path, new[] { new ColumnMapping("Id", "Key"), new ColumnMapping("Name", "Title") });

            // Assert
            StringAssert.Contains(source.Executions.Single(e => e.Kind == "Reader").CommandText, "[Id] AS [Key], [Name] AS [Title]");
            CollectionAssert.AreEqual(new[] { "Key", "Title", "Birthday" }, Read(path)[0].Elements().Select(e => e.Name.LocalName).ToArray());
        }

        [TestMethod]
        public void TestExportDataAsXmlCountsAndSelectsTheRowsThatTheFilterSelects()
        {
            // Setup
            var source = Source();

            // Act
            source.ExportDataAsXml("Person", NewPath(), where: new QueryGroup(new QueryField("Id", Operation.GreaterThan, 1)), commandTimeout: 66);

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
        public void TestExportDataAsXmlWritesTheRowsThatTheInterceptorsGiveInOrder()
        {
            // Setup
            var path = NewPath();
            var interceptors = Interceptors(
                row => { row["Name"] = ((string)row["Name"]).ToUpperInvariant(); return row; },
                row => { row["Name"] = row["Name"] + "!"; return row; });

            // Act
            Source(2).ExportDataAsXml("Person", path, dataInterceptors: interceptors);

            // Assert
            CollectionAssert.AreEqual(new[] { "NAME 1!", "NAME 2!" }, Read(path).Select(row => row.Element("Name").Value).ToArray());
        }

        [TestMethod]
        public void TestExportDataAsXmlWritesTheColumnsThatTheInterceptorsAddOrRemoveAndTheRowsMayDiffer()
        {
            // Setup
            var path = NewPath();
            var interceptors = Interceptors(row => (int)row["Id"] == 2 ? row.Add("Extra", typeof(string), "e") : row, row => { row.Remove("Birthday"); return row; });

            // Act
            Source(3).ExportDataAsXml("Person", path, dataInterceptors: interceptors);

            // Assert
            var rows = Read(path);
            CollectionAssert.AreEqual(new[] { "Id", "Name" }, rows[0].Elements().Select(e => e.Name.LocalName).ToArray());
            CollectionAssert.AreEqual(new[] { "Id", "Name", "Extra" }, rows[1].Elements().Select(e => e.Name.LocalName).ToArray());
        }

        [TestMethod]
        public async Task TestExportDataAsXmlAsyncWritesTheRowsThatTheInterceptorsGive()
        {
            // Setup
            var path = NewPath();

            // Act
            await Source(2).ExportDataAsXmlAsync("Person", path, dataInterceptors: Interceptors(row => { row["Name"] = null; return row; }));

            // Assert
            Assert.IsTrue(Read(path).All(row => row.Element("Name").IsEmpty));
        }

        [TestMethod]
        public void ThrowExceptionOnExportDataAsXmlIfAnInterceptorReturnsNullAndLeaveNoFile()
        {
            // Setup
            var path = NewPath();
            File.WriteAllText(path, "old content");

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => Source().ExportDataAsXml("Person", path, dataInterceptors: Interceptors(row => null)));
            Assert.AreEqual("old content", File.ReadAllText(path));
            Assert.AreEqual(1, Directory.GetFiles(_directory).Length);
        }

        #endregion

        #region Failures

        [TestMethod]
        public void ThrowExceptionOnExportDataAsXmlIfTheCallbackFailsAndLeaveNoPartialFile()
        {
            // Setup
            var path = NewPath();

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => Source().ExportDataAsXml("Person", path, progressCallback: p =>
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
        public async Task ThrowExceptionOnExportDataAsXmlAsyncIfTheTokenIsCancelledAndLeaveNoFile()
        {
            // Setup
            var path = NewPath();
            using (var source = new CancellationTokenSource())
            {
                source.Cancel();

                // Act/Assert
                await Assert.ThrowsAsync<OperationCanceledException>(() => Source().ExportDataAsXmlAsync("Person", path, cancellationToken: source.Token));
                Assert.IsFalse(File.Exists(path));
                Assert.AreEqual(0, Directory.GetFiles(_directory).Length);
            }
        }

        #endregion

        #region Entity

        [TestMethod]
        public void TestExportDataAsXmlOfAnEntityExportsTheTableOfTheEntityFilteredByTheExpression()
        {
            // Setup
            var source = Source();

            // Act
            var actual = source.ExportDataAsXml<Person>(NewPath(), where: p => p.Id > 1);

            // Assert
            var select = source.Executions.Single(e => e.Kind == "Reader");
            Assert.AreEqual(3L, actual.TotalRowCount);
            StringAssert.Contains(select.CommandText, "[Person]");
            StringAssert.Contains(select.CommandText, "WHERE");
            Assert.AreEqual(1, select.Parameters.Values.Single());
        }

        [TestMethod]
        public async Task TestExportDataAsXmlAsyncOfAnEntityWithATableNameExportsThatTable()
        {
            // Setup
            var source = Source();

            // Act
            await source.ExportDataAsXmlAsync<Person>("Customer", NewPath(), where: p => p.Id == 2);

            // Assert
            StringAssert.Contains(source.Executions.Single(e => e.Kind == "Reader").CommandText, "[Customer]");
        }

        [TestMethod]
        public void TestExportDataAsXmlOfAnEntityWithATableNameExportsThatTable()
        {
            // Setup
            var source = Source();

            // Act
            source.ExportDataAsXml<Person>("Customer", NewPath());

            // Assert
            StringAssert.Contains(source.Executions.Single(e => e.Kind == "Reader").CommandText, "[Customer]");
        }

        [TestMethod]
        public async Task TestExportDataAsXmlAsyncOfAnEntityExportsTheTableOfTheEntity()
        {
            // Setup
            var source = Source();

            // Act
            await source.ExportDataAsXmlAsync<Person>(NewPath());

            // Assert
            StringAssert.Contains(source.Executions.Single(e => e.Kind == "Reader").CommandText, "[Person]");
        }

        #endregion
    }
}
