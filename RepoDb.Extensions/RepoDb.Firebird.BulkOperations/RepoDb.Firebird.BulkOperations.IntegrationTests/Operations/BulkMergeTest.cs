using Microsoft.VisualStudio.TestTools.UnitTesting;
using FirebirdSql.Data.FirebirdClient;
using RepoDb.Enumerations.Firebird;
using RepoDb.Extensions;
using RepoDb.IntegrationTests.Setup;
using RepoDb.Firebird.BulkOperations.IntegrationTests.Models;
using System.Linq;

namespace RepoDb.Firebird.BulkOperations.IntegrationTests.Operations
{
    [TestClass]
    public class FirebirdConnectionBulkMergeOperationsTest
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

        /// <summary>
        /// The identity column defaults to being the qualifier here - exercises the <c>EXECUTE BLOCK</c>
        /// branch in <c>FirebirdText.GetMergeFromPseudoTableSql</c> that has to distinguish "insert a new
        /// row" (Id null/0) from "match and update this row" (a real Id), since a plain
        /// <c>MATCHING</c>/<c>ON</c> clause can't tell them apart on its own.
        /// </summary>
        [TestMethod]
        public void TestFirebirdConnectionBulkMergeForEntitiesWithIdentityAsQualifier()
        {
            // Setup - insert some rows first, so half the merge batch matches existing rows
            var existing = Helper.CreateBulkOperationIdentityTables(5);
            using var connection = new FbConnection(Database.ConnectionString);
            connection.BulkInsert(existing, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity);

            Helper.UpdateBulkOperationIdentityTables(existing);
            var newRows = Helper.CreateBulkOperationIdentityTables(5);
            var mergeBatch = existing.Concat(newRows).ToList();

            // Act
            var mergeResult = connection.BulkMerge(mergeBatch, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity);

            // Assert
            Assert.AreEqual(mergeBatch.Count, mergeResult);
            Assert.IsFalse(newRows.Any(e => e.Id <= 0));

            // Act
            var queryResult = connection.QueryAll<BulkOperationIdentityTable>();

            // Assert - 5 originally-inserted rows updated in place + 5 newly-inserted rows = 10 total, not 15
            Assert.AreEqual(10, queryResult.Count());
            mergeBatch.ForEach(t =>
            {
                var item = queryResult.First(e => e.Id == t.Id);
                Helper.AssertPropertiesEquality(t, item);
            });
        }

        /// <summary>
        /// The identity column here is not a qualifier - exercises the plain single-statement ANSI
        /// <c>MERGE INTO ... USING ... ON ...</c> path.
        /// </summary>
        [TestMethod]
        public void TestFirebirdConnectionBulkMergeForNonIdentityTableWithExplicitQualifiers()
        {
            // Setup
            var tables = Helper.CreateBulkOperationNonIdentityTables(10);
            using var connection = new FbConnection(Database.ConnectionString);
            connection.BulkInsert(tables);

            Helper.UpdateBulkOperationNonIdentityTables(tables);

            // Act
            var mergeResult = connection.BulkMerge(tables, qualifiers: Field.From(nameof(BulkOperationNonIdentityTable.Id)));

            // Assert
            Assert.AreEqual(tables.Count, mergeResult);

            // Act
            var queryResult = connection.QueryAll<BulkOperationNonIdentityTable>();

            // Assert
            Assert.AreEqual(tables.Count, queryResult.Count());
            tables.ForEach(t =>
            {
                var item = queryResult.First(e => e.Id == t.Id);
                Helper.AssertPropertiesEquality(t, item);
            });
        }

        [TestMethod]
        public async System.Threading.Tasks.Task TestFirebirdConnectionBulkMergeForEntitiesAsyncWithIdentityAsQualifier()
        {
            // Setup
            var existing = Helper.CreateBulkOperationIdentityTables(5);
            using var connection = new FbConnection(Database.ConnectionString);
            await connection.BulkInsertAsync(existing, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity);

            Helper.UpdateBulkOperationIdentityTables(existing);
            var newRows = Helper.CreateBulkOperationIdentityTables(5);
            var mergeBatch = existing.Concat(newRows).ToList();

            // Act
            var mergeResult = await connection.BulkMergeAsync(mergeBatch, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity);

            // Assert
            Assert.AreEqual(mergeBatch.Count, mergeResult);
            Assert.IsFalse(newRows.Any(e => e.Id <= 0));
        }
    }
}
