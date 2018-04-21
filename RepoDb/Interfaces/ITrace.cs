namespace RepoDb.Interfaces
{
    public interface ITrace
    {
        // Before
        void BeforeBatchQuery(ICancellableTraceLog log);
        void BeforeCount(ICancellableTraceLog log);
        void BeforeCountBig(ICancellableTraceLog log);
        void BeforeQuery(ICancellableTraceLog log);
        void BeforeInlineUpdate(ICancellableTraceLog log);
        void BeforeUpdate(ICancellableTraceLog log);
        void BeforeDelete(ICancellableTraceLog log);
        void BeforeMerge(ICancellableTraceLog log);
        void BeforeInsert(ICancellableTraceLog log);
        void BeforeBulkInsert(ICancellableTraceLog log);
        void BeforeExecuteQuery(ICancellableTraceLog log);
        void BeforeExecuteNonQuery(ICancellableTraceLog log);
        void BeforeExecuteReader(ICancellableTraceLog log);
        void BeforeExecuteScalar(ICancellableTraceLog log);

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
