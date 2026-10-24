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
using RepoDb.Data.Core.UnitTests.Fake.BulkOperations;
using RepoDb.Data.Enumerations;
using RepoDb.Data.Interfaces;
using RepoDb.Data.Models;

namespace RepoDb.Data.Core.UnitTests.Extensions
{
    [TestClass]
    public class ImportDataFromXmlTest
    {
        #region Helpers

        private readonly List<string> _files = new List<string>();

        private string Write(string text)
        {
            var path = Path.Combine(Path.GetTempPath(), "repodb_load_xml_" + Guid.NewGuid().ToString("N") + ".xml");
            File.WriteAllText(path, text, new UTF8Encoding(false));
            _files.Add(path);
            return path;
        }

        private static string Rows(int count) =>
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><rows>" +
            string.Join(string.Empty, Enumerable.Range(1, count).Select(i => "<row><Id>" + i + "</Id><Name>Name " + i + "</Name><Birthday>2026-01-02T03:04:05Z</Birthday></row>")) +
            "</rows>";

        private static Action<ImportProgress> Snapshot(List<(int Batch, long Loaded, long Total)> seen) =>
            p => seen.Add((p.BatchNumber, p.LoadedRowCount, p.TotalRowCount));

        private static IEnumerable<FakeExecution> Inserts(FakeDbConnection connection) =>
            connection.Executions.Where(e => e.CommandText.StartsWith("INSERT INTO", StringComparison.Ordinal));

        [TestInitialize]
        public void Initialize()
        {
            FakeBulkOperations.Reset();
        }

        [TestCleanup]
        public void Cleanup()
        {
            foreach (var file in _files.Where(File.Exists))
            {
                File.Delete(file);
            }
            _files.Clear();
        }

        #endregion

        #region Arguments

