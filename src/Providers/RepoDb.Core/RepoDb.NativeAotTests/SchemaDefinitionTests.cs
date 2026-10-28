#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using RepoDb.Schema;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Sqlite.Microsoft;

namespace RepoDb.NativeAotTests
{
    public static partial class Program
    {
        /// <summary>
        /// Exercises the async exporters with real catalog reads and reflection-based JSON serialization disabled.
        /// </summary>
        private static async Task TestSchemaDefinitionsAsync()
        {
            await RunAsync("Async schema definitions (JSON, XML, YAML; every relationship behavior)", async () =>
            {
                if (JsonSerializer.IsReflectionEnabledByDefault) throw new Exception("Reflection-based JSON serialization must be disabled.");
                using var connection = new SqliteConnection("Data Source=:memory:");
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"CREATE TABLE Customer (Id integer PRIMARY KEY);
                        CREATE TABLE Invoice (Id integer PRIMARY KEY, CustomerId integer REFERENCES Customer(Id),
                            Amount decimal(18,4) NOT NULL DEFAULT 0, Name nvarchar(100),
                            CONSTRAINT CK_Amount CHECK (Amount >= 0));
                        CREATE TABLE InvoiceLine (Id integer PRIMARY KEY, InvoiceId integer REFERENCES Invoice(Id));
                        CREATE INDEX IX_Invoice_Customer ON Invoice(CustomerId);";
                    await command.ExecuteNonQueryAsync();
                }
                SchemaReaderMapper.Add<SqliteConnection>(new SqliteSchemaReader(connection), true);
                var filename = Path.Combine(Path.GetTempPath(), "repodb-aot-definition-" + Guid.NewGuid().ToString("N"));
                try
                {
                    foreach (var behavior in new[] { RelatedTableBehavior.TableOnly, RelatedTableBehavior.Parents,
                        RelatedTableBehavior.Children, RelatedTableBehavior.ParentsAndChildren })
                    {
                        foreach (var format in new[] { "json", "xml", "yaml" })
                        {
                            if (format == "json") connection.SaveDefinitionAsJson("Invoice", filename, behavior);
                            else if (format == "xml") connection.SaveDefinitionAsXml("Invoice", filename, behavior);
                            else connection.SaveDefinitionAsYaml("Invoice", filename, behavior);
                            var expected = File.ReadAllBytes(filename);
                            if (format == "json") await connection.SaveDefinitionAsJsonAsync("Invoice", filename, behavior);
                            else if (format == "xml") await connection.SaveDefinitionAsXmlAsync("Invoice", filename, behavior);
                            else await connection.SaveDefinitionAsYamlAsync("Invoice", filename, behavior);
                            if (!expected.SequenceEqual(File.ReadAllBytes(filename)))
                                throw new Exception("Async schema definition differs from synchronous output: " + format + "/" + behavior);
                        }
                    }
                }
                finally
                {
                    SchemaReaderMapper.Remove<SqliteConnection>();
                    if (File.Exists(filename)) File.Delete(filename);
                }
            });
        }
    }
}
