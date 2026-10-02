#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using RepoDb.Connector.CockroachDb;

namespace RepoDb.Benchmarks.CockroachDb.Setup
{
    public static class DatabaseHelper
    {
        public static string AdminConnectionString { get; private set; }

        public static string ConnectionString { get; private set; }

        public static void Initialize(int elementsCount)
        {
            var adminConnectionString = Environment.GetEnvironmentVariable("REPODB_CONSTR_COCKROACHDB", EnvironmentVariableTarget.Process);
            var connectionString = Environment.GetEnvironmentVariable("REPODB_CONSTR", EnvironmentVariableTarget.Process);

            AdminConnectionString = adminConnectionString ?? "Server=127.0.0.1;Port=26257;Database=defaultdb;User Id=root;";
            ConnectionString = connectionString ?? "Server=127.0.0.1;Port=26257;Database=RepoDb;User Id=root;";

            CreateDatabase();
            CreatePersonTable();
            FillData(elementsCount);
        }

        private static void FillData(int elementsCount)
        {
            const string commandText = @"insert into public.""Person"" (""Name"", ""Age"", ""CreatedDateUtc"")
		                                    values (REPEAT('x', 128), @element, NOW());";

            using var connection = new CockroachDbConnection(ConnectionString);
            connection.Open();

            for (var i = 1; i <= elementsCount; i++)
            {
                connection.ExecuteNonQuery(commandText, new { element = i });
            }
        }

        public static void Cleanup()
        {
            const string commandText = @"TRUNCATE TABLE public.""Person""";

            using var connection = new CockroachDbConnection(ConnectionString);

            connection.Open();
            connection.ExecuteNonQuery(commandText);
        }

        private static void CreateDatabase()
        {
            using var connection = new CockroachDbConnection(AdminConnectionString);
            connection.Open();

            connection.ExecuteNonQuery(@"CREATE DATABASE IF NOT EXISTS ""RepoDb"";");
        }

        private static void CreatePersonTable()
        {
            const string commandText = @"CREATE TABLE IF NOT EXISTS public.""Person""
                    (
	                    ""Id"" INT8 GENERATED ALWAYS AS IDENTITY,
	                    ""Name"" VARCHAR(128),
	                    ""Age"" integer,
	                    ""CreatedDateUtc"" TIMESTAMP(5),
                        CONSTRAINT ""CRIX_Person_Id"" PRIMARY KEY (""Id"")
                    );";

            using var connection = new CockroachDbConnection(ConnectionString);

            connection.Open();
            connection.ExecuteNonQuery(commandText);
        }
    }
}
