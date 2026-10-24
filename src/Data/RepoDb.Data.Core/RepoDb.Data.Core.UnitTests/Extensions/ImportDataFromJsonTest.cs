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
using Moq;
using RepoDb.Data.Core.UnitTests.CustomObjects;
using RepoDb.Data.Core.UnitTests.Fake.BulkOperations;
using RepoDb.Data.Enumerations;
using RepoDb.Data.Interfaces;
using RepoDb.Data.Models;
using RepoDb.DbSettings;

namespace RepoDb.Data.Core.UnitTests.Extensions
{
    [TestClass]
    public class ImportDataFromJsonTest
    {
        #region Helpers

        private readonly List<string> _files = new List<string>();

        private string Write(string text)
        {
            var path = Path.Combine(Path.GetTempPath(), "repodb_load_json_" + Guid.NewGuid().ToString("N") + ".json");
            File.WriteAllText(path, text, new UTF8Encoding(false));
            _files.Add(path);
            return path;
        }

        private static string Rows(int count) =>
            "[" + string.Join(",", Enumerable.Range(1, count).Select(i => "{\"Id\":" + i + ",\"Name\":\"Name " + i + "\",\"Birthday\":\"2026-01-02T03:04:05Z\"}")) + "]";

        private static Action<ImportProgress> Snapshot(List<(int Batch, long Loaded, long Total)> seen) =>
            p => seen.Add((p.BatchNumber, p.LoadedRowCount, p.TotalRowCount));

        [TestInitialize]
        public void Initialize()
        {
            DbSettingMapper.Add<FakeBulkDbConnection>(new FakeDbSetting(), true);
            DbHelperMapper.Add<FakeBulkDbConnection>(new FakeDbHelper(), true);
            DbSettingMapper.Add<FakeBulkOptionsDbConnection>(new FakeDbSetting(), true);
            DbHelperMapper.Add<FakeBulkOptionsDbConnection>(new FakeDbHelper(), true);
            DbSettingMapper.Add<FakeBulkIdentityBehaviorDbConnection>(new FakeDbSetting(), true);
            DbHelperMapper.Add<FakeBulkIdentityBehaviorDbConnection>(new FakeDbHelper(), true);
            DbSettingMapper.Add<FakeBulkSettingDbConnection>(new FakeBulkDbSetting(), true);
            DbHelperMapper.Add<FakeBulkSettingDbConnection>(new FakeDbHelper(), true);
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

        #region Bypass Existing Rows

        [TestMethod]
        public void TestImportDataFromJsonWithBypassExistingRowsBulkMergesTheRowsWithThePrimaryKeyAsTheQualifiers()
        {
            // Setup
            var path = Write(Rows(3));
            var destination = FakeBulkDestination.Create();

            // Act
            var actual = destination.ImportDataFromJson("Customer", path, bypassExistingRows: true);

            // Assert
            var call = FakeBulkOperations.Calls.Single();
            Assert.AreEqual("BulkMerge", call.Method);
            Assert.AreEqual("Customer", call.TableName);
            Assert.AreEqual(3, call.RowCount);
            CollectionAssert.AreEqual(new[] { "Id" }, call.Qualifiers);
            Assert.AreEqual(3, actual.LoadedRowCount);
        }

        [TestMethod]
        public async Task TestImportDataFromJsonAsyncWithBypassExistingRowsBulkMergesTheRowsWithThePrimaryKeyAsTheQualifiers()
        {
            // Setup
            var path = Write(Rows(3));
            var destination = FakeBulkDestination.Create();

            // Act
            var actual = await destination.ImportDataFromJsonAsync("Customer", path, bypassExistingRows: true);

            // Assert
            var call = FakeBulkOperations.Calls.Single();
            Assert.AreEqual("BulkMergeAsync", call.Method);
            CollectionAssert.AreEqual(new[] { "Id" }, call.Qualifiers);
            Assert.AreEqual(3, actual.LoadedRowCount);
        }

        [TestMethod]
        public void TestImportDataFromJsonWithBypassExistingRowsBulkMergesEveryBuffer()
        {
            // Setup
            var path = Write(Rows(5));

            // Act
            var actual = FakeBulkDestination.Create().ImportDataFromJson("Customer", path, bufferSize: 2, bypassExistingRows: true);

            // Assert
            CollectionAssert.AreEqual(new[] { "BulkMerge", "BulkMerge", "BulkMerge" }, FakeBulkOperations.Calls.Select(c => c.Method).ToArray());
            Assert.AreEqual(5, actual.LoadedRowCount);
        }

        [TestMethod]
        public void TestImportDataFromJsonWithBypassExistingRowsBulkMergesTheRowsWithTheIdentityAsTheQualifierIfThereIsNoPrimaryKey()
        {
            // Setup
            var path = Write(Rows(2));

            // Act
            FakeBulkDestination.CreateIdentityOnly().ImportDataFromJson("Customer", path, bypassExistingRows: true);

            // Assert
            CollectionAssert.AreEqual(new[] { "Id" }, FakeBulkOperations.Calls.Single().Qualifiers);
        }

        [TestMethod]
        public void TestImportDataFromJsonWithoutBypassExistingRowsBulkInsertsTheRows()
        {
            // Setup
            var path = Write(Rows(2));

            // Act
            FakeBulkDestination.Create().ImportDataFromJson("Customer", path);

            // Assert
            var call = FakeBulkOperations.Calls.Single();
            Assert.AreEqual("BulkInsert", call.Method);
            Assert.IsNull(call.Qualifiers);
        }

        [TestMethod]
        public void ThrowExceptionOnImportDataFromJsonWithBypassExistingRowsIfTheTableHasNoPrimaryKeyAndNoIdentity()
        {
            // Setup
            var path = Write(Rows(2));

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => FakeBulkDestination.CreateKeyless().ImportDataFromJson("Customer", path, bypassExistingRows: true));
            Assert.AreEqual(0, FakeBulkOperations.Calls.Length);
        }

        [TestMethod]
        public async Task ThrowExceptionOnImportDataFromJsonAsyncWithBypassExistingRowsIfTheTableHasNoPrimaryKeyAndNoIdentity()
        {
            // Setup
            var path = Write(Rows(2));

            // Act/Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => FakeBulkDestination.CreateKeyless().ImportDataFromJsonAsync("Customer", path, bypassExistingRows: true));
            Assert.AreEqual(0, FakeBulkOperations.Calls.Length);
        }

        [TestMethod]
        public void ThrowExceptionOnImportDataFromJsonWithBypassExistingRowsIfTheQualifierIsNotAColumnOfTheFile()
        {
            // Setup
            var path = Write("[{\"Name\":\"Name 1\"}]");

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => FakeBulkDestination.Create().ImportDataFromJson("Customer", path, bypassExistingRows: true));
        }

