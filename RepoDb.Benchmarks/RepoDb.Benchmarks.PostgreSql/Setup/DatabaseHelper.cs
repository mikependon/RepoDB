using System;
using Npgsql;

namespace RepoDb.Benchmarks.PostgreSql.Setup
{
    public static class DatabaseHelper
    {
        public static string ConnectionStringForPostgres { get; private set; }
        
        public static string ConnectionString { get; private set; }

        public static void Initialize(int elementsCount)
        {
            var connectionStringForPostgres = Environment.GetEnvironmentVariable("REPODB_CONSTR_POSTGRESDB", EnvironmentVariableTarget.Process);
            var connectionString = Environment.GetEnvironmentVariable("REPODB_CONSTR", EnvironmentVariableTarget.Process);

            ConnectionStringForPostgres = connectionStringForPostgres ?? "Server=127.0.0.1;Port=5432;Database=postgres;User Id=postgres;Password=postgres;";
            ConnectionString = connectionString ?? "Server=127.0.0.1;Port=5432;Database=RepoDb;User Id=postgres;Password=postgres;";

            CreateDatabase();
            CreatePersonTable();
            FillData(elementsCount);
        }

        private static void FillData(int elementsCount)
        {
            const string commandText = @"insert into public.""Person"" (""Id"", ""Name"", ""Age"", ""CreatedDateUtc"") 
		                                    values (@element, REPEAT('x', 128), @element, NOW());";

            using var connection = new NpgsqlConnection(ConnectionString);
            connection.Open();

            for (var i = 1; i < elementsCount; i++)
            {
                var command = new NpgsqlCommand(commandText, connection);
                command.Parameters.AddWithValue("element", i);
                command.ExecuteNonQuery();
            }
        }

        public static void Cleanup()
        {
            const string commandText = @"TRUNCATE TABLE public.""Person""";

            using var connection = new NpgsqlConnection(ConnectionString);

            connection.Open();
            connection.ExecuteNonQuery(commandText);
        }

        private static void CreateDatabase()
        {
            using var connection = new NpgsqlConnection(ConnectionStringForPostgres);
            connection.Open();
            
            var recordCount = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM pg_database WHERE datname = 'RepoDb';");
            if (recordCount <= 0)
            {
                connection.ExecuteNonQuery(@"CREATE DATABASE ""RepoDb""
                        WITH OWNER = ""postgres""
                        ENCODING = ""UTF8""
                        CONNECTION LIMIT = -1;");
            }
        }

        private static void CreatePersonTable()
        {
            const string commandText = @"CREATE TABLE IF NOT EXISTS public.""Person""
                    (
	                    ""Id"" bigint,
	                    ""Name"" VARCHAR(128),
	                    ""Age"" integer,
	                    ""CreatedDateUtc"" TIMESTAMP(5),
                        CONSTRAINT ""CRIX_Person_Id"" PRIMARY KEY (""Id"")
                    )
                    
                    TABLESPACE pg_default;
                    
                    ALTER TABLE public.""Person""
                    OWNER to postgres;";

            using var connection = new NpgsqlConnection(ConnectionString);

            connection.Open();
            connection.ExecuteNonQuery(commandText);
        }
    }
}