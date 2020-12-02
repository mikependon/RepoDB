using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using RepoDb.Enumerations;
using RepoDb.IntegrationTests.Setup;
using System;
using Microsoft.Data.SqlClient;
using System.Linq;

namespace RepoDb.IntegrationTests
{
    [TestClass]
    public class TypeConversionsTest
    {
        private class CompleteTable { }

        [TestInitialize]
        public void Initialize()
        {
            Database.Initialize();
            Cleanup();
            Converter.ConversionType = ConversionType.Automatic;
        }

        [TestCleanup]
        public void Cleanup()
        {
            Database.Cleanup();
            Converter.ConversionType = ConversionType.Default;
        }

        #region TypedResult

        #region StringToType

        [TestMethod]
        public void TestSqlConnectionExecuteQueryConversionFromStringToString()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Query
                var data = connection.ExecuteQuery<string>("SELECT 'ABC' AS Value;").First();

                // Assert
                Assert.AreEqual("ABC", data);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryConversionFromStringToBigint()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Query
                var data = connection.ExecuteQuery<long>("SELECT '100' AS Value;").First();

                // Assert
                Assert.AreEqual(100, data);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryConversionFromStringToInt()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Query
                var data = connection.ExecuteQuery<int>("SELECT '100' AS Value;").First();

                // Assert
                Assert.AreEqual(100, data);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryConversionFromStringToDouble()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Query
                var data = connection.ExecuteQuery<double>("SELECT '100' AS Value;").First();

                // Assert
                Assert.AreEqual(100, data);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryConversionFromStringToGuid()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Query
                var data = connection.ExecuteQuery<Guid>("SELECT 'DE415ED3-24CB-4090-985B-0C76809578C8' AS Value;").First();

                // Assert
                Assert.AreEqual(Guid.Parse("DE415ED3-24CB-4090-985B-0C76809578C8"), data);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryConversionFromStringToDateTime()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Query
                var data = connection.ExecuteQuery<DateTime>("SELECT '1970-01-01' AS Value;").First();

                // Assert
                Assert.AreEqual(DateTime.Parse("1970-01-01"), data);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryConversionFromStringToBit()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Query
                var data = connection.ExecuteQuery<bool>("SELECT 'true' AS Value;").First();

                // Assert
                Assert.AreEqual(true, data);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryConversionFromStringToDecimal()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Query
                var data = connection.ExecuteQuery<decimal>("SELECT '100.05' AS Value;").First();

                // Assert
                Assert.AreEqual((decimal)100.05, data);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryConversionFromStringToFloat()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Query
                var data = connection.ExecuteQuery<float>("SELECT '100.05' AS Value;").First();

