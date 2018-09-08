using RepoDb.Extensions;
using System;
using Dapper;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using RepoDb;
using RepoDb.Enumerations;

namespace Project_ORM_Perf_Benchmark
{
    class Program
    {
        public const string ConnectionString = @"Server=.;Database=TestDb;Integrated Security=SSPI;";

        static void Main(string[] args)
        {
            // Standby
            while (true)
            {
                Console.WriteLine("[List of Options]");
                Console.WriteLine(new string(char.Parse("-"), 35));
                Console.WriteLine("1. Compare the performance of the Insert operation.");
                Console.WriteLine("2. Compare the performance of the Query operation.");
                Console.WriteLine("3. Insert some records in the database.");
                Console.WriteLine("4. Delete all data from the database.");
                Console.WriteLine("5. Exit");
                Console.WriteLine(new string(char.Parse("-"), 35));

                Console.Write("Select an Option: ");
                var line = Console.ReadLine();

                if (line == 1.ToString())
                {
                    var rows = 0;
                    var b = false;
                    while (!b)
                    {
                        Console.Clear();
                        Console.WriteLine("[Compare Insert Operation]");
                        Console.WriteLine(new string(char.Parse("-"), 35));
                        Console.Write("No. of rows to Insert: ");
                        line = Console.ReadLine();
                        b = int.TryParse(line, out rows);
                        if (!b)
                        {
                            Console.WriteLine("Invalid value inputted.");
                        }
                    }
                    Console.WriteLine(new string(char.Parse("-"), 35));
                    CompareInsert(rows);
                }
                else if (line == 2.ToString())
                {
                    var rows = 0;
                    var b = false;
                    while (!b)
                    {
                        Console.Clear();
                        Console.WriteLine("[Compare Query Operation]");
                        Console.WriteLine(new string(char.Parse("-"), 35));
                        Console.Write("Number of rows to Query: ");
                        line = Console.ReadLine();
                        b = int.TryParse(line, out rows);
                        if (!b)
                        {
                            Console.WriteLine("Invalid value inputted.");
                        }
                    }
                    Console.WriteLine(new string(char.Parse("-"), 35));
                    CompareQuery(rows);
                }
                else if (line == 3.ToString())
                {
                    var rows = 0;
                    var b = false;
                    while (!b)
                    {
                        Console.Clear();
                        Console.WriteLine("[Insert Records]");
                        Console.WriteLine(new string(char.Parse("-"), 35));
                        Console.Write("Number of rows to Insert: ");
                        line = Console.ReadLine();
                        b = int.TryParse(line, out rows);
                        if (!b)
                        {
                            Console.WriteLine("Invalid value inputted.");
                        }
                    }
                    Console.WriteLine(new string(char.Parse("-"), 35));
                    InsertAll(rows);
                }
                else if (line == 4.ToString())
                {
                    Console.Clear();
                    Console.WriteLine("[Delete All Records]");
                    Console.WriteLine(new string(char.Parse("-"), 35));
                    DeleteAll();
                }
                else if (line == 5.ToString())
                {
                    break;
                }
                else
                {
                    Console.Clear();
                    continue;
                }
                Console.WriteLine(new string(char.Parse("-"), 35));
                Console.WriteLine("Press any key to proceed.");
                Console.ReadKey();
                Console.Clear();
            }
        }

        private static void CompareQuery(int rows)
        {
            DapperQueryEntity(rows);
            RepoDbQueryEntity(rows);
            DapperQuery(rows);
            RepoDbExecuteQuery(rows);
        }

        private static void DapperQueryEntity(int rows)
        {
            var now = DateTime.UtcNow;
            using (var connection = new SqlConnection(ConnectionString).EnsureOpen())
            {
                var employees = connection.Query<Employee>($"SELECT TOP {rows} * FROM [dbo].[Employee];");
                var lapsedTime = DateTime.UtcNow - now;
                Console.WriteLine($"Dapper.Query<Employee>: {lapsedTime.TotalSeconds} for {employees?.Count()} rows.");
            }
        }

        private static void RepoDbQueryEntity(int rows)
        {
            var now = DateTime.UtcNow;
            using (var repository = new DbRepository<SqlConnection>(ConnectionString))
            {
                var employees = repository.Query<Employee>(top: rows);
                var lapsedTime = DateTime.UtcNow - now;
                Console.WriteLine($"RepoDb.DbRepository.Query<Employee>: {lapsedTime.TotalSeconds} for {employees?.Count()} rows.");
            }
        }

        private static void DapperQuery(int rows)
        {
            var now = DateTime.UtcNow;
            using (var connection = new SqlConnection(ConnectionString).EnsureOpen())
            {
                var employees = connection.Query<Employee>($"SELECT TOP {rows} * FROM [dbo].[Employee];");
                var lapsedTime = DateTime.UtcNow - now;
                Console.WriteLine($"Dapper.Query (Dynamic): {lapsedTime.TotalSeconds} for {employees?.Count()} rows.");
            }
        }

