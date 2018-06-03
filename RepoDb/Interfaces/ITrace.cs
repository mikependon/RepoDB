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
        void BeforeBatchQuery(ICancellableTraceLog log);

        /// <summary>
        /// A method being raised before the actual <i>Count</i> operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the <i>Count</i> execution.</param>
        void BeforeCount(ICancellableTraceLog log);

        /// <summary>
        /// A method being raised before the actual <i>CountBig</i> operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the <i>CountBig</i> execution.</param>
        void BeforeCountBig(ICancellableTraceLog log);

        /// <summary>
        /// A method being raised before the actual <i>Query</i> operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the <i>Query</i> execution.</param>
        void BeforeQuery(ICancellableTraceLog log);

        /// <summary>
        /// A method being raised before the actual <i>InlineUpdate</i> operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the <i>InlineUpdate</i> execution.</param>
        void BeforeInlineUpdate(ICancellableTraceLog log);

        /// <summary>
        /// A method being raised before the actual <i>Update</i> operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the <i>Update</i> execution.</param>
        void BeforeUpdate(ICancellableTraceLog log);

        /// <summary>
        /// A method being raised before the actual <i>Delete</i> operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the <i>Delete</i> execution.</param>
        void BeforeDelete(ICancellableTraceLog log);

        /// <summary>
        /// A method being raised before the actual <i>Merge</i> operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the <i>Merge</i> execution.</param>
        void BeforeMerge(ICancellableTraceLog log);

        /// <summary>
        /// A method being raised before the actual <i>Insert</i> operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the <i>Insert</i> execution.</param>
        void BeforeInsert(ICancellableTraceLog log);

        /// <summary>
        /// A method being raised before the actual <i>BulkInsert</i> operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the <i>BulkInsert</i> execution.</param>
        void BeforeBulkInsert(ICancellableTraceLog log);

        /// <summary>
        /// A method being raised before the actual <i>ExecuteQuery</i> operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the <i>ExecuteQuery</i> execution.</param>
        void BeforeExecuteQuery(ICancellableTraceLog log);

        /// <summary>
        /// A method being raised before the actual <i>ExecuteNonQuery</i> operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the <i>ExecuteNonQuery</i> execution.</param>
        void BeforeExecuteNonQuery(ICancellableTraceLog log);

        /// <summary>
        /// A method being raised before the actual <i>ExecuteReader</i> operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the <i>ExecuteReader</i> execution.</param>
        void BeforeExecuteReader(ICancellableTraceLog log);

        /// <summary>
        /// A method being raised before the actual <i>ExecuteScalar</i> operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the <i>ExecuteScalar</i> execution.</param>
        void BeforeExecuteScalar(ICancellableTraceLog log);

        /*
         * AFTER
         */

        /// <summary>
        /// A method being raised after the actual <i>BatchQuery</i> operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the <i>BatchQuery</i> execution.</param>

        void AfterBatchQuery(ITraceLog log);

        /// <summary>
        /// A method being raised after the actual <i>Count</i> operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the <i>Count</i> execution.</param>
        void AfterCount(ITraceLog log);

        /// <summary>
        /// A method being raised after the actual <i>CountBig</i> operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the <i>CountBig</i> execution.</param>
        void AfterCountBig(ITraceLog log);

        /// <summary>
        /// A method being raised after the actual <i>Query</i> operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the <i>Query</i> execution.</param>
        void AfterQuery(ITraceLog log);

        /// <summary>
        /// A method being raised after the actual <i>InlineUpdate</i> operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the <i>InlineUpdate</i> execution.</param>
        void AfterInlineUpdate(ITraceLog log);

        /// <summary>
        /// A method being raised after the actual <i>Update</i> operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the <i>Update</i> execution.</param>
        void AfterUpdate(ITraceLog log);

        /// <summary>
        /// A method being raised after the actual <i>Delete</i> operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the <i>Delete</i> execution.</param>
        void AfterDelete(ITraceLog log);

        /// <summary>
        /// A method being raised after the actual <i>Merge</i> operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the <i>Merge</i> execution.</param>
        void AfterMerge(ITraceLog log);

        /// <summary>
        /// A method being raised after the actual <i>Insert</i> operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the <i>Insert</i> execution.</param>
        void AfterInsert(ITraceLog log);

        /// <summary>
        /// A method being raised after the actual <i>BulkInsert</i> operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the <i>BulkInsert</i> execution.</param>
        void AfterBulkInsert(ITraceLog log);

        /// <summary>
        /// A method being raised after the actual <i>ExecuteQuery</i> operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the <i>ExecuteQuery</i> execution.</param>
        void AfterExecuteQuery(ITraceLog log);

        /// <summary>
        /// A method being raised after the actual <i>ExecuteNonQuery</i> operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the <i>ExecuteNonQuery</i> execution.</param>
        void AfterExecuteNonQuery(ITraceLog log);

        /// <summary>
        /// A method being raised after the actual <i>ExecuteReader</i> operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the <i>ExecuteReader</i> execution.</param>
        void AfterExecuteReader(ITraceLog log);

        /// <summary>
        /// A method being raised after the actual <i>ExecuteScalar</i> operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the <i>ExecuteScalar</i> execution.</param>
        void AfterExecuteScalar(ITraceLog log);
    }
}
