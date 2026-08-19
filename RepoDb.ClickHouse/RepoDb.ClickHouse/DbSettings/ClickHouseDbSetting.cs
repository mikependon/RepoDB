using ClickHouse.Driver.ADO;

namespace RepoDb.DbSettings
{
    /// <summary>
    /// A setting class used for <see cref="RepoDbClickHouseConnection"/> data provider.
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
            IsDirectionSupported = false;
            IsExecuteReaderDisposable = false;
            IsMultiStatementExecutable = false;
            IsPreparable = false;
            IsUseUpsert = false;
            MultiStatementSeparator = ";";
            OpeningQuote = "`";
            ParameterPrefix = "@";
        }
    }
}
