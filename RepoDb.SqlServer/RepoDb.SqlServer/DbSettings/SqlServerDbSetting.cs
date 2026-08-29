namespace RepoDb.DbSettings
{
    /// <summary>
    /// A setting class used for SQL Server data provider.
    /// </summary>
    public sealed class SqlServerDbSetting : BaseDbSetting
    {
        /// <summary>
        /// Creates a new instance of <see cref="SqlServerDbSetting"/> class.
        /// </summary>
        public SqlServerDbSetting()
        {
            AreTableHintsSupported = true;
            ClosingQuote = "]";
            DefaultSchema = "dbo";
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
            MultiStatementSeparator = ";";
            OpeningQuote = "[";
            ParameterPrefix = "@";
            SqlTextParameterPrefix = "@";
        }
    }
}
