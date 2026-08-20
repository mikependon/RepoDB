using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.ClickHouse.IntegrationTests.Models;
using RepoDb.ClickHouse.IntegrationTests.Setup;

namespace RepoDb.ClickHouse.IntegrationTests
{
    /// <summary>
    /// ClickHouse has no client-side transactions: no multi-statement atomicity, isolation, or rollback.
    /// <see cref="ClickHouseConnection"/> returns a no-op <see cref="NoOpClickHouseTransaction"/> from
    /// BeginTransaction() rather than throwing, because RepoDb.Core's batch operations (InsertAll,
    /// UpdateAll, MergeAll, DeleteAll, ...) always open an implicit transaction internally for their own
    /// bookkeeping when the caller does not supply one - a hard-throwing BeginTransaction() would break
    /// those operations entirely, not just explicit transaction usage. These tests document the resulting
    /// behavior: Commit()/Rollback() both succeed without error, but Rollback() does not undo statements
    /// that already executed against the server.
    /// </summary>
    [TestClass]
    public class TransactionTests
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

        [TestMethod]
        public void TestClickHouseConnectionBeginTransactionCommitDoesNotThrow()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    connection.Insert<CompleteTable>(Helper.CreateCompleteTables(1).First(), transaction: transaction);
                    transaction.Commit();
                }

                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionRollbackDoesNotUndoAlreadyExecutedStatements()
        {
            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    connection.Insert<CompleteTable>(Helper.CreateCompleteTables(1).First(), transaction: transaction);

                    // Rollback does not throw, but - unlike a real ACID database - it also does not
                    // undo the insert above: ClickHouse has no concept of an uncommitted statement to
                    // discard, so the row remains.
                    transaction.Rollback();
                }

                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
            }
        }
    }
}
