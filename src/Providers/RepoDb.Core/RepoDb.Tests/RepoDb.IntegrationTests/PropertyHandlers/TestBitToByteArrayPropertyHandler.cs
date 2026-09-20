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
    /// The integration tests of <see cref="BitToByteArrayPropertyHandler"/>. The handler targets multi-bit columns (like the MySQL <c>BIT(n)</c>),
    /// which SQL Server does not have. The <c>BIGINT</c> column of the <c>[dbo].[CompleteTable]</c> is used to store the 8 bytes as a number
    /// (big-endian), the <c>VARBINARY(MAX)</c> column is used to read the raw bytes, and the <c>BIT</c> column is used to read a single bit.
    /// As the <c>BIGINT</c> is signed, the highest bit (the most significant bit of the first byte) cannot be stored.
    /// </summary>
    [TestClass]
    public class BitToByteArrayPropertyHandlerTest
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
            PropertyHandlerMapper.Remove(typeof(byte[]));
            PropertyHandlerCache.Flush();
            Database.Cleanup();
        }

        #region Classes

        /// <summary>
        /// Property level (attribute): the handler is bound to the property via <see cref="PropertyHandlerAttribute"/>.
        /// </summary>
        [Map("[dbo].[CompleteTable]")]
        private class ByteArrayAttributeModel
        {
            public Guid SessionId { get; set; }

            [Map("ColumnBigInt"), PropertyHandler(typeof(BitToByteArrayPropertyHandler))]
            public byte[] Bits { get; set; }
        }

        [Map("[dbo].[CompleteTable]")]
        private class ByteArrayOnBinaryColumnModel
        {
            public Guid SessionId { get; set; }

            [Map("ColumnVarBinary"), PropertyHandler(typeof(BitToByteArrayPropertyHandler))]
            public byte[] Bits { get; set; }
        }

        [Map("[dbo].[CompleteTable]")]
        private class ByteArrayOnBitColumnModel
        {
            public Guid SessionId { get; set; }

            [Map("ColumnBit"), PropertyHandler(typeof(BitToByteArrayPropertyHandler))]
            public byte[] Bits { get; set; }
        }

        /// <summary>
        /// Property level (fluent): the handler is bound to the property via <see cref="FluentMapper"/>.
        /// </summary>
        [Map("[dbo].[CompleteTable]")]
        private class ByteArrayFluentModel
        {
            public Guid SessionId { get; set; }

            public byte[] ColumnBigInt { get; set; }
        }

        /// <summary>
        /// Type level: the handler is bound to <see cref="T:byte[]"/> via <see cref="PropertyHandlerMapper"/>.
        /// </summary>
        [Map("[dbo].[CompleteTable]")]
        private class ByteArrayTypeLevelModel
        {
            public Guid SessionId { get; set; }

            public byte[] ColumnBigInt { get; set; }
        }

        #endregion

        #region Helpers

        private static ByteArrayAttributeModel CreateModel(byte[] bits) =>
            new ByteArrayAttributeModel { SessionId = Guid.NewGuid(), Bits = bits };

        private static long GetRawValue(SqlConnection connection,
            Guid sessionId) =>
            connection.ExecuteScalar<long>(
                "SELECT [ColumnBigInt] FROM [dbo].[CompleteTable] WHERE [SessionId] = @SessionId;",
                new { SessionId = sessionId });

        private static bool IsRawValueNull(SqlConnection connection,
            Guid sessionId) =>
            connection.ExecuteScalar<int>(
                "SELECT COUNT(1) FROM [dbo].[CompleteTable] WHERE [SessionId] = @SessionId AND [ColumnBigInt] IS NULL;",
                new { SessionId = sessionId }) == 1;

        private static Guid InsertRawValue(SqlConnection connection,
            string column,
            string literal)
        {
            var sessionId = Guid.NewGuid();
            connection.ExecuteNonQuery(
                $"INSERT INTO [dbo].[CompleteTable] ([SessionId], [{column}]) VALUES (@SessionId, {literal});",
                new { SessionId = sessionId });
            return sessionId;
        }

        #endregion

        #region Positive

        [TestMethod]
        public void TestBitToByteArrayPropertyHandler()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(new byte[] { 0, 0, 0, 0, 0, 0, 1, 2 });

                // Act
                connection.Insert(model);
                var result = connection.Query<ByteArrayAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                CollectionAssert.AreEqual(model.Bits, result.Bits);
            }
        }

        [TestMethod]
        public async Task TestBitToByteArrayPropertyHandlerAsync()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(new byte[] { 0, 0, 0, 0, 0, 0, 1, 2 });

                // Act
                await connection.InsertAsync(model).ConfigureAwait(false);
                var result = (await connection.QueryAsync<ByteArrayAttributeModel>(e => e.SessionId == model.SessionId).ConfigureAwait(false)).First();

                // Assert
                CollectionAssert.AreEqual(model.Bits, result.Bits);
            }
        }

        [TestMethod]
        public void TestBitToByteArrayPropertyHandlerWritesTheBytesAsBigEndianNumber()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(new byte[] { 0, 0, 0, 0, 0, 0, 1, 2 });

                // Act
                connection.Insert(model);
                var raw = GetRawValue(connection, model.SessionId);

                // Assert
                Assert.AreEqual(258L, raw);
            }
        }

        [TestMethod]
        public void TestBitToByteArrayPropertyHandlerReadsTheNumberAsEightBigEndianBytes()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, "ColumnBigInt", "5");

                // Act
                var result = connection.Query<ByteArrayAttributeModel>(e => e.SessionId == sessionId).First();

                // Assert
                CollectionAssert.AreEqual(new byte[] { 0, 0, 0, 0, 0, 0, 0, 5 }, result.Bits);
            }
        }

        [TestMethod]
        public void TestBitToByteArrayPropertyHandlerWithShorterByteArray()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(new byte[] { 1, 2 });

                // Act
                connection.Insert(model);
                var raw = GetRawValue(connection, model.SessionId);
                var result = connection.Query<ByteArrayAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                // The width of the column is unknown, therefore the value is always read back as 8 bytes with the leading zero bytes
                Assert.AreEqual(258L, raw);
                CollectionAssert.AreEqual(new byte[] { 0, 0, 0, 0, 0, 0, 1, 2 }, result.Bits);
            }
        }

        [TestMethod]
        public void TestBitToByteArrayPropertyHandlerWithEmptyByteArray()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(Array.Empty<byte>());

                // Act
                connection.Insert(model);
                var raw = GetRawValue(connection, model.SessionId);
                var result = connection.Query<ByteArrayAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual(0L, raw);
                CollectionAssert.AreEqual(new byte[8], result.Bits);
            }
        }

        [TestMethod]
        public void TestBitToByteArrayPropertyHandlerWithHighestStorableValue()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(new byte[] { 0x7F, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF });

                // Act
                connection.Insert(model);
                var raw = GetRawValue(connection, model.SessionId);
                var result = connection.Query<ByteArrayAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual(long.MaxValue, raw);
                CollectionAssert.AreEqual(model.Bits, result.Bits);
            }
        }

        [TestMethod]
        public void TestBitToByteArrayPropertyHandlerReadsTheBinaryColumn()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, "ColumnVarBinary", "0x0102");

                // Act
                var result = connection.Query<ByteArrayOnBinaryColumnModel>(e => e.SessionId == sessionId).First();

                // Assert
                CollectionAssert.AreEqual(new byte[] { 0, 0, 0, 0, 0, 0, 1, 2 }, result.Bits);
            }
        }

        [TestMethod]
        public void TestBitToByteArrayPropertyHandlerReadsTheBinaryColumnOfEightBytes()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, "ColumnVarBinary", "0xFF00000000000001");

                // Act
                var result = connection.Query<ByteArrayOnBinaryColumnModel>(e => e.SessionId == sessionId).First();

                // Assert
                CollectionAssert.AreEqual(new byte[] { 0xFF, 0, 0, 0, 0, 0, 0, 1 }, result.Bits);
            }
        }

        [TestMethod]
        public void TestBitToByteArrayPropertyHandlerReadsTheBitColumnAsSingleBit()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var setId = InsertRawValue(connection, "ColumnBit", "1");
                var unsetId = InsertRawValue(connection, "ColumnBit", "0");

                // Act
                var setResult = connection.Query<ByteArrayOnBitColumnModel>(e => e.SessionId == setId).First();
                var unsetResult = connection.Query<ByteArrayOnBitColumnModel>(e => e.SessionId == unsetId).First();

                // Assert
                CollectionAssert.AreEqual(new byte[] { 0, 0, 0, 0, 0, 0, 0, 1 }, setResult.Bits);
                CollectionAssert.AreEqual(new byte[8], unsetResult.Bits);
            }
        }

        [TestMethod]
        public void TestBitToByteArrayPropertyHandlerOnInsertAll()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var models = Enumerable.Range(1, 10).Select(i => CreateModel(new byte[] { 0, 0, 0, 0, 0, 0, (byte)i, (byte)(i * 2) })).ToList();

                // Act
                connection.InsertAll(models);
                var result = connection.QueryAll<ByteArrayAttributeModel>().ToList();

                // Assert
                Assert.AreEqual(models.Count, result.Count);
                foreach (var model in models)
                {
                    CollectionAssert.AreEqual(model.Bits, result.First(e => e.SessionId == model.SessionId).Bits);
                }
            }
        }

        [TestMethod]
        public void TestBitToByteArrayPropertyHandlerOnUpdate()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(new byte[] { 0, 0, 0, 0, 0, 0, 0, 1 });
                connection.Insert(model);

                // Act
                model.Bits = new byte[] { 0, 0, 0, 0, 0, 0, 0, 9 };
                var affectedRows = connection.Update(model);
                var result = connection.Query<ByteArrayAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual(1, affectedRows);
                Assert.AreEqual(9L, GetRawValue(connection, model.SessionId));
                CollectionAssert.AreEqual(model.Bits, result.Bits);
            }
        }

        [TestMethod]
        public void TestBitToByteArrayPropertyHandlerOnExecuteQuery()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(new byte[] { 0, 0, 0, 0, 0, 0, 0, 4 });
                connection.Insert(model);

                // Act
                var result = connection.ExecuteQuery<ByteArrayAttributeModel>(
                    "SELECT [SessionId], [ColumnBigInt] FROM [dbo].[CompleteTable] WHERE [SessionId] = @SessionId;",
                    new { SessionId = model.SessionId }).First();

                // Assert
                CollectionAssert.AreEqual(model.Bits, result.Bits);
            }
        }

        [TestMethod]
        public void TestBitToByteArrayPropertyHandlerViaFluentMapper()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                FluentMapper
                    .Entity<ByteArrayFluentModel>()
                    .PropertyHandler(e => e.ColumnBigInt, new BitToByteArrayPropertyHandler());
                var model = new ByteArrayFluentModel { SessionId = Guid.NewGuid(), ColumnBigInt = new byte[] { 0, 0, 0, 0, 0, 0, 0, 7 } };

                // Act
                connection.Insert(model);
                var result = connection.Query<ByteArrayFluentModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                CollectionAssert.AreEqual(model.ColumnBigInt, result.ColumnBigInt);
            }
        }

        [TestMethod]
        public void TestBitToByteArrayPropertyHandlerViaTypeLevelMapper()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                PropertyHandlerMapper.Add<byte[], BitToByteArrayPropertyHandler>(true);
                var model = new ByteArrayTypeLevelModel { SessionId = Guid.NewGuid(), ColumnBigInt = new byte[] { 0, 0, 0, 0, 0, 0, 0, 7 } };

                // Act
                connection.Insert(model);
                var result = connection.Query<ByteArrayTypeLevelModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                CollectionAssert.AreEqual(model.ColumnBigInt, result.ColumnBigInt);
            }
        }

        #endregion

        #region Negative

        [TestMethod]
        public void TestBitToByteArrayPropertyHandlerWithNullByteArray()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(null);

                // Act
                connection.Insert(model);
                var isNull = IsRawValueNull(connection, model.SessionId);
                var result = connection.Query<ByteArrayAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.IsTrue(isNull);
                Assert.IsNull(result.Bits);
            }
        }

        [TestMethod]
        public void TestBitToByteArrayPropertyHandlerWithNullColumn()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, "ColumnBigInt", "NULL");

                // Act
                var result = connection.Query<ByteArrayAttributeModel>(e => e.SessionId == sessionId).First();

                // Assert
                Assert.IsNull(result.Bits);
            }
        }

        [TestMethod]
        public void TestBitToByteArrayPropertyHandlerWithNullBinaryColumn()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, "ColumnVarBinary", "NULL");

                // Act
                var result = connection.Query<ByteArrayOnBinaryColumnModel>(e => e.SessionId == sessionId).First();

                // Assert
                Assert.IsNull(result.Bits);
            }
        }

        [TestMethod]
        public void TestBitToByteArrayPropertyHandlerUpdateToNullByteArray()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(new byte[] { 0, 0, 0, 0, 0, 0, 0, 1 });
                connection.Insert(model);

                // Act
                model.Bits = null;
                connection.Update(model);
                var isNull = IsRawValueNull(connection, model.SessionId);
                var result = connection.Query<ByteArrayAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.IsTrue(isNull);
                Assert.IsNull(result.Bits);
            }
        }

        [TestMethod]
        public void TestBitToByteArrayPropertyHandlerWithMoreThanEightBytes()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(new byte[9]);

                // Act / Assert
                Assert.Throws<ArgumentException>(() => connection.Insert(model));
            }
        }

        [TestMethod]
        public void TestBitToByteArrayPropertyHandlerWithMoreThanEightBytesDoesNotWriteAnyRow()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(new byte[16]);

                // Act
                Assert.Throws<ArgumentException>(() => connection.Insert(model));
                var count = connection.CountAll<ByteArrayAttributeModel>();

                // Assert
                Assert.AreEqual(0, count);
            }
        }

        [TestMethod]
        public void TestBitToByteArrayPropertyHandlerWithTheHighestBitSetCannotBeStoredInSignedBigInt()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                // The most significant bit makes the value greater than the maximum of a signed 64-bit integer
                var model = CreateModel(new byte[] { 0x80, 0, 0, 0, 0, 0, 0, 0 });

                // Act / Assert
                Assert.Throws<OverflowException>(() => connection.Insert(model));
            }
        }

        [TestMethod]
        public void TestBitToByteArrayPropertyHandlerWithNegativeNumberColumn()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, "ColumnBigInt", "-1");

                // Act / Assert
                Assert.Throws<OverflowException>(() =>
                    connection.Query<ByteArrayAttributeModel>(e => e.SessionId == sessionId).ToList());
            }
        }

        [TestMethod]
        public void TestBitToByteArrayPropertyHandlerWithMoreThanEightBytesColumn()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, "ColumnVarBinary", "0x010203040506070809");

                // Act / Assert
                Assert.Throws<ArgumentException>(() =>
                    connection.Query<ByteArrayOnBinaryColumnModel>(e => e.SessionId == sessionId).ToList());
            }
        }

        [TestMethod]
        public void TestBitToByteArrayPropertyHandlerWriteOnBinaryColumnIsNotSupported()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                // The handler writes the value as a number, which cannot be converted to the parameter of a 'VARBINARY' column
                var model = new ByteArrayOnBinaryColumnModel { SessionId = Guid.NewGuid(), Bits = new byte[] { 1, 2 } };

                // Act / Assert
                Assert.Throws<InvalidCastException>(() => connection.Insert(model));
            }
        }

        #endregion
    }
}
