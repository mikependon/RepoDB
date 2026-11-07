#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Data.Common;
using System.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;
using RepoDb.Schema.SqlServer.IntegrationTests.Setup;

namespace RepoDb.Schema.SqlServer.IntegrationTests
{
    /// <summary>
    /// Writes the typed values and the kinds of default values with the formatter, uses them as the default value of a column in a real SQL Server database,
    /// inserts a row without a value for the column, and reads the value back.
    /// </summary>
    [TestClass]
    public class SqlServerSchemaValueFormatterRoundTripTest
    {
        #region Private Variables

        private readonly SqlServerSchemaValueFormatter _formatter = new SqlServerSchemaValueFormatter();
        private static readonly DateTime Moment = new DateTime(2020, 1, 31, 10, 20, 30).AddTicks(1234567);

        #endregion

        #region Private Methods

        private static object Execute(string columnType,
            string defaultText)
        {
            Database.Initialize();
            var table = "rt_" + Guid.NewGuid().ToString("N").Substring(0, 12);
            using (var connection = new SqlConnection(Database.ConnectionStringForTarget))
            {
                connection.Open();
                try
                {
                    Run(connection, $"CREATE TABLE [{table}] ([id] int, [c] {columnType} DEFAULT {defaultText})");
                    Run(connection, $"INSERT INTO [{table}] ([id]) VALUES (1)");
                    return Run(connection, $"SELECT [c] FROM [{table}]");
                }
                finally
                {
                    Run(connection, $"DROP TABLE IF EXISTS [{table}]");
                }
            }
        }

