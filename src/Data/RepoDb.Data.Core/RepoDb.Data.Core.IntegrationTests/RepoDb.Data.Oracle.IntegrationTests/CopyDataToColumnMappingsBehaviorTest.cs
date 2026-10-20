#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Linq;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Data.Enumerations;
using RepoDb.Data.Oracle.IntegrationTests.Entities;
using RepoDb.Data.Oracle.IntegrationTests.Setup;
using RepoDb.DbSettings;
using RepoDb.Enumerations;
using RepoDb.Enumerations.Oracle;
using RepoDb.Exceptions;
using RepoDb.Schema;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Data.Oracle.IntegrationTests
{
    /// <summary>
    /// Copies the rows with the three <see cref="CopyTableSchemaColumnMappingsBehavior"/>, which are passed on to the <see cref="OracleBulkColumnMappingsBehavior"/>
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

        private const string SameColumns = "CREATE TABLE \"Currency\" (\"Code\" CHAR(3) NOT NULL, \"Name\" VARCHAR2(50) NOT NULL, CONSTRAINT \"PK_Currency\" PRIMARY KEY (\"Code\"));";
        private const string MissingColumn = "CREATE TABLE \"Currency\" (\"Code\" CHAR(3) NOT NULL, CONSTRAINT \"PK_Currency\" PRIMARY KEY (\"Code\"));";
        private const string ExtraColumn = "CREATE TABLE \"Currency\" (\"Code\" CHAR(3) NOT NULL, \"Name\" VARCHAR2(50) NOT NULL, \"Extra\" NUMBER(10) NULL, CONSTRAINT \"PK_Currency\" PRIMARY KEY (\"Code\"));";
        private const string ExtraColumnWithDefault = "CREATE TABLE \"Currency\" (\"Code\" CHAR(3) NOT NULL, \"Name\" VARCHAR2(50) NOT NULL, \"Extra\" NUMBER(10) DEFAULT 7 NOT NULL, CONSTRAINT \"PK_Currency\" PRIMARY KEY (\"Code\"));";
        private const string ReorderedColumns = "CREATE TABLE \"Currency\" (\"Name\" VARCHAR2(50) NOT NULL, \"Code\" CHAR(3) NOT NULL, CONSTRAINT \"PK_Currency\" PRIMARY KEY (\"Code\"));";

        private static OracleBulkColumnMappingsBehavior CurrentBehavior(OracleConnection connection) =>
            ((OracleBulkOperationsDbSetting)DbSettingMapper.Get(connection)).BulkColumnMappingsBehavior;

        private static void AssertNotCopied() =>
            Assert.AreEqual(0, Helper.TargetCount("\"Currency\""));

        private static void AssertCopied() =>
            CollectionAssert.AreEqual(new[] { "EUR", "JPY", "PHP" }, Helper.TargetRows("\"Currency\"", "\"Code\"").Select(r => ((string)r["Code"]).Trim()).ToArray());

        #endregion

        #region Enumeration

        [TestMethod]
        public void TestCopyTableSchemaColumnMappingsBehaviorHasTheSameNamesAndValuesAsTheOracleBulkColumnMappingsBehavior()
        {
            // Act
            var actual = Enum.GetValues(typeof(CopyTableSchemaColumnMappingsBehavior)).Cast<CopyTableSchemaColumnMappingsBehavior>().Select(v => (v.ToString(), (short)v)).ToArray();
            var expected = Enum.GetValues(typeof(OracleBulkColumnMappingsBehavior)).Cast<OracleBulkColumnMappingsBehavior>().Select(v => (v.ToString(), (short)v)).ToArray();

            // Assert
            CollectionAssert.AreEqual(expected, actual);
        }

        #endregion

        #region Automatic

        #region Sync

        [TestMethod]
        public void TestCopyDataToWithAutomaticCopiesTheTableWithTheSameColumns()
        {
            using (var source = new OracleConnection(Database.ConnectionStringForSource))
            using (var target = new OracleConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(SameColumns);

                // Act
                var actual = source.CopyDataTo("Currency", target, columnMappingsBehavior: CopyTableSchemaColumnMappingsBehavior.Automatic);

                // Assert
                Assert.AreEqual(3, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("\"Currency\"", "\"Code\""), Helper.TargetRows("\"Currency\"", "\"Code\""));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithoutABehaviorIsAutomatic()
        {
            using (var source = new OracleConnection(Database.ConnectionStringForSource))
            using (var target = new OracleConnection(Database.ConnectionStringForTarget))
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
            using (var source = new OracleConnection(Database.ConnectionStringForSource))
            using (var target = new OracleConnection(Database.ConnectionStringForTarget))
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
            using (var source = new OracleConnection(Database.ConnectionStringForSource))
            using (var target = new OracleConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(ExtraColumn);

                // Act
                var actual = source.CopyDataTo("Currency", target, columnMappingsBehavior: CopyTableSchemaColumnMappingsBehavior.Automatic);

                // Assert
                Assert.AreEqual(3, actual);
                Assert.AreEqual(3, Helper.TargetCount("\"Currency\"", "\"Extra\" IS NULL"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithAutomaticCopiesTheColumnsInAnyOrder()
        {
            using (var source = new OracleConnection(Database.ConnectionStringForSource))
            using (var target = new OracleConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(ReorderedColumns);

                // Act
                var actual = source.CopyDataTo("Currency", target, columnMappingsBehavior: CopyTableSchemaColumnMappingsBehavior.Automatic);

                // Assert
                Assert.AreEqual(3, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("\"Currency\"", "\"Code\""), Helper.TargetRows("\"Currency\"", "\"Code\""), "Code", "Name");
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestCopyDataToAsyncWithAutomaticCopiesTheColumnsThatBothTablesHave()
        {
            using (var source = new OracleConnection(Database.ConnectionStringForSource))
            using (var target = new OracleConnection(Database.ConnectionStringForTarget))
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
            using (var source = new OracleConnection(Database.ConnectionStringForSource))
            using (var target = new OracleConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(SameColumns);

                // Act
                var actual = source.CopyDataTo("Currency", target, columnMappingsBehavior: CopyTableSchemaColumnMappingsBehavior.Strict);

                // Assert
                Assert.AreEqual(3, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("\"Currency\"", "\"Code\""), Helper.TargetRows("\"Currency\"", "\"Code\""));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToWithStrictIfTheTargetTableDoesNotHaveAColumnOfTheSourceTable()
        {
            using (var source = new OracleConnection(Database.ConnectionStringForSource))
            using (var target = new OracleConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(MissingColumn);

                // Act
                var exception = Assert.Throws<OracleBulkColumnMappingsException>(() => source.CopyDataTo("Currency", target, columnMappingsBehavior: CopyTableSchemaColumnMappingsBehavior.Strict));

                // Assert
                Assert.AreEqual(OracleBulkColumnMappingsBehavior.Strict, exception.Behavior);
                CollectionAssert.AreEqual(new[] { "Name" }, exception.SourceColumnsNotInDestination.ToArray());
                AssertNotCopied();
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToWithStrictIfTheTargetTableHasAColumnThatTheSourceTableDoesNotHave()
        {
            using (var source = new OracleConnection(Database.ConnectionStringForSource))
            using (var target = new OracleConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(ExtraColumn);

                // Act
                var exception = Assert.Throws<OracleBulkColumnMappingsException>(() => source.CopyDataTo("Currency", target, columnMappingsBehavior: CopyTableSchemaColumnMappingsBehavior.Strict));

                // Assert
                CollectionAssert.AreEqual(new[] { "Extra" }, exception.DestinationColumnsNotInSource.ToArray());
                AssertNotCopied();
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToWithStrictIfTheColumnsOfTheTargetTableAreInAnotherOrder()
        {
            using (var source = new OracleConnection(Database.ConnectionStringForSource))
            using (var target = new OracleConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(ReorderedColumns);

                // Act
                var exception = Assert.Throws<OracleBulkColumnMappingsException>(() => source.CopyDataTo("Currency", target, columnMappingsBehavior: CopyTableSchemaColumnMappingsBehavior.Strict));

                // Assert
                Assert.IsTrue(exception.IsOrderMismatch);
                AssertNotCopied();
            }
        }

        [TestMethod]
        public void TestCopyDataToWithStrictSetsTheBehaviorOfTheSettingBack()
        {
            using (var source = new OracleConnection(Database.ConnectionStringForSource))
            using (var target = new OracleConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(SameColumns);

                // Act
                source.CopyDataTo("Currency", target, columnMappingsBehavior: CopyTableSchemaColumnMappingsBehavior.Strict);

                // Assert
                Assert.AreEqual(OracleBulkColumnMappingsBehavior.Automatic, CurrentBehavior(target));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithStrictSetsTheBehaviorOfTheSettingBackIfTheCopyFails()
        {
            using (var source = new OracleConnection(Database.ConnectionStringForSource))
            using (var target = new OracleConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(MissingColumn);

                // Act
                Assert.Throws<OracleBulkColumnMappingsException>(() => source.CopyDataTo("Currency", target, columnMappingsBehavior: CopyTableSchemaColumnMappingsBehavior.Strict));

                // Assert
                Assert.AreEqual(OracleBulkColumnMappingsBehavior.Automatic, CurrentBehavior(target));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithStrictAfterAStrictCopyThatFailsCopiesTheTableThatIsValid()
        {
            using (var source = new OracleConnection(Database.ConnectionStringForSource))
            using (var target = new OracleConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(MissingColumn);
                Assert.Throws<OracleBulkColumnMappingsException>(() => source.CopyDataTo("Currency", target, columnMappingsBehavior: CopyTableSchemaColumnMappingsBehavior.Strict));
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
            using (var source = new OracleConnection(Database.ConnectionStringForSource))
            using (var target = new OracleConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Country");

                // Act
                var actual = source.CopyDataTo("Country", target, where: new QueryGroup(new QueryField("Id", Operation.GreaterThan, 1)), columnMappingsBehavior: CopyTableSchemaColumnMappingsBehavior.Strict);

                // Assert
                Assert.AreEqual(2, actual);
                CollectionAssert.AreEqual(new[] { 2, 3 }, Helper.TargetRows("\"Country\"", "\"Id\"").Select(r => Convert.ToInt32(r["Id"])).ToArray());
            }
        }

        [TestMethod]
        public void TestCopyDataToWithStrictAndParentsCopiesTheTablesThatAreCreatedWithTheSameColumns()
        {
            using (var source = new OracleConnection(Database.ConnectionStringForSource))
            using (var target = new OracleConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Person", target, relationshipBehavior: CopyDataRelationshipBehavior.Parents, columnMappingsBehavior: CopyTableSchemaColumnMappingsBehavior.Strict);

                // Assert
                Assert.AreEqual(3 + 6, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("\"Person\"", "\"Id\""), Helper.TargetRows("\"Person\"", "\"Id\""));
                Assert.AreEqual(0, Helper.TargetOrphans("\"Person\"", "\"CountryId\"", "\"Country\"", "\"Id\""));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithStrictAndParentsAndChildrenCopiesTheWholeTree()
        {
            using (var source = new OracleConnection(Database.ConnectionStringForSource))
            using (var target = new OracleConnection(Database.ConnectionStringForTarget))
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
            using (var source = new OracleConnection(Database.ConnectionStringForSource))
            using (var target = new OracleConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.ExecuteOnTarget("CREATE TABLE \"Country\" (\"Id\" NUMBER(10) NOT NULL, \"Name\" VARCHAR2(100) NOT NULL, \"Extra\" NUMBER(10) NULL, CONSTRAINT \"PK_Country\" PRIMARY KEY (\"Id\"));");

                // Act/Assert
                Assert.Throws<OracleBulkColumnMappingsException>(() => source.CopyDataTo("Person", target, relationshipBehavior: CopyDataRelationshipBehavior.Parents, columnMappingsBehavior: CopyTableSchemaColumnMappingsBehavior.Strict));
                Assert.AreEqual(0, Helper.TargetCount("\"Country\""));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithStrictAndATargetTableOfAnotherSchema()
        {
            using (var source = new OracleConnection(Database.ConnectionStringForSource))
            using (var target = new OracleConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Currency", "REPODB_DATA_TGT_SALES.Currency", target, columnMappingsBehavior: CopyTableSchemaColumnMappingsBehavior.Strict);

                // Assert
                Assert.AreEqual(3, actual);
                Assert.AreEqual(3, Helper.TargetCount("\"Sales\".\"Currency\""));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithStrictAndAFilterCopiesTheRowsThatMatch()
        {
            using (var source = new OracleConnection(Database.ConnectionStringForSource))
            using (var target = new OracleConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(SameColumns);

                // Act
                var actual = source.CopyDataTo("Currency", target, where: new QueryGroup(new QueryField("Code", "EUR")), columnMappingsBehavior: CopyTableSchemaColumnMappingsBehavior.Strict);

                // Assert
                Assert.AreEqual(1, actual);
                Assert.AreEqual("Euro", Helper.TargetRows("\"Currency\"", "\"Code\"").Single()["Name"]);
            }
        }

        [TestMethod]
        public void TestCopyDataToOfAnEntityWithStrict()
        {
            using (var source = new OracleConnection(Database.ConnectionStringForSource))
            using (var target = new OracleConnection(Database.ConnectionStringForTarget))
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
            using (var source = new OracleConnection(Database.ConnectionStringForSource))
            using (var target = new OracleConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(ExtraColumn);
                var name = "Target-" + Guid.NewGuid().ToString("N");
                ConnectionManager.Register(name, target);

                // Act/Assert
                Assert.Throws<OracleBulkColumnMappingsException>(() => source.CopyDataTo("Currency", name, columnMappingsBehavior: CopyTableSchemaColumnMappingsBehavior.Strict));
                AssertNotCopied();
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestCopyDataToAsyncWithStrictCopiesTheTableWithTheSameColumnsInTheSameOrder()
        {
            using (var source = new OracleConnection(Database.ConnectionStringForSource))
            using (var target = new OracleConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(SameColumns);

                // Act
                var actual = await source.CopyDataToAsync("Currency", target, columnMappingsBehavior: CopyTableSchemaColumnMappingsBehavior.Strict);

                // Assert
                Assert.AreEqual(3, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("\"Currency\"", "\"Code\""), Helper.TargetRows("\"Currency\"", "\"Code\""));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncWithStrictIfTheTargetTableDoesNotHaveAColumnOfTheSourceTable()
        {
            using (var source = new OracleConnection(Database.ConnectionStringForSource))
            using (var target = new OracleConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(MissingColumn);

                // Act/Assert
                await Assert.ThrowsAsync<OracleBulkColumnMappingsException>(() => source.CopyDataToAsync("Currency", target, columnMappingsBehavior: CopyTableSchemaColumnMappingsBehavior.Strict));
                AssertNotCopied();
                Assert.AreEqual(OracleBulkColumnMappingsBehavior.Automatic, CurrentBehavior(target));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncWithStrictIfTheColumnsOfTheTargetTableAreInAnotherOrder()
        {
            using (var source = new OracleConnection(Database.ConnectionStringForSource))
            using (var target = new OracleConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(ReorderedColumns);

                // Act/Assert
                await Assert.ThrowsAsync<OracleBulkColumnMappingsException>(() => source.CopyDataToAsync("Currency", target, columnMappingsBehavior: CopyTableSchemaColumnMappingsBehavior.Strict));
                AssertNotCopied();
            }
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncWithStrictAndParentsAndChildrenCopiesTheWholeTree()
        {
            using (var source = new OracleConnection(Database.ConnectionStringForSource))
            using (var target = new OracleConnection(Database.ConnectionStringForTarget))
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
            using (var source = new OracleConnection(Database.ConnectionStringForSource))
            using (var target = new OracleConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(SameColumns);

                // Act
                var actual = source.CopyDataTo("Currency", target, columnMappingsBehavior: CopyTableSchemaColumnMappingsBehavior.StrictBypass);

                // Assert
                Assert.AreEqual(3, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("\"Currency\"", "\"Code\""), Helper.TargetRows("\"Currency\"", "\"Code\""));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithStrictBypassCopiesTheColumnsInAnyOrder()
        {
            using (var source = new OracleConnection(Database.ConnectionStringForSource))
            using (var target = new OracleConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(ReorderedColumns);

                // Act
                var actual = source.CopyDataTo("Currency", target, columnMappingsBehavior: CopyTableSchemaColumnMappingsBehavior.StrictBypass);

                // Assert
                Assert.AreEqual(3, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("\"Currency\"", "\"Code\""), Helper.TargetRows("\"Currency\"", "\"Code\""), "Code", "Name");
            }
        }

        [TestMethod]
        public void TestCopyDataToWithStrictBypassBypassesTheColumnsOfTheTargetTableThatTheSourceTableDoesNotHave()
        {
            using (var source = new OracleConnection(Database.ConnectionStringForSource))
            using (var target = new OracleConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(ExtraColumn);

                // Act
                var actual = source.CopyDataTo("Currency", target, columnMappingsBehavior: CopyTableSchemaColumnMappingsBehavior.StrictBypass);

                // Assert
                Assert.AreEqual(3, actual);
                Assert.AreEqual(3, Helper.TargetCount("\"Currency\"", "\"Extra\" IS NULL"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithStrictBypassLeavesTheBypassedColumnsToTheirDefaultValues()
        {
            using (var source = new OracleConnection(Database.ConnectionStringForSource))
            using (var target = new OracleConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(ExtraColumnWithDefault);

                // Act
                var actual = source.CopyDataTo("Currency", target, columnMappingsBehavior: CopyTableSchemaColumnMappingsBehavior.StrictBypass);

                // Assert
                Assert.AreEqual(3, actual);
                Assert.AreEqual(3, Helper.TargetCount("\"Currency\"", "\"Extra\" = 7"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToWithStrictBypassIfTheTargetTableDoesNotHaveAColumnOfTheSourceTable()
        {
            using (var source = new OracleConnection(Database.ConnectionStringForSource))
            using (var target = new OracleConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(MissingColumn);

                // Act
                var exception = Assert.Throws<OracleBulkColumnMappingsException>(() => source.CopyDataTo("Currency", target, columnMappingsBehavior: CopyTableSchemaColumnMappingsBehavior.StrictBypass));

                // Assert
                Assert.AreEqual(OracleBulkColumnMappingsBehavior.StrictBypass, exception.Behavior);
                CollectionAssert.AreEqual(new[] { "Name" }, exception.SourceColumnsNotInDestination.ToArray());
                AssertNotCopied();
            }
        }

        [TestMethod]
        public void TestCopyDataToWithStrictBypassSetsTheBehaviorOfTheSettingBack()
        {
            using (var source = new OracleConnection(Database.ConnectionStringForSource))
            using (var target = new OracleConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(MissingColumn);

                // Act
                Assert.Throws<OracleBulkColumnMappingsException>(() => source.CopyDataTo("Currency", target, columnMappingsBehavior: CopyTableSchemaColumnMappingsBehavior.StrictBypass));

                // Assert
                Assert.AreEqual(OracleBulkColumnMappingsBehavior.Automatic, CurrentBehavior(target));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithStrictBypassAndChildrenCopiesTheTablesThatAreCreatedWithTheSameColumns()
        {
            using (var source = new OracleConnection(Database.ConnectionStringForSource))
            using (var target = new OracleConnection(Database.ConnectionStringForTarget))
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
            using (var source = new OracleConnection(Database.ConnectionStringForSource))
            using (var target = new OracleConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.ExecuteOnTarget("CREATE TABLE \"Country\" (\"Id\" NUMBER(10) NOT NULL, \"Name\" VARCHAR2(100) NOT NULL, \"Extra\" NUMBER(10) NULL, CONSTRAINT \"PK_Country\" PRIMARY KEY (\"Id\"));");

                // Act
                var actual = source.CopyDataTo("Person", target, relationshipBehavior: CopyDataRelationshipBehavior.Parents, columnMappingsBehavior: CopyTableSchemaColumnMappingsBehavior.StrictBypass);

                // Assert
                Assert.AreEqual(3 + 6, actual);
                Assert.AreEqual(3, Helper.TargetCount("\"Country\"", "\"Extra\" IS NULL"));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestCopyDataToAsyncWithStrictBypassBypassesTheColumnsOfTheTargetTableThatTheSourceTableDoesNotHave()
        {
            using (var source = new OracleConnection(Database.ConnectionStringForSource))
            using (var target = new OracleConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(ExtraColumnWithDefault);

                // Act
                var actual = await source.CopyDataToAsync("Currency", target, columnMappingsBehavior: CopyTableSchemaColumnMappingsBehavior.StrictBypass);

                // Assert
                Assert.AreEqual(3, actual);
                Assert.AreEqual(3, Helper.TargetCount("\"Currency\"", "\"Extra\" = 7"));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncWithStrictBypassIfTheTargetTableDoesNotHaveAColumnOfTheSourceTable()
        {
            using (var source = new OracleConnection(Database.ConnectionStringForSource))
            using (var target = new OracleConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(MissingColumn);

                // Act/Assert
                await Assert.ThrowsAsync<OracleBulkColumnMappingsException>(() => source.CopyDataToAsync("Currency", target, columnMappingsBehavior: CopyTableSchemaColumnMappingsBehavior.StrictBypass));
                AssertNotCopied();
            }
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncWithStrictBypassCopiesTheColumnsInAnyOrder()
        {
            using (var source = new OracleConnection(Database.ConnectionStringForSource))
            using (var target = new OracleConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget(ReorderedColumns);

                // Act
                var actual = await source.CopyDataToAsync("Currency", target, columnMappingsBehavior: CopyTableSchemaColumnMappingsBehavior.StrictBypass);

                // Assert
                Assert.AreEqual(3, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("\"Currency\"", "\"Code\""), Helper.TargetRows("\"Currency\"", "\"Code\""), "Code", "Name");
            }
        }

        #endregion

        #endregion

        #region Arguments

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheColumnMappingsBehaviorIsNotDefined()
        {
            using (var source = new OracleConnection(Database.ConnectionStringForSource))
            using (var target = new OracleConnection(Database.ConnectionStringForTarget))
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
            using (var source = new OracleConnection(Database.ConnectionStringForSource))
            using (var target = new OracleConnection(Database.ConnectionStringForTarget))
            {
                // Act/Assert
                await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => source.CopyDataToAsync("Currency", target, columnMappingsBehavior: (CopyTableSchemaColumnMappingsBehavior)99));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheBehaviorIsNotAutomaticAndTheSettingOfTheDestinationConnectionIsNotTheOneOfTheBulkOperations()
        {
            using (var source = new OracleConnection(Database.ConnectionStringForSource))
            using (var target = new OracleConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                GlobalConfiguration.Setup().UseOracle(new OracleDbSetting());
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
            using (var source = new OracleConnection(Database.ConnectionStringForSource))
            using (var target = new OracleConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                GlobalConfiguration.Setup().UseOracle(new OracleDbSetting());
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
            using (var source = new OracleConnection(Database.ConnectionStringForSource))
            using (var target = new OracleConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                GlobalConfiguration.Setup().UseOracle(new OracleBulkOperationsDbSetting { BulkColumnMappingsBehavior = OracleBulkColumnMappingsBehavior.Strict });
                Helper.ExecuteOnTarget(MissingColumn);

                // Act
                var actual = source.CopyDataTo("Currency", target, columnMappingsBehavior: CopyTableSchemaColumnMappingsBehavior.Automatic);

                // Assert
                Assert.AreEqual(3, actual);
                Assert.AreEqual(OracleBulkColumnMappingsBehavior.Strict, CurrentBehavior(target));
            }
        }

        #endregion
    }
}
