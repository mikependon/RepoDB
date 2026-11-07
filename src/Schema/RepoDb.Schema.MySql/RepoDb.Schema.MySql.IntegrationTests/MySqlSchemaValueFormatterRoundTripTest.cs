#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Data.Common;
using System.Linq;
using MySql.Data.MySqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;
using RepoDb.Schema.MySql.IntegrationTests.Setup;

namespace RepoDb.Schema.MySql.IntegrationTests
{
    /// <summary>
    /// Writes the typed values and the kinds of default values with the formatter, uses them as the default value of a column in a real MySQL database,
    /// inserts a row without a value for the column, and reads the value back.
    /// </summary>
    [TestClass]
    public class MySqlSchemaValueFormatterRoundTripTest
    {
        #region Private Variables

        private readonly MySqlSchemaValueFormatter _formatter = new MySqlSchemaValueFormatter();
        private static readonly DateTime Moment = new DateTime(2020, 1, 31, 10, 20, 30).AddTicks(1234567);

        #endregion

        #region Private Methods

        private static object Execute(string columnType,
            string defaultText)
        {
            Database.Initialize();
            var table = "rt_" + Guid.NewGuid().ToString("N").Substring(0, 12);
            using (var connection = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                connection.Open();
                try
                {
                    Run(connection, $"CREATE TABLE `{table}` (`id` int, `c` {columnType} DEFAULT {defaultText})");
                    Run(connection, $"INSERT INTO `{table}` (`id`) VALUES (1)");
                    return Run(connection, $"SELECT `c` FROM `{table}`");
                }
                finally
                {
                    Run(connection, $"DROP TABLE IF EXISTS `{table}`");
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
        public void TestTrueIsStoredInATinyint()
        {
            // Act
            var actual = RoundTrip("tinyint(1)",
                SchemaValue.FromBoolean(true));

            // Assert
            Assert.AreEqual(1, Convert.ToInt32(actual));
        }

        [TestMethod]
        public void TestFalseIsStoredInATinyint()
        {
            // Act
            var actual = RoundTrip("tinyint(1)",
                SchemaValue.FromBoolean(false));

            // Assert
            Assert.AreEqual(0, Convert.ToInt32(actual));
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
            var actual = RoundTrip("varchar(50)",
                SchemaValue.FromText("it's a\\b"));

            // Assert
            Assert.AreEqual("it's a\\b", actual);
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
        public void TestATimeIsStoredInATimeWithTheDigitsOfItsType()
        {
            // Act
            var actual = RoundTrip("time(3)",
                SchemaValue.FromTime(new TimeSpan(10, 20, 30).Add(TimeSpan.FromTicks(1234567))));

            // Assert
            Assert.AreEqual(new TimeSpan(0, 10, 20, 30, 123), actual);
        }

        [TestMethod]
        public void TestADateTimeIsStoredInADatetimeWithTheDigitsOfItsType()
        {
            // Act
            var actual = RoundTrip("datetime(3)",
                SchemaValue.FromDateTime(Moment));

            // Assert
            Assert.AreEqual(new DateTime(2020, 1, 31, 10, 20, 30, 123), actual);
        }

        [TestMethod]
        public void TestADateTimeIsStoredInADatetimeWithoutDigitsIfItsTypeHasNone()
        {
            // Act
            var actual = RoundTrip("datetime",
                SchemaValue.FromDateTime(Moment));

            // Assert
            Assert.AreEqual(new DateTime(2020, 1, 31, 10, 20, 30), actual);
        }

        [TestMethod]
        public void TestADateTimeOffsetIsStoredInUtc()
        {
            // Act
            var actual = RoundTrip("datetime",
                SchemaValue.FromDateTimeOffset(new DateTimeOffset(2020, 1, 31, 10, 20, 30, TimeSpan.FromHours(1))));

            // Assert
            Assert.AreEqual(new DateTime(2020, 1, 31, 9, 20, 30), actual);
        }

        [TestMethod]
        public void TestAGuidIsStoredInACharColumn()
        {
            // Act
            var actual = RoundTrip("char(36)",
                SchemaValue.FromGuid(new Guid("6F9619FF-8B86-D011-B42D-00C04FC964FF")));

            // Assert
            Assert.AreEqual(new Guid("6F9619FF-8B86-D011-B42D-00C04FC964FF"), actual);
        }

        [TestMethod]
        public void TestAGuidIsStoredInABinaryColumnInTheOrderOfItsText()
        {
            // Act
            var actual = RoundTrip("binary(16)",
                SchemaValue.FromGuid(new Guid("6F9619FF-8B86-D011-B42D-00C04FC964FF")));

            // Assert
            Assert.AreEqual(new Guid("6F9619FF-8B86-D011-B42D-00C04FC964FF").ToString("N"), string.Concat(((byte[])actual).Select(item => item.ToString("x2"))));
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
    }
}
