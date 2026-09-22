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
    /// The integration tests of <see cref="StringToGuidPropertyHandler"/>. The <c>NVARCHAR(MAX)</c> column (text-based UUID) and the
    /// <c>UNIQUEIDENTIFIER</c> column of the <c>[dbo].[CompleteTable]</c> are used to store the <see cref="Guid"/>.
    /// The type level mapping is not tested as the handler would also be applied to the <see cref="Guid"/> key of the table.
    /// </summary>
    [TestClass]
    public class StringToGuidPropertyHandlerTest
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
            PropertyHandlerCache.Flush();
            Database.Cleanup();
        }

        #region Classes

        /// <summary>
        /// Property level (attribute): the handler is bound to the property via <see cref="PropertyHandlerAttribute"/>.
        /// </summary>
        [Map("[dbo].[CompleteTable]")]
        private class GuidTextAttributeModel
        {
            public Guid SessionId { get; set; }

            [Map("ColumnNVarChar"), PropertyHandler(typeof(StringToGuidPropertyHandler))]
            public Guid Value { get; set; }
        }

        [Map("[dbo].[CompleteTable]")]
        private class GuidNativeAttributeModel
        {
            public Guid SessionId { get; set; }

            [Map("ColumnUniqueIdentifier"), PropertyHandler(typeof(StringToGuidPropertyHandler))]
            public Guid Value { get; set; }
        }

        /// <summary>
        /// Property level (fluent): the handler is bound to the property via <see cref="FluentMapper"/>.
        /// </summary>
        [Map("[dbo].[CompleteTable]")]
        private class GuidFluentModel
        {
            public Guid SessionId { get; set; }

            public Guid ColumnNVarChar { get; set; }
        }

        /// <summary>
        /// Negative: the handler supports a <see cref="string"/> or <see cref="Guid"/>, but the column is returned as an <see cref="int"/>.
        /// </summary>
        [Map("[dbo].[CompleteTable]")]
        private class GuidOnIntColumnModel
        {
            public Guid SessionId { get; set; }

            [Map("ColumnInt"), PropertyHandler(typeof(StringToGuidPropertyHandler))]
            public Guid Value { get; set; }
        }

        #endregion

        #region Helpers

        private static GuidTextAttributeModel CreateModel(Guid value) =>
            new GuidTextAttributeModel { SessionId = Guid.NewGuid(), Value = value };

        private static string GetRawValue(SqlConnection connection,
            Guid sessionId) =>
            connection.ExecuteScalar(
                "SELECT [ColumnNVarChar] FROM [dbo].[CompleteTable] WHERE [SessionId] = @SessionId;",
                new { SessionId = sessionId }) as string;

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
        public void TestStringToGuidPropertyHandler()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(Guid.NewGuid());

                // Act
                connection.Insert(model);
                var result = connection.Query<GuidTextAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual(model.Value, result.Value);
            }
        }

        [TestMethod]
        public async Task TestStringToGuidPropertyHandlerAsync()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(Guid.NewGuid());

                // Act
                await connection.InsertAsync(model).ConfigureAwait(false);
                var result = (await connection.QueryAsync<GuidTextAttributeModel>(e => e.SessionId == model.SessionId).ConfigureAwait(false)).First();

                // Assert
                Assert.AreEqual(model.Value, result.Value);
            }
        }

        [TestMethod]
        public void TestStringToGuidPropertyHandlerWritesTheHyphenatedLowerCaseText()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(Guid.Parse("A1B2C3D4-E5F6-4789-ABCD-0123456789EF"));

                // Act
                connection.Insert(model);
                var raw = GetRawValue(connection, model.SessionId);

                // Assert
                Assert.AreEqual("a1b2c3d4-e5f6-4789-abcd-0123456789ef", raw, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        [DataRow("a1b2c3d4-e5f6-4789-abcd-0123456789ef")]
        [DataRow("A1B2C3D4-E5F6-4789-ABCD-0123456789EF")]
        [DataRow("{a1b2c3d4-e5f6-4789-abcd-0123456789ef}")]
        [DataRow("(a1b2c3d4-e5f6-4789-abcd-0123456789ef)")]
        [DataRow("a1b2c3d4e5f64789abcd0123456789ef")]
        public void TestStringToGuidPropertyHandlerReadsTheSupportedTextFormats(string text)
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, text);

                // Act
                var result = connection.Query<GuidTextAttributeModel>(e => e.SessionId == sessionId).First();

                // Assert
                Assert.AreEqual(Guid.Parse("a1b2c3d4-e5f6-4789-abcd-0123456789ef"), result.Value);
            }
        }

        [TestMethod]
        public void TestStringToGuidPropertyHandlerWithEmptyGuid()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(Guid.Empty);

                // Act
                connection.Insert(model);
                var raw = GetRawValue(connection, model.SessionId);
                var result = connection.Query<GuidTextAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual("00000000-0000-0000-0000-000000000000", raw, StringComparer.Ordinal);
                Assert.AreEqual(Guid.Empty, result.Value);
            }
        }

        [TestMethod]
        public void TestStringToGuidPropertyHandlerReadsTheGuidReturnedByTheDriver()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = Guid.NewGuid();
                var value = Guid.NewGuid();
                connection.ExecuteNonQuery(
                    "INSERT INTO [dbo].[CompleteTable] ([SessionId], [ColumnUniqueIdentifier]) VALUES (@SessionId, @Value);",
                    new { SessionId = sessionId, Value = value });

                // Act
                var result = connection.Query<GuidNativeAttributeModel>(e => e.SessionId == sessionId).First();

                // Assert
                Assert.AreEqual(value, result.Value);
            }
        }

        [TestMethod]
        public void TestStringToGuidPropertyHandlerOnInsertAll()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var models = Enumerable.Range(1, 10).Select(_ => CreateModel(Guid.NewGuid())).ToList();

                // Act
                connection.InsertAll(models);
                var result = connection.QueryAll<GuidTextAttributeModel>().ToList();

                // Assert
                Assert.AreEqual(models.Count, result.Count);
                foreach (var model in models)
                {
                    Assert.AreEqual(model.Value, result.First(e => e.SessionId == model.SessionId).Value);
                }
            }
        }

        [TestMethod]
        public void TestStringToGuidPropertyHandlerOnUpdate()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(Guid.NewGuid());
                connection.Insert(model);

                // Act
                model.Value = Guid.NewGuid();
                var affectedRows = connection.Update(model);
                var result = connection.Query<GuidTextAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual(1, affectedRows);
                Assert.AreEqual(model.Value, result.Value);
            }
        }

        [TestMethod]
        public void TestStringToGuidPropertyHandlerOnExecuteQuery()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(Guid.NewGuid());
                connection.Insert(model);

                // Act
                var result = connection.ExecuteQuery<GuidTextAttributeModel>(
                    "SELECT [SessionId], [ColumnNVarChar] FROM [dbo].[CompleteTable] WHERE [SessionId] = @SessionId;",
                    new { SessionId = model.SessionId }).First();

                // Assert
                Assert.AreEqual(model.Value, result.Value);
            }
        }

        [TestMethod]
        public void TestStringToGuidPropertyHandlerOnWhereCondition()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var models = Enumerable.Range(1, 3).Select(_ => CreateModel(Guid.NewGuid())).ToList();
                connection.InsertAll(models);
                var target = models[1];

                // Act
                var result = connection.Query<GuidTextAttributeModel>(e => e.Value == target.Value).ToList();

                // Assert
                Assert.AreEqual(1, result.Count);
                Assert.AreEqual(target.SessionId, result[0].SessionId);
            }
        }

        [TestMethod]
        public void TestStringToGuidPropertyHandlerViaFluentMapper()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                FluentMapper
                    .Entity<GuidFluentModel>()
                    .PropertyHandler(e => e.ColumnNVarChar, new StringToGuidPropertyHandler());
                var model = new GuidFluentModel { SessionId = Guid.NewGuid(), ColumnNVarChar = Guid.NewGuid() };

                // Act
                connection.Insert(model);
                var result = connection.Query<GuidFluentModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual(model.ColumnNVarChar, result.ColumnNVarChar);
                Assert.AreEqual(model.ColumnNVarChar.ToString("D"), GetRawValue(connection, model.SessionId), StringComparer.Ordinal);
            }
        }

        #endregion

        #region Negative

        [TestMethod]
        public void TestStringToGuidPropertyHandlerWithNullColumn()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = Guid.NewGuid();
                connection.ExecuteNonQuery(
                    "INSERT INTO [dbo].[CompleteTable] ([SessionId], [ColumnNVarChar]) VALUES (@SessionId, NULL);",
                    new { SessionId = sessionId });

                // Act
                var result = connection.Query<GuidTextAttributeModel>(e => e.SessionId == sessionId).First();

                // Assert
                Assert.AreEqual(Guid.Empty, result.Value);
            }
        }

        [TestMethod]
        public void TestStringToGuidPropertyHandlerWithNullUniqueIdentifierColumn()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = Guid.NewGuid();
                connection.ExecuteNonQuery(
                    "INSERT INTO [dbo].[CompleteTable] ([SessionId], [ColumnUniqueIdentifier]) VALUES (@SessionId, NULL);",
                    new { SessionId = sessionId });

                // Act
                var result = connection.Query<GuidNativeAttributeModel>(e => e.SessionId == sessionId).First();

                // Assert
                Assert.AreEqual(Guid.Empty, result.Value);
            }
        }

        [TestMethod]
        public void TestStringToGuidPropertyHandlerWithEmptyStringColumn()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, string.Empty);

                // Act
                var result = connection.Query<GuidTextAttributeModel>(e => e.SessionId == sessionId).First();

                // Assert
                Assert.AreEqual(Guid.Empty, result.Value);
            }
        }

        [TestMethod]
        public void TestStringToGuidPropertyHandlerWithInvalidText()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, "not-a-guid");

                // Act / Assert
                Assert.Throws<FormatException>(() =>
                    connection.Query<GuidTextAttributeModel>(e => e.SessionId == sessionId).ToList());
            }
        }

        [TestMethod]
        public void TestStringToGuidPropertyHandlerWithTruncatedText()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, "a1b2c3d4-e5f6-4789-abcd");

                // Act / Assert
                Assert.Throws<FormatException>(() =>
                    connection.Query<GuidTextAttributeModel>(e => e.SessionId == sessionId).ToList());
            }
        }

        [TestMethod]
        public void TestStringToGuidPropertyHandlerWithUnsupportedColumnType()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = Guid.NewGuid();
                connection.ExecuteNonQuery(
                    "INSERT INTO [dbo].[CompleteTable] ([SessionId], [ColumnInt]) VALUES (@SessionId, 1);",
                    new { SessionId = sessionId });

                // Act / Assert
                // The driver returns 'int', which is not a supported UUID value
                Assert.Throws<ArgumentException>(() =>
                    connection.Query<GuidOnIntColumnModel>(e => e.SessionId == sessionId).ToList());
            }
        }

        [TestMethod]
        public void TestStringToGuidPropertyHandlerWriteOnUniqueIdentifierColumnIsNotSupported()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                // The handler writes the value as text, but the parameter of a 'UNIQUEIDENTIFIER' column is a 'Guid'
                var model = new GuidNativeAttributeModel { SessionId = Guid.NewGuid(), Value = Guid.NewGuid() };

                // Act / Assert
                Assert.Throws<InvalidCastException>(() => connection.Insert(model));
            }
        }

        #endregion
    }
}
