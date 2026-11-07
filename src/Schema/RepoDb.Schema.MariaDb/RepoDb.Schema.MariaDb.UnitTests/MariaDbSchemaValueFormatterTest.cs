#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.MariaDb.UnitTests
{
    [TestClass]
    public class MariaDbSchemaValueFormatterTest
    {
        #region Private Variables

        private readonly MariaDbSchemaValueFormatter _formatter = new MariaDbSchemaValueFormatter();

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
            Assert.IsInstanceOfType<ISchemaValueSupport>(new MariaDbSchemaComposer());
        }

        [TestMethod]
        public void TestTheComposerGivesTheFormatterOfTheProvider()
        {
            // Act
            var formatter = new MariaDbSchemaComposer().ValueFormatter;

            // Assert
            Assert.IsInstanceOfType<MariaDbSchemaValueFormatter>(formatter);
        }

        #endregion

        #region Format

        [TestMethod]
        public void TestTheFormatterDoesNotHandleTheKinds()
        {
            // Act/Assert
            Assert.IsFalse(_formatter.HandlesKinds);
        }

        [TestMethod]
        public void TestFormatTrue()
        {
            // Act
            var text = Format(SchemaValue.FromBoolean(true));

            // Assert
            Assert.AreEqual("1", text);
        }

        [TestMethod]
        public void TestFormatFalse()
        {
            // Act
            var text = Format(SchemaValue.FromBoolean(false));

            // Assert
            Assert.AreEqual("0", text);
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
        public void TestFormatATextWithABackslashDoubled()
        {
            // Act
            var text = Format(SchemaValue.FromText("a\\b"));

            // Assert
            Assert.AreEqual("'a\\\\b'", text);
        }

        [TestMethod]
        public void TestFormatADate()
        {
            // Act
            var text = Format(SchemaValue.FromDate(new DateTime(2020, 1, 31)));

            // Assert
            Assert.AreEqual("'2020-01-31'", text);
        }

        [TestMethod]
        public void TestFormatATime()
        {
            // Act
            var text = Format(SchemaValue.FromTime(new TimeSpan(10, 20, 30)));

            // Assert
            Assert.AreEqual("'10:20:30'", text);
        }

        [TestMethod]
        public void TestFormatATimeWithTheDigitsOfItsType()
        {
            // Act
            var text = Format(SchemaValue.FromTime(new TimeSpan(10, 20, 30).Add(TimeSpan.FromTicks(1234567))),
                "time(3)");

            // Assert
            Assert.AreEqual("'10:20:30.123'", text);
        }

        [TestMethod]
        public void TestFormatADateTime()
        {
            // Act
            var text = Format(CreateDateTime());

            // Assert
            Assert.AreEqual("'2020-01-31 10:20:30'", text);
        }

        [TestMethod]
        public void TestFormatADateTimeWithoutDigitsIfItsTypeHasNone()
        {
            // Act
            var text = Format(CreateDateTime(1234567),
                "datetime");

            // Assert
            Assert.AreEqual("'2020-01-31 10:20:30'", text);
        }

        [TestMethod]
        public void TestFormatADateTimeWithTheDigitsOfItsType()
        {
            // Act
            var text = Format(CreateDateTime(1234567),
                "datetime(3)");

            // Assert
            Assert.AreEqual("'2020-01-31 10:20:30.123'", text);
        }

        [TestMethod]
        public void TestFormatADateTimeWithoutMoreDigitsThanTheMaximum()
        {
            // Act
            var text = Format(CreateDateTime(1234567),
                "datetime(9)");

            // Assert
            Assert.AreEqual("'2020-01-31 10:20:30.123456'", text);
        }

        [TestMethod]
        public void TestFormatADateTimeOffsetInUtc()
        {
            // Act
            var text = Format(SchemaValue.FromDateTimeOffset(new DateTimeOffset(2020, 1, 31, 10, 20, 30, TimeSpan.FromHours(1))));

            // Assert
            Assert.AreEqual("'2020-01-31 09:20:30'", text);
        }

        [TestMethod]
        public void TestFormatADateTimeOffsetWithAWarningAboutItsOffset()
        {
            // Act
            _formatter.Format(SchemaValue.FromDateTimeOffset(new DateTimeOffset(2020, 1, 31, 10, 20, 30, TimeSpan.FromHours(1))),
                null,
                out var warning);

            // Assert
            StringAssert.Contains(warning, "its offset (+01:00) was applied and the value is written in UTC");
        }

        [TestMethod]
        public void TestFormatAGuidForATextColumn()
        {
            // Act
            var text = Format(SchemaValue.FromGuid(new Guid("6F9619FF-8B86-D011-B42D-00C04FC964FF")),
                "char(36)");

            // Assert
            Assert.AreEqual("'6f9619ff-8b86-d011-b42d-00c04fc964ff'", text);
        }

        [TestMethod]
        public void TestFormatAGuidForAColumnWithoutAType()
        {
            // Act
            var text = Format(SchemaValue.FromGuid(new Guid("6F9619FF-8B86-D011-B42D-00C04FC964FF")));

            // Assert
            Assert.AreEqual("'6f9619ff-8b86-d011-b42d-00c04fc964ff'", text);
        }

        [TestMethod]
        public void TestFormatAGuidForABinaryColumn()
        {
            // Act
            var text = Format(SchemaValue.FromGuid(new Guid("6F9619FF-8B86-D011-B42D-00C04FC964FF")),
                "binary(16)");

            // Assert
            Assert.AreEqual("(UNHEX('6f9619ff8b86d011b42d00c04fc964ff'))", text);
        }

        [TestMethod]
        public void TestFormatABinary()
        {
            // Act
            var text = Format(SchemaValue.FromBinary(new byte[] { 0xDE, 0xAD }));

            // Assert
            Assert.AreEqual("X'DEAD'", text);
        }

        #endregion
    }
}
