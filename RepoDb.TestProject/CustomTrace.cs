using System;
using RepoDb.Interfaces;
using System.Reflection;

namespace RepoDb.TestProject
{
    public class CustomTrace : ITrace
    {
        public void AfterBatchQuery(ITraceLog log)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}: {log.Statement}");
        }

        public void AfterBulkInsert(ITraceLog log)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}: {log.Statement}");
        }

        public void AfterCount(ITraceLog log)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}: {log.Statement}");
        }

        public void AfterCountBig(ITraceLog log)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}: {log.Statement}");
        }

        public void AfterDelete(ITraceLog log)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}: {log.Statement}");
        }

        public void AfterExecuteNonQuery(ITraceLog log)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}: {log.Statement}");
        }

        public void AfterExecuteQuery(ITraceLog log)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}: {log.Statement}");
        }

        public void AfterExecuteReader(ITraceLog log)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}: {log.Statement}");
        }

        public void AfterExecuteScalar(ITraceLog log)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}: {log.Statement}");
        }

        public void AfterInlineUpdate(ITraceLog log)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}: {log.Statement}");
        }

        public void AfterInsert(ITraceLog log)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}: {log.Statement}");
        }

        public void AfterMerge(ITraceLog log)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}: {log.Statement}");
        }

        public void AfterQuery(ITraceLog log)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}: {log.Statement}");
        }

        public void AfterUpdate(ITraceLog log)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}: {log.Statement}");
        }

        public void BeforeBatchQuery(ICancellableTraceLog log)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}: {log.Statement}");
        }

        public void BeforeBulkInsert(ICancellableTraceLog log)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}: {log.Statement}");
        }

        public void BeforeCount(ICancellableTraceLog log)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}: {log.Statement}");
        }

        public void BeforeCountBig(ICancellableTraceLog log)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}: {log.Statement}");
        }

        public void BeforeDelete(ICancellableTraceLog log)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}: {log.Statement}");
        }

        public void BeforeExecuteNonQuery(ICancellableTraceLog log)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}: {log.Statement}");
        }

        public void BeforeExecuteQuery(ICancellableTraceLog log)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}: {log.Statement}");
        }

        public void BeforeExecuteReader(ICancellableTraceLog log)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}: {log.Statement}");
        }

        public void BeforeExecuteScalar(ICancellableTraceLog log)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}: {log.Statement}");
        }

        public void BeforeInlineUpdate(ICancellableTraceLog log)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}: {log.Statement}");
        }

        public void BeforeInsert(ICancellableTraceLog log)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}: {log.Statement}");
        }

        public void BeforeMerge(ICancellableTraceLog log)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}: {log.Statement}");
        }

        public void BeforeQuery(ICancellableTraceLog log)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}: {log.Statement}");
        }

        public void BeforeUpdate(ICancellableTraceLog log)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod().Name}: {log.Statement}");
        }
    }
}
