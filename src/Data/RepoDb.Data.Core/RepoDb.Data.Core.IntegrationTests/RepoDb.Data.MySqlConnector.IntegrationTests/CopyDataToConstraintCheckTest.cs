#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySqlConnector;
using RepoDb.Data.Enumerations;
using RepoDb.Data.MySqlConnector.IntegrationTests.Setup;
using RepoDb.Schema;

namespace RepoDb.Data.MySqlConnector.IntegrationTests
{
    /// <summary>
    /// Copies the rows of a table that references itself (a row is stored before the row that it references), which the foreign key of MySQL refuses
    /// unless the checks are deferred (see <see cref="ConstraintCheckBehavior"/>).
    /// </summary>
    [TestClass]
    public class CopyDataToConstraintCheckTest
    {
        #region Private Variables

        /// <summary>
        /// The statement that drops the table of the source database (the source database is shared by the tests, so the table is not left behind).
        /// </summary>
        private const string DropSourceTable = "DROP TABLE IF EXISTS `Staff`;";

        /// <summary>
        /// The statement that creates the table in the source database (it has no foreign key, so its rows can be stored in any order).
        /// </summary>
        private const string CreateSourceTable = @"CREATE TABLE `Staff` (`Id` INT NOT NULL PRIMARY KEY, `ReportsTo` INT NULL);";

        /// <summary>
        /// The statement that creates the table in the target database, with the foreign key to itself.
        /// </summary>
        private const string CreateTargetTable = @"CREATE TABLE `Staff` (`Id` INT NOT NULL PRIMARY KEY, `ReportsTo` INT NULL, CONSTRAINT `FK_Staff_Staff` FOREIGN KEY (`ReportsTo`) REFERENCES `Staff` (`Id`));";

        /// <summary>
        /// The statement that stores the employees: the first one reports to the second one, which is stored after it.
        /// </summary>
        private const string InsertSourceRows = "INSERT INTO `Staff` (`Id`, `ReportsTo`) VALUES (1, 2), (2, NULL), (3, 2), (4, 1);";

        #endregion

        #region Public Methods

        [TestInitialize]
        public void Initialize()
        {
            Database.Initialize();
            Database.Cleanup();
            Helper.ExecuteOnSource(DropSourceTable);
            Helper.ExecuteOnSource(CreateSourceTable);
            Helper.ExecuteOnSource(InsertSourceRows);
            Helper.ExecuteOnTarget(CreateTargetTable);
        }

        [TestCleanup]
        public void Cleanup()
        {
            SchemaReaderMapper.Clear();
            Helper.ExecuteOnSource(DropSourceTable);
            Database.Cleanup();
        }

        [TestMethod]
        public void TestCopyDataToRefusesTheRowsOfATableThatReferencesItselfWhenTheChecksAreEnforced()
        {
            using (var source = new MySqlConnection(Database.ConnectionStringForSource))
            using (var target = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Act/Assert
                Assert.Throws<MySqlException>(() => source.CopyDataTo("Staff", target));
                Assert.IsTrue(Helper.TargetCount("`Staff`") < 4);
            }
        }

        [TestMethod]
        public void TestCopyDataToCopiesTheRowsOfATableThatReferencesItselfWhenTheChecksAreDeferred()
        {
            using (var source = new MySqlConnection(Database.ConnectionStringForSource))
            using (var target = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                target.Open();

                // Act
                var actual = source.CopyDataTo("Staff", target, constraintCheckBehavior: ConstraintCheckBehavior.Defer).RowCount;

                // Assert
                Assert.AreEqual(4, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("`Staff`", "`Id`"), Helper.TargetRows("`Staff`", "`Id`"));
                Assert.AreEqual(1L, target.ExecuteScalar<long>("SELECT @@FOREIGN_KEY_CHECKS;"));
            }
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncCopiesTheRowsOfATableThatReferencesItselfWhenTheChecksAreDeferred()
        {
            using (var source = new MySqlConnection(Database.ConnectionStringForSource))
            using (var target = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                await target.OpenAsync();

                // Act
                var actual = (await source.CopyDataToAsync("Staff", target, constraintCheckBehavior: ConstraintCheckBehavior.Defer)).RowCount;

                // Assert
                Assert.AreEqual(4, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("`Staff`", "`Id`"), Helper.TargetRows("`Staff`", "`Id`"));
                Assert.AreEqual(1L, await target.ExecuteScalarAsync<long>("SELECT @@FOREIGN_KEY_CHECKS;"));
            }
        }

        [TestMethod]
        public void TestCopyDataToChecksTheForeignKeysAgainWhenTheCopyFailsAfterTheyWereDeferred()
        {
            using (var source = new MySqlConnection(Database.ConnectionStringForSource))
            using (var target = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                target.Open();
                Helper.ExecuteOnTarget("INSERT INTO `Staff` (`Id`, `ReportsTo`) VALUES (3, NULL);");

                // Act/Assert
                Assert.Throws<MySqlException>(() => source.CopyDataTo("Staff", target, constraintCheckBehavior: ConstraintCheckBehavior.Defer));
                Assert.AreEqual(1L, target.ExecuteScalar<long>("SELECT @@FOREIGN_KEY_CHECKS;"));
            }
        }

        #endregion
    }
}
