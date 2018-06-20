using System;
using RepoDb.Interfaces;
using System.Reflection;

namespace RepoDb.TestProject
{
    public class CustomTrace : ITrace
    {
        public void AfterBatchQuery(TraceLog log)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}: {log.Statement}");
        }

        public void AfterBulkInsert(TraceLog log)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}: {log.Statement}");
        }

        public void AfterCount(TraceLog log)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}: {log.Statement}");
        }

        public void AfterDelete(TraceLog log)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}: {log.Statement}");
        }

        public void AfterExecuteNonQuery(TraceLog log)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}: {log.Statement}");
        }

        public void AfterExecuteQuery(TraceLog log)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}: {log.Statement}");
        }

        public void AfterExecuteReader(TraceLog log)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}: {log.Statement}");
        }

        public void AfterExecuteScalar(TraceLog log)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}: {log.Statement}");
        }

        public void AfterInlineUpdate(TraceLog log)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}: {log.Statement}");
        }

        public void AfterInsert(TraceLog log)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}: {log.Statement}");
        }

        public void AfterMerge(TraceLog log)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}: {log.Statement}");
        }

        public void AfterQuery(TraceLog log)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}: {log.Statement}");
        }

        public void AfterUpdate(TraceLog log)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}: {log.Statement}");
        }

        public void BeforeBatchQuery(CancellableTraceLog log)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}: {log.Statement}");
        }

        public void BeforeBulkInsert(CancellableTraceLog log)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}: {log.Statement}");
        }

        public void BeforeCount(CancellableTraceLog log)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}: {log.Statement}");
        }

        public void BeforeDelete(CancellableTraceLog log)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}: {log.Statement}");
        }

        public void BeforeExecuteNonQuery(CancellableTraceLog log)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}: {log.Statement}");
        }

        public void BeforeExecuteQuery(CancellableTraceLog log)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}: {log.Statement}");
        }

        public void BeforeExecuteReader(CancellableTraceLog log)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}: {log.Statement}");
        }

        public void BeforeExecuteScalar(CancellableTraceLog log)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}: {log.Statement}");
        }

        public void BeforeInlineUpdate(CancellableTraceLog log)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}: {log.Statement}");
        }

        public void BeforeInsert(CancellableTraceLog log)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}: {log.Statement}");
        }

        public void BeforeMerge(CancellableTraceLog log)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}: {log.Statement}");
        }

        public void BeforeQuery(CancellableTraceLog log)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}: {log.Statement}");
        }

        public void BeforeUpdate(CancellableTraceLog log)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}: {log.Statement}");
        }
    }
}
