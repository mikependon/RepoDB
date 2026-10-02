#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.IO;
using DuckDB.NET.Data;

namespace RepoDb.Benchmarks.DuckDb.Setup
{
    public static class DatabaseHelper
    {
        public static string ConnectionString { get; private set; }

        public static void Initialize(int elementsCount)
        {
            var connectionString = Environment.GetEnvironmentVariable("REPODB_CONSTR", EnvironmentVariableTarget.Process);

            // DuckDB is an embedded, file-based database - there is no host/port/server to connect to,
            // so the default connection string points at a local file next to the compiled benchmark
            // instead of a Docker container.
            var defaultDatabasePath = Path.Combine(AppContext.BaseDirectory, "RepoDb.duckdb");
            ConnectionString = connectionString ?? $"Data Source={defaultDatabasePath}";

            // Delete any leftover file from a previous run so every run starts from a clean slate -
            // there is no server-side DROP/RECREATE to reach for, the database is just a file.
            if (connectionString == null && File.Exists(defaultDatabasePath))
            {
                File.Delete(defaultDatabasePath);
            }

            CreatePersonTable();
            FillData(elementsCount);
        }

        private static void FillData(int elementsCount)
        {
            // CreatedDateUtc is generated server-side in UTC: NOW() is a TIMESTAMPTZ and AT TIME ZONE 'UTC'
            // turns it into the zone-less TIMESTAMP the column is declared as.
            var commandText = $@"INSERT INTO Person (Name, Age, CreatedDateUtc)
                                SELECT REPEAT('x', 128), i, NOW() AT TIME ZONE 'UTC'
                                FROM range(1, {elementsCount + 1}) t(i);";

            using var connection = new DuckDBConnection(ConnectionString);

            connection.Open();
            connection.ExecuteNonQuery(commandText);
        }

        public static void Cleanup()
        {
            const string commandText = "DELETE FROM Person;";

            using var connection = new DuckDBConnection(ConnectionString);

            connection.Open();
            connection.ExecuteNonQuery(commandText);
        }

        private static void CreatePersonTable()
        {
            // DuckDB has no AUTO_INCREMENT/IDENTITY keyword - an auto-generated key is a column whose
            // DEFAULT pulls the next value from a SEQUENCE.
            const string commandText = @"CREATE SEQUENCE IF NOT EXISTS Person_Id_Seq;
                    CREATE TABLE IF NOT EXISTS Person
                    (
                        Id BIGINT DEFAULT nextval('Person_Id_Seq') PRIMARY KEY,
                        Name VARCHAR(128) NOT NULL,
                        Age INTEGER NOT NULL,
                        CreatedDateUtc TIMESTAMP NOT NULL
                    );";

            using var connection = new DuckDBConnection(ConnectionString);

            connection.Open();
            connection.ExecuteNonQuery(commandText);
        }
    }
}
