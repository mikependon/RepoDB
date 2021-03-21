using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using Dapper;
using Dapper.Contrib.Extensions;
using RepoDb.Benchmarks.PostgreSql.Models;

namespace RepoDb.Benchmarks.PostgreSql.Dapper
{
    public class UpdateAllDapperBenchmarks : DapperBaseBenchmarks
    {
        private readonly List<Person> persons = new ();

        [Params(10, 100, 1000)]
        public int Rows { get; set; }

        protected override void Bootstrap()
        {
            using var connection = GetConnection();

            foreach (var person in connection.GetAll<Person>().Take(Rows))
            {
                person.CreatedDateUtc = DateTime.UtcNow;
                persons.Add(person);
            }
        }

        [Benchmark]
        public void ExecuteUpdateAll()
        {
            using var connection = GetConnection();

            connection.Execute(@"update ""Person"" 
                                set ""CreatedDateUtc"" = @CreatedDateUtc, ""Name"" = @Name, ""Age"" = @Age
                                where ""Id"" = @Id", persons);
        }
    }
}