        private static object Run(DbConnection connection,
            string statement)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = statement;
                return command.ExecuteScalar();
            }
        }

        private object RoundTrip(string columnType,
            SchemaValue value) =>
            Execute(columnType,
                _formatter.Format(value,
                    columnType,
                    out _));

        private object RoundTripKind(string columnType,
            ColumnDefaultValue kind) =>
            Execute(columnType,
                _formatter.FormatKind(kind,
                    columnType,
                    out _));

        private static bool IsNear(DateTime actual,
            DateTime expected) =>
            Math.Abs((actual - expected).TotalHours) < 24;

        #endregion

        #region Values

        [TestMethod]
        public void TestTrueIsStoredInABit()
        {
            // Act
            var actual = RoundTrip("bit",
                SchemaValue.FromBoolean(true));

            // Assert
            Assert.AreEqual(true, actual);
        }

        [TestMethod]
        public void TestFalseIsStoredInABit()
        {
            // Act
            var actual = RoundTrip("bit",
                SchemaValue.FromBoolean(false));

            // Assert
            Assert.AreEqual(false, actual);
        }

        [TestMethod]
        public void TestANumberIsStoredInADecimal()
        {
            // Act
            var actual = RoundTrip("decimal(10,2)",
                SchemaValue.FromNumber("-12.50"));

            // Assert
            Assert.AreEqual(-12.50m, actual);
        }

        [TestMethod]
        public void TestATextWithAQuoteAndABackslashIsStoredAsItIs()
        {
            // Act
            var actual = RoundTrip("nvarchar(50)",
                SchemaValue.FromText("it's a\\b"));

            // Assert
            Assert.AreEqual("it's a\\b", actual);
        }

        [TestMethod]
        public void TestATextWithCharactersThatAreNotAsciiIsStoredAsItIs()
        {
            // Act
            var actual = RoundTrip("nvarchar(50)",
                SchemaValue.FromText("caf\u00e9"));

            // Assert
            Assert.AreEqual("caf\u00e9", actual);
        }

        [TestMethod]
        public void TestADateIsStoredInADate()
        {
            // Act
            var actual = RoundTrip("date",
                SchemaValue.FromDate(new DateTime(2020, 1, 31)));

            // Assert
            Assert.AreEqual(new DateTime(2020, 1, 31), actual);
        }

        [TestMethod]
        public void TestATimeIsStoredInATimeWithTheDigitsThatItHolds()
        {
            // Act
            var actual = RoundTrip("time(7)",
                SchemaValue.FromTime(new TimeSpan(10, 20, 30).Add(TimeSpan.FromTicks(1234567))));

            // Assert
            Assert.AreEqual(new TimeSpan(10, 20, 30).Add(TimeSpan.FromTicks(1234567)), actual);
        }

        [TestMethod]
        public void TestADateTimeIsStoredInADatetime2WithItsDigits()
        {
            // Act
            var actual = RoundTrip("datetime2(7)",
                SchemaValue.FromDateTime(Moment));

            // Assert
            Assert.AreEqual(Moment, actual);
        }

        [TestMethod]
        public void TestADateTimeIsStoredInADatetimeWithThreeDigits()
        {
            // Act
            var actual = RoundTrip("datetime",
                SchemaValue.FromDateTime(Moment));

            // Assert
            Assert.AreEqual(new DateTime(2020, 1, 31, 10, 20, 30, 123), actual);
        }

        [TestMethod]
        public void TestADateTimeIsStoredInASmalldatetimeWithoutDigits()
        {
            // Act
            var actual = RoundTrip("smalldatetime",
                SchemaValue.FromDateTime(new DateTime(2020, 1, 31, 10, 20, 0)));

            // Assert
            Assert.AreEqual(new DateTime(2020, 1, 31, 10, 20, 0), actual);
        }

        [TestMethod]
        public void TestADateTimeOffsetIsStoredWithItsOffset()
        {
            // Act
            var actual = RoundTrip("datetimeoffset(7)",
                SchemaValue.FromDateTimeOffset(new DateTimeOffset(2020, 1, 31, 10, 20, 30, TimeSpan.FromHours(1)).AddTicks(1234567)));

            // Assert
            Assert.AreEqual(new DateTimeOffset(2020, 1, 31, 10, 20, 30, TimeSpan.FromHours(1)).AddTicks(1234567), actual);
        }

        [TestMethod]
        public void TestAGuidIsStoredInAUniqueidentifier()
        {
            // Act
            var actual = RoundTrip("uniqueidentifier",
                SchemaValue.FromGuid(new Guid("6F9619FF-8B86-D011-B42D-00C04FC964FF")));

            // Assert
            Assert.AreEqual(new Guid("6F9619FF-8B86-D011-B42D-00C04FC964FF"), actual);
        }

        [TestMethod]
        public void TestABinaryIsStoredInAVarbinary()
        {
            // Act
            var actual = RoundTrip("varbinary(16)",
                SchemaValue.FromBinary(new byte[] { 0xDE, 0xAD }));

            // Assert
            CollectionAssert.AreEqual(new byte[] { 0xDE, 0xAD }, (byte[])actual);
        }

        #endregion

        #region Kinds

        [TestMethod]
        public void TestCurrentTimestampIsTheCurrentTime()
        {
            // Act
            var actual = RoundTripKind("datetime2",
                ColumnDefaultValue.CurrentTimestamp);

            // Assert
            Assert.IsTrue(IsNear((DateTime)actual, DateTime.UtcNow));
        }

        [TestMethod]
        public void TestCurrentTimestampUtcIsTheCurrentUtcTime()
        {
            // Act
            var actual = RoundTripKind("datetime2",
                ColumnDefaultValue.CurrentTimestampUtc);

            // Assert
            Assert.IsTrue(IsNear((DateTime)actual, DateTime.UtcNow));
        }

        [TestMethod]
        public void TestCurrentTimestampWithOffsetIsTheCurrentInstant()
        {
            // Act
            var actual = RoundTripKind("datetimeoffset",
                ColumnDefaultValue.CurrentTimestampWithOffset);

            // Assert
            Assert.IsTrue(IsNear(((DateTimeOffset)actual).UtcDateTime, DateTime.UtcNow));
        }

        [TestMethod]
        public void TestCurrentDateIsADate()
        {
            // Act
            var actual = RoundTripKind("date",
                ColumnDefaultValue.CurrentDate);

            // Assert
            Assert.IsTrue(IsNear((DateTime)actual, DateTime.UtcNow));
        }

        [TestMethod]
        public void TestCurrentTimeIsATime()
        {
            // Act
            var actual = RoundTripKind("time",
                ColumnDefaultValue.CurrentTime);

            // Assert
            Assert.IsInstanceOfType<TimeSpan>(actual);
        }

        [TestMethod]
        public void TestNewGuidIsAGuid()
        {
            // Act
            var actual = RoundTripKind("uniqueidentifier",
                ColumnDefaultValue.NewGuid);

            // Assert
            Assert.AreNotEqual(Guid.Empty, (Guid)actual);
        }

        [TestMethod]
        public void TestNewSequentialGuidIsAGuid()
        {
            // Act
            var actual = RoundTripKind("uniqueidentifier",
                ColumnDefaultValue.NewSequentialGuid);

            // Assert
            Assert.AreNotEqual(Guid.Empty, (Guid)actual);
        }

        [TestMethod]
        public void TestRandomNumberIsBetweenZeroAndOne()
        {
            // Act
            var actual = RoundTripKind("float",
                ColumnDefaultValue.RandomNumber);

            // Assert
            Assert.IsTrue((double)actual >= 0 && (double)actual < 1);
        }

        [TestMethod]
        public void TestCurrentUserIsAText()
        {
            // Act
            var actual = RoundTripKind("nvarchar(128)",
                ColumnDefaultValue.CurrentUser);

            // Assert
            Assert.IsFalse(string.IsNullOrEmpty((string)actual));
        }

        [TestMethod]
        public void TestCurrentDatabaseIsAText()
        {
            // Act
            var actual = RoundTripKind("nvarchar(128)",
                ColumnDefaultValue.CurrentDatabase);

            // Assert
            Assert.IsFalse(string.IsNullOrEmpty((string)actual));
        }

        #endregion
    }
}
