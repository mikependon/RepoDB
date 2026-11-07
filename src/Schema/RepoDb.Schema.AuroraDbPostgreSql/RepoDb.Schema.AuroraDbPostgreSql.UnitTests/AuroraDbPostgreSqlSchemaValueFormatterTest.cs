#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.AuroraDbPostgreSql.UnitTests
{
    [TestClass]
    public class AuroraDbPostgreSqlSchemaValueFormatterTest
    {
        #region Private Variables

        private readonly AuroraDbPostgreSqlSchemaValueFormatter _formatter = new AuroraDbPostgreSqlSchemaValueFormatter();

        #endregion

        #region Private Methods

        private string Format(SchemaValue value,
            string destinationType = null) =>
            _formatter.Format(value,
                destinationType,
                out _);

        private string FormatKind(ColumnDefaultValue kind) =>
            _formatter.FormatKind(kind,
                null,
                out _);

        private static SchemaValue CreateDateTime(long extraTicks = 0) =>
            SchemaValue.FromDateTime(new DateTime(2020, 1, 31, 10, 20, 30).AddTicks(extraTicks));

        #endregion

        #region Composer

        [TestMethod]
        public void TestTheComposerSupportsTheValues()
        {
            // Act/Assert
            Assert.IsInstanceOfType<ISchemaValueSupport>(new AuroraDbPostgreSqlSchemaComposer());
        }

        [TestMethod]
        public void TestTheComposerGivesTheFormatterOfTheProvider()
        {
            // Act
            var formatter = new AuroraDbPostgreSqlSchemaComposer().ValueFormatter;

            // Assert
            Assert.IsInstanceOfType<AuroraDbPostgreSqlSchemaValueFormatter>(formatter);
        }

        #endregion

        #region Format

        [TestMethod]
        public void TestTheFormatterHandlesTheKinds()
        {
            // Act/Assert
            Assert.IsTrue(_formatter.HandlesKinds);
        }

        [TestMethod]
        public void TestFormatTrue()
        {
            // Act
            var text = Format(SchemaValue.FromBoolean(true));

            // Assert
            Assert.AreEqual("TRUE", text);
        }

        [TestMethod]
        public void TestFormatFalse()
        {
            // Act
            var text = Format(SchemaValue.FromBoolean(false));

            // Assert
            Assert.AreEqual("FALSE", text);
        }

        [TestMethod]
        public void TestFormatANumber()
        {
            // Act
            var text = Format(SchemaValue.FromNumber("-12.50"));

            // Assert
            Assert.AreEqual("-12.50", text);
        }

        [TestMethod]
        public void TestFormatATextWithAQuote()
        {
            // Act
            var text = Format(SchemaValue.FromText("it's"));

            // Assert
            Assert.AreEqual("'it''s'", text);
        }

        [TestMethod]
        public void TestFormatATextWithABackslashAsIs()
        {
            // Act
            var text = Format(SchemaValue.FromText("a\\b"));

            // Assert
            Assert.AreEqual("'a\\b'", text);
        }

        [TestMethod]
        public void TestFormatADate()
        {
            // Act
            var text = Format(SchemaValue.FromDate(new DateTime(2020, 1, 31)));

            // Assert
            Assert.AreEqual("DATE '2020-01-31'", text);
        }

        [TestMethod]
        public void TestFormatATime()
        {
            // Act
            var text = Format(SchemaValue.FromTime(new TimeSpan(10, 20, 30)));

            // Assert
            Assert.AreEqual("TIME '10:20:30'", text);
        }

        [TestMethod]
        public void TestFormatADateTime()
        {
            // Act
            var text = Format(CreateDateTime());

            // Assert
            Assert.AreEqual("TIMESTAMP '2020-01-31 10:20:30'", text);
        }

        [TestMethod]
        public void TestFormatADateTimeWithTheMicrosecondsThatItHolds()
        {
            // Act
            var text = Format(CreateDateTime(1234567),
                "timestamp");

            // Assert
            Assert.AreEqual("TIMESTAMP '2020-01-31 10:20:30.123456'", text);
        }

        [TestMethod]
        public void TestFormatADateTimeWithTheDigitsOfItsType()
        {
            // Act
            var text = Format(CreateDateTime(1234567),
                "timestamp(3)");

            // Assert
            Assert.AreEqual("TIMESTAMP '2020-01-31 10:20:30.123'", text);
        }

        [TestMethod]
        public void TestFormatADateTimeOffset()
        {
            // Act
            var text = Format(SchemaValue.FromDateTimeOffset(new DateTimeOffset(2020, 1, 31, 10, 20, 30, TimeSpan.FromHours(1))));

            // Assert
            Assert.AreEqual("TIMESTAMP WITH TIME ZONE '2020-01-31 10:20:30+01:00'", text);
        }

        [TestMethod]
        public void TestFormatAGuid()
        {
            // Act
            var text = Format(SchemaValue.FromGuid(new Guid("6F9619FF-8B86-D011-B42D-00C04FC964FF")));

            // Assert
            Assert.AreEqual("'6f9619ff-8b86-d011-b42d-00c04fc964ff'", text);
        }

        [TestMethod]
        public void TestFormatABinary()
        {
            // Act
            var text = Format(SchemaValue.FromBinary(new byte[] { 0xDE, 0xAD }));

            // Assert
            Assert.AreEqual("'\\xDEAD'", text);
        }

        #endregion

        #region FormatKind

        [TestMethod]
        public void TestFormatKindCurrentTimestamp()
        {
            // Act
            var text = FormatKind(ColumnDefaultValue.CurrentTimestamp);

            // Assert
            Assert.AreEqual("CURRENT_TIMESTAMP", text);
        }

        [TestMethod]
        public void TestFormatKindCurrentTimestampWithOffset()
        {
            // Act
            var text = FormatKind(ColumnDefaultValue.CurrentTimestampWithOffset);

            // Assert
            Assert.AreEqual("CURRENT_TIMESTAMP", text);
        }

        [TestMethod]
        public void TestFormatKindCurrentTimestampUtc()
        {
            // Act
            var text = FormatKind(ColumnDefaultValue.CurrentTimestampUtc);

            // Assert
            Assert.AreEqual("(NOW() AT TIME ZONE 'utc')", text);
        }

        [TestMethod]
        public void TestFormatKindCurrentDate()
        {
            // Act
            var text = FormatKind(ColumnDefaultValue.CurrentDate);

            // Assert
            Assert.AreEqual("CURRENT_DATE", text);
        }

        [TestMethod]
        public void TestFormatKindCurrentTime()
        {
            // Act
            var text = FormatKind(ColumnDefaultValue.CurrentTime);

            // Assert
            Assert.AreEqual("LOCALTIME", text);
        }

        [TestMethod]
        public void TestFormatKindNewGuid()
        {
            // Act
            var text = FormatKind(ColumnDefaultValue.NewGuid);

            // Assert
            Assert.AreEqual("gen_random_uuid()", text);
        }

        [TestMethod]
        public void TestFormatKindNewSequentialGuid()
        {
            // Act
            var text = FormatKind(ColumnDefaultValue.NewSequentialGuid);

            // Assert
            Assert.AreEqual("gen_random_uuid()", text);
        }

        [TestMethod]
        public void TestFormatKindRandomNumber()
        {
            // Act
            var text = FormatKind(ColumnDefaultValue.RandomNumber);

            // Assert
            Assert.AreEqual("random()", text);
        }

        [TestMethod]
        public void TestFormatKindCurrentUser()
        {
            // Act
            var text = FormatKind(ColumnDefaultValue.CurrentUser);

            // Assert
            Assert.AreEqual("CURRENT_USER", text);
        }

        [TestMethod]
        public void TestFormatKindCurrentDatabase()
        {
            // Act
            var text = FormatKind(ColumnDefaultValue.CurrentDatabase);

            // Assert
            Assert.AreEqual("current_database()", text);
        }

        [TestMethod]
        public void TestFormatKindSequenceNextValueAsNull()
        {
            // Act
            var text = FormatKind(ColumnDefaultValue.SequenceNextValue);

            // Assert
            Assert.IsNull(text);
        }

        #endregion
    }
}
