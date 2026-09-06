#region Copyright Attributions

// Copyright (c) 2020 SergerGood and Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.ComponentModel;
using BenchmarkDotNet.Attributes;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Mapping.ByCode;
using RepoDb.Benchmarks.EnterpriseDb.NHibernate.Models;
using RepoDb.Benchmarks.EnterpriseDb.Setup;

namespace RepoDb.Benchmarks.EnterpriseDb.NHibernate
{
    [Description("NHibernate")]
    public class NHibernateBaseBenchmarks : EnterpriseDbBenchmark
    {
        protected ISessionFactory SessionFactory;

        [GlobalSetup]
        public void Setup()
        {
            BaseSetup();

            var configuration = new Configuration();
            configuration.DataBaseIntegration(properties =>
            {
                properties.Dialect<PostgreSQL83Dialect>();
                properties.ConnectionString = DatabaseHelper.ConnectionString;
            });

            var mapper = new ModelMapper();
            mapper.AddMapping<PersonMap>();
            var mapping = mapper.CompileMappingForAllExplicitlyAddedEntities();
            configuration.AddMapping(mapping);

            SessionFactory = configuration.BuildSessionFactory();
        }

        protected override void Bootstrap()
        {
            // The compilation is explicity added at the Setup() method
        }
    }
}
