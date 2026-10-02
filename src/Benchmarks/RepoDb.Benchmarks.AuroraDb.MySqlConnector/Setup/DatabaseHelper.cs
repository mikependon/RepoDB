#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Connector.AuroraDb.MySqlConnector;
using System;

namespace RepoDb.Benchmarks.AuroraDb.MySqlConnector.Setup
{
    public static class DatabaseHelper
    {
        public static string AdminConnectionString { get; private set; }

        public static string ConnectionString { get; private set; }

        public static void Initialize(int elementsCount)
        {
            var adminConnectionString = Environment.GetEnvironmentVariable("REPODB_CONSTR_AURORADB_MYSQL", EnvironmentVariableTarget.Process);
            var connectionString = Environment.GetEnvironmentVariable("REPODB_CONSTR", EnvironmentVariableTarget.Process);

            AdminConnectionString = adminConnectionString ?? "Server=127.0.0.1;Port=3308;User Id=root;Password=RepoDB2026;";
            ConnectionString = connectionString ?? "Server=127.0.0.1;Port=3308;Database=RepoDb;User Id=root;Password=RepoDB2026;";

            CreateDatabase();
            CreatePersonTable();
            FillData(elementsCount);
        }

        private static void FillData(int elementsCount)
        {
            const string commandText = @"INSERT INTO Person (Name, Age, CreatedDateUtc)
                                        VALUES (REPEAT('x', 128), @element, NOW(5));";

            using var connection = new AuroraDbConnection(ConnectionString);
            connection.Open();

            for (var i = 1; i <= elementsCount; i++)
            {
                connection.ExecuteNonQuery(commandText, new { element = i });
            }
        }

        public static void Cleanup()
        {
            const string commandText = "TRUNCATE TABLE Person;";

            using var connection = new AuroraDbConnection(ConnectionString);

            connection.Open();
            connection.ExecuteNonQuery(commandText);
        }

        private static void CreateDatabase()
        {
            const string commandText = "CREATE DATABASE IF NOT EXISTS RepoDb;";

            using var connection = new AuroraDbConnection(AdminConnectionString);

            connection.Open();
            connection.ExecuteNonQuery(commandText);
        }

        private static void CreatePersonTable()
        {
            const string commandText = @"CREATE TABLE IF NOT EXISTS Person
                    (
                        Id BIGINT NOT NULL AUTO_INCREMENT,
                        Name VARCHAR(128) NOT NULL,
                        Age INT NOT NULL,
                        CreatedDateUtc DATETIME(5) NOT NULL,
                        CONSTRAINT PK_Person PRIMARY KEY (Id)
                    );";

            using var connection = new AuroraDbConnection(ConnectionString);

            connection.Open();
            connection.ExecuteNonQuery(commandText);
        }
    }
}
