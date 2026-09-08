#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using RepoDb.Connector.MariaDbConnector;

namespace RepoDb.Benchmarks.MariaDbConnector.Setup
{
    public static class DatabaseHelper
    {
        public static string AdminConnectionString { get; private set; }

        public static string ConnectionString { get; private set; }

        public static void Initialize(int elementsCount)
        {
            var adminConnectionString = Environment.GetEnvironmentVariable("REPODB_CONSTR_MARIADB", EnvironmentVariableTarget.Process);
            var connectionString = Environment.GetEnvironmentVariable("REPODB_CONSTR", EnvironmentVariableTarget.Process);

            AdminConnectionString = adminConnectionString ?? "Server=127.0.0.1;Port=3307;User Id=root;Password=RepoDB2026;";
            ConnectionString = connectionString ?? "Server=127.0.0.1;Port=3307;Database=RepoDb;User Id=root;Password=RepoDB2026;";

            CreateDatabase();
            CreatePersonTable();
            FillData(elementsCount);
        }

        private static void FillData(int elementsCount)
        {
            const string commandText = @"INSERT INTO Person (Name, Age, CreatedDateUtc)
                                        VALUES (REPEAT('x', 128), @element, NOW(5));";

            using var connection = new MariaDbConnection(ConnectionString);
            connection.Open();

            for (var i = 1; i <= elementsCount; i++)
            {
                var command = new MariaDbCommand(commandText, connection);
                command.Parameters.AddWithValue("@element", i);
                command.ExecuteNonQuery();
            }
        }

        public static void Cleanup()
        {
            const string commandText = "TRUNCATE TABLE Person;";

            using var connection = new MariaDbConnection(ConnectionString);

            connection.Open();
            connection.ExecuteNonQuery(commandText);
        }

        private static void CreateDatabase()
        {
            const string commandText = "CREATE DATABASE IF NOT EXISTS RepoDb;";

            using var connection = new MariaDbConnection(AdminConnectionString);

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

            using var connection = new MariaDbConnection(ConnectionString);

            connection.Open();
            connection.ExecuteNonQuery(commandText);
        }
    }
}
