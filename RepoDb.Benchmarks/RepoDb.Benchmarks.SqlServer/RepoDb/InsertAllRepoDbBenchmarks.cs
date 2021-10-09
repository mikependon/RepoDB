using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Microsoft.Data.SqlClient;
using RepoDb.Benchmarks.SqlServer.Models;

namespace RepoDb.Benchmarks.SqlServer.RepoDb
{
    public class InsertAllRepoDbBenchmarks : RepoDbBaseBenchmarks
    {
        [Params(10, 100, 1000)] 
        public int Rows { get; set; }

        [Benchmark]
        public void BulkInsertAll()
        {
            using var connection = GetConnection().EnsureOpen() as SqlConnection;

            var persons = GetPersons(Rows);
            connection.BulkInsert(persons);
        }
        
        [Benchmark]
        public void InsertAll()
        {
            using var connection = GetConnection().EnsureOpen();

            var persons = GetPersons(Rows);
            connection.InsertAll(persons);
        }
        
        [Benchmark]
        public async Task BulkInsertAllAsync()
        {
            await using var connection = await GetConnection().EnsureOpenAsync() as SqlConnection;

            var persons = GetPersons(Rows);
            await connection.BulkInsertAsync(persons);
        }
        
        [Benchmark]
        public async Task BulkInsertAllAsyncEnumerable()
        {
            await using var connection = await GetConnection().EnsureOpenAsync() as SqlConnection;

            var persons = GetPersons(Rows).ToAsyncEnumerable();
            await connection.BulkInsertAsync(persons);
        }
        
        private static IEnumerable<Person> GetPersons(int count)
        {
            for (var i = 0; i < count; i++)
            {
                yield return new Person
                {
                    Id = i,
                    Name = $"Person-{i}",
                    Age = i + 1,
                    CreatedDateUtc= DateTime.UtcNow
                };
            }
        }
    }
}