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
    /// The integration tests of <see cref="JsonToEntityPropertyHandler{TEntity}"/>. The <c>NVARCHAR(MAX)</c> column
    /// of the <c>[dbo].[CompleteTable]</c> is used to store the entity as a JSON text.
    /// </summary>
    [TestClass]
    public class JsonToEntityPropertyHandlerTest
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
            PropertyHandlerMapper.Remove(typeof(Person));
            PropertyHandlerCache.Flush();
            Database.Cleanup();
        }

        #region Classes

        public class Person
        {
            public string Name { get; set; }

            public int Age { get; set; }

            public List<string> Tags { get; set; }
        }

        public class Company
        {
            public string Name { get; set; }

            public Person Owner { get; set; }

            public List<Person> Employees { get; set; }

            public Dictionary<string, int> Metrics { get; set; }
        }

        /// <summary>
        /// Property level (attribute): the handler is bound to the property via <see cref="PropertyHandlerAttribute"/>.
        /// </summary>
        [Map("[dbo].[CompleteTable]")]
        private class PersonAttributeModel
        {
            public Guid SessionId { get; set; }

            [Map("ColumnNVarChar"), PropertyHandler(typeof(JsonToEntityPropertyHandler<Person>))]
            public Person Owner { get; set; }
        }

        [Map("[dbo].[CompleteTable]")]
        private class CompanyAttributeModel
        {
            public Guid SessionId { get; set; }

            [Map("ColumnNVarChar"), PropertyHandler(typeof(JsonToEntityPropertyHandler<Company>))]
            public Company Company { get; set; }
        }

        /// <summary>
        /// Property level (fluent): the handler is bound to the property via <see cref="FluentMapper"/>.
        /// </summary>
        [Map("[dbo].[CompleteTable]")]
        private class PersonFluentModel
        {
            public Guid SessionId { get; set; }

            public Person ColumnNVarChar { get; set; }
        }

        /// <summary>
        /// Type level: the handler is bound to <see cref="Person"/> via <see cref="PropertyHandlerMapper"/>.
        /// </summary>
        [Map("[dbo].[CompleteTable]")]
        private class PersonTypeLevelModel
        {
            public Guid SessionId { get; set; }

            public Person ColumnNVarChar { get; set; }
        }

        #endregion

        #region Helpers

        private static PersonAttributeModel CreateModel(Person owner) =>
            new PersonAttributeModel { SessionId = Guid.NewGuid(), Owner = owner };

        private static Person CreatePerson(string name = "John Doe",
            int age = 30) =>
            new Person { Name = name, Age = age, Tags = new List<string> { "a", "b" } };

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

        private static void AssertPerson(Person expected,
            Person actual)
        {
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.Name, actual.Name, StringComparer.Ordinal);
            Assert.AreEqual(expected.Age, actual.Age);
            CollectionAssert.AreEqual(expected.Tags, actual.Tags);
        }

        #endregion

        #region Positive

        [TestMethod]
        public void TestJsonToEntityPropertyHandler()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(CreatePerson());

                // Act
                connection.Insert(model);
                var result = connection.Query<PersonAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                AssertPerson(model.Owner, result.Owner);
            }
        }

        [TestMethod]
        public async Task TestJsonToEntityPropertyHandlerAsync()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(CreatePerson());

                // Act
                await connection.InsertAsync(model).ConfigureAwait(false);
                var result = (await connection.QueryAsync<PersonAttributeModel>(e => e.SessionId == model.SessionId).ConfigureAwait(false)).First();

                // Assert
                AssertPerson(model.Owner, result.Owner);
            }
        }

        [TestMethod]
        public void TestJsonToEntityPropertyHandlerWritesJsonText()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(CreatePerson());

                // Act
                connection.Insert(model);
                var raw = GetRawValue(connection, model.SessionId);

                // Assert
                Assert.AreEqual("{\"Name\":\"John Doe\",\"Age\":30,\"Tags\":[\"a\",\"b\"]}", raw, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestJsonToEntityPropertyHandlerReadsJsonText()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, "{\"Name\":\"Jane\",\"Age\":25,\"Tags\":[\"x\"]}");

                // Act
                var result = connection.Query<PersonAttributeModel>(e => e.SessionId == sessionId).First();

                // Assert
                Assert.AreEqual("Jane", result.Owner.Name, StringComparer.Ordinal);
                Assert.AreEqual(25, result.Owner.Age);
                CollectionAssert.AreEqual(new[] { "x" }, result.Owner.Tags);
            }
        }

        [TestMethod]
        public void TestJsonToEntityPropertyHandlerReadsCaseSensitiveJsonProperties()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                // Default serializer options are case-sensitive, therefore the camel-cased names are not bound
                var sessionId = InsertRawValue(connection, "{\"name\":\"Jane\",\"age\":25}");

                // Act
                var result = connection.Query<PersonAttributeModel>(e => e.SessionId == sessionId).First();

                // Assert
                Assert.IsNotNull(result.Owner);
                Assert.IsNull(result.Owner.Name);
                Assert.AreEqual(0, result.Owner.Age);
            }
        }

        [TestMethod]
        public void TestJsonToEntityPropertyHandlerWithNestedEntities()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var company = new Company
                {
                    Name = "Acme",
                    Owner = CreatePerson("Owner", 50),
                    Employees = new List<Person> { CreatePerson("E1", 20), CreatePerson("E2", 21) },
                    Metrics = new Dictionary<string, int>(StringComparer.Ordinal) { { "Revenue", 100 }, { "Cost", 40 } }
                };
                var model = new CompanyAttributeModel { SessionId = Guid.NewGuid(), Company = company };

                // Act
                connection.Insert(model);
                var result = connection.Query<CompanyAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual("Acme", result.Company.Name, StringComparer.Ordinal);
                AssertPerson(company.Owner, result.Company.Owner);
                Assert.AreEqual(2, result.Company.Employees.Count);
                AssertPerson(company.Employees[1], result.Company.Employees[1]);
                Assert.AreEqual(100, result.Company.Metrics["Revenue"]);
                Assert.AreEqual(40, result.Company.Metrics["Cost"]);
            }
        }

        [TestMethod]
        public void TestJsonToEntityPropertyHandlerWithNullMembers()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(new Person { Name = null, Age = 0, Tags = null });

                // Act
                connection.Insert(model);
                var raw = GetRawValue(connection, model.SessionId);
                var result = connection.Query<PersonAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual("{\"Name\":null,\"Age\":0,\"Tags\":null}", raw, StringComparer.Ordinal);
                Assert.IsNotNull(result.Owner);
                Assert.IsNull(result.Owner.Name);
                Assert.IsNull(result.Owner.Tags);
            }
        }

        [TestMethod]
        public void TestJsonToEntityPropertyHandlerWithUnicodeAndSpecialCharacters()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(CreatePerson("日本語 \"quoted\" \\ Ñandú 😀 <tag>"));

                // Act
                connection.Insert(model);
                var result = connection.Query<PersonAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual(model.Owner.Name, result.Owner.Name, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestJsonToEntityPropertyHandlerOnInsertAll()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var models = Enumerable.Range(1, 10)
                    .Select(i => CreateModel(CreatePerson($"Person-{i}", i)))
                    .ToList();

                // Act
                connection.InsertAll(models);
                var result = connection.QueryAll<PersonAttributeModel>().ToList();

                // Assert
                Assert.AreEqual(models.Count, result.Count);
                foreach (var model in models)
                {
                    AssertPerson(model.Owner, result.First(e => e.SessionId == model.SessionId).Owner);
                }
            }
        }

        [TestMethod]
        public void TestJsonToEntityPropertyHandlerOnUpdate()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(CreatePerson("Before", 1));
                connection.Insert(model);

                // Act
                model.Owner = CreatePerson("After", 2);
                var affectedRows = connection.Update(model);
                var result = connection.Query<PersonAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual(1, affectedRows);
                AssertPerson(model.Owner, result.Owner);
            }
        }

        [TestMethod]
        public void TestJsonToEntityPropertyHandlerOnExecuteQuery()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(CreatePerson());
                connection.Insert(model);

                // Act
                var result = connection.ExecuteQuery<PersonAttributeModel>(
                    "SELECT [SessionId], [ColumnNVarChar] FROM [dbo].[CompleteTable] WHERE [SessionId] = @SessionId;",
                    new { SessionId = model.SessionId }).First();

                // Assert
                AssertPerson(model.Owner, result.Owner);
            }
        }

        [TestMethod]
        public void TestJsonToEntityPropertyHandlerViaFluentMapper()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                FluentMapper
                    .Entity<PersonFluentModel>()
                    .PropertyHandler(e => e.ColumnNVarChar, new JsonToEntityPropertyHandler<Person>());
                var model = new PersonFluentModel { SessionId = Guid.NewGuid(), ColumnNVarChar = CreatePerson() };

                // Act
                connection.Insert(model);
                var result = connection.Query<PersonFluentModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                AssertPerson(model.ColumnNVarChar, result.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestJsonToEntityPropertyHandlerViaTypeLevelMapper()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                PropertyHandlerMapper.Add<Person, JsonToEntityPropertyHandler<Person>>(true);
                var model = new PersonTypeLevelModel { SessionId = Guid.NewGuid(), ColumnNVarChar = CreatePerson() };

                // Act
                connection.Insert(model);
                var result = connection.Query<PersonTypeLevelModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                AssertPerson(model.ColumnNVarChar, result.ColumnNVarChar);
            }
        }

        #endregion

        #region Negative

        [TestMethod]
        public void TestJsonToEntityPropertyHandlerWithNullEntity()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(null);

                // Act
                connection.Insert(model);
                var isNull = IsRawValueNull(connection, model.SessionId);
                var result = connection.Query<PersonAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.IsTrue(isNull);
                Assert.IsNull(result.Owner);
            }
        }

        [TestMethod]
        public void TestJsonToEntityPropertyHandlerWithNullColumn()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = Guid.NewGuid();
                connection.ExecuteNonQuery(
                    "INSERT INTO [dbo].[CompleteTable] ([SessionId], [ColumnNVarChar]) VALUES (@SessionId, NULL);",
                    new { SessionId = sessionId });

                // Act
                var result = connection.Query<PersonAttributeModel>(e => e.SessionId == sessionId).First();

                // Assert
                Assert.IsNull(result.Owner);
            }
        }

        [TestMethod]
        public void TestJsonToEntityPropertyHandlerWithEmptyStringColumn()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, string.Empty);

                // Act
                var result = connection.Query<PersonAttributeModel>(e => e.SessionId == sessionId).First();

                // Assert
                Assert.IsNull(result.Owner);
            }
        }

        [TestMethod]
        public void TestJsonToEntityPropertyHandlerWithJsonNullLiteralColumn()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, "null");

                // Act
                var result = connection.Query<PersonAttributeModel>(e => e.SessionId == sessionId).First();

                // Assert
                Assert.IsNull(result.Owner);
            }
        }

        [TestMethod]
        public void TestJsonToEntityPropertyHandlerUpdateToNullEntity()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(CreatePerson());
                connection.Insert(model);

                // Act
                model.Owner = null;
                connection.Update(model);
                var isNull = IsRawValueNull(connection, model.SessionId);
                var result = connection.Query<PersonAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.IsTrue(isNull);
                Assert.IsNull(result.Owner);
            }
        }

        [TestMethod]
        public void TestJsonToEntityPropertyHandlerWithMalformedJson()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, "{\"Name\":\"broken\"");

                // Act / Assert
                Assert.Throws<JsonException>(() =>
                    connection.Query<PersonAttributeModel>(e => e.SessionId == sessionId).ToList());
            }
        }

        [TestMethod]
        public void TestJsonToEntityPropertyHandlerWithNonJsonText()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, "not a json");

                // Act / Assert
                Assert.Throws<JsonException>(() =>
                    connection.Query<PersonAttributeModel>(e => e.SessionId == sessionId).ToList());
            }
        }

        [TestMethod]
        public void TestJsonToEntityPropertyHandlerWithMismatchedJsonShape()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                // A JSON array cannot be bound to an entity
                var sessionId = InsertRawValue(connection, "[1,2,3]");

                // Act / Assert
                Assert.Throws<JsonException>(() =>
                    connection.Query<PersonAttributeModel>(e => e.SessionId == sessionId).ToList());
            }
        }

        [TestMethod]
        public void TestJsonToEntityPropertyHandlerWithMismatchedMemberType()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                // 'Age' is an int, but the JSON has a string
                var sessionId = InsertRawValue(connection, "{\"Name\":\"John\",\"Age\":\"thirty\"}");

                // Act / Assert
                Assert.Throws<JsonException>(() =>
                    connection.Query<PersonAttributeModel>(e => e.SessionId == sessionId).ToList());
            }
        }

        #endregion
    }
}
