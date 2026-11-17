#region Copyright Attributions

// Copyright (c) 2019 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using RepoDb.IntegrationTests.Setup;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace RepoDb.IntegrationTests
{
    /// <summary>
    /// Contains the execute query raw test behavior.
    /// </summary>
    [TestClass]
    public class ExecuteQueryRawTest
    {
        /// <summary>
        /// Performs initialize.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            TypeMapper.Map(typeof(DateTime), DbType.DateTime2, true);
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

        #region DbConnection

        #region ExecuteQuery

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
                    ColumnDecimal = Convert.ToDecimal(123456789.45),
                    ColumnDate = DateTime.UtcNow.Date,
                    ColumnTime = DateTime.UtcNow.TimeOfDay,
                    ColumnDateTime = DateTime.Parse("2019-01-01 00:00:05.123"),
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
                    ColumnDecimal = Convert.ToDecimal(123456789.45),
                    ColumnDate = DateTime.UtcNow.Date,
                    ColumnTime = DateTime.UtcNow.TimeOfDay,
                    ColumnDateTime = DateTime.Parse("2019-01-01 00:00:05.123"),
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
                    ColumnDecimal = (decimal?)Convert.ToDecimal(123456789.45),
                    ColumnDate = (DateTime?)DateTime.UtcNow.Date,
                    ColumnTime = (TimeSpan?)DateTime.UtcNow.TimeOfDay,
                    ColumnDateTime = (DateTime?)DateTime.Parse("2019-01-01 00:00:05.123"),
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
                    ColumnDecimal = (decimal?)Convert.ToDecimal(123456789.45),
                    ColumnDate = (DateTime?)DateTime.UtcNow.Date,
                    ColumnTime = (TimeSpan?)DateTime.UtcNow.TimeOfDay,
                    ColumnDateTime = (DateTime?)DateTime.Parse("2019-01-01 00:00:05.123"),
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
                    ColumnDecimalMapped = Convert.ToDecimal(123456789.45),
                    ColumnDateMapped = DateTime.UtcNow.Date,
                    ColumnTimeMapped = DateTime.UtcNow.TimeOfDay,
                    ColumnDateTimeMapped = DateTime.Parse("2019-01-01 00:00:05.123"),
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
                    ColumnDecimalMapped = Convert.ToDecimal(123456789.45),
                    ColumnDateMapped = DateTime.UtcNow.Date,
                    ColumnTimeMapped = DateTime.UtcNow.TimeOfDay,
                    ColumnDateTimeMapped = DateTime.Parse("2019-01-01 00:00:05.123"),
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
                    ColumnDecimalMapped = (decimal?)Convert.ToDecimal(123456789.45),
                    ColumnDateMapped = (DateTime?)DateTime.UtcNow.Date,
                    ColumnTimeMapped = (TimeSpan?)DateTime.UtcNow.TimeOfDay,
                    ColumnDateTimeMapped = (DateTime?)DateTime.Parse("2019-01-01 00:00:05.123"),
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
                    ColumnDecimalMapped = (decimal?)Convert.ToDecimal(123456789.45),
                    ColumnDateMapped = (DateTime?)DateTime.UtcNow.Date,
                    ColumnTimeMapped = (TimeSpan?)DateTime.UtcNow.TimeOfDay,
                    ColumnDateTimeMapped = (DateTime?)DateTime.Parse("2019-01-01 00:00:05.123"),
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
                    ColumnDecimal = Convert.ToDecimal(123456789.45),
                    ColumnDate = DateTime.UtcNow.Date,
                    ColumnTime = DateTime.UtcNow.TimeOfDay,
                    ColumnDateTime = DateTime.Parse("2019-01-01 00:00:05.123"),
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
                    ColumnDecimal = Convert.ToDecimal(123456789.45),
                    ColumnDate = DateTime.UtcNow.Date,
                    ColumnTime = DateTime.UtcNow.TimeOfDay,
                    ColumnDateTime = DateTime.Parse("2019-01-01 00:00:05.123"),
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
                    ColumnDecimal = (decimal?)Convert.ToDecimal(123456789.45),
                    ColumnDate = (DateTime?)DateTime.UtcNow.Date,
                    ColumnTime = (TimeSpan?)DateTime.UtcNow.TimeOfDay,
                    ColumnDateTime = (DateTime?)DateTime.Parse("2019-01-01 00:00:05.123"),
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
                    ColumnDecimal = (decimal?)Convert.ToDecimal(123456789.45),
                    ColumnDate = (DateTime?)DateTime.UtcNow.Date,
                    ColumnTime = (TimeSpan?)DateTime.UtcNow.TimeOfDay,
                    ColumnDateTime = (DateTime?)DateTime.Parse("2019-01-01 00:00:05.123"),
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

        #endregion

        #endregion

        #endregion

        #endregion
    }
}
