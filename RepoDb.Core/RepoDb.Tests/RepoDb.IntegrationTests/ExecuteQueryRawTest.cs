using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using RepoDb.IntegrationTests.Setup;
using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;

namespace RepoDb.IntegrationTests
{
    [TestClass]
    public class ExecuteQueryRawTest
    {
        [TestInitialize]
        public void Initialize()
        {
            TypeMapper.Add(typeof(DateTime), DbType.DateTime2, true);
            Database.Initialize();
            Cleanup();
        }

        [TestCleanup]
        public void Cleanup()
        {
            Database.Cleanup();
        }

        #region Classes

        private class WhateverClassWithNonNullableProperties
        {
            public int Id { get; set; }
            public int ColumnInt { get; set; }
            public long ColumnBigInt { get; set; }
            public string ColumnNVarChar { get; set; }
            public double ColumnFloat { get; set; }
            public decimal ColumnDecimal { get; set; }
            public DateTime ColumnDate { get; set; }
            public TimeSpan ColumnTime { get; set; }
            public DateTime ColumnDateTime { get; set; }
            public DateTime ColumnDateTime2 { get; set; }
        }

        private class WhateverClassWithNullableProperties
        {
            public int Id { get; set; }
            public int? ColumnInt { get; set; }
            public long? ColumnBigInt { get; set; }
            public string ColumnNVarChar { get; set; }
            public double? ColumnFloat { get; set; }
            public decimal? ColumnDecimal { get; set; }
            public DateTime? ColumnDate { get; set; }
            public TimeSpan? ColumnTime { get; set; }
            public DateTime? ColumnDateTime { get; set; }
            public DateTime? ColumnDateTime2 { get; set; }
        }

        private class MappedWhateverClassWithNonNullableProperties
        {
            [Map("Id")]
            public int IdMapped { get; set; }
            [Map("ColumnInt")]
            public int ColumnIntMapped { get; set; }
            [Map("ColumnBigInt")]
            public long ColumnBigIntMapped { get; set; }
            [Map("ColumnNVarChar")]
            public string ColumnNVarCharMapped { get; set; }
            [Map("ColumnFloat")]
            public double ColumnFloatMapped { get; set; }
            [Map("ColumnDecimal")]
            public decimal ColumnDecimalMapped { get; set; }
            [Map("ColumnDate")]
            public DateTime ColumnDateMapped { get; set; }
            [Map("ColumnTime")]
            public TimeSpan ColumnTimeMapped { get; set; }
            [Map("ColumnDateTime")]
            public DateTime ColumnDateTimeMapped { get; set; }
            [Map("ColumnDateTime2")]
            public DateTime ColumnDateTime2Mapped { get; set; }
        }

        private class MappedWhateverClassWithNullableProperties
        {
            [Map("Id")]
            public int IdMapped { get; set; }
            [Map("ColumnInt")]
            public int? ColumnIntMapped { get; set; }
            [Map("ColumnBigInt")]
            public long? ColumnBigIntMapped { get; set; }
            [Map("ColumnNVarChar")]
            public string ColumnNVarCharMapped { get; set; }
            [Map("ColumnFloat")]
            public double? ColumnFloatMapped { get; set; }
            [Map("ColumnDecimal")]
            public decimal? ColumnDecimalMapped { get; set; }
            [Map("ColumnDate")]
            public DateTime? ColumnDateMapped { get; set; }
            [Map("ColumnTime")]
            public TimeSpan? ColumnTimeMapped { get; set; }
            [Map("ColumnDateTime")]
            public DateTime? ColumnDateTimeMapped { get; set; }
            [Map("ColumnDateTime2")]
            public DateTime? ColumnDateTime2Mapped { get; set; }
        }

        #endregion

        #region DbConnection

        #region ExecuteQuery

        #region NonMapped

        #region NonNullables

        [TestMethod]
        public void TestSqlConnectionExecuteQueryWhateverClassWithNonNullableProperties()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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

        [TestMethod]
        public void TestSqlConnectionExecuteQueryWhateverClassWithNonNullablePropertiesAndWithExtraClassProperties()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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

        [TestMethod]
        public void TestSqlConnectionExecuteQueryWithWhateverClassWithNonNullablePropertiesAndWithExtraQueryProperties()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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

        [TestMethod]
        public void TestSqlConnectionExecuteQueryWhateverClassWithNullableProperties()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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

        [TestMethod]
        public void TestSqlConnectionExecuteQueryWhateverClassWithNullablePropertiesAndWithExtraClassProperties()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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

        [TestMethod]
        public void TestSqlConnectionExecuteQueryWithWhateverClassWithNullablePropertiesAndWithExtraQueryProperties()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMappedWhateverClassWithNonNullableProperties()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMappedWhateverClassWithNonNullablePropertiesAndWithExtraClassProperties()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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

        [TestMethod]
        public void TestSqlConnectionExecuteQueryWithMappedWhateverClassWithNonNullablePropertiesAndWithExtraQueryProperties()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMappedWhateverClassWithNullableProperties()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMappedWhateverClassWithNullablePropertiesAndWithExtraClassProperties()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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

        [TestMethod]
        public void TestSqlConnectionExecuteQueryWithMappedWhateverClassWithNullablePropertiesAndWithExtraQueryProperties()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleWhateverClassWithNonNullableProperties()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleWhateverClassWithNonNullablePropertiesAndWithExtraClassProperties()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleWithWhateverClassWithNonNullablePropertiesAndWithExtraQueryProperties()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleWhateverClassWithNullableProperties()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleWhateverClassWithNullablePropertiesAndWithExtraClassProperties()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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

        [TestMethod]
        public void TestSqlConnectionExecuteQueryMultipleWithWhateverClassWithNullablePropertiesAndWithExtraQueryProperties()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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
