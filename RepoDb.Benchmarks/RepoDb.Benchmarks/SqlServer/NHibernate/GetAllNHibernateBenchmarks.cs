using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using NHibernate;
using NHibernate.Transform;
using RepoDb.Benchmarks.Models;

namespace RepoDb.Benchmarks.SqlServer.NHibernate
{
    public class GetAllNHibernateBenchmarks : NHibernateBaseBenchmarks
    {
        private readonly Consumer consumer = new Consumer();

        [Benchmark]
        public void QueryAll()
        {
            using IStatelessSession session = SessionFactory.OpenStatelessSession();

            session.Query<Person>().Consume(consumer);
        }

        [Benchmark]
        public void CreateSQLQueryAll()
        {
            using IStatelessSession session = SessionFactory.OpenStatelessSession();

            session.CreateSQLQuery("select * from Person")
                .SetResultTransformer(Transformers.AliasToBean<Person>())
                .List<Person>().Consume(consumer);
        }
    }
}