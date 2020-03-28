using MySql.Data.MySqlClient;
using RepoDb.DbSettings;

namespace RepoDb.MySql.DbSettings
{
    /// <summary>
    /// A setting class used for <see cref="MySqlConnection"/> data provider.
    /// </summary>
    public sealed class MySqlDbSetting : BaseDbSetting
    {
        /// <summary>
        /// Creates a new instance of <see cref="MySqlDbSetting"/> class.
        /// </summary>
        public MySqlDbSetting()
        {
            AreTableHintsSupported = false;
            AverageableType = typeof(double);
            ClosingQuote = "`";
            DefaultSchema = null;
            IsDirectionSupported = false;
            IsExecuteReaderDisposable = false;
            IsMultiStatementExecutable = true;
            IsPreparable = true;
            IsUseUpsert = false;
            OpeningQuote = "`";
            ParameterPrefix = "@";
            SchemaSeparator = ".";
        }
    }
}
