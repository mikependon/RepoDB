using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.SqlServer.IntegrationTests.Models;
using RepoDb.SqlServer.IntegrationTests.Setup;
using System.Collections.Generic;
using System.Linq;

namespace RepoDb.SqlServer.IntegrationTests.Operations
{
    /// <summary>
    /// Regression coverage for the SQL Server error:
    /// "The target table '...' of the DML statement cannot have any enabled triggers if the
    /// statement contains an OUTPUT clause without INTO clause."
    /// InsertAll/Merge/MergeAll used to build an OUTPUT clause without INTO to read back
    /// identity/primary values, which SQL Server rejects on any table that has an enabled
    /// trigger. These tests run those operations against a table that has an enabled
    /// AFTER INSERT trigger to prove the fix (OUTPUT ... INTO a table variable) works.
    /// </summary>
    [TestClass]
    public class TriggerCompatibilityTest
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
        public void TestSqlConnectionInsertAllAgainstTableWithEnabledTrigger()
        {
            // Setup
            var entities = Enumerable.Range(1, 5)
                .Select(i => new TriggerCompatibilityTable { Name = $"Name_{i}" })
                .ToList();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.InsertAll<TriggerCompatibilityTable>(entities);

                // Assert
                Assert.AreEqual(entities.Count, result);
                Assert.IsTrue(entities.All(e => e.Id > 0));
                Assert.AreEqual(entities.Count, connection.CountAll<TriggerCompatibilityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAgainstTableWithEnabledTrigger()
        {
            // Setup
            var entity = new TriggerCompatibilityTable { Name = "Name" };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Merge<TriggerCompatibilityTable>(entity, qualifiers: Field.From("Id"));

                // Assert
                Assert.IsTrue(entity.Id > 0);
                Assert.AreEqual(entity.Id, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllAgainstTableWithEnabledTrigger()
        {
            // Setup
            var entities = Enumerable.Range(1, 5)
                .Select(i => new TriggerCompatibilityTable { Name = $"Name_{i}" })
                .ToList();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.MergeAll<TriggerCompatibilityTable>(entities, qualifiers: Field.From("Id"));

                // Assert
                Assert.AreEqual(entities.Count, result);
                Assert.IsTrue(entities.All(e => e.Id > 0));
                Assert.AreEqual(entities.Count, connection.CountAll<TriggerCompatibilityTable>());
            }
        }
    }
}
