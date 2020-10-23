using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using BenchmarkDotNet.Attributes;
using RepoDb.Benchmarks.Models;
using RepoDb.Benchmarks.SqlServer.Setup;

namespace RepoDb.Benchmarks.SqlServer.RepoDb
{
    public class UpdateAllRepoDbBenchmarks : RepoDbBaseBenchmarks
    {
        private readonly List<Person> persons = new List<Person>();

        [Params(10, 100, 1000)]
        public int Rows { get; set; }

        public override void Bootstrap()
        {
            using IDbConnection connection = new SqlConnection(DatabaseHelper.ConnectionString).EnsureOpen();

            foreach (var person in connection.QueryAll<Person>().Take(Rows))
            {
                person.CreatedDateUtc = DateTime.UtcNow;
                persons.Add(person);
            }
        }

        [Benchmark]
        public void UpdateAll()
        {
            using IDbConnection connection = new SqlConnection(DatabaseHelper.ConnectionString).EnsureOpen();

            connection.UpdateAll(persons);
        }

        [Benchmark]
        public void BulkUpdateAll()
        {
            using SqlConnection connection = new SqlConnection(DatabaseHelper.ConnectionString);

            connection.BulkUpdate(persons);
        }
    }
}