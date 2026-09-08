#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Vertica.Data.VerticaClient;

namespace RepoDb.Benchmarks.Vertica.Setup
{
    public static class DatabaseHelper
    {
        public static string ConnectionString { get; private set; }

        public static void Initialize(int elementsCount)
        {
            var connectionString = Environment.GetEnvironmentVariable("REPODB_CONSTR", EnvironmentVariableTarget.Process);

            ConnectionString = connectionString ?? "Host=127.0.0.1;Port=5433;Database=RepoDb;User=dbadmin;Password=RepoDB2026;";

            CreatePersonTable();
            FillData(elementsCount);
        }

        private static void FillData(int elementsCount)
        {
            const string commandText = @"insert into ""Person"" (""Name"", ""Age"", ""CreatedDateUtc"")
                                        values (REPEAT('x', 128), @element, CURRENT_TIMESTAMP)";

            using var connection = new VerticaConnection(ConnectionString);
            connection.Open();

            for (var i = 1; i <= elementsCount; i++)
            {
                var command = new VerticaCommand(commandText, connection);
                command.Parameters.Add(new VerticaParameter("@element", i));
                command.ExecuteNonQuery();
            }
        }

        public static void Cleanup()
        {
            // Vertica has no TRUNCATE TABLE statement; DELETE FROM without a WHERE clause is the
            // idiomatic equivalent (RepoDb.Vertica's own CreateTruncate does the same internally).
            const string commandText = @"delete from ""Person""";

            using var connection = new VerticaConnection(ConnectionString);

            connection.Open();
            connection.ExecuteNonQuery(commandText);
        }

        private static void CreatePersonTable()
        {
            // Vertica has no RECREATE TABLE / CREATE TABLE IF NOT EXISTS, and no multi-statement
            // command text (IsMultiStatementExecutable is false) - drop and create are two separate
            // round trips.
            using var connection = new VerticaConnection(ConnectionString);
            connection.Open();

            connection.ExecuteNonQuery(@"DROP TABLE IF EXISTS ""Person"" CASCADE;");

            connection.ExecuteNonQuery(@"CREATE TABLE ""Person""
                    (
                        ""Id"" IDENTITY(1, 1),
                        ""Name"" VARCHAR(128) NOT NULL,
                        ""Age"" INTEGER NOT NULL,
                        ""CreatedDateUtc"" TIMESTAMP NOT NULL,
                        PRIMARY KEY (""Id"")
                    );");
        }
    }
}
