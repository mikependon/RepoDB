#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Data.Enumerations;
using RepoDb.Data.MySql.IntegrationTests.Setup;
using RepoDb.Enumerations;
using RepoDb.Schema;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Data.MySql.IntegrationTests
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
            using (var source = new MySqlConnection(Database.ConnectionStringForSource))
            using (var target = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Currency", "repodb_data_target_sales.Currency", target);

                // Assert
                Assert.AreEqual(3, actual);
                CollectionAssert.AreEqual(new[] { "`Sales`.`Currency`" }, Helper.TargetTables().ToArray());
                Helper.AssertRowsEqual(Helper.SourceRows("`Currency`", "`Code`"), Helper.TargetRows("`Sales`.`Currency`", "`Code`"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithAQuotedTargetTableOfAnotherSchema()
        {
            using (var source = new MySqlConnection(Database.ConnectionStringForSource))
            using (var target = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Currency", "`repodb_data_target_sales`.`Currency`", target);

                // Assert
                Assert.AreEqual(3, actual);
                Assert.AreEqual(3, Helper.TargetCount("`Sales`.`Currency`"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithATargetTableOfTheDefaultSchema()
        {
            using (var source = new MySqlConnection(Database.ConnectionStringForSource))
            using (var target = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Currency", "repodb_data_target.Currency", target);

                // Assert
                Assert.AreEqual(3, actual);
                CollectionAssert.AreEqual(new[] { "`Currency`" }, Helper.TargetTables().ToArray());
            }
        }

        [TestMethod]
        public void TestCopyDataToFromATableOfAnotherSchemaIntoTheDefaultSchema()
        {
            using (var source = new MySqlConnection(Database.ConnectionStringForSource))
            using (var target = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("repodb_data_source_sales.Invoice", "repodb_data_target.Invoice", target);

                // Assert
                Assert.AreEqual(2, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("`Sales`.`Invoice`", "`Id`"), Helper.TargetRows("`Invoice`", "`Id`"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithATargetTableOfAnotherSchemaAddsTheRowsToTheTableThatExists()
        {
            using (var source = new MySqlConnection(Database.ConnectionStringForSource))
            using (var target = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.ExecuteOnTarget("CREATE TABLE `Sales`.`Currency` (`Code` CHAR(3) NOT NULL, `Name` VARCHAR(50) NOT NULL, CONSTRAINT `PK_Currency` PRIMARY KEY (`Code`)); INSERT INTO `Sales`.`Currency` (`Code`, `Name`) VALUES ('USD', 'US Dollar');");

                // Act
                var actual = source.CopyDataTo("Currency", "repodb_data_target_sales.Currency", target);

                // Assert
                Assert.AreEqual(3, actual);
                Assert.AreEqual(4, Helper.TargetCount("`Sales`.`Currency`"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithATargetTableOfAnotherSchemaAndParentsCopiesTheParentsIntoThatSchema()
        {
            using (var source = new MySqlConnection(Database.ConnectionStringForSource))
            using (var target = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Person", "repodb_data_target_sales.Person", target, relationshipBehavior: CopyRelationshipBehavior.Parents);

                // Assert
                Assert.AreEqual(3 + 6, actual);
                CollectionAssert.AreEqual(new[] { "`Sales`.`Country`", "`Sales`.`Person`" }, Helper.TargetTables().ToArray());
                Helper.AssertRowsEqual(Helper.SourceRows("`Country`", "`Id`"), Helper.TargetRows("`Sales`.`Country`", "`Id`"));
                Helper.AssertRowsEqual(Helper.SourceRows("`Person`", "`Id`"), Helper.TargetRows("`Sales`.`Person`", "`Id`"));
                Assert.AreEqual(0, Helper.TargetOrphans("`Sales`.`Person`", "`CountryId`", "`Sales`.`Country`", "`Id`"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithATargetTableOfAnotherSchemaAndChildrenCopiesTheChildrenIntoThatSchema()
        {
            using (var source = new MySqlConnection(Database.ConnectionStringForSource))
            using (var target = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Country", "repodb_data_target_sales.Country", target, relationshipBehavior: CopyRelationshipBehavior.Children);

                // Assert
                Assert.AreEqual(3 + 6 + 5 + 5, actual);
                CollectionAssert.AreEqual(new[] { "`Sales`.`Country`", "`Sales`.`Person`", "`Sales`.`Pet`", "`Sales`.`Toy`" }, Helper.TargetTables().ToArray());
                Assert.AreEqual(0, Helper.TargetOrphans("`Sales`.`Toy`", "`PetId`", "`Sales`.`Pet`", "`Id`"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithATargetTableOfAnotherSchemaAndAFilterFiltersTheRequestedTableOnly()
        {
            using (var source = new MySqlConnection(Database.ConnectionStringForSource))
            using (var target = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Person", "repodb_data_target_sales.Person", target, where: new QueryGroup(new QueryField("Age", Operation.GreaterThan, 40)), relationshipBehavior: CopyRelationshipBehavior.Parents);

                // Assert
                Assert.AreEqual(3 + 2, actual);
                Assert.AreEqual(2, Helper.TargetCount("`Sales`.`Person`"));
                Assert.AreEqual(3, Helper.TargetCount("`Sales`.`Country`"));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithATargetTableOfAnotherSchemaAndDropReplacesTheTableThatExists()
        {
            using (var source = new MySqlConnection(Database.ConnectionStringForSource))
            using (var target = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                source.CopyDataTo("Currency", "repodb_data_target_sales.Currency", target);

                // Act
                var actual = source.CopyDataTo("Currency", "repodb_data_target_sales.Currency", target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Drop);

                // Assert
                Assert.AreEqual(3, actual);
                Assert.AreEqual(3, Helper.TargetCount("`Sales`.`Currency`"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheTablesHaveDifferentNamesAndTheTargetTableHasASchema()
        {
            using (var source = new MySqlConnection(Database.ConnectionStringForSource))
            using (var target = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act/Assert
                Assert.Throws<ArgumentException>(() => source.CopyDataTo("Currency", "repodb_data_target_sales.Money", target));
                Assert.AreEqual(0, Helper.TargetTables().Count);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheTargetSchemaDoesNotExist()
        {
            using (var source = new MySqlConnection(Database.ConnectionStringForSource))
            using (var target = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act/Assert
                Assert.ThrowsExactly<MySqlException>(() => source.CopyDataTo("Currency", "Missing.Currency", target));
                Assert.AreEqual(0, Helper.TargetTables().Count);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheTableReferencesATableThatDoesNotExistInTheTargetSchema()
        {
            using (var source = new MySqlConnection(Database.ConnectionStringForSource))
            using (var target = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act/Assert
                Assert.ThrowsExactly<MySqlException>(() => source.CopyDataTo("Person", "repodb_data_target_sales.Person", target));
                Assert.AreEqual(0, Helper.TargetCount("`Sales`.`Person`"));
            }
        }

        [TestMethod]
        public void TestCopyDataToIgnoresTheRowsThatAlreadyExistInTheTargetTableOfAnotherSchema()
        {
            using (var source = new MySqlConnection(Database.ConnectionStringForSource))
            using (var target = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                source.CopyDataTo("Currency", "repodb_data_target_sales.Currency", target);

                // Act (the bulk load of MySQL is a LOAD DATA LOCAL, that skips the rows that violate a key)
                source.CopyDataTo("Currency", "repodb_data_target_sales.Currency", target);

                // Assert
                Assert.AreEqual(3, Helper.TargetCount("`Sales`.`Currency`"));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestCopyDataToAsyncWithATargetTableOfAnotherSchemaCreatesTheTableInThatSchema()
        {
            using (var source = new MySqlConnection(Database.ConnectionStringForSource))
            using (var target = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = await source.CopyDataToAsync("Currency", "repodb_data_target_sales.Currency", target);

                // Assert
                Assert.AreEqual(3, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("`Currency`", "`Code`"), Helper.TargetRows("`Sales`.`Currency`", "`Code`"));
            }
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncWithATargetTableOfAnotherSchemaAndParentsCopiesTheParentsIntoThatSchema()
        {
            using (var source = new MySqlConnection(Database.ConnectionStringForSource))
            using (var target = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = await source.CopyDataToAsync("Person", "repodb_data_target_sales.Person", target, relationshipBehavior: CopyRelationshipBehavior.Parents);

                // Assert
                Assert.AreEqual(3 + 6, actual);
                CollectionAssert.AreEqual(new[] { "`Sales`.`Country`", "`Sales`.`Person`" }, Helper.TargetTables().ToArray());
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncIfTheTablesHaveDifferentNamesAndTheTargetTableHasASchema()
        {
            using (var source = new MySqlConnection(Database.ConnectionStringForSource))
            using (var target = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act/Assert
                await Assert.ThrowsAsync<ArgumentException>(() => source.CopyDataToAsync("Currency", "repodb_data_target_sales.Money", target));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncIfTheTargetSchemaDoesNotExist()
        {
            using (var source = new MySqlConnection(Database.ConnectionStringForSource))
            using (var target = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act/Assert
                await Assert.ThrowsExactlyAsync<MySqlException>(() => source.CopyDataToAsync("Currency", "Missing.Currency", target));
            }
        }

        #endregion
    }
}
