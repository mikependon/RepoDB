using ClickHouse.Driver.ADO;

namespace RepoDb.DbSettings
{
    /// <summary>
    /// A setting class used for <see cref="ClickHouseConnection"/> data provider.
    /// </summary>
    public sealed class ClickHouseDbSetting : BaseDbSetting
    {
        /// <summary>
        /// Creates a new instance of <see cref="ClickHouseDbSetting"/> class.
        /// </summary>
        public ClickHouseDbSetting()
        {
            AreTableHintsSupported = false;
            ClosingQuote = "`";
            DefaultSchema = null;
            IsAffectedRowsSupported = false;
            IsDirectionSupported = false;
            IsExecuteReaderDisposable = false;
            IsMultiStatementExecutable = false;
            IsPreparable = false;
            IsTransactionSupported = false;
            IsUseUpsert = false;
            MultiStatementSeparator = ";";
            OpeningQuote = "`";
            ParameterPrefix = string.Empty;
            SqlTextParameterPrefix = "@";
        }
    }
}
