#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using RepoDb.IntegrationTests.Setup;
using RepoDb.PropertyHandlers;
using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace RepoDb.IntegrationTests.PropertyHandlers
{
    /// <summary>
    /// The integration tests of <see cref="JsonToJsonNodePropertyHandler"/>. The <c>NVARCHAR(MAX)</c> column
    /// of the <c>[dbo].[CompleteTable]</c> is used to store the <see cref="JsonNode"/> as a JSON text.
    /// </summary>
    [TestClass]
    public class JsonToJsonNodePropertyHandlerTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Database.Initialize();
            Cleanup();
        }

        [TestCleanup]
        public void Cleanup()
        {
            PropertyHandlerMapper.Remove(typeof(JsonNode));
            PropertyHandlerCache.Flush();
            Database.Cleanup();
        }

        #region Classes

        /// <summary>
        /// Property level (attribute): the handler is bound to the property via <see cref="PropertyHandlerAttribute"/>.
        /// </summary>
        [Map("[dbo].[CompleteTable]")]
        private class JsonNodeAttributeModel
        {
            public Guid SessionId { get; set; }

            [Map("ColumnNVarChar"), PropertyHandler(typeof(JsonToJsonNodePropertyHandler))]
            public JsonNode Node { get; set; }
        }

        /// <summary>
        /// Property level (fluent): the handler is bound to the property via <see cref="FluentMapper"/>.
        /// </summary>
        [Map("[dbo].[CompleteTable]")]
        private class JsonNodeFluentModel
        {
            public Guid SessionId { get; set; }

            public JsonNode ColumnNVarChar { get; set; }
        }

        /// <summary>
        /// Type level: the handler is bound to <see cref="JsonNode"/> via <see cref="PropertyHandlerMapper"/>.
        /// </summary>
        [Map("[dbo].[CompleteTable]")]
        private class JsonNodeTypeLevelModel
        {
            public Guid SessionId { get; set; }

            public JsonNode ColumnNVarChar { get; set; }
        }

        #endregion

        #region Helpers

        private static JsonNodeAttributeModel CreateModel(JsonNode node) =>
            new JsonNodeAttributeModel { SessionId = Guid.NewGuid(), Node = node };

        private static string GetRawValue(SqlConnection connection,
            Guid sessionId) =>
            connection.ExecuteScalar(
                "SELECT [ColumnNVarChar] FROM [dbo].[CompleteTable] WHERE [SessionId] = @SessionId;",
                new { SessionId = sessionId }) as string;

        private static bool IsRawValueNull(SqlConnection connection,
            Guid sessionId) =>
            connection.ExecuteScalar<int>(
                "SELECT COUNT(1) FROM [dbo].[CompleteTable] WHERE [SessionId] = @SessionId AND [ColumnNVarChar] IS NULL;",
                new { SessionId = sessionId }) == 1;

        private static Guid InsertRawValue(SqlConnection connection,
            string value)
        {
            var sessionId = Guid.NewGuid();
            connection.ExecuteNonQuery(
                "INSERT INTO [dbo].[CompleteTable] ([SessionId], [ColumnNVarChar]) VALUES (@SessionId, @Value);",
                new { SessionId = sessionId, Value = value });
            return sessionId;
        }

        #endregion

        #region Positive

        [TestMethod]
        public void TestJsonToJsonNodePropertyHandler()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(new JsonObject { ["name"] = "John", ["age"] = 30 });

                // Act
                connection.Insert(model);
                var result = connection.Query<JsonNodeAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.IsNotNull(result.Node);
                Assert.IsTrue(JsonNode.DeepEquals(model.Node, result.Node));
                Assert.AreEqual("John", (string)result.Node["name"]);
                Assert.AreEqual(30, (int)result.Node["age"]);
            }
        }

        [TestMethod]
        public async Task TestJsonToJsonNodePropertyHandlerAsync()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(new JsonObject { ["name"] = "John", ["age"] = 30 });

                // Act
                await connection.InsertAsync(model).ConfigureAwait(false);
                var result = (await connection.QueryAsync<JsonNodeAttributeModel>(e => e.SessionId == model.SessionId).ConfigureAwait(false)).First();

                // Assert
                Assert.IsTrue(JsonNode.DeepEquals(model.Node, result.Node));
            }
        }

        [TestMethod]
        public void TestJsonToJsonNodePropertyHandlerWritesCompactJsonText()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(new JsonObject { ["name"] = "John", ["tags"] = new JsonArray(1, 2) });

                // Act
                connection.Insert(model);
                var raw = GetRawValue(connection, model.SessionId);

                // Assert
                Assert.AreEqual("{\"name\":\"John\",\"tags\":[1,2]}", raw);
            }
        }

        [TestMethod]
        public void TestJsonToJsonNodePropertyHandlerReadsJsonText()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, "{\"items\":[1,2,3],\"nested\":{\"flag\":true}}");

                // Act
                var result = connection.Query<JsonNodeAttributeModel>(e => e.SessionId == sessionId).First();

                // Assert
                Assert.IsInstanceOfType<JsonObject>(result.Node);
                Assert.AreEqual(3, result.Node["items"].AsArray().Count);
                Assert.IsTrue((bool)result.Node["nested"]["flag"]);
            }
        }

        [TestMethod]
        public void TestJsonToJsonNodePropertyHandlerWithArrayNode()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(new JsonArray(1, "two", true));

                // Act
                connection.Insert(model);
                var raw = GetRawValue(connection, model.SessionId);
                var result = connection.Query<JsonNodeAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual("[1,\"two\",true]", raw);
                Assert.IsInstanceOfType<JsonArray>(result.Node);
                Assert.AreEqual(3, result.Node.AsArray().Count);
            }
        }

        [TestMethod]
        public void TestJsonToJsonNodePropertyHandlerWithValueNodes()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var expectations = new (JsonNode Node, string Json)[]
                {
                    (JsonValue.Create("text"), "\"text\""),
                    (JsonValue.Create(12.5), "12.5"),
                    (JsonValue.Create(true), "true"),
                    (JsonValue.Create(false), "false")
                };

                foreach (var (node, json) in expectations)
                {
                    var model = CreateModel(node);

                    // Act
                    connection.Insert(model);
                    var raw = GetRawValue(connection, model.SessionId);
                    var result = connection.Query<JsonNodeAttributeModel>(e => e.SessionId == model.SessionId).First();

                    // Assert
                    Assert.AreEqual(json, raw);
                    Assert.IsInstanceOfType<JsonValue>(result.Node);
                    Assert.IsTrue(JsonNode.DeepEquals(node, result.Node));
                }
            }
        }

        [TestMethod]
        public void TestJsonToJsonNodePropertyHandlerWithNestedNodes()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(new JsonObject
                {
                    ["owner"] = new JsonObject { ["name"] = "Owner", ["age"] = 50 },
                    ["employees"] = new JsonArray(new JsonObject { ["name"] = "E1" }, new JsonObject { ["name"] = "E2" }),
                    ["nothing"] = null
                });

                // Act
                connection.Insert(model);
                var result = connection.Query<JsonNodeAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.IsTrue(JsonNode.DeepEquals(model.Node, result.Node));
                Assert.AreEqual("E2", (string)result.Node["employees"][1]["name"]);
                Assert.IsNull(result.Node["nothing"]);
            }
        }

        [TestMethod]
        public void TestJsonToJsonNodePropertyHandlerReturnsAMutableNode()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(new JsonObject { ["count"] = 1 });
                connection.Insert(model);

                // Act
                var result = connection.Query<JsonNodeAttributeModel>(e => e.SessionId == model.SessionId).First();
                result.Node["count"] = 2;
                result.Node["added"] = "yes";
                connection.Update(result);
                var updated = connection.Query<JsonNodeAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual(2, (int)updated.Node["count"]);
                Assert.AreEqual("yes", (string)updated.Node["added"]);
            }
        }

        [TestMethod]
        public void TestJsonToJsonNodePropertyHandlerWithUnicodeAndEscapedCharacters()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(new JsonObject { ["text"] = "日本語 \"quoted\" Ñandú 😀 <tag>" });

                // Act
                connection.Insert(model);
                var result = connection.Query<JsonNodeAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual("日本語 \"quoted\" Ñandú 😀 <tag>", (string)result.Node["text"]);
            }
        }

        [TestMethod]
        public void TestJsonToJsonNodePropertyHandlerOnInsertAll()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var models = Enumerable.Range(1, 10).Select(i => CreateModel(new JsonObject { ["id"] = i })).ToList();

                // Act
                connection.InsertAll(models);
                var result = connection.QueryAll<JsonNodeAttributeModel>().ToList();

                // Assert
                Assert.AreEqual(models.Count, result.Count);
                for (var i = 0; i < models.Count; i++)
                {
                    var item = result.First(e => e.SessionId == models[i].SessionId);
                    Assert.AreEqual(i + 1, (int)item.Node["id"]);
                }
            }
        }

        [TestMethod]
        public void TestJsonToJsonNodePropertyHandlerOnUpdate()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(new JsonObject { ["v"] = 1 });
                connection.Insert(model);

                // Act
                model.Node = new JsonObject { ["v"] = 2 };
                var affectedRows = connection.Update(model);
                var result = connection.Query<JsonNodeAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual(1, affectedRows);
                Assert.AreEqual(2, (int)result.Node["v"]);
            }
        }

        [TestMethod]
        public void TestJsonToJsonNodePropertyHandlerOnExecuteQuery()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(new JsonObject { ["v"] = 1 });
                connection.Insert(model);

                // Act
                var result = connection.ExecuteQuery<JsonNodeAttributeModel>(
                    "SELECT [SessionId], [ColumnNVarChar] FROM [dbo].[CompleteTable] WHERE [SessionId] = @SessionId;",
                    new { SessionId = model.SessionId }).First();

                // Assert
                Assert.AreEqual(1, (int)result.Node["v"]);
            }
        }

        [TestMethod]
        public void TestJsonToJsonNodePropertyHandlerViaFluentMapper()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                FluentMapper
                    .Entity<JsonNodeFluentModel>()
                    .PropertyHandler(e => e.ColumnNVarChar, new JsonToJsonNodePropertyHandler());
                var model = new JsonNodeFluentModel { SessionId = Guid.NewGuid(), ColumnNVarChar = new JsonObject { ["v"] = 1 } };

                // Act
                connection.Insert(model);
                var result = connection.Query<JsonNodeFluentModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual(1, (int)result.ColumnNVarChar["v"]);
            }
        }

        [TestMethod]
        public void TestJsonToJsonNodePropertyHandlerViaTypeLevelMapper()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                PropertyHandlerMapper.Add<JsonNode, JsonToJsonNodePropertyHandler>(true);
                var model = new JsonNodeTypeLevelModel { SessionId = Guid.NewGuid(), ColumnNVarChar = new JsonObject { ["v"] = 1 } };

                // Act
                connection.Insert(model);
                var result = connection.Query<JsonNodeTypeLevelModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual(1, (int)result.ColumnNVarChar["v"]);
            }
        }

        #endregion

        #region Negative

        [TestMethod]
        public void TestJsonToJsonNodePropertyHandlerWithNullNode()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(null);

                // Act
                connection.Insert(model);
                var isNull = IsRawValueNull(connection, model.SessionId);
                var result = connection.Query<JsonNodeAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.IsTrue(isNull);
                Assert.IsNull(result.Node);
            }
        }

        [TestMethod]
        public void TestJsonToJsonNodePropertyHandlerWithNullColumn()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = Guid.NewGuid();
                connection.ExecuteNonQuery(
                    "INSERT INTO [dbo].[CompleteTable] ([SessionId], [ColumnNVarChar]) VALUES (@SessionId, NULL);",
                    new { SessionId = sessionId });

                // Act
                var result = connection.Query<JsonNodeAttributeModel>(e => e.SessionId == sessionId).First();

                // Assert
                Assert.IsNull(result.Node);
            }
        }

        [TestMethod]
        public void TestJsonToJsonNodePropertyHandlerWithEmptyStringColumn()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, string.Empty);

                // Act
                var result = connection.Query<JsonNodeAttributeModel>(e => e.SessionId == sessionId).First();

                // Assert
                Assert.IsNull(result.Node);
            }
        }

        [TestMethod]
        public void TestJsonToJsonNodePropertyHandlerWithJsonNullLiteralColumn()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                // A JSON 'null' literal is parsed as a null node
                var sessionId = InsertRawValue(connection, "null");

                // Act
                var result = connection.Query<JsonNodeAttributeModel>(e => e.SessionId == sessionId).First();

                // Assert
                Assert.IsNull(result.Node);
            }
        }

        [TestMethod]
        public void TestJsonToJsonNodePropertyHandlerUpdateToNullNode()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(new JsonObject { ["v"] = 1 });
                connection.Insert(model);

                // Act
                model.Node = null;
                connection.Update(model);
                var isNull = IsRawValueNull(connection, model.SessionId);
                var result = connection.Query<JsonNodeAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.IsTrue(isNull);
                Assert.IsNull(result.Node);
            }
        }

        [TestMethod]
        public void TestJsonToJsonNodePropertyHandlerWithMalformedJson()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, "{\"name\":");

                // Act / Assert
                Assert.Throws<JsonException>(() =>
                    connection.Query<JsonNodeAttributeModel>(e => e.SessionId == sessionId).ToList());
            }
        }

        [TestMethod]
        public void TestJsonToJsonNodePropertyHandlerWithNonJsonText()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, "not a json");

                // Act / Assert
                Assert.Throws<JsonException>(() =>
                    connection.Query<JsonNodeAttributeModel>(e => e.SessionId == sessionId).ToList());
            }
        }

        [TestMethod]
        public void TestJsonToJsonNodePropertyHandlerWithNodeAlreadyAttachedToAParent()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                // Writing does not detach the node from its parent, therefore it is still usable after the insert
                var parent = new JsonObject { ["child"] = new JsonObject { ["v"] = 1 } };
                var model = CreateModel(parent["child"]);

                // Act
                connection.Insert(model);
                var raw = GetRawValue(connection, model.SessionId);

                // Assert
                Assert.AreEqual("{\"v\":1}", raw);
                Assert.AreSame(parent, model.Node.Parent);
            }
        }

        #endregion
    }
}
