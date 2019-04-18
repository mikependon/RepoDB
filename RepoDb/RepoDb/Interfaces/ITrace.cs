using System.Data.Common;

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
        /// A method being raised before the actual batch query operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the batch query execution.</param>
        void BeforeBatchQuery(CancellableTraceLog log);

        /// <summary>
        /// A method being raised before the actual bulk-insert operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the bulk-insert execution.</param>
        void BeforeBulkInsert(CancellableTraceLog log);

        /// <summary>
        /// A method being raised before the actual count operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the count execution.</param>
        void BeforeCount(CancellableTraceLog log);

        /// <summary>
        /// A method being raised before the actual delete operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the delete execution.</param>
        void BeforeDelete(CancellableTraceLog log);

        /// <summary>
        /// A method being raised before the actual delete-all operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the delete-all execution.</param>
        void BeforeDeleteAll(CancellableTraceLog log);

        /// <summary>
        /// A method being raised before the actual execute non-query operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the execute non-query execution.</param>
        void BeforeExecuteNonQuery(CancellableTraceLog log);

        /// <summary>
        /// A method being raised before the actual execute operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the execute query execution.</param>
        void BeforeExecuteQuery(CancellableTraceLog log);

        /// <summary>
        /// A method being raised before the actual execute reader operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the execute reader execution.</param>
        void BeforeExecuteReader(CancellableTraceLog log);

        /// <summary>
        /// A method being raised before the actual execute scalar operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the execute scalar execution.</param>
        void BeforeExecuteScalar(CancellableTraceLog log);

        /// <summary>
        /// A method being raised before the actual inline-delete operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the inline-delete execution.</param>
        void BeforeInlineDelete(CancellableTraceLog log);

        /// <summary>
        /// A method being raised before the actual inline-insert operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the inline-insert execution.</param>
        void BeforeInlineInsert(CancellableTraceLog log);

        /// <summary>
        /// A method being raised before the actual inline-merge operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the inline-merge execution.</param>
        void BeforeInlineMerge(CancellableTraceLog log);

        /// <summary>
        /// A method being raised before the actual inline update operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the inline update execution.</param>
        void BeforeInlineUpdate(CancellableTraceLog log);

        /// <summary>
        /// A method being raised before the actual insert operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the insert execution.</param>
        void BeforeInsert(CancellableTraceLog log);

        /// <summary>
        /// A method being raised before the actual merge operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the merge execution.</param>
        void BeforeMerge(CancellableTraceLog log);

        /// <summary>
        /// A method being raised before the actual query operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the query execution.</param>
        void BeforeQuery(CancellableTraceLog log);

        /// <summary>
        /// A method being raised before the actual query-multiple operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the query execution.</param>
        void BeforeQueryMultiple(CancellableTraceLog log);

        /// <summary>
        /// A method being raised before the actual truncate operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the truncate execution.</param>
        void BeforeTruncate(CancellableTraceLog log);

        /// <summary>
        /// A method being raised before the actual update operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the update execution.</param>
        void BeforeUpdate(CancellableTraceLog log);

        /*
         * AFTER
         */

        /// <summary>
        /// A method being raised after the actual batch query operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the batch query execution.</param>

        void AfterBatchQuery(TraceLog log);

        /// <summary>
        /// A method being raised after the actual bulk-insert operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the bulk-insert execution.</param>
        void AfterBulkInsert(TraceLog log);

        /// <summary>
        /// A method being raised after the actual count operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the count execution.</param>
        void AfterCount(TraceLog log);

        /// <summary>
        /// A method being raised after the actual delete operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the delete execution.</param>
        void AfterDelete(TraceLog log);

        /// <summary>
        /// A method being raised after the actual delete-all operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the delete-all execution.</param>
        void AfterDeleteAll(TraceLog log);

        /// <summary>
        /// A method being raised after the actual execute non-query operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the execute non-query execution.</param>
        void AfterExecuteNonQuery(TraceLog log);

        /// <summary>
        /// A method being raised after the actual execute query operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the execute query execution.</param>
        void AfterExecuteQuery(TraceLog log);

        /// <summary>
        /// A method being raised after the actual execute reader operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the execute reader execution.</param>
        void AfterExecuteReader(TraceLog log);

        /// <summary>
        /// A method being raised after the actual execute scalar operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the execute scalar execution.</param>
        void AfterExecuteScalar(TraceLog log);

        /// <summary>
        /// A method being raised after the actual inline-delete operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the inline-delete execution.</param>
        void AfterInlineDelete(TraceLog log);

        /// <summary>
        /// A method being raised after the actual inline-insert operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the inline-insert execution.</param>
        void AfterInlineInsert(TraceLog log);

        /// <summary>
        /// A method being raised after the actual inline-merge operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the inline-merge execution.</param>
        void AfterInlineMerge(TraceLog log);

        /// <summary>
        /// A method being raised after the actual inline update operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the inline update execution.</param>
        void AfterInlineUpdate(TraceLog log);

        /// <summary>
        /// A method being raised after the actual insert operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the insert execution.</param>
        void AfterInsert(TraceLog log);

        /// <summary>
        /// A method being raised after the actual merge operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the merge execution.</param>
        void AfterMerge(TraceLog log);

        /// <summary>
        /// A method being raised after the actual query operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the query execution.</param>
        void AfterQuery(TraceLog log);

        /// <summary>
        /// A method being raised after the actual query-multiple operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the query execution.</param>
        void AfterQueryMultiple(TraceLog log);

        /// <summary>
        /// A method being raised after the actual truncate operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the truncate execution.</param>
        void AfterTruncate(TraceLog log);

        /// <summary>
        /// A method being raised after the actual update operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the update execution.</param>
        void AfterUpdate(TraceLog log);
    }
}
