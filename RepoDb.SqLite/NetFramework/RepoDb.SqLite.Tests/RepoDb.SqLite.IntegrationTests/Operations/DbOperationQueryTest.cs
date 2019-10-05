using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.SqLite.IntegrationTests.Models;
using System.Data.SQLite;
using System.Linq;

namespace RepoDb.SqLite.IntegrationTests.Operations
{
    [TestClass]
    public class DbOperationQueryTest
    {
        [TestInitialize]
        public void Initialize()
        {
            RepoDb.SqLite.Initializer.Initialize();
            //Database.Initialize();
            Cleanup();
        }

        [TestCleanup]
        public void Cleanup()
        {
            //Database.Cleanup();
        }

        [TestMethod]
        public void TestDbRepositoryQuery()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10);

            using (var connection = new SQLiteConnection(@"Data Source=C:\SqLite\Databases\RepoDb.db;Version=3;"))
            {
                // Act
                //repository.InsertAll(tables);

                // Act
                var result = connection.QueryAll<CompleteTable>();

                // Assert
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result.FirstOrDefault(t => t.Id == table.Id));
                });
            }
        }
    }
}
