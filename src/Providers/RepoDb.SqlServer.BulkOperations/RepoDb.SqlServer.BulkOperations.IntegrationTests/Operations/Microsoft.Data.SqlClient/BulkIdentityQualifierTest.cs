#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.IntegrationTests.Setup;
using RepoDb.SqlServer.BulkOperations.IntegrationTests.Models;

namespace RepoDb.SqlServer.BulkOperations.IntegrationTests.Operations
{
    /// <summary>
    /// Regression tests for #1351. The staging table must keep the caller's identity values, even when
    /// <see cref="SqlBulkCopyOptions.KeepIdentity"/> is not passed, so that operations qualified by the
    /// identity key only touch the intended rows. Each test uses a subset of rows in reverse order, so the
    /// staged ids never line up with the target ids by coincidence.
    /// </summary>
    [TestClass]
    public class MicrosoftSqlConnectionBulkIdentityQualifierTest
    {
        private const int RowCount = 10;
        private const int SubsetCount = 3;

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

        #region Helpers

        private static List<BulkOperationIdentityTable> InsertRows(SqlConnection connection)
        {
            var tables = Helper.CreateBulkOperationIdentityTables(RowCount);
            connection.InsertAll(tables);
            return connection.QueryAll<BulkOperationIdentityTable>().OrderBy(e => e.Id).ToList();
        }

        private static List<BulkOperationIdentityTable> GetLastRowsReversed(List<BulkOperationIdentityTable> rows) =>
            rows.Skip(RowCount - SubsetCount).Reverse().ToList();

        private static void AssertUpdatedOnlySubset(SqlConnection connection,
            List<BulkOperationIdentityTable> original,
            List<BulkOperationIdentityTable> subset)
        {
            var queryResult = connection.QueryAll<BulkOperationIdentityTable>().OrderBy(e => e.Id).ToList();

            Assert.AreEqual(RowCount, queryResult.Count);
            foreach (var row in queryResult)
            {
                var expected = subset.FirstOrDefault(e => e.Id == row.Id) ?? original.First(e => e.Id == row.Id);
                Helper.AssertPropertiesEquality(expected, row);
            }
        }

        #endregion

        #region BulkUpdate

        [TestMethod]
        public void TestMicrosoftSqlConnectionBulkUpdateForSubsetOfIdentityRowsInReverseOrder()
        {
            using var connection = new SqlConnection(Database.ConnectionString);

            // Setup
            var original = InsertRows(connection);
            var subset = GetLastRowsReversed(connection.QueryAll<BulkOperationIdentityTable>().OrderBy(e => e.Id).ToList());
            Helper.UpdateBulkOperationIdentityTables(subset);

            // Act
            var result = connection.BulkUpdate(subset);

            // Assert
            Assert.AreEqual(SubsetCount, result);
            AssertUpdatedOnlySubset(connection, original, subset);
        }

        [TestMethod]
        public async Task TestMicrosoftSqlConnectionBulkUpdateAsyncForSubsetOfIdentityRowsInReverseOrder()
        {
            using var connection = new SqlConnection(Database.ConnectionString);

            // Setup
            var original = InsertRows(connection);
            var subset = GetLastRowsReversed(connection.QueryAll<BulkOperationIdentityTable>().OrderBy(e => e.Id).ToList());
            Helper.UpdateBulkOperationIdentityTables(subset);

            // Act
            var result = await connection.BulkUpdateAsync(subset);

            // Assert
            Assert.AreEqual(SubsetCount, result);
            AssertUpdatedOnlySubset(connection, original, subset);
        }

        #endregion

        #region BulkDelete

        [TestMethod]
        public void TestMicrosoftSqlConnectionBulkDeleteForSubsetOfIdentityRowsInReverseOrder()
        {
            using var connection = new SqlConnection(Database.ConnectionString);

            // Setup
            var original = InsertRows(connection);
            var subset = GetLastRowsReversed(original);

            // Act
            var result = connection.BulkDelete(subset);

            // Assert
            Assert.AreEqual(SubsetCount, result);
            var remainingIds = connection.QueryAll<BulkOperationIdentityTable>().Select(e => e.Id).OrderBy(id => id).ToList();
            CollectionAssert.AreEqual(original.Take(RowCount - SubsetCount).Select(e => e.Id).ToList(), remainingIds);
        }

        [TestMethod]
        public async Task TestMicrosoftSqlConnectionBulkDeleteAsyncForSubsetOfIdentityRowsInReverseOrder()
        {
            using var connection = new SqlConnection(Database.ConnectionString);

            // Setup
            var original = InsertRows(connection);
            var subset = GetLastRowsReversed(original);

            // Act
            var result = await connection.BulkDeleteAsync(subset);

            // Assert
            Assert.AreEqual(SubsetCount, result);
            var remainingIds = connection.QueryAll<BulkOperationIdentityTable>().Select(e => e.Id).OrderBy(id => id).ToList();
            CollectionAssert.AreEqual(original.Take(RowCount - SubsetCount).Select(e => e.Id).ToList(), remainingIds);
        }

        #endregion

        #region BulkMerge

        [TestMethod]
        public void TestMicrosoftSqlConnectionBulkMergeForSubsetOfIdentityRowsInReverseOrder()
        {
            using var connection = new SqlConnection(Database.ConnectionString);

            // Setup
            var original = InsertRows(connection);
            var subset = GetLastRowsReversed(connection.QueryAll<BulkOperationIdentityTable>().OrderBy(e => e.Id).ToList());
            Helper.UpdateBulkOperationIdentityTables(subset);
            var newRows = Helper.CreateBulkOperationIdentityTables(2);

            // Act
            var result = connection.BulkMerge(subset.Concat(newRows).ToList());

            // Assert
            Assert.AreEqual(SubsetCount + newRows.Count, result);
            var queryResult = connection.QueryAll<BulkOperationIdentityTable>().OrderBy(e => e.Id).ToList();
            Assert.AreEqual(RowCount + newRows.Count, queryResult.Count);
            foreach (var row in queryResult.Take(RowCount))
            {
                var expected = subset.FirstOrDefault(e => e.Id == row.Id) ?? original.First(e => e.Id == row.Id);
                Helper.AssertPropertiesEquality(expected, row);
            }
        }

        #endregion
    }
}
