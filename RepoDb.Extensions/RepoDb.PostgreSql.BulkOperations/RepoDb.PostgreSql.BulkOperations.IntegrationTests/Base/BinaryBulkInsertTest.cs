using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.Enumerations.PostgreSql;
using RepoDb.IntegrationTests.Setup;
using RepoDb.PostgreSql.BulkOperations.IntegrationTests.Models;
using System.Linq;

namespace RepoDb.PostgreSql.BulkOperations.IntegrationTests.Base
{
    [TestClass]
    public class BinaryBulkInsertTest
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

        private NpgsqlConnection GetConnection() =>
            (NpgsqlConnection)(new NpgsqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen());

        #region Sync

        #region BinaryImport<TEntity/Anonymous/IDictionary<string, object>>

        [TestMethod]
        public void TestBinaryBulkInsert()
        {
            using (var connection = GetConnection())
            {
                // Prepare
                var entities = Helper.CreateBulkOperationLightIdentityTables(10, true);
                var tableName = "BulkOperationIdentityTable";

                // Act
                var result = NpgsqlConnectionExtension.BinaryBulkInsertBase<BulkOperationLightIdentityTable>(connection,
                    tableName,
                    entities: entities,
                    identityBehavior: BulkImportIdentityBehavior.ReturnIdentity,
                    pseudoTableType: BulkImportPseudoTableType.Physical);

                // Assert
                Assert.AreEqual(entities.Count(), result);

                // Assert
                var queryResult = connection.QueryAll<BulkOperationLightIdentityTable>(tableName).ToList();
                Assert.AreEqual(entities.Count(), queryResult.Count());
                for (var i = 0; i < entities.Count; i++)
                {
                    Helper.AssertPropertiesEquality(entities[i], queryResult[i]);
                }
            }
        }

        #endregion

        #endregion
    }
}
