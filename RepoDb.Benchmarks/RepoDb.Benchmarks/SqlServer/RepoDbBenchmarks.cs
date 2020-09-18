using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using BenchmarkDotNet.Attributes;
using RepoDb.Benchmarks.Models;
using RepoDb.Benchmarks.SqlServer.Setup;

namespace RepoDb.Benchmarks.SqlServer
{
    [Description("RepoDB")]
    public class RepoDbBenchmarks : BaseBenchmark
    {
        [GlobalSetup]
        public void Setup()
        {
            SqlServerBootstrap.Initialize();
            TypeMapper.Add(typeof(DateTime), DbType.DateTime2, true);
            BaseSetup();
        }

        public override void Bootstrap()
        {
            using IDbConnection connection = new SqlConnection(DatabaseHelper.ConnectionString).EnsureOpen();

            connection.Query<Person>(x => x.Id == CurrentId);
            connection.QueryAll<Person>();
        }

        [Benchmark]
        public Person QueryLinqFirst()
        {
            using IDbConnection connection = new SqlConnection(DatabaseHelper.ConnectionString).EnsureOpen();

            return connection.Query<Person>(x => x.Id == CurrentId).First();
        }

        [Benchmark]
        public Person QueryDynamicFirst()
        {
            using IDbConnection connection = new SqlConnection(DatabaseHelper.ConnectionString).EnsureOpen();

            return connection.Query<Person>(new { Id = CurrentId }).First();
        }

        [Benchmark]
        public Person QueryObjectsFirst()
        {
            using IDbConnection connection = new SqlConnection(DatabaseHelper.ConnectionString).EnsureOpen();

            QueryField[] where =
            {
                new QueryField(nameof(Person.Id), CurrentId)
            };

            return connection.Query<Person>(where).First();
        }

        [Benchmark]
        public Person ExecuteQueryFirst()
        {
            using IDbConnection connection = new SqlConnection(DatabaseHelper.ConnectionString).EnsureOpen();

            var param = new
            {
                Id = CurrentId
            };

            return connection.ExecuteQuery<Person>("select * from Person where Id = @Id", param).First();
        }
    }
}