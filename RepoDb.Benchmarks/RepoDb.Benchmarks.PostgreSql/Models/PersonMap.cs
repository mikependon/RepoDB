using NHibernate.Mapping.ByCode.Conformist;

namespace RepoDb.Benchmarks.PostgreSql.Models
{
    public class PersonMap : ClassMapping<Person>
    {
        public PersonMap()
        {
            Table("\"Person\"");
            Id(x => x.Id, mapper => mapper.Column(x => x.Name("\"Id\"")));
            Property(x => x.Name, mapper => mapper.Column(x => x.Name("\"Name\"")));
            Property(x => x.Age, mapper => mapper.Column(x => x.Name("\"Age\"")));
            Property(x => x.CreatedDateUtc, mapper => mapper.Column(x => x.Name("\"CreatedDateUtc\"")));
        }
    }
}