#region Copyright Attributions

// Copyright (c) 2019 Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using RepoDb.Extensions;
using RepoDb.IntegrationTests.Setup;
using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using RepoDb.Interfaces;
using RepoDb.Options;
using System.Globalization;

namespace RepoDb.IntegrationTests
{
    /// <summary>
    /// Contains the execute query raw test behavior.
    /// </summary>
    [TestClass]
    public class ExecuteQueryRawTest
    {
        /// <summary>
        /// Performs class initialize.
        /// </summary>
        /// <param name="context">The context.</param>
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            FluentMapper.Type<DateTime>().DbType(DbType.DateTime2, true);
            FluentMapper.Type<uint>().PropertyHandler<IntPropertyHandler>(true);
#if NET5_0_OR_GREATER
            FluentMapper.Type<StringRecord>().DbType(DbType.AnsiString, true);
#endif
        }

        /// <summary>
        /// Performs class cleaup.
        /// </summary>
        [ClassCleanup]
        public static void ClassCleaup()
        {
            TypeMapCache.Flush();
            PropertyHandlerCache.Flush();
        }

        /// <summary>
        /// Performs initialize.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            Database.Initialize();
            Cleanup();
        }

        /// <summary>
        /// Performs cleanup.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            Database.Cleanup();
        }

        #region PropertyHandlers

        /// <summary>
        /// Contains the int property handler behavior.
        /// </summary>
        private class IntPropertyHandler : IPropertyHandler<uint, uint>
        {
            /// <summary>
            /// Performs get.
            /// </summary>
            /// <param name="input">The input.</param>
            /// <param name="options">The options.</param>
            public uint Get(uint input, PropertyHandlerGetOptions options)
            {
                return input * 2;
            }

            /// <summary>
            /// Performs set.
            /// </summary>
            /// <param name="input">The input.</param>
            /// <param name="options">The options.</param>
            public uint Set(uint input, PropertyHandlerSetOptions options)
            {
                return input;
            }
        }

        #endregion

        #region Enums

        /// <summary>
        /// Contains the gender behavior.
        /// </summary>
        private enum Gender
        {
            Male = 1,
            Female = 2
        }

        #endregion

        #region Classes

        /// <summary>
        /// Contains the whatever class with non nullable properties behavior.
        /// </summary>
        private class WhateverClassWithNonNullableProperties
        {
            /// <summary>
            /// Gets or sets the id.
            /// </summary>
            public int Id { get; set; }
            /// <summary>
            /// Gets or sets the column int.
            /// </summary>
            public int ColumnInt { get; set; }
            /// <summary>
            /// Gets or sets the column big int.
            /// </summary>
            public long ColumnBigInt { get; set; }
            /// <summary>
            /// Gets or sets the column nvar char.
            /// </summary>
            public string ColumnNVarChar { get; set; }
            /// <summary>
            /// Gets or sets the column float.
            /// </summary>
            public double ColumnFloat { get; set; }
            /// <summary>
            /// Gets or sets the column decimal.
            /// </summary>
            public decimal ColumnDecimal { get; set; }
            /// <summary>
            /// Gets or sets the column date.
            /// </summary>
            public DateTime ColumnDate { get; set; }
            /// <summary>
            /// Gets or sets the column time.
            /// </summary>
            public TimeSpan ColumnTime { get; set; }
            /// <summary>
            /// Gets or sets the column date time.
            /// </summary>
            public DateTime ColumnDateTime { get; set; }
            /// <summary>
            /// Gets or sets the column date time2.
            /// </summary>
            public DateTime ColumnDateTime2 { get; set; }
        }

        /// <summary>
        /// Contains the whatever class with nullable properties behavior.
        /// </summary>
        private class WhateverClassWithNullableProperties
        {
            /// <summary>
            /// Gets or sets the id.
            /// </summary>
            public int Id { get; set; }
            /// <summary>
            /// Gets or sets the column int.
            /// </summary>
            public int? ColumnInt { get; set; }
            /// <summary>
            /// Gets or sets the column big int.
            /// </summary>
            public long? ColumnBigInt { get; set; }
            /// <summary>
            /// Gets or sets the column nvar char.
            /// </summary>
            public string ColumnNVarChar { get; set; }
            /// <summary>
            /// Gets or sets the column float.
            /// </summary>
            public double? ColumnFloat { get; set; }
            /// <summary>
            /// Gets or sets the column decimal.
            /// </summary>
            public decimal? ColumnDecimal { get; set; }
            /// <summary>
            /// Gets or sets the column date.
            /// </summary>
            public DateTime? ColumnDate { get; set; }
            /// <summary>
            /// Gets or sets the column time.
            /// </summary>
            public TimeSpan? ColumnTime { get; set; }
            /// <summary>
            /// Gets or sets the column date time.
            /// </summary>
            public DateTime? ColumnDateTime { get; set; }
            /// <summary>
            /// Gets or sets the column date time2.
            /// </summary>
            public DateTime? ColumnDateTime2 { get; set; }
        }

        /// <summary>
        /// Contains the mapped whatever class with non nullable properties behavior.
        /// </summary>
        private class MappedWhateverClassWithNonNullableProperties
        {
            /// <summary>
            /// Gets or sets the id mapped.
            /// </summary>
            [Map("Id")]
            public int IdMapped { get; set; }
            /// <summary>
            /// Gets or sets the column int mapped.
            /// </summary>
            [Map("ColumnInt")]
            public int ColumnIntMapped { get; set; }
            /// <summary>
            /// Gets or sets the column big int mapped.
            /// </summary>
            [Map("ColumnBigInt")]
            public long ColumnBigIntMapped { get; set; }
            /// <summary>
            /// Gets or sets the column nvar char mapped.
            /// </summary>
            [Map("ColumnNVarChar")]
            public string ColumnNVarCharMapped { get; set; }
            /// <summary>
            /// Gets or sets the column float mapped.
            /// </summary>
            [Map("ColumnFloat")]
            public double ColumnFloatMapped { get; set; }
            /// <summary>
            /// Gets or sets the column decimal mapped.
            /// </summary>
            [Map("ColumnDecimal")]
            public decimal ColumnDecimalMapped { get; set; }
            /// <summary>
            /// Gets or sets the column date mapped.
            /// </summary>
            [Map("ColumnDate")]
            public DateTime ColumnDateMapped { get; set; }
            /// <summary>
            /// Gets or sets the column time mapped.
            /// </summary>
            [Map("ColumnTime")]
            public TimeSpan ColumnTimeMapped { get; set; }
            /// <summary>
            /// Gets or sets the column date time mapped.
            /// </summary>
            [Map("ColumnDateTime")]
            public DateTime ColumnDateTimeMapped { get; set; }
            /// <summary>
            /// Gets or sets the column date time2 mapped.
            /// </summary>
            [Map("ColumnDateTime2")]
            public DateTime ColumnDateTime2Mapped { get; set; }
        }

        /// <summary>
        /// Contains the mapped whatever class with nullable properties behavior.
        /// </summary>
        private class MappedWhateverClassWithNullableProperties
        {
            /// <summary>
            /// Gets or sets the id mapped.
            /// </summary>
            [Map("Id")]
            public int IdMapped { get; set; }
            /// <summary>
            /// Gets or sets the column int mapped.
            /// </summary>
            [Map("ColumnInt")]
            public int? ColumnIntMapped { get; set; }
            /// <summary>
            /// Gets or sets the column big int mapped.
            /// </summary>
            [Map("ColumnBigInt")]
            public long? ColumnBigIntMapped { get; set; }
            /// <summary>
            /// Gets or sets the column nvar char mapped.
            /// </summary>
            [Map("ColumnNVarChar")]
            public string ColumnNVarCharMapped { get; set; }
            /// <summary>
            /// Gets or sets the column float mapped.
            /// </summary>
            [Map("ColumnFloat")]
            public double? ColumnFloatMapped { get; set; }
            /// <summary>
            /// Gets or sets the column decimal mapped.
            /// </summary>
            [Map("ColumnDecimal")]
            public decimal? ColumnDecimalMapped { get; set; }
            /// <summary>
            /// Gets or sets the column date mapped.
            /// </summary>
            [Map("ColumnDate")]
            public DateTime? ColumnDateMapped { get; set; }
            /// <summary>
            /// Gets or sets the column time mapped.
            /// </summary>
            [Map("ColumnTime")]
            public TimeSpan? ColumnTimeMapped { get; set; }
            /// <summary>
            /// Gets or sets the column date time mapped.
            /// </summary>
            [Map("ColumnDateTime")]
            public DateTime? ColumnDateTimeMapped { get; set; }
            /// <summary>
            /// Gets or sets the column date time2 mapped.
            /// </summary>
            [Map("ColumnDateTime2")]
            public DateTime? ColumnDateTime2Mapped { get; set; }
        }

        #endregion

#if NET5_0_OR_GREATER

        #region Records

        public sealed record StringRecord(string Value) : IConvertible
        {
            public static implicit operator string(StringRecord source) => source.Value;
            public static implicit operator StringRecord(string source) => new StringRecord(source);

            public TypeCode GetTypeCode() => throw new NotSupportedException();

            public bool ToBoolean(IFormatProvider provider) => throw new NotSupportedException();

            public byte ToByte(IFormatProvider provider) => throw new NotSupportedException();

            public char ToChar(IFormatProvider provider) => throw new NotSupportedException();

            public DateTime ToDateTime(IFormatProvider provider) => throw new NotSupportedException();

            public decimal ToDecimal(IFormatProvider provider) => throw new NotSupportedException();

            public double ToDouble(IFormatProvider provider) => throw new NotSupportedException();

            public short ToInt16(IFormatProvider provider) => throw new NotSupportedException();

            public int ToInt32(IFormatProvider provider) => throw new NotSupportedException();

            public long ToInt64(IFormatProvider provider) => throw new NotSupportedException();

            public sbyte ToSByte(IFormatProvider provider) => throw new NotSupportedException();

            public float ToSingle(IFormatProvider provider) => throw new NotSupportedException();

            public string ToString(IFormatProvider provider) => Value;

            public object ToType(Type conversionType, IFormatProvider provider) => throw new NotSupportedException();

            public ushort ToUInt16(IFormatProvider provider) => throw new NotSupportedException();

            public uint ToUInt32(IFormatProvider provider) => throw new NotSupportedException();

            public ulong ToUInt64(IFormatProvider provider) => throw new NotSupportedException();
        }

        #endregion

