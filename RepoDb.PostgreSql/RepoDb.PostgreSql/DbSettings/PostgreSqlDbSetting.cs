using Npgsql;
using RepoDb.DbSettings;

namespace RepoDb.PostgreSql.DbSettings
{
    /// <summary>
    /// A setting class used for <see cref="NpgsqlConnection"/> data provider.
    /// </summary>
    public sealed class PostgreSqlDbSetting : BaseDbSetting
    {
        /// <summary>
        /// Creates a new instance of <see cref="PostgreSqlDbSetting"/> class.
        /// </summary>
        public PostgreSqlDbSetting()
        {
            AreTableHintsSupported = false;
            AverageableType = typeof(double);
            ClosingQuote = "\"";
            DefaultSchema = "public";
            IsDirectionSupported = true;
            IsExecuteReaderDisposable = true;
            IsMultiStatementExecutable = true;
            IsPreparable = true;
            IsUseUpsert = false;
            OpeningQuote = "\"";
            ParameterPrefix = "@";
        }
    }
}