        [TestMethod]
        public void ThrowExceptionOnImportDataFromJsonWithBypassExistingRowsIfTheQualifierIsGeneratedByTheDatabase()
        {
            // Setup
            var path = Write(Rows(2));

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => FakeBulkDestination.Create().ImportDataFromJson("Customer", path, bypassExistingRows: true, identityBehavior: IdentityBehavior.Ignore));
        }


        [TestMethod]
        public void TestImportDataFromJsonWithQualifiersBulkMergesTheRowsWithTheQualifiers()
        {
            // Setup
            var path = Write(Rows(3));

            // Act
            FakeBulkDestination.Create().ImportDataFromJson("Customer", path, bypassExistingRows: true, qualifiers: new[] { "Name" });

            // Assert
            var call = FakeBulkOperations.Calls.Single();
            Assert.AreEqual("BulkMerge", call.Method);
            CollectionAssert.AreEqual(new[] { "Name" }, call.Qualifiers);
        }

        [TestMethod]
        public async Task TestImportDataFromJsonAsyncWithQualifiersBulkMergesTheRowsWithTheQualifiers()
        {
            // Setup
            var path = Write(Rows(3));

            // Act
            await FakeBulkDestination.Create().ImportDataFromJsonAsync("Customer", path, bypassExistingRows: true, qualifiers: new[] { "Id", "Name" });

            // Assert
            var call = FakeBulkOperations.Calls.Single();
            Assert.AreEqual("BulkMergeAsync", call.Method);
            CollectionAssert.AreEqual(new[] { "Id", "Name" }, call.Qualifiers);
        }

        [TestMethod]
        public void TestImportDataFromJsonWithQualifiersMergesTheRowsOfATableWithoutPrimaryKeyAndIdentity()
        {
            // Setup
            var path = Write(Rows(3));

            // Act
            FakeBulkDestination.CreateKeyless().ImportDataFromJson("Customer", path, bypassExistingRows: true, qualifiers: new[] { "Id" });

            // Assert
            CollectionAssert.AreEqual(new[] { "Id" }, FakeBulkOperations.Calls.Single().Qualifiers);
        }

        [TestMethod]
        public void TestImportDataFromJsonWithQualifiersResolvesTheNamesOfTheColumnsWithoutTheCaseAndWithoutRepeatingThem()
        {
            // Setup
            var path = Write(Rows(3));

            // Act
            FakeBulkDestination.Create().ImportDataFromJson("Customer", path, bypassExistingRows: true, qualifiers: new[] { "name", "NAME", "[Name]" });

            // Assert
            CollectionAssert.AreEqual(new[] { "Name" }, FakeBulkOperations.Calls.Single().Qualifiers);
        }

        [TestMethod]
        public void TestImportDataFromJsonIgnoresTheQualifiersIfTheExistingRowsAreNotBypassed()
        {
            // Setup
            var path = Write(Rows(3));

            // Act
            FakeBulkDestination.Create().ImportDataFromJson("Customer", path, qualifiers: new[] { "Name" });

            // Assert
            var call = FakeBulkOperations.Calls.Single();
            Assert.AreEqual("BulkInsert", call.Method);
            Assert.IsNull(call.Qualifiers);
        }

        [TestMethod]
        public void ThrowExceptionOnImportDataFromJsonWithQualifiersIfAQualifierIsNotAColumnOfTheTable()
        {
            // Setup
            var path = Write(Rows(3));

            // Act/Assert
            Assert.Throws<ArgumentException>(() => FakeBulkDestination.Create().ImportDataFromJson("Customer", path, bypassExistingRows: true, qualifiers: new[] { "Missing" }));
            Assert.AreEqual(0, FakeBulkOperations.Calls.Length);
        }

        [TestMethod]
        public void ThrowExceptionOnImportDataFromJsonWithQualifiersIfAQualifierIsEmpty()
        {
            // Setup
            var path = Write(Rows(3));

            // Act/Assert
            Assert.Throws<ArgumentException>(() => FakeBulkDestination.Create().ImportDataFromJson("Customer", path, bypassExistingRows: true, qualifiers: new[] { "Id", " " }));
        }

        [TestMethod]
        public void ThrowExceptionOnImportDataFromJsonWithQualifiersIfAQualifierIsNotAColumnOfTheFile()
        {
            // Setup
            var path = Write("[{\"Id\":1,\"Name\":\"A\"}]");

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => FakeBulkDestination.Create().ImportDataFromJson("Customer", path, bypassExistingRows: true, qualifiers: new[] { "Birthday" }));
        }

        [TestMethod]
        public void TestImportDataFromJsonWithBypassExistingRowsMergesTheRowsInBatchesIfTheProviderHasNoBulkMerge()
        {
            // Setup
            var path = Write(Rows(3));
            var destination = new FakeDbConnection();

            // Act
            var actual = destination.ImportDataFromJson("Customer", path, bypassExistingRows: true, qualifiers: new[] { "Name" });

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
        public async Task TestImportDataFromJsonAsyncWithBypassExistingRowsMergesTheRowsInBatchesIfTheProviderHasNoBulkMerge()
        {
            // Setup
            var path = Write(Rows(3));
            var destination = new FakeDbConnection();

            // Act
            await destination.ImportDataFromJsonAsync("Customer", path, bypassExistingRows: true);

            // Assert
            var update = destination.Executions.First(e => e.CommandText.StartsWith("UPDATE", StringComparison.Ordinal));
            StringAssert.Contains(update.CommandText, "WHERE [Id] = ");
            StringAssert.Contains(update.CommandText, "SET [Name] = ");
        }


        [TestMethod]
        public void TestImportDataFromJsonWithMappingsLoadsTheMappedColumnsOnly()
        {
            // Setup
            var path = Write("[{\"Code\":7,\"Title\":\"Seven\",\"Extra\":\"x\"}]");
            var destination = new FakeDbConnection();
            var mappings = new[] { new ColumnMapping("Code", "Id"), new ColumnMapping("Title", "Name") };

            // Act
            var actual = destination.ImportDataFromJson("Customer", path, mappings: mappings);

            // Assert
            var insert = destination.Executions.Single(e => e.CommandText.StartsWith("INSERT INTO", StringComparison.Ordinal));
            CollectionAssert.AreEquivalent(new object[] { 7, "Seven" }, insert.Parameters.Values.ToArray());
            Assert.AreEqual(1L, actual.LoadedRowCount);
        }

        [TestMethod]
        public async Task TestImportDataFromJsonAsyncWithMappingsLoadsTheMappedColumnsOnly()
        {
            // Setup
            var path = Write("[{\"Code\":7,\"Title\":\"Seven\",\"Extra\":\"x\"}]");
            var destination = new FakeDbConnection();
            var mappings = new[] { new ColumnMapping("title", "name"), new ColumnMapping("CODE", "ID") };

            // Act
            await destination.ImportDataFromJsonAsync("Customer", path, mappings: mappings);

            // Assert
            var insert = destination.Executions.Single(e => e.CommandText.StartsWith("INSERT INTO", StringComparison.Ordinal));
            CollectionAssert.AreEquivalent(new object[] { 7, "Seven" }, insert.Parameters.Values.ToArray());
        }

        [TestMethod]
        public void TestImportDataFromJsonWithMappingsDoesNotLoadAColumnOfTheFileThatHasTheNameOfAColumnOfTheTableButIsNotMapped()
        {
            // Setup
            var path = Write("[{\"Code\":7,\"Id\":8,\"Name\":\"Same\"}]");
            var destination = new FakeDbConnection();

            // Act
            destination.ImportDataFromJson("Customer", path, mappings: new[] { new ColumnMapping("Code", "Id") });

            // Assert
            var insert = destination.Executions.Single(e => e.CommandText.StartsWith("INSERT INTO", StringComparison.Ordinal));
            CollectionAssert.AreEquivalent(new object[] { 7 }, insert.Parameters.Values.ToArray());
        }

        [TestMethod]
        public void TestImportDataFromJsonWithMappingsLoadsAColumnOfTheFileIntoMoreThanOneColumnOfTheTable()
        {
            // Setup
            var path = Write("[{\"Code\":7,\"Title\":\"Seven\",\"Extra\":\"x\"}]");
            var destination = new FakeDbConnection();
            var mappings = new[] { new ColumnMapping("Code", "Id"), new ColumnMapping("Code", "Name") };

            // Act
            destination.ImportDataFromJson("Customer", path, mappings: mappings);

            // Assert
            var insert = destination.Executions.Single(e => e.CommandText.StartsWith("INSERT INTO", StringComparison.Ordinal));
            CollectionAssert.AreEquivalent(new object[] { 7, "7" }, insert.Parameters.Values.ToArray());
        }

        [TestMethod]
        public void TestImportDataFromJsonWithEmptyMappingsLoadsTheColumnsByTheirNames()
        {
            // Setup
            var path = Write("[{\"Id\":7,\"Name\":\"Same\"}]");
            var destination = new FakeDbConnection();

            // Act
            destination.ImportDataFromJson("Customer", path, mappings: new List<ColumnMapping>());

            // Assert
            var insert = destination.Executions.Single(e => e.CommandText.StartsWith("INSERT INTO", StringComparison.Ordinal));
            CollectionAssert.AreEquivalent(new object[] { 7, "Same" }, insert.Parameters.Values.ToArray());
        }

        [TestMethod]
        public void ThrowExceptionOnImportDataFromJsonWithMappingsIfADestinationColumnIsNotAColumnOfTheTable()
        {
            // Setup
            var path = Write("[{\"Code\":7,\"Title\":\"Seven\",\"Extra\":\"x\"}]");

            // Act/Assert
            Assert.Throws<ArgumentException>(() => new FakeDbConnection().ImportDataFromJson("Customer", path, mappings: new[] { new ColumnMapping("Code", "Missing") }));
        }

        [TestMethod]
        public void ThrowExceptionOnImportDataFromJsonWithMappingsIfADestinationColumnIsMappedMoreThanOnce()
        {
            // Setup
            var path = Write("[{\"Code\":7,\"Title\":\"Seven\",\"Extra\":\"x\"}]");
            var mappings = new[] { new ColumnMapping("Code", "Id"), new ColumnMapping("Title", "id") };

            // Act/Assert
            Assert.Throws<ArgumentException>(() => new FakeDbConnection().ImportDataFromJson("Customer", path, mappings: mappings));
        }

        [TestMethod]
        public void ThrowExceptionOnImportDataFromJsonWithMappingsIfAMappingIsNull()
        {
            // Setup
            var path = Write("[{\"Code\":7,\"Title\":\"Seven\",\"Extra\":\"x\"}]");

            // Act/Assert
            Assert.Throws<ArgumentException>(() => new FakeDbConnection().ImportDataFromJson("Customer", path, mappings: new ColumnMapping[] { null }));
        }

        [TestMethod]
        public void ThrowExceptionOnImportDataFromJsonWithMappingsIfAMappingHasAnEmptyColumn()
        {
            // Setup
            var path = Write("[{\"Code\":7,\"Title\":\"Seven\",\"Extra\":\"x\"}]");

            // Act/Assert
            Assert.Throws<ArgumentException>(() => new FakeDbConnection().ImportDataFromJson("Customer", path, mappings: new[] { new ColumnMapping(" ", "Id") }));
        }

        [TestMethod]
        public void TestImportDataFromJsonWithMappingsLoadsNullIfAMappedColumnIsNotInTheFile()
        {
            // Setup
            var path = Write("[{\"Code\":7,\"Title\":\"Seven\",\"Extra\":\"x\"}]");
            var destination = new FakeDbConnection();
            var mappings = new[] { new ColumnMapping("Code", "Id"), new ColumnMapping("Missing", "Name") };

            // Act
            var actual = destination.ImportDataFromJson("Customer", path, mappings: mappings);

            // Assert
            Assert.AreEqual(1L, actual.LoadedRowCount);
        }

        [TestMethod]
        public void TestImportDataFromJsonWithMappingsAndBypassExistingRowsBulkMergesTheRowsWithTheMappedQualifier()
        {
            // Setup
            var path = Write("[{\"Code\":7,\"Title\":\"Seven\",\"Extra\":\"x\"}]");
            var mappings = new[] { new ColumnMapping("Code", "Id"), new ColumnMapping("Title", "Name") };

            // Act
            FakeBulkDestination.Create().ImportDataFromJson("Customer", path, bypassExistingRows: true, mappings: mappings);

            // Assert
            var call = FakeBulkOperations.Calls.Single();
            Assert.AreEqual("BulkMerge", call.Method);
            CollectionAssert.AreEqual(new[] { "Id" }, call.Qualifiers);
            CollectionAssert.AreEqual(new[] { "Id", "Name" }, FakeBulkOperations.LastColumns);
        }

        [TestMethod]
        public void ThrowExceptionOnImportDataFromJsonWithMappingsAndBypassExistingRowsIfTheMappingsDoNotLoadTheQualifier()
        {
            // Setup
            var path = Write("[{\"Code\":7,\"Title\":\"Seven\",\"Extra\":\"x\"}]");

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => FakeBulkDestination.Create().ImportDataFromJson("Customer", path, bypassExistingRows: true, mappings: new[] { new ColumnMapping("Title", "Name") }));
            Assert.AreEqual(0, FakeBulkOperations.Calls.Length);
        }

        [TestMethod]
        public void TestImportDataFromJsonWithMappingsLeavesTheIdentityColumnToTheDatabaseIfItIsNotToBeKept()
        {
            // Setup
            var path = Write("[{\"Code\":7,\"Title\":\"Seven\",\"Extra\":\"x\"}]");
            var mappings = new[] { new ColumnMapping("Code", "Id"), new ColumnMapping("Title", "Name") };

            // Act
            FakeBulkDestination.Create().ImportDataFromJson("Customer", path, mappings: mappings, identityBehavior: IdentityBehavior.Ignore);

            // Assert
            CollectionAssert.AreEqual(new[] { "Name" }, FakeBulkOperations.LastColumns);
        }


        #region Data Interceptors

        private static IList<IDataInterceptor<Row>> Interceptors(params Func<Row, Row>[] transforms) =>
            transforms.Select(transform => (IDataInterceptor<Row>)new FakeDataInterceptor(transform)).ToList();

        private static List<object> InsertedValues(FakeDbConnection connection) =>
            connection.Executions.Where(e => e.CommandText.StartsWith("INSERT INTO", StringComparison.Ordinal)).SelectMany(e => e.Parameters.Values).ToList();

        [TestMethod]
        public void TestImportDataFromJsonLoadsTheRowsThatTheInterceptorsGiveInOrder()
        {
            // Setup
            var path = Write(Rows(3));
            var destination = new FakeDbConnection();
            var interceptors = Interceptors(
                row => { row["Name"] = ((string)row["Name"]).ToUpperInvariant(); return row; },
                row => { row["Name"] = row["Name"] + "!"; return row; });

            // Act
            var actual = destination.ImportDataFromJson("Customer", path, dataInterceptors: interceptors);

            // Assert
            CollectionAssert.AreEquivalent(new[] { "NAME 1!", "NAME 2!", "NAME 3!" }, InsertedValues(destination).OfType<string>().ToList());
            Assert.AreEqual(3L, actual.LoadedRowCount);
        }

        [TestMethod]
        public async Task TestImportDataFromJsonAsyncLoadsTheRowsThatTheInterceptorsGive()
        {
            // Setup
            var path = Write(Rows(2));
            var destination = new FakeDbConnection();
            var interceptors = Interceptors(row => { row["Name"] = "Changed"; return row; });

            // Act
            await destination.ImportDataFromJsonAsync("Customer", path, dataInterceptors: interceptors);

            // Assert
            CollectionAssert.AreEqual(new[] { "Changed", "Changed" }, InsertedValues(destination).OfType<string>().ToList());
        }

        [TestMethod]
        public void TestImportDataFromJsonGivesTheInterceptorsTheValuesConvertedIntoTheTypesOfTheColumns()
        {
            // Setup
            var path = Write(Rows(2));
            var seen = new List<Type>();
            var interceptors = Interceptors(row => { seen.Add(row["Id"].GetType()); return row; });

            // Act
            new FakeDbConnection().ImportDataFromJson("Customer", path, dataInterceptors: interceptors);

            // Assert
            CollectionAssert.AreEqual(new[] { typeof(int), typeof(int) }, seen);
        }

        [TestMethod]
        public void TestImportDataFromJsonGivesTheInterceptorsTheColumnsOfTheTableThatTheMappingsGive()
        {
            // Setup
            var path = Write("[{\"Code\":7,\"Title\":\"Seven\"}]");
            var names = new List<string>();
            var interceptors = Interceptors(row => { names.AddRange(row.Columns.Select(c => c.Name)); return row; });
            var mappings = new[] { new ColumnMapping("Code", "Id"), new ColumnMapping("Title", "Name") };

            // Act
            new FakeDbConnection().ImportDataFromJson("Customer", path, mappings, dataInterceptors: interceptors);

            // Assert
            CollectionAssert.AreEqual(new[] { "Id", "Name" }, names);
        }

        [TestMethod]
        public void TestImportDataFromJsonConvertsTheValuesThatTheInterceptorsGive()
        {
            // Setup
            var path = Write(Rows(1));
            var destination = new FakeDbConnection();
            var interceptors = Interceptors(row => { row["Id"] = "42"; return row; });

            // Act
            destination.ImportDataFromJson("Customer", path, dataInterceptors: interceptors);

            // Assert
            Assert.IsTrue(InsertedValues(destination).Contains(42));
        }

        [TestMethod]
        public void TestImportDataFromJsonBulkInsertsTheColumnsThatTheInterceptorsGive()
        {
            // Setup
            var path = Write(Rows(2));
            var interceptors = Interceptors(row => { row.Remove("Birthday"); return row; });

            // Act
            FakeBulkDestination.Create().ImportDataFromJson("Customer", path, dataInterceptors: interceptors);

            // Assert
            CollectionAssert.AreEqual(new[] { "Id", "Name" }, FakeBulkOperations.LastColumns);
            Assert.AreEqual(2, FakeBulkOperations.Calls.Single().RowCount);
        }

        [TestMethod]
        public void TestImportDataFromJsonLeavesOutTheColumnsThatTheInterceptorsAddAndTheTableDoesNotHave()
        {
            // Setup
            var path = Write(Rows(1));
            var interceptors = Interceptors(row => row.Add("Extra", typeof(string), "e"));

            // Act
            FakeBulkDestination.Create().ImportDataFromJson("Customer", path, dataInterceptors: interceptors);

            // Assert
            CollectionAssert.AreEqual(new[] { "Id", "Name", "Birthday" }, FakeBulkOperations.LastColumns);
        }

        [TestMethod]
        public void TestImportDataFromJsonLeavesTheIdentityColumnOutAfterTheInterceptorsIfItIsNotToBeKept()
        {
            // Setup
            var path = Write(Rows(1));
            var interceptors = Interceptors(row => { row["Id"] = 500; return row; });

            // Act
            FakeBulkDestination.Create().ImportDataFromJson("Customer", path, identityBehavior: IdentityBehavior.Ignore, dataInterceptors: interceptors);

            // Assert
            CollectionAssert.AreEqual(new[] { "Name", "Birthday" }, FakeBulkOperations.LastColumns);
        }

        [TestMethod]
        public void TestImportDataFromJsonWithInterceptorsAndBypassExistingRowsBulkMergesTheRowsWithTheQualifiers()
        {
            // Setup
            var path = Write(Rows(2));
            var interceptors = Interceptors(row => { row["Name"] = "Changed"; return row; });

            // Act
            FakeBulkDestination.Create().ImportDataFromJson("Customer", path, bypassExistingRows: true, dataInterceptors: interceptors);

            // Assert
            var call = FakeBulkOperations.Calls.Single();
            Assert.AreEqual("BulkMerge", call.Method);
            CollectionAssert.AreEqual(new[] { "Id" }, call.Qualifiers);
        }

        [TestMethod]
        public void ThrowExceptionOnImportDataFromJsonWithBypassExistingRowsIfAnInterceptorRemovesAQualifier()
        {
            // Setup
            var path = Write(Rows(2));
            var interceptors = Interceptors(row => { row.Remove("Id"); return row; });

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => FakeBulkDestination.Create().ImportDataFromJson("Customer", path, bypassExistingRows: true, dataInterceptors: interceptors));
            Assert.AreEqual(0, FakeBulkOperations.Calls.Length);
        }

        [TestMethod]
        public void ThrowExceptionOnImportDataFromJsonIfAnInterceptorReturnsNull()
        {
            // Setup
            var path = Write(Rows(1));

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => new FakeDbConnection().ImportDataFromJson("Customer", path, dataInterceptors: Interceptors(row => null)));
        }

        [TestMethod]
        public void ThrowExceptionOnImportDataFromJsonIfAnInterceptorIsNull()
        {
            // Setup
            var path = Write(Rows(1));

            // Act/Assert
            Assert.Throws<ArgumentException>(() => new FakeDbConnection().ImportDataFromJson("Customer", path, dataInterceptors: new List<IDataInterceptor<Row>> { null }));
        }

        [TestMethod]
        public void ThrowExceptionOnImportDataFromJsonIfAnInterceptorGivesAnotherShapeToAnotherRow()
        {
            // Setup
            var path = Write(Rows(3));
            var interceptors = Interceptors(row => (int)row["Id"] == 2 ? row.Remove("Name") ? row : row : row);

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => new FakeDbConnection().ImportDataFromJson("Customer", path, dataInterceptors: interceptors));
        }

        [TestMethod]
        public void ThrowExceptionOnImportDataFromJsonIfNoColumnOfTheRowsThatTheInterceptorsGiveMatchesAColumnOfTheTable()
        {
            // Setup
            var path = Write(Rows(1));
            var interceptors = Interceptors(row => new Row(new[] { new Column("Unknown", typeof(int)) }, new object[] { 1 }));

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => new FakeDbConnection().ImportDataFromJson("Customer", path, dataInterceptors: interceptors));
        }

        [TestMethod]
        public void TestImportDataFromJsonWithEmptyInterceptorsLoadsTheRowsAsTheyAre()
        {
            // Setup
            var path = Write(Rows(2));
            var destination = new FakeDbConnection();

            // Act
            destination.ImportDataFromJson("Customer", path, dataInterceptors: new List<IDataInterceptor<Row>>());

            // Assert
            CollectionAssert.AreEquivalent(new[] { "Name 1", "Name 2" }, InsertedValues(destination).OfType<string>().ToList());
        }

        #endregion

        #endregion

        #region Arguments

        [TestMethod]
        public void ThrowExceptionOnImportDataFromJsonIfTheConnectionIsNull()
        {
            // Setup
            var path = Write("[]");

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => ((IDbConnection)null).ImportDataFromJson("Customer", path));
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("  ")]
        public void ThrowExceptionOnImportDataFromJsonIfTheTableNameIsNullOrWhiteSpace(string tableName)
        {
            // Setup
            var path = Write("[]");

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => new FakeDbConnection().ImportDataFromJson(tableName, path));
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("  ")]
        public void ThrowExceptionOnImportDataFromJsonIfTheFileNameIsNullOrWhiteSpace(string filename)
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => new FakeDbConnection().ImportDataFromJson("Customer", filename));
        }

        [TestMethod]
        public void ThrowExceptionOnImportDataFromJsonIfTheFileDoesNotExist()
        {
            // Act/Assert
            Assert.Throws<FileNotFoundException>(() => new FakeDbConnection().ImportDataFromJson("Customer", Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".json")));
        }

        [DataTestMethod]
        [DataRow(0)]
        [DataRow(-5)]
        public void ThrowExceptionOnImportDataFromJsonIfTheBufferSizeIsLessThanOne(int size)
        {
            // Setup
            var path = Write("[]");

            // Act/Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => new FakeDbConnection().ImportDataFromJson("Customer", path, bufferSize: size));
        }

        [TestMethod]
        public async Task ThrowExceptionOnImportDataFromJsonAsyncIfTheConnectionIsNull()
        {
            // Setup
            var path = Write("[]");

            // Act/Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => ((IDbConnection)null).ImportDataFromJsonAsync("Customer", path));
        }

        [TestMethod]
        public async Task ThrowExceptionOnImportDataFromJsonAsyncIfTheFileDoesNotExist()
        {
            // Act/Assert
            await Assert.ThrowsAsync<FileNotFoundException>(() => new FakeDbConnection().ImportDataFromJsonAsync("Customer", Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".json")));
        }

        [TestMethod]
        public async Task ThrowExceptionOnImportDataFromJsonAsyncIfTheBufferSizeIsInvalid()
        {
            // Setup
            var path = Write("[]");

            // Act/Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => new FakeDbConnection().ImportDataFromJsonAsync("Customer", path, bufferSize: 0));
        }

        #endregion

        #region Batch Operations

        [TestMethod]
        public void TestImportDataFromJsonInsertsTheRowsInBatchesIfTheProviderHasNoBulkInsert()
        {
            // Setup
            var path = Write(Rows(5));
            var destination = new FakeDbConnection();
            var seen = new List<(int Batch, long Loaded, long Total)>();

            // Act
            var actual = destination.ImportDataFromJson("Customer", path, bufferSize: 2, progressCallback: Snapshot(seen));

            // Assert
            Assert.AreEqual(0, FakeBulkOperations.Calls.Length);
            Assert.AreEqual(5, destination.Executions.Count(e => e.CommandText.StartsWith("INSERT INTO", StringComparison.Ordinal)));
            Assert.AreEqual(5L, actual.TotalRowCount);
            Assert.AreEqual(5L, actual.LoadedRowCount);
            Assert.AreEqual(3, actual.BatchNumber);
            CollectionAssert.AreEqual(new[] { (0, 0L, 5L), (1, 2L, 5L), (2, 4L, 5L), (3, 5L, 5L), (3, 5L, 5L) }, seen);
        }

        [TestMethod]
        public void TestImportDataFromJsonInsertsTheValuesConvertedIntoTheTypesOfTheColumns()
        {
            // Setup
            var path = Write("[{\"Id\":7,\"Name\":\"Seven\",\"Birthday\":\"2026-01-02T03:04:05Z\"}]");
            var destination = new FakeDbConnection();

            // Act
            destination.ImportDataFromJson("Customer", path);

            // Assert
            var execution = destination.Executions.Single(e => e.CommandText.StartsWith("INSERT INTO", StringComparison.Ordinal));
            var values = execution.Parameters.Values.ToArray();
            CollectionAssert.AreEquivalent(new object[] { 7, "Seven", new DateTime(2026, 1, 2, 3, 4, 5, DateTimeKind.Utc) }, values);
        }

        [TestMethod]
        public void TestImportDataFromJsonCallsTheProgressCallbackWithTheSameInstanceThatItReturns()
        {
            // Setup
            var path = Write(Rows(3));
            var seen = new List<ImportProgress>();

            // Act
            var actual = new FakeDbConnection().ImportDataFromJson("Customer", path, bufferSize: 1, progressCallback: seen.Add);

            // Assert
            Assert.IsTrue(seen.Count >= 2);
            Assert.IsTrue(seen.All(p => ReferenceEquals(p, actual)));
        }

        [TestMethod]
        public void TestImportDataFromJsonCallsTheCallbackAfterTheRowsAreCountedAndWhenItEnds()
        {
            // Setup
            var path = Write(Rows(2));
            var seen = new List<(int Batch, long Loaded, long Total)>();

            // Act
            new FakeDbConnection().ImportDataFromJson("Customer", path, bufferSize: 50, progressCallback: p => seen.Add((p.BatchNumber, p.LoadedRowCount, p.TotalRowCount)));

            // Assert
            CollectionAssert.AreEqual(new[] { (0, 0L, 2L), (1, 2L, 2L), (1, 2L, 2L) }, seen);
        }

        [TestMethod]
        public void TestImportDataFromJsonSetsTheStartInUtc()
        {
            // Setup
            var path = Write(Rows(1));
            var before = DateTime.UtcNow;

            // Act
            var actual = new FakeDbConnection().ImportDataFromJson("Customer", path);

            // Assert
            Assert.AreEqual(DateTimeKind.Utc, actual.Start.Kind);
            Assert.IsTrue(actual.Start >= before);
            Assert.IsTrue(actual.Start <= DateTime.UtcNow);
        }

        [TestMethod]
        public void TestImportDataFromJsonOfAFileWithoutRowsCallsTheCallbackTwiceAndInsertsNothing()
        {
            // Setup
            var path = Write("[]");
            var destination = new FakeBulkDbConnection();
            var seen = new List<(int Batch, long Loaded, long Total)>();

            // Act
            var actual = destination.ImportDataFromJson("Customer", path, progressCallback: p => seen.Add((p.BatchNumber, p.LoadedRowCount, p.TotalRowCount)));

            // Assert
            CollectionAssert.AreEqual(new[] { (0, 0L, 0L), (0, 0L, 0L) }, seen);
            Assert.AreEqual(0, FakeBulkOperations.Statements.Length);
            Assert.AreEqual(0, FakeBulkOperations.Calls.Length);
            Assert.AreEqual(0L, actual.LoadedRowCount);
        }

        [TestMethod]
        public void TestImportDataFromJsonWithoutACallbackStillLoadsTheRows()
        {
            // Setup
            var path = Write(Rows(4));
            var destination = new FakeDbConnection();

            // Act
            var actual = destination.ImportDataFromJson("Customer", path, bufferSize: 3, progressCallback: null);

            // Assert
            Assert.AreEqual(4L, actual.LoadedRowCount);
            Assert.AreEqual(2, actual.BatchNumber);
        }

        [TestMethod]
        public void TestImportDataFromJsonPassesTheCommandTimeoutOfTheBatchOperation()
        {
            // Setup
            var path = Write(Rows(2));
            var destination = new FakeDbConnection();

            // Act
            destination.ImportDataFromJson("Customer", path, commandTimeout: 33);

            // Assert
            Assert.IsTrue(destination.Executions.Where(e => e.Kind == "NonQuery").All(e => e.CommandTimeout == 33));
        }

        [TestMethod]
        public void TestImportDataFromJsonIgnoresTheColumnsOfTheFileThatTheTableDoesNotHave()
        {
            // Setup
            var path = Write("[{\"Id\":1,\"Name\":\"A\",\"Unknown\":\"x\"},{\"Id\":2,\"Name\":\"B\",\"Unknown\":\"y\"}]");
            var destination = new FakeDbConnection();

            // Act
            var actual = destination.ImportDataFromJson("Customer", path);

            // Assert
            Assert.AreEqual(2L, actual.LoadedRowCount);
            var execution = destination.Executions.First(e => e.CommandText.StartsWith("INSERT INTO", StringComparison.Ordinal));
            Assert.AreEqual(2, execution.Parameters.Count);
            Assert.IsFalse(execution.CommandText.Contains("Unknown"));
        }

        [TestMethod]
        public void TestImportDataFromJsonInsertsTheMissingValuesAsNull()
        {
            // Setup
            var path = Write("[{\"Id\":1,\"Name\":\"A\",\"Birthday\":null},{\"Id\":2,\"Name\":\"B\"}]");
            var destination = new FakeDbConnection();

            // Act
            var actual = destination.ImportDataFromJson("Customer", path);

            // Assert
            Assert.AreEqual(2L, actual.LoadedRowCount);
        }

        #endregion

        #region Bulk Operations

        [TestMethod]
        public void TestImportDataFromJsonUsesTheBulkInsertInBlocksOfTheBufferSize()
        {
            // Setup
            var path = Write(Rows(10));
            var destination = new FakeBulkDbConnection();
            var seen = new List<(int Batch, long Loaded, long Total)>();

            // Act
            var actual = destination.ImportDataFromJson("Customer", path, bufferSize: 4, progressCallback: Snapshot(seen));

            // Assert
            var calls = FakeBulkOperations.Calls;
            CollectionAssert.AreEqual(new[] { 4, 4, 2 }, calls.Select(c => c.RowCount).ToArray());
            Assert.IsTrue(calls.All(c => c.TableName == "Customer" && c.Method == "BulkInsert"));
            Assert.AreEqual(0, FakeBulkOperations.Statements.Length);
            Assert.AreEqual(10L, actual.LoadedRowCount);
            Assert.AreEqual(3, actual.BatchNumber);
            CollectionAssert.AreEqual(new[] { (0, 0L, 10L), (1, 4L, 10L), (2, 8L, 10L), (3, 10L, 10L), (3, 10L, 10L) }, seen);
        }

        [TestMethod]
        public void TestImportDataFromJsonWithASingleRowUsesTheBulkInsert()
        {
            // Setup
            var path = Write(Rows(1));

            // Act
            new FakeBulkDbConnection().ImportDataFromJson("Customer", path);

            // Assert
            Assert.AreEqual(1, FakeBulkOperations.Calls.Length);
        }

        [TestMethod]
        public void TestImportDataFromJsonUsesTheBatchInsertIfTheProviderHasNoBulkInsert()
        {
            // Setup
            var path = Write(Rows(10));
            var destination = new FakeDbConnection();

            // Act
            var actual = destination.ImportDataFromJson("Customer", path, bufferSize: 4);

            // Assert
            Assert.AreEqual(0, FakeBulkOperations.Calls.Length);
            Assert.AreEqual(10L, actual.LoadedRowCount);
            Assert.AreEqual(3, actual.BatchNumber);
        }

        [TestMethod]
        public void TestImportDataFromJsonPassesTheArgumentsToTheBulkInsert()
        {
            // Setup
            var path = Write(Rows(6));
            var transaction = new Mock<IDbTransaction>().Object;

            // Act
            new FakeBulkDbConnection().ImportDataFromJson("Customer", path, bufferSize: 6, commandTimeout: 12, transaction: transaction);

            // Assert
            var call = FakeBulkOperations.Calls.Single();
            Assert.AreEqual(6, call.BatchSize);
            Assert.AreEqual(12, call.Timeout);
            Assert.AreSame(transaction, call.Transaction);
            Assert.AreEqual(DataTraceKeys.ImportDataFromJson, call.TraceKey);
        }

        [TestMethod]
        public void TestImportDataFromJsonReadsTheBulkRowsWithTheTypesOfTheColumns()
        {
            // Setup
            var path = Write(Rows(3));

            // Act
            var actual = new FakeBulkDbConnection().ImportDataFromJson("Customer", path);

            // Assert
            Assert.AreEqual(3L, actual.LoadedRowCount);
            Assert.AreEqual(3, FakeBulkOperations.Calls.Single().RowCount);
        }

        #endregion

        #region Shapes

        [TestMethod]
        public void TestImportDataFromJsonLoadsAnObjectThatHoldsTheRows()
        {
            // Setup
            var path = Write("{\"meta\":{\"x\":1},\"rows\":" + Rows(3) + "}");

            // Act
            var actual = new FakeDbConnection().ImportDataFromJson("Customer", path);

            // Assert
            Assert.AreEqual(3L, actual.TotalRowCount);
            Assert.AreEqual(3L, actual.LoadedRowCount);
        }

        [TestMethod]
        public void TestImportDataFromJsonLoadsASequenceOfObjects()
        {
            // Setup
            var path = Write(string.Join("\n", Enumerable.Range(1, 4).Select(i => "{\"Id\":" + i + ",\"Name\":\"Name " + i + "\"}")));

            // Act
            var actual = new FakeDbConnection().ImportDataFromJson("Customer", path, bufferSize: 3);

            // Assert
            Assert.AreEqual(4L, actual.TotalRowCount);
            Assert.AreEqual(4L, actual.LoadedRowCount);
            Assert.AreEqual(2, actual.BatchNumber);
        }

        [TestMethod]
        public void TestImportDataFromJsonLoadsAValueWithItsType()
        {
            // Setup
            var path = Write("[{\"Id\":{\"type\":\"Int32\",\"value\":9},\"Name\":{\"type\":\"String\",\"value\":\"Nine\"}}]");
            var destination = new FakeDbConnection();

            // Act
            destination.ImportDataFromJson("Customer", path);

            // Assert
            var values = destination.Executions.Single(e => e.CommandText.StartsWith("INSERT INTO", StringComparison.Ordinal)).Parameters.Values.ToArray();
            CollectionAssert.AreEquivalent(new object[] { 9, "Nine" }, values);
        }

        #endregion

        #region Errors

        [TestMethod]
        public void ThrowExceptionOnImportDataFromJsonIfNoColumnOfTheFileMatchesAColumnOfTheTable()
        {
            // Setup
            var path = Write("[{\"A\":1,\"B\":2}]");

            // Act/Assert
            var exception = Assert.Throws<InvalidOperationException>(() => new FakeDbConnection().ImportDataFromJson("Customer", path));
            StringAssert.Contains(exception.Message, "Customer", StringComparison.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnImportDataFromJsonIfAValueCannotBeConvertedIntoTheTypeOfItsColumn()
        {
            // Setup
            var path = Write("[{\"Id\":1,\"Name\":\"A\"},{\"Id\":1.5,\"Name\":\"B\"}]");

            // Act/Assert
            var exception = Assert.Throws<FormatException>(() => new FakeDbConnection().ImportDataFromJson("Customer", path));
            StringAssert.Contains(exception.Message, "Row 2", StringComparison.Ordinal);
            StringAssert.Contains(exception.Message, "Id", StringComparison.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnImportDataFromJsonIfTheFileIsNotValidJson()
        {
            // Setup
            var path = Write("[{\"Id\":1},");

            // Act/Assert
            Assert.Throws<FormatException>(() => new FakeDbConnection().ImportDataFromJson("Customer", path));
        }

        [TestMethod]
        public void TestImportDataFromJsonStopsAtTheBatchThatFailsAndKeepsTheProgressOfTheOnesThatSucceeded()
        {
            // Setup
            var path = Write(Rows(6));
            var seen = new List<ImportProgress>();
            var destination = new FakeDbConnection();
            var calls = 0;

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() =>
                destination.ImportDataFromJson("Customer", path, bufferSize: 2, progressCallback: p =>
                {
                    seen.Add(p);
                    if (++calls == 3)
                    {
                        throw new InvalidOperationException("stop");
                    }
                }));
            Assert.AreEqual(3, seen.Count);
            Assert.AreEqual(2, seen.Last().BatchNumber);
            Assert.AreEqual(4L, seen.Last().LoadedRowCount);
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestImportDataFromJsonAsyncInsertsTheRowsInBatchesIfTheProviderHasNoBulkInsert()
        {
            // Setup
            var path = Write(Rows(5));
            var destination = new FakeDbConnection();
            var seen = new List<(int Batch, long Loaded, long Total)>();

            // Act
            var actual = await destination.ImportDataFromJsonAsync("Customer", path, bufferSize: 2, progressCallback: Snapshot(seen));

            // Assert
            Assert.AreEqual(0, FakeBulkOperations.Calls.Length);
            Assert.AreEqual(5L, actual.LoadedRowCount);
            Assert.AreEqual(3, actual.BatchNumber);
            CollectionAssert.AreEqual(new[] { (0, 0L, 5L), (1, 2L, 5L), (2, 4L, 5L), (3, 5L, 5L), (3, 5L, 5L) }, seen);
        }

        [TestMethod]
        public async Task TestImportDataFromJsonAsyncUsesTheBulkInsertInBlocksOfTheBufferSize()
        {
            // Setup
            var path = Write(Rows(10));
            var destination = new FakeBulkDbConnection();

            // Act
            var actual = await destination.ImportDataFromJsonAsync("Customer", path, bufferSize: 4);

            // Assert
            var calls = FakeBulkOperations.Calls;
            CollectionAssert.AreEqual(new[] { 4, 4, 2 }, calls.Select(c => c.RowCount).ToArray());
            Assert.IsTrue(calls.All(c => c.Method == "BulkInsertAsync"));
            Assert.AreEqual(10L, actual.LoadedRowCount);
            Assert.AreEqual(3, actual.BatchNumber);
        }

        [TestMethod]
        public async Task TestImportDataFromJsonAsyncLoadsASequenceOfObjects()
        {
            // Setup
            var path = Write(string.Join("\n", Enumerable.Range(1, 4).Select(i => "{\"Id\":" + i + ",\"Name\":\"Name " + i + "\"}")));

            // Act
            var actual = await new FakeDbConnection().ImportDataFromJsonAsync("Customer", path, bufferSize: 3);

            // Assert
            Assert.AreEqual(4L, actual.LoadedRowCount);
            Assert.AreEqual(2, actual.BatchNumber);
        }

        [TestMethod]
        public async Task TestImportDataFromJsonAsyncOfAFileWithoutRowsCallsTheCallbackTwice()
        {
            // Setup
            var path = Write("[]");
            var count = 0;

            // Act
            var actual = await new FakeDbConnection().ImportDataFromJsonAsync("Customer", path, progressCallback: _ => count++);

            // Assert
            Assert.AreEqual(2, count);
            Assert.AreEqual(0L, actual.TotalRowCount);
        }

        [TestMethod]
        public async Task ThrowExceptionOnImportDataFromJsonAsyncIfNoColumnOfTheFileMatchesAColumnOfTheTable()
        {
            // Setup
            var path = Write("[{\"A\":1}]");

            // Act/Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => new FakeDbConnection().ImportDataFromJsonAsync("Customer", path));
        }

        [TestMethod]
        public async Task ThrowExceptionOnImportDataFromJsonAsyncIfAValueCannotBeConvertedIntoTheTypeOfItsColumn()
        {
            // Setup
            var path = Write("[{\"Id\":\"abc\"}]");

            // Act/Assert
            await Assert.ThrowsAsync<FormatException>(() => new FakeDbConnection().ImportDataFromJsonAsync("Customer", path));
        }

        [TestMethod]
        public async Task ThrowExceptionOnImportDataFromJsonAsyncIfTheTokenIsCancelled()
        {
            // Setup
            var path = Write(Rows(5));
            using (var source = new CancellationTokenSource())
            {
                source.Cancel();

                // Act/Assert
                await Assert.ThrowsAsync<OperationCanceledException>(() => new FakeDbConnection().ImportDataFromJsonAsync("Customer", path, cancellationToken: source.Token));
            }
        }

        #endregion

        #region Identity And Column Mapping Behaviors

        [TestMethod]
        public void ThrowExceptionOnImportDataFromJsonIfTheColumnMappingBehaviorIsNotDefined()
        {
            // Setup
            var path = Write("[]");

            // Act/Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => new FakeDbConnection().ImportDataFromJson("Customer", path, columnMappingBehavior: (ColumnMappingBehavior)99));
        }

        [TestMethod]
        public void ThrowExceptionOnImportDataFromJsonIfTheIdentityBehaviorIsNotDefined()
        {
            // Setup
            var path = Write("[]");

            // Act/Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => new FakeDbConnection().ImportDataFromJson("Customer", path, identityBehavior: (IdentityBehavior)99));
        }

        [TestMethod]
        public async Task ThrowExceptionOnImportDataFromJsonAsyncIfTheIdentityBehaviorIsNotDefined()
        {
            // Setup
            var path = Write("[]");

            // Act/Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => new FakeDbConnection().ImportDataFromJsonAsync("Customer", path, identityBehavior: (IdentityBehavior)99));
        }

        [TestMethod]
        public void TestImportDataFromJsonKeepsTheIdentityColumnByDefaultInTheBatchInsert()
        {
            // Setup
            var path = Write(Rows(1));
            var destination = new FakeDbConnection();

            // Act
            destination.ImportDataFromJson("Customer", path);

            // Assert
            var execution = destination.Executions.Single(e => e.CommandText.StartsWith("INSERT INTO", StringComparison.Ordinal));
            Assert.IsTrue(System.Text.RegularExpressions.Regex.IsMatch(execution.CommandText, @"\bId\b"));
            Assert.AreEqual(3, execution.Parameters.Count);
        }

        [TestMethod]
        public void TestImportDataFromJsonLeavesTheIdentityColumnToTheDatabaseInTheBatchInsertIfItIsNotToKeepIt()
        {
            // Setup
            var path = Write(Rows(1));
            var destination = new FakeDbConnection();

            // Act
            destination.ImportDataFromJson("Customer", path, identityBehavior: IdentityBehavior.Ignore);

            // Assert
            var execution = destination.Executions.Single(e => e.CommandText.StartsWith("INSERT INTO", StringComparison.Ordinal));
            Assert.IsFalse(System.Text.RegularExpressions.Regex.IsMatch(execution.CommandText, @"\bId\b"));
            Assert.AreEqual(2, execution.Parameters.Count);
        }

        [TestMethod]
        public async Task TestImportDataFromJsonAsyncLeavesTheIdentityColumnToTheDatabaseInTheBatchInsertIfItIsNotToKeepIt()
        {
            // Setup
            var path = Write(Rows(1));
            var destination = new FakeDbConnection();

            // Act
            await destination.ImportDataFromJsonAsync("Customer", path, identityBehavior: IdentityBehavior.Ignore);

            // Assert
            var execution = destination.Executions.Single(e => e.CommandText.StartsWith("INSERT INTO", StringComparison.Ordinal));
            Assert.IsFalse(System.Text.RegularExpressions.Regex.IsMatch(execution.CommandText, @"\bId\b"));
        }

        [TestMethod]
        public void ThrowExceptionOnImportDataFromJsonIfTheIdentityColumnIsTheOnlyColumnAndItIsNotToBeKept()
        {
            // Setup
            var path = Write("[{\"Id\":1}]");

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => new FakeDbConnection().ImportDataFromJson("Customer", path, identityBehavior: IdentityBehavior.Ignore));
        }

        [TestMethod]
        public void TestImportDataFromJsonPassesTheKeepIdentityOptionToTheBulkInsertByDefault()
        {
            // Setup
            var path = Write(Rows(3));

            // Act
            new FakeBulkOptionsDbConnection().ImportDataFromJson("Customer", path);

            // Assert
            Assert.AreEqual(FakeBulkCopyOptions.KeepIdentity, FakeBulkOperations.Calls.Single().Options);
        }

        [TestMethod]
        public void TestImportDataFromJsonDoesNotPassTheKeepIdentityOptionToTheBulkInsertIfItIsNotToKeepIt()
        {
            // Setup
            var path = Write(Rows(3));

            // Act
            new FakeBulkOptionsDbConnection().ImportDataFromJson("Customer", path, identityBehavior: IdentityBehavior.Ignore);

            // Assert
            Assert.AreEqual(FakeBulkCopyOptions.Default, FakeBulkOperations.Calls.Single().Options);
        }

        [TestMethod]
        public async Task TestImportDataFromJsonAsyncPassesTheIdentityBehaviorToTheBulkInsert()
        {
            // Setup
            var path = Write(Rows(3));

            // Act
            await new FakeBulkIdentityBehaviorDbConnection().ImportDataFromJsonAsync("Customer", path);

            // Assert
            Assert.AreEqual(FakeBulkImportIdentityBehavior.KeepIdentity, FakeBulkOperations.Calls.Single().IdentityBehavior);
        }

        [TestMethod]
        public void TestImportDataFromJsonDoesNotSendTheIdentityColumnToTheBulkInsertIfItIsNotToKeepIt()
        {
            // Setup
            var path = Write(Rows(3));

            // Act
            new FakeBulkIdentityBehaviorDbConnection().ImportDataFromJson("Customer", path, identityBehavior: IdentityBehavior.Ignore);

            // Assert
            Assert.IsNull(FakeBulkOperations.Calls.Single().IdentityBehavior);
            CollectionAssert.AreEqual(new[] { "Name", "Birthday" }, FakeBulkOperations.LastColumns);
        }

        [TestMethod]
        public void TestImportDataFromJsonSendsTheIdentityColumnToTheBulkInsertIfItIsToKeepIt()
        {
            // Setup
            var path = Write(Rows(3));

            // Act
            new FakeBulkIdentityBehaviorDbConnection().ImportDataFromJson("Customer", path);

            // Assert
            CollectionAssert.AreEqual(new[] { "Id", "Name", "Birthday" }, FakeBulkOperations.LastColumns);
        }

        [DataTestMethod]
        [DataRow(ColumnMappingBehavior.Automatic, "Automatic")]
        [DataRow(ColumnMappingBehavior.Strict, "Strict")]
        [DataRow(ColumnMappingBehavior.StrictBypass, "StrictBypass")]
        public void TestImportDataFromJsonPassesTheColumnMappingBehaviorToTheBulkInsert(ColumnMappingBehavior behavior,
            string expected)
        {
            // Setup
            var path = Write(Rows(3));

            // Act
            new FakeBulkSettingDbConnection().ImportDataFromJson("Customer", path, columnMappingBehavior: behavior);

            // Assert
            Assert.AreEqual(expected, FakeBulkOperations.Calls.Single().ColumnMappingsBehavior);
        }

        [TestMethod]
        public async Task TestImportDataFromJsonAsyncPassesTheColumnMappingBehaviorToTheBulkInsert()
        {
            // Setup
            var path = Write(Rows(3));

            // Act
            await new FakeBulkSettingDbConnection().ImportDataFromJsonAsync("Customer", path, columnMappingBehavior: ColumnMappingBehavior.StrictBypass);

            // Assert
            Assert.AreEqual("StrictBypass", FakeBulkOperations.Calls.Single().ColumnMappingsBehavior);
        }

        [TestMethod]
        public void TestImportDataFromJsonIgnoresTheColumnMappingBehaviorInTheBatchInsert()
        {
            // Setup
            var path = Write(Rows(2));
            var destination = new FakeDbConnection();

            // Act
            var actual = destination.ImportDataFromJson("Customer", path, columnMappingBehavior: ColumnMappingBehavior.Strict);

            // Assert
            Assert.AreEqual(2L, actual.LoadedRowCount);
        }

        #endregion
    }
}