#endif

        #region ExecuteQuery

        #region TypeResult

        #region PropertyHandler

        /// <summary>
        /// Verifies sql connection execute query type result for property handler.
        /// </summary>
        [TestMethod]
        public void TestSqlConnectionExecuteQueryTypeResultForPropertyHandler()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteQuery<uint>("SELECT CONVERT(INT, 1) AS Value UNION ALL SELECT 2;").AsList();

                // Assert
                Assert.AreEqual(2, result.Count);
                Assert.AreEqual((uint)2, result[0]);
                Assert.AreEqual((uint)4, result[1]);
            }
        }

        /// <summary>
        /// Verifies sql connection execute query async type result for property handler.
        /// </summary>
        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryAsyncTypeResultForPropertyHandler()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = (await connection.ExecuteQueryAsync<uint>("SELECT CONVERT(INT, 1) AS Value UNION ALL SELECT 2;").ConfigureAwait(false)).AsList();

                // Assert
                Assert.AreEqual(2, result.Count);
                Assert.AreEqual((uint)2, result[0]);
                Assert.AreEqual((uint)4, result[1]);
            }
        }

        #endregion

        #region Enums

        #region String

        /// <summary>
        /// Verifies sql connection execute query type result for enum from string.
        /// </summary>
        [TestMethod]
        public void TestSqlConnectionExecuteQueryTypeResultForEnumFromString()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteQuery<Gender>("SELECT 'Male' AS Value UNION ALL SELECT 'Female';").AsList();

                // Assert
                Assert.AreEqual(2, result.Count);
                Assert.AreEqual(Gender.Male, result[0]);
                Assert.AreEqual(Gender.Female, result[1]);
            }
        }

        /// <summary>
        /// Verifies sql connection execute query async type result for enum from string.
        /// </summary>
        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryAsyncTypeResultForEnumFromString()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = (await connection.ExecuteQueryAsync<Gender>("SELECT 'Male' AS Value UNION ALL SELECT 'Female';").ConfigureAwait(false)).AsList();

                // Assert
                Assert.AreEqual(2, result.Count);
                Assert.AreEqual(Gender.Male, result[0]);
                Assert.AreEqual(Gender.Female, result[1]);
            }
        }

        /// <summary>
        /// Verifies sql connection execute query type result for nullable enum from string.
        /// </summary>
        [TestMethod]
        public void TestSqlConnectionExecuteQueryTypeResultForNullableEnumFromString()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteQuery<Gender?>("SELECT 'Male' AS Value UNION ALL SELECT 'Female';").AsList();

                // Assert
                Assert.AreEqual(2, result.Count);
                Assert.AreEqual(Gender.Male, result[0]);
                Assert.AreEqual(Gender.Female, result[1]);
            }
        }

        /// <summary>
        /// Verifies sql connection execute query async type result for nullable enum from string.
        /// </summary>
        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryAsyncTypeResultForNullableEnumFromString()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = (await connection.ExecuteQueryAsync<Gender?>("SELECT 'Male' AS Value UNION ALL SELECT 'Female';").ConfigureAwait(false)).AsList();

                // Assert
                Assert.AreEqual(2, result.Count);
                Assert.AreEqual(Gender.Male, result[0]);
                Assert.AreEqual(Gender.Female, result[1]);
            }
        }

        /// <summary>
        /// Verifies sql connection execute query type result for nullable enum from string with null results.
        /// </summary>
        [TestMethod]
        public void TestSqlConnectionExecuteQueryTypeResultForNullableEnumFromStringWithNullResults()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteQuery<Gender?>("SELECT CONVERT(NVARCHAR, NULL) AS Value UNION ALL SELECT NULL;").AsList();

                // Assert
                Assert.AreEqual(2, result.Count);
                Assert.IsNull(result[0]);
                Assert.IsNull(result[1]);
            }
        }

        /// <summary>
        /// Verifies sql connection execute query async type result for nullable enum from string with null results.
        /// </summary>
        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryAsyncTypeResultForNullableEnumFromStringWithNullResults()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = (await connection.ExecuteQueryAsync<Gender?>("SELECT CONVERT(NVARCHAR, NULL) AS Value UNION ALL SELECT NULL;").ConfigureAwait(false)).AsList();

                // Assert
                Assert.AreEqual(2, result.Count);
                Assert.IsNull(result[0]);
                Assert.IsNull(result[1]);
            }
        }

        #endregion

        #region NonString

        /// <summary>
        /// Verifies sql connection execute query type result for enum from non string.
        /// </summary>
        [TestMethod]
        public void TestSqlConnectionExecuteQueryTypeResultForEnumFromNonString()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteQuery<Gender>("SELECT 1 AS Value UNION ALL SELECT 2;").AsList();

                // Assert
                Assert.AreEqual(2, result.Count);
                Assert.AreEqual(Gender.Male, result[0]);
                Assert.AreEqual(Gender.Female, result[1]);
            }
        }

        /// <summary>
        /// Verifies sql connection execute query async type result for enum from non string.
        /// </summary>
        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryAsyncTypeResultForEnumFromNonString()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = (await connection.ExecuteQueryAsync<Gender>("SELECT 1 AS Value UNION ALL SELECT 2;").ConfigureAwait(false)).AsList();

                // Assert
                Assert.AreEqual(2, result.Count);
                Assert.AreEqual(Gender.Male, result[0]);
                Assert.AreEqual(Gender.Female, result[1]);
            }
        }

        /// <summary>
        /// Verifies sql connection execute query type result for nullable enum from non string.
        /// </summary>
        [TestMethod]
        public void TestSqlConnectionExecuteQueryTypeResultForNullableEnumFromNonString()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteQuery<Gender?>("SELECT 1 AS Value UNION ALL SELECT 2;").AsList();

                // Assert
                Assert.AreEqual(2, result.Count);
                Assert.AreEqual(Gender.Male, result[0]);
                Assert.AreEqual(Gender.Female, result[1]);
            }
        }

        /// <summary>
        /// Verifies sql connection execute query async type result for nullable enum from non string.
        /// </summary>
        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryAsyncTypeResultForNullableEnumFromNonString()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = (await connection.ExecuteQueryAsync<Gender?>("SELECT 1 AS Value UNION ALL SELECT 2;").ConfigureAwait(false)).AsList();

                // Assert
                Assert.AreEqual(2, result.Count);
                Assert.AreEqual(Gender.Male, result[0]);
                Assert.AreEqual(Gender.Female, result[1]);
            }
        }

        /// <summary>
        /// Verifies sql connection execute query type result for nullable enum from non string with null results.
        /// </summary>
        [TestMethod]
        public void TestSqlConnectionExecuteQueryTypeResultForNullableEnumFromNonStringWithNullResults()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteQuery<Gender?>("SELECT CONVERT(INT, NULL) AS Value UNION ALL SELECT NULL;").AsList();

                // Assert
                Assert.AreEqual(2, result.Count);
                Assert.IsNull(result[0]);
                Assert.IsNull(result[1]);
            }
        }

        /// <summary>
        /// Verifies sql connection execute query async type result for nullable enum from non string with null results.
        /// </summary>
        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryAsyncTypeResultForNullableEnumFromNonStringWithNullResults()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = (await connection.ExecuteQueryAsync<Gender?>("SELECT CONVERT(INT, NULL) AS Value UNION ALL SELECT NULL;").ConfigureAwait(false)).AsList();

                // Assert
                Assert.AreEqual(2, result.Count);
                Assert.IsNull(result[0]);
                Assert.IsNull(result[1]);
            }
        }

        #endregion

        #endregion

        #region NonNullables

        // String

        /// <summary>
        /// Verifies sql connection execute query type result for string.
        /// </summary>
        [TestMethod]
        public void TestSqlConnectionExecuteQueryTypeResultForString()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteQuery<string>("SELECT 'ABC' AS Value UNION ALL SELECT 'DEF';").AsList();

                // Assert
                Assert.AreEqual(2, result.Count);
                Assert.AreEqual("ABC", result[0], StringComparer.Ordinal);
                Assert.AreEqual("DEF", result[1], StringComparer.Ordinal);
            }
        }

        /// <summary>
        /// Verifies sql connection execute query async type result for string.
        /// </summary>
        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryAsyncTypeResultForString()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = (await connection.ExecuteQueryAsync<string>("SELECT 'ABC' AS Value UNION ALL SELECT 'DEF';").ConfigureAwait(false)).AsList();

                // Assert
                Assert.AreEqual(2, result.Count);
                Assert.AreEqual("ABC", result[0], StringComparer.Ordinal);
                Assert.AreEqual("DEF", result[1], StringComparer.Ordinal);
            }
        }

        /// <summary>
        /// Verifies sql connection execute query type result for string with null results.
        /// </summary>
        [TestMethod]
        public void TestSqlConnectionExecuteQueryTypeResultForStringWithNullResults()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteQuery<string>("SELECT CONVERT(NVARCHAR, NULL) AS Value UNION ALL SELECT NULL;").AsList();

                // Assert
                Assert.AreEqual(2, result.Count);
                result.ForEach(item => Assert.AreEqual(default(string), item, StringComparer.Ordinal));
            }
        }

        /// <summary>
        /// Verifies sql connection execute query async type result for string with null results.
        /// </summary>
        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryAsyncTypeResultForStringWithNullResults()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = (await connection.ExecuteQueryAsync<string>("SELECT CONVERT(NVARCHAR, NULL) AS Value UNION ALL SELECT NULL;").ConfigureAwait(false)).AsList();

                // Assert
                Assert.AreEqual(2, result.Count);
                result.ForEach(item => Assert.AreEqual(default(string), item, StringComparer.Ordinal));
            }
        }

        // Guid

        /// <summary>
        /// Verifies sql connection execute query type result for guid.
        /// </summary>
        [TestMethod]
        public void TestSqlConnectionExecuteQueryTypeResultForGuid()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var value = Guid.NewGuid();

                // Act
                var result = connection.ExecuteQuery<Guid>("SELECT @Value UNION ALL SELECT @Value;",
                    new { value }).AsList();

                // Assert
                Assert.AreEqual(2, result.Count);
                result.ForEach(item => Assert.AreEqual(value, item));
            }
        }

        /// <summary>
        /// Verifies sql connection execute query async type result for guid.
        /// </summary>
        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryAsyncTypeResultForGuid()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var value = Guid.NewGuid();

                // Act
                var result = (await connection.ExecuteQueryAsync<Guid>("SELECT @Value UNION ALL SELECT @Value;",
                    new { value }).ConfigureAwait(false)).AsList();

                // Assert
                Assert.AreEqual(2, result.Count);
                result.ForEach(item => Assert.AreEqual(value, item));
            }
        }

        /// <summary>
        /// Verifies sql connection execute query type result for guid with null results.
        /// </summary>
        [TestMethod]
        public void TestSqlConnectionExecuteQueryTypeResultForGuidWithNullResults()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteQuery<Guid>("SELECT CONVERT(UNIQUEIDENTIFIER, NULL) AS Value UNION ALL SELECT NULL;").AsList();

                // Assert
                Assert.AreEqual(2, result.Count);
                result.ForEach(item => Assert.AreEqual(default(Guid), item));
            }
        }

        /// <summary>
        /// Verifies sql connection execute query async type result for guid with null results.
        /// </summary>
        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryAsyncTypeResultForGuidWithNullResults()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = (await connection.ExecuteQueryAsync<Guid>("SELECT CONVERT(UNIQUEIDENTIFIER, NULL) AS Value UNION ALL SELECT NULL;").ConfigureAwait(false)).AsList();

                // Assert
                Assert.AreEqual(2, result.Count);
                result.ForEach(item => Assert.AreEqual(default(Guid), item));
            }
        }

        // Long

        /// <summary>
        /// Verifies sql connection execute query type result for long.
        /// </summary>
        [TestMethod]
        public void TestSqlConnectionExecuteQueryTypeResultForLong()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteQuery<long>("SELECT CONVERT(BIGINT, 100) AS Value UNION ALL SELECT 200;").AsList();

                // Assert
                Assert.AreEqual(2, result.Count);
                Assert.AreEqual(100, result[0]);
                Assert.AreEqual(200, result[1]);
            }
        }

        /// <summary>
        /// Verifies sql connection execute query async type result for long.
        /// </summary>
        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryAsyncTypeResultForLong()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = (await connection.ExecuteQueryAsync<long>("SELECT CONVERT(BIGINT, 100) AS Value UNION ALL SELECT 200;").ConfigureAwait(false)).AsList();

                // Assert
                Assert.AreEqual(2, result.Count);
                Assert.AreEqual(100, result[0]);
                Assert.AreEqual(200, result[1]);
            }
        }

        /// <summary>
        /// Verifies sql connection execute query type result for long with null results.
        /// </summary>
        [TestMethod]
        public void TestSqlConnectionExecuteQueryTypeResultForLongWithNullResults()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteQuery<long>("SELECT CONVERT(BIGINT, NULL) AS Value UNION ALL SELECT NULL;").AsList();

                // Assert
                Assert.AreEqual(2, result.Count);
                result.ForEach(item => Assert.AreEqual(default(long), item));
            }
        }

        /// <summary>
        /// Verifies sql connection execute query async type result for long with null results.
        /// </summary>
        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryAsyncTypeResultForLongWithNullResults()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = (await connection.ExecuteQueryAsync<long>("SELECT CONVERT(BIGINT, NULL) AS Value UNION ALL SELECT NULL;").ConfigureAwait(false)).AsList();

                // Assert
                Assert.AreEqual(2, result.Count);
                result.ForEach(item => Assert.AreEqual(default(long), item));
            }
        }

        // DateTime

        /// <summary>
        /// Verifies sql connection execute query type result for date time.
        /// </summary>
        [TestMethod]
        public void TestSqlConnectionExecuteQueryTypeResultForDateTime()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var value = DateTime.UtcNow.Date.AddDays(-new Random().Next(100));

                // Act
                var result = connection.ExecuteQuery<DateTime>("SELECT @Value AS Value UNION ALL SELECT @Value;",
                    new { value }).AsList();

                // Assert
                Assert.AreEqual(2, result.Count);
                result.ForEach(item => Assert.AreEqual(value, item));
            }
        }

        /// <summary>
        /// Verifies sql connection execute query async type result for date time.
        /// </summary>
        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryAsyncTypeResultForDateTime()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var value = DateTime.UtcNow.Date.AddDays(-new Random().Next(100));

                // Act
                var result = (await connection.ExecuteQueryAsync<DateTime>("SELECT @Value AS Value UNION ALL SELECT @Value;",
                    new { value }).ConfigureAwait(false)).AsList();

                // Assert
                Assert.AreEqual(2, result.Count);
                result.ForEach(item => Assert.AreEqual(value, item));
            }
        }

        /// <summary>
        /// Verifies sql connection execute query type result for date time with null results.
        /// </summary>
        [TestMethod]
        public void TestSqlConnectionExecuteQueryTypeResultForDateTimeWithNullResults()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteQuery<DateTime>("SELECT CONVERT(DATETIME, NULL) AS Value UNION ALL SELECT NULL;").AsList();

                // Assert
                Assert.AreEqual(2, result.Count);
                result.ForEach(item => Assert.AreEqual(default(DateTime), item));
            }
        }

        /// <summary>
        /// Verifies sql connection execute query async type result for date time with null results.
        /// </summary>
        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryAsyncTypeResultForDateTimeWithNullResults()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = (await connection.ExecuteQueryAsync<DateTime>("SELECT CONVERT(DATETIME, NULL) AS Value UNION ALL SELECT NULL;").ConfigureAwait(false)).AsList();

                // Assert
                Assert.AreEqual(2, result.Count);
                result.ForEach(item => Assert.AreEqual(default(DateTime), item));
            }
        }

        #endregion

        #region Nullables

        // Guid

        /// <summary>
        /// Verifies sql connection execute query type result for nullable guid.
        /// </summary>
        [TestMethod]
        public void TestSqlConnectionExecuteQueryTypeResultForNullableGuid()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var value = Guid.NewGuid();

                // Act
                var result = connection.ExecuteQuery<Guid?>("SELECT @Value UNION ALL SELECT @Value;",
                    new { value }).AsList();

                // Assert
                Assert.AreEqual(2, result.Count);
                result.ForEach(item => Assert.AreEqual(value, item));
            }
        }

        /// <summary>
        /// Verifies sql connection execute query async type result for nullable guid.
        /// </summary>
        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryAsyncTypeResultForNullableGuid()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var value = Guid.NewGuid();

                // Act
                var result = (await connection.ExecuteQueryAsync<Guid?>("SELECT @Value UNION ALL SELECT @Value;",
                    new { value }).ConfigureAwait(false)).AsList();

                // Assert
                Assert.AreEqual(2, result.Count);
                result.ForEach(item => Assert.AreEqual(value, item));
            }
        }

        /// <summary>
        /// Verifies sql connection execute query type result for nullable guid with null results.
        /// </summary>
        [TestMethod]
        public void TestSqlConnectionExecuteQueryTypeResultForNullableGuidWithNullResults()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteQuery<Guid?>("SELECT CONVERT(UNIQUEIDENTIFIER, NULL) AS Value UNION ALL SELECT NULL;").AsList();

                // Assert
                Assert.AreEqual(2, result.Count);
                result.ForEach(item => Assert.IsNull(item));
            }
        }

        /// <summary>
        /// Verifies sql connection execute query async type result for nullable guid with null results.
        /// </summary>
        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryAsyncTypeResultForNullableGuidWithNullResults()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = (await connection.ExecuteQueryAsync<Guid?>("SELECT CONVERT(UNIQUEIDENTIFIER, NULL) AS Value UNION ALL SELECT NULL;").ConfigureAwait(false)).AsList();

                // Assert
                Assert.AreEqual(2, result.Count);
                result.ForEach(item => Assert.IsNull(item));
            }
        }

        // Long

        /// <summary>
        /// Verifies sql connection execute query type result for nullable long.
        /// </summary>
        [TestMethod]
        public void TestSqlConnectionExecuteQueryTypeResultForNullableLong()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteQuery<long?>("SELECT CONVERT(BIGINT, 100) AS Value UNION ALL SELECT 200;").AsList();

                // Assert
                Assert.AreEqual(2, result.Count);
                Assert.AreEqual(100, result[0]);
                Assert.AreEqual(200, result[1]);
            }
        }

        /// <summary>
        /// Verifies sql connection execute query async type result for nullable long.
        /// </summary>
        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryAsyncTypeResultForNullableLong()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = (await connection.ExecuteQueryAsync<long?>("SELECT CONVERT(BIGINT, 100) AS Value UNION ALL SELECT 200;").ConfigureAwait(false)).AsList();

                // Assert
                Assert.AreEqual(2, result.Count);
                Assert.AreEqual(100, result[0]);
                Assert.AreEqual(200, result[1]);
            }
        }

        /// <summary>
        /// Verifies sql connection execute query type result for nullable long with null results.
        /// </summary>
        [TestMethod]
        public void TestSqlConnectionExecuteQueryTypeResultForNullableLongWithNullResults()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteQuery<long?>("SELECT CONVERT(BIGINT, NULL) AS Value UNION ALL SELECT NULL;").AsList();

                // Assert
                Assert.AreEqual(2, result.Count);
                result.ForEach(item => Assert.IsNull(item));
            }
        }

        /// <summary>
        /// Verifies sql connection execute query async type result for nullable long with null results.
        /// </summary>
        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryAsyncTypeResultForNullableLongWithNullResults()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = (await connection.ExecuteQueryAsync<long?>("SELECT CONVERT(BIGINT, NULL) AS Value UNION ALL SELECT NULL;").ConfigureAwait(false)).AsList();

                // Assert
                Assert.AreEqual(2, result.Count);
                result.ForEach(item => Assert.IsNull(item));
            }
        }

        // DateTime

        /// <summary>
        /// Verifies sql connection execute query type result for nullable date time.
        /// </summary>
        [TestMethod]
        public void TestSqlConnectionExecuteQueryTypeResultForNullableDateTime()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var value1 = DateTime.UtcNow.Date.AddDays(-100);
                var value2 = DateTime.UtcNow.Date.AddDays(-50);

                // Act
                var result = connection.ExecuteQuery<DateTime?>("SELECT @Value1 AS Value UNION ALL SELECT @Value2;",
                    new { value1, value2 }).AsList();

                // Assert
                Assert.AreEqual(2, result.Count);
                Assert.AreEqual(value1, result[0]);
                Assert.AreEqual(value2, result[1]);
            }
        }

        /// <summary>
        /// Verifies sql connection execute query async type result for nullable date time.
        /// </summary>
        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryAsyncTypeResultForNullableDateTime()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var value1 = DateTime.UtcNow.Date.AddDays(-100);
                var value2 = DateTime.UtcNow.Date.AddDays(-50);

                // Act
                var result = (await connection.ExecuteQueryAsync<DateTime?>("SELECT @Value1 AS Value UNION ALL SELECT @Value2;",
                    new { value1, value2 }).ConfigureAwait(false)).AsList();

                // Assert
                Assert.AreEqual(2, result.Count);
                Assert.AreEqual(value1, result[0]);
                Assert.AreEqual(value2, result[1]);
            }
        }

        /// <summary>
        /// Verifies sql connection execute query type result for nullable date time with null results.
        /// </summary>
        [TestMethod]
        public void TestSqlConnectionExecuteQueryTypeResultForNullableDateTimeWithNullResults()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteQuery<DateTime?>("SELECT CONVERT(DATETIME, NULL) AS Value UNION ALL SELECT NULL;").AsList();

                // Assert
                Assert.AreEqual(2, result.Count);
                result.ForEach(item => Assert.IsNull(item));
            }
        }

        /// <summary>
        /// Verifies sql connection execute query async type result for nullable date time with null results.
        /// </summary>
        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryAsyncTypeResultForNullableDateTimeWithNullResults()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = (await connection.ExecuteQueryAsync<DateTime?>("SELECT CONVERT(DATETIME, NULL) AS Value UNION ALL SELECT NULL;").ConfigureAwait(false)).AsList();

                // Assert
                Assert.AreEqual(2, result.Count);
                result.ForEach(item => Assert.IsNull(item));
            }
        }

        #endregion

        /// <summary>
        /// Verifies throw exception on sql connection execute query type result with more columns.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnSqlConnectionExecuteQueryTypeResultWithMoreColumns()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<InvalidOperationException>(() => connection.ExecuteQuery<int>("SELECT 1 AS Column1, 2 AS Column2 UNION ALL SELECT 3, 4;").AsList());
            }
        }

        /// <summary>
        /// Verifies throw exception on sql connection execute query async type result with more columns.
        /// </summary>
        [TestMethod]
        public async Task ThrowExceptionOnSqlConnectionExecuteQueryAsyncTypeResultWithMoreColumns()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<InvalidOperationException>(async () => (await connection.ExecuteQueryAsync<int>("SELECT 1 AS Column1, 2 AS Column2 UNION ALL SELECT 3, 4;").ConfigureAwait(false)).AsList()).ConfigureAwait(false);
            }
        }

        #endregion

        #region NonMapped

        #region NonNullables

        /// <summary>
        /// Verifies sql connection execute query whatever class with non nullable properties.
        /// </summary>
        [TestMethod]
        public void TestSqlConnectionExecuteQueryWhateverClassWithNonNullableProperties()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var param = new
                {
                    Id = 1,
                    ColumnInt = int.MaxValue,
                    ColumnBigInt = long.MaxValue,
                    ColumnNVarChar = Helper.GetAssemblyDescription(),
                    ColumnFloat = double.MaxValue,
                    ColumnDecimal = 123456789.45m,
                    ColumnDate = DateTime.UtcNow.Date,
                    ColumnTime = DateTime.UtcNow.TimeOfDay,
                    ColumnDateTime = DateTime.Parse("2019-01-01 00:00:05.123", CultureInfo.InvariantCulture),
                    ColumnDateTime2 = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
                };

                // Act
                var result = connection.ExecuteQuery<WhateverClassWithNonNullableProperties>("SELECT @Id AS Id" +
                    ", CONVERT(INT, @ColumnInt) AS ColumnInt" +
                    ", CONVERT(BIGINT, @ColumnBigInt) AS ColumnBigInt" +
                    ", CONVERT(NVARCHAR(MAX), @ColumnNvarChar) AS ColumnNvarChar" +
                    ", CONVERT(FLOAT, @ColumnFloat) AS ColumnFloat" +
                    ", CONVERT(DECIMAL(18,2), @ColumnDecimal) AS ColumnDecimal" +
                    ", CONVERT(DATE, @ColumnDate) AS ColumnDate" +
                    ", CONVERT(TIME, @ColumnTime) AS ColumnTime" +
                    ", CONVERT(DATETIME, @ColumnDateTime) AS ColumnDateTime" +
                    ", CONVERT(DATETIME2(7), @ColumnDateTime2) AS ColumnDateTime2;", param).FirstOrDefault();

                // Assert
                Assert.IsNotNull(result);
                Helper.AssertPropertiesEquality(param, result);
            }
        }

        /// <summary>
        /// Verifies sql connection execute query async whatever class with non nullable properties.
        /// </summary>
        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryAsyncWhateverClassWithNonNullableProperties()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var param = new
                {
                    Id = 1,
                    ColumnInt = int.MaxValue,
                    ColumnBigInt = long.MaxValue,
                    ColumnNVarChar = Helper.GetAssemblyDescription(),
                    ColumnFloat = double.MaxValue,
                    ColumnDecimal = 123456789.45m,
                    ColumnDate = DateTime.UtcNow.Date,
                    ColumnTime = DateTime.UtcNow.TimeOfDay,
                    ColumnDateTime = DateTime.Parse("2019-01-01 00:00:05.123", CultureInfo.InvariantCulture),
                    ColumnDateTime2 = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
                };

                // Act
                var result = (await connection.ExecuteQueryAsync<WhateverClassWithNonNullableProperties>("SELECT @Id AS Id" +
                    ", CONVERT(INT, @ColumnInt) AS ColumnInt" +
                    ", CONVERT(BIGINT, @ColumnBigInt) AS ColumnBigInt" +
                    ", CONVERT(NVARCHAR(MAX), @ColumnNvarChar) AS ColumnNvarChar" +
                    ", CONVERT(FLOAT, @ColumnFloat) AS ColumnFloat" +
                    ", CONVERT(DECIMAL(18,2), @ColumnDecimal) AS ColumnDecimal" +
                    ", CONVERT(DATE, @ColumnDate) AS ColumnDate" +
                    ", CONVERT(TIME, @ColumnTime) AS ColumnTime" +
                    ", CONVERT(DATETIME, @ColumnDateTime) AS ColumnDateTime" +
                    ", CONVERT(DATETIME2(7), @ColumnDateTime2) AS ColumnDateTime2;", param).ConfigureAwait(false)).FirstOrDefault();

                // Assert
                Assert.IsNotNull(result);
                Helper.AssertPropertiesEquality(param, result);
            }
        }

        /// <summary>
        /// Verifies sql connection execute query whatever class with non nullable properties and with extra class properties.
        /// </summary>
        [TestMethod]
        public void TestSqlConnectionExecuteQueryWhateverClassWithNonNullablePropertiesAndWithExtraClassProperties()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var param = new
                {
                    Id = 1,
                    ColumnInt = int.MaxValue,
                    ColumnBigInt = long.MaxValue,
                    ColumnNVarChar = Helper.GetAssemblyDescription(),
                    ColumnFloat = Convert.ToDouble(0),
                    ColumnDecimal = Convert.ToDecimal(0.00),
                    ColumnDate = DateTime.MinValue.Date,
                    ColumnTime = DateTime.MinValue.TimeOfDay,
                    ColumnDateTime = DateTime.MinValue.Date,
                    ColumnDateTime2 = DateTime.MinValue
                };

                // Act
                var result = connection.ExecuteQuery<WhateverClassWithNonNullableProperties>("SELECT @Id AS Id" +
                    ", CONVERT(INT, @ColumnInt) AS ColumnInt" +
                    ", CONVERT(BIGINT, @ColumnBigInt) AS ColumnBigInt" +
                    ", CONVERT(NVARCHAR(MAX), @ColumnNvarChar) AS ColumnNvarChar;", param).FirstOrDefault();

                // Assert
                Assert.IsNotNull(result);
                Helper.AssertPropertiesEquality(param, result);
            }
        }

        /// <summary>
        /// Verifies sql connection execute query async whatever class with non nullable properties and with extra class properties.
        /// </summary>
        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryAsyncWhateverClassWithNonNullablePropertiesAndWithExtraClassProperties()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var param = new
                {
                    Id = 1,
                    ColumnInt = int.MaxValue,
                    ColumnBigInt = long.MaxValue,
                    ColumnNVarChar = Helper.GetAssemblyDescription(),
                    ColumnFloat = Convert.ToDouble(0),
                    ColumnDecimal = Convert.ToDecimal(0.00),
                    ColumnDate = DateTime.MinValue.Date,
                    ColumnTime = DateTime.MinValue.TimeOfDay,
                    ColumnDateTime = DateTime.MinValue.Date,
                    ColumnDateTime2 = DateTime.MinValue
                };

                // Act
                var result = (await connection.ExecuteQueryAsync<WhateverClassWithNonNullableProperties>("SELECT @Id AS Id" +
                    ", CONVERT(INT, @ColumnInt) AS ColumnInt" +
                    ", CONVERT(BIGINT, @ColumnBigInt) AS ColumnBigInt" +
                    ", CONVERT(NVARCHAR(MAX), @ColumnNvarChar) AS ColumnNvarChar;", param).ConfigureAwait(false)).FirstOrDefault();

                // Assert
                Assert.IsNotNull(result);
                Helper.AssertPropertiesEquality(param, result);
            }
        }

        /// <summary>
        /// Verifies sql connection execute query with whatever class with non nullable properties and with extra query properties.
        /// </summary>
        [TestMethod]
        public void TestSqlConnectionExecuteQueryWithWhateverClassWithNonNullablePropertiesAndWithExtraQueryProperties()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var param = new
                {
                    Id = 1,
                    ColumnInt = int.MaxValue,
                    ColumnBigInt = long.MaxValue,
                    ColumnNVarChar = Helper.GetAssemblyDescription(),
                    ColumnFloat = double.MaxValue,
                    ColumnDecimal = 123456789.45m,
                    ColumnDate = DateTime.UtcNow.Date,
                    ColumnTime = DateTime.UtcNow.TimeOfDay,
                    ColumnDateTime = DateTime.Parse("2019-01-01 00:00:05.123", CultureInfo.InvariantCulture),
                    ColumnDateTime2 = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
                };

                // Act
                var result = connection.ExecuteQuery<WhateverClassWithNonNullableProperties>("SELECT @Id AS Id" +
                    ", CONVERT(INT, @ColumnInt) AS ColumnInt" +
                    ", CONVERT(BIGINT, @ColumnBigInt) AS ColumnBigInt" +
                    ", CONVERT(NVARCHAR(MAX), @ColumnNvarChar) AS ColumnNvarChar" +
                    ", CONVERT(FLOAT, @ColumnFloat) AS ColumnFloat" +
                    ", CONVERT(DECIMAL(18,2), @ColumnDecimal) AS ColumnDecimal" +
                    ", CONVERT(DATE, @ColumnDate) AS ColumnDate" +
                    ", CONVERT(TIME, @ColumnTime) AS ColumnTime" +
                    ", CONVERT(DATETIME, @ColumnDateTime) AS ColumnDateTime" +
                    ", CONVERT(DATETIME2(7), @ColumnDateTime2) AS ColumnDateTime2" +
                    ", CONVERT(DATETIME2(5), GETUTCDATE()) AS CurrentDate" +
                    ", CONVERT(NVARCHAR(128), SYSTEM_USER) AS RequestorName;", param).FirstOrDefault();

                // Assert
                Assert.IsNotNull(result);
                Helper.AssertPropertiesEquality(param, result);
            }
        }

        /// <summary>
        /// Verifies sql connection execute query async with whatever class with non nullable properties and with extra query properties.
        /// </summary>
        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryAsyncWithWhateverClassWithNonNullablePropertiesAndWithExtraQueryProperties()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var param = new
                {
                    Id = 1,
                    ColumnInt = int.MaxValue,
                    ColumnBigInt = long.MaxValue,
                    ColumnNVarChar = Helper.GetAssemblyDescription(),
                    ColumnFloat = double.MaxValue,
                    ColumnDecimal = 123456789.45m,
                    ColumnDate = DateTime.UtcNow.Date,
                    ColumnTime = DateTime.UtcNow.TimeOfDay,
                    ColumnDateTime = DateTime.Parse("2019-01-01 00:00:05.123", CultureInfo.InvariantCulture),
                    ColumnDateTime2 = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
                };

                // Act
                var result = (await connection.ExecuteQueryAsync<WhateverClassWithNonNullableProperties>("SELECT @Id AS Id" +
                    ", CONVERT(INT, @ColumnInt) AS ColumnInt" +
                    ", CONVERT(BIGINT, @ColumnBigInt) AS ColumnBigInt" +
                    ", CONVERT(NVARCHAR(MAX), @ColumnNvarChar) AS ColumnNvarChar" +
                    ", CONVERT(FLOAT, @ColumnFloat) AS ColumnFloat" +
                    ", CONVERT(DECIMAL(18,2), @ColumnDecimal) AS ColumnDecimal" +
                    ", CONVERT(DATE, @ColumnDate) AS ColumnDate" +
                    ", CONVERT(TIME, @ColumnTime) AS ColumnTime" +
                    ", CONVERT(DATETIME, @ColumnDateTime) AS ColumnDateTime" +
                    ", CONVERT(DATETIME2(7), @ColumnDateTime2) AS ColumnDateTime2" +
                    ", CONVERT(DATETIME2(5), GETUTCDATE()) AS CurrentDate" +
                    ", CONVERT(NVARCHAR(128), SYSTEM_USER) AS RequestorName;", param).ConfigureAwait(false)).FirstOrDefault();

                // Assert
                Assert.IsNotNull(result);
                Helper.AssertPropertiesEquality(param, result);
            }
        }

        #endregion

        #region Nullables

        /// <summary>
        /// Verifies sql connection execute query whatever class with nullable properties.
        /// </summary>
        [TestMethod]
        public void TestSqlConnectionExecuteQueryWhateverClassWithNullableProperties()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var param = new
                {
                    Id = 1,
                    ColumnInt = (int?)int.MaxValue,
                    ColumnBigInt = (long?)long.MaxValue,
                    ColumnNVarChar = Helper.GetAssemblyDescription(),
                    ColumnFloat = (double?)double.MaxValue,
                    ColumnDecimal = (decimal?)123456789.45m,
                    ColumnDate = (DateTime?)DateTime.UtcNow.Date,
                    ColumnTime = (TimeSpan?)DateTime.UtcNow.TimeOfDay,
                    ColumnDateTime = (DateTime?)DateTime.Parse("2019-01-01 00:00:05.123", CultureInfo.InvariantCulture),
                    ColumnDateTime2 = (DateTime?)DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
                };

                // Act
                var result = connection.ExecuteQuery<WhateverClassWithNullableProperties>("SELECT @Id AS Id" +
                    ", CONVERT(INT, @ColumnInt) AS ColumnInt" +
                    ", CONVERT(BIGINT, @ColumnBigInt) AS ColumnBigInt" +
                    ", CONVERT(NVARCHAR(MAX), @ColumnNvarChar) AS ColumnNvarChar" +
                    ", CONVERT(FLOAT, @ColumnFloat) AS ColumnFloat" +
                    ", CONVERT(DECIMAL(18,2), @ColumnDecimal) AS ColumnDecimal" +
                    ", CONVERT(DATE, @ColumnDate) AS ColumnDate" +
                    ", CONVERT(TIME, @ColumnTime) AS ColumnTime" +
                    ", CONVERT(DATETIME, @ColumnDateTime) AS ColumnDateTime" +
                    ", CONVERT(DATETIME2(7), @ColumnDateTime2) AS ColumnDateTime2;", param).FirstOrDefault();

                // Assert
                Assert.IsNotNull(result);
                Helper.AssertPropertiesEquality(param, result);
            }
        }

        /// <summary>
        /// Verifies sql connection execute query async whatever class with nullable properties.
        /// </summary>
        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryAsyncWhateverClassWithNullableProperties()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var param = new
                {
                    Id = 1,
                    ColumnInt = (int?)int.MaxValue,
                    ColumnBigInt = (long?)long.MaxValue,
                    ColumnNVarChar = Helper.GetAssemblyDescription(),
                    ColumnFloat = (double?)double.MaxValue,
                    ColumnDecimal = (decimal?)123456789.45m,
                    ColumnDate = (DateTime?)DateTime.UtcNow.Date,
                    ColumnTime = (TimeSpan?)DateTime.UtcNow.TimeOfDay,
                    ColumnDateTime = (DateTime?)DateTime.Parse("2019-01-01 00:00:05.123", CultureInfo.InvariantCulture),
                    ColumnDateTime2 = (DateTime?)DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
                };

                // Act
                var result = (await connection.ExecuteQueryAsync<WhateverClassWithNullableProperties>("SELECT @Id AS Id" +
                    ", CONVERT(INT, @ColumnInt) AS ColumnInt" +
                    ", CONVERT(BIGINT, @ColumnBigInt) AS ColumnBigInt" +
                    ", CONVERT(NVARCHAR(MAX), @ColumnNvarChar) AS ColumnNvarChar" +
                    ", CONVERT(FLOAT, @ColumnFloat) AS ColumnFloat" +
                    ", CONVERT(DECIMAL(18,2), @ColumnDecimal) AS ColumnDecimal" +
                    ", CONVERT(DATE, @ColumnDate) AS ColumnDate" +
                    ", CONVERT(TIME, @ColumnTime) AS ColumnTime" +
                    ", CONVERT(DATETIME, @ColumnDateTime) AS ColumnDateTime" +
                    ", CONVERT(DATETIME2(7), @ColumnDateTime2) AS ColumnDateTime2;", param).ConfigureAwait(false)).FirstOrDefault();

                // Assert
                Assert.IsNotNull(result);
                Helper.AssertPropertiesEquality(param, result);
            }
        }

        /// <summary>
        /// Verifies sql connection execute query whatever class with nullable properties and with extra class properties.
        /// </summary>
        [TestMethod]
        public void TestSqlConnectionExecuteQueryWhateverClassWithNullablePropertiesAndWithExtraClassProperties()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var param = new
                {
                    Id = 1,
                    ColumnInt = (int?)int.MaxValue,
                    ColumnBigInt = (long?)long.MaxValue,
                    ColumnNVarChar = Helper.GetAssemblyDescription(),
                    ColumnFloat = (double?)null,
                    ColumnDecimal = (decimal?)null,
                    ColumnDate = (DateTime?)null,
                    ColumnTime = (TimeSpan?)null,
                    ColumnDateTime = (DateTime?)null,
                    ColumnDateTime2 = (DateTime?)null
                };

                // Act
                var result = connection.ExecuteQuery<WhateverClassWithNullableProperties>("SELECT @Id AS Id" +
                    ", CONVERT(INT, @ColumnInt) AS ColumnInt" +
                    ", CONVERT(BIGINT, @ColumnBigInt) AS ColumnBigInt" +
                    ", CONVERT(NVARCHAR(MAX), @ColumnNvarChar) AS ColumnNvarChar;", param).FirstOrDefault();

                // Assert
                Assert.IsNotNull(result);
                Helper.AssertPropertiesEquality(param, result);
            }
        }

        /// <summary>
        /// Verifies sql connection execute query async whatever class with nullable properties and with extra class properties.
        /// </summary>
        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryAsyncWhateverClassWithNullablePropertiesAndWithExtraClassProperties()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var param = new
                {
                    Id = 1,
                    ColumnInt = (int?)int.MaxValue,
                    ColumnBigInt = (long?)long.MaxValue,
                    ColumnNVarChar = Helper.GetAssemblyDescription(),
                    ColumnFloat = (double?)null,
                    ColumnDecimal = (decimal?)null,
                    ColumnDate = (DateTime?)null,
                    ColumnTime = (TimeSpan?)null,
                    ColumnDateTime = (DateTime?)null,
                    ColumnDateTime2 = (DateTime?)null
                };

                // Act
                var result = (await connection.ExecuteQueryAsync<WhateverClassWithNullableProperties>("SELECT @Id AS Id" +
                    ", CONVERT(INT, @ColumnInt) AS ColumnInt" +
                    ", CONVERT(BIGINT, @ColumnBigInt) AS ColumnBigInt" +
                    ", CONVERT(NVARCHAR(MAX), @ColumnNvarChar) AS ColumnNvarChar;", param).ConfigureAwait(false)).FirstOrDefault();

                // Assert
                Assert.IsNotNull(result);
                Helper.AssertPropertiesEquality(param, result);
            }
        }

        /// <summary>
        /// Verifies sql connection execute query with whatever class with nullable properties and with extra query properties.
        /// </summary>
        [TestMethod]
        public void TestSqlConnectionExecuteQueryWithWhateverClassWithNullablePropertiesAndWithExtraQueryProperties()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var param = new
                {
                    Id = 1,
                    ColumnInt = (int?)int.MaxValue,
                    ColumnBigInt = (long?)long.MaxValue,
                    ColumnNVarChar = Helper.GetAssemblyDescription(),
                    ColumnFloat = (double?)double.MaxValue,
                    ColumnDecimal = (decimal?)123456789.45m,
                    ColumnDate = (DateTime?)DateTime.UtcNow.Date,
                    ColumnTime = (TimeSpan?)DateTime.UtcNow.TimeOfDay,
                    ColumnDateTime = (DateTime?)DateTime.Parse("2019-01-01 00:00:05.123", CultureInfo.InvariantCulture),
                    ColumnDateTime2 = (DateTime?)DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
                };

                // Act
                var result = connection.ExecuteQuery<WhateverClassWithNullableProperties>("SELECT @Id AS Id" +
                    ", CONVERT(INT, @ColumnInt) AS ColumnInt" +
                    ", CONVERT(BIGINT, @ColumnBigInt) AS ColumnBigInt" +
                    ", CONVERT(NVARCHAR(MAX), @ColumnNvarChar) AS ColumnNvarChar" +
                    ", CONVERT(FLOAT, @ColumnFloat) AS ColumnFloat" +
                    ", CONVERT(DECIMAL(18,2), @ColumnDecimal) AS ColumnDecimal" +
                    ", CONVERT(DATE, @ColumnDate) AS ColumnDate" +
                    ", CONVERT(TIME, @ColumnTime) AS ColumnTime" +
                    ", CONVERT(DATETIME, @ColumnDateTime) AS ColumnDateTime" +
                    ", CONVERT(DATETIME2(7), @ColumnDateTime2) AS ColumnDateTime2" +
                    ", CONVERT(DATETIME2(5), GETUTCDATE()) AS CurrentDate" +
                    ", CONVERT(NVARCHAR(128), SYSTEM_USER) AS RequestorName;", param).FirstOrDefault();

                // Assert
                Assert.IsNotNull(result);
                Helper.AssertPropertiesEquality(param, result);
            }
        }

        /// <summary>
        /// Verifies sql connection execute query async with whatever class with nullable properties and with extra query properties.
        /// </summary>
        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryAsyncWithWhateverClassWithNullablePropertiesAndWithExtraQueryProperties()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var param = new
                {
                    Id = 1,
                    ColumnInt = (int?)int.MaxValue,
                    ColumnBigInt = (long?)long.MaxValue,
                    ColumnNVarChar = Helper.GetAssemblyDescription(),
                    ColumnFloat = (double?)double.MaxValue,
                    ColumnDecimal = (decimal?)123456789.45m,
                    ColumnDate = (DateTime?)DateTime.UtcNow.Date,
                    ColumnTime = (TimeSpan?)DateTime.UtcNow.TimeOfDay,
                    ColumnDateTime = (DateTime?)DateTime.Parse("2019-01-01 00:00:05.123", CultureInfo.InvariantCulture),
                    ColumnDateTime2 = (DateTime?)DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
                };

                // Act
                var result = (await connection.ExecuteQueryAsync<WhateverClassWithNullableProperties>("SELECT @Id AS Id" +
                    ", CONVERT(INT, @ColumnInt) AS ColumnInt" +
                    ", CONVERT(BIGINT, @ColumnBigInt) AS ColumnBigInt" +
                    ", CONVERT(NVARCHAR(MAX), @ColumnNvarChar) AS ColumnNvarChar" +
                    ", CONVERT(FLOAT, @ColumnFloat) AS ColumnFloat" +
                    ", CONVERT(DECIMAL(18,2), @ColumnDecimal) AS ColumnDecimal" +
                    ", CONVERT(DATE, @ColumnDate) AS ColumnDate" +
                    ", CONVERT(TIME, @ColumnTime) AS ColumnTime" +
                    ", CONVERT(DATETIME, @ColumnDateTime) AS ColumnDateTime" +
                    ", CONVERT(DATETIME2(7), @ColumnDateTime2) AS ColumnDateTime2" +
                    ", CONVERT(DATETIME2(5), GETUTCDATE()) AS CurrentDate" +
                    ", CONVERT(NVARCHAR(128), SYSTEM_USER) AS RequestorName;", param).ConfigureAwait(false)).FirstOrDefault();

                // Assert
                Assert.IsNotNull(result);
                Helper.AssertPropertiesEquality(param, result);
            }
        }

        #endregion

        #endregion

        #region Mapped

        #region NonNullables

        /// <summary>
        /// Verifies sql connection execute query mapped whatever class with non nullable properties.
        /// </summary>
        [TestMethod]
        public void TestSqlConnectionExecuteQueryMappedWhateverClassWithNonNullableProperties()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var param = new
                {
                    IdMapped = 1,
                    ColumnIntMapped = int.MaxValue,
                    ColumnBigIntMapped = long.MaxValue,
                    ColumnNVarCharMapped = Helper.GetAssemblyDescription(),
                    ColumnFloatMapped = double.MaxValue,
                    ColumnDecimalMapped = 123456789.45m,
                    ColumnDateMapped = DateTime.UtcNow.Date,
                    ColumnTimeMapped = DateTime.UtcNow.TimeOfDay,
                    ColumnDateTimeMapped = DateTime.Parse("2019-01-01 00:00:05.123", CultureInfo.InvariantCulture),
                    ColumnDateTime2Mapped = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
                };

                // Act
                var result = connection.ExecuteQuery<MappedWhateverClassWithNonNullableProperties>("SELECT @IdMapped AS Id" +
                    ", CONVERT(INT, @ColumnIntMapped) AS ColumnInt" +
                    ", CONVERT(BIGINT, @ColumnBigIntMapped) AS ColumnBigInt" +
                    ", CONVERT(NVARCHAR(MAX), @ColumnNvarCharMapped) AS ColumnNvarChar" +
                    ", CONVERT(FLOAT, @ColumnFloatMapped) AS ColumnFloat" +
                    ", CONVERT(DECIMAL(18,2), @ColumnDecimalMapped) AS ColumnDecimal" +
                    ", CONVERT(DATE, @ColumnDateMapped) AS ColumnDate" +
                    ", CONVERT(TIME, @ColumnTimeMapped) AS ColumnTime" +
                    ", CONVERT(DATETIME, @ColumnDateTimeMapped) AS ColumnDateTime" +
                    ", CONVERT(DATETIME2(7), @ColumnDateTime2Mapped) AS ColumnDateTime2;", param).FirstOrDefault();

                // Assert
                Assert.IsNotNull(result);
                Helper.AssertPropertiesEquality(param, result);
            }
        }

        /// <summary>
        /// Verifies sql connection execute query async mapped whatever class with non nullable properties.
        /// </summary>
        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryAsyncMappedWhateverClassWithNonNullableProperties()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var param = new
                {
                    IdMapped = 1,
                    ColumnIntMapped = int.MaxValue,
                    ColumnBigIntMapped = long.MaxValue,
                    ColumnNVarCharMapped = Helper.GetAssemblyDescription(),
                    ColumnFloatMapped = double.MaxValue,
                    ColumnDecimalMapped = 123456789.45m,
                    ColumnDateMapped = DateTime.UtcNow.Date,
                    ColumnTimeMapped = DateTime.UtcNow.TimeOfDay,
                    ColumnDateTimeMapped = DateTime.Parse("2019-01-01 00:00:05.123", CultureInfo.InvariantCulture),
                    ColumnDateTime2Mapped = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
                };

                // Act
                var result = (await connection.ExecuteQueryAsync<MappedWhateverClassWithNonNullableProperties>("SELECT @IdMapped AS Id" +
                    ", CONVERT(INT, @ColumnIntMapped) AS ColumnInt" +
                    ", CONVERT(BIGINT, @ColumnBigIntMapped) AS ColumnBigInt" +
                    ", CONVERT(NVARCHAR(MAX), @ColumnNvarCharMapped) AS ColumnNvarChar" +
                    ", CONVERT(FLOAT, @ColumnFloatMapped) AS ColumnFloat" +
                    ", CONVERT(DECIMAL(18,2), @ColumnDecimalMapped) AS ColumnDecimal" +
                    ", CONVERT(DATE, @ColumnDateMapped) AS ColumnDate" +
                    ", CONVERT(TIME, @ColumnTimeMapped) AS ColumnTime" +
                    ", CONVERT(DATETIME, @ColumnDateTimeMapped) AS ColumnDateTime" +
                    ", CONVERT(DATETIME2(7), @ColumnDateTime2Mapped) AS ColumnDateTime2;", param).ConfigureAwait(false)).FirstOrDefault();

                // Assert
                Assert.IsNotNull(result);
                Helper.AssertPropertiesEquality(param, result);
            }
        }

        /// <summary>
        /// Verifies sql connection execute query mapped whatever class with non nullable properties and with extra class properties.
        /// </summary>
        [TestMethod]
        public void TestSqlConnectionExecuteQueryMappedWhateverClassWithNonNullablePropertiesAndWithExtraClassProperties()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var param = new
                {
                    IdMapped = 1,
                    ColumnIntMapped = int.MaxValue,
                    ColumnBigIntMapped = long.MaxValue,
                    ColumnNVarCharMapped = Helper.GetAssemblyDescription(),
                    ColumnFloatMapped = Convert.ToDouble(0),
                    ColumnDecimalMapped = Convert.ToDecimal(0.00),
                    ColumnDateMapped = DateTime.MinValue.Date,
                    ColumnTimeMapped = DateTime.MinValue.TimeOfDay,
                    ColumnDateTimeMapped = DateTime.MinValue.Date,
                    ColumnDateTime2Mapped = DateTime.MinValue
                };

                // Act
                var result = connection.ExecuteQuery<MappedWhateverClassWithNonNullableProperties>("SELECT @IdMapped AS Id" +
                    ", CONVERT(INT, @ColumnIntMapped) AS ColumnInt" +
                    ", CONVERT(BIGINT, @ColumnBigIntMapped) AS ColumnBigInt" +
                    ", CONVERT(NVARCHAR(MAX), @ColumnNvarCharMapped) AS ColumnNvarChar;", param).FirstOrDefault();

                // Assert
                Assert.IsNotNull(result);
                Helper.AssertPropertiesEquality(param, result);
            }
        }

        /// <summary>
        /// Verifies sql connection execute query async mapped whatever class with non nullable properties and with extra class properties.
        /// </summary>
        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryAsyncMappedWhateverClassWithNonNullablePropertiesAndWithExtraClassProperties()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var param = new
                {
                    IdMapped = 1,
                    ColumnIntMapped = int.MaxValue,
                    ColumnBigIntMapped = long.MaxValue,
                    ColumnNVarCharMapped = Helper.GetAssemblyDescription(),
                    ColumnFloatMapped = Convert.ToDouble(0),
                    ColumnDecimalMapped = Convert.ToDecimal(0.00),
                    ColumnDateMapped = DateTime.MinValue.Date,
                    ColumnTimeMapped = DateTime.MinValue.TimeOfDay,
                    ColumnDateTimeMapped = DateTime.MinValue.Date,
                    ColumnDateTime2Mapped = DateTime.MinValue
                };

                // Act
                var result = (await connection.ExecuteQueryAsync<MappedWhateverClassWithNonNullableProperties>("SELECT @IdMapped AS Id" +
                    ", CONVERT(INT, @ColumnIntMapped) AS ColumnInt" +
                    ", CONVERT(BIGINT, @ColumnBigIntMapped) AS ColumnBigInt" +
                    ", CONVERT(NVARCHAR(MAX), @ColumnNvarCharMapped) AS ColumnNvarChar;", param).ConfigureAwait(false)).FirstOrDefault();

                // Assert
                Assert.IsNotNull(result);
                Helper.AssertPropertiesEquality(param, result);
            }
        }

        /// <summary>
        /// Verifies sql connection execute query with mapped whatever class with non nullable properties and with extra query properties.
        /// </summary>
        [TestMethod]
        public void TestSqlConnectionExecuteQueryWithMappedWhateverClassWithNonNullablePropertiesAndWithExtraQueryProperties()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var param = new
                {
                    IdMapped = 1,
                    ColumnIntMapped = int.MaxValue,
                    ColumnBigIntMapped = long.MaxValue,
                    ColumnNVarCharMapped = Helper.GetAssemblyDescription(),
                    ColumnFloatMapped = double.MaxValue,
                    ColumnDecimalMapped = 123456789.45m,
                    ColumnDateMapped = DateTime.UtcNow.Date,
                    ColumnTimeMapped = DateTime.UtcNow.TimeOfDay,
                    ColumnDateTimeMapped = DateTime.Parse("2019-01-01 00:00:05.123", CultureInfo.InvariantCulture),
                    ColumnDateTime2Mapped = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
                };

                // Act
                var result = connection.ExecuteQuery<MappedWhateverClassWithNonNullableProperties>("SELECT @IdMapped AS Id" +
                    ", CONVERT(INT, @ColumnIntMapped) AS ColumnInt" +
                    ", CONVERT(BIGINT, @ColumnBigIntMapped) AS ColumnBigInt" +
                    ", CONVERT(NVARCHAR(MAX), @ColumnNvarCharMapped) AS ColumnNvarChar" +
                    ", CONVERT(FLOAT, @ColumnFloatMapped) AS ColumnFloat" +
                    ", CONVERT(DECIMAL(18,2), @ColumnDecimalMapped) AS ColumnDecimal" +
                    ", CONVERT(DATE, @ColumnDateMapped) AS ColumnDate" +
                    ", CONVERT(TIME, @ColumnTimeMapped) AS ColumnTime" +
                    ", CONVERT(DATETIME, @ColumnDateTimeMapped) AS ColumnDateTime" +
                    ", CONVERT(DATETIME2(7), @ColumnDateTime2Mapped) AS ColumnDateTime2" +
                    ", CONVERT(DATETIME2(5), GETUTCDATE()) AS CurrentDate" +
                    ", CONVERT(NVARCHAR(128), SYSTEM_USER) AS RequestorName;", param).FirstOrDefault();

                // Assert
                Assert.IsNotNull(result);
                Helper.AssertPropertiesEquality(param, result);
            }
        }

        /// <summary>
        /// Verifies sql connection execute query async with mapped whatever class with non nullable properties and with extra query properties.
        /// </summary>
        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryAsyncWithMappedWhateverClassWithNonNullablePropertiesAndWithExtraQueryProperties()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var param = new
                {
                    IdMapped = 1,
                    ColumnIntMapped = int.MaxValue,
                    ColumnBigIntMapped = long.MaxValue,
                    ColumnNVarCharMapped = Helper.GetAssemblyDescription(),
                    ColumnFloatMapped = double.MaxValue,
                    ColumnDecimalMapped = 123456789.45m,
                    ColumnDateMapped = DateTime.UtcNow.Date,
                    ColumnTimeMapped = DateTime.UtcNow.TimeOfDay,
                    ColumnDateTimeMapped = DateTime.Parse("2019-01-01 00:00:05.123", CultureInfo.InvariantCulture),
                    ColumnDateTime2Mapped = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
                };

                // Act
                var result = (await connection.ExecuteQueryAsync<MappedWhateverClassWithNonNullableProperties>("SELECT @IdMapped AS Id" +
                    ", CONVERT(INT, @ColumnIntMapped) AS ColumnInt" +
                    ", CONVERT(BIGINT, @ColumnBigIntMapped) AS ColumnBigInt" +
                    ", CONVERT(NVARCHAR(MAX), @ColumnNvarCharMapped) AS ColumnNvarChar" +
                    ", CONVERT(FLOAT, @ColumnFloatMapped) AS ColumnFloat" +
                    ", CONVERT(DECIMAL(18,2), @ColumnDecimalMapped) AS ColumnDecimal" +
                    ", CONVERT(DATE, @ColumnDateMapped) AS ColumnDate" +
                    ", CONVERT(TIME, @ColumnTimeMapped) AS ColumnTime" +
                    ", CONVERT(DATETIME, @ColumnDateTimeMapped) AS ColumnDateTime" +
                    ", CONVERT(DATETIME2(7), @ColumnDateTime2Mapped) AS ColumnDateTime2" +
                    ", CONVERT(DATETIME2(5), GETUTCDATE()) AS CurrentDate" +
                    ", CONVERT(NVARCHAR(128), SYSTEM_USER) AS RequestorName;", param).ConfigureAwait(false)).FirstOrDefault();

                // Assert
                Assert.IsNotNull(result);
                Helper.AssertPropertiesEquality(param, result);
            }
        }

        #endregion

        #region Nullables

        /// <summary>
        /// Verifies sql connection execute query mapped whatever class with nullable properties.
        /// </summary>
        [TestMethod]
        public void TestSqlConnectionExecuteQueryMappedWhateverClassWithNullableProperties()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var param = new
                {
                    IdMapped = 1,
                    ColumnIntMapped = (int?)int.MaxValue,
                    ColumnBigIntMapped = (long?)long.MaxValue,
                    ColumnNVarCharMapped = Helper.GetAssemblyDescription(),
                    ColumnFloatMapped = (double?)double.MaxValue,
                    ColumnDecimalMapped = (decimal?)123456789.45m,
                    ColumnDateMapped = (DateTime?)DateTime.UtcNow.Date,
                    ColumnTimeMapped = (TimeSpan?)DateTime.UtcNow.TimeOfDay,
                    ColumnDateTimeMapped = (DateTime?)DateTime.Parse("2019-01-01 00:00:05.123", CultureInfo.InvariantCulture),
                    ColumnDateTime2Mapped = (DateTime?)DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
                };

                // Act
                var result = connection.ExecuteQuery<WhateverClassWithNullableProperties>("SELECT @IdMapped AS Id" +
                    ", CONVERT(INT, @ColumnIntMapped) AS ColumnInt" +
                    ", CONVERT(BIGINT, @ColumnBigIntMapped) AS ColumnBigInt" +
                    ", CONVERT(NVARCHAR(MAX), @ColumnNvarCharMapped) AS ColumnNvarChar" +
                    ", CONVERT(FLOAT, @ColumnFloatMapped) AS ColumnFloat" +
                    ", CONVERT(DECIMAL(18,2), @ColumnDecimalMapped) AS ColumnDecimal" +
                    ", CONVERT(DATE, @ColumnDateMapped) AS ColumnDate" +
                    ", CONVERT(TIME, @ColumnTimeMapped) AS ColumnTime" +
                    ", CONVERT(DATETIME, @ColumnDateTimeMapped) AS ColumnDateTime" +
                    ", CONVERT(DATETIME2(7), @ColumnDateTime2Mapped) AS ColumnDateTime2;", param).FirstOrDefault();

                // Assert
                Assert.IsNotNull(result);
                Helper.AssertPropertiesEquality(param, result);
            }
        }

        /// <summary>
        /// Verifies sql connection execute query async mapped whatever class with nullable properties.
        /// </summary>
        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryAsyncMappedWhateverClassWithNullableProperties()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var param = new
                {
                    IdMapped = 1,
                    ColumnIntMapped = (int?)int.MaxValue,
                    ColumnBigIntMapped = (long?)long.MaxValue,
                    ColumnNVarCharMapped = Helper.GetAssemblyDescription(),
                    ColumnFloatMapped = (double?)double.MaxValue,
                    ColumnDecimalMapped = (decimal?)123456789.45m,
                    ColumnDateMapped = (DateTime?)DateTime.UtcNow.Date,
                    ColumnTimeMapped = (TimeSpan?)DateTime.UtcNow.TimeOfDay,
                    ColumnDateTimeMapped = (DateTime?)DateTime.Parse("2019-01-01 00:00:05.123", CultureInfo.InvariantCulture),
                    ColumnDateTime2Mapped = (DateTime?)DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
                };

                // Act
                var result = (await connection.ExecuteQueryAsync<WhateverClassWithNullableProperties>("SELECT @IdMapped AS Id" +
                    ", CONVERT(INT, @ColumnIntMapped) AS ColumnInt" +
                    ", CONVERT(BIGINT, @ColumnBigIntMapped) AS ColumnBigInt" +
                    ", CONVERT(NVARCHAR(MAX), @ColumnNvarCharMapped) AS ColumnNvarChar" +
                    ", CONVERT(FLOAT, @ColumnFloatMapped) AS ColumnFloat" +
                    ", CONVERT(DECIMAL(18,2), @ColumnDecimalMapped) AS ColumnDecimal" +
                    ", CONVERT(DATE, @ColumnDateMapped) AS ColumnDate" +
                    ", CONVERT(TIME, @ColumnTimeMapped) AS ColumnTime" +
                    ", CONVERT(DATETIME, @ColumnDateTimeMapped) AS ColumnDateTime" +
                    ", CONVERT(DATETIME2(7), @ColumnDateTime2Mapped) AS ColumnDateTime2;", param).ConfigureAwait(false)).FirstOrDefault();

                // Assert
                Assert.IsNotNull(result);
                Helper.AssertPropertiesEquality(param, result);
            }
        }

        /// <summary>
        /// Verifies sql connection execute query mapped whatever class with nullable properties and with extra class properties.
        /// </summary>
        [TestMethod]
        public void TestSqlConnectionExecuteQueryMappedWhateverClassWithNullablePropertiesAndWithExtraClassProperties()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var param = new
                {
                    IdMapped = 1,
                    ColumnIntMapped = (int?)int.MaxValue,
                    ColumnBigIntMapped = (long?)long.MaxValue,
                    ColumnNVarCharMapped = Helper.GetAssemblyDescription(),
                    ColumnFloatMapped = (double?)null,
                    ColumnDecimalMapped = (decimal?)null,
                    ColumnDateMapped = (DateTime?)null,
                    ColumnTimeMapped = (TimeSpan?)null,
                    ColumnDateTimeMapped = (DateTime?)null,
                    ColumnDateTime2Mapped = (DateTime?)null
                };

                // Act
                var result = connection.ExecuteQuery<MappedWhateverClassWithNullableProperties>("SELECT @IdMapped AS Id" +
                    ", CONVERT(INT, @ColumnIntMapped) AS ColumnInt" +
                    ", CONVERT(BIGINT, @ColumnBigIntMapped) AS ColumnBigInt" +
                    ", CONVERT(NVARCHAR(MAX), @ColumnNvarCharMapped) AS ColumnNvarChar;", param).FirstOrDefault();

                // Assert
                Assert.IsNotNull(result);
                Helper.AssertPropertiesEquality(param, result);
            }
        }

        /// <summary>
        /// Verifies sql connection execute query async mapped whatever class with nullable properties and with extra class properties.
        /// </summary>
        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryAsyncMappedWhateverClassWithNullablePropertiesAndWithExtraClassProperties()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var param = new
                {
                    IdMapped = 1,
                    ColumnIntMapped = (int?)int.MaxValue,
                    ColumnBigIntMapped = (long?)long.MaxValue,
                    ColumnNVarCharMapped = Helper.GetAssemblyDescription(),
                    ColumnFloatMapped = (double?)null,
                    ColumnDecimalMapped = (decimal?)null,
                    ColumnDateMapped = (DateTime?)null,
                    ColumnTimeMapped = (TimeSpan?)null,
                    ColumnDateTimeMapped = (DateTime?)null,
                    ColumnDateTime2Mapped = (DateTime?)null
                };

                // Act
                var result = (await connection.ExecuteQueryAsync<MappedWhateverClassWithNullableProperties>("SELECT @IdMapped AS Id" +
                    ", CONVERT(INT, @ColumnIntMapped) AS ColumnInt" +
                    ", CONVERT(BIGINT, @ColumnBigIntMapped) AS ColumnBigInt" +
                    ", CONVERT(NVARCHAR(MAX), @ColumnNvarCharMapped) AS ColumnNvarChar;", param).ConfigureAwait(false)).FirstOrDefault();

                // Assert
                Assert.IsNotNull(result);
                Helper.AssertPropertiesEquality(param, result);
            }
        }

        /// <summary>
        /// Verifies sql connection execute query with mapped whatever class with nullable properties and with extra query properties.
        /// </summary>
        [TestMethod]
        public void TestSqlConnectionExecuteQueryWithMappedWhateverClassWithNullablePropertiesAndWithExtraQueryProperties()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var param = new
                {
                    IdMapped = 1,
                    ColumnIntMapped = (int?)int.MaxValue,
                    ColumnBigIntMapped = (long?)long.MaxValue,
                    ColumnNVarCharMapped = Helper.GetAssemblyDescription(),
                    ColumnFloatMapped = (double?)double.MaxValue,
                    ColumnDecimalMapped = (decimal?)123456789.45m,
                    ColumnDateMapped = (DateTime?)DateTime.UtcNow.Date,
                    ColumnTimeMapped = (TimeSpan?)DateTime.UtcNow.TimeOfDay,
                    ColumnDateTimeMapped = (DateTime?)DateTime.Parse("2019-01-01 00:00:05.123", CultureInfo.InvariantCulture),
                    ColumnDateTime2Mapped = (DateTime?)DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
                };

                // Act
                var result = connection.ExecuteQuery<MappedWhateverClassWithNullableProperties>("SELECT @IdMapped AS Id" +
                    ", CONVERT(INT, @ColumnIntMapped) AS ColumnInt" +
                    ", CONVERT(BIGINT, @ColumnBigIntMapped) AS ColumnBigInt" +
                    ", CONVERT(NVARCHAR(MAX), @ColumnNvarCharMapped) AS ColumnNvarChar" +
                    ", CONVERT(FLOAT, @ColumnFloatMapped) AS ColumnFloat" +
                    ", CONVERT(DECIMAL(18,2), @ColumnDecimalMapped) AS ColumnDecimal" +
                    ", CONVERT(DATE, @ColumnDateMapped) AS ColumnDate" +
                    ", CONVERT(TIME, @ColumnTimeMapped) AS ColumnTime" +
                    ", CONVERT(DATETIME, @ColumnDateTimeMapped) AS ColumnDateTime" +
                    ", CONVERT(DATETIME2(7), @ColumnDateTime2Mapped) AS ColumnDateTime2" +
                    ", CONVERT(DATETIME2(5), GETUTCDATE()) AS CurrentDate" +
                    ", CONVERT(NVARCHAR(128), SYSTEM_USER) AS RequestorName;", param).FirstOrDefault();

                // Assert
                Assert.IsNotNull(result);
                Helper.AssertPropertiesEquality(param, result);
            }
        }

        /// <summary>
        /// Verifies sql connection execute query async with mapped whatever class with nullable properties and with extra query properties.
        /// </summary>
        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryAsyncWithMappedWhateverClassWithNullablePropertiesAndWithExtraQueryProperties()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var param = new
                {
                    IdMapped = 1,
                    ColumnIntMapped = (int?)int.MaxValue,
                    ColumnBigIntMapped = (long?)long.MaxValue,
                    ColumnNVarCharMapped = Helper.GetAssemblyDescription(),
                    ColumnFloatMapped = (double?)double.MaxValue,
                    ColumnDecimalMapped = (decimal?)123456789.45m,
                    ColumnDateMapped = (DateTime?)DateTime.UtcNow.Date,
                    ColumnTimeMapped = (TimeSpan?)DateTime.UtcNow.TimeOfDay,
                    ColumnDateTimeMapped = (DateTime?)DateTime.Parse("2019-01-01 00:00:05.123", CultureInfo.InvariantCulture),
                    ColumnDateTime2Mapped = (DateTime?)DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
                };

                // Act
                var result = (await connection.ExecuteQueryAsync<MappedWhateverClassWithNullableProperties>("SELECT @IdMapped AS Id" +
                    ", CONVERT(INT, @ColumnIntMapped) AS ColumnInt" +
                    ", CONVERT(BIGINT, @ColumnBigIntMapped) AS ColumnBigInt" +
                    ", CONVERT(NVARCHAR(MAX), @ColumnNvarCharMapped) AS ColumnNvarChar" +
                    ", CONVERT(FLOAT, @ColumnFloatMapped) AS ColumnFloat" +
                    ", CONVERT(DECIMAL(18,2), @ColumnDecimalMapped) AS ColumnDecimal" +
                    ", CONVERT(DATE, @ColumnDateMapped) AS ColumnDate" +
                    ", CONVERT(TIME, @ColumnTimeMapped) AS ColumnTime" +
                    ", CONVERT(DATETIME, @ColumnDateTimeMapped) AS ColumnDateTime" +
                    ", CONVERT(DATETIME2(7), @ColumnDateTime2Mapped) AS ColumnDateTime2" +
                    ", CONVERT(DATETIME2(5), GETUTCDATE()) AS CurrentDate" +
                    ", CONVERT(NVARCHAR(128), SYSTEM_USER) AS RequestorName;", param).ConfigureAwait(false)).FirstOrDefault();

                // Assert
                Assert.IsNotNull(result);
                Helper.AssertPropertiesEquality(param, result);
            }
        }

        #endregion

        #endregion


