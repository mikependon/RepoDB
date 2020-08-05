using NHibernate.Mapping.ByCode.Conformist;

namespace RepoDb.Benchmarks.Models
{
    public class PersonMap : ClassMapping<Person>
    {
        public PersonMap()
        {
            Id(x => x.Id);
            Property(x => x.Name);
            Property(x => x.Age);
            Property(x => x.CreatedDateUtc);
        }
    }
}