using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using BenchmarkDotNet.Attributes;
using Dapper.Contrib.Extensions;
using Dapper;
using RepoDb.Benchmarks.Models;
using RepoDb.Benchmarks.SqlServer.Setup;

namespace RepoDb.Benchmarks.SqlServer.Dapper
{
    public class UpdateAllDapperBenchmarks : DapperBaseBenchmarks
    {
        private readonly List<Person> persons = new List<Person>();

        [Params(10, 100, 1000)]
        public int Rows { get; set; }

        public override void Bootstrap()
        {
            using IDbConnection connection = new SqlConnection(DatabaseHelper.ConnectionString);
            connection.Open();

            foreach (var person in connection.GetAll<Person>().Take(Rows))
            {
                person.CreatedDateUtc = DateTime.UtcNow;
                persons.Add(person);
            }
        }

        [Benchmark]
        public void ExecuteUpdateAll()
        {
            using IDbConnection connection = new SqlConnection(DatabaseHelper.ConnectionString);
            connection.Open();

            connection.Execute(@"update Person 
                                set CreatedDateUtc = @CreatedDateUtc, Name = @Name, Age = @Age
                                where Id = @Id", persons);
        }
    }
}