using RepoDb.Connector.MariaDbConnector;

namespace RepoDb.DbSettings
{
    /// <summary>
    /// A setting class used for <see cref="MariaDbConnection"/> data provider.
    /// </summary>
    public sealed class MariaDbDbSetting : BaseDbSetting
    {
        /// <summary>
        /// Creates a new instance of <see cref="MariaDbDbSetting"/> class.
        /// </summary>
        public MariaDbDbSetting()
        {
            AreTableHintsSupported = false;
            ClosingQuote = "`";
            DefaultSchema = null;
            IsAffectedRowsSupported = true;
            IsDirectionSupported = false;
            IsExecuteReaderDisposable = false;
            IsMultiStatementExecutable = true;
            IsPreparable = false;
            IsTransactionSupported = true;
            IsUseUpsert = false;
            MaxParameterCount = 2100 - 2;
            MultiStatementSeparator = ";";
            OpeningQuote = "`";
            ParameterPrefix = "@";
            SqlTextParameterPrefix = "@";
        }
    }
}
