using Microsoft.Data.Sqlite;

namespace RepoDb.DbSettings
{
    /// <summary>
    /// A setting class used for <see cref="SqliteConnection"/> data provider.
    /// </summary>
    public sealed class SqLiteDbSetting : BaseDbSetting
    {
        /// <summary>
        /// Creates a new instance of <see cref="SqLiteDbSetting"/> class.
        /// </summary>
        public SqLiteDbSetting()
            : base()
        {
            AreTableHintsSupported = false;
            AverageableType = typeof(double);
            ClosingQuote = "]";
            DefaultSchema = null;
            IsDirectionSupported = false;
            IsExecuteReaderDisposable = false;
            IsMultiStatementExecutable = true;
            IsPreparable = true;
            IsUseUpsert = false;
            OpeningQuote = "[";
            ParameterPrefix = "@";
        }
    }
}
