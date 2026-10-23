#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Data.Enumerations;
using RepoDb.Data.Models;
using RepoDb.Data.MySql.IntegrationTests.Entities;
using RepoDb.Data.MySql.IntegrationTests.Setup;
using RepoDb.Exceptions;
using RepoDb.Schema;

namespace RepoDb.Data.MySql.IntegrationTests
{
    /// <summary>
    /// The arguments that are not valid, and the tables of the destination database that cannot receive the rows.
    /// </summary>
    [TestClass]
    public class CopyDataToArgumentTest
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

        #region Arguments

        #region Sync

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheConnectionIsNull()
        {
            using (var target = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Act/Assert
                Assert.Throws<ArgumentNullException>(() => ((MySqlConnection)null).CopyDataTo("Currency", target));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheSourceTableIsNullOrWhiteSpace()
        {
            using (var source = new MySqlConnection(Database.ConnectionStringForSource))
            using (var target = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Act/Assert
                Assert.Throws<ArgumentNullException>(() => source.CopyDataTo(null, "Currency", target));
                Assert.Throws<ArgumentNullException>(() => source.CopyDataTo(string.Empty, "Currency", target));
                Assert.Throws<ArgumentNullException>(() => source.CopyDataTo(" ", "Currency", target));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheTargetTableIsNullOrWhiteSpace()
        {
            using (var source = new MySqlConnection(Database.ConnectionStringForSource))
            using (var target = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Act/Assert
                Assert.Throws<ArgumentNullException>(() => source.CopyDataTo("Currency", null, target));
                Assert.Throws<ArgumentNullException>(() => source.CopyDataTo("Currency", string.Empty, target));
                Assert.Throws<ArgumentNullException>(() => source.CopyDataTo("Currency", " ", target));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheDestinationConnectionIsNull()
        {
            using (var source = new MySqlConnection(Database.ConnectionStringForSource))
            {
                // Act/Assert
                Assert.Throws<ArgumentNullException>(() => source.CopyDataTo("Currency", (MySqlConnection)null));
                Assert.Throws<ArgumentNullException>(() => source.CopyDataTo("Currency", "Currency", (MySqlConnection)null));
                Assert.Throws<ArgumentNullException>(() => source.CopyDataTo<Currency>((MySqlConnection)null));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheBatchSizeIsNotGreaterThanZero()
        {
            using (var source = new MySqlConnection(Database.ConnectionStringForSource))
            using (var target = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Currency");

                // Act/Assert
                Assert.Throws<ArgumentOutOfRangeException>(() => source.CopyDataTo("Currency", target, batchSize: 0));
                Assert.Throws<ArgumentOutOfRangeException>(() => source.CopyDataTo("Currency", target, batchSize: -1));
                Assert.AreEqual(0, Helper.TargetCount("`Currency`"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheDestinationConnectionIsNotRegistered()
        {
            using (var source = new MySqlConnection(Database.ConnectionStringForSource))
            {
                // Act/Assert
                var exception = Assert.Throws<InvalidOperationException>(() => source.CopyDataTo("Currency", "NotRegistered-" + Guid.NewGuid().ToString("N")));
                StringAssert.Contains(exception.Message, "NotRegistered-");
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheNameOfTheDestinationConnectionIsNullOrWhiteSpace()
        {
            using (var source = new MySqlConnection(Database.ConnectionStringForSource))
            {
                // Act/Assert
                Assert.Throws<ArgumentNullException>(() => source.CopyDataTo("Currency", (string)null));
                Assert.Throws<ArgumentNullException>(() => source.CopyDataTo("Currency", " "));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncIfTheSourceTableIsNullOrWhiteSpace()
        {
            using (var source = new MySqlConnection(Database.ConnectionStringForSource))
            using (var target = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Act/Assert
                await Assert.ThrowsAsync<ArgumentNullException>(() => source.CopyDataToAsync(null, "Currency", target));
                await Assert.ThrowsAsync<ArgumentNullException>(() => source.CopyDataToAsync(" ", "Currency", target));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncIfTheTargetTableIsNullOrWhiteSpace()
        {
            using (var source = new MySqlConnection(Database.ConnectionStringForSource))
            using (var target = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Act/Assert
                await Assert.ThrowsAsync<ArgumentNullException>(() => source.CopyDataToAsync("Currency", null, target));
                await Assert.ThrowsAsync<ArgumentNullException>(() => source.CopyDataToAsync("Currency", " ", target));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncIfTheDestinationConnectionIsNull()
        {
            using (var source = new MySqlConnection(Database.ConnectionStringForSource))
            {
                // Act/Assert
                await Assert.ThrowsAsync<ArgumentNullException>(() => source.CopyDataToAsync("Currency", (MySqlConnection)null));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncIfTheBatchSizeIsNotGreaterThanZero()
        {
            using (var source = new MySqlConnection(Database.ConnectionStringForSource))
            using (var target = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Act/Assert
                await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => source.CopyDataToAsync("Currency", target, batchSize: 0));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncIfTheDestinationConnectionIsNotRegistered()
        {
            using (var source = new MySqlConnection(Database.ConnectionStringForSource))
            {
                // Act/Assert
                await Assert.ThrowsAsync<InvalidOperationException>(() => source.CopyDataToAsync("Currency", "NotRegistered-" + Guid.NewGuid().ToString("N")));
            }
        }

        #endregion

        #endregion

        #region Tables

        #region Sync

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheSourceTableDoesNotExist()
        {
            using (var source = new MySqlConnection(Database.ConnectionStringForSource))
            using (var target = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget("CREATE TABLE `DoesNotExist` (`Id` INT NOT NULL);");

                // Act/Assert
                Assert.Throws<MissingFieldsException>(() => source.CopyDataTo("DoesNotExist", target));
                Assert.AreEqual(0, Helper.TargetCount("`DoesNotExist`"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheTargetTableDoesNotExist()
        {
            using (var source = new MySqlConnection(Database.ConnectionStringForSource))
            using (var target = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Act/Assert
                Assert.Throws<MissingFieldsException>(() => source.CopyDataTo("Currency", "CurrencyMissing", target));
                Assert.AreEqual(0, Helper.TargetTables().Count);
            }
        }

        [TestMethod]
        public void TestCopyDataToIntoATargetTableThatDoesNotHaveAColumnOfTheSourceTableCopiesTheColumnsThatBothTablesHave()
        {
            using (var source = new MySqlConnection(Database.ConnectionStringForSource))
            using (var target = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget("CREATE TABLE `Currency` (`Code` CHAR(3) NOT NULL, CONSTRAINT `PK_Currency` PRIMARY KEY (`Code`));");

                // Act
                var actual = source.CopyDataTo("Currency", target);

                // Assert
                Assert.AreEqual(3, actual);
                CollectionAssert.AreEqual(new[] { "EUR", "JPY", "PHP" }, Helper.TargetRows("`Currency`", "`Code`").Select(r => ((string)r["Code"]).Trim()).ToArray());
            }
        }

        [TestMethod]
        public void TestCopyDataToIntoATargetTableThatHasARequiredColumnThatTheSourceTableDoesNotHaveFillsItWithTheImplicitDefault()
        {
            using (var source = new MySqlConnection(Database.ConnectionStringForSource))
            using (var target = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget("CREATE TABLE `Currency` (`Code` CHAR(3) NOT NULL, `Name` VARCHAR(50) NOT NULL, `Required` INT NOT NULL, CONSTRAINT `PK_Currency` PRIMARY KEY (`Code`));");

                // Act (the bulk load of MySQL is a LOAD DATA LOCAL, that fills the missing columns with their implicit default)
                var actual = source.CopyDataTo("Currency", target);

                // Assert
                Assert.AreEqual(3, actual);
                Assert.AreEqual(3, Helper.TargetCount("`Currency`"));
            }
        }

        [TestMethod]
        public void TestCopyDataToIntoATargetTableThatHasAnOptionalColumnThatTheSourceTableDoesNotHave()
        {
            using (var source = new MySqlConnection(Database.ConnectionStringForSource))
            using (var target = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget("CREATE TABLE `Currency` (`Code` CHAR(3) NOT NULL, `Name` VARCHAR(50) NOT NULL, `Extra` INT NULL DEFAULT ((7)), CONSTRAINT `PK_Currency` PRIMARY KEY (`Code`));");

                // Act
                var actual = source.CopyDataTo("Currency", target);

                // Assert
                Assert.AreEqual(3, actual);
                Assert.AreEqual(3, Helper.TargetCount("`Currency`"));
            }
        }

        [TestMethod]
        public void TestCopyDataToIgnoresTheRowsThatAlreadyExistInTheTargetTable()
        {
            using (var source = new MySqlConnection(Database.ConnectionStringForSource))
            using (var target = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Currency");
                source.CopyDataTo("Currency", target);

                // Act (the bulk load of MySQL is a LOAD DATA LOCAL, that skips the rows that violate a key)
                source.CopyDataTo("Currency", target);

                // Assert
                Assert.AreEqual(3, Helper.TargetCount("`Currency`"));
            }
        }

        [TestMethod]
        public void TestCopyDataToTruncatesTheValuesThatAreTooLongForTheTargetTable()
        {
            using (var source = new MySqlConnection(Database.ConnectionStringForSource))
            using (var target = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget("CREATE TABLE `Currency` (`Code` CHAR(3) NOT NULL, `Name` VARCHAR(5) NOT NULL, CONSTRAINT `PK_Currency` PRIMARY KEY (`Code`));");

                // Act (the bulk load of MySQL is a LOAD DATA LOCAL, that truncates the values instead of failing)
                var actual = source.CopyDataTo("Currency", target);

                // Assert
                Assert.AreEqual(3, actual);
                Assert.IsTrue(Helper.Read(Database.ConnectionStringForTarget, "SELECT `Name` FROM `Currency`").All(row => ((string)row["Name"]).Length <= 5));
            }
        }

        [TestMethod]
        public void TestCopyDataToSkipsTheRowsThatViolateAConstraintOfTheTargetTable()
        {
            using (var source = new MySqlConnection(Database.ConnectionStringForSource))
            using (var target = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget("CREATE TABLE `Currency` (`Code` CHAR(3) NOT NULL, `Name` VARCHAR(50) NOT NULL, CONSTRAINT `PK_Currency` PRIMARY KEY (`Code`), CONSTRAINT `UQ_Currency_Name` UNIQUE (`Name`));");
                Helper.ExecuteOnTarget("INSERT INTO `Currency` (`Code`, `Name`) VALUES ('XXX', 'Euro');");

                // Act (the bulk load of MySQL is a LOAD DATA LOCAL, that skips the rows that violate a constraint)
                CopyProgress progress = null;
                source.CopyDataTo("Currency", target, progressCallback: p => progress = p);

                // Assert (the row of Euro is skipped, since the name is unique)
                Assert.AreEqual(3, Helper.TargetCount("`Currency`"));
                Assert.IsTrue(progress.Logs.Any(log => log.Level == CopyLogLevel.Warning && log.Text.Contains("1 of 3 row(s) were skipped", StringComparison.Ordinal)));
                Assert.IsTrue(progress.Logs.All(log => log.DatabaseType == target.GetType()));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncIfTheSourceTableDoesNotExist()
        {
            using (var source = new MySqlConnection(Database.ConnectionStringForSource))
            using (var target = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Act/Assert
                await Assert.ThrowsAsync<MissingFieldsException>(() => source.CopyDataToAsync("DoesNotExistAsync", target));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncIfTheTargetTableDoesNotExist()
        {
            using (var source = new MySqlConnection(Database.ConnectionStringForSource))
            using (var target = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Act/Assert
                await Assert.ThrowsAsync<MissingFieldsException>(() => source.CopyDataToAsync("Currency", "CurrencyMissingAsync", target));
            }
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncIgnoresTheRowsThatAlreadyExistInTheTargetTable()
        {
            using (var source = new MySqlConnection(Database.ConnectionStringForSource))
            using (var target = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Currency");
                await source.CopyDataToAsync("Currency", target);

                // Act (the bulk load of MySQL is a LOAD DATA LOCAL, that skips the rows that violate a key)
                await source.CopyDataToAsync("Currency", target);

                // Assert
                Assert.AreEqual(3, Helper.TargetCount("`Currency`"));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncIfTheOperationIsCancelled()
        {
            using (var source = new MySqlConnection(Database.ConnectionStringForSource))
            using (var target = new MySqlConnection(Database.ConnectionStringForTarget))
            using (var tokenSource = new CancellationTokenSource())
            {
                // Setup
                Helper.CreateTargetTables("Number");
                tokenSource.Cancel();

                // Act/Assert
                await Assert.ThrowsAsync<OperationCanceledException>(() => source.CopyDataToAsync("Number", target, cancellationToken: tokenSource.Token));
                Assert.AreEqual(0, Helper.TargetCount("`Number`"));
            }
        }

        #endregion

        #endregion

        #region Connections

        [TestMethod]
        public void TestCopyDataToWithConnectionsThatAreAlreadyOpen()
        {
            using (var source = new MySqlConnection(Database.ConnectionStringForSource))
            using (var target = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Currency");
                source.Open();
                target.Open();

                // Act
                var actual = source.CopyDataTo("Currency", target);

                // Assert
                Assert.AreEqual(3, actual);
                Assert.AreEqual(System.Data.ConnectionState.Open, source.State);
                Assert.AreEqual(System.Data.ConnectionState.Open, target.State);
            }
        }

        [TestMethod]
        public void TestCopyDataToCanBeCalledManyTimesWithTheSameConnections()
        {
            using (var source = new MySqlConnection(Database.ConnectionStringForSource))
            using (var target = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Currency", "Country");

                // Act
                var first = source.CopyDataTo("Currency", target);
                var second = source.CopyDataTo("Country", target);

                // Assert
                Assert.AreEqual(3, first);
                Assert.AreEqual(3, second);
            }
        }

        #endregion
    }
}
