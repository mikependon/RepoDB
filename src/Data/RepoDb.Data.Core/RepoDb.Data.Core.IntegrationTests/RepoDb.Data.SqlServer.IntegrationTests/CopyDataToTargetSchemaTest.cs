#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Data.Enumerations;
using RepoDb.Data.SqlServer.IntegrationTests.Setup;
using RepoDb.Enumerations;
using RepoDb.Schema;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Data.SqlServer.IntegrationTests
{
    /// <summary>
    /// Copies the rows of a table into a target table of another schema (the schema of the name of the target table, i.e.: <c>Sales.Person</c>).
    /// The tables are created in that schema, and the copy of the schema requires the source and the target tables to have the same name.
    /// </summary>
    [TestClass]
    public class CopyDataToTargetSchemaTest
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

        #region Sync

        [TestMethod]
        public void TestCopyDataToWithATargetTableOfAnotherSchemaCreatesTheTableInThatSchema()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Currency", "Sales.Currency", target).RowCount;

                // Assert
                Assert.AreEqual(3, actual);
                CollectionAssert.AreEqual(new[] { "[Sales].[Currency]" }, Helper.TargetTables().ToArray());
                Helper.AssertRowsEqual(Helper.SourceRows("[dbo].[Currency]", "[Code]"), Helper.TargetRows("[Sales].[Currency]", "[Code]"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithAQuotedTargetTableOfAnotherSchema()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Currency", "[Sales].[Currency]", target).RowCount;

                // Assert
                Assert.AreEqual(3, actual);
                Assert.AreEqual(3, Helper.TargetCount("[Sales].[Currency]"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithATargetTableOfTheDefaultSchema()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Currency", "dbo.Currency", target).RowCount;

                // Assert
                Assert.AreEqual(3, actual);
                CollectionAssert.AreEqual(new[] { "[dbo].[Currency]" }, Helper.TargetTables().ToArray());
            }
        }

        [TestMethod]
        public void TestCopyDataToFromATableOfAnotherSchemaIntoTheDefaultSchema()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Sales.Invoice", "dbo.Invoice", target).RowCount;

                // Assert
                Assert.AreEqual(2, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("[Sales].[Invoice]", "[Id]"), Helper.TargetRows("[dbo].[Invoice]", "[Id]"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithATargetTableOfAnotherSchemaAddsTheRowsToTheTableThatExists()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.ExecuteOnTarget("CREATE TABLE [Sales].[Currency] ([Code] CHAR(3) NOT NULL, [Name] NVARCHAR(50) NOT NULL, CONSTRAINT [PK_Currency] PRIMARY KEY ([Code])); INSERT INTO [Sales].[Currency] ([Code], [Name]) VALUES ('USD', N'US Dollar');");

                // Act
                var actual = source.CopyDataTo("Currency", "Sales.Currency", target).RowCount;

                // Assert
                Assert.AreEqual(3, actual);
                Assert.AreEqual(4, Helper.TargetCount("[Sales].[Currency]"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithATargetTableOfAnotherSchemaAndParentsCopiesTheParentsIntoThatSchema()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Person", "Sales.Person", target, relationshipBehavior: CopyRelationshipBehavior.Parents).RowCount;

                // Assert
                Assert.AreEqual(3 + 6, actual);
                CollectionAssert.AreEqual(new[] { "[Sales].[Country]", "[Sales].[Person]" }, Helper.TargetTables().ToArray());
                Helper.AssertRowsEqual(Helper.SourceRows("[dbo].[Country]", "[Id]"), Helper.TargetRows("[Sales].[Country]", "[Id]"));
                Helper.AssertRowsEqual(Helper.SourceRows("[dbo].[Person]", "[Id]"), Helper.TargetRows("[Sales].[Person]", "[Id]"));
                Assert.AreEqual(0, Helper.TargetOrphans("[Sales].[Person]", "[CountryId]", "[Sales].[Country]", "[Id]"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithATargetTableOfAnotherSchemaAndChildrenCopiesTheChildrenIntoThatSchema()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Country", "Sales.Country", target, relationshipBehavior: CopyRelationshipBehavior.Children).RowCount;

                // Assert
                Assert.AreEqual(3 + 6 + 5 + 5, actual);
                CollectionAssert.AreEqual(new[] { "[Sales].[Country]", "[Sales].[Person]", "[Sales].[Pet]", "[Sales].[Toy]" }, Helper.TargetTables().ToArray());
                Assert.AreEqual(0, Helper.TargetOrphans("[Sales].[Toy]", "[PetId]", "[Sales].[Pet]", "[Id]"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithATargetTableOfAnotherSchemaAndAFilterFiltersTheRequestedTableOnly()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Person", "Sales.Person", target, where: new QueryGroup(new QueryField("Age", Operation.GreaterThan, 40)), relationshipBehavior: CopyRelationshipBehavior.Parents).RowCount;

                // Assert
                Assert.AreEqual(3 + 2, actual);
                Assert.AreEqual(2, Helper.TargetCount("[Sales].[Person]"));
                Assert.AreEqual(3, Helper.TargetCount("[Sales].[Country]"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithATargetTableOfAnotherSchemaAndDropReplacesTheTableThatExists()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                source.CopyDataTo("Currency", "Sales.Currency", target);

                // Act
                var actual = source.CopyDataTo("Currency", "Sales.Currency", target, tableExistenceBehavior: TableExistsBehavior.Drop).RowCount;

                // Assert
                Assert.AreEqual(3, actual);
                Assert.AreEqual(3, Helper.TargetCount("[Sales].[Currency]"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheTablesHaveDifferentNamesAndTheTargetTableHasASchema()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act/Assert
                Assert.Throws<ArgumentException>(() => source.CopyDataTo("Currency", "Sales.Money", target));
                Assert.AreEqual(0, Helper.TargetTables().Count);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheTargetSchemaDoesNotExist()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act/Assert
                Assert.ThrowsExactly<SqlException>(() => source.CopyDataTo("Currency", "Missing.Currency", target));
                Assert.AreEqual(0, Helper.TargetTables().Count);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheTableReferencesATableThatDoesNotExistInTheTargetSchema()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act/Assert
                Assert.ThrowsExactly<SqlException>(() => source.CopyDataTo("Person", "Sales.Person", target));
                Assert.AreEqual(0, Helper.TargetCount("[Sales].[Person]"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheTargetTableOfAnotherSchemaIsCopiedTwiceBecauseTheRowsAlreadyExist()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                source.CopyDataTo("Currency", "Sales.Currency", target);

                // Act/Assert
                Assert.ThrowsExactly<SqlException>(() => source.CopyDataTo("Currency", "Sales.Currency", target));
                Assert.AreEqual(3, Helper.TargetCount("[Sales].[Currency]"));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestCopyDataToAsyncWithATargetTableOfAnotherSchemaCreatesTheTableInThatSchema()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = (await source.CopyDataToAsync("Currency", "Sales.Currency", target)).RowCount;

                // Assert
                Assert.AreEqual(3, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("[dbo].[Currency]", "[Code]"), Helper.TargetRows("[Sales].[Currency]", "[Code]"));
            }
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncWithATargetTableOfAnotherSchemaAndParentsCopiesTheParentsIntoThatSchema()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = (await source.CopyDataToAsync("Person", "Sales.Person", target, relationshipBehavior: CopyRelationshipBehavior.Parents)).RowCount;

                // Assert
                Assert.AreEqual(3 + 6, actual);
                CollectionAssert.AreEqual(new[] { "[Sales].[Country]", "[Sales].[Person]" }, Helper.TargetTables().ToArray());
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncIfTheTablesHaveDifferentNamesAndTheTargetTableHasASchema()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act/Assert
                await Assert.ThrowsAsync<ArgumentException>(() => source.CopyDataToAsync("Currency", "Sales.Money", target));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncIfTheTargetSchemaDoesNotExist()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act/Assert
                await Assert.ThrowsExactlyAsync<SqlException>(() => source.CopyDataToAsync("Currency", "Missing.Currency", target));
            }
        }

        #endregion
    }
}
