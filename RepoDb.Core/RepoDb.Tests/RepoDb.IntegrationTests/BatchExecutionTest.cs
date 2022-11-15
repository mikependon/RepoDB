using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.IntegrationTests;
using RepoDb.IntegrationTests.Setup;
using System.Threading.Tasks;

namespace RepoDb.SqlServer.IntegrationTests
{
    [TestClass]
    public class BatchExecutionTest
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
        public async Task TestBatchExecutionForInsertAll()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                var hasError = false;
                for (var i = (Constant.DefaultBatchOperationSize * 2); i > 0; i--)
                {
                    try
                    {
                        var identityTables = Helper.CreateIdentityTables(i);
                        connection.InsertAll(identityTables);
                        await connection.InsertAllAsync(identityTables);
                        connection.UpdateAll(identityTables);
                        await connection.UpdateAllAsync(identityTables);
                        connection.MergeAll(identityTables);
                        await connection.MergeAllAsync(identityTables);
                    }
                    catch
                    {
                        hasError = true;
                        break;
                    }
                }
                Assert.IsFalse(hasError);
            }
        }

        [TestMethod]
        public async Task TestBatchExecutionForUpdateAll()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                var hasError = false;
                for (var i = (Constant.DefaultBatchOperationSize + 2); i > 0; i--)
                {
                    try
                    {
                        var identityTables = Helper.CreateIdentityTables(i);
                        connection.InsertAll(identityTables);
                        connection.UpdateAll(identityTables);
                        await connection.UpdateAllAsync(identityTables);
                    }
                    catch
                    {
                        hasError = true;
                        break;
                    }
                }
                Assert.IsFalse(hasError);
            }
        }

        [TestMethod]
        public async Task TestBatchExecutionForMergeAllEmptyTable()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                var hasError = false;
                for (var i = (Constant.DefaultBatchOperationSize * 2); i > 0; i--)
                {
                    try
                    {
                        var identityTables = Helper.CreateIdentityTables(i);
                        connection.MergeAll(identityTables);
                        await connection.MergeAllAsync(identityTables);
                    }
                    catch
                    {
                        hasError = true;
                        break;
                    }
                }
                Assert.IsFalse(hasError);
            }
        }

        [TestMethod]
        public async Task TestBatchExecutionForMergeAllNonEmptyTable()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                var hasError = false;
                for (var i = (Constant.DefaultBatchOperationSize * 2); i > 0; i--)
                {
                    try
                    {
                        var identityTables = Helper.CreateIdentityTables(i);
                        connection.InsertAll(identityTables);
                        connection.MergeAll(identityTables);
                        await connection.MergeAllAsync(identityTables);
                    }
                    catch
                    {
                        hasError = true;
                        break;
                    }
                }
                Assert.IsFalse(hasError);
            }
        }
    }
}
