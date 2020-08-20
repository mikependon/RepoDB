using System.ComponentModel;
using System.Linq;
using BenchmarkDotNet.Attributes;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Mapping.ByCode;
using NHibernate.Transform;
using RepoDb.Benchmarks.Models;
using RepoDb.Benchmarks.SqlServer.Setup;

namespace RepoDb.Benchmarks.SqlServer
{
    [Description("NHibernate 5.3.2")]
    public class NHibernateBenchmarks : BaseBenchmark
    {
        private ISessionFactory sessionFactory;

        [GlobalSetup]
        public void Setup()
        {
            BaseSetup();

            var configuration = new Configuration();
            configuration.DataBaseIntegration(properties =>
            {
                properties.Dialect<MsSql2005Dialect>();
                properties.ConnectionString = DatabaseHelper.ConnectionString;
            });

            var mapper = new ModelMapper();
            mapper.AddMapping<PersonMap>();
            var mapping = mapper.CompileMappingForAllExplicitlyAddedEntities();
            configuration.AddMapping(mapping);

            sessionFactory = configuration.BuildSessionFactory();
        }

        [Benchmark]
        public Person QueryFirst()
        {
            using IStatelessSession session = sessionFactory.OpenStatelessSession();

            return session.Query<Person>().First(x => x.Id == CurrentId);
        }

        [Benchmark]
        public Person CreateSQLQueryFirst()
        {
            using IStatelessSession session = sessionFactory.OpenStatelessSession();

            return session.CreateSQLQuery("select * from Person where Id = :id")
                .SetInt32("id", CurrentId)
                .SetResultTransformer(Transformers.AliasToBean<Person>())
                .List<Person>()[0];
        }
    }
}