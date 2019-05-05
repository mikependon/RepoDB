using RepoDb;
using RepoDb.Interfaces;
using System;

namespace SqlDbParameterPerformanceNetFramework
{
    public class CustomTrace : ITrace
    {
        public void AfterBatchQuery(TraceLog log)
        {
            
        }

        public void AfterBulkInsert(TraceLog log)
        {
            
        }

        public void AfterCount(TraceLog log)
        {
            
        }

        public void AfterCountAll(TraceLog log)
        {
            
        }

        public void AfterDelete(TraceLog log)
        {
            
        }

        public void AfterDeleteAll(TraceLog log)
        {
            
        }

        public void AfterExecuteNonQuery(TraceLog log)
        {
            
        }

        public void AfterExecuteQuery(TraceLog log)
        {
            
        }

        public void AfterExecuteReader(TraceLog log)
        {
            
        }

        public void AfterExecuteScalar(TraceLog log)
        {
            
        }

        public void AfterInsert(TraceLog log)
        {
            
        }

        public void AfterInsertAll(TraceLog log)
        {
            
        }

        public void AfterMerge(TraceLog log)
        {
            
        }

        public void AfterQuery(TraceLog log)
        {
            
        }

        public void AfterQueryAll(TraceLog log)
        {
            
        }

        public void AfterQueryMultiple(TraceLog log)
        {
            
        }

        public void AfterTruncate(TraceLog log)
        {
            
        }

        public void AfterUpdate(TraceLog log)
        {
            
        }

        public void BeforeBatchQuery(CancellableTraceLog log)
        {
            
        }

        public void BeforeBulkInsert(CancellableTraceLog log)
        {
            
        }

        public void BeforeCount(CancellableTraceLog log)
        {
            
        }

        public void BeforeCountAll(CancellableTraceLog log)
        {
            
        }

        public void BeforeDelete(CancellableTraceLog log)
        {
            
        }

        public void BeforeDeleteAll(CancellableTraceLog log)
        {
            
        }

        public void BeforeExecuteNonQuery(CancellableTraceLog log)
        {
            
        }

        public void BeforeExecuteQuery(CancellableTraceLog log)
        {
            
        }

        public void BeforeExecuteReader(CancellableTraceLog log)
        {
            
        }

        public void BeforeExecuteScalar(CancellableTraceLog log)
        {
            
        }

        public void BeforeInsert(CancellableTraceLog log)
        {
            
        }

        public void BeforeInsertAll(CancellableTraceLog log)
        {
            Console.WriteLine(log.Statement);
        }

        public void BeforeMerge(CancellableTraceLog log)
        {
            
        }

        public void BeforeQuery(CancellableTraceLog log)
        {
            
        }

        public void BeforeQueryAll(CancellableTraceLog log)
        {
            
        }

        public void BeforeQueryMultiple(CancellableTraceLog log)
        {
            
        }

        public void BeforeTruncate(CancellableTraceLog log)
        {
            
        }

        public void BeforeUpdate(CancellableTraceLog log)
        {
            
        }
    }
}
