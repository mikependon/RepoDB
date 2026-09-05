#region Copyright Attributions

// Copyright (c) 2020 SergerGood and Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using RepoDb.Benchmarks.Db2.Models;

namespace RepoDb.Benchmarks.Db2.NHibernate.Models
{
    public class PersonMap : ClassMapping<Person>
    {
        public PersonMap()
        {
            Table("PERSON");
            Id(x => x.Id, mapper =>
            {
                mapper.Column("ID");
                mapper.Generator(Generators.Identity);
            });
            Property(x => x.Name, mapper => mapper.Column("NAME"));
            Property(x => x.Age, mapper => mapper.Column("AGE"));
            Property(x => x.CreatedDateUtc, mapper => mapper.Column("CREATEDDATEUTC"));
        }
    }
}
