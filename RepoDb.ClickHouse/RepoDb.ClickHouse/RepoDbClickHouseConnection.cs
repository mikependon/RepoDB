using ClickHouse.Driver.ADO;
using System.Data;
using System.Data.Common;

namespace RepoDb
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class RepoDbClickHouseConnection : ClickHouseConnection
    {
        /// <summary>
        /// Creates a new instance of <see cref="RepoDbClickHouseConnection"/> class.
        /// </summary>
        public RepoDbClickHouseConnection()
            : base()
        { }

        /// <summary>
        /// Creates a new instance of <see cref="RepoDbClickHouseConnection"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string to be used.</param>
        public RepoDbClickHouseConnection(string connectionString)
            : base(connectionString)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="RepoDbClickHouseConnection"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string to be used.</param>
        /// <param name="skipServerCertificateValidation">Whether to skip TLS server certificate validation.</param>
        public RepoDbClickHouseConnection(
            string connectionString,
            bool skipServerCertificateValidation)
            : base(connectionString, skipServerCertificateValidation)
        { }

        /// <inheritdoc/>
        protected override DbCommand CreateDbCommand() => new NormalizingClickHouseCommand(this);

        /// <inheritdoc/>
        protected override DbTransaction BeginDbTransaction(
            IsolationLevel isolationLevel) =>
            new NoOpClickHouseTransaction(this, isolationLevel);
    }
}
