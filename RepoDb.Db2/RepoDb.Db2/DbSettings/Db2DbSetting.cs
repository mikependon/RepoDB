namespace RepoDb.DbSettings
{
    /// <summary>
    /// A setting class used for Db2 data provider.
    /// </summary>
    public sealed class Db2DbSetting : BaseDbSetting
    {
        /// <summary>
        /// Creates a new instance of <see cref="Db2DbSetting"/> class.
        /// </summary>
        public Db2DbSetting()
        {
            AreTableHintsSupported = false;
            AverageableType = typeof(double);
            ClosingQuote = "\"";
            DefaultSchema = null;
            IsDirectionSupported = true;
            IsExecuteReaderDisposable = true;
            IsMultiStatementExecutable = true;
            IsPreparable = true;
            IsUseUpsert = false;
            MultiStatementSeparator = ";";
            OpeningQuote = "\"";
            ParameterPrefix = ":";
        }
    }
}
