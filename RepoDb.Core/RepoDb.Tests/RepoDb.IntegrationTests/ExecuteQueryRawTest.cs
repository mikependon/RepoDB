using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using RepoDb.Extensions;
using RepoDb.IntegrationTests.Setup;
using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using RepoDb.Interfaces;

namespace RepoDb.IntegrationTests
{
    [TestClass]
    public class ExecuteQueryRawTest
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            FluentMapper.Type<DateTime>().DbType(DbType.DateTime2, true);
            FluentMapper.Type<uint>().PropertyHandler<IntPropertyHandler>(true);
        }

        [ClassCleanup]
        public static void ClassCleaup()
        {
            TypeMapCache.Flush();
            PropertyHandlerCache.Flush();
        }

        [TestInitialize]
        public void Initialize()
        {
            Database.Initialize();
            Cleanup();
        }

        [TestCleanup]
        public void Cleanup()
        {
            Database.Cleanup();
        }

        #region PropertyHandlers

        private class IntPropertyHandler : IPropertyHandler<uint, uint>
        {
            public uint Get(uint input, ClassProperty property)
            {
                return (uint)(input * 2);
            }

            public uint Set(uint input, ClassProperty property)
            {
                return input;
            }
        }

        #endregion

        #region Enums

        private enum Gender
        {
            Male = 1,
            Female = 2
        }

        #endregion

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

        #region ExecuteQuery

        #region TypeResult

        #region PropertyHandler

        [TestMethod]
        public void TestSqlConnectionExecuteQueryTypeResultForPropertyHandler()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteQuery<uint>("SELECT CONVERT(INT, 1) AS Value UNION ALL SELECT 2;").AsList();

                // Assert
                Assert.AreEqual(2, result.Count);
                Assert.AreEqual((uint)2, result[0]);
                Assert.AreEqual((uint)4, result[1]);
            }
        }

        #endregion

        #region Enums

        #region String

        [TestMethod]
        public void TestSqlConnectionExecuteQueryTypeResultForEnumFromString()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteQuery<Gender>("SELECT 'Male' AS Value UNION ALL SELECT 'Female';").AsList();

                // Assert
                Assert.AreEqual(2, result.Count);
                Assert.AreEqual(Gender.Male, result[0]);
                Assert.AreEqual(Gender.Female, result[1]);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryTypeResultForNullableEnumFromString()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteQuery<Gender?>("SELECT 'Male' AS Value UNION ALL SELECT 'Female';").AsList();

                // Assert
                Assert.AreEqual(2, result.Count);
                Assert.AreEqual(Gender.Male, result[0]);
                Assert.AreEqual(Gender.Female, result[1]);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryTypeResultForNullableEnumFromStringWithNullResults()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteQuery<Gender?>("SELECT CONVERT(NVARCHAR, NULL) AS Value UNION ALL SELECT NULL;").AsList();

                // Assert
                Assert.AreEqual(2, result.Count);
                Assert.IsNull(result[0]);
                Assert.IsNull(result[1]);
            }
        }

        #endregion

        #region NonString

        [TestMethod]
        public void TestSqlConnectionExecuteQueryTypeResultForEnumFromNonString()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteQuery<Gender>("SELECT 1 AS Value UNION ALL SELECT 2;").AsList();

                // Assert
                Assert.AreEqual(2, result.Count);
                Assert.AreEqual(Gender.Male, result[0]);
                Assert.AreEqual(Gender.Female, result[1]);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryTypeResultForNullableEnumFromNonString()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteQuery<Gender?>("SELECT 1 AS Value UNION ALL SELECT 2;").AsList();

                // Assert
                Assert.AreEqual(2, result.Count);
                Assert.AreEqual(Gender.Male, result[0]);
                Assert.AreEqual(Gender.Female, result[1]);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryTypeResultForNullableEnumFromNonStringWithNullResults()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteQuery<Gender?>("SELECT CONVERT(INT, NULL) AS Value UNION ALL SELECT NULL;").AsList();

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

        [TestMethod]
        public void TestSqlConnectionExecuteQueryTypeResultForString()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteQuery<string>("SELECT 'ABC' AS Value UNION ALL SELECT 'DEF';").AsList();

                // Assert
                Assert.AreEqual(2, result.Count);
                Assert.AreEqual("ABC", result[0]);
                Assert.AreEqual("DEF", result[1]);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryTypeResultForStringWithNullResults()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteQuery<string>("SELECT CONVERT(NVARCHAR, NULL) AS Value UNION ALL SELECT NULL;").AsList();

                // Assert
                Assert.AreEqual(2, result.Count);
                result.ForEach(item => Assert.AreEqual(default(string), item));
            }
        }

        // Guid

        [TestMethod]
        public void TestSqlConnectionExecuteQueryTypeResultForGuid()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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

        [TestMethod]
        public void TestSqlConnectionExecuteQueryTypeResultForGuidWithNullResults()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteQuery<Guid>("SELECT CONVERT(UNIQUEIDENTIFIER, NULL) AS Value UNION ALL SELECT NULL;").AsList();

                // Assert
                Assert.AreEqual(2, result.Count);
                result.ForEach(item => Assert.AreEqual(default(Guid), item));
            }
        }

        // Long

        [TestMethod]
        public void TestSqlConnectionExecuteQueryTypeResultForLong()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteQuery<long>("SELECT CONVERT(BIGINT, 100) AS Value UNION ALL SELECT 200;").AsList();

                // Assert
                Assert.AreEqual(2, result.Count);
                Assert.AreEqual(100, result[0]);
                Assert.AreEqual(200, result[1]);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryTypeResultForLongWithNullResults()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteQuery<long>("SELECT CONVERT(BIGINT, NULL) AS Value UNION ALL SELECT NULL;").AsList();

                // Assert
                Assert.AreEqual(2, result.Count);
                result.ForEach(item => Assert.AreEqual(default(long), item));
            }
        }

        // DateTime

        [TestMethod]
        public void TestSqlConnectionExecuteQueryTypeResultForDateTime()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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

        [TestMethod]
        public void TestSqlConnectionExecuteQueryTypeResultForDateTimeWithNullResults()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteQuery<DateTime>("SELECT CONVERT(DATETIME, NULL) AS Value UNION ALL SELECT NULL;").AsList();

                // Assert
                Assert.AreEqual(2, result.Count);
                result.ForEach(item => Assert.AreEqual(default(DateTime), item));
            }
        }

        #endregion

        #region Nullables

        // Guid

        [TestMethod]
        public void TestSqlConnectionExecuteQueryTypeResultForNullableGuid()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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

        [TestMethod]
        public void TestSqlConnectionExecuteQueryTypeResultForNullableGuidWithNullResults()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteQuery<Guid?>("SELECT CONVERT(UNIQUEIDENTIFIER, NULL) AS Value UNION ALL SELECT NULL;").AsList();

                // Assert
                Assert.AreEqual(2, result.Count);
                result.ForEach(item => Assert.IsNull(item));
            }
        }

        // Long

        [TestMethod]
        public void TestSqlConnectionExecuteQueryTypeResultForNullableLong()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteQuery<long?>("SELECT CONVERT(BIGINT, 100) AS Value UNION ALL SELECT 200;").AsList();

                // Assert
                Assert.AreEqual(2, result.Count);
                Assert.AreEqual(100, result[0]);
                Assert.AreEqual(200, result[1]);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryTypeResultForNullableLongWithNullResults()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteQuery<long?>("SELECT CONVERT(BIGINT, NULL) AS Value UNION ALL SELECT NULL;").AsList();

                // Assert
                Assert.AreEqual(2, result.Count);
                result.ForEach(item => Assert.IsNull(item));
            }
        }

        // DateTime

        [TestMethod]
        public void TestSqlConnectionExecuteQueryTypeResultForNullableDateTime()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
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

        [TestMethod]
        public void TestSqlConnectionExecuteQueryTypeResultForNullableDateTimeWithNullResults()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.ExecuteQuery<DateTime?>("SELECT CONVERT(DATETIME, NULL) AS Value UNION ALL SELECT NULL;").AsList();

                // Assert
                Assert.AreEqual(2, result.Count);
                result.ForEach(item => Assert.IsNull(item));
            }
        }

        #endregion

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlConnectionExecuteQueryTypeResultWithMoreColumns()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.ExecuteQuery<int>("SELECT 1 AS Column1, 2 AS Column2 UNION ALL SELECT 3, 4;").AsList();
            }
        }

        #endregion

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
    }
}
