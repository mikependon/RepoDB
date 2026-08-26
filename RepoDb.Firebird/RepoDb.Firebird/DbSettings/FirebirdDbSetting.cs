using FirebirdSql.Data.FirebirdClient;

namespace RepoDb.DbSettings
{
    /// <summary>
    /// A setting class used for <see cref="FbConnection"/> data provider.
    /// </summary>
    public sealed class FirebirdDbSetting : BaseDbSetting
    {
        /// <summary>
        /// Creates a new instance of <see cref="FirebirdDbSetting"/> class.
        /// </summary>
        public FirebirdDbSetting()
        {
            AreTableHintsSupported = false;
            ClosingQuote = "\"";
            DefaultSchema = null;
            IsAffectedRowsSupported = true;
            IsDirectionSupported = false;
            IsExecuteReaderDisposable = true;
            IsMultiStatementExecutable = false;
            IsPreparable = true;
            IsTransactionSupported = true;
            IsUseUpsert = false;
            MultiStatementSeparator = ";";
            OpeningQuote = "\"";
            ParameterPrefix = "@";
            SqlTextParameterPrefix = "@";
        }
    }
}
