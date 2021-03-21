using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using RepoDb.Benchmarks.PostgreSql.Models;

namespace RepoDb.Benchmarks.PostgreSql.RepoDb
{
    public class UpdateAllRepoDbBenchmarks : RepoDbBaseBenchmarks
    {
        private readonly List<Person> persons = new();

        [Params(10, 100, 1000)] 
        public int Rows { get; set; }

        protected override void Bootstrap()
        {
            using var connection = GetConnection().EnsureOpen();

            foreach (var person in connection.QueryAll<Person>().Take(Rows))
            {
                person.CreatedDateUtc = DateTime.UtcNow;
                persons.Add(person);
            }
        }

        [Benchmark]
        public void UpdateAll()
        {
            using var connection = GetConnection().EnsureOpen();

            connection.UpdateAll(persons);
        }
    }
}