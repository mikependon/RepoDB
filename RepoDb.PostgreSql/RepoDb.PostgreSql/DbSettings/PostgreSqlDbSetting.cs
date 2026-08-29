using Npgsql;

namespace RepoDb.DbSettings
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
            ClosingQuote = "\"";
            DefaultSchema = "public";
            IsAffectedRowsSupported = true;
            IsDirectionSupported = true;
            IsExecuteReaderDisposable = true;
            IsMultiStatementExecutable = true;
            IsInsertAllBatchable = true;
            IsPreparable = true;
            IsTransactionSupported = true;
            IsUseUpsert = false;
            RequiresDbTypeBeforeValue = false;
            SkipsUnreferencedParameters = false;
            MaxParameterCount = 2100 - 2;
            OpeningQuote = "\"";
            ParameterPrefix = "@";
            SqlTextParameterPrefix = "@";
        }
    }
}
