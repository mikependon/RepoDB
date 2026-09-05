#region Copyright Attributions

// Copyright (c) 2020 SergerGood and Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using NHibernate.Transform;
using RepoDb.Benchmarks.Core.Models;

namespace RepoDb.Benchmarks.SqlServer.NHibernate
{
    public class GetAllNHibernateBenchmarks : NHibernateBaseBenchmarks
    {
        private readonly Consumer consumer = new ();

        [Params(ElementsCount)]
        public int Rows { get; set; }

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