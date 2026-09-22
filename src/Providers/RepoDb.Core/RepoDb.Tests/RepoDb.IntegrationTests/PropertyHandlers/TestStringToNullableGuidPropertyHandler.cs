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
    /// The integration tests of <see cref="StringToNullableGuidPropertyHandler"/>. The <c>NVARCHAR(MAX)</c> column (text-based UUID) and the
    /// <c>UNIQUEIDENTIFIER</c> column of the <c>[dbo].[CompleteTable]</c> are used to store the <see cref="Nullable{T}"/> of <see cref="Guid"/>.
    /// The type level mapping is not tested as the handler would also be applied to the <see cref="Guid"/> key of the table.
    /// </summary>
    [TestClass]
    public class StringToNullableGuidPropertyHandlerTest
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
        private class NullableGuidTextAttributeModel
        {
            public Guid SessionId { get; set; }

            [Map("ColumnNVarChar"), PropertyHandler(typeof(StringToNullableGuidPropertyHandler))]
            public Guid? Value { get; set; }
        }

        [Map("[dbo].[CompleteTable]")]
        private class NullableGuidNativeAttributeModel
        {
            public Guid SessionId { get; set; }

            [Map("ColumnUniqueIdentifier"), PropertyHandler(typeof(StringToNullableGuidPropertyHandler))]
            public Guid? Value { get; set; }
        }

        /// <summary>
        /// Property level (fluent): the handler is bound to the property via <see cref="FluentMapper"/>.
        /// </summary>
        [Map("[dbo].[CompleteTable]")]
        private class NullableGuidFluentModel
        {
            public Guid SessionId { get; set; }

            public Guid? ColumnNVarChar { get; set; }
        }

        /// <summary>
        /// Negative: the handler supports a <see cref="string"/> or <see cref="Guid"/>, but the column is returned as an <see cref="int"/>.
        /// </summary>
        [Map("[dbo].[CompleteTable]")]
        private class NullableGuidOnIntColumnModel
        {
            public Guid SessionId { get; set; }

            [Map("ColumnInt"), PropertyHandler(typeof(StringToNullableGuidPropertyHandler))]
            public Guid? Value { get; set; }
        }

        #endregion

        #region Helpers

        private static NullableGuidTextAttributeModel CreateModel(Guid? value) =>
            new NullableGuidTextAttributeModel { SessionId = Guid.NewGuid(), Value = value };

        private static string GetRawValue(SqlConnection connection,
            Guid sessionId) =>
            connection.ExecuteScalar(
                "SELECT [ColumnNVarChar] FROM [dbo].[CompleteTable] WHERE [SessionId] = @SessionId;",
                new { SessionId = sessionId }) as string;

        private static bool IsRawValueNull(SqlConnection connection,
            string column,
            Guid sessionId) =>
            connection.ExecuteScalar<int>(
                $"SELECT COUNT(1) FROM [dbo].[CompleteTable] WHERE [SessionId] = @SessionId AND [{column}] IS NULL;",
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
        public void TestStringToNullableGuidPropertyHandler()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(Guid.NewGuid());

                // Act
                connection.Insert(model);
                var result = connection.Query<NullableGuidTextAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual(model.Value, result.Value);
            }
        }

        [TestMethod]
        public async Task TestStringToNullableGuidPropertyHandlerAsync()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(Guid.NewGuid());

                // Act
                await connection.InsertAsync(model).ConfigureAwait(false);
                var result = (await connection.QueryAsync<NullableGuidTextAttributeModel>(e => e.SessionId == model.SessionId).ConfigureAwait(false)).First();

                // Assert
                Assert.AreEqual(model.Value, result.Value);
            }
        }

        [TestMethod]
        public void TestStringToNullableGuidPropertyHandlerWritesTheHyphenatedLowerCaseText()
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
        [DataRow("a1b2c3d4e5f64789abcd0123456789ef")]
        public void TestStringToNullableGuidPropertyHandlerReadsTheSupportedTextFormats(string text)
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, text);

                // Act
                var result = connection.Query<NullableGuidTextAttributeModel>(e => e.SessionId == sessionId).First();

                // Assert
                Assert.AreEqual(Guid.Parse("a1b2c3d4-e5f6-4789-abcd-0123456789ef"), result.Value);
            }
        }

        [TestMethod]
        public void TestStringToNullableGuidPropertyHandlerWithEmptyGuidIsNotNull()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(Guid.Empty);

                // Act
                connection.Insert(model);
                var raw = GetRawValue(connection, model.SessionId);
                var result = connection.Query<NullableGuidTextAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual("00000000-0000-0000-0000-000000000000", raw, StringComparer.Ordinal);
                Assert.IsTrue(result.Value.HasValue);
                Assert.AreEqual(Guid.Empty, result.Value.Value);
            }
        }

        [TestMethod]
        public void TestStringToNullableGuidPropertyHandlerOnInsertAll()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var models = Enumerable.Range(1, 10).Select(_ => CreateModel(Guid.NewGuid())).ToList();

                // Act
                connection.InsertAll(models);
                var result = connection.QueryAll<NullableGuidTextAttributeModel>().ToList();

                // Assert
                Assert.AreEqual(models.Count, result.Count);
                foreach (var model in models)
                {
                    Assert.AreEqual(model.Value, result.First(e => e.SessionId == model.SessionId).Value);
                }
            }
        }

        [TestMethod]
        public void TestStringToNullableGuidPropertyHandlerOnInsertAllWithMixedValues()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var models = new[] { CreateModel(Guid.NewGuid()), CreateModel(null), CreateModel(Guid.Empty) };

                // Act
                connection.InsertAll(models);
                var result = connection.QueryAll<NullableGuidTextAttributeModel>().ToList();

                // Assert
                Assert.AreEqual(3, result.Count);
                Assert.AreEqual(models[0].Value, result.First(e => e.SessionId == models[0].SessionId).Value);
                Assert.IsNull(result.First(e => e.SessionId == models[1].SessionId).Value);
                Assert.AreEqual(Guid.Empty, result.First(e => e.SessionId == models[2].SessionId).Value);
            }
        }

        [TestMethod]
        public void TestStringToNullableGuidPropertyHandlerOnUpdate()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(Guid.NewGuid());
                connection.Insert(model);

                // Act
                model.Value = Guid.NewGuid();
                var affectedRows = connection.Update(model);
                var result = connection.Query<NullableGuidTextAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual(1, affectedRows);
                Assert.AreEqual(model.Value, result.Value);
            }
        }

        [TestMethod]
        public void TestStringToNullableGuidPropertyHandlerOnExecuteQuery()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(Guid.NewGuid());
                connection.Insert(model);

                // Act
                var result = connection.ExecuteQuery<NullableGuidTextAttributeModel>(
                    "SELECT [SessionId], [ColumnNVarChar] FROM [dbo].[CompleteTable] WHERE [SessionId] = @SessionId;",
                    new { SessionId = model.SessionId }).First();

                // Assert
                Assert.AreEqual(model.Value, result.Value);
            }
        }

        [TestMethod]
        public void TestStringToNullableGuidPropertyHandlerViaFluentMapper()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                FluentMapper
                    .Entity<NullableGuidFluentModel>()
                    .PropertyHandler(e => e.ColumnNVarChar, new StringToNullableGuidPropertyHandler());
                var model = new NullableGuidFluentModel { SessionId = Guid.NewGuid(), ColumnNVarChar = Guid.NewGuid() };

                // Act
                connection.Insert(model);
                var result = connection.Query<NullableGuidFluentModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual(model.ColumnNVarChar, result.ColumnNVarChar);
                Assert.AreEqual(model.ColumnNVarChar.Value.ToString("D"), GetRawValue(connection, model.SessionId), StringComparer.Ordinal);
            }
        }

        #endregion

        #region Negative

        [TestMethod]
        public void TestStringToNullableGuidPropertyHandlerWithNullValue()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(null);

                // Act
                connection.Insert(model);
                var isNull = IsRawValueNull(connection, "ColumnNVarChar", model.SessionId);
                var result = connection.Query<NullableGuidTextAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.IsTrue(isNull);
                Assert.IsNull(result.Value);
            }
        }

        [TestMethod]
        public void TestStringToNullableGuidPropertyHandlerWithNullValueOnUniqueIdentifierColumn()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = new NullableGuidNativeAttributeModel { SessionId = Guid.NewGuid(), Value = null };

                // Act
                connection.Insert(model);
                var isNull = IsRawValueNull(connection, "ColumnUniqueIdentifier", model.SessionId);
                var result = connection.Query<NullableGuidNativeAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.IsTrue(isNull);
                Assert.IsNull(result.Value);
            }
        }

        [TestMethod]
        public void TestStringToNullableGuidPropertyHandlerWithNullColumn()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = Guid.NewGuid();
                connection.ExecuteNonQuery(
                    "INSERT INTO [dbo].[CompleteTable] ([SessionId], [ColumnNVarChar]) VALUES (@SessionId, NULL);",
                    new { SessionId = sessionId });

                // Act
                var result = connection.Query<NullableGuidTextAttributeModel>(e => e.SessionId == sessionId).First();

                // Assert
                Assert.IsNull(result.Value);
            }
        }

        [TestMethod]
        public void TestStringToNullableGuidPropertyHandlerWithEmptyStringColumn()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, string.Empty);

                // Act
                var result = connection.Query<NullableGuidTextAttributeModel>(e => e.SessionId == sessionId).First();

                // Assert
                Assert.IsNull(result.Value);
            }
        }

        [TestMethod]
        public void TestStringToNullableGuidPropertyHandlerUpdateToNullValue()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(Guid.NewGuid());
                connection.Insert(model);

                // Act
                model.Value = null;
                connection.Update(model);
                var isNull = IsRawValueNull(connection, "ColumnNVarChar", model.SessionId);
                var result = connection.Query<NullableGuidTextAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.IsTrue(isNull);
                Assert.IsNull(result.Value);
            }
        }

        [TestMethod]
        public void TestStringToNullableGuidPropertyHandlerWithInvalidText()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, "not-a-guid");

                // Act / Assert
                Assert.Throws<FormatException>(() =>
                    connection.Query<NullableGuidTextAttributeModel>(e => e.SessionId == sessionId).ToList());
            }
        }

        [TestMethod]
        public void TestStringToNullableGuidPropertyHandlerWithUnsupportedColumnType()
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
                    connection.Query<NullableGuidOnIntColumnModel>(e => e.SessionId == sessionId).ToList());
            }
        }

        [TestMethod]
        public void TestStringToNullableGuidPropertyHandlerWriteOnUniqueIdentifierColumnIsNotSupported()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                // The handler writes the value as text, but the parameter of a 'UNIQUEIDENTIFIER' column is a 'Guid'
                var model = new NullableGuidNativeAttributeModel { SessionId = Guid.NewGuid(), Value = Guid.NewGuid() };

                // Act / Assert
                Assert.Throws<InvalidCastException>(() => connection.Insert(model));
            }
        }

        #endregion
    }
}
