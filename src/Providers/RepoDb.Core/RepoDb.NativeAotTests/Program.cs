#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using RepoDb.Attributes;
using RepoDb.Enumerations;
using RepoDb.Interfaces;
using RepoDb.Options;

namespace RepoDb.NativeAotTests
{
    public enum PersonKind
    {
        Unknown = 0,
        Employee = 1,
        Contractor = 2
    }

    [Map("Person")]
    public class Person
    {
        [Primary, Identity]
        public long Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string CreatedUtc { get; set; }
        public string ExternalId { get; set; }
        public byte[] Blob { get; set; }
        public decimal? Salary { get; set; }
        public double? Score { get; set; }
        public long IsActive { get; set; }
        public PersonKind Kind { get; set; }
        public PersonKind? OptionalKind { get; set; }
        [PropertyHandler(typeof(UpperCaseHandler))]
        public string Nickname { get; set; }
    }

    [Map("Person")]
    public class PersonSlim
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }

    // Only used with ConversionType.Automatic, as SQLite returns these as TEXT.
    [Map("TypedRow")]
    public class TypedRow
    {
        public long Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Modified { get; set; }
        public Guid Key { get; set; }
        public Guid? OptionalKey { get; set; }
        public decimal Amount { get; set; }
        public short Small { get; set; }
        public float Ratio { get; set; }
        public bool Flag { get; set; }
    }

    // Constructor-materialized type.
    public record PersonRecord(long Id, string Name, int Age);

    public class UpperCaseHandler : IPropertyHandler<string, string>
    {
        public string Get(string input, PropertyHandlerGetOptions options) => input?.ToUpperInvariant();

        public string Set(string input, PropertyHandlerSetOptions options) => input?.ToLowerInvariant();
    }

    public static class Program
    {
        private static int failures;
        private static int passes;
        private static readonly DateTime created = new(2024, 5, 6, 7, 8, 9, DateTimeKind.Utc);
        private static long firstId;

        private static string Stamp(DateTime value) => value.ToString("o");

        public static async Task<int> Main(string[] args)
        {
            var automatic = args.Contains("--automatic");
            Console.WriteLine($"ConversionType: {(automatic ? ConversionType.Automatic : ConversionType.Default)}");
            GlobalConfiguration
                .Setup(new GlobalConfigurationOptions { ConversionType = automatic ? ConversionType.Automatic : ConversionType.Default })
                .UseSqlite();

            using var connection = new SqliteConnection("Data Source=:memory:;");
            connection.Open();

            CreateTables(connection);
            await RunGenericApisAsync(connection, automatic);
            RunObjectApisWithSafeArguments(connection);
            Cleanup(connection);

            Console.WriteLine();
            Console.WriteLine($"{passes} passed, {failures} failed.");
            return failures == 0 ? 0 : 1;
        }

        [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "No parameter object is passed.")]
        private static void CreateTables(SqliteConnection connection)
        {
            Run("ExecuteNonQuery (create table)", () =>
            {
                connection.ExecuteNonQuery(@"CREATE TABLE [Person]
                    (
                        Id INTEGER PRIMARY KEY,
                        Name TEXT,
                        Age INTEGER,
                        CreatedUtc TEXT,
                        ExternalId TEXT,
                        Blob BLOB,
                        Salary DECIMAL,
                        Score REAL,
                        IsActive INTEGER,
                        Kind INTEGER,
                        OptionalKind INTEGER,
                        Nickname TEXT
                    );");
                connection.ExecuteNonQuery("CREATE TABLE [TypedRow] (Id INTEGER PRIMARY KEY, Created TEXT, Modified TEXT, Key TEXT, OptionalKey TEXT, Amount DECIMAL, Small INTEGER, Ratio REAL, Flag BOOLEAN);");
            });
        }

        /// <summary>
        /// The generic, lambda expression and typed (QueryField/QueryGroup) based operations, which are AOT-safe (no warnings).
        /// </summary>
        private static async Task RunGenericApisAsync(SqliteConnection connection, bool automatic)
        {
            var externalId = Guid.NewGuid().ToString();

            Run("Insert<TEntity>", () =>
            {
                firstId = connection.Insert<Person, long>(new Person
                {
                    Name = "Alice",
                    Age = 30,
                    CreatedUtc = Stamp(created),
                    ExternalId = externalId,
                    Blob = new byte[] { 1, 2, 3 },
                    Salary = 1234m,
                    Score = 9.5,
                    IsActive = 1,
                    Kind = PersonKind.Employee,
                    OptionalKind = PersonKind.Contractor,
                    Nickname = "Ally"
                });
                Assert(firstId > 0, "identity was not returned");
            });

            Run("InsertAll<TEntity>", () =>
            {
                var people = Enumerable.Range(1, 10).Select(i => new Person
                {
                    Name = $"Person{i}",
                    Age = 20 + i,
                    CreatedUtc = Stamp(created.AddDays(i)),
                    ExternalId = Guid.NewGuid().ToString(),
                    IsActive = i % 2 == 0 ? 1 : 0,
                    Kind = (PersonKind)(i % 3)
                }).ToList();
                var inserted = connection.InsertAll(people);
                Assert(inserted == 10, $"expected 10 inserted rows, got {inserted}");
                Assert(people.All(p => p.Id > 0), "identities were not set back");
            });

            Run("Query<TEntity>(expression)", () =>
            {
                var alice = connection.Query<Person>(e => e.Id == firstId).Single();
                Assert(alice.Name == "Alice", "Name");
                Assert(alice.Age == 30, "Age");
                Assert(alice.CreatedUtc == Stamp(created), $"CreatedUtc {alice.CreatedUtc}");
                Assert(alice.ExternalId == externalId, "ExternalId");
                Assert(alice.Blob.SequenceEqual(new byte[] { 1, 2, 3 }), "Blob");
                Assert(alice.Salary == 1234m, $"Salary {alice.Salary}");
                Assert(alice.Score == 9.5, "Score");
                Assert(alice.IsActive == 1, "IsActive");
                Assert(alice.Kind == PersonKind.Employee, "Kind");
                Assert(alice.OptionalKind == PersonKind.Contractor, "OptionalKind");
                Assert(alice.Nickname == "ALLY", $"Nickname (property handler) {alice.Nickname}");
            });

            Run("Query<TEntity>(complex expression)", () =>
            {
                var names = new[] { "Person1", "Person2", "Person3" };
                var result = connection.Query<Person>(e => names.Contains(e.Name) && e.Age > 21 && e.Name.StartsWith("Person")).ToList();
                Assert(result.Count == 2, $"expected 2, got {result.Count}");
            });

            Run("Query<TEntity>(enum expression)", () =>
            {
                var kinds = new[] { PersonKind.Contractor };
                var result = connection.Query<Person>(e => e.Kind == PersonKind.Contractor || kinds.Contains(e.Kind)).ToList();
                Assert(result.Count == 3, $"expected 3, got {result.Count}");
            });

            Run("Query<TEntity>(QueryGroup)", () =>
            {
                var result = connection.Query<Person>(new QueryGroup(new[]
                {
                    new QueryField("Age", Operation.GreaterThanOrEqual, 25),
                    new QueryField("Age", Operation.LessThan, 28)
                })).ToList();
                Assert(result.Count == 3, $"expected 3, got {result.Count}");
            });

            Run("Query<TEntity>(fields, orderBy, top)", () =>
            {
                var result = connection.Query<Person>(e => e.Age > 0,
                    fields: Field.From("Id", "Name", "Age"),
                    orderBy: new[] { OrderField.Descending<Person>(e => e.Age) },
                    top: 2).ToList();
                Assert(result.Count == 2, $"top {result.Count}");
                Assert(result[0].Age == 30 && result[1].Age == 30, "descending order");
                Assert(result[0].CreatedUtc == null, "fields projection");
            });

            Run("QueryAll<TEntity>(orderBy)", () =>
            {
                var result = connection.QueryAll<Person>(orderBy: new[] { OrderField.Ascending<Person>(e => e.Age) }).ToList();
                Assert(result.Count == 11 && result[0].Age == 21, "QueryAll");
            });

            Run("QueryAll<TEntity> (subset class)", () =>
            {
                var result = connection.QueryAll<PersonSlim>().ToList();
                Assert(result.Count == 11 && result.Any(p => p.Name == "Alice"), "subset class");
            });

            Run("Update<TEntity>", () =>
            {
                var alice = connection.Query<Person>(e => e.Id == firstId).Single();
                alice.Age = 32;
                alice.Salary = null;
                var updated = connection.Update(alice);
                Assert(updated == 1, "Update count");
                var reloaded = connection.Query<Person>(e => e.Id == firstId).Single();
                Assert(reloaded.Age == 32 && reloaded.Salary == null, "Update values");
            });

            Run("UpdateAll<TEntity>", () =>
            {
                var people = connection.Query<Person>(e => e.Name.StartsWith("Person")).ToList();
                people.ForEach(p => p.Score = 1.0);
                var updated = connection.UpdateAll(people);
                Assert(updated == 10, $"expected 10, got {updated}");
            });

            Run("Merge<TEntity>", () =>
            {
                var bob = new Person { Name = "Bob", Age = 40, CreatedUtc = Stamp(created), ExternalId = Guid.NewGuid().ToString() };
                var id = connection.Merge<Person, long>(bob);
                Assert(id > 0, "Merge insert");
                bob.Id = id;
                bob.Age = 41;
                connection.Merge(bob);
                Assert(connection.Query<Person>(e => e.Id == id).Single().Age == 41, "Merge update");
            });

            Run("Count / Exists / Sum / Max / Min / Average", () =>
            {
                Assert(connection.CountAll<Person>() == 12, "CountAll");
                Assert(connection.Count<Person>(e => e.IsActive == 1) == 6, "Count");
                Assert(connection.Exists<Person>(e => e.Name == "Bob"), "Exists");
                Assert(Convert.ToInt64(connection.Sum<Person>(e => e.Age, e => e.Name.StartsWith("Person"))) == 255, "Sum");
                Assert(Convert.ToInt64(connection.Max<Person>(e => e.Age, e => e.Age > 0)) == 41, "Max");
                Assert(Convert.ToInt64(connection.MinAll<Person>(e => e.Age)) == 21, "MinAll");
                Assert(Convert.ToDouble(connection.AverageAll<Person>(e => e.Age)) > 0, "AverageAll");
            });

            Run("BatchQuery<TEntity>", () =>
            {
                var page = connection.BatchQuery<Person>(page: 1, rowsPerBatch: 3,
                    orderBy: new[] { OrderField.Ascending<Person>(e => e.Id) },
                    where: e => e.Age > 0).ToList();
                Assert(page.Count == 3, $"BatchQuery {page.Count}");
            });

            Run("QueryMultiple<T1, T2>", () =>
            {
                var result = connection.QueryMultiple<Person, PersonSlim>(e => e.Id == firstId, e => e.Name == "Bob");
                Assert(result.Item1.Single().Name == "Alice", "QueryMultiple 1");
                Assert(result.Item2.Single().Name == "Bob", "QueryMultiple 2");
            });

            Run("Insert<Dictionary>(tableName)", () =>
            {
                var id = connection.Insert<Dictionary<string, object>, long>("Person", new Dictionary<string, object> { ["Name"] = "Dave", ["Age"] = 60 });
                Assert(id > 0, "Insert(dictionary)");
            });

            Run("Transaction", () =>
            {
                using var transaction = connection.BeginTransaction();
                connection.Insert(new Person { Name = "Rollback", Age = 1, CreatedUtc = Stamp(created) }, transaction: transaction);
                transaction.Rollback();
                Assert(!connection.Exists<Person>(e => e.Name == "Rollback"), "Rollback");
            });

            if (automatic)
            {
                Run("Automatic conversion (DateTime, Guid, decimal, short, float, bool)", () =>
                {
                    var key = Guid.NewGuid();
                    var id = connection.Insert<TypedRow, long>(new TypedRow { Created = created, Modified = null, Key = key, OptionalKey = key, Amount = 12m, Small = 7, Ratio = 0.5f, Flag = true });
                    var row = connection.Query<TypedRow>(e => e.Id == id).Single();
                    Assert(row.Created == created, $"Created {row.Created:o}");
                    Assert(row.Modified == null, "Modified");
                    Assert(row.Key == key && row.OptionalKey == key, "Key");
                    Assert(row.Amount == 12m && row.Small == 7 && row.Ratio == 0.5f && row.Flag, $"numerics {row.Amount} {row.Small} {row.Ratio}");
                });
            }

            await RunAsync("Async operations", async () =>
            {
                var id = await connection.InsertAsync<Person, long>(new Person { Name = "Async", Age = 5, CreatedUtc = Stamp(created) });
                var fetched = (await connection.QueryAsync<Person>(e => e.Id == id)).Single();
                Assert(fetched.Name == "Async", "QueryAsync");
                var count = await connection.CountAsync<Person>(e => e.Name == "Async");
                Assert(count == 1, "CountAsync");
                var deleted = await connection.DeleteAsync<Person>(e => e.Id == id);
                Assert(deleted == 1, "DeleteAsync");
            });

            Run("Delete<TEntity>", () =>
            {
                Assert(connection.Delete<Person>(e => e.Name == "Bob") == 1, "Delete(expression)");
                var alice = connection.Query<Person>(e => e.Id == firstId).Single();
                Assert(connection.Delete(alice) == 1, "Delete(entity)");
            });
        }

        /// <summary>
        /// The object-based operations (annotated with RequiresUnreferencedCode), used with the arguments that are not reflected
        /// (null, dictionaries, ExpandoObject, primitive key values, QueryField/QueryGroup), as documented in the warning message.
        /// </summary>
        [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Only null, dictionaries, ExpandoObject and primitive values are passed.")]
        private static void RunObjectApisWithSafeArguments(SqliteConnection connection)
        {
            var id = connection.ExecuteScalar<long>("SELECT Id FROM [Person] WHERE Name = @Name;", new Dictionary<string, object> { ["Name"] = "Person1" });

            Run("Query<TEntity>(primary key value)", () =>
            {
                Assert(connection.Query<Person>(id).Single().Name == "Person1", "primary key query");
            });

            Run("ExecuteQuery<TEntity>(dictionary param)", () =>
            {
                var result = connection.ExecuteQuery<Person>("SELECT * FROM [Person] WHERE Name = @Name;",
                    new Dictionary<string, object> { ["Name"] = "Person2" }).Single();
                Assert(result.Age == 22, "ExecuteQuery<T>(dictionary)");
            });

            Run("ExecuteQuery<TEntity>(array param)", () =>
            {
                var result = connection.ExecuteQuery<Person>("SELECT * FROM [Person] WHERE Name IN (@Names);",
                    new Dictionary<string, object> { ["Names"] = new[] { "Person1", "Person2" } }).ToList();
                Assert(result.Count == 2, $"ExecuteQuery<T>(array) {result.Count}");
            });

            Run("ExecuteQuery<record>(constructor)", () =>
            {
                var result = connection.ExecuteQuery<PersonRecord>("SELECT Id, Name, Age FROM [Person] WHERE Id = @Id;",
                    new Dictionary<string, object> { ["Id"] = id }).Single();
                Assert(result.Name == "Person1" && result.Age == 21, "ExecuteQuery<record>");
            });

            Run("ExecuteQuery<long>(scalar list)", () =>
            {
                var result = connection.ExecuteQuery<long>("SELECT Id FROM [Person] ORDER BY Id;").ToList();
                Assert(result.Count > 0 && result.Contains(id), "ExecuteQuery<long>");
            });

            Run("ExecuteQuery<string>(scalar list)", () =>
            {
                var result = connection.ExecuteQuery<string>("SELECT Name FROM [Person] ORDER BY Id;").ToList();
                Assert(result.Contains("Person1"), "ExecuteQuery<string>");
            });

            Run("ExecuteQuery (dynamic)", () =>
            {
                object result = connection.ExecuteQuery("SELECT * FROM [Person] WHERE Id = @Id;", new Dictionary<string, object> { ["Id"] = id }).Single();
                Assert((string)((IDictionary<string, object>)result)["Name"] == "Person1", "ExecuteQuery dynamic");
            });

            Run("ExecuteScalar<T>", () =>
            {
                var count = connection.ExecuteScalar<long>("SELECT COUNT(*) FROM [Person] WHERE Age > @Age;", new Dictionary<string, object> { ["Age"] = 25 });
                Assert(count > 0, $"ExecuteScalar {count}");
                var name = connection.ExecuteScalar<string>("SELECT Name FROM [Person] WHERE Id = @Id;", new Dictionary<string, object> { ["Id"] = id });
                Assert(name == "Person1", "ExecuteScalar<string>");
            });

            Run("ExecuteQueryMultiple", () =>
            {
                using var extractor = connection.ExecuteQueryMultiple("SELECT * FROM [Person] WHERE Id = @Id; SELECT COUNT(*) FROM [Person];",
                    new Dictionary<string, object> { ["Id"] = id });
                var people = extractor.Extract<Person>().ToList();
                var count = extractor.Scalar<long>();
                Assert(people.Count == 1 && count > 0, "ExecuteQueryMultiple");
            });

            Run("Query(tableName, QueryGroup)", () =>
            {
                object result = connection.Query("Person", new QueryGroup(new QueryField("Name", "Person3"))).Single();
                Assert((string)((IDictionary<string, object>)result)["Name"] == "Person3", "Query(tableName)");
            });

            Run("Insert(tableName, ExpandoObject)", () =>
            {
                IDictionary<string, object> expando = new ExpandoObject();
                expando["Name"] = "Carol";
                expando["Age"] = 50;
                var newId = connection.Insert<long>("Person", expando);
                Assert(newId > 0, "Insert(expando)");
            });

            Run("Update(tableName, Dictionary)", () =>
            {
                var updated = connection.Update("Person", new Dictionary<string, object> { ["Id"] = id, ["Age"] = 99 });
                Assert(updated == 1, "Update(table) count");
                Assert(connection.Query<Person>(e => e.Id == id).Single().Age == 99, "Update(table) value");
            });

            Run("Delete(tableName, QueryGroup)", () =>
            {
                Assert(connection.Delete("Person", new QueryGroup(new QueryField("Name", "Carol"))) == 1, "Delete(tableName)");
            });
        }

        private static void Cleanup(SqliteConnection connection)
        {
            Run("DeleteAll<TEntity>", () =>
            {
                var deleted = connection.DeleteAll<Person>();
                Assert(deleted > 0, $"DeleteAll {deleted}");
                Assert(connection.CountAll<Person>() == 0, "CountAll after DeleteAll");
            });
        }

        private static void Assert(bool condition, string message)
        {
            if (!condition)
            {
                throw new InvalidOperationException($"Assertion failed: {message}");
            }
        }

        private static void Run(string name, Action action)
        {
            try
            {
                action();
                passes++;
                Console.WriteLine($"[PASS] {name}");
            }
            catch (Exception ex)
            {
                failures++;
                Console.WriteLine($"[FAIL] {name}: {ex}");
            }
        }

        private static async Task RunAsync(string name, Func<Task> action)
        {
            try
            {
                await action();
                passes++;
                Console.WriteLine($"[PASS] {name}");
            }
            catch (Exception ex)
            {
                failures++;
                Console.WriteLine($"[FAIL] {name}: {ex}");
            }
        }
    }
}
