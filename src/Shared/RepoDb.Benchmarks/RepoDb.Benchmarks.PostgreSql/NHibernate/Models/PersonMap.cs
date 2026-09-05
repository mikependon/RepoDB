#region Copyright Attributions

// Copyright (c) 2026 SergerGood and Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using RepoDb.Benchmarks.PostgreSql.Models;

namespace RepoDb.Benchmarks.PostgreSql.NHibernate.Models
{
    public class PersonMap : ClassMapping<Person>
    {
        public PersonMap()
        {
            Table("\"Person\"");
            Id(x => x.Id, mapper =>
            {
                mapper.Column("\"Id\"");
                mapper.Generator(Generators.Identity);
            });
            Property(x => x.Name, mapper => mapper.Column(x => x.Name("\"Name\"")));
            Property(x => x.Age, mapper => mapper.Column(x => x.Name("\"Age\"")));
            Property(x => x.CreatedDateUtc, mapper => mapper.Column(x => x.Name("\"CreatedDateUtc\"")));
        }
    }
}
