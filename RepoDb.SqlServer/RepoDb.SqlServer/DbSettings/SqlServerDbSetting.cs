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
            AverageableType = typeof(double);
            ClosingQuote = "]";
            DefaultSchema = "dbo";
            IsDirectionSupported = true;
            IsExecuteReaderDisposable = true;
            IsMultiStatementExecutable = true;
            IsPreparable = true;
            IsUseUpsert = false;
            OpeningQuote = "[";
            ParameterPrefix = "@";
        }
    }
}