        [TestMethod]
        public void ThrowExceptionOnImportDataFromXmlIfTheConnectionIsNull()
        {
            // Setup
            var path = Write("<rows/>");

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => ((IDbConnection)null).ImportDataFromXml("Customer", path));
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("  ")]
        public void ThrowExceptionOnImportDataFromXmlIfTheFileNameIsNullOrWhiteSpace(string filename)
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => new FakeDbConnection().ImportDataFromXml("Customer", filename));
        }

        [TestMethod]
        public void ThrowExceptionOnImportDataFromXmlIfTheFileDoesNotExist()
        {
            // Act/Assert
            Assert.Throws<FileNotFoundException>(() => new FakeDbConnection().ImportDataFromXml("Customer", Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".xml")));
        }

        [TestMethod]
        public void ThrowExceptionOnImportDataFromXmlIfTheBufferSizeIsLessThanOne()
        {
            // Setup
            var path = Write("<rows/>");

            // Act/Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => new FakeDbConnection().ImportDataFromXml("Customer", path, bufferSize: 0));
        }

        [TestMethod]
        public async Task ThrowExceptionOnImportDataFromXmlAsyncIfTheFileDoesNotExist()
        {
            // Act/Assert
            await Assert.ThrowsAsync<FileNotFoundException>(() => new FakeDbConnection().ImportDataFromXmlAsync("Customer", Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".xml")));
        }

        #endregion

        #region Rows

        [TestMethod]
        public void TestImportDataFromXmlInsertsTheValuesConvertedIntoTheTypesOfTheColumns()
        {
            // Setup
            var path = Write("<rows><row><Id>7</Id><Name>Seven</Name><Birthday>2026-01-02T03:04:05Z</Birthday></row></rows>");
            var destination = new FakeDbConnection();

            // Act
            destination.ImportDataFromXml("Customer", path);

            // Assert
            var values = Inserts(destination).Single().Parameters.Values.ToArray();
            CollectionAssert.AreEquivalent(new object[] { 7, "Seven", new DateTime(2026, 1, 2, 3, 4, 5, DateTimeKind.Utc) }, values);
        }

        [TestMethod]
        public void TestImportDataFromXmlInsertsTheRowsInBatchesAndReportsTheProgress()
        {
            // Setup
            var path = Write(Rows(5));
            var destination = new FakeDbConnection();
            var seen = new List<(int Batch, long Loaded, long Total)>();

            // Act
            var actual = destination.ImportDataFromXml("Customer", path, bufferSize: 2, progressCallback: Snapshot(seen));

            // Assert
            Assert.AreEqual(5, Inserts(destination).Count());
            Assert.AreEqual(5L, actual.TotalRowCount);
            Assert.AreEqual(5L, actual.LoadedRowCount);
            Assert.AreEqual(3, actual.BatchNumber);
            CollectionAssert.AreEqual(new[] { (0, 0L, 5L), (1, 2L, 5L), (2, 4L, 5L), (3, 5L, 5L), (3, 5L, 5L) }, seen);
        }

        [TestMethod]
        public async Task TestImportDataFromXmlAsyncInsertsTheRowsInBatchesAndReportsTheProgress()
        {
            // Setup
            var path = Write(Rows(5));
            var destination = new FakeDbConnection();
            var seen = new List<(int Batch, long Loaded, long Total)>();

            // Act
            var actual = await destination.ImportDataFromXmlAsync("Customer", path, bufferSize: 2, progressCallback: Snapshot(seen));

            // Assert
            Assert.AreEqual(5, Inserts(destination).Count());
            Assert.AreEqual(5L, actual.LoadedRowCount);
            CollectionAssert.AreEqual(new[] { (0, 0L, 5L), (1, 2L, 5L), (2, 4L, 5L), (3, 5L, 5L), (3, 5L, 5L) }, seen);
        }

        [TestMethod]
        public void TestImportDataFromXmlIgnoresTheAttributes()
        {
            // Setup
            var path = Write("<rows><row Id=\"9\" Extra=\"x\"><Id>1</Id><Name>A</Name></row></rows>");
            var destination = new FakeDbConnection();

            // Act
            destination.ImportDataFromXml("Customer", path);

            // Assert
            var values = Inserts(destination).Single().Parameters.Values.ToArray();
            Assert.IsTrue(values.Contains(1));
            Assert.IsFalse(values.Contains(9));
        }

        [TestMethod]
        public void TestImportDataFromXmlInsertsAnEmptyElementAndAMissingElementAsNull()
        {
            // Setup
            var path = Write("<rows><row><Id>1</Id><Name/><Birthday></Birthday></row><row><Id>2</Id></row></rows>");
            var destination = new FakeDbConnection();

            // Act
            var actual = destination.ImportDataFromXml("Customer", path);

            // Assert
            Assert.AreEqual(2L, actual.LoadedRowCount);
            Assert.IsTrue(Inserts(destination).All(e => e.Parameters.Values.Count(v => v == null || v is DBNull) >= 2));
        }

        [TestMethod]
        public void TestImportDataFromXmlInsertsASelfClosingElementAsNullAndAnElementWithoutContentAsAnEmptyText()
        {
            // Setup
            var path = Write("<rows><row><Id>1</Id><Name></Name></row><row><Id>2</Id><Name /></row></rows>");
            var destination = new FakeDbConnection();

            // Act
            destination.ImportDataFromXml("Customer", path);

            // Assert
            var inserts = Inserts(destination).ToList();
            Assert.IsTrue(inserts[0].Parameters.Values.Contains(string.Empty));
            Assert.IsFalse(inserts[1].Parameters.Values.Any(v => v is string));
        }

        [TestMethod]
        public void TestImportDataFromXmlInsertsAnElementWithoutContentOfAColumnThatIsNotATextAsNull()
        {
            // Setup
            var path = Write("<rows><row><Id></Id><Name>A</Name><Birthday></Birthday></row></rows>");
            var destination = new FakeDbConnection();

            // Act
            var actual = destination.ImportDataFromXml("Customer", path);

            // Assert
            Assert.AreEqual(1L, actual.LoadedRowCount);
            Assert.IsFalse(Inserts(destination).Single().Parameters.Values.Any(v => v is int || v is DateTime));
        }

        [TestMethod]
        public void TestImportDataFromXmlGivesTheXmlOfTheContentOfAnElementThatHasChildElements()
        {
            // Setup
            var path = Write("<rows><row><Id>1</Id><Name>a <b>x</b> c</Name></row></rows>");
            var destination = new FakeDbConnection();

            // Act
            destination.ImportDataFromXml("Customer", path);

            // Assert
            Assert.IsTrue(Inserts(destination).Single().Parameters.Values.Contains("a <b>x</b> c"));
        }

        [TestMethod]
        public void TestImportDataFromXmlReadsTheEntitiesAndTheCdata()
        {
            // Setup
            var path = Write("<rows><row><Id>1</Id><Name>A &amp; B <![CDATA[<c>]]></Name></row></rows>");
            var destination = new FakeDbConnection();

            // Act
            destination.ImportDataFromXml("Customer", path);

            // Assert
            Assert.IsTrue(Inserts(destination).Single().Parameters.Values.Contains("A & B <c>"));
        }

        [TestMethod]
        public void TestImportDataFromXmlIgnoresTheColumnsOfTheFileThatTheTableDoesNotHave()
        {
            // Setup
            var path = Write("<rows><row><Id>1</Id><Name>A</Name><Unknown>z</Unknown></row></rows>");
            var destination = new FakeDbConnection();

            // Act
            var actual = destination.ImportDataFromXml("Customer", path);

            // Assert
            Assert.AreEqual(1L, actual.LoadedRowCount);
            Assert.IsFalse(Inserts(destination).Single().Parameters.Values.Contains("z"));
        }

        [TestMethod]
        public void TestImportDataFromXmlOfAFileWithoutRowsCallsTheCallbackTwiceAndInsertsNothing()
        {
            // Setup
            var path = Write("<rows/>");
            var destination = new FakeDbConnection();
            var seen = new List<(int Batch, long Loaded, long Total)>();

            // Act
            var actual = destination.ImportDataFromXml("Customer", path, progressCallback: Snapshot(seen));

            // Assert
            Assert.AreEqual(0L, actual.LoadedRowCount);
            Assert.AreEqual(2, seen.Count);
            Assert.AreEqual(0, Inserts(destination).Count());
        }

        [TestMethod]
        public void TestImportDataFromXmlReadsAFileWithACommentAndMarkupBetweenTheRows()
        {
            // Setup
            var path = Write("<?xml version=\"1.0\"?>\n<!-- rows -->\n<rows>\n  <!-- first -->\n  <row>\n    <Id>1</Id>\n    <Name>A</Name>\n  </row>\n  <row>\n    <Id>2</Id>\n    <Name>B</Name>\n  </row>\n</rows>\n");
            var destination = new FakeDbConnection();

            // Act
            var actual = destination.ImportDataFromXml("Customer", path);

            // Assert
            Assert.AreEqual(2L, actual.TotalRowCount);
            Assert.AreEqual(2L, actual.LoadedRowCount);
        }

        [TestMethod]
        public void ThrowExceptionOnImportDataFromXmlIfNoColumnOfTheFileMatchesAColumnOfTheTable()
        {
            // Setup
            var path = Write("<rows><row><Foo>1</Foo></row></rows>");

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => new FakeDbConnection().ImportDataFromXml("Customer", path));
        }

        [TestMethod]
        public void ThrowExceptionOnImportDataFromXmlIfAValueCannotBeConvertedIntoTheTypeOfItsColumn()
        {
            // Setup
            var path = Write("<rows><row><Id>abc</Id><Name>A</Name></row></rows>");

            // Act
            var exception = Assert.Throws<FormatException>(() => new FakeDbConnection().ImportDataFromXml("Customer", path));

            // Assert
            StringAssert.Contains(exception.Message, "Row 1");
        }

        [DataTestMethod]
        [DataRow("<rows><row><Id>1</Id>")]
        [DataRow("<rows><row><Id>1</Name></row></rows>")]
        [DataRow("not xml")]
        [DataRow("")]
        public void ThrowExceptionOnImportDataFromXmlIfTheFileIsNotValidXml(string text)
        {
            // Setup
            var path = Write(text);

            // Act/Assert
            Assert.Throws<FormatException>(() => new FakeDbConnection().ImportDataFromXml("Customer", path));
        }

        [TestMethod]
        public async Task ThrowExceptionOnImportDataFromXmlAsyncIfTheFileIsNotValidXml()
        {
            // Setup
            var path = Write("<rows><row><Id>1</Id>");

            // Act/Assert
            await Assert.ThrowsAsync<FormatException>(() => new FakeDbConnection().ImportDataFromXmlAsync("Customer", path));
        }

        [TestMethod]
        public async Task ThrowExceptionOnImportDataFromXmlAsyncIfTheTokenIsCancelled()
        {
            // Setup
            var path = Write(Rows(3));
            using (var source = new CancellationTokenSource())
            {
                source.Cancel();

                // Act/Assert
                await Assert.ThrowsAsync<OperationCanceledException>(() => new FakeDbConnection().ImportDataFromXmlAsync("Customer", path, cancellationToken: source.Token));
            }
        }

        #endregion

        #region Bulk Operations

        [TestMethod]
        public void TestImportDataFromXmlUsesTheBulkInsertInBlocksOfTheBufferSize()
        {
            // Setup
            var path = Write(Rows(5));

            // Act
            var actual = FakeBulkDestination.Create().ImportDataFromXml("Customer", path, bufferSize: 2);

            // Assert
            CollectionAssert.AreEqual(new[] { 2, 2, 1 }, FakeBulkOperations.Calls.Select(c => c.RowCount).ToArray());
            Assert.IsTrue(FakeBulkOperations.Calls.All(c => c.Method == "BulkInsert"));
            Assert.AreEqual(5L, actual.LoadedRowCount);
        }

        [TestMethod]
        public async Task TestImportDataFromXmlAsyncWithBypassExistingRowsBulkMergesTheRows()
        {
            // Setup
            var path = Write(Rows(3));

            // Act
            var actual = await FakeBulkDestination.Create().ImportDataFromXmlAsync("Customer", path, bypassExistingRows: true);

            // Assert
            var call = FakeBulkOperations.Calls.Single();
            Assert.AreEqual("BulkMergeAsync", call.Method);
            CollectionAssert.AreEqual(new[] { "Id" }, call.Qualifiers);
            Assert.AreEqual(3L, actual.LoadedRowCount);
        }

        [TestMethod]
        public void TestImportDataFromXmlWithBypassExistingRowsBulkMergesTheRows()
        {
            // Setup
            var path = Write(Rows(3));

            // Act
            FakeBulkDestination.Create().ImportDataFromXml("Customer", path, bypassExistingRows: true);

            // Assert
            var call = FakeBulkOperations.Calls.Single();
            Assert.AreEqual("BulkMerge", call.Method);
            CollectionAssert.AreEqual(new[] { "Id" }, call.Qualifiers);
        }

        [TestMethod]
        public void ThrowExceptionOnImportDataFromXmlWithBypassExistingRowsIfTheTableHasNoPrimaryKeyAndNoIdentity()
        {
            // Setup
            var path = Write(Rows(2));

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => FakeBulkDestination.CreateKeyless().ImportDataFromXml("Customer", path, bypassExistingRows: true));
            Assert.AreEqual(0, FakeBulkOperations.Calls.Length);
        }

        [TestMethod]
        public void TestImportDataFromXmlWithQualifiersBulkMergesTheRowsWithTheQualifiers()
        {
            // Setup
            var path = Write(Rows(3));

            // Act
            FakeBulkDestination.Create().ImportDataFromXml("Customer", path, bypassExistingRows: true, qualifiers: new[] { "Name" });

            // Assert
            var call = FakeBulkOperations.Calls.Single();
            Assert.AreEqual("BulkMerge", call.Method);
            CollectionAssert.AreEqual(new[] { "Name" }, call.Qualifiers);
        }

        [TestMethod]
        public async Task TestImportDataFromXmlAsyncWithQualifiersBulkMergesTheRowsWithTheQualifiers()
        {
            // Setup
            var path = Write(Rows(3));

            // Act
            await FakeBulkDestination.Create().ImportDataFromXmlAsync("Customer", path, bypassExistingRows: true, qualifiers: new[] { "Id", "Name" });

            // Assert
            var call = FakeBulkOperations.Calls.Single();
            Assert.AreEqual("BulkMergeAsync", call.Method);
            CollectionAssert.AreEqual(new[] { "Id", "Name" }, call.Qualifiers);
        }

        [TestMethod]
        public void TestImportDataFromXmlWithQualifiersMergesTheRowsOfATableWithoutPrimaryKeyAndIdentity()
        {
            // Setup
            var path = Write(Rows(3));

            // Act
            FakeBulkDestination.CreateKeyless().ImportDataFromXml("Customer", path, bypassExistingRows: true, qualifiers: new[] { "Id" });

            // Assert
            CollectionAssert.AreEqual(new[] { "Id" }, FakeBulkOperations.Calls.Single().Qualifiers);
        }

        [TestMethod]
        public void TestImportDataFromXmlWithQualifiersResolvesTheNamesOfTheColumnsWithoutTheCaseAndWithoutRepeatingThem()
        {
            // Setup
            var path = Write(Rows(3));

            // Act
            FakeBulkDestination.Create().ImportDataFromXml("Customer", path, bypassExistingRows: true, qualifiers: new[] { "name", "NAME", "[Name]" });

            // Assert
            CollectionAssert.AreEqual(new[] { "Name" }, FakeBulkOperations.Calls.Single().Qualifiers);
        }

        [TestMethod]
        public void TestImportDataFromXmlIgnoresTheQualifiersIfTheExistingRowsAreNotBypassed()
        {
            // Setup
            var path = Write(Rows(3));

            // Act
            FakeBulkDestination.Create().ImportDataFromXml("Customer", path, qualifiers: new[] { "Name" });

            // Assert
            var call = FakeBulkOperations.Calls.Single();
            Assert.AreEqual("BulkInsert", call.Method);
            Assert.IsNull(call.Qualifiers);
        }

        [TestMethod]
        public void ThrowExceptionOnImportDataFromXmlWithQualifiersIfAQualifierIsNotAColumnOfTheTable()
        {
            // Setup
            var path = Write(Rows(3));

            // Act/Assert
            Assert.Throws<ArgumentException>(() => FakeBulkDestination.Create().ImportDataFromXml("Customer", path, bypassExistingRows: true, qualifiers: new[] { "Missing" }));
            Assert.AreEqual(0, FakeBulkOperations.Calls.Length);
        }

        [TestMethod]
        public void ThrowExceptionOnImportDataFromXmlWithQualifiersIfAQualifierIsEmpty()
        {
            // Setup
            var path = Write(Rows(3));

            // Act/Assert
            Assert.Throws<ArgumentException>(() => FakeBulkDestination.Create().ImportDataFromXml("Customer", path, bypassExistingRows: true, qualifiers: new[] { "Id", " " }));
        }

        [TestMethod]
        public void ThrowExceptionOnImportDataFromXmlWithQualifiersIfAQualifierIsNotAColumnOfTheFile()
        {
            // Setup
            var path = Write("<rows><row><Id>1</Id><Name>A</Name></row></rows>");

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => FakeBulkDestination.Create().ImportDataFromXml("Customer", path, bypassExistingRows: true, qualifiers: new[] { "Birthday" }));
        }

        [TestMethod]
        public void TestImportDataFromXmlWithBypassExistingRowsMergesTheRowsInBatchesIfTheProviderHasNoBulkMerge()
        {
            // Setup
            var path = Write(Rows(3));
            var destination = new FakeDbConnection();

            // Act
            var actual = destination.ImportDataFromXml("Customer", path, bypassExistingRows: true, qualifiers: new[] { "Name" });

            // Assert
            var updates = destination.Executions.Where(e => e.CommandText.StartsWith("UPDATE", StringComparison.Ordinal)).ToList();
            Assert.AreEqual(3, updates.Count);
            StringAssert.Contains(updates[0].CommandText, "WHERE [Name] = ");
            Assert.IsFalse(updates[0].CommandText.Contains("SET [Name]"));
            Assert.AreEqual(0, destination.Executions.Count(e => e.CommandText.StartsWith("INSERT INTO", StringComparison.Ordinal)));
            Assert.AreEqual(0, FakeBulkOperations.Calls.Length);
            Assert.AreEqual(3L, actual.TotalRowCount);
        }

        [TestMethod]
        public async Task TestImportDataFromXmlAsyncWithBypassExistingRowsMergesTheRowsInBatchesIfTheProviderHasNoBulkMerge()
        {
            // Setup
            var path = Write(Rows(3));
            var destination = new FakeDbConnection();

            // Act
            await destination.ImportDataFromXmlAsync("Customer", path, bypassExistingRows: true);

            // Assert
            var update = destination.Executions.First(e => e.CommandText.StartsWith("UPDATE", StringComparison.Ordinal));
            StringAssert.Contains(update.CommandText, "WHERE [Id] = ");
            StringAssert.Contains(update.CommandText, "SET [Name] = ");
        }

        [TestMethod]
        public void TestImportDataFromXmlWithMappingsLoadsTheMappedColumnsOnly()
        {
            // Setup
            var path = Write("<rows><row><Code>7</Code><Title>Seven</Title><Extra>x</Extra></row></rows>");
            var destination = new FakeDbConnection();
            var mappings = new[] { new ColumnMapping("Code", "Id"), new ColumnMapping("Title", "Name") };

            // Act
            var actual = destination.ImportDataFromXml("Customer", path, mappings: mappings);

            // Assert
            var insert = destination.Executions.Single(e => e.CommandText.StartsWith("INSERT INTO", StringComparison.Ordinal));
            CollectionAssert.AreEquivalent(new object[] { 7, "Seven" }, insert.Parameters.Values.ToArray());
            Assert.AreEqual(1L, actual.LoadedRowCount);
        }

        [TestMethod]
        public async Task TestImportDataFromXmlAsyncWithMappingsLoadsTheMappedColumnsOnly()
        {
            // Setup
            var path = Write("<rows><row><Code>7</Code><Title>Seven</Title><Extra>x</Extra></row></rows>");
            var destination = new FakeDbConnection();
            var mappings = new[] { new ColumnMapping("title", "name"), new ColumnMapping("CODE", "ID") };

            // Act
            await destination.ImportDataFromXmlAsync("Customer", path, mappings: mappings);

            // Assert
            var insert = destination.Executions.Single(e => e.CommandText.StartsWith("INSERT INTO", StringComparison.Ordinal));
            CollectionAssert.AreEquivalent(new object[] { 7, "Seven" }, insert.Parameters.Values.ToArray());
        }

        [TestMethod]
        public void TestImportDataFromXmlWithMappingsDoesNotLoadAColumnOfTheFileThatHasTheNameOfAColumnOfTheTableButIsNotMapped()
        {
            // Setup
            var path = Write("<rows><row><Code>7</Code><Id>8</Id><Name>Same</Name></row></rows>");
            var destination = new FakeDbConnection();

            // Act
            destination.ImportDataFromXml("Customer", path, mappings: new[] { new ColumnMapping("Code", "Id") });

            // Assert
            var insert = destination.Executions.Single(e => e.CommandText.StartsWith("INSERT INTO", StringComparison.Ordinal));
            CollectionAssert.AreEquivalent(new object[] { 7 }, insert.Parameters.Values.ToArray());
        }

        [TestMethod]
        public void TestImportDataFromXmlWithMappingsLoadsAColumnOfTheFileIntoMoreThanOneColumnOfTheTable()
        {
            // Setup
            var path = Write("<rows><row><Code>7</Code><Title>Seven</Title><Extra>x</Extra></row></rows>");
            var destination = new FakeDbConnection();
            var mappings = new[] { new ColumnMapping("Code", "Id"), new ColumnMapping("Code", "Name") };

            // Act
            destination.ImportDataFromXml("Customer", path, mappings: mappings);

            // Assert
            var insert = destination.Executions.Single(e => e.CommandText.StartsWith("INSERT INTO", StringComparison.Ordinal));
            CollectionAssert.AreEquivalent(new object[] { 7, "7" }, insert.Parameters.Values.ToArray());
        }

        [TestMethod]
        public void TestImportDataFromXmlWithEmptyMappingsLoadsTheColumnsByTheirNames()
        {
            // Setup
            var path = Write("<rows><row><Id>7</Id><Name>Same</Name></row></rows>");
            var destination = new FakeDbConnection();

            // Act
            destination.ImportDataFromXml("Customer", path, mappings: new List<ColumnMapping>());

            // Assert
            var insert = destination.Executions.Single(e => e.CommandText.StartsWith("INSERT INTO", StringComparison.Ordinal));
            CollectionAssert.AreEquivalent(new object[] { 7, "Same" }, insert.Parameters.Values.ToArray());
        }

        [TestMethod]
        public void ThrowExceptionOnImportDataFromXmlWithMappingsIfADestinationColumnIsNotAColumnOfTheTable()
        {
            // Setup
            var path = Write("<rows><row><Code>7</Code><Title>Seven</Title><Extra>x</Extra></row></rows>");

            // Act/Assert
            Assert.Throws<ArgumentException>(() => new FakeDbConnection().ImportDataFromXml("Customer", path, mappings: new[] { new ColumnMapping("Code", "Missing") }));
        }

        [TestMethod]
        public void ThrowExceptionOnImportDataFromXmlWithMappingsIfADestinationColumnIsMappedMoreThanOnce()
        {
            // Setup
            var path = Write("<rows><row><Code>7</Code><Title>Seven</Title><Extra>x</Extra></row></rows>");
            var mappings = new[] { new ColumnMapping("Code", "Id"), new ColumnMapping("Title", "id") };

            // Act/Assert
            Assert.Throws<ArgumentException>(() => new FakeDbConnection().ImportDataFromXml("Customer", path, mappings: mappings));
        }

        [TestMethod]
        public void ThrowExceptionOnImportDataFromXmlWithMappingsIfAMappingIsNull()
        {
            // Setup
            var path = Write("<rows><row><Code>7</Code><Title>Seven</Title><Extra>x</Extra></row></rows>");

            // Act/Assert
            Assert.Throws<ArgumentException>(() => new FakeDbConnection().ImportDataFromXml("Customer", path, mappings: new ColumnMapping[] { null }));
        }

        [TestMethod]
        public void ThrowExceptionOnImportDataFromXmlWithMappingsIfAMappingHasAnEmptyColumn()
        {
            // Setup
            var path = Write("<rows><row><Code>7</Code><Title>Seven</Title><Extra>x</Extra></row></rows>");

            // Act/Assert
            Assert.Throws<ArgumentException>(() => new FakeDbConnection().ImportDataFromXml("Customer", path, mappings: new[] { new ColumnMapping(" ", "Id") }));
        }

        [TestMethod]
        public void TestImportDataFromXmlWithMappingsLoadsNullIfAMappedColumnIsNotInTheFile()
        {
            // Setup
            var path = Write("<rows><row><Code>7</Code><Title>Seven</Title><Extra>x</Extra></row></rows>");
            var destination = new FakeDbConnection();
            var mappings = new[] { new ColumnMapping("Code", "Id"), new ColumnMapping("Missing", "Name") };

            // Act
            var actual = destination.ImportDataFromXml("Customer", path, mappings: mappings);

            // Assert
            Assert.AreEqual(1L, actual.LoadedRowCount);
        }

        [TestMethod]
        public void TestImportDataFromXmlWithMappingsAndBypassExistingRowsBulkMergesTheRowsWithTheMappedQualifier()
        {
            // Setup
            var path = Write("<rows><row><Code>7</Code><Title>Seven</Title><Extra>x</Extra></row></rows>");
            var mappings = new[] { new ColumnMapping("Code", "Id"), new ColumnMapping("Title", "Name") };

            // Act
            FakeBulkDestination.Create().ImportDataFromXml("Customer", path, bypassExistingRows: true, mappings: mappings);

            // Assert
            var call = FakeBulkOperations.Calls.Single();
            Assert.AreEqual("BulkMerge", call.Method);
            CollectionAssert.AreEqual(new[] { "Id" }, call.Qualifiers);
            CollectionAssert.AreEqual(new[] { "Id", "Name" }, FakeBulkOperations.LastColumns);
        }

        [TestMethod]
        public void ThrowExceptionOnImportDataFromXmlWithMappingsAndBypassExistingRowsIfTheMappingsDoNotLoadTheQualifier()
        {
            // Setup
            var path = Write("<rows><row><Code>7</Code><Title>Seven</Title><Extra>x</Extra></row></rows>");

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => FakeBulkDestination.Create().ImportDataFromXml("Customer", path, bypassExistingRows: true, mappings: new[] { new ColumnMapping("Title", "Name") }));
            Assert.AreEqual(0, FakeBulkOperations.Calls.Length);
        }

        [TestMethod]
        public void TestImportDataFromXmlWithMappingsLeavesTheIdentityColumnToTheDatabaseIfItIsNotToBeKept()
        {
            // Setup
            var path = Write("<rows><row><Code>7</Code><Title>Seven</Title><Extra>x</Extra></row></rows>");
            var mappings = new[] { new ColumnMapping("Code", "Id"), new ColumnMapping("Title", "Name") };

            // Act
            FakeBulkDestination.Create().ImportDataFromXml("Customer", path, mappings: mappings, identityBehavior: IdentityBehavior.Ignore);

            // Assert
            CollectionAssert.AreEqual(new[] { "Name" }, FakeBulkOperations.LastColumns);
        }

        #region Data Interceptors

        private static IList<IDataInterceptor<Row>> Interceptors(params Func<Row, Row>[] transforms) =>
            transforms.Select(transform => (IDataInterceptor<Row>)new FakeDataInterceptor(transform)).ToList();

        private static List<object> InsertedValues(FakeDbConnection connection) =>
            connection.Executions.Where(e => e.CommandText.StartsWith("INSERT INTO", StringComparison.Ordinal)).SelectMany(e => e.Parameters.Values).ToList();

        [TestMethod]
        public void TestImportDataFromXmlLoadsTheRowsThatTheInterceptorsGiveInOrder()
        {
            // Setup
            var path = Write(Rows(3));
            var destination = new FakeDbConnection();
            var interceptors = Interceptors(
                row => { row["Name"] = ((string)row["Name"]).ToUpperInvariant(); return row; },
                row => { row["Name"] = row["Name"] + "!"; return row; });

            // Act
            var actual = destination.ImportDataFromXml("Customer", path, dataInterceptors: interceptors);

            // Assert
            CollectionAssert.AreEquivalent(new[] { "NAME 1!", "NAME 2!", "NAME 3!" }, InsertedValues(destination).OfType<string>().ToList());
            Assert.AreEqual(3L, actual.LoadedRowCount);
        }

        [TestMethod]
        public async Task TestImportDataFromXmlAsyncLoadsTheRowsThatTheInterceptorsGive()
        {
            // Setup
            var path = Write(Rows(2));
            var destination = new FakeDbConnection();
            var interceptors = Interceptors(row => { row["Name"] = "Changed"; return row; });

            // Act
            await destination.ImportDataFromXmlAsync("Customer", path, dataInterceptors: interceptors);

            // Assert
            CollectionAssert.AreEqual(new[] { "Changed", "Changed" }, InsertedValues(destination).OfType<string>().ToList());
        }

        [TestMethod]
        public void TestImportDataFromXmlGivesTheInterceptorsTheValuesConvertedIntoTheTypesOfTheColumns()
        {
            // Setup
            var path = Write(Rows(2));
            var seen = new List<Type>();
            var interceptors = Interceptors(row => { seen.Add(row["Id"].GetType()); return row; });

            // Act
            new FakeDbConnection().ImportDataFromXml("Customer", path, dataInterceptors: interceptors);

            // Assert
            CollectionAssert.AreEqual(new[] { typeof(int), typeof(int) }, seen);
        }

        [TestMethod]
        public void TestImportDataFromXmlGivesTheInterceptorsTheColumnsOfTheTableThatTheMappingsGive()
        {
            // Setup
            var path = Write("<rows><row><Code>7</Code><Title>Seven</Title></row></rows>");
            var names = new List<string>();
            var interceptors = Interceptors(row => { names.AddRange(row.Columns.Select(c => c.Name)); return row; });
            var mappings = new[] { new ColumnMapping("Code", "Id"), new ColumnMapping("Title", "Name") };

            // Act
            new FakeDbConnection().ImportDataFromXml("Customer", path, mappings, dataInterceptors: interceptors);

            // Assert
            CollectionAssert.AreEqual(new[] { "Id", "Name" }, names);
        }

        [TestMethod]
        public void TestImportDataFromXmlConvertsTheValuesThatTheInterceptorsGive()
        {
            // Setup
            var path = Write(Rows(1));
            var destination = new FakeDbConnection();
            var interceptors = Interceptors(row => { row["Id"] = "42"; return row; });

            // Act
            destination.ImportDataFromXml("Customer", path, dataInterceptors: interceptors);

            // Assert
            Assert.IsTrue(InsertedValues(destination).Contains(42));
        }

        [TestMethod]
        public void TestImportDataFromXmlBulkInsertsTheColumnsThatTheInterceptorsGive()
        {
            // Setup
            var path = Write(Rows(2));
            var interceptors = Interceptors(row => { row.Remove("Birthday"); return row; });

            // Act
            FakeBulkDestination.Create().ImportDataFromXml("Customer", path, dataInterceptors: interceptors);

            // Assert
            CollectionAssert.AreEqual(new[] { "Id", "Name" }, FakeBulkOperations.LastColumns);
            Assert.AreEqual(2, FakeBulkOperations.Calls.Single().RowCount);
        }

        [TestMethod]
        public void TestImportDataFromXmlLeavesOutTheColumnsThatTheInterceptorsAddAndTheTableDoesNotHave()
        {
            // Setup
            var path = Write(Rows(1));
            var interceptors = Interceptors(row => row.Add("Extra", typeof(string), "e"));

            // Act
            FakeBulkDestination.Create().ImportDataFromXml("Customer", path, dataInterceptors: interceptors);

            // Assert
            CollectionAssert.AreEqual(new[] { "Id", "Name", "Birthday" }, FakeBulkOperations.LastColumns);
        }

        [TestMethod]
        public void TestImportDataFromXmlLeavesTheIdentityColumnOutAfterTheInterceptorsIfItIsNotToBeKept()
        {
            // Setup
            var path = Write(Rows(1));
            var interceptors = Interceptors(row => { row["Id"] = 500; return row; });

            // Act
            FakeBulkDestination.Create().ImportDataFromXml("Customer", path, identityBehavior: IdentityBehavior.Ignore, dataInterceptors: interceptors);

            // Assert
            CollectionAssert.AreEqual(new[] { "Name", "Birthday" }, FakeBulkOperations.LastColumns);
        }

        [TestMethod]
        public void TestImportDataFromXmlWithInterceptorsAndBypassExistingRowsBulkMergesTheRowsWithTheQualifiers()
        {
            // Setup
            var path = Write(Rows(2));
            var interceptors = Interceptors(row => { row["Name"] = "Changed"; return row; });

            // Act
            FakeBulkDestination.Create().ImportDataFromXml("Customer", path, bypassExistingRows: true, dataInterceptors: interceptors);

            // Assert
            var call = FakeBulkOperations.Calls.Single();
            Assert.AreEqual("BulkMerge", call.Method);
            CollectionAssert.AreEqual(new[] { "Id" }, call.Qualifiers);
        }

        [TestMethod]
        public void ThrowExceptionOnImportDataFromXmlWithBypassExistingRowsIfAnInterceptorRemovesAQualifier()
        {
            // Setup
            var path = Write(Rows(2));
            var interceptors = Interceptors(row => { row.Remove("Id"); return row; });

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => FakeBulkDestination.Create().ImportDataFromXml("Customer", path, bypassExistingRows: true, dataInterceptors: interceptors));
            Assert.AreEqual(0, FakeBulkOperations.Calls.Length);
        }

        [TestMethod]
        public void ThrowExceptionOnImportDataFromXmlIfAnInterceptorReturnsNull()
        {
            // Setup
            var path = Write(Rows(1));

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => new FakeDbConnection().ImportDataFromXml("Customer", path, dataInterceptors: Interceptors(row => null)));
        }

        [TestMethod]
        public void ThrowExceptionOnImportDataFromXmlIfAnInterceptorIsNull()
        {
            // Setup
            var path = Write(Rows(1));

            // Act/Assert
            Assert.Throws<ArgumentException>(() => new FakeDbConnection().ImportDataFromXml("Customer", path, dataInterceptors: new List<IDataInterceptor<Row>> { null }));
        }

        [TestMethod]
        public void ThrowExceptionOnImportDataFromXmlIfAnInterceptorGivesAnotherShapeToAnotherRow()
        {
            // Setup
            var path = Write(Rows(3));
            var interceptors = Interceptors(row => (int)row["Id"] == 2 ? row.Remove("Name") ? row : row : row);

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => new FakeDbConnection().ImportDataFromXml("Customer", path, dataInterceptors: interceptors));
        }

        [TestMethod]
        public void ThrowExceptionOnImportDataFromXmlIfNoColumnOfTheRowsThatTheInterceptorsGiveMatchesAColumnOfTheTable()
        {
            // Setup
            var path = Write(Rows(1));
            var interceptors = Interceptors(row => new Row(new[] { new Column("Unknown", typeof(int)) }, new object[] { 1 }));

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => new FakeDbConnection().ImportDataFromXml("Customer", path, dataInterceptors: interceptors));
        }

        [TestMethod]
        public void TestImportDataFromXmlWithEmptyInterceptorsLoadsTheRowsAsTheyAre()
        {
            // Setup
            var path = Write(Rows(2));
            var destination = new FakeDbConnection();

            // Act
            destination.ImportDataFromXml("Customer", path, dataInterceptors: new List<IDataInterceptor<Row>>());

            // Assert
            CollectionAssert.AreEquivalent(new[] { "Name 1", "Name 2" }, InsertedValues(destination).OfType<string>().ToList());
        }

        #endregion

        #endregion
    }
}
