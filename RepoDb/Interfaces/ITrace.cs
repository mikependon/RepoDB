namespace RepoDb.Interfaces
{
    public interface ITrace
    {
        // Before
        void BeforeBatchQuery(ICancelableTraceLog log);
        void BeforeCount(ICancelableTraceLog log);
        void BeforeCountBig(ICancelableTraceLog log);
        void BeforeQuery(ICancelableTraceLog log);
        void BeforeInlineUpdate(ICancelableTraceLog log);
        void BeforeUpdate(ICancelableTraceLog log);
        void BeforeDelete(ICancelableTraceLog log);
        void BeforeMerge(ICancelableTraceLog log);
        void BeforeInsert(ICancelableTraceLog log);
        void BeforeBulkInsert(ICancelableTraceLog log);
        void BeforeExecuteQuery(ICancelableTraceLog log);
        void BeforeExecuteNonQuery(ICancelableTraceLog log);
        void BeforeExecuteReader(ICancelableTraceLog log);
        void BeforeExecuteScalar(ICancelableTraceLog log);

        // After
        void AfterBatchQuery(ITraceLog log);
        void AfterCount(ITraceLog log);
        void AfterCountBig(ITraceLog log);
        void AfterQuery(ITraceLog log);
        void AfterInlineUpdate(ITraceLog log);
        void AfterUpdate(ITraceLog log);
        void AfterDelete(ITraceLog log);
        void AfterMerge(ITraceLog log);
        void AfterInsert(ITraceLog log);
        void AfterBulkInsert(ITraceLog log);
        void AfterExecuteQuery(ITraceLog log);
        void AfterExecuteNonQuery(ITraceLog log);
        void AfterExecuteReader(ITraceLog log);
        void AfterExecuteScalar(ITraceLog log);
    }
}
