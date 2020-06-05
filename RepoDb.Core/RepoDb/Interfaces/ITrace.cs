namespace RepoDb.Interfaces
{
    /// <summary>
    /// An interface that is used to mark a class to be usable for tracing the operations. A trace object is being used to provide a auditing and debugging capability
    /// when executing the actual operation. The caller can modify the SQL Statements or the parameters being passed prior the actual execution, or even cancel the prior-execution.
    /// </summary>
    public interface ITrace
    {
        #region Average

        /// <summary>
        /// A method being raised before the actual average operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the average execution.</param>
        void BeforeAverage(CancellableTraceLog log);

        /// <summary>
        /// A method being raised after the actual average operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the average execution.</param>
        void AfterAverage(TraceLog log);

        #endregion

        #region AverageAll

        /// <summary>
        /// A method being raised before the actual average-all operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the average-all execution.</param>
        void BeforeAverageAll(CancellableTraceLog log);

        /// <summary>
        /// A method being raised after the actual average-all operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the average-all execution.</param>
        void AfterAverageAll(TraceLog log);

        #endregion

        #region BatchQuery

        /// <summary>
        /// A method being raised before the actual batch query operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the batch query execution.</param>
        void BeforeBatchQuery(CancellableTraceLog log);

        /// <summary>
        /// A method being raised after the actual batch query operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the batch query execution.</param>

        void AfterBatchQuery(TraceLog log);

        #endregion

        #region Count

        /// <summary>
        /// A method being raised before the actual count operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the count execution.</param>
        void BeforeCount(CancellableTraceLog log);

        /// <summary>
        /// A method being raised after the actual count operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the count execution.</param>
        void AfterCount(TraceLog log);

        #endregion

        #region CountAll

        /// <summary>
        /// A method being raised before the actual count-all operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the count-all execution.</param>
        void BeforeCountAll(CancellableTraceLog log);

        /// <summary>
        /// A method being raised after the actual count-all operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the count-all execution.</param>
        void AfterCountAll(TraceLog log);

        #endregion

        #region Delete

        /// <summary>
        /// A method being raised before the actual delete operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the delete execution.</param>
        void BeforeDelete(CancellableTraceLog log);

        /// <summary>
        /// A method being raised after the actual delete operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the delete execution.</param>
        void AfterDelete(TraceLog log);

        #endregion

        #region DeleteAll

        /// <summary>
        /// A method being raised before the actual delete-all operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the delete-all execution.</param>
        void BeforeDeleteAll(CancellableTraceLog log);

        /// <summary>
        /// A method being raised after the actual delete-all operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the delete-all execution.</param>
        void AfterDeleteAll(TraceLog log);

        #endregion

        #region Exists

        /// <summary>
        /// A method being raised before the actual exists operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the count execution.</param>
        void BeforeExists(CancellableTraceLog log);

        /// <summary>
        /// A method being raised after the actual exists operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the count execution.</param>
        void AfterExists(TraceLog log);

        #endregion

        #region ExecuteNonQuery

        /// <summary>
        /// A method being raised before the actual execute non-query operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the execute non-query execution.</param>
        void BeforeExecuteNonQuery(CancellableTraceLog log);

        /// <summary>
        /// A method being raised after the actual execute non-query operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the execute non-query execution.</param>
        void AfterExecuteNonQuery(TraceLog log);

        #endregion

        #region ExecuteQuery

        /// <summary>
        /// A method being raised before the actual execute operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the execute query execution.</param>
        void BeforeExecuteQuery(CancellableTraceLog log);

        /// <summary>
        /// A method being raised after the actual execute query operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the execute query execution.</param>
        void AfterExecuteQuery(TraceLog log);

        #endregion

        #region ExecuteReader

        /// <summary>
        /// A method being raised before the actual execute reader operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the execute reader execution.</param>
        void BeforeExecuteReader(CancellableTraceLog log);

        /// <summary>
        /// A method being raised after the actual execute reader operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the execute reader execution.</param>
        void AfterExecuteReader(TraceLog log);

        #endregion

        #region ExecuteScalar

        /// <summary>
        /// A method being raised before the actual execute scalar operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the execute scalar execution.</param>
        void BeforeExecuteScalar(CancellableTraceLog log);

        /// <summary>
        /// A method being raised after the actual execute scalar operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the execute scalar execution.</param>
        void AfterExecuteScalar(TraceLog log);

        #endregion

        #region Insert

        /// <summary>
        /// A method being raised before the actual insert operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the insert execution.</param>
        void BeforeInsert(CancellableTraceLog log);

        /// <summary>
        /// A method being raised after the actual insert operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the insert execution.</param>
        void AfterInsert(TraceLog log);

        #endregion

        #region InsertAll

        /// <summary>
        /// A method being raised before the actual insert-all operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the insert-all execution.</param>
        void BeforeInsertAll(CancellableTraceLog log);

        /// <summary>
        /// A method being raised after the actual insert-all operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the insert-all execution.</param>
        void AfterInsertAll(TraceLog log);

        #endregion

        #region Max

        /// <summary>
        /// A method being raised before the actual maximum operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the maximum execution.</param>
        void BeforeMax(CancellableTraceLog log);

        /// <summary>
        /// A method being raised after the actual maximum operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the maximum execution.</param>
        void AfterMax(TraceLog log);

        #endregion

        #region MaxAll

        /// <summary>
        /// A method being raised before the actual maximum-all operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the maximum-all execution.</param>
        void BeforeMaxAll(CancellableTraceLog log);

        /// <summary>
        /// A method being raised after the actual maximum-all operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the maximum-all execution.</param>
        void AfterMaxAll(TraceLog log);

        #endregion

        #region Merge

        /// <summary>
        /// A method being raised before the actual merge operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the merge execution.</param>
        void BeforeMerge(CancellableTraceLog log);

        /// <summary>
        /// A method being raised after the actual merge operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the merge execution.</param>
        void AfterMerge(TraceLog log);

        #endregion

        #region MergeAll

        /// <summary>
        /// A method being raised before the actual merge-all operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the merge-all execution.</param>
        void BeforeMergeAll(CancellableTraceLog log);

        /// <summary>
        /// A method being raised after the actual merge-all operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the merge-all execution.</param>
        void AfterMergeAll(TraceLog log);

        #endregion

        #region Min

        /// <summary>
        /// A method being raised before the actual minimum operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the minimum execution.</param>
        void BeforeMin(CancellableTraceLog log);

        /// <summary>
        /// A method being raised after the actual minimum operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the minimum execution.</param>
        void AfterMin(TraceLog log);

        #endregion

        #region MinAll

        /// <summary>
        /// A method being raised before the actual minimum-all operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the minimum-all execution.</param>
        void BeforeMinAll(CancellableTraceLog log);

        /// <summary>
        /// A method being raised after the actual minimum-all operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the minimum-all execution.</param>
        void AfterMinAll(TraceLog log);

        #endregion

        #region Query

        /// <summary>
        /// A method being raised before the actual query operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the query execution.</param>
        void BeforeQuery(CancellableTraceLog log);

        /// <summary>
        /// A method being raised after the actual query operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the query execution.</param>
        void AfterQuery(TraceLog log);

        #endregion

        #region QueryAll

        /// <summary>
        /// A method being raised before the actual query-all operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the query-all execution.</param>
        void BeforeQueryAll(CancellableTraceLog log);

        /// <summary>
        /// A method being raised after the actual query-all operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the query-all execution.</param>
        void AfterQueryAll(TraceLog log);

        #endregion

        #region QueryMultiple

        /// <summary>
        /// A method being raised before the actual query-multiple operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the query execution.</param>
        void BeforeQueryMultiple(CancellableTraceLog log);

        /// <summary>
        /// A method being raised after the actual query-multiple operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the query execution.</param>
        void AfterQueryMultiple(TraceLog log);

        #endregion

        #region Sum

        /// <summary>
        /// A method being raised before the actual sum operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the sum execution.</param>
        void BeforeSum(CancellableTraceLog log);

        /// <summary>
        /// A method being raised after the actual sum operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the sum execution.</param>
        void AfterSum(TraceLog log);

        #endregion

        #region SumAll

        /// <summary>
        /// A method being raised before the actual sum-all operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the sum-all execution.</param>
        void BeforeSumAll(CancellableTraceLog log);

        /// <summary>
        /// A method being raised after the actual sum-all operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the sum-all execution.</param>
        void AfterSumAll(TraceLog log);

        #endregion

        #region Truncate

        /// <summary>
        /// A method being raised before the actual truncate operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the truncate execution.</param>
        void BeforeTruncate(CancellableTraceLog log);

        /// <summary>
        /// A method being raised after the actual truncate operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the truncate execution.</param>
        void AfterTruncate(TraceLog log);

        #endregion

        #region Update

        /// <summary>
        /// A method being raised before the actual update operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the update execution.</param>
        void BeforeUpdate(CancellableTraceLog log);

        /// <summary>
        /// A method being raised after the actual update operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the update execution.</param>
        void AfterUpdate(TraceLog log);

        #endregion

        #region UpdateAll

        /// <summary>
        /// A method being raised before the actual update-all operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the update-all execution.</param>
        void BeforeUpdateAll(CancellableTraceLog log);

        /// <summary>
        /// A method being raised after the actual update-all operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the update-all execution.</param>
        void AfterUpdateAll(TraceLog log);

        #endregion
    }
}
