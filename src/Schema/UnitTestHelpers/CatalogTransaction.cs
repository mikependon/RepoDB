using System.Data;
using System.Data.Common;

namespace RepoDb.Schema.UnitTestHelpers
{
    /// <summary>
    /// Provides a transaction for catalog reader tests.
    /// </summary>
    public sealed class CatalogTransaction : DbTransaction
    {
        #region Constructors

        /// <summary>
        /// Creates a catalog transaction.
        /// </summary>
        /// <param name="connection">The owning connection.</param>
        /// <param name="isolationLevel">The isolation level.</param>
        public CatalogTransaction(DbConnection connection,
            IsolationLevel isolationLevel)
        {
            DbConnection = connection;
            IsolationLevel = isolationLevel;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the owning connection.
        /// </summary>
        protected override DbConnection DbConnection { get; }

        /// <summary>
        /// Gets the isolation level.
        /// </summary>
        public override IsolationLevel IsolationLevel { get; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Completes the simulated transaction.
        /// </summary>
        public override void Commit() { }

        /// <summary>
        /// Rolls back the simulated transaction.
        /// </summary>
        public override void Rollback() { }

        #endregion
    }
}
