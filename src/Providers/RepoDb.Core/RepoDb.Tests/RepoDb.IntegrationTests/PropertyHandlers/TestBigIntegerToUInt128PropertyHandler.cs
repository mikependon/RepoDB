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
    /// The integration tests of <see cref="BigIntegerToUInt128PropertyHandler"/>. The handler targets 128-bit integer columns that the driver exposes as a
    /// <see cref="System.Numerics.BigInteger"/> (like the Firebird <c>INT128</c>), which SQL Server does not have. The <c>BIGINT</c> and <c>INT</c>
    /// columns of the <c>[dbo].[CompleteTable]</c> (returned by the driver as <see cref="long"/> and <see cref="int"/>) are used to test the read side.
    /// The write side is limited on SQL Server as the driver does not accept a <see cref="System.Numerics.BigInteger"/> parameter value.
    /// </summary>
    [TestClass]
    public class BigIntegerToUInt128PropertyHandlerTest
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
            PropertyHandlerMapper.Remove(typeof(UInt128));
            PropertyHandlerCache.Flush();
            Database.Cleanup();
        }

        #region Classes

        /// <summary>
        /// Property level (attribute): the handler is bound to the property via <see cref="PropertyHandlerAttribute"/>.
        /// </summary>
        [Map("[dbo].[CompleteTable]")]
        private class AttributeModel
        {
            public Guid SessionId { get; set; }

            [Map("ColumnBigInt"), PropertyHandler(typeof(BigIntegerToUInt128PropertyHandler))]
            public UInt128 Value { get; set; }
        }

        [Map("[dbo].[CompleteTable]")]
        private class OnIntColumnModel
        {
            public Guid SessionId { get; set; }

            [Map("ColumnInt"), PropertyHandler(typeof(BigIntegerToUInt128PropertyHandler))]
            public UInt128 Value { get; set; }
        }

        /// <summary>
        /// Property level (fluent): the handler is bound to the property via <see cref="FluentMapper"/>.
        /// </summary>
        [Map("[dbo].[CompleteTable]")]
        private class FluentModel
        {
            public Guid SessionId { get; set; }

            public UInt128 ColumnBigInt { get; set; }
        }

        /// <summary>
        /// Type level: the handler is bound to <see cref="UInt128"/> via <see cref="PropertyHandlerMapper"/>.
        /// </summary>
        [Map("[dbo].[CompleteTable]")]
        private class TypeLevelModel
        {
            public Guid SessionId { get; set; }

            public UInt128 ColumnBigInt { get; set; }
        }

        /// <summary>
        /// Negative: the handler supports <see cref="long"/>, <see cref="int"/> and <see cref="System.Numerics.BigInteger"/>, but the column is returned as a <see cref="decimal"/>.
        /// </summary>
        [Map("[dbo].[CompleteTable]")]
        private class OnDecimalColumnModel
        {
            public Guid SessionId { get; set; }

            [Map("ColumnDecimal"), PropertyHandler(typeof(BigIntegerToUInt128PropertyHandler))]
            public UInt128 Value { get; set; }
        }

        /// <summary>
        /// Negative: the handler supports <see cref="long"/>, <see cref="int"/> and <see cref="System.Numerics.BigInteger"/>, but the column is returned as a <see cref="short"/>.
        /// </summary>
        [Map("[dbo].[CompleteTable]")]
        private class OnSmallIntColumnModel
        {
            public Guid SessionId { get; set; }

            [Map("ColumnSmallInt"), PropertyHandler(typeof(BigIntegerToUInt128PropertyHandler))]
            public UInt128 Value { get; set; }
        }

        /// <summary>
        /// Negative: the handler supports <see cref="long"/>, <see cref="int"/> and <see cref="System.Numerics.BigInteger"/>, but the column is returned as a <see cref="string"/>.
        /// </summary>
        [Map("[dbo].[CompleteTable]")]
        private class OnStringColumnModel
        {
            public Guid SessionId { get; set; }

            [Map("ColumnNVarChar"), PropertyHandler(typeof(BigIntegerToUInt128PropertyHandler))]
            public UInt128 Value { get; set; }
        }

        #endregion

        #region Helpers

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

        private static bool IsRawValueNull(SqlConnection connection,
            Guid sessionId) =>
            connection.ExecuteScalar<int>(
                "SELECT COUNT(1) FROM [dbo].[CompleteTable] WHERE [SessionId] = @SessionId AND [ColumnBigInt] IS NULL;",
                new { SessionId = sessionId }) == 1;

        #endregion

        #region Positive

        [TestMethod]
        public void TestBigIntegerToUInt128PropertyHandler()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, "ColumnBigInt", "5");

                // Act
                var result = connection.Query<AttributeModel>(e => e.SessionId == sessionId).First();

                // Assert
                Assert.AreEqual((UInt128)5, result.Value);
            }
        }

        [TestMethod]
        public async Task TestBigIntegerToUInt128PropertyHandlerAsync()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, "ColumnBigInt", "5");

                // Act
                var result = (await connection.QueryAsync<AttributeModel>(e => e.SessionId == sessionId).ConfigureAwait(false)).First();

                // Assert
                Assert.AreEqual((UInt128)5, result.Value);
            }
        }

        [TestMethod]
        [DataRow(0L)]
        [DataRow(1L)]
        [DataRow(4294967296L)]
        [DataRow(long.MaxValue)]
        public void TestBigIntegerToUInt128PropertyHandlerReadsTheBigIntColumn(long value)
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, "ColumnBigInt", value.ToString(System.Globalization.CultureInfo.InvariantCulture));

                // Act
                var result = connection.Query<AttributeModel>(e => e.SessionId == sessionId).First();

                // Assert
                Assert.AreEqual((UInt128)value, result.Value);
            }
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(42)]
        [DataRow(int.MaxValue)]
        public void TestBigIntegerToUInt128PropertyHandlerReadsTheIntColumn(int value)
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, "ColumnInt", value.ToString(System.Globalization.CultureInfo.InvariantCulture));

                // Act
                var result = connection.Query<OnIntColumnModel>(e => e.SessionId == sessionId).First();

                // Assert
                Assert.AreEqual((UInt128)value, result.Value);
            }
        }

        [TestMethod]
        public void TestBigIntegerToUInt128PropertyHandlerOnQueryAll()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionIds = Enumerable.Range(1, 10)
                    .Select(i => InsertRawValue(connection, "ColumnBigInt", (i * 1000).ToString(System.Globalization.CultureInfo.InvariantCulture)))
                    .ToList();

                // Act
                var result = connection.QueryAll<AttributeModel>().ToList();

                // Assert
                Assert.AreEqual(sessionIds.Count, result.Count);
                for (var i = 0; i < sessionIds.Count; i++)
                {
                    Assert.AreEqual((UInt128)((i + 1) * 1000), result.First(e => e.SessionId == sessionIds[i]).Value);
                }
            }
        }

        [TestMethod]
        public void TestBigIntegerToUInt128PropertyHandlerOnExecuteQuery()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, "ColumnBigInt", "77");

                // Act
                var result = connection.ExecuteQuery<AttributeModel>(
                    "SELECT [SessionId], [ColumnBigInt] FROM [dbo].[CompleteTable] WHERE [SessionId] = @SessionId;",
                    new { SessionId = sessionId }).First();

                // Assert
                Assert.AreEqual((UInt128)77, result.Value);
            }
        }

        [TestMethod]
        public void TestBigIntegerToUInt128PropertyHandlerViaFluentMapper()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                FluentMapper
                    .Entity<FluentModel>()
                    .PropertyHandler(e => e.ColumnBigInt, new BigIntegerToUInt128PropertyHandler());
                var sessionId = InsertRawValue(connection, "ColumnBigInt", "88");

                // Act
                var result = connection.Query<FluentModel>(e => e.SessionId == sessionId).First();

                // Assert
                Assert.AreEqual((UInt128)88, result.ColumnBigInt);
            }
        }

        [TestMethod]
        public void TestBigIntegerToUInt128PropertyHandlerViaTypeLevelMapper()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                PropertyHandlerMapper.Add<UInt128, BigIntegerToUInt128PropertyHandler>(true);
                var sessionId = InsertRawValue(connection, "ColumnBigInt", "99");

                // Act
                var result = connection.Query<TypeLevelModel>(e => e.SessionId == sessionId).First();

                // Assert
                Assert.AreEqual((UInt128)99, result.ColumnBigInt);
            }
        }

        #endregion

        #region Negative

        [TestMethod]
        public void TestBigIntegerToUInt128PropertyHandlerWithNullColumn()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, "ColumnBigInt", "NULL");

                // Act
                var result = connection.Query<AttributeModel>(e => e.SessionId == sessionId).First();

                // Assert
                Assert.AreEqual(default(UInt128), result.Value);
            }
        }

        [TestMethod]
        public void TestBigIntegerToUInt128PropertyHandlerWithNullIntColumn()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, "ColumnInt", "NULL");

                // Act
                var result = connection.Query<OnIntColumnModel>(e => e.SessionId == sessionId).First();

                // Assert
                Assert.AreEqual(default(UInt128), result.Value);
            }
        }

        [TestMethod]
        public void TestBigIntegerToUInt128PropertyHandlerWithNegativeBigIntColumn()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, "ColumnBigInt", "-1");

                // Act / Assert
                Assert.Throws<OverflowException>(() =>
                    connection.Query<AttributeModel>(e => e.SessionId == sessionId).ToList());
            }
        }

        [TestMethod]
        public void TestBigIntegerToUInt128PropertyHandlerWithNegativeIntColumn()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, "ColumnInt", "-1");

                // Act / Assert
                Assert.Throws<OverflowException>(() =>
                    connection.Query<OnIntColumnModel>(e => e.SessionId == sessionId).ToList());
            }
        }

        [TestMethod]
        public void TestBigIntegerToUInt128PropertyHandlerWriteOfValueIsNotSupported()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                // The handler writes a 'BigInteger', which the SQL Server driver cannot convert to the parameter of the 'BIGINT' column
                var model = new AttributeModel { SessionId = Guid.NewGuid(), Value = (UInt128)42 };

                // Act / Assert
                Assert.Throws<InvalidCastException>(() => connection.Insert(model));
            }
        }

        [TestMethod]
        public void TestBigIntegerToUInt128PropertyHandlerWriteOfValueIsNotSupportedDoesNotWriteAnyRow()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var model = new AttributeModel { SessionId = Guid.NewGuid(), Value = (UInt128)42 };

                // Act
                Assert.Throws<InvalidCastException>(() => connection.Insert(model));
                var count = connection.CountAll<AttributeModel>();

                // Assert
                Assert.AreEqual(0, count);
            }
        }

        [TestMethod]
        public void TestBigIntegerToUInt128PropertyHandlerWithDecimalColumn()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, "ColumnDecimal", "5.00");

                // Act / Assert
                // The driver returns 'decimal', which is not a supported 128-bit integer value
                Assert.Throws<ArgumentException>(() =>
                    connection.Query<OnDecimalColumnModel>(e => e.SessionId == sessionId).ToList());
            }
        }

        [TestMethod]
        public void TestBigIntegerToUInt128PropertyHandlerWithSmallIntColumn()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, "ColumnSmallInt", "5");

                // Act / Assert
                // The driver returns 'short', which is not a supported 128-bit integer value
                Assert.Throws<ArgumentException>(() =>
                    connection.Query<OnSmallIntColumnModel>(e => e.SessionId == sessionId).ToList());
            }
        }

        [TestMethod]
        public void TestBigIntegerToUInt128PropertyHandlerWithTextColumn()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var sessionId = InsertRawValue(connection, "ColumnNVarChar", "N'12345'");

                // Act / Assert
                // The driver returns 'string', which is not a supported 128-bit integer value
                Assert.Throws<ArgumentException>(() =>
                    connection.Query<OnStringColumnModel>(e => e.SessionId == sessionId).ToList());
            }
        }

        #endregion
    }
}
