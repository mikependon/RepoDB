#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Data.Enumerations;
using RepoDb.Data.PostgreSql.IntegrationTests.Setup;
using RepoDb.Exceptions;
using RepoDb.Schema;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Data.PostgreSql.IntegrationTests
{
    /// <summary>
    /// Copies the rows of a table with the different <see cref="TableExistsBehavior"/>: what happens to the tables of the destination
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
            Helper.Read(Database.ConnectionStringForTarget, $"SELECT column_name AS \"name\" FROM information_schema.columns WHERE table_schema || '.' || table_name = '{quotedName.Replace("\"", string.Empty)}' ORDER BY ordinal_position")
                .Select(row => (string)row["name"])
                .ToArray();

        private static bool TargetIndexExists(string quotedName,
            string indexName) =>
            Helper.Read(Database.ConnectionStringForTarget, $"SELECT indexname AS \"name\" FROM pg_indexes WHERE schemaname || '.' || tablename = '{quotedName.Replace("\"", string.Empty)}' AND indexname = '{indexName}'").Count == 1;

        #endregion

        #region Skip

        #region Sync

        [TestMethod]
        public void TestCopyDataToWithSkipCreatesTheMissingTables()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Person", target, relationshipBehavior: CopyRelationshipBehavior.Parents, tableExistenceBehavior: TableExistsBehavior.Skip);

                // Assert
                Assert.AreEqual(3 + 6, actual);
                CollectionAssert.AreEqual(new[] { "\"public\".\"Country\"", "\"public\".\"Person\"" }, Helper.TargetTables().ToArray());
            }
        }

        [TestMethod]
        public void TestCopyDataToWithSkipLeavesTheTablesThatExistAsTheyAre()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.ExecuteOnTarget("CREATE TABLE \"public\".\"Currency\" (\"Code\" CHAR(3) NOT NULL, \"Name\" varchar(50) NOT NULL, \"Extra\" INT NULL, CONSTRAINT \"PK_Currency\" PRIMARY KEY (\"Code\"));");

                // Act
                var actual = source.CopyDataTo("Currency", target, relationshipBehavior: CopyRelationshipBehavior.Parents, tableExistenceBehavior: TableExistsBehavior.Skip);

                // Assert
                Assert.AreEqual(3, actual);
                CollectionAssert.AreEqual(new[] { "Code", "Name", "Extra" }, TargetColumns("\"public\".\"Currency\""));
                Assert.AreEqual(3, Helper.TargetCount("\"public\".\"Currency\""));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithSkipDoesNotAddTheMissingColumnsOfTheTablesThatExist()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.ExecuteOnTarget("CREATE TABLE \"public\".\"Currency\" (\"Code\" CHAR(3) NOT NULL, CONSTRAINT \"PK_Currency\" PRIMARY KEY (\"Code\"));");

                // Act
                var actual = source.CopyDataTo("Currency", target, relationshipBehavior: CopyRelationshipBehavior.Parents, tableExistenceBehavior: TableExistsBehavior.Skip);

                // Assert
                Assert.AreEqual(3, actual);
                CollectionAssert.AreEqual(new[] { "Code" }, TargetColumns("\"public\".\"Currency\""));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToWithSkipAndWithoutASchemaCopyIfTheTableDoesNotExist()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Act/Assert
                Assert.Throws<MissingFieldsException>(() => source.CopyDataTo("Currency", target, tableExistenceBehavior: TableExistsBehavior.Skip));
                Assert.AreEqual(0, Helper.TargetTables().Count);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestCopyDataToAsyncWithSkipCreatesTheMissingTables()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = await source.CopyDataToAsync("Person", target, relationshipBehavior: CopyRelationshipBehavior.Parents, tableExistenceBehavior: TableExistsBehavior.Skip);

                // Assert
                Assert.AreEqual(3 + 6, actual);
                CollectionAssert.AreEqual(new[] { "\"public\".\"Country\"", "\"public\".\"Person\"" }, Helper.TargetTables().ToArray());
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncWithSkipAndWithoutASchemaCopyIfTheTableDoesNotExist()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Act/Assert
                await Assert.ThrowsAsync<MissingFieldsException>(() => source.CopyDataToAsync("Currency", target, tableExistenceBehavior: TableExistsBehavior.Skip));
            }
        }

        #endregion

        #endregion

        #region Throw

        #region Sync

        [TestMethod]
        public void TestCopyDataToWithThrowCreatesTheTableIfItDoesNotExist()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Currency", target, tableExistenceBehavior: TableExistsBehavior.Throw);

                // Assert
                Assert.AreEqual(3, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("\"public\".\"Currency\"", "\"Code\""), Helper.TargetRows("\"public\".\"Currency\"", "\"Code\""));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToWithThrowIfTheTableExists()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.CreateTargetTables("Currency");

                // Act
                var exception = Assert.Throws<InvalidOperationException>(() => source.CopyDataTo("Currency", target, tableExistenceBehavior: TableExistsBehavior.Throw));

                // Assert
                StringAssert.Contains(exception.Message, "Currency");
                StringAssert.Contains(exception.Message, "already exists");
                Assert.AreEqual(0, Helper.TargetCount("\"public\".\"Currency\""));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToWithThrowIfAParentTableExists()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.CreateTargetTables("Country");

                // Act/Assert
                Assert.Throws<InvalidOperationException>(() => source.CopyDataTo("Person", target, relationshipBehavior: CopyRelationshipBehavior.Parents, tableExistenceBehavior: TableExistsBehavior.Throw));
                Assert.IsFalse(Helper.TargetTableExists("Person"));
                Assert.AreEqual(0, Helper.TargetCount("\"public\".\"Country\""));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToWithThrowTheSecondTimeThatTheTableIsCopied()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                source.CopyDataTo("Currency", target, tableExistenceBehavior: TableExistsBehavior.Throw);

                // Act/Assert
                Assert.Throws<InvalidOperationException>(() => source.CopyDataTo("Currency", target, tableExistenceBehavior: TableExistsBehavior.Throw));
                Assert.AreEqual(3, Helper.TargetCount("\"public\".\"Currency\""));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithThrowAndAllTheTablesMissingCopiesTheWholeTree()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Pet", target, relationshipBehavior: CopyRelationshipBehavior.ParentsAndChildren, tableExistenceBehavior: TableExistsBehavior.Throw);

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
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = await source.CopyDataToAsync("Currency", target, tableExistenceBehavior: TableExistsBehavior.Throw);

                // Assert
                Assert.AreEqual(3, actual);
                Assert.AreEqual(3, Helper.TargetCount("\"public\".\"Currency\""));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncWithThrowIfTheTableExists()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.CreateTargetTables("Currency");

                // Act/Assert
                await Assert.ThrowsAsync<InvalidOperationException>(() => source.CopyDataToAsync("Currency", target, tableExistenceBehavior: TableExistsBehavior.Throw));
                Assert.AreEqual(0, Helper.TargetCount("\"public\".\"Currency\""));
            }
        }

        #endregion

        #endregion

        #region Drop

        #region Sync

        [TestMethod]
        public void TestCopyDataToWithDropCreatesTheTableIfItDoesNotExist()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Currency", target, tableExistenceBehavior: TableExistsBehavior.Drop);

                // Assert
                Assert.AreEqual(3, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("\"public\".\"Currency\"", "\"Code\""), Helper.TargetRows("\"public\".\"Currency\"", "\"Code\""));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithDropReplacesTheTableThatExistsAndItsRows()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.CreateTargetTables("Currency");
                Helper.ExecuteOnTarget("INSERT INTO \"public\".\"Currency\" (\"Code\", \"Name\") VALUES ('USD', 'US Dollar');");

                // Act
                var actual = source.CopyDataTo("Currency", target, tableExistenceBehavior: TableExistsBehavior.Drop);

                // Assert
                Assert.AreEqual(3, actual);
                Assert.AreEqual(3, Helper.TargetCount("\"public\".\"Currency\""));
                Assert.AreEqual(0, Helper.TargetCount("\"public\".\"Currency\"", "\"Code\" = 'USD'"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithDropReplacesTheTableThatExistsWithTheStructureOfTheSourceTable()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.ExecuteOnTarget("CREATE TABLE \"public\".\"Currency\" (\"Code\" INT NOT NULL, \"Foo\" varchar(10) NULL);");

                // Act
                var actual = source.CopyDataTo("Currency", target, tableExistenceBehavior: TableExistsBehavior.Drop);

                // Assert
                Assert.AreEqual(3, actual);
                CollectionAssert.AreEqual(new[] { "Code", "Name" }, TargetColumns("\"public\".\"Currency\""));
                Helper.AssertRowsEqual(Helper.SourceRows("\"public\".\"Currency\"", "\"Code\""), Helper.TargetRows("\"public\".\"Currency\"", "\"Code\""));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithDropAndParentsReplacesTheTablesThatExist()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                source.CopyDataTo("Person", target, relationshipBehavior: CopyRelationshipBehavior.Parents);
                Helper.ExecuteOnTarget("INSERT INTO \"public\".\"Country\" (\"Id\", \"Name\") VALUES (100, 'Atlantis');");

                // Act
                var actual = source.CopyDataTo("Person", target, relationshipBehavior: CopyRelationshipBehavior.Parents, tableExistenceBehavior: TableExistsBehavior.Drop);

                // Assert
                Assert.AreEqual(3 + 6, actual);
                Assert.AreEqual(3, Helper.TargetCount("\"public\".\"Country\""));
                Assert.AreEqual(0, Helper.TargetCount("\"public\".\"Country\"", "\"Name\" = 'Atlantis'"));
                Assert.AreEqual(6, Helper.TargetCount("\"public\".\"Person\""));
                Assert.AreEqual(0, Helper.TargetOrphans("\"public\".\"Person\"", "\"CountryId\"", "\"public\".\"Country\"", "\"Id\""));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithDropCanBeRepeated()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                source.CopyDataTo("Currency", target, tableExistenceBehavior: TableExistsBehavior.Drop);

                // Act
                var actual = source.CopyDataTo("Currency", target, tableExistenceBehavior: TableExistsBehavior.Drop);

                // Assert
                Assert.AreEqual(3, actual);
                Assert.AreEqual(3, Helper.TargetCount("\"public\".\"Currency\""));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestCopyDataToAsyncWithDropReplacesTheTableThatExistsAndItsRows()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.CreateTargetTables("Currency");
                Helper.ExecuteOnTarget("INSERT INTO \"public\".\"Currency\" (\"Code\", \"Name\") VALUES ('USD', 'US Dollar');");

                // Act
                var actual = await source.CopyDataToAsync("Currency", target, tableExistenceBehavior: TableExistsBehavior.Drop);

                // Assert
                Assert.AreEqual(3, actual);
                Assert.AreEqual(0, Helper.TargetCount("\"public\".\"Currency\"", "\"Code\" = 'USD'"));
            }
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncWithDropAndParentsReplacesTheTablesThatExist()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                await source.CopyDataToAsync("Person", target, relationshipBehavior: CopyRelationshipBehavior.Parents);

                // Act
                var actual = await source.CopyDataToAsync("Person", target, relationshipBehavior: CopyRelationshipBehavior.Parents, tableExistenceBehavior: TableExistsBehavior.Drop);

                // Assert
                Assert.AreEqual(3 + 6, actual);
                Assert.AreEqual(6, Helper.TargetCount("\"public\".\"Person\""));
            }
        }

        #endregion

        #endregion

        #region Align

        #region Sync

        [TestMethod]
        public void TestCopyDataToWithAlignCreatesTheTableIfItDoesNotExist()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Currency", target, tableExistenceBehavior: TableExistsBehavior.Align);

                // Assert
                Assert.AreEqual(3, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("\"public\".\"Currency\"", "\"Code\""), Helper.TargetRows("\"public\".\"Currency\"", "\"Code\""));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithAlignAddsTheMissingColumnsOfTheTableThatExists()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.ExecuteOnTarget("CREATE TABLE \"public\".\"Currency\" (\"Code\" CHAR(3) NOT NULL, CONSTRAINT \"PK_Currency\" PRIMARY KEY (\"Code\"));");

                // Act
                var actual = source.CopyDataTo("Currency", target, tableExistenceBehavior: TableExistsBehavior.Align);

                // Assert
                Assert.AreEqual(3, actual);
                CollectionAssert.AreEqual(new[] { "Code", "Name" }, TargetColumns("\"public\".\"Currency\""));
                Helper.AssertRowsEqual(Helper.SourceRows("\"public\".\"Currency\"", "\"Code\""), Helper.TargetRows("\"public\".\"Currency\"", "\"Code\""));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithAlignAddsTheMissingIndexesOfTheTableThatExists()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.CreateTargetTables("Country", "Person");
                Helper.ExecuteOnTarget("DROP INDEX \"public\".\"IX_Person_Name\";");
                Assert.IsFalse(TargetIndexExists("\"public\".\"Person\"", "IX_Person_Name"));

                // Act
                var actual = source.CopyDataTo("Person", target, relationshipBehavior: CopyRelationshipBehavior.Parents, tableExistenceBehavior: TableExistsBehavior.Align);

                // Assert
                Assert.AreEqual(3 + 6, actual);
                Assert.IsTrue(TargetIndexExists("\"public\".\"Person\"", "IX_Person_Name"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithAlignKeepsTheColumnsOfTheTableThatExistsAndAreNotInTheSourceTable()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.ExecuteOnTarget("CREATE TABLE \"public\".\"Currency\" (\"Code\" CHAR(3) NOT NULL, \"Name\" varchar(50) NOT NULL, \"Extra\" INT NULL, CONSTRAINT \"PK_Currency\" PRIMARY KEY (\"Code\"));");

                // Act
                var actual = source.CopyDataTo("Currency", target, tableExistenceBehavior: TableExistsBehavior.Align);

                // Assert
                Assert.AreEqual(3, actual);
                CollectionAssert.AreEqual(new[] { "Code", "Name", "Extra" }, TargetColumns("\"public\".\"Currency\""));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithAlignAndATableThatIsAlreadyAlignedCopiesTheRows()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.CreateTargetTables("Currency");

                // Act
                var actual = source.CopyDataTo("Currency", target, tableExistenceBehavior: TableExistsBehavior.Align);

                // Assert
                Assert.AreEqual(3, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("\"public\".\"Currency\"", "\"Code\""), Helper.TargetRows("\"public\".\"Currency\"", "\"Code\""));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithAlignAndParentsAndChildrenAlignsTheWholeTree()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.CreateTargetTables("Country", "Person");

                // Act
                var actual = source.CopyDataTo("Pet", target, relationshipBehavior: CopyRelationshipBehavior.ParentsAndChildren, tableExistenceBehavior: TableExistsBehavior.Align);

                // Assert
                Assert.AreEqual(3 + 6 + 5 + 5, actual);
                Assert.AreEqual(4, Helper.TargetTables().Count);
                Assert.AreEqual(0, Helper.TargetOrphans("\"public\".\"Toy\"", "\"PetId\"", "\"public\".\"Pet\"", "\"Id\""));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestCopyDataToAsyncWithAlignAddsTheMissingColumnsOfTheTableThatExists()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.ExecuteOnTarget("CREATE TABLE \"public\".\"Currency\" (\"Code\" CHAR(3) NOT NULL, CONSTRAINT \"PK_Currency\" PRIMARY KEY (\"Code\"));");

                // Act
                var actual = await source.CopyDataToAsync("Currency", target, tableExistenceBehavior: TableExistsBehavior.Align);

                // Assert
                Assert.AreEqual(3, actual);
                CollectionAssert.AreEqual(new[] { "Code", "Name" }, TargetColumns("\"public\".\"Currency\""));
            }
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncWithAlignCreatesTheTableIfItDoesNotExist()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = await source.CopyDataToAsync("Person", target, relationshipBehavior: CopyRelationshipBehavior.Parents, tableExistenceBehavior: TableExistsBehavior.Align);

                // Assert
                Assert.AreEqual(3 + 6, actual);
                CollectionAssert.AreEqual(new[] { "\"public\".\"Country\"", "\"public\".\"Person\"" }, Helper.TargetTables().ToArray());
            }
        }

        #endregion

        #endregion
    }
}
