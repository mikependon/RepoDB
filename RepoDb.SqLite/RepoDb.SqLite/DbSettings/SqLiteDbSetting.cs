namespace RepoDb.DbSettings
{
    /// <summary>
    /// A setting class used for SQLite data provider.
    /// </summary>
    public sealed class SqLiteDbSetting : BaseDbSetting
    {
        /// <summary>
        /// Creates a new instance of <see cref="SqLiteDbSetting"/> class.
        /// </summary>
        public SqLiteDbSetting()
            : this(true)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="SqLiteDbSetting"/> class.
        /// </summary>
        public SqLiteDbSetting(bool isExecuteReaderDisposable)
        {
            AreTableHintsSupported = false;
            AverageableType = typeof(double);
            ClosingQuote = "]";
            DefaultSchema = null;
            IsAffectedRowsSupported = true;
            IsDirectionSupported = false;
            IsExecuteReaderDisposable = isExecuteReaderDisposable;
            IsMultiStatementExecutable = true;
            IsInsertAllBatchable = true;
            IsPreparable = true;
            IsTransactionSupported = true;
            IsUseUpsert = true;
            RequiresDbTypeBeforeValue = false;
            SkipsUnreferencedParameters = false;
            MaxParameterCount = 999;
            OpeningQuote = "[";
            ParameterPrefix = "@";
            SqlTextParameterPrefix = "@";
        }
    }
}
