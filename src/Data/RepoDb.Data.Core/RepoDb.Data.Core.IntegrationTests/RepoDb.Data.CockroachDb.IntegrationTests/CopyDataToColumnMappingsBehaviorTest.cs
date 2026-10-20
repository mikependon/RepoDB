#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Linq;
using System.Threading.Tasks;
using RepoDb.Connector.CockroachDb;
using Npgsql;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Data.Enumerations;
using RepoDb.Data.CockroachDb.IntegrationTests.Entities;
using RepoDb.Data.CockroachDb.IntegrationTests.Setup;
using RepoDb.DbSettings;
using RepoDb.Enumerations;
using RepoDb.Enumerations.CockroachDb;
using RepoDb.Exceptions;
using RepoDb.Schema;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Data.CockroachDb.IntegrationTests
{
    /// <summary>
    /// Copies the rows with the three <see cref="CopyTableSchemaColumnMappingsBehavior"/>, which are passed on to the <see cref="CockroachDbBulkColumnMappingsBehavior"/>
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

        private static CockroachDbBulkColumnMappingsBehavior CurrentBehavior(CockroachDbConnection connection) =>
            ((CockroachDbBulkOperationsDbSetting)DbSettingMapper.Get(connection)).BulkColumnMappingsBehavior;

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
            var actual = Enum.GetValues(typeof(CopyTableSchemaColumnMappingsBehavior)).Cast<CopyTableSchemaColumnMappingsBehavior>().Select(v => (v.ToString(), (short)v)).ToArray();
            var expected = Enum.GetValues(typeof(CockroachDbBulkColumnMappingsBehavior)).Cast<CockroachDbBulkColumnMappingsBehavior>().Select(v => (v.ToString(), (short)v)).ToArray();

            // Assert
            CollectionAssert.AreEqual(expected, actual);
        }

        #endregion

        #region Automatic

        #region Sync

        [TestMethod]
        public void TestCopyDataToWithAutomaticCopiesTheTableWithTheSameColumns()
        {
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(SameColumns);

                // Act
                var actual = source.CopyDataTo("Currency", target, columnMappingsBehavior: CopyTableSchemaColumnMappingsBehavior.Automatic);

                // Assert
                Assert.AreEqual(3, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("\"public\".\"Currency\"", "\"Code\""), Helper.TargetRows("\"public\".\"Currency\"", "\"Code\""));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithoutABehaviorIsAutomatic()
        {
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(MissingColumn);

                // Act
                var actual = source.CopyDataTo("Currency", target);

                // Assert
                Assert.AreEqual(3, actual);
                AssertCopied();
            }
        }

        [TestMethod]
        public void TestCopyDataToWithAutomaticCopiesTheColumnsThatBothTablesHave()
        {
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(MissingColumn);

                // Act
                var actual = source.CopyDataTo("Currency", target, columnMappingsBehavior: CopyTableSchemaColumnMappingsBehavior.Automatic);

                // Assert
                Assert.AreEqual(3, actual);
                AssertCopied();
            }
        }

        [TestMethod]
        public void TestCopyDataToWithAutomaticIgnoresTheColumnsOfTheTargetTableThatTheSourceTableDoesNotHave()
        {
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(ExtraColumn);

                // Act
                var actual = source.CopyDataTo("Currency", target, columnMappingsBehavior: CopyTableSchemaColumnMappingsBehavior.Automatic);

                // Assert
                Assert.AreEqual(3, actual);
                Assert.AreEqual(3, Helper.TargetCount("\"public\".\"Currency\"", "\"Extra\" IS NULL"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithAutomaticCopiesTheColumnsInAnyOrder()
        {
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(ReorderedColumns);

                // Act
                var actual = source.CopyDataTo("Currency", target, columnMappingsBehavior: CopyTableSchemaColumnMappingsBehavior.Automatic);

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
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(MissingColumn);

                // Act
                var actual = await source.CopyDataToAsync("Currency", target, columnMappingsBehavior: CopyTableSchemaColumnMappingsBehavior.Automatic);

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
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(SameColumns);

                // Act
                var actual = source.CopyDataTo("Currency", target, columnMappingsBehavior: CopyTableSchemaColumnMappingsBehavior.Strict);

                // Assert
                Assert.AreEqual(3, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("\"public\".\"Currency\"", "\"Code\""), Helper.TargetRows("\"public\".\"Currency\"", "\"Code\""));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToWithStrictIfTheTargetTableDoesNotHaveAColumnOfTheSourceTable()
        {
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(MissingColumn);

                // Act
                var exception = Assert.Throws<CockroachDbBulkColumnMappingsException>(() => source.CopyDataTo("Currency", target, columnMappingsBehavior: CopyTableSchemaColumnMappingsBehavior.Strict));

                // Assert
                Assert.AreEqual(CockroachDbBulkColumnMappingsBehavior.Strict, exception.Behavior);
                CollectionAssert.AreEqual(new[] { "Name" }, exception.SourceColumnsNotInDestination.ToArray());
                AssertNotCopied();
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToWithStrictIfTheTargetTableHasAColumnThatTheSourceTableDoesNotHave()
        {
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(ExtraColumn);

                // Act
                var exception = Assert.Throws<CockroachDbBulkColumnMappingsException>(() => source.CopyDataTo("Currency", target, columnMappingsBehavior: CopyTableSchemaColumnMappingsBehavior.Strict));

                // Assert
                CollectionAssert.AreEqual(new[] { "Extra" }, exception.DestinationColumnsNotInSource.ToArray());
                AssertNotCopied();
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToWithStrictIfTheColumnsOfTheTargetTableAreInAnotherOrder()
        {
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(ReorderedColumns);

                // Act
                var exception = Assert.Throws<CockroachDbBulkColumnMappingsException>(() => source.CopyDataTo("Currency", target, columnMappingsBehavior: CopyTableSchemaColumnMappingsBehavior.Strict));

                // Assert
                Assert.IsTrue(exception.IsOrderMismatch);
                AssertNotCopied();
            }
        }

        [TestMethod]
        public void TestCopyDataToWithStrictSetsTheBehaviorOfTheSettingBack()
        {
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(SameColumns);

                // Act
                source.CopyDataTo("Currency", target, columnMappingsBehavior: CopyTableSchemaColumnMappingsBehavior.Strict);

                // Assert
                Assert.AreEqual(CockroachDbBulkColumnMappingsBehavior.Automatic, CurrentBehavior(target));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithStrictSetsTheBehaviorOfTheSettingBackIfTheCopyFails()
        {
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(MissingColumn);

                // Act
                Assert.Throws<CockroachDbBulkColumnMappingsException>(() => source.CopyDataTo("Currency", target, columnMappingsBehavior: CopyTableSchemaColumnMappingsBehavior.Strict));

                // Assert
                Assert.AreEqual(CockroachDbBulkColumnMappingsBehavior.Automatic, CurrentBehavior(target));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithStrictAfterAStrictCopyThatFailsCopiesTheTableThatIsValid()
        {
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(MissingColumn);
                Assert.Throws<CockroachDbBulkColumnMappingsException>(() => source.CopyDataTo("Currency", target, columnMappingsBehavior: CopyTableSchemaColumnMappingsBehavior.Strict));
                Helper.CreateTargetTables("Country");

                // Act
                var actual = source.CopyDataTo("Country", target, columnMappingsBehavior: CopyTableSchemaColumnMappingsBehavior.Strict);

                // Assert
                Assert.AreEqual(3, actual);
            }
        }

        [TestMethod]
        public void TestCopyDataToWithStrictOfATableWithAnIdentityColumnKeepsTheIdentityValues()
        {
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Country");

                // Act
                var actual = source.CopyDataTo("Country", target, where: new QueryGroup(new QueryField("Id", Operation.GreaterThan, 1)), columnMappingsBehavior: CopyTableSchemaColumnMappingsBehavior.Strict);

                // Assert
                Assert.AreEqual(2, actual);
                CollectionAssert.AreEqual(new[] { 2, 3 }, Helper.TargetRows("\"public\".\"Country\"", "\"Id\"").Select(r => Convert.ToInt32(r["Id"])).ToArray());
            }
        }

        [TestMethod]
        public void TestCopyDataToWithStrictAndParentsCopiesTheTablesThatAreCreatedWithTheSameColumns()
        {
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Person", target, relationshipBehavior: CopyDataRelationshipBehavior.Parents, columnMappingsBehavior: CopyTableSchemaColumnMappingsBehavior.Strict);

                // Assert
                Assert.AreEqual(3 + 6, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("\"public\".\"Person\"", "\"Id\""), Helper.TargetRows("\"public\".\"Person\"", "\"Id\""));
                Assert.AreEqual(0, Helper.TargetOrphans("\"public\".\"Person\"", "\"CountryId\"", "\"public\".\"Country\"", "\"Id\""));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithStrictAndParentsAndChildrenCopiesTheWholeTree()
        {
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Pet", target, relationshipBehavior: CopyDataRelationshipBehavior.ParentsAndChildren, columnMappingsBehavior: CopyTableSchemaColumnMappingsBehavior.Strict);

                // Assert
                Assert.AreEqual(3 + 6 + 5 + 5, actual);
                Assert.AreEqual(4, Helper.TargetTables().Count);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToWithStrictAndParentsIfATableThatExistsHasAnotherColumn()
        {
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.ExecuteOnTarget("CREATE TABLE \"public\".\"Country\" (\"Id\" INT NOT NULL, \"Name\" varchar(100) NOT NULL, \"Extra\" INT NULL, CONSTRAINT \"PK_Country\" PRIMARY KEY (\"Id\"));");

                // Act/Assert
                Assert.Throws<CockroachDbBulkColumnMappingsException>(() => source.CopyDataTo("Person", target, relationshipBehavior: CopyDataRelationshipBehavior.Parents, columnMappingsBehavior: CopyTableSchemaColumnMappingsBehavior.Strict));
                Assert.AreEqual(0, Helper.TargetCount("\"public\".\"Country\""));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToWithStrictAndATargetTableOfAnotherSchemaBecauseTheBulkCopyOfCockroachDbCanNotLoadTheTablesOfAnotherSchema()
        {
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act/Assert
                Assert.Throws<PostgresException>(() => source.CopyDataTo("Currency", "Sales.Currency", target, columnMappingsBehavior: CopyTableSchemaColumnMappingsBehavior.Strict));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithStrictAndAFilterCopiesTheRowsThatMatch()
        {
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(SameColumns);

                // Act
                var actual = source.CopyDataTo("Currency", target, where: new QueryGroup(new QueryField("Code", "EUR")), columnMappingsBehavior: CopyTableSchemaColumnMappingsBehavior.Strict);

                // Assert
                Assert.AreEqual(1, actual);
                Assert.AreEqual("Euro", Helper.TargetRows("\"public\".\"Currency\"", "\"Code\"").Single()["Name"]);
            }
        }

        [TestMethod]
        public void TestCopyDataToOfAnEntityWithStrict()
        {
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(SameColumns);

                // Act
                var actual = source.CopyDataTo<Currency>(target, columnMappingsBehavior: CopyTableSchemaColumnMappingsBehavior.Strict);

                // Assert
                Assert.AreEqual(3, actual);
            }
        }

        [TestMethod]
        public void TestCopyDataToWithStrictAndARegisteredConnection()
        {
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(ExtraColumn);
                var name = "Target-" + Guid.NewGuid().ToString("N");
                ConnectionManager.Register(name, target);

                // Act/Assert
                Assert.Throws<CockroachDbBulkColumnMappingsException>(() => source.CopyDataTo("Currency", name, columnMappingsBehavior: CopyTableSchemaColumnMappingsBehavior.Strict));
                AssertNotCopied();
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestCopyDataToAsyncWithStrictCopiesTheTableWithTheSameColumnsInTheSameOrder()
        {
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(SameColumns);

                // Act
                var actual = await source.CopyDataToAsync("Currency", target, columnMappingsBehavior: CopyTableSchemaColumnMappingsBehavior.Strict);

                // Assert
                Assert.AreEqual(3, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("\"public\".\"Currency\"", "\"Code\""), Helper.TargetRows("\"public\".\"Currency\"", "\"Code\""));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncWithStrictIfTheTargetTableDoesNotHaveAColumnOfTheSourceTable()
        {
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(MissingColumn);

                // Act/Assert
                await Assert.ThrowsAsync<CockroachDbBulkColumnMappingsException>(() => source.CopyDataToAsync("Currency", target, columnMappingsBehavior: CopyTableSchemaColumnMappingsBehavior.Strict));
                AssertNotCopied();
                Assert.AreEqual(CockroachDbBulkColumnMappingsBehavior.Automatic, CurrentBehavior(target));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncWithStrictIfTheColumnsOfTheTargetTableAreInAnotherOrder()
        {
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(ReorderedColumns);

                // Act/Assert
                await Assert.ThrowsAsync<CockroachDbBulkColumnMappingsException>(() => source.CopyDataToAsync("Currency", target, columnMappingsBehavior: CopyTableSchemaColumnMappingsBehavior.Strict));
                AssertNotCopied();
            }
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncWithStrictAndParentsAndChildrenCopiesTheWholeTree()
        {
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = await source.CopyDataToAsync("Country", target, relationshipBehavior: CopyDataRelationshipBehavior.ParentsAndChildren, columnMappingsBehavior: CopyTableSchemaColumnMappingsBehavior.Strict);

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
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(SameColumns);

                // Act
                var actual = source.CopyDataTo("Currency", target, columnMappingsBehavior: CopyTableSchemaColumnMappingsBehavior.StrictBypass);

                // Assert
                Assert.AreEqual(3, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("\"public\".\"Currency\"", "\"Code\""), Helper.TargetRows("\"public\".\"Currency\"", "\"Code\""));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithStrictBypassCopiesTheColumnsInAnyOrder()
        {
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(ReorderedColumns);

                // Act
                var actual = source.CopyDataTo("Currency", target, columnMappingsBehavior: CopyTableSchemaColumnMappingsBehavior.StrictBypass);

                // Assert
                Assert.AreEqual(3, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("\"public\".\"Currency\"", "\"Code\""), Helper.TargetRows("\"public\".\"Currency\"", "\"Code\""), "Code", "Name");
            }
        }

        [TestMethod]
        public void TestCopyDataToWithStrictBypassBypassesTheColumnsOfTheTargetTableThatTheSourceTableDoesNotHave()
        {
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(ExtraColumn);

                // Act
                var actual = source.CopyDataTo("Currency", target, columnMappingsBehavior: CopyTableSchemaColumnMappingsBehavior.StrictBypass);

                // Assert
                Assert.AreEqual(3, actual);
                Assert.AreEqual(3, Helper.TargetCount("\"public\".\"Currency\"", "\"Extra\" IS NULL"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithStrictBypassLeavesTheBypassedColumnsToTheirDefaultValues()
        {
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(ExtraColumnWithDefault);

                // Act
                var actual = source.CopyDataTo("Currency", target, columnMappingsBehavior: CopyTableSchemaColumnMappingsBehavior.StrictBypass);

                // Assert
                Assert.AreEqual(3, actual);
                Assert.AreEqual(3, Helper.TargetCount("\"public\".\"Currency\"", "\"Extra\" = 7"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToWithStrictBypassIfTheTargetTableDoesNotHaveAColumnOfTheSourceTable()
        {
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(MissingColumn);

                // Act
                var exception = Assert.Throws<CockroachDbBulkColumnMappingsException>(() => source.CopyDataTo("Currency", target, columnMappingsBehavior: CopyTableSchemaColumnMappingsBehavior.StrictBypass));

                // Assert
                Assert.AreEqual(CockroachDbBulkColumnMappingsBehavior.StrictBypass, exception.Behavior);
                CollectionAssert.AreEqual(new[] { "Name" }, exception.SourceColumnsNotInDestination.ToArray());
                AssertNotCopied();
            }
        }

        [TestMethod]
        public void TestCopyDataToWithStrictBypassSetsTheBehaviorOfTheSettingBack()
        {
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(MissingColumn);

                // Act
                Assert.Throws<CockroachDbBulkColumnMappingsException>(() => source.CopyDataTo("Currency", target, columnMappingsBehavior: CopyTableSchemaColumnMappingsBehavior.StrictBypass));

                // Assert
                Assert.AreEqual(CockroachDbBulkColumnMappingsBehavior.Automatic, CurrentBehavior(target));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithStrictBypassAndChildrenCopiesTheTablesThatAreCreatedWithTheSameColumns()
        {
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Country", target, relationshipBehavior: CopyDataRelationshipBehavior.Children, columnMappingsBehavior: CopyTableSchemaColumnMappingsBehavior.StrictBypass);

                // Assert
                Assert.AreEqual(3 + 6 + 5 + 5, actual);
            }
        }

        [TestMethod]
        public void TestCopyDataToWithStrictBypassAndATableThatExistsWithAnExtraColumnAndParents()
        {
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.ExecuteOnTarget("CREATE TABLE \"public\".\"Country\" (\"Id\" INT NOT NULL, \"Name\" varchar(100) NOT NULL, \"Extra\" INT NULL, CONSTRAINT \"PK_Country\" PRIMARY KEY (\"Id\"));");

                // Act
                var actual = source.CopyDataTo("Person", target, relationshipBehavior: CopyDataRelationshipBehavior.Parents, columnMappingsBehavior: CopyTableSchemaColumnMappingsBehavior.StrictBypass);

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
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(ExtraColumnWithDefault);

                // Act
                var actual = await source.CopyDataToAsync("Currency", target, columnMappingsBehavior: CopyTableSchemaColumnMappingsBehavior.StrictBypass);

                // Assert
                Assert.AreEqual(3, actual);
                Assert.AreEqual(3, Helper.TargetCount("\"public\".\"Currency\"", "\"Extra\" = 7"));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncWithStrictBypassIfTheTargetTableDoesNotHaveAColumnOfTheSourceTable()
        {
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(MissingColumn);

                // Act/Assert
                await Assert.ThrowsAsync<CockroachDbBulkColumnMappingsException>(() => source.CopyDataToAsync("Currency", target, columnMappingsBehavior: CopyTableSchemaColumnMappingsBehavior.StrictBypass));
                AssertNotCopied();
            }
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncWithStrictBypassCopiesTheColumnsInAnyOrder()
        {
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(ReorderedColumns);

                // Act
                var actual = await source.CopyDataToAsync("Currency", target, columnMappingsBehavior: CopyTableSchemaColumnMappingsBehavior.StrictBypass);

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
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(SameColumns);

                // Act/Assert
                Assert.Throws<ArgumentOutOfRangeException>(() => source.CopyDataTo("Currency", target, columnMappingsBehavior: (CopyTableSchemaColumnMappingsBehavior)99));
                AssertNotCopied();
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncIfTheColumnMappingsBehaviorIsNotDefined()
        {
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Act/Assert
                await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => source.CopyDataToAsync("Currency", target, columnMappingsBehavior: (CopyTableSchemaColumnMappingsBehavior)99));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheBehaviorIsNotAutomaticAndTheSettingOfTheDestinationConnectionIsNotTheOneOfTheBulkOperations()
        {
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                GlobalConfiguration.Setup().UseCockroachDb(new CockroachDbDbSetting());
                Helper.ExecuteOnTarget(SameColumns);

                // Act
                var exception = Assert.Throws<InvalidOperationException>(() => source.CopyDataTo("Currency", target, columnMappingsBehavior: CopyTableSchemaColumnMappingsBehavior.Strict));

                // Assert
                StringAssert.Contains(exception.Message, "BulkColumnMappingsBehavior");
                AssertNotCopied();
            }
        }

        [TestMethod]
        public void TestCopyDataToWithAutomaticAndTheSettingOfTheDestinationConnectionThatIsNotTheOneOfTheBulkOperations()
        {
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                GlobalConfiguration.Setup().UseCockroachDb(new CockroachDbDbSetting());
                Helper.ExecuteOnTarget(SameColumns);

                // Act
                var actual = source.CopyDataTo("Currency", target, columnMappingsBehavior: CopyTableSchemaColumnMappingsBehavior.Automatic);

                // Assert
                Assert.AreEqual(3, actual);
            }
        }

        [TestMethod]
        public void TestCopyDataToTakesTheBehaviorThatTheCopyAsksForAndNotTheOneOfTheSetting()
        {
            using (var source = new CockroachDbConnection(Database.ConnectionStringForSource))
            using (var target = new CockroachDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                GlobalConfiguration.Setup().UseCockroachDb(new CockroachDbBulkOperationsDbSetting { BulkColumnMappingsBehavior = CockroachDbBulkColumnMappingsBehavior.Strict });
                Helper.ExecuteOnTarget(MissingColumn);

                // Act
                var actual = source.CopyDataTo("Currency", target, columnMappingsBehavior: CopyTableSchemaColumnMappingsBehavior.Automatic);

                // Assert
                Assert.AreEqual(3, actual);
                Assert.AreEqual(CockroachDbBulkColumnMappingsBehavior.Strict, CurrentBehavior(target));
            }
        }

        #endregion
    }
}
