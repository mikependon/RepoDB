using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.IntegrationTests;
using RepoDb.IntegrationTests.Setup;

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
        public void TestBatchExecutionForInsertAll()
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
                        connection.InsertAllAsync(identityTables).Wait();
                        connection.UpdateAll(identityTables);
                        connection.UpdateAllAsync(identityTables).Wait();
                        connection.MergeAll(identityTables);
                        connection.MergeAllAsync(identityTables).Wait();
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
        public void TestBatchExecutionForUpdateAll()
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
                        connection.UpdateAllAsync(identityTables).Wait();
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
        public void TestBatchExecutionForMergeAllEmptyTable()
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
                        connection.MergeAllAsync(identityTables).Wait();
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
        public void TestBatchExecutionForMergeAllNonEmptyTable()
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
                        connection.MergeAllAsync(identityTables).Wait();
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
