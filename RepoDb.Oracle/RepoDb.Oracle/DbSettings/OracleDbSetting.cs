namespace RepoDb.DbSettings
{
    /// <summary>
    /// A setting class used for Oracle data provider.
    /// </summary>
    public sealed class OracleDbSetting : BaseDbSetting
    {
        /// <summary>
        /// Creates a new instance of <see cref="OracleDbSetting"/> class.
        /// </summary>
        public OracleDbSetting()
        {
            AreTableHintsSupported = false;
            ClosingQuote = "\"";
            DefaultSchema = null;
            IsDirectionSupported = true;
            IsExecuteReaderDisposable = true;
            IsMultiStatementExecutable = false;
            IsPreparable = true;
            IsUseUpsert = false;
            MultiStatementSeparator = ";";
            OpeningQuote = "\"";
            ParameterPrefix = ":";
        }
    }
}
