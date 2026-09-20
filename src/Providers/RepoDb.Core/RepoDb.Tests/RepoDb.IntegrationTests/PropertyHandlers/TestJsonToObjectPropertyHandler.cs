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
    /// The integration tests of <see cref="JsonToObjectPropertyHandler{TObject}"/>. The <c>NVARCHAR(MAX)</c> column
    /// of the <c>[dbo].[CompleteTable]</c> is used to store the object (dictionary, list, etc.) as a JSON text.
    /// </summary>
    [TestClass]
    public class JsonToObjectPropertyHandlerTest
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
            PropertyHandlerMapper.Remove(typeof(Dictionary<string, int>));
            PropertyHandlerCache.Flush();
            Database.Cleanup();
        }

        #region Classes

        /// <summary>
        /// Property level (attribute): the handler is bound to the property via <see cref="PropertyHandlerAttribute"/>.
        /// </summary>
        [Map("[dbo].[CompleteTable]")]
        private class DictionaryAttributeModel
        {
            public Guid SessionId { get; set; }

            [Map("ColumnNVarChar"), PropertyHandler(typeof(JsonToObjectPropertyHandler<Dictionary<string, int>>))]
            public Dictionary<string, int> Values { get; set; }
        }

        [Map("[dbo].[CompleteTable]")]
        private class ListAttributeModel
        {
            public Guid SessionId { get; set; }

            [Map("ColumnNVarChar"), PropertyHandler(typeof(JsonToObjectPropertyHandler<List<int>>))]
            public List<int> Values { get; set; }
        }

        [Map("[dbo].[CompleteTable]")]
        private class IntegerAttributeModel
        {
            public Guid SessionId { get; set; }

            [Map("ColumnNVarChar"), PropertyHandler(typeof(JsonToObjectPropertyHandler<int?>))]
            public int? Value { get; set; }
        }

        [Map("[dbo].[CompleteTable]")]
        private class NestedAttributeModel
        {
            public Guid SessionId { get; set; }

            [Map("ColumnNVarChar"), PropertyHandler(typeof(JsonToObjectPropertyHandler<Dictionary<string, List<int>>>))]
            public Dictionary<string, List<int>> Values { get; set; }
        }

        /// <summary>
        /// Property level (fluent): the handler is bound to the property via <see cref="FluentMapper"/>.
        /// </summary>
        [Map("[dbo].[CompleteTable]")]
        private class DictionaryFluentModel
        {
            public Guid SessionId { get; set; }

            public Dictionary<string, int> ColumnNVarChar { get; set; }
        }

        /// <summary>
        /// Type level: the handler is bound to <see cref="Dictionary{TKey, TValue}"/> via <see cref="PropertyHandlerMapper"/>.
        /// </summary>
        [Map("[dbo].[CompleteTable]")]
        private class DictionaryTypeLevelModel
        {
            public Guid SessionId { get; set; }

            public Dictionary<string, int> ColumnNVarChar { get; set; }
        }

        #endregion

        #region Helpers

        private static DictionaryAttributeModel CreateModel(Dictionary<string, int> values) =>
            new DictionaryAttributeModel { SessionId = Guid.NewGuid(), Values = values };

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
        public void TestJsonToObjectPropertyHandler()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(new Dictionary<string, int> { { "one", 1 }, { "two", 2 }, { "three", 3 } });

                // Act
                connection.Insert(model);
                var result = connection.Query<DictionaryAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.IsNotNull(result.Values);
                Assert.AreEqual(3, result.Values.Count);
                Assert.AreEqual(1, result.Values["one"]);
                Assert.AreEqual(2, result.Values["two"]);
                Assert.AreEqual(3, result.Values["three"]);
            }
        }

        [TestMethod]
        public async Task TestJsonToObjectPropertyHandlerAsync()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(new Dictionary<string, int> { { "one", 1 }, { "two", 2 } });

                // Act
                await connection.InsertAsync(model).ConfigureAwait(false);
                var result = (await connection.QueryAsync<DictionaryAttributeModel>(e => e.SessionId == model.SessionId).ConfigureAwait(false)).First();

                // Assert
                Assert.AreEqual(2, result.Values.Count);
                Assert.AreEqual(1, result.Values["one"]);
                Assert.AreEqual(2, result.Values["two"]);
            }
        }

        [TestMethod]
        public void TestJsonToObjectPropertyHandlerWritesJsonText()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(new Dictionary<string, int> { { "one", 1 }, { "two", 2 } });

                // Act
                connection.Insert(model);
                var raw = GetRawValue(connection, model.SessionId);

                // Assert
                Assert.AreEqual("{\"one\":1,\"two\":2}", raw);
            }
        }

        [TestMethod]
        public void TestJsonToObjectPropertyHandlerReadsJsonText()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, "{\"a\":10,\"b\":20}");

                // Act
                var result = connection.Query<DictionaryAttributeModel>(e => e.SessionId == sessionId).First();

                // Assert
                Assert.AreEqual(10, result.Values["a"]);
                Assert.AreEqual(20, result.Values["b"]);
            }
        }

        [TestMethod]
        public void TestJsonToObjectPropertyHandlerWithEmptyObject()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(new Dictionary<string, int>());

                // Act
                connection.Insert(model);
                var raw = GetRawValue(connection, model.SessionId);
                var result = connection.Query<DictionaryAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual("{}", raw);
                Assert.IsNotNull(result.Values);
                Assert.AreEqual(0, result.Values.Count);
            }
        }

        [TestMethod]
        public void TestJsonToObjectPropertyHandlerWithList()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = new ListAttributeModel { SessionId = Guid.NewGuid(), Values = new List<int> { 3, 1, 2 } };

                // Act
                connection.Insert(model);
                var raw = GetRawValue(connection, model.SessionId);
                var result = connection.Query<ListAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual("[3,1,2]", raw);
                CollectionAssert.AreEqual(model.Values, result.Values);
            }
        }

        [TestMethod]
        public void TestJsonToObjectPropertyHandlerWithEmptyList()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = new ListAttributeModel { SessionId = Guid.NewGuid(), Values = new List<int>() };

                // Act
                connection.Insert(model);
                var raw = GetRawValue(connection, model.SessionId);
                var result = connection.Query<ListAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual("[]", raw);
                Assert.IsNotNull(result.Values);
                Assert.AreEqual(0, result.Values.Count);
            }
        }

        [TestMethod]
        public void TestJsonToObjectPropertyHandlerWithScalarValue()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = new IntegerAttributeModel { SessionId = Guid.NewGuid(), Value = 12345 };

                // Act
                connection.Insert(model);
                var raw = GetRawValue(connection, model.SessionId);
                var result = connection.Query<IntegerAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual("12345", raw);
                Assert.AreEqual(12345, result.Value);
            }
        }

        [TestMethod]
        public void TestJsonToObjectPropertyHandlerWithNestedGenerics()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = new NestedAttributeModel
                {
                    SessionId = Guid.NewGuid(),
                    Values = new Dictionary<string, List<int>> { { "odd", new List<int> { 1, 3 } }, { "even", new List<int> { 2, 4 } } }
                };

                // Act
                connection.Insert(model);
                var result = connection.Query<NestedAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                CollectionAssert.AreEqual(new[] { 1, 3 }, result.Values["odd"]);
                CollectionAssert.AreEqual(new[] { 2, 4 }, result.Values["even"]);
            }
        }

        [TestMethod]
        public void TestJsonToObjectPropertyHandlerWithUnicodeKeys()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(new Dictionary<string, int> { { "日本語", 1 }, { "Ñandú \"q\"", 2 } });

                // Act
                connection.Insert(model);
                var result = connection.Query<DictionaryAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual(1, result.Values["日本語"]);
                Assert.AreEqual(2, result.Values["Ñandú \"q\""]);
            }
        }

        [TestMethod]
        public void TestJsonToObjectPropertyHandlerOnInsertAll()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var models = Enumerable.Range(1, 10)
                    .Select(i => CreateModel(new Dictionary<string, int> { { $"key-{i}", i } }))
                    .ToList();

                // Act
                connection.InsertAll(models);
                var result = connection.QueryAll<DictionaryAttributeModel>().ToList();

                // Assert
                Assert.AreEqual(models.Count, result.Count);
                foreach (var model in models)
                {
                    var item = result.First(e => e.SessionId == model.SessionId);
                    Assert.AreEqual(model.Values.First().Value, item.Values[model.Values.First().Key]);
                }
            }
        }

        [TestMethod]
        public void TestJsonToObjectPropertyHandlerOnUpdate()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(new Dictionary<string, int> { { "before", 1 } });
                connection.Insert(model);

                // Act
                model.Values = new Dictionary<string, int> { { "after", 2 } };
                var affectedRows = connection.Update(model);
                var result = connection.Query<DictionaryAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual(1, affectedRows);
                Assert.AreEqual(1, result.Values.Count);
                Assert.AreEqual(2, result.Values["after"]);
            }
        }

        [TestMethod]
        public void TestJsonToObjectPropertyHandlerViaFluentMapper()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                FluentMapper
                    .Entity<DictionaryFluentModel>()
                    .PropertyHandler(e => e.ColumnNVarChar, new JsonToObjectPropertyHandler<Dictionary<string, int>>());
                var model = new DictionaryFluentModel { SessionId = Guid.NewGuid(), ColumnNVarChar = new Dictionary<string, int> { { "k", 7 } } };

                // Act
                connection.Insert(model);
                var result = connection.Query<DictionaryFluentModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual(7, result.ColumnNVarChar["k"]);
            }
        }

        [TestMethod]
        public void TestJsonToObjectPropertyHandlerViaTypeLevelMapper()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                PropertyHandlerMapper.Add<Dictionary<string, int>, JsonToObjectPropertyHandler<Dictionary<string, int>>>(true);
                var model = new DictionaryTypeLevelModel { SessionId = Guid.NewGuid(), ColumnNVarChar = new Dictionary<string, int> { { "k", 7 } } };

                // Act
                connection.Insert(model);
                var result = connection.Query<DictionaryTypeLevelModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual(7, result.ColumnNVarChar["k"]);
            }
        }

        #endregion

        #region Negative

        [TestMethod]
        public void TestJsonToObjectPropertyHandlerWithNullObject()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(null);

                // Act
                connection.Insert(model);
                var isNull = IsRawValueNull(connection, model.SessionId);
                var result = connection.Query<DictionaryAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.IsTrue(isNull);
                Assert.IsNull(result.Values);
            }
        }

        [TestMethod]
        public void TestJsonToObjectPropertyHandlerWithNullColumn()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = Guid.NewGuid();
                connection.ExecuteNonQuery(
                    "INSERT INTO [dbo].[CompleteTable] ([SessionId], [ColumnNVarChar]) VALUES (@SessionId, NULL);",
                    new { SessionId = sessionId });

                // Act
                var result = connection.Query<DictionaryAttributeModel>(e => e.SessionId == sessionId).First();

                // Assert
                Assert.IsNull(result.Values);
            }
        }

        [TestMethod]
        public void TestJsonToObjectPropertyHandlerWithNullScalarValue()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = new IntegerAttributeModel { SessionId = Guid.NewGuid(), Value = null };

                // Act
                connection.Insert(model);
                var isNull = IsRawValueNull(connection, model.SessionId);
                var result = connection.Query<IntegerAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.IsTrue(isNull);
                Assert.IsNull(result.Value);
            }
        }

        [TestMethod]
        public void TestJsonToObjectPropertyHandlerWithEmptyStringColumn()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, string.Empty);

                // Act
                var result = connection.Query<DictionaryAttributeModel>(e => e.SessionId == sessionId).First();

                // Assert
                Assert.IsNull(result.Values);
            }
        }

        [TestMethod]
        public void TestJsonToObjectPropertyHandlerWithJsonNullLiteralColumn()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, "null");

                // Act
                var result = connection.Query<DictionaryAttributeModel>(e => e.SessionId == sessionId).First();

                // Assert
                Assert.IsNull(result.Values);
            }
        }

        [TestMethod]
        public void TestJsonToObjectPropertyHandlerWithMalformedJson()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, "{\"a\":1,");

                // Act / Assert
                Assert.Throws<JsonException>(() =>
                    connection.Query<DictionaryAttributeModel>(e => e.SessionId == sessionId).ToList());
            }
        }

        [TestMethod]
        public void TestJsonToObjectPropertyHandlerWithMismatchedJsonShape()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                // A JSON array cannot be bound to a dictionary
                var sessionId = InsertRawValue(connection, "[1,2,3]");

                // Act / Assert
                Assert.Throws<JsonException>(() =>
                    connection.Query<DictionaryAttributeModel>(e => e.SessionId == sessionId).ToList());
            }
        }

        [TestMethod]
        public void TestJsonToObjectPropertyHandlerWithMismatchedValueType()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                // The dictionary values are int, but the JSON has a string
                var sessionId = InsertRawValue(connection, "{\"a\":\"text\"}");

                // Act / Assert
                Assert.Throws<JsonException>(() =>
                    connection.Query<DictionaryAttributeModel>(e => e.SessionId == sessionId).ToList());
            }
        }

        #endregion
    }
}
