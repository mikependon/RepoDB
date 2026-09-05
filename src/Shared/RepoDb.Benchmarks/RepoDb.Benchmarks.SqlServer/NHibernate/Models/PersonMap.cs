#region Copyright Attributions

// Copyright (c) 2020 SergerGood and Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using NHibernate.Mapping.ByCode.Conformist;
using RepoDb.Benchmarks.Core.Models;

namespace RepoDb.Benchmarks.SqlServer.NHibernate.Models
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
