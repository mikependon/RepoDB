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
            // Oracle hint syntax ("SELECT /*+ HINT */ ...") is positioned differently than the
            // "WITH (...)" hints emitted right after the table name by the base statement builder,
            // so table hints are not supported for now.
            AreTableHintsSupported = false;
            AverageableType = typeof(double);
            ClosingQuote = "\"";
            DefaultSchema = null;
            IsDirectionSupported = true;
            IsExecuteReaderDisposable = true;
            // Oracle (ODP.NET) does not support multiple SQL statements in a single CommandText
            // the way SQL Server/PostgreSql do; batching multiple INSERT/MERGE statements (and
            // their RETURNING values) into a single round-trip is not implemented yet, so batch
            // operations fall back to one round-trip per row.
            IsMultiStatementExecutable = false;
            IsPreparable = true;
            // Oracle has a real MERGE statement, no need for upsert emulation.
            IsUseUpsert = false;
            OpeningQuote = "\"";
            ParameterPrefix = ":";
        }
    }
}
