using Npgsql;
using RepoDb.DbSettings;

namespace RepoDb.PostgreSql.DbSettings
{
    /// <summary>
    /// A setting class used for <see cref="NpgsqlConnection"/> data provider.
    /// </summary>
    internal sealed class PostgreSqlDbSetting : BaseDbSetting
    {
        /// <summary>
        /// Creates a new instance of <see cref="SqlServerDbSetting"/> class.
        /// </summary>
        public PostgreSqlDbSetting()
            : base()
        {
            AreTableHintsSupported = false;
            AverageableType = typeof(double);
            ClosingQuote = "\"";
            DefaultSchema = "public";
            IsDirectionSupported = true;
            IsExecuteReaderDisposable = true;
            IsMultiStatementExecutable = true;
            IsPreparable = false;
            IsUseUpsert = false;
            OpeningQuote = "\"";
            ParameterPrefix = "@";
            SchemaSeparator = ".";
        }
    }
}
