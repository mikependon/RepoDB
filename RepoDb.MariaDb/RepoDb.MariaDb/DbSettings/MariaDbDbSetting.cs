using RepoDb.Connector.MariaDb;

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
            AverageableType = typeof(double);
            ClosingQuote = "`";
            DefaultSchema = null;
            IsDirectionSupported = false;
            IsExecuteReaderDisposable = false;
            IsMultiStatementExecutable = true;
            IsPreparable = false;
            IsUseUpsert = false;
            MultiStatementSeparator = ";";
            OpeningQuote = "`";
            ParameterPrefix = "@";
        }
    }
}
