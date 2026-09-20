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
using System.Threading.Tasks;

namespace RepoDb.IntegrationTests.PropertyHandlers
{
    /// <summary>
    /// The integration tests of <see cref="ArrayToListPropertyHandler{T}"/>. SQL Server has no native array column, therefore the
    /// <c>VARBINARY(MAX)</c> column (returned by the driver as <see cref="T:byte[]"/>) of the <c>[dbo].[CompleteTable]</c> is used
    /// to exercise the handler as a <see cref="T:byte[]"/> to <see cref="List{T}"/> mapping.
    /// </summary>
    [TestClass]
    public class ArrayToListPropertyHandlerTest
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
            PropertyHandlerMapper.Remove(typeof(List<byte>));
            PropertyHandlerCache.Flush();
            Database.Cleanup();
        }

        #region Classes

        /// <summary>
        /// Property level (attribute): the handler is bound to the property via <see cref="PropertyHandlerAttribute"/>.
        /// </summary>
        [Map("[dbo].[CompleteTable]")]
        private class ByteListAttributeModel
        {
            public Guid SessionId { get; set; }

            [Map("ColumnVarBinary"), PropertyHandler(typeof(ArrayToListPropertyHandler<byte>))]
            public List<byte> Bytes { get; set; }
        }

        /// <summary>
        /// Property level (fluent): the handler is bound to the property via <see cref="FluentMapper"/>.
        /// </summary>
        [Map("[dbo].[CompleteTable]")]
        private class ByteListFluentModel
        {
            public Guid SessionId { get; set; }

            public List<byte> ColumnVarBinary { get; set; }
        }

        /// <summary>
        /// Type level: the handler is bound to <see cref="List{T}"/> of <see cref="byte"/> via <see cref="PropertyHandlerMapper"/>.
        /// </summary>
        [Map("[dbo].[CompleteTable]")]
        private class ByteListTypeLevelModel
        {
            public Guid SessionId { get; set; }

            public List<byte> ColumnVarBinary { get; set; }
        }

        /// <summary>
        /// Negative: no handler is bound at all to the <see cref="List{T}"/> of <see cref="byte"/> property.
        /// </summary>
        [Map("[dbo].[CompleteTable]")]
        private class ByteListNoHandlerModel
        {
            public Guid SessionId { get; set; }

            public List<byte> ColumnVarBinary { get; set; }
        }

        /// <summary>
        /// Negative: the handler expects an array of <see cref="int"/>, but the column is returned as an array of <see cref="byte"/>.
        /// </summary>
        [Map("[dbo].[CompleteTable]")]
        private class IntListOnByteColumnModel
        {
            public Guid SessionId { get; set; }

            [Map("ColumnVarBinary"), PropertyHandler(typeof(ArrayToListPropertyHandler<int>))]
            public List<int> Values { get; set; }
        }

        /// <summary>
        /// Negative: the handler expects an array, but the column is returned as a <see cref="string"/>.
        /// </summary>
        [Map("[dbo].[CompleteTable]")]
        private class ByteListOnStringColumnModel
        {
            public Guid SessionId { get; set; }

            [Map("ColumnNVarChar"), PropertyHandler(typeof(ArrayToListPropertyHandler<byte>))]
            public List<byte> Bytes { get; set; }
        }

        #endregion

        #region Helpers

        private static byte[] CreateBytes(int count,
            int seed = 1)
        {
            var random = new Random(seed);
            var bytes = new byte[count];
            random.NextBytes(bytes);
            return bytes;
        }

        private static ByteListAttributeModel CreateAttributeModel(List<byte> bytes) =>
            new ByteListAttributeModel { SessionId = Guid.NewGuid(), Bytes = bytes };

        private static byte[] GetColumnVarBinary(SqlConnection connection,
            Guid sessionId) =>
            connection.ExecuteScalar(
                "SELECT [ColumnVarBinary] FROM [dbo].[CompleteTable] WHERE [SessionId] = @SessionId;",
                new { SessionId = sessionId }) as byte[];

        private static bool IsColumnVarBinaryNull(SqlConnection connection,
            Guid sessionId) =>
            connection.ExecuteScalar<int>(
                "SELECT COUNT(1) FROM [dbo].[CompleteTable] WHERE [SessionId] = @SessionId AND [ColumnVarBinary] IS NULL;",
                new { SessionId = sessionId }) == 1;

        #endregion

        #region Positive

        [TestMethod]
        public void TestArrayToListPropertyHandler()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateAttributeModel(CreateBytes(16).ToList());

                // Act
                connection.Insert(model);
                var result = connection.Query<ByteListAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.IsNotNull(result.Bytes);
                CollectionAssert.AreEqual(model.Bytes, result.Bytes);
            }
        }

        [TestMethod]
        public async Task TestArrayToListPropertyHandlerAsync()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateAttributeModel(CreateBytes(16).ToList());

                // Act
                await connection.InsertAsync(model).ConfigureAwait(false);
                var result = (await connection.QueryAsync<ByteListAttributeModel>(e => e.SessionId == model.SessionId).ConfigureAwait(false)).First();

                // Assert
                Assert.IsNotNull(result.Bytes);
                CollectionAssert.AreEqual(model.Bytes, result.Bytes);
            }
        }

        [TestMethod]
        public void TestArrayToListPropertyHandlerWritesTheElementsAsArray()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateAttributeModel(new List<byte> { 1, 2, 3, 4, 5 });

                // Act
                connection.Insert(model);
                var raw = GetColumnVarBinary(connection, model.SessionId);

                // Assert
                Assert.IsNotNull(raw);
                CollectionAssert.AreEqual(new byte[] { 1, 2, 3, 4, 5 }, raw);
            }
        }

        [TestMethod]
        public void TestArrayToListPropertyHandlerReadsTheArrayAsListInstance()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = Guid.NewGuid();
                connection.ExecuteNonQuery(
                    "INSERT INTO [dbo].[CompleteTable] ([SessionId], [ColumnVarBinary]) VALUES (@SessionId, 0x0A0B0C);",
                    new { SessionId = sessionId });

                // Act
                var result = connection.Query<ByteListAttributeModel>(e => e.SessionId == sessionId).First();

                // Assert
                Assert.IsInstanceOfType<List<byte>>(result.Bytes);
                CollectionAssert.AreEqual(new List<byte> { 0x0A, 0x0B, 0x0C }, result.Bytes);
            }
        }

        [TestMethod]
        public void TestArrayToListPropertyHandlerWithEmptyList()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateAttributeModel(new List<byte>());

                // Act
                connection.Insert(model);
                var raw = GetColumnVarBinary(connection, model.SessionId);
                var result = connection.Query<ByteListAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.IsNotNull(raw);
                Assert.AreEqual(0, raw.Length);
                Assert.IsNotNull(result.Bytes);
                Assert.AreEqual(0, result.Bytes.Count);
            }
        }

        [TestMethod]
        public void TestArrayToListPropertyHandlerWithSingleElement()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateAttributeModel(new List<byte> { byte.MaxValue });

                // Act
                connection.Insert(model);
                var result = connection.Query<ByteListAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual(1, result.Bytes.Count);
                Assert.AreEqual(byte.MaxValue, result.Bytes[0]);
            }
        }

        [TestMethod]
        public void TestArrayToListPropertyHandlerWithLargeList()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateAttributeModel(CreateBytes(100_000).ToList());

                // Act
                connection.Insert(model);
                var result = connection.Query<ByteListAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual(model.Bytes.Count, result.Bytes.Count);
                CollectionAssert.AreEqual(model.Bytes, result.Bytes);
            }
        }

        [TestMethod]
        public void TestArrayToListPropertyHandlerWithDuplicatesAndOrder()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateAttributeModel(new List<byte> { 9, 1, 9, 0, 1, 9 });

                // Act
                connection.Insert(model);
                var result = connection.Query<ByteListAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                CollectionAssert.AreEqual(new List<byte> { 9, 1, 9, 0, 1, 9 }, result.Bytes);
            }
        }

        [TestMethod]
        public void TestArrayToListPropertyHandlerOnInsertAll()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var models = Enumerable.Range(1, 10)
                    .Select(i => CreateAttributeModel(CreateBytes(i * 3, i).ToList()))
                    .ToList();

                // Act
                connection.InsertAll(models);
                var result = connection.QueryAll<ByteListAttributeModel>().ToList();

                // Assert
                Assert.AreEqual(models.Count, result.Count);
                foreach (var model in models)
                {
                    var item = result.First(e => e.SessionId == model.SessionId);
                    CollectionAssert.AreEqual(model.Bytes, item.Bytes);
                }
            }
        }

        [TestMethod]
        public void TestArrayToListPropertyHandlerOnInsertAllWithMixedValues()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var models = new List<ByteListAttributeModel>
                {
                    CreateAttributeModel(new List<byte> { 1, 2, 3 }),
                    CreateAttributeModel(new List<byte>()),
                    CreateAttributeModel(null)
                };

                // Act
                connection.InsertAll(models);
                var result = connection.QueryAll<ByteListAttributeModel>().ToList();

                // Assert
                Assert.AreEqual(3, result.Count);
                CollectionAssert.AreEqual(models[0].Bytes, result.First(e => e.SessionId == models[0].SessionId).Bytes);
                Assert.AreEqual(0, result.First(e => e.SessionId == models[1].SessionId).Bytes.Count);
                Assert.IsNull(result.First(e => e.SessionId == models[2].SessionId).Bytes);
            }
        }

        [TestMethod]
        public void TestArrayToListPropertyHandlerOnUpdate()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateAttributeModel(new List<byte> { 1, 2, 3 });
                connection.Insert(model);

                // Act
                model.Bytes = new List<byte> { 7, 8, 9, 10 };
                var affectedRows = connection.Update(model);
                var result = connection.Query<ByteListAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual(1, affectedRows);
                CollectionAssert.AreEqual(new List<byte> { 7, 8, 9, 10 }, result.Bytes);
            }
        }

        [TestMethod]
        public void TestArrayToListPropertyHandlerOnUpdateToEmptyList()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateAttributeModel(new List<byte> { 1, 2, 3 });
                connection.Insert(model);

                // Act
                model.Bytes = new List<byte>();
                connection.Update(model);
                var result = connection.Query<ByteListAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.IsNotNull(result.Bytes);
                Assert.AreEqual(0, result.Bytes.Count);
            }
        }

        [TestMethod]
        public void TestArrayToListPropertyHandlerOnMerge()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateAttributeModel(new List<byte> { 1, 2, 3 });

                // Act
                connection.Merge(model);
                model.Bytes = new List<byte> { 4, 5, 6 };
                connection.Merge(model);
                var result = connection.QueryAll<ByteListAttributeModel>().ToList();

                // Assert
                Assert.AreEqual(1, result.Count);
                CollectionAssert.AreEqual(new List<byte> { 4, 5, 6 }, result[0].Bytes);
            }
        }

        [TestMethod]
        public void TestArrayToListPropertyHandlerOnWhereCondition()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var models = new List<ByteListAttributeModel>
                {
                    CreateAttributeModel(new List<byte> { 1, 2, 3 }),
                    CreateAttributeModel(new List<byte> { 4, 5, 6 })
                };
                connection.InsertAll(models);
                var target = models[1];

                // Act
                var result = connection.Query<ByteListAttributeModel>(e => e.Bytes == target.Bytes).ToList();

                // Assert
                Assert.AreEqual(1, result.Count);
                Assert.AreEqual(target.SessionId, result[0].SessionId);
                CollectionAssert.AreEqual(target.Bytes, result[0].Bytes);
            }
        }

        [TestMethod]
        public void TestArrayToListPropertyHandlerOnExecuteQuery()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateAttributeModel(CreateBytes(32).ToList());
                connection.Insert(model);

                // Act
                var result = connection.ExecuteQuery<ByteListAttributeModel>(
                    "SELECT [SessionId], [ColumnVarBinary] FROM [dbo].[CompleteTable] WHERE [SessionId] = @SessionId;",
                    new { SessionId = model.SessionId }).First();

                // Assert
                CollectionAssert.AreEqual(model.Bytes, result.Bytes);
            }
        }

        [TestMethod]
        public void TestArrayToListPropertyHandlerViaFluentMapper()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                FluentMapper
                    .Entity<ByteListFluentModel>()
                    .PropertyHandler(e => e.ColumnVarBinary, new ArrayToListPropertyHandler<byte>());
                var model = new ByteListFluentModel
                {
                    SessionId = Guid.NewGuid(),
                    ColumnVarBinary = CreateBytes(24).ToList()
                };

                // Act
                connection.Insert(model);
                var result = connection.Query<ByteListFluentModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                CollectionAssert.AreEqual(model.ColumnVarBinary, result.ColumnVarBinary);
            }
        }

        [TestMethod]
        public void TestArrayToListPropertyHandlerViaTypeLevelMapper()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                PropertyHandlerMapper.Add<List<byte>, ArrayToListPropertyHandler<byte>>(true);
                var model = new ByteListTypeLevelModel
                {
                    SessionId = Guid.NewGuid(),
                    ColumnVarBinary = CreateBytes(24).ToList()
                };

                // Act
                connection.Insert(model);
                var result = connection.Query<ByteListTypeLevelModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                CollectionAssert.AreEqual(model.ColumnVarBinary, result.ColumnVarBinary);
            }
        }

        #endregion

        #region Negative

        [TestMethod]
        public void TestArrayToListPropertyHandlerWithNullList()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateAttributeModel(null);

                // Act
                connection.Insert(model);
                var isNull = IsColumnVarBinaryNull(connection, model.SessionId);
                var result = connection.Query<ByteListAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.IsTrue(isNull);
                Assert.IsNull(result.Bytes);
            }
        }

        [TestMethod]
        public async Task TestArrayToListPropertyHandlerWithNullListAsync()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateAttributeModel(null);

                // Act
                await connection.InsertAsync(model).ConfigureAwait(false);
                var result = (await connection.QueryAsync<ByteListAttributeModel>(e => e.SessionId == model.SessionId).ConfigureAwait(false)).First();

                // Assert
                Assert.IsNull(result.Bytes);
            }
        }

        [TestMethod]
        public void TestArrayToListPropertyHandlerWithNullColumn()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = Guid.NewGuid();
                connection.ExecuteNonQuery(
                    "INSERT INTO [dbo].[CompleteTable] ([SessionId], [ColumnVarBinary]) VALUES (@SessionId, NULL);",
                    new { SessionId = sessionId });

                // Act
                var result = connection.Query<ByteListAttributeModel>(e => e.SessionId == sessionId).First();

                // Assert
                Assert.IsNull(result.Bytes);
            }
        }

        [TestMethod]
        public void TestArrayToListPropertyHandlerUpdateToNullList()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateAttributeModel(new List<byte> { 1, 2, 3 });
                connection.Insert(model);

                // Act
                model.Bytes = null;
                connection.Update(model);
                var isNull = IsColumnVarBinaryNull(connection, model.SessionId);
                var result = connection.Query<ByteListAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.IsTrue(isNull);
                Assert.IsNull(result.Bytes);
            }
        }

        [TestMethod]
        public void TestArrayToListPropertyHandlerOnWhereConditionWithNonMatchingList()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateAttributeModel(new List<byte> { 1, 2, 3 });
                var other = new List<byte> { 1, 2 };
                connection.Insert(model);

                // Act
                var result = connection.Query<ByteListAttributeModel>(e => e.Bytes == other).ToList();

                // Assert
                Assert.AreEqual(0, result.Count);
            }
        }

        [TestMethod]
        public void TestArrayToListPropertyHandlerWithElementTypeMismatch()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = Guid.NewGuid();
                connection.ExecuteNonQuery(
                    "INSERT INTO [dbo].[CompleteTable] ([SessionId], [ColumnVarBinary]) VALUES (@SessionId, 0x0A0B0C);",
                    new { SessionId = sessionId });

                // Act / Assert
                // The driver returns 'byte[]', which cannot be handled by the 'ArrayToListPropertyHandler<int>' (int[])
                Assert.Throws<InvalidCastException>(() =>
                    connection.Query<IntListOnByteColumnModel>(e => e.SessionId == sessionId).ToList());
            }
        }

        [TestMethod]
        public void TestArrayToListPropertyHandlerWithNonArrayColumn()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = Guid.NewGuid();
                connection.ExecuteNonQuery(
                    "INSERT INTO [dbo].[CompleteTable] ([SessionId], [ColumnNVarChar]) VALUES (@SessionId, N'Not an array');",
                    new { SessionId = sessionId });

                // Act / Assert
                // The driver returns 'string', which cannot be handled by the 'ArrayToListPropertyHandler<byte>' (byte[])
                Assert.Throws<InvalidOperationException>(() =>
                    connection.Query<ByteListOnStringColumnModel>(e => e.SessionId == sessionId).ToList());
            }
        }

        [TestMethod]
        public void TestArrayToListPropertyHandlerWithoutHandlerCannotMapArrayToList()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = Guid.NewGuid();
                connection.ExecuteNonQuery(
                    "INSERT INTO [dbo].[CompleteTable] ([SessionId], [ColumnVarBinary]) VALUES (@SessionId, 0x0A0B0C);",
                    new { SessionId = sessionId });

                // Act / Assert
                // A 'List<byte>' property with no property handler is not mappable from the 'byte[]' column
                Assert.Throws<InvalidCastException>(() =>
                    connection.Query<ByteListNoHandlerModel>(e => e.SessionId == sessionId).ToList());
            }
        }

        #endregion
    }
}
