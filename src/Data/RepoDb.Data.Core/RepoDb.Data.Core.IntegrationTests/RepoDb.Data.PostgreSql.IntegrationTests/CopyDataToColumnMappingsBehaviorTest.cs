#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.Data.Enumerations;
using RepoDb.Data.Models;
using RepoDb.Data.PostgreSql.IntegrationTests.Entities;
using RepoDb.Data.PostgreSql.IntegrationTests.Setup;
using RepoDb.DbSettings;
using RepoDb.Enumerations;
using RepoDb.Enumerations.PostgreSql;
using RepoDb.Exceptions;
using RepoDb.Schema;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Data.PostgreSql.IntegrationTests
{
    /// <summary>
    /// Copies the rows with the three <see cref="ColumnMappingBehavior"/>, which are passed on to the <see cref="PostgreSqlBulkColumnMappingsBehavior"/>
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

        private const string SameColumns = "CREATE TABLE \"public\".\"Currency\" (\"Code\" CHAR(3) NOT NULL, \"Name\" varchar(50) NOT NULL, CONSTRAINT \"PK_Currency\" PRIMARY KEY (\"Code\"));";
        private const string MissingColumn = "CREATE TABLE \"public\".\"Currency\" (\"Code\" CHAR(3) NOT NULL, CONSTRAINT \"PK_Currency\" PRIMARY KEY (\"Code\"));";
        private const string ExtraColumn = "CREATE TABLE \"public\".\"Currency\" (\"Code\" CHAR(3) NOT NULL, \"Name\" varchar(50) NOT NULL, \"Extra\" INT NULL, CONSTRAINT \"PK_Currency\" PRIMARY KEY (\"Code\"));";
        private const string ExtraColumnWithDefault = "CREATE TABLE \"public\".\"Currency\" (\"Code\" CHAR(3) NOT NULL, \"Name\" varchar(50) NOT NULL, \"Extra\" INT NOT NULL CONSTRAINT \"DF_Currency_Extra\" DEFAULT ((7)), CONSTRAINT \"PK_Currency\" PRIMARY KEY (\"Code\"));";
        private const string ReorderedColumns = "CREATE TABLE \"public\".\"Currency\" (\"Name\" varchar(50) NOT NULL, \"Code\" CHAR(3) NOT NULL, CONSTRAINT \"PK_Currency\" PRIMARY KEY (\"Code\"));";

        private static PostgreSqlBulkColumnMappingsBehavior CurrentBehavior(NpgsqlConnection connection) =>
            ((PostgreSqlBulkOperationsDbSetting)DbSettingMapper.Get(connection)).BulkColumnMappingsBehavior;

        private static void AssertNotCopied() =>
            Assert.AreEqual(0, Helper.TargetCount("\"public\".\"Currency\""));

        private static void AssertCopied() =>
            CollectionAssert.AreEqual(new[] { "EUR", "JPY", "PHP" }, Helper.TargetRows("\"public\".\"Currency\"", "\"Code\"").Select(r => ((string)r["Code"]).Trim()).ToArray());

        #endregion

        #region Enumeration

        [TestMethod]
        public void TestCopyTableSchemaColumnMappingsBehaviorHasTheSameNamesAndValuesAsThePostgreSqlBulkColumnMappingsBehavior()
        {
            // Act
            var actual = Enum.GetValues(typeof(ColumnMappingBehavior)).Cast<ColumnMappingBehavior>().Select(v => (v.ToString(), (short)v)).ToArray();
            var expected = Enum.GetValues(typeof(PostgreSqlBulkColumnMappingsBehavior)).Cast<PostgreSqlBulkColumnMappingsBehavior>().Select(v => (v.ToString(), (short)v)).ToArray();

            // Assert
            CollectionAssert.AreEqual(expected, actual);
        }

        #endregion

        #region Automatic

        #region Sync

        [TestMethod]
        public void TestCopyDataToWithAutomaticCopiesTheTableWithTheSameColumns()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(SameColumns);

                // Act
                var actual = source.CopyDataTo("Currency", target, columnMappingsBehavior: ColumnMappingBehavior.Automatic).RowCount;

                // Assert
                Assert.AreEqual(3, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("\"public\".\"Currency\"", "\"Code\""), Helper.TargetRows("\"public\".\"Currency\"", "\"Code\""));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithoutABehaviorIsAutomatic()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
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
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(MissingColumn);

                // Act
                var actual = source.CopyDataTo("Currency", target, columnMappingsBehavior: ColumnMappingBehavior.Automatic).RowCount;

                // Assert
                Assert.AreEqual(3, actual);
                AssertCopied();
            }
        }

        [TestMethod]
        public void TestCopyDataToWithAutomaticIgnoresTheColumnsOfTheTargetTableThatTheSourceTableDoesNotHave()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(ExtraColumn);

                // Act
                var actual = source.CopyDataTo("Currency", target, columnMappingsBehavior: ColumnMappingBehavior.Automatic).RowCount;

                // Assert
                Assert.AreEqual(3, actual);
                Assert.AreEqual(3, Helper.TargetCount("\"public\".\"Currency\"", "\"Extra\" IS NULL"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithAutomaticCopiesTheColumnsInAnyOrder()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(ReorderedColumns);

                // Act
                var actual = source.CopyDataTo("Currency", target, columnMappingsBehavior: ColumnMappingBehavior.Automatic).RowCount;

                // Assert
                Assert.AreEqual(3, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("\"public\".\"Currency\"", "\"Code\""), Helper.TargetRows("\"public\".\"Currency\"", "\"Code\""), "Code", "Name");
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestCopyDataToAsyncWithAutomaticCopiesTheColumnsThatBothTablesHave()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(MissingColumn);

                // Act
                var actual = (await source.CopyDataToAsync("Currency", target, columnMappingsBehavior: ColumnMappingBehavior.Automatic)).RowCount;

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
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(SameColumns);

                // Act
                var actual = source.CopyDataTo("Currency", target, columnMappingsBehavior: ColumnMappingBehavior.Strict).RowCount;

                // Assert
                Assert.AreEqual(3, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("\"public\".\"Currency\"", "\"Code\""), Helper.TargetRows("\"public\".\"Currency\"", "\"Code\""));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToWithStrictIfTheTargetTableDoesNotHaveAColumnOfTheSourceTable()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(MissingColumn);

                // Act
                var exception = Assert.Throws<PostgreSqlBulkColumnMappingsException>(() => source.CopyDataTo("Currency", target, columnMappingsBehavior: ColumnMappingBehavior.Strict));

                // Assert
                Assert.AreEqual(PostgreSqlBulkColumnMappingsBehavior.Strict, exception.Behavior);
                CollectionAssert.AreEqual(new[] { "Name" }, exception.SourceColumnsNotInDestination.ToArray());
                AssertNotCopied();
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToWithStrictIfTheTargetTableHasAColumnThatTheSourceTableDoesNotHave()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(ExtraColumn);

                // Act
                var exception = Assert.Throws<PostgreSqlBulkColumnMappingsException>(() => source.CopyDataTo("Currency", target, columnMappingsBehavior: ColumnMappingBehavior.Strict));

                // Assert
                CollectionAssert.AreEqual(new[] { "Extra" }, exception.DestinationColumnsNotInSource.ToArray());
                AssertNotCopied();
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToWithStrictIfTheColumnsOfTheTargetTableAreInAnotherOrder()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(ReorderedColumns);

                // Act
                var exception = Assert.Throws<PostgreSqlBulkColumnMappingsException>(() => source.CopyDataTo("Currency", target, columnMappingsBehavior: ColumnMappingBehavior.Strict));

                // Assert
                Assert.IsTrue(exception.IsOrderMismatch);
                AssertNotCopied();
            }
        }

        [TestMethod]
        public void TestCopyDataToWithStrictSetsTheBehaviorOfTheSettingBack()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(SameColumns);

                // Act
                source.CopyDataTo("Currency", target, columnMappingsBehavior: ColumnMappingBehavior.Strict);

                // Assert
                Assert.AreEqual(PostgreSqlBulkColumnMappingsBehavior.Automatic, CurrentBehavior(target));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithStrictSetsTheBehaviorOfTheSettingBackIfTheCopyFails()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(MissingColumn);

                // Act
                Assert.Throws<PostgreSqlBulkColumnMappingsException>(() => source.CopyDataTo("Currency", target, columnMappingsBehavior: ColumnMappingBehavior.Strict));

                // Assert
                Assert.AreEqual(PostgreSqlBulkColumnMappingsBehavior.Automatic, CurrentBehavior(target));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithStrictAfterAStrictCopyThatFailsCopiesTheTableThatIsValid()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(MissingColumn);
                Assert.Throws<PostgreSqlBulkColumnMappingsException>(() => source.CopyDataTo("Currency", target, columnMappingsBehavior: ColumnMappingBehavior.Strict));
                Helper.CreateTargetTables("Country");

                // Act
                var actual = source.CopyDataTo("Country", target, columnMappingsBehavior: ColumnMappingBehavior.Strict).RowCount;

                // Assert
                Assert.AreEqual(3, actual);
            }
        }

        [TestMethod]
        public void TestCopyDataToWithStrictOfATableWithAnIdentityColumnKeepsTheIdentityValues()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Country");

                // Act
                var actual = source.CopyDataTo("Country", target, where: new QueryGroup(new QueryField("Id", Operation.GreaterThan, 1)), columnMappingsBehavior: ColumnMappingBehavior.Strict).RowCount;

                // Assert
                Assert.AreEqual(2, actual);
                CollectionAssert.AreEqual(new[] { 2, 3 }, Helper.TargetRows("\"public\".\"Country\"", "\"Id\"").Select(r => (int)r["Id"]).ToArray());
            }
        }

        [TestMethod]
        public void TestCopyDataToWithStrictAndParentsCopiesTheTablesThatAreCreatedWithTheSameColumns()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Person", target, relationshipBehavior: RelationshipBehavior.Parents, columnMappingsBehavior: ColumnMappingBehavior.Strict).RowCount;

                // Assert
                Assert.AreEqual(3 + 6, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("\"public\".\"Person\"", "\"Id\""), Helper.TargetRows("\"public\".\"Person\"", "\"Id\""));
                Assert.AreEqual(0, Helper.TargetOrphans("\"public\".\"Person\"", "\"CountryId\"", "\"public\".\"Country\"", "\"Id\""));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithStrictAndParentsAndChildrenCopiesTheWholeTree()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Pet", target, relationshipBehavior: RelationshipBehavior.ParentsAndChildren, columnMappingsBehavior: ColumnMappingBehavior.Strict).RowCount;

                // Assert
                Assert.AreEqual(3 + 6 + 5 + 5, actual);
                Assert.AreEqual(4, Helper.TargetTables().Count);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToWithStrictAndParentsIfATableThatExistsHasAnotherColumn()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.ExecuteOnTarget("CREATE TABLE \"public\".\"Country\" (\"Id\" INT NOT NULL, \"Name\" varchar(100) NOT NULL, \"Extra\" INT NULL, CONSTRAINT \"PK_Country\" PRIMARY KEY (\"Id\"));");

                // Act/Assert
                Assert.Throws<PostgreSqlBulkColumnMappingsException>(() => source.CopyDataTo("Person", target, relationshipBehavior: RelationshipBehavior.Parents, columnMappingsBehavior: ColumnMappingBehavior.Strict));
                Assert.AreEqual(0, Helper.TargetCount("\"public\".\"Country\""));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithStrictAndATargetTableOfAnotherSchema()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Currency", "Sales.Currency", target, columnMappingsBehavior: ColumnMappingBehavior.Strict).RowCount;

                // Assert
                Assert.AreEqual(3, actual);
                Assert.AreEqual(3, Helper.TargetCount("\"Sales\".\"Currency\""));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithStrictAndAFilterCopiesTheRowsThatMatch()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(SameColumns);

                // Act
                var actual = source.CopyDataTo("Currency", target, where: new QueryGroup(new QueryField("Code", "EUR")), columnMappingsBehavior: ColumnMappingBehavior.Strict).RowCount;

                // Assert
                Assert.AreEqual(1, actual);
                Assert.AreEqual("Euro", Helper.TargetRows("\"public\".\"Currency\"", "\"Code\"").Single()["Name"]);
            }
        }

        [TestMethod]
        public void TestCopyDataToOfAnEntityWithStrict()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(SameColumns);

                // Act
                var actual = source.CopyDataTo<Currency>(target, columnMappingsBehavior: ColumnMappingBehavior.Strict).RowCount;

                // Assert
                Assert.AreEqual(3, actual);
            }
        }

        [TestMethod]
        public void TestCopyDataToWithStrictAndADestinationConnection()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(ExtraColumn);

                // Act/Assert
                Assert.Throws<PostgreSqlBulkColumnMappingsException>(() => source.CopyDataTo("Currency", target, columnMappingsBehavior: ColumnMappingBehavior.Strict));
                AssertNotCopied();
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestCopyDataToAsyncWithStrictCopiesTheTableWithTheSameColumnsInTheSameOrder()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(SameColumns);

                // Act
                var actual = (await source.CopyDataToAsync("Currency", target, columnMappingsBehavior: ColumnMappingBehavior.Strict)).RowCount;

                // Assert
                Assert.AreEqual(3, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("\"public\".\"Currency\"", "\"Code\""), Helper.TargetRows("\"public\".\"Currency\"", "\"Code\""));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncWithStrictIfTheTargetTableDoesNotHaveAColumnOfTheSourceTable()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(MissingColumn);

                // Act/Assert
                await Assert.ThrowsAsync<PostgreSqlBulkColumnMappingsException>(() => source.CopyDataToAsync("Currency", target, columnMappingsBehavior: ColumnMappingBehavior.Strict));
                AssertNotCopied();
                Assert.AreEqual(PostgreSqlBulkColumnMappingsBehavior.Automatic, CurrentBehavior(target));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncWithStrictIfTheColumnsOfTheTargetTableAreInAnotherOrder()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(ReorderedColumns);

                // Act/Assert
                await Assert.ThrowsAsync<PostgreSqlBulkColumnMappingsException>(() => source.CopyDataToAsync("Currency", target, columnMappingsBehavior: ColumnMappingBehavior.Strict));
                AssertNotCopied();
            }
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncWithStrictAndParentsAndChildrenCopiesTheWholeTree()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = (await source.CopyDataToAsync("Country", target, relationshipBehavior: RelationshipBehavior.ParentsAndChildren, columnMappingsBehavior: ColumnMappingBehavior.Strict)).RowCount;

                // Assert
                Assert.AreEqual(3 + 6 + 5 + 5, actual);
            }
        }

        #endregion

        #endregion

        #region StrictBypass

        #region Sync

        [TestMethod]
        public void TestCopyDataToWithStrictBypassCopiesTheTableWithTheSameColumns()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(SameColumns);

                // Act
                var actual = source.CopyDataTo("Currency", target, columnMappingsBehavior: ColumnMappingBehavior.StrictBypass).RowCount;

                // Assert
                Assert.AreEqual(3, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("\"public\".\"Currency\"", "\"Code\""), Helper.TargetRows("\"public\".\"Currency\"", "\"Code\""));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithStrictBypassCopiesTheColumnsInAnyOrder()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(ReorderedColumns);

                // Act
                var actual = source.CopyDataTo("Currency", target, columnMappingsBehavior: ColumnMappingBehavior.StrictBypass).RowCount;

                // Assert
                Assert.AreEqual(3, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("\"public\".\"Currency\"", "\"Code\""), Helper.TargetRows("\"public\".\"Currency\"", "\"Code\""), "Code", "Name");
            }
        }

        [TestMethod]
        public void TestCopyDataToWithStrictBypassBypassesTheColumnsOfTheTargetTableThatTheSourceTableDoesNotHave()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(ExtraColumn);

                // Act
                var actual = source.CopyDataTo("Currency", target, columnMappingsBehavior: ColumnMappingBehavior.StrictBypass).RowCount;

                // Assert
                Assert.AreEqual(3, actual);
                Assert.AreEqual(3, Helper.TargetCount("\"public\".\"Currency\"", "\"Extra\" IS NULL"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithStrictBypassLeavesTheBypassedColumnsToTheirDefaultValues()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(ExtraColumnWithDefault);

                // Act
                var actual = source.CopyDataTo("Currency", target, columnMappingsBehavior: ColumnMappingBehavior.StrictBypass).RowCount;

                // Assert
                Assert.AreEqual(3, actual);
                Assert.AreEqual(3, Helper.TargetCount("\"public\".\"Currency\"", "\"Extra\" = 7"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToWithStrictBypassIfTheTargetTableDoesNotHaveAColumnOfTheSourceTable()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(MissingColumn);

                // Act
                var exception = Assert.Throws<PostgreSqlBulkColumnMappingsException>(() => source.CopyDataTo("Currency", target, columnMappingsBehavior: ColumnMappingBehavior.StrictBypass));

                // Assert
                Assert.AreEqual(PostgreSqlBulkColumnMappingsBehavior.StrictBypass, exception.Behavior);
                CollectionAssert.AreEqual(new[] { "Name" }, exception.SourceColumnsNotInDestination.ToArray());
                AssertNotCopied();
            }
        }

        [TestMethod]
        public void TestCopyDataToWithStrictBypassSetsTheBehaviorOfTheSettingBack()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(MissingColumn);

                // Act
                Assert.Throws<PostgreSqlBulkColumnMappingsException>(() => source.CopyDataTo("Currency", target, columnMappingsBehavior: ColumnMappingBehavior.StrictBypass));

                // Assert
                Assert.AreEqual(PostgreSqlBulkColumnMappingsBehavior.Automatic, CurrentBehavior(target));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithStrictBypassAndChildrenCopiesTheTablesThatAreCreatedWithTheSameColumns()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Country", target, relationshipBehavior: RelationshipBehavior.Children, columnMappingsBehavior: ColumnMappingBehavior.StrictBypass).RowCount;

                // Assert
                Assert.AreEqual(3 + 6 + 5 + 5, actual);
            }
        }

        [TestMethod]
        public void TestCopyDataToWithStrictBypassAndATableThatExistsWithAnExtraColumnAndParents()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.ExecuteOnTarget("CREATE TABLE \"public\".\"Country\" (\"Id\" INT NOT NULL, \"Name\" varchar(100) NOT NULL, \"Extra\" INT NULL, CONSTRAINT \"PK_Country\" PRIMARY KEY (\"Id\"));");

                // Act
                var actual = source.CopyDataTo("Person", target, relationshipBehavior: RelationshipBehavior.Parents, columnMappingsBehavior: ColumnMappingBehavior.StrictBypass).RowCount;

                // Assert
                Assert.AreEqual(3 + 6, actual);
                Assert.AreEqual(3, Helper.TargetCount("\"public\".\"Country\"", "\"Extra\" IS NULL"));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestCopyDataToAsyncWithStrictBypassBypassesTheColumnsOfTheTargetTableThatTheSourceTableDoesNotHave()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(ExtraColumnWithDefault);

                // Act
                var actual = (await source.CopyDataToAsync("Currency", target, columnMappingsBehavior: ColumnMappingBehavior.StrictBypass)).RowCount;

                // Assert
                Assert.AreEqual(3, actual);
                Assert.AreEqual(3, Helper.TargetCount("\"public\".\"Currency\"", "\"Extra\" = 7"));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncWithStrictBypassIfTheTargetTableDoesNotHaveAColumnOfTheSourceTable()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(MissingColumn);

                // Act/Assert
                await Assert.ThrowsAsync<PostgreSqlBulkColumnMappingsException>(() => source.CopyDataToAsync("Currency", target, columnMappingsBehavior: ColumnMappingBehavior.StrictBypass));
                AssertNotCopied();
            }
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncWithStrictBypassCopiesTheColumnsInAnyOrder()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(ReorderedColumns);

                // Act
                var actual = (await source.CopyDataToAsync("Currency", target, columnMappingsBehavior: ColumnMappingBehavior.StrictBypass)).RowCount;

                // Assert
                Assert.AreEqual(3, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("\"public\".\"Currency\"", "\"Code\""), Helper.TargetRows("\"public\".\"Currency\"", "\"Code\""), "Code", "Name");
            }
        }

        #endregion

        #endregion

        #region Arguments

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheColumnMappingsBehaviorIsNotDefined()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(SameColumns);

                // Act/Assert
                Assert.Throws<ArgumentOutOfRangeException>(() => source.CopyDataTo("Currency", target, columnMappingsBehavior: (ColumnMappingBehavior)99));
                AssertNotCopied();
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncIfTheColumnMappingsBehaviorIsNotDefined()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Act/Assert
                await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => source.CopyDataToAsync("Currency", target, columnMappingsBehavior: (ColumnMappingBehavior)99));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheBehaviorIsNotAutomaticAndTheSettingOfTheDestinationConnectionIsNotTheOneOfTheBulkOperations()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                GlobalConfiguration.Setup().UsePostgreSql(new PostgreSqlDbSetting());
                Helper.ExecuteOnTarget(SameColumns);

                // Act
                var exception = Assert.Throws<InvalidOperationException>(() => source.CopyDataTo("Currency", target, columnMappingsBehavior: ColumnMappingBehavior.Strict));

                // Assert
                StringAssert.Contains(exception.Message, "BulkColumnMappingsBehavior");
                AssertNotCopied();
            }
        }

        [TestMethod]
        public void TestCopyDataToWithAutomaticAndTheSettingOfTheDestinationConnectionThatIsNotTheOneOfTheBulkOperations()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                GlobalConfiguration.Setup().UsePostgreSql(new PostgreSqlDbSetting());
                Helper.ExecuteOnTarget(SameColumns);

                // Act
                var actual = source.CopyDataTo("Currency", target, columnMappingsBehavior: ColumnMappingBehavior.Automatic).RowCount;

                // Assert
                Assert.AreEqual(3, actual);
            }
        }

        [TestMethod]
        public void TestCopyDataToTakesTheBehaviorThatTheCopyAsksForAndNotTheOneOfTheSetting()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                GlobalConfiguration.Setup().UsePostgreSql(new PostgreSqlBulkOperationsDbSetting { BulkColumnMappingsBehavior = PostgreSqlBulkColumnMappingsBehavior.Strict });
                Helper.ExecuteOnTarget(MissingColumn);

                // Act
                var actual = source.CopyDataTo("Currency", target, columnMappingsBehavior: ColumnMappingBehavior.Automatic).RowCount;

                // Assert
                Assert.AreEqual(3, actual);
                Assert.AreEqual(PostgreSqlBulkColumnMappingsBehavior.Strict, CurrentBehavior(target));
            }
        }

        #endregion
    }
}
