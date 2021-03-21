using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using NHibernate.Transform;
using RepoDb.Benchmarks.SqlServer.Models;

namespace RepoDb.Benchmarks.SqlServer.NHibernate
{
    public class GetAllNHibernateBenchmarks : NHibernateBaseBenchmarks
    {
        private readonly Consumer consumer = new ();

        [Benchmark]
        public void QueryAll()
        {
            using var session = SessionFactory.OpenStatelessSession();

            session.Query<Person>().Consume(consumer);
        }

        [Benchmark]
        public void CreateSQLQueryAll()
        {
            using var session = SessionFactory.OpenStatelessSession();

            session.CreateSQLQuery("select * from Person")
                .SetResultTransformer(Transformers.AliasToBean<Person>())
                .List<Person>().Consume(consumer);
        }
    }
}