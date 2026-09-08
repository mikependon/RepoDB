#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.IO;
using Microsoft.Data.Sqlite;

namespace RepoDb.Benchmarks.Sqlite.Microsoft.Setup
{
    public static class DatabaseHelper
    {
        public static string ConnectionString { get; private set; }

        public static void Initialize(int elementsCount)
        {
            var connectionString = Environment.GetEnvironmentVariable("REPODB_CONSTR", EnvironmentVariableTarget.Process);

            // SQLite is a serverless, file-based database - there is no host/port/server to connect to,
            // so unlike every other benchmark in this suite, the default connection string points at a
            // local file next to the compiled benchmark instead of a Docker container.
            var defaultDatabasePath = Path.Combine(AppContext.BaseDirectory, "RepoDb.db");
            ConnectionString = connectionString ?? $"Data Source={defaultDatabasePath};";

            // Delete any leftover file from a previous run so every run starts from a clean slate -
            // there is no server-side DROP/RECREATE to reach for, the database is just a file.
            var dataSource = new SqliteConnectionStringBuilder(ConnectionString).DataSource;
            if (!string.Equals(dataSource, ":memory:", StringComparison.OrdinalIgnoreCase) && File.Exists(dataSource))
            {
                File.Delete(dataSource);
            }

            CreatePersonTable();
            FillData(elementsCount);
        }

        private static void FillData(int elementsCount)
        {
            // SQLite has no built-in REPEAT()/RPAD() function, so the 128-char padding is built in C#
            // and bound as a parameter instead. CreatedDateUtc is bound as a real DateTime parameter
            // rather than generated via CURRENT_TIMESTAMP for the same kind of reason: SQLite has no
            // native date/time storage class, and CURRENT_TIMESTAMP's TEXT output can't be coerced back
            // into a DateTime by RepoDb's compiled reader - binding a DateTime directly lets
            // Microsoft.Data.Sqlite round-trip it using its own consistent read/write conversion.
            var name = new string('x', 128);
            const string commandText = @"INSERT INTO Person (Name, Age, CreatedDateUtc)
                                        VALUES (@Name, @element, @CreatedDateUtc);";

            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            for (var i = 1; i <= elementsCount; i++)
            {
                var command = new SqliteCommand(commandText, connection);
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@element", i);
                command.Parameters.AddWithValue("@CreatedDateUtc", DateTime.UtcNow);
                command.ExecuteNonQuery();
            }
        }

        public static void Cleanup()
        {
            const string commandText = "DELETE FROM Person;";

            using var connection = new SqliteConnection(ConnectionString);

            connection.Open();
            connection.ExecuteNonQuery(commandText);
        }

        private static void CreatePersonTable()
        {
            // "INTEGER PRIMARY KEY" is SQLite's own identity column - it's an alias for the row's
            // ROWID and auto-increments on its own, no AUTOINCREMENT keyword needed (that keyword only
            // changes the ID-reuse behavior after deletes, at an extra cost - see https://sqlite.org/autoinc.html).
            const string commandText = @"CREATE TABLE IF NOT EXISTS Person
                    (
                        Id INTEGER PRIMARY KEY,
                        Name VARCHAR(128) NOT NULL,
                        Age INT NOT NULL,
                        CreatedDateUtc DATETIME NOT NULL
                    );";

            using var connection = new SqliteConnection(ConnectionString);

            connection.Open();
            connection.ExecuteNonQuery(commandText);
        }
    }
}
