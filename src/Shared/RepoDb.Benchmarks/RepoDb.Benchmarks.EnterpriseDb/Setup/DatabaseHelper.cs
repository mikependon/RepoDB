#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using RepoDb.Connector.EnterpriseDb;

namespace RepoDb.Benchmarks.EnterpriseDb.Setup
{
    public static class DatabaseHelper
    {
        public static string AdminConnectionString { get; private set; }

        public static string ConnectionString { get; private set; }

        public static void Initialize(int elementsCount)
        {
            var adminConnectionString = Environment.GetEnvironmentVariable("REPODB_CONSTR_ENTERPRISEDB", EnvironmentVariableTarget.Process);
            var connectionString = Environment.GetEnvironmentVariable("REPODB_CONSTR", EnvironmentVariableTarget.Process);

            AdminConnectionString = adminConnectionString ?? "Server=127.0.0.1;Port=5433;Database=edb;User Id=enterprisedb;Password=RepoDB2026;";
            ConnectionString = connectionString ?? "Server=127.0.0.1;Port=5433;Database=RepoDb;User Id=enterprisedb;Password=RepoDB2026;";

            CreateDatabase();
            CreatePersonTable();
            FillData(elementsCount);
        }

        private static void FillData(int elementsCount)
        {
            const string commandText = @"insert into public.""Person"" (""Name"", ""Age"", ""CreatedDateUtc"")
		                                    values (REPEAT('x', 128), @element, NOW());";

            using var connection = new EDBConnection(ConnectionString);
            connection.Open();

            for (var i = 1; i <= elementsCount; i++)
            {
                var command = new EDBCommand(commandText, connection);
                command.Parameters.AddWithValue("element", i);
                command.ExecuteNonQuery();
            }
        }

        public static void Cleanup()
        {
            const string commandText = @"TRUNCATE TABLE public.""Person""";

            using var connection = new EDBConnection(ConnectionString);

            connection.Open();
            connection.ExecuteNonQuery(commandText);
        }

        private static void CreateDatabase()
        {
            using var connection = new EDBConnection(AdminConnectionString);
            connection.Open();

            var recordCount = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM pg_database WHERE datname = 'RepoDb';");
            if (recordCount <= 0)
            {
                connection.ExecuteNonQuery(@"CREATE DATABASE ""RepoDb""
                        WITH OWNER = ""enterprisedb""
                        ENCODING = ""UTF8""
                        CONNECTION LIMIT = -1;");
            }
        }

        private static void CreatePersonTable()
        {
            const string commandText = @"CREATE TABLE IF NOT EXISTS public.""Person""
                    (
	                    ""Id"" bigint GENERATED ALWAYS AS IDENTITY,
	                    ""Name"" VARCHAR(128),
	                    ""Age"" integer,
	                    ""CreatedDateUtc"" TIMESTAMP(5),
                        CONSTRAINT ""CRIX_Person_Id"" PRIMARY KEY (""Id"")
                    );";

            using var connection = new EDBConnection(ConnectionString);

            connection.Open();
            connection.ExecuteNonQuery(commandText);
        }
    }
}
