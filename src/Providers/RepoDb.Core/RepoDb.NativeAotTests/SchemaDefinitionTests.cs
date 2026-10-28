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
using RepoDb.Attributes;
using RepoDb.Schema;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;
using RepoDb.Schema.Sqlite.Microsoft;

namespace RepoDb.NativeAotTests
{
    public static partial class Program
    {
        [Map("Invoice")]
        private sealed class SchemaInvoice
        {
            public long Id { get; set; }
        }

        /// <summary>
        /// Exercises the async exporters with real catalog reads and reflection-based JSON serialization disabled.
        /// </summary>
        private static async Task TestSchemaDefinitionsAsync()
        {
            await RunAsync("Schema save/read/copy extensions (sync/async; every relationship behavior)", async () =>
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
                            SchemaDefinitionResult synchronous;
                            SchemaDefinitionResult asynchronous;
                            if (format == "json")
                            {
                                synchronous = connection.ReadDefinitionFromJson(filename);
                                asynchronous = await connection.ReadDefinitionFromJsonAsync(filename);
                            }
                            else if (format == "xml")
                            {
                                synchronous = connection.ReadDefinitionFromXml(filename);
                                asynchronous = await connection.ReadDefinitionFromXmlAsync(filename);
                            }
                            else
                            {
                                synchronous = connection.ReadDefinitionFromYaml(filename);
                                asynchronous = await connection.ReadDefinitionFromYamlAsync(filename);
                            }
                            var count = behavior == RelatedTableBehavior.TableOnly ? 1
                                : behavior == RelatedTableBehavior.ParentsAndChildren ? 3 : 2;
                            if (synchronous.Tables.Count != count || asynchronous.Tables.Count != count ||
                                !System.Text.Json.Nodes.JsonNode.DeepEquals(synchronous.Definition, asynchronous.Definition))
                                throw new Exception("Schema readers did not preserve the saved definition.");
                            var invoice = asynchronous.Tables.Single(table => table.Table.Name == "Invoice");
                            if (invoice.Columns.Count != 4 || invoice.PrimaryKey == null || invoice.Indexes.Count != 1 ||
                                invoice.CheckConstraints.Count != 1 || invoice.ForeignKeys.Count != (behavior == RelatedTableBehavior.TableOnly ? 0 : 1))
                                throw new Exception("Schema readers did not reconstruct the table metadata.");
                        }

                        SchemaComposerMapper.Add<SqliteConnection>(new SqliteSchemaComposer(), true);
                        using var target = new SqliteConnection("Data Source=:memory:");
                        await target.OpenAsync();
                        var copy = connection.CopyTableSchemaTo<SchemaInvoice>(target, relationshipBehavior: behavior);
                        var copyAsync = await connection.CopyTableSchemaToAsync<SchemaInvoice>(target,
                            tableExistenceBehavior: TableExistsBehavior.Drop, relationshipBehavior: behavior);
                        if (copy.Errors.Count != 0 || copyAsync.Errors.Count != 0 || copy.Items.Count != copyAsync.Items.Count)
                            throw new Exception("Generic schema copy failed.");
                        var multiple = connection.CopyTableSchemaTo(new[] { "Invoice" }, target,
                            tableExistenceBehavior: TableExistsBehavior.Skip, relationshipBehavior: behavior);
                        var multipleAsync = await connection.CopyTableSchemaToAsync(new[] { "Invoice" }, target,
                            tableExistenceBehavior: TableExistsBehavior.Skip, relationshipBehavior: behavior);
                        if (multiple.Errors.Count != 0 || multipleAsync.Errors.Count != 0)
                            throw new Exception("Multiple-table schema copy failed.");
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
