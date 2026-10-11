#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Npgsql;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RepoDb.Connector.EnterpriseDb;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Data.EnterpriseDb.IntegrationTests.Entities;
using RepoDb.Enumerations;
using RepoDb.Data.EnterpriseDb.IntegrationTests.Setup;
using RepoDb.Schema;

namespace RepoDb.Data.EnterpriseDb.IntegrationTests
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
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
            using (var target = new EDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Country");

                // Act
                var actual = source.CopyDataTo("Country", target);

                // Assert
                Assert.AreEqual(3, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("\"public\".\"Country\"", "\"Id\""), Helper.TargetRows("\"public\".\"Country\"", "\"Id\""));
            }
        }

        [TestMethod]
        [DataRow("Country", "\"public\".\"Country\"", "\"Id\"", 3)]
        [DataRow("Currency", "\"public\".\"Currency\"", "\"Code\"", 3)]
        [DataRow("Sample", "\"public\".\"Sample\"", "\"Id\"", 3)]
        [DataRow("\"Order Details\"", "\"public\".\"Order Details\"", "\"Id\"", 2)]
        [DataRow("Employee", "\"public\".\"Employee\"", "\"Id\"", 3)]
        [DataRow("Number", "\"public\".\"Number\"", "\"Value\"", 2500)]
        [DataRow("NoKey", "\"public\".\"NoKey\"", "\"Value\", \"Amount\"", 3)]
        public void TestCopyDataToCopiesTheRowsOfDifferentTables(string tableName,
            string quotedName,
            string orderBy,
            int expected)
        {
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
            using (var target = new EDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.CreateTargetTables(tableName);

                // Act
                var actual = source.CopyDataTo(tableName, target);

                // Assert
                Assert.AreEqual(expected, actual);
                Assert.AreEqual(expected, Helper.TargetCount(quotedName));
                Helper.AssertRowsEqual(Helper.SourceRows(quotedName, orderBy), Helper.TargetRows(quotedName, orderBy));
            }
        }

        [TestMethod]
        public void TestCopyDataToCopiesTheValuesOfAllTheDataTypes()
        {
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
            using (var target = new EDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Sample");

                // Act
                source.CopyDataTo("Sample", target);

                // Assert
                var rows = Helper.TargetRows("\"public\".\"Sample\"", "\"Id\"");
                Assert.AreEqual(long.MaxValue, rows[0]["BigIntValue"]);
                Assert.AreEqual(true, rows[0]["BitValue"]);
                Assert.AreEqual(12345.6789m, rows[0]["DecimalValue"]);
                Assert.AreEqual(new DateTime(2026, 2, 3, 4, 5, 6).AddTicks(1234560), rows[0]["DateTime2Value"]);
                Assert.AreEqual(new Guid("6F9619FF-8B86-D011-B42D-00C04FC964FF"), rows[0]["GuidValue"]);
                CollectionAssert.AreEqual(new byte[] { 1, 2, 3, 4, 5 }, (byte[])rows[0]["BinaryValue"]);
                Assert.AreEqual("A long text with unicode: ñ é 日本語", rows[0]["TextValue"]);
                Assert.AreEqual(long.MinValue, rows[1]["BigIntValue"]);
                Assert.AreEqual(false, rows[1]["BitValue"]);
                Assert.AreEqual(new DateTime(9999, 12, 31), rows[1]["DateValue"]);
                CollectionAssert.AreEqual(Array.Empty<byte>(), (byte[])rows[1]["BinaryValue"]);
                Assert.AreEqual(string.Empty, rows[1]["TextValue"]);
                Assert.IsTrue(rows[2].Where(c => c.Key != "Id").All(c => c.Value == null));
            }
        }

        [TestMethod]
        public void TestCopyDataToKeepsTheIdentityValuesOfTheSourceRows()
        {
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
            using (var target = new EDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Country");

                // Act
                source.CopyDataTo("Country", target, where: new QueryGroup(new QueryField("Id", Operation.GreaterThan, 1)));

                // Assert
                var ids = Helper.TargetRows("\"public\".\"Country\"", "\"Id\"").Select(r => (int)r["Id"]).ToArray();
                CollectionAssert.AreEqual(new[] { 2, 3 }, ids);
            }
        }

        [TestMethod]
        public void TestCopyDataToDoesNotChangeTheSourceTable()
        {
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
            using (var target = new EDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Country");
                var before = Helper.SourceRows("\"public\".\"Country\"", "\"Id\"");

                // Act
                source.CopyDataTo("Country", target);

                // Assert
                Helper.AssertRowsEqual(before, Helper.SourceRows("\"public\".\"Country\"", "\"Id\""));
            }
        }

        [TestMethod]
        public void TestCopyDataToOfATableWithoutRowsCopiesNothing()
        {
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
            using (var target = new EDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Empty");

                // Act
                var actual = source.CopyDataTo("Empty", target);

                // Assert
                Assert.AreEqual(0, actual);
                Assert.AreEqual(0, Helper.TargetCount("\"public\".\"Empty\""));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithATargetTableOfAnotherNameCopiesIntoTheTargetTable()
        {
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
            using (var target = new EDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget("CREATE TABLE \"public\".\"CurrencyCopy\" (\"Code\" CHAR(3) NOT NULL, \"Name\" varchar(50) NOT NULL, CONSTRAINT \"PK_CurrencyCopy\" PRIMARY KEY (\"Code\"));");

                // Act
                var actual = source.CopyDataTo("Currency", "CurrencyCopy", target);

                // Assert
                Assert.AreEqual(3, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("\"public\".\"Currency\"", "\"Code\""), Helper.TargetRows("\"public\".\"CurrencyCopy\"", "\"Code\""));
            }
        }

        [TestMethod]
        public void TestCopyDataToKeepsTheRowsThatAlreadyExistInTheTargetTable()
        {
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
            using (var target = new EDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Currency");
                Helper.ExecuteOnTarget("INSERT INTO \"public\".\"Currency\" (\"Code\", \"Name\") VALUES ('USD', 'US Dollar');");

                // Act
                var actual = source.CopyDataTo("Currency", target);

                // Assert
                Assert.AreEqual(3, actual);
                Assert.AreEqual(4, Helper.TargetCount("\"public\".\"Currency\""));
                Assert.AreEqual(1, Helper.TargetCount("\"public\".\"Currency\"", "\"Code\" = 'USD'"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithABatchSizeAndACommandTimeoutCopiesAllTheRows()
        {
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
            using (var target = new EDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Number");

                // Act
                var actual = source.CopyDataTo("Number", target, batchSize: 100, commandTimeout: 60);

                // Assert
                Assert.AreEqual(2500, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("\"public\".\"Number\"", "\"Value\""), Helper.TargetRows("\"public\".\"Number\"", "\"Value\""));
            }
        }

        [TestMethod]
        public void TestCopyDataToCallsTheProgressCallback()
        {
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
            using (var target = new EDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Number");
                var progress = new List<(int Batch, int Rows, int Total, DateTime Start, DateTime End)>();

                // Act
                var actual = source.CopyDataTo("Number", target, batchSize: 500, progressCallback: p => progress.Add((p.BatchNumber, p.RowCount, p.TotalCopiedRowCount, p.StartTime, p.EndTime)));

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
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
            using (var target = new EDBConnection(Database.ConnectionStringForTarget))
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
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
            using (var target = new EDBConnection(Database.ConnectionStringForTarget))
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
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
            using (var target = new EDBConnection(Database.ConnectionStringForTarget))
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
                Assert.AreEqual(3, Helper.TargetCount("\"public\".\"Country\""));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithARolledBackTransactionLeavesTheTargetTableEmpty()
        {
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
            using (var target = new EDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Country");
                target.Open();
                using (var transaction = target.BeginTransaction())
                {
                    // Act
                    var actual = source.CopyDataTo("Country", target, transaction: transaction);
                    transaction.Rollback();

                    // Assert
                    Assert.AreEqual(3, actual);
                }
                Assert.AreEqual(0, Helper.TargetCount("\"public\".\"Country\""));
            }
        }

        #endregion

        #region Overloads

        [TestMethod]
        public void TestCopyDataToWithARegisteredConnection()
        {
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
            using (var target = new EDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Country");
                var name = "Target-" + Guid.NewGuid().ToString("N");
                ConnectionManager.Register(name, target);

                // Act
                var actual = source.CopyDataTo("Country", name);

                // Assert
                Assert.AreEqual(3, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("\"public\".\"Country\"", "\"Id\""), Helper.TargetRows("\"public\".\"Country\"", "\"Id\""));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithARegisteredConnectionAndATargetTable()
        {
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
            using (var target = new EDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget("CREATE TABLE \"public\".\"CurrencyCopy\" (\"Code\" CHAR(3) NOT NULL, \"Name\" varchar(50) NOT NULL, CONSTRAINT \"PK_CurrencyCopy\" PRIMARY KEY (\"Code\"));");
                var name = "Target-" + Guid.NewGuid().ToString("N");
                ConnectionManager.Register(name, target);

                // Act
                var actual = source.CopyDataTo("Currency", "CurrencyCopy", name);

                // Assert
                Assert.AreEqual(3, actual);
                Assert.AreEqual(3, Helper.TargetCount("\"public\".\"CurrencyCopy\""));
            }
        }

        [TestMethod]
        public void TestCopyDataToOfAnEntity()
        {
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
            using (var target = new EDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Currency");

                // Act
                var actual = source.CopyDataTo<Currency>(target);

                // Assert
                Assert.AreEqual(3, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("\"public\".\"Currency\"", "\"Code\""), Helper.TargetRows("\"public\".\"Currency\"", "\"Code\""));
            }
        }

        [TestMethod]
        public void TestCopyDataToOfAnEntityWithAnExpression()
        {
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
            using (var target = new EDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Number");

                // Act
                var actual = source.CopyDataTo<Number>(target, n => n.Value <= 10);

                // Assert
                Assert.AreEqual(10, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("\"public\".\"Number\"", "\"Value\"", "\"Value\" <= 10"), Helper.TargetRows("\"public\".\"Number\"", "\"Value\""));
            }
        }

        [TestMethod]
        public void TestCopyDataToOfAnEntityWithATargetTable()
        {
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
            using (var target = new EDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget("CREATE TABLE \"public\".\"CurrencyCopy\" (\"Code\" CHAR(3) NOT NULL, \"Name\" varchar(50) NOT NULL, CONSTRAINT \"PK_CurrencyCopy\" PRIMARY KEY (\"Code\"));");

                // Act
                var actual = source.CopyDataTo<Currency>("CurrencyCopy", target, c => c.Code != "JPY");

                // Assert
                Assert.AreEqual(2, actual);
                CollectionAssert.AreEqual(new[] { "EUR", "PHP" }, Helper.TargetRows("\"public\".\"CurrencyCopy\"", "\"Code\"").Select(r => (string)r["Code"]).ToArray());
            }
        }

        [TestMethod]
        public void TestCopyDataToOfAnEntityWithARegisteredConnection()
        {
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
            using (var target = new EDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Currency");
                var name = "Target-" + Guid.NewGuid().ToString("N");
                ConnectionManager.Register(name, target);

                // Act
                var actual = source.CopyDataTo<Currency>(name, c => c.Code == "EUR");

                // Assert
                Assert.AreEqual(1, actual);
                Assert.AreEqual("Euro", Helper.TargetRows("\"public\".\"Currency\"", "\"Code\"").Single()["Name"]);
            }
        }

        #endregion

        #endregion

        #region CopyDataToAsync

        #region Async

        [TestMethod]
        public async Task TestCopyDataToAsync()
        {
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
            using (var target = new EDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Country");

                // Act
                var actual = await source.CopyDataToAsync("Country", target);

                // Assert
                Assert.AreEqual(3, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("\"public\".\"Country\"", "\"Id\""), Helper.TargetRows("\"public\".\"Country\"", "\"Id\""));
            }
        }

        [TestMethod]
        [DataRow("Country", "\"public\".\"Country\"", "\"Id\"", 3)]
        [DataRow("Currency", "\"public\".\"Currency\"", "\"Code\"", 3)]
        [DataRow("Sample", "\"public\".\"Sample\"", "\"Id\"", 3)]
        [DataRow("\"Order Details\"", "\"public\".\"Order Details\"", "\"Id\"", 2)]
        [DataRow("Employee", "\"public\".\"Employee\"", "\"Id\"", 3)]
        [DataRow("Number", "\"public\".\"Number\"", "\"Value\"", 2500)]
        [DataRow("NoKey", "\"public\".\"NoKey\"", "\"Value\", \"Amount\"", 3)]
        public async Task TestCopyDataToAsyncCopiesTheRowsOfDifferentTables(string tableName,
            string quotedName,
            string orderBy,
            int expected)
        {
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
            using (var target = new EDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.CreateTargetTables(tableName);

                // Act
                var actual = await source.CopyDataToAsync(tableName, target);

                // Assert
                Assert.AreEqual(expected, actual);
                Helper.AssertRowsEqual(Helper.SourceRows(quotedName, orderBy), Helper.TargetRows(quotedName, orderBy));
            }
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncKeepsTheIdentityValuesOfTheSourceRows()
        {
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
            using (var target = new EDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Country");

                // Act
                await source.CopyDataToAsync("Country", target, where: new QueryGroup(new QueryField("Id", Operation.GreaterThan, 1)));

                // Assert
                var ids = Helper.TargetRows("\"public\".\"Country\"", "\"Id\"").Select(r => (int)r["Id"]).ToArray();
                CollectionAssert.AreEqual(new[] { 2, 3 }, ids);
            }
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncWithATargetTableOfAnotherNameCopiesIntoTheTargetTable()
        {
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
            using (var target = new EDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget("CREATE TABLE \"public\".\"CurrencyCopy\" (\"Code\" CHAR(3) NOT NULL, \"Name\" varchar(50) NOT NULL, CONSTRAINT \"PK_CurrencyCopy\" PRIMARY KEY (\"Code\"));");

                // Act
                var actual = await source.CopyDataToAsync("Currency", "CurrencyCopy", target);

                // Assert
                Assert.AreEqual(3, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("\"public\".\"Currency\"", "\"Code\""), Helper.TargetRows("\"public\".\"CurrencyCopy\"", "\"Code\""));
            }
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncCallsTheProgressCallback()
        {
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
            using (var target = new EDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Number");
                var progress = new List<(int Rows, int Total)>();

                // Act
                var actual = await source.CopyDataToAsync("Number", target, batchSize: 500, progressCallback: p => progress.Add((p.RowCount, p.TotalCopiedRowCount)));

                // Assert
                Assert.AreEqual(2500, actual);
                Assert.AreEqual(2500, progress.Last().Total);
                Assert.AreEqual(2500, progress.Sum(p => p.Rows));
            }
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncWithARolledBackTransactionLeavesTheTargetTableEmpty()
        {
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
            using (var target = new EDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Country");
                await target.OpenAsync();
                using (var transaction = target.BeginTransaction())
                {
                    // Act
                    var actual = await source.CopyDataToAsync("Country", target, transaction: transaction);
                    transaction.Rollback();

                    // Assert
                    Assert.AreEqual(3, actual);
                }
                Assert.AreEqual(0, Helper.TargetCount("\"public\".\"Country\""));
            }
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncWithARegisteredConnection()
        {
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
            using (var target = new EDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Country");
                var name = "Target-" + Guid.NewGuid().ToString("N");
                ConnectionManager.Register(name, target);

                // Act
                var actual = await source.CopyDataToAsync("Country", name);

                // Assert
                Assert.AreEqual(3, actual);
                Assert.AreEqual(3, Helper.TargetCount("\"public\".\"Country\""));
            }
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncOfAnEntityWithAnExpression()
        {
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
            using (var target = new EDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Number");

                // Act
                var actual = await source.CopyDataToAsync<Number>(target, n => n.Value > 2490);

                // Assert
                Assert.AreEqual(10, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("\"public\".\"Number\"", "\"Value\"", "\"Value\" > 2490"), Helper.TargetRows("\"public\".\"Number\"", "\"Value\""));
            }
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncOfAnEntityWithATargetTableAndARegisteredConnection()
        {
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
            using (var target = new EDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget("CREATE TABLE \"public\".\"CurrencyCopy\" (\"Code\" CHAR(3) NOT NULL, \"Name\" varchar(50) NOT NULL, CONSTRAINT \"PK_CurrencyCopy\" PRIMARY KEY (\"Code\"));");
                var name = "Target-" + Guid.NewGuid().ToString("N");
                ConnectionManager.Register(name, target);

                // Act
                var actual = await source.CopyDataToAsync<Currency>("CurrencyCopy", name, c => c.Code == "PHP");

                // Assert
                Assert.AreEqual(1, actual);
                Assert.AreEqual("Philippine Peso", Helper.TargetRows("\"public\".\"CurrencyCopy\"", "\"Code\"").Single()["Name"]);
            }
        }

        #endregion

        #endregion
    }
}
