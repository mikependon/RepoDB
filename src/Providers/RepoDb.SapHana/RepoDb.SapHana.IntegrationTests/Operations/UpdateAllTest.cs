#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sap.Data.Hana;
using RepoDb.Extensions;
using RepoDb.SapHana.IntegrationTests.Models;
using RepoDb.SapHana.IntegrationTests.Setup;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.SapHana.IntegrationTests.Operations
{
    [TestClass]
    public class UpdateAllTest
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
            Database.Cleanup();
        }

        #region DataEntity

        #region Sync

        [TestMethod]
        [Ignore("HANA binds command parameters by position, not by name. UpdateAll (with no explicit " +
            "fields/qualifiers) bundles the primary-key parameter into the same physically-ordered list as " +
            "the SET-column parameters (RepoDb.Core's UpdateAllExecutionContextProvider), but HANA's UPDATE " +
            "syntax forces WHERE - and so the key's placeholder - to appear textually last. No SQL " +
            "restructuring (CTE prefix, updatable subquery) HANA accepts can reorder around that.")]
        public void TestHanaConnectionUpdateAll()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Setup
                tables.AsList().ForEach(table => Helper.UpdateCompleteTableProperties(table));

                // Act
                var result = connection.UpdateAll<CompleteTable>(tables);

                // Assert
                Assert.AreEqual(10, result);

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                tables.AsList().ForEach(table =>
                    Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        [Ignore("See the remark on TestHanaConnectionUpdateAll.")]
        public async Task TestHanaConnectionUpdateAllAsync()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Setup
                tables.AsList().ForEach(table => Helper.UpdateCompleteTableProperties(table));

                // Act
                var result = await connection.UpdateAllAsync<CompleteTable>(tables);

                // Assert
                Assert.AreEqual(10, result);

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                tables.AsList().ForEach(table =>
                    Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        [Ignore("See the remark on TestHanaConnectionUpdateAll.")]
        public void TestHanaConnectionUpdateAllViaTableName()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Setup
                tables.AsList().ForEach(table => Helper.UpdateCompleteTableProperties(table));

                // Act
                var result = connection.UpdateAll(ClassMappedNameCache.Get<CompleteTable>(), tables);

                // Assert
                Assert.AreEqual(10, result);

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                tables.AsList().ForEach(table =>
                    Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        [Ignore("See the remark on TestHanaConnectionUpdateAll.")]
        public void TestHanaConnectionUpdateAllViaTableNameAsExpandoObjects()
        {
            // Setup
            var entities = Database.CreateCompleteTables(10).AsList();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Helper.CreateCompleteTablesAsExpandoObjects(10).AsList();
                tables.ForEach(e => ((IDictionary<string, object>)e)["Id"] = entities[tables.IndexOf(e)].Id);

                // Act
                var result = connection.UpdateAll(ClassMappedNameCache.Get<CompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(10, result);

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                tables.AsList().ForEach(table =>
                    Helper.AssertMembersEquality(queryResult.First(e => e.Id == ((dynamic)table).Id), table));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        [Ignore("See the remark on TestHanaConnectionUpdateAll.")]
        public async Task TestHanaConnectionUpdateAllAsyncViaTableName()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Setup
                tables.AsList().ForEach(table => Helper.UpdateCompleteTableProperties(table));

                // Act
                var result = await connection.UpdateAllAsync(ClassMappedNameCache.Get<CompleteTable>(), tables);

                // Assert
                Assert.AreEqual(10, result);

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                tables.AsList().ForEach(table =>
                    Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        [Ignore("See the remark on TestHanaConnectionUpdateAll.")]
        public async Task TestHanaConnectionUpdateAllAsyncViaTableNameAsExpandoObjects()
        {
            // Setup
            var entities = Database.CreateCompleteTables(10).AsList();

            using (var connection = new HanaConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Helper.CreateCompleteTablesAsExpandoObjects(10).AsList();
                tables.ForEach(e => ((IDictionary<string, object>)e)["Id"] = entities[tables.IndexOf(e)].Id);

                // Act
                var result = await connection.UpdateAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(10, result);

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                tables.AsList().ForEach(table =>
                    Helper.AssertMembersEquality(queryResult.First(e => e.Id == ((dynamic)table).Id), table));
            }
        }

        #endregion

        #endregion
    }
}
