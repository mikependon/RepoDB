using System.Data;
using System.Data.Common;
using System.Linq;

namespace RepoDb.Schema.UnitTestHelpers
{
    /// <summary>
    /// Provides an in-memory catalog test fixture.
    /// </summary>
    public sealed class CatalogCommand : DbCommand
    {
        #region Constructors

        /// <summary>
        /// Creates a catalog command.
        /// </summary>
        /// <param name="connection">The connection value.</param>
        public CatalogCommand(CatalogConnection connection)
        {
            DbConnection = connection;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the SQL text.
        /// </summary>
        public override string CommandText { get; set; }

        /// <summary>
        /// Gets or sets the timeout.
        /// </summary>
        public override int CommandTimeout { get; set; }

        /// <summary>
        /// Gets or sets the command type.
        /// </summary>
        public override CommandType CommandType { get; set; }

        /// <summary>
        /// Gets or sets designer visibility.
        /// </summary>
        public override bool DesignTimeVisible { get; set; }

        /// <summary>
        /// Gets or sets the row source.
        /// </summary>
        public override UpdateRowSource UpdatedRowSource { get; set; }

        /// <summary>
        /// Gets or sets the connection.
        /// </summary>
        protected override DbConnection DbConnection { get; set; }

        /// <summary>
        /// Gets the parameter collection.
        /// </summary>
        protected override DbParameterCollection DbParameterCollection { get; } = new CatalogParameterCollection();

        /// <summary>
        /// Gets or sets the transaction.
        /// </summary>
        protected override DbTransaction DbTransaction { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Cancels the command without external effects.
        /// </summary>
        public override void Cancel() { }

        /// <summary>
        /// Prepares the command without external effects.
        /// </summary>
        public override void Prepare() { }

        /// <summary>
        /// Reports no changed rows.
        /// </summary>
        public override int ExecuteNonQuery() => 0;

        /// <summary>
        /// Reads the first value of the catalog.
        /// </summary>
        public override object ExecuteScalar() =>
            ((CatalogConnection)DbConnection).Data.Rows.Cast<DataRow>()
                .Select(row => row[0]).FirstOrDefault();

        #endregion

        #region Protected Methods

        /// <summary>
        /// Creates a parameter with static property access.
        /// </summary>
        protected override DbParameter CreateDbParameter() => new CatalogParameter();

        /// <summary>
        /// Reads the configured catalog rows.
        /// </summary>
        /// <param name="behavior">The behavior value.</param>
        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior) =>
            (((CatalogConnection)DbConnection).Query?.Invoke(CommandText) ??
                ((CatalogConnection)DbConnection).Data).CreateDataReader();

        #endregion
    }
}
