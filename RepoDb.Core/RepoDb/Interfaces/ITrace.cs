namespace RepoDb.Interfaces
{
    /// <summary>
    /// An interface used to mark a class to be usable for tracing operations. A trace object is being used by the repositories on every operations
    /// (before or after) the actual execution. It provides the flexibility of the operations to be traceable and debuggable. The caller can modify
    /// the SQL Statements or the parameters being passed prior the actual execution, or even cancel the prior-execution.
    /// </summary>
    public interface ITrace
    {
        /*
         * BEFORE
         */

        /// <summary>
        /// A method being raised before the actual <i>BatchQuery</i> operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the <i>BatchQuery</i> execution.</param>
        void BeforeBatchQuery(CancellableTraceLog log);

        /// <summary>
        /// A method being raised before the actual <i>BulkInsert</i> operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the <i>BulkInsert</i> execution.</param>
        void BeforeBulkInsert(CancellableTraceLog log);

        /// <summary>
        /// A method being raised before the actual <i>Count</i> operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the <i>Count</i> execution.</param>
        void BeforeCount(CancellableTraceLog log);

        /// <summary>
        /// A method being raised before the actual <i>Delete</i> operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the <i>Delete</i> execution.</param>
        void BeforeDelete(CancellableTraceLog log);

        /// <summary>
        /// A method being raised before the actual <i>DeleteAll</i> operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the <i>DeleteAll</i> execution.</param>
        void BeforeDeleteAll(CancellableTraceLog log);

        /// <summary>
        /// A method being raised before the actual <i>ExecuteNonQuery</i> operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the <i>ExecuteNonQuery</i> execution.</param>
        void BeforeExecuteNonQuery(CancellableTraceLog log);

        /// <summary>
        /// A method being raised before the actual <i>ExecuteQuery</i> operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the <i>ExecuteQuery</i> execution.</param>
        void BeforeExecuteQuery(CancellableTraceLog log);

        /// <summary>
        /// A method being raised before the actual <i>ExecuteReader</i> operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the <i>ExecuteReader</i> execution.</param>
        void BeforeExecuteReader(CancellableTraceLog log);

        /// <summary>
        /// A method being raised before the actual <i>ExecuteScalar</i> operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the <i>ExecuteScalar</i> execution.</param>
        void BeforeExecuteScalar(CancellableTraceLog log);

        /// <summary>
        /// A method being raised before the actual <i>InlineInsert</i> operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the <i>InlineInsert</i> execution.</param>
        void BeforeInlineInsert(CancellableTraceLog log);

        /// <summary>
        /// A method being raised before the actual <i>InlineMerge</i> operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the <i>InlineMerge</i> execution.</param>
        void BeforeInlineMerge(CancellableTraceLog log);

        /// <summary>
        /// A method being raised before the actual <i>InlineUpdate</i> operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the <i>InlineUpdate</i> execution.</param>
        void BeforeInlineUpdate(CancellableTraceLog log);

        /// <summary>
        /// A method being raised before the actual <i>Insert</i> operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the <i>Insert</i> execution.</param>
        void BeforeInsert(CancellableTraceLog log);

        /// <summary>
        /// A method being raised before the actual <i>Merge</i> operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the <i>Merge</i> execution.</param>
        void BeforeMerge(CancellableTraceLog log);

        /// <summary>
        /// A method being raised before the actual <i>Query</i> operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the <i>Query</i> execution.</param>
        void BeforeQuery(CancellableTraceLog log);

        /// <summary>
        /// A method being raised before the actual <i>Truncate</i> operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the <i>Truncate</i> execution.</param>
        void BeforeTruncate(CancellableTraceLog log);

        /// <summary>
        /// A method being raised before the actual <i>Update</i> operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the <i>Update</i> execution.</param>
        void BeforeUpdate(CancellableTraceLog log);

        /*
         * AFTER
         */

        /// <summary>
        /// A method being raised after the actual <i>BatchQuery</i> operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the <i>BatchQuery</i> execution.</param>

        void AfterBatchQuery(TraceLog log);

        /// <summary>
        /// A method being raised after the actual <i>BulkInsert</i> operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the <i>BulkInsert</i> execution.</param>
        void AfterBulkInsert(TraceLog log);

        /// <summary>
        /// A method being raised after the actual <i>Count</i> operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the <i>Count</i> execution.</param>
        void AfterCount(TraceLog log);

        /// <summary>
        /// A method being raised after the actual <i>Delete</i> operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the <i>Delete</i> execution.</param>
        void AfterDelete(TraceLog log);

        /// <summary>
        /// A method being raised after the actual <i>DeleteAll</i> operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the <i>DeleteAll</i> execution.</param>
        void AfterDeleteAll(TraceLog log);

        /// <summary>
        /// A method being raised after the actual <i>ExecuteNonQuery</i> operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the <i>ExecuteNonQuery</i> execution.</param>
        void AfterExecuteNonQuery(TraceLog log);

        /// <summary>
        /// A method being raised after the actual <i>ExecuteQuery</i> operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the <i>ExecuteQuery</i> execution.</param>
        void AfterExecuteQuery(TraceLog log);

        /// <summary>
        /// A method being raised after the actual <i>ExecuteReader</i> operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the <i>ExecuteReader</i> execution.</param>
        void AfterExecuteReader(TraceLog log);

        /// <summary>
        /// A method being raised after the actual <i>ExecuteScalar</i> operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the <i>ExecuteScalar</i> execution.</param>
        void AfterExecuteScalar(TraceLog log);

        /// <summary>
        /// A method being raised after the actual <i>InlineInsert</i> operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the <i>InlineInsert</i> execution.</param>
        void AfterInlineInsert(TraceLog log);

        /// <summary>
        /// A method being raised after the actual <i>InlineMerge</i> operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the <i>InlineMerge</i> execution.</param>
        void AfterInlineMerge(TraceLog log);

        /// <summary>
        /// A method being raised after the actual <i>InlineUpdate</i> operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the <i>InlineUpdate</i> execution.</param>
        void AfterInlineUpdate(TraceLog log);

        /// <summary>
        /// A method being raised after the actual <i>Insert</i> operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the <i>Insert</i> execution.</param>
        void AfterInsert(TraceLog log);

        /// <summary>
        /// A method being raised after the actual <i>Merge</i> operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the <i>Merge</i> execution.</param>
        void AfterMerge(TraceLog log);

        /// <summary>
        /// A method being raised after the actual <i>Query</i> operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the <i>Query</i> execution.</param>
        void AfterQuery(TraceLog log);

        /// <summary>
        /// A method being raised after the actual <i>Truncate</i> operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the <i>Truncate</i> execution.</param>
        void AfterTruncate(TraceLog log);

        /// <summary>
        /// A method being raised after the actual <i>Update</i> operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the <i>Update</i> execution.</param>
        void AfterUpdate(TraceLog log);

    }
}
