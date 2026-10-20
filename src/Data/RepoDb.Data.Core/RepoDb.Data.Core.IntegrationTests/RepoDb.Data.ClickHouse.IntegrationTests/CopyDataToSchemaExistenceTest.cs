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
using RepoDb.Data.ClickHouse.IntegrationTests.Setup;
using RepoDb.Exceptions;
using RepoDb.Schema;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Data.ClickHouse.IntegrationTests
{
    /// <summary>
    /// Copies the rows of a table with the different <see cref="CopyTableSchemaExistsBehavior"/>: what happens to the tables of the destination
    /// database (that may or may not exist) before the rows are copied.
    /// </summary>
    [TestClass]
    public class CopyDataToSchemaExistenceTest
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

        private static string[] TargetColumns(string quotedName) =>
            Helper.Read(Database.ConnectionStringForTarget, $"SELECT name AS `name` FROM system.columns WHERE database = '{Database.TargetName}' AND table = '{quotedName.Trim('`')}' ORDER BY position")
                .Select(row => (string)row["name"])
                .ToArray();

        private static bool TargetIndexExists(string quotedName,
            string indexName) =>
            Helper.Read(Database.ConnectionStringForTarget, $"SELECT name AS `index_name` FROM system.data_skipping_indices WHERE database = '{Database.TargetName}' AND table = '{quotedName.Trim('`')}' AND name = '{indexName}'").Count == 1;

        #endregion

        #region Skip

        #region Sync

        [TestMethod]
        public void TestCopyDataToWithSkipLeavesTheTablesThatExistAsTheyAre()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.ExecuteOnTarget("CREATE TABLE `Currency` (`Code` String, `Name` String, `Extra` Nullable(Int32)) ENGINE = MergeTree ORDER BY (`Code`)");

                // Act
                var actual = source.CopyDataTo("Currency", target, relationshipBehavior: CopyDataRelationshipBehavior.Parents, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Skip);

                // Assert
                Assert.AreEqual(3, actual);
                CollectionAssert.AreEqual(new[] { "Code", "Name", "Extra" }, TargetColumns("`Currency`"));
                Assert.AreEqual(3, Helper.TargetCount("`Currency`"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithSkipDoesNotAddTheMissingColumnsOfTheTablesThatExist()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.ExecuteOnTarget("CREATE TABLE `Currency` (`Code` String) ENGINE = MergeTree ORDER BY (`Code`)");

                // Act
                var actual = source.CopyDataTo("Currency", target, relationshipBehavior: CopyDataRelationshipBehavior.Parents, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Skip);

                // Assert
                Assert.AreEqual(3, actual);
                CollectionAssert.AreEqual(new[] { "Code" }, TargetColumns("`Currency`"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToWithSkipAndWithoutASchemaCopyIfTheTableDoesNotExist()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Act/Assert
                Assert.Throws<MissingFieldsException>(() => source.CopyDataTo("Currency", target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Skip));
                Assert.AreEqual(0, Helper.TargetTables().Count);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncWithSkipAndWithoutASchemaCopyIfTheTableDoesNotExist()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Act/Assert
                await Assert.ThrowsAsync<MissingFieldsException>(() => source.CopyDataToAsync("Currency", target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Skip));
            }
        }

        #endregion

        #endregion

        #region Throw

        #region Sync

        [TestMethod]
        public void TestCopyDataToWithThrowCreatesTheTableIfItDoesNotExist()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Currency", target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Throw);

                // Assert
                Assert.AreEqual(3, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("`Currency`", "`Code`"), Helper.TargetRows("`Currency`", "`Code`"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToWithThrowIfTheTableExists()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.CreateTargetTables("Currency");

                // Act
                var exception = Assert.Throws<InvalidOperationException>(() => source.CopyDataTo("Currency", target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Throw));

                // Assert
                StringAssert.Contains(exception.Message, "Currency");
                StringAssert.Contains(exception.Message, "already exists");
                Assert.AreEqual(0, Helper.TargetCount("`Currency`"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToWithThrowTheSecondTimeThatTheTableIsCopied()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                source.CopyDataTo("Currency", target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Throw);

                // Act/Assert
                Assert.Throws<InvalidOperationException>(() => source.CopyDataTo("Currency", target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Throw));
                Assert.AreEqual(3, Helper.TargetCount("`Currency`"));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestCopyDataToAsyncWithThrowCreatesTheTableIfItDoesNotExist()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = await source.CopyDataToAsync("Currency", target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Throw);

                // Assert
                Assert.AreEqual(3, actual);
                Assert.AreEqual(3, Helper.TargetCount("`Currency`"));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncWithThrowIfTheTableExists()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.CreateTargetTables("Currency");

                // Act/Assert
                await Assert.ThrowsAsync<InvalidOperationException>(() => source.CopyDataToAsync("Currency", target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Throw));
                Assert.AreEqual(0, Helper.TargetCount("`Currency`"));
            }
        }

        #endregion

        #endregion

        #region Drop

        #region Sync

        [TestMethod]
        public void TestCopyDataToWithDropCreatesTheTableIfItDoesNotExist()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Currency", target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Drop);

                // Assert
                Assert.AreEqual(3, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("`Currency`", "`Code`"), Helper.TargetRows("`Currency`", "`Code`"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithDropReplacesTheTableThatExistsAndItsRows()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.CreateTargetTables("Currency");
                Helper.ExecuteOnTarget("INSERT INTO `Currency` (`Code`, `Name`) VALUES ('USD', 'US Dollar');");

                // Act
                var actual = source.CopyDataTo("Currency", target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Drop);

                // Assert
                Assert.AreEqual(3, actual);
                Assert.AreEqual(3, Helper.TargetCount("`Currency`"));
                Assert.AreEqual(0, Helper.TargetCount("`Currency`", "`Code` = 'USD'"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithDropReplacesTheTableThatExistsWithTheStructureOfTheSourceTable()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.ExecuteOnTarget("CREATE TABLE `Currency` (`Code` Int32, `Foo` Nullable(String)) ENGINE = MergeTree ORDER BY tuple()");

                // Act
                var actual = source.CopyDataTo("Currency", target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Drop);

                // Assert
                Assert.AreEqual(3, actual);
                CollectionAssert.AreEqual(new[] { "Code", "Name" }, TargetColumns("`Currency`"));
                Helper.AssertRowsEqual(Helper.SourceRows("`Currency`", "`Code`"), Helper.TargetRows("`Currency`", "`Code`"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithDropCanBeRepeated()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                source.CopyDataTo("Currency", target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Drop);

                // Act
                var actual = source.CopyDataTo("Currency", target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Drop);

                // Assert
                Assert.AreEqual(3, actual);
                Assert.AreEqual(3, Helper.TargetCount("`Currency`"));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestCopyDataToAsyncWithDropReplacesTheTableThatExistsAndItsRows()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.CreateTargetTables("Currency");
                Helper.ExecuteOnTarget("INSERT INTO `Currency` (`Code`, `Name`) VALUES ('USD', 'US Dollar');");

                // Act
                var actual = await source.CopyDataToAsync("Currency", target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Drop);

                // Assert
                Assert.AreEqual(3, actual);
                Assert.AreEqual(0, Helper.TargetCount("`Currency`", "`Code` = 'USD'"));
            }
        }

        #endregion

        #endregion

        #region Align

        #region Sync

        [TestMethod]
        public void TestCopyDataToWithAlignCreatesTheTableIfItDoesNotExist()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Currency", target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Align);

                // Assert
                Assert.AreEqual(3, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("`Currency`", "`Code`"), Helper.TargetRows("`Currency`", "`Code`"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithAlignAddsTheMissingColumnsOfTheTableThatExists()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.ExecuteOnTarget("CREATE TABLE `Currency` (`Code` String) ENGINE = MergeTree ORDER BY (`Code`)");

                // Act
                var actual = source.CopyDataTo("Currency", target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Align);

                // Assert
                Assert.AreEqual(3, actual);
                CollectionAssert.AreEqual(new[] { "Code", "Name" }, TargetColumns("`Currency`"));
                Helper.AssertRowsEqual(Helper.SourceRows("`Currency`", "`Code`"), Helper.TargetRows("`Currency`", "`Code`"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithAlignAddsTheMissingIndexesOfTheTableThatExists()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.CreateTargetTables("Person");
                Helper.ExecuteOnTarget("ALTER TABLE `Person` DROP INDEX `IX_Person_Name`;");
                Assert.IsFalse(TargetIndexExists("`Person`", "IX_Person_Name"));

                // Act
                var actual = source.CopyDataTo("Person", target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Align);

                // Assert
                Assert.AreEqual(6, actual);
                Assert.IsTrue(TargetIndexExists("`Person`", "IX_Person_Name"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithAlignKeepsTheColumnsOfTheTableThatExistsAndAreNotInTheSourceTable()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.ExecuteOnTarget("CREATE TABLE `Currency` (`Code` String, `Name` String, `Extra` Nullable(Int32)) ENGINE = MergeTree ORDER BY (`Code`)");

                // Act
                var actual = source.CopyDataTo("Currency", target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Align);

                // Assert
                Assert.AreEqual(3, actual);
                CollectionAssert.AreEqual(new[] { "Code", "Name", "Extra" }, TargetColumns("`Currency`"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithAlignAndATableThatIsAlreadyAlignedCopiesTheRows()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.CreateTargetTables("Currency");

                // Act
                var actual = source.CopyDataTo("Currency", target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Align);

                // Assert
                Assert.AreEqual(3, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("`Currency`", "`Code`"), Helper.TargetRows("`Currency`", "`Code`"));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestCopyDataToAsyncWithAlignAddsTheMissingColumnsOfTheTableThatExists()
        {
            using (var source = new ClickHouseConnection(Database.ConnectionStringForSource))
            using (var target = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.ExecuteOnTarget("CREATE TABLE `Currency` (`Code` String) ENGINE = MergeTree ORDER BY (`Code`)");

                // Act
                var actual = await source.CopyDataToAsync("Currency", target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Align);

                // Assert
                Assert.AreEqual(3, actual);
                CollectionAssert.AreEqual(new[] { "Code", "Name" }, TargetColumns("`Currency`"));
            }
        }

        #endregion

        #endregion
    }
}
