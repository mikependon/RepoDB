using System.Data.SqlClient;

namespace RepoDb.DbSettings
{
    /// <summary>
    /// A setting class used for <see cref="SqlConnection"/> data provider.
    /// </summary>
    internal sealed class SqlServerDbSetting : BaseDbSetting
    {
        /// <summary>
        /// Creates a new instance of <see cref="SqlServerDbSetting"/> class.
        /// </summary>
        public SqlServerDbSetting()
            : base()
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
            SchemaSeparator = ".";
        }
    }
}
