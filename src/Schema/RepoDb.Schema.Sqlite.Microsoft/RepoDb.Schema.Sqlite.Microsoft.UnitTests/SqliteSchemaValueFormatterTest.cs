#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.Sqlite.Microsoft.UnitTests
{
    [TestClass]
    public class SqliteSchemaValueFormatterTest
    {
        #region Private Variables

        private readonly SqliteSchemaValueFormatter _formatter = new SqliteSchemaValueFormatter();

        #endregion

        #region Private Methods

        private string FormatKind(ColumnDefaultValue kind,
            string destinationType = null) =>
            _formatter.FormatKind(kind,
                destinationType,
                out _);

        private string FormatKind(ColumnDefaultValue kind,
            out string warning) =>
            _formatter.FormatKind(kind,
                null,
                out warning);

        private string Format(SchemaValue value) =>
            _formatter.Format(value,
                null,
                out _);

        #endregion

        #region Public Methods

        [TestMethod]
        public void TestSqliteSchemaValueFormatterHandlesTheKinds()
        {
            // Act
            var handles = _formatter.HandlesKinds;

            // Assert
            Assert.IsTrue(handles);
        }

        [TestMethod]
        public void TestSqliteSchemaValueFormatterComposerSupportsTheValues()
        {
            // Act
            var composer = new SqliteSchemaComposer();

            // Assert
            Assert.IsInstanceOfType<ISchemaValueSupport>(composer);
        }

        [TestMethod]
        public void TestSqliteSchemaValueFormatterComposerGivesTheFormatterOfTheProvider()
        {
            // Act
            var formatter = new SqliteSchemaComposer().ValueFormatter;

            // Assert
            Assert.IsInstanceOfType<SqliteSchemaValueFormatter>(formatter);
        }

        [TestMethod]
        public void TestFormatKindCurrentTimestamp()
        {
            // Act
            var text = FormatKind(ColumnDefaultValue.CurrentTimestamp);

            // Assert
            Assert.AreEqual("(datetime('now','localtime'))", text);
        }

        [TestMethod]
        public void TestFormatKindCurrentTimestampUtc()
        {
            // Act
            var text = FormatKind(ColumnDefaultValue.CurrentTimestampUtc);

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
        public void TestFormatKindCurrentTimestampWithOffsetGivesAWarning()
        {
            // Act
            FormatKind(ColumnDefaultValue.CurrentTimestampWithOffset,
                out var warning);

            // Assert
            Assert.IsNotNull(warning);
        }

        [TestMethod]
        public void TestFormatKindCurrentDate()
        {
            // Act
            var text = FormatKind(ColumnDefaultValue.CurrentDate);

            // Assert
            Assert.AreEqual("(date('now','localtime'))", text);
        }

        [TestMethod]
        public void TestFormatKindCurrentTime()
        {
            // Act
            var text = FormatKind(ColumnDefaultValue.CurrentTime);

            // Assert
            Assert.AreEqual("(time('now','localtime'))", text);
        }

        [TestMethod]
        public void TestFormatKindNewGuid()
        {
            // Act
            var text = FormatKind(ColumnDefaultValue.NewGuid);

            // Assert
            Assert.AreEqual("(lower(hex(randomblob(16))))", text);
        }

        [TestMethod]
        public void TestFormatKindNewSequentialGuid()
        {
            // Act
            var text = FormatKind(ColumnDefaultValue.NewSequentialGuid);

            // Assert
            Assert.AreEqual("(lower(hex(randomblob(16))))", text);
        }

        [TestMethod]
        public void TestFormatKindRandomNumber()
        {
            // Act
            var text = FormatKind(ColumnDefaultValue.RandomNumber);

            // Assert
            Assert.AreEqual("(random())", text);
        }

        [TestMethod]
        public void TestFormatKindCurrentUser()
        {
            // Act
            var text = FormatKind(ColumnDefaultValue.CurrentUser);

            // Assert
            Assert.IsNull(text);
        }

        [TestMethod]
        public void TestFormatKindCurrentDatabase()
        {
            // Act
            var text = FormatKind(ColumnDefaultValue.CurrentDatabase);

            // Assert
            Assert.IsNull(text);
        }

        [TestMethod]
        public void TestFormatKindOfASequenceHasNoEquivalent()
        {
            // Act
            var text = FormatKind(ColumnDefaultValue.SequenceNextValue);

            // Assert
            Assert.IsNull(text);
        }

        [TestMethod]
        public void TestFormatKindOfALiteralHasNoEquivalent()
        {
            // Act
            var text = FormatKind(ColumnDefaultValue.Literal);

            // Assert
            Assert.IsNull(text);
        }

        [TestMethod]
        public void TestFormatBinary()
        {
            // Act
            var text = Format(SchemaValue.FromBinary(new byte[] { 0xDE, 0xAD }));

            // Assert
            Assert.AreEqual("X'DEAD'", text);
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
        public void TestFormatADateAsAQuotedText()
        {
            // Act
            var text = Format(SchemaValue.FromDate(new DateTime(2020, 1, 31)));

            // Assert
            Assert.AreEqual("'2020-01-31'", text);
        }

        [TestMethod]
        public void TestFormatADateAndTimeAsAQuotedText()
        {
            // Act
            var text = Format(SchemaValue.FromDateTime(new DateTime(2020, 1, 31, 10, 20, 30)));

            // Assert
            Assert.AreEqual("'2020-01-31 10:20:30'", text);
        }

        [TestMethod]
        public void TestFormatADateAndTimeWithAnOffsetInUtc()
        {
            // Act
            var text = Format(SchemaValue.FromDateTimeOffset(new DateTimeOffset(2020, 1, 31, 10, 20, 30, TimeSpan.FromHours(2))));

            // Assert
            Assert.AreEqual("'2020-01-31 08:20:30'", text);
        }

        [TestMethod]
        public void TestFormatATimeAsAQuotedText()
        {
            // Act
            var text = Format(SchemaValue.FromTime(new TimeSpan(10, 20, 30)));

            // Assert
            Assert.AreEqual("'10:20:30'", text);
        }

        #endregion
    }
}
