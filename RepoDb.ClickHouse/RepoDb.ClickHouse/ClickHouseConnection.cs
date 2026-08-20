using System.Data;
using System.Data.Common;

namespace RepoDb
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class ClickHouseConnection : ClickHouse.Driver.ADO.ClickHouseConnection
    {
        /// <summary>
        /// Creates a new instance of <see cref="ClickHouseConnection"/> class.
        /// </summary>
        public ClickHouseConnection()
            : base()
        { }

        /// <summary>
        /// Creates a new instance of <see cref="ClickHouseConnection"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string to be used.</param>
        public ClickHouseConnection(string connectionString)
            : base(connectionString)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="ClickHouseConnection"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string to be used.</param>
        /// <param name="skipServerCertificateValidation">Whether to skip TLS server certificate validation.</param>
        public ClickHouseConnection(
            string connectionString,
            bool skipServerCertificateValidation)
            : base(connectionString, skipServerCertificateValidation)
        { }

        /// <inheritdoc/>
        protected override DbCommand CreateDbCommand() => new ClickHouseCommand(this);

        /// <inheritdoc/>
        protected override DbTransaction BeginDbTransaction(
            IsolationLevel isolationLevel) =>
            new ClickHouseTransaction(this, isolationLevel);
    }
}
