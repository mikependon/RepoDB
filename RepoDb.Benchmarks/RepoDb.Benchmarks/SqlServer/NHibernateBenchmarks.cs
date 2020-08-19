using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
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
        public async Task<Person> FirstAsync()
        {
            using IStatelessSession session = sessionFactory.OpenStatelessSession();

            return await session.Query<Person>().FirstAsync(x => x.Id == CurrentId);
        }

        [Benchmark]
        public Person First()
        {
            using IStatelessSession session = sessionFactory.OpenStatelessSession();

            return session.Query<Person>().First(x => x.Id == CurrentId);
        }
    }
}