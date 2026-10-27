#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Data.Firebird.IntegrationTests.Entities;
using RepoDb.Enumerations;
using RepoDb.Data.Firebird.IntegrationTests.Setup;
using RepoDb.Schema;

namespace RepoDb.Data.Firebird.IntegrationTests
{
    /// <summary>
    /// Copies the rows of a table into the same table of another database (the table already exists in the destination database).
    /// </summary>
    [TestClass]
    public class CopyDataToTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Database.Initialize();
            Cleanup();
        }

        [TestCleanup]
        public void Cleanup()
        {
            SchemaReaderMapper.Clear();
            Database.Cleanup();
        }

        #region CopyDataTo

        #region Sync

        [TestMethod]
        public void TestCopyDataTo()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Country");

                // Act
                var actual = source.CopyDataTo("Country", target).RowCount;

                // Assert
                Assert.AreEqual(3, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("\"Country\"", "\"Id\""), Helper.TargetRows("\"Country\"", "\"Id\""));
            }
        }

        [TestMethod]
        public void TestCopyDataToCopiesTheValuesOfAllTheDataTypes()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Sample");

                // Act
                source.CopyDataTo("Sample", target);

                // Assert
                var rows = Helper.TargetRows("\"Sample\"", "\"Id\"");
                Assert.AreEqual(long.MaxValue, rows[0]["BigIntValue"]);
                Assert.IsTrue(Convert.ToBoolean(rows[0]["BitValue"]));
                Assert.AreEqual(12345.6789m, rows[0]["DecimalValue"]);
                Assert.AreEqual(new DateTime(2026, 2, 3, 4, 5, 6).AddTicks(1234000), rows[0]["DateTime2Value"]);
                Assert.AreEqual("6f9619ff-8b86-d011-b42d-00c04fc964ff", rows[0]["GuidValue"]);
                CollectionAssert.AreEqual(new byte[] { 1, 2, 3, 4, 5 }, (byte[])rows[0]["BinaryValue"]);
                Assert.AreEqual("A long text with unicode: ñ é 日本語", rows[0]["TextValue"]);
                Assert.AreEqual(long.MinValue + 1, rows[1]["BigIntValue"]);
                Assert.IsFalse(Convert.ToBoolean(rows[1]["BitValue"]));
                Assert.AreEqual(new DateTime(9999, 12, 31), rows[1]["DateValue"]);
                CollectionAssert.AreEqual(new byte[] { 255 }, (byte[])rows[1]["BinaryValue"]);
                Assert.AreEqual("text", rows[1]["TextValue"]);
                Assert.IsTrue(rows[2].Where(c => c.Key != "Id").All(c => c.Value == null));
            }
        }

        [TestMethod]
        public void TestCopyDataToKeepsTheIdentityValuesOfTheSourceRows()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Country");

                // Act
                source.CopyDataTo("Country", target, where: new QueryGroup(new QueryField("Id", Operation.GreaterThan, 1)));

                // Assert
                var ids = Helper.TargetRows("\"Country\"", "\"Id\"").Select(r => (int)r["Id"]).ToArray();
                CollectionAssert.AreEqual(new[] { 2, 3 }, ids);
            }
        }

        [TestMethod]
        public void TestCopyDataToDoesNotChangeTheSourceTable()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Country");
                var before = Helper.SourceRows("\"Country\"", "\"Id\"");

                // Act
                source.CopyDataTo("Country", target);

                // Assert
                Helper.AssertRowsEqual(before, Helper.SourceRows("\"Country\"", "\"Id\""));
            }
        }

        [TestMethod]
        public void TestCopyDataToOfATableWithoutRowsCopiesNothing()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Empty");

                // Act
                var actual = source.CopyDataTo("Empty", target).RowCount;

                // Assert
                Assert.AreEqual(0, actual);
                Assert.AreEqual(0, Helper.TargetCount("\"Empty\""));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithATargetTableOfAnotherNameCopiesIntoTheTargetTable()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget("CREATE TABLE \"CurrencyCopy\" (\"Code\" CHAR(3) NOT NULL, \"Name\" VARCHAR(50) NOT NULL, CONSTRAINT \"PK_CurrencyCopy\" PRIMARY KEY (\"Code\"));");

                // Act
                var actual = source.CopyDataTo("Currency", "CurrencyCopy", target).RowCount;

                // Assert
                Assert.AreEqual(3, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("\"Currency\"", "\"Code\""), Helper.TargetRows("\"CurrencyCopy\"", "\"Code\""));
            }
        }

        [TestMethod]
        public void TestCopyDataToKeepsTheRowsThatAlreadyExistInTheTargetTable()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Currency");
                Helper.ExecuteOnTarget("INSERT INTO \"Currency\" (\"Code\", \"Name\") VALUES ('USD', 'US Dollar');");

                // Act
                var actual = source.CopyDataTo("Currency", target).RowCount;

                // Assert
                Assert.AreEqual(3, actual);
                Assert.AreEqual(4, Helper.TargetCount("\"Currency\""));
                Assert.AreEqual(1, Helper.TargetCount("\"Currency\"", "\"Code\" = 'USD'"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithABufferSizeAndACommandTimeoutCopiesAllTheRows()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Number");

                // Act
                var actual = source.CopyDataTo("Number", target, bufferSize: 100, commandTimeout: 60).RowCount;

                // Assert
                Assert.AreEqual(2500, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("\"Number\"", "\"Value\""), Helper.TargetRows("\"Number\"", "\"Value\""));
            }
        }

        [TestMethod]
        public void TestCopyDataToCallsTheProgressCallback()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Number");
                var progress = new List<(int Batch, int Rows, int Total, DateTime Start, DateTime End)>();

                // Act
                var actual = source.CopyDataTo("Number", target, bufferSize: 500, progressCallback: p => { if (p.CurrentBatch != null) progress.Add((p.BatchCount, p.CurrentBatch.RowCount, p.RowCount, p.StartTime, p.EndTime)); }).RowCount;

                // Assert
                Assert.AreEqual(2500, actual);
                Assert.IsTrue(progress.Count >= 1);
                Assert.AreEqual(2500, progress.Last().Total);
                Assert.AreEqual(2500, progress.Sum(p => p.Rows));
                Assert.AreEqual(progress.Count, progress.Last().Batch);
                Assert.IsTrue(progress.Last().Start != default(DateTime));
                Assert.IsTrue(progress.Last().End >= progress.Last().Start);
            }
        }

        [TestMethod]
        public void TestCopyDataToTracesTheReadingOfTheSourceTableWithTheDefaultTraceKey()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Country");
                var trace = new RecordingTrace();

                // Act
                source.CopyDataTo("Country", target, trace: trace);

                // Assert
                CollectionAssert.Contains(trace.Keys, DataTraceKeys.CopyDataTo);
            }
        }

        [TestMethod]
        public void TestCopyDataToWithATraceKeyTracesTheReadingOfTheSourceTableWithTheKey()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Country");
                var trace = new RecordingTrace();

                // Act
                source.CopyDataTo("Country", target, traceKey: "MyCopy", trace: trace);

                // Assert
                CollectionAssert.Contains(trace.Keys, "MyCopy");
            }
        }

        [TestMethod]
        public void TestCopyDataToWithACommittedTransactionKeepsTheRows()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Country");
                target.Open();
                using (var transaction = target.BeginTransaction())
                {
                    // Act
                    source.CopyDataTo("Country", target, transaction: transaction);
                    transaction.Commit();
                }

                // Assert
                Assert.AreEqual(3, Helper.TargetCount("\"Country\""));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithARolledBackTransactionLeavesTheTargetTableEmpty()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Country");
                target.Open();
                using (var transaction = target.BeginTransaction())
                {
                    // Act
                    var actual = source.CopyDataTo("Country", target, transaction: transaction).RowCount;
                    transaction.Rollback();

                    // Assert
                    Assert.AreEqual(3, actual);
                }
                Assert.AreEqual(0, Helper.TargetCount("\"Country\""));
            }
        }

        #endregion

        #region Overloads

        [TestMethod]
        public void TestCopyDataToWithARegisteredConnection()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Country");
                var name = "Target-" + Guid.NewGuid().ToString("N");
                ConnectionManager.Register(name, target);

                // Act
                var actual = source.CopyDataTo("Country", name).RowCount;

                // Assert
                Assert.AreEqual(3, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("\"Country\"", "\"Id\""), Helper.TargetRows("\"Country\"", "\"Id\""));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithARegisteredConnectionAndATargetTable()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget("CREATE TABLE \"CurrencyCopy\" (\"Code\" CHAR(3) NOT NULL, \"Name\" VARCHAR(50) NOT NULL, CONSTRAINT \"PK_CurrencyCopy\" PRIMARY KEY (\"Code\"));");
                var name = "Target-" + Guid.NewGuid().ToString("N");
                ConnectionManager.Register(name, target);

                // Act
                var actual = source.CopyDataTo("Currency", "CurrencyCopy", name).RowCount;

                // Assert
                Assert.AreEqual(3, actual);
                Assert.AreEqual(3, Helper.TargetCount("\"CurrencyCopy\""));
            }
        }

        [TestMethod]
        public void TestCopyDataToOfAnEntity()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Currency");

                // Act
                var actual = source.CopyDataTo<Currency>(target).RowCount;

                // Assert
                Assert.AreEqual(3, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("\"Currency\"", "\"Code\""), Helper.TargetRows("\"Currency\"", "\"Code\""));
            }
        }

        [TestMethod]
        public void TestCopyDataToOfAnEntityWithAnExpression()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Number");

                // Act
                var actual = source.CopyDataTo<Number>(target, n => n.Value <= 10).RowCount;

                // Assert
                Assert.AreEqual(10, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("\"Number\"", "\"Value\"", "\"Value\" <= 10"), Helper.TargetRows("\"Number\"", "\"Value\""));
            }
        }

        [TestMethod]
        public void TestCopyDataToOfAnEntityWithATargetTable()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget("CREATE TABLE \"CurrencyCopy\" (\"Code\" CHAR(3) NOT NULL, \"Name\" VARCHAR(50) NOT NULL, CONSTRAINT \"PK_CurrencyCopy\" PRIMARY KEY (\"Code\"));");

                // Act
                var actual = source.CopyDataTo<Currency>("CurrencyCopy", target, c => c.Code != "JPY").RowCount;

                // Assert
                Assert.AreEqual(2, actual);
                CollectionAssert.AreEqual(new[] { "EUR", "PHP" }, Helper.TargetRows("\"CurrencyCopy\"", "\"Code\"").Select(r => (string)r["Code"]).ToArray());
            }
        }

        [TestMethod]
        public void TestCopyDataToOfAnEntityWithARegisteredConnection()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Currency");
                var name = "Target-" + Guid.NewGuid().ToString("N");
                ConnectionManager.Register(name, target);

                // Act
                var actual = source.CopyDataTo<Currency>(name, c => c.Code == "EUR").RowCount;

                // Assert
                Assert.AreEqual(1, actual);
                Assert.AreEqual("Euro", Helper.TargetRows("\"Currency\"", "\"Code\"").Single()["Name"]);
            }
        }

        #endregion

        #endregion

        #region CopyDataToAsync

        #region Async

        [TestMethod]
        public async Task TestCopyDataToAsync()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Country");

                // Act
                var actual = (await source.CopyDataToAsync("Country", target)).RowCount;

                // Assert
                Assert.AreEqual(3, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("\"Country\"", "\"Id\""), Helper.TargetRows("\"Country\"", "\"Id\""));
            }
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncKeepsTheIdentityValuesOfTheSourceRows()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Country");

                // Act
                await source.CopyDataToAsync("Country", target, where: new QueryGroup(new QueryField("Id", Operation.GreaterThan, 1)));

                // Assert
                var ids = Helper.TargetRows("\"Country\"", "\"Id\"").Select(r => (int)r["Id"]).ToArray();
                CollectionAssert.AreEqual(new[] { 2, 3 }, ids);
            }
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncWithATargetTableOfAnotherNameCopiesIntoTheTargetTable()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget("CREATE TABLE \"CurrencyCopy\" (\"Code\" CHAR(3) NOT NULL, \"Name\" VARCHAR(50) NOT NULL, CONSTRAINT \"PK_CurrencyCopy\" PRIMARY KEY (\"Code\"));");

                // Act
                var actual = (await source.CopyDataToAsync("Currency", "CurrencyCopy", target)).RowCount;

                // Assert
                Assert.AreEqual(3, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("\"Currency\"", "\"Code\""), Helper.TargetRows("\"CurrencyCopy\"", "\"Code\""));
            }
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncCallsTheProgressCallback()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Number");
                var progress = new List<(int Rows, int Total)>();

                // Act
                var actual = (await source.CopyDataToAsync("Number", target, bufferSize: 500, progressCallback: p => { if (p.CurrentBatch != null) progress.Add((p.CurrentBatch.RowCount, p.RowCount)); })).RowCount;

                // Assert
                Assert.AreEqual(2500, actual);
                Assert.AreEqual(2500, progress.Last().Total);
                Assert.AreEqual(2500, progress.Sum(p => p.Rows));
            }
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncWithARolledBackTransactionLeavesTheTargetTableEmpty()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Country");
                await target.OpenAsync();
                using (var transaction = target.BeginTransaction())
                {
                    // Act
                    var actual = (await source.CopyDataToAsync("Country", target, transaction: transaction)).RowCount;
                    transaction.Rollback();

                    // Assert
                    Assert.AreEqual(3, actual);
                }
                Assert.AreEqual(0, Helper.TargetCount("\"Country\""));
            }
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncWithARegisteredConnection()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Country");
                var name = "Target-" + Guid.NewGuid().ToString("N");
                ConnectionManager.Register(name, target);

                // Act
                var actual = (await source.CopyDataToAsync("Country", name)).RowCount;

                // Assert
                Assert.AreEqual(3, actual);
                Assert.AreEqual(3, Helper.TargetCount("\"Country\""));
            }
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncOfAnEntityWithAnExpression()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Number");

                // Act
                var actual = (await source.CopyDataToAsync<Number>(target, n => n.Value > 2490)).RowCount;

                // Assert
                Assert.AreEqual(10, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("\"Number\"", "\"Value\"", "\"Value\" > 2490"), Helper.TargetRows("\"Number\"", "\"Value\""));
            }
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncOfAnEntityWithATargetTableAndARegisteredConnection()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget("CREATE TABLE \"CurrencyCopy\" (\"Code\" CHAR(3) NOT NULL, \"Name\" VARCHAR(50) NOT NULL, CONSTRAINT \"PK_CurrencyCopy\" PRIMARY KEY (\"Code\"));");
                var name = "Target-" + Guid.NewGuid().ToString("N");
                ConnectionManager.Register(name, target);

                // Act
                var actual = (await source.CopyDataToAsync<Currency>("CurrencyCopy", name, c => c.Code == "PHP")).RowCount;

                // Assert
                Assert.AreEqual(1, actual);
                Assert.AreEqual("Philippine Peso", Helper.TargetRows("\"CurrencyCopy\"", "\"Code\"").Single()["Name"]);
            }
        }

        #endregion

        #endregion
    }
}
