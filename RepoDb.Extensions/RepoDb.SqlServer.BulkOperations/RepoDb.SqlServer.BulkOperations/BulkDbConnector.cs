using System;
using Microsoft.Data.SqlClient;
using RepoDb.Enumerations;

namespace RepoDb
{
    /// <summary>
    /// A class with the connection for bulk operations.
    /// </summary>
    internal sealed class BulkDbConnector : IDisposable
    {
        private readonly DbRepository<SqlConnection> repository;
        private readonly SqlTransaction transaction;

        /// <summary>
        /// A class with the connection for bulk operations.
        /// </summary>
        /// <param name="transaction">The transaction to be used.</param>
        /// <param name="repository">The instance of <see cref="DbRepository{SqlConnection}"/> object.</param>
        public BulkDbConnector(SqlTransaction transaction, DbRepository<SqlConnection> repository)
        {
            this.transaction = transaction;
            this.repository = repository;

            Connection = transaction?.Connection ?? repository.CreateConnection();
        }

        /// <summary>
        /// Represents a connection to a SQL Server database.
        /// </summary>
        public SqlConnection Connection { get; }

        /// <inheritdoc />
        public void Dispose()
        {
            if (repository.ConnectionPersistency != ConnectionPersistency.PerCall) return;

            if (transaction == null) 
                Connection?.Dispose();
        }
    }
}