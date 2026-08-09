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
            // Deliberately false, not just "not yet implemented": this only governs whether
            // RepoDb.Core batches *multiple entities* into one round-trip command text
            // (InsertAll/UpdateAll/MergeAll batch-size clamping, QueryMultiple's round-trip
            // strategy - see RepoDb.Core/RepoDb/Operations/DbConnection/{InsertAll,UpdateAll,
            // MergeAll,QueryMultiple}.cs). It has no bearing on whether a single statement-builder
            // call may itself return multi-statement SQL text for one entity - Db2StatementBuilder
            // already does exactly that for Merge (see WrapMergeWithReturningResult), independent
            // of this flag. Flipping this to true would make Core start passing batchSize > 1 into
            // CreateInsertAll/CreateUpdateAll/CreateMergeAll, which only implement genuine
            // single-row SQL today (see ValidateMultipleStatementExecution) - that would need real
            // multi-row batch SQL generation first, not just this flag.
            IsMultiStatementExecutable = false;
            IsPreparable = true;
            IsUseUpsert = false;
            OpeningQuote = "\"";
            ParameterPrefix = ":";
        }
    }
}
