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
using RepoDb.Data.Models;

namespace RepoDb.Data.Core.UnitTests.Extensions
{
    [TestClass]
    public class ImportDataFromCsvTest
    {
        #region Helpers

        private readonly List<string> _files = new List<string>();

        private string Write(string text, Encoding encoding = null)
        {
            var path = Path.Combine(Path.GetTempPath(), "repodb_load_csv_" + Guid.NewGuid().ToString("N") + ".csv");
            File.WriteAllText(path, text, encoding ?? new UTF8Encoding(false));
            _files.Add(path);
            return path;
        }

        private static string Rows(int count, string separator = ",") =>
            string.Join(separator, "Id", "Name", "Birthday") + "\n" +
            string.Join("\n", Enumerable.Range(1, count).Select(i => string.Join(separator, i, "Name " + i, "2026-01-02T03:04:05Z")));

        private static Action<LoadProgress> Snapshot(List<(int Batch, long Loaded, long Total)> seen) =>
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
        public void ThrowExceptionOnImportDataFromCsvIfTheConnectionIsNull()
        {
            // Setup
            var path = Write("Id\n1");

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => ((IDbConnection)null).ImportDataFromCsv("Customer", path));
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("  ")]
        public void ThrowExceptionOnImportDataFromCsvIfTheFileNameIsNullOrWhiteSpace(string filename)
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => new FakeDbConnection().ImportDataFromCsv("Customer", filename));
        }

        [TestMethod]
        public void ThrowExceptionOnImportDataFromCsvIfTheFileDoesNotExist()
        {
            // Act/Assert
            Assert.Throws<FileNotFoundException>(() => new FakeDbConnection().ImportDataFromCsv("Customer", Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".csv")));
        }

        [TestMethod]
        public void ThrowExceptionOnImportDataFromCsvIfTheBufferSizeIsLessThanOne()
        {
            // Setup
            var path = Write(Rows(1));

            // Act/Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => new FakeDbConnection().ImportDataFromCsv("Customer", path, bufferSize: 0));
        }

        [DataTestMethod]
        [DataRow('"')]
        [DataRow('\r')]
        [DataRow('\n')]
        public void ThrowExceptionOnImportDataFromCsvIfTheSeparatorIsAQuoteOrALineBreak(char separator)
        {
            // Setup
            var path = Write(Rows(1));

            // Act/Assert
            Assert.Throws<ArgumentException>(() => new FakeDbConnection().ImportDataFromCsv("Customer", path, separator: separator));
        }

        [TestMethod]
        public async Task ThrowExceptionOnImportDataFromCsvAsyncIfTheSeparatorIsAQuote()
        {
            // Setup
            var path = Write(Rows(1));

            // Act/Assert
            await Assert.ThrowsAsync<ArgumentException>(() => new FakeDbConnection().ImportDataFromCsvAsync("Customer", path, separator: '"'));
        }

        #endregion

        #region Header

        [DataTestMethod]
        [DataRow("")]
        [DataRow("\n")]
        [DataRow("\r\n\r\n")]
        [DataRow(",,\n1,2,3")]
        public void ThrowExceptionOnImportDataFromCsvIfTheFileHasNoHeader(string text)
        {
            // Setup
            var path = Write(text);

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => new FakeDbConnection().ImportDataFromCsv("Customer", path));
        }

        [TestMethod]
        public async Task ThrowExceptionOnImportDataFromCsvAsyncIfTheFileHasNoHeader()
        {
            // Setup
            var path = Write(string.Empty);

            // Act/Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => new FakeDbConnection().ImportDataFromCsvAsync("Customer", path));
        }

        [TestMethod]
        public void TestImportDataFromCsvOfAFileWithOnlyTheHeaderCallsTheCallbackTwiceAndInsertsNothing()
        {
            // Setup
            var path = Write("Id,Name,Birthday\n");
            var destination = new FakeDbConnection();
            var seen = new List<(int Batch, long Loaded, long Total)>();

            // Act
            var actual = destination.ImportDataFromCsv("Customer", path, progressCallback: Snapshot(seen));

            // Assert
            Assert.AreEqual(0L, actual.TotalRowCount);
            Assert.AreEqual(2, seen.Count);
            Assert.AreEqual(0, Inserts(destination).Count());
        }

        [TestMethod]
        public void TestImportDataFromCsvMatchesTheHeaderWithoutTheCase()
        {
            // Setup
            var path = Write("id,NAME\n1,A");
            var destination = new FakeDbConnection();

            // Act
            var actual = destination.ImportDataFromCsv("Customer", path);

            // Assert
            Assert.AreEqual(1L, actual.LoadedRowCount);
            Assert.IsTrue(Inserts(destination).Single().Parameters.Values.Contains("A"));
        }

        [TestMethod]
        public void TestImportDataFromCsvIgnoresTheColumnsOfTheFileThatTheTableDoesNotHave()
        {
            // Setup
            var path = Write("Id,Unknown,Name\n1,z,A");
            var destination = new FakeDbConnection();

            // Act
            destination.ImportDataFromCsv("Customer", path);

            // Assert
            var values = Inserts(destination).Single().Parameters.Values.ToArray();
            Assert.IsFalse(values.Contains("z"));
            Assert.IsTrue(values.Contains("A"));
        }

        [TestMethod]
        public void ThrowExceptionOnImportDataFromCsvIfNoColumnOfTheHeaderMatchesAColumnOfTheTable()
        {
            // Setup
            var path = Write("Foo,Bar\n1,2");

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => new FakeDbConnection().ImportDataFromCsv("Customer", path));
        }

        #endregion

        #region Rows

        [TestMethod]
        public void TestImportDataFromCsvInsertsTheValuesConvertedIntoTheTypesOfTheColumns()
        {
            // Setup
            var path = Write("Id,Name,Birthday\n7,Seven,2026-01-02T03:04:05Z");
            var destination = new FakeDbConnection();

            // Act
            destination.ImportDataFromCsv("Customer", path);

            // Assert
            var values = Inserts(destination).Single().Parameters.Values.ToArray();
            CollectionAssert.AreEquivalent(new object[] { 7, "Seven", new DateTime(2026, 1, 2, 3, 4, 5, DateTimeKind.Utc) }, values);
        }

        [TestMethod]
        public void TestImportDataFromCsvInsertsTheRowsInBatchesAndReportsTheProgress()
        {
            // Setup
            var path = Write(Rows(5));
            var destination = new FakeDbConnection();
            var seen = new List<(int Batch, long Loaded, long Total)>();

            // Act
            var actual = destination.ImportDataFromCsv("Customer", path, bufferSize: 2, progressCallback: Snapshot(seen));

            // Assert
            Assert.AreEqual(5, Inserts(destination).Count());
            Assert.AreEqual(5L, actual.TotalRowCount);
            Assert.AreEqual(5L, actual.LoadedRowCount);
            Assert.AreEqual(3, actual.BatchNumber);
            CollectionAssert.AreEqual(new[] { (0, 0L, 5L), (1, 2L, 5L), (2, 4L, 5L), (3, 5L, 5L), (3, 5L, 5L) }, seen);
        }

        [TestMethod]
        public async Task TestImportDataFromCsvAsyncInsertsTheRowsInBatchesAndReportsTheProgress()
        {
            // Setup
            var path = Write(Rows(5));
            var destination = new FakeDbConnection();
            var seen = new List<(int Batch, long Loaded, long Total)>();

            // Act
            var actual = await destination.ImportDataFromCsvAsync("Customer", path, bufferSize: 2, progressCallback: Snapshot(seen));

            // Assert
            Assert.AreEqual(5, Inserts(destination).Count());
            CollectionAssert.AreEqual(new[] { (0, 0L, 5L), (1, 2L, 5L), (2, 4L, 5L), (3, 5L, 5L), (3, 5L, 5L) }, seen);
            Assert.AreEqual(5L, actual.LoadedRowCount);
        }

        [DataTestMethod]
        [DataRow(';')]
        [DataRow('\t')]
        [DataRow('|')]
        public void TestImportDataFromCsvSplitsTheFieldsBySeparator(char separator)
        {
            // Setup
            var path = Write(Rows(2, separator.ToString()));
            var destination = new FakeDbConnection();

            // Act
            var actual = destination.ImportDataFromCsv("Customer", path, separator: separator);

            // Assert
            Assert.AreEqual(2L, actual.LoadedRowCount);
            Assert.IsTrue(Inserts(destination).Any(e => e.Parameters.Values.Contains("Name 2")));
        }

        [TestMethod]
        public void TestImportDataFromCsvDoesNotSplitTheFieldsByTheDefaultSeparatorIfAnotherOneIsGiven()
        {
            // Setup
            var path = Write("Id;Name\n1;a,b");
            var destination = new FakeDbConnection();

            // Act
            destination.ImportDataFromCsv("Customer", path, separator: ';');

            // Assert
            Assert.IsTrue(Inserts(destination).Single().Parameters.Values.Contains("a,b"));
        }

        [TestMethod]
        public void TestImportDataFromCsvReadsTheQuotedFields()
        {
            // Setup
            var path = Write("Id,Name\n1,\"a,b \"\"q\"\"\nline 2\"\n2,\"plain\"");
            var destination = new FakeDbConnection();

            // Act
            var actual = destination.ImportDataFromCsv("Customer", path);

            // Assert
            Assert.AreEqual(2L, actual.LoadedRowCount);
            var values = Inserts(destination).SelectMany(e => e.Parameters.Values).ToArray();
            Assert.IsTrue(values.Contains("a,b \"q\"\nline 2"));
            Assert.IsTrue(values.Contains("plain"));
        }

        [TestMethod]
        public void TestImportDataFromCsvInsertsAnUnquotedEmptyFieldAsNullAndAQuotedEmptyFieldAsAnEmptyText()
        {
            // Setup
            var path = Write("Id,Name,Birthday\n1,,\n2,\"\",");
            var destination = new FakeDbConnection();

            // Act
            destination.ImportDataFromCsv("Customer", path);

            // Assert
            var inserts = Inserts(destination).ToList();
            Assert.IsFalse(inserts[0].Parameters.Values.Contains(string.Empty));
            Assert.IsTrue(inserts[1].Parameters.Values.Contains(string.Empty));
        }

        [TestMethod]
        public void TestImportDataFromCsvInsertsTheMissingFieldsOfAShortRowAsNull()
        {
            // Setup
            var path = Write("Id,Name,Birthday\n1,A\n2");
            var destination = new FakeDbConnection();

            // Act
            var actual = destination.ImportDataFromCsv("Customer", path);

            // Assert
            Assert.AreEqual(2L, actual.LoadedRowCount);
        }

        [TestMethod]
        public void TestImportDataFromCsvReadsTheLineBreaksOfAnyKindTheBlankLinesAndTheByteOrderMark()
        {
            // Setup
            var path = Write("Id,Name\r\n1,A\r\n\r\n2,B\r3,C\n\n4,D", new UTF8Encoding(true));
            var destination = new FakeDbConnection();

            // Act
            var actual = destination.ImportDataFromCsv("Customer", path);

            // Assert
            Assert.AreEqual(4L, actual.TotalRowCount);
            Assert.AreEqual(4L, actual.LoadedRowCount);
            Assert.IsTrue(Inserts(destination).Any(e => e.Parameters.Values.Contains("D")));
        }

        [TestMethod]
        public void TestImportDataFromCsvReadsALastLineWithoutALineBreak()
        {
            // Setup
            var path = Write("Id,Name\n1,A\n2,B");

            // Act
            var actual = new FakeDbConnection().ImportDataFromCsv("Customer", path);

            // Assert
            Assert.AreEqual(2L, actual.LoadedRowCount);
        }

        [TestMethod]
        public void TestImportDataFromCsvReadsMoreRowsThanTheBlockOfTheReader()
        {
            // Setup
            var path = Write(Rows(3000));

            // Act
            var actual = new FakeDbConnection().ImportDataFromCsv("Customer", path, bufferSize: 1000);

            // Assert
            Assert.AreEqual(3000L, actual.LoadedRowCount);
            Assert.AreEqual(3, actual.BatchNumber);
        }

        [TestMethod]
        public void ThrowExceptionOnImportDataFromCsvIfARowHasMoreFieldsThanTheHeader()
        {
            // Setup
            var path = Write("Id,Name\n1,A,extra");

            // Act
            var exception = Assert.Throws<FormatException>(() => new FakeDbConnection().ImportDataFromCsv("Customer", path));

            // Assert
            StringAssert.Contains(exception.Message, "Row 1");
        }

        [TestMethod]
        public void ThrowExceptionOnImportDataFromCsvIfAQuotedFieldIsNotClosed()
        {
            // Setup
            var path = Write("Id,Name\n1,\"A\n2,B");

            // Act/Assert
            Assert.Throws<FormatException>(() => new FakeDbConnection().ImportDataFromCsv("Customer", path));
        }

        [TestMethod]
        public void ThrowExceptionOnImportDataFromCsvIfAQuotedFieldIsFollowedByText()
        {
            // Setup
            var path = Write("Id,Name\n1,\"A\"x");

            // Act/Assert
            Assert.Throws<FormatException>(() => new FakeDbConnection().ImportDataFromCsv("Customer", path));
        }

        [TestMethod]
        public void ThrowExceptionOnImportDataFromCsvIfAValueCannotBeConvertedIntoTheTypeOfItsColumn()
        {
            // Setup
            var path = Write("Id,Name\nabc,A");

            // Act
            var exception = Assert.Throws<FormatException>(() => new FakeDbConnection().ImportDataFromCsv("Customer", path));

            // Assert
            StringAssert.Contains(exception.Message, "Row 1");
        }

        [TestMethod]
        public async Task ThrowExceptionOnImportDataFromCsvAsyncIfTheTokenIsCancelled()
        {
            // Setup
            var path = Write(Rows(3));
            using (var source = new CancellationTokenSource())
            {
                source.Cancel();

                // Act/Assert
                await Assert.ThrowsAsync<OperationCanceledException>(() => new FakeDbConnection().ImportDataFromCsvAsync("Customer", path, cancellationToken: source.Token));
            }
        }

        #endregion

        #region Bulk Operations

        [TestMethod]
        public void TestImportDataFromCsvUsesTheBulkInsertInBlocksOfTheBufferSize()
        {
            // Setup
            var path = Write(Rows(5));

            // Act
            var actual = FakeBulkDestination.Create().ImportDataFromCsv("Customer", path, bufferSize: 2);

            // Assert
            CollectionAssert.AreEqual(new[] { 2, 2, 1 }, FakeBulkOperations.Calls.Select(c => c.RowCount).ToArray());
            Assert.IsTrue(FakeBulkOperations.Calls.All(c => c.Method == "BulkInsert"));
            Assert.AreEqual(5L, actual.LoadedRowCount);
        }

        [TestMethod]
        public void TestImportDataFromCsvWithBypassExistingRowsBulkMergesTheRows()
        {
            // Setup
            var path = Write(Rows(3));

            // Act
            FakeBulkDestination.Create().ImportDataFromCsv("Customer", path, bypassExistingRows: true);

            // Assert
            var call = FakeBulkOperations.Calls.Single();
            Assert.AreEqual("BulkMerge", call.Method);
            CollectionAssert.AreEqual(new[] { "Id" }, call.Qualifiers);
        }

        [TestMethod]
        public async Task TestImportDataFromCsvAsyncWithBypassExistingRowsBulkMergesTheRows()
        {
            // Setup
            var path = Write(Rows(3, ";"));

            // Act
            var actual = await FakeBulkDestination.Create().ImportDataFromCsvAsync("Customer", path, separator: ';', bypassExistingRows: true);

            // Assert
            Assert.AreEqual("BulkMergeAsync", FakeBulkOperations.Calls.Single().Method);
            Assert.AreEqual(3L, actual.LoadedRowCount);
        }

        [TestMethod]
        public void ThrowExceptionOnImportDataFromCsvWithBypassExistingRowsIfTheTableHasNoPrimaryKeyAndNoIdentity()
        {
            // Setup
            var path = Write(Rows(2));

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => FakeBulkDestination.CreateKeyless().ImportDataFromCsv("Customer", path, bypassExistingRows: true));
            Assert.AreEqual(0, FakeBulkOperations.Calls.Length);
        }

        [TestMethod]
        public void TestImportDataFromCsvWithQualifiersBulkMergesTheRowsWithTheQualifiers()
        {
            // Setup
            var path = Write(Rows(3));

            // Act
            FakeBulkDestination.Create().ImportDataFromCsv("Customer", path, bypassExistingRows: true, qualifiers: new[] { "Name" });

            // Assert
            var call = FakeBulkOperations.Calls.Single();
            Assert.AreEqual("BulkMerge", call.Method);
            CollectionAssert.AreEqual(new[] { "Name" }, call.Qualifiers);
        }

        [TestMethod]
        public async Task TestImportDataFromCsvAsyncWithQualifiersBulkMergesTheRowsWithTheQualifiers()
        {
            // Setup
            var path = Write(Rows(3));

            // Act
            await FakeBulkDestination.Create().ImportDataFromCsvAsync("Customer", path, bypassExistingRows: true, qualifiers: new[] { "Id", "Name" });

            // Assert
            var call = FakeBulkOperations.Calls.Single();
            Assert.AreEqual("BulkMergeAsync", call.Method);
            CollectionAssert.AreEqual(new[] { "Id", "Name" }, call.Qualifiers);
        }

        [TestMethod]
        public void TestImportDataFromCsvWithQualifiersMergesTheRowsOfATableWithoutPrimaryKeyAndIdentity()
        {
            // Setup
            var path = Write(Rows(3));

            // Act
            FakeBulkDestination.CreateKeyless().ImportDataFromCsv("Customer", path, bypassExistingRows: true, qualifiers: new[] { "Id" });

            // Assert
            CollectionAssert.AreEqual(new[] { "Id" }, FakeBulkOperations.Calls.Single().Qualifiers);
        }

        [TestMethod]
        public void TestImportDataFromCsvWithQualifiersResolvesTheNamesOfTheColumnsWithoutTheCaseAndWithoutRepeatingThem()
        {
            // Setup
            var path = Write(Rows(3));

            // Act
            FakeBulkDestination.Create().ImportDataFromCsv("Customer", path, bypassExistingRows: true, qualifiers: new[] { "name", "NAME", "[Name]" });

            // Assert
            CollectionAssert.AreEqual(new[] { "Name" }, FakeBulkOperations.Calls.Single().Qualifiers);
        }

        [TestMethod]
        public void TestImportDataFromCsvIgnoresTheQualifiersIfTheExistingRowsAreNotBypassed()
        {
            // Setup
            var path = Write(Rows(3));

            // Act
            FakeBulkDestination.Create().ImportDataFromCsv("Customer", path, qualifiers: new[] { "Name" });

            // Assert
            var call = FakeBulkOperations.Calls.Single();
            Assert.AreEqual("BulkInsert", call.Method);
            Assert.IsNull(call.Qualifiers);
        }

        [TestMethod]
        public void ThrowExceptionOnImportDataFromCsvWithQualifiersIfAQualifierIsNotAColumnOfTheTable()
        {
            // Setup
            var path = Write(Rows(3));

            // Act/Assert
            Assert.Throws<ArgumentException>(() => FakeBulkDestination.Create().ImportDataFromCsv("Customer", path, bypassExistingRows: true, qualifiers: new[] { "Missing" }));
            Assert.AreEqual(0, FakeBulkOperations.Calls.Length);
        }

        [TestMethod]
        public void ThrowExceptionOnImportDataFromCsvWithQualifiersIfAQualifierIsEmpty()
        {
            // Setup
            var path = Write(Rows(3));

            // Act/Assert
            Assert.Throws<ArgumentException>(() => FakeBulkDestination.Create().ImportDataFromCsv("Customer", path, bypassExistingRows: true, qualifiers: new[] { "Id", " " }));
        }

        [TestMethod]
        public void ThrowExceptionOnImportDataFromCsvWithQualifiersIfAQualifierIsNotAColumnOfTheFile()
        {
            // Setup
            var path = Write("Id,Name\n1,A");

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => FakeBulkDestination.Create().ImportDataFromCsv("Customer", path, bypassExistingRows: true, qualifiers: new[] { "Birthday" }));
        }

        [TestMethod]
        public void TestImportDataFromCsvWithBypassExistingRowsMergesTheRowsInBatchesIfTheProviderHasNoBulkMerge()
        {
            // Setup
            var path = Write(Rows(3));
            var destination = new FakeDbConnection();

            // Act
            var actual = destination.ImportDataFromCsv("Customer", path, bypassExistingRows: true, qualifiers: new[] { "Name" });

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
        public async Task TestImportDataFromCsvAsyncWithBypassExistingRowsMergesTheRowsInBatchesIfTheProviderHasNoBulkMerge()
        {
            // Setup
            var path = Write(Rows(3));
            var destination = new FakeDbConnection();

            // Act
            await destination.ImportDataFromCsvAsync("Customer", path, bypassExistingRows: true);

            // Assert
            var update = destination.Executions.First(e => e.CommandText.StartsWith("UPDATE", StringComparison.Ordinal));
            StringAssert.Contains(update.CommandText, "WHERE [Id] = ");
            StringAssert.Contains(update.CommandText, "SET [Name] = ");
        }

        [TestMethod]
        public void TestImportDataFromCsvWithMappingsLoadsTheMappedColumnsOnly()
        {
            // Setup
            var path = Write("Code,Title,Extra\n7,Seven,x");
            var destination = new FakeDbConnection();
            var mappings = new[] { new CopyColumnMapping("Code", "Id"), new CopyColumnMapping("Title", "Name") };

            // Act
            var actual = destination.ImportDataFromCsv("Customer", path, mappings: mappings);

            // Assert
            var insert = destination.Executions.Single(e => e.CommandText.StartsWith("INSERT INTO", StringComparison.Ordinal));
            CollectionAssert.AreEquivalent(new object[] { 7, "Seven" }, insert.Parameters.Values.ToArray());
            Assert.AreEqual(1L, actual.LoadedRowCount);
        }

        [TestMethod]
        public async Task TestImportDataFromCsvAsyncWithMappingsLoadsTheMappedColumnsOnly()
        {
            // Setup
            var path = Write("Code,Title,Extra\n7,Seven,x");
            var destination = new FakeDbConnection();
            var mappings = new[] { new CopyColumnMapping("title", "name"), new CopyColumnMapping("CODE", "ID") };

            // Act
            await destination.ImportDataFromCsvAsync("Customer", path, mappings: mappings);

            // Assert
            var insert = destination.Executions.Single(e => e.CommandText.StartsWith("INSERT INTO", StringComparison.Ordinal));
            CollectionAssert.AreEquivalent(new object[] { 7, "Seven" }, insert.Parameters.Values.ToArray());
        }

        [TestMethod]
        public void TestImportDataFromCsvWithMappingsDoesNotLoadAColumnOfTheFileThatHasTheNameOfAColumnOfTheTableButIsNotMapped()
        {
            // Setup
            var path = Write("Code,Id,Name\n7,8,Same");
            var destination = new FakeDbConnection();

            // Act
            destination.ImportDataFromCsv("Customer", path, mappings: new[] { new CopyColumnMapping("Code", "Id") });

            // Assert
            var insert = destination.Executions.Single(e => e.CommandText.StartsWith("INSERT INTO", StringComparison.Ordinal));
            CollectionAssert.AreEquivalent(new object[] { 7 }, insert.Parameters.Values.ToArray());
        }

        [TestMethod]
        public void TestImportDataFromCsvWithMappingsLoadsAColumnOfTheFileIntoMoreThanOneColumnOfTheTable()
        {
            // Setup
            var path = Write("Code,Title,Extra\n7,Seven,x");
            var destination = new FakeDbConnection();
            var mappings = new[] { new CopyColumnMapping("Code", "Id"), new CopyColumnMapping("Code", "Name") };

            // Act
            destination.ImportDataFromCsv("Customer", path, mappings: mappings);

            // Assert
            var insert = destination.Executions.Single(e => e.CommandText.StartsWith("INSERT INTO", StringComparison.Ordinal));
            CollectionAssert.AreEquivalent(new object[] { 7, "7" }, insert.Parameters.Values.ToArray());
        }

        [TestMethod]
        public void TestImportDataFromCsvWithEmptyMappingsLoadsTheColumnsByTheirNames()
        {
            // Setup
            var path = Write("Id,Name\n7,Same");
            var destination = new FakeDbConnection();

            // Act
            destination.ImportDataFromCsv("Customer", path, mappings: new List<CopyColumnMapping>());

            // Assert
            var insert = destination.Executions.Single(e => e.CommandText.StartsWith("INSERT INTO", StringComparison.Ordinal));
            CollectionAssert.AreEquivalent(new object[] { 7, "Same" }, insert.Parameters.Values.ToArray());
        }

        [TestMethod]
        public void ThrowExceptionOnImportDataFromCsvWithMappingsIfADestinationColumnIsNotAColumnOfTheTable()
        {
            // Setup
            var path = Write("Code,Title,Extra\n7,Seven,x");

            // Act/Assert
            Assert.Throws<ArgumentException>(() => new FakeDbConnection().ImportDataFromCsv("Customer", path, mappings: new[] { new CopyColumnMapping("Code", "Missing") }));
        }

        [TestMethod]
        public void ThrowExceptionOnImportDataFromCsvWithMappingsIfADestinationColumnIsMappedMoreThanOnce()
        {
            // Setup
            var path = Write("Code,Title,Extra\n7,Seven,x");
            var mappings = new[] { new CopyColumnMapping("Code", "Id"), new CopyColumnMapping("Title", "id") };

            // Act/Assert
            Assert.Throws<ArgumentException>(() => new FakeDbConnection().ImportDataFromCsv("Customer", path, mappings: mappings));
        }

        [TestMethod]
        public void ThrowExceptionOnImportDataFromCsvWithMappingsIfAMappingIsNull()
        {
            // Setup
            var path = Write("Code,Title,Extra\n7,Seven,x");

            // Act/Assert
            Assert.Throws<ArgumentException>(() => new FakeDbConnection().ImportDataFromCsv("Customer", path, mappings: new CopyColumnMapping[] { null }));
        }

        [TestMethod]
        public void ThrowExceptionOnImportDataFromCsvWithMappingsIfAMappingHasAnEmptyColumn()
        {
            // Setup
            var path = Write("Code,Title,Extra\n7,Seven,x");

            // Act/Assert
            Assert.Throws<ArgumentException>(() => new FakeDbConnection().ImportDataFromCsv("Customer", path, mappings: new[] { new CopyColumnMapping(" ", "Id") }));
        }

        [TestMethod]
        public void ThrowExceptionOnImportDataFromCsvWithMappingsIfASourceColumnIsNotAColumnOfTheHeader()
        {
            // Setup
            var path = Write("Code,Title,Extra\n7,Seven,x");
            var destination = new FakeDbConnection();
            var mappings = new[] { new CopyColumnMapping("Code", "Id"), new CopyColumnMapping("Missing", "Name") };

            // Act
            // Act/Assert
            Assert.Throws<ArgumentException>(() => destination.ImportDataFromCsv("Customer", path, mappings: mappings));
        }

        [TestMethod]
        public void TestImportDataFromCsvWithMappingsAndBypassExistingRowsBulkMergesTheRowsWithTheMappedQualifier()
        {
            // Setup
            var path = Write("Code,Title,Extra\n7,Seven,x");
            var mappings = new[] { new CopyColumnMapping("Code", "Id"), new CopyColumnMapping("Title", "Name") };

            // Act
            FakeBulkDestination.Create().ImportDataFromCsv("Customer", path, bypassExistingRows: true, mappings: mappings);

            // Assert
            var call = FakeBulkOperations.Calls.Single();
            Assert.AreEqual("BulkMerge", call.Method);
            CollectionAssert.AreEqual(new[] { "Id" }, call.Qualifiers);
            CollectionAssert.AreEqual(new[] { "Id", "Name" }, FakeBulkOperations.LastColumns);
        }

        [TestMethod]
        public void ThrowExceptionOnImportDataFromCsvWithMappingsAndBypassExistingRowsIfTheMappingsDoNotLoadTheQualifier()
        {
            // Setup
            var path = Write("Code,Title,Extra\n7,Seven,x");

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => FakeBulkDestination.Create().ImportDataFromCsv("Customer", path, bypassExistingRows: true, mappings: new[] { new CopyColumnMapping("Title", "Name") }));
            Assert.AreEqual(0, FakeBulkOperations.Calls.Length);
        }

        [TestMethod]
        public void TestImportDataFromCsvWithMappingsLeavesTheIdentityColumnToTheDatabaseIfItIsNotToBeKept()
        {
            // Setup
            var path = Write("Code,Title,Extra\n7,Seven,x");
            var mappings = new[] { new CopyColumnMapping("Code", "Id"), new CopyColumnMapping("Title", "Name") };

            // Act
            FakeBulkDestination.Create().ImportDataFromCsv("Customer", path, mappings: mappings, identityBehavior: LoadIdentityBehavior.Ignore);

            // Assert
            CollectionAssert.AreEqual(new[] { "Name" }, FakeBulkOperations.LastColumns);
        }

        #endregion
    }
}
