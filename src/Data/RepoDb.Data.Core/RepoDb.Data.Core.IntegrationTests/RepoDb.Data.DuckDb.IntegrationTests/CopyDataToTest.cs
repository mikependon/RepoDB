#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Data.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DuckDB.NET.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Data.DuckDb.IntegrationTests.Entities;
using RepoDb.Enumerations;
using RepoDb.Data.DuckDb.IntegrationTests.Setup;
using RepoDb.Schema;

namespace RepoDb.Data.DuckDb.IntegrationTests
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
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
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
        [DataRow("Country", "\"Country\"", "\"Id\"", 3)]
        [DataRow("Currency", "\"Currency\"", "\"Code\"", 3)]
        [DataRow("Sample", "\"Sample\"", "\"Id\"", 3)]
        [DataRow("\"Order Details\"", "\"Order Details\"", "\"Id\"", 2)]
        [DataRow("Sales.Invoice", "\"Sales\".\"Invoice\"", "\"Id\"", 2)]
        [DataRow("Number", "\"Number\"", "\"Value\"", 2500)]
        [DataRow("NoKey", "\"NoKey\"", "\"Value\", \"Amount\"", 3)]
        public void TestCopyDataToCopiesTheRowsOfDifferentTables(string tableName,
            string quotedName,
            string orderBy,
            int expected)
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.CreateTargetTables(tableName);

                // Act
                var actual = source.CopyDataTo(tableName, target).RowCount;

                // Assert
                Assert.AreEqual(expected, actual);
                Assert.AreEqual(expected, Helper.TargetCount(quotedName));
                Helper.AssertRowsEqual(Helper.SourceRows(quotedName, orderBy), Helper.TargetRows(quotedName, orderBy));
            }
        }

        [TestMethod]
        public void TestCopyDataToGeneratesTheIdentityValuesIfItIsNotToKeepThem()
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Country");
                Helper.ExecuteOnTarget("INSERT INTO \"Country\" (\"Name\") VALUES ('Other')");

                // Act
                var actual = source.CopyDataTo("Country", target, identityBehavior: IdentityBehavior.Ignore);

                // Assert
                Assert.AreEqual(3, actual.RowCount);
                var rows = Helper.TargetRows("\"Country\"", "\"Id\"");
                var ids = rows.Select(r => Convert.ToInt64(r["Id"])).ToArray();
                CollectionAssert.AreEqual(new[] { "Other", "Philippines", "Japan", "Germany" }, rows.Select(r => (string)r["Name"]).ToArray());
                Assert.AreEqual(4, ids.Distinct().Count());
                Assert.IsTrue(ids[0] < ids[1] && ids[1] < ids[2] && ids[2] < ids[3]);
                Assert.AreNotEqual(1L, ids[1]);
            }
        }

        [TestMethod]
        public void TestCopyDataToKeepsTheIdentityValuesOfTheSource()
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Country");

                // Act
                source.CopyDataTo("Country", target, identityBehavior: IdentityBehavior.KeepIdentity);

                // Assert
                var rows = Helper.TargetRows("\"Country\"", "\"Id\"");
                CollectionAssert.AreEqual(new[] { "Philippines", "Japan", "Germany" }, rows.Select(r => (string)r["Name"]).ToArray());
                CollectionAssert.AreEqual(new long[] { 1, 2, 3 }, rows.Select(r => Convert.ToInt64(r["Id"])).ToArray());
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheIdentityValuesAreGeneratedWithTheRelatedTables()
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                // Act/Assert
                Assert.Throws<ArgumentException>(() => source.CopyDataTo("Person", target, relationshipBehavior: RelationshipBehavior.Parents, identityBehavior: IdentityBehavior.Ignore));
            }
        }

        [TestMethod]
        public void TestCopyDataToCopiesTheValuesOfAllTheDataTypes()
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Sample");

                // Act
                source.CopyDataTo("Sample", target);

                // Assert
                var rows = Helper.TargetRows("\"Sample\"", "\"Id\"");
                Assert.AreEqual(long.MaxValue, rows[0]["BigIntValue"]);
                Assert.AreEqual(true, rows[0]["BitValue"]);
                Assert.AreEqual(12345.6789m, rows[0]["DecimalValue"]);
                Assert.AreEqual(new DateTime(2026, 2, 3, 4, 5, 6).AddTicks(1234560), rows[0]["DateTime2Value"]);
                Assert.AreEqual(new Guid("6F9619FF-8B86-D011-B42D-00C04FC964FF"), rows[0]["GuidValue"]);
                CollectionAssert.AreEqual(new byte[] { 1, 2, 3, 4, 5 }, (byte[])rows[0]["BinaryValue"]);
                Assert.AreEqual("A long text with unicode: ñ é 日本語", rows[0]["TextValue"]);
                Assert.AreEqual(long.MinValue + 1, rows[1]["BigIntValue"]);
                Assert.AreEqual(false, rows[1]["BitValue"]);
                Assert.AreEqual(new DateOnly(9999, 12, 31), rows[1]["DateValue"]);
                CollectionAssert.AreEqual(Array.Empty<byte>(), (byte[])rows[1]["BinaryValue"]);
                Assert.AreEqual(string.Empty, rows[1]["TextValue"]);
                Assert.IsTrue(rows[2].Where(c => c.Key != "Id").All(c => c.Value == null));
            }
        }

        [TestMethod]
        public void TestCopyDataToKeepsTheIdentityValuesOfTheSourceRows()
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
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
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
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
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
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
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget("CREATE TABLE \"CurrencyCopy\" (\"Code\" CHAR(3) NOT NULL, \"Name\" varchar(50) NOT NULL, CONSTRAINT \"PK_CurrencyCopy\" PRIMARY KEY (\"Code\"));");

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
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
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
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
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
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
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
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
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
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
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
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
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
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
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
        public void TestCopyDataToWithADestinationConnection()
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
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
        public void TestCopyDataToWithADestinationConnectionAndATargetTable()
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget("CREATE TABLE \"CurrencyCopy\" (\"Code\" CHAR(3) NOT NULL, \"Name\" varchar(50) NOT NULL, CONSTRAINT \"PK_CurrencyCopy\" PRIMARY KEY (\"Code\"));");

                // Act
                var actual = source.CopyDataTo("Currency", "CurrencyCopy", target).RowCount;

                // Assert
                Assert.AreEqual(3, actual);
                Assert.AreEqual(3, Helper.TargetCount("\"CurrencyCopy\""));
            }
        }

        [TestMethod]
        public void TestCopyDataToOfAnEntity()
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
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
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
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
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget("CREATE TABLE \"CurrencyCopy\" (\"Code\" CHAR(3) NOT NULL, \"Name\" varchar(50) NOT NULL, CONSTRAINT \"PK_CurrencyCopy\" PRIMARY KEY (\"Code\"));");

                // Act
                var actual = source.CopyDataTo<Currency>("CurrencyCopy", target, c => c.Code != "JPY").RowCount;

                // Assert
                Assert.AreEqual(2, actual);
                CollectionAssert.AreEqual(new[] { "EUR", "PHP" }, Helper.TargetRows("\"CurrencyCopy\"", "\"Code\"").Select(r => (string)r["Code"]).ToArray());
            }
        }

        [TestMethod]
        public void TestCopyDataToOfAnEntityWithADestinationConnection()
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Currency");

                // Act
                var actual = source.CopyDataTo<Currency>(target, c => c.Code == "EUR").RowCount;

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
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
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
        [DataRow("Country", "\"Country\"", "\"Id\"", 3)]
        [DataRow("Currency", "\"Currency\"", "\"Code\"", 3)]
        [DataRow("Sample", "\"Sample\"", "\"Id\"", 3)]
        [DataRow("\"Order Details\"", "\"Order Details\"", "\"Id\"", 2)]
        [DataRow("Sales.Invoice", "\"Sales\".\"Invoice\"", "\"Id\"", 2)]
        [DataRow("Number", "\"Number\"", "\"Value\"", 2500)]
        [DataRow("NoKey", "\"NoKey\"", "\"Value\", \"Amount\"", 3)]
        public async Task TestCopyDataToAsyncCopiesTheRowsOfDifferentTables(string tableName,
            string quotedName,
            string orderBy,
            int expected)
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.CreateTargetTables(tableName);

                // Act
                var actual = (await source.CopyDataToAsync(tableName, target)).RowCount;

                // Assert
                Assert.AreEqual(expected, actual);
                Helper.AssertRowsEqual(Helper.SourceRows(quotedName, orderBy), Helper.TargetRows(quotedName, orderBy));
            }
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncKeepsTheIdentityValuesOfTheSourceRows()
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
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
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget("CREATE TABLE \"CurrencyCopy\" (\"Code\" CHAR(3) NOT NULL, \"Name\" varchar(50) NOT NULL, CONSTRAINT \"PK_CurrencyCopy\" PRIMARY KEY (\"Code\"));");

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
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
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
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
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
        public async Task TestCopyDataToAsyncWithADestinationConnection()
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Country");

                // Act
                var actual = (await source.CopyDataToAsync("Country", target)).RowCount;

                // Assert
                Assert.AreEqual(3, actual);
                Assert.AreEqual(3, Helper.TargetCount("\"Country\""));
            }
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncOfAnEntityWithAnExpression()
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
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
        public async Task TestCopyDataToAsyncOfAnEntityWithATargetTableAndADestinationConnection()
        {
            using (var source = new DuckDBConnection(Database.ConnectionStringForSource))
            using (var target = new DuckDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget("CREATE TABLE \"CurrencyCopy\" (\"Code\" CHAR(3) NOT NULL, \"Name\" varchar(50) NOT NULL, CONSTRAINT \"PK_CurrencyCopy\" PRIMARY KEY (\"Code\"));");

                // Act
                var actual = (await source.CopyDataToAsync<Currency>("CurrencyCopy", target, c => c.Code == "PHP")).RowCount;

                // Assert
                Assert.AreEqual(1, actual);
                Assert.AreEqual("Philippine Peso", Helper.TargetRows("\"CurrencyCopy\"", "\"Code\"").Single()["Name"]);
            }
        }

        #endregion

        #endregion
    }
}
