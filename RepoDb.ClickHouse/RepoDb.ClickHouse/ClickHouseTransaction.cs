using System.Data;
using System.Data.Common;

namespace RepoDb
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class ClickHouseTransaction : DbTransaction
    {
        private readonly ClickHouseConnection connection;

        public ClickHouseTransaction(
            ClickHouseConnection connection,
            IsolationLevel isolationLevel)
        {
            this.connection = connection;
            IsolationLevel = isolationLevel;
        }

        /// <inheritdoc/>
        protected override DbConnection DbConnection => connection;

        /// <inheritdoc/>
        public override IsolationLevel IsolationLevel { get; }

        /// <inheritdoc/>
        public override void Commit() { }

        /// <inheritdoc/>
        public override void Rollback() { }
    }
}
