#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Connector.MariaDb;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.MariaDb.IntegrationTests.Setup;
using RepoDb.Schema.Models;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System;

namespace RepoDb.Schema.MariaDb.IntegrationTests
{
    [TestClass]
    public class LoadDefinitionTest
    {
        [TestInitialize]
        public void Initialize() =>
            Database.Initialize();

        public static IEnumerable<object[]> Cases()
        {
            foreach (var format in new[] { "json", "xml", "yaml" })
                foreach (RelatedTableBehavior behavior in Enum.GetValues(typeof(RelatedTableBehavior)))
                    foreach (var asynchronous in new[] { false, true })
                        yield return new object[] { format, behavior, asynchronous };
        }

        [TestMethod]
        [DynamicData(nameof(Cases))]
        public async Task LoadsSavedRelationshipsIntoDestination(string format, RelatedTableBehavior behavior, bool asynchronous)
        {
            var filename = Path.Combine(Path.GetTempPath(), "repodb-load-live-" + Guid.NewGuid().ToString("N"));
            try
            {
                Database.Cleanup();
                using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
                using (var target = new MariaDbConnection(Database.ConnectionStringForTarget))
                {
                    var sourceReader = new MariaDbSchemaReader(connection);
                    var previous = SchemaReaderMapper.Get(connection);
                    Register(connection, sourceReader);
                    try { connection.SaveDefinitionAsJson("Child", filename, behavior); }
                    finally { Restore(connection, previous); }

                    // Restore into the target's current schema rather than the fixture's source owner/database.
                    var document = JsonNode.Parse(File.ReadAllText(filename));
                    ClearSchemas(document);
                    File.WriteAllText(filename, SchemaDefinitionSerializer.Serialize(document.AsObject(), format));
                    var callbacks = new List<CopyTableSchemaResultItem>();

                    var result = await Load(target, filename, format, asynchronous, callbacks.Add);
                    Assert.IsTrue(JsonNode.DeepEquals(document, result.Definition.Definition));
                    Assert.AreEqual(document["tables"].AsArray().Count, result.CopyResult.Items.Count);
                    Assert.AreEqual(result.CopyResult.Items.Count, callbacks.Count);
                    Assert.AreEqual(0, result.CopyResult.Errors.Count);
                    var targetReader = new MariaDbSchemaReader(target);
                    foreach (var item in result.CopyResult.Items)
                    {
                        Assert.AreEqual(TableOutcome.Created, item.Outcome);
                        Assert.IsTrue(callbacks.Contains(item));
                        var actual = asynchronous
                            ? await targetReader.GetTableSchemaAsync(item.Target.Table.Table.Name)
                            : targetReader.GetTableSchema(item.Target.Table.Table.Name);
                        Assert.AreEqual(item.ColumnCount, actual.Columns.Count);
                        Assert.IsNotNull(actual.PrimaryKey);
                    }
                    var skipped = await Load(target, filename, format, asynchronous, null);
                    Assert.IsTrue(skipped.CopyResult.Items.All(item => item.Outcome == TableOutcome.Skipped));
                }
            }
            finally
            {
                if (File.Exists(filename)) File.Delete(filename);
            }
        }

        private static Task<LoadDefinitionResult> Load(IDbConnection connection, string filename, string format,
            bool asynchronous, Action<CopyTableSchemaResultItem> callback)
        {
            if (format == "json") return asynchronous ? connection.LoadDefinitionFromJsonAsync(filename, createdCallback: callback)
                : Task.FromResult(connection.LoadDefinitionFromJson(filename, createdCallback: callback));
            if (format == "xml") return asynchronous ? connection.LoadDefinitionFromXmlAsync(filename, createdCallback: callback)
                : Task.FromResult(connection.LoadDefinitionFromXml(filename, createdCallback: callback));
            return asynchronous ? connection.LoadDefinitionFromYamlAsync(filename, createdCallback: callback)
                : Task.FromResult(connection.LoadDefinitionFromYaml(filename, createdCallback: callback));
        }

        private static void ClearSchemas(JsonNode node)
        {
            if (node is JsonObject obj)
            {
                if (obj.ContainsKey("schema") && obj.ContainsKey("name") && obj.ContainsKey("database") && obj.ContainsKey("provider"))
                    obj["schema"] = null;
                foreach (var value in obj.Select(property => property.Value).ToArray()) ClearSchemas(value);
            }
            else if (node is JsonArray array)
                foreach (var value in array) ClearSchemas(value);
        }

        private static void Register<TConnection>(TConnection connection, ISchemaReader reader)
            where TConnection : IDbConnection =>
            SchemaReaderMapper.Add<TConnection>(reader, true);

        private static void Restore<TConnection>(TConnection connection, ISchemaReader reader)
            where TConnection : IDbConnection
        {
            if (reader == null) SchemaReaderMapper.Remove<TConnection>();
            else SchemaReaderMapper.Add<TConnection>(reader, true);
        }
    }
}