        private static void RepoDbExecuteQuery(int rows)
        {
            var now = DateTime.UtcNow;
            using (var connection = new SqlConnection(ConnectionString).EnsureOpen())
            {
                var employees = connection.ExecuteQuery($"SELECT TOP {rows} * FROM [dbo].[Employee];");
                var lapsedTime = DateTime.UtcNow - now;
                Console.WriteLine($"RepoDb.Connection.ExecuteQuery (Dynamic - No IL): {lapsedTime.TotalSeconds} for {employees?.Count()} rows.");
            }
        }

        private static void CompareInsert(int rows)
        {
            var employees = CreateEmployees(rows);
            DapperInsert(employees);
            RepoDbInsert(employees);
            RepoDbBulkInsert(employees);
        }

        private static void DapperInsert(IEnumerable<Employee> employees)
        {
            var now = DateTime.UtcNow;
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                employees?.ToList().ForEach(employee =>
                {
                    connection.Execute(@"
                        INSERT INTO [dbo].[Employee]
                        (
	                        FirstName,
	                        LastName,
	                        Address,
	                        Phone,
	                        Age,
	                        Worth,
	                        Salary,
	                        DateOfBirth,
	                        Gender,
	                        Email,
	                        CreatedDate,
	                        UpdatedDate
                        )
                        VALUES
                        (
	                        @FirstName,
	                        @LastName,
	                        @Address,
	                        @Phone,
	                        @Age,
	                        @Worth,
	                        @Salary,
	                        @DateOfBirth,
	                        @Gender,
	                        @Email,
	                        @CreatedDate,
	                        @UpdatedDate
                        )", employee);
                });
            }
            var lapsedTime = DateTime.UtcNow - now;
            Console.WriteLine($"Dapper.Insert: {lapsedTime.TotalSeconds} for {employees?.Count()} rows.");
        }

        private static void RepoDbInsert(IEnumerable<Employee> employees)
        {
            var now = DateTime.UtcNow;
            using (var repository = new DbRepository<SqlConnection>(ConnectionString, connectionPersistency: ConnectionPersistency.Instance))
            {
                employees?.ToList().ForEach(employee =>
                {
                    repository.Insert(employee);
                });
                var lapsedTime = DateTime.UtcNow - now;
                Console.WriteLine($"RepoDb.Insert: {lapsedTime.TotalSeconds} for {employees?.Count()} rows.");
            }
        }

        private static void RepoDbBulkInsert(IEnumerable<Employee> employees)
        {
            var now = DateTime.UtcNow;
            using (var repository = new DbRepository<SqlConnection>(ConnectionString))
            {
                repository.BulkInsert(employees);
                var lapsedTime = DateTime.UtcNow - now;
                Console.WriteLine($"RepoDb.BulkInsert: {lapsedTime.TotalSeconds} for {employees?.Count()} rows.");
            }
        }

        private static void InsertAll(int rows)
        {
            var now = DateTime.UtcNow;
            var employees = CreateEmployees(rows);
            using (var repository = new DbRepository<SqlConnection>(ConnectionString))
            {
                repository.BulkInsert(employees);
                var lapsedTime = DateTime.UtcNow - now;
                Console.WriteLine($"{rows} row(s) has been inserted for {lapsedTime.TotalSeconds} second(s).");
            }
        }

        private static void DeleteAll()
        {
            Console.WriteLine($"Deleting all records, please wait...");
            using (var repository = new DbRepository<SqlConnection>(ConnectionString))
            {
                var affectedRows = repository.DeleteAll<Employee>();
                Console.WriteLine($"{affectedRows} row(s) has been deleted.");
            }
        }

        private static IList<Employee> CreateEmployees(int count)
        {
            var list = new List<Employee>();
            var random = new Random();
            for (var i = 0; i < count; i++)
            {
                list.Add(new Employee()
                {
                    Id = i,
                    Address = $"ADDR: {Guid.NewGuid().ToString().ToUpper()}",
                    Age = random.Next(50, 100),
                    CreatedDate = DateTime.UtcNow,
                    DateOfBirth = DateTime.UtcNow.Date.AddYears(-random.Next(30, 60)).AddMonths(-(i % 10)),
                    Email = $"{Guid.NewGuid().ToString().ToUpper()}@email.com",
                    FirstName = $"FNAM: {Guid.NewGuid().ToString().ToUpper()}",
                    Gender = Convert.ToInt16(random.Next(0, 1)),
                    LastName = $"LNAM: {Guid.NewGuid().ToString().ToUpper()}",
                    Phone = $"PHON: {random.Next(100, 999)}-{random.Next(1000, 9999)}-{random.Next(100, 999)}",
                    Salary = random.Next(12000, 15000) + ((i % 5) * 100),
                    UpdatedDate = DateTime.UtcNow,
                    Worth = random.Next(120000, 150000) + ((i % 5) * 1000)
                });
            }
            return list;
        }
    }
}
