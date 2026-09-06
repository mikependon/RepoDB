#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using ClickHouse.Driver.ADO;

namespace RepoDb.Benchmarks.ClickHouse.Setup
{
    public static class DatabaseHelper
    {
        public static string AdminConnectionString { get; private set; }

        public static string ConnectionString { get; private set; }

        public static void Initialize(int elementsCount)
        {
            var adminConnectionString = Environment.GetEnvironmentVariable("REPODB_CONSTR_CLICKHOUSEDB", EnvironmentVariableTarget.Process);
            var connectionString = Environment.GetEnvironmentVariable("REPODB_CONSTR", EnvironmentVariableTarget.Process);

            AdminConnectionString = adminConnectionString ?? "Host=127.0.0.1;Port=8123;Username=default;Password=RepoDB2026;Database=default;Protocol=http;";
            ConnectionString = connectionString ?? "Host=127.0.0.1;Port=8123;Username=default;Password=RepoDB2026;Database=RepoDb;Protocol=http;";

            CreateDatabase();
            CreatePersonTable();
            FillData(elementsCount);
        }

        private static void FillData(int elementsCount)
        {
            const string commandText = @"INSERT INTO Person (Id, Name, Age, CreatedDateUtc)
                                        VALUES (@id, repeat('x', 128), @element, now64(5))";

            using var connection = new ClickHouseConnection(ConnectionString);
            connection.Open();

            for (var i = 1; i <= elementsCount; i++)
            {
                var command = new ClickHouseCommand(connection)
                {
                    CommandText = commandText
                };

                var idParameter = command.CreateParameter();
                idParameter.ParameterName = "id";
                idParameter.Value = (long)i;
                command.Parameters.Add(idParameter);

                var elementParameter = command.CreateParameter();
                elementParameter.ParameterName = "element";
                elementParameter.Value = i;
                command.Parameters.Add(elementParameter);

                command.ExecuteNonQuery();
            }
        }

        public static void Cleanup()
        {
            const string commandText = "TRUNCATE TABLE Person";

            using var connection = new ClickHouseConnection(ConnectionString);

            connection.Open();
            connection.ExecuteNonQuery(commandText);
        }

        private static void CreateDatabase()
        {
            const string commandText = "CREATE DATABASE IF NOT EXISTS RepoDb;";

            using var connection = new ClickHouseConnection(AdminConnectionString);

            connection.Open();
            connection.ExecuteNonQuery(commandText);
        }

        private static void CreatePersonTable()
        {
            const string commandText = @"CREATE TABLE IF NOT EXISTS Person
                    (
                        Id Int64,
                        Name String,
                        Age Int32,
                        CreatedDateUtc DateTime64(5)
                    )
                    ENGINE = ReplacingMergeTree
                    ORDER BY Id";

            using var connection = new ClickHouseConnection(ConnectionString);

            connection.Open();
            connection.ExecuteNonQuery(commandText);
        }
    }
}
