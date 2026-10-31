#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Data.SqlServer.IntegrationTests.Entities;
using RepoDb.Data.SqlServer.IntegrationTests.Setup;
using RepoDb.Exceptions;
using RepoDb.Schema;

namespace RepoDb.Data.SqlServer.IntegrationTests
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
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Act/Assert
                Assert.Throws<ArgumentNullException>(() => ((SqlConnection)null).CopyDataTo("Currency", target));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheSourceTableIsNullOrWhiteSpace()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
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
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
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
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Act/Assert
                Assert.Throws<ArgumentNullException>(() => source.CopyDataTo("Currency", (SqlConnection)null));
                Assert.Throws<ArgumentNullException>(() => source.CopyDataTo("Currency", "Currency", (SqlConnection)null));
                Assert.Throws<ArgumentNullException>(() => source.CopyDataTo<Currency>((SqlConnection)null));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheBufferSizeIsNotGreaterThanZero()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Currency");

                // Act/Assert
                Assert.Throws<ArgumentOutOfRangeException>(() => source.CopyDataTo("Currency", target, bufferSize: 0));
                Assert.Throws<ArgumentOutOfRangeException>(() => source.CopyDataTo("Currency", target, bufferSize: -1));
                Assert.AreEqual(0, Helper.TargetCount("[dbo].[Currency]"));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncIfTheSourceTableIsNullOrWhiteSpace()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Act/Assert
                await Assert.ThrowsAsync<ArgumentNullException>(() => source.CopyDataToAsync(null, "Currency", target));
                await Assert.ThrowsAsync<ArgumentNullException>(() => source.CopyDataToAsync(" ", "Currency", target));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncIfTheTargetTableIsNullOrWhiteSpace()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Act/Assert
                await Assert.ThrowsAsync<ArgumentNullException>(() => source.CopyDataToAsync("Currency", null, target));
                await Assert.ThrowsAsync<ArgumentNullException>(() => source.CopyDataToAsync("Currency", " ", target));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncIfTheDestinationConnectionIsNull()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            {
                // Act/Assert
                await Assert.ThrowsAsync<ArgumentNullException>(() => source.CopyDataToAsync("Currency", (SqlConnection)null));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncIfTheBufferSizeIsNotGreaterThanZero()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Act/Assert
                await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => source.CopyDataToAsync("Currency", target, bufferSize: 0));
            }
        }

        #endregion

        #endregion

        #region Tables

        #region Sync

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheSourceTableDoesNotExist()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget("CREATE TABLE [dbo].[DoesNotExist] ([Id] INT NOT NULL);");

                // Act/Assert
                Assert.Throws<MissingFieldsException>(() => source.CopyDataTo("DoesNotExist", target));
                Assert.AreEqual(0, Helper.TargetCount("[dbo].[DoesNotExist]"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheTargetTableDoesNotExist()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Act/Assert
                Assert.Throws<MissingFieldsException>(() => source.CopyDataTo("Currency", "CurrencyMissing", target));
                Assert.AreEqual(0, Helper.TargetTables().Count);
            }
        }

        [TestMethod]
        public void TestCopyDataToIntoATargetTableThatDoesNotHaveAColumnOfTheSourceTableCopiesTheColumnsThatBothTablesHave()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget("CREATE TABLE [dbo].[Currency] ([Code] CHAR(3) NOT NULL, CONSTRAINT [PK_Currency] PRIMARY KEY ([Code]));");

                // Act
                var actual = source.CopyDataTo("Currency", target).RowCount;

                // Assert
                Assert.AreEqual(3, actual);
                CollectionAssert.AreEqual(new[] { "EUR", "JPY", "PHP" }, Helper.TargetRows("[dbo].[Currency]", "[Code]").Select(r => ((string)r["Code"]).Trim()).ToArray());
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheTargetTableHasARequiredColumnThatTheSourceTableDoesNotHave()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget("CREATE TABLE [dbo].[Currency] ([Code] CHAR(3) NOT NULL, [Name] NVARCHAR(50) NOT NULL, [Required] INT NOT NULL, CONSTRAINT [PK_Currency] PRIMARY KEY ([Code]));");

                // Act/Assert
                Assert.Throws<SqlException>(() => source.CopyDataTo("Currency", target));
                Assert.AreEqual(0, Helper.TargetCount("[dbo].[Currency]"));
            }
        }

        [TestMethod]
        public void TestCopyDataToIntoATargetTableThatHasAnOptionalColumnThatTheSourceTableDoesNotHave()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget("CREATE TABLE [dbo].[Currency] ([Code] CHAR(3) NOT NULL, [Name] NVARCHAR(50) NOT NULL, [Extra] INT NULL DEFAULT ((7)), CONSTRAINT [PK_Currency] PRIMARY KEY ([Code]));");

                // Act
                var actual = source.CopyDataTo("Currency", target).RowCount;

                // Assert
                Assert.AreEqual(3, actual);
                Assert.AreEqual(3, Helper.TargetCount("[dbo].[Currency]"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheRowsAlreadyExistInTheTargetTable()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Currency");
                source.CopyDataTo("Currency", target);

                // Act/Assert
                Assert.Throws<SqlException>(() => source.CopyDataTo("Currency", target));
                Assert.AreEqual(3, Helper.TargetCount("[dbo].[Currency]"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheValuesAreNotValidForTheTargetTable()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget("CREATE TABLE [dbo].[Currency] ([Code] CHAR(3) NOT NULL, [Name] NVARCHAR(5) NOT NULL, CONSTRAINT [PK_Currency] PRIMARY KEY ([Code]));");

                // Act/Assert
                Assert.Throws<InvalidOperationException>(() => source.CopyDataTo("Currency", target));
                Assert.AreEqual(0, Helper.TargetCount("[dbo].[Currency]"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheTargetTableHasAConstraintThatTheRowsViolate()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.ExecuteOnTarget("CREATE TABLE [dbo].[Currency] ([Code] CHAR(3) NOT NULL, [Name] NVARCHAR(50) NOT NULL, CONSTRAINT [PK_Currency] PRIMARY KEY ([Code]), CONSTRAINT [UQ_Currency_Name] UNIQUE ([Name]));");
                Helper.ExecuteOnTarget("INSERT INTO [dbo].[Currency] ([Code], [Name]) VALUES ('XXX', N'Euro');");

                // Act/Assert
                Assert.Throws<SqlException>(() => source.CopyDataTo("Currency", target));
                Assert.AreEqual(1, Helper.TargetCount("[dbo].[Currency]"));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncIfTheSourceTableDoesNotExist()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Act/Assert
                await Assert.ThrowsAsync<MissingFieldsException>(() => source.CopyDataToAsync("DoesNotExistAsync", target));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncIfTheTargetTableDoesNotExist()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Act/Assert
                await Assert.ThrowsAsync<MissingFieldsException>(() => source.CopyDataToAsync("Currency", "CurrencyMissingAsync", target));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncIfTheRowsAlreadyExistInTheTargetTable()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Currency");
                await source.CopyDataToAsync("Currency", target);

                // Act/Assert
                await Assert.ThrowsAsync<SqlException>(() => source.CopyDataToAsync("Currency", target));
                Assert.AreEqual(3, Helper.TargetCount("[dbo].[Currency]"));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncIfTheOperationIsCancelled()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            using (var tokenSource = new CancellationTokenSource())
            {
                // Setup
                Helper.CreateTargetTables("Number");
                tokenSource.Cancel();

                // Act/Assert
                await Assert.ThrowsAsync<OperationCanceledException>(() => source.CopyDataToAsync("Number", target, cancellationToken: tokenSource.Token));
                Assert.AreEqual(0, Helper.TargetCount("[dbo].[Number]"));
            }
        }

        #endregion

        #endregion

        #region Connections

        [TestMethod]
        public void TestCopyDataToWithConnectionsThatAreAlreadyOpen()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
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
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
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
