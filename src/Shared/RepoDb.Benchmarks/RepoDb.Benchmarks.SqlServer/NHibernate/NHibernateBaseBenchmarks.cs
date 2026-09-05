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
using RepoDb.Benchmarks.SqlServer.NHibernate.Models;
using RepoDb.Benchmarks.SqlServer.Setup;

namespace RepoDb.Benchmarks.SqlServer.NHibernate
{
    [Description("NHibernate")]
    public class NHibernateBaseBenchmarks : SqlServerBenchmark
    {
        protected ISessionFactory SessionFactory;

        [GlobalSetup]
        public void Setup()
        {
            BaseSetup();

            var configuration = new Configuration();
            configuration.DataBaseIntegration(properties =>
            {
                properties.Dialect<MsSql2005Dialect>();
                properties.Driver<MicrosoftDataSqlClientDriver>(); // Use Microsoft.Data.SqlClient instead of the legacy System.Data.SqlClient
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
