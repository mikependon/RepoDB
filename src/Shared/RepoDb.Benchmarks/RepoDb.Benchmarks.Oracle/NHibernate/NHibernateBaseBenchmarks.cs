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
using NHibernate.Driver;
using NHibernate.Mapping.ByCode;
using RepoDb.Benchmarks.Oracle.NHibernate.Models;
using RepoDb.Benchmarks.Oracle.Setup;

namespace RepoDb.Benchmarks.Oracle.NHibernate
{
    [Description("NHibernate")]
    public class NHibernateBaseBenchmarks : OracleBenchmark
    {
        protected ISessionFactory SessionFactory;

        [GlobalSetup]
        public void Setup()
        {
            BaseSetup();

            var configuration = new Configuration();
            configuration.DataBaseIntegration(properties =>
            {
                properties.Dialect<Oracle12cDialect>();
                properties.Driver<OracleManagedDataClientDriver>();
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
