#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Linq;
using System.Threading.Tasks;
using ClickHouse.Driver;
using ClickHouse.Driver.ADO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Data.Enumerations;
using RepoDb.Data.ClickHouse.IntegrationTests.Entities;
using RepoDb.Data.ClickHouse.IntegrationTests.Setup;
using RepoDb.DbSettings;
using RepoDb.Enumerations;
using RepoDb.Enumerations.ClickHouse;
using RepoDb.Exceptions;
using RepoDb.Schema;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Data.ClickHouse.IntegrationTests
{
    /// <summary>
    /// Copies the rows with the three <see cref="CopyColumnMappingsBehavior"/>, which are passed on to the <see cref="ClickHouseBulkColumnMappingsBehavior"/>
    /// of the bulk insert: how the columns of the source table are aligned with the columns of the target table.
    /// </summary>
    [TestClass]
    [DoNotParallelize]
    public class CopyDataToColumnMappingsBehaviorTest
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

        #region Helpers

        private const string SameColumns = "CREATE TABLE `Currency` (`Code` String, `Name` String) ENGINE = MergeTree ORDER BY (`Code`)";
        private const string MissingColumn = "CREATE TABLE `Currency` (`Code` String) ENGINE = MergeTree ORDER BY (`Code`)";
        private const string ExtraColumn = "CREATE TABLE `Currency` (`Code` String, `Name` String, `Extra` Nullable(Int32)) ENGINE = MergeTree ORDER BY (`Code`)";
        private const string ExtraColumnWithDefault = "CREATE TABLE `Currency` (`Code` String, `Name` String, `Extra` Int32 DEFAULT 7) ENGINE = MergeTree ORDER BY (`Code`)";
        private const string ReorderedColumns = "CREATE TABLE `Currency` (`Name` String, `Code` String) ENGINE = MergeTree ORDER BY (`Code`)";

        private static ClickHouseBulkColumnMappingsBehavior CurrentBehavior(ClickHouseConnection connection) =>
            ((ClickHouseBulkDbSetting)DbSettingMapper.Get(connection)).BulkColumnMappingsBehavior;

        private static void AssertNotCopied() =>
            Assert.AreEqual(0, Helper.TargetCount("`Currency`"));

        private static void AssertCopied() =>
            CollectionAssert.AreEqual(new[] { "EUR", "JPY", "PHP" }, Helper.TargetRows("`Currency`", "`Code`").Select(r => ((string)r["Code"]).Trim()).ToArray());

        #endregion

        #region Enumeration

        [TestMethod]
        public void TestCopyTableSchemaColumnMappingsBehaviorHasTheSameNamesAndValuesAsTheClickHouseBulkColumnMappingsBehavior()
        {
            // Act
            var actual = Enum.GetValues(typeof(CopyColumnMappingsBehavior)).Cast<CopyColumnMappingsBehavior>().Select(v => (v.ToString(), (short)v)).ToArray();
            var expected = Enum.GetValues(typeof(ClickHouseBulkColumnMappingsBehavior)).Cast<ClickHouseBulkColumnMappingsBehavior>().Select(v => (v.ToString(), (short)v)).ToArray();

            // Assert
            CollectionAssert.AreEqual(expected, actual);
        }

        #endregion

        #region Automatic

        #region Sync

        [TestMethod]
        public void TestCopyDataToWithAutomaticCopiesTheTableWithTheSameColumns()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(SameColumns);

                // Act
                var actual = source.CopyDataTo("Currency", target, columnMappingsBehavior: CopyColumnMappingsBehavior.Automatic).RowCount;

                // Assert
                Assert.AreEqual(3, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("`Currency`", "`Code`"), Helper.TargetRows("`Currency`", "`Code`"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithoutABehaviorIsAutomatic()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(MissingColumn);

                // Act
                var actual = source.CopyDataTo("Currency", target).RowCount;

                // Assert
                Assert.AreEqual(3, actual);
                AssertCopied();
            }
        }

        [TestMethod]
        public void TestCopyDataToWithAutomaticCopiesTheColumnsThatBothTablesHave()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(MissingColumn);

                // Act
                var actual = source.CopyDataTo("Currency", target, columnMappingsBehavior: CopyColumnMappingsBehavior.Automatic).RowCount;

                // Assert
                Assert.AreEqual(3, actual);
                AssertCopied();
            }
        }

        [TestMethod]
        public void TestCopyDataToWithAutomaticIgnoresTheColumnsOfTheTargetTableThatTheSourceTableDoesNotHave()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(ExtraColumn);

                // Act
                var actual = source.CopyDataTo("Currency", target, columnMappingsBehavior: CopyColumnMappingsBehavior.Automatic).RowCount;

                // Assert
                Assert.AreEqual(3, actual);
                Assert.AreEqual(3, Helper.TargetCount("`Currency`", "`Extra` IS NULL"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithAutomaticCopiesTheColumnsInAnyOrder()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(ReorderedColumns);

                // Act
                var actual = source.CopyDataTo("Currency", target, columnMappingsBehavior: CopyColumnMappingsBehavior.Automatic).RowCount;

                // Assert
                Assert.AreEqual(3, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("`Currency`", "`Code`"), Helper.TargetRows("`Currency`", "`Code`"), "Code", "Name");
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestCopyDataToAsyncWithAutomaticCopiesTheColumnsThatBothTablesHave()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(MissingColumn);

                // Act
                var actual = (await source.CopyDataToAsync("Currency", target, columnMappingsBehavior: CopyColumnMappingsBehavior.Automatic)).RowCount;

                // Assert
                Assert.AreEqual(3, actual);
                AssertCopied();
            }
        }

        #endregion

        #endregion

        #region Strict

        #region Sync

        [TestMethod]
        public void TestCopyDataToWithStrictCopiesTheTableWithTheSameColumnsInTheSameOrder()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(SameColumns);

                // Act
                var actual = source.CopyDataTo("Currency", target, columnMappingsBehavior: CopyColumnMappingsBehavior.Strict).RowCount;

                // Assert
                Assert.AreEqual(3, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("`Currency`", "`Code`"), Helper.TargetRows("`Currency`", "`Code`"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToWithStrictIfTheTargetTableDoesNotHaveAColumnOfTheSourceTable()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(MissingColumn);

                // Act
                var exception = Assert.Throws<ClickHouseBulkColumnMappingsException>(() => source.CopyDataTo("Currency", target, columnMappingsBehavior: CopyColumnMappingsBehavior.Strict));

                // Assert
                Assert.AreEqual(ClickHouseBulkColumnMappingsBehavior.Strict, exception.Behavior);
                CollectionAssert.AreEqual(new[] { "Name" }, exception.SourceColumnsNotInDestination.ToArray());
                AssertNotCopied();
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToWithStrictIfTheTargetTableHasAColumnThatTheSourceTableDoesNotHave()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(ExtraColumn);

                // Act
                var exception = Assert.Throws<ClickHouseBulkColumnMappingsException>(() => source.CopyDataTo("Currency", target, columnMappingsBehavior: CopyColumnMappingsBehavior.Strict));

                // Assert
                CollectionAssert.AreEqual(new[] { "Extra" }, exception.DestinationColumnsNotInSource.ToArray());
                AssertNotCopied();
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToWithStrictIfTheColumnsOfTheTargetTableAreInAnotherOrder()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(ReorderedColumns);

                // Act
                var exception = Assert.Throws<ClickHouseBulkColumnMappingsException>(() => source.CopyDataTo("Currency", target, columnMappingsBehavior: CopyColumnMappingsBehavior.Strict));

                // Assert
                Assert.IsTrue(exception.IsOrderMismatch);
                AssertNotCopied();
            }
        }

        [TestMethod]
        public void TestCopyDataToWithStrictSetsTheBehaviorOfTheSettingBack()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(SameColumns);

                // Act
                source.CopyDataTo("Currency", target, columnMappingsBehavior: CopyColumnMappingsBehavior.Strict);

                // Assert
                Assert.AreEqual(ClickHouseBulkColumnMappingsBehavior.Automatic, CurrentBehavior(target));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithStrictSetsTheBehaviorOfTheSettingBackIfTheCopyFails()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(MissingColumn);

                // Act
                Assert.Throws<ClickHouseBulkColumnMappingsException>(() => source.CopyDataTo("Currency", target, columnMappingsBehavior: CopyColumnMappingsBehavior.Strict));

                // Assert
                Assert.AreEqual(ClickHouseBulkColumnMappingsBehavior.Automatic, CurrentBehavior(target));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithStrictAfterAStrictCopyThatFailsCopiesTheTableThatIsValid()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(MissingColumn);
                Assert.Throws<ClickHouseBulkColumnMappingsException>(() => source.CopyDataTo("Currency", target, columnMappingsBehavior: CopyColumnMappingsBehavior.Strict));
                Helper.CreateTargetTables("Country");

                // Act
                var actual = source.CopyDataTo("Country", target, columnMappingsBehavior: CopyColumnMappingsBehavior.Strict).RowCount;

                // Assert
                Assert.AreEqual(3, actual);
            }
        }

        [TestMethod]
        public void TestCopyDataToWithStrictOfATableWithAnIdentityColumnKeepsTheIdentityValues()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Country");

                // Act
                var actual = source.CopyDataTo("Country", target, where: new QueryGroup(new QueryField("Id", Operation.GreaterThan, 1)), columnMappingsBehavior: CopyColumnMappingsBehavior.Strict).RowCount;

                // Assert
                Assert.AreEqual(2, actual);
                CollectionAssert.AreEqual(new[] { 2, 3 }, Helper.TargetRows("`Country`", "`Id`").Select(r => (int)r["Id"]).ToArray());
            }
        }

        [TestMethod]
        public void TestCopyDataToWithStrictAndATargetTableOfAnotherSchema()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Currency", "repodb_data_target_sales.Currency", target, columnMappingsBehavior: CopyColumnMappingsBehavior.Strict).RowCount;

                // Assert
                Assert.AreEqual(3, actual);
                Assert.AreEqual(3, Helper.TargetCount("`Sales`.`Currency`"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithStrictAndAFilterCopiesTheRowsThatMatch()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(SameColumns);

                // Act
                var actual = source.CopyDataTo("Currency", target, where: new QueryGroup(new QueryField("Code", "EUR")), columnMappingsBehavior: CopyColumnMappingsBehavior.Strict).RowCount;

                // Assert
                Assert.AreEqual(1, actual);
                Assert.AreEqual("Euro", Helper.TargetRows("`Currency`", "`Code`").Single()["Name"]);
            }
        }

        [TestMethod]
        public void TestCopyDataToOfAnEntityWithStrict()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(SameColumns);

                // Act
                var actual = source.CopyDataTo<Currency>(target, columnMappingsBehavior: CopyColumnMappingsBehavior.Strict).RowCount;

                // Assert
                Assert.AreEqual(3, actual);
            }
        }

        [TestMethod]
        public void TestCopyDataToWithStrictAndARegisteredConnection()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(ExtraColumn);
                var name = "Target-" + Guid.NewGuid().ToString("N");
                ConnectionManager.Register(name, target);

                // Act/Assert
                Assert.Throws<ClickHouseBulkColumnMappingsException>(() => source.CopyDataTo("Currency", name, columnMappingsBehavior: CopyColumnMappingsBehavior.Strict));
                AssertNotCopied();
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestCopyDataToAsyncWithStrictCopiesTheTableWithTheSameColumnsInTheSameOrder()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(SameColumns);

                // Act
                var actual = (await source.CopyDataToAsync("Currency", target, columnMappingsBehavior: CopyColumnMappingsBehavior.Strict)).RowCount;

                // Assert
                Assert.AreEqual(3, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("`Currency`", "`Code`"), Helper.TargetRows("`Currency`", "`Code`"));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncWithStrictIfTheTargetTableDoesNotHaveAColumnOfTheSourceTable()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(MissingColumn);

                // Act/Assert
                await Assert.ThrowsAsync<ClickHouseBulkColumnMappingsException>(() => source.CopyDataToAsync("Currency", target, columnMappingsBehavior: CopyColumnMappingsBehavior.Strict));
                AssertNotCopied();
                Assert.AreEqual(ClickHouseBulkColumnMappingsBehavior.Automatic, CurrentBehavior(target));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncWithStrictIfTheColumnsOfTheTargetTableAreInAnotherOrder()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(ReorderedColumns);

                // Act/Assert
                await Assert.ThrowsAsync<ClickHouseBulkColumnMappingsException>(() => source.CopyDataToAsync("Currency", target, columnMappingsBehavior: CopyColumnMappingsBehavior.Strict));
                AssertNotCopied();
            }
        }

        #endregion

        #endregion

        #region StrictBypass

        #region Sync

        [TestMethod]
        public void TestCopyDataToWithStrictBypassCopiesTheTableWithTheSameColumns()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(SameColumns);

                // Act
                var actual = source.CopyDataTo("Currency", target, columnMappingsBehavior: CopyColumnMappingsBehavior.StrictBypass).RowCount;

                // Assert
                Assert.AreEqual(3, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("`Currency`", "`Code`"), Helper.TargetRows("`Currency`", "`Code`"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithStrictBypassCopiesTheColumnsInAnyOrder()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(ReorderedColumns);

                // Act
                var actual = source.CopyDataTo("Currency", target, columnMappingsBehavior: CopyColumnMappingsBehavior.StrictBypass).RowCount;

                // Assert
                Assert.AreEqual(3, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("`Currency`", "`Code`"), Helper.TargetRows("`Currency`", "`Code`"), "Code", "Name");
            }
        }

        [TestMethod]
        public void TestCopyDataToWithStrictBypassBypassesTheColumnsOfTheTargetTableThatTheSourceTableDoesNotHave()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(ExtraColumn);

                // Act
                var actual = source.CopyDataTo("Currency", target, columnMappingsBehavior: CopyColumnMappingsBehavior.StrictBypass).RowCount;

                // Assert
                Assert.AreEqual(3, actual);
                Assert.AreEqual(3, Helper.TargetCount("`Currency`", "`Extra` IS NULL"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithStrictBypassLeavesTheBypassedColumnsToTheirDefaultValues()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(ExtraColumnWithDefault);

                // Act
                var actual = source.CopyDataTo("Currency", target, columnMappingsBehavior: CopyColumnMappingsBehavior.StrictBypass).RowCount;

                // Assert
                Assert.AreEqual(3, actual);
                Assert.AreEqual(3, Helper.TargetCount("`Currency`", "`Extra` = 7"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToWithStrictBypassIfTheTargetTableDoesNotHaveAColumnOfTheSourceTable()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(MissingColumn);

                // Act
                var exception = Assert.Throws<ClickHouseBulkColumnMappingsException>(() => source.CopyDataTo("Currency", target, columnMappingsBehavior: CopyColumnMappingsBehavior.StrictBypass));

                // Assert
                Assert.AreEqual(ClickHouseBulkColumnMappingsBehavior.StrictBypass, exception.Behavior);
                CollectionAssert.AreEqual(new[] { "Name" }, exception.SourceColumnsNotInDestination.ToArray());
                AssertNotCopied();
            }
        }

        [TestMethod]
        public void TestCopyDataToWithStrictBypassSetsTheBehaviorOfTheSettingBack()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(MissingColumn);

                // Act
                Assert.Throws<ClickHouseBulkColumnMappingsException>(() => source.CopyDataTo("Currency", target, columnMappingsBehavior: CopyColumnMappingsBehavior.StrictBypass));

                // Assert
                Assert.AreEqual(ClickHouseBulkColumnMappingsBehavior.Automatic, CurrentBehavior(target));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestCopyDataToAsyncWithStrictBypassBypassesTheColumnsOfTheTargetTableThatTheSourceTableDoesNotHave()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(ExtraColumnWithDefault);

                // Act
                var actual = (await source.CopyDataToAsync("Currency", target, columnMappingsBehavior: CopyColumnMappingsBehavior.StrictBypass)).RowCount;

                // Assert
                Assert.AreEqual(3, actual);
                Assert.AreEqual(3, Helper.TargetCount("`Currency`", "`Extra` = 7"));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncWithStrictBypassIfTheTargetTableDoesNotHaveAColumnOfTheSourceTable()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(MissingColumn);

                // Act/Assert
                await Assert.ThrowsAsync<ClickHouseBulkColumnMappingsException>(() => source.CopyDataToAsync("Currency", target, columnMappingsBehavior: CopyColumnMappingsBehavior.StrictBypass));
                AssertNotCopied();
            }
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncWithStrictBypassCopiesTheColumnsInAnyOrder()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(ReorderedColumns);

                // Act
                var actual = (await source.CopyDataToAsync("Currency", target, columnMappingsBehavior: CopyColumnMappingsBehavior.StrictBypass)).RowCount;

                // Assert
                Assert.AreEqual(3, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("`Currency`", "`Code`"), Helper.TargetRows("`Currency`", "`Code`"), "Code", "Name");
            }
        }

        #endregion

        #endregion

        #region Arguments

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheColumnMappingsBehaviorIsNotDefined()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(SameColumns);

                // Act/Assert
                Assert.Throws<ArgumentOutOfRangeException>(() => source.CopyDataTo("Currency", target, columnMappingsBehavior: (CopyColumnMappingsBehavior)99));
                AssertNotCopied();
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncIfTheColumnMappingsBehaviorIsNotDefined()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Act/Assert
                await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => source.CopyDataToAsync("Currency", target, columnMappingsBehavior: (CopyColumnMappingsBehavior)99));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheBehaviorIsNotAutomaticAndTheSettingOfTheDestinationConnectionIsNotTheOneOfTheBulkOperations()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                GlobalConfiguration.Setup().UseClickHouse(new ClickHouseDbSetting());
                Helper.ExecuteOnTarget(SameColumns);

                // Act
                var exception = Assert.Throws<InvalidOperationException>(() => source.CopyDataTo("Currency", target, columnMappingsBehavior: CopyColumnMappingsBehavior.Strict));

                // Assert
                StringAssert.Contains(exception.Message, "BulkColumnMappingsBehavior");
                AssertNotCopied();
            }
        }

        [TestMethod]
        public void TestCopyDataToWithAutomaticAndTheSettingOfTheDestinationConnectionThatIsNotTheOneOfTheBulkOperations()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                GlobalConfiguration.Setup().UseClickHouse(new ClickHouseDbSetting());
                Helper.ExecuteOnTarget(SameColumns);

                // Act
                var actual = source.CopyDataTo("Currency", target, columnMappingsBehavior: CopyColumnMappingsBehavior.Automatic).RowCount;

                // Assert
                Assert.AreEqual(3, actual);
            }
        }

        [TestMethod]
        public void TestCopyDataToTakesTheBehaviorThatTheCopyAsksForAndNotTheOneOfTheSetting()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                GlobalConfiguration.Setup().UseClickHouse(new ClickHouseBulkDbSetting { BulkColumnMappingsBehavior = ClickHouseBulkColumnMappingsBehavior.Strict });
                Helper.ExecuteOnTarget(MissingColumn);

                // Act
                var actual = source.CopyDataTo("Currency", target, columnMappingsBehavior: CopyColumnMappingsBehavior.Automatic).RowCount;

                // Assert
                Assert.AreEqual(3, actual);
                Assert.AreEqual(ClickHouseBulkColumnMappingsBehavior.Strict, CurrentBehavior(target));
            }
        }

        #endregion
    }
}
