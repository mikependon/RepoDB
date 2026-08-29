using ClickHouse.Driver.ADO;
using RepoDb.ClickHouse.Interfaces;

namespace RepoDb.DbSettings
{
    /// <summary>
    /// A setting class used for <see cref="ClickHouseConnection"/> data provider.
    /// </summary>
    public sealed class ClickHouseDbSetting : BaseDbSetting, IClickHouseDbSetting
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
            MaxParameterCount = 2100 - 2;
            MultiStatementSeparator = ";";
            OpeningQuote = "`";
            ParameterPrefix = string.Empty;
            SqlTextParameterPrefix = "@";
        }

        /// <summary>
        /// Gets or sets a value indicating whether waiting for mutations to complete is enabled for the ClickHouse database.
        /// </summary>
        public bool IsWaitForMutationsEnabled { get; set; } = true;
    }
}
