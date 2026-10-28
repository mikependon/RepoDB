#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Firebird.IntegrationTests.Setup;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.Firebird.IntegrationTests
{
    [TestClass]
    public class ReadDefinitionTest
    {
        #region Methods

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
        public async Task SavedDefinitionRestoresProviderMetadata(string format, RelatedTableBehavior behavior, bool asynchronous)
        {
            var filename = Path.Combine(Path.GetTempPath(), "repodb-definition-" + Guid.NewGuid().ToString("N"));
            try
            {
                using (var connection = new FbConnection(Database.ConnectionStringForSource))
                {
                    // Setup: extract real provider metadata and its related tables.
                    using (Register(connection, new FirebirdSchemaReader(connection)))
                    {
                        connection.SaveDefinitionAsJson("Child", filename, behavior);
                        var expected = JsonNode.Parse(File.ReadAllText(filename));
                        if (format == "xml") connection.SaveDefinitionAsXml("Child", filename, behavior);
                        if (format == "yaml") connection.SaveDefinitionAsYaml("Child", filename, behavior);
                        if (asynchronous)
                        {
                            if (format == "json") await connection.SaveDefinitionAsJsonAsync("Child", filename, behavior);
                            else if (format == "xml") await connection.SaveDefinitionAsXmlAsync("Child", filename, behavior);
                            else await connection.SaveDefinitionAsYamlAsync("Child", filename, behavior);
                        }
                        var state = connection.State;

                        // Act
                        SchemaDefinitionResult actual;
                        if (format == "json")
                            actual = asynchronous ? await connection.ReadDefinitionFromJsonAsync(filename) : connection.ReadDefinitionFromJson(filename);
                        else if (format == "xml")
                            actual = asynchronous ? await connection.ReadDefinitionFromXmlAsync(filename) : connection.ReadDefinitionFromXml(filename);
                        else
                            actual = asynchronous ? await connection.ReadDefinitionFromYamlAsync(filename) : connection.ReadDefinitionFromYaml(filename);

                        // Assert: compare all saved fields, including metadata excluded from model equality.
                        Assert.AreEqual(state, connection.State);
                        Assert.AreEqual(behavior, actual.RelationshipBehavior);
                        Assert.IsTrue(JsonNode.DeepEquals(expected, actual.Definition));
                        var reconstructed = SchemaDefinition.Create(actual.RootTable, actual.Tables, actual.RelationshipBehavior);
                        Assert.IsTrue(JsonNode.DeepEquals(expected, reconstructed));
                    }
                }
            }
            finally
            {
                if (File.Exists(filename)) File.Delete(filename);
            }
        }


        private static IDisposable Register<TDbConnection>(TDbConnection connection, ISchemaReader reader)
            where TDbConnection : System.Data.IDbConnection =>
            new ReaderScope<TDbConnection>(connection, reader);

        private sealed class ReaderScope<TDbConnection> : IDisposable
            where TDbConnection : System.Data.IDbConnection
        {
            private readonly ISchemaReader previous;

            internal ReaderScope(TDbConnection connection, ISchemaReader reader)
            {
                previous = SchemaReaderMapper.Get(connection);
                SchemaReaderMapper.Add<TDbConnection>(reader, true);
            }

            public void Dispose()
            {
                if (previous == null) SchemaReaderMapper.Remove<TDbConnection>();
                else SchemaReaderMapper.Add<TDbConnection>(previous, true);
            }
        }

        #endregion
    }
}
