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
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace RepoDb.IntegrationTests.PropertyHandlers
{
    /// <summary>
    /// The integration tests of <see cref="JsonToJsonElementPropertyHandler"/>. The <c>NVARCHAR(MAX)</c> column
    /// of the <c>[dbo].[CompleteTable]</c> is used to store the <see cref="JsonElement"/> as a JSON text.
    /// </summary>
    [TestClass]
    public class JsonToJsonElementPropertyHandlerTest
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
            PropertyHandlerMapper.Remove(typeof(JsonElement));
            PropertyHandlerCache.Flush();
            Database.Cleanup();
        }

        #region Classes

        /// <summary>
        /// Property level (attribute): the handler is bound to the property via <see cref="PropertyHandlerAttribute"/>.
        /// </summary>
        [Map("[dbo].[CompleteTable]")]
        private class JsonElementAttributeModel
        {
            public Guid SessionId { get; set; }

            [Map("ColumnNVarChar"), PropertyHandler(typeof(JsonToJsonElementPropertyHandler))]
            public JsonElement Element { get; set; }
        }

        /// <summary>
        /// Property level (fluent): the handler is bound to the property via <see cref="FluentMapper"/>.
        /// </summary>
        [Map("[dbo].[CompleteTable]")]
        private class JsonElementFluentModel
        {
            public Guid SessionId { get; set; }

            public JsonElement ColumnNVarChar { get; set; }
        }

        /// <summary>
        /// Type level: the handler is bound to <see cref="JsonElement"/> via <see cref="PropertyHandlerMapper"/>.
        /// </summary>
        [Map("[dbo].[CompleteTable]")]
        private class JsonElementTypeLevelModel
        {
            public Guid SessionId { get; set; }

            public JsonElement ColumnNVarChar { get; set; }
        }

        #endregion

        #region Helpers

        private static JsonElementAttributeModel CreateModel(string json) =>
            new JsonElementAttributeModel
            {
                SessionId = Guid.NewGuid(),
                Element = json == null ? default : Parse(json)
            };

        private static JsonElement Parse(string json)
        {
            using (var document = JsonDocument.Parse(json))
            {
                return document.RootElement.Clone();
            }
        }

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
        public void TestJsonToJsonElementPropertyHandler()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel("{\"name\":\"John\",\"age\":30}");

                // Act
                connection.Insert(model);
                var result = connection.Query<JsonElementAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual(JsonValueKind.Object, result.Element.ValueKind);
                Assert.AreEqual("John", result.Element.GetProperty("name").GetString());
                Assert.AreEqual(30, result.Element.GetProperty("age").GetInt32());
            }
        }

        [TestMethod]
        public async Task TestJsonToJsonElementPropertyHandlerAsync()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel("{\"name\":\"John\",\"age\":30}");

                // Act
                await connection.InsertAsync(model).ConfigureAwait(false);
                var result = (await connection.QueryAsync<JsonElementAttributeModel>(e => e.SessionId == model.SessionId).ConfigureAwait(false)).First();

                // Assert
                Assert.AreEqual("John", result.Element.GetProperty("name").GetString());
            }
        }

        [TestMethod]
        public void TestJsonToJsonElementPropertyHandlerWritesRawJsonText()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel("{\"name\":\"John\",\"tags\":[1,2]}");

                // Act
                connection.Insert(model);
                var raw = GetRawValue(connection, model.SessionId);

                // Assert
                Assert.AreEqual("{\"name\":\"John\",\"tags\":[1,2]}", raw);
            }
        }

        [TestMethod]
        public void TestJsonToJsonElementPropertyHandlerReadsJsonText()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, "{\"items\":[1,2,3],\"nested\":{\"flag\":true}}");

                // Act
                var result = connection.Query<JsonElementAttributeModel>(e => e.SessionId == sessionId).First();

                // Assert
                Assert.AreEqual(3, result.Element.GetProperty("items").GetArrayLength());
                Assert.IsTrue(result.Element.GetProperty("nested").GetProperty("flag").GetBoolean());
            }
        }

        [TestMethod]
        public void TestJsonToJsonElementPropertyHandlerStaysUsableAfterTheReadIsCompleted()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel("{\"name\":\"John\"}");
                connection.Insert(model);

                // Act
                var result = connection.Query<JsonElementAttributeModel>(e => e.SessionId == model.SessionId).First();
                GC.Collect();
                GC.WaitForPendingFinalizers();

                // Assert
                // The element is a clone, therefore it does not depend on the (disposed) document used by the handler
                Assert.AreEqual("John", result.Element.GetProperty("name").GetString());
            }
        }

        [TestMethod]
        public void TestJsonToJsonElementPropertyHandlerWithValueKinds()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var expectations = new[]
                {
                    ("{\"a\":1}", JsonValueKind.Object),
                    ("[1,2,3]", JsonValueKind.Array),
                    ("\"text\"", JsonValueKind.String),
                    ("12.5", JsonValueKind.Number),
                    ("true", JsonValueKind.True),
                    ("false", JsonValueKind.False)
                };

                foreach (var (json, kind) in expectations)
                {
                    var model = CreateModel(json);

                    // Act
                    connection.Insert(model);
                    var raw = GetRawValue(connection, model.SessionId);
                    var result = connection.Query<JsonElementAttributeModel>(e => e.SessionId == model.SessionId).First();

                    // Assert
                    Assert.AreEqual(json, raw);
                    Assert.AreEqual(kind, result.Element.ValueKind);
                }
            }
        }

        [TestMethod]
        public void TestJsonToJsonElementPropertyHandlerWithJsonNullLiteral()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                // A JSON 'null' literal is a valid element (kind 'Null'), therefore it is not the same as a database NULL
                var model = CreateModel("null");

                // Act
                connection.Insert(model);
                var isNull = IsRawValueNull(connection, model.SessionId);
                var raw = GetRawValue(connection, model.SessionId);
                var result = connection.Query<JsonElementAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.IsFalse(isNull);
                Assert.AreEqual("null", raw);
                Assert.AreEqual(JsonValueKind.Null, result.Element.ValueKind);
            }
        }

        [TestMethod]
        public void TestJsonToJsonElementPropertyHandlerWithUnicodeAndEscapedCharacters()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel("{\"text\":\"日本語 \\\"quoted\\\" Ñandú 😀\"}");

                // Act
                connection.Insert(model);
                var result = connection.Query<JsonElementAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual("日本語 \"quoted\" Ñandú 😀", result.Element.GetProperty("text").GetString());
            }
        }

        [TestMethod]
        public void TestJsonToJsonElementPropertyHandlerOnInsertAll()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var models = Enumerable.Range(1, 10).Select(i => CreateModel($"{{\"id\":{i}}}")).ToList();

                // Act
                connection.InsertAll(models);
                var result = connection.QueryAll<JsonElementAttributeModel>().ToList();

                // Assert
                Assert.AreEqual(models.Count, result.Count);
                for (var i = 0; i < models.Count; i++)
                {
                    var item = result.First(e => e.SessionId == models[i].SessionId);
                    Assert.AreEqual(i + 1, item.Element.GetProperty("id").GetInt32());
                }
            }
        }

        [TestMethod]
        public void TestJsonToJsonElementPropertyHandlerOnUpdate()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel("{\"v\":1}");
                connection.Insert(model);

                // Act
                model.Element = Parse("{\"v\":2}");
                var affectedRows = connection.Update(model);
                var result = connection.Query<JsonElementAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual(1, affectedRows);
                Assert.AreEqual(2, result.Element.GetProperty("v").GetInt32());
            }
        }

        [TestMethod]
        public void TestJsonToJsonElementPropertyHandlerOnExecuteQuery()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel("{\"v\":1}");
                connection.Insert(model);

                // Act
                var result = connection.ExecuteQuery<JsonElementAttributeModel>(
                    "SELECT [SessionId], [ColumnNVarChar] FROM [dbo].[CompleteTable] WHERE [SessionId] = @SessionId;",
                    new { SessionId = model.SessionId }).First();

                // Assert
                Assert.AreEqual(1, result.Element.GetProperty("v").GetInt32());
            }
        }

        [TestMethod]
        public void TestJsonToJsonElementPropertyHandlerViaFluentMapper()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                FluentMapper
                    .Entity<JsonElementFluentModel>()
                    .PropertyHandler(e => e.ColumnNVarChar, new JsonToJsonElementPropertyHandler());
                var model = new JsonElementFluentModel { SessionId = Guid.NewGuid(), ColumnNVarChar = Parse("{\"v\":1}") };

                // Act
                connection.Insert(model);
                var result = connection.Query<JsonElementFluentModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual(1, result.ColumnNVarChar.GetProperty("v").GetInt32());
            }
        }

        [TestMethod]
        public void TestJsonToJsonElementPropertyHandlerViaTypeLevelMapper()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                PropertyHandlerMapper.Add<JsonElement, JsonToJsonElementPropertyHandler>(true);
                var model = new JsonElementTypeLevelModel { SessionId = Guid.NewGuid(), ColumnNVarChar = Parse("{\"v\":1}") };

                // Act
                connection.Insert(model);
                var result = connection.Query<JsonElementTypeLevelModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual(1, result.ColumnNVarChar.GetProperty("v").GetInt32());
            }
        }

        #endregion

        #region Negative

        [TestMethod]
        public void TestJsonToJsonElementPropertyHandlerWithUndefinedElement()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                // A default 'JsonElement' (kind 'Undefined') is written as NULL
                var model = CreateModel(null);

                // Act
                connection.Insert(model);
                var isNull = IsRawValueNull(connection, model.SessionId);
                var result = connection.Query<JsonElementAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.IsTrue(isNull);
                Assert.AreEqual(JsonValueKind.Undefined, result.Element.ValueKind);
            }
        }

        [TestMethod]
        public void TestJsonToJsonElementPropertyHandlerWithNullColumn()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = Guid.NewGuid();
                connection.ExecuteNonQuery(
                    "INSERT INTO [dbo].[CompleteTable] ([SessionId], [ColumnNVarChar]) VALUES (@SessionId, NULL);",
                    new { SessionId = sessionId });

                // Act
                var result = connection.Query<JsonElementAttributeModel>(e => e.SessionId == sessionId).First();

                // Assert
                Assert.AreEqual(JsonValueKind.Undefined, result.Element.ValueKind);
            }
        }

        [TestMethod]
        public void TestJsonToJsonElementPropertyHandlerWithEmptyStringColumn()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, string.Empty);

                // Act
                var result = connection.Query<JsonElementAttributeModel>(e => e.SessionId == sessionId).First();

                // Assert
                Assert.AreEqual(JsonValueKind.Undefined, result.Element.ValueKind);
            }
        }

        [TestMethod]
        public void TestJsonToJsonElementPropertyHandlerUpdateToUndefinedElement()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel("{\"v\":1}");
                connection.Insert(model);

                // Act
                model.Element = default;
                connection.Update(model);
                var isNull = IsRawValueNull(connection, model.SessionId);
                var result = connection.Query<JsonElementAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.IsTrue(isNull);
                Assert.AreEqual(JsonValueKind.Undefined, result.Element.ValueKind);
            }
        }

        [TestMethod]
        public void TestJsonToJsonElementPropertyHandlerWithMalformedJson()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, "{\"name\":");

                // Act / Assert
                Assert.Throws<JsonException>(() =>
                    connection.Query<JsonElementAttributeModel>(e => e.SessionId == sessionId).ToList());
            }
        }

        [TestMethod]
        public void TestJsonToJsonElementPropertyHandlerWithNonJsonText()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, "not a json");

                // Act / Assert
                Assert.Throws<JsonException>(() =>
                    connection.Query<JsonElementAttributeModel>(e => e.SessionId == sessionId).ToList());
            }
        }

        [TestMethod]
        public void TestJsonToJsonElementPropertyHandlerReadingMissingPropertyOfTheElement()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel("{\"name\":\"John\"}");
                connection.Insert(model);

                // Act
                var result = connection.Query<JsonElementAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.IsFalse(result.Element.TryGetProperty("missing", out _));
                Assert.Throws<KeyNotFoundException>(() => result.Element.GetProperty("missing"));
            }
        }

        #endregion
    }
}
