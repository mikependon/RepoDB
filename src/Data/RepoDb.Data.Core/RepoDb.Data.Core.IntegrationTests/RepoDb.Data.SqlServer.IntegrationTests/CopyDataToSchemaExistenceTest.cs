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
using RepoDb.Exceptions;
using RepoDb.Schema;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Data.SqlServer.IntegrationTests
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
            Helper.Read(Database.ConnectionStringForTarget, $"SELECT [name] FROM sys.columns WHERE [object_id] = OBJECT_ID(N'{quotedName}') ORDER BY [column_id]")
                .Select(row => (string)row["name"])
                .ToArray();

        private static bool TargetIndexExists(string quotedName,
            string indexName) =>
            Helper.Read(Database.ConnectionStringForTarget, $"SELECT [name] FROM sys.indexes WHERE [object_id] = OBJECT_ID(N'{quotedName}') AND [name] = N'{indexName}'").Count == 1;

        #endregion

        #region Skip

        #region Sync

        [TestMethod]
        public void TestCopyDataToWithSkipCreatesTheMissingTables()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Person", target, relationshipBehavior: CopyRelationshipBehavior.Parents, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Skip);

                // Assert
                Assert.AreEqual(3 + 6, actual);
                CollectionAssert.AreEqual(new[] { "[dbo].[Country]", "[dbo].[Person]" }, Helper.TargetTables().ToArray());
            }
        }

        [TestMethod]
        public void TestCopyDataToWithSkipLeavesTheTablesThatExistAsTheyAre()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.ExecuteOnTarget("CREATE TABLE [dbo].[Currency] ([Code] CHAR(3) NOT NULL, [Name] NVARCHAR(50) NOT NULL, [Extra] INT NULL, CONSTRAINT [PK_Currency] PRIMARY KEY ([Code]));");

                // Act
                var actual = source.CopyDataTo("Currency", target, relationshipBehavior: CopyRelationshipBehavior.Parents, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Skip);

                // Assert
                Assert.AreEqual(3, actual);
                CollectionAssert.AreEqual(new[] { "Code", "Name", "Extra" }, TargetColumns("[dbo].[Currency]"));
                Assert.AreEqual(3, Helper.TargetCount("[dbo].[Currency]"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithSkipDoesNotAddTheMissingColumnsOfTheTablesThatExist()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.ExecuteOnTarget("CREATE TABLE [dbo].[Currency] ([Code] CHAR(3) NOT NULL, CONSTRAINT [PK_Currency] PRIMARY KEY ([Code]));");

                // Act
                var actual = source.CopyDataTo("Currency", target, relationshipBehavior: CopyRelationshipBehavior.Parents, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Skip);

                // Assert
                Assert.AreEqual(3, actual);
                CollectionAssert.AreEqual(new[] { "Code" }, TargetColumns("[dbo].[Currency]"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToWithSkipAndWithoutASchemaCopyIfTheTableDoesNotExist()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Act/Assert
                Assert.Throws<MissingFieldsException>(() => source.CopyDataTo("Currency", target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Skip));
                Assert.AreEqual(0, Helper.TargetTables().Count);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestCopyDataToAsyncWithSkipCreatesTheMissingTables()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = await source.CopyDataToAsync("Person", target, relationshipBehavior: CopyRelationshipBehavior.Parents, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Skip);

                // Assert
                Assert.AreEqual(3 + 6, actual);
                CollectionAssert.AreEqual(new[] { "[dbo].[Country]", "[dbo].[Person]" }, Helper.TargetTables().ToArray());
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncWithSkipAndWithoutASchemaCopyIfTheTableDoesNotExist()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
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
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Currency", target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Throw);

                // Assert
                Assert.AreEqual(3, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("[dbo].[Currency]", "[Code]"), Helper.TargetRows("[dbo].[Currency]", "[Code]"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToWithThrowIfTheTableExists()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.CreateTargetTables("Currency");

                // Act
                var exception = Assert.Throws<InvalidOperationException>(() => source.CopyDataTo("Currency", target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Throw));

                // Assert
                StringAssert.Contains(exception.Message, "Currency");
                StringAssert.Contains(exception.Message, "already exists");
                Assert.AreEqual(0, Helper.TargetCount("[dbo].[Currency]"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToWithThrowIfAParentTableExists()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.CreateTargetTables("Country");

                // Act/Assert
                Assert.Throws<InvalidOperationException>(() => source.CopyDataTo("Person", target, relationshipBehavior: CopyRelationshipBehavior.Parents, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Throw));
                Assert.IsFalse(Helper.TargetTableExists("Person"));
                Assert.AreEqual(0, Helper.TargetCount("[dbo].[Country]"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToWithThrowTheSecondTimeThatTheTableIsCopied()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                source.CopyDataTo("Currency", target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Throw);

                // Act/Assert
                Assert.Throws<InvalidOperationException>(() => source.CopyDataTo("Currency", target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Throw));
                Assert.AreEqual(3, Helper.TargetCount("[dbo].[Currency]"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithThrowAndAllTheTablesMissingCopiesTheWholeTree()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Pet", target, relationshipBehavior: CopyRelationshipBehavior.ParentsAndChildren, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Throw);

                // Assert
                Assert.AreEqual(3 + 6 + 5 + 5, actual);
                Assert.AreEqual(4, Helper.TargetTables().Count);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestCopyDataToAsyncWithThrowCreatesTheTableIfItDoesNotExist()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = await source.CopyDataToAsync("Currency", target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Throw);

                // Assert
                Assert.AreEqual(3, actual);
                Assert.AreEqual(3, Helper.TargetCount("[dbo].[Currency]"));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncWithThrowIfTheTableExists()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.CreateTargetTables("Currency");

                // Act/Assert
                await Assert.ThrowsAsync<InvalidOperationException>(() => source.CopyDataToAsync("Currency", target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Throw));
                Assert.AreEqual(0, Helper.TargetCount("[dbo].[Currency]"));
            }
        }

        #endregion

        #endregion

        #region Drop

        #region Sync

        [TestMethod]
        public void TestCopyDataToWithDropCreatesTheTableIfItDoesNotExist()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Currency", target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Drop);

                // Assert
                Assert.AreEqual(3, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("[dbo].[Currency]", "[Code]"), Helper.TargetRows("[dbo].[Currency]", "[Code]"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithDropReplacesTheTableThatExistsAndItsRows()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.CreateTargetTables("Currency");
                Helper.ExecuteOnTarget("INSERT INTO [dbo].[Currency] ([Code], [Name]) VALUES ('USD', N'US Dollar');");

                // Act
                var actual = source.CopyDataTo("Currency", target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Drop);

                // Assert
                Assert.AreEqual(3, actual);
                Assert.AreEqual(3, Helper.TargetCount("[dbo].[Currency]"));
                Assert.AreEqual(0, Helper.TargetCount("[dbo].[Currency]", "[Code] = 'USD'"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithDropReplacesTheTableThatExistsWithTheStructureOfTheSourceTable()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.ExecuteOnTarget("CREATE TABLE [dbo].[Currency] ([Code] INT NOT NULL, [Foo] NVARCHAR(10) NULL);");

                // Act
                var actual = source.CopyDataTo("Currency", target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Drop);

                // Assert
                Assert.AreEqual(3, actual);
                CollectionAssert.AreEqual(new[] { "Code", "Name" }, TargetColumns("[dbo].[Currency]"));
                Helper.AssertRowsEqual(Helper.SourceRows("[dbo].[Currency]", "[Code]"), Helper.TargetRows("[dbo].[Currency]", "[Code]"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithDropAndParentsReplacesTheTablesThatExist()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                source.CopyDataTo("Person", target, relationshipBehavior: CopyRelationshipBehavior.Parents);
                Helper.ExecuteOnTarget("SET IDENTITY_INSERT [dbo].[Country] ON; INSERT INTO [dbo].[Country] ([Id], [Name]) VALUES (100, N'Atlantis'); SET IDENTITY_INSERT [dbo].[Country] OFF;");

                // Act
                var actual = source.CopyDataTo("Person", target, relationshipBehavior: CopyRelationshipBehavior.Parents, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Drop);

                // Assert
                Assert.AreEqual(3 + 6, actual);
                Assert.AreEqual(3, Helper.TargetCount("[dbo].[Country]"));
                Assert.AreEqual(0, Helper.TargetCount("[dbo].[Country]", "[Name] = N'Atlantis'"));
                Assert.AreEqual(6, Helper.TargetCount("[dbo].[Person]"));
                Assert.AreEqual(0, Helper.TargetOrphans("[dbo].[Person]", "[CountryId]", "[dbo].[Country]", "[Id]"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithDropCanBeRepeated()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                source.CopyDataTo("Currency", target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Drop);

                // Act
                var actual = source.CopyDataTo("Currency", target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Drop);

                // Assert
                Assert.AreEqual(3, actual);
                Assert.AreEqual(3, Helper.TargetCount("[dbo].[Currency]"));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestCopyDataToAsyncWithDropReplacesTheTableThatExistsAndItsRows()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.CreateTargetTables("Currency");
                Helper.ExecuteOnTarget("INSERT INTO [dbo].[Currency] ([Code], [Name]) VALUES ('USD', N'US Dollar');");

                // Act
                var actual = await source.CopyDataToAsync("Currency", target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Drop);

                // Assert
                Assert.AreEqual(3, actual);
                Assert.AreEqual(0, Helper.TargetCount("[dbo].[Currency]", "[Code] = 'USD'"));
            }
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncWithDropAndParentsReplacesTheTablesThatExist()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                await source.CopyDataToAsync("Person", target, relationshipBehavior: CopyRelationshipBehavior.Parents);

                // Act
                var actual = await source.CopyDataToAsync("Person", target, relationshipBehavior: CopyRelationshipBehavior.Parents, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Drop);

                // Assert
                Assert.AreEqual(3 + 6, actual);
                Assert.AreEqual(6, Helper.TargetCount("[dbo].[Person]"));
            }
        }

        #endregion

        #endregion

        #region Align

        #region Sync

        [TestMethod]
        public void TestCopyDataToWithAlignCreatesTheTableIfItDoesNotExist()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Currency", target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Align);

                // Assert
                Assert.AreEqual(3, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("[dbo].[Currency]", "[Code]"), Helper.TargetRows("[dbo].[Currency]", "[Code]"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithAlignAddsTheMissingColumnsOfTheTableThatExists()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.ExecuteOnTarget("CREATE TABLE [dbo].[Currency] ([Code] CHAR(3) NOT NULL, CONSTRAINT [PK_Currency] PRIMARY KEY ([Code]));");

                // Act
                var actual = source.CopyDataTo("Currency", target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Align);

                // Assert
                Assert.AreEqual(3, actual);
                CollectionAssert.AreEqual(new[] { "Code", "Name" }, TargetColumns("[dbo].[Currency]"));
                Helper.AssertRowsEqual(Helper.SourceRows("[dbo].[Currency]", "[Code]"), Helper.TargetRows("[dbo].[Currency]", "[Code]"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithAlignAddsTheMissingIndexesOfTheTableThatExists()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.CreateTargetTables("Country", "Person");
                Helper.ExecuteOnTarget("DROP INDEX [IX_Person_Name] ON [dbo].[Person];");
                Assert.IsFalse(TargetIndexExists("[dbo].[Person]", "IX_Person_Name"));

                // Act
                var actual = source.CopyDataTo("Person", target, relationshipBehavior: CopyRelationshipBehavior.Parents, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Align);

                // Assert
                Assert.AreEqual(3 + 6, actual);
                Assert.IsTrue(TargetIndexExists("[dbo].[Person]", "IX_Person_Name"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithAlignKeepsTheColumnsOfTheTableThatExistsAndAreNotInTheSourceTable()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.ExecuteOnTarget("CREATE TABLE [dbo].[Currency] ([Code] CHAR(3) NOT NULL, [Name] NVARCHAR(50) NOT NULL, [Extra] INT NULL, CONSTRAINT [PK_Currency] PRIMARY KEY ([Code]));");

                // Act
                var actual = source.CopyDataTo("Currency", target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Align);

                // Assert
                Assert.AreEqual(3, actual);
                CollectionAssert.AreEqual(new[] { "Code", "Name", "Extra" }, TargetColumns("[dbo].[Currency]"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithAlignAndATableThatIsAlreadyAlignedCopiesTheRows()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.CreateTargetTables("Currency");

                // Act
                var actual = source.CopyDataTo("Currency", target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Align);

                // Assert
                Assert.AreEqual(3, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("[dbo].[Currency]", "[Code]"), Helper.TargetRows("[dbo].[Currency]", "[Code]"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithAlignAndParentsAndChildrenAlignsTheWholeTree()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.CreateTargetTables("Country", "Person");

                // Act
                var actual = source.CopyDataTo("Pet", target, relationshipBehavior: CopyRelationshipBehavior.ParentsAndChildren, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Align);

                // Assert
                Assert.AreEqual(3 + 6 + 5 + 5, actual);
                Assert.AreEqual(4, Helper.TargetTables().Count);
                Assert.AreEqual(0, Helper.TargetOrphans("[dbo].[Toy]", "[PetId]", "[dbo].[Pet]", "[Id]"));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestCopyDataToAsyncWithAlignAddsTheMissingColumnsOfTheTableThatExists()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.ExecuteOnTarget("CREATE TABLE [dbo].[Currency] ([Code] CHAR(3) NOT NULL, CONSTRAINT [PK_Currency] PRIMARY KEY ([Code]));");

                // Act
                var actual = await source.CopyDataToAsync("Currency", target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Align);

                // Assert
                Assert.AreEqual(3, actual);
                CollectionAssert.AreEqual(new[] { "Code", "Name" }, TargetColumns("[dbo].[Currency]"));
            }
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncWithAlignCreatesTheTableIfItDoesNotExist()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = await source.CopyDataToAsync("Person", target, relationshipBehavior: CopyRelationshipBehavior.Parents, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Align);

                // Assert
                Assert.AreEqual(3 + 6, actual);
                CollectionAssert.AreEqual(new[] { "[dbo].[Country]", "[dbo].[Person]" }, Helper.TargetTables().ToArray());
            }
        }

        #endregion

        #endregion
    }
}
