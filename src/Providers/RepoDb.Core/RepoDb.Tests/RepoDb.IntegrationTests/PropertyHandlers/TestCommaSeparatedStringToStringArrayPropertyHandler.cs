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
using System.Threading.Tasks;

namespace RepoDb.IntegrationTests.PropertyHandlers
{
    /// <summary>
    /// The integration tests of <see cref="CommaSeparatedStringToStringArrayPropertyHandler"/>. The <c>NVARCHAR(MAX)</c> column
    /// of the <c>[dbo].[CompleteTable]</c> is used to store the <see cref="T:string[]"/> as a comma-separated value.
    /// </summary>
    [TestClass]
    public class CommaSeparatedStringToStringArrayPropertyHandlerTest
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
            PropertyHandlerMapper.Remove(typeof(string[]));
            PropertyHandlerCache.Flush();
            Database.Cleanup();
        }

        #region Classes

        /// <summary>
        /// Property level (attribute): the handler is bound to the property via <see cref="PropertyHandlerAttribute"/>.
        /// </summary>
        [Map("[dbo].[CompleteTable]")]
        private class StringArrayAttributeModel
        {
            public Guid SessionId { get; set; }

            [Map("ColumnNVarChar"), PropertyHandler(typeof(CommaSeparatedStringToStringArrayPropertyHandler))]
            public string[] Tags { get; set; }
        }

        /// <summary>
        /// Property level (fluent): the handler is bound to the property via <see cref="FluentMapper"/>.
        /// </summary>
        [Map("[dbo].[CompleteTable]")]
        private class StringArrayFluentModel
        {
            public Guid SessionId { get; set; }

            public string[] ColumnNVarChar { get; set; }
        }

        /// <summary>
        /// Type level: the handler is bound to <see cref="T:string[]"/> via <see cref="PropertyHandlerMapper"/>.
        /// </summary>
        [Map("[dbo].[CompleteTable]")]
        private class StringArrayTypeLevelModel
        {
            public Guid SessionId { get; set; }

            public string[] ColumnNVarChar { get; set; }
        }

        #endregion

        #region Helpers

        private static StringArrayAttributeModel CreateModel(string[] tags) =>
            new StringArrayAttributeModel { SessionId = Guid.NewGuid(), Tags = tags };

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
        public void TestCommaSeparatedStringToStringArrayPropertyHandler()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(new[] { "red", "green", "blue" });

                // Act
                connection.Insert(model);
                var result = connection.Query<StringArrayAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.IsNotNull(result.Tags);
                CollectionAssert.AreEqual(model.Tags, result.Tags);
            }
        }

        [TestMethod]
        public async Task TestCommaSeparatedStringToStringArrayPropertyHandlerAsync()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(new[] { "red", "green", "blue" });

                // Act
                await connection.InsertAsync(model).ConfigureAwait(false);
                var result = (await connection.QueryAsync<StringArrayAttributeModel>(e => e.SessionId == model.SessionId).ConfigureAwait(false)).First();

                // Assert
                Assert.IsNotNull(result.Tags);
                CollectionAssert.AreEqual(model.Tags, result.Tags);
            }
        }

        [TestMethod]
        public void TestCommaSeparatedStringToStringArrayPropertyHandlerWritesCommaSeparatedValue()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(new[] { "red", "green", "blue" });

                // Act
                connection.Insert(model);
                var raw = GetRawValue(connection, model.SessionId);

                // Assert
                Assert.AreEqual("red,green,blue", raw, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestCommaSeparatedStringToStringArrayPropertyHandlerReadsCommaSeparatedValue()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, "x,y,z");

                // Act
                var result = connection.Query<StringArrayAttributeModel>(e => e.SessionId == sessionId).First();

                // Assert
                CollectionAssert.AreEqual(new[] { "x", "y", "z" }, result.Tags);
            }
        }

        [TestMethod]
        public void TestCommaSeparatedStringToStringArrayPropertyHandlerWithSingleMember()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(new[] { "only" });

                // Act
                connection.Insert(model);
                var raw = GetRawValue(connection, model.SessionId);
                var result = connection.Query<StringArrayAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual("only", raw, StringComparer.Ordinal);
                CollectionAssert.AreEqual(new[] { "only" }, result.Tags);
            }
        }

        [TestMethod]
        public void TestCommaSeparatedStringToStringArrayPropertyHandlerWithEmptyArray()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(Array.Empty<string>());

                // Act
                connection.Insert(model);
                var raw = GetRawValue(connection, model.SessionId);
                var result = connection.Query<StringArrayAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual(string.Empty, raw, StringComparer.Ordinal);
                Assert.IsNotNull(result.Tags);
                Assert.AreEqual(0, result.Tags.Length);
            }
        }

        [TestMethod]
        public void TestCommaSeparatedStringToStringArrayPropertyHandlerPreservesSpacesAndCase()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(new[] { " Leading", "Trailing ", "In Between", "UPPER" });

                // Act
                connection.Insert(model);
                var result = connection.Query<StringArrayAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                CollectionAssert.AreEqual(model.Tags, result.Tags);
            }
        }

        [TestMethod]
        public void TestCommaSeparatedStringToStringArrayPropertyHandlerWithEmptyMembers()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(new[] { "a", string.Empty, "b", string.Empty });

                // Act
                connection.Insert(model);
                var raw = GetRawValue(connection, model.SessionId);
                var result = connection.Query<StringArrayAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual("a,,b,", raw, StringComparer.Ordinal);
                CollectionAssert.AreEqual(model.Tags, result.Tags);
            }
        }

        [TestMethod]
        public void TestCommaSeparatedStringToStringArrayPropertyHandlerWithUnicodeMembers()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(new[] { "日本語", "Ñandú", "😀" });

                // Act
                connection.Insert(model);
                var result = connection.Query<StringArrayAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                CollectionAssert.AreEqual(model.Tags, result.Tags);
            }
        }

        [TestMethod]
        public void TestCommaSeparatedStringToStringArrayPropertyHandlerOnInsertAll()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var models = Enumerable.Range(1, 10)
                    .Select(i => CreateModel(Enumerable.Range(0, i).Select(n => $"Item-{i}-{n}").ToArray()))
                    .ToList();

                // Act
                connection.InsertAll(models);
                var result = connection.QueryAll<StringArrayAttributeModel>().ToList();

                // Assert
                Assert.AreEqual(models.Count, result.Count);
                foreach (var model in models)
                {
                    CollectionAssert.AreEqual(model.Tags, result.First(e => e.SessionId == model.SessionId).Tags);
                }
            }
        }

        [TestMethod]
        public void TestCommaSeparatedStringToStringArrayPropertyHandlerOnUpdate()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(new[] { "a", "b" });
                connection.Insert(model);

                // Act
                model.Tags = new[] { "c", "d", "e" };
                var affectedRows = connection.Update(model);
                var result = connection.Query<StringArrayAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual(1, affectedRows);
                CollectionAssert.AreEqual(new[] { "c", "d", "e" }, result.Tags);
            }
        }

        [TestMethod]
        public void TestCommaSeparatedStringToStringArrayPropertyHandlerOnExecuteQuery()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(new[] { "p", "q" });
                connection.Insert(model);

                // Act
                var result = connection.ExecuteQuery<StringArrayAttributeModel>(
                    "SELECT [SessionId], [ColumnNVarChar] FROM [dbo].[CompleteTable] WHERE [SessionId] = @SessionId;",
                    new { SessionId = model.SessionId }).First();

                // Assert
                CollectionAssert.AreEqual(model.Tags, result.Tags);
            }
        }

        [TestMethod]
        public void TestCommaSeparatedStringToStringArrayPropertyHandlerViaFluentMapper()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                FluentMapper
                    .Entity<StringArrayFluentModel>()
                    .PropertyHandler(e => e.ColumnNVarChar, new CommaSeparatedStringToStringArrayPropertyHandler());
                var model = new StringArrayFluentModel { SessionId = Guid.NewGuid(), ColumnNVarChar = new[] { "a", "b" } };

                // Act
                connection.Insert(model);
                var result = connection.Query<StringArrayFluentModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                CollectionAssert.AreEqual(model.ColumnNVarChar, result.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestCommaSeparatedStringToStringArrayPropertyHandlerViaTypeLevelMapper()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                PropertyHandlerMapper.Add<string[], CommaSeparatedStringToStringArrayPropertyHandler>(true);
                var model = new StringArrayTypeLevelModel { SessionId = Guid.NewGuid(), ColumnNVarChar = new[] { "a", "b" } };

                // Act
                connection.Insert(model);
                var result = connection.Query<StringArrayTypeLevelModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                CollectionAssert.AreEqual(model.ColumnNVarChar, result.ColumnNVarChar);
            }
        }

        #endregion

        #region Negative

        [TestMethod]
        public void TestCommaSeparatedStringToStringArrayPropertyHandlerWithNullArray()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(null);

                // Act
                connection.Insert(model);
                var isNull = IsRawValueNull(connection, model.SessionId);
                var result = connection.Query<StringArrayAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.IsTrue(isNull);
                Assert.IsNull(result.Tags);
            }
        }

        [TestMethod]
        public void TestCommaSeparatedStringToStringArrayPropertyHandlerWithNullColumn()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = Guid.NewGuid();
                connection.ExecuteNonQuery(
                    "INSERT INTO [dbo].[CompleteTable] ([SessionId], [ColumnNVarChar]) VALUES (@SessionId, NULL);",
                    new { SessionId = sessionId });

                // Act
                var result = connection.Query<StringArrayAttributeModel>(e => e.SessionId == sessionId).First();

                // Assert
                Assert.IsNull(result.Tags);
            }
        }

        [TestMethod]
        public void TestCommaSeparatedStringToStringArrayPropertyHandlerUpdateToNullArray()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(new[] { "a", "b" });
                connection.Insert(model);

                // Act
                model.Tags = null;
                connection.Update(model);
                var isNull = IsRawValueNull(connection, model.SessionId);
                var result = connection.Query<StringArrayAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.IsTrue(isNull);
                Assert.IsNull(result.Tags);
            }
        }

        [TestMethod]
        public void TestCommaSeparatedStringToStringArrayPropertyHandlerWithSingleEmptyMemberReadsBackAsEmptyArray()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                // An array of a single empty member and an empty array are both stored as the empty string
                var model = CreateModel(new[] { string.Empty });

                // Act
                connection.Insert(model);
                var result = connection.Query<StringArrayAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual(0, result.Tags.Length);
            }
        }

        [TestMethod]
        public void TestCommaSeparatedStringToStringArrayPropertyHandlerWithMemberContainingComma()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(new[] { "valid", "not,valid" });

                // Act / Assert
                Assert.Throws<ArgumentException>(() => connection.Insert(model));
            }
        }

        [TestMethod]
        public void TestCommaSeparatedStringToStringArrayPropertyHandlerWithMemberContainingCommaDoesNotWriteAnyRow()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(new[] { "valid", "not,valid" });

                // Act
                Assert.Throws<ArgumentException>(() => connection.Insert(model));
                var count = connection.CountAll<StringArrayAttributeModel>();

                // Assert
                Assert.AreEqual(0, count);
            }
        }

        #endregion
    }
}
