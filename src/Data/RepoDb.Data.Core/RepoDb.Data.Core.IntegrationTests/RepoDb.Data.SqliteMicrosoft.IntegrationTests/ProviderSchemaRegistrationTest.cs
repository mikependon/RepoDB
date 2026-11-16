using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Sqlite.Microsoft;

namespace RepoDb.Data.SqliteMicrosoft.IntegrationTests
{
    /// <summary>
    /// Tests data copies with automatically registered schema readers.
    /// </summary>
    [TestClass]
    public class ProviderSchemaRegistrationTest
    {
        #region Public Methods

        /// <summary>
        /// Copies schema and data after provider setup alone.
        /// </summary>
        [TestMethod]
        public void CopiesSchemaWithoutManualReaderRegistration()
        {
            GlobalConfiguration.Setup().UseSqliteSchema();
            using var source = CreateSource(7);
            using var destination = CreateConnection();
            source.CopyDataTo("Items",
                destination,
                tableExistenceBehavior: TableExistsBehavior.Align);
            Assert.AreEqual(7L,
                destination.ExecuteScalar<long>("SELECT Id FROM Items"));
        }

        /// <summary>
        /// Copies schema and data asynchronously after provider setup.
        /// </summary>
        [TestMethod]
        public async Task CopiesSchemaWithoutManualReaderRegistrationAsync()
        {
            GlobalConfiguration.Setup().UseSqliteSchema();
            using var source = CreateSource(7);
            using var destination = CreateConnection();
            await source.CopyDataToAsync("Items",
                destination,
                tableExistenceBehavior: TableExistsBehavior.Align);
            Assert.AreEqual(7L,
                destination.ExecuteScalar<long>("SELECT Id FROM Items"));
        }

        /// <summary>
        /// Reuses registration for another database of the same provider.
        /// </summary>
        [TestMethod]
        public void UsesTheCurrentSourceAfterAnEarlierCopy()
        {
            GlobalConfiguration.Setup().UseSqliteSchema();
            using var first = CreateSource(7);
            using var second = CreateSource(9);
            using var firstDestination = CreateConnection();
            using var secondDestination = CreateConnection();
            first.CopyDataTo("Items",
                firstDestination,
                tableExistenceBehavior: TableExistsBehavior.Align);
            second.CopyDataTo("Items",
                secondDestination,
                tableExistenceBehavior: TableExistsBehavior.Align);
            Assert.AreEqual(9L,
                secondDestination.ExecuteScalar<long>("SELECT Id FROM Items"));
        }

        /// <summary>
        /// Keeps the destination transaction separate from source reads.
        /// </summary>
        [TestMethod]
        public void UsesTheDestinationTransaction()
        {
            GlobalConfiguration.Setup().UseSqliteSchema();
            using var source = CreateSource(7);
            using var destination = CreateConnection();
            using var transaction = destination.BeginTransaction();
            source.CopyDataTo("Items",
                destination,
                tableExistenceBehavior: TableExistsBehavior.Align,
                transaction: transaction);
            Assert.AreEqual(7L,
                destination.ExecuteScalar<long>("SELECT Id FROM Items",
                    transaction: transaction));
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates an open in-memory database.
        /// </summary>
        /// <returns>The open connection.</returns>
        private static SqliteConnection CreateConnection()
        {
            var connection = new SqliteConnection("Data Source=:memory:");
            connection.Open();
            return connection;
        }

        /// <summary>
        /// Creates a source table containing one row.
        /// </summary>
        /// <param name="id">The source row identifier.</param>
        /// <returns>The source connection.</returns>
        private static SqliteConnection CreateSource(int id)
        {
            var connection = CreateConnection();
            connection.ExecuteNonQuery($"CREATE TABLE Items (Id integer PRIMARY KEY); INSERT INTO Items VALUES ({id});");
            return connection;
        }

        #endregion
    }
}
