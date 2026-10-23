#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Npgsql;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RepoDb.Connector.EnterpriseDb;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Data.EnterpriseDb.IntegrationTests.Entities;
using RepoDb.Data.EnterpriseDb.IntegrationTests.Setup;
using RepoDb.Exceptions;
using RepoDb.Schema;

namespace RepoDb.Data.EnterpriseDb.IntegrationTests
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
            using (var target = new EDBConnection(Database.ConnectionStringForTarget))
            {
                // Act/Assert
                Assert.Throws<ArgumentNullException>(() => ((EDBConnection)null).CopyDataTo("Currency", target));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheSourceTableIsNullOrWhiteSpace()
        {
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
            using (var target = new EDBConnection(Database.ConnectionStringForTarget))
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
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
            using (var target = new EDBConnection(Database.ConnectionStringForTarget))
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
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
            {
                // Act/Assert
                Assert.Throws<ArgumentNullException>(() => source.CopyDataTo("Currency", (EDBConnection)null));
                Assert.Throws<ArgumentNullException>(() => source.CopyDataTo("Currency", "Currency", (EDBConnection)null));
                Assert.Throws<ArgumentNullException>(() => source.CopyDataTo<Currency>((EDBConnection)null));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheBufferSizeIsNotGreaterThanZero()
        {
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
            using (var target = new EDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Currency");

                // Act/Assert
                Assert.Throws<ArgumentOutOfRangeException>(() => source.CopyDataTo("Currency", target, bufferSize: 0));
                Assert.Throws<ArgumentOutOfRangeException>(() => source.CopyDataTo("Currency", target, bufferSize: -1));
                Assert.AreEqual(0, Helper.TargetCount("\"public\".\"Currency\""));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheDestinationConnectionIsNotRegistered()
        {
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
            {
                // Act/Assert
                var exception = Assert.Throws<InvalidOperationException>(() => source.CopyDataTo("Currency", "NotRegistered-" + Guid.NewGuid().ToString("N")));
                StringAssert.Contains(exception.Message, "NotRegistered-");
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheNameOfTheDestinationConnectionIsNullOrWhiteSpace()
        {
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
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
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
            using (var target = new EDBConnection(Database.ConnectionStringForTarget))
            {
                // Act/Assert
                await Assert.ThrowsAsync<ArgumentNullException>(() => source.CopyDataToAsync(null, "Currency", target));
                await Assert.ThrowsAsync<ArgumentNullException>(() => source.CopyDataToAsync(" ", "Currency", target));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncIfTheTargetTableIsNullOrWhiteSpace()
        {
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
            using (var target = new EDBConnection(Database.ConnectionStringForTarget))
            {
                // Act/Assert
                await Assert.ThrowsAsync<ArgumentNullException>(() => source.CopyDataToAsync("Currency", null, target));
                await Assert.ThrowsAsync<ArgumentNullException>(() => source.CopyDataToAsync("Currency", " ", target));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncIfTheDestinationConnectionIsNull()
        {
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
            {
                // Act/Assert
                await Assert.ThrowsAsync<ArgumentNullException>(() => source.CopyDataToAsync("Currency", (EDBConnection)null));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncIfTheBufferSizeIsNotGreaterThanZero()
        {
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
            using (var target = new EDBConnection(Database.ConnectionStringForTarget))
            {
                // Act/Assert
                await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => source.CopyDataToAsync("Currency", target, bufferSize: 0));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncIfTheDestinationConnectionIsNotRegistered()
        {
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
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
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
            using (var target = new EDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget("CREATE TABLE \"public\".\"DoesNotExist\" (\"Id\" INT NOT NULL);");

                // Act/Assert
                Assert.Throws<MissingFieldsException>(() => source.CopyDataTo("DoesNotExist", target));
                Assert.AreEqual(0, Helper.TargetCount("\"public\".\"DoesNotExist\""));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheTargetTableDoesNotExist()
        {
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
            using (var target = new EDBConnection(Database.ConnectionStringForTarget))
            {
                // Act/Assert
                Assert.Throws<MissingFieldsException>(() => source.CopyDataTo("Currency", "CurrencyMissing", target));
                Assert.AreEqual(0, Helper.TargetTables().Count);
            }
        }

        [TestMethod]
        public void TestCopyDataToIntoATargetTableThatDoesNotHaveAColumnOfTheSourceTableCopiesTheColumnsThatBothTablesHave()
        {
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
            using (var target = new EDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget("CREATE TABLE \"public\".\"Currency\" (\"Code\" CHAR(3) NOT NULL, CONSTRAINT \"PK_Currency\" PRIMARY KEY (\"Code\"));");

                // Act
                var actual = source.CopyDataTo("Currency", target).RowCount;

                // Assert
                Assert.AreEqual(3, actual);
                CollectionAssert.AreEqual(new[] { "EUR", "JPY", "PHP" }, Helper.TargetRows("\"public\".\"Currency\"", "\"Code\"").Select(r => ((string)r["Code"]).Trim()).ToArray());
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheTargetTableHasARequiredColumnThatTheSourceTableDoesNotHave()
        {
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
            using (var target = new EDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget("CREATE TABLE \"public\".\"Currency\" (\"Code\" CHAR(3) NOT NULL, \"Name\" varchar(50) NOT NULL, \"Required\" INT NOT NULL, CONSTRAINT \"PK_Currency\" PRIMARY KEY (\"Code\"));");

                // Act/Assert
                Assert.Throws<PostgresException>(() => source.CopyDataTo("Currency", target));
                Assert.AreEqual(0, Helper.TargetCount("\"public\".\"Currency\""));
            }
        }

        [TestMethod]
        public void TestCopyDataToIntoATargetTableThatHasAnOptionalColumnThatTheSourceTableDoesNotHave()
        {
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
            using (var target = new EDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget("CREATE TABLE \"public\".\"Currency\" (\"Code\" CHAR(3) NOT NULL, \"Name\" varchar(50) NOT NULL, \"Extra\" INT NULL DEFAULT ((7)), CONSTRAINT \"PK_Currency\" PRIMARY KEY (\"Code\"));");

                // Act
                var actual = source.CopyDataTo("Currency", target).RowCount;

                // Assert
                Assert.AreEqual(3, actual);
                Assert.AreEqual(3, Helper.TargetCount("\"public\".\"Currency\""));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheRowsAlreadyExistInTheTargetTable()
        {
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
            using (var target = new EDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Currency");
                source.CopyDataTo("Currency", target);

                // Act/Assert
                Assert.Throws<PostgresException>(() => source.CopyDataTo("Currency", target));
                Assert.AreEqual(3, Helper.TargetCount("\"public\".\"Currency\""));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheValuesAreNotValidForTheTargetTable()
        {
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
            using (var target = new EDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget("CREATE TABLE \"public\".\"Currency\" (\"Code\" CHAR(3) NOT NULL, \"Name\" varchar(5) NOT NULL, CONSTRAINT \"PK_Currency\" PRIMARY KEY (\"Code\"));");

                // Act/Assert
                Assert.Throws<PostgresException>(() => source.CopyDataTo("Currency", target));
                Assert.AreEqual(0, Helper.TargetCount("\"public\".\"Currency\""));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheTargetTableHasAConstraintThatTheRowsViolate()
        {
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
            using (var target = new EDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget("CREATE TABLE \"public\".\"Currency\" (\"Code\" CHAR(3) NOT NULL, \"Name\" varchar(50) NOT NULL, CONSTRAINT \"PK_Currency\" PRIMARY KEY (\"Code\"), CONSTRAINT \"UQ_Currency_Name\" UNIQUE (\"Name\"));");
                Helper.ExecuteOnTarget("INSERT INTO \"public\".\"Currency\" (\"Code\", \"Name\") VALUES ('XXX', 'Euro');");

                // Act/Assert
                Assert.Throws<PostgresException>(() => source.CopyDataTo("Currency", target));
                Assert.AreEqual(1, Helper.TargetCount("\"public\".\"Currency\""));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncIfTheSourceTableDoesNotExist()
        {
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
            using (var target = new EDBConnection(Database.ConnectionStringForTarget))
            {
                // Act/Assert
                await Assert.ThrowsAsync<MissingFieldsException>(() => source.CopyDataToAsync("DoesNotExistAsync", target));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncIfTheTargetTableDoesNotExist()
        {
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
            using (var target = new EDBConnection(Database.ConnectionStringForTarget))
            {
                // Act/Assert
                await Assert.ThrowsAsync<MissingFieldsException>(() => source.CopyDataToAsync("Currency", "CurrencyMissingAsync", target));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncIfTheRowsAlreadyExistInTheTargetTable()
        {
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
            using (var target = new EDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Currency");
                await source.CopyDataToAsync("Currency", target);

                // Act/Assert
                await Assert.ThrowsAsync<PostgresException>(() => source.CopyDataToAsync("Currency", target));
                Assert.AreEqual(3, Helper.TargetCount("\"public\".\"Currency\""));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncIfTheOperationIsCancelled()
        {
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
            using (var target = new EDBConnection(Database.ConnectionStringForTarget))
            using (var tokenSource = new CancellationTokenSource())
            {
                // Setup
                Helper.CreateTargetTables("Number");
                tokenSource.Cancel();

                // Act/Assert
                await Assert.ThrowsAsync<OperationCanceledException>(() => source.CopyDataToAsync("Number", target, cancellationToken: tokenSource.Token));
                Assert.AreEqual(0, Helper.TargetCount("\"public\".\"Number\""));
            }
        }

        #endregion

        #endregion

        #region Connections

        [TestMethod]
        public void TestCopyDataToWithConnectionsThatAreAlreadyOpen()
        {
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
            using (var target = new EDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Currency");
                source.Open();
                target.Open();

                // Act
                var actual = source.CopyDataTo("Currency", target).RowCount;

                // Assert
                Assert.AreEqual(3, actual);
                Assert.AreEqual(System.Data.ConnectionState.Open, source.State);
                Assert.AreEqual(System.Data.ConnectionState.Open, target.State);
            }
        }

        [TestMethod]
        public void TestCopyDataToCanBeCalledManyTimesWithTheSameConnections()
        {
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
            using (var target = new EDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Currency", "Country");

                // Act
                var first = source.CopyDataTo("Currency", target).RowCount;
                var second = source.CopyDataTo("Country", target).RowCount;

                // Assert
                Assert.AreEqual(3, first);
                Assert.AreEqual(3, second);
            }
        }

        #endregion
    }
}
