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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.Connector.AuroraDb.Npgsql;
using RepoDb.Schema.AuroraDbPostgreSql.IntegrationTests.Setup;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.AuroraDbPostgreSql.IntegrationTests
{
    /// <summary>
    /// Contains the read definition test behavior.
    /// </summary>
    [TestClass]
    public class ReadDefinitionTest
    {
        #region Public Methods

        /// <summary>
        /// Performs initialize.
        /// </summary>
        [TestInitialize]
        public void Initialize() => Database.Initialize();

        /// <summary>
        /// Performs cases.
        /// </summary>
        public static IEnumerable<object[]> Cases()
        {
            foreach (var format in new[]
            {
                "json",
                "xml",
                "yaml"
            }

            )
                foreach (RelatedTableBehavior behavior in Enum.GetValues(typeof(RelatedTableBehavior)))
                    foreach (var asynchronous in new[]
                    {
                        false,
                        true
                    }

                    )
                        yield return new object[]
                        {
                            format,
                            behavior,
                            asynchronous
                        };
        }

        /// <summary>
        /// Verifies saved definition restores provider metadata.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="behavior">The behavior.</param>
        /// <param name="asynchronous">The asynchronous.</param>
        [TestMethod]
        [DynamicData(nameof(Cases))]
        public async Task SavedDefinitionRestoresProviderMetadata(string format,
            RelatedTableBehavior behavior,
            bool asynchronous)
        {
            var filename=Path.Combine(Path.GetTempPath(),
                "repodb-definition-" + Guid.NewGuid().ToString("N"));
            try
            {
                using (var connection = new AuroraDbConnection(Database.ConnectionStringForSource))
                {
                    // Setup: extract real provider metadata and its related tables.
                    using (Register(connection,
                        new AuroraDbPostgreSqlSchemaReader()))
                    {
                        connection.SaveDefinitionAsJson("Person",
                            filename,
                            behavior);
                        var expected = JsonNode.Parse(File.ReadAllText(filename));
                        if (format == "xml")
                            connection.SaveDefinitionAsXml("Person",
                                filename,
                                behavior);
                        if (format == "yaml")
                            connection.SaveDefinitionAsYaml("Person",
                                filename,
                                behavior);
                        if (asynchronous)
                        {
                            if (format == "json")
                                await connection.SaveDefinitionAsJsonAsync("Person",
                                    filename,
                                    behavior);
                            else if (format == "xml")
                                await connection.SaveDefinitionAsXmlAsync("Person",
                                    filename,
                                    behavior);
                            else
                                await connection.SaveDefinitionAsYamlAsync("Person",
                                    filename,
                                    behavior);
                        }

                        var state = connection.State;
                        // Act
                        SchemaDefinitionInfo actual;
                        if (format == "json")
                            actual = asynchronous ? await connection.ReadDefinitionFromJsonAsync(filename) : connection.ReadDefinitionFromJson(filename);
                        else if (format == "xml")
                            actual = asynchronous ? await connection.ReadDefinitionFromXmlAsync(filename) : connection.ReadDefinitionFromXml(filename);
                        else
                            actual = asynchronous ? await connection.ReadDefinitionFromYamlAsync(filename) : connection.ReadDefinitionFromYaml(filename);
                        // Assert: compare all saved fields, including metadata excluded from model equality.
                        Assert.AreEqual(state,
                            connection.State);
                        Assert.AreEqual(behavior,
                            actual.RelationshipBehavior);
                        Assert.IsTrue(JsonNode.DeepEquals(expected,
                                actual.Definition));
                        var reconstructed = SchemaDefinition.Create(actual.RootTable,
                            actual.Tables,
                            actual.RelationshipBehavior);
                        Assert.IsTrue(JsonNode.DeepEquals(expected,
                                reconstructed));
                    }
                }
            }
            finally
            {
                if (File.Exists(filename))
                    File.Delete(filename);
            }
        }

        /// <summary>
        /// Performs register.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="reader">The reader.</param>
        /// <typeparam name="TDbConnection">The tdb connection type.</typeparam>
        private static IDisposable Register<TDbConnection>(TDbConnection connection,
            ISchemaReader reader)
            where TDbConnection : System.Data.IDbConnection => new ReaderScope<TDbConnection>(connection,
                reader);

        /// <summary>
        /// Contains the reader scope behavior.
        /// </summary>
        private sealed class ReaderScope<TDbConnection> : IDisposable where TDbConnection : System.Data.IDbConnection
        {
            private readonly ISchemaReader previous;
            /// <summary>
            /// Creates a ReaderScope instance.
            /// </summary>
            /// <param name="connection">The connection.</param>
            /// <param name="reader">The reader.</param>
            internal ReaderScope(TDbConnection connection,
                ISchemaReader reader)
            {
                previous = SchemaReaderMapper.Get(connection);
                SchemaReaderMapper.Add<TDbConnection>(reader,
                    true);
            }

            /// <summary>
            /// Performs dispose.
            /// </summary>
            public void Dispose()
            {
                if (previous == null)
                    SchemaReaderMapper.Remove<TDbConnection>();
                else
                    SchemaReaderMapper.Add<TDbConnection>(previous,
                        true);
            }
        }
        #endregion
    }
}
