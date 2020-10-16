using System.Linq;
using BenchmarkDotNet.Attributes;
using NHibernate;
using NHibernate.Transform;
using RepoDb.Benchmarks.Models;

namespace RepoDb.Benchmarks.SqlServer.NHibernate
{
    public class GetFirstNHibernateBenchmarks : NHibernateBaseBenchmarks
    {
        [Benchmark]
        public Person QueryFirst()
        {
            using IStatelessSession session = SessionFactory.OpenStatelessSession();

            return session.Query<Person>().First(x => x.Id == CurrentId);
        }

        [Benchmark]
        public Person CreateSQLQueryFirst()
        {
            using IStatelessSession session = SessionFactory.OpenStatelessSession();

            return session.CreateSQLQuery("select * from Person where Id = :id")
                .SetInt32("id", CurrentId)
                .SetResultTransformer(Transformers.AliasToBean<Person>())
                .List<Person>()[0];
        }
    }
}