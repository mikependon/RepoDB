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
using System.Threading.Tasks;

namespace RepoDb.IntegrationTests.PropertyHandlers
{
    /// <summary>
    /// The integration tests of <see cref="JsonToJsonDocumentPropertyHandler"/>. The <c>NVARCHAR(MAX)</c> column
    /// of the <c>[dbo].[CompleteTable]</c> is used to store the <see cref="JsonDocument"/> as a JSON text.
    /// </summary>
    [TestClass]
    public class JsonToJsonDocumentPropertyHandlerTest
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
            PropertyHandlerMapper.Remove(typeof(JsonDocument));
            PropertyHandlerCache.Flush();
            Database.Cleanup();
        }

        #region Classes

        /// <summary>
        /// Property level (attribute): the handler is bound to the property via <see cref="PropertyHandlerAttribute"/>.
        /// </summary>
        [Map("[dbo].[CompleteTable]")]
        private class JsonDocumentAttributeModel
        {
            public Guid SessionId { get; set; }

            [Map("ColumnNVarChar"), PropertyHandler(typeof(JsonToJsonDocumentPropertyHandler))]
            public JsonDocument Document { get; set; }
        }

        /// <summary>
        /// Property level (fluent): the handler is bound to the property via <see cref="FluentMapper"/>.
        /// </summary>
        [Map("[dbo].[CompleteTable]")]
        private class JsonDocumentFluentModel
        {
            public Guid SessionId { get; set; }

            public JsonDocument ColumnNVarChar { get; set; }
        }

        /// <summary>
        /// Type level: the handler is bound to <see cref="JsonDocument"/> via <see cref="PropertyHandlerMapper"/>.
        /// </summary>
        [Map("[dbo].[CompleteTable]")]
        private class JsonDocumentTypeLevelModel
        {
            public Guid SessionId { get; set; }

            public JsonDocument ColumnNVarChar { get; set; }
        }

        #endregion

        #region Helpers

        private static JsonDocumentAttributeModel CreateModel(JsonDocument document) =>
            new JsonDocumentAttributeModel { SessionId = Guid.NewGuid(), Document = document };

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
        public void TestJsonToJsonDocumentPropertyHandler()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                using var document = JsonDocument.Parse("{\"name\":\"John\",\"age\":30}");
                var model = CreateModel(document);

                // Act
                connection.Insert(model);
                var result = connection.Query<JsonDocumentAttributeModel>(e => e.SessionId == model.SessionId).First();
                using var resultDocument = result.Document;

                // Assert
                Assert.IsNotNull(resultDocument);
                Assert.AreEqual("John", resultDocument.RootElement.GetProperty("name").GetString());
                Assert.AreEqual(30, resultDocument.RootElement.GetProperty("age").GetInt32());
            }
        }

        [TestMethod]
        public async Task TestJsonToJsonDocumentPropertyHandlerAsync()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                using var document = JsonDocument.Parse("{\"name\":\"John\",\"age\":30}");
                var model = CreateModel(document);

                // Act
                await connection.InsertAsync(model).ConfigureAwait(false);
                var result = (await connection.QueryAsync<JsonDocumentAttributeModel>(e => e.SessionId == model.SessionId).ConfigureAwait(false)).First();
                using var resultDocument = result.Document;

                // Assert
                Assert.IsNotNull(resultDocument);
                Assert.AreEqual("John", resultDocument.RootElement.GetProperty("name").GetString());
            }
        }

        [TestMethod]
        public void TestJsonToJsonDocumentPropertyHandlerWritesRawJsonText()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                using var document = JsonDocument.Parse("{\"name\":\"John\",\"age\":30}");
                var model = CreateModel(document);

                // Act
                connection.Insert(model);
                var raw = GetRawValue(connection, model.SessionId);

                // Assert
                Assert.AreEqual("{\"name\":\"John\",\"age\":30}", raw);
            }
        }

        [TestMethod]
        public void TestJsonToJsonDocumentPropertyHandlerPreservesTheOriginalText()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                // The raw text of the root element is written as-is, including the whitespaces
                using var document = JsonDocument.Parse("{ \"name\" : \"John\",\n  \"age\" : 30 }");
                var model = CreateModel(document);

                // Act
                connection.Insert(model);
                var raw = GetRawValue(connection, model.SessionId);

                // Assert
                Assert.AreEqual("{ \"name\" : \"John\",\n  \"age\" : 30 }", raw);
            }
        }

        [TestMethod]
        public void TestJsonToJsonDocumentPropertyHandlerReadsJsonText()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, "{\"items\":[1,2,3],\"nested\":{\"flag\":true}}");

                // Act
                var result = connection.Query<JsonDocumentAttributeModel>(e => e.SessionId == sessionId).First();
                using var resultDocument = result.Document;

                // Assert
                Assert.AreEqual(JsonValueKind.Object, resultDocument.RootElement.ValueKind);
                Assert.AreEqual(3, resultDocument.RootElement.GetProperty("items").GetArrayLength());
                Assert.IsTrue(resultDocument.RootElement.GetProperty("nested").GetProperty("flag").GetBoolean());
            }
        }

        [TestMethod]
        public void TestJsonToJsonDocumentPropertyHandlerWithArrayRoot()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                using var document = JsonDocument.Parse("[1,2,3]");
                var model = CreateModel(document);

                // Act
                connection.Insert(model);
                var result = connection.Query<JsonDocumentAttributeModel>(e => e.SessionId == model.SessionId).First();
                using var resultDocument = result.Document;

                // Assert
                Assert.AreEqual(JsonValueKind.Array, resultDocument.RootElement.ValueKind);
                Assert.AreEqual(3, resultDocument.RootElement.GetArrayLength());
            }
        }

        [TestMethod]
        public void TestJsonToJsonDocumentPropertyHandlerWithScalarRoots()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var expectations = new[]
                {
                    ("\"text\"", JsonValueKind.String),
                    ("12.5", JsonValueKind.Number),
                    ("true", JsonValueKind.True),
                    ("false", JsonValueKind.False)
                };

                foreach (var (json, kind) in expectations)
                {
                    using var document = JsonDocument.Parse(json);
                    var model = CreateModel(document);

                    // Act
                    connection.Insert(model);
                    var raw = GetRawValue(connection, model.SessionId);
                    var result = connection.Query<JsonDocumentAttributeModel>(e => e.SessionId == model.SessionId).First();
                    using var resultDocument = result.Document;

                    // Assert
                    Assert.AreEqual(json, raw);
                    Assert.AreEqual(kind, resultDocument.RootElement.ValueKind);
                }
            }
        }

        [TestMethod]
        public void TestJsonToJsonDocumentPropertyHandlerWithJsonNullLiteralRoot()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                // A JSON 'null' literal is a valid document, therefore it is not the same as a database NULL
                using var document = JsonDocument.Parse("null");
                var model = CreateModel(document);

                // Act
                connection.Insert(model);
                var isNull = IsRawValueNull(connection, model.SessionId);
                var raw = GetRawValue(connection, model.SessionId);
                var result = connection.Query<JsonDocumentAttributeModel>(e => e.SessionId == model.SessionId).First();
                using var resultDocument = result.Document;

                // Assert
                Assert.IsFalse(isNull);
                Assert.AreEqual("null", raw);
                Assert.IsNotNull(resultDocument);
                Assert.AreEqual(JsonValueKind.Null, resultDocument.RootElement.ValueKind);
            }
        }

        [TestMethod]
        public void TestJsonToJsonDocumentPropertyHandlerWithUnicodeAndEscapedCharacters()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                using var document = JsonDocument.Parse("{\"text\":\"日本語 \\\"quoted\\\" Ñandú 😀\"}");
                var model = CreateModel(document);

                // Act
                connection.Insert(model);
                var result = connection.Query<JsonDocumentAttributeModel>(e => e.SessionId == model.SessionId).First();
                using var resultDocument = result.Document;

                // Assert
                Assert.AreEqual("日本語 \"quoted\" Ñandú 😀", resultDocument.RootElement.GetProperty("text").GetString());
            }
        }

        [TestMethod]
        public void TestJsonToJsonDocumentPropertyHandlerOnInsertAll()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var documents = Enumerable.Range(1, 10).Select(i => JsonDocument.Parse($"{{\"id\":{i}}}")).ToList();
                var models = documents.Select(CreateModel).ToList();

                // Act
                connection.InsertAll(models);
                var result = connection.QueryAll<JsonDocumentAttributeModel>().ToList();

                // Assert
                Assert.AreEqual(models.Count, result.Count);
                for (var i = 0; i < models.Count; i++)
                {
                    var item = result.First(e => e.SessionId == models[i].SessionId);
                    Assert.AreEqual(i + 1, item.Document.RootElement.GetProperty("id").GetInt32());
                    item.Document.Dispose();
                }
                documents.ForEach(e => e.Dispose());
            }
        }

        [TestMethod]
        public void TestJsonToJsonDocumentPropertyHandlerOnUpdate()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                using var before = JsonDocument.Parse("{\"v\":1}");
                using var after = JsonDocument.Parse("{\"v\":2}");
                var model = CreateModel(before);
                connection.Insert(model);

                // Act
                model.Document = after;
                var affectedRows = connection.Update(model);
                var result = connection.Query<JsonDocumentAttributeModel>(e => e.SessionId == model.SessionId).First();
                using var resultDocument = result.Document;

                // Assert
                Assert.AreEqual(1, affectedRows);
                Assert.AreEqual(2, resultDocument.RootElement.GetProperty("v").GetInt32());
            }
        }

        [TestMethod]
        public void TestJsonToJsonDocumentPropertyHandlerOnExecuteQuery()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                using var document = JsonDocument.Parse("{\"v\":1}");
                var model = CreateModel(document);
                connection.Insert(model);

                // Act
                var result = connection.ExecuteQuery<JsonDocumentAttributeModel>(
                    "SELECT [SessionId], [ColumnNVarChar] FROM [dbo].[CompleteTable] WHERE [SessionId] = @SessionId;",
                    new { SessionId = model.SessionId }).First();
                using var resultDocument = result.Document;

                // Assert
                Assert.AreEqual(1, resultDocument.RootElement.GetProperty("v").GetInt32());
            }
        }

        [TestMethod]
        public void TestJsonToJsonDocumentPropertyHandlerViaFluentMapper()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                FluentMapper
                    .Entity<JsonDocumentFluentModel>()
                    .PropertyHandler(e => e.ColumnNVarChar, new JsonToJsonDocumentPropertyHandler());
                using var document = JsonDocument.Parse("{\"v\":1}");
                var model = new JsonDocumentFluentModel { SessionId = Guid.NewGuid(), ColumnNVarChar = document };

                // Act
                connection.Insert(model);
                var result = connection.Query<JsonDocumentFluentModel>(e => e.SessionId == model.SessionId).First();
                using var resultDocument = result.ColumnNVarChar;

                // Assert
                Assert.AreEqual(1, resultDocument.RootElement.GetProperty("v").GetInt32());
            }
        }

        [TestMethod]
        public void TestJsonToJsonDocumentPropertyHandlerViaTypeLevelMapper()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                PropertyHandlerMapper.Add<JsonDocument, JsonToJsonDocumentPropertyHandler>(true);
                using var document = JsonDocument.Parse("{\"v\":1}");
                var model = new JsonDocumentTypeLevelModel { SessionId = Guid.NewGuid(), ColumnNVarChar = document };

                // Act
                connection.Insert(model);
                var result = connection.Query<JsonDocumentTypeLevelModel>(e => e.SessionId == model.SessionId).First();
                using var resultDocument = result.ColumnNVarChar;

                // Assert
                Assert.AreEqual(1, resultDocument.RootElement.GetProperty("v").GetInt32());
            }
        }

        #endregion

        #region Negative

        [TestMethod]
        public void TestJsonToJsonDocumentPropertyHandlerWithNullDocument()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(null);

                // Act
                connection.Insert(model);
                var isNull = IsRawValueNull(connection, model.SessionId);
                var result = connection.Query<JsonDocumentAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.IsTrue(isNull);
                Assert.IsNull(result.Document);
            }
        }

        [TestMethod]
        public void TestJsonToJsonDocumentPropertyHandlerWithNullColumn()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = Guid.NewGuid();
                connection.ExecuteNonQuery(
                    "INSERT INTO [dbo].[CompleteTable] ([SessionId], [ColumnNVarChar]) VALUES (@SessionId, NULL);",
                    new { SessionId = sessionId });

                // Act
                var result = connection.Query<JsonDocumentAttributeModel>(e => e.SessionId == sessionId).First();

                // Assert
                Assert.IsNull(result.Document);
            }
        }

        [TestMethod]
        public void TestJsonToJsonDocumentPropertyHandlerWithEmptyStringColumn()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, string.Empty);

                // Act
                var result = connection.Query<JsonDocumentAttributeModel>(e => e.SessionId == sessionId).First();

                // Assert
                Assert.IsNull(result.Document);
            }
        }

        [TestMethod]
        public void TestJsonToJsonDocumentPropertyHandlerUpdateToNullDocument()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                using var document = JsonDocument.Parse("{\"v\":1}");
                var model = CreateModel(document);
                connection.Insert(model);

                // Act
                model.Document = null;
                connection.Update(model);
                var isNull = IsRawValueNull(connection, model.SessionId);
                var result = connection.Query<JsonDocumentAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.IsTrue(isNull);
                Assert.IsNull(result.Document);
            }
        }

        [TestMethod]
        public void TestJsonToJsonDocumentPropertyHandlerWithMalformedJson()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, "{\"name\":");

                // Act / Assert
                Assert.Throws<JsonException>(() =>
                    connection.Query<JsonDocumentAttributeModel>(e => e.SessionId == sessionId).ToList());
            }
        }

        [TestMethod]
        public void TestJsonToJsonDocumentPropertyHandlerWithNonJsonText()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, "not a json");

                // Act / Assert
                Assert.Throws<JsonException>(() =>
                    connection.Query<JsonDocumentAttributeModel>(e => e.SessionId == sessionId).ToList());
            }
        }

        [TestMethod]
        public void TestJsonToJsonDocumentPropertyHandlerWithDisposedDocument()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var document = JsonDocument.Parse("{\"v\":1}");
                document.Dispose();
                var model = CreateModel(document);

                // Act / Assert
                Assert.Throws<ObjectDisposedException>(() => connection.Insert(model));
            }
        }

        #endregion
    }
}
