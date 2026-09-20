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
using System.Collections;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.IntegrationTests.PropertyHandlers
{
    /// <summary>
    /// The integration tests of <see cref="BitToBitArrayPropertyHandler"/>. The handler targets multi-bit columns (like the MySQL <c>BIT(n)</c>),
    /// which SQL Server does not have. The <c>BIGINT</c> column of the <c>[dbo].[CompleteTable]</c> is used to store the 64 bits as a number,
    /// and the <c>BIT</c> column is used to read a single bit (a <see cref="bool"/> returned by the driver).
    /// As the <c>BIGINT</c> is signed, the highest bit (index 63) cannot be stored.
    /// </summary>
    [TestClass]
    public class BitToBitArrayPropertyHandlerTest
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
            PropertyHandlerMapper.Remove(typeof(BitArray));
            PropertyHandlerCache.Flush();
            Database.Cleanup();
        }

        #region Classes

        /// <summary>
        /// Property level (attribute): the handler is bound to the property via <see cref="PropertyHandlerAttribute"/>.
        /// </summary>
        [Map("[dbo].[CompleteTable]")]
        private class BitArrayAttributeModel
        {
            public Guid SessionId { get; set; }

            [Map("ColumnBigInt"), PropertyHandler(typeof(BitToBitArrayPropertyHandler))]
            public BitArray Bits { get; set; }
        }

        [Map("[dbo].[CompleteTable]")]
        private class BitArrayOnBitColumnModel
        {
            public Guid SessionId { get; set; }

            [Map("ColumnBit"), PropertyHandler(typeof(BitToBitArrayPropertyHandler))]
            public BitArray Bits { get; set; }
        }

        /// <summary>
        /// Property level (fluent): the handler is bound to the property via <see cref="FluentMapper"/>.
        /// </summary>
        [Map("[dbo].[CompleteTable]")]
        private class BitArrayFluentModel
        {
            public Guid SessionId { get; set; }

            public BitArray ColumnBigInt { get; set; }
        }

        /// <summary>
        /// Type level: the handler is bound to <see cref="BitArray"/> via <see cref="PropertyHandlerMapper"/>.
        /// </summary>
        [Map("[dbo].[CompleteTable]")]
        private class BitArrayTypeLevelModel
        {
            public Guid SessionId { get; set; }

            public BitArray ColumnBigInt { get; set; }
        }

        /// <summary>
        /// Negative: the handler supports numbers, but the column is returned as a text that is not a number.
        /// </summary>
        [Map("[dbo].[CompleteTable]")]
        private class BitArrayOnStringColumnModel
        {
            public Guid SessionId { get; set; }

            [Map("ColumnNVarChar"), PropertyHandler(typeof(BitToBitArrayPropertyHandler))]
            public BitArray Bits { get; set; }
        }

        #endregion

        #region Helpers

        private static BitArray CreateBits(int length,
            params int[] setIndexes)
        {
            var bits = new BitArray(length);
            foreach (var index in setIndexes)
            {
                bits[index] = true;
            }
            return bits;
        }

        private static void AssertBits(BitArray actual,
            params int[] setIndexes)
        {
            Assert.IsNotNull(actual);
            Assert.AreEqual(64, actual.Length);
            for (var index = 0; index < actual.Length; index++)
            {
                Assert.AreEqual(setIndexes.Contains(index), actual[index], $"The bit at index {index} is not the expected one.");
            }
        }

        private static BitArrayAttributeModel CreateModel(BitArray bits) =>
            new BitArrayAttributeModel { SessionId = Guid.NewGuid(), Bits = bits };

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
        public void TestBitToBitArrayPropertyHandler()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(CreateBits(64, 0, 3, 10, 62));

                // Act
                connection.Insert(model);
                var result = connection.Query<BitArrayAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                AssertBits(result.Bits, 0, 3, 10, 62);
            }
        }

        [TestMethod]
        public async Task TestBitToBitArrayPropertyHandlerAsync()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(CreateBits(64, 0, 3, 10, 62));

                // Act
                await connection.InsertAsync(model).ConfigureAwait(false);
                var result = (await connection.QueryAsync<BitArrayAttributeModel>(e => e.SessionId == model.SessionId).ConfigureAwait(false)).First();

                // Assert
                AssertBits(result.Bits, 0, 3, 10, 62);
            }
        }

        [TestMethod]
        public void TestBitToBitArrayPropertyHandlerWritesTheBitsAsNumberWithIndexZeroAsLeastSignificantBit()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(CreateBits(64, 0, 3));

                // Act
                connection.Insert(model);
                var raw = GetRawValue(connection, model.SessionId);

                // Assert
                Assert.AreEqual(9L, raw);
            }
        }

        [TestMethod]
        public void TestBitToBitArrayPropertyHandlerReadsTheNumberAsBits()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, "ColumnBigInt", "5");

                // Act
                var result = connection.Query<BitArrayAttributeModel>(e => e.SessionId == sessionId).First();

                // Assert
                AssertBits(result.Bits, 0, 2);
            }
        }

        [TestMethod]
        public void TestBitToBitArrayPropertyHandlerWithShorterBitArray()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(CreateBits(8, 1, 7));

                // Act
                connection.Insert(model);
                var raw = GetRawValue(connection, model.SessionId);
                var result = connection.Query<BitArrayAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                // The width of the column is unknown, therefore the value is always read back as 64 bits
                Assert.AreEqual(130L, raw);
                AssertBits(result.Bits, 1, 7);
            }
        }

        [TestMethod]
        public void TestBitToBitArrayPropertyHandlerWithEmptyBitArray()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(new BitArray(0));

                // Act
                connection.Insert(model);
                var raw = GetRawValue(connection, model.SessionId);
                var result = connection.Query<BitArrayAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual(0L, raw);
                AssertBits(result.Bits);
            }
        }

        [TestMethod]
        public void TestBitToBitArrayPropertyHandlerWithNoBitSet()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(new BitArray(64));

                // Act
                connection.Insert(model);
                var raw = GetRawValue(connection, model.SessionId);
                var result = connection.Query<BitArrayAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual(0L, raw);
                AssertBits(result.Bits);
            }
        }

        [TestMethod]
        public void TestBitToBitArrayPropertyHandlerWithHighestStorableBit()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(CreateBits(64, 62));

                // Act
                connection.Insert(model);
                var raw = GetRawValue(connection, model.SessionId);
                var result = connection.Query<BitArrayAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual(1L << 62, raw);
                AssertBits(result.Bits, 62);
            }
        }

        [TestMethod]
        public void TestBitToBitArrayPropertyHandlerWithAllStorableBitsSet()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(CreateBits(64, Enumerable.Range(0, 63).ToArray()));

                // Act
                connection.Insert(model);
                var raw = GetRawValue(connection, model.SessionId);
                var result = connection.Query<BitArrayAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual(long.MaxValue, raw);
                AssertBits(result.Bits, Enumerable.Range(0, 63).ToArray());
            }
        }

        [TestMethod]
        public void TestBitToBitArrayPropertyHandlerReadsTheBitColumnAsSingleBit()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var setId = InsertRawValue(connection, "ColumnBit", "1");
                var unsetId = InsertRawValue(connection, "ColumnBit", "0");

                // Act
                var setResult = connection.Query<BitArrayOnBitColumnModel>(e => e.SessionId == setId).First();
                var unsetResult = connection.Query<BitArrayOnBitColumnModel>(e => e.SessionId == unsetId).First();

                // Assert
                AssertBits(setResult.Bits, 0);
                AssertBits(unsetResult.Bits);
            }
        }

        [TestMethod]
        public void TestBitToBitArrayPropertyHandlerOnInsertAll()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var models = Enumerable.Range(0, 10).Select(i => CreateModel(CreateBits(64, i, i + 20))).ToList();

                // Act
                connection.InsertAll(models);
                var result = connection.QueryAll<BitArrayAttributeModel>().ToList();

                // Assert
                Assert.AreEqual(models.Count, result.Count);
                for (var i = 0; i < models.Count; i++)
                {
                    AssertBits(result.First(e => e.SessionId == models[i].SessionId).Bits, i, i + 20);
                }
            }
        }

        [TestMethod]
        public void TestBitToBitArrayPropertyHandlerOnUpdate()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(CreateBits(64, 0));
                connection.Insert(model);

                // Act
                model.Bits = CreateBits(64, 1, 2);
                var affectedRows = connection.Update(model);
                var result = connection.Query<BitArrayAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.AreEqual(1, affectedRows);
                Assert.AreEqual(6L, GetRawValue(connection, model.SessionId));
                AssertBits(result.Bits, 1, 2);
            }
        }

        [TestMethod]
        public void TestBitToBitArrayPropertyHandlerOnExecuteQuery()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(CreateBits(64, 4));
                connection.Insert(model);

                // Act
                var result = connection.ExecuteQuery<BitArrayAttributeModel>(
                    "SELECT [SessionId], [ColumnBigInt] FROM [dbo].[CompleteTable] WHERE [SessionId] = @SessionId;",
                    new { SessionId = model.SessionId }).First();

                // Assert
                AssertBits(result.Bits, 4);
            }
        }

        [TestMethod]
        public void TestBitToBitArrayPropertyHandlerViaFluentMapper()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                FluentMapper
                    .Entity<BitArrayFluentModel>()
                    .PropertyHandler(e => e.ColumnBigInt, new BitToBitArrayPropertyHandler());
                var model = new BitArrayFluentModel { SessionId = Guid.NewGuid(), ColumnBigInt = CreateBits(64, 5, 6) };

                // Act
                connection.Insert(model);
                var result = connection.Query<BitArrayFluentModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                AssertBits(result.ColumnBigInt, 5, 6);
            }
        }

        [TestMethod]
        public void TestBitToBitArrayPropertyHandlerViaTypeLevelMapper()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                PropertyHandlerMapper.Add<BitArray, BitToBitArrayPropertyHandler>(true);
                var model = new BitArrayTypeLevelModel { SessionId = Guid.NewGuid(), ColumnBigInt = CreateBits(64, 5, 6) };

                // Act
                connection.Insert(model);
                var result = connection.Query<BitArrayTypeLevelModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                AssertBits(result.ColumnBigInt, 5, 6);
            }
        }

        #endregion

        #region Negative

        [TestMethod]
        public void TestBitToBitArrayPropertyHandlerWithNullBitArray()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(null);

                // Act
                connection.Insert(model);
                var isNull = IsRawValueNull(connection, model.SessionId);
                var result = connection.Query<BitArrayAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.IsTrue(isNull);
                Assert.IsNull(result.Bits);
            }
        }

        [TestMethod]
        public void TestBitToBitArrayPropertyHandlerWithNullColumn()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, "ColumnBigInt", "NULL");

                // Act
                var result = connection.Query<BitArrayAttributeModel>(e => e.SessionId == sessionId).First();

                // Assert
                Assert.IsNull(result.Bits);
            }
        }

        [TestMethod]
        public void TestBitToBitArrayPropertyHandlerWithNullBitColumn()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, "ColumnBit", "NULL");

                // Act
                var result = connection.Query<BitArrayOnBitColumnModel>(e => e.SessionId == sessionId).First();

                // Assert
                Assert.IsNull(result.Bits);
            }
        }

        [TestMethod]
        public void TestBitToBitArrayPropertyHandlerUpdateToNullBitArray()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(CreateBits(64, 1));
                connection.Insert(model);

                // Act
                model.Bits = null;
                connection.Update(model);
                var isNull = IsRawValueNull(connection, model.SessionId);
                var result = connection.Query<BitArrayAttributeModel>(e => e.SessionId == model.SessionId).First();

                // Assert
                Assert.IsTrue(isNull);
                Assert.IsNull(result.Bits);
            }
        }

        [TestMethod]
        public void TestBitToBitArrayPropertyHandlerWithMoreThan64Bits()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(new BitArray(65));

                // Act / Assert
                Assert.Throws<ArgumentException>(() => connection.Insert(model));
            }
        }

        [TestMethod]
        public void TestBitToBitArrayPropertyHandlerWithMoreThan64BitsDoesNotWriteAnyRow()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = CreateModel(new BitArray(128));

                // Act
                Assert.Throws<ArgumentException>(() => connection.Insert(model));
                var count = connection.CountAll<BitArrayAttributeModel>();

                // Assert
                Assert.AreEqual(0, count);
            }
        }

        [TestMethod]
        public void TestBitToBitArrayPropertyHandlerWithTheHighestBitSetCannotBeStoredInSignedBigInt()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                // The bit 63 makes the value greater than the maximum of a signed 64-bit integer
                var model = CreateModel(CreateBits(64, 63));

                // Act / Assert
                Assert.Throws<OverflowException>(() => connection.Insert(model));
            }
        }

        [TestMethod]
        public void TestBitToBitArrayPropertyHandlerWithNegativeNumberColumn()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, "ColumnBigInt", "-1");

                // Act / Assert
                Assert.Throws<OverflowException>(() =>
                    connection.Query<BitArrayAttributeModel>(e => e.SessionId == sessionId).ToList());
            }
        }

        [TestMethod]
        public void TestBitToBitArrayPropertyHandlerWithNonNumericTextColumn()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = Guid.NewGuid();
                connection.ExecuteNonQuery(
                    "INSERT INTO [dbo].[CompleteTable] ([SessionId], [ColumnNVarChar]) VALUES (@SessionId, N'abc');",
                    new { SessionId = sessionId });

                // Act / Assert
                Assert.Throws<FormatException>(() =>
                    connection.Query<BitArrayOnStringColumnModel>(e => e.SessionId == sessionId).ToList());
            }
        }

        #endregion
    }
}