                // Assert
                Assert.AreEqual((float)100.05, data);
            }
        }

        #endregion

        #region IntToType

        [TestMethod]
        public void TestSqlConnectionExecuteQueryConversionFromIntToInt()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Query
                var data = connection.ExecuteQuery<int>("SELECT CONVERT(INT, 10) AS Value;").First();

                // Assert
                Assert.AreEqual(10, data);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryConversionFromIntToString()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Query
                var data = connection.ExecuteQuery<string>("SELECT CONVERT(INT, 10) AS Value;").First();

                // Assert
                Assert.AreEqual("10", data);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryConversionFromIntToLong()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Query
                var data = connection.ExecuteQuery<long>("SELECT CONVERT(INT, 10) AS Value;").First();

                // Assert
                Assert.AreEqual(10, data);
            }
        }

        #endregion

        #region BigIntToType

        [TestMethod]
        public void TestSqlConnectionExecuteQueryConversionFromBigIntToLong()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Query
                var data = connection.ExecuteQuery<long>("SELECT CONVERT(BIGINT, 10) AS Value;").First();

                // Assert
                Assert.AreEqual((long)10, data);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryConversionFromBigIntToString()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Query
                var data = connection.ExecuteQuery<string>("SELECT CONVERT(BIGINT, 10) AS Value;").First();

                // Assert
                Assert.AreEqual("10", data);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryConversionFromBigIntToInt()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Query
                var data = connection.ExecuteQuery<int>("SELECT CONVERT(BIGINT, 10) AS Value;").First();

                // Assert
                Assert.AreEqual(10, data);
            }
        }

        #endregion

        #region DecimalToType

        [TestMethod]
        public void TestSqlConnectionExecuteQueryConversionFromDecimalToDecimal()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Query
                var data = connection.ExecuteQuery<decimal>("SELECT CONVERT(DECIMAL(18,2), 100.05) AS Value;").First();

                // Assert
                Assert.AreEqual((decimal)100.05, data);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryConversionFromDecimalToString()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Query
                var data = connection.ExecuteQuery<string>("SELECT CONVERT(DECIMAL(18,2), 100.05) AS Value;").First();

                // Assert
                Assert.AreEqual("100.05", data);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryConversionFromDecimalToFloat()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Query
                var data = connection.ExecuteQuery<float>("SELECT CONVERT(DECIMAL(18,2), 100.05) AS Value;").First();

                // Assert
                Assert.AreEqual((float)100.05, data);
            }
        }

        #endregion

        #region RealToType

        [TestMethod]
        public void TestSqlConnectionExecuteQueryConversionFromRealToFloat()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Query
                var data = connection.ExecuteQuery<float>("SELECT CONVERT(REAL, 100.05) AS Value;").First();

                // Assert
                Assert.AreEqual((float)100.05, data);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryConversionFromRealToString()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Query
                var data = connection.ExecuteQuery<string>("SELECT CONVERT(REAL, 100.05) AS Value;").First();

                // Assert
                Assert.AreEqual("100.05", data);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryConversionFromRealToDecimal()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Query
                var data = connection.ExecuteQuery<decimal>("SELECT CONVERT(REAL, 100.05) AS Value;").First();

                // Assert
                Assert.AreEqual((decimal)100.05, data);
            }
        }

        #endregion

        #region DateTimeToType

        [TestMethod]
        public void TestSqlConnectionExecuteQueryConversionFromDateTimeToDateTime()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Query
                var data = connection.ExecuteQuery<DateTime>("SELECT CONVERT(DATETIME2(5), '1970-01-01') AS Value;").First();

                // Assert
                Assert.AreEqual(DateTime.Parse("1970-01-01"), data);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryConversionFromDateTimeToString()
        {
            using (var cultureScope = new CultureScope("en-US"))
            {
                using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
                {
                    // Act Query
                    var data = connection.ExecuteQuery<string>("SELECT CONVERT(DATETIME2(5), '1970-01-01') AS Value;").First();

                    // Assert
                    Assert.AreEqual("1/1/1970 12:00:00 AM", data);
                }
            }
        }

        #endregion

        #region UniqueIdentifierToType

        [TestMethod]
        public void TestSqlConnectionExecuteQueryConversionFromUniqueIdentifierToGuid()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Query
                var data = connection.ExecuteQuery<Guid>("SELECT CONVERT(UNIQUEIDENTIFIER, 'DE415ED3-24CB-4090-985B-0C76809578C8') AS Value;").First();

                // Assert
                Assert.AreEqual(Guid.Parse("DE415ED3-24CB-4090-985B-0C76809578C8"), data);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryConversionFromUniqueIdentifierToString()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Query
                var data = connection.ExecuteQuery<string>("SELECT CONVERT(UNIQUEIDENTIFIER, 'DE415ED3-24CB-4090-985B-0C76809578C8') AS Value;").First();

                // Assert
                Assert.IsTrue(string.Equals("DE415ED3-24CB-4090-985B-0C76809578C8", data, StringComparison.OrdinalIgnoreCase));
            }
        }

        #endregion

        #region BitToType

        [TestMethod]
        public void TestSqlConnectionExecuteQueryConversionFromBitToBoolean()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Query
                var data = connection.ExecuteQuery<bool>("SELECT CONVERT(BIT, 1) AS Value;").First();

                // Assert
                Assert.IsTrue(data);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryConversionFromBitToString()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Query
                var data = connection.ExecuteQuery<string>("SELECT CONVERT(BIT, 1) AS Value;").First();

                // Assert
                Assert.IsTrue(string.Equals("true", data, StringComparison.OrdinalIgnoreCase));
            }
        }

        #endregion

        #endregion

        #region Query

        #region StringToBigIntClass

        [Map("CompleteTable")]
        private class StringToBigIntClass
        {
            [Primary]
            public Guid SessionId { get; set; }
            public string ColumnBigInt { get; set; }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAndQueryConversionFromStringToBigInt()
        {
            // Setup
            var entity = new StringToBigIntClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBigInt = 12345.ToString()
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<StringToBigIntClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(entity.ColumnBigInt, data.ColumnBigInt);

                // Act Delete
                var deletedRows = connection.Delete<StringToBigIntClass>(e => e.SessionId == (Guid)id);

                // Act Query
                data = connection.Query<StringToBigIntClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(1, deletedRows);
                Assert.IsNull(data);
            }
        }

        #endregion

        #region StringToBitClass

        [Map("CompleteTable")]
        private class StringToBitClass
        {
            [Primary]
            public Guid SessionId { get; set; }
            public string ColumnBit { get; set; }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAndQueryConversionFromStringToBit()
        {
            // Setup
            var entity = new StringToBitClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBit = bool.TrueString
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<StringToBitClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(entity.ColumnBit, data.ColumnBit);
            }
        }

        #endregion

        #region StringToDecimalClass

        [Map("CompleteTable")]
        private class StringToDecimalClass
        {
            [Primary]
            public Guid SessionId { get; set; }
            public string ColumnDecimal { get; set; }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAndQueryConversionFromStringToDecimal()
        {
            // Setup
            var entity = new StringToDecimalClass
            {
                SessionId = Guid.NewGuid(),
                ColumnDecimal = "12345.55"
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<StringToDecimalClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(entity.ColumnDecimal, data.ColumnDecimal);
            }
        }

        #endregion

        #region StringToFloatClass

        [Map("CompleteTable")]
        private class StringToFloatClass
        {
            [Primary]
            public Guid SessionId { get; set; }
            public string ColumnFloat { get; set; }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAndQueryConversionFromStringToFloat()
        {
            // Setup
            var entity = new StringToFloatClass
            {
                SessionId = Guid.NewGuid(),
                ColumnFloat = "12345.55"
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<StringToFloatClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(entity.ColumnFloat, data.ColumnFloat);
            }
        }

        #endregion

        #region StringToIntClass

        [Map("CompleteTable")]
        private class StringToIntClass
        {
            [Primary]
            public Guid SessionId { get; set; }
            public string ColumnInt { get; set; }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAndQueryConversionFromStringToInt()
        {
            // Setup
            var entity = new StringToIntClass
            {
                SessionId = Guid.NewGuid(),
                ColumnInt = "12345"
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<StringToIntClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(entity.ColumnInt, data.ColumnInt);
            }
        }

        #endregion

        #region StringToMoneyClass

        [Map("CompleteTable")]
        private class StringToMoneyClass
        {
            [Primary]
            public Guid SessionId { get; set; }
            public string ColumnMoney { get; set; }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAndQueryConversionFromStringToMoney()
        {
            // Setup
            var entity = new StringToMoneyClass
            {
                SessionId = Guid.NewGuid(),
                ColumnMoney = "12345.6789"
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<StringToMoneyClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(entity.ColumnMoney, data.ColumnMoney);
            }
        }

        #endregion

        #region StringToNumericClass

        [Map("CompleteTable")]
        private class StringToNumericClass
        {
            [Primary]
            public Guid SessionId { get; set; }
            public string ColumnNumeric { get; set; }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAndQueryConversionFromStringToNumeric()
        {
            // Setup
            var entity = new StringToNumericClass
            {
                SessionId = Guid.NewGuid(),
                ColumnNumeric = "12345.67"
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<StringToNumericClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(entity.ColumnNumeric, data.ColumnNumeric);
            }
        }

        #endregion

        #region StringToRealClass

        [Map("CompleteTable")]
        private class StringToRealClass
        {
            [Primary]
            public Guid SessionId { get; set; }
            public string ColumnReal { get; set; }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAndQueryConversionFromStringToReal()
        {
            // Setup
            var entity = new StringToRealClass
            {
                SessionId = Guid.NewGuid(),
                ColumnReal = "12345.67"
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<StringToRealClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(entity.ColumnReal, data.ColumnReal);
            }
        }

        #endregion

        #region StringToSmallIntClass

        [Map("CompleteTable")]
        private class StringToSmallIntClass
        {
            [Primary]
            public Guid SessionId { get; set; }
            public string ColumnSmallInt { get; set; }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAndQueryConversionFromStringToSmallInt()
        {
            // Setup
            var entity = new StringToSmallIntClass
            {
                SessionId = Guid.NewGuid(),
                ColumnSmallInt = "12345"
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<StringToSmallIntClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(entity.ColumnSmallInt, data.ColumnSmallInt);
            }
        }

        #endregion

        #region StringToSmallMoneyClass

        [Map("CompleteTable")]
        private class StringToSmallMoneyClass
        {
            [Primary]
            public Guid SessionId { get; set; }
            public string ColumnSmallMoney { get; set; }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAndQueryConversionFromStringToSmallMoney()
        {
            // Setup
            var entity = new StringToSmallMoneyClass
            {
                SessionId = Guid.NewGuid(),
                ColumnSmallMoney = "12345.6700"
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<StringToSmallMoneyClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(entity.ColumnSmallMoney, data.ColumnSmallMoney);
            }
        }

        #endregion

        #region StringToDateClass

        [Map("CompleteTable")]
        private class StringToDateClass
        {
            [Primary]
            public Guid SessionId { get; set; }
            public string ColumnDate { get; set; }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAndQueryConversionFromStringToDate()
        {
            using (var cultureScope = new CultureScope("en-US"))
            {
                // Setup
                var entity = new StringToDateClass
                {
                    SessionId = Guid.NewGuid(),
                    ColumnDate = "1970-01-01"
                };

                using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
                {
                    // Act Insert
                    var id = connection.Insert(entity);

                    // Act Query
                    var data = connection.Query<StringToDateClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                    // Assert
                    Assert.AreEqual("1/1/1970 12:00:00 AM", data.ColumnDate);
                }
            }
        }

        #endregion

        #region StringToDateTimeClass

        [Map("CompleteTable")]
        private class StringToDateTimeClass
        {
            [Primary]
            public Guid SessionId { get; set; }
            public string ColumnDateTime { get; set; }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAndQueryConversionFromStringToDateTime()
        {
            using (var cultureScope = new CultureScope("en-US"))
            {
                // Setup
                var entity = new StringToDateTimeClass
                {
                    SessionId = Guid.NewGuid(),
                    ColumnDateTime = "1970-01-01 11:30 AM"
                };

                using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
                {
                    // Act Insert
                    var id = connection.Insert(entity);

                    // Act Query
                    var data = connection.Query<StringToDateTimeClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                    // Assert
                    Assert.AreEqual("1/1/1970 11:30:00 AM", data.ColumnDateTime);
                }
            }
        }

        #endregion

        #region StringToDateTime2Class

        [Map("CompleteTable")]
        private class StringToDateTime2Class
        {
            [Primary]
            public Guid SessionId { get; set; }
            public string ColumnDateTime2 { get; set; }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAndQueryConversionFromStringToDateTime2()
        {
            using (var cultureScope = new CultureScope("en-US"))
            {
                // Setup
                var entity = new StringToDateTime2Class
                {
                    SessionId = Guid.NewGuid(),
                    ColumnDateTime2 = "2019-03-03 15:22:10.0500000"
                };

                using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
                {
                    // Act Insert
                    var id = connection.Insert(entity);

                    // Act Query
                    var data = connection.Query<StringToDateTime2Class>(e => e.SessionId == (Guid)id).FirstOrDefault();

                    // Assert
                    Assert.AreEqual("3/3/2019 3:22:10 PM", data.ColumnDateTime2);
                }
            }
        }

        #endregion

        #region StringToUniqueIdentifierClass

        [Map("CompleteTable")]
        private class StringToUniqueIdentifierClass
        {
            [Primary]
            public Guid SessionId { get; set; }
            public string ColumnUniqueIdentifier { get; set; }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAndQueryConversionFromStringToUniqueIdentifier()
        {
            // Setup
            var entity = new StringToUniqueIdentifierClass
            {
                SessionId = Guid.NewGuid(),
                ColumnUniqueIdentifier = Guid.NewGuid().ToString()
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<StringToUniqueIdentifierClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(entity.ColumnUniqueIdentifier, data.ColumnUniqueIdentifier);
            }
        }

        #endregion

        #region UniqueIdentifierToStringClass

        [Map("CompleteTable")]
        private class UniqueIdentifierToStringClass
        {
            [Primary]
            public Guid SessionId { get; set; }
            public Guid? ColumnNVarChar { get; set; }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAndQueryConversionFromUniqueIdentifierToString()
        {
            // Setup
            var entity = new UniqueIdentifierToStringClass
            {
                SessionId = Guid.NewGuid(),
                ColumnNVarChar = Guid.NewGuid()
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<UniqueIdentifierToStringClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(entity.ColumnNVarChar, data.ColumnNVarChar);
            }
        }

        #endregion

        #region BitToStringClass

        [Map("CompleteTable")]
        private class BitToStringClass
        {
            [Primary]
            public Guid SessionId { get; set; }
            public bool ColumnNVarChar { get; set; }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAndQueryConversionFromStringToString()
        {
            // Setup
            var entity = new BitToStringClass
            {
                SessionId = Guid.NewGuid(),
                ColumnNVarChar = true
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<BitToStringClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(entity.ColumnNVarChar, data.ColumnNVarChar);
            }
        }

        #endregion

        #region DateTimeToStringClass (Date, DateTime, DateTime2)

        [Map("CompleteTable")]
        private class DateTimeToStringClass
        {
            [Primary]
            public Guid SessionId { get; set; }
            public DateTime? ColumnNVarChar { get; set; }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAndQueryConversionFromDateTimeToString()
        {
            // Setup
            var entity = new DateTimeToStringClass
            {
                SessionId = Guid.NewGuid(),
                ColumnNVarChar = DateTime.UtcNow.Date
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<DateTimeToStringClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(entity.ColumnNVarChar, data.ColumnNVarChar);
            }
        }

        #endregion

        #region IntToStringClass

        [Map("CompleteTable")]
        private class IntToStringClass
        {
            [Primary]
            public Guid SessionId { get; set; }
            public int? ColumnNVarChar { get; set; }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAndQueryConversionFromIntToString()
        {
            // Setup
            var entity = new IntToStringClass
            {
                SessionId = Guid.NewGuid(),
                ColumnNVarChar = int.MaxValue
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<IntToStringClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(entity.ColumnNVarChar, data.ColumnNVarChar);
            }
        }

        #endregion

        #region IntToBigIntClass

        [Map("CompleteTable")]
        private class IntToBigIntClass
        {
            [Primary]
            public Guid SessionId { get; set; }
            public int? ColumnBigInt { get; set; }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAndQueryConversionFromIntToBigInt()
        {
            // Setup
            var entity = new IntToBigIntClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBigInt = int.MaxValue
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<IntToBigIntClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(entity.ColumnBigInt, data.ColumnBigInt);
            }
        }

        #endregion

        #region IntToSmallIntClass

        [Map("CompleteTable")]
        private class IntToSmallIntClass
        {
            [Primary]
            public Guid SessionId { get; set; }
            public int? ColumnSmallInt { get; set; }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAndQueryConversionFromIntToSmallInt()
        {
            // Setup
            var entity = new IntToSmallIntClass
            {
                SessionId = Guid.NewGuid(),
                ColumnSmallInt = (int)short.MaxValue
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<IntToSmallIntClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(entity.ColumnSmallInt, data.ColumnSmallInt);
            }
        }

        #endregion

        #region IntToDecimalClass

        [Map("CompleteTable")]
        private class IntToDecimalClass
        {
            [Primary]
            public Guid SessionId { get; set; }
            public int? ColumnDecimal { get; set; }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAndQueryConversionFromIntToDecimal()
        {
            // Setup
            var entity = new IntToDecimalClass
            {
                SessionId = Guid.NewGuid(),
                ColumnDecimal = int.MaxValue
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<IntToDecimalClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(entity.ColumnDecimal, data.ColumnDecimal);
            }
        }

        #endregion

        #region IntToFloatClass

        [Map("CompleteTable")]
        private class IntToFloatClass
        {
            [Primary]
            public Guid SessionId { get; set; }
            public int? ColumnFloat { get; set; }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAndQueryConversionFromIntToFloat()
        {
            // Setup
            var entity = new IntToFloatClass
            {
                SessionId = Guid.NewGuid(),
                ColumnFloat = 12345
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<IntToFloatClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(entity.ColumnFloat, data.ColumnFloat);
            }
        }

        #endregion

        #region IntToRealClass

        [Map("CompleteTable")]
        private class IntToRealClass
        {
            [Primary]
            public Guid SessionId { get; set; }
            public int? ColumnReal { get; set; }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAndQueryConversionFromIntToReal()
        {
            // Setup
            var entity = new IntToRealClass
            {
                SessionId = Guid.NewGuid(),
                ColumnReal = 12345
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<IntToRealClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(entity.ColumnReal, data.ColumnReal);
            }
        }

        #endregion

        #region IntToBitClass

        [Map("CompleteTable")]
        private class IntToBitClass
        {
            [Primary]
            public Guid SessionId { get; set; }
            public int? ColumnBit { get; set; }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAndQueryConversionFromIntToBit()
        {
            // Setup
            var entity = new IntToBitClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBit = 1
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<IntToBitClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(entity.ColumnBit, data.ColumnBit);
            }
        }

        #endregion

        #region BigIntToStringClass

        [Map("CompleteTable")]
        private class BigIntToStringClass
        {
            [Primary]
            public Guid SessionId { get; set; }
            public long? ColumnNVarChar { get; set; }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAndQueryConversionFromBigIntToString()
        {
            // Setup
            var entity = new BigIntToStringClass
            {
                SessionId = Guid.NewGuid(),
                ColumnNVarChar = long.MaxValue
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<BigIntToStringClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(entity.ColumnNVarChar, data.ColumnNVarChar);
            }
        }

        #endregion

        #region BigIntToIntClass

        [Map("CompleteTable")]
        private class BigIntToIntClass
        {
            [Primary]
            public Guid SessionId { get; set; }
            public long? ColumnInt { get; set; }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAndQueryConversionFromBigIntToInt()
        {
            // Setup
            var entity = new BigIntToIntClass
            {
                SessionId = Guid.NewGuid(),
                ColumnInt = int.MaxValue
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<BigIntToIntClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(entity.ColumnInt, data.ColumnInt);
            }
        }

        #endregion

        #region BigIntToSmallIntClass

        [Map("CompleteTable")]
        private class BigIntToSmallIntClass
        {
            [Primary]
            public Guid SessionId { get; set; }
            public long? ColumnSmallInt { get; set; }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAndQueryConversionFromBigIntToSmallInt()
        {
            // Setup
            var entity = new BigIntToSmallIntClass
            {
                SessionId = Guid.NewGuid(),
                ColumnSmallInt = short.MaxValue
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<BigIntToSmallIntClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(entity.ColumnSmallInt, data.ColumnSmallInt);
            }
        }

        #endregion

        #region BigIntToDecimalClass

        [Map("CompleteTable")]
        private class BigIntToDecimalClass
        {
            [Primary]
            public Guid SessionId { get; set; }
            public long? ColumnDecimal { get; set; }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAndQueryConversionFromBigIntToDecimal()
        {
            // Setup
            var entity = new BigIntToDecimalClass
            {
                SessionId = Guid.NewGuid(),
                ColumnDecimal = 12345
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<BigIntToDecimalClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(entity.ColumnDecimal, data.ColumnDecimal);
            }
        }

        #endregion

        #region BigIntToFloatClass

        [Map("CompleteTable")]
        private class BigIntToFloatClass
        {
            [Primary]
            public Guid SessionId { get; set; }
            public long? ColumnFloat { get; set; }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAndQueryConversionFromBigIntToFloat()
        {
            // Setup
            var entity = new BigIntToFloatClass
            {
                SessionId = Guid.NewGuid(),
                ColumnFloat = 12345
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<BigIntToFloatClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(entity.ColumnFloat, data.ColumnFloat);
            }
        }

        #endregion

        #region BigIntToRealClass

        [Map("CompleteTable")]
        private class BigIntToRealClass
        {
            [Primary]
            public Guid SessionId { get; set; }
            public long? ColumnReal { get; set; }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAndQueryConversionFromBigIntToReal()
        {
            // Setup
            var entity = new BigIntToRealClass
            {
                SessionId = Guid.NewGuid(),
                ColumnReal = 12345
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<BigIntToRealClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(entity.ColumnReal, data.ColumnReal);
            }
        }

        #endregion

        #region BigIntToBitClass

        [Map("CompleteTable")]
        private class BigIntToBitClass
        {
            [Primary]
            public Guid SessionId { get; set; }
            public long? ColumnBit { get; set; }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAndQueryConversionFromBigIntToBit()
        {
            // Setup
            var entity = new BigIntToBitClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBit = 1
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<BigIntToBitClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(entity.ColumnBit, data.ColumnBit);
            }
        }

        #endregion

        #region SmallIntToStringClass

        [Map("CompleteTable")]
        private class SmallIntToStringClass
        {
            [Primary]
            public Guid SessionId { get; set; }
            public short? ColumnNVarChar { get; set; }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAndQueryConversionFromSmallIntToString()
        {
            // Setup
            var entity = new SmallIntToStringClass
            {
                SessionId = Guid.NewGuid(),
                ColumnNVarChar = short.MaxValue
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<SmallIntToStringClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(entity.ColumnNVarChar, data.ColumnNVarChar);
            }
        }

        #endregion

        #region SmallIntToIntClass

        [Map("CompleteTable")]
        private class SmallIntToIntClass
        {
            [Primary]
            public Guid SessionId { get; set; }
            public short? ColumnInt { get; set; }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAndQueryConversionFromSmallIntToInt()
        {
            // Setup
            var entity = new SmallIntToIntClass
            {
                SessionId = Guid.NewGuid(),
                ColumnInt = short.MaxValue
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<SmallIntToIntClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(entity.ColumnInt, data.ColumnInt);
            }
        }

        #endregion

        #region SmallIntToBigIntClass

        [Map("CompleteTable")]
        private class SmallIntToBigIntClass
        {
            [Primary]
            public Guid SessionId { get; set; }
            public short? ColumnBigInt { get; set; }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAndQueryConversionFromSmallIntToBigInt()
        {
            // Setup
            var entity = new SmallIntToBigIntClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBigInt = short.MaxValue
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<SmallIntToBigIntClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(entity.ColumnBigInt, data.ColumnBigInt);
            }
        }

        #endregion

        #region SmallIntToDecimalClass

        [Map("CompleteTable")]
        private class SmallIntToDecimalClass
        {
            [Primary]
            public Guid SessionId { get; set; }
            public short? ColumnDecimal { get; set; }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAndQueryConversionFromSmallIntToDecimal()
        {
            // Setup
            var entity = new SmallIntToDecimalClass
            {
                SessionId = Guid.NewGuid(),
                ColumnDecimal = 12345
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<SmallIntToDecimalClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(entity.ColumnDecimal, data.ColumnDecimal);
            }
        }

        #endregion

        #region SmallIntToFloatClass

        [Map("CompleteTable")]
        private class SmallIntToFloatClass
        {
            [Primary]
            public Guid SessionId { get; set; }
            public short? ColumnFloat { get; set; }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAndQueryConversionFromSmallIntToFloat()
        {
            // Setup
            var entity = new SmallIntToFloatClass
            {
                SessionId = Guid.NewGuid(),
                ColumnFloat = 12345
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<SmallIntToFloatClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(entity.ColumnFloat, data.ColumnFloat);
            }
        }

        #endregion

        #region SmallIntToRealClass

        [Map("CompleteTable")]
        private class SmallIntToRealClass
        {
            [Primary]
            public Guid SessionId { get; set; }
            public short? ColumnReal { get; set; }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAndQueryConversionFromSmallIntToReal()
        {
            // Setup
            var entity = new SmallIntToRealClass
            {
                SessionId = Guid.NewGuid(),
                ColumnReal = 12345
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<SmallIntToRealClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(entity.ColumnReal, data.ColumnReal);
            }
        }

        #endregion

        #region SmallIntToBitClass

        [Map("CompleteTable")]
        private class SmallIntToBitClass
        {
            [Primary]
            public Guid SessionId { get; set; }
            public short? ColumnBit { get; set; }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAndQueryConversionFromSmallIntToBit()
        {
            // Setup
            var entity = new SmallIntToBitClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBit = 12345
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<SmallIntToBitClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual((short)1, data.ColumnBit);
            }
        }

        #endregion

        #region DecimalToStringClass

        [Map("CompleteTable")]
        private class DecimalToStringClass
        {
            [Primary]
            public Guid SessionId { get; set; }
            public decimal? ColumnNVarChar { get; set; }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAndQueryConversionFromDecimalToString()
        {
            // Setup
            var entity = new DecimalToStringClass
            {
                SessionId = Guid.NewGuid(),
                ColumnNVarChar = decimal.MaxValue
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<DecimalToStringClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(entity.ColumnNVarChar, data.ColumnNVarChar);
            }
        }

        #endregion

        #region DecimalToIntClass

        [Map("CompleteTable")]
        private class DecimalToIntClass
        {
            [Primary]
            public Guid SessionId { get; set; }
            public decimal? ColumnInt { get; set; }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAndQueryConversionFromDecimalToInt()
        {
            // Setup
            var entity = new DecimalToIntClass
            {
                SessionId = Guid.NewGuid(),
                ColumnInt = 12345
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<DecimalToIntClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(12345, data.ColumnInt);
            }
        }

        #endregion

        #region DecimalToBigIntClass

        [Map("CompleteTable")]
        private class DecimalToBigIntClass
        {
            [Primary]
            public Guid SessionId { get; set; }
            public decimal? ColumnBigInt { get; set; }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAndQueryConversionFromDecimalToBigInt()
        {
            // Setup
            var entity = new DecimalToBigIntClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBigInt = 12345
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<DecimalToBigIntClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(12345, data.ColumnBigInt);
            }
        }

        #endregion

        #region DecimalToSmallIntClass

        [Map("CompleteTable")]
        private class DecimalToSmallIntClass
        {
            [Primary]
            public Guid SessionId { get; set; }
            public decimal? ColumnSmallInt { get; set; }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAndQueryConversionFromDecimalToSmallInt()
        {
            // Setup
            var entity = new DecimalToSmallIntClass
            {
                SessionId = Guid.NewGuid(),
                ColumnSmallInt = 12345
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<DecimalToSmallIntClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(12345, data.ColumnSmallInt);
            }
        }

        #endregion

        #region DecimalToFloatIntClass

        [Map("CompleteTable")]
        private class DecimalToFloatClass
        {
            [Primary]
            public Guid SessionId { get; set; }
            public decimal? ColumnFloat { get; set; }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAndQueryConversionFromDecimalToFloat()
        {
            // Setup
            var entity = new DecimalToFloatClass
            {
                SessionId = Guid.NewGuid(),
                ColumnFloat = 12345.67M
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<DecimalToFloatClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(entity.ColumnFloat, data.ColumnFloat);
            }
        }

        #endregion

        #region DecimalToRealClass

        [Map("CompleteTable")]
        private class DecimalToRealClass
        {
            [Primary]
            public Guid SessionId { get; set; }
            public decimal? ColumnReal { get; set; }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAndQueryConversionFromDecimalToReal()
        {
            // Setup
            var entity = new DecimalToRealClass
            {
                SessionId = Guid.NewGuid(),
                ColumnReal = 12345.67M
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<DecimalToRealClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(entity.ColumnReal, data.ColumnReal);
            }
        }

        #endregion

        #region DecimalToBitClass

        [Map("CompleteTable")]
        private class DecimalToBitClass
        {
            [Primary]
            public Guid SessionId { get; set; }
            public decimal? ColumnBit { get; set; }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAndQueryConversionFromDecimalToBit()
        {
            // Setup
            var entity = new DecimalToBitClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBit = 1
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<DecimalToBitClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(1, data.ColumnBit);
            }
        }

        #endregion

        #region DoubleToStringClass

        [Map("CompleteTable")]
        private class DoubleToStringClass
        {
            [Primary]
            public Guid SessionId { get; set; }
            public double? ColumnNVarChar { get; set; }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAndQueryConversionFromDoubleToString()
        {
            // Setup
            var entity = new DoubleToStringClass
            {
                SessionId = Guid.NewGuid(),
                ColumnNVarChar = 12345
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<DoubleToStringClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(entity.ColumnNVarChar, data.ColumnNVarChar);
            }
        }

        #endregion

        #region DoubleToDecimalClass

        [Map("CompleteTable")]
        private class DoubleToDecimalClass
        {
            [Primary]
            public Guid SessionId { get; set; }
            public double? ColumnDecimal { get; set; }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAndQueryConversionFromDoubleToDecimal()
        {
            // Setup
            var entity = new DoubleToDecimalClass
            {
                SessionId = Guid.NewGuid(),
                ColumnDecimal = 12345.67
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<DoubleToDecimalClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(entity.ColumnDecimal, data.ColumnDecimal);
            }
        }

        #endregion

        #region DoubleToBigIntClass

        [Map("CompleteTable")]
        private class DoubleToBigIntClass
        {
            [Primary]
            public Guid SessionId { get; set; }
            public double? ColumnBigInt { get; set; }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAndQueryConversionFromDoubleToBigInt()
        {
            // Setup
            var entity = new DoubleToBigIntClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBigInt = 12345.67
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<DoubleToBigIntClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(12346, data.ColumnBigInt);
            }
        }

        #endregion

        #region DoubleToIntClass

        [Map("CompleteTable")]
        private class DoubleToIntClass
        {
            [Primary]
            public Guid SessionId { get; set; }
            public double? ColumnInt { get; set; }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAndQueryConversionFromDoubleToInt()
        {
            // Setup
            var entity = new DoubleToIntClass
            {
                SessionId = Guid.NewGuid(),
                ColumnInt = 12345.67
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<DoubleToIntClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(12346, data.ColumnInt);
            }
        }

        #endregion

        #region DoubleToIntClass

        [Map("CompleteTable")]
        private class DoubleToSmallIntClass
        {
            [Primary]
            public Guid SessionId { get; set; }
            public double? ColumnInt { get; set; }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAndQueryConversionFromDoubleToSmallInt()
        {
            // Setup
            var entity = new DoubleToSmallIntClass
            {
                SessionId = Guid.NewGuid(),
                ColumnInt = 12345.67
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<DoubleToSmallIntClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(12346, data.ColumnInt);
            }
        }

        #endregion

        #region DoubleToFloatClass

        [Map("CompleteTable")]
        private class DoubleToFloatClass
        {
            [Primary]
            public Guid SessionId { get; set; }
            public double? ColumnFloat { get; set; }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAndQueryConversionFromDoubleToFloat()
        {
            // Setup
            var entity = new DoubleToFloatClass
            {
                SessionId = Guid.NewGuid(),
                ColumnFloat = 12345.67
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<DoubleToFloatClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(entity.ColumnFloat, data.ColumnFloat);
            }
        }

        #endregion

        #region DoubleToRealClass

        [Map("CompleteTable")]
        private class DoubleToRealClass
        {
            [Primary]
            public Guid SessionId { get; set; }
            public double? ColumnReal { get; set; }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAndQueryConversionFromDoubleToReal()
        {
            // Setup
            var entity = new DoubleToRealClass
            {
                SessionId = Guid.NewGuid(),
                ColumnReal = 12345.67
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<DoubleToRealClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(entity.ColumnReal, Math.Round(data.ColumnReal.Value, 2));
            }
        }

        #endregion

        #region DoubleToBitClass

        [Map("CompleteTable")]
        private class DoubleToBitClass
        {
            [Primary]
            public Guid SessionId { get; set; }
            public double? ColumnBit { get; set; }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAndQueryConversionFromDoubleToBit()
        {
            // Setup
            var entity = new DoubleToBitClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBit = 12345.67
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<DoubleToBitClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(1, data.ColumnBit);
            }
        }

        #endregion

        #region FloatToStringClass (Real, Money, Float)

        [Map("CompleteTable")]
        private class FloatToStringClass
        {
            [Primary]
            public Guid SessionId { get; set; }
            public float? ColumnNVarChar { get; set; }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAndQueryConversionFromFloatToString()
        {
            // Setup
            var entity = new FloatToStringClass
            {
                SessionId = Guid.NewGuid(),
                ColumnNVarChar = 12345.7F
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<FloatToStringClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(entity.ColumnNVarChar, data.ColumnNVarChar);
            }
        }

        #endregion

        #region FloatToDecimalClass

        [Map("CompleteTable")]
        private class FloatToDecimalClass
        {
            [Primary]
            public Guid SessionId { get; set; }
            public double? ColumnDecimal { get; set; }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAndQueryConversionFromFloatToDecimal()
        {
            // Setup
            var entity = new FloatToDecimalClass
            {
                SessionId = Guid.NewGuid(),
                ColumnDecimal = 12345.67
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<FloatToDecimalClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(entity.ColumnDecimal, data.ColumnDecimal);
            }
        }

        #endregion

        #region FloatToBigIntClass

        [Map("CompleteTable")]
        private class FloatToBigIntClass
        {
            [Primary]
            public Guid SessionId { get; set; }
            public double? ColumnBigInt { get; set; }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAndQueryConversionFromFloatToBigInt()
        {
            // Setup
            var entity = new FloatToBigIntClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBigInt = 12345.67
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<FloatToBigIntClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(12346, data.ColumnBigInt);
            }
        }

        #endregion

        #region FloatToIntClass

        [Map("CompleteTable")]
        private class FloatToIntClass
        {
            [Primary]
            public Guid SessionId { get; set; }
            public double? ColumnInt { get; set; }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAndQueryConversionFromFloatToInt()
        {
            // Setup
            var entity = new FloatToIntClass
            {
                SessionId = Guid.NewGuid(),
                ColumnInt = 12345.67
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<FloatToIntClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(12346, data.ColumnInt);
            }
        }

        #endregion

        #region FloatToIntClass

        [Map("CompleteTable")]
        private class FloatToSmallIntClass
        {
            [Primary]
            public Guid SessionId { get; set; }
            public double? ColumnInt { get; set; }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAndQueryConversionFromFloatToSmallInt()
        {
            // Setup
            var entity = new FloatToSmallIntClass
            {
                SessionId = Guid.NewGuid(),
                ColumnInt = 12345.67
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<FloatToSmallIntClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(12346, data.ColumnInt);
            }
        }

        #endregion

        #region FloatToFloatClass

        [Map("CompleteTable")]
        private class FloatToFloatClass
        {
            [Primary]
            public Guid SessionId { get; set; }
            public double? ColumnFloat { get; set; }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAndQueryConversionFromFloatToFloat()
        {
            // Setup
            var entity = new FloatToFloatClass
            {
                SessionId = Guid.NewGuid(),
                ColumnFloat = 12345.67
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<FloatToFloatClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(entity.ColumnFloat, data.ColumnFloat);
            }
        }

        #endregion

        #region FloatToRealClass

        [Map("CompleteTable")]
        private class FloatToRealClass
        {
            [Primary]
            public Guid SessionId { get; set; }
            public double? ColumnReal { get; set; }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAndQueryConversionFromFloatToReal()
        {
            // Setup
            var entity = new FloatToRealClass
            {
                SessionId = Guid.NewGuid(),
                ColumnReal = 12345.67
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<FloatToRealClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(entity.ColumnReal, Math.Round(data.ColumnReal.Value, 2));
            }
        }

        #endregion

        #region FloatToBitClass

        [Map("CompleteTable")]
        private class FloatToBitClass
        {
            [Primary]
            public Guid SessionId { get; set; }
            public double? ColumnBit { get; set; }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAndQueryConversionFromFloatToBit()
        {
            // Setup
            var entity = new FloatToBitClass
            {
                SessionId = Guid.NewGuid(),
                ColumnBit = 12345.67
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb).EnsureOpen())
            {
                // Act Insert
                var id = connection.Insert(entity);

                // Act Query
                var data = connection.Query<FloatToBitClass>(e => e.SessionId == (Guid)id).FirstOrDefault();

                // Assert
                Assert.AreEqual(1, data.ColumnBit);
            }
        }

        #endregion

        #endregion
    }
}
