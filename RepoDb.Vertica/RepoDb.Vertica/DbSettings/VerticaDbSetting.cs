using Vertica.Data.VerticaClient;

namespace RepoDb.DbSettings
{
    /// <summary>
    /// A setting class used for <see cref="VerticaConnection"/> data provider.
    /// </summary>
    public sealed class VerticaDbSetting : BaseDbSetting
    {
        /// <summary>
        /// Creates a new instance of <see cref="VerticaDbSetting"/> class.
        /// </summary>
        public VerticaDbSetting()
        {
            AreTableHintsSupported = false;
            ClosingQuote = "\"";
            DefaultSchema = null;
            IsAffectedRowsSupported = true;
            IsDirectionSupported = false;
            IsExecuteReaderDisposable = false;
            IsMultiStatementExecutable = false;
            IsPreparable = true;
            IsTransactionSupported = true;
            IsUseUpsert = false;
            MaxParameterCount = 1500;
            MultiStatementSeparator = ";";
            OpeningQuote = "\"";
            ParameterPrefix = "@";
            SqlTextParameterPrefix = "@";
        }
    }
}
