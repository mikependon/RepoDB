namespace RepoDb.Interfaces
{
    /// <summary>
    /// An interface that is used to mark a class to be usable for tracing the operations. A trace object is being used to provide an auditing and debugging capability when executing the actual operation.
    /// The caller can modify the SQL Statements or the parameters being passed prior the actual execution, or even cancel the prior-execution.
    /// </summary>
    public interface ITrace
    {
        #region Average

        /// <summary>
        /// A method that is being raised before the actual 'Average' operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the Average' execution.</param>
        void BeforeAverage(CancellableTraceLog log);

        /// <summary>
        /// A method that is being raised after the actual Average' operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the 'Average' execution.</param>
        void AfterAverage(TraceLog log);

        #endregion

        #region AverageAll

        /// <summary>
        /// A method that is being raised before the actual 'AverageAll' operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the AverageAll' execution.</param>
        void BeforeAverageAll(CancellableTraceLog log);

        /// <summary>
        /// A method that is being raised after the actual AverageAll' operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the 'AverageAll' execution.</param>
        void AfterAverageAll(TraceLog log);

        #endregion

        #region BatchQuery

        /// <summary>
        /// A method that is being raised before the actual 'BatchQuery' operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the BatchQuery' execution.</param>
        void BeforeBatchQuery(CancellableTraceLog log);

        /// <summary>
        /// A method that is being raised after the actual BatchQuery' operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the 'BatchQuery' execution.</param>

        void AfterBatchQuery(TraceLog log);

        #endregion

        #region Count

        /// <summary>
        /// A method that is being raised before the actual 'Count' operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the Count' execution.</param>
        void BeforeCount(CancellableTraceLog log);

        /// <summary>
        /// A method that is being raised after the actual Count' operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the 'Count' execution.</param>
        void AfterCount(TraceLog log);

        #endregion

        #region CountAll

        /// <summary>
        /// A method that is being raised before the actual 'CountAll' operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the CountAll' execution.</param>
        void BeforeCountAll(CancellableTraceLog log);

        /// <summary>
        /// A method that is being raised after the actual CountAll' operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the 'CountAll' execution.</param>
        void AfterCountAll(TraceLog log);

        #endregion

        #region Delete

        /// <summary>
        /// A method that is being raised before the actual 'Delete' operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the Delete' execution.</param>
        void BeforeDelete(CancellableTraceLog log);

        /// <summary>
        /// A method that is being raised after the actual Delete' operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the 'Delete' execution.</param>
        void AfterDelete(TraceLog log);

        #endregion

        #region DeleteAll

        /// <summary>
        /// A method that is being raised before the actual 'DeleteAll' operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the DeleteAll' execution.</param>
        void BeforeDeleteAll(CancellableTraceLog log);

        /// <summary>
        /// A method that is being raised after the actual DeleteAll' operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the 'DeleteAll' execution.</param>
        void AfterDeleteAll(TraceLog log);

        #endregion

        #region Exists

        /// <summary>
        /// A method that is being raised before the actual 'Exists' operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the Exists' execution.</param>
        void BeforeExists(CancellableTraceLog log);

        /// <summary>
        /// A method that is being raised after the actual Exists' operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the 'Exists' execution.</param>
        void AfterExists(TraceLog log);

        #endregion

        #region ExecuteNonQuery

        /// <summary>
        /// A method that is being raised before the actual 'ExecuteNonQuery' operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the ExecuteNonQuery' execution.</param>
        void BeforeExecuteNonQuery(CancellableTraceLog log);

        /// <summary>
        /// A method that is being raised after the actual ExecuteNonQuery' operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the 'ExecuteNonQuery' execution.</param>
        void AfterExecuteNonQuery(TraceLog log);

        #endregion

        #region ExecuteQuery

        /// <summary>
        /// A method that is being raised before the actual 'ExecuteQuery' operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the ExecuteQuery' execution.</param>
        void BeforeExecuteQuery(CancellableTraceLog log);

        /// <summary>
        /// A method that is being raised after the actual ExecuteQuery' operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the 'ExecuteQuery' execution.</param>
        void AfterExecuteQuery(TraceLog log);

        #endregion

        #region ExecuteReader

        /// <summary>
        /// A method that is being raised before the actual 'ExecuteReader' operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the ExecuteReader' execution.</param>
        void BeforeExecuteReader(CancellableTraceLog log);

        /// <summary>
        /// A method that is being raised after the actual ExecuteReader' operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the 'ExecuteReader' execution.</param>
        void AfterExecuteReader(TraceLog log);

        #endregion

        #region ExecuteScalar

        /// <summary>
        /// A method that is being raised before the actual 'ExecuteScalar' operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the ExecuteScalar' execution.</param>
        void BeforeExecuteScalar(CancellableTraceLog log);

        /// <summary>
        /// A method that is being raised after the actual ExecuteScalar' operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the 'ExecuteScalar' execution.</param>
        void AfterExecuteScalar(TraceLog log);

        #endregion

        #region Insert

        /// <summary>
        /// A method that is being raised before the actual 'Insert' operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the Insert' execution.</param>
        void BeforeInsert(CancellableTraceLog log);

        /// <summary>
        /// A method that is being raised after the actual Insert' operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the 'Insert' execution.</param>
        void AfterInsert(TraceLog log);

        #endregion

        #region InsertAll

        /// <summary>
        /// A method that is being raised before the actual 'InsertAll' operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the InsertAll' execution.</param>
        void BeforeInsertAll(CancellableTraceLog log);

        /// <summary>
        /// A method that is being raised after the actual InsertAll' operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the 'InsertAll' execution.</param>
        void AfterInsertAll(TraceLog log);

        #endregion

        #region Max

        /// <summary>
        /// A method that is being raised before the actual 'Max' operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the Max' execution.</param>
        void BeforeMax(CancellableTraceLog log);

        /// <summary>
        /// A method that is being raised after the actual Max' operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the 'Max' execution.</param>
        void AfterMax(TraceLog log);

        #endregion

        #region MaxAll

        /// <summary>
        /// A method that is being raised before the actual 'MaxAll' operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the MaxAll' execution.</param>
        void BeforeMaxAll(CancellableTraceLog log);

        /// <summary>
        /// A method that is being raised after the actual MaxAll' operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the 'MaxAll' execution.</param>
        void AfterMaxAll(TraceLog log);

        #endregion

        #region Merge

        /// <summary>
        /// A method that is being raised before the actual 'Merge' operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the Merge' execution.</param>
        void BeforeMerge(CancellableTraceLog log);

        /// <summary>
        /// A method that is being raised after the actual Merge' operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the 'Merge' execution.</param>
        void AfterMerge(TraceLog log);

        #endregion

        #region MergeAll

        /// <summary>
        /// A method that is being raised before the actual 'MergeAll' operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the MergeAll' execution.</param>
        void BeforeMergeAll(CancellableTraceLog log);

        /// <summary>
        /// A method that is being raised after the actual MergeAll' operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the 'MergeAll' execution.</param>
        void AfterMergeAll(TraceLog log);

        #endregion

        #region Min

        /// <summary>
        /// A method that is being raised before the actual 'Min' operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the Min' execution.</param>
        void BeforeMin(CancellableTraceLog log);

        /// <summary>
        /// A method that is being raised after the actual Min' operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the 'Min' execution.</param>
        void AfterMin(TraceLog log);

        #endregion

        #region MinAll

        /// <summary>
        /// A method that is being raised before the actual 'MinAll' operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the MinAll' execution.</param>
        void BeforeMinAll(CancellableTraceLog log);

        /// <summary>
        /// A method that is being raised after the actual MinAll' operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the 'MinAll' execution.</param>
        void AfterMinAll(TraceLog log);

        #endregion

        #region Query

        /// <summary>
        /// A method that is being raised before the actual 'Query' operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the Query' execution.</param>
        void BeforeQuery(CancellableTraceLog log);

        /// <summary>
        /// A method that is being raised after the actual Query' operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the 'Query' execution.</param>
        void AfterQuery(TraceLog log);

        #endregion

        #region QueryAll

        /// <summary>
        /// A method that is being raised before the actual 'QueryAll' operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the QueryAll' execution.</param>
        void BeforeQueryAll(CancellableTraceLog log);

        /// <summary>
        /// A method that is being raised after the actual QueryAll' operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the 'QueryAll' execution.</param>
        void AfterQueryAll(TraceLog log);

        #endregion

        #region QueryMultiple

        /// <summary>
        /// A method that is being raised before the actual 'QueryMultiple' operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the QueryMultiple' execution.</param>
        void BeforeQueryMultiple(CancellableTraceLog log);

        /// <summary>
        /// A method that is being raised after the actual QueryMultiple' operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the 'QueryMultiple' execution.</param>
        void AfterQueryMultiple(TraceLog log);

        #endregion

        #region Sum

        /// <summary>
        /// A method that is being raised before the actual 'Sum' operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the Sum' execution.</param>
        void BeforeSum(CancellableTraceLog log);

        /// <summary>
        /// A method that is being raised after the actual Sum' operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the 'Sum' execution.</param>
        void AfterSum(TraceLog log);

        #endregion

        #region SumAll

        /// <summary>
        /// A method that is being raised before the actual 'SumAll' operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the SumAll' execution.</param>
        void BeforeSumAll(CancellableTraceLog log);

        /// <summary>
        /// A method that is being raised after the actual SumAll' operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the 'SumAll' execution.</param>
        void AfterSumAll(TraceLog log);

        #endregion

        #region Truncate

        /// <summary>
        /// A method that is being raised before the actual 'Truncate' operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the Truncate' execution.</param>
        void BeforeTruncate(CancellableTraceLog log);

        /// <summary>
        /// A method that is being raised after the actual Truncate' operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the 'Truncate' execution.</param>
        void AfterTruncate(TraceLog log);

        #endregion

        #region Update

        /// <summary>
        /// A method that is being raised before the actual 'Update' operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the Update' execution.</param>
        void BeforeUpdate(CancellableTraceLog log);

        /// <summary>
        /// A method that is being raised after the actual Update' operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the 'Update' execution.</param>
        void AfterUpdate(TraceLog log);

        #endregion

        #region UpdateAll

        /// <summary>
        /// A method that is being raised before the actual 'UpdateAll' operation execution.
        /// </summary>
        /// <param name="log">The cancellable log object referenced by the UpdateAll' execution.</param>
        void BeforeUpdateAll(CancellableTraceLog log);

        /// <summary>
        /// A method that is being raised after the actual UpdateAll' operation execution.
        /// </summary>
        /// <param name="log">The log object referenced by the 'UpdateAll' execution.</param>
        void AfterUpdateAll(TraceLog log);

        #endregion
    }
}
