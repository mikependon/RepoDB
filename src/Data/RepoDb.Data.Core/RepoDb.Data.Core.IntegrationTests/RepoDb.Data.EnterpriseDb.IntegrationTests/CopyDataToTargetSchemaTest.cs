#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Npgsql;
using System.Linq;
using System.Threading.Tasks;
using RepoDb.Connector.EnterpriseDb;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Data.Enumerations;
using RepoDb.Data.EnterpriseDb.IntegrationTests.Setup;
using RepoDb.Enumerations;
using RepoDb.Schema;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Data.EnterpriseDb.IntegrationTests
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
        public void ThrowExceptionOnCopyDataToWithATargetTableOfAnotherSchemaCreatesTheTableInThatSchemaBecauseTheBulkCopyOfEnterpriseDbCanNotLoadTheTablesOfAnotherSchema()
        {
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
            using (var target = new EDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act/Assert
                Assert.Throws<PostgresException>(() => source.CopyDataTo("Currency", "Sales.Currency", target));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToWithAQuotedTargetTableOfAnotherSchemaBecauseTheBulkCopyOfEnterpriseDbCanNotLoadTheTablesOfAnotherSchema()
        {
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
            using (var target = new EDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act/Assert
                Assert.Throws<PostgresException>(() => source.CopyDataTo("Currency", "\"Sales\".\"Currency\"", target));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithATargetTableOfTheDefaultSchema()
        {
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
            using (var target = new EDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Currency", "public.Currency", target);

                // Assert
                Assert.AreEqual(3, actual);
                CollectionAssert.AreEqual(new[] { "\"public\".\"Currency\"" }, Helper.TargetTables().ToArray());
            }
        }

        [TestMethod]
        public void TestCopyDataToFromATableOfAnotherSchemaIntoTheDefaultSchema()
        {
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
            using (var target = new EDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act
                var actual = source.CopyDataTo("Sales.Invoice", "public.Invoice", target);

                // Assert
                Assert.AreEqual(2, actual);
                Helper.AssertRowsEqual(Helper.SourceRows("\"Sales\".\"Invoice\"", "\"Id\""), Helper.TargetRows("\"public\".\"Invoice\"", "\"Id\""));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToWithATargetTableOfAnotherSchemaAddsTheRowsToTheTableThatExistsBecauseTheBulkCopyOfEnterpriseDbCanNotLoadTheTablesOfAnotherSchema()
        {
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
            using (var target = new EDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);
                Helper.ExecuteOnTarget("CREATE TABLE \"Sales\".\"Currency\" (\"Code\" CHAR(3) NOT NULL, \"Name\" varchar(50) NOT NULL, CONSTRAINT \"PK_Currency\" PRIMARY KEY (\"Code\")); INSERT INTO \"Sales\".\"Currency\" (\"Code\", \"Name\") VALUES ('USD', 'US Dollar');");

                // Act/Assert
                Assert.Throws<PostgresException>(() => source.CopyDataTo("Currency", "Sales.Currency", target));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToWithATargetTableOfAnotherSchemaAndParentsCopiesTheParentsIntoThatSchemaBecauseTheBulkCopyOfEnterpriseDbCanNotLoadTheTablesOfAnotherSchema()
        {
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
            using (var target = new EDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act/Assert
                Assert.Throws<PostgresException>(() => source.CopyDataTo("Person", "Sales.Person", target, relationshipBehavior: CopyDataRelationshipBehavior.Parents));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToWithATargetTableOfAnotherSchemaAndChildrenCopiesTheChildrenIntoThatSchemaBecauseTheBulkCopyOfEnterpriseDbCanNotLoadTheTablesOfAnotherSchema()
        {
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
            using (var target = new EDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act/Assert
                Assert.Throws<PostgresException>(() => source.CopyDataTo("Country", "Sales.Country", target, relationshipBehavior: CopyDataRelationshipBehavior.Children));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToWithATargetTableOfAnotherSchemaAndAFilterFiltersTheRequestedTableOnlyBecauseTheBulkCopyOfEnterpriseDbCanNotLoadTheTablesOfAnotherSchema()
        {
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
            using (var target = new EDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act/Assert
                Assert.Throws<PostgresException>(() => source.CopyDataTo("Person", "Sales.Person", target, where: new QueryGroup(new QueryField("Age", Operation.GreaterThan, 40)), relationshipBehavior: CopyDataRelationshipBehavior.Parents));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToWithATargetTableOfAnotherSchemaAndDropReplacesTheTableThatExistsBecauseTheBulkCopyOfEnterpriseDbCanNotLoadTheTablesOfAnotherSchema()
        {
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
            using (var target = new EDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act/Assert
                Assert.Throws<PostgresException>(() => source.CopyDataTo("Currency", "Sales.Currency", target, tableExistenceBehavior: CopySchemaExistsBehavior.Drop));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheTablesHaveDifferentNamesAndTheTargetTableHasASchema()
        {
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
            using (var target = new EDBConnection(Database.ConnectionStringForTarget))
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
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
            using (var target = new EDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act/Assert
                Assert.ThrowsExactly<EDBException>(() => source.CopyDataTo("Currency", "Missing.Currency", target));
                Assert.AreEqual(0, Helper.TargetTables().Count);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheTableReferencesATableThatDoesNotExistInTheTargetSchema()
        {
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
            using (var target = new EDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act/Assert
                Assert.ThrowsExactly<EDBException>(() => source.CopyDataTo("Person", "Sales.Person", target));
                Assert.AreEqual(0, Helper.TargetCount("\"Sales\".\"Person\""));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheTargetTableOfAnotherSchemaIsCopiedTwiceBecauseTheRowsAlreadyExistBecauseTheBulkCopyOfEnterpriseDbCanNotLoadTheTablesOfAnotherSchema()
        {
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
            using (var target = new EDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act/Assert
                Assert.Throws<PostgresException>(() => source.CopyDataTo("Currency", "Sales.Currency", target));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncWithATargetTableOfAnotherSchemaCreatesTheTableInThatSchemaBecauseTheBulkCopyOfEnterpriseDbCanNotLoadTheTablesOfAnotherSchema()
        {
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
            using (var target = new EDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act/Assert
                await Assert.ThrowsAsync<PostgresException>(() => source.CopyDataToAsync("Currency", "Sales.Currency", target));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncWithATargetTableOfAnotherSchemaAndParentsCopiesTheParentsIntoThatSchemaBecauseTheBulkCopyOfEnterpriseDbCanNotLoadTheTablesOfAnotherSchema()
        {
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
            using (var target = new EDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act/Assert
                await Assert.ThrowsAsync<PostgresException>(() => source.CopyDataToAsync("Person", "Sales.Person", target, relationshipBehavior: CopyDataRelationshipBehavior.Parents));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncIfTheTablesHaveDifferentNamesAndTheTargetTableHasASchema()
        {
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
            using (var target = new EDBConnection(Database.ConnectionStringForTarget))
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
            using (var source = new EDBConnection(Database.ConnectionStringForSource))
            using (var target = new EDBConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.MapSchemaReader(source);

                // Act/Assert
                await Assert.ThrowsExactlyAsync<EDBException>(() => source.CopyDataToAsync("Currency", "Missing.Currency", target));
            }
        }

        #endregion
    }
}
