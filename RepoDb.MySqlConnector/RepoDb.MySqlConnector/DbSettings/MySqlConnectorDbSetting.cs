using MySqlConnector;
using RepoDb.DbSettings;

namespace RepoDb.MySqlConnector.DbSettings
{
    /// <summary>
    /// A setting class used for <see cref="MySqlConnection"/> data provider.
    /// </summary>
    public sealed class MySqlConnectorDbSetting : BaseDbSetting
    {
        /// <summary>
        /// Creates a new instance of <see cref="MySqlConnectorDbSetting"/> class.
        /// </summary>
        public MySqlConnectorDbSetting()
        {
            AreTableHintsSupported = false;
            AverageableType = typeof(double);
            ClosingQuote = "`";
            DefaultSchema = null;
            IsDirectionSupported = false;
            IsExecuteReaderDisposable = true;
            IsMultiStatementExecutable = true;
            IsPreparable = true;
            IsUseUpsert = false;
            OpeningQuote = "`";
            ParameterPrefix = "@";
        }
    }
}
