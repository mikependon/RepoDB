using System;
using System.Data;
using System.Data.Common;

namespace RepoDb.Schema.UnitTestHelpers
{
    /// <summary>
    /// Provides an in-memory catalog test fixture.
    /// </summary>
    public sealed class CatalogConnection : DbConnection
    {
        #region Private Variables

        private ConnectionState _state = ConnectionState.Open;

        #endregion

        #region Constructors

        /// <summary>
        /// Registers the statically typed catalog connection.
        /// </summary>
        public CatalogConnection()
        {
            DbSettingMapper.Add<CatalogConnection>(new CatalogDbSetting(),
                true);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the returned catalog rows.
        /// </summary>
        public DataTable Data { get; set; } = new DataTable();

        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        public override string ConnectionString { get; set; } = "Catalog";

        /// <summary>
        /// Gets the current database.
        /// </summary>
        public override string Database => "Catalog";

        /// <summary>
        /// Gets the data source.
        /// </summary>
        public override string DataSource => "Memory";

        /// <summary>
        /// Gets the simulated server version.
        /// </summary>
        public override string ServerVersion => "1.0";

        /// <summary>
        /// Gets the connection state.
        /// </summary>
        public override ConnectionState State => _state;

        /// <summary>
        /// Gets or sets catalog results by SQL statement.
        /// </summary>
        public Func<string, DataTable> Query { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Changes the simulated database.
        /// </summary>
        /// <param name="databaseName">The databaseName value.</param>
        public override void ChangeDatabase(string databaseName)
        {
            ConnectionString = databaseName;
        }

        /// <summary>
        /// Closes the simulated connection.
        /// </summary>
        public override void Close() => _state = ConnectionState.Closed;

        /// <summary>
        /// Opens the simulated connection.
        /// </summary>
        public override void Open() => _state = ConnectionState.Open;

        #endregion

        #region Protected Methods

        /// <summary>
        /// Rejects transactions in the catalog fixture.
        /// </summary>
        /// <param name="isolationLevel">The isolationLevel value.</param>
        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel) =>
            throw new NotSupportedException();

        /// <summary>
        /// Creates a command over the catalog rows.
        /// </summary>
        protected override DbCommand CreateDbCommand() =>
            new CatalogCommand(this);

        #endregion
    }
}
