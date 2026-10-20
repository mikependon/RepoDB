#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Linq;
using System.Threading.Tasks;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Data.Enumerations;
using RepoDb.Data.Firebird.IntegrationTests.Setup;
using RepoDb.Exceptions;
using RepoDb.Schema;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Data.Firebird.IntegrationTests
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
            Helper.Read(Database.ConnectionStringForTarget, $"SELECT TRIM(RDB$FIELD_NAME) AS \"name\" FROM RDB$RELATION_FIELDS WHERE RDB$RELATION_NAME = '{quotedName.Trim('\"')}' ORDER BY RDB$FIELD_POSITION")
                .Select(row => (string)row["name"])
                .ToArray();

        private static bool TargetIndexExists(string quotedName,
            string indexName) =>
            Helper.Read(Database.ConnectionStringForTarget, $"SELECT TRIM(RDB$INDEX_NAME) AS \"index_name\" FROM RDB$INDICES WHERE RDB$RELATION_NAME = '{quotedName.Trim('\"')}' AND RDB$INDEX_NAME = '{indexName}'").Count == 1;

        #endregion

        #region Skip

        #region Sync

        [TestMethod]
        public void TestCopyDataToWithSkipCreatesTheMissingTables()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Person", target, relationshipBehavior: CopyDataRelationshipBehavior.Parents, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Skip);

                // Assert
                Assert.AreEqual(3 + 6, actual);
                CollectionAssert.AreEqual(new[] { "\"Country\"", "\"Person\"" }, Helper.TargetTables().ToArray());
            }
        }

        [TestMethod]
        public void TestCopyDataToWithSkipLeavesTheTablesThatExistAsTheyAre()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.ExecuteOnTarget("CREATE TABLE \"Currency\" (\"Code\" CHAR(3) NOT NULL, \"Name\" VARCHAR(50) NOT NULL, \"Extra\" INTEGER, CONSTRAINT \"PK_Currency\" PRIMARY KEY (\"Code\"));");

                // Act
                var actual = source.CopyDataTo("Currency", target, relationshipBehavior: CopyDataRelationshipBehavior.Parents, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Skip);

                // Assert
                Assert.AreEqual(3, actual);
                CollectionAssert.AreEqual(new[] { "Code", "Name", "Extra" }, TargetColumns("\"Currency\""));
                Assert.AreEqual(3, Helper.TargetCount("\"Currency\""));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithSkipDoesNotAddTheMissingColumnsOfTheTablesThatExist()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.ExecuteOnTarget("CREATE TABLE \"Currency\" (\"Code\" CHAR(3) NOT NULL, CONSTRAINT \"PK_Currency\" PRIMARY KEY (\"Code\"));");

                // Act
                var actual = source.CopyDataTo("Currency", target, relationshipBehavior: CopyDataRelationshipBehavior.Parents, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Skip);

                // Assert
                Assert.AreEqual(3, actual);
                CollectionAssert.AreEqual(new[] { "Code" }, TargetColumns("\"Currency\""));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToWithSkipAndWithoutASchemaCopyIfTheTableDoesNotExist()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
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
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = await source.CopyDataToAsync("Person", target, relationshipBehavior: CopyDataRelationshipBehavior.Parents, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Skip);

                // Assert
                Assert.AreEqual(3 + 6, actual);
                CollectionAssert.AreEqual(new[] { "\"Country\"", "\"Person\"" }, Helper.TargetTables().ToArray());
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncWithSkipAndWithoutASchemaCopyIfTheTableDoesNotExist()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
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
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Currency", target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Throw);

                // Assert
                Assert.AreEqual(3, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("\"Currency\"", "\"Code\""), Helper.TargetRows("\"Currency\"", "\"Code\""));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToWithThrowIfTheTableExists()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.CreateTargetTables("Currency");

                // Act
                var exception = Assert.Throws<InvalidOperationException>(() => source.CopyDataTo("Currency", target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Throw));

                // Assert
                StringAssert.Contains(exception.Message, "Currency");
                StringAssert.Contains(exception.Message, "already exists");
                Assert.AreEqual(0, Helper.TargetCount("\"Currency\""));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToWithThrowIfAParentTableExists()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.CreateTargetTables("Country");

                // Act/Assert
                Assert.Throws<InvalidOperationException>(() => source.CopyDataTo("Person", target, relationshipBehavior: CopyDataRelationshipBehavior.Parents, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Throw));
                Assert.IsFalse(Helper.TargetTableExists("Person"));
                Assert.AreEqual(0, Helper.TargetCount("\"Country\""));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToWithThrowTheSecondTimeThatTheTableIsCopied()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                source.CopyDataTo("Currency", target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Throw);

                // Act/Assert
                Assert.Throws<InvalidOperationException>(() => source.CopyDataTo("Currency", target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Throw));
                Assert.AreEqual(3, Helper.TargetCount("\"Currency\""));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithThrowAndAllTheTablesMissingCopiesTheWholeTree()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Pet", target, relationshipBehavior: CopyDataRelationshipBehavior.ParentsAndChildren, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Throw);

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
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = await source.CopyDataToAsync("Currency", target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Throw);

                // Assert
                Assert.AreEqual(3, actual);
                Assert.AreEqual(3, Helper.TargetCount("\"Currency\""));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncWithThrowIfTheTableExists()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.CreateTargetTables("Currency");

                // Act/Assert
                await Assert.ThrowsAsync<InvalidOperationException>(() => source.CopyDataToAsync("Currency", target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Throw));
                Assert.AreEqual(0, Helper.TargetCount("\"Currency\""));
            }
        }

        #endregion

        #endregion

        #region Drop

        #region Sync

        [TestMethod]
        public void TestCopyDataToWithDropCreatesTheTableIfItDoesNotExist()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Currency", target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Drop);

                // Assert
                Assert.AreEqual(3, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("\"Currency\"", "\"Code\""), Helper.TargetRows("\"Currency\"", "\"Code\""));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithDropReplacesTheTableThatExistsAndItsRows()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.CreateTargetTables("Currency");
                Helper.ExecuteOnTarget("INSERT INTO \"Currency\" (\"Code\", \"Name\") VALUES ('USD', 'US Dollar');");

                // Act
                var actual = source.CopyDataTo("Currency", target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Drop);

                // Assert
                Assert.AreEqual(3, actual);
                Assert.AreEqual(3, Helper.TargetCount("\"Currency\""));
                Assert.AreEqual(0, Helper.TargetCount("\"Currency\"", "\"Code\" = 'USD'"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithDropReplacesTheTableThatExistsWithTheStructureOfTheSourceTable()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.ExecuteOnTarget("CREATE TABLE \"Currency\" (\"Code\" INTEGER NOT NULL, \"Foo\" VARCHAR(10));");

                // Act
                var actual = source.CopyDataTo("Currency", target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Drop);

                // Assert
                Assert.AreEqual(3, actual);
                CollectionAssert.AreEqual(new[] { "Code", "Name" }, TargetColumns("\"Currency\""));
                Helper.AssertRowsEqual(Helper.SourceRows("\"Currency\"", "\"Code\""), Helper.TargetRows("\"Currency\"", "\"Code\""));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithDropAndParentsReplacesTheTablesThatExist()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                source.CopyDataTo("Person", target, relationshipBehavior: CopyDataRelationshipBehavior.Parents);
                Helper.ExecuteOnTarget("INSERT INTO \"Country\" (\"Id\", \"Name\") VALUES (100, 'Atlantis');");

                // Act
                var actual = source.CopyDataTo("Person", target, relationshipBehavior: CopyDataRelationshipBehavior.Parents, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Drop);

                // Assert
                Assert.AreEqual(3 + 6, actual);
                Assert.AreEqual(3, Helper.TargetCount("\"Country\""));
                Assert.AreEqual(0, Helper.TargetCount("\"Country\"", "\"Name\" = 'Atlantis'"));
                Assert.AreEqual(6, Helper.TargetCount("\"Person\""));
                Assert.AreEqual(0, Helper.TargetOrphans("\"Person\"", "\"CountryId\"", "\"Country\"", "\"Id\""));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithDropCanBeRepeated()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                source.CopyDataTo("Currency", target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Drop);

                // Act
                var actual = source.CopyDataTo("Currency", target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Drop);

                // Assert
                Assert.AreEqual(3, actual);
                Assert.AreEqual(3, Helper.TargetCount("\"Currency\""));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestCopyDataToAsyncWithDropReplacesTheTableThatExistsAndItsRows()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.CreateTargetTables("Currency");
                Helper.ExecuteOnTarget("INSERT INTO \"Currency\" (\"Code\", \"Name\") VALUES ('USD', 'US Dollar');");

                // Act
                var actual = await source.CopyDataToAsync("Currency", target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Drop);

                // Assert
                Assert.AreEqual(3, actual);
                Assert.AreEqual(0, Helper.TargetCount("\"Currency\"", "\"Code\" = 'USD'"));
            }
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncWithDropAndParentsReplacesTheTablesThatExist()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                await source.CopyDataToAsync("Person", target, relationshipBehavior: CopyDataRelationshipBehavior.Parents);

                // Act
                var actual = await source.CopyDataToAsync("Person", target, relationshipBehavior: CopyDataRelationshipBehavior.Parents, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Drop);

                // Assert
                Assert.AreEqual(3 + 6, actual);
                Assert.AreEqual(6, Helper.TargetCount("\"Person\""));
            }
        }

        #endregion

        #endregion

        #region Align

        #region Sync

        [TestMethod]
        public void TestCopyDataToWithAlignCreatesTheTableIfItDoesNotExist()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Currency", target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Align);

                // Assert
                Assert.AreEqual(3, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("\"Currency\"", "\"Code\""), Helper.TargetRows("\"Currency\"", "\"Code\""));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithAlignAddsTheMissingColumnsOfTheTableThatExists()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.ExecuteOnTarget("CREATE TABLE \"Currency\" (\"Code\" CHAR(3) NOT NULL, CONSTRAINT \"PK_Currency\" PRIMARY KEY (\"Code\"));");

                // Act
                var actual = source.CopyDataTo("Currency", target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Align);

                // Assert
                Assert.AreEqual(3, actual);
                CollectionAssert.AreEqual(new[] { "Code", "Name" }, TargetColumns("\"Currency\""));
                Helper.AssertRowsEqual(Helper.SourceRows("\"Currency\"", "\"Code\""), Helper.TargetRows("\"Currency\"", "\"Code\""));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithAlignAddsTheMissingIndexesOfTheTableThatExists()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.CreateTargetTables("Country", "Person");
                Helper.ExecuteOnTarget("DROP INDEX \"IX_Person_Name\";");
                Assert.IsFalse(TargetIndexExists("\"Person\"", "IX_Person_Name"));

                // Act
                var actual = source.CopyDataTo("Person", target, relationshipBehavior: CopyDataRelationshipBehavior.Parents, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Align);

                // Assert
                Assert.AreEqual(3 + 6, actual);
                Assert.IsTrue(TargetIndexExists("\"Person\"", "IX_Person_Name"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithAlignKeepsTheColumnsOfTheTableThatExistsAndAreNotInTheSourceTable()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.ExecuteOnTarget("CREATE TABLE \"Currency\" (\"Code\" CHAR(3) NOT NULL, \"Name\" VARCHAR(50) NOT NULL, \"Extra\" INTEGER, CONSTRAINT \"PK_Currency\" PRIMARY KEY (\"Code\"));");

                // Act
                var actual = source.CopyDataTo("Currency", target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Align);

                // Assert
                Assert.AreEqual(3, actual);
                CollectionAssert.AreEqual(new[] { "Code", "Name", "Extra" }, TargetColumns("\"Currency\""));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithAlignAndATableThatIsAlreadyAlignedCopiesTheRows()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.CreateTargetTables("Currency");

                // Act
                var actual = source.CopyDataTo("Currency", target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Align);

                // Assert
                Assert.AreEqual(3, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("\"Currency\"", "\"Code\""), Helper.TargetRows("\"Currency\"", "\"Code\""));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithAlignAndParentsAndChildrenAlignsTheWholeTree()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.CreateTargetTables("Country", "Person");

                // Act
                var actual = source.CopyDataTo("Pet", target, relationshipBehavior: CopyDataRelationshipBehavior.ParentsAndChildren, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Align);

                // Assert
                Assert.AreEqual(3 + 6 + 5 + 5, actual);
                Assert.AreEqual(4, Helper.TargetTables().Count);
                Assert.AreEqual(0, Helper.TargetOrphans("\"Toy\"", "\"PetId\"", "\"Pet\"", "\"Id\""));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestCopyDataToAsyncWithAlignAddsTheMissingColumnsOfTheTableThatExists()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.ExecuteOnTarget("CREATE TABLE \"Currency\" (\"Code\" CHAR(3) NOT NULL, CONSTRAINT \"PK_Currency\" PRIMARY KEY (\"Code\"));");

                // Act
                var actual = await source.CopyDataToAsync("Currency", target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Align);

                // Assert
                Assert.AreEqual(3, actual);
                CollectionAssert.AreEqual(new[] { "Code", "Name" }, TargetColumns("\"Currency\""));
            }
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncWithAlignCreatesTheTableIfItDoesNotExist()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = await source.CopyDataToAsync("Person", target, relationshipBehavior: CopyDataRelationshipBehavior.Parents, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Align);

                // Assert
                Assert.AreEqual(3 + 6, actual);
                CollectionAssert.AreEqual(new[] { "\"Country\"", "\"Person\"" }, Helper.TargetTables().ToArray());
            }
        }

        #endregion

        #endregion
    }
}