#if NET5_0_OR_GREATER

        #region Records

        [TestMethod]
        public void TestSqlConnectionExecuteQueryWithStringRecordParam()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteQuery<string>("WITH CTE AS (SELECT 'ABC' AS Value UNION ALL SELECT 'DEF') SELECT * FROM CTE WHERE Value = @Value;",
                    new { Value = new StringRecord("ABC") }).AsList();

                // Assert
                Assert.AreEqual(1, result.Count);
                Assert.AreEqual("ABC", result[0], StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryAsyncWithStringRecordParam()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = (await connection.ExecuteQueryAsync<string>("WITH CTE AS (SELECT 'ABC' AS Value UNION ALL SELECT 'DEF') SELECT * FROM CTE WHERE Value = @Value;",
                    new { Value = new StringRecord("ABC") })).AsList();

                // Assert
                Assert.AreEqual(1, result.Count);
                Assert.AreEqual("ABC", result[0], StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryWithMultipleStringRecordParams()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteQuery<string>("WITH CTE AS (SELECT 'ABC' AS Value UNION ALL SELECT 'DEF' UNION ALL SELECT 'GHI') SELECT * FROM CTE WHERE Value IN (@Values);",
                    new { Values = new StringRecord[] { "ABC", "DEF", "GHI" } }).AsList();

                // Assert
                Assert.AreEqual(3, result.Count);
                Assert.AreEqual("ABC", result[0], StringComparer.Ordinal);
                Assert.AreEqual("DEF", result[1], StringComparer.Ordinal);
                Assert.AreEqual("GHI", result[2], StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryAsyncWithMultipleStringRecordParams()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = (await connection.ExecuteQueryAsync<string>("WITH CTE AS (SELECT 'ABC' AS Value UNION ALL SELECT 'DEF' UNION ALL SELECT 'GHI') SELECT * FROM CTE WHERE Value IN (@Values);",
                    new { Values = new StringRecord[] { "ABC", "DEF", "GHI" } })).AsList();

                // Assert
                Assert.AreEqual(3, result.Count);
                Assert.AreEqual("ABC", result[0], StringComparer.Ordinal);
                Assert.AreEqual("DEF", result[1], StringComparer.Ordinal);
                Assert.AreEqual("GHI", result[2], StringComparer.Ordinal);
            }
        }

        #endregion

#endif

        #region IDbDataParameter

        #region Sync

        /// <summary>
        /// Verifies sql connection execute query via idb data parameter.
        /// </summary>
        [TestMethod]
        public void TestSqlConnectionExecuteQueryViaIDbDataParameter()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteQuery<int>("SELECT 1 * @Value;",
                    new { Value = new SqlParameter("_", 100) }).FirstOrDefault();

                // Assert
                Assert.AreEqual(100, result);
            }
        }

        /// <summary>
        /// Verifies sql connection execute query via query field for idb data parameter.
        /// </summary>
        [TestMethod]
        public void TestSqlConnectionExecuteQueryViaQueryFieldForIDbDataParameter()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteQuery<int>("SELECT 1 * @Value;",
                    new QueryField("Value", new SqlParameter("_", 100))).FirstOrDefault();

                // Assert
                Assert.AreEqual(100, result);
            }
        }

        /// <summary>
        /// Verifies sql connection execute query via query fields for idb data parameter.
        /// </summary>
        [TestMethod]
        public void TestSqlConnectionExecuteQueryViaQueryFieldsForIDbDataParameter()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteQuery<int>("SELECT 1 * @Value;",
                    new QueryField("Value", new SqlParameter("_", 100)).AsEnumerable()).FirstOrDefault();

                // Assert
                Assert.AreEqual(100, result);
            }
        }

        /// <summary>
        /// Verifies sql connection execute query via query groups for idb data parameter.
        /// </summary>
        [TestMethod]
        public void TestSqlConnectionExecuteQueryViaQueryGroupsForIDbDataParameter()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.ExecuteQuery<int>("SELECT 1 * @Value;",
                    new QueryGroup(new QueryField("Value", new SqlParameter("_", 100)))).FirstOrDefault();

                // Assert
                Assert.AreEqual(100, result);
            }
        }

        #endregion

        #region Async

        /// <summary>
        /// Verifies sql connection execute query async via idb data parameter.
        /// </summary>
        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryAsyncViaIDbDataParameter()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = (await connection.ExecuteQueryAsync<int>("SELECT 1 * @Value;",
                    new { Value = new SqlParameter("_", 100) }).ConfigureAwait(false)).FirstOrDefault();

                // Assert
                Assert.AreEqual(100, result);
            }
        }

        /// <summary>
        /// Verifies sql connection execute query async via query field for idb data parameter.
        /// </summary>
        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryAsyncViaQueryFieldForIDbDataParameter()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = (await connection.ExecuteQueryAsync<int>("SELECT 1 * @Value;",
                    new QueryField("Value", new SqlParameter("_", 100))).ConfigureAwait(false)).FirstOrDefault();

                // Assert
                Assert.AreEqual(100, result);
            }
        }

        /// <summary>
        /// Verifies sql connection execute query async via query fields for idb data parameter.
        /// </summary>
        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryAsyncViaQueryFieldsForIDbDataParameter()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = (await connection.ExecuteQueryAsync<int>("SELECT 1 * @Value;",
                    new QueryField("Value", new SqlParameter("_", 100)).AsEnumerable()).ConfigureAwait(false)).FirstOrDefault();

                // Assert
                Assert.AreEqual(100, result);
            }
        }

        /// <summary>
        /// Verifies sql connection execute query async via query groups for idb data parameter.
        /// </summary>
        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryAsyncViaQueryGroupsForIDbDataParameter()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = (await connection.ExecuteQueryAsync<int>("SELECT 1 * @Value;",
                    new QueryGroup(new QueryField("Value", new SqlParameter("_", 100)))).ConfigureAwait(false)).FirstOrDefault();

                // Assert
                Assert.AreEqual(100, result);
            }
        }

        #endregion

        #endregion

        #endregion

        #region ExecuteQueryMultiple

        #region NonMapped

        #region NonNullables

        /// <summary>
        /// Verifies sql connection execute query multiple whatever class with non nullable properties.
        /// </summary>
        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleWhateverClassWithNonNullableProperties()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var param = new
                {
                    Id = 1,
                    ColumnInt = int.MaxValue,
                    ColumnBigInt = long.MaxValue,
                    ColumnNVarChar = Helper.GetAssemblyDescription(),
                    ColumnFloat = double.MaxValue,
                    ColumnDecimal = 123456789.45m,
                    ColumnDate = DateTime.UtcNow.Date,
                    ColumnTime = DateTime.UtcNow.TimeOfDay,
                    ColumnDateTime = DateTime.Parse("2019-01-01 00:00:05.123", CultureInfo.InvariantCulture),
                    ColumnDateTime2 = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
                };

                // Act
                var extractor = connection.ExecuteQueryMultiple("SELECT @Id AS Id" +
                    ", CONVERT(INT, @ColumnInt) AS ColumnInt" +
                    ", CONVERT(BIGINT, @ColumnBigInt) AS ColumnBigInt" +
                    ", CONVERT(NVARCHAR(MAX), @ColumnNvarChar) AS ColumnNvarChar" +
                    ", CONVERT(FLOAT, @ColumnFloat) AS ColumnFloat" +
                    ", CONVERT(DECIMAL(18,2), @ColumnDecimal) AS ColumnDecimal" +
                    ", CONVERT(DATE, @ColumnDate) AS ColumnDate" +
                    ", CONVERT(TIME, @ColumnTime) AS ColumnTime" +
                    ", CONVERT(DATETIME, @ColumnDateTime) AS ColumnDateTime" +
                    ", CONVERT(DATETIME2(7), @ColumnDateTime2) AS ColumnDateTime2; " +
                    "SELECT @Id AS Id" +
                    ", CONVERT(INT, @ColumnInt) AS ColumnInt" +
                    ", CONVERT(BIGINT, @ColumnBigInt) AS ColumnBigInt" +
                    ", CONVERT(NVARCHAR(MAX), @ColumnNvarChar) AS ColumnNvarChar" +
                    ", CONVERT(FLOAT, @ColumnFloat) AS ColumnFloat" +
                    ", CONVERT(DECIMAL(18,2), @ColumnDecimal) AS ColumnDecimal" +
                    ", CONVERT(DATE, @ColumnDate) AS ColumnDate" +
                    ", CONVERT(TIME, @ColumnTime) AS ColumnTime" +
                    ", CONVERT(DATETIME, @ColumnDateTime) AS ColumnDateTime" +
                    ", CONVERT(DATETIME2(7), @ColumnDateTime2) AS ColumnDateTime2;", param);
                var firstResult = extractor.Extract<WhateverClassWithNonNullableProperties>();
                var secondResult = extractor.Extract<WhateverClassWithNonNullableProperties>();

                // Assert
                Assert.IsNotNull(firstResult);
                Assert.IsNotNull(secondResult);
                Helper.AssertPropertiesEquality(param, firstResult);
                Helper.AssertPropertiesEquality(param, secondResult);
            }
        }

        /// <summary>
        /// Verifies sql connection execute query multiple async whatever class with non nullable properties.
        /// </summary>
        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryMultipleAsyncWhateverClassWithNonNullableProperties()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var param = new
                {
                    Id = 1,
                    ColumnInt = int.MaxValue,
                    ColumnBigInt = long.MaxValue,
                    ColumnNVarChar = Helper.GetAssemblyDescription(),
                    ColumnFloat = double.MaxValue,
                    ColumnDecimal = 123456789.45m,
                    ColumnDate = DateTime.UtcNow.Date,
                    ColumnTime = DateTime.UtcNow.TimeOfDay,
                    ColumnDateTime = DateTime.Parse("2019-01-01 00:00:05.123", CultureInfo.InvariantCulture),
                    ColumnDateTime2 = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
                };

                // Act
                var extractor = await connection.ExecuteQueryMultipleAsync("SELECT @Id AS Id" +
                    ", CONVERT(INT, @ColumnInt) AS ColumnInt" +
                    ", CONVERT(BIGINT, @ColumnBigInt) AS ColumnBigInt" +
                    ", CONVERT(NVARCHAR(MAX), @ColumnNvarChar) AS ColumnNvarChar" +
                    ", CONVERT(FLOAT, @ColumnFloat) AS ColumnFloat" +
                    ", CONVERT(DECIMAL(18,2), @ColumnDecimal) AS ColumnDecimal" +
                    ", CONVERT(DATE, @ColumnDate) AS ColumnDate" +
                    ", CONVERT(TIME, @ColumnTime) AS ColumnTime" +
                    ", CONVERT(DATETIME, @ColumnDateTime) AS ColumnDateTime" +
                    ", CONVERT(DATETIME2(7), @ColumnDateTime2) AS ColumnDateTime2; " +
                    "SELECT @Id AS Id" +
                    ", CONVERT(INT, @ColumnInt) AS ColumnInt" +
                    ", CONVERT(BIGINT, @ColumnBigInt) AS ColumnBigInt" +
                    ", CONVERT(NVARCHAR(MAX), @ColumnNvarChar) AS ColumnNvarChar" +
                    ", CONVERT(FLOAT, @ColumnFloat) AS ColumnFloat" +
                    ", CONVERT(DECIMAL(18,2), @ColumnDecimal) AS ColumnDecimal" +
                    ", CONVERT(DATE, @ColumnDate) AS ColumnDate" +
                    ", CONVERT(TIME, @ColumnTime) AS ColumnTime" +
                    ", CONVERT(DATETIME, @ColumnDateTime) AS ColumnDateTime" +
                    ", CONVERT(DATETIME2(7), @ColumnDateTime2) AS ColumnDateTime2;", param).ConfigureAwait(false);
                var firstResult = extractor.Extract<WhateverClassWithNonNullableProperties>();
                var secondResult = extractor.Extract<WhateverClassWithNonNullableProperties>();

                // Assert
                Assert.IsNotNull(firstResult);
                Assert.IsNotNull(secondResult);
                Helper.AssertPropertiesEquality(param, firstResult);
                Helper.AssertPropertiesEquality(param, secondResult);
            }
        }

        /// <summary>
        /// Verifies sql connection execute query multiple whatever class with non nullable properties and with extra class properties.
        /// </summary>
        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleWhateverClassWithNonNullablePropertiesAndWithExtraClassProperties()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var param = new
                {
                    Id = 1,
                    ColumnInt = int.MaxValue,
                    ColumnBigInt = long.MaxValue,
                    ColumnNVarChar = Helper.GetAssemblyDescription(),
                    ColumnFloat = Convert.ToDouble(0),
                    ColumnDecimal = Convert.ToDecimal(0.00),
                    ColumnDate = DateTime.MinValue.Date,
                    ColumnTime = DateTime.MinValue.TimeOfDay,
                    ColumnDateTime = DateTime.MinValue.Date,
                    ColumnDateTime2 = DateTime.MinValue
                };

                // Act
                var extractor = connection.ExecuteQueryMultiple("SELECT @Id AS Id" +
                    ", CONVERT(INT, @ColumnInt) AS ColumnInt" +
                    ", CONVERT(BIGINT, @ColumnBigInt) AS ColumnBigInt" +
                    ", CONVERT(NVARCHAR(MAX), @ColumnNvarChar) AS ColumnNvarChar; " +
                    "SELECT @Id AS Id" +
                    ", CONVERT(INT, @ColumnInt) AS ColumnInt" +
                    ", CONVERT(BIGINT, @ColumnBigInt) AS ColumnBigInt" +
                    ", CONVERT(NVARCHAR(MAX), @ColumnNvarChar) AS ColumnNvarChar;", param);
                var firstResult = extractor.Extract<WhateverClassWithNonNullableProperties>();
                var secondResult = extractor.Extract<WhateverClassWithNonNullableProperties>();

                // Assert
                Assert.IsNotNull(firstResult);
                Assert.IsNotNull(secondResult);
                Helper.AssertPropertiesEquality(param, firstResult);
                Helper.AssertPropertiesEquality(param, secondResult);
            }
        }

        /// <summary>
        /// Verifies sql connection execute query multiple async whatever class with non nullable properties and with extra class properties.
        /// </summary>
        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryMultipleAsyncWhateverClassWithNonNullablePropertiesAndWithExtraClassProperties()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var param = new
                {
                    Id = 1,
                    ColumnInt = int.MaxValue,
                    ColumnBigInt = long.MaxValue,
                    ColumnNVarChar = Helper.GetAssemblyDescription(),
                    ColumnFloat = Convert.ToDouble(0),
                    ColumnDecimal = Convert.ToDecimal(0.00),
                    ColumnDate = DateTime.MinValue.Date,
                    ColumnTime = DateTime.MinValue.TimeOfDay,
                    ColumnDateTime = DateTime.MinValue.Date,
                    ColumnDateTime2 = DateTime.MinValue
                };

                // Act
                var extractor = await connection.ExecuteQueryMultipleAsync("SELECT @Id AS Id" +
                    ", CONVERT(INT, @ColumnInt) AS ColumnInt" +
                    ", CONVERT(BIGINT, @ColumnBigInt) AS ColumnBigInt" +
                    ", CONVERT(NVARCHAR(MAX), @ColumnNvarChar) AS ColumnNvarChar; " +
                    "SELECT @Id AS Id" +
                    ", CONVERT(INT, @ColumnInt) AS ColumnInt" +
                    ", CONVERT(BIGINT, @ColumnBigInt) AS ColumnBigInt" +
                    ", CONVERT(NVARCHAR(MAX), @ColumnNvarChar) AS ColumnNvarChar;", param).ConfigureAwait(false);
                var firstResult = extractor.Extract<WhateverClassWithNonNullableProperties>();
                var secondResult = extractor.Extract<WhateverClassWithNonNullableProperties>();

                // Assert
                Assert.IsNotNull(firstResult);
                Assert.IsNotNull(secondResult);
                Helper.AssertPropertiesEquality(param, firstResult);
                Helper.AssertPropertiesEquality(param, secondResult);
            }
        }

        /// <summary>
        /// Verifies sql connection execute query multiple with whatever class with non nullable properties and with extra query properties.
        /// </summary>
        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleWithWhateverClassWithNonNullablePropertiesAndWithExtraQueryProperties()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var param = new
                {
                    Id = 1,
                    ColumnInt = int.MaxValue,
                    ColumnBigInt = long.MaxValue,
                    ColumnNVarChar = Helper.GetAssemblyDescription(),
                    ColumnFloat = double.MaxValue,
                    ColumnDecimal = 123456789.45m,
                    ColumnDate = DateTime.UtcNow.Date,
                    ColumnTime = DateTime.UtcNow.TimeOfDay,
                    ColumnDateTime = DateTime.Parse("2019-01-01 00:00:05.123", CultureInfo.InvariantCulture),
                    ColumnDateTime2 = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
                };

                // Act
                var extractor = connection.ExecuteQueryMultiple("SELECT @Id AS Id" +
                    ", CONVERT(INT, @ColumnInt) AS ColumnInt" +
                    ", CONVERT(BIGINT, @ColumnBigInt) AS ColumnBigInt" +
                    ", CONVERT(NVARCHAR(MAX), @ColumnNvarChar) AS ColumnNvarChar" +
                    ", CONVERT(FLOAT, @ColumnFloat) AS ColumnFloat" +
                    ", CONVERT(DECIMAL(18,2), @ColumnDecimal) AS ColumnDecimal" +
                    ", CONVERT(DATE, @ColumnDate) AS ColumnDate" +
                    ", CONVERT(TIME, @ColumnTime) AS ColumnTime" +
                    ", CONVERT(DATETIME, @ColumnDateTime) AS ColumnDateTime" +
                    ", CONVERT(DATETIME2(7), @ColumnDateTime2) AS ColumnDateTime2" +
                    ", CONVERT(DATETIME2(5), GETUTCDATE()) AS CurrentDate" +
                    ", CONVERT(NVARCHAR(128), SYSTEM_USER) AS RequestorName; SELECT @Id AS Id" +
                    ", CONVERT(INT, @ColumnInt) AS ColumnInt" +
                    ", CONVERT(BIGINT, @ColumnBigInt) AS ColumnBigInt" +
                    ", CONVERT(NVARCHAR(MAX), @ColumnNvarChar) AS ColumnNvarChar" +
                    ", CONVERT(FLOAT, @ColumnFloat) AS ColumnFloat" +
                    ", CONVERT(DECIMAL(18,2), @ColumnDecimal) AS ColumnDecimal" +
                    ", CONVERT(DATE, @ColumnDate) AS ColumnDate" +
                    ", CONVERT(TIME, @ColumnTime) AS ColumnTime" +
                    ", CONVERT(DATETIME, @ColumnDateTime) AS ColumnDateTime" +
                    ", CONVERT(DATETIME2(7), @ColumnDateTime2) AS ColumnDateTime2" +
                    ", CONVERT(DATETIME2(5), GETUTCDATE()) AS CurrentDate" +
                    ", CONVERT(NVARCHAR(128), SYSTEM_USER) AS RequestorName;", param);
                var firstResult = extractor.Extract<WhateverClassWithNonNullableProperties>();
                var secondResult = extractor.Extract<WhateverClassWithNonNullableProperties>();

                // Assert
                Assert.IsNotNull(firstResult);
                Assert.IsNotNull(secondResult);
                Helper.AssertPropertiesEquality(param, firstResult);
                Helper.AssertPropertiesEquality(param, secondResult);
            }
        }

        /// <summary>
        /// Verifies sql connection execute query multiple async with whatever class with non nullable properties and with extra query properties.
        /// </summary>
        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryMultipleAsyncWithWhateverClassWithNonNullablePropertiesAndWithExtraQueryProperties()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var param = new
                {
                    Id = 1,
                    ColumnInt = int.MaxValue,
                    ColumnBigInt = long.MaxValue,
                    ColumnNVarChar = Helper.GetAssemblyDescription(),
                    ColumnFloat = double.MaxValue,
                    ColumnDecimal = 123456789.45m,
                    ColumnDate = DateTime.UtcNow.Date,
                    ColumnTime = DateTime.UtcNow.TimeOfDay,
                    ColumnDateTime = DateTime.Parse("2019-01-01 00:00:05.123", CultureInfo.InvariantCulture),
                    ColumnDateTime2 = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
                };

                // Act
                var extractor = await connection.ExecuteQueryMultipleAsync("SELECT @Id AS Id" +
                    ", CONVERT(INT, @ColumnInt) AS ColumnInt" +
                    ", CONVERT(BIGINT, @ColumnBigInt) AS ColumnBigInt" +
                    ", CONVERT(NVARCHAR(MAX), @ColumnNvarChar) AS ColumnNvarChar" +
                    ", CONVERT(FLOAT, @ColumnFloat) AS ColumnFloat" +
                    ", CONVERT(DECIMAL(18,2), @ColumnDecimal) AS ColumnDecimal" +
                    ", CONVERT(DATE, @ColumnDate) AS ColumnDate" +
                    ", CONVERT(TIME, @ColumnTime) AS ColumnTime" +
                    ", CONVERT(DATETIME, @ColumnDateTime) AS ColumnDateTime" +
                    ", CONVERT(DATETIME2(7), @ColumnDateTime2) AS ColumnDateTime2" +
                    ", CONVERT(DATETIME2(5), GETUTCDATE()) AS CurrentDate" +
                    ", CONVERT(NVARCHAR(128), SYSTEM_USER) AS RequestorName; SELECT @Id AS Id" +
                    ", CONVERT(INT, @ColumnInt) AS ColumnInt" +
                    ", CONVERT(BIGINT, @ColumnBigInt) AS ColumnBigInt" +
                    ", CONVERT(NVARCHAR(MAX), @ColumnNvarChar) AS ColumnNvarChar" +
                    ", CONVERT(FLOAT, @ColumnFloat) AS ColumnFloat" +
                    ", CONVERT(DECIMAL(18,2), @ColumnDecimal) AS ColumnDecimal" +
                    ", CONVERT(DATE, @ColumnDate) AS ColumnDate" +
                    ", CONVERT(TIME, @ColumnTime) AS ColumnTime" +
                    ", CONVERT(DATETIME, @ColumnDateTime) AS ColumnDateTime" +
                    ", CONVERT(DATETIME2(7), @ColumnDateTime2) AS ColumnDateTime2" +
                    ", CONVERT(DATETIME2(5), GETUTCDATE()) AS CurrentDate" +
                    ", CONVERT(NVARCHAR(128), SYSTEM_USER) AS RequestorName;", param).ConfigureAwait(false);
                var firstResult = extractor.Extract<WhateverClassWithNonNullableProperties>();
                var secondResult = extractor.Extract<WhateverClassWithNonNullableProperties>();

                // Assert
                Assert.IsNotNull(firstResult);
                Assert.IsNotNull(secondResult);
                Helper.AssertPropertiesEquality(param, firstResult);
                Helper.AssertPropertiesEquality(param, secondResult);
            }
        }

        #endregion

        #region Nullables

        /// <summary>
        /// Verifies sql connection execute query multiple whatever class with nullable properties.
        /// </summary>
        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleWhateverClassWithNullableProperties()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var param = new
                {
                    Id = 1,
                    ColumnInt = (int?)int.MaxValue,
                    ColumnBigInt = (long?)long.MaxValue,
                    ColumnNVarChar = Helper.GetAssemblyDescription(),
                    ColumnFloat = (double?)double.MaxValue,
                    ColumnDecimal = (decimal?)123456789.45m,
                    ColumnDate = (DateTime?)DateTime.UtcNow.Date,
                    ColumnTime = (TimeSpan?)DateTime.UtcNow.TimeOfDay,
                    ColumnDateTime = (DateTime?)DateTime.Parse("2019-01-01 00:00:05.123", CultureInfo.InvariantCulture),
                    ColumnDateTime2 = (DateTime?)DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
                };

                // Act
                var extractor = connection.ExecuteQueryMultiple("SELECT @Id AS Id" +
                    ", CONVERT(INT, @ColumnInt) AS ColumnInt" +
                    ", CONVERT(BIGINT, @ColumnBigInt) AS ColumnBigInt" +
                    ", CONVERT(NVARCHAR(MAX), @ColumnNvarChar) AS ColumnNvarChar" +
                    ", CONVERT(FLOAT, @ColumnFloat) AS ColumnFloat" +
                    ", CONVERT(DECIMAL(18,2), @ColumnDecimal) AS ColumnDecimal" +
                    ", CONVERT(DATE, @ColumnDate) AS ColumnDate" +
                    ", CONVERT(TIME, @ColumnTime) AS ColumnTime" +
                    ", CONVERT(DATETIME, @ColumnDateTime) AS ColumnDateTime" +
                    ", CONVERT(DATETIME2(7), @ColumnDateTime2) AS ColumnDateTime2; SELECT @Id AS Id" +
                    ", CONVERT(INT, @ColumnInt) AS ColumnInt" +
                    ", CONVERT(BIGINT, @ColumnBigInt) AS ColumnBigInt" +
                    ", CONVERT(NVARCHAR(MAX), @ColumnNvarChar) AS ColumnNvarChar" +
                    ", CONVERT(FLOAT, @ColumnFloat) AS ColumnFloat" +
                    ", CONVERT(DECIMAL(18,2), @ColumnDecimal) AS ColumnDecimal" +
                    ", CONVERT(DATE, @ColumnDate) AS ColumnDate" +
                    ", CONVERT(TIME, @ColumnTime) AS ColumnTime" +
                    ", CONVERT(DATETIME, @ColumnDateTime) AS ColumnDateTime" +
                    ", CONVERT(DATETIME2(7), @ColumnDateTime2) AS ColumnDateTime2;", param);
                var firstResult = extractor.Extract<WhateverClassWithNonNullableProperties>();
                var secondResult = extractor.Extract<WhateverClassWithNonNullableProperties>();

                // Assert
                Assert.IsNotNull(firstResult);
                Assert.IsNotNull(secondResult);
                Helper.AssertPropertiesEquality(param, firstResult);
                Helper.AssertPropertiesEquality(param, secondResult);
            }
        }

        /// <summary>
        /// Verifies sql connection execute query multiple async whatever class with nullable properties.
        /// </summary>
        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryMultipleAsyncWhateverClassWithNullableProperties()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var param = new
                {
                    Id = 1,
                    ColumnInt = (int?)int.MaxValue,
                    ColumnBigInt = (long?)long.MaxValue,
                    ColumnNVarChar = Helper.GetAssemblyDescription(),
                    ColumnFloat = (double?)double.MaxValue,
                    ColumnDecimal = (decimal?)123456789.45m,
                    ColumnDate = (DateTime?)DateTime.UtcNow.Date,
                    ColumnTime = (TimeSpan?)DateTime.UtcNow.TimeOfDay,
                    ColumnDateTime = (DateTime?)DateTime.Parse("2019-01-01 00:00:05.123", CultureInfo.InvariantCulture),
                    ColumnDateTime2 = (DateTime?)DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
                };

                // Act
                var extractor = await connection.ExecuteQueryMultipleAsync("SELECT @Id AS Id" +
                    ", CONVERT(INT, @ColumnInt) AS ColumnInt" +
                    ", CONVERT(BIGINT, @ColumnBigInt) AS ColumnBigInt" +
                    ", CONVERT(NVARCHAR(MAX), @ColumnNvarChar) AS ColumnNvarChar" +
                    ", CONVERT(FLOAT, @ColumnFloat) AS ColumnFloat" +
                    ", CONVERT(DECIMAL(18,2), @ColumnDecimal) AS ColumnDecimal" +
                    ", CONVERT(DATE, @ColumnDate) AS ColumnDate" +
                    ", CONVERT(TIME, @ColumnTime) AS ColumnTime" +
                    ", CONVERT(DATETIME, @ColumnDateTime) AS ColumnDateTime" +
                    ", CONVERT(DATETIME2(7), @ColumnDateTime2) AS ColumnDateTime2; SELECT @Id AS Id" +
                    ", CONVERT(INT, @ColumnInt) AS ColumnInt" +
                    ", CONVERT(BIGINT, @ColumnBigInt) AS ColumnBigInt" +
                    ", CONVERT(NVARCHAR(MAX), @ColumnNvarChar) AS ColumnNvarChar" +
                    ", CONVERT(FLOAT, @ColumnFloat) AS ColumnFloat" +
                    ", CONVERT(DECIMAL(18,2), @ColumnDecimal) AS ColumnDecimal" +
                    ", CONVERT(DATE, @ColumnDate) AS ColumnDate" +
                    ", CONVERT(TIME, @ColumnTime) AS ColumnTime" +
                    ", CONVERT(DATETIME, @ColumnDateTime) AS ColumnDateTime" +
                    ", CONVERT(DATETIME2(7), @ColumnDateTime2) AS ColumnDateTime2;", param).ConfigureAwait(false);
                var firstResult = extractor.Extract<WhateverClassWithNonNullableProperties>();
                var secondResult = extractor.Extract<WhateverClassWithNonNullableProperties>();

                // Assert
                Assert.IsNotNull(firstResult);
                Assert.IsNotNull(secondResult);
                Helper.AssertPropertiesEquality(param, firstResult);
                Helper.AssertPropertiesEquality(param, secondResult);
            }
        }

        /// <summary>
        /// Verifies sql connection execute query multiple whatever class with nullable properties and with extra class properties.
        /// </summary>
        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleWhateverClassWithNullablePropertiesAndWithExtraClassProperties()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var param = new
                {
                    Id = 1,
                    ColumnInt = (int?)int.MaxValue,
                    ColumnBigInt = (long?)long.MaxValue,
                    ColumnNVarChar = Helper.GetAssemblyDescription(),
                    ColumnFloat = (double?)null,
                    ColumnDecimal = (decimal?)null,
                    ColumnDate = (DateTime?)null,
                    ColumnTime = (TimeSpan?)null,
                    ColumnDateTime = (DateTime?)null,
                    ColumnDateTime2 = (DateTime?)null
                };

                // Act
                var extractor = connection.ExecuteQueryMultiple("SELECT @Id AS Id" +
                    ", CONVERT(INT, @ColumnInt) AS ColumnInt" +
                    ", CONVERT(BIGINT, @ColumnBigInt) AS ColumnBigInt" +
                    ", CONVERT(NVARCHAR(MAX), @ColumnNvarChar) AS ColumnNvarChar; " +
                    "SELECT @Id AS Id" +
                    ", CONVERT(INT, @ColumnInt) AS ColumnInt" +
                    ", CONVERT(BIGINT, @ColumnBigInt) AS ColumnBigInt" +
                    ", CONVERT(NVARCHAR(MAX), @ColumnNvarChar) AS ColumnNvarChar;", param);
                var firstResult = extractor.Extract<WhateverClassWithNonNullableProperties>();
                var secondResult = extractor.Extract<WhateverClassWithNonNullableProperties>();

                // Assert
                Assert.IsNotNull(firstResult);
                Assert.IsNotNull(secondResult);
                Helper.AssertPropertiesEquality(param, firstResult);
                Helper.AssertPropertiesEquality(param, secondResult);
            }
        }

        /// <summary>
        /// Verifies sql connection execute query multiple async whatever class with nullable properties and with extra class properties.
        /// </summary>
        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryMultipleAsyncWhateverClassWithNullablePropertiesAndWithExtraClassProperties()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var param = new
                {
                    Id = 1,
                    ColumnInt = (int?)int.MaxValue,
                    ColumnBigInt = (long?)long.MaxValue,
                    ColumnNVarChar = Helper.GetAssemblyDescription(),
                    ColumnFloat = (double?)null,
                    ColumnDecimal = (decimal?)null,
                    ColumnDate = (DateTime?)null,
                    ColumnTime = (TimeSpan?)null,
                    ColumnDateTime = (DateTime?)null,
                    ColumnDateTime2 = (DateTime?)null
                };

                // Act
                var extractor = await connection.ExecuteQueryMultipleAsync("SELECT @Id AS Id" +
                    ", CONVERT(INT, @ColumnInt) AS ColumnInt" +
                    ", CONVERT(BIGINT, @ColumnBigInt) AS ColumnBigInt" +
                    ", CONVERT(NVARCHAR(MAX), @ColumnNvarChar) AS ColumnNvarChar; " +
                    "SELECT @Id AS Id" +
                    ", CONVERT(INT, @ColumnInt) AS ColumnInt" +
                    ", CONVERT(BIGINT, @ColumnBigInt) AS ColumnBigInt" +
                    ", CONVERT(NVARCHAR(MAX), @ColumnNvarChar) AS ColumnNvarChar;", param).ConfigureAwait(false);
                var firstResult = extractor.Extract<WhateverClassWithNonNullableProperties>();
                var secondResult = extractor.Extract<WhateverClassWithNonNullableProperties>();

                // Assert
                Assert.IsNotNull(firstResult);
                Assert.IsNotNull(secondResult);
                Helper.AssertPropertiesEquality(param, firstResult);
                Helper.AssertPropertiesEquality(param, secondResult);
            }
        }

        /// <summary>
        /// Verifies sql connection execute query multiple with whatever class with nullable properties and with extra query properties.
        /// </summary>
        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleWithWhateverClassWithNullablePropertiesAndWithExtraQueryProperties()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var param = new
                {
                    Id = 1,
                    ColumnInt = (int?)int.MaxValue,
                    ColumnBigInt = (long?)long.MaxValue,
                    ColumnNVarChar = Helper.GetAssemblyDescription(),
                    ColumnFloat = (double?)double.MaxValue,
                    ColumnDecimal = (decimal?)123456789.45m,
                    ColumnDate = (DateTime?)DateTime.UtcNow.Date,
                    ColumnTime = (TimeSpan?)DateTime.UtcNow.TimeOfDay,
                    ColumnDateTime = (DateTime?)DateTime.Parse("2019-01-01 00:00:05.123", CultureInfo.InvariantCulture),
                    ColumnDateTime2 = (DateTime?)DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
                };

                // Act
                var extractor = connection.ExecuteQueryMultiple("SELECT @Id AS Id" +
                    ", CONVERT(INT, @ColumnInt) AS ColumnInt" +
                    ", CONVERT(BIGINT, @ColumnBigInt) AS ColumnBigInt" +
                    ", CONVERT(NVARCHAR(MAX), @ColumnNvarChar) AS ColumnNvarChar" +
                    ", CONVERT(FLOAT, @ColumnFloat) AS ColumnFloat" +
                    ", CONVERT(DECIMAL(18,2), @ColumnDecimal) AS ColumnDecimal" +
                    ", CONVERT(DATE, @ColumnDate) AS ColumnDate" +
                    ", CONVERT(TIME, @ColumnTime) AS ColumnTime" +
                    ", CONVERT(DATETIME, @ColumnDateTime) AS ColumnDateTime" +
                    ", CONVERT(DATETIME2(7), @ColumnDateTime2) AS ColumnDateTime2" +
                    ", CONVERT(DATETIME2(5), GETUTCDATE()) AS CurrentDate" +
                    ", CONVERT(NVARCHAR(128), SYSTEM_USER) AS RequestorName; SELECT @Id AS Id" +
                    ", CONVERT(INT, @ColumnInt) AS ColumnInt" +
                    ", CONVERT(BIGINT, @ColumnBigInt) AS ColumnBigInt" +
                    ", CONVERT(NVARCHAR(MAX), @ColumnNvarChar) AS ColumnNvarChar" +
                    ", CONVERT(FLOAT, @ColumnFloat) AS ColumnFloat" +
                    ", CONVERT(DECIMAL(18,2), @ColumnDecimal) AS ColumnDecimal" +
                    ", CONVERT(DATE, @ColumnDate) AS ColumnDate" +
                    ", CONVERT(TIME, @ColumnTime) AS ColumnTime" +
                    ", CONVERT(DATETIME, @ColumnDateTime) AS ColumnDateTime" +
                    ", CONVERT(DATETIME2(7), @ColumnDateTime2) AS ColumnDateTime2" +
                    ", CONVERT(DATETIME2(5), GETUTCDATE()) AS CurrentDate" +
                    ", CONVERT(NVARCHAR(128), SYSTEM_USER) AS RequestorName;", param);
                var firstResult = extractor.Extract<WhateverClassWithNonNullableProperties>();
                var secondResult = extractor.Extract<WhateverClassWithNonNullableProperties>();

                // Assert
                Assert.IsNotNull(firstResult);
                Assert.IsNotNull(secondResult);
                Helper.AssertPropertiesEquality(param, firstResult);
                Helper.AssertPropertiesEquality(param, secondResult);
            }
        }

        /// <summary>
        /// Verifies sql connection execute query multiple async with whatever class with nullable properties and with extra query properties.
        /// </summary>
        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryMultipleAsyncWithWhateverClassWithNullablePropertiesAndWithExtraQueryProperties()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var param = new
                {
                    Id = 1,
                    ColumnInt = (int?)int.MaxValue,
                    ColumnBigInt = (long?)long.MaxValue,
                    ColumnNVarChar = Helper.GetAssemblyDescription(),
                    ColumnFloat = (double?)double.MaxValue,
                    ColumnDecimal = (decimal?)123456789.45m,
                    ColumnDate = (DateTime?)DateTime.UtcNow.Date,
                    ColumnTime = (TimeSpan?)DateTime.UtcNow.TimeOfDay,
                    ColumnDateTime = (DateTime?)DateTime.Parse("2019-01-01 00:00:05.123", CultureInfo.InvariantCulture),
                    ColumnDateTime2 = (DateTime?)DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
                };

                // Act
                var extractor = await connection.ExecuteQueryMultipleAsync("SELECT @Id AS Id" +
                    ", CONVERT(INT, @ColumnInt) AS ColumnInt" +
                    ", CONVERT(BIGINT, @ColumnBigInt) AS ColumnBigInt" +
                    ", CONVERT(NVARCHAR(MAX), @ColumnNvarChar) AS ColumnNvarChar" +
                    ", CONVERT(FLOAT, @ColumnFloat) AS ColumnFloat" +
                    ", CONVERT(DECIMAL(18,2), @ColumnDecimal) AS ColumnDecimal" +
                    ", CONVERT(DATE, @ColumnDate) AS ColumnDate" +
                    ", CONVERT(TIME, @ColumnTime) AS ColumnTime" +
                    ", CONVERT(DATETIME, @ColumnDateTime) AS ColumnDateTime" +
                    ", CONVERT(DATETIME2(7), @ColumnDateTime2) AS ColumnDateTime2" +
                    ", CONVERT(DATETIME2(5), GETUTCDATE()) AS CurrentDate" +
                    ", CONVERT(NVARCHAR(128), SYSTEM_USER) AS RequestorName; SELECT @Id AS Id" +
                    ", CONVERT(INT, @ColumnInt) AS ColumnInt" +
                    ", CONVERT(BIGINT, @ColumnBigInt) AS ColumnBigInt" +
                    ", CONVERT(NVARCHAR(MAX), @ColumnNvarChar) AS ColumnNvarChar" +
                    ", CONVERT(FLOAT, @ColumnFloat) AS ColumnFloat" +
                    ", CONVERT(DECIMAL(18,2), @ColumnDecimal) AS ColumnDecimal" +
                    ", CONVERT(DATE, @ColumnDate) AS ColumnDate" +
                    ", CONVERT(TIME, @ColumnTime) AS ColumnTime" +
                    ", CONVERT(DATETIME, @ColumnDateTime) AS ColumnDateTime" +
                    ", CONVERT(DATETIME2(7), @ColumnDateTime2) AS ColumnDateTime2" +
                    ", CONVERT(DATETIME2(5), GETUTCDATE()) AS CurrentDate" +
                    ", CONVERT(NVARCHAR(128), SYSTEM_USER) AS RequestorName;", param).ConfigureAwait(false);
                var firstResult = extractor.Extract<WhateverClassWithNonNullableProperties>();
                var secondResult = extractor.Extract<WhateverClassWithNonNullableProperties>();

                // Assert
                Assert.IsNotNull(firstResult);
                Assert.IsNotNull(secondResult);
                Helper.AssertPropertiesEquality(param, firstResult);
                Helper.AssertPropertiesEquality(param, secondResult);
            }
        }

        #endregion

        #endregion

        #region IDbDataParameter

        #region Sync

        /// <summary>
        /// Verifies sql connection execute query multiple via idb data parameter.
        /// </summary>
        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleViaIDbDataParameter()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                using (var result = connection.ExecuteQueryMultiple("SELECT 1 * @Value;",
                    new { Value = new SqlParameter("_", 100) }))
                {
                    var value = result.Extract<int>().FirstOrDefault();

                    // Assert
                    Assert.AreEqual(100, value);
                }
            }
        }

        /// <summary>
        /// Verifies sql connection execute query multiple via query field for idb data parameter.
        /// </summary>
        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleViaQueryFieldForIDbDataParameter()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                using (var result = connection.ExecuteQueryMultiple("SELECT 1 * @Value;",
                    new QueryField("Value", new SqlParameter("_", 100))))
                {
                    var value = result.Extract<int>().FirstOrDefault();

                    // Assert
                    Assert.AreEqual(100, value);
                }
            }
        }

        /// <summary>
        /// Verifies sql connection execute query multiple via query fields for idb data parameter.
        /// </summary>
        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleViaQueryFieldsForIDbDataParameter()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                using (var result = connection.ExecuteQueryMultiple("SELECT 1 * @Value;",
                    new QueryField("Value", new SqlParameter("_", 100)).AsEnumerable()))
                {
                    var value = result.Extract<int>().FirstOrDefault();

                    // Assert
                    Assert.AreEqual(100, value);
                }
            }
        }

        /// <summary>
        /// Verifies sql connection execute query multiple via query group for idb data parameter.
        /// </summary>
        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleViaQueryGroupForIDbDataParameter()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                using (var result = connection.ExecuteQueryMultiple("SELECT 1 * @Value;",
                    new QueryGroup(new QueryField("Value", new SqlParameter("_", 100)))))
                {
                    var value = result.Extract<int>().FirstOrDefault();

                    // Assert
                    Assert.AreEqual(100, value);
                }
            }
        }

        #endregion

        #region Async

        /// <summary>
        /// Verifies sql connection execute query multiple async via idb data parameter.
        /// </summary>
        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryMultipleAsyncViaIDbDataParameter()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                using (var result = await connection.ExecuteQueryMultipleAsync("SELECT 1 * @Value;",
                    new { Value = new SqlParameter("_", 100) }).ConfigureAwait(false))
                {
                    var value = (await result.ExtractAsync<int>().ConfigureAwait(false)).FirstOrDefault();

                    // Assert
                    Assert.AreEqual(100, value);
                }
            }
        }

        /// <summary>
        /// Verifies sql connection execute query multiple async via query field for idb data parameter.
        /// </summary>
        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryMultipleAsyncViaQueryFieldForIDbDataParameter()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                using (var result = await connection.ExecuteQueryMultipleAsync("SELECT 1 * @Value;",
                    new QueryField("Value", new SqlParameter("_", 100))).ConfigureAwait(false))
                {
                    var value = (await result.ExtractAsync<int>().ConfigureAwait(false)).FirstOrDefault();

                    // Assert
                    Assert.AreEqual(100, value);
                }
            }
        }

        /// <summary>
        /// Verifies sql connection execute query multiple async via query fields for idb data parameter.
        /// </summary>
        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryMultipleAsyncViaQueryFieldsForIDbDataParameter()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                using (var result = await connection.ExecuteQueryMultipleAsync("SELECT 1 * @Value;",
                    new QueryField("Value", new SqlParameter("_", 100)).AsEnumerable()).ConfigureAwait(false))
                {
                    var value = (await result.ExtractAsync<int>().ConfigureAwait(false)).FirstOrDefault();

                    // Assert
                    Assert.AreEqual(100, value);
                }
            }
        }

        /// <summary>
        /// Verifies sql connection execute query multiple async via query group for idb data parameter.
        /// </summary>
        [TestMethod]
        public async Task TestSqlConnectionExecuteQueryMultipleAsyncViaQueryGroupForIDbDataParameter()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                using (var result = await connection.ExecuteQueryMultipleAsync("SELECT 1 * @Value;",
                    new QueryGroup(new QueryField("Value", new SqlParameter("_", 100)))).ConfigureAwait(false))
                {
                    var value = (await result.ExtractAsync<int>().ConfigureAwait(false)).FirstOrDefault();

                    // Assert
                    Assert.AreEqual(100, value);
                }
            }
        }

        #endregion

        #endregion

        #endregion
    }
}
