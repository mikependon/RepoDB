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
    /// The integration tests of <see cref="ByteArrayToFloatArrayPropertyHandler"/>. The <c>VARBINARY(MAX)</c> column (returned by the driver
    /// as <see cref="T:byte[]"/>) of the <c>[dbo].[CompleteTable]</c> is used to store the <see cref="T:float[]"/> as little-endian 4-byte values.
    /// </summary>
    [TestClass]
    public class ByteArrayToFloatArrayPropertyHandlerTest
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
            PropertyHandlerMapper.Remove(typeof(float[]));
            PropertyHandlerCache.Flush();
            Database.Cleanup();
        }

        #region Classes

        /// <summary>
        /// Property level (attribute): the handler is bound to the property via <see cref="PropertyHandlerAttribute"/>.
        /// </summary>
        [Map("[dbo].[CompleteTable]")]
        private class FloatArrayAttributeModel
        {
            public Guid SessionId { get; set; }

            [Map("ColumnVarBinary"), PropertyHandler(typeof(ByteArrayToFloatArrayPropertyHandler))]
            public float[] Vector { get; set; }
        }

        /// <summary>
        /// Property level (fluent): the handler is bound to the property via <see cref="FluentMapper"/>.
        /// </summary>
        [Map("[dbo].[CompleteTable]")]
        private class FloatArrayFluentModel
        {
            public Guid SessionId { get; set; }

            public float[] ColumnVarBinary { get; set; }
        }

        /// <summary>
        /// Type level: the handler is bound to <see cref="T:float[]"/> via <see cref="PropertyHandlerMapper"/>.
        /// </summary>
        [Map("[dbo].[CompleteTable]")]
        private class FloatArrayTypeLevelModel
        {
            public Guid SessionId { get; set; }

            public float[] ColumnVarBinary { get; set; }
        }

        /// <summary>
        /// Negative: the handler expects a <see cref="T:byte[]"/>, but the column is returned as a <see cref="string"/>.
        /// </summary>
        [Map("[dbo].[CompleteTable]")]
        private class FloatArrayOnStringColumnModel
        {
            public Guid SessionId { get; set; }

            [Map("ColumnNVarChar"), PropertyHandler(typeof(ByteArrayToFloatArrayPropertyHandler))]
            public float[] Vector { get; set; }
        }

        #endregion

        #region Helpers

        private static FloatArrayAttributeModel CreateModel(float[] vector) =>
            new FloatArrayAttributeModel { SessionId = Guid.NewGuid(), Vector = vector };

        private static byte[] GetRawValue(SqlConnection connection,
            Guid sessionId) =>
            connection.ExecuteScalar(
                "SELECT [ColumnVarBinary] FROM [dbo].[CompleteTable] WHERE [SessionId] = @SessionId;",
                new { SessionId = sessionId }) as byte[];

        private static bool IsRawValueNull(SqlConnection connection,
            Guid sessionId) =>
            connection.ExecuteScalar<int>(
                "SELECT COUNT(1) FROM [dbo].[CompleteTable] WHERE [SessionId] = @SessionId AND [ColumnVarBinary] IS NULL;",
                new { SessionId = sessionId }) == 1;

        private static Guid InsertRawValue(SqlConnection connection,
            string hexLiteral)
        {
            var sessionId = Guid.NewGuid();
            connection.ExecuteNonQuery(
                $"INSERT INTO [dbo].[CompleteTable] ([SessionId], [ColumnVarBinary]) VALUES (@SessionId, {hexLiteral});",
                new { SessionId = sessionId });
            return sessionId;
        }

        #endregion

        #region Positive

        [TestMethod]
        public void TestByteArrayToFloatArrayPropertyHandler()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(new[] { 1.5f, -2.25f, 3.14159f, 0f });

                // Act
                connection.Insert(model);
                var result = connection.Query<FloatArrayAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.IsNotNull(result.Vector);
                CollectionAssert.AreEqual(model.Vector, result.Vector);
            }
        }

        [TestMethod]
        public async Task TestByteArrayToFloatArrayPropertyHandlerAsync()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(new[] { 1.5f, -2.25f, 3.14159f, 0f });

                // Act
                await connection.InsertAsync(model).ConfigureAwait(false);
                var result = (await connection.QueryAsync<FloatArrayAttributeModel>(e => e.SessionId == model.SessionId).ConfigureAwait(false)).First();

                // Assert
                Assert.IsNotNull(result.Vector);
                CollectionAssert.AreEqual(model.Vector, result.Vector);
            }
        }

        [TestMethod]
        public void TestByteArrayToFloatArrayPropertyHandlerWritesLittleEndianFourBytesPerElement()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(new[] { 1.5f, -2.0f });

                // Act
                connection.Insert(model);
                var raw = GetRawValue(connection, model.SessionId);

                // Assert
                Assert.IsNotNull(raw);
                CollectionAssert.AreEqual(new byte[] { 0x00, 0x00, 0xC0, 0x3F, 0x00, 0x00, 0x00, 0xC0 }, raw);
            }
        }

        [TestMethod]
        public void TestByteArrayToFloatArrayPropertyHandlerReadsLittleEndianFourBytesPerElement()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, "0x0000C03F000000C0");

                // Act
                var result = connection.Query<FloatArrayAttributeModel>(e => e.SessionId == sessionId).First();

                // Assert
                CollectionAssert.AreEqual(new[] { 1.5f, -2.0f }, result.Vector);
            }
        }

        [TestMethod]
        public void TestByteArrayToFloatArrayPropertyHandlerWithEmptyArray()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(Array.Empty<float>());

                // Act
                connection.Insert(model);
                var raw = GetRawValue(connection, model.SessionId);
                var result = connection.Query<FloatArrayAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.IsNotNull(raw);
                Assert.AreEqual(0, raw.Length);
                Assert.IsNotNull(result.Vector);
                Assert.AreEqual(0, result.Vector.Length);
            }
        }

        [TestMethod]
        public void TestByteArrayToFloatArrayPropertyHandlerWithSingleElement()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(new[] { 42.5f });

                // Act
                connection.Insert(model);
                var result = connection.Query<FloatArrayAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                CollectionAssert.AreEqual(new[] { 42.5f }, result.Vector);
            }
        }

        [TestMethod]
        public void TestByteArrayToFloatArrayPropertyHandlerWithSpecialValues()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(new[]
                {
                    float.NaN, float.PositiveInfinity, float.NegativeInfinity,
                    float.MaxValue, float.MinValue, float.Epsilon, -0f
                });

                // Act
                connection.Insert(model);
                var result = connection.Query<FloatArrayAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual(model.Vector.Length, result.Vector.Length);
                Assert.IsTrue(float.IsNaN(result.Vector[0]));
                Assert.AreEqual(float.PositiveInfinity, result.Vector[1]);
                Assert.AreEqual(float.NegativeInfinity, result.Vector[2]);
                Assert.AreEqual(float.MaxValue, result.Vector[3]);
                Assert.AreEqual(float.MinValue, result.Vector[4]);
                Assert.AreEqual(float.Epsilon, result.Vector[5]);
                Assert.IsTrue(float.IsNegative(result.Vector[6]));
            }
        }

        [TestMethod]
        public void TestByteArrayToFloatArrayPropertyHandlerWithEmbeddingSizedVector()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var random = new Random(1);
                var model = CreateModel(Enumerable.Range(0, 1536).Select(_ => random.NextSingle()).ToArray());

                // Act
                connection.Insert(model);
                var raw = GetRawValue(connection, model.SessionId);
                var result = connection.Query<FloatArrayAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual(1536 * sizeof(float), raw.Length);
                CollectionAssert.AreEqual(model.Vector, result.Vector);
            }
        }

        [TestMethod]
        public void TestByteArrayToFloatArrayPropertyHandlerOnInsertAll()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var models = Enumerable.Range(1, 10)
                    .Select(i => CreateModel(Enumerable.Range(0, i).Select(n => (float)(i * 10 + n) / 4).ToArray()))
                    .ToList();

                // Act
                connection.InsertAll(models);
                var result = connection.QueryAll<FloatArrayAttributeModel>().ToList();

                // Assert
                Assert.AreEqual(models.Count, result.Count);
                foreach (var model in models)
                {
                    CollectionAssert.AreEqual(model.Vector, result.First(e => e.SessionId == model.SessionId).Vector);
                }
            }
        }

        [TestMethod]
        public void TestByteArrayToFloatArrayPropertyHandlerOnInsertAllWithMixedValues()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var models = new[]
                {
                    CreateModel(new[] { 1f, 2f, 3f }),
                    CreateModel(Array.Empty<float>()),
                    CreateModel(null)
                };

                // Act
                connection.InsertAll(models);
                var result = connection.QueryAll<FloatArrayAttributeModel>().ToList();

                // Assert
                Assert.AreEqual(3, result.Count);
                CollectionAssert.AreEqual(models[0].Vector, result.First(e => e.SessionId == models[0].SessionId).Vector);
                Assert.AreEqual(0, result.First(e => e.SessionId == models[1].SessionId).Vector.Length);
                Assert.IsNull(result.First(e => e.SessionId == models[2].SessionId).Vector);
            }
        }

        [TestMethod]
        public void TestByteArrayToFloatArrayPropertyHandlerOnUpdate()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(new[] { 1f, 2f, 3f });
                connection.Insert(model);

                // Act
                model.Vector = new[] { 9.5f, 8.5f };
                var affectedRows = connection.Update(model);
                var result = connection.Query<FloatArrayAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual(1, affectedRows);
                CollectionAssert.AreEqual(new[] { 9.5f, 8.5f }, result.Vector);
            }
        }

        [TestMethod]
        public void TestByteArrayToFloatArrayPropertyHandlerOnExecuteQuery()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(new[] { 0.25f, 0.5f, 0.75f });
                connection.Insert(model);

                // Act
                var result = connection.ExecuteQuery<FloatArrayAttributeModel>(
                    "SELECT [SessionId], [ColumnVarBinary] FROM [dbo].[CompleteTable] WHERE [SessionId] = @SessionId;",
                    new { SessionId = model.SessionId }).First();

                // Assert
                CollectionAssert.AreEqual(model.Vector, result.Vector);
            }
        }

        [TestMethod]
        public void TestByteArrayToFloatArrayPropertyHandlerViaFluentMapper()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                FluentMapper
                    .Entity<FloatArrayFluentModel>()
                    .PropertyHandler(e => e.ColumnVarBinary, new ByteArrayToFloatArrayPropertyHandler());
                var model = new FloatArrayFluentModel { SessionId = Guid.NewGuid(), ColumnVarBinary = new[] { 1.25f, 2.5f } };

                // Act
                connection.Insert(model);
                var result = connection.Query<FloatArrayFluentModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                CollectionAssert.AreEqual(model.ColumnVarBinary, result.ColumnVarBinary);
            }
        }

        [TestMethod]
        public void TestByteArrayToFloatArrayPropertyHandlerViaTypeLevelMapper()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                PropertyHandlerMapper.Add<float[], ByteArrayToFloatArrayPropertyHandler>(true);
                var model = new FloatArrayTypeLevelModel { SessionId = Guid.NewGuid(), ColumnVarBinary = new[] { 1.25f, 2.5f } };

                // Act
                connection.Insert(model);
                var result = connection.Query<FloatArrayTypeLevelModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                CollectionAssert.AreEqual(model.ColumnVarBinary, result.ColumnVarBinary);
            }
        }

        #endregion

        #region Negative

        [TestMethod]
        public void TestByteArrayToFloatArrayPropertyHandlerWithNullArray()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(null);

                // Act
                connection.Insert(model);
                var isNull = IsRawValueNull(connection, model.SessionId);
                var result = connection.Query<FloatArrayAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.IsTrue(isNull);
                Assert.IsNull(result.Vector);
            }
        }

        [TestMethod]
        public void TestByteArrayToFloatArrayPropertyHandlerWithNullColumn()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, "NULL");

                // Act
                var result = connection.Query<FloatArrayAttributeModel>(e => e.SessionId == sessionId).First();

                // Assert
                Assert.IsNull(result.Vector);
            }
        }

        [TestMethod]
        public void TestByteArrayToFloatArrayPropertyHandlerUpdateToNullArray()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(new[] { 1f, 2f });
                connection.Insert(model);

                // Act
                model.Vector = null;
                connection.Update(model);
                var isNull = IsRawValueNull(connection, model.SessionId);
                var result = connection.Query<FloatArrayAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.IsTrue(isNull);
                Assert.IsNull(result.Vector);
            }
        }

        [TestMethod]
        public void TestByteArrayToFloatArrayPropertyHandlerWithLengthNotMultipleOfFour()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, "0x010203");

                // Act / Assert
                Assert.Throws<ArgumentException>(() =>
                    connection.Query<FloatArrayAttributeModel>(e => e.SessionId == sessionId).ToList());
            }
        }

        [TestMethod]
        public void TestByteArrayToFloatArrayPropertyHandlerWithNonBinaryColumn()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = Guid.NewGuid();
                connection.ExecuteNonQuery(
                    "INSERT INTO [dbo].[CompleteTable] ([SessionId], [ColumnNVarChar]) VALUES (@SessionId, N'Not a vector');",
                    new { SessionId = sessionId });

                // Act / Assert
                // The driver returns 'string', which cannot be handled by the handler (byte[])
                Assert.Throws<InvalidOperationException>(() =>
                    connection.Query<FloatArrayOnStringColumnModel>(e => e.SessionId == sessionId).ToList());
            }
        }

        #endregion
    }
}
